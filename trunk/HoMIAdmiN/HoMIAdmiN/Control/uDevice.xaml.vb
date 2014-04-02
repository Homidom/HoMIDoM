Imports HoMIDom.HoMIDom
Imports HoMIDom.HoMIDom.Device
Imports HoMIDom.HoMIDom.Api
Imports System.IO
Imports System.Threading
Imports System.Text.RegularExpressions
Imports System.ComponentModel


Partial Public Class uDevice

    '--- Variables ------------------
    Public Event CloseMe(ByVal MyObject As Object, ByVal Cancel As Boolean)
    Dim _Action As EAction 'Définit si modif ou création d'un device
    Dim _DeviceId As String = "" 'Id du device à modifier
    Dim FlagNewCmd, FlagNewDevice As Boolean
    Dim _Driver As HoMIDom.HoMIDom.TemplateDriver = Nothing
    Dim x As HoMIDom.HoMIDom.TemplateDevice = Nothing
    Dim ListeDrivers As List(Of TemplateDriver)
    Dim flagnewdev As Boolean = False
    Dim _ListVar As New Dictionary(Of String, String)


    Public Sub New(ByVal Action As Classe.EAction, ByVal DeviceId As String)
        Try
            ' Cet appel est requis par le Concepteur Windows Form.
            InitializeComponent()

            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            _DeviceId = DeviceId
            _Action = Action

            'Liste les type de devices dans le combo
            For Each value As ListeDevices In [Enum].GetValues(GetType(HoMIDom.HoMIDom.Device.ListeDevices))
                CbType.Items.Add(value.ToString)
            Next

            'Liste les drivers
            ListeDrivers = myService.GetAllDrivers(IdSrv)

            'Nouveau Device
            If Action = EAction.Nouveau Then

                'ajout de tous les drivers actif au combo
                For i As Integer = 0 To ListeDrivers.Count - 1
                    If ListeDrivers.Item(i).Enable Then CbDriver.Items.Add(ListeDrivers.Item(i).Nom)
                Next

                FlagNewDevice = True
                ImgDevice.Tag = ""
                CbType.IsEnabled = False

                'on cache des champs qui ne seront rendu visible que si le type selectionné le necessite
                StkValueMINMAX.Visibility = Windows.Visibility.Collapsed
                StkValueDefaultPrecision.Visibility = Windows.Visibility.Collapsed
                StkValueCorrectionFormatage.Visibility = Windows.Visibility.Collapsed

                'si c'est un nouveau device créé depuis un autocreateddevice alors on pré-rempli certains champs
                If NewDevice IsNot Nothing Then
                    TxtNom.Text = NewDevice.Name
                    TxtAdresse1.Text = NewDevice.Adresse1
                    TxtAdresse2.Text = NewDevice.Adresse2
                    CbDriver.SelectedValue = myService.ReturnDriverByID(IdSrv, NewDevice.IdDriver).Nom
                    CbType.IsEnabled = True
                    CbType.SelectedValue = NewDevice.Type
                    CbType_MouseLeave(CbType, Nothing)
                    flagnewdev = True
                End If

            Else 'Modification d'un Device

                FlagNewDevice = False
                x = myService.ReturnDeviceByID(IdSrv, DeviceId)

                If x IsNot Nothing Then 'on a trouvé le device

                    _Driver = myService.ReturnDriverByID(IdSrv, x.DriverID)
                    'ajout des drivers compatibles avec ce type de device au combo
                    For i As Integer = 0 To ListeDrivers.Count - 1
                        'pour chaque driver on regarde si le type est compatible
                        If ListeDrivers.Item(i).DeviceSupport.Count > 0 Then
                            For j As Integer = 0 To ListeDrivers.Item(i).DeviceSupport.Count - 1
                                If ListeDrivers.Item(i).DeviceSupport.Item(j).ToString = x.Type.ToString Then
                                    'on ajoute le drier a la liste si il est enable ou si il correspond à notre device
                                    If ListeDrivers.Item(i).Enable Or ListeDrivers.Item(i).Nom = _Driver.Nom Then CbDriver.Items.Add(ListeDrivers.Item(i).Nom)
                                    Exit For
                                End If
                            Next
                        End If
                    Next

                    'Affiche les propriétés du device
                    TxtNom.Text = x.Name
                    ChkEnable.IsChecked = x.Enable
                    ChKSolo.IsChecked = x.Solo
                    ChKLastEtat.IsChecked = x.LastEtat
                    ChKAllValue.IsChecked = x.AllValue
                    TxtUnit.Text = x.Unit
                    TxtPuissance.Text = x.Puissance
                    TxtDescript.Text = x.Description
                    If TxtPuissance.Text = "" Then TxtPuissance.Text = "0"
                    If _Driver IsNot Nothing Then
                        CbDriver.SelectedValue = _Driver.Nom
                    End If
                    CbType.SelectedValue = x.Type.ToString
                    CbType.IsEnabled = False
                    CbType.Foreground = Brushes.Black
                    TxtAdresse1.Text = x.Adresse1
                    'CbAdresse1.Text = x.Adresse1
                    TxtAdresse2.Text = x.Adresse2
                    CBModele.Text = x.Modele
                    TxtModele.Text = x.Modele
                    TxtRefresh.Text = x.Refresh
                    TxtLastChangeDuree.Text = x.LastChangeDuree
                    TxtID.Text = x.ID
                    _ListVar = x.VariablesOfDevice

                    Refresh_cbVar()

                    'affichage de l'image du composant
                    ImgDevice.Source = ConvertArrayToImage(myService.GetByteFromImage(x.Picture))
                    ImgDevice.Tag = x.Picture

                    'gestion des champs des composants avec VALUE INTEGER/DOUBLE/LONG
                    If AsProperty(x, "ValueMin") And AsProperty(x, "ValueMax") Then
                        StkValueMINMAX.Visibility = Windows.Visibility.Visible
                        TxtValueMin.Text = x.ValueMin
                        TxtValueMax.Text = x.ValueMax
                    Else : StkValueMINMAX.Visibility = Windows.Visibility.Collapsed
                    End If
                    If AsProperty(x, "ValueDef") And AsProperty(x, "precision") Then
                        StkValueDefaultPrecision.Visibility = Windows.Visibility.Visible
                        TxtValDef.Text = x.ValueDef
                        TxtPrecision.Text = x.Precision
                    Else : StkValueDefaultPrecision.Visibility = Windows.Visibility.Collapsed
                    End If
                    If AsProperty(x, "Correction") And AsProperty(x, "Formatage") Then
                        StkValueCorrectionFormatage.Visibility = Windows.Visibility.Visible
                        TxtCorrection.Text = x.Correction
                        TxtFormatage.Text = x.Formatage
                    Else : StkValueCorrectionFormatage.Visibility = Windows.Visibility.Collapsed
                    End If

                    BtnTest.Visibility = Windows.Visibility.Visible
                    StkModel.Visibility = Visibility.Visible
                    If x.Type = ListeDevices.MULTIMEDIA Then
                        ImgEditTemplate.Visibility = Windows.Visibility.Visible
                        LabelModele.Visibility = Windows.Visibility.Visible
                        LabelModele.Content = "Template"
                        TxtModele.Visibility = Windows.Visibility.Collapsed
                        CBModele.ItemsSource = myService.GetListOfTemplate
                        CBModele.DisplayMemberPath = "Name"
                        CBModele.Visibility = Windows.Visibility.Visible
                        Dim idx As Integer = 0
                        For Each itm In CBModele.Items
                            If itm.id = x.Modele Then
                                CBModele.SelectedIndex = idx
                                Exit For
                            End If
                            idx += 1
                        Next
                        StkDriver.Visibility = Windows.Visibility.Collapsed
                        LabelModele.ToolTip = "Template à utiliser"
                        CBModele.ToolTip = "Template à utiliser"
                        StkType.Margin = New Thickness(0)
                    End If

                    'on verifie si le device est un device systeme pour le rendre NON modifiable
                    If Left(x.Name, 5) = "HOMI_" Then
                        Label1.Content = "Composant SYSTEME"
                        TxtNom.IsReadOnly = True
                        TxtDescript.IsReadOnly = True
                        CbDriver.IsEditable = False
                        CbDriver.IsReadOnly = True
                        CbDriver.IsEnabled = False
                        CbDriver.Foreground = Brushes.Black
                        ChkEnable.Visibility = Windows.Visibility.Collapsed
                        ChKSolo.Visibility = Windows.Visibility.Collapsed
                        StkAdr1.Visibility = Windows.Visibility.Collapsed
                        StkAdr2.Visibility = Windows.Visibility.Collapsed
                        CBModele.Visibility = Windows.Visibility.Collapsed
                        StkLastChange.Visibility = Windows.Visibility.Collapsed
                        StkRefresh.Visibility = Windows.Visibility.Collapsed
                        BtnTest.Visibility = Windows.Visibility.Collapsed
                        LabelModele.Visibility = Windows.Visibility.Collapsed
                        StkUnit.Visibility = Windows.Visibility.Collapsed
                        StkPuiss.Visibility = Windows.Visibility.Collapsed
                    End If

                    'Affiche du bouton Historique, avec un tooltip si il y a des valeurs
                    BtnHisto.Visibility = Windows.Visibility.Visible
                    Dim tl As New ToolTip
                    tl.Foreground = System.Windows.Media.Brushes.White
                    tl.Background = System.Windows.Media.Brushes.WhiteSmoke
                    tl.BorderBrush = System.Windows.Media.Brushes.Black
                    Dim stkpopup As New StackPanel
                    Dim tool As New Label
                    Dim nbhisto As Double = myService.DeviceAsHisto(DeviceId)
                    If nbhisto > 0 Then
                        tool.Content &= "Derniere Valeur: " & x.Value & vbCrLf
                        tool.Content &= "Date MAJ: " & x.LastChange & vbCrLf
                        tool.Content &= "Nb Histo: " & nbhisto & vbCrLf
                    Else
                        BtnHisto.FontStyle = System.Windows.FontStyles.Italic
                        tool.Content = "Aucun Historique"
                    End If
                    stkpopup.Children.Add(tool)
                    tool = Nothing
                    tl.Content = stkpopup
                    stkpopup = Nothing
                    BtnHisto.ToolTip = tl
                End If
            End If

            'Liste toutes les zones dans la liste
            For i As Integer = 0 To myService.GetAllZones(IdSrv).Count - 1
                Dim ch1 As New CheckBox
                Dim ch2 As New CheckBox
                Dim ImgZone As New Image
                Dim stk As New StackPanel
                stk.Orientation = Orientation.Horizontal
                ImgZone.Width = 32
                ImgZone.Height = 32
                ImgZone.Margin = New Thickness(2)
                ImgZone.Source = ConvertArrayToImage(myService.GetByteFromImage(myService.GetAllZones(IdSrv).Item(i).Icon))
                ch1.Width = 80
                ch1.Content = myService.GetAllZones(IdSrv).Item(i).Name
                ch1.ToolTip = ch1.Content
                ch1.Uid = myService.GetAllZones(IdSrv).Item(i).ID
                AddHandler ch1.Click, AddressOf ChkElement_Click
                ch2.Content = "Visible"
                ch2.ToolTip = "Visible dans la zone côté client"
                ch2.Visibility = Windows.Visibility.Collapsed
                For j As Integer = 0 To myService.GetAllZones(IdSrv).Item(i).ListElement.Count - 1
                    If myService.GetAllZones(IdSrv).Item(i).ListElement.Item(j).ElementID = _DeviceId Then
                        ch1.IsChecked = True
                        ch2.Visibility = Windows.Visibility.Visible
                        If myService.GetAllZones(IdSrv).Item(i).ListElement.Item(j).Visible = True Then ch2.IsChecked = True
                        Exit For
                    End If
                Next
                stk.Children.Add(ImgZone)
                stk.Children.Add(ch1)
                stk.Children.Add(ch2)
                ListZone.Items.Add(stk)
            Next
        Catch Ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur: " & Ex.ToString, "Erreur", "")
        End Try
    End Sub

    Private Sub SaveInZone()
        Try
            For i As Integer = 0 To ListZone.Items.Count - 1
                Dim stk As StackPanel = ListZone.Items(i)
                Dim x1 As CheckBox = stk.Children.Item(1)
                Dim x2 As CheckBox = stk.Children.Item(2)
                Dim trv As Boolean = False

                For Each dev In myService.GetDeviceInZone(IdSrv, x1.Uid)
                    If dev IsNot Nothing Then
                        If dev = _DeviceId Then
                            trv = True
                            Exit For
                        End If
                    End If
                Next

                If trv = True And x1.IsChecked = False Then
                    myService.DeleteDeviceToZone(IdSrv, x1.Uid, _DeviceId)
                Else
                    If trv = True And x1.IsChecked = True Then
                    Else
                        If trv = False And x1.IsChecked = True Then myService.AddDeviceToZone(IdSrv, x1.Uid, _DeviceId, x2.IsChecked)
                    End If
                End If
            Next

        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur dans le programme SaveInZone: " & ex.ToString, "Admin", "")
        End Try

    End Sub

    'Modif des champs suivant le driver sélectionné
    Private Sub CbDriver_SelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles CbDriver.SelectionChanged
        MaJDriver()
    End Sub
    Private Sub MaJDriver()
        Try
            Dim _Driver As Object = Nothing
            Me.ForceCursor = True
            'on cherche le driver

            If CbDriver.SelectedItem IsNot Nothing Then
                For i As Integer = 0 To ListeDrivers.Count - 1
                    If ListeDrivers.Item(i).Nom = CbDriver.SelectedItem.ToString Then
                        _Driver = myService.ReturnDriverByID(IdSrv, ListeDrivers.Item(i).ID)
                        Exit For
                    End If
                Next
            End If

            'si on a trouvé le driver selectionné
            If _Driver IsNot Nothing Then
                'Si c'est un nouveau device, on peut modifier le type sinon non
                If FlagNewDevice Then
                    Dim mem As String = CbType.Text
                    CbType.IsEnabled = True
                    CbType.Items.Clear()

                    For j As Integer = 0 To _Driver.DeviceSupport.count - 1
                        CbType.Items.Add(_Driver.DeviceSupport.item(j).ToString)
                    Next
                    CbType.IsEnabled = True
                    CbType.Text = mem
                End If

                If _Driver.ID = "8B8EFC7E-F66D-11E1-AD40-71AF6188709B" And CbAdresse1.Tag <> "WEATHERMETEO" Then
                    CbAdresse1.Tag = "WEATHERMETEO"
                    CbAdresse1.Items.Clear()

                    Dim CityID As New Dictionary(Of String, String)
                    CityID = GetWeatherCityID()

                    CbAdresse1.ItemsSource = New Forms.BindingSource(CityID, Nothing)
                    CbAdresse1.DisplayMemberPath = "Value"
                    CbAdresse1.SelectedValuePath = "Key"

                    TxtAdresse1.Visibility = Windows.Visibility.Collapsed
                    CbAdresse1.Visibility = Windows.Visibility.Visible
                Else
                    CbAdresse1.Text = ""
                    CbAdresse1.ItemsSource = Nothing
                    CbAdresse1.Items.Clear()
                    CbAdresse1.Tag = Nothing
                    TxtAdresse1.Visibility = Windows.Visibility.Visible
                    CbAdresse1.Visibility = Windows.Visibility.Collapsed
                End If

                'Suivant le driver, on change les champs (personnalisation des Labels, affichage suivant @...
                If _Driver.LabelsDevice.Count > 0 Then
                    For k As Integer = 0 To _Driver.LabelsDevice.Count - 1
                        Select Case UCase(_Driver.LabelsDevice.Item(k).NomChamp)
                            Case "ADRESSE1"
                                If _Driver.LabelsDevice.Item(k).LabelChamp = "@" Then
                                    StkAdr1.Visibility = Windows.Visibility.Collapsed
                                Else
                                    LabelAdresse1.Content = _Driver.LabelsDevice.Item(k).LabelChamp
                                    If String.IsNullOrEmpty(_Driver.LabelsDevice.Item(k).Tooltip) = False Then
                                        LabelAdresse1.ToolTip = _Driver.LabelsDevice.Item(k).Tooltip
                                        TxtAdresse1.ToolTip = _Driver.LabelsDevice.Item(k).Tooltip
                                    End If
                                    StkAdr1.Visibility = Windows.Visibility.Visible
                                End If
                            Case "ADRESSE2"
                                If _Driver.LabelsDevice.Item(k).LabelChamp = "@" Then
                                    StkAdr2.Visibility = Windows.Visibility.Collapsed
                                Else
                                    LabelAdresse2.Content = _Driver.LabelsDevice.Item(k).LabelChamp
                                    If String.IsNullOrEmpty(_Driver.LabelsDevice.Item(k).Tooltip) = False Then
                                        LabelAdresse2.ToolTip = _Driver.LabelsDevice.Item(k).Tooltip
                                        TxtAdresse2.ToolTip = _Driver.LabelsDevice.Item(k).Tooltip
                                    End If
                                    StkAdr2.Visibility = Windows.Visibility.Visible
                                End If
                            Case "SOLO"
                                If _Driver.LabelsDevice.Item(k).LabelChamp = "@" Then
                                    ChKSolo.Visibility = Windows.Visibility.Collapsed
                                Else
                                    ChKSolo.ToolTip = _Driver.LabelsDevice.Item(k).Tooltip
                                    ChKSolo.Visibility = Windows.Visibility.Visible
                                End If
                            Case "REFRESH"
                                If _Driver.LabelsDevice.Item(k).LabelChamp = "@" Then
                                    StkRefresh.Visibility = Windows.Visibility.Collapsed
                                Else
                                    LabelRefresh.Content = _Driver.LabelsDevice.Item(k).LabelChamp
                                    If String.IsNullOrEmpty(_Driver.LabelsDevice.Item(k).Tooltip) = False Then
                                        LabelRefresh.ToolTip = _Driver.LabelsDevice.Item(k).Tooltip
                                        TxtRefresh.ToolTip = _Driver.LabelsDevice.Item(k).Tooltip
                                    End If
                                    StkRefresh.Visibility = Windows.Visibility.Visible
                                End If
                            Case "LASTCHANGEDUREE"
                                If _Driver.LabelsDevice.Item(k).LabelChamp = "@" Then
                                    StkLastChange.Visibility = Windows.Visibility.Collapsed
                                Else
                                    LabelLastChangeDuree.Content = _Driver.LabelsDevice.Item(k).LabelChamp
                                    If String.IsNullOrEmpty(_Driver.LabelsDevice.Item(k).Tooltip) = True Then
                                        TxtLastChangeDuree.ToolTip = "Permet de vérifier si le composant a été mis à jour depuis moins de x minutes sinon il apparait en erreur"
                                        LabelLastChangeDuree.ToolTip = "Permet de vérifier si le composant a été mis à jour depuis moins de x minutes sinon il apparait en erreur"
                                    Else
                                        TxtLastChangeDuree.ToolTip = _Driver.LabelsDevice.Item(k).Tooltip
                                        LabelLastChangeDuree.ToolTip = _Driver.LabelsDevice.Item(k).Tooltip
                                    End If
                                    StkLastChange.Visibility = Windows.Visibility.Visible
                                End If
                            Case "MODELE"
                                If _Driver.LabelsDevice.Item(k).LabelChamp = "@" Then
                                    CBModele.Visibility = Windows.Visibility.Collapsed
                                    CBModele.Tag = 0
                                    TxtModele.Visibility = Windows.Visibility.Collapsed
                                    TxtModele.Tag = 0
                                    LabelModele.Visibility = Windows.Visibility.Collapsed
                                    StkModel.Visibility = Windows.Visibility.Collapsed
                                Else
                                    LabelModele.Visibility = Windows.Visibility.Visible
                                    StkModel.Visibility = Windows.Visibility.Visible
                                    If String.IsNullOrEmpty(_Driver.LabelsDevice.Item(k).LabelChamp) = False Then LabelModele.Content = _Driver.LabelsDevice.Item(k).LabelChamp
                                    If String.IsNullOrEmpty(_Driver.LabelsDevice.Item(k).Tooltip) = False Then
                                        LabelModele.ToolTip = _Driver.LabelsDevice.Item(k).ToolTip
                                        CBModele.ToolTip = _Driver.LabelsDevice.Item(k).ToolTip
                                    End If
                                    If String.IsNullOrEmpty(_Driver.LabelsDevice.Item(k).Parametre) = False Then
                                        CBModele.Items.Clear()
                                        Dim a() As String = _Driver.LabelsDevice.Item(k).Parametre.Split("|")
                                        If a.Length > 0 Then
                                            For g As Integer = 0 To a.Length - 1
                                                CBModele.Items.Add(a(g))
                                            Next
                                            CBModele.IsEditable = False
                                            CBModele.Visibility = Windows.Visibility.Visible
                                            CBModele.Tag = 1
                                            TxtModele.Visibility = Windows.Visibility.Collapsed
                                            TxtModele.Tag = 0
                                        Else
                                            CBModele.Visibility = Windows.Visibility.Collapsed
                                            CBModele.Tag = 0
                                            TxtModele.Visibility = Windows.Visibility.Visible
                                            TxtModele.Tag = 1
                                            TxtModele.Text = a(0)
                                        End If
                                    Else
                                        CBModele.Visibility = Windows.Visibility.Collapsed
                                        CBModele.Tag = 0
                                        TxtModele.Visibility = Windows.Visibility.Visible
                                        TxtModele.Tag = 1
                                    End If
                                End If
                        End Select
                    Next
                End If
            End If
            Me.Cursor = Nothing
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur: " & ex.ToString, "Erreur", "")
        End Try
    End Sub

    'Verification du driver suivant son ID
    Private Sub VerifDriver(ByVal IdDriver As String)
        Try
            Dim x As TemplateDriver = myService.ReturnDriverByID(IdSrv, IdDriver)
            If x IsNot Nothing Then
                If x.Enable = False Then
                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.INFO, "Le driver " & x.Nom & " n'est pas activé (Enable), le composant ne pourra pas être utilisé!", "INFO", "")
                    Exit Sub
                End If
                If x.IsConnect = False Then AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.INFO, "Le driver " & x.Nom & " n'est pas démarré, le composant ne pourra pas être utilisé!", "INFO", "")
            Else
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le driver n'a pas pu être trouvé ! (ID du driver: " & IdDriver & ")", "ERREUR", "")
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR VerifDriver: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    'Quand on change le TYPE d'un composant
    Private Sub CbType_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles CbType.MouseLeave
        Try
            If _Action = EAction.Nouveau Then
                'Gestion si Device avec Value
                If CbType.SelectedValue Is Nothing Then Exit Sub
                If CbType.Text = "BAROMETRE" _
                    Or CbType.Text = "COMPTEUR" _
                    Or CbType.Text = "ENERGIEINSTANTANEE" _
                    Or CbType.Text = "ENERGIETOTALE" _
                    Or CbType.Text = "GENERIQUEVALUE" _
                    Or CbType.Text = "HUMIDITE" _
                    Or CbType.Text = "LAMPE" _
                    Or CbType.Text = "PLUIECOURANT" _
                    Or CbType.Text = "PLUIETOTAL" _
                    Or CbType.Text = "TEMPERATURE" _
                    Or CbType.Text = "TEMPERATURECONSIGNE" _
                    Or CbType.Text = "VITESSEVENT" _
                    Or CbType.Text = "UV" _
                    Or CbType.Text = "VOLET" _
                    Then
                    StkValueMINMAX.Visibility = Windows.Visibility.Visible
                    StkValueDefaultPrecision.Visibility = Windows.Visibility.Visible
                    StkValueCorrectionFormatage.Visibility = Windows.Visibility.Visible

                    'gestion des champs valuemin/max/defaut suivant le type
                    If CbType.Text = "HUMIDITE" Or CbType.Text = "LAMPE" Or CbType.Text = "VOLET" Then
                        TxtValueMin.Text = "0"
                        TxtValueMax.Text = "100"
                        TxtValDef.Text = "0"
                        TxtCorrection.Text = "0"
                        TxtPrecision.Text = "0"
                    ElseIf CbType.Text = "BAROMETRE" _
                        Or CbType.Text = "COMPTEUR" _
                        Or CbType.Text = "ENERGIEINSTANTANEE" _
                        Or CbType.Text = "ENERGIETOTALE" _
                        Or CbType.Text = "GENERIQUEVALUE" _
                        Or CbType.Text = "PLUIECOURANT" _
                        Or CbType.Text = "PLUIETOTAL" _
                        Or CbType.Text = "VITESSEVENT" _
                        Or CbType.Text = "UV" Then
                        TxtValueMin.Text = "0"
                        TxtValueMax.Text = "9999999"
                        TxtValDef.Text = "0"
                        TxtCorrection.Text = "0"
                        TxtPrecision.Text = "0"
                    ElseIf CbType.Text = "TEMPERATURE" _
                        Or CbType.Text = "TEMPERATURECONSIGNE" _
                        Then
                        TxtValueMin.Text = "-99"
                        TxtValueMax.Text = "9999"
                        TxtValDef.Text = "0"
                        TxtCorrection.Text = "0"
                        TxtPrecision.Text = "0"
                    End If
                Else
                    StkValueMINMAX.Visibility = Windows.Visibility.Collapsed
                    StkValueDefaultPrecision.Visibility = Windows.Visibility.Collapsed
                    StkValueCorrectionFormatage.Visibility = Windows.Visibility.Collapsed
                End If

                'Gestion AllValue et LastEtat par défaut
                If CbType.Text = "BATTERIE" Or CbType.Text = "FREEBOX" Or CbType.Text = "GENERIQUESTRING" Or CbType.Text = "MULTIMEDIA" Or CbType.Text = "SWITCH" Or CbType.Text = "TELECOMMANDE" Then
                    ChKLastEtat.IsChecked = False
                    ChKAllValue.IsChecked = True
                Else
                    ChKLastEtat.IsChecked = False
                    ChKAllValue.IsChecked = False
                End If

                StkModel.Visibility = Windows.Visibility.Visible
                If CbType.SelectedValue = "MULTIMEDIA" Then
                    StkDriver.Visibility = Windows.Visibility.Collapsed

                    LabelModele.Content = "Template"
                    LabelModele.ToolTip = "Template à utiliser"
                    CBModele.ToolTip = "Template à utiliser"
                    TxtModele.Visibility = Windows.Visibility.Collapsed
                    CBModele.ItemsSource = myService.GetListOfTemplate
                    CBModele.DisplayMemberPath = "Name"
                    CBModele.Visibility = Windows.Visibility.Visible
                    StkType.Margin = New Thickness(0)
                End If
            End If
        Catch Ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur lors du changement de type: " & Ex.Message, "Erreur", "")
        End Try
    End Sub

    Private Sub TxtRefresh_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtRefresh.TextChanged
        Try
            If String.IsNullOrEmpty(TxtRefresh.Text) = False And IsNumeric(TxtRefresh.Text) = False Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Veuillez saisir une valeur numérique")
                TxtRefresh.Text = 0
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uDevice TxtRefresh_TextChanged: " & ex.Message, "ERREUR", "")
        End Try
    End Sub

    Private Sub TxtLastChangeDuree_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtLastChangeDuree.TextChanged
        Try
            If String.IsNullOrEmpty(TxtLastChangeDuree.Text) = False And IsNumeric(TxtLastChangeDuree.Text) = False Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Veuillez saisir une valeur numérique")
                TxtLastChangeDuree.Text = 0
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uDevice TxtLastChangeDuree_TextChanged: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Sub TxtPuissance_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtPuissance.TextChanged
        If IsNumeric(TxtPuissance.Text) = False Then
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Veuillez saisir une valeur numérique !!", "ERREUR", "")
            TxtPuissance.Undo()
        End If
    End Sub

    Private Sub ImgDevice_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ImgDevice.MouseLeftButtonDown
        Try
            Dim frm As New WindowImg
            frm.ShowDialog()
            If frm.DialogResult.HasValue And frm.DialogResult.Value Then
                Dim retour As String = frm.FileName
                If String.IsNullOrEmpty(retour) = False Then
                    ImgDevice.Source = ConvertArrayToImage(myService.GetByteFromImage(retour))
                    ImgDevice.Tag = retour
                End If
                frm.Close()
            Else
                frm.Close()
            End If
            frm = Nothing
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub ImgDevice_MouseLeftButtonDown: " & ex.Message, "ERREUR", "")
        End Try
    End Sub

    'Gestion des zones
    Private Sub ChkElement_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        Try
            For Each stk As StackPanel In ListZone.Items
                Dim x As CheckBox = stk.Children.Item(1)
                If x.IsChecked = True Then
                    stk.Children.Item(2).Visibility = Windows.Visibility.Visible
                Else
                    stk.Children.Item(2).Visibility = Windows.Visibility.Collapsed
                End If
            Next
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uDevice ChkElement_Click: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Sub UnloadControl(ByVal MyControl As Object)
        Try
            For i As Integer = 0 To Window1.CanvasUser.Children.Count - 1
                If Window1.CanvasUser.Children.Item(i).Uid = MyControl.uid Then
                    Window1.CanvasUser.Children.RemoveAt(i)
                    Exit Sub
                End If
            Next
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uDevice UnloadControl: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

