Imports HoMIDom
Imports HoMIDom.HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device

Imports System.Text.RegularExpressions
Imports STRGS = Microsoft.VisualBasic.Strings
Imports HoMIOAuth2

' Auteur : domomath35 sur une base HoMIDoM
' Date : 30/03/2015

''' <summary>Driver Nest Reception de données de Thermostat intelligent et de detecteur de Fumée/Co2</summary>
''' <remarks></remarks>
<Serializable()> Public Class Driver_Nest
    Implements IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "8425877A-0478-11E5-9454-977C1E5D46B0"
    Dim _Nom As String = "Nest"
    Dim _Enable As Boolean = False
    Dim _Description As String = "Données Nest"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "API WEB"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "@"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "Nest"
    Dim _Version As String = My.Application.Info.Version.ToString
    Dim _OsPlatform As String = "3264"
    Dim _Picture As String = ""
    Dim _Server As Server
    Dim _Device As Device
    Dim _DeviceSupport As New ArrayList
    Dim _Parametres As New ArrayList
    Dim _LabelsDriver As New ArrayList
    Dim _LabelsDevice As New ArrayList
    Dim MyTimer As New Timers.Timer
    Dim _IdSrv As String
    Dim _DeviceCommandPlus As New List(Of Device.DeviceCommande)
    Dim _AutoDiscover As Boolean = False

    'A ajouter dans les ppt du driver
    Dim _tempsentrereponse As Integer = 1500
    Dim _ignoreadresse As Boolean = False
    Dim _lastetat As Boolean = True

    'param avancé
    Dim _DEBUG As Boolean = False

#End Region

#Region "Variables internes"


    Dim Auth As Authentication
    Dim devlist As All
    Dim First As Boolean = True
    Dim cpt_restart As Integer = 0

    Dim cptBeforeToken As Integer = 0
    Dim cpt As Integer = 0

    Dim detect As New Dictionary(Of String, Object)
    Dim thermo As New Dictionary(Of String, Object)

    Public Class All
        Public Property devices() As Devices
        Public Property structures() As Dictionary(Of String, [Structure])
    End Class

    Public Class Devices
        Public Property thermostats() As Dictionary(Of String, Thermostat)
        Public Property smoke_co_alarms() As Dictionary(Of String, smoke_co_alarm)
    End Class

    Public Class [Structure]
        Public Property name As String
        Public Property country_code As String
        Public Property time_zone As String
        Public Property away As String
        Public Property thermostats As IList(Of String)
        Public Property structure_id As String
    End Class

    Public Class Thermostat
        Public Property humidity As Integer
        Public Property locale As String
        Public Property temperature_scale As String
        Public Property is_using_emergency_heat As Boolean
        Public Property has_fan As Boolean
        Public Property software_version As String
        Public Property has_leaf As Boolean
        Public Property device_id As String
        Public Property name As String
        Public Property can_heat As Boolean
        Public Property can_cool As Boolean
        Public Property hvac_mode As String
        Public Property target_temperature_c As Double
        Public Property target_temperature_f As Integer
        Public Property target_temperature_high_c As Double
        Public Property target_temperature_high_f As Integer
        Public Property target_temperature_low_c As Double
        Public Property target_temperature_low_f As Integer
        Public Property ambient_temperature_c As Double
        Public Property ambient_temperature_f As Integer
        Public Property away_temperature_high_c As Double
        Public Property away_temperature_high_f As Integer
        Public Property away_temperature_low_c As Double
        Public Property away_temperature_low_f As Integer
        Public Property structure_id As String
        Public Property fan_timer_active As Boolean
        Public Property name_long As String
        Public Property is_online As Boolean
        Public Property last_connection As String
    End Class

    Public Class metadata
        Public Property access_token As String
        Public Property client_version As Integer
    End Class

    Public Class smoke_co_alarm

        Public Property device_id As String
        Public Property locale As String
        Public Property software_version As String
        Public Property structure_id As String
        Public Property name As String
        Public Property name_long As String
        Public Property last_connection As String
        Public Property is_online As Boolean
        Public Property battery_health As String
        Public Property co_alarm_state As String
        Public Property smoke_alarm_state As String
        Public Property is_manual_test_active As Boolean
        Public Property last_manual_test_time As String
        Public Property ui_color_state As String
    End Class

