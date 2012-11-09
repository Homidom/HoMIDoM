Public Class uKeyPad

    Dim _Value As Integer = Nothing

    Public Event KeyPadOk(ByVal Value As Integer)

    Private Sub Btn7_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Btn7.Click, Btn8.Click, Btn9.Click, Btn6.Click, Btn5.Click, Btn4.Click, Btn1.Click, Btn2.Click, Btn3.Click, Btn0.Click
        Dim x As Button = sender
        TxtSaisie.Text &= x.Content
        _Value = CInt(TxtSaisie.Text)
    End Sub

    Private Sub BtnC_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnC.Click
        TxtSaisie.Text = ""
    End Sub

    Private Sub BtnOK_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        RaiseEvent KeyPadOk(_Value)
    End Sub

End Class
