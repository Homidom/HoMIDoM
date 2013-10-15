Imports HoMIDom.HoMIDom.Api

Public Class uMacro
    Public Event CloseMe(ByVal MyObject As Object)

    Dim _Action As EAction
    Dim _MacroId As String
    Public _Width As Integer = 600
    Public _Height As Integer = 450

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
                cEnable.IsChecked = True
            Else 'Modifier Macro
                Dim x As HoMIDom.HoMIDom.Macro = myService.ReturnMacroById(IdSrv, MacroId)

                If x IsNot Nothing Then
                    TxtNom.Text = x.Nom
                    TxtDescription.Text = x.Description
                    cEnable.IsChecked = x.Enable
                    UScenario1.Items = x.ListActions
                End If
            End If

            AddHandler UScenario1.AsChange, AddressOf Scenario_AsChange

        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur dans uMacro - New: " & ex.ToString, "Erreur", "")
        End Try
    End Sub

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        Try
            If String.IsNullOrEmpty(TxtNom.Text) Or HaveCaractSpecial(TxtNom.Text) Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le nom de la macro est obligatoire et ne doit pas comporter de caractère spécial!", "Macro", "")
                Exit Sub
            End If

            If _Action = EAction.Nouveau Then
                Dim tabl As New ArrayList
                tabl = UScenario1.Items
                _MacroId = myService.SaveMacro(IdSrv, "", TxtNom.Text, cEnable.IsChecked, TxtDescription.Text, tabl)
            Else
                Dim tabl As New ArrayList
                tabl = UScenario1.Items
                _MacroId = myService.SaveMacro(IdSrv, _MacroId, TxtNom.Text, cEnable.IsChecked, TxtDescription.Text, tabl)
            End If

            FlagChange = True
            RaiseEvent CloseMe(Me)
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur lors de l'enregistrement de la macro, message: " & ex.ToString, "Erreur", "")
        End Try
    End Sub

    Private Sub BtnTest_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnTest.Click
        If String.IsNullOrEmpty(_MacroId) = False And IsConnect Then
            myService.RunMacro(IdSrv, _MacroId)
        End If
    End Sub

    Sub Scenario_AsChange()
        BtnTest.Visibility = Windows.Visibility.Collapsed
    End Sub

    Private Sub UScenario1_Loaded(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles UScenario1.Loaded
        UScenario1.Width = _Width - 15
        UScenario1.Height = _Height - 160
    End Sub

    Private Sub StackPanel1_Loaded(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles StackPanel1.Loaded
        UScenario1.Width = _Width - 15
        UScenario1.Height = _Height - 160
    End Sub
End Class
