Imports HoMIDom.HoMIDom
Imports System.IO

Public Class FrmZone
    Public MyID As String
    Dim _TablDeviceId As New ArrayList
    ' Dim _Myzone As Zone

    Private Sub FrmZone_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'If MyID = "" Then
        '    MessageBox.Show("Erreur il n'y as pas d'ID asscié à cette zone", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
        '    Exit Sub
        'Else
        '    _Myzone = FRMMere.Obj.ReturnZoneByID(MyID)
        'End If

        'TxtName.Text = _Myzone.Name
        'TxtImage.Text = _Myzone.Image

        '_TablDeviceId.Clear()
        'For i As Integer = 0 To FRMMere.Obj.Devices.Count - 1
        '    CbDeviceDispo.Items.Add(FRMMere.Obj.Devices.Item(i).name)
        '    _TablDeviceId.Add(FRMMere.Obj.Devices.Item(i).id)
        'Next

        'AfficheDeviceSelect()
    End Sub

    Private Sub BtnAjoutDevice_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnAjoutDevice.Click
        'If CbDeviceDispo.Text = "" Or CbDeviceDispo.SelectedIndex < 0 Then
        '    MessageBox.Show("Veuillez sélectionner un device à ajouter", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        '    Exit Sub
        'Else
        '    Dim flag As Boolean = False
        '    For i As Integer = 0 To ListDeviceSelect.Items.Count - 1
        '        If ListDeviceSelect.Items(i) = CbDeviceDispo.Text Then
        '            flag = True
        '        End If
        '    Next
        '    If flag = False Then
        '        _Myzone.ListDeviceId.Add(_TablDeviceId.Item(CbDeviceDispo.SelectedIndex))
        '        AfficheDeviceSelect()
        '    Else
        '        MessageBox.Show("Le device existe déjà dans la liste", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        '        Exit Sub
        '    End If
        'End If
    End Sub

    Private Sub AfficheDeviceSelect()
        'ListDeviceSelect.Items.Clear()
        'For i As Integer = 0 To _Myzone.ListDeviceId.Count - 1
        '    Dim x As Object
        '    x = FRMMere.Obj.ReturnDeviceByID(_Myzone.ListDeviceId.Item(i))
        '    If x IsNot Nothing Then ListDeviceSelect.Items.Add(x.name)
        '    x = Nothing
        'Next
    End Sub

    Private Sub BtnDelDevice_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnDelDevice.Click
        'If ListDeviceSelect.SelectedIndex < 0 Then
        '    MessageBox.Show("Veuillez sélectionner un device à ajouter", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        '    Exit Sub
        'Else
        '    Dim i As Integer = ListDeviceSelect.SelectedIndex
        '    ListDeviceSelect.Items.RemoveAt(i)
        '    _Myzone.ListDeviceId.RemoveAt(i)

        '    AfficheDeviceSelect()
        'End If
    End Sub

    Private Sub BtnLoadImage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnLoadImage.Click
        'OpenFileDialog1.Filter = "jpeg (*.jpg) |*.jpg;*.jpeg|(*.png) |*.png|(*.*) |*.*"

        'If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
        '    TxtImage.Text = OpenFileDialog1.FileName
        '    _Myzone.Image = TxtImage.Text
        'End If
    End Sub

    Private Sub TxtImage_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TxtImage.TextChanged
        'If File.Exists(TxtImage.Text) Then
        '    _Myzone.Image = TxtImage.Text
        '    MyImage.Load(TxtImage.Text)
        'End If
    End Sub

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnOK.Click
        'FRMMere.Obj.SaveZone(MyID, TxtName.Text, TxtImage.Text, _Myzone.ListDeviceId)
        'FRMMere.affzone()
        'Me.Close()
    End Sub

    Private Sub BtnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnCancel.Click
        Me.Close()
    End Sub

End Class