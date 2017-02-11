Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device

Imports System.Text.RegularExpressions
Imports STRGS = Microsoft.VisualBasic.Strings
Imports HoMIDom.HoMIDom
Imports HoMIOAuth2

' Auteur : jphomi sur une base HoMIDoM meteoweather
' Date : 01/03/2015

''' <summary>Driver MyFox Meteo Reception de datas base plus module température/pluviometre</summary>
''' <remarks></remarks>
<Serializable()> Public Class Driver_MyFox
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "4D1C0D54-CF4B-11E6-843F-43E100593AC9"
    Dim _Nom As String = "MyFox"
    Dim _Enable As Boolean = False
    Dim _Description As String = "Données station météo MyFox"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "WEB"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "@"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "MyFox"
    Dim _Version As String = My.Application.Info.Version.ToString
    Dim _OsPlatform As String = "3264"
    Dim _Picture As String = ""
    Dim _Server As HoMIDom.HoMIDom.Server
    Dim _Device As HoMIDom.HoMIDom.Device
    Dim _DeviceSupport As New ArrayList
    Dim _Parametres As New ArrayList
    Dim _LabelsDriver As New ArrayList
    Dim _LabelsDevice As New ArrayList
    Dim MyTimer As New Timers.Timer
    Dim _IdSrv As String
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande)
    Dim _AutoDiscover As Boolean = False

    'A ajouter dans les ppt du driver
    Dim _tempsentrereponse As Integer = 1500
    Dim _ignoreadresse As Boolean = False
    Dim _lastetat As Boolean = True

#End Region

#Region "Variables internes"

    Dim Auth As Authentication
    Dim sitelist As Site
    Dim statelist As DeviceWithStateCollection
    Dim socketlist As SocketCollection

    Dim devices As List(Of String) = New List(Of String)

    'param avancé
    Dim _DEBUG As Boolean = False

    Dim cptBeforeToken As Integer = 0
    Dim cpt As Integer = 0
    Dim cpt_restart As Integer
    Dim First As Boolean = True

    Public Class DeviceWithStateCollection
        Public items As List(Of DeviceWithState)
    End Class

    Public Class DeviceWithState
        Public deviceId As Integer ' The device identifier,
        Public label As String ' The device label,
        Public stateLabel As String ' = ['opened' or 'closed']: Current state,
        Public modelId As String ' The device model identifier,
        Public modelLabel As String ' The device model label
    End Class

    Public Class SocketCollection
        Public items As List(Of Socket)
    End Class

    Public Class Socket
        Public deviceId As Integer ' The device identifier,
        Public label As String ' The device label,
        Public modelId As String ' The device model identifier,
        Public modelLabel As String ' The device model label
    End Class

    Public Class Site
        Public siteId As Integer ' The site unique identifier,
        Public label As String ' The site label,
        Public brand As String ' The brand of the site,
        Public timezone As String ' The timezone of the site location,
        Public AXA As String ' AXA Assistance identifier,
        Public cameraCount As Integer ' Number of cameras on the site,
        Public gateCount As Integer ' Number of gates on the site,
        Public shutterCount As Integer ' Number of shutters on the site,
        Public socketCount As Integer ' Number of sockets on the site,
        Public moduleCount As Integer ' Number of modules on the site,
        Public heaterCount As Integer ' Number of heaters on the site,
        Public scenarioCount As Integer ' Number of scenarios on the site,
        Public deviceTemperatureCount As Integer ' Number of temperature sensors on the site,
        Public deviceStateCount As Integer ' Number of IntelliTag on the site,
        Public deviceLightCount As Integer ' Number of light sensors on the site,
        Public deviceDetectorCount As Integer ' Number of generic detectors on the site
    End Class
#End Region

