#Region "Imports"
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading
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
Imports System.Web.HttpUtility
Imports System.Windows.Controls.Primitives
Imports HoMIWpF.Designer
Imports HoMIWpF.Designer.ResizeRotateAdorner
Imports System.Windows.Media.Animation
#End Region


Class Window1

#Region "Variables"
    ' Used when manually scrolling.
    Private scrollTarget As Point
    Private scrollStartPoint As Point
    Private scrollStartOffset As Point
    Public imgStackPnl As New StackPanel()
    Dim FlagMsgDeconnect As Boolean = False
    Private Shared lock_logwrite As New Object
    Dim _flagIsShowScroll As Boolean = True
    Dim _MaskTaskMnu As Boolean = False

    'Paramètres de connexion à HomeSeer
    Dim _Serveur As String = ""
    Dim _Login As String = ""
    Dim _PasswordHS As String = ""
    Dim _PortSOAP As String = "7999"
    Dim _IP As String = "localhost"
    Dim _TimeoutSrvlive As Integer = 30000

    'XML
    Dim myxml As HoMIDom.HoMIDom.XML
    Dim list As XmlNodeList
    Dim _ConfigFile As String

    'Divers
    Dim _AutoSave As Boolean = True 'Sauvegarde automatique en quittant
    Dim _flagSave As Boolean = False 'True si première execution du client ou si on a changer un paramètre de config

    'User Graphic
    Dim mypush As PushNotification()
    Dim _ShowSoleil As Boolean
    Dim _ShowTemperature As Boolean
    Dim _ImageBackGroundDefault As String = ""
    Dim _ListMnu As New List(Of uCtrlImgMnu)
    Dim _Design As Boolean = False
    Dim _FullScreen As Boolean = True
    Dim mybuttonstyle As Style
    Public _CurrentIdZone As String = ""
    Dim RandomNumber As New Random
    Dim _ShowQuitter As Boolean = True
    Dim _SaveDiffBack As Boolean 'True si on veut créer un fichier de backup différent géré avec dateheure
    Dim _PassWord As String = ""
    Dim _WithPassword As Boolean = False
    Dim _AffLastError As Boolean = False
    Dim _Ville As String = "" 'Ville par défaut du client
    Dim _MousePosition As Point
    Dim _TimeMouseDown As DateTime = Now
    Dim _TimeOutPage As Integer = 1 'Timeout d'une page en minute
    Dim _DefautPage As String = "Aucune" 'Page par défaut à afficher après tiemout
    Dim _AsTimeOutPage As Boolean = False 'definit si on gère le timeout d'une page
    Dim _TaskMnuTransp As Byte = 99
    Dim _ShowLabelMnu As Boolean = False 'si true affiche le nom du menu sélectionné dans la barre du haut
    Dim _ShowDateTime As Boolean = True 'si true affiche l'heure du menu sélectionné dans la barre du haut
    Dim _ShowKeyBoard As Boolean = True 'True si on affiche le bouton clavier virtuel
    Dim _TranspBarHaut As Integer = 58
    Dim _TranspBarBas As Integer = 153
    Dim _KeyBoardPath As String = "osk"
    Dim _MaJWidgetFromServer As Boolean = True
    Dim _ShowTimeFromServer As Boolean = True 'Afficher l'heure du serveur ou si non celle du client
#End Region

