Imports HoMIDom
Imports HoMIDom.HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports System.Net.Mail
Imports S22.Imap ' http://smiley22.github.io/S22.Imap/Documentation/
Imports S22.Pop3 ' http://smiley22.github.io/S22.Pop3/Documentation/ 

' Auteur : Mathieu complété jphomi POP3 1/3/2015
' Date : 02/03/2015

''' <summary>Class Driver_Mail</summary>
''' <remarks></remarks>
<Serializable()> Public Class Driver_Mail
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "44AAF056-52F6-11E3-8B13-BA266288709B" 'ne pas modifier car utilisé dans le code du serveur
    Dim _Nom As String = "Mail" 'Nom du driver à afficher
    Dim _Enable As Boolean = False 'Activer/Désactiver le driver
    Dim _Description As String = "Driver pour Mail : Infos, Commande" 'Description du driver
    Dim _StartAuto As Boolean = False 'True si le driver doit démarrer automatiquement
    Dim _Protocol As String = "HTTP" 'Protocole utilisé par le driver, exemple: RS232
    Dim _IsConnect As Boolean = False 'True si le driver est connecté et sans erreur
    Dim _IP_TCP As String = "@" 'Adresse IP TCP à utiliser, "@" si non applicable pour le cacher côté client
    Dim _Port_TCP As String = "@" 'Port TCP à utiliser, "@" si non applicable pour le cacher côté client
    Dim _IP_UDP As String = "@" 'Adresse IP UDP à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Port_UDP As String = "@" 'Port UDP à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Com As String = "@" 'Port COM à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Refresh As Integer = 0 'Valeur à laquelle le driver doit rafraichir les valeurs des devices (ex: toutes les 200ms aller lire les devices)
    Dim _Modele As String = "Mail" 'Modèle du driver/interface
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
#End Region

#Region "Variables Internes"
    'Insérer ici les variables internes propres au driver et non communes

    Dim _PopHost As String
    Dim _UserName As String
    Dim _Password As String
    Dim _PortNm As String
    Dim _Protocole As String
    Dim DmdStart As Boolean
    Dim boxmail As MailBoxHandler
	
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
                    Write(MyDevice, Command, Param(0))
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

    ''' <summary>Démarrer le driver</summary>
    ''' <remarks></remarks>
    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        Try
            _DEBUG = _Parametres.Item(0).Valeur
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "Erreur dans les paramétres avancés. utilisation des valeur par défaut" & ex.Message)
        End Try

        Try
            _DEBUG = _Parametres.Item(0).Valeur
            _PopHost = _Parametres.Item(1).Valeur
            _UserName = _Parametres.Item(2).Valeur
            _Password = _Parametres.Item(3).Valeur
            _PortNm = _Parametres.Item(4).Valeur
            _Protocole = _Parametres.Item(5).Valeur

            'Verification POP3 ou IMAP
            If _Protocole.ToUpper <> "POP3" Then _Protocole = "IMAP" Else _Protocole = "POP3"

            'Vérifie si Mail est démarrer si non tentative de de connexion  
            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " POP/IMAP", "Demande login : " & _PopHost & ":" & _PortNm)
            boxmail = New MailBoxHandler(_PopHost, _PortNm, _UserName, _Password, _Protocole)
            _IsConnect = boxmail.IsConnected
            boxmail.Dispose()
            If _IsConnect Then
                If _Refresh > 0 Then
                    Me.MyTimer.Interval = _Refresh * 1000
                    Me.MyTimer.Enabled = True
                    AddHandler Me.MyTimer.Elapsed, AddressOf Me.TimerTick
                End If
                DmdStart = True
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Mail Start", "Demarré")
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", ex.Message)
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            MyTimer.Stop()

            RemoveHandler Me.MyTimer.Elapsed, AddressOf Me.TimerTick
            DmdStart = False
            _IsConnect = False
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Stop", "Driver " & Me.Nom & " arrêté")
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Stop", ex.Message)
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
            If _Enable = False Then Exit Sub

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", ex.Message)
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
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write", "Erreur: Impossible de traiter la commande car le driver n'est pas activé (Enable)")
                Exit Sub
            End If

            If _IsConnect = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write", "Erreur: Impossible de traiter la commande car le driver n'est pas connecté à la carte")
                Exit Sub
            End If

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " DeleteDevice", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " NewDevice", ex.Message)
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

            'liste des devices compatibles
            _DeviceSupport.Add(ListeDevices.GENERIQUESTRING)

            'Parametres avancés
            Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)
            Add_ParamAvance("Serveur Mail", "Adresse serveur de votre boite de mail", "imap.gmail.com")
            Add_ParamAvance("Utilisateur", "Utilisateur du compte mail", "")
            Add_ParamAvance("Mot de passe", "Mot de passe du compte mail", "")
            Add_ParamAvance("Numéro du port", "Numéro du port du compte mail", "993")
            Add_ParamAvance("Protocole", "Protocole IMAP ou POP3", "IMAP")

            'ajout des commandes avancées pour les devices
            'add_devicecommande("COMMANDE", "DESCRIPTION", nbparametre)
            Add_DeviceCommande("RECEIVE", "Recevoir un Message", 1)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Element à recuperer", "Information à retourner au format -> 'adresse expediteur autorisé','champ à récupérer' ce qui fait xxx@yyy.com,Objet ou Texte")
            Add_LibelleDevice("ADRESSE2", "Texte à rechercher", "Mettre <texte avant><texte après> pour extraire un texte entre deux autres")
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("REFRESH", "Refresh", "")
            'Add_LibelleDevice("LASTCHANGEDUREE", "LastChange Durée", "")

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " New", ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs)
        Try
            If DmdStart = True Then
                boxmail = New MailBoxHandler(_PopHost, _PortNm, _UserName, _Password, _Protocole)
                AddHandler boxmail.HandleMessage, AddressOf receiveMsg
                Dim checkmail = boxmail.CheckExistingEmails()
                If checkmail <> "ok" Then
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Replication", checkmail)
                End If
                RemoveHandler boxmail.HandleMessage, AddressOf receiveMsg
                boxmail.Dispose()
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Replication", ex.Message)
        End Try
    End Sub

