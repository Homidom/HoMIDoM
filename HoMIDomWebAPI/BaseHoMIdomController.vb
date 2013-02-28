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

End Class
