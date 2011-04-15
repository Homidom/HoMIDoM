Imports HoMIDom
Imports HoMIDom.HoMIDom.Server

Namespace HoMIDom

    ''' <summary>Class Macro, Défini le type pour les macros</summary>
    ''' <remarks>_condition contient un tableau des conditions à vérifier pour lancer les actions si TRUE ou si FALSE</remarks>
    Public Class macro

        Dim _ID As String
        Dim _Nom As String
        Dim _Description As String
        Dim _Enable As Boolean
        Dim _Condition As ArrayList
        Dim _ActionTrue As ArrayList
        Dim _ActionFalse As ArrayList

        Dim _Server As Server
        Dim _Device As Device

        Public ReadOnly Property ID As String
            Get
                Return _ID
            End Get
        End Property
        Public Property Nom As String
            Get
                Return _Nom
            End Get
            Set(ByVal value As String)
                _Nom = value
            End Set
        End Property
        Public Property Description As String
            Get
                Return _Description
            End Get
            Set(ByVal value As String)
                _Description = value
            End Set
        End Property
        Public Property Enable As Boolean
            Get
                Return _Enable
            End Get
            Set(ByVal value As Boolean)
                _Enable = value
            End Set
        End Property
        Public Property Condition As ArrayList
            Get
                Return _Condition
            End Get
            Set(ByVal value As ArrayList)
                _Condition = value
            End Set
        End Property
        Public Property ActionTrue As ArrayList
            Get
                Return _ActionTrue
            End Get
            Set(ByVal value As ArrayList)
                _ActionTrue = value
            End Set
        End Property
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
                'analyse des conditions
                If Analyse() = True Then
                    'actionsTRUE
                    If _ActionTrue.Count <> 0 Then
                        Action(_ActionTrue)
                    End If
                ElseIf _ActionFalse.Count <> 0 Then
                    Action(_ActionFalse)
                End If
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Macro:Execute_avec_conditions", ex.ToString)
            End Try
        End Sub

        ''' <summary>Execute une macro sans analyse des conditions</summary>
        ''' <remarks>lance les actions si TRUE</remarks>
        Public Sub Execute_sans_conditions()
            Try
                If _ActionTrue.Count <> 0 Then
                    Action(_ActionTrue)
                End If
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Macro:Execute_sans_conditions", ex.ToString)
            End Try
        End Sub

        ''' <summary>Analyse les conditions et renvoie le résultat</summary>
        ''' <remarks></remarks>
        Public Function Analyse() As Boolean
            Try



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

        Dim _ID As String
        Dim _Nom As String
        Dim _Description As String
        Dim _Enable As Boolean
        Dim _Condition As ArrayList
        Dim _Macro As ArrayList

        Dim _Server As Server

        Public ReadOnly Property ID As String
            Get
                Return _ID
            End Get
        End Property
        Public Property Nom As String
            Get
                Return _Nom
            End Get
            Set(ByVal value As String)
                _Nom = value
            End Set
        End Property
        Public Property Description As String
            Get
                Return _Description
            End Get
            Set(ByVal value As String)
                _Description = value
            End Set
        End Property
        Public Property Enable As Boolean
            Get
                Return _Enable
            End Get
            Set(ByVal value As Boolean)
                _Enable = value
            End Set
        End Property
        Public Property Condition As ArrayList
            Get
                Return _Condition
            End Get
            Set(ByVal value As ArrayList)
                _Condition = value
            End Set
        End Property
        Public Property Macro As ArrayList
            Get
                Return _Macro
            End Get
            Set(ByVal value As ArrayList)
                _Macro = value
            End Set
        End Property

        ''' <summary>Analyse si un device fait parti des conditions</summary>
        ''' <remarks></remarks>
        Public Function Analyse(ByVal DeviceId As String) As Boolean
            Try
                For i = 0 To _Condition.Count - 1
                    If _Condition.Item(0) = DeviceId Then Return True
                Next
                'device non trouvé dans la liste
                Return False
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Trigger:analyse", ex.ToString)
                Return False
            End Try
        End Function

    End Class

End Namespace