#Region "Property"
    Public Property ShowTimeFromServer As Boolean
        Get
            Return _ShowTimeFromServer
        End Get
        Set(value As Boolean)
            _ShowTimeFromServer = False
        End Set
    End Property

    Public Property MaJWidgetFromServer As Boolean
        Get
            Return _MaJWidgetFromServer
        End Get
        Set(value As Boolean)
            _MaJWidgetFromServer = value
        End Set
    End Property

    Public Property KeyboardPath As String
        Get
            Return _KeyBoardPath
        End Get
        Set(value As String)
            _KeyBoardPath = value
        End Set
    End Property

    Public Property TransparenceHaut As Integer
        Get
            Return _TranspBarHaut
        End Get
        Set(value As Integer)
            If value < 0 Then value = 0
            If value > 255 Then value = 255
            _TranspBarHaut = value
            StkTop.Background = New SolidColorBrush(Color.FromArgb(_TranspBarHaut, 0, 0, 0))
        End Set
    End Property

    Public Property TransparenceBas As Integer
        Get
            Return _TranspBarBas
        End Get
        Set(value As Integer)
            If value < 0 Then value = 0
            If value > 255 Then value = 255
            _TranspBarBas = value
            ScrollViewer1.Background = New SolidColorBrush(Color.FromArgb(_TranspBarBas, 0, 0, 0))
        End Set
    End Property

    Public Property SaveDiffBackup As Boolean
        Get
            Return _SaveDiffBack
        End Get
        Set(value As Boolean)
            _SaveDiffBack = value
        End Set
    End Property

    Public Property FlagSave As Boolean
        Get
            Return _flagSave
        End Get
        Set(value As Boolean)
            _flagSave = value
        End Set
    End Property

    Public Property AutoSave As Boolean
        Get
            Return _AutoSave
        End Get
        Set(value As Boolean)
            _AutoSave = value
            'If _AutoSave Then
            '    MnuSave.Visibility = Windows.Visibility.Collapsed
            'Else
            MnuSave.Visibility = Windows.Visibility.Visible
            'End If
        End Set
    End Property

    Public Property ShowLabelMnu As Boolean
        Get
            Return _ShowLabelMnu
        End Get
        Set(ByVal value As Boolean)
            _ShowLabelMnu = value
            If _ShowLabelMnu Then
                LblZone.Visibility = Windows.Visibility.Visible
            Else
                LblZone.Visibility = Windows.Visibility.Collapsed
            End If
        End Set
    End Property

    Public Property ShowDateTime As Boolean
        Get
            Return _ShowDateTime
        End Get
        Set(ByVal value As Boolean)
            _ShowDateTime = value
            If _ShowDateTime Then
                LblTime.Visibility = Windows.Visibility.Visible
            Else
                LblTime.Visibility = Windows.Visibility.Collapsed
            End If
        End Set
    End Property

    Public Property Ville As String
        Get
            Return _Ville
        End Get
        Set(ByVal value As String)
            _Ville = value
        End Set
    End Property

    Public Property TaskMnuTranp As Byte
        Get
            Return _TaskMnuTransp
        End Get
        Set(ByVal value As Byte)
            _TaskMnuTransp = value

        End Set
    End Property

    Public Property AffLastError As Boolean
        Get
            Return _AffLastError
        End Get
        Set(ByVal value As Boolean)
            _AffLastError = value
            If _AffLastError Then
                ImgMess.Visibility = Windows.Visibility.Visible
            Else
                ImgMess.Visibility = Windows.Visibility.Collapsed
            End If
        End Set
    End Property

    Public Property WithPassword As Boolean
        Get
            Return _WithPassword
        End Get
        Set(ByVal value As Boolean)
            _WithPassword = value
        End Set
    End Property

    Public Property Password As String
        Get
            Return _PassWord
        End Get
        Set(ByVal value As String)
            _PassWord = value
        End Set
    End Property

    Public Property ShowQuitter As Boolean
        Get
            Return _ShowQuitter
        End Get
        Set(ByVal value As Boolean)
            _ShowQuitter = value
            If _ShowQuitter Then
                BtnQuit.Visibility = Windows.Visibility.Visible
            Else
                BtnQuit.Visibility = Windows.Visibility.Hidden
            End If
        End Set
    End Property

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
        End Set
    End Property

    Public Property MaskTaskMnu As Boolean
        Get
            Return _MaskTaskMnu
        End Get
        Set(ByVal value As Boolean)
            _MaskTaskMnu = value
            If value Then
                Aff_TaskMnu()
            Else
                DesAff_TaskMnu()
            End If
        End Set
    End Property

    Public Property ImageBackGround As String
        Get
            Return _ImageBackGroundDefault
        End Get
        Set(ByVal value As String)
            _ImageBackGroundDefault = value
            Try
                If String.IsNullOrEmpty(_ImageBackGroundDefault) = False Then
                    If File.Exists(_ImageBackGroundDefault) Then
                        ImgBackground.Source = LoadBitmapImage(_ImageBackGroundDefault)
                    Else
                        ImgBackground.Source = New BitmapImage(New Uri(_MonRepertoire & "\Images\Fond-logo.png"))
                        _ImageBackGroundDefault = _MonRepertoire & "\Images\Fond-logo.png"
                    End If
                Else
                    ImgBackground.Source = New BitmapImage(New Uri(_MonRepertoire & "\Images\Fond-logo.png"))
                    _ImageBackGroundDefault = _MonRepertoire & "\Images\Fond-logo.png"
                End If
            Catch ex As Exception
                ImgBackground.Source = Nothing
                _ImageBackGroundDefault = ""
            End Try
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

    Public Property TimeOutServerLive As Integer
        Get
            Return _TimeoutSrvlive
        End Get
        Set(value As Integer)
            _TimeoutSrvlive = value
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
            Return _PassWord
        End Get
        Set(ByVal value As String)
            _PassWord = value
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

    Public Property ConfigFile As String
        Get
            Return _ConfigFile
        End Get
        Set(ByVal value As String)
            _ConfigFile = value
        End Set
    End Property

    Public Property AsTimeOutPage As Boolean
        Get
            Return _AsTimeOutPage
        End Get
        Set(ByVal value As Boolean)
            _AsTimeOutPage = value
            If value Then _TimeMouseDown = Now
        End Set
    End Property

    Public Property DefautPage As String
        Get
            Return _DefautPage
        End Get
        Set(ByVal value As String)
            _DefautPage = value
        End Set
    End Property

    Public Property TimeOutPage As Integer
        Get
            Return _TimeOutPage
        End Get
        Set(ByVal value As Integer)
            _TimeOutPage = value
        End Set
    End Property

    Public Property ShowKeyboard As Boolean
        Get
            Return _ShowKeyBoard
        End Get
        Set(value As Boolean)
            _ShowKeyBoard = value

            If _ShowKeyBoard Then
                StkKeyboard.Visibility = Windows.Visibility.Visible
            Else
                StkKeyboard.Visibility = Windows.Visibility.Collapsed
            End If
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
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur UnloadControl: " & ex.Message, "Erreur", "UnloadControl")
        End Try
    End Sub

    Private Sub StkTop_MouseLeftButtonDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs) Handles StkTop.MouseLeftButtonDown
        Try
            Dim pt As System.Windows.Point
            pt = e.GetPosition(sender)
            If pt.X > 40 Then
                DragMove()
            End If

        Catch ex As Exception
        End Try
    End Sub

    Public Sub New()
        Try
            ' Cet appel est requis par le Concepteur Windows Form.
            InitializeComponent()

            Me.Cursor = Cursors.Wait

            'Affiche splash Screen
            Dim spl As Window2 = New Window2
            spl.Show()


            Dim mystyles As New ResourceDictionary()
            mystyles.Source = New Uri("/HoMIWpF;component/Resources/DesignerItem.xaml",
                    UriKind.RelativeOrAbsolute)
            mybuttonstyle = mystyles("DesignerItemStyle")


            'Cree les sous répertoires s'ils nexistent pas
            If System.IO.Directory.Exists(_MonRepertoire & "\Cache") = False Then
                System.IO.Directory.CreateDirectory(_MonRepertoire & "\Cache")
                Log(TypeLog.INFO, TypeSource.CLIENT, "Start", "Création du dossier Cache")
            End If
            If System.IO.Directory.Exists(_MonRepertoire & "\Cache\Images") = False Then
                System.IO.Directory.CreateDirectory(_MonRepertoire & "\Cache\Images")
                Log(TypeLog.INFO, TypeSource.CLIENT, "Start", "Création du dossier images")
            End If

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
            Dim s() As String = System.Environment.GetCommandLineArgs()
            If s.Count > 1 Then
                ' Utilisation du fichier de configuration spécifié à la ligne de commande
                _ConfigFile = s(1)
                If _ConfigFile.Contains("\") = False Then
                    _ConfigFile = _MonRepertoireAppData & "\Config\" & _ConfigFile
                End If
            Else
                _ConfigFile = _MonRepertoireAppData & "\Config\HoMIWpF.xml"
            End If
            Log(TypeLog.INFO, TypeSource.CLIENT, "LOADCONFIG", "Message: " & LoadConfig(_ConfigFile))
            s = Nothing

            ' Create StackPanel and set child elements to horizontal orientation
            imgStackPnl.HorizontalAlignment = HorizontalAlignment.Center
            imgStackPnl.VerticalAlignment = VerticalAlignment.Center
            imgStackPnl.Orientation = Orientation.Horizontal

            If IsConnect = True Then
                LoadZones()
            Else
                AfficheMessageAndLog(Fonctions.TypeLog.MESSAGE, "Pas de connexion au serveur. Veuillez entrer les informations de connexion dans l'onglet serveur de la fenêtre de configuration.", "Information", "New")
                _ImageBackGroundDefault = _MonRepertoire & "\Images\Fond-logo.png"
                Me.Show()
                MnuConfig_Click(Me, Nothing)
                SaveConfig(_ConfigFile)
                Log(TypeLog.INFO, TypeSource.CLIENT, "LOADCONFIG", "Message: " & LoadConfig(_ConfigFile))
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

            If IsConnect Then Refresh()
            'MaJ Liste des devices
            'Dim x As New Thread(AddressOf Refresh)
            'x.Priority = ThreadPriority.Highest
            'x.Start()
            'x = Nothing

            myxml = Nothing
            frmMere = Me

            If _AffLastError And IsConnect Then
                Dim _string As String = ""
                For Each logerror As String In myService.GetLastLogsError
                    If String.IsNullOrEmpty(logerror) = False Then
                        _string &= logerror & vbCrLf
                        ImgMess.ToolTip = _string
                    End If
                Next
            End If

            spl.Close()
            spl = Nothing
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur lors du lancement de l'application: " & ex.ToString, "Erreur", "New")
        End Try
    End Sub

    'Event lorsqu'un device change
    Private Sub DeviceChanged(deviceid As String, DeviceValue As String)
        Try
            If MaJWidgetFromServer Then Exit Sub

            Dim _devTrv As Boolean 'Flag si on a trouvé le device dans la liste

            If IsConnect Then
                Do While lock_dev
                    Thread.Sleep(100)
                Loop

                lock_dev = True
                For Each _dev In AllDevices
                    If _dev.ID = deviceid Then
                        Dim mydev As TemplateDevice = Nothing
                        mydev = myService.ReturnDeviceByID(IdSrv, deviceid)
                        If mydev IsNot Nothing Then
                            MaJ_Device(mydev, _dev)
                            _devTrv = True
                            Exit For
                        End If
                    End If
                Next

                If _devTrv = False Then AllDevices.Add(myService.ReturnDeviceByID(IdSrv, deviceid))

                lock_dev = False
            End If
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur Event DeviceChanged: " & ex.ToString, "Erreur", " Event DeviceChanged")
        End Try
    End Sub

    Private Sub MaJ_Device(Source As TemplateDevice, Destination As TemplateDevice)
        Try
            Destination.Type = Source.Type
            If Source.Type = Device.ListeDevices.METEO Then
                Destination.ConditionActuel = Source.ConditionActuel
                Destination.ConditionJ1 = Source.ConditionActuel
                Destination.ConditionJ2 = Source.ConditionJ2
                Destination.ConditionJ3 = Source.ConditionJ3
                Destination.ConditionToday = Source.ConditionToday
                Destination.IconJ1 = Source.IconJ1
                Destination.IconJ2 = Source.IconJ2
                Destination.IconJ3 = Source.IconJ3
                Destination.IconToday = Source.IconToday
                Destination.JourJ1 = Source.JourJ1
                Destination.JourJ2 = Source.JourJ2
                Destination.JourJ3 = Source.JourJ3
                Destination.JourToday = Source.JourToday
                Destination.MaxJ1 = Source.MaxJ1
                Destination.MaxJ2 = Source.MaxJ2
                Destination.MaxJ3 = Source.MaxJ3
                Destination.MaxToday = Source.MaxToday
                Destination.MinJ1 = Source.MinJ1
                Destination.MinJ2 = Source.MinJ2
                Destination.MinJ3 = Source.MinJ3
                Destination.MinToday = Source.MinToday
                Destination.TemperatureActuel = Source.TemperatureActuel
                Destination.VentActuel = Source.VentActuel
                Destination.HumiditeActuel = Source.HumiditeActuel
                Destination.IconActuel = Source.IconActuel
            End If
            Destination.Name = Source.Name
            Destination.Value = Source.Value
            Destination.Adresse1 = Source.Adresse1
            Destination.Adresse2 = Source.Adresse2
            Destination.AllValue = Source.AllValue
            Destination.Correction = Source.Correction
            Destination.DateCreated = Source.DateCreated
            Destination.Description = Source.Description
            Destination.DeviceAction = Source.DeviceAction
            Destination.DriverID = Source.DriverID
            Destination.Enable = Source.Enable
            Destination.Formatage = Source.Formatage
            Destination.GetDeviceCommandePlus = Source.GetDeviceCommandePlus
            Destination.ID = Source.ID
            Destination.LastChange = Source.LastChange
            Destination.LastChangeDuree = Source.LastChangeDuree
            Destination.LastEtat = Source.LastEtat
            Destination.Modele = Source.Modele
            Destination.Picture = Source.Picture
            Destination.Precision = Source.Precision
            Destination.Puissance = Source.Puissance
            Destination.Refresh = Source.Refresh
            Destination.Solo = Source.Solo
            Destination.Unit = Source.Unit
            Destination.ValueDef = Source.ValueDef
            Destination.ValueLast = Source.ValueLast
            Destination.ValueMax = Source.ValueMax
            Destination.ValueMin = Source.ValueMin
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur Sub MaJ_Device: " & ex.ToString, "Erreur", " Sub MaJ_Device")
        End Try
    End Sub

    Private Sub NewLog(ByVal TypLog As HoMIDom.HoMIDom.Server.TypeLog, ByVal Source As HoMIDom.HoMIDom.Server.TypeSource, ByVal Fonction As String, ByVal Message As String) 'Evènement lorsqu'un nouveau log est écrit
        Try

            If _AffLastError Then
                Dim _string As String = ""
                For Each logerror As String In myService.GetLastLogsError
                    If String.IsNullOrEmpty(logerror) = False Then
                        _string &= logerror & vbCrLf
                        ImgMess.ToolTip = _string
                    End If
                Next
            End If
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur Sub NewLog: " & ex.ToString, "Erreur", " Sub NewLog")
        End Try
    End Sub

    Private Sub MessageFromServeur(Id As String, Time As DateTime, Message As String) 'Message provenant du serveur

    End Sub
    Private Sub DriverChanged(DriverId As String) 'Evènement lorsq'un driver est modifié

    End Sub
    Private Sub ZoneChanged(ZoneId As String) 'Evènement lorsq'une zone est modifiée ou créée

    End Sub
    Private Sub MacroChanged(MacroId As String) 'Evènement lorsq'une macro est modifiée ou créée

    End Sub
    Private Sub HeureSoleilChanged() 'Evènement lorsque l'heure de lever/couché du soleil est modifié
        Try
            If ShowSoleil = True Then
                Dim mydate As Date = myService.GetHeureLeverSoleil
                LblLeve.Content = mydate.ToShortTimeString
                mydate = myService.GetHeureCoucherSoleil
                LblCouche.Content = mydate.ToShortTimeString
                mydate = Nothing
            End If
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur Sub HeureSoleilChanged: " & ex.ToString, "Erreur", " Sub HeureSoleilChanged")
        End Try
    End Sub


#Region "Configuration"
    ''' <summary>Chargement de la config depuis le fichier XML</summary>
    ''' <param name="Fichier"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LoadConfig(ByVal Fichier As String) As String
        _ListElement.Clear()
        _ListMnu.Clear()
        imgStackPnl.Children.Clear()
        ScrollViewer1.Content = Nothing

        'Copy du fichier de config avant chargement
        Try
            If SaveDiffBackup = False Then
                If File.Exists(Fichier.Replace(".xml", ".bak")) = True Then File.Delete(Fichier.Replace(".xml", ".bak"))
            End If
            If File.Exists(Fichier) = True Then
                If SaveDiffBackup = False Then
                    File.Copy(Fichier, Fichier.Replace(".xml", ".bak"))
                Else
                    Dim fich As String = Fichier.Replace(".xml", ".bak")
                    fich = fich.Replace(".", Now.Year & Now.Month & Now.Day & Now.Hour & Now.Minute & Now.Second & ".")
                    File.Copy(Fichier, fich)
                End If
                Log(TypeLog.INFO, TypeSource.CLIENT, "LoadConfig", "Création du backup (.bak) du fichier de config avant chargement")
            Else
                ' Le fichier de config est inexistant. Premier lancement de WPF.
                LoadConfig = "Fichier de configuration inexistant."
                Exit Function
            End If
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur lors du lancement de l'application: " & ex.Message, "Erreur", "LoadConfig")
        End Try

        Try
            Dim myxml As XML
            Dim list As XmlNodeList

            myxml = New XML(Fichier)

            Log(TypeLog.INFO, TypeSource.CLIENT, "LoadConfig", "Chargement du fichier config: " & Fichier)

            FlagSave = True

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
                        Case "timeoutsrvlive"
                            TimeOutServerLive = CInt(list.Item(0).Attributes.Item(j).Value)
                        Case Else
                            Log(TypeLog.INFO, TypeSource.CLIENT, "LoadConfig", "Un attribut correspondant au serveur est inconnu: nom:" & list.Item(0).Attributes.Item(j).Name & " Valeur: " & list.Item(0).Attributes.Item(j).Value)
                    End Select
                Next
            Else
                AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Il manque les paramètres du client WPF dans le fichier de config !!", "Erreur", "LoadConfig")
            End If
            Log(TypeLog.INFO, TypeSource.CLIENT, "LoadConfig", "Paramètres du client WPF chargés")

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
                            m_friction = CDbl(list.Item(0).Attributes.Item(j).Value.Replace(".", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator))
                        Case "speedtouch"
                            m_SpeedTouch = CDbl(list.Item(0).Attributes.Item(j).Value.Replace(".", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator))
                        Case Else
                            Log(TypeLog.INFO, TypeSource.CLIENT, "LoadConfig", "Un attribut correspondant au serveur est inconnu: nom:" & list.Item(0).Attributes.Item(j).Name & " Valeur: " & list.Item(0).Attributes.Item(j).Value)
                    End Select
                Next
            Else
                AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Il manque les paramètres du client WPF dans le fichier de config !!", "Erreur", "LoadConfig")
            End If
            Log(TypeLog.INFO, TypeSource.CLIENT, "LoadConfig", "Paramètres tactiles chargés")

            '******************************************
            'on va chercher les paramètres interface
            '******************************************
            list = myxml.SelectNodes("/homidom/interface")
            If list.Count > 0 Then 'présence des paramètres du server
                For j As Integer = 0 To list.Item(0).Attributes.Count - 1
                    Select Case list.Item(0).Attributes.Item(j).Name
                        Case "showtimefromsrv"
                            ShowTimeFromServer = CBool(list.Item(0).Attributes.Item(j).Value)
                        Case "majwidgetfromsrv"
                            MaJWidgetFromServer = CBool(list.Item(0).Attributes.Item(j).Value)
                        Case "keyboardpath"
                            KeyboardPath = list.Item(0).Attributes.Item(j).Value
                        Case "transparencebarhaut"
                            TransparenceHaut = CInt(list.Item(0).Attributes.Item(j).Value)
                        Case "transparencebarbas"
                            TransparenceBas = CInt(list.Item(0).Attributes.Item(j).Value)
                        Case "savediffback"
                            SaveDiffBackup = CBool(list.Item(0).Attributes.Item(j).Value)
                        Case "flagsave"
                            FlagSave = CBool(list.Item(0).Attributes.Item(j).Value)
                        Case "autosave"
                            AutoSave = CBool(list.Item(0).Attributes.Item(j).Value)
                        Case "showlbltime"
                            ShowDateTime = CBool(list.Item(0).Attributes.Item(j).Value)
                        Case "showsoleil"
                            ShowSoleil = CBool(list.Item(0).Attributes.Item(j).Value)
                        Case "showtemperature"
                            ShowTemperature = CBool(list.Item(0).Attributes.Item(j).Value)
                        Case "showlblzone"
                            ShowLabelMnu = CBool(list.Item(0).Attributes.Item(j).Value)
                        Case "showkeyboard"
                            ShowKeyboard = CBool(list.Item(0).Attributes.Item(j).Value)
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
                        Case "showquitter"
                            ShowQuitter = CBool(list.Item(0).Attributes.Item(j).Value)
                        Case "widthpassword"
                            _WithPassword = CBool(list.Item(0).Attributes.Item(j).Value)
                        Case "password"
                            _PassWord = list.Item(0).Attributes.Item(j).Value
                        Case "left"
                            Me.Left = CDbl(list.Item(0).Attributes.Item(j).Value.Replace(".", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator))
                        Case "top"
                            Me.Top = CDbl(list.Item(0).Attributes.Item(j).Value.Replace(".", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator))
                        Case "width"
                            Me.Width = CDbl(list.Item(0).Attributes.Item(j).Value.Replace(".", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator))
                        Case "height"
                            Me.Height = CDbl(list.Item(0).Attributes.Item(j).Value.Replace(".", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator))
                        Case "afflasterror"
                            If IsBoolean(list.Item(0).Attributes.Item(j).Value) Then AffLastError = list.Item(0).Attributes.Item(j).Value
                        Case "astimeoutpage"
                            _AsTimeOutPage = list.Item(0).Attributes.Item(j).Value
                        Case "timeoutpage"
                            _TimeOutPage = list.Item(0).Attributes.Item(j).Value
                        Case "masktaskmenu"
                            _MaskTaskMnu = list.Item(0).Attributes.Item(j).Value
                        Case "defautpage"
                            _DefautPage = list.Item(0).Attributes.Item(j).Value
                        Case "ville"
                            _Ville = list.Item(0).Attributes.Item(j).Value
                        Case Else
                            Log(TypeLog.INFO, TypeSource.CLIENT, "LoadConfig", "Un attribut correspondant au serveur est inconnu: nom:" & list.Item(0).Attributes.Item(j).Name & " Valeur: " & list.Item(0).Attributes.Item(j).Value)
                    End Select
                Next
            Else
                AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Il manque les paramètres du client WPF dans le fichier de config !!", "Erreur", "LoadConfig")
            End If
            Log(TypeLog.INFO, TypeSource.CLIENT, "LoadConfig", "Paramètres de l'interface chargés")

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
                                        _MnuType = uCtrlImgMnu.TypeOfMnu.None
                                    Case Else
                                        _MnuType = uCtrlImgMnu.TypeOfMnu.None
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
            End If
            Log(TypeLog.INFO, TypeSource.CLIENT, "LoadConfig", "Menus chargés")


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
                                    Case uWidgetEmpty.TypeOfWidget.Volet.ToString
                                        x.Type = uWidgetEmpty.TypeOfWidget.Volet
                                    Case uWidgetEmpty.TypeOfWidget.Moteur.ToString
                                        x.Type = uWidgetEmpty.TypeOfWidget.Moteur
                                    Case uWidgetEmpty.TypeOfWidget.Rss.ToString
                                        x.Type = uWidgetEmpty.TypeOfWidget.Rss
                                    Case uWidgetEmpty.TypeOfWidget.Meteo.ToString
                                        x.Type = uWidgetEmpty.TypeOfWidget.Meteo
                                    Case uWidgetEmpty.TypeOfWidget.KeyPad.ToString
                                        x.Type = uWidgetEmpty.TypeOfWidget.KeyPad
                                    Case uWidgetEmpty.TypeOfWidget.Label.ToString
                                        x.Type = uWidgetEmpty.TypeOfWidget.Label
                                    Case uWidgetEmpty.TypeOfWidget.Image.ToString
                                        x.Type = uWidgetEmpty.TypeOfWidget.Image
                                End Select
                            Case "caneditvalue"
                                x.CanEditValue = list.Item(j).Attributes.Item(k).Value
                            Case "zoneid"
                                x.ZoneId = list.Item(j).Attributes.Item(k).Value
                            Case "iscommun"
                                x.IsCommun = list.Item(j).Attributes.Item(k).Value
                            Case "refresh"
                                x.Refresh = list.Item(j).Attributes.Item(k).Value
                            Case "x"
                                x.X = list.Item(j).Attributes.Item(k).Value
                            Case "y"
                                x.Y = list.Item(j).Attributes.Item(k).Value
                            Case "width"
                                x.Width = list.Item(j).Attributes.Item(k).Value.Replace(".", ",")
                            Case "height"
                                x.Height = list.Item(j).Attributes.Item(k).Value.Replace(".", ",")
                            Case "angle"
                                x.Rotation = list.Item(j).Attributes.Item(k).Value.Replace(".", ",")
                            Case "anglex"
                                x.RotationX = list.Item(j).Attributes.Item(k).Value.Replace(".", ",")
                            Case "angley"
                                x.RotationY = list.Item(j).Attributes.Item(k).Value.Replace(".", ",")
                            Case "zindex"
                                x.ZIndex = list.Item(j).Attributes.Item(k).Value
                            Case "min"
                                x.Min = list.Item(j).Attributes.Item(k).Value
                            Case "max"
                                x.Max = list.Item(j).Attributes.Item(k).Value
                            Case "showetiquette"
                                x.ShowEtiquette = list.Item(j).Attributes.Item(k).Value
                            Case "majetiquettefromsrv"
                                x.MaJEtiquetteFromServeur = list.Item(j).Attributes.Item(k).Value
                            Case "fondu"
                                x.Fondu = list.Item(j).Attributes.Item(k).Value
                            Case "showstatus"
                                x.ShowStatus = list.Item(j).Attributes.Item(k).Value
                            Case "etiquette"
                                x.Etiquette = list.Item(j).Attributes.Item(k).Value
                            Case "picture"
                                x.Picture = list.Item(j).Attributes.Item(k).Value
                            Case "showpicture"
                                x.ShowPicture = list.Item(j).Attributes.Item(k).Value
                            Case "savepictureproportion"
                                x.GarderProportionImage = list.Item(j).Attributes.Item(k).Value
                            Case "defautlabelstatus"
                                x.DefautLabelStatus = list.Item(j).Attributes.Item(k).Value
                            Case "taillestatus"
                                x.TailleStatus = list.Item(j).Attributes.Item(k).Value
                            Case "tailleetiquette"
                                x.TailleEtiquette = list.Item(j).Attributes.Item(k).Value
                            Case "alignementetiquette"
                                x.EtiquetteAlignement = list.Item(j).Attributes.Item(k).Value
                            Case "colorbackground"
                                If String.IsNullOrEmpty(list.Item(j).Attributes.Item(k).Value) = False Then
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
                                        If list.Item(j).ChildNodes.Item(l).ChildNodes.Item(m).Attributes.Count >= 5 Then
                                            If list.Item(j).ChildNodes.Item(l).ChildNodes.Item(m).Attributes.Item(4).Value IsNot Nothing Then .Operateur = list.Item(j).ChildNodes.Item(l).ChildNodes.Item(m).Attributes.Item(4).Value
                                        End If
                                        If list.Item(j).ChildNodes.Item(l).ChildNodes.Item(m).Attributes.Count >= 6 Then
                                            If list.Item(j).ChildNodes.Item(l).ChildNodes.Item(m).Attributes.Item(5).Value IsNot Nothing Then .Text = list.Item(j).ChildNodes.Item(l).ChildNodes.Item(m).Attributes.Item(5).Value
                                        End If
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
                        Else
                            _ListElement.Add(x)
                        End If
                    Else
                        _ListElement.Add(x)
                    End If
                Next
            Else
                Log(TypeLog.ERREUR, TypeSource.CLIENT, "LoadConfig", "Il manque des paramètres dans le fichier de configuration du client WPF")
            End If
            Log(TypeLog.INFO, TypeSource.CLIENT, "LoadConfig", "Eléments chargés")
            '**************
            'Next
            'End If

            ''Vide les variables
            'dirInfo = Nothing
            'File = Nothing
            'files = Nothing
            myxml = Nothing

            Return " Chargement de la configuration terminée"

        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "ERREUR LOADCONFIG " & ex.Message, "Erreur", "LoadConfig")
            Return " Erreur de chargement de la config: " & ex.Message
        End Try
    End Function

    ''' <summary>Sauvegarde de la config dans le fichier XML</summary>
    ''' <remarks></remarks>
    Public Sub SaveConfig(ByVal Fichier As String)
        Try

            Log(TypeLog.INFO, TypeSource.CLIENT, "SaveConfig", "Sauvegarde de la config sous le fichier " & Fichier)

            ''Copy du fichier de config avant sauvegarde
            Try
                If SaveDiffBackup = False Then
                    If File.Exists(Fichier.Replace(".xml", ".sav")) = True Then File.Delete(Fichier.Replace(".xml", ".sav"))
                    File.Copy(Fichier, Fichier.Replace(".xml", ".sav"))
                Else
                    Dim fich As String = Fichier.Replace(".xml", ".sav")
                    fich = fich.Replace(".", Now.Year & Now.Month & Now.Day & Now.Hour & Now.Minute & Now.Second & ".")
                    File.Copy(Fichier, fich)
                End If
                Log(TypeLog.INFO, TypeSource.CLIENT, "SaveConfig", "Création de sauvegarde (.sav) du fichier de config avant sauvegarde")
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.CLIENT, "SaveConfig", "Erreur impossible de créer une copie de backup du fichier de config: " & ex.Message)
            End Try

            If ChkEdit.IsChecked Then ChkEdit.IsChecked = False
            If ChkMove.IsChecked Then ChkMove.IsChecked = False

            ''Creation du fichier XML
            Dim writer As New XmlTextWriter(Fichier, System.Text.Encoding.UTF8)
            writer.WriteStartDocument(True)
            writer.Formatting = Formatting.Indented
            writer.Indentation = 2

            writer.WriteStartElement("homidom")

            Log(TypeLog.INFO, TypeSource.CLIENT, "SaveConfig", "Sauvegarde des paramètres du client WPF")
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
            writer.WriteStartAttribute("timeoutsrvlive")
            writer.WriteValue(TimeOutServerLive)
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
            writer.WriteStartAttribute("showtimefromsrv")
            writer.WriteValue(ShowTimeFromServer)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("majwidgetfromsrv")
            writer.WriteValue(MaJWidgetFromServer)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("keyboardpath")
            writer.WriteValue(KeyboardPath)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("transparencebarhaut")
            writer.WriteValue(TransparenceHaut)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("transparencebarbas")
            writer.WriteValue(TransparenceBas)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("savediffback")
            writer.WriteValue(SaveDiffBackup)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("flagsave")
            writer.WriteValue(FlagSave)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("autosave")
            writer.WriteValue(AutoSave)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("showsoleil")
            writer.WriteValue(ShowSoleil)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("showlblzone")
            writer.WriteValue(ShowLabelMnu)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("showlbltime")
            writer.WriteValue(ShowDateTime)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("showtemperature")
            writer.WriteValue(ShowTemperature)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("showkeyboard")
            writer.WriteValue(ShowKeyboard)
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
            writer.WriteStartAttribute("showquitter")
            writer.WriteValue(ShowQuitter)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("widthpassword")
            writer.WriteValue(_WithPassword)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("password")
            writer.WriteValue(_PassWord)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("afflasterror")
            writer.WriteValue(_AffLastError)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("astimeoutpage")
            writer.WriteValue(_AsTimeOutPage)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("timeoutpage")
            writer.WriteValue(_TimeOutPage)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("defautpage")
            writer.WriteValue(_DefautPage)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("masktaskmenu")
            writer.WriteValue(_MaskTaskMnu)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("ville")
            writer.WriteValue(_Ville)
            writer.WriteEndAttribute()
            writer.WriteEndElement()

            ''------------
            ''Sauvegarde des menus
            ''------------
            writer.WriteStartElement("menus")
            For i As Integer = 0 To ListMnu.Count - 1
                writer.WriteStartElement("menu")
                writer.WriteStartAttribute("nom")
                writer.WriteValue(ListMnu.Item(i).Label)
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
                'If (_ListElement.Item(i).IsEmpty = False And IsConnect = True) Or _ListElement.Item(i).IsEmpty = True Then

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
                writer.WriteStartAttribute("iscommun")
                writer.WriteValue(_ListElement.Item(i).IsCommun)
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
                writer.WriteStartAttribute("anglex")
                writer.WriteValue(_ListElement.Item(i).RotationX)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("angley")
                writer.WriteValue(_ListElement.Item(i).RotationY)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("zindex")
                writer.WriteValue(_ListElement.Item(i).ZIndex)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("majetiquettefromsrv")
                writer.WriteValue(_ListElement.Item(i).MaJEtiquetteFromServeur)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("showetiquette")
                writer.WriteValue(_ListElement.Item(i).ShowEtiquette)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("fondu")
                writer.WriteValue(_ListElement.Item(i).Fondu)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("showstatus")
                writer.WriteValue(_ListElement.Item(i).ShowStatus)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("showpicture")
                writer.WriteValue(_ListElement.Item(i).ShowPicture)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("savepictureproportion")
                writer.WriteValue(_ListElement.Item(i).GarderProportionImage)
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
                writer.WriteStartAttribute("alignementetiquette")
                writer.WriteValue(_ListElement.Item(i).EtiquetteAlignement)
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
                writer.WriteStartAttribute("refresh")
                writer.WriteValue(_ListElement.Item(i).Refresh)
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
                writer.WriteStartAttribute("min")
                writer.WriteValue(_ListElement.Item(i).Min)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("max")
                writer.WriteValue(_ListElement.Item(i).Max)
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
                    writer.WriteStartAttribute("image") 'image
                    writer.WriteValue(_ListElement.Item(i).Visuel.Item(j).Image)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("operateur") 'operateur
                    writer.WriteValue(_ListElement.Item(i).Visuel.Item(j).Operateur)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("text") 'text
                    writer.WriteValue(_ListElement.Item(i).Visuel.Item(j).Text)
                    writer.WriteEndAttribute()
                    writer.WriteEndElement()
                Next
                writer.WriteEndElement()
                writer.WriteEndElement()
                'End If
            Next
            writer.WriteEndElement()
            ''FIN DES ELEMENTS------------

            writer.WriteEndDocument()
            writer.Close()
            Log(TypeLog.INFO, TypeSource.CLIENT, "SaveConfig", "Sauvegarde terminée")
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "ERREUR SAVECONFIG " & ex.Message, "Erreur", "SaveConfig")
        End Try
    End Sub

