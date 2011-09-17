Imports HoMIDom.HoMIDom
Imports System.Net.Mail

''' <summary>
''' Execute chaque Action
''' </summary>
''' <remarks></remarks>
Public Class ThreadAction
    Dim _Server As Server
    Dim _Action As Object

    Public Sub Execute()
        'Gestion du délai
        Dim t As DateTime = DateTime.Now
        t = t.AddSeconds(_Action.timing.Second)
        t = t.AddMinutes(_Action.timing.Minute)
        t = t.AddHours(_Action.timing.Hour)
        Do While DateTime.Now < t

        Loop

        'On execute l'action
        Select Case _Action.TypeAction
            Case Action.TypeAction.ActionDevice
                Dim x As Action.ActionDevice = _Action
                Dim y As New HoMIDom.DeviceAction
                y.Nom = x.Method

                'If x.Parametres IsNot Nothing Then
                '    If x.Parametres.Item(0) IsNot Nothing Then
                '        If x.Parametres.Item(0).Value IsNot Nothing Then
                '            Dim y2 As New HoMIDom.DeviceAction.Parametre
                '            y2.Value = x.Parametres.Item(0)
                '            y.Parametres.Add(y2)
                '        End If
                '    End If
                'End If
                '_Server.ExecuteDeviceCommand(x.IdDevice, y)

            Case Action.TypeAction.ActionIf
                Dim x As Action.ActionIf = _Action
                Dim flag As Boolean = False

                For i As Integer = 0 To x.Conditions.Count - 1
                    If x.Conditions.Item(i).Type = Action.TypeCondition.Device Then

                    End If
                Next

            Case Action.TypeAction.ActionMacro
                Dim x As Action.ActionMacro = _Action
                Dim y As Macro = _Server.ReturnMacroById(x.IdMacro)
                y.Execute(_Server)

            Case Action.TypeAction.ActionMail
                Dim x As Action.ActionMail = _Action
                Send_email(_Server.ReturnUserById(x.UserId).eMail, x.Sujet, x.Message)

        End Select
    End Sub

    Public Sub New(ByVal Server As Server, ByVal Action As Object)
        _Server = Server
        _Action = Action
    End Sub

    Public Sub Send_email(ByVal adresse As String, ByVal sujet As String, ByVal texte As String)
        If adresse <> "" And sujet <> "" And texte <> "" And _Server.GetSMTPServeur <> "" Then
            Try

                'envoi de l'email à adresse avec sujet et texte via les smtp définis dans le serveur
                Dim email As System.Net.Mail.MailMessage = New System.Net.Mail.MailMessage()
                email.From = New MailAddress(_Server.GetSMTPMailServeur)
                email.To.Add(adresse)
                email.Subject = sujet
                email.Body = texte
                Dim mailSender As New System.Net.Mail.SmtpClient(_Server.GetSMTPServeur)

                If _Server.GetSMTPLogin() <> "" Then
                    mailSender.Credentials = New Net.NetworkCredential(_Server.GetSMTPLogin, _Server.GetSMTPPassword)
                    '
                    'email.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", "1")
                    'email.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", mMailServerLogin)
                    'email.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", mMailServerPassword)
                End If

                mailSender.Send(email)
                email = Nothing
                mailSender = Nothing

            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SCRIPT, "SendMail", "Erreur lors de l'envoi du mail: " & ex.Message)
            End Try
        End If
    End Sub
End Class
