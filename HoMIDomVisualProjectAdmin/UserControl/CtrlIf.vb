Imports HoMIDom.HoMIDom

Public Class CtrlIf
    'Declaration des Variables
    Dim _Expand As Boolean = False
    Dim _index As Integer

    Public Event Down(ByVal sender As Object)
    Public Event Up(ByVal sender As Object)
    Public Event Delete(ByVal sender As Object)

    Dim _ListCondition As New ArrayList
    Dim _ListThen As New ArrayList
    Dim _ListElse As New ArrayList

    Public Property Index() As Integer
        Get
            Return _index
        End Get
        Set(ByVal value As Integer)
            _index = value
        End Set
    End Property

    Public Property ListCondition() As ArrayList
        Get
            Return _ListCondition
        End Get
        Set(ByVal value As ArrayList)
            _ListCondition = value
            ViewActionsIf()
        End Set
    End Property

    Public Property ListThen() As ArrayList
        Get
            Return _ListThen
        End Get
        Set(ByVal value As ArrayList)
            _ListThen = value
            ViewActionsThen()
        End Set
    End Property

    Public Property ListElse() As ArrayList
        Get
            Return _ListElse
        End Get
        Set(ByVal value As ArrayList)
            _ListElse = value
            ViewActionsElse()
        End Set
    End Property

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
            Me.Height = 560
            _Expand = True
            BtnExpand.Text = "-"
        Else
            Me.Height = 40
            _Expand = False
            BtnExpand.Text = "+"
        End If
    End Sub

    'Nouvelle condition
    Private Sub BtnNewCondition_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnNewCondition.Click
        Dim x As New CtrlCondition
        With x
            .Width = 640
            .Visible = True
            .Index = TLPIf.RowCount
            AddHandler .Down, AddressOf ActionDown
            AddHandler .Up, AddressOf ActionUp
            AddHandler .Delete, AddressOf ActionDelete
        End With
        TLPIf.RowCount = TLPIf.RowCount + 1
        TLPIf.Controls.Add(x, 0, TLPIf.RowCount - 1)
    End Sub

    'Descendre l'action
    Private Sub ActionDown(ByVal sender As Object)
        If sender.Index >= sender.parent.Controls.Count - 1 Then Exit Sub

        Dim _tmplist As New ArrayList
        Dim _A As Object
        Dim _B As Object
        Dim tableau As TableLayoutPanel = sender.parent

        For i As Integer = 0 To tableau.Controls.Count - 1
            _tmplist.Add(tableau.Controls.Item(i))
        Next

        _A = tableau.Controls.Item(sender.Index + 1)
        _B = tableau.Controls.Item(sender.Index)

        _A.index = _B.index
        _B.index = _A.index + 1

        _tmplist.Item(sender.Index + 1) = _B
        _tmplist.Item(sender.Index) = _A

        tableau.Controls.Clear()
        For i As Integer = 0 To _tmplist.Count - 1
            tableau.Controls.Add(_tmplist.Item(i), 0, i)
        Next

        _tmplist = Nothing
    End Sub

    'Monter l'action
    Private Sub ActionUp(ByVal sender As Object)
        If sender.Index < 1 Then Exit Sub

        Dim _tmplist As New ArrayList
        Dim _A As Object
        Dim _B As Object
        Dim idx As Integer = sender.Index
        Dim tableau As TableLayoutPanel = sender.parent

        For i As Integer = 0 To tableau.Controls.Count - 1
            _tmplist.Add(tableau.Controls.Item(i))
        Next

        _A = tableau.Controls.Item(idx - 1)
        _B = tableau.Controls.Item(idx)

        _A.index = idx
        _B.index = idx - 1

        _tmplist.Item(idx - 1) = _B
        _tmplist.Item(idx) = _A

        tableau.Controls.Clear()
        For i As Integer = 0 To _tmplist.Count - 1
            tableau.Controls.Add(_tmplist.Item(i), 0, i)
        Next

        _tmplist = Nothing
    End Sub

    'Supprimer l'action
    Private Sub ActionDelete(ByVal sender As Object)
        Dim _tmplist As New ArrayList
        Dim tableau As TableLayoutPanel = sender.parent

        tableau.Controls.RemoveAt(sender.Index)

        For i As Integer = 0 To tableau.Controls.Count - 1
            _tmplist.Add(tableau.Controls.Item(i))
        Next

        tableau.Controls.Clear()
        For i As Integer = 0 To _tmplist.Count - 1
            _tmplist.Item(i).index = i
            tableau.Controls.Add(_tmplist.Item(i), 0, i)
        Next

        _tmplist = Nothing
    End Sub

    'THEN
