Imports System.Windows.Threading
Imports System.Reflection
Imports System.Threading
Imports System.Net

Public Class uHttp
    Dim _URL As String = ""
    Dim _Refresh As Integer = 0
    Dim dt As DispatcherTimer
    Dim _ListButton As New List(Of ButtonHttp)

    Public Property URL As String
        Get
            Return _URL
        End Get
        Set(ByVal value As String)
            Try
                _URL = value
                If String.IsNullOrEmpty(_URL) = False Then
                    If _URL.ToLower().StartsWith("www.") Then _URL = "http://" & _URL
                    If My.Computer.Network.IsAvailable = True And String.IsNullOrEmpty(_URL) = False Then
                        WebBrowser1.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, DirectCast(Sub() WebBrowser1.Navigate(New Uri(_URL)), ThreadStart))
                    End If
                End If
            Catch ex As Exception
                AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uHttp.URL: " & ex.ToString, "Erreur", "uHttp.URL")
            End Try
        End Set
    End Property

    Public Property Refresh As Integer
        Get
            Return _Refresh
        End Get
        Set(ByVal value As Integer)
            _Refresh = value
            Try
                If value > 0 And value < 3600 Then
                    AddHandler dt.Tick, AddressOf dispatcherTimer_Tick
                    dt.IsEnabled = True
                    dt.Stop()
                    dt.Interval = New TimeSpan(0, 0, _Refresh)
                    dt.Start()
                Else
                    If dt.IsEnabled = True Then
                        dt.IsEnabled = False
                        dt.Stop()
                        RemoveHandler dt.Tick, AddressOf dispatcherTimer_Tick
                    End If
                End If
            Catch ex As Exception
                AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uHttp.Refresh.set: " & ex.ToString, "Erreur", "uHttp.Refresh.set")
            End Try
        End Set
    End Property

    Public Property ListButton As List(Of ButtonHttp)
        Get
            Return _ListButton
        End Get
        Set(ByVal value As List(Of ButtonHttp))
            Try
                _ListButton = value

                If _ListButton IsNot Nothing Then
                    StkButton.Children.Clear()
                    For Each _button As ButtonHttp In _ListButton
                        Dim x As New ButtonHttp
                        x.Foreground = Brushes.White
                        x.Margin = New Thickness(5)
                        x.Height = _button.Height
                        x.Width = _button.Width
                        x.Content = _button.Content
                        'x.IconImageUri = _button.IconImageUri
                        x.URL = _button.URL
                        x.SetResourceReference(Control.TemplateProperty, "GlassButton")
                        AddHandler x.Click, AddressOf Button_Click
                        StkButton.Children.Add(x)
                    Next
                End If
            Catch ex As Exception
                AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uHttp.ListButton.set: " & ex.ToString, "Erreur", "uHttp.ListButton.set")
            End Try
        End Set
    End Property

    Private Sub Button_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Try
            Dim x As ButtonHttp = sender
            If My.Computer.Network.IsAvailable = True And String.IsNullOrEmpty(x.URL) = False Then
                If UrlIsValid(x.URL) Then
                    Dim _tpurl As String = x.URL
                    If _tpurl.ToLower().StartsWith("www.") Then _tpurl = "http://" & _tpurl
                    WebBrowser1.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, DirectCast(Sub() WebBrowser1.Navigate(New Uri(_tpurl)), ThreadStart))
                Else
                    MessageBox.Show("Erreur l'url: " & x.URL & " n'est pas valide", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
                End If
            End If
        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uHttp.Button_Click: " & ex.ToString, "Erreur", "uHttp.Button_Click")
        End Try
    End Sub

    Private Sub wb_Navigated(ByVal sender As Object, ByVal e As System.Windows.Navigation.NavigationEventArgs) Handles WebBrowser1.Navigated
        Try
            SuppressScriptErrors(sender, True)
            GC.Collect()

        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uHttp.wb_Navigated: " & ex.ToString, "Erreur", "uHttp.wb_Navigated")
        End Try
    End Sub

    Private Sub wb_Navigating(ByVal sender As Object, ByVal e As System.Windows.Navigation.NavigatingCancelEventArgs) Handles WebBrowser1.Navigating
        Try
            SuppressScriptErrors(sender, True)
        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uHttp.wb_Navigating: " & ex.ToString, "Erreur", "uHttp.wb_Navigating")
        End Try
    End Sub

    Sub SuppressScriptErrors(ByVal wb As Controls.WebBrowser, ByVal Hide As Boolean)
        Try
            Dim fiComWebBrowser As FieldInfo = GetType(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance Or BindingFlags.NonPublic)
            If fiComWebBrowser Is Nothing Then
                Return
            End If
            Dim objComWebBrowser As Object = fiComWebBrowser.GetValue(wb)
            If objComWebBrowser Is Nothing Then
                Return
            End If
            objComWebBrowser.[GetType]().InvokeMember("Silent", BindingFlags.SetProperty, Nothing, objComWebBrowser, New Object() {Hide})
        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uHttp.SuppressScriptErrors: " & ex.ToString, "Erreur", "uHttp.SuppressScriptErrors")
        End Try
    End Sub

    Public Sub dispatcherTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        Try
            If WebBrowser1.Document IsNot Nothing Then WebBrowser1.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, DirectCast(Sub() WebBrowser1.Refresh(), ThreadStart))
        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uHttp.dispatcherTimer_Tick: " & ex.ToString, "Erreur", "uHttp.dispatcherTimer_Tick")
        End Try
    End Sub


    Public Sub New()

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        dt = New DispatcherTimer()
        dt.IsEnabled = False
    End Sub

    Public Class ButtonHttp
        Inherits Button

        Dim _Url As String = ""

        Public Property URL As String
            Get
                Return _Url
            End Get
            Set(ByVal value As String)
                _Url = value
            End Set
        End Property

        Public Sub New()
            Me.Foreground = Brushes.White
        End Sub
    End Class

    Private Sub uHttp_SizeChanged(ByVal sender As Object, ByVal e As System.Windows.SizeChangedEventArgs) Handles Me.SizeChanged
        Try
            Dim _size As Size = e.NewSize
            Dim y As Double = _size.Height

            If _ListButton.Count > 0 Then
                y = y - StkButton.ActualHeight - 30
            End If

            WebBrowser1.Height = y
        Catch ex As Exception
            AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uHttp.uHttp_SizeChanged: " & ex.ToString, "Erreur", "uHttp.uHttp_SizeChanged")
        End Try

    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Private Sub uHttp_Unloaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Unloaded
        dt.Stop()
        WebBrowser1.Dispose()
    End Sub
End Class

