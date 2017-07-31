Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports System.Net
Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.Text.RegularExpressions
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Xml.XPath

<Serializable()> Public Class Driver_Wattlet
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "06338F9C-6B28-11E7-9C18-39FC4E7303AE" 'ne pas modifier car utilisé dans le code du serveur
    Dim _Nom As String = "Wattlet" 'Nom du driver à afficher
    Dim _Enable As Boolean = False 'Activer/Désactiver le driver
    Dim _Description As String = "Driver Wattlet" 'Description du driver
    Dim _StartAuto As Boolean = False 'True si le driver doit démarrer automatiquement
    Dim _Protocol As String = "Http" 'Protocole utilisé par le driver, exemple: RS232
    Dim _IsConnect As Boolean = False 'True si le driver est connecté et sans erreur
    Dim _IP_TCP As String = "192.168.0.1" 'Adresse IP TCP à utiliser, "@" si non applicable pour le cacher côté client
    Dim _Port_TCP As String = "80" 'Port TCP à utiliser, "@" si non applicable pour le cacher côté client
    Dim _IP_UDP As String = "@" 'Adresse IP UDP à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Port_UDP As String = "@" 'Port UDP à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Com As String = "@" 'Port COM à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Refresh As Integer = 0 'Valeur à laquelle le driver doit rafraichir les valeurs des devices (ex: toutes les 200ms aller lire les devices)
    Dim _Modele As String = "" 'Modèle du driver/interface
    Dim _Version As String = My.Application.Info.Version.ToString 'Version du driver
    Dim _OsPlatform As String = "3264" 'plateforme compatible 32 64 ou 3264 bits
    Dim _Picture As String = "" 'Image du driver (non utilisé actuellement)
    Dim _Server As HoMIDom.HoMIDom.Server 'Objet Reflètant le serveur
    Dim _DeviceSupport As New ArrayList 'Type de Device supporté par le driver
    Dim _Device As HoMIDom.HoMIDom.Device 'Image reflétant un device
    Dim _Parametres As New ArrayList 'Paramètres supplémentaires associés au driver
    Dim _LabelsDriver As New ArrayList 'Libellés, tooltip associés au driver
    Dim _LabelsDevice As New ArrayList 'Libellés, tooltip des devices associés au driver
    Dim MyTimer As New Timers.Timer 'Timer du driver
    Dim _IdSrv As String 'Id du Serveur (pour autoriser à utiliser des commandes)
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande) 'Liste des commandes avancées du driver
    Dim _AutoDiscover As Boolean = False

    'A ajouter dans les ppt du driver

    'param avancé
    Dim _DEBUG As Boolean = False

#End Region

#Region "Variables Internes"
    'Insérer ici les variables internes propres au driver et non communes

    Dim _UrlWattCube As String = ""
    Dim _Adr1Txt As New ArrayList()
    Dim _Username As String = ""
    Dim _Password As String = ""
    Dim versionWattCube As String = ""


    ' config wattcube web etmodules http://192.168.0.1/config.xml

    ' liste modules http://192.168.0.1/status.json
    '{"modules":[{"address":1441,"name":"1441","io1_name":"1441_io1","io2_name":"1441_io2","type":"POWER","state":{"io1":0,"io2":0},"soft":0,"hard":0}]}

    ' juste pour info, pas utilisé dans le driver
    ' detail module http://192.168.0.1/status_2.json  calendrier, extinction avant
    ' {"modules":[{"address":1441,"b_delay":{"io1":0,"io2":1},"delay":{"io1":{"day":0,"hour":0,"minute":0,"second":0},"io2":{"day":0,"hour":0,"minute":0,"second":0}},"nb_calendars":0,"calendars":{"io1":[],"io2":[]}}]}

    Dim devlist As ListModules
    Public Class ListModules
        Public modules As List(Of Modules)
    End Class

    Public Class Modules
        Public address As Integer
        Public name As String
        Public io1_name As String
        Public io2_name As String
        Public type As String
        Public state As Etat
        Public soft As Integer
        Public hard As Integer
   End Class

    Public Class Etat
        Public io1 As Integer
        Public io2 As Integer
    End Class
