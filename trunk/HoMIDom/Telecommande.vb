Namespace HoMIDom
    ''' <summary>
    ''' Class pour gérer les télécommandes
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Telecommande
        Public Class Template

#Region "Variables"
            Dim _ID As String
            Dim _Name As String
            Dim _Fabricant As String
            Dim _Modele As String
            Dim _Driver As String
            Dim _File As String
            Dim _IsAudioVideo As Boolean = False
            Dim _Type As Integer = 0 '0:http 1:ir 2:rs232
            Dim _TrameInit As String 'trame d'initialisation
            Dim _CharEndReceive 'caractère de fin d'une trame reçue
            Dim _IP As String 'adresse IP
            Dim _Port As Integer 'Port IP
            Dim _Cmd As New List(Of Commandes)
            Dim _Var As New List(Of TemplateVar)
            Dim _GraficTemplate As New GraficTemplate
            Dim _DefautCmdAudioVideo() As String = {"PLAY", "PAUSE", "STOP", "POWER", "AVANCE", "RECUL", "NEXTCHAPITRE", "PREVIOUSCHAPITRE", "OK", "VOLUMEUP", "VOLUMEDOWN", "MUTE", "FLECHEHAUT", "FLECHEBAS", "FLECHEGAUCHE", "FLECHEDROITE", "ENREGISTRER", "BLUE", "RED", "GREEN", "YELLOW", "CHANNELUP", "CHANNELDOWN"}
#End Region

#Region "Property"

            Public Property ID As String
                Get
                    Return _ID
                End Get
                Set(ByVal value As String)
                    _ID = value
                End Set
            End Property

            ''' <summary>
            ''' Nom du template
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Name As String
                Get
                    Return _Name
                End Get
                Set(ByVal value As String)
                    _Name = value
                End Set
            End Property

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

            ''' <summary>
            ''' Type de driver 0=http 1=IR 2=RS232, par défaut 0
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Type As Integer
                Get
                    Return _Type
                End Get
                Set(ByVal value As Integer)
                    Dim val As Integer = value
                    If val < 0 Then val = 0
                    If val > 2 Then val = 2
                    _Type = val
                End Set
            End Property

            ''' <summary>
            ''' Définit si de base le template est un équipement audio/video si oui on a le minimum 
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property IsAudioVideo As Boolean
                Get
                    Return _IsAudioVideo
                End Get
                Set(value As Boolean)
                    _IsAudioVideo = value
                End Set
            End Property

            Public Sub InitCmd()
                _Cmd.Clear()
                If _IsAudioVideo Then
                    If _Cmd.Count = 0 Then
                        Dim idx As Integer
                        For idx = 0 To 9
                            Dim _newcmd As New Commandes
                            _newcmd.Name = idx
                            _Cmd.Add(_newcmd)
                        Next
                        For Each _lblcmd In _DefautCmdAudioVideo
                            Dim _newcmd As New Commandes
                            _newcmd.Name = _lblcmd
                            _Cmd.Add(_newcmd)
                        Next
                    End If
                End If
            End Sub

            ''' <summary>
            ''' Trame d'initialisation à envoyer avant tout envoi de commande
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property TrameInit As String
                Get
                    Return _TrameInit
                End Get
                Set(ByVal value As String)
                    _TrameInit = value
                End Set
            End Property

            ''' <summary>
            ''' Caractère(s) séparateur(s) entre chaque trame pour les différencier
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property CharEndReceive As String
                Get
                    Return _CharEndReceive
                End Get
                Set(ByVal value As String)
                    _CharEndReceive = value
                End Set
            End Property

            ''' <summary>
            ''' Liste des commandes associées au template
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Commandes As List(Of Commandes)
                Get
                    Return _Cmd
                End Get
                Set(ByVal value As List(Of Commandes))
                    _Cmd = value
                End Set
            End Property

            ''' <summary>
            ''' Liste des variables associées au template
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Variables As List(Of TemplateVar)
                Get
                    Return _Var
                End Get
                Set(ByVal value As List(Of TemplateVar))
                    _Var = value
                End Set
            End Property

            ''' <summary>
            ''' Adresse IP du template
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property IP As String
                Get
                    Return _IP
                End Get
                Set(ByVal value As String)
                    _IP = value

                    If String.IsNullOrEmpty(_IP) = False Then
                        For Each var In _Var
                            If var.Name.ToLower = "ip" Then
                                If String.IsNullOrEmpty(_Port) Then
                                    var.Value = _IP
                                    Exit For
                                Else
                                    var.Value = _IP & ":" & _Port
                                    Exit For
                                End If
                            End If
                        Next
                    End If
                End Set
            End Property

            ''' <summary>
            ''' Port IP du template
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Port As Integer
                Get
                    Return _Port
                End Get
                Set(ByVal value As Integer)
                    _Port = value
                    IP = _IP
                End Set
            End Property

            Public Property GraphicTemplate As GraficTemplate
                Get
                    Return _GraficTemplate
                End Get
                Set(value As GraficTemplate)
                    _GraficTemplate = value
                End Set
            End Property
