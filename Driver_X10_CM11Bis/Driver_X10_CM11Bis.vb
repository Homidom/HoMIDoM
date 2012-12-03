Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports System.IO.Ports
Imports System.Math
Imports System.Net.Sockets
Imports System.Threading
Imports System.Globalization
Imports DwellNet

Public Class Driver_X10_CM11Bis
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "BC3A3E6C-0944-11E2-B91F-3B9B6188709B"
    Dim _Nom As String = "X10 CM11A"
    Dim _Enable As String = False
    Dim _Description As String = "Driver X10 CM11A version B"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "COM"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = ""
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "CM11"
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
#End Region

#Region "Variables Internes"
    Dim cm11 As DwellNet.Cm11 = New DwellNet.Cm11
#End Region

#Region "Déclaration"
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
                    If String.IsNullOrEmpty(Value) = True Then
                        retour = "l'adresse du module est obligatoire"
                    ElseIf Len(Value) < 2 Then
                        retour = "l'adresse doit à minima comporter une lettre (House) et un chiffre (Code)"
                    ElseIf IsNumeric(Mid(Value, 1, 1)) Then
                        retour = "l'adresse doit commencer par une lettre (House), ex: A"
                    ElseIf Value < "A" Or Value > "P" Then
                        retour = "l'adresse House doit être compris entre Ax et Px (x numéro de Code)"
                    ElseIf IsNumeric(Mid(Value, 2, Len(Value) - 1)) = False Then
                        retour = "l'adresse doit être assciée au House puis le Code qui doit être compris entre 1 et 16, ex: C3"
                    ElseIf CInt(Mid(Value, 2, Len(Value) - 1)) < 1 Or CInt(Mid(Value, 2, Len(Value) - 1)) > 16 Then
                        retour = "l'adresse doit être assciée au House puis le Code qui doit être compris entre 1 et 16, ex: C3"
                    End If
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

                If String.IsNullOrEmpty(_Com) = True Then
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 Start", "Le port COM est vide veuillez le renseigner")
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
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 Start", "Le port COM " & _Com & " n'existe pas, seuls les ports " & _ports & " existe(s)!")
                    Exit Sub
                End If

                cm11.Close()
                cm11.Open(_Com)

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
                cm11.Clear()
                cm11.Close()
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
            If _Enable = False Then
                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Write", "Impossible d'exécuter la commande car le driver n'est pas activé (Enable)")
                Exit Sub
            End If
            If _IsConnect = False Then
                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Write", "Impossible d'exécuter la commande car le driver n'est pas démarré")
                Exit Sub
            End If

            Select Case UCase(Command)
                Case "ON"
                    cm11.Execute(Objet.Adresse1 & " On")
                    Objet.value = 100
                Case "OFF"
                    cm11.Execute(Objet.Adresse1 & " Off")
                    Objet.value = 0
                Case "DIM"
                    If Parametre1 IsNot Nothing Then
                        Dim x As Integer = Parametre1
                        If x <= 0 Then
                            cm11.Execute(Objet.Adresse1 & " Off")
                        End If
                        If x >= 100 Then
                            cm11.Execute(Objet.Adresse1 & " On")
                        End If
                        If 0 < x < 100 Then
                            cm11.Execute(Objet.Adresse1 & " Dim" & x)
                        End If
                        'ecrire(Objet.Adresse1, "EXTENDED_CODE", x)
                        Objet.value = x
                    End If
                Case "OUVERTURE"
                    If Parametre1 IsNot Nothing Then
                        Dim x As Integer = Parametre1
                        If x <= 0 Then
                            cm11.Execute(Objet.Adresse1 & " Off")
                        End If
                        If x >= 100 Then
                            cm11.Execute(Objet.Adresse1 & " On")
                        End If
                        If 0 < x < 100 Then
                            If Objet.value > Parametre1 Then
                                cm11.Execute(Objet.Adresse1 & " Dim" & x)
                            Else
                                cm11.Execute(Objet.Adresse1 & " Brighten" & x)
                            End If

                        End If
                        'ecrire(Objet.adresse1, "EXTENDED_CODE", x)
                        Objet.value = x
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

    ''' <summary>Creation d'un objet de type</summary>
    ''' <remarks></remarks>
    Public Sub New()
        Try
            _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString

            'cm11.InvokeEventsUsing = Me
            AddHandler cm11.AllOffReceived, New DwellNet.Cm11HouseNotificationEventDelegate(AddressOf cm11_AllOffReceived)
            AddHandler cm11.OnReceived, New DwellNet.Cm11DeviceNotificationEventDelegate(AddressOf cm11_OnReceived)
            'AddHandler cm11.LogMessage, New DwellNet.Cm11LogMessageEventDelegate(AddressOf cm11_LogMessage)
            AddHandler cm11.AllLightsOffReceived, New DwellNet.Cm11HouseNotificationEventDelegate(AddressOf cm11_AllLightsOffReceived)
            AddHandler cm11.AllLightsOnReceived, New DwellNet.Cm11HouseNotificationEventDelegate(AddressOf cm11_AllLightsOnReceived)
            AddHandler cm11.BrightenReceived, New DwellNet.Cm11BrightenOrDimNotificationEventDelegate(AddressOf cm11_BrightenReceived)
            AddHandler cm11.IdleStateChange, New DwellNet.Cm11IdleStateChangeEventDelegate(AddressOf cm11_IdleStateChange)
            AddHandler cm11.Error, New DwellNet.Cm11ErrorEventDelegate(AddressOf cm11_Error)
            AddHandler cm11.Notification, New DwellNet.Cm11LowLevelNotificationEventDelegate(AddressOf cm11_LowLevelNotification)
            AddHandler cm11.OffReceived, New DwellNet.Cm11DeviceNotificationEventDelegate(AddressOf cm11_OffReceived)
            AddHandler cm11.DimReceived, New DwellNet.Cm11BrightenOrDimNotificationEventDelegate(AddressOf cm11_DimReceived)

            'liste des devices compatibles
            _DeviceSupport.Add(ListeDevices.APPAREIL.ToString)
            _DeviceSupport.Add(ListeDevices.CONTACT.ToString)
            _DeviceSupport.Add(ListeDevices.DETECTEUR.ToString)
            _DeviceSupport.Add(ListeDevices.LAMPE.ToString)
            _DeviceSupport.Add(ListeDevices.SWITCH.ToString)
            _DeviceSupport.Add(ListeDevices.VOLET.ToString)

            'ajout des commandes avancées pour les devices
            'add_devicecommande("COMMANDE", "DESCRIPTION", 0)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Adresse du module", "Adresse HouseCode du module (ex: C3)")
            Add_LibelleDevice("ADRESSE2", "@", "")
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("MODELE", "@", "")
            Add_LibelleDevice("REFRESH", "Refresh", "")
            Add_LibelleDevice("LASTCHANGEDUREE", "LastChange Durée", "")
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 New", ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick()

    End Sub

