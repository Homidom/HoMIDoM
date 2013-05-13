
Imports System.Windows.Threading
Imports System.Reflection
Imports System.Threading
Imports System.Net
Imports MjpegProcessor
Imports System.IO

Public Class uCamera
    Dim _URL As String = ""
    Dim _ListButton As New List(Of uHttp.ButtonHttp)
    Private _mjpeg As MjpegDecoder

    Public Property URL As String
        Get
            Return _URL
        End Get
        Set(ByVal value As String)
            Try
                _URL = value
                If String.IsNullOrEmpty(_URL) = False Then 'UrlIsValid(_URL) = False Then
                    If My.Computer.Network.Ping(_URL) = True Then
                        lbl.Visibility = Windows.Visibility.Collapsed
                        lbl.Content = ""
                        _mjpeg.ParseStream(New Uri(_URL))
                    Else
                        lbl.Content = "Ping de la caméra fail " & _URL & " est erronée !!"
                        lbl.Visibility = Windows.Visibility.Visible
                    End If
                Else
                    lbl.Content = "L'URL de la caméra doit être renseignée !!"
                    lbl.Visibility = Windows.Visibility.Visible
                End If
            Catch ex As Exception
                MessageBox.Show("Erreur uCamera.URL: " & ex.ToString, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End Set
    End Property

    Public Property ListButton As List(Of uHttp.ButtonHttp)
        Get
            Return _ListButton
        End Get
        Set(ByVal value As List(Of uHttp.ButtonHttp))
            _ListButton = value

            Try

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
            Catch ex As Exception
                MessageBox.Show("Erreur uCamera.ListButton.set: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End Set
    End Property

    Private Sub Button_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Try
            Dim x As uHttp.ButtonHttp = sender
            'If My.Computer.Network.IsAvailable = True And String.IsNullOrEmpty(x.URL) = False Then
            If String.IsNullOrEmpty(x.URL) = False Then
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
        Catch ex As Exception
            MessageBox.Show("Erreur uCamera.Button_Click: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub


    Public Sub New()

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        Try
            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            _mjpeg = New MjpegDecoder
            AddHandler _mjpeg.FrameReady, AddressOf mjpeg_FrameReady

        Catch ex As Exception
            MessageBox.Show("Erreur uCamera.New: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub uCamera_SizeChanged(ByVal sender As Object, ByVal e As System.Windows.SizeChangedEventArgs) Handles Me.SizeChanged
        Try

            Dim _size As Size = e.NewSize
            Dim y As Double = _size.Height

            If _ListButton.Count > 0 Then
                y = y - StkButton.ActualHeight - 30
            End If

            image.Width = _size.Width
            image.Height = y

        Catch ex As Exception
            MessageBox.Show("Erreur uCamera.uCamera_SizeChanged: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
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
