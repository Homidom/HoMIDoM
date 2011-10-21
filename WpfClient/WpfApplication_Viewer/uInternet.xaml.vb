Imports System.Reflection

Partial Public Class uInternet
    Dim _Url As String 'Url internet de la page à afficher 

    Private Sub uInternet_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Try
            x.Navigate(New Uri(_Url))
            x.Height = Me.Height - 30
        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'ouverture de la page internet " & _Url & " erreur: " & ex.ToString, "Erreur")
        End Try
    End Sub

    Public Sub New(ByVal Url As String)
        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()
        AddHandler x.Navigating, New NavigatingCancelEventHandler(AddressOf wb_Navigating)
        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        _Url = Url
    End Sub

    Private Sub Image1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Image1.MouseDown
        If x.CanGoBack Then x.GoBack()
    End Sub

    Private Sub Image2_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Image2.MouseDown
        If x.CanGoForward Then x.GoForward()
    End Sub

    Private Sub wb_Navigating(ByVal sender As Object, ByVal e As NavigatingCancelEventArgs)
        Dim wb As Controls.WebBrowser = sender
        SuppressScriptErrors(wb, True)
    End Sub

    Public Sub SuppressScriptErrors(ByVal wb As System.Windows.Controls.WebBrowser, ByVal Hide As Boolean)
        Dim fi As FieldInfo = GetType(System.Windows.Controls.WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance Or BindingFlags.NonPublic)

        If fi IsNot Nothing Then
            Dim browser As Object = fi.GetValue(wb)

            If browser IsNot Nothing Then

                browser.[GetType]().InvokeMember("Silent", BindingFlags.SetProperty, Nothing, browser, New Object() {Hide})
            End If
        End If
    End Sub

    Private Sub uInternet_SizeChanged(ByVal sender As Object, ByVal e As System.Windows.SizeChangedEventArgs) Handles Me.SizeChanged
        x.Height = Me.Height - 30
    End Sub

    Protected Overrides Sub Finalize()
        x.Dispose()
        MyBase.Finalize()
    End Sub
End Class
