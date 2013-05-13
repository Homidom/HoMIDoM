Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports NCrontab
Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.Net.Mail
Imports System.Threading

Namespace HoMIDom

    ''' <summary>Class Macro, Défini le type pour les macros</summary>
    ''' <remarks>_condition contient un tableau des conditions à vérifier pour lancer les actions si TRUE ou si FALSE</remarks>
    <Serializable()> Public Class Macro
        'Déclaration des variables
        Dim _ID As String = ""
        Dim _Nom As String = ""
        Dim _Description As String = ""
        Dim _Enable As Boolean = False
        Dim _ListActions As New ArrayList
        <NonSerialized()> Dim _ListThread As New List(Of Thread)
        <NonSerialized()> Public _Server As Server

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
        Public Property ListActions As ArrayList
            Get
                Return _ListActions
            End Get
            Set(ByVal value As ArrayList)
                _ListActions = value
            End Set
        End Property

        ''' <summary>
        ''' Exécuter la macro
        ''' </summary>
        ''' <param name="Server"></param>
        ''' <remarks></remarks>
        Public Sub Execute(ByVal Server As Server)
            _Server = Server
            Try

                For i As Integer = 0 To _ListActions.Count - 1
                    'If _ListActions.Item(i)._Action.TypeAction = Action.TypeAction.ActionStop Then
                    If _ListActions.Item(i).TypeAction <> Action.TypeAction.ActionStop Then
                        Dim _action As New ThreadAction(_Server, _ListActions.Item(i))
                        Dim y As New Thread(AddressOf _action.Execute)
                        y.Name = "Traitement du script"
                        y.Start()
                        _ListThread.Add(y)
                        y = Nothing
                    Else
                        If _ListThread.Count > 0 Then
                            For Each thr In _ListThread
                                thr.Abort()
                            Next
                        End If
                    End If
                Next
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.SCRIPT, "Execute macro", ex.ToString)
            End Try
        End Sub
    End Class

    ''' <summary>Class trigger, Défini le type pour les triggers Device/timers</summary>
    ''' <remarks>
    ''' _condition contient un string: DeviceID ou CRON : le déclencheur du trigger
    ''' _macro contient un tableau de string : MacroID : liste des macros à lancer
    ''' </remarks>
    <Serializable()> Public Class Trigger

        ''' <summary>
        ''' Type de trigger TIME ou DEVICE
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum TypeTrigger
            ''' <summary>
            ''' Trigger sur une date/heure
            ''' </summary>
            ''' <remarks></remarks>
            TIMER = 0
            ''' <summary>
            ''' Trigger suivant une propriété d'un device
            ''' </summary>
            ''' <remarks></remarks>
            DEVICE = 1
        End Enum

        'Declaration des variables
        Dim _ID As String = ""
        Dim _Nom As String = ""
        Dim _Description As String = " "
        Dim _Type As TypeTrigger
        Dim _Enable As Boolean = False
        Dim _ConditionTime As String = ""
        Dim _ConditionDeviceId As String = ""
        Dim _ConditionDeviceProperty As String = ""
        Dim _Prochainedateheure As DateTime 'la date/heure de prochaine execution utile uniquement pour un type CRON
        Dim _ListMacro As New List(Of String)
        <NonSerialized()> Public _Server As Server

        'Propriétés
        ''' <summary>
        ''' ID du trigger
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
        ''' Nom du trigger
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
        ''' Description du trigger
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
        ''' Type du trigger 0=Time 1=Device
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Type As TypeTrigger
            Get
                Return _Type
            End Get
            Set(ByVal value As TypeTrigger)
                _Type = value
            End Set
        End Property

        ''' <summary>
        ''' Active de trigger
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
        ''' Condition du trigger suivant Date/Time
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ConditionTime As String
            Get
                Return _ConditionTime
            End Get
            Set(ByVal value As String)
                If _ConditionTime <> value Then
                    If ConditionTime <> value Then
                        _ConditionTime = value
                        maj_cron()
                    End If
                Else
                    _ConditionTime = value
                End If
            End Set
        End Property

        ''' <summary>
        ''' Id du device de la condition
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ConditionDeviceId As String
            Get
                Return _ConditionDeviceId
            End Get
            Set(ByVal value As String)
                _ConditionDeviceId = value
            End Set
        End Property

        ''' <summary>
        ''' Nom de la Propriété du device à surveillé pour le trigger
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ConditionDeviceProperty As String
            Get
                Return _ConditionDeviceProperty
            End Get
            Set(ByVal value As String)
                _ConditionDeviceProperty = value
            End Set
        End Property

        ''' <summary>
        ''' DateHeure de la prochaine exécution
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Prochainedateheure As DateTime
            Get
                Return _Prochainedateheure
            End Get
            Set(ByVal value As DateTime)
                _Prochainedateheure = value
            End Set
        End Property

        ''' <summary>
        ''' Liste des macros à déclencher
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ListMacro As List(Of String)
            Get
                Return _ListMacro
            End Get
            Set(ByVal value As List(Of String))
                _ListMacro = value
            End Set
        End Property
        ' ''' <summary>Analyse si un device fait parti des conditions</summary>
        ' ''' <remarks></remarks>
        'Public Function Analyse(ByVal DeviceId As String) As Boolean
        '    Try
        '        If Condition = DeviceId Then Return True 'device trouvé dans la liste
        '        Return False 'device non trouvé dans la liste
        '    Catch ex As Exception
        '        _Server.Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Trigger:analyse", ex.ToString)
        '        Return False
        '    End Try
        'End Function

        ''' <summary>convertit la condition au format cron "cron_ss#mm#hh#jj#MMM#JJJ" en dateTime dans le champ prochainedateheure</summary>
        ''' <remarks></remarks>
        Public Sub maj_cron()
            'convertit la condition au format cron "cron_ss#mm#hh#jj#MMM#JJJ" en dateTime
            Try
                'on vérifie si la condition est un cron
                If STRGS.Left(_ConditionTime, 5) = "_cron" Then
                    Dim conditions() As String = STRGS.Split(_ConditionTime, "#")
                    conditions(0) = Mid(conditions(0), 6, Len(conditions(0)) - 5)
                    'ex: CrontabSchedule.Parse("0 17-19 * * *")
                    Dim s = CrontabSchedule.Parse(conditions(1) & " " & conditions(2) & " " & conditions(3) & " " & conditions(4) & " " & conditions(5))
                    'recupere le prochain shedule
                    Dim nextcron = s.GetNextOccurrence(DateAndTime.Now)
                    If (conditions(0) <> "*" And conditions(0) <> "") Then nextcron = nextcron.AddSeconds(conditions(0))
                    Prochainedateheure = nextcron.ToString("yyyy-MM-dd HH:mm:ss")
                Else
                    'Prochainedateheure = Nothing 'on le laisse à vide car Trigger type Device
                End If
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Trigger:cron_convertendate", ex.ToString)
            End Try
        End Sub

    End Class

    ''' <summary>Class action, Défini les actions à réaliser depuis les macros...</summary>
    ''' <remarks></remarks>
    <Serializable()> Public Class Action
        <NonSerialized()> Public _Server As Server

        ''' <summary>
        ''' Enumération des types d'actions
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum TypeAction
            ''' <summary>
            ''' Action de type Device
            ''' </summary>
            ''' <remarks></remarks>
            ActionDevice = 0
            ''' <summary>
            ''' Action de type envoyer un mail
            ''' </summary>
            ''' <remarks></remarks>
            ActionMail = 1
            ''' <summary>
            ''' Action de type If Then/else
            ''' </summary>
            ''' <remarks></remarks>
            ActionIf = 2
            ''' <summary>
            ''' Action de type lancer une macro
            ''' </summary>
            ''' <remarks></remarks>
            ActionMacro = 3
            ''' <summary>
            ''' Action de type parler
            ''' </summary>
            ''' <remarks></remarks>
            ActionSpeech = 4

            ''' <summary>
            ''' Action de type commande http
            ''' </summary>
            ''' <remarks></remarks>
            ActionHttp = 5

            ''' <summary>
            ''' Action de type log windows
            ''' </summary>
            ''' <remarks></remarks>
            ActionLogEvent = 6

            ''' <summary>
            ''' Action de type log Homidom
            ''' </summary>
            ''' <remarks></remarks>
            ActionLogEventHomidom = 7

            ''' <summary>
            ''' Action de type log Commnde Dos
            ''' </summary>
            ''' <remarks></remarks>
            ActionDOS = 8

            ''' <summary>
            ''' Action de type ScriptVB
            ''' </summary>
            ''' <remarks></remarks>
            ActionVB = 9

            ''' <summary>
            ''' Action de type ScriptVB
            ''' </summary>
            ''' <remarks></remarks>
            ActionStop = 10
        End Enum

        ''' <summary>
        ''' Enumération des types  de signe (inferieur,egal,different...)
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum TypeSigne
            ''' <summary>
            ''' Egal
            ''' </summary>
            ''' <remarks></remarks>
            Egal = 0
            ''' <summary>
            ''' Inférieur
            ''' </summary>
            ''' <remarks></remarks>
            Inferieur = 1
            ''' <summary>
            ''' Inférieur ou égal
            ''' </summary>
            ''' <remarks></remarks>
            InferieurEgal = 2
            ''' <summary>
            ''' Supérieur
            ''' </summary>
            ''' <remarks></remarks>
            Superieur = 3
            ''' <summary>
            ''' Supérieur ou égal
            ''' </summary>
            ''' <remarks></remarks>
            SuperieurEgal = 4
            ''' <summary>
            ''' Différent de
            ''' </summary>
            ''' <remarks></remarks>
            Different = 5
        End Enum

        ''' <summary>
        ''' Enumération des types d'operateur
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum TypeOperateur
            ''' <summary>
            ''' Aucun
            ''' </summary>
            ''' <remarks></remarks>
            NONE = 0
            ''' <summary>
            ''' AND
            ''' </summary>
            ''' <remarks></remarks>
            [AND] = 1
            ''' <summary>
            ''' OR
            ''' </summary>
            ''' <remarks></remarks>
            [OR] = 2

        End Enum

        ''' <summary>
        ''' Enumération des types de conditions
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum TypeCondition
            ''' <summary>
            ''' Condition du type Date et heure
            ''' </summary>
            ''' <remarks></remarks>
            DateTime = 0
            ''' <summary>
            ''' Condition portant sur une propriété d'un device
            ''' </summary>
            ''' <remarks></remarks>
            Device = 1
            ''' <summary>
            ''' Parenthèse (non utilisé)
            ''' </summary>
            ''' <remarks></remarks>
            Parenthese = 2
        End Enum

        ''' <summary>
        ''' Action Device
        ''' </summary>
        ''' <remarks></remarks>
        <Serializable()> Public Class ActionDevice
            Dim _IdDevice As String
            Dim _Method As String
            Dim _Parametres As New ArrayList
            Dim _Timing As DateTime

            Public Property Timing As DateTime
                Get
                    Return _Timing
                End Get
                Set(ByVal value As DateTime)
                    _Timing = value
                End Set
            End Property

            Public Property IdDevice As String
                Get
                    Return _IdDevice
                End Get
                Set(ByVal value As String)
                    _IdDevice = value
                End Set
            End Property

            Public Property Method As String
                Get
                    Return _Method
                End Get
                Set(ByVal value As String)
                    _Method = value
                End Set
            End Property

            Public Property Parametres As ArrayList
                Get
                    Return _Parametres
                End Get
                Set(ByVal value As ArrayList)
                    _Parametres = value
                End Set
            End Property

            Public ReadOnly Property TypeAction As TypeAction
                Get
                    Return TypeAction.ActionDevice
                End Get
            End Property
        End Class

        ''' <summary>
        ''' Action Mail
        ''' </summary>
        ''' <remarks></remarks>
        <Serializable()> Public Class ActionMail
            Dim _Sujet As String
            Dim _Message As String
            Dim _Timing As DateTime
            Dim _UserId As String

            Public Property Timing As DateTime
                Get
                    Return _Timing
                End Get
                Set(ByVal value As DateTime)
                    _Timing = value
                End Set
            End Property

            Public Property Sujet As String
                Get
                    Return _Sujet
                End Get
                Set(ByVal value As String)
                    _Sujet = value
                End Set
            End Property

            Public Property Message As String
                Get
                    Return _Message
                End Get
                Set(ByVal value As String)
                    _Message = value
                End Set
            End Property

            Public Property UserId As String
                Get
                    Return _UserId
                End Get
                Set(ByVal value As String)
                    _UserId = value
                End Set
            End Property

            Public ReadOnly Property TypeAction As TypeAction
                Get
                    Return TypeAction.ActionMail
                End Get
            End Property
        End Class

        ''' <summary>
        ''' Action If
        ''' </summary>
        ''' <remarks></remarks>
        Public Class ActionIf
            Dim _Conditions As New List(Of Condition)
            Dim _ListTrue As New ArrayList
            Dim _ListFalse As New ArrayList
            Dim _Timing As DateTime

            Public Property Timing As DateTime
                Get
                    Return _Timing
                End Get
                Set(ByVal value As DateTime)
                    _Timing = value
                End Set
            End Property

            Public Property Conditions As List(Of Condition)
                Get
                    Return _Conditions
                End Get
                Set(ByVal value As List(Of Condition))
                    _Conditions = value
                End Set
            End Property

            Public Property ListTrue As ArrayList
                Get
                    Return _ListTrue
                End Get
                Set(ByVal value As ArrayList)
                    _ListTrue = value
                End Set
            End Property

            Public Property ListFalse As ArrayList
                Get
                    Return _ListFalse
                End Get
                Set(ByVal value As ArrayList)
                    _ListFalse = value
                End Set
            End Property

            Public ReadOnly Property TypeAction As TypeAction
                Get
                    Return TypeAction.ActionIf
                End Get
            End Property

            Public ReadOnly Property FormatNCalc As String
                Get
                    Dim _FormatNCalc As String = " "
                    For i As Integer = 0 To _Conditions.Count - 1
                        Select Case _Conditions.Item(i).Type
                            Case TypeCondition.DateTime

                            Case TypeCondition.Device

                        End Select
                    Next
                    Return _FormatNCalc
                End Get
            End Property

        End Class

        ''' <summary>
        ''' Action macro
        ''' </summary>
        ''' <remarks></remarks>
        Public Class ActionMacro
            Dim _IdMacro As String
            Dim _Timing As DateTime

            Public Property Timing As DateTime
                Get
                    Return _Timing
                End Get
                Set(ByVal value As DateTime)
                    _Timing = value
                End Set
            End Property

            Public Property IdMacro As String
                Get
                    Return _IdMacro
                End Get
                Set(ByVal value As String)
                    _IdMacro = value
                End Set
            End Property

            Public ReadOnly Property TypeAction As TypeAction
                Get
                    Return TypeAction.ActionMacro
                End Get
            End Property
        End Class

        ''' <summary>
        ''' Condition pour action If
        ''' </summary>
        ''' <remarks></remarks>
        Public Class Condition
            Dim _Type As TypeCondition
            Dim _DateTime As String
            Dim _IdDevice As String
            Dim _PropertyDevice As String
            Dim _Value As Object
            Dim _Condition As TypeSigne
            Dim _Operateur As TypeOperateur

            Public Property Type As TypeCondition
                Get
                    Return _Type
                End Get
                Set(ByVal value As TypeCondition)
                    _Type = value
                End Set
            End Property

            Public Property DateTime As String
                Get
                    Return _DateTime
                End Get
                Set(ByVal value As String)
                    _DateTime = value
                End Set
            End Property

            Public Property IdDevice As String
                Get
                    Return _IdDevice
                End Get
                Set(ByVal value As String)
                    _IdDevice = value
                End Set
            End Property

            Public Property PropertyDevice As String
                Get
                    Return _PropertyDevice
                End Get
                Set(ByVal value As String)
                    _PropertyDevice = value
                End Set
            End Property

            Public Property Value As Object
                Get
                    Return _Value
                End Get
                Set(ByVal value As Object)
                    _Value = value
                End Set
            End Property

            Public Property Condition As TypeSigne
                Get
                    Return _Condition
                End Get
                Set(ByVal value As TypeSigne)
                    _Condition = value
                End Set
            End Property

            Public Property Operateur As TypeOperateur
                Get
                    Return _Operateur
                End Get
                Set(ByVal value As TypeOperateur)
                    _Operateur = value
                End Set
            End Property

        End Class

        ''' <summary>
        ''' Action parler
        ''' </summary>
        ''' <remarks></remarks>
        Public Class ActionSpeech
            Dim _Message As String
            Dim _Timing As DateTime

            Public Property Timing As DateTime
                Get
                    Return _Timing
                End Get
                Set(ByVal value As DateTime)
                    _Timing = value
                End Set
            End Property

            Public Property Message As String
                Get
                    Return _Message
                End Get
                Set(ByVal value As String)
                    _Message = value
                End Set
            End Property

            Public ReadOnly Property TypeAction As TypeAction
                Get
                    Return TypeAction.ActionSpeech
                End Get
            End Property

        End Class

        ''' <summary>
        ''' Action parler
        ''' </summary>
        ''' <remarks></remarks>
        Public Class ActionHttp
            Dim _Commande As String
            Dim _Timing As DateTime

            Public Property Timing As DateTime
                Get
                    Return _Timing
                End Get
                Set(ByVal value As DateTime)
                    _Timing = value
                End Set
            End Property

            Public Property Commande As String
                Get
                    Return _Commande
                End Get
                Set(ByVal value As String)
                    _Commande = value
                End Set
            End Property

            Public ReadOnly Property TypeAction As TypeAction
                Get
                    Return TypeAction.ActionHttp
                End Get
            End Property

        End Class

        ''' <summary>
        ''' Action Log windows
        ''' </summary>
        ''' <remarks></remarks>
        Public Class ActionLogEvent
            Dim _Message As String
            Dim _type As TypeEventLog
            Dim _eventid As Integer = 0
            Dim _Timing As DateTime

            Public Property Timing As DateTime
                Get
                    Return _Timing
                End Get
                Set(ByVal value As DateTime)
                    _Timing = value
                End Set
            End Property

            Public Property Message As String
                Get
                    Return _Message
                End Get
                Set(ByVal value As String)
                    _Message = value
                End Set
            End Property

            Public Property Type As TypeEventLog
                Get
                    Return _type
                End Get
                Set(ByVal value As TypeEventLog)
                    _type = value
                End Set
            End Property

            Public Property Eventid As Integer
                Get
                    Return _eventid
                End Get
                Set(ByVal value As Integer)
                    _eventid = value
                End Set
            End Property

            Public ReadOnly Property TypeAction As TypeAction
                Get
                    Return TypeAction.ActionLogEvent
                End Get
            End Property

        End Class

        ''' <summary>
        ''' Action Log Homidom
        ''' </summary>
        ''' <remarks></remarks>
        Public Class ActionLogEventHomidom
            Dim _Message As String
            Dim _type As HoMIDom.Server.TypeLog = TypeLog.INFO
            Dim _fonction As String
            Dim _Timing As DateTime

            Public Property Timing As DateTime
                Get
                    Return _Timing
                End Get
                Set(ByVal value As DateTime)
                    _Timing = value
                End Set
            End Property

            Public Property Message As String
                Get
                    Return _Message
                End Get
                Set(ByVal value As String)
                    _Message = value
                End Set
            End Property

            Public Property Type As HoMIDom.Server.TypeLog
                Get
                    Return _type
                End Get
                Set(ByVal value As HoMIDom.Server.TypeLog)
                    _type = value
                End Set
            End Property

            Public Property Fonction As String
                Get
                    Return _fonction
                End Get
                Set(ByVal value As String)
                    _fonction = value
                End Set
            End Property

            Public ReadOnly Property TypeAction As TypeAction
                Get
                    Return TypeAction.ActionLogEventHomidom
                End Get
            End Property

        End Class

        ''' <summary>
        ''' Action Dos 
        ''' </summary>
        ''' <remarks></remarks>
        Public Class ActionDos
            Dim _Fichier As String
            Dim _Arguments As String
            Dim _Timing As DateTime

            Public Property Timing As DateTime
                Get
                    Return _Timing
                End Get
                Set(ByVal value As DateTime)
                    _Timing = value
                End Set
            End Property

            Public Property Fichier As String
                Get
                    Return _Fichier
                End Get
                Set(ByVal value As String)
                    _Fichier = value
                End Set
            End Property

            Public Property Arguments As String
                Get
                    Return _Arguments
                End Get
                Set(ByVal value As String)
                    _Arguments = value
                End Set
            End Property

            Public ReadOnly Property TypeAction As TypeAction
                Get
                    Return TypeAction.ActionDOS
                End Get
            End Property

        End Class

        ''' <summary>
        ''' Action Dos 
        ''' </summary>
        ''' <remarks></remarks>
        Public Class ActionVB
            Dim _Label As String 'Description du code
            Dim _Script As String 'Code script à executer
            Dim _Timing As DateTime

            Public Property Timing As DateTime
                Get
                    Return _Timing
                End Get
                Set(ByVal value As DateTime)
                    _Timing = value
                End Set
            End Property

            Public Property Script As String
                Get
                    Return _Script
                End Get
                Set(ByVal value As String)
                    _Script = value
                End Set
            End Property

            Public Property Label As String
                Get
                    Return _Label
                End Get
                Set(ByVal value As String)
                    _Label = value
                End Set
            End Property

            Public ReadOnly Property TypeAction As TypeAction
                Get
                    Return TypeAction.ActionVB
                End Get
            End Property

        End Class

        ''' <summary>log dans les fichiers logs et base sqlite</summary>
        ''' <remarks></remarks>
        Public Class log
            Public Sub execute(ByVal texte As String)
                'envoi de l'email à adresse avec sujet et texte via les smtp définis dans le serveur

            End Sub
        End Class

        Public Sub Send_log(ByVal texte As String)

        End Sub

        ''' <summary>
        ''' Action Dos 
        ''' </summary>
        ''' <remarks></remarks>
        Public Class ActionSTOP
            Dim _Timing As DateTime

            Public ReadOnly Property TypeAction As TypeAction
                Get
                    Return TypeAction.ActionStop
                End Get
            End Property

            Public Property Timing As DateTime
                Get
                    Return _Timing
                End Get
                Set(ByVal value As DateTime)
                    _Timing = value
                End Set
            End Property
        End Class
    End Class

End Namespace