#End Region


            Public Function AddNewVar(ByVal Var As TemplateVar) As Integer
                Try

                Catch ex As Exception

                End Try
            End Function

            ''' <summary>
            ''' Démarrer le template
            ''' </summary>
            ''' <param name="idsrv"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function Start(ByVal idsrv As String) As String
                Try
                    'on verifie l'id du serveur pour la sécurité
                    If idsrv <> _IdSrv Then
                        Return "99"
                        Exit Function
                    End If

                    'On effectue l'init suivant 
                    Select Case _Type
                        Case 0 'Template de type http
                            'créer la connexion http

                            'si trame d'initialisation l'envoyer
                            If String.IsNullOrEmpty(_TrameInit) = False Then

                            End If

                        Case 1 'Template de type IR
                            'créer la connexion IR

                            'si trame d'initialisation l'envoyer
                            If String.IsNullOrEmpty(_TrameInit) = False Then

                            End If
                        Case 2 'Template de type rs232
                            'créer la connexion rs232

                            'si trame d'initialisation l'envoyer
                            If String.IsNullOrEmpty(_TrameInit) = False Then

                            End If
                    End Select

                    Return "0"
                Catch ex As Exception
                    Return ex.ToString
                End Try
            End Function

            ''' <summary>
            ''' Execute une commande suivant son nom
            ''' </summary>
            ''' <param name="idsrv"></param>
            ''' <param name="Name"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function ExecuteCommand(ByVal idsrv As String, ByVal Name As String, Server As Server) As String
                Try
                    'on verifie l'id du serveur pour la sécurité
                    If idsrv <> _IdSrv Then
                        Return "99"
                        Exit Function
                    End If

                    For Each cmd In _Cmd
                        If cmd.Name.ToLower = Name.ToLower Then 'on a trouvé la commande
                            Dim code As String = cmd.Code

                            'on remplace les variables mis en accolades par leur valeur
                            For Each var In _Var
                                If var.Type = TypeOfVar.String Or var.Type = TypeOfVar.Double Then
                                    Dim _var As String = "{" & var.Name.ToLower & "}"
                                    If code.ToLower.Contains(_var) Then
                                        code = code.Replace(_var, var.Value)
                                    End If
                                End If
                            Next

                            'Executer la commande...
                            If cmd.IsScript Then

                            End If

                            'On envoi la commande
                            Select Case _Type
                                Case 0 'Template de type http

                                Case 1 'Template de type IR

                                Case 2 'Template de type rs232

                            End Select
                        End If
                    Next

                    Return "0"
                Catch ex As Exception
                    Return ex.ToString
                End Try
            End Function

            ''' <summary>
            ''' Permet de définir la valeur d'une variable associée au template
            ''' </summary>
            ''' <param name="idsrv"></param>
            ''' <param name="namevar"></param>
            ''' <param name="Value"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function SetVariable(ByVal idsrv As String, ByVal namevar As String, ByVal Value As Object) As String
                Try
                    'on verifie l'id du serveur pour la sécurité
                    If idsrv <> _IdSrv Then
                        Return "99"
                        Exit Function
                    End If

                    For Each var In _Var
                        If var.Name.ToLower = namevar.ToLower Then
                            var.Value = Value
                            Exit For
                        End If
                    Next
                    Return "0"
                Catch ex As Exception
                    Return ex.ToString
                End Try
            End Function
        End Class

        Public Class Commandes
            Dim _Name As String
            Dim _Code As String
            Dim _IsScript As Boolean = False 'si false c'est une commande à envoyer si false c'est un script à exécuter
            Dim _Repeat As Integer = 0
            Dim _Picture As String

            'Variables graphiques
            Dim _Width As Double = 45
            Dim _Height As Double = 45
            Dim _X As Double = 0
            Dim _Y As Double = 0
            Dim _BackgroundPicture As String = ""
            Dim _ColorBackGround As Double 'SolidColorBrush = Brushes.Black

            Property Name As String
                Get
                    Return _Name
                End Get
                Set(ByVal value As String)
                    _Name = value
                End Set
            End Property

            Property Code As String
                Get
                    Return _Code
                End Get
                Set(ByVal value As String)
                    _Code = value
                End Set
            End Property

            Property IsScript As Boolean
                Get
                    Return _IsScript
                End Get
                Set(ByVal value As Boolean)
                    _IsScript = value
                End Set
            End Property

            Property Repeat As Integer
                Get
                    Return _Repeat
                End Get
                Set(ByVal value As Integer)
                    _Repeat = value
                End Set
            End Property

            Property Picture As String
                Get
                    Return _Picture
                End Get
                Set(ByVal value As String)
                    _Picture = value
                End Set
            End Property

        End Class

        Public Enum TypeOfVar
            [String] = 0
            [Double] = 1
            [Tableau] = 2
        End Enum

        ''' <summary>
        ''' Variable associées à un template pour y stocker des donnnées reçues
        ''' </summary>
        ''' <remarks></remarks>
        Public Class TemplateVar
            Dim _Name As String = ""
            Dim _Type As TypeOfVar = TypeOfVar.String
            Dim _ValString As String = ""
            Dim _ValDouble As Double = 0
            Dim _ValTableau As New ArrayList

            ''' <summary>
            ''' Nom de la variable
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Name As String
                Get
                    Return _Name
                End Get
                Set(ByVal value As String)
                    _Name = value
                End Set
            End Property

            ''' <summary>
            ''' Type de la variable
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Type As TypeOfVar
                Get
                    Return _Type
                End Get
                Set(ByVal value As TypeOfVar)
                    _Type = value
                End Set
            End Property

            ''' <summary>
            ''' Valeur associée à la variable
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Value As Object
                Get
                    Select Case _Type
                        Case TypeOfVar.String
                            Return _ValString
                        Case TypeOfVar.Double
                            Return _ValDouble
                        Case TypeOfVar.Tableau
                            Return _ValTableau
                        Case Else
                            Return _ValString
                    End Select
                End Get
                Set(ByVal value As Object)
                    Select Case _Type
                        Case TypeOfVar.String
                            _ValString = value
                        Case TypeOfVar.Double
                            _ValDouble = value
                        Case TypeOfVar.Tableau
                            _ValTableau = value
                        Case Else
                            _ValString = value
                    End Select
                End Set
            End Property
        End Class

#Region "Template Graphique"
        Public Class GraficTemplate

#Region "Variables"
            'Variables graphiques du template
            Dim _Width As Double
            Dim _Height As Double
            Dim _BackgroundPicture As String = ""
            Dim _ColorBackGround As Double
            Dim _Widgets As New List(Of Widget)
#End Region

#Region "Property Graphic"
            Public Property Width As Double
                Get
                    Return _Width
                End Get
                Set(ByVal value As Double)
                    _Width = value
                End Set
            End Property

            Public Property Height As Double
                Get
                    Return _Height
                End Get
                Set(ByVal value As Double)
                    _Height = value
                End Set
            End Property

            Public Property BackGroundPicture As String
                Get
                    Return _BackgroundPicture
                End Get
                Set(ByVal value As String)
                    _BackgroundPicture = value
                End Set
            End Property

            Public Property ColorBackGround As Double 'SolidColorBrush
                Get
                    Return _ColorBackGround
                End Get
                Set(ByVal value As Double) 'SolidColorBrush)
                    _ColorBackGround = value
                End Set
            End Property

            Public Property Widgets As List(Of Widget)
                Get
                    Return _Widgets
                End Get
                Set(value As List(Of Widget))
                    _Widgets = value
                End Set
            End Property
#End Region
        End Class

#End Region
    End Class
End Namespace