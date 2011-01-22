Public Class FrmHisto

    Private Sub BtnNewHisto_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnNewHisto.Click
        'Dim x As New CtrlHisto
        'x.ID = eHomeApi.eHomeApi.Api.GenerateGUID
        'x.Tag = PnlHisto.Controls.Count
        'AddHandler x.Delete, AddressOf DeleteHisto
        'PnlHisto.Controls.Add(x)
    End Sub

    Private Sub FrmHisto_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'TxtRetention.Text = FRMMere.Obj.HistorisationDayMax

        'PnlHisto.Controls.Clear()
        'For i As Integer = 0 To FRMMere.Obj.Historisations.Count - 1
        '    Dim x As New CtrlHisto
        '    With x
        '        .Tag = i
        '        .ID = FRMMere.Obj.Historisations.Item(i).id
        '        .Enable = FRMMere.Obj.Historisations.Item(i).enable
        '        .DeviceId = FRMMere.Obj.Historisations.Item(i).deviceid
        '        .PropertyDevice = FRMMere.Obj.Historisations.Item(i).PropertyDevice
        '        AddHandler .Delete, AddressOf DeleteHisto
        '    End With
        '    PnlHisto.Controls.Add(x)
        'Next
    End Sub

    Private Sub DeleteHisto(ByVal Objet As Object)
        PnlHisto.Controls.RemoveAt(Objet.tag)
    End Sub

    Private Sub TxtRetention_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtRetention.TextChanged
        'If IsNumeric(TxtRetention.Text) = False Then
        '    MessageBox.Show("Le nombre de jour de rétention doit être numérique!", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
        '    TxtRetention.Text = FRMMere.Obj.HistorisationDayMax
        '    Exit Sub
        'End If
    End Sub

    Private Sub BtnOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnOk.Click
        'FRMMere.Obj.HistorisationDayMax = TxtRetention.Text

        'Dim _list As New ArrayList
        'For i As Integer = 0 To PnlHisto.Controls.Count - 1
        '    Dim x As New eHomeApi.eHomeApi.Historisation
        '    Dim y As New CtrlHisto
        '    y = PnlHisto.Controls.Item(i)
        '    With x
        '        .ID = y.ID
        '        .Enable = y.Enable
        '        .DeviceId = y.DeviceId
        '        .PropertyDevice = y.PropertyDevice
        '    End With
        '    _list.Add(x)
        '    x = Nothing
        '    y = Nothing
        'Next
        'FRMMere.Obj.Historisations = _list
        '_list = Nothing

        'Me.Close()
    End Sub

    Private Sub BtnAnnuler_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnAnnuler.Click
        Me.Close()
    End Sub
End Class