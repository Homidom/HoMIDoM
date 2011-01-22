Imports HoMIDom.HoMIDom

Public Class CtrlCondition
    Public MyID As String
    Private MouseIsDown As Boolean = False

    Dim _index As Integer

    Dim _Item As String
    Dim _Type As String

    Dim _DeviceId As New ArrayList
    Dim _list As New ArrayList
    Dim _listtypefct As New ArrayList

    Public Event Down(ByVal sender As Object)
    Public Event Up(ByVal sender As Object)
    Public Event Delete(ByVal sender As Object)

    Public Property Item() As String
        Get
            Return _Item
        End Get
        Set(ByVal value As String)
            _Item = value
            If _Type <> "TEMPS" Then
                Dim x As Object = FRMMere.Obj.ReturnDeviceByID(value)
                If x IsNot Nothing Then
                    For i As Integer = 0 To CItem.Items.Count
                        If CItem.Items(i) = x.name Then
                            CItem.SelectedIndex = i
                            Exit For
                        End If
                    Next
                End If
            End If
        End Set
    End Property

    Public Property TypeCondition() As String
        Get
            Return _Type
        End Get
        Set(ByVal value As String)
            _Type = value
            CMode.Text = value
        End Set
    End Property

    Public Property Index() As Integer
        Get
            Return _index
        End Get
        Set(ByVal value As Integer)
            _index = value
        End Set
    End Property



    Private Sub CItem_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CItem.SelectedIndexChanged
        CProperty.Items.Clear()
        CProperty.Text = ""
        CProperty.Enabled = True
        CValue.Text = ""
        CValue.Items.Clear()
        COpe.Text = ""

        Select Case CMode.Text
            Case "TEMPS"
                If CItem.Text = "Lever Soleil" Then
                    _Item = CItem.Text
                    CProperty.Enabled = False
                    CValue.Items.Add("True")
                    CValue.Items.Add("False")
                End If
                If CItem.Text = "Coucher Soleil" Then
                    _Item = CItem.Text
                    CProperty.Enabled = False
                    CValue.Items.Add("True")
                    CValue.Items.Add("False")
                End If
                If CItem.Text = "Nuit" Then
                    _Item = CItem.Text
                    CProperty.Enabled = False
                    CValue.Items.Add("True")
                    CValue.Items.Add("False")
                End If
                If CItem.Text = "Jour" Then
                    _Item = CItem.Text
                    CProperty.Enabled = False
                    CValue.Items.Add("True")
                    CValue.Items.Add("False")
                End If
            Case "DEVICE"
                If CItem.SelectedIndex < 0 Then Exit Sub
                _Item = _DeviceId.Item(CItem.SelectedIndex)

                Dim obj2 As Object = FRMMere.Obj.ReturnDeviceByID(_DeviceId.Item(CItem.SelectedIndex))

                CProperty.Items.Clear()
                CValue.Items.Clear()
                CValue.Text = ""

                If obj2 IsNot Nothing Then 'si le device est existant
                    _list = ListProperty(obj2)
                    _listtypefct.Clear()

                    'on récupère les fonctions du device
                    For j As Integer = 0 To _list.Count - 1
                        Dim a() As String = _list(j).split("|")
                        CProperty.Items.Add(a(0))
                        _listtypefct.Add(a(1))
                    Next
                End If
        End Select
    End Sub

    Private Sub CProperty_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CProperty.SelectedIndexChanged
        CValue.Text = ""
        CValue.Items.Clear()
        CValue.DropDownStyle = ComboBoxStyle.DropDown

        If TypeCondition = "DEVICE" Then
            Select Case _listtypefct(CProperty.SelectedIndex)
                Case "Boolean"
                    '        'Si type Boolean on complète la liste avec True ou False
                    '        'on récupère et affiche les valeurs
                    CValue.Items.Add("True")
                    CValue.Items.Add("False")
                    CValue.DropDownStyle = ComboBoxStyle.DropDownList
                Case Else
                    CValue.DropDownStyle = ComboBoxStyle.DropDown
            End Select
        End If
    End Sub


    Private Sub BtnDown_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnDown.Click
        RaiseEvent Down(Me)
    End Sub

    Private Sub BtnUp_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnUp.Click
        RaiseEvent Up(Me)
    End Sub

    Private Sub BtnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        RaiseEvent Delete(Me)
    End Sub

    Private Sub CMode_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CMode.SelectedValueChanged
        'Vide les autres champs
        CItem.Items.Clear()
        CItem.Text = ""
        CProperty.Items.Clear()
        CProperty.Text = ""
        COpe.Text = ""
        CValue.Items.Clear()
        CValue.Text = ""

        _DeviceId.Clear()
        Select Case CMode.Text
            Case "TEMPS"
                _Type = "TEMPS"
                CItem.Visible = False
                CProperty.Items.Add("Date Heure")
                CProperty.Items.Add("Lever Soleil")
                CProperty.Items.Add("Coucher Soleil")
                CProperty.Items.Add("Nuit")
                CProperty.Items.Add("Jour")
            Case "DEVICE"
                _Type = "DEVICE"
                'on liste tous les devices dispo
                CItem.Visible = True
                For i As Integer = 0 To FRMMere.Obj.Devices.Count - 1
                    CItem.Items.Add(FRMMere.Obj.Devices.Item(i).name())
                    _DeviceId.Add(FRMMere.Obj.Devices.Item(i).id)
                Next
        End Select
    End Sub

    Private Sub CValue_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CValue.TextChanged
        If TypeCondition = "DEVICE" Then
            Select Case _listtypefct(CProperty.SelectedIndex)
                Case "Boolean"
                    If CValue.Text <> "" And CValue.Text <> "True" And CValue.Text <> "False" Then
                        MessageBox.Show("Type Boolean obligatoire!", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        CValue.Text = ""
                    End If
                Case "Byte"
                    If CValue.Text <> "" And IsNumeric(CValue.Text) = False Then
                        MessageBox.Show("Type Byte obligatoire!", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        CValue.Text = ""
                    End If
                Case "Single"
                    If CValue.Text <> "" And IsNumeric(CValue.Text) = False Then
                        MessageBox.Show("Type Single obligatoire!", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        CValue.Text = ""
                    End If
            End Select
        End If
    End Sub

End Class
