Namespace HoMIDom

    ''' <summary>
    ''' Classe Users pour gérer les utilisateurs
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> Public Class Users

        ''' <summary>
        ''' Type de profil disponible
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum TypeProfil
            ''' <summary>
            ''' Invité
            ''' </summary>
            ''' <remarks></remarks>
            invite = 0
            ''' <summary>
            ''' Utilisateur (actions limitées)
            ''' </summary>
            ''' <remarks></remarks>
            user = 1
            ''' <summary>
            ''' Administrateur
            ''' </summary>
            ''' <remarks></remarks>
            admin = 2
        End Enum

        ''' <summary>
        ''' Classe permettant d'un user
        ''' </summary>
        ''' <remarks></remarks>
        Public Class User
            Dim _ID As String = ""
            Dim _UserName As String = ""
            Dim _Nom As String = ""
            Dim _Prenom As String = ""
            Dim _Profil As TypeProfil = TypeProfil.invite
            Dim _Password As String = ""
            Dim _NumberIdentification As String = ""
            Dim _Image As String = ""
            Dim _eMail As String = ""
            Dim _eMailAutre As String = ""
            Dim _TelFixe As String = ""
            Dim _TelMobile As String = ""
            Dim _TelAutre As String = ""
            Dim _Adresse As String = ""
            Dim _Ville As String = ""
            Dim _CodePostal As String = ""

            ''' <summary>
            ''' ID du user
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property ID() As String
                Get
                    Return _ID
                End Get
                Set(ByVal value As String)
                    _ID = value
                End Set
            End Property

            ''' <summary>
            ''' Identifiant pour la connexion
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property UserName() As String
                Get
                    Return _UserName
                End Get
                Set(ByVal value As String)
                    _UserName = value
                End Set
            End Property

            ''' <summary>
            ''' Nom du user
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Nom() As String
                Get
                    Return _Nom
                End Get
                Set(ByVal value As String)
                    _Nom = value
                End Set
            End Property

            ''' <summary>
            ''' Prénom du user
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Prenom() As String
                Get
                    Return _Prenom
                End Get
                Set(ByVal value As String)
                    _Prenom = value
                End Set
            End Property

            ''' <summary>
            ''' Profil du user (invité, user, admin)
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Profil() As TypeProfil
                Get
                    Return _Profil
                End Get
                Set(ByVal value As TypeProfil)
                    _Profil = value
                End Set
            End Property

            ''' <summary>
            ''' Password du user
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Password() As String
                Get
                    Return _Password
                End Get
                Set(ByVal value As String)
                    _Password = value
                End Set
            End Property

            ''' <summary>
            ''' Identification du user utilisé pour RFID, code à barre....
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property NumberIdentification() As String
                Get
                    Return _NumberIdentification
                End Get
                Set(ByVal value As String)
                    _NumberIdentification = value
                End Set
            End Property

            ''' <summary>
            ''' Image du user
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
            ''' eMail principale du user
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property eMail() As String
                Get
                    Return _eMail
                End Get
                Set(ByVal value As String)
                    _eMail = value
                End Set
            End Property

            ''' <summary>
            ''' Adresse secondaire email du user
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property eMailAutre() As String
                Get
                    Return _eMailAutre
                End Get
                Set(ByVal value As String)
                    _eMailAutre = value
                End Set
            End Property

            ''' <summary>
            ''' Numéro de telephone fixe du user
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property TelFixe() As String
                Get
                    Return _TelFixe
                End Get
                Set(ByVal value As String)
                    _TelFixe = value
                End Set
            End Property

            ''' <summary>
            ''' Numéro de téléphone mobile du user
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property TelMobile() As String
                Get
                    Return _TelMobile
                End Get
                Set(ByVal value As String)
                    _TelMobile = value
                End Set
            End Property

            ''' <summary>
            ''' Numéro de téléphone divers pour user
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property TelAutre() As String
                Get
                    Return _TelAutre
                End Get
                Set(ByVal value As String)
                    _TelAutre = value
                End Set
            End Property

            ''' <summary>
            ''' Adresse du user
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Adresse() As String
                Get
                    Return _Adresse
                End Get
                Set(ByVal value As String)
                    _Adresse = value
                End Set
            End Property

            ''' <summary>
            ''' Ville du user
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Ville() As String
                Get
                    Return _Ville
                End Get
                Set(ByVal value As String)
                    _Ville = value
                End Set
            End Property

            ''' <summary>
            ''' Code Postal du user
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property CodePostal() As String
                Get
                    Return _CodePostal
                End Get
                Set(ByVal value As String)
                    _CodePostal = value
                End Set
            End Property
        End Class
    End Class
End Namespace