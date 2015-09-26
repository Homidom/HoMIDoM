Imports HoMIDom
Imports HoMIDom.HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports SKYPE4COMLib

' Auteur : HoMIDoM
' Date : 05/04/2013

''' <summary>Class Driver_Skype</summary>
''' <remarks></remarks>
<Serializable()> Public Class Driver_Skype
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "69E59C6A-9DF0-11E2-A3E7-5DE16088709B" 'ne pas modifier car utilisé dans le code du serveur
    Dim _Nom As String = "Skype" 'Nom du driver à afficher
    Dim _Enable As Boolean = False 'Activer/Désactiver le driver
    Dim _Description As String = "Driver pour Skype : SMS, Call..." 'Description du driver
    Dim _StartAuto As Boolean = False 'True si le driver doit démarrer automatiquement
    Dim _Protocol As String = "HTTP" 'Protocole utilisé par le driver, exemple: RS232
    Dim _IsConnect As Boolean = False 'True si le driver est connecté et sans erreur
    Dim _IP_TCP As String = "@" 'Adresse IP TCP à utiliser, "@" si non applicable pour le cacher côté client
    Dim _Port_TCP As String = "@" 'Port TCP à utiliser, "@" si non applicable pour le cacher côté client
    Dim _IP_UDP As String = "@" 'Adresse IP UDP à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Port_UDP As String = "@" 'Port UDP à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Com As String = "@" 'Port COM à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Refresh As Integer = 0 'Valeur à laquelle le driver doit rafraichir les valeurs des devices (ex: toutes les 200ms aller lire les devices)
    Dim _Modele As String = "Skype" 'Modèle du driver/interface
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

    Private oSkype As New SKYPE4COMLib.Skype
    Private oCall As New SKYPE4COMLib.Call
    Private mmCall As Boolean

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
            Try
                _DEBUG = _Parametres.Item(0).Valeur
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "Erreur dans les paramétres avancés. utilisation des valeur par défaut" & ex.Message)
            End Try

            Dim test As Microsoft.Win32.RegistryKey = My.Computer.Registry.ClassesRoot.OpenSubKey("CLSID\{830690FC-BF2F-47A6-AC2D-330BCB402664}", False)
            If test Is Nothing Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Skype Start", "Veuillez installer Skype avant de démarrer et d'utiliser le driver")
                Exit Sub
            End If

            'Vérifie si Skype est démarrer si non tentative de de connexion  
            oSkype = New Skype
            'oSkype = CreateObject("Skype4COM.Skype", "Skype_")

            If oSkype.Client.IsRunning = False Then
                oSkype.Client.Start(True, True)
                Threading.Thread.Sleep(3000)
            End If

            'While Not oSkype.Client.IsRunning
            'System.Threading.Thread.Sleep(10000)
            'End While

            oSkype.Attach(5, True)
            'oSkype.Attach(7, False)

            AddHandler oSkype.MessageStatus, AddressOf MessageStatus
            AddHandler oSkype.SmsMessageStatusChanged, AddressOf MessageStatusSMS
            AddHandler oSkype.CallStatus, AddressOf CallStatus

            If _Refresh > 0 Then
                MyTimer.Interval = _Refresh * 1000
                MyTimer.Enabled = True
                AddHandler MyTimer.Elapsed, AddressOf TimerTick
            End If

            Threading.Thread.Sleep(3000)

            If oSkype.AttachmentStatus = oSkype.Convert.TextToAttachmentStatus("SUCCESS") Then
                _IsConnect = True
            End If

            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Skype Start", "Demarré")

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", ex.Message)
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try

            RemoveHandler oSkype.MessageStatus, AddressOf MessageStatus
            RemoveHandler oSkype.SmsMessageStatusChanged, AddressOf MessageStatusSMS
            RemoveHandler oSkype.CallStatus, AddressOf CallStatus
            RemoveHandler MyTimer.Elapsed, AddressOf TimerTick

            MyTimer.Stop()
            'oSkype.Client.Shutdown()

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

            If Objet.Type = "GENERIQUEBOOLEEN" Then
                If (UCase(Objet.adresse1.ToString) = "ENVOYER UN SMS" Or UCase(Objet.adresse1.ToString) = "ENVOYER UN MESSAGE") And Objet.value = True Then
                    Write(Objet, "SEND", Objet.modele)
                    Objet.setvalue(False)
                End If
                If UCase(Objet.adresse1.ToString) = "APPELER" Then
                    If Objet.value = True Then
                        Write(Objet, "CALL")
                        mmCall = True
                    Else
                        If mmCall Then
                            Write(Objet, "STOP CALL")
                            mmCall = False
                        End If
                    End If
                End If
            End If

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

            Select Case UCase(Command)
                Case "SEND"

                    Try

                        If Parametre1 = "" Then
                            _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "Skype Write", "SMS à envoyer vide, annulation")
                        Else

                            _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "Skype Write", "SMS envoi en cours : " & Parametre1 & " à " & Objet.adresse2.ToString)
                            Select Case UCase(Objet.adresse1.ToString)
                                Case "ENVOYER UN SMS"

                                    Dim SmsStatus As SKYPE4COMLib.TSmsMessageStatus

                                    oSkype.CreateSms(TSmsMessageType.smsMessageTypeOutgoing, Objet.adresse2.ToString)
                                    oSkype.SendSms(Objet.adresse2.ToString, Parametre1)

                                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Write", oSkype.Convert.SmsMessageStatusToText(SmsStatus))

                                Case "ENVOYER UN MESSAGE"
                                    Dim ChatStatus As SKYPE4COMLib.TChatMessageStatus

                                    oSkype.CreateChatWith(Objet.adresse2.ToString)
                                    oSkype.SendMessage(Objet.adresse2.ToString, Parametre1)

                                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Write", oSkype.Convert.ChatMessageStatusToText(ChatStatus))

                            End Select

                        End If

                    Catch ex As Exception
                        _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "Skype Write", "Error Sending SMS: " & ex.ToString)
                    End Try

                Case "CALL"
                    Try
                        If UCase(Objet.adresse1.ToString) = "APPELER" Then
                            oCall = oSkype.PlaceCall(Objet.adresse2.ToString)
                        End If

                    Catch ex As Exception
                        _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DRIVER, "Skype Write CALL", "error:" & ex.Message)
                    End Try

                Case "STOP CALL"
                    Try
                        If UCase(Objet.adresse1.ToString) = "APPELER" Then
                            oCall.Finish()
                        End If

                    Catch ex As Exception
                        _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DRIVER, "Skype Write STOP CALL", "error:" & ex.Message)
                    End Try

                Case Else : _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DRIVER, "Skype Write", "Commande " & Command & " non gérée")
            End Select

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
            _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN)

            'Parametres avancés
            Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)

            'ajout des commandes avancées pour les devices
            'add_devicecommande("COMMANDE", "DESCRIPTION", nbparametre)
            Add_DeviceCommande("SEND", "Envoyer un Message", 1)
            Add_DeviceCommande("CALL", "Appeler", 0)
            Add_DeviceCommande("STOP CALL", "Raccrocher", 0)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Commande", "Envoyer un message, Envoyer un SMS, Recevoir un message, Recevoir un SMS, Appeler")
            Add_LibelleDevice("ADRESSE2", "Contact", "Pseudo du contact (respecter la casse) ou numéro de téléphone commencant par +33xxxxxxxxx")
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("MODELE", "Message", "Maxi 160 caractères")
            Add_LibelleDevice("REFRESH", "Refresh", "")
            'Add_LibelleDevice("LASTCHANGEDUREE", "LastChange Durée", "")

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " New", ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)
        If oSkype.Client.IsRunning = True And oSkype.AttachmentStatus <> oSkype.Convert.TextToAttachmentStatus("SUCCESS") Then
            oSkype.Attach(5, True)
            Threading.Thread.Sleep(3000)
        End If
        If oSkype.AttachmentStatus = oSkype.Convert.TextToAttachmentStatus("SUCCESS") Then
            _IsConnect = True
        Else
            _IsConnect = False
        End If
    End Sub

