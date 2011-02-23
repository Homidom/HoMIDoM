Imports System.Data.SQLite
Imports HoMIDom.HoMIDom.Server
Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.IO

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
    Public Function connect(ByVal dbname As String) As String
        Try
            If File.Exists("./bdd/" & dbname & ".db") Then
                SQLconnect.ConnectionString = "Data Source=./bdd/" & dbname & ".db;"
                SQLconnect.Open()
                bdd_name = dbname
                Return "Connecté à la base sqlite " & dbname
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
    Public Function disconnect() As String
        Try
            SQLconnect.Close()
            Return "Deconnecté de " & bdd_name
        Catch ex As Exception
            Return "ERR: Erreur lors de la déconnexion de la base " & bdd_name
        End Try
    End Function

    ''' <summary>
    ''' Requete sans résultat
    ''' </summary>
    ''' <param name="commande">ex : DELETE FROM contact where Contact_id=10</param>
    ''' <returns>String si OK, String "ERR:..." si erreur</returns>
    ''' <remarks></remarks>
    Public Function nonquery(ByVal commande As String) As String
        Dim SQLcommand As SQLiteCommand
        Try
            If commande Is Nothing And commande <> "" Then
                SQLcommand = SQLconnect.CreateCommand
                SQLcommand.CommandText = commande
                SyncLock lock
                    SQLcommand.ExecuteNonQuery()
                End SyncLock
                SQLcommand.Dispose()
                Return "Commande éxécutée avec succés : " & commande
            Else
                Return "ERR: La commande est vide"
            End If
        Catch ex As Exception
            Return "ERR: Erreur lors de la query " & commande
        End Try
    End Function

    ''' <summary>
    ''' Requete avec résultat
    ''' </summary>
    ''' <param name="commande">ex : SELECT * FROM contact</param>
    ''' <param name="resultat">Arralist contenant la liste des résultats</param>
    ''' <returns>String si OK, String "ERR:..." si erreur</returns>
    ''' <remarks></remarks>
    Public Function query(ByVal commande As String, ByRef resultat As DataTable) As String
        Dim SQLcommand As SQLiteCommand
        Dim SQLreader As SQLiteDataReader
        Try
            If commande Is Nothing And commande <> "" Then
                SQLcommand = SQLconnect.CreateCommand
                SQLcommand.CommandText = commande
                SyncLock lock
                    SQLreader = SQLcommand.ExecuteReader()
                End SyncLock
                If SQLreader.HasRows = True Then
                    While SQLreader.Read()
                        For i = 0 To SQLreader.FieldCount - 1
                            '        TxtID = SQLreader(i)

                        Next
                    End While
                    Return "Commande éxécutée avec succés : " & commande
                Else
                    Return "Commande éxécutée avec succés mais pas de résultat : " & commande
                End If
                SQLcommand.Dispose()
            Else
                Return "ERR: La commande est vide"
            End If
        Catch ex As Exception
            Return "ERR: Erreur lors de la query " & commande
        End Try
    End Function

End Class
