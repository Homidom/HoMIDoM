
Imports Microsoft.AspNet.SignalR
Imports Microsoft.AspNet.SignalR.Hubs
Imports Owin

Public Class Startup
    Public Sub Configuration(ByVal app As IAppBuilder)

        Dim config = New HubConfiguration With {.EnableCrossDomain = True}
        app.MapHubs(config)
    End Sub
End Class


<HubName("NotificationHub")> _
<HubAuthorize()> _
Public Class NotificationHub
    Inherits Hub
    Public Sub Test(param As String)
        Console.WriteLine(param)
        Clients.All.test(param)
    End Sub
End Class


Public Class HubAuthorizeAttribute
    Inherits Attribute
    Implements IAuthorizeHubConnection

    Public Function AuthorizeHubConnection(hubDescriptor As HubDescriptor, request As IRequest) As Boolean Implements IAuthorizeHubConnection.AuthorizeHubConnection
        Return request.Headers("sKey") = HoMIDomAPI.ServerKey
    End Function
End Class