Imports System.Xml.Serialization

''' <summary>
''' Classe utilisée dans la liste des serveurs 
''' </summary>
''' <remarks></remarks>
<Serializable()> Public Class ClServer
    Dim _Nom As String
    Dim _Adresse As String
    Dim _Defaut As Boolean
    Dim _Port As Double
    Dim _Icon As String
    Dim _Id As String

    Public Property Id As String
        Get
            Return _Id
        End Get
        Set(ByVal value As String)
            _Id = value
        End Set
    End Property

    ''' <summary>
    ''' Nom du serveur
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Nom As String
        Get
            Return _Nom
        End Get
        Set(ByVal value As String)
            _Nom = value
        End Set
    End Property


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

    Public Property Icon As String
        Get
            Return _Icon
        End Get
        Set(ByVal value As String)
            _Icon = value
        End Set
    End Property
End Class
