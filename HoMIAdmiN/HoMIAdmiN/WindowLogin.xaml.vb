Public Class Window3

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        Try
            'If TxtUsername.Text = "" Or TxtPassword.Password = "" Then
            '    MessageBox.Show("Le username ou le password doivent être renseigné", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            '    Exit Sub
            'End If
            If TxtIP.Text = "" Or TxtPort.Text = "" Then
                MessageBox.Show("L'adresse IP et le port du serveur doivent être renseigné", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Exit Sub
            End If
            If String.IsNullOrEmpty(TxtID.Text) = True Then
                MessageBox.Show("L'ID du serveur doit être renseigné", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Exit Sub
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

            IdSrv = TxtID.Text
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
        ShowSrv()
    End Sub

    Private Sub ShowSrv()
        If Site.Children.Count > 0 Then Site.Children.Clear()

        Try
            Dim j As Integer = 0

            For i As Integer = 0 To Window1.ListServer.Count - 1
                Dim x As New uCtrlImgMnu
                x.Text = Window1.ListServer.Item(i).Nom
                x.Adresse = Window1.ListServer.Item(i).Adresse
                x.Port = Window1.ListServer.Item(i).Port
                x.Defaut = Window1.ListServer.Item(i).Defaut
                x.Icon = Window1.ListServer.Item(i).Icon
                x.ID = Window1.ListServer.Item(i).Id
                If Window1.ListServer.Item(i).Defaut = True Then
                    x.IsSelect = True
                    TxtName.Text = x.Text
                    TxtIP.Text = x.Adresse
                    TxtPort.Text = x.Port
                    ChkDefaut.IsChecked = True
                    TxtID.Text = x.ID
                End If
                x.Tag = i
                j = i
                AddHandler x.click, AddressOf IconMnuDoubleClick
                AddHandler x.Rightclick, AddressOf IconMnuRightClick
                Site.Children.Add(x)
                x = Nothing
            Next

            Dim y As New uCtrlImgMnu
            y.Text = "Connexion Manuelle"
            y.Adresse = ""
            y.Port = ""
            y.ID = ""
            y.Tag = j + 1
            AddHandler y.click, AddressOf IconMnuDoubleClick
            AddHandler y.Rightclick, AddressOf IconMnuRightClick
            Site.Children.Add(y)
            y = Nothing
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub ShowSrv: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub IconMnuRightClick(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Try
            Dim dlg As New Microsoft.Win32.OpenFileDialog()
            dlg.Filter = "(*.png) |*.png|(*.*) |*.*"
            If dlg.ShowDialog() = True Then
                sender.Icon = dlg.FileName
                For i As Integer = 0 To Window1.ListServer.Count - 1
                    If Window1.ListServer.Item(i).Nom = sender.text Then
                        Window1.ListServer.Item(i).Icon = sender.icon
                    End If
                Next
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur: " & ex.ToString, "Erreur")
        End Try
    End Sub


    'Clic sur un menu de la barre du bas
    Private Sub IconMnuDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Try
            For i As Integer = 0 To Site.Children.Count - 1
                Dim x As uCtrlImgMnu
                x = Site.Children.Item(i)
                If x.Tag <> sender.tag Then
                    x.IsSelect = False
                Else
                    TxtName.Text = x.Text
                    TxtIP.Text = x.Adresse
                    TxtPort.Text = x.Port
                    TxtID.Text = x.ID
                    ChkDefaut.IsChecked = x.Defaut
                    If TxtName.Text = "Connexion Manuelle" Then
                        TxtName.IsEnabled = False
                        DelSite.Visibility = Windows.Visibility.Hidden
                        NewSite.Visibility = Windows.Visibility.Hidden
                        ChkDefaut.Visibility = Windows.Visibility.Hidden
                    Else
                        TxtName.IsEnabled = True
                        DelSite.Visibility = Windows.Visibility.Visible
                        NewSite.Visibility = Windows.Visibility.Visible
                        ChkDefaut.Visibility = Windows.Visibility.Visible
                    End If
                End If
            Next
        Catch ex As Exception
            MessageBox.Show("Erreur: " & ex.ToString, "Erreur")
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

    Private Sub TxtPassword_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles TxtPassword.KeyDown
        If e.Key = Key.Enter And TxtPassword.Password <> "" Then
            Try
                If TxtUsername.Text = "" Or TxtPassword.Password = "" Then
                    MessageBox.Show("Le username ou le password doivent être renseigné", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                    Exit Sub
                End If
                If TxtIP.Text = "" Or TxtPort.Text = "" Then
                    MessageBox.Show("L'adresse IP et le port du serveur doivent être renseigné", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                    Exit Sub
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
                MessageBox.Show("ERREUR Sub Login TxtPassword_KeyDown: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End If
    End Sub

    Private Sub NewSite_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles NewSite.Click
        Try
            Dim flagnew As Boolean

            For i As Integer = 0 To Window1.ListServer.Count - 1
                If Window1.ListServer.Item(i).Nom = TxtName.Text Then
                    Window1.ListServer.Item(i).Adresse = TxtIP.Text
                    Window1.ListServer.Item(i).Port = TxtPort.Text
                    Window1.ListServer.Item(i).Defaut = ChkDefaut.IsChecked
                    Window1.ListServer.Item(i).Id = TxtID.Text
                    flagnew = True
                End If
            Next

            If flagnew = False Then
                If TxtName.Text = "" Or TxtName.Text = " " Or TxtIP.Text = "" Or TxtIP.Text = " " Or TxtPort.Text = "" Or TxtPort.Text = " " Or TxtID.Text = "" Or TxtID.Text = " " Then
                    MessageBox.Show("Le nom, l'ID, l'adresse et le port du serveur ne peuvent être vide!", "Admin", MessageBoxButton.OK, MessageBoxImage.Error)
                    Exit Sub
                End If
                Dim x As New ClServer
                x.Nom = TxtName.Text
                x.Adresse = TxtIP.Text
                x.Port = TxtPort.Text
                x.Defaut = ChkDefaut.IsChecked
                x.Id = TxtID.Text

                Window1.ListServer.Add(x)
            End If

            If ChkDefaut.IsChecked = True Then
                For i As Integer = 0 To Window1.ListServer.Count - 1
                    If Window1.ListServer.Item(i).Nom <> TxtName.Text And Window1.ListServer.Item(i).Defaut = True Then
                        Window1.ListServer.Item(i).Defaut = False
                    End If
                Next
            End If

            ShowSrv()
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub NewSite_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub DelSite_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles DelSite.Click
        Try
            If TxtName.Text = "Connexion Manuelle" Then
                MessageBox.Show("Vous ne pouvez pas supprimer ce serveur!", "Admin", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Exit Sub
            End If
            For i As Integer = 0 To Window1.ListServer.Count - 1
                If Window1.ListServer.Item(i).Nom = TxtName.Text Then
                    Window1.ListServer.RemoveAt(i)
                    Exit For
                End If
            Next

            ShowSrv()
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub DelSite_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

End Class