#Region "Ajout Action Then"

    Private Sub BtnNewActionDevice_T_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnNewActionDevice_T.Click
        Dim x As New CtrlDevice
        With x
            .Width = 625
            .Visible = True
            .Index = TLPThen.RowCount
            AddHandler .Down, AddressOf ActionDown
            AddHandler .Up, AddressOf ActionUp
            AddHandler .Delete, AddressOf ActionDelete
        End With
        TLPThen.RowCount = TLPThen.RowCount + 1
        TLPThen.Controls.Add(x, 0, TLPThen.RowCount - 1)
    End Sub

    Private Sub BtnNewActionPause_T_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnNewActionPause_T.Click
        Dim x As New CtrlPause
        With x
            .Width = 625
            .Visible = True
            .Index = TLPThen.RowCount
            AddHandler .Down, AddressOf ActionDown
            AddHandler .Up, AddressOf ActionUp
            AddHandler .Delete, AddressOf ActionDelete
        End With
        TLPThen.RowCount = TLPThen.RowCount + 1
        TLPThen.Controls.Add(x, 0, TLPThen.RowCount - 1)
    End Sub

    Private Sub BtnNewActionExit_T_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnNewActionExit_T.Click
        Dim x As New CtrlExit
        With x
            .Width = 625
            .Visible = True
            .Index = TLPThen.RowCount
            AddHandler .Down, AddressOf ActionDown
            AddHandler .Up, AddressOf ActionUp
            AddHandler .Delete, AddressOf ActionDelete
        End With
        TLPThen.RowCount = TLPThen.RowCount + 1
        TLPThen.Controls.Add(x, 0, TLPThen.RowCount - 1)
    End Sub

    Private Sub BtnNewActionMail_T_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnNewActionMail_T.Click
        Dim x As New CtrlMail
        With x
            .Width = 625
            .Visible = True
            .Index = TLPThen.RowCount
            AddHandler .Down, AddressOf ActionDown
            AddHandler .Up, AddressOf ActionUp
            AddHandler .Delete, AddressOf ActionDelete
        End With
        TLPThen.RowCount = TLPThen.RowCount + 1
        TLPThen.Controls.Add(x, 0, TLPThen.RowCount - 1)
    End Sub

    Private Sub BtnNewActionSpeek_T_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnNewActionSpeek_T.Click
        Dim x As New CtrlParler
        With x
            .Width = 625
            .Visible = True
            .Index = TLPThen.RowCount
            AddHandler .Down, AddressOf ActionDown
            AddHandler .Up, AddressOf ActionUp
            AddHandler .Delete, AddressOf ActionDelete
        End With
        TLPThen.RowCount = TLPThen.RowCount + 1
        TLPThen.Controls.Add(x, 0, TLPThen.RowCount - 1)
    End Sub

#End Region

