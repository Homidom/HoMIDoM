Namespace HoMIDom

    Public Module Api

        Public Function GenerateGUID() As String
            Dim sGUID As String
            sGUID = System.Guid.NewGuid.ToString()
            GenerateGUID = sGUID
        End Function

        'Liste les propriétés public d'un objet
        'Nom_de_la_propriété|Type_retourné
        Public Function ListProperty(ByVal OBjet As Object) As ArrayList
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
        End Function
    End Module

End Namespace
