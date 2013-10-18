Namespace HoMIDom

    Public Class Widget

#Region "Variables"
        Dim _uid As String
        Dim _Label As String
        Dim _X As Double
        Dim _Y As Double
        Dim _Width As Double
        Dim _Height As Double
        Dim _Pictures As New List(Of Picture)
        Dim _Outputs As New List(Of Output)
#End Region

#Region "Property"

        Public Property Uid As String
            Get
                Return _uid
            End Get
            Set(value As String)
                _uid = value
            End Set
        End Property

        Public Property Label As String
            Get
                Return _Label
            End Get
            Set(value As String)
                _Label = value
            End Set
        End Property

        Public Property X As Double
            Get
                Return _X
            End Get
            Set(value As Double)
                _X = value
            End Set
        End Property

        Public Property Y As Double
            Get
                Return _Y
            End Get
            Set(value As Double)
                _Y = value
            End Set
        End Property

        Public Property Width As Double
            Get
                Return _Width
            End Get
            Set(value As Double)
                _Width = value
            End Set
        End Property

        Public Property Height As Double
            Get
                Return _Height
            End Get
            Set(value As Double)
                _Height = value
            End Set
        End Property

        Public Property Pictures As List(Of Picture)
            Get
                Return _Pictures
            End Get
            Set(value As List(Of Picture))
                _Pictures = value
            End Set
        End Property

        Public Property Outputs As List(Of Output)
            Get
                Return _Outputs
            End Get
            Set(value As List(Of Output))
                _Outputs = value
            End Set
        End Property
#End Region

#Region "Methode"
        Public Sub New()
            Uid = System.Guid.NewGuid.ToString()
        End Sub
#End Region

        Public Class Picture
            Dim _Path As String

            Public Property Path As String
                Get
                    Return _Path
                End Get
                Set(value As String)
                    _Path = value
                End Set
            End Property
        End Class

        Public Class Output
            Dim _TemplateID As String 'ID du template concerné
            Dim _Commande As String 'Commande à envoyer

            Public Property TemplateID As String
                Get
                    Return _TemplateID
                End Get
                Set(value As String)
                    _TemplateID = value
                End Set
            End Property

            Public Property Commande As String
                Get
                    Return _Commande
                End Get
                Set(value As String)
                    _Commande = value
                End Set
            End Property
        End Class


    End Class
End Namespace