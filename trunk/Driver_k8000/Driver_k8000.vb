Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device


''' <summary>Driver Velleman k8055, le device doit dans son adresse 1 indiqué sa carte et son numéro de relais séparé par un x, exemple pour le relais 1 de la carte 1: 1x1</summary>
''' <remarks>Nécessite la dll k8055d.dll</remarks>
<Serializable()> Public Class Driver_k8000
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variable Driver"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "BC12622C-6EE6-11E2-8832-2F1A6188709B"
    Dim _Nom As String = "K8000"
    Dim _Enable As Boolean = False
    Dim _Description As String = "Carte Velleman k8000"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "LPT"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "@"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "K8055"
    Dim _Version As String = My.Application.Info.Version.ToString
    Dim _OsPlatform As String = "32"
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

    'A ajouter dans les ppt du driver
    Dim _tempsentrereponse As Integer = 1500
    Dim _ignoreadresse As Boolean = False
    Dim _lastetat As Boolean = True
#End Region

#Region "Declaration"
    Private Declare Sub ConfigAllIOasInput Lib "k8d.dll" ()
    Private Declare Sub ConfigAllIOasOutput Lib "k8d.dll" ()
    Private Declare Sub ConfigIOchipAsInput Lib "k8d.dll" (ByVal Chip_no As Integer)
    Private Declare Sub ConfigIOchipAsOutput Lib "k8d.dll" (ByVal Chip_no As Integer)
    Private Declare Sub ConfigIOchannelAsInput Lib "k8d.dll" (ByVal Channel_no As Integer)
    Private Declare Sub ConfigIOchannelAsOutput Lib "k8d.dll" (ByVal Channel_no As Integer)

   'OUTPUT PROCEDURES

    Private Declare Sub IOoutput Lib "k8d.dll" (ByVal Chip_no As Integer, ByVal Data As Integer)
    Private Declare Sub UpdateAllIO Lib "k8d.dll" ()
    Private Declare Sub ClearAllIO Lib "k8d.dll" ()
    Private Declare Sub SetAllIO Lib "k8d.dll" ()
    Private Declare Sub UpdateIOchip Lib "k8d.dll" (ByVal Chip_no As Integer)
    Private Declare Sub ClearIOchip Lib "k8d.dll" (ByVal Chip_no As Integer)
    Private Declare Sub SetIOchip Lib "k8d.dll" (ByVal Chip_no As Integer)
    Private Declare Sub SetIOchannel Lib "k8d.dll" (ByVal Channel_no As Integer)
    Private Declare Sub ClearIOchannel Lib "k8d.dll" (ByVal Channel_no As Integer)

    'INPUT FUNCTIONS AND PROCEDURES
    Private Declare Function ReadIOchip Lib "k8d.dll" (ByVal Chip_no As Integer) As Integer
    Private Declare Function ReadIOchannel Lib "k8d.dll" (ByVal Channel_no As Integer) As Boolean
    Private Declare Sub ReadIOconficArray Lib "k8d.dll" (ByVal Array_Pointer As Integer)
    Private Declare Sub ReadIOdataArray Lib "k8d.dll" (ByVal Array_Pointer As Integer)
    Private Declare Sub ReadDACarray Lib "k8d.dll" (ByVal Array_Pointer As Integer)
    Private Declare Sub ReadDAarray Lib "k8d.dll" (ByVal Array_Pointer As Integer)

    'How to use these calls:

    ' ReadIOconficArray IOconfig(0)

    ' ReadIOdataArray IOdata(0)

    ' ReadDACarray DAC(1)

    ' ReadDAarray DA(1)



    '6 BIT DAC CONVERTER PROCEDURES

    Private Declare Sub OutputDACchannel Lib "k8d.dll" (ByVal Channel_no As Integer, ByVal Data As Integer)
    Private Declare Sub ClearDACchannel Lib "k8d.dll" (ByVal Channel_no As Integer)
    Private Declare Sub SetDACchannel Lib "k8d.dll" (ByVal Channel_no As Integer)
    Private Declare Sub ClearDACchip Lib "k8d.dll" (ByVal Chip_no As Integer)
    Private Declare Sub SetDACchip Lib "k8d.dll" (ByVal Chip_no As Integer)
    Private Declare Sub ClearAllDAC Lib "k8d.dll" ()
    Private Declare Sub SetAllDAC Lib "k8d.dll" ()



    '8 BIT DA CONVERTER PROCEDURES
    Private Declare Sub OutputDAchannel Lib "k8d.dll" (ByVal Channel_no As Integer, ByVal Data As Integer)
    Private Declare Sub ClearDAchannel Lib "k8d.dll" (ByVal Channel_no As Integer)
    Private Declare Sub SetDAchannel Lib "k8d.dll" (ByVal Channel_no As Integer)
    Private Declare Sub ClearAllDA Lib "k8d.dll" ()
    Private Declare Sub SetAllDA Lib "k8d.dll" ()

    '8 BIT AD CONVERTER FUNCTION
    Private Declare Function ReadADchannel Lib "k8d.dll" (ByVal Channel_no As Integer) As Integer

    'GENERAL PROCEDURES
    Private Declare Sub SelectI2CprinterPort Lib "k8d.dll" (ByVal port As Integer)
    Private Declare Sub Start_K8000 Lib "k8d.dll" ()
    Private Declare Sub Stop_K8000 Lib "k8d.dll" ()

    'COMMON USED GLOBALS
    Const MaxIOcard As Integer = 3
    Const MaxIOchip As Integer = 7
    Const MaxDACchannel As Integer = 32
    Const MaxDAchannel As Integer = 4

    'Declare variables
    Dim IOconfig(MaxIOchip) As Integer
    Dim IOdata(MaxIOchip) As Integer
    Dim DAC(MaxDACchannel) As Integer
    Dim DA(MaxDAchannel) As Integer

    Dim _carte(3) As Boolean
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

    Public Sub Read(ByVal Objet As Object) Implements HoMIDom.HoMIDom.IDriver.Read
        Try
            If _Enable = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Read", "Erreur: Impossible de traiter la commande car le driver n'est pas activé (Enable)")
                Exit Sub
            End If

            If _IsConnect = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Read", "Erreur: Impossible de traiter la commande car le driver n'est pas connecté à la carte")
                Exit Sub
            End If

            'Dim _IsAna As Boolean = False

            'If IsNumeric(Objet.Adresse1) = False Then
            '    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Read", "Erreur: l'adresse du device (Adresse1) " & Objet.Adresse1 & " n'est pas une valeur numérique")
            '    Exit Sub
            'End If
            'Dim adr As Long = Objet.Adresse1

            'If Objet.Type <> "GENERIQUEBOOLEEN" Then
            '    If Objet.Type <> "GENERIQUEVALUE" Then
            '        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Read", "Erreur: Le device doit être du type GENERIQUEBOOLEEN ou GENERIQUEVALUE")
            '        Exit Sub
            '    Else
            '        If adr < 1 Or adr > 2 Then
            '            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Read", "Erreur: l'adresse du device (Adresse1) doit être comprise entre 1 et 2 pour une entree analogique")
            '            Exit Sub
            '        Else
            '            _IsAna = True
            '        End If
            '    End If
            'Else
            '    If adr < 1 Or adr > 5 Then
            '        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Read", "Erreur: l'adresse du device (Adresse1) doit être comprise entre 1 et 5 pour une entree binaire")
            '        Exit Sub
            '    End If
            'End If

            'If IsNumeric(Objet.Adresse2) = False Then
            '    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Read", "Erreur: l'adresse de la carte (Adresse2) " & Objet.Adresse2 & " n'est pas une valeur numérique")
            '    Exit Sub
            'End If
            'If CInt(Objet.Adresse2) < 0 Or CInt(Objet.Adresse2) > 3 Then
            '    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Read", "Erreur: l'adresse de la carte (Adresse2) doit être comprise entre 0 et 3")
            '    Exit Sub
            'End If

            'Dim _numcarte As Integer = SetCurrentDevice(CInt(Objet.Adresse2))
            'If _numcarte <> -1 Then
            '    If _numcarte <> CInt(Objet.Adresse2) Then
            '        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Read", "Erreur: impossible de changer de carte N°: " & Objet.Adresse2)
            '        Exit Sub
            '    End If
            '    If _IsAna = True Then
            '        Dim val As Integer = ReadAnalogChannel(Objet.Adresse1)
            '        Objet.Value = val
            '        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "K8055 Read", "Device analogique adresse:" & Objet.Adresse1 & " carte: " & Objet.Adresse2 & " Valeur: " & val)
            '    Else
            '        Dim Val As Boolean = ReadDigitalChannel(Objet.Adresse1)
            '        Objet.Value = Val
            '        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "K8055 Read", "Device binaire adresse:" & Objet.Adresse1 & " carte: " & Objet.Adresse2 & " Valeur: " & Val)
            '    End If
            'Else
            '    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Read", "Erreur: l'adresse de la carte: " & Objet.Adresse2 & " n'a pas été trouvée")
            '    Exit Sub
            'End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Read", "Erreur: " & ex.ToString)
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
                    Select Case UCase(Command)
                        Case "VALEUR"
                            'For i As Integer = 0 To Param.Length - 1
                            '    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " ExecuteCommand", "Param (" & i & ") : " & Param(i))
                            'Next
                            Write(MyDevice, Command, Param(0), Param(1))
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
                    If Value = " " Then retour = "l'adresse du device est obligatoire"
                    If IsNumeric(Value) = False Then retour = "l'adresse doit être numérique "
                    If CInt(Value) < 1 Or CInt(Value) > 8 Then retour = "l'adresse doit être numérique et comprise entre 1 et 5 pour une entrée (bianire), 1 et 2 (entrée analogique) et 1 et 8 pour une sortie"
                Case "ADRESSE2"
                    If Value = " " Then retour = "l'adresse de la carte est obligatoire"
                    If IsNumeric(Value) = False Then retour = "l'adresse de la carte doit être numérique et comprise entre 0 et 3"
                    If CInt(Value) < 0 Or CInt(Value) > 3 Then retour = "l'adresse de la carte doit être numérique et comprise entre 0 et 3"
            End Select
            Return retour
        Catch ex As Exception
            Return "Une erreur est apparue lors de la vérification du champ " & Champ & ": " & ex.ToString
        End Try
    End Function

    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        'cree l'objet
        Try
            'Dim k As Integer = 0
            'k = SearchDevices()
            'If k = 0 Then
            '    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Start", "Aucune Carte n'a été trouvée")
            '    _IsConnect = False
            'End If
            'If (k And 1) Then
            '    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "K8055 Start", "Carte 0 connectée")
            '    _IsConnect = True
            '    SetCurrentDevice(0)
            '    _carte(0) = True
            'End If
            'If (k And 2) Then
            '    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "K8055 Start", "Carte 1 connectée")
            '    If Not _IsConnect Then
            '        _IsConnect = True
            '        SetCurrentDevice(1)
            '        _carte(1) = True
            '    End If
            'End If
            'If (k And 4) Then
            '    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "K8055 Start", "Carte 2 connectée")
            '    If Not _IsConnect Then
            '        _IsConnect = True
            '        SetCurrentDevice(2)
            '        _carte(2) = True
            '    End If
            'End If
            'If (k And 8) Then
            '    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "K8055 Start", "Carte 3 connectée")
            '    If Not _IsConnect Then
            '        _IsConnect = True
            '        SetCurrentDevice(3)
            '        _carte(3) = True
            '    End If
            'End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Start", "Erreur lors du démarrage du driver: " & ex.ToString)
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

    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        'cree l'objet
        Try
            For i As Integer = 0 To 3
                If _carte(i) = True Then
                    'SetCurrentDevice(i)
                    'CloseDevice()
                    _carte(i) = False
                End If
            Next
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "K8055 Stop", "Driver arrêté")
            _IsConnect = False
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Stop", "Erreur lors de l'arrêt du driver: " & ex.ToString)
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

    Public Sub Write(ByVal Objet As Object, ByVal Commande As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
        Try
            If _Enable = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Write", "Erreur: Impossible de traiter la commande car le driver n'est pas activé (Enable)")
                Exit Sub
            End If

            If _IsConnect = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Write", "Erreur: Impossible de traiter la commande car le driver n'est pas connecté à la carte")
                Exit Sub
            End If

            'Dim _IsAna As Boolean = False

            'If IsNumeric(Objet.Adresse1) = False Then
            '    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Write", "Erreur: l'adresse du device (Adresse1) " & Objet.Adresse1 & " n'est pas une valeur numérique")
            '    Exit Sub
            'End If
            'Dim adr As Long = Objet.Adresse1

            'If Objet.Type = "SWITCH" Or Objet.Type = "APPAREIL" Then
            '    If adr < 1 Or adr > 8 Then
            '        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Write", "Erreur: l'adresse du device (Adresse1) doit être comprise entre 1 et 8 pour une sortie binaire")
            '        Exit Sub
            '    End If
            'Else
            '    If Objet.Type = "GENERIQUEVALUE" Then
            '        If adr < 1 Or adr > 2 Then
            '            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Write", "Erreur: l'adresse du device (Adresse1) doit être comprise entre 1 et 2 pour une sortie analogique")
            '            Exit Sub
            '        Else
            '            _IsAna = True
            '        End If
            '    Else
            '        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Write", "Erreur: le type du device " & Objet.Type & " n'est pas reconnu pour ce driver")
            '        Exit Sub
            '    End If
            'End If

            'If IsNumeric(Objet.Adresse2) = False Then
            '    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Write", "Erreur: l'adresse de la carte (Adresse2) " & Objet.Adresse2 & " n'est pas une valeur numérique")
            '    Exit Sub
            'End If
            'If CInt(Objet.Adresse2) < 0 Or CInt(Objet.Adresse2) > 3 Then
            '    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Write", "Erreur: l'adresse de la carte (Adresse2) doit être comprise entre 0 et 3")
            '    Exit Sub
            'End If

            'SetCurrentDevice(CInt(Objet.Adresse2))
            'If SetCurrentDevice(CInt(Objet.Adresse2)) < 0 Then
            '    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Write", "Erreur: l'adresse de la carte: " & Objet.Adresse2 & " n'a pas été trouvée")
            '    Exit Sub
            'End If

            'If _IsAna = False Then
            '    If Commande = "ON" Then
            '        SetDigitalChannel(adr)
            '        Objet.Value = True
            '    End If
            '    If Commande = "OFF" Then
            '        ClearDigitalChannel(adr)
            '        Objet.Value = False
            '    End If
            'Else
            '    If Commande = "VALEUR" Then
            '        If Parametre1 IsNot Nothing Then
            '            If IsNumeric(Parametre1) Then
            '                OutputAnalogChannel(adr, Parametre1)
            '                Objet.Value = Parametre1
            '                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "K8055 Write", "Commande VALEUR, valeur envoyée: " & Parametre1 & " sur l'adresse: " & adr)
            '            Else
            '                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "K8055 Write", "Erreur: Commande VALEUR, le paramètre n'est pas numérique: " & Parametre1)
            '            End If
            '        Else
            '            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "K8055 Write", "Erreur: Commande VALEUR, le paramètre est vide")
            '        End If
            '    End If
            'End If

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Write", "Erreur: " & ex.ToString)
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
            x.DescriptionCommand = Description
            x.CountParam = NbParam
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

    Public Sub New()
        _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString

        _DeviceSupport.Add(ListeDevices.SWITCH) 'SORTIE
        _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN) 'ENTREE
        _DeviceSupport.Add(ListeDevices.APPAREIL) 'SORTIE
        _DeviceSupport.Add(ListeDevices.GENERIQUEVALUE) 'E/S ANA (Adresse Ex ou Sx)

        'ajout des commandes avancées pour les devices
        Add_DeviceCommande("VALEUR", "Valeur à écrire", 1)

        Add_LibelleDevice("ADRESSE1", "Adresse du device", "Doit être compris entre 1 et 5 pour une entrée et 1 et 8 pour une sortie")
        Add_LibelleDevice("ADRESSE2", "Adresse de la carte", "Adresse de la carte qui doit être compris entre 0 et 3")
        Add_LibelleDevice("SOLO", "@", "")
        Add_LibelleDevice("MODELE", "@", "")
        'Add_LibelleDevice("REFRESH", "Refresh (sec)", "Valeur de rafraîchissement de la mesure en secondes")
        'Add_LibelleDevice("LASTCHANGEDUREE", "LastChange Durée", "")
    End Sub
#End Region

#Region "Fonctions propres au driver"


#End Region

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub


End Class
