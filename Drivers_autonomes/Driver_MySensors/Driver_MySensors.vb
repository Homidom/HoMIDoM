Imports HoMIDom
Imports HoMIDom.HoMIDom.Device
Imports HoMIDom.HoMIDom.Server
Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.IO.Ports
Imports System.Text.RegularExpressions

'************************************************
'INFOS 
'************************************************
'Le driver communique en "COM" avec l'arduino gateway qui doit implémenter un sketch spécifique compatible MySensors
'http://mysensors.org
'************************************************

Public Class Driver_Arduino_USB
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "A5B6D4C4-FF24-11E4-9733-BF931D5D46B0"
    Dim _Nom As String = "MySensors"
    Dim _Enable As Boolean = False
    Dim _Description As String = "Driver MySensors"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "COM"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
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
    Dim _idsrv As String
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande)
    Dim _AutoDiscover As Boolean = False
    Dim _acknowledge As Boolean = False

    'param avancé
    Dim _Ack As Byte
    Dim _ACKNOLEDGE As Boolean = False
    Dim _DEBUG As Boolean = False

#End Region

#Region "Variables Internes"
    Private serialPortObj As SerialPort
    'Public WithEvents port As New System.IO.Ports.SerialPort
    Dim _BAUD As Integer = 9600
    Dim _RCVERROR As Boolean = True
    Dim first As Boolean = False
#End Region

