Imports HoMIDom.HoMIDom
Imports System.Web.Http.SelfHost
Imports System.Web.Http

Public Class HoMIDomAPI
    Private Shared server As HttpSelfHostServer

    Public Shared Property CurrentServer() As IHoMIDom

    Public Shared Property ServerKey() As String

    Public Shared Sub Start(url As String, key As String)
        ServerKey = key
        Dim config = New HttpSelfHostConfiguration(url)

        config.Routes.MapHttpRoute("DefaultApi", "api/{key}/{controller}/{id}", New With { _
         .id = RouteParameter.[Optional] _
        })
        config.Routes.MapHttpRoute("CommandApi", "api/{key}/command/{controller}/{id}/{command}", New With { _
         .action = "ExecuteCommand" _
        })

        ' Add jsonp formatter for cross domain call
        config.Formatters(0) = New WebApiContrib.Formatting.Jsonp.JsonpMediaTypeFormatter() ' Replace default JSON by JSONP formatter

        Console.WriteLine(String.Format("{0} API Web démarrée sur l'adresse:  {1}", Now, url))
        server = New HttpSelfHostServer(config)
        server.OpenAsync()
    End Sub
End Class
