'Option Strict On
Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports STRGS = Microsoft.VisualBasic.Strings
Imports VB = Microsoft.VisualBasic
Imports System.IO.Ports

' Auteur : David
' Date : 10/02/2011

''' <summary>Class Driver_PLCBUS, permet de commander et recevoir des ordres avec les périphériques PLCBUS via un 1141 ou 1141+</summary>
''' <remarks>Pour la version USB, necessite l'installation du driver</remarks>
<Serializable()> Public Class Driver_PLCBUS
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "5AEDF3A8-3568-11E0-9DEC-D164DFD72085"
    Dim _Nom As String = "PLCBUS"
    Dim _Enable As Boolean = False
    Dim _Description As String = "PLCBUS COM / USB (PLC1141)"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "COM"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "COM1"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "USB/COM 1141/1141+"
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
    Public _IdSrv As String
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande)
    Dim _AutoDiscover As Boolean = False

    'Ajoutés dans les ppt avancés dans New()
    Dim plcack As Boolean = True 'gestion des acks ?
    Dim plctriphase As Boolean = False 'installation en triphase
    Dim plcusercode As Integer = 209 'usercode PLCBUS defaut : &HD1 / 209
    Dim _DEBUG As Boolean = False
    'Dim plcmodele As String = "1141" '"1141" ou "1141+" si modele 1141+, utilise le checksum plutot que H3 en fin de paquet
#End Region

