Namespace HoMIDom
    <Serializable()> Public Class Sequence
        ''' <summary>
        ''' Type de Sequence
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum TypeOfSequence
            Server = 0
            Driver = 1
            Device = 2
            Zone = 3
            Trigger = 4
            Macro = 5
        End Enum

        Dim _Number As String = Nothing
        Dim _Property As String = Nothing
        Dim _Value As Object = Nothing
        Dim _Type As TypeOfSequence = 0
        Dim _ID As String = Nothing
        Dim _DateTime As Date

        ''' <summary>
        ''' Numero de la sequence
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Numero As String
            Get
                Return _Number
            End Get
            Set(value As String)
                _Number = value
            End Set
        End Property

        ''' <summary>
        ''' ID de l'élément concerné
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ID As String
            Get
                Return _ID
            End Get
            Set(value As String)
                _ID = value
            End Set
        End Property

        ''' <summary>
        ''' Date/heure de l'évènement
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DateTime As Date
            Get
                Return _DateTime
            End Get
            Set(value As Date)
                _DateTime = value
            End Set
        End Property

        ''' <summary>
        ''' Propriété de l'évènement concerné
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property [Property] As String
            Get
                Return _Property
            End Get
            Set(value As String)

            End Set
        End Property

        ''' <summary>
        ''' Valeur modifiée
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Value As Object
            Get
                Return _Value
            End Get
            Set(value1 As Object)
                _Value = value1
            End Set
        End Property

        ''' <summary>
        ''' Type de séquence
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SequenceType As TypeOfSequence
            Get
                Return _Type
            End Get
            Set(value As TypeOfSequence)
                _Type = value
            End Set
        End Property

    End Class
End Namespace