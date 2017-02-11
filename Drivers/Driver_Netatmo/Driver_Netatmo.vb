Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device

Imports System.Text.RegularExpressions
Imports STRGS = Microsoft.VisualBasic.Strings
Imports HoMIDom.HoMIDom
Imports HoMIOAuth2

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
    Dim _Description As String = "Données station météo Netatmo"
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
    Dim devlist1 As DeviceList2
    Dim devlist As DeviceList
    Dim meteolist As DeviceList
    Dim thermlist As DeviceList

    Dim thermostat As List(Of String) = New List(Of String)
    Dim meteo As List(Of String) = New List(Of String)

    'param avancé
    Dim _DEBUG As Boolean = False

    Dim cptBeforeToken As Integer = 0
    Dim cpt As Integer = 0
    Dim cpt_restart As Integer
    Dim First As Boolean = True

    Public Class DeviceList2
        Public status As String
        Public body As body2
        Public time_exec As Double
    End Class

    Public Class body2
        Public devices As List(Of devices2)
        Public modules As List(Of modules2)
    End Class

    Public Class modules2
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
        Public dashboard_data As dashboarddata2
    End Class

    Public Class devices2
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
        Public dashboard_data As dashboarddata2
    End Class

    Public Class dashboarddata2
        Public AbsolutePressure As Double
        Public CO2 As Integer
        Public Humidity As Double
        Public Noise As Integer
        Public Pressure As Double
        Public Rain As Double
        Public sum_rain_24 As Double
        Public sum_rain_1 As Double
        Public Temperature As Double
        Public date_max_temp As Integer
        Public date_min_temp As Integer
        Public max_temp As Double
        Public min_temp As Double
        Public WindAngle As Double
        Public WindStrength As Double
        Public GustAngle As Double
        Public GustStrength As Double
        Public WindHistoric As List(Of WindHistoric)
        Public time_utc As Integer
    End Class

    Public Class DeviceList
        Public status As String
        Public body As body
        Public time_exec As Double
        Public time_server As Long
    End Class

    Public Class datasmodule
        Public body As body
        Public time_exec As String
        Public time_server As String
    End Class

    Public Class body
        Public devices As List(Of Devices)
        Public user As user
    End Class

    Public Class modules
        Public _id As String
        Public battery_rint As Integer
        Public battery_vp As Integer
        Public date_setup As netatmodate
        Public firmware As Integer
        Public last_alarm_stored As Integer
        Public last_event_stored As Integer
        Public last_message As Long
        Public last_seen As Long
        Public main_device As String
        Public module_name As String
        Public rf_status As Integer
        Public type As String
        Public therm_orientation As Integer
        Public therm_relay_cmd As Integer
        Public setpoint_history As List(Of setpoint_history)
        Public last_therm_seen As Long
        Public setpoint As setpoint
        Public therm_program_list As List(Of therm_program_list)
        Public measured As measured
        Public dashboard_data As dashboarddata
        Public data_type As List(Of String)
    End Class

    Public Class devices
        Public _id As String
        Public co2_calibrating As Boolean
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
        Public last_status_store As Long
        Public last_upgrade As Integer
        Public module_name As String
        Public modules As List(Of modules)
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
        Public data_type As List(Of String)
        Public udp_conn As Boolean
        Public last_plug_seen As Long
    End Class

    Public Class dashboarddata
        Public AbsolutePressure As Double
        Public CO2 As Integer
        Public Humidity As Double
        Public Noise As Integer
        Public Pressure As Double
        Public Rain As Double
        Public sum_rain_24 As Double
        Public sum_rain_1 As Double
        Public Temperature As Double
        Public date_max_temp As Long
        Public date_min_temp As Long
        Public max_temp As Double
        Public min_temp As Double
        Public WindAngle As Integer
        Public WindStrength As Integer
        Public GustAngle As Integer
        Public GustStrength As Integer
        Public WindHistoric As List(Of WindHistoric)
        Public date_max_wind_str As Long
        Public max_wind_angle As Integer
        Public max_wind_str As Integer
        Public time_utc As Long
    End Class

    Public Class WindHistoric
        Public WindAngle As Integer
        Public WindStrength As Integer
        Public time_utc As Long
    End Class

    Public Class netatmodate
        Public sec As Integer
        Public usec As Integer
    End Class

    Public Class place
        Public altitude As Double
        Public bssid As String
        Public city As String
        Public country As String
        Public improveLocProposed As Boolean
        Public location As List(Of Double)
        Public timezone As String
        Public trust_location As Boolean
    End Class

    Public Class user
        Public mail As String
        Public administrative As administrative
    End Class

    Public Class administrative
        Public reg_locale As String
        Public lang As String
        Public unit As Integer
        Public windunit As Integer
        Public pressureunit As Integer
        Public feel_like_algo As Integer
    End Class

    Public Class setpoint_history
        Public setpoint As setpoint
        Public timestamp As Long
    End Class

    Public Class setpoint
        Public setpoint_temp As Double
        Public setpoint_endtime As Long
        Public setpoint_mode As String
    End Class

    Public Class zones
        Public type As Integer
        Public temp As Integer
        Public id As Integer
    End Class

    Public Class timetable
        Public id As Integer
        Public m_offset As Integer
    End Class

    Public Class therm_program_list
        Public zones As List(Of zones)
        Public timetable As List(Of timetable)
        Public program_id As String
        Public name As String
        Public selected As Boolean
    End Class

    Public Class measured
        Public time As Long
        Public temperature As Double
        Public setpoint_temp As Integer
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
                First = True
            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Erreur dans les paramétres avancés. utilisation des valeur par défaut : " & ex.Message)
            End Try

            If GetRefreshToken("Netatmo", "https://api.netatmo.net/oauth2/token") Then

                ScanData()
                _IsConnect = True
                cpt_restart = 0
                WriteLog("Driver " & Me.Nom & " démarré")

            Else
                cpt_restart += 1
                If cpt_restart < 4 Then
                    GetRefreshToken("Netatmo", "https://api.netatmo.net/oauth2/token")
                    Start()
                Else
                    _IsConnect = False
                    WriteLog("Driver " & Me.Nom & " non démarré")
                    WriteLog("ERR: Verifié que votre authentification est valide avec HoMIAdmiN dans HoMIDoM/Config")
                    cpt_restart = 0
                End If
                Exit Sub
            End If

            If _Refresh > 0 Then
                If _Refresh < 60 Then
                    _Refresh = 60
                End If

                cptBeforeToken = (Auth.expires_in / _Refresh) - 2

                MyTimer.Interval = _Refresh * 1000
                MyTimer.Enabled = True
                AddHandler MyTimer.Elapsed, AddressOf TimerTick
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

            If devlist IsNot Nothing Then GetData(Objet)

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

            'exemple'setthermpoint?access_token=[YOURTOKEN]&device_id=[RELAY_ID]&module_id=[THERM_ID]&setpoint_mode=away
            If Command = "SETPOINT" Then
                Dim client As New Net.WebClient
                Dim responsebody = client.DownloadString("https://api.netatmo.net/api/setthermpoint?access_token=" & Auth.access_token & "&device_id=" & Objet.adresse1 & "&module_id=" & Objet.adresse2 & "&setpoint_mode" & Parametre1)
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
            _DeviceSupport.Add(ListeDevices.TEMPERATURECONSIGNE)
            _DeviceSupport.Add(ListeDevices.GENERIQUEVALUE)

            Add_DeviceCommande("SETPOINT", "Set un programme de thermostat", 1)

            'Parametres avancés
            Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)

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
        If cpt >= cptBeforeToken Then
            GetRefreshToken("Netatmo", "https://api.netatmo.net/oauth2/token")
            cpt = 0
        End If
        If Not IsConnect And Not First And Me.StartAuto Then
            First = True
            Start()
        Else
            ScanData()
        End If
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
                GetRefreshToken("Netatmo", "https://api.netatmo.net/oauth2/token")
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
            If GetAccessToken("Netatmo") Then
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


            If First Then
                Try
