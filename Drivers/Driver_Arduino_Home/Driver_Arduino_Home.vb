Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports System.IO.Ports
Imports System.Xml

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

Public Class Driver_Arduino_Energie
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variable Driver"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "2CC3C14C-C91E-11E6-B266-E28B4C4C4C3A"
    Dim _Nom As String = "Arduino Home"
    Dim _Enable As Boolean = False
    Dim _Description As String = "Arduino Home"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "RS232"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "COM4"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "@"
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
    Dim _idsrv As String
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande)
    Dim _AutoDiscover As Boolean = False

    'A ajouter dans les ppt du driver
    Dim _tempsentrereponse As Integer = 1500
    Dim _ignoreadresse As Boolean = False
    Dim _lastetat As Boolean = True
#End Region

#Region "Declaration"
    Dim serialPortObj As New SerialPort
    Dim _Baud As Integer = 9600
    Dim msg As String = String.Empty
    Public _Lock As Boolean
    Dim _token As String = "123456"
    Dim _uuid As String = "1"
    Dim _ver As String = String.Empty
    Dim _uuidArduino As String = String.Empty
    Dim _cpteau As Double = 0
    Dim _ana0 As Integer = 0
    Dim _ana1 As Integer = 0
    Dim _ana2 As Integer = 0
    Dim _ana3 As Integer = 0
    Dim _cntessai As Integer = 0
    Dim _maxessai As Integer = 3
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

    Public Property AutoDiscover() As Boolean Implements HoMIDom.HoMIDom.IDriver.AutoDiscover
        Get
            Return _AutoDiscover
        End Get
        Set(ByVal value As Boolean)
            _AutoDiscover = value
        End Set
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

            GetStatus()

            Select Case Objet.Type
                Case "GENERIQUEVALUE"
                    If (Objet.Adresse1.ToString.ToUpper = "CPT_EAU") Then
                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Device:" & Objet.Name & " Adresse:" & Objet.Adresse1 & " Valeur:" & _cpteau)
                        Objet.Value = _cpteau
                    End If
                    If (Objet.Adresse1.ToString.ToUpper = "ANA0") Then
                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Device:" & Objet.Name & " Adresse:" & Objet.Adresse1 & " Valeur:" & _ana0)
                        Objet.Value = _ana0
                    End If
                    If (Objet.Adresse1.ToString.ToUpper = "ANA1") Then
                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Device:" & Objet.Name & " Adresse:" & Objet.Adresse1 & " Valeur:" & _ana1)
                        Objet.Value = _ana1
                    End If
                    If (Objet.Adresse1.ToString.ToUpper = "ANA2") Then
                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Device:" & Objet.Name & " Adresse:" & Objet.Adresse1 & " Valeur:" & _ana2)
                        Objet.Value = _ana2
                    End If
                    If (Objet.Adresse1.ToString.ToUpper = "ANA3") Then
                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Device:" & Objet.Name & " Adresse:" & Objet.Adresse1 & " Valeur:" & _ana3)
                        Objet.Value = _ana3
                    End If
                Case "GENERIQUESTRING"
                    If (Objet.Adresse1.ToString.ToUpper = "CPT_EAU") Then
                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Device:" & Objet.Name & " Adresse:" & Objet.Adresse1 & " Valeur:" & _cpteau)
                        Objet.Value = _cpteau
                    End If
                    If (Objet.Adresse1.ToString.ToUpper = "ANA0") Then
                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Device:" & Objet.Name & " Adresse:" & Objet.Adresse1 & " Valeur:" & _ana0)
                        Objet.Value = _ana0
                    End If
                    If (Objet.Adresse1.ToString.ToUpper = "ANA1") Then
                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Device:" & Objet.Name & " Adresse:" & Objet.Adresse1 & " Valeur:" & _ana1)
                        Objet.Value = _ana1
                    End If
                    If (Objet.Adresse1.ToString.ToUpper = "ANA2") Then
                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Device:" & Objet.Name & " Adresse:" & Objet.Adresse1 & " Valeur:" & _ana2)
                        Objet.Value = _ana2
                    End If
                    If (Objet.Adresse1.ToString.ToUpper = "ANA3") Then
                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Device:" & Objet.Name & " Adresse:" & Objet.Adresse1 & " Valeur:" & _ana3)
                        Objet.Value = _ana3
                    End If
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
            If Enable = True And serialPortObj.IsOpen = False Then
                'récupération des paramétres avancés
                Try
                    _Baud = _Parametres.Item(0).Valeur 'Baud
                    _uuid = _Parametres.Item(1).Valeur 'uuid
                    _token = _Parametres.Item(2).Valeur 'token
                    _cntessai = 0
                Catch ex As Exception
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "Erreur dans les paramétres avancés. utilisation des valeur par défaut" & ex.Message)
                    _Baud = 9600
                    _uuid = 1
                    _token = 123456
                    _cntessai = 0
                End Try


                'Connexion à la carte
                Try
                    Try
                        Dim trv As Boolean = False
                        Dim _ports As String = "<AUCUN>"

                        If _Com = "" Or _Com = " " Then
                            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "Le port COM est vide veuillez le renseigner")
                            Exit Sub
                        End If

                        Dim portNames As String() = SerialPort.GetPortNames()
                        Array.Sort(portNames)
                        For Each serialPortName As String In portNames
                            _ports &= serialPortName & " "
                            If UCase(serialPortName) = UCase(_Com) Then
                                trv = True
                            End If
                        Next

                        If trv = False Then
                            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "Le port COM " & _Com & " n'existe pas, seuls les ports " & _ports & " existe(s)!")
                            Exit Sub
                        End If

                        serialPortObj.PortName = _Com
                        serialPortObj.BaudRate = 9600
                        serialPortObj.DataBits = 8
                        serialPortObj.Parity = Parity.None
                        serialPortObj.StopBits = StopBits.One
                        serialPortObj.Handshake = Handshake.None
                        serialPortObj.Encoding = System.Text.Encoding.Default
                        serialPortObj.Open()
                        _IsConnect = True
                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Start", "Driver démarré sur " & _Com)

                    Catch ex As Exception
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "Erreur lors de la connexion au port COM: " & ex.ToString)
                        _IsConnect = False
                        Exit Sub
                    End Try

                    GetStatus()
                Catch ex As Exception
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "Erreur lors de la connexion au port COM: " & ex.ToString)
                    _IsConnect = False
                End Try


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
            _IsConnect = False
            _cntessai = 0
            'Deconnexion de la carte Arduino
            If serialPortObj.IsOpen Then serialPortObj.Close()
            'On désaffecte les évènements pouvant être produit par l'objet (Arduino)
            RemoveHandler serialPortObj.DataReceived, AddressOf OnReceive
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Stop", "Driver arrêté")
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

    Public ReadOnly Property OsPlatform() As String Implements HoMIDom.HoMIDom.IDriver.OsPlatform
        Get
            Return _OsPlatform
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
            'If Objet.type = "APPAREIL" Then
            '    'If Commande = "ON" Then
            '    '    ArduinoVB.DigitalWrite(Objet.Adresse1, 1)
            '    '    Objet.Value = True
            '    '    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Write", "Activation de la pin:" & Objet.Adresse1)
            '    'End If
            '    'If Commande = "OFF" Then
            '    '    ArduinoVB.DigitalWrite(Objet.Adresse1, 0)
            '    '    Objet.Value = False
            '    '    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Write", "Désactivation de la pin:" & Objet.Adresse1)
            '    'End If
            'Else
            '    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Write", "Impossible d'écrire sur un device autre que de type APPAREIL")
            'End If
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
        _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString

        'Devices supportés par le driver
        _DeviceSupport.Add(ListeDevices.GENERIQUEVALUE)
        _DeviceSupport.Add(ListeDevices.GENERIQUESTRING)


        'Paramétres avancés pouvant être définit côté Admin sur le driver
        Add_ParamAvance("Baud", "BaudRate", 9600)
        Add_ParamAvance("UUID", "UUID de la carte", 1)
        Add_ParamAvance("Token", "Token de la carte", 123456)


        Add_LibelleDevice("ADRESSE1", "Tag", "")
        Add_LibelleDevice("ADRESSE2", "@", "")
        Add_LibelleDevice("SOLO", "@", "")
        Add_LibelleDevice("MODELE", "@", "")
    End Sub
