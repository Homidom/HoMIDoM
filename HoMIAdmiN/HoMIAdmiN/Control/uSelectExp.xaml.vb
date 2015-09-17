Public Class uSelectExp

    Public Event CloseMe(ByVal MyObject As Object)

    Private DevList As DeviceList
    Private ZoneList As RoomList
    Private SysInfo As SystemInfo
    Private Retour As ActionFeedback

    Private cptDevice As Integer
    Private cptRoom As Integer

    Private typeDevice As Dictionary(Of Integer, String)
    Private zoneName As Dictionary(Of String, String)

    Private allDevImperi As DeviceList

    Public Sub New()

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        Try
            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().

            typeDevice = New Dictionary(Of Integer, String)
            typeDevice.Add(1, "DevDimmer")
            typeDevice.Add(2, "DevMultiSwitch") '
            typeDevice.Add(3, "DevPressure")
            typeDevice.Add(4, "DevElectricity") '
            typeDevice.Add(5, "DevElectricity") '
            typeDevice.Add(6, "DevGenericSensor") '
            typeDevice.Add(7, "DevGenericSensor")
            typeDevice.Add(8, "DevWind")
            typeDevice.Add(9, "DevElectricity")
            typeDevice.Add(10, "DevElectricity")
            typeDevice.Add(11, "DevMultiSwitch")
            typeDevice.Add(12, "DevGenericSensor")
            typeDevice.Add(13, "DevThermostat")
            typeDevice.Add(14, "DevDimmer")
            typeDevice.Add(15, "DevHygrometry")
            typeDevice.Add(16, "DevDimmer")
            typeDevice.Add(17, "DevMultiSwitch")
            typeDevice.Add(18, "DevMultiSwitch")
            typeDevice.Add(19, "DevRain")
            typeDevice.Add(20, "DevRain")
            typeDevice.Add(21, "DevSwitch")
            typeDevice.Add(22, "DevMultiSwitch")
            typeDevice.Add(23, "DevTemperature")
            typeDevice.Add(24, "DevThermostat")
            typeDevice.Add(25, "DevUV")
            typeDevice.Add(26, "DevWind")
            typeDevice.Add(27, "DevShutter")
            typeDevice.Add(28, "DevRGBLight")

            'device
            Dim importOk = ImportImperiHome()
            For Each device In myService.GetAllDevices(IdSrv)

                Dim stk As New StackPanel
                stk.Orientation = Orientation.Horizontal
                Dim x As New CheckBox
                x.Content = device.Name
                x.ToolTip = device.ID
                x.HorizontalAlignment = HorizontalAlignment.Left
                stk.Children.Add(x)
                stk.HorizontalAlignment = HorizontalAlignment.Left
                ListBox1.Items.Add(stk)
                x.IsChecked = False
                If importOk Then
                    For Each dev In allDevImperi.devices
                        If dev.name = device.Name Then
                            x.IsChecked = True
                        End If
                    Next
                End If
                x = Nothing
            Next

            'zone
            zoneName = New Dictionary(Of String, String)
            For Each zone In myService.GetAllZones(IdSrv)
                zoneName.Add(zone.ID, zone.Name)
            Next

            'macro

            For Each macro In myService.GetAllMacros(IdSrv)
                Dim stk As New StackPanel
                stk.Orientation = Orientation.Horizontal
                Dim x As New CheckBox
                x.Content = macro.Nom
                x.ToolTip = macro.ID
                x.HorizontalAlignment = HorizontalAlignment.Left
                stk.Children.Add(x)
                stk.HorizontalAlignment = HorizontalAlignment.Left
                ListBox2.Items.Add(stk)
                x.IsChecked = False
                If importOk Then
                    For Each dev In allDevImperi.devices
                        If dev.name = macro.Nom Then
                            x.IsChecked = True
                        End If
                    Next
                End If
                x = Nothing
            Next

        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur lors de l'exécution de NewSelectElement: " & ex.ToString, "Erreur Admin", "")
        End Try
    End Sub

    Private Sub BtnCancel_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnCancel.Click

        RaiseEvent CloseMe(Me)

    End Sub

    Private Sub BtnOK_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        Try
            ExportImperiHome()
                RaiseEvent CloseMe(Me)

        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uSelectExp BtnOK_Click: " & ex.Message, "ERREUR", "")
        End Try
    End Sub

    Private Function ImportImperiHome() As Boolean


        Dim fileName = My.Application.Info.DirectoryPath & "\config\Imperihome\devices.json"

        If System.IO.File.Exists(fileName) Then
            Dim stream = System.IO.File.ReadAllText(fileName)
            allDevImperi = Newtonsoft.Json.JsonConvert.DeserializeObject(stream, GetType(DeviceList))
            Return True
        Else
            Return False
        End If

    End Function

    Private Function ExportImperiHome() As Boolean


        cptDevice = 0
        cptRoom = 0

        DevList = New DeviceList
        DevList.devices = New List(Of Device)
        ZoneList = New RoomList
        ZoneList.rooms = New List(Of Room)

        For Each dev As StackPanel In ListBox1.Items
            For Each child As CheckBox In dev.Children
                If child.IsChecked Then
                    cptDevice += 1

                    Dim comp = ReturnDeviceByID(child.ToolTip)

                    'device                    

                    Dim Temp As Device = New Device

                    Temp.id = "DEV" & Format(cptDevice, "000")

                    Temp.name = comp.Name

                    Temp.room = searchZone(comp.ID)

                    Temp.type = typeDevice(comp.Type)

                    Temp.params = ParamByType(comp)

                    DevList.devices.Add(Temp)

                End If
            Next

        Next
        For Each dev As StackPanel In ListBox2.Items
            For Each child As CheckBox In dev.Children
                If child.IsChecked Then
                    cptDevice += 1

                    Dim comp = myService.ReturnMacroById(IdSrv, child.ToolTip)

                    'macro

                    Dim Temp As Device = New Device

                    Temp.id = "DEV" & Format(cptDevice, "000")

                    Temp.name = comp.Nom

                    Temp.room = searchZone(comp.ID)

                    Temp.type = "DevScene"

                    Dim params As DeviceParam = New DeviceParam
                    params.key = "LastRun"
                    params.value = Now
                    Temp.params = New List(Of DeviceParam)
                    Temp.params.Add(params)

                    DevList.devices.Add(Temp)
                End If
            Next
        Next

        Dim stream = "<?php" & vbCrLf

        'stream &="($tempgarage = file_get_contents("http://192.168.0.90:7999/api/123456789/value/device/aaed77b5-a2ea-4aff-b37f-xxxxxxxxxx/Value"));" & vbCrLf
        'stream &="($humigarage = file_get_contents("http://192.168.0.90:7999/api/123456789/value/device/b530e999-c328-4c82-8aec-xxxxxxxxxx/Value"));" & vbCrLf
        'stream &="($detecgarage = file_get_contents("http://192.168.0.90:7999/api/123456789/value/device/233b4d1c-b3ba-4a60-bfe0-xxxxxxxxxx/Value"));" & vbCrLf
        'stream &="($voletethan = file_get_contents("http://192.168.0.90:7999/api/123456789/value/device/52aa2fff-6e5c-4295-a55c-xxxxxxxxxx/Value")); //retour d'état volet" & vbCrLf

        'stream &="$adresse = "http://".$_SERVER['SERVER_NAME'].$_SERVER["REQUEST_URI"]; //test l'url entrante" & vbCrLf
        'stream &= "$_SESSION['adresse'] = $adresse;" & vbCrLf

        'stream &="if ($adresse === "http://192.168.0.90/imperihome/devices/dev02/action/setLevel/0") { " & vbCrLf
        'stream &="  $handle = fopen("http://192.168.0.90:7999/api/123456789/command/device/af554ab0-4302-417f-a537-xxxxxxxxx/off/", "r"); " & vbCrLf
        'stream &= "} //fermeture volets" & vbCrLf

        'stream &="if ($adresse === "/http://192.168.0.90/imperihome/devices/dev02/action/setLevel/100") { " & vbCrLf
        'stream &="  $handle = fopen("http://192.168.0.90:7999/api/123456789/command/device/af554ab0-4302-417f-a537-xxxxxxxxxx/on/", "r"); " & vbCrLf
        'stream &= "} //ouverture volets" & vbCrLf

        'stream &= "echo ('" & vbCrLf
        'stream &= Newtonsoft.Json.JsonConvert.SerializeObject(DevList)


        System.IO.File.WriteAllText(My.Application.Info.DirectoryPath & "\config\Imperihome\devices", stream)

        stream = Newtonsoft.Json.JsonConvert.SerializeObject(ZoneList)
        System.IO.File.WriteAllText(My.Application.Info.DirectoryPath & "\config\Imperihome\rooms", stream)

        SysInfo = New SystemInfo
        SysInfo.id = IdSrv
        SysInfo.apiversion = 1
        stream = Newtonsoft.Json.JsonConvert.SerializeObject(SysInfo)
        System.IO.File.WriteAllText(My.Application.Info.DirectoryPath & "\config\Imperihome\system", stream)

        Retour = New ActionFeedback
        Retour.success = True
        Retour.errormsg = "ok"
        stream = Newtonsoft.Json.JsonConvert.SerializeObject(Retour)
        System.IO.File.WriteAllText(My.Application.Info.DirectoryPath & "\config\Imperihome\action_ret", stream)

    End Function
   
    Private Function searchZone(ByVal idDevice As String) As String

        Dim idzone As String = ""
        Dim devZone As List(Of String)
        For Each zones In zoneName
            devZone = myService.GetDeviceInZone(IdSrv, zones.Key)
            devZone.AddRange(myService.GetMacroInZone(IdSrv, zones.Key))
            For Each iddev In devZone
                If iddev = idDevice Then
                    idzone = zones.Key
                    Exit For
                End If
            Next
            If idzone = zones.Key Then
                Exit For
            End If
        Next

        Dim tempRoom As Room = New Room

        If idzone = "" Then
            If ZoneList.rooms IsNot Nothing Then
                For Each room As Room In ZoneList.rooms
                    If room.name = "Reserve" Then
                        Return room.id
                        Exit Function
                    End If
                Next
            End If
            tempRoom.id = "ROOM99" & Format(cptRoom, "00")
            tempRoom.name = "Reserve"
            ZoneList.rooms.Add(tempRoom)
            Return tempRoom.id
            Exit Function
        End If

        If zoneList.rooms IsNot Nothing Then
            For Each room As Room In zoneList.rooms
                If room.name = zoneName(idzone) Then
                    Return room.id
                    Exit Function
                End If
            Next
        End If
        cptRoom += 1
        tempRoom.id = "ROOM" & Format(cptRoom, "00")
        tempRoom.name = zoneName(idzone)
        zoneList.rooms.Add(tempRoom)
        Return tempRoom.id

    End Function

    Private Function ParamByType(ByVal comp As HoMIDom.HoMIDom.TemplateDevice) As List(Of DeviceParam)
        Dim tempParams As List(Of DeviceParam) = New List(Of DeviceParam)
        Dim params(10) As DeviceParam

        Select Case comp.Type

            Case 1 '"APPAREIL"
                params(0) = New DeviceParam
                params(0).key = "Status"
                params(0).value = comp.Value

            Case 27 '"VOLET"
                params(0) = New DeviceParam
                params(0).key = "Level"
                params(0).value = comp.Value

            Case 16 '"LAMPE"
                params(0) = New DeviceParam
                params(0).key = "Level"
                params(0).value = comp.Value
                params(1) = New DeviceParam
                params(1).key = "Status"
                params(1).value = comp.Value

            Case 9 '"ENERGIEINSTANTANEE"
                params(0) = New DeviceParam
                params(0).key = "Watts"
                params(0).value = comp.Value

            Case 10 '"ENERGIETOTALE"
                params(0) = New DeviceParam
                params(0).key = "ConsoTotal"
                params(0).value = comp.Value

            Case 15, 25, 3, 19, 21, 23, 7 '"HUMIDITE", "UV", "BAROMETRE, "PLUIECOURANT", "SWITCH", "TEMPERATURE", "DETECTEUR"
                params(0) = New DeviceParam
                params(0).key = "Value"
                params(0).value = comp.Value

            Case 26 '"VITESSEVENT"
                params(0) = New DeviceParam
                params(0).key = "Speed"
                params(0).value = comp.Value

            Case 8 ' "DIRECTIONVENT"
                params(0) = New DeviceParam
                params(0).key = "Direction"
                params(0).value = comp.Value

            Case 20 '"PLUIETOTAL"
                params(0) = New DeviceParam
                params(0).key = "Accumulation"
                params(0).value = comp.Value

            Case 24 '"TEMPERATURECONSIGNE"
                params(0) = New DeviceParam
                params(0).key = "cursetpoint"
                params(0).value = comp.Value

            Case 28 '"LAMPERGBW"
                params(0) = New DeviceParam
                params(0).key = "Level"
                params(0).value = comp.Value
                params(1) = New DeviceParam
                params(1).key = "whitechannel"
                params(1).value = comp.temperature
                params(2) = New DeviceParam
                params(2).key = "color"
                params(2).value = comp.optionnal

            Case Else
                params(0) = New DeviceParam
                params(0).key = "Value"
                params(0).value = comp.Value

        End Select

        For i = 0 To params.Count - 1
            If params(i) IsNot Nothing Then
                tempParams.Add(params(i))
            End If
        Next

        Return tempParams
    End Function

