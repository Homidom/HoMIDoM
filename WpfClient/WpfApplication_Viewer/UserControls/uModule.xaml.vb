Imports System
Imports System.Windows.Threading
Imports System.IO

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
    Dim _DimValue As Integer = 0
    Dim _Script As String
    Dim _Fonction As String
    Dim _Command As String

    Public Enum eTypeDevice
        OnOff = 0
        OnOffVariation = 1
        Capteur = 2
        Audio = 3
        Temperature = 4
        Volet = 5
        Macro = 6
    End Enum

#Region "Property"

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
            Select Case value
                Case eTypeDevice.OnOff
                    AfficheStk("DEVICE")
                    LblPercent.Visibility = Windows.Visibility.Hidden
                    BtnMoins.Visibility = Windows.Visibility.Hidden
                    BtnPlus.Visibility = Windows.Visibility.Hidden
                Case eTypeDevice.OnOffVariation
                    AfficheStk("DEVICE")
                Case eTypeDevice.Capteur
                    AfficheStk("CAPTEUR")
                    LblPercent.Visibility = Windows.Visibility.Hidden
                Case eTypeDevice.Audio
                    AfficheStk("AUDIO")
                    LblPercent.Visibility = Windows.Visibility.Hidden
                Case eTypeDevice.Macro
                    AfficheStk("MACRO")
                    LblPercent.Visibility = Windows.Visibility.Hidden
                    LblStatus.Visibility = Windows.Visibility.Hidden
                Case eTypeDevice.Temperature
                    AfficheStk("TEMPERATURE")
                    LblPercent.Visibility = Windows.Visibility.Hidden
                Case eTypeDevice.Volet
                    AfficheStk("VOLET")
                    LblPercent.Visibility = Windows.Visibility.Hidden
            End Select
        End Set
    End Property

    Private Sub AfficheStk(ByVal Tag As String)
        Try
            Dim j As Integer = StkPanel.Children.Count

            For k As Integer = 0 To j
                For i As Integer = 0 To StkPanel.Children.Count - 1
                    Dim x As StackPanel = StkPanel.Children.Item(i)
                    If x.Tag <> UCase(Tag) And x.Tag <> "TITRE" And x.Tag <> "LABEL" Then
                        StkPanel.Children.RemoveAt(i)
                        Exit For
                    End If
                Next
            Next
        Catch ex As Exception
            MsgBox("Error AfficheStk: " & ex.ToString & vbCrLf)
        End Try
    End Sub

    Public Property Icon() As String
        Get
            Return _Icon
        End Get
        Set(ByVal value As String)
            _Icon = value
            If _Icon <> "" Then
                If File.Exists(_Icon) Then
                    Dim bmpImage As New BitmapImage()
                    bmpImage.BeginInit()
                    bmpImage.UriSource = New Uri(_Icon, UriKind.Absolute)
                    bmpImage.EndInit()
                    Ico.Source = bmpImage
                End If
            End If
        End Set
    End Property

    Public Property Status() As Integer
        Get
            Return _Status
        End Get
        Set(ByVal value As Integer)
            _Status = value
            If _TypeDevice <> eTypeDevice.Audio And _TypeDevice <> eTypeDevice.Temperature Then
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
                If _TypeDevice = eTypeDevice.Audio Then LblStatus.Content = "STATUS: " & _DeviceString
                If _TypeDevice = eTypeDevice.Temperature Then LblStatus.Content = "VALEUR: " & _DeviceString & " °C"
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
            Else
            End If
        End Set
    End Property

    Public Property DeviceValue() As String
        Get
            Return _DeviceValue
        End Get
        Set(ByVal value As String)
            _DeviceValue = value
        End Set
    End Property

    Public Property DeviceString() As String
        Get
            Return _DeviceString
        End Get
        Set(ByVal value As String)
            _DeviceString = value
        End Set
    End Property

    Public Property Script() As String
        Get
            Return _Script
        End Get
        Set(ByVal value As String)
            _Script = value
        End Set
    End Property

    Public Property Fonction() As String
        Get
            Return _Fonction
        End Get
        Set(ByVal value As String)
            _Fonction = value
        End Set
    End Property

    Public Property Command() As String
        Get
            Return _Command
        End Get
        Set(ByVal value As String)
            _Command = value
        End Set
    End Property
