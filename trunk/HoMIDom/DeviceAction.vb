Namespace HoMIDom

    ''' <summary>
    ''' Class template pour lister les actions possibles sur un device
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DeviceAction
        Dim _Nom As String
        Dim _Param As New List(Of Parametre)

        Public Property Nom() As String
            Get
                Return _Nom
            End Get
            Set(ByVal value As String)
                _Nom = value
            End Set
        End Property

        Public Property Parametres() As List(Of Parametre)
            Get
                Return _Param
            End Get
            Set(ByVal value As List(Of Parametre))
                _Param = value
            End Set
        End Property

        Public Class Parametre
            Dim _Nom As String
            Dim _Type As String
            Dim _Value As Object

            Public Property Nom() As String
                Get
                    Return _Nom
                End Get
                Set(ByVal value As String)
                    _Nom = value
                End Set
            End Property

            Public Property Type() As String
                Get
                    Return _Type
                End Get
                Set(ByVal value As String)
                    _Type = value
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
        End Class
    End Class

End Namespace