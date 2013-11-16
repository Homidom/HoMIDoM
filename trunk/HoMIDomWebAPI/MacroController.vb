
Imports System.Web.Http
Imports HoMIDom
Imports HoMIDom.HoMIDom

Public Class MacroController
    Inherits BaseHoMIdomController

    Public Function [Get]() As List(Of Macro)
        Return HoMIDomAPI.CurrentServer.GetAllMacros(Me.ServerKey)
    End Function

    Public Function [Get](id As String) As Macro
        Return HoMIDomAPI.CurrentServer.ReturnMacroById(Me.ServerKey, id)
    End Function

    <HttpGet()>
    Public Function ExecuteCommand(id As String, command As String) As Boolean
        HoMIDomAPI.CurrentServer.RunMacro(Me.ServerKey, id)
        Return True
    End Function

    <HttpGet()>
    Public Function GetValue(id As String, field As String) As Object
        Return Me.GetField(Me.Get(id), field)
    End Function
End Class
