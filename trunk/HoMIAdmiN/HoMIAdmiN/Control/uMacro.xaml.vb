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
