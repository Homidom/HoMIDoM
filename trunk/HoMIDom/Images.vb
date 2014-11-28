Imports System.Windows.Media.Imaging
Imports System.IO

Namespace HoMIDom

    Public Module mImages
        ''' <summary>
        ''' Scan tous les fichiers image (png ou jpg) présents sur le serveur
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub ScanImage()
            Try
                Dim dirInfo As New System.IO.DirectoryInfo(_MonRepertoire & "\images\")
                Dim files() As System.IO.FileInfo = dirInfo.GetFiles("*.*g*", System.IO.SearchOption.AllDirectories)

                If (files IsNot Nothing) Then
                    For Each file As System.IO.FileInfo In files
                        Dim a As String = file.Extension.ToLower.Replace(".", "")
                        If a = "jpg" Or a = "gif" Or a = "png" Then
                            Dim x As New ImageFile
                            x.Path = file.FullName
                            x.FileName = file.Name
                            Server.Instance.Images.Add(x)
                            x = Nothing
                        End If
                    Next
                End If
                files = Nothing
                dirInfo = Nothing

            Catch ex As Exception
                Server.Instance.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "ScanImage", "Erreur:" & ex.Message)
            End Try
        End Sub

        ''' <summary>
        ''' Convertit Byte en Image
        ''' </summary>
        ''' <param name="value">Image sous forme de Byte</param>
        ''' <param name="Taille">Taille en pixel de l'imate</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ConvertArrayToImage(ByVal value As Object, Optional ByVal Taille As Integer = 0) As Object
            Try
                Dim ImgSource As BitmapImage = Nothing
                Dim array As Byte() = TryCast(value, Byte())

                If array IsNot Nothing Then
                    ImgSource = New BitmapImage()
                    If Taille > 0 Then
                        ImgSource.DecodePixelHeight = Taille
                        ImgSource.DecodePixelWidth = Taille
                    End If
                    ImgSource.BeginInit()
                    ImgSource.CacheOption = BitmapCacheOption.OnLoad
                    ImgSource.CreateOptions = BitmapCreateOptions.DelayCreation
                    ImgSource.StreamSource = New MemoryStream(array)
                    array = Nothing
                    ImgSource.EndInit()
                    If ImgSource.CanFreeze Then ImgSource.Freeze()
                End If

                Return ImgSource
                ImgSource = Nothing
            Catch ex As Exception
                Server.Instance.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "ConvertArrayToImage", "Erreur:" & ex.Message)
                Return Nothing
            End Try
        End Function

    End Module
End Namespace