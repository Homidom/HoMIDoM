Imports System.Windows.Controls.DataVisualization.Charting
Imports System.Data
Imports HoMIDom.HoMIDom

Public Class uHisto
    Public Event CloseMe(ByVal MyObject As Object)

    Public Sub New(ByVal Devices As Dictionary(Of String, String))

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        Try

            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            Dim result As New List(Of Historisation)

            Chart1.Series.Clear()

            For Each kvp As KeyValuePair(Of String, String) In Devices
                Dim LineSeries As New LineSeries
                Dim MyData As New List(Of KeyValuePair(Of DateTime, Double))
                Dim _namedevice As String = myService.ReturnDeviceByID(IdSrv, kvp.Key).Name

                result = myService.GetHistoDeviceSource(IdSrv, kvp.Key, kvp.Value)

                For Each data As Historisation In result
                    MyData.Add(New KeyValuePair(Of DateTime, Double)(data.DateTime, data.Value))
                Next

                LineSeries.ItemsSource = MyData
                LineSeries.Title = _namedevice & ": " & kvp.Value
                LineSeries.IndependentValueBinding = New Binding("Key")
                LineSeries.DependentValueBinding = New Binding("Value")

                Chart1.Series.Add(LineSeries)

                Dim _Tabitem As New TabItem
                _Tabitem.Header = _namedevice & ":" & kvp.Value

                Dim _Releve As New uReleve(result, _namedevice & ": " & kvp.Value)
                _Tabitem.Content = _Releve

                TabControl1.Items.Add(_Tabitem)

            Next kvp
            Me.Cursor = Nothing

        Catch ex As Exception
            MessageBox.Show("Erreur lors de la création de la fenêtre des relevés: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me)
    End Sub
End Class
