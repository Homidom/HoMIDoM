Partial Public Class uConfigServer
    Public Event CloseMe(ByVal MyObject As Object)

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        Try
            If IsConnect = True Then
                If IsNumeric(TxtSOAP.Text) = False Or TxtSOAP.Text = "" Then
                    MessageBox.Show("Le port SOAP est erroné !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
                    Exit Sub
                End If
                If TxtIdSrv.Text = "" Or TxtIdSrv.Text = " " Then
                    MessageBox.Show("L'id du serveur ne peut être vide !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
                    Exit Sub
                End If
                If IsNumeric(TxtSave.Text) = False Or TxtSave.Text = "" Or TxtSave.Text = " " Or CInt(TxtSave.Text) < 0 Then
                    MessageBox.Show("La valeur de saubegarde doit être un chiffre et positif !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
                    Exit Sub
                End If
                If TxtSOAP.Text <> myservice.GetPortSOAP Then
                    MessageBox.Show("Vous avez modifié le port SOAP, n'oubliez pas:" & vbCrLf & "- D'enregistrer la configuration pour qu'elle soit prise en compte au prochain démarrage" & vbCrLf & "- De redémarrer le service", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                    myservice.SetPortSOAP(IdSrv, TxtSOAP.Text)
                End If
                If IsNumeric(TxtFile.Text) = False Or CDbl(TxtFile.Text) < 1 Then
                    MessageBox.Show("Veuillez saisir un numérique et positif pour la taille max du fichier de log!", "Admin", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                    Exit Sub
                End If

                myservice.SetIdServer(IdSrv, TxtIdSrv.Text)
                IdSrv = TxtIdSrv.Text

                Dim tmpSave As Integer = TxtSave.Text

                tmpSave = Format(tmpSave, "#0")
                Dim retour As String = myservice.SetTimeSave(IdSrv, tmpSave)
                If retour <> "0" Then MessageBox.Show(retour, "Erreur SetTimeSave")
                myservice.SetLongitude(IdSrv, CDbl(TxtLong.Text.Replace(".", ",")))
                myservice.SetLatitude(IdSrv, CDbl(TxtLat.Text.Replace(".", ",")))
                myservice.SetHeureCorrectionLever(IdSrv, CInt(HCL.Text))
                myservice.SetHeureCorrectionCoucher(IdSrv, CInt(HCC.Text))
                myservice.SetSMTPMailServeur(IdSrv, TxtMail.Text)
                myservice.SetSMTPServeur(IdSrv, TxtAdresse.Text)
                myservice.SetSMTPLogin(IdSrv, TxtLogin.Text)
                myservice.SetSMTPPassword(IdSrv, TxtPassword.Password)
                myService.SetMaxFileSizeLog(CDbl(TxtFile.Text))
                myService.SetMaxMonthLog(CDbl(TxtMaxLogMonth.Text))

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

                FlagChange = True
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
                TxtSOAP.Text = myservice.GetPortSOAP
                TxtLat.Text = myservice.GetLatitude
                TxtLong.Text = myservice.GetLongitude
                HCL.Text = myservice.GetHeureCorrectionLever
                HCC.Text = myservice.GetHeureCorrectionCoucher
                TxtIdSrv.Text = IdSrv
                TxtSave.Text = myservice.GetTimeSave(IdSrv)
                TxtFile.Text = myService.GetMaxFileSizeLog
                TxtMaxLogMonth.Text = myService.GetMaxMonthLog

                TxtAdresse.Text = myservice.GetSMTPServeur(IdSrv)
                TxtMail.Text = myservice.GetSMTPMailServeur(IdSrv)
                TxtLogin.Text = myservice.GetSMTPLogin(IdSrv)
                TxtPassword.Password = myService.GetSMTPPassword(IdSrv)

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
            End If

        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uconfigserver New: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub TxtMaxLogMonth_TextChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtMaxLogMonth.TextChanged
        If IsNumeric(TxtMaxLogMonth.Text) = False Then
            MessageBox.Show("Veuillez saisir un chiffre comme durée de mois maximum", "Admin", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            TxtMaxLogMonth.Text = myservice.GetMaxMonthLog
        End If
    End Sub

    Private Sub TxtFile_TextChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtFile.TextChanged
        If IsNumeric(TxtFile.Text) = False Then
            MessageBox.Show("Veuillez saisir un chiffre comme taille maximale", "Admin", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            TxtFile.Text = myservice.GetMaxFileSizeLog
        End If
    End Sub
End Class
