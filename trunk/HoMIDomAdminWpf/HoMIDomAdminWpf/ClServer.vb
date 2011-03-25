Imports System.Xml.Serialization

<Serializable()> Public Class ClServer
    Dim _Adresse As String
    Dim _Defaut As Boolean

    Public Property Adresse() As String
        Get
            Return _Adresse
        End Get
        Set(ByVal value As String)
            _Adresse = value
        End Set
    End Property

    Public Property Defaut() As Boolean
        Get
            Return _Defaut
        End Get
        Set(ByVal value As Boolean)
            _Defaut = value
        End Set
    End Property


End Class