#End Region

#Region "Connexion"
    'Connexion au serveur Homdidom
    Private Sub ConnectToHomidom()
        Dim myChannelFactory As ServiceModel.ChannelFactory(Of HoMIDom.HoMIDom.IHoMIDom) = Nothing

        Try
            Dim myadress As String = "http://" & IP & ":" & PortSOAP & "/service"
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
                If myService.GetIdServer(IdSrv) = "99" Then
                    AfficheMessageAndLog(Fonctions.TypeLog.MESSAGE, "L'ID du serveur est erroné, impossible de communiquer avec celui-ci", "Erreur", "ConnectToHomidom")
                    IsConnect = False
                Else
                    myService.GetTime()
                    IsConnect = True

                    Dim mypush As New PushNotification("http://" & IP & ":" & PortSOAP & "/live", IdSrv, TimeOutServerLive)
                    AddHandler mypush.DeviceChanged, AddressOf DeviceChanged
                    mypush.Open()

                    MnuMacro.Items.Clear()
                    For Each _mac As Macro In myService.GetAllMacros(IdSrv)
                        Dim mnu As New MenuItem
                        mnu.Tag = _mac.ID
                        mnu.Header = _mac.Nom
                        mnu.FontSize = 14
                        mnu.Height = 40
                        mnu.Foreground = Brushes.White
                        mnu.Background = MnuMacro.Background
                        AddHandler mnu.Click, AddressOf MnuExecuteMacro
                        MnuMacro.Items.Add(mnu)
                    Next

                    MnuLastError.Items.Clear()
                    Dim list As List(Of String) = myService.GetLastLogsError
                    If list.Count > 0 Then
                        For Each logerror As String In list
                            If String.IsNullOrEmpty(logerror) = False Then
                                Dim mnu As New MenuItem
                                mnu.Header = logerror
                                mnu.FontSize = 10
                                mnu.Foreground = Brushes.Black
                                MnuLastError.Items.Add(mnu)
                            End If
                        Next
                    End If
                    list = Nothing
                End If
            Catch ex As Exception
                IsConnect = False
            End Try

        Catch ex As Exception
            myChannelFactory.Abort()
            IsConnect = False
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur lors de la connexion au Serveur (ConnectToHomidom): " & ex.Message, "Erreur", "ConnectToHomidom")
        End Try
    End Sub
