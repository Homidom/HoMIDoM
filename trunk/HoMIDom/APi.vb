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
            GenerateGUID = sGUID
        End Function

        'Liste les propriétés public d'un objet
        'Nom_de_la_propriété|Type_retourné
        Public Function ListProperty(ByVal OBjet As Object) As ArrayList
            Try
                Dim X As String
                Dim Info As Reflection.PropertyInfo
                Dim _Tabl As New ArrayList

                For Each Info In OBjet.GetType.GetProperties
                    If Info.Name.ToString <> "Server" And Info.CanWrite = True Then
                        X = (Info.Name.ToString) 'retourne le type string, boolean
                        X &= "|" & Info.PropertyType.FullName.Replace("System.", "") & "|" & CallByName(OBjet, Info.Name.ToString, CallType.Get, Nothing)
                        _Tabl.Add(X)
                    End If
                Next
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


        'Liste les méthodes public d'un objet
        'Nom_de_la_methode|parametre1:Type|parametre2:type...
        Public Function ListMethod(ByVal Objet As Object) As ArrayList
            Try
                Dim X As String
                Dim Info As Reflection.MethodInfo
                Dim paraminfos() As Object
                Dim _Tabl As New ArrayList

                For Each Info In Objet.GetType.GetMethods()
                    X = (Info.ReturnType.ToString) 'retourne le type string, boolean
                    If Info.Attributes = 6 And X = "System.Void" Then 'on prend que les méthodes public
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
                Return _Tabl
            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "API ListProperty", "Exception : " & ex.ToString)
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Retourne le résultat d'un calcul (formule, ex: *2 +3) en se basant sur une valeur de base (Valeur)
        ''' </summary>
        ''' <param name="Formule"></param>
        ''' <param name="Valeur"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Evaluation(ByRef Formule As String, ByVal Valeur As Double) As Double
            Try
                Dim PosSep As Integer
                Dim PosCour As Integer
                Dim PosAV As Integer
                Dim PosAP As Integer
                Dim Av, Ap As String
                Dim Res As Double
                Dim Separateurs(3) As String
                Dim NumSep As Integer

                'liste des separateurs
                Formule = Valeur & Formule
                Formule = Replace(Formule, " ", "")

                Separateurs(0) = "/"
                Separateurs(1) = "*"
                Separateurs(2) = "+"
                Separateurs(3) = "-"

                For NumSep = 0 To 3

                    PosSep = InStr(1, Formule, Separateurs(NumSep))
                    While PosSep > 0
                        'on determine le nombre AVANT le separateur
                        PosCour = PosSep - 1
                        While (IsNumeric(Mid(Formule, PosCour, 1)))
                            PosCour = PosCour - 1
                            If PosCour = 0 Then Exit While
                        End While

                        PosAV = PosCour + 1
                        Av = Mid(Formule, PosAV, PosSep - PosAV)
                        'on determine le nombre APRES le separateur
                        PosCour = PosSep + 1
                        While IsNumeric(Mid(Formule, PosCour, 1))
                            PosCour = PosCour + 1
                        End While
                        PosAP = PosCour
                        Ap = Mid(Formule, PosSep + 1, PosAP - PosSep - 1)

                        'On calcule la sous-partie isolée
                        Select Case NumSep
                            Case 0 '/
                                Res = Val(Av) / Val(Ap)
                            Case 1 '*
                                Res = Val(Av) * Val(Ap)
                            Case 2 '+
                                Res = Val(Av) + Val(Ap)
                            Case 3 '-
                                Res = Val(Av) - Val(Ap)
                        End Select

                        'on réécrit la formule avec la sous-partie calculée
                        Formule = Left(Formule, PosAV - 1) & Trim(Str(Res)) & Mid(Formule, PosAP)
                        PosSep = InStr(1, Formule, Separateurs(NumSep))
                    End While
                Next
                Evaluation = Val(Formule)
            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "API Evaluation", "Erreur : " & ex.ToString)
                Return Valeur
            End Try
        End Function

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

    End Module

End Namespace
