Public Class FrmConfigProperty

    Private Sub BtnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnCancel.Click
        Me.Close()
    End Sub

    Private Sub BtnOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnOk.Click
        FRMMere.Obj.Longitude = CDbl(TxtLong.Text.Replace(".", ","))
        FRMMere.Obj.Latitude = CDbl(TxtLat.Text.Replace(".", ","))
        FRMMere.Obj.HeureCorrectionLever = CInt(HCL.Text)
        FRMMere.Obj.HeureCorrectionCoucher = CInt(HCC.Text)
        Me.Close()
    End Sub

    Private Sub FrmConfigProperty_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        TxtLat.Text = FRMMere.Obj.Latitude
        TxtLong.Text = FRMMere.Obj.Longitude
        HCL.Text = FRMMere.Obj.HeureCorrectionLever
        HCC.Text = FRMMere.Obj.HeureCorrectionCoucher
    End Sub
End Class