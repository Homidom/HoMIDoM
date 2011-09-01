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

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        For i As Integer = 0 To 59
            CbMinute.Items.Add(i)
            CbSeconde.Items.Add(i)
        Next
        For i As Integer = 0 To 23
            CbHeure.Items.Add(i)
        Next
        CbJour.Items.Add("")
        For i As Integer = 1 To 31
            CbJour.Items.Add(i)
        Next
        CbMois.Items.Add("")
        For i As Integer = 1 To 12
            CbMois.Items.Add(i)
        Next

        _Action = Action
        _TriggerId = TriggerId

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        If _Action = EAction.Nouveau Then 'Nouveau Trigger

        Else 'Modifier Trigger
            Dim x As HoMIDom.HoMIDom.Trigger = Window1.myService.ReturnTriggerById(_TriggerId)

            If x IsNot Nothing Then
                TxtNom.Text = x.Nom
                ChkEnable.IsChecked = x.Enable
                TxtDescription.Text = x.Description
                _ListMacro = x.ListMacro

                If _ListMacro IsNot Nothing Then
                    For i As Integer = 0 To _ListMacro.Count - 1
                        ListBox1.Items.Add(Window1.myService.ReturnMacroById(_ListMacro.Item(i)).Nom)
                    Next
                End If

                Dim cron As String = x.ConditionTime
                If cron.StartsWith("_cron") Then
                    cron = Mid(cron, 6, Len(cron) - 5)
                    Dim c() As String = cron.Split("#")
                    CbSeconde.Text = c(0)
                    CbMinute.Text = c(1)
                    CbHeure.Text = c(2)
                    CbJour.Text = c(3)
                    CbMois.Text = c(4)
                    If InStr(c(5), "0") Then CheckBox1.IsChecked = True
                    If InStr(c(5), "1") Then CheckBox2.IsChecked = True
                    If InStr(c(5), "2") Then CheckBox3.IsChecked = True
                    If InStr(c(5), "3") Then CheckBox4.IsChecked = True
                    If InStr(c(5), "4") Then CheckBox5.IsChecked = True
                    If InStr(c(5), "5") Then CheckBox6.IsChecked = True
                    If InStr(c(5), "6") Then CheckBox7.IsChecked = True
                End If
            End If

            End If
    End Sub

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        If TxtNom.Text = "" Then
            MessageBox.Show("Le nom du trigger est obligatoire!", "Trigger", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        End If

        Dim _myconditiontime As String = "_cron"
        _myconditiontime &= CbSeconde.Text & "#"
        _myconditiontime &= CbMinute.Text & "#"
        _myconditiontime &= CbHeure.Text & "#"
        _myconditiontime &= CbJour.Text & "#"
        _myconditiontime &= CbMois.Text & "#"

        Dim _prepajr As String = ""
        If CheckBox1.IsChecked = True Then _prepajr = "0"
        If CheckBox2.IsChecked = True Then
            If _prepajr <> "" Then
                _prepajr &= ",1"
            Else
                _prepajr = "1"
            End If
        End If
        If CheckBox3.IsChecked = True Then
            If _prepajr <> "" Then
                _prepajr &= ",2"
            Else
                _prepajr = "2"
            End If
        End If
        If CheckBox4.IsChecked = True Then
            If _prepajr <> "" Then
                _prepajr &= ",3"
            Else
                _prepajr = "3"
            End If
        End If
        If CheckBox5.IsChecked = True Then
            If _prepajr <> "" Then
                _prepajr &= ",4"
            Else
                _prepajr = "4"
            End If
        End If
        If CheckBox6.IsChecked = True Then
            If _prepajr <> "" Then
                _prepajr &= ",5"
            Else
                _prepajr = "5"
            End If
        End If
        If CheckBox7.IsChecked = True Then
            If _prepajr <> "" Then
                _prepajr &= ",6"
            Else
                _prepajr = "6"
            End If
        End If
        _myconditiontime &= _prepajr

        If _Action = EAction.Nouveau Then
            Window1.myService.SaveTrigger("", TxtNom.Text, ChkEnable.IsChecked, 0, TxtDescription.Text, _myconditiontime, "", "", _ListMacro)
        Else
            Window1.myService.SaveTrigger(_TriggerId, TxtNom.Text, ChkEnable.IsChecked, 0, TxtDescription.Text, _myconditiontime, "", "", _ListMacro)
        End If
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
            ListBox1.Items.Add(Window1.myService.ReturnMacroById(uri).Nom)
        Else
            e.Effects = DragDropEffects.None
        End If
    End Sub

End Class
