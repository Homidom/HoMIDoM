Imports System.Text

Namespace HoMIDom

    Module UDP
        ''' <summary>
        ''' Permet d'envoyer un message vers tous les clients
        ''' </summary>
        ''' <param name="Message"></param>
        ''' <remarks></remarks>
        Public Sub SendMessageToClient(Message As String, Optional TypeMessage As String = "")
            Try
                Server.Instance.Log(Server.TypeLog.DEBUG, Server.TypeSource.SERVEUR, "SendMessageToClient", "Envoi du message UDP:" & Message)

                Dim udpClient As New Net.Sockets.UdpClient()
                udpClient.Connect(Server.Instance.GetIPSOAP, Server.Instance.GetPortSOAP - 1)
                Dim senddata As Byte()
                senddata = Encoding.ASCII.GetBytes(Message)
                udpClient.Send(senddata, senddata.Length)
            Catch ex As Exception
                Server.Instance.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "SendMessageToClient", "Erreur UDP:" & ex.Message)
            End Try
        End Sub
    End Module

End Namespace
