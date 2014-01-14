Imports System.Windows.Forms
Imports System.Drawing
Imports HoMIDom.HoMIDom
Imports System.Net

Public Class WWidgetProperty
    Dim Obj As uWidgetEmpty
    Dim _FlagNewAction As Boolean = False
    Dim _FlagNewVisu As Boolean = False
    Dim _listhttpbtn As New List(Of uHttp.ButtonHttp)

    Public Property Objet As uWidgetEmpty
        Get
            Return Obj
        End Get
        Set(ByVal value As uWidgetEmpty)
            Obj = value

            Try
                ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
                ChkIsCommun.IsChecked = Obj.IsCommun
                ChkMajEtiq.IsChecked = Obj.MaJEtiquetteFromServeur
                ChkShowStatus.IsChecked = Obj.ShowStatus
                ChkShowEtiq.IsChecked = Obj.ShowEtiquette
                ChkEditValue.IsChecked = Obj.CanEditValue
                ChkPicture.IsChecked = Obj.ShowPicture
                ChkSaveProportionPicture.IsChecked = Obj.GarderProportionImage
                TxtEtiq.Text = Obj.Etiquette
                TxtSizeEtiq.Text = Obj.TailleEtiquette
                TxtX.Text = Obj.X
                TxtY.Text = Obj.Y
                TxtWidth.Text = Obj.Width
                TxtHeight.Text = Obj.Height
                TxtRotation.Text = Obj.Rotation
                TxtRotationX.Text = Obj.RotationX
                TxtRotationY.Text = Obj.RotationY
                TxtZOrder.Text = Obj.ZIndex
                TxtDefStatus.Text = Obj.DefautLabelStatus
                TxtTailleStatus.Text = Obj.TailleStatus
                lblColor.Background = Obj.ColorBackGround
                lblColorStatus.Background = Obj.ColorStatus
                lblColorEtiquette.Background = Obj.ColorEtiquette
                lblColorBorder.Background = Obj.ColorBorder
                CbEtiqAlign.SelectedIndex = Obj.EtiquetteAlignement
                TxtBorderAngle.Text = Obj.CornerRadius
                TxtBorderSize.Text = Obj.BorderThickness
                ColorPicker1.SelectedColor = Obj.ColorBackGround
                ColorPicker2.SelectedColor = Obj.ColorStatus
                ColorPicker3.SelectedColor = Obj.ColorEtiquette
                ColorPicker4.SelectedColor = Obj.ColorBorder
                Slider1.Value = CDec("&H" & Obj.ColorBackGround.ToString.Substring(1, 2))
                TxtURL.Text = Obj.URL
                TxtURLRss.Text = Obj.UrlRss
                TxtRefresh.Text = Obj.HttpRefresh
                TxtValueMin.Text = Obj.Min
                TxtValueMax.Text = Obj.Max

                ChKShowPassWord.IsChecked = Obj.ShowPassWord
                ChKShowClavier.IsChecked = Obj.ShowClavier
                ChKClearAfterEnter.IsChecked = Obj.ClearAfterEnter
                ChkFondu.IsChecked = Obj.Fondu

                _listhttpbtn = Obj.ListHttpButton
                If IO.File.Exists(Obj.Picture) Then
                    ImgPicture.Source = New BitmapImage(New Uri(Obj.Picture))
                Else
                    ImgPicture.Source = ConvertArrayToImage(myService.GetByteFromImage(Obj.Picture))
                End If
                ImgPicture.Tag = Obj.Picture

                If Obj.IsEmpty = False Then
                    BtnEditAction.Visibility = Windows.Visibility.Collapsed
                    BtnEditVisu.Visibility = Windows.Visibility.Collapsed
                    BtnDelete.Visibility = Windows.Visibility.Collapsed
                    ImgPicture.IsEnabled = False
                    BtnInitPict.Visibility = Windows.Visibility.Visible
                    'TxtEtiq.IsReadOnly = True
                Else
                    Select Case Obj.Type
                        Case uWidgetEmpty.TypeOfWidget.Empty
                            BtnEditAction.Visibility = Windows.Visibility.Visible
                            BtnEditVisu.Visibility = Windows.Visibility.Visible
                            BtnDelete.Visibility = Windows.Visibility.Visible

                            Refresh_LstObjetVisu()

                            Charge_Device_InCombo()

                            Charge_Macro_InCombo()

                            Charge_Zone_InCombo()

                            Charge_Cmd_InCombo()

                        Case uWidgetEmpty.TypeOfWidget.Image
                            StkPicture.Visibility = Visibility.Visible
                            StkStatus.Visibility = Visibility.Collapsed
                            StkEtiq.Visibility = Windows.Visibility.Collapsed
                            ChkMajEtiq.Visibility = Windows.Visibility.Hidden
                            ChkShowStatus.Visibility = Visibility.Collapsed
                            ChkShowEtiq.Visibility = Visibility.Collapsed
                            ChkEditValue.Visibility = Windows.Visibility.Collapsed
                            BtnEditAction.Visibility = Windows.Visibility.Visible
                            BtnEditVisu.Visibility = Windows.Visibility.Visible
                            BtnDelete.Visibility = Windows.Visibility.Visible

                            Refresh_LstObjetVisu()

                            Charge_Device_InCombo()

                            Charge_Macro_InCombo()

                            Charge_Zone_InCombo()

                            Charge_Cmd_InCombo()

                        Case uWidgetEmpty.TypeOfWidget.Web
                            StkPicture.Visibility = Visibility.Collapsed
                            StkStatus.Visibility = Visibility.Collapsed
                            BtnEditAction.Visibility = Windows.Visibility.Collapsed
                            BtnEditVisu.Visibility = Windows.Visibility.Collapsed
                            BtnDelete.Visibility = Windows.Visibility.Visible
                            GrpPropertyHttp.Visibility = Windows.Visibility.Visible
                            Refresh_LstHttpBtn()
                        Case uWidgetEmpty.TypeOfWidget.Camera
                            StkPicture.Visibility = Visibility.Collapsed
                            StkStatus.Visibility = Visibility.Collapsed
                            BtnEditAction.Visibility = Windows.Visibility.Collapsed
                            BtnEditVisu.Visibility = Windows.Visibility.Collapsed
                            BtnDelete.Visibility = Windows.Visibility.Visible
                            GrpPropertyHttp.Visibility = Windows.Visibility.Visible
                            Refresh_LstHttpBtn()
                        Case uWidgetEmpty.TypeOfWidget.Rss
                            StkPicture.Visibility = Visibility.Collapsed
                            StkStatus.Visibility = Visibility.Collapsed
                            StkRss.Visibility = Windows.Visibility.Visible
                            BtnEditAction.Visibility = Windows.Visibility.Collapsed
                            BtnEditVisu.Visibility = Windows.Visibility.Collapsed
                            BtnDelete.Visibility = Windows.Visibility.Visible
                        Case uWidgetEmpty.TypeOfWidget.Meteo
                            StkPicture.Visibility = Visibility.Collapsed
                            StkStatus.Visibility = Visibility.Collapsed
                            StkRss.Visibility = Windows.Visibility.Collapsed
                            StkMeteo.Visibility = Windows.Visibility.Visible
                            BtnEditAction.Visibility = Windows.Visibility.Collapsed
                            BtnEditVisu.Visibility = Windows.Visibility.Collapsed
                            BtnDelete.Visibility = Windows.Visibility.Visible

                            If IsConnect Then
                                CbVilleMeteo.Items.Clear()
                                Dim idx As Integer = -1
                                For Each _devmeteo As TemplateDevice In AllDevices
                                    If _devmeteo.Type = Device.ListeDevices.METEO Then
                                        Dim x As New ComboBoxItem
                                        x.Content = _devmeteo.Name
                                        x.Tag = _devmeteo.ID
                                        CbVilleMeteo.Items.Add(x)

                                        If _devmeteo.ID = Obj.IDMeteo Then idx = CbVilleMeteo.Items.Count - 1
                                    End If
                                Next

                                CbVilleMeteo.SelectedIndex = idx
                            End If
                        Case uWidgetEmpty.TypeOfWidget.KeyPad
                            StkPicture.Visibility = Visibility.Collapsed
                            StkStatus.Visibility = Visibility.Collapsed
                            StkRss.Visibility = Windows.Visibility.Collapsed
                            StkMeteo.Visibility = Windows.Visibility.Collapsed
                            StkKeyPad.Visibility = Windows.Visibility.Visible
                            BtnEditAction.Visibility = Windows.Visibility.Collapsed
                            BtnEditVisu.Visibility = Windows.Visibility.Collapsed
                            BtnDelete.Visibility = Windows.Visibility.Visible
                            GrpPropertyKeyPad.Visibility = Windows.Visibility.Visible

                            If IsConnect Then
                                CbDeviceKeyPad.Items.Clear()
                                Dim idx As Integer = -1
                                For Each _devk As TemplateDevice In AllDevices
                                    If _devk.Type = Device.ListeDevices.GENERIQUEVALUE Then
                                        Dim x As New ComboBoxItem
                                        x.Content = _devk.Name
                                        x.Tag = _devk.ID
                                        CbDeviceKeyPad.Items.Add(x)

                                        If _devk.ID = Obj.IDKeyPad Then idx = CbDeviceKeyPad.Items.Count - 1
                                    End If
                                Next

                                CbDeviceKeyPad.SelectedIndex = idx

                            End If
                        Case uWidgetEmpty.TypeOfWidget.Volet
                            StkPicture.Visibility = Visibility.Collapsed
                            StkStatus.Visibility = Visibility.Collapsed
                            StkRss.Visibility = Windows.Visibility.Collapsed
                            StkMeteo.Visibility = Windows.Visibility.Collapsed
                            StkKeyPad.Visibility = Windows.Visibility.Visible
                            BtnEditAction.Visibility = Windows.Visibility.Collapsed
                            BtnEditVisu.Visibility = Windows.Visibility.Collapsed
                            BtnDelete.Visibility = Windows.Visibility.Visible

                            If IsConnect Then
                                CbDeviceKeyPad.Items.Clear()
                                Dim idx As Integer = -1
                                For Each _devk As TemplateDevice In AllDevices
                                    If _devk.Type = Device.ListeDevices.GENERIQUEVALUE Or _devk.Type = Device.ListeDevices.VOLET Then
                                        Dim x As New ComboBoxItem
                                        x.Content = _devk.Name
                                        x.Tag = _devk.ID
                                        CbDeviceKeyPad.Items.Add(x)

                                        If _devk.ID = Obj.Id Then idx = CbDeviceKeyPad.Items.Count - 1
                                    End If
                                Next

                                CbDeviceKeyPad.SelectedIndex = idx

                            End If
                        Case uWidgetEmpty.TypeOfWidget.Moteur
                            StkPicture.Visibility = Visibility.Visible
                            StkStatus.Visibility = Visibility.Visible
                            StkRss.Visibility = Windows.Visibility.Collapsed
                            StkMeteo.Visibility = Windows.Visibility.Collapsed
                            StkKeyPad.Visibility = Windows.Visibility.Visible
                            BtnEditAction.Visibility = Windows.Visibility.Collapsed
                            BtnEditVisu.Visibility = Windows.Visibility.Collapsed
                            BtnDelete.Visibility = Windows.Visibility.Visible
                            StkMinMax.Visibility = Windows.Visibility.Visible

                            If IsConnect Then
                                CbDeviceKeyPad.Items.Clear()
                                Dim idx As Integer = -1
                                For Each _devk As TemplateDevice In AllDevices
                                    If _devk.Type = Device.ListeDevices.GENERIQUEVALUE Or _devk.Type = Device.ListeDevices.APPAREIL Or _devk.Type = Device.ListeDevices.GENERIQUEBOOLEEN Or _devk.Type = Device.ListeDevices.SWITCH Then
                                        Dim x As New ComboBoxItem
                                        x.Content = _devk.Name
                                        x.Tag = _devk.ID
                                        CbDeviceKeyPad.Items.Add(x)

                                        If _devk.ID = Obj.Id Then idx = CbDeviceKeyPad.Items.Count - 1
                                    End If
                                Next

                                CbDeviceKeyPad.SelectedIndex = idx

                            End If
                        Case uWidgetEmpty.TypeOfWidget.Prise
                            StkPicture.Visibility = Visibility.Visible
                            StkStatus.Visibility = Visibility.Visible
                            StkRss.Visibility = Windows.Visibility.Collapsed
                            StkMeteo.Visibility = Windows.Visibility.Collapsed
                            StkKeyPad.Visibility = Windows.Visibility.Visible
                            BtnEditAction.Visibility = Windows.Visibility.Collapsed
                            BtnEditVisu.Visibility = Windows.Visibility.Collapsed
                            BtnDelete.Visibility = Windows.Visibility.Visible
                            StkMinMax.Visibility = Windows.Visibility.Collapsed

                            If IsConnect Then
                                CbDeviceKeyPad.Items.Clear()
                                Dim idx As Integer = -1
                                For Each _devk As TemplateDevice In AllDevices
                                    If _devk.Type = Device.ListeDevices.GENERIQUEVALUE Or _devk.Type = Device.ListeDevices.APPAREIL Or _devk.Type = Device.ListeDevices.GENERIQUEBOOLEEN Or _devk.Type = Device.ListeDevices.SWITCH Then
                                        Dim x As New ComboBoxItem
                                        x.Content = _devk.Name
                                        x.Tag = _devk.ID
                                        CbDeviceKeyPad.Items.Add(x)

                                        If _devk.ID = Obj.Id Then idx = CbDeviceKeyPad.Items.Count - 1
                                    End If
                                Next

                                CbDeviceKeyPad.SelectedIndex = idx

                            End If
                        Case uWidgetEmpty.TypeOfWidget.Label
                            StkPicture.Visibility = Visibility.Collapsed
                            StkStatus.Visibility = Visibility.Collapsed
                            ChkMajEtiq.Visibility = Visibility.Collapsed
                            ChkShowStatus.Visibility = Visibility.Collapsed
                            ChkShowEtiq.Visibility = Visibility.Collapsed
                            BtnEditAction.Visibility = Windows.Visibility.Collapsed
                            ChkEditValue.Visibility = Windows.Visibility.Collapsed
                            BtnEditVisu.Visibility = Windows.Visibility.Collapsed
                            BtnDelete.Visibility = Windows.Visibility.Visible

                    End Select

                End If
            Catch ex As Exception
                AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur WWidgetProperty.Set Objet: " & ex.Message, "Erreur", " WWidgetProperty.Set Objet")
            End Try
        End Set
    End Property

