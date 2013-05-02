Public Class HMDObject

    Private _id As String
    Public Property Id() As String
        Get
            Return _id
        End Get
        Set(ByVal value As String)
            _id = value
        End Set
    End Property

    Private _name As String
    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property

    Private _type As String
    Public Property Type() As String
        Get
            Return _type
        End Get
        Set(ByVal value As String)
            _type = value
        End Set
    End Property

    Private _readObjects As List(Of String)
    Public Property ReadObjects() As List(Of String)
        Get
            Return _readObjects
        End Get
        Set(ByVal value As List(Of String))
            _readObjects = value
        End Set
    End Property

    Private _writeObjects As List(Of String)
    Public Property WriteObjects() As List(Of String)
        Get
            Return _writeObjects
        End Get
        Set(ByVal value As List(Of String))
            _writeObjects = value
        End Set
    End Property

    Private _value As String
    Public Property Value() As String
        Get
            Return _value
        End Get
        Set(ByVal value As String)
            _value = value
        End Set
    End Property


    Public Sub New()
        _readObjects = New List(Of String)
        _writeObjects = New List(Of String)
    End Sub
End Class
