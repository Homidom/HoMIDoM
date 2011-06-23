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
        If _Action = EAction.Nouveau Then 'Nouvelle macro

        Else 'Modifier Macro
            Dim x As HoMIDom.HoMIDom.Macro = Window1.myService.ReturnMacroById(MacroId)

            If x IsNot Nothing Then
                TxtNom.Text = x.Nom
                TxtDescription.Text = x.Description
                cEnable.IsChecked = x.Enable
                UScenario1.Items = x.ListActions
            End If
        End If
    End Sub

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        If TxtNom.Text = "" Then
            MessageBox.Show("Le nom de la macro est obligatoire!", "Macro", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        End If

        If _Action = EAction.Nouveau Then
            Dim tabl As New ArrayList
            tabl = UScenario1.Items
            Window1.myService.SaveMacro("", TxtNom.Text, cEnable.IsChecked, TxtDescription.Text, tabl)
        Else
            Dim tabl As New ArrayList
            tabl = UScenario1.Items
            Window1.myService.SaveMacro(_MacroId, TxtNom.Text, cEnable.IsChecked, TxtDescription.Text, tabl)
        End If
        RaiseEvent CloseMe(Me)
    End Sub

End Class
