Imports HoMIDom.NCrontab
Imports HoMIDom.HoMIDom.Api

Public Class uTriggerTimer
    Public Event CloseMe(ByVal MyObject As Object)

    Dim _Action As EAction
    Dim _TriggerId As String
    Dim _ListMacro As New List(Of String)

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me)
    End Sub

    Private Sub TabItemChanged(ByVal sender As System.Object, ByVal e As System.Windows.DependencyPropertyChangedEventArgs) Handles Tabitem_avance.IsVisibleChanged
        simulate_cron_avance()
    End Sub

    Public Sub New(ByVal Action As Classe.EAction, ByVal TriggerId As String)
        Try
            ' Cet appel est requis par le concepteur.
            InitializeComponent()


            _Action = Action
            _TriggerId = TriggerId

            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            If _Action = EAction.Nouveau Then 'Nouveau Trigger
                ChkEnable.IsChecked = True
            Else 'Modifier Trigger
                Dim x As HoMIDom.HoMIDom.Trigger = myService.ReturnTriggerById(IdSrv, _TriggerId)

                If x IsNot Nothing Then
                    TxtNom.Text = x.Nom
                    ChkEnable.IsChecked = x.Enable
                    TxtDescription.Text = x.Description
                    _ListMacro = x.ListMacro

                    Dim cron As String = x.ConditionTime
                    If cron.Length > 10 Then
                        'cron = Mid(cron, 6, Len(cron) - 5)
                        Dim c() As String = cron.Split("#")

                        'mode simple (si pas de caracteres spéciaux sinon avancé uniquement)
                        If InStr(c(0), "/") = 0 And InStr(c(1), "/") = 0 And InStr(c(2), "/") = 0 And InStr(c(3), "/") = 0 And InStr(c(4), "/") = 0 And _
                           InStr(c(0), "-") = 0 And InStr(c(1), "-") = 0 And InStr(c(2), "-") = 0 And InStr(c(3), "-") = 0 And InStr(c(4), "-") = 0 And _
                           InStr(c(0), ",") = 0 And InStr(c(1), ",") = 0 And InStr(c(2), ",") = 0 And InStr(c(3), ",") = 0 And InStr(c(4), ",") = 0 Then
                            SP_modesimplecontenu.Visibility = Windows.Visibility.Visible
                            SP_modesimplealerte.Visibility = Windows.Visibility.Collapsed
                            SP_exemplesimple.Visibility = Windows.Visibility.Visible
                            'les champs sont remplis automatiquement car on change la valeur du champ avancé
                            'If c(0) = "*" Then TxtSc.Text = "" Else TxtSc.Text = c(0)
                            'If c(1) = "*" Then TxtMn.Text = "" Else TxtMn.Text = c(1)
                            'If c(2) = "*" Then TxtHr.Text = "" Else TxtHr.Text = c(2)
                            'If c(3) = "*" Then TxtJr.Text = "" Else TxtJr.Text = c(3)
                            'If c(4) = "*" Then TxtMs.Text = "" Else TxtMs.Text = c(4)
                            If InStr(c(5), "1") Then CheckBox1.IsChecked = True
                            If InStr(c(5), "2") Then CheckBox2.IsChecked = True
                            If InStr(c(5), "3") Then CheckBox3.IsChecked = True
                            If InStr(c(5), "4") Then CheckBox4.IsChecked = True
                            If InStr(c(5), "5") Then CheckBox5.IsChecked = True
                            If InStr(c(5), "6") Then CheckBox6.IsChecked = True
                            If InStr(c(5), "0") Then CheckBox7.IsChecked = True
                        Else
                            'on desactive le mode simple
                            SP_modesimplecontenu.Visibility = Windows.Visibility.Collapsed
                            SP_modesimplealerte.Visibility = Windows.Visibility.Visible
                            SP_exemplesimple.Visibility = Windows.Visibility.Collapsed

                        End If

                        'mode avancé
                        If InStr(c(0), "/") Then c(0) = Right(c(0), c(0).Length - InStr(c(0), "/")) : CheckBox9.IsChecked = True
                        If InStr(c(1), "/") Then c(1) = Right(c(1), c(1).Length - InStr(c(1), "/")) : CheckBox10.IsChecked = True
                        If InStr(c(2), "/") Then c(2) = Right(c(2), c(2).Length - InStr(c(2), "/")) : CheckBox11.IsChecked = True
                        If InStr(c(3), "/") Then c(3) = Right(c(3), c(3).Length - InStr(c(3), "/")) : CheckBox12.IsChecked = True
                        If InStr(c(4), "/") Then c(4) = Right(c(4), c(4).Length - InStr(c(4), "/")) : CheckBox13.IsChecked = True
                        TxtSc_avance.Text = c(0)
                        TxtMn_avance.Text = c(1)
                        TxtHr_avance.Text = c(2)
                        TxtJr_avance.Text = c(3)
                        TxtMs_avance.Text = c(4)
                        If InStr(c(5), "1") Then CheckBox1_avance.IsChecked = True
                        If InStr(c(5), "2") Then CheckBox2_avance.IsChecked = True
                        If InStr(c(5), "3") Then CheckBox3_avance.IsChecked = True
                        If InStr(c(5), "4") Then CheckBox4_avance.IsChecked = True
                        If InStr(c(5), "5") Then CheckBox5_avance.IsChecked = True
                        If InStr(c(5), "6") Then CheckBox6_avance.IsChecked = True
                        If InStr(c(5), "0") Then CheckBox7_avance.IsChecked = True

                    End If
                End If

            End If
            RemplirMacro()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur NewuTriggerTimer: " & ex.ToString)
        End Try
    End Sub

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        Try
            If String.IsNullOrEmpty(TxtNom.Text) Or HaveCaractSpecial(TxtNom.Text) Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le nom du trigger est obligatoire et ne doit pas comporter de caractère spécial!", "Trigger", "")
                Exit Sub
            End If

            If checkModeSimple Then
                If Instr(listeprochainscronssimple.Text, "Erreur") Then
                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le résultat de ces conditions simples retourne une erreur, revoyer les valeurs indiquées", "Trigger", "")
                    Exit Sub
                End If
            Else
                If Instr(listeprochainscrons.Text, "Erreur") Then
                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le résultat de ces conditions avancées retourne une erreur, revoyez les valeurs indiquées, commencez et terminez par un nombre, utilisez uniquement tiret du 6 et virgule . ", "Trigger", "")
                    Exit Sub
                End If
            End If

            'calcul du mode complexe
            Dim _myconditiontime_avance As String = ""
            If Not CheckBox9.IsChecked Then _myconditiontime_avance &= TxtSc_avance.Text & "#" Else If TxtSc_avance.Text = "*" Then _myconditiontime_avance &= TxtSc_avance.Text & "#" Else _myconditiontime_avance &= "*/" & TxtSc_avance.Text & "#"
            If Not CheckBox10.IsChecked Then _myconditiontime_avance &= TxtMn_avance.Text & "#" Else If TxtMn_avance.Text = "*" Then _myconditiontime_avance &= TxtMn_avance.Text & "#" Else _myconditiontime_avance &= "*/" & TxtMn_avance.Text & "#"
            If Not CheckBox11.IsChecked Then _myconditiontime_avance &= TxtHr_avance.Text & "#" Else If TxtHr_avance.Text = "*" Then _myconditiontime_avance &= TxtHr_avance.Text & "#" Else _myconditiontime_avance &= "*/" & TxtHr_avance.Text & "#"
            If Not CheckBox12.IsChecked Then _myconditiontime_avance &= TxtJr_avance.Text & "#" Else If TxtJr_avance.Text = "*" Then _myconditiontime_avance &= TxtJr_avance.Text & "#" Else _myconditiontime_avance &= "*/" & TxtJr_avance.Text & "#"
            If Not CheckBox13.IsChecked Then _myconditiontime_avance &= TxtMs_avance.Text & "#" Else If TxtMs_avance.Text = "*" Then _myconditiontime_avance &= TxtMs.Text & "#" Else _myconditiontime_avance &= "*/" & TxtMs_avance.Text & "#"
            '_myconditiontime_avance &= TxtSc_avance.Text & "#"
            '_myconditiontime_avance &= TxtMn_avance.Text & "#"
            '_myconditiontime_avance &= TxtHr_avance.Text & "#"
            '_myconditiontime_avance &= TxtJr_avance.Text & "#"
            '_myconditiontime_avance &= TxtMs_avance.Text & "#"

            Dim _myconditiontime As String = ""
            'Si on a des caracteres spéciaux alors on utilise le mode avancé
            If InStr(_myconditiontime_avance, "/") > 0 Or InStr(_myconditiontime_avance, "-") > 0 Or InStr(_myconditiontime_avance, ",") > 0 Then
                'on ajoute les jours
                Dim _prepajr_avance As String = ""
                If CheckBox7_avance.IsChecked = True Then _prepajr_avance = "0"
                If CheckBox1_avance.IsChecked = True Then If _prepajr_avance <> "" Then _prepajr_avance &= ",1" Else _prepajr_avance = "1"
                If CheckBox2_avance.IsChecked = True Then If _prepajr_avance <> "" Then _prepajr_avance &= ",2" Else _prepajr_avance = "2"
                If CheckBox3_avance.IsChecked = True Then If _prepajr_avance <> "" Then _prepajr_avance &= ",3" Else _prepajr_avance = "3"
                If CheckBox4_avance.IsChecked = True Then If _prepajr_avance <> "" Then _prepajr_avance &= ",4" Else _prepajr_avance = "4"
                If CheckBox5_avance.IsChecked = True Then If _prepajr_avance <> "" Then _prepajr_avance &= ",5" Else _prepajr_avance = "5"
                If CheckBox6_avance.IsChecked = True Then If _prepajr_avance <> "" Then _prepajr_avance &= ",6" Else _prepajr_avance = "6"
                If _prepajr_avance = "" Then _prepajr_avance = "*"
                _myconditiontime_avance &= _prepajr_avance
                _myconditiontime = _myconditiontime_avance
            Else
                'sinon on utilise les champs du mode simple
                If Not IsNumeric(TxtSc.Text) Then _myconditiontime &= "*#" Else _myconditiontime &= CInt(TxtSc.Text) & "#"
                If Not IsNumeric(TxtMn.Text) Then _myconditiontime &= "*#" Else _myconditiontime &= CInt(TxtMn.Text) & "#"
                If Not IsNumeric(TxtHr.Text) Then _myconditiontime &= "*#" Else _myconditiontime &= CInt(TxtHr.Text) & "#"
                If Not IsNumeric(TxtJr.Text) Then _myconditiontime &= "*#" Else _myconditiontime &= CInt(TxtJr.Text) & "#"
                If Not IsNumeric(TxtMs.Text) Then _myconditiontime &= "*#" Else _myconditiontime &= CInt(TxtMs.Text) & "#"
                'If TxtSc.Text = "" Then _myconditiontime &= "*#" Else _myconditiontime &= CInt(TxtSc.Text) & "#"
                'If TxtMn.Text = "" Then _myconditiontime &= "*#" Else _myconditiontime &= CInt(TxtMn.Text) & "#"
                'If TxtHr.Text = "" Then _myconditiontime &= "*#" Else _myconditiontime &= CInt(TxtHr.Text) & "#"
                'If TxtJr.Text = "" Then _myconditiontime &= "*#" Else _myconditiontime &= CInt(TxtJr.Text) & "#"
                'If TxtMs.Text = "" Then _myconditiontime &= "*#" Else _myconditiontime &= CInt(TxtMs.Text) & "#"
                Dim _prepajr As String = ""
                If CheckBox7.IsChecked = True Then _prepajr = "0"
                If CheckBox1.IsChecked = True Then If _prepajr <> "" Then _prepajr &= ",1" Else _prepajr = "1"
                If CheckBox2.IsChecked = True Then If _prepajr <> "" Then _prepajr &= ",2" Else _prepajr = "2"
                If CheckBox3.IsChecked = True Then If _prepajr <> "" Then _prepajr &= ",3" Else _prepajr = "3"
                If CheckBox4.IsChecked = True Then If _prepajr <> "" Then _prepajr &= ",4" Else _prepajr = "4"
                If CheckBox5.IsChecked = True Then If _prepajr <> "" Then _prepajr &= ",5" Else _prepajr = "5"
                If CheckBox6.IsChecked = True Then If _prepajr <> "" Then _prepajr &= ",6" Else _prepajr = "6"
                If _prepajr = "" Then _prepajr = "*"
                _myconditiontime &= _prepajr
            End If

            _ListMacro.Clear()
            For i As Integer = 0 To ListBox1.Items.Count - 1
                Dim x As StackPanel = ListBox1.Items.Item(i)
                Dim w As CheckBox = x.Children.Item(0)
                If w.IsChecked = True Then _ListMacro.Add(ListBox1.Items.Item(i).uid)
            Next

            If _Action = EAction.Nouveau Then
                myService.SaveTrigger(IdSrv, "", TxtNom.Text, ChkEnable.IsChecked, 0, TxtDescription.Text, _myconditiontime, "", "", _ListMacro)
            Else
                myService.SaveTrigger(IdSrv, _TriggerId, TxtNom.Text, ChkEnable.IsChecked, 0, TxtDescription.Text, _myconditiontime, "", "", _ListMacro)
            End If

            FlagChange = True
            RaiseEvent CloseMe(Me)
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur lors de l'enregistrement du trigger, message: " & ex.ToString, "Erreur", "")
        End Try
    End Sub

    'Private Sub UpMacro_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles UpMacro.Click
    '    Try
    '        If ListBox1.SelectedIndex <= 0 Then
    '            AfficheMessageAndLog (HoMIDom.HoMIDom.Server.TypeLog.ERREUR,"Aucune macro sélectionnée ou celle-ci est déjà en 1ère position, veuillez en choisir une à déplacer !", "Trigger","")
    '            Exit Sub
    '        Else
    '            Dim i As Integer = ListBox1.SelectedIndex

    '            Dim x As StackPanel = ListBox1.Items.Item(i)
    '            Dim w As CheckBox = x.Children.Item(0)
    '            If w.IsChecked = False Then
    '                AfficheMessageAndLog (HoMIDom.HoMIDom.Server.TypeLog.ERREUR,"Cette macro n'est pas cochée, elle ne peut être déplacée !", "Trigger","")
    '                Exit Sub
    '            End If

    '            Dim a As String = _ListMacro(i - 1)
    '            Dim b As String = _ListMacro(i)

    '            _ListMacro(i - 1) = b
    '            _ListMacro(i) = a
    '            RemplirMacro()
    '        End If
    '    Catch ex As Exception
    '        AfficheMessageAndLog (HoMIDom.HoMIDom.Server.TypeLog.ERREUR,"Erreur UpMacro: " & ex.ToString, "Erreur")
    '    End Try
    'End Sub

    'Private Sub DownMacro_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles DownMacro.Click
    '    Try

    '        If ListBox1.SelectedIndex < 0 Or ListBox1.SelectedIndex = ListBox1.Items.Count - 1 Then
    '            AfficheMessageAndLog (HoMIDom.HoMIDom.Server.TypeLog.ERREUR,"Aucune macro sélectionnée ou celle-ci est déjà en dernière position, veuillez en choisir une déplacer!", "Trigger","")
    '            Exit Sub
    '        Else
    '            Dim i As Integer = ListBox1.SelectedIndex

    '            Dim x As StackPanel = ListBox1.Items.Item(i)
    '            Dim w As CheckBox = x.Children.Item(0)
    '            If w.IsChecked = False Then
    '                AfficheMessageAndLog (HoMIDom.HoMIDom.Server.TypeLog.ERREUR,"Cette macro n'est pas cochée, elle ne peut être déplacée !", "Trigger","")
    '                Exit Sub
    '            End If
    '            x = ListBox1.Items.Item(i + 1)
    '            w = x.Children.Item(0)
    '            If w.IsChecked = False Then
    '                AfficheMessageAndLog (HoMIDom.HoMIDom.Server.TypeLog.ERREUR,"Cette macro ne peut pas être descendu dans la liste car la macro suivante n'est pas cochée, elle ne peut être déplacée !", "Trigger","")
    '                Exit Sub
    '            End If

    '            Dim a As String = _ListMacro(i + 1)
    '            Dim b As String = _ListMacro(i)

    '            _ListMacro(i + 1) = b
    '            _ListMacro(i) = a
    '            RemplirMacro()
    '        End If
    '    Catch ex As Exception
    '        AfficheMessageAndLog (HoMIDom.HoMIDom.Server.TypeLog.ERREUR,"Erreur DownMacro: " & ex.ToString, "Erreur")
    '    End Try
    'End Sub

    Private Sub RemplirMacro()
        Try
            Dim _ListIDMacroDelete As New List(Of String) 'Liste des macro à supprimer si elles n'existent plus

            ListBox1.Items.Clear()

            If _ListMacro IsNot Nothing Then
                For Each _IDMacro As String In _ListMacro
                    Dim x As New CheckBox
                    Dim stk As New StackPanel
                    Dim _macro As HoMIDom.HoMIDom.Macro = myService.ReturnMacroById(IdSrv, _IDMacro)

                    'Vérifie que la macro existe toujours
                    If _macro IsNot Nothing Then
                        stk.Margin = New Thickness(2)
                        x.Content = _macro.Nom
                        stk.Uid = _macro.ID
                        x.Uid = stk.Uid
                        x.IsChecked = True
                        AddHandler x.Click, AddressOf CheckClick
                        stk.Children.Add(x)
                        ListBox1.Items.Add(stk)
                    Else
                        _ListIDMacroDelete.Add(_IDMacro)
                    End If
                Next
            End If

            'Supprime la macro rattachée si elle n'existe plus
            For Each _IDmacro As String In _ListIDMacroDelete
                _ListMacro.Remove(_IDmacro)
            Next

            For i As Integer = 0 To myService.GetAllMacros(IdSrv).Count - 1
                If IsInList(myService.GetAllMacros(IdSrv).Item(i).ID) = False Then
                    Dim x As New CheckBox
                    Dim stk As New StackPanel
                    x.Content = myService.GetAllMacros(IdSrv).Item(i).Nom
                    x.IsChecked = False
                    AddHandler x.Click, AddressOf CheckClick
                    stk.Uid = myService.GetAllMacros(IdSrv).Item(i).ID
                    x.Uid = stk.Uid
                    stk.Margin = New Thickness(2)
                    stk.Children.Add(x)
                    ListBox1.Items.Add(stk)
                End If
            Next
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur remplirMacro: " & ex.ToString, "Erreur")
        End Try
    End Sub

    Private Sub CheckClick(ByVal sender As Object, ByVal e As Windows.RoutedEventArgs)
        Try
            If sender.IsChecked = True Then
                _ListMacro.Add(sender.uid)
            Else
                For j As Integer = 0 To _ListMacro.Count - 1
                    If _ListMacro.Item(j) = sender.uid Then
                        _ListMacro.RemoveAt(j)
                        Exit For
                    End If
                Next
            End If
            RemplirMacro()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uMacro CheckClick: " & ex.Message, "ERREUR", "")
        End Try
    End Sub

    Private Function IsInList(ByVal uid As String) As Boolean
        Try
            Dim flag As Boolean = False

            For i As Integer = 0 To ListBox1.Items.Count - 1
                If ListBox1.Items.Item(i).uid = uid Then
                    flag = True
                    Exit For
                End If
            Next

            Return flag
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uMacro IsInList: " & ex.ToString, "Erreur")
        End Try
    End Function

