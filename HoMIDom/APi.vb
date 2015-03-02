Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Windows.Media.Imaging

Namespace HoMIDom

    Public Module Api

        Dim _Server As Server

        ''' <summary>
        ''' Génère un ID unique
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GenerateGUID() As String
            Dim sGUID As String = ""
            sGUID = System.Guid.NewGuid.ToString()
            Return sGUID
        End Function

        'Liste les propriétés public d'un objet
        'Nom_de_la_propriété|Type_retourné
        Public Function ListProperty(ByVal OBjet As Object) As ArrayList
            Try
                Dim X As String = ""
                Dim Info As Reflection.PropertyInfo
                Dim _Tabl As New ArrayList

                For Each Info In OBjet.GetType.GetProperties
                    If Info.Name.ToString <> "Server" And Info.CanWrite = True Then
                        X = (Info.Name.ToString) 'retourne le type string, boolean
                        X &= "|" & Info.PropertyType.FullName.Replace("System.", "") & "|" & CallByName(OBjet, Info.Name.ToString, CallType.Get, Nothing)
                        _Tabl.Add(X)
                        X = ""
                    End If
                Next

                Info = Nothing
                Return _Tabl
            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "API ListProperty", "Exception : " & ex.ToString)
                Return Nothing
            End Try
        End Function

        Public Function AsProperty(ByVal OBjet As Object, ByVal [Property] As String) As Boolean
            Try
                Dim Info As Reflection.PropertyInfo
                Dim _Myobjet As Object = Nothing
                Dim _result As Boolean = False

                Select Case OBjet.Type.ToString
                    Case HoMIDom.Device.ListeDevices.APPAREIL.ToString
                        _Myobjet = New Device.APPAREIL (Nothing)
                    Case HoMIDom.Device.ListeDevices.AUDIO.ToString
                        _Myobjet = New Device.AUDIO(Nothing)
                    Case HoMIDom.Device.ListeDevices.BAROMETRE.ToString
                        _Myobjet = New Device.BAROMETRE(Nothing)
                    Case HoMIDom.Device.ListeDevices.BATTERIE.ToString
                        _Myobjet = New Device.BATTERIE(Nothing)
                    Case HoMIDom.Device.ListeDevices.COMPTEUR.ToString
                        _Myobjet = New Device.COMPTEUR(Nothing)
                    Case HoMIDom.Device.ListeDevices.CONTACT.ToString
                        _Myobjet = New Device.CONTACT(Nothing)
                    Case HoMIDom.Device.ListeDevices.DETECTEUR.ToString
                        _Myobjet = New Device.DETECTEUR(Nothing)
                    Case HoMIDom.Device.ListeDevices.DIRECTIONVENT.ToString
                        _Myobjet = New Device.DIRECTIONVENT(Nothing)
                    Case HoMIDom.Device.ListeDevices.ENERGIEINSTANTANEE.ToString
                        _Myobjet = New Device.ENERGIEINSTANTANEE(Nothing)
                    Case HoMIDom.Device.ListeDevices.ENERGIETOTALE.ToString
                        _Myobjet = New Device.ENERGIETOTALE(Nothing)
                    Case HoMIDom.Device.ListeDevices.FREEBOX.ToString
                        _Myobjet = New Device.FREEBOX(Nothing)
                    Case HoMIDom.Device.ListeDevices.GENERIQUEBOOLEEN.ToString
                        _Myobjet = New Device.GENERIQUEBOOLEEN(Nothing)
                    Case HoMIDom.Device.ListeDevices.GENERIQUESTRING.ToString
                        _Myobjet = New Device.GENERIQUESTRING(Nothing)
                    Case HoMIDom.Device.ListeDevices.GENERIQUEVALUE.ToString
                        _Myobjet = New Device.GENERIQUEVALUE(Nothing)
                    Case HoMIDom.Device.ListeDevices.HUMIDITE.ToString
                        _Myobjet = New Device.HUMIDITE(Nothing)
                    Case HoMIDom.Device.ListeDevices.LAMPE.ToString
                        _Myobjet = New Device.LAMPE(Nothing)
                    Case HoMIDom.Device.ListeDevices.METEO.ToString
                        _Myobjet = New Device.METEO(Nothing)
                    Case HoMIDom.Device.ListeDevices.MULTIMEDIA.ToString
                        _Myobjet = New Device.MULTIMEDIA(Nothing)
                    Case HoMIDom.Device.ListeDevices.PLUIECOURANT.ToString
                        _Myobjet = New Device.PLUIECOURANT(Nothing)
                    Case HoMIDom.Device.ListeDevices.PLUIETOTAL.ToString
                        _Myobjet = New Device.PLUIETOTAL(Nothing)
                    Case HoMIDom.Device.ListeDevices.SWITCH.ToString
                        _Myobjet = New Device.SWITCH(Nothing)
                    Case HoMIDom.Device.ListeDevices.TELECOMMANDE.ToString
                        _Myobjet = New Device.TELECOMMANDE(Nothing)
                    Case HoMIDom.Device.ListeDevices.TEMPERATURE.ToString
                        _Myobjet = New Device.TEMPERATURE(Nothing)
                    Case HoMIDom.Device.ListeDevices.TEMPERATURECONSIGNE.ToString
                        _Myobjet = New Device.TEMPERATURECONSIGNE(Nothing)
                    Case HoMIDom.Device.ListeDevices.UV.ToString
                        _Myobjet = New Device.UV(Nothing)
                    Case HoMIDom.Device.ListeDevices.VITESSEVENT.ToString
                        _Myobjet = New Device.VITESSEVENT(Nothing)
                    Case HoMIDom.Device.ListeDevices.VOLET.ToString
                        _Myobjet = New Device.VOLET(Nothing)
                End Select

                For Each Info In _Myobjet.GetType.GetProperties
                    If LCase(Info.Name.ToString) = LCase([Property]) Then
                        _result = True
                        Exit For
                    End If
                Next

                Return _result
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Function TypeOfProperty(ByVal OBjet As Object, ByVal [Property] As String) As String
            Try
                Dim Info As Reflection.PropertyInfo
                Dim _Myobjet As Object = Nothing
                Dim _result As String = ""

                Select Case OBjet.Type.ToString
                    Case HoMIDom.Device.ListeDevices.APPAREIL.ToString
                        _Myobjet = New Device.APPAREIL(Nothing)
                    Case HoMIDom.Device.ListeDevices.AUDIO.ToString
                        _Myobjet = New Device.AUDIO(Nothing)
                    Case HoMIDom.Device.ListeDevices.BAROMETRE.ToString
                        _Myobjet = New Device.BAROMETRE(Nothing)
                    Case HoMIDom.Device.ListeDevices.BATTERIE.ToString
                        _Myobjet = New Device.BATTERIE(Nothing)
                    Case HoMIDom.Device.ListeDevices.COMPTEUR.ToString
                        _Myobjet = New Device.COMPTEUR(Nothing)
                    Case HoMIDom.Device.ListeDevices.CONTACT.ToString
                        _Myobjet = New Device.CONTACT(Nothing)
                    Case HoMIDom.Device.ListeDevices.DETECTEUR.ToString
                        _Myobjet = New Device.DETECTEUR(Nothing)
                    Case HoMIDom.Device.ListeDevices.DIRECTIONVENT.ToString
                        _Myobjet = New Device.DIRECTIONVENT(Nothing)
                    Case HoMIDom.Device.ListeDevices.ENERGIEINSTANTANEE.ToString
                        _Myobjet = New Device.ENERGIEINSTANTANEE(Nothing)
                    Case HoMIDom.Device.ListeDevices.ENERGIETOTALE.ToString
                        _Myobjet = New Device.ENERGIETOTALE(Nothing)
                    Case HoMIDom.Device.ListeDevices.FREEBOX.ToString
                        _Myobjet = New Device.FREEBOX(Nothing)
                    Case HoMIDom.Device.ListeDevices.GENERIQUEBOOLEEN.ToString
                        _Myobjet = New Device.GENERIQUEBOOLEEN(Nothing)
                    Case HoMIDom.Device.ListeDevices.GENERIQUESTRING.ToString
                        _Myobjet = New Device.GENERIQUESTRING(Nothing)
                    Case HoMIDom.Device.ListeDevices.GENERIQUEVALUE.ToString
                        _Myobjet = New Device.GENERIQUEVALUE(Nothing)
                    Case HoMIDom.Device.ListeDevices.HUMIDITE.ToString
                        _Myobjet = New Device.HUMIDITE(Nothing)
                    Case HoMIDom.Device.ListeDevices.LAMPE.ToString
                        _Myobjet = New Device.LAMPE(Nothing)
                    Case HoMIDom.Device.ListeDevices.METEO.ToString
                        _Myobjet = New Device.METEO(Nothing)
                    Case HoMIDom.Device.ListeDevices.MULTIMEDIA.ToString
                        _Myobjet = New Device.MULTIMEDIA(Nothing)
                    Case HoMIDom.Device.ListeDevices.PLUIECOURANT.ToString
                        _Myobjet = New Device.PLUIECOURANT(Nothing)
                    Case HoMIDom.Device.ListeDevices.PLUIETOTAL.ToString
                        _Myobjet = New Device.PLUIETOTAL(Nothing)
                    Case HoMIDom.Device.ListeDevices.SWITCH.ToString
                        _Myobjet = New Device.SWITCH(Nothing)
                    Case HoMIDom.Device.ListeDevices.TELECOMMANDE.ToString
                        _Myobjet = New Device.TELECOMMANDE(Nothing)
                    Case HoMIDom.Device.ListeDevices.TEMPERATURE.ToString
                        _Myobjet = New Device.TEMPERATURE(Nothing)
                    Case HoMIDom.Device.ListeDevices.TEMPERATURECONSIGNE.ToString
                        _Myobjet = New Device.TEMPERATURECONSIGNE(Nothing)
                    Case HoMIDom.Device.ListeDevices.UV.ToString
                        _Myobjet = New Device.UV(Nothing)
                    Case HoMIDom.Device.ListeDevices.VITESSEVENT.ToString
                        _Myobjet = New Device.VITESSEVENT(Nothing)
                    Case HoMIDom.Device.ListeDevices.VOLET.ToString
                        _Myobjet = New Device.VOLET(Nothing)
                End Select

                For Each Info In _Myobjet.GetType.GetProperties
                    If LCase(Info.Name.ToString) = LCase([Property]) Then
                        _result = Info.PropertyType.FullName
                        _result = LCase(_result)
                        _result = _result.Replace("system.", "")
                        Exit For
                    End If
                Next

                Return _result
            Catch ex As Exception
                Return ""
            End Try
        End Function

        'Liste les méthodes public d'un objet
        'Nom_de_la_methode|parametre1:Type|parametre2:type...
        Public Function ListMethod(ByVal Objet As Object) As ArrayList
            Try
                Dim X As String = ""
                Dim Info As Reflection.MethodInfo
                Dim paraminfos() As Object = Nothing
                Dim _Tabl As New ArrayList

                For Each Info In Objet.GetType.GetMethods()
                    X = (Info.ReturnType.ToString) 'retourne le type string, boolean

                    If (Info.Attributes = 6 Or Info.Attributes = 838) And X = "System.Void" Then 'on prend que les méthodes public
                        X = Info.Name 'Nom de la méthode
                        paraminfos = Info.GetParameters()

                        Dim tabl() As String
                        For i As Integer = 0 To paraminfos.Count - 1
                            tabl = paraminfos(i).ToString.Split(" ")
                            X &= "|" & tabl(1) & ":" & tabl(0)
                        Next

                        _Tabl.Add(X)
                    End If
                Next

                paraminfos = Nothing
                Return _Tabl
            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "API ListProperty", "Exception : " & ex.ToString)
                Return Nothing
            End Try
        End Function

        ' ''' <summary>
        ' ''' Retourne le résultat d'un calcul (formule, ex: *2 +3) en se basant sur une valeur de base (Valeur)
        ' ''' </summary>
        ' ''' <param name="Formule"></param>
        ' ''' <param name="Valeur"></param>
        ' ''' <returns></returns>
        ' ''' <remarks></remarks>
        'Public Function EvaluationDevice(ByVal Formule As String, ByVal Valeur As Double) As Double
        '    'Dim PosSep As Integer
        '    'Dim PosCour As Integer
        '    'Dim PosAV As Integer
        '    'Dim PosAP As Integer
        '    'Dim Av, Ap As String
        '    'Dim Res As Double
        '    'Dim Separateurs(3) As String
        '    'Dim NumSep As Integer

        '    ''liste des separateurs
        '    'Formule = Valeur & Formule
        '    'Formule = Replace(Formule, " ", "")

        '    'Separateurs(0) = "/"
        '    'Separateurs(1) = "*"
        '    'Separateurs(2) = "+"
        '    'Separateurs(3) = "-"

        '    'For NumSep = 0 To 3

        '    '    PosSep = InStr(1, Formule, Separateurs(NumSep))
        '    '    While PosSep > 0
        '    '        'on determine le nombre AVANT le separateur
        '    '        PosCour = PosSep - 1
        '    '        While (IsNumeric(Mid(Formule, PosCour, 1)))
        '    '            PosCour = PosCour - 1
        '    '            If PosCour = 0 Then Exit While
        '    '        End While

        '    '        PosAV = PosCour + 1
        '    '        Av = Mid(Formule, PosAV, PosSep - PosAV)
        '    '        'on determine le nombre APRES le separateur
        '    '        PosCour = PosSep + 1
        '    '        While IsNumeric(Mid(Formule, PosCour, 1))
        '    '            PosCour = PosCour + 1
        '    '        End While
        '    '        PosAP = PosCour
        '    '        Ap = Mid(Formule, PosSep + 1, PosAP - PosSep - 1)

        '    '        'On calcule la sous-partie isolée
        '    '        Select Case NumSep
        '    '            Case 0 '/
        '    '                Res = Val(Av) / Val(Ap)
        '    '            Case 1 '*
        '    '                Res = Val(Av) * Val(Ap)
        '    '            Case 2 '+
        '    '                Res = Val(Av) + Val(Ap)
        '    '            Case 3 '-
        '    '                Res = Val(Av) - Val(Ap)
        '    '        End Select

        '    '        'on réécrit la formule avec la sous-partie calculée
        '    '        Formule = Left(Formule, PosAV - 1) & Trim(Str(Res)) & Mid(Formule, PosAP)
        '    '        PosSep = InStr(1, Formule, Separateurs(NumSep))
        '    '    End While
        '    'Next
        '    'Evaluation = Val(Formule)

        '    _Server.Log(Server.TypeLog.DEBUG, Server.TypeSource.SERVEUR, "API Evaluation", "test")

        '    _Server.Log(Server.TypeLog.DEBUG, Server.TypeSource.SERVEUR, "API Evaluation", "Evaluation de : " & Valeur & " " & Formule)

        '    Dim resultatSTR As String = Valeur & Formule
        '    Try
        '        Dim startcmd As Integer = InStr(1, resultatSTR, "<")
        '        Dim endcmd As Integer = InStr(1, resultatSTR, ">")
        '        Dim newcmd As String = resultatSTR

        '        Do While startcmd > 0 And endcmd > 0
        '            Dim _device As String = Mid(newcmd, startcmd + 1, endcmd - startcmd - 1)

        '            _Server.Log(Server.TypeLog.DEBUG, Server.TypeSource.SERVEUR, "API Evaluation Conversion", "Remplacement de " & _device & " par sa valeur")

        '            'Dim Tabl() As String = _device.Split(".")
        '            Dim Tabl() As String = _device.Split(System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
        '            If Tabl.Length = 1 Then
        '                Select Case _device
        '                    Case "SYSTEM_DATE"
        '                        _device = Now.Date.ToShortDateString
        '                    Case "SYSTEM_LONG_DATE"
        '                        _device = Now.Date.ToLongDateString
        '                    Case "SYSTEM_TIME"
        '                        _device = Now.ToShortTimeString
        '                    Case "SYSTEM_LONG_TIME"
        '                        _device = Now.ToLongTimeString
        '                    Case "SYSTEM_SOLEIL_COUCHE"
        '                        Dim _date As Date = _Server.GetHeureCoucherSoleil
        '                        _device = _date.ToShortTimeString
        '                    Case "SYSTEM_SOLEIL_LEVE"
        '                        Dim _date As Date = _Server.GetHeureLeverSoleil
        '                        _device = _date.ToShortTimeString
        '                    Case Else
        '                        Dim x As Object = _Server.ReturnRealDeviceByName(Tabl(0))
        '                        If x IsNot Nothing Then
        '                            _device = x.Value
        '                        Else
        '                            _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "API Evaluation", "Composant: " & Tabl(0) & " non trouvé --> Arrêt du traitement")
        '                            Return -1
        '                        End If
        '                End Select
        '            ElseIf Tabl.Length = 2 Then
        '                Dim x As Object = _Server.ReturnRealDeviceByName(Tabl(0))
        '                If x IsNot Nothing Then
        '                    Dim value As Object = CallByName(x, Tabl(1), CallType.Get)
        '                    _device = value
        '                End If
        '            End If

        '            Dim start As String = Mid(newcmd, 1, startcmd - 1)
        '            Dim fin As String = Mid(newcmd, endcmd + 1, newcmd.Length - endcmd)
        '            newcmd = start & _device & fin
        '            resultatSTR = newcmd
        '            startcmd = InStr(1, newcmd, "<")
        '            endcmd = InStr(1, newcmd, ">")
        '        Loop

        '    Catch ex As Exception
        '        _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "API Evaluation Conversion", "Exception: " & ex.Message)
        '        Return -1
        '    End Try


        '    'On fait le calcul
        '    Try

        '        _Server.Log(Server.TypeLog.DEBUG, Server.TypeSource.SERVEUR, "API Evaluation CALCUL", "Calcul de " & resultatSTR)


        '        If Text.RegularExpressions.Regex.IsMatch(resultatSTR, "^[0-9+\-*/\^().,]*$") Then
        '            Dim dt = New DataTable()
        '            Dim resultat As Double = CDbl(dt.Compute(resultatSTR, ""))
        '            dt = Nothing
        '            Return resultat
        '        Else
        '            Return resultatSTR
        '        End If
        '    Catch ex As Exception
        '        _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "API Evaluation Calcul", "Erreur : " & ex.ToString)
        '        Return Valeur
        '    End Try
        'End Function

        Public Function ConvertArrayToImage(ByVal value As Object) As Object
            Try
                Dim ImgSource As BitmapImage = Nothing
                Dim array As Byte() = TryCast(value, Byte())

                If array IsNot Nothing Then
                    ImgSource = New BitmapImage()
                    ImgSource.BeginInit()
                    ImgSource.CacheOption = BitmapCacheOption.OnLoad
                    ImgSource.CreateOptions = BitmapCreateOptions.DelayCreation
                    ImgSource.StreamSource = New MemoryStream(array)
                    array = Nothing
                    ImgSource.EndInit()
                    If ImgSource.CanFreeze Then ImgSource.Freeze()
                End If

                Return ImgSource
                ImgSource = Nothing
            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "API ConvertArrayToImage", "Erreur : " & ex.ToString)
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Retourne True si la chaine comporte un caractère spécial
        ''' </summary>
        ''' <param name="txt"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function HaveCaractSpecial(ByVal txt As String) As Boolean
            Dim illegalChars As Char() = "!@#$%^&*(){}[]""_+<>?/-".ToCharArray()
            Dim str As String = txt
            Dim retour As Boolean = False

            For Each ch As Char In str
                If Not Array.IndexOf(illegalChars, ch) = -1 Then
                    retour = True
                End If
            Next

            Return retour
        End Function
    End Module

End Namespace
