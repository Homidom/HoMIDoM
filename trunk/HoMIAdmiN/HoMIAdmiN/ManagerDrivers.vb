Imports HoMIDom.HoMIDom

Module ManagerDrivers
    Public Sub LoadDrivers()
        Try
            _ListeDrivers = myService.GetAllDrivers(IdSrv)
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur LoadDrivers: " & ex.ToString, "Erreur", "LoadDrivers")
        End Try
    End Sub

    Public Function ReturnDriverByID(ID As String) As HoMIDom.HoMIDom.TemplateDriver
        Try
            Dim x As New HoMIDom.HoMIDom.TemplateDriver

            For Each _dev In _ListeDrivers
                If _dev.ID = ID Then x = _dev
            Next

            Return x
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur ReturnDriverByID: " & ex.ToString, "Erreur", " Event DeviceChanged")
            Return Nothing
        End Try
    End Function

    Public Sub EnableDriver(ID As String)
        Try
            For Each _dev In _ListeDrivers
                If _dev.ID = ID Then _dev.Enable = True
            Next
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur EnableDriver: " & ex.ToString, "Erreur", " Event DeviceChanged")
        End Try
    End Sub

    Public Sub DisableDriver(ID As String)
        Try
            For Each _dev In _ListeDrivers
                If _dev.ID = ID Then _dev.Enable = False
            Next
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur DisableDriver: " & ex.ToString, "Erreur", " Event DeviceChanged")
        End Try
    End Sub
End Module
