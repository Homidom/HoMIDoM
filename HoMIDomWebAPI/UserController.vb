
Imports System.Web.Http
Imports HoMIDom
Imports HoMIDom.HoMIDom

Public Class UserController
    Inherits BaseHoMIdomController

    Public Function [Get]() As List(Of Users.User)
        Return HoMIDomAPI.CurrentServer.GetAllUsers(Me.ServerKey)
    End Function

    Public Function [Get](id As String) As Users.User
        Return HoMIDomAPI.CurrentServer.ReturnUserByUsername(Me.ServerKey, id)
    End Function


    <HttpGet()>
    Public Function ExecuteCommand(id As String, command As String) As Boolean
        HoMIDomAPI.CurrentServer.ExecuteDeviceCommand(Me.ServerKey, id, New DeviceAction() With {.Nom = command})
        Return True
    End Function

End Class
