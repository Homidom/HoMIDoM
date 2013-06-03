Imports System.Text
Imports System.Net
Imports System.Text.RegularExpressions
Imports System.IO
Imports System.Threading
Imports System.Xml

Namespace HoMIDom
    ''' <summary>
    ''' Permet d'effectuer un download (update) via SVN
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Update
        Dim _waitingForStop As ManualResetEvent
        Dim _finishedReadingTree As ManualResetEvent
        Dim _filesToDownload As List(Of FileDownloadData)
        Dim _readingThread As Thread
        Dim _selectedSourceType As String = "SVN"
        Dim _TargetFolder As String = ""
        Dim _SourceSvnUrl As String = ""
        Dim _Server As Server = Nothing

        ''' <summary>
        ''' URL Source
        ''' </summary>
        ''' <value>exemple: "http://homidom.googlecode.com/svn/trunk/release"</value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SourceSvnUrl As String
            Get
                Return _SourceSvnUrl
            End Get
            Set(ByVal value As String)
                _SourceSvnUrl = value
            End Set
        End Property

        ''' <summary>
        ''' Chemin de destination
        ''' </summary>
        ''' <value>"D:/temp/Homidom</value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property TargetFolder As String
            Get
                Return _TargetFolder
            End Get
            Set(ByVal value As String)
                _TargetFolder = value
            End Set
        End Property

        Public Sub New(ByVal Server As Server)
            _Server = Server
        End Sub

        ''' <summary>
        ''' Mettre à jour
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Update()
            If _readingThread Is Nothing Then
                Dim t As New Thread(New ThreadStart(AddressOf Run))
                t.Start()
                _readingThread = t
            Else
                [Stop]()
            End If
        End Sub

        Private Sub Run()
            ' Start downloading threads
            _finishedReadingTree = New ManualResetEvent(False)
            _waitingForStop = New ManualResetEvent(False)
            _filesToDownload = New List(Of FileDownloadData)()

            Dim downloadThreads As New List(Of Thread)()
            For i As Integer = 0 To 4
                Dim t As New Thread(New ThreadStart(AddressOf DownloadFilesThread))
                t.Start()
                downloadThreads.Add(t)
            Next

            Try
                If (TargetFolder <> "") AndAlso (SourceSvnUrl <> "") Then
                    Dim url As String = SourceSvnUrl

                    If _selectedSourceType = "GIT" Then
                        RunSvn(TargetFolder, SourceSvnUrl, RepositoryType.GIT)
                    Else
                        ' "SVN"
                        RunSvn(TargetFolder, SourceSvnUrl, RepositoryType.SVN)
                    End If
                Else
                    WriteToScreen("Parametres (Source, Destination) non définis")
                End If
            Catch ex As Exception
                WriteToScreen("Failed: " & Convert.ToString(ex))
                SyncLock _filesToDownload
                    _filesToDownload.Clear()
                End SyncLock
            Finally
                _finishedReadingTree.[Set]()
            End Try

            ' Wait for downloading threads
            WriteToScreen("Waiting for file downloading threads to finish")
            For i As Integer = 0 To downloadThreads.Count - 1
                downloadThreads(i).Join()
            Next

            WriteToScreen("Done.")
            _readingThread = Nothing
        End Sub

        Private Sub DownloadFilesThread()
            While True
                Dim fileDownloadData As FileDownloadData = Nothing
                SyncLock _filesToDownload
                    If _filesToDownload.Count > 0 Then
                        fileDownloadData = _filesToDownload(0)
                        _filesToDownload.RemoveAt(0)
                    End If
                End SyncLock

                If (fileDownloadData Is Nothing) AndAlso (_finishedReadingTree.WaitOne(0, False) = True) Then
                    Return
                End If

                If fileDownloadData IsNot Nothing Then
                    Dim retry As Boolean = True
                    While retry = True
                        If _waitingForStop.WaitOne(0, False) = True Then
                            Return
                        End If

                        Try
                            DownloadFile(fileDownloadData.Url, fileDownloadData.FileName)
                            retry = False
                        Catch ex As Exception
                            WriteToScreen("Failed to download: " & ex.Message)
                        End Try
                    End While
                Else
                    Thread.Sleep(100)
                End If
            End While
        End Sub

        Public Enum RepositoryType
            SVN
            GIT
        End Enum

        Private Sub RunSvn(ByVal baseFolder As String, ByVal baseUrl As String, ByVal repositoryType__1 As RepositoryType)
            If repositoryType__1 = RepositoryType.SVN Then
                If baseUrl.EndsWith("/") = False Then
                    baseUrl += "/"
                End If
            End If

            If baseFolder.EndsWith("\") = False Then
                baseFolder += "\"
            End If

            Dim urls As New List(Of FolderLinkData)()
            urls.Add(New FolderLinkData(baseUrl, ""))

            While urls.Count > 0
                If _waitingForStop.WaitOne(0, False) = True Then
                    WriteToScreen("Stopping...")
                    SyncLock _filesToDownload
                        _filesToDownload.Clear()
                    End SyncLock
                    Exit While
                End If

                Dim targetUrlData As FolderLinkData = urls(0)
                Dim targetUrl As String = targetUrlData.Url
                urls.RemoveAt(0)

                ' Create the folder
                Dim relative As String
                If targetUrlData.RelativePath Is Nothing Then
                    relative = targetUrl.Substring(baseUrl.Length)
                Else
                    relative = targetUrlData.RelativePath
                End If

                relative = relative.Replace("/", "\")
                Dim targetFolder As String = Path.Combine(baseFolder, relative)
                If Directory.Exists(targetFolder) = False Then
                    Directory.CreateDirectory(targetFolder)
                End If

                ' Download target page
                Dim page As String = Nothing
                Dim retry As Boolean = True
                While retry = True
                    If _waitingForStop.WaitOne(0, False) = True Then
                        Return
                    End If

                    Try
                        page = DownloadUrl(targetUrl)
                        retry = False
                    Catch ex As Exception
                        WriteToScreen("Failed to download: " & ex.Message)
                    End Try
                End While

                If repositoryType__1 = RepositoryType.SVN Then
                    Dim links As List(Of String) = ParseLinks(page)

                    For Each link As String In links
                        Dim linkFullUrl As String = targetUrl & link
                        If linkFullUrl.EndsWith("/") = True Then
                            urls.Add(New FolderLinkData(linkFullUrl, Nothing))
                        Else
                            ' file - download
                            Dim fileName As String = targetFolder & link
                            SyncLock _filesToDownload
                                _filesToDownload.Add(New FileDownloadData(linkFullUrl, fileName))
                            End SyncLock
                        End If
                    Next
                ElseIf repositoryType__1 = RepositoryType.GIT Then
                    Dim links As List(Of PageLink) = ParseGitLinks(page)
                    Dim pos As Integer = targetUrl.IndexOf("/?")
                    Dim serverUrl As String = targetUrl.Substring(0, pos)

                    For Each link As PageLink In links
                        Dim linkFullUrl As String = serverUrl & link.Url
                        If link.IsFolder = True Then
                            urls.Add(New FolderLinkData(linkFullUrl, targetUrlData.RelativePath & link.Name & "\"))
                        Else
                            Dim fileName As String = targetFolder & link.Name

                            SyncLock _filesToDownload
                                _filesToDownload.Add(New FileDownloadData(linkFullUrl, fileName))
                            End SyncLock
                        End If
                    Next
                End If
            End While
        End Sub

        Private Function ParseLinks(ByVal page As String) As List(Of String)
            Try
                Return ParseLinksFromXml(page)
            Catch
                Return ParseLinksFromHtml(page)
            End Try
        End Function

        Private Function ParseLinksFromXml(ByVal page As String) As List(Of String)
            Dim list As New List(Of String)()

            Dim doc As New XmlDocument()
            doc.LoadXml(page)

            Dim svnNode As XmlNode = doc.SelectSingleNode("/svn")
            If svnNode Is Nothing Then
                Throw New Exception("Not a valid SVN xml")
            End If

            For Each node As XmlNode In doc.SelectNodes("/svn/index/dir")
                Dim dir As String = node.Attributes("href").Value
                list.Add(dir)
            Next

            For Each node As XmlNode In doc.SelectNodes("/svn/index/file")
                Dim file As String = node.Attributes("href").Value
                list.Add(file)
            Next

            Return list
        End Function

        Private Function ParseLinksFromHtml(ByVal page As String) As List(Of String)
            Dim links As New List(Of String)()
            Dim listArea As String = Nothing

            ' Find list area: <ul> ... </ul>
            Dim pos As Integer = page.IndexOf("<ul>")
            If pos >= 0 Then
                Dim lastPos As Integer = page.IndexOf("</ul>", pos)
                If lastPos >= 0 Then
                    listArea = page.Substring(pos + 4, lastPos - (pos + 4))
                End If
            End If

            If listArea IsNot Nothing Then
                Dim lines As String() = listArea.Split(ControlChars.Lf)
                Dim linePattern As String = "<a [^>]*>([^<]*)<"
                For i As Integer = 0 To lines.Length - 1
                    Dim match As Match = Regex.Match(lines(i), linePattern)
                    If match.Success = True Then
                        Dim linkRelUrl As String = match.Groups(1).Value
                        If linkRelUrl <> ".." Then
                            links.Add(linkRelUrl)
                        End If
                    End If
                Next
            End If

            Return links
        End Function

        Private Function ParseGitLinks(ByVal page As String) As List(Of PageLink)
            Dim links As New List(Of PageLink)()

            Dim dataStartMarker As String = "<td class=""mode"">"
            Dim nameMarker As String = "hb=HEAD"">"

            Using sr As New StringReader(page)
                Dim line As String = ""
                While (InlineAssignHelper(line, sr.ReadLine())) IsNot Nothing
                    If line.StartsWith(dataStartMarker) = False Then
                        Continue While
                    End If

                    Dim isFolder As Boolean = False
                    If line(dataStartMarker.Length) = "d"c Then
                        isFolder = True
                    End If

                    line = sr.ReadLine()

                    ' Get name
                    Dim pos As Integer = line.IndexOf(nameMarker)
                    Dim endPos As Integer = line.IndexOf("<", pos)
                    pos += nameMarker.Length

                    Dim name As String = line.Substring(pos, endPos - pos)

                    If (name = "..") OrElse (name = ".") Then
                        Continue While
                    End If

                    ' Get URL
                    pos = line.IndexOf("href=""")
                    endPos = line.IndexOf(""">", pos)
                    pos += "href=""".Length
                    Dim url As String = line.Substring(pos, endPos - pos)
                    If isFolder = False Then
                        url = url.Replace(";a=blob;", ";a=blob_plain;")

                        pos = url.IndexOf(";h=")
                        url = url.Substring(0, pos)
                        url = url & ";hb=HEAD"
                    End If

                    If url.Contains(";a=tree;") Then
                        isFolder = True
                    End If

                    links.Add(New PageLink(name, url, isFolder))
                End While
            End Using

            Return links
        End Function

#Region "Download helper functions"
        Private Sub DownloadFile(ByVal url As String, ByVal fileName As String)
            WriteToScreen("Downloading File: " & url)

            Dim webRequest__1 As WebRequest = WebRequest.Create(url)
            Dim webResponse As WebResponse = Nothing
            Dim responseStream As Stream = Nothing
            Try
                webResponse = webRequest__1.GetResponse()
                responseStream = webResponse.GetResponseStream()

                Using fs As New FileStream(fileName, FileMode.Create)
                    Dim buffer As Byte() = New Byte(1023) {}
                    Dim readSize As Integer
                    While (InlineAssignHelper(readSize, responseStream.Read(buffer, 0, buffer.Length))) > 0
                        fs.Write(buffer, 0, readSize)
                    End While
                End Using
            Finally
                If responseStream IsNot Nothing Then
                    responseStream.Close()
                End If

                If webResponse IsNot Nothing Then
                    webResponse.Close()
                End If
            End Try
        End Sub

        Private Function DownloadUrl(ByVal url As String) As String
            WriteToScreen("Downloading: " & url)
            Using client As New WebClient()
                Dim data As String = client.DownloadString(url)

                Return data
            End Using
        End Function
#End Region

        Private Delegate Sub WriteToScreenDelegate(ByVal str As String)
        Private Sub WriteToScreen(ByVal str As String)
            _Server.Log(Server.TypeLog.DEBUG, Server.TypeSource.SERVEUR, "Update WriteToScreen ", str)
        End Sub

        Private Sub [Stop]()
            _waitingForStop.[Set]()
        End Sub

        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
            target = value
            Return value
        End Function

        Protected Overrides Sub Finalize()
            If _readingThread IsNot Nothing Then
                [Stop]()
            End If

            MyBase.Finalize()
        End Sub
    End Class

    Public Class PageLink
        Private _name As String
        Private _url As String
        Private _isFolder As Boolean

        Public ReadOnly Property Name() As String
            Get
                Return _name
            End Get
        End Property
        Public ReadOnly Property Url() As String
            Get
                Return _url
            End Get
        End Property
        Public ReadOnly Property IsFolder() As Boolean
            Get
                Return _isFolder
            End Get
        End Property

        Public Sub New(ByVal name As String, ByVal url As String, ByVal isFolder As Boolean)
            _name = name
            _url = url
            _isFolder = isFolder
        End Sub
    End Class

    Public Class FolderLinkData
        Private _url As String
        Private _relativePath As String

        Public ReadOnly Property Url() As String
            Get
                Return _url
            End Get
        End Property
        Public ReadOnly Property RelativePath() As String
            Get
                Return _relativePath
            End Get
        End Property

        Public Sub New(ByVal url As String, ByVal relativePath As String)
            _url = url
            _relativePath = relativePath
        End Sub
    End Class

    Public Class FileDownloadData
        Private _url As String
        Private _fileName As String

        Public ReadOnly Property Url() As String
            Get
                Return _url
            End Get
        End Property

        Public ReadOnly Property FileName() As String
            Get
                Return _fileName
            End Get
        End Property

        Public Sub New(ByVal url As String, ByVal fileName As String)
            _url = url
            _fileName = fileName
        End Sub

    End Class
End Namespace
