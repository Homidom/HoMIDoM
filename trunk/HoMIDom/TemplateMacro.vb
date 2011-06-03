Namespace HoMIDom
    Public Class TemplateMacro
        'Déclaration des variables
        Dim _ID As String
        Dim _Nom As String
        Dim _Description As String
        Dim _Enable As Boolean
        Dim _ListActions As New List(Of TemplateAction)
        Dim _Server As Server

        'Propriétés

        ''' <summary>
        ''' ID de la macro
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ID As String
            Get
                Return _ID
            End Get
            Set(ByVal value As String)
                _ID = value
            End Set
        End Property

        ''' <summary>
        ''' Nom de la macro
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Nom As String
            Get
                Return _Nom
            End Get
            Set(ByVal value As String)
                _Nom = value
            End Set
        End Property

        ''' <summary>
        ''' Decription de la macro
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Description As String
            Get
                Return _Description
            End Get
            Set(ByVal value As String)
                _Description = value
            End Set
        End Property

        ''' <summary>
        ''' Active la macro
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Enable As Boolean
            Get
                Return _Enable
            End Get
            Set(ByVal value As Boolean)
                _Enable = value
            End Set
        End Property

        ''' <summary>
        ''' Liste des actions de la macro macro est False
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ListActions As List(Of TemplateAction)
            Get
                Return _ListActions
            End Get
            Set(ByVal value As List(Of TemplateAction))
                _ListActions = value
            End Set
        End Property

    End Class
End Namespace