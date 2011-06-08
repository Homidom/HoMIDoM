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

''' <summary>Class Driver_RFXMitter, permet de communiquer avec le RFXMitter Ethernet/COM</summary>
''' <remarks>Pour la version USB, necessite l'installation du driver USB RFXCOM</remarks>
<Serializable()> Public Class Driver_RFXMitter
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "C2B6AA22-77E7-11E0-A193-47D34824019B"
    Dim _Nom As String = "RFXMitter"
    Dim _Enable As String = False
    Dim _Description As String = "RFXMitter Ethernet Interface"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "RF"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = ""
    Dim _Port_TCP As String = "10002"
    Dim _IP_UDP As String = ""
    Dim _Port_UDP As String = ""
    Dim _Com As String = ""
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "RFXMitter"
    Dim _Version As String = "1.0"
    Dim _Picture As String = "rfxcom.png"
    Dim _Server As HoMIDom.HoMIDom.Server
    Dim _Device As HoMIDom.HoMIDom.Device
    Dim _DeviceSupport As New ArrayList
    Dim _Parametres As New ArrayList
    Dim MyTimer As New Timers.Timer

    'Ajoutés dans les ppt avancés dans New()
    Dim rfxsynchro As Boolean = True 'synchronisation avec le receiver
#End Region

#Region "Variables Internes"
    Public Enum DEVICE_COMMAND As Integer
        'Device values for output devices
        All_Units_Off = 0
        All_Lights_On = 1
        UOn = 2
        UOff = 3
        UDim = 4
        UBright = 5
        All_Lights_Off = 6
        Extended_Code = 7
        Hail_Request = 8
        Hail_Ack = 9
        Preset_Dim1 = 10
        Preset_Dim2 = 11
        Ex_Data_Xfer = 12
        Status_On = 13
        Status_Off = 14
        Status_Request = 15
        Dim_To_Off = 16
        No_Cmd = 17
        Stat_Unknown = 17
        Any_Cmd = 18
        Value_Set = 19
        Value_Increment = 20
        Set_On = 21
        Set_Off = 22
        Set_Any = 23
        Value_Decrement = 24
    End Enum

    'REMOTE commands
    Public Enum SEC_REMOTE As Integer
        None = 0
        ArmAway = 1
        ArmAwayMaxDelay = 2
        ArmHome = 3
        ArmHomeMaxDelay = 4
        Disarm = 5
        LightsOn = 6
        LightsOff = 7
        Panic = 8
        LightsOn2 = 9
        LightsOff2 = 10
        Disabled = 11
        BatteryLow = 12 'not used as device command, only used to display batt low for CO18 and SD18
        StopPanic = 13
        Alarm = 14
        AnyValue = 999
    End Enum

    'states of gintXmitstate(ixmit):
    Public Enum XmitState As Integer
        Reconnect = -1      ' reconnecting TCP
        OffLine = 0         ' xmitter not used
        Initializing = 1    ' initializing transmitter
        WaitInit = 2        ' wait for Ack on Init to Var length
        WaitResetRF = 3     ' wait for Ack on Reset RF
        WaitDisableX10 = 4  ' wait for Ack on Disable X10
        WaitEnableKAKU = 5  ' wait for Ack on Enable KAKU
        WaitEnableHAR = 6   ' wait for Ack on Enable Harrison
        EndInit = 20        ' transmitter initialized
        WaitAck = 21        ' wait for Ack, transmitter ready
        Idle = 22           ' idle
    End Enum

    Public Enum XmitInitCmd As Byte
        GetSWversion = &H30 ' return transmitter software version init RF
        InitHS = &H33       ' init transmitter with use of HS
        InitNoHS = &H37     ' init transmitter no handshake
        EnableHar = &H3C    ' Enable Harrison
        EnableARC = &H3D    ' Enable ARC RF
        EnableFLA = &H3E    ' Enable Koppla or Flamingo RF
        DisableX10 = &H3F   ' Disable X10 RF
    End Enum

    Private WithEvents port As New System.IO.Ports.SerialPort
    Private port_name As String = ""
    'liste des variables de base
    Private slave As Boolean
    Private mess As Boolean = False
    Private currentdevice As Byte
    Private currentunit As Byte
    Private pulse As Char
    Private firstbyte As Boolean = True
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

    Const GETSW As Byte = &H30
    Const MODEBLK As Byte = &H31
    Const PING As Byte = &H32
    Const MODERBRB48 As Byte = &H33
    Const MODECONT As Byte = &H35
    Const MODEBRB48 As Byte = &H37

    Private protocolsynchro As Integer = MODEBRB48
    Private ack As Boolean = False
    Private ack_ok As Boolean = True

