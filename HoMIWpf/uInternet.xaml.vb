Imports System.Reflection

Partial Public Class uInternet
    Dim _Url As String 'Url internet de la page à afficher 

    Private Sub uInternet_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        x.Navigate(New Uri(_Url))
        x.Height = Me.Height - 30
    End Sub

    Public Sub New(ByVal Url As String)
        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        _Url = Url
        'AddHandler x.Navigating, New NavigatingCancelEventHandler(AddressOf wb_Navigating)
    End Sub

    Private Sub Image1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Image1.MouseDown
        x.GoBack()
    End Sub

    Private Sub Image2_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Image2.MouseDown
        x.GoForward()
    End Sub

    'Private Sub wb_Navigating(ByVal sender As Object, ByVal e As System.Windows.Navigation.NavigationEventArgs)
    '    Dim wb As Controls.WebBrowser = sender
    '    SuppressScriptErrors(wb, True)
    'End Sub

    Sub SuppressScriptErrors(ByVal wb As Controls.WebBrowser, ByVal Hide As Boolean)
        Dim fiComWebBrowser As FieldInfo = GetType(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance Or BindingFlags.NonPublic)
        If fiComWebBrowser Is Nothing Then
            Return
        End If
        Dim objComWebBrowser As Object = fiComWebBrowser.GetValue(wb)
        If objComWebBrowser Is Nothing Then
            Return
        End If
        objComWebBrowser.[GetType]().InvokeMember("Silent", BindingFlags.SetProperty, Nothing, objComWebBrowser, New Object() {Hide})
    End Sub

    Private Sub x_Navigated(ByVal sender As Object, ByVal e As System.Windows.Navigation.NavigationEventArgs) Handles x.Navigated
        SuppressScriptErrors(x, True)
    End Sub

    Private Sub x_Navigating(ByVal sender As Object, ByVal e As System.Windows.Navigation.NavigatingCancelEventArgs) Handles x.Navigating
        SuppressScriptErrors(x, True)

    End Sub
End Class
