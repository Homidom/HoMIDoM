Public Class ImageButton
    Inherits Image

    Dim _Row As Integer
    Dim _Column As Integer

    Public Property Row As Integer
        Get
            Return _Row
        End Get
        Set(ByVal value As Integer)
            _Row = value
        End Set
    End Property

    Public Property Column As Integer
        Get
            Return _Column
        End Get
        Set(ByVal value As Integer)
            _Column = value
        End Set
    End Property
End Class
