
Imports System.Web.Http
Imports HoMIDom
Imports HoMIDom.HoMIDom

Public Class DriverController
    Inherits BaseHoMIdomController

    Public Function [Get]() As List(Of TemplateDriver)
        Return HoMIDomAPI.CurrentServer.GetAllDrivers(Me.ServerKey)
    End Function

    Public Function [Get](id As String) As TemplateDriver
        Return HoMIDomAPI.CurrentServer.ReturnDriverByID(Me.ServerKey, id)
    End Function


    <HttpGet()>
    Public Function ExecuteCommand(id As String, command As String) As Boolean
        HoMIDomAPI.CurrentServer.ExecuteDriverCommand(Me.ServerKey, id, New DeviceAction() With {.Nom = command})
        Return True
    End Function

End Class
