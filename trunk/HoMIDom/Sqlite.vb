Imports System.Data.SQLite
Imports HoMIDom.HoMIDom.Server
Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.IO

Namespace HoMIDom


    ''' <summary>      
    ''' Classe de gestion des base de données sqlite     
    ''' </summary>   
    '''    ''' <remarks></remarks> 
    ''' 
    Public Class Sqlite


        Private SQLconnect As New SQLiteConnection()
        Private Shared lock As New Object
        Public bdd_name As String = ""

        ''' <summary>          
        ''' Connection à une BDD Sqlite
        ''' </summary>
        ''' <param name="dbname">Nom de la base de donnée : homidom / media</param>
        ''' <returns>String si OK, String "ERR:..." si erreur</returns>
        ''' <remarks></remarks>          
        ''' 
        Public Function connect(ByVal dbname As String) As String
            Try

                If File.Exists("./bdd/" & dbname & ".db") Then
                    SQLconnect.ConnectionString = "Data Source=./bdd/" & dbname & ".db;"
                    SQLconnect.Open()
                    bdd_name = dbname
                    Return "Connecté"
                Else
                    Return "ERR: base sqlite " & dbname & " non trouvé"
                End If
            Catch ex As Exception
                Return "ERR: Non Connecté à la base sqlite " & dbname
            End Try
        End Function

        ''' <summary>
        ''' DeConnection à une BDD Sqlite 
        ''' </summary>       
        ''' <returns>String si OK, String "ERR:..." si erreur</returns>
        ''' <remarks></remarks>          
        ''' 
        Public Function disconnect() As String

            Try

                SQLconnect.Close()
                Return "Deconnecté"
            Catch ex As Exception
                Return "ERR: Erreur lors de la déconnexion de la base " & bdd_name
            End Try
        End Function

        ''' <summary>
        ''' Requete sans résultat
        ''' </summary>
        ''' <param name="commande">ex : DELETE FROM contact where Contact_id=10</param>
        ''' <param name="params">Liste es paramètres utilisés dans la query sous forme @parameter0, @parameter1 etc</param>
        ''' <returns>String si OK, String "ERR:..." si erreur</returns>
        ''' <remarks></remarks>
        Public Function nonquery(ByVal commande As String, ByVal ParamArray params() As String)
            Dim SQLcommand As SQLiteCommand

            Try
                'on vérifie si on est connecté à la BDD       
                If SQLconnect.State = ConnectionState.Open Then
                    'on vérifie si la commande n'est pas vide
                    If Not String.IsNullOrEmpty(commande) Then
                        SQLcommand = SQLconnect.CreateCommand
                        SQLcommand.CommandText = commande
                        If params IsNot Nothing Then
                            For p = 0 To params.Length - 1
                                SQLcommand.Parameters.Add(New SQLiteParameter("@parameter" + p.ToString(), params(p)))
                            Next
                        End If
                        'lock pour etre sur de ne pas faire deux operations en meme temps      
                        SyncLock lock
                            SQLcommand.ExecuteNonQuery()
                        End SyncLock
                        SQLcommand.Dispose()
                        Return "Commande éxécutée avec succés : " & commande
                    Else
                        Return "ERR: La commande est vide"
                    End If
                Else
                    Return "ERR: Non connecté à la BDD " & bdd_name
                End If
            Catch ex As Exception
                Return "ERR: Erreur lors de la query " & commande & "  --> " & ex.ToString
            End Try
        End Function

        ''' <summary>Requete avec résultat</summary>          
        ''' <param name="commande">ex : SELECT * FROM contact</param>          
        ''' <param name="resultat">Arralist contenant la liste des résultats</param>          
        ''' <returns>String si OK, String "ERR:..." si erreur</returns>          
        ''' <remarks></remarks>          
        Public Function query(ByVal commande As String, ByRef resultat As DataTable, ByVal ParamArray params() As String) As String
            Dim SQLcommand As SQLiteCommand
            Dim SQLreader As SQLiteDataReader
            Dim resultattemp As New DataTable
            Dim x As DataColumn

            Try
                'on vérifie si on est connecté à la BDD   
                If SQLconnect.State = ConnectionState.Open Then
                    'on vérifie si la commande n'est pas vide  
                    If commande Is Nothing And commande <> "" Then
                        SQLcommand = SQLconnect.CreateCommand
                        SQLcommand.CommandText = commande

                        If params IsNot Nothing Then
                            For p = 0 To params.Length - 1
                                SQLcommand.Parameters.Add(New SQLiteParameter("@parameter" + p.ToString(), params(p)))
                            Next
                        End If
                        'lock pour etre sur de ne pas faire deux operations en meme temps
                        SyncLock lock
                            SQLreader = SQLcommand.ExecuteReader()
                        End SyncLock

                        If SQLreader.HasRows = True Then
                            'lecture de la premiere ligne       
                            SQLreader.Read()
                            resultattemp.NewRow() ' on créé une premiere ligne dans le datatble      
                            For i = 0 To SQLreader.FieldCount - 1 'pour chaque colonne, on va créer la même dans le datable 
                                x = New DataColumn
                                x.ColumnName = "colonne_i"
                                resultattemp.Columns.Add(x)
                                resultattemp(0).Item(i) = SQLreader(i) 'on affete la valeur      
                            Next

                            'lecture de la suite des lignes   
                            Dim j As Integer = 1
                            While SQLreader.Read()
                                resultattemp.NewRow()
                                For i = 0 To SQLreader.FieldCount - 1
                                    resultattemp(j).Item(i) = SQLreader(i)
                                Next
                                j = j + 1
                            End While
                            resultat = resultattemp
                            Return "Commande éxécutée avec succés : " & commande
                        Else
                            Return "Commande éxécutée avec succés mais pas de résultat : " & commande
                        End If
                        SQLreader.Close()
                        SQLcommand.Dispose()
                    Else
                        Return "ERR: La commande est vide"
                    End If
                Else
                    Return "ERR: Non connecté à la BDD " & bdd_name
                End If
            Catch ex As Exception
                Return "ERR: Erreur lors de la query " & commande
            End Try
        End Function
    End Class
End Namespace
