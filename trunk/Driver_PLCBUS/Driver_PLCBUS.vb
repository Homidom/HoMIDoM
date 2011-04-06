Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
'Imports System.Xml
'Imports System.Xml.Serialization
Imports STRGS = Microsoft.VisualBasic.Strings
Imports VB = Microsoft.VisualBasic
Imports System.IO.Ports
'Imports System.Math
'Imports System.Net.Sockets
'Imports System.Threading
'Imports System.Globalization

' Driver PLCBUS COM/USB(COM Virtuel)
' Pour la version USB, necessite l'installation du driver
' Auteur : David
' Date : 10/02/2011

<Serializable()> Public Class Driver_PLCBUS
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variable Driver"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "5AEDF3A8-3568-11E0-9DEC-D164DFD72085"
    Dim _Nom As String = "PLCBUS"
    Dim _Enable As String = False
    Dim _Description As String = "PLCBUS COM / USB (PLC1141)"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "COM"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = ""
    Dim _Port_TCP As String = ""
    Dim _IP_UDP As String = ""
    Dim _Port_UDP As String = ""
    Dim _Com As String = ""
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "PLCBUS"
    Dim _Version As String = "1.0"
    Dim _Picture As String = ""
    Dim _Server As HoMIDom.HoMIDom.Server
    Dim _Device As HoMIDom.HoMIDom.Device
    Dim _DeviceSupport As New ArrayList
    Dim MyTimer As New Timers.Timer

    'A ajouter dans les ppt du driver
    Public plcack As Boolean = True
    Public plctriphase As Boolean = False
#End Region

#Region "Déclaration"

    Private WithEvents port As New System.IO.Ports.SerialPort
    Private ackreceived As Boolean = False
    Private port_name As String = ""

    Dim com_to_hex As New Dictionary(Of String, String)
    Dim hex_to_com As New Dictionary(Of String, String)

    Private BufferIn(8192) As Byte
    Private firstbyte As Boolean = True
    Private bytecnt As Integer = 0
    Private recbuf(30) As Byte

    Public gestion_ack As Boolean = True
    Public gestion_triphase As Boolean = False
#End Region

#Region "Fonctions génériques"
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