#End Region

#Region "Fonctions propres au driver"

    Private Sub serialPortObj_ErrorReceived(ByVal sender As Object, ByVal e As SerialErrorReceivedEventArgs)
        _IsConnect = False
        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ErrorReceived", "Error: " & e.ToString)
    End Sub


    ''' <summary>
    ''' Message envoyé par Arduino
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OnReceive(sender As Object, e As IO.Ports.SerialDataReceivedEventArgs)
        Try
            If _Lock Then Exit Sub
            msg = serialPortObj.ReadTo("|")

            If String.IsNullOrEmpty(msg) = False Then
                _Lock = True
                ConvertXML()
                msg = String.Empty
            End If

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " OnReceive", "Erreur : " & ex.ToString)
        End Try
    End Sub

    Sub GetStatus()
        Try
            If _IsConnect Then
                AddHandler serialPortObj.DataReceived, AddressOf OnReceive
                Dim trm As String = "token=" & _token & ";id=" & _uuid & ";cmd=getstatus"
                serialPortObj.Write(trm)
                trm = String.Empty
            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " GetStatus", "Impossible d'envoyer une trame vers la carte car la connexion est fermée")
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " GetStatus", "Erreur : " & ex.ToString)
        End Try
    End Sub

    ''' <summary>Va écrire les valeurs dans Homidom</summary>
    ''' <remarks></remarks>
    'Private Sub traitement()
    '    Try
    '        'Recherche si un device affecté
    '        Dim listedevices As New ArrayList

    '        listedevices = _Server.ReturnDeviceByDriver(_idsrv, Me._ID, True)

    '        'un device trouvé on maj la value
    '        If (listedevices.Count >= 1) Then

    '            For Each _item In listedevices
    '                If _item.Adresse1.ToString.ToUpper = "CPT_EAU" Then
    '                    listedevices.Item(0).Value = _cpteau
    '                End If
    '                If _item.Adresse1.ToString.ToUpper = "ANA0" Then
    '                    listedevices.Item(0).Value = _ana0
    '                End If
    '                If _item.Adresse1.ToString.ToUpper = "ANA1" Then
    '                    listedevices.Item(0).Value = _ana1
    '                End If
    '                If _item.Adresse1.ToString.ToUpper = "ANA2" Then
    '                    listedevices.Item(0).Value = _ana2
    '                End If
    '                If _item.Adresse1.ToString.ToUpper = "ANA3" Then
    '                    listedevices.Item(0).Value = _ana3
    '                End If
    '            Next
    '        End If


    '        listedevices = Nothing
    '    Catch ex As Exception
    '        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " traitement", "Exception : " & ex.Message)
    '    End Try
    'End Sub

    Sub ConvertXML()
        Try
            Dim XmlDoc As XmlDocument = New XmlDocument()

            If String.IsNullOrEmpty(msg) Then Exit Sub

            msg = msg.TrimStart()
            msg = msg.Replace("|", "")

            XmlDoc.LoadXml(msg)
            Dim element As XmlNodeList
            Dim noeud As XmlNode

            element = XmlDoc.DocumentElement.GetElementsByTagName("ver")
            For Each noeud In element
                Dim rtr As String = CStr(noeud.InnerText)
                If rtr <> _ver Then
                    _ver = rtr
                    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom, "Arduino version: " & _ver)
                End If
            Next
            element = XmlDoc.DocumentElement.GetElementsByTagName("uuid")
            For Each noeud In element
                Dim rtr As String = CStr(noeud.InnerText)
                If rtr <> _uuidArduino Then
                    _uuidArduino = rtr
                    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom, "Arduino uuid: " & _uuidArduino)
                End If
            Next
            'element = XmlDoc.DocumentElement.GetElementsByTagName("token")
            'For Each noeud In element
            '    LblToken.Text = CBool(noeud.InnerText)
            'Next
            element = XmlDoc.DocumentElement.GetElementsByTagName("compteau")
            For Each noeud In element
                _cpteau = CStr(noeud.InnerText)
                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ConvertXML", "compteau: " & _cpteau)
            Next
            element = XmlDoc.DocumentElement.GetElementsByTagName("ana0")
            For Each noeud In element
                _ana0 = CInt(noeud.InnerText)
                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ConvertXML", "ana0: " & _ana0)
            Next
            element = XmlDoc.DocumentElement.GetElementsByTagName("ana1")
            For Each noeud In element
                _ana1 = CInt(noeud.InnerText)
                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ConvertXML", "ana1: " & _ana1)
            Next
            element = XmlDoc.DocumentElement.GetElementsByTagName("ana2")
            For Each noeud In element
                _ana2 = CBool(noeud.InnerText)
                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ConvertXML", "ana2: " & _ana2)
            Next
            element = XmlDoc.DocumentElement.GetElementsByTagName("ana3")
            For Each noeud In element
                _ana3 = noeud.InnerText
                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ConvertXML", "ana3: " & _ana3)
            Next

            RemoveHandler serialPortObj.DataReceived, AddressOf OnReceive
            _Lock = False
            XmlDoc = Nothing
            element = Nothing
            noeud = Nothing
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ConvertXML", "Exception : " & ex.Message)

            _cntessai += 1
            If _cntessai <= _maxessai And _uuidArduino = "" Then
                _Lock = False
                _uuidArduino = String.Empty
                msg = String.Empty
                GetStatus()
                Threading.Thread.Sleep(1000)
            End If
        End Try
    End Sub

#End Region


End Class
