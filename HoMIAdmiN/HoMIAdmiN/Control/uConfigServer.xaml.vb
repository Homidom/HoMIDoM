Imports System.Text.RegularExpressions
Imports System.IO
Imports System.Threading

Partial Public Class uConfigServer
    Public Event CloseMe(ByVal MyObject As Object)

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        Try
            If IsConnect = True Then
                If String.IsNullOrEmpty(TxtIPSOAP.Text) = True Then
                    MessageBox.Show("L'adresse IP SOAP est erronée !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
                    Exit Sub
                End If
                If IsNumeric(TxtSOAP.Text) = False Or String.IsNullOrEmpty(TxtSOAP.Text) = True Then
                    MessageBox.Show("Le port SOAP est erroné !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
                    Exit Sub
                End If
                If String.IsNullOrEmpty(TxtIdSrv.Text) = True Then
                    MessageBox.Show("L'id du serveur ne peut être vide !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
                    Exit Sub
                End If
                If IsNumeric(TxtSave.Text) = False Or String.IsNullOrEmpty(TxtSave.Text) = True Or CInt(TxtSave.Text) < 0 Then
                    MessageBox.Show("La valeur de saubegarde doit être un chiffre et positif !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
                    Exit Sub
                End If
                If TxtSOAP.Text <> myService.GetPortSOAP Then
                    MessageBox.Show("Vous avez modifié le port SOAP, n'oubliez pas:" & vbCrLf & "- D'enregistrer la configuration pour qu'elle soit prise en compte au prochain démarrage" & vbCrLf & "- De redémarrer le service", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                    myService.SetPortSOAP(IdSrv, TxtSOAP.Text)
                    If TxtSOAP.Text <> myService.GetPortSOAP Then
                        MessageBox.Show("Une erreur est survenue lors du changement du port, veuillez consulter le log", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                    End If
                End If
                If TxtIPSOAP.Text <> myService.GetIPSOAP Then
                    MessageBox.Show("Vous avez modifié l'adresse IP SOAP (donc celle du serveur), n'oubliez pas:" & vbCrLf & "- D'enregistrer la configuration pour qu'elle soit prise en compte au prochain démarrage" & vbCrLf & "- De redémarrer le service" & vbCrLf & "- De modifier l'adresse du serveur au prochain lancement de l'admin", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                    myService.SetIPSOAP(IdSrv, TxtIPSOAP.Text)
                    If TxtIPSOAP.Text <> myService.GetIPSOAP Then
                        MessageBox.Show("Une erreur est survenue lors du changement de l'adresse IP SOAP, veuillez consulter le log", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                    End If
                End If
                If IsNumeric(TxtFile.Text) = False Or CDbl(TxtFile.Text) < 1 Then
                    MessageBox.Show("Veuillez saisir un numérique et positif pour la taille max du fichier de log!", "Admin", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                    Exit Sub
                End If

                myService.SetIdServer(IdSrv, TxtIdSrv.Text)
                IdSrv = TxtIdSrv.Text

                Dim tmpSave As Integer = TxtSave.Text

                tmpSave = Format(tmpSave, "#0")
                Dim retour As String = myService.SetTimeSave(IdSrv, tmpSave)
                If retour <> "0" Then MessageBox.Show(retour, "Erreur SetTimeSave")

                'myService.SetLongitude(IdSrv, CDbl(TxtLong.Text.Replace(".", ",")))
                'myService.SetLatitude(IdSrv, CDbl(TxtLat.Text.Replace(".", ",")))
                myService.SetLongitude(IdSrv, CDbl(TxtLong.Text.Replace(".", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)))
                myService.SetLatitude(IdSrv, CDbl(TxtLat.Text.Replace(".", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)))
                myService.SetHeureCorrectionLever(IdSrv, CInt(HCL.Text))
                myService.SetHeureCorrectionCoucher(IdSrv, CInt(HCC.Text))
                myService.SetSMTPMailServeur(IdSrv, TxtMail.Text)
                myService.SetSMTPServeur(IdSrv, TxtAdresse.Text)
                myService.SetSMTPLogin(IdSrv, TxtLogin.Text)
                myService.SetSMTPPassword(IdSrv, TxtPassword.Password)
                myService.SetSMTPPort(IdSrv, TxtSmtpPort.Text)
                myService.SetSMTPSSL(IdSrv, ChkSSL.IsChecked)
                myService.SetMaxFileSizeLog(CDbl(TxtFile.Text))
                myService.SetMaxMonthLog(CDbl(TxtMaxLogMonth.Text))
                myService.SetDefautVoice(CbVoice.Text)
                myService.SetSaveRealTime(ChKSaveRealTime.IsChecked)
                myService.SetDevise(TxtDevise.Text)
                myService.SetGererEnergie(ChkEnergie.IsChecked)
                myService.SetTarifJour(TxtTarifJour.Text)
                myService.SetTarifNuit(TxtTarifNuit.Text)

                myService.SetPortServeurWeb(TxtPortSrvWeb.Text)
                myService.SetEnableServeurWeb(ChKEnableSrvWeb.IsChecked)

                myService.SetModeDecouverte(ChkModeDecouv.IsChecked)

                Dim _list As New List(Of Boolean)
                If ChkTyp0.IsChecked Then
                    _list.Add(True)
                Else
                    _list.Add(False)
                End If
                If ChkTyp1.IsChecked Then
                    _list.Add(True)
                Else
                    _list.Add(False)
                End If
                If ChkTyp2.IsChecked Then
                    _list.Add(True)
                Else
                    _list.Add(False)
                End If
                If ChkTyp3.IsChecked Then
                    _list.Add(True)
                Else
                    _list.Add(False)
                End If
                If ChkTyp4.IsChecked Then
                    _list.Add(True)
                Else
                    _list.Add(False)
                End If
                If ChkTyp5.IsChecked Then
                    _list.Add(True)
                Else
                    _list.Add(False)
                End If
                If ChkTyp6.IsChecked Then
                    _list.Add(True)
                Else
                    _list.Add(False)
                End If
                If ChkTyp7.IsChecked Then
                    _list.Add(True)
                Else
                    _list.Add(False)
                End If
                If ChkTyp8.IsChecked Then
                    _list.Add(True)
                Else
                    _list.Add(False)
                End If
                If ChkTyp9.IsChecked Then
                    _list.Add(True)
                Else
                    _list.Add(False)
                End If

                myService.SetTypeLogEnable(_list)

                My.Settings.SaveRealTime = ChKSaveRealTime.IsChecked
                My.Settings.Save()

                FlagChange = True
                SaveRealTime()
            End If
            RaiseEvent CloseMe(Me)
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uconfigserver BtnOK_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnCancel.Click
        Try
            RaiseEvent CloseMe(Me)
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uconfigserver BtnCancel_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Public Sub New()

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        Try
            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            If IsConnect = True Then
                TxtIPSOAP.Text = myService.GetIPSOAP
                TxtSOAP.Text = myService.GetPortSOAP
                TxtLat.Text = myService.GetLatitude
                TxtLong.Text = myService.GetLongitude
                HCL.Text = myService.GetHeureCorrectionLever
                HCC.Text = myService.GetHeureCorrectionCoucher
                TxtIdSrv.Text = IdSrv
                TxtSave.Text = myService.GetTimeSave(IdSrv)
                TxtFile.Text = myService.GetMaxFileSizeLog
                TxtMaxLogMonth.Text = myService.GetMaxMonthLog

                TxtAdresse.Text = myService.GetSMTPServeur(IdSrv)
                TxtMail.Text = myService.GetSMTPMailServeur(IdSrv)
                TxtLogin.Text = myService.GetSMTPLogin(IdSrv)
                TxtPassword.Password = myService.GetSMTPPassword(IdSrv)
                TxtSmtpPort.Text = myService.GetSMTPPort(IdSrv)
                ChkSSL.IsChecked = myService.GetSMTPSSL(IdSrv)

                TxtDevise.Text = myService.GetDevise
                ChkEnergie.IsChecked = myService.GetGererEnergie
                TxtTarifJour.Text = myService.GetTarifJour
                TxtTarifNuit.Text = myService.GetTarifNuit

                TxtPortSrvWeb.Text = myService.GetPortServeurWeb
                ChKEnableSrvWeb.IsChecked = myService.GetEnableServeurWeb

                ChkModeDecouv.IsChecked = myService.GetModeDecouverte

                Dim idx = -1
                For i As Integer = 0 To myService.GetAllVoice.Count - 1
                    CbVoice.Items.Add(myService.GetAllVoice.Item(i))
                    If myService.GetAllVoice.Item(i) = myService.GetDefautVoice Then idx = i
                Next
                CbVoice.SelectedIndex = idx

                Dim _list As List(Of Boolean) = myService.GetTypeLogEnable
                ChkTyp0.IsChecked = _list.Item(0)
                ChkTyp1.IsChecked = _list.Item(1)
                ChkTyp2.IsChecked = _list.Item(2)
                ChkTyp3.IsChecked = _list.Item(3)
                ChkTyp4.IsChecked = _list.Item(4)
                ChkTyp5.IsChecked = _list.Item(5)
                ChkTyp6.IsChecked = _list.Item(6)
                ChkTyp7.IsChecked = _list.Item(7)
                ChkTyp8.IsChecked = _list.Item(8)
                ChkTyp9.IsChecked = _list.Item(9)

                ChKSaveRealTime.IsChecked = My.Settings.SaveRealTime
            End If

        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uconfigserver New: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub TxtMaxLogMonth_TextChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtMaxLogMonth.TextChanged
        Try
            If IsNumeric(TxtMaxLogMonth.Text) = False Then
                MessageBox.Show("Veuillez saisir un chiffre comme durée de mois maximum", "Admin", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                TxtMaxLogMonth.Text = myService.GetMaxMonthLog
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur uConfigServer TxtMaxLogMonth_TextChanged: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub TxtFile_TextChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtFile.TextChanged
        Try
            If IsNumeric(TxtFile.Text) = False Then
                MessageBox.Show("Veuillez saisir un chiffre comme taille maximale", "Admin", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                TxtFile.Text = myService.GetMaxFileSizeLog
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur uConfigServer TxtFile_TextChanged: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnImport_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnImport.Click
        Try
            If IsConnect = False Then
                Exit Sub
            End If

            Me.Cursor = Cursors.Wait

            ' Configure open file dialog box
            Dim dlg As New Microsoft.Win32.OpenFileDialog()
            dlg.FileName = "Homidom" ' Default file name
            dlg.DefaultExt = ".xml" ' Default file extension
            dlg.Filter = "Fichier de configuration (.xml)|*.xml" ' Filter files by extension

            ' Show open file dialog box
            Dim result As Boolean = dlg.ShowDialog()

            ' Process open file dialog box results
            If result = True Then
                ' Open document
                Dim filename As String = dlg.FileName

                If MessageBox.Show("Etes vous sur que le serveur puisse accéder au fichier " & filename & " que ce soit en local ou via le réseau, sinon il ne pourra pas l'importer!", "Import Config", MessageBoxButton.OKCancel, MessageBoxImage.Question) = MessageBoxResult.Cancel Then
                    Exit Sub
                End If

                Dim retour As String = myService.ImportConfig(IdSrv, filename)
                If retour <> "0" Then
                    MessageBox.Show(retour, "Erreur import config", MessageBoxButton.OK, MessageBoxImage.Error)
                Else
                    MessageBox.Show("L'import du fichier de configuration a été effectué, l'ancien fichier a été renommé en .old, veuillez redémarrer le serveur pour prendre en compte cette nouvelle configuration", "Import config", MessageBoxButton.OK, MessageBoxImage.Information)
                End If
            End If

            Me.Cursor = Nothing
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub BtnImport_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnExport_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnExport.Click
        Try
            If IsConnect = False Then
                Exit Sub
            End If

            Me.Cursor = Cursors.Wait
           'Exporter le fichier de config

            ' Configure open file dialog box
            Dim dlg As New Microsoft.Win32.SaveFileDialog()
            dlg.FileName = "" ' Default file name
            dlg.DefaultExt = ".xml" ' Default file extension
            dlg.Filter = "Fichier de configuration (.xml)|*.xml" ' Filter files by extension

            ' Show open file dialog box
            Dim result As Boolean = dlg.ShowDialog()

            ' Process open file dialog box results
            If result = True Then
                ' Open document
                Dim filename As String = dlg.FileName
                Dim retour As String = myService.ExportConfig(IdSrv)
                If retour.StartsWith("ERREUR") Then
                    MessageBox.Show(retour, "Erreur export config", MessageBoxButton.OK, MessageBoxImage.Error)
                Else
                    Dim TargetFile As StreamWriter
                    TargetFile = New StreamWriter(filename, False)
                    TargetFile.Write(retour)
                    TargetFile.Close()
                    MessageBox.Show("L'export du fichier de configuration a été effectué", "Export config", MessageBoxButton.OK, MessageBoxImage.Information)
                End If
            End If
       
        Me.Cursor = Nothing
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub BtnExport_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnTestMail_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnTestMail.Click
        Try
            If IsConnect = False Then
                Exit Sub
            End If

            Dim mail As String = InputBox("Veuillez saisir l'adresse mail du destinataire (ex: xy@xxx.com)", "Saisie de l'adresse mail")
            Dim retour As String = myService.TestSendMail(IdSrv, TxtMail.Text, mail, TxtAdresse.Text, TxtSmtpPort.Text, ChkSSL.IsChecked, TxtLogin.Text, TxtPassword.Password)

            If retour = "0" Then
                MessageBox.Show("Le mail de test a été envoyé à l'adresse: " & mail, "Test mail", MessageBoxButton.OK, MessageBoxImage.Information)
            Else
                MessageBox.Show("Erreur lors de l'envoi du mail de test envoyé à l'adresse: " & mail & ": " & retour, "Test mail", MessageBoxButton.OK, MessageBoxImage.Error)
            End If

        Catch ex As Exception
            MessageBox.Show("ERREUR Sub BtnTestMail_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub TxtTarifJour_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtTarifJour.TextChanged
        If IsNumeric(TxtTarifJour.Text) = False Then
            MessageBox.Show("Veuillez saisir une valeur numérique pour le tarif de jour", MessageBoxButton.OK, MessageBoxImage.Error)
            TxtTarifJour.Undo()
        End If
    End Sub

    Private Sub TxtTarifNuit_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtTarifNuit.TextChanged
        If IsNumeric(TxtTarifNuit.Text) = False Then
            MessageBox.Show("Veuillez saisir une valeur numérique pour le tarif de jour", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
            TxtTarifNuit.Undo()
        End If
    End Sub

    Private Sub TxtPortSrvWeb_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtPortSrvWeb.TextChanged
        If IsNumeric(TxtPortSrvWeb.Text) = False Then
            MessageBox.Show("Veuillez saisir une valeur numérique pour le port du serveur web", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
            TxtPortSrvWeb.Undo()
        End If
    End Sub

    Private Sub BtnStartSrvWeb_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnStartSrvWeb.Click
        Try
            myService.RestartServeurWeb()
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub BtnStartSrvWeb_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class
