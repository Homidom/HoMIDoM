Module ManagerZones
    Public Sub LoadZones()
        Try
            _ListeZones = myService.GetAllZones(IdSrv)
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur LoadZones: " & ex.ToString, "Erreur", " Event DeviceChanged")
        End Try
    End Sub
End Module