#Region "Ajout Action Else"

    Private Sub BtnNewActionDevice_E_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnNewActionDevice_E.Click
        Dim x As New CtrlDevice
        With x
            .Width = 625
            .Visible = True
            .Index = TLPElse.RowCount
            AddHandler .Down, AddressOf ActionDown
            AddHandler .Up, AddressOf ActionUp
            AddHandler .Delete, AddressOf ActionDelete
        End With
        TLPElse.RowCount = TLPElse.RowCount + 1
        TLPElse.Controls.Add(x, 0, TLPElse.RowCount - 1)
    End Sub

    Private Sub BtnNewActionPause_E_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnNewActionPause_E.Click
        Dim x As New CtrlPause
        With x
            .Width = 625
            .Visible = True
            .Index = TLPElse.RowCount
            AddHandler .Down, AddressOf ActionDown
            AddHandler .Up, AddressOf ActionUp
            AddHandler .Delete, AddressOf ActionDelete
        End With
        TLPElse.RowCount = TLPElse.RowCount + 1
        TLPElse.Controls.Add(x, 0, TLPElse.RowCount - 1)
    End Sub

    Private Sub BtnNewActionExit_E_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnNewActionExit_E.Click
        Dim x As New CtrlExit
        With x
            .Width = 625
            .Visible = True
            .Index = TLPElse.RowCount
            AddHandler .Down, AddressOf ActionDown
            AddHandler .Up, AddressOf ActionUp
            AddHandler .Delete, AddressOf ActionDelete
        End With
        TLPElse.RowCount = TLPElse.RowCount + 1
        TLPElse.Controls.Add(x, 0, TLPElse.RowCount - 1)
    End Sub

    Private Sub BtnNewActionMail_E_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnNewActionMail_E.Click
        Dim x As New CtrlMail
        With x
            .Width = 625
            .Visible = True
            .Index = TLPElse.RowCount
            AddHandler .Down, AddressOf ActionDown
            AddHandler .Up, AddressOf ActionUp
            AddHandler .Delete, AddressOf ActionDelete
        End With
        TLPElse.RowCount = TLPElse.RowCount + 1
        TLPElse.Controls.Add(x, 0, TLPElse.RowCount - 1)
    End Sub

    Private Sub BtnNewActionSpeek_S_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnNewActionSpeek_E.click
        Dim x As New CtrlParler
        With x
            .Width = 625
            .Visible = True
            .Index = TLPElse.RowCount
            AddHandler .Down, AddressOf ActionDown
            AddHandler .Up, AddressOf ActionUp
            AddHandler .Delete, AddressOf ActionDelete
        End With
        TLPElse.RowCount = TLPElse.RowCount + 1
        TLPElse.Controls.Add(x, 0, TLPElse.RowCount - 1)
    End Sub