#Region "Chargement des combos"
    Private Sub Charge_Device_InCombo()
        For Each obj As TemplateDevice In myService.GetAllDevices(IdSrv)
            Dim lbl1 As New ComboBoxItem
            Dim lbl2 As New ComboBoxItem
            lbl1.Content = obj.Name & " [Device]"
            lbl1.Tag = "DEVICE"
            lbl1.Uid = obj.ID
            lbl2.Content = obj.Name & " [Device]"
            lbl2.Tag = "DEVICE"
            lbl2.Uid = obj.ID
            CbObjet.Items.Add(lbl1)
            CbObjetVisu.Items.Add(lbl2)
            lbl1 = Nothing
            lbl2 = Nothing
        Next
    End Sub

    Private Sub Charge_Macro_InCombo()
        For Each obj As Macro In myService.GetAllMacros(IdSrv)
            Dim lbl1 As New ComboBoxItem
            Dim lbl2 As New ComboBoxItem
            lbl1.Content = obj.Nom & " [Macro]"
            lbl1.Tag = "MACRO"
            lbl1.Uid = obj.ID
            lbl2.Content = obj.Nom & " [Macro]"
            lbl2.Tag = "MACRO"
            lbl2.Uid = obj.ID
            CbObjet.Items.Add(lbl1)
            CbObjetVisu.Items.Add(lbl2)
            lbl1 = Nothing
            lbl2 = Nothing
        Next
    End Sub

    Private Sub Charge_Zone_InCombo()
        For Each obj As Zone In myService.GetAllZones(IdSrv)
            Dim lbl1 As New ComboBoxItem
            Dim lbl2 As New ComboBoxItem
            lbl1.Content = obj.Name & " [Zone]"
            lbl1.Tag = "ZONE"
            lbl1.Uid = obj.ID
            lbl2.Content = obj.Name & " [Zone]"
            lbl2.Tag = "ZONE"
            lbl2.Uid = obj.ID
            CbObjet.Items.Add(lbl1)
            CbObjetVisu.Items.Add(lbl2)
            lbl1 = Nothing
            lbl2 = Nothing
        Next
    End Sub

    Private Sub Charge_Cmd_InCombo()
        ' Commandes
        Dim lb1 As New ComboBoxItem
        Dim lb2 As New ComboBoxItem
        lb1.Content = "Commandes"
        lb1.Tag = "COMMAND"
        lb1.Uid = Nothing
        lb2.Content = "Commandes"
        lb2.Tag = "COMMAND"
        lb2.Uid = Nothing
        CbObjet.Items.Add(lb1)
        CbObjetVisu.Items.Add(lb2)
        lb1 = Nothing
        lb2 = Nothing
    End Sub