#Region "Gestion des BOUTONS"

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        flagnewdev = False
        NewDevice = Nothing
        RaiseEvent CloseMe(Me, True)
    End Sub

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        Try
            Dim retour As String = ""
            Dim _driverid As String = ""

            'on recupere le DriverID depuis le combobox
            For i As Integer = 0 To myService.GetAllDrivers(IdSrv).Count - 1
                If myService.GetAllDrivers(IdSrv).Item(i).Nom = CbDriver.Text Then
                    _driverid = myService.GetAllDrivers(IdSrv).Item(i).ID
                    Exit For
                End If
            Next

            'on corrige certains valeurs
            'TxtRefresh.Text = Replace(TxtRefresh.Text, ".", ",")
            TxtRefresh.Text = Regex.Replace(TxtRefresh.Text, "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
            'TxtRefresh.Text = Replace(TxtRefresh.Text, ",", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
            TxtPrecision.Text = Regex.Replace(TxtPrecision.Text, "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
            'TxtPrecision.Text = Replace(TxtPrecision.Text, ",", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
            TxtCorrection.Text = Regex.Replace(TxtCorrection.Text, "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
            'TxtCorrection.Text = Replace(TxtCorrection.Text, ",", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
            TxtValDef.Text = Regex.Replace(TxtValDef.Text, "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
            'TxtValDef.Text = Replace(TxtValDef.Text, ",", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
            TxtValueMax.Text = Regex.Replace(TxtValueMax.Text, "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
            'TxtValueMax.Text = Replace(TxtValueMax.Text, ",", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
            TxtValueMin.Text = Regex.Replace(TxtValueMin.Text, "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
            'TxtValueMin.Text = Replace(TxtValueMin.Text, ",", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)

            'on check les valeurs renseignés
            If Left(TxtNom.Text, 5) <> "HOMI_" Then
                If (String.IsNullOrEmpty(TxtNom.Text) Or HaveCaractSpecial(TxtNom.Text)) Then
                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le nom du composant doit être renseigné et ne doit pas comporter de caractère spécial", "Erreur", "")
                    Exit Sub
                End If
            End If
            If IsNumeric(TxtPrecision.Text) = False Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le champ Précision doit être un Nombre", "Erreur", "")
                Exit Sub
            End If
            ' Le champ correction peut contenir des symboles mathematiques (*/+-)
            'If IsNumeric(TxtCorrection.Text) = False Then
            'AfficheMessageAndLog (HoMIDom.HoMIDom.Server.TypeLog.ERREUR,"Le champ Correction doit être un Nombre", "Erreur","")
            '  Exit Sub
            ' End If
            If IsNumeric(TxtValDef.Text) = False Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le champ Valeur Defaut doit être un Nombre", "Erreur", "")
                Exit Sub
            End If
            If IsNumeric(TxtValueMax.Text) = False Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le champ Valeur Max doit être un Nombre", "Erreur", "")
                Exit Sub
            End If
            If IsNumeric(TxtValueMin.Text) = False Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le champ Valeur Min doit être un Nombre", "Erreur", "")
                Exit Sub
            End If
            If String.IsNullOrEmpty(TxtNom.Text) = True Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le nom du composant est obligatoire !!", "Erreur", "")
                Exit Sub
            End If
            If String.IsNullOrEmpty(CbType.Text) = True Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le type du composant est obligatoire !!", "Erreur", "")
                Exit Sub
            End If
            If String.IsNullOrEmpty(CbDriver.Text) = True Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le driver du composant est obligatoire !!", "Erreur", "")
                Exit Sub
            End If
            If String.IsNullOrEmpty(TxtAdresse1.Text) = True Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "L'adresse de base du composant est obligatoire !!", "Erreur", "")
                Exit Sub
            End If
            retour = myService.VerifChamp(IdSrv, _driverid, "ADRESSE1", TxtAdresse1.Text)
            If retour <> "0" Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Champ " & LabelAdresse1.Content & ": " & retour, "Erreur", "")
                Exit Sub
            End If
            retour = myService.VerifChamp(IdSrv, _driverid, "ADRESSE2", TxtAdresse2.Text)
            If retour <> "0" Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Champ " & LabelAdresse2.Content & ": " & retour, "Erreur", "")
                Exit Sub
            End If

            'on recupere le bon champ Modele : Combobox ou texte
            Dim _modele As String = ""
            If CBModele.Tag = 1 Then
                _modele = CBModele.Text
            Else
                If TxtModele.Tag = 1 Then
                    _modele = TxtModele.Text
                Else
                    _modele = ""
                End If
            End If

            'on sauvegarde le composant
            If CbType.Text = "MULTIMEDIA" Then
                If CBModele.SelectedItem IsNot Nothing Then
                    _modele = CBModele.SelectedItem.ID
                Else
                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Veuillez sélectionner ou ajouter un template au device!", "Erreur", "")
                    Exit Sub
                End If

                'If x IsNot Nothing Then
                '    If String.IsNullOrEmpty(x.Modele) = True Then

                '    End If
                '    _modele = x.Modele
                'End If
                If _Action = EAction.Modifier Then
                    If x IsNot Nothing Then retour = myService.SaveDevice(IdSrv, _DeviceId, TxtNom.Text, TxtAdresse1.Text, ChkEnable.IsChecked, ChKSolo.IsChecked, _driverid, CbType.Text, TxtRefresh.Text, TxtAdresse2.Text, ImgDevice.Tag, _modele, TxtDescript.Text, TxtLastChangeDuree.Text, ChKLastEtat.IsChecked, TxtCorrection.Text, TxtFormatage.Text, TxtPrecision.Text, TxtValueMax.Text, TxtValueMin.Text, TxtValDef.Text, x.Commandes, TxtUnit.Text, TxtPuissance.Text, ChKAllValue.IsChecked, _ListVar)
                Else
                    retour = myService.SaveDevice(IdSrv, _DeviceId, TxtNom.Text, TxtAdresse1.Text, ChkEnable.IsChecked, ChKSolo.IsChecked, _driverid, CbType.Text, TxtRefresh.Text, TxtAdresse2.Text, ImgDevice.Tag, _modele, TxtDescript.Text, TxtLastChangeDuree.Text, ChKLastEtat.IsChecked, TxtCorrection.Text, TxtFormatage.Text, TxtPrecision.Text, TxtValueMax.Text, TxtValueMin.Text, TxtValDef.Text, Nothing, TxtUnit.Text, TxtPuissance.Text, ChKAllValue.IsChecked, _ListVar)
                End If
            Else
                retour = myService.SaveDevice(IdSrv, _DeviceId, TxtNom.Text, TxtAdresse1.Text, ChkEnable.IsChecked, ChKSolo.IsChecked, _driverid, CbType.Text, TxtRefresh.Text, TxtAdresse2.Text, ImgDevice.Tag, _modele, TxtDescript.Text, TxtLastChangeDuree.Text, ChKLastEtat.IsChecked, TxtCorrection.Text, TxtFormatage.Text, TxtPrecision.Text, TxtValueMax.Text, TxtValueMin.Text, TxtValDef.Text, Nothing, TxtUnit.Text, TxtPuissance.Text, ChKAllValue.IsChecked, _ListVar)
            End If

            If retour = "98" Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le nom du device: " & TxtNom.Text & " existe déjà impossible de l'enregister", "ERREUR", "")
                Exit Sub
            End If

            'on affiche l'ID du composant (si c'était un nouveau composant, il n'y avait pas encore d'ID)
            TxtID.Text = retour

            VerifDriver(_driverid)
            If String.IsNullOrEmpty(_DeviceId) = True Then _DeviceId = retour
            SaveInZone()
            FlagChange = True

            If _Action = EAction.Nouveau And NewDevice IsNot Nothing And flagnewdev Then
                myService.DeleteNewDevice(IdSrv, NewDevice.ID)
                NewDevice = Nothing
                flagnewdev = False
            End If

            RaiseEvent CloseMe(Me, False)
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uDevice BtnOK_Click: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Sub BtnTest_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnTest.Click
        Try
            If myService.ReturnDeviceByID(IdSrv, _DeviceId).Enable = False Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Vous ne pouvez pas exécuter de commandes car le device n'est pas activé (propriété Enable)!", "Erreur", "")
                Exit Sub
            End If

            Dim y As New uTestDevice(_DeviceId)
            y.Uid = System.Guid.NewGuid.ToString()
            AddHandler y.CloseMe, AddressOf UnloadControl
            Window1.CanvasUser.Children.Add(y)
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur Tester: " & ex.Message, "Erreur", "")
        End Try
    End Sub

    Private Sub BtnSave_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnSave.Click
        Try
            Dim retour As String = ""
            Dim _driverid As String = ""

            'on recupere le DriverID depuis le combobox
            For i As Integer = 0 To myService.GetAllDrivers(IdSrv).Count - 1
                If myService.GetAllDrivers(IdSrv).Item(i).Nom = CbDriver.Text Then
                    _driverid = myService.GetAllDrivers(IdSrv).Item(i).ID
                    Exit For
                End If
            Next

            'on corrige certains valeurs
            'TxtRefresh.Text = Replace(TxtRefresh.Text, ".", ",")
            TxtRefresh.Text = Regex.Replace(TxtRefresh.Text, "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
            'TxtRefresh.Text = Replace(TxtRefresh.Text, ",", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
            TxtPrecision.Text = Regex.Replace(TxtPrecision.Text, "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
            'TxtPrecision.Text = Replace(TxtPrecision.Text, ",", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
            TxtCorrection.Text = Regex.Replace(TxtCorrection.Text, "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
            'TxtCorrection.Text = Replace(TxtCorrection.Text, ",", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
            TxtValDef.Text = Regex.Replace(TxtValDef.Text, "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
            'TxtValDef.Text = Replace(TxtValDef.Text, ",", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
            TxtValueMax.Text = Regex.Replace(TxtValueMax.Text, "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
            'TxtValueMax.Text = Replace(TxtValueMax.Text, ",", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
            TxtValueMin.Text = Regex.Replace(TxtValueMin.Text, "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
            'TxtValueMin.Text = Replace(TxtValueMin.Text, ",", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)

            'on check les valeurs renseignés
            If CbType.Text = "BAROMETRE" _
                Or CbType.Text = "COMPTEUR" _
                Or CbType.Text = "ENERGIEINSTANTANEE" _
                Or CbType.Text = "ENERGIETOTALE" _
                Or CbType.Text = "GENERIQUEVALUE" _
                Or CbType.Text = "HUMIDITE" _
                Or CbType.Text = "LAMPE" _
                Or CbType.Text = "PLUIECOURANT" _
                Or CbType.Text = "PLUIETOTAL" _
                Or CbType.Text = "TEMPERATURE" _
                Or CbType.Text = "TEMPERATURECONSIGNE" _
                Or CbType.Text = "VITESSEVENT" _
                Or CbType.Text = "UV" _
                Or CbType.Text = "VOLET" _
                Then
                If IsNumeric(TxtPrecision.Text) = False Then
                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le champ Précision doit être un Nombre", "Erreur", "")
                    Exit Sub
                End If
                ' Le champ correction peut contenir des symboles mathematiques (*/+-)
                'If IsNumeric(TxtCorrection.Text) = False Then
                '    AfficheMessageAndLog (HoMIDom.HoMIDom.Server.TypeLog.ERREUR,"Le champ Correction doit être un Nombre", "Erreur","")
                '    Exit Sub
                'End If
                If IsNumeric(TxtValDef.Text) = False Then
                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le champ Valeur Defaut doit être un Nombre", "Erreur", "")
                    Exit Sub
                End If
                If IsNumeric(TxtValueMax.Text) = False Then
                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le champ Valeur Max doit être un Nombre", "Erreur", "")
                    Exit Sub
                End If
                If IsNumeric(TxtValueMin.Text) = False Then
                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le champ Valeur Min doit être un Nombre", "Erreur", "")
                    Exit Sub
                End If
            End If
            If Left(TxtNom.Text, 5) <> "HOMI_" Then
                If (String.IsNullOrEmpty(TxtNom.Text) Or HaveCaractSpecial(TxtNom.Text)) Then
                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le nom du composant doit être renseigné et ne doit pas comporter de caractère spécial", "Erreur", "")
                    Exit Sub
                End If
            End If
            If String.IsNullOrEmpty(CbType.Text) = True Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le type du device est obligatoire !!", "Erreur", "")
                Exit Sub
            End If
            If String.IsNullOrEmpty(CbDriver.Text) = True Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le driver du device est obligatoire !!", "Erreur", "")
                Exit Sub
            End If
            If String.IsNullOrEmpty(TxtAdresse1.Text) = True Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "L'adresse de base du device est obligatoire !!", "Erreur", "")
                Exit Sub
            End If

            'on recupere le bon champ Modele : Combobox ou texte
            Dim _modele As String
            If CBModele.Tag = 1 Then
                _modele = CBModele.Text
            Else
                If TxtModele.Tag = 1 Then
                    _modele = TxtModele.Text
                Else
                    _modele = ""
                End If
            End If

            'on sauvegarde le composant
            If CbType.Text = "MULTIMEDIA" Then
                If CBModele.SelectedItem IsNot Nothing Then
                    _modele = CBModele.SelectedItem.ID
                Else
                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Veuillez sélectionner ou ajouter un template au device!", "Erreur", "")
                    Exit Sub
                End If
                If _Action = EAction.Modifier Then
                    If x IsNot Nothing Then retour = myService.SaveDevice(IdSrv, _DeviceId, TxtNom.Text, TxtAdresse1.Text, ChkEnable.IsChecked, ChKSolo.IsChecked, _driverid, CbType.Text, TxtRefresh.Text, TxtAdresse2.Text, ImgDevice.Tag, _modele, TxtDescript.Text, TxtLastChangeDuree.Text, ChKLastEtat.IsChecked, TxtCorrection.Text, TxtFormatage.Text, TxtPrecision.Text, TxtValueMax.Text, TxtValueMin.Text, TxtValDef.Text, x.Commandes, TxtUnit.Text, TxtPuissance.Text, ChKAllValue.IsChecked, _ListVar)
                Else
                    retour = myService.SaveDevice(IdSrv, _DeviceId, TxtNom.Text, TxtAdresse1.Text, ChkEnable.IsChecked, ChKSolo.IsChecked, _driverid, CbType.Text, TxtRefresh.Text, TxtAdresse2.Text, ImgDevice.Tag, _modele, TxtDescript.Text, TxtLastChangeDuree.Text, ChKLastEtat.IsChecked, TxtCorrection.Text, TxtFormatage.Text, TxtPrecision.Text, TxtValueMax.Text, TxtValueMin.Text, TxtValDef.Text, Nothing, TxtUnit.Text, TxtPuissance.Text, ChKAllValue.IsChecked, _ListVar)
                End If
            Else
                retour = myService.SaveDevice(IdSrv, _DeviceId, TxtNom.Text, TxtAdresse1.Text, ChkEnable.IsChecked, ChKSolo.IsChecked, _driverid, CbType.Text, TxtRefresh.Text, TxtAdresse2.Text, ImgDevice.Tag, _modele, TxtDescript.Text, TxtLastChangeDuree.Text, ChKLastEtat.IsChecked, TxtCorrection.Text, TxtFormatage.Text, TxtPrecision.Text, TxtValueMax.Text, TxtValueMin.Text, TxtValDef.Text, Nothing, TxtUnit.Text, TxtPuissance.Text, ChKAllValue.IsChecked, _ListVar)
            End If
            If retour = "98" Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le nom du device: " & TxtNom.Text & " existe déjà impossible de l'enregister", "ERREUR", "")
                Exit Sub
            End If

            'on affiche l'ID du composant (si c'était un nouveau composant, il n'y avait pas encore d'ID)
            TxtID.Text = retour

            VerifDriver(_driverid)
            If String.IsNullOrEmpty(_DeviceId) = True Then _DeviceId = retour
            SaveInZone()
            FlagChange = True

            If _Action = EAction.Nouveau And NewDevice IsNot Nothing And flagnewdev Then
                myService.DeleteNewDevice(IdSrv, NewDevice.ID)
                NewDevice = Nothing
                flagnewdev = False
            End If

            'Dim uid As String = myService.SaveDevice(IdSrv, _DeviceId, TxtNom.Text, TxtAdresse1.Text, ChkEnable.IsChecked, ChKSolo.IsChecked, _driverid, CbType.Text, TxtRefresh.Text, TxtAdresse2.Text, ImgDevice.Tag, CBModele.Text, TxtDescript.Text, TxtLastChangeDuree.Text)

            BtnTest.Visibility = Windows.Visibility.Visible
            BtnHisto.Visibility = Windows.Visibility.Visible
            If CbType.SelectedValue = "MULTIMEDIA" Then
                BtnEditTel.Visibility = Windows.Visibility.Visible
                TxtModele.Visibility = Visibility.Hidden
                LabelModele.Visibility = Windows.Visibility.Hidden
            End If

            If _DeviceId.Length > 3 Then x = myService.ReturnDeviceByID(IdSrv, _DeviceId)
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uDevice BtnSave_Click: " & ex.Message, "ERREUR", "")
        End Try
    End Sub

    'Private Sub BtnEditTel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnEditTel.Click
    '    Try
    '        Dim _driverid As String = ""
    '        For i As Integer = 0 To myService.GetAllDrivers(IdSrv).Count - 1
    '            If myService.GetAllDrivers(IdSrv).Item(i).Nom = CbDriver.Text Then
    '                _driverid = myService.GetAllDrivers(IdSrv).Item(i).ID
    '                Exit For
    '            End If
    '        Next

    '        Dim frm As New WTelecommande(_DeviceId, _driverid, x)
    '        frm.ShowDialog()
    '        If frm.DialogResult.HasValue And frm.DialogResult.Value Then
    '            If x IsNot Nothing Then
    '                If String.IsNullOrEmpty(x.Modele) = False Then 'On vérifie si on viens de changer de template
    '                    'If x.Commandes.Count = 0 The
    '                    'BtnEditTel.Visibility = Windows.Visibility.Collapsed
    '                End If
    '                frm.Close()
    '            End If
    '        Else
    '            frm.Close()
    '        End If
    '    Catch ex As Exception
    '        AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR BtnEditTel_MouseDown: " & ex.ToString, "ERREUR", "")
    '    End Try
    'End Sub

    Private Sub BtnHisto_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnHisto.Click
        Try
            If IsConnect = False Then
                Exit Sub
            End If

            Me.Cursor = Cursors.Wait

            If myService.DeviceAsHisto(_DeviceId, "Value") Then
                Dim Devices As New List(Of Dictionary(Of String, String))
                Dim y As New Dictionary(Of String, String)
                y.Add(_DeviceId, "Value")
                Devices.Add(y)

                Dim x As New uHisto(Devices)
                x.Uid = System.Guid.NewGuid.ToString()
                x.Width = Window1.CanvasUser.ActualWidth - 20
                x.Height = Window1.CanvasUser.ActualHeight - 20
                x._with = Window1.CanvasUser.ActualHeight - 20
                AddHandler x.CloseMe, AddressOf UnloadControl
                Window1.CanvasUser.Children.Add(x)

            End If
            Me.Cursor = Nothing
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur lors de la génération du relevé: " & ex.ToString, "Erreur Admin", "")
        End Try
    End Sub

#End Region

    'gestion des options non compatibles entre elle
    Private Sub ChKAllValue_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ChKAllValue.Click
        If ChKAllValue.IsChecked Then ChKLastEtat.IsChecked = False
    End Sub
    Private Sub ChKLastEtat_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ChKLastEtat.Click
        If ChKLastEtat.IsChecked Then ChKAllValue.IsChecked = False
    End Sub

    Private Sub ImgEditTemplate_MouseDown(sender As System.Object, e As System.Windows.Input.MouseButtonEventArgs) Handles ImgEditTemplate.MouseDown
        Try
            Dim frm As New WTelecommandeNew(CBModele.Text)
            frm.ShowDialog()
            If frm.DialogResult.HasValue And frm.DialogResult.Value Then
                frm.Close()

                CBModele.ItemsSource = myService.GetListOfTemplate
                CBModele.DisplayMemberPath = "Name"
                CBModele.Visibility = Windows.Visibility.Visible

                Dim idx As Integer = 0
                For Each itm In CBModele.Items
                    If itm.id = x.Modele Then
                        CBModele.SelectedIndex = idx
                        Exit For
                    End If
                    idx += 1
                Next
            Else
                frm.Close()
            End If

        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur ImgEditTemplate_MouseDown: " & ex.ToString, "Erreur Admin", "ImgEditTemplate_MouseDown")
        End Try
    End Sub

    Private Sub CBModele_SelectionChanged(sender As Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles CBModele.SelectionChanged
        If CbType.SelectedValue = "MULTIMEDIA" Then
            If CBModele.SelectedItem IsNot Nothing Then
                Dim selecttemplate As HoMIDom.HoMIDom.Telecommande.Template = CBModele.SelectedItem

                Select Case selecttemplate.Type
                    Case 0 'http
                        CbDriver.SelectedValue = "HTTP"
                        LabelAdresse1.Content = "Adresse IP"
                        If String.IsNullOrEmpty(TxtAdresse1.Text) Then TxtAdresse1.Text = "localhost"
                        LabelAdresse2.Content = "Port IP"
                    Case 1 'IR
                        CbDriver.SelectedValue = "USBuirt"
                        StkAdr1.Visibility = Windows.Visibility.Collapsed
                        StkAdr2.Visibility = Windows.Visibility.Collapsed
                    Case 2 'RS232
                        CbDriver.SelectedValue = "RS232"
                        LabelAdresse1.Content = "Port COM"
                        If My.Computer.Ports.SerialPortNames.Count > 0 Then
                            If String.IsNullOrEmpty(TxtAdresse1.Text) Then TxtAdresse1.Text = My.Computer.Ports.SerialPortNames.Item(0)
                        Else
                            TxtAdresse1.Text = "Aucun port RS232 disponible !!"
                        End If
                        LabelAdresse2.Content = "Paramètres"
                        TxtAdresse2.Text = "9600,0,8,1"
                End Select
            End If
        End If
    End Sub

    Private Sub CbAdresse1_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles CbAdresse1.SelectionChanged
        If CbAdresse1.Tag = "WEATHERMETEO" Then
            TxtAdresse1.Text = CbAdresse1.SelectedValue
        End If
    End Sub

    Private Sub TxtAdresse1_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs) Handles TxtAdresse1.TextChanged
        If CbAdresse1.Tag = "WEATHERMETEO" Then
            Dim flagTrouv As Boolean = False
            Dim idx As Integer = 0

            For Each item In CbAdresse1.Items
                If TxtAdresse1.Text.ToUpper.Trim = item.key.ToString.ToUpper.Trim Then
                    flagTrouv = True
                    Exit For
                End If
                idx += 1
            Next

            If flagTrouv Then
                CbAdresse1.SelectedIndex = idx
            Else
                CbAdresse1.Text = TxtAdresse1.Text
            End If
        End If
    End Sub


#Region "Variables"

    ''' <summary>
    ''' bouton nouvelle variable
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BtnNewVar_MouseDown(sender As System.Object, e As System.Windows.Input.MouseButtonEventArgs) Handles BtnNewVar.MouseDown
        Try
            TxtVarName.Text = ""
            TxtVarName.IsReadOnly = False
            TxtVarValue.Text = ""
            BtnNewVar.Visibility = Windows.Visibility.Collapsed
            BtnDelVar.Visibility = Windows.Visibility.Collapsed
            BtnApplyVar.Visibility = Windows.Visibility.Visible
            StkVar.Visibility = Windows.Visibility.Visible
            BtnNewVar.Tag = 1 '0=Modifier 1=New
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur BtnNewVar_MouseDown: " & ex.ToString, "Erreur Admin", "BtnNewVar_MouseDown")
        End Try
    End Sub

    ''' <summary>
    ''' Bouton supprimer variable
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BtnDelVar_MouseDown(sender As System.Object, e As System.Windows.Input.MouseButtonEventArgs) Handles BtnDelVar.MouseDown
        Try
            If CbVariables.SelectedIndex >= 0 Then
                If _ListVar.ContainsKey(CbVariables.SelectedItem.key) Then
                    _ListVar.Remove(CbVariables.SelectedItem.key)
                    Refresh_cbVar()
                    StkVar.Visibility = Windows.Visibility.Collapsed
                    BtnApplyVar.Visibility = Windows.Visibility.Collapsed
                    BtnNewVar.Visibility = Windows.Visibility.Visible
                    BtnDelVar.Visibility = Windows.Visibility.Visible
                    BtnNewVar.Tag = Nothing
                Else
                    MessageBox.Show("Le nom de cette variable n'existe pas!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                End If
            Else
                MessageBox.Show("Veuillez sélectionner une variable à supprimer", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur BtnDelVar_MouseDown: " & ex.ToString, "Erreur Admin", "BtnDelVar_MouseDown")
        End Try
    End Sub

    ''' <summary>
    ''' Bouton appliquer variable
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BtnApplyVar_MouseDown(sender As System.Object, e As System.Windows.Input.MouseButtonEventArgs) Handles BtnApplyVar.MouseDown
        Try

            If BtnNewVar.Tag = 0 Then 'Modification de variable
                If _ListVar.ContainsKey(TxtVarName.Text) Then
                    _ListVar(TxtVarName.Text) = TxtVarValue.Text
                    Refresh_cbVar()
                Else
                    MessageBox.Show("Le nom de cette variable n'existe pas!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                End If
            Else 'Nouvelle variable
                If String.IsNullOrEmpty(TxtVarName.Text) Then Exit Sub
                If Not _ListVar.ContainsKey(TxtVarName.Text) Then
                    _ListVar.Add(TxtVarName.Text, TxtVarValue.Text)
                    Refresh_cbVar()
                Else
                    MessageBox.Show("Erreur cette variable existe déjà!!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                End If
            End If
            BtnApplyVar.Visibility = Windows.Visibility.Collapsed
            BtnNewVar.Visibility = Windows.Visibility.Visible
            BtnNewVar.Tag = Nothing
            BtnDelVar.Visibility = Windows.Visibility.Visible
            StkVar.Visibility = Windows.Visibility.Collapsed
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur BtnApplyVar_MouseDown: " & ex.ToString, "Erreur Admin", "BtnApplyVar_MouseDown")
        End Try
    End Sub

    Private Sub Refresh_cbVar()
        Try
            CbVariables.ItemsSource = New Forms.BindingSource(_ListVar, Nothing)
            CbVariables.DisplayMemberPath = "Key"
            CbVariables.SelectedValuePath = "Value"
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur Refresh_cbVar: " & ex.ToString, "Erreur Admin", "Refresh_cbVar")
        End Try
    End Sub


    Private Sub CbVariables_SelectionChanged(sender As Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles CbVariables.SelectionChanged
        Try
            If CbVariables.SelectedItem IsNot Nothing Then
                TxtVarName.Text = CbVariables.SelectedItem.key
                TxtVarName.IsReadOnly = True
                TxtVarValue.Text = _ListVar(TxtVarName.Text)
                StkVar.Visibility = Windows.Visibility.Visible
                BtnApplyVar.Visibility = Windows.Visibility.Visible
                BtnNewVar.Visibility = Windows.Visibility.Visible
                BtnDelVar.Visibility = Windows.Visibility.Visible
                BtnNewVar.Tag = 0 '0=Modifier 1=New
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur CbVariables_MouseUp: " & ex.ToString, "Erreur Admin", "CbVariables_MouseUp")
        End Try
    End Sub

#End Region


End Class
