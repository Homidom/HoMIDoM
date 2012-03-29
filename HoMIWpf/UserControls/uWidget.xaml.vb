Public Class uWidget

    Dim _ID As String
    Dim _Etiquette As String

    Public Property ID As String
        Get
            Return _ID
        End Get
        Set(ByVal value As String)
            _ID = value
        End Set
    End Property

    Public Property Etiquette As String
        Get
            Return _Etiquette
        End Get
        Set(ByVal value As String)
            _Etiquette = value
        End Set
    End Property


End Class