#End Region

#Region "Button OK Annuler"

    Private Sub BtnOk_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOk.Click
        Try
            Obj.ShowStatus = ChkShowStatus.IsChecked
            Obj.ShowEtiquette = ChkShowEtiq.IsChecked
            Obj.ShowPicture = ChkPicture.IsChecked
            Obj.IsCommun = ChkIsCommun.IsChecked
            Obj.CanEditValue = ChkEditValue.IsChecked
            Obj.MaJEtiquetteFromServeur = ChkMajEtiq.IsChecked
            Obj.Etiquette = TxtEtiq.Text
            Obj.EtiquetteAlignement = CbEtiqAlign.SelectedIndex
            Obj.X = TxtX.Text
            Obj.Y = TxtY.Text
            Obj.Width = TxtWidth.Text
            Obj.Height = TxtHeight.Text
            Obj.Rotation = TxtRotation.Text
            Obj.RotationX = TxtRotationX.Text
            Obj.RotationY = TxtRotationY.Text
            Obj.ZIndex = TxtZOrder.Text
            Obj.DefautLabelStatus = TxtDefStatus.Text
            Obj.Picture = ImgPicture.Tag
            Obj.GarderProportionImage = ChkSaveProportionPicture.IsChecked
            Obj.ListHttpButton = _listhttpbtn
            Obj.HttpRefresh = TxtRefresh.Text
            Obj.ShowPassWord = ChKShowPassWord.IsChecked
            Obj.ShowClavier = ChKShowClavier.IsChecked
            Obj.ClearAfterEnter = ChKClearAfterEnter.IsChecked
            Obj.Fondu = ChkFondu.IsChecked
            Obj.Min = TxtValueMin.Text
            Obj.Max = TxtValueMax.Text

            Try
                Obj.TailleStatus = TxtTailleStatus.Text
            Catch ex As Exception
                AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur: " & ex.Message, "Erreur", "Erreur: " & ex.Message)
                Exit Sub
            End Try

            Try
                Obj.TailleEtiquette = CDbl(TxtSizeEtiq.Text)
            Catch ex As Exception
                AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur: " & ex.Message, "Erreur", "Erreur: " & ex.Message)
                Exit Sub
            End Try

            Obj.BorderThickness = TxtBorderSize.Text
            Obj.CornerRadius = TxtBorderAngle.Text
            Obj.ColorBorder = ColorPicker4.SelectedColor
            Obj.ColorBackGround = lblColor.Background
            Obj.ColorStatus = ColorPicker2.SelectedColor
            Obj.ColorEtiquette = ColorPicker3.SelectedColor
            Obj.URL = TxtURL.Text
            Obj.UrlRss = TxtURLRss.Text

            If CbVilleMeteo.SelectedIndex >= 0 Then
                Dim x As ComboBoxItem = CbVilleMeteo.Items(CbVilleMeteo.SelectedIndex)
                Obj.IDMeteo = x.Tag
            End If
            If CbDeviceKeyPad.SelectedIndex >= 0 And Obj.Type = uWidgetEmpty.TypeOfWidget.KeyPad Then
                Dim x As ComboBoxItem = CbDeviceKeyPad.Items(CbDeviceKeyPad.SelectedIndex)
                Obj.IDKeyPad = x.Tag
            End If
            If CbDeviceKeyPad.SelectedIndex >= 0 And (Obj.Type = uWidgetEmpty.TypeOfWidget.Volet Or Obj.Type = uWidgetEmpty.TypeOfWidget.Moteur Or Obj.Type = uWidgetEmpty.TypeOfWidget.Prise) Then
                Dim x As ComboBoxItem = CbDeviceKeyPad.Items(CbDeviceKeyPad.SelectedIndex)
                Obj.Id = x.Tag
            End If

            DialogResult = True
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur: " & ex.Message, "Erreur", "Erreur: " & ex.Message)
        End Try
    End Sub

    Private Sub BtnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnCancel.Click
        DialogResult = False
    End Sub
