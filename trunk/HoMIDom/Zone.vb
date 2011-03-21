Namespace HoMIDom

    '***********************************************
    '** CLASS ZONE
    '** version 1.0
    '** Date de création: 09/02/2011
    '** Historique (SebBergues): 09/02/2011: Création 
    '***********************************************

    ''' <summary>
    ''' Classe Zone, une zone peut représenter une maison, un étage, un groupe de pièces, une pièce, un endroit dans une pièce
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> Public Class Zone
        Dim _Id As String 'Id unique de la zone
        Dim _Name As String 'Libellé de la zone
        Dim _ListDevice As New List(Of Device_Zone) 'Liste d'objet de type DeviceZone
        Dim _Icon As String = "" 'Icon de la zone
        Dim _Image As String = "" 'Image de la zone

        ''' <summary>
        ''' Identification unique de la zone
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ID() As String
            Get
                Return _Id
            End Get
            Set(ByVal value As String)
                _Id = value
            End Set
        End Property

        ''' <summary>
        ''' Image de la zone pour arrière plan, plan.. (grand format)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Image() As String
            Get
                Return _Image
            End Get
            Set(ByVal value As String)
                _Image = value
            End Set
        End Property

        ''' <summary>
        ''' Icone de la zone (pour bouton, treeview, représentation...)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Icon() As String
            Get
                Return _Icon
            End Get
            Set(ByVal value As String)
                _Icon = value
            End Set
        End Property

        ''' <summary>
        ''' Nom de la zone
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = value
            End Set
        End Property

        ''' <summary>
        ''' Contient la liste des devices associés à la zone de type Device_Zone
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ListDevice() As List(Of Device_Zone)
            Get
                Return _ListDevice
            End Get
            Set(ByVal value As List(Of Device_Zone))
                _ListDevice = value
            End Set
        End Property

        ''' <summary>
        ''' Class Device_zone représente un device dans une zone
        ''' </summary>
        ''' <remarks></remarks>
        <Serializable()> Public Class Device_Zone
            Dim _DeviceID As String
            Dim _Visible As Boolean = False
            Dim _X As Double = 0
            Dim _Y As Double = 0

            ''' <summary>
            ''' ID du device
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property DeviceID() As String
                Get
                    Return _DeviceID
                End Get
                Set(ByVal value As String)
                    _DeviceID = value
                End Set
            End Property

            ''' <summary>
            ''' Si le device doit être visible dans la zone
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Visible() As Boolean
                Get
                    Return _Visible
                End Get
                Set(ByVal value As Boolean)
                    _Visible = value
                End Set
            End Property

            ''' <summary>
            ''' Position X du Device dans la zone (pour la partie cliente)
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property X() As Double
                Get
                    Return _X
                End Get
                Set(ByVal value As Double)
                    _X = value
                End Set
            End Property

            ''' <summary>
            ''' Position Y du Device dans la zone (pour la partie cliente)
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Y() As Double
                Get
                    Return _Y
                End Get
                Set(ByVal value As Double)
                    _Y = value
                End Set
            End Property

            Sub New(ByVal DeviceID As String, ByVal Visible As Boolean, ByVal X As Double, ByVal Y As Double)
                _DeviceID = DeviceID
                _Visible = Visible
                _X = X
                _Y = Y
            End Sub
        End Class
    End Class

End Namespace