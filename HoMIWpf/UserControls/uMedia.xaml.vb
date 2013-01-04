Imports System.Windows.Threading
Imports TagLib
Imports System.IO
Imports System.Drawing.Imaging

Public Class uMedia

    Dim _ShowToolBar As Boolean = True
    Dim _ShowTag As Boolean = True
    Dim _ShowBtnPlay As Boolean = True
    Dim _ShowBtnPause As Boolean = True
    Dim _ShowBtnStop As Boolean = True
    Dim _ShowBtnAvance As Boolean = True
    Dim _ShowBtnRecul As Boolean = True
    Dim _ShowBtnAvanceTitre As Boolean = False
    Dim _ShowBtnReculTitre As Boolean = False
    Dim _ShowBtnOpen As Boolean = True
    Dim _ShowSliderTime As Boolean = True
    Dim _ShowBtnVolume As Boolean = True
    Dim _ShowVideo As Boolean = True
    Dim _Volume As Double = 0.5
    Dim _VideoWidth As Double = 300
    Dim _VideoHeight As Double = 300
    Dim _IsLocal As Boolean = True 'Si true on utilise le lecteur media local
    Public Filter As New List(Of String)

    Dim _Uri As String = ""
    Dim dt As DispatcherTimer = New DispatcherTimer()
    Private Const THUMBNAIL_DATA As Integer = 20507

    'EVENT *************************************************
    Public Event Play()
    Public Event Pause()
    Public Event [Stop]()
    Public Event Avance()
    Public Event Recul()
    Public Event PreviousChap()
    Public Event NextChap()
    Public Event Mute()
    Public Event VolumeUp()
    Public Event VolumeDown()

    Public Sub New()

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        ShowBtnReculTitre = False
        ShowBtnAvanceTitre = False
        If _IsLocal Then _Volume = MediaElement1.Volume
        VolumeValue.Value = _Volume
        AddHandler dt.Tick, AddressOf dispatcherTimer_Tick
        dt.Interval = New TimeSpan(0, 0, 1)
        dt.Start()
    End Sub

    Public Property ShowToolBar As Boolean
        Get
            Return _ShowToolBar
        End Get
        Set(ByVal value As Boolean)
            _ShowToolBar = value
            If value = True Then
                Toolbar.Visibility = Windows.Visibility.Visible
            Else
                Toolbar.Visibility = Windows.Visibility.Collapsed
            End If
        End Set
    End Property

    Public Property ShowTag As Boolean
        Get
            Return _ShowTag
        End Get
        Set(ByVal value As Boolean)
            _ShowTag = value
            If value = True Then
                StkInfo.Visibility = Windows.Visibility.Visible
            Else
                StkInfo.Visibility = Windows.Visibility.Collapsed
            End If
        End Set
    End Property

    Public Property ShowBtnPlay As Boolean
        Get
            Return _ShowBtnPlay
        End Get
        Set(ByVal value As Boolean)
            _ShowBtnPlay = value
            If value = True Then
                BtnPlay.Visibility = Windows.Visibility.Visible
            Else
                BtnPlay.Visibility = Windows.Visibility.Collapsed
            End If
        End Set
    End Property

    Public Property ShowBtnPause As Boolean
        Get
            Return _ShowBtnPause
        End Get
        Set(ByVal value As Boolean)
            _ShowBtnPause = value
            If value = True Then
                BtnPause.Visibility = Windows.Visibility.Visible
            Else
                BtnPause.Visibility = Windows.Visibility.Collapsed
            End If
        End Set
    End Property

    Public Property ShowBtnStop As Boolean
        Get
            Return _ShowBtnStop
        End Get
        Set(ByVal value As Boolean)
            _ShowBtnStop = value
            If value = True Then
                BtnStop.Visibility = Windows.Visibility.Visible
            Else
                BtnStop.Visibility = Windows.Visibility.Collapsed
            End If
        End Set
    End Property

    Public Property ShowBtnAvance As Boolean
        Get
            Return _ShowBtnAvance
        End Get
        Set(ByVal value As Boolean)
            _ShowBtnAvance = value
            If value = True Then
                BtnAvance.Visibility = Windows.Visibility.Visible
            Else
                BtnAvance.Visibility = Windows.Visibility.Collapsed
            End If
        End Set
    End Property

    Public Property ShowBtnRecul As Boolean
        Get
            Return _ShowBtnRecul
        End Get
        Set(ByVal value As Boolean)
            _ShowBtnRecul = value
            If value = True Then
                BtnRecul.Visibility = Windows.Visibility.Visible
            Else
                BtnRecul.Visibility = Windows.Visibility.Collapsed
            End If
        End Set
    End Property

    Public Property ShowBtnAvanceTitre As Boolean
        Get
            Return _ShowBtnAvanceTitre
        End Get
        Set(ByVal value As Boolean)
            _ShowBtnAvanceTitre = value
            If value = True Then
                BtnAvanceTitre.Visibility = Windows.Visibility.Visible
            Else
                BtnAvanceTitre.Visibility = Windows.Visibility.Collapsed
            End If
        End Set
    End Property

    Public Property ShowBtnReculTitre As Boolean
        Get
            Return _ShowBtnReculTitre
        End Get
        Set(ByVal value As Boolean)
            _ShowBtnReculTitre = value
            If value = True Then
                BtnReculTitre.Visibility = Windows.Visibility.Visible
            Else
                BtnReculTitre.Visibility = Windows.Visibility.Collapsed
            End If
        End Set
    End Property

    Public Property ShowBtnOpen As Boolean
        Get
            Return _ShowBtnOpen
        End Get
        Set(ByVal value As Boolean)
            _ShowBtnOpen = value
            If value = True Then
                BtnOpen.Visibility = Windows.Visibility.Visible
            Else
                BtnOpen.Visibility = Windows.Visibility.Collapsed
            End If
        End Set
    End Property

    Public Property ShowBtnVolume As Boolean
        Get
            Return _ShowBtnVolume
        End Get
        Set(ByVal value As Boolean)
            _ShowBtnVolume = value
            If value = True Then
                StkVolume.Visibility = Windows.Visibility.Visible
            Else
                StkVolume.Visibility = Windows.Visibility.Collapsed
            End If
        End Set
    End Property

    Public Property ShowSliderTime As Boolean
        Get
            Return _ShowSliderTime
        End Get
        Set(ByVal value As Boolean)
            _ShowSliderTime = value
            If value = True Then
                SliderSeek.Visibility = Windows.Visibility.Visible
            Else
                SliderSeek.Visibility = Windows.Visibility.Collapsed
            End If
        End Set
    End Property

    Public Property Uri As String
        Get
            Return _Uri
        End Get
        Set(ByVal value As String)
            If _Uri <> value Then
                _Uri = value
                MediaElement1.Source = New Uri(_Uri)
                MediaElement1.LoadedBehavior = MediaState.Play
            End If
            _Uri = value

        End Set
    End Property

    Public Property Volume As Double
        Get
            Return _Volume
        End Get
        Set(ByVal value As Double)
            _Volume = value
            If _Volume < 0 Then _Volume = 0
            If _Volume > 1 Then _Volume = 1

            MediaElement1.Volume = _Volume
        End Set
    End Property

    Public Property VideoWidth As Double
        Get
            Return _VideoWidth
        End Get
        Set(ByVal value As Double)
            _VideoWidth = value
            MediaElement1.Width = _VideoWidth
        End Set
    End Property

    Public Property VideoHeight As Double
        Get
            Return _VideoHeight
        End Get
        Set(ByVal value As Double)
            _VideoHeight = value
            MediaElement1.Height = _VideoHeight
        End Set
    End Property

    Public Property ShowVideo As Boolean
        Get
            Return _ShowVideo
        End Get
        Set(ByVal value As Boolean)
            _ShowVideo = value
            If value = True Then
                MediaElement1.Visibility = Windows.Visibility.Visible
            Else
                MediaElement1.Visibility = Windows.Visibility.Collapsed
            End If
        End Set
    End Property

    Public Property IsLocal As Boolean
        Get
            Return _IsLocal
        End Get
        Set(ByVal value As Boolean)
            _IsLocal = value
        End Set
    End Property

    Private Sub BtnPlay_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnPlay.MouseDown
        RaiseEvent Play()
        If _IsLocal Then MediaElement1.LoadedBehavior = MediaState.Play
    End Sub

    Private Sub BtnPause_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnPause.MouseDown
        RaiseEvent Pause()
        If _IsLocal Then MediaElement1.LoadedBehavior = MediaState.Pause
    End Sub

    Private Sub BtnStop_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnStop.MouseDown
        RaiseEvent Stop()
        If _IsLocal Then
            MediaElement1.LoadedBehavior = MediaState.Stop
            dt.Stop()
            SliderSeek.Value = 0
        End If
        ImgThumb.Source = Nothing
    End Sub

    Private Sub BtnRecul_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnRecul.MouseDown
        RaiseEvent Recul()
        If _IsLocal Then MediaElement1.Position = MediaElement1.Position - TimeSpan.FromSeconds(10)
    End Sub

    Private Sub BtnReculTitre_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnReculTitre.MouseDown
        RaiseEvent PreviousChap()
    End Sub

    Private Sub BtnAvance_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnAvance.MouseDown
        RaiseEvent Avance()
        If _IsLocal Then MediaElement1.Position = MediaElement1.Position + TimeSpan.FromSeconds(10)
    End Sub

    Private Sub BtnAvanceTitre_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnAvanceTitre.MouseDown
        RaiseEvent NextChap()
    End Sub

    Private Sub BtnOpen_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnOpen.MouseDown
        Try
            Me.Cursor = Cursors.Wait
            Dim ofd As System.Windows.Forms.OpenFileDialog = New System.Windows.Forms.OpenFileDialog
            If Filter.Count = 0 Then
                ofd.Filter = "Tous (*.*)|*.*"
            Else
                For i As Integer = 0 To Filter.Count - 1
                    If Filter.Count > 1 Then
                        ofd.Filter = Filter(i) & "|"
                    Else
                        ofd.Filter = Filter(i)
                    End If
                Next
            End If

            If ofd.ShowDialog = Forms.DialogResult.OK Then
                Dim X As TagLib.File
                _Uri = ofd.FileName

                ' Recupere les tags du fichier Audio 
                X = TagLib.File.Create(ofd.FileName)
                LblTitle.Content = X.Tag.Title
                LblArtiste.Content = X.Tag.FirstPerformer
                LblAlbum.Content = X.Tag.Album
                LblAnnee.Content = X.Tag.Year
                LblComment.Content = X.Tag.Comment
                LblGenre.Content = X.Tag.FirstGenre
                LblDuree.Content = System.Convert.ToString(X.Properties.Duration.Minutes) & ":" & System.Convert.ToString(Format(X.Properties.Duration.Seconds, "00"))

                If X.Tag.Pictures.Length > 0 Then
                    Dim bin = DirectCast(X.Tag.Pictures(0).Data.Data, Byte())
                    Dim bImg As New System.Windows.Media.Imaging.BitmapImage()
                    bImg.BeginInit()
                    bImg.StreamSource = New MemoryStream(bin)
                    bImg.EndInit()
                    ImgThumb.Source = bImg
                Else
                    ImgThumb.Visibility = Windows.Visibility.Collapsed
                End If

                X = Nothing

                If _IsLocal Then MediaElement1.Source = New Uri(ofd.FileName)
                If _IsLocal Then MediaElement1.LoadedBehavior = MediaState.Play
            End If

        Catch ex As Exception
            MessageBox.Show("Erreur: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try

        Me.Cursor = Nothing
    End Sub

    Private Sub dispatcherTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        If _IsLocal Then SliderSeek.Value = MediaElement1.Position.TotalSeconds
    End Sub

    Private Sub MediaElement1_MediaOpened(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MediaElement1.MediaOpened
        dt.Stop()
        SliderSeek.Maximum = MediaElement1.NaturalDuration.TimeSpan.Seconds
        SliderSeek.SmallChange = 2
        dt.Start()
    End Sub

    Private Sub SliderSeek_ValueChanged() Handles SliderSeek.MouseLeftButtonUp
        Dim SliderValue As Integer = CType(SliderSeek.Value, Integer)

        ' Overloaded constructor takes the arguments days, hours, minutes, seconds, miniseconds. 
        ' Create a TimeSpan with miliseconds equal to the slider value. 
        Dim ts As New TimeSpan(0, 0, 0, 0, SliderValue)
        If _IsLocal Then MediaElement1.Position = ts
    End Sub

    Private Sub BtnVolumeMute_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnVolumeMute.MouseDown
        _Volume = 0
        VolumeValue.Value = _Volume
        RaiseEvent Mute()
        If _IsLocal Then MediaElement1.Volume = _Volume
    End Sub

    Private Sub BtnVolumeDown_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnVolumeDown.MouseDown
        _Volume -= 0.1
        If _Volume < 0 Then _Volume = 0
        VolumeValue.Value = _Volume
        RaiseEvent VolumeDown()
        If _IsLocal Then MediaElement1.Volume = _Volume
    End Sub

    Private Sub BtnVolumeUp_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnVolumeUp.MouseDown
        _Volume += 0.1
        If _Volume > 1 Then _Volume = 1
        VolumeValue.Value = _Volume
        RaiseEvent VolumeUp()
        If _IsLocal Then MediaElement1.Volume = _Volume
    End Sub

End Class
