Imports HoMIDom.HoMIDom

Public Class FrmScene

    Public Enum EAction
        Nouveau
        Modifier
    End Enum

    Public MyID As String
    Public Action As EAction
    Public ListAction As New ArrayList 'Image des actions du script
    'Dim obj As Script
    Dim FlagNew As Boolean 'Nouvelle action
    Dim _TypeAction As String 'Nouvelle type d'action en cours
    Dim _CurrentCtrl As Object

    Private Sub BtnAnnuler_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnAnnuler.Click
        Me.Close()
    End Sub

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnOK.Click
        If TxtNom.Text = "" Then
            MessageBox.Show("Le nom du Script est obligatoire!")
            Exit Sub
        End If

        SaveToListAction()

        'If Action = EAction.Nouveau Then
        '    FRMMere.Obj.SaveScript("", TxtNom.Text, cEnable.Checked, ListAction)
        'End If
        'If Action = EAction.Modifier Then
        '    FRMMere.Obj.SaveScript(MyID, TxtNom.Text, cEnable.Checked, ListAction)
        'End If
      
        FRMMere.AffScene()
    End Sub

    Private Sub FrmScene_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Modification du script
        If Action = 1 Then
            'obj = FRMMere.Obj.ReturnScriptByID(MyID)
            'TxtNom.Text = obj.Name
            'cEnable.Checked = obj.Enable

            'ListAction = obj.ListAction

            ViewActions()
        End If
    End Sub

    'Enregistre les actions dans ListAction
    Private Sub SaveToListAction()
        'ListAction.Clear()
        'For i As Integer = 0 To TableLayoutPanel1.Controls.Count - 1
        '    Dim a() As String = (TableLayoutPanel1.Controls.Item(i).ToString.Split("."))
        '    Select Case a(1)
        '        Case "CtrlDevice"
        '            Dim x As New Script.ClassDevice
        '            Dim y As New CtrlDevice
        '            y = TableLayoutPanel1.Controls.Item(i)
        '            With x
        '                .IDDevice = y.DeviceId
        '                .DeviceFunction = y.Fonction
        '                .Value = y.Valeur
        '            End With
        '            ListAction.Add(x)
        '        Case "CtrlPause"
        '            Dim x As New Script.ClassPause
        '            Dim y As New CtrlPause
        '            y = TableLayoutPanel1.Controls.Item(i)
        '            With x
        '                .Heure = y.Heure
        '                .Minute = y.Minute
        '                .Seconde = y.Seconde
        '                .MilliSeconde = y.MilliSeconde
        '            End With
        '            ListAction.Add(x)
        '        Case "CtrlExit"
        '            Dim x As New Script.ClassExit
        '            Dim y As New CtrlExit
        '            y = TableLayoutPanel1.Controls.Item(i)
        '            ListAction.Add(x)
        '        Case "CtrlMail"
        '            Dim x As New Script.ClassEmail
        '            Dim y As New CtrlMail
        '            y = TableLayoutPanel1.Controls.Item(i)
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
        '            ListAction.Add(x)
        '        Case "CtrlParler"
        '            Dim x As New Script.ClassVoice
        '            Dim y As New CtrlParler
        '            y = TableLayoutPanel1.Controls.Item(i)
        '            With x
        '                .Message = y.Message
        '            End With
        '            ListAction.Add(x)
        '        Case "CtrlIf"
        '            Dim x As New Script.ClassIf
        '            Dim y As New CtrlIf
        '            y = TableLayoutPanel1.Controls.Item(i)
        '            With x
        '                .ListCondition = y.ListCondition
        '                .ThenListAction = y.ListThen
        '                .ElseListAction = y.ListElse
        '            End With
        '            ListAction.Add(x)
        '    End Select
        'Next
    End Sub


