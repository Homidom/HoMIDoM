Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports KNXConnector.EIBD
Imports System.ComponentModel
Imports System.Text.RegularExpressions
Imports System.IO
Imports System.Reflection
Imports System.Threading
Imports System.Threading.Tasks

' Auteur : HoMIDoM
' Date : 05/04/2013

''' <summary>Class Driver_KNX</summary>
''' <remarks></remarks>
<Serializable()> Public Class Driver_KNX
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "80964F08-9E3C-11E2-8A55-A64B6188709B" 'ne pas modifier car utilisé dans le code du serveur
    Dim _Nom As String = "KNX/EIBD" 'Nom du driver à afficher
    Dim _Enable As Boolean = False 'Activer/Désactiver le driver
    Dim _Description As String = "Driver KNX over EIBD" 'Description du driver
    Dim _StartAuto As Boolean = False 'True si le driver doit démarrer automatiquement
    Dim _Protocol As String = "ETHERNET" 'Protocole utilisé par le driver, exemple: RS232
    Dim _IsConnect As Boolean = False 'True si le driver est connecté et sans erreur
    Dim _IP_TCP As String = "" 'Adresse IP TCP à utiliser, "@" si non applicable pour le cacher côté client
    Dim _Port_TCP As String = "" 'Port TCP à utiliser, "@" si non applicable pour le cacher côté client
    Dim _IP_UDP As String = "@" 'Adresse IP UDP à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Port_UDP As String = "@" 'Port UDP à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Com As String = "@" 'Port COM à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Refresh As Integer = 0 'Valeur à laquelle le driver doit rafraichir les valeurs des devices (ex: toutes les 200ms aller lire les devices)
    Dim _Modele As String = "KNX" 'Modèle du driver/interface
    Dim _Version As String = My.Application.Info.Version.ToString 'Version du driver
    Dim _OsPlatform As String = "3264" 'Plateforme compatible 32 64 ou 3264
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

    'param avancé
    Dim _DEBUG As Boolean = False
    Dim _BUSMONITOR As Boolean = False

#End Region

