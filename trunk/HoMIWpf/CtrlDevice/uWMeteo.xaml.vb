Imports System.Windows.Threading
Imports System.IO
Imports System.Threading

Public Class uWMeteo
    Dim _Id As String = ""
    Dim dt As DispatcherTimer
    Dim _dev As HoMIDom.HoMIDom.TemplateDevice = Nothing

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
                GetMeteo()
            End If

        End Set
    End Property

    Private Sub GetMeteo()
        Try
            If IsConnect = True Then
                If String.IsNullOrEmpty(_Id) = False Then
                    Dim x As HoMIDom.HoMIDom.TemplateDevice = myService.ReturnDeviceByID(IdSrv, _Id)

                    If x Is Nothing Then Exit Sub

                    If IsDiff(_dev, x) Then
                        Exit Sub
                    Else
                        _dev = x
                        x = Nothing
                    End If

                    LblVille.Content = _dev.Name
                    LblTemp.Content = _dev.TemperatureActuel & "°"
                    LblMin.Content = "Min: " & _dev.MinToday & "°"
                    LblMinJ1.Content = _dev.MinToday & "°"
                    LblMinJ2.Content = _dev.MinJ1 & "°"
                    LblMinJ3.Content = _dev.MinJ2 & "°"
                    LblMinJ4.Content = _dev.MinJ3 & "°"
                    LblMax.Content = "Max: " & _dev.MaxToday & "°"
                    LblMaxJ1.Content = _dev.MaxToday & "°"
                    LblMaxJ2.Content = _dev.MaxJ1 & "°"
                    LblMaxJ3.Content = _dev.MaxJ2 & "°"
                    LblMaxJ4.Content = _dev.MaxJ3 & "°"
                    LblJ1.Content = _dev.JourToday
                    LblJ2.Content = _dev.JourJ1
                    LblJ3.Content = _dev.JourJ2
                    LblJ4.Content = _dev.JourJ3

                    Dim chm As String = ""
                    If File.Exists(_MonRepertoire & "\Images\Meteo\" & _dev.IconActuel & ".png") = True Then
                        chm = _MonRepertoire & "\Images\Meteo\" & _dev.IconActuel & ".png"
                    Else
                        chm = _MonRepertoire & "\Images\Meteo\na.png"
                    End If
                    Icon.Source = LoadBitmapImage(chm)
                    chm = ""

                    If File.Exists(_MonRepertoire & "\Images\Meteo\" & _dev.IconToday & ".png") = True Then
                        chm = _MonRepertoire & "\Images\Meteo\" & _dev.IconToday & ".png"
                    Else
                        chm = _MonRepertoire & "\Images\Meteo\na.png"
                    End If
                    IconJ1.Source = LoadBitmapImage(chm)
                    chm = ""

                    If File.Exists(_MonRepertoire & "\Images\Meteo\" & _dev.IconJ1 & ".png") = True Then
                        chm = _MonRepertoire & "\Images\Meteo\" & _dev.IconJ1 & ".png"
                    Else
                        chm = _MonRepertoire & "\Images\Meteo\na.png"
                    End If
                    IconJ2.Source = LoadBitmapImage(chm)
                    chm = ""

                    If File.Exists(_MonRepertoire & "\Images\Meteo\" & _dev.IconJ2 & ".png") = True Then
                        chm = _MonRepertoire & "\Images\Meteo\" & _dev.IconJ2 & ".png"
                    Else
                        chm = _MonRepertoire & "\Images\Meteo\na.png"
                    End If
                    IconJ3.Source = LoadBitmapImage(chm)
                    chm = ""

                    If File.Exists(_MonRepertoire & "\Images\Meteo\" & _dev.IconJ3 & ".png") = True Then
                        chm = _MonRepertoire & "\Images\Meteo\" & _dev.IconJ3 & ".png"
                    Else
                        chm = _MonRepertoire & "\Images\Meteo\na.png"
                    End If
                    IconJ4.Source = LoadBitmapImage(chm)
                    chm = ""
                End If
            End If
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur UWMeteo.GetMeteo: " & ex.ToString, "Erreur", "UWMeteo.GetMeteo")
        End Try

    End Sub

    Public Sub dispatcherTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        GetMeteo()
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Private Sub uWMeteo_Unloaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Unloaded
        Try
            If dt IsNot Nothing Then
                dt.Stop()
                RemoveHandler dt.Tick, AddressOf dispatcherTimer_Tick
                dt = Nothing
            End If
            Icon.Source = Nothing
            IconJ1.Source = Nothing
            IconJ2.Source = Nothing
            IconJ3.Source = Nothing
            IconJ4.Source = Nothing
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur uWMeteo_Unloaded: " & ex.ToString, "Erreur", "uWMeteo_Unloaded")
        End Try
    End Sub
End Class
