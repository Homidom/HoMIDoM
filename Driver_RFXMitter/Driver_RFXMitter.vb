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
'-------------------------------------------------------------------------------------
'                                                                     
'                     Software License Agreement                      
'                                                                     
' A part of this code is owned by RFXCOM, and is protected under applicable copyright laws.
' 
' It is not allowed to use this code or any part of it in an exclusive or patented
' product without the express prior written permission of RFXCOM.
' It is not allowed to use this software or any part of it for non-RFXCOM products.
'
' Any use in violation of the foregoing restrictions may subject the  
' user to criminal sanctions under applicable laws, as well as to     
' civil liability for the breach of the terms and conditions of this license.                                                             
'                                                                      
'------------------------------------------------------------------------------------- 

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
    Dim _Description As String = "RFXMitter USB/Ethernet Interface"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "RF"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "192.168.1.10"
    Dim _Port_TCP As String = "10002"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = ""
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
    Dim _IdSrv As String
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande)

    'Ajoutés dans les ppt avancés dans New()
    Dim _rfxsynchro As Boolean = True 'synchronisation avec le receiver
    Dim _DEBUG As Boolean = False
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

    Dim adressetoint() As String = {"00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "0A", "0B", "0C", "0D", "0E", "0F", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "1A", "1B", "1C", "1D", "1E", "1F", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "2A", "2B", "2C", "2D", "2E", "2F", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "3A", "3B", "3C", "3D", "3E", "3F", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "4A", "4B", "4C", "4D", "4E", "4F", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "5A", "5B", "5C", "5D", "5E", "5F", "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "6A", "6B", "6C", "6D", "6E", "6F", "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "7A", "7B", "7C", "7D", "7E", "7F", "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "8A", "8B", "8C", "8D", "8E", "8F", "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "9A", "9B", "9C", "9D", "9E", "9F", "A0", "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "AA", "AB", "AC", "AD", "AE", "AF", "B0", "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9", "BA", "BB", "BC", "BD", "BE", "BF", "C0", "C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "CA", "CB", "CC", "CD", "CE", "CF", "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "DA", "DB", "DC", "DD", "DE", "DF", "E0", "E1", "E2", "E3", "E4", "E5", "E6", "E7", "E8", "E9", "EA", "EB", "EC", "ED", "EE", "EF", "F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "FA", "FB", "FC", "FD", "FE", "FF"}
    Dim adressetoint2() As String = {"0", "1", "2", "3"}
    Dim unittoint() As String = {"1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16"}

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
    Public ReadOnly Property OsPlatform() As String Implements HoMIDom.HoMIDom.IDriver.OsPlatform
        Get
            Return _OsPlatform
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
                    Write(MyDevice, Command, Param(0), Param(1))
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


            End Select
            Return retour
        Catch ex As Exception
            Return "Une erreur est apparue lors de la vérification du champ " & Champ & ": " & ex.ToString
        End Try
    End Function

    ''' <summary>Démarrer le du driver</summary>
    ''' <remarks></remarks>
    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        '_IsConnect = True
        Dim retour As String

        'récupération des paramétres avancés
        Try
            _rfxsynchro = _Parametres.Item(0).Valeur
            _DEBUG = _Parametres.Item(1).Valeur
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
        Try
            retour = fermer()
            If STRGS.Left(retour, 4) = "ERR:" Then
                retour = STRGS.Right(retour, retour.Length - 5)
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter Stop", retour)
            Else
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFXMitter Stop", retour)
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXmitter Stop", ex.Message)
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
            If _DEBUG Then WriteLog("WRITE Device " & Objet.Name & " <-- " & Command)
            'suivant le protocole, on lance la bonne fonction
            ' AC / X10 / ARC / WAVEMAN
            Select Case UCase(Objet.modele)
                Case "AC" 'AC : Chacon...
                    If IsNothing(Parametre1) Then
                        protocol_chacon(Objet.adresse1, Command, False)
                    Else
                        protocol_chacon(Objet.adresse1, Command, False, Parametre1)
                    End If
                Case "ACEU" 'AC norme Europe
                    If IsNothing(Parametre1) Then
                        protocol_chacon(Objet.adresse1, Command, True)
                    Else
                        protocol_chacon(Objet.adresse1, Command, True, Parametre1)
                    End If
                Case "X10"
                    protocol_x10(Objet.adresse1, Command)
                Case "ARC"
                    protocol_arc(Objet.adresse1, Command)
                Case "WAVEMAN"
                    protocol_waveman(Objet.Adresse1, Command)
                Case Else
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter WRITE", "Protocole non géré : " & Objet.Modele.ToString.ToUpper)
            End Select
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter WRITE", ex.ToString)
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

    ''' <summary>ajout des commandes avancées pour les devices</summary>
    ''' <remarks></remarks>
    Private Sub add_devicecommande(ByVal nom As String, ByVal description As String, ByVal nbparam As Integer)
        Try
            Dim x As New DeviceCommande
            x.NameCommand = nom
            x.DescriptionCommand = description
            x.CountParam = nbparam
            _DeviceCommandPlus.Add(x)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS add_devicecommande", "Exception : " & ex.Message)
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
    Private Sub add_paramavance(ByVal nom As String, ByVal description As String, ByVal valeur As Object)
        Try
            Dim x As New HoMIDom.HoMIDom.Driver.Parametre
            x.Nom = nom
            x.Description = description
            x.Valeur = valeur
            _Parametres.Add(x)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS add_devicecommande", "Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Creation d'un objet de type</summary>
    ''' <remarks></remarks>
    Public Sub New()
        Try
            _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString

            'Parametres avancés
            add_paramavance("synchro", "Synchronisation avec le receiver (True/False)", True)
            add_paramavance("Debug", "Activer le Debug complet (True/False)", False)

            'liste des devices compatibles
            _DeviceSupport.Add(ListeDevices.APPAREIL.ToString)
            _DeviceSupport.Add(ListeDevices.LAMPE.ToString)
            _DeviceSupport.Add(ListeDevices.SWITCH.ToString)
            _DeviceSupport.Add(ListeDevices.VOLET.ToString)

            'ajout des commandes avancées pour les devices
            'add_devicecommande("COMMANDE", "DESCRIPTION", nbparametre)
            add_devicecommande("GROUP_ON", "Protocole AC/ACEU/ARC : ON sur le groupe du composant", 2)
            add_devicecommande("GROUP_OFF", "Protocole AC/ACEU/ARC : OFF sur le groupe du composant", 2)
            add_devicecommande("GROUP_DIM", "Protocole AC/ACEU : DIM sur le groupe du composant", 2)
            add_devicecommande("BRIGHT", "Protocole X10 : Bright", 2)
            add_devicecommande("ALL_LIGHT_ON", "Protocole X10 : ALL_LIGHT_ON", 2)
            add_devicecommande("ALL_LIGHT_OFF", "Protocole X10 : ALL_LIGHT_OFF", 2)
            add_devicecommande("CHIME", "Protocole ARC : Chime", 2)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Adresse", "Adresse du composant. Le format dépend du protocole")
            Add_LibelleDevice("ADRESSE2", "@", "")
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("MODELE", "Protocole", "Nom du protocole à utiliser : AC / ACEU / X10 / ARC / WAVEMAN", "AC|ACEU|ARC|WAVEMAN|X10")
            Add_LibelleDevice("REFRESH", "@", "")
            Add_LibelleDevice("LASTCHANGEDUREE", "LastChange Durée", "")
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXmitter New", ex.Message)
        End Try
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
        Try
            'Thread.CurrentThread.CurrentCulture = New CultureInfo("en-US")
            'My.Application.ChangeCulture("en-US")
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
                    port.BaudRate = 4800 'vitesse du port : toujours 4800
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
            If _rfxsynchro Then
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
        Try
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

            If _DEBUG Then
                message = VB.Right("0" & Hex(kar(0)), 2)
                intEnd = ((kar(0) And &HF8) / 8)
                If (kar(0) And &H7) <> 0 Then
                    intEnd += 1
                End If
                For intIndex = 1 To intEnd
                    message = message + VB.Right("0" & Hex(kar(intIndex)), 2)
                Next
                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "RFXMitter Ecrirecommande", "Exception" & message)
            End If
            ack = False
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXmitter ecrirecommande", "Exception" & ex.Message)
        End Try
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
            If temp = protocolsynchro Then
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

    ''' <summary>Gestion du protocole CHACON - HomeEasy</summary>
    ''' <param name="adresse">Adresse du type 02F4416-1 (02F4416-1 en leftshifted : 0BD10580-1 dans le rfxmitter.exe) ou 0 (pour les Heaters)</param>
    ''' <param name="commande">commande ON, OFF, DIM, GROUP_ON, GROUP_OFF, GROUP_DIM, HEATER_ON, HEATER_OFF</param>
    ''' <param name="europe">Type Europe ou US ?</param>
    ''' <param name="dimlevel">Niveau du Dim</param>
    ''' <remarks></remarks>
    Private Sub protocol_chacon(ByVal adresse As String, ByVal commande As String, ByVal europe As Boolean, Optional ByVal dimlevel As Integer = 0)
        Try
            If commande <> "HEATER_ON" And commande <> "HEATER_OFF" Then
                If adresse.Length <> 9 Then
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter ECRIRE CHACON", "Adresse : incorrecte " & adresse & " (ex : 04E073E-1)")
                    Exit Sub
                End If
                Dim kar(5) As Byte
                'Dim adressetab As String() = adresse.Split("-")
                'If europe Then kar(0) = 34 Else kar(0) = 33
                'kar(1) = CByte(Array.IndexOf(adressetoint, adressetab(0)))
                'kar(2) = CByte(Array.IndexOf(adressetoint, adressetab(1)))
                'kar(3) = CByte(Array.IndexOf(adressetoint, adressetab(2)))
                'Select Case Array.IndexOf(adressetoint2, adressetab(3))
                '    Case 0 : kar(4) = 0
                '    Case 1 : kar(4) = &H40
                '    Case 2 : kar(4) = &H80
                '    Case 3 : kar(4) = &HC0
                'End Select

                Dim adressetab As String() = adresse.Split("-")
                If europe Then kar(0) = 34 Else kar(0) = 33
                'Leftshit (2) de l'adresse : 02F44160 -> 0BD10580
                Dim adresse_hex As Long = CLng("&h" & adressetab(0) & "0") << 2
                kar(1) = (adresse_hex And "&hFF000000") >> 24
                kar(2) = (adresse_hex And "&hFF0000") >> 16
                kar(3) = (adresse_hex And "&hFF00") >> 8
                kar(4) = (adresse_hex And "&hFF")


                'kar(1) = CByte(Array.IndexOf(adressetoint, adressetab(0).Substring(1, 2)))
                'kar(2) = CByte(Array.IndexOf(adressetoint, adressetab(0).Substring(3, 2)))
                'kar(3) = CByte(Array.IndexOf(adressetoint, adressetab(0).Substring(3, 2)))
                'Select Case Array.IndexOf(adressetoint2, adressetab(0).Substring(0, 1))
                '    Case 0 : kar(4) = 0
                '    Case 1 : kar(4) = &H40
                '    Case 2 : kar(4) = &H80
                '    Case 3 : kar(4) = &HC0
                'End Select

                Select Case commande
                    Case "ON"
                        kar(4) = kar(4) Or CByte(Array.IndexOf(unittoint, adressetab(1)))
                        kar(4) = kar(4) Or &H10
                        kar(5) = 0
                        WriteRetour(adresse, "", commande)
                    Case "OFF"
                        kar(4) = kar(4) Or CByte(Array.IndexOf(unittoint, adressetab(1)))
                        kar(5) = 0
                        WriteRetour(adresse, "", commande)
                    Case "GROUP_ON"
                        kar(4) = kar(4) Or CByte(Array.IndexOf(unittoint, adressetab(1)))
                        kar(4) = kar(4) Or &H30
                        kar(5) = 0
                        WriteRetour(adresse, "", "ON")
                    Case "GROUP_OFF"
                        kar(4) = kar(4) Or CByte(Array.IndexOf(unittoint, adressetab(1)))
                        kar(4) = kar(4) Or &H20
                        kar(5) = 0
                        WriteRetour(adresse, "", "OFF")
                    Case "DIM"
                        kar(4) = kar(4) Or CByte(Array.IndexOf(unittoint, adressetab(1)))
                        kar(5) = CByte(dimlevel) << 4
                        WriteRetour(adresse, "", dimlevel)
                    Case "GROUP_DIM"
                        kar(4) = kar(4) Or &H20
                        kar(5) = CByte(dimlevel) << 4
                        WriteRetour(adresse, "", dimlevel)
                End Select
                ecrirecommande(kar)
            Else
                Dim kar(2) As Byte
                Select Case commande
                    Case "HEATER_ON"
                        kar(0) = 12
                        kar(1) = CByte(adresse) << 3
                        kar(1) = kar(1) Or &H4
                        kar(2) = &HD0
                        WriteRetour(adresse, "", "ON")
                    Case "HEATER_OFF"
                        kar(0) = 12
                        kar(1) = CByte(adresse) << 3
                        kar(1) = kar(1) Or &H4
                        kar(2) = &HB0
                        WriteRetour(adresse, "", "OFF")
                End Select
                ecrirecommande(kar)
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter ECRIRE CHACON", ex.ToString)
        End Try
    End Sub

    ''' <summary>Gestion du protocole X10 RF</summary>
    ''' <param name="adresse">Adresse du type A1</param>
    ''' <param name="commande">commande ON, OFF, BRIGHT, DIM, ALL_LIGHT_ON, ALL_LIGHT_OFF</param>
    ''' <remarks></remarks>
    Private Sub protocol_x10(ByVal adresse As String, ByVal commande As String)
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
                    WriteRetour(adresse, "", commande)
                Case "OFF"
                    kar(1) = currentunit
                    kar(3) = currentdevice Or &H20
                    WriteRetour(adresse, "", commande)
                Case "BRIGHT"
                    kar(1) = currentunit And &HF0
                    kar(3) = &H88
                    WriteRetour(adresse, "", "ON")
                Case "DIM"
                    kar(1) = currentunit And &HF0
                    kar(3) = &H98
                    WriteRetour(adresse, "", "ON")
                Case "ALL_LIGHT_ON"
                    kar(1) = currentunit And &HF0
                    kar(3) = &H90
                    WriteRetour(adresse, "", "ON")
                    'traiter toutes les lights



                Case "ALL_LIGHT_OFF"
                    kar(1) = currentunit And &HF0
                    kar(3) = &H80
                    WriteRetour(adresse, "", "OFF")
                    'traiter toutes les lights




            End Select
            kar(0) = &H20
            kar(2) = &HFF - kar(1)
            kar(4) = &HFF - kar(3)

            ecrirecommande(kar)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter ECRIRE X10", ex.Message)
        End Try
    End Sub

    ''' <summary>Gestion du protocole ARC</summary>
    ''' <param name="adresse">Adresse du type A1</param>
    ''' <param name="commande">commande ON, OFF, GROUP_ON, GROUP_OFF, CHIME</param>
    ''' <remarks></remarks>
    Private Sub protocol_arc(ByVal adresse As String, ByVal commande As String)
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
                    WriteRetour(adresse, "", commande)
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
                    WriteRetour(adresse, "", commande)
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
                    WriteRetour(adresse, "", "ON")
                    'traiter toutes le groupe




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
                    WriteRetour(adresse, "", "OFF")
                    'traiter toutes le groupe




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

    ''' <summary>Gestion du protocole WAVEMAN</summary>
    ''' <param name="adresse">Adresse du type A1</param>
    ''' <param name="commande">commande ON, OFF</param>
    ''' <remarks></remarks>
    Private Sub protocol_waveman(ByVal adresse As String, ByVal commande As String)
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
            WriteRetour(adresse, "", commande)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter ECRIRE WAVEMAN", ex.Message)
        End Try
    End Sub


#End Region

#Region "Write"

    Private Sub WriteLog(ByVal message As String)
        Try
            'utilise la fonction de base pour loguer un event
            If STRGS.InStr(message, "DBG:") > 0 Then
                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "RFXMitter", STRGS.Right(message, message.Length - 4))
            ElseIf STRGS.InStr(message, "ERR:") > 0 Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter", STRGS.Right(message, message.Length - 4))
            Else
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFXMitter", STRGS.Right(message, message.Length - 4))
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFXMitter WriteLog", ex.Message)
        End Try
    End Sub

    Private Sub WriteRetour(ByVal adresse As String, ByVal type As String, ByVal valeur As String)
        Try
            'Forcer le . 
            'Thread.CurrentThread.CurrentCulture = New CultureInfo("en-US")
            'My.Application.ChangeCulture("en-US")

            'log tous les paquets en mode debug
            If _DEBUG Then WriteLog("DBG: WriteRetour receive from " & adresse & " (" & type & ") -> " & valeur)

            If Not _IsConnect Then Exit Sub 'si on ferme le port on quitte
            If DateTime.Now < DateAdd(DateInterval.Second, 10, dateheurelancement) Then Exit Sub 'on ne traite rien pendant les 10 premieres secondes

            'Recherche si un device affecté
            Dim listedevices As New ArrayList
            listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, adresse, type, Me._ID, True)
            If (listedevices.Count = 1) Then
                'un device trouvé on maj la value
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
                WriteLog("ERR: Plusieurs devices correspondent à : " & type & " " & adresse & ":" & valeur)
            Else
                'on vérifie si le device est configuré en RFXReceiver
                listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, adresse, type, "3B808B6C-25B3-11E0-A6DB-36D2DED72085", True)
                If (listedevices.Count = 1) Then
                    'un device trouvé on maj la value
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
                Else
                    WriteLog("ERR: Device non trouvé : " & type & " " & adresse & ":" & valeur)
                End If


                'Ajouter la gestion des composants bannis (si dans la liste des composant bannis alors on log en debug sinon onlog device non trouve empty)


            End If
        Catch ex As Exception
            WriteLog("ERR: Writeretour Exception : " & ex.Message)
        End Try
    End Sub

#End Region

End Class
