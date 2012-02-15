
Namespace HoMIDom
    ''' <summary>
    ''' Class pour gérer les télécommandes
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Telecommande
        Public Class Template
            Dim _Fabricant As String = ""
            Dim _Modele As String = ""
            Dim _Driver As String = ""
            Dim _File As String = ""

            ''' <summary>
            ''' Définit ou retourne le fabricant de l'équipement
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Fabricant As String
                Get
                    Return _Fabricant
                End Get
                Set(ByVal value As String)
                    _Fabricant = value
                End Set
            End Property

            ''' <summary>
            ''' Définit ou retourne le modèle d'équipement
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Modele As String
                Get
                    Return _Modele
                End Get
                Set(ByVal value As String)
                    _Modele = value
                End Set
            End Property

            ''' <summary>
            ''' Définit ou retourne le type de driver (ex: http, ir, rs232...)
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Driver As String
                Get
                    Return _Driver
                End Get
                Set(ByVal value As String)
                    _Driver = value
                End Set
            End Property

            ''' <summary>
            ''' Donne le nom du fichier ainsi que son extension (ex: TOTO.xml)
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property File As String
                Get
                    Return _File
                End Get
                Set(ByVal value As String)
                    _File = value
                End Set
            End Property
        End Class

        ''' <summary>
        ''' Type d'équipement par défaut, pour définir ensuite les commandes de bases
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum TypeEquipement
            VIDE = 0
            TV = 1
            DVD = 2
            AUDIO = 3
            BOX = 4
        End Enum
    End Class
End Namespace