#End Region

#Region "Fonctions internes"
    'Insérer ci-dessous les fonctions propres au driver et nom communes (ex: start)

    Public Sub receiveMsg(ByVal sender As Object, ByVal e As HandleMessageEventArgs)
        Try
            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "Reception Message Mail : ", "Recu de : " & e.Message.From.Address & " avec l'objet: " & e.Message.Subject & " et le texte: " & e.Message.Body)
            'Parcourt tous les evenements 
            If e.Message.Subject.ToUpper = UCase("Commande HoMIDoM") Then
                CommandeString(e.Message.Body)
            Else
                'Recherche si un device affecté
                Dim listedevices As New ArrayList
                Dim txt As String = ""
                listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, "", "", Me._ID, True)
                'un device trouvé on maj la value
                If (listedevices.Count > 0) Then
                    For Each objet As Object In listedevices
                        If InStr(objet.adresse1, ",") Then
                            Dim ParaAdr1 = Split(objet.adresse1, ",")

                            If InStr(objet.adresse2, "><") Then
                                Dim ParaAdr2 = Split(objet.adresse2, "><")
                                Dim Avant = Right(ParaAdr2(0), ParaAdr2(0).Length - 1)
                                Dim Apres = Left(ParaAdr2(1), ParaAdr2(1).Length - 1)

                                If ParaAdr1(0) = e.Message.From.Address Then

                                    Select Case UCase(ParaAdr1(1))
                                        Case "OBJET"
                                            txt = e.Message.Subject
                                        Case "TEXTE"
                                            txt = e.Message.Body
                                        Case Else
                                            Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Reception Message Mail : ", "'Texte à rechercher' doit etre au format 'xxx@yyy.com,Objet' ou 'xxx@yyy.com,Texte' avec Texte ou Objet uniquement")
                                    End Select

                                    If InStr(txt, Avant) Or InStr(txt, Apres) Then

                                        If InStr(txt, Avant) Then
                                            txt = Right(txt, txt.Length - InStr(txt, Avant) - Avant.Length + 1)
                                        End If
                                        If InStr(txt, Apres) Then
                                            txt = Left(txt, InStr(txt, Apres) - 1)
                                        End If
                                        objet.value = txt
                                    Else
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "Reception Message Mail : ", "Le texte cherché n'a pas été trouvé dans le message : " & txt)
                                    End If
                                Else
                                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "Reception Message Mail : ", "L'expediteur n'est pas celui recherché : " & e.Message.From.Address)
                                End If
                            Else
                                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Reception Message Mail : ", "'Element à recuperer' doit etre au format <texte avant><texte après> et non comme ceci : " & txt)
                            End If
                        Else
                            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Reception Message Mail : ", "'Texte à rechercher' doit etre au format 'xxx@yyy.com,Objet' ou 'xxx@yyy.com,Texte' avec xxx@yyy.com étant l'expediteur authorisé en on comme ceci : " & txt)
                        End If
                    Next
                Else
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Reception Message Mail", "Composant non trouvé : " & e.Message.Subject)
                End If
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Mail Message Reçu", ex.Message)
        End Try
    End Sub

    Private Sub CommandeString(ByVal msg As String)
        Try
            Dim ParaAdr2 = Split(msg, ":")
            Dim IDCmd As String

            ParaAdr2(0) = Replace(ParaAdr2(0), vbCrLf, "")
            ParaAdr2(1) = Replace(ParaAdr2(1), vbCrLf, "")
            ParaAdr2(2) = Replace(ParaAdr2(2), vbCrLf, "")

            '           _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Traitement Commande", "ID/nom macro : " & ParaAdr2(1))

            Select Case ParaAdr2(0).ToUpper
                Case "COMPOSANT" 'If _DEBUG Then
                    If InStr(ParaAdr2(1), "-") = 0 Then  'teste si cest id device qui est passé ou son nom
                        IDCmd = ReturnDeviceIDByName(ParaAdr2(1))
                    Else
                        IDCmd = ParaAdr2(1)
                    End If
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Reception Message Commande", "Passage par la partie composant d'ID : " & IDCmd & " L'action     est : " & ParaAdr2(2))
                    Dim TempDevice As TemplateDevice = _Server.ReturnDeviceById(_IdSrv, IDCmd)
                    If TempDevice IsNot Nothing Then
                        If TempDevice.Type = ListeDevices.LAMPE Or TempDevice.Type = ListeDevices.APPAREIL Then

                            ' Analyse de la commande 
                            Select Case ParaAdr2(2).ToUpper
                                Case "ON", "OFF"
                                    Dim x As DeviceAction = New DeviceAction
                                    x.Nom = ParaAdr2(2)
                                    _Server.ExecuteDeviceCommand(_IdSrv, IDCmd, x)

                                Case "DIM"
                                    Dim x As DeviceActionSimple = New DeviceActionSimple
                                    x.Nom = ParaAdr2(2)
                                    x.Param1 = ParaAdr2(3)
                                    _Server.ExecuteDeviceCommandSimple(_IdSrv, IDCmd, x)

                                Case "READ"


                            End Select
                        End If
                    End If

                Case "MACRO"
                    If InStr(ParaAdr2(1), "-") = 0 Then  'teste si cest 'id  macro qui est passé ou son nom
                        IDCmd = ReturnMacroIDByName(ParaAdr2(1))
                    Else
                        IDCmd = ParaAdr2(1)
                    End If
                    If ID = "" Then
                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Reception Message Commande", "ID macro inconnu : " & ParaAdr2(1))
                        Exit Sub
                    End If

                    Dim TempMacro As Macro = _Server.ReturnMacroById(_IdSrv, IDCmd)
                    If TempMacro IsNot Nothing And TempMacro.Enable = True Then
                        ' Analyse de la commande 
                        '                       _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Reception Message Commande", "Execution macro d'ID : " & IDCmd)
                        Select Case ParaAdr2(2).ToUpper
                            Case "START"
                                '                               _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Reception Message Commande", "Execution START macro d'ID : " & IDCmd)
                                _Server.RunMacro(_IdSrv, IDCmd)
                            Case "STOP"
                                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Reception Message Commande", "Execution STOP macro d'ID : " & IDCmd)
                        End Select
                    End If

                Case Else
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Reception Message Mail", "Commande non non trouvée : " & ParaAdr2(2).ToUpper)
            End Select

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Commande Mail", ex.Message)
        End Try
    End Sub

    Private Function ReturnDeviceIDByName(ByVal Name As String) As String
        Dim sortie As String = ""
        Try

            Dim listeDevices As New List(Of TemplateDevice)
            listeDevices = _Server.GetAllDevices(_IdSrv)
            For Each objet As Object In listeDevices
                If Name = objet.Name Then
                    sortie = objet.ID
                    Exit For
                End If
            Next

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Mail - ReturnDeviceIDByName", ex.Message)
        End Try
        Return sortie
    End Function

    Private Function ReturnMacroIDByName(ByVal Name As String) As String
        Dim sortie As String = ""
        Try

            Dim listeMacros As New List(Of Macro)
            listeMacros = _Server.GetAllMacros(_IdSrv)
            For Each objet As Object In listeMacros
                If Name = objet.Nom Then
                    sortie = objet.ID
                    Exit For
                End If
            Next

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Mail - ReturnMacroIDByName", ex.Message)
        End Try

        Return sortie

    End Function

