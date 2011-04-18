Public Class Window3

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        Try
            If TxtUsername.Text = "" Or TxtPassword.Password = "" Then
                MessageBox.Show("Le username ou le password doivent être renseigné", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Exit Sub
            End If
            If CbServer.SelectedIndex = 0 And (TxtIP.Text = "" Or TxtPort.Text = "") Then
                MessageBox.Show("L'adresse IP et le port du serveur doivent être renseigné", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Exit Sub
            End If
            If CbServer.SelectedIndex = 0 Then
                MessageBox.Show("Vous avez sélectionné une connexion manuelle, si celle-ci est effective elle sera ajoutée automatiquement dans votre liste avec le nom ServeurX (X sera le nombre de serveur déjà paramétré)!", "Information", MessageBoxButton.OK, MessageBoxImage.Information)
            End If

            If CheckBox1.IsChecked = True Then
                My.Settings.SavePassword = True
                My.Settings.Password = TxtPassword.Password
                My.Settings.UserPassword = TxtUsername.Text
                My.Settings.Save()
            Else
                My.Settings.SavePassword = False
                My.Settings.UserPassword = ""
                My.Settings.Password = ""
                My.Settings.Save()
            End If

            DialogResult = True
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub Login BtnOK_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnCancel.Click
        Try
            DialogResult = False
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub Login BtnCancel_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub Window3_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Try
            Dim j As Integer = 0
            CbServer.Items.Add("Connexion manuelle")
            For i As Integer = 0 To Window1.ListServer.Count - 1
                If Window1.ListServer.Item(i).Nom = "" Then
                    Window1.ListServer.Item(i).Nom = "Serveur par défaut"
                End If
                CbServer.Items.Add(Window1.ListServer.Item(i).Nom)
                If Window1.ListServer.Item(i).Defaut = True Then
                    j = i + 1
                End If
            Next
            CbServer.SelectedIndex = j
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub Login Window3_Loaded: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub TxtUsername_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles TxtUsername.KeyDown
        Try
            If My.Settings.SavePassword = True And TxtUsername.Text = My.Settings.UserPassword Then
                CheckBox1.IsChecked = True
                TxtPassword.Password = My.Settings.Password
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub Login TxtUsername_KeyDown: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub CbServer_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles CbServer.SelectionChanged
        If CbServer.SelectedIndex = 0 Then
            Label5.Visibility = Windows.Visibility.Visible
            Label6.Visibility = Windows.Visibility.Visible
            TxtIP.Visibility = Windows.Visibility.Visible
            TxtPort.Visibility = Windows.Visibility.Visible
            TxtIP.Text = ""
            TxtPort.Text = ""
            Me.Width = 475
        Else
            Label5.Visibility = Windows.Visibility.Hidden
            Label6.Visibility = Windows.Visibility.Hidden
            TxtIP.Visibility = Windows.Visibility.Hidden
            TxtPort.Visibility = Windows.Visibility.Hidden
            Me.Width = 390
            CbServer.ToolTip = Window1.ListServer.Item(CbServer.SelectedIndex - 1).Adresse & " : " & Window1.ListServer.Item(CbServer.SelectedIndex - 1).Port
        End If
    End Sub
End Class
