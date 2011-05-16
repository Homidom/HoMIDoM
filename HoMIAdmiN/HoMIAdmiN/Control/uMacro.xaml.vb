Public Class uMacro
    Public Event CloseMe(ByVal MyObject As Object)

    Public Enum EAction
        Nouveau
        Modifier
    End Enum

    Dim _Action As EAction
    Dim _MacroId As String

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me)
    End Sub

    Public Sub New(ByVal Action As EAction, ByVal MacroId As String)

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        _Action = Action
        _MacroId = MacroId

        Dim _time As DateTime
        For j As Integer = 0 To 255
            Dim x As New Label
            x.Width = 60
            x.Foreground = New SolidColorBrush(Colors.White)
            x.Content = _time.ToLongTimeString
            StckPnlLib.Children.Add(x)
            _time = _time.AddSeconds(5)
        Next

        Dim y As New Canvas
        y.Width = 28
        y.Background = New SolidColorBrush(Colors.Transparent)
        StckPnlLibTr.Children.Add(y)

        Dim flag As Boolean
        For j = 0 To 255
            Dim y1 As New Canvas
            y1.Width = 60
            If flag = False Then
                y1.Background = New SolidColorBrush(Colors.Transparent)
                flag = True
            Else
                y1.Background = New SolidColorBrush(Colors.Transparent)
                flag = False
            End If
            Dim R1 As New Rectangle
            R1.Fill = New SolidColorBrush(Colors.White)
            R1.Width = 3
            R1.Height = 16
            y1.Children.Add(R1)
            For k = 1 To 4
                Dim R2 As New Rectangle
                R2.Fill = New SolidColorBrush(Colors.White)
                R2.Width = 1
                R2.Height = 8
                y1.Children.Add(R2)
                y1.SetLeft(R2, k * (60 / 5))
            Next
            StckPnlLibTr.Children.Add(y1)
        Next

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        If _Action = EAction.Nouveau Then 'Nouveau Trigger

        Else 'Modifier Trigger

        End If
    End Sub

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        'If TxtNom.Text = "" Or CbDevice.SelectedIndex < 0 Then
        '    MessageBox.Show("Le nom du trigger ou le device sont obligatoires!", "Trigger", MessageBoxButton.OK, MessageBoxImage.Exclamation)
        '    Exit Sub
        'End If

        'If _Action = EAction.Nouveau Then
        '    Window1.myService.SaveTrigger("", TxtNom.Text, ChkEnable.IsChecked, HoMIDom.HoMIDom.Trigger.TypeTrigger.DEVICE, TxtDescription.Text, "", _ListDeviceId(CbDevice.SelectedIndex).id, CbProperty.Text, _ListMacro)
        'Else
        '    Window1.myService.SaveTrigger(_TriggerId, TxtNom.Text, ChkEnable.IsChecked, HoMIDom.HoMIDom.Trigger.TypeTrigger.DEVICE, TxtDescription.Text, "", _ListDeviceId(CbDevice.SelectedIndex).id, CbProperty.Text, _ListMacro)
        'End If
    End Sub

End Class
