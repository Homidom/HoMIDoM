﻿Imports System.Windows.Threading

Public Class uWidgetEmpty

    Dim _Id As String
    Dim _Down As DateTime
    Dim _dev As HoMIDom.HoMIDom.TemplateDevice = Nothing
    Dim _macro As HoMIDom.HoMIDom.Macro = Nothing
    Dim _zone As HoMIDom.HoMIDom.Zone = Nothing
    Dim _ShowStatus As Boolean = True 'Affiche le status - Oui par défaut
    Dim _ShowEtiquette As Boolean = True 'Affiche le libellé du composant par défaut - Oui par défaut
    Dim _Etiquette As String = "" 'Etiquette
    Dim _X As Double = 0
    Dim _Y As Double = 0
    Dim _Rotation As Double = 0
    Dim _ModeEdition As Boolean
    Dim _Picture As String
    Dim _ImageBackGround As String
    Dim _LabelStatus As String
    Dim _DefautLabelStatus As String = "?"
    Dim _Refresh As Integer = 1
    Dim _ColorBackGround As SolidColorBrush
    Dim _Visuel As New List(Of cWidget.Visu)
    'Dim _Type As cElement.WidgetType = cElement.WidgetType.VIERGE
    Dim _ZoneId As String
    Dim dt As DispatcherTimer

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
                    _Picture = _dev.Picture
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
                        Case HoMIDom.HoMIDom.Device.ListeDevices.FREEBOX
                            ShowStatus = False
                            Dim x As New uFreeBox
                            AddHandler x.ButtonClick, AddressOf FreeTouch
                            StkPopup.Children.Add(x)
                        Case Else

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
                    _Picture = _zone.Icon
                End If
            End If

            TraiteRefresh()
        End Set
    End Property

    Public Property ZoneId As String
        Get
            Return _ZoneId
        End Get
        Set(ByVal value As String)
            _ZoneId = value
        End Set
    End Property

    'Public ReadOnly Property Type As cElement.WidgetType
    '    Get
    '        Return _Type
    '    End Get
    'End Property

    Public Property ShowEtiquette As Boolean
        Get
            Return _ShowEtiquette
        End Get
        Set(ByVal value As Boolean)
            _ShowEtiquette = value
            If value = True Then
                Lbl.Visibility = Windows.Visibility.Visible
                If _dev IsNot Nothing Then
                    Etiquette = _dev.Name
                End If
                If _zone IsNot Nothing Then
                    Etiquette = _zone.Name
                End If
                If _macro IsNot Nothing Then
                    Etiquette = _macro.Nom
                End If
            Else
                Lbl.Visibility = Windows.Visibility.Collapsed
            End If
        End Set
    End Property

    Public Property Etiquette As String
        Get
            Return _Etiquette
        End Get
        Set(ByVal value As String)
            _Etiquette = value
            Lbl.Content = _Etiquette
        End Set
    End Property

    Public Property X As Double
        Get
            Return _X
        End Get
        Set(ByVal value As Double)
            _X = value
        End Set
    End Property

    Public Property Y As Double
        Get
            Return _Y
        End Get
        Set(ByVal value As Double)
            _Y = value
        End Set
    End Property

    Public Property Rotation As Double
        Get
            Return _Rotation
        End Get
        Set(ByVal value As Double)
            _Rotation = value
        End Set
    End Property

    Public Property ModeEdition As Boolean
        Get
            Return _ModeEdition
        End Get
        Set(ByVal value As Boolean)
            _ModeEdition = value
        End Set
    End Property

    Public Property Picture As String
        Get
            Return _Picture
        End Get
        Set(ByVal value As String)
            _Picture = value
        End Set
    End Property

    Public Property ImageBackGround As String
        Get
            Return _ImageBackGround
        End Get
        Set(ByVal value As String)
            _ImageBackGround = value
        End Set
    End Property

    Public Property LabelStatus As String
        Get
            Return _LabelStatus
        End Get
        Set(ByVal value As String)
            _LabelStatus = value
            LblStatus.Content = value
        End Set
    End Property

    Public Property ShowStatus As Boolean
        Get
            Return _ShowStatus
        End Get
        Set(ByVal value As Boolean)
            _ShowStatus = value
            If value = False Then
                LblStatus.Visibility = Windows.Visibility.Collapsed
            Else
                LblStatus.Visibility = Windows.Visibility.Visible
                LblStatus.Width = Double.NaN
                LblStatus.Height = Double.NaN
            End If
        End Set
    End Property

    Public Property DefautLabelStatus As String
        Get
            Return _DefautLabelStatus
        End Get
        Set(ByVal value As String)
            _DefautLabelStatus = value
        End Set
    End Property

    Public Property Refresh As Integer
        Get
            Return _Refresh
        End Get
        Set(ByVal value As Integer)
            _Refresh = value
            dt.Interval = New TimeSpan(0, 0, _Refresh)
        End Set
    End Property

    Public Property ColorBackGround As SolidColorBrush
        Get
            Return _ColorBackGround
        End Get
        Set(ByVal value As SolidColorBrush)
            _ColorBackGround = value
            Border1.Background = value
        End Set
    End Property

    Public Property Visuel As List(Of cWidget.Visu)
        Get
            Return _Visuel
        End Get
        Set(ByVal value As List(Of cWidget.Visu))
            _Visuel = value
        End Set
    End Property

    Public Sub New()

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        dt = New DispatcherTimer()
        AddHandler dt.Tick, AddressOf dispatcherTimer_Tick
        dt.Interval = New TimeSpan(0, 0, _Refresh)
        dt.Start()

        LblStatus.Content = DefautLabelStatus
    End Sub

    Private Sub TraiteRefresh()
        Try
            If _Id = "" Then Exit Sub

            If IsConnect = True Then
                _dev = myService.ReturnDeviceByID(IdSrv, _Id)
            Else
                Exit Sub
            End If

            If _dev IsNot Nothing Then
                If ShowEtiquette And _dev.Name <> Etiquette Then
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
                        If StkPopup.Children.Count = 2 Then
                            Dim x2 As uVariateur = StkPopup.Children.Item(1)
                            x2.Value = _dev.Value
                        End If
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
                If ShowEtiquette And _zone.Name <> Etiquette Then
                    Etiquette = _zone.Name
                End If
            End If

            If _macro IsNot Nothing Then
                If ShowEtiquette And _macro.Nom <> Etiquette Then
                    Etiquette = _macro.Nom
                End If
            End If
        Catch ex As Exception
            MsgBox("Error Refresh: " & ex.ToString & vbCrLf)
        End Try
    End Sub

    Public Sub dispatcherTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        TraiteRefresh()
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

    Private Sub FreeTouch(ByVal Touche As String)
        Try
            Dim x As New HoMIDom.HoMIDom.DeviceAction
            If _dev IsNot Nothing Then
                x.Nom = Touche
                x.Parametres = Nothing
                myService.ExecuteDeviceCommand(IdSrv, Id, x)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString, "Erreur", MessageBoxButton.OK)
        End Try
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

                If StkPopup.Children.Count > 0 Then
                    If Popup1.IsOpen = True Then
                        Popup1.IsOpen = False
                    End If
                End If
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

                If StkPopup.Children.Count > 0 Then
                    If Popup1.IsOpen = True Then
                        Popup1.IsOpen = False
                    End If
                End If
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


        If _ModeEdition Then
            Try
                Dim x As New WWidgetProperty(Me)
                x.Owner = frmMere
                x.ShowDialog()
                If x.DialogResult.HasValue And x.DialogResult.Value Then
                    x.Close()
                Else
                    x.Close()
                End If

            Catch ex As Exception
                MessageBox.Show("Erreur: " & ex.ToString)
            End Try
        Else
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
        End If
    End Sub

    Private Sub uWidgetEmpty_Unloaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Unloaded

    End Sub
End Class