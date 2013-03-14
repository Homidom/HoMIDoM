
Imports System.Windows.Threading
Imports System.Reflection
Imports System.Threading
Imports System.Net
Imports MjpegProcessor
Imports System.IO

Public Class uCamera
    Dim _URL As String = ""
    Dim _Refresh As Integer = 0
    Dim _ListButton As New List(Of uHttp.ButtonHttp)
    Private _mjpeg As MjpegDecoder

    Public Property URL As String
        Get
            Return _URL
        End Get
        Set(ByVal value As String)
            Try
                _URL = value
                If String.IsNullOrEmpty(_URL) = False And UrlIsValid(_URL) Then
                    _mjpeg.ParseStream(New Uri(_URL))
                End If

            Catch ex As Exception
                MessageBox.Show("Erreur uCamera.URL: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End Set
    End Property

    Public Property ListButton As List(Of uHttp.ButtonHttp)
        Get
            Return _ListButton
        End Get
        Set(ByVal value As List(Of uHttp.ButtonHttp))
            _ListButton = value

            If _ListButton IsNot Nothing Then
                StkButton.Children.Clear()
                For Each _button As uHttp.ButtonHttp In _ListButton
                    Dim x As New uHttp.ButtonHttp
                    x.Foreground = Brushes.White
                    x.Margin = New Thickness(5)
                    x.Height = _button.Height
                    x.Width = _button.Width
                    x.Content = _button.Content
                    x.URL = _button.URL
                    x.SetResourceReference(Control.TemplateProperty, "GlassButton")
                    AddHandler x.Click, AddressOf Button_Click
                    StkButton.Children.Add(x)
                Next
            End If
        End Set
    End Property

    Private Sub Button_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Try
            Dim x As uHttp.ButtonHttp = sender
            If My.Computer.Network.IsAvailable = True And String.IsNullOrEmpty(x.URL) = False Then
                If UrlIsValid(x.URL) Then
                    Dim reader As StreamReader = Nothing
                    Dim str As String = ""
                    Dim request As WebRequest = WebRequest.Create(URL)
                    Dim response As WebResponse = request.GetResponse()

                    reader = New StreamReader(response.GetResponseStream())
                    str = reader.ReadToEnd
                    reader.Close()

                Else
                    MessageBox.Show("Erreur l'url: " & x.URL & " n'est pas valide", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur uCamera.Button_Click: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub


    Public Sub New()

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        _mjpeg = New MjpegDecoder
        AddHandler _mjpeg.FrameReady, AddressOf mjpeg_FrameReady
    End Sub

    Private Sub uCamera_SizeChanged(ByVal sender As Object, ByVal e As System.Windows.SizeChangedEventArgs) Handles Me.SizeChanged
        Dim _size As Size = e.NewSize
        Dim y As Double = _size.Height

        If _ListButton.Count > 0 Then
            y = y - StkButton.ActualHeight - 30
        End If

        image.Width = _size.Width
        image.Height = y
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Private Sub uCamera_Unloaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Unloaded
        _mjpeg = Nothing
    End Sub

    Private Sub mjpeg_FrameReady(ByVal sender As Object, ByVal e As FrameReadyEventArgs)
        Try
            image.Source = e.BitmapImage
        Catch ex As Exception
            MessageBox.Show("Erreur uCamera_mjpeg_FrameReady: " & ex.ToString)
        End Try
    End Sub

End Class