#End Region

End Class

#Region "MailBoxHandler"

Public Class MailBoxHandler
    Implements IDisposable

    Private _imapClient As ImapClient
    Private _pop3client As Pop3Client
    Private memUids As UInteger()
    
    Public Event HandleMessage As EventHandler(Of HandleMessageEventArgs)

    Public Sub New(ByVal hostName As String, ByVal port As Integer, ByVal userName As String, ByVal password As String, ByVal protocole As String)
        Try
            If protocole = "IMAP" Then
                _imapClient = New ImapClient(hostName, port, True)
                _imapClient.Login(userName, password, S22.Imap.AuthMethod.Login)
            Else
                _pop3client = New Pop3Client(hostName, port, False)
                _pop3client.Login(userName, password, S22.Pop3.AuthMethod.Login)
            End If
        Catch ex As Exception

        End Try
    End Sub

    Public Function CheckExistingEmails() As String
        If _imapClient IsNot Nothing Then
            Try
                Dim uids As UInteger() = Me._imapClient.Search(SearchCondition.Unseen)
                For Each uid As UInteger In uids
                    If memUids IsNot Nothing Then
                        If memUids.Contains(uid) = False Then
                            Dim message = Me._imapClient.GetMessage(uid)
                            Me.ProcessMessage(uid, message)
                            'Me._imapClient.AddMessageFlags(uid, S22.Imap.MessageFlag.Seen)
                        End If
                    Else
                        Dim message = Me._imapClient.GetMessage(uid)
                        Me.ProcessMessage(uid, message)
                    End If
                Next
                memUids = uids
                Return "ok"
            Catch ex As Exception
                Return "Erreur dans CheckExistingImapEmails"
            End Try
        Else
            Try
                Dim uids As UInteger() = Me._pop3client.GetMessageNumbers
                For Each uid As UInteger In uids
                    If memUids IsNot Nothing Then
                        If memUids.Contains(uid) = False Then
                            Dim message = Me._pop3client.GetMessage(uid, S22.Pop3.FetchOptions.Normal, True)
                            message.Body = Replace(message.Body, vbLf, "") ' supprime les saut de ligne éventuels
                            message.Body = Replace(message.Body, vbCr, "") ' supprime les retours chariot éventuels
                            message.Body = Replace(message.Body, vbCrLf, "") ' supprime les deux éventuels
                            Me.ProcessMessage(uid, message)
                        End If
                    Else
                        Dim message = Me._pop3client.GetMessage(uid, S22.Pop3.FetchOptions.Normal, True)
                        message.Body = Replace(message.Body, vbLf, "") ' supprime les saut de ligne éventuels
                        message.Body = Replace(message.Body, vbCr, "") ' supprime les retours chariot éventuels
                        message.Body = Replace(message.Body, vbCrLf, "") ' supprime les deux éventuels
                        Me.ProcessMessage(uid, message)
                    End If
                Next
                memUids = uids
                Return "ok"
            Catch ex As Exception
                Return "Erreur dans CheckExistingPop3Emails"
            End Try
        End If
    End Function

    Public Function IsConnected() As Boolean
        If _imapClient IsNot Nothing Then
            Try
                If _imapClient.Authed Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                Return False
            End Try
        Else
            Try
                If _pop3client.Authed Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                Return False
            End Try
        End If
    End Function

    Protected Sub ProcessMessage(ByVal uid As UInteger, ByVal message As MailMessage)
        Try
            Dim hea = New HandleMessageEventArgs(message)
            RaiseEvent HandleMessage(Me, hea)

        Catch ex As Exception

        Finally
            message.Dispose()
            message = Nothing
        End Try
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        If _imapClient IsNot Nothing Then
            Try
                _imapClient.Logout()
                _imapClient.Dispose()
                _imapClient = Nothing
            Catch ex As Exception

            End Try
        Else
            Try
                _pop3client.Logout()
                _pop3client.Dispose()
                _pop3client = Nothing
            Catch ex As Exception

            End Try
        End If
    End Sub
End Class

#End Region

#Region "HandleMessageEventArgs"

Public Class HandleMessageEventArgs
    Inherits EventArgs
   
    Public Property Message() As MailMessage
        Get
            Return m_Message
        End Get
        Private Set(ByVal value As MailMessage)
            m_Message = value
        End Set
    End Property
    Private m_Message As MailMessage

    Public Sub New(ByVal message As MailMessage)
        Me.Message = message
    End Sub
End Class

#End Region
