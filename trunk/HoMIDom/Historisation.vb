Namespace HoMIDom
    <Serializable()> Public Class Historisation
        Dim _nom As String = ""
        Dim _iddevice As String = ""
        Dim _select As Boolean = False
        Dim _datetime As DateTime
        Dim _value As String = ""

        Public Property Nom As String
            Get
                Return _nom
            End Get
            Set(ByVal value As String)
                _nom = value
            End Set
        End Property

        Public Property IdDevice As String
            Get
                Return _iddevice
            End Get
            Set(ByVal value As String)
                _iddevice = value
            End Set
        End Property

        Public Property [Select] As Boolean
            Get
                Return _select
            End Get
            Set(ByVal value As Boolean)
                _select = value
            End Set
        End Property

        Public Property [DateTime] As DateTime
            Get
                Return _datetime
            End Get
            Set(ByVal value As DateTime)
                _datetime = value
            End Set
        End Property

        Public Property Value As String
            Get
                Return _value
            End Get
            Set(ByVal value1 As String)
                _value = value1
            End Set
        End Property

    End Class
End Namespace
