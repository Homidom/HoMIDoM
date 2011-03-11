Partial Public Class uConfigServer
    Public Event CloseMe(ByVal MyObject As Object)

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        Window1.myService.SetLongitude(CDbl(TxtLong.Text.Replace(".", ",")))
        Window1.myService.SetLatitude(CDbl(TxtLat.Text.Replace(".", ",")))
        Window1.myService.SetHeureCorrectionLever(CInt(HCL.Text))
        Window1.myService.SetHeureCorrectionCoucher(CInt(HCC.Text))
        RaiseEvent CloseMe(Me)
    End Sub

    Private Sub BtnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnCancel.Click
        RaiseEvent CloseMe(Me)
    End Sub

    Public Sub New()

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        TxtLat.Text = Window1.myService.GetLatitude
        TxtLong.Text = Window1.myService.GetLongitude
        HCL.Text = Window1.myService.GetHeureCorrectionLever
        HCC.Text = Window1.myService.GetHeureCorrectionCoucher
    End Sub
End Class
