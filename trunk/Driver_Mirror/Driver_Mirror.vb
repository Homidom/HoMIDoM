Imports HoMIDom
Imports HoMIDom.HoMIDom.Device
Imports HoMIDom.HoMIDom.Server
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Threading
Imports UsbLibrary

' Auteur : Seb
' Date : 10/02/2011

''' <summary>Class Driver_Mirror, permet de recevoir des infos du Mir:ror</summary>
''' <remarks>Nécessite la dll usblibrary</remarks>
<Serializable()> Public Class Driver_Mirror
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "30E229A2-34F1-11E0-BDFE-9FD3DED72085"
    Dim _Nom As String = "RFID"
    Dim _Enable As String = False
    Dim _Description As String = "Récepteur Rfid mir:ror"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "RFID"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "@"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "RFID"
    Dim _Version As String = "1.0"
    Dim _Picture As String = "rfid.png"
    Dim _Server As HoMIDom.HoMIDom.Server
    Dim _Device As HoMIDom.HoMIDom.Device
    Dim _DeviceSupport As New ArrayList
    Dim _Parametres As New ArrayList
    Dim MyTimer As New Timers.Timer
    Dim _IdSrv As String
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande)

    'A ajouter dans les ppt du driver
    Dim _tempsentrereponse As Integer = 1500
    Dim _ignoreadresse As Boolean = False
    Dim _lastetat As Boolean = True
#End Region

