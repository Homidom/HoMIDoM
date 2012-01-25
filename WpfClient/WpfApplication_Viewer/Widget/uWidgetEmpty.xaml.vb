Imports System.Windows.Threading

Public Class uWidgetEmpty

    Dim _Id As String
    Dim _Down As DateTime
    Dim _dev As HoMIDom.HoMIDom.TemplateDevice = Nothing
    Dim _macro As HoMIDom.HoMIDom.Macro = Nothing
    Dim _zone As HoMIDom.HoMIDom.Zone = Nothing
    Dim _ShowStatus As Boolean = True 'Affiche le status - Oui par défaut
    Dim _ShowEtiqDefaut As Boolean = True 'Affiche le libellé du composant par défaut - Oui par défaut
    Dim _Etiquette As String = "" 'Etiquette
    Dim _ColorBackGround As SolidColorBrush = New SolidColorBrush(Colors.Black)
    Dim _Type As cElement.WidgetType = cElement.WidgetType.VIERGE

    Public Event Click(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
    Public Event LongClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
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
                    Etiquette = _dev.Name
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
                    Etiquette = _macro.Nom
                    ShowStatus = False
                End If

                If _zone IsNot Nothing Then
                    Etiquette = _zone.Name
                    Image.Source = ConvertArrayToImage(myService.GetByteFromImage(_zone.Icon))
                    ShowStatus = False
                End If
            End If

            Refresh()
        End Set
    End Property

    Public ReadOnly Property Type As cElement.WidgetType
        Get
            Return _Type
        End Get
    End Property


    Public Property ShowEtiquetteByDefaut As Boolean
        Get
            Return _ShowEtiqDefaut
        End Get
        Set(ByVal value As Boolean)
            _ShowEtiqDefaut = value
            If value = True Then
                If _dev IsNot Nothing Then
                    Etiquette = _dev.Name
                End If
                If _zone IsNot Nothing Then
                    Etiquette = _zone.Name
                End If
                If _macro IsNot Nothing Then
                    Etiquette = _macro.Nom
                End If
            End If
        End Set
    End Property

    Private Sub ShowEtiquette()
        Lbl.Content = _Etiquette
    End Sub

    Public Property Etiquette As String
        Get
            Return _Etiquette
        End Get
        Set(ByVal value As String)
            _Etiquette = value
            ShowEtiquette()
        End Set
    End Property

    Public Property ShowStatus As Boolean
        Get
            Return _ShowStatus
        End Get
        Set(ByVal value As Boolean)
            _ShowStatus = value
            If value = False Then
                LblStatus.Visibility = Windows.Visibility.Hidden
                LblStatus.Width = 0
                LblStatus.Height = 0
            Else
                LblStatus.Visibility = Windows.Visibility.Visible
                LblStatus.Width = Double.NaN
                LblStatus.Height = Double.NaN
            End If
        End Set
    End Property

    Public Property ColorBackGround As SolidColorBrush
        Get
            Return _ColorBackGround
        End Get
        Set(ByVal value As SolidColorBrush)
            Border1.Background = value
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
                If ShowEtiquetteByDefaut And _dev.Name <> Etiquette Then
                    Etiquette = _dev.Name
                End If

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
            End If

            If _zone IsNot Nothing Then
                If ShowEtiquetteByDefaut And _zone.Name <> Etiquette Then
                    Etiquette = _zone.Name
                End If
            End If

            If _macro IsNot Nothing Then
                If ShowEtiquetteByDefaut And _macro.Nom <> Etiquette Then
                    Etiquette = _macro.Nom
                End If
            End If
        Catch ex As Exception
            MsgBox("Error Refresh: " & ex.ToString & vbCrLf)
        End Try
    End Sub

    Public Sub dispatcherTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        Refresh()
    End Sub

    Private Sub Image_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Image.MouseLeftButtonUp, Stk1.MouseLeftButtonUp
        Dim vDiff As TimeSpan = Now - _Down
        If vDiff.Seconds < 1 Then
            RaiseEvent Click(Me, e)
        Else
            RaiseEvent LongClick(Me, e)
        End If
    End Sub

    Private Sub Image_PreviewMouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Image.PreviewMouseDown, Stk1.PreviewMouseDown
        _Down = Now
    End Sub

    Private Sub ClosePopup()
        Popup1.IsOpen = False
    End Sub

    Private Sub ClickOn(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Try
            Dim x As New HoMIDom.HoMIDom.DeviceAction
            If _dev IsNot Nothing Then
                If _dev.Type <> HoMIDom.HoMIDom.Device.ListeDevices.VOLET Then
                    x.Nom = "ON"
                Else
                    x.Nom = "OPEN"
                End If
                x.Parametres = Nothing
                myService.ExecuteDeviceCommand(IdSrv, Id, x)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString, "Erreur", MessageBoxButton.OK)
        End Try
    End Sub

    Private Sub ClickOff(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Try
            Dim x As New HoMIDom.HoMIDom.DeviceAction
            If _dev IsNot Nothing Then
                If _dev.Type <> HoMIDom.HoMIDom.Device.ListeDevices.VOLET Then
                    x.Nom = "OFF"
                Else
                    x.Nom = "CLOSE"
                End If
                x.Parametres = Nothing
                myService.ExecuteDeviceCommand(IdSrv, Id, x)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString, "Erreur", MessageBoxButton.OK)
        End Try
    End Sub

    Private Sub ValueChange(ByVal Value As Integer)
        Try
            Dim x As New HoMIDom.HoMIDom.DeviceAction
            Dim y As New HoMIDom.HoMIDom.DeviceAction.Parametre

            If _dev IsNot Nothing Then
                If _dev.Type <> HoMIDom.HoMIDom.Device.ListeDevices.VOLET Then
                    x.Nom = "DIM"
                Else
                    x.Nom = "OUVERTURE"
                End If

                y.Value = Value
                x.Parametres.Add(y)
                myService.ExecuteDeviceCommand(IdSrv, Id, x)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString, "Erreur", MessageBoxButton.OK)
        End Try
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