#Region "Ajouter nouvelle action"
    'Nouvelle action device
    Private Sub BtnNewActionDevice_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnNewActionDevice.Click
        Dim x As New CtrlDevice
        With x
            .Width = 625
            .Visible = True
            .Index = TableLayoutPanel1.RowCount
            AddHandler .Down, AddressOf ActionDown
            AddHandler .Up, AddressOf ActionUp
            AddHandler .Delete, AddressOf ActionDelete
        End With
        TableLayoutPanel1.RowCount = TableLayoutPanel1.RowCount + 1
        TableLayoutPanel1.Controls.Add(x, 0, TableLayoutPanel1.RowCount - 1)
    End Sub

    'Nouvelle Action Pause
    Private Sub BtnNewActionPause_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnNewActionPause.Click
        Dim x As New CtrlPause
        With x
            .Width = 625
            .Visible = True
            .Index = TableLayoutPanel1.RowCount
            AddHandler .Down, AddressOf ActionDown
            AddHandler .Up, AddressOf ActionUp
            AddHandler .Delete, AddressOf ActionDelete
        End With
        TableLayoutPanel1.RowCount = TableLayoutPanel1.RowCount + 1
        TableLayoutPanel1.Controls.Add(x, 0, TableLayoutPanel1.RowCount - 1)
    End Sub

    'Nouvelle Action Exit
    Private Sub BtnNewActionExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnNewActionExit.Click
        Dim x As New CtrlExit
        With x
            .Width = 625
            .Visible = True
            .Index = TableLayoutPanel1.RowCount
            AddHandler .Down, AddressOf ActionDown
            AddHandler .Up, AddressOf ActionUp
            AddHandler .Delete, AddressOf ActionDelete
        End With
        TableLayoutPanel1.RowCount = TableLayoutPanel1.RowCount + 1
        TableLayoutPanel1.Controls.Add(x, 0, TableLayoutPanel1.RowCount - 1)
    End Sub

    'Nouvelle action Mail
    Private Sub BtnNewActionMail_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnNewActionMail.Click
        Dim x As New CtrlMail
        With x
            .Width = 625
            .Visible = True
            .Index = TableLayoutPanel1.RowCount
            AddHandler .Down, AddressOf ActionDown
            AddHandler .Up, AddressOf ActionUp
            AddHandler .Delete, AddressOf ActionDelete
        End With
        TableLayoutPanel1.RowCount = TableLayoutPanel1.RowCount + 1
        TableLayoutPanel1.Controls.Add(x, 0, TableLayoutPanel1.RowCount - 1)
    End Sub

    'Nouvelle Action Parler
    Private Sub BtnNewActionSpeek_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnNewActionSpeek.Click
        Dim x As New CtrlParler
        With x
            .Width = 625
            .Visible = True
            .Index = TableLayoutPanel1.RowCount
            AddHandler .Down, AddressOf ActionDown
            AddHandler .Up, AddressOf ActionUp
            AddHandler .Delete, AddressOf ActionDelete
        End With
        TableLayoutPanel1.RowCount = TableLayoutPanel1.RowCount + 1
        TableLayoutPanel1.Controls.Add(x, 0, TableLayoutPanel1.RowCount - 1)
    End Sub

    'Nouvelle Action If
    Private Sub BtnNewActionIf_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnNewActionIf.Click
        Dim x As New CtrlIf
        With x
            .Width = 625
            .Visible = True
            .Index = TableLayoutPanel1.RowCount
            AddHandler .Down, AddressOf ActionDown
            AddHandler .Up, AddressOf ActionUp
            AddHandler .Delete, AddressOf ActionDelete
        End With
        TableLayoutPanel1.RowCount = TableLayoutPanel1.RowCount + 1
        TableLayoutPanel1.Controls.Add(x, 0, TableLayoutPanel1.RowCount - 1)
    End Sub
