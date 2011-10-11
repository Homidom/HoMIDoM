Public Class uTriggerTimer
    Public Event CloseMe(ByVal MyObject As Object)

    Public Enum EAction
        Nouveau
        Modifier
    End Enum

    Dim _Action As EAction
    Dim _TriggerId As String
    Dim _ListMacro As New List(Of String)

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me)
    End Sub

    Public Sub New(ByVal Action As EAction, ByVal TriggerId As String)
        Try
            ' Cet appel est requis par le concepteur.
            InitializeComponent()


            _Action = Action
            _TriggerId = TriggerId

            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            If _Action = EAction.Nouveau Then 'Nouveau Trigger

            Else 'Modifier Trigger
                Dim x As HoMIDom.HoMIDom.Trigger = Window1.myService.ReturnTriggerById(IdSrv, _TriggerId)

                If x IsNot Nothing Then
                    TxtNom.Text = x.Nom
                    ChkEnable.IsChecked = x.Enable
                    TxtDescription.Text = x.Description
                    _ListMacro = x.ListMacro

                    If _ListMacro IsNot Nothing Then
                        For i As Integer = 0 To _ListMacro.Count - 1
                            ListBox1.Items.Add(Window1.myService.ReturnMacroById(IdSrv, _ListMacro.Item(i)).Nom)
                        Next
                    End If

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
                _myconditiontime &= TxtSc.Text & "#"
            End If
            If TxtMn.Text = "" Then
                _myconditiontime &= "*#"
            Else
                _myconditiontime &= TxtMn.Text & "#"
            End If
            If TxtHr.Text = "" Then
                _myconditiontime &= "*#"
            Else
                _myconditiontime &= TxtHr.Text & "#"
            End If
            If TxtJr.Text = "" Then
                _myconditiontime &= "*#"
            Else
                _myconditiontime &= TxtJr.Text & "#"
            End If
            If TxtMs.Text = "" Then
                _myconditiontime &= "*#"
            Else
                _myconditiontime &= TxtMs.Text & "#"
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
            _myconditiontime &= _prepajr

            If _Action = EAction.Nouveau Then
                Window1.myService.SaveTrigger(IdSrv, "", TxtNom.Text, ChkEnable.IsChecked, 0, TxtDescription.Text, _myconditiontime, "", "", _ListMacro)
                RaiseEvent CloseMe(Me)
            Else
                Window1.myService.SaveTrigger(IdSrv, _TriggerId, TxtNom.Text, ChkEnable.IsChecked, 0, TxtDescription.Text, _myconditiontime, "", "", _ListMacro)
                RaiseEvent CloseMe(Me)
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'enregistrement du trigger, message: " & ex.ToString, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub ListBox1_DragOver(ByVal sender As Object, ByVal e As System.Windows.DragEventArgs) Handles ListBox1.DragOver
        If e.Data.GetDataPresent(GetType(String)) Then
            e.Effects = DragDropEffects.Copy
        Else
            e.Effects = DragDropEffects.None
        End If
    End Sub

    Private Sub ListBox1_Drop(ByVal sender As Object, ByVal e As System.Windows.DragEventArgs) Handles ListBox1.Drop
        If e.Data.GetDataPresent(GetType(String)) Then
            e.Effects = DragDropEffects.Copy

            Dim uri As String = e.Data.GetData(GetType(String)).ToString
            _ListMacro.Add(uri)
            ListBox1.Items.Add(Window1.myService.ReturnMacroById(IdSrv, uri).Nom)
        Else
            e.Effects = DragDropEffects.None
        End If
    End Sub

    Private Sub DeleteMacro_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles DeleteMacro.Click
        If ListBox1.SelectedIndex < 0 Then
            MessageBox.Show("Aucune macro sélectionnée, veuillez en choisir une à supprimer!", "Trigger", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        Else
            Dim i As Integer = ListBox1.SelectedIndex
            _ListMacro.RemoveAt(i)
            ListBox1.Items.Clear()
            For j As Integer = 0 To _ListMacro.Count - 1
                ListBox1.Items.Add(Window1.myService.ReturnMacroById(IdSrv, _ListMacro(j)).Nom)
            Next
            i = Nothing
        End If
    End Sub

    Private Sub UpMacro_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles UpMacro.Click
        If ListBox1.SelectedIndex <= 0 Then
            MessageBox.Show("Aucune macro sélectionnée ou celle-ci est déjà en 1ère position, veuillez en choisir une à déplacer !", "Trigger", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        Else
            Dim i As Integer = ListBox1.SelectedIndex
            Dim a As String = _ListMacro(i - 1)
            Dim b As String = _ListMacro(i)

            _ListMacro(i - 1) = b
            _ListMacro(i) = a
            ListBox1.Items.Clear()
            For j As Integer = 0 To _ListMacro.Count - 1
                ListBox1.Items.Add(Window1.myService.ReturnMacroById(IdSrv, _ListMacro(j)).Nom)
            Next
        End If
    End Sub

    Private Sub DownMacro_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles DownMacro.Click
        If ListBox1.SelectedIndex < 0 Or ListBox1.SelectedIndex = ListBox1.Items.Count - 1 Then
            MessageBox.Show("Aucune macro sélectionnée ou celle-ci est déjà en dernière position, veuillez en choisir une déplacer!", "Trigger", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        Else
            Dim i As Integer = ListBox1.SelectedIndex
            Dim a As String = _ListMacro(i + 1)
            Dim b As String = _ListMacro(i)

            _ListMacro(i + 1) = b
            _ListMacro(i) = a
            ListBox1.Items.Clear()
            For j As Integer = 0 To _ListMacro.Count - 1
                ListBox1.Items.Add(Window1.myService.ReturnMacroById(IdSrv, _ListMacro(j)).Nom)
            Next
        End If
    End Sub

#Region "Gestion Date/time"
    Private Sub BtnPHr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnPHr.Click
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
    End Sub

    Private Sub BtnPMn_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnPMn.Click
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
    End Sub

    Private Sub BtnPSc_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnPSc.Click
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
    End Sub

    Private Sub BtnMHr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnMHr.Click
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
    End Sub

    Private Sub BtnMMn_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnMMn.Click
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

    End Sub

    Private Sub BtnMSec_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnMSec.Click
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
    End Sub

    Private Sub BtnPJr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnPJr.Click
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
    End Sub

    Private Sub BtnPMs_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnPMs.Click
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
    End Sub

    Private Sub BtnMJr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnMJr.Click
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
    End Sub

    Private Sub BtnMMs_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnMMs.Click
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
    End Sub

    Private Sub TxtHr_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtHr.TextChanged
        If TxtHr.Text <> "" Then
            If IsNumeric(TxtHr.Text) = False Then
                TxtHr.Text = "00"
            End If
        End If
    End Sub

    Private Sub TxtMn_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtMn.TextChanged
        If TxtMn.Text <> "" Then
            If IsNumeric(TxtMn.Text) = False Then
                TxtMn.Text = "00"
            End If
        End If
    End Sub

    Private Sub TxtSc_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtSc.TextChanged
        If TxtSc.Text <> "" Then
            If IsNumeric(TxtSc.Text) = False Then
                TxtSc.Text = "00"
            End If
        End If
    End Sub

    Private Sub TxtJr_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtJr.TextChanged
        If TxtJr.Text <> "" Then
            If IsNumeric(TxtJr.Text) = False Then
                TxtJr.Text = "01"
            End If
        End If
    End Sub

    Private Sub TxtMs_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtMs.TextChanged
        If TxtMs.Text <> "" Then
            If IsNumeric(TxtMs.Text) = False Then
                TxtMs.Text = "01"
            End If
        End If
    End Sub
#End Region
End Class
