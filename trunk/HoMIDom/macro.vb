Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports NCrontab
Imports STRGS = Microsoft.VisualBasic.Strings

Namespace HoMIDom

    ''' <summary>Class Macro, Défini le type pour les macros</summary>
    ''' <remarks>_condition contient un tableau des conditions à vérifier pour lancer les actions si TRUE ou si FALSE</remarks>
    Public Class Macro

        Dim ID As String
        Dim Nom As String
        Dim Description As String
        Dim Enable As Boolean
        Dim Condition As ArrayList
        Dim ActionTrue As ArrayList
        Dim ActionFalse As ArrayList

        Dim _Server As Server
        Dim _Device As Device

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
    ''' _condition contien un tableau de DeviceID ou CRON : liste des déclencheurs du trigger
    ''' _macro contien un tableau de MacroID : liste des macros à lancer
    ''' </remarks>
    Public Class trigger

        Public ID As String
        Public Nom As String
        Public Description As String
        Public Enable As Boolean
        Public Condition As ArrayList
        Public Macro As ArrayList

        Public _Server As Server

        ''' <summary>Analyse si un device fait parti des conditions</summary>
        ''' <remarks></remarks>
        Public Function Analyse(ByVal DeviceId As String) As Boolean
            Try
                For i = 0 To Condition.Count - 1
                    If Condition.Item(0) = DeviceId Then Return True
                Next
                'device non trouvé dans la liste
                Return False
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Trigger:analyse", ex.ToString)
                Return False
            End Try
        End Function

    End Class

    ''' <summary>Class triggercron,permet uniquement d'etre utiliser dans le timer interne au server</summary>
    ''' <remarks></remarks>
    Public Class triggercron
        Public cron As String 'Le cron du trigger
        Public prochainedateheure As DateTime 'la date/heure de prochaine execution
        Public TriggerID As String 'l'id du trigger pour lancer les macros associés

        Public _Server As Server

        ''' <summary>Converti un cron en prochaine date/heure d'execution</summary>
        ''' <remarks></remarks>
        Public Sub maj_cron()
            'convertit la condition au format cron "cron_ss#mm#hh#jj#MMM#JJJ" en dateTime
            Try
                Dim conditions = STRGS.Split(cron, "#")

                'Dim s = CrontabSchedule.Parse("0 17-19 * * *")
                Dim s = CrontabSchedule.Parse(conditions(1) & " " & conditions(2) & " " & conditions(3) & " " & conditions(4) & " " & conditions(5))
                'recupere le prochain shedule
                Dim nextcron = s.GetNextOccurrence(DateAndTime.Now)
                If (conditions(0) <> "*" And conditions(0) <> "") Then nextcron = nextcron.AddSeconds(conditions(0))
                cron = nextcron.ToString("yyyy-MM-dd HH:mm:ss")

                'recupere la liste des prochains shedule
                'Dim nextcron = s.GetNextOccurrences(DateAndTime.Now, DateAndTime.Now.AddDays(1))
                'For Each i In nextcron
                '    MsgBox(i.ToString("yyyy-MM-dd HH:mm:ss"))
                'Next
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Trigger:cron_convertendate", ex.ToString)
            End Try
        End Sub
    End Class

End Namespace