#End Region

    'Affiche les actions
    Private Sub ViewActions()
        TableLayoutPanel1.Controls.Clear()
        TableLayoutPanel1.RowCount = ListAction.Count

        For i As Integer = 0 To ListAction.Count - 1
            Select Case ListAction.Item(i).typeclass
                Case "DEVICE"
                    Dim x As New CtrlDevice
                    With x
                        .DeviceId = ListAction.Item(i).IDDevice
                        .Fonction = ListAction.Item(i).DeviceFunction
                        .Valeur = ListAction.Item(i).value
                        .Index = i
                        .Width = 625
                        .Visible = True
                        AddHandler .Down, AddressOf ActionDown
                        AddHandler .Up, AddressOf ActionUp
                        AddHandler .Delete, AddressOf ActionDelete
                    End With
                    TableLayoutPanel1.Controls.Add(x, 0, i)
                Case "PAUSE"
                    Dim x As New CtrlPause
                    With x
                        .Heure = ListAction.Item(i).Heure
                        .Minute = ListAction.Item(i).Minute
                        .Seconde = ListAction.Item(i).Seconde
                        .MilliSeconde = ListAction.Item(i).MilliSeconde
                        .Index = i
                        .Width = 625
                        .Visible = True
                        AddHandler .Down, AddressOf ActionDown
                        AddHandler .Up, AddressOf ActionUp
                        AddHandler .Delete, AddressOf ActionDelete
                    End With
                    TableLayoutPanel1.Controls.Add(x, 0, i)
                Case "MAIL"
                    Dim x As New CtrlMail
                    With x
                        .ServeurSMTP = ListAction.Item(i).MailServerSMTP
                        .CheckIdentification = ListAction.Item(i).MailServerIdentification
                        .Login = ListAction.Item(i).MailServerLogin
                        .Password = ListAction.Item(i).MailServerPassword
                        .De = ListAction.Item(i).From
                        .A = ListAction.Item(i).A
                        .Sujet = ListAction.Item(i).Sujet
                        .Message = ListAction.Item(i).Message
                        .Index = i
                        .Width = 625
                        .Visible = True
                        AddHandler .Down, AddressOf ActionDown
                        AddHandler .Up, AddressOf ActionUp
                        AddHandler .Delete, AddressOf ActionDelete
                    End With
                    TableLayoutPanel1.Controls.Add(x, 0, i)
                Case "PARLER"
                    Dim x As New CtrlParler
                    With x
                        .Message = ListAction.Item(i).message
                        .Index = i
                        .Width = 625
                        .Visible = True
                        AddHandler .Down, AddressOf ActionDown
                        AddHandler .Up, AddressOf ActionUp
                        AddHandler .Delete, AddressOf ActionDelete
                    End With
                    TableLayoutPanel1.Controls.Add(x, 0, i)
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
                    TableLayoutPanel1.Controls.Add(x, 0, i)
                Case "IF"
                    Dim x As New CtrlIf
                    With x
                        .ListCondition = ListAction.Item(i).ListCondition
                        .ListThen = ListAction.Item(i).ThenListAction
                        .ListElse = ListAction.Item(i).ElseListAction
                        .Index = i
                        .Width = 625
                        .Visible = True
                        AddHandler .Down, AddressOf ActionDown
                        AddHandler .Up, AddressOf ActionUp
                        AddHandler .Delete, AddressOf ActionDelete
                    End With
                    TableLayoutPanel1.Controls.Add(x, 0, i)
            End Select
        Next
    End Sub

    'Descendre l'action
    Private Sub ActionDown(ByVal sender As Object)
        If sender.Index >= sender.parent.Controls.Count - 1 Then Exit Sub

        Dim idx As Integer = sender.Index
        Dim _tmplist As New ArrayList
        Dim _A As Object
        Dim _B As Object
        Dim tableau As TableLayoutPanel = sender.parent

        For i As Integer = 0 To tableau.Controls.Count - 1
            _tmplist.Add(tableau.Controls.Item(i))
        Next

        _A = tableau.Controls.Item(idx + 1)
        _B = tableau.Controls.Item(idx)

        _A.index = _B.index
        _B.index = _A.index + 1

        _tmplist.Item(idx + 1) = _B
        _tmplist.Item(idx) = _A

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

    'Supprimer toutes les actions
    Private Sub BtnDeleteAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnDeleteAll.Click
        TableLayoutPanel1.Controls.Clear()
    End Sub

    Private Sub BtnTest_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnTest.Click
        '    FRMMere.Obj.RunMacro(MyID)
    End Sub
End Class