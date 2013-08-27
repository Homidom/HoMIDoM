Imports System.Xml
Imports System.Xml.XPath
Imports System.Windows.Threading
Imports System.IO
Imports System.Net
Imports System.Windows.Media.Animation

Partial Public Class uMeteo
    Dim dt As New DispatcherTimer
    Dim _Id As String = ""
    Dim _dev As HoMIDom.HoMIDom.TemplateDevice = Nothing
    Private myStoryboard As Storyboard

    Public Property ID As String
        Get
            Return _Id
        End Get
        Set(ByVal value As String)
            _Id = value
            GetMeteo2()
        End Set
    End Property

    Private Sub GetMeteo2()
        'Dim x As HoMIDom.HoMIDom.TemplateDevice = Nothing

        Try
            If IsConnect = True Then
                If String.IsNullOrEmpty(_Id) = False Then
                    _dev = ReturnDeviceById(_Id) 'myService.ReturnDeviceByID(IdSrv, _Id)

                    If _dev IsNot Nothing Then
                        'If IsDiff(_dev, x) Then
                        Lbl.Content = _dev.Name
                        LblTemps.Content = _dev.ConditionActuel

                        If String.IsNullOrEmpty(_dev.TemperatureActuel) = False Then
                            LblTemp.Content = _dev.TemperatureActuel & "°"
                        Else
                            LblTemp.Content = ""
                        End If

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

                        Dim chm As String = ""
                        If File.Exists(_MonRepertoire & "\Images\Meteo\" & _dev.IconActuel & ".png") = True Then
                            chm = _MonRepertoire & "\Images\Meteo\" & _dev.IconActuel & ".png"
                        Else
                            chm = _MonRepertoire & "\Images\Meteo\na.png"
                        End If
                        Ico.Source = LoadBitmapImage(chm)

                        If File.Exists(_MonRepertoire & "\Images\Meteo\" & _dev.IconToday & ".png") = True Then
                            chm = _MonRepertoire & "\Images\Meteo\" & _dev.IconToday & ".png"
                        Else
                            chm = _MonRepertoire & "\Images\Meteo\na.png"
                        End If
                        ImgD0.Source = LoadBitmapImage(chm)

                        If File.Exists(_MonRepertoire & "\Images\Meteo\" & _dev.IconJ1 & ".png") = True Then
                            chm = _MonRepertoire & "\Images\Meteo\" & _dev.IconJ1 & ".png"
                        Else
                            chm = _MonRepertoire & "\Images\Meteo\na.png"
                        End If
                        ImgD1.Source = LoadBitmapImage(chm)

                        If File.Exists(_MonRepertoire & "\Images\Meteo\" & _dev.IconJ2 & ".png") = True Then
                            chm = _MonRepertoire & "\Images\Meteo\" & _dev.IconJ2 & ".png"
                        Else
                            chm = _MonRepertoire & "\Images\Meteo\na.png"
                        End If
                        ImgD2.Source = LoadBitmapImage(chm)

                        If File.Exists(_MonRepertoire & "\Images\Meteo\" & _dev.IconJ3 & ".png") = True Then
                            chm = _MonRepertoire & "\Images\Meteo\" & _dev.IconJ3 & ".png"
                        Else
                            chm = _MonRepertoire & "\Images\Meteo\na.png"
                        End If
                        ImgD3.Source = LoadBitmapImage(chm)
                        'End If
                    End If

                End If
            End If

            '_dev = x
            'x = Nothing
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur uMeteo.GetMeteo: " & ex.ToString, "Erreur", "uMeteo.GetMeteo")
        End Try

    End Sub

    Public Sub New(ByVal vID As String)
        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ID = vID

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        GetMeteo2()

        dt.Interval = New TimeSpan(0, 0, 5)
        AddHandler dt.Tick, AddressOf dispatcherTimer_Tick
        dt.Start()

        Dim myDoubleAnimation As New DoubleAnimation()
        myDoubleAnimation.From = 0.0
        myDoubleAnimation.To = 1.0
        myDoubleAnimation.Duration = New Duration(TimeSpan.FromSeconds(1))

        myStoryboard = New Storyboard()
        myStoryboard.Children.Add(myDoubleAnimation)
        Storyboard.SetTarget(myDoubleAnimation, Me)
        Storyboard.SetTargetProperty(myDoubleAnimation, New PropertyPath(UserControl.OpacityProperty))
    End Sub

    Public Sub dispatcherTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        GetMeteo2()
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Private Sub uMeteo_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        myStoryboard.Begin()
    End Sub

    Private Sub uMeteo_Unloaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Unloaded
        If dt IsNot Nothing Then
            dt.Stop()
            RemoveHandler dt.Tick, AddressOf dispatcherTimer_Tick
            dt = Nothing
        End If
    End Sub

End Class
