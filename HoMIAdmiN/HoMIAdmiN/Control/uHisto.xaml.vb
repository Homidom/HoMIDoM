'Imports System.Windows.Controls.DataVisualization.Charting
Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Windows.Forms.DataVisualization.Charting.Utilities
Imports System.Data
Imports HoMIDom.HoMIDom
Imports System.Drawing

Public Class uHisto
    Public Event CloseMe(ByVal MyObject As Object)
    Dim result As New List(Of Historisation)
    Dim _Devices As New List(Of Dictionary(Of String, String))
    Dim _MaxData As Integer = 1000
    Dim _CurrentChart As Chart = Nothing

    Public Sub New(ByVal Devices As List(Of Dictionary(Of String, String)))

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        Try
            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().

            DateStartSelect.DisplayDate = Now.Date.AddDays(-31).ToString '"01/01/2012"
            DateStartSelect.Text = Now.Date.AddDays(-7).ToString '"01/01/2012"
            DateFinSelect.DisplayDate = Now.Date.AddDays(1).ToString
            DateFinSelect.Text = Now.Date.AddDays(1).ToString

            _Devices = Devices

            Update_Graphe()

        Catch ex As Exception
            MessageBox.Show("Erreur lors de la création de la fenêtre des relevés: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me)
    End Sub

    Private Sub Refresh_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Refresh.Click
        Try
            _MaxData = TxtMaxData.Text
            If IsNumeric(_MaxData) = False Or _MaxData > 5000 Or _MaxData < 0 Then
                MessageBox.Show("Le nombre maximal de données doit être un numérique et <5000", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                TxtMaxData.Text = 1000
                Exit Sub
            End If

            Update_Graphe()
        Catch ex As Exception
            MessageBox.Show("Erreur uHisto Refresh_Click: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Sub Update_Graphe()
        Try
            Cursor = Cursors.Wait

            Dim Chart2 As New System.Windows.Forms.DataVisualization.Charting.Chart()
            ' Add a chart area.
            Chart2.ChartAreas.Add("Default")
            Chart2.Width = 925
            Chart2.Height = 500


            Dim legend1 As New Legend
            Chart2.Legends.Add(legend1)

            ' Set docking of the legend title
            Chart2.Legends(0).Docking = Docking.Bottom

            Chart2.ChartAreas("Default").AxisX.LabelStyle.Font = New Font("Arial", 8)
            Chart2.ChartAreas("Default").AxisY.LabelStyle.Font = New Font("Arial", 8)

            ' Set automatic zooming
            Chart2.ChartAreas("Default").AxisX.ScaleView.Zoomable = True
            Chart2.ChartAreas("Default").AxisY.ScaleView.Zoomable = True

            ' Set automatic scrolling
            Chart2.ChartAreas("Default").CursorX.AutoScroll = True
            Chart2.ChartAreas("Default").CursorY.AutoScroll = True

            Do While TabControl1.Items.Count > 1
                TabControl1.Items.RemoveAt(1)
            Loop

            For Each _item In _Devices
                For Each kvp As KeyValuePair(Of String, String) In _item
                    Dim _namedevice As String = myService.ReturnDeviceByID(IdSrv, kvp.Key).Name
                    Dim datestart As String = DateStartSelect.Text
                    Dim dateend As String = DateFinSelect.Text 'displaydate
                    Dim moyenne As String = ComboBoxMoyenne.Text

                    'Recuperation des historiques
                    Dim a() As String = datestart.Split("/")
                    If a.Length = 3 Then datestart = a(2) & "-" & a(1) & "-" & a(0) Else datestart = ""
                    a = dateend.Split("/")
                    If a.Length = 3 Then dateend = a(2) & "-" & a(1) & "-" & a(0) Else dateend = ""
                    result = myService.GetHistoDeviceSource(IdSrv, kvp.Key, kvp.Value, datestart, dateend, moyenne)

                    'Construction de la serie dans le graphe
                    Dim series As New System.Windows.Forms.DataVisualization.Charting.Series(_namedevice & ": " & kvp.Value)
                    Dim cnt As Integer = 0
                    For Each data As Historisation In result
                        series.Points.AddXY(data.DateTime.ToString("G"), data.Value.Replace(",", "."))
                        cnt += 1
                        If cnt > _MaxData Then Exit For
                    Next
                    series.BorderWidth = 3
                    Chart2.Series.Add(series)

                    'Tableau de valeur
                    Dim _Tabitem As New TabItem
                    If kvp.Value = "Value" Then _Tabitem.Header = _namedevice Else _Tabitem.Header = _namedevice & ":" & kvp.Value
                    Dim _Releve As New uReleve(result, _namedevice & ": " & kvp.Value)
                    _Tabitem.Content = _Releve
                    TabControl1.Items.Add(_Tabitem)
                Next kvp
            Next

            If ChkLine.IsChecked Then
                For i As Integer = 0 To Chart2.Series.Count - 1
                    Chart2.Series(i).ChartType = SeriesChartType.Line
                    Chart2.Series(i).ToolTip = "#VALX" & " Valeur:" & "#VALY"
                Next
            ElseIf ChkLine_Full.IsChecked Then
                For i As Integer = 0 To Chart2.Series.Count - 1
                    Chart2.Series(i).ChartType = SeriesChartType.SplineArea
                    Chart2.Series(i).ToolTip = "#VALX" & " Valeur:" & "#VALY"
                Next
            ElseIf ChkHisto.IsChecked Then
                For i As Integer = 0 To Chart2.Series.Count - 1
                    Chart2.Series(i).ChartType = SeriesChartType.Bar
                    Chart2.Series(i).ToolTip = "#VALX" & " Valeur:" & "#VALY"
                Next
            Else
                For i As Integer = 0 To Chart2.Series.Count - 1
                    Chart2.Series(i).ChartType = SeriesChartType.Pie
                    Chart2.Series(i).ToolTip = "#VALX" & " Valeur:" & "#VALY"
                Next
            End If
            
            ' Zoom into the X axis
            'Chart2.ChartAreas("Default").AxisX.ScaleView.Zoom(2, 3)

            ' Enable range selection and zooming end user interface
            Chart2.ChartAreas("Default").CursorX.IsUserEnabled = True
            Chart2.ChartAreas("Default").CursorX.IsUserSelectionEnabled = True
            Chart2.ChartAreas("Default").AxisX.ScaleView.Zoomable = True
            Chart2.ChartAreas("Default").AxisX.ScrollBar.IsPositionedInside = True

            Select Case CbBackColor.SelectedIndex
                Case 0 : Chart2.ChartAreas("Default").BackColor = Color.White
                Case 1 : Chart2.ChartAreas("Default").BackColor = Color.LightBlue
                Case 2 : Chart2.ChartAreas("Default").BackColor = Color.LightYellow
                Case 3 : Chart2.ChartAreas("Default").BackColor = Color.Red
                Case 4 : Chart2.ChartAreas("Default").BackColor = Color.LightGreen
                Case 5 : Chart2.ChartAreas("Default").BackColor = Color.LightGray
                Case 6 : Chart2.ChartAreas("Default").BackColor = Color.Transparent
            End Select

            ' Add the chart to the Windows Form Host.
            Test.Child = Chart2
            _CurrentChart = Chart2

            Me.Cursor = Nothing
        Catch ex As Exception
            Me.Cursor = Nothing
            MessageBox.Show("Erreur uHisto Update_Graphe: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnSave_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnSave.Click
        If _CurrentChart IsNot Nothing Then

            ' Create a new save file dialog
            Dim saveFileDialog1 As New Microsoft.Win32.SaveFileDialog()

            ' Sets the current file name filter string, which determines 
            ' the choices that appear in the "Save as file type" or 
            ' "Files of type" box in the dialog box.
            saveFileDialog1.Filter = "Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|EMF (*.emf)|*.emf|PNG (*.png)|*.png|SVG (*.svg)|*.svg|GIF (*.gif)|*.gif|TIFF (*.tif)|*.tif"
            saveFileDialog1.FilterIndex = 2
            saveFileDialog1.RestoreDirectory = True

            ' Set image file format
            If saveFileDialog1.ShowDialog() = Forms.DialogResult.OK Then
                Dim format As ChartImageFormat = ChartImageFormat.Bmp

                If saveFileDialog1.FileName.EndsWith("bmp") Then
                    format = ChartImageFormat.Bmp
                Else
                    If saveFileDialog1.FileName.EndsWith("jpg") Then
                        format = ChartImageFormat.Jpeg
                    Else
                        If saveFileDialog1.FileName.EndsWith("emf") Then
                            format = ChartImageFormat.Emf
                        Else
                            If saveFileDialog1.FileName.EndsWith("gif") Then
                                format = ChartImageFormat.Gif
                            Else
                                If saveFileDialog1.FileName.EndsWith("png") Then
                                    format = ChartImageFormat.Png
                                Else
                                    If saveFileDialog1.FileName.EndsWith("tif") Then
                                        format = ChartImageFormat.Tiff
                                    End If
                                End If ' Save image
                            End If
                        End If
                    End If
                End If
                _CurrentChart.SaveImage(saveFileDialog1.FileName, format)
            End If
        Else
            MessageBox.Show("No curent")
        End If
    End Sub

    Private Sub CbBackColor_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles CbBackColor.SelectionChanged
        If _CurrentChart IsNot Nothing Then
            Select Case CbBackColor.SelectedIndex
                Case 0
                    _CurrentChart.ChartAreas("Default").BackColor = Color.White
                Case 1
                    _CurrentChart.ChartAreas("Default").BackColor = Color.LightBlue
                Case 2
                    _CurrentChart.ChartAreas("Default").BackColor = Color.LightYellow
                Case 3
                    _CurrentChart.ChartAreas("Default").BackColor = Color.Red
                Case 4
                    _CurrentChart.ChartAreas("Default").BackColor = Color.LightGreen
                Case 5
                    _CurrentChart.ChartAreas("Default").BackColor = Color.LightGray
                Case 6
                    _CurrentChart.ChartAreas("Default").BackColor = Color.Transparent
            End Select

        End If
    End Sub


End Class
