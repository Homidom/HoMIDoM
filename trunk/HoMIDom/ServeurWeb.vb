Imports System.Collections
Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports System.Xml

Namespace HoMIDom

    Public Class ServeurWeb
        Shared _Srv As Server = Nothing
        Dim _PortWeb As Integer = 0
        Dim _httpServer As HttpServer
        Dim _thread As Thread = Nothing
        Dim _IsStart As Boolean = False

        Public Class HttpProcessor
            Public socket As TcpClient
            Public srv As HttpServer

            Private inputStream As Stream
            Public outputStream As StreamWriter

            Public http_method As [String]
            Public http_url As [String]
            Public http_protocol_versionstring As [String]
            Public httpHeaders As New Hashtable()


            Private Shared MAX_POST_SIZE As Integer = 10 * 1024 * 1024
            ' 10MB
            Public Sub New(ByVal s As TcpClient, ByVal srv As HttpServer)
                Me.socket = s
                Me.srv = srv
            End Sub


            Private Function streamReadLine(ByVal inputStream As Stream) As String
                Dim next_char As Integer
                Dim data As String = ""
                While True
                    next_char = inputStream.ReadByte()
                    If next_char = 10 Then  'ControlChars.Lf
                        Exit While
                    End If
                    If next_char = 13 Then 'ControlChars.Cr
                        Continue While
                    End If
                    If next_char = -1 Then
                        Thread.Sleep(1)
                        Continue While
                    End If


                    data += Convert.ToChar(next_char)
                End While
                Return data
            End Function
            Public Sub process()
                ' we can't use a StreamReader for input, because it buffers up extra data on us inside it's
                ' "processed" view of the world, and we want the data raw after the headers
                inputStream = New BufferedStream(socket.GetStream())

                ' we probably shouldn't be using a streamwriter for all output from handlers either
                outputStream = New StreamWriter(New BufferedStream(socket.GetStream()))
                Try
                    parseRequest()
                    readHeaders()
                    If http_method.Equals("GET") Then
                        handleGETRequest()
                    ElseIf http_method.Equals("POST") Then
                        handlePOSTRequest()
                    End If
                Catch e As Exception
                    _Srv.Log(Server.TypeLog.DEBUG, Server.TypeSource.SERVEUR, "process", "Exception: " & e.ToString())
                    writeFailure()
                End Try
                outputStream.Flush()
                ' bs.Flush(); // flush any remaining output
                inputStream = Nothing
                outputStream = Nothing
                ' bs = null;            
                socket.Close()
            End Sub

            Public Sub parseRequest()
                Dim request As [String] = streamReadLine(inputStream)
                Dim tokens As String() = request.Split(" "c)
                If tokens.Length <> 3 Then
                    Throw New Exception("invalid http request line")
                End If
                http_method = tokens(0).ToUpper()
                http_url = tokens(1)
                http_protocol_versionstring = tokens(2)

                _Srv.Log(Server.TypeLog.DEBUG, Server.TypeSource.SERVEUR, "parseRequest", "starting: " & request)

            End Sub

            Public Sub readHeaders()
                _Srv.Log(Server.TypeLog.DEBUG, Server.TypeSource.SERVEUR, "readHeaders", "readHeaders()")

                Dim line As [String]
                While (InlineAssignHelper(line, streamReadLine(inputStream))) IsNot Nothing
                    If line.Equals("") Then
                        _Srv.Log(Server.TypeLog.DEBUG, Server.TypeSource.SERVEUR, "readHeaders", "got headers")
                        Return
                    End If

                    Dim separator As Integer = line.IndexOf(":"c)
                    If separator = -1 Then
                        Throw New Exception("invalid http header line: " & line)
                    End If
                    Dim name As [String] = line.Substring(0, separator)
                    Dim pos As Integer = separator + 1
                    While (pos < line.Length) AndAlso (line(pos) = " "c)
                        ' strip any spaces
                        pos += 1
                    End While

                    Dim value As String = line.Substring(pos, line.Length - pos)
                    _Srv.Log(Server.TypeLog.DEBUG, Server.TypeSource.SERVEUR, "readHeaders", "header: " & name & ":" & value)
                    httpHeaders(name) = value
                End While
            End Sub

            Public Sub handleGETRequest()
                srv.handleGETRequest(Me)
            End Sub

            Private Const BUF_SIZE As Integer = 4096
            Public Sub handlePOSTRequest()
                ' this post data processing just reads everything into a memory stream.
                ' this is fine for smallish things, but for large stuff we should really
                ' hand an input stream to the request processor. However, the input stream 
                ' we hand him needs to let him see the "end of the stream" at this content 
                ' length, because otherwise he won't know when he's seen it all! 

                _Srv.Log(Server.TypeLog.DEBUG, Server.TypeSource.SERVEUR, "handlePOSTRequest", "get post data start")

                Dim content_len As Integer = 0
                Dim ms As New MemoryStream()
                If Me.httpHeaders.ContainsKey("Content-Length") Then
                    content_len = Convert.ToInt32(Me.httpHeaders("Content-Length"))
                    If content_len > MAX_POST_SIZE Then
                        Throw New Exception([String].Format("POST Content-Length({0}) too big for this simple server", content_len))
                    End If
                    Dim buf As Byte() = New Byte(BUF_SIZE - 1) {}
                    Dim to_read As Integer = content_len
                    While to_read > 0
                        _Srv.Log(Server.TypeLog.DEBUG, Server.TypeSource.SERVEUR, "handlePOSTRequest", "starting Read, to_read=" & to_read)

                        Dim numread As Integer = Me.inputStream.Read(buf, 0, Math.Min(BUF_SIZE, to_read))
                        _Srv.Log(Server.TypeLog.DEBUG, Server.TypeSource.SERVEUR, "handlePOSTRequest", "read finished, numread=" & numread)

                        If numread = 0 Then
                            If to_read = 0 Then
                                Exit While
                            Else
                                Throw New Exception("client disconnected during post")
                            End If
                        End If
                        to_read -= numread
                        ms.Write(buf, 0, numread)
                    End While
                    ms.Seek(0, SeekOrigin.Begin)
                End If
                _Srv.Log(Server.TypeLog.DEBUG, Server.TypeSource.SERVEUR, "handlePOSTRequest", "get post data end")
                srv.handlePOSTRequest(Me, New StreamReader(ms))

            End Sub

            Public Sub writeSuccess()
                outputStream.WriteLine("HTTP/1.0 200 OK")
                outputStream.WriteLine("Content-Type: text/html")
                outputStream.WriteLine("Connection: close")
                outputStream.WriteLine("")
            End Sub

            Public Sub writeFailure()
                outputStream.WriteLine("HTTP/1.0 404 File not found")
                outputStream.WriteLine("Connection: close")
                outputStream.WriteLine("")
            End Sub
            Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
                target = value
                Return value
            End Function
        End Class

        Public MustInherit Class HttpServer

            Protected port As Integer
            Private listener As TcpListener
            Private is_active As Boolean = True

            Public Sub New(ByVal port As Integer)
                Me.port = port
            End Sub

            Public Sub listen()
                listener = New TcpListener(port)
                listener.Start()
                While is_active
                    Dim s As TcpClient = listener.AcceptTcpClient()
                    Dim processor As New HttpProcessor(s, Me)
                    Dim thread__1 As New Thread(New ThreadStart(AddressOf processor.process))
                    thread__1.Start()
                    Thread.Sleep(1)
                End While
            End Sub

            Public MustOverride Sub handleGETRequest(ByVal p As HttpProcessor)
            Public MustOverride Sub handlePOSTRequest(ByVal p As HttpProcessor, ByVal inputData As StreamReader)
        End Class

        Public Class MyHttpServer
            Inherits HttpServer
            Public Sub New(ByVal port As Integer)
                MyBase.New(port)
            End Sub
            Public Overrides Sub handleGETRequest(ByVal p As HttpProcessor)
                _Srv.Log(Server.TypeLog.DEBUG, Server.TypeSource.SERVEUR, "handleGETRequest", "request: " & p.http_url)

                p.writeSuccess()

                Dim txt As String = ReturnResult(p.http_url)
                p.outputStream.WriteLine(txt)

            End Sub

            Public Overrides Sub handlePOSTRequest(ByVal p As HttpProcessor, ByVal inputData As StreamReader)
                Console.WriteLine("POST request: {0}", p.http_url)
                Dim data As String = inputData.ReadToEnd()

                p.outputStream.WriteLine("<html><body><h1>test server</h1>")
                p.outputStream.WriteLine("<a href=/test>return</a><p>")
                p.outputStream.WriteLine("postbody: <pre>{0}</pre>", data)


            End Sub

            Public Function ReturnResult(ByVal request As String) As String
                Try
                    Dim xDoc As New XmlDocument
                    Dim xmldecl As XmlDeclaration
                    Dim commande As String = Mid(request, 2, Len(request) - 1).ToLower
                    Dim tabl() As String = commande.Split(";")
                    Dim flagIdSrv As Boolean = False
                    Dim flagcmd As Boolean = False
                    Dim _cmd As String = ""

                    xmldecl = xDoc.CreateXmlDeclaration("1.0", Nothing, Nothing)
                    xmldecl.Encoding = "UTF-8"
                    xmldecl.Standalone = "yes"

                    Dim root As XmlElement = xDoc.DocumentElement
                    xDoc.InsertBefore(xmldecl, root)


                    ' Create outer XML
                    Dim xNode As XmlNode = xDoc.AppendChild(xDoc.CreateElement("homidom"))

                    'Message erroné"
                    If tabl.Count < 2 Then
                        Dim xAuthor As XmlNode = xNode.AppendChild(xDoc.CreateElement("Message"))
                        xAuthor.InnerText = "Error"
                    End If
                    'Message erroné car l'id est erroné ou on a pas passé le paramètre"
                    If String.IsNullOrEmpty(tabl(0)) = True Then
                        Dim xAuthor As XmlNode = xNode.AppendChild(xDoc.CreateElement("Message"))
                        xAuthor.InnerText = "Erreur il manque l'idsrv"
                    ElseIf tabl(0).StartsWith("idsrv=") = False Then
                        Dim xAuthor As XmlNode = xNode.AppendChild(xDoc.CreateElement("Message"))
                        xAuthor.InnerText = "Erreur il manque le parametre idsrv"
                    Else
                        Dim _tp() As String = tabl(0).Split("=")
                        If _tp.Count <> 2 Then
                            Dim xAuthor As XmlNode = xNode.AppendChild(xDoc.CreateElement("Message"))
                            xAuthor.InnerText = "Error"
                        ElseIf String.IsNullOrEmpty(_tp(1)) = True Then
                            Dim xAuthor As XmlNode = xNode.AppendChild(xDoc.CreateElement("Message"))
                            xAuthor.InnerText = "Erreur l'idsrv ne peut être vide"
                        ElseIf _Srv.VerifIdSrv(_tp(1)) = False Then
                            Dim xAuthor As XmlNode = xNode.AppendChild(xDoc.CreateElement("Message"))
                            xAuthor.InnerText = "Erreur l'idsrv est erroné"
                        Else
                            flagIdSrv = True
                        End If
                    End If

                    If flagIdSrv Then
                        'on va cherche la commande
                        If String.IsNullOrEmpty(tabl(1)) = True Then
                            Dim xAuthor As XmlNode = xNode.AppendChild(xDoc.CreateElement("Message"))
                            xAuthor.InnerText = "Erreur il manque la commande"
                        ElseIf tabl(1).StartsWith("cmd=") = False Then
                            Dim xAuthor As XmlNode = xNode.AppendChild(xDoc.CreateElement("Message"))
                            xAuthor.InnerText = "Erreur il manque le nom de la commande"
                        End If
                        Dim _tp() As String = tabl(1).Split("=")
                        If _tp.Count <> 2 Then
                            Dim xAuthor As XmlNode = xNode.AppendChild(xDoc.CreateElement("Message"))
                            xAuthor.InnerText = "Erreur de commande"
                        ElseIf String.IsNullOrEmpty(_tp(1)) = True Then
                            Dim xAuthor As XmlNode = xNode.AppendChild(xDoc.CreateElement("Message"))
                            xAuthor.InnerText = "Erreur la commande ne peut être vide"
                        Else
                            _cmd = _tp(1)
                            flagcmd = True
                        End If
                    End If

                        If flagcmd Then
                            Select Case _cmd
                                Case "ok"
                                    Dim xAuthor As XmlNode = xNode.AppendChild(xDoc.CreateElement("Message"))
                                    xAuthor.InnerText = "OK"
                                Case "getalldevices"
                                    Dim xAuthor As XmlNode = xNode.AppendChild(xDoc.CreateElement("devices"))
                                    Dim _dev As TemplateDevice
                                    For Each _dev In _Srv.GetAllDevices(_IdSrv)
                                        Dim xdev As XmlNode = xAuthor.AppendChild(xDoc.CreateElement("device"))
                                        Dim xdev1 As XmlNode = xdev.AppendChild(xDoc.CreateElement("id"))
                                        xdev1.InnerText = _dev.ID
                                        Dim xdev2 As XmlNode = xdev.AppendChild(xDoc.CreateElement("name"))
                                    xdev2.InnerText = _dev.Name
                                    Dim xdev3 As XmlNode = xdev.AppendChild(xDoc.CreateElement("type"))
                                        xdev3.InnerText = _dev.Type.ToString
                                        Dim xdev5 As XmlNode = xdev.AppendChild(xDoc.CreateElement("enable"))
                                        xdev5.InnerText = _dev.Enable
                                        Dim xdev4 As XmlNode = xdev.AppendChild(xDoc.CreateElement("value"))
                                        xdev4.InnerText = _dev.Value
                                    Next
                                Case "getallzones"
                                    Dim xAuthor As XmlNode = xNode.AppendChild(xDoc.CreateElement("zones"))
                                    Dim _zon As Zone
                                    For Each _zon In _Srv.GetAllZones(_IdSrv)
                                        Dim xdev As XmlNode = xAuthor.AppendChild(xDoc.CreateElement("zone"))
                                        Dim xdev1 As XmlNode = xdev.AppendChild(xDoc.CreateElement("id"))
                                        xdev1.InnerText = _zon.ID
                                        Dim xdev2 As XmlNode = xdev.AppendChild(xDoc.CreateElement("name"))
                                        xdev1.InnerText = _zon.Name
                                        Dim xdev3 As XmlNode = xdev.AppendChild(xDoc.CreateElement("icon"))
                                        xdev3.InnerText = _zon.Icon
                                        Dim xdev4 As XmlNode = xdev.AppendChild(xDoc.CreateElement("image"))
                                        xdev4.InnerText = _zon.Image
                                        Dim _elmt As Zone.Element_Zone
                                        Dim xAuthor2 As XmlNode = xdev.AppendChild(xDoc.CreateElement("elements"))
                                        For Each _elmt In _zon.ListElement
                                            Dim xelmt1 As XmlNode = xAuthor2.AppendChild(xDoc.CreateElement("element"))
                                            Dim xelmt2 As XmlNode = xelmt1.AppendChild(xDoc.CreateElement("id"))
                                            xelmt2.InnerText = _elmt.ElementID
                                            Dim xelmt3 As XmlNode = xelmt1.AppendChild(xDoc.CreateElement("visible"))
                                            xelmt3.InnerText = _elmt.Visible
                                        Next
                                Next
                            Case "getdevice"
                                Dim _id As String = ""
                                'on va cherche la commande
                                If String.IsNullOrEmpty(tabl(2)) = True Then
                                    Dim xAuthor As XmlNode = xNode.AppendChild(xDoc.CreateElement("Message"))
                                    xAuthor.InnerText = "Erreur il manque l'id du device"
                                ElseIf tabl(2).StartsWith("id=") = False Then
                                    Dim xAuthor As XmlNode = xNode.AppendChild(xDoc.CreateElement("Message"))
                                    xAuthor.InnerText = "Erreur il manque l'id du device"
                                End If
                                Dim _tp() As String = tabl(2).Split("=")
                                If _tp.Count <> 2 Then
                                    Dim xAuthor As XmlNode = xNode.AppendChild(xDoc.CreateElement("Message"))
                                    xAuthor.InnerText = "Erreur manque id"
                                Else
                                    _id = _tp(1)
                                    Dim _dev As TemplateDevice = _Srv.ReturnDeviceById(_IdSrv, _id)
                                    If _dev IsNot Nothing Then
                                        Dim xdev As XmlNode = xNode.AppendChild(xDoc.CreateElement("device"))
                                        Dim xdev1 As XmlNode = xdev.AppendChild(xDoc.CreateElement("id"))
                                        xdev1.InnerText = _dev.ID
                                        Dim xdev2 As XmlNode = xdev.AppendChild(xDoc.CreateElement("name"))
                                        xdev1.InnerText = _dev.Name
                                        Dim xdev3 As XmlNode = xdev.AppendChild(xDoc.CreateElement("type"))
                                        xdev3.InnerText = _dev.Type.ToString
                                        Dim xdev5 As XmlNode = xdev.AppendChild(xDoc.CreateElement("enable"))
                                        xdev5.InnerText = _dev.Enable
                                        Dim xdev4 As XmlNode = xdev.AppendChild(xDoc.CreateElement("value"))
                                        xdev4.InnerText = _dev.Value
                                    End If
                                End If
                            Case "ondevice"
                                Dim _id As String = ""
                                'on va cherche la commande
                                If String.IsNullOrEmpty(tabl(2)) = True Then
                                    Dim xAuthor As XmlNode = xNode.AppendChild(xDoc.CreateElement("Message"))
                                    xAuthor.InnerText = "Erreur il manque l'id du device"
                                ElseIf tabl(2).StartsWith("id=") = False Then
                                    Dim xAuthor As XmlNode = xNode.AppendChild(xDoc.CreateElement("Message"))
                                    xAuthor.InnerText = "Erreur il manque l'id du device"
                                End If
                                Dim _tp() As String = tabl(2).Split("=")
                                If _tp.Count <> 2 Then
                                    Dim xAuthor As XmlNode = xNode.AppendChild(xDoc.CreateElement("Message"))
                                    xAuthor.InnerText = "Erreur manque id"
                                Else
                                    _id = _tp(1)
                                    If _id IsNot Nothing Then
                                        Dim x As DeviceAction = New DeviceAction
                                        x.Nom = "On"
                                        _Srv.ExecuteDeviceCommand(_IdSrv, _id, x)
                                    End If
                                End If
                            Case "offdevice"
                                Dim _id As String = ""
                                'on va cherche la commande
                                If String.IsNullOrEmpty(tabl(2)) = True Then
                                    Dim xAuthor As XmlNode = xNode.AppendChild(xDoc.CreateElement("Message"))
                                    xAuthor.InnerText = "Erreur il manque l'id du device"
                                ElseIf tabl(2).StartsWith("id=") = False Then
                                    Dim xAuthor As XmlNode = xNode.AppendChild(xDoc.CreateElement("Message"))
                                    xAuthor.InnerText = "Erreur il manque l'id du device"
                                End If
                                Dim _tp() As String = tabl(2).Split("=")
                                If _tp.Count <> 2 Then
                                    Dim xAuthor As XmlNode = xNode.AppendChild(xDoc.CreateElement("Message"))
                                    xAuthor.InnerText = "Erreur manque id"
                                Else
                                    _id = _tp(1)
                                    If _id IsNot Nothing Then
                                        Dim x As DeviceAction = New DeviceAction
                                        x.Nom = "Off"
                                        _Srv.ExecuteDeviceCommand(_IdSrv, _id, x)
                                    End If
                                End If
                        End Select
                        End If

                        ' Create StringWriter to convert XMLDoc to string
                        Dim xWriter As New IO.StringWriter()
                        Dim xml_writer As New XmlTextWriter(xWriter)
                        xDoc.WriteContentTo(xml_writer)
                        Return xWriter.ToString
                Catch ex As Exception
                    _Srv.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "ReturnResult", "Exception : " & ex.Message)
                    Return Nothing
                End Try
            End Function
        End Class

        Public Function StartSrvWeb() As Integer
            Try
                _httpServer = New MyHttpServer(_PortWeb)
                _thread = New Thread(New ThreadStart(AddressOf _httpServer.listen))
                _thread.Start()
                _IsStart = True
                Return 0
            Catch ex As Exception
                _Srv.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "StartSrvWeb", "Exception : " & ex.Message)
                _IsStart = False
                Return -1
            End Try
        End Function

        Public ReadOnly Property IsStart As Boolean
            Get
                Return _IsStart
            End Get
        End Property

        Public Function StopSrvWeb() As Integer
            Try
                If _thread IsNot Nothing Then _thread.Abort()
                _thread = Nothing
                _httpServer = Nothing
                _IsStart = False
                Return 0
            Catch ex As Exception
                _Srv.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "StopSrvWeb", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        Public Sub New(ByVal Server As Server, ByVal Port As Integer)
            _Srv = Server
            _PortWeb = Port
        End Sub
    End Class
End Namespace