Imports System.Xml.Serialization

''' <summary>
''' Classe utilisée dans la liste des serveurs 
''' </summary>
''' <remarks></remarks>
<Serializable()> Public Class ClServer
    Dim _Adresse As String
    Dim _Defaut As Boolean
    Dim _Port As Double

    ''' <summary>
    ''' Adresse du serveur
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Adresse() As String
        Get
            Return _Adresse
        End Get
        Set(ByVal value As String)
            _Adresse = value
        End Set
    End Property

    ''' <summary>
    ''' True si l'adresse est celle utilisée par défaut
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Defaut() As Boolean
        Get
            Return _Defaut
        End Get
        Set(ByVal value As Boolean)
            _Defaut = value
        End Set
    End Property

    Public Property Port() As Double
        Get
            Return _Port
        End Get
        Set(ByVal value As Double)
            _Port = value
        End Set
    End Property
End Class
