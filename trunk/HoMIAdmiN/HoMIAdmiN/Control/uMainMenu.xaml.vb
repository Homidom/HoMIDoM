Public Class uMainMenu
    Public Event ChangeMenu(ByVal IndexMenu As Integer)


    Private Sub Mnu1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Mnu1.MouseDown
        RaiseEvent ChangeMenu(0)
    End Sub

    Private Sub Mnu2_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Mnu2.MouseDown
        RaiseEvent ChangeMenu(1)
    End Sub

    Private Sub Mnu3_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Mnu3.MouseDown
        RaiseEvent ChangeMenu(2)
    End Sub

    Private Sub ImgMnu4_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ImgMnu4.MouseDown
        RaiseEvent ChangeMenu(3)
    End Sub

    Private Sub ImgMnu5_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ImgMnu5.MouseDown
        RaiseEvent ChangeMenu(4)
    End Sub

    Private Sub ImgMnu6_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ImgMnu6.MouseDown
        RaiseEvent ChangeMenu(5)
    End Sub

    Private Sub ImgMnu7_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ImgMnu7.MouseDown
        RaiseEvent ChangeMenu(6)
    End Sub

    Private Sub ImgMnu8_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ImgMnu8.MouseDown
        RaiseEvent ChangeMenu(7)
    End Sub
End Class
