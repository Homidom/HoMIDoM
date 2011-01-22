Imports HoMIDom.HoMIDom

Public Class CtrlDevice

    Dim mDeviceID As String
    Dim mFonction As String = ""
    Dim mParametre As String = ""
    Dim _index As Integer

    Dim _Expand As Boolean = False
    Dim _mListDeviceID As New ArrayList
    Dim _list As New ArrayList
    Dim _listtypefct As New ArrayList

    Public Event Down(ByVal sender As Object)
    Public Event Up(ByVal sender As Object)
    Public Event Delete(ByVal sender As Object)

    Public Property Index() As Integer
        Get
            Return _index
        End Get
        Set(ByVal value As Integer)
            _index = value
        End Set
    End Property

    Public Property DeviceId() As String
        Get
            If cDevice.SelectedIndex >= 0 Then
                Return _mListDeviceID.Item(cDevice.SelectedIndex)
            Else
                Return ""
            End If
        End Get
        Set(ByVal value As String)
            mDeviceID = value

            'on remplie la liste des devices
            For i As Integer = 0 To _mListDeviceID.Count - 1
                If _mListDeviceID.Item(i) = value Then
                    cDevice.SelectedIndex = i
                End If
            Next

            'on remplie la liste des commandes du device
            Dim obj As Object = FRMMere.Obj.ReturnDeviceByID(value)
            'If obj IsNot Nothing Then
            '    _list.Clear()
            '    _listtypefct.Clear()
            '    cFonction.Items.Clear()

            '    _list = ListMethod(obj)
            '    'on récupère les fonctions du device
            '    For j As Integer = 0 To _list.Count - 1
            '        Dim a() As String = _list(j).split("|")
            '        cFonction.Items.Add(a(0))
            '        If a.Length > 1 Then
            '            _listtypefct.Add(a(1))
            '        End If
            '    Next
            'End If


        End Set
    End Property

    Public Property Fonction() As String
        Get
            If cFonction.Text <> "" Then
                Return cFonction.Text
            Else
                Return ""
            End If
        End Get
        Set(ByVal value As String)
            For i As Integer = 0 To cFonction.Items.Count - 1
                If cFonction.Items(i) = value Then
                    cFonction.SelectedIndex = i
                End If
            Next
        End Set
    End Property

    Public Property Valeur() As String
        Get
            If cValeur.Text <> "" Then
                Return cValeur.Text
            Else
                Return ""
            End If
        End Get
        Set(ByVal value As String)
            cValeur.Text = value
        End Set
    End Property

    Public Sub New()

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        _mListDeviceID.Clear()
        cDevice.Items.Clear()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        For i As Integer = 0 To FRMMere.Obj.Devices.Count - 1
            'On liste que les devices qui ont des commandes
            'If FRMMere.Obj.Devices.Item(i).Listcommand.count > 0 Then
            cDevice.Items.Add(FRMMere.Obj.Devices.Item(i).name)
            _mListDeviceID.Add(FRMMere.Obj.Devices.Item(i).id)
            'End If
        Next

    End Sub


    Private Sub cDevice_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cDevice.SelectedValueChanged
        'If cDevice.SelectedIndex < 0 Then Exit Sub

        ''on remplie la liste des commandes du device
        'Dim obj As Object = FRMMere.Obj.ReturnDeviceByID(_mListDeviceID.Item(cDevice.SelectedIndex))

        'If obj IsNot Nothing Then
        '    _list.Clear()
        '    _listtypefct.Clear()
        '    cFonction.Items.Clear()

        '    _list = ListMethod(obj)
        '    'on récupère les fonctions du device
        '    For j As Integer = 0 To _list.Count - 1
        '        Dim a() As String = _list(j).split("|")
        '        If a.Length > 0 Then
        '            cFonction.Items.Add(a(0))
        '            If a.Length > 1 Then
        '                _listtypefct.Add(a(1))
        '            End If
        '        End If
        '    Next

        'End If
    End Sub

    Private Sub cValeur_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cValeur.TextChanged
        If cFonction.SelectedIndex < 0 Then Exit Sub
        Dim a() As String = _listtypefct(cFonction.SelectedIndex).split(":")
        If a.Length < 2 Then Exit Sub
        Select Case a(1)
            Case "System.Boolean"
                If cValeur.Text <> "" And cValeur.Text <> "True" And cValeur.Text <> "False" Then
                    MessageBox.Show("Type Boolean obligatoire!", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    cValeur.Text = ""
                End If
            Case "System.Byte"
                If cValeur.Text <> "" And IsNumeric(cValeur.Text) = False Then
                    MessageBox.Show("Type Byte obligatoire!", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    cValeur.Text = ""
                End If
            Case "System.Single"
                If cValeur.Text <> "" And IsNumeric(cValeur.Text) = False Then
                    MessageBox.Show("Type Single obligatoire!", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    cValeur.Text = ""
                End If
        End Select
    End Sub

    Private Sub cFonction_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cFonction.SelectedValueChanged
        If cFonction.SelectedIndex < 0 Then Exit Sub
        Select Case _listtypefct(cFonction.SelectedIndex)
            Case "System.Boolean"
                'Si type Boolean on complète la liste avec True ou False
                'on récupère et affiche les valeurs
                cValeur.Items.Add("True")
                cValeur.Items.Add("False")
                cValeur.DropDownStyle = ComboBoxStyle.DropDownList
            Case Else
                cValeur.DropDownStyle = ComboBoxStyle.DropDown
        End Select
    End Sub

    Private Sub BtnUp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnUp.Click
        RaiseEvent Up(Me)
    End Sub

    Private Sub BtnDown_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnDown.Click
        RaiseEvent Down(Me)
    End Sub

    Private Sub BtnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnDelete.Click
        RaiseEvent Delete(Me)
    End Sub

    Private Sub BtnExpand_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnExpand.Click
        If _Expand = False Then
            Me.Height = 100
            _Expand = True
            BtnExpand.Text = "-"
        Else
            Me.Height = 50
            _Expand = False
            BtnExpand.Text = "+"
        End If
    End Sub


End Class
