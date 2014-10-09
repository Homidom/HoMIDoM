Imports System.IO
Imports System.Xml
Imports System.Web.HttpUtility

Public Class WLog
    Dim _IsClient As Boolean = False
    Dim _NbView As Integer = 16
    ' Used when manually scrolling.
    Private scrollTarget As Point
    Private scrollStartPoint As Point
    Private scrollStartOffset As Point

    Private Sub BtnRefresh_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles BtnRefresh.Click
        RefreshLog()
    End Sub

    Private Sub RefreshLog()
        Try
            'Variables
            Me.Cursor = Cursors.Wait

            If Stk Is Nothing Then
                Exit Sub
            Else
                Stk.Children.Clear()
            End If


            If IsConnect = True Then
                Dim TargetFile As StreamWriter
                TargetFile = New StreamWriter("log.txt", False)
                If _IsClient Then
                    Label1.Content = "Log du Client"
                    TargetFile.Write(ReturnLog)
                Else
                    Label1.Content = "Log du Serveur"
                    TargetFile.Write(myService.ReturnLog(_NbView))
                End If
                TargetFile.Close()
                TargetFile.Dispose()
                TargetFile = Nothing

                Dim tr As TextReader = New StreamReader("log.txt")

                While tr.Peek() >= 0
                    Try
                        Dim line As String = tr.ReadLine()

                        Dim tmp As String() = line.Trim.Split(vbTab)
                        Dim data As String() = Nothing

                        If tmp.Length >= 5 Then
                            If tmp(4).Length > 255 And tmp(4).Length > 0 Then tmp(4) = Mid(tmp(4), 1, 255)
                            data = tmp

                            If data IsNot Nothing Then
                                Dim widLog As New UWidgetLog(data(1), data(2), data(3), data(4), data(0))
                                Stk.Children.Add(WidLog)
                            End If
                        End If

                    Catch ex As Exception
                        MessageBox.Show("Erreur lors de la ligne du fichier log: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
                    End Try
                End While

                tr.Dispose()
                tr = Nothing
            End If

            If File.Exists("log.txt") Then File.Delete("log.txt")
            Me.Cursor = Nothing
        Catch ex As Exception
            MessageBox.Show("Erreur lors de la récuppération du fichier log: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Public Sub New(Optional ByVal IsClient As Boolean = False)

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        _IsClient = IsClient
        RefreshLog()
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

    Private Sub BtnClose_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        DialogResult = True
    End Sub

#Region "Barre du bas"

    Private Sub ScrollViewer1_PreviewMouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ScrollViewer1.PreviewMouseDown
        scrollStartPoint = e.GetPosition(Me)
        scrollStartOffset.Y = ScrollViewer1.VerticalOffset
    End Sub

    Private Sub ScrollViewer1_PreviewMouseMove(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles ScrollViewer1.PreviewMouseMove
        If e.LeftButton = MouseButtonState.Pressed Then
            Dim currentPoint As Point = e.GetPosition(Me)
            Dim delta As New Point(scrollStartPoint.X - currentPoint.X, scrollStartPoint.Y - currentPoint.Y)
            scrollTarget.Y = scrollStartOffset.Y + delta.Y
            ScrollToPosition(ScrollViewer1, currentPoint.X, scrollTarget.Y, m_SpeedTouch)
        End If
    End Sub
#End Region

    Private Sub CbView_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles CbView.SelectionChanged
        Select Case CbView.SelectedIndex
            Case 0
                _NbView = 0
            Case 1
                _NbView = 16
            Case 2
                _NbView = 32
            Case 3
                _NbView = 64
        End Select

        RefreshLog()
    End Sub
End Class
