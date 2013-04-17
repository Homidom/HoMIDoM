
Imports System.Web.Http
Imports HoMIDom
Imports HoMIDom.HoMIDom

Public Class TriggerController
    Inherits BaseHoMIdomController

    Public Function [Get]() As List(Of Trigger)
        Return HoMIDomAPI.CurrentServer.GetAllTriggers(Me.ServerKey)
    End Function

    Public Function [Get](id As String) As Trigger
        Return HoMIDomAPI.CurrentServer.ReturnTriggerById(Me.ServerKey, id)
    End Function

End Class
