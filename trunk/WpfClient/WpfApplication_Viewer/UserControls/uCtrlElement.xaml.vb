Imports System.Windows.Threading

Partial Public Class uCtrlElement

    Dim _Id As String
    Dim _Down As DateTime
    Dim _dev As HoMIDom.HoMIDom.TemplateDevice = Nothing
    Dim _macro As HoMIDom.HoMIDom.Macro = Nothing
    Dim _zone As HoMIDom.HoMIDom.Zone = Nothing

    Public Event click(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
    Public Event ShowZone(ByVal zoneid As String)

    Public Property Id As String
        Get
            Return _Id
        End Get
        Set(ByVal value As String)
            _Id = value
            If IsConnect = True Then
                _dev = myService.ReturnDeviceByID(IdSrv, _Id)
                _macro = myService.ReturnMacroById(IdSrv, _Id)
                _zone = myService.ReturnZoneByID(IdSrv, _Id)

                If _dev IsNot Nothing Then
                    Lbl.Content = _dev.Name
                    Image.Source = ConvertArrayToImage(myService.GetByteFromImage(_dev.Picture))

                    Select Case _dev.Type
                        Case HoMIDom.HoMIDom.Device.ListeDevices.APPAREIL
                            LblStatus.Content = "Status: " & _dev.Value
                            Dim x As New uOnOff
                            x.ContentOn = "ON"
                            x.ContentOff = "OFF"
                            AddHandler x.ClickOn, AddressOf ClickOn
                            AddHandler x.ClickOff, AddressOf ClickOff
                            StkPopup.Children.Add(x)
                        Case HoMIDom.HoMIDom.Device.ListeDevices.AUDIO
                            LblStatus.Content = "Status: " & _dev.Value
                        Case HoMIDom.HoMIDom.Device.ListeDevices.BAROMETRE
                            LblStatus.Content = "Status: " & _dev.Value
                        Case HoMIDom.HoMIDom.Device.ListeDevices.BATTERIE
                            LblStatus.Content = "Status: " & _dev.Value
                        Case HoMIDom.HoMIDom.Device.ListeDevices.COMPTEUR
                            LblStatus.Content = "Status: " & _dev.Value
                        Case HoMIDom.HoMIDom.Device.ListeDevices.CONTACT
                            LblStatus.Content = "Status: " & _dev.Value
                        Case HoMIDom.HoMIDom.Device.ListeDevices.DETECTEUR
                            LblStatus.Content = "Status: " & _dev.Value
                        Case HoMIDom.HoMIDom.Device.ListeDevices.DIRECTIONVENT
                            LblStatus.Content = "Status: " & _dev.Value
                        Case HoMIDom.HoMIDom.Device.ListeDevices.ENERGIEINSTANTANEE
                            LblStatus.Content = "Status: " & _dev.Value
                        Case HoMIDom.HoMIDom.Device.ListeDevices.ENERGIETOTALE
                            LblStatus.Content = "Status: " & _dev.Value
                        Case HoMIDom.HoMIDom.Device.ListeDevices.GENERIQUESTRING
                            LblStatus.Content = "Status: " & _dev.Value
                        Case HoMIDom.HoMIDom.Device.ListeDevices.GENERIQUEBOOLEEN
                            LblStatus.Content = "Status: " & _dev.Value
                        Case HoMIDom.HoMIDom.Device.ListeDevices.GENERIQUEVALUE
                            LblStatus.Content = "Status: " & _dev.Value
                        Case HoMIDom.HoMIDom.Device.ListeDevices.HUMIDITE
                            LblStatus.Content = "Status: " & _dev.Value & "%"
                        Case HoMIDom.HoMIDom.Device.ListeDevices.LAMPE
                            LblStatus.Content = "Status: " & _dev.Value & "%"
                            Dim x As New uOnOff
                            x.ContentOn = "ON"
                            x.ContentOff = "OFF"
                            AddHandler x.ClickOn, AddressOf ClickOn
                            AddHandler x.ClickOff, AddressOf ClickOff
                            StkPopup.Children.Add(x)
                            Dim x2 As New uVariateur
                            AddHandler x2.ValueChange, AddressOf ValueChange
                            StkPopup.Children.Add(x2)
                        Case HoMIDom.HoMIDom.Device.ListeDevices.METEO
                            Me.Visibility = Windows.Visibility.Hidden
                        Case HoMIDom.HoMIDom.Device.ListeDevices.MULTIMEDIA
                        Case HoMIDom.HoMIDom.Device.ListeDevices.PLUIECOURANT
                            LblStatus.Content = "Status: " & _dev.Value
                        Case HoMIDom.HoMIDom.Device.ListeDevices.PLUIETOTAL
                            LblStatus.Content = "Status: " & _dev.Value
                        Case HoMIDom.HoMIDom.Device.ListeDevices.SWITCH
                            LblStatus.Content = "Status: " & _dev.Value
                            Dim x As New uOnOff
                            x.ContentOn = "ON"
                            x.ContentOff = "OFF"
                            AddHandler x.ClickOn, AddressOf ClickOn
                            AddHandler x.ClickOff, AddressOf ClickOff
                            StkPopup.Children.Add(x)
                        Case HoMIDom.HoMIDom.Device.ListeDevices.TELECOMMANDE
                            LblStatus.Content = "Status: " & _dev.Value
                        Case HoMIDom.HoMIDom.Device.ListeDevices.TEMPERATURE
                            LblStatus.Content = "Status: " & _dev.Value & "°C"
                        Case HoMIDom.HoMIDom.Device.ListeDevices.TEMPERATURECONSIGNE
                            LblStatus.Content = "Status: " & _dev.Value & "°C"
                        Case HoMIDom.HoMIDom.Device.ListeDevices.VITESSEVENT
                            LblStatus.Content = "Status: " & _dev.Value
                        Case HoMIDom.HoMIDom.Device.ListeDevices.VOLET
                            LblStatus.Content = "Status: " & _dev.Value & "%"
                            Dim x As New uOnOff
                            x.ContentOn = "OUVRIR"
                            x.ContentOff = "FERMER"
                            AddHandler x.ClickOn, AddressOf ClickOn
                            AddHandler x.ClickOff, AddressOf ClickOff
                            StkPopup.Children.Add(x)
                            Dim x2 As New uVariateur
                            AddHandler x2.ValueChange, AddressOf ValueChange
                            StkPopup.Children.Add(x2)
                        Case HoMIDom.HoMIDom.Device.ListeDevices.UV
                            LblStatus.Content = "Status: " & _dev.Value
                    End Select

                    

                End If
                If _macro IsNot Nothing Then
                    Lbl.Content = _macro.Nom
                    LblStatus.Visibility = Windows.Visibility.Hidden
                    LblStatus.Width = 0
                    LblStatus.Height = 0
                End If
                If _zone IsNot Nothing Then
                    Lbl.Content = _zone.Name
                    Image.Source = ConvertArrayToImage(myService.GetByteFromImage(_zone.Icon))
                    LblStatus.Visibility = Windows.Visibility.Hidden
                    LblStatus.Width = 0
                    LblStatus.Height = 0
                End If
            End If

            Refresh()
        End Set
    End Property

    Public Sub New()

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        Dim dt As DispatcherTimer = New DispatcherTimer()
        AddHandler dt.Tick, AddressOf dispatcherTimer_Tick
        dt.Interval = New TimeSpan(0, 0, 1)
        dt.Start()
    End Sub

    Private Sub Refresh()
        Try
            If _Id = "" Then Exit Sub

            If IsConnect = True Then
                _dev = myService.ReturnDeviceByID(IdSrv, _Id)
            Else
                Exit Sub
            End If

            If _dev IsNot Nothing Then
                Select Case _dev.Type
                    Case HoMIDom.HoMIDom.Device.ListeDevices.APPAREIL
                        LblStatus.Content = "Status: " & _dev.Value
                    Case HoMIDom.HoMIDom.Device.ListeDevices.AUDIO
                        LblStatus.Content = "Status: " & _dev.Value
                    Case HoMIDom.HoMIDom.Device.ListeDevices.BAROMETRE
                        LblStatus.Content = "Status: " & _dev.Value
                    Case HoMIDom.HoMIDom.Device.ListeDevices.BATTERIE
                        LblStatus.Content = "Status: " & _dev.Value
                    Case HoMIDom.HoMIDom.Device.ListeDevices.COMPTEUR
                        LblStatus.Content = "Status: " & _dev.Value
                    Case HoMIDom.HoMIDom.Device.ListeDevices.CONTACT
                        LblStatus.Content = "Status: " & _dev.Value
                    Case HoMIDom.HoMIDom.Device.ListeDevices.DETECTEUR
                        LblStatus.Content = "Status: " & _dev.Value
                    Case HoMIDom.HoMIDom.Device.ListeDevices.DIRECTIONVENT
                        LblStatus.Content = "Status: " & _dev.Value
                    Case HoMIDom.HoMIDom.Device.ListeDevices.ENERGIEINSTANTANEE
                        LblStatus.Content = "Status: " & _dev.Value
                    Case HoMIDom.HoMIDom.Device.ListeDevices.ENERGIETOTALE
                        LblStatus.Content = "Status: " & _dev.Value
                    Case HoMIDom.HoMIDom.Device.ListeDevices.GENERIQUESTRING
                        LblStatus.Content = "Status: " & _dev.Value
                    Case HoMIDom.HoMIDom.Device.ListeDevices.GENERIQUEBOOLEEN
                        LblStatus.Content = "Status: " & _dev.Value
                    Case HoMIDom.HoMIDom.Device.ListeDevices.GENERIQUEVALUE
                        LblStatus.Content = "Status: " & _dev.Value
                    Case HoMIDom.HoMIDom.Device.ListeDevices.HUMIDITE
                        LblStatus.Content = "Status: " & _dev.Value & "%"
                    Case HoMIDom.HoMIDom.Device.ListeDevices.LAMPE
                        LblStatus.Content = "Status: " & _dev.Value & "%"
                    Case HoMIDom.HoMIDom.Device.ListeDevices.METEO

                    Case HoMIDom.HoMIDom.Device.ListeDevices.MULTIMEDIA
                    Case HoMIDom.HoMIDom.Device.ListeDevices.PLUIECOURANT
                        LblStatus.Content = "Status: " & _dev.Value
                    Case HoMIDom.HoMIDom.Device.ListeDevices.PLUIETOTAL
                        LblStatus.Content = "Status: " & _dev.Value
                    Case HoMIDom.HoMIDom.Device.ListeDevices.SWITCH
                        LblStatus.Content = "Status: " & _dev.Value
                    Case HoMIDom.HoMIDom.Device.ListeDevices.TELECOMMANDE
                        LblStatus.Content = "Status: " & _dev.Value
                    Case HoMIDom.HoMIDom.Device.ListeDevices.TEMPERATURE
                        LblStatus.Content = "Status: " & _dev.Value & "°C"
                    Case HoMIDom.HoMIDom.Device.ListeDevices.TEMPERATURECONSIGNE
                        LblStatus.Content = "Status: " & _dev.Value & "°C"
                    Case HoMIDom.HoMIDom.Device.ListeDevices.VITESSEVENT
                        LblStatus.Content = "Status: " & _dev.Value
                    Case HoMIDom.HoMIDom.Device.ListeDevices.VOLET
                        LblStatus.Content = "Status: " & _dev.Value & "%"
                    Case HoMIDom.HoMIDom.Device.ListeDevices.UV
                        LblStatus.Content = "Status: " & _dev.Value
                End Select
                _dev = Nothing
            End If
        Catch ex As Exception
            MsgBox("Error Refresh: " & ex.ToString & vbCrLf)
        End Try
    End Sub

    Public Sub dispatcherTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        Refresh()
    End Sub


    Private Sub Image_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Image.MouseLeftButtonUp
        Dim vDiff As TimeSpan = Now - _Down
        If vDiff.Seconds < 1 Then RaiseEvent click(Me, e)
    End Sub

    Private Sub Image_PreviewMouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Image.PreviewMouseDown
        _Down = Now
    End Sub

    Private Sub ClosePopup()
        Popup1.IsOpen = False
    End Sub

    Private Sub ClickOn(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim x As New HoMIDom.HoMIDom.DeviceAction
        If _dev.Type <> HoMIDom.HoMIDom.Device.ListeDevices.VOLET Then
            x.Nom = "ON"
        Else
            x.Nom = "OPEN"
        End If
        x.Parametres = Nothing
        myService.ExecuteDeviceCommand(IdSrv, Id, x)
    End Sub

    Private Sub ClickOff(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim x As New HoMIDom.HoMIDom.DeviceAction
        If _dev.Type <> HoMIDom.HoMIDom.Device.ListeDevices.VOLET Then
            x.Nom = "OFF"
        Else
            x.Nom = "CLOSE"
        End If
        x.Parametres = Nothing
        myService.ExecuteDeviceCommand(IdSrv, Id, x)
    End Sub

    Private Sub ValueChange(ByVal Value As Integer)
        Dim x As New HoMIDom.HoMIDom.DeviceAction
        Dim y As New HoMIDom.HoMIDom.DeviceAction.Parametre

        If _dev.Type <> HoMIDom.HoMIDom.Device.ListeDevices.VOLET Then
            x.Nom = "DIM"
        Else
            x.Nom = "OUVERTURE"
        End If

        y.Value = Value
        x.Parametres.Add(y)
        myService.ExecuteDeviceCommand(IdSrv, Id, x)
    End Sub

    Private Sub Stk1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Stk1.MouseDown
        If _zone IsNot Nothing Then
            RaiseEvent ShowZone(_zone.ID)
            Exit Sub
        End If
        If _macro IsNot Nothing Then
            myService.RunMacro(IdSrv, _macro.ID)
            Exit Sub
        End If
        If StkPopup.Children.Count > 0 Then
            If Popup1.IsOpen = False Then
                Popup1.IsOpen = True
            Else
                Popup1.IsOpen = False
            End If
        End If

    End Sub
End Class