#End Region

#Region "Fonctions internes"

    ''' <summary>
    ''' Called when an "On" command is transmitted by a controller or device
    ''' on the X10 network.
    ''' </summary>
    '''
    Private Sub cm11_OnReceived(ByVal address As String)
        traitement("ON", address)
    End Sub


    ''' <summary>
    ''' Called when an "Off" command is transmitted by a controller or device
    ''' on the X10 network.
    ''' </summary>
    Private Sub cm11_OffReceived(ByVal address As String)
        traitement("OFF", address)
    End Sub

    ''' <summary>
    ''' Called when a "Brighten" command is transmitted by a controller or
    ''' device on the X10 network.
    ''' </summary>
    Private Sub cm11_BrightenReceived(ByVal address As String, ByVal percent As Integer)
        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " cm11_BrightenReceived", "address:" & address & " percent:" & percent)
    End Sub

    ''' <summary>
    ''' Called when a "Dim" command is transmitted by a controller or device
    ''' on the X10 network.
    ''' </summary>
    Private Sub cm11_DimReceived(ByVal address As String, ByVal percent As Integer)
        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " cm11_DimReceived", "address:" & address & " percent:" & percent)
        traitement("DIM", address, percent)
    End Sub

    ''' <summary>
    ''' Called when an "AllLightsOn" command is transmitted by a controller or
    ''' device on the X10 network.
    ''' </summary>
    Private Sub cm11_AllLightsOnReceived(ByVal houseCode As Char)
        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " cm11_AllLightsOnReceived", "houseCode:" & houseCode)
    End Sub

    ''' <summary>
    ''' Called when an "AllLightsOff" command is transmitted by a controller
    ''' or device on the X10 network.
    ''' </summary>
    Private Sub cm11_AllLightsOffReceived(ByVal houseCode As Char)
        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " cm11_AllLightsOffReceived", "houseCode:" & houseCode)
    End Sub


    ''' <summary>
    ''' Called when an "AllOff" command is transmitted by a controller or
    ''' device on the X10 network.
    ''' </summary>
    Private Sub cm11_AllOffReceived(ByVal houseCode As Char)
        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " cm11_AllOffReceived", "houseCode:" & houseCode)
    End Sub

    ''' <summary>
    ''' Called when a notification of an event on the X10 network is received
    ''' from the CM11 hardware.
    ''' </summary>
    '''
    Private Sub cm11_LowLevelNotification(ByVal commandName As String, ByVal commandParameter As Integer)
        Dim command As String = commandName
        If commandParameter >= 0 Then
            command = [String].Format("{0}{1}", command, commandParameter)
        End If
        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " cm11_LowLevelNotification", "commandName:" & commandName & " commandParameter:" & commandParameter)
    End Sub

    ''' <summary>
    ''' Called when the <r>Cm11</r> object changes from processing commands to
    ''' being idle, or vice versa.
    ''' </summary>
    '''
    Private Sub cm11_IdleStateChange(ByVal idle As Boolean)
        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " cm11_IdleStateChange", "Idle:" & idle)
    End Sub

    ''' <summary>
    ''' Called when communication with the CM11 hardware fails, or the hardware
    ''' itself fails.
    ''' </summary>
    '''
    Private Sub cm11_Error(ByVal message As String)
        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 ErrorReceived", "Error: " & message)
    End Sub



    Private Sub m_serialPort_ErrorReceived(ByVal sender As Object, ByVal e As SerialErrorReceivedEventArgs)
        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 ErrorReceived", "Error: " & e.ToString)
    End Sub

    ''' <summary>Traite les paquets reçus</summary>
    ''' <remarks></remarks>
    Private Sub traitement(ByVal valeur As String, ByVal adresse As String, Optional ByVal data As Integer = 0)
        valeur = UCase(valeur)
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
                        ElseIf valeur = "DIM" Then
                            valeur = data
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

#End Region

End Class