#Region "Propriétés génériques"
    Public WriteOnly Property IdSrv As String Implements HoMIDom.HoMIDom.IDriver.IdSrv
        Set(ByVal value As String)
            _IdSrv = value
        End Set
    End Property

    Public Property COM() As String Implements HoMIDom.HoMIDom.IDriver.COM
        Get
            Return _Com
        End Get
        Set(ByVal value As String)
            _Com = value
        End Set
    End Property
    Public ReadOnly Property Description() As String Implements HoMIDom.HoMIDom.IDriver.Description
        Get
            Return _Description
        End Get
    End Property
    Public ReadOnly Property DeviceSupport() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.DeviceSupport
        Get
            Return _DeviceSupport
        End Get
    End Property
    Public Property Parametres() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.Parametres
        Get
            Return _Parametres
        End Get
        Set(ByVal value As System.Collections.ArrayList)
            _Parametres = value
        End Set
    End Property

    Public Property LabelsDriver() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.LabelsDriver
        Get
            Return _LabelsDriver
        End Get
        Set(ByVal value As System.Collections.ArrayList)
            _LabelsDriver = value
        End Set
    End Property
    Public Property LabelsDevice() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.LabelsDevice
        Get
            Return _LabelsDevice
        End Get
        Set(ByVal value As System.Collections.ArrayList)
            _LabelsDevice = value
        End Set
    End Property



    Public Event DriverEvent(ByVal DriveName As String, ByVal TypeEvent As String, ByVal Parametre As Object) Implements HoMIDom.HoMIDom.IDriver.DriverEvent

    Public Property Enable() As Boolean Implements HoMIDom.HoMIDom.IDriver.Enable
        Get
            Return _Enable
        End Get
        Set(ByVal value As Boolean)
            _Enable = value
        End Set
    End Property
    Public ReadOnly Property ID() As String Implements HoMIDom.HoMIDom.IDriver.ID
        Get
            Return _ID
        End Get
    End Property
    Public Property IP_TCP() As String Implements HoMIDom.HoMIDom.IDriver.IP_TCP
        Get
            Return _IP_TCP
        End Get
        Set(ByVal value As String)
            _IP_TCP = value
        End Set
    End Property
    Public Property IP_UDP() As String Implements HoMIDom.HoMIDom.IDriver.IP_UDP
        Get
            Return _IP_UDP
        End Get
        Set(ByVal value As String)
            _IP_UDP = value
        End Set
    End Property
    Public ReadOnly Property IsConnect() As Boolean Implements HoMIDom.HoMIDom.IDriver.IsConnect
        Get
            Return _IsConnect
        End Get
    End Property
    Public Property Modele() As String Implements HoMIDom.HoMIDom.IDriver.Modele
        Get
            Return _Modele
        End Get
        Set(ByVal value As String)
            _Modele = value
        End Set
    End Property
    Public ReadOnly Property Nom() As String Implements HoMIDom.HoMIDom.IDriver.Nom
        Get
            Return _Nom
        End Get
    End Property
    Public Property Picture() As String Implements HoMIDom.HoMIDom.IDriver.Picture
        Get
            Return _Picture
        End Get
        Set(ByVal value As String)
            _Picture = value
        End Set
    End Property
    Public Property Port_TCP() As String Implements HoMIDom.HoMIDom.IDriver.Port_TCP
        Get
            Return _Port_TCP
        End Get
        Set(ByVal value As String)
            _Port_TCP = value
        End Set
    End Property
    Public Property Port_UDP() As String Implements HoMIDom.HoMIDom.IDriver.Port_UDP
        Get
            Return _Port_UDP
        End Get
        Set(ByVal value As String)
            _Port_UDP = value
        End Set
    End Property
    Public ReadOnly Property Protocol() As String Implements HoMIDom.HoMIDom.IDriver.Protocol
        Get
            Return _Protocol
        End Get
    End Property
    Public Property Refresh() As Integer Implements HoMIDom.HoMIDom.IDriver.Refresh
        Get
            Return _Refresh
        End Get
        Set(ByVal value As Integer)
            _Refresh = value
        End Set
    End Property
    Public Property Server() As HoMIDom.HoMIDom.Server Implements HoMIDom.HoMIDom.IDriver.Server
        Get
            Return _Server
        End Get
        Set(ByVal value As HoMIDom.HoMIDom.Server)
            _Server = value
        End Set
    End Property
    Public ReadOnly Property Version() As String Implements HoMIDom.HoMIDom.IDriver.Version
        Get
            Return _Version
        End Get
    End Property
    Public ReadOnly Property OsPlatform() As String Implements HoMIDom.HoMIDom.IDriver.OsPlatform
        Get
            Return _OsPlatform
        End Get
    End Property
    Public Property StartAuto() As Boolean Implements HoMIDom.HoMIDom.IDriver.StartAuto
        Get
            Return _StartAuto
        End Get
        Set(ByVal value As Boolean)
            _StartAuto = value
        End Set
    End Property
    Public Property AutoDiscover() As Boolean Implements HoMIDom.HoMIDom.IDriver.AutoDiscover
        Get
            Return _AutoDiscover
        End Get
        Set(ByVal value As Boolean)
            _AutoDiscover = value
        End Set
    End Property
