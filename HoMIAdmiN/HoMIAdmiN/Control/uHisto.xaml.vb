Imports ZedGraph
Imports System.Windows.Controls.DataVisualization.Charting

Public Class uHisto
    Public Event CloseMe(ByVal MyObject As Object)

    Public Sub New(ByVal y As GraphPane)

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        Try

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        Dim x As New ZedGraphControl
        x.Name = "Graph"
        x.Width = 650
        x.Height = 400

        y.Fill = New ZedGraph.Fill(System.Drawing.Color.DarkGray)
        x.GraphPane = y
        x.AxisChange()
        host.Child = x

            'Dim pieSeries As LineSeries = New LineSeries()
            'Dim KeyBind As Binding = New Binding("Key")
            'Dim ValueBind As Binding = New Binding("Value")
            'pieSeries.Title = "Test"
            'pieSeries.IndependentValueBinding = KeyBind
            'pieSeries.DependentValueBinding = ValueBind

            ''Dim itemsSource As  KeyValuePair(Of Object, Double)(){New KeyValuePair(Of DateTime, Integer)(DateTime.Now, 100), New KeyValuePair(Of DateTime, Integer)(DateTime.Now.AddMonths(1), 130), New KeyValuePair(Of DateTime, Integer)(DateTime.Now.AddMonths(2), 150), New KeyValuePair(Of DateTime, Integer)(DateTime.Now.AddMonths(3), 125)}
            '    Dim MyData As New List(Of KeyValuePair(Of DateTime, Double))

            '    MyData.Add(New KeyValuePair(Of DateTime, Double)(DateTime.Now, 100))
            '    MyData.Add(New KeyValuePair(Of DateTime, Double)(DateTime.Now.AddDays(1), 200))

            '    pieSeries.DependentValuePath = "Value"
            '    pieSeries.IndependentValuePath = "Key"
            '    pieSeries.ItemsSource = MyData
            '    pieSeries.Title = "Demo data series"
            '    Chart1.Series.Clear()
            '    Chart1.Series.Add(pieSeries)

        Catch ex As Exception
            MessageBox.Show("Erreur lors de la création de la fenêtre des relevés: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me)
    End Sub
End Class
