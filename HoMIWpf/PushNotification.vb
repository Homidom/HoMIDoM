Imports System.Collections.Generic
Imports Microsoft.AspNet.SignalR.Client.Hubs

Public Class PushNotification
    Dim _Url As String
    Dim _Key As String
    Dim _Wait As Integer = 30000
    Dim _Connection As HubConnection

    Public Event DeviceChanged(ByVal DeviceId As String, ByVal DeviceValue As String)

    Public Sub PushNotification(ByVal url As String, ByVal serverKey As String)
        Me._Url = url
        Me._Key = serverKey
    End Sub

    Public Function Open() As Boolean
        _Connection = New HubConnection(_Url)
        _Connection.Headers.Add("sKey", Me._Key)
        Dim hub = _Connection.CreateHubProxy("NotificationHub")
        hub.On(Of String, String)("DeviceChanged", AddressOf Me.OnDeviceChanged)

        Return _Connection.Start().Wait(_Wait)
    End Function

    Public Sub Close()
        _Connection.Stop()
    End Sub

    Protected Function OnDeviceChanged(ByVal id As String, ByVal val As String) As Boolean
        RaiseEvent DeviceChanged(id, val)
    End Function

    Public Sub New(ByVal url As String, ByVal serverKey As String, Optional Wait As Integer = 30000)
        Me._Url = url
        Me._Key = serverKey
        Me._Wait = Wait
    End Sub
End Class
