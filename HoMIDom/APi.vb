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

       

    End Module

End Namespace
