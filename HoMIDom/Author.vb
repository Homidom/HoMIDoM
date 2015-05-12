Imports HoMIDom
Imports HoMIDom.HoMIDom.Server

Namespace HoMIDom

    ''' <summary>Class Authentication, Défini le type pour les autorisations</summary>
    <Serializable()> Public Class Authentication

        Dim _Nom As String = ""
        Dim _access_token As String
        Dim _refresh_token As String
        Dim _expires_in As Integer

        'Propriétés

        ''' <summary>
        ''' Nom de cette autorisation
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

        'Propriétés

        ''' <summary>
        ''' access_token de cette autorisation
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property access_token As String
            Get
                Return _access_token
            End Get
            Set(ByVal value As String)
                _access_token = value
            End Set
        End Property

        'Propriétés

        ''' <summary>
        ''' refresh token de cette autorisation
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property refresh_token As String
            Get
                Return _refresh_token
            End Get
            Set(ByVal value As String)
                _refresh_token = value
            End Set
        End Property

        'Propriétés

        ''' <summary>
        ''' expiration du access_token de cette autorisation
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property expires_in As Integer
            Get
                Return _expires_in
            End Get
            Set(ByVal value As Integer)
                _expires_in = value
            End Set
        End Property

    End Class

    ''' <summary>Class installed, Défini le type des données clients</summary>
    <Serializable()> Public Class web

        Dim _client_secret As String = ""
        Dim _redirect_uris As List(Of String)
        Dim _client_id As String = ""

        ''' <summary>
        ''' client_secret pour ce client
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property client_secret As String
            Get
                Return _client_secret
            End Get
            Set(ByVal value As String)
                _client_secret = value
            End Set
        End Property

        ''' <summary>
        ''' redirect_uris pour ce client
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property redirect_uris As List(Of String)
            Get
                Return _redirect_uris
            End Get
            Set(ByVal value As List(Of String))
                _redirect_uris = value
            End Set
        End Property

        ''' <summary>
        '''client_id pour ce client
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property client_id As String
            Get
                Return _client_id
            End Get
            Set(ByVal value As String)
                _client_id = value
            End Set
        End Property

    End Class

    ''' <summary>Class ClientOAuth2, Défini le type pour le client OAuth2</summary>
    <Serializable()> Public Class ClientOAuth2

        Dim _Nom As String = ""
        Dim _web As web

        'Propriétés

        ''' <summary>
        ''' Nom de ce client
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
        '''client_id pour ce client
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property web As web
            Get
                Return _web
            End Get
            Set(ByVal value As web)
                _web = value
            End Set
        End Property
    End Class


End Namespace

