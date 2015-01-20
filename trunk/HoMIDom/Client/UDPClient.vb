Imports System.Data
Imports System.Text
Imports System.Net
Imports System.Net.Sockets
Imports HoMIDom.HoMIDom

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
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "Client UDP SendMessage", "Erreur UDP:" & generatedExceptionName.Message)
            End Try
        End Sub

        Public Sub OnSend(ar As IAsyncResult)
            Try
                clientSocket.EndSend(ar)
            Catch generatedExceptionName As ObjectDisposedException
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "Client UDP OnSend", "Erreur UDP:" & ex.Message)
            End Try
        End Sub

        'Gestion des évènements
#Region "Event"
        ''' <summary>
        ''' Evènement lorsqu'on reçoit un message
        ''' </summary>
        ''' <param name="message"></param>
        ''' <remarks></remarks>
        Public Event OnMessageReceive(message As String)
        Public Event EvtDevice(e As Sequence)
        Public Event EvtDeviceChange(e As Sequence)
        Public Event EvtDeviceAdd(e As Sequence)
        Public Event EvtDeviceDelete(e As Sequence)
        Public Event EvtDriver(e As Sequence)
        Public Event EvtDriverAdd(e As Sequence)
        Public Event EvtDriverChange(e As Sequence)
        Public Event EvtDriverDelete(e As Sequence)
        Public Event EvtHistoryChange(e As Sequence)
        Public Event EvtLog(e As Sequence)
        Public Event EvtServer(e As Sequence)
        Public Event EvtServerShutDown(e As Sequence)
        Public Event EvtServerStart(e As Sequence)
        Public Event EvtMacro(e As Sequence)
        Public Event EvtMacroAdd(e As Sequence)
        Public Event EvtMacroDelete(e As Sequence)
        Public Event EvtMacroChange(e As Sequence)
        Public Event EvtMessage(e As Sequence)
        Public Event EvtNotification(e As Sequence)
        Public Event EvtTrigger(e As Sequence)
        Public Event EvtTriggerAdd(e As Sequence)
        Public Event EvtTriggerChange(e As Sequence)
        Public Event EvtTriggerDelete(e As Sequence)
        Public Event EvtUser(e As Sequence)
        Public Event EvtUserAdd(e As Sequence)
        Public Event EvtUserChange(e As Sequence)
        Public Event EvtUserDelete(e As Sequence)
        Public Event EvtVariable(e As Sequence)
        Public Event EvtVariableAdd(e As Sequence)
        Public Event EvtVariableChange(e As Sequence)
        Public Event EvtVariableDelete(e As Sequence)
        Public Event EvtZone(e As Sequence)
        Public Event EvtZoneAdd(e As Sequence)
        Public Event EvtZoneChange(e As Sequence)
        Public Event EvtZoneDelete(e As Sequence)

#End Region

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
                    OnMessage(msgReceived.strMessage)
                End If

                byteData = New Byte(1023) {}

                'Start listening to receive more data from the user
                clientSocket.BeginReceiveFrom(byteData, 0, byteData.Length, SocketFlags.None, epServer, New AsyncCallback(AddressOf OnReceive), _
                 Nothing)
            Catch generatedExceptionName As ObjectDisposedException
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "Client UDP OnReceive", "Erreur UDP:" & ex.Message)
            End Try
        End Sub

        Public Sub OnMessage(Message As String)
            Try
                RaiseEvent OnMessageReceive(Message)

                Try
                    If String.IsNullOrEmpty(Message) = False Then
                        Dim a() As String
                        a = Message.Split("|")
                        If a.Length > 0 Then
                            Dim seq As Sequence = _serveur.ReturnSequenceFromNumero(a(1))
                            Select Case a(0).ToUpper
                                Case Sequence.TypeOfSequence.Device
                                    RaiseEvent EvtDevice(seq)
                                Case Sequence.TypeOfSequence.DeviceChange
                                    RaiseEvent EvtDeviceChange(seq)
                                Case Sequence.TypeOfSequence.DeviceAdd
                                    RaiseEvent EvtDeviceAdd(seq)
                                Case Sequence.TypeOfSequence.DeviceDelete
                                    RaiseEvent EvtDeviceDelete(seq)
                                Case Sequence.TypeOfSequence.Driver
                                    RaiseEvent EvtDriver(seq)
                                Case Sequence.TypeOfSequence.DriverAdd
                                    RaiseEvent EvtDriverAdd(seq)
                                Case Sequence.TypeOfSequence.DriverChange
                                    RaiseEvent EvtDriverChange(seq)
                                Case Sequence.TypeOfSequence.DriverDelete
                                    RaiseEvent EvtDriverDelete(seq)
                                Case Sequence.TypeOfSequence.HistoryChange
                                    RaiseEvent EvtHistoryChange(seq)
                                Case Sequence.TypeOfSequence.Log
                                    RaiseEvent EvtLog(seq)
                                Case Sequence.TypeOfSequence.Server
                                    RaiseEvent EvtServer(seq)
                                Case Sequence.TypeOfSequence.ServerShutDown
                                    RaiseEvent EvtServerShutDown(seq)
                                Case Sequence.TypeOfSequence.ServerStart
                                    RaiseEvent EvtServerStart(seq)
                                Case Sequence.TypeOfSequence.Macro
                                    RaiseEvent EvtMacro(seq)
                                Case Sequence.TypeOfSequence.MacroAdd
                                    RaiseEvent EvtMacroAdd(seq)
                                Case Sequence.TypeOfSequence.MacroChange
                                    RaiseEvent EvtMacroChange(seq)
                                Case Sequence.TypeOfSequence.MacroDelete
                                    RaiseEvent EvtMacroDelete(seq)
                                Case Sequence.TypeOfSequence.Message
                                    RaiseEvent EvtMessage(seq)
                                Case Sequence.TypeOfSequence.Notification
                                    RaiseEvent EvtNotification(seq)
                                Case Sequence.TypeOfSequence.Trigger
                                    RaiseEvent EvtTrigger(seq)
                                Case Sequence.TypeOfSequence.TriggerAdd
                                    RaiseEvent EvtTriggerAdd(seq)
                                Case Sequence.TypeOfSequence.TriggerChange
                                    RaiseEvent EvtTriggerChange(seq)
                                Case Sequence.TypeOfSequence.TriggerDelete
                                    RaiseEvent EvtTriggerDelete(seq)
                                Case Sequence.TypeOfSequence.User
                                    RaiseEvent EvtUser(seq)
                                Case Sequence.TypeOfSequence.UserAdd
                                    RaiseEvent EvtUserAdd(seq)
                                Case Sequence.TypeOfSequence.UserChange
                                    RaiseEvent EvtUserChange(seq)
                                Case Sequence.TypeOfSequence.UserDelete
                                    RaiseEvent EvtUserDelete(seq)
                                Case Sequence.TypeOfSequence.Variable
                                    RaiseEvent EvtVariable(seq)
                                Case Sequence.TypeOfSequence.VariableAdd
                                    RaiseEvent EvtVariableAdd(seq)
                                Case Sequence.TypeOfSequence.VariableChange
                                    RaiseEvent EvtVariableChange(seq)
                                Case Sequence.TypeOfSequence.VariableDelete
                                    RaiseEvent EvtVariableDelete(seq)
                                Case Sequence.TypeOfSequence.Zone
                                    RaiseEvent EvtZone(seq)
                                Case Sequence.TypeOfSequence.ZoneAdd
                                    RaiseEvent EvtZoneAdd(seq)
                                Case Sequence.TypeOfSequence.ZoneChange
                                    RaiseEvent EvtZoneChange(seq)
                                Case Sequence.TypeOfSequence.ZoneDelete
                                    RaiseEvent EvtZoneDelete(seq)
                                Case Else

                            End Select
                        End If
                    End If
                Catch ex As Exception
                    _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "Client UDP OnMessage", "Erreur OnMessage:" & ex.ToString)
                End Try
            Catch ex As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "Client UDP OnMessage", "Erreur OnMessage:" & ex.ToString)
            End Try
        End Sub

        ''' <summary>
        ''' Se déconnecter du serveur
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Disconnect()
            Try
                'Fill the info for the message to be send
                Dim msgToSend As New Data()

                msgToSend.strName = strName
                msgToSend.strMessage = Nothing
                msgToSend.cmdCommand = Command.Logout

                Dim byteData As Byte() = msgToSend.ToByte()

                'Send it to the server
                clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epServer, New AsyncCallback(AddressOf OnSend), _
                 Nothing)

            Catch generatedExceptionName As Exception
                _serveur.Log(Server.TypeLog.ERREUR, Server.TypeSource.CLIENT, "Client UDP Disconnect", "Erreur UDP:" & generatedExceptionName.Message)
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

                _serveur.Log(Server.TypeLog.INFO, Server.TypeSource.CLIENT, "Client UDP Connect", "Connexion UDP effectuée")
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