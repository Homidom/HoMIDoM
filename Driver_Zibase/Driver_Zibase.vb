'Option Strict On
Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports STRGS = Microsoft.VisualBasic.Strings
Imports VB = Microsoft.VisualBasic
Imports System.IO.Ports
Imports System.Math
Imports System.Threading
Imports System.Globalization
Imports System.ComponentModel
Imports ZibaseDll

' Auteur : David
' Date : 18/04/2012

''' <summary>Class Driver_Zibase, permet de communiquer avec la Zibase Ethernet</summary>
''' <remarks>Nécessite la dll déceloppé par Planete Domotique zibasedll.dll</remarks>
<Serializable()> Public Class Driver_Zibase
    Implements HoMIDom.HoMIDom.IDriver
    Implements ISynchronizeInvoke

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "0A94F22A-E824-11E0-B989-175F4824019B"
    Dim _Nom As String = "ZIBASE"
    Dim _Enable As Boolean = False
    Dim _Description As String = "Zibase Ethernet"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "ETHERNET"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "@"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "Zibase Ethernet/Wifi V1/2"
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

    'param avancé
    Dim _DEBUG As Boolean = False
    Dim _AUTODISCOVER As Boolean = True
#End Region

#Region "Variables Internes"
    Private WithEvents zba As New ZibaseDll.ZiBase

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
        Try
            If MyDevice IsNot Nothing Then
                'Pas de commande demandée donc erreur
                If Command = "" Then
                    Return False
                Else
                    'Write(deviceobject, Command, Param(0), Param(1))
                    Select Case UCase(Command)
                        Case "RUN_SCENARIO"
                            If Not IsNothing(Param(0)) Then
                                RunScenario(CStr(Param(0)))
                            Else
                                WriteLog("ERR: ExecuteCommand : RUN_SCENARIO : il manque un parametre")
                            End If
                        Case "RUN_SCRIPT"
                            If Not IsNothing(Param(0)) Then
                                ExecScript(CStr(Param(0)))
                            Else
                                WriteLog("ERR: ExecuteCommand : RUN_SCRIPT : il manque un parametre")
                            End If
                        Case Else : WriteLog("ERR: ExecuteCommand : command incorrecte : " & Command)
                    End Select
                    Return True
                End If
            Else
                Return False
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ExecuteCommand", "exception : " & ex.Message)
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


            End Select
            Return retour
        Catch ex As Exception
            Return "Une erreur est apparue lors de la vérification du champ " & Champ & ": " & ex.ToString
        End Try
    End Function

    ''' <summary>Démarrer le du driver</summary>
    ''' <remarks></remarks>
    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        Try
            'récupération des paramétres avancés
            Try
                _DEBUG = _Parametres.Item(0).Valeur
                _AUTODISCOVER = _Parametres.Item(1).Valeur
            Catch ex As Exception
                WriteLog("ERR: Erreur dans les paramétres avancés. utilisation des valeur par défaut" & ex.Message)
            End Try

            Try
                zba.StartZB()
                _IsConnect = True
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Start", "Driver " & Me.Nom & " démarré")
            Catch ex As Exception
                WriteLog("ERR: Start Exception " & ex.Message)
            End Try
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Zibase Start", ex.Message)
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            zba.StopZB()
            _IsConnect = False
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Stop", "Driver " & Me.Nom & " arrêté")
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Zibase Stop", ex.Message)
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
            Dim sei As ZiBase.SensorInfo
            Dim retour As String
            If _Enable = False Then Exit Sub
            If _DEBUG Then WriteLog("DBG: WRITE Read " & Objet.Name)

            sei = zba.GetSensorInfo(Objet.adresse1, Objet.Modele)
            If STRGS.UCase(sei.sType) = "TEM" Then
                retour = CStr(sei.dwValue / 100) 'si c'est une temperature on / par 100
            Else
                retour = CStr(sei.dwValue)
            End If
            WriteRetour(Objet.adresse, Objet.type.ToString, retour) 'Modification du device

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Zibase Read", ex.Message)
        End Try
    End Sub

    ''' <summary>Commander un device</summary>
    ''' <param name="Objet">Objet représetant le device à interroger</param>
    ''' <param name="Command">La commande à passer</param>
    ''' <param name="Parametre1"></param>
    ''' <param name="Parametre2"></param>
    ''' <remarks></remarks>
    Public Sub Write(ByVal Objet As Object, ByVal Command As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
        Try
            Dim retour As String = ""
            If _Enable = False Then Exit Sub
            If _DEBUG Then WriteLog("DBG: WRITE Device " & Objet.Name & " <-- " & Command)

            If IsNothing(Parametre1) Or Str(Parametre1) = "" Then retour = Ecrirecommand(Objet.adresse1, Objet.modele, Objet.adresse2, Command, 0) Else retour = Ecrirecommand(Objet.adresse1, Objet.modele, Objet.adresse2, Command, Parametre1)
            If STRGS.InStr(retour, "ERR:") > 0 Then
                WriteLog(retour)
            Else
                WriteRetour(Objet.adresse, Objet.type.ToString, retour) 'Modification du device
            End If


        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Zibase Write", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Zibase DeleteDevice", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Zibase NewDevice", ex.Message)
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
            x.DescriptionCommand = description
            x.CountParam = nbparam
            _DeviceCommandPlus.Add(x)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " add_devicecommande", "Exception : " & ex.Message)
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
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " add_devicecommande", "Exception : " & ex.Message)
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
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " add_devicecommande", "Exception : " & ex.Message)
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
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " add_devicecommande", "Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Creation d'un objet de type</summary>
    ''' <remarks></remarks>
    Public Sub New()
        Try
            _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString

            'Parametres avancés
            Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)
            Add_ParamAvance("AutoDiscover", "Permet de créer automatiquement des composants si ceux-ci n'existent pas encore (True/False)", False)

            'liste des devices compatibles
            _DeviceSupport.Add(ListeDevices.APPAREIL.ToString)
            _DeviceSupport.Add(ListeDevices.BAROMETRE.ToString)
            _DeviceSupport.Add(ListeDevices.BATTERIE.ToString)
            _DeviceSupport.Add(ListeDevices.COMPTEUR.ToString)
            _DeviceSupport.Add(ListeDevices.CONTACT.ToString)
            _DeviceSupport.Add(ListeDevices.DETECTEUR.ToString)
            _DeviceSupport.Add(ListeDevices.DIRECTIONVENT.ToString)
            _DeviceSupport.Add(ListeDevices.ENERGIEINSTANTANEE.ToString)
            _DeviceSupport.Add(ListeDevices.ENERGIETOTALE.ToString)
            _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN.ToString)
            _DeviceSupport.Add(ListeDevices.GENERIQUESTRING.ToString)
            _DeviceSupport.Add(ListeDevices.GENERIQUEVALUE.ToString)
            _DeviceSupport.Add(ListeDevices.HUMIDITE.ToString)
            _DeviceSupport.Add(ListeDevices.LAMPE.ToString)
            _DeviceSupport.Add(ListeDevices.PLUIECOURANT.ToString)
            _DeviceSupport.Add(ListeDevices.PLUIETOTAL.ToString)
            _DeviceSupport.Add(ListeDevices.SWITCH.ToString)
            _DeviceSupport.Add(ListeDevices.TELECOMMANDE.ToString)
            _DeviceSupport.Add(ListeDevices.TEMPERATURE.ToString)
            _DeviceSupport.Add(ListeDevices.TEMPERATURECONSIGNE.ToString)
            _DeviceSupport.Add(ListeDevices.UV.ToString)
            _DeviceSupport.Add(ListeDevices.VITESSEVENT.ToString)
            _DeviceSupport.Add(ListeDevices.VOLET.ToString)

            'ajout des commandes avancées pour les devices
            'add_devicecommande("PRESETDIM", "permet de paramétrer le DIM : param1=niveau, param2=timer", 2)
            Add_DeviceCommande("RUN_SCRIPT", "Execute le script passé en parametre", 1)
            Add_DeviceCommande("RUN_SCENARIO", "Execute le scenario passé en parametre", 1)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Adresse", "Adresse du composant au format XX0000000")
            Add_LibelleDevice("ADRESSE2", "Adresse Emission", "Adresse d'emission pour certains composants")
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("MODELE", "Protocole", "Nom du protocole à utiliser : aucun / BROADC / CHACON / DOMIA / RFS10 / VIS433 / VIS868 / X10 / XDD433 / XDD868 / XDD868_INTER_SHUTTER / XDD868_BOILER_AC / XDD868_PILOT_WIRE / ZWAVE", "aucun|BROADC|CHACON|DOMIA|RFS10|VIS433|VIS868|X10|XDD868|XDD868_INTER_SHUTTER|XDD868_BOILER_AC|XDD868_PILOT_WIRE|ZWAVE")
            Add_LibelleDevice("REFRESH", "@", "")
            'Add_LibelleDevice("LASTCHANGEDUREE", "LastChange Durée", "")
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Zibase New", ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)

    End Sub

#End Region

#Region "Fonctions internes"

    Public Function BeginInvoke(ByVal method As System.Delegate, ByVal args() As Object) As System.IAsyncResult Implements System.ComponentModel.ISynchronizeInvoke.BeginInvoke
        BeginInvoke = Nothing
    End Function
    Public Function EndInvoke(ByVal result As System.IAsyncResult) As Object Implements System.ComponentModel.ISynchronizeInvoke.EndInvoke
        EndInvoke = Nothing
    End Function
    Public Function Invoke(ByVal method As System.Delegate, ByVal args() As Object) As Object Implements System.ComponentModel.ISynchronizeInvoke.Invoke
        Invoke = Nothing
    End Function
    Public ReadOnly Property InvokeRequired() As Boolean Implements System.ComponentModel.ISynchronizeInvoke.InvokeRequired
        Get
            Return False
        End Get
    End Property

    'reception d'une valeur -> analyse
    Private Sub zba_UpdateSensorInfo(ByVal seInfo As ZibaseDll.ZiBase.SensorInfo) Handles zba.UpdateSensorInfo
        'WriteLog("DBG: " & seInfo.sID & "_" & seInfo.sType & " ----> " & seInfo.sValue)
        traitement(seInfo.sID, seInfo.sType, seInfo.dwValue, seInfo.sValue)
    End Sub
    Private Sub zba_NewSensorDetected(ByVal seInfo As ZibaseDll.ZiBase.SensorInfo) Handles zba.NewSensorDetected
        'si on detecte une nouveau device
        'WriteLog("DBG: " & seInfo.sID & "_" & seInfo.sType & " ----> " & seInfo.sValue)
        traitement(seInfo.sID, seInfo.sType, seInfo.dwValue, seInfo.sValue)
    End Sub

    'nouvelle zibase detecté -> Log
    Private Sub zba_newzibasedetected(ByVal ZiInfo As ZibaseDll.ZiBase.ZibaseInfo) Handles zba.NewZibaseDetected
        WriteLog("Nouvelle Zibase détecté : " & ZiInfo.sLabelBase & "-" & ZiInfo.lIpAddress)
    End Sub

    'la zibase a qqch à logger
    Private Sub ZibaseLog(ByVal sMsg As String, ByVal level As Integer) Handles zba.WriteMessage
        If _DEBUG Then WriteLog("DBG: " & sMsg & " - " & level)
    End Sub

    'executer un script stocké sur la zibase
    Private Function ExecScript(ByVal sScript As String) As String
        'sScript : nom du script sur la zibase
        Try
            zba.ExecScript(sScript)
            Return "Script Executé : " & sScript
        Catch ex As Exception
            Return "ERR: ExecScript: " & ex.Message
        End Try
    End Function

    'executer un scénario stocké sur la zibase
    Private Function RunScenario(ByVal sCmd As String) As String
        'sCmd : nom du scenario sur la zibase
        Try
            zba.RunScenario(sCmd)
            Return " Scenario Executé : " & sCmd
        Catch ex As Exception
            Return "ERR: RunScenario: " & ex.Message
        End Try
    End Function

    'ecrire device
    Private Function Ecrirecommand(ByVal composants_adresse As String, ByVal composants_modele_nom As String, ByVal composants_adresse2 As String, ByVal ordre As String, ByVal iDim As Integer) As String
        'composants_adresse : adresse du composant
        'composants_modele_nom : modele du composant
        'composants_adresse2 : adresse secondaire du composant chacon
        'ordre : ordre à envoyer
        'iDim: nombre de 0 à 100 pour l'ordre DIM sur chacon

        Dim protocole As ZiBase.Protocol
        Dim adresse As String
        Dim valeur As String = ""

        Try
            adresse = composants_adresse2 'adresse = Split(composants_adresse, "_")(0)
            If adresse = "" Then adresse = composants_adresse

            Select Case UCase(composants_modele_nom)
                Case "" : protocole = ZiBase.Protocol.PROTOCOL_BROADCAST
                Case "aucun" : protocole = ZiBase.Protocol.PROTOCOL_BROADCAST
                Case "BROADC" : protocole = ZiBase.Protocol.PROTOCOL_BROADCAST
                Case "CHACON"
                    protocole = ZiBase.Protocol.PROTOCOL_CHACON
                    'adresse = composants_adresse2 'on a 2 adresses pour chacon : reception et emission dans le champ divers
                Case "DOMIA" : protocole = ZiBase.Protocol.PROTOCOL_DOMIA
                Case "VIS433" : protocole = ZiBase.Protocol.PROTOCOL_VISONIC433
                Case "VIS868" : protocole = ZiBase.Protocol.PROTOCOL_VISONIC868
                Case "X10" : protocole = ZiBase.Protocol.PROTOCOL_X10
                Case "ZWAVE" : protocole = ZiBase.Protocol.PROTOCOL_ZWAVE
                Case "RFS10" : protocole = ZiBase.Protocol.PROTOCOL_RFS10
                Case "XDD433" : protocole = ZiBase.Protocol.PROTOCOL_X2D433
                Case "XDD868" : protocole = ZiBase.Protocol.PROTOCOL_X2D868
                Case "XDD868_INTER_SHUTTER" : protocole = ZiBase.Protocol.PROTOCOL_X2D868_INTER_SHUTTER
                Case "XDD868_BOILER_AC" : protocole = ZiBase.Protocol.PROTOCOL_XDD868_BOILER_AC
                Case "XDD868_PILOT_WIRE" : protocole = ZiBase.Protocol.PROTOCOL_XDD868_PILOT_WIRE
                Case Else : Return ("ERR: protocole incorrect : " & Modele(0))
            End Select

            'verification Adresse
            If adresse = "" Then Return ("ERR: pas d'adresse renseignée")

            'ecriture sur la zibase
            Select Case UCase(ordre)
                Case "ON"
                    zba.SendCommand(adresse, ZiBase.State.STATE_ON, 0, protocole, 1)
                    valeur = CStr(100)
                Case "OFF"
                    zba.SendCommand(adresse, ZiBase.State.STATE_OFF, 0, protocole, 1)
                    valeur = CStr(0)
                Case "DIM"
                    If UCase(Modele(0)) <> "CHACON" Then
                        zba.SendCommand(adresse, ZiBase.State.STATE_DIM, 0, protocole, 1)
                        valeur = CStr(100)
                    Else
                        zba.SendCommand(adresse, ZiBase.State.STATE_DIM, iDim, protocole, 1)
                        valeur = CStr(iDim)
                    End If
                Case Else : Return ("ERR: ordre incorrect : " & ordre)
            End Select

            'retour normal : on renvoie la valeur
            Return valeur

        Catch ex As Exception
            Return "ERR: Zib_ecrirecommand" & ex.Message & " --> adresse:" & composants_adresse & " (" & composants_adresse2 & ") commande:" & ordre & "-" & iDim
        End Try

    End Function
#End Region

#Region "Write"

    Private Sub traitement(ByVal adresse As String, ByVal type As String, ByVal valeurentiere As Long, ByVal valeurstring As String)
        Try
            Dim valeur As String = CStr(valeurentiere)
            If [String].IsNullOrEmpty(valeurstring) Then valeurstring = " "
            'modification des informations suivant le type
            Select Case UCase(type)
                Case "TEM"
                    'valeur = STRGS.Left(valeur, (valeur.Length - 2))
                    valeur = CStr(CInt(valeur) / 100)
                    type = "THE" 'tem Température (°C)
                    'Case "hum"
                    'valeur = STRGS.Left(valeur, (valeur.Length - 1))
                Case "TEMC"
                    'valeur = STRGS.Left(valeur, (valeur.Length - 2))
                    valeur = CStr(CInt(valeur) / 100)
                    type = "THC" 'Température de consigne (Thermostat : °C)
                Case "XSE", "BAT", "LNK", "STA" : valeur = valeurstring 'on utilise la valeur normale et non l'entier
            End Select
            'dans le cas des adresse du tpe M5
            If adresse.Length = 2 Then valeur = valeurstring 'on utilise la valeur normale et non l'entier

            'Action suivant le type
            Select Case LCase(type)
                Case "bat" : If STRGS.UCase(valeur) = "LOW" Then WriteBattery(adresse) 'Niveau de batterie (Ok / Low)
                Case "lev" : If _DEBUG Then WriteLog("DBG: Signal Level : " & valeur & " (Adresse:" & adresse & ")") 'on log le level si debug : Niveau de réception RF (1 à 5)
                Case "lnk" : WriteLog("DBG: Etat de la connexion avec la Zibase " & adresse & " : " & valeur) 'Etat de la connexion Zibase
                Case "" : WriteRetour(adresse, "", valeur) ' si pas de type particulier
                Case "the" : WriteRetour(adresse, ListeDevices.TEMPERATURE.ToString, valeur) 'Température (°C)
                Case "thc" : WriteRetour(adresse, ListeDevices.TEMPERATURECONSIGNE.ToString, valeur) 'Température de consigne (Thermostat : °C)
                Case "hum" : WriteRetour(adresse, ListeDevices.HUMIDITE.ToString, valeur) 'Humidité (%)
                Case "uvl" : WriteRetour(adresse, ListeDevices.UV.ToString, valeur) 'Niveau d’UV
                Case "tra" : WriteRetour(adresse, ListeDevices.PLUIETOTAL.ToString, valeur) 'Niveau de pluie total (Total Rain)
                Case "cra" : WriteRetour(adresse, ListeDevices.PLUIECOURANT.ToString, valeur) 'Niveau de pluie courant (Currant Rain)
                Case "Kw" : WriteRetour(adresse, ListeDevices.ENERGIEINSTANTANEE.ToString, valeur) 'Mesure d’énergie instantanée (CM119)
                Case "kwh" : WriteRetour(adresse, ListeDevices.ENERGIETOTALE.ToString, valeur) 'Mesure d’énergie totale (CM119)
                Case "awi" : WriteRetour(adresse, ListeDevices.VITESSEVENT.ToString, valeur) ' Mesure de la vitesse du vent
                Case "drt" : WriteRetour(adresse, ListeDevices.DIRECTIONVENT.ToString, valeur) 'Direction du vent
                Case "xse" : WriteRetour(adresse, "", valeur) 'detecteurs fumées/co... (Alert, Normal)
                Case "Lnk" : WriteRetour(adresse, "", valeur) 'Etat de la connexion Zibase
                Case "sta" : WriteRetour(adresse, ListeDevices.SWITCH.ToString, valeur) 'Status pour un switch (ON/OFF)
                Case Else : WriteRetour(adresse & "_" & STRGS.UCase(type), "", valeur)
            End Select
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Zibase traitement", ex.Message)
        End Try
    End Sub

    Private Sub WriteLog(ByVal message As String)
        Try
            'utilise la fonction de base pour loguer un event
            If STRGS.InStr(message, "DBG:") > 0 Then
                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "Zibase", STRGS.Right(message, message.Length - 5))
            ElseIf STRGS.InStr(message, "ERR:") > 0 Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Zibase", STRGS.Right(message, message.Length - 5))
            Else
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Zibase", message)
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Zibase WriteLog", ex.Message)
        End Try
    End Sub

    Private Sub WriteBattery(ByVal adresse As String)
        Try
            'Forcer le . 
            'Thread.CurrentThread.CurrentCulture = New CultureInfo("en-US")
            'My.Application.ChangeCulture("en-US")

            'log tous les paquets en mode debug
            If _DEBUG Then WriteLog("DBG: WriteBattery : receive from " & adresse)

            If Not _IsConnect Then Exit Sub 'si on ferme le port on quitte

            'Recherche si un device affecté
            Dim listedevices As New ArrayList
            listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, adresse, "", Me._ID, True)
            If (listedevices.Count >= 1) Then
                'on a trouvé un ou plusieurs composants avec cette adresse, on prend le premier
                WriteLog("ERR: " & listedevices.Item(0).Name & " (" & adresse & ") : Battery Empty")
            Else
                'device pas trouvé
                WriteLog("ERR: Device non trouvé : " & adresse & ": Battery Empty")

                'Ajouter la gestion des composants bannis (si dans la liste des composant bannis alors on log en debug sinon onlog device non trouve empty)

            End If
        Catch ex As Exception
            WriteLog("ERR: WriteBattery Exception : " & ex.Message & " --> " & adresse)
        End Try
    End Sub

    Private Sub WriteRetour(ByVal adresse As String, ByVal type As String, ByVal valeur As String)
        Try
            'Forcer le . 
            'Thread.CurrentThread.CurrentCulture = New CultureInfo("en-US")
            'My.Application.ChangeCulture("en-US")

            'log tous les paquets en mode debug
            If _DEBUG Then WriteLog("DBG: WriteRetour : receive from " & adresse & " (" & type & ") -> " & valeur)

            If Not _IsConnect Then Exit Sub 'si on ferme le port on quitte

            'Recherche si un device affecté
            Dim listedevices As New ArrayList
            listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, adresse, type, Me._ID, True)
            If (listedevices.Count = 1) Then
                'un device trouvé 
                If STRGS.InStr(valeur, "CFG:") > 0 Then
                    'c'est un message de config, on log juste
                    WriteLog(listedevices.Item(0).name & " : " & valeur)
                Else
                    'on maj la value si la durée entre les deux receptions est > à 1.5s
                    If (DateTime.Now - Date.Parse(listedevices.Item(0).LastChange)).TotalMilliseconds > 1500 Then
                        If valeur = "ON" Then
                            If TypeOf listedevices.Item(0).Value Is Boolean Then
                                listedevices.Item(0).Value = True
                            ElseIf TypeOf listedevices.Item(0).Value Is Long Then
                                listedevices.Item(0).Value = 100
                            Else
                                listedevices.Item(0).Value = "ON"
                            End If
                        ElseIf valeur = "OFF" Then
                            If TypeOf listedevices.Item(0).Value Is Boolean Then
                                listedevices.Item(0).Value = False
                            ElseIf TypeOf listedevices.Item(0).Value Is Long Then
                                listedevices.Item(0).Value = 0
                            Else
                                listedevices.Item(0).Value = "OFF"
                            End If
                        Else
                            listedevices.Item(0).Value = valeur
                        End If
                    Else
                        WriteLog("DBG: Reception < 1.5s de deux valeurs pour le meme composant : " & listedevices.Item(0).name & ":" & valeur)
                    End If
                End If
            ElseIf (listedevices.Count > 1) Then
                WriteLog("ERR: Plusieurs devices correspondent à : " & type & " " & adresse & ":" & valeur)
            Else

                'si autodiscover = true alors on crée le composant sinon on logue
                If _AUTODISCOVER Then
                    If type = "" Then
                        WriteLog("ERR: Device non trouvé, AutoCreation impossible du composant car le type ne peut etre déterminé : " & adresse & ":" & valeur)
                    Else
                        Try
                            WriteLog("Device non trouvé, AutoCreation du composant : " & type & " " & adresse & ":" & valeur)
                            _Server.SaveDevice(_IdSrv, "", "_RFXtrx_" & Date.Now.ToString("ddMMyyHHmmssf"), adresse, True, False, Me._ID, type, 0, "", "", "", "AutoDiscover RFXtrx", 0, False, "0", "", 0, 999999, -999999, 0, Nothing, "", 0, False)
                        Catch ex As Exception
                            WriteLog("ERR: Writeretour Exception : AutoDiscover Creation composant: " & ex.Message)
                        End Try
                    End If
                Else
                    WriteLog("ERR: Device non trouvé : " & type & " " & adresse & ":" & valeur)
                End If

                'Ajouter la gestion des composants bannis (si dans la liste des composant bannis alors on log en debug sinon onlog device non trouve empty)

            End If
        Catch ex As Exception
            WriteLog("ERR: Writeretour Exception : " & ex.Message)
        End Try
    End Sub

#End Region
End Class
