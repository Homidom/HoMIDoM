Partial Public Class uConfigServer
    Public Event CloseMe(ByVal MyObject As Object)

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        Window1.Obj.Longitude = CDbl(TxtLong.Text.Replace(".", ","))
        Window1.Obj.Latitude = CDbl(TxtLat.Text.Replace(".", ","))
        Window1.Obj.HeureCorrectionLever = CInt(HCL.Text)
        Window1.Obj.HeureCorrectionCoucher = CInt(HCC.Text)
        RaiseEvent CloseMe(Me)
    End Sub

    Private Sub BtnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnCancel.Click
        RaiseEvent CloseMe(Me)
    End Sub

    Public Sub New()

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        TxtLat.Text = Window1.Obj.Latitude
        TxtLong.Text = Window1.Obj.Longitude
        HCL.Text = Window1.Obj.HeureCorrectionLever
        HCC.Text = Window1.Obj.HeureCorrectionCoucher
    End Sub
End Class