#End Region

    'Creation  du menu
    Private Sub NewBtnMnu(ByVal Label As String, ByVal type As uCtrlImgMnu.TypeOfMnu, Optional ByVal Parametres As List(Of String) = Nothing, Optional ByVal Defaut As Boolean = False, Optional ByVal Tag As String = "", Optional ByVal Icon As String = "", Optional ByVal IdElement As String = "", Optional ByVal Visible As Boolean = False)
        Try
            For Each _Mnu In _ListMnu
                If _Mnu.Name = Label And _Mnu.Type = type Then Exit Sub
            Next

            Dim ctrl As New uCtrlImgMnu
            ctrl.Type = type
            ctrl.Defaut = Defaut
            ctrl.Id = System.Guid.NewGuid.ToString()
            ctrl.Label = Label
            ctrl.Tag = Tag
            ctrl.Icon = Icon
            ctrl.Parametres = Parametres
            ctrl.IDElement = IdElement
            ctrl.Visible = Visible
            AddHandler ctrl.click, AddressOf IconMnuDoubleClick
            _ListMnu.Add(ctrl)

            If type = uCtrlImgMnu.TypeOfMnu.Zone Then
                If Visible Then imgStackPnl.Children.Add(ctrl)
            Else
                imgStackPnl.Children.Add(ctrl)
            End If

            ctrl = Nothing
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur lors de la création du bouton menu: " & ex.Message, "Erreur", "NewBtnMnu")
        End Try
    End Sub

    'Affiche la date et heure, heures levé et couché du soleil
    Public Sub dispatcherTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        Try
            LblTime.Content = Now.ToLongDateString & " "
            If IsConnect And ShowTimeFromServer Then
                LblTime.Content &= myService.GetTime
            Else
                LblTime.Content &= Now.ToLongTimeString
            End If

            If _AsTimeOutPage Then
                Dim vDiff As TimeSpan = Now - _TimeMouseDown
                If vDiff.Minutes >= _TimeOutPage Then
                    _TimeMouseDown = Now
                    If Canvas1.Children.Count > 0 Then Canvas1.Children.Clear()
                    ImageBackGround = _ImageBackGroundDefault
                    If _DefautPage IsNot Nothing Then 'si la page par defaut est pas null on l'affiche sinon on affiche rien
                        For Each icmnu In ListMnu
                            If icmnu.Label = _DefautPage Then
                                IconMnuDoubleClick(icmnu, Nothing)
                                Exit For
                            End If
                        Next
                    End If
                End If
            End If

            'If Now.Second = 0 Then
            '    'MaJ Liste des devices au moins toutes les minutes
            '    Dim x As New Thread(AddressOf Refresh)
            '    x.Priority = ThreadPriority.Highest
            '    x.Start()
            '    x = Nothing
            'End If
        Catch ex As Exception
            IsConnect = False

            If FlagMsgDeconnect = False Then
                AfficheMessageAndLog(Fonctions.TypeLog.INFO, "La communication a été perdue avec le serveur, veuillez vérifier que celui-ci est toujours actif et redémarrer le client", "Erreur")
                FlagMsgDeconnect = True
            End If

            Log(TypeLog.INFO, TypeSource.CLIENT, "DispatcherTimer", "DispatcherTimer: " & ex.Message)
            LblTime.Content = Now.ToLongDateString & " " & Now.ToShortTimeString
            LblLeve.Content = "?"
            LblCouche.Content = "?"

            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur: " & ex.Message, "Erreur")
        End Try
    End Sub

    'Clic sur un menu de la barre du bas
    Private Sub IconMnuDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Try
            Me.Cursor = Cursors.Wait

            DesAff_TaskMnu()
            _TimeMouseDown = Now
            LblZone.Content = sender.Label

            For Each _ctl In _ListMnu
                _ctl.IsSelect = False
            Next

            sender.IsSelect = True

            If sender.type <> uCtrlImgMnu.TypeOfMnu.LecteurMedia Then
                Canvas1.Children.Clear()
                If Media IsNot Nothing Then Media.Visibility = Windows.Visibility.Hidden
            End If

            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()

            Me.UpdateLayout()

            ChkMove.Visibility = Windows.Visibility.Collapsed
            ChkEdit.Visibility = Windows.Visibility.Collapsed
            Chk3.Visibility = Windows.Visibility.Collapsed

            Dim y As uCtrlImgMnu = sender

            If y IsNot Nothing Then
                Select Case y.Type
                    Case uCtrlImgMnu.TypeOfMnu.Internet
                        ImageBackGround = _ImageBackGroundDefault
                        Dim x As New uInternet(y.Parametres(0))
                        Canvas1.Children.Add(x)
                        x.Width = Canvas1.ActualWidth
                        x.Height = Canvas1.ActualHeight - 30
                    Case uCtrlImgMnu.TypeOfMnu.Config
                    Case uCtrlImgMnu.TypeOfMnu.LecteurMedia
                        If Media IsNot Nothing Then
                            If Media.Visibility = Windows.Visibility.Hidden Then
                                Media.Visibility = Windows.Visibility.Visible
                            Else
                                Media.Visibility = Windows.Visibility.Hidden
                            End If
                        Else
                            Media = New WMedia
                            Media.Owner = Me.Parent
                            Media.Show()
                            Media.WindowState = Windows.WindowState.Normal
                        End If
                    Case uCtrlImgMnu.TypeOfMnu.Meteo

                        'Gestion de l'erreur si le serveur n'est pas connecté
                        If IsConnect = True Then
                            ImageBackGround = _ImageBackGroundDefault
                            Dim x As New uMeteos
                            Canvas1.Children.Add(x)
                            x.Width = Canvas1.ActualWidth
                            x.Height = Canvas1.ActualHeight
                        Else
                            AfficheMessageAndLog(Fonctions.TypeLog.INFO, "Le serveur Homidom n'est pas connecté, impossible d'afficher les météos", "Information", "IconMnuDoubleClick")
                        End If

                    Case uCtrlImgMnu.TypeOfMnu.Zone
                        ShowZone(y.IDElement)
                End Select
            End If

            y = Nothing
            Me.UpdateLayout()
            Me.Cursor = Nothing
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur IconMnuDoubleClick: " & ex.Message, "Erreur", "IconMnuDoubleClick")
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
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur ElementShowZone: " & ex.Message, "Erreur", "ElementShowZone")
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        Try
            MyBase.Finalize()
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur Finalize Window1: " & ex.Message, "Erreur", "Finalize")
        End Try
    End Sub

    Private Sub Window1_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        Try
            'push.Close()
            myService = Nothing
            Log(TypeLog.INFO, TypeSource.CLIENT, "Client", "Fermeture de l'application")
        Catch ex As Exception
            Log(TypeLog.INFO, TypeSource.CLIENT, "Client", "Erreur Lors de la fermeture: " & ex.Message)
        End Try
    End Sub

    Private Sub ScrollViewer1_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles ScrollViewer1.MouseLeave
        DesAff_TaskMnu()
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

    Public Sub LoadZones()
        Try
            Dim cntNewZone As Integer = 0

            If IsConnect = False Then Exit Sub

            For Each _zon In myService.GetAllZones(IdSrv)
                If ListMnu.Count = 0 Then
                    NewBtnMnu(_zon.Name, uCtrlImgMnu.TypeOfMnu.Zone, , False, , , _zon.ID)
                    cntNewZone += 1
                Else
                    Dim flagexist As Boolean = False
                    'On vérifie si la zone est déjà créée
                    For j As Integer = 0 To ListMnu.Count - 1
                        If ListMnu.Item(j).IDElement = _zon.ID Then
                            flagexist = True
                            Exit For
                        End If
                    Next
                    'si la zone n'existe pas on la crée
                    If flagexist = False Then
                        NewBtnMnu(_zon.Name, uCtrlImgMnu.TypeOfMnu.Zone, , False, , , _zon.ID, True)
                        cntNewZone += 1
                    End If
                End If
            Next

            If cntNewZone > 0 Then
                AfficheMessageAndLog(Fonctions.TypeLog.INFO, cntNewZone & " nouvelle(s) zone(s) ajoutée(s)", "Information", "LoadZones")
            End If
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur LoadZones: " & ex.Message, "Erreur", "LoadZones")
        End Try
    End Sub

    Public Sub ShowZone(ByVal IdZone As String)
        Try
            'Gestion de l'erreur si le serveur n'est pas connecté
            If IsConnect = False Then
                AfficheMessageAndLog(Fonctions.TypeLog.INFO, "Le serveur Homidom n'est pas connecté, impossible d'afficher les éléments de la zone sélectionnée", "Information", "ShowZone")
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

            'Affiche l'image background de la zone
            ImgBackground.Source = Nothing
            If _zone.Image.Contains("Zone_Image.png") = True Or _zone.Image.EndsWith("\defaut.jpg") = True Then
                ImageBackGround = _MonRepertoire & "\Images\Fond-defaut.png"
            Else
                Dim _nfile As String = _zone.Image
                Dim _result As String = ""

                For j As Integer = Len(_nfile) To 1 Step -1
                    If Mid(_nfile, j, 1) <> "\" Then
                        _result = Mid(_nfile, j, 1) & _result
                    Else
                        Exit For
                    End If
                Next
                _result = _MonRepertoire & "\cache\images\" & _result

                If File.Exists(_result) Then
                    ImgBackground.Source = LoadBitmapImage(_result)
                Else
                    ImgBackground.Source = ConvertArrayToImage(myService.GetByteFromImage(_zone.Image))

                    'on enregistre l'image en cache
                    Dim oFileStream As System.IO.FileStream
                    oFileStream = New System.IO.FileStream(_result, System.IO.FileMode.Create)
                    oFileStream.Write(myService.GetByteFromImage(_zone.Image), 0, myService.GetByteFromImage(_zone.Image).Length)
                    oFileStream.Close()
                    oFileStream = Nothing
                End If

                _nfile = ""
                _result = ""
            End If

            'Desgin
            ChkMove.Visibility = Windows.Visibility.Visible
            ChkEdit.Visibility = Windows.Visibility.Visible
            Chk3.Visibility = Windows.Visibility.Visible
            If ChkMove.IsChecked = True Then ChkMove.IsChecked = False
            If ChkEdit.IsChecked = True Then ChkEdit.IsChecked = False

            'On parcours tous les éléments de la zone (hors widgets empty)
            For i As Integer = 0 To _zone.ListElement.Count - 1
                Dim z As Zone.Element_Zone = myService.ReturnZoneByID(IdSrv, IdZone).ListElement.Item(i)

                'l'élément est définit comme visible dans la zone
                If z.Visible = True Then
                    For j As Integer = 0 To _ListElement.Count - 1
                        If ((_ListElement.Item(j).Id = z.ElementID And _ListElement.Item(j).ZoneId = IdZone) Or _ListElement.Item(j).IsCommun) And _ListElement.Item(j).IsEmpty = False Then
                            'Ajouter un nouveau Control
                            Dim x As New ContentControl
                            Dim Trg As New TransformGroup
                            Dim Rot As New RotateTransform(_ListElement.Item(j).Rotation)
                            Dim RotXY As New SkewTransform(_ListElement.Item(j).RotationX, _ListElement.Item(j).RotationY)

                            Trg.Children.Add(Rot)
                            Trg.Children.Add(RotXY)
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
                            y.Refresh = _ListElement.Item(j).Refresh
                            y.Width = x.Width
                            y.Height = x.Height
                            y.X = _ListElement.Item(j).X
                            y.Y = _ListElement.Item(j).Y
                            y.Rotation = _ListElement.Item(j).Rotation
                            y.RotationX = _ListElement.Item(j).RotationX
                            y.RotationY = _ListElement.Item(j).RotationY
                            y.ZIndex = _ListElement.Item(j).ZIndex
                            y.IsEmpty = _ListElement.Item(j).IsEmpty
                            y.ShowEtiquette = _ListElement.Item(j).ShowEtiquette
                            y.MaJEtiquetteFromServeur = _ListElement.Item(j).MaJEtiquetteFromServeur
                            y.IsCommun = _ListElement.Item(j).IsCommun
                            y.Fondu = _ListElement.Item(j).Fondu
                            y.ShowStatus = _ListElement.Item(j).ShowStatus
                            y.ShowPicture = _ListElement.Item(j).ShowPicture
                            y.Picture = _ListElement.Item(j).Picture
                            y.GarderProportionImage = _ListElement.Item(j).GarderProportionImage
                            y.Etiquette = _ListElement.Item(j).Etiquette
                            y.EtiquetteAlignement = _ListElement.Item(j).EtiquetteAlignement
                            y.DefautLabelStatus = _ListElement.Item(j).DefautLabelStatus
                            y.TailleStatus = _ListElement.Item(j).TailleStatus
                            y.TailleEtiquette = _ListElement.Item(j).TailleEtiquette
                            y.ColorBackGround = _ListElement.Item(j).ColorBackGround
                            y.ColorStatus = _ListElement.Item(j).ColorStatus
                            y.ColorEtiquette = _ListElement.Item(j).ColorEtiquette
                            y.Min = _ListElement.Item(j).Min
                            y.Max = _ListElement.Item(j).Max
                            y.IsHitTestVisible = True 'True:bouge pas False:Bouge
                            AddHandler y.ShowZone, AddressOf ElementShowZone
                            x.Content = y
                            Canvas1.Children.Add(x)
                            Canvas.SetLeft(x, _ListElement.Item(j).X)
                            Canvas.SetTop(x, _ListElement.Item(j).Y)
                            Canvas.SetZIndex(x, _ListElement.Item(j).ZIndex)

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
                If (_ListElement.Item(i).ZoneId = IdZone Or _ListElement.Item(i).IsCommun) And _ListElement.Item(i).IsEmpty = True Then
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
                    y.Id = _ListElement.Item(i).Id
                    y.ZoneId = _ListElement.Item(i).ZoneId
                    y.Width = x.Width
                    y.Height = x.Height
                    y.X = _ListElement.Item(i).X
                    y.Y = _ListElement.Item(i).Y
                    y.Rotation = _ListElement.Item(i).Rotation
                    y.RotationX = _ListElement.Item(i).RotationX
                    y.RotationY = _ListElement.Item(i).RotationY
                    y.ZIndex = _ListElement.Item(i).ZIndex
                    y.IsEmpty = _ListElement.Item(i).IsEmpty
                    y.Type = _ListElement.Item(i).Type
                    y.Refresh = _ListElement.Item(i).Refresh
                    y.CanEditValue = _ListElement.Item(i).CanEditValue
                    y.Picture = _ListElement.Item(i).Picture
                    y.ShowPicture = _ListElement.Item(i).ShowPicture
                    y.GarderProportionImage = _ListElement.Item(i).GarderProportionImage
                    y.ShowEtiquette = _ListElement.Item(i).ShowEtiquette
                    y.IsCommun = _ListElement.Item(i).IsCommun
                    y.Fondu = _ListElement.Item(i).Fondu
                    y.ShowStatus = _ListElement.Item(i).ShowStatus
                    y.Etiquette = _ListElement.Item(i).Etiquette
                    y.DefautLabelStatus = _ListElement.Item(i).DefautLabelStatus
                    y.TailleStatus = _ListElement.Item(i).TailleStatus
                    y.TailleEtiquette = _ListElement.Item(i).TailleEtiquette
                    y.EtiquetteAlignement = _ListElement.Item(i).EtiquetteAlignement
                    y.MaJEtiquetteFromServeur = _ListElement.Item(i).MaJEtiquetteFromServeur
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
                    y.Min = _ListElement.Item(i).Min
                    y.Max = _ListElement.Item(i).Max

                    AddHandler y.ShowZone, AddressOf ElementShowZone
                    x.Content = y
                    Canvas1.Children.Add(x)
                    Canvas.SetLeft(x, _ListElement.Item(i).X)
                    Canvas.SetTop(x, _ListElement.Item(i).Y)
                    Canvas.SetZIndex(x, _ListElement.Item(i).ZIndex)

                    x = Nothing
                    y = Nothing
                End If
            Next

        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur ShowZone: " & ex.ToString, "Erreur", "ShowZone")
        End Try
    End Sub

    Private Sub Deplacement_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ChkMove.Click
        Try
            'Mode déplacement
            If ChkMove.IsChecked = True Then
                ChkEdit.IsChecked = False
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
                                Dim gt As TransformGroup = child.RenderTransform
                                For k = 0 To gt.Children.Count - 1
                                    If InStr(LCase(gt.Children.Item(k).GetType.ToString), "rotatetransform") > 0 Then
                                        Dim rt As RotateTransform = gt.Children.Item(k)
                                        If rt IsNot Nothing Then
                                            _ListElement.Item(j).Rotation = rt.Angle
                                        End If
                                    End If
                                    If InStr(LCase(gt.Children.Item(k).GetType.ToString), "skewtransform") > 0 Then
                                        Dim rt As SkewTransform = gt.Children.Item(k)
                                        If rt IsNot Nothing Then
                                            _ListElement.Item(j).RotationX = rt.AngleX
                                            _ListElement.Item(j).RotationY = rt.AngleY
                                        End If
                                    End If
                                Next
                            End If
                            If InStr(child.RenderTransform.GetType.ToString, "RotateTransform") > 0 Then
                                _ListElement.Item(j).Rotation = child.RenderTransform.GetValue(RotateTransform.AngleProperty)
                            End If
                            If InStr(child.RenderTransform.GetType.ToString, "SkewTransform") > 0 Then
                                _ListElement.Item(j).RotationX = child.RenderTransform.GetValue(SkewTransform.AngleXProperty)
                                _ListElement.Item(j).RotationX = child.RenderTransform.GetValue(SkewTransform.AngleYProperty)
                            End If
                        End If
                    Next

                    Dim lbl As uWidgetEmpty = child.Content
                    lbl.IsHitTestVisible = True

                Next
            End If

            Me.UpdateLayout()
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur Chk1: " & ex.Message, "Erreur", "Chk1")
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
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur Resize: " & ex.Message, "Erreur", "Resize")
        End Try
    End Sub

    Private Sub ModeEdition_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ChkEdit.Click
        Try

            If ChkEdit.IsChecked = True Then
                ChkMove.IsChecked = False

                'On a finit le déplacement
                Design = False
                ChkEdit.IsChecked = True
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
                                Dim gt As TransformGroup = child.RenderTransform
                                For k = 0 To gt.Children.Count - 1
                                    If InStr(LCase(gt.Children.Item(k).GetType.ToString), "rotatetransform") > 0 Then
                                        Dim rt As RotateTransform = gt.Children.Item(k)
                                        If rt IsNot Nothing Then
                                            _ListElement.Item(j).Rotation = rt.Angle
                                        End If
                                    End If
                                    If InStr(LCase(gt.Children.Item(k).GetType.ToString), "skewtransform") > 0 Then
                                        Dim rt As SkewTransform = gt.Children.Item(k)
                                        If rt IsNot Nothing Then
                                            _ListElement.Item(j).RotationX = rt.AngleX
                                            _ListElement.Item(j).RotationY = rt.AngleY
                                        End If
                                    End If
                                Next
                            End If
                            If InStr(child.RenderTransform.GetType.ToString, "RotateTransform") > 0 Then
                                _ListElement.Item(j).Rotation = child.RenderTransform.GetValue(RotateTransform.AngleProperty)
                            End If
                            If InStr(child.RenderTransform.GetType.ToString, "SkewTransform") > 0 Then
                                _ListElement.Item(j).RotationX = child.RenderTransform.GetValue(SkewTransform.AngleXProperty)
                                _ListElement.Item(j).RotationX = child.RenderTransform.GetValue(SkewTransform.AngleYProperty)
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
                obj.ModeEdition = ChkEdit.IsChecked
                obj = Nothing
            Next

            Me.UpdateLayout()
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur Chk2: " & ex.Message, "Erreur", "Chk2")
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
                                End If
                                If InStr(LCase(gt.Children.Item(k).GetType.ToString), "skewtransform") > 0 Then
                                    Dim rt As SkewTransform = gt.Children.Item(k)
                                    If rt IsNot Nothing Then
                                        _ListElement.Item(j).RotationX = rt.AngleX
                                        _ListElement.Item(j).RotationY = rt.AngleY
                                    End If
                                End If
                            Next
                        End If
                        If InStr(child.RenderTransform.GetType.ToString, "RotateTransform") > 0 Then
                            _ListElement.Item(j).Rotation = child.RenderTransform.GetValue(RotateTransform.AngleProperty)
                        End If
                        If InStr(child.RenderTransform.GetType.ToString, "SkewTransform") > 0 Then
                            _ListElement.Item(j).RotationX = child.RenderTransform.GetValue(SkewTransform.AngleXProperty)
                            _ListElement.Item(j).RotationX = child.RenderTransform.GetValue(SkewTransform.AngleYProperty)
                        End If
                    End If
                Next

                Dim lbl As uWidgetEmpty = child.Content
                lbl.IsHitTestVisible = True
            Next
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur Resize: " & ex.Message, "Erreur", "Resize")
        End Try
    End Sub

