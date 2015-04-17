Imports HoMIDom
Imports HoMIDom.HoMIDom.Device
Imports HoMIDom.HoMIDom.Server
Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.IO.Ports
Imports System.Text.RegularExpressions

'************************************************
'INFOS 
'************************************************
'Le driver communique en "COM" avec l'arduino maitre qui doit implémenter un sketch spécifique
'************************************************

Public Class Driver_Arduino_USB
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "1CE6CD10-ABAB-11E4-B53A-25491E5D46B0"
    Dim _Nom As String = "Arduino++ USB"
    Dim _Enable As Boolean = False
    Dim _Description As String = "Driver Arduino Etendu USB (Permet de piloter plusieurs arduino et shields spécifiques utilisants un protocole 1-Wire, I2C, SPI)"
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

    'param avancé
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
                Catch ex As Exception
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "ERR: Erreur dans les paramétres avancés. utilisation des valeur par défaut" & ex.Message)
                    _DEBUG = False
                    _BAUD = 57600
                    _RCVERROR = True
                End Try

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

        ' Initialisation des I/O utilisées
        Try
            Dim ListeDevices As New ArrayList
            ListeDevices = _Server.ReturnDeviceByDriver(_idsrv, Me._ID, True)
            If ListeDevices IsNot Nothing Then
                For Each _dev In ListeDevices
                    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Liste de composants : ", "Arduino N° " & _dev.adresse1.ToString & " Pin " & _dev.adresse2.ToString & " Valeur " & _dev.valeur.ToString)
                Next
            End If

        Catch ex As Exception

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
                WriteLog("Le driver n'est pas démarré, impossible de communiquer avec l'arduino")
                Exit Sub
            End If
            If _DEBUG Then WriteLog("DBG: WRITE Device " & Objet.Name & " <-- " & Command())

            'verification si adresse1 n'est pas vide
            If String.IsNullOrEmpty(Objet.Adresse1) Or Objet.Adresse1 = "" Then
                WriteLog("ERR: WRITE l'adresse de l'arduino doit etre renseigné (ex: 1 pour un arduino maitre, 2-255 pour un arduino esclave) : " & Objet.Name)
                Exit Sub
            End If


            'suivant le type du PIN on lance la bonne commande : ENTREE_ANA|ENTREE_DIG|SORTIE_DIG|PWM|1WIRE

            Dim arduinocommande As String = ""
            'If Command = "CONFIG_TYPE_PIN" Then
            '    Select Case UCase(Objet.Modele)
            '        Case "ENTREE_DIG" : arduinocommande = "255|0|" & Objet.Adresse1 & "|" & Objet.Adresse2 & "|0|" & Objet.Parametre1 & "|"
            '        Case "SORTIE_DIG" : arduinocommande = "255|0|" & Objet.Adresse1 & "|" & Objet.Adresse2 & "|1|" & Objet.Parametre1 & "|"
            '        Case "ENTREE_ANA" : arduinocommande = "255|0|" & Objet.Adresse1 & "|" & Objet.Adresse2 & "|2|" & Objet.Parametre1 & "|"
            '        Case "PWM" : arduinocommande = "255|0|" & Objet.Adresse1 & "|" & Objet.Adresse2 & "|3|" & Objet.Parametre1 & "|"
            '        Case "1WIRE" : arduinocommande = "255|0|" & Objet.Adresse1 & "|" & Objet.Adresse2 & "|7|" & Objet.Parametre1 & "|"
            '        Case Else
            '            WriteLog("ERR: WRITE CONFIG_TYPE_PIN : Type de PIN non géré : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
            '            Exit Sub
            '    End Select
            'Else
            Select Case UCase(Objet.Modele)
                Case "ANALOG_IN"
                    arduinocommande = "RF ADR " & Objet.Adresse1 & " AR " & Objet.Adresse2
                    WriteLog("DBG: Commande passée à l arduino de type : ANALOG_IN")
                Case "DIGITAL_IN"
                    WriteLog("DBG: Commande passée à l arduino de type : DIGITAL_IN")
                    arduinocommande = "RF ADR " & Objet.Adresse1 & " DR " & Objet.Adresse2
                Case "DHTXX"
                    WriteLog("DBG: Commande passée à l arduino de type : DHTXX")
                    arduinocommande = "RF ADR " & Objet.Adresse1 & " DHTXX " & Objet.Adresse2
                Case "BMP180"
                    WriteLog("DBG: Commande passée à l arduino de type : BMP180")
                    arduinocommande = "RF ADR " & Objet.Adresse1 & " BMP180 " & Objet.Adresse2
                Case "CUSTOM"
                    WriteLog("DBG: Commande passée à l arduino de type : CUSTOM")
                    arduinocommande = "RF ADR " & Objet.Adresse1 & " " & Objet.Adresse2
                Case "1WIRE"
                    WriteLog("le 1-wire n'est pas encore géré :" & Objet.Name)
                    Exit Sub
                Case ""
                    WriteLog("ERR: WRITE Pas de protocole d'emission pour " & Objet.Name)
                    Exit Sub
                Case Else
                    WriteLog("ERR: WRITE Protocole non géré : " & Objet.Modele.ToString.ToUpper)
                    Exit Sub
            End Select
            'End If

            'If arduinocommande <> "" Then
            'If _DEBUG Then WriteLog("DBG: WRITE Composant " & Objet.Name & " URL : " & arduinocommande)

            WriteLog("DBG: Commande passée à l arduino : " & arduinocommande)
            serialPortObj.WriteLine(arduinocommande) ', 0, 8)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", ex.Message)
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
                WriteLog("Le driver n'est pas démarré, impossible de communiquer avec l'arduino")
                Exit Sub
            End If
            If _DEBUG Then WriteLog("DBG: WRITE Device " & Objet.Name & " <-- " & Command)

            'verification si adresse1 n'est pas vide
            If String.IsNullOrEmpty(Objet.Adresse1) Or Objet.Adresse1 = "" Then
                WriteLog("ERR: WRITE l'adresse de l'arduino doit etre renseigné (ex: 1 pour un arduino maitre, 2-255 pour un arduino esclave) : " & Objet.Name)
                Exit Sub
            End If

            Dim arduinocommande As String = ""

            Select Case UCase(Objet.Modele)
                Case "ANALOG_IN"
                    arduinocommande = "RF ADR " & Objet.Adresse1 & " AR " & Objet.Adresse2
                    WriteLog("DBG: Commande passée à l arduino de type : ANALOG_IN")
                Case "DIGITAL_IN"
                    WriteLog("DBG: Commande passée à l arduino de type : DIGITAL_IN")
                    arduinocommande = "RF ADR " & Objet.Adresse1 & " DR " & Objet.Adresse2
                Case "DIGITAL_OUT"
                    'Digital Write
                    WriteLog("DBG: Commande passée à l arduino de type : DIGITAL_OUT")
                    Select Case Command
                        Case "ON" : arduinocommande = "RF ADR " & Objet.Adresse1 & " DW " & Objet.Adresse2 & " 1"
                        Case "OFF" : arduinocommande = "RF ADR " & Objet.Adresse1 & " DW " & Objet.Adresse2 & " 0"
                        Case Else
                            WriteLog("ERR: Send AC : Commande invalide : " & Command & " (ON/OFF supporté sur une SORTIE: digital write)")
                            Exit Sub
                    End Select
                Case "DIGITAL_PWM"
                    'Analogique write (0-255)
                    'on convertit ON/OFF/DIM en DIM de 0 à 255 (commande PWM sur l'arduino)
                    WriteLog("DBG: Commande passée à l arduino de type : DIGITAL_PWM")
                    Select Case Command
                        Case "ON" : arduinocommande = "RF ADR " & Objet.Adresse1 & " PWM " & Objet.Adresse2 & " 255"
                        Case "OFF" : arduinocommande = "RF ADR " & Objet.Adresse1 & " PWM " & Objet.Adresse2 & " 0"
                        Case "DIM"
                            If Not IsNothing(Parametre1) Then
                                If IsNumeric(Parametre1) Then
                                    'Conversion du parametre de % (0 à 100) en 0 à 255
                                    Parametre1 = CInt(Parametre1 * 255 / 100)
                                    arduinocommande = "RF ADR " & Objet.Adresse1 & " DW " & Objet.Adresse2 & " " & Parametre1
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
                                    arduinocommande = "RF ADR " & Objet.Adresse1 & " PWM " & Objet.Adresse1 & " " & Parametre1
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
                Case ""
                    WriteLog("ERR: WRITE Pas de protocole d'emission pour " & Objet.Name)
                    Exit Sub
                Case Else
                    WriteLog("ERR: WRITE Protocole non géré : " & Objet.Modele.ToString.ToUpper)
                    Exit Sub
            End Select

            WriteLog("DBG: Commande passée à l arduino : " & arduinocommande)
            serialPortObj.WriteLine(arduinocommande) ', 0, 8)

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

            'ajout des commandes avancées pour les devices
            'add_devicecommande("COMMANDE", "DESCRIPTION", nbparametre)
            'add_devicecommande("CONFIG_TYPE_PIN", "configurer le type de PIN sur l arduino suivant les propriétés du composant", 0)
            'add_devicecommande("PWM", "Envoyer une commande PWM avec une valeur de 0 à 255", 1)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Adresse Arduino", "Adresse de l arduino gérant ce composant (0:Arduino maitre ou 1-255: Arduino esclave)")
            Add_LibelleDevice("ADRESSE2", "Numéro du PIN ou de périphérique connecté à l'arduino", "Valeur de type numérique")
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("MODELE", "TYPE PIN/MODELE", "Type de PIN : ANALOG_IN(Analogique Read)/DIGITAL_IN(Digital Read)/DIGITAL_OUT(Digital write: ON/OFF)/DIGITAL_PWM(Analogique write: 0-255) ou de Modèle : 1WIRE", "ANALOG_IN|DIGITAL_IN|DIGITAL_OUT|DIGITAL_PWM|1WIRE|DHTXX|BMP180|RTC")
            Add_LibelleDevice("REFRESH", "@", "")
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
            If first Then
                first = False
            Else
                'on attend d'avoir le reste
                System.Threading.Thread.Sleep(500)

                Dim line As String = serialPortObj.ReadExisting
                line = line.Replace(vbCr, "").Replace(vbLf, "")
                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " DataReceived", "Données reçues: " & line)
                Dim aryLine() As String
                aryLine = line.Split(" ")
                If UBound(aryLine) >= 5 Then
                    Dim Adresse1 As String = aryLine(2)
                    Dim Commande As String = aryLine(3)
                    Dim Adresse2 As String = aryLine(4)
                    Dim Valeur As String = aryLine(5)
                    Select Case aryLine(3)
                        Case "AW"
                            traitement("", Adresse1, Adresse2, Valeur)
                            '_Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Datareceived", "Ack:" & Commande & " Commande executée")
                        Case "AR"
                            traitement("", Adresse1, Adresse2, Valeur)
                        Case "DW"
                            traitement("", Adresse1, Adresse2, Valeur)
                            '_Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Datareceived", "Ack:" & Commande & " Commande executée")
                        Case "PWM"
                            traitement("", Adresse1, Adresse2, Valeur)
                            ' _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Datareceived", "Ack:" & Commande & " Commande executée")
                        Case "DR"
                            traitement("", Adresse1, Adresse2, Valeur)
                        Case "DHTXX"
                            traitement("", Adresse1, Adresse2, Valeur)
                        Case "BMP180"
                            traitement("", Adresse1, Adresse2, Valeur)
                        Case Else
                            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Datareceived", "Erreur:" & Commande & " Commande inconnue !")
                    End Select
                End If
            End If

        Catch Ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Datareceived", "Erreur:" & Ex.ToString)
        End Try
    End Sub

    ''' <summary>Traite les paquets reçus</summary>
    ''' <remarks></remarks>
    Private Sub traitement(ByVal type As String, ByVal adresse As String, ByVal adresse2 As String, ByVal valeur As String)
        '    Private Sub traitement(ByVal adresse As String, ByVal valeur As String)
        Try
            'correction valeur
            valeur = Regex.Replace(valeur, "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)

            'Recherche si un device affecté
            Dim listedevices As New ArrayList

            listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_idsrv, adresse, type, Me._ID, True)
            'un device trouvé on maj la value
            If (listedevices.Count = 1) Then
                listedevices.Item(0).Value = valeur
            ElseIf (listedevices.Count > 1) Then
                For i As Integer = 0 To listedevices.Count - 1
                    If (listedevices.Item(i).adresse2.ToUpper() = adresse2.ToUpper()) Then
                        listedevices.Item(i).Value = valeur
                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Reception : ", "Arduino N° " & adresse & " Pin " & adresse2 & " Valeur " & valeur)
                    End If
                Next
                '_Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Process", "Plusieurs devices correspondent à : " & adresse)
                ' Else
                ' 'si autodiscover = true ou modedecouverte du serveur actif alors on crée le composant sinon on logue
                ' If _AutoDiscover Or _Server.GetModeDecouverte Then
                ' _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Process", "Device non trouvé, AutoCreation du composant : " & type & " " & adresse & ":" & valeur)
                ' _Server.AddDetectNewDevice(adresse, _ID, type, "", valeur)
                ' Else
                ' _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Process", "Device non trouvé : " & type & " " & adresse & ":" & valeur)
                ' End If
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " traitement", "Exception : " & ex.Message & " --> " & adresse & " : " & valeur)
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

