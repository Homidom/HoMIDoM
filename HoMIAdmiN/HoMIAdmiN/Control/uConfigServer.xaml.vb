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

                TxtAdresse.Text = myservice.GetSMTPServeur(IdSrv)
                TxtMail.Text = myservice.GetSMTPMailServeur(IdSrv)
                TxtLogin.Text = myservice.GetSMTPLogin(IdSrv)
                TxtPassword.Password = myservice.GetSMTPPassword(IdSrv)
            End If

        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uconfigserver New: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

End Class
