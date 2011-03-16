Namespace HoMIDom

    ''' <summary>
    ''' Template de Classe de type Driver pour le service web
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> Public Class TemplateDriver
        Dim _ID As String = ""
        Dim _Nom As String = ""
        Dim _Enable As Boolean = False
        Dim _Description As String = ""
        Dim _Startauto As Boolean = False
        Dim _Protocol As String = ""
        Dim _IsConnect As Boolean = False
        Dim _IP_TCP As String = ""
        Dim _Port_TCP As String = ""
        Dim _IP_UDP As String = ""
        Dim _Port_UDP As String = ""
        Dim _Com As String = ""
        Dim _Refresh As Integer = 0
        Dim _Modele As String = ""
        Dim _Version As String = ""
        Dim _Picture As String = ""
        Dim _DeviceSupport As New List(Of String)
        Dim _DeviceAction As New List(Of DeviceAction)

        Public Property ID() As String  'Identification unique du driver
            Get
                Return _ID
            End Get
            Set(ByVal value As String)
                _ID = value
            End Set
        End Property
        Public Property Nom() As String
            Get
                Return _Nom
            End Get
            Set(ByVal value As String)
                _Nom = value
            End Set
        End Property
        Public Property Enable() As Boolean
            Get
                Return _Enable
            End Get
            Set(ByVal value As Boolean)
                _Enable = value
            End Set
        End Property
        Public Property Description() As String
            Get
                Return _Description
            End Get
            Set(ByVal value As String)
                _Description = value
            End Set
        End Property
        Public Property StartAuto() As Boolean
            Get
                Return _Startauto
            End Get
            Set(ByVal value As Boolean)
                _Startauto = value
            End Set
        End Property
        Public Property Protocol() As String
            Get
                Return _Protocol
            End Get
            Set(ByVal value As String)
                _Protocol = value
            End Set
        End Property
        Public Property IsConnect() As Boolean
            Get
                Return _IsConnect
            End Get
            Set(ByVal value As Boolean)
                _IsConnect = value
            End Set
        End Property
        Public Property IP_TCP() As String
            Get
                Return _IP_TCP
            End Get
            Set(ByVal value As String)
                _IP_TCP = value
            End Set
        End Property
        Public Property Port_TCP() As String
            Get
                Return _Port_TCP
            End Get
            Set(ByVal value As String)
                _Port_TCP = value
            End Set
        End Property
        Public Property IP_UDP() As String
            Get
                Return _IP_UDP
            End Get
            Set(ByVal value As String)
                _IP_UDP = value
            End Set
        End Property
        Public Property Port_UDP() As String
            Get
                Return _Port_UDP
            End Get
            Set(ByVal value As String)
                _Port_UDP = value
            End Set
        End Property
        Public Property COM() As String
            Get
                Return _Com
            End Get
            Set(ByVal value As String)
                _Com = value
            End Set
        End Property
        Public Property Refresh() As Integer
            Get
                Return _Refresh
            End Get
            Set(ByVal value As Integer)
                _Refresh = value
            End Set
        End Property
        Public Property Modele() As String
            Get
                Return _Modele
            End Get
            Set(ByVal value As String)
                _Modele = value
            End Set
        End Property
        Public Property Version() As String
            Get
                Return _Version
            End Get
            Set(ByVal value As String)
                _Version = value
            End Set
        End Property
        Public Property Picture() As String
            Get
                Return _Picture
            End Get
            Set(ByVal value As String)
                _Picture = value
            End Set
        End Property
        Public Property DeviceSupport() As List(Of String)
            Get
                Return _DeviceSupport
            End Get
            Set(ByVal value As List(Of String))
                _DeviceSupport = value
            End Set
        End Property
        Public Property DeviceAction() As List(Of DeviceAction)
            Get
                Return _DeviceAction
            End Get
            Set(ByVal value As List(Of DeviceAction))
                _DeviceAction = value
            End Set
        End Property

    End Class

End Namespace