Namespace HoMIDom

    ''' <summary>
    ''' Template Action
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TemplateAction
        Dim _TypeAction As HoMIDom.Action.TypeAction
        Dim _Timing As DateTime
        Dim _IdDevice As String
        Dim _Action As String
        Dim _Parametres As New ArrayList

        Public Property TypeAction As HoMIDom.Action.TypeAction
            Get
                Return _TypeAction
            End Get
            Set(ByVal value As HoMIDom.Action.TypeAction)
                _TypeAction = value
            End Set
        End Property

        Public Property Timing As DateTime
            Get
                Return _Timing
            End Get
            Set(ByVal value As DateTime)
                _Timing = value
            End Set
        End Property

        Public Property IdDevice As String
            Get
                Return _IdDevice
            End Get
            Set(ByVal value As String)
                _IdDevice = value
            End Set
        End Property

        Public Property Action As String
            Get
                Return _Action
            End Get
            Set(ByVal value As String)
                _Action = value
            End Set
        End Property

        Public Property Parametres As ArrayList
            Get
                Return _Parametres
            End Get
            Set(ByVal value As ArrayList)
                _Parametres = value
            End Set
        End Property
    End Class

End Namespace