Imports HoMIDom.HoMIDom.Server

Namespace HoMIDom
    '***********************************************
    '** CLASS VARIABLE
    '** version 1.0
    '** Date de création: 15/05/2014
    '***********************************************

    'Class definissant les variables au sein d'Homidom
    '***********************************************

    <Serializable()> Public Class Variable

        Dim _ID As String = "" 'ID de la variable
        Dim _Nom As String = "" 'Nom de la variable
        Dim _Value As String = "" 'Valeur de la variable
        Dim _Description As String = "" 'Description de la variable
        Dim _Enable As Boolean = True 'Variable Active/Desactive
        <NonSerialized()> Dim _Server As Server = Nothing

        ''' <summary>
        ''' ID de la variable
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ID As String
            Get
                Return _ID
            End Get
            Set(value As String)
                If String.IsNullOrEmpty(value) = False Then _ID = value
            End Set
        End Property

        ''' <summary>
        ''' Nom de la variable, pas de doublon autorisé
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Nom As String
            Get
                Return _Nom
            End Get
            Set(value As String)
                If String.IsNullOrEmpty(value) = False Then _Nom = value
            End Set
        End Property

        ''' <summary>
        ''' Valeur de la variable
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Value As String
            Get
                Return _Value
            End Get
            Set(value1 As String)
                If _Server IsNot Nothing Then
                    If _Server.Etat_server Then
                        If value1 <> _Value And _Server IsNot Nothing Then
                            _Server.VarEvent(Nom, value1)
                        End If
                    End If
                End If
                _Value = value1
            End Set
        End Property

        ''' <summary>
        ''' Activation ou Non de la variable, par défaut activée
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Enable As Boolean
            Get
                Return _Enable
            End Get
            Set(value As Boolean)
                _Enable = value
            End Set
        End Property

        ''' <summary>
        ''' Description de la variable
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Description As String
            Get
                Return _Description
            End Get
            Set(value As String)
                _Description = value
            End Set
        End Property

        Public Sub New(Optional ByRef Serveur As Server = Nothing)
            _Server = Serveur
        End Sub
    End Class

End Namespace