Imports System.Web.Http
Imports HoMIDom
Imports HoMIDom.HoMIDom

Public MustInherit Class BaseHoMIdomController
    Inherits ApiController

    'Public Property ServerKey As String
    Public ReadOnly Property ServerKey() As String
        Get
            Return Me.ControllerContext.RouteData.Values("key")
        End Get
    End Property


    Protected Function GetField(ByVal device As Object, ByVal field As String) As Object
        Try
            Dim oType = device.GetType()
            Dim oProp = oType.GetProperty(field)
            If oProp = Nothing Then
                Throw New ArgumentException(String.Format("Le champs '{0}' n'existe pas sur cet élément.", field))
            End If

            Dim oPropGet = oProp.GetGetMethod()
            Return oPropGet.Invoke(device, Nothing)
        Catch ex As Exception
            Throw
        End Try
    End Function

End Class
