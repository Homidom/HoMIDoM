Namespace HoMIDom
    '***********************************************
    '** CLASS NAMESPACE
    '** version 1.1
    '** Date de création: 21/01/2011
    '** Historique (SebBergues): 21/01/2011: Création 
    '***********************************************

    <Serializable()> Public Class Driver
        Dim _ID As String
        Dim _Server As Server

        Public ReadOnly Property DeviceSupport() As ArrayList
            Get
                Return _Server.ReturnDriverById(_ID).DeviceSupport
            End Get
        End Property

        Public Property COM() As String
            Get
                Return _Server.ReturnDriverById(_ID).COM
            End Get
            Set(ByVal value As String)
                _Server.ReturnDriverById(_ID).COM = value
            End Set
        End Property

        Public ReadOnly Property Description() As String
            Get
                Return _Server.ReturnDriverById(_ID).Description
            End Get
        End Property

        Public Property Enable() As Boolean
            Get
                Return _Server.ReturnDriverById(_ID).Enable
            End Get
            Set(ByVal value As Boolean)
                _Server.ReturnDriverById(_ID).Enable = value
            End Set
        End Property

        Public ReadOnly Property ID() As String
            Get
                Return _Server.ReturnDriverById(_ID)
            End Get
        End Property

        Public Property IP_TCP() As String
            Get
                Return _Server.ReturnDriverById(_ID).IP_TCP
            End Get
            Set(ByVal value As String)
                _Server.ReturnDriverById(_ID).IP_TCP = value
            End Set
        End Property

        Public Property IP_UDP() As String
            Get
                Return _Server.ReturnDriverById(_ID).IP_UDP
            End Get
            Set(ByVal value As String)
                _Server.ReturnDriverById(_ID).IP_UDP = value
            End Set
        End Property

        Public ReadOnly Property IsConnect() As Boolean
            Get
                Return _Server.ReturnDriverById(_ID).IsConnect
            End Get
        End Property

        Public ReadOnly Property Modele() As String
            Get
                Return _Server.ReturnDriverById(_ID).Modele
            End Get
        End Property

        Public ReadOnly Property Nom() As String
            Get
                Return "0"
                '  MsgBox("0")
                '  Dim x As Object = _Server.ReturnDriverById(_ID)
                '  Return _Server.Longitude  '_Server.ReturnDriverById(_ID).Nom
            End Get
        End Property

        Public Property Picture() As String
            Get
                Return _Server.ReturnDriverById(_ID).Picture
            End Get
            Set(ByVal value As String)
                _Server.ReturnDriverById(_ID).Picture = value
            End Set
        End Property

        Public Property Port_TCP() As Object
            Get
                Return _Server.ReturnDriverById(_ID).Port_TCP
            End Get
            Set(ByVal value As Object)
                _Server.ReturnDriverById(_ID).Port_TCP = value
            End Set
        End Property

        Public Property Port_UDP() As String
            Get
                Return _Server.ReturnDriverById(_ID).Port_UDP
            End Get
            Set(ByVal value As String)
                _Server.ReturnDriverById(_ID).Port_UDP = value
            End Set
        End Property

        Public ReadOnly Property Protocol() As String
            Get
                Return _Server.ReturnDriverById(_ID).Protocol
            End Get
        End Property

        Public Property Refresh() As Integer
            Get
                Return _Server.ReturnDriverById(_ID).refresh
            End Get
            Set(ByVal value As Integer)
                _Server.ReturnDriverById(_ID).refresh = value
            End Set
        End Property

        Public Sub Restart()
            _Server.ReturnDriverById(_ID).restart()
        End Sub

        Public Sub Start()
            _Server.ReturnDriverById(_ID).start()
        End Sub

        Public Property StartAuto() As Boolean
            Get
                _Server.ReturnDriverById(_ID).startauto()
            End Get
            Set(ByVal value As Boolean)
                _Server.ReturnDriverById(_ID).StartAuto = value
            End Set
        End Property

        Public Sub [Stop]()
            _Server.ReturnDriverById(_ID).stop()
        End Sub

        Public ReadOnly Property Version() As String
            Get
                Return _Server.ReturnDriverById(_ID).Version
            End Get
        End Property

        Public Sub New(ByVal Serveur As Server, ByVal DriverId As String)
            _Server = Serveur
            _ID = DriverId
        End Sub

    End Class
End Namespace