#End Region

#Region "Propriétés génériques"
    Public WriteOnly Property IdSrv As String Implements IDriver.IdSrv
        Set(ByVal value As String)
            _IdSrv = value
        End Set
    End Property

    Public Property COM() As String Implements IDriver.COM
        Get
            Return _Com
        End Get
        Set(ByVal value As String)
            _Com = value
        End Set
    End Property
    Public ReadOnly Property Description() As String Implements IDriver.Description
        Get
            Return _Description
        End Get
    End Property
    Public ReadOnly Property DeviceSupport() As System.Collections.ArrayList Implements IDriver.DeviceSupport
        Get
            Return _DeviceSupport
        End Get
    End Property
    Public Property Parametres() As System.Collections.ArrayList Implements IDriver.Parametres
        Get
            Return _Parametres
        End Get
        Set(ByVal value As System.Collections.ArrayList)
            _Parametres = value
        End Set
    End Property

    Public Property LabelsDriver() As System.Collections.ArrayList Implements IDriver.LabelsDriver
        Get
            Return _LabelsDriver
        End Get
        Set(ByVal value As System.Collections.ArrayList)
            _LabelsDriver = value
        End Set
    End Property
    Public Property LabelsDevice() As System.Collections.ArrayList Implements IDriver.LabelsDevice
        Get
            Return _LabelsDevice
        End Get
        Set(ByVal value As System.Collections.ArrayList)
            _LabelsDevice = value
        End Set
    End Property



    Public Event DriverEvent(ByVal DriveName As String, ByVal TypeEvent As String, ByVal Parametre As Object) Implements IDriver.DriverEvent

    Public Property Enable() As Boolean Implements IDriver.Enable
        Get
            Return _Enable
        End Get
        Set(ByVal value As Boolean)
            _Enable = value
        End Set
    End Property
    Public ReadOnly Property ID() As String Implements IDriver.ID
        Get
            Return _ID
        End Get
    End Property
    Public Property IP_TCP() As String Implements IDriver.IP_TCP
        Get
            Return _IP_TCP
        End Get
        Set(ByVal value As String)
            _IP_TCP = value
        End Set
    End Property
    Public Property IP_UDP() As String Implements IDriver.IP_UDP
        Get
            Return _IP_UDP
        End Get
        Set(ByVal value As String)
            _IP_UDP = value
        End Set
    End Property
    Public ReadOnly Property IsConnect() As Boolean Implements IDriver.IsConnect
        Get
            Return _IsConnect
        End Get
    End Property
    Public Property Modele() As String Implements IDriver.Modele
        Get
            Return _Modele
        End Get
        Set(ByVal value As String)
            _Modele = value
        End Set
    End Property
    Public ReadOnly Property Nom() As String Implements IDriver.Nom
        Get
            Return _Nom
        End Get
    End Property
    Public Property Picture() As String Implements IDriver.Picture
        Get
            Return _Picture
        End Get
        Set(ByVal value As String)
            _Picture = value
        End Set
    End Property
    Public Property Port_TCP() As String Implements IDriver.Port_TCP
        Get
            Return _Port_TCP
        End Get
        Set(ByVal value As String)
            _Port_TCP = value
        End Set
    End Property
    Public Property Port_UDP() As String Implements IDriver.Port_UDP
        Get
            Return _Port_UDP
        End Get
        Set(ByVal value As String)
            _Port_UDP = value
        End Set
    End Property
    Public ReadOnly Property Protocol() As String Implements IDriver.Protocol
        Get
            Return _Protocol
        End Get
    End Property
    Public Property Refresh() As Integer Implements IDriver.Refresh
        Get
            Return _Refresh
        End Get
        Set(ByVal value As Integer)
            _Refresh = value
        End Set
    End Property
    Public Property Server() As Server Implements IDriver.Server
        Get
            Return _Server
        End Get
        Set(ByVal value As Server)
            _Server = value
        End Set
    End Property
    Public ReadOnly Property Version() As String Implements IDriver.Version
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

            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Erreur dans les paramétres avancés. utilisation des valeur par défaut : " & ex.Message)
            End Try

            If GetAccessToken("Nest") Then

                First = True
                _IsConnect = True

                If _Refresh > 0 Then
                    If _Refresh < 60 Then
                        _Refresh = 60
                    End If
                    cptBeforeToken = (Auth.expires_in / _Refresh) - 2
                    MyTimer.Interval = _Refresh * 1000
                    MyTimer.Enabled = True
                    AddHandler MyTimer.Elapsed, AddressOf TimerTick
                End If

                cpt_restart = 0
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom, "Driver " & Me.Nom & " démarré")

            Else
                cpt_restart += 1
                If cpt_restart < 4 Then
                    'GetRefreshToken("Nest", "https://home.nest.com/login/oauth2")
                    Start()
                Else
                    _IsConnect = False
                    WriteLog("Driver " & Me.Nom & " non démarré")
                    WriteLog("ERR: Verifié que votre authentification est valide avec HoMIAdmiN dans HoMIDoM/Config")
                End If
            End If

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GoogleCalendar Start", ex.Message & ex.Data.ToString)
        End Try

    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            If _Refresh > 0 Then
                MyTimer.Enabled = False
                RemoveHandler MyTimer.Elapsed, AddressOf TimerTick
            End If
            _IsConnect = False
            WriteLog("Driver " & Me.Nom & " arrêté")
        Catch ex As Exception
            WriteLog("ERR: Driver " & Me.Nom & " Erreur arrêt " & ex.Message)
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
                WriteLog("ERR: READ, Le driver n'est pas démarré, impossible de lire le port")
                Exit Sub
            End If


            'Si internet n'est pas disponible on ne mets pas à jour les informations
            If My.Computer.Network.IsAvailable = False Then
                Exit Sub
            End If

            ' recherche du device/module a interroger

            If Not First Then
                GetData(Objet)
            End If

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", ex.Message)
            WriteLog("ERR: Read, Exception : " & ex.Message)
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

        Catch ex As Exception
            WriteLog("ERR: Write, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub DeleteDevice(ByVal DeviceId As String) Implements IDriver.DeleteDevice
        Try

        Catch ex As Exception
            WriteLog("ERR: DeleteDevice, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub NewDevice(ByVal DeviceId As String) Implements IDriver.NewDevice
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
            Dim y0 As New Driver.cLabels
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
            Dim ld0 As New Driver.cLabels
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
            Dim x As New Driver.Parametre
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
            _DeviceSupport.Add(ListeDevices.METEO.ToString)
            _DeviceSupport.Add(ListeDevices.BATTERIE.ToString)
            _DeviceSupport.Add(ListeDevices.APPAREIL.ToString)
            _DeviceSupport.Add(ListeDevices.COMPTEUR.ToString)
            _DeviceSupport.Add(ListeDevices.CONTACT.ToString)
            _DeviceSupport.Add(ListeDevices.DETECTEUR.ToString)
            _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN.ToString)
            _DeviceSupport.Add(ListeDevices.GENERIQUEVALUE.ToString)
            _DeviceSupport.Add(ListeDevices.GENERIQUESTRING.ToString)
            _DeviceSupport.Add(ListeDevices.HUMIDITE.ToString)
            _DeviceSupport.Add(ListeDevices.SWITCH.ToString)
            _DeviceSupport.Add(ListeDevices.TEMPERATURE.ToString)
            _DeviceSupport.Add(ListeDevices.TEMPERATURECONSIGNE.ToString)

            'Parametres avancés
            Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Nom du module", "Nom du module ", "")
            Add_LibelleDevice("MODELE", "Donnée à lire", "Donnée à lire dans le module ", "device_id|locale|software_version|" & _
                    "structure_id|name|name_long|last_connection|is_online|" & _
                    "battery_health|co_alarm_state|smoke_alarm_state|is_manual_test_active|last_manual_test_time|ui_color_state|" & _
                    "humidity|temperature_scale|is_using_emergency_heat|has_fan|has_leaf|can_heat|can_cool|" & _
                    "hvac_mode|target_temperature_c|target_temperature_f|target_temperature_high_c|target_temperature_high_f|" & _
                    "target_temperature_low_c|target_temperature_low_f|ambient_temperature_c|ambient_temperature_f|" & _
                    "away_temperature_high_c|away_temperature_high_f|away_temperature_low_c|away_temperature_low_f|fan_timer_active")

            Add_LibelleDevice("REFRESH", "Refresh en sec", "Minimum 600, valeur rafraicissement station", "600")

            ' Libellés Device inutiles
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("ADRESSE2", "@", "")
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
        'If cpt >= cptBeforeToken Then
        'RefreshToken("Nest", "https://home.nest.com/login/oauth2")
        'cpt = 0
        'End If
        ScanData()
    End Sub

#End Region

#Region "Fonctions internes"

    Private Function GetAccessToken(ByVal clientOauth As String) As Boolean

        Try

            Try ' lecture de la variable debug, permet de rafraichir la variable debug sans redemarrer le service
                _DEBUG = _Parametres.Item(0).Valeur
            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Erreur de lecture de debug : " & ex.Message)
            End Try

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
            WriteLog("ERR: GetAccessToken, Exception : " & ex.Message)
            Return False
        End Try
    End Function

    Private Sub GetRefreshToken(ByVal clientOauth As String, ByVal httpsOauth As String)
        Try
            Dim client As New Net.WebClient
            Dim reqparm As New Specialized.NameValueCollection
            Dim OAuth2 = New HoMIOAuth2.HoMIOAuth2(_IdSrv, _Server.GetIPSOAP, _Server.GetPortSOAP, "HoMIDoM")
            reqparm.Add("client_id", OAuth2.GetClientFile(clientOauth).web.client_id)
            reqparm.Add("client_secret", OAuth2.GetClientFile(clientOauth).web.client_secret)
            reqparm.Add("refresh_token", Auth.refresh_token)
            reqparm.Add("grant_type", "refresh_token")
            Dim responsebytes = client.UploadValues(httpsOauth, "POST", reqparm)
            Dim responsebody = (New System.Text.UTF8Encoding).GetString(responsebytes)
            Auth = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebody, GetType(Authentication))

            Dim stream = Newtonsoft.Json.JsonConvert.SerializeObject(Auth)
            System.IO.File.WriteAllText(My.Application.Info.DirectoryPath & "\config\reponse_accesstoken_" & clientOauth & ".json", stream)
            If Auth.expires_in > 0 Then
                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " RefreshToken : ", "Requête " & httpsOauth & " OK")
                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " RefreshToken", "Connect : " & responsebody.ToString)
            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " RefreshToken", "Non connecté")
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " RefreshToken", "Exception : " & ex.Message)
        End Try
    End Sub

    Private Sub ScanData()

        Try
