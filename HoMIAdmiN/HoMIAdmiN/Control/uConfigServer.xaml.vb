Partial Public Class uConfigServer
    Public Event CloseMe(ByVal MyObject As Object)

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        Try
            If Window1.IsConnect = True Then
                If IsNumeric(TxtSOAP.Text) = False Or TxtSOAP.Text = "" Then
                    MessageBox.Show("Le port SOAP est erroné !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
                    Exit Sub
                End If
                If TxtIdSrv.Text = "" Or TxtIdSrv.Text = " " Then
                    MessageBox.Show("L'id du serveur ne peut être vide !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
                    Exit Sub
                End If
                If TxtSOAP.Text <> Window1.myService.GetPortSOAP Then
                    MessageBox.Show("Vous avez modifié le port SOAP, n'oubliez pas:" & vbCrLf & "- D'enregistrer la configuration pour qu'elle soit prise en compte au prochain démarrage" & vbCrLf & "- De redémarrer le service", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                    Window1.myService.SetPortSOAP(IdSrv, TxtSOAP.Text)
                End If
                Window1.myService.SetIdServer(IdSrv, TxtIdSrv.Text)
                IdSrv = TxtIdSrv.Text

                Window1.myService.SetLongitude(IdSrv, CDbl(TxtLong.Text.Replace(".", ",")))
                Window1.myService.SetLatitude(IdSrv, CDbl(TxtLat.Text.Replace(".", ",")))
                Window1.myService.SetHeureCorrectionLever(IdSrv, CInt(HCL.Text))
                Window1.myService.SetHeureCorrectionCoucher(IdSrv, CInt(HCC.Text))
                Window1.myService.SetSMTPMailServeur(IdSrv, TxtMail.Text)
                Window1.myService.SetSMTPServeur(IdSrv, TxtAdresse.Text)
                Window1.myService.SetSMTPLogin(IdSrv, TxtLogin.Text)
                Window1.myService.SetSMTPPassword(IdSrv, TxtPassword.Password)
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
            If Window1.IsConnect = True Then
                TxtSOAP.Text = Window1.myService.GetPortSOAP
                TxtLat.Text = Window1.myService.GetLatitude
                TxtLong.Text = Window1.myService.GetLongitude
                HCL.Text = Window1.myService.GetHeureCorrectionLever
                HCC.Text = Window1.myService.GetHeureCorrectionCoucher
                TxtIdSrv.Text = IdSrv

                TxtAdresse.Text = Window1.myService.GetSMTPServeur(IdSrv)
                TxtMail.Text = Window1.myService.GetSMTPMailServeur(IdSrv)
                TxtLogin.Text = Window1.myService.GetSMTPLogin(IdSrv)
                TxtPassword.Password = Window1.myService.GetSMTPPassword(IdSrv)
            End If

        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uconfigserver New: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

End Class