#End Region

#Region "Propriétés génériques"
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

        'récupération des paramétres avancés
        Try
            rfxsynchro = _Parametres.Item(0).Valeur
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXmitter Start", "Erreur dans les paramétres avancés. utilisation des valeur par défaut" & ex.Message)
        End Try

        'ouverture du port suivant le Port Com ou IP
        Try
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
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter", "Driver non démarré : " & retour)
            Else
                'le driver est démarré, on log puis on lance les handlers
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFXMitter", "Driver démarré : " & retour)
                retour = lancer()
                If STRGS.Left(retour, 4) = "ERR:" Then
                    retour = STRGS.Right(retour, retour.Length - 5)
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter", retour & " non lancé, arrêt du driver")
                    [Stop]()
                Else
                    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFXMitter", retour)
                    'les handlers sont lancés, on configure le rfxmitter
                    retour = configurer()
                    If STRGS.Left(retour, 4) = "ERR:" Then
                        retour = STRGS.Right(retour, retour.Length - 5)
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter", retour)
                        [Stop]()
                    Else
                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFXMitter", retour)
                    End If
                End If
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter Start", ex.Message)
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Dim retour As String
        retour = fermer()
        If STRGS.Left(retour, 4) = "ERR:" Then
            retour = STRGS.Right(retour, retour.Length - 5)
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter Stop", retour)
        Else
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFXMitter Stop", retour)
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
        Try
            If _Enable = False Then Exit Sub
            'suivant le protocole, on lance la bonne fonction

            'CHACON / X10 / ARC / WAVEMAN
            Select Case Objet.Modele.ToString.ToUpper
                Case "CHACON"

                Case "X10"

                Case "ARC"

                Case "WAVEMAN"

                Case Else

            End Select



            'ecrire(&HF0, Parametre1)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter WRITE", ex.Message)
        End Try
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

        'Paramétres avancés
        Dim x As New HoMIDom.HoMIDom.Driver.Parametre
        x.Nom = "synchro"
        x.Description = "Synchronisation avec le receiver (True/False)"
        x.Valeur = True
        _Parametres.Add(x)

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
                    'RFXMitter est un modele ethernet
                    tcp = True
                    client = New TcpClient(numero, _Port_TCP)
                    _IsConnect = True
                    Return ("Port IP " & port_name & ":" & _Port_TCP & " ouvert")
                Else
                    'RFXMitter est un modele usb
                    tcp = False
                    port.PortName = port_name 'nom du port : COM1
                    port.BaudRate = 4800 'vitesse du port 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 57600, 115200
                    port.Parity = Parity.None 'pas de parité
                    port.StopBits = StopBits.Two '2 bits d'arrêt par octet
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
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter LANCER GETSTREAM", ex.Message)
                Return "ERR: Handler IP"
            End Try
        Else
            Try
                tmrRead.Enabled = True
                AddHandler port.DataReceived, New SerialDataReceivedEventHandler(AddressOf DataReceived)
                AddHandler port.ErrorReceived, New SerialErrorReceivedEventHandler(AddressOf ReadErrorEvent)
                Return "Handler COM OK"
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter LANCER Serial", ex.Message)
                Return "ERR: Handler COM"
            End Try
        End If
    End Function

    ''' <summary>Configurer le RFXMitter</summary>
    ''' <remarks></remarks>
    Private Function configurer() As String
        'configurer le rfxmitter
        Try
            'get firmware version
            Dim kar1() As Byte = {&HF0, GETSW, &HF0, GETSW}
            ecrire(kar1)

            'configuration de la synchronisation avec le receiver (param avancé)
            If rfxsynchro Then
                Dim kar() As Byte = {&HF0, MODERBRB48, &HF0, MODERBRB48}
                protocolsynchro = MODERBRB48
                ecrire(kar)
            Else
                Dim kar() As Byte = {&HF0, MODEBRB48, &HF0, MODEBRB48}
                protocolsynchro = MODEBRB48
                ecrire(kar)
            End If

            Return "Configuration OK"
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter LANCER Configuration", ex.Message)
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
    ''' <remarks></remarks>
    Private Function ecrire(ByVal commande() As Byte) As String
        Dim message As String = ""
        Try
            If tcp Then
                stream.Write(commande, 0, commande.Length)
            Else
                port.Write(commande, 0, commande.Length)
            End If
            Return ""
        Catch ex As Exception
            Return ("ERR: " & ex.Message)
        End Try
    End Function

    Private Sub ecrirecommande(ByVal kar As Byte())
        Dim message As String
        Dim temp, tcpdata(0) As Byte
        Dim ok(0) As Byte
        Dim intIndex, intEnd As Integer
        Dim Finish As Double
        ack_ok = True

        Finish = VB.DateAndTime.Timer + 3.0   ' wait for ACK, max 3-seconds

        Do While (ack = False)
            If VB.DateAndTime.Timer > Finish Then
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFXMitter", "No ACK received witin 3 seconds !")
                ack_ok = False
                Exit Do
            End If

            If tcp = True Then
                ' As long as there is information, read one byte at a time and output it.
                While stream.DataAvailable
                    stream.Read(tcpdata, 0, 1)
                    temp = tcpdata(0)
                    If temp = protocolsynchro Then
                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFXMitter", "ACK  => " & VB.Right("0" & Hex(temp), 2))
                    ElseIf temp = &H5A Then
                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFXMitter", "NAK  => " & VB.Right("0" & Hex(temp), 2))
                    End If
                    mess = True
                End While
            Else
                Try
                    ' As long as there is information, read one byte at a time and 
                    '   output it.
                    While (port.BytesToRead() > 0)
                        ' Write the output to the screen.
                        temp = port.ReadByte()
                        If temp = protocolsynchro Then
                            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFXMitter", "ACK  => " & VB.Right("0" & Hex(temp), 2))
                        ElseIf temp = &H5A Then
                            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFXMitter", "NAK  => " & VB.Right("0" & Hex(temp), 2))
                        End If
                        mess = True
                    End While
                Catch exc As Exception
                    ' An exception is raised when there is no information to read : Don't do anything here, just let the exception go.
                End Try
            End If

            If mess Then
                ack = True
                mess = False
            End If
        Loop

        ack = False

        ' Write a user specified Command to the Port.
        Try
            ecrire(kar)
        Catch exc As Exception
            ' Warn the user.
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFXMitter", "Unable to write to port")
            ack_ok = False
        Finally

        End Try

        message = VB.Right("0" & Hex(kar(0)), 2)
        intEnd = ((kar(0) And &HF8) / 8)
        If (kar(0) And &H7) <> 0 Then
            intEnd += 1
        End If
        For intIndex = 1 To intEnd
            message = message + VB.Right("0" & Hex(kar(intIndex)), 2)
        Next
        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFXMitter", message)
        ack = False
    End Sub

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
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter Datareceived", Ex.Message)
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
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter ERRORDatareceived", Ex.Message)
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
            WriteLog("ERR: RFXMitter TCPDatareceived : " & Ex.Message)
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
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter ProcessNewTCPData", ex.Message)
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
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter", "Buffer flushed due to timeout")
                End If
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter tmrRead_Elapsed", ex.Message)
        End Try
    End Sub

    ''' <summary>Traite chaque byte reçu</summary>
    ''' <param name="temp">Byte recu</param>
    ''' <remarks></remarks>
    Private Sub ProcessReceivedChar(ByVal temp As Byte)
        Try
            If temp = Protocol Then
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFXMitter ProcessReceivedChar", "ACK => " & VB.Right("0" & Hex(temp), 2))
            End If
            mess = True
            ack = True
            mess = False
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter ProcessReceivedChar", ex.Message)
        End Try
    End Sub

    ''' <summary>Decode le message pour l'envoyer aux bonnes fonctions de traitement</summary>
    ''' <remarks></remarks>
    Private Sub display_mess()
        Try
            If Not _IsConnect Then Exit Sub 'si on ferme le port on quitte cette boucle

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter display_mess", ex.ToString)
        End Try
    End Sub
