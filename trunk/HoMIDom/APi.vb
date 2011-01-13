Namespace HoMIDom

    Public Module Api

        Public Function GenerateGUID() As String
            Dim sGUID As String
            sGUID = System.Guid.NewGuid.ToString()
            GenerateGUID = sGUID
        End Function

    End Module

End Namespace
