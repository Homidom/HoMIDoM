Public Class uEditValue

    Public Event ChangeValue(ByVal Value As String)

    Private Sub BtnOk_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOk.Click
        RaiseEvent ChangeValue(TxtValue.Text)
    End Sub

    Private Sub uEditValue_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        TxtValue.Text = ""
    End Sub
End Class