#End Region

#Region "Fonctions ecriture protocoles"

    'Home Easy : Chacon
    'adresse du type 00-00-00-00-0 ou 0 (pour les Heaters)
    'commande ON, OFF, DIM, GROUP_ON, GROUP_OFF, GROUP_DIM, HEATER_ON, HEATER_OFF
    Private Sub protocol_chacon(ByVal adresse As String, ByVal commande As String, ByVal europe As Boolean, ByVal dimlevel As Integer)
        Try
            Dim kar(5) As Byte
            Dim adressetab As String() = adresse.Split("-")
            If europe Then kar(0) = 34 Else kar(0) = 33
            kar(1) = CByte(adressetab(0))
            kar(2) = CByte(adressetab(1))
            kar(3) = CByte(adressetab(2))
            kar(4) = CByte(adressetab(3))
            Select Case adressetab(3)
                Case 0
                    kar(4) = 0
                Case 1
                    kar(4) = &H40
                Case 2
                    kar(4) = &H80
                Case 3
                    kar(4) = &HC0
            End Select
            kar(4) = kar(4) Or CByte(adressetab(4))

            Select Case commande
                Case "ON"
                    kar(4) = kar(4) Or &H10
                    kar(5) = 0
                Case "OFF"
                    kar(5) = 0
                Case "GROUP_ON"
                    kar(4) = kar(4) Or &H30
                    kar(5) = 0
                Case "GROUP_OFF"
                    kar(4) = kar(4) Or &H20
                    kar(5) = 0
                Case "DIM"
                    kar(5) = CByte(dimlevel) << 4
                Case "GROUP_DIM"
                    kar(4) = kar(4) Or &H20
                    kar(5) = CByte(dimlevel) << 4
                Case "HEATER_ON"
                    Dim kar2(2) As Byte
                    kar2(0) = 12
                    kar2(1) = CByte(adresse) << 3
                    kar2(1) = kar(1) Or &H4
                    kar2(2) = &HD0
                    kar = kar2
                Case "HEATER_OFF"
                    Dim kar2(2) As Byte
                    kar2(0) = 12
                    kar2(1) = CByte(adresse) << 3
                    kar2(1) = kar(1) Or &H4
                    kar2(2) = &HB0
                    kar = kar2
            End Select
            ecrirecommande(kar)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter ECRIRE CHACON", ex.Message)
        End Try
    End Sub

    'X10
    'adresse du type A1 
    'commande ON, OFF, BRIGHT, DIM, ALL_LIGHT_ON, ALL_LIGHT_OFF
    Private Sub protocol_x10(ByVal adresse As String, ByVal commande As String, ByVal europe As Boolean, ByVal dimlevel As Integer)
        Try
            Dim kar(4) As Byte
            Dim temp As Byte

            'getunit from adresse
            Select Case adresse.Substring(0, 1)
                Case "A" : temp = &H60
                Case "B" : temp = &H70
                Case "C" : temp = &H40
                Case "D" : temp = &H50
                Case "E" : temp = &H80
                Case "F" : temp = &H90
                Case "G" : temp = &HA0
                Case "H" : temp = &HB0
                Case "I" : temp = &HE0
                Case "J" : temp = &HF0
                Case "K" : temp = &HC0
                Case "L" : temp = &HD0
                Case "M" : temp = &H0
                Case "N" : temp = &H10
                Case "O" : temp = &H20
                Case "P" : temp = &H30
                Case Else : temp = &H60 'unexpected character force A
            End Select
            currentunit = temp
            If Int(adresse.Substring(1, adresse.Length - 1)) > 8 Then currentunit = currentunit Or &H4

            'get Device from adresse
            Dim dev As Integer
            dev = Int(adresse.Substring(1, adresse.Length - 1))
            If dev > 8 Then dev = dev - 8
            Select Case dev
                Case 1 : temp = 0
                Case 2 : temp = &H10
                Case 3 : temp = &H8
                Case 4 : temp = &H18
                Case 5 : temp = &H40
                Case 6 : temp = &H50
                Case 7 : temp = &H48
                Case 8 : temp = &H58
            End Select
            currentdevice = temp

            Select Case commande
                Case "ON"
                    kar(1) = currentunit
                    kar(3) = currentdevice
                Case "OFF"
                    kar(1) = currentunit
                    kar(3) = currentdevice Or &H20
                Case "BRIGHT"
                    kar(1) = currentunit And &HF0
                    kar(3) = &H88
                Case "DIM"
                    kar(1) = currentunit And &HF0
                    kar(3) = &H98
                Case "ALL_LIGHT_ON"
                    kar(1) = currentunit And &HF0
                    kar(3) = &H90
                Case "ALL_LIGHT_OFF"
                    kar(1) = currentunit And &HF0
                    kar(3) = &H80
            End Select
            kar(0) = &H20
            kar(2) = &HFF - kar(1)
            kar(4) = &HFF - kar(3)

            ecrirecommande(kar)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter ECRIRE X10", ex.Message)
        End Try
    End Sub

    'ARC
    'adresse du type A1 
    'commande ON, OFF, GROUP_ON, GROUP_OFF, CHIME
    Private Sub protocol_arc(ByVal adresse As String, ByVal commande As String, ByVal europe As Boolean)
        Try
            Dim kar(3) As Byte
            Dim ch As Integer

            kar(0) = 24
            Select Case commande
                Case "ON"
                    kar(1) = &H54
                    ch = Int(adresse.Substring(1, adresse.Length - 1)) - 1
                    kar(2) = ((ch And &H8) << 3) Or ((ch And &H4) << 2) Or ((ch And &H2) << 1) Or (ch And &H1)
                    ch = Asc(adresse.Substring(0, 1)) - &H41
                    kar(3) = ((ch And &H8) << 3) Or ((ch And &H4) << 2) Or ((ch And &H2) << 1) Or (ch And &H1)
                    ecrirecommande(kar)
                    kar(1) = &H55
                    ch = Int(adresse.Substring(1, adresse.Length - 1)) - 1
                    kar(2) = &H80 Or ((ch And &H8) << 3) Or ((ch And &H4) << 2) Or &H7
                    ch = Asc(adresse.Substring(0, 1)) - &H41
                    kar(3) = &H80 Or (((ch And &H8) << 3) Or ((ch And &H4) << 2) Or ((ch And &H2) << 1) Or (ch And &H1))
                    ecrirecommande(kar)
                Case "OFF"
                    kar(1) = &H14
                    ch = Int(adresse.Substring(1, adresse.Length - 1)) - 1
                    kar(2) = ((ch And &H8) << 3) Or ((ch And &H4) << 2) Or ((ch And &H2) << 1) Or (ch And &H1)
                    ch = Asc(adresse.Substring(0, 1)) - &H41
                    kar(3) = ((ch And &H8) << 3) Or ((ch And &H4) << 2) Or ((ch And &H2) << 1) Or (ch And &H1)
                    ecrirecommande(kar)
                    kar(1) = &H55
                    ch = Int(adresse.Substring(1, adresse.Length - 1)) - 1
                    kar(2) = &H80 Or ((ch And &H8) << 3) Or ((ch And &H4) << 2) Or &H7
                    ch = Asc(adresse.Substring(0, 1)) - &H41
                    kar(3) = &H80 Or (((ch And &H8) << 3) Or ((ch And &H4) << 2) Or ((ch And &H2) << 1) Or (ch And &H1))
                    ecrirecommande(kar)
                Case "GROUP_ON"
                    kar(1) = &H54
                    kar(2) = &HFF
                    ch = Asc(adresse.Substring(0, 1)) - &H41
                    kar(3) = ((ch And &H8) << 3) Or ((ch And &H4) << 2) Or ((ch And &H2) << 1) Or (ch And &H1)
                    ecrirecommande(kar)
                    kar(1) = &H55
                    kar(2) = &HFF
                    ch = Asc(adresse.Substring(0, 1)) - &H41
                    kar(3) = &H80 Or (((ch And &H8) << 3) Or ((ch And &H4) << 2) Or ((ch And &H2) << 1) Or (ch And &H1))
                    ecrirecommande(kar)
                Case "GROUP_OFF"
                    kar(1) = &H14
                    kar(2) = &HFF
                    ch = Asc(adresse.Substring(0, 1)) - &H41
                    kar(3) = ((ch And &H8) << 3) Or ((ch And &H4) << 2) Or ((ch And &H2) << 1) Or (ch And &H1)
                    ecrirecommande(kar)
                    kar(1) = &H55
                    kar(2) = &HFF
                    ch = Asc(adresse.Substring(0, 1)) - &H41
                    kar(3) = &H80 Or (((ch And &H8) << 3) Or ((ch And &H4) << 2) Or ((ch And &H2) << 1) Or (ch And &H1))
                    ecrirecommande(kar)
                Case "CHIME"
                    kar(1) = &H55
                    kar(2) = &H15
                    ch = Asc(adresse.Substring(0, 1)) - &H41
                    kar(3) = ((ch And &H8) << 3) Or ((ch And &H4) << 2) Or ((ch And &H2) << 1) Or (ch And &H1)
                    ecrirecommande(kar)
                    ecrirecommande(kar)
            End Select
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter ECRIRE ARC", ex.Message)
        End Try
    End Sub

    'Waveman
    'adresse du type A1 
    'commande ON, OFF
    Private Sub protocol_waveman(ByVal adresse As String, ByVal commande As String, ByVal europe As Boolean)
        Try
            Dim kar(3) As Byte
            Dim xlate As Byte() = {&H0, &H1, &H4, &H5, &H10, &H11, &H14, &H15, &H40, &H41, &H44, &H45, &H50, &H51, &H54, &H55}
            kar(0) = 24

            Select Case commande
                Case "ON" : kar(1) = &H54
                Case "OFF" : kar(1) = &H0
            End Select
            kar(2) = xlate(Int(adresse.Substring(1, adresse.Length - 1)) - 1)
            kar(3) = xlate(Asc(adresse.Substring(0, 1)) - &H41)
            ecrirecommande(kar)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter ECRIRE WAVEMAN", ex.Message)
        End Try
    End Sub


