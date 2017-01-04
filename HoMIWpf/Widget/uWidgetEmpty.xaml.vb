Imports System.Windows.Threading
Imports System.Reflection
Imports System.Threading
Imports WpfAnimatedGif
Imports System.Windows.Media.Animation
Imports System.IO
Imports STRGS = Microsoft.VisualBasic.Strings


Public Class uWidgetEmpty
    Public Enum TypeOfWidget
        Empty = 0
        Web = 1
        Media = 2
        Rss = 3
        Meteo = 4
        KeyPad = 5
        Label = 6
        Camera = 7
        Volet = 8
        Moteur = 9
        Image = 10
        Prise = 11
        Gauge = 12
        Device = 99
    End Enum

    'Variables
    Dim _BorderThickness As Double = Nothing
    Dim _CornerRadius As Double = 6
    Dim _ColorBorder As SolidColorBrush = Brushes.Transparent
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
    Dim _ColorEtiquette As SolidColorBrush = Brushes.White
    Dim _EtiquetteAlignement As Windows.HorizontalAlignment = Windows.HorizontalAlignment.Center
    Dim _MaJEtiqFromServer As Boolean = True
    Dim _X As Double = 0
    Dim _Y As Double = 0
    Dim _Rotation As Double = 0
    Dim _RotationX As Double = 0
    Dim _RotationY As Double = 0
    Dim _ModeEdition As Boolean '=True si on est en mode edition
    Dim _ShowPicture As Boolean = True
    Dim _Picture As String = ""
    Dim _ImageBackGround As String
    Dim _GarderProportionImage As Boolean = True 'True si on veut garder proportion Width=Height de l'image
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
    Dim _Fondu As Boolean = True
    Dim _IsCommun As Boolean = False 'indique si le widget est commun à toutes les pages
    Dim _Zindex As Integer = 0

    'Variables Widget Web/Camera
    Dim _URL As String = ""
    Dim _Webbrowser As uHttp = Nothing
    Dim _Camera As uCamera = Nothing
    Dim _ListHttpBtn As New List(Of uHttp.ButtonHttp)
    Dim _RefreshHttp As Integer = 0

    'Variables Widget RSS
    Dim _URLRss As String = ""
    Dim _RSS As uRSS = Nothing

    'Variables Widget Meteo
    Dim _IDMeteo As String = ""
    Dim _METEO As uWMeteo = Nothing

    'Variables Widget Volet
    Dim _VOLET As uVolet = Nothing

    'Variables Widget KeyPad
    Dim _IDKeyPad As String = ""
    Dim _ShowPassWord As Boolean = True
    Dim _ClearAfterEnter As Boolean = False
    Dim _ShowClavier As Boolean = True
    Dim _KeyPad As uKeyPad = Nothing

    'Variable Widget Moteur
    Dim _MOTEUR As uMoteur = Nothing

    'Variable Widget Prise
    Dim _PRISE As uPrise = Nothing

    'Variables Widget Gauge
    Dim _Gauge As uGauge = Nothing

    'Variable Min/Max
    Dim _Min As Integer
    Dim _Max As Integer

    'Variable Template
    Dim _IsUseForTemplate As Boolean = False  'utilisé pour un template

    'Event
    Public Event Click(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
    Public Event LongClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
    Public Event GestureHautBas(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
    Public Event GestureBasHaut(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
    Public Event GestureGaucheDroite(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
    Public Event GestureDroiteGauche(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
    Public Event ShowZone(ByVal zoneid As String)
    Public Event ShowTemplate(Templateid As String, Deviceid As String)

    'Animation
    Private myStoryboard As Storyboard

#Region "Property"
    Public Overloads Property BorderThickness As Double
        Get
            Return _BorderThickness
        End Get
        Set(value As Double)
            _BorderThickness = value
            Border1.BorderThickness = New Thickness(value)
        End Set
    End Property

    Public Property CornerRadius As Double
        Get
            Return _CornerRadius
        End Get
        Set(value As Double)
            _CornerRadius = value
            Border1.CornerRadius = New CornerRadius(value)
        End Set
    End Property

    Public Property ColorBorder As SolidColorBrush
        Get
            Return _ColorBorder
        End Get
        Set(value As SolidColorBrush)
            _ColorBorder = value
            Border1.BorderBrush = value
        End Set
    End Property

    Public Property ZIndex As Integer
        Get
            Return _Zindex
        End Get
        Set(value As Integer)
            _Zindex = value
        End Set
    End Property

    Public Property IsCommun As Boolean
        Get
            Return _IsCommun
        End Get
        Set(ByVal value As Boolean)
            _IsCommun = value
        End Set
    End Property

    Public Property Show As Boolean
        Get
            Return _Show
        End Get
        Set(ByVal value As Boolean)
            _Show = value
        End Set
    End Property

    Public Property Fondu As Boolean
        Get
            Return _Fondu
        End Get
        Set(ByVal value As Boolean)
            _Fondu = value
        End Set
    End Property

    Public Property Id As String
        Get
            Return _Id
        End Get
        Set(ByVal value As String)
            Try
                _Id = value
                If _Show = False Then Exit Property

                If IsConnect = True And String.IsNullOrEmpty(value) = False Then
                    If frmMere.MaJWidgetFromServer Then
                        _dev = myService.ReturnDeviceByID(IdSrv, _Id)
                    Else
                        _dev = ReturnDeviceById(_Id)
                    End If
                    _macro = myService.ReturnMacroById(IdSrv, _Id)
                    _zone = myService.ReturnZoneByID(IdSrv, _Id)

                    IsEmpty = True
                    StkPopup.Children.Clear()

                    If _dev IsNot Nothing Then
                        If MaJEtiquetteFromServeur Then Etiquette = _dev.Name
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
                                x.ShowBtnVolume = False
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
                                AddHandler x.SetFile, AddressOf AudioSetFile

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
                                IDMeteo = _Id
                                StkTool.Children.Add(_METEO)

                            Case HoMIDom.HoMIDom.Device.ListeDevices.MULTIMEDIA
                                ShowStatus = False
                                _ShowValue = False

                                '    Dim x As New uTelecommande(_Id)
                                '    AddHandler x.SendCommand, AddressOf SendCommand
                                '    StkPopup.Children.Add(x)
                                'Case HoMIDom.HoMIDom.Device.ListeDevices.PLUIECOURANT
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
                            LoadPicture()
                        End If
                    ElseIf _macro IsNot Nothing Then
                        If MaJEtiquetteFromServeur Then Etiquette = _macro.Nom
                        ShowStatus = False
                        IsEmpty = False
                    ElseIf _zone IsNot Nothing Then
                        If MaJEtiquetteFromServeur Then Etiquette = _zone.Name
                        Image.Source = ConvertArrayToImage(myService.GetByteFromImage(_zone.Icon))
                        ShowStatus = False
                        _Picture = _zone.Icon
                        IsEmpty = False
                    End If
                End If

                TraiteRefresh()
            Catch ex As Exception
                AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.id.set: " & ex.Message, "Erreur", " uWidgetEmpty.id.set")
            End Try
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
                    Case TypeOfWidget.Camera
                        StkEmptyetDevice.Visibility = Windows.Visibility.Collapsed
                        StkTool.Visibility = Windows.Visibility.Visible
                        _Camera = New uCamera
                        StkTool.Children.Add(_Camera)
                    Case TypeOfWidget.Volet
                        StkEmptyetDevice.Visibility = Windows.Visibility.Collapsed
                        StkTool.Visibility = Windows.Visibility.Visible
                        _VOLET = New uVolet
                        StkTool.Children.Add(_VOLET)
                    Case TypeOfWidget.Moteur
                        ShowPicture = False
                        _MOTEUR = New uMoteur
                        StkReplaceImage.Children.Add(_MOTEUR)
                    Case TypeOfWidget.Prise
                        ShowPicture = False
                        ShowStatus = False
                        _PRISE = New uPrise
                        StkReplaceImage.Children.Add(_PRISE)
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

                        If _Show = False Then Exit Property

                        StkTool.Children.Add(_METEO)
                    Case TypeOfWidget.KeyPad
                        StkEmptyetDevice.Visibility = Windows.Visibility.Collapsed
                        StkTool.Visibility = Windows.Visibility.Visible

                        _KeyPad = New uKeyPad
                        AddHandler _KeyPad.KeyPadOk, AddressOf KeyPadOK
                        _KeyPad.Width = Double.NaN
                        _KeyPad.Height = Double.NaN

                        StkTool.Children.Add(_KeyPad)
                    Case TypeOfWidget.Label
                        ColorBackGround = Brushes.Transparent
                        Lbl.MinWidth = 100
                        StkEmptyetDevice.Visibility = Windows.Visibility.Collapsed
                        StkTool.Visibility = Windows.Visibility.Collapsed
                        ShowPicture = False
                    Case TypeOfWidget.Image
                        MaJEtiquetteFromServeur = False
                        ColorBackGround = Brushes.Transparent
                        StkTool.Visibility = Windows.Visibility.Collapsed
                        ShowStatus = False
                        ShowPicture = True
                        ShowEtiquette = False
                    Case TypeOfWidget.Gauge
                        CanEditValue = False
                        StkEmptyetDevice.Visibility = Windows.Visibility.Collapsed
                        StkTool.Visibility = Windows.Visibility.Visible
                        _Gauge = New uGauge("")
                        If _Show = False Then Exit Property
                        StkTool.Children.Add(_Gauge)
                    Case Else

                End Select

                _dt = New DispatcherTimer()
                _dt.Interval = New TimeSpan(0, 0, _Refresh)
                AddHandler _dt.Tick, AddressOf dispatcherTimer_Tick
                _dt.Start()
            Catch ex As Exception
                AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.Type: " & ex.Message, "Erreur", " uWidgetEmpty.Type")
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
                    If MaJEtiquetteFromServeur Then Etiquette = _dev.Name
                End If
                If _zone IsNot Nothing Then
                    If MaJEtiquetteFromServeur Then Etiquette = _zone.Name
                End If
                If _macro IsNot Nothing Then
                    If MaJEtiquetteFromServeur Then Etiquette = _macro.Nom
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
            If MaJEtiquetteFromServeur = False Then
                Lbl.Content = TraiteBalise(_Etiquette)
            Else
                Lbl.Content = _Etiquette
            End If
        End Set
    End Property

    ''' <summary>
    ''' Mettre à jour l'étiquette automatiquement depuis le serveur
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MaJEtiquetteFromServeur As Boolean
        Get
            Return _MaJEtiqFromServer
        End Get
        Set(ByVal value As Boolean)
            _MaJEtiqFromServer = value
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
                AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.TailleEtiquette Set: " & ex.Message, "Erreur", " uWidgetEmpty.TailleEtiquette Set")
            End Try
        End Set
    End Property

    Public Property ColorEtiquette As SolidColorBrush
        Get
            Return _ColorEtiquette
        End Get
        Set(ByVal value As SolidColorBrush)
            _ColorEtiquette = value
            If _Show = False Then Exit Property
            Lbl.Foreground = value
        End Set
    End Property

    Public Property EtiquetteAlignement As Windows.HorizontalAlignment
        Get
            Return _EtiquetteAlignement
        End Get
        Set(value As Windows.HorizontalAlignment)
            Try
                _EtiquetteAlignement = value
                Lbl.HorizontalContentAlignment = _EtiquetteAlignement
            Catch ex As Exception
                _EtiquetteAlignement = Windows.HorizontalAlignment.Center
                Lbl.HorizontalContentAlignment = _EtiquetteAlignement
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

    Public Property RotationX As Double
        Get
            Return _RotationX
        End Get
        Set(ByVal value As Double)
            _RotationX = value
            If _Show = False Then Exit Property

            If Me.Parent IsNot Nothing Then
                Dim x As ContentControl = Me.Parent
                Dim Trg As New TransformGroup
                Dim Rot As New SkewTransform(_RotationX, _RotationY)
                Trg.Children.Add(Rot)
                x.RenderTransform = Trg
            End If
        End Set
    End Property

    Public Property RotationY As Double
        Get
            Return _RotationY
        End Get
        Set(ByVal value As Double)
            _RotationY = value
            If _Show = False Then Exit Property

            If Me.Parent IsNot Nothing Then
                Dim x As ContentControl = Me.Parent
                Dim Trg As New TransformGroup
                Dim Rot As New SkewTransform(_RotationX, _RotationY)
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
                    Image.Tag = _Picture
                    LoadPicture()
                End If
            Catch ex As Exception
                AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.Set Picture: " & ex.Message, "Erreur", " uWidgetEmpty.Set Picture")
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

    Public Property GarderProportionImage As Boolean
        Get
            Return _GarderProportionImage
        End Get
        Set(value As Boolean)
            _GarderProportionImage = value

            If _GarderProportionImage Then
                If ShowPicture Then If Image.Width < Image.Height Then Image.Height = Image.Width
                If ShowPicture Then If Image.Height < Image.Width Then Image.Width = Image.Height
                If _MOTEUR IsNot Nothing Then If _MOTEUR.Width < _MOTEUR.Height Then _MOTEUR.Height = _MOTEUR.Width
                If _MOTEUR IsNot Nothing Then If _MOTEUR.Height < _MOTEUR.Width Then _MOTEUR.Width = _MOTEUR.Height
                If _PRISE IsNot Nothing Then If _PRISE.Width < _PRISE.Height Then _PRISE.Height = _PRISE.Width
                If _PRISE IsNot Nothing Then If _PRISE.Height < _PRISE.Width Then _PRISE.Width = _PRISE.Height
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
                If _dt IsNot Nothing Then
                    _dt.Stop()
                    _dt.Interval = New TimeSpan(0, 0, _Refresh)
                    _dt.Start()
                End If
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

#Region "Property Template"
    Public Property IsUseForTemplate As Boolean
        Get
            Return _IsUseForTemplate
        End Get
        Set(value As Boolean)
            _IsUseForTemplate = value
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
                If My.Computer.Network.IsAvailable = True Then
                    If _Webbrowser IsNot Nothing And _type = TypeOfWidget.Web Then
                        _Webbrowser.URL = _URL
                    ElseIf _Camera IsNot Nothing And _type = TypeOfWidget.Camera Then
                        _Camera.URL = _URL
                    End If
                End If
            Catch ex As Exception
                AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.Empty.URL: " & ex.Message, "Erreur", " uWidgetEmpty.Empty.URL")
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
            If _Camera IsNot Nothing Then _Camera.ListButton = value
        End Set
    End Property

    Public Property HttpRefresh As Integer
        Get
            Return _RefreshHttp
        End Get
        Set(ByVal value As Integer)
            _RefreshHttp = value

            If _Show = False Then Exit Property

            If _Webbrowser IsNot Nothing Then _Webbrowser.Refresh = value
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

            If _METEO IsNot Nothing Then _METEO.ID = value
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

    Public Property ShowPassWord As Boolean
        Get
            Return _ShowPassWord
        End Get
        Set(ByVal value As Boolean)
            _ShowPassWord = value
            If _KeyPad IsNot Nothing Then
                _KeyPad.ShowPassWord = value
            End If
        End Set
    End Property

    Public Property ClearAfterEnter As Boolean
        Get
            Return _ClearAfterEnter
        End Get
        Set(ByVal value As Boolean)
            _ClearAfterEnter = value
            If _KeyPad IsNot Nothing Then
                _KeyPad.ClearAfterEnter = value
            End If
        End Set
    End Property

    Public Property ShowClavier As Boolean
        Get
            Return _ShowClavier
        End Get
        Set(ByVal value As Boolean)
            _ShowClavier = value
            If _KeyPad IsNot Nothing Then
                _KeyPad.ShowClavier = value
            End If
        End Set
    End Property
#End Region

#Region "Property Min/Max"
    Public Property Min As Integer
        Get
            Return _Min
        End Get
        Set(value As Integer)
            _Min = value

            If _MOTEUR IsNot Nothing Then _MOTEUR.Min = _Min
        End Set
    End Property

    Public Property Max As Integer
        Get
            Return _Max
        End Get
        Set(value As Integer)
            _Max = value

            If _MOTEUR IsNot Nothing Then _MOTEUR.Max = _Max
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

            Dim myDoubleAnimation As New DoubleAnimation()
            myDoubleAnimation.From = 0.0
            myDoubleAnimation.To = 1.0
            myDoubleAnimation.Duration = New Duration(TimeSpan.FromSeconds(1))

            myStoryboard = New Storyboard()
            myStoryboard.Children.Add(myDoubleAnimation)
            Storyboard.SetTarget(myDoubleAnimation, Me)
            Storyboard.SetTargetProperty(myDoubleAnimation, New PropertyPath(UserControl.OpacityProperty))
        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.New: " & ex.Message, "Erreur", " uWidgetEmpty.New")
        End Try
    End Sub

    Private Sub TraiteRefresh()
        Try
            Dim FlagVisu As Boolean = False

            If _ModeEdition = False And IsConnect Then
                If _Visuel.Count > 0 Then
                    For Each _ElmtVisu As cWidget.Visu In _Visuel
                        If frmMere.MaJWidgetFromServer Then
                            If IsConnect Then
                                _dev = myService.ReturnDeviceByID(IdSrv, _ElmtVisu.IdObject)
                            End If
                        Else
                            _dev = ReturnDeviceById(_ElmtVisu.IdObject)
                        End If

                        If _dev IsNot Nothing Then
                            Dim retour As Object = CallByName(_dev, _ElmtVisu.Propriete, CallType.Get)

                            'If _dev.Type <> HoMIDom.HoMIDom.Device.ListeDevices.METEO Then
                            Select Case _ElmtVisu.Operateur
                                Case cWidget.Operateur.Diff
                                    If retour <> _ElmtVisu.Value Then
                                        If String.IsNullOrEmpty(_ElmtVisu.Text) = False Then Etiquette = _ElmtVisu.Text
                                        If Image.Tag <> _ElmtVisu.Image Then
                                            Picture = _ElmtVisu.Image
                                            FlagVisu = True
                                        End If
                                    End If
                                Case cWidget.Operateur.Equal
                                    If retour = _ElmtVisu.Value Then
                                        If String.IsNullOrEmpty(_ElmtVisu.Text) = False Then Etiquette = _ElmtVisu.Text
                                        If Image.Tag <> _ElmtVisu.Image Then
                                            Picture = _ElmtVisu.Image
                                            FlagVisu = True
                                        End If
                                    End If
                                Case cWidget.Operateur.Inferieur
                                    If retour < _ElmtVisu.Value Then
                                        If String.IsNullOrEmpty(_ElmtVisu.Text) = False Then Etiquette = _ElmtVisu.Text
                                        If Image.Tag <> _ElmtVisu.Image Then
                                            Picture = _ElmtVisu.Image
                                            FlagVisu = True
                                        End If
                                    End If
                                Case cWidget.Operateur.InferieurEqual
                                    If retour <= _ElmtVisu.Value Then
                                        If String.IsNullOrEmpty(_ElmtVisu.Text) = False Then Etiquette = _ElmtVisu.Text
                                        If Image.Tag <> _ElmtVisu.Image Then
                                            Picture = _ElmtVisu.Image
                                            FlagVisu = True
                                        End If
                                    End If
                                Case cWidget.Operateur.Superieur
                                    If retour > _ElmtVisu.Value Then
                                        If String.IsNullOrEmpty(_ElmtVisu.Text) = False Then Etiquette = _ElmtVisu.Text
                                        If Image.Tag <> _ElmtVisu.Image Then
                                            Picture = _ElmtVisu.Image
                                            FlagVisu = True
                                        End If
                                    End If
                                Case cWidget.Operateur.SuperieurEqual
                                    If retour >= _ElmtVisu.Value Then
                                        If String.IsNullOrEmpty(_ElmtVisu.Text) = False Then Etiquette = _ElmtVisu.Text
                                        If Image.Tag <> _ElmtVisu.Image Then
                                            Picture = _ElmtVisu.Image
                                            FlagVisu = True
                                        End If
                                    End If
                            End Select
                        End If
                    Next
                End If

                If String.IsNullOrEmpty(_Id) = False Then
                    If frmMere.MaJWidgetFromServer Then
                        If IsConnect Then
                            _dev = myService.ReturnDeviceByID(IdSrv, _Id)
                        End If
                    Else
                        _dev = ReturnDeviceById(_Id)
                    End If

                    If _dev IsNot Nothing Then
                        If ShowEtiquette And _dev.Name <> Etiquette Then
                            If ShowEtiquette And MaJEtiquetteFromServeur Then Etiquette = _dev.Name
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

                        If Me.Type = TypeOfWidget.Volet And _VOLET IsNot Nothing Then
                            _VOLET.Value = _dev.Value
                        End If
                        If Me.Type = TypeOfWidget.Gauge And _Gauge IsNot Nothing Then
                            _Gauge.Value = _dev.Value
                        End If
                        If Me.Type = TypeOfWidget.Moteur And _MOTEUR IsNot Nothing Then
                            If _dev.Value.GetType.ToString.ToUpper.Contains("BOOLEAN") Then
                                If _dev.Value Then
                                    _MOTEUR.Value = 1
                                Else
                                    _MOTEUR.Value = 0
                                End If
                            Else
                                _MOTEUR.Value = _dev.Value
                            End If

                        End If
                        If Me.Type = TypeOfWidget.Prise And _PRISE IsNot Nothing Then
                            If _dev.Value.GetType.ToString.ToUpper.Contains("BOOLEAN") Then
                                If _dev.Value Then
                                    _PRISE.Value = 1
                                Else
                                    _PRISE.Value = 0
                                End If
                            Else
                                _PRISE.Value = _dev.Value
                            End If

                        End If

                        If _ShowValue Then
                            If _CurrentValue <> _dev.Value Then
                                LblStatus.Content = _dev.Value & _dev.Unit
                                _CurrentValue = _dev.Value
                                If FlagVisu = False Then LoadPicture()
                            End If
                        End If

                        If _IsVariation Then
                            If StkPopup.Children.Count = 2 Then
                                Dim x2 As uVariateur = StkPopup.Children.Item(1)
                                x2.Value = _dev.Value
                            End If
                        End If

                    ElseIf _zone IsNot Nothing Then
                        If ShowEtiquette And _zone.Name <> Etiquette Then
                            If MaJEtiquetteFromServeur Then Etiquette = _zone.Name
                            If Image.Tag <> _zone.Icon Then
                                Image.Tag = _zone.Icon
                                If IsConnect Then Image.Source = ConvertArrayToImage(myService.GetByteFromImage(_zone.Icon))
                            End If
                        End If
                    ElseIf _macro IsNot Nothing Then
                        If ShowEtiquette And _macro.Nom <> Etiquette Then
                            If MaJEtiquetteFromServeur Then Etiquette = _macro.Nom
                        End If
                    End If
                End If
            End If

            If Etiquette.ToUpper <> "<SYSTEM_ICO_METEO>" Then
                Lbl.Content = TraiteBalise(Etiquette)
            Else
                Dim chm As String = ""
                If File.Exists(_MonRepertoire & "\Images\Meteo\" & TraiteBalise(Etiquette) & ".png") = True Then
                    chm = _MonRepertoire & "\Images\Meteo\" & TraiteBalise(Etiquette) & ".png"
                Else
                    chm = _MonRepertoire & "\Images\Meteo\na.png"
                End If
                If Image.Tag <> chm Then
                    ImageBehavior.SetAnimatedSource(Image, New BitmapImage(New Uri(chm)))
                    Image.Tag = chm
                End If
                Lbl.Content = frmMere.Ville
            End If

            Me.UpdateLayout()
        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.TraiteRefresh: " & ex.Message, "Erreur", " uWidgetEmpty.TraiteRefresh")
            _dt.Stop()
        End Try
    End Sub

    Public Sub dispatcherTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        Try
            If _FlagBlock = False Then TraiteRefresh()
        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur dispatcherTimer_Tick: " & ex.Message, "Erreur", " dispatcherTimer_Tick")
        End Try
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
                Case TypeOfWidget.Camera
                    _Camera.Width = Me.ActualWidth
                    _Camera.Height = Me.ActualHeight
                Case TypeOfWidget.Gauge
                    _Gauge.Width = Me.ActualWidth
                    _Gauge.Height = Me.ActualHeight
                Case TypeOfWidget.Volet
                    _VOLET.Width = Me.ActualWidth
                    _VOLET.Height = Me.ActualHeight - 30
                Case TypeOfWidget.Moteur
                    ShowPicture = False
                Case TypeOfWidget.Prise
                    ShowPicture = False
                Case TypeOfWidget.Meteo
                    _METEO.Width = Double.NaN
                    _METEO.Height = Double.NaN
                    _METEO.UpdateLayout()

                    Me.Width = Double.NaN
                    Me.Height = Double.NaN
                    Me.UpdateLayout()

                    MyBase.Width = Me.ActualWidth
                    MyBase.Height = Me.ActualHeight
                    MyBase.UpdateLayout()

                    If Me.Parent IsNot Nothing Then
                        Dim x As ContentControl = Me.Parent
                        x.Height = Me.ActualHeight
                        x.Width = Me.ActualWidth
                        x.UpdateLayout()
                    End If

            End Select

            If _Fondu And myStoryboard IsNot Nothing Then
                myStoryboard.Begin()
                myStoryboard = Nothing
            End If

        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.Loaded: " & ex.Message, "Erreur", " uWidgetEmpty.Loaded")
        End Try
    End Sub

    Private Sub uWidgetEmpty_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles Me.MouseLeave
        Try
            If _Show = False Then Exit Sub

            If e.LeftButton = MouseButtonState.Released Then Exit Sub

            If Popup1.IsOpen = True Then
                Popup1.IsOpen = False
            End If

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
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.MouseLeave: " & ex.Message, "Erreur", " uWidgetEmpty.MouseLeave")
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
                If IsEmpty = True And _type <> TypeOfWidget.Device Then
                    Dim thr As New Thread(AddressOf Traite_Action_OnClick)
                    thr.Start()
                    'Traite_Action_OnClick()
                End If
                RaiseEvent Click(Me, e)
            Else
                If IsEmpty = True And _type = TypeOfWidget.Empty Then
                    Dim thr As New Thread(AddressOf Traite_Action_OnLongClick)
                    thr.Start()
                    'Traite_Action_OnLongClick()
                End If
                RaiseEvent LongClick(Me, e)
            End If
        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.Image_MouseLeftButtonUp: " & ex.Message, "Erreur", " uWidgetEmpty.Image_MouseLeftButtonUp")
        End Try
    End Sub

    Private Sub Image_PreviewMouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Me.PreviewMouseDown
        Try
            _Down = Now
            _oldposition = e.GetPosition(Me)
        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur Image_PreviewMouseDown: " & ex.Message, "Erreur", " Image_PreviewMouseDown")
        End Try
    End Sub


#Region "Gerer_Action_Gesture"
    Private Sub Traite_Action_OnClick()
        Try
            If _Show = False Then Exit Sub

            For Each _act As cWidget.Action In _Action_On_Click
                Dim _Existdev As Boolean = False

                'If frmMere.MaJWidgetFromServer Then
                _Existdev = myService.ExistDeviceById(_act.IdObject)
                'Else
                '    _dev = ReturnDeviceById(_act.IdObject)
                'End If

                If _Existdev Then
                    Dim x As New HoMIDom.HoMIDom.DeviceAction
                    x.Nom = _act.Methode
                    If String.IsNullOrEmpty(_act.Value.ToString) = False Then
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
                        'myService.ExecuteDeviceCommand(IdSrv, _act.IdObject, x)
                        myService.RunMacro(IdSrv, _act.IdObject)
                        _FlagBlock = False
                    Else
                        Dim _zon As HoMIDom.HoMIDom.Zone = myService.ReturnZoneByID(IdSrv, _act.IdObject)
                        If _zon IsNot Nothing Then
                            frmMere.ShowZone(_act.IdObject)
                        Else
                            ' Commandes
                            ProcessCommand(_act.Methode, _act.Value)
                        End If
                    End If
                End If
            Next
        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.Traite_Action_OnClick: " & ex.Message, "Erreur", " uWidgetEmpty.Traite_Action_OnClick")
        End Try
    End Sub

    Private Sub Traite_Action_OnLongClick()
        Try
            If _Show = False Then Exit Sub

            For Each _act As cWidget.Action In _Action_On_LongClick
                Dim _dev As HoMIDom.HoMIDom.TemplateDevice
                If frmMere.MaJWidgetFromServer Then
                    _dev = myService.ReturnDeviceByID(IdSrv, _act.IdObject)
                Else
                    _dev = ReturnDeviceById(_act.IdObject)
                End If

                If _dev IsNot Nothing Then
                    Dim x As New HoMIDom.HoMIDom.DeviceAction
                    x.Nom = _act.Methode
                    If String.IsNullOrEmpty(_act.Value.ToString) = False Then
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
                        myService.RunMacro(IdSrv, _act.IdObject)
                    Else
                        Dim _zon As HoMIDom.HoMIDom.Zone = myService.ReturnZoneByID(IdSrv, _act.IdObject)
                        If _zon IsNot Nothing Then
                            frmMere.ShowZone(_act.IdObject)
                        Else
                            ' Commandes
                            ProcessCommand(_act.Methode, _act.Value)
                        End If
                    End If
                End If
            Next
        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.Traite_Action_OnLongClick: " & ex.Message, "Erreur", " uWidgetEmpty.Traite_Action_OnLongClick")
        End Try
    End Sub

    Private Sub Traite_Action_HautBas()
        Try
            If _Show = False Then Exit Sub

            For Each _act As cWidget.Action In _Action_GestureHautBas
                Dim _dev As HoMIDom.HoMIDom.TemplateDevice
                If frmMere.MaJWidgetFromServer Then
                    _dev = myService.ReturnDeviceByID(IdSrv, _act.IdObject)
                Else
                    _dev = ReturnDeviceById(_act.IdObject)
                End If

                If _dev IsNot Nothing Then
                    Dim x As New HoMIDom.HoMIDom.DeviceAction
                    x.Nom = _act.Methode
                    If String.IsNullOrEmpty(_act.Value.ToString) = False Then
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
                        myService.RunMacro(IdSrv, _act.IdObject)
                        _FlagBlock = False
                    Else
                        Dim _zon As HoMIDom.HoMIDom.Zone = myService.ReturnZoneByID(IdSrv, _act.IdObject)
                        If _zon IsNot Nothing Then
                            frmMere.ShowZone(_act.IdObject)
                        Else
                            ' Commandes
                            ProcessCommand(_act.Methode, _act.Value)
                        End If
                    End If
                End If
            Next
        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.Traite_Action_HautBas: " & ex.Message, "Erreur", " uWidgetEmpty.Traite_Action_HautBas")
        End Try
    End Sub

    Private Sub Traite_Action_BasHaut()
        Try
            If _Show = False Then Exit Sub

            For Each _act As cWidget.Action In _Action_GestureBasHaut
                Dim _dev As HoMIDom.HoMIDom.TemplateDevice
                If frmMere.MaJWidgetFromServer Then
                    _dev = myService.ReturnDeviceByID(IdSrv, _act.IdObject)
                Else
                    _dev = ReturnDeviceById(_act.IdObject)
                End If

                If _dev IsNot Nothing Then
                    Dim x As New HoMIDom.HoMIDom.DeviceAction
                    x.Nom = _act.Methode
                    If String.IsNullOrEmpty(_act.Value.ToString) = False Then
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
                        myService.RunMacro(IdSrv, _act.IdObject)
                        _FlagBlock = False
                    Else
                        Dim _zon As HoMIDom.HoMIDom.Zone = myService.ReturnZoneByID(IdSrv, _act.IdObject)
                        If _zon IsNot Nothing Then
                            frmMere.ShowZone(_act.IdObject)
                        Else
                            ' Commandes
                            ProcessCommand(_act.Methode, _act.Value)
                        End If
                    End If
                End If
            Next
        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.Traite_Action_BasHaut: " & ex.Message, "Erreur", " uWidgetEmpty.Traite_Action_BasHaut")
        End Try
    End Sub

    Private Sub Traite_Action_GaucheDroite()
        Try
            If _Show = False Then Exit Sub

            For Each _act As cWidget.Action In _Action_GestureGaucheDroite
                Dim _dev As HoMIDom.HoMIDom.TemplateDevice
                If frmMere.MaJWidgetFromServer Then
                    _dev = myService.ReturnDeviceByID(IdSrv, _act.IdObject)
                Else
                    _dev = ReturnDeviceById(_act.IdObject)
                End If

                If _dev IsNot Nothing Then
                    Dim x As New HoMIDom.HoMIDom.DeviceAction
                    x.Nom = _act.Methode
                    If String.IsNullOrEmpty(_act.Value.ToString) = False Then
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
                        myService.RunMacro(IdSrv, _act.IdObject)
                        _FlagBlock = False
                    Else
                        Dim _zon As HoMIDom.HoMIDom.Zone = myService.ReturnZoneByID(IdSrv, _act.IdObject)
                        If _zon IsNot Nothing Then
                            frmMere.ShowZone(_act.IdObject)
                        Else
                            ' Commandes
                            ProcessCommand(_act.Methode, _act.Value)
                        End If
                    End If
                End If
            Next
        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.Traite_Action_GaucheDroite: " & ex.Message, "Erreur", " uWidgetEmpty.Traite_Action_GaucheDroite")
        End Try
    End Sub

    Private Sub Traite_Action_DroiteGauche()
        Try
            If _Show = False Then Exit Sub

            For Each _act As cWidget.Action In _Action_GestureDroiteGauche
                Dim _dev As HoMIDom.HoMIDom.TemplateDevice
                If frmMere.MaJWidgetFromServer Then
                    _dev = myService.ReturnDeviceByID(IdSrv, _act.IdObject)
                Else
                    _dev = ReturnDeviceById(_act.IdObject)
                End If

                If _dev IsNot Nothing Then
                    Dim x As New HoMIDom.HoMIDom.DeviceAction
                    x.Nom = _act.Methode
                    If String.IsNullOrEmpty(_act.Value.ToString) = False Then
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
                        myService.RunMacro(IdSrv, _act.IdObject)
                        _FlagBlock = False
                    Else
                        Dim _zon As HoMIDom.HoMIDom.Zone = myService.ReturnZoneByID(IdSrv, _act.IdObject)
                        If _zon IsNot Nothing Then
                            frmMere.ShowZone(_act.IdObject)
                        Else
                            ' Commandes
                            ProcessCommand(_act.Methode, _act.Value)
                        End If
                    End If
                End If
            Next
        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.Traite_Action_DroiteGauche: " & ex.Message, "Erreur", " uWidgetEmpty.Traite_Action_DroiteGauche")
        End Try
    End Sub
#End Region


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
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.FreeTouch: " & ex.Message, "Erreur", " uWidgetEmpty.FreeTouch")
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
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.ClickOn: " & ex.Message, "Erreur", " uWidgetEmpty.ClickOn")
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
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.ClickOff: " & ex.Message, "Erreur", " uWidgetEmpty.ClickOff")
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
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.ValueChange: " & ex.Message, "Erreur", " uWidgetEmpty.ValueChange")
            _FlagBlock = False
        End Try
    End Sub

    Private Sub SendCommand(ByVal Commande As String)
        Try
            Dim x As New HoMIDom.HoMIDom.DeviceAction
            If _dev IsNot Nothing Then
                Dim retour As String = myService.TelecommandeSendCommand(IdSrv, _dev.ID, Commande)
                If retour <> 0 Then
                    AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.SendCommand: " & retour, "Erreur", " uWidgetEmpty.SendCommand")
                End If

                If StkPopup.Children.Count > 0 Then
                    If Popup1.IsOpen = True Then
                        Popup1.IsOpen = False
                    End If
                End If
            End If
        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.SendCommand: " & ex.Message, "Erreur", " uWidgetEmpty.SendCommand")
            _FlagBlock = False
        End Try
    End Sub

    Private Sub ChangeValue(ByVal Value As String)
        Try
            Dim _value As Object = Nothing
            Dim _flag As Boolean = False

            _CurrentValue = Nothing

            If _dev IsNot Nothing And String.IsNullOrEmpty(Value) = False Then
                If _dev.Type = HoMIDom.HoMIDom.Device.ListeDevices.GENERIQUEBOOLEEN Then
                    Try
                        _value = Value
                        _flag = True
                    Catch ex As Exception
                        AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.ChangeValue: " & ex.Message, "Erreur", " uWidgetEmpty.ChangeValue")
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
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.ChangeValue: " & ex.Message, "Erreur", " uWidgetEmpty.ChangeValue")
            _FlagBlock = False
        End Try
    End Sub

    Private Sub AudioPlay()
        Try
            Dim x As New HoMIDom.HoMIDom.DeviceAction
            If _dev IsNot Nothing Then
                x.Nom = "EmptyPlayList"
            End If
            x.Parametres = Nothing

            myService.ExecuteDeviceCommand(IdSrv, Id, x)

            'Dim x As New HoMIDom.HoMIDom.DeviceAction
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
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.AudioPlay: " & ex.Message, "Erreur", " uWidgetEmpty.AudioPlay")
        End Try
    End Sub

    Private Sub AudioPause()
        Try
            Dim x As New HoMIDom.HoMIDom.DeviceAction
            If _dev IsNot Nothing Then
                x.Nom = "Pause"
            End If
            x.Parametres = Nothing

            myService.ExecuteDeviceCommand(IdSrv, Id, x)

            If StkPopup.Children.Count > 0 Then
                If Popup1.IsOpen = True Then
                    Popup1.IsOpen = False
                End If
            End If
        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.AudioPause: " & ex.Message, "Erreur", " uWidgetEmpty.AudioPause")
        End Try
    End Sub

    Private Sub AudioStop()
        Try
            Dim x As New HoMIDom.HoMIDom.DeviceAction
            If _dev IsNot Nothing Then
                x.Nom = "Stop"
            End If
            x.Parametres = Nothing

            myService.ExecuteDeviceCommand(IdSrv, Id, x)

            If StkPopup.Children.Count > 0 Then
                If Popup1.IsOpen = True Then
                    Popup1.IsOpen = False
                End If
            End If
        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.AudioStop: " & ex.Message, "Erreur", " uWidgetEmpty.AudioStop")
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
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.AudioNextChap: " & ex.Message, "Erreur", " uWidgetEmpty.AudioNextChap")
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
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.AudioPreviousChap: " & ex.Message, "Erreur", " uWidgetEmpty.AudioPreviousChap")
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
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.AudioMute: " & ex.Message, "Erreur", " uWidgetEmpty.AudioMute")
        End Try
    End Sub

    Private Sub AudioSetFile(ByVal File As String)
        Try
            Dim x As New HoMIDom.HoMIDom.DeviceAction
            If _dev IsNot Nothing Then
                x.Nom = "SetFichierAudio"
            End If
            Dim y As New HoMIDom.HoMIDom.DeviceAction.Parametre
            y.Value = File
            x.Parametres.Add(y)

            myService.ExecuteDeviceCommand(IdSrv, Id, x)

        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.AudioSetFile: " & ex.Message, "Erreur", " uWidgetEmpty.AudioSetFile")
        End Try
    End Sub

    Private Sub AudioVolumeUp()
        Try
            Dim x As New HoMIDom.HoMIDom.DeviceAction
            If _dev IsNot Nothing Then
                x.Nom = "VolumeUp"
            End If
            x.Parametres = Nothing

            myService.ExecuteDeviceCommand(IdSrv, Id, x)

            If StkPopup.Children.Count > 0 Then
                If Popup1.IsOpen = True Then
                    Popup1.IsOpen = False
                End If
            End If
        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.AudioVolumeUp: " & ex.Message, "Erreur", " uWidgetEmpty.AudioVolumeUp")
        End Try
    End Sub

    Private Sub AudioVolumeDown()
        Try
            Dim x As New HoMIDom.HoMIDom.DeviceAction
            If _dev IsNot Nothing Then
                x.Nom = "VolumeDown"
            End If
            x.Parametres = Nothing

            myService.ExecuteDeviceCommand(IdSrv, Id, x)

            If StkPopup.Children.Count > 0 Then
                If Popup1.IsOpen = True Then
                    Popup1.IsOpen = False
                End If
            End If
        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.AudioVolumeDown: " & ex.Message, "Erreur", " uWidgetEmpty.AudioVolumeDown")
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

                        ' <Horizon99> Suppression de la mise à jour immédiate: cela crée une sorte de "deuxième couche" de widgets.
                        ' S'il y a un problème de rafraîchissement on peut cliquer 
                        ' sur l 'icône de la zone pour faire un refresh manuel
                        'frmMere.ShowZone(frmMere._CurrentIdZone)
                    End If
                    X.Close()

                Catch ex As Exception
                    AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.Stk1_MouseDown: " & ex.Message, "Erreur", " uWidgetEmpty.Stk1_MouseDown")
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
                    If _dev IsNot Nothing Then
                        If _dev.Type = HoMIDom.HoMIDom.Device.ListeDevices.MULTIMEDIA Then
                            RaiseEvent ShowTemplate(_dev.Modele, _dev.ID)
                            Exit Sub
                        End If
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
                Else
                    If StkPopup.Children.Count > 0 Then
                        If Popup1.IsOpen = False Then
                            Popup1.IsOpen = True
                        Else
                            Popup1.IsOpen = False
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.Stk1_MouseDown: " & ex.ToString, "Erreur", " uWidgetEmpty.Stk1_MouseDown")
        End Try
    End Sub

    Private Sub uWidgetEmpty_SizeChanged(ByVal sender As Object, ByVal e As System.Windows.SizeChangedEventArgs) Handles Me.SizeChanged
        Try
            If _Show = False Then Exit Sub

            Select Case _type
                Case TypeOfWidget.Web
                    _Webbrowser.Width = Me.ActualWidth
                    _Webbrowser.Height = Me.ActualHeight - 20
                Case TypeOfWidget.Camera
                    _Camera.Width = Me.ActualWidth
                    _Camera.Height = Me.ActualHeight - 2
                Case TypeOfWidget.Volet
                    _VOLET.Width = Me.ActualWidth
                    _VOLET.Height = Me.ActualHeight - 30
                Case TypeOfWidget.Moteur
                Case TypeOfWidget.Prise
                Case TypeOfWidget.Rss
                    _RSS.Width = Me.ActualWidth
                    _RSS.Height = Me.ActualHeight - 20
                Case TypeOfWidget.Meteo
                    Exit Sub
                Case TypeOfWidget.KeyPad
                    Exit Sub
                Case TypeOfWidget.Label
                    Exit Sub
                Case TypeOfWidget.Gauge
                    Exit Sub
            End Select

            If ShowEtiquette And ShowPicture And Lbl.ActualHeight > 0 And Me.ActualHeight > 0 Then
                Dim H As Double = Me.ActualHeight - Lbl.ActualHeight
                If H > 0 Then Image.Height = H
                H = 0
            End If
            If ShowEtiquette = False And (ShowPicture Or _type = TypeOfWidget.Moteur Or _type = TypeOfWidget.Prise) And Me.ActualHeight > 0 Then
                If ShowPicture Then Image.Height = Me.ActualHeight
                If _MOTEUR IsNot Nothing Then _MOTEUR.Height = Me.ActualHeight
                If _PRISE IsNot Nothing Then _PRISE.Height = Me.ActualHeight
            End If

            Dim W As Double
            If ShowStatus And (ShowPicture Or _type = TypeOfWidget.Moteur Or Type = TypeOfWidget.Prise) And LblStatus.ActualHeight > 0 And Me.ActualWidth > 0 Then
                W = Me.ActualWidth - LblStatus.ActualWidth
                If W < 0 Then W = 0
                If ShowPicture Then Image.Width = W
                If _MOTEUR IsNot Nothing Then _MOTEUR.Width = W
                If _PRISE IsNot Nothing Then _PRISE.Width = W
            End If
            If ShowStatus = False And (ShowPicture Or _type = TypeOfWidget.Moteur Or _type = TypeOfWidget.Prise) Then
                If ShowPicture Then Image.Width = Me.ActualWidth
                If _MOTEUR IsNot Nothing Then _MOTEUR.Width = Me.ActualWidth
                If _PRISE IsNot Nothing Then _PRISE.Width = Me.ActualWidth
            End If
            If (ShowPicture Or _type = TypeOfWidget.Moteur Or _type = TypeOfWidget.Prise) And _GarderProportionImage Then
                If ShowPicture Then If Image.Width < Image.Height Then Image.Height = Image.Width
                If ShowPicture Then If Image.Height < Image.Width Then Image.Width = Image.Height
                If _MOTEUR IsNot Nothing Then If _MOTEUR.Width < _MOTEUR.Height Then _MOTEUR.Height = _MOTEUR.Width
                If _MOTEUR IsNot Nothing Then If _MOTEUR.Height < _MOTEUR.Width Then _MOTEUR.Width = _MOTEUR.Height
                If _PRISE IsNot Nothing Then If _PRISE.Width < _PRISE.Height Then _PRISE.Height = _PRISE.Width
                If _PRISE IsNot Nothing Then If _PRISE.Height < _PRISE.Width Then _PRISE.Width = _PRISE.Height
            End If
        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.SizeChanged: " & ex.Message, "Erreur", " uWidgetEmpty.SizeChanged")
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        Try
            MyBase.Finalize()
        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.Finalize: " & ex.Message, "Erreur", " uWidgetEmpty.Finalize")
        End Try
    End Sub

    Private Sub KeyPadOK(ByVal Value As Integer)
        Try
            If IsConnect Then
                If String.IsNullOrEmpty(_IDKeyPad) = False Then
                    myService.ChangeValueOfDevice(IdSrv, _IDKeyPad, Value)
                End If
            End If
        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur KeyPadOK: " & ex.Message, "Erreur", " KeyPadOK")
        End Try
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
            _Camera = Nothing
            _VOLET = Nothing
            _MOTEUR = Nothing
            _PRISE = Nothing
            _Gauge = Nothing
        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.Unloaded: " & ex.Message, "Erreur", " uWidgetEmpty.Unloaded")
        End Try

    End Sub

    Private Function GetStatusPicture(ByVal Filename As String, ByVal Value As Object) As String
        Try
            Dim _file As String = Filename.ToLower
            If _file.EndsWith("-defaut.png") = True Or _file.Contains("composant_") = True Then
                ' Lorsque l'utilisateur n'a pas modifié l'image par défaut automatiquement
                ' générée sur le serveur à la création du device, on sélectionne l'image à afficher
                ' sur le widget client en sélectionnant une image reflétant la valeur ou l'état du device
                Select Case _dev.Type
                    Case HoMIDom.HoMIDom.Device.ListeDevices.APPAREIL
                        If Value = "True" Then
                            GetStatusPicture = _MonRepertoire & "\Images\Devices\appareil-on.png"
                        Else
                            GetStatusPicture = _MonRepertoire & "\Images\Devices\appareil-off.png"
                        End If
                    Case HoMIDom.HoMIDom.Device.ListeDevices.AUDIO
                        GetStatusPicture = _MonRepertoire & "\Images\Devices\audio-defaut.png"
                    Case HoMIDom.HoMIDom.Device.ListeDevices.BAROMETRE
                        GetStatusPicture = _MonRepertoire & "\Images\Devices\barometre-defaut.png"
                    Case HoMIDom.HoMIDom.Device.ListeDevices.BATTERIE
                        If TypeOf Value Is Boolean Then
                            If Value = True Then GetStatusPicture = _MonRepertoire & "\Images\Devices\batterie-100.png" Else GetStatusPicture = _MonRepertoire & "\Images\Devices\batterie-0.png"
                        ElseIf IsNumeric(Value) Then 'TypeOf Value Is Integer Or TypeOf Value Is Long Or TypeOf Value Is Single Or TypeOf Value Is Double Then
                            If CSng(Value) > 80 Then
                                GetStatusPicture = _MonRepertoire & "\Images\Devices\batterie-100.png"
                            ElseIf CSng(Value) > 60 Then
                                GetStatusPicture = _MonRepertoire & "\Images\Devices\batterie-75.png"
                            ElseIf CSng(Value) > 40 Then
                                GetStatusPicture = _MonRepertoire & "\Images\Devices\batterie-50.png"
                            ElseIf CSng(Value) > 20 Then
                                GetStatusPicture = _MonRepertoire & "\Images\Devices\batterie-25.png"
                            Else
                                GetStatusPicture = _MonRepertoire & "\Images\Devices\batterie-0.png"
                            End If
                        Else
                            If STRGS.UCase(Value) = "LOW" Or STRGS.UCase(Value) = "0%" Then GetStatusPicture = _MonRepertoire & "\Images\Devices\batterie-0.png" Else GetStatusPicture = _MonRepertoire & "\Images\Devices\batterie-100.png"
                        End If

                    Case HoMIDom.HoMIDom.Device.ListeDevices.COMPTEUR
                        GetStatusPicture = _MonRepertoire & "\Images\Devices\compteur-defaut.png"
                    Case HoMIDom.HoMIDom.Device.ListeDevices.CONTACT
                        If Value = "True" Then
                            GetStatusPicture = _MonRepertoire & "\Images\Devices\contact-on.png"
                        Else
                            GetStatusPicture = _MonRepertoire & "\Images\Devices\contact-off.png"
                        End If
                    Case HoMIDom.HoMIDom.Device.ListeDevices.DETECTEUR
                        If Value = "True" Then
                            GetStatusPicture = _MonRepertoire & "\Images\Devices\detecteur-on.png"
                        Else
                            GetStatusPicture = _MonRepertoire & "\Images\Devices\detecteur-off.png"
                        End If
                    Case HoMIDom.HoMIDom.Device.ListeDevices.DIRECTIONVENT
                        GetStatusPicture = _MonRepertoire & "\Images\Devices\directionvent-defaut.png"
                    Case HoMIDom.HoMIDom.Device.ListeDevices.ENERGIEINSTANTANEE
                        GetStatusPicture = _MonRepertoire & "\Images\Devices\energieinstantanee-defaut.png"
                    Case HoMIDom.HoMIDom.Device.ListeDevices.ENERGIETOTALE
                        GetStatusPicture = _MonRepertoire & "\Images\Devices\energietotale-defaut.png"
                    Case HoMIDom.HoMIDom.Device.ListeDevices.FREEBOX
                        GetStatusPicture = _MonRepertoire & "\Images\Devices\freebox-defaut.png"
                    Case HoMIDom.HoMIDom.Device.ListeDevices.GENERIQUEBOOLEEN
                        If Value = "True" <> 0 Then
                            GetStatusPicture = _MonRepertoire & "\Images\Devices\generiquebooleen-on.png"
                        Else
                            GetStatusPicture = _MonRepertoire & "\Images\Devices\generiquebooleen-off.png"
                        End If
                    Case HoMIDom.HoMIDom.Device.ListeDevices.GENERIQUESTRING
                        GetStatusPicture = _MonRepertoire & "\Images\Devices\generiquestring-defaut.png"
                    Case HoMIDom.HoMIDom.Device.ListeDevices.GENERIQUEVALUE
                        GetStatusPicture = _MonRepertoire & "\Images\Devices\generiquevalue-defaut.png"
                    Case HoMIDom.HoMIDom.Device.ListeDevices.HUMIDITE
                        GetStatusPicture = _MonRepertoire & "\Images\Devices\humidite-defaut.png"
                    Case HoMIDom.HoMIDom.Device.ListeDevices.LAMPE
                        If CSng(Value) > 80 Then
                            GetStatusPicture = _MonRepertoire & "\Images\Devices\lampe-100.png"
                        ElseIf Value > 60 Then
                            GetStatusPicture = _MonRepertoire & "\Images\Devices\lampe-75.png"
                        ElseIf Value > 40 Then
                            GetStatusPicture = _MonRepertoire & "\Images\Devices\lampe-50.png"
                        ElseIf Value > 20 Then
                            GetStatusPicture = _MonRepertoire & "\Images\Devices\lampe-25.png"
                        Else
                            GetStatusPicture = _MonRepertoire & "\Images\Devices\lampe-0.png"
                        End If
                    Case HoMIDom.HoMIDom.Device.ListeDevices.METEO
                        GetStatusPicture = _MonRepertoire & "\Images\Devices\meteo-defaut.png"
                    Case HoMIDom.HoMIDom.Device.ListeDevices.MULTIMEDIA
                        GetStatusPicture = _MonRepertoire & "\Images\Devices\multimedia-defaut.png"
                    Case HoMIDom.HoMIDom.Device.ListeDevices.PLUIECOURANT
                        GetStatusPicture = _MonRepertoire & "\Images\Devices\pluiecourant-defaut.png"
                    Case HoMIDom.HoMIDom.Device.ListeDevices.PLUIETOTAL
                        GetStatusPicture = _MonRepertoire & "\Images\Devices\pluietotal-defaut.png"
                    Case HoMIDom.HoMIDom.Device.ListeDevices.SWITCH
                        If Value = "True" Then
                            GetStatusPicture = _MonRepertoire & "\Images\Devices\switch-on.png"
                        Else
                            GetStatusPicture = _MonRepertoire & "\Images\Devices\switch-off.png"
                        End If
                    Case HoMIDom.HoMIDom.Device.ListeDevices.TELECOMMANDE
                        GetStatusPicture = _MonRepertoire & "\Images\Devices\telecommande-defaut.png"
                    Case HoMIDom.HoMIDom.Device.ListeDevices.TEMPERATURE
                        GetStatusPicture = _MonRepertoire & "\Images\Devices\temperature-defaut.png"
                    Case HoMIDom.HoMIDom.Device.ListeDevices.TEMPERATURECONSIGNE
                        GetStatusPicture = _MonRepertoire & "\Images\Devices\temperatureconsigne-defaut.png"
                    Case HoMIDom.HoMIDom.Device.ListeDevices.UV
                        GetStatusPicture = _MonRepertoire & "\Images\Devices\uv-defaut.png"
                    Case HoMIDom.HoMIDom.Device.ListeDevices.VITESSEVENT
                        GetStatusPicture = _MonRepertoire & "\Images\Devices\vitessevent-defaut.png"
                    Case HoMIDom.HoMIDom.Device.ListeDevices.VOLET
                        If CSng(Value) > 80 Then
                            GetStatusPicture = _MonRepertoire & "\Images\Devices\volet-100.png"
                        ElseIf CSng(Value) > 60 Then
                            GetStatusPicture = _MonRepertoire & "\Images\Devices\volet-75.png"
                        ElseIf CSng(Value) > 40 Then
                            GetStatusPicture = _MonRepertoire & "\Images\Devices\volet-50.png"
                        ElseIf CSng(Value) > 20 Then
                            GetStatusPicture = _MonRepertoire & "\Images\Devices\volet-25.png"
                        Else
                            GetStatusPicture = _MonRepertoire & "\Images\Devices\volet-0.png"
                        End If
                    Case Else
                        GetStatusPicture = Filename
                End Select
            ElseIf _file.Contains("_on") = True Or _file.Contains("_off") = True Then
                ' On recherche s'il existe des images de type double état (ON/OFF) et si oui:
                ' - affiche l 'image xxxx_on.jpg si la valeur numérique est différent de zéro (ou False)
                ' - affiche l 'image xxxx_off.jpg si la valeur numérique est de zéro (ou False)
                If CSng(Value) = 0 Then
                    GetStatusPicture = _file.Replace("_on", "_off")
                Else
                    GetStatusPicture = _file.Replace("_off", "_on")
                End If
            Else
                ' Lorsqu'il y a une autre image, on la charge simplement
                GetStatusPicture = _file
            End If

            If _dev.Name.ToUpper = "HOMI_JOUR" Then
                If _dev.Value = True Then
                    GetStatusPicture = _MonRepertoire & "\Images\Devices\jour-jour.png"
                Else
                    GetStatusPicture = _MonRepertoire & "\Images\Devices\jour-nuit.png"
                End If
            End If

        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.GetStatusPicture: " & ex.Message, "Erreur", " uWidgetEmpty.GetStatusPicture")
            Return Filename
        End Try
    End Function

    Private Sub LoadPicture()
        Try
            Dim DisplayPictureFileName As String = ""

            If _dev IsNot Nothing Then
                DisplayPictureFileName = GetStatusPicture(_Picture, _dev.Value)
                If IO.File.Exists(DisplayPictureFileName) Then
                    ' L'image existe en local
                    ImageBehavior.SetAnimatedSource(Image, New BitmapImage(New Uri(DisplayPictureFileName)))
                Else
                    ' L'image n'a pas été trouvée en local, on la reprend du serveur
                    ImageBehavior.SetAnimatedSource(Image, ConvertArrayToImage(myService.GetByteFromImage(DisplayPictureFileName)))
                End If
            ElseIf (_type = TypeOfWidget.Empty Or _type = TypeOfWidget.Device Or Type = TypeOfWidget.Image) And _Visuel.Count = 0 And String.IsNullOrEmpty(_Picture) = False Then
                ' Affichage de l'image pour un widget empty s'il n'existe aucune visualisation
                If IO.File.Exists(_Picture) Then
                    ' L'image existe en local
                    ImageBehavior.SetAnimatedSource(Image, New BitmapImage(New Uri(_Picture)))
                Else
                    ' L'image n'a pas été trouvée en local, on la reprend du serveur
                    ImageBehavior.SetAnimatedSource(Image, ConvertArrayToImage(myService.GetByteFromImage(_Picture)))
                End If
            End If

            If (_type = TypeOfWidget.Empty Or _type = TypeOfWidget.Device Or Type = TypeOfWidget.Image) And _Visuel.Count > 0 And String.IsNullOrEmpty(_Picture) = False Then
                ' Affichage de l'image pour un widget empty s'il une visualisation
                If IO.File.Exists(_Picture) Then
                    ' L'image existe en local
                    ImageBehavior.SetAnimatedSource(Image, New BitmapImage(New Uri(_Picture)))
                Else
                    ' L'image n'a pas été trouvée en local, on la reprend du serveur
                    ImageBehavior.SetAnimatedSource(Image, ConvertArrayToImage(myService.GetByteFromImage(_Picture)))
                End If
            End If
        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uWidgetEmpty.LoadPicture: " & ex.Message, "Erreur", " uWidgetEmpty.LoadPicture")
        End Try

    End Sub

    Private Sub ProcessCommand(Command As String, Argument As String)
        Try
            Select Case Command
                Case "Exécuter commande DOS"
                    Process.Start(Argument)
                Case "Quitter HoMIWpF"
                    'frmMere.SaveConfig(frmMere.ConfigFile)
                    Log(TypeLog.INFO, TypeSource.CLIENT, "Client", "Fermture de l'application")
                    End
                Case "Charger configuration"
                    frmMere.ConfigFile = Argument
                    frmMere.LoadConfig(Argument)
                    frmMere.Canvas1.Children.Clear()
                    frmMere.LoadZones()
                    frmMere.ScrollViewer1.Content = frmMere.imgStackPnl
            End Select
        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur ProcessCommand: " & ex.Message, "Erreur", " ProcessCommand")
        End Try
    End Sub

    Private Sub Popup1_MouseLeave(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles Popup1.MouseLeave
        Try
            Popup1.IsOpen = False
        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur Popup1_MouseLeave: " & ex.Message, "Erreur", " Popup1_MouseLeave")
        End Try
    End Sub


End Class