#End Region

#Region "Fonctions génériques"
    ''' <summary>
    ''' Retourne la liste des Commandes avancées
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCommandPlus() As List(Of DeviceCommande)
        Return _DeviceCommandPlus
    End Function

    ''' <summary>Execute une commande avancée</summary>
    ''' <param name="MyDevice">Objet représentant le Device </param>
    ''' <param name="Command">Nom de la commande avancée à éxécuter</param>
    ''' <param name="Param">tableau de paramétres</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ExecuteCommand(ByVal MyDevice As Object, ByVal Command As String, Optional ByVal Param() As Object = Nothing) As Boolean
        Dim retour As Boolean = False
        Try
            If MyDevice IsNot Nothing Then
                'Pas de commande demandée donc erreur
                If Command = "" Then
                    Return False
                Else
                    'Write(deviceobject, Command, Param(0), Param(1))
                    Select Case UCase(Command)
                        Case ""
                        Case Else
                    End Select
                    Return True
                End If
            Else
                Return False
            End If
        Catch ex As Exception
            WriteLog("ERR: ExecuteCommand exception : " & ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Permet de vérifier si un champ est valide
    ''' </summary>
    ''' <param name="Champ"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function VerifChamp(ByVal Champ As String, ByVal Value As Object) As String Implements HoMIDom.HoMIDom.IDriver.VerifChamp

        Try
            Dim retour As String = "0"
            Select Case UCase(Champ)
                Case "ADRESSE1"
                    If Value IsNot Nothing Then
                        If String.IsNullOrEmpty(Value) Then
                            retour = "Veuillez saisir le nom du module en respectant la casse ( maj/minuscule )"
                        End If
                    End If
            End Select
            Return retour
        Catch ex As Exception
            Return "Une erreur est apparue lors de la vérification du champ " & Champ & ": " & ex.ToString
        End Try
    End Function

    ''' <summary>Démarrer le driver</summary>
    ''' <remarks></remarks>
    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        Try
            'récupération des paramétres avancés
            Try
                _DEBUG = _Parametres.Item(0).Valeur
                First = True
            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Erreur dans les paramétres avancés. utilisation des valeur par défaut : " & ex.Message)
            End Try

            If GetRefreshToken("MyFox", "https://api.myfox.me/oauth2/token") Then


                If _Refresh > 0 Then
                    If _Refresh < 60 Then
                        _Refresh = 60
                    End If

                    cptBeforeToken = (Auth.expires_in / _Refresh) - 2

                    MyTimer.Interval = _Refresh * 1000
                    MyTimer.Enabled = True
                    AddHandler MyTimer.Elapsed, AddressOf TimerTick
                End If

                ScanData()
                _IsConnect = True
                cpt_restart = 0
                WriteLog("Driver " & Me.Nom & " démarré")

            Else
                cpt_restart += 1
                If cpt_restart < 4 Then
                    GetRefreshToken("MyFox", "https://api.myfox.me/oauth2/token")
                    Start()
                Else
                    _IsConnect = False
                    WriteLog("Driver " & Me.Nom & " non démarré")
                    WriteLog("ERR: Verifié que votre authentification est valide avec HoMIAdmiN dans HoMIDoM/Config")
                    cpt_restart = 0
                End If
                Exit Sub
            End If

        Catch ex As Exception
            _IsConnect = False
            WriteLog("Driver " & Me.Nom & " non démarré")
            WriteLog("ERR: START Driver " & Me.Nom & " Erreur démarrage " & ex.Message)
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            _IsConnect = False
            WriteLog("Driver " & Me.Nom & " arrêté")
        Catch ex As Exception
            WriteLog("ERR: STOP Driver " & Me.Nom & " Erreur arrêt " & ex.Message)
        End Try
    End Sub

    ''' <summary>Re-Démarrer le du driver</summary>
    ''' <remarks></remarks>
    Public Sub Restart() Implements HoMIDom.HoMIDom.IDriver.Restart
        [Stop]()
        Start()
    End Sub

    ''' <summary>Intérroger un device</summary>
    ''' <param name="Objet">Objet représetant le device à interroger</param>
    ''' <remarks>pas utilisé</remarks>
    Public Sub Read(ByVal Objet As Object) Implements HoMIDom.HoMIDom.IDriver.Read

        Try

            If _Enable = False Then Exit Sub

            If _IsConnect = False Then
                WriteLog("ERR: READ, Le driver n'est pas démarré, impossible d'écrire sur le port")
                Exit Sub
            End If

            'récupération des paramétres avancés pour rafraichir l'affichage
            Try
                _DEBUG = _Parametres.Item(0).Valeur
            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Erreur dans les paramétres avancés. utilisation des valeur par défaut : " & ex.Message)
            End Try

            If statelist.items.Count > 0 Then
                For Each state1 In statelist.items
                    If state1.label = Objet.adresse1 Then
                        If TypeOf Objet.Value Is Boolean Then
                            If state1.stateLabel = "opened" Then
                                Objet.Value = True
                            ElseIf state1.stateLabel = "closed" Then
                                Objet.Value = False
                            End If
                        ElseIf TypeOf Objet.Value Is String Then
                            Objet.value = state1.stateLabel
                        Else
                            If state1.stateLabel = "opened" Then
                                Objet.Value = 1
                            ElseIf state1.stateLabel = "closed" Then
                                Objet.Value = 0
                            End If
                        End If
                    End If
                Next
            End If

        Catch ex As Exception
            WriteLog("ERR: READ, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Commander un device</summary>
    ''' <param name="Objet">Objet représentant le device à interroger</param>
    ''' <param name="Command">La commande à passer</param>
    ''' <param name="Parametre1"></param>
    ''' <param name="Parametre2"></param>
    ''' <remarks></remarks>
    Public Sub Write(ByVal Objet As Object, ByVal Command As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
        Try
            If _Enable = False Then Exit Sub

            If _IsConnect = False Then
                WriteLog("ERR: READ, Le driver n'est pas démarré, impossible d'écrire sur le port")
                Exit Sub
            End If

            Dim client As New Net.WebClient
            Dim responsebody = client.DownloadString("https://api.myfox.me:443/v2/site/" & sitelist.siteId & "/device/" & Objet.adresse1 & "/socket/" & Command & "?access_token=" & Auth.access_token)

        Catch ex As Exception
            WriteLog("ERR: Write, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice
        Try

        Catch ex As Exception
            WriteLog("ERR: DeleteDevice, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice
        Try

        Catch ex As Exception
            WriteLog("ERR: NewDevice, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>ajout des commandes avancées pour les devices</summary>
    ''' <param name="nom">Nom de la commande avancée</param>
    ''' <param name="description">Description qui sera affichée dans l'admin</param>
    ''' <param name="nbparam">Nombre de parametres attendus</param>
    ''' <remarks></remarks>
    Private Sub Add_DeviceCommande(ByVal Nom As String, ByVal Description As String, ByVal NbParam As Integer)
        Try
            Dim x As New DeviceCommande
            x.NameCommand = Nom
            x.DescriptionCommand = Description
            x.CountParam = NbParam
            _DeviceCommandPlus.Add(x)
        Catch ex As Exception
            WriteLog("ERR: add_devicecommande, Exception :" & ex.Message)
        End Try
    End Sub

    ''' <summary>ajout Libellé pour le Driver</summary>
    ''' <param name="nom">Nom du champ : HELP</param>
    ''' <param name="labelchamp">Nom à afficher : Aide</param>
    ''' <param name="tooltip">Tooltip à afficher au dessus du champs dans l'admin</param>
    ''' <remarks></remarks>
    Private Sub Add_LibelleDriver(ByVal Nom As String, ByVal Labelchamp As String, ByVal Tooltip As String, Optional ByVal Parametre As String = "")
        Try
            Dim y0 As New HoMIDom.HoMIDom.Driver.cLabels
            y0.LabelChamp = Labelchamp
            y0.NomChamp = UCase(Nom)
            y0.Tooltip = Tooltip
            y0.Parametre = Parametre
            _LabelsDriver.Add(y0)
        Catch ex As Exception
            WriteLog("ERR: add_devicecommande, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Ajout Libellé pour les Devices</summary>
    ''' <param name="nom">Nom du champ : HELP</param>
    ''' <param name="labelchamp">Nom à afficher : Aide, si = "@" alors le champ ne sera pas affiché</param>
    ''' <param name="tooltip">Tooltip à afficher au dessus du champs dans l'admin</param>
    ''' <remarks></remarks>
    Private Sub Add_LibelleDevice(ByVal Nom As String, ByVal Labelchamp As String, ByVal Tooltip As String, Optional ByVal Parametre As String = "")
        Try
            Dim ld0 As New HoMIDom.HoMIDom.Driver.cLabels
            ld0.LabelChamp = Labelchamp
            ld0.NomChamp = UCase(Nom)
            ld0.Tooltip = Tooltip
            ld0.Parametre = Parametre
            _LabelsDevice.Add(ld0)
        Catch ex As Exception
            WriteLog("ERR: add_devicecommande, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>ajout de parametre avancés</summary>
    ''' <param name="nom">Nom du parametre (sans espace)</param>
    ''' <param name="description">Description du parametre</param>
    ''' <param name="valeur">Sa valeur</param>
    ''' <remarks></remarks>
    Private Sub Add_ParamAvance(ByVal nom As String, ByVal description As String, ByVal valeur As Object)
        Try
            Dim x As New HoMIDom.HoMIDom.Driver.Parametre
            x.Nom = nom
            x.Description = description
            x.Valeur = valeur
            _Parametres.Add(x)
        Catch ex As Exception
            WriteLog("ERR: add_devicecommande, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Creation d'un objet de type</summary>
    ''' <remarks></remarks>
    Public Sub New()
        Try
            _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString

            'liste des devices compatibles
            _DeviceSupport.Add(ListeDevices.METEO)
            _DeviceSupport.Add(ListeDevices.BAROMETRE)
            _DeviceSupport.Add(ListeDevices.UV)
            _DeviceSupport.Add(ListeDevices.VITESSEVENT)
            _DeviceSupport.Add(ListeDevices.DIRECTIONVENT)
            _DeviceSupport.Add(ListeDevices.TEMPERATURE)
            _DeviceSupport.Add(ListeDevices.BATTERIE)
            _DeviceSupport.Add(ListeDevices.PLUIECOURANT)
            _DeviceSupport.Add(ListeDevices.HUMIDITE)
            _DeviceSupport.Add(ListeDevices.PLUIETOTAL)
            _DeviceSupport.Add(ListeDevices.TEMPERATURECONSIGNE)
            _DeviceSupport.Add(ListeDevices.TEMPERATURE)
            _DeviceSupport.Add(ListeDevices.HUMIDITE)
            _DeviceSupport.Add(ListeDevices.LAMPE)
            _DeviceSupport.Add(ListeDevices.LAMPERGBW)
            _DeviceSupport.Add(ListeDevices.SWITCH)
            _DeviceSupport.Add(ListeDevices.APPAREIL)
            _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN)
            _DeviceSupport.Add(ListeDevices.GENERIQUESTRING)
            _DeviceSupport.Add(ListeDevices.GENERIQUEVALUE)
            _DeviceSupport.Add(ListeDevices.VOLET)

            'Parametres avancés
            Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "@", "")
            Add_LibelleDevice("ADRESSE2", "@", "")
            Add_LibelleDevice("REFRESH", "Refresh en sec", "Minimum 600, valeur rafraicissement station", "600")
            ' Libellés Device inutiles
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("MODELE", "@", "")
            Add_LibelleDevice("LASTCHANGEDUREE", "@", "")
        Catch ex As Exception
            WriteLog("ERR: New, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)
        ' Attente de 3s pour eviter le relancement de la procedure dans le laps de temps
        'System.Threading.Thread.Sleep(3000)
        If cpt >= cptBeforeToken Then
            GetRefreshToken("MyFox", "https://api.myfox.me/oauth2/token")
            cpt = 0
        End If
        ScanData()
    End Sub

#End Region

#Region "Fonctions internes"

    Private Function GetAccessToken(ByVal clientOauth As String) As Boolean

        Try

            Dim fileName = My.Application.Info.DirectoryPath & "\config\reponse_accesstoken_" & clientOauth & ".json"

            If System.IO.File.Exists(fileName) Then

                Dim stream = System.IO.File.ReadAllText(fileName)
                Auth = Newtonsoft.Json.JsonConvert.DeserializeObject(stream, GetType(Authentication))
                'va chercher les module que si connecté
                If Auth.expires_in > 0 Then
                    WriteLog("DBG: Token : " & Auth.access_token)
                    Return True
                    Exit Function
                End If
            Else
                WriteLog("ERR: GetAccessToken,  Le fichier " & fileName & " n'existe pas")
            End If
            Return False
        Catch ex As Exception
            cpt_restart += 1
            If cpt_restart < 4 Then
                GetRefreshToken("MyFox", "https://api.myfox.me/oauth2/token")
                Return True
            Else
                WriteLog("ERR: Verifié que votre authentification est valide avec HoMIAdmiN dans HoMIDoM/Config")
                WriteLog("ERR: GetAccessToken, Exception : " & ex.Message)
                Return False
            End If

        End Try
    End Function

    Private Function GetRefreshToken(ByVal clientOauth As String, ByVal httpsOauth As String) As Boolean
        Try
            If GetAccessToken("MyFox") Then
                Dim client As New Net.WebClient
                Dim reqparm As New Specialized.NameValueCollection
                Dim OAuth2 = New HoMIOAuth2.HoMIOAuth2(_IdSrv, _Server.GetIPSOAP, _Server.GetPortSOAP, "HoMIDoM")
                reqparm.Add("grant_type", "refresh_token")
                reqparm.Add("refresh_token", Auth.refresh_token)
                reqparm.Add("client_id", OAuth2.GetClientFile(clientOauth).web.client_id)
                reqparm.Add("client_secret", OAuth2.GetClientFile(clientOauth).web.client_secret)
                Dim responsebytes = client.UploadValues(httpsOauth, "POST", reqparm)
                Dim responsebody = (New System.Text.UTF8Encoding).GetString(responsebytes)
                Auth = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebody, GetType(Authentication))

                If Auth.expires_in > 0 And Auth.refresh_token <> Nothing Then
                    Dim stream = Newtonsoft.Json.JsonConvert.SerializeObject(Auth)
                    System.IO.File.WriteAllText(My.Application.Info.DirectoryPath & "\config\reponse_accesstoken_" & clientOauth & ".json", stream)
                    WriteLog("DBG: " & Me.Nom & " RefreshToken, Requête " & httpsOauth & " OK")
                    WriteLog("DBG: " & Me.Nom & " RefreshToken, Connect : " & responsebody.ToString)
                Else
                    WriteLog("ERR: " & Me.Nom & " RefreshToken, Non connecté")
                End If
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            WriteLog("ERR: " & Me.Nom & " RefreshToken" & ex.Message)
            Return True
        End Try
    End Function

    Private Sub ScanData()

        Try
            Dim client As New Net.WebClient
            Dim responsebody As String
            Dim IdLib As String = ""

            If First Then
                Try
line1:
                    responsebody = client.DownloadString("https://api.myfox.me:443/v2/client/site/items?access_token=" & Auth.access_token)
                Catch ex As Exception
                    ' recherche du device/module a interroger
                    cpt_restart += 1
                    If cpt_restart < 4 Then
                        GetRefreshToken("MyFox", "https://api.myfox.me/oauth2/token")
                        GoTo line1
                    Else
                        WriteLog("ERR: Verifié que votre authentification est valide avec HoMIAdmiN dans HoMIDoM/Config")
                        WriteLog("ERR: ScanData, Exception : " & ex.Message)
                        cpt_restart = 0
                        Exit Sub
                    End If
                End Try

                devices = New List(Of String)
                sitelist = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebody, GetType(Site))

            End If

            statelist = Nothing
            socketlist = Nothing

            If sitelist.deviceStateCount > 0 Then
                responsebody = client.DownloadString("https://api.myfox.me:443/v2/site/" & sitelist.siteId & "/device/data/state/items?access_token=" & Auth.access_token)
                statelist = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebody, GetType(DeviceWithStateCollection))
                WriteLog("DBG: ScanData State : " & responsebody.ToString)
            End If
           
            If First Then
                If sitelist.socketCount > 0 Then
                    responsebody = client.DownloadString("https://api.myfox.me:443/v2/site/" & sitelist.siteId & "/device/socket/items?access_token=" & Auth.access_token)
                    socketlist = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebody, GetType(SocketCollection))
                    WriteLog("DBG: ScanData Socket : " & responsebody.ToString)
                End If
                If statelist IsNot Nothing Then
                    For Each state1 In statelist.items
                        devices.Add(state1.label)
                        If IdLib <> "" Then IdLib += "|"
                        IdLib += state1.label
                    Next
                End If
                If statelist IsNot Nothing Then
                    For Each socket1 In socketlist.items
                        devices.Add(socket1.label)
                        If IdLib <> "" Then IdLib += "|"
                        IdLib += socket1.label
                    Next
                End If
                Add_LibelleDevice("ADRESSE1", "Nom du module", "Nom du module en respectant maj./minuscule", IdLib)
                First = False
            End If
            cpt += 1
            cpt_restart = 0
        Catch ex As Exception
            WriteLog("ERR: ScanData, Exception : " & ex.Message)
        End Try
    End Sub

    Private Sub WriteLog(ByVal message As String)
        Try
            'utilise la fonction de base pour loguer un event
            If STRGS.InStr(message, "DBG:") > 0 Then
                If _DEBUG Then
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom, STRGS.Right(message, message.Length - 5))
                End If
            ElseIf STRGS.InStr(message, "ERR:") > 0 Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom, STRGS.Right(message, message.Length - 5))
            Else
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom, message)
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " WriteLog", ex.Message)
        End Try
    End Sub

#End Region

End Class
