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
                Return _Server.ReturnDriver(_ID).Item(15)
            End Get
        End Property

        Public Property COM() As String
            Get
                Return _Server.ReturnDriver(_ID).Item(10)
            End Get
            Set(ByVal value As String)
                _Server.ReturnDriverById(_ID).COM = value
            End Set
        End Property

        Public ReadOnly Property Description() As String
            Get
                Return _Server.ReturnDriver(_ID).Item(2)
            End Get
        End Property

        Public Property Enable() As Boolean
            Get
                Return _Server.ReturnDriver(_ID).Item(1)
            End Get
            Set(ByVal value As Boolean)
                '_Server.ReturnDriverById(_ID).Enable = value
            End Set
        End Property

        Public ReadOnly Property ID() As String
            Get
                Return _ID
            End Get
        End Property

        Public Property IP_TCP() As String
            Get
                Return _Server.ReturnDriver(_ID).Item(6)
            End Get
            Set(ByVal value As String)
                '_Server.ReturnDriverById(_ID).IP_TCP = value
            End Set
        End Property

        Public Property IP_UDP() As String
            Get
                Return _Server.ReturnDriver(_ID).Item(8)
            End Get
            Set(ByVal value As String)
                '_Server.ReturnDriverById(_ID).IP_UDP = value
            End Set
        End Property

        Public ReadOnly Property IsConnect() As Boolean
            Get
                Return _Server.ReturnDriver(_ID).Item(5)
            End Get
        End Property

        Public ReadOnly Property Modele() As String
            Get
                Return _Server.ReturnDriver(_ID).Item(12)
            End Get
        End Property

        Public ReadOnly Property Nom() As String
            Get
                Return _Server.ReturnDriver(_ID).Item(0)
            End Get
        End Property

        Public Property Picture() As String
            Get
                Return _Server.ReturnDriver(_ID).Item(14)
            End Get
            Set(ByVal value As String)
                '   _Server.ReturnDriverById(_ID).Picture = value
            End Set
        End Property

        Public Property Port_TCP() As Object
            Get
                Return _Server.ReturnDriver(_ID).Item(7)
            End Get
            Set(ByVal value As Object)
                ' _Server.ReturnDriverById(_ID).Port_TCP = value
            End Set
        End Property

        Public Property Port_UDP() As String
            Get
                Return _Server.ReturnDriver(_ID).Item(9)
            End Get
            Set(ByVal value As String)
                '_Server.ReturnDriverById(_ID).Port_UDP = value
            End Set
        End Property

        Public ReadOnly Property Protocol() As String
            Get
                Return _Server.ReturnDriver(_ID).Item(4)
            End Get
        End Property

        Public Property Refresh() As Integer
            Get
                Return _Server.ReturnDriver(_ID).Item(11)
            End Get
            Set(ByVal value As Integer)
                '_Server.ReturnDriverById(_ID).refresh = value
            End Set
        End Property

        Public Sub Restart()
            '_Server.ReturnDriverById(_ID).restart()
        End Sub

        Public Sub Start()
            '_Server.ReturnDriverById(_ID).start()
        End Sub

        Public Property StartAuto() As Boolean
            Get
                Return _Server.ReturnDriver(_ID).Item(3)
            End Get
            Set(ByVal value As Boolean)
                '  _Server.ReturnDriverById(_ID).StartAuto = value
            End Set
        End Property

        Public Sub [Stop]()
            '_Server.ReturnDriverById(_ID).stop()
        End Sub

        Public ReadOnly Property Version() As String
            Get
                Return _Server.ReturnDriver(_ID).Item(13)
            End Get
        End Property

        Public Sub New(ByVal Serveur As Server, ByVal DriverId As String)
            _Server = Serveur
            _ID = DriverId
        End Sub
    End Class
End Namespace