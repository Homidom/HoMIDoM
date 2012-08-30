Public Class uMacro
    Public Event CloseMe(ByVal MyObject As Object)

    Dim _Action As EAction
    Dim _MacroId As String

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me)
    End Sub

    Public Sub New(ByVal Action As Classe.EAction, ByVal MacroId As String)
        Try

            ' Cet appel est requis par le concepteur.
            InitializeComponent()

            _Action = Action
            _MacroId = MacroId

            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            If _Action = EAction.Nouveau Then 'Nouvelle macro

            Else 'Modifier Macro
                Dim x As HoMIDom.HoMIDom.Macro = myservice.ReturnMacroById(IdSrv, MacroId)

                If x IsNot Nothing Then
                    TxtNom.Text = x.Nom
                    TxtDescription.Text = x.Description
                    cEnable.IsChecked = x.Enable
                    UScenario1.Items = x.ListActions
                End If
            End If

        Catch ex As Exception
            MessageBox.Show("Erreur dans uMacro - New: " & ex.ToString, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        Try
            If TxtNom.Text = "" Then
                MessageBox.Show("Le nom de la macro est obligatoire!", "Macro", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Exit Sub
            End If

            If _Action = EAction.Nouveau Then
                Dim tabl As New ArrayList
                tabl = UScenario1.Items
                _MacroId = myService.SaveMacro(IdSrv, "", TxtNom.Text, cEnable.IsChecked, TxtDescription.Text, tabl)
                FlagChange = True
                RaiseEvent CloseMe(Me)
            Else
                Dim tabl As New ArrayList
                tabl = UScenario1.Items
                _MacroId = myservice.SaveMacro(IdSrv, _MacroId, TxtNom.Text, cEnable.IsChecked, TxtDescription.Text, tabl)
                FlagChange = True
                RaiseEvent CloseMe(Me)
            End If
            RaiseEvent CloseMe(Me)
        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'enregistrement de la macro, message: " & ex.ToString, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnTest_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnTest.Click
        If _MacroId <> "" And IsConnect Then
            myService.RunMacro(IdSrv, _MacroId)
        End If
    End Sub

End Class