#Region "Gestion Date/time mode simple"
    Private Sub BtnPHr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnPHr.Click
        Try
            Dim i As Integer
            If TxtHr.Text = "" Then
                i = 0
                TxtHr.Text = Format(i, "00")
                TxtHr_avance.Text = 0
            Else
                i = TxtHr.Text
                i += 1
                If i > 23 Then
                    TxtHr.Text = ""
                    TxtHr_avance.Text = "*"
                Else
                    TxtHr.Text = i
                    TxtHr_avance.Text = i
                End If

            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer BtnPHr_Click: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub BtnPMn_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnPMn.Click
        Try
            Dim i As Integer
            If TxtMn.Text = "" Then
                i = 0
                TxtMn.Text = Format(i, "00")
                TxtMn_avance.Text = 0
            Else
                i = TxtMn.Text
                i += 1
                If i > 59 Then
                    TxtMn.Text = ""
                    TxtMn_avance.Text = "*"
                Else
                    TxtMn.Text = i
                    TxtMn_avance.Text = i
                End If

            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer BtnPMn_Click: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub BtnPSc_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnPSc.Click
        Try
            Dim i As Integer
            If TxtSc.Text = "" Then
                i = 0
                TxtSc.Text = Format(i, "00")
                TxtSc_avance.Text = 0
            Else
                i = TxtSc.Text
                i += 1
                If i > 59 Then
                    TxtSc.Text = ""
                    TxtSc_avance.Text = "*"
                Else
                    TxtSc.Text = i
                    TxtSc_avance.Text = i
                End If

            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer BtnPSc_Click: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub BtnMHr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnMHr.Click
        Try
            Dim i As Integer
            If TxtHr.Text = "" Then
                i = 23
                TxtHr.Text = Format(i, "00")
                TxtHr_avance.Text = 0
            Else
                i = TxtHr.Text
                i -= 1
                If i < 0 Then
                    TxtHr.Text = ""
                    TxtHr_avance.Text = "*"
                Else
                    TxtHr.Text = i
                    TxtHr_avance.Text = i
                End If

            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer BtnMHr_Click: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub BtnMMn_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnMMn.Click
        Try
            Dim i As Integer
            If TxtMn.Text = "" Then
                i = 59
                TxtMn.Text = Format(i, "00")
                TxtMn_avance.Text = 0
            Else
                i = TxtMn.Text
                i -= 1
                If i < 0 Then
                    TxtMn.Text = ""
                    TxtMn_avance.Text = "*"
                Else
                    TxtMn.Text = i
                    TxtMn_avance.Text = i
                End If

            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer BtnMMn_Click: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub BtnMSec_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnMSec.Click
        Try
            Dim i As Integer
            If TxtSc.Text = "" Then
                i = 59
                TxtSc.Text = Format(i, "00")
                TxtSc_avance.Text = 0
            Else
                i = TxtSc.Text
                i -= 1
                If i < 0 Then
                    TxtSc.Text = ""
                    TxtSc_avance.Text = "*"
                Else
                    TxtSc.Text = i
                    TxtSc_avance.Text = i
                End If
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer BtnMSec_Click: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub BtnPJr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnPJr.Click
        Try
            Dim i As Integer
            If TxtJr.Text = "" Then
                i = 1
                TxtJr.Text = Format(i, "00")
                TxtJr_avance.Text = 1
            Else
                i = TxtJr.Text
                i += 1
                If i > 31 Then
                    TxtJr.Text = ""
                    TxtJr_avance.Text = "*"
                Else
                    TxtJr.Text = i
                    TxtJr_avance.Text = i
                End If
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer BtnPJr_Click: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub BtnPMs_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnPMs.Click
        Try
            Dim i As Integer
            If TxtMs.Text = "" Then
                i = 1
                TxtMs.Text = Format(i, "00")
                TxtMs_avance.Text = 1
            Else
                i = TxtMs.Text
                i += 1
                If i > 12 Then
                    TxtMs.Text = ""
                    TxtMs_avance.Text = "*"
                Else
                    TxtMs.Text = i
                    TxtMs_avance.Text = i
                End If
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer BtnPMs_Click: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub BtnMJr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnMJr.Click
        Try
            Dim i As Integer
            If TxtJr.Text = "" Then
                i = 31
                TxtJr.Text = Format(i, "00")
                TxtJr_avance.Text = 31
            Else
                i = TxtJr.Text
                i -= 1
                If i < 1 Then
                    TxtJr.Text = ""
                    TxtJr_avance.Text = "*"
                Else
                    TxtJr.Text = i
                    TxtJr_avance.Text = i
                End If
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer BtnMJr_Click: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub BtnMMs_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnMMs.Click
        Try
            Dim i As Integer
            If TxtMs.Text = "" Then
                i = 12
                TxtMs.Text = Format(i, "00")
                TxtMs_avance.Text = 12
            Else
                i = TxtMs.Text
                i -= 1
                If i < 1 Then
                    TxtMs.Text = ""
                    TxtMs_avance.Text = "*"
                Else
                    TxtMs.Text = i
                    TxtMs_avance.Text = i
                End If
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer BtnMMs_Click: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub TxtHr_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtHr.TextChanged
        Try
            If TxtHr.Text <> "" Then
                If IsNumeric(TxtHr.Text) = False Then
                    TxtHr.Text = "0"
                    If Not IsNothing(TxtHr_avance) Then TxtHr_avance.Text = "0"
                Else
                    If TxtHr.Text < 0 Then
                        TxtHr.Text = ""
                        If Not IsNothing(TxtHr_avance) Then TxtHr_avance.Text = "*"
                    ElseIf TxtHr.Text > 23 Then
                        TxtHr.Text = ""
                        If Not IsNothing(TxtHr_avance) Then TxtHr_avance.Text = "*"
                    Else
                        If Not IsNothing(TxtHr_avance) Then TxtHr_avance.Text = TxtHr.Text
                    End If
                End If
            Else
                If Not IsNothing(TxtHr_avance) Then TxtHr_avance.Text = "*"
            End If
            simulate_cron_simple()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer TxtHr_TextChanged: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub TxtMn_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtMn.TextChanged
        Try
            If TxtMn.Text <> "" Then
                If IsNumeric(TxtMn.Text) = False Then
                    TxtMn.Text = "0"
                    If Not IsNothing(TxtMn_avance) Then TxtMn_avance.Text = "0"
                Else
                    If TxtMn.Text < 0 Then
                        TxtMn.Text = ""
                        If Not IsNothing(TxtMn_avance) Then TxtMn_avance.Text = "*"
                    ElseIf TxtMn.Text > 59 Then
                        TxtMn.Text = ""
                        If Not IsNothing(TxtMn_avance) Then TxtMn_avance.Text = "*"
                    Else
                        If Not IsNothing(TxtMn_avance) Then TxtMn_avance.Text = TxtMn.Text
                    End If
                End If
            Else
                If Not IsNothing(TxtMn_avance) Then TxtMn_avance.Text = "*"
            End If
            simulate_cron_simple()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer TxtMn_TextChanged: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub TxtSc_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtSc.TextChanged
        Try
            If TxtSc.Text <> "" Then
                If IsNumeric(TxtSc.Text) = False Then
                    TxtSc.Text = "0"
                    If Not IsNothing(TxtSc_avance) Then TxtSc_avance.Text = "0"
                Else
                    If TxtSc.Text < 0 Then
                        TxtSc.Text = ""
                        If Not IsNothing(TxtSc_avance) Then TxtSc_avance.Text = "*"
                    ElseIf TxtSc.Text > 59 Then
                        TxtSc.Text = ""
                        If Not IsNothing(TxtSc_avance) Then TxtSc_avance.Text = "*"
                    Else
                        If Not IsNothing(TxtSc_avance) Then TxtSc_avance.Text = TxtSc.Text
                    End If
                End If
            Else
                If Not IsNothing(TxtSc_avance) Then TxtSc_avance.Text = "*"
            End If
            simulate_cron_simple()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer TxtSc_TextChanged: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub TxtJr_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtJr.TextChanged
        Try
            If TxtJr.Text <> "" Then
                If IsNumeric(TxtJr.Text) = False Then
                    TxtJr.Text = "1"
                    TxtJr_avance.Text = "1"
                Else
                    If TxtJr.Text < 0 Then
                        TxtJr.Text = ""
                        TxtJr_avance.Text = "*"
                    ElseIf TxtJr.Text > 31 Then
                        TxtJr.Text = ""
                        TxtJr_avance.Text = "*"
                    Else
                        TxtJr_avance.Text = TxtJr.Text
                    End If
                End If
            Else
                TxtJr_avance.Text = "*"
            End If
            simulate_cron_simple()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer TxtJr_TextChanged: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub TxtMs_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtMs.TextChanged
        Try
            If TxtMs.Text <> "" Then
                If IsNumeric(TxtMs.Text) = False Then
                    TxtMs.Text = "1"
                    TxtMs_avance.Text = "1"
                Else
                    If TxtMs.Text < 0 Then
                        TxtMs.Text = ""
                        TxtMs_avance.Text = "*"
                    ElseIf TxtMs.Text > 12 Then
                        TxtMs.Text = ""
                        TxtMs_avance.Text = "*"
                    Else
                        TxtMs_avance.Text = TxtMs.Text
                    End If
                End If
            Else
                TxtMs_avance.Text = "*"
            End If
            simulate_cron_simple()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer TxtMs_TextChanged: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub CheckBox1_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CheckBox1.Click
        Try
            If CheckBox1.IsChecked Then CheckBox1_avance.IsChecked = True Else CheckBox1_avance.IsChecked = False
            simulate_cron_simple()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer CheckBox1_Checked: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub CheckBox2_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CheckBox2.Click
        Try
            If CheckBox2.IsChecked Then CheckBox2_avance.IsChecked = True Else CheckBox2_avance.IsChecked = False
            simulate_cron_simple()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer CheckBox2_Checked: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub CheckBox3_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CheckBox3.Click
        Try
            If CheckBox3.IsChecked Then CheckBox3_avance.IsChecked = True Else CheckBox3_avance.IsChecked = False
            simulate_cron_simple()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer CheckBox3_Checked: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub CheckBox4_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CheckBox4.Click
        Try
            If CheckBox4.IsChecked Then CheckBox4_avance.IsChecked = True Else CheckBox4_avance.IsChecked = False
            simulate_cron_simple()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer CheckBox4_Checked: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub CheckBox5_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CheckBox5.Click
        Try
            If CheckBox5.IsChecked Then CheckBox5_avance.IsChecked = True Else CheckBox5_avance.IsChecked = False
            simulate_cron_simple()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer CheckBox5_Checked: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub CheckBox6_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CheckBox6.Click
        Try
            If CheckBox6.IsChecked Then CheckBox6_avance.IsChecked = True Else CheckBox6_avance.IsChecked = False
            simulate_cron_simple()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer CheckBox6_Checked: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub CheckBox7_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CheckBox7.Click
        Try
            If CheckBox7.IsChecked Then CheckBox7_avance.IsChecked = True Else CheckBox7_avance.IsChecked = False
            simulate_cron_simple()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer CheckBox7_Checked: " & ex.Message, "ERREUR", "")
        End Try
    End Sub

    Private Sub simulate_cron_simple()
        Try
            'on formate le cron avec les données
            Dim _myconditiontime As String = ""
            Try
                If TxtSc.Text = "" Then _myconditiontime &= "* " Else _myconditiontime &= TxtSc.Text & " "
                If TxtMn.Text = "" Then _myconditiontime &= "* " Else _myconditiontime &= TxtMn.Text & " "
                If TxtHr.Text = "" Then _myconditiontime &= "* " Else _myconditiontime &= TxtHr.Text & " "
                If TxtJr.Text = "" Then _myconditiontime &= "* " Else _myconditiontime &= TxtJr.Text & " "
                If TxtMs.Text = "" Then _myconditiontime &= "* " Else _myconditiontime &= TxtMs.Text & " "
                Dim _prepajr As String = ""
                If CheckBox7.IsChecked = True Then _prepajr = "0"
                If CheckBox1.IsChecked = True Then If _prepajr <> "" Then _prepajr &= ",1" Else _prepajr = "1"
                If CheckBox2.IsChecked = True Then If _prepajr <> "" Then _prepajr &= ",2" Else _prepajr = "2"
                If CheckBox3.IsChecked = True Then If _prepajr <> "" Then _prepajr &= ",3" Else _prepajr = "3"
                If CheckBox4.IsChecked = True Then If _prepajr <> "" Then _prepajr &= ",4" Else _prepajr = "4"
                If CheckBox5.IsChecked = True Then If _prepajr <> "" Then _prepajr &= ",5" Else _prepajr = "5"
                If CheckBox6.IsChecked = True Then If _prepajr <> "" Then _prepajr &= ",6" Else _prepajr = "6"
                If _prepajr = "" Then _prepajr = "*"
                _myconditiontime &= _prepajr
            Catch ex As Exception
                Exit Sub
            End Try

            'on essaye de recuperer les prochaines occurences
            Try
                Dim nextcron As Date
                listeprochainscronssimple.Text = ""
                nextcron = CrontabSchedule.Parse(_myconditiontime).GetNextOccurrence(DateAndTime.Now)
                listeprochainscronssimple.Text &= nextcron.ToString("yyyy-MM-dd HH:mm:ss") & vbCrLf
                For i As Integer = 1 To 7
                    nextcron = CrontabSchedule.Parse(_myconditiontime).GetNextOccurrence(nextcron)
                    listeprochainscronssimple.Text &= nextcron.ToString("yyyy-MM-dd HH:mm:ss") & vbCrLf
                Next
            Catch ex As Exception
                listeprochainscronssimple.Text = "Erreur... "
            End Try
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer simulate_cron_simple: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

