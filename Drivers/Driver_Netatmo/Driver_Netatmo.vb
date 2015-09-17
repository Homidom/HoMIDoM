Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device

Imports System.Text.RegularExpressions
Imports STRGS = Microsoft.VisualBasic.Strings
Imports HoMIDom.HoMIDom

' Auteur : jphomi sur une base HoMIDoM meteoweather
' Date : 01/03/2015

''' <summary>Driver Netatmo Meteo Reception de datas base plus module température/pluviometre</summary>
''' <remarks></remarks>
<Serializable()> Public Class Driver_NetAtmo
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "71BAB1C8-B072-11E4-A32C-93901D5D46B0"
    Dim _Nom As String = "Netatmo"
    Dim _Enable As Boolean = False
    Dim _Description As String = "Données Netatmo"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "WEB"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "@"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "Netatmo"
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
    Dim devlist As DeviceList

    'param avancé
    Dim _DEBUG As Boolean = False
    Dim _ClientID As String = "abcdefghi0123456789"
    Dim _ClientSecret As String = "abcdefghi0123456789"
    Dim _Username As String = "homidom@homidom.com"
    Dim _Password As String = "homi123456"
    Dim _OauthSecret As Boolean = True

    Dim cptBeforeToken As Integer = 0
    Dim cpt As Integer = 0
    Dim cpt_restart As Integer

    Public Class DeviceList
        Public status As String
        Public body As body
        Public time_exec As Double
    End Class

    Public Class datasmodule
        Public body As body
        Public time_exec As String
        Public time_server As String
    End Class

    Public Class body
        Public devices As List(Of devices)
        Public modules As List(Of modules)
    End Class

    Public Class modules
        Public _id As String
        Public battery_rint As Integer
        Public battery_vp As Integer
        Public date_setup As netatmodate
        Public firmware As Integer
        Public last_alarm_stored As Integer
        Public last_event_stored As Integer
        Public last_message As Integer
        Public last_seen As Integer
        Public main_device As String
        Public module_name As String
        Public rf_status As Integer
        Public type As String
        Public dashboard_data As dashboarddata
    End Class

    Public Class devices
        Public _id As String
        Public access_code As String
        Public battery_rint As Integer
        Public battery_vp As Integer
        Public date_creation As netatmodate
        Public firmware As Integer
        Public invitation_disable As Boolean
        Public ip As String
        Public last_alarm_stored As Integer
        Public last_data_store As Object
        Public last_event_stored As Integer
        Public last_status_store As Integer
        Public module_name As String
        Public modules As List(Of Object)
        Public netcom_transport As String
        Public place As place
        Public public_ext_data As Boolean
        Public rf_amb_status As Integer
        Public station_name As String
        Public type As String
        Public update_device As Boolean
        Public user_owner As String
        Public wifi_status As Integer
        Public streaming_key As String
        Public dashboard_data As dashboarddata
    End Class

    Public Class dashboarddata
        Public Temperature As Double
        Public Humidity As Double
        Public min_temp As Double
        Public max_temp As Double
        Public CO2 As Integer
        Public Rain As Double
        Public sum_rain_24 As Double
        Public sum_rain_1 As Double
        Public AbsolutePressure As Double
        Public Noise As Integer
        Public Pressure As Double
        Public time_utc As Integer
        Public date_max_temp As Integer
        Public date_min_temp As Integer
    End Class

    Class netatmodate
        Public sec As Integer
        Public usec As Integer
    End Class

    Class place
        Public altitude As Integer
        Public bssid As String
        Public city As String
        Public country As String
        Public location As List(Of Double)
        Public timezone As String
        Public trust_location As Boolean
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
                        If String.IsNullOrEmpty(Value) Or IsNumeric(Value) Then
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
                _ClientID = _Parametres.Item(1).Valeur
                _ClientSecret = _Parametres.Item(2).Valeur
                _Username = _Parametres.Item(3).Valeur
                _Password = _Parametres.Item(4).Valeur
                _OauthSecret = _Parametres.Item(5).Valeur

            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Erreur dans les paramétres avancés. utilisation des valeur par défaut : " & ex.Message)
            End Try

            If Not _OauthSecret Then

                GetAccessTokenOld()

            Else

                If GetAccessToken("Netatmo") Then

                    cptBeforeToken = (Auth.expires_in / _Refresh) - 2


                    If _Refresh > 0 Then
                        If _Refresh < 60 Then
                            _Refresh = 60
                        End If
                        MyTimer.Interval = _Refresh * 1000
                        MyTimer.Enabled = True
                        AddHandler MyTimer.Elapsed, AddressOf TimerTick
                    End If


                Else
                    cpt_restart += 1
                    If cpt_restart < 4 Then
                        GetRefreshToken("Netatmo", "https://api.netatmo.net/oauth2/token")
                        Start()
                    Else
                        _IsConnect = False
                        WriteLog("Driver " & Me.Nom & " non démarré")
                        WriteLog("ERR: Verifié que votre authentification est valide avec HoMIAdmiN dans HoMIDoM/Config")
                    End If
                    Exit Sub
                End If
            End If

            If _Refresh > 0 Then
                If _Refresh < 60 Then
                    _Refresh = 60
                End If
                MyTimer.Interval = _Refresh * 1000
                MyTimer.Enabled = True
                AddHandler MyTimer.Elapsed, AddressOf TimerTick
            End If
            _IsConnect = True
            cpt_restart = 0
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom, "Driver " & Me.Nom & " démarré")

        Catch ex As Exception
            _IsConnect = False
            WriteLog("ERR: Driver " & Me.Nom & " Erreur démarrage " & ex.Message)
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
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
                WriteLog("ERR: READ, Le driver n'est pas démarré, impossible d'écrire sur le port")
                Exit Sub
            End If

            GetData(Objet)

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
            _DeviceSupport.Add(ListeDevices.DIRECTIONVENT)
            _DeviceSupport.Add(ListeDevices.HUMIDITE)
            _DeviceSupport.Add(ListeDevices.UV)
            _DeviceSupport.Add(ListeDevices.VITESSEVENT)
            _DeviceSupport.Add(ListeDevices.TEMPERATURE)
            _DeviceSupport.Add(ListeDevices.BATTERIE)
            _DeviceSupport.Add(ListeDevices.PLUIECOURANT)
            _DeviceSupport.Add(ListeDevices.HUMIDITE)
            _DeviceSupport.Add(ListeDevices.PLUIETOTAL)
            _DeviceSupport.Add(ListeDevices.GENERIQUESTRING)

            'Parametres avancés
            Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)
            Add_ParamAvance("ClientID", "ID client (Optionnel)", "abcdefghi0123456789")
            Add_ParamAvance("ClientSecret", "Secret du client (Optionnel)", "abcdefghi0123456789")
            Add_ParamAvance("Username", "Nom utilisateur(Optionnel)", "homidom@homidom.com")
            Add_ParamAvance("Password", "Mot de passe (Optionnel)", "homi123456")
            Add_ParamAvance("TokenUse", "Utilisation du token", True)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Nom module", "Nom du module en respectant maj./minuscule", "")
            Add_LibelleDevice("ADRESSE2", "Nom valeur", "Obligatoire pour Noise, CO2, ", "")
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
        If cpt >= cptBeforeToken And _OauthSecret Then
            GetRefreshToken("Netatmo", "https://api.netatmo.net/oauth2/token")
            cpt = 0
        End If

        ScanData()

    End Sub

