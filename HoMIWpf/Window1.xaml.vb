#Region "Imports"
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Threading
Imports System.Xml
Imports System.Xml.XPath
Imports System.IO
Imports System.ServiceModel
Imports System.ServiceModel.Description
Imports System.Xml.Serialization
Imports HoMIDom.HoMIDom
Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.Web.HttpUtility
Imports System.Windows.Controls.Primitives
'Imports WpfApplication1.Designer
Imports HoMIWpF.Designer
Imports HoMIWpF.Designer.ResizeRotateAdorner
'Imports WpfApplication1.Designer.ResizeRotateAdorner
#End Region


Class Window1

#Region "Data"
    ' Used when manually scrolling.
    Private scrollTarget As Point
    Private scrollStartPoint As Point
    Private scrollStartOffset As Point
    Private imgStackPnl As New StackPanel()
    Dim FlagMsgDeconnect As Boolean = False
    Private Shared lock_logwrite As New Object

    'Paramètres de connexion à HomeSeer
    Dim _Serveur As String = ""
    Dim _Login As String = ""
    Dim _Password As String = ""
    Dim _PortSOAP As String = "7999"
    Dim _IP As String = "localhost"
    'XML
    Dim myxml As clsXML
    Dim list As XmlNodeList

    'User Graphic
    Dim _ShowSoleil As Boolean
    Dim _ShowTemperature As Boolean
    Dim _ImageBackGroundDefault As String
    Dim _ListMnu As New List(Of uCtrlImgMnu)
    Dim _Design As Boolean = False
    Dim _FullScreen As Boolean = True
    Dim mybuttonstyle As Style
    Public _CurrentIdZone As String
    Dim RandomNumber As New Random

#End Region

#Region "Property"

    Public Property ShowSoleil As Boolean
        Get
            Return _ShowSoleil
        End Get
        Set(ByVal value As Boolean)
            _ShowSoleil = value
            Affiche("Soleil", value)
        End Set
    End Property

    Public Property FullScreen As Boolean
        Get
            Return _FullScreen
        End Get
        Set(ByVal value As Boolean)
            _FullScreen = value
            Affiche("FullScreen", value)
        End Set
    End Property

    Public Property ShowTemperature As Boolean
        Get
            Return _ShowTemperature
        End Get
        Set(ByVal value As Boolean)
            _ShowTemperature = value
            ' Affiche("Temperature", value)
        End Set
    End Property

    Public Property ImageBackGround As String
        Get
            Return _ImageBackGroundDefault
        End Get
        Set(ByVal value As String)
            _ImageBackGroundDefault = value
            If _ImageBackGroundDefault <> "" Then
                If File.Exists(_ImageBackGroundDefault) Then
                    Dim bmpImage As New BitmapImage()
                    bmpImage.BeginInit()
                    bmpImage.CacheOption = BitmapCacheOption.OnLoad
                    bmpImage.CreateOptions = BitmapCreateOptions.DelayCreation
                    bmpImage.UriSource = New Uri(_ImageBackGroundDefault, UriKind.Absolute)
                    bmpImage.EndInit()
                    If bmpImage.CanFreeze Then bmpImage.Freeze()
                    ImgBackground.Source = bmpImage
                    bmpImage = Nothing
                Else
                    ImgBackground.Source = Nothing
                    _ImageBackGroundDefault = ""
                End If
            End If
        End Set
    End Property

    Public Property Friction As Double
        Get
            Return m_friction
        End Get
        Set(ByVal value As Double)
            m_friction = value
        End Set
    End Property

    Public Property SpeedTouch As Double
        Get
            Return m_SpeedTouch
        End Get
        Set(ByVal value As Double)
            If value < 100 Then value = 100
            If value > 1000 Then value = 100
            m_SpeedTouch = value
        End Set
    End Property

    Public Property PortSOAP As String
        Get
            Return _PortSOAP
        End Get
        Set(ByVal value As String)
            _PortSOAP = value
        End Set
    End Property

    Public Property IP As String
        Get
            Return _IP
        End Get
        Set(ByVal value As String)
            _IP = value
        End Set
    End Property

    Public Property HSAdresse As String
        Get
            Return _Serveur
        End Get
        Set(ByVal value As String)
            _Serveur = value
        End Set
    End Property

    Public Property HSLogin As String
        Get
            Return _Login
        End Get
        Set(ByVal value As String)
            _Login = value
        End Set
    End Property

    Public Property HSPassword As String
        Get
            Return _Password
        End Get
        Set(ByVal value As String)
            _Password = value
        End Set
    End Property

    Public Property ListMnu As List(Of uCtrlImgMnu)
        Get
            Return _ListMnu
        End Get
        Set(ByVal value As List(Of uCtrlImgMnu))
            _ListMnu = value
        End Set
    End Property
