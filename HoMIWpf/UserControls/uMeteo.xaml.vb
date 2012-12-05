Imports System.Xml
Imports System.Xml.XPath
Imports System.Windows.Threading
Imports System.IO
Imports System.Net

Partial Public Class uMeteo

    Dim _Id As String

    Public Property ID As String
        Get
            Return _Id
        End Get
        Set(ByVal value As String)
            _Id = value
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
                    If File.Exists(_MonRepertoire & "\Images\Meteo\" & _dev.IconActuel & ".png") = True Then
                        bmpImage.UriSource = New Uri(_MonRepertoire & "\Images\Meteo\" & _dev.IconActuel & ".png", UriKind.Absolute)
                    Else
                        bmpImage.UriSource = New Uri(_MonRepertoire & "\Images\Meteo\na.png", UriKind.Absolute)
                    End If
                    bmpImage.EndInit()
                    Ico.Source = bmpImage

                    Dim bmpImage2 As New BitmapImage()
                    bmpImage2.BeginInit()
                    If File.Exists(_MonRepertoire & "\Images\Meteo\" & _dev.IconToday & ".png") = True Then
                        bmpImage2.UriSource = New Uri(_MonRepertoire & "\Images\Meteo\" & _dev.IconToday & ".png", UriKind.Absolute)
                    Else
                        bmpImage2.UriSource = New Uri(_MonRepertoire & "\Images\Meteo\na.png", UriKind.Absolute)
                    End If
                    bmpImage2.EndInit()
                    ImgD0.Source = bmpImage2

                    Dim bmpImage3 As New BitmapImage()
                    bmpImage3.BeginInit()
                    If File.Exists(_MonRepertoire & "\Images\Meteo\" & _dev.IconJ1 & ".png") = True Then
                        bmpImage3.UriSource = New Uri(_MonRepertoire & "\Images\Meteo\" & _dev.IconJ1 & ".png", UriKind.Absolute)
                    Else
                        bmpImage3.UriSource = New Uri(_MonRepertoire & "\Images\Meteo\na.png", UriKind.Absolute)
                    End If
                    bmpImage3.EndInit()
                    ImgD1.Source = bmpImage3

                    Dim bmpImage4 As New BitmapImage()
                    bmpImage4.BeginInit()
                    If File.Exists(_MonRepertoire & "\Images\Meteo\" & _dev.IconJ2 & ".png") = True Then
                        bmpImage4.UriSource = New Uri(_MonRepertoire & "\Images\Meteo\" & _dev.IconJ2 & ".png", UriKind.Absolute)
                    Else
                        bmpImage4.UriSource = New Uri(_MonRepertoire & "\Images\Meteo\na.png", UriKind.Absolute)
                    End If
                    bmpImage4.EndInit()
                    ImgD2.Source = bmpImage4

                    Dim bmpImage5 As New BitmapImage()
                    bmpImage5.BeginInit()
                    If File.Exists(_MonRepertoire & "\Images\Meteo\" & _dev.IconJ3 & ".png") = True Then
                        bmpImage5.UriSource = New Uri(_MonRepertoire & "\Images\Meteo\" & _dev.IconJ3 & ".png", UriKind.Absolute)
                    Else
                        bmpImage5.UriSource = New Uri(_MonRepertoire & "\Images\Meteo\na.png", UriKind.Absolute)
                    End If
                    bmpImage5.EndInit()
                    ImgD3.Source = bmpImage5
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

    Public Sub New(ByVal ID As String)

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()
        _Id = ID

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        Dim dt As DispatcherTimer = New DispatcherTimer()
        AddHandler dt.Tick, AddressOf dispatcherTimer_Tick
        dt.Interval = New TimeSpan(0, 5, 0)
        dt.Start()
    End Sub

    Public Sub dispatcherTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        GetMeteo2()
    End Sub

    Private Sub uMeteo_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        If _Id <> "" Then GetMeteo2()
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
