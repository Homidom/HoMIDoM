Imports System.Windows.Threading
Imports System.IO
Imports System.Threading

Public Class uWMeteo
    Dim _Id As String = ""
    Dim dt As DispatcherTimer

    Public Property ID As String
        Get
            Return _Id
        End Get
        Set(ByVal value As String)
            _Id = value

            If String.IsNullOrEmpty(_Id) = False Then
                
                GetMeteo()
            End If

        End Set
    End Property

    Private Sub GetMeteo()
        Try
            If IsConnect = True Then
                If String.IsNullOrEmpty(_Id) = False Then
                    Dim x As HoMIDom.HoMIDom.TemplateDevice = ReturnDeviceById(_Id)

                    If x IsNot Nothing Then

                        LblVille.Content = x.Name
                        LblTemp.Content = x.TemperatureActuel & "°"
                        LblMin.Content = "Min: " & x.MinToday & "°"
                        LblMinJ1.Content = x.MinToday & "°"
                        LblMinJ2.Content = x.MinJ1 & "°"
                        LblMinJ3.Content = x.MinJ2 & "°"
                        LblMinJ4.Content = x.MinJ3 & "°"
                        LblMax.Content = "Max: " & x.MaxToday & "°"
                        LblMaxJ1.Content = x.MaxToday & "°"
                        LblMaxJ2.Content = x.MaxJ1 & "°"
                        LblMaxJ3.Content = x.MaxJ2 & "°"
                        LblMaxJ4.Content = x.MaxJ3 & "°"
                        LblJ1.Content = x.JourToday
                        LblJ2.Content = x.JourJ1
                        LblJ3.Content = x.JourJ2
                        LblJ4.Content = x.JourJ3

                        Dim chm As String = ""
                        If File.Exists(_MonRepertoire & "\Images\Meteo\" & x.IconActuel & ".png") = True Then
                            chm = _MonRepertoire & "\Images\Meteo\" & x.IconActuel & ".png"
                        Else
                            chm = _MonRepertoire & "\Images\Meteo\na.png"
                        End If
                        Icon.Source = LoadBitmapImage(chm)
                        chm = ""

                        If File.Exists(_MonRepertoire & "\Images\Meteo\" & x.IconToday & ".png") = True Then
                            chm = _MonRepertoire & "\Images\Meteo\" & x.IconToday & ".png"
                        Else
                            chm = _MonRepertoire & "\Images\Meteo\na.png"
                        End If
                        IconJ1.Source = LoadBitmapImage(chm)
                        chm = ""

                        If File.Exists(_MonRepertoire & "\Images\Meteo\" & x.IconJ1 & ".png") = True Then
                            chm = _MonRepertoire & "\Images\Meteo\" & x.IconJ1 & ".png"
                        Else
                            chm = _MonRepertoire & "\Images\Meteo\na.png"
                        End If
                        IconJ2.Source = LoadBitmapImage(chm)
                        chm = ""

                        If File.Exists(_MonRepertoire & "\Images\Meteo\" & x.IconJ2 & ".png") = True Then
                            chm = _MonRepertoire & "\Images\Meteo\" & x.IconJ2 & ".png"
                        Else
                            chm = _MonRepertoire & "\Images\Meteo\na.png"
                        End If
                        IconJ3.Source = LoadBitmapImage(chm)
                        chm = ""

                        If File.Exists(_MonRepertoire & "\Images\Meteo\" & x.IconJ3 & ".png") = True Then
                            chm = _MonRepertoire & "\Images\Meteo\" & x.IconJ3 & ".png"
                        Else
                            chm = _MonRepertoire & "\Images\Meteo\na.png"
                        End If
                        IconJ4.Source = LoadBitmapImage(chm)
                        chm = ""
                    End If
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

    Public Sub New()

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        dt = New DispatcherTimer()
        AddHandler dt.Tick, AddressOf dispatcherTimer_Tick
        dt.Interval = New TimeSpan(0, 0, 5)
        dt.Start()
    End Sub
End Class
