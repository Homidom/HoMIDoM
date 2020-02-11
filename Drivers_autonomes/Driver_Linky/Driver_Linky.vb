Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports System.Net
Imports System.Reflection
Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.Text.RegularExpressions
Imports System.IO
Imports System.Text
Imports Newtonsoft.Json.Linq
Imports System.Net.Http
'Imports System.Data.SQLite

<Serializable()> Public Class Driver_Linky
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "C68E7576-4724-11E8-A0D2-B54EF4E0CD91" 'ne pas modifier car utilisé dans le code du serveur
    Dim _Nom As String = "Linky" 'Nom du driver à afficher
    Dim _Enable As Boolean = False 'Activer/Désactiver le driver
    Dim _Description As String = "Driver Linky" 'Description du driver
    Dim _StartAuto As Boolean = False 'True si le driver doit démarrer automatiquement
    Dim _Protocol As String = "Http" 'Protocole utilisé par le driver, exemple: RS232
    Dim _IsConnect As Boolean = False 'True si le driver est connecté et sans erreur
    Dim _IP_TCP As String = "@" 'Adresse IP TCP à utiliser, "@" si non applicable pour le cacher côté client
    Dim _Port_TCP As String = "@" 'Port TCP à utiliser, "@" si non applicable pour le cacher côté client
    Dim _IP_UDP As String = "@" 'Adresse IP UDP à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Port_UDP As String = "@" 'Port UDP à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Com As String = "@" 'Port COM à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Refresh As Integer = 0 'Valeur à laquelle le driver doit rafraichir les valeurs des devices (ex: toutes les 200ms aller lire les devices)
    Dim _Modele As String = "@" 'Modèle du driver/interface
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


    Dim _LOGIN_BASE_URI As String = "https://espace-client-connexion.enedis.fr"
    Dim _API_BASE_URI = "https://espace-client-particuliers.enedis.fr/group/espace-particuliers"
    Dim _API_ENDPOINT_LOGIN = "/auth/UI/Login"
    Dim _API_ENDPOINT_HOME = "/accueil"
    Dim _API_ENDPOINT_DATA = "/suivi-de-consommation"

    Dim _UserNameList As String = ""
    Dim _PasswordList As String = ""

    Dim contrats As List(Of Contrat) = New List(Of Contrat)

    Dim sqlite_homidom As New HoMIDom.HoMIDom.Sqlite("homidom", Me.Server) 'BDD sqlite pour Homidom

    Dim TimerDataHeure As New Timers.Timer 'Timer de la fonction getdata si les datas heure du dernier jour ne sont pas encore transmises
    Dim _GetDataUrlLogin As String = ""
    Dim _GetDataUrlDatas As String = ""
    Dim _GetDataObjet As Object
    Dim _RefreshTimerDataHeure As Integer = 0

    Public Class Contrat
        Public Pdl As String
        Public adresse As String
        Public fournisseur As String
        Public PuissanceSouscrite As String
        Public User As String
        Public Password As String
    End Class

    Public Class Donnee
        Public etat As etat
        Public graphe As graphe
    End Class
    Public Class etat
        Public valeur As String
    End Class
    Public Class graphe
        Public decalage As Integer
        Public PuissanceSouscrite As Integer
        Public periode As periodes
        Public data As List(Of datas)
    End Class

    Public Class datas
        Public valeur As Double
        Public ordre As Integer
    End Class

    Public Class periodes
        Public datefin As String
        Public datedebut As String
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
                'Pas de commande demandée donc erreur
                If Command = "" Then
                    Return False
                Else
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
                _UserNameList = _Parametres.Item(1).Valeur
                _PasswordList = _Parametres.Item(2).Valeur
            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Start, Erreur dans les paramétres avancés. utilisation des valeur par défaut : " & ex.Message)
            End Try

            WriteLog("Start, connection au serveur " & _LOGIN_BASE_URI & _API_ENDPOINT_LOGIN)

            Get_ListContrat()

            If contrats.Count > 0 Then
                _IsConnect = True
                WriteLog("Driver démarré avec succés à l'adresse " & _LOGIN_BASE_URI)
            Else
                _IsConnect = False
                WriteLog("ERR: Driver non démarré à l'adresse " & _LOGIN_BASE_URI & _API_ENDPOINT_LOGIN)
            End If

            'Timer des datasheure si datas heure dernier jours non transmise par enedis
            ' recupere la liste des contrats ttes les 1h
            If _RefreshTimerDataHeure = 0 Then _RefreshTimerDataHeure = 3600
            TimerDataHeure.Interval = _RefreshTimerDataHeure * 1000
            TimerDataHeure.Enabled = False
            AddHandler TimerDataHeure.Elapsed, AddressOf TimerTickDataHeure

        Catch ex As Exception
            _IsConnect = False
            WriteLog("ERR: Driver Erreur démarrage " & ex.Message)
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
    ''' <param name="Objet">Objet représentant le device à interroger</param>
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

            Dim retourvaleur As Double = Get_Datas(_API_BASE_URI & _API_ENDPOINT_DATA, _API_BASE_URI & _API_ENDPOINT_DATA, Objet)
            Select Case True
                Case retourvaleur > 0   's'il y a eu des valeurs récupérées
                    If Objet.adresse2 <> "Heure" Then Objet.Value = Regex.Replace(CStr(retourvaleur), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                Case retourvaleur = -2 ' cas des données pas encore accessible
                        _GetDataUrlLogin = _API_BASE_URI & _API_ENDPOINT_DATA
                    _GetDataUrlDatas = _API_BASE_URI & _API_ENDPOINT_DATA
                    _GetDataObjet = Objet
                    If Not TimerDataHeure.Enabled Then TimerDataHeure.Enabled = True
            End Select
        Catch ex As Exception
            WriteLog("ERR: Read, adresse1 : " & Objet.adresse1 & " - adresse2 : " & Objet.adresse2)
            WriteLog("ERR: Read, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Commander un device</summary>
    ''' <param name="Objet">Objet représentant le device à commander</param>
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
            WriteLog("ERR: WRITE, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
    ''' <param name="DeviceId">Objet représentant le device à interroger</param>
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
            _DeviceSupport.Add(ListeDevices.ENERGIETOTALE)


            'Parametres avancés driver
            Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)
            Add_ParamAvance("Username", "Nom utilisateur", "admin")
            Add_ParamAvance("Password", "Mot de passe", "homi123456")

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'ajout des commandes avancées pour les devices
            'Add_DeviceCommande("ALL_LIGHT_ON", "", 0)

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Numéro du point de livraison", "Numéro de PDL")
            Add_LibelleDevice("ADRESSE2", "Choix du pas de temps", "Choix de la valeur")
            Add_LibelleDevice("REFRESH", "Refresh (sec)", "Valeur de rafraîchissement de la mesure en secondes")
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("MODELE", "@", "")

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

        Get_ListContrat()

    End Sub


#End Region

#Region "Fonctions internes"
    'Insérer ci-dessous les fonctions propres au driver

    Private Sub TimerTickDataHeure(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)
        If (_Enable = False) Or (_IsConnect = False) Then
            'arrete le timer du driver
            TimerDataHeure.Enabled = False
            Exit Sub
        End If

        Get_Datas(_GetDataUrlLogin, _GetDataUrlDatas, _GetDataObjet)
    End Sub

    Sub Get_ListContrat()
        'cree la liste des contrat dans le champ adresse1

        Try
            Dim stUN As String = _UserNameList
            Dim strUN As String = "----"
            Dim stPW As String = _PasswordList
            Dim strPW As String = "----"
            contrats.Clear() 'supprime tous les contrats de la liste

            While True
                'extrait le nom de user. Si plusieurs contrat séparés par |
                If InStr(stUN, "|") > 0 Then
                    strUN = Mid(stUN, 1, InStr(stUN, "|") - 1)
                Else
                    strUN = stUN
                End If
                If InStr(stPW, "|") > 0 Then
                    strPW = Mid(stPW, 1, InStr(stPW, "|") - 1)
                Else
                    strPW = stPW
                End If

                If Get_ListPdl(_LOGIN_BASE_URI & _API_ENDPOINT_LOGIN, strUN, strPW) Then

                    Dim IdAdr1 As String = ""
                    Dim j As Integer = 1
                    Dim IdAdr2 As String = ""
                    For Each detailcontract In contrats
                        WriteLog("Contrat trouvé : " & detailcontract.Pdl & " - " & detailcontract.adresse & ", Puis. sousc. : " & detailcontract.PuissanceSouscrite)
                        IdAdr1 += j & " # " & detailcontract.Pdl
                        IdAdr1 += "|"
                        IdAdr2 = j & " #; Heure|" & j & " #; Jour|" & j & " #; Mois|"
                        j += 1
                        If InStr(_UserNameList, "|") > 0 Then

                        Else
                            Exit For
                        End If
                    Next

                    ' evite les doublons 
                    Dim ld0 As New HoMIDom.HoMIDom.Driver.cLabels
                    For i As Integer = 0 To _LabelsDevice.Count - 1
                        ld0 = _LabelsDevice(i)
                        Select Case ld0.NomChamp
                            Case "ADRESSE1"
                                ld0.Parametre = IdAdr1
                                _LabelsDevice(i) = ld0
                            Case "ADRESSE2"
                                IdAdr2 = Mid(IdAdr2, 1, Len(IdAdr2) - 1) 'enleve le dernier | pour eviter davoir une ligne vide a la fin
                                ld0.Parametre = IdAdr2
                                _LabelsDevice(i) = ld0
                        End Select
                    Next
                End If
                If InStr(stUN, "|") = 0 Then
                    Exit While  ' sortie de la boucle quand tous les contrats traités
                Else
                    'retire le contrat qui vient d'être traité
                    stUN = Mid(stUN, InStr(stUN, "|") + 1, Len(stUN))
                    stPW = Mid(stPW, InStr(stPW, "|") + 1, Len(stUN))
                End If
            End While
        Catch ex As Exception
            WriteLog("ERR: " & "GET_Contrat, " & ex.Message)
        End Try

    End Sub
    Private Function Get_ListPdl(adrs As String, user As String, password As String) As Boolean
        ' recupere les configuration de Linky

        Try
            Dim responsebodystr As String = ""
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 'rajout pout TSL1.2 sinon plantage
            Dim request As HttpWebRequest
            Dim response As HttpWebResponse

            request = CType(WebRequest.Create(adrs & "?realm=particuliers"), HttpWebRequest)
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1500.72 Safari/537.36"
            request.AllowAutoRedirect = False
            request.Method = "GET"
            request.Host = Replace(_LOGIN_BASE_URI, "https://", "")
            request.KeepAlive = True
            request.CookieContainer = New CookieContainer

            response = request.GetResponse()
            Dim responsereader = New StreamReader(response.GetResponseStream())
            responsebodystr = responsereader.ReadToEnd()
            responsereader.Close()
            response.Close()
            Dim cookieToken As CookieContainer = request.CookieContainer
            '-------------------------------------------------------------------------------------------OK-----------------------------------------------------------------------------
            Dim reqparam As String = ""
            reqparam = "IDToken1=" & user & "&IDToken2=" & password & "&goto=aHR0cHM6Ly9lc3BhY2UtY2xpZW50LXBhcnRpY3VsaWVycy5lbmVkaXMuZnIv&gotoOnFail=&SunQueryParamsString=cmVhbG09cGFydGljdWxpZXJz&encoded=true&gx_charset=UTF-8"

            request = CType(WebRequest.Create(adrs), HttpWebRequest)
            request.ContentLength = reqparam.Length
            request.ContentType = "application/x-www-form-urlencoded"
            request.Method = "POST"
            request.KeepAlive = True
            request.Referer = adrs & "&goto=https://espace-client-particuliers.enedis.fr%2Fgroup%2Fespace-particuliers%2Faccueil"
            request.UserAgent = "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Mobile Safari/537.36"
            request.Accept = "application/json, text/javascript, */*; q=0.01"
            request.CookieContainer = cookieToken
            request.Headers.Add("Origin", "https://espace-client-connexion.enedis.fr")
            request.Headers.Add("Accept-Language", "fr,fr-FR;q=0.8,en;q=0.6")

            Dim postBytes As Byte() = Encoding.UTF8.GetBytes(reqparam)
            request.ContentLength = postBytes.Length
            Dim writer As New StreamWriter(request.GetRequestStream)
            writer.Write(reqparam)
            writer.Close()

            responsebodystr = ""

            response = request.GetResponse()
            responsereader = New StreamReader(response.GetResponseStream())
            responsebodystr = responsereader.ReadToEnd()
            responsereader.Close()
            response.Close()

            If InStr(UCase(responsebodystr), "MES INFOS") = 0 Then
                WriteLog("ERR: Get_ListPdl, contrat non trouvé avec le user/password -> " & user & "/" & password)
                Return False
            End If

            'recherche des infos contrat
            Dim st As String = Mid(responsebodystr, InStr(UCase(responsebodystr), "MES INFOS"), Len(responsebodystr))
            If Len(st) = 0 Then Return False

            ' extrait les caractéristiques du contrat
            Dim ctrt As New Contrat
            ctrt.PuissanceSouscrite = Mid(st, InStr(st, "kVA") - 3, 6)
            WriteLog("DBG: Get_ListPdl, contract.PuissanceSouscrite -> " & ctrt.PuissanceSouscrite)

            st = Mid(responsebodystr, InStr(UCase(responsebodystr), "PDL") - 1, 500)
            ctrt.Pdl = Mid(st, InStr(st, "PDL"), 18)
            WriteLog("DBG: Get_ListPdl, contract.PDL -> " & ctrt.Pdl)

            ctrt.adresse = Mid(st, InStr(st, "CtnVille"">") + 10, 50)
            ctrt.adresse = Mid(ctrt.adresse, 1, InStr(ctrt.adresse, "</p>") - 1)
            WriteLog("DBG: Get_ListPdl, contract.adresse -> " & ctrt.adresse)

            ctrt.User = user
            ctrt.Password = password

            'rajoute a la liste des contrats
            contrats.Add(ctrt)

            Return True

        Catch ex As Exception
            WriteLog("ERR: " & "GET_ListPdl, " & ex.Message)
            WriteLog("ERR: " & "GET_ListPdl, Url: " & adrs)
            Return False
        End Try
    End Function

    Function Get_Datas(adrslogin As String, adrsdatas As String, ByVal objet As Object)
        Try
            Dim responsebodystr As String = ""
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 'rajout pout TSL1.2 sinon plantage
            Dim request As HttpWebRequest
            Dim response As HttpWebResponse
            Dim user As String = ""
            Dim password As String = ""
            Dim pdl As String = Trim(Mid(objet.adresse1, InStr(objet.adresse1, "#") + 1, Len(objet.adresse1)))
            Dim typevaleur As String = objet.adresse2

            adrslogin = _LOGIN_BASE_URI & _API_ENDPOINT_LOGIN

            request = CType(WebRequest.Create(adrslogin & "?realm=particuliers"), HttpWebRequest)
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1500.72 Safari/537.36"
            request.AllowAutoRedirect = False
            request.Method = "GET"
            request.Host = Replace(_LOGIN_BASE_URI, "https://", "")
            request.KeepAlive = True
            request.CookieContainer = New CookieContainer

            response = request.GetResponse()
            Dim responsereader = New StreamReader(response.GetResponseStream())
            responsebodystr = responsereader.ReadToEnd()
            responsereader.Close()
            response.Close()
            Dim cookieToken As CookieContainer = request.CookieContainer
            '------------------------------------------------------------------connection login-----------------------------------------------------------
            For i = 0 To contrats.Count - 1
                If contrats.Item(i).Pdl = pdl Then
                    user = contrats.Item(i).User
                    password = contrats.Item(i).Password
                End If
            Next
            Dim reqparam As String = "IDToken1=" & user & "&IDToken2=" & password & "&goto=aHR0cHM6Ly9lc3BhY2UtY2xpZW50LXBhcnRpY3VsaWVycy5lbmVkaXMuZnIv&gotoOnFail=&SunQueryParamsString=cmVhbG09cGFydGljdWxpZXJz&encoded=true&gx_charset=UTF-8"

            request = CType(WebRequest.Create(adrslogin), HttpWebRequest)
            request.ContentLength = reqparam.Length
            request.ContentType = "application/x-www-form-urlencoded"
            request.Method = "POST"
            request.KeepAlive = True
            request.Referer = adrslogin & "&goto=https://espace-client-particuliers.enedis.fr%2Fgroup%2Fespace-particuliers%2Faccueil"
            request.UserAgent = "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Mobile Safari/537.36"
            request.Accept = "application/json, text/javascript, */*; q=0.01"
            request.CookieContainer = cookieToken
            request.Headers.Add("Origin", "https://espace-client-connexion.enedis.fr")
            request.Headers.Add("Accept-Language", "fr,fr-FR;q=0.8,en;q=0.6")

            Dim postBytes As Byte() = Encoding.UTF8.GetBytes(reqparam)
            request.ContentLength = postBytes.Length
            Dim writer As New StreamWriter(request.GetRequestStream)
            writer.Write(reqparam)
            writer.Close()

            responsebodystr = ""
            response = request.GetResponse()
            responsereader = New StreamReader(response.GetResponseStream())
            responsebodystr = responsereader.ReadToEnd()
            responsereader.Close()
            response.Close()
            '-------------------------------------------------------------------------connection data----------------------------------------------------------------------------
            cookieToken = request.CookieContainer

            adrsdatas = _API_BASE_URI & _API_ENDPOINT_DATA

            'preparation de la requete https
            Dim datejour As Date = Now
            Dim datefin As String = datejour.ToString("dd/MM/yyyy")
            Dim datedebut As String = ""
            If typevaleur = "" Then typevaleur = "Jour"
            Select Case typevaleur
                Case "Heure" : adrsdatas = adrsdatas & "?p_p_id=lincspartdisplaycdc_WAR_lincspartcdcportlet&p_p_lifecycle=2&p_p_state=normal&p_p_mode=view&p_p_resource_id=urlCdcHeure&p_p_cacheability=cacheLevelPage&p_p_col_id=column-1&p_p_col_count=2"
                    datedebut = datejour.AddDays(-3).ToString("dd/MM/yyyy")
                Case "Jour" : adrsdatas = adrsdatas & "?p_p_id=lincspartdisplaycdc_WAR_lincspartcdcportlet&p_p_lifecycle=2&p_p_state=normal&p_p_mode=view&p_p_resource_id=urlCdcJour&p_p_cacheability=cacheLevelPage&p_p_col_id=column-1&p_p_col_count=2"
                    datedebut = datejour.AddDays(-31).ToString("dd/MM/yyyy")
                Case "Mois" : adrsdatas = adrsdatas & "?p_p_id=lincspartdisplaycdc_WAR_lincspartcdcportlet&p_p_lifecycle=2&p_p_state=normal&p_p_mode=view&p_p_resource_id=urlCdcMois&p_p_cacheability=cacheLevelPage&p_p_col_id=column-1&p_p_col_count=2"
                    datedebut = datejour.AddDays(-360).ToString("dd/MM/yyyy")
            End Select

            reqparam = "_lincspartdisplaycdc_WAR_lincspartcdcportlet_dateDebut=" & HttpUtility.UrlEncode(datedebut) & "&_lincspartdisplaycdc_WAR_lincspartcdcportlet_dateFin=" & HttpUtility.UrlEncode(datefin)

            request = CType(WebRequest.Create(adrsdatas), HttpWebRequest)
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8"
            request.Method = "POST"
            request.KeepAlive = True
            request.Referer = "https://espace-client-particuliers.enedis.fr/group/espace-particuliers/suivi-de-consommation"
            request.AllowAutoRedirect = False
            request.Host = "espace-client-particuliers.enedis.fr"
            request.CookieContainer = cookieToken
            request.Headers.Add("Origin", "https://espace-client-connexion.enedis.fr")

            postBytes = Encoding.UTF8.GetBytes(reqparam)
            request.ContentLength = postBytes.Length
            writer = New StreamWriter(request.GetRequestStream)
            writer.Write(reqparam)
            writer.Close()

            responsebodystr = ""
            response = request.GetResponse()
            responsereader = New StreamReader(response.GetResponseStream())
            responsebodystr = responsereader.ReadToEnd()
            responsereader.Close()
            response.Close()
            '     responsebodystr = "{""etat"":  {""valeur"":""termine""},""graphe"":{""decalage"":0,""puissanceSouscrite"":12,""periode"":{""dateFin"":""25/12/2019"",""dateDebut"":""22/12/2019""},""data"":[{""valeur"":0.574,""ordre"":1},{""valeur"":0.356,""ordre"":2},{""valeur"":2.924,""ordre"":3},{""valeur"":0.35,""ordre"":4},{""valeur"":1.542,""ordre"":5},{""valeur"":0.298,""ordre"":6},{""valeur"":0.396,""ordre"":7},{""valeur"":0.622,""ordre"":8},{""valeur"":0.294,""ordre"":9},{""valeur"":0.59,""ordre"":10},{""valeur"":0.342,""ordre"":11},{""valeur"":0.674,""ordre"":12},{""valeur"":0.362,""ordre"":13},{""valeur"":0.71,""ordre"":14},{""valeur"":3.518,""ordre"":15},{""valeur"":2.818,""ordre"":16},{""valeur"":3.214,""ordre"":17},{""valeur"":3.118,""ordre"":18},{""valeur"":3.216,""ordre"":19},{""valeur"":3.088,""ordre"":20},{""valeur"":3.994,""ordre"":21},{""valeur"":3.164,""ordre"":22},{""valeur"":4.022,""ordre"":23},{""valeur"":3.368,""ordre"":24},{""valeur"":4.374,""ordre"":25},{""valeur"":3.472,""ordre"":26},{""valeur"":4.116,""ordre"":27},{""valeur"":3.294,""ordre"":28},{""valeur"":3.73,""ordre"":29},{""valeur"":3.302,""ordre"":30},{""valeur"":3.37,""ordre"":31},{""valeur"":3.438,""ordre"":32},{""valeur"":3.184,""ordre"":33},{""valeur"":3.598,""ordre"":34},{""valeur"":3.03,""ordre"":35},{""valeur"":3.38,""ordre"":36},{""valeur"":3.298,""ordre"":37},{""valeur"":2.916,""ordre"":38},{""valeur"":3.538,""ordre"":39},{""valeur"":2.698,""ordre"":40},{""valeur"":3.754,""ordre"":41},{""valeur"":2.76,""ordre"":42},{""valeur"":3.336,""ordre"":43},{""valeur"":3.218,""ordre"":44},{""valeur"":0.668,""ordre"":45},{""valeur"":0.684,""ordre"":46},{""valeur"":0.438,""ordre"":47},{""valeur"":0.72,""ordre"":48},{""valeur"":0.344,""ordre"":49},{""valeur"":0.86,""ordre"":50},{""valeur"":0.548,""ordre"":51},{""valeur"":0.754,""ordre"":52},{""valeur"":0.57,""ordre"":53},{""valeur"":0.942,""ordre"":54},{""valeur"":0.528,""ordre"":55},{""valeur"":1.002,""ordre"":56},{""valeur"":0.672,""ordre"":57},{""valeur"":0.986,""ordre"":58},{""valeur"":1.152,""ordre"":59},{""valeur"":4.204,""ordre"":60},{""valeur"":3.374,""ordre"":61},{""valeur"":4.494,""ordre"":62},{""valeur"":0.848,""ordre"":63},{""valeur"":1.104,""ordre"":64},{""valeur"":0.742,""ordre"":65},{""valeur"":1.268,""ordre"":66},{""valeur"":0.692,""ordre"":67},{""valeur"":0.966,""ordre"":68},{""valeur"":0.898,""ordre"":69},{""valeur"":1.03,""ordre"":70},{""valeur"":0.738,""ordre"":71},{""valeur"":1.562,""ordre"":72},{""valeur"":0.706,""ordre"":73},{""valeur"":1.136,""ordre"":74},{""valeur"":0.834,""ordre"":75},{""valeur"":0.914,""ordre"":76},{""valeur"":1.244,""ordre"":77},{""valeur"":0.88,""ordre"":78},{""valeur"":0.736,""ordre"":79},{""valeur"":0.592,""ordre"":80},{""valeur"":0.814,""ordre"":81},{""valeur"":4.076,""ordre"":82},{""valeur"":3.502,""ordre"":83},{""valeur"":4.13,""ordre"":84},{""valeur"":3.832,""ordre"":85},{""valeur"":4.076,""ordre"":86},{""valeur"":4.132,""ordre"":87},{""valeur"":4.258,""ordre"":88},{""valeur"":4.014,""ordre"":89},{""valeur"":4.134,""ordre"":90},{""valeur"":3.652,""ordre"":91},{""valeur"":3.946,""ordre"":92},{""valeur"":1.098,""ordre"":93},{""valeur"":0.778,""ordre"":94},{""valeur"":0.754,""ordre"":95},{""valeur"":0.456,""ordre"":96},{""valeur"":0.464,""ordre"":97},{""valeur"":0.552,""ordre"":98},{""valeur"":0.394,""ordre"":99},{""valeur"":0.684,""ordre"":100},{""valeur"":0.566,""ordre"":101},{""valeur"":0.732,""ordre"":102},{""valeur"":0.382,""ordre"":103},{""valeur"":0.764,""ordre"":104},{""valeur"":0.652,""ordre"":105},{""valeur"":0.802,""ordre"":106},{""valeur"":0.452,""ordre"":107},{""valeur"":4.214,""ordre"":108},{""valeur"":2.862,""ordre"":109},{""valeur"":4.196,""ordre"":110},{""valeur"":0.65,""ordre"":111},{""valeur"":1.238,""ordre"":112},{""valeur"":0.918,""ordre"":113},{""valeur"":1.2,""ordre"":114},{""valeur"":1.142,""ordre"":115},{""valeur"":0.756,""ordre"":116},{""valeur"":0.572,""ordre"":117},{""valeur"":0.672,""ordre"":118},{""valeur"":0.568,""ordre"":119},{""valeur"":0.938,""ordre"":120},{""valeur"":0.636,""ordre"":121},{""valeur"":0.874,""ordre"":122},{""valeur"":0.768,""ordre"":123},{""valeur"":1.246,""ordre"":124},{""valeur"":0.572,""ordre"":125},{""valeur"":0.964,""ordre"":126},{""valeur"":0.822,""ordre"":127},{""valeur"":0.98,""ordre"":128},{""valeur"":0.652,""ordre"":129},{""valeur"":4.494,""ordre"":130},{""valeur"":3.222,""ordre"":131},{""valeur"":4.436,""ordre"":132},{""valeur"":3.524,""ordre"":133},{""valeur"":4.212,""ordre"":134},{""valeur"":3.676,""ordre"":135},{""valeur"":4.116,""ordre"":136},{""valeur"":3.722,""ordre"":137},{""valeur"":3.992,""ordre"":138},{""valeur"":3.462,""ordre"":139},{""valeur"":3.794,""ordre"":140},{""valeur"":0.944,""ordre"":141},{""valeur"":0.564,""ordre"":142},{""valeur"":0.418,""ordre"":143},{""valeur"":-2,""ordre"":144}]}}"
            '     responsebodystr = "{""etat"":  {""valeur"":""termine""},""graphe"":{""decalage"":0,""puissanceSouscrite"":12,""periode"":{""dateFin"":""25/12/2019"",""dateDebut"":""22/12/2019""},""data"":[{""valeur"":0.574,""ordre"":1},{""valeur"":0.356,""ordre"":2},{""valeur"":2.924,""ordre"":3},{""valeur"":0.35,""ordre"":4},{""valeur"":1.542,""ordre"":5},{""valeur"":0.298,""ordre"":6},{""valeur"":0.396,""ordre"":7},{""valeur"":0.622,""ordre"":8},{""valeur"":0.294,""ordre"":9},{""valeur"":0.59,""ordre"":10},{""valeur"":0.342,""ordre"":11},{""valeur"":0.674,""ordre"":12},{""valeur"":0.362,""ordre"":13},{""valeur"":0.71,""ordre"":14},{""valeur"":3.518,""ordre"":15},{""valeur"":2.818,""ordre"":16},{""valeur"":3.214,""ordre"":17},{""valeur"":3.118,""ordre"":18},{""valeur"":3.216,""ordre"":19},{""valeur"":3.088,""ordre"":20},{""valeur"":3.994,""ordre"":21},{""valeur"":3.164,""ordre"":22},{""valeur"":4.022,""ordre"":23},{""valeur"":3.368,""ordre"":24},{""valeur"":4.374,""ordre"":25},{""valeur"":3.472,""ordre"":26},{""valeur"":4.116,""ordre"":27},{""valeur"":3.294,""ordre"":28},{""valeur"":3.73,""ordre"":29},{""valeur"":3.302,""ordre"":30},{""valeur"":3.37,""ordre"":31},{""valeur"":3.438,""ordre"":32},{""valeur"":3.184,""ordre"":33},{""valeur"":3.598,""ordre"":34},{""valeur"":3.03,""ordre"":35},{""valeur"":3.38,""ordre"":36},{""valeur"":3.298,""ordre"":37},{""valeur"":2.916,""ordre"":38},{""valeur"":3.538,""ordre"":39},{""valeur"":2.698,""ordre"":40},{""valeur"":3.754,""ordre"":41},{""valeur"":2.76,""ordre"":42},{""valeur"":3.336,""ordre"":43},{""valeur"":3.218,""ordre"":44},{""valeur"":0.668,""ordre"":45},{""valeur"":0.684,""ordre"":46},{""valeur"":0.438,""ordre"":47},{""valeur"":0.72,""ordre"":48},{""valeur"":0.344,""ordre"":49},{""valeur"":0.86,""ordre"":50},{""valeur"":0.548,""ordre"":51},{""valeur"":0.754,""ordre"":52},{""valeur"":0.57,""ordre"":53},{""valeur"":0.942,""ordre"":54},{""valeur"":0.528,""ordre"":55},{""valeur"":1.002,""ordre"":56},{""valeur"":0.672,""ordre"":57},{""valeur"":0.986,""ordre"":58},{""valeur"":1.152,""ordre"":59},{""valeur"":4.204,""ordre"":60},{""valeur"":3.374,""ordre"":61},{""valeur"":4.494,""ordre"":62},{""valeur"":0.848,""ordre"":63},{""valeur"":1.104,""ordre"":64},{""valeur"":0.742,""ordre"":65},{""valeur"":1.268,""ordre"":66},{""valeur"":0.692,""ordre"":67},{""valeur"":0.966,""ordre"":68},{""valeur"":0.898,""ordre"":69},{""valeur"":1.03,""ordre"":70},{""valeur"":0.738,""ordre"":71},{""valeur"":1.562,""ordre"":72},{""valeur"":0.706,""ordre"":73},{""valeur"":1.136,""ordre"":74},{""valeur"":0.834,""ordre"":75},{""valeur"":0.914,""ordre"":76},{""valeur"":1.244,""ordre"":77},{""valeur"":0.88,""ordre"":78},{""valeur"":0.736,""ordre"":79},{""valeur"":0.592,""ordre"":80},{""valeur"":0.814,""ordre"":81},{""valeur"":4.076,""ordre"":82},{""valeur"":3.502,""ordre"":83},{""valeur"":4.13,""ordre"":84},{""valeur"":3.832,""ordre"":85},{""valeur"":4.076,""ordre"":86},{""valeur"":4.132,""ordre"":87},{""valeur"":4.258,""ordre"":88},{""valeur"":4.014,""ordre"":89},{""valeur"":4.134,""ordre"":90},{""valeur"":3.652,""ordre"":91},{""valeur"":3.946,""ordre"":92},{""valeur"":1.098,""ordre"":93},{""valeur"":0.778,""ordre"":94},{""valeur"":0.754,""ordre"":95},{""valeur"":0.456,""ordre"":96},{""valeur"":0.464,""ordre"":97},{""valeur"":0.552,""ordre"":98},{""valeur"":0.394,""ordre"":99},{""valeur"":0.684,""ordre"":100},{""valeur"":0.566,""ordre"":101},{""valeur"":0.732,""ordre"":102},{""valeur"":0.382,""ordre"":103},{""valeur"":0.764,""ordre"":104},{""valeur"":0.652,""ordre"":105},{""valeur"":0.802,""ordre"":106},{""valeur"":0.452,""ordre"":107},{""valeur"":4.214,""ordre"":108},{""valeur"":2.862,""ordre"":109},{""valeur"":4.196,""ordre"":110},{""valeur"":0.65,""ordre"":111},{""valeur"":1.238,""ordre"":112},{""valeur"":0.918,""ordre"":113},{""valeur"":1.2,""ordre"":114},{""valeur"":1.142,""ordre"":115},{""valeur"":0.756,""ordre"":116},{""valeur"":0.572,""ordre"":117},{""valeur"":0.672,""ordre"":118},{""valeur"":0.568,""ordre"":119},{""valeur"":0.938,""ordre"":120},{""valeur"":0.636,""ordre"":121},{""valeur"":0.874,""ordre"":122},{""valeur"":0.768,""ordre"":123},{""valeur"":1.246,""ordre"":124},{""valeur"":0.572,""ordre"":125},{""valeur"":0.964,""ordre"":126},{""valeur"":0.822,""ordre"":127},{""valeur"":0.98,""ordre"":128},{""valeur"":0.652,""ordre"":129},{""valeur"":4.494,""ordre"":130},{""valeur"":3.222,""ordre"":131},{""valeur"":4.436,""ordre"":132},{""valeur"":3.524,""ordre"":133},{""valeur"":4.212,""ordre"":134},{""valeur"":3.676,""ordre"":135},{""valeur"":4.116,""ordre"":136},{""valeur"":3.722,""ordre"":137},{""valeur"":3.992,""ordre"":138},{""valeur"":3.462,""ordre"":139},{""valeur"":3.794,""ordre"":140},{""valeur"":0.944,""ordre"":141},{""valeur"":0.564,""ordre"":142},{""valeur"":0.418,""ordre"":143},{""valeur"":0.416,""ordre"":144}]}}"
            WriteLog("DBG: Get_Datas, responsebody " & pdl & " -> " & responsebodystr)

            'mise en forme de la chaine récupérée
            Dim donnees As Donnee = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebodystr, GetType(Donnee))

            If typevaleur <> "Heure" Then
                Return donnees.graphe.data.Item(donnees.graphe.data.Count - 1).valeur
            Else
                If donnees.graphe.data.Item(donnees.graphe.data.Count - 1).valeur = -2 Then  'cas des enregistrements du dernier jour non accessible sur le site
                    WriteLog("Pas de donnée horaire actuellement disponible pour le " & donnees.graphe.periode.datefin)
                    Return -2
                Else
                    WriteLog("DBG: Get_Datas, nbre de données horaire -> " & donnees.graphe.data.Count)
                    Return InsertDataJourHeure(objet, donnees)
                End If
            End If

        Catch ex As Exception
            WriteLog("ERR: " & "GET_Datas, " & ex.Message)
            Return 0
        End Try

    End Function

    Private Function InsertDataJourHeure(ByVal objet As Object, datajson As Donnee)
        ' permet de récuperer les données de la journée précédente sur le pas de temps de l'heure. Il faut avoir autoriser ce type de données sur le site d'enedis
        'on ne prends que la dernière journée

        Try
            'recherche de l'id du device
            Dim id As String = ""
            Dim ListeDevices = _Server.GetAllDevices(_IdSrv)
            For Each _dev In ListeDevices
                If objet.name = _dev.Name Then
                    id = _dev.ID
                End If
            Next

            'suppression des datas du jour
            Dim datejour As Date = datajson.graphe.periode.datedebut & " 00:00:00"
            Dim dateQ As DateTime = datejour
            dateQ = DateAdd("d", 2, dateQ)
            Dim QValues As String = "DELETE FROM historiques WHERE (device_id=""" & id & """) AND (dateheure='" & Format(dateQ, "yyyy-MM-dd") & "'); "
            Dim Tmpvalues As String = ""
            Dim nbrededata As Integer = datajson.graphe.data.Count / 3
            Dim nbredatainser As Integer = 0
            Dim sommejour As Double = 0
            Dim diviseurvaleur As Double = 72 / datajson.graphe.data.Count 'on divise la valeur par deux pour être cohérent avec conso jour, soit heure ou demi-heure
            For i = datajson.graphe.data.Count - nbrededata To datajson.graphe.data.Count - 1
                If datajson.graphe.data(i).valeur > 0 Then
                    Tmpvalues = " VALUES (""" & id & """," & """Value""," & """" & Format(dateQ, "yyyy-MM-dd HH:mm:ss") & """,""" & (datajson.graphe.data(i).valeur * diviseurvaleur) & """)"
                    QValues = QValues & "INSERT INTO historiques (device_id,source, dateheure, valeur)" & Tmpvalues & "; "
                    nbredatainser += 1
                    sommejour += (datajson.graphe.data(i).valeur * diviseurvaleur)
                End If
                dateQ = DateAdd("n", 30, dateQ)
            Next

            If nbredatainser > 0 Then
                Dim result As New DataTable
                Dim retourQuery As String = sqlite_homidom.query("Begin; " & QValues & "Commit;", result)
                If Mid(retourQuery, 1, 4) <> "ERR:" Then
                    WriteLog(nbredatainser & " valeurs insérées pour la journée du " & Format(DateAdd("d", 2, datejour), "dd/MM/yyyy"))
                    TimerDataHeure.Enabled = False ' pour etre sur d'arreter le timer
                    WriteLog("Total conso horaire de la journée du " & Format(DateAdd("d", 2, datejour), "dd/MM/yyyy") & "  -> " & sommejour)
                    Return sommejour
                Else
                    WriteLog("ERR: InsertDataJourHeure, Pas de valeur insérée pour la journée du " & Format(DateAdd("d", 2, datejour), "dd/MM/yyyy"))
                    WriteLog("ERR: InsertDataJourHeure, Erreur sur la requete d'insertion")
                    WriteLog("DBG: " & "InsertDataJourHeure, retourQuery -> " & retourQuery)
                    Return 0
                End If
            Else
                WriteLog("ERR: InsertDataJourHeure, Pas de valeur à insérer pour la journée du " & Format(DateAdd("d", 2, datejour), "dd/MM/yyyy"))
                WriteLog("DBG: InsertDataJourHeure, QValues -> " & QValues)
                Return 0
            End If
        Catch ex As Exception
            WriteLog("ERR: InsertDataJourHeure, " & ex.Message)
            Return 0
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
