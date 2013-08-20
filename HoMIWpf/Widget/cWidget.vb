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

    Public Enum Operateur
        Equal = 0
        Diff = 1
        Inferieur = 2
        Superieur = 3
        InferieurEqual = 4
        SuperieurEqual = 5
    End Enum

    Public Class Visu
        Dim _IdObject As String
        Dim _Propriete As String
        Dim _Op As Operateur = Operateur.Equal
        Dim _Value As Object
        Dim _Image As String
        Dim _Txt As String

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

        Public Property Operateur As Operateur
            Get
                Return _Op
            End Get
            Set(value As Operateur)
                _Op = value
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

        Public Property Text As String
            Get
                Return _Txt
            End Get
            Set(value As String)
                _Txt = value
            End Set
        End Property

        Sub New(Optional ByVal vIdObject As String = "", Optional ByVal vPropriete As String = "", Optional ByVal vValue As Object = Nothing, Optional ByVal vImage As String = "", Optional vOperateur As Operateur = cWidget.Operateur.Equal, Optional vText As String = "")
            _IdObject = vIdObject
            _Propriete = vPropriete
            _Value = vValue
            _Image = vImage
            _Propriete = vPropriete
            _Txt = vText
        End Sub

    End Class
End Class
