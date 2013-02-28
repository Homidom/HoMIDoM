
Imports System.Web.Http
Imports HoMIDom
Imports HoMIDom.HoMIDom

Public Class DeviceController
    Inherits BaseHoMIdomController

    Public Function [Get]() As List(Of TemplateDevice)
        Return HoMIDomAPI.CurrentServer.GetAllDevices(Me.ServerKey)
    End Function

    Public Function [Get](id As String) As TemplateDevice
        Return HoMIDomAPI.CurrentServer.ReturnDeviceByID(Me.ServerKey, id)
    End Function


    <HttpGet()>
    Public Function ExecuteCommand(id As String, command As String) As Boolean
        HoMIDomAPI.CurrentServer.ExecuteDeviceCommand(Me.ServerKey, id, New DeviceAction() With {.Nom = command})
        Return True
    End Function
End Class
