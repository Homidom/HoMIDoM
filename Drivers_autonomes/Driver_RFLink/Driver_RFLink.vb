Imports HoMIDom
Imports HoMIDom.HoMIDom.Device
Imports HoMIDom.HoMIDom.Server
Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.IO.Ports
Imports System.Text.RegularExpressions

'************************************************
'INFOS 
'************************************************
'Le driver communique en "COM" avec l'arduino gateway qui doit implémenter un sketch spécifique compatible RFLink
'http://www.nemcon.nl/blog2/
'************************************************

Public Class Driver_RFLink
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "11376610-A000-11E5-9079-774A1D5D46B0"
    Dim _Nom As String = "RFLink"
    Dim _Enable As Boolean = False
    Dim _Description As String = "Driver RFLink"
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
    Dim _DEBUG As Boolean = False

#End Region

#Region "Variables Internes"
    Private serialPortObj As SerialPort
    'Public WithEvents port As New System.IO.Ports.SerialPort
    Dim _BAUD As Integer = 57600
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
                    'Case "ADRESSE2"
                    '   If Value = " " Then retour = "l'adresse est obligatoire"
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

                Catch ex As Exception
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "ERR: Erreur dans les paramétres avancés. utilisation des valeur par défaut" & ex.Message)
                    _DEBUG = False
                    _BAUD = 57600
                    _RCVERROR = True
                End Try
                _Ack = 0
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
                serialPortObj.WriteLine("10;REBOOT;")
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
                WriteLog("Le driver n'est pas démarré, impossible de communiquer avec le module RFLink")
                Exit Sub
            End If
            If _DEBUG Then WriteLog("DBG: WRITE Device " & Objet.Name & " <-- " & Command())

            'verification si adresse1 n'est pas vide
            If String.IsNullOrEmpty(Objet.Adresse1) Or Objet.Adresse1 = "" Then
                WriteLog("ERR: WRITE l'adresse du noeud RFLink doit etre renseigné : " & Objet.Name)
                Exit Sub
            End If

            '            'verification si adresse2 n'est pas vide
            '            If String.IsNullOrEmpty(Objet.Adresse2) Or Objet.Adresse2 = "" Then
            '                WriteLog("ERR: WRITE l'ID capteur/actionneur RFLink doit etre renseigné : " & Objet.Name)
            '                Exit Sub
            '            End If
            '
            '            Dim RFLinkCommand As String = ""
            '            Select Case UCase(Objet.Modele)
            '                '10/20;Protocol Name;Device Adress;Button Number;Action;
            '                Case "Kaku"
            '                    RFLinkCommand = "20;" + Objet.Modele + ";" + Objet.Adresse1 & ";1;"
            '                Case Else
            '                    WriteLog("ERR: WRITE : Ce type de capteur/actionneur ne peut pas être piloté : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
            '                    Exit Sub
            '            End Select
            '
            '            WriteLog("DBG: Commande passée à la passerelle RFLink : " & RFLinkCommand)
            '            serialPortObj.WriteLine(RFLinkCommand) ', 0, 8)

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
                WriteLog("Le driver n'est pas démarré, impossible de communiquer avec le module RFLink")
                Exit Sub
            End If
            '           If _DEBUG Then WriteLog("DBG: WRITE Device " & Objet.Name & " <-- " & Command)
            WriteLog("DBG: WRITE Device " & Objet.Name & " <-- " & Command)

            'verification si adresse1 n'est pas vide
            If String.IsNullOrEmpty(Objet.Adresse1) Or Objet.Adresse1 = "" Then
                WriteLog("ERR: WRITE l'adresse du composant doit etre renseigné : " & Objet.Name)
                Exit Sub
            End If

            '            'verification si adresse2 n'est pas vide
            '            If String.IsNullOrEmpty(Objet.Adresse2) Or Objet.Adresse2 = "" Then
            '                WriteLog("ERR: WRITE l'adresse du composant doit etre renseigné : " & Objet.Name)
            '                Exit Sub
            '            End If

            Dim RFLinkCommand As String = ""
            Dim RFAdresse() As String
            RFAdresse = Objet.Adresse1.Split("-")
            Select Case UCase(Objet.Modele)
                Case "RFLINK CTRL"
                    'Special Control Commands - Send:   
                    '--------------------------------   
                    '10;REBOOT;       => Reboot RFLink Gateway hardware   
                    '10;PING;         => a "keep alive" function. Is replied with: 20;99;PONG;   
                    '10;VERSION;      => Version and build indicator. Is replied with: 20;99;"RFLink Gateway software version";    
                    '10;RFDEBUG=ON;   => ON/OFF to Enable/Disable showing of RF packets. Is replied with: 20;99;RFDEBUG="state";   
                    '10;RFUDEBUG=ON;  => ON/OFF to Enable/Disable showing of undecoded RF packets. Is replied with: 20;99;RFUDEBUG="state";  
                    '10;QRFDEBUG=ON;  => ON/OFF to Enable/Disable showing of undecoded RF packets. Is replied with: 20;99;QRFDEBUG="state";   
                    '                    QRFDEBUG is a faster version of RFUDEBUG but all pulse times are shown in hexadecimal and need to be multiplied by 30   
                    '10;RTSCLEAN;     => Clean Rolling code table stored in internal EEPROM   
                    '10;RTSRECCLEAN=9 => Clean Rolling code record number (value from 0 - 15)   
                    '10;RTSSHOW;      => Show Rolling code table stored in internal EEPROM   
                    '10;STATUS;       => Reports the status of the various modules that can be enabled/disabled   
                    '                    20;B5;STATUS;setRF433=ON;NodoNRF=OFF;MilightNRF=ON;setLivingColors=ON;setAnsluta=OFF;setGPIO=OFF;   
                    '10;setRF433=ON   => enable/disable scanning of received 433/868 mhz signals   
                    '10;setGPIO=ON    => enable/disable scanning of IOPIN changes    
                    '10;setBLE=ON     => enable/disable scanning of Bluetooth LE signals, requires an NRF24L01 wired for Milight use, not together with Milight/MySensors   
                    '10;setMySensors=ON  => enable/disable scanning of MySensors signals, requires an NRF24L01 wired for Milight use, not together with Milight/BLE   
                    '10;setMilight=ON => enable/disable scanning of Milight RF signals, requires an NRF24L01 wired for Milight use, not together with BLE/MySensors    
                    '10;setNodoNRF=ON => enable/disable scanning of Nodo NRF signals, requires an NRF24L01 wired for Nodo use   
                    '10;setANSLUTA=ON => enable/disable scanning of Ikea Ansluta signals, requires a CC2500, not together with Livingcolors   
                    '10;setLIVINGCOLORS=O => enable/disable scanning of Living Colors Gen1 signals, requires a CC2500, not together with Ansluta   
                    '10;LicoClean;    => Erase the Living colors lamp address table 
                    '10;LicoShow;     => Show the Living colors lamp address table
                    '10;EEPClean;     => Clean all settings in EEPROM
                    Select Case UCase(Command)
                        Case "RFLINKCTRL"
                            RFLinkCommand = "10;" & Parametre1 & ";"
                        Case "RFLINKFULLCMD"
                            RFLinkCommand = Parametre1
                        Case Else
                            WriteLog("ERR: WRITE : La commande ne peut pas piloter le module RFLink : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
                            Exit Sub
                    End Select

                Case "RFLINK FULL CMD"
                    Select Case UCase(Command)
                        Case "RFLINKFULLCMD"
                            RFLinkCommand = Parametre1
                        Case Else
                            WriteLog("ERR: WRITE : Ce type de capteur/actionneur ne peut pas être piloté : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
                            Exit Sub
                    End Select

                Case "DELTRONIC"
                    '10;DELTRONIC;001c33;        => Deltronic protocol;address
                    RFLinkCommand = "10;" & Objet.Modele & ";" & RFAdresse(0) & ";"

                Case "MERTIK"
                    '10;MERTIK;64;UP;            => Mertik protocol, address, command
                    RFLinkCommand = "10;" & Objet.Modele & ";" & RFAdresse(0) & ";" & Command & ";"

                Case "RTS"
                    '10;RTS;1a602a;0;ON;         => RTS protocol, address, command (zero is unused for now)
                    '10;RTS;1b602b;0123;PAIR;    => Pairing for RTS rolling code:  RTS protocol, address, rolling code number, PAIR command
                    '10;RTS;1b602b;0123;0;PAIR;  => Extended Pairing for RTS rolling code:  RTS protocol, address, rolling code number, eeprom record number, PAIR command
                    Select Case UCase(Command)
                        Case "RFLINKFULLCMD"
                            RFLinkCommand = Parametre1
                        Case "RFLINKCMD"
                            If IsNothing(Parametre2) Then
                                RFLinkCommand = "10;" & Objet.Modele & ";" & RFAdresse(0) & ";" & RFAdresse(1) & ";" & Parametre1 & ";"
                            Else
                                RFLinkCommand = "10;" & Objet.Modele & ";" & RFAdresse(0) & ";" & RFAdresse(1) & ";" & Parametre2 & ";" & Parametre1 & ";"
                            End If
                        Case "ON", "OFF", "UP", "DOWN"
                            RFLinkCommand = "10;" & Objet.Modele & ";" & RFAdresse(0) & ";0;" & Command & ";"
                        Case "DIM"
                            If Not IsNothing(Parametre1) Then
                                Parametre1 = Hex(Parametre1)
                                RFLinkCommand = "10;" & Objet.Modele & ";" & RFAdresse(0) & ";" & RFAdresse(1) & ";" & Parametre1 & ";"
                            Else
                                WriteLog("ERR: DIM Il manque un parametre pour (" & Objet.Name & ")")
                                Exit Sub
                            End If

                        Case Else
                            WriteLog("ERR: WRITE : Ce type de capteur/actionneur ne peut pas être piloté : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
                            Exit Sub
                    End Select

                Case "SELECTPLUS"
                    '10;Selectplus;001c33; => SelectPlus protocol;address
                    RFLinkCommand = "10;" & Objet.Modele & ";" & RFAdresse(0) & ";"

                Case "MILIGHTV1"
                    '10;MiLightv1;F746;00;3c00;ON;     => Milight v1 protocol;address;button/unit number;color & brightness;command (ON/OFF/ALLON/ALLOFF/DISCO+/DISCO-/MODE0 - MODE8 
                    '10;MiLightv1;F746;01;34BC;PAIR;   => Milight v1 protocol;address;button/unit number;color & brightness -not relevant-; PAIR command 
                    '10;MiLightv1;F746;01;34BC;UNPAIR; => Milight v1 protocol;address;button/unit number;color & brightness -not relevant-; UNPAIR command 
                    '10;MiLightv1;F746;01;34BC;BRIGHT; => Milight v1 protocol;address;button/unit number;color & brightness; Set brightness
                    '10;MiLightv1;F746;01;34BC;COLOR;  => Milight v1 protocol;address;button/unit number;color & brightness; Set color 
                    Select Case UCase(Command)
                        Case "RFLINKFULLCMD"
                            RFLinkCommand = Parametre1
                        Case "RFLINKCMD"
                            If IsNothing(Parametre2) Then
                                RFLinkCommand = "10;" & Objet.Modele & ";" & RFAdresse(0) & ";" & RFAdresse(1) & ";" & Parametre1 & ";"
                            Else
                                RFLinkCommand = "10;" & Objet.Modele & ";" & RFAdresse(0) & ";" & RFAdresse(1) & ";" & Parametre2 & ";" & Parametre1 & ";"
                            End If
                        Case "ON", "OFF", "UP", "DOWN"
                            RFLinkCommand = "10;" & Objet.Modele & ";" & RFAdresse(0) & ";" & RFAdresse(1) & ";" & Command & ";"
                        Case "DIM"
                            If Not IsNothing(Parametre1) Then
                                Parametre1 = Hex(Parametre1)
                                RFLinkCommand = "10;" & Objet.Modele & ";" & RFAdresse(0) & ";" & RFAdresse(1) & ";" & Parametre1 & ";"
                            Else
                                WriteLog("ERR: DIM Il manque un parametre pour (" & Objet.Name & ")")
                                Exit Sub
                            End If
                        Case Else
                            WriteLog("ERR: WRITE : Ce type de capteur/actionneur ne peut pas être piloté : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
                            Exit Sub
                    End Select

                Case Else
                    '10;AB400D;00004d;1;OFF;     => Sartano     protocol;address;action (ON/OFF)
                    '10;Byron;112233;01;OFF;     => Dyron SX    protocol;address;ringtone
                    '10;BYRON;00009F;01;ON;      => Byron       protocol;address;chime number,command
                    '10;HomeConfort;01b523;D3;ON;=> HomeConfort protocol;address;action (ON/OFF)
                    '10;EV1527;000080;0;ON;      => EV1527      protocol;address;device 0x00-0x0f,action ON/OFF
                    '10;FA500;001b523;D3;ON;     => Flamingo    protocol;address;action (ON/OFF)
                    '10;Ikea Koppla;000080;0;ON; => Koppla      protocol;address;action (ON/OFF) 
                    '10;Impuls;00004d;1;OFF;     => Impuls      protocol;address;action (ON/OFF)
                    '10;Kambrook;050325;a1;ON;   => Kambrook    protocol;address;unit/button number;action (ON/OFF)
                    '10;Kaku;00004d;1;OFF;       => Kaku/ARC    protocol;address;action (ON/OFF)
                    '10;NewKaku;00c142;1;ON;     => Newkaku/AC  protocol;address (24 bits);button number (hexadecimal 0x0-0x0f);action (ON/OFF/ALLON/ALLOFF/15 - 1 to 15 for direct dim level)
                    '10;NewKaku;128ac4d;1;OFF;   => Newkaku/AC  protocol;address (28 bits);button number (hexadecimal 0x0-0x0f);action (ON/OFF/ALLON/ALLOFF/15 - 1 to 15 for direct dim level)
                    '10;Powerfix;000080;0;ON;    => Powerfix/Quigg/Chacon protocol;address;action (ON/OFF)
                    '10;UNITEC;7796;01;ON;       => Unitec regular ON command
                    '10;UNITEC;7796;01;PAIR;     => Unitec pair command to pair RFLink as a remote to a power switch 
                    '10;X10;000041;1;OFF;        => X10         protocol;address;action (ON/OFF)
                    '10;Eurodomest;123456;01;ON; => Eurodomest protocol;address;button number;action (ON/OFF/ALLON/ALLOFF)
                    '10;Blyss;ff98;A1;OFF;       => Blyss protocol;address;button;action (ON/OFF/ALLON/ALLOFF)
                    '10;Conrad;ff0607;1;OFF;     => Conrad RSL protocol, address, button number, action (ON/OFF/ALLON/ALLOFF)
                    '10;HomeEasy;7900b100;3;ON;  => Home Easy protocol;address;action (ON/OFF/ALLON/ALLOFF)
                    '10;Chuango;000080;2;ON;     => Chuango Protocol;address;action (ON/OFF/ALLON/ALLOFF)
                    '10;FA20RF;67f570;1;ON;      => Flamingo FA20RF / FA21 / KD101 protocol, address, button number, action (ON/OFF/ALLON/ALLOFF)
                    Select Case UCase(Command)
                        Case "RFLINKFULLCMD"
                            RFLinkCommand = Parametre1
                        Case "RFLINKCMD"
                            If IsNothing(Parametre2) Then
                                RFLinkCommand = "10;" & Objet.Modele & ";" & RFAdresse(0) & ";" & RFAdresse(1) & ";" & Parametre1 & ";"
                            Else
                                RFLinkCommand = "10;" & Objet.Modele & ";" & RFAdresse(0) & ";" & RFAdresse(1) & ";" & Parametre2 & ";" & Parametre1 & ";"
                            End If
                        Case "ON", "OFF", "UP", "DOWN"
                            RFLinkCommand = "10;" & Objet.Modele & ";" & RFAdresse(0) & ";" & RFAdresse(1) & ";" & Command & ";"
                        Case "DIM"
                            If Not IsNothing(Parametre1) Then
                                Parametre1 = Hex(Parametre1)
                                RFLinkCommand = "10;" & Objet.Modele & ";" & RFAdresse(0) & ";" & RFAdresse(1) & ";" & Parametre1 & ";"
                            Else
                                WriteLog("ERR: DIM Il manque un parametre pour (" & Objet.Name & ")")
                                Exit Sub
                            End If
                        Case Else
                            WriteLog("ERR: WRITE : Ce type de capteur/actionneur ne peut pas être piloté : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
                            Exit Sub
                    End Select
            End Select
            WriteLog("DBG: Commande passée au module RFLink : " & RFLinkCommand)
            serialPortObj.WriteLine(RFLinkCommand) ', 0, 8)

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
            add_paramavance("BaudRate", "Vitesse du port COM (57600 ou 9600)", 57600)
            add_paramavance("ErrorReceived", "Gérer les erreurs de réception (True=Activé, False=Désactivé)", True)
            'add_paramavance("AutoDiscover", "Permet de créer automatiquement des composants si ceux-ci n'existent pas encore (True/False)", False)
            '          add_paramavance("Acknoledge", "Activer l'accuser réception RFLink (True=Activé/False=Désactivé)", False)

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
            add_devicecommande("RFLinkCtrl", "Commande de parametrage du module RFLink.", 2)
            add_devicecommande("RFLinkFullCmd", "Permet d'utiliser une syntaxe de commande non supportée par HoMIDoM.", 1)
            add_devicecommande("RFLinkCmd", "", 2)

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "ID du noeud RFLink", "Valeur de type numérique")
            Add_LibelleDevice("ADRESSE2", "@", "")
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("MODELE", "Protocole RFLink", "Détail des protocoles utilisables dans la documentation du driver", "Aucun|RFLink Ctrl|RFLink Full Cmd|AB400D|Byron|Byron MP|Blyss|Chuango|Conrad|DELTRONIC|Eurodomest|EV1527|FA20RF|FA500|HomeConfort|HomeEasy|Ikea Koppla|Impuls|Kaku|Kambrook|MERTIK|NewKaku|Powerfix|RTS|Selectplus|X10")
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
                        For index As Integer = 5 To UBound(aryLine)
                            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Type '" & aryLine(index - 1) & "' " & aryLine(0) & ";" & aryLine(2) & ";" & aryLine(3) & ";" & aryLine(index - 1))
                            ' traitement(aryLine(0), aryLine(index - 1), aryLine(2), aryLine(3))
                            Dim RF_SubData() As String
                            Dim RF_Mode As String = aryLine(0)
                            Dim RF_Protocol As String = aryLine(2)
                            RF_SubData = aryLine(3).Split("=")
                            Dim RF_ID As String = RF_SubData(1)
                            RF_SubData = aryLine(index - 1).Split("=")
                            Dim RF_DataType As String = RF_SubData(0)
                            Dim RF_Value As String = RF_SubData(1)
                            If RF_DataType = "SWITCH" Then
                                RF_ID &= "-" & RF_Value
                                RF_SubData = aryLine(index).Split("=")
                                RF_Value = RF_SubData(1)
                            End If
                            traitement(RF_Mode, RF_DataType, RF_ID, RF_Value)
                            If RF_DataType = "SWITCH" Then Exit For
                        Next
                    ElseIf UBound(aryLine) = 3 Then
                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Datareceived", aryLine(2))
                    Else
                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Datareceived", "Format attendu incorrect")
                    End If
                End If
            Loop

        Catch Ex As Exception
            '_Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Datareceived", "Erreur:" & Ex.ToString)
        End Try
    End Sub

    ''' <summary>Traite les paquets reçus</summary>
    ''' <remarks></remarks>
    Private Sub traitement(ByVal mode As String, ByVal type As String, ByVal adresse As String, ByVal valeur As String)
        '    Private Sub traitement(ByVal msgtype As String, ByVal type As String, ByVal adresse As String, ByVal adresse2 As String, ByVal valeur As String)
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

            Select Case mode
                Case 20
                    '_Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Traitement : ", "Noeud " & adresse & " Sensor " & " Type " & msgtype & " Valeur " & valeur)
                    'valeur = vbNull
                    deviceupdate = True
                    Select Case type
                        Case "BAT"
                            _Type = "BATTERIE"
                            homidom_type = 3
                            Select Case valeur
                                Case "OK"
                                    valeur = 100
                                Case "LOW"
                                    valeur = 0
                            End Select
                        Case "BARO"
                            _Type = "BAROMETRE"
                            homidom_type = 2
                            valeur = Convert.ToInt32(valeur, 16) * 0.1
                        Case "SMOKEALERT", "PIR"
                            _Type = "DETECTEUR"
                            homidom_type = 6
                            Select Case valeur
                                Case "ON"
                                    valeur = True
                                Case "OFF"
                                    valeur = False
                            End Select
                        Case "WINDIR"
                            _Type = "DIRECTIONVENT"
                            homidom_type = 7
                        Case "WATT", "KWATT"
                            _Type = "ENERGIEINSTANTANEE"
                            homidom_type = 8
                        Case "HSTATUS", "BFORECAST", "CHIME", "SMOKEALERT", "CO2", "SOUND", "DIST", "METER", "VOLT", "CURRENT"
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 13
                        Case "AWINSP"
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 13
                            valeur = Convert.ToInt32(valeur, 16) * 0.1
                        Case "LUX", "WINGS"
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 13
                            valeur = Convert.ToInt32(valeur, 16)
                        Case "WINCHL", "WINTMP"
                            _Type = "GENERIQUEVALUE"
                            homidom_type = 13
                            valeur = Convert.ToInt32(valeur, 16)
                            If (valeur > 32768) Then
                                valeur = 32768 - valeur
                            End If
                            valeur = valeur * 0.1
                        Case "HUM"
                            _Type = "HUMIDITE"
                            homidom_type = 14
                        Case "RAIN"
                            _Type = "PLUIECOURANT"
                            homidom_type = 18
                            valeur = Convert.ToInt32(valeur, 16) * 0.1
                        Case "RAINTOT"
                            _Type = "PLUIETOTAL"
                            homidom_type = 19
                            valeur = Convert.ToInt32(valeur, 16) * 0.1
                        Case "SWITCH"
                            _Type = "SWITCH"
                            homidom_type = 20
                            Select Case valeur
                                Case "ON"
                                    valeur = True
                                Case "OFF"
                                    valeur = False
                            End Select
                        Case "TEMP"
                            _Type = "TEMPERATURE"
                            homidom_type = 22
                            valeur = Convert.ToInt32(valeur, 16)
                            If (valeur > 32768) Then
                                valeur = 32768 - valeur
                            End If
                            valeur = valeur * 0.1
                        Case "UV"
                            _Type = "UV"
                            homidom_type = 24
                            valeur = Convert.ToInt32(valeur, 16) * 0.1
                        Case "WINSP"
                            _Type = "VITESSEVENT"
                            homidom_type = 25
                            valeur = Convert.ToInt32(valeur, 16) * 0.1
                            'Case ""
                        Case Else
                            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Process", "Le type de device n'appartient pas à ce driver: " & type)
                            Exit Sub
                    End Select

                    listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_idsrv, adresse, _Type, Me._ID, True)
                    'un device trouvé on maj la valeur
                    If (listedevices.Count = 1) Then
                        If deviceupdate = True Then
                            listedevices.Item(0).Value = valeur
                            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Reception : ", "Noeud N° " & adresse & " capteur/actionneur " & " Valeur " & valeur)
                        End If
                    Else
                        'si autodiscover = true ou modedecouverte du serveur actif alors on crée le composant sinon on logue
                        If autodevice = True Then
                            If (_AutoDiscover Or _Server.GetModeDecouverte) Then
                                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Process", "Device non trouvé, AutoCreation du composant : " & _Type & " " & adresse & " " & ":" & valeur)
                                _Server.AddDetectNewDevice(adresse, _ID, homidom_type, "", valeur)
                            Else
                                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Process", "Device non trouvé : " & _Type & " " & adresse & " " & ":" & valeur)
                            End If
                        End If
                    End If
            End Select
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " traitement", "Exception : " & ex.Message & " --> " & adresse & " " & " : " & valeur)
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

