Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports STRGS = Microsoft.VisualBasic.Strings
Imports VB = Microsoft.VisualBasic
Imports System.IO.Ports
Imports System.Math
Imports System.Net.Sockets
Imports System.Threading
Imports System.Globalization

' Auteur : David
' Date : 22/01/2011

''' <summary>Class Driver_RFXComReceiver, permet de communiquer avec un RFXCOM Ethernet ou USB</summary>
''' <remarks>Pour la version USB, necessite l'installation du driver USB RFXCOM</remarks>
<Serializable()> Public Class Driver_RFXComReceiver
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "3B808B6C-25B3-11E0-A6DB-36D2DED72085"
    Dim _Nom As String = "RFXCom_receiver"
    Dim _Enable As String = False
    Dim _Description As String = "RFXCom Receiver COM, USB or Ethernet"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "RF"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = ""
    Dim _Port_TCP As String = "10001"
    Dim _IP_UDP As String = ""
    Dim _Port_UDP As String = ""
    Dim _Com As String = ""
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "RFXCom"
    Dim _Version As String = "1.0"
    Dim _Picture As String = "rfxcom.png"
    Dim _Server As HoMIDom.HoMIDom.Server
    Dim _Device As HoMIDom.HoMIDom.Device
    Dim _DeviceSupport As New ArrayList
    Dim _Parametres As New ArrayList
    Dim MyTimer As New Timers.Timer
    Dim _IdSrv As String

#End Region

#Region "Variables Internes"
    Private WithEvents port As New System.IO.Ports.SerialPort
    Private port_name As String = ""
    'Liste des commandes
    Public Const SWVERS As Byte = &H20
    Public Const MODEHS As Byte = &H21
    Public Const MODEKOP As Byte = &H23
    Public Const MODEARC As Byte = &H24
    Public Const MODEBD As Byte = &H25
    Public Const MODEB32 As Byte = &H29
    Public Const MODEVISONIC As Byte = &H40
    Public Const MODENOXLAT As Byte = &H41
    Public Const MODEVISAUX As Byte = &H42
    Public Const MODEVAR As Byte = &H2C
    Public Const ENALL As Byte = &H2A
    Public Const DISARC As Byte = &H2D
    Public Const DISKOP As Byte = &H2E
    Public Const DISX10 As Byte = &H2F
    Public Const DISHE As Byte = &H28
    Public Const DISOREGON As Byte = &H43
    Public Const DISATI As Byte = &H44
    Public Const DISVIS As Byte = &H45
    Public Const DISSOMFY As Byte = &H46
    Public Const DISEU As Byte = &H47
    'liste des variables de base
    Private slave As Boolean
    Private mess As Boolean = False
    Private currentdevice As Byte
    Private currentunit As Byte
    Private pulse As Char
    Private firstbyte As Boolean = True
    Private protocolmode As Byte = MODEVAR
    Private recbuf(30), recbytes, recbits As Byte
    Private bytecnt As Integer = 0
    Private message As String
    Private dateheurelancement As DateTime
    Private waitforack As Boolean = False
    Private vresponse As Boolean = False
    Private supply_voltage As Integer
    Private temperature As Single
    Private maxticks As Byte = 0
    Private simulate As Boolean
    Private TCPData(1024) As Byte
    Private client As TcpClient
    Private tcp As Boolean
    Private stream As NetworkStream
    Private WithEvents tmrRead As New System.Timers.Timer
    Private messagetemp, messagelast, adresselast, valeurlast, recbuf_last As String
    Private nblast As Integer = 0
    Private BufferIn(8192) As Byte
#End Region

