Module ManagerDevices
    Public Sub LoadDevices()
        Try
            _ListeDevices = myService.GetAllDevices(IdSrv)
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur LoadDevices: " & ex.ToString, "Erreur", " Event DeviceChanged")
        End Try
    End Sub

    Public Function ReturnDeviceByID(ID As String) As HoMIDom.HoMIDom.TemplateDevice
        Try
            Dim x As New HoMIDom.HoMIDom.TemplateDevice

            For Each _dev In _ListeDevices
                If _dev.ID = ID Then x = _dev
            Next

            Return x
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur ReturnDeviceByID: " & ex.ToString, "Erreur", " Event DeviceChanged")
            Return Nothing
        End Try
    End Function

    Public Sub EnableDevice(ID As String)
        Try
            For Each _dev In _ListeDevices
                If _dev.ID = ID Then _dev.Enable = True
            Next
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur EnableDevice: " & ex.ToString, "Erreur", " Event DeviceChanged")
        End Try
    End Sub

    Public Sub DisableDevice(ID As String)
        Try
            For Each _dev In _ListeDevices
                If _dev.ID = ID Then _dev.Enable = False
            Next
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur DisableDevice: " & ex.ToString, "Erreur", " Event DeviceChanged")
        End Try
    End Sub

    Public Sub DeleteDevice(ID As String)
        Try
            _ListeDevices = myService.GetAllDevices(IdSrv)
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur DisableDevice: " & ex.ToString, "Erreur", " Event DeviceChanged")
        End Try
    End Sub
End Module
