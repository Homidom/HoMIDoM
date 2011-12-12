Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports System.IO.Ports
Imports System.Math
Imports System.Net.Sockets
Imports System.Threading
Imports System.Globalization

Public Class Driver_x10
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "9BC60A04-3569-11E0-B9A7-3F66DFD72085"
    Dim _Nom As String = "X10"
    Dim _Enable As String = False
    Dim _Description As String = "X10 CM11"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "COM"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = ""
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "X10"
    Dim _Version As String = "1.0"
    Dim _Picture As String = ""
    Dim _Server As HoMIDom.HoMIDom.Server
    Dim _Device As HoMIDom.HoMIDom.Device
    Dim _DeviceSupport As New ArrayList
    Dim _Parametres As New ArrayList
    Dim MyTimer As New Timers.Timer
    Dim _IdSrv As String
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande)
#End Region

#Region "Variables Internes"
    Public WithEvents port As New System.IO.Ports.SerialPort
    Private port_ouvert As Boolean = False
    Private port_name As String = ""
    Private com_to_hex As New Dictionary(Of String, Byte)
    Private house_to_hex As New Dictionary(Of String, Byte)
    Private device_to_hex As New Dictionary(Of String, Byte)
    Private GetPortInput As Boolean
    Private OutPortDevice As Boolean = False

#End Region

#Region "Déclaration"
    ' CM11 Handshaking codes
    Public Const SET_TIME As Byte = &H9B
    Public Const INFERFACE_READY As Byte = &H55
    Public Const COMPUTER_READY As Byte = &HC3
    Public Const ACK As Byte = &H0

    ' CM11 Hail codes
    Public Const INTERFACE_CQ As Byte = &H5A
    Public Const CM11_CLOCK_REQ As Byte = &HA5
    Public Const CP10_CLOCK_REQ As Byte = &HA6
    Public Const ACK_DEF_CM11 As Byte = &HFB

    'CM11 Command Header Definitions
    Public Const STANDARD_ADDRESS As Byte = &H4
    Public Const STANDARD_FUNCTION As Byte = &H6
    Public Const ENHANCED_ADDRESS As Byte = &H5
    Public Const ENHANCED_FUNCTION As Byte = &H7

    Private BufferIn(8192) As Byte
    Dim CurrentHouse As String = ""
    Dim CurrentCode As String = ""
    Dim IntToCmd() As String = {"All Units Off", "All Lights On", "On", "Off", "Dim", "Bright", "All Lights Off", "Extended Code", "Hail Request", "Hail Acknowledge", "Preset Dim 1", "Preset Dim 2", "Extended Data Transfer", "Status On", "Status Off", "Status Request"}
#End Region