#Region "Variables internes"
    Dim WithEvents usb1 As UsbLibrary.UsbHidPort
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
    Public Property StartAuto() As Boolean Implements HoMIDom.HoMIDom.IDriver.StartAuto
        Get
            Return _StartAuto
        End Get
        Set(ByVal value As Boolean)
            _StartAuto = value
        End Set
    End Property
    Public ReadOnly Property Version() As String Implements HoMIDom.HoMIDom.IDriver.Version
        Get
            Return _Version
        End Get
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
            usb1 = New UsbLibrary.UsbHidPort
            AddHandler usb1.OnSpecifiedDeviceRemoved, AddressOf Me.usb_OnSpecifiedDeviceRemoved
            AddHandler usb1.OnDeviceArrived, AddressOf Me.usb_OnDeviceArrived
            AddHandler usb1.OnDeviceRemoved, AddressOf Me.usb_OnDeviceRemoved
            AddHandler usb1.OnDataRecieved, AddressOf Me.usb_OnDataRecieved
            AddHandler usb1.OnSpecifiedDeviceArrived, AddressOf usb_OnSpecifiedDeviceArrived
            Me.usb1.ProductId = Int32.Parse("1301", System.Globalization.NumberStyles.HexNumber)
            Me.usb1.VendorId = Int32.Parse("1DA8", System.Globalization.NumberStyles.HexNumber)
            Me.usb1.CheckDevicePresent()
            If usb1 IsNot Nothing Then
                _IsConnect = True
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Mirror", "Driver démarré:" & IsConnect)
            Else
                _IsConnect = False
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Mirror", "Driver erreur lors du démarrage")
            End If
        Catch ex As Exception
            _IsConnect = False
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Mirror Start", "Driver erreur lors du démarrage: " & ex.Message)
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            usb1 = Nothing
            _IsConnect = False
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Mirror", "Driver arrêté")
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Mirror Stop", ex.Message)
        End Try
    End Sub

    ''' <summary>Re-Démarrer le driver</summary>
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
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Mirror Read", ex.Message)
        End Try
    End Sub

    ''' <summary>Commander un device</summary>
    ''' <param name="Objet">Objet représetant le device à interroger</param>
    ''' <param name="Commande">La commande à passer</param>
    ''' <param name="Parametre1"></param>
    ''' <param name="Parametre2"></param>
    ''' <remarks></remarks>
    Public Sub Write(ByVal Objet As Object, ByVal Commande As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
        Try
            If _Enable = False Then Exit Sub
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Mirror Write", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Mirror DeleteDevice", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice
        Try
            'ajout des commandes avancées pour les devices
            'Ci-dessous un exemple
            'Dim x As New DeviceCommande
            'x.NameCommand = "Test"
            'x.DescriptionCommand = "Ceci est une commande avancée de test"
            'x.CountParam = 1
            '_DeviceCommandPlus.Add(x)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Mirror NewDevice", ex.Message)
        End Try
    End Sub

    ''' <summary>Creation d'un objet de type</summary>
    ''' <remarks></remarks>
    Public Sub New()
        Try
            'liste des devices compatibles
            _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN)
            _DeviceSupport.Add(ListeDevices.SWITCH)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Mirror New", ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick()

    End Sub

#End Region

#Region "Fonctions internes"
    Private Sub usb_OnDeviceArrived(ByVal sender As Object, ByVal e As EventArgs)
        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "Mirror", "usb_OnDeviceArrived")
    End Sub

    Private Sub usb_OnDeviceRemoved(ByVal sender As Object, ByVal e As EventArgs)
        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "Mirror", "usb_OnDeviceRemoved")
    End Sub

    Private Sub usb_OnSpecifiedDeviceArrived(ByVal sender As Object, ByVal e As EventArgs)
        _IsConnect = True
        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Mirror", "Lecteur RFID détecté")
    End Sub

    Private Sub usb_OnSpecifiedDeviceRemoved(ByVal sender As Object, ByVal e As EventArgs)
        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "Mirror", "usb_OnSpecifiedDeviceRemoved")
    End Sub

    Private Sub usb_OnDataRecieved(ByVal sender As Object, ByVal args As DataRecievedEventArgs)
        Dim different0 As Boolean = False
        Dim rec_data As String = "Data: "
        For Each myData As Byte In args.data
            If myData <> 0 Then
                different0 = True
            End If

            rec_data += myData.ToString("X") & " "
        Next

        If different0 Then
            processMirrorData(args.data)
        End If
    End Sub

    Private Sub processMirrorData(ByVal mirrorData As Byte())
        processLaunch(mirrorData)
    End Sub

    Private Sub processLaunch(ByVal mirrorData As Byte())
        If mirrorData(1) = 1 Then
            'action miroir 
            If mirrorData(2) = 4 Then
                'remis à l'endroit 
                'RaiseEvent DriverEvent(_Nom, 38, "2|Remise à l'endroit du mir:ror")
            ElseIf mirrorData(2) = 5 Then
                ' mise à l'envers 
                'RaiseEvent DriverEvent(_Nom, 38, "3|Retournement du mir:ror")
            End If
        ElseIf mirrorData(1) = 2 Then
            'action ztamp 
            Dim idZtamp As String = [String].Empty
            For i As Integer = 3 To 13
                idZtamp += mirrorData(i).ToString("X2")
            Next
            If mirrorData(2) = 1 Then
                'dépot 
                SetDevice(idZtamp, True)
                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "Mirror", "Puce RFID N°: " & idZtamp & " Value=True")
            ElseIf mirrorData(2) = 2 Then
                ' retrait 
                SetDevice(idZtamp, False)
                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "Mirror", "Puce RFID N°: " & idZtamp & " Value=False")
            End If
        Else
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Mirror", "Une erreur inconnue est survenue...")
        End If
    End Sub

    Private Sub SetDevice(ByVal idZtamp As String, ByVal Value As Boolean)
        For i As Integer = 0 To _Server.Devices.Count - 1
            If _Server.Devices.Item(i).Adresse1 = idZtamp And (_Server.Devices.Item(i).type = "GENERIQUEBOOLEEN" Or _Server.Devices.Item(i).type = "SWITCH") Then
                _Server.Devices.Item(i).value = Value
                Exit For
            End If
        Next
    End Sub
#End Region

End Class
