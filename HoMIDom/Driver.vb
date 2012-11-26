Namespace HoMIDom
    '***********************************************
    '** CLASS NAMESPACE
    '** version 1.1
    '** Date de création: 21/01/2011
    '** Historique (SebBergues): 21/01/2011: Création, sert d'image pour le service web
    '***********************************************

    <Serializable()> Public Class Driver
        Dim _ID As String
        <NonSerialized()> Dim _Server As Server
        <NonSerialized()> Dim _IdSrv As String

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

        Public Property LabelsDriver() As ArrayList
            Get
                Return _Server.ReturnDriver(_ID).Item(17)
            End Get
            Set(ByVal value As ArrayList)
                _Server.WriteDriver(_ID, "LABELSDRIVER", value)
            End Set
        End Property

        Public Property LabelsDevice() As ArrayList
            Get
                Return _Server.ReturnDriver(_ID).Item(17)
            End Get
            Set(ByVal value As ArrayList)
                _Server.WriteDriver(_ID, "LABELSDEVICE", value)
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

        Public Property Modele() As String
            Get
                Return _Server.ReturnDriver(_ID).Item(12)
            End Get
            Set(ByVal value As String)
                _Server.WriteDriver(_ID, "MODELE", value)
            End Set
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

        Public Sub DeleteDevice(ByVal DeviceId As String)
            _Server.WriteDriver(_ID, "DELETEDEVICE", DeviceId)
        End Sub

        Public Sub NewDevice(ByVal DeviceId)
            _Server.WriteDriver(_ID, "NEWDEVICE", DeviceId)
        End Sub

        Public ReadOnly Property Version() As String
            Get
                Return _Server.ReturnDriver(_ID).Item(13)
            End Get
        End Property

        Public ReadOnly Property OsPlatform() As String
            Get
                Return _Server.ReturnDriver(_ID).Item(18)
            End Get
        End Property

        Public Sub New(ByRef Serveur As Server, ByVal SrvId As String, ByVal DriverId As String)
            _Server = Serveur
            _ID = DriverId
            _IdSrv = SrvId
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

        Public Class cLabels
            Dim _NomChamp As String
            Dim _LabelChamp As String
            Dim _ToolTip As String
            Dim _Param As String

            Public Property NomChamp As String
                Get
                    Return _NomChamp
                End Get
                Set(ByVal value As String)
                    _NomChamp = value
                End Set
            End Property

            Public Property LabelChamp As String
                Get
                    Return _LabelChamp
                End Get
                Set(ByVal value As String)
                    _LabelChamp = value
                End Set
            End Property

            Public Property Tooltip As String
                Get
                    Return _ToolTip
                End Get
                Set(ByVal value As String)
                    _ToolTip = value
                End Set
            End Property

            Public Property Parametre As String
                Get
                    Return _Param
                End Get
                Set(ByVal value As String)
                    _Param = value
                End Set
            End Property
        End Class
    End Class
End Namespace