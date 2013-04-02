Imports HoMIDom.HoMIDom
Imports System.Net.Mail
Imports System.Net


Namespace HoMIDom

    ''' <summary>
    ''' Envoi un mail
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Mail
        Dim _Server As Server = Nothing
        Dim _adresse As String = ""
        Dim _sujet As String = ""
        Dim _texte As String = ""
        Dim _Login As String = ""
        Dim _Password As String = ""
        Dim _smtpsrv As String = ""
        Dim _smtpport As Integer = 587
        Dim _SSL As Boolean = True
        Dim _De As String = ""

        Public Sub New(ByVal Server As Server, ByVal De As String, ByVal adresse As String, ByVal sujet As String, ByVal texte As String, ByVal smtpserver As String, ByVal smtpPort As Integer, ByVal UserSSL As Boolean, Optional ByVal Login As String = "", Optional ByVal Password As String = "")
            Try
                _Server = Server
                _adresse = adresse
                _De = De
                _sujet = sujet
                _texte = texte
                _Login = Login
                _Password = Password
                _smtpsrv = smtpserver
                _smtpport = smtpPort
                _SSL = UserSSL
            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "New ", "Exception : " & ex.ToString)
            End Try
        End Sub

        Public Sub Send_email()
            Try
                If _adresse <> "" And _sujet <> "" And _texte <> "" And _smtpsrv <> "" Then
                    Dim TheServer As String = _smtpsrv 'pour modifier le serveur selon les besoins
                    Dim Smtp As New SmtpClient(TheServer, _smtpport) 'Simple Mail Transfer Protocol
                    Dim EmailMessage As New MailMessage
                    Try
                        With (EmailMessage)
                            .From = New MailAddress(_De)
                            .To.Add(_adresse)
                            .Subject = _sujet
                            .Body = DecodeCommand(_texte)
                        End With
                    Catch ex As Exception
                        _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "SendMail", "Erreur lors de la préparation du mail: " & ex.ToString)
                    End Try

                    With (Smtp)
                        .UseDefaultCredentials = False
                        .DeliveryMethod = SmtpDeliveryMethod.Network
                        .Timeout = 100000
                        .EnableSsl = _SSL

                        .Credentials = New Net.NetworkCredential(_Login, _Password) 'De:Votre Email , Pass: password de l'Email
                        .Send(EmailMessage)
                        _Server.Log(Server.TypeLog.DEBUG, Server.TypeSource.SERVEUR, "SendMail", "Message envoyé, Message=" & EmailMessage.Body)
                    End With
                End If
            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "SendMail", "Erreur lors de l'envoi du mail: " & ex.ToString)
            End Try
        End Sub

        Public Function DecodeCommand(ByVal Command As String) As String
            Dim retour As String = Command
            Try
                Dim startcmd As Integer = InStr(1, Command, "<")
                Dim endcmd As Integer = InStr(1, Command, ">")
                Dim newcmd As String = Command

                Do While startcmd > 0 And endcmd > 0
                    Dim _device As String = Mid(newcmd, startcmd + 1, endcmd - startcmd - 1)
                    Dim Tabl() As String = _device.Split(".")

                    If Tabl.Length = 1 Then
                        Dim x As Object = _Server.ReturnRealDeviceByName(Tabl(0))
                        If x IsNot Nothing Then
                            _device = x.Value
                        Else
                            _Server.Log(Server.TypeLog.DEBUG, Server.TypeSource.SCRIPT, "DecodeCommand", "Device: " & Tabl(0) & " non trouvé")
                        End If
                    ElseIf Tabl.Length = 2 Then
                        Dim x As Object = _Server.ReturnRealDeviceByName(Tabl(0))
                        If x IsNot Nothing Then
                            Dim value As Object = CallByName(x, Tabl(1), CallType.Get)
                            _device = value
                        End If
                    End If

                    Dim start As String = Mid(newcmd, 1, startcmd - 1)
                    Dim fin As String = Mid(newcmd, endcmd + 1, newcmd.Length - endcmd)
                    newcmd = start & _device & fin
                    retour = newcmd
                    startcmd = InStr(1, newcmd, "<")
                    endcmd = InStr(1, newcmd, ">")
                Loop

                Return retour
            Catch ex As Exception
                retour = "Error:" & ex.ToString
                '_Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SCRIPT, "DecodeCommand", "Error:" & ex.ToString)
                Return retour
            End Try
        End Function

    End Class
End Namespace