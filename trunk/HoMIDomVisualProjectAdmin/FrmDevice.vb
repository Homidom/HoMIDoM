Imports System.Reflection
Imports System.IO
Imports HoMIDom.HoMIDom.Device

Public Class FrmDevice
    '*****************************************************
    'VARIABLES
    '*****************************************************
    Public Enum EAction
        Nouveau
        Modifier
    End Enum

    Public Action As EAction 'Définit si modif ou création d'un device
    Public DeviceID As String 'ID du device à modifier

    Dim FlagNewCmd As Boolean 'true si nouvelle commande
    Dim CmdID As String
    '*******************************************************

    '******************************************************
    'Chargement de la page
    Private Sub FrmDevice_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ListCmd.Items.Clear()

        'Chargement de la liste des drivers
        'For i As Integer = 0 To FRMMere.Obj.Drivers.Count - 1
        '    LDriver.Items.Add(FRMMere.Obj.Drivers.Item(i)) '.PluginName)
        'Next
        LDriver.DisplayMember = "nom"

        'si c'est modification on affiche les propriétés du device
        If DeviceID <> "" And Action = EAction.Modifier Then
            Dim x As Object = FRMMere.Obj.ReturnDeviceByID(DeviceID)
            For k As Integer = 0 To LDriver.Items.Count - 1
                If LDriver.Items(k).id = x.driverid Then
                    LDriver.SelectedIndex = k
                End If
            Next
            TxtName.Text = x.Name
            TxtAdress.Text = x.Adresse
            TxtImg.Text = x.Picture
            cEnable.Checked = x.Enable
            CbTypeClass.Text = x.typeclass

            If CbTypeClass.Text = "tv" Then
                For i As Integer = 0 To x.listcommandname.count - 1
                    ListCmd.Items.Add(x.listcommandname(i))
                Next
            End If
        End If
    End Sub

    'Bouton Annuler
    Private Sub BtnAnnuler_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnAnnuler.Click
        Me.Close()
    End Sub

    'Bouton OK
    Private Sub BtnOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnOk.Click
        If CbTypeClass.Text = "" Then
            MessageBox.Show("Le type de device est obligatoire!", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Exit Sub
        End If
        If TxtName.Text = "" Then
            MessageBox.Show("Le nom du device est obligatoire!", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Exit Sub
        End If

        Dim adapter As String
        If LDriver.SelectedIndex >= 0 Then
            adapter = LDriver.Items(LDriver.SelectedIndex).id 'TablID(LDriver.SelectedIndex)
        Else
            MessageBox.Show("Le driver est obligatoire!", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Exit Sub
        End If

        If Action = EAction.Nouveau Then
            'DeviceID = FRMMere.Obj.SaveDevice("", TxtName.Text, TxtAdress.Text, TxtImg.Text, cEnable.Checked, adapter, CbTypeClass.Text)
        End If
        If Action = EAction.Modifier Then
            Dim ID As String = DeviceID
            'DeviceID = FRMMere.Obj.SaveDevice(ID, TxtName.Text, TxtAdress.Text, TxtImg.Text, cEnable.Checked, adapter, CbTypeClass.Text)
        End If

        DeviceID = ""
        Me.Action = Nothing
        FRMMere.AffDevice()
        Me.Close()
    End Sub

    'Sélection d'une commande
    Private Sub ListCmd_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListCmd.SelectedIndexChanged
        If ListCmd.SelectedIndex < 0 Then Exit Sub
        TxtCmdName.Text = FRMMere.Obj.ReturnDeviceByID(DeviceID).listcommandname(ListCmd.SelectedIndex)
        TxtCmdData.Text = FRMMere.Obj.ReturnDeviceByID(DeviceID).listcommanddata(ListCmd.SelectedIndex)
        TxtCmdRepeat.Text = FRMMere.Obj.ReturnDeviceByID(DeviceID).listcommandrepeat(ListCmd.SelectedIndex)
    End Sub

    'Création d'une commande
    Private Sub BtnNewCmd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnNewCmd.Click
        TxtCmdName.Text = ""
        TxtCmdRepeat.Text = "0"
        TxtCmdData.Text = ""

        FlagNewCmd = True
    End Sub

    'Enregistrer une commande nouveau/création
    Private Sub BtnSaveCmd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSaveCmd.Click
        If IsNumeric(TxtCmdRepeat.Text) = False Then
            MsgBox("Numérique obligatoire pour repeat !!")
            Exit Sub
        End If

        If FlagNewCmd = True Then 'nouvelle commande
            'FRMMere.Obj.SaveDeviceCommand(DeviceID, TxtCmdName.Text, TxtCmdData.Text, TxtCmdRepeat.Text)
        Else 'modifier commande
            'FRMMere.Obj.SaveDeviceCommand(DeviceID, TxtCmdName.Text, TxtCmdData.Text, TxtCmdRepeat.Text)
        End If

        ListCmd.Items.Clear()
        Dim x As Object = FRMMere.Obj.ReturnDeviceByID(DeviceID)
        For i As Integer = 0 To x.listcommandname.count - 1
            ListCmd.Items.Add(x.listcommandname(i))
        Next
        x = Nothing

        FlagNewCmd = False
    End Sub

    'Supprimer une commande
    Private Sub BtnDelCmd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnDelCmd.Click
        If ListCmd.SelectedIndex >= 0 Then
            'FRMMere.Obj.DeleteDeviceCommand(FRMMere.Obj.ReturnDeviceByID(DeviceID).id, TxtCmdName.Text)

            TxtCmdName.Text = ""
            TxtCmdData.Text = ""
            TxtCmdRepeat.Text = "0"
            ListCmd.Items.Clear()

            Dim x As Object = FRMMere.Obj.ReturnDeviceByID(DeviceID)
            For i As Integer = 0 To x.listcommandname.count - 1
                ListCmd.Items.Add(x.listcommandname(i))
            Next
            x = Nothing

        End If
    End Sub

    'Apprendre la commande
    Private Sub BtnLearn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnLearn.Click
        TxtCmdData.Text = FRMMere.Obj.StartIrLearning
    End Sub

    'Tester la commande
    Private Sub BtnTstCmd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnTstCmd.Click
        'FRMMere.Obj.SendCommand(TxtCmdName.Text, DeviceID)
    End Sub

    Private Sub CbTypeClass_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CbTypeClass.SelectedValueChanged
        If CbTypeClass.Text = "tv" Then
            Me.Height = 385
            Grp14.Visible = True
        Else
            Me.Height = 203
            Grp14.Visible = False
        End If
    End Sub

    Private Sub BtnLoadImg_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnLoadImg.Click
        OpenFileDialog1.Filter = "jpeg (*.jpg) |*.jpg;*.jpeg|(*.png) |*.png|(*.*) |*.*"

        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            TxtImg.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub TxtImg_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TxtImg.TextChanged
        If File.Exists(TxtImg.Text) Then
            MyImage.Load(TxtImg.Text)
        End If
    End Sub
End Class