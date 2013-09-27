Imports System.IO

Public Class WConfig
    Dim Frm As Window1
    Dim FlagNewMnu As Integer = -1
    Dim MyListMnu As New List(Of uCtrlImgMnu)

    Private Sub BtnOk_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOk.Click
        Try

            Frm.WithPassword = ChkWidthPass.IsChecked

            If Frm.WithPassword Then
                If String.IsNullOrEmpty(TxtPassword.Text) = True Then
                    MessageBox.Show("Veuillez saisir un mot de passe pour accéder aux fonctions critiques!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                    Exit Sub
                End If
            End If

            Frm.MaJWidgetFromServer = ChkMaJWidgetFromSrv.IsChecked
            Frm.ShowKeyboard = ChkShowKeyboard.IsChecked
            Frm.KeyboardPath = TxtKeyboardPath.Text
            Frm.SaveDiffBackup = ChKDiffSave.IsChecked
            Frm.AutoSave = ChkSaveAuto.IsChecked
            Frm.ShowQuitter = ChkShowBtnQuit.IsChecked
            Frm.Password = TxtPassword.Text
            Frm.AffLastError = ChkAffLastError.IsChecked
            Frm.ShowDateTime = ChkDateTime.IsChecked
            Frm.ShowTimeFromServer = ChkTimeFromServer.IsChecked
            Frm.ShowSoleil = ChkSoleil.IsChecked
            Frm.ShowLabelMnu = ChkShowlblMnu.IsChecked
            Frm.FullScreen = ChkFullScreen.IsChecked
            Frm.Friction = SliderFriction.Value.ToString("0.00")
            Frm.ImageBackGround = TxtImgBack.Text
            Frm.SpeedTouch = SliderSpeed.Value.ToString("0.00")
            Frm.IP = TxtIP.Text
            Frm.PortSOAP = TxtPort.Text
            If IsNumeric(TxtTimeOutSrvLive.Text) Then Frm.TimeOutServerLive = TxtTimeOutSrvLive.Text
            Frm.ListMnu = MyListMnu
            IdSrv = TxtID.Text
            Frm.AsTimeOutPage = ChkTimeOutPage.IsChecked
            Frm.TimeOutPage = CInt(CbTimeOutPage.Text)
            Frm.DefautPage = CbPageDefaut.Text
            Frm.MaskTaskMnu = ChkMaskTaskMnu.IsChecked
            If String.IsNullOrEmpty(CbVille.Text) = False Then Frm.Ville = CbVille.Text
            Frm.TransparenceBas = SliderTranspBas.Value
            Frm.TransparenceHaut = SliderTranspHaut.Value
            DialogResult = True
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur WConfig.BtnOk: " & ex.ToString, "Erreur", "WConfig_New")
        End Try
    End Sub

    Private Sub BtnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnCancel.Click
        DialogResult = False
    End Sub

    Public Sub New(ByVal FrmMere As Window1)

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        Frm = FrmMere
        ChkMaJWidgetFromSrv.IsChecked = Frm.MaJWidgetFromServer
        ChKDiffSave.IsChecked = Frm.SaveDiffBackup
        ChkSaveAuto.IsChecked = Frm.AutoSave
        ChkDateTime.IsChecked = Frm.ShowDateTime
        ChkTimeFromServer.IsChecked = Frm.ShowTimeFromServer
        ChkShowBtnQuit.IsChecked = Frm.ShowQuitter
        ChkWidthPass.IsChecked = Frm.WithPassword
        ChkSoleil.IsChecked = Frm.ShowSoleil
        ChkShowlblMnu.IsChecked = Frm.ShowLabelMnu
        ChkFullScreen.IsChecked = Frm.FullScreen
        SliderFriction.Value = Frm.Friction.ToString("0.00")
        LblFriction.Content = Frm.Friction.ToString("0.00")
        TxtImgBack.Text = Frm.ImageBackGround
        SliderSpeed.Value = Frm.SpeedTouch.ToString("0.00")
        LblSpeed.Content = Frm.SpeedTouch.ToString("0.00")
        TxtIP.Text = Frm.IP
        TxtPort.Text = Frm.PortSOAP
        TxtTimeOutSrvLive.Text = Frm.TimeOutServerLive
        TxtID.Text = IdSrv
        ChkAffLastError.IsChecked = Frm.AffLastError
        ChkTimeOutPage.IsChecked = Frm.AsTimeOutPage
        ChkMaskTaskMnu.IsChecked = Frm.MaskTaskMnu
        SliderTranspBas.Value = Frm.TransparenceBas
        SliderTranspHaut.Value = Frm.TransparenceHaut
        LblvalTransBas.Content = Frm.TransparenceBas
        LblvalTransHaut.Content = Frm.TransparenceHaut
        ChkShowKeyboard.IsChecked = Frm.ShowKeyboard
        TxtKeyboardPath.Text = Frm.KeyboardPath

        If Frm.TimeOutPage >= 0 Then CbTimeOutPage.Text = Frm.TimeOutPage

        ListMnu.Items.Clear()
        MyListMnu.Clear()
        CbPageDefaut.Items.Clear()

        'charger les pages
        CbPageDefaut.Items.Add("Aucune")
        For Each icmnu In Frm.ListMnu
            ListMnu.Items.Add(icmnu.Label)
            CbPageDefaut.Items.Add(icmnu.Label)
        Next
        MyListMnu = Frm.ListMnu

        Try
            CbPageDefaut.SelectedValue = Frm.DefautPage
        Catch ex As Exception
            CbPageDefaut.SelectedIndex = 0
        End Try

        'affiche les programmes
        Try
            If IsConnect Then
                listesversionsprogrammes.Text = ""
                listesversionsprogrammes.Text &= " HoMIWpF : " & My.Application.Info.Version.ToString & vbCrLf
                listesversionsprogrammes.Text &= " HoMIDomService : " & myService.GetServerVersion() & vbCrLf
                listesversionsprogrammes.Text &= " Service démarré : " & myService.GetLastStartTime & vbCrLf
                listesversionsprogrammes.Text &= " Port SOAP utilisé : " & myService.GetPortSOAP & vbCrLf
            End If
            listesversionsprogrammes.Text &= " Version du frameWork: " & System.Runtime.InteropServices.RuntimeEnvironment.GetSystemVersion() & vbCrLf
            If System.Environment.Is64BitOperatingSystem = True Then
                listesversionsprogrammes.Text &= " Version de l'OS: " & My.Computer.Info.OSFullName.ToString & " 64 Bits" & vbCrLf
            Else
                listesversionsprogrammes.Text &= " Version de l'OS: " & My.Computer.Info.OSFullName.ToString & " 32 Bits" & vbCrLf
            End If
            listesversionsprogrammes.Text &= " Répertoire utilisé par le client WPF: " & My.Application.Info.DirectoryPath.ToString & vbCrLf
            listesversionsprogrammes.Text &= " Fichier de configuration chargé : " & Frm.ConfigFile & vbCrLf
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur New WConfig: " & ex.ToString, "Erreur", "WConfig_New")
        End Try
    End Sub

    Private Sub SliderFriction_ValueChanged(ByVal sender As System.Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Double)) Handles SliderFriction.ValueChanged
        If LblFriction IsNot Nothing Then
            LblFriction.Content = SliderFriction.Value
        End If
    End Sub

    Private Sub BtnFileImgBack_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnFileImgBack.Click
        Dim dlg As New Microsoft.Win32.OpenFileDialog()
        dlg.Filter = "jpeg (*.jpg) |*.jpg;*.jpeg|(*.png) |*.png|(*.*) |*.*"

        If dlg.ShowDialog() = True Then
            TxtImgBack.Text = dlg.FileName
        End If
    End Sub

    Private Sub SliderSpeed_ValueChanged(ByVal sender As Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of Double)) Handles SliderSpeed.ValueChanged
        If LblSpeed IsNot Nothing Then
            LblSpeed.Content = SliderSpeed.Value
        End If
    End Sub

    Protected Overrides Sub Finalize()
        Frm = Nothing
        MyBase.Finalize()
    End Sub

    Private Sub ListMnu_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles ListMnu.SelectionChanged
        Try

            If ListMnu.SelectedIndex < 0 Then Exit Sub

            If ListMnu.SelectedIndex >= 0 Then
                BtnUP.IsEnabled = True
                BtnDown.IsEnabled = True
            End If
            If ListMnu.SelectedIndex = 0 Then
                BtnUP.IsEnabled = False
            End If
            If ListMnu.SelectedIndex = ListMnu.Items.Count - 1 Then
                BtnDown.IsEnabled = False
            End If

            If MyListMnu.Item(ListMnu.SelectedIndex).Defaut = True Then
                BtnNewMnu.ContextMenu.Items(1).isenabled = False
                TxtName.Text = MyListMnu.Item(ListMnu.SelectedIndex).Label
                LblType.Content = MyListMnu.Item(ListMnu.SelectedIndex).Type.ToString
                TxtImage.Text = MyListMnu.Item(ListMnu.SelectedIndex).Icon
                StkProperty.Visibility = Windows.Visibility.Visible
                StkImage.Visibility = Windows.Visibility.Visible

                If String.IsNullOrEmpty(TxtImage.Text) = False Then
                    If File.Exists(TxtImage.Text) Then
                        ImgMnu.Source = LoadBitmapImage(TxtImage.Text)
                    End If
                End If

            Else
                BtnNewMnu.ContextMenu.Items(1).isenabled = True
                TxtName.Text = MyListMnu.Item(ListMnu.SelectedIndex).Label
                LblType.Content = MyListMnu.Item(ListMnu.SelectedIndex).Type.ToString
                If Frm.ListMnu.Item(ListMnu.SelectedIndex).Type <> uCtrlImgMnu.TypeOfMnu.Zone Then
                    StkImage.Visibility = Windows.Visibility.Visible
                    Button1.Visibility = Windows.Visibility.Visible
                    TxtName.IsReadOnly = False
                    ChkVisible.Visibility = Windows.Visibility.Hidden
                Else
                    ImgMnu.Source = ConvertArrayToImage(myService.GetByteFromImage(myService.ReturnZoneByID(IdSrv, MyListMnu.Item(ListMnu.SelectedIndex).IDElement).Icon))
                    Button1.Visibility = Windows.Visibility.Hidden
                    StkImage.Visibility = Windows.Visibility.Hidden
                    TxtName.IsReadOnly = True
                    ChkVisible.IsChecked = MyListMnu.Item(ListMnu.SelectedIndex).Visible
                    ChkVisible.Visibility = Windows.Visibility.Visible
                End If
                TxtImage.Text = MyListMnu.Item(ListMnu.SelectedIndex).Icon
                If Frm.ListMnu.Item(ListMnu.SelectedIndex).Type = uCtrlImgMnu.TypeOfMnu.Internet Then
                    LblParam.Content = "Url: "
                    TxtParam.Text = MyListMnu.Item(ListMnu.SelectedIndex).Parametres.Item(0)
                    StkParam.Visibility = Windows.Visibility.Visible
                Else
                    StkParam.Visibility = Windows.Visibility.Hidden
                End If

                StkProperty.Visibility = Windows.Visibility.Visible
            End If

        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur: " & ex.Message, "Erreur", "ListMnu_SelectionChanged")
        End Try
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button1.Click
        Try
            Dim dlg As New Microsoft.Win32.OpenFileDialog()
            dlg.Filter = "jpeg (*.jpg) |*.jpg;*.jpeg|(*.png) |*.png|(*.*) |*.*"

            If dlg.ShowDialog() = True Then
                TxtImage.Text = dlg.FileName
                If String.IsNullOrEmpty(TxtImage.Text) = False Then
                    If File.Exists(TxtImage.Text) Then
                        ImgMnu.Source = LoadBitmapImage(TxtImage.Text)
                    End If
                End If
            End If
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur WConfig.Button1_Click: " & ex.ToString, "Erreur", "WConfig.Button1_Click")
        End Try
    End Sub

    Private Sub BtnOkMnu_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOkMnu.Click
        Try
            If String.IsNullOrEmpty(TxtName.Text) = True Then
                MessageBox.Show("Veuillez saisir le nom du menu!", "Erreur")
                Exit Sub
            End If
            Select Case FlagNewMnu
                Case -1
                    If ListMnu.SelectedIndex < 0 Then Exit Sub
                    MyListMnu.Item(ListMnu.SelectedIndex).Label = TxtName.Text
                    If MyListMnu.Item(ListMnu.SelectedIndex).Type <> uCtrlImgMnu.TypeOfMnu.Zone Then
                        MyListMnu.Item(ListMnu.SelectedIndex).Icon = TxtImage.Text
                    End If
                    If MyListMnu.Item(ListMnu.SelectedIndex).Type = uCtrlImgMnu.TypeOfMnu.Internet Then
                        MyListMnu.Item(ListMnu.SelectedIndex).Parametres.Item(0) = TxtParam.Text
                    End If
                    If MyListMnu.Item(ListMnu.SelectedIndex).Type = uCtrlImgMnu.TypeOfMnu.Zone Then
                        MyListMnu.Item(ListMnu.SelectedIndex).Visible = ChkVisible.IsChecked
                    End If
                Case 1 'New Internet
                    If String.IsNullOrEmpty(TxtParam.Text) = True Then
                        MessageBox.Show("Veuillez saisir l'url internet!", "Erreur")
                        Exit Sub
                    End If
                    Dim x As New uCtrlImgMnu
                    x.Label = TxtName.Text
                    x.Type = uCtrlImgMnu.TypeOfMnu.Internet
                    x.Icon = TxtImage.Text
                    x.Parametres.Add(TxtParam.Text)
                    x.Visible = ChkVisible.IsChecked
                    MyListMnu.Add(x)
                    ListMnu.Items.Add(TxtName.Text)
                Case 2 'New Meteo
                    Dim x As New uCtrlImgMnu
                    x.Label = TxtName.Text
                    x.Type = uCtrlImgMnu.TypeOfMnu.Meteo
                    x.Icon = TxtImage.Text
                    MyListMnu.Add(x)
                    ListMnu.Items.Add(TxtName.Text)
                Case 4 'New Media
                    Dim x As New uCtrlImgMnu
                    x.Label = TxtName.Text
                    x.Type = uCtrlImgMnu.TypeOfMnu.LecteurMedia
                    x.Icon = TxtImage.Text
                    MyListMnu.Add(x)
                    ListMnu.Items.Add(TxtName.Text)
            End Select
            StkProperty.Visibility = Windows.Visibility.Hidden
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur WConfig.BtnOkMnu_Click: " & ex.ToString, "Erreur", "WConfig.BtnOkMnu_Click")
        End Try
    End Sub

    Private Sub BtnCancelMnu_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnCancelMnu.Click
        StkProperty.Visibility = Windows.Visibility.Hidden
    End Sub

    Private Sub MnuAddInternet_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MnuAddInternet.Click
        TxtName.Text = ""
        LblType.Content = uCtrlImgMnu.TypeOfMnu.Internet.ToString
        TxtImage.Text = ""
        LblParam.Content = "Url: "
        TxtParam.Text = ""
        TxtName.IsReadOnly = False
        ChkVisible.Visibility = Windows.Visibility.Hidden
        StkImage.Visibility = Windows.Visibility.Visible
        StkParam.Visibility = Windows.Visibility.Visible
        StkProperty.Visibility = Windows.Visibility.Visible
        FlagNewMnu = 1
    End Sub

    Private Sub MnuAddMeteo_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MnuAddMeteo.Click
        For i As Integer = 0 To MyListMnu.Count - 1
            If MyListMnu.Item(i).Type = uCtrlImgMnu.TypeOfMnu.Meteo Then
                MessageBox.Show("Vous ne pouvez plus ajouter de menu Meteo car il en existe déjà un ! ")
                Exit Sub
            End If
        Next

        TxtName.Text = ""
        LblType.Content = uCtrlImgMnu.TypeOfMnu.Meteo.ToString
        TxtImage.Text = ""
        TxtName.IsReadOnly = False
        ChkVisible.Visibility = Windows.Visibility.Hidden
        StkImage.Visibility = Windows.Visibility.Visible
        StkParam.Visibility = Windows.Visibility.Hidden
        StkProperty.Visibility = Windows.Visibility.Visible
        FlagNewMnu = 2
    End Sub

    Private Sub MnuAddMedia_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MnuAddMedia.Click
        For i As Integer = 0 To MyListMnu.Count - 1
            If MyListMnu.Item(i).Type = uCtrlImgMnu.TypeOfMnu.LecteurMedia Then
                MessageBox.Show("Vous ne pouvez plus ajouter de menu Media car il en existe déjà un ! ")
                Exit Sub
            End If
        Next

        TxtName.Text = ""
        LblType.Content = uCtrlImgMnu.TypeOfMnu.LecteurMedia.ToString
        TxtImage.Text = ""
        TxtName.IsReadOnly = False
        ChkVisible.Visibility = Windows.Visibility.Hidden
        StkImage.Visibility = Windows.Visibility.Visible
        StkParam.Visibility = Windows.Visibility.Hidden
        StkProperty.Visibility = Windows.Visibility.Visible
        FlagNewMnu = 4
    End Sub

    Private Sub BtnUP_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnUP.Click
        If ListMnu.SelectedIndex < 1 Then Exit Sub
        Dim tmp1 As uCtrlImgMnu
        Dim tmp2 As uCtrlImgMnu

        tmp1 = MyListMnu.Item(ListMnu.SelectedIndex)
        tmp2 = MyListMnu.Item(ListMnu.SelectedIndex - 1)

        MyListMnu.Item(ListMnu.SelectedIndex) = tmp2
        MyListMnu.Item(ListMnu.SelectedIndex - 1) = tmp1

        ListMnu.Items.Clear()
        For i As Integer = 0 To MyListMnu.Count - 1
            ListMnu.Items.Add(MyListMnu.Item(i).Label)
        Next

        If ListMnu.SelectedIndex - 1 = 0 Then BtnUP.IsEnabled = False
        StkProperty.Visibility = Windows.Visibility.Hidden
    End Sub

    Private Sub BtnDown_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDown.Click
        If ListMnu.SelectedIndex < 0 Then Exit Sub
        If ListMnu.SelectedIndex + 1 = ListMnu.Items.Count Then Exit Sub
        Dim tmp1 As uCtrlImgMnu
        Dim tmp2 As uCtrlImgMnu

        tmp1 = MyListMnu.Item(ListMnu.SelectedIndex)
        tmp2 = MyListMnu.Item(ListMnu.SelectedIndex + 1)

        MyListMnu.Item(ListMnu.SelectedIndex) = tmp2
        MyListMnu.Item(ListMnu.SelectedIndex + 1) = tmp1

        ListMnu.Items.Clear()
        For i As Integer = 0 To MyListMnu.Count - 1
            ListMnu.Items.Add(MyListMnu.Item(i).Label)
        Next

        If ListMnu.SelectedIndex + 1 = ListMnu.Items.Count Then BtnDown.IsEnabled = False
        StkProperty.Visibility = Windows.Visibility.Hidden
    End Sub

    Private Sub ChkWidthPass_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ChkWidthPass.Checked, ChkWidthPass.Click
        If ChkWidthPass.IsChecked Then
            TxtPassword.Visibility = Windows.Visibility.Visible
            TxtPassword.Text = Frm.Password
        Else
            TxtPassword.Text = ""
            TxtPassword.Visibility = Windows.Visibility.Collapsed
        End If
    End Sub

    Private Sub ChkTimeOutPage_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ChkTimeOutPage.Click
        If ChkTimeOutPage.IsChecked Then
            CbPageDefaut.IsEnabled = True
            CbTimeOutPage.IsEnabled = True
        Else
            CbPageDefaut.IsEnabled = False
            CbTimeOutPage.IsEnabled = False
        End If
    End Sub

    ''' <summary>
    ''' Supprimer un menu
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BtnDel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDel.Click
        Try
            If ListMnu.SelectedIndex < 0 Then Exit Sub
            If MyListMnu.Item(ListMnu.SelectedIndex).Type = uCtrlImgMnu.TypeOfMnu.Zone Then
                MessageBox.Show("Une zone ne peut être supprimée, veuillez sélectionner sa propriété visible pour la cacher dans les menus")
                Exit Sub
            End If
            MyListMnu.RemoveAt(ListMnu.SelectedIndex)
            ListMnu.Items.RemoveAt(ListMnu.SelectedIndex)
            StkProperty.Visibility = Windows.Visibility.Hidden
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur WConfig.MenuItem1_Click: " & ex.ToString, "Erreur", "WConfig.MenuItem1_Click")
        End Try
    End Sub

    ''' <summary>
    ''' Afficher le menu Ajouter Menu
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BtnNewMnu_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnNewMnu.Click
        CtxMnuBtn.PlacementTarget = sender
        CtxMnuBtn.IsOpen = True
    End Sub

    Private Sub CbVille_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CbVille.Loaded
        'charger les villes
        CbVille.Items.Clear()

        If IsConnect Then
            For Each ObjMeteo As HoMIDom.HoMIDom.TemplateDevice In AllDevices
                If ObjMeteo.Type = HoMIDom.HoMIDom.Device.ListeDevices.METEO And ObjMeteo.Enable = True Then
                    CbVille.Items.Add(ObjMeteo.Name)
                End If
            Next
            If CbVille.Items.Count > 0 Then CbVille.SelectedValue = Frm.Ville
        End If
    End Sub

    Private Sub BtnDeleteCache_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDeleteCache.Click
        Dim retour As String = Cache.DeleteAllFileOffPath(_MonRepertoire & "\cache\images\")
        If retour IsNot Nothing Then
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur DeleteAllFileOffPath: " & retour, "Erreur", "BtnDeleteCache_Click")
        End If
    End Sub

    Private Sub WConfig_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        Frm = Nothing
    End Sub

    Private Sub SliderTranspHaut_ValueChanged(sender As System.Object, e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Double)) Handles SliderTranspHaut.ValueChanged
        LblvalTransHaut.Content = Format(sender.Value, "0")
    End Sub

    Private Sub SliderTranspBas_ValueChanged(sender As System.Object, e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Double)) Handles SliderTranspBas.ValueChanged
        LblvalTransBas.Content = Format(sender.Value, "0")
    End Sub

    Private Sub ChkDateTime_Checked(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles ChkDateTime.Checked, ChkDateTime.Unchecked
        If ChkDateTime.IsChecked Then
            ChkTimeFromServer.Visibility = Windows.Visibility.Visible
        Else
            ChkTimeFromServer.IsChecked = False
            ChkTimeFromServer.Visibility = Windows.Visibility.Collapsed
        End If
    End Sub
End Class
