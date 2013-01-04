Imports System.Windows.Threading
Imports System.Reflection
Imports System.Threading

Public Class uWidgetEmpty
    Public Enum TypeOfWidget
        Empty = 0
        Web = 1
        Media = 2
        Rss = 3
        Meteo = 4
        KeyPad = 5
        Label = 6
        Device = 99
    End Enum

    'Variables
    Dim _Show As Boolean
    Dim _Id As String = ""
    Dim _ParentId As String = ""
    Dim _Down As DateTime
    Dim _dev As HoMIDom.HoMIDom.TemplateDevice = Nothing
    Dim _macro As HoMIDom.HoMIDom.Macro = Nothing
    Dim _zone As HoMIDom.HoMIDom.Zone = Nothing
    Dim _ShowStatus As Boolean = True 'Affiche le status - Oui par défaut
    Dim _ShowEtiquette As Boolean = True 'Affiche le libellé du composant par défaut - Oui par défaut
    Dim _Etiquette As String = "" 'Etiquette
    Dim _TailleEtiquette As Double = 9
    Dim _X As Double = 0
    Dim _Y As Double = 0
    Dim _Rotation As Double = 0
    Dim _ModeEdition As Boolean '=True si on est en mode edition
    Dim _ShowPicture As Boolean = True
    Dim _Picture As String = ""
    Dim _ImageBackGround As String
    Dim _LabelStatus As String
    Dim _DefautLabelStatus As String = "?"
    Dim _TailleStatus As Double = 9
    Dim _Refresh As Integer = 1
    Dim _ColorStatus As SolidColorBrush = Brushes.White
    Dim _ColorBackGround As SolidColorBrush = Brushes.Black
    Dim _Visuel As New List(Of cWidget.Visu)
    Dim _ZoneId As String
    Dim _IsEmpty As Boolean = True
    Dim _Action_On_Click As New List(Of cWidget.Action)
    Dim _Action_On_LongClick As New List(Of cWidget.Action)
    Dim _Action_GestureGaucheDroite As New List(Of cWidget.Action)
    Dim _Action_GestureDroiteGauche As New List(Of cWidget.Action)
    Dim _Action_GestureHautBas As New List(Of cWidget.Action)
    Dim _Action_GestureBasHaut As New List(Of cWidget.Action)
    Dim _oldposition As Point = Nothing
    Dim _FlagBlock As Boolean = False
    Dim _dt As DispatcherTimer
    Dim _CurrentValue As Object = Nothing
    Dim _type As TypeOfWidget = TypeOfWidget.Empty
    Dim _CanEditValue As Boolean = False

    'Variables Widget Web
    Dim _URL As String = ""
    Dim _Webbrowser As uHttp = Nothing
    Dim _ListHttpBtn As New List(Of uHttp.ButtonHttp)
    Dim _RefreshHttp As Integer = 0

    'Variables Widget RSS
    Dim _URLRss As String = ""
    Dim _RSS As uRSS = Nothing

    'Variables Widget Meteo
    Dim _IDMeteo As String = ""
    Dim _METEO As uWMeteo = Nothing

    'Variables Widget KeyPad
    Dim _IDKeyPad As String = ""
    Dim _KeyPad As uKeyPad = Nothing

    'Event
    Public Event Click(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
    Public Event LongClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
    Public Event GestureHautBas(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
    Public Event GestureBasHaut(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
    Public Event GestureGaucheDroite(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
    Public Event GestureDroiteGauche(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
    Public Event ShowZone(ByVal zoneid As String)

#Region "Property"
    Public Property Show As Boolean
        Get
            Return _Show
        End Get
        Set(ByVal value As Boolean)
            _Show = value
        End Set
    End Property

    Public Property Id As String
        Get
            Return _Id
        End Get
        Set(ByVal value As String)
            _Id = value
            If _Show = False Then Exit Property

            If IsConnect = True And value <> "" Then
                _dev = myService.ReturnDeviceByID(IdSrv, _Id)
                _macro = myService.ReturnMacroById(IdSrv, _Id)
                _zone = myService.ReturnZoneByID(IdSrv, _Id)

                IsEmpty = True

                If _dev IsNot Nothing Then
                    Etiquette = _dev.Name
                    Picture = _dev.Picture
                    Dim _ShowValue As Boolean = True

                    Select Case _dev.Type
                        Case HoMIDom.HoMIDom.Device.ListeDevices.APPAREIL
                            Dim x As New uOnOff
                            x.ContentOn = "ON"
                            x.ContentOff = "OFF"
                            AddHandler x.ClickOn, AddressOf ClickOn
                            AddHandler x.ClickOff, AddressOf ClickOff
                            StkPopup.Children.Add(x)
                        Case HoMIDom.HoMIDom.Device.ListeDevices.AUDIO
                            Dim x As New uMedia
                            x.IsLocal = False
                            x.ShowVideo = False
                            x.ShowTag = False
                            x.ShowBtnAvance = False
                            x.ShowBtnRecul = False
                            x.ShowSliderTime = False
                            AddHandler x.Play, AddressOf AudioPlay
                            AddHandler x.Pause, AddressOf AudioPause
                            AddHandler x.Stop, AddressOf AudioStop
                            AddHandler x.Avance, AddressOf AudioAvance
                            AddHandler x.Recul, AddressOf AudioRecul
                            AddHandler x.NextChap, AddressOf AudioNextChap
                            AddHandler x.PreviousChap, AddressOf AudioPreviousChap
                            AddHandler x.Mute, AddressOf AudioMute
                            AddHandler x.VolumeUp, AddressOf AudioVolumeUp
                            AddHandler x.VolumeDown, AddressOf AudioVolumeDown

                            StkPopup.Children.Add(x)
                        Case HoMIDom.HoMIDom.Device.ListeDevices.BAROMETRE
                        Case HoMIDom.HoMIDom.Device.ListeDevices.BATTERIE
                        Case HoMIDom.HoMIDom.Device.ListeDevices.COMPTEUR
                        Case HoMIDom.HoMIDom.Device.ListeDevices.CONTACT
                        Case HoMIDom.HoMIDom.Device.ListeDevices.DETECTEUR
                        Case HoMIDom.HoMIDom.Device.ListeDevices.DIRECTIONVENT
                        Case HoMIDom.HoMIDom.Device.ListeDevices.ENERGIEINSTANTANEE
                        Case HoMIDom.HoMIDom.Device.ListeDevices.ENERGIETOTALE
                        Case HoMIDom.HoMIDom.Device.ListeDevices.GENERIQUESTRING
                            Dim x As New uEditValue
                            AddHandler x.ChangeValue, AddressOf ChangeValue
                            StkPopup.Children.Add(x)

                        Case HoMIDom.HoMIDom.Device.ListeDevices.GENERIQUEBOOLEEN
                            Dim x As New uEditValue
                            AddHandler x.ChangeValue, AddressOf ChangeValue
                            StkPopup.Children.Add(x)

                        Case HoMIDom.HoMIDom.Device.ListeDevices.GENERIQUEVALUE
                            Dim x As New uEditValue
                            AddHandler x.ChangeValue, AddressOf ChangeValue
                            StkPopup.Children.Add(x)

                        Case HoMIDom.HoMIDom.Device.ListeDevices.HUMIDITE

                        Case HoMIDom.HoMIDom.Device.ListeDevices.LAMPE
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
                            StkEmptyetDevice.Visibility = Windows.Visibility.Collapsed
                            StkTool.Visibility = Windows.Visibility.Visible

                            _METEO = New uWMeteo
                            StkTool.Children.Add(_METEO)
                            IDMeteo = _Id
                        Case HoMIDom.HoMIDom.Device.ListeDevices.MULTIMEDIA
                            ShowStatus = False
                            _ShowValue = False
                            Dim x As New uTelecommande(_Id)
                            AddHandler x.SendCommand, AddressOf SendCommand
                            StkPopup.Children.Add(x)
                        Case HoMIDom.HoMIDom.Device.ListeDevices.PLUIECOURANT
                        Case HoMIDom.HoMIDom.Device.ListeDevices.PLUIETOTAL
                        Case HoMIDom.HoMIDom.Device.ListeDevices.SWITCH
                            Dim x As New uOnOff
                            x.ContentOn = "ON"
                            x.ContentOff = "OFF"
                            AddHandler x.ClickOn, AddressOf ClickOn
                            AddHandler x.ClickOff, AddressOf ClickOff
                            StkPopup.Children.Add(x)
                        Case HoMIDom.HoMIDom.Device.ListeDevices.TELECOMMANDE
                            _ShowValue = False
                        Case HoMIDom.HoMIDom.Device.ListeDevices.TEMPERATURE
                        Case HoMIDom.HoMIDom.Device.ListeDevices.TEMPERATURECONSIGNE
                            Dim x As New uEditValue
                            AddHandler x.ChangeValue, AddressOf ChangeValue
                            StkPopup.Children.Add(x)

                        Case HoMIDom.HoMIDom.Device.ListeDevices.VITESSEVENT
                        Case HoMIDom.HoMIDom.Device.ListeDevices.VOLET

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
                        Case HoMIDom.HoMIDom.Device.ListeDevices.FREEBOX
                            _ShowValue = False
                            ShowStatus = False
                            Dim x As New uFreeBox
                            AddHandler x.ButtonClick, AddressOf FreeTouch
                            StkPopup.Children.Add(x)
                        Case Else

                    End Select

                    If _ShowValue Then
                        LblStatus.Content = _dev.Value & _dev.Unit
                        _CurrentValue = _dev.Value

                        If IsNumeric(_dev.Value) = True Or IsBoolean(_dev.Value) = True Then
                            Picture = GetOnOffPicture(_dev.Picture, CSng(_dev.Value))
                        End If
                    End If
                ElseIf _macro IsNot Nothing Then
                    Etiquette = _macro.Nom
                    ShowStatus = False
                    IsEmpty = False
                ElseIf _zone IsNot Nothing Then
                    Etiquette = _zone.Name
                    Image.Source = ConvertArrayToImage(myService.GetByteFromImage(_zone.Icon))
                    ShowStatus = False
                    _Picture = _zone.Icon
                    IsEmpty = False
                End If
            End If

            TraiteRefresh()
        End Set
    End Property

    Public Property ParentId As String
        Get
            Return _Id
        End Get
        Set(ByVal value As String)
            _ParentId = value
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

    Public Property IsEmpty As Boolean
        Get
            Return _IsEmpty
        End Get
        Set(ByVal value As Boolean)
            _IsEmpty = value

            If _Show = False Then Exit Property

            If value = True Then
                Image.Width = Double.NaN
                Image.Height = Double.NaN
            Else
                _type = TypeOfWidget.Device
            End If
        End Set
    End Property

    Public Property Type As TypeOfWidget
        Get
            Return _type
        End Get
        Set(ByVal value As TypeOfWidget)
            _type = value

            If _Show = False Then Exit Property

            Try
                Select Case _type
                    Case TypeOfWidget.Web
                        StkEmptyetDevice.Visibility = Windows.Visibility.Collapsed
                        StkTool.Visibility = Windows.Visibility.Visible
                        _Webbrowser = New uHttp
                        StkTool.Children.Add(_Webbrowser)
                    Case TypeOfWidget.Rss
                        StkEmptyetDevice.Visibility = Windows.Visibility.Collapsed
                        StkTool.Visibility = Windows.Visibility.Visible

                        _RSS = New uRSS
                        _RSS.VerticalAlignment = Windows.VerticalAlignment.Stretch
                        StkTool.Children.Add(_RSS)
                    Case TypeOfWidget.Meteo
                        StkEmptyetDevice.Visibility = Windows.Visibility.Collapsed
                        StkTool.Visibility = Windows.Visibility.Visible

                        _METEO = New uWMeteo
                        StkTool.Children.Add(_METEO)
                    Case TypeOfWidget.KeyPad
                        StkEmptyetDevice.Visibility = Windows.Visibility.Collapsed
                        StkTool.Visibility = Windows.Visibility.Visible

                        _KeyPad = New uKeyPad
                        AddHandler _KeyPad.KeyPadOk, AddressOf KeyPadOK
                        StkTool.Children.Add(_KeyPad)
                    Case TypeOfWidget.Label
                        ColorBackGround = Brushes.Transparent
                        Lbl.MinWidth = 100
                        StkEmptyetDevice.Visibility = Windows.Visibility.Collapsed
                        StkTool.Visibility = Windows.Visibility.Collapsed
                        ShowPicture = False
                    Case Else
                        _dt = New DispatcherTimer()
                        _dt.Interval = New TimeSpan(0, 0, _Refresh)
                        AddHandler _dt.Tick, AddressOf dispatcherTimer_Tick
                        _dt.Start()
                End Select
            Catch ex As Exception
                MessageBox.Show("Erreur Property Type Set: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End Set
    End Property

    Public Property CanEditValue As Boolean
        Get
            Return _CanEditValue
        End Get
        Set(ByVal value As Boolean)
            _CanEditValue = value
        End Set
    End Property

    Public Property ShowEtiquette As Boolean
        Get
            Return _ShowEtiquette
        End Get
        Set(ByVal value As Boolean)
            _ShowEtiquette = value

            If _Show = False Then Exit Property

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
            If _Show = False Then Exit Property
            Lbl.Content = _Etiquette
        End Set
    End Property

    Public Property TailleEtiquette As Double
        Get
            Return _TailleEtiquette
        End Get
        Set(ByVal value As Double)
            Try
                _TailleEtiquette = value
                If _Show = False Then Exit Property
                Lbl.FontSize = value
            Catch ex As Exception
                MessageBox.Show("Erreur Property TailleEtiquette Set: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End Set
    End Property

    Public Property X As Double
        Get
            Return _X
        End Get
        Set(ByVal value As Double)
            _X = value
            If _Show = False Then Exit Property
            If Me.Parent IsNot Nothing Then
                Dim x As ContentControl = Me.Parent
                Canvas.SetLeft(x, value)
            End If
        End Set
    End Property

    Public Property Y As Double
        Get
            Return _Y
        End Get
        Set(ByVal value As Double)
            _Y = value
            If _Show = False Then Exit Property

            If Me.Parent IsNot Nothing Then
                Dim x As ContentControl = Me.Parent
                Canvas.SetTop(x, value)
            End If
        End Set
    End Property

    Public Property Rotation As Double
        Get
            Return _Rotation
        End Get
        Set(ByVal value As Double)
            _Rotation = value
            If _Show = False Then Exit Property

            If Me.Parent IsNot Nothing Then
                Dim x As ContentControl = Me.Parent
                Dim Trg As New TransformGroup
                Dim Rot As New RotateTransform(value)
                Trg.Children.Add(Rot)
                x.RenderTransform = Trg
            End If
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
            Try
                If _Show = True Then
                    If Image.Tag <> _Picture Then
                        Image.Tag = _Picture
                        Image.Source = ConvertArrayToImage(myService.GetByteFromImage(_Picture))
                    End If
                End If
            Catch ex As Exception
                MessageBox.Show("Erreur Property Set Picture: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End Set
    End Property

    Public Property ShowPicture As Boolean
        Get
            Return _ShowPicture
        End Get
        Set(ByVal value As Boolean)
            _ShowPicture = value
            If _Show = False Then Exit Property

            If value = True Then
                Image.Visibility = Windows.Visibility.Visible
            Else
                Image.Visibility = Windows.Visibility.Collapsed
            End If
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
            If _Show = False Then Exit Property

            LblStatus.Content = value
        End Set
    End Property

    Public Property TailleStatus As Double
        Get
            Return _TailleStatus
        End Get
        Set(ByVal value As Double)
            _TailleStatus = value
            If _Show = False Then Exit Property

            LblStatus.FontSize = value
        End Set
    End Property

    Public Property ShowStatus As Boolean
        Get
            Return _ShowStatus
        End Get
        Set(ByVal value As Boolean)
            _ShowStatus = value
            If _Show = False Then Exit Property

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
            If _Show = False Then
                Exit Property
            Else
                _dt.Stop()
                _dt.Interval = New TimeSpan(0, 0, _Refresh)
                _dt.Start()
            End If
        End Set
    End Property

    Public Property ColorBackGround As SolidColorBrush
        Get
            Return _ColorBackGround
        End Get
        Set(ByVal value As SolidColorBrush)
            _ColorBackGround = value
            If _Show = False Then Exit Property

            Border1.Background = value
        End Set
    End Property

    Public Property ColorStatus As SolidColorBrush
        Get
            Return _ColorStatus
        End Get
        Set(ByVal value As SolidColorBrush)
            _ColorStatus = value
            If _Show = False Then Exit Property

            LblStatus.Foreground = value
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

    Public Shadows Property Width As Double
        Get
            Return MyBase.Width
        End Get
        Set(ByVal value As Double)
            MyBase.Width = value
            If _Show = False Then Exit Property

            If Me.Parent IsNot Nothing Then
                Dim x As ContentControl = Me.Parent
                x.Width = value
            End If
        End Set
    End Property

    Public Shadows Property Height As Double
        Get
            Return MyBase.Height
        End Get
        Set(ByVal value As Double)
            MyBase.Height = value
            If _Show = False Then Exit Property

            If Me.Parent IsNot Nothing Then
                Dim x As ContentControl = Me.Parent
                x.Height = value
            End If
        End Set
    End Property
#End Region

#Region "Property/Sub Web"
    Public Property URL As String
        Get
            Return _URL
        End Get
        Set(ByVal value As String)
            _URL = value
            If _Show = False Then Exit Property

            Try
                If My.Computer.Network.IsAvailable = True And _Webbrowser IsNot Nothing And _type = TypeOfWidget.Web Then
                    _Webbrowser.URL = _URL
                    '_Webbrowser.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, DirectCast(Sub() _Webbrowser.Navigate(New Uri(_URL)), ThreadStart))
                End If
            Catch ex As Exception
                MessageBox.Show("Erreur UWidget.Empty.URL: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End Set
    End Property

    Public Property ListHttpButton As List(Of uHttp.ButtonHttp)
        Get
            Return _ListHttpBtn
        End Get
        Set(ByVal value As List(Of uHttp.ButtonHttp))
            _ListHttpBtn = value
            If _Show = False Then Exit Property

            If _Webbrowser IsNot Nothing Then _Webbrowser.ListButton = value
        End Set
    End Property

    Public Property HttpRefresh As Integer
        Get
            Return _RefreshHttp
        End Get
        Set(ByVal value As Integer)
            _RefreshHttp = value
        End Set
    End Property
#End Region

#Region "Property/Sub Rss"
    Public Property UrlRss As String
        Get
            Return _URLRss
        End Get
        Set(ByVal value As String)
            _URLRss = value
            If _Show = False Then Exit Property

            If _RSS IsNot Nothing And _type = TypeOfWidget.Rss Then _RSS.URIRss = value
        End Set
    End Property
#End Region

#Region "Property/Sub Meteo"
    Public Property IDMeteo As String
        Get
            Return _IDMeteo
        End Get
        Set(ByVal value As String)
            _IDMeteo = value
            If _Show = False Then Exit Property

            If _METEO IsNot Nothing And _type = TypeOfWidget.Meteo Then _METEO.ID = value
        End Set
    End Property
#End Region

#Region "Property/Sub Keypad"
    Public Property IDKeyPad As String
        Get
            Return _IDKeyPad
        End Get
        Set(ByVal value As String)
            _IDKeyPad = value
        End Set
    End Property
#End Region

#Region "Property Actions"
    Public Property Action_On_Click As List(Of cWidget.Action)
        Get
            Return _Action_On_Click
        End Get
        Set(ByVal value As List(Of cWidget.Action))
            _Action_On_Click = value
        End Set
    End Property

    Public Property Action_On_LongClick As List(Of cWidget.Action)
        Get
            Return _Action_On_LongClick
        End Get
        Set(ByVal value As List(Of cWidget.Action))
            _Action_On_LongClick = value
        End Set
    End Property

    Public Property Action_GestureGaucheDroite As List(Of cWidget.Action)
        Get
            Return _Action_GestureGaucheDroite
        End Get
        Set(ByVal value As List(Of cWidget.Action))
            _Action_GestureGaucheDroite = value
        End Set
    End Property

    Public Property Action_GestureDroiteGauche As List(Of cWidget.Action)
        Get
            Return _Action_GestureDroiteGauche
        End Get
        Set(ByVal value As List(Of cWidget.Action))
            _Action_GestureDroiteGauche = value
        End Set
    End Property

    Public Property Action_GestureHautBas As List(Of cWidget.Action)
        Get
            Return _Action_GestureHautBas
        End Get
        Set(ByVal value As List(Of cWidget.Action))
            _Action_GestureHautBas = value
        End Set
    End Property

    Public Property Action_GestureBasHaut As List(Of cWidget.Action)
        Get
            Return _Action_GestureBasHaut
        End Get
        Set(ByVal value As List(Of cWidget.Action))
            _Action_GestureBasHaut = value
        End Set
    End Property
#End Region

    Public Sub New()

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        Try
            LblStatus.Content = DefautLabelStatus
        Catch ex As Exception
            MessageBox.Show("Erreur New: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub TraiteRefresh()
        Try
            If _ModeEdition = False And IsConnect Then
                If _Visuel.Count > 0 Then
                    For Each _ElmtVisu As cWidget.Visu In _Visuel
                        _dev = myService.ReturnDeviceByID(IdSrv, _ElmtVisu.IdObject)
                        If _dev IsNot Nothing Then
                            If _dev.Value = _ElmtVisu.Value Then
                                If Image.Tag <> _ElmtVisu.Image Then
                                    Image.Tag = _ElmtVisu.Image
                                    Image.Source = ConvertArrayToImage(myService.GetByteFromImage(_ElmtVisu.Image))
                                End If
                            End If
                        End If
                    Next
                End If

                If _Id = "" Then Exit Sub

                _dev = myService.ReturnDeviceByID(IdSrv, _Id)

                If _dev IsNot Nothing Then
                    If ShowEtiquette And _dev.Name <> _Etiquette And _IsEmpty = False Then
                        Etiquette = _dev.Name
                    End If
                    If Image.Tag <> _dev.Picture Then
                        Image.Tag = _dev.Picture
                        Image.Source = ConvertArrayToImage(myService.GetByteFromImage(_dev.Picture))
                    End If

                    Dim _ShowValue As Boolean = True
                    Dim _IsVariation As Boolean = False

                    Select Case _dev.Type
                        Case HoMIDom.HoMIDom.Device.ListeDevices.APPAREIL

                        Case HoMIDom.HoMIDom.Device.ListeDevices.AUDIO

                        Case HoMIDom.HoMIDom.Device.ListeDevices.BAROMETRE

                        Case HoMIDom.HoMIDom.Device.ListeDevices.BATTERIE

                        Case HoMIDom.HoMIDom.Device.ListeDevices.COMPTEUR

                        Case HoMIDom.HoMIDom.Device.ListeDevices.CONTACT

                        Case HoMIDom.HoMIDom.Device.ListeDevices.DETECTEUR

                        Case HoMIDom.HoMIDom.Device.ListeDevices.DIRECTIONVENT

                        Case HoMIDom.HoMIDom.Device.ListeDevices.ENERGIEINSTANTANEE

                        Case HoMIDom.HoMIDom.Device.ListeDevices.ENERGIETOTALE

                        Case HoMIDom.HoMIDom.Device.ListeDevices.GENERIQUESTRING

                        Case HoMIDom.HoMIDom.Device.ListeDevices.GENERIQUEBOOLEEN

                        Case HoMIDom.HoMIDom.Device.ListeDevices.GENERIQUEVALUE

                        Case HoMIDom.HoMIDom.Device.ListeDevices.HUMIDITE

                        Case HoMIDom.HoMIDom.Device.ListeDevices.LAMPE
                            _IsVariation = True
                        Case HoMIDom.HoMIDom.Device.ListeDevices.METEO
                            _ShowValue = False
                        Case HoMIDom.HoMIDom.Device.ListeDevices.MULTIMEDIA
                            _ShowValue = False
                        Case HoMIDom.HoMIDom.Device.ListeDevices.PLUIECOURANT

                        Case HoMIDom.HoMIDom.Device.ListeDevices.PLUIETOTAL

                        Case HoMIDom.HoMIDom.Device.ListeDevices.SWITCH

                        Case HoMIDom.HoMIDom.Device.ListeDevices.TELECOMMANDE

                        Case HoMIDom.HoMIDom.Device.ListeDevices.TEMPERATURE

                        Case HoMIDom.HoMIDom.Device.ListeDevices.TEMPERATURECONSIGNE

                        Case HoMIDom.HoMIDom.Device.ListeDevices.VITESSEVENT

                        Case HoMIDom.HoMIDom.Device.ListeDevices.VOLET
                            _IsVariation = True
                        Case HoMIDom.HoMIDom.Device.ListeDevices.UV

                    End Select

                    If _ShowValue Then
                        If _CurrentValue <> _dev.Value Then
                            LblStatus.Content = _dev.Value & _dev.Unit
                            _CurrentValue = _dev.Value

                            If IsNumeric(_dev.Value) = True Or IsBoolean(_dev.Value) = True Then
                                Picture = GetOnOffPicture(_dev.Picture, CSng(_dev.Value))
                            End If

                            If _IsVariation Then
                                If StkPopup.Children.Count = 2 Then
                                    Dim x2 As uVariateur = StkPopup.Children.Item(1)
                                    x2.Value = _dev.Value
                                End If
                            End If
                        End If
                    End If
                ElseIf _zone IsNot Nothing Then
                    If ShowEtiquette And _zone.Name <> Etiquette Then
                        Etiquette = _zone.Name
                        If Image.Tag <> _zone.Icon Then
                            Image.Tag = _zone.Icon
                            Image.Source = ConvertArrayToImage(myService.GetByteFromImage(_zone.Icon))
                        End If
                    End If
                ElseIf _macro IsNot Nothing Then
                    If ShowEtiquette And _macro.Nom <> Etiquette Then
                        Etiquette = _macro.Nom
                    End If
                End If
            End If

            Me.UpdateLayout()
        Catch ex As Exception
            MessageBox.Show("Error Refresh: " & ex.Message & vbCrLf, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
            _dt.Stop()
        End Try
    End Sub

    Public Sub dispatcherTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        If _FlagBlock = False Then TraiteRefresh()
    End Sub

    Private Sub uWidgetEmpty_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Try
            If _Show = False Then Exit Sub

            Select Case _type
                Case TypeOfWidget.Web
                    _Webbrowser.Width = Me.ActualWidth
                    _Webbrowser.Height = Me.ActualHeight
                Case TypeOfWidget.Rss
                    _RSS.Width = Me.ActualWidth
                    _RSS.Height = Me.ActualHeight
            End Select
        Catch ex As Exception
            MessageBox.Show("Erreur uWidgetEmpty_Loaded: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub uWidgetEmpty_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles Me.MouseLeave
        Try
            If _Show = False Then Exit Sub

            If e.LeftButton = MouseButtonState.Released Then Exit Sub

            Dim _NewPos As Point = e.GetPosition(Me)
            Dim _DiffY As Double = _oldposition.Y - _NewPos.Y
            Dim _DiffX As Double = _oldposition.X - _NewPos.X

            'Gesture Bas Haut 
            If _DiffY > 20 And (-20 < _DiffX < 20) Then
                If IsEmpty = True And _type = TypeOfWidget.Empty Then
                    Traite_Action_BasHaut()
                End If
                RaiseEvent GestureBasHaut(Me, e)
                Exit Sub
            End If
            'Gesture Haut Bas
            If _DiffY < -20 And (-20 < _DiffX < 20) Then
                If IsEmpty = True And _type = TypeOfWidget.Empty Then
                    Traite_Action_HautBas()
                End If
                RaiseEvent GestureHautBas(Me, e)
                Exit Sub
            End If
            'Gesture Gauche Droite
            If _DiffX < -20 And (-20 < _DiffY < 20) Then
                If IsEmpty = True And _type = TypeOfWidget.Empty Then
                    Traite_Action_GaucheDroite()
                End If
                RaiseEvent GestureGaucheDroite(Me, e)
                Exit Sub
            End If
            'Gesture Droite Gauche
            If _DiffX > 20 And (-20 < _DiffY < 20) Then
                If IsEmpty = True And _type = TypeOfWidget.Empty Then
                    Traite_Action_DroiteGauche()
                End If
                RaiseEvent GestureDroiteGauche(Me, e)
                Exit Sub
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur uWidgetEmpty_MouseLeave: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub Image_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Me.MouseLeftButtonUp
        Try
            If _Show = False Then Exit Sub

            Dim vDiff As TimeSpan = Now - _Down
            Dim _NewPos As Point = e.GetPosition(Me)
            Dim _DiffY As Double = _oldposition.Y - _NewPos.Y
            Dim _DiffX As Double = _oldposition.X - _NewPos.X

            'Gesture Bas Haut 
            If _DiffY > 20 And (-20 < _DiffX < 20) Then
                If IsEmpty = True And _type = TypeOfWidget.Empty Then
                    Traite_Action_BasHaut()
                End If
                RaiseEvent GestureBasHaut(Me, e)
                Exit Sub
            End If
            'Gesture Haut Bas
            If _DiffY < -20 And (-20 < _DiffX < 20) Then
                If IsEmpty = True And _type = TypeOfWidget.Empty Then
                    Traite_Action_HautBas()
                End If
                RaiseEvent GestureHautBas(Me, e)
                Exit Sub
            End If
            'Gesture Gauche Droite
            If _DiffX < -20 And (-20 < _DiffY < 20) Then
                If IsEmpty = True And _type = TypeOfWidget.Empty Then
                    Traite_Action_GaucheDroite()
                End If
                RaiseEvent GestureGaucheDroite(Me, e)
                Exit Sub
            End If
            'Gesture Droite Gauche
            If _DiffX > 20 And (-20 < _DiffY < 20) Then
                If IsEmpty = True And _type = TypeOfWidget.Empty Then
                    Traite_Action_DroiteGauche()
                End If
                RaiseEvent GestureDroiteGauche(Me, e)
                Exit Sub
            End If

            If vDiff.Seconds < 1 Then
                If IsEmpty = True And _type = TypeOfWidget.Empty Then
                    Traite_Action_OnClick()
                End If
                RaiseEvent Click(Me, e)
            Else
                If IsEmpty = True And _type = TypeOfWidget.Empty Then
                    Traite_Action_OnLongClick()
                End If
                RaiseEvent LongClick(Me, e)
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur Image_MouseLeftButtonUp: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

#Region "Gerer_Action_Gesture"
    Private Sub Traite_Action_OnClick()
        If _Show = False Then Exit Sub

        For Each _act As cWidget.Action In _Action_On_Click
            Dim _dev As HoMIDom.HoMIDom.TemplateDevice = myService.ReturnDeviceByID(IdSrv, _act.IdObject)
            If _dev IsNot Nothing Then
                Dim x As New HoMIDom.HoMIDom.DeviceAction
                x.Nom = _act.Methode
                If _act.Value.ToString <> "" Then
                    Dim param As New HoMIDom.HoMIDom.DeviceAction.Parametre
                    param.Value = _act.Value
                    x.Parametres.Add(param)
                End If

                _FlagBlock = True
                myService.ExecuteDeviceCommand(IdSrv, _act.IdObject, x)
                _FlagBlock = False

            Else
                Dim _mac As HoMIDom.HoMIDom.Macro = myService.ReturnMacroById(IdSrv, _act.IdObject)
                If _mac IsNot Nothing Then
                    Dim x As New HoMIDom.HoMIDom.DeviceAction
                    x.Nom = _act.Methode

                    _FlagBlock = True
                    myService.ExecuteDeviceCommand(IdSrv, _act.IdObject, x)
                    _FlagBlock = False
                Else
                    Dim _zon As HoMIDom.HoMIDom.Zone = myService.ReturnZoneByID(IdSrv, _act.IdObject)
                    If _zon IsNot Nothing Then
                        frmMere.ShowZone(_act.IdObject)
                    End If
                End If
            End If
        Next
    End Sub

    Private Sub Traite_Action_OnLongClick()
        If _Show = False Then Exit Sub

        For Each _act As cWidget.Action In _Action_On_LongClick
            Dim _dev As HoMIDom.HoMIDom.TemplateDevice = myService.ReturnDeviceByID(IdSrv, _act.IdObject)
            If _dev IsNot Nothing Then
                Dim x As New HoMIDom.HoMIDom.DeviceAction
                x.Nom = _act.Methode
                If _act.Value.ToString <> "" Then
                    Dim param As New HoMIDom.HoMIDom.DeviceAction.Parametre
                    param.Value = _act.Value
                    x.Parametres.Add(param)
                End If

                _FlagBlock = True
                myService.ExecuteDeviceCommand(IdSrv, _act.IdObject, x)
                _FlagBlock = False
            Else
                Dim _mac As HoMIDom.HoMIDom.Macro = myService.ReturnMacroById(IdSrv, _act.IdObject)
                If _mac IsNot Nothing Then
                    Dim x As New HoMIDom.HoMIDom.DeviceAction
                    x.Nom = _act.Methode
                    myService.ExecuteDeviceCommand(IdSrv, _act.IdObject, x)
                Else
                    Dim _zon As HoMIDom.HoMIDom.Zone = myService.ReturnZoneByID(IdSrv, _act.IdObject)
                    If _zon IsNot Nothing Then
                        frmMere.ShowZone(_act.IdObject)
                    End If
                End If
            End If
        Next
    End Sub

    Private Sub Traite_Action_HautBas()
        If _Show = False Then Exit Sub

        For Each _act As cWidget.Action In _Action_GestureHautBas
            Dim _dev As HoMIDom.HoMIDom.TemplateDevice = myService.ReturnDeviceByID(IdSrv, _act.IdObject)
            If _dev IsNot Nothing Then
                Dim x As New HoMIDom.HoMIDom.DeviceAction
                x.Nom = _act.Methode
                If _act.Value.ToString <> "" Then
                    Dim param As New HoMIDom.HoMIDom.DeviceAction.Parametre
                    param.Value = _act.Value
                    x.Parametres.Add(param)
                End If

                _FlagBlock = True
                myService.ExecuteDeviceCommand(IdSrv, _act.IdObject, x)
                _FlagBlock = False
            Else
                Dim _mac As HoMIDom.HoMIDom.Macro = myService.ReturnMacroById(IdSrv, _act.IdObject)
                If _mac IsNot Nothing Then
                    Dim x As New HoMIDom.HoMIDom.DeviceAction
                    x.Nom = _act.Methode

                    _FlagBlock = True
                    myService.ExecuteDeviceCommand(IdSrv, _act.IdObject, x)
                    _FlagBlock = False
                Else
                    Dim _zon As HoMIDom.HoMIDom.Zone = myService.ReturnZoneByID(IdSrv, _act.IdObject)
                    If _zon IsNot Nothing Then
                        frmMere.ShowZone(_act.IdObject)
                    End If
                End If
            End If
        Next
    End Sub

    Private Sub Traite_Action_BasHaut()
        If _Show = False Then Exit Sub

        For Each _act As cWidget.Action In _Action_GestureBasHaut
            Dim _dev As HoMIDom.HoMIDom.TemplateDevice = myService.ReturnDeviceByID(IdSrv, _act.IdObject)
            If _dev IsNot Nothing Then
                Dim x As New HoMIDom.HoMIDom.DeviceAction
                x.Nom = _act.Methode
                If _act.Value.ToString <> "" Then
                    Dim param As New HoMIDom.HoMIDom.DeviceAction.Parametre
                    param.Value = _act.Value
                    x.Parametres.Add(param)
                End If

                _FlagBlock = True
                myService.ExecuteDeviceCommand(IdSrv, _act.IdObject, x)
                _FlagBlock = False
            Else
                Dim _mac As HoMIDom.HoMIDom.Macro = myService.ReturnMacroById(IdSrv, _act.IdObject)
                If _mac IsNot Nothing Then
                    Dim x As New HoMIDom.HoMIDom.DeviceAction
                    x.Nom = _act.Methode

                    _FlagBlock = True
                    myService.ExecuteDeviceCommand(IdSrv, _act.IdObject, x)
                    _FlagBlock = False
                Else
                    Dim _zon As HoMIDom.HoMIDom.Zone = myService.ReturnZoneByID(IdSrv, _act.IdObject)
                    If _zon IsNot Nothing Then
                        frmMere.ShowZone(_act.IdObject)
                    End If
                End If
            End If
        Next
    End Sub

    Private Sub Traite_Action_GaucheDroite()
        If _Show = False Then Exit Sub

        For Each _act As cWidget.Action In _Action_GestureGaucheDroite
            Dim _dev As HoMIDom.HoMIDom.TemplateDevice = myService.ReturnDeviceByID(IdSrv, _act.IdObject)
            If _dev IsNot Nothing Then
                Dim x As New HoMIDom.HoMIDom.DeviceAction
                x.Nom = _act.Methode
                If _act.Value.ToString <> "" Then
                    Dim param As New HoMIDom.HoMIDom.DeviceAction.Parametre
                    param.Value = _act.Value
                    x.Parametres.Add(param)
                End If

                _FlagBlock = True
                myService.ExecuteDeviceCommand(IdSrv, _act.IdObject, x)
                _FlagBlock = False
            Else
                Dim _mac As HoMIDom.HoMIDom.Macro = myService.ReturnMacroById(IdSrv, _act.IdObject)
                If _mac IsNot Nothing Then
                    Dim x As New HoMIDom.HoMIDom.DeviceAction
                    x.Nom = _act.Methode

                    _FlagBlock = True
                    myService.ExecuteDeviceCommand(IdSrv, _act.IdObject, x)
                    _FlagBlock = False
                Else
                    Dim _zon As HoMIDom.HoMIDom.Zone = myService.ReturnZoneByID(IdSrv, _act.IdObject)
                    If _zon IsNot Nothing Then
                        frmMere.ShowZone(_act.IdObject)
                    End If
                End If
            End If
        Next
    End Sub

    Private Sub Traite_Action_DroiteGauche()
        If _Show = False Then Exit Sub

        For Each _act As cWidget.Action In _Action_GestureDroiteGauche
            Dim _dev As HoMIDom.HoMIDom.TemplateDevice = myService.ReturnDeviceByID(IdSrv, _act.IdObject)
            If _dev IsNot Nothing Then
                Dim x As New HoMIDom.HoMIDom.DeviceAction
                x.Nom = _act.Methode
                If _act.Value.ToString <> "" Then
                    Dim param As New HoMIDom.HoMIDom.DeviceAction.Parametre
                    param.Value = _act.Value
                    x.Parametres.Add(param)
                End If

                _FlagBlock = True
                myService.ExecuteDeviceCommand(IdSrv, _act.IdObject, x)
                _FlagBlock = False
            Else
                Dim _mac As HoMIDom.HoMIDom.Macro = myService.ReturnMacroById(IdSrv, _act.IdObject)
                If _mac IsNot Nothing Then
                    Dim x As New HoMIDom.HoMIDom.DeviceAction
                    x.Nom = _act.Methode

                    _FlagBlock = True
                    myService.ExecuteDeviceCommand(IdSrv, _act.IdObject, x)
                    _FlagBlock = False
                Else
                    Dim _zon As HoMIDom.HoMIDom.Zone = myService.ReturnZoneByID(IdSrv, _act.IdObject)
                    If _zon IsNot Nothing Then
                        frmMere.ShowZone(_act.IdObject)
                    End If
                End If
            End If
        Next
    End Sub
#End Region

    Private Sub Image_PreviewMouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Me.PreviewMouseDown
        _Down = Now
        _oldposition = e.GetPosition(Me)
    End Sub

    Private Sub ClosePopup()
        Popup1.IsOpen = False
    End Sub

#Region "Gestion des evenements provenant des popup"
    Private Sub FreeTouch(ByVal Touche As String)
        Try
            Dim x As New HoMIDom.HoMIDom.DeviceAction
            If _dev IsNot Nothing Then
                x.Nom = Touche
                x.Parametres = Nothing

                _FlagBlock = True
                myService.ExecuteDeviceCommand(IdSrv, Id, x)
                _FlagBlock = False

            End If
        Catch ex As Exception
            MessageBox.Show("Erreur FreeTouch: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
            _FlagBlock = False
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

                _FlagBlock = True
                myService.ExecuteDeviceCommand(IdSrv, Id, x)
                _FlagBlock = False

                If StkPopup.Children.Count > 0 Then
                    If Popup1.IsOpen = True Then
                        Popup1.IsOpen = False
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur ClickOn: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
            _FlagBlock = False
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

                _FlagBlock = True
                myService.ExecuteDeviceCommand(IdSrv, Id, x)
                _FlagBlock = False

                If StkPopup.Children.Count > 0 Then
                    If Popup1.IsOpen = True Then
                        Popup1.IsOpen = False
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur ClickOff: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
            _FlagBlock = False
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

                _FlagBlock = True
                myService.ExecuteDeviceCommand(IdSrv, Id, x)
                _FlagBlock = False
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur ValueChange: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
            _FlagBlock = False
        End Try
    End Sub

    Private Sub SendCommand(ByVal Commande As String)
        Try
            Dim x As New HoMIDom.HoMIDom.DeviceAction
            If _dev IsNot Nothing Then
                Dim retour As String = myService.TelecommandeSendCommand(IdSrv, _dev.ID, Commande)
                If retour <> 0 Then
                    MessageBox.Show(retour, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
                End If

                If StkPopup.Children.Count > 0 Then
                    If Popup1.IsOpen = True Then
                        Popup1.IsOpen = False
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur SendCommand: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
            _FlagBlock = False
        End Try
    End Sub

    Private Sub ChangeValue(ByVal Value As String)
        Try
            Dim _value As Object = Nothing
            Dim _flag As Boolean = False

            _CurrentValue = Nothing

            If _dev IsNot Nothing And Value <> "" Then
                If _dev.Type = HoMIDom.HoMIDom.Device.ListeDevices.GENERIQUEBOOLEEN Then
                    Try
                        _value = Value
                        _flag = True
                    Catch ex As Exception
                        MessageBox.Show("Erreur: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
                        _flag = False
                    End Try
                End If
                If _dev.Type = HoMIDom.HoMIDom.Device.ListeDevices.GENERIQUEVALUE Or HoMIDom.HoMIDom.Device.ListeDevices.TEMPERATURECONSIGNE Then
                    If IsNumeric(Value) Then
                        _value = Value
                        _flag = True
                    Else
                        MessageBox.Show("Erreur: La valeur saisie doit être numérique", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
                        _flag = False
                    End If
                End If
                If _dev.Type = HoMIDom.HoMIDom.Device.ListeDevices.GENERIQUESTRING Then
                    _value = Value
                    _flag = True
                End If

                If _flag = True Then
                    _FlagBlock = True
                    myService.ChangeValueOfDevice(IdSrv, _dev.ID, _value)
                    _FlagBlock = False
                End If
            End If

            If StkPopup.Children.Count > 0 Then
                If Popup1.IsOpen = True Then
                    Popup1.IsOpen = False
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur ChangeValue: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
            _FlagBlock = False
        End Try
    End Sub

    Private Sub AudioPlay()
        Try
            Dim x As New HoMIDom.HoMIDom.DeviceAction
            If _dev IsNot Nothing Then
                x.Nom = "Play"
            End If
            x.Parametres = Nothing

            myService.ExecuteDeviceCommand(IdSrv, Id, x)

            If StkPopup.Children.Count > 0 Then
                If Popup1.IsOpen = True Then
                    Popup1.IsOpen = False
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur AudioPlay: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub AudioPause()
        Try
            Dim x As New HoMIDom.HoMIDom.DeviceAction
            If _dev IsNot Nothing Then
                x.Nom = "PauseAudio"
            End If
            x.Parametres = Nothing

            myService.ExecuteDeviceCommand(IdSrv, Id, x)

            If StkPopup.Children.Count > 0 Then
                If Popup1.IsOpen = True Then
                    Popup1.IsOpen = False
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur AudioPause: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub AudioStop()
        Try
            Dim x As New HoMIDom.HoMIDom.DeviceAction
            If _dev IsNot Nothing Then
                x.Nom = "StopAudio"
            End If
            x.Parametres = Nothing

            myService.ExecuteDeviceCommand(IdSrv, Id, x)

            If StkPopup.Children.Count > 0 Then
                If Popup1.IsOpen = True Then
                    Popup1.IsOpen = False
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur AudioStop: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub AudioAvance()

    End Sub

    Private Sub AudioRecul()

    End Sub

    Private Sub AudioNextChap()
        Try
            Dim x As New HoMIDom.HoMIDom.DeviceAction
            If _dev IsNot Nothing Then
                x.Nom = "NextAudio"
            End If
            x.Parametres = Nothing

            myService.ExecuteDeviceCommand(IdSrv, Id, x)

            If StkPopup.Children.Count > 0 Then
                If Popup1.IsOpen = True Then
                    Popup1.IsOpen = False
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur AudioNextChap: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub AudioPreviousChap()
        Try
            Dim x As New HoMIDom.HoMIDom.DeviceAction
            If _dev IsNot Nothing Then
                x.Nom = "PreviousAudio"
            End If
            x.Parametres = Nothing

            myService.ExecuteDeviceCommand(IdSrv, Id, x)

            If StkPopup.Children.Count > 0 Then
                If Popup1.IsOpen = True Then
                    Popup1.IsOpen = False
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur AudioPreviousChap: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub AudioMute()
        Try
            Dim x As New HoMIDom.HoMIDom.DeviceAction
            If _dev IsNot Nothing Then
                x.Nom = "VolumeMuteAudio"
            End If
            x.Parametres = Nothing

            myService.ExecuteDeviceCommand(IdSrv, Id, x)

            If StkPopup.Children.Count > 0 Then
                If Popup1.IsOpen = True Then
                    Popup1.IsOpen = False
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur AudioMute: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub AudioVolumeUp()
        Try
            Dim x As New HoMIDom.HoMIDom.DeviceAction
            If _dev IsNot Nothing Then
                x.Nom = "VolumeUpAudio"
            End If
            x.Parametres = Nothing

            myService.ExecuteDeviceCommand(IdSrv, Id, x)

            If StkPopup.Children.Count > 0 Then
                If Popup1.IsOpen = True Then
                    Popup1.IsOpen = False
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur AudioVolumeUp: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub AudioVolumeDown()
        Try
            Dim x As New HoMIDom.HoMIDom.DeviceAction
            If _dev IsNot Nothing Then
                x.Nom = "VolumeDownAudio"
            End If
            x.Parametres = Nothing

            myService.ExecuteDeviceCommand(IdSrv, Id, x)

            If StkPopup.Children.Count > 0 Then
                If Popup1.IsOpen = True Then
                    Popup1.IsOpen = False
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur AudioVolumeDown: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
#End Region

    Private Sub Stk1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Me.MouseDown
        Try
            If _Show = False Then Exit Sub

            If _ModeEdition Then
                Try
                    Dim x As New WWidgetProperty
                    x.Objet = Me
                    x.Owner = frmMere
                    x.ShowDialog()

                    If x.DialogResult.HasValue Then

                        For i As Integer = 0 To _ListElement.Count - 1
                            If _ListElement.Item(i).Uid = Me.Uid And _ListElement.Item(i).ZoneId = Me.ZoneId Then
                                _ListElement.Item(i) = x.Objet
                                Exit For
                            End If
                        Next

                        x.Close()

                        frmMere.ShowZone(frmMere._CurrentIdZone)
                    Else
                        x.Close()
                    End If

                Catch ex As Exception
                    MessageBox.Show("Erreur Stk1_MouseDown: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
                End Try
            Else
                If IsEmpty = False Then
                    If e.ClickCount > 1 Then Exit Sub
                    If _zone IsNot Nothing Then
                        RaiseEvent ShowZone(_zone.ID)
                        Exit Sub
                    End If
                    If _macro IsNot Nothing Then
                        myService.RunMacro(IdSrv, _macro.ID)
                        Exit Sub
                    End If
                    If StkPopup.Children.Count > 0 Then
                        Select Case _dev.Type
                            Case HoMIDom.HoMIDom.Device.ListeDevices.GENERIQUEBOOLEEN
                                If _CanEditValue = True Then
                                    If Popup1.IsOpen = False Then
                                        Popup1.IsOpen = True
                                    Else
                                        Popup1.IsOpen = False
                                    End If
                                End If
                            Case HoMIDom.HoMIDom.Device.ListeDevices.GENERIQUESTRING
                                If _CanEditValue = True Then
                                    If Popup1.IsOpen = False Then
                                        Popup1.IsOpen = True
                                    Else
                                        Popup1.IsOpen = False
                                    End If
                                End If
                            Case HoMIDom.HoMIDom.Device.ListeDevices.GENERIQUEVALUE
                                If _CanEditValue = True Then
                                    If Popup1.IsOpen = False Then
                                        Popup1.IsOpen = True
                                    Else
                                        Popup1.IsOpen = False
                                    End If
                                End If
                            Case HoMIDom.HoMIDom.Device.ListeDevices.TEMPERATURECONSIGNE
                                If _CanEditValue = True Then
                                    If Popup1.IsOpen = False Then
                                        Popup1.IsOpen = True
                                    Else
                                        Popup1.IsOpen = False
                                    End If
                                End If
                            Case Else
                                If Popup1.IsOpen = False Then
                                    Popup1.IsOpen = True
                                Else
                                    Popup1.IsOpen = False
                                End If
                        End Select
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur Stk1_MouseDown: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub uWidgetEmpty_SizeChanged(ByVal sender As Object, ByVal e As System.Windows.SizeChangedEventArgs) Handles Me.SizeChanged
        Try
            If _Show = False Then Exit Sub

            Select Case _type
                Case TypeOfWidget.Web
                    _Webbrowser.Width = Me.ActualWidth
                    _Webbrowser.Height = Me.ActualHeight - 20
                Case TypeOfWidget.Rss
                    _RSS.Width = Me.ActualWidth
                    _RSS.Height = Me.ActualHeight - 20
                Case TypeOfWidget.Meteo
                    Exit Sub
                Case TypeOfWidget.KeyPad
                    Exit Sub
                Case TypeOfWidget.Label
                    Exit Sub
            End Select

            If ShowEtiquette And ShowPicture And Lbl.ActualHeight > 0 And Me.ActualHeight > 0 Then
                Dim H As Double = Me.ActualHeight - Lbl.ActualHeight
                If H > 0 Then Image.Height = H
            End If
            If ShowEtiquette = False And ShowPicture And Me.ActualHeight > 0 Then
                Image.Height = Me.ActualHeight
            End If
            If ShowStatus And ShowPicture And LblStatus.ActualHeight > 0 And Me.ActualWidth > 0 Then
                Image.Width = Me.ActualWidth - LblStatus.ActualWidth
            End If
            If ShowStatus = False And ShowPicture Then
                Image.Width = Me.ActualWidth
            End If
            If ShowStatus Then
                If Image.Width < Image.Height Then Image.Height = Image.Width
                If Image.Height < Image.Width Then Image.Width = Image.Height
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur uWidgetEmpty_SizeChanged: " & ex.ToString, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        Try
            MyBase.Finalize()
        Catch ex As Exception
            MessageBox.Show("Erreur uWidgetEmpty_Finalize: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub KeyPadOK(ByVal Value As Integer)
        If IsConnect Then
            If _IDKeyPad <> "" Then
                myService.ChangeValueOfDevice(IdSrv, _IDKeyPad, Value)
            End If
        End If
    End Sub

    Private Sub uWidgetEmpty_Unloaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Unloaded
        Try
            If _dt IsNot Nothing Then
                _dt.Stop()
                RemoveHandler _dt.Tick, AddressOf dispatcherTimer_Tick
                _dt = Nothing
            End If
            Image.Source = Nothing
            _Webbrowser = Nothing
            _RSS = Nothing
            _METEO = Nothing
            _KeyPad = Nothing
        Catch ex As Exception
            MessageBox.Show("Erreur uWidgetEmpty_Unloaded: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try

    End Sub

    Private Function GetOnOffPicture(ByVal filename As String, ByVal value As Single) As String
        Try
            ' On recherche s'il existe des images de type double état (ON/OFF) et si oui:
            ' - affiche l 'image xxxx_on.jpg si la valeur numérique est différent de zéro (ou False)
            ' - affiche l 'image xxxx_off.jpg si la valeur numérique est de zéro (ou False)
            Dim _file As String = filename.ToLower
            If _file.Contains("_on") = True Or _file.Contains("_off") = True Then
                If value = 0 Then
                    GetOnOffPicture = _file.Replace("_on", "_off")
                Else
                    GetOnOffPicture = _file.Replace("_off", "_on")
                End If
            Else
                GetOnOffPicture = _file
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur GetOnOffPicture: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
            Return filename
        End Try
    End Function

End Class