#End Region

#Region "Fonctions internes"

    Private Sub GetAccessTokenOld()

        Try

            Try ' lecture de la variable debug, permet de rafraichir la variable debug sans redemarrer le service
                _DEBUG = _Parametres.Item(0).Valeur
            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Erreur de lecture de debug : " & ex.Message)
            End Try

            Dim client As New Net.WebClient
            Dim reqparm As New Specialized.NameValueCollection

            reqparm.Add("grant_type", "password")
            reqparm.Add("client_id", _Parametres.Item(1).valeur)
            reqparm.Add("client_secret", _Parametres.Item(2).valeur)
            reqparm.Add("username", _Parametres.Item(3).valeur)
            reqparm.Add("password", _Parametres.Item(4).valeur)
            reqparm.Add("scope", "read_station")
            Dim responsebytes = client.UploadValues("https://api.netatmo.net/oauth2/token", "POST", reqparm)
            Dim responsebody = (New System.Text.UTF8Encoding).GetString(responsebytes)
            WriteLog("DBG: responsebody : " & responsebody.ToString)
            Auth = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebody, GetType(Authentication))

            'va chercher les module que si connecté
            If Auth.expires_in > 0 Then
                WriteLog("Requête https://api.netatmo.net/oauth2/token  OK")
                WriteLog("DBG: Connect : " & responsebody.ToString)
            Else
                WriteLog("ERR: Connect, non connecté")
            End If
        Catch ex As Exception
            WriteLog("ERR: GetListDevice, Exception : " & ex.Message)
        End Try
    End Sub

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
            reqparm.Add("client_id", _Server.GetClientFile(clientOauth).web.client_id)
            reqparm.Add("client_secret", _Server.GetClientFile(clientOauth).web.client_secret)
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
            Dim responsebody = client.DownloadString("https://api.netatmo.net/api/devicelist?access_token=" & Auth.access_token)
            devlist = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebody, GetType(DeviceList))

            WriteLog("DBG: ScanData : " & responsebody.ToString)

            Dim i As Integer
            WriteLog("DBG: Device : " & devlist.body.devices.Item(0).module_name & " | type -> " & devlist.body.devices.Item(0).type & ", ID = " & devlist.body.devices.Item(0)._id)
            WriteLog("DBG: Nbre module : " & devlist.body.modules.Count)
            For i = 0 To devlist.body.modules.Count - 1
                WriteLog("DBG: Module : " & devlist.body.modules.Item(i).module_name & " | type -> " & devlist.body.modules.Item(i).type & ", ID = " & devlist.body.modules.Item(i)._id)
                If i > 0 Then IdLib += "|"
                IdLib += devlist.body.devices.Item(0).module_name
            Next
            If _OauthSecret Then cpt += 1
            If _OauthSecret Then cpt_restart = 0
        Catch ex As Exception
            WriteLog("ERR: ScanData, Exception : " & ex.Message)
            ' recherche du device/module a interroger
            If _OauthSecret Then
                cpt_restart += 1
                If cpt_restart < 4 Then
                    GetRefreshToken("Netatmo", "https://api.netatmo.net/oauth2/token")
                    GoTo line1
                Else
                    WriteLog("ERR: Verifié que votre authentification est valide avec HoMIAdmiN dans HoMIDoM/Config")
                    Restart()
                End If
            End If
        End Try
    End Sub

    Private Sub GetData(ByVal objet As Object)
        Try
            'Si internet n'est pas disponible on ne mets pas à jour les informations
            If My.Computer.Network.IsAvailable = False Then
                Exit Sub
            End If

            Dim deviceIDalire, moduleIDalire As String
            Dim nummodulealire As Integer
            deviceIDalire = ""
            moduleIDalire = ""
            nummodulealire = 99
            If (objet.adresse1 = devlist.body.devices.Item(0).module_name) Then
                deviceIDalire = devlist.body.devices.Item(0)._id
            End If
            For i = 0 To devlist.body.modules.Count - 1
                If objet.adresse1 = devlist.body.modules.Item(i).module_name Then
                    moduleIDalire = devlist.body.modules.Item(i)._id
                    nummodulealire = i
                    Exit For
                End If
            Next

            ' nom de device non trouve
            If (deviceIDalire = "") And (moduleIDalire = "") Then
                WriteLog("ERR: Pas de nom de device/module pour adresse1= " & objet.adresse1)
                Exit Sub
            End If

            'releve de la batterie device/module
            If (objet.Type = "BATTERIE") And (moduleIDalire = "") Then
                objet.Value = devlist.body.devices.Item(0).battery_vp
                Exit Sub
            End If
            If (objet.Type = "BATTERIE") And (moduleIDalire <> "") Then
                ' ne fais pas la différence entre module int et ext
                objet.value = Format(((devlist.body.modules.Item(nummodulealire).battery_vp - 3600) * 100) / 2400, "#0")
                Exit Sub
            End If

            Select Case objet.Type
                Case "METEO"
                    If nummodulealire = 99 Then
                        objet.TemperatureActuel = Regex.Replace(CStr(devlist.body.devices.Item(0).dashboard_data.Temperature), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                        objet.HumiditeActuel = Regex.Replace(CStr(devlist.body.devices.Item(0).dashboard_data.Humidity), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                        objet.MinToday = Regex.Replace(CStr(devlist.body.devices.Item(0).dashboard_data.min_temp), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                        objet.MaxToday = Regex.Replace(CStr(devlist.body.devices.Item(0).dashboard_data.max_temp), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                        Exit Sub
                    Else
                        objet.TemperatureActuel = Regex.Replace(CStr(devlist.body.modules.Item(nummodulealire).dashboard_data.Temperature), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                        objet.HumiditeActuel = Regex.Replace(CStr(devlist.body.modules.Item(nummodulealire).dashboard_data.Humidity), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                        objet.MinToday = Regex.Replace(CStr(devlist.body.modules.Item(nummodulealire).dashboard_data.min_temp), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                        objet.MaxToday = Regex.Replace(CStr(devlist.body.modules.Item(nummodulealire).dashboard_data.max_temp), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                        Exit Sub
                    End If
                Case "TEMPERATURE"
                    If nummodulealire = 99 Then
                        objet.Value = Regex.Replace(CStr(devlist.body.devices.Item(0).dashboard_data.Temperature), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    Else
                        objet.Value = Regex.Replace(CStr(devlist.body.modules.Item(nummodulealire).dashboard_data.Temperature), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    End If
                Case "HUMIDITE"
                    If nummodulealire = 99 Then
                        objet.Value = Regex.Replace(CStr(devlist.body.devices.Item(0).dashboard_data.Humidity), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    Else
                        objet.Value = Regex.Replace(CStr(devlist.body.modules.Item(nummodulealire).dashboard_data.Humidity), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    End If
                Case "BRUIT"
                    If nummodulealire = 99 Then
                        objet.Value = devlist.body.devices.Item(0).dashboard_data.Noise
                    Else
                        objet.Value = devlist.body.modules.Item(nummodulealire).dashboard_data.Noise
                    End If
                Case "CO2"
                    If nummodulealire = 99 Then
                        objet.Value = devlist.body.devices.Item(0).dashboard_data.CO2
                    Else
                        objet.Value = devlist.body.modules.Item(nummodulealire).dashboard_data.CO2
                    End If
                Case "PLUIETOTAL"
                    If nummodulealire = 99 Then
                        objet.Value = Regex.Replace(CStr(devlist.body.devices.Item(0).dashboard_data.sum_rain_24), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    Else
                        objet.Value = Regex.Replace(CStr(devlist.body.modules.Item(nummodulealire).dashboard_data.sum_rain_24), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    End If
                Case "PLUIECOURANT"
                    If nummodulealire = 99 Then
                        objet.Value = Regex.Replace(CStr(devlist.body.devices.Item(0).dashboard_data.sum_rain_1), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    Else
                        objet.Value = Regex.Replace(CStr(devlist.body.modules.Item(nummodulealire).dashboard_data.sum_rain_1), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    End If
                Case "BAROMETRE"
                    If nummodulealire = 99 Then
                        objet.Value = devlist.body.devices.Item(0).dashboard_data.Pressure
                    Else
                        objet.Value = devlist.body.modules.Item(nummodulealire).dashboard_data.Pressure
                    End If
                Case "VITESSEVENT"
                    If nummodulealire = 99 Then
                        objet.Value = devlist.body.devices.Item(0).dashboard_data.Pressure
                    Else
                        objet.Value = devlist.body.modules.Item(nummodulealire).dashboard_data.Pressure
                    End If
                Case "GENERIQUESTRING"
                    If nummodulealire = 99 Then
                        Select Case objet.adresse2.toUpper
                            Case "CO2"
                                objet.Value = devlist.body.devices.Item(0).dashboard_data.CO2
                            Case "NOISE"
                                objet.Value = devlist.body.devices.Item(0).dashboard_data.Noise
                        End Select
                    Else
                        Select Case objet.adresse2.toUpper
                            Case "CO2"
                                objet.Value = devlist.body.modules.Item(nummodulealire).dashboard_data.CO2
                            Case "NOISE"
                                objet.Value = devlist.body.modules.Item(nummodulealire).dashboard_data.Noise
                        End Select
                    End If
                Case Else
                    WriteLog("ERR: Pas de valeur enregistrée")
                    Exit Sub
            End Select
            WriteLog("Valeur enregistrée : " & objet.Type & " -> " & objet.value)
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
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " WriteLog", ex.Message)
        End Try
    End Sub

#End Region

End Class
