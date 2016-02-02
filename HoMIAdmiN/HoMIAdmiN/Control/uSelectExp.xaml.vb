Public Class uSelectExp

    Public Event CloseMe(ByVal MyObject As Object)

    Private SysInfo As SystemInfo
    Private Retour As ActionFeedback

    Private typeDevice As Dictionary(Of Integer, String)
    Private zoneName As Dictionary(Of String, String)
    Private devZone As New Dictionary(Of String, List(Of String)) 'liste des composants par zones

    Private allDevImperi As DeviceList
    Private allZoneImperi As RoomList

    Private allDevice As List(Of HoMIDom.HoMIDom.TemplateDevice) = myService.GetAllDevices(IdSrv)
    Private allMacro As List(Of HoMIDom.HoMIDom.Macro) = myService.GetAllMacros(IdSrv)

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
            typeDevice.Add(13, "DevGenericSensor") 'GENERIQUESTRING = 13
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

            'zone
            ImportImperiHomeZone()

            zoneName = New Dictionary(Of String, String)
            zoneName.Add("", "Reserve")
            For Each zone In myService.GetAllZones(IdSrv)
                zoneName.Add(zone.ID, zone.Name)
            Next

            If allZoneImperi Is Nothing Then
                allZoneImperi = New RoomList
                allZoneImperi.rooms = New List(Of Room)
            Else
                Dim zoneFind = New List(Of Room)
                For Each zone In allZoneImperi.rooms
                    If zone.id = "ROOM999" Then zoneFind.Add(zone)
                Next
                For Each zone In zoneFind
                    allZoneImperi.rooms.Remove(zone)
                Next
            End If
            Dim tempRoom As Room = New Room
            tempRoom.id = "ROOM999"
            tempRoom.name = "Reserve"
            allZoneImperi.rooms.Add(tempRoom)

            For Each zones In zoneName
                If Not devZone.Keys.Contains(zones.Key) Then
                    Dim tempdevzone As List(Of String)
                    tempdevzone = myService.GetDeviceInZone(IdSrv, zones.Key)
                    tempdevzone.AddRange(myService.GetMacroInZone(IdSrv, zones.Key))
                    devZone.Add(zones.Key, tempdevzone)
                End If
            Next

            'device
            Dim importOk = ImportImperiHomeDevice()

            If allDevImperi Is Nothing Then
                allDevImperi = New DeviceList
                allDevImperi.devices = New List(Of Device)
            End If

            For Each device In allDevice

                Dim x As New CheckBox
                Dim z As New Button

                x.Content = device.Name
                x.ToolTip = device.ID
                x.HorizontalAlignment = HorizontalAlignment.Left
                x.IsChecked = False
                x.Width = 270
                x.Height = 20
                AddHandler x.Checked, AddressOf ChkElement_Click
                AddHandler x.Unchecked, AddressOf ChkElement_Click

                z.Content = "Paramètres"
                z.HorizontalAlignment = HorizontalAlignment.Right
                z.Width = 65
                x.Height = 20
                AddHandler z.Click, AddressOf BtnElement_Click

                Dim stk As New StackPanel
                stk.Orientation = Orientation.Horizontal
                stk.HorizontalAlignment = HorizontalAlignment.Left

                If importOk Then
                    For Each dev In allDevImperi.devices
                        If dev.name = device.Name Then
                            x.IsChecked = True
                            stk.Name = dev.id
                        End If
                    Next
                End If

                If x.IsChecked = True Then
                    z.Visibility = Windows.Visibility.Visible
                Else
                    z.Visibility = Windows.Visibility.Collapsed
                End If

                stk.Children.Add(x)
                stk.Children.Add(z)
                ListBoxDevices.Items.Add(stk)
                x = Nothing
                z = Nothing
            Next

            'macro

            For Each macro In allMacro
                Dim x As New CheckBox
                x.Content = macro.Nom
                x.ToolTip = macro.ID
                x.HorizontalAlignment = HorizontalAlignment.Left
                AddHandler x.Checked, AddressOf ChkElement_Click
                AddHandler x.Unchecked, AddressOf ChkElement_Click
                x.Height = 20
                x.IsChecked = False

                Dim stk As New StackPanel
                stk.Orientation = Orientation.Horizontal
                stk.HorizontalAlignment = HorizontalAlignment.Left

                If importOk Then
                    For Each dev In allDevImperi.devices
                        If dev.name = macro.Nom Then
                            x.IsChecked = True
                            stk.Name = dev.id
                        End If
                    Next
                End If

                stk.Children.Add(x)
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

    Private Function ImportImperiHomeZone() As Boolean
        Try
            
            Dim fileName1 = My.Application.Info.DirectoryPath & "\Drivers\Imperihome\rooms.json"

            If System.IO.File.Exists(fileName1) Then
                Dim stream = System.IO.File.ReadAllText(fileName1)
                allZoneImperi = Newtonsoft.Json.JsonConvert.DeserializeObject(stream, GetType(RoomList))
            Else
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.INFO, "Le fichier " & fileName1 & " n'existe pas, il va etre créé", "INFO", "ImportImperiHome")
                Return False
                Exit Function
            End If

            Return True
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR ImportImperiHome: " & ex.ToString, "ERREUR", "ImportImperiHome")
            Return Nothing
        End Try
    End Function

    Private Function ImportImperiHomeDevice() As Boolean
        Try
            Dim fileName = My.Application.Info.DirectoryPath & "\Drivers\Imperihome\devices.json"

            If System.IO.File.Exists(fileName) Then
                Dim stream = System.IO.File.ReadAllText(fileName)
                allDevImperi = Newtonsoft.Json.JsonConvert.DeserializeObject(stream, GetType(DeviceList))
            Else
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.INFO, "Le fichier " & fileName & " n'existe pas, il va etre créé", "INFO", "ImportImperiHome")
                Return False
                Exit Function
            End If
            
            MajAllDevice()
            Return True
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR ImportImperiHome: " & ex.ToString, "ERREUR", "ImportImperiHome")
            Return Nothing
        End Try
    End Function

    Private Function ExportImperiHome() As Boolean
        Try

            Dim BasePath As System.IO.DirectoryInfo = New System.IO.DirectoryInfo(My.Application.Info.DirectoryPath & "\Drivers\")
            Dim NewPath As System.IO.DirectoryInfo = New System.IO.DirectoryInfo("Imperihome")
            Dim DitPath() As System.IO.DirectoryInfo
            DitPath = BasePath.GetDirectories
            If Not DitPath.Contains(NewPath) Then
                BasePath.CreateSubdirectory("Imperihome")
            End If


            Dim find = New List(Of Device)
            Dim memdev As Device = Nothing

            For Each devImperi In allDevImperi.devices
                For Each dev In allDevice
                    memdev = devImperi
                    If dev.Name = devImperi.name Then
                        memdev = Nothing
                        Exit For
                    End If
                Next
                If memdev IsNot Nothing Then
                    For Each dev In allMacro
                        memdev = devImperi
                        If dev.Nom = devImperi.name Then
                            memdev = Nothing
                            Exit For
                        End If
                    Next
                End If
                If memdev IsNot Nothing Then find.Add(memdev)
            Next

            If find.Count > 0 Then
                For Each devfind In find
                    allDevImperi.devices.Remove(devfind)
                Next
            End If

            Dim stream As String = ""

            stream = Newtonsoft.Json.JsonConvert.SerializeObject(allDevImperi)
            System.IO.File.WriteAllText(My.Application.Info.DirectoryPath & "\Drivers\Imperihome\devices.json", stream)

            stream = Newtonsoft.Json.JsonConvert.SerializeObject(allZoneImperi)
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

    Private Function MajDevice(dev As StackPanel) As Boolean
        Try

            Dim memId As Integer = 0

            Dim x As CheckBox = dev.Children.Item(0)
            Dim devImperiExist As Boolean = False
            Dim devGraphExist As Boolean = False

            If dev.Name <> "" Then devGraphExist = True

            Dim devImperiTemp As Device = New Device
                For Each devimperi In allDevImperi.devices
                    If devGraphExist Then
                        If devimperi.id = "DEV" & Right(dev.Name, 3) Then
                            devImperiTemp = devimperi
                            devImperiExist = True
                        End If
                    End If
                    If CInt(Right(devimperi.id, 3)) > memId Then memId = CInt(Right(devimperi.id, 3))
                Next

            If x.IsChecked Then
                If dev.Children.Count > 1 Then dev.Children.Item(1).Visibility = Windows.Visibility.Visible
                If Not devGraphExist Then dev.Name = "DEV" & Format(memId + 1, "000")

                If Not devImperiExist Then

                    devImperiTemp.id = dev.Name
                    Dim comp = ReturnDeviceByID(x.ToolTip)
                    If comp.ID = x.ToolTip Then
                        'device 
                        devImperiTemp.name = comp.Name
                        devImperiTemp.room = searchZone(comp.ID)
                        devImperiTemp.type = typeDevice(comp.Type)
                        devImperiTemp.params = ParamByType(comp, devImperiTemp.type)
                    Else
                        'macro
                        Dim macr = myService.ReturnMacroById(IdSrv, x.ToolTip)
                        If macr.ID = x.ToolTip Then
                            devImperiTemp.name = macr.Nom
                            devImperiTemp.room = searchZone(macr.ID)
                            devImperiTemp.type = "DevScene"
                            Dim params As DeviceParam = New DeviceParam
                            params.key = "LastRun"
                            params.value = Now
                            devImperiTemp.params = New List(Of DeviceParam)
                            devImperiTemp.params.Add(params)
                        End If
                    End If

                    allDevImperi.devices.Add(devImperiTemp)

                End If
            Else
                If dev.Children.Count > 1 Then dev.Children.Item(1).Visibility = Windows.Visibility.Collapsed
                If devImperiExist Then allDevImperi.devices.Remove(devImperiTemp)
            End If

        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR ExportImperiHome: " & ex.ToString, "ERREUR", "ExportImperiHome")
            Return Nothing
        End Try
    End Function

    Private Sub ChkElement_Click(ByVal sender As CheckBox, ByVal e As System.Windows.RoutedEventArgs)
        Try
            Dim x As StackPanel = sender.Parent
            If sender.Parent IsNot Nothing Then
                MajDevice(x)
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uSelectExp ChkElement_Click: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Sub BtnElement_Click(ByVal sender As Button, ByVal e As System.Windows.RoutedEventArgs)
        Try

            Dim devImperiTemp As Device = New Device
            Dim x As StackPanel = sender.Parent
            If sender.Parent IsNot Nothing Then
                Dim devImperiExist As Boolean = False
                For Each devimperi In allDevImperi.devices
                    If devimperi.id = x.Name Then
                        devImperiTemp = devimperi
                        devImperiExist = True
                        Exit For
                    End If
                Next
                If devImperiExist Then
                    Dim chk As CheckBox = x.Children.Item(0)
                    Dim frm As New WImperiParametrage(devImperiTemp, chk.ToolTip)
                    frm.ShowDialog()
                    If frm.DialogResult.HasValue And frm.DialogResult.Value Then
                        allDevImperi.devices.Remove(devImperiTemp)
                        devImperiTemp = frm.ObjDevice
                        allDevImperi.devices.Add(devImperiTemp)
                        frm.Close()
                    End If
                Else
                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uSelectExp BtnOK_Click: Pas de device à executer ")
                End If

            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uSelectExp BtnOK_Click: " & ex.Message, "ERREUR", "")
        End Try
    End Sub
    Private Sub MajAllDevice()
        Try
            For Each devimperiTemp In allDevImperi.devices
                Dim comp = ReturnDeviceByID(devimperiTemp.params(0).value)
                If comp.ID = devimperiTemp.params(0).value Then
                    'device 
                    devimperiTemp.name = comp.Name
                    devimperiTemp.room = searchZone(comp.ID)
                End If
            Next
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR MajAllDevice: " & ex.ToString, "ERREUR", "MajAllDevice")

        End Try
    End Sub
    Private Function searchZone(ByVal idDevice As String) As String
        Try
            Dim idzone As String = ""
            Dim memIdZone As Integer = 0

            For Each zones In zoneName
                For Each iddev In devZone(zones.Key)
                    If iddev = idDevice Then
                        idzone = zones.Key
                        Exit For
                    End If
                Next
                If idzone = zones.Key And zones.Key <> "" Then
                    Exit For
                End If
            Next

            Dim tempRoom As Room = New Room
            Dim idfind As String = ""

            For Each room As Room In allZoneImperi.rooms
                If room.name = zoneName(idzone) Then
                    idfind = room.id
                End If
                If CInt(Right(room.id, 3)) > memIdZone And CInt(Right(room.id, 3)) < 999 Then memIdZone = CInt(Right(room.id, 3))
            Next
            If idfind = "" Then
                memIdZone += 1
                tempRoom.id = "ROOM" & Format(memIdZone, "000")
                tempRoom.name = zoneName(idzone)
                allZoneImperi.rooms.Add(tempRoom)
                idfind = tempRoom.id
            End If

            Return idfind
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR searchZone: " & ex.ToString, "ERREUR", "searchZone")
            Return ""
        End Try
    End Function

    Public Shared Function ParamByType(ByVal comp As HoMIDom.HoMIDom.TemplateDevice, type As String) As List(Of DeviceParam)
        Try

            Dim params As List(Of DeviceParam) = New List(Of DeviceParam)

            Select Case type

                Case "DevCO2", "DevGenericSensor", "DevHygrometry", "DevLuminosity", "DevNoise", "DevPressure", "DevTemperature", "DevUV"
                    params.Add(New DeviceParam)
                    params(0).key = "Value"
                    params(0).value = comp.ID
                    params(0).unit = comp.Unit
                    If type <> "DevCO2" Then params(0).graphable = comp.IsHisto

                Case "DevDimmer"

                    params.Add(New DeviceParam)
                    params(0).key = "Level"
                    params(0).value = comp.ID
                    params(0).unit = comp.Unit

                    params.Add(New DeviceParam)
                    params(1).key = "Status"
                    params(1).value = comp.ID

                    params.Add(New DeviceParam)
                    params(2).key = "Energy"
                    params(2).value = ""
                    params(2).unit = comp.Unit

                Case "DevShutter"

                    params.Add(New DeviceParam)
                    params(0).key = "Level"
                    params(0).value = comp.ID
                    params(0).unit = comp.Unit

                    params.Add(New DeviceParam)
                    params(1).key = "stopable"
                    params(1).value = 1

                    params.Add(New DeviceParam)
                    params(2).key = "pulseable"
                    params(2).value = 1

                Case "DevDoor", "DevFlood", "DevMotion", "DevSmoke", "DevCO2Alert "

                    params.Add(New DeviceParam)
                    params(0).key = "Tripped"
                    params(0).value = comp.ID

                    params.Add(New DeviceParam)
                    params(1).key = "Armed"
                    params(1).value = 1

                    params.Add(New DeviceParam)
                    params(2).key = "Armable"
                    params(2).value = 1

                    params.Add(New DeviceParam)
                    params(3).key = "Ackable"
                    params(3).value = 1

                    params.Add(New DeviceParam)
                    params(4).key = "lasttrip"
                    params(4).value = comp.ID

                Case "DevElectricity"

                    params.Add(New DeviceParam)
                    params(0).key = "Watts"
                    params(0).value = comp.ID
                    params(0).unit = comp.Unit
                    params(0).graphable = comp.IsHisto

                    params.Add(New DeviceParam)
                    params(1).key = "ConsoTotal"
                    params(1).value = ""
                    params(1).unit = comp.Unit
                    params(1).graphable = comp.IsHisto

                Case "DevLock"

                    params.Add(New DeviceParam)
                    params(0).key = "Status"
                    params(0).value = comp.ID

                Case "DevMultiSwitch"

                    params.Add(New DeviceParam)
                    params(0).key = "Value"
                    params(0).value = comp.ID

                    params.Add(New DeviceParam)
                    params(1).key = "Choises"
                    params(1).value = comp.ID

                Case "DevRain"

                    params.Add(New DeviceParam)
                    params(0).key = "Value"
                    params(0).value = comp.ID
                    params(0).unit = comp.Unit
                    params(0).graphable = comp.IsHisto

                    params.Add(New DeviceParam)
                    params(1).key = "Accumulation"
                    params(1).value = ""
                    params(1).unit = comp.Unit
                    params(1).graphable = comp.IsHisto

                Case "DevRGBLight"

                    params.Add(New DeviceParam)
                    params(0).key = "Level"
                    params(0).value = comp.ID

                    params.Add(New DeviceParam)
                    params(1).key = "color"
                    params(1).value = comp.ID

                    params.Add(New DeviceParam)
                    params(2).key = "dimmable"
                    params(2).value = 1

                    params.Add(New DeviceParam)
                    params(3).key = "whitechannel"
                    params(3).value = comp.ID

                    params.Add(New DeviceParam)
                    params(4).key = "Energy"
                    params(4).value = comp.ID
                    params(4).unit = comp.Unit

                    params.Add(New DeviceParam)
                    params(5).key = "Status"
                    params(5).value = comp.ID

                    'Case "DevScene"

                    'params(0) = New DeviceParam
                    'params(0).key = "LastRun"
                    'params(0).value = comp.LastChange.Ticks

                Case "DevSwitch"
                    params.Add(New DeviceParam)
                    params(0).key = "Status"
                    params(0).value = comp.ID

                    params.Add(New DeviceParam)
                    params(1).key = "Energy"
                    params(1).value = comp.ID
                    params(1).unit = comp.Unit

                    params.Add(New DeviceParam)
                    params(2).key = "pulseable"
                    params(2).value = 1

                Case "DevTempHygro"
                    params.Add(New DeviceParam)
                    params(0).key = "temp"
                    params(0).value = comp.ID
                    params(1).unit = comp.Unit
                    params(1).graphable = comp.IsHisto

                    params.Add(New DeviceParam)
                    params(1).key = "hygro"
                    params(1).value = comp.ID
                    params(1).unit = comp.Unit
                    params(1).graphable = comp.IsHisto

                Case "DevThermostat"
                    params.Add(New DeviceParam)
                    params(0).key = "cursetpoint"
                    params(0).value = comp.ID

                    params.Add(New DeviceParam)
                    params(1).key = "curtemp"
                    params(1).value = comp.ID
                    params(1).unit = comp.Unit

                    params.Add(New DeviceParam)
                    params(2).key = "curmode"
                    params(2).value = comp.ID
                           
                    params.Add(New DeviceParam)
                    params(3).key = "availablemodes"
                    params(3).value = comp.ID

                Case "DevWind"
                    params.Add(New DeviceParam)
                    params(0).key = "Speed"
                    params(0).value = comp.ID
                    params(0).unit = comp.Unit
                    params(0).graphable = comp.IsHisto

                    params.Add(New DeviceParam)
                    params(1).key = "Direction"
                    params(1).value = comp.ID
                    params(1).unit = comp.Unit

            End Select

            Return params
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
                'Next
            Next
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Chkselectmacros_CheckedUnchecked: " & ex.ToString, "ERREUR", "Chkselectmacros_CheckedUnchecked")
        End Try
    End Sub

End Class

''' <summary>Class DeviceParam, Défini le type parametre de composant pour le client Imperihome</summary>
<Serializable()> Public Class DeviceParam

    Public Property key As String = ""
    Public Property value As String = ""
    Public Property unit As String = ""
    Public Property graphable As Boolean = False

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
