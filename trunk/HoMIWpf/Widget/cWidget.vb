Public Class cWidget

    Public Class Action
        Dim _IdObject As String
        Dim _Methode As String
        Dim _Value As Object
        Dim _Sound As String

        Public Property IdObject As String
            Get
                Return _IdObject
            End Get
            Set(ByVal value As String)
                _IdObject = value
            End Set
        End Property

        Public Property Methode As String
            Get
                Return _Methode
            End Get
            Set(ByVal value As String)
                _Methode = value
            End Set
        End Property

        Public Property Value As Object
            Get
                Return _Value
            End Get
            Set(ByVal value2 As Object)
                _Value = value2
            End Set
        End Property

        Public Property Sound As String
            Get
                Return _Sound
            End Get
            Set(ByVal value As String)
                _Sound = value
            End Set
        End Property

        Sub New(Optional ByVal vIdObject As String = "", Optional ByVal vMethode As String = "", Optional ByVal vValue As Object = Nothing)
            _IdObject = vIdObject
            _Methode = vMethode
            _Value = vValue
        End Sub
    End Class

    Public Class Visu
        Dim _IdObject As String
        Dim _Propriete As String
        Dim _Value As Object
        Dim _Image As String

        Public Property IdObject As String
            Get
                Return _IdObject
            End Get
            Set(ByVal value As String)
                _IdObject = value
            End Set
        End Property

        Public Property Propriete As String
            Get
                Return _Propriete
            End Get
            Set(ByVal value As String)
                _Propriete = value
            End Set
        End Property

        Public Property Value As Object
            Get
                Return _Value
            End Get
            Set(ByVal value2 As Object)
                _Value = value2
            End Set
        End Property

        Public Property Image As String
            Get
                Return _Image
            End Get
            Set(ByVal value2 As String)
                _Image = value2
            End Set
        End Property

        Sub New(Optional ByVal vIdObject As String = "", Optional ByVal vPropriete As String = "", Optional ByVal vValue As Object = Nothing, Optional ByVal vImage As String = "")
            _IdObject = vIdObject
            _Propriete = vPropriete
            _Value = vValue
            _Image = vImage
        End Sub

    End Class
End Class