#Region "Propriétés génériques"
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
                Case "ADRESSE1"
                    If Value = " " Then retour = "l'adresse est obligatoire"
                Case "ADRESSE2"
                    If Value = " " Then retour = "l'adresse est obligatoire"
            End Select
            Return retour
        Catch ex As Exception
            Return "Une erreur est apparue lors de la vérification du champ " & Champ & ": " & ex.ToString
        End Try
    End Function

    ''' <summary>Démarrer le du driver</summary>
    ''' <remarks></remarks>
    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        Try
            If Not _IsConnect Then
                Dim trv As Boolean = False
                Dim _ports As String = "<AUCUN>"

                'récupération des paramétres avancés
                Try
                    _DEBUG = _Parametres.Item(0).Valeur
                    _BAUD = _Parametres.Item(1).Valeur
                    _RCVERROR = _Parametres.Item(2).Valeur
                    _ACKNOLEDGE = _Parametres.Item(3).Valeur
                Catch ex As Exception
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "ERR: Erreur dans les paramétres avancés. utilisation des valeur par défaut" & ex.Message)
                    _ACKNOLEDGE = False
                    _DEBUG = False
                    _BAUD = 57600
                    _RCVERROR = True
                End Try
                '_Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " AdvParams", "_DEBUG " & _DEBUG)
                '_Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " AdvParams", "_BAUD " & _BAUD)
                '_Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " AdvParams", "_RCVERROR " & _RCVERROR)
                '_Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " AdvParams", "_ACKNOLEDGE " & _ACKNOLEDGE)
                _Ack = Convert.ToInt32(_ACKNOLEDGE)
                '_Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " AdvParams", "_Ack " & _Ack)

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

                serialPortObj = New SerialPort()
                serialPortObj.PortName = _Com
                serialPortObj.BaudRate = _BAUD
                serialPortObj.Parity = Parity.None
                serialPortObj.DataBits = 8
                serialPortObj.StopBits = 1
                serialPortObj.ReadTimeout = 50000
                serialPortObj.Encoding = System.Text.Encoding.GetEncoding("ISO-8859-1")

                If _RCVERROR Then AddHandler serialPortObj.ErrorReceived, New SerialErrorReceivedEventHandler(AddressOf serialPortObj_ErrorReceived)
                AddHandler serialPortObj.DataReceived, New SerialDataReceivedEventHandler(AddressOf DataReceived)

                If serialPortObj.IsOpen Then
                    serialPortObj.Close()
                End If

                serialPortObj.Open()
                serialPortObj.DiscardInBuffer()
                _IsConnect = True
                first = True
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Start", "Port " & _Com & " ouvert")
            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "Port " & _Com & " déjà ouvert")
            End If

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", ex.ToString)
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            If _IsConnect Then
                serialPortObj.Close()
                RemoveHandler serialPortObj.ErrorReceived, New SerialErrorReceivedEventHandler(AddressOf serialPortObj_ErrorReceived)
                RemoveHandler serialPortObj.DataReceived, New SerialDataReceivedEventHandler(AddressOf DataReceived)
                _IsConnect = False
            End If
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
    ''' <remarks></remarks>
    Public Sub Read(ByVal Objet As Object) Implements HoMIDom.HoMIDom.IDriver.Read
        Try
            If _Enable = False Then Exit Sub
            If _IsConnect = False Then
                WriteLog("Le driver n'est pas démarré, impossible de communiquer avec la passerelle MySensors")
                Exit Sub
            End If
            If _DEBUG Then WriteLog("DBG: WRITE Device " & Objet.Name & " <-- " & Command)

            'verification si adresse1 n'est pas vide
            If String.IsNullOrEmpty(Objet.Adresse1) Or Objet.Adresse1 = "" Then
                WriteLog("ERR: WRITE l'adresse du noeud MySensors doit etre renseigné : " & Objet.Name)
                Exit Sub
            End If

            'verification si adresse1 n'est pas vide
            If String.IsNullOrEmpty(Objet.Adresse2) Or Objet.Adresse2 = "" Then
                WriteLog("ERR: WRITE l'ID capteur/actionneur MySensors doit etre renseigné : " & Objet.Name)
                Exit Sub
            End If

            Dim MySensorsCommand As String = ""
            Select Case UCase(Objet.Modele)

                Case "V_TEMP"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";0;0" '0 
                Case "V_HUM"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";1;0" '1:
                Case "V_STATUS"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";2;0" '2:
                Case "V_LIGHT"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";2;0" '2:
                Case "V_PERCENTAGE"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";3;0" '3:
                Case "V_DIMMER"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";3;0" '3:
                Case "V_PRESSURE"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";4;0" '4:
                Case "V_FORECAST"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";5;0" '5:
                Case "V_RAIN"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";6;0" '6:
                Case "V_RAINRATE"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";7;0" '7:
                Case "V_WIND"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";8;0" '8:
                Case "V_GUST"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";9;0" '9:
                Case "V_DIRECTION"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";10;0" '10:
                Case "V_UV"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";11;0" '11:
                Case "V_WEIGHT"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";12;0" '12:
                Case "V_DISTANCE"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";13;0" '13:
                Case "V_IMPEDANCE"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";14;0" '14:
                Case "V_ARMED"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";15;0" '15:
                Case "V_TRIPPED"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";16;0" '16:
                Case "V_WATT"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";17;0" '17:
                Case "V_KWH"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";18;0" '18:
                Case "V_SCENE_ON"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";19;0" '19:
                Case "V_SCENE_OFF"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";20;0" '20:
                Case "V_HVAC_FLOW_STATE"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";21;0" '21:
                Case "V_HVAC_SPEED"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";22;0" '22:
                Case "V_LIGHT_LEVEL"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";23;0" '23:
                Case "V_VAR1"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";24;0" '24:
                Case "V_VAR2"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";25;0" '25:
                Case "V_VAR3"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";26;0" '26:
                Case "V_VAR4"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";27;0" '27:
                Case "V_VAR5"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";28;0" '28:
                Case "V_UP"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";29;0" '29:
                Case "V_DOWN"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";30;0" '30:
                Case "V_STOP"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";31;0" '31:
                Case "V_IR_SEND"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";32;0" '32:
                Case "V_IR_RECEIVE"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";33;0" '33:
                Case "V_FLOW"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";34;0" '34:
                Case "V_VOLUME"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";35;0" '35:
                Case "V_LOCK_STATUS"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";36;0" '36:
                Case "V_LEVEL"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";37;0" '37:
                Case "V_VOLTAGE"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";38;0" '38:
                Case "V_CURRENT"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";39;0" '39:
                Case "V_RGB"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";40;0" '40:
                Case "V_RGBW"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";41;0" '41:
                Case "V_ID"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";42;0" '42:
                Case "V_UNIT_PREFIX"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";43;0" '43:
                Case "V_HVAC_SETPOINT_COOL"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";44;0" '44:
                Case "V_HVAC_SETPOINT_HEAT"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";45;0" '45:
                Case "V_HVAC_FLOW_MODE"
                    MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";2;" & _Ack & ";46;0" '46:
                Case Else
                    WriteLog("ERR: WRITE : Ce type de capteur/actionneur ne peut pas être piloté : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
                    Exit Sub
            End Select

            WriteLog("DBG: Commande passée à la passerelle MySensors : " & MySensorsCommand)
            serialPortObj.WriteLine(MySensorsCommand) ', 0, 8)

        Catch ex As Exception
            WriteLog("ERR: WRITE " & ex.ToString)
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
            If _IsConnect = False Then
                WriteLog("Le driver n'est pas démarré, impossible de communiquer avec la passerelle MySensors")
                Exit Sub
            End If
            '           If _DEBUG Then WriteLog("DBG: WRITE Device " & Objet.Name & " <-- " & Command)
            WriteLog("DBG: WRITE Device " & Objet.Name & " <-- " & Command)

            'verification si adresse1 n'est pas vide
            If String.IsNullOrEmpty(Objet.Adresse1) Or Objet.Adresse1 = "" Then
                WriteLog("ERR: WRITE l'adresse du noeud MySensors doit etre renseigné : " & Objet.Name)
                Exit Sub
            End If

            'verification si adresse1 n'est pas vide
            If String.IsNullOrEmpty(Objet.Adresse2) Or Objet.Adresse2 = "" Then
                WriteLog("ERR: WRITE l'ID capteur/actionneur MySensors doit etre renseigné : " & Objet.Name)
                Exit Sub
            End If

            Dim MySensorsCommand As String = ""
            If Command = "VAR" Then
                Select Case UCase(Objet.Modele)
                    Case "V_VAR1"
                        If Not IsNothing(Parametre1) Then
                            MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";24;" & Parametre1
                        Else
                            WriteLog("ERR: V_VAR V_VAR1 Il manque un parametre pour (" & Objet.Name & ")")
                            Exit Sub
                        End If
                    Case "V_VAR2"
                        If Not IsNothing(Parametre1) Then
                            MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";25;" & Parametre1
                        Else
                            WriteLog("ERR: V_VAR V_VAR2 Il manque un parametre pour (" & Objet.Name & ")")
                            Exit Sub
                        End If
                    Case "V_VAR3"
                        If Not IsNothing(Parametre1) Then
                            MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";26;" & Parametre1
                        Else
                            WriteLog("ERR: V_VAR V_VAR3 Il manque un parametre pour (" & Objet.Name & ")")
                            Exit Sub
                        End If
                    Case "V_VAR4"
                        If Not IsNothing(Parametre1) Then
                            MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";27;" & Parametre1
                        Else
                            WriteLog("ERR: V_VAR V_VAR4 Il manque un parametre pour (" & Objet.Name & ")")
                            Exit Sub
                        End If
                    Case "V_VAR5"
                        If Not IsNothing(Parametre1) Then
                            MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";28;" & Parametre1
                        Else
                            WriteLog("ERR: V_VAR V_VAR5 Il manque un parametre pour (" & Objet.Name & ")")
                            Exit Sub
                        End If
                    Case Else
                        WriteLog("ERR: WRITE : Ce type de capteur/actionneur ne peut pas être piloté : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
                        Exit Sub
                End Select
            ElseIf Command = "UP" Then
                ' S_COVER
                ' V_UP (Window covering. Up)
                Select UCase(Objet.Modele)
                    Case "V_UP"
                        MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";29;0"
                        'If Not IsNothing(Parametre1) Then
                        'MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";29;" & Parametre1
                        'Else
                        'WriteLog("ERR: V_UP Il manque un parametre pour (" & Objet.Name & ")")
                        'Exit Sub
                        'End If
                    Case Else
                        WriteLog("ERR: WRITE : Ce type de capteur/actionneur ne peut pas être piloté : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
                        Exit Sub
                End Select
            ElseIf Command = "DOWN" Then
                ' S_COVER
                ' V_DOWN (Window covering. Down)
                Select Case UCase(Objet.Modele)
                    Case "V_DOWN"
                        MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";30;0"
                        ' If Not IsNothing(Parametre1) Then
                        'MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";30;" & Parametre1
                        'Else
                        'WriteLog("ERR: V_DOWN Il manque un parametre pour (" & Objet.Name & ")")
                        'Exit Sub
                        'End If
                    Case Else
                        WriteLog("ERR: WRITE : Ce type de capteur/actionneur ne peut pas être piloté : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
                        Exit Sub
                End Select
            ElseIf Command = "STOP" Then
                ' S_COVER
                ' V_STOP (Window covering. Stop)
                Select Case UCase(Objet.Modele)
                    Case "V_STOP"
                        MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";31;0"
                        'If Not IsNothing(Parametre1) Then
                        'MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";31;" & Parametre1
                        'Else
                        'WriteLog("ERR: V_STOP Il manque un parametre pour (" & Objet.Name & ")")
                        'Exit Sub
                        'End If
                    Case Else
                        WriteLog("ERR: WRITE : Ce type de capteur/actionneur ne peut pas être piloté : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
                        Exit Sub
                End Select
            ElseIf Command = "IRSEND" Then
                ' S_IR
                ' V_IR_SEND (This message contains a received IR-command)
                Select Case UCase(Objet.Modele)
                    Case "V_IR_SEND"
                        If Not IsNothing(Parametre1) Then
                            MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";32;" & Parametre1
                        Else
                            WriteLog("ERR: V_IR_SEND Il manque un parametre pour (" & Objet.Name & ")")
                            Exit Sub
                        End If
                    Case Else
                        WriteLog("ERR: WRITE : Ce type de capteur/actionneur ne peut pas être piloté : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
                        Exit Sub
                End Select
            ElseIf Command = "LOCKSTATUS" Then
                ' S_LOCK
                ' V_LOCK_STATUS (Set or get lock status. 1=Locked, 0=Unlocked)
                Select Case UCase(Objet.Modele)
                    Case "V_LOCK_STATUS"
                        If Not IsNothing(Parametre1) Then
                            MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";36;" & Parametre1
                        Else
                            WriteLog("ERR: V_LOCK_STATUS Il manque un parametre pour (" & Objet.Name & ")")
                            Exit Sub
                        End If
                    Case Else
                        WriteLog("ERR: WRITE : Ce type de capteur/actionneur ne peut pas être piloté : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
                        Exit Sub
                End Select
                ' S_DUST, S_AIR_QUALITY, S_SOUND(dB), S_VIBRATION(hz), S_LIGHT_LEVEL(lux)
                ' V_LEVEL (Used for sending level-value)
                Select Case UCase(Objet.Modele)
                    Case "V_LEVEL"
                        If Not IsNothing(Parametre1) Then
                            MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";37;" & Parametre1
                        Else
                            WriteLog("ERR: V_LEVEL Il manque un parametre pour (" & Objet.Name & ")")
                            Exit Sub
                        End If
                    Case Else
                        WriteLog("ERR: WRITE : Ce type de capteur/actionneur ne peut pas être piloté : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
                        Exit Sub
                End Select
            ElseIf Command = "COLORSENSOR" Then
                ' S_RGB_LIGHT, S_COLOR_SENSOR
                ' V_RGB (RGB value transmitted as ASCII hex string (I.e "ff0000" for red))
                Select Case UCase(Objet.Modele)
                    Case "V_RGB"
                        If Not IsNothing(Parametre1) Then
                            MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";40;" & Parametre1
                        Else
                            WriteLog("ERR: V_RGB Il manque un parametre pour (" & Objet.Name & ")")
                            Exit Sub
                        End If
                    Case Else
                        WriteLog("ERR: WRITE : Ce type de capteur/actionneur ne peut pas être piloté : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
                        Exit Sub
                End Select
            ElseIf Command = "RGB" Then
                ' S_RGBW_LIGHT
                ' V_RGBW (RGBW value transmitted as ASCII hex string (I.e "ff0000ff" for red + full white))
                Select Case UCase(Objet.Modele)
                    Case "V_RGBW"
                        If Not IsNothing(Parametre1) Then
                            MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";41;" & Parametre1
                        Else
                            WriteLog("ERR: V_RGBW Il manque un parametre pour (" & Objet.Name & ")")
                            Exit Sub
                        End If
                    Case Else
                        WriteLog("ERR: WRITE : Ce type de capteur/actionneur ne peut pas être piloté : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
                        Exit Sub
                End Select
            ElseIf Command = "HVACSETPOINTCOOL" Then
                ' S_HVAC
                ' V_HVAC_SETPOINT_COOL (HVAC cold setpoint)
                Select Case UCase(Objet.Modele)
                    Case "V_HVAC_SETPOINT_COOL"
                        If Not IsNothing(Parametre1) Then
                            MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";44;" & Parametre1
                        Else
                            WriteLog("ERR: V_HVAC_SETPOINT_COOL Il manque un parametre pour (" & Objet.Name & ")")
                            Exit Sub
                        End If
                    Case Else
                        WriteLog("ERR: WRITE : Ce type de capteur/actionneur ne peut pas être piloté : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
                        Exit Sub
                End Select
            ElseIf Command = "HVACSETPOINTHEAT" Then
                ' S_HVAC, S_HEATER
                ' V_HVAC_SETPOINT_HEAT (HVAC/Heater setpoint)
                Select Case UCase(Objet.Modele)
                    Case "V_HVAC_SETPOINT_HEAT"
                        If Not IsNothing(Parametre1) Then
                            MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";45;" & Parametre1
                        Else
                            WriteLog("ERR: V_HVAC_SETPOINT_HEAT Il manque un parametre pour (" & Objet.Name & ")")
                            Exit Sub
                        End If
                    Case Else
                        WriteLog("ERR: WRITE : Ce type de capteur/actionneur ne peut pas être piloté : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
                        Exit Sub
                End Select
            ElseIf Command = "HVACFLOWMODE" Then
                ' S_HVAC
                ' V_HVAC_FLOW_MODE (Flow mode for HVAC ("Auto", "ContinuousOn", "PeriodicOn"))
                Select Case UCase(Objet.Modele)
                    Case "V_HVAC_FLOW_MODE"
                        If Not IsNothing(Parametre1) Then
                            MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";46;" & Parametre1
                        Else
                            WriteLog("ERR: V_HVAC_FLOW_MODE Il manque un parametre pour (" & Objet.Name & ")")
                            Exit Sub
                        End If
                    Case Else
                        WriteLog("ERR: WRITE : Ce type de capteur/actionneur ne peut pas être piloté : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
                        Exit Sub
                End Select
            ElseIf Command = "HVACSPEED" Then
                ' S_HVAC, S_HEATER
                ' V_HVAC_SPEED (HVAC/Heater fan speed ("Min", "Normal", "Max", "Auto"))
                Select Case UCase(Objet.Modele)
                    Case "V_HVAC_SPEED"
                        If Not IsNothing(Parametre1) Then
                            MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";21;" & Parametre1
                        Else
                            WriteLog("ERR: V_HVAC_SPEED Il manque un parametre pour (" & Objet.Name & ")")
                            Exit Sub
                        End If
                    Case Else
                        WriteLog("ERR: WRITE : Ce type de capteur/actionneur ne peut pas être piloté : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
                        Exit Sub
                End Select
            ElseIf Command = "HVACFLOWSTATE" Then
                ' S_HVAC, S_HEATER
                ' V_HVAC_FLOW_STATE (Mode of header. One of "Off", "HeatOn", "CoolOn", or "AutoChangeOver")
                Select Case UCase(Objet.Modele)
                    Case "V_HVAC_FLOW_STATE"
                        If Not IsNothing(Parametre1) Then
                            MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";22;" & Parametre1
                        Else
                            WriteLog("ERR: V_HVAC_FLOW_STATE Il manque un parametre pour (" & Objet.Name & ")")
                            Exit Sub
                        End If
                    Case Else
                        WriteLog("ERR: WRITE : Ce type de capteur/actionneur ne peut pas être piloté : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
                        Exit Sub
                End Select
            ElseIf Command = "LIGHTLEVEL" Then
                ' S_LIGHT_LEVEL
                ' V_LIGHT_LEVEL (Uncalibrated light level. 0-100%. Use V_LEVEL for light level in lux.)
                Select Case UCase(Objet.Modele)
                    Case "V_LIGHT_LEVEL"
                        If Not IsNothing(Parametre1) Then
                            MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";23;" & Parametre1
                        Else
                            WriteLog("ERR: V_LIGHT_LEVEL Il manque un parametre pour (" & Objet.Name & ")")
                            Exit Sub
                        End If
                    Case Else
                        WriteLog("ERR: WRITE : Ce type de capteur/actionneur ne peut pas être piloté : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
                        Exit Sub
                End Select
            ElseIf Command = "SCENEON" Then
                ' S_SCENE_CONTROLLER
                ' V_SCENE_ON (Turn on a scene)
                Select Case UCase(Objet.Modele)
                    Case "V_SCENE_ON"
                        If Not IsNothing(Parametre1) Then
                            MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";19;" & Parametre1
                        Else
                            WriteLog("ERR: V_SCENE_ON Il manque un parametre pour (" & Objet.Name & ")")
                            Exit Sub
                        End If
                    Case Else
                        WriteLog("ERR: WRITE : Ce type de capteur/actionneur ne peut pas être piloté : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
                        Exit Sub
                End Select
            ElseIf Command = "SCENEOFF" Then
                ' S_SCENE_CONTROLLER
                ' V_SCENE_OFF (Turn off a scene)
                Select Case UCase(Objet.Modele)
                    Case "V_SCENE_OFF"
                        If Not IsNothing(Parametre1) Then
                            MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";20;" & Parametre1
                        Else
                            WriteLog("ERR: V_SCENE_OFF Il manque un parametre pour (" & Objet.Name & ")")
                            Exit Sub
                        End If
                    Case Else
                        WriteLog("ERR: WRITE : Ce type de capteur/actionneur ne peut pas être piloté : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
                        Exit Sub
                End Select
            ElseIf Command = "TRIPPED" Then
                ' S_DOOR, S_MOTION, S_SMOKE, S_SPRINKLER, S_WATER_LEAK, S_SOUND, S_VIBRATION, S_MOISTURE
                ' V_TRIPPED (Tripped status of a security sensor. 1=Tripped, 0=Untripped)
                Select Case UCase(Objet.Modele)
                    Case "V_TRIPPED"
                        If Not IsNothing(Parametre1) Then
                            MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";16;" & Parametre1
                        Else
                            WriteLog("ERR: V_TRIPPED Il manque un parametre pour (" & Objet.Name & ")")
                            Exit Sub
                        End If
                    Case Else
                        WriteLog("ERR: WRITE : Ce type de capteur/actionneur ne peut pas être piloté : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
                        Exit Sub
                End Select
            ElseIf Command = "ARMED" Then
                ' S_DOOR, S_MOTION, S_SMOKE, S_SPRINKLER, S_WATER_LEAK, S_SOUND, S_VIBRATION, S_MOISTURE
                ' V_ARMED (Armed status of a security sensor. 1=Armed, 0=Bypassed)
                Select Case UCase(Objet.Modele)
                    Case "V_ARMED"
                        If Not IsNothing(Parametre1) Then
                            MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";15;" & Parametre1
                        Else
                            WriteLog("ERR: V_ARMED Il manque un parametre pour (" & Objet.Name & ")")
                            Exit Sub
                        End If
                    Case Else
                        WriteLog("ERR: WRITE : Ce type de capteur/actionneur ne peut pas être piloté : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
                        Exit Sub
                End Select
                '            ElseIf Command = "PERCENTAGE" Then
                '                ' S_DIMMER
                '                ' V_PERCENTAGE (Percentage value. 0-100 (%))
                '                Select Case UCase(Objet.Modele)
                '                    Case "V_PERCENTAGE"
                '                        If Not IsNothing(Parametre1) Then
                '                            MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";3;" & Parametre1
                '                        Else
                '                            WriteLog("ERR: V_PERCENTAGE Il manque un parametre pour (" & Objet.Name & ")")
                '                            Exit Sub
                '                        End If
                '                    Case Else
                '                        WriteLog("ERR: WRITE : Ce type de capteur/actionneur ne peut pas être piloté : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
                '                        Exit Sub
                '                End Select
                '            ElseIf Command = "DIMMER" Then
                '                ' S_DIMMER
                '                ' V_DIMMER (Deprecated. Alias for V_PERCENTAGE. Dimmer value. 0-100 (%))
                '                Select Case UCase(Objet.Modele)
                '                    Case "V_DIMMER"
                '                        If Not IsNothing(Parametre1) Then
                '                            MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";3;" & Parametre1
                '                        Else
                '                            WriteLog("ERR: V_DIMMER Il manque un parametre pour (" & Objet.Name & ")")
                '                            Exit Sub
                '                        End If
                '                    Case Else
                '                        WriteLog("ERR: WRITE : Ce type de capteur/actionneur ne peut pas être piloté : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
                '                        Exit Sub
                '                End Select
            Else
                Select Case UCase(Objet.Modele)
                    Case "V_LIGHT", "V_STATUS"
                        Select Case Command
                            Case "ON"
                                MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";2;1"
                            Case "OFF"
                                MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";2;0"
                        End Select
                    Case "V_DIMMER", "V_PERCENTAGE"
                        Select Case Command
                            Case "ON"
                                MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";3;100"
                            Case "OFF"
                                MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";3;0"
                            Case "DIM"
                                If Not IsNothing(Parametre1) Then
                                    If IsNumeric(Parametre1) Then
                                        ''Conversion du parametre de % (0 à 100) en 0 à 255
                                        'Parametre1 = CInt(Parametre1 * 255 / 100)
                                        MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";3;" & Parametre1
                                    Else
                                        WriteLog("ERR: WRITE DIM Le parametre " & CStr(Parametre1) & " n'est pas un entier (" & Objet.Name & ")")
                                    End If
                                Else
                                    WriteLog("ERR: WRITE DIM Il manque un parametre (" & Objet.Name & ")")
                                End If
                            Case "PWM"
                                If Not IsNothing(Parametre1) Then
                                    If IsNumeric(Parametre1) Then
                                        If CInt(Parametre1) > 255 Then Parametre1 = 255
                                        If CInt(Parametre1) < 0 Then Parametre1 = 0
                                        'Conversion du parametre de 0 à 255 en % (0 à 100)
                                        Parametre1 = CInt(Parametre1 * 100 / 255)
                                        MySensorsCommand = Objet.Adresse1 & ";" & Objet.adresse2 & ";1;" & _Ack & ";3;" & Parametre1
                                    Else
                                        WriteLog("ERR: WRITE DIM Le parametre " & CStr(Parametre1) & " n'est pas un entier (" & Objet.Name & ")")
                                    End If
                                Else
                                    WriteLog("ERR: WRITE DIM Il manque un parametre (" & Objet.Name & ")")
                                End If
                            Case Else
                                WriteLog("ERR: Send AC : Commande invalide : " & Command & " (ON/OFF/DIM/PWM supporté sur une SORTIE: Analogique write)")
                                Exit Sub
                        End Select
                    Case Else
                        WriteLog("ERR: WRITE : Ce type de capteur/actionneur ne peut pas être piloté : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
                        Exit Sub
                End Select
            End If

            WriteLog("DBG: Commande passée à la passerelle MySensors : " & MySensorsCommand)
            serialPortObj.WriteLine(MySensorsCommand) ', 0, 8)

        Catch ex As Exception
            WriteLog("ERR: WRITE " & ex.ToString)
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
    ''' <remarks></remarks>
    Private Sub add_devicecommande(ByVal nom As String, ByVal description As String, ByVal nbparam As Integer)
        Try
            Dim x As New DeviceCommande
            x.NameCommand = nom
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
    Private Sub add_paramavance(ByVal nom As String, ByVal description As String, ByVal valeur As Object)
        Try
            Dim x As New HoMIDom.HoMIDom.Driver.Parametre
            x.Nom = nom
            x.Description = description
            x.Valeur = valeur
            _Parametres.Add(x)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " add_paramavance", "ERR: " & ex.Message)
        End Try
    End Sub

    ''' <summary>Creation d'un objet de type</summary>
    ''' <remarks></remarks>
    Public Sub New()
        Try
            _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString

            'Parametres avancés
            add_paramavance("Debug", "Activer le Debug complet (True/False)", False)
            add_paramavance("BaudRate", "Vitesse du port COM (57600 ou 9600)", 9600)
            add_paramavance("ErrorReceived", "Gérer les erreurs de réception (True=Activé, False=Désactivé)", True)
            'add_paramavance("AutoDiscover", "Permet de créer automatiquement des composants si ceux-ci n'existent pas encore (True/False)", False)
            add_paramavance("Acknoledge", "Activer l'accuser réception MySensors (True=Activé/False=Désactivé)", False)

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

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'ajout des commandes avancées pour les devices
            'add_devicecommande("COMMANDE", "DESCRIPTION", nbparametre)
            add_devicecommande("UP", "Ouverture volet de fenêtre", 0)
            add_devicecommande("DOWN", "Fermeture volet de fenetre", 0)
            add_devicecommande("STOP", "Arrêt commande volet de fenêtre", 0)
            add_devicecommande("IRSEND", "Message contenant une commande IR", 1)
            add_devicecommande("LOCKSTATUS", "Défini ou demande un stattu de vérouillage. 1=vérouillé, 0=dévérouillé", 1)
            add_devicecommande("RGB", "RGB valeur transmise sous forme de chaîne ASCII hex (Ex. ff0000 pour rouge", 1)
            add_devicecommande("RGBW", "RGBW valeur transmise sous forme de chaîne ASCII hex (Ex. ff0000ff pour rouge + Blanc", 1)
            add_devicecommande("HVACSETPOINTCOOL", "HVAC: consigne froid", 1)
            add_devicecommande("HVACSETPOINTHEAT", "HVAC: consigne chaud", 1)
            add_devicecommande("HVACFLOWMODE", "HVAC: Mode de fonctionnement (Auto, ContinuousOn, PeriodicOn)", 1)
            add_devicecommande("HVACSPEED", "HVAC: Vitesse du ventilateur (Min, Normal, Max, Auto)", 1)
            add_devicecommande("HVACFLOWSTATE", "Etat de fonctionnement (Off, HeatOn, CoolOn, AutoChangeOver", 1)
            add_devicecommande("LEVEL", "Utilisé pour envoyer un niveau", 1)
            add_devicecommande("LIGHTLEVEL", "Uncalibrated light level. 0-100%. Use V_LEVEL for light level in lux.", 1)
            add_devicecommande("SCENEON", "Active un sénario", 0)
            add_devicecommande("SCENEOFF", "Désactive un sénario", 0)
            add_devicecommande("TRIPPED", "État (déclenché) d'un capteur de sécurité. 1=Déclenché, 0=Non-Déclenché", 1)
            add_devicecommande("ARMED", "État (armé) d'un capteur de sécurité. 1=Activé, 0=Désactivé", 1)
            add_devicecommande("PERCENTAGE", "Valeur en pourcentage. 0-100 (%)", 1)
            add_devicecommande("DIMMER", "[Déconseillé] Alias de V_PERCENTAGE. Valeur en pourcentage. 0-100 (%)", 1)
            add_devicecommande("VAR", "Envoyer une valeur de type string à une variable V_VARx", 1)

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "ID du noeud MySensors", "Valeur de type numérique")
            Add_LibelleDevice("ADRESSE2", "ID du capteur/actionneur", "Valeur de type numérique")
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("MODELE", "TYPE MySensors", "Détail des types dans la documentation du driver", "V_ARMED|V_CURRENT|V_DIMMER|V_DIRECTION|V_DISTANCE|V_DOWN|V_DUST_LEVEL|V_FLOWVOLUME|V_FORECAST|V_GUST|V_HEATER|V_HEATER_SW|V_HUM|V_IMPEDANCE|V_IR_SEND|V_IR_RECEIVE|V_LIGHT|V_LIGHT_LEVEL|V_LOCK_STATUS|V_KWH|V_PRESSURE|V_RAIN|V_RAINRATE|V_SCENE_ON|V_SCENE_OFF|V_TEMP|V_STOP|V_TRIPPED|V_UP|V_UV|V_VAR1|V_VAR2|V_VAR3|V_VAR4|V_VAR5|V_VOLTAGE|V_WATT|V_WEIGHT|V_WIND")
            Add_LibelleDevice("REFRESH", "Refresh", "0")
            'Add_LibelleDevice("LASTCHANGEDUREE", "LastChange Durée", "")
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " New", ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)

    End Sub

