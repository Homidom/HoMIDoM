Public Class uOnOff

    Public Event ClickOn(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    Public Event ClickOff(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)

    Public Property ContentOn As String
        Get
            Return BtnOn.Content
        End Get
        Set(ByVal value As String)
            BtnOn.Content = value
        End Set
    End Property

    Public Property ContentOff As String
        Get
            Return BtnOff.Content
        End Get
        Set(ByVal value As String)
            BtnOff.Content = value
        End Set
    End Property

    Private Sub BtnOn_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOn.Click
        RaiseEvent ClickOn(sender, e)
    End Sub

    Private Sub BtnOff_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOff.Click
        RaiseEvent ClickOff(sender, e)
    End Sub
End Class
