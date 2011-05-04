Imports System
Imports System.Windows.Threading


Partial Public Class uModule
    Dim _Label As String
    Dim _Adresse As String
    Dim _Icon As String
    Dim _HasValue As Boolean
    Dim _HasString As Boolean
    Dim _DeviceValue As String
    Dim _DeviceString As String
    Dim _Status As Integer
    Dim lstatus As String() = {"0", "1", "On", "Off", "Dim", "Dim", "", "", "", "", "", "", "", "", "", "", "", "Unknown"}
    Dim _TypeDevice As eTypeDevice
    Dim Rouge As New SolidColorBrush(Colors.Red)
    Dim Vert As New SolidColorBrush(Colors.Green)

    Public Enum eTypeDevice
        OnOff = 0
        OnOffVariation = 1
        Capteur = 2
        Audio = 3
    End Enum

    Public Property Label() As String
        Get
            Return _Label
        End Get
        Set(ByVal value As String)
            _Label = value
            Lbl.Content = value
        End Set
    End Property

    Public Property Adresse() As String
        Get
            Return _Adresse
        End Get
        Set(ByVal value As String)
            _Adresse = value
        End Set
    End Property

    Public Property TypeDevice() As eTypeDevice
        Get
            Return _TypeDevice
        End Get
        Set(ByVal value As eTypeDevice)
            _TypeDevice = value
            If value >= 2 Then
                BtnON.Visibility = Windows.Visibility.Hidden
                BtnOFF.Visibility = Windows.Visibility.Hidden
            End If
            If value <> 1 Then
                Slider1.Visibility = Windows.Visibility.Hidden
            End If
            If value <> eTypeDevice.Audio Then
                BtnStop.Visibility = Windows.Visibility.Hidden
                BtnPlay.Visibility = Windows.Visibility.Hidden
                BtnPause.Visibility = Windows.Visibility.Hidden
                BtnMute.Visibility = Windows.Visibility.Hidden
                BtnVolumDown.Visibility = Windows.Visibility.Hidden
                BtnVolumUp.Visibility = Windows.Visibility.Hidden
            End If
        End Set
    End Property

    Public Property Icon() As String
        Get
            Return _Icon
        End Get
        Set(ByVal value As String)
            _Icon = value
            If _Icon <> "" Then
                Dim bmpImage As New BitmapImage()
                bmpImage.BeginInit()
                bmpImage.UriSource = New Uri(_Icon, UriKind.Absolute)
                bmpImage.EndInit()
                Ico.Source = bmpImage
            End If
        End Set
    End Property

    Public Property Status() As Integer
        Get
            Return _Status
        End Get
        Set(ByVal value As Integer)
            _Status = value
            If _TypeDevice <> eTypeDevice.Audio Then
                LblStatus.Content = "ETAT: " & lstatus(value)
                LED.Visibility = Windows.Visibility.Visible
                If value = 2 Then
                    Dim ImgSrc As ImageSource = New BitmapImage(New Uri("C:\ehome\Images\orb_green.png"))
                    LED.Source = ImgSrc
                End If
                If value = 3 Then
                    Dim ImgSrc As ImageSource = New BitmapImage(New Uri("C:\ehome\Images\orb_red.png"))
                    LED.Source = ImgSrc
                End If
            Else
                LED.Visibility = Windows.Visibility.Hidden
                LblStatus.Content = "ETAT: " & _DeviceString
            End If
        End Set
    End Property

    Public Property HasValue() As Boolean
        Get
            Return _HasValue
        End Get
        Set(ByVal value As Boolean)
            _HasValue = value
            If value = True Then
                ' LblValue.Visibility = Windows.Visibility.Visible
            Else
                ' LblValue.Visibility = Windows.Visibility.Hidden
            End If
        End Set
    End Property

    Public Property HasString() As Boolean
        Get
            Return _HasString
        End Get
        Set(ByVal value As Boolean)
            _HasString = value
            If value = True Then
                'LblString.Visibility = Windows.Visibility.Visible
            Else
                'LblString.Visibility = Windows.Visibility.Hidden
            End If
        End Set
    End Property

    Public Property DeviceValue() As String
        Get
            Return _DeviceValue
        End Get
        Set(ByVal value As String)
            _DeviceValue = value
            '  LblValue.Content = value
        End Set
    End Property

    Public Property DeviceString() As String
        Get
            Return _DeviceString
        End Get
        Set(ByVal value As String)
            _DeviceString = value
            'LblString.Content = value
        End Set
    End Property

    Public Sub New()

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()
        Dim ImgSrc As ImageSource = New BitmapImage(New Uri("C:\ehome\Images\11.png"))
        Dim imgBrush As ImageBrush = New ImageBrush(ImgSrc)
        BtnON.Background = imgBrush
        BtnOFF.Background = imgBrush

        Dim dt As DispatcherTimer = New DispatcherTimer()
        AddHandler dt.Tick, AddressOf dispatcherTimer_Tick
        dt.Interval = New TimeSpan(0, 0, 1)
        dt.Start()
        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
    End Sub

    Public Sub dispatcherTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        If _TypeDevice <> eTypeDevice.Audio Then
            'Status = hs.DeviceStatus(_Adresse)
        Else
            '_DeviceString = hs.DeviceString(Adresse)
            LblStatus.Content = "ETAT: " & _DeviceString
        End If
    End Sub

    Private Sub BtnON_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If TypeDevice = eTypeDevice.OnOff Or TypeDevice = eTypeDevice.OnOffVariation Then
            'If hs.DeviceStatus(_Adresse) = 3 Then hs.ExecX10(_Adresse, "on")
        End If
    End Sub

#Region "Bouton ON/OFF"
    Private Sub BtnON_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
        Me.OnPreviewMouseDown(e)
    End Sub

    Private Sub BtnOFF_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOFF.Click
        If TypeDevice = eTypeDevice.OnOff Or TypeDevice = eTypeDevice.OnOffVariation Then
            'If hs.DeviceStatus(_Adresse) = 2 Then hs.ExecX10(_Adresse, "off")
        End If
    End Sub
#End Region

#Region "Bouton Audio"

    Private Sub BtnStop_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnStop.MouseDown
        'Dim s
        's = hs.RunEx("Multiroom.vb", "", "SDB|stop|a")
    End Sub

    Private Sub BtnPlay_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnPlay.MouseDown
        'Dim s
        's = hs.RunEx("Multiroom.vb", "", "SDB|play|http://streaming.radio.rtl.fr/rtl-1-44-96")
    End Sub

    Private Sub BtnPrecedent_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnPrecedent.MouseDown

    End Sub

    Private Sub BtnSuivant_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnSuivant.MouseDown

    End Sub

    Private Sub BtnPause_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnPause.MouseDown
        'Dim s
        's = hs.RunEx("Multiroom.vb", "", "SDB|pause|a")
    End Sub

    Private Sub BtnMute_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnMute.MouseDown
        'Dim s
        's = hs.RunEx("Multiroom.vb", "", "SDB|VOLUME MUTE|a")
    End Sub

    Private Sub BtnVolumDown_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnVolumDown.MouseDown
        'Dim s
        's = hs.RunEx("Multiroom.vb", "", "SDB|VOLUME DOWN|a")
    End Sub

    Private Sub BtnVolumUp_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnVolumUp.MouseDown
        'Dim s
        's = hs.RunEx("Multiroom.vb", "", "SDB|VOLUME UP|a")
    End Sub
#End Region
End Class
