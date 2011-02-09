Namespace HoMIDom

    '***********************************************
    '** CLASS ZONE
    '** version 1.0
    '** Date de création: 09/02/2011
    '** Historique (SebBergues): 09/02/2011: Création 
    '***********************************************

    <Serializable()> Public Class Zone
        Dim _Id As String 'Id unique de la zone
        Dim _Name As String 'Libellé de la zone
        Dim _ListDevice As New ArrayList
        Dim _Icon As String = "" 'Icon de la zone
        Dim _Image As String = "" 'Image de la zone

        Public Property ID() As String
            Get
                Return _Id
            End Get
            Set(ByVal value As String)
                _Id = value
            End Set
        End Property
        Public Property Image() As String
            Get
                Return _Image
            End Get
            Set(ByVal value As String)
                _Image = value
            End Set
        End Property
        Public Property Icon() As String
            Get
                Return _Icon
            End Get
            Set(ByVal value As String)
                _Icon = value
            End Set
        End Property
        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = value
            End Set
        End Property

        Public Property ListDevice() As ArrayList
            Get
                Return _ListDevice
            End Get
            Set(ByVal value As ArrayList)
                _ListDevice = value
            End Set
        End Property

        <Serializable()> Public Class Device_Zone
            Dim _DeviceID As String
            Dim _Visible As Boolean = False
            Dim _X As Double = 0
            Dim _Y As Double = 0

            Public Property DeviceID() As String
                Get
                    Return _DeviceID
                End Get
                Set(ByVal value As String)
                    _DeviceID = value
                End Set
            End Property

            Public Property Visible() As Boolean
                Get
                    Return _Visible
                End Get
                Set(ByVal value As Boolean)
                    _Visible = value
                End Set
            End Property

            Public Property X() As Double
                Get
                    Return _X
                End Get
                Set(ByVal value As Double)
                    _X = value
                End Set
            End Property

            Public Property Y() As Double
                Get
                    Return _Y
                End Get
                Set(ByVal value As Double)
                    _Y = value
                End Set
            End Property

            Sub New(ByVal DeviceID As String, ByVal Visible As Boolean, ByVal X As Double, ByVal Y As Double)
                _DeviceID = DeviceID
                _Visible = Visible
                _X = X
                _Y = Y
            End Sub
        End Class
    End Class

End Namespace