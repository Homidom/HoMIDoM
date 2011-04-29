Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports NCrontab
Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.Net.Mail

Namespace HoMIDom

    ''' <summary>Class Macro, Défini le type pour les macros</summary>
    ''' <remarks>_condition contient un tableau des conditions à vérifier pour lancer les actions si TRUE ou si FALSE</remarks>
    Public Class Macro
        'Déclaration des variables
        Dim _ID As String
        Dim _Nom As String
        Dim _Description As String
        Dim _Enable As Boolean
        Dim _Condition As ArrayList
        Dim _ActionTrue As ArrayList
        Dim _ActionFalse As ArrayList

        Dim _Server As Server
        Dim _Device As Device

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
        ''' Liste des conditions de la macro
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Condition As ArrayList
            Get
                Return _Condition
            End Get
            Set(ByVal value As ArrayList)
                _Condition = value
            End Set
        End Property

        ''' <summary>
        ''' Liste des actions si les conditions de la macro macro est True
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ActionTrue As ArrayList
            Get
                Return _ActionTrue
            End Get
            Set(ByVal value As ArrayList)
                _ActionTrue = value
            End Set
        End Property

        ''' <summary>
        ''' Liste des actions si les conditions de la macro macro est False
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ActionFalse As ArrayList
            Get
                Return _ActionFalse
            End Get
            Set(ByVal value As ArrayList)
                _ActionFalse = value
            End Set
        End Property

        ''' <summary>Execute une macro avec analyse des conditions</summary>
        ''' <remarks>lance les actions True ou False suivant le résultat des conditions</remarks>
        Public Sub Execute_avec_conditions()
            Try
                _Server.Log(TypeLog.DEBUG, TypeSource.SERVEUR, "Macro:Execute_avec_conditions", "Execution avec tests des conditions de " & Nom)
                'analyse des conditions
                If Analyse() = True Then
                    'actionsTRUE
                    If ActionTrue.Count <> 0 Then
                        Action(ActionTrue)
                    End If
                ElseIf ActionFalse.Count <> 0 Then
                    Action(ActionFalse)
                End If
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Macro:Execute_avec_conditions", ex.ToString)
            End Try
        End Sub

        ''' <summary>Execute une macro sans analyse des conditions</summary>
        ''' <remarks>lance les actions si TRUE</remarks>
        Public Sub Execute_sans_conditions()
            Try
                _Server.Log(TypeLog.DEBUG, TypeSource.SERVEUR, "Macro:Execute_sans_conditions", "Execution sans tests des conditions de " & Nom)
                'on ne teste pas, on lance direct les actions de True
                If ActionTrue.Count <> 0 Then
                    Action(ActionTrue)
                End If
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Macro:Execute_sans_conditions", ex.ToString)
            End Try
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

        'Declaration des variables
        Dim _ID As String = ""
        Dim _Nom As String = ""
        Dim _Description As String = ""
        Dim _Enable As Boolean = False
        Dim _Condition As String = ""
        Dim _Prochainedateheure As DateTime 'la date/heure de prochaine execution utile uniquement pour un type CRON
        Public Macro As ArrayList
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
        ''' Condition du trigger
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Condition As String
            Get
                Return _Condition
            End Get
            Set(ByVal value As String)
                _Condition = value
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
                If STRGS.Left(Condition, 5) = "cron_" Then
                    Dim conditions = STRGS.Split(Condition, "#")
                    'ex: CrontabSchedule.Parse("0 17-19 * * *")
                    Dim s = CrontabSchedule.Parse(conditions(1) & " " & conditions(2) & " " & conditions(3) & " " & conditions(4) & " " & conditions(5))
                    'recupere le prochain shedule
                    Dim nextcron = s.GetNextOccurrence(DateAndTime.Now)
                    If (conditions(0) <> "*" And conditions(0) <> "") Then nextcron = nextcron.AddSeconds(conditions(0))
                    prochainedateheure = nextcron.ToString("yyyy-MM-dd HH:mm:ss")
                    'recupere la liste des prochains shedule
                    'Dim nextcron = s.GetNextOccurrences(DateAndTime.Now, DateAndTime.Now.AddDays(1))
                    'For Each i In nextcron
                    '    MsgBox(i.ToString("yyyy-MM-dd HH:mm:ss"))
                    'Next
                Else
                    prochainedateheure = Nothing 'on le laisse à vide car Trigger type Device
                End If
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Trigger:cron_convertendate", ex.ToString)
            End Try
        End Sub

    End Class

    ''' <summary>Class action, Défini les actions à réaliser depuis les macros...</summary>
    ''' <remarks></remarks>
    Public Class action

        Dim _ID As String
        Dim _Nom As String
        Dim _Description As String
        Dim _Enable As Boolean
        Dim _Type As String
        Public _Server As Server

        ''' <summary>convertit la condition au format cron "cron_ss#mm#hh#jj#MMM#JJJ" en dateTime dans le champ prochainedateheure</summary>
        ''' <remarks></remarks>
        Public Sub execute(ByVal param As ArrayList)
            Try

            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Action:xxx", ex.ToString)
            End Try
        End Sub

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

        Public Sub Pause(ByVal Heure As Integer, ByVal Minute As Integer, ByVal Seconde As Integer, ByVal Milliseconde As Integer)
            Dim t As DateTime = DateTime.Now
            t = t.AddHours(Heure)
            t = t.AddMinutes(Minute)
            t = t.AddSeconds(Seconde)
            t = t.AddMilliseconds(Milliseconde)
            Do While DateTime.Now < t

            Loop
        End Sub



        Public Sub Send_log(ByVal texte As String)

        End Sub

    End Class


End Namespace