#Region "Variables internes"

    Private WithEvents port As New System.IO.Ports.SerialPort
    Private ackreceived As Boolean = False
    Private port_name As String = ""
    Private Shared plcbuslock As New Object

    Dim com_to_hex As New Dictionary(Of String, Integer)
    Dim hex_to_com As New Dictionary(Of Integer, String)

    Private BufferIn(8192) As Byte
    Private firstbyte As Boolean = True
    Private bytecnt As Integer = 0
    Private recbuf(30) As Byte

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
            If (value <> "1141" And value <> "1141+") Then value = "1141" 'pour etre sur d'avoir un modéle correct
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
                    Write(MyDevice, Command, Param(0), Param(1))
                    'Select Case UCase(Command)
                    '    Case ""
                    '    Case Else
                    'End Select
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
        Dim retour As String = ""
        'récupération des paramétres avancés
        Try
            plcack = _Parametres.Item(0).Valeur
            plctriphase = _Parametres.Item(1).Valeur
            plcusercode = _Parametres.Item(2).Valeur
            _DEBUG = _Parametres.Item(3).Valeur
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Start", "Erreur dans les paramétres avancés. utilisation des valeur par défaut" & ex.Message)
        End Try

        'ouverture du port suivant le Port Com ou IP
        Try
            If _Com <> "" Then
                retour = ouvrir(_Com)
            Else
                retour = "ERR: Port Com non défini. Impossible d'ouvrir le port !"
            End If
            'traitement du message de retour
            If STRGS.Left(retour, 4) = "ERR:" Then
                _IsConnect = False
                retour = STRGS.Right(retour, retour.Length - 5)
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS", "Driver non démarré : " & retour)
            Else
                _IsConnect = True
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "PLCBUS", retour)
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Start", ex.Message)
            _IsConnect = False
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Dim retour As String
        Try
            retour = fermer()
            If STRGS.Left(retour, 4) = "ERR:" Then
                retour = STRGS.Right(retour, retour.Length - 5)
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS", retour)
            Else
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "PLCBUS", retour)
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Stop", ex.Message)
        End Try
    End Sub

    ''' <summary>Re-Démarrer le du driver</summary>
    ''' <remarks></remarks>
    Public Sub Restart() Implements HoMIDom.HoMIDom.IDriver.Restart
        Try
            [Stop]()
            Start()
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS ReStart", ex.Message)
        End Try
    End Sub

    ''' <summary>Intérroger un device</summary>
    ''' <param name="Objet">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub Read(ByVal Objet As Object) Implements HoMIDom.HoMIDom.IDriver.Read
        Try
            If _Enable = False Then Exit Sub
            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "PLCBUS Read", "Lecture de " & Objet.Name)
            If (Objet.adresse1.ToString.Length > 1) Then
                'c'est une adresse std on fait un status request
                ecrire(Objet.adresse1, "STATUS_REQUEST")
            ElseIf (Objet.adresse1.ToString.Length = 1) Then
                'fastpooling
                If plctriphase Then
                    ecrire(Objet.adresse1, "ReportOnlyOnIdPulse3Phase")
                Else
                    ecrire(Objet.adresse1, "GetOnlyOnIdPulse")
                End If
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Read", ex.Message)
        End Try
    End Sub

    ''' <summary>Commander un device</summary>
    ''' <param name="Objet">Objet représetant le device à interroger</param>
    ''' <param name="Commande">La commande à passer</param>
    ''' <param name="Parametre1"></param>
    ''' <param name="Parametre2"></param>
    ''' <remarks></remarks>
    Public Sub Write(ByVal Objet As Object, ByVal Commande As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
        'Parametre1 = data1
        'Parametre2 = data2
        Dim sendtwice As Boolean = False
        Try
            If _Enable = False Then Exit Sub
            If _IsConnect = False Then
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "PLCBUS Write", "Le driver n'est pas démarré, impossible d'écrire sur le port")
                Exit Sub
            End If
            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "PLCBUS Write", "Ecriture de " & Objet.Name)
            If Parametre1 Is Nothing Then Parametre1 = 0
            If Parametre2 Is Nothing Then Parametre2 = 0
            If Objet.type = "APPAREIL" Or Objet.type = "LAMPE" Then sendtwice = True
            ecrire(Objet.adresse1, Commande, Parametre1, Parametre2, sendtwice)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Write", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS DeleteDevice", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS NewDevice", ex.Message)
        End Try
    End Sub

    ''' <summary>ajout des commandes avancées pour les devices</summary>
    ''' <remarks></remarks>
    Private Sub add_devicecommande(ByVal nom As String, ByVal description As String, ByVal nbparam As Integer)
        Try
            Dim x As New DeviceCommande
            x.NameCommand = nom
            x.DescriptionCommand = description
            x.CountParam = nbparam
            _DeviceCommandPlus.Add(x)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS add_devicecommande", "Exception : " & ex.Message)
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
    Private Sub add_paramavance(ByVal nom As String, ByVal description As String, ByVal valeur As Object)
        Try
            Dim x As New HoMIDom.HoMIDom.Driver.Parametre
            x.Nom = nom
            x.Description = description
            x.Valeur = valeur
            _Parametres.Add(x)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS add_devicecommande", "Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Creation d'un objet de type</summary>
    ''' <remarks></remarks>
    Public Sub New()
        Try
            _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString

            'Liste des devices compatibles
            _DeviceSupport.Add(ListeDevices.SWITCH.ToString)
            _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN.ToString)
            _DeviceSupport.Add(ListeDevices.CONTACT.ToString)
            _DeviceSupport.Add(ListeDevices.APPAREIL.ToString)
            _DeviceSupport.Add(ListeDevices.LAMPE.ToString)
            _DeviceSupport.Add(ListeDevices.VOLET.ToString)

            'Paramétres avancés
            add_paramavance("ack", "Gestion du ack", True)
            add_paramavance("triphase", "Installation en triphase", False)
            add_paramavance("plcusercode", "User Code (0-255)", 209)
            add_paramavance("Debug", "Activer le Debug complet (True/False)", False)

            'ajout des commandes avancées pour les devices
            add_devicecommande("ALL_UNITS_OFF", "Eteint tous les appareils du meme range que ce device", 0)
            add_devicecommande("ALL_LIGHTS_ON", "Allume toutes les lampes du meme range que ce device", 0)
            add_devicecommande("BRIGHT", "light will bright until fade-stop is received /  parametre = Fade rate", 1)
            add_devicecommande("ALL_LIGHTS_OFF", "Eteint toutes les lampes du meme range que ce device", 0)
            add_devicecommande("All_USER_LIGHTS_ON", "Allume toutes les lampes", 0)
            add_devicecommande("All_USER_UNITS_OFF", "Eteint tous les appareils", 0)
            add_devicecommande("All_USER_LIGHTS_OFF", "Eteint toutes les lampes", 0)
            add_devicecommande("BLINK", "Flashe à intervalle régulier, parametre = intervalle", 1)
            add_devicecommande("FADE_STOP", "Arrete de varier DIM/BRIGHT", 0)
            add_devicecommande("PRESET_DIM", " Allume et enregistre à tel level et en rate secondes la lampe, param1=dim level, param2=rate", 2)
            add_devicecommande("STATUS_ON", "", 0)
            add_devicecommande("STATUS_OFF", "", 0)
            'add_devicecommande("STATUS_REQUEST", "", 0)
            add_devicecommande("ReceiverMasterAddressSetup", "param1=New user code, param2=new home+unitcode", 2)
            add_devicecommande("TransmitterMasterAddressSetup", "param1=New user code, param2=new home+unitcode", 2)
            add_devicecommande("SceneAddressSetup", "", 0)
            add_devicecommande("SceneAddressErase", "", 0)
            add_devicecommande("AllSceneAddressErase", "", 0)
            add_devicecommande("GetSignalStrength", "param = signal strength", 1)
            add_devicecommande("GetNoiseStrength", "param = Noise strength", 1)
            add_devicecommande("ReportSignalStrength", "", 0)
            add_devicecommande("ReportNoiseStrength", "", 0)
            'add_devicecommande("GetAllIdPulse", "", 0)
            'add_devicecommande("GetOnlyOnIdPulse", "", 0)
            'add_devicecommande("ReportAllIdPulse3Phase", "", 0)
            'add_devicecommande("ReportOnlyOnIdPulse3Phase", "", 0)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Adresse", "Adresse du composant de type L1 ou L")
            Add_LibelleDevice("ADRESSE2", "@", "")
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("MODELE", "@", "")
            Add_LibelleDevice("REFRESH", "Fastpooling/STATUS_REQUEST (Secondes)", "Permet de faire un STATUS_REQUEST (ex: L2) ou du fastpooling (ex: 'L')")
            'Add_LibelleDevice("LASTCHANGEDUREE", "LastChange Durée", "")

            'dictionnaire Commande STRING -> INT
            com_to_hex.Add("ALL_UNITS_OFF", 0)
            com_to_hex.Add("ALL_LIGHTS_ON", 1)
            com_to_hex.Add("ON", 2) 'data1 must be 100, data2 must be 0
            com_to_hex.Add("OFF", 3) 'data1 must be 0, data2 must be 0
            com_to_hex.Add("DIM", 4) 'light will dim until fade-stop func=11 is received /  data1 = Fade rate
            com_to_hex.Add("BRIGHT", 5) 'light will bright until fade-stop func=11 is received /  data1 = Fade rate
            com_to_hex.Add("ALL_LIGHTS_OFF", 6)
            com_to_hex.Add("All_USER_LIGHTS_ON", 7)
            com_to_hex.Add("All_USER_UNITS_OFF", 8)
            com_to_hex.Add("All_USER_LIGHTS_OFF", 9)
            com_to_hex.Add("BLINK", 10) 'data1=interval
            com_to_hex.Add("FADE_STOP", 11)
            com_to_hex.Add("PRESET_DIM", 12) 'data1=dim level, data2=rate
            com_to_hex.Add("STATUS_ON", 13)
            com_to_hex.Add("STATUS_OFF", 14)
            com_to_hex.Add("STATUS_REQUEST", 15)
            com_to_hex.Add("ReceiverMasterAddressSetup", 16) 'data1=New user code, data2=new home+unitcode
            com_to_hex.Add("TransmitterMasterAddressSetup", 17) 'data1=New user code, data2=new home+unitcode
            com_to_hex.Add("SceneAddressSetup", 18)
            com_to_hex.Add("SceneAddressErase", 19)
            com_to_hex.Add("AllSceneAddressErase", 20)
            com_to_hex.Add("Reserved1", 21)
            com_to_hex.Add("Reserved2", 22)
            com_to_hex.Add("Reserved3", 23)
            com_to_hex.Add("GetSignalStrength", 24) 'data1=signal strength
            com_to_hex.Add("GetNoiseStrength", 25) 'data1=Noise strength
            com_to_hex.Add("ReportSignalStrength", 26)
            com_to_hex.Add("ReportNoiseStrength", 27)
            com_to_hex.Add("GetAllIdPulse", 28)
            com_to_hex.Add("GetOnlyOnIdPulse", 29)
            com_to_hex.Add("ReportAllIdPulse3Phase", 30)
            com_to_hex.Add("ReportOnlyOnIdPulse3Phase", 31)

            'dictionnaire Commande INT -> STRING
            hex_to_com.Add(0, "ALL_UNITS_OFF")
            hex_to_com.Add(1, "ALL_LIGHTS_ON")
            hex_to_com.Add(2, "ON")
            hex_to_com.Add(3, "OFF")
            hex_to_com.Add(4, "DIM")
            hex_to_com.Add(5, "BRIGHT")
            hex_to_com.Add(6, "ALL_LIGHTS_OFF")
            hex_to_com.Add(7, "All_USER_LIGHTS_ON")
            hex_to_com.Add(8, "All_USER_UNITS_OFF")
            hex_to_com.Add(9, "All_USER_LIGHTS_OFF")
            hex_to_com.Add(10, "BLINK")
            hex_to_com.Add(11, "FADE_STOP")
            hex_to_com.Add(12, "PRESET_DIM")
            hex_to_com.Add(13, "STATUS_ON")
            hex_to_com.Add(14, "STATUS_OFF")
            hex_to_com.Add(15, "STATUS_REQUEST")
            hex_to_com.Add(16, "ReceiverMasterAddressSetup")
            hex_to_com.Add(17, "TransmitterMasterAddressSetup")
            hex_to_com.Add(18, "SceneAddressSetup")
            hex_to_com.Add(19, "SceneAddressErase")
            hex_to_com.Add(20, "AllSceneAddressErase")
            hex_to_com.Add(24, "GetSignalStrength")
            hex_to_com.Add(25, "GetNoiseStrength")
            hex_to_com.Add(26, "ReportSignalStrength")
            hex_to_com.Add(27, "ReportNoiseStrength")
            hex_to_com.Add(28, "GetAllIdPulse")
            hex_to_com.Add(29, "GetOnlyOnIdPulse")
            hex_to_com.Add(30, "ReportAllIdPulse3Phase")
            hex_to_com.Add(31, "ReportOnlyOnIdPulse3Phase")

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS New", "Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS TimerTick", ex.Message)
        End Try
    End Sub

#End Region

#Region "Fonctions internes"

    ''' <summary>Ouvrir le port PLCBUS</summary>
    ''' <param name="numero">Nom/Numero du port COM: COM2</param>
    ''' <remarks></remarks>
    Private Function ouvrir(ByVal numero As String) As String
        Try
            'ouverture du port
            If Not _IsConnect Then
                port_name = numero
                port.PortName = numero 'nom du port : COM1
                port.BaudRate = 9600 'vitesse du port 300, 600, 1200, 2400, 9600, 14400, 19200, 38400, 57600, 115200
                port.Parity = IO.Ports.Parity.None 'pas de parité
                port.StopBits = IO.Ports.StopBits.One 'un bit d'arrêt par octet
                port.DataBits = 8 'nombre de bit par octet
                'port.Handshake = Handshake.None
                port.ReadTimeout = 3000
                port.WriteTimeout = 5000
                'port.RtsEnable = False 'ligne Rts désactivé
                'port.DtrEnable = False 'ligne Dtr désactivé
                port.Open()
                AddHandler port.DataReceived, New SerialDataReceivedEventHandler(AddressOf DataReceived)
                Return ("Port " & port_name & " ouvert")
            Else
                Return ("Port " & port_name & " dejà ouvert")
            End If
        Catch ex As Exception
            Return ("ERR: " & ex.Message)
        End Try
    End Function

    ''' <summary>Fermer le port PLCBUS</summary>
    ''' <remarks></remarks>
    Private Function fermer() As String
        Try
            If _IsConnect Then
                If (Not (port Is Nothing)) Then ' The COM port exists.
                    If port.IsOpen Then
                        'vidage des tampons
                        Dim i As Integer = 0
                        port.DiscardOutBuffer()
                        Do While (port.BytesToWrite > 0 And i < 50) ' Wait for the transmit buffer to empty.
                            i = i + 1
                            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "PLCBUS Fermer", "Wait " & port.BytesToWrite & "BytesToWrite " & i)
                            wait(10)
                        Loop
                        i = 0
                        port.DiscardInBuffer()
                        Do While (port.BytesToRead > 0 And i < 20) ' Wait for the receipt buffer to empty.
                            i = i + 1
                            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "PLCBUS Fermer", "Wait " & port.BytesToRead & "BytesToRead " & i)
                            wait(10)
                        Loop
                        port.Close()
                        port.Dispose()
                        _IsConnect = False
                        Return ("Port " & port_name & " fermé")
                    Else
                        Return ("Port " & port_name & "  est déjà fermé")
                    End If
                Else
                    Return ("Port " & port_name & " n'existe pas")
                End If
            Else
                Return ("Port " & port_name & "  est déjà fermé (port_ouvert=false)")
            End If
        Catch ex As UnauthorizedAccessException
            Return ("ERR: Port " & port_name & " IGNORE")
            ' The port may have been removed. Ignore.
        End Try
        Return "ERR: Not defined"
    End Function

    ''' <summary>Converti les adresses de string en hexa</summary>
    ''' <remarks></remarks>
    Private Function adresse_to_hex(ByVal adresse As String) As Integer
        'convertit une adresse du type L1 en byte
        Try
            Dim table() As Integer = {0, 16, 32, 48, 64, 80, 96, 112, 128, 144, 160, 176, 192, 208, 224, 240}
            If adresse.Length > 1 Then
                Return table(Asc(Microsoft.VisualBasic.Left(adresse, 1)) - 65) + CInt(Microsoft.VisualBasic.Right(adresse, adresse.Length - 1)) - 1
            Else
                Return table(Asc(adresse) - 65)
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS adresse_to_hex", ex.Message)
            Return 0
        End Try
    End Function
    'Private Function adresse_to_hex(ByVal adresse As String)
    '    'convertit une adresse du type L1 en byte
    '    Try
    '        Dim table() As String = {0, 16, 32, 48, 64, 80, 96, 112, 128, 144, 160, 176, 192, 208, 224, 240}
    '        If adresse.Length > 1 Then
    '            Return table(Asc(Microsoft.VisualBasic.Left(adresse, 1)) - 65) + CInt(Microsoft.VisualBasic.Right(adresse, adresse.Length - 1)) - 1
    '        Else
    '            Return table(Asc(adresse) - 65)
    '        End If
    '    Catch ex As Exception
    '        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS adresse_to_hex", ex.Message)
    '        Return ""
    '    End Try
    'End Function

    ''' <summary>Converti les adresses d'hexa en string</summary>
    ''' <remarks></remarks>
    Private Function hex_to_adresse(ByVal adresse As Byte) As String
        'convertit une adresse en byte en type L1
        Try
            Dim x As Integer = 0
            Dim y As Integer = 0

            If adresse >= 128 Then
                x = x + 8
                adresse = CByte(adresse - 128)
            End If
            If adresse >= 64 Then
                x = x + 4
                adresse = CByte(adresse - 64)
            End If
            If adresse >= 32 Then
                x = x + 2
                adresse = CByte(adresse - 32)
            End If
            If adresse >= 16 Then
                x = x + 1
                adresse = CByte(adresse - 16)
            End If
            If adresse >= 8 Then
                y = y + 8
                adresse = CByte(adresse - 8)
            End If
            If adresse >= 4 Then
                y = y + 4
                adresse = CByte(adresse - 4)
            End If
            If adresse >= 2 Then
                y = y + 2
                adresse = CByte(adresse - 2)
            End If
            If adresse >= 1 Then
                y = y + 1
                adresse = CByte(adresse - 1)
            End If
            Return Chr(x + 65) & (y + 1)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS hex_to_adresse", "exception : " & ex.Message)
            Return "" 'on renvoi rien car il y a eu une erreur
        End Try
    End Function

    ''' <summary>Pause pour attendre x msecondes </summary>
    ''' <remarks></remarks>
    Private Sub wait(ByVal msec As Integer)
        '100msec = 1 secondes
        Try
            Dim ticks = Date.Now.Ticks + (msec * 100000) '10000000 = 1 secondes
            Dim limite = 0
            While limite = 0
                If ticks <= Date.Now.Ticks Then limite = 1
            End While
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Wait", "Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Ecrire sur le port PLCBUS</summary>
    ''' <param name="adresse">Adresse du device : A1...</param>
    ''' <param name="commande">commande à envoyer : ON, OFF...</param>
    ''' <param name="data1">voir description des actions plus haut ou doc plcbus</param>
    ''' <param name="data2">voir description des actions plus haut ou doc plcbus</param>
    ''' <param name="ecriretwice">Booleen : Ecrire l'ordre deux fois</param>
    ''' <remarks></remarks>
    Private Function ecrire(ByVal adresse As String, ByVal commande As String, Optional ByVal data1 As Integer = 0, Optional ByVal data2 As Integer = 0, Optional ByVal ecriretwice As Boolean = False) As String
        Dim _adresse = 0
        Dim _cmd = 0
        'Dim checksum = &H3
        'Dim tblack() As DataRow
        Try
            If _IsConnect Then
                'Dim usercode = &HD1 'D1
                Try
                    _adresse = adresse_to_hex(adresse)
                Catch ex As Exception
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Ecrire", "Adresse non valide : " & _adresse)
                    Return ""
                End Try
                Try
                    _cmd = com_to_hex(commande)
                Catch ex As Exception
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Ecrire", "Commande non valide : " & commande)
                    Return ""
                End Try

                Try
                    '--- usercode ---
                    'on verifie si bon format : 1-255
                    If Not (IsNumeric(plcusercode) And plcusercode >= 0 And plcusercode <= 255) Then
                        'non correct, on log et prend la valeur par défaut
                        plcusercode = 209
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Ecrire", "usercode non valide (0-255 defaut:209) : " & plcusercode)
                    End If

                    '--- TriPhase ---
                    If plctriphase Then _cmd = _cmd Or &H40
                    '--- request acks --- (sauf pour les status_request car pas important et encombre le port)
                    'If commande <> "STATUS_REQUEST" And commande <> "GetOnlyOnIdPulse" And commande <> "GetAllIdPulse" And commande <> "ReportAllIdPulse3Phase" And commande <> "ReportOnlyOnIdPulse3Phase" Then
                    '    _cmd = _cmd Or &H20
                    'End If
                    ' --- correction data suivant la commande ---
                    If commande = "ON" Then
                        data1 = 100
                        data2 = 0
                    ElseIf commande = "OFF" Then
                        data1 = 0
                        data2 = 0
                    End If

                    'If _Modele = "1141+" Then
                    '    'pour les modeles 1141+
                    '    checksum = &H200 - (&H2 + &H5 + plcusercode + _adresse + _cmd + data1 + data2)
                    'Else
                    '    'pour les modéles 1141
                    '    checksum = &H3
                    'End If

                    Dim donnee() As Byte = {&H2, &H5, CByte(plcusercode), CByte(_adresse), CByte(_cmd), CByte(data1), CByte(data2), &H3}

                    'ecriture sur le port
                    SyncLock plcbuslock 'lock pour etre sur de ne pas faire deux operations en meme temps 
                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "PLCBUS Ecrire", " Ecrire " & adresse & " : " & commande & " " & data1 & "-" & data2 & " (data:" & &H2 & "." & &H5 & "." & plcusercode & "." & _adresse & "." & _cmd & "." & data1 & "." & data2 & "." & &H3 & ")")
                        port.Write(donnee, 0, donnee.Length)
                        'If ecriretwice Then port.Write(donnee, 0, donnee.Length) 'on ecrit deux fois : voir la norme PLCBUS

                        'gestion des acks (sauf pour les status_request car pas important et encombre le port)
                        If plcack And Not attente_ack() And commande <> "STATUS_REQUEST" And commande <> "GetOnlyOnIdPulse" And commande <> "GetAllIdPulse" And commande <> "ReportAllIdPulse3Phase" And commande <> "ReportOnlyOnIdPulse3Phase" Then
                            'pas de ack, on relance l ordre
                            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Ecrire", "Pas de Ack -> resend : " & adresse & " : " & commande & " " & data1 & "-" & data2)
                            port.Write(donnee, 0, donnee.Length)
                            'port.Write(donnee, 0, donnee.Length) 'on ecrit deux fois : voir la norme PLCBUS
                            If Not attente_ack() Then
                                'pas de ack > NOK
                                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Ecrire", "Pas de Ack --> " & adresse & " : " & commande & " " & data1 & "-" & data2)
                                Return "" 'on renvoi rien car le composant n'a pas recu l'ordre
                            End If
                        Else
                            wait(20) 'on attend 0.2s pour liberer le bus correctement
                        End If
                    End SyncLock
                Catch ex As Exception
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Ecrire", "Ecriture sur le port exception : " & ex.Message & " --> " & adresse & " : " & commande & " " & data1 & "-" & data2)
                    Return "" 'on renvoi rien car il y a eu une erreur
                End Try

                'Modification du device
                Try
                    traitement("", adresse, commande, True)
                    Select Case commande
                        Case "ON", "OFF", "BRIGHT", "BLINK", "FADE_STOP", "STATUS_REQUEST"
                            If data1 = 0 Then traitement("OFF", adresse, commande, True) Else traitement("ON", adresse, commande, True)
                        Case "DIM", "PRESET_DIM"
                            traitement(CStr(data1), adresse, commande, True)
                        Case "ALL_UNITS_OFF", "All_USER_UNITS_OFF", "ALL_LIGHTS_OFF", "All_USER_LIGHTS_OFF"
                            traitement("OFF", adresse, commande, True)
                        Case "ALL_LIGHTS_ON", "All_USER_LIGHTS_ON"
                            traitement("ON", adresse, commande, True)
                    End Select
                Catch ex As Exception
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Ecrire", "Modification du device exception : " & ex.Message & " --> " & adresse & " : " & commande & " " & data1 & "-" & data2)
                    Return "" 'on renvoi rien car il y a eu une erreur
                End Try

                'renvoie la valeur ecrite
                Select Case UCase(commande)
                    Case "ON", "ALL_LIGHTS_ON", "All_USER_LIGHTS_ON" : Return "ON"
                    Case "OFF", "ALL_UNITS_OFF", "ALL_LIGHTS_OFF", "All_USER_LIGHTS_OFF", "All_USER_UNITS_OFF" : Return "OFF"
                    Case "PRESET_DIM" : Return "ON" 'data1
                    Case Else : Return ""
                End Select

            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Ecrire", "Port Fermé, impossible d ecrire : " & adresse & " : " & commande & " " & data1 & "-" & data2)
                Return ""
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Ecrire", "exception : " & ex.Message)
            Return "" 'on renvoi rien car il y a eu une erreur
        End Try
    End Function

    Private Function attente_ack() As Boolean
        'test si on recoit le ack pendant 1.5 secondes
        Try
            Dim nbtest As Integer = 0
            While nbtest < 15
                If ackreceived Then 'ack recu on sort
                    ackreceived = False
                    Return True
                Else
                    nbtest += 1
                    wait(10) 'on attend 0.1s
                End If
            End While
            Return False
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS attente_ack", "Exception : " & ex.Message)
        End Try
    End Function

    ''' <summary>Fonction lancée sur reception de données sur le port COM</summary>
    ''' <remarks></remarks>
    Private Sub DataReceived(ByVal sender As Object, ByVal e As SerialDataReceivedEventArgs)
        'fonction qui lit les données sur le port serie
        Try
            Dim count As Integer = 0
            count = port.BytesToRead
            If _IsConnect And count > 0 Then
                port.Read(BufferIn, 0, count)
                For i As Integer = 0 To count - 1
                    ProcessReceivedChar(BufferIn(i))
                Next
            End If
        Catch Ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Datareceived", "Exception : " & Ex.Message)
        End Try
    End Sub

    ''' <summary>Récupére les données reçu du port com et detecte les débuts et fin de paquet</summary>
    ''' <remarks></remarks>
    Private Sub ProcessReceivedChar(ByVal temp As Byte)
        'fonction qui rassemble un message complet
        'si c'est le premier byte qu'on recoit
        Try
            If firstbyte Then
                firstbyte = False
                bytecnt = 0
            End If
            recbuf(bytecnt) = temp
            bytecnt += 1
            If bytecnt = 9 Then
                firstbyte = True
                Process(recbuf)
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS ProcessReceivedChar", "Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Recomponse les messages reçu</summary>
    ''' <remarks></remarks>
    Private Sub Process(ByVal comBuffer() As Byte)
        'traite le message recu
        Dim plcbus_commande As String = ""
        Dim plcbus_adresse As String = ""
        Dim data1 As String = ""
        Dim data2 As String = ""
        Dim listeactif As String = ""
        Dim TblBits(7) As Boolean
        Dim unbyte As Byte
        Dim checksum As Integer
        Dim verifchecksum As Boolean = False

        Try
            ''ONLY FOR DEBUGGING
            '_Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "PLCBUS Process", " Received data: " & comBuffer(0) & "." & comBuffer(1) & "." & comBuffer(2) & "." & comBuffer(3) & "." & comBuffer(4) & "." & comBuffer(5) & "." & comBuffer(6) & "." & comBuffer(7) & "." & comBuffer(8))

            'test du cheksum suivant le modele
            Try
                'If _Modele = "1141+" Then
                '    'pour les modeles 1141+
                '    For i = 0 To 8
                '        checksum += CInt(comBuffer(i))
                '    Next
                '    If (checksum = &H100 Or checksum = &H200 Or checksum = &H300 Or checksum = &H400 Or checksum = &H500) Then verifchecksum = True
                'Else
                '    'pour les modéles 1141
                '    If (comBuffer(8) = &H3) Then verifchecksum = True
                'End If

                'fonctionne pour les deux modele 1141 et 1141+
                For i = 0 To 8
                    checksum += CInt(comBuffer(i))
                Next
                If (comBuffer(8) = &H3 Or (checksum = &H100 Or checksum = &H200 Or checksum = &H300 Or checksum = &H400 Or checksum = &H500)) Then verifchecksum = True

            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Process : Checksum", "Exception : " & ex.Message)
            End Try


            If ((comBuffer(1) = &H6) And (comBuffer(0) = &H2) And verifchecksum) Then ' si trame de reponse valide
                Try
                    plcbus_adresse = hex_to_adresse(comBuffer(3))
                    plcbus_commande = hex_to_com(comBuffer(4))
                    data1 = CStr(comBuffer(5))
                    data2 = CStr(comBuffer(6))
                Catch ex As Exception
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Process : Get data", "Exception : " & ex.Message)
                End Try

                Try
                    'test si c'est un ack
                    unbyte = comBuffer(7)
                    For Iteration As Integer = 0 To 7
                        TblBits(Iteration) = CBool(unbyte And 1)
                        unbyte >>= 1
                    Next
                Catch ex As Exception
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Process : Test Ack", "Exception : " & ex.Message)
                End Try

                If TblBits(4) Then
                    Try
                        'c'est un Ack
                        ackreceived = True
                        If plcbus_commande = "STATUS_REQUEST" Or plcbus_commande = "GetOnlyOnIdPulse" Or plcbus_commande = "GetAllIdPulse" Then
                            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "PLCBUS Process", " <- ACK :" & plcbus_commande & "-" & plcbus_adresse & " : " & data1 & "-" & data2 & " : " & comBuffer(7))
                        Else
                            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "PLCBUS Process", " <- ACK :" & plcbus_commande & "-" & plcbus_adresse & " : " & data1 & "-" & data2 & " : " & comBuffer(7))
                        End If
                    Catch ex As Exception
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Process : Traitement Ack", "Exception : " & ex.Message)
                    End Try
                Else
                    Try
                        'ce n'est pas un ack, je traite le paquet
                        ackreceived = False

                        Select Case plcbus_commande
                            Case "ON", "OFF", "BRIGHT", "BLINK", "FADE_STOP", "STATUS_REQUEST"
                                If CInt(data1) = 0 Then traitement("OFF", plcbus_adresse, plcbus_commande, True) Else traitement("ON", plcbus_adresse, plcbus_commande, True)
                            Case "DIM", "PRESET_DIM"
                                traitement(data1, plcbus_adresse, plcbus_commande, True)
                            Case "GetSignalStrength", "GetNoiseStrength"
                                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "PLCBUS Process", " <- " & plcbus_commande & "-" & plcbus_adresse & " : " & data1)
                            Case "ReceiverMasterAddressSetup", "TransmitterMasterAddressSetup"
                                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "PLCBUS Process", " <- " & plcbus_commande & "-" & plcbus_adresse & " : " & data1 & "-" & data2)
                            Case "ALL_UNITS_OFF"
                                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "PLCBUS Process", " <- " & plcbus_commande & "-" & plcbus_adresse & " : " & data1 & "-" & data2)
                                'il faut ajouter le traitement pour tous les devices affectés pour les mettre à OFF



                            Case "All_USER_UNITS_OFF"
                                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "PLCBUS Process", " <- " & plcbus_commande & "-" & plcbus_adresse & " : " & data1 & "-" & data2)
                                'il faut ajouter le traitement pour tous les devices affectés pour les mettre à OFF



                            Case "ALL_LIGHTS_ON"
                                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "PLCBUS Process", " <- " & plcbus_commande & "-" & plcbus_adresse & " : " & data1 & "-" & data2)
                                'il faut ajouter le traitement pour tous les devices affectés pour les mettre à ON/OFF


                            Case "ALL_LIGHTS_OFF"
                                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "PLCBUS Process", " <- " & plcbus_commande & "-" & plcbus_adresse & " : " & data1 & "-" & data2)
                                'il faut ajouter le traitement pour tous les devices affectés pour les mettre à ON/OFF


                            Case "All_USER_LIGHTS_ON"
                                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "PLCBUS Process", " <- " & plcbus_commande & "-" & plcbus_adresse & " : " & data1 & "-" & data2)
                                'il faut ajouter le traitement pour tous les devices affectés pour les mettre à ON/OFF


                            Case "All_USER_LIGHTS_OFF"
                                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "PLCBUS Process", " <- " & plcbus_commande & "-" & plcbus_adresse & " : " & data1 & "-" & data2)
                                'il faut ajouter le traitement pour tous les devices affectés pour les mettre à ON/OFF



                            Case "STATUS_ON"
                                traitement("ON", plcbus_adresse, plcbus_commande, True)
                            Case "STATUS_OFF"
                                traitement("OFF", plcbus_adresse, plcbus_commande, True)
                            Case "SceneAddressSetup", "SceneAddressErase", "AllSceneAddressErase"
                                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "PLCBUS Process", " <- " & plcbus_commande & "-" & plcbus_adresse & " : " & data1 & "-" & data2)
                            Case "ReportSignalStrength", "ReportNoiseStrength"
                                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "PLCBUS Process", " <- " & plcbus_commande & "-" & plcbus_adresse & " : " & data1 & "-" & data2)
                            Case "GetAllIdPulse", "ReportAllIdPulse3Phase"
                                unbyte = comBuffer(6)
                                For Iteration As Integer = 0 To 7
                                    If CBool((unbyte And 1)) Then listeactif = listeactif & " " & Left(plcbus_adresse, 1) & (Iteration + 1)
                                    unbyte >>= 1
                                Next
                                unbyte = comBuffer(5)
                                For Iteration As Integer = 0 To 7
                                    If CBool((unbyte And 1)) Then listeactif = listeactif & " " & Left(plcbus_adresse, 1) & (Iteration + 9)
                                    unbyte >>= 1
                                Next
                                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "PLCBUS Process", " <- Liste des composants actifs : " & listeactif)
                            Case "GetOnlyOnIdPulse", "ReportOnlyOnIdPulse3Phase"
                                unbyte = comBuffer(6)
                                For Iteration As Integer = 0 To 7

                                    If CBool((unbyte And 1)) Then
                                        listeactif = listeactif & " " & Left(plcbus_adresse, 1) & (Iteration + 1)
                                        traitement("ON", Left(plcbus_adresse, 1) & (Iteration + 1), "GetOnlyOnIdPulse", False)
                                    Else
                                        traitement("OFF", Left(plcbus_adresse, 1) & (Iteration + 1), "GetOnlyOnIdPulse", False)
                                    End If
                                    unbyte >>= 1
                                Next
                                unbyte = comBuffer(5)
                                For Iteration As Integer = 0 To 7
                                    If CBool((unbyte And 1)) Then
                                        listeactif = listeactif & " " & Left(plcbus_adresse, 1) & (Iteration + 9)
                                        traitement("ON", Left(plcbus_adresse, 1) & (Iteration + 9), "GetOnlyOnIdPulse", False)
                                    Else
                                        traitement("OFF", Left(plcbus_adresse, 1) & (Iteration + 9), "GetOnlyOnIdPulse", False)
                                    End If
                                    unbyte >>= 1
                                Next
                                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "PLCBUS Process", " <- Liste des composants ON : " & Left(plcbus_adresse, 1) & " -> " & listeactif)
                        End Select
                    Catch ex As Exception
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Process : Traitement Paquet reçu", "Exception : " & ex.Message)
                    End Try
                End If
            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Process", "Message recu invalide")
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Process", "Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Traite les paquets reçus</summary>
    ''' <remarks></remarks>
    Private Sub traitement(ByVal valeur As String, ByVal adresse As String, ByVal plcbus_commande As String, ByVal erreursidevicepastrouve As Boolean)
        If valeur <> "" Then
            Try

                'Recherche si un device affecté
                Dim listedevices As New ArrayList
                listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, adresse, "", Me._ID, True)
                If IsNothing(listedevices) Then
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Process", "Communication impossible avec le serveur, l'IDsrv est peut être erroné : " & _IdSrv)
                    Exit Sub
                End If
                'un device trouvé on maj la value
                If (listedevices.Count = 1) Then
                    'correction valeur pour correspondre au type de value
                    If TypeOf listedevices.Item(0).Value Is Integer Then
                        If valeur = "ON" Then
                            listedevices.Item(0).Value = 100
                        ElseIf valeur = "OFF" Then
                            listedevices.Item(0).Value = 0
                        End If
                    ElseIf TypeOf listedevices.Item(0).Value Is Boolean Then
                        If valeur = "ON" Then
                            listedevices.Item(0).Value = True
                        ElseIf valeur = "OFF" Then
                            listedevices.Item(0).Value = False
                        Else
                            listedevices.Item(0).Value = True
                        End If
                    Else
                        listedevices.Item(0).Value = valeur
                    End If
                ElseIf (listedevices.Count > 1) Then
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Process", "Plusieurs devices correspondent à : " & adresse & ":" & valeur)
                Else
                    If erreursidevicepastrouve Then
                        'si autodiscover = true ou modedecouverte du serveur actif alors on crée le composant sinon on logue
                        If _AutoDiscover Or _Server.GetModeDecouverte Then
                            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "PLCBUS Process", "Device non trouvé, AutoCreation du composant : " & adresse & ":" & valeur)
                            _Server.AddDetectNewDevice(adresse, _ID, "", "", valeur)
                        Else
                            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Process", "Device non trouvé : " & adresse & ":" & valeur)
                        End If
                    End If
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Process", "Device non trouvé : " & adresse & ":" & valeur)


                    'Ajouter la gestion des composants bannis (si dans la liste des composant bannis alors on log en debug sinon onlog device non trouve empty)


                End If
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS traitement", "Exception : " & ex.Message & " --> " & adresse & " : " & plcbus_commande & "-" & valeur)
            End Try
        End If
    End Sub

#End Region

End Class
