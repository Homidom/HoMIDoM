﻿Public Class uTriggerTimer
    Public Event CloseMe(ByVal MyObject As Object)

    Dim _Action As EAction
    Dim _TriggerId As String
    Dim _ListMacro As New List(Of String)

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me)
    End Sub

    Public Sub New(ByVal Action As Classe.EAction, ByVal TriggerId As String)
        Try
            ' Cet appel est requis par le concepteur.
            InitializeComponent()


            _Action = Action
            _TriggerId = TriggerId



            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            If _Action = EAction.Nouveau Then 'Nouveau Trigger

            Else 'Modifier Trigger
                Dim x As HoMIDom.HoMIDom.Trigger = myservice.ReturnTriggerById(IdSrv, _TriggerId)

                If x IsNot Nothing Then
                    TxtNom.Text = x.Nom
                    ChkEnable.IsChecked = x.Enable
                    TxtDescription.Text = x.Description
                    _ListMacro = x.ListMacro

                    Dim cron As String = x.ConditionTime
                    If cron.StartsWith("_cron") Then
                        cron = Mid(cron, 6, Len(cron) - 5)
                        Dim c() As String = cron.Split("#")
                        If c(0) = "*" Then
                            TxtSc.Text = ""
                        Else
                            TxtSc.Text = c(0)
                        End If
                        If c(1) = "*" Then
                            TxtMn.Text = ""
                        Else
                            TxtMn.Text = c(1)
                        End If
                        If c(2) = "*" Then
                            TxtHr.Text = ""
                        Else
                            TxtHr.Text = c(2)
                        End If
                        If c(3) = "*" Then
                            TxtJr.Text = ""
                        Else
                            TxtJr.Text = c(3)
                        End If
                        If c(4) = "*" Then
                            TxtMs.Text = ""
                        Else
                            TxtMs.Text = c(4)
                        End If

                        If InStr(c(5), "1") Then CheckBox1.IsChecked = True
                        If InStr(c(5), "2") Then CheckBox2.IsChecked = True
                        If InStr(c(5), "3") Then CheckBox3.IsChecked = True
                        If InStr(c(5), "4") Then CheckBox4.IsChecked = True
                        If InStr(c(5), "5") Then CheckBox5.IsChecked = True
                        If InStr(c(5), "6") Then CheckBox6.IsChecked = True
                        If InStr(c(5), "0") Then CheckBox7.IsChecked = True
                    End If
                End If

            End If
            RemplirMacro()
        Catch ex As Exception
            MessageBox.Show("Erreur NewuTriggerTimer: " & ex.ToString)
        End Try
    End Sub

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        Try
            If TxtNom.Text = "" Then
                MessageBox.Show("Le nom du trigger est obligatoire!", "Trigger", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Exit Sub
            End If

            Dim _myconditiontime As String = "_cron"
            If TxtSc.Text = "" Then
                _myconditiontime &= "*#"
            Else
                _myconditiontime &= CInt(TxtSc.Text) & "#"
            End If
            If TxtMn.Text = "" Then
                _myconditiontime &= "*#"
            Else
                _myconditiontime &= CInt(TxtMn.Text) & "#"
            End If
            If TxtHr.Text = "" Then
                _myconditiontime &= "*#"
            Else
                _myconditiontime &= CInt(TxtHr.Text) & "#"
            End If
            If TxtJr.Text = "" Then
                _myconditiontime &= "*#"
            Else
                _myconditiontime &= CInt(TxtJr.Text) & "#"
            End If
            If TxtMs.Text = "" Then
                _myconditiontime &= "*#"
            Else
                _myconditiontime &= CInt(TxtMs.Text) & "#"
            End If

            Dim _prepajr As String = ""
            If CheckBox7.IsChecked = True Then _prepajr = "0"
            If CheckBox1.IsChecked = True Then
                If _prepajr <> "" Then
                    _prepajr &= ",1"
                Else
                    _prepajr = "1"
                End If
            End If
            If CheckBox2.IsChecked = True Then
                If _prepajr <> "" Then
                    _prepajr &= ",2"
                Else
                    _prepajr = "2"
                End If
            End If
            If CheckBox3.IsChecked = True Then
                If _prepajr <> "" Then
                    _prepajr &= ",3"
                Else
                    _prepajr = "3"
                End If
            End If
            If CheckBox4.IsChecked = True Then
                If _prepajr <> "" Then
                    _prepajr &= ",4"
                Else
                    _prepajr = "4"
                End If
            End If
            If CheckBox5.IsChecked = True Then
                If _prepajr <> "" Then
                    _prepajr &= ",5"
                Else
                    _prepajr = "5"
                End If
            End If
            If CheckBox6.IsChecked = True Then
                If _prepajr <> "" Then
                    _prepajr &= ",6"
                Else
                    _prepajr = "6"
                End If
            End If
            If _prepajr = "" Then _prepajr = "*"
            _myconditiontime &= _prepajr

            _ListMacro.Clear()
            For i As Integer = 0 To ListBox1.Items.Count - 1
                Dim x As StackPanel = ListBox1.Items.Item(i)
                Dim w As CheckBox = x.Children.Item(0)
                If w.IsChecked = True Then _ListMacro.Add(ListBox1.Items.Item(i).uid)
            Next

            If _Action = EAction.Nouveau Then
                myservice.SaveTrigger(IdSrv, "", TxtNom.Text, ChkEnable.IsChecked, 0, TxtDescription.Text, _myconditiontime, "", "", _ListMacro)
            Else
                myService.SaveTrigger(IdSrv, _TriggerId, TxtNom.Text, ChkEnable.IsChecked, 0, TxtDescription.Text, _myconditiontime, "", "", _ListMacro)
            End If

            FlagChange = True
            RaiseEvent CloseMe(Me)
        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'enregistrement du trigger, message: " & ex.ToString, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Private Sub UpMacro_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles UpMacro.Click
    '    Try
    '        If ListBox1.SelectedIndex <= 0 Then
    '            MessageBox.Show("Aucune macro sélectionnée ou celle-ci est déjà en 1ère position, veuillez en choisir une à déplacer !", "Trigger", MessageBoxButton.OK, MessageBoxImage.Exclamation)
    '            Exit Sub
    '        Else
    '            Dim i As Integer = ListBox1.SelectedIndex

    '            Dim x As StackPanel = ListBox1.Items.Item(i)
    '            Dim w As CheckBox = x.Children.Item(0)
    '            If w.IsChecked = False Then
    '                MessageBox.Show("Cette macro n'est pas cochée, elle ne peut être déplacée !", "Trigger", MessageBoxButton.OK, MessageBoxImage.Exclamation)
    '                Exit Sub
    '            End If

    '            Dim a As String = _ListMacro(i - 1)
    '            Dim b As String = _ListMacro(i)

    '            _ListMacro(i - 1) = b
    '            _ListMacro(i) = a
    '            RemplirMacro()
    '        End If
    '    Catch ex As Exception
    '        MessageBox.Show("Erreur UpMacro: " & ex.ToString, "Erreur")
    '    End Try
    'End Sub

    'Private Sub DownMacro_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles DownMacro.Click
    '    Try

    '        If ListBox1.SelectedIndex < 0 Or ListBox1.SelectedIndex = ListBox1.Items.Count - 1 Then
    '            MessageBox.Show("Aucune macro sélectionnée ou celle-ci est déjà en dernière position, veuillez en choisir une déplacer!", "Trigger", MessageBoxButton.OK, MessageBoxImage.Exclamation)
    '            Exit Sub
    '        Else
    '            Dim i As Integer = ListBox1.SelectedIndex

    '            Dim x As StackPanel = ListBox1.Items.Item(i)
    '            Dim w As CheckBox = x.Children.Item(0)
    '            If w.IsChecked = False Then
    '                MessageBox.Show("Cette macro n'est pas cochée, elle ne peut être déplacée !", "Trigger", MessageBoxButton.OK, MessageBoxImage.Exclamation)
    '                Exit Sub
    '            End If
    '            x = ListBox1.Items.Item(i + 1)
    '            w = x.Children.Item(0)
    '            If w.IsChecked = False Then
    '                MessageBox.Show("Cette macro ne peut pas être descendu dans la liste car la macro suivante n'est pas cochée, elle ne peut être déplacée !", "Trigger", MessageBoxButton.OK, MessageBoxImage.Exclamation)
    '                Exit Sub
    '            End If

    '            Dim a As String = _ListMacro(i + 1)
    '            Dim b As String = _ListMacro(i)

    '            _ListMacro(i + 1) = b
    '            _ListMacro(i) = a
    '            RemplirMacro()
    '        End If
    '    Catch ex As Exception
    '        MessageBox.Show("Erreur DownMacro: " & ex.ToString, "Erreur")
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

                    'Vérifie que la mcro existe toujours
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
            MessageBox.Show("Erreur remplirMacro: " & ex.ToString, "Erreur")
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
            MessageBox.Show("ERREUR Sub uMacro CheckClick: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
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
            MessageBox.Show("Erreur uMacro IsInList: " & ex.ToString, "Erreur")
        End Try
    End Function

#Region "Gestion Date/time"
    Private Sub BtnPHr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnPHr.Click
        Try
            Dim i As Integer
            If TxtHr.Text = "" Then
                i = 0
                TxtHr.Text = Format(i, "00")
            Else
                i = TxtHr.Text
                i += 1
                If i > 23 Then
                    TxtHr.Text = ""
                Else
                    TxtHr.Text = i
                End If

            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uTriggerTimer BtnPHr_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnPMn_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnPMn.Click
        Try
            Dim i As Integer
            If TxtMn.Text = "" Then
                i = 0
                TxtMn.Text = Format(i, "00")
            Else
                i = TxtMn.Text
                i += 1
                If i > 59 Then
                    TxtMn.Text = ""
                Else
                    TxtMn.Text = i
                End If

            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uTriggerTimer BtnPMn_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnPSc_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnPSc.Click
        Try
            Dim i As Integer
            If TxtSc.Text = "" Then
                i = 0
                TxtSc.Text = Format(i, "00")
            Else
                i = TxtSc.Text
                i += 1
                If i > 59 Then
                    TxtSc.Text = ""
                Else
                    TxtSc.Text = i
                End If

            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uTriggerTimer BtnPSc_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnMHr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnMHr.Click
        Try
            Dim i As Integer
            If TxtHr.Text = "" Then
                i = 23
                TxtHr.Text = Format(i, "00")
            Else
                i = TxtHr.Text
                i -= 1
                If i < 0 Then
                    TxtHr.Text = ""
                Else
                    TxtHr.Text = i
                End If

            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uTriggerTimer BtnMHr_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnMMn_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnMMn.Click
        Try
            Dim i As Integer
            If TxtMn.Text = "" Then
                i = 59
                TxtMn.Text = Format(i, "00")
            Else
                i = TxtMn.Text
                i -= 1
                If i < 0 Then
                    TxtMn.Text = ""
                Else
                    TxtMn.Text = i
                End If

            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uTriggerTimer BtnMMn_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnMSec_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnMSec.Click
        Try
            Dim i As Integer
            If TxtSc.Text = "" Then
                i = 59
                TxtSc.Text = Format(i, "00")
            Else
                i = TxtSc.Text
                i -= 1
                If i < 0 Then
                    TxtSc.Text = ""
                Else
                    TxtSc.Text = i
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uTriggerTimer BtnMSec_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnPJr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnPJr.Click
        Try
            Dim i As Integer
            If TxtJr.Text = "" Then
                i = 1
                TxtJr.Text = Format(i, "00")
            Else
                i = TxtJr.Text
                i += 1
                If i > 31 Then
                    TxtJr.Text = ""
                Else
                    TxtJr.Text = i
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uTriggerTimer BtnPJr_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnPMs_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnPMs.Click
        Try
            Dim i As Integer
            If TxtMs.Text = "" Then
                i = 1
                TxtMs.Text = Format(i, "00")
            Else
                i = TxtMs.Text
                i += 1
                If i > 12 Then
                    TxtMs.Text = ""
                Else
                    TxtMs.Text = i
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uTriggerTimer BtnPMs_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnMJr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnMJr.Click
        Try
            Dim i As Integer
            If TxtJr.Text = "" Then
                i = 31
                TxtJr.Text = Format(i, "00")
            Else
                i = TxtJr.Text
                i -= 1
                If i < 1 Then
                    TxtJr.Text = ""
                Else
                    TxtJr.Text = i
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uTriggerTimer BtnMJr_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnMMs_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnMMs.Click
        Try
            Dim i As Integer
            If TxtMs.Text = "" Then
                i = 12
                TxtMs.Text = Format(i, "00")
            Else
                i = TxtMs.Text
                i -= 1
                If i < 1 Then
                    TxtMs.Text = ""
                Else
                    TxtMs.Text = i
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uTriggerTimer BtnMMs_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub TxtHr_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtHr.TextChanged
        Try
            If TxtHr.Text <> "" Then
                If IsNumeric(TxtHr.Text) = False Then
                    TxtHr.Text = "0"
                ElseIf TxtHr.Text <> "" Then
                    If TxtHr.Text < 0 Then TxtHr.Text = ""
                    If TxtHr.Text > 23 Then TxtHr.Text = ""
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uTriggerTimer TxtHr_TextChanged: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub TxtMn_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtMn.TextChanged
        Try
            If TxtMn.Text <> "" Then
                If IsNumeric(TxtMn.Text) = False Then
                    TxtMn.Text = "0"
                ElseIf TxtMn.Text <> "" Then
                    If TxtMn.Text < 0 Then TxtMn.Text = ""
                    If TxtMn.Text > 59 Then TxtMn.Text = ""
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uTriggerTimer TxtMn_TextChanged: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub TxtSc_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtSc.TextChanged
        Try
            If TxtSc.Text <> "" Then
                If IsNumeric(TxtSc.Text) = False Then
                    TxtSc.Text = "0"
                ElseIf TxtSc.Text <> "" Then
                    If TxtSc.Text < 0 Then TxtSc.Text = ""
                    If TxtSc.Text > 59 Then TxtSc.Text = ""
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uTriggerTimer TxtSc_TextChanged: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub TxtJr_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtJr.TextChanged
        Try
            If TxtJr.Text <> "" Then
                If IsNumeric(TxtJr.Text) = False Then
                    TxtJr.Text = "1"
                ElseIf TxtJr.Text <> "" Then
                    If TxtJr.Text < 0 Then TxtJr.Text = ""
                    If TxtJr.Text > 31 Then TxtJr.Text = ""
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uTriggerTimer TxtJr_TextChanged: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub TxtMs_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtMs.TextChanged
        Try
            If TxtMs.Text <> "" Then
                If IsNumeric(TxtMs.Text) = False Then
                    TxtMs.Text = "1"
                ElseIf TxtMs.Text <> "" Then
                    If TxtMs.Text < 0 Then TxtMs.Text = ""
                    If TxtMs.Text > 12 Then TxtMs.Text = ""
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uTriggerTimer TxtMs_TextChanged: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
#End Region


End Class
