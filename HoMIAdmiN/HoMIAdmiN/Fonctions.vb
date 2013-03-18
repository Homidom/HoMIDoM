﻿Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Runtime.Serialization.Formatters.Soap
Imports System.ServiceModel

Module Fonctions
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
            MessageBox.Show("ERREUR Sub ConvertArrayToImage: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
            Return Nothing
        End Try
    End Function

    Public Function UrlIsValid(ByVal Host As String) As Boolean
        Try
            Dim bValid As Boolean = False
            Try
                Dim url As New Uri(Host)
                Dim Request As HttpWebRequest = CType(HttpWebRequest.Create(url), System.Net.HttpWebRequest)
                Dim response As Net.HttpWebResponse = CType(Request.GetResponse(), Net.HttpWebResponse)
                bValid = True
            Catch ex As Exception
                bValid = False
            End Try
            Return bValid
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub UrlIsValid: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
            Return False
        End Try
    End Function

    Public Sub SaveRealTime()
        Try
            If IsConnect And My.Settings.SaveRealTime Then
                Dim retour As String = myService.SaveConfig(IdSrv)
                If retour = "0" Then FlagChange = False
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub SaveRealTime: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Public Function ImageFromUri(ByVal Uri As String) As Windows.Media.Imaging.BitmapImage
        Try
            Dim bmpImage As New BitmapImage()

            bmpImage.BeginInit()
            bmpImage.CacheOption = BitmapCacheOption.OnLoad
            bmpImage.CreateOptions = BitmapCreateOptions.DelayCreation
            bmpImage.UriSource = New Uri(Uri, UriKind.Absolute)
            bmpImage.EndInit()
            If bmpImage.CanFreeze Then bmpImage.Freeze()
            Return bmpImage

            bmpImage = Nothing
        Catch ex As Exception
            MessageBox.Show("ERREUR ImageFromUri: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
            Return Nothing
        End Try
    End Function


    ''' <summary>
    ''' Télécharge un fichier depuis le serveur
    ''' </summary>
    ''' <param name="IdServ">Id du serveur</param>
    ''' <param name="Fichier">Chemin local vers le fichier à envoyer, p.ex "C:\zik.mp3"</param>
    ''' <param name="Location">Endroit où stocker le fichier sur le serveur, p.ex. "Musique/zik.mp3"</param>
    ''' <returns>-1 en cas d'erreur, 0 en cas de réussite.</returns>
    ''' <remarks></remarks>
    Public Function Download(ByVal IdServ As String, ByVal Fichier As String, ByVal Location As String) As Integer

        Dim fileData As HoMIDom.HoMIDom.FileData = Nothing
        Dim myServiceFile As HoMIDom.HoMIDom.IFileServer = Nothing
        Dim myChannelFactory As ServiceModel.ChannelFactory(Of HoMIDom.HoMIDom.IFileServer) = Nothing

        Try
            Dim _myadress = Replace(myadress, "/service", "/fileServer")

            If UrlIsValid(_myadress) = False Then
                MessageBox.Show("Erreur lors de la connexion au serveur de fichier, veuillez vérifier que celui-ci est démarré", "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Return -1
            End If

            Dim binding As New ServiceModel.BasicHttpBinding
            binding.MaxBufferPoolSize = 250000000
            binding.MaxReceivedMessageSize = 250000000
            binding.MaxBufferSize = 250000000
            binding.ReaderQuotas.MaxArrayLength = 250000000
            binding.ReaderQuotas.MaxNameTableCharCount = 250000000
            binding.ReaderQuotas.MaxBytesPerRead = 250000000
            binding.ReaderQuotas.MaxStringContentLength = 250000000
            binding.SendTimeout = TimeSpan.FromMinutes(60)
            binding.CloseTimeout = TimeSpan.FromMinutes(60)
            binding.OpenTimeout = TimeSpan.FromMinutes(60)
            binding.ReceiveTimeout = TimeSpan.FromMinutes(60)
            binding.TransferMode = TransferMode.Streamed


            myChannelFactory = New ServiceModel.ChannelFactory(Of HoMIDom.HoMIDom.IFileServer)(binding, New System.ServiceModel.EndpointAddress(_myadress))
            myServiceFile = myChannelFactory.CreateChannel()

            'On vérifie qu'on est bien connecté
            If myChannelFactory.State <> CommunicationState.Opened Then
                MessageBox.Show("Erreur lors de la connexion au serveur de fichier", "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
                Return -1
                Exit Function
            Else
                Dim x As HoMIDom.HoMIDom.RequestFileData = New HoMIDom.HoMIDom.RequestFileData()
                With x
                    .Name = Location
                    .IdServ = IdServ
                End With

                fileData = myServiceFile.Download(x)

                Using writeStream As FileStream = File.Open(Fichier, FileMode.Create, FileAccess.Write)
                    fileData.Stream.CopyTo(writeStream)
                    fileData.Stream.Close()
                End Using

                myChannelFactory.Close()
                fileData = Nothing
                binding = Nothing

                Return 0
            End If
        Catch ex As Exception
            myChannelFactory.Abort()
            If Not fileData Is Nothing And Not fileData.Stream Is Nothing Then
                fileData.Stream.Close()
            End If
            MessageBox.Show("Erreur lors de la connexion au serveur de fichier sélectionné: " & ex.Message, "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
            Return -1
        End Try
    End Function


    ''' <summary>
    ''' Envoi un fichier au serveur
    ''' </summary>
    ''' <param name="IdServ">Id du serveur</param>
    ''' <param name="Fichier">Chemin local vers le fichier à envoyer, p.ex "C:\zik.mp3"</param>
    ''' <param name="Location">Endroit où stocker le fichier sur le serveur, p.ex. "Musique/zik.mp3"</param>
    ''' <returns>-1 en cas d'erreur, 0 en cas de réussite.</returns>
    ''' <remarks></remarks>
    Public Function Upload(ByVal IdServ As String, ByVal Fichier As String, ByVal Location As String) As Integer

        If Not System.IO.File.Exists(Fichier) Then
            Return -1 ' Le fichier donné n'existe pas
        End If

        Dim myStream As System.IO.Stream = Nothing
        Dim myServiceFile As HoMIDom.HoMIDom.IFileServer = Nothing
        Dim myChannelFactory As ServiceModel.ChannelFactory(Of HoMIDom.HoMIDom.IFileServer) = Nothing

        Try
            Dim _myadress = Replace(myadress, "/service", "/fileServer")

            If UrlIsValid(_myadress) = False Then
                MessageBox.Show("Erreur lors de la connexion au serveur de fichier, veuillez vérifier que celui-ci est démarré", "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Return -1
            End If

            Dim binding As New ServiceModel.BasicHttpBinding
            binding.MaxBufferPoolSize = 250000000
            binding.MaxReceivedMessageSize = 250000000
            binding.MaxBufferSize = 250000000
            binding.ReaderQuotas.MaxArrayLength = 250000000
            binding.ReaderQuotas.MaxNameTableCharCount = 250000000
            binding.ReaderQuotas.MaxBytesPerRead = 250000000
            binding.ReaderQuotas.MaxStringContentLength = 250000000
            binding.SendTimeout = TimeSpan.FromMinutes(60)
            binding.CloseTimeout = TimeSpan.FromMinutes(60)
            binding.OpenTimeout = TimeSpan.FromMinutes(60)
            binding.ReceiveTimeout = TimeSpan.FromMinutes(60)
            binding.TransferMode = TransferMode.Streamed

            myChannelFactory = New ServiceModel.ChannelFactory(Of HoMIDom.HoMIDom.IFileServer)(binding, New System.ServiceModel.EndpointAddress(_myadress))
            myServiceFile = myChannelFactory.CreateChannel()

            'On vérifie qu'on est bien connecté
            If myChannelFactory.State <> CommunicationState.Opened Then
                MessageBox.Show("Erreur lors de la connexion au serveur de fichier", "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
                Return -1
                Exit Function
            Else
                myStream = System.IO.File.Open(Fichier, FileMode.Open, FileAccess.Read)
                Dim data As HoMIDom.HoMIDom.UploadFileData = New HoMIDom.HoMIDom.UploadFileData()
                With data
                    .FilePath = Location
                    .Stream = myStream
                    .IdServ = IdServ
                End With

                Dim resultat = myServiceFile.Upload(data)
                If resultat.Success Then
                    Return 1
                End If

                myChannelFactory.Close()
                Return 0
                binding = Nothing
            End If
        Catch ex As Exception
            myChannelFactory.Abort()
            If Not myStream Is Nothing Then
                myStream.Close()
            End If
            MessageBox.Show("Erreur lors de la connexion au serveur de fichier sélectionné: " & ex.Message, "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
            Return -1
        End Try
    End Function

End Module
