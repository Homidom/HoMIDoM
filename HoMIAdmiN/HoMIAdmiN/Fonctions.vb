Imports System.IO
Imports System.Net
Imports System.Net.Sockets
'Imports System.Runtime.Serialization.Formatters.Soap
Imports System.ServiceModel
Imports HoMIDom

Module Fonctions
    ''' <summary>
    ''' Affiche le message et connecté log dans le serveur
    ''' </summary>
    ''' <param name="Type"></param>
    ''' <param name="Message"></param>
    ''' <param name="Title"></param>
    ''' <param name="Fonction"></param>
    ''' <remarks></remarks>
    Public Sub AfficheMessageAndLog(ByVal Type As HoMIDom.HoMIDom.Server.TypeLog, ByVal Message As String, Optional ByVal Title As String = "", Optional ByVal Fonction As String = "")
        Dim Icon As MessageBoxImage = MessageBoxImage.Error

        Select Case Type
            Case HoMIDom.HoMIDom.Server.TypeLog.MESSAGE : Icon = MessageBoxImage.Information
            Case HoMIDom.HoMIDom.Server.TypeLog.INFO : Icon = MessageBoxImage.Information
            Case HoMIDom.HoMIDom.Server.TypeLog.DEBUG : Icon = MessageBoxImage.Warning
            Case Else : Icon = MessageBoxImage.Error
        End Select

        If Type <> HoMIDom.HoMIDom.Server.TypeLog.DEBUG Then MessageBox.Show(Message, Title, MessageBoxButton.OK, Icon)

        If IsConnect Then myService.Log(Type, HoMIDom.HoMIDom.Server.TypeSource.CLIENT, Fonction, Message)
    End Sub


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
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub ConvertArrayToImage: " & ex.Message, "ERREUR", "")
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
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub UrlIsValid: " & ex.Message, "ERREUR", "")
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
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub SaveRealTime: " & ex.Message, "ERREUR", "")
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
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR ImageFromUri: " & ex.Message, "ERREUR", "")
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
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur lors de la connexion au serveur de fichier, veuillez vérifier que celui-ci est démarré", "Erreur Admin", "")
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
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur lors de la connexion au serveur de fichier", "Erreur Admin", "")
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
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur lors de la connexion au serveur de fichier sélectionné: " & ex.Message, "Erreur Admin", "")
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
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur lors de la connexion au serveur de fichier, veuillez vérifier que celui-ci est démarré", "Erreur Admin", "")
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
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur lors de la connexion au serveur de fichier", "Erreur Admin", "")
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
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur lors de la connexion au serveur de fichier sélectionné: " & ex.Message, "Erreur Admin", "")
            Return -1
        End Try
    End Function

    Public Function GetWeatherCityID() As Dictionary(Of String, String)
        Try
            Dim Retour As New Dictionary(Of String, String)
            Dim linesFR() As String = My.Resources.WeatherCityID_FR.Split(System.Environment.NewLine)
            For Each Line In linesFR
                If String.IsNullOrEmpty(Line) = False Then
                    Dim tab() As String = Line.Split(";")
                    If tab.Count = 2 Then
                        Retour.Add(tab(0).Replace(vbCr, "").Replace(vbLf, ""), tab(1).Replace(vbCr, "").Replace(vbLf, ""))
                    End If
                End If
            Next
            Dim linesBE() As String = My.Resources.WeatherCityID_BE.Split(System.Environment.NewLine)
            For Each Line In linesBE
                If String.IsNullOrEmpty(Line) = False Then
                    Dim tab() As String = Line.Split(";")
                    If tab.Count = 2 Then
                        Retour.Add(tab(0).Replace(vbCr, "").Replace(vbLf, ""), tab(1).Replace(vbCr, "").Replace(vbLf, ""))
                    End If
                End If
            Next
            'Retour = Retour.OrderBy(Function(x) x.Key).ToDictionary(Function(x) x.Key, Function(y) y.Value)
            'Retour = Retour.OrderBy(Function(x) x.Value).ToDictionary(Function(x) x.Key, Function(y) y.Value)

            Return Retour
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR GetWeatherCityID: " & ex.Message, "ERREUR", "")
            Return Nothing
        End Try
    End Function

    Public Function GetFrameworkVersionString() As String
        Try
            Dim CLRVersion As Version = System.Environment.Version

            If (CLRVersion.Major = 4 & CLRVersion.Minor = 0) Then
                '4.0.30319.237   - 4.0
                '4.0.30319.17020 - 4.5 (Microsoft .NET Framework 4.5 Developer Preview)
                '4.0.30319.17379 - 4.5 (Microsoft .NET Framework 4.5 Consumer Preview)
                '4.0.30319.17626 - 4.5 (Microsoft .NET Framework 4.5 RC)
                '4.0.30319.17929 - 4.5 (Microsoft .NET Framework 4.5 RTM)
                '4.0.30319.18408 - 4.5.1 (Microsoft .NET Framework 4.5.1 RTM - Windows Vista/7 - KB2858728)
                '4.0.30319.34003 - 4.5.1 (Microsoft .NET Framework 4.5.1 RTM - Windows 8.1)
                '4.0.30319.34209 - 4.5.2 (Microsoft .NET Framework 4.5.2 May 2014 Update)
                '4.0.30319.42000 - 4.6 - In .NET Framework 4.6, the Environment.Version property returns the fixed version string 4.0.30319.42000

                If (CLRVersion >= New Version(4, 0, 30319, 42000)) Then
                    Return "4.6"
                ElseIf (CLRVersion >= New Version(4, 0, 30319, 34209)) Then
                    Return "4.5.2"
                ElseIf (CLRVersion >= New Version(4, 0, 30319, 18408)) Then
                    Return "4.5.1"
                ElseIf (CLRVersion >= New Version(4, 0, 30319, 17020)) Then
                    Return "4.5"
                End If
                Return "4.0"   '//4.0.30319.237
            ElseIf (CLRVersion.Major = 2 & CLRVersion.Minor = 0) Then
                If (CLRVersion >= New Version(2, 0, 50727, 3521)) Then     '3.5.1
                    '2.0.50727.3521 - 3.5.1 in Windows 7 Beta 2
                    '2.0.50727.4016 - 3.5 SP1 in Windows Vista SP2 or Windows Server 2008 SP2
                    '2.0.50727.4918 - 3.5.1 in Windows 7 RC or Windows Server 2008 R2
                    Try
                        System.Reflection.Assembly.Load("System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")
                        If (Environment.OSVersion.Platform = PlatformID.Win32NT & Environment.OSVersion.Version.Major >= 6 & Environment.OSVersion.Version.Minor >= 1) Then Return "3.5.1"
                        Return "3.5 SP1"
                    Catch ex As Exception
                    End Try
                    Return "2.0 SP2"
                End If
                If (CLRVersion >= New Version(2, 0, 50727, 3053)) Then Return "3.5 SP1"
                If (CLRVersion >= New Version(2, 0, 50727, 1433)) Then
                    '2.0 SP1 or 3.0 SP1 or 3.5
                    Try
                        System.Reflection.Assembly.Load("System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")
                        Return "3.5"
                    Catch ex As Exception
                    End Try

                    Try
                        System.Reflection.Assembly.Load("WindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")
                        Return "3.0 SP1"
                    Catch ex As Exception
                    End Try

                    Return "2.0 SP1"
                End If

                If (CLRVersion = New Version(2, 0, 50727, 312)) Then Return "3.0" 'Vista RTM

                '//2.0.50727.42 RTM - 2.0 or 3.0
                Try
                    System.Reflection.Assembly.Load("WindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")
                    Return "3.0"
                Catch ex As Exception
                End Try

                Return "2.0"
            End If

            Return CLRVersion.Major.ToString(System.Globalization.CultureInfo.InvariantCulture) + "." + CLRVersion.Minor.ToString(System.Globalization.CultureInfo.InvariantCulture)
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR GetFrameworkVersionString: " & ex.Message, "ERREUR", "GetFrameworkVersionString")
            Return ""
        End Try
    End Function
End Module
