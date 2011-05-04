Public Class uTriggerTimer
    Public Event CloseMe(ByVal MyObject As Object)

    Public Enum EAction
        Nouveau
        Modifier
    End Enum

    Dim _Action As EAction
    Dim _TriggerId As String

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me)
    End Sub

    Public Sub New(ByVal Action As EAction, ByVal TriggerId As String)

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

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
            End If

        End If
    End Sub

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        If TxtNom.Text = "" Then
            MessageBox.Show("Le nom du trigger est obligatoire!", "Trigger", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        End If

        If _Action = EAction.Nouveau Then
            Window1.myService.SaveTrigger("", TxtNom.Text, ChkEnable.IsChecked, TxtDescription.Text)
        Else
            Window1.myService.SaveTrigger(_TriggerId, TxtNom.Text, ChkEnable.IsChecked, TxtDescription.Text)
        End If
    End Sub
End Class
