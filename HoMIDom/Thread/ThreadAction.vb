Imports HoMIDom.HoMIDom
Imports System.Net.Mail
Imports System.Threading
Imports System.Speech
Imports System.Net
Imports System.IO
Imports System.Xml
Imports System.Reflection
Imports System.CodeDom
Imports System.CodeDom.Compiler
Imports HoMIDom.HoMIDom.Server
Imports STRGS = Microsoft.VisualBasic.Strings

Namespace HoMIDom

    ''' <summary>
    ''' Execute chaque Action
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ThreadAction
        Dim _Server As Server = Nothing
        Dim _Action As Object = Nothing
        Dim _NameMacro As String = ""
        Dim _ID As String = ""

        Public Sub Execute()
            Try
                'Gestion du délai
                Dim t As DateTime = DateTime.Now
                t = t.AddSeconds(_Action.timing.Second)
                t = t.AddMinutes(_Action.timing.Minute)
                t = t.AddHours(_Action.timing.Hour)

                Do While DateTime.Now < t
                    Thread.Sleep(1000)
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
                            _Server.Log(Server.TypeLog.DEBUG, Server.TypeSource.SCRIPT, "Execute", "commande avancée: " & y.Nom)

                            Dim param As New DeviceAction.Parametre
                            param.Value = _cmdav
                            y.Parametres.Add(param)
                            _Server.Log(Server.TypeLog.DEBUG, Server.TypeSource.SCRIPT, "Execute", "param0: " & _cmdav)

                            'si on a des parametres et qu'ils sont corrects, on les ajoute
                            If x.Parametres.Count > 0 Then
                                For idx As Integer = 0 To x.Parametres.Count - 1
                                    Dim param1 As New DeviceAction.Parametre
                                    If x.Parametres.Item(idx) IsNot Nothing Then
                                        'on recupere la valeur du parametre et on remplace les <compnom.value> et <comnom> par leur valeur
                                        Dim _var0 As String = DecodeCommand(x.Parametres.Item(idx).ToString)
                                        'si il y a une erreur on n'ajoute pas le parametre et on logue l'erreur
                                        If STRGS.InStr(_var0, "ERR:") > 0 Then
                                            _Server.Log(TypeLog.ERREUR, TypeSource.SERVEUR, "ThreadAction Execute DeviceAction", STRGS.Right(_var0, _var0.Length - 5))
                                        Else
                                            If String.IsNullOrEmpty(_var0) Then
                                                param1.Value = x.Parametres.Item(idx)
                                            Else
                                                'param1.Value = _var0
                                                param1.Value = EvaluateCommand(_var0)
                                            End If
                                            _Server.Log(Server.TypeLog.DEBUG, Server.TypeSource.SCRIPT, "Execute", "param" & idx + 1 & ": " & param1.Value)
                                            y.Parametres.Add(param1)
                                        End If
                                    End If
                                Next
                            End If
                        Else
                            'C'est une commande standard
                            y.Nom = x.Method

                            'si on a un parametre et qu'il est correct, on l'ajoute
                            If x.Parametres IsNot Nothing Then
                                If x.Parametres.Count > 0 Then
                                    If x.Parametres.Item(0) IsNot Nothing Then
                                        If x.Parametres.Item(0) <> "" Then
                                            'on recupere la valeur du parametre et on remplace les <compnom.value> et <comnom> par leur valeur
                                            Dim _var0 As String = DecodeCommand(x.Parametres.Item(0).ToString)
                                            'si il y a une erreur on n'ajoute pas le parametre et on logue l'erreur
                                            If STRGS.InStr(_var0, "ERR:") > 0 Then
                                                _Server.Log(TypeLog.ERREUR, TypeSource.SERVEUR, "ThreadAction Execute DeviceAction", STRGS.Right(_var0, _var0.Length - 5))
                                            Else
                                                Dim y2 As New HoMIDom.DeviceAction.Parametre
                                                If String.IsNullOrEmpty(_var0) Then
                                                    y2.Value = x.Parametres.Item(0)
                                                Else
                                                    'y2.Value = _var0
                                                    y2.Value = EvaluateCommand(_var0)

                                                End If
                                                y.Parametres.Add(y2)
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If

                        _Server.ExecuteDeviceCommand(_IdSrv, x.IdDevice, y)
                    Case Action.TypeAction.ActionDriver
                        Dim x As Action.ActionDriver = _Action
                        Dim y As New HoMIDom.DeviceAction

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

                        If y.Nom.ToUpper = "START" Then _Server.StartDriver(_IdSrv, x.IdDriver)
                        If y.Nom.ToUpper = "STOP" Then _Server.StopDriver(_IdSrv, x.IdDriver)
                        If y.Nom.ToUpper = "RESTART" Then
                            _Server.StopDriver(_IdSrv, x.IdDriver)
                            Thread.Sleep(2000)
                            _Server.StartDriver(_IdSrv, x.IdDriver)
                            Thread.Sleep(2000)
                        End If
                    Case Action.TypeAction.ActionIf
                        Dim x As Action.ActionIf = _Action
                        Dim flag As Boolean = False

                        For i As Integer = 0 To x.Conditions.Count - 1
                            Dim result As Boolean

                            If x.Conditions.Item(i).Type = Action.TypeCondition.Device Then
                                Dim retour As Object = CallByName(_Server.ReturnRealDeviceById(x.Conditions.Item(i).IdDevice), x.Conditions.Item(i).PropertyDevice, CallType.Get) 'Recup la valeur du device
                                Dim retour2 As Object = Nothing 'Valeur à comparer

                                '========== Version 1 ========== 
                                'Dim _var0 As String = x.Conditions.Item(i).Value.ToString
                                '_var0 = _var0.Replace("&lt;", "<")
                                '_var0 = _var0.Replace("&gt;", ">")
                                '_var0 = DecodeCommand(_var0) 'decodage pour remplacement de <compnom> par la valeur
                                'If STRGS.InStr(_var0, "ERR:") > 0 Then 'si il y a une erreur on n'ajoute pas le parametre et on logue l'erreur
                                '    _Server.Log(TypeLog.ERREUR, TypeSource.SERVEUR, "ThreadAction Execute ActionIf", STRGS.Right(_var0, _var0.Length - 5))
                                'Else
                                '    If String.IsNullOrEmpty(_var0) Then
                                '        retour2 = x.Conditions.Item(i).Value
                                '    Else
                                '        retour2 = EvaluateCommand(_var0) 'on fait un calcul (par ex <comnom>*10)
                                '    End If
                                'End If


                                '========== Version 2 ========== 
                                'If x.Conditions.Item(i).Value.ToString.StartsWith("<") And x.Conditions.Item(i).Value.ToString.EndsWith(">") Then
                                '    Dim a() As String = x.Conditions.Item(i).Value.ToString.Split("|")
                                '    If a.Length = 3 Then
                                '        Dim _dev As Object = _Server.ReturnRealDeviceById(Mid(a(0), 2, Len(a(0)) - 1))

                                '        If _dev IsNot Nothing Then
                                '            retour2 = CallByName(_dev, Mid(a(2), 1, Len(a(2)) - 1), CallType.Get)
                                '        End If
                                '    Else
                                '        retour2 = x.Conditions.Item(i).Value
                                '    End If
                                'Else
                                '    retour2 = x.Conditions.Item(i).Value
                                'End If

                                '========== Version 3 ========== 
                                'remplacement de <composantnom> <composantnom.meteo> <SYSTEM_XXX>
                                If String.IsNullOrEmpty(x.Conditions.Item(i).Value.ToString) Then
                                    retour2 = ""
                                Else
                                    Dim resultatSTR As String = x.Conditions.Item(i).Value.ToString
                                    Try
                                        Dim startcmd As Integer = InStr(1, resultatSTR, "<")
                                        Dim endcmd As Integer = InStr(1, resultatSTR, ">")
                                        Dim newcmd As String = resultatSTR
                                        If startcmd > 0 Then _Server.Log(Server.TypeLog.DEBUG, Server.TypeSource.SERVEUR, "ThreadAction Execute Replace", "Evaluation de : " & resultatSTR)
                                        Do While startcmd > 0 And endcmd > 0
                                            Dim _device As String = Mid(newcmd, startcmd + 1, endcmd - startcmd - 1)
                                            'Dim Tabl() As String = _device.Split(".")
                                            Dim Tabl() As String = _device.Split(System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                                            If Tabl.Length = 1 Then
                                                Select Case _device
                                                    Case "SYSTEM_DATE"
                                                        _device = Now.Date.ToShortDateString
                                                    Case "SYSTEM_LONG_DATE"
                                                        _device = Now.Date.ToLongDateString
                                                    Case "SYSTEM_TIME"
                                                        _device = Now.ToShortTimeString
                                                    Case "SYSTEM_LONG_TIME"
                                                        _device = Now.ToLongTimeString
                                                    Case "SYSTEM_SOLEIL_COUCHE"
                                                        Dim _date As Date = _Server.GetHeureCoucherSoleil
                                                        _device = _date.ToShortTimeString
                                                    Case "SYSTEM_SOLEIL_LEVE"
                                                        Dim _date As Date = _Server.GetHeureLeverSoleil
                                                        _device = _date.ToShortTimeString
                                                    Case Else
                                                        Dim y As Object = _Server.ReturnRealDeviceByName(Tabl(0))
                                                        If y IsNot Nothing Then
                                                            _device = y.Value
                                                        Else
                                                            _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "ThreadAction Execute Replace", "Composant: " & Tabl(0) & " non trouvé --> Arrêt du traitement")
                                                            Exit Sub
                                                        End If
                                                End Select
                                            ElseIf Tabl.Length = 2 Then
                                                Dim y As Object = _Server.ReturnRealDeviceByName(Tabl(0))
                                                If y IsNot Nothing Then
                                                    Dim value As Object = CallByName(y, Tabl(1), CallType.Get)
                                                    _device = value
                                                End If
                                            End If

                                            Dim start As String = Mid(newcmd, 1, startcmd - 1)
                                            Dim fin As String = Mid(newcmd, endcmd + 1, newcmd.Length - endcmd)
                                            newcmd = start & _device & fin
                                            resultatSTR = newcmd
                                            startcmd = InStr(1, newcmd, "<")
                                            endcmd = InStr(1, newcmd, ">")
                                        Loop
                                    Catch ex As Exception
                                        _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DEVICE, "ThreadAction Execute Replace", "Exception: " & ex.Message)
                                        Exit Sub
                                    End Try
                                    retour2 = resultatSTR
                                End If
                                ' Calcul de {xxxxxxxxxxxxxx}
                                Try
                                    Dim startcmd As Integer = InStr(1, retour2, "{")
                                    Dim endcmd As Integer = InStr(1, retour2, "}")
                                    Dim newcmd As String = retour2
                                    If startcmd > 0 Then _Server.Log(Server.TypeLog.DEBUG, Server.TypeSource.SERVEUR, "ThreadAction Execute Replace", "Calcul de : " & retour2)
                                    Do While startcmd > 0 And endcmd > 0
                                        Dim _calculstring As String = Mid(newcmd, startcmd + 1, endcmd - startcmd - 1)

                                        Try
                                            If Text.RegularExpressions.Regex.IsMatch(_calculstring, "^[0-9+\-*/\^().,]*$") Then
                                                Dim dt = New DataTable()
                                                Dim resultat As Double = CDbl(dt.Compute(_calculstring, ""))
                                                dt = Nothing
                                                _calculstring = resultat
                                            End If
                                        Catch ex As Exception
                                            _calculstring = Mid(newcmd, startcmd + 1, endcmd - startcmd - 1)
                                            _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "ThreadAction Execute Calcul", "Erreur dans le calcul de '" & _calculstring & "' : " & ex.Message)
                                        End Try

                                        Dim start As String = Mid(newcmd, 1, startcmd - 1)
                                        Dim fin As String = Mid(newcmd, endcmd + 1, newcmd.Length - endcmd)
                                        newcmd = start & _calculstring & fin
                                        retour2 = newcmd
                                        startcmd = InStr(1, newcmd, "{")
                                        endcmd = InStr(1, newcmd, "}")
                                    Loop
                                Catch ex As Exception
                                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "ThreadAction Execute Calcul", "Exception: " & ex.Message)
                                    Exit Sub
                                End Try

                                'on teste si les deux valeurs sont du meme type sinon impossible de tester et on retourn False
                                If (retour.GetType = retour2.GetType) Then
                                    Try
                                        Select Case x.Conditions.Item(i).Condition
                                            Case Action.TypeSigne.Egal : If retour = retour2 Then result = True Else result = False
                                            Case Action.TypeSigne.Different : If retour <> retour2 Then result = True Else result = False
                                            Case Action.TypeSigne.Inferieur : If retour < retour2 Then result = True Else result = False
                                            Case Action.TypeSigne.InferieurEgal : If retour <= retour2 Then result = True Else result = False
                                            Case Action.TypeSigne.Superieur : If retour > retour2 Then result = True Else result = False
                                            Case Action.TypeSigne.SuperieurEgal : If retour >= retour2 Then result = True Else result = False
                                        End Select
                                    Catch ex As Exception
                                        _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "ThreadAction Execute Test", "Erreur dans le test de '" & retour & "' <-> '" & retour2 & "'  : " & ex.Message)
                                        result = False
                                    End Try
                                Else
                                    _Server.Log(Server.TypeLog.DEBUG, Server.TypeSource.SERVEUR, "ThreadAction Execute Test", "Type de valeurs different: '" & retour & "' <-> '" & retour2 & "'  : Test annulé --> Faux")
                                    result = False
                                End If
                                
                                If i = 0 Then 'c le 1er donc pas prendre en compte l'operateur
                                    flag = result
                                Else
                                    Select Case x.Conditions.Item(i).Operateur
                                        Case Action.TypeOperateur.AND : flag = flag And result
                                        Case Action.TypeOperateur.OR : flag = flag Or result
                                        Case Action.TypeOperateur.NONE : flag = result
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
                                    If _tim(0) = "*" Then sc = Now.Second Else sc = CInt(_tim(0))
                                    If _tim(1) = "*" Then mn = Now.Minute Else mn = CInt(_tim(1))
                                    If _tim(2) = "*" Then hr = Now.Hour Else hr = CInt(_tim(2))
                                    a1 = New Date(Now.Year, Now.Month, Now.Day, hr, mn, sc)
                                End If
                                'On travail sur une heure + date
                                If (_tim(0) <> "*" Or _tim(1) <> "*" Or _tim(2) <> "*") And (_tim(3) <> "*" Or _tim(4) <> "*") And _tim(5) = "" And _tim(6) <> "1" And _tim(7) <> "1" Then
                                    If _tim(0) = "*" Then sc = Now.Second Else sc = CInt(_tim(0))
                                    If _tim(1) = "*" Then mn = Now.Minute Else mn = CInt(_tim(1))
                                    If _tim(2) = "*" Then hr = Now.Hour Else hr = CInt(_tim(2))
                                    If _tim(3) = "*" Then
                                        dd = Now.Day
                                    Else
                                        dd = CInt(_tim(3))
                                        If dd = 0 Then dd = Now.Day
                                    End If
                                    If _tim(4) = "*" Then
                                        ms = Now.Month
                                    Else
                                        ms = CInt(_tim(4))
                                        If ms = 0 Then ms = Now.Month
                                    End If
                                    a1 = New Date(Now.Year, ms, dd, hr, mn, sc)
                                End If
                                'On travail sur une heure + jour
                                If (_tim(0) <> "*" Or _tim(1) <> "*" Or _tim(2) <> "*") And (_tim(3) = "*" And _tim(4) = "*") And _tim(5) <> "" And _tim(6) <> "1" And _tim(7) <> "1" Then
                                    If _tim(0) = "*" Then sc = Now.Second Else sc = CInt(_tim(0))
                                    If _tim(1) = "*" Then mn = Now.Minute Else mn = CInt(_tim(1))
                                    If _tim(2) = "*" Then hr = Now.Hour Else hr = CInt(_tim(2))
                                    a1 = New Date(Now.Year, Now.Month, Now.Day, hr, mn, sc)
                                    Flagjour = True
                                End If
                                'On travail sur une date
                                If (_tim(0) = "*" And _tim(1) = "*" And _tim(2) = "*") And (_tim(3) <> "*" Or _tim(4) <> "*") And _tim(5) = "" And _tim(6) <> "1" And _tim(7) <> "1" Then
                                    If _tim(3) = "*" Then dd = Now.Day Else dd = CInt(_tim(3))
                                    If _tim(4) = "*" Then ms = Now.Month Else ms = CInt(_tim(4))
                                    a1 = New Date(Now.Year, ms, dd, Now.Hour, Now.Minute, Now.Second)
                                End If
                                'On travail sur un jour
                                If (_tim(0) = "*" And _tim(1) = "*" And _tim(2) = "*") And _tim(3) = "*" And _tim(4) = "*" And _tim(5) <> "" And _tim(6) <> "1" And _tim(7) <> "1" Then
                                    Flagjour = True
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

                                Dim compare As Integer = DateTime.Compare(Now, a1)

                                Select Case x.Conditions.Item(i).Condition
                                    Case Action.TypeSigne.Egal : If compare = 0 Then result = True Else result = False
                                    Case Action.TypeSigne.Different : If compare <> 0 Then result = True Else result = False
                                    Case Action.TypeSigne.Inferieur : If compare < 0 Then result = True Else result = False
                                    Case Action.TypeSigne.InferieurEgal : If compare <= 0 Then result = True Else result = False
                                    Case Action.TypeSigne.Superieur : If compare > 0 Then result = True Else result = False
                                    Case Action.TypeSigne.SuperieurEgal : If compare >= 0 Then result = True Else result = False
                                End Select

                                If Flagjour = True Then
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
                                        Case Action.TypeOperateur.AND : flag = flag And result
                                        Case Action.TypeOperateur.OR : flag = flag Or result
                                        Case Action.TypeOperateur.NONE : flag = result
                                    End Select
                                End If
                            End If
                        Next

                        'La condition est vrai
                        If flag = True Then
                            For i As Integer = 0 To x.ListTrue.Count - 1
                                Dim _action As New ThreadAction(_Server, x.ListTrue.Item(i), _NameMacro, _ID)
                                Dim y As New Thread(AddressOf _Action.Execute)
                                y.Name = _ID
                                y.Start()
                                _Server.GetListThread.Add(y)
                                y = Nothing
                            Next
                        Else
                            For i As Integer = 0 To x.ListFalse.Count - 1
                                Dim _action As New ThreadAction(_Server, x.ListFalse.Item(i), _NameMacro, _ID)
                                Dim y As New Thread(AddressOf _Action.Execute)
                                y.Name = _ID
                                y.Start()
                                _Server.GetListThread.Add(y)
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
                            Dim _action As New Mail(_Server, _Server.GetSMTPMailServeur(_IdSrv), _user.eMail, x.Sujet, x.Message, _Server.GetSMTPServeur(_IdSrv), _Server.GetSMTPPort(_IdSrv), _Server.GetSMTPSSL(_IdSrv), _Server.GetSMTPLogin(_IdSrv), _Server.GetSMTPPassword(_IdSrv))
                            Dim y As New Thread(AddressOf _Action.Send_email)
                            y.Name = "Traitement du script"
                            y.Start()
                            y = Nothing

                        Else
                            _Server.Log(Server.TypeLog.MESSAGE, Server.TypeSource.SCRIPT, "ThreadAction Execute", "Le user Id:" & x.UserId & " n'a pas pu être trouvé, le mail ne pourra pas être donc envoyé")
                        End If

                    Case Action.TypeAction.ActionVB
                        Dim x As Action.ActionVB = _Action
                        ExecuteScript(x.Script)
                    Case Action.TypeAction.ActionStop
                        _Server.Log(TypeLog.DEBUG, TypeSource.SCRIPT, "Arrêt de la macro", "Macro: " & _NameMacro)
                        ' _Server.Log(TypeLog.DEBUG, TypeSource.SCRIPT, "Arrêt de la macro", "Macro: " & _Server.GetListThread.Count)
                        Dim _listthrstop As New List(Of Thread)

                        If _Server.GetListThread.Count > 0 Then

                            For Each thr In _Server.GetListThread
                                If thr.Name.EndsWith(".STOP") Then
                                    If thr.Name.Replace(".STOP", "") = _ID Then _listthrstop.Add(thr)
                                Else
                                    If thr.Name = _ID Then
                                        Try
                                            SyncLock thr
                                                thr.Abort()
                                                thr = Nothing
                                                _Server.GetListThread.Remove(thr)
                                            End SyncLock
                                        Catch ex As Exception
                                            'ça génère une erreur donc on fait rien
                                            Dim b As String = ex.ToString
                                            '_Server.Log(TypeLog.DEBUG, TypeSource.SCRIPT, "Arrêt de la macro", "Erreur: " & ex.ToString)
                                        End Try
                                    End If
                                End If

                            Next

                            If _listthrstop.Count > 0 Then
                                For Each thr In _listthrstop
                                    Try
                                        SyncLock thr
                                            thr.Abort()
                                            thr = Nothing
                                        End SyncLock
                                    Catch ex As Exception
                                        Dim b As String = ex.ToString
                                    End Try

                                Next
                            End If
                            _listthrstop = Nothing
                        End If
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
                    Case Action.TypeAction.ActionVar
                        Dim x As Action.ActionVar = _Action
                        _Server.SetValueOfVariable(_IdSrv, x.Nom, x.Value)
                End Select
            Catch ex As Exception
                If ex.ToString.Contains("ThreadAbortException") = False Then _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "ThreadAction Execute", "Exception: " & ex.ToString)
            End Try
        End Sub

        Public Sub New(ByVal Server As Server, ByVal Action As Object, NameMacro As String, ID As String)
            Try
                _Server = Server
                _Action = Action
                _NameMacro = NameMacro
                _ID = ID
            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "New Execute", "Exception: " & ex.ToString)
            End Try
        End Sub

        Public Sub Sendhttp(ByVal Commande As String)
            Try
                Dim texte As String = Commande
                'remplace les balises par la valeur
                texte = DecodeCommand(texte)
                'texte = texte.Replace("{time}", Now.ToShortTimeString) '--> pb avec le format qui ne passe pas dans une URL
                'texte = texte.Replace("{date}", Now.ToLongDateString) '--> pb avec le format qui ne passe pas dans une URL

                Dim reader As StreamReader = Nothing
                Dim str As String = ""
                Dim request As WebRequest = WebRequest.Create(texte)
                Dim response As WebResponse = request.GetResponse()
                reader = New StreamReader(response.GetResponseStream())
                str = reader.ReadToEnd
                reader.Close()
            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SCRIPT, "Sendhttp", "Exception lors de l'envoi de la commande http: " & ex.Message)
            End Try
        End Sub

        Private Sub Parler(ByVal Message As String)
            Try
                Dim texte As String = Message
                'remplace les balises par la valeur
                texte = texte.Replace("{time}", Now.ToShortTimeString)
                texte = texte.Replace("{date}", Now.ToLongDateString)
                texte = DecodeCommand(texte)

                Dim lamachineaparler As New Speech.Synthesis.SpeechSynthesizer
                _Server.Log(Server.TypeLog.DEBUG, Server.TypeSource.SCRIPT, "Parler", "Message: " & texte)
                With lamachineaparler
                    .SelectVoice(_Server.GetDefautVoice)
                    '.SetOutputToWaveFile("C:\tet.wav")
                    '.SetOutputToWaveFile(File)
                    .SpeakAsync(texte)
                End With

                texte = Nothing
                lamachineaparler = Nothing
            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SCRIPT, "Parler", "Exception lors de l'annonce du message: " & Message & " : " & ex.ToString)
            End Try
        End Sub

        Private Sub ExecuteScript(ByVal ScriptSource As String)
            Try
                ' Instantiate the VB compiler.
                'Dim objCodeCompiler As ICodeCompiler = New VBCodeProvider().CreateCompiler
                Dim objCodeCompiler As VBCodeProvider = New VBCodeProvider()

                ' Pass parameters into the compiler.
                Dim objCompilerParameters As New CompilerParameters
                objCompilerParameters.ReferencedAssemblies.Add("System.dll")
                objCompilerParameters.ReferencedAssemblies.Add("System.xml.dll")
                objCompilerParameters.ReferencedAssemblies.Add("System.xml.linq.dll")
                objCompilerParameters.ReferencedAssemblies.Add("System.net.dll")
                objCompilerParameters.ReferencedAssemblies.Add("System.Windows.Forms.dll")
                objCompilerParameters.ReferencedAssemblies.Add("Microsoft.VisualBasic.dll")
                objCompilerParameters.ReferencedAssemblies.Add(My.Application.Info.DirectoryPath + "\Homidom.dll")
                objCompilerParameters.GenerateInMemory = True

                ' Get te source code and compile it.
                Dim strCode As String = ScriptSource
                Dim objCompileResults As CompilerResults = objCodeCompiler.CompileAssemblyFromSource(objCompilerParameters, strCode)

                ' Check for compiler errors.
                If objCompileResults.Errors.HasErrors Then
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SCRIPT, "ExecuteScript", "Erreur lors de l'execution du script: Line>" & objCompileResults.Errors(0).Line.ToString & ", " & objCompileResults.Errors(0).ErrorText)
                    Exit Sub
                End If

                ' Get a reference to the assembly.
                Dim objAssembly As System.Reflection.Assembly = objCompileResults.CompiledAssembly

                ' Create an instance of the DynamicCode class referenced in the source code.
                Dim objTheClass As Object = objAssembly.CreateInstance("Dynam.DynamicCode")

                If objTheClass Is Nothing Then
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SCRIPT, "ExecuteScript", "Unable to load class.")
                    Exit Sub
                End If

                ' Create a parameter to be passed into the ExecuteCode function in class DynamicCode.
                Dim objFunctionParameters(0) As Object
                objFunctionParameters(0) = _Server

                ' Call the DynamicCode.ExecuteCode
                Dim objResult As Object = objTheClass.GetType.InvokeMember("ExecuteCode", _
                                BindingFlags.InvokeMethod, Nothing, objTheClass, objFunctionParameters)

            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SCRIPT, "ExecuteScript", "Exception: " & ex.ToString)
            End Try
        End Sub

        Public Function DecodeCommand(ByVal Command As String) As String
            Dim retour As String = Command
            Try
                _Server.Log(Server.TypeLog.DEBUG, Server.TypeSource.DEVICE, "DecodeCommand", "Evaluation de : " & Command)

                Dim startcmd As Integer = InStr(1, Command, "<")
                Dim endcmd As Integer = InStr(1, Command, ">")
                Dim newcmd As String = Command

                Do While startcmd > 0 And endcmd > 0
                    Dim _device As String = Mid(newcmd, startcmd + 1, endcmd - startcmd - 1)
                    'Dim Tabl() As String = _device.Split(".")
                    Dim Tabl() As String = _device.Split(System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)

                    If Tabl.Length = 1 Then
                        Select Case _device
                            Case "SYSTEM_DATE"
                                _device = Now.Date.ToShortDateString
                            Case "SYSTEM_LONG_DATE"
                                _device = Now.Date.ToLongDateString
                            Case "SYSTEM_TIME"
                                _device = Now.ToShortTimeString
                            Case "SYSTEM_LONG_TIME"
                                _device = Now.ToLongTimeString
                            Case "SYSTEM_SOLEIL_COUCHE"
                                Dim _date As Date = _Server.GetHeureCoucherSoleil
                                _device = _date.ToShortTimeString
                            Case "SYSTEM_SOLEIL_LEVE"
                                Dim _date As Date = _Server.GetHeureLeverSoleil
                                _device = _date.ToShortTimeString
                            Case Else
                                Dim x As Object = _Server.ReturnRealDeviceByName(Tabl(0))
                                If x IsNot Nothing Then
                                    _device = x.Value
                                Else
                                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SCRIPT, "DecodeCommand", "Composant: " & Tabl(0) & " non trouvé --> Arrêt du traitement")
                                    retour = "ERR: DecodeCommand Composant: " & Tabl(0) & " non trouvé --> Arrêt du traitement"
                                    Return retour
                                End If
                        End Select

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
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SCRIPT, "DecodeCommand", "Exception: " & ex.Message)
                retour = "ERR: " & ex.ToString
                Return retour
            End Try
        End Function

        Public Function EvaluateCommand(ByVal Command As String) As String
            'si la chaine ne contient que des chiffres et operateurs "+-*/^().," on calcule sinon on affecte
            Try
                If Text.RegularExpressions.Regex.IsMatch(Command, "^[0-9+\-*/\^().,]*$") Then
                    Dim dt = New DataTable()
                    Dim resultat As Double = CDbl(dt.Compute(Command, ""))
                    dt = Nothing
                    Return resultat
                Else
                    Return Command
                End If
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.SERVEUR, "EvaluateCommand", "Exception: " & ex.ToString)
                Return Command
            End Try
        End Function
    End Class
End Namespace