#End Region

    Public Sub New()

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        Dim dt As DispatcherTimer = New DispatcherTimer()
        AddHandler dt.Tick, AddressOf dispatcherTimer_Tick
        dt.Interval = New TimeSpan(0, 0, 1)
        dt.Start()
    End Sub

    Public Sub dispatcherTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        Try
            If _TypeDevice = eTypeDevice.Macro Then Exit Sub
            If _TypeDevice < eTypeDevice.Audio Then
                Status = hs.DeviceStatus(_Adresse)
                If _TypeDevice = eTypeDevice.OnOffVariation Then
                    Dim i As Integer = hs.DeviceValue(Adresse)
                    LblPercent.Content = i & "%"
                    If _DimValue <> i Then _DimValue = i
                End If
            Else
                _DeviceString = hs.DeviceString(Adresse)
                If _TypeDevice = eTypeDevice.Audio Then LblStatus.Content = "STATUS: " & _DeviceString
                If _TypeDevice = eTypeDevice.Temperature Then LblStatus.Content = "TEMPERATURE: " & _DeviceString & " °C"
            End If

        Catch ex As Exception
            MsgBox("Error dispatcherTimer_Tick: " & ex.ToString & vbCrLf)
        End Try
    End Sub


#Region "Bouton ON/OFF"

    Private Sub BtnOFF_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOFF.Click
        If TypeDevice = eTypeDevice.OnOff Or TypeDevice = eTypeDevice.OnOffVariation Then
            hs.ExecX10(_Adresse, "off", 0, 0)
            _DimValue = 0
        End If
    End Sub

    Private Sub BtnON_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnON.Click
        If TypeDevice = eTypeDevice.OnOff Or TypeDevice = eTypeDevice.OnOffVariation Then
            hs.ExecX10(_Adresse, "on", 0, 0)
            _DimValue = 100
        End If
    End Sub
#End Region

#Region "Bouton Audio"

    Private Sub BtnStop_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnStop.MouseDown
        Dim s
        s = hs.RunEx("Multiroom.vb", "", "SDB|stop|a")
    End Sub

    Private Sub BtnPlay_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnPlay.MouseDown
        Dim s
        s = hs.RunEx("Multiroom.vb", "", "SDB|play|http://streaming.radio.rtl.fr/rtl-1-44-96")
    End Sub

    Private Sub BtnPause_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnPause.MouseDown
        Dim s
        s = hs.RunEx("Multiroom.vb", "", "SDB|pause|a")
    End Sub

    Private Sub BtnMute_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnMute.MouseDown
        Dim s
        s = hs.RunEx("Multiroom.vb", "", "SDB|VOLUME MUTE|a")
    End Sub

    Private Sub BtnVolumDown_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnVolumDown.MouseDown
        Dim s
        s = hs.RunEx("Multiroom.vb", "", "SDB|VOLUME DOWN|a")
    End Sub

    Private Sub BtnVolumUp_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnVolumUp.MouseDown
        Dim s
        s = hs.RunEx("Multiroom.vb", "", "SDB|VOLUME UP|a")
    End Sub
#End Region

#Region "Bouton Variation"

    Private Sub BtnPlus_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnPlus.Click
        _DimValue += 10
        If _DimValue > 100 Then _DimValue = 100
        hs.ExecX10(_Adresse, "ddim", _DimValue, 0)
    End Sub

    Private Sub BtnMoins_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnMoins.Click
        _DimValue -= 10
        If _DimValue < 0 Then _DimValue = 0
        If _DimValue > 0 Then
            hs.ExecX10(_Adresse, "ddim", _DimValue, 0)
        Else
            hs.ExecX10(_Adresse, "off", 0, 0)
        End If
    End Sub
#End Region

#Region "Volet"

    Private Sub Volet0_Click(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Volet0.MouseDown
        hs.ExecX10(_Adresse, "off", 0, 0)
    End Sub

    Private Sub Volet25_Click(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Volet25.MouseDown
        hs.ExecX10(_Adresse, "extended", 10, 3)
    End Sub

    Private Sub Volet50_Click(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Volet50.MouseDown
        hs.ExecX10(_Adresse, "extended", 16, 3)
    End Sub

    Private Sub Volet75_Click(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Volet75.MouseDown
        hs.ExecX10(_Adresse, "extended", 20, 3)
    End Sub

    Private Sub Volet100_Click(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Volet100.MouseDown
        hs.ExecX10(_Adresse, "on", 0, 0)
    End Sub
#End Region

#Region "Macro"
    Private Sub BtnRun_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnRun.Click
        Dim s
        s = hs.RunEx(_Script, _Fonction, _Command)
    End Sub
#End Region
End Class