#Region "Variables Internes"

    ' Controlleur d'objets KNX (KNXFramework)
    Dim m_Controller As ObjectController

    ' BackgroundWorker pour le moniteur de bus
    ' How to: Use a Background Worker => http://msdn.microsoft.com/en-us/library/cc221403(v=vs.95).aspx
    Dim bw As BackgroundWorker
    Dim eibc As tuwien.auto.eibclient.EIBConnection


    'accès au fichier de configuration local
    Dim configReader As ConfigReader
    Dim knxSettingsReader As KNXConnector.Settings.SettingsReader

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
    Public ReadOnly Property OsPlatform() As String Implements HoMIDom.HoMIDom.IDriver.OsPlatform
        Get
            Return _OsPlatform
        End Get
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
            Me.LogError(MethodInfo.GetCurrentMethod(), ex)
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

                Case "ADRESSE2"

            End Select
            Return retour
        Catch ex As Exception
            Return "Une erreur est apparue lors de la vérification du champ " & Champ & ": " & ex.ToString
        End Try
    End Function

    ''' <summary>Permet de démarrer le driver</summary>
    ''' <remarks></remarks>
    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        Try

            'Récupération des paramétres avancés
            Try
                _DEBUG = _Parametres.Item(0).Valeur
                _BUSMONITOR = _Parametres.Item(1).Valeur
            Catch ex As Exception
                Me.LogError(MethodInfo.GetCurrentMethod(), "Erreur dans les paramétres avancés. Utilisation des valeur par défaut" & ex.Message)
            End Try

            ' *** Debugger.Launch()

            'Vérification de la présence des fichiers de configuration (obligatoire)
            Dim knxConfigFileName As String = System.IO.Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly().Location) & "\\KNX\\knxobjects.xml"
            If Not File.Exists(knxConfigFileName) Then Throw New Exception(String.Format("Fichier de configuration KNX introuvable ! ({0})", knxConfigFileName))

            Dim configFileName As String = System.IO.Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly().Location) & "\\KNX\\config.xml"
            If Not File.Exists(configFileName) Then Throw New Exception(String.Format("Fichier de configuration du driver introuvable ! ({0})", configFileName))

            'Chargement du fichier de configuration des objets KNX
            knxSettingsReader = New KNXConnector.Settings.SettingsReader()
            knxSettingsReader.ConfigFilePath = New Uri(knxConfigFileName).ToString()

            'Chargement du fichier de configuration des objets "Homidom"
            configReader = New ConfigReader()
            configReader.ConfigFilePath = configFileName

            'Initialisation du controleur KNX
            m_Controller = New ObjectController()
            m_Controller.HostName = IIf(String.IsNullOrEmpty(_IP_TCP), "localhost", _IP_TCP)
            m_Controller.Port = 6720
            If (Not String.IsNullOrEmpty(_Port_TCP)) Then
                Dim _port As Integer
                If Integer.TryParse(_Port_TCP, _port) Then
                    m_Controller.Port = _port
                End If
            End If

            'Démarrage du controleur / Connexion au daemon EIBD
            m_Controller.Start(knxSettingsReader.GetAllObjects())
            _IsConnect = True
            Me.LogMessage(MethodInfo.GetCurrentMethod(), "Connecté à {0}:{1}", m_Controller.HostName, m_Controller.Port)

            'Démarrage du moniteur de bus (optionnel)
            If _BUSMONITOR Then Me.StartMonitor(m_Controller.HostName, m_Controller.Port)

        Catch ex As Exception
            Me.LogError(MethodInfo.GetCurrentMethod(), ex)
        End Try
    End Sub

    ''' <summary>Permet d'arrêter le driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            If (Not IsNothing(m_Controller)) Then
                If (m_Controller.IsRunning) Then
                    m_Controller.Stop()
                End If
            End If

            Me.StopMonitor()

            _IsConnect = False
            Me.LogMessage(MethodInfo.GetCurrentMethod(), "Driver arrêté.")

        Catch ex As Exception
            Me.LogError(MethodInfo.GetCurrentMethod(), ex)
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
            If Not BasicChecks() Then Exit Sub

            'récupération de l'objet HMD
            Dim hmdObj As HMDObject
            hmdObj = configReader.HMDObjects.Where(Function(t) t.Id.ToUpper() = Objet.adresse1.ToString().ToUpper()).FirstOrDefault()
            If hmdObj Is Nothing Then Throw New Exception(String.Format("L'objet '{0}' est introuvable !", Objet.adresse1))
            If hmdObj.ReadObjects.Count = 0 Then Throw New Exception(String.Format("L'objet '{0}' ne dispose pas d'objet KNX 'write' !", Objet.adresse1))

            'récupération de l'objet KNX pour lecture de la valeur
            Dim obj As KNXObjects.ObjectBase
            obj = m_Controller.GetKNXObject(hmdObj.ReadObjects.First())
            If _DEBUG Then LogDebug(MethodInfo.GetCurrentMethod(), "GetKNXObject: GA={0},Value={1}", obj.GroupAddress.ToString(), obj.ToString())

            'Mise à jour de la valeur
            'UpdateDeviceValue(Objet.adresse1, Objet.type.ToString, Objet.Value)



        Catch ex As Exception
            Me.LogError(MethodInfo.GetCurrentMethod(), ex)
        End Try
    End Sub

    ''' <summary>Commander un composant (device)</summary>
    ''' <param name="Objet">Objet représetant le composant à commander</param>
    ''' <param name="Command">La commande à passer</param>
    ''' <param name="Parametre1">parametre 1 de la commande (optionnel)</param>
    ''' <param name="Parametre2">parametre 2 de la commande (optionnel)</param>
    ''' <remarks></remarks>
    Public Sub Write(ByVal Objet As Object, ByVal Command As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
        Try
            If Not BasicChecks() Then Exit Sub

            'récupération de l'objet HMD
            Dim hmdObj As HMDObject
            hmdObj = configReader.HMDObjects.Where(Function(t) t.Id.ToUpper() = Objet.adresse1.ToString().ToUpper()).FirstOrDefault()

            If hmdObj Is Nothing Then Throw New Exception(String.Format("L'objet '{0}' est introuvable !", Objet.adresse1))
            If hmdObj.WriteObjects.Count = 0 Then Throw New Exception(String.Format("L'objet '{0}' ne dispose pas d'objet KNX 'write' !", Objet.adresse1))


            'Execution de la commande : Envoi de la commande au bus KNX
            Select Case UCase(Command)
                Case "ON"
                    m_Controller.SendKNXValue(hmdObj.WriteObjects.First(), 1)
                    Objet.Value = 100
                Case "OFF"
                    m_Controller.SendKNXValue(hmdObj.WriteObjects.First(), 0)
                    Objet.Value = 0
            End Select

            'Mise à jour de la valeur du composant (notification) *** déja mis à jour automatiquement par le serveur 
            'UpdateDeviceValue(Objet.adresse1, Objet.type.ToString, Objet.Value)


        Catch ex As Exception
            Me.LogError(MethodInfo.GetCurrentMethod(), ex)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice
        Try

        Catch ex As Exception
            Me.LogError(MethodInfo.GetCurrentMethod(), ex)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice
        Try

        Catch ex As Exception
            Me.LogError(MethodInfo.GetCurrentMethod(), ex)
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
            Me.LogError(MethodInfo.GetCurrentMethod(), ex)
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
            Me.LogError(MethodInfo.GetCurrentMethod(), ex)
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
            Me.LogError(MethodInfo.GetCurrentMethod(), ex)
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
            Me.LogError(MethodInfo.GetCurrentMethod(), ex)
        End Try
    End Sub

    ''' <summary>Creation d'un objet de type</summary>
    ''' <remarks></remarks>
    Public Sub New()
        Try
            _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString

            'liste des devices compatibles
            '_DeviceSupport.Add(ListeDevices.LAMPE)
            _DeviceSupport.Add(ListeDevices.SWITCH)

            'Parametres avancés
            Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)
            Add_ParamAvance("BusMonitor", "Activer le moniteur de bus KNX (True/False)", True)

            'ajout des commandes avancées pour les devices
            'add_devicecommande("COMMANDE", "DESCRIPTION", nbparametre)
            'add_devicecommande("PRESETDIM", "permet de paramétrer le DIM : param1=niveau, param2=timer", 2)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Object ID", "ID de l'objet spécifié dans le fichier de configuration.")
            Add_LibelleDevice("ADRESSE2", "@", "")
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("MODELE", "@", "")
            Add_LibelleDevice("REFRESH", "@", "")



        Catch ex As Exception
            Me.LogError(MethodInfo.GetCurrentMethod(), ex)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)

    End Sub


#End Region

#Region "Fonctions internes"

    ''' <summary>
    ''' Permet de mettre à jour la valeur d'un composant
    ''' </summary>
    ''' <param name="adresse">ID du composant KNX</param>
    ''' <param name="type"></param>
    ''' <param name="valeur"></param>
    ''' <remarks></remarks>
    Private Sub UpdateDeviceValue(ByVal adresse As String, ByVal type As String, ByVal valeur As String)
        Try
            If Not BasicChecks() Then Exit Sub

            'Recherche du composant concerné
            Dim _devices As New ArrayList
            _devices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, adresse, type, Me._ID, True)
            'Vérifications
            If _devices.Count = 0 Then Throw New Exception(String.Format("Le composant correspondant à l'adresse '{0}' est introuvable !", adresse))
            If _devices.Count > 1 Then Throw New Exception(String.Format("Plusieurs composant correspondant à l'adresse '{0}' !", adresse))

            If valeur = "ON" Then
                If TypeOf _devices.Item(0).Value Is Boolean Then
                    If _devices.Item(0).Value = True Then Exit Sub
                    _devices.Item(0).Value = True
                ElseIf TypeOf _devices.Item(0).Value Is Long Then
                    If _devices.Item(0).Value = 100 Then Exit Sub
                    _devices.Item(0).Value = 100
                Else
                    If _devices.Item(0).Value = "ON" Then Exit Sub
                    _devices.Item(0).Value = "ON"
                End If
            ElseIf valeur = "OFF" Then
                If TypeOf _devices.Item(0).Value Is Boolean Then
                    If _devices.Item(0).Value = False Then Exit Sub
                    _devices.Item(0).Value = False
                ElseIf TypeOf _devices.Item(0).Value Is Long Then
                    If _devices.Item(0).Value = 0 Then Exit Sub
                    _devices.Item(0).Value = 0
                Else
                    If _devices.Item(0).Value = "OFF" Then Exit Sub
                    _devices.Item(0).Value = "OFF"
                End If
            Else
                _devices.Item(0).Value = valeur
            End If


        Catch ex As Exception
            Me.LogError(MethodInfo.GetCurrentMethod(), ex)
        End Try
    End Sub

    ''' <summary>
    ''' Vérifications de base : 
    ''' * Le driver doit être activé
    ''' * Le driver doit être connecté au daemon EIBD
    ''' </summary>
    ''' <returns>True | False</returns>
    ''' <remarks></remarks>
    Private Function BasicChecks() As Boolean
        Try
            If _Enable = False Then Throw New Exception("Impossible de traiter la commande car le driver n'est pas activé (Enable)")
            If _IsConnect = False Then Throw New Exception("Impossible de traiter la commande car le driver n'est pas démarré.")
            Return True
        Catch ex As Exception
            Me.LogError(MethodInfo.GetCurrentMethod(), ex)
            Return False
        End Try
    End Function

#End Region

#Region "Moniteur de Bus KNX"

    Private Sub StartMonitor(ByVal hostname As String, Optional ByVal port As Integer = 6720)
        Try
            If bw Is Nothing Then
                ' Moniteur de bus KNX
                bw = New BackgroundWorker()
                bw.WorkerReportsProgress = True
                bw.WorkerSupportsCancellation = True
                AddHandler bw.DoWork, AddressOf StartBusMonitor
                AddHandler bw.ProgressChanged, AddressOf BusMonitorEvent
                AddHandler bw.RunWorkerCompleted, AddressOf BusMonitorStoped
            End If

            If Not bw.IsBusy = True Then
                LogMessage(MethodInfo.GetCurrentMethod, "Démarrage du Moniteur de Bus - {0}", Thread.CurrentThread.ManagedThreadId)
                bw.RunWorkerAsync(String.Format("{0}:{1}", hostname, port))
            End If
        Catch ex As Exception
            Me.LogError(MethodInfo.GetCurrentMethod(), ex)
        End Try
    End Sub

    Private Sub StopMonitor()

        Try

            If bw.WorkerSupportsCancellation = True Then
                bw.CancelAsync()
            End If
            eibc.EIBClose()
        Catch ex As Exception
            Me.LogError(MethodInfo.GetCurrentMethod(), ex)
        End Try
    End Sub

    Private Sub StartBusMonitor(ByVal sender As Object, ByVal e As DoWorkEventArgs)
        Try
            'Traitement des paramètres
            Dim worker As BackgroundWorker = CType(sender, BackgroundWorker)



            Dim _hostname As String = Regex.Split(e.Argument, ":")(0)
            Dim _port As Integer = Convert.ToInt32(Regex.Split(e.Argument, ":")(1))
            'Connection au serveur eibd
            eibc = New tuwien.auto.eibclient.EIBConnection(_hostname, _port)
            'Activation du moniteur de bus 
            If eibc.EIBOpenVBusmonitorText() = -1 Then Throw New Exception("Impossible de démarrer le moniteur de bus KNX !")

            LogMessage(MethodInfo.GetCurrentMethod, "Moniteur de Bus KNX démarré - {0}", Thread.CurrentThread.ManagedThreadId)
            Do
                If bw.CancellationPending = True Then
                    e.Cancel = True
                    Exit Do
                Else
                    Dim buf As tuwien.auto.eibclient.EIBBuffer = New tuwien.auto.eibclient.EIBBuffer()
                    Dim len As Integer = eibc.EIBGetBusmonitorPacket(buf)
                    If len = -1 Then
                        Exit Do
                    End If

                    If Not IsNothing(buf.data) Then
                        worker.ReportProgress(0, System.Text.Encoding.Default.GetString(buf.data))
                    End If
                End If
            Loop

            eibc.EIBClose()

        Catch ex As Exception
            If bw.CancellationPending Then
                e.Cancel = True
                Exit Sub
            End If
            Me.LogError(MethodInfo.GetCurrentMethod(), ex)
        End Try
    End Sub

    Private Sub BusMonitorEvent(ByVal sender As Object, ByVal e As ProgressChangedEventArgs)
        ParseBusMonitorMessage(Convert.ToString(e.UserState))
    End Sub



    Private Sub ParseBusMonitorMessage(ByVal message As String)

        Try
            If String.IsNullOrEmpty(message) Then Exit Sub
            Dim knxMsg As KNXLogMessage = New KNXLogMessage()
            knxMsg.Message = message

            If _DEBUG Then LogDebug(MethodInfo.GetCurrentMethod(), message)

            ' traitement des messages qui ne proviennent pas de l'interface IP (EIBD)
            If knxMsg.From <> "0.0.0" Then

                'récupération de l'objet KNX concerné
                Dim knxObject As KNXObjects.ObjectBase = knxSettingsReader.GetAllObjects.Where(Function(t) t.GroupAddress.ToString() = knxMsg.To).FirstOrDefault()
                If knxObject Is Nothing Then Throw New WarningException("Objet KNX introuvable !")

                'récupération des objets HMD concernés
                ' - objets HMD dont l'objet KNX fait partie de la liste des objets KNX 'Read'
                Dim hmdObjects As List(Of HMDObject) = configReader.HMDObjects.Where(Function(t) t.ReadObjects.Any(Function(o) o = knxObject.ID)).ToList()
                If hmdObjects.Count = 0 Then Throw New WarningException("Objet(s) HMD introuvable(s) ! [knxObject.ID=" + knxObject.ID + "]")

                For Each hmdObject As HMDObject In hmdObjects

                    'Dim encoding As New System.Text.UTF8Encoding()
                    'hmdObject.Value = BitConverter.ToString(encoding.GetBytes(knxMsg.Value))

                    Select Case hmdObject.Type.ToLower()
                        Case "switch"
                            'switch: on attend une valeur de type : 
                            '* (small) 01 => ON
                            '* (small) 00 => OFF

                            Select Case knxMsg.Value
                                Case "(small) 01"
                                    hmdObject.Value = "ON"
                                Case "(small) 00"
                                    hmdObject.Value = "OFF"
                            End Select

                            LogMessage(MethodInfo.GetCurrentMethod(), "Id={0}, Type={1}, Value={2}", hmdObject.Id, hmdObject.Type.ToUpper(), knxMsg.Value + " | " + hmdObject.Value)
                            UpdateDeviceValue(hmdObject.Id, hmdObject.Type.ToUpper(), hmdObject.Value)

                        Case "dimmer"

                        Case Else

                    End Select


                Next

            End If




        Catch wex As WarningException
            If _DEBUG Then LogDebug(MethodInfo.GetCurrentMethod(), wex.Message)
        Catch ex As Exception
            LogError(MethodInfo.GetCurrentMethod(), ex.Message)
        End Try

    End Sub

    Private Sub BusMonitorStoped(sender As Object, e As RunWorkerCompletedEventArgs)
        LogMessage(MethodInfo.GetCurrentMethod, "Moniteur de Bus arrété.")
    End Sub

#End Region

#Region "Custom Log Methods"

    Private Sub LogMessage(ByVal method As System.Reflection.MethodBase, ByVal message As String)
        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & ":" & method.Name, message)
    End Sub
    Private Sub LogMessage(ByVal method As System.Reflection.MethodBase, ByVal format As String, ByVal ParamArray args() As Object)
        LogMessage(method, String.Format(format, args))
    End Sub
    Private Sub LogError(ByVal method As System.Reflection.MethodBase, ByVal message As String)
        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & ":" & method.Name, message)
    End Sub
    Private Sub LogError(ByVal method As System.Reflection.MethodBase, ByVal ex As Exception)
        LogError(method, "Erreur: " & ex.Message)
    End Sub
    Private Sub LogError(ByVal method As System.Reflection.MethodBase, ByVal format As String, ByVal ParamArray args() As Object)
        LogError(method, String.Format(format, args))
    End Sub

    Private Sub LogDebug(ByVal method As System.Reflection.MethodBase, ByVal message As String)
        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & ":" & method.Name, message)
    End Sub
    Private Sub LogDebug(ByVal method As System.Reflection.MethodBase, ByVal format As String, ByVal ParamArray args() As Object)
        LogDebug(method, String.Format(format, args))
    End Sub

#End Region



End Class







