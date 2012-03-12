Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports Firmata 'Référence à la dll Firmata

'************************************************
'INFOS 
'************************************************
'Le driver fonctionne de la manière suivante:
' 
'ETAPE1: Le driver est créé par Homidom --> lancement de la fonction Sub (pour récupérer/définir les paramètres avancés)
'ETAPE2: Le driver est lancé par Homidom --> lancement de la fonction Start (communication, ajout des évènements, config des pins...)
'ETAPE3:
'          - une pin (ana ou binaire) change sur la carte --> déclenchement des fonctions DigitalMessageReceieved ou AnalogMessageReceieved
'          - l'utilisateur active un device (ON/OFF) --> lancement de la fonction write
'          - l'utilisateur demande la lecture d'un device --> lancement de la fonction read
'ETAPE4: le driver est arrêté par Homidom --> lancement de la fonction stop
'************************************************

Public Class Driver_Arduino
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variable Driver"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "958D5812-390B-11E1-A6F7-D4CA4724019B"
    Dim _Nom As String = "Arduino"
    Dim _Enable As String = False
    Dim _Description As String = "Arduino"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "RS232"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "COM2"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "Arduino"
    Dim _Version As String = "1.0"
    Dim _Picture As String = ""
    Dim _Server As HoMIDom.HoMIDom.Server
    Dim _Device As HoMIDom.HoMIDom.Device
    Dim _DeviceSupport As New ArrayList
    Dim _Parametres As New ArrayList
    Dim _LabelsDriver As New ArrayList
    Dim _LabelsDevice As New ArrayList
    Dim MyTimer As New Timers.Timer
    Dim _idsrv As String
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande)

    'A ajouter dans les ppt du driver
    Dim _tempsentrereponse As Integer = 1500
    Dim _ignoreadresse As Boolean = False
    Dim _lastetat As Boolean = True
#End Region

#Region "Declaration"
    Dim WithEvents ArduinoVB As New Firmata.FirmataVB 'Déclaration de l'objet représentant la carte (via la dll firmata)
    Dim _Baud As Integer = 57600
    Dim _RemotingPort0 As Integer = 0
    Dim _RemotingPort1 As Integer = 1
    Dim _Pin(13) As Integer 'Tableau représentant les 13 pins binaire, 0 si entrée 1 si sortie
#End Region

