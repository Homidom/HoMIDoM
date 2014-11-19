Imports System.Data
Imports System.Text
Imports System.Net
Imports System.Net.Sockets

Namespace HoMIDom

    ''' <summary>
    ''' Client UDP
    ''' </summary>
    ''' <remarks></remarks>
    Public Class UDPClient
        Public clientSocket As Socket
        'The main client socket
        Public strName As String
        'Name by which the user logs into the room
        Public epServer As EndPoint
        'The EndPoint of the server
        Private byteData As Byte() = New Byte(1023) {}
        'Liste des clients
        Dim ListClients As New List(Of String)
        'Serveur
        Dim _serveur As IHoMIDom

        Public Sub New(Server As IHoMIDom)
            _serveur = Server
        End Sub

        ''' <summary>
        ''' Envoyer un message au serveur
        ''' </summary>
        ''' <param name="Message"></param>
        ''' <remarks></remarks>
        Public Sub SendMessage(Message As String)
            Try
                'Fill the info for the message to be send
                Dim msgToSend As New Data()

                msgToSend.strName = strName
                msgToSend.strMessage = Message
                msgToSend.cmdCommand = Command.Message

                Dim byteData As Byte() = msgToSend.ToByte()

                'Send it to the server
                clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epServer, New AsyncCallback(AddressOf OnSend), _
                 Nothing)

            Catch generatedExceptionName As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "Client UDP SendMessage", "Erreur UDP:" & generatedExceptionName.Message)
            End Try
        End Sub

        Public Sub OnSend(ar As IAsyncResult)
            Try
                clientSocket.EndSend(ar)
            Catch generatedExceptionName As ObjectDisposedException
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "Client UDP OnSend", "Erreur UDP:" & ex.Message)
            End Try
        End Sub

        ''' <summary>
        ''' Evènement lorsqu'on reçoit un message
        ''' </summary>
        ''' <param name="message"></param>
        ''' <remarks></remarks>
        Public Event OnMessageReceive(message As String)

        Public Sub OnReceive(ar As IAsyncResult) 
            Try
                clientSocket.EndReceive(ar)

                'Convert the bytes received into an object of type Data
                Dim msgReceived As New Data(byteData)

                'Accordingly process the message received
                Select Case msgReceived.cmdCommand
                    Case Command.Login
                        ListClients.Add(msgReceived.strName)
                        Exit Select

                    Case Command.Logout
                        ListClients.Remove(msgReceived.strName)
                        Exit Select

                    Case Command.Message
                        Exit Select

                    Case Command.List
                        ListClients.AddRange(msgReceived.strMessage.Split("*"c))
                        ListClients.RemoveAt(ListClients.Count - 1)
                        RaiseEvent OnMessageReceive("<<<" & strName & " a rejoind le réseau>>>")
                        Exit Select
                End Select

                If msgReceived.strMessage IsNot Nothing AndAlso msgReceived.cmdCommand <> Command.List Then
                    RaiseEvent OnMessageReceive(msgReceived.strMessage)
                End If

                byteData = New Byte(1023) {}

                'Start listening to receive more data from the user
                clientSocket.BeginReceiveFrom(byteData, 0, byteData.Length, SocketFlags.None, epServer, New AsyncCallback(AddressOf OnReceive), _
                 Nothing)
            Catch generatedExceptionName As ObjectDisposedException
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "Client UDP OnReceive", "Erreur UDP:" & ex.Message)
            End Try
        End Sub

        ''' <summary>
        ''' Permet de se connecter au serveur UDP
        ''' </summary>
        ''' <param name="NameClient">Nom du client</param>
        ''' <param name="IPServeur">Adresse IP du serveur</param>
        ''' <param name="Port">Port à utilisé (utilise celui actuel -1)</param>
        ''' <remarks></remarks>
        Public Sub Connect(NameClient As String, IPServeur As String, Port As Integer)
            Try
                _serveur.Log(Server.TypeLog.INFO, Server.TypeSource.CLIENT, "Client UDP Connect", "Tentative de connexion UDP (" & IPServeur & ":" & Port & ")")

                'Using UDP sockets
                clientSocket = New Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)

                'IP address of the server machine
                Dim ipAddress__1 As IPAddress = IPAddress.Parse(IPServeur)
                'Server is listening on port 
                Dim ipEndPoint As New IPEndPoint(ipAddress__1, Port)

                epServer = DirectCast(ipEndPoint, EndPoint)

                Dim msgToSend As New Data()
                msgToSend.cmdCommand = Command.Login
                msgToSend.strMessage = Nothing
                msgToSend.strName = NameClient

                Dim byteData As Byte() = msgToSend.ToByte()

                'Login to the server
                clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epServer, New AsyncCallback(AddressOf OnSend2), _
                 Nothing)


                'The user has logged into the system so we now request the server to send
                'the names of all users who are in the chat room
                msgToSend.cmdCommand = Command.List
                msgToSend.strName = strName
                msgToSend.strMessage = Nothing

                byteData = msgToSend.ToByte()

                clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epServer, New AsyncCallback(AddressOf OnSend), _
                 Nothing)

                byteData = New Byte(1023) {}
                'Start listening to the data asynchronously
                clientSocket.BeginReceiveFrom(byteData, 0, byteData.Length, SocketFlags.None, epServer, New AsyncCallback(AddressOf OnReceive), _
                 Nothing)
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "Client UDP Connect", "Erreur UDP:" & ex.Message)
            End Try
        End Sub


        Public Sub OnSend2(ar As IAsyncResult)
            Try
                clientSocket.EndSend(ar)
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "Client UDP OnSend2", "Erreur UDP:" & ex.Message)
            End Try
        End Sub
    End Class

End Namespace