#End Region


#Region "Write"

    Public Sub WriteLog(ByVal message As String)
        Try
            'utilise la fonction de base pour loguer un event
            If STRGS.InStr(message, "DBG:") > 0 Then
                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "RFXMitter ", STRGS.Right(message, message.Length - 4))
            ElseIf STRGS.InStr(message, "ERR:") > 0 Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter ", STRGS.Right(message, message.Length - 4))
            Else
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFXMitter ", STRGS.Right(message, message.Length - 4))
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter WriteLog", ex.Message)
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
                listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(adresse, type, Me._ID)
                If (listedevices.Count = 1) Then
                    'un device trouvé on maj la value
                    listedevices.Item(0).Value = valeur
                ElseIf (listedevices.Count > 1) Then
                    WriteLog("ERR: Plusieurs devices correspondent à : " & type & " " & adresse & ":" & valeur)
                Else
                    WriteLog("ERR: Device non trouvé : " & type & " " & adresse & ":" & valeur)


                    'Ajouter la gestion des composants bannis (si dans la liste des composant bannis alors on log en debug sinon onlog device non trouve empty)


                End If
            End If

        Catch ex As Exception
            WriteLog("ERR: Writeretour Exception : " & ex.Message)
        End Try
    End Sub

#End Region

End Class
