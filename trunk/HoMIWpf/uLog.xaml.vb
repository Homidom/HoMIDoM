Imports System.Data
Imports System.IO
Imports System.Collections.ObjectModel
Imports System.Xml
Imports System.Web.HttpUtility

Partial Public Class uLog
    Public Event CloseMe(ByVal uid As String)
    Public ligneLog As New ObservableCollection(Of Dictionary(Of String, Object))
    Dim keys As New List(Of String)
    Dim headers As String() = {"datetime", "typesource", "source", "fonction", "message"}
    Dim _IsClient As Boolean = False

    Private Sub BtnRefresh_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnRefresh.Click
        RefreshLog()
    End Sub

    Private Sub RefreshLog()
        Try
            'Variables
            Dim _LigneIgnorees As Integer = 0
            Me.Cursor = Cursors.Wait
            ligneLog.Clear()

            If IsConnect = True Then
                Dim TargetFile As StreamWriter
                TargetFile = New StreamWriter("log.txt", False)
                If _IsClient Then
                    Label1.Content = "Log du Client"
                    TargetFile.Write(ReturnLog)
                Else
                    Label1.Content = "Log du Serveur"
                    TargetFile.Write(myService.ReturnLog)
                End If
                TargetFile.Close()
                TargetFile.Dispose()
                TargetFile = Nothing

                Dim tr As TextReader = New StreamReader("log.txt")
                Dim lineCount As Integer = 1

                While tr.Peek() >= 0
                    Try
                        Dim line As String = tr.ReadLine()

                        Dim tmp As String() = line.Trim.Split(vbTab)
                        Dim data As String() = Nothing

                        If tmp.Length >= 5 Then
                            If tmp(4).Length > 255 And tmp(4).Length > 0 Then tmp(4) = Mid(tmp(4), 1, 255)
                            data = tmp
                        Else
                            _LigneIgnorees += 1
                        End If

                        ' creates a dictionary where the column name is the key and
                        '    and the data is the value
                        Dim sensorData As New Dictionary(Of String, Object)

                        If data IsNot Nothing Then
                            For i As Integer = 0 To data.Length - 1
                                sensorData(keys(i)) = data(i)
                            Next
                            ligneLog.Add(sensorData)
                        End If

                        sensorData = Nothing
                        lineCount += 1
                    Catch ex As Exception
                        MessageBox.Show("Erreur lors de la ligne du fichier log: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
                    End Try
                End While

                tr.Dispose()
                tr = Nothing
            End If

            Try
                DGW.DataContext = ligneLog
            Catch ex As Exception
                MessageBox.Show("Erreur: " & ex.Message)
            End Try

            If File.Exists("log.txt") Then File.Delete("log.txt")
            Me.Cursor = Nothing
        Catch ex As Exception
            MessageBox.Show("Erreur lors de la récuppération du fichier log: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub CreateGridColumn(ByVal headerText As String)
        Try
            Dim col1 As DataGridTextColumn = New DataGridTextColumn()
            col1.Header = headerText
            col1.Binding = New Binding(String.Format("[{0}]", headerText))
            DGW.Columns.Add(col1)
        Catch ex As Exception
            MessageBox.Show("Erreur Sub CreateGridColumn: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Public Sub New(Optional ByVal IsClient As Boolean = False)
        Try

            ' Cet appel est requis par le Concepteur Windows Form.
            InitializeComponent()

            _IsClient = IsClient

            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            For Each h As String In headers
                CreateGridColumn(h)
                keys.Add(h)
            Next

            RefreshLog()
        Catch ex As Exception
            MessageBox.Show("Erreur lors sur la fonction New de uLog: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me.Uid)
    End Sub

    Private Sub DGW_LoadingRow(ByVal sender As Object, ByVal e As System.Windows.Controls.DataGridRowEventArgs) Handles DGW.LoadingRow
        Try
            Dim RowDataContaxt As Dictionary(Of String, Object) = TryCast(e.Row.DataContext, Dictionary(Of String, Object))
            If RowDataContaxt IsNot Nothing Then
                Select Case RowDataContaxt(keys(1))
                    Case "INFO"
                        e.Row.Background = Brushes.White
                    Case "ACTION"
                        e.Row.Background = Brushes.White
                    Case "MESSAGE"
                        e.Row.Background = Brushes.White
                    Case "VALEUR CHANGE"
                        e.Row.Background = Brushes.White
                    Case "VALEUR INCHANGE"
                        e.Row.Background = Brushes.White
                    Case "VALEUR INCHANGE PRECISION"
                        e.Row.Background = Brushes.White
                    Case "VALEUR INCHANGE LASTETAT"
                        e.Row.Background = Brushes.White
                    Case "ERREUR"
                        e.Row.Background = Brushes.Red
                    Case "ERREUR CRITIQUE"
                        e.Row.Background = Brushes.Red
                    Case "DEBUG"
                        e.Row.Background = Brushes.Yellow
                End Select
            End If

        Catch ex As Exception
            MessageBox.Show("Erreur DGW_LoadingRow: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub ExportLog()
        Try
            ' Configure open file dialog box
            Dim dlg As New Microsoft.Win32.SaveFileDialog()
            dlg.FileName = "Log" ' Default file name
            dlg.DefaultExt = ".txt" ' Default file extension
            dlg.Filter = "Fichier log (.txt)|*.txt" ' Filter files by extension
            ' Show open file dialog box
            Dim result As Boolean = dlg.ShowDialog()
            ' Process open file dialog box results
            If result = True Then
                ' Open document
                Dim filename As String = dlg.FileName
                Dim retour As String = myService.ReturnLog
                If retour.StartsWith("ERREUR") Then
                    MessageBox.Show(retour, "Erreur ReturnLog", MessageBoxButton.OK, MessageBoxImage.Error)
                Else
                    Dim TargetFile As StreamWriter
                    TargetFile = New StreamWriter(filename, False)
                    TargetFile.Write(retour)
                    TargetFile.Close()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur ExportLog: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnExportLog_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnExportLog.Click
        ExportLog()
    End Sub

    ''' <summary>renvoi le fichier log suivant une requête xml si besoin</summary>
    ''' <param name="Requete"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function ReturnLog(Optional ByVal Requete As String = "") As String
        Try
            Dim retour As String = ""
            If String.IsNullOrEmpty(Requete) = True Then
                Dim SR As New StreamReader(_MonRepertoire & "\logs\log_" & DateAndTime.Now.ToString("yyyyMMdd") & ".txt", FileMode.Open)
                retour = SR.ReadToEnd()
                retour = HtmlDecode(retour)
                SR.Close()
                SR.Dispose()
                SR = Nothing
            Else
                'creation d'une nouvelle instance du membre xmldocument
                Dim XmlDoc As XmlDocument = New XmlDocument()
                XmlDoc.Load(_MonRepertoire & "\logs\log.xml")
            End If
            If retour.Length > 1000000 Then
                Dim retour2 As String = Mid(retour, retour.Length - 1000001, 1000000)
                retour = "Erreur, trop de ligne à traiter depuis le log seules les dernières lignes seront affichées, merci de consulter le fichier sur le serveur par en avoir la totalité!!" & vbCrLf & vbCrLf & retour2
                Return retour
            End If
            Return retour
        Catch ex As Exception
            ReturnLog = "Erreur lors de la récupération du log: " & ex.ToString
        End Try
    End Function

End Class
