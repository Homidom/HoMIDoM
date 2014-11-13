Imports System.Threading

Module ManagerSequences
    Public Function IsNewSequenceDriver(IDSequence As String) As Boolean
        Try
            If IsConnect Then
                If IDSequence = myService.GetSequenceDriver(IdSrv) Then
                    Return True
                Else
                    Return False
                End If
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur IsNewSequenceDriver: " & ex.ToString, "Erreur", "IsNewSequenceDriver")
            Return False
        End Try
    End Function

    Public Function IsNewSequenceDevice(IDSequence As String) As Boolean
        Try
            If IsConnect Then
                If IDSequence = myService.GetSequenceDevice(IdSrv) Then
                    Return True
                Else
                    Return False
                End If
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur IsNewSequenceDevice: " & ex.ToString, "Erreur", "IsNewSequenceDevice")
            Return False
        End Try
    End Function

    Public Function IsNewSequenceTrigger(IDSequence As String) As Boolean
        Try
            If IsConnect Then
                If IDSequence = myService.GetSequenceTrigger(IdSrv) Then
                    Return True
                Else
                    Return False
                End If
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur IsNewSequenceTrigger: " & ex.ToString, "Erreur", "IsNewSequenceTrigger")
            Return False
        End Try
    End Function

    Public Function IsNewSequenceZone(IDSequence As String) As Boolean
        Try
            If IsConnect Then
                If IDSequence = myService.GetSequenceZone(IdSrv) Then
                    Return True
                Else
                    Return False
                End If
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur IsNewSequenceZone: " & ex.ToString, "Erreur", "IsNewSequenceZone")
            Return False
        End Try
    End Function

    Public Function IsNewSequenceMacro(IDSequence As String) As Boolean
        Try
            If IsConnect Then
                If IDSequence = myService.GetSequenceMacro(IdSrv) Then
                    Return True
                Else
                    Return False
                End If
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur IsNewSequenceMacro: " & ex.ToString, "Erreur", "IsNewSequenceMacro")
            Return False
        End Try
    End Function

    Public Function IsNewSequenceServer(IDSequence As String) As Boolean
        Try
            If IsConnect Then
                If IDSequence = myService.GetSequenceServer(IdSrv) Then
                    Return True
                Else
                    Return False
                End If
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur IsNewSequenceServer: " & ex.ToString, "Erreur", "IsNewSequenceServer")
            Return False
        End Try
    End Function
End Module
