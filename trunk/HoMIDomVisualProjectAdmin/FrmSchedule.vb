Imports HoMIDom.HoMIDom

Public Class FrmSchedule
    Public Enum EAction
        Nouveau
        Modifier
    End Enum
    Public MyID As String
    Public Action As EAction
    Public ListAction As New ArrayList
    Dim Flag0 As Boolean
    Dim _ListScriptIdDispo As New ArrayList
    Dim _ListScriptIdSelect As New ArrayList

    Private Sub BtnAnnuler_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnAnnuler.Click
        Me.Close()
    End Sub

    Private Sub ComboBox1_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cTgType.SelectedValueChanged
        Select Case cTgType.Text
            Case "Custom"
                PanelOffset.Visible = False
            Case "SunSet"
                PanelOffset.Visible = True
            Case "SunRise"
                PanelOffset.Visible = True
        End Select
    End Sub

    Private Sub FrmSchedule_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'LblTimeEnd.Text = Now
        'LblTimeStart.Text = Now
        'ListBoxScriptDispo.Items.Clear()
        'ListBoxScriptSelect.Items.Clear()

        ''on rempli la liste des scripts dispo
        'For i As Integer = 0 To FRMMere.Obj.Scripts.Count - 1
        '    ListBoxScriptDispo.Items.Add(FRMMere.Obj.Scripts.Item(i).name)
        '    _ListScriptIdDispo.Add(FRMMere.Obj.Scripts.Item(i).id)
        'Next

        ''Si on modifie un schedule
        'If Action = EAction.Modifier Then
        '    Dim obj As Schedule = FRMMere.Obj.ReturnScheduleByID(MyID)

        '    TxtNom.Text = obj.Name
        '    cEnable.Checked = obj.Enable
        '    _ListScriptIdSelect = obj.ListScript

        '    'on récupère les noms des scripts
        '    For j As Integer = 0 To _ListScriptIdSelect.Count - 1
        '        For k As Integer = 0 To FRMMere.Obj.Scripts.Count - 1
        '            If _ListScriptIdSelect.Item(j) = FRMMere.Obj.Scripts.Item(k).id Then
        '                ListBoxScriptSelect.Items.Add(FRMMere.Obj.Scripts.Item(k).name)
        '            End If
        '        Next
        '    Next

        '    Select Case obj.TriggerType
        '        Case 0 'Custom
        '            cTgType.SelectedIndex = 0
        '            LblTimeStart.Text = obj.StartDateTime
        '            LblTimeEnd.Text = obj.EndDateTime
        '            If obj.Jour(0) = True Then CJ0.Checked = True
        '            If obj.Jour(1) = True Then CJ1.Checked = True
        '            If obj.Jour(2) = True Then CJ2.Checked = True
        '            If obj.Jour(3) = True Then CJ3.Checked = True
        '            If obj.Jour(4) = True Then CJ4.Checked = True
        '            If obj.Jour(5) = True Then CJ5.Checked = True
        '            If obj.Jour(6) = True Then CJ6.Checked = True
        '            Select Case obj.RecurrencePatternList
        '                Case 0
        '                    RC0.Checked = True
        '                Case 1
        '                    RC1.Checked = True
        '                Case 2
        '                    RC2.Checked = True
        '                Case 3
        '                    RC3.Checked = True
        '            End Select
        '            If obj.AsEnd = False Then
        '                RE0.Checked = True
        '            Else
        '                RE1.Checked = False
        '            End If
        '        Case 1 'SunRise
        '            cTgType.SelectedIndex = 1
        '            If obj.SunRiseBefore = True Then
        '                R0.Checked = True
        '                R1.Checked = False
        '            Else
        '                R0.Checked = False
        '                R1.Checked = True
        '            End If
        '            OffSetTime.Value = obj.SunRiseBeforeTime
        '        Case 2 'Sunset
        '            cTgType.SelectedIndex = 2
        '            If obj.SunSetBefore = True Then
        '                R0.Checked = True
        '                R1.Checked = False
        '            Else
        '                R0.Checked = False
        '                R1.Checked = True
        '            End If
        '            OffSetTime.Value = obj.SunsetBeforeTime
        '    End Select

        'End If

        'For i As Byte = 0 To 23
        '    For j As Byte = 0 To 59
        '        vTime.Items.Add(Format(i, "00") & ":" & Format(j, "00"))
        '    Next
        'Next
    End Sub

    Private Sub BtnCalStart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnCalStart.Click
        GroupBox4.Top = GroupBox3.Top
        GroupBox4.Left = GroupBox3.Left
        GroupBox4.Visible = True
        Dim Mydate As DateTime = CDate(LblTimeStart.Text)

        MonthCalendar1.SetDate(Mydate)
        LblCalendarTime.Text = Mydate
        vTime.Text = Mydate.ToShortTimeString
        Flag0 = False
    End Sub

    Private Sub BtnCalOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnCalOk.Click
        GroupBox4.Visible = False
        If Flag0 = False Then
            LblTimeStart.Text = LblCalendarTime.Text
        Else
            LblTimeEnd.Text = LblCalendarTime.Text
        End If
    End Sub

    Private Sub BtnCalAnnuler_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnCalAnnuler.Click
        GroupBox4.Visible = False
    End Sub

    Private Sub BtnCalEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnCalEnd.Click
        GroupBox4.Top = GroupBox3.Top
        GroupBox4.Left = GroupBox3.Left
        GroupBox4.Visible = True
        Dim Mydate As DateTime = CDate(LblTimeEnd.Text)

        MonthCalendar1.SetDate(Mydate)
        vTime.Text = Mydate.ToShortTimeString
        Flag0 = True
    End Sub

    Private Sub MonthCalendar1_DateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DateRangeEventArgs) Handles MonthCalendar1.DateChanged
        LblCalendarTime.Text = MonthCalendar1.SelectionStart.ToLongDateString & " " & vTime.Text
    End Sub

    Private Sub vTime_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles vTime.TextChanged
        LblCalendarTime.Text = MonthCalendar1.SelectionStart.ToLongDateString & " " & vTime.Text
    End Sub

    Private Sub MonthCalendar1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MonthCalendar1.MouseDown
        LblCalendarTime.Text = MonthCalendar1.SelectionStart.ToLongDateString & " " & vTime.Text
    End Sub

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnOK.Click

        'Erreur si nom du trigger vide
        If TxtNom.Text = "" Then
            MessageBox.Show("Le nom du Trigger est obligatoire!")
            Exit Sub
        End If

        ''Nouveau Schedule
        'If Action = 0 Then
        '    FRMMere.Obj.SaveSchedule("", TxtNom.Text, cEnable.Checked, _ListScriptIdSelect, cTgType.SelectedIndex, ReturnRecType, LblTimeStart.Text, ReturnJour, RE1.Checked, LblTimeEnd.Text, R0.Checked, OffSetTime.Value, R0.Checked, OffSetTime.Value)
        'End If

        ''Modification Schedule
        'If Action = 1 Then
        '    FRMMere.Obj.SaveSchedule(MyID, TxtNom.Text, cEnable.Checked, _ListScriptIdSelect, cTgType.SelectedIndex, ReturnRecType, LblTimeStart.Text, ReturnJour, RE1.Checked, LblTimeEnd.Text, R0.Checked, OffSetTime.Value, R0.Checked, OffSetTime.Value)
        'End If

        Me.Close()
        FRMMere.AffSchedule()
    End Sub

    Private Function ReturnSchType() As Integer
        Select Case (cTgType.Text)
            Case "Custom"
                Return 0
            Case "SunSet"
                Return 2
            Case "SunRise"
                Return 1
            Case Else
                Return 0
        End Select
    End Function

    Private Function ReturnRecType() As Integer
        If RC0.Checked = True Then Return 0 'Jour
        If RC1.Checked = True Then Return 1 'Semaine
        If RC2.Checked = True Then Return 2 'Mois
        If RC3.Checked = True Then Return 3 'Annee
    End Function

    Private Sub ReturnTypeRec(ByVal Value As Integer)
        Select Case Value
            Case 0
                RC0.Checked = True
            Case 1
                RC1.Checked = True
            Case 2
                RC2.Checked = True
            Case 3
                RC3.Checked = True
            Case Else
                RC0.Checked = True
        End Select
    End Sub

    Private Function ReturnJour() As String
        Dim _j As String
        If CJ0.Checked = True Then
            _j = 1
        Else
            _j = 0
        End If
        If CJ1.Checked = True Then
            _j &= 1
        Else
            _j &= 0
        End If
        If CJ2.Checked = True Then
            _j &= 1
        Else
            _j &= 0
        End If
        If CJ3.Checked = True Then
            _j &= 1
        Else
            _j &= 0
        End If
        If CJ4.Checked = True Then
            _j &= 1
        Else
            _j &= 0
        End If
        If CJ5.Checked = True Then
            _j &= 1
        Else
            _j &= 0
        End If
        If CJ6.Checked = True Then
            _j &= 1
        Else
            _j &= 0
        End If

        Return _j
    End Function

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

    Private Sub RC0_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RC0.CheckedChanged
        If RC0.Checked = True Then
            PanelRJ.Visible = False
        Else
            PanelRJ.Visible = True
        End If
    End Sub
End Class