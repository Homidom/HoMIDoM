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
    Dim _Port_TCP As String = ""
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

#End Region

#Region "Variables Internes"
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
                    'les handlers sont lancés, on configure le rfxcom
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

    End Sub

    ''' <summary>Commander un device</summary>
    ''' <param name="Objet">Objet représetant le device à interroger</param>
    ''' <param name="Command">La commande à passer</param>
    ''' <param name="Parametre1"></param>
    ''' <param name="Parametre2"></param>
    ''' <remarks></remarks>
    Public Sub Write(ByVal Objet As Object, ByVal Command As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
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
                    client = New TcpClient(numero, 10002)
                    _IsConnect = True
                    Return ("Port IP " & port_name & " ouvert")
                Else
                    'RFXCOM est un modele usb
                    tcp = False
                    port.PortName = port_name 'nom du port : COM1
                    port.BaudRate = 38400 'vitesse du port 300, 600, 1200, 2400, 9600, 14400, 19200, 38400, 57600, 115200
                    port.Parity = IO.Ports.Parity.None 'pas de parité
                    port.StopBits = IO.Ports.StopBits.One 'un bit d'arrêt par octet
                    port.DataBits = 8 'nombre de bit par octet
                    port.Encoding = System.Text.Encoding.GetEncoding(1252)  'Extended ASCII (8-bits)
                    port.StopBits = StopBits.One
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

    ''' <summary>Configurer le RFXCOM</summary>
    ''' <remarks></remarks>
    Private Function configurer() As String
        'configurer le rfxcom
        Try

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
    ''' <param name="commande2">deuxieme paquet à envoyer</param>
    ''' <remarks></remarks>
    Public Function ecrire(ByVal commande As Byte, ByVal commande2 As Byte) As String
        Dim cmd() As Byte = {commande, commande2}
        Dim message As String = ""
        Try
            Return ""
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
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter ProcessReceivedChar", ex.Message)
        End Try
    End Sub

    ''' <summary>Decode le message pour l'envoyer aux bonnes fonctions de traitement</summary>
    ''' <remarks></remarks>
    Public Sub display_mess()
        Try
            If Not _IsConnect Then Exit Sub 'si on ferme le port on quitte cette boucle

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter display_mess", ex.ToString)
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
