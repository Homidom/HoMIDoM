Imports HoMIDom.HoMIDom

Public Class WImperiParametrage
    Dim ListeDevices As List(Of TemplateDevice)

    Dim memParam0 As DeviceParam = Nothing
    Dim _ObjDevice As Device = Nothing

    Dim Comp As TemplateDevice = New TemplateDevice
    Public Property ObjDevice As Device
        Get
            Return _ObjDevice
        End Get
        Set(ByVal value As Device)
            _ObjDevice = value
        End Set
    End Property

    Public Sub New(ByVal ObjDevice As Device, ByVal IdComp As String)

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        Try
            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            _ObjDevice = ObjDevice

            If _ObjDevice IsNot Nothing Then

                ListeDevices = myService.GetAllDevices(IdSrv)
                Comp = ReturnDeviceByID(IdComp)

                Dim TDevAucun As TemplateDevice = New TemplateDevice
                Dim TDevOn As TemplateDevice = New TemplateDevice
                Dim TDevOff As TemplateDevice = New TemplateDevice
                TDevAucun.Name = "Aucun"
                TDevOn.Name = "On"
                TDevOn.ID = "1"
                TDevOff.Name = "Off"
                TDevOff.ID = "0"

                ListeDevices.Add(TDevAucun)
                ListeDevices.Add(TDevOn)
                ListeDevices.Add(TDevOff)


                Dim w As New Label
                w.Content = "Composant : " & _ObjDevice.name
                w.HorizontalAlignment = HorizontalAlignment.Left
                w.Foreground = New SolidColorBrush(Colors.White)
                w.Width = 400
                w.Height = 25
                StkLb1.Children.Add(w)

                Dim x As New Label
                x.Content = "Type du composant"
                x.HorizontalAlignment = HorizontalAlignment.Left
                x.Foreground = New SolidColorBrush(Colors.White)
                x.Width = 250
                x.Height = 25
                StkLb2.Children.Add(x)

                Dim y As New ComboBox
                y.HorizontalAlignment = HorizontalAlignment.Right
                y.Width = 200
                y.Height = 25
                y.Items.Add("Camera")
                y.Items.Add("CO2")
                y.Items.Add("CO2Alert")
                y.Items.Add("Dimmer")
                y.Items.Add("Door")
                y.Items.Add("Electricity")
                y.Items.Add("Flood")
                y.Items.Add("GenericSensor")
                y.Items.Add("Hygrometry")
                y.Items.Add("Lock")
                y.Items.Add("Luminosity")
                y.Items.Add("Motion")
                y.Items.Add("MultiSwitch")
                y.Items.Add("Noise")
                y.Items.Add("Pressure")
                y.Items.Add("Rain")
                y.Items.Add("RGBLight")
                y.Items.Add("Scene")
                y.Items.Add("Shutter")
                y.Items.Add("Smoke")
                y.Items.Add("Switch")
                y.Items.Add("Temperature")
                y.Items.Add("TempHygro")
                y.Items.Add("Thermostat")
                y.Items.Add("UV")
                y.Items.Add("Wind")

                y.HorizontalAlignment = HorizontalAlignment.Right
                y.Text = Right(_ObjDevice.type, _ObjDevice.type.Length - 3)

                AddHandler y.SelectionChanged, AddressOf CbxDev_Change

                StkCB2.Children.Add(y)
                y = Nothing

                Init()


            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur New: " & ex.ToString, "Erreur Admin", "")
        End Try
    End Sub
    Private Sub CbxDev_Change(ByVal sender As ComboBox, ByVal e As System.Windows.RoutedEventArgs)
        Try
            If sender.SelectedItem <> _ObjDevice.type Then
                _ObjDevice.params = uSelectExp.ParamByType(Comp, "Dev" & sender.SelectedItem)
                _ObjDevice.type = "Dev" & sender.SelectedItem
                Init()
            End If

        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur Wimperi CbxDev_Change: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub
    Private Sub CbxType_Change(ByVal sender As ComboBox, ByVal e As System.Windows.RoutedEventArgs)
        Try
            
            If memParam0.key <> sender.SelectedItem.key Then Init(sender)

        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur Wimperi CbxType_Change: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Sub CbxParam_Change(ByVal sender As ComboBox, ByVal e As System.Windows.RoutedEventArgs)
        Try

            Dim stk As StackPanel = sender.Parent
            For i = 1 To stk.Children.Count - 1
                Dim cb As ComboBox = stk.Children.Item(i)
                _ObjDevice.params(i).value = cb.SelectedItem.id
            Next

        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur Wimperi CbxParam_Change: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Sub ChkElement_Click(ByVal sender As CheckBox, ByVal e As System.Windows.RoutedEventArgs)
        Try

            Dim stk As StackPanel = sender.Parent
            For i = 0 To stk.Children.Count - 1
                Dim ch As CheckBox = stk.Children.Item(i)
                _ObjDevice.params(i).graphable = ch.IsChecked
            Next

        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur Wimperi CbxParam_Change: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Sub Init(Optional ByVal sender As ComboBox = Nothing)
        Try

            If sender IsNot Nothing Then
                memParam0 = sender.SelectedItem
            Else
                memParam0 = _ObjDevice.params(0)
            End If
            StkLb.Children.Clear()
            StkCB.Children.Clear()
            StkCH.Children.Clear()

            Dim Lbl1 As New Label
            Dim Cb1 As New ComboBox
            Dim Ch1 As New CheckBox

            Lbl1.Content = "Paramètre du composant"
            Lbl1.HorizontalAlignment = HorizontalAlignment.Left
            Lbl1.Foreground = New SolidColorBrush(Colors.White)
            Lbl1.Width = 250
            Lbl1.Height = 25

            Cb1.HorizontalAlignment = HorizontalAlignment.Right
            Cb1.Width = 200
            Cb1.Height = 25
            AddHandler Cb1.SelectionChanged, AddressOf CbxType_Change

            Cb1.ItemsSource = _ObjDevice.params
            Cb1.DisplayMemberPath = "key"
            Cb1.Text = memParam0.key

            Ch1.Content = "Histo"
            Ch1.HorizontalAlignment = HorizontalAlignment.Left
            Ch1.Foreground = New SolidColorBrush(Colors.White)
            Ch1.Width = 100
            Ch1.Height = 25
            
            Select Case memParam0.key
                Case "Speed", "Direction", "Value", "hygro", "temp", "Accumulation", "Watts", "ConsoTotal"
                    Ch1.IsEnabled = True
                    If _ObjDevice.type <> "DevMultiSwitch" Then
                        If memParam0.graphable Then
                            Ch1.IsChecked = True
                        Else
                            Ch1.IsChecked = False
                        End If
                    Else
                        Ch1.IsChecked = False
                        Ch1.IsEnabled = False
                    End If
                Case Else
                    Ch1.IsChecked = False
                    Ch1.IsEnabled = False
            End Select
            AddHandler Ch1.Checked, AddressOf ChkElement_Click
            AddHandler Ch1.Unchecked, AddressOf ChkElement_Click

            StkLb.Children.Add(Lbl1)
            StkCB.Children.Add(Cb1)
            StkCH.Children.Add(Ch1)
            Lbl1 = Nothing
            Cb1 = Nothing
            Ch1 = Nothing

            Dim paramTemp As DeviceParam = New DeviceParam
            Dim paramsTemp As List(Of DeviceParam) = New List(Of DeviceParam)
            paramsTemp.Add(paramTemp)

            For Each param In _ObjDevice.params
                Dim x As New Label
                Dim y As New ComboBox
                Dim z As New CheckBox
                If Not memParam0.key = param.key Then
                    paramsTemp.Add(param)
                    x.Content = "Composant du paramètre " & param.key
                    x.HorizontalAlignment = HorizontalAlignment.Left
                    x.Foreground = New SolidColorBrush(Colors.White)
                    x.Width = 250
                    x.Height = 25
                    y.HorizontalAlignment = HorizontalAlignment.Right
                    y.Width = 200
                    y.Height = 25
                    y.ItemsSource = ListeDevices
                    y.DisplayMemberPath = "Name"
                    z.Content = "Histo"
                    z.HorizontalAlignment = HorizontalAlignment.Left
                    z.Foreground = New SolidColorBrush(Colors.White)
                    z.Width = 100
                    z.Height = 25
                    For i As Integer = 0 To y.Items.Count - 1
                        y.Text = param.value
                        If param.value = y.Items(i).Id Then
                            y.SelectedIndex = i
                            Exit For
                        End If
                    Next
                    AddHandler y.SelectionChanged, AddressOf CbxParam_Change

                    Select Case param.key
                        Case "Speed", "Direction", "Value", "hygro", "temp", "Accumulation", "Watts", "ConsoTotal"
                            z.IsEnabled = True
                            If _ObjDevice.type <> "DevMultiSwitch" Then
                                If param.graphable Then
                                    z.IsChecked = True
                                Else
                                    z.IsChecked = False
                                End If
                            Else
                                z.IsChecked = False
                                z.IsEnabled = False
                            End If
                        Case Else
                            z.IsChecked = False
                            z.IsEnabled = False
                    End Select
                    AddHandler z.Checked, AddressOf ChkElement_Click
                    AddHandler z.Unchecked, AddressOf ChkElement_Click

                    StkLb.Children.Add(x)
                    StkCB.Children.Add(y)
                    StkCH.Children.Add(z)
                    x = Nothing
                    y = Nothing
                    z = Nothing

                Else
                    paramsTemp(0) = param
                End If
            Next
            _ObjDevice.params = paramsTemp


        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur Wimperi Init: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOk.Click
        Try

            DialogResult = True

        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur Click_Ok: " & ex.ToString, "Erreur Admin", "")
        End Try
    End Sub

End Class
