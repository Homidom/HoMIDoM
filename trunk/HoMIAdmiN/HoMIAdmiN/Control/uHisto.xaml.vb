Imports ZedGraph
Imports System.Windows.Controls.DataVisualization.Charting

Public Class uHisto
    Public Event CloseMe(ByVal MyObject As Object)

    Public Sub New(ByVal y As GraphPane)

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        Dim x As New ZedGraphControl
        x.Name = "Graph"
        x.Width = 650
        x.Height = 400

        y.Fill = New ZedGraph.Fill(System.Drawing.Color.DarkGray)
        x.GraphPane = y
        x.AxisChange()
        host.Child = x

    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me)
    End Sub

    Private Sub UserControl_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded

    End Sub
End Class
