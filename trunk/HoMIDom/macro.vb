Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports NCrontab
Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.Net.Mail

Namespace HoMIDom

    ''' <summary>Class Macro, Défini le type pour les macros</summary>
    ''' <remarks>_condition contient un tableau des conditions à vérifier pour lancer les actions si TRUE ou si FALSE</remarks>
    <Serializable()> Public Class Macro
        'Déclaration des variables
        Dim _ID As String
        Dim _Nom As String
        Dim _Description As String
        Dim _Enable As Boolean
        Dim _ListActions As New ArrayList
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
        Public Property ListActions As ArrayList
            Get
                Return _ListActions
            End Get
            Set(ByVal value As ArrayList)
                _ListActions = value
            End Set
        End Property

        ''' <summary>Execute une macro avec analyse des conditions</summary>
        ''' <remarks>lance les actions True ou False suivant le résultat des conditions</remarks>
        Public Sub Execute_avec_conditions()
            'Try
            '    _Server.Log(TypeLog.DEBUG, TypeSource.SERVEUR, "Macro:Execute_avec_conditions", "Execution avec tests des conditions de " & Nom)
            '    'analyse des conditions
            '    If Analyse() = True Then
            '        'actionsTRUE
            '        If ActionTrue.Count <> 0 Then
            '            Action(ActionTrue)
            '        End If
            '    ElseIf ActionFalse.Count <> 0 Then
            '        Action(ActionFalse)
            '    End If
            'Catch ex As Exception
            '    _Server.Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Macro:Execute_avec_conditions", ex.ToString)
            'End Try
        End Sub

        ''' <summary>Execute une macro sans analyse des conditions</summary>
        ''' <remarks>lance les actions si TRUE</remarks>
        Public Sub Execute_sans_conditions()
            'Try
            '    _Server.Log(TypeLog.DEBUG, TypeSource.SERVEUR, "Macro:Execute_sans_conditions", "Execution sans tests des conditions de " & Nom)
            '    'on ne teste pas, on lance direct les actions de True
            '    If ActionTrue.Count <> 0 Then
            '        Action(ActionTrue)
            '    End If
            'Catch ex As Exception
            '    _Server.Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Macro:Execute_sans_conditions", ex.ToString)
            'End Try
        End Sub

        ''' <summary>Analyse les conditions et renvoie le résultat</summary>
        ''' <remarks></remarks>
        Public Function Analyse() As Boolean
            Try
                _Server.Log(TypeLog.DEBUG, TypeSource.SERVEUR, "Macro:Analyse", "Analyse des conditions de : " & Nom)


                'Ajouter le code Ncalc ou autre


                Return True



            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Macro:analyse", ex.ToString)
                Return False
            End Try
        End Function

        ''' <summary>Execute les actions</summary>
        ''' <remarks></remarks>
        Public Sub Action(ByVal listeactions As ArrayList)
            Try
                For i = 0 To listeactions.Count - 1
                    _Server.Log(TypeLog.DEBUG, TypeSource.SERVEUR, "Macro:Action", "Execution de l'action : " & listeactions.Item(i))


                Next
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Macro:Action", ex.ToString)
            End Try
        End Sub
    End Class

    ''' <summary>Class trigger, Défini le type pour les triggers Device/timers</summary>
    ''' <remarks>
    ''' _condition contient un string: DeviceID ou CRON : le déclencheur du trigger
    ''' _macro contient un tableau de string : MacroID : liste des macros à lancer
    ''' </remarks>
    Public Class Trigger
        Public Enum TypeTrigger
            TIMER = 0
            DEVICE = 1
        End Enum

        'Declaration des variables
        Dim _ID As String = ""
        Dim _Nom As String = ""
        Dim _Description As String = ""
        Dim _Type As TypeTrigger
        Dim _Enable As Boolean = False
        Dim _ConditionTime As String = ""
        Dim _ConditionDeviceId As String = ""
        Dim _ConditionDeviceProperty As String = ""
        Dim _Prochainedateheure As DateTime 'la date/heure de prochaine execution utile uniquement pour un type CRON
        Dim _ListMacro As New ArrayList
        Public _Server As Server

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
                _ConditionTime = value
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
        Public Property ListMacro As ArrayList
            Get
                Return _ListMacro
            End Get
            Set(ByVal value As ArrayList)
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
                If STRGS.Left(_ConditionTime, 5) = "cron_" Then
                    Dim conditions = STRGS.Split(_ConditionTime, "#")
                    'ex: CrontabSchedule.Parse("0 17-19 * * *")
                    Dim s = CrontabSchedule.Parse(conditions(1) & " " & conditions(2) & " " & conditions(3) & " " & conditions(4) & " " & conditions(5))
                    'recupere le prochain shedule
                    Dim nextcron = s.GetNextOccurrence(DateAndTime.Now)
                    If (conditions(0) <> "*" And conditions(0) <> "") Then nextcron = nextcron.AddSeconds(conditions(0))
                    Prochainedateheure = nextcron.ToString("yyyy-MM-dd HH:mm:ss")
                    'recupere la liste des prochains shedule
                    'Dim nextcron = s.GetNextOccurrences(DateAndTime.Now, DateAndTime.Now.AddDays(1))
                    'For Each i In nextcron
                    '    MsgBox(i.ToString("yyyy-MM-dd HH:mm:ss"))
                    'Next
                Else
                    Prochainedateheure = Nothing 'on le laisse à vide car Trigger type Device
                End If
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Trigger:cron_convertendate", ex.ToString)
            End Try
        End Sub

    End Class

    ''' <summary>Class action, Défini les actions à réaliser depuis les macros...</summary>
    ''' <remarks></remarks>
    <Serializable()> Public Class Action
        Public _Server As Server

        ''' <summary>
        ''' Enumération des types d'actions
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum TypeAction
            ActionDevice = 0
            ActionMail = 1
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

        ''' <summary>Envoi d'un email</summary>
        ''' <remarks></remarks>
        Public Class email
            Public adresse As String 'adresse email
            Public Sub execute(ByVal sujet As String, ByVal texte As String)
                'envoi de l'email à adresse avec sujet et texte via les smtp définis dans le serveur

            End Sub
        End Class

        ''' <summary>log dans les fichiers logs et base sqlite</summary>
        ''' <remarks></remarks>
        Public Class log
            Public Sub execute(ByVal texte As String)
                'envoi de l'email à adresse avec sujet et texte via les smtp définis dans le serveur

            End Sub
        End Class

        Public Sub Send_email(ByVal adresse As String, ByVal sujet As String, ByVal texte As String)
            'envoi de l'email à adresse avec sujet et texte via les smtp définis dans le serveur
            Dim email As System.Net.Mail.MailMessage = New System.Net.Mail.MailMessage()
            email.From = New MailAddress(_Server.GetSMTPMailServeur)
            email.To.Add(adresse)
            email.Subject = sujet
            email.Body = texte
            Dim mailSender As New System.Net.Mail.SmtpClient(_Server.GetSMTPServeur)
            If _Server.GetSMTPLogin() <> "" Then
                mailSender.Credentials = New Net.NetworkCredential(_Server.GetSMTPLogin, _Server.GetSMTPPassword)
                '
                'email.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", "1")
                'email.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", mMailServerLogin)
                'email.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", mMailServerPassword)
            End If
            mailSender.Send(email)
            email = Nothing
            mailSender = Nothing
        End Sub

        Public Sub Send_log(ByVal texte As String)

        End Sub

    End Class



End Namespace