End Class

''' <summary>Class DeviceParam, Défini le type parametre de composant pour le client Imperihome</summary>
<Serializable()> Public Class DeviceParam

    Public key As String = ""
    Public value As String = ""
    Public unit As String = ""
    Public graphable As Boolean = False

End Class

''' <summary>Class Device, Défini le type composant pour le client Imperihome</summary>
<Serializable()> Public Class Device

    Public id As String = ""
    Public name As String = ""
    Public type As String = ""
    Public room As String = ""
    Public params As List(Of DeviceParam)

End Class

''' <summary>Class DeviceList, Défini le type liste des composants pour le client Imperihome</summary>
<Serializable()> Public Class DeviceList

    Public devices As List(Of Device)

End Class

''' <summary>Class Room, Défini le type piece pour le client Imperihome</summary>
<Serializable()> Public Class Room

    Public id As String = ""
    Public name As String = ""

End Class

''' <summary>Class RoomList, Défini le type liste des pieces pour le client Imperihome</summary>
<Serializable()> Public Class RoomList

    Public rooms As List(Of Room) = New List(Of Room)

End Class

''' <summary>Class Room, Défini le type system pour le client Imperihome</summary>
<Serializable()> Public Class SystemInfo

    Public id As String = ""
    Public apiversion As Integer = 1

End Class

''' <summary>Class Room, Défini le type retour pour le client Imperihome</summary>
<Serializable()> Public Class ActionFeedback

    Public success As Boolean = True
    Public errormsg As String = ""

End Class