#Region "Fonctions du Driver"

    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        Dim retour As String
        Try
            'ouverture du port suivant le Port Com ou IP
            If _Com <> "" Then
                retour = ouvrir(_Com, plcack, plctriphase)
            Else
                retour = "ERR: Port Com non défini. Impossible d'ouvrir le port !"
            End If
            'traitement du message de retour
            If STRGS.Left(retour, 4) = "ERR:" Then
                _IsConnect = False
                retour = STRGS.Right(retour, retour.Length - 5)
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS", "Driver non démarré : " & retour)
            Else
                _IsConnect = True
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "PLCBUS", retour)
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Start", ex.Message)
            _IsConnect = False
        End Try
    End Sub

    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Dim retour As String
        retour = fermer()
        If STRGS.Left(retour, 4) = "ERR:" Then
            retour = STRGS.Right(retour, retour.Length - 5)
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS", retour)
        Else
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "PLCBUS", retour)
        End If
    End Sub

    Public Sub Restart() Implements HoMIDom.HoMIDom.IDriver.Restart
        [Stop]()
        Start()
    End Sub

    Public Sub Read(ByVal Objet As Object) Implements HoMIDom.HoMIDom.IDriver.Read
        Try
            If (Objet.adresse1.ToString.Length > 1) Then
                'c'est une adresse std on fait un status request
                ecrire(Objet.adresse1, "STATUS_REQUEST")
            ElseIf (Objet.adresse1.ToString.Length = 1) Then
                'fastpooling
                If plctriphase Then
                    ecrire(Objet.adresse1, "ReportOnlyOnIdPulse3Phase")
                Else
                    ecrire(Objet.adresse1, "GetOnlyOnIdPulse")
                End If
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Read", ex.Message)
        End Try
    End Sub

    Public Sub Write(ByVal Objet As Object, ByVal Commande As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
        'Parametre1 = data1
        'Parametre2 = data2
        Dim sendtwice As Boolean = False
        Try
            If Parametre1 Is Nothing Then Parametre1 = 0
            If Parametre2 Is Nothing Then Parametre2 = 0
            If Objet.type = "APPAREIL" Or Objet.type = "LAMPE" Then sendtwice = True
            ecrire(Objet.adresse1, Commande, Parametre1, Parametre2, sendtwice)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Write", ex.Message)
        End Try
    End Sub

    Public Sub New()
        _DeviceSupport.Add(ListeDevices.SWITCH.ToString)
        _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN.ToString)
        _DeviceSupport.Add(ListeDevices.CONTACT.ToString)
        _DeviceSupport.Add(ListeDevices.APPAREIL.ToString)
        _DeviceSupport.Add(ListeDevices.LAMPE.ToString)
        _DeviceSupport.Add(ListeDevices.VOLET.ToString)

        'dictionnaire Commande STRING -> INT
        com_to_hex.Add("ALL_UNITS_OFF", 0)
        com_to_hex.Add("ALL_LIGHTS_ON", 1)
        com_to_hex.Add("ON", 2) 'data1 must be 100, data2 must be 0
        com_to_hex.Add("OFF", 3) 'data1 must be 0, data2 must be 0
        com_to_hex.Add("DIM", 4) 'light will dim until fade-stop func=11 is received /  data1 = Fade rate
        com_to_hex.Add("BRIGHT", 5) 'light will bright until fade-stop func=11 is received /  data1 = Fade rate
        com_to_hex.Add("ALL_LIGHTS_OFF", 6)
        com_to_hex.Add("All_USER_LIGHTS_ON", 7)
        com_to_hex.Add("All_USER_UNITS_OFF", 8)
        com_to_hex.Add("All_USER_LIGHTS_OFF", 9)
        com_to_hex.Add("BLINK", 10) 'data1=interval
        com_to_hex.Add("FADE_STOP", 11)
        com_to_hex.Add("PRESET_DIM", 12) 'data1=dim level, data2=rate
        com_to_hex.Add("STATUS_ON", 13)
        com_to_hex.Add("STATUS_OFF", 14)
        com_to_hex.Add("STATUS_REQUEST", 15)
        com_to_hex.Add("ReceiverMasterAddressSetup", 16) 'data1=New user code, data2=new home+unitcode
        com_to_hex.Add("TransmitterMasterAddressSetup", 17) 'data1=New user code, data2=new home+unitcode
        com_to_hex.Add("SceneAddressSetup", 18)
        com_to_hex.Add("SceneAddressErase", 19)
        com_to_hex.Add("AllSceneAddressErase", 20)
        com_to_hex.Add("Reserved1", 21)
        com_to_hex.Add("Reserved2", 22)
        com_to_hex.Add("Reserved3", 23)
        com_to_hex.Add("GetSignalStrength", 24) 'data1=signal strength
        com_to_hex.Add("GetNoiseStrength", 25) 'data1=Noise strength
        com_to_hex.Add("ReportSignalStrength", 26)
        com_to_hex.Add("ReportNoiseStrength", 27)
        com_to_hex.Add("GetAllIdPulse", 28)
        com_to_hex.Add("GetOnlyOnIdPulse", 29)
        com_to_hex.Add("ReportAllIdPulse3Phase", 30)
        com_to_hex.Add("ReportOnlyOnIdPulse3Phase", 31)

        'dictionnaire Commande INT -> STRING
        hex_to_com.Add(0, "ALL_UNITS_OFF")
        hex_to_com.Add(1, "ALL_LIGHTS_ON")
        hex_to_com.Add(2, "ON")
        hex_to_com.Add(3, "OFF")
        hex_to_com.Add(4, "DIM")
        hex_to_com.Add(5, "BRIGHT")
        hex_to_com.Add(6, "ALL_LIGHTS_OFF")
        hex_to_com.Add(7, "All_USER_LIGHTS_ON")
        hex_to_com.Add(8, "All_USER_UNITS_OFF")
        hex_to_com.Add(9, "All_USER_LIGHTS_OFF")
        hex_to_com.Add(10, "BLINK")
        hex_to_com.Add(11, "FADE_STOP")
        hex_to_com.Add(12, "PRESET_DIM")
        hex_to_com.Add(13, "STATUS_ON")
        hex_to_com.Add(14, "STATUS_OFF")
        hex_to_com.Add(15, "STATUS_REQUEST")
        hex_to_com.Add(16, "ReceiverMasterAddressSetup")
        hex_to_com.Add(17, "TransmitterMasterAddressSetup")
        hex_to_com.Add(18, "SceneAddressSetup")
        hex_to_com.Add(19, "SceneAddressErase")
        hex_to_com.Add(20, "AllSceneAddressErase")
        hex_to_com.Add(24, "GetSignalStrength")
        hex_to_com.Add(25, "GetNoiseStrength")
        hex_to_com.Add(26, "ReportSignalStrength")
        hex_to_com.Add(27, "ReportNoiseStrength")
        hex_to_com.Add(28, "GetAllIdPulse")
        hex_to_com.Add(29, "GetOnlyOnIdPulse")
        hex_to_com.Add(30, "ReportAllIdPulse3Phase")
        hex_to_com.Add(31, "ReportOnlyOnIdPulse3Phase")

    End Sub
