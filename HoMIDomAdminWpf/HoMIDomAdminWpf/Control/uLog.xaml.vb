Partial Public Class uLog
    Public Event CloseMe(ByVal MyObject As Object)

    Private Sub BtnRefresh_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnRefresh.Click
        TxtLog.Text = Nothing
        RefreshLog()
    End Sub

    Private Sub RefreshLog()
        TxtLog.Text = Window1.myService.ReturnLog()
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
End Class
