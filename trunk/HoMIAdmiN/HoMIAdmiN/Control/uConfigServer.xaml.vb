﻿Imports System.Text.RegularExpressions

Partial Public Class uConfigServer
    Public Event CloseMe(ByVal MyObject As Object)

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        Try
            If IsConnect = True Then
                If TxtIPSOAP.Text = "" Then
                    MessageBox.Show("L'adresse IP SOAP est erronée !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
                    Exit Sub
                End If
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
                If TxtSOAP.Text <> myService.GetPortSOAP Then
                    MessageBox.Show("Vous avez modifié le port SOAP, n'oubliez pas:" & vbCrLf & "- D'enregistrer la configuration pour qu'elle soit prise en compte au prochain démarrage" & vbCrLf & "- De redémarrer le service", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                    myService.SetPortSOAP(IdSrv, TxtSOAP.Text)
                    If TxtSOAP.Text <> myService.GetPortSOAP Then
                        MessageBox.Show("Une erreur est survenue lors du changement du port, veuillez consulter le log", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                    End If
                End If
                If TxtIPSOAP.Text <> myService.GetIPSOAP Then
                    MessageBox.Show("Vous avez modifié l'adresse IP SOAP (donc celle du serveur), n'oubliez pas:" & vbCrLf & "- D'enregistrer la configuration pour qu'elle soit prise en compte au prochain démarrage" & vbCrLf & "- De redémarrer le service" & vbCrLf & "- De modifier l'adresse du serveur au prochain lancement de l'admin", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                    myService.SetPortSOAP(IdSrv, TxtIPSOAP.Text)
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
                myService.SetLongitude(IdSrv, CDbl(TxtLong.Text.Replace(".", ",")))
                myService.SetLatitude(IdSrv, CDbl(TxtLat.Text.Replace(".", ",")))
                myService.SetHeureCorrectionLever(IdSrv, CInt(HCL.Text))
                myService.SetHeureCorrectionCoucher(IdSrv, CInt(HCC.Text))
                myService.SetSMTPMailServeur(IdSrv, TxtMail.Text)
                myService.SetSMTPServeur(IdSrv, TxtAdresse.Text)
                myService.SetSMTPLogin(IdSrv, TxtLogin.Text)
                myService.SetSMTPPassword(IdSrv, TxtPassword.Password)
                myService.SetMaxFileSizeLog(CDbl(TxtFile.Text))
                myService.SetMaxMonthLog(CDbl(TxtMaxLogMonth.Text))
                myService.SetDefautVoice(CbVoice.Text)

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

                For i As Integer = 0 To ListBox1.Items.Count - 1
                    Dim x As CheckBox = ListBox1.Items(i)
                    myService.EnableRepertoireAudio(IdSrv, x.Content, x.IsChecked)
                Next

                For i As Integer = 0 To ListBox2.Items.Count - 1
                    Dim x As CheckBox = ListBox2.Items(i)
                    myService.EnableExtensionAudio(IdSrv, x.Content, x.IsChecked)
                Next

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

                RefreshList()
            End If

            BtnDelRepertoireAudio.IsEnabled = False
            BtnDeleteExtension.IsEnabled = False
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uconfigserver New: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub TxtMaxLogMonth_TextChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtMaxLogMonth.TextChanged
        If IsNumeric(TxtMaxLogMonth.Text) = False Then
            MessageBox.Show("Veuillez saisir un chiffre comme durée de mois maximum", "Admin", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            TxtMaxLogMonth.Text = myService.GetMaxMonthLog
        End If
    End Sub

    Private Sub TxtFile_TextChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtFile.TextChanged
        If IsNumeric(TxtFile.Text) = False Then
            MessageBox.Show("Veuillez saisir un chiffre comme taille maximale", "Admin", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            TxtFile.Text = myService.GetMaxFileSizeLog
        End If
    End Sub

    Private Sub RefreshList()
        Try
            ListBox1.Items.Clear()
            For i As Integer = 0 To myService.GetAllRepertoiresAudio(IdSrv).Count - 1
                Dim a As New CheckBox
                a.Content = myService.GetAllRepertoiresAudio(IdSrv).Item(i).Repertoire
                a.IsChecked = myService.GetAllRepertoiresAudio(IdSrv).Item(i).Enable
                ListBox1.Items.Add(a)
                BtnDelRepertoireAudio.IsEnabled = True
            Next

            ListBox2.Items.Clear()
            For i As Integer = 0 To myService.GetAllExtensionsAudio(IdSrv).Count - 1
                Dim a As New CheckBox
                a.Content = myService.GetAllExtensionsAudio(IdSrv).Item(i).Extension
                a.IsChecked = myService.GetAllExtensionsAudio(IdSrv).Item(i).Enable
                ListBox2.Items.Add(a)
                BtnDeleteExtension.IsEnabled = True
            Next



        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uconfigaudio RefreshList: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnDelRepertoireAudio_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDelRepertoireAudio.Click
        Try
            If ListBox1.SelectedIndex < 0 Then
                MessageBox.Show("Veuillez un répertoire dans la liste!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Exit Sub
            Else
                myService.DeleteRepertoireAudio(IdSrv, ListBox1.SelectedItem.content)
                RefreshList()
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uconfigaudio BtnDelRepertoireAudio_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnNewRepertoire_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnNewRepertoire.Click
        Dim dlg As New Forms.FolderBrowserDialog()

        ' Ouverture de la fenetre de selection d un repertoire
        Dim retour As Forms.DialogResult = dlg.ShowDialog()

        If dlg.SelectedPath = "" Then
            MessageBox.Show("Le chemin du répertoire audio ne peut être vide!", "Repertoire Audio", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        Else
            If myService.NewRepertoireAudio(IdSrv, dlg.SelectedPath, False) = -1 Then
                MessageBox.Show("Le chemin du répertoire audio existe déjà il ne sera pas pris en compte", "Repertoire Audio", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Exit Sub
            Else
                RefreshList()
            End If

        End If
    End Sub

    Private Sub BtnNewExension_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnNewExension.Click
        Dim retour As String
        retour = InputBox("Veuillez saisir la nouvelle extension Audio sans le point, puis l'activer si besoin par la suite", "Extension Audio", "")
        If retour = "" Then
            MessageBox.Show("L'extension audio ne peut être vide!", "Extension Audio", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        End If
        If InStr(retour, ".") > 0 Then
            MessageBox.Show("Veuillez ne pas saisir le . !", "Extension Audio", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        End If
        If retour.Length <> 3 Then
            MessageBox.Show("L'extension doit comporter 3 caractères", "Extension Audio", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        End If
        retour = "." & LCase(retour)
        If myService.NewExtensionAudio(IdSrv, retour, False) = -1 Then
            MessageBox.Show("L'extension audio existe déjà elle ne sera pas pris en compte", "Extension Audio", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        Else
            RefreshList()
        End If

    End Sub

    Private Sub BtnDeleteExtension_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDeleteExtension.Click
        Try
            If ListBox2.SelectedIndex < 0 Then
                MessageBox.Show("Veuillez une extension dans la liste!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Exit Sub
            Else
                myService.DeleteExtensionAudio(IdSrv, ListBox2.SelectedItem.content)
                RefreshList()
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uconfigaudio BtnDeleteExtension_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub ListBox1_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles ListBox1.SelectionChanged
        If ListBox1.SelectedItem IsNot Nothing Then
            BtnDelRepertoireAudio.IsEnabled = True
        Else
            BtnDelRepertoireAudio.IsEnabled = False
        End If
    End Sub

    Private Sub ListBox2_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles ListBox2.SelectionChanged
        If ListBox2.SelectedItem IsNot Nothing Then
            BtnDeleteExtension.IsEnabled = True
        Else
            BtnDeleteExtension.IsEnabled = False
        End If
    End Sub

End Class