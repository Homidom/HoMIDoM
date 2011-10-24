
Namespace HoMIDom
    ''' <summary>
    ''' Classe de gestion Audio
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> Public Class Audio

        ''' <summary>
        ''' Type d'extension Audio
        ''' </summary>
        ''' <remarks></remarks>
        <Serializable()> Public Class ExtensionAudio
            Dim _Extension As String
            Dim _Enable As Boolean

            ''' <summary>
            ''' Nom de l'extension ex: .wma
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Extension As String
                Get
                    Return _Extension
                End Get
                Set(ByVal value As String)
                    _Extension = value
                End Set
            End Property

            ''' <summary>
            ''' Si l'extension doit être prise en compte
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Enable As String
                Get
                    Return _Enable
                End Get
                Set(ByVal value As String)
                    _Enable = value
                End Set
            End Property
        End Class

        ''' <summary>
        ''' Type Répertoire Audio
        ''' </summary>
        ''' <remarks></remarks>
        <Serializable()> Public Class RepertoireAudio
            Dim _Repertoire As String
            Dim _Enable As Boolean

            ''' <summary>
            ''' Chemin du répertoire
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Repertoire As String
                Get
                    Return _Repertoire
                End Get
                Set(ByVal value As String)
                    _Repertoire = value
                End Set
            End Property

            ''' <summary>
            ''' Si le répertoire doit être pris en compte
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

        End Class

        ''' <summary>
        ''' Class Fichier Audio
        ''' </summary>
        ''' <remarks></remarks>
        Public Class FilePlayList
            Dim _Titre As String
            Dim _Artiste As String
            Dim _Album As String
            Dim _Année As String
            Dim _Comment As String
            Dim _Genre As String
            Dim _Durée As String
            Dim _Source As String
            Dim _SourceWpath As String
            Dim _Track As String

            Public Sub New(ByVal titre As String, ByVal artiste As String, ByVal album As String, ByVal année As String, ByVal comment As String,
                           ByVal genre As String, ByVal durée As String, ByVal source As String, ByVal sourcepath As String, ByVal track As String)
                Me._Titre = titre
                Me._Durée = durée
                Me._Genre = genre
                Me._Année = année
                Me._SourceWpath = sourcepath
                Me._Track = track
            End Sub

            Public Property Titre() As String
                Get
                    Return Me._Titre
                End Get
                Set(ByVal value As String)
                    Me._Titre = value
                End Set
            End Property

            Public Property Artiste() As String
                Get
                    Return Me._Artiste
                End Get
                Set(ByVal value As String)
                    Me._Artiste = value
                End Set
            End Property

            Public Property Album() As String
                Get
                    Return Me._Album
                End Get
                Set(ByVal value As String)
                    Me._Album = value
                End Set
            End Property

            Public Property Année() As String
                Get
                    Return Me._Année
                End Get
                Set(ByVal value As String)
                    Me._Année = value
                End Set
            End Property

            Public Property Comment() As String
                Get
                    Return Me._Comment
                End Get
                Set(ByVal value As String)
                    Me._Comment = value
                End Set
            End Property

            Public Property Genre() As String
                Get
                    Return Me._Genre
                End Get
                Set(ByVal value As String)
                    Me._Genre = value
                End Set
            End Property

            Public Property Durée() As String
                Get
                    Return Me._Durée
                End Get
                Set(ByVal value As String)
                    Me._Durée = value
                End Set
            End Property

            Public Property Source() As String
                Get
                    Return Me._Source
                End Get
                Set(ByVal value As String)
                    Me._Source = value
                End Set
            End Property


            Public Property SourceWpath() As String
                Get
                    Return Me._SourceWpath
                End Get
                Set(ByVal value As String)
                    Me._SourceWpath = value
                End Set
            End Property

            Public Property Track() As String
                Get
                    Return Me._Track
                End Get
                Set(ByVal value As String)
                    Me._Track = value
                End Set
            End Property
        End Class

    End Class
End Namespace
