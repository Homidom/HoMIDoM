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
                            .Body = _texte
                        End With
                    Catch ex As Exception
                        _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "SendMail", "Erreur lors de l'envoi du mail: " & ex.Message)
                    End Try

                    With (Smtp)
                        .UseDefaultCredentials = False
                        .DeliveryMethod = SmtpDeliveryMethod.Network
                        .Timeout = 100000
                        .EnableSsl = _SSL

                        .Credentials = New Net.NetworkCredential(_Login, _Password) 'De:Votre Email , Pass: password de l'Email
                        .Send(EmailMessage)
                        _Server.Log(Server.TypeLog.DEBUG, Server.TypeSource.SERVEUR, "SendMail", "Message envoyé")
                    End With
                End If
            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "SendMail", "Erreur lors de l'envoi du mail: " & ex.Message)
            End Try
        End Sub

    End Class
End Namespace