#End Region

#Region "Gestion Date/time mode avancé"

    Private Function checkModeSimple() As Boolean

        If CheckBox13.IsChecked Or InStr(TxtMs_avance.Text, ",") Or InStr(TxtMs_avance.Text, "-") Or _
                    CheckBox12.IsChecked Or InStr(TxtJr_avance.Text, ",") Or InStr(TxtJr_avance.Text, "-") Or _
                    CheckBox11.IsChecked Or InStr(TxtHr_avance.Text, ",") Or InStr(TxtHr_avance.Text, "-") Or _
                    CheckBox10.IsChecked Or InStr(TxtMn_avance.Text, ",") Or InStr(TxtMn_avance.Text, "-") Or _
                    CheckBox9.IsChecked Or InStr(TxtSc_avance.Text, ",") Or InStr(TxtSc_avance.Text, "-") Then
            SP_modesimplecontenu.Visibility = Windows.Visibility.Collapsed
            SP_modesimplealerte.Visibility = Windows.Visibility.Visible
            SP_exemplesimple.Visibility = Windows.Visibility.Collapsed
            Return False
        Else
            SP_modesimplecontenu.Visibility = Windows.Visibility.Visible
            SP_modesimplealerte.Visibility = Windows.Visibility.Collapsed
            SP_exemplesimple.Visibility = Windows.Visibility.Visible
            Return True
        End If

    End Function

    Private Sub refreshMode(ByVal condtiontime As String)
        If checkModeSimple Then
            If TxtSc_avance.Text = "*" Then TxtSc.Text = "" Else TxtSc.Text = TxtSc_avance.Text
            If TxtMn_avance.Text = "*" Then TxtMn.Text = "" Else TxtMn.Text = TxtMn_avance.Text
            If TxtHr_avance.Text = "*" Then TxtHr.Text = "" Else TxtHr.Text = TxtHr_avance.Text
            If TxtJr_avance.Text = "*" Then TxtJr.Text = "" Else TxtJr.Text = TxtJr_avance.Text
            If TxtMs_avance.Text = "*" Then TxtMs.Text = "" Else TxtMs.Text = TxtMs_avance.Text
            simulate_cron_simple()
        End If
    End Sub

    Private Sub TxtHr_avance_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtHr_avance.TextChanged
        Try
            If TxtHr_avance.Text = "" Or TxtHr_avance.Text = " " Then TxtHr_avance.Text = "*"

            simulate_cron_avance()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer TxtHr_avance_TextChanged: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub TxtMn_avance_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtMn_avance.TextChanged
        Try
            If TxtMn_avance.Text = "" Or TxtMn_avance.Text = " " Then TxtMn_avance.Text = "*"

            simulate_cron_avance()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer TxtMn_avance_TextChanged: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub TxtSc_avance_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtSc_avance.TextChanged
        Try
            If TxtSc_avance.Text = "" Or TxtSc_avance.Text = " " Then TxtSc_avance.Text = "*"

            simulate_cron_avance()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer TxtSc_avance_TextChanged: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub TxtMs_avance_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtMs_avance.TextChanged
        Try
            If TxtMs_avance.Text = "" Or TxtMs_avance.Text = " " Then TxtMs_avance.Text = "*"

            simulate_cron_avance()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer TxtMs_avance_TextChanged: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub TxtJr_avance_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtJr_avance.TextChanged
        Try
            If TxtJr_avance.Text = "" Or TxtJr_avance.Text = " " Then TxtJr_avance.Text = "*"

            simulate_cron_avance()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer TxtJr_avance_TextChanged: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub CheckBox1_avance_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CheckBox1_avance.Click
        Try
            If CheckBox1_avance.IsChecked Then CheckBox1.IsChecked = True Else CheckBox1.IsChecked = False
            simulate_cron_avance()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer CheckBox1_avance_Checked: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub CheckBox2_avance_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CheckBox2_avance.Click
        Try
            If CheckBox2_avance.IsChecked Then CheckBox2.IsChecked = True Else CheckBox2.IsChecked = False
            simulate_cron_avance()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer CheckBox2_avance_Checked: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub CheckBox3_avance_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CheckBox3_avance.Click
        Try
            If CheckBox3_avance.IsChecked Then CheckBox3.IsChecked = True Else CheckBox3.IsChecked = False
            simulate_cron_avance()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer CheckBox3_avance_Checked: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub CheckBox4_avance_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CheckBox4_avance.Click
        Try
            If CheckBox4_avance.IsChecked Then CheckBox4.IsChecked = True Else CheckBox4.IsChecked = False
            simulate_cron_avance()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer CheckBox4_avance_Checked: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub CheckBox5_avance_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CheckBox5_avance.Click
        Try
            If CheckBox5_avance.IsChecked Then CheckBox5.IsChecked = True Else CheckBox5.IsChecked = False
            simulate_cron_avance()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer CheckBox5_avance_Checked: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub CheckBox6_avance_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CheckBox6_avance.Click
        Try
            If CheckBox6_avance.IsChecked Then CheckBox6.IsChecked = True Else CheckBox6.IsChecked = False
            simulate_cron_avance()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer CheckBox6_avance_Checked: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub CheckBox7_avance_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CheckBox7_avance.Click
        Try
            If CheckBox7_avance.IsChecked Then CheckBox7.IsChecked = True Else CheckBox7.IsChecked = False
            simulate_cron_avance()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer CheckBox7_avance_Checked: " & ex.Message, "ERREUR", "")
        End Try
    End Sub

    Private Sub simulate_cron_avance()
        Try
            'If Not IsNothing(TxtSc_avance) And Not IsNothing(TxtMn_avance) And Not IsNothing(TxtHr_avance) And Not IsNothing(TxtJr_avance) And Not IsNothing(TxtMs_avance) Then
            'on formate le cron avec les données
            Dim _myconditiontime_avance As String = ""
            Try
                If CheckBox13.IsChecked And (InStr(TxtMs_avance.Text, ",") Or InStr(TxtMs_avance.Text, "-")) Then CheckBox13.IsChecked = False : AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Impossible de mettre 'chaque' et les instructions '-' ou ',' en même temps, 'chaque' est desactivé ", "Trigger", "")
                If CheckBox12.IsChecked And (InStr(TxtJr_avance.Text, ",") Or InStr(TxtJr_avance.Text, "-")) Then CheckBox12.IsChecked = False : AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Impossible de mettre 'chaque' et les instructions '-' ou ',' en même temps, 'chaque' est desactivé ", "Trigger", "")
                If CheckBox11.IsChecked And (InStr(TxtHr_avance.Text, ",") Or InStr(TxtHr_avance.Text, "-")) Then CheckBox11.IsChecked = False : AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Impossible de mettre 'chaque' et les instructions '-' ou ',' en même temps, 'chaque' est desactivé ", "Trigger", "")
                If CheckBox10.IsChecked And (InStr(TxtMn_avance.Text, ",") Or InStr(TxtMn_avance.Text, "-")) Then CheckBox10.IsChecked = False : AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Impossible de mettre 'chaque' et les instructions '-' ou ',' en même temps, 'chaque' est desactivé ", "Trigger", "")
                If CheckBox9.IsChecked And (InStr(TxtSc_avance.Text, ",") Or InStr(TxtSc_avance.Text, "-")) Then CheckBox9.IsChecked = False : AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Impossible de mettre 'chaque' et les instructions '-' ou ',' en même temps, 'chaque' est desactivé ", "Trigger", "")

                If Not CheckBox9.IsChecked Then _myconditiontime_avance &= TxtSc_avance.Text & " " Else If TxtSc_avance.Text = "*" Then _myconditiontime_avance &= TxtSc_avance.Text & " " Else _myconditiontime_avance &= "*/" & TxtSc_avance.Text & " "
                If Not CheckBox10.IsChecked Then _myconditiontime_avance &= TxtMn_avance.Text & " " Else If TxtMn_avance.Text = "*" Then _myconditiontime_avance &= TxtMn_avance.Text & " " Else _myconditiontime_avance &= "*/" & TxtMn_avance.Text & " "
                If Not CheckBox11.IsChecked Then _myconditiontime_avance &= TxtHr_avance.Text & " " Else If TxtHr_avance.Text = "*" Then _myconditiontime_avance &= TxtHr_avance.Text & " " Else _myconditiontime_avance &= "*/" & TxtHr_avance.Text & " "
                If Not CheckBox12.IsChecked Then _myconditiontime_avance &= TxtJr_avance.Text & " " Else If TxtJr_avance.Text = "*" Then _myconditiontime_avance &= TxtJr_avance.Text & " " Else _myconditiontime_avance &= "*/" & TxtJr_avance.Text & " "
                If Not CheckBox13.IsChecked Then _myconditiontime_avance &= TxtMs_avance.Text & " " Else If TxtMs_avance.Text = "*" Then _myconditiontime_avance &= TxtMs_avance.Text & " " Else _myconditiontime_avance &= "*/" & TxtMs_avance.Text & " "

                Dim _prepajr_avance As String = ""
                If CheckBox7_avance.IsChecked = True Then _prepajr_avance = "0"
                If CheckBox1_avance.IsChecked = True Then If _prepajr_avance <> "" Then _prepajr_avance &= ",1" Else _prepajr_avance = "1"
                If CheckBox2_avance.IsChecked = True Then If _prepajr_avance <> "" Then _prepajr_avance &= ",2" Else _prepajr_avance = "2"
                If CheckBox3_avance.IsChecked = True Then If _prepajr_avance <> "" Then _prepajr_avance &= ",3" Else _prepajr_avance = "3"
                If CheckBox4_avance.IsChecked = True Then If _prepajr_avance <> "" Then _prepajr_avance &= ",4" Else _prepajr_avance = "4"
                If CheckBox5_avance.IsChecked = True Then If _prepajr_avance <> "" Then _prepajr_avance &= ",5" Else _prepajr_avance = "5"
                If CheckBox6_avance.IsChecked = True Then If _prepajr_avance <> "" Then _prepajr_avance &= ",6" Else _prepajr_avance = "6"
                If _prepajr_avance = "" Then _prepajr_avance = "*"
                _myconditiontime_avance &= _prepajr_avance
                refreshMode(_myconditiontime_avance)

            Catch ex As Exception
                Exit Sub
            End Try

            'on essaye de recuperer les prochaines occurences
            Try
                Dim nextcron As Date
                listeprochainscrons.Text = ""
                'listeprochainscrons.Text = _myconditiontime_avance & vbCrLf
                nextcron = CrontabSchedule.Parse(_myconditiontime_avance).GetNextOccurrence(DateAndTime.Now)
                listeprochainscrons.Text &= nextcron.ToString("yyyy-MM-dd HH:mm:ss") & vbCrLf
                For i As Integer = 1 To 7
                    nextcron = CrontabSchedule.Parse(_myconditiontime_avance).GetNextOccurrence(nextcron)
                    listeprochainscrons.Text &= nextcron.ToString("yyyy-MM-dd HH:mm:ss") & vbCrLf
                Next
            Catch ex As Exception
                listeprochainscrons.Text &= "Erreur... "
            End Try
            'Else
            'listeprochainscrons.Text = "..."
            'End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uTriggerTimer simulate_cron_avance: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

#End Region

    Private Sub CheckBox9_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CheckBox9.Click

        simulate_cron_avance()

    End Sub

    Private Sub CheckBox10_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CheckBox10.Click

        simulate_cron_avance()

    End Sub

    Private Sub CheckBox11_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CheckBox11.Click

        simulate_cron_avance()

    End Sub

    Private Sub CheckBox12_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CheckBox12.Click

        simulate_cron_avance()

    End Sub

    Private Sub CheckBox13_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CheckBox13.Click

        simulate_cron_avance()

    End Sub
End Class