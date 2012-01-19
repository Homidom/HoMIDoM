Imports System.Data
Imports System.IO

Partial Public Class uLog
    Public Event CloseMe(ByVal MyObject As Object)

    Private Sub BtnRefresh_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnRefresh.Click
        RefreshLog()
    End Sub

    Private Sub RefreshLog()
        Try
            Me.Cursor = Cursors.Wait
            If IsConnect = True Then
                If File.Exists("log.xml") Then
                    File.Delete("log.xml")
                End If

                Dim TargetFile As StreamWriter
                TargetFile = New StreamWriter("log.xml", True)
                TargetFile.Write(myservice.ReturnLog)
                TargetFile.Close()

                Dim ds As DataSet = New DataSet("Table")
                ds.ReadXml("log.xml")
                DGW.ItemsSource = ds.Tables(0).DefaultView

                File.Delete("log.xml")
            End If
            Me.Cursor = Nothing
        Catch ex As Exception
            MessageBox.Show("Erreur" & ex.ToString)
        End Try
    End Sub


    Public Sub New()

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        RefreshLog()

    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me)
    End Sub

    Private Sub DGW_LoadingRow(ByVal sender As Object, ByVal e As System.Windows.Controls.DataGridRowEventArgs) Handles DGW.LoadingRow
        Exit Sub
        If e Is Nothing Then Exit Sub
        If e.Row IsNot Nothing Then
            If e.Row.DataContext IsNot Nothing Then
                If e.Row.Item IsNot Nothing Then
                    Dim RowDataContaxt As System.Data.DataRowView = e.Row.DataContext
                    If RowDataContaxt IsNot Nothing Then
                        If RowDataContaxt.Row IsNot Nothing Then
                            If RowDataContaxt.Row.ItemArray IsNot Nothing Then
                                Select Case RowDataContaxt.Row.Item(1).ToString
                                    Case "1"
                                        RowDataContaxt.Row.Item(1) = "INFO"
                                        'e.Row.Background = Brushes.White
                                    Case "2"
                                        RowDataContaxt.Row.Item(1) = "ACTION"
                                        'e.Row.Background = Brushes.White
                                    Case "3"
                                        RowDataContaxt.Row.Item(1) = "MESSAGE"
                                        'e.Row.Background = Brushes.White
                                    Case "4"
                                        RowDataContaxt.Row.Item(1) = "VALEUR CHANGE"
                                        'e.Row.Background = Brushes.White
                                    Case "5"
                                        RowDataContaxt.Row.Item(1) = "VALEUR INCHANGE"
                                        'e.Row.Background = Brushes.White
                                    Case "6"
                                        RowDataContaxt.Row.Item(1) = "VALEUR INCHANGE PRECISION"
                                        'e.Row.Background = Brushes.White
                                    Case "7"
                                        RowDataContaxt.Row.Item(1) = "VALEUR INCHANGE LASTETAT"
                                        'e.Row.Background = Brushes.White
                                    Case "8"
                                        RowDataContaxt.Row.Item(1) = "ERREUR"
                                        e.Row.Background = Brushes.Red
                                    Case "9"
                                        RowDataContaxt.Row.Item(1) = "ERREUR CRITIQUE"
                                        e.Row.Background = Brushes.Red
                                    Case "10"
                                        RowDataContaxt.Row.Item(1) = "DEBUG"
                                        e.Row.Background = Brushes.Yellow
                                End Select
                                Select Case RowDataContaxt.Row.Item(2).ToString
                                    Case "1"
                                        RowDataContaxt.Row.Item(2) = "SERVEUR"
                                    Case "2"
                                        RowDataContaxt.Row.Item(2) = "SCRIPT"
                                    Case "3"
                                        RowDataContaxt.Row.Item(2) = "TRIGGER"
                                    Case "4"
                                        RowDataContaxt.Row.Item(2) = "DEVICE"
                                    Case "5"
                                        RowDataContaxt.Row.Item(2) = "DRIVER"
                                    Case "6"
                                        RowDataContaxt.Row.Item(2) = "SOAP"
                                    Case "7"
                                        RowDataContaxt.Row.Item(2) = "CLIENT"
                                End Select
                            End If
                        End If
                    End If
                End If
            End If
        End If
    End Sub
End Class