line1:
            Dim IdLib As String = ""
            Dim client As New Net.WebClient
            Dim responsebody = client.DownloadString("https://developer-api.nest.com/?auth=" & Auth.access_token)
            devlist = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebody, GetType(All))

            WriteLog("DBG: GetData : " & responsebody.ToString)
            If First Then

                If devlist.devices.smoke_co_alarms IsNot Nothing Then
                    WriteLog("DBG: Nombre de detecteur Fumée/Co2 : " & devlist.devices.smoke_co_alarms.Count)
                    detect.Clear()
                    detect.Add("last_connection", "GENERIQUESTRING")
                    detect.Add("is_online", "GENERIQUEBOOLEAN")
                    detect.Add("battery_health", "GENERIQUESTRING")
                    detect.Add("co_alarm_state", "GENERIQUESTRING")
                    detect.Add("smoke_alarm_state", "GENERIQUESTRING")
                    detect.Add("is_manual_test_active", "GENERIQUEBOOLEAN")
                    detect.Add("last_manual_test_time", "GENERIQUESTRING")
                    For i = 0 To devlist.devices.smoke_co_alarms.Count - 1
                        IdLib = "Detecteur " & devlist.devices.smoke_co_alarms.Values(i).name
                        If i < devlist.devices.smoke_co_alarms.Count - 1 Then IdLib += "|"
                        WriteLog("DBG: Detecteur Fumée/Co2 : " & devlist.devices.smoke_co_alarms.Values(i).name & ", ID = " & devlist.devices.smoke_co_alarms.Values(i).device_id)
                        If _AutoDiscover Or _Server.GetModeDecouverte Then
                            For Each det In detect
                                Dim listedevicessearch As New ArrayList
                                listedevicessearch = _Server.ReturnDeviceByAdresse2TypeDriver(_IdSrv, det.Key, "", Me._ID, True)
                                If listedevicessearch.Count = i Then
                                    _Server.AddDetectNewDevice("Detecteur " & devlist.devices.smoke_co_alarms.Values(i).name, _ID, det.Value, det.Key, "")
                                End If
                            Next
                        End If

                    Next
                End If

                If devlist.devices.thermostats IsNot Nothing Then
                    WriteLog("DBG: Nombre de Thermostat : " & devlist.devices.thermostats.Count)
                    thermo.Clear()
                    thermo.Add("humidity", "GENERIQUEVALUE")
                    thermo.Add("temperature_scale", "GENERIQUESTRING")
                    thermo.Add("is_using_emergency_heat", "GENERIQUEBOOLEAN")
                    thermo.Add("has_fan", "GENERIQUEBOOLEAN")
                    thermo.Add("has_leaf", "GENERIQUEBOOLEAN")
                    thermo.Add("can_heat", "GENERIQUEBOOLEAN")
                    thermo.Add("can_cool", "GENERIQUEBOOLEAN")
                    thermo.Add("hvac_mode", "GENERIQUESTRING")
                    thermo.Add("target_temperature_c", "GENERIQUEVALUE")
                    thermo.Add("target_temperature_f", "GENERIQUEVALUE")
                    thermo.Add("target_temperature_high_c", "GENERIQUEVALUE")
                    thermo.Add("target_temperature_high_f", "GENERIQUEVALUE")
                    thermo.Add("target_temperature_low_c", "GENERIQUEVALUE")
                    thermo.Add("target_temperature_low_f", "GENERIQUEVALUE")
                    thermo.Add("ambient_temperature_c", "GENERIQUEVALUE")
                    thermo.Add("ambient_temperature_f", "GENERIQUEVALUE")
                    thermo.Add("away_temperature_high_c", "GENERIQUEVALUE")
                    thermo.Add("away_temperature_high_f", "GENERIQUEVALUE")
                    thermo.Add("away_temperature_low_c", "GENERIQUEVALUE")
                    thermo.Add("away_temperature_low_f", "GENERIQUEVALUE")
                    thermo.Add("fan_timer_active", "GENERIQUEBOOLEAN")
                    thermo.Add("is_online", "GENERIQUEBOOLEAN")
                    thermo.Add("last_connection", "GENERIQUESTRING")
                    For i = 0 To devlist.devices.thermostats.Count - 1
                        If IdLib <> "" Then IdLib += "|"
                        IdLib += "Thermostat " & devlist.devices.thermostats.Values(i).name
                        If i < devlist.devices.thermostats.Count - 1 Then IdLib += "|"
                        WriteLog("DBG: Thermostat : " & devlist.devices.thermostats.Values(i).name & ", ID = " & devlist.devices.thermostats.Values(i).device_id)
                        If _AutoDiscover Or _Server.GetModeDecouverte Then
                            For Each det In thermo
                                Dim listedevicessearch As New ArrayList
                                listedevicessearch = _Server.ReturnDeviceByAdresse2TypeDriver(_IdSrv, det.Key, "", Me._ID, True)
                                If listedevicessearch.Count = i Then
                                    _Server.AddDetectNewDevice("Thermostat " & devlist.devices.thermostats.Values(i).name, _ID, det.Value, det.Key, "")
                                End If
                            Next
                        End If
                    Next
                End If

                Add_LibelleDevice("ADRESSE1", "Nom du module", "Nom du module ", IdLib)
                First = False
            End If
            cpt += 1
            cpt_restart = 0
        Catch ex As Exception

            cpt_restart += 1
            If cpt_restart < 4 Then
                'GetRefreshToken("Nest", "https://home.nest.com/login/oauth2")
                GoTo line1
            Else
                WriteLog("ERR: Verifié que votre authentification est valide avec HoMIAdmiN dans HoMIDoM/Config")
                Restart()
            End If

            WriteLog("DBG: ScanData, Exception : " & ex.Message)
        End Try
    End Sub


    Private Sub GetData(ByVal Objet As Object)
        Try
            If devlist.devices.smoke_co_alarms IsNot Nothing Then
                For i = 0 To devlist.devices.smoke_co_alarms.Count - 1
                    If Objet.adresse1 = "Detecteur " & devlist.devices.smoke_co_alarms.Values(i).name Then

                        If TypeOf (Objet.value) Is String Then
                            Select Case Objet.modele

                                Case "locale"
                                    Objet.value = devlist.devices.smoke_co_alarms.Values(i).locale
                                Case "name"
                                    Objet.value = devlist.devices.smoke_co_alarms.Values(i).name
                                Case "name_long"
                                    Objet.value = devlist.devices.smoke_co_alarms.Values(i).name_long
                                Case "last_connection"
                                    Objet.value = devlist.devices.smoke_co_alarms.Values(i).last_connection
                                Case "battery_health"
                                    Objet.value = devlist.devices.smoke_co_alarms.Values(i).battery_health
                                Case "co_alarm_state"
                                    Objet.value = devlist.devices.smoke_co_alarms.Values(i).co_alarm_state
                                Case "smoke_alarm_state"
                                    Objet.value = devlist.devices.smoke_co_alarms.Values(i).smoke_alarm_state
                                Case "last_manual_test_time"
                                    Objet.value = devlist.devices.smoke_co_alarms.Values(i).last_manual_test_time
                                Case "ui_color_state"
                                    Objet.value = devlist.devices.smoke_co_alarms.Values(i).ui_color_state
                                Case "all"
                                    Objet.value = "locale = " & devlist.devices.smoke_co_alarms.Values(i).locale & vbCrLf & _
                                    "name = " & devlist.devices.smoke_co_alarms.Values(i).name & vbCrLf & _
                                    "name_long = " & devlist.devices.smoke_co_alarms.Values(i).name_long & vbCrLf & _
                                    "last_connection = " & devlist.devices.smoke_co_alarms.Values(i).last_connection & vbCrLf & _
                                    "battery_health = " & devlist.devices.smoke_co_alarms.Values(i).battery_health & vbCrLf & _
                                    "co_alarm_state = " & devlist.devices.smoke_co_alarms.Values(i).co_alarm_state & vbCrLf & _
                                    "smoke_alarm_state = " & devlist.devices.smoke_co_alarms.Values(i).smoke_alarm_state & vbCrLf & _
                                    "last_manual_test_time = " & devlist.devices.smoke_co_alarms.Values(i).last_manual_test_time & vbCrLf & _
                                    "is_online = " & CStr(devlist.devices.smoke_co_alarms.Values(i).is_online) & vbCrLf & _
                                    "is_manual_test_active = " & CStr(devlist.devices.smoke_co_alarms.Values(i).is_manual_test_active) & vbCrLf & _
                                    "ui_color_state = " & devlist.devices.smoke_co_alarms.Values(i).ui_color_state

                            End Select
                        End If

                        If TypeOf (Objet.value) Is Boolean Then
                            Select Case Objet.modele

                                Case "is_online"
                                    Objet.value = devlist.devices.smoke_co_alarms.Values(i).is_online
                                Case "is_manual_test_active"
                                    Objet.value = devlist.devices.smoke_co_alarms.Values(i).is_manual_test_active
                                Case "battery_health"
                                    If devlist.devices.smoke_co_alarms.Values(i).battery_health = "Ok" Then
                                        Objet.value = True
                                    Else
                                        Objet.value = False
                                    End If
                                Case "co_alarm_state"
                                    If devlist.devices.smoke_co_alarms.Values(i).co_alarm_state = "Ok" Then
                                        Objet.value = True
                                    Else
                                        Objet.value = False
                                    End If
                                Case "smoke_alarm_state"
                                    If devlist.devices.smoke_co_alarms.Values(i).smoke_alarm_state = "Ok" Then
                                        Objet.value = True
                                    Else
                                        Objet.value = False
                                    End If
                            End Select
                        End If

                    End If
                Next
            End If

            If devlist.devices.thermostats IsNot Nothing Then
                For i = 0 To devlist.devices.thermostats.Count - 1
                    If Objet.adresse1 = "Thermostat " & devlist.devices.thermostats.Values(i).name Then

                        If TypeOf (Objet.value) Is String Then
                            Select Case Objet.modele

                                Case "locale"
                                    Objet.value = devlist.devices.thermostats.Values(i).locale
                                Case "temperature_scale"
                                    Objet.value = devlist.devices.thermostats.Values(i).temperature_scale
                                Case "name"
                                    Objet.value = devlist.devices.thermostats.Values(i).name
                                Case "hvac_mode"
                                    Objet.value = devlist.devices.thermostats.Values(i).hvac_mode
                                Case "name_long"
                                    Objet.value = devlist.devices.thermostats.Values(i).name_long
                                Case "last_connection"
                                    Objet.value = devlist.devices.thermostats.Values(i).last_connection
                                Case "all"

                            End Select
                        End If

                        If TypeOf (Objet.value) Is Boolean Then
                            Select Case Objet.modele

                                Case "is_using_emergency_heat"
                                    Objet.value = devlist.devices.thermostats.Values(i).is_using_emergency_heat
                                Case "has_fan"
                                    Objet.value = devlist.devices.thermostats.Values(i).has_fan
                                Case "has_leaf"
                                    Objet.value = devlist.devices.thermostats.Values(i).has_leaf
                                Case "can_heat"
                                    Objet.value = devlist.devices.thermostats.Values(i).can_heat
                                Case "can_cool"
                                    Objet.value = devlist.devices.thermostats.Values(i).can_cool
                                Case "fan_timer_active"
                                    Objet.value = devlist.devices.thermostats.Values(i).fan_timer_active
                                Case "is_online"
                                    Objet.value = devlist.devices.thermostats.Values(i).is_online

                            End Select
                        End If

                        If TypeOf (Objet.value) Is Double Or TypeOf (Objet.value) Is Integer Then
                            Select Case Objet.modele

                                Case "humidity"
                                    Objet.value = devlist.devices.thermostats.Values(i).humidity
                                Case "target_temperature_c"
                                    Objet.value = devlist.devices.thermostats.Values(i).target_temperature_c
                                Case "target_temperature_f"
                                    Objet.value = devlist.devices.thermostats.Values(i).target_temperature_f
                                Case "target_temperature_high_c"
                                    Objet.value = devlist.devices.thermostats.Values(i).target_temperature_high_c
                                Case "target_temperature_high_f"
                                    Objet.value = devlist.devices.thermostats.Values(i).target_temperature_high_f
                                Case "target_temperature_low_c"
                                    Objet.value = devlist.devices.thermostats.Values(i).target_temperature_low_c
                                Case "target_temperature_low_f"
                                    Objet.value = devlist.devices.thermostats.Values(i).target_temperature_low_f
                                Case "ambient_temperature_c"
                                    Objet.value = devlist.devices.thermostats.Values(i).ambient_temperature_c
                                Case "ambient_temperature_f"
                                    Objet.value = devlist.devices.thermostats.Values(i).ambient_temperature_f
                                Case "away_temperature_high_c"
                                    Objet.value = devlist.devices.thermostats.Values(i).away_temperature_high_c
                                Case "away_temperature_high_f"
                                    Objet.value = devlist.devices.thermostats.Values(i).away_temperature_high_f
                                Case "away_temperature_low_c"
                                    Objet.value = devlist.devices.thermostats.Values(i).away_temperature_low_c
                                Case "away_temperature_low_f"
                                    Objet.value = devlist.devices.thermostats.Values(i).away_temperature_low_f
                            End Select
                        End If

                    End If
                Next
            End If

            WriteLog("DBG: Valeur enregistrée : " & Objet.Name & " -> " & Objet.value)
        Catch ex As Exception
            WriteLog("ERR: GetData, Exception : " & ex.Message)
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
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Nest WriteLog", ex.Message)
        End Try
    End Sub

#End Region

End Class
