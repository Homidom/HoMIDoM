Namespace HoMIDom
    '***********************************************
    '** CLASS DRIVER VIRTUEL
    '** version 1.0
    '** Date de création: 12/01/2011
    '** Historique (SebBergues): 12/01/2011: Création 
    '***********************************************

    <Serializable()> Public Class Driver_Virtuel
        Inherits Server

        Dim _ID As String
        Dim _Nom As String = "Virtuel"
        Dim _Enable As String = False
        Dim _Description As String = "Driver Virtuel"
        Dim _StartAuto As Boolean = False
        Dim _Protocol As String = "Virtuel"
        Dim _IsConnect As Boolean = False
        Dim _IP_TCP As String
        Dim _Port_TCP As String
        Dim _IP_UDP As String
        Dim _Port_UDP As String
        Dim _Com As String
        Dim _Refresh As Integer
        Dim _Modele As String = "Virtuel"
        Dim _Version As String = "1.0"
        Dim _Picture As String
        Dim MyTimer As New Timers.Timer

        Public Event DriverEvent(ByVal DriveName As String, ByVal TypeEvent As String, ByVal Parametre As Object)



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
            _IsConnect = True
            'Log.Log(TypeLog.INFO, TypeSource.DRIVER, "Driver " & Me.Nom & " démarré")
        End Sub

        Public Sub [Stop]()
            _IsConnect = False
            'Log.Log(TypeLog.INFO, TypeSource.DRIVER, "Driver " & Me.Nom & " arrêté")
        End Sub

        Public Sub Restart()
            [Stop]()
            Start()
        End Sub

        'Fonctions gérées par le driver provenant des devices

    End Class

End Namespace