#Region "Propriétés génériques"
    Public WriteOnly Property IdSrv As String Implements HoMIDom.HoMIDom.IDriver.IdSrv
        Set(ByVal value As String)
            _IdSrv = value
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
    Public Property Refresh() As Integer Implements HoMIDom.HoMIDom.IDriver.Refresh
        Get
            Return _Refresh
        End Get
        Set(ByVal value As Integer)
            _Refresh = value
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
    ''' <summary>
    ''' Retourne la liste des Commandes avancées
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCommandPlus() As List(Of DeviceCommande)
        Return _DeviceCommandPlus
    End Function

    ''' <summary>
    ''' Execute une commande avancée
    ''' </summary>
    ''' <param name="Command"></param>
    ''' <param name="Param"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ExecuteCommand(ByVal Command As String, Optional ByVal Param() As Object = Nothing) As Boolean
        Dim retour As Boolean = False

        If Command = "" Then
            Return False
            Exit Function
        End If

        Select Case UCase(Command)
            Case ""
            Case Else
        End Select

        Return retour
    End Function


    ''' <summary>Démarrer le du driver</summary>
    ''' <remarks></remarks>
    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        Try
            If Not _IsConnect Then
                port.PortName = _Com  'nom du port : COM1
                port.BaudRate = 4800 'vitesse du port 300, 600, 1200, 2400, 9600, 14400, 19200, 38400, 57600, 115200
                port.Parity = IO.Ports.Parity.None 'pas de parité
                port.StopBits = IO.Ports.StopBits.One 'un bit d'arrêt par octet
                port.DataBits = 8 'nombre de bit par octet
                'port.Encoding = System.Text.Encoding.GetEncoding(1252)  'Extended ASCII (8-bits)
                'port.ReadBufferSize = CInt(4096)
                'port.ReceivedBytesThreshold = 1
                port.StopBits = StopBits.One
                port.Handshake = IO.Ports.Handshake.XOnXOff
                port.ReadTimeout = 5000
                port.WriteTimeout = 5000
                AddHandler port.DataReceived, New SerialDataReceivedEventHandler(AddressOf DataReceived)
                AddHandler port.ErrorReceived, New SerialErrorReceivedEventHandler(AddressOf m_serialPort_ErrorReceived)
                port.Open()
                'If port.IsOpen Then
                '    port.DtrEnable = True
                '    port.RtsEnable = True
                '    port.DiscardInBuffer()
                'End If
                _IsConnect = True
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "X10 Start", "Port " & _Com & " ouvert")
            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 Start", "Port " & _Com & " déjà ouvert")
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 Start", ex.ToString)
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            If _IsConnect Then
                _IsConnect = False
                'suppression de l'attente de données à lire
                RemoveHandler port.DataReceived, AddressOf DataReceived
                'fermeture des ports
                If (Not (port Is Nothing)) Then ' The COM port exists.
                    If port.IsOpen Then
                        Dim limite As Integer = 0
                        Do While (port.BytesToWrite > 0 And limite < 100) ' Wait for the transmit buffer to empty.
                            limite = limite + 1
                        Loop
                        limite = 0
                        Do While (port.BytesToRead > 0 And limite < 100) ' Wait for the receipt buffer to empty.
                            limite = limite + 1
                        Loop
                        port.Close()
                        port.Dispose()
                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "X10 Stop", "Port " & _Com & " fermé")
                    End If
                    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "X10 Stop", "Port " & _Com & " est déjà fermé")
                End If
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 Stop", "Port " & _Com & " n'existe pas")
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 Stop", ex.Message)
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
    ''' <remarks>pas utilisé</remarks>
    Public Sub Read(ByVal Objet As Object) Implements HoMIDom.HoMIDom.IDriver.Read
        Try
            If _Enable = False Then Exit Sub
            If _IsConnect = False Then Exit Sub

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 Read", ex.Message)
        End Try
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
            If _IsConnect = False Then Exit Sub
            Select Case UCase(Command)
                Case "ON"
                    ecrire(Objet.adresse1, "ON", 0)
                Case "OFF"
                    ecrire(Objet.adresse1, "OFF", 0)
                Case "DIM"
                    If Parametre1 IsNot Nothing Then
                        Dim x As Integer = Parametre1
                        If x <= 0 Then
                            ecrire(Objet.adresse1, "OFF", 0)
                        End If
                        If x >= 100 Then
                            ecrire(Objet.adresse1, "ON", 0)
                        End If
                        ecrire(Objet.adresse1, "EXTENDED_CODE", x)
                    End If
                Case "OUVERTURE"
                    If Parametre1 IsNot Nothing Then
                        Dim x As Integer = Parametre1
                        If x <= 0 Then
                            ecrire(Objet.adresse1, "OFF", 0)
                        End If
                        If x >= 100 Then
                            ecrire(Objet.adresse1, "ON", 0)
                        End If
                        ecrire(Objet.adresse1, "EXTENDED_CODE", x)
                    End If
            End Select
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 Write", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 DeleteDevice", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 NewDevice", ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick()

    End Sub

