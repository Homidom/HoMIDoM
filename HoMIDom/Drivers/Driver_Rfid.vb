Imports UsbLibrary

Namespace HoMIDom
    '***********************************************
    '** CLASS DRIVER RFID (utilise mir:ror de chez Violet)
    '** version 1.0
    '** Date de création: 14/01/2011
    '** Historique (SebBergues): 14/01/2011: Création 
    '***********************************************

    <Serializable()> Public Class Driver_Rfid
        Inherits Server

        Dim _ID As String
        Dim _Nom As String = "Rfid"
        Dim _Enable As String = False
        Dim _Description As String = "Driver Rfid mir:ror"
        Dim _StartAuto As Boolean = False
        Dim _Protocol As String = "Rfid"
        Dim _IsConnect As Boolean = False
        Dim _IP_TCP As String
        Dim _Port_TCP As String
        Dim _IP_UDP As String
        Dim _Port_UDP As String
        Dim _Com As String
        Dim _Refresh As Integer
        Dim _Modele As String = "mir:ror"
        Dim _Version As String = "1.0"
        Dim _Picture As String
        Dim MyTimer As New Timers.Timer

        Public Event DriverEvent(ByVal DriveName As String, ByVal TypeEvent As String, ByVal Parametre As Object)

        'variables propres à ce driver
        Dim usb1 As UsbLibrary.UsbHidPort

        'Identification unique du driver
        Public Property ID() As String
            Get
                Return _ID
            End Get
            Set(ByVal value As String)
                _ID = value
            End Set
        End Property

        'Libellé de driver (qui sert aussi à l'affichage)
        Public ReadOnly Property Nom() As String
            Get
                Return _Nom
            End Get
        End Property

        'Activation du Driver
        Public Property Enable() As Boolean
            Get
                Return _Enable
            End Get
            Set(ByVal value As Boolean)
                _Enable = value
            End Set
        End Property

        'Description qui peut être le modèle du driver ou autre chose
        Public ReadOnly Property Description() As String
            Get
                Return _Description
            End Get
        End Property

        'True si le driver doit être activé dès le démarrage du serveur ou False s’il doit être activé manuellement
        Public Property StartAuto() As Boolean
            Get
                Return _StartAuto
            End Get
            Set(ByVal value As Boolean)
                _StartAuto = value
                If value = True Then Start()
            End Set
        End Property

        'X10, IP, PLCBUS, 1WIRE, …
        Public ReadOnly Property Protocol() As String
            Get
                Return _Protocol
            End Get
        End Property

        'True si le driver est actif
        Public ReadOnly Property IsConnect() As Boolean
            Get
                Return _IsConnect
            End Get
        End Property

        'Adresse IP (facultatif) en TCP
        Public Property IP_TCP() As String
            Get
                Return _IP_TCP
            End Get
            Set(ByVal value As String)
                _IP_TCP = value
            End Set
        End Property

        'Port IP (facultatif) en TCP
        Public Property Port_TCP()
            Get
                Return _Port_TCP
            End Get
            Set(ByVal value)
                _Port_TCP = value
            End Set
        End Property

        'Adresse IP (facultatif) en UDP
        Public Property IP_UDP() As String
            Get
                Return _IP_UDP
            End Get
            Set(ByVal value As String)
                _IP_UDP = value
            End Set
        End Property

        'Port IP (facultatif) en UDP
        Public Property Port_UDP() As String
            Get
                Return _Port_UDP
            End Get
            Set(ByVal value As String)
                _Port_UDP = value
            End Set
        End Property

        'Port Com (facultatif)
        Public Property COM() As String
            Get
                Return _Com
            End Get
            Set(ByVal value As String)
                _Com = value
            End Set
        End Property

        'Paramétre de rafraichissement ou de pooling (facultatif) en ms
        Public Property Refresh() As Integer
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

        'Si refresh >0 gestion du timer
        Private Sub TimerTick()

        End Sub

        'Modèle du driver (CM11, CM15…) facultatif
        Public ReadOnly Property Modele() As String
            Get
                Return _Modele
            End Get
        End Property

        'Version du driver
        Public ReadOnly Property Version() As String
            Get
                Return _Version
            End Get
        End Property

        'Adresse de son image
        Public Property Picture() As String
            Get
                Return _Picture
            End Get
            Set(ByVal value As String)
                _Picture = value
            End Set
        End Property

        'Fonctions propores au driver

        Public Sub Start()
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
                'Log.Log(TypeLog.INFO, TypeSource.DRIVER, "Driver " & Me.Nom & " démarré")
            Catch ex As Exception
                _IsConnect = False
                'Log.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Driver " & Me.Nom & " erreur lors du démarrage: " & ex.Message)
            End Try
        End Sub

        Public Sub [Stop]()
            usb1 = Nothing
            _IsConnect = False
            'Log.Log(TypeLog.INFO, TypeSource.DRIVER, "Driver " & Me.Nom & " arrêté")
        End Sub

        Public Sub Restart()
            [Stop]()
            Start()
        End Sub

        '*******************************************************
        'Fonctions/Sub gérées par le driver provenant des devices


        '******************************************************
        'Fonctions/Sub propres à ce driver
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
    End Class

End Namespace
