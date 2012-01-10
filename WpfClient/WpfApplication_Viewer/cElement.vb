Public Class cElement
    Dim _ID As String
    Dim _ZoneId As String
    Dim _Height As Double
    Dim _Width As Double
    Dim _Angle As Double
    Dim _X As Double
    Dim _Y As Double
    'Dim _Label As String = ""

    Public Property ID As String
        Get
            Return _ID
        End Get
        Set(ByVal value As String)
            _ID = value
        End Set
    End Property

    Public Property ZoneId As String
        Get
            Return _ZoneId
        End Get
        Set(ByVal value As String)
            _ZoneId = value
        End Set
    End Property

    Public Property Height As Double
        Get
            Return _Height
        End Get
        Set(ByVal value As Double)
            _Height = value
        End Set
    End Property

    Public Property Width As Double
        Get
            Return _Width
        End Get
        Set(ByVal value As Double)
            _Width = value
        End Set
    End Property

    Public Property Angle As Double
        Get
            Return _Angle
        End Get
        Set(ByVal value As Double)
            _Angle = value
        End Set
    End Property

    Public Property X As Double
        Get
            Return _X
        End Get
        Set(ByVal value As Double)
            _X = value
        End Set
    End Property

    Public Property Y As Double
        Get
            Return _Y
        End Get
        Set(ByVal value As Double)
            _Y = value
        End Set
    End Property

    'Public Property Label As String
    '    Get
    '        Return _Label
    '    End Get
    '    Set(ByVal value As String)
    '        _Label = value
    '    End Set
    'End Property
End Class
