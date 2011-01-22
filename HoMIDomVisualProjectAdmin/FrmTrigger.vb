Imports HoMIDom.HoMIDom

Public Class FrmTrigger
    'Declaration des variables
    Public Enum EAction
        Nouveau
        Modifier
    End Enum
    Public MyID As String
    Public Action As EAction
    Public ListAction As ArrayList = New ArrayList
    Dim _ListDeviceid As New List(Of String)
    Dim _ListScriptIdDispo As New ArrayList
    Dim _ListScriptIdSelect As New ArrayList
    Dim _list As New ArrayList
    Dim _listtypefct As New ArrayList

    Private Sub FrmTrigger_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        TxtDevice.Items.Clear()
        CbStatus.Items.Clear()
        'CbCondition.Items.Clear()
        CbValue.Items.Clear()
        _ListDeviceid.Clear()
        TxtNom.Text = ""
        cEnable.Checked = False
        ListBoxScriptDispo.Items.Clear()
        ListBoxScriptSelect.Items.Clear()

        'on rempli la liste des devices dispo
        For i As Integer = 0 To FRMMere.Obj.Devices.Count - 1
            If FRMMere.Obj.Devices.Item(i).enable = True Then
                TxtDevice.Items.Add(FRMMere.Obj.Devices.Item(i).name)
                _ListDeviceid.Add(FRMMere.Obj.Devices.Item(i).id)
            End If
        Next
        'on rempli la liste des scripts dispo
        'For i As Integer = 0 To FRMMere.Obj.Scripts.Count - 1
        '    ListBoxScriptDispo.Items.Add(FRMMere.Obj.Scripts.Item(i).name)
        '    _ListScriptIdDispo.Add(FRMMere.Obj.Scripts.Item(i).id)
        'Next

        '************************************************************
        'If Action = EAction.Modifier Then 'Modifier
        '    Dim obj As Trigger = FRMMere.Obj.ReturnTriggerByID(MyID)
        '    Dim obj2 As Object = FRMMere.Obj.ReturnDeviceByID(obj.DeviceId)

        '    TxtNom.Text = obj.Name 'nom du trigger
        '    cEnable.Checked = obj.Enable 'recupere enable
        '    _ListScriptIdSelect = obj.ListScript

        '    'on récupère les noms des scripts
        '    For j As Integer = 0 To _ListScriptIdSelect.Count - 1
        '        For k As Integer = 0 To FRMMere.Obj.Scripts.Count - 1
        '            If _ListScriptIdSelect.Item(j) = FRMMere.Obj.Scripts.Item(k).id Then
        '                ListBoxScriptSelect.Items.Add(FRMMere.Obj.Scripts.Item(k).name)
        '            End If
        '        Next
        '    Next

        '    'on récûpère le nom du device
        '    For j As Integer = 0 To TxtDevice.Items.Count - 1
        '        If TxtDevice.Items(j) = obj2.Name Then
        '            TxtDevice.SelectedIndex = j
        '        End If
        '    Next

        '    If obj2 IsNot Nothing Then 'si le device est existant
        '        Dim _sel0 As Integer = 0
        '        Dim _sel1 As Integer = 0
        '        _listtypefct.Clear()

        '        TxtDevice.Text = obj2.Name 'nom du device
        '        _list = ListProperty(obj2)

        '        'on récupère les propriétées du device
        '        For j As Integer = 0 To _list.Count - 1
        '            Dim a() As String = _list(j).split("|")
        '            CbStatus.Items.Add(a(0))
        '            _listtypefct.Add(a(1))
        '        Next
        '        For j As Byte = 0 To CbStatus.Items.Count - 1
        '            If CbStatus.Items(j) = obj.Status Then
        '                CbStatus.SelectedIndex = j
        '                Exit For
        '            End If
        '        Next

        '        'on affiche la condition
        '        CbCondition.Text = obj.Condition

        '        Select Case _listtypefct(CbStatus.SelectedIndex)
        '            Case "Boolean"
        '                '        'Si type Boolean on complète la liste avec True ou False
        '                '        'on récupère et affiche les valeurs
        '                CbValue.Items.Add("True")
        '                CbValue.Items.Add("False")
        '                If obj.Value = "True" Then
        '                    CbValue.SelectedIndex = 0
        '                Else
        '                    CbValue.SelectedIndex = 1
        '                End If
        '                CbValue.DropDownStyle = ComboBoxStyle.DropDownList
        '            Case Else
        '                CbValue.DropDownStyle = ComboBoxStyle.DropDown
        '                CbValue.Text = obj.Value
        '        End Select
        '    End If
        'End If
    End Sub

    'Annuler
    Private Sub BtnAnnuler_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnAnnuler.Click
        Me.Close()
    End Sub

    'Enregistrer nouveau ou modif trigger
    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnOK.Click
        'Erreur si nom trigger = vide
        If TxtNom.Text = "" Then
            MessageBox.Show("Veuillez saisir un nom de Trigger")
            Exit Sub
        End If

        'Nouveau Trigger
        If Action = 0 Then
            'FRMMere.Obj.SaveTrigger("", TxtNom.Text, cEnable.Checked, _ListDeviceid(TxtDevice.SelectedIndex), CbStatus.Text, CbCondition.Text, CbValue.Text, _ListScriptIdSelect)
        End If

        'Modification Trigger
        If Action = 1 Then
            'FRMMere.Obj.SaveTrigger(MyID, TxtNom.Text, cEnable.Checked, _ListDeviceid(TxtDevice.SelectedIndex), CbStatus.Text, CbCondition.Text, CbValue.Text, _ListScriptIdSelect)
        End If

        Me.Close()
        FRMMere.AffTrigger()
    End Sub

    'Quand on sélectionne un device on liste les propriétés dispo
    Private Sub TxtDevice_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles TxtDevice.MouseClick
       
    End Sub

    'Quand on sélectionne une propriété on liste les conditions dispo et les valeurs dispo
    Private Sub CbStatus_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbStatus.SelectedIndexChanged
        Select Case _listtypefct(CbStatus.SelectedIndex)
            Case "Boolean"
                '        'Si type Boolean on complète la liste avec True ou False
                '        'on récupère et affiche les valeurs
                CbValue.Items.Add("True")
                CbValue.Items.Add("False")
                CbValue.DropDownStyle = ComboBoxStyle.DropDownList
            Case Else
                CbValue.DropDownStyle = ComboBoxStyle.DropDown
        End Select
    End Sub

