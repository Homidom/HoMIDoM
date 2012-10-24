Public Class uMedia

    Dim _ShowToolBar As Boolean = True
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
    Dim _Volume As Double = 0.5
    Dim _VideoWidth As Double = 300
    Dim _VideoHeight As Double = 300
    Dim _Uri As String = ""

    Public Sub New()

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        ShowBtnReculTitre = False
        ShowBtnAvanceTitre = False
        _Volume = MediaElement1.Volume
    End Sub

    Public Property ShowToolBar As Boolean
        Get
            Return _ShowToolBar
        End Get
        Set(ByVal value As Boolean)
            _ShowToolBar = value
            If value = True Then
                ToolBar.Visibility = Windows.Visibility.Visible
            Else
                ToolBar.Visibility = Windows.Visibility.Collapsed
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

    Private Sub BtnPlay_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnPlay.MouseDown
        MediaElement1.LoadedBehavior = MediaState.Play
    End Sub

    Private Sub BtnPause_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnPause.MouseDown
        MediaElement1.LoadedBehavior = MediaState.Pause
    End Sub

    Private Sub BtnStop_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnStop.MouseDown
        MediaElement1.LoadedBehavior = MediaState.Stop
    End Sub

    Private Sub BtnReculTitre_ImageFailed(ByVal sender As System.Object, ByVal e As System.Windows.ExceptionRoutedEventArgs) Handles BtnReculTitre.ImageFailed

    End Sub

    Private Sub BtnRecul_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnRecul.MouseDown
        MediaElement1.Position = MediaElement1.Position - TimeSpan.FromSeconds(10)
    End Sub

    Private Sub BtnAvance_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnAvance.MouseDown
        MediaElement1.Position = MediaElement1.Position + TimeSpan.FromSeconds(10)
    End Sub

    Private Sub BtnAvanceTitre_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnAvanceTitre.MouseDown

    End Sub

    Private Sub BtnOpen_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnOpen.MouseDown
        Try
            Dim ofd As System.Windows.Forms.OpenFileDialog = New System.Windows.Forms.OpenFileDialog
            ofd.Filter = "Tous (*.*)|*.*"

            If ofd.ShowDialog = Forms.DialogResult.OK Then
                MediaElement1.Source = New Uri(ofd.FileName)
                MediaElement1.LoadedBehavior = MediaState.Play
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur: " & ex.ToString, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub MediaElement1_MediaOpened(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MediaElement1.MediaOpened
        SliderSeek.Maximum = MediaElement1.NaturalDuration.TimeSpan.TotalMilliseconds
    End Sub

    Private Sub SliderSeek_ValueChanged(ByVal sender As System.Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Double)) Handles SliderSeek.ValueChanged
        Dim SliderValue As Integer = CType(SliderSeek.Value, Integer)

        ' Overloaded constructor takes the arguments days, hours, minutes, seconds, miniseconds. 
        ' Create a TimeSpan with miliseconds equal to the slider value. 
        Dim ts As New TimeSpan(0, 0, 0, 0, SliderValue)
        MediaElement1.Position = ts

    End Sub

    Private Sub BtnVolumeMute_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnVolumeMute.MouseDown
        _Volume = 0
        MediaElement1.Volume = _Volume
    End Sub

    Private Sub BtnVolumeDown_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnVolumeDown.MouseDown
        _Volume -= 0.1
        If _Volume < 0 Then _Volume = 0
        MediaElement1.Volume = _Volume
    End Sub

    Private Sub BtnVolumeUp_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnVolumeUp.MouseDown
        _Volume += 0.1
        If _Volume > 0 Then _Volume = 1
        MediaElement1.Volume = _Volume
    End Sub
End Class
