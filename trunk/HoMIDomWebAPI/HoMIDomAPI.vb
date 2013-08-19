Imports HoMIDom.HoMIDom
Imports HoMIDom.HoMIDom.Server
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
            Try
                If homidom IsNot Nothing Then
                    RemoveHandler homidom.DeviceChanged, AddressOf DeviceChanged
                    RemoveHandler homidom.DriverChanged, AddressOf DriverChanged
                    RemoveHandler homidom.NewLog, AddressOf NewLog
                    RemoveHandler homidom.ZoneChanged, AddressOf ZoneChanged
                    RemoveHandler homidom.DriverChanged, AddressOf DriverChanged
                    RemoveHandler homidom.MacroChanged, AddressOf MacroChanged
                    RemoveHandler homidom.HeureSoleilChanged, AddressOf HeureSoleilChanged
                End If
                homidom = value
                AddHandler homidom.DeviceChanged, AddressOf DeviceChanged
                AddHandler homidom.DriverChanged, AddressOf DriverChanged
                AddHandler homidom.NewLog, AddressOf NewLog
                AddHandler homidom.ZoneChanged, AddressOf ZoneChanged
                AddHandler homidom.DriverChanged, AddressOf DriverChanged
                AddHandler homidom.MacroChanged, AddressOf MacroChanged
                AddHandler homidom.HeureSoleilChanged, AddressOf HeureSoleilChanged
            Catch ex As Exception
                If (Environment.UserInteractive) Then
                    Console.WriteLine(Now & " ERROR   WEBAPI : CurrentServer ERROR : " & ex.ToString)
                Else
                    Dim myEventLog = New EventLog()
                    myEventLog.Source = "HoMIServicE"
                    myEventLog.WriteEntry("WEBAPI : CurrentServer ERROR : " & ex.ToString, EventLogEntryType.Error, 90)
                End If
            End Try
        End Set
    End Property

    Public Shared Property ServerKey() As String

    Public Shared Sub Start(url As String, key As String)
        Try
            ServerKey = key
            Dim configUrl = New Uri(url)
            Dim webApiUrl As String = url + "api/"
            Dim liveApiUrl As String = String.Format("http://*:{0}/live/", configUrl.Port)
            Dim config = New HttpSelfHostConfiguration(webApiUrl)

            config.Routes.MapHttpRoute("DefaultApi", "{key}/{controller}/{id}", New With { _
             .id = RouteParameter.[Optional] _
            })
            config.Routes.MapHttpRoute("CommandApi", "{key}/command/{controller}/{id}/{command}", New With { _
             .action = "ExecuteCommand" _
            })

            ' Add jsonp formatter for cross domain call
            config.Formatters(0) = New WebApiContrib.Formatting.Jsonp.JsonpMediaTypeFormatter() ' Replace default JSON by JSONP formatter

            If (Environment.UserInteractive) Then Console.WriteLine(Now & " INFO    API Web démarrée sur l'adresse :  " & webApiUrl)
            server = New HttpSelfHostServer(config)
            server.OpenAsync()

            ' Start SignalR notification hub
            signalR = WebApp.Start(Of Startup)(liveApiUrl)
            If (Environment.UserInteractive) Then Console.WriteLine(Now & " INFO    API Live démarrée sur l'adresse : " & liveApiUrl)
        Catch ex As Exception
            If (Environment.UserInteractive) Then
                Console.WriteLine(Now & " ERROR   WEBAPI : Start ERROR : " & ex.ToString)
            Else
                Dim myEventLog = New EventLog()
                myEventLog.Source = "HoMIServicE"
                myEventLog.WriteEntry("WEBAPI : Start ERROR : " & ex.ToString, EventLogEntryType.Error, 90)
            End If
        End Try
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

    Private Shared Sub NewLog(ByVal TypLog As HoMIDom.HoMIDom.Server.TypeLog, ByVal Source As HoMIDom.HoMIDom.Server.TypeSource, ByVal Fonction As String, ByVal Message As String) 'Evènement lorsqu'un nouveau log est écrit
        Dim context As IHubContext = GlobalHost.ConnectionManager.GetHubContext(Of NotificationHub)()
        context.Clients.All.NewLog(TypLog, Source, Fonction, Message)
    End Sub
    Private Shared Sub MessageFromServeur(Id As String, Time As DateTime, Message As String) 'Message provenant du serveur
        Dim context As IHubContext = GlobalHost.ConnectionManager.GetHubContext(Of NotificationHub)()
        context.Clients.All.MessageFromServeur(Id, Time, Message)
    End Sub
    Private Shared Sub DriverChanged(DriverId As String) 'Evènement lorsq'un driver est modifié
        Dim context As IHubContext = GlobalHost.ConnectionManager.GetHubContext(Of NotificationHub)()
        context.Clients.All.DriverChanged(DriverId)
    End Sub
    Private Shared Sub ZoneChanged(ZoneId As String) 'Evènement lorsq'une zone est modifiée ou créée
        Dim context As IHubContext = GlobalHost.ConnectionManager.GetHubContext(Of NotificationHub)()
        context.Clients.All.ZoneChanged(ZoneId)
    End Sub
    Private Shared Sub MacroChanged(MacroId As String) 'Evènement lorsq'une macro est modifiée ou créée
        Dim context As IHubContext = GlobalHost.ConnectionManager.GetHubContext(Of NotificationHub)()
        context.Clients.All.MacroChanged(MacroId)
    End Sub
    Private Shared Sub HeureSoleilChanged() 'Evènement lorsque l'heure de lever/couché du soleil est modifié
        Dim context As IHubContext = GlobalHost.ConnectionManager.GetHubContext(Of NotificationHub)()
        context.Clients.All.HeureSoleilChanged()
    End Sub
End Class
