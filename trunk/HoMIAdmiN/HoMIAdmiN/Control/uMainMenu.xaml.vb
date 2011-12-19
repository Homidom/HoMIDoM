Public Class uMainMenu
    Public Event ChangeMenu(ByVal IndexMenu As Integer)
    Public Event Delete(ByVal IndexMenu As Integer)
    Public Event Create(ByVal IndexMenu As Integer)
    Public Event Edit(ByVal IndexMenu As Integer)

    Private Sub Mnu1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ImgMnu1.MouseDown, ImgMnu2.MouseDown, ImgMnu3.MouseDown, ImgMnu4.MouseDown, ImgMnu5.MouseDown, ImgMnu6.MouseDown, ImgMnu7.MouseDown, ImgMnu8.MouseDown
        RaiseEvent ChangeMenu(sender.tag)
    End Sub

    Private Sub Delete_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Img102.MouseDown, Img202.MouseDown, Img302.MouseDown, Img402.MouseDown, Img502.MouseDown, Img602.MouseDown, Img702.MouseDown, Img802.MouseDown
        RaiseEvent Delete(sender.tag)
    End Sub

    Private Sub Create_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Img100.MouseDown, Img200.MouseDown, Img300.MouseDown, Img400.MouseDown, Img600.MouseDown, Img700.MouseDown, Img800.MouseDown, Img500.MouseDown, Img510.MouseDown
        RaiseEvent Create(sender.tag)
    End Sub

    Private Sub Edit_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Img101.MouseDown, Img201.MouseDown, Img301.MouseDown, Img401.MouseDown, Img501.MouseDown, Img601.MouseDown, Img701.MouseDown, Img801.MouseDown
        RaiseEvent Edit(sender.tag)
    End Sub
End Class
