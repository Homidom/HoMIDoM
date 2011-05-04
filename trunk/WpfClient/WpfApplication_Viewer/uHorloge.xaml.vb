Imports System
Imports System.Windows.Threading

Partial Public Class uHorloge
    Dim Rnd As New Random

    Public Sub New()

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        LblTime.Content = Now.ToLongTimeString

        Dim dt As DispatcherTimer = New DispatcherTimer()
        AddHandler dt.Tick, AddressOf dispatcherTimer_Tick
        dt.Interval = New TimeSpan(0, 0, 1)
        dt.Start()
    End Sub

    Public Sub dispatcherTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        LblTime.Content = Now.ToLongTimeString
        Dim x As Double = Rnd.Next(70, 400)
        Dim y As Double = Rnd.Next(50, 300)
        Canvas1.SetLeft(LblTime, x)
        Canvas1.SetTop(LblTime, y)
    End Sub
End Class