#End Region

#Region "Fonctions internes"
    'Insérer ci-dessous les fonctions propres au driver et nom communes (ex: start)

    ''' <summary>Reception de SMS</summary>
    Private Sub MessageStatusSMS(ByVal msg As SmsMessage, ByVal status As TSmsMessageStatus)
        Try
            If _Enable = False Then Exit Sub
            If status = TSmsMessageStatus.smsMessageStatusReceived Then

                If InStr(msg.Body, ":") Then

                    CommandeString(msg.Body)
                Else

                    'Recherche si un device affecté
                    Dim listedevices As New ArrayList
                    listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, "", "", Me._ID, True)
                    'un device trouvé on maj la value
                    If (listedevices.Count > 0) Then
                        For Each objet As Object In listedevices
                            If InStr(msg.ReplyToNumber, "#" & objet.adresse2) And UCase(objet.adresse1) = "RECEVOIR UN SMS" Then
                                objet.setValue = "Recu le : " & msg.Timestamp & " de: " & objet.adresse2 & " : " & msg.Body
                                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "Reception Message SMS : ", "Recu de : " & objet.adresse2 & " le : " & msg.Timestamp & " : " & msg.Body)
                            End If
                        Next
                    Else
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Skype Reception Message SMS", "Composant non trouvé : " & msg.Body)
                    End If
                End If

            End If

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Skype Reception Message SMS", "Exception" & ex.Message)
        End Try
    End Sub

    Private Sub MessageStatus(ByVal msg As ChatMessage, ByVal status As TChatMessageStatus)
        Try
            If _Enable = False Then Exit Sub
            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "Reception Message Skype : ", "Recu de : " & msg.ChatName & " le: " & msg.Timestamp & " status: " & status & " : " & msg.Body)

            If status = TChatMessageStatus.cmsReceived Then

                If InStr(msg.Body, ":") Then

                    CommandeString(msg.Body)
                Else

                    'Recherche si un device affecté
                    Dim listedevices As New ArrayList
                    listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, "", "", Me._ID, True)
                    'un device trouvé on maj la value
                    If (listedevices.Count > 0) Then
                        For Each objet As Object In listedevices
                            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "Reception Message Skype : ", "Recu de : " & objet.adresse2 & " de type: " & objet.adresse1)

                            If InStr(msg.ChatName, "#" & objet.adresse2) And UCase(objet.adresse1) = "RECEVOIR UN MESSAGE" Then
                                objet.setValue("Recu le : " & msg.Timestamp & " de: " & objet.adresse2 & " : " & msg.Body)
                                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "Reception Message Skype : ", "Recu de : " & msg.ChatName & " le: " & msg.Timestamp & " status: " & status & " : " & msg.Body)
                            End If
                        Next
                    Else
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Reception Message Skype", "Composant non trouvé : " & msg.Body)
                    End If
                End If

            End If

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Reception Message Skype", "Exception" & ex.Message)
        End Try
    End Sub

    Private Sub CallStatus(ByVal pCall As SKYPE4COMLib.Call, ByVal aStatus As SKYPE4COMLib.TCallStatus)

        If oSkype.Convert.TextToCallStatus("RINGING") = aStatus And _
            (oSkype.Convert.TextToCallType("INCOMING_P2P") = pCall.Type() Or _
             oSkype.Convert.TextToCallType("INCOMING_PSTN") = pCall.Type) Then
            pCall.Answer()
        End If

    End Sub

    Private Sub CommandeString(ByVal msg As String)

        Dim ParaAdr2 = Split(msg, ":")

        Select Case ParaAdr2(0).ToUpper
            Case "COMPOSANT" 'If _DEBUG Then
                Dim ID = ReturnDeviceIDByName(ParaAdr2(1))
                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Reception Message Commande", "Passage par la partie composant d'ID : " & ID & " L'action     est : " & ParaAdr2(2))
                Dim TempDevice As TemplateDevice = _Server.ReturnDeviceById(_IdSrv, ID)
                If TempDevice IsNot Nothing Then
                    If TempDevice.Type = ListeDevices.LAMPE Or TempDevice.Type = ListeDevices.APPAREIL Then

                        ' Analyse de la commande 
                        Select Case ParaAdr2(2).ToUpper
                            Case "ON", "OFF"
                                Dim x As DeviceAction = New DeviceAction
                                x.Nom = ParaAdr2(2)
                                _Server.ExecuteDeviceCommand(_IdSrv, ID, x)

                            Case "DIM"
                                Dim x As DeviceActionSimple = New DeviceActionSimple
                                x.Nom = ParaAdr2(2)
                                x.Param1 = ParaAdr2(3)
                                _Server.ExecuteDeviceCommandSimple(_IdSrv, ID, x)

                            Case "READ"


                        End Select
                    End If
                End If

            Case "MACRO"
                Dim ID = ReturnMacroIDByName(ParaAdr2(1))
                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Reception Message Commande", "Passage par la partie Macro d'ID : " & ID)

                Dim TempMacro As Macro = _Server.ReturnMacroById(_IdSrv, ID)
                If TempMacro IsNot Nothing And TempMacro.Enable = True Then
                    ' Analyse de la commande 
                    Select Case ParaAdr2(2).ToUpper
                        Case "START"
                            _Server.RunMacro(_IdSrv, ID)
                        Case "STOP"

                    End Select
                End If

            Case Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Reception Message Skype", "Commande non non trouvée : " & ParaAdr2(2).ToUpper)
        End Select

    End Sub

    Private Function ReturnDeviceIDByName(ByVal Name As String) As String
        Dim listeDevices As New List(Of TemplateDevice)
        Dim sortie As String = ""
        listeDevices = _Server.GetAllDevices(_IdSrv)
        For Each objet As Object In listeDevices
            If Name = objet.Name Then
                sortie = objet.ID
                Exit For
            End If
        Next
        Return sortie
    End Function

    Private Function ReturnMacroIDByName(ByVal Name As String) As String
        Dim listeMacros As New List(Of Macro)
        Dim sortie As String = ""
        listeMacros = _Server.GetAllMacros(_IdSrv)
        For Each objet As Object In listeMacros
            If Name = objet.Nom Then
                sortie = objet.ID
                Exit For
            End If
        Next
        Return sortie
    End Function

#End Region

End Class
