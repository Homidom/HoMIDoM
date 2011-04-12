Namespace HoMIDom
    '***********************************************
    '** CLASS NAMESPACE
    '** version 1.1
    '** Date de création: 21/01/2011
    '** Historique (SebBergues): 21/01/2011: Création, sert d'image pour le service web
    '***********************************************

    <Serializable()> Public Class Driver
        Dim _ID As String
        Dim _Server As Server

        Public ReadOnly Property DeviceSupport() As ArrayList
            Get
                Return _Server.ReturnDriver(_ID).Item(15)
            End Get
        End Property

        Public Property Parametres() As ArrayList
            Get
                Return _Server.ReturnDriver(_ID).Item(16)
            End Get
            Set(ByVal value As ArrayList)
                _Server.WriteDriver(_ID, "PARAMETRES", value)
            End Set
        End Property

        Public Property COM() As String
            Get
                Return _Server.ReturnDriver(_ID).Item(10)
            End Get
            Set(ByVal value As String)
                _Server.WriteDriver(_ID, "COM", value)
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
                _Server.WriteDriver(_ID, "ENABLE", value)
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
                _Server.WriteDriver(_ID, "IP_TCP", value)
            End Set
        End Property

        Public Property IP_UDP() As String
            Get
                Return _Server.ReturnDriver(_ID).Item(8)
            End Get
            Set(ByVal value As String)
                _Server.WriteDriver(_ID, "IP_UDP", value)
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
                _Server.WriteDriver(_ID, "PICTURE", value)
            End Set
        End Property

        Public Property Port_TCP() As Object
            Get
                Return _Server.ReturnDriver(_ID).Item(7)
            End Get
            Set(ByVal value As Object)
                _Server.WriteDriver(_ID, "PORT_TCP", value)
            End Set
        End Property

        Public Property Port_UDP() As String
            Get
                Return _Server.ReturnDriver(_ID).Item(9)
            End Get
            Set(ByVal value As String)
                _Server.WriteDriver(_ID, "PORT_UDP", value)
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
                _Server.WriteDriver(_ID, "REFRESH", value)
            End Set
        End Property

        Public Sub Restart()
            _Server.WriteDriver(_ID, "RESTART", Nothing)
        End Sub

        Public Sub Start()
            _Server.WriteDriver(_ID, "START", Nothing)
        End Sub

        Public Property StartAuto() As Boolean
            Get
                Return _Server.ReturnDriver(_ID).Item(3)
            End Get
            Set(ByVal value As Boolean)
                _Server.WriteDriver(_ID, "STARTAUTO", value)
            End Set
        End Property

        Public Sub [Stop]()
            _Server.WriteDriver(_ID, "STOP", Nothing)
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

        Public Class Parametre
            Dim _Nom As String
            Dim _Description As String
            Dim _Value As Object

            Public Property Nom As String
                Get
                    Return _Nom
                End Get
                Set(ByVal value As String)
                    _Nom = value
                End Set
            End Property

            Public Property Description As String
                Get
                    Return _Description
                End Get
                Set(ByVal value As String)
                    _Description = value
                End Set
            End Property

            Public Property Valeur As Object
                Get
                    Return _Value
                End Get
                Set(ByVal value As Object)
                    _Value = value
                End Set
            End Property
        End Class
    End Class
End Namespace