#Region "Fonctions génériques"

    Public WriteOnly Property IdSrv As String Implements HoMIDom.HoMIDom.IDriver.IdSrv
        Set(ByVal value As String)
            _idsrv = value
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

    Public Property Port_TCP() As Object Implements HoMIDom.HoMIDom.IDriver.Port_TCP
        Get
            Return _Port_TCP
        End Get
        Set(ByVal value As Object)
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

    ''' <summary>
    ''' Aller lire une entrée
    ''' </summary>
    ''' <param name="Objet">Device</param>
    ''' <remarks></remarks>
    Public Sub Read(ByVal Objet As Object) Implements HoMIDom.HoMIDom.IDriver.Read
        Try
            If _Enable = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Impossible d'effectuer un Read car le driver n'est pas Activé")
                Exit Sub
            End If
            If _IsConnect = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Impossible d'effectuer un Read car le driver n'est pas connecté à la carte")
                Exit Sub
            End If

            Dim _type As Integer = 0
            Select Case Objet.Type
                Case "CONTACT"
                    _type = 0
                    'Aller lire une pin digital via DigitalRead(Pin as integer)
                    Dim Val As Integer = ArduinoVB.DigitalRead(CInt(Objet.Adresse1))
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Device:" & Objet.Name & " Adresse:" & Objet.Adresse1 & " Valeur:" & Val)
                    Objet.Value = Val
                Case "APPAREIL"
                    _type = 1
                    'Aller lire une pin digital via DigitalRead(Pin as integer)
                    Dim Val As Integer = ArduinoVB.DigitalRead(CInt(Objet.Adresse1))
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Device:" & Objet.Name & " Adresse:" & Objet.Adresse1 & " Valeur:" & Val)
                    Objet.Value = Val
                Case "GENERIQUEVALUE"
                    _type = 2
                    'Aller lire une pin analogique via AnalogRead(Pin as integer)
                    If IsNumeric(Objet.Adresse1) = False Then
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Impossible d'effectuer un Read car l'adresse du device GENERIQUEVALUE n'est pas un nombre (0-5)")
                        Exit Sub
                    End If
                    If CInt(Objet.Adresse1) < 0 Or CInt(Objet.Adresse1) > 5 Then
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Impossible d'effectuer un Read car l'adresse du device GENERIQUEVALUE doit être comprise entre 0 et 5")
                        Exit Sub
                    End If
                    Dim Val As Integer = ArduinoVB.AnalogRead(CInt(Objet.Adresse1))
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Device:" & Objet.Name & " Adresse:" & CInt(Objet.Adresse1) & " Valeur:" & Val)
                    Objet.Value = Val
                Case Else
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Le type de device " & Objet.Type & " n'est pas supporté pas ce driver")
                    Exit Sub
            End Select


        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur : " & ex.ToString)
        End Try
    End Sub

    Public Property Refresh() As Integer Implements HoMIDom.HoMIDom.IDriver.Refresh
        Get
            Return _Refresh
        End Get
        Set(ByVal value As Integer)
            _Refresh = value
        End Set
    End Property

    Public Sub Restart() Implements HoMIDom.HoMIDom.IDriver.Restart
        [Stop]()
        Start()
    End Sub

    Public Property Server() As HoMIDom.HoMIDom.Server Implements HoMIDom.HoMIDom.IDriver.Server
        Get
            Return _Server
        End Get
        Set(ByVal value As HoMIDom.HoMIDom.Server)
            _Server = value
        End Set
    End Property

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
                Case "ADRESSE1"
                    If IsNumeric(Value) = False Then
                        retour = "veuillez saisir une adresse numérique est positive entre 2 et 12"
                    Else
                        If Value < 2 And Value > 12 Then
                            retour = "veuillez saisir une adresse entre 2 et 12"
                        End If
                    End If
            End Select
            Return retour
        Catch ex As Exception
            Return "Une erreur est apparue lors de la vérification du champ " & Champ & ": " & ex.ToString
        End Try
    End Function

    ''' <summary>
    ''' Démarrer le driver
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        Try
            If Enable = True And ArduinoVB.PortOpen = False Then
                'récupération des paramétres avancés
                Try
                    _Baud = _Parametres.Item(0).Valeur 'Baud
                    _RemotingPort0 = _Parametres.Item(1).Valeur 'RemotingPort0
                    _RemotingPort1 = _Parametres.Item(2).Valeur 'RemotingPort1

                    'On récupère la config des pins définie par l'utilisateur
                    For i As Integer = 2 To 13 'on prend que les pins 2 à 13
                        If _Parametres.Item(i + 1).valeur = 0 Then
                            _Pin(i) = 0 'Définie en entrée
                        Else
                            _Pin(i) = 1 'Définie en sortie
                        End If
                    Next
                Catch ex As Exception
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "Erreur dans les paramétres avancés. utilisation des valeur par défaut" & ex.Message)
                    _Baud = 57600
                    _RemotingPort0 = 0
                    _RemotingPort1 = 0
                    For i = 2 To 6
                        _Pin(i) = 0
                    Next
                    For i = 7 To 13
                        _Pin(i) = 1
                    Next
                End Try

                'On ajoute à l'objet les évènements générés par la carte (fournis par la dll)
                AddHandler ArduinoVB.DigitalMessageReceieved, AddressOf FirmataVB1_DigitalMessageReceieved 'Evènement lors d'un changement de port Binaire
                AddHandler ArduinoVB.AnalogMessageReceieved, AddressOf FirmataVB1_AnalogMessageReceieved 'Evènement lors d'un changement d'entrée analogique
                AddHandler ArduinoVB.VersionInfoReceieved, AddressOf FirmataVB1_VersionInfoReceieved 'Evènement lorsque la carte envoi sa version

                'Connexion à la carte
                ArduinoVB.Connect(_Com, _Baud)

                Threading.Thread.Sleep(1000)

                'Si le port de communication est ouvert
                If ArduinoVB.PortOpen = True Then
                    ArduinoVB.QueryVersion() 'Demander la version à la carte (fonction fournie par la dll) pour s'assurer qu'on discute bien avec elle
                    _IsConnect = True
                    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Start", "Carte connectée sur le port:" & ArduinoVB.PortName & " Baud:" & ArduinoVB.Baud)
                    ArduinoVB.DigitalPortReport(0, _RemotingPort0) 'Activer le port0
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Start", "Activation du port 0 effectué")
                    ArduinoVB.DigitalPortReport(1, _RemotingPort1) 'Activer le port1
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Start", "Activation du port 1 effectué")
                    'Définir les pins 2 à 12 en entrée ou sortie
                    For i As Integer = 2 To 13
                        If _Pin(i) = 0 Then
                            'Pin à définir en entrée via la fonction PinMid(Pin as integer, Mode as integer) --> fournie par la dll
                            ArduinoVB.PinMode(i, Firmata.FirmataVB.INPUT)
                            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Start", "Pin" & i & " définie en entrée")
                        Else
                            'Pin à définir en sortie
                            ArduinoVB.PinMode(i, Firmata.FirmataVB.OUTUPT)
                            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Start", "Pin" & i & " définie en sortie")
                        End If
                    Next
                Else 'Sinon on peut rien faire on est pas connecté
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "Le driver n'a pas réussit à se connecter à la carte")
                    _IsConnect = False
                    Exit Sub
                End If
            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "Le driver n'est pas activé ou la carte est déjà connectée ")
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "Erreur lors du démarrage du driver: " & ex.ToString)
            _IsConnect = False
        End Try
    End Sub

    Public Property StartAuto() As Boolean Implements HoMIDom.HoMIDom.IDriver.StartAuto
        Get
            Return _StartAuto
        End Get
        Set(ByVal value As Boolean)
            _StartAuto = value
        End Set
    End Property

    ''' <summary>
    ''' Arrêter le driver
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        'cree l'objet
        Try
            'On désaffecte les évènements pouvant être produit par l'objet (Arduino)
            RemoveHandler ArduinoVB.DigitalMessageReceieved, AddressOf FirmataVB1_DigitalMessageReceieved
            RemoveHandler ArduinoVB.AnalogMessageReceieved, AddressOf FirmataVB1_AnalogMessageReceieved
            RemoveHandler ArduinoVB.VersionInfoReceieved, AddressOf FirmataVB1_VersionInfoReceieved
            'Deconnexion de la carte Arduino
            ArduinoVB.Disconnect()
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Stop", "Driver arrêté")
            _IsConnect = False
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Stop", "Erreur lors de l'arrêt du driver: " & ex.ToString)
            _IsConnect = False
        End Try
    End Sub

    Public ReadOnly Property Version() As String Implements HoMIDom.HoMIDom.IDriver.Version
        Get
            Return _Version
        End Get
    End Property

    ''' <summary>
    ''' Activer une sortie
    ''' </summary>
    ''' <param name="Objet"></param>
    ''' <param name="Commande"></param>
    ''' <param name="Parametre1"></param>
    ''' <param name="Parametre2"></param>
    ''' <remarks></remarks>
    Public Sub Write(ByVal Objet As Object, ByVal Commande As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
        If _Enable = False Then
            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Write", "Impossible de traiter cette commande car le driver n'est pas activé")
            Exit Sub
        End If
        If _IsConnect = False Then
            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Write", "Impossible de traiter cette commande car le driver n'est connecté")
            Exit Sub
        End If
        Try
            If Objet.type = "APPAREIL" Then
                If Commande = "ON" Then
                    ArduinoVB.DigitalWrite(Objet.Adresse1, 1)
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Write", "Activation de la pin:" & Objet.Adresse1)
                End If
                If Commande = "OFF" Then
                    ArduinoVB.DigitalWrite(Objet.Adresse1, 0)
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Write", "Désactivation de la pin:" & Objet.Adresse1)
                End If
            Else
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Write", "Impossible d'écrire sur un device autre que de type APPAREIL")
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write", "Erreur: " & ex.ToString)
        End Try
    End Sub

    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice

    End Sub

    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice

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

    ''' <summary>
    ''' Déclaration du driver
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        'Devices supportés par le driver
        _DeviceSupport.Add(ListeDevices.GENERIQUEVALUE)
        _DeviceSupport.Add(ListeDevices.CONTACT)
        _DeviceSupport.Add(ListeDevices.APPAREIL)

        'On définit par défaut les pins 2 à 6 en entrée et 7 à 12 en sortie
        _Pin(0) = -1 'Pin 0 (non utilisée)
        _Pin(1) = -1 'Pin 1 (non utilisée)
        _Pin(2) = 0 'Pin 2 en entrée
        _Pin(3) = 0 'Pin 3 en entrée
        _Pin(4) = 0 'Pin 4 en entrée
        _Pin(5) = 0 'Pin 5 en entrée
        _Pin(6) = 0 'Pin 6 en entrée
        _Pin(7) = 1 'Pin 7 en sortie
        _Pin(8) = 1 'Pin 8 en sortie
        _Pin(9) = 1 'Pin 9 en sortie
        _Pin(10) = 1 'Pin 10 en sortie
        _Pin(11) = 1 'Pin 11 en sortie
        _Pin(12) = 1 'Pin 12 en sortie
        _Pin(13) = 1 'Pin 13 en sortie

        'Paramétres avancés pouvant être définit côté Admin sur le driver
        Add_ParamAvance("Baud", "BaudRate", 57600)
        Add_ParamAvance("RemotingPort0", "Si actif (1, sinon 0) Arduino envoi régulièrement les valeurs des pins du port 0", 0)
        Add_ParamAvance("RemotingPort1", "Si actif (1, sinon 0) Arduino envoi régulièrement les valeurs des pins du port 1", 0)
        Add_ParamAvance("Pin2", "Définit si Pin 2 est en entrée(0) ou en sortie(1)", 0)
        Add_ParamAvance("Pin3", "Définit si Pin 3 est en entrée(0) ou en sortie(1)", 0)
        Add_ParamAvance("Pin4", "Définit si Pin 4 est en entrée(0) ou en sortie(1)", 0)
        Add_ParamAvance("Pin5", "Définit si Pin 5 est en entrée(0) ou en sortie(1)", 0)
        Add_ParamAvance("Pin6", "Définit si Pin 6 est en entrée(0) ou en sortie(1)", 0)
        Add_ParamAvance("Pin7", "Définit si Pin 7 est en entrée(0) ou en sortie(1)", 1)
        Add_ParamAvance("Pin8", "Définit si Pin 8 est en entrée(0) ou en sortie(1)", 1)
        Add_ParamAvance("Pin9", "Définit si Pin 9 est en entrée(0) ou en sortie(1)", 1)
        Add_ParamAvance("Pin10", "Définit si Pin 10 est en entrée(0) ou en sortie(1)", 1)
        Add_ParamAvance("Pin11", "Définit si Pin 11 est en entrée(0) ou en sortie(1)", 1)
        Add_ParamAvance("Pin12", "Définit si Pin 12 est en entrée(0) ou en sortie(1)", 1)
        Add_ParamAvance("Pin13", "Définit si Pin 13 est en entrée(0) ou en sortie(1)", 1)

        Add_LibelleDevice("ADRESSE1", "Adresse", "")
        Add_LibelleDevice("ADRESSE2", "@", "")
        Add_LibelleDevice("SOLO", "@", "")
        Add_LibelleDevice("MODELE", "@", "")
        Add_LibelleDevice("REFRESH", "Refresh", "")
        Add_LibelleDevice("LASTCHANGEDUREE", "LastChange Durée", "")
    End Sub
#End Region

#Region "Fonctions propres au driver"

    ''' <summary>
    ''' Message envoyé par la carte (évènement) lorsqu'un port binaire a changée
    ''' </summary>
    ''' <param name="portNumber">N° du port où la pin se situe (sera =0 pour les pins 0 à 7 et =1 pour les pins suivantes)</param>
    ''' <param name="portData">Valeur représentant la valeur du port (ex: si pin 0=1 et pin1= 1 donc 11 en binaire donne 3 en dec, la valeur de portData sera 3)</param>
    ''' <remarks></remarks>
    Private Sub FirmataVB1_DigitalMessageReceieved(ByVal portNumber As Integer, ByVal portData As Integer)
        Try
            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " DigitalMessageRecu", "PortNumber:" & portNumber & " Value:" & portData)

            Select Case portNumber 'Quel port est concerné ?
                Case 0 'On va aller lire les pins 2 à 7 du port 0 (on prend pas en compte les pins 0 et 1)
                    For i As Integer = 2 To 7
                        'la fonction DigitalRead(pin as integer) est fournie par la dll, elle permet pour une pin donnée de savoir son état 0/1
                        traitement(ArduinoVB.DigitalRead(i), i, _Pin(i))
                    Next
                Case 1 'On va aller lire les pins 8 à 13 du port 1 (on prend pas en compte la pin 13)
                    For i As Integer = 8 To 13
                        traitement(ArduinoVB.DigitalRead(i), i, _Pin(i))
                    Next
            End Select

        Catch ex As Exception
            'Il y a eu une erreur lors du traitement
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " DigitalMessageReceieved", "Erreur : " & ex.ToString)
        End Try
    End Sub

    ''' <summary>
    ''' Retourne la version de la carte (fonction fournie par la dll)
    ''' </summary>
    ''' <param name="majorVersion">version majeure</param>
    ''' <param name="minorVersion">version mineure</param>
    ''' <remarks></remarks>
    Private Sub FirmataVB1_VersionInfoReceieved(ByVal majorVersion As Integer, ByVal minorVersion As Integer)
        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " VersionInfoReceieved", "Version:" & majorVersion & "." & minorVersion)
    End Sub

    ''' <summary>
    ''' Message envoyé par la carte (évènement) lorsqu'une pin analogique a changée
    ''' </summary>
    ''' <param name="pin">numéro de la pin</param>
    ''' <param name="value">valeur lue</param>
    ''' <remarks></remarks>
    Private Sub FirmataVB1_AnalogMessageReceieved(ByVal pin As Integer, ByVal value As Integer)
        Try
            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " AnalogMessageReceieved", "Pin:" & pin & " Value:" & value)
            traitement(value, pin, 2) 'aller écrire la valeur dans le device --> Homidom
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " AnalogMessageReceieved", "Erreur : " & ex.ToString)
        End Try
    End Sub

    ''' <summary>Va écrire les valeurs dans Homidom</summary>
    ''' <remarks></remarks>
    Private Sub traitement(ByVal valeur As Integer, ByVal adresse As String, ByVal type As Integer)
        Try
            'Recherche si un device affecté
            Dim listedevices As New ArrayList
            Dim _Type As String = ""

            Select Case type
                Case 0 'CONTACT
                    _Type = "CONTACT" 'ENTREE
                Case 1 'APPAREIL
                    _Type = "APPAREIL" 'SORTIE
                Case 2 'GENERIQUE VALUE
                    _Type = "GENERIQUEVALUE" 'ENTREE ANA
                Case Else
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Process", "Le type de device n'appartient pas à ce driver: " & type)
                    Exit Sub
            End Select

            listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_idsrv, adresse, type, Me._ID, True)

            'un device trouvé on maj la value
            If (listedevices.Count = 1) Then
                'correction valeur pour correspondre au type de value
                'If TypeOf listedevices.Item(0).Value Is Integer Then
                '    If valeur = 1 Then
                '        valeur = 100
                '    ElseIf valeur = 0 Then
                '        valeur = 0
                '    End If
                'Else
                listedevices.Item(0).Value = valeur
                'End If

            ElseIf (listedevices.Count > 1) Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " traitement", "Plusieurs devices correspondent à : " & adresse & ":" & valeur)
            Else
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " traitement", "Le device à l'adresse : " & adresse & " de type " & _Type & " nexiste pas")
                'Le Device n'existe pas dans Homidom
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " traitement", "Exception : " & ex.Message & " --> " & adresse & " : " & valeur)
        End Try
    End Sub


#End Region


End Class
