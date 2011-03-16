'***********************************************
'** CLASS TEMPLATEDEVICE
'** version 1.0
'** Date de création: 09/02/2011
'** Historique (SebBergues): 09/02/2011: Création 
'***********************************************
Namespace HoMIDom

    ''' <summary>
    ''' Template de class de type Device pour le service web
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> Public Class TemplateDevice
        Dim _ID As String = ""
        Dim _Name As String = ""
        Dim _Enable As Boolean = False
        Dim _DriverId As String = ""
        Dim _Description As String = ""
        Dim _Type As String = ""
        Dim _Adresse1 As String = ""
        Dim _Adresse2 As String = ""
        Dim _DateCreated As Date = Now
        Dim _LastChange As Date = Now
        Dim _LastChangeDuree As Integer = 0
        Dim _Refresh As Double = 0
        Dim _Modele As String = ""
        Dim _Picture As String = ""
        Dim _Solo As Boolean = True
        Dim _LastEtat As Boolean = False
        Dim _ValueLast As Double = 0
        Dim _ValueMin As Double = -9999
        Dim _ValueMax As Double = 9999
        Dim _ValueDef As Double = 0
        Dim _Precision As Double = 0
        Dim _Correction As Double = 0
        Dim _Formatage As String = ""
        Dim _Value As Object = Nothing
        Dim _DeviceAction As New List(Of DeviceAction)

        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = value
            End Set
        End Property
        Public Property ID() As String
            Get
                Return _ID
            End Get
            Set(ByVal value As String)
                _ID = value
            End Set
        End Property

        'Activation du Device
        Public Property Enable() As Boolean
            Get
                Return _Enable
            End Get
            Set(ByVal value As Boolean)
                _Enable = value
            End Set
        End Property

        'Id du driver affect
        Public Property DriverID() As String
            Get
                Return _DriverId
            End Get
            Set(ByVal value As String)
                _DriverId = value
            End Set
        End Property

        'Description qui peut être le modèle du device ou autre chose
        Public Property Description() As String
            Get
                Return _Description
            End Get
            Set(ByVal value As String)
                _Description = value
            End Set
        End Property

        'TEMPERATURE|HUMIDITE|APPAREIL|LUMIERE|CONTACT|TV…
        Public Property Type() As String
            Get
                Return _Type
            End Get
            Set(ByVal value As String)
                _Type = value
            End Set
        End Property

        'Adresse par défaut (pour le X10 par exemple)
        Public Property Adresse1() As String
            Get
                Return _Adresse1
            End Get
            Set(ByVal value As String)
                _Adresse1 = value
            End Set
        End Property

        'Adresse supplémentaire si besoin (cas du RFXCOM)
        Public Property Adresse2() As String
            Get
                Return _Adresse2
            End Get
            Set(ByVal value As String)
                _Adresse2 = value
            End Set
        End Property

        'Date et heure de création du device
        Public Property DateCreated() As Date
            Get
                Return _DateCreated
            End Get
            Set(ByVal value As Date)
                _DateCreated = value
            End Set
        End Property

        'Date et heure de la derniere modification de Value
        Public Property LastChange() As Date
            Get
                Return _LastChange
            End Get
            Set(ByVal value As Date)
                _LastChange = value
            End Set
        End Property

        'Si Lastchanged+LastchangeDuree< Now alors le composant a un problème car il n'a pas emis depuis au moins lastchangedduree.
        Public Property LastChangeDuree() As Integer
            Get
                Return _LastChangeDuree
            End Get
            Set(ByVal value As Integer)
                _LastChangeDuree = value
            End Set
        End Property

        'Modèle du composant
        Public Property Modele() As String
            Get
                Return _Modele
            End Get
            Set(ByVal value As String)
                _Modele = value
            End Set
        End Property

        'Adresse de son image
        Public Property Picture() As String
            Get
                Return _Picture
            End Get
            Set(ByVal value As String)
                _Picture = value
            End Set
        End Property

        'Si le device est solo ou s'il contient plusieurs I/O
        Public Property Solo() As Boolean
            Get
                Return _Solo
            End Get
            Set(ByVal value As Boolean)
                _Solo = value
            End Set
        End Property

        'si =true, on ne prend pas en comtpe les modifications style 19.1 19 19.1 19...
        Public Property LastEtat() As Boolean
            Get
                Return _LastEtat
            End Get
            Set(ByVal value As Boolean)
                _LastEtat = value
            End Set
        End Property

        Public Property DeviceAction() As List(Of DeviceAction)
            Get
                Return _DeviceAction
            End Get
            Set(ByVal value As List(Of DeviceAction))
                _DeviceAction = value
            End Set
        End Property

        Public Property Refresh() As Double
            Get
                Return _Refresh
            End Get
            Set(ByVal value As Double)
                _Refresh = value
            End Set
        End Property

        Public Property Value() As Object
            Get
                Return _Value
            End Get
            Set(ByVal value As Object)
                _Value = value
            End Set
        End Property

        'Contien l'avant derniere valeur
        Public Property ValueLast() As Double
            Get
                Return _ValueLast
            End Get
            Set(ByVal value As Double)
                _ValueLast = value
            End Set
        End Property

        'Valeur minimale que value peut avoir 
        Public Property ValueMin() As Double
            Get
                Return _ValueMin
            End Get
            Set(ByVal value As Double)
                _ValueMin = value
            End Set
        End Property

        'Valeur maximale que value peut avoir 
        Public Property ValueMax() As Double
            Get
                Return _ValueMax
            End Get
            Set(ByVal value As Double)
                _ValueMax = value
            End Set
        End Property

        'Valeur par défaut de Value au démarrage du Device, si Vide = Value
        Public Property ValueDef() As Double
            Get
                Return _ValueDef
            End Get
            Set(ByVal value As Double)
                _ValueDef = value
            End Set
        End Property

        'Precision de value
        Public Property Precision() As Double
            Get
                Return _Precision
            End Get
            Set(ByVal value As Double)
                _Precision = value
            End Set
        End Property

        'Correction en +/-/*/div à effectuer sur la value
        Public Property Correction() As Double
            Get
                Return _Correction
            End Get
            Set(ByVal value As Double)
                _Correction = value
            End Set
        End Property

        'Format de value 0.0 ou 0.00...
        Public Property Formatage() As String
            Get
                Return _Formatage
            End Get
            Set(ByVal value As String)
                _Formatage = value
            End Set
        End Property

        Public ListCommandName As New List(Of String)
        Public ListCommandData As New List(Of String)
        Public ListCommandRepeat As New List(Of String)
    End Class
End Namespace