#End Region

    Public Sub [Affiche](ByVal Tag As String, ByVal Visible As Boolean)
        For i As Integer = 0 To StkTop.Children.Count - 1
            Dim x As Object = StkTop.Children.Item(i)
            If UCase(x.Tag) = UCase(Tag) Then
                If Visible = True Then
                    x.Visibility = Visibility.Visible
                Else
                    x.Visibility = Visibility.Hidden
                End If
            End If
        Next
    End Sub

    Public Sub UnloadControl(ByVal uid As String)
        Try
            For i As Integer = 0 To Canvas1.Children.Count - 1
                If Canvas1.Children.Item(i).Uid = uid Then
                    Canvas1.Children.RemoveAt(i)
                    Exit For
                End If
            Next

            ImageBackGround = _ImageBackGroundDefault
        Catch ex As Exception
            MessageBox.Show("Erreur UnloadControl: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Public Sub New()
        Try
            ' Cet appel est requis par le Concepteur Windows Form.
            InitializeComponent()

            Me.Cursor = Cursors.Wait

            Dim mystyles As New ResourceDictionary()
            mystyles.Source = New Uri("/HoMIWpF;component/Resources/DesignerItem.xaml",
                    UriKind.RelativeOrAbsolute)
            mybuttonstyle = mystyles("DesignerItemStyle")

            'si le repertoire appdata n'existe pas on le crée et copie la config depuis le repertoire d'installation
            If Not System.IO.Directory.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData) & "\HoMIWpF") Then
                System.IO.Directory.CreateDirectory(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData) & "\HoMIWpF")
            End If
            _MonRepertoireAppData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData) & "\HoMIWpF"
            If Not System.IO.Directory.Exists(_MonRepertoireAppData & "\Config") Then
                System.IO.Directory.CreateDirectory(_MonRepertoireAppData & "\Config")
                Dim oSource As DirectoryInfo = New DirectoryInfo(_MonRepertoire & "\Config")
                Dim oDestination As DirectoryInfo = New DirectoryInfo(_MonRepertoireAppData & "\Config")
                For Each oFichier As FileInfo In oSource.GetFiles()
                    oFichier.CopyTo(Path.Combine(oDestination.FullName, oFichier.Name))
                Next
            End If
            If Not System.IO.Directory.Exists(_MonRepertoireAppData & "\Logs") Then
                System.IO.Directory.CreateDirectory(_MonRepertoireAppData & "\Logs")
            End If

            'Chargement des paramètres et connexion au serveur si ok
            Log(TypeLog.INFO, TypeSource.CLIENT, "LOADCONFIG", "Message: " & LoadConfig(_MonRepertoireAppData & "\Config\"))

            ' Create StackPanel and set child elements to horizontal orientation
            imgStackPnl.HorizontalAlignment = HorizontalAlignment.Center
            imgStackPnl.VerticalAlignment = VerticalAlignment.Center
            imgStackPnl.Orientation = Orientation.Horizontal

            If IsConnect = True Then
                LoadZones()
            Else
                MessageBox.Show("Pas de connexion au serveur. Veuillez entrer les informations de connexion dans l'onglet serveur de la fenêtre de configuration.", "Information", MessageBoxButton.OK, MessageBoxImage.Information)
                _ImageBackGroundDefault = _MonRepertoire & "\Images\Fond-logo.png"
                Me.Show()
                MnuConfig_Click(Me, Nothing)
                SaveConfig(_MonRepertoireAppData & "\Config\HoMIWpF.xml")
                Log(TypeLog.INFO, TypeSource.CLIENT, "LOADCONFIG", "Message: " & LoadConfig(_MonRepertoireAppData & "\Config\"))
                LoadZones()
            End If

            'Mise en forme du scrollviewer
            ScrollViewer1.Content = imgStackPnl

            'Timer pour afficher la date & heure et levé/couché soleil
            Dim dt As DispatcherTimer = New DispatcherTimer()
            AddHandler dt.Tick, AddressOf dispatcherTimer_Tick
            dt.Interval = New TimeSpan(0, 0, 1)
            dt.Start()

            If IsConnect = True And ShowSoleil = True Then
                Dim mydate As Date
                mydate = myService.GetHeureLeverSoleil
                LblLeve.Content = mydate.ToShortTimeString
                mydate = myService.GetHeureCoucherSoleil
                LblCouche.Content = mydate.ToShortTimeString
            End If

            myxml = Nothing
            frmMere = Me
        Catch ex As Exception
            MessageBox.Show("Erreur lors du lancement de l'application: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub


#Region "Configuration"
    ''' <summary>Chargement de la config depuis le fichier XML</summary>
    ''' <param name="Fichier"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LoadConfig(ByVal Fichier As String) As String
        'Copy du fichier de config avant chargement
        Try
            Dim _file As String = Fichier & "HoMIWpF"
            If File.Exists(_file & ".bak") = True Then File.Delete(_file & ".bak")
            If File.Exists(_file & ".xml") = True Then
                File.Copy(_file & ".xml", Mid(_file & ".xml", 1, Len(_file & ".xml") - 4) & ".bak")
                Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Création du backup (.bak) du fichier de config avant chargement")
            Else
                ' Le fichier de config est inexistant. Premier lancement de WPF.
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur lors du lancement de l'application: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try

        Try
            Dim dirInfo As New System.IO.DirectoryInfo(Fichier)
            Dim file As System.IO.FileInfo
            Dim files() As System.IO.FileInfo = dirInfo.GetFiles("HoMIWpF.xml")
            Dim myxml As XML

            If (files IsNot Nothing) Then
                For Each file In files
                    Dim myfile As String = file.FullName
                    Dim list As XmlNodeList

                    myxml = New XML(myfile)

                    Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Chargement du fichier config: " & myfile)

                    '******************************************
                    'on va chercher les paramètres du serveur
                    '******************************************
                    list = myxml.SelectNodes("/homidom/server")
                    If list.Count > 0 Then 'présence des paramètres du server
                        For j As Integer = 0 To list.Item(0).Attributes.Count - 1
                            Select Case list.Item(0).Attributes.Item(j).Name
                                Case "portsoap"
                                    _PortSOAP = list.Item(0).Attributes.Item(j).Value
                                Case "ip"
                                    _IP = list.Item(0).Attributes.Item(j).Value
                                Case "id"
                                    IdSrv = list.Item(0).Attributes.Item(j).Value
                                Case Else
                                    Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Un attribut correspondant au serveur est inconnu: nom:" & list.Item(0).Attributes.Item(j).Name & " Valeur: " & list.Item(0).Attributes.Item(j).Value)
                            End Select
                        Next
                    Else
                        MsgBox("Il manque les paramètres du client WPF dans le fichier de config !!", MsgBoxStyle.Exclamation, "Erreur serveur")
                    End If
                    Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Paramètres du client WPF chargés")

                    'ON peut se connecter
                    'Connexion à Homidom
                    ConnectToHomidom()

                    Dim _srv As New ClServer
                    _srv.Adresse = _IP
                    _srv.Defaut = True
                    _srv.Nom = "Defaut"
                    _srv.Port = _PortSOAP
                    _ListServer.Add(_srv)

                    '******************************************
                    'on va chercher les paramètres tactile
                    '******************************************
                    list = myxml.SelectNodes("/homidom/tactile")
                    If list.Count > 0 Then 'présence des paramètres du server
                        For j As Integer = 0 To list.Item(0).Attributes.Count - 1
                            Select Case list.Item(0).Attributes.Item(j).Name
                                Case "friction"
                                    'm_friction = CDbl(list.Item(0).Attributes.Item(j).Value.Replace(".", ","))
                                    m_friction = CDbl(list.Item(0).Attributes.Item(j).Value.Replace(".", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator))
                                Case "speedtouch"
                                    'm_SpeedTouch = CDbl(list.Item(0).Attributes.Item(j).Value.Replace(".", ","))
                                    m_SpeedTouch = CDbl(list.Item(0).Attributes.Item(j).Value.Replace(".", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator))
                                Case Else
                                    Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Un attribut correspondant au serveur est inconnu: nom:" & list.Item(0).Attributes.Item(j).Name & " Valeur: " & list.Item(0).Attributes.Item(j).Value)
                            End Select
                        Next
                    Else
                        MsgBox("Il manque les paramètres du client WPF dans le fichier de config !!", MsgBoxStyle.Exclamation, "Erreur Client WPF")
                    End If
                    Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Paramètres tactiles chargés")

                    '******************************************
                    'on va chercher les paramètres interface
                    '******************************************
                    list = myxml.SelectNodes("/homidom/interface")
                    If list.Count > 0 Then 'présence des paramètres du server
                        For j As Integer = 0 To list.Item(0).Attributes.Count - 1
                            Select Case list.Item(0).Attributes.Item(j).Name
                                Case "showsoleil"
                                    ShowSoleil = list.Item(0).Attributes.Item(j).Value
                                Case "showtemperature"
                                    ShowTemperature = list.Item(0).Attributes.Item(j).Value
                                Case "imgbackground"
                                    ImageBackGround = list.Item(0).Attributes.Item(j).Value
                                Case "fullscreen"
                                    If list.Item(0).Attributes.Item(j).Value = False Then
                                        Me.FullScreen = False
                                        Me.WindowState = Windows.WindowState.Normal
                                    Else
                                        Me.FullScreen = True
                                        Me.WindowState = Windows.WindowState.Maximized
                                    End If
                                Case "left"
                                    Me.Left = list.Item(0).Attributes.Item(j).Value
                                Case "top"
                                    Me.Top = list.Item(0).Attributes.Item(j).Value
                                Case "width"
                                    Me.Width = list.Item(0).Attributes.Item(j).Value
                                Case "height"
                                    Me.Height = list.Item(0).Attributes.Item(j).Value
                                Case Else
                                    Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Un attribut correspondant au serveur est inconnu: nom:" & list.Item(0).Attributes.Item(j).Name & " Valeur: " & list.Item(0).Attributes.Item(j).Value)
                            End Select
                        Next
                    Else
                        MsgBox("Il manque les paramètres du client WPF dans le fichier de config !!", MsgBoxStyle.Exclamation, "Erreur Client WPF")
                    End If
                    Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Paramètres de l'interface chargés")

                    '******************************************
                    'on va chercher les menus
                    '******************************************
                    list = myxml.SelectNodes("/homidom/menus/menu")
                    If list.Count > 0 Then 'présence des paramètres du server
                        For j As Integer = 0 To list.Count - 1
                            Dim _MnuNom As String = ""
                            Dim _MnuType As uCtrlImgMnu.TypeOfMnu
                            Dim _MnuIcon As String = ""
                            Dim _MnuParam As New List(Of String)
                            Dim _Mnudefaut As Boolean
                            Dim _MnuIDElement As String = ""
                            Dim _MnuVisible As Boolean = False

                            For k As Integer = 0 To list.Item(j).Attributes.Count - 1
                                Select Case list.Item(j).Attributes.Item(k).Name
                                    Case "nom"
                                        _MnuNom = list.Item(j).Attributes.Item(k).Value
                                    Case "defaut"
                                        _Mnudefaut = list.Item(j).Attributes.Item(k).Value
                                    Case "type"
                                        Select Case list.Item(j).Attributes.Item(k).Value
                                            Case uCtrlImgMnu.TypeOfMnu.Internet.ToString
                                                _MnuType = uCtrlImgMnu.TypeOfMnu.Internet
                                            Case uCtrlImgMnu.TypeOfMnu.Meteo.ToString
                                                _MnuType = uCtrlImgMnu.TypeOfMnu.Meteo
                                            Case uCtrlImgMnu.TypeOfMnu.Zone.ToString
                                                _MnuType = uCtrlImgMnu.TypeOfMnu.Zone
                                            Case uCtrlImgMnu.TypeOfMnu.Config.ToString
                                                _MnuType = uCtrlImgMnu.TypeOfMnu.Config
                                            Case uCtrlImgMnu.TypeOfMnu.LecteurMedia.ToString
                                                _MnuType = uCtrlImgMnu.TypeOfMnu.LecteurMedia
                                            Case uCtrlImgMnu.TypeOfMnu.None.ToString
                                                _MnuType = uCtrlImgMnu.TypeOfMnu.Config
                                        End Select
                                    Case "icon"
                                        _MnuIcon = list.Item(j).Attributes.Item(k).Value
                                    Case "idelement"
                                        _MnuIDElement = list.Item(j).Attributes.Item(k).Value
                                    Case "visible"
                                        _MnuVisible = list.Item(j).Attributes.Item(k).Value
                                    Case Else
                                        Dim a As String = list.Item(j).Attributes.Item(k).Name
                                        If a.Contains("parametre") Then
                                            _MnuParam.Add(list.Item(j).Attributes.Item(k).Value)
                                        End If
                                End Select
                            Next
                            If _MnuType <> uCtrlImgMnu.TypeOfMnu.Config Then NewBtnMnu(_MnuNom, _MnuType, _MnuParam, _Mnudefaut, , _MnuIcon, _MnuIDElement, _MnuVisible)
                        Next
                    Else
                        'MsgBox("Il manque les paramètres du client WPF dans le fichier de config !!", MsgBoxStyle.Exclamation, "Erreur Client WPF")
                    End If
                    Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Menus chargés")

                    '******************************************
                    'on va chercher les éléments
                    '******************************************
                    list = myxml.SelectNodes("/homidom/elements/element")
                    If list.Count > 0 Then 'présence des éléments
                        For j As Integer = 0 To list.Count - 1
                            Dim x As New uWidgetEmpty

                            For k As Integer = 0 To list.Item(j).Attributes.Count - 1
                                Select Case list.Item(j).Attributes.Item(k).Name
                                    Case "uid"
                                        x.Uid = list.Item(j).Attributes.Item(k).Value
                                    Case "id"
                                        x.Id = list.Item(j).Attributes.Item(k).Value
                                    Case "isempty"
                                        x.IsEmpty = list.Item(j).Attributes.Item(k).Value
                                    Case "type"
                                        Select Case list.Item(j).Attributes.Item(k).Value
                                            Case uWidgetEmpty.TypeOfWidget.Empty.ToString
                                                x.Type = uWidgetEmpty.TypeOfWidget.Empty
                                            Case uWidgetEmpty.TypeOfWidget.Device.ToString
                                                x.Type = uWidgetEmpty.TypeOfWidget.Device
                                            Case uWidgetEmpty.TypeOfWidget.Media.ToString
                                                x.Type = uWidgetEmpty.TypeOfWidget.Media
                                            Case uWidgetEmpty.TypeOfWidget.Web.ToString
                                                x.Type = uWidgetEmpty.TypeOfWidget.Web
                                            Case uWidgetEmpty.TypeOfWidget.Camera.ToString
                                                x.Type = uWidgetEmpty.TypeOfWidget.Camera
                                            Case uWidgetEmpty.TypeOfWidget.Rss.ToString
                                                x.Type = uWidgetEmpty.TypeOfWidget.Rss
                                            Case uWidgetEmpty.TypeOfWidget.Meteo.ToString
                                                x.Type = uWidgetEmpty.TypeOfWidget.Meteo
                                            Case uWidgetEmpty.TypeOfWidget.KeyPad.ToString
                                                x.Type = uWidgetEmpty.TypeOfWidget.KeyPad
                                            Case uWidgetEmpty.TypeOfWidget.Label.ToString
                                                x.Type = uWidgetEmpty.TypeOfWidget.Label
                                        End Select
                                    Case "caneditvalue"
                                        x.CanEditValue = list.Item(j).Attributes.Item(k).Value
                                    Case "zoneid"
                                        x.ZoneId = list.Item(j).Attributes.Item(k).Value
                                    Case "x"
                                        x.X = list.Item(j).Attributes.Item(k).Value
                                    Case "y"
                                        x.Y = list.Item(j).Attributes.Item(k).Value
                                    Case "width"
                                        x.Width = list.Item(j).Attributes.Item(k).Value
                                    Case "height"
                                        x.Height = list.Item(j).Attributes.Item(k).Value
                                    Case "angle"
                                        x.Rotation = list.Item(j).Attributes.Item(k).Value
                                    Case "showetiquette"
                                        x.ShowEtiquette = list.Item(j).Attributes.Item(k).Value
                                    Case "showstatus"
                                        x.ShowStatus = list.Item(j).Attributes.Item(k).Value
                                    Case "etiquette"
                                        x.Etiquette = list.Item(j).Attributes.Item(k).Value
                                    Case "picture"
                                        x.Picture = list.Item(j).Attributes.Item(k).Value
                                    Case "showpicture"
                                        x.ShowPicture = list.Item(j).Attributes.Item(k).Value
                                    Case "defautlabelstatus"
                                        x.DefautLabelStatus = list.Item(j).Attributes.Item(k).Value
                                    Case "taillestatus"
                                        x.TailleStatus = list.Item(j).Attributes.Item(k).Value
                                    Case "tailleetiquette"
                                        x.TailleEtiquette = list.Item(j).Attributes.Item(k).Value
                                    Case "colorbackground"
                                        If list.Item(j).Attributes.Item(k).Value <> "" Then
                                            Dim a As Byte = CByte("&H" & Mid(list.Item(j).Attributes.Item(k).Value, 2, 2))
                                            Dim R As Byte = CByte("&H" & Mid(list.Item(j).Attributes.Item(k).Value, 4, 2))
                                            Dim G As Byte = CByte("&H" & Mid(list.Item(j).Attributes.Item(k).Value, 6, 2))
                                            Dim B As Byte = CByte("&H" & Mid(list.Item(j).Attributes.Item(k).Value, 8, 2))
                                            x.ColorBackGround = New SolidColorBrush(Color.FromArgb(a, R, G, B))
                                        End If
                                    Case "colorstatus"
                                        Dim a As Byte = CByte("&H" & Mid(list.Item(j).Attributes.Item(k).Value, 2, 2))
                                        Dim R As Byte = CByte("&H" & Mid(list.Item(j).Attributes.Item(k).Value, 4, 2))
                                        Dim G As Byte = CByte("&H" & Mid(list.Item(j).Attributes.Item(k).Value, 6, 2))
                                        Dim B As Byte = CByte("&H" & Mid(list.Item(j).Attributes.Item(k).Value, 8, 2))
                                        x.ColorStatus = New SolidColorBrush(Color.FromArgb(a, R, G, B))
                                    Case "coloretiquette"
                                        Dim a As Byte = CByte("&H" & Mid(list.Item(j).Attributes.Item(k).Value, 2, 2))
                                        Dim R As Byte = CByte("&H" & Mid(list.Item(j).Attributes.Item(k).Value, 4, 2))
                                        Dim G As Byte = CByte("&H" & Mid(list.Item(j).Attributes.Item(k).Value, 6, 2))
                                        Dim B As Byte = CByte("&H" & Mid(list.Item(j).Attributes.Item(k).Value, 8, 2))
                                        x.ColorEtiquette = New SolidColorBrush(Color.FromArgb(a, R, G, B))
                                    Case "url"
                                        x.URL = list.Item(j).Attributes.Item(k).Value
                                    Case "urlrss"
                                        x.UrlRss = list.Item(j).Attributes.Item(k).Value
                                    Case "idmeteo"
                                        x.IDMeteo = list.Item(j).Attributes.Item(k).Value
                                    Case "idkeypad"
                                        x.IDKeyPad = list.Item(j).Attributes.Item(k).Value
                                    Case "showpassword"
                                        x.ShowPassWord = list.Item(j).Attributes.Item(k).Value
                                    Case "clearafterenter"
                                        x.ClearAfterEnter = list.Item(j).Attributes.Item(k).Value
                                    Case "showclavier"
                                        x.ShowClavier = list.Item(j).Attributes.Item(k).Value
                                    Case "httprefresh"
                                        x.HttpRefresh = list.Item(j).Attributes.Item(k).Value
                                End Select
                            Next

                            If list.Item(j).HasChildNodes Then
                                For l As Integer = 0 To list.Item(j).ChildNodes.Count - 1
                                    If UCase(list.Item(j).ChildNodes.Item(l).Name) = "ACTIONS" Then
                                        For m As Integer = 0 To list.Item(j).ChildNodes.Item(l).ChildNodes.Count - 1
                                            Dim _act As New cWidget.Action
                                            With _act
                                                .IdObject = list.Item(j).ChildNodes.Item(l).ChildNodes.Item(m).Attributes.Item(1).Value
                                                .Methode = list.Item(j).ChildNodes.Item(l).ChildNodes.Item(m).Attributes.Item(2).Value
                                                If list.Item(j).ChildNodes.Item(l).ChildNodes.Item(m).Attributes.Item(3).Value IsNot Nothing Then .Value = list.Item(j).ChildNodes.Item(l).ChildNodes.Item(m).Attributes.Item(3).Value
                                                If list.Item(j).ChildNodes.Item(l).ChildNodes.Item(m).Attributes.Item(4).Value IsNot Nothing Then .Sound = list.Item(j).ChildNodes.Item(l).ChildNodes.Item(m).Attributes.Item(4).Value
                                            End With

                                            Select Case list.Item(j).ChildNodes.Item(l).ChildNodes.Item(m).Attributes.Item(0).Value
                                                Case "gestureonclick"
                                                    x.Action_On_Click.Add(_act)
                                                Case "gestureonlongclick"
                                                    x.Action_On_LongClick.Add(_act)
                                                Case "gesturehautbas"
                                                    x.Action_GestureHautBas.Add(_act)
                                                Case "gesturebashaut"
                                                    x.Action_GestureBasHaut.Add(_act)
                                                Case "gesturegauchedroite"
                                                    x.Action_GestureGaucheDroite.Add(_act)
                                                Case "gesturedroitegauche"
                                                    x.Action_GestureDroiteGauche.Add(_act)
                                            End Select
                                        Next
                                    End If

                                    If UCase(list.Item(j).ChildNodes.Item(l).Name) = "VISUELS" Then
                                        For m As Integer = 0 To list.Item(j).ChildNodes.Item(l).ChildNodes.Count - 1
                                            Dim _act As New cWidget.Visu
                                            With _act
                                                .IdObject = list.Item(j).ChildNodes.Item(l).ChildNodes.Item(m).Attributes.Item(0).Value
                                                .Propriete = list.Item(j).ChildNodes.Item(l).ChildNodes.Item(m).Attributes.Item(1).Value
                                                If list.Item(j).ChildNodes.Item(l).ChildNodes.Item(m).Attributes.Item(2).Value IsNot Nothing Then .Value = list.Item(j).ChildNodes.Item(l).ChildNodes.Item(m).Attributes.Item(2).Value
                                                If list.Item(j).ChildNodes.Item(l).ChildNodes.Item(m).Attributes.Item(3).Value IsNot Nothing Then .Image = list.Item(j).ChildNodes.Item(l).ChildNodes.Item(m).Attributes.Item(3).Value
                                            End With

                                            x.Visuel.Add(_act)
                                        Next
                                    End If

                                    If UCase(list.Item(j).ChildNodes.Item(l).Name) = "WEB" Or UCase(list.Item(j).ChildNodes.Item(l).Name) = "CAMERA" Then
                                        For m As Integer = 0 To list.Item(j).ChildNodes.Item(l).ChildNodes.Count - 1
                                            Dim _btn As New uHttp.ButtonHttp
                                            With _btn
                                                .Content = list.Item(j).ChildNodes.Item(l).ChildNodes.Item(m).Attributes.Item(0).Value
                                                .URL = list.Item(j).ChildNodes.Item(l).ChildNodes.Item(m).Attributes.Item(1).Value
                                                .Width = list.Item(j).ChildNodes.Item(l).ChildNodes.Item(m).Attributes.Item(2).Value
                                                .Height = list.Item(j).ChildNodes.Item(l).ChildNodes.Item(m).Attributes.Item(3).Value
                                            End With

                                            x.ListHttpButton.Add(_btn)

                                        Next
                                    End If
                                Next
                            End If

                            If x.IsEmpty = False Then 'si c pas un widget vide
                                If IsConnect Then
                                    If myService.ReturnDeviceByID(IdSrv, x.Id) IsNot Nothing Then 'Si le device n'a pas été trouvé on le prend pas en compte pour le supprimer par la suite
                                        _ListElement.Add(x)
                                    Else
                                        If myService.ReturnMacroById(IdSrv, x.Id) IsNot Nothing Then
                                            _ListElement.Add(x)
                                        End If
                                    End If
                                End If
                            Else
                                _ListElement.Add(x)
                            End If
                        Next
                    Else
                        'MsgBox("Il manque les paramètres du client WPF dans le fichier de config !!", MsgBoxStyle.Exclamation, "Erreur Client WPF")
                        Log(TypeLog.ERREUR, TypeSource.SERVEUR, "LoadConfig", "Il manque des paramètres dans le fichier de configuration du client WPF")
                    End If
                    Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Eléments chargés")
                    '**************
                Next
            End If

            'Vide les variables
            dirInfo = Nothing
            file = Nothing
            files = Nothing
            myxml = Nothing

            Return " Chargement de la configuration terminée"

        Catch ex As Exception
            MsgBox("ERREUR LOADCONFIG " & ex.Message, MsgBoxStyle.Exclamation, "Erreur Client WPF")
            Return " Erreur de chargement de la config: " & ex.Message
        End Try
    End Function

    ''' <summary>Sauvegarde de la config dans le fichier XML</summary>
    ''' <remarks></remarks>
    Private Sub SaveConfig(ByVal Fichier As String)
        Try

            Log(TypeLog.INFO, TypeSource.SERVEUR, "SaveConfig", "Sauvegarde de la config sous le fichier " & Fichier)

            ''Copy du fichier de config avant sauvegarde
            Try
                Dim _file As String = Fichier.Replace(".xml", "")
                If File.Exists(_file & ".sav") = True Then File.Delete(_file & ".sav")
                File.Copy(_file & ".xml", _file & ".sav")
                Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Création de sauvegarde (.sav) du fichier de config avant sauvegarde")
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SaveConfig", "Erreur impossible de créer une copie de backup du fichier de config: " & ex.Message)
            End Try

            ''Creation du fichier XML
            Dim writer As New XmlTextWriter(Fichier, System.Text.Encoding.UTF8)
            writer.WriteStartDocument(True)
            writer.Formatting = Formatting.Indented
            writer.Indentation = 2

            writer.WriteStartElement("homidom")

            Log(TypeLog.INFO, TypeSource.SERVEUR, "SaveConfig", "Sauvegarde des paramètres du client WPF")
            ''------------ server
            writer.WriteStartElement("server")
            writer.WriteStartAttribute("portsoap")
            writer.WriteValue(_PortSOAP)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("ip")
            writer.WriteValue(_IP)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("id")
            writer.WriteValue(IdSrv)
            writer.WriteEndAttribute()
            writer.WriteEndElement()

            ''------------ HomeSeer
            writer.WriteStartElement("HS")
            writer.WriteStartAttribute("adresse")
            writer.WriteValue(_Serveur)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("login")
            writer.WriteValue(_Login)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("password")
            writer.WriteValue(_Login)
            writer.WriteEndAttribute()
            writer.WriteEndElement()

            ''-------------------
            ''------------tactile
            ''------------------
            writer.WriteStartElement("tactile")
            writer.WriteStartAttribute("speedtouch")
            writer.WriteValue(SpeedTouch)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("friction")
            writer.WriteValue(Friction)
            writer.WriteEndAttribute()
            writer.WriteEndElement()

            ''------------
            ''Sauvegarde des parametres d'interface
            ''------------
            writer.WriteStartElement("interface")
            writer.WriteStartAttribute("showsoleil")
            writer.WriteValue(ShowSoleil)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("showtemperature")
            writer.WriteValue(ShowTemperature)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("imgbackground")
            writer.WriteValue(ImageBackGround)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("fullscreen")
            writer.WriteValue(FullScreen)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("left")
            writer.WriteValue(Me.Left)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("top")
            writer.WriteValue(Me.Top)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("width")
            writer.WriteValue(Me.Width)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("height")
            writer.WriteValue(Me.Height)
            writer.WriteEndAttribute()
            writer.WriteEndElement()

            ''------------
            ''Sauvegarde des menus
            ''------------
            writer.WriteStartElement("menus")
            For i As Integer = 0 To ListMnu.Count - 1
                writer.WriteStartElement("menu")
                writer.WriteStartAttribute("nom")
                writer.WriteValue(ListMnu.Item(i).Text)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("defaut")
                writer.WriteValue(ListMnu.Item(i).Defaut)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("type")
                writer.WriteValue(ListMnu.Item(i).Type.ToString)
                writer.WriteEndAttribute()
                If ListMnu.Item(i).Type <> uCtrlImgMnu.TypeOfMnu.Zone Then
                    writer.WriteStartAttribute("icon")
                    writer.WriteValue(ListMnu.Item(i).Icon)
                    writer.WriteEndAttribute()
                End If
                If ListMnu.Item(i).Type = uCtrlImgMnu.TypeOfMnu.Zone Then
                    writer.WriteStartAttribute("idelement")
                    writer.WriteValue(ListMnu.Item(i).IDElement)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("visible")
                    writer.WriteValue(ListMnu.Item(i).Visible)
                    writer.WriteEndAttribute()
                End If
                If ListMnu.Item(i).Parametres IsNot Nothing Then
                    For j As Integer = 0 To ListMnu.Item(i).Parametres.Count - 1
                        writer.WriteStartAttribute("parametre" & j)
                        writer.WriteValue(ListMnu.Item(i).Parametres(j))
                        writer.WriteEndAttribute()
                    Next
                End If
                writer.WriteEndElement()
            Next
            writer.WriteEndElement()

            ''------------
            ''Sauvegarde des elements
            ''------------
            writer.WriteStartElement("elements")
            For i As Integer = 0 To _ListElement.Count - 1

                writer.WriteStartElement("element")
                writer.WriteStartAttribute("uid") 'ID du widget
                writer.WriteValue(_ListElement.Item(i).Uid)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("id") 'ID de l'élément (device, zone, macro...)
                writer.WriteValue(_ListElement.Item(i).Id)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("isempty") 'ID de l'élément (device, zone, macro...)
                writer.WriteValue(_ListElement.Item(i).IsEmpty)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("type") 'type de widget
                writer.WriteValue(_ListElement.Item(i).Type.ToString)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("caneditvalue") 'type de widget
                writer.WriteValue(_ListElement.Item(i).CanEditValue)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("zoneid")
                writer.WriteValue(_ListElement.Item(i).ZoneId)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("x")
                writer.WriteValue(Replace(CDbl(_ListElement.Item(i).X), ".", ","))
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("y")
                writer.WriteValue(Replace(CDbl(_ListElement.Item(i).Y), ".", ","))
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("width")
                writer.WriteValue(_ListElement.Item(i).Width)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("height")
                writer.WriteValue(_ListElement.Item(i).Height)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("angle")
                writer.WriteValue(_ListElement.Item(i).Rotation)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("showetiquette")
                writer.WriteValue(_ListElement.Item(i).ShowEtiquette)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("showstatus")
                writer.WriteValue(_ListElement.Item(i).ShowStatus)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("showpicture")
                writer.WriteValue(_ListElement.Item(i).ShowPicture)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("picture")
                writer.WriteValue(_ListElement.Item(i).Picture)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("etiquette")
                writer.WriteValue(_ListElement.Item(i).Etiquette)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("defautlabelstatus")
                writer.WriteValue(_ListElement.Item(i).DefautLabelStatus)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("taillestatus")
                writer.WriteValue(_ListElement.Item(i).TailleStatus)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("tailleetiquette")
                writer.WriteValue(_ListElement.Item(i).TailleEtiquette)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("colorbackground")
                'writer.WriteValue(_ListElement.Item(i).ColorBackGround.ToString)
                writer.WriteValue(_ListElement.Item(i).ColorBackGround.ToString.Replace("#FF", "#" & Hex(CInt(_ListElement.Item(i).ColorBackGround.Opacity * 255))))
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("colorstatus")
                writer.WriteValue(_ListElement.Item(i).ColorStatus.ToString)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("coloretiquette")
                writer.WriteValue(_ListElement.Item(i).ColorEtiquette.ToString)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("url")
                writer.WriteValue(_ListElement.Item(i).URL)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("urlrss")
                writer.WriteValue(_ListElement.Item(i).UrlRss)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("idmeteo")
                writer.WriteValue(_ListElement.Item(i).IDMeteo)
                writer.WriteEndAttribute()
                If _ListElement.Item(i).Type = uWidgetEmpty.TypeOfWidget.KeyPad Then
                    writer.WriteStartAttribute("idkeypad")
                    writer.WriteValue(_ListElement.Item(i).IDKeyPad)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("showpassword")
                    writer.WriteValue(_ListElement.Item(i).ShowPassWord)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("clearafterenter")
                    writer.WriteValue(_ListElement.Item(i).ShowPassWord)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("showclavier")
                    writer.WriteValue(_ListElement.Item(i).ShowPassWord)
                    writer.WriteEndAttribute()
                End If
                writer.WriteStartAttribute("httprefresh")
                writer.WriteValue(_ListElement.Item(i).HttpRefresh)
                writer.WriteEndAttribute()

                If _ListElement.Item(i).Type = uWidgetEmpty.TypeOfWidget.Web Or _ListElement.Item(i).Type = uWidgetEmpty.TypeOfWidget.Camera Then
                    writer.WriteStartElement("web")
                    For j As Integer = 0 To _ListElement.Item(i).ListHttpButton.Count - 1
                        writer.WriteStartElement("httpbutton")
                        writer.WriteStartAttribute("label")
                        writer.WriteValue(_ListElement.Item(i).ListHttpButton.Item(j).Content)
                        writer.WriteEndAttribute()
                        writer.WriteStartAttribute("url")
                        writer.WriteValue(_ListElement.Item(i).ListHttpButton.Item(j).URL)
                        writer.WriteEndAttribute()
                        writer.WriteStartAttribute("width")
                        writer.WriteValue(_ListElement.Item(i).ListHttpButton.Item(j).Width)
                        writer.WriteEndAttribute()
                        writer.WriteStartAttribute("height")
                        writer.WriteValue(_ListElement.Item(i).ListHttpButton.Item(j).Height)
                        writer.WriteEndAttribute()
                        writer.WriteEndElement()
                    Next
                    writer.WriteEndElement()
                End If

                writer.WriteStartElement("actions")
                For j As Integer = 0 To _ListElement.Item(i).Action_GestureBasHaut.Count - 1
                    writer.WriteStartElement("action")
                    writer.WriteStartAttribute("type") 'type d'action
                    writer.WriteValue("gesturebashaut")
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("idobject") 'objet
                    writer.WriteValue(_ListElement.Item(i).Action_GestureBasHaut.Item(j).IdObject)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("methode") 'methode
                    writer.WriteValue(_ListElement.Item(i).Action_GestureBasHaut.Item(j).Methode)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("value") 'methode
                    writer.WriteValue(_ListElement.Item(i).Action_GestureBasHaut.Item(j).Value)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("sound") 'methode
                    writer.WriteValue(_ListElement.Item(i).Action_GestureBasHaut.Item(j).Sound)
                    writer.WriteEndAttribute()
                    writer.WriteEndElement()
                Next
                For j As Integer = 0 To _ListElement.Item(i).Action_GestureDroiteGauche.Count - 1
                    writer.WriteStartElement("action")
                    writer.WriteStartAttribute("type") 'type d'action
                    writer.WriteValue("gesturedroitegauche")
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("idobject") 'objet
                    writer.WriteValue(_ListElement.Item(i).Action_GestureDroiteGauche.Item(j).IdObject)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("methode") 'methode
                    writer.WriteValue(_ListElement.Item(i).Action_GestureDroiteGauche.Item(j).Methode)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("value") 'methode
                    writer.WriteValue(_ListElement.Item(i).Action_GestureDroiteGauche.Item(j).Value)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("sound") 'methode
                    writer.WriteValue(_ListElement.Item(i).Action_GestureDroiteGauche.Item(j).Sound)
                    writer.WriteEndAttribute()
                    writer.WriteEndElement()
                Next
                For j As Integer = 0 To _ListElement.Item(i).Action_GestureGaucheDroite.Count - 1
                    writer.WriteStartElement("action")
                    writer.WriteStartAttribute("type") 'type d'action
                    writer.WriteValue("gesturegauchedroite")
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("idobject") 'objet
                    writer.WriteValue(_ListElement.Item(i).Action_GestureGaucheDroite.Item(j).IdObject)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("methode") 'methode
                    writer.WriteValue(_ListElement.Item(i).Action_GestureGaucheDroite.Item(j).Methode)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("value") 'methode
                    writer.WriteValue(_ListElement.Item(i).Action_GestureGaucheDroite.Item(j).Value)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("sound") 'methode
                    writer.WriteValue(_ListElement.Item(i).Action_GestureGaucheDroite.Item(j).Sound)
                    writer.WriteEndAttribute()
                    writer.WriteEndElement()
                Next
                For j As Integer = 0 To _ListElement.Item(i).Action_GestureHautBas.Count - 1
                    writer.WriteStartElement("action")
                    writer.WriteStartAttribute("type") 'type d'action
                    writer.WriteValue("gesturehautbas")
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("idobject") 'objet
                    writer.WriteValue(_ListElement.Item(i).Action_GestureHautBas.Item(j).IdObject)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("methode") 'methode
                    writer.WriteValue(_ListElement.Item(i).Action_GestureHautBas.Item(j).Methode)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("value") 'methode
                    writer.WriteValue(_ListElement.Item(i).Action_GestureHautBas.Item(j).Value)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("sound") 'methode
                    writer.WriteValue(_ListElement.Item(i).Action_GestureHautBas.Item(j).Sound)
                    writer.WriteEndAttribute()
                    writer.WriteEndElement()
                Next
                For j As Integer = 0 To _ListElement.Item(i).Action_On_Click.Count - 1
                    writer.WriteStartElement("action")
                    writer.WriteStartAttribute("type") 'type d'action
                    writer.WriteValue("gestureonclick")
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("idobject") 'objet
                    writer.WriteValue(_ListElement.Item(i).Action_On_Click.Item(j).IdObject)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("methode") 'methode
                    writer.WriteValue(_ListElement.Item(i).Action_On_Click.Item(j).Methode)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("value") 'methode
                    writer.WriteValue(_ListElement.Item(i).Action_On_Click.Item(j).Value)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("sound") 'methode
                    writer.WriteValue(_ListElement.Item(i).Action_On_Click.Item(j).Sound)
                    writer.WriteEndAttribute()
                    writer.WriteEndElement()
                Next
                For j As Integer = 0 To _ListElement.Item(i).Action_On_LongClick.Count - 1
                    writer.WriteStartElement("action")
                    writer.WriteStartAttribute("type") 'type d'action
                    writer.WriteValue("gestureonlongclick")
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("idobject") 'objet
                    writer.WriteValue(_ListElement.Item(i).Action_On_LongClick.Item(j).IdObject)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("methode") 'methode
                    writer.WriteValue(_ListElement.Item(i).Action_On_LongClick.Item(j).Methode)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("value") 'methode
                    writer.WriteValue(_ListElement.Item(i).Action_On_LongClick.Item(j).Value)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("sound") 'methode
                    writer.WriteValue(_ListElement.Item(i).Action_On_LongClick.Item(j).Sound)
                    writer.WriteEndAttribute()
                    writer.WriteEndElement()
                Next
                writer.WriteEndElement()

                writer.WriteStartElement("visuels")
                For j As Integer = 0 To _ListElement.Item(i).Visuel.Count - 1
                    writer.WriteStartElement("visuel")
                    writer.WriteStartAttribute("idobject") 'objet
                    writer.WriteValue(_ListElement.Item(i).Visuel.Item(j).IdObject)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("property") 'methode
                    writer.WriteValue(_ListElement.Item(i).Visuel.Item(j).Propriete)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("value")
                    writer.WriteValue(_ListElement.Item(i).Visuel.Item(j).Value)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("image") 'methode
                    writer.WriteValue(_ListElement.Item(i).Visuel.Item(j).Image)
                    writer.WriteEndAttribute()
                    writer.WriteEndElement()
                Next
                writer.WriteEndElement()
                writer.WriteEndElement()
            Next
            writer.WriteEndElement()
            ''FIN DES ELEMENTS------------

            writer.WriteEndDocument()
            writer.Close()
            Log(TypeLog.INFO, TypeSource.SERVEUR, "SaveConfig", "Sauvegarde terminée")
        Catch ex As Exception
            MsgBox("ERREUR SAVECONFIG " & ex.Message, MsgBoxStyle.Exclamation, "Erreur Client WPF")
            Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SaveConfig", " Erreur de sauvegarde de la configuration: " & ex.Message)
        End Try

    End Sub

#End Region

#Region "Log"
    Dim _File As String = _MonRepertoireAppData & "\logs\logClientWPF.xml" 'Représente le fichier log: ex"C:\users\xxx\homiwpf\logs\log.xml"
    Dim _MaxFileSize As Long = 5120 'en Koctets

    ''' <summary>
    ''' Permet de connaître le chemin du fichier log
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property FichierLog() As String
        Get
            Return _File
        End Get
    End Property

    ''' <summary>
    ''' Retourne/Fixe la Taille max du fichier log en Ko
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MaxFileSize() As Long
        Get
            Return _MaxFileSize
        End Get
        Set(ByVal value As Long)
            _MaxFileSize = value
        End Set
    End Property

    ''' <summary>Indique le type du Log: si c'est une erreur, une info, un message...</summary>
    ''' <remarks></remarks>
    Public Enum TypeLog
        INFO = 1                    'divers
        ACTION = 2                  'action lancé par un driver/device/trigger
        MESSAGE = 3
        VALEUR_CHANGE = 4           'Valeur ayant changé
        VALEUR_INCHANGE = 5         'Valeur n'ayant pas changé
        VALEUR_INCHANGE_PRECISION = 6 'Valeur n'ayant pas changé pour cause de precision
        VALEUR_INCHANGE_LASTETAT = 7 'Valeur n'ayant pas changé pour cause de lastetat
        ERREUR = 8                   'erreur générale
        ERREUR_CRITIQUE = 9          'erreur critique demandant la fermeture du programme
        DEBUG = 10                   'visible uniquement si Homidom est en mode debug
    End Enum

    ''' <summary>Indique la source du log si c'est le serveur, un script, un device...</summary>
    ''' <remarks></remarks>
    Public Enum TypeSource
        SERVEUR = 1
        SCRIPT = 2
        TRIGGER = 3
        DEVICE = 4
        DRIVER = 5
        SOAP = 6
        CLIENT = 7
    End Enum

    ''' <summary>Ecrit un log dans le fichier log au format xml</summary>
    ''' <param name="TypLog"></param>
    ''' <param name="Source"></param>
    ''' <param name="Fonction"></param>
    ''' <param name="Message"></param>
    ''' <remarks></remarks>
    Public Sub Log(ByVal TypLog As TypeLog, ByVal Source As TypeSource, ByVal Fonction As String, ByVal Message As String)
        Try

            'écriture dans un fichier texte
            _File = _MonRepertoire & "\logs\log_" & DateAndTime.Now.ToString("yyyyMMdd") & ".txt"
            Dim FreeF As Integer
            Dim texte As String = Now & vbTab & TypLog.ToString & vbTab & Source.ToString & vbTab & Fonction & vbTab & Message

            Try
                FreeF = FreeFile()
                texte = Replace(texte, vbLf, vbCrLf)
                SyncLock lock_logwrite
                    FileOpen(FreeF, _File, OpenMode.Append)
                    Print(FreeF, texte & vbCrLf)
                    FileClose(FreeF)
                End SyncLock
            Catch ex As IOException
                'wait(500)
                Console.WriteLine(Now & " " & TypLog & " CLIENT WPF LOG ERROR IOException : " & ex.ToString)
            Catch ex As Exception
                'wait(500)
                Console.WriteLine(Now & " " & TypLog & " CLIENT WPF LOG ERROR Exception : " & ex.ToString)
            End Try
            texte = Nothing
            FreeF = Nothing

        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'écriture d'un log: " & ex.Message, MsgBoxStyle.Exclamation, "Erreur Client WPF")
        End Try
    End Sub

    ''' <summary>Créer nouveau Fichier (donner chemin complet et nom) log</summary>
    ''' <param name="NewFichier"></param>
    ''' <remarks></remarks>
    Public Sub CreateNewFileLog(ByVal NewFichier As String)
        Try
            Dim rw As XmlTextWriter = New XmlTextWriter(NewFichier, Nothing)
            rw.WriteStartDocument()
            rw.WriteStartElement("logs")
            rw.WriteStartElement("log")
            rw.WriteAttributeString("time", Now)
            rw.WriteAttributeString("type", 0)
            rw.WriteAttributeString("source", 0)
            rw.WriteAttributeString("message", "Création du nouveau fichier log")
            rw.WriteEndElement()
            rw.WriteEndElement()
            rw.WriteEndDocument()
            rw.Close()
        Catch ex As Exception
            MessageBox.Show("Erreur CreateNewFileLog: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
#End Region

#Region "Connexion"
    'Connexion au serveur Homdidom
    Private Sub ConnectToHomidom()
        Dim myChannelFactory As ServiceModel.ChannelFactory(Of HoMIDom.HoMIDom.IHoMIDom) = Nothing

        Try
            Dim myadress As String = "http://" & IP & ":" & PortSOAP & "/ServiceModelSamples/service"
            Dim binding As New ServiceModel.BasicHttpBinding
            binding.MaxBufferPoolSize = 5000000
            binding.MaxReceivedMessageSize = 5000000
            binding.MaxBufferSize = 5000000
            binding.ReaderQuotas.MaxArrayLength = 5000000
            binding.ReaderQuotas.MaxStringContentLength = 5000000
            binding.SendTimeout = TimeSpan.FromMinutes(60)
            binding.CloseTimeout = TimeSpan.FromMinutes(60)
            binding.OpenTimeout = TimeSpan.FromMinutes(60)
            binding.ReceiveTimeout = TimeSpan.FromMinutes(60)

            myChannelFactory = New ServiceModel.ChannelFactory(Of HoMIDom.HoMIDom.IHoMIDom)(binding, New System.ServiceModel.EndpointAddress(myadress))
            myService = myChannelFactory.CreateChannel()
            Try
                myService.GetServerVersion()
                IsConnect = True

                MnuMacro.Items.Clear()
                For Each _mac As Macro In myService.GetAllMacros(IdSrv)
                    Dim mnu As New MenuItem
                    mnu.Tag = _mac.ID
                    mnu.Header = _mac.Nom
                    AddHandler mnu.Click, AddressOf MnuExecuteMacro
                    MnuMacro.Items.Add(mnu)
                Next

                MnuLastError.Items.Clear()
                Dim list As List(Of String) = myService.GetLastLogsError
                If list.Count > 0 Then
                    Dim _tool As String = ""
                    For Each logerror As String In list
                        If logerror <> "" And logerror <> " " Then
                            Dim mnu As New MenuItem
                            mnu.Header = logerror
                            mnu.FontSize = 10
                            MnuLastError.Items.Add(mnu)
                        End If
                    Next
                End If
                list = Nothing

            Catch ex As Exception
                IsConnect = False
            End Try

        Catch ex As Exception
            myChannelFactory.Abort()
            IsConnect = False
        End Try
    End Sub
#End Region

    'Creation  du menu
    Private Sub NewBtnMnu(ByVal Label As String, ByVal type As uCtrlImgMnu.TypeOfMnu, Optional ByVal Parametres As List(Of String) = Nothing, Optional ByVal Defaut As Boolean = False, Optional ByVal Tag As String = "", Optional ByVal Icon As String = "", Optional ByVal IdElement As String = "", Optional ByVal Visible As Boolean = False)
        Try
            Dim ctrl As New uCtrlImgMnu
            ctrl.Type = type
            ctrl.Defaut = Defaut
            ctrl.Id = HoMIDom.HoMIDom.Api.GenerateGUID
            ctrl.Text = Label
            ctrl.Tag = Tag
            ctrl.Icon = Icon
            ctrl.Parametres = Parametres
            ctrl.IDElement = IdElement
            ctrl.Visible = Visible
            AddHandler ctrl.click, AddressOf IconMnuDoubleClick
            _ListMnu.Add(ctrl)
            If type = uCtrlImgMnu.TypeOfMnu.Zone Then
                If Visible = True Then imgStackPnl.Children.Add(ctrl)
            Else
                imgStackPnl.Children.Add(ctrl)
            End If

        Catch ex As Exception
            Log(TypeLog.INFO, TypeSource.CLIENT, "NewBtnMnu", "Erreur NewBtnMnu: " & ex.Message)
            MessageBox.Show("Erreur lors de la création du bouton menu: " & ex.Message)
        End Try
    End Sub

    'Affiche la date et heure, heures levé et couché du soleil
    Public Sub dispatcherTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Me.UpdateLayout()

            If IsConnect Then
                Dim mytime As String = myService.GetTime
                LblTime.Content = Now.ToLongDateString & " " & mytime & " "

                If ShowSoleil = True Then
                    Dim mydate As Date
                    mydate = myService.GetHeureLeverSoleil
                    LblLeve.Content = mydate.ToShortTimeString
                    mydate = myService.GetHeureCoucherSoleil
                    LblCouche.Content = mydate.ToShortTimeString
                    mydate = Nothing
                End If
            End If
        Catch ex As Exception
            IsConnect = False
            If FlagMsgDeconnect = False Then
                MessageBox.Show("La communication a été perdue avec le serveur, veuillez vérifier que celui-ci est toujours actif et redémarrer le client", "ERREUR", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                FlagMsgDeconnect = True
            End If
            Log(TypeLog.INFO, TypeSource.CLIENT, "DispatcherTimer", "DispatcherTimer: " & ex.Message)
            LblTime.Content = Now.ToLongDateString & " " & Now.ToShortTimeString
            MessageBox.Show("Erreur: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Clic sur un menu de la barre du bas
    Private Sub IconMnuDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Try
            Me.Cursor = Cursors.Wait
            ' If Canvas1.Children.Count > 0 And sender.Type <> uCtrlImgMnu.TypeOfMnu.LecteurMedia Then
            Canvas1.Children.Clear()

            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()

            Me.UpdateLayout()

            '  End If

            Chk1.Visibility = Windows.Visibility.Collapsed
            Chk2.Visibility = Windows.Visibility.Collapsed
            Chk3.Visibility = Windows.Visibility.Collapsed

            Dim y As uCtrlImgMnu = sender

            If y IsNot Nothing Then
                Select Case y.Type
                    Case uCtrlImgMnu.TypeOfMnu.Internet
                        ImageBackGround = _ImageBackGroundDefault
                        Dim x As New uInternet(y.Parametres(0))
                        Canvas1.Children.Add(x)
                        x.Width = Canvas1.ActualWidth
                        x.Height = Canvas1.ActualHeight
                    Case uCtrlImgMnu.TypeOfMnu.Config
                    Case uCtrlImgMnu.TypeOfMnu.LecteurMedia
                        Dim x As New WMedia
                        x.Owner = Me
                        x.ShowDialog()
                    Case uCtrlImgMnu.TypeOfMnu.Meteo
                        ImageBackGround = _ImageBackGroundDefault
                        Dim x As New uMeteos
                        Canvas1.Children.Add(x)
                        x.Width = Canvas1.ActualWidth
                        x.Height = Canvas1.ActualHeight
                    Case uCtrlImgMnu.TypeOfMnu.Zone
                        ShowZone(y.IDElement)
                End Select
            End If

            y = Nothing
            Me.UpdateLayout()
            Me.Cursor = Nothing
        Catch ex As Exception
            MessageBox.Show("Erreur IconMnuDoubleClick: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
            Log(TypeLog.INFO, TypeSource.CLIENT, "IconMnuDoubleClick", "Erreur: " & ex.Message)
        End Try
    End Sub

    'Element demande afficher zone
    Private Sub ElementShowZone(ByVal Zoneid As String)
        Try
            Canvas1.Children.Clear()

            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()

            Me.UpdateLayout()

            ShowZone(Zoneid)
        Catch ex As Exception
            MessageBox.Show("Erreur ElementShowZone: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
            Log(TypeLog.INFO, TypeSource.CLIENT, "ElementShowZone", "Erreur: " & ex.Message)
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        Try
            MyBase.Finalize()
        Catch ex As Exception
            MessageBox.Show("Erreur Finalize Window1: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub Window1_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        Try
            Log(TypeLog.INFO, TypeSource.CLIENT, "Client", "Fermeture de l'application")
        Catch ex As Exception
            Log(TypeLog.INFO, TypeSource.CLIENT, "Client", "Erreur Lors de la fermeture: " & ex.Message)
        End Try
    End Sub

    Private Sub ScrollViewer1_PreviewMouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ScrollViewer1.PreviewMouseDown
        scrollStartPoint = e.GetPosition(Me)
        scrollStartOffset.X = ScrollViewer1.HorizontalOffset
    End Sub

    Private Sub ScrollViewer1_PreviewMouseMove(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles ScrollViewer1.PreviewMouseMove
        If e.LeftButton = MouseButtonState.Pressed Then
            Dim currentPoint As Point = e.GetPosition(Me)
            Dim delta As New Point(scrollStartPoint.X - currentPoint.X, scrollStartPoint.Y - currentPoint.Y)
            scrollTarget.X = scrollStartOffset.X + delta.X
            ScrollToPosition(ScrollViewer1, scrollTarget.X, currentPoint.Y, m_SpeedTouch)
        End If
    End Sub

    'Bouton Quitter
    Private Sub BtnQuit_Click_1(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnQuit.Click
        Try
            SaveConfig(_MonRepertoireAppData & "\config\HoMIWpF.xml")
            Log(TypeLog.INFO, TypeSource.CLIENT, "Client", "Fermture de l'application")
            End
        Catch ex As Exception
            MessageBox.Show("Erreur BtnQuit_Click_1: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
            End
        End Try
    End Sub

    Private Sub LoadZones()

        Dim cntNewZone As Integer = 0
        For i As Integer = 0 To myService.GetAllZones(IdSrv).Count - 1
            Dim x As Zone = myService.GetAllZones(IdSrv).Item(i)
            If ListMnu.Count = 0 Then
                NewBtnMnu(x.Name, uCtrlImgMnu.TypeOfMnu.Zone, , False, , , x.ID)
                cntNewZone += 1
            Else
                Dim flagexist As Boolean = False
                For j As Integer = 0 To ListMnu.Count - 1
                    If ListMnu.Item(j).IDElement = x.ID Then
                        flagexist = True
                    End If
                Next
                If flagexist = False Then
                    NewBtnMnu(x.Name, uCtrlImgMnu.TypeOfMnu.Zone, , False, , , x.ID, True)
                    cntNewZone += 1
                End If
            End If
        Next
        If cntNewZone > 0 Then
            MessageBox.Show(cntNewZone & " nouvelle(s) zone(s) ajoutée(s)", "Information", MessageBoxButton.OK, MessageBoxImage.Information)
        End If

    End Sub
    Public Sub ShowZone(ByVal IdZone As String)
        Try
            'Gestion de l'erreur si le serveur n'est pas connecté
            If IsConnect = False Then
                MessageBox.Show("Le serveur Homidom n'est pas connecté, impossible d'afficher les éléments de la zone sélectionnée", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Exit Sub
            End If

            'Déclaration des variables
            Dim _zone As Zone
            Dim _flagTrouv As Boolean = False
            Dim _Left As Double = 0
            Dim _Top As Double = 20
            Dim _idx As Integer = 0

            'Init des variables communes
            _CurrentIdZone = IdZone
            _zone = myService.ReturnZoneByID(IdSrv, IdZone)
            ImgBackground.Source = Nothing
            If _zone.Image.Contains("Zone_Image.png") = True Then
                ImageBackGround = _MonRepertoire & "\Images\Fond-defaut.png"
            Else
                ImgBackground.Source = ConvertArrayToImage(myService.GetByteFromImage(_zone.Image))
            End If

            'Desgin
            Chk1.Visibility = Windows.Visibility.Visible
            Chk2.Visibility = Windows.Visibility.Visible
            Chk3.Visibility = Windows.Visibility.Visible
            If Chk1.IsChecked = True Then Chk1.IsChecked = False
            If Chk2.IsChecked = True Then Chk2.IsChecked = False

            'On parcours tous les éléments de la zone (hors widgets empty)
            For i As Integer = 0 To _zone.ListElement.Count - 1
                Dim z As Zone.Element_Zone = myService.ReturnZoneByID(IdSrv, IdZone).ListElement.Item(i)

                'l'élément est définit comme visible dans la zone
                If z.Visible = True Then
                    For j As Integer = 0 To _ListElement.Count - 1
                        If _ListElement.Item(j).Id = z.ElementID And _ListElement.Item(j).ZoneId = IdZone And _ListElement.Item(j).IsEmpty = False Then
                            'Ajouter un nouveau Control
                            Dim x As New ContentControl
                            Dim Trg As New TransformGroup
                            Dim Rot As New RotateTransform(_ListElement.Item(j).Rotation)

                            Trg.Children.Add(Rot)
                            x.Width = _ListElement.Item(j).Width
                            x.Height = _ListElement.Item(j).Height
                            x.RenderTransform = Trg
                            x.Style = mybuttonstyle
                            x.Tag = True
                            x.Uid = _ListElement.Item(j).Uid

                            Dim y As New uWidgetEmpty
                            y.Show = True
                            y.Visibility = Windows.Visibility.Visible
                            y.Uid = _ListElement.Item(j).Uid
                            y.Id = z.ElementID
                            y.Type = uWidgetEmpty.TypeOfWidget.Device
                            y.CanEditValue = _ListElement.Item(j).CanEditValue
                            y.ZoneId = _ListElement.Item(j).ZoneId
                            y.Width = x.Width
                            y.Height = x.Height
                            y.X = _ListElement.Item(j).X
                            y.Y = _ListElement.Item(j).Y
                            y.Rotation = _ListElement.Item(j).Rotation
                            y.IsEmpty = _ListElement.Item(j).IsEmpty
                            y.ShowEtiquette = _ListElement.Item(j).ShowEtiquette
                            y.ShowStatus = _ListElement.Item(j).ShowStatus
                            y.ShowPicture = _ListElement.Item(j).ShowPicture
                            y.Picture = _ListElement.Item(j).Picture
                            y.Etiquette = _ListElement.Item(j).Etiquette
                            y.DefautLabelStatus = _ListElement.Item(j).DefautLabelStatus
                            y.TailleStatus = _ListElement.Item(j).TailleStatus
                            y.TailleEtiquette = _ListElement.Item(j).TailleEtiquette
                            y.ColorBackGround = _ListElement.Item(j).ColorBackGround
                            y.ColorStatus = _ListElement.Item(j).ColorStatus
                            y.ColorEtiquette = _ListElement.Item(j).ColorEtiquette
                            y.IsHitTestVisible = True 'True:bouge pas False:Bouge
                            AddHandler y.ShowZone, AddressOf ElementShowZone
                            x.Content = y
                            Canvas1.Children.Add(x)
                            Canvas.SetLeft(x, _ListElement.Item(j).X)
                            Canvas.SetTop(x, _ListElement.Item(j).Y)

                            _flagTrouv = True
                            x = Nothing
                            y = Nothing
                            Exit For
                        End If

                    Next

                    'Nouvel élément qui n'existe pas dans la config
                    If (_flagTrouv = False) And z.ElementID.Length > 1 Then

                        'Ajouter un nouveau Control
                        Dim x As New ContentControl
                        x.Width = 140
                        x.Height = 60
                        x.Style = mybuttonstyle
                        x.Tag = True
                        x.Uid = System.Guid.NewGuid.ToString()

                        If _idx = 5 Then
                            _idx = 0
                            _Top += 70
                        End If
                        _Left = (150 * _idx) + 40

                        'Ajoute l'élément dans la liste
                        Dim elmt As New uWidgetEmpty
                        elmt.Show = True
                        elmt.Id = z.ElementID
                        elmt.Uid = x.Uid
                        elmt.ZoneId = IdZone
                        elmt.Width = x.Width
                        elmt.Height = x.Height
                        elmt.Rotation = 0
                        elmt.X = _Left
                        elmt.Y = _Top
                        elmt.ZoneId = IdZone
                        elmt.IsEmpty = False
                        elmt.Type = uWidgetEmpty.TypeOfWidget.Device
                        elmt.ColorBackGround = New SolidColorBrush(Color.FromArgb(127, 80, 80, 80))
                        elmt.TailleStatus = 20
                        ' y = elmt
                        elmt.IsHitTestVisible = True
                        AddHandler elmt.ShowZone, AddressOf ElementShowZone
                        x.Content = elmt
                        _ListElement.Add(elmt)

                        Canvas1.Children.Add(x)
                        Canvas.SetLeft(x, _Left)
                        Canvas.SetTop(x, _Top)

                        _idx += 1
                        x = Nothing
                        elmt = Nothing
                    End If

                End If
                _flagTrouv = False
            Next

            'On va afficher tous les widgets empty
            For i As Integer = 0 To _ListElement.Count - 1
                If _ListElement.Item(i).ZoneId = IdZone And _ListElement.Item(i).IsEmpty = True Then
                    'Ajouter un nouveau Control
                    Dim x As New ContentControl
                    Dim Trg As New TransformGroup
                    Dim Rot As New RotateTransform(_ListElement.Item(i).Rotation)

                    Trg.Children.Add(Rot)
                    x.Width = _ListElement.Item(i).Width
                    x.Height = _ListElement.Item(i).Height
                    x.RenderTransform = Trg
                    x.Style = mybuttonstyle
                    x.Tag = True
                    x.Uid = _ListElement.Item(i).Uid

                    Dim y As New uWidgetEmpty
                    y.Show = True
                    y.Uid = _ListElement.Item(i).Uid
                    y.ZoneId = _ListElement.Item(i).ZoneId
                    y.Width = x.Width
                    y.Height = x.Height
                    y.X = _ListElement.Item(i).X
                    y.Y = _ListElement.Item(i).Y
                    y.Rotation = _ListElement.Item(i).Rotation
                    y.IsEmpty = _ListElement.Item(i).IsEmpty
                    y.Type = _ListElement.Item(i).Type
                    y.CanEditValue = _ListElement.Item(i).CanEditValue
                    y.Picture = _ListElement.Item(i).Picture
                    y.ShowPicture = _ListElement.Item(i).ShowPicture
                    y.ShowEtiquette = _ListElement.Item(i).ShowEtiquette
                    y.ShowStatus = _ListElement.Item(i).ShowStatus
                    y.Etiquette = _ListElement.Item(i).Etiquette
                    y.DefautLabelStatus = _ListElement.Item(i).DefautLabelStatus
                    y.TailleStatus = _ListElement.Item(i).TailleStatus
                    y.TailleEtiquette = _ListElement.Item(i).TailleEtiquette
                    y.ColorBackGround = _ListElement.Item(i).ColorBackGround
                    y.ColorEtiquette = _ListElement.Item(i).ColorEtiquette
                    y.ColorStatus = _ListElement.Item(i).ColorStatus
                    y.IsHitTestVisible = True 'True:bouge pas False:Bouge
                    y.Action_GestureBasHaut = _ListElement.Item(i).Action_GestureBasHaut
                    y.Action_GestureDroiteGauche = _ListElement.Item(i).Action_GestureDroiteGauche
                    y.Action_GestureGaucheDroite = _ListElement.Item(i).Action_GestureGaucheDroite
                    y.Action_GestureHautBas = _ListElement.Item(i).Action_GestureHautBas
                    y.Action_On_Click = _ListElement.Item(i).Action_On_Click
                    y.Action_On_LongClick = _ListElement.Item(i).Action_On_LongClick
                    y.Visuel = _ListElement.Item(i).Visuel
                    y.URL = _ListElement.Item(i).URL
                    y.HttpRefresh = _ListElement.Item(i).HttpRefresh
                    y.UrlRss = _ListElement.Item(i).UrlRss
                    y.ListHttpButton = _ListElement.Item(i).ListHttpButton
                    y.IDMeteo = _ListElement.Item(i).IDMeteo
                    y.IDKeyPad = _ListElement.Item(i).IDKeyPad
                    y.ShowPassWord = _ListElement.Item(i).ShowPassWord
                    y.ClearAfterEnter = _ListElement.Item(i).ClearAfterEnter
                    y.ShowClavier = _ListElement.Item(i).ShowClavier
                    AddHandler y.ShowZone, AddressOf ElementShowZone
                    x.Content = y
                    Canvas1.Children.Add(x)
                    Canvas.SetLeft(x, _ListElement.Item(i).X)
                    Canvas.SetTop(x, _ListElement.Item(i).Y)

                    x = Nothing
                    y = Nothing
                End If
            Next


        Catch ex As Exception
            MessageBox.Show("Erreur ShowZone: " & ex.ToString, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub Deplacement_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Chk1.Click
        Try
            'Mode déplacement
            If Chk1.IsChecked = True Then
                Chk2.IsChecked = False
                Design = True
                Dim child As ContentControl
                For Each child In Canvas1.Children
                    Dim obj As uWidgetEmpty = child.Content
                    obj.ModeEdition = True
                    obj.IsHitTestVisible = False
                    obj = Nothing
                    Selector.SetIsSelected(child, True)
                    AddHandler child.SizeChanged, AddressOf Resize
                Next
            Else
                'On a finit le déplacement
                Design = False
                Dim a As String = ""
                Dim child As ContentControl
                For Each child In Canvas1.Children
                    Dim obj As uWidgetEmpty = child.Content
                    obj.IsHitTestVisible = False
                    obj.ModeEdition = False
                    obj = Nothing
                    RemoveHandler child.SizeChanged, AddressOf Resize
                    Selector.SetIsSelected(child, False)

                    For j As Integer = 0 To _ListElement.Count - 1
                        If _ListElement.Item(j).Uid = child.Uid And _ListElement.Item(j).ZoneId = _CurrentIdZone Then
                            _ListElement.Item(j).X = CType(Canvas.GetLeft(child), Double)
                            _ListElement.Item(j).Y = CType(Canvas.GetTop(child), Double)
                            _ListElement.Item(j).Width = child.Width
                            _ListElement.Item(j).Height = child.Height

                            If InStr(child.RenderTransform.GetType.ToString, "TransformGroup") > 0 Then
                                Dim gt As TransformGroup = child.RenderTransform '.GetValue(RotateTransform.AngleProperty)
                                For k = 0 To gt.Children.Count - 1
                                    If InStr(LCase(gt.Children.Item(k).GetType.ToString), "rotatetransform") > 0 Then
                                        Dim rt As RotateTransform = gt.Children.Item(k)
                                        If rt IsNot Nothing Then
                                            _ListElement.Item(j).Rotation = rt.Angle
                                        End If
                                        Exit For
                                    End If
                                Next
                            End If
                            If InStr(child.RenderTransform.GetType.ToString, "RotateTransform") > 0 Then
                                _ListElement.Item(j).Rotation = child.RenderTransform.GetValue(RotateTransform.AngleProperty)
                            End If
                        End If
                    Next

                    Dim lbl As uWidgetEmpty = child.Content
                    lbl.IsHitTestVisible = True

                Next
            End If

            Me.UpdateLayout()
        Catch ex As Exception
            MessageBox.Show("Erreur Chk1: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub Resize(ByVal sender As Object, ByVal e As System.Windows.SizeChangedEventArgs)
        Try
            If sender.Content IsNot Nothing Then
                Dim obj As uWidgetEmpty = sender.Content
                obj.Width = e.NewSize.Width
                obj.Height = e.NewSize.Height
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur Resize: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub ModeEdition_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Chk2.Click

        Try

            If Chk2.IsChecked = True Then
                Chk1.IsChecked = False

                'On a finit le déplacement
                Design = False
                Dim a As String = ""
                Chk2.IsChecked = True
                Dim child As ContentControl
                For Each child In Canvas1.Children
                    Dim obj As uWidgetEmpty = child.Content
                    obj.IsHitTestVisible = False
                    obj.ModeEdition = False
                    obj = Nothing

                    Selector.SetIsSelected(child, False)

                    For j As Integer = 0 To _ListElement.Count - 1
                        If _ListElement.Item(j).Uid = child.Uid And _ListElement.Item(j).ZoneId = _CurrentIdZone Then
                            _ListElement.Item(j).X = CType(Canvas.GetLeft(child), Double)
                            _ListElement.Item(j).Y = CType(Canvas.GetTop(child), Double)
                            _ListElement.Item(j).Width = child.Width
                            _ListElement.Item(j).Height = child.Height

                            If InStr(child.RenderTransform.GetType.ToString, "TransformGroup") > 0 Then
                                Dim gt As TransformGroup = child.RenderTransform '.GetValue(RotateTransform.AngleProperty)
                                For k = 0 To gt.Children.Count - 1
                                    If InStr(LCase(gt.Children.Item(k).GetType.ToString), "rotatetransform") > 0 Then
                                        Dim rt As RotateTransform = gt.Children.Item(k)
                                        If rt IsNot Nothing Then
                                            _ListElement.Item(j).Rotation = rt.Angle
                                        End If
                                        Exit For
                                    End If
                                Next
                            End If
                            If InStr(child.RenderTransform.GetType.ToString, "RotateTransform") > 0 Then
                                _ListElement.Item(j).Rotation = child.RenderTransform.GetValue(RotateTransform.AngleProperty)
                            End If
                        End If
                    Next

                    Dim lbl As uWidgetEmpty = child.Content
                    lbl.IsHitTestVisible = True
                    lbl = Nothing
                Next
            End If

            Dim child2 As ContentControl
            For Each child2 In Canvas1.Children
                Dim obj As uWidgetEmpty = child2.Content
                obj.ModeEdition = Chk2.IsChecked
                obj = Nothing
            Next

            Me.UpdateLayout()
        Catch ex As Exception
            MessageBox.Show("Erreur: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub MaJ_Element(Optional ByVal Objet As Object = Nothing)
        Try
            Dim a As String = ""
            Dim child As ContentControl
            For Each child In Canvas1.Children
                Dim obj As uWidgetEmpty = child.Content
                If Objet IsNot Nothing Then
                    obj = Objet.Content
                End If
                obj.IsHitTestVisible = False
                obj.ModeEdition = False
                obj = Nothing

                Selector.SetIsSelected(child, False)

                For j As Integer = 0 To _ListElement.Count - 1
                    If _ListElement.Item(j).Id = child.Uid And _ListElement.Item(j).ZoneId = _CurrentIdZone Then
                        _ListElement.Item(j).X = CType(Canvas.GetLeft(child), Double)
                        _ListElement.Item(j).Y = CType(Canvas.GetTop(child), Double)
                        _ListElement.Item(j).Width = child.Width
                        _ListElement.Item(j).Height = child.Height

                        If InStr(child.RenderTransform.GetType.ToString, "TransformGroup") > 0 Then
                            Dim gt As TransformGroup = child.RenderTransform '.GetValue(RotateTransform.AngleProperty)
                            For k = 0 To gt.Children.Count - 1
                                If InStr(LCase(gt.Children.Item(k).GetType.ToString), "rotatetransform") > 0 Then
                                    Dim rt As RotateTransform = gt.Children.Item(k)
                                    If rt IsNot Nothing Then
                                        _ListElement.Item(j).Rotation = rt.Angle 'child.RenderTransform.GetValue(RotateTransform.AngleProperty)
                                    End If
                                    Exit For
                                End If
                            Next
                        End If
                        If InStr(child.RenderTransform.GetType.ToString, "RotateTransform") > 0 Then
                            _ListElement.Item(j).Rotation = child.RenderTransform.GetValue(RotateTransform.AngleProperty)
                        End If
                    End If
                Next

                Dim lbl As uWidgetEmpty = child.Content
                lbl.IsHitTestVisible = True
            Next
        Catch ex As Exception
            MessageBox.Show("Erreur Resize: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub NewWidgetEmpty_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles NewWidgetEmpty.Click
        Try
            ' Remettre à zéro les modes édition + déplacement
            Chk1.IsChecked = False
            Chk2.IsChecked = False
            Deplacement_Click(Me, e)

            'Ajouter un nouveau Control
            Dim x As New ContentControl
            x.Width = 100
            x.Height = 100
            x.Style = mybuttonstyle
            x.Tag = True
            x.Uid = System.Guid.NewGuid.ToString()

            'Ajoute l'élément dans la liste
            Dim elmt As New uWidgetEmpty
            elmt.Show = True
            elmt.Uid = x.Uid
            elmt.ZoneId = _CurrentIdZone
            elmt.Width = 100
            elmt.Height = 100
            elmt.Rotation = 0
            elmt.X = 100 + RandomNumber.Next(0, 200)
            elmt.Y = 100 + RandomNumber.Next(0, 200)
            elmt.IsEmpty = True
            elmt.Type = uWidgetEmpty.TypeOfWidget.Empty
            elmt.ShowStatus = False
            elmt.Etiquette = "Widget " & Canvas1.Children.Count + 1
            elmt.Visibility = Windows.Visibility.Visible
            elmt.ColorBackGround = New SolidColorBrush(Color.FromArgb(127, 80, 80, 80))
            elmt.TailleStatus = 20
            _ListElement.Add(elmt)

            elmt.IsHitTestVisible = True 'True:bouge pas False:Bouge
            x.Content = elmt
            Canvas1.Children.Add(x)
            Canvas.SetLeft(x, elmt.X)
            Canvas.SetTop(x, elmt.Y)

        Catch ex As Exception
            MessageBox.Show("Erreur NewWidgetEmpty: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub NewWidgetWeb_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles NewWidgetWeb.Click
        Try
            ' Remettre à zéro les modes édition + déplacement
            Chk1.IsChecked = False
            Chk2.IsChecked = False
            Deplacement_Click(Me, e)

            'Ajouter un nouveau Control
            Dim x As New ContentControl
            x.Width = 100
            x.Height = 100
            x.Style = mybuttonstyle
            x.Tag = True
            x.Uid = System.Guid.NewGuid.ToString()

            'Ajoute l'élément dans la liste
            Dim elmt As New uWidgetEmpty
            elmt.Show = True
            elmt.Uid = x.Uid
            elmt.ZoneId = _CurrentIdZone
            elmt.Width = 100
            elmt.Height = 100
            elmt.Rotation = 0
            elmt.X = 300
            elmt.Y = 300
            elmt.IsEmpty = True
            elmt.Type = uWidgetEmpty.TypeOfWidget.Web
            elmt.ColorBackGround = New SolidColorBrush(Color.FromArgb(255, 0, 0, 0))
            elmt.ShowStatus = False
            elmt.Etiquette = "Widget " & Canvas1.Children.Count + 1
            _ListElement.Add(elmt)

            elmt.IsHitTestVisible = True 'True:bouge pas False:Bouge
            x.Content = elmt
            Canvas1.Children.Add(x)
            Canvas.SetLeft(x, 300)
            Canvas.SetTop(x, 300)
        Catch ex As Exception
            MessageBox.Show("Erreur NewWidgetWeb: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub NewWidgetRss_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles NewWidgetRss.Click
        Try
            ' Remettre à zéro les modes édition + déplacement
            Chk1.IsChecked = False
            Chk2.IsChecked = False
            Deplacement_Click(Me, e)

            'Ajouter un nouveau Control
            Dim x As New ContentControl
            x.Width = 100
            x.Height = 100
            x.Style = mybuttonstyle
            x.Tag = True
            x.Uid = System.Guid.NewGuid.ToString()

            'Ajoute l'élément dans la liste
            Dim elmt As New uWidgetEmpty
            elmt.Show = True
            elmt.Uid = x.Uid
            elmt.ZoneId = _CurrentIdZone
            elmt.Width = 100
            elmt.Height = 100
            elmt.Rotation = 0
            elmt.X = 300
            elmt.Y = 300
            elmt.IsEmpty = True
            elmt.Type = uWidgetEmpty.TypeOfWidget.Rss
            elmt.ColorBackGround = New SolidColorBrush(Color.FromArgb(255, 0, 0, 0))
            elmt.ShowStatus = False
            elmt.Etiquette = "Widget " & Canvas1.Children.Count + 1
            _ListElement.Add(elmt)

            elmt.IsHitTestVisible = True 'True:bouge pas False:Bouge
            x.Content = elmt
            Canvas1.Children.Add(x)
            Canvas.SetLeft(x, 300)
            Canvas.SetTop(x, 300)

        Catch ex As Exception
            MessageBox.Show("Erreur NewWidgetRss: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub NewWidgetMeteo_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles NewWidgetMeteo.Click
        Try
            ' Remettre à zéro les modes édition + déplacement
            Chk1.IsChecked = False
            Chk2.IsChecked = False
            Deplacement_Click(Me, e)

            'Ajouter un nouveau Control
            Dim x As New ContentControl
            x.Width = 202
            x.Height = 193
            x.Style = mybuttonstyle
            x.Tag = True
            x.Uid = System.Guid.NewGuid.ToString()

            'Ajoute l'élément dans la liste
            Dim elmt As New uWidgetEmpty
            elmt.Show = True
            elmt.Uid = x.Uid
            elmt.ZoneId = _CurrentIdZone
            elmt.Width = 202
            elmt.Height = 193
            elmt.Rotation = 0
            elmt.X = 300
            elmt.Y = 300
            elmt.IsEmpty = True
            elmt.Type = uWidgetEmpty.TypeOfWidget.Meteo
            elmt.ShowStatus = False
            elmt.Etiquette = "Widget " & Canvas1.Children.Count + 1
            elmt.Visibility = Windows.Visibility.Visible
            elmt.ColorBackGround = New SolidColorBrush(Color.FromArgb(255, 0, 0, 0))
            _ListElement.Add(elmt)

            elmt.IsHitTestVisible = True 'True:bouge pas False:Bouge
            x.Content = elmt
            Canvas1.Children.Add(x)
            Canvas.SetLeft(x, 300)
            Canvas.SetTop(x, 300)

        Catch ex As Exception
            MessageBox.Show("Erreur NewWidgetMeteo: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub NewWidgetKeyPad_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles NewWidgetKeyPad.Click
        Try
            ' Remettre à zéro les modes édition + déplacement
            Chk1.IsChecked = False
            Chk2.IsChecked = False
            Deplacement_Click(Me, e)

            'Ajouter un nouveau Control
            Dim x As New ContentControl
            x.Width = 214
            x.Height = 287
            x.Style = mybuttonstyle
            x.Tag = True
            x.Uid = System.Guid.NewGuid.ToString()

            'Ajoute l'élément dans la liste
            Dim elmt As New uWidgetEmpty
            elmt.Show = True
            elmt.Uid = x.Uid
            elmt.ZoneId = _CurrentIdZone
            elmt.Width = 214
            elmt.Height = 287
            elmt.Rotation = 0
            elmt.X = 300
            elmt.Y = 300
            elmt.IsEmpty = True
            elmt.Type = uWidgetEmpty.TypeOfWidget.KeyPad
            elmt.ShowStatus = False
            elmt.Etiquette = "Widget " & Canvas1.Children.Count + 1
            elmt.ColorBackGround = New SolidColorBrush(Color.FromArgb(255, 0, 0, 0))
            elmt.Visibility = Windows.Visibility.Visible
            _ListElement.Add(elmt)

            elmt.IsHitTestVisible = True 'True:bouge pas False:Bouge
            x.Content = elmt
            Canvas1.Children.Add(x)
            Canvas.SetLeft(x, 300)
            Canvas.SetTop(x, 300)
        Catch ex As Exception
            MessageBox.Show("Erreur NewWidgetKeyPad: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub NewWidgetLabel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles NewWidgetLabel.Click
        Try
            ' Remettre à zéro les modes édition + déplacement
            Chk1.IsChecked = False
            Chk2.IsChecked = False
            Deplacement_Click(Me, e)

            'Ajouter un nouveau Control
            Dim x As New ContentControl
            x.Width = 100
            x.Height = 25
            x.Style = mybuttonstyle
            x.Tag = True
            x.Uid = System.Guid.NewGuid.ToString()

            'Ajoute l'élément dans la liste
            Dim elmt As New uWidgetEmpty
            elmt.Show = True
            elmt.Uid = x.Uid
            elmt.ZoneId = _CurrentIdZone
            elmt.Width = 100
            elmt.Height = 25
            elmt.Rotation = 0
            elmt.X = 300
            elmt.Y = 300
            elmt.IsEmpty = True
            elmt.Type = uWidgetEmpty.TypeOfWidget.Label
            elmt.ColorBackGround = New SolidColorBrush(Color.FromArgb(127, 80, 80, 80))
            elmt.ShowStatus = False
            elmt.Etiquette = "Widget " & Canvas1.Children.Count + 1
            _ListElement.Add(elmt)

            elmt.IsHitTestVisible = True 'True:bouge pas False:Bouge
            x.Content = elmt
            Canvas1.Children.Add(x)
            Canvas.SetLeft(x, 300)
            Canvas.SetTop(x, 300)
            Canvas.SetZIndex(x, 0)
        Catch ex As Exception
            MessageBox.Show("Erreur NewWidgetLabel_Click: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub NewWidgetCamera_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles NewWidgetCamera.Click
        Try
            ' Remettre à zéro les modes édition + déplacement
            Chk1.IsChecked = False
            Chk2.IsChecked = False
            Deplacement_Click(Me, e)

            'Ajouter un nouveau Control
            Dim x As New ContentControl
            x.Width = 640
            x.Height = 480
            x.Style = mybuttonstyle
            x.Tag = True
            x.Uid = System.Guid.NewGuid.ToString()

            'Ajoute l'élément dans la liste
            Dim elmt As New uWidgetEmpty
            elmt.Show = True
            elmt.Uid = x.Uid
            elmt.ZoneId = _CurrentIdZone
            elmt.Width = 640
            elmt.Height = 480
            elmt.Rotation = 0
            elmt.X = 300
            elmt.Y = 300
            elmt.IsEmpty = True
            elmt.Type = uWidgetEmpty.TypeOfWidget.Camera
            elmt.ShowStatus = False
            elmt.Etiquette = "Widget " & Canvas1.Children.Count + 1
            elmt.ColorBackGround = New SolidColorBrush(Color.FromArgb(255, 0, 0, 0))
            _ListElement.Add(elmt)

            elmt.IsHitTestVisible = True 'True:bouge pas False:Bouge
            x.Content = elmt
            Canvas1.Children.Add(x)
            Canvas.SetLeft(x, 300)
            Canvas.SetTop(x, 300)
        Catch ex As Exception
            MessageBox.Show("Erreur NewWidgetCamera: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

#Region "Menu"

    Private Sub ViewLog_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ViewLog.Click
        Try
            Me.Cursor = Cursors.Wait
            If Canvas1.Children.Count > 0 Then
                Canvas1.Children.Clear()
                Me.UpdateLayout()
            End If

            Chk1.Visibility = Windows.Visibility.Collapsed
            Chk2.Visibility = Windows.Visibility.Collapsed
            Chk3.Visibility = Windows.Visibility.Collapsed

            Dim x As New uLog
            x.Uid = System.Guid.NewGuid.ToString()
            AddHandler x.CloseMe, AddressOf UnloadControl
            x.Width = Canvas1.ActualWidth - 100
            x.Height = Canvas1.ActualHeight - 50
            Canvas1.Children.Add(x)

            Me.Cursor = Nothing
        Catch ex As Exception
            Me.Cursor = Nothing
            MessageBox.Show("Erreur ViewLog: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub ViewCalendar_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ViewCalendar.Click
        Try
            Dim x As New WCalendar
            x.Owner = Me
            x.ShowDialog()
        Catch ex As Exception
            MessageBox.Show("Erreur ViewCalendar:" & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub MnuHisto_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MnuHisto.Click
        Try
            Dim x As New uHisto(Nothing)
            x.Uid = System.Guid.NewGuid.ToString()
            x.Width = Canvas1.ActualWidth - 50
            x.Height = Canvas1.ActualHeight - 20
            AddHandler x.CloseMe, AddressOf UnloadControl
            Canvas1.Children.Clear()
            Me.UpdateLayout()
            Canvas1.Children.Add(x)
        Catch ex As Exception
            Me.Cursor = Nothing
            MessageBox.Show("Erreur MnuHisto: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub MnuExecuteMacro(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Try
            myService.RunMacro(IdSrv, sender.tag)
        Catch ex As Exception
            MessageBox.Show("Erreur MnuMacro_MouseDown: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub MnuMacro_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles MnuMacro.MouseDown
        Try
            MnuMacro.Items.Clear()
            For Each _mac As Macro In myService.GetAllMacros(IdSrv)
                Dim mnu As New MenuItem
                mnu.Tag = _mac.ID
                mnu.Header = _mac.Nom
                mnu.FontSize = 14
                AddHandler mnu.Click, AddressOf MnuExecuteMacro
                MnuMacro.Items.Add(mnu)
            Next
        Catch ex As Exception
            MessageBox.Show("Erreur MnuMacro_MouseDown: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub MnuConfig_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MnuConfig.Click
        Try
            Canvas1.Children.Clear()
            Me.UpdateLayout()
            ImageBackGround = _ImageBackGroundDefault
            Dim x As New WConfig(Me)
            x.Owner = Me
            x.ShowDialog()
            If x.DialogResult.HasValue And x.DialogResult.Value Then
                imgStackPnl.Children.Clear()
                For i As Integer = 0 To ListMnu.Count - 1
                    AddHandler ListMnu.Item(i).click, AddressOf IconMnuDoubleClick
                    If ListMnu.Item(i).Type = uCtrlImgMnu.TypeOfMnu.Zone Then
                        If ListMnu.Item(i).Visible = True Then imgStackPnl.Children.Add(ListMnu.Item(i))
                    Else
                        imgStackPnl.Children.Add(ListMnu.Item(i))
                    End If
                Next
                If x.ChkFullScreen.IsChecked = False Then
                    Me.WindowState = Windows.WindowState.Normal
                Else
                    Me.WindowState = Windows.WindowState.Maximized
                End If
                x.Close()
            Else
                x.Close()
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur MnuConfig_Click: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub MenuItem1_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuItem1.Click
        Try
            MnuLastError.Items.Clear()

            If IsConnect Then
                Dim list As List(Of String) = myService.GetLastLogsError
                If list.Count > 0 Then
                    Dim _tool As String = ""
                    For Each logerror As String In list
                        If logerror <> "" And logerror <> " " Then
                            Dim mnu As New MenuItem
                            mnu.Header = logerror
                            mnu.FontSize = 10
                            MnuLastError.Items.Add(mnu)
                        End If
                    Next
                End If
                list = Nothing
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur MenuItem1_Click: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
#End Region


    Private Sub Window1_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Try
            Me.Cursor = Nothing
        Catch ex As Exception
            MessageBox.Show("Erreur Window1_Loaded: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub


    Private Sub StkTop_MouseLeftButtonDown_1(sender As Object, e As MouseButtonEventArgs)

    End Sub

    Private Sub StkTop_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs) Handles StkTop.MouseLeftButtonDown
        Try
            DragMove()
        Catch ex As Exception
        End Try
    End Sub

End Class
