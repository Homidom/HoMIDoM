Imports HoMIDom.HoMIDom
Imports System.Net.Mail
Imports System.Threading
Imports System.Speech
Imports System.Net
Imports System.IO
Imports System.Xml
Imports System.Reflection
Imports System.CodeDom.Compiler

Namespace HoMIDom

    ''' <summary>
    ''' Execute chaque Action
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ThreadAction
        Dim _Server As Server
        Dim _Action As Object

        Public Sub Execute()
            Try
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

                        If x.Method.StartsWith("{") And x.Method.EndsWith("}") Then
                            'c'est une commande avancée
                            Dim _cmdav As String = Mid(x.Method, 2, x.Method.Length - 2)
                            y.Nom = "ExecuteCommand"

                            Dim param As New DeviceAction.Parametre
                            param.Value = _cmdav
                            y.Parametres.Add(param)

                            If x.Parametres.Count > 1 Then
                                Dim param1 As New DeviceAction.Parametre
                                param1.Value = x.Parametres.Item(0)
                                y.Parametres.Add(param1)
                            End If
                        Else
                            'C'est une commande standard
                            y.Nom = x.Method

                            If x.Parametres IsNot Nothing Then
                                If x.Parametres.Count > 0 Then
                                    If x.Parametres.Item(0) IsNot Nothing Then
                                        If x.Parametres.Item(0) <> "" Then
                                            Dim y2 As New HoMIDom.DeviceAction.Parametre
                                            y2.Value = x.Parametres.Item(0)
                                            y.Parametres.Add(y2)
                                        End If
                                    End If
                                End If
                            End If
                        End If

                        _Server.ExecuteDeviceCommand(_IdSrv, x.IdDevice, y)

                    Case Action.TypeAction.ActionIf
                        Dim x As Action.ActionIf = _Action
                        Dim flag As Boolean = False

                        For i As Integer = 0 To x.Conditions.Count - 1
                            Dim result As Boolean

                            If x.Conditions.Item(i).Type = Action.TypeCondition.Device Then
                                Dim retour As Object = CallByName(_Server.ReturnRealDeviceById(x.Conditions.Item(i).IdDevice), x.Conditions.Item(i).PropertyDevice, CallType.Get)
                                Dim retour2 As Object
                                If x.Conditions.Item(i).Value.ToString.StartsWith("[") And x.Conditions.Item(i).Value.ToString.EndsWith("]") Then
                                    Dim a() As String = x.Conditions.Item(i).Value.ToString.Split("|")
                                    If a.Length = 3 Then
                                        retour2 = CallByName(_Server.ReturnRealDeviceById(Mid(a(2), 2, Len(a(2)) - 1)), Len(a(2)) - 1, CallType.Get)
                                    Else
                                        retour2 = x.Conditions.Item(i).Value
                                    End If
                                Else
                                    retour2 = x.Conditions.Item(i).Value
                                End If

                                Select Case x.Conditions.Item(i).Condition
                                    Case Action.TypeSigne.Egal
                                        If retour = retour2 Then
                                            result = True
                                        Else
                                            result = False
                                        End If
                                    Case Action.TypeSigne.Different
                                        If retour <> retour2 Then
                                            result = True
                                        Else
                                            result = False
                                        End If
                                    Case Action.TypeSigne.Inferieur
                                        If retour < retour2 Then
                                            result = True
                                        Else
                                            result = False
                                        End If
                                    Case Action.TypeSigne.InferieurEgal
                                        If retour <= retour2 Then
                                            result = True
                                        Else
                                            result = False
                                        End If
                                    Case Action.TypeSigne.Superieur
                                        If retour > retour2 Then
                                            result = True
                                        Else
                                            result = False
                                        End If
                                    Case Action.TypeSigne.SuperieurEgal
                                        If retour > retour2 Then
                                            result = True
                                        Else
                                            result = False
                                        End If
                                End Select
                                If i = 0 Then 'c le 1er donc pas prendre en compte l'operateur
                                    flag = result
                                Else
                                    Select Case x.Conditions.Item(i).Operateur
                                        Case Action.TypeOperateur.AND
                                            flag = flag And result
                                        Case Action.TypeOperateur.OR
                                            flag = flag Or result
                                        Case Action.TypeOperateur.NONE
                                            flag = result
                                    End Select
                                End If
                            End If

                            If x.Conditions.Item(i).Type = Action.TypeCondition.DateTime Then
                                Dim a1 As DateTime
                                Dim sc As Integer = 0
                                Dim mn As Integer = 0
                                Dim hr As Integer = 0
                                Dim dd As Integer = 0
                                Dim ms As Integer = 0
                                Dim Flagjour As Boolean = False 'True si on travail sur un jour
                                Dim _tim() As String = x.Conditions.Item(i).DateTime.Split("#")

                                'On travail juste sur une heure
                                If (_tim(0) <> "*" Or _tim(1) <> "*" Or _tim(2) <> "*") And _tim(3) = "*" And _tim(4) = "*" And _tim(5) = "" And _tim(6) <> "1" And _tim(7) <> "1" Then
                                    If _tim(0) = "*" Then
                                        sc = Now.Second
                                    Else
                                        sc = CInt(_tim(0))
                                    End If
                                    If _tim(1) = "*" Then
                                        mn = Now.Minute
                                    Else
                                        mn = CInt(_tim(1))
                                    End If
                                    If _tim(2) = "*" Then
                                        hr = Now.Hour
                                    Else
                                        hr = CInt(_tim(2))
                                    End If
                                    a1 = New Date(Now.Year, Now.Month, Now.Day, hr, mn, sc)
                                End If
                                'On travail sur une heure + date
                                If (_tim(0) <> "*" Or _tim(1) <> "*" Or _tim(2) <> "*") And (_tim(3) <> "*" Or _tim(4) <> "*") And _tim(5) = "" And _tim(6) <> "1" And _tim(7) <> "1" Then
                                    If _tim(0) = "*" Then
                                        sc = Now.Second
                                    Else
                                        sc = CInt(_tim(0))
                                    End If
                                    If _tim(1) = "*" Then
                                        mn = Now.Minute
                                    Else
                                        mn = CInt(_tim(1))
                                    End If
                                    If _tim(2) = "*" Then
                                        hr = Now.Hour
                                    Else
                                        hr = CInt(_tim(2))
                                    End If
                                    If _tim(3) = "*" Then
                                        dd = Now.Day
                                    Else
                                        dd = CInt(_tim(3))
                                    End If
                                    If _tim(4) = "*" Then
                                        ms = Now.Month
                                    Else
                                        ms = CInt(_tim(4))
                                    End If
                                    a1 = New Date(Now.Year, ms, dd, hr, mn, sc)
                                End If
                                'On travail sur une heure + jour
                                If (_tim(0) <> "*" Or _tim(1) <> "*" Or _tim(2) <> "*") And (_tim(3) = "*" And _tim(4) = "*") And _tim(5) <> "" And _tim(6) <> "1" And _tim(7) <> "1" Then
                                    If _tim(0) = "*" Then
                                        sc = Now.Second
                                    Else
                                        sc = CInt(_tim(0))
                                    End If
                                    If _tim(1) = "*" Then
                                        mn = Now.Minute
                                    Else
                                        mn = CInt(_tim(1))
                                    End If
                                    If _tim(2) = "*" Then
                                        hr = Now.Hour
                                    Else
                                        hr = CInt(_tim(2))
                                    End If
                                    a1 = New Date(Now.Year, Now.Month, Now.Day, hr, mn, sc)
                                    Flagjour = True
                                End If
                                'On travail sur une date
                                If (_tim(0) = "*" And _tim(1) = "*" And _tim(2) = "*") And (_tim(3) <> "*" Or _tim(4) <> "*") And _tim(5) = "" And _tim(6) <> "1" And _tim(7) <> "1" Then
                                    If _tim(3) = "*" Then
                                        dd = Now.Day
                                    Else
                                        dd = CInt(_tim(3))
                                    End If
                                    If _tim(4) = "*" Then
                                        ms = Now.Month
                                    Else
                                        ms = CInt(_tim(4))
                                    End If
                                    a1 = New Date(Now.Year, ms, dd, Now.Hour, Now.Minute, Now.Second)
                                End If
                                'On travail sur un jour
                                If (_tim(0) = "*" And _tim(1) = "*" And _tim(2) = "*") And _tim(3) = "*" And _tim(4) = "*" And _tim(5) <> "" And _tim(6) <> "1" And _tim(7) <> "1" Then
                                    flag = True
                                    a1 = Now
                                End If
                                'On travail sur heure levé du soleil
                                If (_tim(0) = "*" And _tim(1) = "*" And _tim(2) = "*") And _tim(3) = "*" And _tim(4) = "*" And _tim(5) = "" And _tim(6) = "1" And _tim(7) <> "1" Then
                                    a1 = CDate(_Server.GetHeureLeverSoleil)
                                End If
                                'On travail sur heure couché du soleil
                                If (_tim(0) = "*" And _tim(1) = "*" And _tim(2) = "*") And _tim(3) = "*" And _tim(4) = "*" And _tim(5) = "" And _tim(6) <> "1" And _tim(7) = "1" Then
                                    a1 = CDate(_Server.GetHeureCoucherSoleil)
                                End If


                                Select Case x.Conditions.Item(i).Condition
                                    Case Action.TypeSigne.Egal
                                        If DateDiff(DateInterval.Second, Now, a1) = 0 Then
                                            result = True
                                        Else
                                            result = False
                                        End If
                                    Case Action.TypeSigne.Different
                                        If DateDiff(DateInterval.Second, Now, a1) <> 0 Then
                                            result = True
                                        Else
                                            result = False
                                        End If
                                    Case Action.TypeSigne.Inferieur
                                        If DateDiff(DateInterval.Second, Now, a1) < 0 Then
                                            result = True
                                        Else
                                            result = False
                                        End If
                                    Case Action.TypeSigne.InferieurEgal
                                        If DateDiff(DateInterval.Second, Now, a1) <= 0 Then
                                            result = True
                                        Else
                                            result = False
                                        End If
                                    Case Action.TypeSigne.Superieur
                                        If DateDiff(DateInterval.Second, Now, a1) > 0 Then
                                            result = True
                                        Else
                                            result = False
                                        End If
                                    Case Action.TypeSigne.SuperieurEgal
                                        If DateDiff(DateInterval.Second, Now, a1) >= 0 Then
                                            result = True
                                        Else
                                            result = False
                                        End If
                                End Select
                                If Flagjour = True Then
                                    Dim jj As String = UCase(Now.ToString("dddd"))
                                    If (InStr(_tim(5), "0") > 0 And Now.DayOfWeek = DayOfWeek.Sunday) Or (InStr(_tim(5), "1") > 0 And Now.DayOfWeek = DayOfWeek.Monday) Or (InStr(_tim(5), "2") > 0 And Now.DayOfWeek = DayOfWeek.Tuesday) Or (InStr(_tim(5), "3") > 0 And Now.DayOfWeek = DayOfWeek.Wednesday) Or (InStr(_tim(5), "4") > 0 And Now.DayOfWeek = DayOfWeek.Thursday) Or (InStr(_tim(5), "5") > 0 And Now.DayOfWeek = DayOfWeek.Friday) Or (InStr(_tim(5), "6") > 0 And Now.DayOfWeek = DayOfWeek.Saturday) Then
                                        result = result And True
                                    Else
                                        result = result And False
                                    End If
                                End If

                                If i = 0 Then
                                    flag = result
                                Else
                                    Select Case x.Conditions.Item(i).Operateur
                                        Case Action.TypeOperateur.AND
                                            flag = flag And result
                                        Case Action.TypeOperateur.OR
                                            flag = flag Or result
                                        Case Action.TypeOperateur.NONE
                                            flag = result
                                    End Select
                                End If
                            End If
                        Next
                        If flag = True Then
                            For i As Integer = 0 To x.ListTrue.Count - 1
                                Dim _action As New ThreadAction(_Server, x.ListTrue.Item(i))
                                Dim y As New Thread(AddressOf _action.Execute)
                                y.Name = "Traitement du script"
                                y.Start()
                                y = Nothing
                            Next
                        Else
                            For i As Integer = 0 To x.ListFalse.Count - 1
                                Dim _action As New ThreadAction(_Server, x.ListFalse.Item(i))
                                Dim y As New Thread(AddressOf _action.Execute)
                                y.Name = "Traitement du script"
                                y.Start()
                                y = Nothing
                            Next
                        End If
                    Case Action.TypeAction.ActionMacro
                        Dim x As Action.ActionMacro = _Action
                        Dim y As Macro = _Server.ReturnMacroById(_IdSrv, x.IdMacro)
                        If y IsNot Nothing Then
                            y.Execute(_Server)
                        Else
                            _Server.Log(Server.TypeLog.MESSAGE, Server.TypeSource.SCRIPT, "ThreadAction Execute", "La macro Id:" & x.IdMacro & " n'a pas pu être trouvée donc elle ne pourra pas être lancée")
                        End If


                    Case Action.TypeAction.ActionMail
                        Dim x As Action.ActionMail = _Action
                        Dim _user As Users.User = _Server.ReturnUserById(_IdSrv, x.UserId)
                        If _user IsNot Nothing Then
                            Send_email(_user.eMail, x.Sujet, x.Message)
                        Else
                            _Server.Log(Server.TypeLog.MESSAGE, Server.TypeSource.SCRIPT, "ThreadAction Execute", "Le user Id:" & x.UserId & " n'a pas pu être trouvé, le mail ne pourra pas être donc envoyé")
                        End If

                    Case Action.TypeAction.ActionVB
                        Dim x As Action.ActionVB = _Action
                        ExecuteScript(x.Script)

                    Case Action.TypeAction.ActionSpeech
                        Dim x As Action.ActionSpeech = _Action
                        Parler(x.Message)
                    Case Action.TypeAction.ActionHttp
                        Dim x As Action.ActionHttp = _Action
                        Sendhttp(x.Commande)
                    Case Action.TypeAction.ActionLogEvent
                        Dim x As Action.ActionLogEvent = _Action
                        _Server.LogEvent(x.Message, x.Type, x.Eventid)
                    Case Action.TypeAction.ActionLogEventHomidom
                        Dim x As Action.ActionLogEventHomidom = _Action
                        _Server.Log(x.Type, HoMIDom.Server.TypeSource.SCRIPT, x.Fonction, x.Message)
                    Case Action.TypeAction.ActionDOS
                        Dim x As Action.ActionDos = _Action
                        Try
                            If File.Exists(x.Fichier) Then
                                Dim process As Process = New Process()
                                process.StartInfo.FileName = x.Fichier
                                process.StartInfo.Arguments = x.Arguments
                                process.Start()
                            Else
                                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "ThreadAction Execute", "ActionDos : le fichier " & x.Fichier & " n'existe pas")
                            End If
                        Catch ex As Exception
                            _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "ThreadAction Execute", "Erreur ActionDos : " & ex.ToString)
                        End Try
                End Select
            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "ThreadAction Execute", "Exception : " & ex.ToString)
            End Try
        End Sub

        Public Sub New(ByVal Server As Server, ByVal Action As Object)
            Try
                _Server = Server
                _Action = Action
            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "New Execute", "Exception : " & ex.ToString)
            End Try
        End Sub

        Public Sub Sendhttp(ByVal Commande As String)
            Try

                Dim reader As StreamReader = Nothing
                Dim str As String = ""
                Dim request As WebRequest = WebRequest.Create(DecodeCommand(Commande))
                Dim response As WebResponse = request.GetResponse()
                reader = New StreamReader(response.GetResponseStream())
                str = reader.ReadToEnd
                reader.Close()
            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SCRIPT, "Sendhttp", "Erreur lors de l'envoi de la commande http: " & ex.Message)
            End Try
        End Sub


        Public Sub Send_email(ByVal adresse As String, ByVal sujet As String, ByVal texte As String)
            Try
                If adresse <> "" And sujet <> "" And texte <> "" And _Server.GetSMTPServeur(_IdSrv) <> "" Then
                    'envoi de l'email à adresse avec sujet et texte via les smtp définis dans le serveur
                    Dim email As System.Net.Mail.MailMessage = New System.Net.Mail.MailMessage()
                    email.From = New MailAddress(_Server.GetSMTPMailServeur(_IdSrv))
                    email.To.Add(adresse)
                    email.Subject = sujet
                    email.Body = DecodeCommand(texte)
                    Dim mailSender As New System.Net.Mail.SmtpClient(_Server.GetSMTPServeur(_IdSrv))

                    If _Server.GetSMTPLogin(_IdSrv) <> "" Then
                        mailSender.Credentials = New Net.NetworkCredential(_Server.GetSMTPLogin(_IdSrv), _Server.GetSMTPPassword(_IdSrv))
                        '
                        'email.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", "1")
                        'email.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", mMailServerLogin)
                        'email.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", mMailServerPassword)
                    End If

                    mailSender.Send(email)

                    _Server.Log(Server.TypeLog.DEBUG, Server.TypeSource.SCRIPT, "SendMail", "Envoi du mail effectué, Adresse:" & adresse & " Sujet: " & sujet & " Message: " & texte)
                    email = Nothing
                    mailSender = Nothing
                End If
            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SCRIPT, "SendMail", "Erreur lors de l'envoi du mail: " & ex.Message)
            End Try
        End Sub

        Private Sub Parler(ByVal Message As String)
            Dim texte As String = Message
            'remplace les balises par la valeur
            texte = texte.Replace("{time}", Now.ToShortTimeString)
            texte = texte.Replace("{date}", Now.ToLongDateString)
            texte = DecodeCommand(texte)

            Try
                Dim lamachineaparler As New Speech.Synthesis.SpeechSynthesizer
                _Server.Log(Server.TypeLog.DEBUG, Server.TypeSource.SCRIPT, "Parler", "Message:" & texte)
                With lamachineaparler
                    .SelectVoice(_Server.GetDefautVoice)
                    '.SetOutputToWaveFile("C:\tet.wav")
                    '.SetOutputToWaveFile(File)
                    .SpeakAsync(texte)
                End With
                texte = Nothing
                lamachineaparler = Nothing
            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SCRIPT, "Parler", "Erreur lors de l'annonce du message: " & Message & " : " & ex.ToString)
            End Try
        End Sub

        Private Sub ExecuteScript(ByVal ScriptSource As String)
            '
            ' Instantiate the VB compiler.
            '
            Dim objCodeCompiler As ICodeCompiler = New VBCodeProvider().CreateCompiler

            '
            ' Pass parameters into the compiler.
            '
            Dim objCompilerParameters As New CompilerParameters
            objCompilerParameters.ReferencedAssemblies.Add("System.dll")
            objCompilerParameters.ReferencedAssemblies.Add("System.Windows.Forms.dll")
            objCompilerParameters.ReferencedAssemblies.Add("Microsoft.VisualBasic.dll")
            objCompilerParameters.ReferencedAssemblies.Add("Homidom.dll")
            objCompilerParameters.GenerateInMemory = True

            '
            ' Get te source code and compile it.
            '
            Dim strCode As String = ScriptSource
            Dim objCompileResults As CompilerResults = objCodeCompiler.CompileAssemblyFromSource(objCompilerParameters, strCode)

            '
            ' Check for compiler errors.
            '
            If objCompileResults.Errors.HasErrors Then
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SCRIPT, "ExecuteScript", "Erreur lors de l'execution du script: Line>" & objCompileResults.Errors(0).Line.ToString & ", " & objCompileResults.Errors(0).ErrorText)
                Exit Sub
            End If

            '
            ' Get a reference to the assembly.
            '
            Dim objAssembly As System.Reflection.Assembly = objCompileResults.CompiledAssembly

            '
            ' Create an instance of the DynamicCode class referenced in the source code.
            '
            Dim objTheClass As Object = objAssembly.CreateInstance("Dynam.DynamicCode")

            If objTheClass Is Nothing Then
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SCRIPT, "ExecuteScript", "Unable to load class.")
                Exit Sub
            End If

            '
            ' Create a parameter to be passed into the ExecuteCode function in class DynamicCode.
            '
            Dim objFunctionParameters(0) As Object
            objFunctionParameters(0) = _Server

            '
            ' Call the DynamicCode.ExecuteCode
            '
            Try
                Dim objResult As Object = objTheClass.GetType.InvokeMember("ExecuteCode", _
                                BindingFlags.InvokeMethod, Nothing, objTheClass, objFunctionParameters)

            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SCRIPT, "ExecuteScript", "Error:" & ex.Message)
            End Try
        End Sub

        Public Function DecodeCommand(ByVal Command As String) As String
            Try
                Dim startcmd As Integer = InStr(1, Command, "<")
                Dim endcmd As Integer = InStr(1, Command, ">")
                Dim retour As String = Command

                Do While startcmd > 0 And endcmd > 0
                    Dim _device As String = Mid(Command, startcmd + 1, endcmd - startcmd - 1)
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

                    Dim start As String = Mid(Command, 1, startcmd - 1)
                    Dim fin As String = Mid(Command, endcmd + 1, Command.Length - endcmd)
                    Dim newcmd As String = start & _device & fin

                    retour = newcmd
                    startcmd = InStr(endcmd + 1, Command, "<")
                    endcmd = InStr(startcmd + 1, Command, ">")
                Loop

                Return retour
            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SCRIPT, "DecodeCommand", "Error:" & ex.ToString)
                Return Command
            End Try
        End Function

    End Class
End Namespace