#Region "Nouveau Widget"


    Private Sub NewWidgetEmpty_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles NewWidgetEmpty.Click
        Try
            ' Remettre à zéro les modes édition + déplacement
            ChkMove.IsChecked = False
            ChkEdit.IsChecked = False
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
            elmt.MaJEtiquetteFromServeur = False
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
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur NewWidgetEmpty: " & ex.Message, "Erreur", "NewWidgetEmpty")
        End Try
    End Sub

    Private Sub NewWidgetWeb_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles NewWidgetWeb.Click
        Try
            ' Remettre à zéro les modes édition + déplacement
            ChkMove.IsChecked = False
            ChkEdit.IsChecked = False
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
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur NewWidgetWeb: " & ex.Message, "Erreur", "NewWidgetWeb")
        End Try
    End Sub

    Private Sub NewWidgetRss_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles NewWidgetRss.Click
        Try
            ' Remettre à zéro les modes édition + déplacement
            ChkMove.IsChecked = False
            ChkEdit.IsChecked = False
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
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur NewWidgetRss: " & ex.Message, "Erreur", "NewWidgetRss")
        End Try
    End Sub

    Private Sub NewWidgetMeteo_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles NewWidgetMeteo.Click
        Try
            ' Remettre à zéro les modes édition + déplacement
            ChkMove.IsChecked = False
            ChkEdit.IsChecked = False
            Deplacement_Click(Me, e)

            'Ajouter un nouveau Control
            Dim x As New ContentControl
            x.Width = 202
            x.Height = 211
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
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur NewWidgetMeteo: " & ex.Message, "Erreur", "NewWidgetMeteo")
        End Try
    End Sub

    Private Sub NewWidgetKeyPad_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles NewWidgetKeyPad.Click
        Try
            ' Remettre à zéro les modes édition + déplacement
            ChkMove.IsChecked = False
            ChkEdit.IsChecked = False
            Deplacement_Click(Me, e)

            'Ajouter un nouveau Control
            Dim x As New ContentControl
            x.Width = 214
            x.Height = 305
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
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur NewWidgetKeyPad: " & ex.Message, "Erreur", "NewWidgetKeyPad")
        End Try
    End Sub

    Private Sub NewWidgetLabel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles NewWidgetLabel.Click
        Try
            ' Remettre à zéro les modes édition + déplacement
            ChkMove.IsChecked = False
            ChkEdit.IsChecked = False
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

            elmt = Nothing
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur NewWidgetLabel_Click: " & ex.Message, "Erreur", "NewWidgetLabel_Click")
        End Try
    End Sub

    Private Sub NewWidgetImage_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles NewWidgetImage.Click
        Try
            ' Remettre à zéro les modes édition + déplacement
            ChkMove.IsChecked = False
            ChkEdit.IsChecked = False
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
            elmt.Height = 25
            elmt.Rotation = 0
            elmt.X = 300
            elmt.Y = 300
            elmt.IsEmpty = True
            elmt.Type = uWidgetEmpty.TypeOfWidget.Image
            elmt.ShowStatus = False
            _ListElement.Add(elmt)

            elmt.IsHitTestVisible = True 'True:bouge pas False:Bouge
            x.Content = elmt
            Canvas1.Children.Add(x)
            Canvas.SetLeft(x, 300)
            Canvas.SetTop(x, 300)
            Canvas.SetZIndex(x, 0)

            elmt = Nothing
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur NewWidgetLabel_Click: " & ex.Message, "Erreur", "NewWidgetLabel_Click")
        End Try
    End Sub


    Private Sub NewWidgetCamera_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles NewWidgetCamera.Click
        Try
            ' Remettre à zéro les modes édition + déplacement
            ChkMove.IsChecked = False
            ChkEdit.IsChecked = False
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
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur NewWidgetCamera: " & ex.Message, "Erreur", "NewWidgetCamera")
        End Try
    End Sub

    Private Sub NewWidgetVolet_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles NewWidgetVolet.Click
        Try
            ' Remettre à zéro les modes édition + déplacement
            ChkMove.IsChecked = False
            ChkEdit.IsChecked = False
            Deplacement_Click(Me, e)

            'Ajouter un nouveau Control
            Dim x As New ContentControl
            x.Width = 270
            x.Height = 195
            x.Style = mybuttonstyle
            x.Tag = True
            x.Uid = System.Guid.NewGuid.ToString()

            'Ajoute l'élément dans la liste
            Dim elmt As New uWidgetEmpty
            elmt.Show = True
            elmt.Uid = x.Uid
            elmt.ZoneId = _CurrentIdZone
            elmt.Width = 270
            elmt.Height = 195
            elmt.Rotation = 0
            elmt.X = 300
            elmt.Y = 300
            elmt.IsEmpty = True
            elmt.Type = uWidgetEmpty.TypeOfWidget.Volet
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
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur NewWidgetVolet: " & ex.Message, "Erreur", "NewWidgetVolet")
        End Try
    End Sub

    Private Sub NewWidgetMoteur_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles NewWidgetMoteur.Click
        Try
            ' Remettre à zéro les modes édition + déplacement
            ChkMove.IsChecked = False
            ChkEdit.IsChecked = False
            Deplacement_Click(Me, e)

            'Ajouter un nouveau Control
            Dim x As New ContentControl
            x.Width = 200
            x.Height = 120
            x.Style = mybuttonstyle
            x.Tag = True
            x.Uid = System.Guid.NewGuid.ToString()

            'Ajoute l'élément dans la liste
            Dim elmt As New uWidgetEmpty
            elmt.Show = True
            elmt.Uid = x.Uid
            elmt.ZoneId = _CurrentIdZone
            elmt.Width = 270
            elmt.Height = 195
            elmt.Rotation = 0
            elmt.X = 300
            elmt.Y = 300
            elmt.IsEmpty = True
            elmt.Type = uWidgetEmpty.TypeOfWidget.Moteur
            elmt.ShowStatus = True
            elmt.Etiquette = "Widget " & Canvas1.Children.Count + 1
            elmt.ColorBackGround = New SolidColorBrush(Color.FromArgb(255, 0, 0, 0))
            _ListElement.Add(elmt)

            elmt.IsHitTestVisible = True 'True:bouge pas False:Bouge
            x.Content = elmt
            Canvas1.Children.Add(x)
            Canvas.SetLeft(x, 300)
            Canvas.SetTop(x, 300)
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur NewWidgetVolet: " & ex.Message, "Erreur", "NewWidgetVolet")
        End Try
    End Sub