#End Region

    Public Sub New()
        Try
            house_to_hex.Add("A", &H60)
            house_to_hex.Add("B", &HE0)
            house_to_hex.Add("C", &H20)
            house_to_hex.Add("D", &HA0)
            house_to_hex.Add("E", &H10)
            house_to_hex.Add("F", &H90)
            house_to_hex.Add("G", &H50)
            house_to_hex.Add("H", &HD0)
            house_to_hex.Add("I", &H70)
            house_to_hex.Add("J", &HF0)
            house_to_hex.Add("K", &H30)
            house_to_hex.Add("L", &HB0)
            house_to_hex.Add("M", &H0)
            house_to_hex.Add("N", &H80)
            house_to_hex.Add("O", &H40)
            house_to_hex.Add("P", &HC0)

            device_to_hex.Add("1", &H6)
            device_to_hex.Add("2", &HE)
            device_to_hex.Add("3", &H2)
            device_to_hex.Add("4", &HA)
            device_to_hex.Add("5", &H1)
            device_to_hex.Add("6", &H9)
            device_to_hex.Add("7", &H5)
            device_to_hex.Add("8", &HD)
            device_to_hex.Add("9", &H7)
            device_to_hex.Add("10", &HF)
            device_to_hex.Add("11", &H3)
            device_to_hex.Add("12", &HB)
            device_to_hex.Add("13", &H0)
            device_to_hex.Add("14", &H8)
            device_to_hex.Add("15", &H4)
            device_to_hex.Add("16", &HC)

            com_to_hex.Add("ALL_UNITS_OFF", &H0)
            com_to_hex.Add("ALL_LIGHTS_ON", &H1)
            com_to_hex.Add("ON", &H2)
            com_to_hex.Add("OFF", &H3)
            com_to_hex.Add("DIM", &H4)
            com_to_hex.Add("BRIGHT", &H5)
            com_to_hex.Add("ALL_LIGHTS_OFF", &H6)
            com_to_hex.Add("EXTENDED_CODE", &H7)
            com_to_hex.Add("HAIL_REQ", &H8)
            com_to_hex.Add("HAIL_ACK", &H9)
            com_to_hex.Add("PRESET_DIM_1", &HA)
            com_to_hex.Add("PRESET_DIM_2", &HB)
            com_to_hex.Add("EXTENDED_DATA_TRANSFER", &HC)
            com_to_hex.Add("STATUS_ON", &HD)
            com_to_hex.Add("STATUS_OFF", &HE)
            com_to_hex.Add("STATUS_REQUEST", &HF)

            'liste des devices compatibles
            _DeviceSupport.Add(ListeDevices.APPAREIL.ToString)
            _DeviceSupport.Add(ListeDevices.CONTACT.ToString)
            _DeviceSupport.Add(ListeDevices.DETECTEUR.ToString)
            _DeviceSupport.Add(ListeDevices.LAMPE.ToString)
            _DeviceSupport.Add(ListeDevices.SWITCH.ToString)
            _DeviceSupport.Add(ListeDevices.VOLET.ToString)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 New", ex.Message)
        End Try
    End Sub

    Private Sub m_serialPort_ErrorReceived(ByVal sender As Object, ByVal e As SerialErrorReceivedEventArgs)
        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 ErrorReceived", "Error: " & e.ToString)
    End Sub

    ''' <summary>
    ''' Traite les infos reçus
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DataReceived(ByVal sender As Object, ByVal e As SerialDataReceivedEventArgs)
        Try
            'Nombre d'octet à lire
            'Dim count As Integer = port.BytesToRead

            If _IsConnect Then
                'port.Read(BufferIn, 0, 1)

                Select Case port.ReadByte
                    Case INTERFACE_CQ
                        'suppression de l'attente de données à lire
                        RemoveHandler port.DataReceived, AddressOf DataReceived

                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "X10 DataReceived", "Un Device a envoyé un ordre")

                        '' Attend le reste des données
                        Dim Time_Out As Integer = 0
                        Dim Inbyte As Integer = 0

                        Do While Time_Out <= 20
                            'L'interface demande au pc de lui envoyer des données et on doit répondre 
                            Dim donnee As Byte() = {&HC3}
                            port.Write(donnee, 0, 1)

                            '_Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "X10 DataReceived", "Le serveur a repondu OK")
                            System.Threading.Thread.Sleep(200)

                            'Si on a reçu un byte différent que la demande de notification on sort de la boucle pour traiter le reste
                            Inbyte = port.ReadByte
                            If Inbyte <> INTERFACE_CQ Then Exit Do

                            Time_Out += 1
                        Loop

                        ''A t-on reçu des données?
                        If Time_Out > 20 Then
                            'Temps d'attente dépassé
                            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 DataReceived", "Temps d'attente dépassé")
                            Exit Sub
                        End If

                        If Inbyte < 2 And Inbyte > 9 Then
                            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "X10 TraiteLire", "Trop ou pas assez de Bytes reçu: " & Inbyte)
                            Exit Sub
                        End If

                        '_Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "X10 DataReceived", Inbyte & " bytes à traiter")

                        'On attend de recevoir le reste
                        Time_Out = 0
                        Do While Time_Out <= 20 And port.BytesToRead < Inbyte
                            System.Threading.Thread.Sleep(100)
                            Time_Out += 1
                        Loop

                        ''A t-on reçu des données restantes?
                        If Time_Out > 20 Then
                            'Temps d'attente dépassé
                            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 DataReceived", "Temps d'attente (reste) dépassé")
                            Exit Sub
                        End If

                        Dim trame(Inbyte - 1) As Byte
                        port.Read(trame, 0, Inbyte)

                        'Dim tramerecue As String = ""
                        'For j As Integer = 0 To trame.Length - 1
                        'tramerecue &= CInt(trame(j)).ToString & " "
                        'Next
                        '_Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "X10 DataReceived", "Trame recue: " & tramerecue)

                        TraiteLire(trame)
                        AddHandler port.DataReceived, New SerialDataReceivedEventHandler(AddressOf DataReceived)
                    Case CM11_CLOCK_REQ
                        ' Power failure macro refresh request (Chr165 = 0xA5) Erreur CM11
                        ' Power fail/recovery detected.
                        port.Write(ACK_DEF_CM11) '0xfb
                    Case CP10_CLOCK_REQ

                    Case Else
                End Select
            End If
        Catch Ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 Datareceived", "Erreur:" & Ex.ToString)
        End Try
    End Sub


    ''' <summary>
    ''' Traite la trame reçu provenant d'un device
    ''' </summary>
    ''' <param name="Data"></param>
    ''' <remarks></remarks>
    Private Sub TraiteLire(ByVal Data() As Byte)
        Try

            Dim Recieved_Function As String = ""
            Dim Recieved_FAMask As String = ""
            Dim Recieved_Variation As Double

            'Wake-up and data recieved
            Dim trame() As Byte = Data

            'BufSize = CInt(trame(0)) 'récupère la taille de la trame qui ne peu faire que 10 octets maxi

            'Byte   Function
            '0      Upload Buffer Size --> déjà traité avant
            '1      Function / Address Mask
            '2      Data Byte #0
            '3      Data Byte #1
            '4      Data Byte #2
            '5      Data Byte #3
            '6      Data Byte #4
            '7      Data Byte #5
            '8      Data Byte #6
            '9      Data Byte #7

            'If BufSize > 2 And BufSize <= 10 Then 'Vérifie qu'il ne doit y avoir que 10 octet maximum qui doivent être envoyé sinon message d'erreur

            ' Le mask représente les octets 2 à 9 (bit0 pour octet2, bit1 pour octet3..,bit 8 pour octet9)
            ' Si le bit est à 0 cela veut dire que l'octet correspondant est une Adresse et si le bit est à 1 c'est une fonction
            Recieved_FAMask = Int2Bin(CInt(trame(0)))
            Recieved_FAMask = StrReverse(Recieved_FAMask)

            For i = 1 To trame.Length - 1
                Dim Bin As String = Int2Bin(CInt(trame(i)))
                CurrentHouse = GetHouse(Mid(Bin, 1, 4))

                Dim Mask As String = Mid(Recieved_FAMask, i, 1)
                Select Case Mask
                    Case "0" 'Le Mask est à 0 donc c'est une adresse
                        CurrentCode = GetDevice(Mid(Bin, 5, 4))
                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "X10", "Adresse courante:" & CurrentHouse & CurrentCode)
                    Case "1" 'Le Mask est à 1 donc c'est une fonction
                        Recieved_Function = GetFunction(Mid(Bin, 5, 4))
                        If Recieved_Function = "5" Or Recieved_Function = "6" Then
                            'C'est une fonction Dim ou Bright donc octet suivant c'est la valeur de variation
                            Recieved_Variation = CInt(trame(i + 1)) / 210
                            i += 1
                        End If
                        If CurrentCode <> "" And CurrentHouse <> "" Then
                            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "X10 TraiteLire", CurrentHouse & CurrentCode & ":" & IntToCmd(Recieved_Function))
                            Dim _add As String = CurrentHouse & CurrentCode
                            Select Case Recieved_Function
                                Case "1"
                                    traitement("ALLLIGHTSON", _add)
                                Case "3"
                                    traitement("ON", _add)
                                Case "4"
                                    traitement("OFF", _add)
                            End Select
                        End If
                    Case Else 'C'est une erreur car ni adresse ni fonction
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 TraiteLire", "Erreur inconnu - Mask=" & Mask)
                End Select
            Next
            'Else
            'ERREUR TROP DE BYTES A RECEVOIR MAX 10
            '_Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 TraiteLire", "Trop ou pas assez de Bytes reçu: " & BufSize)
            'End If
        Catch Ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 TraiteLire", "Erreur: " & Ex.ToString)
        End Try
    End Sub

    ''' <summary>Traite les paquets reçus</summary>
    ''' <remarks></remarks>
    Private Sub traitement(ByVal valeur As String, ByVal adresse As String)
        If valeur <> "" Then
            Try
                'Recherche si un device affecté
                Dim listedevices As New ArrayList

                listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, adresse, "", Me._ID, True)
                'un device trouvé on maj la value
                If (listedevices.Count = 1) Then
                    'correction valeur pour correspondre au type de value
                    If TypeOf listedevices.Item(0).Value Is Integer Then
                        If valeur = "ON" Then
                            valeur = 100
                        ElseIf valeur = "OFF" Then
                            valeur = 0
                        End If
                    ElseIf TypeOf listedevices.Item(0).Value Is Boolean Then
                        If valeur = "ON" Then
                            valeur = True
                        ElseIf valeur = "OFF" Then
                            valeur = False
                        Else
                            valeur = True
                        End If
                    End If

                    listedevices.Item(0).Value = valeur

                ElseIf (listedevices.Count > 1) Then
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 Process", "Plusieurs devices correspondent à : " & adresse & ":" & valeur)
                Else
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 Process", "Device non trouvé : " & adresse & ":" & valeur)

                End If
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 traitement", "Exception : " & ex.Message & " --> " & adresse & " : " & valeur)
            End Try
        End If
    End Sub

    Public Sub ecrire(ByVal adresse As String, ByVal commande As String, ByVal data As Integer)
        'adresse= adresse du composant : A1
        'commande : ON, OFF...
        'data

        Dim axbData(5) As Byte
        Dim ReadaxbData(1) As Byte
        Dim xbCheckSum As Byte
        Dim nbboucle As Integer

        If _IsConnect Then
            If Not OutPortDevice Then
                OutPortDevice = True

                'suppression de l'attente de données à lire
                RemoveHandler port.DataReceived, AddressOf DataReceived

                Try
                    'composition des messages à envoyer
                    axbData(0) = STANDARD_ADDRESS
                    axbData(1) = house_to_hex(Microsoft.VisualBasic.Left(adresse, 1)) Or device_to_hex(Microsoft.VisualBasic.Right(adresse, adresse.Length - 1))
                    axbData(2) = ACK
                    axbData(3) = ((data * 8) And 255) Or STANDARD_FUNCTION
                    axbData(4) = house_to_hex(Microsoft.VisualBasic.Left(adresse, 1)) Or com_to_hex(commande)
                Catch ex As Exception
                    OutPortDevice = False
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 ecrire", "ERR: X10: messages non valides : " & adresse & "-" & commande & " --> " & ex.ToString)
                End Try

                Try

                    Dim donnee As Byte() = {axbData(0), axbData(1)}
                    nbboucle = 0
                    Do
                        'ecriture de H4 et housecode-devicecode
                        port.Write(donnee, 0, 2)
                        System.Threading.Thread.Sleep(50)

                        'lecture du checksum renvoyé par le module
                        ReadaxbData(0) = port.ReadByte()
                        xbCheckSum = (axbData(0) + axbData(1)) And &HFF
                        nbboucle += 1
                    Loop Until ReadaxbData(0) = xbCheckSum Or nbboucle >= 4

                    'le chesksum n'a jamais été bon
                    If nbboucle >= 4 Then
                        OutPortDevice = False
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 ecrire", "ERR: X10 : cheksum non valide")
                    End If

                    'on envoie le ack
                    Dim donnee2 As Byte() = {axbData(2)}
                    port.Write(donnee2, 0, 1)

                    'on lit la reponse Interface ready
                    'System.Threading.Thread.Sleep(500)
                    'ReadaxbData(0) = port.ReadByte()
                    'If (ReadaxbData(0) <> INFERFACE_READY) Then
                    '    OutPortDevice = False
                    '    Return ("ERR: X10: INTERFACE NOT READY")
                    'End If

                    Dim donnee3 As Byte() = {axbData(3), axbData(4)}
                    nbboucle = 0
                    Do
                        'ecriture de la H6 + valeur du DIM et housecode-commandecode
                        port.Write(donnee3, 0, 2)
                        System.Threading.Thread.Sleep(50)

                        'lecture du checksum renvoyé par le module
                        ReadaxbData(0) = port.ReadByte()
                        xbCheckSum = (axbData(3) + axbData(4)) And &HFF
                        nbboucle += 1
                    Loop Until ReadaxbData(0) = xbCheckSum Or nbboucle >= 4

                    'le chesksum n'a jamais été bon
                    If nbboucle >= 4 Then
                        OutPortDevice = False
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 ecrire", "ERR: X10 : cheksum non valide")
                    End If

                    'on envoie le ack
                    port.Write(donnee2, 0, 1)

                    'on lit la reponse Interface ready
                    'System.Threading.Thread.Sleep(500)
                    'ReadaxbData(0) = port.ReadByte()
                    'If (ReadaxbData(0) <> INFERFACE_READY) Then
                    '    OutPortDevice = False
                    '    Return ("ERR: X10: INTERFACE NOT READY")
                    'End If

                Catch ex As Exception
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 ecrire", "ERR: X10: " & ex.ToString)
                End Try
            Else
                OutPortDevice = False
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 ecrire", "ERR: X10: ecriture déjà en cours")
            End If

            AddHandler port.DataReceived, New SerialDataReceivedEventHandler(AddressOf DataReceived)

            OutPortDevice = False
            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "X10 ecrire", "OK Adresse:" & adresse & " Commande:" & commande & " Data:" & data)
        Else
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 ecrire", "ERR: X10: Port fermé")
        End If

    End Sub

    ''' <summary>
    ''' Retourne la date et l'heure, la version
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function X10_GetClock() As String

        Dim X10_Recieved_Trame As String
        Dim X10_Send_Command As String
        Dim Trame As String = "" 'Trame envoyé depuis le CM11 en binaire
        Dim j As Integer
        Dim tablS(14) As String
        Dim GClk_DimStatDev As String 'Dim status of the monitored devices
        Dim GClk_StatDev As String ''On/off status of the monitored devices
        Dim GClk_AdrDev As String ''currently addressed monitored devices
        Dim GClk_FirmwareRev As String 'Firmware revision level 0 to 15
        Dim GClk_MonitHC As String 'Monitored house code
        Dim GClk_Day As String 'Day Mask  (SMTWTFS, bit 57=Sunday)
        Dim GClk_Date As String 'Current year day
        Dim GClk_Hour As String ''Current time (hours/2, ranging from 0 to 11)
        Dim GClk_Minute As String 'Current time (minutes, ranging from 0 to 119)
        Dim GClk_Second As String 'Current time (seconds)
        Dim GClK_Battery As String 'Battery Timer (set to 0xffff on reset)
        Dim SupMinut As Boolean

        ' Prepare the Command and function
        X10_Send_Command = "10001011" '0x8b=> demander l'heure
        X10_Send_Command = Bin2Int(X10_Send_Command) 'Convert to an Integer
        X10_Send_Command = Chr(X10_Send_Command) 'Convert to ASCII binary value

        ' Send the command to the CM11
        port.Write(X10_Send_Command)

        ' Wait for the checksum byte to be returned
        Dim Time_Out As Integer = 0
        Do While Time_Out <= 2000 And port.BytesToRead = 0
            System.Threading.Thread.Sleep(100)
            Time_Out += 1
        Loop
        ' Do we have data?
        If Time_Out >= 2000 Then
            X10_GetClock = ("X10_GetClock,1: time out dépassé - Annulé")
            Exit Function
        End If

        If (port.BytesToRead * 8) <> 112 Then 'Vérifie que ce qui est reçu correspond bien au nombre de bits attendus
            X10_GetClock = ("X10_GetClock,2: Mauvaise réception - Aborted")
            Exit Function
        End If

        ' Reçoit la trame reçue
        X10_Recieved_Trame = port.ReadLine

        'Découpe et converti la trame reçu par octet
        For j = 1 To Len(X10_Recieved_Trame)
            tablS(j) = Format(Int2Bin(Asc(Mid(X10_Recieved_Trame, j, 1))), "00000000")
            If tablS(j) = "" Then
                tablS(j) = "00000000"
            End If
            Trame = Trame & tablS(j)
        Next j


        GClk_DimStatDev = Mid(Trame, 97, 16) 'Dim status of the monitored devices
        GClk_StatDev = Mid(Trame, 81, 16) 'On/off status of the monitored devices
        GClk_AdrDev = Mid(Trame, 65, 16) 'currently addressed monitored devices
        GClk_FirmwareRev = Mid(Trame, 61, 4)  'Firmware revision level 0 to 15
        GClk_MonitHC = Mid(Trame, 57, 4)  'Monitored house code
        GClk_Day = Mid(Trame, 50, 7)  'Day Mask  (SMTWTFS, bit 57=Sunday)
        GClk_Date = Mid(Trame, 41, 9)  'Current year day
        GClk_Hour = Mid(Trame, 33, 8)   'Current time (hours/2, ranging from 0 to 11)
        GClk_Minute = Mid(Trame, 25, 8)  'Current time (minutes, ranging from 0 to 119)
        GClk_Second = Mid(Trame, 17, 8) 'Current time (seconds)
        GClK_Battery = Mid(Trame, 1, 16) 'Battery Timer (set to 0xffff on reset)

        'Retourne la version firmware du CM11 *****************
        GClk_FirmwareRev = Bin2Int(GClk_FirmwareRev)
        If GClk_FirmwareRev <> 8 Then 'N° firmware différent recommencer si err com ou autre
            X10_GetClock = ("X10_GetClock,3: Numéro du firmware différent - Aborted")
            Exit Function
        End If

        'Retourne le jour *************************************
        GClk_Day = Format(GClk_Day, "0000000")
        GClk_Day = Bin2Day(GClk_Day)

        'Retourne les secondes ************************************
        GClk_Second = Format(Bin2Int(GClk_Second), "00")

        'retourne les minutes (sachant que minutes va de 0 à 120 donc si supérieur à 59 rajouter 1 à heure
        GClk_Minute = Bin2Int(GClk_Minute)
        If CInt(GClk_Minute) > 59 Then
            GClk_Minute = GClk_Minute - 60
            SupMinut = True
        Else
            SupMinut = False
        End If
        GClk_Minute = Format(GClk_Minute, "00")

        'retourne les heures (sachant heure est /2) ****************
        GClk_Hour = Bin2Int(GClk_Hour)
        GClk_Hour = GClk_Hour * 2
        If SupMinut = True Then GClk_Hour += 1

        'Date (Nombre jour de l'année (de 0 pour 1 janvier à 365 pour 31 décembre)*****************************************************
        If Bin2Int(GClk_Date) > 365 Then
            X10_GetClock = "Get Clock,4 - Il y a plus de 365 jours donc problème"
            Exit Function
        End If

        If Bin2dbl(GClk_Date) > 365 Then
            X10_GetClock = ""
            ' Exit Function
        Else
            GClk_Date = Jour2Date(Bin2Int(GClk_Date))

            'Battery timer ********************************************
            GClK_Battery = Bin2dbl(GClK_Battery)

            X10_GetClock = "Battery: " & GClK_Battery & " - House: " & GetHouse(GClk_MonitHC) & " Add: " & GetAdd(GClk_AdrDev) & " - firmware: " & GClk_FirmwareRev & " - " & GClk_Day & " " & GClk_Date & " " & GClk_Hour & ":" & GClk_Minute & ":" & GClk_Second

        End If
    End Function


#Region "Conversion"
    ''' <summary>
    ''' Renvoi la fonction depuis la valeur binaire
    ''' </summary>
    ''' <param name="Bin"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFunction(ByVal Bin As String) As String
        Select Case Bin
            Case "0000" : GetFunction = "1"     'All units off
            Case "0001" : GetFunction = "2"     'All lights on
            Case "0010" : GetFunction = "3"     'On
            Case "0011" : GetFunction = "4"     'Off
            Case "0100" : GetFunction = "5"     'Dim
            Case "0101" : GetFunction = "6"     'Bright
            Case "0110" : GetFunction = "7"     'All lights off
            Case "0111" : GetFunction = "8"     'Extended code
            Case "1000" : GetFunction = "9"     'Hail request
            Case "1001" : GetFunction = "10"    'Hail Ack
            Case "1010" : GetFunction = "11"    'Preset dim 1
            Case "1011" : GetFunction = "12"    'Preset dim 2
            Case "1100" : GetFunction = "13"    'Extended data transfer
            Case "1101" : GetFunction = "14"    'Status on
            Case "1110" : GetFunction = "15"    'Status off
            Case "1111" : GetFunction = "16"    'Status request
            Case Else : GetFunction = ""
        End Select
    End Function

    ''' <summary>
    ''' Renvoi le numéro d'adresse depuis la valeur Binaire
    ''' </summary>
    ''' <param name="Bin"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDevice(ByVal Bin As String) As String
        Select Case Bin
            Case "0110" : GetDevice = "1"
            Case "1110" : GetDevice = "2"
            Case "0010" : GetDevice = "3"
            Case "1010" : GetDevice = "4"
            Case "0001" : GetDevice = "5"
            Case "1001" : GetDevice = "6"
            Case "0101" : GetDevice = "7"
            Case "1101" : GetDevice = "8"
            Case "0111" : GetDevice = "9"
            Case "1111" : GetDevice = "10"
            Case "0011" : GetDevice = "11"
            Case "1011" : GetDevice = "12"
            Case "0000" : GetDevice = "13"
            Case "1000" : GetDevice = "14"
            Case "0100" : GetDevice = "15"
            Case "1100" : GetDevice = "16"
            Case Else : GetDevice = ""
        End Select
    End Function

    ''' <summary>
    ''' Renvoi le Code House depuis la valeur Binaire
    ''' </summary>
    ''' <param name="Bin"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetHouse(ByVal Bin As String) As String
        Select Case Bin
            Case "0110" : GetHouse = "A"
            Case "1110" : GetHouse = "B"
            Case "0010" : GetHouse = "C"
            Case "1010" : GetHouse = "D"
            Case "0001" : GetHouse = "E"
            Case "1001" : GetHouse = "F"
            Case "0101" : GetHouse = "G"
            Case "1101" : GetHouse = "H"
            Case "0111" : GetHouse = "I"
            Case "1111" : GetHouse = "J"
            Case "0011" : GetHouse = "K"
            Case "1011" : GetHouse = "L"
            Case "0000" : GetHouse = "M"
            Case "1000" : GetHouse = "N"
            Case "0100" : GetHouse = "O"
            Case "1100" : GetHouse = "P"
            Case Else : GetHouse = ""
        End Select
    End Function

    ''' <summary>
    ''' Conversion Entier en Binaire
    ''' </summary>
    ''' <param name="Dec"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Int2Bin(ByVal Dec As Integer) As String
        Dim tdec As Integer = Dec
        Dim Bin As String = ""

        Do While tdec > 0
            If tdec / 2 = tdec \ 2 Then
                Bin = "0" & Bin
            ElseIf tdec / 2 <> tdec \ 2 Then
                Bin = "1" & Bin
            End If
            tdec = tdec \ 2
        Loop

        Dim retour As New String("0", 8 - Len(Bin))
        retour &= Bin
        Return retour
    End Function

    ''' <summary>
    ''' Conversion Binaire en Entier
    ''' </summary>
    ''' <param name="Bin"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function Bin2Int(ByVal Bin As String) As Integer
        Dim ax As Integer
        Dim z As Integer
        For y As Integer = Len(Bin) To 1 Step -1
            If Mid(Bin, y, 1) = "1" Then ax = ax + (2 ^ z)
            z += 1
        Next
        Bin2Int = ax
    End Function

    ''' <summary>
    ''' Conversion Bin en jour
    ''' </summary>
    ''' <param name="Bin"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function Bin2Day(ByVal Bin As String) As String
        'à modifier car mapage incorrect voir le retour d'état de l'interface

        Select Case Bin
            Case "0000001" : Bin2Day = "Dimanche"
            Case "0000010" : Bin2Day = "Lundi"
            Case "0000100" : Bin2Day = "Mardi"
            Case "0001000" : Bin2Day = "Mercredi"
            Case "0010000" : Bin2Day = "Jeudi"
            Case "0100000" : Bin2Day = "Vendredi"
            Case "1000000" : Bin2Day = "Samedi"
            Case Else : Bin2Day = "?"
        End Select
    End Function

    ''' <summary>
    ''' Conversion Binaire en Double
    ''' </summary>
    ''' <param name="Bin"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Bin2dbl(ByVal Bin As String) As Double
        Dim y As Integer
        Dim z As Integer
        Dim ax As Double
        For y = Len(Bin) To 1 Step -1
            If Mid(Bin, y, 1) = "1" Then ax = ax + (2 ^ z)
            z += 1
        Next
        Bin2dbl = ax
    End Function

    ''' <summary>
    ''' Convertie le nombre de jour en date
    ''' </summary>
    ''' <param name="NbJour"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Jour2Date(ByVal NbJour As Integer) As String
        Dim Cal As DateTime = CDate("01/01/" & Now.Year)
        Cal.AddDays(NbJour)
        Jour2Date = Cal
    End Function

    ''' <summary>
    ''' Renvoi l'adresse pour getclock
    ''' </summary>
    ''' <param name="Bin"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAdd(ByVal Bin As String) As String
        Select Case Bin
            Case "1000000000000000" : GetAdd = "9"     '
            Case "0100000000000000" : GetAdd = "1"     '
            Case "0010000000000000" : GetAdd = "7"     '
            Case "0001000000000000" : GetAdd = "15"     '
            Case "0000100000000000" : GetAdd = "11"     '
            Case "0000010000000000" : GetAdd = "3"     '
            Case "0000001000000000" : GetAdd = "5"     '
            Case "0000000100000000" : GetAdd = "13"     '
            Case "0000000010000000" : GetAdd = "10"     '
            Case "0000000001000000" : GetAdd = "2"    '
            Case "0000000000100000" : GetAdd = "8"    '
            Case "0000000000010000" : GetAdd = "16"    '
            Case "0000000000001000" : GetAdd = "12"    '
            Case "0000000000000100" : GetAdd = "4"    '
            Case "0000000000000010" : GetAdd = "6"    '
            Case "0000000000000001" : GetAdd = "14"    '
            Case Else : GetAdd = "0"
        End Select
    End Function
#End Region

 
End Class
