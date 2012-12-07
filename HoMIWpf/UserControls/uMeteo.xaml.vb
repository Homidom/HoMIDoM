Imports System.Xml
Imports System.Xml.XPath
Imports System.Windows.Threading
Imports System.IO
Imports System.Net

Partial Public Class uMeteo
    Dim dt As DispatcherTimer
    Dim _Id As String

    Public Property ID As String
        Get
            Return _Id
        End Get
        Set(ByVal value As String)
            _Id = value
            If String.IsNullOrEmpty(_Id) = False Then
                If dt Is Nothing Then
                    dt = New DispatcherTimer()
                    AddHandler dt.Tick, AddressOf dispatcherTimer_Tick
                    dt.Interval = New TimeSpan(0, 5, 0)
                    dt.Start()
                End If
                GetMeteo2()
            End If
        End Set
    End Property

    Private Sub GetMeteo2()
        Try

            If IsConnect = True Then
                If _Id <> "" Then
                    Dim _dev As HoMIDom.HoMIDom.TemplateDevice = myService.ReturnDeviceByID(IdSrv, _Id)
                    Lbl.Content = _dev.Name
                    LblTemps.Content = _dev.ConditionActuel
                    LblTemp.Content = _dev.TemperatureActuel & "°"
                    LblHum.Content = "Humidité: " & _dev.HumiditeActuel
                    LblVent.Content = "Vent: " & _dev.VentActuel
                    Day0.Content = _dev.JourToday
                    Day1.Content = _dev.JourJ1
                    Day2.Content = _dev.JourJ2
                    Day3.Content = _dev.JourJ3
                    Lbl0Min.Content = "Min: " & _dev.MinToday & "°"
                    MinD0.Content = _dev.MinToday & "°"
                    MinD1.Content = _dev.MinJ1 & "°"
                    MinD2.Content = _dev.MinJ2 & "°"
                    MinD3.Content = _dev.MinJ3 & "°"
                    MaxD0.Content = _dev.MaxToday & "°"
                    Lbl0Max.Content = "Max: " & _dev.MaxToday & "°"
                    MaxD0.Content = _dev.MaxToday & "°"
                    MaxD1.Content = _dev.MaxJ1 & "°"
                    MaxD2.Content = _dev.MaxJ2 & "°"
                    MaxD3.Content = _dev.MaxJ3 & "°"

                    Dim bmpImage As New BitmapImage()
                    bmpImage.BeginInit()
                    bmpImage.CacheOption = BitmapCacheOption.OnLoad
                    bmpImage.CreateOptions = BitmapCreateOptions.DelayCreation
                    If File.Exists(_MonRepertoire & "\Images\Meteo\" & _dev.IconActuel & ".png") = True Then
                        bmpImage.UriSource = New Uri(_MonRepertoire & "\Images\Meteo\" & _dev.IconActuel & ".png", UriKind.Absolute)
                    Else
                        bmpImage.UriSource = New Uri(_MonRepertoire & "\Images\Meteo\na.png", UriKind.Absolute)
                    End If
                    bmpImage.EndInit()
                    If bmpImage.CanFreeze Then bmpImage.Freeze()
                    Ico.Source = bmpImage
                    bmpImage = Nothing

                    Dim bmpImage2 As New BitmapImage()
                    bmpImage2.BeginInit()
                    bmpImage2.CacheOption = BitmapCacheOption.OnLoad
                    bmpImage2.CreateOptions = BitmapCreateOptions.DelayCreation
                    If File.Exists(_MonRepertoire & "\Images\Meteo\" & _dev.IconToday & ".png") = True Then
                        bmpImage2.UriSource = New Uri(_MonRepertoire & "\Images\Meteo\" & _dev.IconToday & ".png", UriKind.Absolute)
                    Else
                        bmpImage2.UriSource = New Uri(_MonRepertoire & "\Images\Meteo\na.png", UriKind.Absolute)
                    End If
                    bmpImage2.EndInit()
                    If bmpImage2.CanFreeze Then bmpImage2.Freeze()
                    ImgD0.Source = bmpImage2
                    bmpImage2 = Nothing

                    Dim bmpImage3 As New BitmapImage()
                    bmpImage3.BeginInit()
                    bmpImage3.CacheOption = BitmapCacheOption.OnLoad
                    bmpImage3.CreateOptions = BitmapCreateOptions.DelayCreation
                    If File.Exists(_MonRepertoire & "\Images\Meteo\" & _dev.IconJ1 & ".png") = True Then
                        bmpImage3.UriSource = New Uri(_MonRepertoire & "\Images\Meteo\" & _dev.IconJ1 & ".png", UriKind.Absolute)
                    Else
                        bmpImage3.UriSource = New Uri(_MonRepertoire & "\Images\Meteo\na.png", UriKind.Absolute)
                    End If
                    bmpImage3.EndInit()
                    If bmpImage3.CanFreeze Then bmpImage3.Freeze()
                    ImgD1.Source = bmpImage3
                    bmpImage3 = Nothing

                    Dim bmpImage4 As New BitmapImage()
                    bmpImage4.BeginInit()
                    bmpImage4.CacheOption = BitmapCacheOption.OnLoad
                    bmpImage4.CreateOptions = BitmapCreateOptions.DelayCreation
                    If File.Exists(_MonRepertoire & "\Images\Meteo\" & _dev.IconJ2 & ".png") = True Then
                        bmpImage4.UriSource = New Uri(_MonRepertoire & "\Images\Meteo\" & _dev.IconJ2 & ".png", UriKind.Absolute)
                    Else
                        bmpImage4.UriSource = New Uri(_MonRepertoire & "\Images\Meteo\na.png", UriKind.Absolute)
                    End If
                    bmpImage4.EndInit()
                    If bmpImage4.CanFreeze Then bmpImage4.Freeze()
                    ImgD2.Source = bmpImage4
                    bmpImage4 = Nothing

                    Dim bmpImage5 As New BitmapImage()
                    bmpImage5.BeginInit()
                    bmpImage5.CacheOption = BitmapCacheOption.OnLoad
                    bmpImage5.CreateOptions = BitmapCreateOptions.DelayCreation
                    If File.Exists(_MonRepertoire & "\Images\Meteo\" & _dev.IconJ3 & ".png") = True Then
                        bmpImage5.UriSource = New Uri(_MonRepertoire & "\Images\Meteo\" & _dev.IconJ3 & ".png", UriKind.Absolute)
                    Else
                        bmpImage5.UriSource = New Uri(_MonRepertoire & "\Images\Meteo\na.png", UriKind.Absolute)
                    End If
                    bmpImage5.EndInit()
                    If bmpImage5.CanFreeze Then bmpImage5.Freeze()
                    ImgD3.Source = bmpImage5
                    bmpImage5 = Nothing
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur GetMeteo: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try

    End Sub

    Private Function TraduireJour(ByVal Jour As String) As String
        TraduireJour = "?"
        Select Case Jour
            Case "Thu"
                TraduireJour = "Jeu"
            Case "Fri"
                TraduireJour = "Ven"
            Case "Sat"
                TraduireJour = "Sam"
            Case "Sun"
                TraduireJour = "Dim"
            Case "Mon"
                TraduireJour = "Lun"
            Case "Tue"
                TraduireJour = "Mar"
            Case "Wed"
                TraduireJour = "Mer"
        End Select
    End Function

    Public Sub New(ByVal vID As String)

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()
        ID = vID
        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
    End Sub

    Public Sub dispatcherTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        GetMeteo2()
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Private Sub uMeteo_Unloaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Unloaded
        If dt IsNot Nothing Then
            dt.Stop()
            RemoveHandler dt.Tick, AddressOf dispatcherTimer_Tick
            dt = Nothing
        End If
    End Sub
End Class