#End Region

#Region "Menu"

    Private Sub ViewLog_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ViewLogClient.Click, ViewLogSrv.Click
        Try
            Me.Cursor = Cursors.Wait
            If Canvas1.Children.Count > 0 Then
                Canvas1.Children.Clear()
                Me.UpdateLayout()
            End If

            ChkMove.Visibility = Windows.Visibility.Collapsed
            ChkEdit.Visibility = Windows.Visibility.Collapsed
            Chk3.Visibility = Windows.Visibility.Collapsed

            Dim x As New uLog(sender.tag)
            x.Uid = System.Guid.NewGuid.ToString()
            AddHandler x.CloseMe, AddressOf UnloadControl
            x.Width = Canvas1.ActualWidth - 100
            x.Height = Canvas1.ActualHeight - 50
            Canvas1.Children.Add(x)

            Me.Cursor = Nothing
        Catch ex As Exception
            Me.Cursor = Nothing
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur ViewLog: " & ex.Message, "Erreur", "ViewLog")
        End Try
    End Sub

    Private Sub ViewCalendar_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ViewCalendar.Click
        Try
            Dim x As New WCalendar
            x.Owner = Me
            x.ShowDialog()
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur ViewCalendar: " & ex.Message, "Erreur", "ViewCalendar")
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
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur MnuHisto: " & ex.Message, "Erreur", "MnuHisto")
        End Try
    End Sub

    Private Sub MnuExecuteMacro(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Try
            myService.RunMacro(IdSrv, sender.tag)
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur MnuMacro_MouseDown: " & ex.Message, "Erreur", "MnuMacro_MouseDown")
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
                mnu.Foreground = Brushes.White
                mnu.Height = 40
                mnu.Background = MnuMacro.Background

                AddHandler mnu.Click, AddressOf MnuExecuteMacro
                MnuMacro.Items.Add(mnu)
            Next

            MnuMacro.UpdateLayout()
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur MnuMacro_MouseDown: " & ex.Message, "Erreur", "MnuMacro_MouseDown")
        End Try
    End Sub

    Private Sub MnuConfig_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MnuConfig.Click
        Try
            If VerifPassword() = False Then Exit Sub

            Canvas1.Children.Clear()
            Me.UpdateLayout()

            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()

            ImageBackGround = _ImageBackGroundDefault
            Dim x As New WConfig(Me)
            x.Owner = Me
            x.ShowDialog()

            If x.DialogResult.HasValue And x.DialogResult.Value Then
                imgStackPnl.Children.Clear()
                For Each mnu In ListMnu
                    RemoveHandler mnu.click, AddressOf IconMnuDoubleClick

                    If mnu.Type = uCtrlImgMnu.TypeOfMnu.Zone Then
                        If mnu.Visible = True Then imgStackPnl.Children.Add(mnu)
                    Else
                        imgStackPnl.Children.Add(mnu)
                    End If

                    AddHandler mnu.click, AddressOf IconMnuDoubleClick
                Next
                'For i As Integer = 0 To ListMnu.Count - 1
                '    If ListMnu.Item(i).Type = uCtrlImgMnu.TypeOfMnu.Zone Then
                '        If ListMnu.Item(i).Visible = True Then imgStackPnl.Children.Add(ListMnu.Item(i))
                '    Else
                '        imgStackPnl.Children.Add(ListMnu.Item(i))
                '    End If
                'Next
                If x.ChkFullScreen.IsChecked = False Then
                    Me.WindowState = Windows.WindowState.Normal
                Else
                    Me.WindowState = Windows.WindowState.Maximized
                End If
                x.Close()
                FlagSave = True
            Else
                x.Close()
            End If
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur MnuConfig_Click: " & ex.Message, "Erreur", "MnuConfig_Click")
        End Try
    End Sub

#End Region


    Private Sub Window1_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Try
            Me.Cursor = Nothing
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur Window1_Loaded: " & ex.Message, "Erreur", "Window1_Loaded")
        End Try
    End Sub



#Region "Quitter"
    'Bouton Quitter
    Private Sub BtnQuit_Click_1(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnQuit.Click
        Quitter()
    End Sub

    ''' <summary>
    ''' Menu Quitter
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub MnuQuitter_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MnuQuitter.Click
        Quitter()
    End Sub

    Private Sub Quitter()
        Try
            If VerifPassword() = False Then Exit Sub
            If AutoSave Or FlagSave Then SaveConfig(_ConfigFile)
            Log(TypeLog.INFO, TypeSource.CLIENT, "Client", "Fermeture de l'application")
            End
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur Quitter: " & ex.Message, "Erreur", "Quitter")
            End
        End Try
    End Sub

    Private Function VerifPassword() As Boolean
        Try
            If _WithPassword And String.IsNullOrEmpty(_PassWord) = False Then
                If InputBox("Veuillez saisir le mot de passe:", "Homidom") <> _PassWord Then
                    AfficheMessageAndLog(Fonctions.TypeLog.MESSAGE, "Mot de passe erroné!", "Erreur", "VerifPassword")
                    Return False
                Else
                    Return True
                End If
            Else
                Return True
            End If
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur VerifPassword: " & ex.Message, "Erreur", "VerifPassword")
            Return True
        End Try
    End Function
#End Region


    Private Sub Window1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Me.MouseDown
        If _flagIsShowScroll Then
            DesAff_TaskMnu()
        Else
            Aff_TaskMnu()
        End If
    End Sub

    Private Sub Window1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles Me.MouseMove
        If _AsTimeOutPage Then
            If e.GetPosition(Me) <> _MousePosition Then
                _TimeMouseDown = Now
                _MousePosition = e.GetPosition(Me)
            End If
        End If
    End Sub

    Private Sub Menu1_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Menu1.MouseDown
        CtxMnuBtn.PlacementTarget = sender
        CtxMnuBtn.IsOpen = True

        If IsConnect Then
            MnuLastError.Items.Clear()
            Dim list As List(Of String) = myService.GetLastLogsError
            If list.Count > 0 Then
                Dim _tool As String = ""
                For Each logerror As String In list
                    If String.IsNullOrEmpty(logerror) = False Then
                        Dim mnu As New MenuItem
                        mnu.Header = logerror
                        mnu.FontSize = 12
                        mnu.Foreground = Brushes.Black
                        MnuLastError.Items.Add(mnu)
                    End If
                Next
            End If
            list = Nothing
        End If
    End Sub

#Region "TaskMenu"
    ''' <summary>
    ''' Disapparaitre la barre de menu du bas
    ''' </summary>
    ''' <remarks></remarks>
    Sub DesAff_TaskMnu()
        If _MaskTaskMnu Then
            If _flagIsShowScroll Then
                _flagIsShowScroll = False
                Dim st As Storyboard = TryFindResource("sb_Rect")
                ScrollViewer1.BeginStoryboard(st)
            End If
        End If
    End Sub

    ''' <summary>
    ''' fait apparaitre la barre de menu du bas
    ''' </summary>
    ''' <remarks></remarks>
    Sub Aff_TaskMnu()
        If _MaskTaskMnu Then
            If _flagIsShowScroll = False Then
                _flagIsShowScroll = True
                Dim st2 As Storyboard = TryFindResource("sb_Rect2")
                ScrollViewer1.BeginStoryboard(st2)
            End If
        End If
    End Sub
#End Region


    ''' <summary>
    ''' Menu sauvegarder
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub MnuSave_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles MnuSave.Click
        Try
            SaveConfig(_ConfigFile)
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur MnuSave_Click: " & ex.Message, "Erreur", "MnuSave_Click")
        End Try
    End Sub

End Class
