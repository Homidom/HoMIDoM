Module ManagerDevices
    Public Sub LoadDevices()
        Try
            _ListeDevices = myService.GetAllDevices(IdSrv)
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur LoadDevices: " & ex.ToString, "Erreur", " Event DeviceChanged")
        End Try
    End Sub

    'Public Sub LoadHisto()
    '    _DevicesAsHisto = myService.DevicesAsHisto
    'End Sub
End Module
