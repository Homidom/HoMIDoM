Imports HoMIDom.HoMIDom

Module ManagerDrivers
    Public Sub LoadDrivers()
        Try
            _ListeDrivers = myService.GetAllDrivers(IdSrv)
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur LoadDrivers: " & ex.ToString, "Erreur", "LoadDrivers")
        End Try
    End Sub

    ''' <summary>Retourne un driver par son ID</summary>
    ''' <param name="DriverId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ReturnDriverById(ByVal DriverId As String) As TemplateDriver
        Try
            Dim retour As New TemplateDriver

            For i As Integer = 0 To _ListeDrivers.Count - 1
                If _ListeDrivers.Item(i).ID = DriverId Then
                    Return _ListeDrivers.Item(i)
                End If
            Next
            Return retour
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur ReturnDriverById: " & ex.ToString, "Erreur", "ReturnDriverById")
            Return Nothing
        End Try
    End Function

End Module
