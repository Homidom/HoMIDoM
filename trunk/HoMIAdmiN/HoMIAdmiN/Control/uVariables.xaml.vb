Imports HoMIDom.HoMIDom

Public Class uVariables

    'Variables
    Dim _ListVar As New List(Of Variable)
    Dim _FlagNewVar As Boolean
    Dim _flagEditVar As Boolean
    Dim _flagDeleteVar As Boolean
    Dim _CurrentVar As String
    Public Event CloseMe(ByVal MyObject As Object)

    Public Sub New()

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        Try
            Refresh_List()

            StkEditVar.Visibility = Windows.Visibility.Collapsed
            BtnDelete.Visibility = Windows.Visibility.Collapsed
            BtnEdit.Visibility = Windows.Visibility.Collapsed
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur: " & ex.Message, "Erreur", "")
        End Try
    End Sub

    Private Sub Refresh_List()
        Try
            _ListVar = myService.GetAllVariables(IdSrv)

            ListBoxVar.Items.Clear()

            If _ListVar IsNot Nothing Then
                For Each _var In _ListVar
                    ListBoxVar.Items.Add(_var.Nom)
                Next
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur Refresh_List: " & ex.Message, "Erreur", "")
        End Try
    End Sub

    Private Sub ListBoxVar_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles ListBoxVar.SelectionChanged
        Try
            _FlagNewVar = False
            _flagDeleteVar = False
            _flagEditVar = False

            If ListBoxVar.SelectedItem IsNot Nothing Then
                For Each _var In _ListVar
                    If _var.Nom.ToLower = ListBoxVar.SelectedValue.ToString.ToLower Then
                        TxtVarName.Text = _var.Nom
                        TxtVarName.IsEnabled = False
                        TxtVarValue.Text = _var.Value
                        TxtVarValue.IsEnabled = False
                        ChkEnableVar.IsChecked = _var.Enable
                        ChkEnableVar.IsEnabled = False
                        TxtVarDescription.Text = _var.Description
                        TxtVarDescription.IsEnabled = False

                        BtnDelete.Visibility = Windows.Visibility.Visible
                        BtnEdit.Visibility = Windows.Visibility.Visible
                        StkEditVar.Visibility = Windows.Visibility.Visible

                        BtnValid.Visibility = Windows.Visibility.Collapsed
                        BtnCancel.Visibility = Windows.Visibility.Collapsed
                        Exit For
                    End If
                Next
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur ListBoxVar_SelectionChanged: " & ex.Message, "Erreur", "")
        End Try
    End Sub

    ''' <summary>
    ''' Nouvelle variable
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BtnNew_MouseDown(sender As System.Object, e As System.Windows.Input.MouseButtonEventArgs) Handles BtnNew.MouseDown
        Try
            _flagDeleteVar = False
            _flagEditVar = False
            _FlagNewVar = True

            TxtVarName.Text = ""
            TxtVarName.IsEnabled = True
            TxtVarValue.Text = ""
            TxtVarValue.IsEnabled = True
            ChkEnableVar.IsChecked = True
            ChkEnableVar.IsEnabled = True
            TxtVarDescription.Text = ""
            TxtVarDescription.IsEnabled = True

            BtnNew.Visibility = Windows.Visibility.Collapsed
            BtnDelete.Visibility = Windows.Visibility.Collapsed
            BtnEdit.Visibility = Windows.Visibility.Collapsed
            BtnValid.Visibility = Windows.Visibility.Visible
            BtnCancel.Visibility = Windows.Visibility.Visible
            StkEditVar.Visibility = Windows.Visibility.Visible
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur BtnNew_MouseDown: " & ex.Message, "Erreur", "")
        End Try
    End Sub

    ''' <summary>
    ''' Bouton Annuler Editer/Nouvelle variable
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BtnCancel_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles BtnCancel.Click
        Try
            _flagDeleteVar = False
            _flagEditVar = False
            _FlagNewVar = False

            TxtVarName.Text = ""
            TxtVarName.IsEnabled = False
            TxtVarValue.Text = ""
            TxtVarValue.IsEnabled = False
            ChkEnableVar.IsChecked = True
            ChkEnableVar.IsEnabled = False
            TxtVarDescription.Text = ""
            TxtVarDescription.IsEnabled = False

            BtnNew.Visibility = Windows.Visibility.Visible
            BtnDelete.Visibility = Windows.Visibility.Collapsed
            BtnEdit.Visibility = Windows.Visibility.Collapsed
            StkEditVar.Visibility = Windows.Visibility.Collapsed

            ListBoxVar.SelectedIndex = -1
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur BtnCancel_Click: " & ex.Message, "Erreur", "")
        End Try
    End Sub


    ''' <summary>
    ''' Editer/Modifier une variable
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BtnEdit_MouseDown(sender As System.Object, e As System.Windows.Input.MouseButtonEventArgs) Handles BtnEdit.MouseDown
        Try
            _flagDeleteVar = False
            _flagEditVar = True
            _FlagNewVar = False

            If ListBoxVar.SelectedIndex >= 0 Then
                TxtVarValue.IsEnabled = True
                ChkEnableVar.IsEnabled = True
                TxtVarDescription.IsEnabled = True

                BtnNew.Visibility = Windows.Visibility.Collapsed
                BtnDelete.Visibility = Windows.Visibility.Collapsed
                BtnEdit.Visibility = Windows.Visibility.Collapsed
                BtnValid.Visibility = Windows.Visibility.Visible
                BtnCancel.Visibility = Windows.Visibility.Visible
                StkEditVar.Visibility = Windows.Visibility.Visible
            Else
                MessageBox.Show("Veuillez sélectionner une variable à éditer", "Information", MessageBoxButton.OK, MessageBoxImage.Information)
            End If

        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur BtnEdit_MouseDown: " & ex.Message, "Erreur", "")
        End Try
    End Sub

    ''' <summary>
    ''' Supprimer une variable
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BtnDelete_MouseDown(sender As System.Object, e As System.Windows.Input.MouseButtonEventArgs) Handles BtnDelete.MouseDown
        Try
            _flagDeleteVar = True
            _flagEditVar = False
            _FlagNewVar = False

            If ListBoxVar.SelectedIndex >= 0 Then
                StkEditVar.Visibility = Windows.Visibility.Visible

                If MessageBox.Show("Voulez vous supprimer la variable " & ListBoxVar.SelectedItem.ToString & " ?", "Message", MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.Yes Then

                    Dim retour As String = myService.DeleteVariable(IdSrv, ListBoxVar.SelectedItem.ToString)
                    If String.IsNullOrEmpty(retour) = False Then
                        MessageBox.Show(retour, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
                    End If
                    Refresh_List()

                    StkEditVar.Visibility = Windows.Visibility.Collapsed

                    ListBoxVar.SelectedIndex = -1
                Else

                End If
            Else
                MessageBox.Show("Veuillez sélectionner une variable à supprimer", "Information", MessageBoxButton.OK, MessageBoxImage.Information)
            End If

            _flagDeleteVar = False
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur BtnDelete_MouseDown: " & ex.Message, "Erreur", "")
        End Try
    End Sub

    Private Sub BtnValid_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles BtnValid.Click
        Try
            'Gestion des erreurs*************
            'Verif si nom est null
            If String.IsNullOrEmpty(TxtVarName.Text) Then
                MessageBox.Show("Le nom de la variable doit être renseigné!!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                _flagEditVar = False
                _FlagNewVar = False
                _flagDeleteVar = False
                Exit Sub
            End If

            'Nouvelle variable
            If _FlagNewVar Then
                Dim retour As String = myService.AddVariable(IdSrv, TxtVarName.Text, ChkEnableVar.IsChecked, TxtVarValue.Text, TxtVarDescription.Text)
                If String.IsNullOrEmpty(retour) = False Then
                    MessageBox.Show(retour, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
                Else
                    FlagChange = True
                End If
            End If

            If _flagEditVar Then
                Dim retour As String = myService.SaveVariable(IdSrv, TxtVarName.Text, ChkEnableVar.IsChecked, TxtVarValue.Text, TxtVarDescription.Text)
                If String.IsNullOrEmpty(retour) = False Then
                    MessageBox.Show(retour, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
                Else
                    FlagChange = True
                End If
            End If

            Refresh_List()

            BtnNew.Visibility = Windows.Visibility.Visible
            BtnDelete.Visibility = Windows.Visibility.Visible
            BtnEdit.Visibility = Windows.Visibility.Collapsed
            StkEditVar.Visibility = Windows.Visibility.Collapsed
            ListBoxVar.SelectedIndex = -1

            _flagEditVar = False
            _FlagNewVar = False
            _flagDeleteVar = False
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur BtnValid_Click: " & ex.Message, "Erreur", "")
        End Try
    End Sub

    Private Sub BtnClose_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me)
    End Sub
End Class