#End Region

#Region "Propriétés génériques"
    ''' <summary>
    ''' Evènement déclenché par le driver au serveur
    ''' </summary>
    ''' <param name="DriveName"></param>
    ''' <param name="TypeEvent"></param>
    ''' <param name="Parametre"></param>
    ''' <remarks></remarks>
    Public Event DriverEvent(ByVal DriveName As String, ByVal TypeEvent As String, ByVal Parametre As Object) Implements HoMIDom.HoMIDom.IDriver.DriverEvent

    ''' <summary>
    ''' ID du serveur
    ''' </summary>
    ''' <value>ID du serveur</value>
    ''' <remarks>Permet d'accéder aux commandes du serveur pour lesquels il faut passer l'ID du serveur</remarks>
    Public WriteOnly Property IdSrv As String Implements HoMIDom.HoMIDom.IDriver.IdSrv
        Set(ByVal value As String)
            _IdSrv = value
        End Set
    End Property

    ''' <summary>
    ''' Port COM du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
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

    ''' <summary>
    ''' Retourne la liste des devices supportés par le driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Voir Sub New</remarks>
    Public ReadOnly Property DeviceSupport() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.DeviceSupport
        Get
            Return _DeviceSupport
        End Get
    End Property

    ''' <summary>
    ''' Liste des paramètres avancés du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Voir Sub New</remarks>
    Public Property Parametres() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.Parametres
        Get
            Return _Parametres
        End Get
        Set(ByVal value As System.Collections.ArrayList)
            _Parametres = value
        End Set
    End Property

    ''' <summary>
    ''' Liste les libellés et tooltip des champs associés au driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LabelsDriver() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.LabelsDriver
        Get
            Return _LabelsDriver
        End Get
        Set(ByVal value As System.Collections.ArrayList)
            _LabelsDriver = value
        End Set
    End Property

    ''' <summary>
    ''' Liste les libellés et tooltip des champs associés au device associé au driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LabelsDevice() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.LabelsDevice
        Get
            Return _LabelsDevice
        End Get
        Set(ByVal value As System.Collections.ArrayList)
            _LabelsDevice = value
        End Set
    End Property

    ''' <summary>
    ''' Active/Désactive le driver
    ''' </summary>
    ''' <value>True si actif</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Enable() As Boolean Implements HoMIDom.HoMIDom.IDriver.Enable
        Get
            Return _Enable
        End Get
        Set(ByVal value As Boolean)
            _Enable = value
        End Set
    End Property

    ''' <summary>
    ''' ID du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ID() As String Implements HoMIDom.HoMIDom.IDriver.ID
        Get
            Return _ID
        End Get
    End Property

    ''' <summary>
    ''' Adresse IP TCP du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IP_TCP() As String Implements HoMIDom.HoMIDom.IDriver.IP_TCP
        Get
            Return _IP_TCP
        End Get
        Set(ByVal value As String)
            _IP_TCP = value
        End Set
    End Property

    ''' <summary>
    ''' Adresse IP UDP du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IP_UDP() As String Implements HoMIDom.HoMIDom.IDriver.IP_UDP
        Get
            Return _IP_UDP
        End Get
        Set(ByVal value As String)
            _IP_UDP = value
        End Set
    End Property

    ''' <summary>
    ''' Permet de savoir si le driver est actif
    ''' </summary>
    ''' <value>Retourne True si le driver est démarré</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsConnect() As Boolean Implements HoMIDom.HoMIDom.IDriver.IsConnect
        Get
            Return _IsConnect
        End Get
    End Property

    ''' <summary>
    ''' Modèle du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Modele() As String Implements HoMIDom.HoMIDom.IDriver.Modele
        Get
            Return _Modele
        End Get
        Set(ByVal value As String)
            _Modele = value
        End Set
    End Property

    ''' <summary>
    ''' Nom du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Nom() As String Implements HoMIDom.HoMIDom.IDriver.Nom
        Get
            Return _Nom
        End Get
    End Property

    ''' <summary>
    ''' Image du driver (non utilisé)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Picture() As String Implements HoMIDom.HoMIDom.IDriver.Picture
        Get
            Return _Picture
        End Get
        Set(ByVal value As String)
            _Picture = value
        End Set
    End Property

    ''' <summary>
    ''' Port TCP du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Port_TCP() As String Implements HoMIDom.HoMIDom.IDriver.Port_TCP
        Get
            Return _Port_TCP
        End Get
        Set(ByVal value As String)
            _Port_TCP = value
        End Set
    End Property

    ''' <summary>
    ''' Port UDP du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Port_UDP() As String Implements HoMIDom.HoMIDom.IDriver.Port_UDP
        Get
            Return _Port_UDP
        End Get
        Set(ByVal value As String)
            _Port_UDP = value
        End Set
    End Property

    ''' <summary>
    ''' Type de protocole utilisé par le driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Protocol() As String Implements HoMIDom.HoMIDom.IDriver.Protocol
        Get
            Return _Protocol
        End Get
    End Property

    ''' <summary>
    ''' Valeur de rafraichissement des devices
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Refresh() As Integer Implements HoMIDom.HoMIDom.IDriver.Refresh
        Get
            Return _Refresh
        End Get
        Set(ByVal value As Integer)
            _Refresh = value
        End Set
    End Property

    ''' <summary>
    ''' Objet représentant le serveur
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Server() As HoMIDom.HoMIDom.Server Implements HoMIDom.HoMIDom.IDriver.Server
        Get
            Return _Server
        End Get
        Set(ByVal value As HoMIDom.HoMIDom.Server)
            _Server = value
        End Set
    End Property

    ''' <summary>
    ''' Version du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
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

    ''' <summary>
    ''' True si le driver doit démarrer automatiquement
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
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

    ''' <summary>Retourne la liste des Commandes avancées de type DeviceCommande</summary>
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
                If Command = "" Then
                    Return False
                Else
                    _UrlWattCube = "http://" & _IP_TCP & ":" & _Port_TCP & "/"
                    Dim urlcommand As String = ""
                    Select Case UCase(Command)
                        Case "ALL_LIGHT_ON"
                            urlcommand = _UrlWattCube & "000000/wrio/01"
                            Get_response(urlcommand, _Username, _Password)
                            WriteLog("DBG: " & "ExecuteCommand, Passage par la commande ALL_LIGHT_ON, " & urlcommand)
                        Case "ALL_LIGHT_OFF"
                            urlcommand = _UrlWattCube & "000000/wrio/00"
                            Get_response(urlcommand, _Username, _Password)
                            WriteLog("DBG: " & "ExecuteCommand, Passage par la commande ALL_LIGHT_OFF, " & urlcommand)
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

    ''' <summary>Permet de vérifier si un champ est valide</summary>
    ''' <param name="Champ">Nom du champ à vérifier, ex ADRESSE1</param>
    ''' <param name="Value">Valeur à vérifier</param>
    ''' <returns>Retourne 0 si OK, sinon un message d'erreur</returns>
    ''' <remarks></remarks>
    Public Function VerifChamp(ByVal Champ As String, ByVal Value As Object) As String Implements HoMIDom.HoMIDom.IDriver.VerifChamp
        Try
            Dim retour As String = "0"
            Select Case UCase(Champ)
                Case "ADRESSE1"
                    If Value IsNot Nothing Then
                        If String.IsNullOrEmpty(Value) Or IsNumeric(Value) Then
                            retour = "Veuillez choisir l'équipement"
                        End If
                    End If
                Case "ADRESSE2"

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
            If My.Computer.Network.IsAvailable = False Then
                _IsConnect = False
                WriteLog("ERR: Pas d'accés réseau! Vérifiez votre connection")
                WriteLog("Driver non démarré")
                Exit Sub
            End If

            Try
                _DEBUG = _Parametres.Item(0).Valeur
                _Username = _Parametres.Item(1).Valeur
                _Password = _Parametres.Item(2).Valeur
            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Erreur dans les paramétres avancés. utilisation des valeur par défaut : " & ex.Message)
            End Try

            _UrlWattCube = "http://" & _IP_TCP & ":" & _Port_TCP & "/"
            WriteLog("Start, connection au serveur " & _UrlWattCube)

            If Get_Config(_UrlWattCube & "config.xml", _Username, _Password) Then
                _IsConnect = True
                WriteLog("Driver " & Me.Nom & " démarré avec succés à l'adresse " & _UrlWattCube)
                WriteLog(" Wattcube web version " & versionWattCube)

                Get_ConfigDevice()
                WriteLog(devlist.modules.Count & " modules trouvés")
                For i = 0 To devlist.modules.Count - 1
                    WriteLog("idDevice : " & devlist.modules.Item(i).address & " / " & devlist.modules.Item(i).name & " - version hard/soft " & Hex(devlist.modules.Item(0).hard) & " / " & Hex(devlist.modules.Item(0).soft))
                Next
                'lance le time du driver, mini toutes les minutes
                If _Refresh = 0 Then _Refresh = 5
                MyTimer.Interval = _Refresh * 1000
                MyTimer.Enabled = True
                AddHandler MyTimer.Elapsed, AddressOf TimerTick

            Else
                _IsConnect = False
                WriteLog("Driver " & Me.Nom & " non démarré à l'adresse " & _UrlWattCube)
            End If
        Catch ex As Exception
            _IsConnect = False
            WriteLog("ERR: Driver " & Me.Nom & " Erreur démarrage " & ex.Message)
            WriteLog("Driver non démarré")
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            _IsConnect = False
            MyTimer.Enabled = False
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
    ''' <remarks>Le device demande au driver d'aller le lire suivant son adresse</remarks>
    Public Sub Read(ByVal Objet As Object) Implements HoMIDom.HoMIDom.IDriver.Read
        Try
            If _Enable = False Then
                WriteLog("ERR: Read, Erreur: Impossible de traiter la commande car le driver n'est pas activé (Enable)")
                Exit Sub
            End If
            Try ' lecture de la variable debug, permet de rafraichir la variable debug sans redemarrer le service
                _DEBUG = _Parametres.Item(0).Valeur
            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Erreur de lecture de debug : " & ex.Message)
            End Try

            'Si internet n'est pas disponible on ne mets pas à jour les informations
            If My.Computer.Network.IsAvailable = False Then
                WriteLog("ERR: READ, Pas de réseau! Lecture du périphérique impossible")
                Exit Sub
            End If
        Catch ex As Exception
            WriteLog("ERR: Read, adresse1 : " & Objet.adresse1 & " - adresse2 : " & Objet.adresse2)
            WriteLog("ERR: Read, Exception : " & ex.Message)
        End Try

    End Sub

    ''' <summary>Commander un device</summary>
    ''' <param name="Objet">Objet représetant le device à commander</param>
    ''' <param name="Command">La commande à passer</param>
    ''' <param name="Parametre1">parametre 1 de la commande, optionnel</param>
    ''' <param name="Parametre2">parametre 2 de la commande, optionnel</param>
    ''' <remarks></remarks>
    Public Sub Write(ByVal Objet As Object, ByVal Command As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
        Try
            If _Enable = False Then
                WriteLog("ERR: Read, Erreur: Impossible de traiter la commande car le driver n'est pas activé (Enable)")
                Exit Sub
            End If
            Try ' lecture de la variable debug, permet de rafraichir la variable debug sans redemarrer le service
                _DEBUG = _Parametres.Item(0).Valeur
                _Username = _Parametres.Item(1).Valeur
                _Password = _Parametres.Item(2).Valeur
            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Erreur de lecture de debug : " & ex.Message)
            End Try

            'Si internet n'est pas disponible on ne mets pas à jour les informations
            If My.Computer.Network.IsAvailable = False Then
                WriteLog("ERR: READ, Pas de réseau! Lecture du périphérique impossible")
                Exit Sub
            End If

            Dim iddevice As String = Trim(Mid(Objet.adresse1, 1, InStr(Objet.adresse1, "#") - 1))
            Dim identree As String = Int(Trim(Mid(Objet.adresse2, 1, InStr(Objet.adresse2, "#") - 1)) - 1).ToString

            _UrlWattCube = "http://" & _IP_TCP & ":" & _Port_TCP & "/"
            Dim urlcommand As String = ""
            Try
                Select Case Objet.Type
                    Case "LAMPE", "APPAREIL", "SWITCH", "VOLET"
                        Select Case Command
                            Case "ON", "OUVERTURE"
                                urlcommand = _UrlWattCube & iddevice & "/wrio/" & identree & "1"
                                Get_response(urlcommand, _Username, _Password)
                            Case "OFF", "FERMETURE"
                                urlcommand = _UrlWattCube & iddevice & "/wrio/" & identree & "0"
                                Get_response(urlcommand, _Username, _Password)
                        End Select
                        WriteLog("DBG: commande lancée : " & Command & ", à l'url " & urlcommand)
                    Case Else
                        WriteLog("ERR: WRITE Erreur Write Type de composant non géré : " & Objet.Type.ToString)
                End Select

            Catch ex As Exception
                WriteLog("ERR: WRITE Erreur commande " & Command & " : " & ex.ToString)
            End Try
        Catch ex As Exception
            WriteLog("ERR: WRITE, Exception : " & ex.Message)
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
            WriteLog("ERR: add_DeviceCommande, Exception :" & ex.Message)
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
            WriteLog("ERR: add_LibelleDriver, Exception : " & ex.Message)
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
            WriteLog("ERR: add_LibelleDevice, Exception : " & ex.Message)
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
            WriteLog("ERR: add_ParamAvance, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Creation d'un objet de type</summary>
    ''' <remarks></remarks>
    Public Sub New()
        Try
            _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString

            'liste des devices compatibles
            _DeviceSupport.Add(ListeDevices.APPAREIL)
            _DeviceSupport.Add(ListeDevices.COMPTEUR)
            _DeviceSupport.Add(ListeDevices.CONTACT)
            _DeviceSupport.Add(ListeDevices.LAMPE)
            _DeviceSupport.Add(ListeDevices.SWITCH)
            _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN)
            _DeviceSupport.Add(ListeDevices.GENERIQUESTRING)
            _DeviceSupport.Add(ListeDevices.GENERIQUEVALUE)
            _DeviceSupport.Add(ListeDevices.VOLET)
            _DeviceSupport.Add(ListeDevices.ENERGIEINSTANTANEE)


            'Parametres avancés
            Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)
            Add_ParamAvance("Username", "Nom utilisateur", "admin")
            Add_ParamAvance("Password", "Mot de passe", "homi123456")

            'ajout des commandes avancées pour les devices

            'ajout des commandes avancées pour les devices
            Add_DeviceCommande("ALL_LIGHT_ON", "", 0)
            Add_DeviceCommande("ALL_LIGHT_OFF", "", 0)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Numéro de l'équipement", "Numéro de l'équipement")
            Add_LibelleDevice("ADRESSE2", "Entrée de la commande", "Entrée de la commande ( 1 ou 2 )")
            Add_LibelleDevice("REFRESH", "Refresh (sec)", "Valeur de rafraîchissement de la mesure en secondes")
            'Add_LibelleDevice("LASTCHANGEDUREE", "LastChange Durée", "")

        Catch ex As Exception
            WriteLog("ERR: New, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)
        If (_Enable = False) Or (_IsConnect = False) Then
            'arrete le timer du driver
            MyTimer.Enabled = False
            Exit Sub
        End If
        Get_ConfigDevice()
        Get_Value()
    End Sub


#End Region

#Region "Fonctions internes"
    'Insérer ci-dessous les fonctions propres au driver
    Function Get_response(url As String, user As String, password As String) As String
        ' recupere les configarions des equipements et scenarios de Wattlet
        Try
            Dim client As New Net.WebClient
            client.Credentials = New NetworkCredential(user, password)

            Dim responsebody As String = client.DownloadString(url)
            While client.IsBusy
            End While

            Return responsebody
        Catch ex As Exception

            WriteLog("ERR: " & "GET_response, " & ex.Message)
            WriteLog("ERR: " & "GET_response, Url: " & _UrlWattCube)
            Return False
        End Try

    End Function

    Function Get_Config(adrs As String, user As String, password As String) As Boolean
        ' recupere les configuration de WattCube_Web

        WriteLog("DBG: Get_config, Acquisition fichier xml -> " & adrs)
        Try
            Dim client As New Net.WebClient
            client.Credentials = New NetworkCredential(user, password)

            Dim responsebody As String = client.DownloadString(adrs)
            While client.IsBusy
            End While

            Dim response As String = ""
            If responsebody <> "" Then
                response = Mid(responsebody, InStr(responsebody, "<version>") + 9, InStr(responsebody, "</version>") - (InStr(responsebody, "<version>") + 9))
            End If
            versionWattCube = response
            Return True

        Catch ex As Exception
            WriteLog("ERR: " & "GET_Config, " & ex.Message)
            WriteLog("ERR: " & "GET_Config, Url: " & _UrlWattCube)
            Return False
        End Try
    End Function
    Function Get_ConfigDevice() As Boolean
        ' recupere les configarions des equipements de Wattlet

        Try
            Dim adrs As String = _UrlWattCube & "status.json"
            _Adr1Txt.Clear()
            WriteLog("DBG: " & "GET_CONFIGDevice Url: " & adrs)

            Dim responsebody As String = Get_response(adrs, _Username, _Password)

            WriteLog("DBG: Get_ConfigDevice inputs : " & responsebody.ToString)
            devlist = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebody, GetType(ListModules))
            Dim adress As String = ""
            Dim _libelleadr1 As String = ""
            Dim NbSortie As String = ""
            For i = 0 To devlist.modules.Count - 1
                adress = Mid("000000", Len(devlist.modules.Item(i).address) + 1, 6) & devlist.modules.Item(i).address
                _libelleadr1 += adress & " # " & devlist.modules.Item(i).type & "|"
                _Adr1Txt.Add(adress & " # " & devlist.modules.Item(i).type)
                WriteLog("DBG: Device : " & devlist.modules.Item(i).name & " | type -> " & devlist.modules.Item(i).type & ", ID = " & adress)
                Dim IdMod As String = ""
                IdMod += adress & " # " & devlist.modules.Item(i).type & "|"
                Select Case UCase(devlist.modules.Item(i).type)
                    Case "POWER"
                        NbSortie += adress & " #; " & "1" & " # " & "Sortie 1"
                    Case "PUSH"
                        NbSortie += adress & " #; " & "1" & " # " & "Sortie 1"
                    Case "PUSH2"
                        NbSortie += adress & " #; " & "1" & " # " & "Sortie 1"
                        NbSortie += adress & " #; " & "2" & " # " & "Sortie 2"
                    Case "PUSHL"
                        NbSortie += adress & " #; " & "1" & " # " & "Sortie 1"
                    Case "LIGHT"
                        NbSortie += adress & " #; " & "1" & " # " & "Sortie 1"
                    Case "LIGHT2"
                        NbSortie += adress & " #; " & "1" & " # " & "Sortie 1"
                        NbSortie += adress & " #; " & "2" & " # " & "Sortie 2"
                    Case "OPEN"
                        NbSortie += adress & " #; " & "1" & " # " & "Sortie 1"
                    Case "DSC"
                        NbSortie += adress & " #; " & "1" & " # " & "Sortie 1"
                    Case "VMC"
                        NbSortie += adress & " #; " & "1" & " # " & "Sortie 1"
                        NbSortie += adress & " #; " & "2" & " # " & "Sortie 2"
                    Case "PILOT"
                        NbSortie += adress & " #; " & "1" & " # " & "Sortie 1"
                    Case "WIN"
                        NbSortie += adress & " #; " & "1" & " # " & "Montée 1"
                        NbSortie += adress & " #; " & "2" & " # " & "Descente 1"
                End Select
            Next

            Dim ld0 As New HoMIDom.HoMIDom.Driver.cLabels
            For i As Integer = 0 To _LabelsDevice.Count - 1
                ld0 = _LabelsDevice(i)
                Select Case ld0.NomChamp
                    Case "ADRESSE1"
                        _libelleadr1 = Mid(_libelleadr1, 1, Len(_libelleadr1) - 1) 'enleve le dernier | pour eviter davoir une ligne vide a la fin
                        ld0.Parametre = _libelleadr1
                        _LabelsDevice(i) = ld0
                    Case "ADRESSE2"
                        NbSortie = Mid(NbSortie, 1, Len(NbSortie) - 1) 'enleve le dernier | pour eviter davoir une ligne vide a la fin
                        'si 1 seule entree, enleve la reference au module
                        If devlist.modules.Count = 1 Then NbSortie = Mid(NbSortie, InStr(NbSortie, "#;") + 3, Len(NbSortie))
                        ld0.Parametre = NbSortie
                        _LabelsDevice(i) = ld0
                End Select
            Next
            Return True
        Catch ex As Exception
            WriteLog("ERR: " & "GET_ConfigDevice, " & ex.Message)
            WriteLog("ERR: " & "GET_ConfigDevice, Url: " & _UrlWattCube)
            Return False
        End Try
    End Function
    Function Get_Value() As Boolean
        ' recupere les valeurs des equipements Wattlet

        Try
            'Si internet n'est pas disponible on ne mets pas à jour les informations
            If My.Computer.Network.IsAvailable = False Then
                Return False
            End If

            Dim deviceadr1 As New ArrayList()
            Dim devices As New ArrayList()
            For i = 0 To _Adr1Txt.Count - 1
                deviceadr1 = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, _Adr1Txt.Item(i), "", Me._ID, True)
                If deviceadr1.Count > 0 Then
                    WriteLog("DBG: ADR1 : " & devlist.modules.Item(i).address & " -> Nb Device " & deviceadr1.Count)
                    devices.AddRange(deviceadr1)
                End If
                Dim idCom As String = ""
                For Each LocalDevice As Object In devices
                    If LocalDevice.adresse2 <> "" Then ' cas ou saisi dans la zone texte
                        If InStr(LocalDevice.adresse2, "#") > 0 Then
                            idCom = (Trim(Mid(LocalDevice.adresse2, 1, InStr(LocalDevice.adresse2, "#") - 1)))
                        Else
                            idCom = LocalDevice.adresse2
                        End If
                    End If
                    WriteLog("DBG: " & "idCom, " & idCom)

                    Select Case LocalDevice.GetType.Name
                        Case "LAMPE", "APPAREIL", "CONTACT", "SWITCH"
                            If idCom = 1 Then
                                If LocalDevice.Value <> devlist.modules.Item(i).state.io1 Then LocalDevice.Value = devlist.modules.Item(i).state.io1
                            Else
                                If LocalDevice.Value <> devlist.modules.Item(i).state.io2 Then LocalDevice.Value = devlist.modules.Item(i).state.io2
                            End If
                        Case "BATTERIE", "TEMPERATURE", "HUMIDITE", "ENERGIEINSTANTANEE", "GENERIQUEVALUE", "COMPTEUR"
                            Dim valuetmp As Integer
                            If idCom = 1 Then
                                valuetmp = Regex.Replace(CStr(devlist.modules.Item(i).state.io1), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                                If LocalDevice.Value <> valuetmp Then LocalDevice.Value = Regex.Replace(CStr(devlist.modules.Item(i).state.io1), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                            Else
                                valuetmp = Regex.Replace(CStr(devlist.modules.Item(i).state.io2), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                                If LocalDevice.Value <> valuetmp Then LocalDevice.Value = Regex.Replace(CStr(devlist.modules.Item(i).state.io2), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                            End If
                        Case "GENERIQUESTRING"
                        Case "GENERIQUEBOOLEEN"
                    End Select
                Next
            Next
                Return True
        Catch ex As Exception
            WriteLog("ERR: " & "GET_Value, " & ex.Message)
            WriteLog("ERR: " & "GET_Value, Url: " & _UrlWattCube)
            Return False
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
