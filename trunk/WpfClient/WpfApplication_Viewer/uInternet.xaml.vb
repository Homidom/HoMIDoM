Partial Public Class uInternet
    Dim _Url As String 'Url internet de la page à afficher 
    Dim x As WebBrowser = New WebBrowser
    Dim flag As Boolean

    Private Sub uInternet_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Grid1.Width = Me.Width
        Grid1.Height = Me.Height - 28
        x.Navigate(New Uri(_Url))
        Grid1.Children.Add(x)
    End Sub

    Public Sub New(ByVal Url As String)

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        _Url = Url
    End Sub

    Private Sub Image1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Image1.MouseDown
        x.GoBack()
    End Sub

    Private Sub Image2_ImageFailed(ByVal sender As System.Object, ByVal e As System.Windows.ExceptionRoutedEventArgs) Handles Image2.ImageFailed
        x.GoForward()

    End Sub
End Class
