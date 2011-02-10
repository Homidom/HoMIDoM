Imports HoMIDom
Imports HoMIDom.HoMIDom.Device
Imports HoMIDom.HoMIDom.Server
Imports UsbLibrary

' Driver RFID mir:ror
' Nécessite la dll usblibrary
' Auteur : Seb
' Date : 10/02/2011

Public Class Driver_RFID
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variable Driver"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "30E229A2-34F1-11E0-BDFE-9FD3DED72085"
    Dim _Nom As String = "RFID"
    Dim _Enable As String = False
    Dim _Description As String = "Récepteur Rfid mir:ror"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "RFID"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = ""
    Dim _Port_TCP As String = ""
    Dim _IP_UDP As String = ""
    Dim _Port_UDP As String = ""
    Dim _Com As String = ""
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "RFID"
    Dim _Version As String = "1.0"
    Dim _Picture As String = "rfid.png"
    Dim _Server As HoMIDom.HoMIDom.Server
    Dim _Device As HoMIDom.HoMIDom.Device
    Dim _DeviceSupport As New ArrayList
    Dim MyTimer As New Timers.Timer

    'A ajouter dans les ppt du driver
    Dim _tempsentrereponse As Integer = 1500
    Dim _ignoreadresse As Boolean = False
    Dim _lastetat As Boolean = True
#End Region

#Region "Déclaration#"
    'variables propres à ce driver
    Dim usb1 As UsbLibrary.UsbHidPort
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

    Public Sub Read(ByVal Objet As Object) Implements HoMIDom.HoMIDom.IDriver.Read

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

    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        Try
            usb1 = New UsbLibrary.UsbHidPort
            AddHandler usb1.OnSpecifiedDeviceRemoved, AddressOf usb_OnSpecifiedDeviceRemoved
            AddHandler usb1.OnDeviceArrived, AddressOf usb_OnDeviceArrived
            AddHandler usb1.OnDeviceRemoved, AddressOf usb_OnDeviceRemoved
            AddHandler usb1.OnDataRecieved, AddressOf usb_OnDataRecieved
            AddHandler usb1.OnSpecifiedDeviceArrived, AddressOf usb_OnSpecifiedDeviceArrived
            Me.usb1.ProductId = Int32.Parse("1301", System.Globalization.NumberStyles.HexNumber)
            Me.usb1.VendorId = Int32.Parse("1DA8", System.Globalization.NumberStyles.HexNumber)
            Me.usb1.CheckDevicePresent()
            _IsConnect = True
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFID", "Driver démarré")
        Catch ex As Exception
            _IsConnect = False
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "RFID", "Driver erreur lors du démarrage: " & ex.Message)
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
        usb1 = Nothing
        _IsConnect = False
        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "RFID", "Driver arrêté")
    End Sub

    Public ReadOnly Property Version() As String Implements HoMIDom.HoMIDom.IDriver.Version
        Get
            Return _Version
        End Get
    End Property

    Public Sub Write(ByVal Objet As Object, ByVal Commande As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write

    End Sub

    Public Sub New()
        _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN)
        _DeviceSupport.Add(ListeDevices.SWITCH)
    End Sub
#End Region

#Region "Fonctions propres au driver"
    Private Sub usb_OnDeviceArrived(ByVal sender As Object, ByVal e As EventArgs)
        'MsgBox("1")
    End Sub

    Private Sub usb_OnDeviceRemoved(ByVal sender As Object, ByVal e As EventArgs)
        'MsgBox("Device was removed")
    End Sub

    Private Sub usb_OnSpecifiedDeviceArrived(ByVal sender As Object, ByVal e As EventArgs)
        _IsConnect = True
    End Sub

    Private Sub usb_OnSpecifiedDeviceRemoved(ByVal sender As Object, ByVal e As EventArgs)
        'MsgBox("Device déconecté")
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
                RaiseEvent DriverEvent(_Nom, 38, "2|Remise à l'endroit du mir:ror")
            ElseIf mirrorData(2) = 5 Then
                ' mise à l'envers 
                RaiseEvent DriverEvent(_Nom, 38, "3|Retournement du mir:ror")
            End If
        ElseIf mirrorData(1) = 2 Then
            'action ztamp 
            Dim idZtamp As String = [String].Empty
            For i As Integer = 3 To 13
                idZtamp += mirrorData(i).ToString("X2")
            Next
            If mirrorData(2) = 1 Then
                'dépot 
                RaiseEvent DriverEvent(_Nom, 38, "1|" & idZtamp)
                'MsgBox("ID:" & idZtamp & " - POSE")
            ElseIf mirrorData(2) = 2 Then
                ' retrait 
                RaiseEvent DriverEvent(_Nom, 38, "0|" & idZtamp)
                'MsgBox("ID:" & idZtamp & " - DEPOSE")
            End If
        Else
            'Log.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Driver " & _Nom & " une erreur inconnue est survenue...")
        End If
    End Sub
#End Region
End Class
