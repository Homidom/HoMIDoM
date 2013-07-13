Imports HoMIDom.HoMIDom
Imports System.Web.Http.SelfHost
Imports System.Web.Http
Imports Microsoft.Owin.Hosting
Imports Owin
Imports Microsoft.AspNet.SignalR

Public Class HoMIDomAPI
    Private Shared homidom As IHoMIDom
    Private Shared server As HttpSelfHostServer
    Private Shared signalR As IDisposable

    Public Shared Property CurrentServer() As IHoMIDom
        Get
            Return homidom
        End Get
        Set(ByVal value As IHoMIDom)
            If homidom IsNot Nothing Then
                RemoveHandler homidom.DeviceChanged, AddressOf DeviceChanged
            End If
            homidom = value
            AddHandler homidom.DeviceChanged, AddressOf DeviceChanged
        End Set
    End Property

    Public Shared Property ServerKey() As String

    Public Shared Sub Start(url As String, key As String)
        ServerKey = key
        Dim webApiUrl As String = url + "api/"
        Dim liveApiUrl As String = url + "live/"
        Dim config = New HttpSelfHostConfiguration(webApiUrl)

        config.Routes.MapHttpRoute("DefaultApi", "{key}/{controller}/{id}", New With { _
         .id = RouteParameter.[Optional] _
        })
        config.Routes.MapHttpRoute("CommandApi", "{key}/command/{controller}/{id}/{command}", New With { _
         .action = "ExecuteCommand" _
        })

        ' Add jsonp formatter for cross domain call
        config.Formatters(0) = New WebApiContrib.Formatting.Jsonp.JsonpMediaTypeFormatter() ' Replace default JSON by JSONP formatter

        Console.WriteLine(String.Format("{0} API Web démarrée sur l'adresse:  {1}", Now, webApiUrl))
        server = New HttpSelfHostServer(config)
        server.OpenAsync()

        ' Start SignalR notification hub
        signalR = WebApp.Start(Of Startup)(liveApiUrl)
        Console.WriteLine(String.Format("{0} API Live démarrée sur l'adresse:  {1}", Now, liveApiUrl))
    End Sub

    'Public Shared Sub Start(url As String, key As String)
    '    ServerKey = key
    '    Dim config = New HttpSelfHostConfiguration(url)

    '    config.Routes.MapHttpRoute("DefaultApi", "api/{key}/{controller}/{id}", New With { _
    '     .id = RouteParameter.[Optional] _
    '    })
    '    config.Routes.MapHttpRoute("CommandApi", "api/{key}/command/{controller}/{id}/{command}", New With { _
    '     .action = "ExecuteCommand" _
    '    })

    '    ' Add jsonp formatter for cross domain call
    '    config.Formatters(0) = New WebApiContrib.Formatting.Jsonp.JsonpMediaTypeFormatter() ' Replace default JSON by JSONP formatter

    '    Console.WriteLine(String.Format("{0} API Web démarrée sur l'adresse:  {1}", Now, url))
    '    server = New HttpSelfHostServer(config)
    '    server.OpenAsync()
    'End Sub

    Private Shared Sub DeviceChanged(id As String, val As String)
        Dim context As IHubContext = GlobalHost.ConnectionManager.GetHubContext(Of NotificationHub)()
        context.Clients.All.DeviceChanged(id, val)
    End Sub
End Class