#Region "Propriétés génériques"
    Public WriteOnly Property IdSrv As String Implements HoMIDom.HoMIDom.IDriver.IdSrv
        Set(ByVal value As String)
            _IdSrv = value
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
    Public ReadOnly Property DeviceSupport() As ArrayList Implements HoMIDom.HoMIDom.IDriver.DeviceSupport
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
    Public ReadOnly Property Modele() As String Implements HoMIDom.HoMIDom.IDriver.Modele
        Get
            Return _Modele
        End Get
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
    Public Property Refresh() As Integer Implements HoMIDom.HoMIDom.IDriver.Refresh
        Get
            Return _Refresh
        End Get
        Set(ByVal value As Integer)
            _Refresh = value
            If _Refresh > 0 Then
                MyTimer.Interval = _Refresh
                MyTimer.Enabled = True
                AddHandler MyTimer.Elapsed, AddressOf TimerTick
            End If
        End Set
    End Property
    Public ReadOnly Property Version() As String Implements HoMIDom.HoMIDom.IDriver.Version
        Get
            Return _Version
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

    ''' <summary>Démarrer le du driver</summary>
    ''' <remarks></remarks>
    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        '_IsConnect = True
        Dim retour As String
        Try
            'ouverture du port suivant le Port Com ou IP
            If _Com <> "" Then
                retour = ouvrir(_Com)
            ElseIf _IP_TCP <> "" Then
                retour = ouvrir(_IP_TCP)
            Else
                retour = "ERR: Port Com ou IP_TCP non défini. Impossible d'ouvrir le port !"
            End If
            'traitement du message de retour
            If STRGS.Left(retour, 4) = "ERR:" Then
                retour = STRGS.Right(retour, retour.Length - 5)
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXCOM_RECEIVER", "Driver non démarré : " & retour)
            Else
                'le driver est démarré, on log puis on lance les handlers
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFXCOM_RECEIVER", "Driver démarré : " & retour)
                retour = lancer()
                If STRGS.Left(retour, 4) = "ERR:" Then
                    retour = STRGS.Right(retour, retour.Length - 5)
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXCOM_RECEIVER", retour & " non lancé, arrêt du driver")
                    [Stop]()
                Else
                    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFXCOM_RECEIVER", retour)
                    'les handlers sont lancés, on configure le rfxcom
                    retour = configurer()
                    If STRGS.Left(retour, 4) = "ERR:" Then
                        retour = STRGS.Right(retour, retour.Length - 5)
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXCOM_RECEIVER", retour)
                        [Stop]()
                    Else
                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFXCOM_RECEIVER", retour)
                    End If
                End If
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXCOM_RECEIVER Start", ex.Message)
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Dim retour As String
        retour = fermer()
        If STRGS.Left(retour, 4) = "ERR:" Then
            retour = STRGS.Right(retour, retour.Length - 5)
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXCOM_RECEIVER", retour)
        Else
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFXCOM_RECEIVER", retour)
        End If
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
        'pas utilisé
        If _Enable = False Then Exit Sub
    End Sub

    ''' <summary>Commander un device</summary>
    ''' <param name="Objet">Objet représetant le device à interroger</param>
    ''' <param name="Command">La commande à passer</param>
    ''' <param name="Parametre1"></param>
    ''' <param name="Parametre2"></param>
    ''' <remarks></remarks>
    Public Sub Write(ByVal Objet As Object, ByVal Command As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
        If _Enable = False Then Exit Sub
        'command pas utiliser car String, on utilise donc Parametre1 pour transmettre les commandes style MODEB32
        ecrire(&HF0, Parametre1)
    End Sub

    ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice

    End Sub

    ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice

    End Sub

    ''' <summary>Creation d'un objet de type</summary>
    ''' <remarks></remarks>
    Public Sub New()
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
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick()

    End Sub

#End Region

#Region "Fonctions Internes"

    ''' <summary>Ouvrir le port COM/ETHERNET</summary>
    ''' <param name="numero">Nom/Numero du port COM/Adresse IP: COM2</param>
    ''' <remarks></remarks>
    Private Function ouvrir(ByVal numero As String) As String
        'Forcer le . 
        Thread.CurrentThread.CurrentCulture = New CultureInfo("en-US")
        My.Application.ChangeCulture("en-US")
        Try
            If Not _IsConnect Then
                port_name = numero 'pour se rapeller du nom du port
                If VB.Left(numero, 3) <> "COM" Then
                    'RFXCOM est un modele ethernet
                    tcp = True
                    client = New TcpClient(numero, _Port_TCP)
                    _IsConnect = True
                    Return ("Port IP " & port_name & ":" & _Port_TCP & " ouvert")
                Else
                    'RFXCOM est un modele usb
                    tcp = False
                    port.PortName = port_name 'nom du port : COM1
                    port.BaudRate = 38400 'vitesse du port 300, 600, 1200, 2400, 9600, 14400, 19200, 38400, 57600, 115200
                    port.Parity = Parity.None 'pas de parité
                    port.StopBits = StopBits.One 'un bit d'arrêt par octet
                    port.DataBits = 8 'nombre de bit par octet
                    port.Encoding = System.Text.Encoding.GetEncoding(1252)  'Extended ASCII (8-bits)
                    port.Handshake = Handshake.None
                    port.ReadBufferSize = CInt(8192)
                    port.ReceivedBytesThreshold = 1
                    port.ReadTimeout = 100
                    port.WriteTimeout = 500
                    port.Open()
                    _IsConnect = True
                    If port.IsOpen Then
                        port.DtrEnable = True
                        port.RtsEnable = True
                        port.DiscardInBuffer()
                    End If
                    dateheurelancement = DateTime.Now
                    Return ("Port " & port_name & " ouvert")
                End If
            Else
                Return ("Port " & port_name & " dejà ouvert")
            End If
        Catch ex As Exception
            Return ("ERR: " & ex.Message)
        End Try
    End Function

    ''' <summary>Lances les handlers sur le port</summary>
    ''' <remarks></remarks>
    Private Function lancer() As String
        'lancer les handlers
        If tcp Then
            Try
                stream = client.GetStream()
                stream.BeginRead(TCPData, 0, 1024, AddressOf TCPDataReceived, Nothing)
                Return "Handler IP OK"
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXCOM_RECEIVER LANCER GETSTREAM", ex.Message)
                Return "ERR: Handler IP"
            End Try
        Else
            Try
                tmrRead.Enabled = True
                AddHandler port.DataReceived, New SerialDataReceivedEventHandler(AddressOf DataReceived)
                AddHandler port.ErrorReceived, New SerialErrorReceivedEventHandler(AddressOf ReadErrorEvent)
                Return "Handler COM OK"
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXCOM_RECEIVER LANCER Serial", ex.Message)
                Return "ERR: Handler COM"
            End Try
        End If
    End Function

    ''' <summary>Configurer le RFXCOM</summary>
    ''' <remarks></remarks>
    Private Function configurer() As String
        'configurer le rfxcom
        Try
            ecrire(&HF0, MODEVAR)
            ecrire(&HF0, ENALL)
            'ecrire(&HF0, DISOREGON)
            'ecrire(&HF0, DISATI)
            ecrire(&HF0, DISSOMFY) 'disable SOMFY car pas encore géré
            'ecrire(&HF0, DISVIS)
            'ecrire(&HF0, DISHE)
            'ecrire(&HF0, DISKOP)
            'ecrire(&HF0, DISARC)
            Return "Configuration OK"
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXCOM_RECEIVER LANCER Configuration", ex.Message)
            Return "ERR: Configuration " & ex.Message
        End Try
    End Function

    ''' <summary>Ferme la connexion au port</summary>
    ''' <remarks></remarks>
    Private Function fermer() As String
        Try
            If _IsConnect Then
                'fermeture des ports
                If tcp Then
                    _IsConnect = False
                    client.Close()
                    stream.Close()
                    Return ("Port IP fermé")
                Else
                    _IsConnect = False
                    'suppression de l'attente de données à lire
                    RemoveHandler port.DataReceived, AddressOf DataReceived
                    RemoveHandler port.ErrorReceived, AddressOf ReadErrorEvent
                    If (Not (port Is Nothing)) Then ' The COM port exists.
                        If port.IsOpen Then
                            Dim limite As Integer = 0
                            'vidage des tampons
                            port.DiscardInBuffer()
                            port.DiscardOutBuffer()
                            'au cas on verifie si encore quelque chose à lire
                            Do While (port.BytesToWrite > 0 And limite < 100) ' Wait for the transmit buffer to empty.
                                limite = limite + 1
                            Loop
                            limite = 0
                            Do While (port.BytesToRead > 0 And limite < 100) ' Wait for the receipt buffer to empty.
                                limite = limite + 1
                            Loop
                            port.Close()
                            port.Dispose()
                            Return ("Port " & port_name & " fermé")
                        End If
                        Return ("Port " & port_name & "  est déjà fermé")
                    End If
                    Return ("Port " & port_name & " n'existe pas")
                End If
            End If
        Catch ex As UnauthorizedAccessException
            Return ("ERR: Port " & port_name & " IGNORE") ' The port may have been removed. Ignore.
        Catch ex As Exception
            Return ("ERR: Port " & port_name & " : " & ex.Message)
        End Try
        Return True
    End Function

    ''' <summary>ecrire sur le port</summary>
    ''' <param name="commande">premier paquet à envoyer</param>
    ''' <param name="commande2">deuxieme paquet à envoyer</param>
    ''' <remarks></remarks>
    Public Function ecrire(ByVal commande As Byte, ByVal commande2 As Byte) As String
        Dim cmd() As Byte = {commande, commande2}
        Dim message As String = ""
        Try
            Select Case commande2
                Case MODEB32
                    protocolmode = MODEB32
                    message = "Init cmd to receiver => F0" & VB.Right("0" & Hex(MODEB32), 2) & " Mode: 32 bit"
                Case MODEVAR
                    protocolmode = MODEVAR
                    message = "Init cmd to receiver => F0" & VB.Right("0" & Hex(MODEVAR), 2) & " Mode: Variable length mode"
                Case MODEKOP
                    protocolmode = MODEKOP
                    message = "Init cmd to receiver => F0" & VB.Right("0" & Hex(MODEKOP), 2) & " Mode: 24 bit KOPPLA"
                Case MODEARC
                    protocolmode = MODEARC
                    message = "Init cmd to receiver => F0" & VB.Right("0" & Hex(MODEARC), 2) & "Mode: Arc"
                Case MODEHS
                    protocolmode = MODEHS
                    message = "Init cmd to receiver => F0" & VB.Right("0" & Hex(MODEHS), 2) & "Mode: RFXCOM-HS plugin mode"
                Case MODEVISONIC
                    protocolmode = MODEVISONIC
                    message = "Init cmd to receiver => F0" & VB.Right("0" & Hex(MODEVISONIC), 2) & "Mode: Visonic only"
                Case MODENOXLAT
                    protocolmode = MODENOXLAT
                    message = "Init cmd to receiver => F0" & VB.Right("0" & Hex(MODENOXLAT), 2) & "Mode: Visonic & Variable mode"
                Case MODEBD : message = "Init cmd to receiver => F0" & VB.Right("0" & Hex(MODEBD), 2) & "Mode: Bd"
                Case MODEVISAUX : message = "Init cmd to receiver => F0" & VB.Right("0" & Hex(MODEVISAUX), 2) & "Mode: Visionic AUX"
                Case DISKOP : message = "Disable Koppla RF => F0" & VB.Right("0" & Hex(DISKOP), 2)
                Case DISX10 : message = "Disable X10 RF => F0" & VB.Right("0" & Hex(DISX10), 2)
                Case DISARC : message = "Disable ARC RF => F0" & VB.Right("0" & Hex(DISARC), 2)
                Case DISOREGON : message = "Disable Oregon RF => F0" & VB.Right("0" & Hex(DISOREGON), 2)
                Case DISATI : message = "Disable ATI Wonder RF => F0" & VB.Right("0" & Hex(DISATI), 2)
                Case DISHE : message = "Disable HomeEasy RF => F0" & VB.Right("0" & Hex(DISHE), 2)
                Case DISVIS : message = "Disable Visonic RF => F0" & VB.Right("0" & Hex(DISVIS), 2)
                Case DISSOMFY : message = "Disable Somfy RF => F0" & VB.Right("0" & Hex(DISSOMFY), 2)
                Case ENALL : message = "Enable ALL RF => F0" & VB.Right("0" & Hex(ENALL), 2)
                Case SWVERS
                    maxticks = 0
                    firstbyte = True
                    waitforack = True
                    vresponse = True
                    message = "Version request to receiver => F0" & VB.Right("0" & Hex(SWVERS), 2)
            End Select

            If tcp Then
                stream.Write(cmd, 0, cmd.Length)
            Else
                port.Write(cmd, 0, cmd.Length) 'port.Write(Chr(commande)) port.Write(Chr(commande2))
            End If
            waitforack = True
            Return message
        Catch ex As Exception
            Return ("ERR: " & ex.Message)
        End Try
    End Function

    ''' <summary>Executer lors de la reception d'une donnée sur le port</summary>
    ''' <remarks></remarks>
    Private Sub DataReceived(ByVal sender As Object, ByVal e As SerialDataReceivedEventArgs)
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
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXCOM_RECEIVER Datareceived", Ex.Message)
        End Try
    End Sub

    ''' <summary>Executer lors de la reception d'une erreur sur le port</summary>
    ''' <remarks></remarks>
    Private Sub ReadErrorEvent(ByVal sender As Object, ByVal ev As SerialErrorReceivedEventArgs)
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
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXCOM_RECEIVER ERRORDatareceived", Ex.Message)
        End Try
    End Sub

    ''' <summary>Executer lors de la reception d'une donnée sur le port IP</summary>
    ''' <remarks></remarks>
    Private Sub TCPDataReceived(ByVal ar As IAsyncResult)
        Dim intCount As Integer
        Try
            If _IsConnect Then
                intCount = stream.EndRead(ar)
                ProcessNewTCPData(TCPData, 0, intCount)
                stream.BeginRead(TCPData, 0, 1024, AddressOf TCPDataReceived, Nothing)
            End If
        Catch Ex As Exception
            WriteLog("ERR: RFXCOM TCPDatareceived : " & Ex.Message)
        End Try
    End Sub

    ''' <summary>Traite les données IP recu</summary>
    ''' <remarks></remarks>
    Private Sub ProcessNewTCPData(ByVal Bytes() As Byte, ByVal offset As Integer, ByVal count As Integer)
        Dim intIndex As Integer
        Try
            For intIndex = offset To offset + count - 1
                ProcessReceivedChar(Bytes(intIndex))
            Next
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXCOM_RECEIVER ProcessNewTCPData", ex.Message)
        End Try
    End Sub

    ''' <summary>xxx</summary>
    ''' <remarks></remarks>
    Private Sub tmrRead_Elapsed(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrRead.Elapsed
        Try
            If Not firstbyte Then
                maxticks += 1
            End If
            If maxticks > 3 Then 'flush buffer due to 400ms timeout
                maxticks = 0
                firstbyte = True
                If vresponse = True Then  'display version?
                    If (bytecnt = 2) Then
                        display_mess()
                    End If
                Else
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXCOM_RECEIVER", "Buffer flushed due to timeout")
                End If
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXCOM_RECEIVER tmrRead_Elapsed", ex.Message)
        End Try
    End Sub

    ''' <summary>Rassemble un message complet pour ensuite l'envoyer à displaymess</summary>
    ''' <param name="temp">Byte recu</param>
    ''' <remarks></remarks>
    Public Sub ProcessReceivedChar(ByVal temp As Byte)
        Try
            'Dim temp As Byte

            maxticks = 0
            If firstbyte Then
                firstbyte = False
                bytecnt = 0
            End If
            'WriteLog(VB.Right("0" & Hex(temp), 2))
            If Not waitforack Then
                If vresponse = True Then  'display version?
                    recbuf(bytecnt) = temp
                    If (bytecnt = 3) Then mess = True
                Else
                    If bytecnt = 15 Then
                        recbuf(bytecnt - 1) = temp
                        mess = True
                    ElseIf bytecnt = 0 Then
                        recbits = temp And &H7F
                        If (temp And &H80) = 0 Then slave = False Else slave = True
                        If (recbits And &H7) = 0 Then recbytes = ((recbits And &H7F) >> 3) Else recbytes = ((recbits And &H7F) >> 3) + 1
                    ElseIf bytecnt = recbytes Then
                        recbuf(bytecnt - 1) = temp
                        bytecnt -= 1
                        mess = True
                    Else
                        recbuf(bytecnt - 1) = temp
                    End If
                End If
                bytecnt += 1
            Else
                mess = True
            End If
            If mess Then
                'gestion pour ne pas gérer deux fois le même paquet car la norme veut que les paquets soient envoyé deux fois de suite
                Dim xxx As String = ""
                For i As Integer = 0 To bytecnt
                    'If Not domos_svc.Serv_DOMOS Then Exit Sub 'si on quitte Domos on quitte cette fonction
                    xxx = xxx & (VB.Right("0" & Hex(recbuf(i)), 2))
                Next
                If recbuf_last <> xxx Or nblast >= 2 Then 'nouveau paquet ou c'est la troisieme fois qu'on le recoit 
                    display_mess()
                    recbuf_last = xxx
                    nblast = 1
                Else 'c'est la deuxieme paquet indentique qu'on recoit, on l'ignore
                    nblast = 2
                    firstbyte = True
                    mess = False
                End If
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXCOM_RECEIVER ProcessReceivedChar", ex.Message)
        End Try
    End Sub

    ''' <summary>Decode le message pour l'envoyer aux bonnes fonctions de traitement</summary>
    ''' <remarks></remarks>
    Public Sub display_mess()
        Try
            If Not _IsConnect Then Exit Sub 'si on ferme le port on quitte cette boucle
            'interprete le message recu
            Dim parity As Integer
            Dim rfxsensor, rfxpower As Boolean
            Dim logtemp As String = ""
            mess = False
            firstbyte = True

            ''affichage de la chaine reçu
            'Dim valtemp As String = ""
            'Dim xxx As String = ""
            'For i As Integer = 0 To bytecnt
            '    valtemp = valtemp & recbuf(i)
            '    xxx = xxx & (VB.Right("0" & Hex(recbuf(i)), 2))
            'Next
            'WriteLog(xxx)

            If Not waitforack Then
                If bytecnt = 4 Then
                    parity = Not (((recbuf(0) And &HF0) >> 4) + (recbuf(0) And &HF) + (recbuf(1) >> 4) + (recbuf(1) And &HF) + (recbuf(2) >> 4) + (recbuf(2) And &HF) + (recbuf(3) >> 4)) And &HF
                    If (parity = (recbuf(3) And &HF)) And (recbuf(0) + (recbuf(1) Xor &HF) = &HFF) Then rfxsensor = True Else rfxsensor = False
                ElseIf bytecnt = 6 Then
                    parity = Not ((recbuf(0) >> 4) + (recbuf(0) And &HF) + (recbuf(1) >> 4) + (recbuf(1) And &HF) + (recbuf(2) >> 4) + (recbuf(2) And &HF) + (recbuf(3) >> 4) + (recbuf(3) And &HF) + (recbuf(4) >> 4) + (recbuf(4) And &HF) + (recbuf(5) >> 4)) And &HF
                    If (parity = (recbuf(5) And &HF)) And (recbuf(0) + (recbuf(1) Xor &HF) = &HFF) Then rfxpower = True Else rfxpower = False
                End If

                If vresponse Then  'display version
                    vresponse = False
                    recbits = 0
                    If recbuf(0) = Asc("M") Then
                        logtemp = " Version Master=" & VB.Right("0" & Hex(recbuf(1)), 2)
                        If bytecnt > 3 Then
                            If recbuf(2) = Asc("S") Then
                                logtemp = logtemp & " Slave=" & VB.Right("0" & Hex(recbuf(3)), 2)
                            Else
                                logtemp = logtemp & VB.Right("0" & Hex(recbuf(2)), 2)
                            End If
                        End If
                    ElseIf recbuf(0) = Asc("S") Then
                        logtemp = " Version Slave=" & VB.Right("0" & Hex(recbuf(1)), 2)
                        If bytecnt > 3 Then
                            If recbuf(2) = Asc("M") Then
                                logtemp = logtemp & " Master=" & VB.Right("0" & Hex(recbuf(3)), 2)
                            Else
                                logtemp = logtemp & VB.Right("0" & Hex(recbuf(2)), 2)
                            End If
                        End If
                    Else
                        logtemp = "Version " & VB.Right("0" & Hex(recbuf(0)), 2)
                        If bytecnt > 3 Then
                            If recbuf(1) = Asc("S") Then
                                logtemp = logtemp & " Slave=" & VB.Right("0" & Hex(recbuf(2)), 2)
                            ElseIf recbuf(1) = Asc("M") Then
                                logtemp = logtemp & " Master=" & VB.Right("0" & Hex(recbuf(2)), 2)
                            Else
                                logtemp = logtemp & " " & VB.Right("0" & Hex(recbuf(1)), 2)
                            End If
                        ElseIf bytecnt = 3 Then
                            logtemp = logtemp & " " & VB.Right("0" & Hex(recbuf(1)), 2)
                        End If
                    End If
                    WriteLog(logtemp)
                    'ElseIf protocolmode = MODEARC Then : processARC()
                    'ElseIf protocolmode = MODEKOP Then : processkoppla()
                ElseIf rfxsensor Then : processrfxsensor()
                ElseIf rfxpower Then : processrfxmeter()
                    'ElseIf protocolmode = MODEVISONIC Then : processvisonic(recbits)
                ElseIf protocolmode = MODEVAR And recbits = 20 Then : processati()
                ElseIf protocolmode = MODEVAR And recbits = 21 Then : processatiplus()
                ElseIf protocolmode = MODEVAR And (recbits = 12 Or recbits = 34 Or recbits = 38) Then : processhe()
                ElseIf protocolmode = MODEVAR And recbits = 56 Then : processsomfy()
                ElseIf protocolmode = MODEVAR And (recbits = 56 Or recbits > 59) Then : processoregon(recbits)
                    '    ElseIf protocolmode = MODENOXLAT Then
                    '    If recbits = 36 Or recbits = 66 Or recbits = 72 Then
                    '        processvisonic(recbits)
                    '    ElseIf recbits > 59 Then
                    '        If processoregon(recbits) = False Then
                    '            processvisonic(recbits)
                    '        End If
                    '    Else
                    '        processx(recbits)
                    '    End If
                Else
                    processx(recbits)
                End If
                'If (protocolmode = MODEVAR Or protocolmode = MODENOXLAT) And recbits <> 0 Then
                '    If slave Then
                '        WriteMessage(" bits=" & Convert.ToString(recbits) & " from SLAVE", False)
                '    Else
                '        WriteMessage(" bits=" & Convert.ToString(recbits), False)
                '    End If
                'End If
            Else
                WriteLog("ACK")
            End If
            waitforack = False
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXCOM_RECEIVER display_mess", ex.ToString)
        End Try
    End Sub

#End Region

#Region "Process"
    'OK
    Private Sub processx(ByVal recbits As Byte)
        Try
            Dim adresse As String = ""
            Dim valeur As String = ""

            'Dim hsaddr As Integer
            If ((recbuf(0) Xor recbuf(1)) = &HFF) And (recbuf(0) = &HEE) Then
                processrfremote()
            ElseIf recbuf(3) = 0 And (recbuf(2) And &HF) = 0 Then
                processatiplus()
            ElseIf ((recbuf(0) Xor recbuf(1)) = &HFF) And ((recbuf(2) Xor recbuf(3)) = &HFF) Then
                processx10()
            ElseIf recbits = 32 And ((recbuf(0) Xor recbuf(1)) = &HFE) And ((recbuf(2) Xor recbuf(3)) = &HFF) Then
                processdm10()
            ElseIf (recbuf(0) = ((recbuf(1) And &HF0) + (&HF - (recbuf(1) And &HF)))) And ((recbuf(2) Xor recbuf(3)) = &HFF) Then
                processx10security()
            ElseIf ((recbuf(2) Xor recbuf(3)) = &HFF) Then
                adresse = (recbuf(0) * 256 + recbuf(1)).ToString
                Select Case recbuf(2)
                    Case &H44 : valeur = "ON" '"S DWS Visonic door sensor Alert + Tamper"
                    Case &HC4 : valeur = "OFF" '"S DWS Visonic door sensor Normal + Tamper)"
                    Case &H4 : valeur = "ON" '"S DWS Visonic door sensor Alert "
                    Case &H5
                        valeur = "ON" '"S DWS Visonic door sensor Alert (battery low)"
                        WriteBattery(adresse)
                    Case &H84 : valeur = "OFF" '"S DWS Visonic door sensor Normal"
                    Case &H85
                        valeur = "OFF" '"S DWS Visonic door sensor Normal (battery low)"
                        WriteBattery(adresse)
                    Case &H4C : valeur = "ON" '"S DWS Visonic motion sensor Alert + Tamper"
                    Case &HCC : valeur = "OFF" '"S DWS Visonic motion sensor Normal + Tamper)"
                    Case &HC : valeur = "ON" '"S DWS Visonic motion sensor Alert "
                    Case &HD
                        valeur = "ON" '"S DWS Visonic motion sensor Alert (battery low)"
                        WriteBattery(adresse)
                    Case &H8C : valeur = "OFF" '"S DWS Visonic motion sensor Normal"
                    Case &H8D
                        valeur = "OFF" '"S DWS Visonic motion sensor Normal (battery low)"
                        WriteBattery(adresse)
                    Case &HE0
                        If recbuf(0) = &HFF Then
                            WriteLog("X10Security : Master receiver jamming detected")
                        ElseIf recbuf(0) = &H0 Then
                            WriteLog("X10Security : Slave receiver jamming detected")
                        Else
                            WriteLog("X10Security : ERR: Unknown data packet received")
                        End If
                    Case &HF8
                        If recbuf(0) = &HFF Then
                            WriteLog("X10Security : Master receiver end jamming detected")
                        ElseIf recbuf(0) = &H0 Then
                            WriteLog("X10Security : Slave receiver end jamming detected")
                        Else
                            WriteLog("ERR: unknown cmd")
                        End If
                    Case &H2 : valeur = "S REMOTE Visonic keyfob ARM Away (max)"
                    Case &HE : valeur = "S REMOTE Visonic keyfob ARM Home (min)"
                    Case &H22 : valeur = "S REMOTE Visonic keyfob Panic   "
                    Case &H42 : valeur = "S REMOTE Visonic keyfob Lights On"
                    Case &H82 : valeur = "S REMOTE Visonic keyfob Disarm  "
                    Case Else : WriteLog("ERR: Unknown data packet received")
                End Select
                'hsaddr = createhsaddr()
                'If protocolmode = MODEB32 Then
                '    WriteLog("X10Security : addr:" & VB.Right("0" & Hex(recbuf(0)), 2) & " ID:" & VB.Right("    " & Str(hsaddr), 5))
                'Else
                '    WriteLog("X10Security : addr:" & VB.Right("0" & Hex(recbuf(0)), 2) & VB.Right("0" & Hex(recbuf(1)), 2) & " " & VB.Right("0" & Hex(recbuf(4)), 2) & " ID:" & VB.Right("    " & Str(hsaddr), 5))
                'End If
                If valeur <> "" Then WriteRetour(adresse, "", valeur)

            ElseIf protocolmode = MODEVAR Or protocolmode = MODENOXLAT Then
                If recbits = 32 And recbuf(0) = &H52 And recbuf(1) = &H46 Then
                    Select Case recbuf(2)
                        Case &H58 : valeur = "RFXSensor Type-1"
                        Case &H32 : valeur = "RFXSensor Type-2"
                        Case &H33 : valeur = "RFXSensor Type-3"
                        Case &H34 : valeur = "RFXVamp"
                        Case Else : valeur = "Unknown RFXSensor"
                    End Select
                    If recbuf(2) <> &H34 Then
                        If (recbuf(3) And &H80) = 0 Then valeur = valeur & " Fast sampling mode" Else valeur = valeur & " Slow sampling mode"
                    End If
                    valeur = valeur & " version:" & CStr(recbuf(3) And &H7F)
                    WriteLog(valeur)
                ElseIf recbits = 40 And recbuf(0) = &H53 And recbuf(1) = &H45 And recbuf(2) = &H4E Then
                    valeur = "Sensor " & Hex(recbuf(3)) & " type="
                    Select Case recbuf(4)
                        Case &H26 : valeur = valeur & "DS2438"
                        Case &H28 : valeur = valeur & "DS18B20"
                        Case Else : valeur = "ERR: Processx : unknown 1-Wire sensor"
                    End Select
                    WriteLog(valeur)
                ElseIf recbits = 48 Then
                    WriteLog("DBG: Processx : noise or a 1-Wire sensor internal address")
                ElseIf (((recbuf(2) And &HF0) = &H10) Or ((recbuf(2) And &HF0) = &H20)) And recbits = 44 Then
                    processdigimax()
                Else
                    WriteLog("DBG: Processx : noise or an unknown data packet received")
                End If
            Else
                WriteLog("ERR: Processx : Unknown data packet received")
            End If
        Catch ex As Exception
            WriteLog("ERR: RFXCOM processx : " & ex.Message)
        End Try

    End Sub

    'OK 
    Private Sub processhe()
        Try
            Dim adresse, valeur As String
            'WriteLog("ERR: Process HE pas encore géré")
            'WriteMessage(" HE", False)
            'WriteMessage(" Device=" & Hex(recbuf(0) >> 6), False)
            'WriteMessage(VB.Right("0" & Hex((recbuf(0) << 2 Or recbuf(1) >> 6) And &HFF), 2), False)
            'WriteMessage(VB.Right("0" & Hex((recbuf(1) << 2 Or recbuf(2) >> 6) And &HFF), 2), False)
            'WriteMessage(VB.Right("0" & Hex((recbuf(2) << 2 Or recbuf(3) >> 6) And &HFF), 2), False)
            'WriteMessage(" Unit=" & CStr((recbuf(3) And &HF) + 1), False)
            adresse = Hex(recbuf(0) >> 6) & VB.Right("0" & Hex((recbuf(0) << 2 Or recbuf(1) >> 6) And &HFF), 2) & VB.Right("0" & Hex((recbuf(1) << 2 Or recbuf(2) >> 6) And &HFF), 2) & VB.Right("0" & Hex((recbuf(2) << 2 Or recbuf(3) >> 6) And &HFF), 2) & "-" & CStr((recbuf(3) And &HF) + 1)
            If recbits = 34 Then
                Select Case recbuf(3) And &H30
                    Case &H0 : WriteRetour(adresse, "", "OFF")
                    Case &H10 : WriteRetour(adresse, "", "ON")
                    Case &H20 : WriteRetour(adresse, "", "GROUP OFF")
                    Case &H30 : WriteRetour(adresse, "", "GROUP ON")
                End Select
            Else
                valeur = ""
                Select Case recbuf(4) And &HC
                    Case &H0
                        valeur = "ERR: preset record length without preset bits set"
                    Case &H4
                        If (recbuf(3) And &H20) = &H0 Then
                            valeur = "CFG: Preset command"
                        Else
                            valeur = "CFG: Preset group command"
                        End If
                    Case &H8 : valeur = "CFG: Reserved (unexpected)"
                    Case &HC : valeur = "CFG: Reserved (unexpected)"
                End Select
                WriteRetour(adresse, "", "CFG: " & valeur & " Level=" & CStr((recbuf(4) >> 4) + 1))
            End If
        Catch ex As Exception
            WriteLog("ERR: RFXCOM processhe : " & ex.Message)
        End Try
    End Sub

    'OK
    Private Sub processati()
        Try
            Dim remote As Integer
            Dim adresse, valeur As String
            If recbuf(0) > recbuf(1) Then remote = recbuf(0) - recbuf(1) Else remote = (recbuf(0) + &H100) - recbuf(1)
            adresse = (recbuf(2) >> 4).ToString
            'WriteMessage(" ATI[" & (recbuf(2) >> 4).ToString & "]C Remote type= ", False)
            'Select Case remote
            '    Case &HC5
            '        WriteMessage("ATI Remote Wonder", False)
            '    Case &HD5
            '        WriteMessage("Medion", False)
            '    Case Else
            '        WriteMessage("Unknown:" & Hex(remote), False)
            'End Select
            'WriteMessage(" Channel=" & Str(recbuf(2) >> 4), False)
            Select Case recbuf(1)
                Case &H0 : If remote = &HC5 Then valeur = "A" Else valeur = "Mute"
                Case &H1 : valeur = "B"
                Case &H2 : valeur = "power"
                Case &H3 : valeur = "TV"
                Case &H4 : valeur = "DVD"
                Case &H5 : If remote = &HC5 Then valeur = "WEB" Else valeur = "PHOTO"
                Case &H6 : If remote = &HC5 Then valeur = "GUIDE" Else valeur = "MUSIC"
                Case &H7 : valeur = "DRAG"
                Case &H8 : If remote = &HC5 Then valeur = "VOL+" Else valeur = "VOL-"
                Case &H9 : If remote = &HC5 Then valeur = "VOL-" Else valeur = "VOL+"
                Case &HA : valeur = "MUTE"
                Case &HB : valeur = "CHAN+"
                Case &HC : valeur = "CHAN-"
                Case &HD : valeur = "1"
                Case &HE : valeur = "2"
                Case &HF : valeur = "3"
                Case &H10 : valeur = "4"
                Case &H11 : valeur = "5"
                Case &H12 : valeur = "6"
                Case &H13 : valeur = "7"
                Case &H14 : valeur = "8"
                Case &H15 : valeur = "9"
                Case &H16 : valeur = "TXT"
                Case &H17 : valeur = "0"
                Case &H18 : valeur = "SNAPSHOT"
                Case &H19 : If remote = &HC5 Then valeur = "C" Else valeur = "DVD MENU"
                Case &H1A : valeur = "CURSOR-UP"
                Case &H1B : If remote = &HC5 Then valeur = "D" Else valeur = "SETUP"
                Case &H1C : valeur = "TV-RADIO"
                Case &H1D : valeur = "CURSOR-LEFT"
                Case &H1E : valeur = "OK"
                Case &H1F : valeur = "CURSOR-RIGHT"
                Case &H20 : valeur = "RETURN"
                Case &H21 : valeur = "E"
                Case &H22 : valeur = "CURSOR-DOWN"
                Case &H23 : valeur = "F"
                Case &H24 : valeur = "REWIND"
                Case &H25 : valeur = "PLAY"
                Case &H26 : valeur = "FAST-FORWARD"
                Case &H27 : valeur = "RECORD"
                Case &H28 : valeur = "STOP"
                Case &H29 : valeur = "PAUSE"
                Case &H2C : valeur = "TV"
                Case &H2D : valeur = "VCR"
                Case &H2E : valeur = "RADIO"
                Case &H2F : valeur = "TV-PREVIEW"
                Case &H30 : valeur = "CHANNEL-LIST"
                Case &H31 : valeur = "VIDEO-DESKTOP"
                Case &H32 : valeur = "RED"
                Case &H33 : valeur = "GREEN"
                Case &H34 : valeur = "YELLOW"
                Case &H35 : valeur = "BLUE"
                Case &H36 : valeur = "RENAME-TAB"
                Case &H37 : valeur = "SNAPSHOT"
                Case &H38 : valeur = "EDIT-IMAGE"
                Case &H39 : valeur = "FULL-SCREEN"
                Case &H3A : valeur = "DVD-AUDIO"
                Case &H70 : valeur = "MOUSE-LEFT"
                Case &H71 : valeur = "MOUSE-RIGHT"
                Case &H72 : valeur = "MOUSE-UP"
                Case &H73 : valeur = "MOUSE-DOWN"
                Case &H74 : valeur = "MOUSE-UP-LEFT"
                Case &H75 : valeur = "MOUSE-UP-RIGHT"
                Case &H76 : valeur = "MOUSE-DOWN-RIGHT"
                Case &H77 : valeur = "MOUSE-DOWN-LEFT"
                Case &H78 : valeur = "V"
                Case &H79 : valeur = "V-END"
                Case &H7C : valeur = "X"
                Case &H7D : valeur = "X-END"
                Case Else : valeur = "unknown"
            End Select
            WriteRetour(adresse, "", valeur)
        Catch ex As Exception
            WriteLog("ERR: RFXCOM processati : " & ex.Message)
        End Try
    End Sub

    'OK
    Private Sub processatiplus()
        Try
            Dim remote As Integer
            Dim adresse, valeur As String
            If recbuf(0) > recbuf(1) Then remote = recbuf(0) - recbuf(1) Else remote = (recbuf(0) + &H100) - recbuf(1)
            adresse = (recbuf(2) >> 4).ToString
            'WriteMessage(" ATIPLUS[" & (recbuf(2) >> 4).ToString & "]C Remote type= ", False)
            'Select Case remote
            '    Case &HC5 : WriteMessage("ATI Remote Wonder Plus", False)
            '    Case Else : WriteMessage("Unknown remote:" & Hex(remote), False)
            'End Select
            'WriteMessage(" Channel=" & Str(recbuf(2) >> 4), False)
            'If (recbuf(1) And &H80) = 0 Then WriteMessage(" Even ", False) Else WriteMessage(" Odd  ", False)
            Select Case (recbuf(1) And &H7F)
                Case &H0 : valeur = "A"
                Case &H1 : valeur = "B"
                Case &H2 : valeur = "POWER"
                Case &H3 : valeur = "TV"
                Case &H4 : valeur = "DVD"
                Case &H5 : valeur = "?"
                Case &H6 : valeur = "GUIDE"
                Case &H7 : valeur = "DRAG"
                Case &H8 : valeur = "VOL+"
                Case &H9 : valeur = "VOL-"
                Case &HA : valeur = "MUTE"
                Case &HB : valeur = "CHAN+"
                Case &HC : valeur = "CHAN-"
                Case &HD : valeur = "1"
                Case &HE : valeur = "2"
                Case &HF : valeur = "3"
                Case &H10 : valeur = "4"
                Case &H11 : valeur = "5"
                Case &H12 : valeur = "6"
                Case &H13 : valeur = "7"
                Case &H14 : valeur = "8"
                Case &H15 : valeur = "9"
                Case &H16 : valeur = "TXT"
                Case &H17 : valeur = "0"
                Case &H18 : valeur = "SETUP-MENU"
                Case &H19 : valeur = "C"
                Case &H1A : valeur = "CURSOR-UP"
                Case &H1B : valeur = "D"
                Case &H1C : valeur = "FM"
                Case &H1D : valeur = "CURSOR-LEFT"
                Case &H1E : valeur = "OK"
                Case &H1F : valeur = "CURSOR-RIGHT"
                Case &H20 : valeur = "MAX-RESTORE-WINDOW"
                Case &H21 : valeur = "E"
                Case &H22 : valeur = "CURSOR-DOWN"
                Case &H23 : valeur = "F"
                Case &H24 : valeur = "REWIND"
                Case &H25 : valeur = "PLAY"
                Case &H26 : valeur = "FAST-FORWARD"
                Case &H27 : valeur = "RECORD"
                Case &H28 : valeur = "STOP"
                Case &H29 : valeur = "PAUSE"
                Case &H2A : valeur = "TV2"
                Case &H2B : valeur = "CLOCK"
                Case &H2C : valeur = "I"
                Case &H2D : valeur = "ATI"
                Case &H2E : valeur = "RADIO"
                Case &H2F : valeur = "TV-PREVIEW"
                Case &H30 : valeur = "CHANNEL-LIST"
                Case &H31 : valeur = "VIDEO-DESKTOP"
                Case &H32 : valeur = "RED"
                Case &H33 : valeur = "GREEN"
                Case &H34 : valeur = "YELLOW"
                Case &H35 : valeur = "BLUE"
                Case &H36 : valeur = "RENAME-TAB"
                Case &H37 : valeur = "SNAPSHOT"
                Case &H38 : valeur = "EDIT-IMAGE"
                Case &H39 : valeur = "FULL-SCREEN"
                Case &H3A : valeur = "DVD-AUDIO"
                Case &H70 : valeur = "MOUSE-LEFT"
                Case &H71 : valeur = "MOUSE-RIGHT"
                Case &H72 : valeur = "MOUSE-UP"
                Case &H73 : valeur = "MOUSE-DOWN"
                Case &H74 : valeur = "MOUSE-UP-LEFT"
                Case &H75 : valeur = "MOUSE-UP-RIGHT"
                Case &H76 : valeur = "MOUSE-DOWN-RIGHT"
                Case &H77 : valeur = "MOUSE-DOWN-LEFT"
                Case &H78 : valeur = "LEFT-MOUSE-BUTTON"
                Case &H79 : valeur = "V-END"
                Case &H7C : valeur = "RIGHT-MOUSE-BUTTON"
                Case &H7D : valeur = "X-END"
                Case Else : valeur = "unknown"
            End Select
            WriteRetour(adresse, "", valeur)
        Catch ex As Exception
            WriteLog("ERR: RFXCOM processatiplus : " & ex.Message)
        End Try
    End Sub

    'pas géré
    Private Sub processsomfy()
        'WriteLog("Process Somfy pas encore géré")
        'WriteMessage(" Somfy ", False)
    End Sub

    'OK
    Private Sub processARC()
        Try
            Dim group As Byte
            Dim unit As Byte
            Dim housecode As Char
            Dim i As Integer
            Dim adresse, message As String
            If bytecnt = 3 Then
                group = (((recbuf(1) And &H40) >> 5) Or ((recbuf(1) And &H10) >> 4)) + 1
                unit = (((recbuf(1) And &H4) >> 1) Or (recbuf(1) And &H1)) + 1
                housecode = Chr((((recbuf(2) And &H40) >> 3) Or ((recbuf(2) And &H10) >> 2) Or ((recbuf(2) And &H4) >> 1) Or (recbuf(2) And &H1)) + &H41)
                adresse = ""
                message = ""
                If recbuf(1) = &HFF Then
                    Select Case recbuf(0)
                        Case &H54
                            'message = " GROUP Housecode=" & housecode & " Command: ON"
                            message = "ON"
                        Case &H14
                            'message = " GROUP Housecode=" & housecode & " Command: OFF"
                            message = "OFF"
                        Case &H55
                            'message = " GROUP Housecode=" & housecode & " Command: Button released"
                            message = "Button released"
                        Case Else
                    End Select
                    adresse = housecode
                Else
                    Select Case recbuf(0)
                        Case &H54
                            'message = " Housecode=" & housecode & " Group=" & Str(group) & " Unit=" & Str(unit) & " Command: ON"
                            message = "ON"
                        Case &H14
                            'message = " Housecode=" & housecode & " Group=" & Str(group) & " Unit=" & Str(unit) & " Command: OFF"
                            message = "OFF"
                        Case &H55
                            'message = " Housecode=" & housecode & " Group=" & Str(group) & " Unit=" & Str(unit) & " Command: Button released"
                            message = "Button released"
                        Case Else
                    End Select
                    adresse = housecode & Str(group) & Str(unit)
                End If
                WriteRetour(adresse, "", message)
            ElseIf bytecnt = 8 Or bytecnt = 9 Then
                message = " HomeEasy code="
                For i = 0 To (bytecnt - 1)
                    message = message & VB.Right("0" & Hex(recbuf(i)), 2)
                Next
                WriteLog(message)
            Else
                message = "Unknown code="
                For i = 0 To (bytecnt - 1)
                    message = message & VB.Right("0" & Hex(recbuf(i)), 2)
                Next
                WriteLog("ERR: " & message)
            End If
            'WriteMessage(message & " bits=" & recbits, False)
        Catch ex As Exception
            WriteLog("ERR: RFXCOM processARC : " & ex.Message)
        End Try
    End Sub

    'OK
    Private Sub processkoppla()
        Try
            Dim temp As Byte
            Dim parity, i As Integer
            Dim morech As Boolean
            Dim adresse As String
            'message = " System=" & VB.Right("0" & Trim(Str((recbuf(2) And &HF) + 1)), 2)
            'message = message & "  CH:"
            adresse = VB.Right("0" & Trim(Str((recbuf(2) And &HF) + 1)), 2) & "_"
            If (recbuf(2) And &H20) = &H20 Then
                adresse = adresse & "1"
                morech = True
            End If
            If (recbuf(2) And &H40) <> 0 Then
                If morech Then adresse = adresse & "-"
                morech = True
                adresse = adresse & "2"
            End If
            If (recbuf(2) And &H80) <> 0 Then
                If morech Then adresse = adresse & "-"
                morech = True
                adresse = adresse & "3"
            End If
            If (recbuf(1) And &H1) <> 0 Then
                If morech Then adresse = adresse & "-"
                morech = True
                adresse = adresse & "4"
            End If
            If (recbuf(1) And &H2) <> 0 Then
                If morech Then adresse = adresse & "-"
                morech = True
                adresse = adresse & "5"
            End If
            If (recbuf(1) And &H4) <> 0 Then
                If morech Then adresse = adresse & "-"
                morech = True
                adresse = adresse & "6"
            End If
            If (recbuf(1) And &H8) <> 0 Then
                If morech Then adresse = adresse & "-"
                morech = True
                adresse = adresse & "7"
            End If
            If (recbuf(1) And &H10) <> 0 Then
                If morech Then adresse = adresse & "-"
                morech = True
                adresse = adresse & "8"
            End If
            If (recbuf(1) And &H20) <> 0 Then
                If morech Then adresse = adresse & "-"
                morech = True
                adresse = adresse & "9"
            End If
            If (recbuf(2) And &H10) <> 0 Then
                If morech Then adresse = adresse & "-"
                morech = True
                adresse = adresse & "10"
            End If
            'message = "  Command:"
            message = ""
            If (recbuf(0) And &H3F) = &H10 Then
                message = message & " ON"
            ElseIf (recbuf(0) And &H3F) = &H11 Then
                message = message & " LEVEL 1"
            ElseIf (recbuf(0) And &H3F) = &H12 Then
                message = message & " LEVEL 2"
            ElseIf (recbuf(0) And &H3F) = &H13 Then
                message = message & " LEVEL 3"
            ElseIf (recbuf(0) And &H3F) = &H14 Then
                message = message & " LEVEL 4"
            ElseIf (recbuf(0) And &H3F) = &H15 Then
                message = message & " LEVEL 5"
            ElseIf (recbuf(0) And &H3F) = &H16 Then
                message = message & " LEVEL 6"
            ElseIf (recbuf(0) And &H3F) = &H17 Then
                message = message & " LEVEL 7"
            ElseIf (recbuf(0) And &H3F) = &H18 Then
                message = message & " LEVEL 8"
            ElseIf (recbuf(0) And &H3F) = &H19 Then
                message = message & " LEVEL 9"
            ElseIf (recbuf(0) And &H3F) = &H1A Then
                message = message & " OFF"
            ElseIf (recbuf(0) And &H38) = &H0 Then
                message = message & " UP"
                message = message & " cnt=" & VB.Right("0" & (Hex(recbuf(0) And &H7)), 2)
            ElseIf (recbuf(0) And &H38) = &H8 Then
                message = message & " DOWN"
                message = message & " cnt=" & VB.Right("0" & (Hex(recbuf(0) And &H7)), 2)
            ElseIf (recbuf(0) And &H3F) = &H1C Then
                message = message & " PROG"
            Else
                message = message & " unknown cmd:" & VB.Right("0" & (Hex(recbuf(0) And &H3F)), 2)
            End If
            parity = &H0
            temp = recbuf(0)
            For i = 1 To 4
                parity = parity + (temp And &H1)
                temp = temp >> 2
            Next
            If (parity And &H1) <> 1 Then message = message & " Odd parity error in byte 0"
            parity = &H0
            For i = 1 To 8
                parity = parity + (recbuf(0) And &H1)
                recbuf(0) = recbuf(0) >> 1
            Next
            If (parity And &H1) <> 0 Then message = message & " Even parity error in byte 0"
            parity = &H0
            temp = recbuf(2)
            For i = 1 To 4
                parity = parity + (temp And &H1)
                temp = temp >> 2
            Next
            temp = recbuf(1)
            For i = 1 To 4
                parity = parity + (temp And &H1)
                temp = temp >> 2
            Next
            If (parity And &H1) <> 1 Then message = message & " Odd parity error in byte 1-2"
            parity = &H0
            For i = 1 To 8
                parity = parity + (recbuf(2) And &H1)
                recbuf(2) = recbuf(2) >> 1
            Next
            For i = 1 To 8
                parity = parity + (recbuf(1) And &H1)
                recbuf(1) = recbuf(1) >> 1
            Next
            If (parity And &H1) <> 0 Then message = message & " Even parity error in byte 1-2"
            WriteRetour(adresse, "", message)
        Catch ex As Exception
            WriteLog("ERR: RFXCOM processkoppla : " & ex.Message)
        End Try
    End Sub

    'OK
    Private Sub processvisonic(ByVal recbits As Byte)
        Try
            'WriteLog("ERR: Process VISONIC pas encore géré")
            Dim parity As Integer
            Dim adresse, message As String
            If recbits = 96 Then
                WriteLog("Visionic - MKP150 cmd from Console")
            ElseIf recbits = 80 Then
                WriteLog("Visionic - MKP150 cmd")
            ElseIf recbits = 72 Then
                WriteLog("Visionic - MKP150 cmd")
            ElseIf recbits = 66 Then
                'adresse = "CodeSecure"
                'adresse = " encr:" & VB.Right("0" & Hex(recbuf(0)), 2) & VB.Right("0" & Hex(recbuf(1)), 2) & VB.Right("0" & Hex(recbuf(2)), 2) & VB.Right("0" & Hex(recbuf(3)), 2)
                'adresse = adresse & " serial:" & VB.Right("0" & Hex(recbuf(4)), 2) & VB.Right("0" & Hex(recbuf(5)), 2) & VB.Right("0" & Hex(recbuf(6)), 2) & Hex((recbuf(7) >> 4) And &HF)
                adresse = "" & VB.Right("0" & Hex(recbuf(0)), 2) & VB.Right("0" & Hex(recbuf(1)), 2) & VB.Right("0" & Hex(recbuf(2)), 2) & VB.Right("0" & Hex(recbuf(3)), 2)
                adresse = adresse & "-" & VB.Right("0" & Hex(recbuf(4)), 2) & VB.Right("0" & Hex(recbuf(5)), 2) & VB.Right("0" & Hex(recbuf(6)), 2) & Hex((recbuf(7) >> 4) And &HF)
                message = ""
                If (recbuf(7) And &H1) <> 0 Then message = " Light"
                If (recbuf(7) And &H2) <> 0 Then message = " Arm"
                If (recbuf(7) And &H4) <> 0 Then message = " Disarm"
                If (recbuf(7) And &H8) <> 0 Then message = " Arm-Home"
                If (recbuf(8) And &H4) <> 0 Then message = " Repeat bit"
                If (recbuf(8) And &H8) <> 0 Then message = " Bat-Low"
                WriteRetour(adresse, "", message)
            ElseIf recbits = 36 Then
                parity = &H0
                If (recbuf(0) And &H10) <> 0 Then parity += 1
                If (recbuf(0) And &H1) <> 0 Then parity += 1
                If (recbuf(1) And &H10) <> 0 Then parity += 1
                If (recbuf(1) And &H1) <> 0 Then parity += 1
                If (recbuf(2) And &H10) <> 0 Then parity += 1
                If (recbuf(2) And &H1) <> 0 Then parity += 1
                If (recbuf(3) And &H10) <> 0 Then parity += 1
                If (recbuf(3) And &H1) <> 0 Then parity += 1
                If (recbuf(4) And &H10) <> 0 Then parity += 1
                If (parity And &H1) <> 0 Then WriteLog(" Visionic - B0 Parity error B4+B8+B12+B16+B20+B24+B28+B32")
                parity = &H0
                If (recbuf(0) And &H20) <> 0 Then parity += 1
                If (recbuf(0) And &H2) <> 0 Then parity += 1
                If (recbuf(1) And &H20) <> 0 Then parity += 1
                If (recbuf(1) And &H2) <> 0 Then parity += 1
                If (recbuf(2) And &H20) <> 0 Then parity += 1
                If (recbuf(2) And &H2) <> 0 Then parity += 1
                If (recbuf(3) And &H20) <> 0 Then parity += 1
                If (recbuf(3) And &H2) <> 0 Then parity += 1
                If (recbuf(4) And &H20) <> 0 Then parity += 1
                If (parity And &H1) <> 0 Then WriteLog(" B1 Parity error B5+B9+B13+B17+B21+B25+B29+B33")
                parity = &H0
                If (recbuf(0) And &H40) <> 0 Then parity += 1
                If (recbuf(0) And &H4) <> 0 Then parity += 1
                If (recbuf(1) And &H40) <> 0 Then parity += 1
                If (recbuf(1) And &H4) <> 0 Then parity += 1
                If (recbuf(2) And &H40) <> 0 Then parity += 1
                If (recbuf(2) And &H4) <> 0 Then parity += 1
                If (recbuf(3) And &H40) <> 0 Then parity += 1
                If (recbuf(3) And &H4) <> 0 Then parity += 1
                If (recbuf(4) And &H40) <> 0 Then parity += 1
                If (parity And &H1) <> 0 Then WriteLog(" B2 Parity error B6+B10+B14+B18+B22+B26+B30+B34")
                parity = &H0
                If (recbuf(0) And &H80) <> 0 Then parity += 1
                If (recbuf(0) And &H8) <> 0 Then parity += 1
                If (recbuf(1) And &H80) <> 0 Then parity += 1
                If (recbuf(1) And &H8) <> 0 Then parity += 1
                If (recbuf(2) And &H80) <> 0 Then parity += 1
                If (recbuf(2) And &H8) <> 0 Then parity += 1
                If (recbuf(3) And &H80) <> 0 Then parity += 1
                If (recbuf(3) And &H8) <> 0 Then parity += 1
                If (recbuf(4) And &H80) <> 0 Then parity += 1
                If (parity And &H1) <> 0 Then WriteLog(" B3 Parity error B7+B11+B15+B19+B23+B27+B31+B35")
                'WriteMessage("               PowerCode", False)
                adresse = VB.Right("0" & Hex(recbuf(0)), 2) + VB.Right("0" & Hex(recbuf(1)), 2) + VB.Right("0" & Hex(recbuf(2)), 2)
                message = ""
                If (recbuf(3) And &H80) <> 0 Then
                    message = message & " Tamper   ,"
                Else
                    message = message & " No Tamper,"
                End If
                If (recbuf(3) And &H40) <> 0 Then
                    message = message & "Alert,"
                Else
                    message = message & "Close,"
                End If
                If (recbuf(3) And &H20) <> 0 Then
                    message = message & "Battery-Low,"
                Else
                    message = message & "Battery-OK ,"
                End If
                If (recbuf(3) And &H10) <> 0 Then
                    message = message & "Alive,"
                Else
                    message = message & "Event,"
                End If
                If (recbuf(3) And &H8) <> 0 Then
                    message = message & "Restore reported    ,"
                Else
                    message = message & "Restore not reported,"
                End If
                If (recbuf(3) And &H4) <> 0 Then
                    message = message & "Primary contact"
                Else
                    message = message & "Second. contact"
                End If
                If (recbuf(3) And &H2) <> 0 Then message = message & " Bit 5??"
                If (recbuf(3) And &H1) <> 0 Then message = message & " Bit 4??"
                WriteRetour(adresse, "", message)
            End If
        Catch ex As Exception
            WriteLog("ERR: RFXCOM processvisonic : " & ex.Message)
        End Try
    End Sub

    'pas géré
    Private Sub processrfxsensor()
        Try
            WriteLog("ERR: Process RFXSENSOR pas encore géré ")
            'Dim barometer As Integer
            'Dim measured_value, humidity As Single

            'WriteMessage("               RFXsensor[" & (recbuf(0) * 256 + recbuf(1)).ToString, False)
            'Select Case (recbuf(0) And &H3)
            '    Case 0 : WriteMessage("]T", False)
            '    Case 1 : WriteMessage("]Z", False)
            '    Case 2 : WriteMessage("]V", False)
            '    Case 3 : WriteMessage("]?", False)
            'End Select
            'WriteMessage(" RFXSensor ", False)
            'WriteMessage(" addr:" & VB.Right("0" & Hex(recbuf(0)), 2), False)
            'WriteMessage(VB.Right("0" & Hex(recbuf(1)), 2), False)
            'WriteMessage(" ID:" & Convert.ToString(recbuf(1) + (recbuf(0) * 256)) & " ", False)
            'If (recbuf(3) And &H10) <> 0 Then
            '    Select Case recbuf(2)
            '        Case &H81 : WriteMessage(" Error: No 1-Wire device connected", False)
            '        Case &H82 : WriteMessage(" Error: 1-Wire ROM CRC error", False)
            '        Case &H83 : WriteMessage(" Error: 1-Wire device connected is not a DS1820", False)
            '        Case &H84 : WriteMessage(" Error: No end of read signal received from 1-Wire device", False)
            '        Case &H85 : WriteMessage(" Error: 1-Wire device Scratchpad CRC error", False)
            '        Case &H1 : WriteMessage(" Info: address incremented", False)
            '        Case &H2 : WriteMessage(" Info: battery low", False)
            '        Case Else : WriteMessage(" Unknown Info/Error code!", False)
            '    End Select
            'Else
            '    If (recbuf(0) And &H3) = 0 Then
            '        measured_value = recbuf(2) + ((recbuf(3) >> 5) * 0.125)
            '        If measured_value > 200 Then
            '            '   WriteMessage("Temp:-" & Convert.ToString(256.0 - measured_value) & "ºC | " & Convert.ToString((256.0 - measured_value) * 1.8 + 32) & "ºF", False)
            '            measured_value = 0 - (256 - measured_value)
            '        End If
            '        WriteMessage("Temp:" & Convert.ToString(measured_value) & "ºC | " & Convert.ToString((measured_value) * 1.8 + 32) & "ºF", False)
            '        temperature = measured_value

            '    ElseIf (recbuf(0) And &H3) = 1 Then
            '        measured_value = (recbuf(2) * 256 + recbuf(3)) >> 5
            '        ' It is assumed that only one RFXSensor is active!
            '        ' For correct processing you need to save the temp and voltage for each sensor
            '        ' and use this in the checks and calculations.
            '        If supply_voltage <> 0 Then
            '            humidity = (((measured_value / supply_voltage) - 0.16) / 0.0062)
            '            barometer = ((measured_value / supply_voltage) + 0.095) / 0.0009
            '        Else
            '            WriteMessage(" ", True)
            '            WriteMessage("                                   Not yet able to calculate the right RH and barometric pressure values.", True)
            '            WriteMessage("                                   Supply Voltage not yet available! Now 4.7V assumed. Reset the RFXSensor or wait. (max. 80min)", True)
            '            humidity = (((measured_value / 470) - 0.16) / 0.0062)
            '            barometer = ((measured_value / 470) + 0.095) / 0.0009
            '        End If
            '        If temperature <> 0 Then
            '            humidity = Math.Round(humidity / (1.0546 - 0.00216 * temperature), 2)
            '        Else
            '            If supply_voltage <> 0 Then
            '                WriteMessage(" ", True)
            '                WriteMessage("                                   Not yet able to calculate the right RH and barometric pressure values.", True)
            '            End If
            '            WriteMessage("                                   Temperature not yet available! Now 25ºC assumed. Reset the RFXSensor or wait. (max. 80min)", True)
            '            humidity = Math.Round(humidity / (1.0546 - 0.00216 * 25), 2)
            '        End If
            '        If supply_voltage = 0 Or temperature = 0 Then
            '            WriteMessage("                                   ", False)
            '        End If
            '        WriteMessage("RH:" & Convert.ToString(humidity) & "%", False)
            '        WriteMessage(" Barometer:" & Convert.ToString(barometer) & "hPa", False)
            '        WriteMessage(" A/D voltage:" & Convert.ToString(measured_value / 100), False)
            '    ElseIf (recbuf(0) And &H3) = 2 Then
            '        supply_voltage = (recbuf(2) * 256 + recbuf(3)) >> 5
            '        WriteMessage("Supply Voltage:" & Convert.ToString(supply_voltage / 100), False)
            '    ElseIf (recbuf(0) And &H3) = 3 Then
            '        If (recbuf(3) And &H20) = 0 Then
            '            WriteMessage("ZAP25:" & Convert.ToString(Math.Round((5 / 1024) * (recbuf(2) * 2 + (recbuf(3) >> 7)) / 0.033, 2) & "A"), False)
            '            WriteMessage(" ZAP50:" & Convert.ToString(Math.Round((5 / 1024) * (recbuf(2) * 2 + (recbuf(3) >> 7)) / 0.023, 2) & "A"), False)
            '            WriteMessage(" ZAP100:" & Convert.ToString(Math.Round((5 / 1024) * (recbuf(2) * 2 + (recbuf(3) >> 7)) / 0.019, 2) & "A"), False)
            '        Else
            '            WriteMessage("Voltage=" & Convert.ToString(recbuf(2) * 2), False)
            '        End If
            '    End If
            'End If
        Catch ex As Exception
            WriteLog("ERR: RFXCOM processrfxsensor : " & ex.Message)
        End Try
    End Sub

    'OK
    Private Sub processrfxmeter()
        Try
            Dim adresse, valeur, valeurtemp As String
            'Dim measured_value As Single
            'WriteMessage("           RFXMeter[" & (recbuf(0) * 256 + recbuf(1)).ToString & "]M RFXMeter", False)
            'WriteMessage(" addr:" & VB.Right("0" & Hex(recbuf(0)), 2), False)
            'WriteMessage(VB.Right("0" & Hex(recbuf(1)), 2), False)
            adresse = VB.Right("0" & Hex(recbuf(0)), 2) & VB.Right("0" & Hex(recbuf(1)), 2) & "-" & Convert.ToString(recbuf(1) + (recbuf(0) * 256))
            'WriteMessage(" ID:" & Convert.ToString(recbuf(1) + (recbuf(0) * 256)) & " ", False)
            Select Case recbuf(5) And &HF0
                Case &H0
                    valeur = ((recbuf(4) * 65536) + (recbuf(2) * 256) + recbuf(3))
                    WriteRetour(adresse, "", valeur / 100)
                    'WriteMessage("RFXMeter: " & Convert.ToString(measured_value), False)
                    'WriteMessage(";  RFXPower: " & Convert.ToString(measured_value / 100) & " kWh", False)
                    'WriteMessage(";  RFXPower-Module: " & Convert.ToString(measured_value / 1000) & " kWh", False)
                Case &H10
                    Select Case recbuf(2)
                        Case &H1 : valeur = "Interval: 30 sec."
                        Case &H2 : valeur = "Interval: 1 min."
                        Case &H4 : valeur = "Interval: 6 (old=5) min."
                        Case &H8 : valeur = "Interval: 12 (old=10) min."
                        Case &H10 : valeur = "Interval: 15 min."
                        Case &H20 : valeur = "Interval: 30 min."
                        Case &H40 : valeur = "Interval: 45 min."
                        Case &H80 : valeur = "Interval: 60 min."
                        Case Else : valeur = "Interval: illegal value"
                    End Select
                    WriteRetour(adresse, "", "CFG: " & valeur)
                Case &H20
                    Select Case (recbuf(4) And &HC0)
                        Case &H0 : valeurtemp = "Input-0 "
                        Case &H40 : valeurtemp = "Input-1 "
                        Case &H80 : valeurtemp = "Input-2 "
                        Case Else : valeurtemp = "Error, unknown input "
                    End Select
                    valeur = (((recbuf(4) And &H3F) * 65536) + (recbuf(2) * 256) + recbuf(3)) / 1000
                    valeurtemp = valeurtemp & " Calibration: " & Convert.ToString(valeur) & "msec "
                    If valeur <> 0 Then
                        valeurtemp = valeurtemp & "RFXPower= " & Convert.ToString(Round(1 / ((16 * valeur) / (3600000 / 100)), 3)) & "kW"
                        valeurtemp = valeurtemp & " RFXPwr= " & Convert.ToString(Round(1 / ((16 * valeur) / (3600000 / 62.5)), 3)) & "|" & Convert.ToString(Round((1 / ((16 * valeur) / (3600000 / 62.5))) * 1.917, 3)) & "kW"
                    End If
                    WriteRetour(adresse, "", "CFG: Calibration" & Convert.ToString(valeur) & "msec " & valeurtemp)
                Case &H30
                    WriteRetour(adresse, "", "CFG: New address set")
                Case &H40
                    Select Case (recbuf(4) And &HC0)
                        Case &H0 : WriteRetour(adresse, "", "CFG: Counter for Input-0 will be set to zero within 5 seconds OR push MODE button for next command.")
                        Case &H40 : WriteRetour(adresse, "", "CFG: Counter for Input-1 will be set to zero within 5 seconds OR push MODE button for next command.")
                        Case &H80 : WriteRetour(adresse, "", "CFG: Counter for Input-2 will be set to zero within 5 seconds OR push MODE button for next command.")
                        Case Else : WriteRetour(adresse, "", "ERR: unknown input ")
                    End Select
                Case &H50
                    valeur = (recbuf(2) >> 4) * 100000 + (recbuf(2) And &HF) * 10000 + (recbuf(3) >> 4) * 1000 + (recbuf(3) And &HF) * 100 + (recbuf(4) >> 4) * 10 + (recbuf(4) And &HF)
                    WriteRetour(adresse, "", "CFG: Push MODE push button within 5 seconds to increment the 1st digit - Counter value = " & VB.Right("00000" & Convert.ToString(valeur), 6))
                Case &H60
                    valeur = (recbuf(2) >> 4) * 100000 + (recbuf(2) And &HF) * 10000 + (recbuf(3) >> 4) * 1000 + (recbuf(3) And &HF) * 100 + (recbuf(4) >> 4) * 10 + (recbuf(4) And &HF)
                    WriteRetour(adresse, "", "CFG: Push MODE push button within 5 seconds to increment the 2nd digit - Counter value = " & VB.Right("00000" & Convert.ToString(valeur), 6))
                Case &H70
                    valeur = (recbuf(2) >> 4) * 100000 + (recbuf(2) And &HF) * 10000 + (recbuf(3) >> 4) * 1000 + (recbuf(3) And &HF) * 100 + (recbuf(4) >> 4) * 10 + (recbuf(4) And &HF)
                    WriteRetour(adresse, "", "CFG: Push MODE push button within 5 seconds to increment the 3rd digit - Counter value = " & VB.Right("00000" & Convert.ToString(valeur), 6))
                Case &H80
                    valeur = (recbuf(2) >> 4) * 100000 + (recbuf(2) And &HF) * 10000 + (recbuf(3) >> 4) * 1000 + (recbuf(3) And &HF) * 100 + (recbuf(4) >> 4) * 10 + (recbuf(4) And &HF)
                    WriteRetour(adresse, "", "CFG: Push MODE push button within 5 seconds to increment the 4th digit - Counter value = " & VB.Right("00000" & Convert.ToString(valeur), 6))
                Case &H90
                    valeur = (recbuf(2) >> 4) * 100000 + (recbuf(2) And &HF) * 10000 + (recbuf(3) >> 4) * 1000 + (recbuf(3) And &HF) * 100 + (recbuf(4) >> 4) * 10 + (recbuf(4) And &HF)
                    WriteRetour(adresse, "", "CFG: Push MODE push button within 5 seconds to increment the 5th digit - Counter value = " & VB.Right("00000" & Convert.ToString(valeur), 6))
                Case &HA0
                    valeur = (recbuf(2) >> 4) * 100000 + (recbuf(2) And &HF) * 10000 + (recbuf(3) >> 4) * 1000 + (recbuf(3) And &HF) * 100 + (recbuf(4) >> 4) * 10 + (recbuf(4) And &HF)
                    WriteRetour(adresse, "", "CFG: Push MODE push button within 5 seconds to increment the 6th digit - Counter value = " & VB.Right("00000" & Convert.ToString(valeur), 6))
                Case &HB0
                    Select Case recbuf(4)
                        Case &H0 : WriteRetour(adresse, "", "CFG: Counter for Input-0 reset to zero.")
                        Case &H40 : WriteRetour(adresse, "", "CFG: Counter for Input-1 reset to zero.")
                        Case &H80 : WriteRetour(adresse, "", "CFG: Counter for Input-2 reset to zero.")
                        Case Else : WriteRetour(adresse, "", "CFG: protocol error.")
                    End Select
                Case &HC0
                    WriteRetour(adresse, "", "CFG: Enter SET INTERVAL RATE mode within 5 seconds OR push MODE button for next command.")
                Case &HD0
                    Select Case (recbuf(4) And &HC0)
                        Case &H0 : WriteRetour(adresse, "", "CFG: Enter CALIBRATION mode for Input-0 within 5 seconds OR push MODE button for next command.")
                        Case &H40 : WriteRetour(adresse, "", "CFG: Enter CALIBRATION mode for Input-1 within 5 seconds OR push MODE button for next command.")
                        Case &H80 : WriteRetour(adresse, "", "CFG: Enter CALIBRATION mode for Input-2 within 5 seconds OR push MODE button for next command.")
                        Case Else : WriteRetour(adresse, "", "CFG: unknown input ")
                    End Select
                Case &HE0
                    WriteRetour(adresse, "", "CFG: Enter SET ADDRESS mode within 5 seconds OR push MODE button for next command.")
                Case &HF0
                    If recbuf(2) < &H40 Then
                        valeur = "RFXPower Identification, "
                    ElseIf recbuf(2) < &H80 Then
                        valeur = "RFXWater Identification, "
                    ElseIf recbuf(2) < &HC0 Then
                        valeur = "RFXGas Identification, "
                    Else
                        valeur = "RFXMeter Identification, "
                    End If
                    valeur = valeur & "Firmware Version: " & VB.Right("0" & Hex(recbuf(2)), 2) & ", Interval rate: "
                    Select Case recbuf(3)
                        Case &H1 : valeur = valeur & "30 seconds"
                        Case &H2 : valeur = valeur & "1 minute"
                        Case &H4 : valeur = valeur & "6 minutes"
                        Case &H8 : valeur = valeur & "12 minutes"
                        Case &H10 : valeur = valeur & "15 minutes"
                        Case &H20 : valeur = valeur & "30 minutes"
                        Case &H40 : valeur = valeur & "45 minutes"
                        Case &H80 : valeur = valeur & "60 minutes"
                        Case Else : valeur = valeur & "illegal value"
                    End Select
                    WriteRetour(adresse, "", "CFG:" & valeur)
                Case Else
                    WriteRetour(adresse, "", "ERR: illegal packet type")
            End Select
        Catch ex As Exception
            WriteLog("ERR: RFXCOM processrfxmeter : " & ex.Message)
        End Try
    End Sub

    'pas géré : renvoi le message recu dans le log
    Private Sub processrfremote()
        Try
            message = " REMOTE[0]C PC Remote: "
            Select Case recbuf(2)
                Case &H2 : message = message & "0"
                Case &H82 : message = message & "1"
                Case &HD1 : message = message & "MP3"
                Case &H42 : message = message & "2"
                Case &HD2 : message = message & "DVD"
                Case &HC2 : message = message & "3"
                Case &HD3 : message = message & "CD"
                Case &H22 : message = message & "4"
                Case &HD4 : message = message & "PC or SHIFT-4"
                Case &HA2 : message = message & "5"
                Case &HD5 : message = message & "SHIFT-5"
                Case &H62 : message = message & "6"
                Case &HE2 : message = message & "7"
                Case &H12 : message = message & "8"
                Case &H92 : message = message & "9"
                Case &HC0 : message = message & "CH-"
                Case &H40 : message = message & "CH+"
                Case &HE0 : message = message & "VOL-"
                Case &H60 : message = message & "VOL+"
                Case &HA0 : message = message & "MUTE"
                Case &H3A : message = message & "INFO"
                Case &H38 : message = message & "REW"
                Case &HB8 : message = message & "FF"
                Case &HB0 : message = message & "PLAY"
                Case &H72 : message = message & "PAUSE"
                Case &H70 : message = message & "STOP"
                Case &HB6 : message = message & "MENU"
                Case &HFF : message = message & "REC"
                Case &HC9 : message = message & "EXIT"
                Case &HD8 : message = message & "TEXT"
                Case &HD9 : message = message & "SHIFT-TEXT"
                Case &HF2 : message = message & "TELETEXT"
                Case &HD7 : message = message & "SHIFT-TELETEXT"
                Case &HBA : message = message & "A+B"
                Case &H52 : message = message & "ENT"
                Case &HD6 : message = message & "SHIFT-ENT"
                Case Else : message = message & "Unknown cmd"
            End Select
            WriteLog("ERR: Process RFREMOTE pas encore géré : " & message)
        Catch ex As Exception
            WriteLog("ERR: RFXCOM processrfremote : " & ex.Message)
        End Try
    End Sub

    'OK
    Private Sub processx10()
        Try
            Dim recbytes As Byte
            Dim adresse, valeur As String
            Select Case (recbuf(0) And &HF0)
                Case &H60 : adresse = "A"
                Case &H70 : adresse = "B"
                Case &H40 : adresse = "C"
                Case &H50 : adresse = "D"
                Case &H80 : adresse = "E"
                Case &H90 : adresse = "F"
                Case &HA0 : adresse = "G"
                Case &HB0 : adresse = "H"
                Case &HE0 : adresse = "I"
                Case &HF0 : adresse = "J"
                Case &HC0 : adresse = "K"
                Case &HD0 : adresse = "L"
                Case &H0 : adresse = "M"
                Case &H10 : adresse = "N"
                Case &H20 : adresse = "O"
                Case &H30 : adresse = "P"
                Case Else : adresse = "Unknown unit-"
            End Select
            Select Case recbuf(2)
                Case &H80 : valeur = "ALL-LIGHTS-OFF "
                Case &H90 : valeur = "ALL-LIGHTS-ON"
                Case &H88 : valeur = "BRIGHT"
                Case &H98 : valeur = "DIM"
                Case Else
                    If (recbuf(2) And &H10) = 0 Then recbytes = 0 Else recbytes = &H1
                    If (recbuf(2) And &H8) <> 0 Then recbytes = recbytes + &H2
                    If (recbuf(2) And &H40) <> 0 Then recbytes = recbytes + &H4
                    If (recbuf(0) And &H4) <> 0 Then recbytes = recbytes + &H8
                    recbytes = recbytes + 1
                    adresse = adresse + Trim(Str(recbytes))
                    If (recbuf(2) And &H1) = 1 Then
                        valeur = "-Prog Koppla (non X10)"
                    ElseIf (recbuf(2) And &H20) = 0 Then
                        valeur = "ON"
                    Else
                        valeur = "OFF"
                    End If
            End Select
            WriteRetour(adresse, "", valeur)
        Catch ex As Exception
            WriteLog("ERR: RFXCOM processx10 : " & ex.Message)
        End Try
    End Sub

    'OK
    Private Sub processdm10()
        Try
            Dim adresse, valeur As String
            adresse = (recbuf(0) * 256 + recbuf(1)).ToString
            'WriteMessage(" addr:" & VB.Right("0" & Hex(recbuf(0)), 2), False)
            'WriteMessage(" ID:" & VB.Right("    " & Str(createhsaddr()), 5), False)
            Select Case recbuf(2)
                Case &HE0 : valeur = "Motion detected"
                Case &HF0 : valeur = "Dark detected"
                Case &HF8 : valeur = "Light detected"
                Case Else : valeur = "Unknown command:" & VB.Right("0" & Hex(recbuf(2)), 2)
            End Select
            WriteRetour(adresse, "", valeur)
        Catch ex As Exception
            WriteLog("ERR: RFXCOM processdm10 : " & ex.Message)
        End Try
    End Sub

    'OK
    Private Sub processx10security()
        Try
            Dim adresse As String = ""
            Dim valeur As String = ""
            Dim batteryempty As Boolean = False
            'Dim hsaddr As Integer
            adresse = (recbuf(0) * 256 + recbuf(1)).ToString
            Select Case recbuf(2)
                'Case &H3 : valeur = "ALERT: PANIC"
                'Case &H4 : valeur = "ALERT"
                'Case &H5
                '    batteryempty = True
                '    valeur = "ALERT"
                'Case &H22 : valeur = "ALERT: PANIC"
                'Case &H26 : valeur = "ALERT: PANIC"
                'Case &H44 : valeur = "Normal"
                'Case &H5 : valeur = "Normal"
                'Case &H66
                '    batteryempty = True
                '    valeur = "Normal"
                'Case &H80 : valeur = "Normal"
                'Case &H84 : valeur = "Normal"
                'Case &H85
                '    batteryempty = True
                '    valeur = "Normal"
                'Case Else : valeur = "ALERT: ??????"

                Case &H0 : valeur = "ALERT (Max delay)"
                Case &H1
                    batteryempty = True
                    valeur = "ALERT (Battery + Max delay)"
                Case &H2 : valeur = "ARM Away (max)"
                Case &H3 : valeur = "ALERT: PANIC"
                Case &H4 : valeur = "ALERT"
                Case &H5
                    batteryempty = True
                    valeur = "ALERT (Battery)"
                Case &H6 : valeur = "ARM Away (min)"
                Case &HA : valeur = "ARM Home (max)"
                Case &HC : valeur = "ALERT"
                Case &HD
                    batteryempty = True
                    valeur = "ALERT (Battery + sensor stop)"
                Case &HE : valeur = "ARM Home (min)"
                Case &H1C : valeur = "Temp -< Set"
                Case &H20 : valeur = "Dark sensor"
                Case &H22 : valeur = "ALERT: PANIC"
                Case &H26 : valeur = "ALERT: PANIC"
                Case &H2B : valeur = "Temp > Set"
                Case &H40 : valeur = "ALERT + Tamper (Max delay)"
                Case &H42 : valeur = "Lights On"
                Case &H44 : valeur = "Normal" 'Case &H44 : valeur = "ALERT + Tamper" ----------------------
                Case &H46 : valeur = "Lights On"
                Case &H4C : valeur = "ALERT + Tamper"
                Case &H66 : valeur = "ERR: Battery empty"
                Case &H80 : valeur = "Normal (Max delay)"
                Case &H81
                    batteryempty = True
                    valeur = "Normal (Battery + Max delay)"
                Case &H82 : valeur = "Disarm"
                Case &H84 : valeur = "Normal"
                Case &H85
                    batteryempty = True
                    valeur = "Normal (Battery)"
                Case &H86 : valeur = "Disarm"
                Case &H8C : valeur = "Normal"
                Case &HC0 : valeur = "Normal + Tamper (Max delay)"
                Case &HC2 : valeur = "Lights Off"
                Case &HC4 : valeur = "Normal + Tamper"
                Case &HC6 : valeur = "Lights Off"
                Case &HCC : valeur = "Normal + Tamper"
                Case &HE0 : valeur = "Motion"
                Case &HF0 : valeur = "Darkness detected"
                Case &HF8 : valeur = "Light detected"
                Case Else : valeur = "ALERT ??????"
            End Select
            If protocolmode = MODEB32 Then
                'hsaddr = createhsaddr()
                'WriteLog("DEBUG : X10Security : addr:" & VB.Right("0" & Hex(recbuf(0)), 2) & " ID:" & VB.Right("    " & Str(hsaddr), 5))
            Else
                'hsaddr = createhsaddr()
                'WriteLog("DEBUG : X10Security : addr:" & VB.Right("0" & Hex(recbuf(0)), 2) & VB.Right("0" & Hex(recbuf(1)), 2) & " " & VB.Right("0" & Hex(recbuf(4)), 2) & " ID:" & VB.Right("    " & Str(hsaddr), 5))
            End If
            If valeur <> "" Then
                'valeur = valeur + " " + VB.Right("0" & Hex(recbuf(0)), 2) + "-" + VB.Right("0" & Hex(recbuf(1)), 2) + "-" + VB.Right("0" & Hex(recbuf(2)), 2) + "-" + VB.Right("0" & Hex(recbuf(3)), 2) + "-" + VB.Right("0" & Hex(recbuf(4)), 2) + "-" + VB.Right("0" & Hex(recbuf(5)), 2)
                valeur = valeur & " (" & VB.Right("0" & Hex(recbuf(2)), 2) & ")"
                WriteRetour(adresse, "", valeur)
                If batteryempty Then WriteBattery(adresse)
            End If
        Catch ex As Exception
            WriteLog("ERR: RFXCOM processx10security : " & ex.Message)
        End Try
    End Sub

    'OK
    Private Sub processdigimax()
        Try
            Dim parity, hsaddr As Integer
            Dim adresse, valeur As String
            Dim err As Boolean = False
            hsaddr = recbuf(1) * 256 + recbuf(0)
            adresse = CStr(recbuf(1) * 256 + recbuf(0))
            'WriteMessage(" DIGIMAX[" & CStr(recbuf(1) * 256 + recbuf(0)) & "]TY", False)
            'WriteMessage(" Digimax  addr:" & VB.Right("0" & Hex(recbuf(0)), 2), False)
            'WriteMessage(VB.Right("0" & Hex(recbuf(1)), 2) & " ID:" & VB.Right("    " & Str(hsaddr), 5), False)
            If (recbuf(4) And &H40) = 0 Then
                Select Case recbuf(2) And &H30
                    Case &H0 : valeur = "No Set temp available"
                    Case &H10 : valeur = "Demand for heat"
                    Case &H20 : valeur = "No demand for heat"
                    Case &H30 : valeur = "Initializing"
                End Select
            Else
                Select Case recbuf(2) And &H30
                    Case &H0 : valeur = "No Set temp available"
                    Case &H10 : valeur = "No demand for cooling"
                    Case &H20 : valeur = "Demand for cooling"
                    Case &H30 : valeur = "Initializing"
                End Select
            End If
            valeur = Convert.ToString(recbuf(3)) 'valeur de la temperature mesurée
            If protocolmode = MODEB32 Then
                parity = Not (((recbuf(0) And &HF0) >> 4) + (recbuf(0) And &HF) + ((recbuf(1) And &HF0) >> 4) + (recbuf(1) And &HF) + ((recbuf(2) And &HF0) >> 4)) And &HF
                If parity <> (recbuf(2) And &HF) Then
                    WriteLog("ERR: DIGIMAX " & adresse & " : Parity error on address/status SB:" & Hex(parity))
                    err = True
                End If
            Else
                valeur = valeur & "->" & Convert.ToString(recbuf(4) And &H3F) 'ajout de la temperature désiré
                parity = Not (((recbuf(0) And &HF0) >> 4) + (recbuf(0) And &HF) + ((recbuf(1) And &HF0) >> 4) + (recbuf(1) And &HF) + ((recbuf(2) And &HF0) >> 4)) And &HF
                If parity <> (recbuf(2) And &HF) Then
                    WriteLog("ERR: DIGIMAX " & adresse & " : Parity error on address/status SB:" & Hex(parity))
                    err = True
                End If
                parity = Not (((recbuf(3) And &HF0) >> 4) + (recbuf(3) And &HF) + ((recbuf(4) And &HF0) >> 4) + (recbuf(4) And &HF)) And &HF
                If parity <> ((recbuf(5) And &HF0) >> 4) Then
                    WriteLog("ERR: DIGIMAX " & adresse & " : Parity error on temp/set SB:" & Hex(parity))
                    err = True
                End If
            End If
            If err = False Then WriteRetour(adresse, "", valeur)
        Catch ex As Exception
            WriteLog("ERR: RFXCOM processdigimax : " & ex.Message)
        End Try
    End Sub

    'OK
    Private Function processoregon(ByVal recbits As Byte) As Boolean
        Try
            Dim direction As Integer
            Dim uv, dd, mm, yy As Single
            Dim hr, mn, sc, dag As String
            Dim oregon As Boolean = False
            'Dim ch As Byte
            Dim adresse, valeur As String

            'WriteMessage(" addr:" & VB.Right("0" & Hex(recbuf(3)), 2), False)

            If recbuf(0) = &HA And recbuf(1) = &H4D And recbits >= 72 Then
                '------------- THR128,THx138 ---------------
                oregon = True
                adresse = CStr((recbuf(3)) * 256 + ((recbuf(2) >> 4) And &H7))
                'ch = wrchannel()
                If ((recbuf(5) And &HF0) < &HA0) And ((recbuf(5) And &HF) < &HA) And ((recbuf(4) And &HF0) < &HA0) Then
                    If (recbuf(6) And &H8) = 0 Then
                        valeur = CStr(CSng(Hex(recbuf(5))) + CSng(Hex(recbuf(4) >> 4)) / 10)
                    Else
                        valeur = CStr(0 - (CSng(Hex(recbuf(5))) + CSng(Hex(recbuf(4) >> 4)) / 10))
                    End If
                Else
                    valeur = "ERR: wrong value in temperature field=" & Hex(recbuf(5)) & "." & Hex(recbuf(4) >> 4)
                End If
                WriteRetour(adresse, ListeDevices.TEMPERATURE.ToString, valeur)
                If (recbuf(4) And &H4) = &H4 Then WriteBattery(adresse)
                'checksum8()

            ElseIf recbuf(0) = &HEA And recbuf(1) = &H4C And recbits >= 60 Then
                '------------- THN132N,THWR288 ---------------
                oregon = True
                adresse = CStr((recbuf(3)) * 256 + ((recbuf(2) >> 4) And &H7))
                'ch = wrchannel()
                If (recbuf(6) And &H8) = 0 Then
                    valeur = CStr(CSng(Hex(recbuf(5))) + CSng(Hex(recbuf(4) >> 4)) / 10)
                Else
                    valeur = CStr(0 - (CSng(Hex(recbuf(5))) + CSng(Hex(recbuf(4) >> 4)) / 10))
                End If
                WriteRetour(adresse, ListeDevices.TEMPERATURE.ToString, valeur)
                If (recbuf(4) And &H4) = &H4 Then WriteBattery(adresse)
                'checksumw()

            ElseIf recbuf(0) = &H1A And recbuf(1) = &H2D And recbits >= 72 Then
                '------------- THGN122N-NX,THGR228N,THGR268 ---------------
                oregon = True
                adresse = CStr((recbuf(3)) * 256 + ((recbuf(2) >> 4) And &H7))
                'ch = wrchannel()
                If (recbuf(6) And &H8) = 0 Then
                    valeur = CStr(CSng(Hex(recbuf(5))) + CSng(Hex(recbuf(4) >> 4)) / 10)
                Else
                    valeur = CStr(0 - (CSng(Hex(recbuf(5))) + CSng(Hex(recbuf(4) >> 4)) / 10))
                End If
                WriteRetour(adresse, ListeDevices.TEMPERATURE.ToString, valeur)
                valeur = CStr(VB.Right(Hex(((recbuf(7) << 4) And &HF0) + ((recbuf(6) >> 4) And &HF)), 2))
                WriteRetour(adresse, ListeDevices.HUMIDITE.ToString, valeur)
                'valeur = wrhum(recbuf(7) And &HC0)
                'WriteRetour(adresse & "_HUM", valeur)
                If (recbuf(4) And &H4) = &H4 Then WriteBattery(adresse)
                'checksum8()

            ElseIf recbuf(0) = &HFA And recbuf(1) = &H28 And recbits >= 72 Then
                '------------- THR128,THx138 ---------------
                oregon = True
                adresse = CStr((recbuf(3)) * 256 + ((recbuf(2) >> 4) And &H7))
                'ch = wrchannel3()
                If (recbuf(6) And &H8) = 0 Then
                    valeur = CStr(CSng(Hex(recbuf(5))) + CSng(Hex(recbuf(4) >> 4)) / 10)
                Else
                    valeur = CStr(0 - (CSng(Hex(recbuf(5))) + CSng(Hex(recbuf(4) >> 4)) / 10))
                End If
                WriteRetour(adresse, ListeDevices.TEMPERATURE.ToString, valeur)
                valeur = CStr(VB.Right(Hex(((recbuf(7) << 4) And &HF0) + ((recbuf(6) >> 4) And &HF)), 2))
                WriteRetour(adresse, ListeDevices.HUMIDITE.ToString, valeur)
                'valeur = wrhum(recbuf(7) And &HC0)
                'WriteRetour(adresse & "_HUM", valeur)
                If (recbuf(4) And &H4) = &H4 Then WriteBattery(adresse)
                'checksum8()

            ElseIf (recbuf(0) And &HF) = &HA And recbuf(1) = &HCC And recbits >= 72 Then
                '------------- RTGR328N ---------------
                oregon = True
                adresse = CStr((recbuf(3)) * 256 + ((recbuf(2) >> 4) And &H7))
                'ch = wrchannel3()
                'WriteMessage(" counter:" & CStr(recbuf(0) >> 4), False)
                If (recbuf(6) And &H8) = 0 Then
                    valeur = CStr(CSng(Hex(recbuf(5))) + CSng(Hex(recbuf(4) >> 4)) / 10)
                Else
                    valeur = CStr(0 - (CSng(Hex(recbuf(5))) + CSng(Hex(recbuf(4) >> 4)) / 10))
                End If
                WriteRetour(adresse, ListeDevices.TEMPERATURE.ToString, valeur)
                valeur = CStr(VB.Right(Hex(((recbuf(7) << 4) And &HF0) + ((recbuf(6) >> 4) And &HF)), 2))
                WriteRetour(adresse, ListeDevices.HUMIDITE.ToString, valeur)
                'valeur = wrhum(recbuf(7) And &HC0)
                'WriteRetour(adresse & "_HUM", valeur)
                If (recbuf(4) And &H4) = &H4 Then WriteBattery(adresse)
                'checksum8()

            ElseIf recbuf(0) = &HCA And recbuf(1) = &H2C And recbits >= 72 Then
                '------------- THGR328 ---------------
                oregon = True
                adresse = CStr((recbuf(3)) * 256 + ((recbuf(2) >> 4) And &H7))
                'ch = wrchannel()
                'WriteMessage(" counter:" & CStr(recbuf(0) >> 4), False)
                If (recbuf(6) And &H8) = 0 Then
                    valeur = CStr(CSng(Hex(recbuf(5))) + CSng(Hex(recbuf(4) >> 4)) / 10)
                Else
                    valeur = CStr(0 - (CSng(Hex(recbuf(5))) + CSng(Hex(recbuf(4) >> 4)) / 10))
                End If
                WriteRetour(adresse, ListeDevices.TEMPERATURE.ToString, valeur)
                valeur = CStr(VB.Right(Hex(((recbuf(7) << 4) And &HF0) + ((recbuf(6) >> 4) And &HF)), 2))
                WriteRetour(adresse, ListeDevices.HUMIDITE.ToString, valeur)
                'valeur = wrhum(recbuf(7) And &HC0)
                'WriteRetour(adresse & "_HUM", valeur)
                If (recbuf(4) And &H4) = &H4 Then WriteBattery(adresse)
                'checksum8()

            ElseIf recbuf(0) = &HFA And recbuf(1) = &HB8 And recbits >= 72 Then
                '------------- WTGR800 ---------------
                oregon = True
                adresse = CStr(recbuf(3) * 256)
                If (recbuf(6) And &H8) = 0 Then
                    valeur = CStr(CSng(Hex(recbuf(5))) + CSng(Hex(recbuf(4) >> 4)) / 10)
                Else
                    valeur = CStr(0 - (CSng(Hex(recbuf(5))) + CSng(Hex(recbuf(4) >> 4)) / 10))
                End If
                WriteRetour(adresse, ListeDevices.TEMPERATURE.ToString, valeur)
                valeur = CStr(VB.Right(Hex(((recbuf(7) << 4) And &HF0) + ((recbuf(6) >> 4) And &HF)), 2))
                WriteRetour(adresse, ListeDevices.HUMIDITE.ToString, valeur)
                'valeur = wrhum(recbuf(7) And &HC0)
                'WriteRetour(adresse & "_HUM", valeur)
                valeur = wrbattery()
                WriteRetour(adresse, ListeDevices.BATTERIE.ToString, valeur)
                'checksum8()

            ElseIf recbuf(0) = &H1A And recbuf(1) = &H3D And recbits >= 72 Then
                '------------- THGR918 ---------------
                oregon = True
                adresse = CStr((recbuf(3)) * 256 + ((recbuf(2) >> 4) And &H7))
                'ch = wrchannel()
                If (recbuf(6) And &H8) = 0 Then
                    valeur = CStr(CSng(Hex(recbuf(5))) + CSng(Hex(recbuf(4) >> 4)) / 10)
                Else
                    valeur = CStr(0 - (CSng(Hex(recbuf(5))) + CSng(Hex(recbuf(4) >> 4)) / 10))
                End If
                WriteRetour(adresse, ListeDevices.TEMPERATURE.ToString, valeur)
                valeur = CStr(VB.Right(Hex(((recbuf(7) << 4) And &HF0) + ((recbuf(6) >> 4) And &HF)), 2))
                WriteRetour(adresse, ListeDevices.HUMIDITE.ToString, valeur)
                'valeur = wrhum(recbuf(7) And &HC0)
                'WriteRetour(adresse & "_HUM", valeur)
                valeur = wrbattery()
                WriteRetour(adresse, ListeDevices.BATTERIE.ToString, valeur)
                'checksum8()

            ElseIf recbuf(0) = &H5A And recbuf(1) = &H5D And recbits >= 88 Then
                '------------- BTHR918 ---------------
                oregon = True
                adresse = CStr((recbuf(3)) * 256 + ((recbuf(2) >> 4) And &H7))
                'ch = wrchannel()
                If (recbuf(6) And &H8) = 0 Then
                    valeur = CStr(CSng(Hex(recbuf(5))) + CSng(Hex(recbuf(4) >> 4)) / 10)
                Else
                    valeur = CStr(0 - (CSng(Hex(recbuf(5))) + CSng(Hex(recbuf(4) >> 4)) / 10))
                End If
                WriteRetour(adresse, ListeDevices.TEMPERATURE.ToString, valeur)
                valeur = CStr(VB.Right(Hex(((recbuf(7) << 4) And &HF0) + ((recbuf(6) >> 4) And &HF)), 2))
                WriteRetour(adresse, ListeDevices.HUMIDITE, valeur)
                'valeur = wrhum(recbuf(7) And &HC0)
                'WriteRetour(adresse & "_HUM", valeur)
                valeur = CStr(recbuf(8) + 795) 'en hPa
                WriteRetour(adresse, ListeDevices.BAROMETRE.ToString, valeur)
                valeur = wrforecast(recbuf(9) And &HF)


                'FOR ?????
                'WriteRetour(adresse, "FOR", valeur)



                If (recbuf(4) And &H4) = &H4 Then WriteBattery(adresse)
                'checksum10()

            ElseIf recbuf(0) = &H5A And recbuf(1) = &H6D And recbits >= 88 Then
                '------------- BTHR918N,BTHR968 ---------------
                oregon = True
                adresse = CStr((recbuf(3)) * 256 + ((recbuf(2) >> 4) And &H7))
                'ch = wrchannel()
                If (recbuf(6) And &H8) = 0 Then
                    valeur = CStr(CSng(Hex(recbuf(5))) + CSng(Hex(recbuf(4) >> 4)) / 10)
                Else
                    valeur = CStr(0 - (CSng(Hex(recbuf(5))) + CSng(Hex(recbuf(4) >> 4)) / 10))
                End If
                WriteRetour(adresse, ListeDevices.TEMPERATURE.ToString, valeur)
                valeur = CStr(VB.Right(Hex(((recbuf(7) << 4) And &HF0) + ((recbuf(6) >> 4) And &HF)), 2))
                WriteRetour(adresse, ListeDevices.HUMIDITE.ToString, valeur)
                'valeur = wrhum(recbuf(7) And &HC0)
                'WriteRetour(adresse & "_HUM", valeur)
                valeur = CStr(recbuf(8) + 795) 'en hPa
                WriteRetour(adresse, ListeDevices.BAROMETRE.ToString, valeur)
                valeur = wrforecast(recbuf(9) >> 4)


                'FOR ?????
                'WriteRetour(adresse, "FOR", valeur)



                valeur = wrbattery()
                WriteRetour(adresse, ListeDevices.BATTERIE.ToString, valeur)
                'checksum10()

            ElseIf recbuf(0) = &H2A And recbuf(1) = &H1D And recbits >= 80 Then
                '------------- RGR126,RGR682,RGR918 ---------------
                oregon = True
                adresse = CStr(recbuf(3) * 256)
                valeur = CStr(CSng(Hex(recbuf(5))) * 10 + CSng(Hex((recbuf(4) >> 4) And &HF)))
                WriteRetour(adresse, ListeDevices.PLUIECOURANT.ToString, valeur) 'mm/hr
                valeur = CStr(CSng(Hex(recbuf(8) And &HF)) * 1000 + CSng(Hex(recbuf(7))) * 10 + CSng(Hex(recbuf(6) >> 4)))
                WriteRetour(adresse, ListeDevices.PLUIETOTAL.ToString, valeur) 'mm
                valeur = Hex(recbuf(6) And &HF)


                'RAP ?????
                'WriteRetour(adresse, "RAP", valeur) 'flip cnt



                If (recbuf(4) And &H4) <> 0 Then WriteBattery(adresse)
                'checksum2()

            ElseIf recbuf(0) = &H2A And recbuf(1) = &H19 And recbits >= 84 Then
                '------------- PCR800 ---------------
                oregon = True
                adresse = CStr(recbuf(3) * 256)
                valeur = CStr(Round((((CSng(Hex(recbuf(5))) + CSng(Hex((recbuf(4) >> 4) And &HF))) / 10 + CSng(Hex((recbuf(4) And &HF))) / 100) * 25.4), 2)) ' mm/hr"
                WriteRetour(adresse, ListeDevices.PLUIECOURANT.ToString, valeur)
                valeur = (CSng(Hex(recbuf(7))) / 100 + CSng(Hex(recbuf(6) >> 4)) / 1000)
                valeur = CStr(Round(((valeur + (CSng(Hex(recbuf(9) And &HF)) * 100 + CSng(Hex(recbuf(8))))) * 25.4), 2))
                WriteRetour(adresse, ListeDevices.PLUIETOTAL.ToString, valeur) 'mm
                If (recbuf(4) And &H4) <> 0 Then WriteBattery(adresse)
                'checksumr()

            ElseIf recbuf(0) = &H6 And recbuf(1) = &HE4 And recbits >= 84 Then
                '------------- RAIN XX ---------------
                oregon = True
                adresse = CStr(recbuf(3) * 256)
                valeur = CStr(Round((((CSng(Hex(recbuf(5))) + CSng(Hex((recbuf(4) >> 4) And &HF))) / 10 + CSng(Hex((recbuf(4) And &HF))) / 100) * 25.4), 2)) ' mm/hr"
                WriteRetour(adresse, ListeDevices.PLUIECOURANT.ToString, valeur)
                valeur = (CSng(Hex(recbuf(7))) / 100 + CSng(Hex(recbuf(6) >> 4)) / 1000)
                valeur = CStr(Round(((valeur + (CSng(Hex(recbuf(9) And &HF)) * 100 + CSng(Hex(recbuf(8))))) * 25.4), 2))
                WriteRetour(adresse, ListeDevices.PLUIETOTAL.ToString, valeur) 'mm
                If (recbuf(4) And &H4) <> 0 Then WriteBattery(adresse)
                'checksumr()

            ElseIf recbuf(0) = &H1A And recbuf(1) = &H99 And recbits >= 80 Then
                '------------- WTGR800 ---------------
                oregon = True
                adresse = CStr(recbuf(3) * 256)
                valeur = CStr(CSng(recbuf(4) >> 4) * 22.5)
                'WriteRetour(adresse & "_WID", valeur) '°
                valeur = wrdirection(direction)
                WriteRetour(adresse, ListeDevices.DIRECTIONVENT.ToString, valeur) 'direction en lettres
                valeur = (CSng(Hex(recbuf(7) And &HF)) * 10) + (CSng(Hex(recbuf(6))) / 10)
                WriteRetour(adresse, ListeDevices.VITESSEVENT.ToString, valeur) 'vitesse en m/s
                valeur = wrspeed(valeur)
                'WriteRetour(adresse & "_WIF", valeur) 'vitesse en Force
                WriteRetour(adresse, ListeDevices.VITESSEVENT.ToString, valeur) 'vitesse en Force
                ' autre mesure mais je sais pas a quoi ca correpond
                'speed = CSng(Hex(recbuf(8))) + (CSng(Hex((recbuf(7) >> 4) And &HF)) / 10)
                'WriteMessage(" av.", False)
                'wrspeed(speed)
                valeur = wrbattery()
                WriteRetour(adresse, ListeDevices.BATTERIE.ToString, valeur)
                'checksum9()

            ElseIf recbuf(0) = &H1A And recbuf(1) = &H89 And recbits >= 80 Then
                '------------- WGR800 ---------------
                oregon = True
                adresse = CStr(recbuf(3) * 256)
                valeur = CStr(CSng(recbuf(4) >> 4) * 22.5)
                'WriteRetour(adresse & "_WID", valeur) '°
                valeur = wrdirection(direction)
                WriteRetour(adresse, ListeDevices.DIRECTIONVENT.ToString, valeur) 'direction en lettres
                valeur = (CSng(Hex(recbuf(7) And &HF)) * 10) + (CSng(Hex(recbuf(6))) / 10)
                WriteRetour(adresse, ListeDevices.VITESSEVENT.ToString, valeur) 'vitesse en m/s
                valeur = wrspeed(valeur)
                'WriteRetour(adresse & "_WIF", valeur) 'vitesse en Force
                ' autre mesure mais je sais pas a quoi ca correpond
                'speed = CSng(Hex(recbuf(8))) + (CSng(Hex((recbuf(7) >> 4) And &HF)) / 10)
                'WriteMessage(" av.", False)
                'wrspeed(speed)
                valeur = wrbattery()
                WriteRetour(adresse, ListeDevices.BATTERIE.ToString, valeur)
                'checksum9()

            ElseIf recbuf(0) = &H3A And recbuf(1) = &HD And recbits >= 80 Then
                '------------- STR918,WGR918 ---------------
                oregon = True
                adresse = CStr(recbuf(3) * 256)
                valeur = CStr(CSng(recbuf(4) >> 4) * 22.5)
                'WriteRetour(adresse & "_WID", valeur) '°
                valeur = wrdirection(direction)
                WriteRetour(adresse, ListeDevices.DIRECTIONVENT.ToString, valeur) 'direction en lettres
                valeur = (CSng(Hex(recbuf(7) And &HF)) * 10) + (CSng(Hex(recbuf(6))) / 10)
                WriteRetour(adresse, ListeDevices.VITESSEVENT.ToString, valeur) 'vitesse en m/s
                valeur = wrspeed(valeur)
                'WriteRetour(adresse & "_WIF", valeur) 'vitesse en Force
                ' autre mesure mais je sais pas a quoi ca correpond
                'speed = CSng(Hex(recbuf(8))) + (CSng(Hex((recbuf(7) >> 4) And &HF)) / 10)
                'WriteMessage(" av.", False)
                'wrspeed(speed)
                valeur = wrbattery()
                WriteRetour(adresse, ListeDevices.BATTERIE.ToString, valeur)
                'checksum9()

            ElseIf recbuf(0) = &HEA And recbuf(1) = &H7C And recbits >= 60 Then
                '------------- UVR138 ---------------
                oregon = True
                adresse = CStr(recbuf(3) * 256)
                uv = CSng(Hex(recbuf(5) And &HF)) * 10 + CSng(Hex(recbuf(4) >> 4))
                WriteRetour(adresse, ListeDevices.UV.ToString, CStr(uv)) 'en chiffre
                'If uv < 3 Then
                '    WriteRetour(adresse & "_UVL", "Low") 'en level
                'ElseIf uv < 6 Then
                '    WriteRetour(adresse & "_UVL", "Medium") 'en level
                'ElseIf uv < 8 Then
                '    WriteRetour(adresse & "_UVL", "High") 'en level
                'ElseIf uv < 11 Then
                '    WriteRetour(adresse & "_UVL", "Very High") 'en level
                'Else
                '    WriteRetour(adresse & "_UVL", "Dangerous") 'en level
                'End If
                If (recbuf(4) And &H4) = &H4 Then WriteBattery(adresse)
                'checksumw()

            ElseIf recbuf(0) = &HDA And recbuf(1) = &H78 And recbits >= 64 Then
                '------------- UVN800 ---------------
                oregon = True
                adresse = CStr((recbuf(3)) * 256)
                uv = CSng(Hex(recbuf(5) And &HF)) * 10 + CSng(Hex(recbuf(4) >> 4))
                WriteRetour(adresse, ListeDevices.UV.ToString, CStr(uv)) 'en chiffre
                'If uv < 3 Then
                '    WriteRetour(adresse & "_UVL", "Low") 'en level
                'ElseIf uv < 6 Then
                '    WriteRetour(adresse & "_UVL", "Medium") 'en level
                'ElseIf uv < 8 Then
                '    WriteRetour(adresse & "_UVL", "High") 'en level
                'ElseIf uv < 11 Then
                '    WriteRetour(adresse & "_UVL", "Very High") 'en level
                'Else
                '    WriteRetour(adresse & "_UVL", "Dangerous") 'en level
                'End If
                If (recbuf(4) And &H4) = &H4 Then WriteBattery(adresse)
                'checksum7()

            ElseIf recbuf(0) = &H8A And recbuf(1) = &HEC And recbits >= 96 Then
                '------------- RTGR328N ---------------
                oregon = True
                adresse = CStr(recbuf(3) * 256 + (recbuf(2) >> 4))
                'ch = wrchannel3()
                hr = VB.Right("0" & CStr(CSng(recbuf(7) And &HF) * 10 + CSng(recbuf(6) >> 4)), 2)
                mn = VB.Right("0" & CStr(CSng(recbuf(6) And &HF) * 10 + CSng(recbuf(5) >> 4)), 2)
                sc = VB.Right("0" & CStr(CSng(recbuf(5) And &HF) * 10 + CSng(recbuf(4) >> 4)), 2)
                valeur = " time=" & CStr(hr) & ":" & CStr(mn) & ":" & CStr(sc)
                Select Case recbuf(9) And &H7
                    Case 0 : dag = " Sunday"
                    Case 1 : dag = " Monday"
                    Case 2 : dag = " Tuesday"
                    Case 3 : dag = " Wednesday"
                    Case 4 : dag = " Thursday"
                    Case 5 : dag = " Friday"
                    Case 6 : dag = " Saterday"
                    Case Else : dag = " day error"
                End Select
                valeur = valeur & dag
                dd = CSng(recbuf(8) And &HF) * 10 + CSng(recbuf(7) >> 4)
                mm = CSng(recbuf(8) >> 4)
                yy = CSng(recbuf(10) And &HF) * 10 + CSng(recbuf(9) >> 4) + 2000
                valeur = valeur & " " & dd & "-" & mm & "-" & yy
                'WriteRetour(adresse, valeur) 'renvoie la date/heure
                'checksum11()

            ElseIf recbuf(0) = &HEA And (recbuf(1) And &HC0) = &H0 And recbits >= 64 Then
                '------------- cent-a-meter ---------------
                oregon = True
                adresse = CStr(recbuf(2) * 256)
                ' WriteMessage(" counter:" & CSng(recbuf(1) And &HF), False)
                valeur = CStr((CSng(recbuf(3)) + CSng((recbuf(4) And &H3) * 256)) / 10)
                WriteRetour(adresse & "-1", ListeDevices.COMPTEUR.ToString, valeur) 'en Ampere
                valeur = CStr((CSng((recbuf(4) >> 2) And &H3F) + CSng((recbuf(5) And &HF) * 64)) / 10)
                WriteRetour(adresse & "-2", ListeDevices.COMPTEUR.ToString, valeur) 'en Ampere
                valeur = CStr((CSng((recbuf(5) >> 4) And &HF) + CSng((recbuf(6) And &H3F) * 16)) / 10)
                WriteRetour(adresse & "-3", ListeDevices.COMPTEUR.ToString, valeur) 'en Ampere
                'checksume()

            ElseIf recbits = 56 Then
                '------------- BWR101,BWR102 ---------------
                oregon = True
                adresse = CStr(recbuf(1) >> 4)
                If IsNumeric(Hex(recbuf(4))) And IsNumeric(Hex(recbuf(3) >> 4)) Then
                    'WriteMessage(" addr:" & Hex(recbuf(1) >> 4), False)
                    valeur = CStr(CSng(Hex(recbuf(5) And &H1)) * 100 + CSng(Hex(recbuf(4))) + CSng(Hex(recbuf(3) >> 4)) / 10)
                    WriteRetour(adresse, ListeDevices.GENERIQUEVALUE.ToString, valeur) 'en kg
                    'WriteMessage(" Unknown byte=" & CStr(Hex(recbuf(3) And &HF)) & CStr(Hex(recbuf(2) >> 4)), False)
                    'If Not (((recbuf(0) And &HF0) = (recbuf(5) And &HF0)) And ((recbuf(1) And &HF) = (recbuf(6) And &HF))) Then
                    '    WriteRetour(adresse, "ERR: Checksum error")
                    'End If
                Else
                    WriteRetour(adresse, ListeDevices.GENERIQUEVALUE.ToString, "ERR: weight value is not a decimal value.")
                End If

            ElseIf (recbuf(0) And &HF) = &H3 And recbits = 64 Then
                '------------- GR101 ---------------
                'Dim i As Integer
                oregon = True
                adresse = CStr(recbuf(1) >> 4)
                'For i = 7 To 0 Step -1
                '    WriteMessage(VB.Right("0" & Hex(recbuf(i)), 2), False)
                'Next
                valeur = CStr(Round((((recbuf(4) And &HF) * 4096) + (recbuf(3) * 16) + (recbuf(2) >> 4) / 400.8), 1))
                WriteRetour(adresse, ListeDevices.GENERIQUEVALUE.ToString, valeur) 'en kg

                ' 
            ElseIf (recbuf(0) = &H1A Or recbuf(0) = &H2A Or recbuf(0) = &H3A) And recbits = 108 Then
                '------------- OWL CM119 ---------------
                oregon = True
                adresse = CStr(recbuf(2) * 256)
                'addr:" & Hex(RecBuf[2], 2)
                'counter:" & CSng(RecBuf[1] AND &HF)
                valeur = CLng(recbuf(10)) << 36
                valeur += CLng(recbuf(9)) << 28
                valeur += CLng(recbuf(8)) << 20
                valeur += CLng(recbuf(7)) << 12
                valeur += CLng(recbuf(6)) << 4
                valeur += (CLng(recbuf(5)) >> 4) And &HF
                valeur = valeur / 223000
                'Checksum12() 
                WriteRetour(adresse, ListeDevices.ENERGIETOTALE.ToString, valeur) 'total en kWh
                WriteRetour(adresse, ListeDevices.ENERGIEINSTANTANEE.ToString, CSng((recbuf(5) And &HF) * 65536) + CSng(recbuf(4) * 256) + CSng(recbuf(3))) 'now en Watt
            End If
            Return oregon
        Catch ex As Exception
            WriteLog("ERR: RFXCOM processoregon : " & ex.Message)
        End Try
    End Function

#End Region

#Region "Fonctions"

    Function createhsaddr() As Integer
        Try
            Dim hsaddr As Integer = 0
            If (recbuf(0) And &H1) <> 0 Then hsaddr = hsaddr Or &H80
            If (recbuf(0) And &H2) <> 0 Then hsaddr = hsaddr Or &H40
            If (recbuf(0) And &H4) <> 0 Then hsaddr = hsaddr Or &H20
            If (recbuf(0) And &H8) <> 0 Then hsaddr = hsaddr Or &H10
            If (recbuf(0) And &H10) <> 0 Then hsaddr = hsaddr Or &H8
            If (recbuf(0) And &H20) <> 0 Then hsaddr = hsaddr Or &H4
            If (recbuf(0) And &H40) <> 0 Then hsaddr = hsaddr Or &H2
            If (recbuf(0) And &H80) <> 0 Then hsaddr = hsaddr Or &H1
            If (recbuf(1) And &H1) <> 0 Then hsaddr = hsaddr Or &H8000
            If (recbuf(1) And &H2) <> 0 Then hsaddr = hsaddr Or &H4000
            If (recbuf(1) And &H4) <> 0 Then hsaddr = hsaddr Or &H2000
            If (recbuf(1) And &H8) <> 0 Then hsaddr = hsaddr Or &H1000
            If (recbuf(1) And &H10) <> 0 Then hsaddr = hsaddr Or &H800
            If (recbuf(1) And &H20) <> 0 Then hsaddr = hsaddr Or &H400
            If (recbuf(1) And &H40) <> 0 Then hsaddr = hsaddr Or &H200
            If (recbuf(1) And &H80) <> 0 Then hsaddr = hsaddr Or &H100
            Return hsaddr
        Catch ex As Exception
            WriteLog("ERR: RFXCOM createhsaddr : " & ex.Message)
            Return "ERR: " & ex.Message
        End Try
    End Function

    Public Function wrdirection(ByVal direction As Integer) As String
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
            WriteLog("ERR: RFXCOM wrdirection : " & ex.Message)
            Return "ERR: " & ex.Message
        End Try
    End Function

    Public Function wrspeed(ByVal speed As Single) As String
        Try
            'WriteMessage(" speed " & CStr(speed) & " m/sec", False)
            If speed < 0.2 Then
                Return "0"
            ElseIf speed < 1.6 Then
                Return "1"
            ElseIf speed < 3.4 Then
                Return "2"
            ElseIf speed < 5.5 Then
                Return "3"
            ElseIf speed < 8 Then
                Return "4"
            ElseIf speed < 10.8 Then
                Return "5"
            ElseIf speed < 13.9 Then
                Return "6"
            ElseIf speed < 17.2 Then
                Return "7"
            ElseIf speed < 20.8 Then
                Return "8"
            ElseIf speed < 25.4 Then
                Return "9"
            ElseIf speed < 28.5 Then
                Return "10"
            ElseIf speed < 32.7 Then
                Return "11"
            Else
                Return "12"
            End If
            'WriteMessage("Bft ", False)
        Catch ex As Exception
            WriteLog("ERR: RFXCOM wrspeed : " & ex.Message)
            Return "ERR: " & ex.Message
        End Try
    End Function

    Function wrbattery() As String
        Try
            Select Case (recbuf(4) And &HF)
                Case 0 : Return "battery 100%"
                Case 1 : Return "battery 90%"
                Case 2 : Return "battery 80%"
                Case 3 : Return "battery 70%"
                Case 4 : Return "battery 60%"
                Case 5 : Return "battery 50%"
                Case 6 : Return "battery 40%"
                Case 7 : Return "battery 30%"
                Case 8 : Return "battery 20%"
                Case 9 : Return "battery 10%"
                Case Else : Return ""
            End Select
        Catch ex As Exception
            WriteLog("ERR: RFXCOM wrbattery : " & ex.Message)
            Return "ERR: " & ex.Message
        End Try
    End Function

    Function wrchannel() As Byte
        Try
            Select Case (recbuf(2) And &H70)
                Case &H10
                    'WriteMessage(" CH 1", False)
                    wrchannel = 1
                Case &H20
                    'WriteMessage(" CH 2", False)
                    wrchannel = 2
                Case &H40
                    'WriteMessage(" CH 3", False)
                    wrchannel = 4
                Case Else
                    ' WriteMessage(" CH ? = " & VB.Right("0" & Hex(recbuf(2)), 2), False)
                    wrchannel = 0
            End Select
        Catch ex As Exception
            WriteLog("ERR: RFXCOM wrchannel : " & ex.Message)
            wrchannel = 0
        End Try
    End Function

    Function wrchannel3() As Byte
        Try
            'WriteMessage(" CH " & (recbuf(2) >> 4), False)
            wrchannel3 = (recbuf(2) >> 4)
        Catch ex As Exception
            WriteLog("ERR: RFXCOM wrchannel3 : " & ex.Message)
            wrchannel3 = 0
        End Try
    End Function

    Function wrforecast(ByVal forecast As Byte) As String
        Try
            Select Case forecast
                Case &HC : Return "Sunny"
                Case &H6 : Return "Partly"
                Case &H2 : Return "Cloudy"
                Case &H3 : Return "Rain"
                Case Else : Return "forecast ??"
            End Select
        Catch ex As Exception
            WriteLog("ERR: RFXCOM wrforecast : " & ex.Message)
            Return "ERR: " & ex.Message
        End Try
    End Function

    Function wrhum(ByVal hum As Byte) As String
        Try
            Select Case hum
                Case &H0 : Return "Normal"
                Case &H40 : Return "Comfort"
                Case &H80 : Return "Dry"
                Case &HC0 : Return "Wet"
                Case Else : Return ""
            End Select
        Catch ex As Exception
            WriteLog("ERR: RFXCOM wrhum : " & ex.Message)
            Return "ERR: " & ex.Message
        End Try
    End Function

    Function cs8() As Byte
        Try
            Dim cs As Byte

            cs = (recbuf(0) >> 4 And &HF) + (recbuf(0) And &HF)
            cs += (recbuf(1) >> 4 And &HF) + (recbuf(1) And &HF)
            cs += (recbuf(2) >> 4 And &HF) + (recbuf(2) And &HF)
            cs += (recbuf(3) >> 4 And &HF) + (recbuf(3) And &HF)
            cs += (recbuf(4) >> 4 And &HF) + (recbuf(4) And &HF)
            cs += (recbuf(5) >> 4 And &HF) + (recbuf(5) And &HF)
            cs += (recbuf(6) >> 4 And &HF) + (recbuf(6) And &HF)
            cs += (recbuf(7) >> 4 And &HF) + (recbuf(7) And &HF)
            Return cs
        Catch ex As Exception
            WriteLog("ERR: RFXCOM cs8 : " & ex.Message)
            Return "ERR: " & ex.Message
        End Try
    End Function

    Sub checksume()
        Try
            Dim cs As Short
            cs = (recbuf(0) >> 4 And &HF) + (recbuf(0) And &HF)
            cs += (recbuf(1) >> 4 And &HF) + (recbuf(1) And &HF)
            cs += (recbuf(2) >> 4 And &HF) + (recbuf(2) And &HF)
            cs += (recbuf(3) >> 4 And &HF) + (recbuf(3) And &HF)
            cs += (recbuf(4) >> 4 And &HF) + (recbuf(4) And &HF)
            cs += (recbuf(5) >> 4 And &HF) + (recbuf(5) And &HF)
            cs += (recbuf(6) >> 4 And &HF) + (recbuf(6) And &HF)
            cs = (cs - recbuf(7)) And &HFF
            If cs <> &H18 Then
                'WriteMessage(" Checksum Error", False)
            End If
        Catch ex As Exception
            WriteLog("ERR: RFXCOM checksume : " & ex.Message)
        End Try
    End Sub

    Sub checksum7()
        Try
            Dim cs As Short
            cs = (recbuf(0) >> 4 And &HF) + (recbuf(0) And &HF)
            cs += (recbuf(1) >> 4 And &HF) + (recbuf(1) And &HF)
            cs += (recbuf(2) >> 4 And &HF) + (recbuf(2) And &HF)
            cs += (recbuf(3) >> 4 And &HF) + (recbuf(3) And &HF)
            cs += (recbuf(4) >> 4 And &HF) + (recbuf(4) And &HF)
            cs += (recbuf(5) >> 4 And &HF) + (recbuf(5) And &HF)
            cs += (recbuf(6) >> 4 And &HF) + (recbuf(6) And &HF)
            cs = (cs - recbuf(7)) And &HFF
            If cs <> &HA Then
                'WriteMessage(" Checksum Error", False)
            End If
        Catch ex As Exception
            WriteLog("ERR: RFXCOM checksum7 : " & ex.Message)
        End Try
    End Sub

    Sub checksum8()
        Try
            Dim cs As Short
            cs = cs8()
            cs = (cs - recbuf(8)) And &HFF
            If cs <> &HA Then
                ' WriteMessage(" Checksum Error", False)
            End If
        Catch ex As Exception
            WriteLog("ERR: RFXCOM checksum8 : " & ex.Message)
        End Try
    End Sub

    Sub checksum2()
        Try
            Dim cs As Short
            cs = cs8()
            cs += recbuf(8) And &HF
            cs = (cs - ((recbuf(8) >> 4 And &HF) + ((recbuf(9) << 4) And &HF0))) And &HFF
            If cs <> &HA Then
                'WriteMessage(" Checksum Error", False)
            End If
        Catch ex As Exception
            WriteLog("ERR: RFXCOM checksum2 : " & ex.Message)
        End Try
    End Sub

    Sub checksum9()
        Try
            Dim cs As Short
            cs = cs8()
            cs += (recbuf(8) >> 4 And &HF) + (recbuf(8) And &HF)
            cs = (cs - recbuf(9)) And &HFF
            If cs <> &HA Then
                'WriteMessage(" Checksum Error", False)
            End If
        Catch ex As Exception
            WriteLog("ERR: RFXCOM checksum9 : " & ex.Message)
        End Try
    End Sub

    Sub checksum10()
        Try
            Dim cs As Short
            cs = cs8()
            cs += (recbuf(8) >> 4 And &HF) + (recbuf(8) And &HF)
            cs += (recbuf(9) >> 4 And &HF) + (recbuf(9) And &HF)
            cs = (cs - recbuf(10)) And &HFF
            If cs <> &HA Then
                'WriteMessage(" Checksum Error", False)
            End If
        Catch ex As Exception
            WriteLog("ERR: RFXCOM checksum10 : " & ex.Message)
        End Try
    End Sub

    Sub checksum11()
        Try
            Dim cs As Short
            cs = cs8()
            cs += (recbuf(8) >> 4 And &HF) + (recbuf(8) And &HF)
            cs += (recbuf(9) >> 4 And &HF) + (recbuf(9) And &HF)
            cs += (recbuf(10) >> 4 And &HF) + (recbuf(10) And &HF)
            cs = (cs - recbuf(11)) And &HFF
            If cs <> &HA Then
                'WriteMessage(" Checksum Error", False)
            End If
        Catch ex As Exception
            WriteLog("ERR: RFXCOM checksum11 : " & ex.Message)
        End Try
    End Sub

    Function checksumw() As Byte
        Try
            Dim cs As Short

            cs = (recbuf(0) >> 4 And &HF) + (recbuf(0) And &HF)
            cs += (recbuf(1) >> 4 And &HF) + (recbuf(1) And &HF)
            cs += (recbuf(2) >> 4 And &HF) + (recbuf(2) And &HF)
            cs += (recbuf(3) >> 4 And &HF) + (recbuf(3) And &HF)
            cs += (recbuf(4) >> 4 And &HF) + (recbuf(4) And &HF)
            cs += (recbuf(5) >> 4 And &HF) + (recbuf(5) And &HF)
            cs += (recbuf(6) And &HF)
            cs = (cs - ((recbuf(6) >> 4 And &HF) + (recbuf(7) << 4 And &HF0))) And &HFF
            If cs <> &HA Then
                'WriteMessage(" Checksum Error", False)
            End If
            Return cs
        Catch ex As Exception
            WriteLog("ERR: RFXCOM checksumw : " & ex.Message)
            Return "ERR: " & ex.Message
        End Try
    End Function

    Function checksumr() As Byte
        Try
            Dim cs As Short

            cs = cs8()
            cs += (recbuf(8) >> 4 And &HF) + (recbuf(8) And &HF)
            cs += (recbuf(9) And &HF)
            cs = (cs - ((recbuf(9) >> 4 And &HF) + (recbuf(10) << 4 And &HF0))) And &HFF
            If cs <> &HA Then
                ' WriteMessage(" Checksum Error", False)
            End If
            Return cs
        Catch ex As Exception
            WriteLog("ERR: RFXCOM checksumr : " & ex.Message)
            Return "ERR: " & ex.Message
        End Try
    End Function

#End Region

#Region "Write"

    Public Sub WriteLog(ByVal message As String)
        Try
            'utilise la fonction de base pour loguer un event
            If STRGS.InStr(message, "DBG:") > 0 Then
                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "RFXCOM_RECEIVER ", STRGS.Right(message, message.Length - 4))
            ElseIf STRGS.InStr(message, "ERR:") > 0 Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXCOM_RECEIVER ", STRGS.Right(message, message.Length - 4))
            Else
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFXCOM_RECEIVER ", STRGS.Right(message, message.Length - 4))
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXCOM_RECEIVER WriteLog", ex.Message)
        End Try
    End Sub

    Public Sub WriteBattery(ByVal adresse As String)
        Try
            'Dim tabletmp() As DataRow

            'log tous les paquets en mode debug
            WriteLog("DBG: WriteBattery : receive from " & adresse)

            'on ne traite rien pendant les 6 premieres secondes
            If DateTime.Now > DateAdd(DateInterval.Second, 6, dateheurelancement) Then
                'Recherche si un device affecté
                Dim listedevices As New ArrayList
                listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, adresse, "", Me._ID, True)
                If (listedevices.Count >= 1) Then
                    'on a trouvé un ou plusieurs composants avec cette adresse, on prend le premier
                    WriteLog(listedevices.Item(0)._Name & " (" & adresse & ") : Battery Empty")
                Else
                    'device pas trouvé
                    WriteLog("ERR: Device non trouvé : RFXCOM " & adresse & ": Battery Empty")

                    'Ajouter la gestion des composants bannis (si dans la liste des composant bannis alors on log en debug sinon onlog device non trouve empty)

                End If
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXCOM_RECEIVER WriteBattery", ex.Message & " --> " & adresse)
        End Try
    End Sub

    Public Sub WriteRetour(ByVal adresse As String, ByVal type As String, ByVal valeur As String)
        Try
            If Not _IsConnect Then Exit Sub 'si on ferme le port on quitte

            'Forcer le . 
            Thread.CurrentThread.CurrentCulture = New CultureInfo("en-US")
            My.Application.ChangeCulture("en-US")

            'log tous les paquets en mode debug
            'WriteLog("DBG: WriteRetour receive from " & adresse & " (" & type & ") -> " & valeur)

            'on ne traite rien pendant les 6 premieres secondes
            If DateTime.Now > DateAdd(DateInterval.Second, 6, dateheurelancement) Then
                'Recherche si un device affecté
                Dim listedevices As New ArrayList
                listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, adresse, type, Me._ID, True)
                If (listedevices.Count = 1) Then
                    'un device trouvé on maj la value
                    If valeur = "ON" Then
                        listedevices.Item(0).Value = True
                    ElseIf valeur = "OFF" Then
                        listedevices.Item(0).Value = False
                    Else
                        listedevices.Item(0).Value = valeur
                    End If
                ElseIf (listedevices.Count > 1) Then
                    WriteLog("ERR: Plusieurs devices correspondent à : " & type & " " & adresse & ":" & valeur)
                Else
                    'on vérifie si le device est configuré en RFXMitter
                    listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, adresse, type, "C2B6AA22-77E7-11E0-A193-47D34824019B", True)
                    If (listedevices.Count = 1) Then
                        'un device trouvé on maj la value
                        If valeur = "ON" Then
                            listedevices.Item(0).Value = True
                        ElseIf valeur = "OFF" Then
                            listedevices.Item(0).Value = False
                        Else
                            listedevices.Item(0).Value = valeur
                        End If
                    Else
                        WriteLog("ERR: Device non trouvé : " & type & " " & adresse & ":" & valeur)
                    End If

                    'Ajouter la gestion des composants bannis (si dans la liste des composant bannis alors on log en debug sinon onlog device non trouve empty)


                End If
            End If
            adresselast = adresse
            valeurlast = valeur
            nblast = 1
        Catch ex As Exception
            WriteLog("ERR: Writeretour Exception : " & ex.Message)
        End Try
    End Sub

#End Region

End Class
