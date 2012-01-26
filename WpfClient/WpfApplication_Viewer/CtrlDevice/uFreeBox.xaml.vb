Public Class uFreeBox
    Public Event ButtonClick(ByVal Touche As String)

    Private Sub Touche_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Btn0.Click, Btn1.Click, Btn2.Click, Btn3.Click, Btn4.Click, Btn5.Click, Btn6.Click, Btn7.Click, Btn8.Click, Btn9.Click, BtnVolP.Click, BtnVolM.Click, BtnVolMute.Click, BtnProgP.Click, BtnProgM.Click
        RaiseEvent ButtonClick(sender.tag)
    End Sub
End Class
