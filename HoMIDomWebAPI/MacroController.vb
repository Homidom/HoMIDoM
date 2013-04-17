
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

End Class