#End Region

    Private Function ShowDialog(ByVal color As Nullable(Of System.Windows.Media.Color)) As Nullable(Of System.Windows.Media.Color)
        ' Instancier une boite de dilogue de Winform 
        Dim dialogBox As New System.Windows.Forms.ColorDialog()

        ' Configurer cette boite
        If color.HasValue Then
            dialogBox.Color = System.Drawing.Color.FromArgb(color.Value.A, color.Value.R, color.Value.G, color.Value.B)
        End If

        ' Affichage de la boite de dialogue
        If dialogBox.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            ' Retourner la couleur choisie
            Return System.Windows.Media.Color.FromArgb(dialogBox.Color.A, dialogBox.Color.R, dialogBox.Color.G, dialogBox.Color.B)
        Else
            ' Selection annulée
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' Editer les actions
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BtnEditAction_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnEditAction.Click
        GrpEditAction.Visibility = Windows.Visibility.Visible
        CbAction.SelectedIndex = 0
    End Sub

    Private Sub BtnCloseEditAction_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnCloseEditAction.Click
        GrpEditAction.Visibility = Windows.Visibility.Collapsed
    End Sub

    ''' <summary>
    ''' Editer les visuels
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BtnEditVisu_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnEditVisu.Click
        GrpEditVisu.Visibility = Windows.Visibility.Visible
    End Sub

    Private Sub BtnCloseEditVisu_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnCloseEditVisu.Click
        GrpEditVisu.Visibility = Windows.Visibility.Collapsed
    End Sub

    Private Sub CbObjet_SelectionChanged(ByVal sender As System.Object, ByVal e As Object) Handles CbObjet.SelectionChanged, CbObjet.MouseLeftButtonUp
        If CbObjet.SelectedItem IsNot Nothing Then
            If CbObjet.SelectedItem.tag = "DEVICE" Then
                Dim _DeviceId As String
                Dim _Device As HoMIDom.HoMIDom.TemplateDevice

                _DeviceId = CbObjet.SelectedItem.uid
                _Device = myService.ReturnDeviceByID(IdSrv, _DeviceId)
                CbMethode.Items.Clear()
                CbActionValue.Items.Clear()

                If _Device.Type = Device.ListeDevices.MULTIMEDIA Then
                    Dim lbl1 As New ComboBoxItem
                    lbl1.Content = "EnvoyerCommande"
                    lbl1.Tag = 0 'c une fonction de base
                    CbMethode.Items.Add(lbl1)
                    LblMethode.Visibility = Windows.Visibility.Visible
                    TxtValue.Visibility = Windows.Visibility.Collapsed
                    CbMethode.Visibility = Windows.Visibility.Visible
                    CbMethode.SelectedIndex = 0

                    If String.IsNullOrEmpty(_Device.Modele) = False Then
                        TxtValue.Visibility = Windows.Visibility.Collapsed
                        Dim tpl As Telecommande.Template = myService.GetTemplateFromID(_Device.Modele)
                        If tpl IsNot Nothing Then
                            For Each cmd In tpl.Commandes
                                CbActionValue.Items.Add(cmd.Name)
                            Next
                        End If
                    End If
                Else
                    For i As Integer = 0 To _Device.DeviceAction.Count - 1
                        Dim lbl1 As New ComboBoxItem
                        lbl1.Content = _Device.DeviceAction.Item(i).Nom
                        lbl1.Tag = 0 'c une fonction de base
                        CbMethode.Items.Add(lbl1)
                        LblMethode.Visibility = Windows.Visibility.Visible
                        CbMethode.Visibility = Windows.Visibility.Visible
                    Next
                    TxtValue.Visibility = Windows.Visibility.Visible
                End If

                _Device = Nothing
                TxtValue.Text = Nothing

            End If

            If CbObjet.SelectedItem.tag = "MACRO" Then
                Dim _MacroId As String
                Dim _Macro As HoMIDom.HoMIDom.Macro

                _MacroId = CbObjet.SelectedItem.uid
                _Macro = myService.ReturnMacroById(IdSrv, _MacroId)
                CbMethode.Items.Clear()

                Dim lbl1 As New ComboBoxItem
                lbl1.Content = "Execute"
                lbl1.Tag = 8 'c une macro
                CbMethode.Items.Add(lbl1)
                CbMethode.SelectedIndex = 0
                _Macro = Nothing

                LblActionValue.Visibility = Windows.Visibility.Collapsed
                TxtValue.Visibility = Windows.Visibility.Collapsed
                TxtValue.Text = Nothing
            End If

            If CbObjet.SelectedItem.tag = "ZONE" Then
                Dim _ZoneId As String
                Dim _Zone As HoMIDom.HoMIDom.Zone

                _ZoneId = CbObjet.SelectedItem.uid
                _Zone = myService.ReturnZoneByID(IdSrv, _ZoneId)
                CbMethode.Items.Clear()

                Dim lbl1 As New ComboBoxItem
                lbl1.Content = "Afficher"
                lbl1.Tag = 9 'c une zone
                CbMethode.Items.Add(lbl1)
                CbMethode.SelectedIndex = 0
                _Zone = Nothing

                LblActionValue.Visibility = Windows.Visibility.Collapsed
                TxtValue.Visibility = Windows.Visibility.Collapsed
                TxtValue.Text = Nothing
            End If

            If CbObjet.SelectedItem.tag = "COMMAND" Then
                CbMethode.Items.Clear()

                Dim lbl1 As New ComboBoxItem
                lbl1.Content = "Exécuter commande DOS"
                lbl1.Tag = 10 'c une commande Shell
                CbMethode.Items.Add(lbl1)
                CbMethode.SelectedIndex = 0

                lbl1 = New ComboBoxItem
                lbl1.Content = "Charger configuration"
                lbl1.Tag = 11 'c une commande LoadConfig
                CbMethode.Items.Add(lbl1)
                CbMethode.SelectedIndex = 0

                lbl1 = New ComboBoxItem
                lbl1.Content = "Quitter HoMIWpF"
                lbl1.Tag = 12 'c une commande Quit
                CbMethode.Items.Add(lbl1)
                CbMethode.SelectedIndex = 0

                LblActionValue.Visibility = Windows.Visibility.Visible
                CbMethode.Visibility = Windows.Visibility.Visible

            End If
        End If
    End Sub

    Private Sub CbObjetVisu_SelectionChanged(ByVal sender As Object, ByVal e As Object) Handles CbObjetVisu.SelectionChanged, CbObjetVisu.MouseLeftButtonDown, CbObjetVisu.MouseDown
        If CbObjetVisu.SelectedIndex < 0 Or CbObjetVisu.SelectedItem.tag <> "DEVICE" Then
            Exit Sub
        End If

        CbPropertyVisu.Items.Clear()
        Select Case AllDevices.Item(CbObjetVisu.SelectedIndex).Type
            Case 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27
                CbPropertyVisu.Items.Add("Value")
            Case 17
                CbPropertyVisu.Items.Add("Value")
                CbPropertyVisu.Items.Add("ConditionActuel")
                CbPropertyVisu.Items.Add("TemperatureActuel")
                CbPropertyVisu.Items.Add("HumiditeActuel")
                CbPropertyVisu.Items.Add("VentActuel")
                CbPropertyVisu.Items.Add("JourToday")
                CbPropertyVisu.Items.Add("MinToday")
                CbPropertyVisu.Items.Add("MaxToday")
                CbPropertyVisu.Items.Add("ConditionToday")
                CbPropertyVisu.Items.Add("JourJ1")
                CbPropertyVisu.Items.Add("MinJ1")
                CbPropertyVisu.Items.Add("MaxJ1")
                CbPropertyVisu.Items.Add("ConditionJ1")
                CbPropertyVisu.Items.Add("JourJ2")
                CbPropertyVisu.Items.Add("MinJ2")
                CbPropertyVisu.Items.Add("MaxJ2")
                CbPropertyVisu.Items.Add("ConditionJ2")
                CbPropertyVisu.Items.Add("JourJ3")
                CbPropertyVisu.Items.Add("MinJ3")
                CbPropertyVisu.Items.Add("MaxJ3")
                CbPropertyVisu.Items.Add("ConditionJ3")
            Case Else
                CbPropertyVisu.Items.Add("Value")
        End Select

    End Sub

    Private Sub CbAction_MouseLeftButtonDown(ByVal sender As System.Object, ByVal e As Object) Handles CbAction.MouseLeftButtonUp, CbAction.SelectionChanged
        BtnOkAction.Visibility = Windows.Visibility.Collapsed
        LblObjet.Visibility = Windows.Visibility.Collapsed
        CbObjet.Visibility = Windows.Visibility.Collapsed
        LblMethode.Visibility = Windows.Visibility.Collapsed
        CbMethode.Visibility = Windows.Visibility.Collapsed
        TxtValue.Visibility = Windows.Visibility.Collapsed
        CbActionValue.Visibility = Windows.Visibility.Collapsed
        LblActionValue.Visibility = Windows.Visibility.Collapsed

        Refresh_LstObjetAction()
    End Sub

    Private Sub Refresh_LstObjetAction()
        Dim _list As New List(Of cWidget.Action)

        Select Case CbAction.SelectedIndex
            Case 0
                _list = Obj.Action_On_Click
            Case 1
                _list = Obj.Action_On_LongClick
            Case 2
                _list = Obj.Action_GestureGaucheDroite
            Case 3
                _list = Obj.Action_GestureDroiteGauche
            Case 4
                _list = Obj.Action_GestureHautBas
            Case 5
                _list = Obj.Action_GestureBasHaut
        End Select

        LstObjetActions.Items.Clear()
        For Each Action As cWidget.Action In _list
            Dim x As New ListBoxItem
            x.Uid = Action.IdObject

            Dim _dev As TemplateDevice = myService.ReturnDeviceByID(IdSrv, Action.IdObject)
            If _dev IsNot Nothing Then
                x.Content = _dev.Name
            Else
                Dim _mac As Macro = myService.ReturnMacroById(IdSrv, Action.IdObject)
                If _mac IsNot Nothing Then
                    x.Content = _mac.Nom & "[Macro]"
                Else
                    Dim _zon As Zone = myService.ReturnZoneByID(IdSrv, Action.IdObject)
                    If _zon IsNot Nothing Then
                        x.Content = _zon.Name & "[Zone]"
                    Else
                        x.Content = "Commande"
                    End If
                End If
            End If
            LstObjetActions.Items.Add(x)
        Next

    End Sub

    Private Sub LstObjetActions_SelectionChanged(ByVal sender As Object, ByVal e As Object) Handles LstObjetActions.SelectionChanged, LstObjetActions.MouseLeftButtonUp
        If LstObjetActions.SelectedIndex >= 0 Then
            Dim _act As cWidget.Action = Nothing

            LblActionValue.Visibility = Windows.Visibility.Collapsed
            CbActionValue.Visibility = Windows.Visibility.Collapsed
            TxtValue.Visibility = Windows.Visibility.Collapsed

            Select Case CbAction.SelectedIndex
                Case 0
                    _act = Obj.Action_On_Click.Item(LstObjetActions.SelectedIndex)
                Case 1
                    _act = Obj.Action_On_LongClick.Item(LstObjetActions.SelectedIndex)
                Case 2
                    _act = Obj.Action_GestureGaucheDroite.Item(LstObjetActions.SelectedIndex)
                Case 3
                    _act = Obj.Action_GestureDroiteGauche.Item(LstObjetActions.SelectedIndex)
                Case 4
                    _act = Obj.Action_GestureHautBas.Item(LstObjetActions.SelectedIndex)
                Case 5
                    _act = Obj.Action_GestureBasHaut.Item(LstObjetActions.SelectedIndex)
            End Select

            Dim _idx As Integer = -1
            For i As Integer = 0 To CbObjet.Items.Count - 1
                If CbObjet.Items(i).uid = _act.IdObject Then
                    _idx = i
                    Exit For
                End If
            Next
            CbObjet.SelectedIndex = _idx

            _idx = -1
            CbMethode.SelectedIndex = -1
            For i As Integer = 0 To CbMethode.Items.Count - 1
                If CbMethode.Items(i).Content = _act.Methode Then
                    _idx = i
                    Exit For
                End If
            Next
            If _idx >= 0 Then
                CbMethode.SelectedIndex = _idx
            Else
                CbMethode.Text = _act.Methode
            End If


            If _act.Value.ToString = Nothing Then
                TxtValue.Visibility = Windows.Visibility.Collapsed
                LblActionValue.Visibility = Windows.Visibility.Collapsed
            Else
                LblActionValue.Visibility = Windows.Visibility.Visible
                TxtValue.Text = _act.Value.ToString
                CbActionValue.Text = _act.Value.ToString
                If CbMethode.Text <> "EnvoyerCommande" Then
                    TxtValue.Visibility = Windows.Visibility.Visible
                    CbActionValue.Visibility = Windows.Visibility.Collapsed
                Else
                    TxtValue.Visibility = Windows.Visibility.Collapsed
                    CbActionValue.Visibility = Windows.Visibility.Visible
                End If
            End If

            BtnOkAction.Visibility = Windows.Visibility.Visible
            LblMethode.Visibility = Windows.Visibility.Visible
            CbMethode.Visibility = Windows.Visibility.Visible
        End If
    End Sub

    Private Sub BtnOkAction_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOkAction.Click
        If String.IsNullOrEmpty(CbObjet.Text) Or CbObjet.SelectedIndex < 0 Then
            MessageBox.Show("Veuillez sélectionner un Objet!", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Exit Sub
        End If
        If String.IsNullOrEmpty(CbMethode.Text) Or CbMethode.SelectedIndex < 0 Then
            MessageBox.Show("Veuillez sélectionner une méthode!", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Exit Sub
        End If
        If TxtValue.Visibility = Windows.Visibility.Visible And String.IsNullOrEmpty(TxtValue.Text) Then
            MessageBox.Show("Veuillez saisir une valeur!", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Exit Sub
        End If

        Dim _act As New cWidget.Action
        With _act
            .IdObject = CbObjet.SelectedItem.uid
            .Methode = CbMethode.Text
            If CbMethode.Text <> "EnvoyerCommande" Then
                .Value = TxtValue.Text
            Else
                .Value = CbActionValue.Text
            End If
        End With

        If _FlagNewAction = False Then
            Select Case CbAction.SelectedIndex
                Case 0
                    Obj.Action_On_Click.Item(LstObjetActions.SelectedIndex) = _act
                Case 1
                    Obj.Action_On_LongClick.Item(LstObjetActions.SelectedIndex) = _act
                Case (2)
                    Obj.Action_GestureGaucheDroite.Item(LstObjetActions.SelectedIndex) = _act
                Case 3
                    Obj.Action_GestureDroiteGauche.Item(LstObjetActions.SelectedIndex) = _act
                Case 4
                    Obj.Action_GestureHautBas.Item(LstObjetActions.SelectedIndex) = _act
                Case 5
                    Obj.Action_GestureBasHaut.Item(LstObjetActions.SelectedIndex) = _act
            End Select
        Else
            Select Case CbAction.SelectedIndex
                Case 0
                    Obj.Action_On_Click.Add(_act)
                Case 1
                    Obj.Action_On_LongClick.Add(_act)
                Case (2)
                    Obj.Action_GestureGaucheDroite.Add(_act)
                Case 3
                    Obj.Action_GestureDroiteGauche.Add(_act)
                Case 4
                    Obj.Action_GestureHautBas.Add(_act)
                Case 5
                    Obj.Action_GestureBasHaut.Add(_act)
            End Select
        End If

        TxtValue.Text = Nothing
        Refresh_LstObjetAction()
        _FlagNewAction = False
        LstObjetActions.SelectedIndex = -1
        BtnOkAction.Visibility = Windows.Visibility.Collapsed
        LblObjet.Visibility = Windows.Visibility.Collapsed
        CbObjet.Visibility = Windows.Visibility.Collapsed
        LblMethode.Visibility = Windows.Visibility.Collapsed
        CbMethode.Visibility = Windows.Visibility.Collapsed
        LblActionValue.Visibility = Windows.Visibility.Collapsed
        TxtValue.Visibility = Windows.Visibility.Collapsed
        CbActionValue.Visibility = Windows.Visibility.Collapsed
    End Sub

    Private Sub BtnNewAction_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnNewAction.Click
        BtnOkAction.Visibility = Windows.Visibility.Visible
        LblObjet.Visibility = Windows.Visibility.Visible
        CbObjet.Visibility = Windows.Visibility.Visible
        CbObjet.SelectedIndex = -1
        TxtValue.Visibility = Windows.Visibility.Collapsed
        LblActionValue.Visibility = Windows.Visibility.Collapsed
        _FlagNewAction = True
    End Sub

    Private Sub BtnDelAction_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDelAction.Click
        If LstObjetActions.SelectedIndex < 0 Then
            MessageBox.Show("Veuillez sélectionner une action à supprimer!", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Exit Sub
        Else
            Select Case CbAction.SelectedIndex
                Case 0
                    Obj.Action_On_Click.RemoveAt(LstObjetActions.SelectedIndex)
                Case 1
                    Obj.Action_On_LongClick.RemoveAt(LstObjetActions.SelectedIndex)
                Case (2)
                    Obj.Action_GestureGaucheDroite.RemoveAt(LstObjetActions.SelectedIndex)
                Case 3
                    Obj.Action_GestureDroiteGauche.RemoveAt(LstObjetActions.SelectedIndex)
                Case 4
                    Obj.Action_GestureHautBas.RemoveAt(LstObjetActions.SelectedIndex)
                Case 5
                    Obj.Action_GestureBasHaut.RemoveAt(LstObjetActions.SelectedIndex)
            End Select
            Refresh_LstObjetAction()
        End If
    End Sub

    Private Sub CbMethode_SelectionChanged(ByVal sender As Object, ByVal e As Object) Handles CbMethode.SelectionChanged, CbMethode.MouseLeftButtonUp, CbMethode.SourceUpdated
        Dim _dev As TemplateDevice = myService.ReturnDeviceByID(IdSrv, CbObjet.SelectedItem.uid)

        If CbMethode.SelectedIndex < 0 Then Exit Sub

        TxtValue.Visibility = Windows.Visibility.Collapsed
        LblActionValue.Visibility = Windows.Visibility.Collapsed
        CbActionValue.Visibility = Windows.Visibility.Collapsed

        If _dev IsNot Nothing Then
            If _dev.DeviceAction.Item(CbMethode.SelectedIndex).Parametres.Count > 0 Then
                LblActionValue.Visibility = Windows.Visibility.Visible
                If _dev.Type <> Device.ListeDevices.MULTIMEDIA Then
                    TxtValue.Visibility = Windows.Visibility.Visible
                    CbActionValue.Visibility = Windows.Visibility.Collapsed
                Else
                    TxtValue.Visibility = Windows.Visibility.Collapsed
                    CbActionValue.Visibility = Windows.Visibility.Visible
                End If
                'TxtValue.Visibility = Windows.Visibility.Visible
            Else
                TxtValue.Text = String.Empty
            End If
        End If

        If CType(CbMethode.SelectedItem, ComboBoxItem).Content = "Exécuter commande DOS" Or CType(CbMethode.SelectedItem, ComboBoxItem).Content = "Charger configuration" Then
            LblActionValue.Visibility = Windows.Visibility.Visible
            TxtValue.Visibility = Windows.Visibility.Visible
        End If

    End Sub

    Private Sub Refresh_LstObjetVisu()
        Try
            LstObjetVisu.Items.Clear()
            TxtText.Text = ""
            TxtValueVisu.Text = ""

            For Each Visu As cWidget.Visu In Obj.Visuel
                Dim x As New ListBoxItem
                x.Uid = Visu.IdObject
                Dim _dev As TemplateDevice = myService.ReturnDeviceByID(IdSrv, Visu.IdObject)
                If _dev IsNot Nothing Then
                    x.Content = _dev.Name
                    LstObjetVisu.Items.Add(x)
                End If
            Next
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur Refresh_LstObjetVisu: " & ex.Message, "Erreur", "Refresh_LstObjetVisu")
        End Try
    End Sub

    Private Sub LstObjetVisu_SelectionChanged(ByVal sender As System.Object, ByVal e As Object) Handles LstObjetVisu.SelectionChanged, LstObjetVisu.MouseLeftButtonDown
        Try
            If LstObjetVisu.SelectedIndex >= 0 Then
                Dim _act As cWidget.Visu = Obj.Visuel.Item(LstObjetVisu.SelectedIndex)

                Dim _idx As Integer = -1
                For i As Integer = 0 To CbObjetVisu.Items.Count - 1
                    If CbObjetVisu.Items(i).uid = _act.IdObject Then
                        _idx = i
                        Exit For
                    End If
                Next
                CbObjetVisu.SelectedIndex = _idx

                _idx = -1
                CbPropertyVisu.SelectedValue = _act.Propriete
                CbOperateurVisu.SelectedIndex = _act.Operateur

                TxtValueVisu.Text = _act.Value.ToString
                If _act.Image IsNot Nothing Then
                    ImgVisu.Source = ConvertArrayToImage(myService.GetByteFromImage(_act.Image))
                    ImgVisu.Tag = _act.Image
                End If

                TxtText.Text = _act.Text

                BtnOkVisu.Visibility = Windows.Visibility.Visible
                LblProperty.Visibility = Windows.Visibility.Visible
                CbPropertyVisu.Visibility = Windows.Visibility.Visible
                LblVisuValue.Visibility = Windows.Visibility.Visible
                TxtValueVisu.Visibility = Windows.Visibility.Visible
                LblPicture.Visibility = Windows.Visibility.Visible
                ImgVisu.Visibility = Windows.Visibility.Visible
                BtnImgVisu.Visibility = Windows.Visibility.Visible
                LblOperateur.Visibility = Windows.Visibility.Visible
                CbOperateurVisu.Visibility = Windows.Visibility.Visible
                LblText.Visibility = Windows.Visibility.Visible
                TxtText.Visibility = Windows.Visibility.Visible
            End If
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur LstObjetVisu_SelectionChanged: " & ex.ToString, "Erreur", "LstObjetVisu_SelectionChanged")
        End Try

    End Sub

    Private Sub BtnNewVisu_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnNewVisu.Click
        BtnOkVisu.Visibility = Windows.Visibility.Visible
        LblObjetVisu.Visibility = Windows.Visibility.Visible
        CbObjetVisu.Visibility = Windows.Visibility.Visible
        CbPropertyVisu.Visibility = Windows.Visibility.Visible
        LblProperty.Visibility = Windows.Visibility.Visible
        LblVisuValue.Visibility = Windows.Visibility.Visible
        TxtValueVisu.Visibility = Windows.Visibility.Visible
        LblPicture.Visibility = Windows.Visibility.Visible
        ImgVisu.Visibility = Windows.Visibility.Visible
        BtnImgVisu.Visibility = Windows.Visibility.Visible
        LblOperateur.Visibility = Windows.Visibility.Visible
        CbOperateurVisu.Visibility = Windows.Visibility.Visible
        LblText.Visibility = Windows.Visibility.Visible
        TxtText.Visibility = Windows.Visibility.Visible
        _FlagNewVisu = True
    End Sub

    Private Sub BtnDelVisu_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDelVisu.Click
        If LstObjetVisu.SelectedIndex < 0 Then
            MessageBox.Show("Veuillez sélectionner un visuel à supprimer!", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Exit Sub
        Else
            Obj.Visuel.RemoveAt(LstObjetVisu.SelectedIndex)
            Refresh_LstObjetVisu()
        End If
    End Sub

    Private Sub BtnOkVisu_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOkVisu.Click
        Try
            If CbObjetVisu.Text = "" Or CbObjetVisu.SelectedIndex < 0 Then
                MessageBox.Show("Veuillez sélectionner un Objet!", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Sub
            End If
            If CbPropertyVisu.Text = "" Or CbPropertyVisu.SelectedIndex < 0 Then
                MessageBox.Show("Veuillez sélectionner une propriété!", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Sub
            End If
            If TxtValueVisu.Text = "" Then
                MessageBox.Show("Veuillez saisir une valeur!", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Sub
            End If

            Dim _act As New cWidget.Visu
            With _act
                .IdObject = CbObjetVisu.SelectedItem.uid
                .Propriete = CbPropertyVisu.Text
                .Value = TxtValueVisu.Text
                .Image = ImgVisu.Tag
                .Operateur = CbOperateurVisu.SelectedIndex
                .Text = TxtText.Text
            End With

            If _FlagNewVisu = False Then
                Obj.Visuel.Item(LstObjetVisu.SelectedIndex) = _act
            Else
                Obj.Visuel.Add(_act)
            End If

            TxtValueVisu.Text = ""
            TxtText.Text = ""
            Refresh_LstObjetVisu()
            _FlagNewVisu = False
            LstObjetVisu.SelectedIndex = -1
            BtnOkVisu.Visibility = Windows.Visibility.Collapsed
            LblObjetVisu.Visibility = Windows.Visibility.Collapsed
            CbObjetVisu.Visibility = Windows.Visibility.Collapsed
            LblProperty.Visibility = Windows.Visibility.Collapsed
            CbPropertyVisu.Visibility = Windows.Visibility.Collapsed
            TxtValueVisu.Visibility = Windows.Visibility.Collapsed
            LblVisuValue.Visibility = Windows.Visibility.Collapsed
            LblPicture.Visibility = Windows.Visibility.Collapsed
            ImgVisu.Visibility = Windows.Visibility.Collapsed
            BtnImgVisu.Visibility = Windows.Visibility.Collapsed
            LblOperateur.Visibility = Windows.Visibility.Collapsed
            CbOperateurVisu.Visibility = Windows.Visibility.Collapsed
            LblText.Visibility = Windows.Visibility.Collapsed
            TxtText.Visibility = Windows.Visibility.Collapsed
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur BtnOkVisu_Click: " & ex.Message, "Erreur", "LstObjetVisu_SelectionChanged")
        End Try
    End Sub

    Private Sub ImgVisu_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ImgVisu.MouseDown, BorderVisu.MouseDown
        Try
            Dim frm As New WindowImg
            frm.ShowDialog()
            If frm.DialogResult.HasValue And frm.DialogResult.Value Then
                Dim retour As String = frm.FileName
                If String.IsNullOrEmpty(retour) = False Then
                    ImgVisu.Source = ConvertArrayToImage(myService.GetByteFromImage(retour))
                    ImgVisu.Tag = retour
                End If
                frm.Close()
            Else
                frm.Close()
            End If
            frm = Nothing
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur WWidgetProperty.ImgVisu_MouseDown: " & ex.Message, "Erreur", "WWidgetProperty.ImgVisu_MouseDown")
        End Try
    End Sub

    Private Sub BtnImgVisu_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnImgVisu.Click
        Try
            Dim frm As New WindowImg
            frm.ShowDialog()
            If frm.DialogResult.HasValue And frm.DialogResult.Value Then
                Dim retour As String = frm.FileName
                If String.IsNullOrEmpty(retour) = False Then
                    ImgVisu.Source = ConvertArrayToImage(myService.GetByteFromImage(retour))
                    ImgVisu.Tag = retour
                End If
                frm.Close()
            Else
                frm.Close()
            End If
            frm = Nothing
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur WWidgetProperty.ImgVisu_Click: " & ex.Message, "Erreur", "WWidgetProperty.ImgVisu_Click")
        End Try
    End Sub

    Private Sub BtnDelete_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDelete.Click
        For i As Integer = 0 To _ListElement.Count - 1
            If _ListElement.Item(i).Uid = Obj.Uid And _ListElement.Item(i).ZoneId = Obj.ZoneId Then
                _ListElement.RemoveAt(i)
                For j As Integer = 0 To frmMere.Canvas1.Children.Count - 1
                    If frmMere.Canvas1.Children.Item(j).Uid = Obj.Uid Then
                        frmMere.Canvas1.Children.RemoveAt(j)
                        Exit For
                    End If
                Next
                Exit For
            End If
        Next

        DialogResult = True
    End Sub

    Private Sub ImgPicture_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ImgPicture.MouseDown
        Try
            Dim frm As New WindowImg
            frm.ShowDialog()
            If frm.DialogResult.HasValue And frm.DialogResult.Value Then
                Dim retour As String = frm.FileName
                If String.IsNullOrEmpty(retour) = False Then
                    ImgPicture.Source = ConvertArrayToImage(myService.GetByteFromImage(retour))
                    ImgPicture.Tag = retour
                End If
                frm.Close()
            Else
                frm.Close()
            End If
            frm = Nothing
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur ImgPicture.MouseDown: " & ex.Message, "Erreur", "ImgPicture_MouseDown")
        End Try
    End Sub

    Private Sub BtnInitPict_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnInitPict.Click
        ImgPicture.Source = Nothing
        ImgPicture.Tag = "-defaut.png"
    End Sub

    Private Sub BtnInitPictLocal_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnInitPict_Local.Click
        Try
            ' l'utilisateur ouvre une image située sur l'ordinateur client
            Dim openFileDialog1 As New Microsoft.Win32.OpenFileDialog()
            openFileDialog1.Filter = "Fichiers image (*.PNG;*.JPG;*.GIF)|*.PNG;*.JPG;*.GIF|All files (*.*)|*.*"
            openFileDialog1.FilterIndex = 1
            openFileDialog1.RestoreDirectory = True
            Dim DlgResult As Forms.DialogResult
            DlgResult = openFileDialog1.ShowDialog()
            If DlgResult = Forms.DialogResult.OK Or DlgResult = -1 Then
                ImgPicture.Source = New BitmapImage(New Uri(openFileDialog1.FileName))
                ImgPicture.Tag = openFileDialog1.FileName
            End If
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur ImgPicture.BtnInitPictLocal_Click: " & ex.Message, "Erreur", "ImgPicture.BtnInitPictLocal_Click")
        End Try

    End Sub

    Private Sub BtnInitPictServeur_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnInitPict_Serveur.Click
        Try
            ' Image téléchargée depuis le serveur: une boîte de dialogue contenant les images du serveur est affichée.
            Dim frm As New WindowImg
            frm.ShowDialog()

            If frm.DialogResult.HasValue And frm.DialogResult.Value Then
                Dim retour As String = frm.FileName
                If String.IsNullOrEmpty(retour) = False Then
                    ImgPicture.Source = ConvertArrayToImage(myService.GetByteFromImage(retour))
                    ImgPicture.Tag = retour
                End If
                frm.Close()
            Else
                frm.Close()
            End If
            frm = Nothing

        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur ImgPicture.BtnInitPictServeur_Click: " & ex.ToString, "Erreur", "ImgPicture.BtnInitPictServeur_Click")
        End Try

    End Sub

#Region "Http"
    Dim _FlagNewBtnhttp As Boolean = False

    Private Sub BtnCloseProperty_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnCloseProperty.Click
        GrpPropertyHttp.Visibility = Windows.Visibility.Collapsed
    End Sub

    Private Sub HttpBtnNewBtn_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles HttpBtnNewBtn.Click
        HttpStkEditBtn.Visibility = Windows.Visibility.Visible
        TxtHttpLabel.Text = ""
        TxtHttpURL.Text = ""
        TxtHttpWidth.Text = "20"
        TxtHttpHeight.Text = "20"
        _FlagNewBtnhttp = True
    End Sub

    Private Sub HttpDelBtn_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles HttpDelBtn.Click
        Try
            If HttpListBtn.SelectedIndex < 0 Then
                MessageBox.Show("Veuillez sélectionner un bouton à supprimer!", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Sub
            Else
                Objet.ListHttpButton.RemoveAt(HttpListBtn.SelectedIndex)
                Refresh_LstHttpBtn()
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur: " & ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Sub Refresh_LstHttpBtn()
        Try
            HttpListBtn.Items.Clear()

            For i As Integer = 0 To _listhttpbtn.Count - 1
                HttpListBtn.Items.Add(_listhttpbtn.Item(i).Content)
            Next
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur WWidgetProperty.Refresh_LstHttpBtn: " & ex.Message, "Erreur", "WWidgetProperty.Refresh_LstHttpBtn")
        End Try
    End Sub

    Private Sub HttpOkBtn_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles HttpOkBtn.Click
        Try
            If String.IsNullOrEmpty(TxtHttpLabel.Text) = True Then
                MessageBox.Show("Le Label du nouveau bouton est obligatoire!", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Sub
            ElseIf String.IsNullOrEmpty(TxtHttpURL.Text) = True Then
                MessageBox.Show("L'URL du nouveau bouton est obligatoire!", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Sub
            ElseIf UrlIsValid(TxtHttpURL.Text) = False Then
                MessageBox.Show("L'URL du widget n'est pas valide!", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            ElseIf String.IsNullOrEmpty(TxtHttpWidth.Text) = True Then
                MessageBox.Show("La propriété Width du nouveau bouton est obligatoire!", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Sub
            ElseIf String.IsNullOrEmpty(TxtHttpHeight.Text) = True Then
                MessageBox.Show("La propriété Height du nouveau bouton est obligatoire!", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Sub
            End If

            If _FlagNewBtnhttp Then
                Dim x As New uHttp.ButtonHttp
                With x
                    .Content = TxtHttpLabel.Text
                    .URL = TxtHttpURL.Text
                    .Width = TxtHttpWidth.Text
                    .Height = TxtHttpHeight.Text
                End With
                HttpListBtn.Items.Add(x.Content)
                _listhttpbtn.Add(x)
            Else
                If HttpListBtn.SelectedIndex >= 0 Then
                    _listhttpbtn.Item(HttpListBtn.SelectedIndex).Content = TxtHttpLabel.Text
                    _listhttpbtn.Item(HttpListBtn.SelectedIndex).URL = TxtHttpURL.Text
                    _listhttpbtn.Item(HttpListBtn.SelectedIndex).Width = TxtHttpWidth.Text
                    _listhttpbtn.Item(HttpListBtn.SelectedIndex).Height = TxtHttpHeight.Text
                End If
            End If
            HttpStkEditBtn.Visibility = Visibility.Collapsed
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur HttpOkBtn_Click: " & ex.Message, "Erreur", "HttpOkBtn_Click")
        End Try
    End Sub

    Private Sub HttpListBtn_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles HttpListBtn.SelectionChanged
        Try
            If HttpListBtn.SelectedIndex >= 0 Then
                Dim _btn As uHttp.ButtonHttp = Objet.ListHttpButton.Item(HttpListBtn.SelectedIndex)

                HttpStkEditBtn.Visibility = Windows.Visibility.Visible

                TxtHttpLabel.Text = _btn.Content
                TxtHttpURL.Text = _btn.URL
                TxtHttpWidth.Text = _btn.Width
                TxtHttpHeight.Text = _btn.Height
            End If
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur HttpListBtn_SelectionChanged: " & ex.Message, "Erreur", "HttpListBtn_SelectionChanged")
        End Try
    End Sub

    Private Sub HttpSearchIcon_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles HttpSearchIcon.Click
        Try
            Dim frm As New WindowImg
            frm.ShowDialog()
            If frm.DialogResult.HasValue And frm.DialogResult.Value Then
                Dim retour As String = frm.FileName
                If String.IsNullOrEmpty(retour) = False Then
                    HttpIcon.Source = ConvertArrayToImage(myService.GetByteFromImage(retour))
                    HttpIcon.Tag = retour
                End If
                frm.Close()
            Else
                frm.Close()
            End If
            frm = Nothing
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur HttpSearchIcon_Click: " & ex.Message, "Erreur", "HttpSearchIcon_Click")
        End Try
    End Sub

    Private Sub TxtHttpWidth_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtHttpWidth.TextChanged
        If IsNumeric(TxtHttpWidth.Text) = False Then
            MessageBox.Show("La valeur doit être numérique", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End If
    End Sub

    Private Sub TxtHttpHeight_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtHttpHeight.TextChanged
        If IsNumeric(TxtHttpHeight.Text) = False Then
            MessageBox.Show("La valeur doit être numérique", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End If
    End Sub

    Private Sub TxtRefresh_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtRefresh.TextChanged
        If String.IsNullOrEmpty(TxtRefresh.Text) Then
            e.Handled = True
            MessageBox.Show("La valeur doit être numérique", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        ElseIf 0 < CInt(TxtRefresh.Text) > 3600 Then
            e.Handled = True
            MessageBox.Show("La valeur doit être comprise entre 0 et 3600", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End If
    End Sub

#End Region


    Private Sub TxtValueVisu_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtValueVisu.TextChanged
        Dim _flag As Boolean = False

        Try
            If String.IsNullOrEmpty(CbObjetVisu.Text) = False And String.IsNullOrEmpty(CbPropertyVisu.Text) = False And String.IsNullOrEmpty(TxtValueVisu.Text) = False Then
                Dim _ID As String = AllDevices.Item(CbObjetVisu.SelectedIndex).ID
                If String.IsNullOrEmpty(_ID) Then Exit Sub

                Dim _type As String = myService.TypeOfPropertyOfDevice(_ID, CbPropertyVisu.Text)
                Dim _obj As Object = Nothing

                If String.IsNullOrEmpty(_type) = False Then
                    Select Case _type.ToLower
                        Case "string"
                            Try
                                _obj = CStr(TxtValueVisu.Text)
                            Catch ex As Exception
                                MessageBox.Show("Veuillez saisir un type String !!", "ERREUR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                TxtValueVisu.Text = ""
                                _flag = True
                            End Try
                        Case "boolean"
                            Try
                                _obj = CBool(TxtValueVisu.Text)
                            Catch ex As Exception
                                MessageBox.Show("Veuillez saisir un type Boolean !!", "ERREUR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                TxtValueVisu.Text = 0
                                _flag = True
                            End Try
                        Case "byte"
                            Try
                                _obj = CByte(TxtValueVisu.Text)
                            Catch ex As Exception
                                MessageBox.Show("Veuillez saisir un type Byte !!", "ERREUR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                TxtValueVisu.Text = 0
                                _flag = True
                            End Try
                        Case "char"
                            Try
                                _obj = CChar(TxtValueVisu.Text)
                            Catch ex As Exception
                                MessageBox.Show("Veuillez saisir un type Char !!", "ERREUR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                TxtValueVisu.Text = ""
                                _flag = True
                            End Try
                        Case "datetime"
                            Try
                                _obj = CDate(TxtValueVisu.Text)
                            Catch ex As Exception
                                MessageBox.Show("Veuillez saisir un type DateTime !!", "ERREUR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                TxtValueVisu.Text = ""
                                _flag = True
                            End Try
                        Case "decimal"
                            Try
                                _obj = CDec(TxtValueVisu.Text)
                            Catch ex As Exception
                                MessageBox.Show("Veuillez saisir un type Decimal !!", "ERREUR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                TxtValueVisu.Text = 0
                                _flag = True
                            End Try
                        Case "double"
                            Try
                                _obj = CDbl(TxtValueVisu.Text)
                            Catch ex As Exception
                                MessageBox.Show("Veuillez saisir un type Double !!", "ERREUR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                TxtValueVisu.Text = "0"
                                _flag = True
                            End Try
                        Case "long"
                            Try
                                _obj = CLng(TxtValueVisu.Text)
                            Catch ex As Exception
                                MessageBox.Show("Veuillez saisir un type Long !!", "ERREUR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                TxtValueVisu.Text = "0"
                                _flag = True
                            End Try
                        Case "integer"
                            Try
                                _obj = CInt(TxtValueVisu.Text)
                            Catch ex As Exception
                                TxtValueVisu.Text = "0"
                                _flag = True
                                MessageBox.Show("Veuillez saisir un type Integer !!", "ERREUR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                            End Try
                        Case "int16"
                            Try
                                _obj = CInt(TxtValueVisu.Text)
                            Catch ex As Exception
                                TxtValueVisu.Text = "0"
                                _flag = True
                                MessageBox.Show("Veuillez saisir un type Integer !!", "ERREUR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                            End Try
                        Case "int32"
                            Try
                                _obj = CInt(TxtValueVisu.Text)
                            Catch ex As Exception
                                TxtValueVisu.Text = "0"
                                _flag = True
                                MessageBox.Show("Veuillez saisir un type Integer !!", "ERREUR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                            End Try
                        Case "int64"
                            Try
                                _obj = CInt(TxtValueVisu.Text)
                            Catch ex As Exception
                                TxtValueVisu.Text = "0"
                                _flag = True
                                MessageBox.Show("Veuillez saisir un type Integer !!", "ERREUR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                            End Try
                        Case "single"
                            Try
                                _obj = CSng(TxtValueVisu.Text)
                            Catch ex As Exception
                                MessageBox.Show("Veuillez saisir un type Single !!", "ERREUR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                TxtValueVisu.Text = "0"
                                _flag = True
                            End Try
                    End Select
                End If
            End If
        Catch ex As Exception
            If _flag = False Then AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur TxtValue_TextChanged: " & ex.Message, "Erreur", "TxtValueVisu_TextChanged")
        End Try
    End Sub

    Private Sub ColorPicker3_SelectColorChange(ByVal sender As System.Windows.Media.Brush) Handles ColorPicker3.SelectColorChange
        lblColorEtiquette.Background = ColorPicker3.SelectedColor
    End Sub

    Private Sub ColorPicker1_SelectColorChange(ByVal sender As System.Windows.Media.Brush) Handles ColorPicker1.SelectColorChange
        ' L'utilisation de couleurs nommées (p.ex. colors.antiqueblue) rend impossible l'attribution de la transparence
        ' par affectation de la valeur alpha dans les valeurs ARGB.
        ' Chaque fois que l'utilisateur choisit une couleur nommée dans la liste, on la convertir en valeur ARGB
        Dim mycolor As System.Windows.Media.Color
        Dim R, G, B As Byte
        R = CByte("&H" & ColorPicker1.SelectedColor.ToString.Substring(3, 2))
        G = CByte("&H" & ColorPicker1.SelectedColor.ToString.Substring(5, 2))
        B = CByte("&H" & ColorPicker1.SelectedColor.ToString.Substring(7, 2))
        mycolor = System.Windows.Media.Color.FromArgb(Slider1.Value, R, G, B)
        Dim mybrush = New SolidColorBrush(mycolor)
        lblColor.Background = mybrush
    End Sub

    Private Sub ColorPicker2_SelectColorChange(ByVal sender As System.Windows.Media.Brush) Handles ColorPicker2.SelectColorChange
        lblColorStatus.Background = ColorPicker2.SelectedColor
    End Sub

    Private Sub ColorPicker4_SelectColorChange(ByVal sender As System.Windows.Media.Brush) Handles ColorPicker4.SelectColorChange
        lblColorBorder.Background = ColorPicker4.SelectedColor
    End Sub


    Private Sub Slider1_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles Slider1.ValueChanged
        Dim mycolor As System.Windows.Media.Color
        Dim R, G, B As Byte
        R = CByte("&H" & lblColor.Background.ToString.Substring(3, 2))
        G = CByte("&H" & lblColor.Background.ToString.Substring(5, 2))
        B = CByte("&H" & lblColor.Background.ToString.Substring(7, 2))
        mycolor = System.Windows.Media.Color.FromArgb(Slider1.Value, R, G, B)
        Dim mybrush = New SolidColorBrush(mycolor)
        lblColor.Background = mybrush
    End Sub

#Region "KeyPad"
    Private Sub BtnClosePropertyKeypad_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClosePropertyKeypad.Click
        GrpPropertyKeyPad.Visibility = Windows.Visibility.Collapsed
    End Sub
#End Region

    Private Sub cbBalise_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles cbBalise.SelectionChanged
        TxtEtiq.Text = "<" & cbBalise.SelectedItem.Tag & ">"
    End Sub

    Private Sub TxtBorderSize_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs) Handles TxtBorderSize.TextChanged
        If IsNumeric(TxtBorderSize.Text) = False Then
            MessageBox.Show("Veuillez saisir une valeur numérique!", "Erreur", MessageBoxButtons.OK)
            TxtBorderSize.Text = 1
        End If
    End Sub

    Private Sub TxtBorderAngle_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs) Handles TxtBorderAngle.TextChanged
        If IsNumeric(TxtBorderAngle.Text) = False Then
            MessageBox.Show("Veuillez saisir une valeur numérique!", "Erreur", MessageBoxButtons.OK)
            TxtBorderAngle.Text = 6
        End If
    End Sub
End Class