#End Region

    'Affiche les conditions
    Private Sub ViewActionsIf()
        TLPIf.Controls.Clear()
        TLPIf.RowCount = _ListCondition.Count

        For i As Integer = 0 To _ListCondition.Count - 1
            Dim x As New CtrlCondition
            With x
                .TypeCondition = _ListCondition.Item(i).TypeCondition
                .Item = ListCondition.Item(i).Itemid
                .CProperty.Text = ListCondition.Item(i).Parametre
                .COpe.Text = ListCondition.Item(i).Operateur
                .CValue.Text = ListCondition.Item(i).Value
                .Index = i
                .Width = 625
                .Visible = True
                AddHandler .Down, AddressOf ActionDown
                AddHandler .Up, AddressOf ActionUp
                AddHandler .Delete, AddressOf ActionDelete
            End With
            TLPIf.Controls.Add(x, 0, i)
        Next
    End Sub

    'Affiche les actions Then
    Private Sub ViewActionsThen()
        TLPThen.Controls.Clear()
        TLPThen.RowCount = _ListThen.Count

        For i As Integer = 0 To _ListThen.Count - 1
            Select Case _ListThen.Item(i).typeclass
                Case "DEVICE"
                    Dim x As New CtrlDevice
                    With x
                        .DeviceId = _ListThen.Item(i).IDDevice
                        .Fonction = _ListThen.Item(i).DeviceFunction
                        .Valeur = _ListThen.Item(i).value
                        .Index = i
                        .Width = 625
                        .Visible = True
                        AddHandler .Down, AddressOf ActionDown
                        AddHandler .Up, AddressOf ActionUp
                        AddHandler .Delete, AddressOf ActionDelete
                    End With
                    TLPThen.Controls.Add(x, 0, i)
                Case "PAUSE"
                    Dim x As New CtrlPause
                    With x
                        .Heure = _ListThen.Item(i).Heure
                        .Minute = _ListThen.Item(i).Minute
                        .Seconde = _ListThen.Item(i).Seconde
                        .MilliSeconde = _ListThen.Item(i).MilliSeconde
                        .Index = i
                        .Width = 625
                        .Visible = True
                        AddHandler .Down, AddressOf ActionDown
                        AddHandler .Up, AddressOf ActionUp
                        AddHandler .Delete, AddressOf ActionDelete
                    End With
                    TLPThen.Controls.Add(x, 0, i)
                Case "MAIL"
                    Dim x As New CtrlMail
                    With x
                        .ServeurSMTP = _ListThen.Item(i).MailServerSMTP
                        .CheckIdentification = _ListThen.Item(i).MailServerIdentification
                        .Login = _ListThen.Item(i).MailServerLogin
                        .Password = _ListThen.Item(i).MailServerPassword
                        .De = _ListThen.Item(i).From
                        .A = _ListThen.Item(i).A
                        .Sujet = _ListThen.Item(i).Sujet
                        .Message = _ListThen.Item(i).Message
                        .Index = i
                        .Width = 625
                        .Visible = True
                        AddHandler .Down, AddressOf ActionDown
                        AddHandler .Up, AddressOf ActionUp
                        AddHandler .Delete, AddressOf ActionDelete
                    End With
                    TLPThen.Controls.Add(x, 0, i)
                Case "PARLER"
                    Dim x As New CtrlParler
                    With x
                        .Message = _ListThen.Item(i).message
                        .Index = i
                        .Width = 625
                        .Visible = True
                        AddHandler .Down, AddressOf ActionDown
                        AddHandler .Up, AddressOf ActionUp
                        AddHandler .Delete, AddressOf ActionDelete
                    End With
                    TLPThen.Controls.Add(x, 0, i)
                Case "EXIT"
                    Dim x As New CtrlExit
                    With x
                        .Index = i
                        .Width = 625
                        .Visible = True
                        AddHandler .Down, AddressOf ActionDown
                        AddHandler .Up, AddressOf ActionUp
                        AddHandler .Delete, AddressOf ActionDelete
                    End With
                    TLPThen.Controls.Add(x, 0, i)
            End Select
        Next
    End Sub

    'Affiche les actions Else
    Private Sub ViewActionsElse()
        TLPElse.Controls.Clear()
        TLPElse.RowCount = _ListElse.Count

        For i As Integer = 0 To _ListElse.Count - 1
            Select Case _ListElse.Item(i).typeclass
                Case "DEVICE"
                    Dim x As New CtrlDevice
                    With x
                        .DeviceId = _ListElse.Item(i).IDDevice
                        .Fonction = _ListElse.Item(i).DeviceFunction
                        .Valeur = _ListElse.Item(i).value
                        .Index = i
                        .Width = 625
                        .Visible = True
                        AddHandler .Down, AddressOf ActionDown
                        AddHandler .Up, AddressOf ActionUp
                        AddHandler .Delete, AddressOf ActionDelete
                    End With
                    TLPElse.Controls.Add(x, 0, i)
                Case "PAUSE"
                    Dim x As New CtrlPause
                    With x
                        .Heure = _ListElse.Item(i).Heure
                        .Minute = _ListElse.Item(i).Minute
                        .Seconde = _ListElse.Item(i).Seconde
                        .MilliSeconde = _ListElse.Item(i).MilliSeconde
                        .Index = i
                        .Width = 625
                        .Visible = True
                        AddHandler .Down, AddressOf ActionDown
                        AddHandler .Up, AddressOf ActionUp
                        AddHandler .Delete, AddressOf ActionDelete
                    End With
                    TLPElse.Controls.Add(x, 0, i)
                Case "MAIL"
                    Dim x As New CtrlMail
                    With x
                        .ServeurSMTP = _ListElse.Item(i).MailServerSMTP
                        .CheckIdentification = _ListElse.Item(i).MailServerIdentification
                        .Login = _ListElse.Item(i).MailServerLogin
                        .Password = _ListElse.Item(i).MailServerPassword
                        .De = _ListElse.Item(i).From
                        .A = _ListElse.Item(i).A
                        .Sujet = _ListElse.Item(i).Sujet
                        .Message = _ListElse.Item(i).Message
                        .Index = i
                        .Width = 625
                        .Visible = True
                        AddHandler .Down, AddressOf ActionDown
                        AddHandler .Up, AddressOf ActionUp
                        AddHandler .Delete, AddressOf ActionDelete
                    End With
                    TLPElse.Controls.Add(x, 0, i)
                Case "PARLER"
                    Dim x As New CtrlParler
                    With x
                        .Message = _ListElse.Item(i).message
                        .Index = i
                        .Width = 625
                        .Visible = True
                        AddHandler .Down, AddressOf ActionDown
                        AddHandler .Up, AddressOf ActionUp
                        AddHandler .Delete, AddressOf ActionDelete
                    End With
                    TLPElse.Controls.Add(x, 0, i)
                Case "EXIT"
                    Dim x As New CtrlExit
                    With x
                        .Index = i
                        .Width = 625
                        .Visible = True
                        AddHandler .Down, AddressOf ActionDown
                        AddHandler .Up, AddressOf ActionUp
                        AddHandler .Delete, AddressOf ActionDelete
                    End With
                    TLPElse.Controls.Add(x, 0, i)
            End Select
        Next
    End Sub

    Private Sub SaveIF()
        'Enregistre les actions dans ListElse
        _ListCondition.Clear()
        For i As Integer = 0 To TLPIf.Controls.Count - 1
            'Dim x As New Script.ClassIf.Condition
            'Dim y As New CtrlCondition
            'y = TLPIf.Controls.Item(i)
            'With x
            '    .TypeCondition = y.TypeCondition
            '    .ItemId = y.Item
            '    .Parametre = y.CProperty.Text
            '    .Operateur = y.COpe.Text
            '    .Value = y.CValue.Text
            'End With
            '_ListCondition.Add(x)
        Next

        'Enregistre les actions dans Listthen
        _ListThen.Clear()
        'For i As Integer = 0 To TLPThen.Controls.Count - 1
        '    Dim a() As String = (TLPThen.Controls.Item(i).ToString.Split("."))
        '    Select Case a(1)
        '        Case "CtrlDevice"
        '            Dim x As New Script.ClassDevice
        '            Dim y As New CtrlDevice
        '            y = TLPThen.Controls.Item(i)
        '            With x
        '                .IDDevice = y.DeviceId
        '                .DeviceFunction = y.Fonction
        '                .Value = y.Valeur
        '            End With
        '            _ListThen.Add(x)
        '        Case "CtrlPause"
        '            Dim x As New Script.ClassPause
        '            Dim y As New CtrlPause
        '            y = TLPThen.Controls.Item(i)
        '            With x
        '                .Heure = y.Heure
        '                .Minute = y.Minute
        '                .Seconde = y.Seconde
        '                .MilliSeconde = y.MilliSeconde
        '            End With
        '            _ListThen.Add(x)
        '        Case "CtrlExit"
        '            Dim x As New Script.ClassExit
        '            Dim y As New CtrlExit
        '            y = TLPThen.Controls.Item(i)
        '            _ListThen.Add(x)
        '        Case "CtrlMail"
        '            Dim x As New Script.ClassEmail
        '            Dim y As New CtrlMail
        '            y = TLPThen.Controls.Item(i)
        '            With x
        '                .MailServerSMTP = y.ServeurSMTP
        '                .MailServerIdentification = y.CheckIdentification
        '                .MailServerLogin = y.Login
        '                .MailServerPassword = y.Password
        '                .From = y.De
        '                .A = y.A
        '                .Sujet = y.Sujet
        '                .Message = y.Message
        '            End With
        '            _ListThen.Add(x)
        '        Case "CtrlParler"
        '            Dim x As New Script.ClassVoice
        '            Dim y As New CtrlParler
        '            y = TLPThen.Controls.Item(i)
        '            With x
        '                .Message = y.Message
        '            End With
        '            _ListThen.Add(x)
        '    End Select
        'Next

        ''Enregistre les actions dans ListElse
        '_ListElse.Clear()
        'For i As Integer = 0 To TLPElse.Controls.Count - 1
        '    Dim a() As String = (TLPElse.Controls.Item(i).ToString.Split("."))
        '    Select Case a(1)
        '        Case "CtrlDevice"
        '            Dim x As New Script.ClassDevice
        '            Dim y As New CtrlDevice
        '            y = TLPElse.Controls.Item(i)
        '            With x
        '                .IDDevice = y.DeviceId
        '                .DeviceFunction = y.Fonction
        '                .Value = y.Valeur
        '            End With
        '            _ListElse.Add(x)
        '        Case "CtrlPause"
        '            Dim x As New Script.ClassPause
        '            Dim y As New CtrlPause
        '            y = TLPElse.Controls.Item(i)
        '            With x
        '                .Heure = y.Heure
        '                .Minute = y.Minute
        '                .Seconde = y.Seconde
        '                .MilliSeconde = y.MilliSeconde
        '            End With
        '            _ListElse.Add(x)
        '        Case "CtrlExit"
        '            Dim x As New Script.ClassExit
        '            Dim y As New CtrlExit
        '            y = TLPElse.Controls.Item(i)
        '            _ListElse.Add(x)
        '        Case "CtrlMail"
        '            Dim x As New Script.ClassEmail
        '            Dim y As New CtrlMail
        '            y = TLPElse.Controls.Item(i)
        '            With x
        '                .MailServerSMTP = y.ServeurSMTP
        '                .MailServerIdentification = y.CheckIdentification
        '                .MailServerLogin = y.Login
        '                .MailServerPassword = y.Password
        '                .From = y.De
        '                .A = y.A
        '                .Sujet = y.Sujet
        '                .Message = y.Message
        '            End With
        '            _ListElse.Add(x)
        '        Case "CtrlParler"
        '            Dim x As New Script.ClassVoice
        '            Dim y As New CtrlParler
        '            y = TLPElse.Controls.Item(i)
        '            With x
        '                .Message = y.Message
        '            End With
        '            _ListElse.Add(x)
        '    End Select
        'Next
    End Sub

    Private Sub TLPIf_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles TLPIf.MouseLeave
        ''Enregistre les actions dans ListIf
        '_ListCondition.Clear()
        'For i As Integer = 0 To TLPIf.Controls.Count - 1
        '    Dim x As New Script.ClassIf.Condition
        '    Dim y As New CtrlCondition
        '    y = TLPIf.Controls.Item(i)
        '    With x
        '        .TypeCondition = y.TypeCondition
        '        .ItemId = y.item
        '        .Parametre = y.CProperty.Text
        '        .Operateur = y.COpe.Text
        '        .Value = y.CValue.Text
        '    End With
        '    _ListCondition.Add(x)
        'Next
    End Sub

    Private Sub TLPThen_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles TLPThen.MouseLeave
        ''Enregistre les actions dans Listthen
        '_ListThen.Clear()
        'For i As Integer = 0 To TLPThen.Controls.Count - 1
        '    Dim a() As String = (TLPThen.Controls.Item(i).ToString.Split("."))
        '    Select Case a(1)
        '        Case "CtrlDevice"
        '            Dim x As New Script.ClassDevice
        '            Dim y As New CtrlDevice
        '            y = TLPThen.Controls.Item(i)
        '            With x
        '                .IDDevice = y.DeviceId
        '                .DeviceFunction = y.Fonction
        '                .Value = y.Valeur
        '            End With
        '            _ListThen.Add(x)
        '        Case "CtrlPause"
        '            Dim x As New Script.ClassPause
        '            Dim y As New CtrlPause
        '            y = TLPThen.Controls.Item(i)
        '            With x
        '                .Heure = y.Heure
        '                .Minute = y.Minute
        '                .Seconde = y.Seconde
        '                .MilliSeconde = y.MilliSeconde
        '            End With
        '            _ListThen.Add(x)
        '        Case "CtrlExit"
        '            Dim x As New Script.ClassExit
        '            Dim y As New CtrlExit
        '            y = TLPThen.Controls.Item(i)
        '            _ListThen.Add(x)
        '        Case "CtrlMail"
        '            Dim x As New Script.ClassEmail
        '            Dim y As New CtrlMail
        '            y = TLPThen.Controls.Item(i)
        '            With x
        '                .MailServerSMTP = y.ServeurSMTP
        '                .MailServerIdentification = y.CheckIdentification
        '                .MailServerLogin = y.Login
        '                .MailServerPassword = y.Password
        '                .From = y.De
        '                .A = y.A
        '                .Sujet = y.Sujet
        '                .Message = y.Message
        '            End With
        '            _ListThen.Add(x)
        '        Case "CtrlParler"
        '            Dim x As New Script.ClassVoice
        '            Dim y As New CtrlParler
        '            y = TLPThen.Controls.Item(i)
        '            With x
        '                .Message = y.Message
        '            End With
        '            _ListThen.Add(x)
        '    End Select
        'Next
    End Sub

    Private Sub TLPElse_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles TLPElse.MouseLeave
        ''Enregistre les actions dans ListElse
        '_ListElse.Clear()
        'For i As Integer = 0 To TLPElse.Controls.Count - 1
        '    Dim a() As String = (TLPElse.Controls.Item(i).ToString.Split("."))
        '    Select Case a(1)
        '        Case "CtrlDevice"
        '            Dim x As New Script.ClassDevice
        '            Dim y As New CtrlDevice
        '            y = TLPElse.Controls.Item(i)
        '            With x
        '                .IDDevice = y.DeviceId
        '                .DeviceFunction = y.Fonction
        '                .Value = y.Valeur
        '            End With
        '            _ListElse.Add(x)
        '        Case "CtrlPause"
        '            Dim x As New Script.ClassPause
        '            Dim y As New CtrlPause
        '            y = TLPElse.Controls.Item(i)
        '            With x
        '                .Heure = y.Heure
        '                .Minute = y.Minute
        '                .Seconde = y.Seconde
        '                .MilliSeconde = y.MilliSeconde
        '            End With
        '            _ListElse.Add(x)
        '        Case "CtrlExit"
        '            Dim x As New Script.ClassExit
        '            Dim y As New CtrlExit
        '            y = TLPElse.Controls.Item(i)
        '            _ListElse.Add(x)
        '        Case "CtrlMail"
        '            Dim x As New Script.ClassEmail
        '            Dim y As New CtrlMail
        '            y = TLPElse.Controls.Item(i)
        '            With x
        '                .MailServerSMTP = y.ServeurSMTP
        '                .MailServerIdentification = y.CheckIdentification
        '                .MailServerLogin = y.Login
        '                .MailServerPassword = y.Password
        '                .From = y.De
        '                .A = y.A
        '                .Sujet = y.Sujet
        '                .Message = y.Message
        '            End With
        '            _ListElse.Add(x)
        '        Case "CtrlParler"
        '            Dim x As New Script.ClassVoice
        '            Dim y As New CtrlParler
        '            y = TLPElse.Controls.Item(i)
        '            With x
        '                .Message = y.Message
        '            End With
        '            _ListElse.Add(x)
        '    End Select
        'Next
    End Sub

    Private Sub CtrlIf_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.MouseLeave
        SaveIF()
    End Sub

End Class