#Region "Gestion des scripts du trigger"

    'Selection d'un script
    Private Sub BtnSelect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSelect.Click
        If ListBoxScriptDispo.SelectedIndex >= 0 Then
            ListBoxScriptSelect.Items.Add(ListBoxScriptDispo.SelectedItem)
            _ListScriptIdSelect.Add(_ListScriptIdDispo.Item(ListBoxScriptDispo.SelectedIndex))
        End If
    End Sub

    'Supprime tous les scripts sélectionnés
    Private Sub BtnDelAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnDelAll.Click
        ListBoxScriptSelect.Items.Clear()
        _ListScriptIdSelect.Clear()
    End Sub

    'Déselectionne un script
    Private Sub BtnUnSelect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnUnSelect.Click
        If ListBoxScriptSelect.SelectedIndex >= 0 Then
            _ListScriptIdSelect.RemoveAt(ListBoxScriptSelect.SelectedIndex)
            ListBoxScriptSelect.Items.RemoveAt(ListBoxScriptSelect.SelectedIndex)
        End If
    End Sub

    'Monter un script dans la list
    Private Sub BtnUp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnUp.Click
        If ListBoxScriptSelect.SelectedIndex > 0 Then
            Dim _a As String = ListBoxScriptSelect.Items(ListBoxScriptSelect.SelectedIndex)
            Dim _b As String = _ListScriptIdSelect.Item(ListBoxScriptSelect.SelectedIndex)
            ListBoxScriptSelect.Items(ListBoxScriptSelect.SelectedIndex) = ListBoxScriptSelect.Items(ListBoxScriptSelect.SelectedIndex - 1)
            _ListScriptIdSelect.Item(ListBoxScriptSelect.SelectedIndex) = _ListScriptIdSelect.Item(ListBoxScriptSelect.SelectedIndex - 1)
            ListBoxScriptSelect.Items(ListBoxScriptSelect.SelectedIndex - 1) = _a
            _ListScriptIdSelect.Item(ListBoxScriptSelect.SelectedIndex - 1) = _b
        End If
    End Sub

    'Descendre un script dans la list
    Private Sub BtnDown_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnDown.Click
        If ListBoxScriptSelect.SelectedIndex < ListBoxScriptSelect.Items.Count - 1 Then
            Dim _a As String = ListBoxScriptSelect.Items(ListBoxScriptSelect.SelectedIndex)
            Dim _b As String = _ListScriptIdSelect.Item(ListBoxScriptSelect.SelectedIndex)
            ListBoxScriptSelect.Items(ListBoxScriptSelect.SelectedIndex) = ListBoxScriptSelect.Items(ListBoxScriptSelect.SelectedIndex + 1)
            _ListScriptIdSelect.Item(ListBoxScriptSelect.SelectedIndex) = _ListScriptIdSelect.Item(ListBoxScriptSelect.SelectedIndex + 1)
            ListBoxScriptSelect.Items(ListBoxScriptSelect.SelectedIndex + 1) = _a
            _ListScriptIdSelect.Item(ListBoxScriptSelect.SelectedIndex + 1) = _b
        End If
    End Sub
#End Region

    Private Sub CbValue_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CbValue.TextChanged
        Select Case _listtypefct(CbStatus.SelectedIndex)
            Case "Boolean"
                If CbValue.Text <> "" And CbValue.Text <> "True" And CbValue.Text <> "False" Then
                    MessageBox.Show("Type Boolean obligatoire!", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    CbValue.Text = ""
                End If
            Case "Byte"
                If CbValue.Text <> "" And IsNumeric(CbValue.Text) = False Then
                    MessageBox.Show("Type Byte obligatoire!", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    CbValue.Text = ""
                End If
            Case "Single"
                If CbValue.Text <> "" And IsNumeric(CbValue.Text) = False Then
                    MessageBox.Show("Type Single obligatoire!", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    CbValue.Text = ""
                End If
        End Select
    End Sub


    Private Sub TxtDevice_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TxtDevice.TextChanged
        If TxtDevice.SelectedIndex < 0 Then Exit Sub
        Dim obj2 As Object = FRMMere.Obj.ReturnDeviceByID(_ListDeviceid.Item(TxtDevice.SelectedIndex))

        CbStatus.Items.Clear()
        CbValue.Items.Clear()
        CbValue.Text = ""

        If obj2 IsNot Nothing Then 'si le device est existant
            _list = ListProperty(obj2)
            _listtypefct.Clear()

            'on récupère les fonctions du device
            For j As Integer = 0 To _list.Count - 1
                Dim a() As String = _list(j).split("|")
                CbStatus.Items.Add(a(0))
                _listtypefct.Add(a(1))
            Next
        End If
    End Sub
End Class