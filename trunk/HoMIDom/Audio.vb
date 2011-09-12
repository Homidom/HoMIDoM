
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
            Dim _Enable As String

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
            Dim _Enable As String

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
    End Class
End Namespace