line1:
                    First = False
                    responsebody = client.DownloadString("https://api.netatmo.net/api/devicelist?access_token=" & Auth.access_token)
                Catch ex As Exception
                    ' recherche du device/module a interroger
                    cpt_restart += 1
                    If cpt_restart < 4 Then
                        GetRefreshToken("Netatmo", "https://api.netatmo.net/oauth2/token")
                        GoTo line1
                    Else
                        WriteLog("ERR: Verifié que votre authentification est valide avec HoMIAdmiN dans HoMIDoM/Config")
                        WriteLog("ERR: ScanData, Exception : " & ex.Message)
                        cpt_restart = 0
                        Exit Sub
                    End If
                End Try

                meteo = New List(Of String)
                thermostat = New List(Of String)

                devlist1 = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebody, GetType(DeviceList2))
                Dim IdLib As String = ""
                For Each Device In devlist1.body.devices

                    WriteLog("DBG: Device : " & Device.module_name & " | type -> " & Device.type & ", ID = " & Device._id)
                    IdLib += Device.module_name

                    If Device.type = "NAMain" Then
                        meteo.Add(Device._id)
                    End If
                    If Device.type = "NAPlug" Then
                        thermostat.Add(Device._id)
                    End If

                    WriteLog("DBG: Nbre module : " & Device.modules.Count)
                    For Each _module In devlist1.body.modules
                        WriteLog("DBG: Module : " & _module.module_name & " | type -> " & _module.type & ", ID = " & _module._id)
                        IdLib += "|"
                        IdLib += _module.module_name
                    Next
                Next
                Add_LibelleDevice("ADRESSE1", "Nom du module", "Nom du module en respectant maj./minuscule", IdLib)
            End If

            devlist = Nothing

            'getthermostatsdata?access_token=[YOUR_ACCESS_TOKEN]&device_id=[RELAY_ID]
            'setthermpoint?access_token=[YOURTOKEN]&device_id=[RELAY_ID]&module_id=[THERM_ID]&setpoint_mode=away
            For Each _thermostat In thermostat
                responsebody = client.DownloadString("https://api.netatmo.net/api/getthermostatsdata?access_token=" & Auth.access_token & "&device_id=" & _thermostat)
                thermlist = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebody, GetType(DeviceList))
                If devlist Is Nothing Then
                    devlist = thermlist
                Else
                    devlist.body.devices.AddRange(thermlist.body.devices)
                End If

                WriteLog("DBG: ScanData : " & responsebody.ToString)
            Next

            'getstationsdata?access_token=[YOUR_ACCESS_TOKEN]&device_id=[DEVICE_ID]
            For Each _meteo In meteo
                responsebody = client.DownloadString("https://api.netatmo.net/api/getstationsdata?access_token=" & Auth.access_token & "&device_id=" & _meteo)
                meteolist = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebody, GetType(DeviceList))
                If devlist Is Nothing Then
                    devlist = meteolist
                Else
                    devlist.body.devices.AddRange(meteolist.body.devices)
                End If
                WriteLog("DBG: ScanData : " & responsebody.ToString)
            Next

            cpt += 1
            cpt_restart = 0
        Catch ex As Exception
            WriteLog("ERR: ScanData, Exception : " & ex.Message)
            Me.Stop()
        End Try
    End Sub

    Private Sub GetData(ByVal objet As Object)

        Try
            'Si internet n'est pas disponible on ne mets pas à jour les informations
            If My.Computer.Network.IsAvailable = False And Not IsConnect Then
                Exit Sub
            End If

            Dim Typealire As String = ""
            Dim deviceIDalire As devices = Nothing
            Dim moduleIDalire As modules = Nothing


            For Each _Dev In devlist.body.devices
                If (objet.adresse1 = _Dev.module_name) Then
                    deviceIDalire = _Dev
                    Typealire = _Dev.type
                End If
                For Each _Mod In _Dev.modules
                    If objet.adresse1 = _Mod.module_name Then
                        moduleIDalire = _Mod
                        Typealire = _Mod.type
                        Exit For
                    End If
                Next
            Next

            ' nom de device non trouve
            If (deviceIDalire Is Nothing) And (moduleIDalire Is Nothing) Then
                WriteLog("ERR: GetData=> Pas de nom de device/module pour adresse1= " & objet.adresse1)
                Exit Sub
            End If

            'releve de la batterie device/module
            Select Case objet.Type
                Case "METEO"
                    If Typealire = "NAMain" Then
                        objet.TemperatureActuel = Regex.Replace(CStr(deviceIDalire.dashboard_data.Temperature), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                        objet.HumiditeActuel = Regex.Replace(CStr(deviceIDalire.dashboard_data.Humidity), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                        objet.MinToday = Regex.Replace(CStr(deviceIDalire.dashboard_data.min_temp), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                        objet.MaxToday = Regex.Replace(CStr(deviceIDalire.dashboard_data.max_temp), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                        Exit Sub
                    Else
                        objet.TemperatureActuel = Regex.Replace(CStr(moduleIDalire.dashboard_data.Temperature), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                        objet.HumiditeActuel = Regex.Replace(CStr(moduleIDalire.dashboard_data.Humidity), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                        objet.MinToday = Regex.Replace(CStr(moduleIDalire.dashboard_data.min_temp), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                        objet.MaxToday = Regex.Replace(CStr(moduleIDalire.dashboard_data.max_temp), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                        If Typealire = " NAModule2" Then
                            objet.VentActuel = Regex.Replace(CStr(moduleIDalire.dashboard_data.WindStrength), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                        End If
                        Exit Sub
                    End If

                Case "BATTERIE"
                    Select Case Typealire
                        Case "NAMain", "NAPlug"
                            objet.Value = deviceIDalire.battery_vp
                        Case "NAModule4"
                            objet.value = Format(((moduleIDalire.battery_vp - 4200) * 100) / 1800, "#0")
                        Case "NAModule1", "NAModule3"
                            objet.value = Format(((moduleIDalire.battery_vp - 3600) * 100) / 2400, "#0")
                        Case "NAModule2"
                            objet.value = Format(((moduleIDalire.battery_vp - 3950) * 100) / 2050, "#0")
                        Case "NATherm1"
                            objet.value = Format(((moduleIDalire.battery_vp - 3000) * 100) / 1500, "#0")
                    End Select
                Case "TEMPERATURE"
                    Select Case Typealire
                        Case "NAMain"
                            objet.Value = Regex.Replace(CStr(deviceIDalire.dashboard_data.Temperature), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                        Case "NATherm1"
                            objet.Value = Regex.Replace(CStr(moduleIDalire.measured.temperature), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                        Case Else
                            objet.Value = Regex.Replace(CStr(moduleIDalire.dashboard_data.Temperature), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    End Select
                Case "HUMIDITE"
                    If Typealire = "NAMain" Then
                        objet.Value = Regex.Replace(CStr(deviceIDalire.dashboard_data.Humidity), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    Else
                        objet.Value = Regex.Replace(CStr(moduleIDalire.dashboard_data.Humidity), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    End If
                Case "PLUIETOTAL"
                    If Typealire = "NAMain" Then
                        objet.Value = Regex.Replace(CStr(deviceIDalire.dashboard_data.sum_rain_24), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    Else
                        objet.Value = Regex.Replace(CStr(moduleIDalire.dashboard_data.sum_rain_24), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    End If
                Case "PLUIECOURANT"
                    If Typealire = "NAMain" Then
                        objet.Value = Regex.Replace(CStr(deviceIDalire.dashboard_data.sum_rain_1), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    Else
                        objet.Value = Regex.Replace(CStr(moduleIDalire.dashboard_data.sum_rain_1), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    End If
                Case "BAROMETRE"
                    If Typealire = "NAMain" Then
                        objet.Value = deviceIDalire.dashboard_data.Pressure
                    Else
                        objet.Value = moduleIDalire.dashboard_data.Pressure
                    End If
                Case "VITESSEVENT"
                    If Typealire = "NAMain" Then
                        objet.Value = Regex.Replace(CStr(deviceIDalire.dashboard_data.WindStrength), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    Else
                        objet.Value = Regex.Replace(CStr(moduleIDalire.dashboard_data.WindStrength), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    End If
                Case "DIRECTIONVENT"
                    If Typealire = "NAMain" Then
                        objet.Value = DirVent(deviceIDalire.dashboard_data.WindAngle)
                    Else
                        objet.Value = DirVent(moduleIDalire.dashboard_data.WindAngle)
                    End If
                Case "GENERIQUEVALUE"
                    Select Case Typealire
                        Case "NAMain"
                            Select Case objet.adresse2.toUpper
                                Case "CO2"
                                    objet.Value = deviceIDalire.dashboard_data.CO2
                                Case "NOISE"
                                    objet.Value = deviceIDalire.dashboard_data.Noise
                            End Select
                        Case Else
                            Select Case objet.adresse2.toUpper
                                Case "CO2"
                                    objet.Value = moduleIDalire.dashboard_data.CO2
                                Case "NOISE"
                                    objet.Value = moduleIDalire.dashboard_data.Noise
                            End Select
                    End Select
                Case "GENERIQUESTRING"
                    Select Case Typealire
                        Case "NATherm1"
                            objet.Value = moduleIDalire.setpoint.setpoint_mode & " - " & moduleIDalire.setpoint.setpoint_temp & " - " & moduleIDalire.setpoint.setpoint_endtime
                    End Select
                Case "TEMPERATURECONSIGNE"
                    Select Case Typealire
                        Case "NATherm1"
                            objet.Value = moduleIDalire.measured.setpoint_temp
                    End Select
                Case Else
                    WriteLog("ERR: GetData=> Pas de valeur enregistrée")
                    Exit Sub
            End Select
            WriteLog("DBG: Valeur enregistrée : " & objet.Type & " -> " & objet.value)

        Catch ex As Exception
            WriteLog("ERR: GetData=> Read, Exception : " & ex.Message)
        End Try
    End Sub

    Private Function DirVent(ByVal direction As Integer) As String
        Try
            If direction > 348.75 Or direction < 11.26 Then
                Return "N"
            ElseIf direction < 33.76 Then
                Return "NNE"
            ElseIf direction < 56.26 Then
                Return "NE"
            ElseIf direction < 78.76 Then
                Return "ENE"
            ElseIf direction < 101.26 Then
                Return "E"
            ElseIf direction < 123.76 Then
                Return "ESE"
            ElseIf direction < 146.26 Then
                Return "SE"
            ElseIf direction < 168.76 Then
                Return "SSE"
            ElseIf direction < 191.26 Then
                Return "S"
            ElseIf direction < 213.76 Then
                Return "SSW"
            ElseIf direction < 236.26 Then
                Return "SW"
            ElseIf direction < 258.76 Then
                Return "WSW"
            ElseIf direction < 281.26 Then
                Return "W"
            ElseIf direction < 303.76 Then
                Return "WNW"
            ElseIf direction < 326.26 Then
                Return "NW"
            Else
                Return "NNW"
            End If
        Catch ex As Exception
            WriteLog("ERR: wrdirection : " & ex.Message)
            Return "ERR: " & ex.Message
        End Try
    End Function

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
