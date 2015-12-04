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
    Private allZoneImperi As RoomList

    Public Sub New()

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        Try
            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().

            typeDevice = New Dictionary(Of Integer, String)
            typeDevice.Add(1, "DevDimmer") 'APPAREIL = 1 ?DevSwitch?
            typeDevice.Add(2, "DevGenericSensor") 'AUDIO = 2
            typeDevice.Add(3, "DevPressure") 'BAROMETRE = 3
            typeDevice.Add(4, "DevGenericSensor")  'BATTERIE = 4
            typeDevice.Add(5, "DevGenericSensor") 'COMPTEUR = 5
            typeDevice.Add(6, "DevGenericSensor") 'CONTACT = 6
            typeDevice.Add(7, "DevGenericSensor") 'DETECTEUR = 7
            typeDevice.Add(8, "DevWind") 'DIRECTIONVENT = 8
            typeDevice.Add(9, "DevElectricity") 'ENERGIEINSTANTANEE = 9
            typeDevice.Add(10, "DevElectricity") 'ENERGIETOTALE = 10
            typeDevice.Add(11, "DevGenericSensor") 'FREEBOX = 11
            typeDevice.Add(12, "DevGenericSensor") 'GENERIQUEBOOLEEN = 12
            typeDevice.Add(13, "DevMultiSwitch") 'GENERIQUESTRING = 13
            typeDevice.Add(14, "DevGenericSensor") 'GENERIQUEVALUE = 14
            typeDevice.Add(15, "DevHygrometry") 'HUMIDITE = 15
            typeDevice.Add(16, "DevDimmer") 'LAMPE = 16
            typeDevice.Add(17, "DevGenericSensor") 'METEO = 17
            typeDevice.Add(18, "DevGenericSensor") 'MULTIMEDIA = 18
            typeDevice.Add(19, "DevRain") 'PLUIECOURANT = 19
            typeDevice.Add(20, "DevRain") 'PLUIETOTAL = 20
            typeDevice.Add(21, "DevSwitch") 'Switch = 21
            typeDevice.Add(22, "DevMultiSwitch") 'TELECOMMANDE = 22
            typeDevice.Add(23, "DevTemperature") 'TEMPERATURE = 23
            typeDevice.Add(24, "DevThermostat") 'TEMPERATURECONSIGNE = 24
            typeDevice.Add(25, "DevUV") 'UV = 25
            typeDevice.Add(26, "DevWind") 'VITESSEVENT = 26
            typeDevice.Add(27, "DevShutter") 'VOLET = 27
            typeDevice.Add(28, "DevRGBLight") 'LAMPERGBW = 28


            'device
            Dim importOk = ImportImperiHome()
            For Each device In myService.GetAllDevices(IdSrv)

                Dim x As New CheckBox
                Dim y As New ComboBox

                x.Content = device.Name
                x.ToolTip = device.ID
                x.HorizontalAlignment = HorizontalAlignment.Left
                x.IsChecked = False

                x.Width = 300
                AddHandler x.Click, AddressOf ChkElement_Click

                y.Items.Add("DevCamera")
                y.Items.Add("DevCO2")
                y.Items.Add("DevCO2Alert")
                y.Items.Add("DevDimmer")
                y.Items.Add("DevDoor")
                y.Items.Add("DevElectricity")
                y.Items.Add("DevFlood")
                y.Items.Add("DevGenericSensor")
                y.Items.Add("DevHygrometry")
                y.Items.Add("DevLock")
                y.Items.Add("DevLuminosity")
                y.Items.Add("DevMotion")
                y.Items.Add("DevMultiSwitch")
                y.Items.Add("DevNoise")
                y.Items.Add("DevPressure")
                y.Items.Add("DevRain")
                y.Items.Add("DevRGBLight")
                y.Items.Add("DevScene")
                y.Items.Add("DevShutter")
                y.Items.Add("DevSmoke")
                y.Items.Add("DevSwitch")
                y.Items.Add("DevTemperature")
                y.Items.Add("DevTempHygro")
                y.Items.Add("DevThermostat")
                y.Items.Add("DevUV")
                y.Items.Add("DevWind")
                y.HorizontalAlignment = HorizontalAlignment.Right
                y.Text = typeDevice(device.Type)
                If importOk Then
                    For Each dev In allDevImperi.devices
                        If dev.name = device.Name Then
                            x.IsChecked = True
                            y.Text = dev.type
                        End If
                    Next
                End If
                If x.IsChecked = True Then
                    y.Visibility = Windows.Visibility.Visible
                Else
                    y.Visibility = Windows.Visibility.Collapsed
                End If
                Dim stk As New StackPanel
                stk.Orientation = Orientation.Horizontal
                stk.Children.Add(x)
                stk.Children.Add(y)
                stk.HorizontalAlignment = HorizontalAlignment.Left
                ListBoxDevices.Items.Add(stk)
                x = Nothing
            Next

            'zone
            zoneName = New Dictionary(Of String, String)
            For Each zone In myService.GetAllZones(IdSrv)
                zoneName.Add(zone.ID, zone.Name)
            Next

            'macro

            For Each macro In myService.GetAllMacros(IdSrv)
                Dim x As New CheckBox
                x.Content = macro.Nom
                x.ToolTip = macro.ID
                x.HorizontalAlignment = HorizontalAlignment.Left
                x.IsChecked = False
                If importOk Then
                    For Each dev In allDevImperi.devices
                        If dev.name = macro.Nom Then
                            x.IsChecked = True
                        End If
                    Next
                End If
                Dim stk As New StackPanel
                stk.Orientation = Orientation.Horizontal
                stk.Children.Add(x)
                stk.HorizontalAlignment = HorizontalAlignment.Left
                ListBoxMacros.Items.Add(stk)
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
        Try
            Dim fileName = My.Application.Info.DirectoryPath & "\Drivers\Imperihome\devices.json"

            If System.IO.File.Exists(fileName) Then
                Dim stream = System.IO.File.ReadAllText(fileName)
                allDevImperi = Newtonsoft.Json.JsonConvert.DeserializeObject(stream, GetType(DeviceList))
            Else
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR ImportImperiHome: Le fichier " & fileName & " n'existe pas", "ERREUR", "ImportImperiHome")
                Return False
                Exit Function
            End If
            Dim fileName1 = My.Application.Info.DirectoryPath & "\Drivers\Imperihome\rooms.json"

            If System.IO.File.Exists(fileName1) Then
                Dim stream = System.IO.File.ReadAllText(fileName1)
                allZoneImperi = Newtonsoft.Json.JsonConvert.DeserializeObject(stream, GetType(RoomList))
            Else
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR ImportImperiHome: Le fichier " & fileName & " n'existe pas", "ERREUR", "ImportImperiHome")
                Return False
                Exit Function
            End If
            Return True
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR ImportImperiHome: " & ex.ToString, "ERREUR", "ImportImperiHome")
            Return Nothing
        End Try
    End Function

    Private Function ExportImperiHome() As Boolean
        Try

            cptDevice = 0
            cptRoom = 0
            Dim idfind As String = ""
            Dim memId As Integer = 0

            DevList = New DeviceList
            DevList.devices = New List(Of Device)
            ZoneList = New RoomList
            ZoneList.rooms = New List(Of Room)

            For Each dev As StackPanel In ListBoxDevices.Items

                Dim x As CheckBox = dev.Children.Item(0)
                Dim y As ComboBox = dev.Children.Item(1)
                If x.IsChecked Then
                    Dim comp = ReturnDeviceByID(x.ToolTip)
                    'device  
                    idfind = ""
                    memId = 0
                    Dim Temp As Device = New Device
                    If allDevImperi IsNot Nothing Then
                        For Each impdev In allDevImperi.devices
                            If comp.Name = impdev.name Then idfind = impdev.id
                            If CInt(Right(impdev.id, 3)) > memId Then memId = CInt(Right(impdev.id, 3))
                        Next
                    End If
                    If idfind = "" Then
                        cptDevice += 1
                        Temp.id = "DEV" & Format(memId + cptDevice, "000")
                    Else
                        Temp.id = idfind
                    End If
                    Temp.name = comp.Name
                    Temp.room = searchZone(comp.ID)
                    Temp.type = y.Text
                    Temp.params = ParamByType(comp)
                    DevList.devices.Add(Temp)
                End If
            Next

            For Each dev As StackPanel In ListBoxMacros.Items

                Dim z As CheckBox = dev.Children.Item(0)
                If z.IsChecked Then
                    Dim comp = myService.ReturnMacroById(IdSrv, z.ToolTip)
                    'macro
                    idfind = ""
                    memId = 0
                    Dim Temp As Device = New Device

                    If allDevImperi IsNot Nothing Then
                        For Each impdev In allDevImperi.devices
                            If comp.Nom = impdev.name Then idfind = impdev.id
                            If CInt(Right(impdev.id, 3)) > memId Then memId = CInt(Right(impdev.id, 3))
                        Next
                    End If
                    If idfind = "" Then
                        cptDevice += 1
                        Temp.id = "DEV" & Format(memId + cptDevice, "000")
                    Else
                        Temp.id = idfind
                    End If
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
            Dim BasePath As System.IO.DirectoryInfo = New System.IO.DirectoryInfo(My.Application.Info.DirectoryPath & "\Drivers\")
            Dim NewPath As System.IO.DirectoryInfo = New System.IO.DirectoryInfo("Imperihome")
            Dim DitPath() As System.IO.DirectoryInfo
            DitPath = BasePath.GetDirectories
            If Not DitPath.Contains(NewPath) Then
                BasePath.CreateSubdirectory("Imperihome")
            End If

            Dim stream As String = ""

            stream = Newtonsoft.Json.JsonConvert.SerializeObject(DevList)
            System.IO.File.WriteAllText(My.Application.Info.DirectoryPath & "\Drivers\Imperihome\devices.json", stream)

            stream = Newtonsoft.Json.JsonConvert.SerializeObject(ZoneList)
            System.IO.File.WriteAllText(My.Application.Info.DirectoryPath & "\Drivers\Imperihome\rooms.json", stream)

            SysInfo = New SystemInfo
            SysInfo.id = IdSrv
            SysInfo.apiversion = 1
            stream = Newtonsoft.Json.JsonConvert.SerializeObject(SysInfo)
            System.IO.File.WriteAllText(My.Application.Info.DirectoryPath & "\Drivers\Imperihome\system.json", stream)

            Retour = New ActionFeedback
            Retour.success = True
            Retour.errormsg = "ok"
            stream = Newtonsoft.Json.JsonConvert.SerializeObject(Retour)
            System.IO.File.WriteAllText(My.Application.Info.DirectoryPath & "\Drivers\Imperihome\action_ret.json", stream)

            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.INFO, "L'export de la configuration pour ImperHome a été effectué", "Export Vers ImperiHome", "ExportImperiHome")
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR ExportImperiHome: " & ex.ToString, "ERREUR", "ExportImperiHome")
            Return Nothing
        End Try
    End Function

    Private Sub ChkElement_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        Try
            For Each stk As StackPanel In ListBoxDevices.Items
                Dim x As CheckBox = stk.Children.Item(0)
                If x.IsChecked = True Then
                    stk.Children.Item(1).Visibility = Windows.Visibility.Visible
                Else
                    stk.Children.Item(1).Visibility = Windows.Visibility.Collapsed
                End If
            Next
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uSelectExp ChkElement_Click: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Function searchZone(ByVal idDevice As String) As String
        Try
            Dim idzone As String = ""
            Dim devZone As List(Of String)
            If ZoneList Is Nothing Then ZoneList = allZoneImperi
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
                tempRoom.id = "ROOM999"
                tempRoom.name = "Reserve"
                ZoneList.rooms.Add(tempRoom)
                Return tempRoom.id
                Exit Function
            End If
            Dim idfind As String = ""
            Dim memId As Integer = 0
            If ZoneList.rooms IsNot Nothing Then
                For Each room As Room In ZoneList.rooms
                    If room.name = zoneName(idzone) Then
                        If room.name = zoneName(idzone) Then idfind = room.id
                        If CInt(Right(room.id, 3)) > memId Then memId = CInt(Right(room.id, 3))
                    End If
                Next
            End If
            If idfind = "" Then
                tempRoom.id = "ROOM" & Format(memId + 1, "000")
                ZoneList.rooms.Add(tempRoom)
            Else
                tempRoom.id = idfind
            End If
            tempRoom.name = zoneName(idzone)
            Return tempRoom.id
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR searchZone: " & ex.ToString, "ERREUR", "searchZone")
            Return ""
        End Try
    End Function


    Private Function ParamByType(ByVal comp As HoMIDom.HoMIDom.TemplateDevice) As List(Of DeviceParam)
        Try
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
                    If comp.Value > 0 Then
                        params(1).value = "1"
                    Else
                        params(1).value = "0"
                    End If

                Case 9 '"ENERGIEINSTANTANEE"
                    params(0) = New DeviceParam
                    params(0).key = "Watts"
                    params(0).value = comp.Value

                Case 10 '"ENERGIETOTALE"
                    params(0) = New DeviceParam
                    params(0).key = "ConsoTotal"
                    params(0).value = comp.Value

                Case 15, 25, 3, 19, 23, 7 '"HUMIDITE", "UV", "BAROMETRE, "PLUIECOURANT", "TEMPERATURE", "DETECTEUR"
                    params(0) = New DeviceParam
                    params(0).key = "Value"
                    params(0).value = comp.Value

                Case 21 '"SWITCH"
                    params(0) = New DeviceParam
                    params(0).key = "Status"
                    If comp.Value = False Then
                        params(0).value = "0"
                    Else
                        params(0).value = "1"
                    End If

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
                    If params(i).value.Contains(","c) Then
                        params(i).value.Replace(Chr(44), Chr(46))
                    End If
                    tempParams.Add(params(i))
                End If
            Next

            Return tempParams
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR ParamByType: " & ex.ToString, "ERREUR", "ParamByType")
            Return Nothing
        End Try
    End Function

    Private Sub Chkselectdevices_CheckedUnchecked(sender As Object, e As RoutedEventArgs) Handles Chkselectdevices.Checked, Chkselectdevices.Unchecked
        Try
            For Each dev As StackPanel In ListBoxDevices.Items
                'For Each child As CheckBox In dev.Children
                Dim child As CheckBox = dev.Children.Item(0)
                child.IsChecked = Chkselectdevices.IsChecked
                If child.IsChecked Then
                    dev.Children.Item(1).Visibility = Windows.Visibility.Visible
                Else
                    dev.Children.Item(1).Visibility = Windows.Visibility.Collapsed
                End If
                'Next
            Next
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Chkselectdevices_CheckedUnchecked: " & ex.ToString, "ERREUR", "Chkselectdevices_CheckedUnchecked")
        End Try
    End Sub

    Private Sub Chkselectmacros_CheckedUnchecked(sender As Object, e As RoutedEventArgs) Handles Chkselectmacros.Checked, Chkselectmacros.Unchecked
        Try
            For Each dev As StackPanel In ListBoxMacros.Items
                'For Each child As CheckBox In dev.Children
                Dim child As CheckBox = dev.Children.Item(0)
                child.IsChecked = Chkselectmacros.IsChecked
                If child.IsChecked Then
                    dev.Children.Item(1).Visibility = Windows.Visibility.Visible
                Else
                    dev.Children.Item(1).Visibility = Windows.Visibility.Collapsed
                End If
                'Next
            Next
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Chkselectmacros_CheckedUnchecked: " & ex.ToString, "ERREUR", "Chkselectmacros_CheckedUnchecked")
        End Try
    End Sub

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
