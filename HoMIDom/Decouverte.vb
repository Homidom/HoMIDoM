Namespace HoMIDom

    <Serializable()> Public Class NewDevice
        Dim _ID As String ' N° créé automatiquement permet d'identifier le device de façon unique
        Dim _IDDriver As String ' N° ID du driver
        Dim _Adresse1 As String = "" ' Adresse1 du device
        Dim _Adresse2 As String = "" 'Adresse2 du device (si besoin au cas où, option)
        Dim _Name As String 'Nom du device donné automatiquement en attendant du type <Device><Number> (ex: Device12)
        Dim _Type As String = "" 'Type de device par défaut null
        Dim _Ignore As Boolean = False 'Indique si le device doit être ignoré
        Dim _DateTetect As DateTime 'Indique la date et l'heure quand le nouveau device a été ajouté (permettant de les différencier/retrouver)
        Dim _Value As String = "" 'valeur actuel du device

        Public Property ID As String
            Get
                Return _ID
            End Get
            Set(ByVal value As String)
                _ID = value
            End Set
        End Property

        Public Property IdDriver As String
            Get
                Return _IDDriver
            End Get
            Set(ByVal value As String)
                _IDDriver = value
            End Set
        End Property

        Public Property Adresse1 As String
            Get
                Return _Adresse1
            End Get
            Set(ByVal value As String)
                _Adresse1 = value
            End Set
        End Property

        Public Property Adresse2 As String
            Get
                Return _Adresse2
            End Get
            Set(ByVal value As String)
                _Adresse2 = value
            End Set
        End Property

        Public Property Name As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = value
            End Set
        End Property

        Public Property Type As String
            Get
                Return _Type
            End Get
            Set(ByVal value As String)
                _Type = value
            End Set
        End Property

        Public Property Ignore As Boolean
            Get
                Return _Ignore
            End Get
            Set(ByVal value As Boolean)
                _Ignore = value
            End Set
        End Property

        Public Property DateTetect As DateTime
            Get
                Return _DateTetect
            End Get
            Set(ByVal value As DateTime)
                _DateTetect = value
            End Set
        End Property

        Public Property Value As String
            Get
                Return _Value
            End Get
            Set(ByVal value As String)
                _Value = value
            End Set
        End Property
    End Class

End Namespace