#End Region

#Region "Fonctions internes"

    Private Sub serialPortObj_ErrorReceived(ByVal sender As Object, ByVal e As SerialErrorReceivedEventArgs)
        Select Case e.EventType
            Case SerialError.Frame
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ErrorReceived", "Error: Le matériel a détecté une erreur de trame")
            Case SerialError.Overrun
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ErrorReceived", "Error: Un dépassement de mémoire tampon de caractères s'est produit.Le caractère suivant est perdu")
            Case SerialError.RXOver
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ErrorReceived", "Error: Un dépassement de la mémoire tampon d'entrée s'est produit.Il n'y a plus de place dans la mémoire tampon d'entrée ou un caractère a été reçu après le caractère de fin de fichier")
                serialPortObj.DiscardInBuffer()
            Case SerialError.RXParity
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ErrorReceived", "Error: Le matériel a détecté une erreur de parité")
            Case SerialError.TXFull
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ErrorReceived", "Error: L'application a essayé de transmettre un caractère, mais la mémoire tampon de sortie était pleine")
            Case Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ErrorReceived", "Erreur inconnue, le driver va tenter de traiter les données")
                Dim line As String = serialPortObj.ReadLine()
                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ErrorReceived", "Données reçues: " & line)

        End Select
    End Sub

    ''' <summary>
    ''' Traite les infos reçus
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DataReceived(ByVal sender As Object, ByVal e As SerialDataReceivedEventArgs)
        Try
            serialPortObj.ReadTimeout = 1000
            Do
                Dim line As String = serialPortObj.ReadLine()
                If line Is Nothing Then
                    Exit Do
                Else
                    line = line.Replace(vbCr, "").Replace(vbLf, "")
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " DataReceived", "Données reçues: " & line)
                    Dim aryLine() As String
                    aryLine = line.Split(";")
                    ' Action après réception d'une trame sur le port COM/USB
                    If UBound(aryLine) >= 5 Then
                        Dim Commande As String = aryLine(3)
                        Dim Valeur As String = aryLine(5)
                        'If aryLine(0) = "DEBUG" Then
                        '    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Datareceived", "From Arduino : " & line)
                        'Else
                        Select Case aryLine(2)
                            Case "0" ' Message type "presentation"
                                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Message Type 'presentation' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                Select Case aryLine(4)
                                    Case "0" ' Door and window sensors
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_DOOR' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "1" ' Motion sensors
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_MOTION' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "2" ' Smoke sensor
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_SMOKE' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "3" ' Light Actuator (on/off) / Binary device (on/off), Alias for S_LIGHT 
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_LIGHT' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "4" ' Dimmable device of some kind
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_DIMMER' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "5" ' Window covers or shades
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_COVER' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "6" ' Temperature sensor
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_TEMP' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "7" ' Humidity sensor
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_HUM' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "8" ' Barometer sensor (Pressure)
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_BARO' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "9" ' Wind sensor
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_WIND' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "10" ' Rain sensor
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_RAIN' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "11" ' UV sensor
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_UV' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "12" ' Weight sensor for scales etc.
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_WEIGHT' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "13" ' Power measuring device, like power meters
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_POWER' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "14" ' Heater device
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_HEATE' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "15" ' Distance sensor
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_DISTANCE' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "16" ' Light sensor
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_LIGHT_LEVEL' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "17" ' Arduino node device
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_ARDUINO_NODE' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "18" ' Arduino repeating node device
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_ARDUINO_RELAY' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "19" ' Lock device
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_LOCK' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "20" ' Ir sender/receiver device
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_IR' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "21" ' Water meter
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_WATER' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "22" ' Air quality sensor e.g. MQ-2
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_AIR_QUALITY' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "23" ' Use this for custom sensors where no other fits.
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_CUSTOM' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "24" ' Dust level sensor
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_DUST' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "25" ' Scene controller device
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_SCENE_CONTROLLER' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "26" ' RGB light
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_RGB_LIGHT' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "27" ' RGBW light (with separate white component)
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_RGBW_LIGHT' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "28" ' Color sensor
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_COLOR_SENSOR' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "29" ' Thermostat/HVAC device
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_HVAC' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "30" ' Multimeter device
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_MULTIMETER' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "31" ' Sprinkler device
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_SPRINKLER' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "32" ' Water leak sensor
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_WATER_LEAK' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "33" ' Sound sensor
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_SOUND' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "34" ' Vibration sensor
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_VIBRATION' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "35" ' Moisture sensor
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_MOISTURE' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case Else
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'S_????' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                End Select
                            Case "1", "2" ' Message type "set/req"
                                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Message Type 'set/req' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                Select Case aryLine(4)
                                    Case "0" ' Temperature 
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_TEMP' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "1" ' Humidity
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_HUM' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "2" ' Binary status. 0=off 1=on / Deprecated. Alias for V_STATUS. Light status. 0=off 1=on
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_LIGHT' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "3" ' Percentage value. 0-100 (%) / Deprecated. Alias for V_PERCENTAGE. Dimmer value. 0-100 (%)
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_DIMMER' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "4" ' Atmospheric Pressure
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_PRESSURE' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "5" ' Whether forecast. One of "stable", "sunny", "cloudy", "unstable", "thunderstorm" or "unknown"
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_FORECAST' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "6" ' Amount of rain
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_RAIN' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "7" ' Rate of rain
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_RAINRATE' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "8" ' Windspeed
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_WIND' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "9" ' Gust
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_GUST' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "10" ' Wind direction
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_DIRECTION' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "11" ' UV light level
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_UV' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "12" ' Weight (for scales etc)
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_WEIGHT' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "13" ' Distance
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_DISTANCE' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "14" ' Impedance value
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_IMPEDANCE' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "15" ' Armed status of a security sensor. 1=Armed, 0=Bypassed
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_ARMED' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "16" ' Tripped status of a security sensor. 1=Tripped, 0=Untripped
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_TRIPPED' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "17" ' Watt value for power meters
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_WATT' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "18" ' Accumulated number of KWH for a power meter
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_KWH' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "19" ' Turn on a scene
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_SCENE_ON' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "20" ' Turn of a scene
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_SCENE_OFF' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "21" ' Mode of header. One of "Off", "HeatOn", "CoolOn", or "AutoChangeOver"
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_HEATER' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "22" ' HVAC/Heater fan speed ("Min", "Normal", "Max", "Auto")
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_HEATER_SW' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "23" ' Uncalibrated light level. 0-100%. Use V_LEVEL for light level in lux.
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_LIGHT_LEVEL' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "24" ' Custom value
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_VAR1' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "25" ' Custom value
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_VAR2' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "26" ' Custom value
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_VAR3' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "27" ' Custom value
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_VAR4' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "28" ' Custom value
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_VAR5' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "29" ' Window covering. Up.
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_UP' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "30" ' Window covering. Down.
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_DOWN' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "31" ' Window covering. Stop.
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_STOP' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "32" ' Send out an IR-command
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_IR_SEND' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "33" ' received IR-command
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_IR_RECEIVE' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "34" ' Flow of water (in meter)
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_FLOW' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "35" ' Water volume
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_VOLUME' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "36" ' Set or get lock status. 1=Locked, 0=Unlocked
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_LOCK_STATUS' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "37" ' Used for sending level-value
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_DUST_LEVEL' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "38" ' Voltage level
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_VOLTAGE' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "39" ' Current level
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_CURRENT' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "40" ' RGB value transmitted as ASCII hex string (I.e "ff0000" for red)
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_RGB' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "41" ' RGBW value transmitted as ASCII hex string (I.e "ff0000ff" for red + full white)
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_RGBW' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "42" ' Optional unique sensor id (e.g. OneWire DS1820b ids)
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_ID' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "43" ' Allows sensors to send in a string representing the unit prefix to be displayed in GUI. This is not parsed by controller! E.g. cm, m, km, inch.
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_UNIT_PREFIX' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "44" ' HVAC cold setpoint
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_HVAC_SETPOINT_COOL' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "45" ' HVAC/Heater setpoint
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_HVAC_SETPOINT_HEAT' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case "46" ' Flow mode for HVAC ("Auto", "ContinuousOn", "PeriodicOn")
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_HVAC_FLOW_MODE' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                        traitement(aryLine(2), aryLine(4), aryLine(0), aryLine(1), aryLine(5))
                                    Case Else
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'V_????' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                End Select

                            Case "3" ' Message type "internal"
                                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Message Type 'internal' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                Select Case aryLine(4)
                                    Case "0" ' Use this to report the battery level (in percent 0-100).
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'I_BATTERY_LEVEL' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                    Case "1" ' Sensors can request the current time from the Controller using this message. The time will be reported as the seconds since 1970
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'I_TIME' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                    Case "2" ' Used to request gateway version from controller.
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'I_VERSION' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                    Case "3" ' Use this to request a unique node id from the controller.
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'I_ID_REQUEST' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                    Case "4" ' Id response back to sensor. Payload contains sensor id.
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'I_ID_RESPONSE' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                    Case "5" ' Start/stop inclusion mode of the Controller (1=start, 0=stop).
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'I_INCLUSION_MODE' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                    Case "6" ' Config request from node. Reply with (M)etric or (I)mperal back to sensor.
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'I_CONFIG' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                    Case "7" ' When a sensor starts up, it broadcast a search request to all neighbor nodes. They reply with a I_FIND_PARENT_RESPONSE.
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'I_FIND_PARENT' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                    Case "8" ' Reply message type to I_FIND_PARENT request.
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'I_FIND_PARENT_RESPONSE' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                    Case "9" ' Sent by the gateway to the Controller to trace-log a message
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'I_LOG_MESSAGE' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                    Case "10" ' A message that can be used to transfer child sensors (from EEPROM routing table) of a repeating node.
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'I_CHILDREN' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                    Case "11" ' Optional sketch name that can be used to identify sensor in the Controller GUI
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'I_SKETCH_NAME' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                    Case "12" ' Optional sketch version that can be reported to keep track of the version of sensor in the Controller GUI.
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'I_SKETCH_VERSION' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                    Case "13" ' Used by OTA firmware updates. Request for node to reboot.
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'I_REBOOT' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                    Case "14" ' Send by gateway to controller when startup is complete.
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'I_GATEWAY_READY' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                    Case "15" ' Used between sensors when initialting signing.
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'I_REQUEST_SIGNING' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                    Case "16" ' Used between sensors when requesting nonce.
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'I_GET_NONCE' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                    Case "17" ' Used between sensors for nonce response.
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'I_GET_NONCE_RESPONSE' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                    Case Else
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type 'I_????' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                                End Select
                            Case "4" ' Message type "stream"
                                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Message Type 'stream' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))
                            Case Else
                                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Message Type '????' " & aryLine(0) & ";" & aryLine(1) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(4) & ";" & aryLine(5))

                        End Select
                        'End If
                    End If
                    'End If
                End If
            Loop

        Catch Ex As Exception
            '_Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Datareceived", "Erreur:" & Ex.ToString)
        End Try
    End Sub

    ''' <summary>Traite les paquets reçus</summary>
    ''' <remarks></remarks>
    Private Sub traitement(ByVal msgtype As String, ByVal type As String, ByVal adresse As String, ByVal adresse2 As String, ByVal valeur As String)
        '    Private Sub traitement(ByVal adresse As String, ByVal valeur As String)
        Try
            'correction valeur
            valeur = Regex.Replace(valeur, "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)

            'Recherche si un device affecté
            Dim listedevices As New ArrayList
            Dim homidom_type As Integer
            Dim _Type As String = ""
            Dim autodevice As Boolean = True
            Dim deviceupdate As Boolean = False

            Select Case msgtype
                Case 0
                    '_Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Traitement : ", "Noeud " & adresse & " Sensor " & adresse2 & " Type " & msgtype & " Valeur " & valeur)
                    valeur = vbNull
                    Select Case type
                        Case 0 'S_DOOR
                            _Type = "GENERIQUEBOOLEEN"
                            homidom_type = 12
                        Case 1 'S_MOTION
                            _Type = "GENERIQUEBOOLEEN"
                            homidom_type = 12
                        Case 2 'S_SMOKE
                            _Type = "GENERIQUEBOOLEEN"
                            homidom_type = 12
                        Case 3 'S_LIGHT
                            _Type = "LAMPE"
                            homidom_type = 16
                        Case 4 'S_DIMMER
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 14
                        Case 5 'S_COVER
                            _Type = "VOLET"
                            homidom_type = 27
                        Case 6 'S_TEMP
                            _Type = "TEMPERATURE"
                            homidom_type = 22
                        Case 7 'S_HUM
                            _Type = "HUMIDITE"
                            homidom_type = 14
                        Case 8 'S_BARO
                            _Type = "BAROMETRE"
                            homidom_type = 3
                        Case 9 'S_WIND
                            _Type = "VITESSEVENT"
                            homidom_type = 26
                        Case 10 'S_RAIN
                            _Type = "PLUIECOURANT"
                            homidom_type = 19
                        Case 11 'S_UV
                            _Type = "UV"
                            homidom_type = 25
                        Case 12 'S_WEIGHT
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 14
                        Case 13 'S_POWER
                            _Type = "ENERGIEINSTANTANEE"
                            homidom_type = 9
                        Case 14 'S_HEATER
                            _Type = "GENERIQUESTRING"
                            homidom_type = 13
                        Case 15 'S_DISTANCE
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 14
                        Case 16 'S_LIGHT_LEVEL
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 14
                        Case 17 'S_ARDUINO_NODE
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 14
                            autodevice = False
                        Case 18 'S_ARDUINO_RELAY
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 14
                            autodevice = False
                        Case 19 'S_LOCK
                            _Type = "GENERIQUEBOOLEEN"
                            homidom_type = 12
                        Case 20 'S_IR
                            _Type = "GENERIQUESTRING"
                            homidom_type = 13
                        Case 21 'S_WATER
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 14
                        Case 22 'S_AIR_QUALITY
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 14
                        Case 23 'S_CUSTOM
                            _Type = "GENERIQUESTRING"
                            homidom_type = 13
                        Case 24 'S_DUST
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 14
                        Case 25 'S_SCENE_CONTROLLER
                            _Type = "GENERIQUEBOOLEEN"
                            homidom_type = 12
                        Case 26 'S_RGB_LIGHT
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 14
                        Case 27 'S_RGBW_LIGHT
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 14
                        Case 28 'S_COLOR_SENSOR
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 14
                        Case 29 'S_HVAC
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 14
                        Case 30 'S_MULTIMETER
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 14
                        Case 31 'S_SPRINKLER
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 14
                        Case 32 'S_WATER_LEAK
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 14
                        Case 33 'S_SOUND
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 14
                        Case 34 'S_VIBRATION
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 14
                        Case 35 'S_MOISTURE
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 14
                    End Select
                Case 1, 2
                    '_Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Traitement : ", "Noeud " & adresse & " Sensor " & adresse2 & " Type " & msgtype & " Valeur " & valeur)
                    deviceupdate = True
                    Select Case type
                        Case 0 'V_TEMP (température)
                            _Type = "TEMPERATURE"
                            homidom_type = 22
                        Case 1 'V_HUM (pourcentage d'humidité)
                            _Type = "HUMIDITE"
                            homidom_type = 14
                        Case 2 'V_LIGHT (etat de la lumière on-off : 0=Off, 1=On)
                            _Type = "LAMPE"
                            homidom_type = 15
                        Case 3 'V_DIM (valeur du variateur 0-100%)
                            _Type = "GENERIQUESTRING"
                            homidom_type = 12
                        Case 4 'V_PRESSURE (pression atmosphérique)
                            _Type = "BAROMETRE"
                            homidom_type = 2
                        Case 5 'V_FORECAST (Prévisions météo stable "stable", ensoleillé "sunny", orage ""thunderstorm", instable "unstable", nuageux "unstable" ou inconnu "unknown")
                            _Type = "GENERIQUESTRING"
                            homidom_type = 12
                        Case 6 'V_RAIN (quantite de pluie)
                            _Type = "PLUIECOURANT"
                            homidom_type = 18
                        Case 7 'V_RAINRATE (intensité de la pluie)
                            _Type = "PLUIETOTAL"
                            homidom_type = 19
                        Case 8 'V_WIND (vitesse du vent)
                            _Type = "VITESSEVENT"
                            homidom_type = 25
                        Case 9 'V_GUST (type de vent)
                            _Type = "GENERIQUESTRING"
                            homidom_type = 12
                        Case 10 'V_DIRECTION (direction du vent)
                            _Type = "DIRECTIONVENT"
                            homidom_type = 7
                        Case 11 'V_UV (indice d'UV)
                            _Type = "UV"
                            homidom_type = 24
                        Case 12 'V_WEIGHT (poids)
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 13
                        Case 13 'V_DISTANCE (mesure de distance)
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 13
                        Case 14 'V_IMPEDANCE (valeur de l'impedance)
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 13
                        Case 15 'V_ARMED (état armé d'un capteur de sécurité armé/ignoré : 1=Armed, 0=Bypassed)
                            _Type = "GENERIQUEBOOLEEN"
                            homidom_type = 11
                        Case 16 'V_TRIPPED (état déclenché par un capteur de sécurité déclenché/non déclenché : 1=Tripped, 0=Untripped)
                            _Type = "GENERIQUEBOOLEEN"
                            homidom_type = 11
                        Case 17 'V_WATT (valeur en watts pour les compteurs électriques)
                            _Type = "ENERGIEINSTANTANEE"
                            homidom_type = 8
                        Case 18 'V_KMH (nombre cumulé de KW/h)
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 13
                        Case 19 'V_SCENE_ON (activer un sénario)
                            _Type = "GENERIQUEBOOLEEN"
                            homidom_type = 11
                        Case 20 'V_SCENE_OFF (désactiver un sénario)
                            _Type = "GENERIQUEBOOLEEN"
                            homidom_type = 11
                        Case 21 'V_HEATER (mode de chauffage : arrêt "Off", chaud "HeatOn", froid "CoolOn", changement automatique)
                            _Type = "GENERIQUESTRING"
                            homidom_type = 12
                        Case 22 'V_HEATER_SW (interrupteur d'alimentation du chauffage : 1=On, 0=Off)
                            _Type = "GENERIQUEBOOLEEN"
                            homidom_type = 11
                        Case 23 'V_LIGHT_LEVEL (niveau de lumière 0-100%)
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 13
                        Case 24 'V_VAR1 (valeur personnalisée N°1)
                            _Type = "GENERIQUESTRING"
                            homidom_type = 12
                        Case 25 'V_VAR2 (valeur personnalisée N°2)
                            _Type = "GENERIQUESTRING"
                            homidom_type = 12
                        Case 26 'V_VAR3 (valeur personnalisée N°3)
                            _Type = "GENERIQUESTRING"
                            homidom_type = 12
                        Case 27 'V_VAR4 (valeur personnalisée N°4)
                            _Type = "GENERIQUESTRING"
                            homidom_type = 12
                        Case 28 'V_VAR5 (valeur personnalisée N°5)
                            _Type = "GENERIQUESTRING"
                            homidom_type = 12
                        Case 29 'V_UP (commande de volet "Up")
                            _Type = "VOLET"
                            homidom_type = 26
                        Case 30 'V_DOWN (commande de volet "Down")
                            _Type = "VOLET"
                            homidom_type = 26
                        Case 31 'V_STOP (commande de volet "Stop")
                            _Type = "VOLET"
                            homidom_type = 26
                        Case 32 'V_IR_SEND (envoi d'une commande Infrarouge)
                            _Type = "GENERIQUESTRING"
                            homidom_type = 12
                        Case 33 'V_IR_RECEIVE (reception d'une commande Infrarouge)
                            _Type = "GENERIQUESTRING"
                            homidom_type = 12
                        Case 34 'V_FLOW (niveau d'eau en mètre)
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 13
                        Case 35 'V_VOLUME (volume d'eau)
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 13
                        Case 36 'V_LOCK_STATUS (status de vérouillage : 1=Locked, 0=Unlocked)
                            _Type = "GENERIQUEBOOLEEN"
                            homidom_type = 11
                        Case 37 'V_DUST_LEVEL ()
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 13
                        Case 38 'V_VOLTAGE (tension mesurée)
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 13
                        Case 39 'V_CURRENT (courant mesurée)
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 13
                        Case 40 'V_RGB (RGB value transmitted as ASCII hex string (I.e "ff0000" for red))
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 13
                        Case 41 'V_RGBW (RGBW value transmitted as ASCII hex string (I.e "ff0000ff" for red + full white))
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 13
                        Case 42 'V_ID(Optional unique sensor id (e.g. OneWire DS1820b ids))
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 13
                        Case 43 'V_UNIT_PREFIX (Allows sensors to send in a string representing the unit prefix to be displayed in GUI. This is not parsed by controller! E.g. cm, m, km, inch.)
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 13
                        Case 44 'V_HVAC_SETPOINT_COOL (HVAC cold setpoint)
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 13
                        Case 45 'V_HVAC_SETPOINT_HEAT (HVAC/Heater setpoint)
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 13
                        Case 46 'V_HVAC_FLOW_MODE (Flow mode for HVAC ("Auto", "ContinuousOn", "PeriodicOn"))
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 13
                        Case Else
                            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Process", "Le type de device n'appartient pas à ce driver: " & type)
                            Exit Sub
                    End Select
            End Select

            listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_idsrv, adresse, _Type, Me._ID, True)
            'un device trouvé on maj la valeur
            If (listedevices.Count = 1) Then
                If deviceupdate = True Then
                    listedevices.Item(0).Value = valeur
                    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Reception : ", "Noeud N° " & adresse & " capteur/actionneur " & adresse2 & " Valeur " & valeur)
                End If
            ElseIf (listedevices.Count > 1) Then
                For i As Integer = 0 To listedevices.Count - 1
                    If listedevices.Item(i).adresse2.ToUpper() = adresse2.ToUpper() Then
                        If deviceupdate = True Then
                            listedevices.Item(i).Value = valeur
                            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Reception : ", "Noeud N° " & adresse & " capteur/actionneur " & adresse2 & " Valeur " & valeur)
                        End If
                    End If
                Next
                '_Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Process", "Plusieurs devices correspondent à : " & adresse)
            Else
                'si autodiscover = true ou modedecouverte du serveur actif alors on crée le composant sinon on logue
                If autodevice = True Then
                    If (_AutoDiscover Or _Server.GetModeDecouverte) Then
                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Process", "Device non trouvé, AutoCreation du composant : " & _Type & " " & adresse & " " & adresse2 & ":" & valeur)
                        _Server.AddDetectNewDevice(adresse, _ID, homidom_type, adresse2, valeur)
                    Else
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Process", "Device non trouvé : " & _Type & " " & adresse & " " & adresse2 & ":" & valeur)
                    End If
                End If
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " traitement", "Exception : " & ex.Message & " --> " & adresse & " " & adresse2 & " : " & valeur)
        End Try
    End Sub

    Private Sub WriteLog(ByVal message As String)
        Try
            'utilise la fonction de base pour loguer un event
            If STRGS.InStr(message, "DBG:") > 0 Then
                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom, STRGS.Right(message, message.Length - 5))
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

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class