#End Region

#Region "Fonctions propres au driver"
    Public Function ouvrir(ByVal numero As String, ByVal plcack As Boolean, ByVal plctriphase As Boolean) As String
        Try
            'recuperation de la configuration
            gestion_ack = plcack
            gestion_triphase = plctriphase
            'ouverture du port
            If Not _IsConnect Then
                port_name = numero
                port.PortName = numero 'nom du port : COM1
                port.BaudRate = 9600 'vitesse du port 300, 600, 1200, 2400, 9600, 14400, 19200, 38400, 57600, 115200
                port.Parity = IO.Ports.Parity.None 'pas de parité
                port.StopBits = IO.Ports.StopBits.One 'un bit d'arrêt par octet
                port.DataBits = 8 'nombre de bit par octet
                'port.Handshake = Handshake.None
                port.ReadTimeout = 3000
                port.WriteTimeout = 5000
                'port.RtsEnable = False 'ligne Rts désactivé
                'port.DtrEnable = False 'ligne Dtr désactivé
                port.Open()
                AddHandler port.DataReceived, New SerialDataReceivedEventHandler(AddressOf DataReceived)
                Return ("Port " & port_name & " ouvert")
            Else
                Return ("Port " & port_name & " dejà ouvert")
            End If
        Catch ex As Exception
            Return ("ERR: " & ex.Message)
        End Try
    End Function

    Public Function fermer() As String
        Try
            If _IsConnect Then
                If (Not (port Is Nothing)) Then ' The COM port exists.
                    If port.IsOpen Then
                        'vidage des tampons
                        Dim i As Integer = 0
                        port.DiscardOutBuffer()
                        Do While (port.BytesToWrite > 0 And i < 50) ' Wait for the transmit buffer to empty.
                            i = i + 1
                            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "PLCBUS Fermer", "Wait " & port.BytesToWrite & "BytesToWrite " & i)
                            wait(10)
                        Loop
                        i = 0
                        port.DiscardInBuffer()
                       Do While (port.BytesToRead > 0 And i < 20) ' Wait for the receipt buffer to empty.
                            i = i + 1
                            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "PLCBUS Fermer", "Wait " & port.BytesToRead & "BytesToRead " & i)
                            wait(10)
                        Loop
                        port.Close()
                        port.Dispose()
                        _IsConnect = False
                        Return ("Port " & port_name & " fermé")
                    Else
                        Return ("Port " & port_name & "  est déjà fermé")
                    End If
                Else
                    Return ("Port " & port_name & " n'existe pas")
                End If
            Else
                Return ("Port " & port_name & "  est déjà fermé (port_ouvert=false)")
            End If
        Catch ex As UnauthorizedAccessException
            Return ("ERR: Port " & port_name & " IGNORE")
            ' The port may have been removed. Ignore.
        End Try
        Return True
    End Function

    Private Function adresse_to_hex(ByVal adresse As String)
        'convertit une adresse du type L1 en byte
        Dim table() As String = {0, 16, 32, 48, 64, 80, 96, 112, 128, 144, 160, 176, 192, 208, 224, 240}
        If adresse.Length > 1 Then
            Return table(Asc(Microsoft.VisualBasic.Left(adresse, 1)) - 65) + CInt(Microsoft.VisualBasic.Right(adresse, adresse.Length - 1)) - 1
        Else
            Return table(Asc(adresse) - 65)
        End If
    End Function

    Private Function hex_to_adresse(ByVal adresse As Byte)
        'convertit une adresse en byte en type L1
        Dim x As Integer = 0
        Dim y As Integer = 0

        If adresse >= 128 Then
            x = x + 8
            adresse = adresse - 128
        End If
        If adresse >= 64 Then
            x = x + 4
            adresse = adresse - 64
        End If
        If adresse >= 32 Then
            x = x + 2
            adresse = adresse - 32
        End If
        If adresse >= 16 Then
            x = x + 1
            adresse = adresse - 16
        End If
        If adresse >= 8 Then
            y = y + 8
            adresse = adresse - 8
        End If
        If adresse >= 4 Then
            y = y + 4
            adresse = adresse - 4
        End If
        If adresse >= 2 Then
            y = y + 2
            adresse = adresse - 2
        End If
        If adresse >= 1 Then
            y = y + 1
            adresse = adresse - 1
        End If
        Return Chr(x + 65) & (y + 1)
    End Function

    Private Sub wait(ByVal msec As Integer)
        '100msec = 1 secondes
        Try
            Dim ticks = Date.Now.Ticks + (msec * 100000) '10000000 = 1 secondes
            Dim limite = 0
            While limite = 0
                If ticks <= Date.Now.Ticks Then limite = 1
            End While
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Wait", "Exception : " & ex.Message)
        End Try
    End Sub

    Private Function ecrire(ByVal adresse As String, ByVal commande As String, Optional ByVal data1 As Integer = 0, Optional ByVal data2 As Integer = 0, Optional ByVal ecriretwice As Boolean = False) As String
        'adresse= adresse du composant : A1
        'commande : ON, OFF...
        'data1 et 2, voir description des actions plus haut ou doc plcbus
        Dim _adresse = 0
        Dim _cmd = 0
        'Dim tblack() As DataRow

        If _IsConnect Then
            Dim usercode = &HD1 'D1
            Try
                _adresse = adresse_to_hex(adresse)
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Ecrire", "Adresse non valide : " & _adresse)
                Return ""
            End Try
            Try
                _cmd = com_to_hex(commande)
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Ecrire", "Commande non valide : " & commande)
                Return ""
            End Try

            Try
                '--- TriPhase ---
                If gestion_triphase Then _cmd = _cmd Or &H40
                '--- request acks --- (sauf pour les status_request car pas important et encombre le port)
                'If commande <> "STATUS_REQUEST" And commande <> "GetOnlyOnIdPulse" And commande <> "GetAllIdPulse" And commande <> "ReportAllIdPulse3Phase" And commande <> "ReportOnlyOnIdPulse3Phase" Then
                '    _cmd = _cmd Or &H20
                'End If
                ' --- correction data suivant la commande ---
                If commande = "ON" Then
                    data1 = 100
                    data2 = 0
                ElseIf commande = "OFF" Then
                    data1 = 0
                    data2 = 0
                End If

                Dim donnee() As Byte = {&H2, &H5, usercode, _adresse, _cmd, data1, data2, &H3}

                'ecriture sur le port
                port.Write(donnee, 0, donnee.Length)
                If ecriretwice Then port.Write(donnee, 0, donnee.Length) 'on ecrit deux fois : voir la norme PLCBUS

                'gestion des acks (sauf pour les status_request car pas important et encombre le port)
                If gestion_ack And Not attente_ack() And commande <> "STATUS_REQUEST" And commande <> "GetOnlyOnIdPulse" And commande <> "GetAllIdPulse" And commande <> "ReportAllIdPulse3Phase" And commande <> "ReportOnlyOnIdPulse3Phase" Then
                    'pas de ack, on relance l ordre
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Ecrire", "Pas de Ack -> resend : " & adresse & " : " & commande & " " & data1 & "-" & data2)
                    port.Write(donnee, 0, donnee.Length)
                    'port.Write(donnee, 0, donnee.Length) 'on ecrit deux fois : voir la norme PLCBUS
                    If Not attente_ack() Then
                        'pas de ack > NOK
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Ecrire", "Pas de Ack --> " & adresse & " : " & commande & " " & data1 & "-" & data2)
                        Return "" 'on renvoi rien car le composant n'a pas recu l'ordre
                    End If
                Else
                    wait(30) 'on attend 0.3s pour liberer le bus correctement
                End If
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Ecrire", "exception : " & ex.Message & " --> " & adresse & " : " & commande & " " & data1 & "-" & data2)
                Return "" 'on renvoi rien car il y a eu une erreur
            End Try

            'renvoie la valeur ecrite
            Select Case UCase(commande)
                Case "ON", "ALL_LIGHTS_ON", "All_USER_LIGHTS_ON" : Return "ON"
                Case "OFF", "ALL_UNITS_OFF", "ALL_LIGHTS_OFF", "All_USER_LIGHTS_OFF", "All_USER_UNITS_OFF" : Return "OFF"
                Case "PRESET_DIM" : Return "ON" 'data1
                Case Else : Return ""
            End Select

        Else
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Ecrire", "Port Fermé, impossible d ecrire : " & adresse & " : " & commande & " " & data1 & "-" & data2)
            Return ""
        End If
    End Function

    Private Function attente_ack() As Boolean
        'test si on recoit le ack pendant 1.5 secondes
        Dim nbtest As Integer = 0
        While nbtest < 15
            If ackreceived Then 'ack recu on sort
                ackreceived = False
                Return True
            Else
                nbtest += 1
                wait(10) 'on attend 0.1s
            End If
        End While
        Return False
    End Function

    Private Sub DataReceived(ByVal sender As Object, ByVal e As SerialDataReceivedEventArgs)
        'fonction qui lit les données sur le port serie
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
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Datareceived", "Exception : " & Ex.Message)
        End Try
    End Sub

    Private Sub ProcessReceivedChar(ByVal temp As Byte)
        'fonction qui rassemble un message complet
        'si c'est le premier byte qu'on recoit
        Try
            If firstbyte Then
                firstbyte = False
                bytecnt = 0
            End If
            recbuf(bytecnt) = temp
            bytecnt += 1
            If bytecnt = 9 Then
                firstbyte = True
                Process(recbuf)
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS ProcessReceivedChar", "Exception : " & ex.Message)
        End Try
    End Sub

    Private Sub Process(ByVal comBuffer() As Byte)
        'traite le message recu
        Dim plcbus_commande As String = ""
        Dim plcbus_adresse As String = ""
        Dim data1 As String = ""
        Dim data2 As String = ""
        Dim actif As Boolean
        Dim listeactif As String = ""
        Dim TblBits(7) As Boolean
        Dim unbyte As Byte

        Try
            If ((comBuffer(1) = &H6) And (comBuffer(0) = &H2) And (comBuffer(8) = &H3)) Then ' si trame de reponse valide
                plcbus_adresse = hex_to_adresse(comBuffer(3))
                plcbus_commande = hex_to_com(comBuffer(4))
                data1 = comBuffer(5)
                data2 = comBuffer(6)

                'test si c'est un ack
                unbyte = comBuffer(7)
                For Iteration As Integer = 0 To 7
                    TblBits(Iteration) = unbyte And 1
                    unbyte >>= 1
                Next
                If TblBits(4) Then
                    'c'est un Ack
                    ackreceived = True
                    If plcbus_commande = "STATUS_REQUEST" Or plcbus_commande = "GetOnlyOnIdPulse" Or plcbus_commande = "GetAllIdPulse" Then
                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "PLCBUS Process", " <- ACK :" & plcbus_commande & "-" & plcbus_adresse & " : " & data1 & "-" & data2 & " : " & comBuffer(7))
                    Else
                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "PLCBUS Process", " <- ACK :" & plcbus_commande & "-" & plcbus_adresse & " : " & data1 & "-" & data2 & " : " & comBuffer(7))
                    End If
                Else
                    'ce n'est pas un ack, je traite le paquet
                    ackreceived = False

                    Select Case plcbus_commande
                        Case "ON", "OFF", "BRIGHT", "BLINK", "FADE_STOP", "STATUS_REQUEST"
                            If data1 = 0 Then traitement("OFF", plcbus_adresse, plcbus_commande) Else traitement("ON", plcbus_adresse, plcbus_commande)
                        Case "DIM", "PRESET_DIM"
                            traitement(data1, plcbus_adresse, plcbus_commande)
                        Case "GetSignalStrength", "GetNoiseStrength"
                            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "PLCBUS Process", " <- " & plcbus_commande & "-" & plcbus_adresse & " : " & data1)
                        Case "ReceiverMasterAddressSetup", "TransmitterMasterAddressSetup"
                            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "PLCBUS Process", " <- " & plcbus_commande & "-" & plcbus_adresse & " : " & data1 & "-" & data2)
                        Case "ALL_UNITS_OFF", "All_USER_UNITS_OFF"
                            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "PLCBUS Process", " <- " & plcbus_commande & "-" & plcbus_adresse & " : " & data1 & "-" & data2)
                        Case "ALL_LIGHTS_ON", "ALL_LIGHTS_OFF", "All_USER_LIGHTS_ON", "All_USER_LIGHTS_OFF"
                            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "PLCBUS Process", " <- " & plcbus_commande & "-" & plcbus_adresse & " : " & data1 & "-" & data2)
                        Case "STATUS_ON"
                            traitement("ON", plcbus_adresse, plcbus_commande)
                        Case "STATUS_OFF"
                            traitement("OFF", plcbus_adresse, plcbus_commande)
                        Case "SceneAddressSetup", "SceneAddressErase", "AllSceneAddressErase"
                            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "PLCBUS Process", " <- " & plcbus_commande & "-" & plcbus_adresse & " : " & data1 & "-" & data2)
                        Case "ReportSignalStrength", "ReportNoiseStrength"
                            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "PLCBUS Process", " <- " & plcbus_commande & "-" & plcbus_adresse & " : " & data1 & "-" & data2)
                        Case "GetAllIdPulse", "ReportAllIdPulse3Phase"
                            unbyte = comBuffer(6)
                            For Iteration As Integer = 0 To 7
                                actif = (unbyte And 1)
                                If actif Then listeactif = listeactif & " " & Left(plcbus_adresse, 1) & (Iteration + 1)
                                unbyte >>= 1
                            Next
                            unbyte = comBuffer(5)
                            For Iteration As Integer = 0 To 7
                                actif = (unbyte And 1)
                                If actif Then listeactif = listeactif & " " & Left(plcbus_adresse, 1) & (Iteration + 9)
                                unbyte >>= 1
                            Next
                            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "PLCBUS Process", " <- Liste des composants actifs : " & listeactif)
                        Case "GetOnlyOnIdPulse", "ReportOnlyOnIdPulse3Phase"
                            unbyte = comBuffer(6)
                            For Iteration As Integer = 0 To 7
                                actif = (unbyte And 1)
                                If actif Then
                                    listeactif = listeactif & " " & Left(plcbus_adresse, 1) & (Iteration + 1)
                                    traitement("ON", Left(plcbus_adresse, 1) & (Iteration + 1), "GetOnlyOnIdPulse")
                                Else
                                    traitement("OFF", Left(plcbus_adresse, 1) & (Iteration + 1), "GetOnlyOnIdPulse")
                                End If
                                unbyte >>= 1
                            Next
                            unbyte = comBuffer(5)
                            For Iteration As Integer = 0 To 7
                                actif = (unbyte And 1)
                                If actif Then
                                    listeactif = listeactif & " " & Left(plcbus_adresse, 1) & (Iteration + 9)
                                    traitement("ON", Left(plcbus_adresse, 1) & (Iteration + 9), "GetOnlyOnIdPulse")
                                Else
                                    traitement("OFF", Left(plcbus_adresse, 1) & (Iteration + 9), "GetOnlyOnIdPulse")
                                End If
                                unbyte >>= 1
                            Next
                            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "PLCBUS Process", " <- Liste des composants ON : " & Left(plcbus_adresse, 1) & " -> " & listeactif)
                    End Select
                End If
            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Process", "Message recu invalide")
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Process", "Exception : " & ex.Message)
        End Try
    End Sub

    Private Sub traitement(ByVal valeur As String, ByVal adresse As String, ByVal plcbus_commande As String)
        If valeur <> "" Then
            Try

                'Recherche si un device affecté
                Dim listedevices As New ArrayList
                listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(adresse, "", Me._ID)
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
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Process", "Plusieurs devices correspondent à : " & adresse & ":" & valeur)
                Else
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS Process", "Device non trouvé : " & adresse & ":" & valeur)


                    'Ajouter la gestion des composants bannis (si dans la liste des composant bannis alors on log en debug sinon onlog device non trouve empty)


                End If
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "PLCBUS traitement", "Exception : " & ex.Message & " --> " & adresse & " : " & plcbus_commande & "-" & valeur)
            End Try
        End If
    End Sub

#End Region

End Class
