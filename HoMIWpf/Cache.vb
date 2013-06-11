Module Cache
    Public Function DeleteAllFileOffPath(ByVal Path As String) As String
        Try
            If System.IO.Directory.Exists(Path) Then
                For Each files As String In System.IO.Directory.GetFiles(Path)
                    System.IO.File.Delete(files)
                Next
            End If
            Return Nothing
        Catch ex As Exception
            Return ex.ToString
        End Try
    End Function

    Public Function Cache(ByVal File As String, ByVal CachePath As String) As BitmapImage
        Dim retour As BitmapImage = Nothing

        Try
            If System.IO.Directory.Exists(CachePath) Then 'On vérifie que le dossier cache existe
                If IO.File.Exists(CachePath & "\" & File) Then

                Else 'le fichier n'existe pas il faut le récup

                End If
            End If
            Return retour
        Catch ex As Exception

            Return retour
        End Try
    End Function
End Module
