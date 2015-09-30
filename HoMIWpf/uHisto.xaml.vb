'Imports System.Windows.Controls.DataVisualization.Charting
Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Windows.Forms.DataVisualization.Charting.Utilities
Imports HoMIDom.HoMIDom
Imports System.Drawing
Imports System.ComponentModel


Public Class uHisto
    Public Event CloseMe(ByVal MyObject As Object)
    Dim result As New List(Of Historisation)
    Dim _Devices As New List(Of Dictionary(Of String, String))
    Dim _MaxData As Integer = 1000
    Dim _CurrentChart As Chart = Nothing
    Dim _FirstGener As Boolean = False
    Public _with As Integer = 600

    Public Sub New(ByVal Devices As List(Of Dictionary(Of String, String)))

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        Try
            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().

            DateStartSelect.DisplayDate = Now.Date.AddDays(-31).ToString '"01/01/2012"
            DateStartSelect.Text = Now.Date.AddDays(-7).ToString '"01/01/2012"
            DateFinSelect.DisplayDate = Now.Date.AddDays(1).ToString
            DateFinSelect.Text = Now.Date.AddDays(1).ToString

            AffHisto()
        Catch ex As Exception
            MessageBox.Show("Erreur lors de la création de la fenêtre des relevés: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me.Uid)
    End Sub

    Private Sub Refresh_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Refresh.Click
        Try
            If _FirstGener = False Then Generation()

            _MaxData = TxtMaxData.Text
            If IsNumeric(_MaxData) = False Or _MaxData > 5000 Or _MaxData < 0 Then
                MessageBox.Show("Le nombre maximal de données doit être un numérique et <5000", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                TxtMaxData.Text = 1000
                Exit Sub
            End If

            Update_Graphe()
        Catch ex As Exception
            MessageBox.Show("Erreur uHisto Refresh_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Sub Update_Graphe()
        Try
            Cursor = Cursors.Wait

            Test.Child = Nothing
            Test.UpdateLayout()
            Me.UpdateLayout()

            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()

            Dim Chart2 As New System.Windows.Forms.DataVisualization.Charting.Chart()
            ' Add a chart area.
            Chart2.ChartAreas.Add("Default")
            Chart2.Width = 925
            Chart2.Height = 500

            AddHandler Chart2.MouseMove, AddressOf Chart2_MouseMove

            Dim legend1 As New Legend
            Chart2.Legends.Add(legend1)

            ' Set docking of the legend title
            Chart2.Legends(0).Docking = Docking.Bottom

            Chart2.ChartAreas("Default").AxisX.LabelStyle.Font = New Font("Arial", 8)
            Chart2.ChartAreas("Default").AxisY.LabelStyle.Font = New Font("Arial", 8)

            ' Set automatic zooming
            Chart2.ChartAreas("Default").AxisX.ScaleView.Zoomable = True
            Chart2.ChartAreas("Default").AxisY.ScaleView.Zoomable = True

            'Chart2.ChartAreas("Default").AxisX.MajorGrid.Interval = 1
            'Chart2.ChartAreas("Default").AxisX.MajorGrid.IntervalType = DateTimeIntervalType.Hours
            'Chart2.ChartAreas("Default").AxisX.MajorTickMark.Interval = 1
            'Chart2.ChartAreas("Default").AxisX.MajorTickMark.IntervalType = DateTimeIntervalType.Hours
            'Chart2.ChartAreas("Default").AxisX.Interval = 1
            'Chart2.ChartAreas("Default").AxisX.IntervalType = DateTimeIntervalType.Hours
            'Chart2.ChartAreas("Default").AxisX.LabelStyle.Interval = 1
            'Chart2.ChartAreas("Default").AxisX.LabelStyle.IntervalType = DateTimeIntervalType.Hours
            Chart2.ChartAreas("Default").AxisX.LabelStyle.Angle = -60
            Chart2.ChartAreas("Default").AxisX.LabelStyle.Format = "dd/MM/yyyy HH:mm"
            Chart2.ChartAreas("Default").AxisX.LabelStyle.IntervalType = DateTimeIntervalType.Auto
            Chart2.ChartAreas("Default").AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount
            Chart2.ChartAreas("Default").AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot
            Chart2.ChartAreas("Default").AxisX.MajorGrid.LineColor = Color.SlateGray

            Chart2.ChartAreas("Default").AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount
            Chart2.ChartAreas("Default").AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot
            Chart2.ChartAreas("Default").AxisY.MajorGrid.LineColor = Color.SlateGray

            ' Set automatic scrolling
            Chart2.ChartAreas("Default").CursorX.AutoScroll = True
            Chart2.ChartAreas("Default").CursorY.AutoScroll = True

            Do While TabControl1.Items.Count > 1
                TabControl1.Items.RemoveAt(1)
            Loop

            Dim datestart As String = DateStartSelect.Text
            Dim dateend As String = DateFinSelect.Text 'displaydate
            Dim moyenne As String = ComboBoxMoyenne.Text
            'Dim a() As String = datestart.Split("/")
            Dim a() As String = datestart.Split(System.Globalization.DateTimeFormatInfo.CurrentInfo.DateSeparator)
            If a.Length = 3 Then datestart = a(2) & "-" & a(1) & "-" & a(0) Else datestart = ""
            'a = dateend.Split("/")
            a = dateend.Split(System.Globalization.DateTimeFormatInfo.CurrentInfo.DateSeparator)
            If a.Length = 3 Then dateend = a(2) & "-" & a(1) & "-" & a(0) Else dateend = ""

            For Each _item In _Devices
                For Each kvp As KeyValuePair(Of String, String) In _item
                    Dim _namedevice As String = myService.ReturnDeviceByID(IdSrv, kvp.Key).Name

                    'Recuperation des historiques
                    result = myService.GetHistoDeviceSource(IdSrv, kvp.Key, kvp.Value, datestart, dateend, moyenne)

                    'Construction de la serie dans le graphe
                    Dim series As New System.Windows.Forms.DataVisualization.Charting.Series(_namedevice & ": " & kvp.Value)
                    series.XValueType = ChartValueType.DateTime
                    Dim cnt As Integer = 0
                    For Each data As Historisation In result
                        'series.Points.AddXY(data.DateTime.ToString("G"), data.Value.Replace(",", "."))
                        series.Points.AddXY(data.DateTime, data.Value.Replace(System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator, "."))
                        cnt += 1
                        If cnt > _MaxData Then Exit For
                    Next
                    series.BorderWidth = 3
                    series.IsXValueIndexed = False
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
                    Chart2.Series(i).ToolTip = "#VALX{dd/MM/yyyy HH:mm:ss}" & " Valeur:" & "#VALY"
                Next
            ElseIf ChkLine_Full.IsChecked Then
                For i As Integer = 0 To Chart2.Series.Count - 1
                    Chart2.Series(i).ChartType = SeriesChartType.SplineArea
                    Chart2.Series(i).ToolTip = "#VALX{dd/MM/yyyy HH:mm:ss}" & " Valeur:" & "#VALY"
                Next
            ElseIf ChkHisto.IsChecked Then
                For i As Integer = 0 To Chart2.Series.Count - 1
                    Chart2.Series(i).ChartType = SeriesChartType.Column
                    Chart2.Series(i).ToolTip = "#VALX{dd/MM/yyyy HH:mm:ss}" & " Valeur:" & "#VALY"
                Next
            Else
                For i As Integer = 0 To Chart2.Series.Count - 1
                    Chart2.Series(i).ChartType = SeriesChartType.Pie
                    Chart2.Series(i).ToolTip = "#VALX{dd/MM/yyyy HH:mm:ss}" & " Valeur:" & "#VALY"
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
                Case 5
                    Chart2.ChartAreas("Default").BackColor = Color.LightGray
                    Chart2.ChartAreas("Default").BackGradientStyle = GradientStyle.TopBottom
                    Chart2.ChartAreas("Default").BackSecondaryColor = Color.WhiteSmoke
                Case 6 : Chart2.ChartAreas("Default").BackColor = Color.Transparent
            End Select

            Select Case CbZoom.Text
                Case "2 heures"
                    Chart2.ChartAreas("Default").AxisX.ScaleView.Zoom(0, 1 / 2)
                Case "1 heure"
                    Chart2.ChartAreas("Default").AxisX.ScaleView.Zoom(0, 1 / 3)
                Case "30 minutes"
                    Chart2.ChartAreas("Default").AxisX.ScaleView.Zoom(0, 1 / 5)
                Case "15 minutes"
                    Chart2.ChartAreas("Default").AxisX.ScaleView.Zoom(0, 1 / 9)
            End Select

            ' Add the chart to the Windows Form Host.
            Test.Child = Chart2
            _CurrentChart = Chart2

            result = Nothing
            Me.Cursor = Nothing
            Me.UpdateLayout()
        Catch ex As Exception
            Me.Cursor = Nothing
            MessageBox.Show("Erreur uHisto Update_Graphe: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
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

    'Afficher la liste des historisations
    Public Sub AffHisto()
        Try
            Cursor = Cursors.Wait
            TreeViewHisto.Items.Clear()

            Dim x As New List(Of HoMIDom.HoMIDom.Historisation)
            Dim ListeDevices = AllDevices

            x = myService.GetAllListHisto(IdSrv)

            If ListeDevices IsNot Nothing Then
                For Each _dev As TemplateDevice In ListeDevices
                    Dim IsNode As Boolean = False
                    Dim parent = New TreeViewItem
                    Dim y As New CheckBox
                    Dim trv As Boolean = False

                    If AsMultiHisto(_dev.ID, x) Then 'le device a différents historique
                        parent.Header = _dev.Name
                        parent.Foreground = New SolidColorBrush(Colors.White)
                        IsNode = True
                    Else
                        y.Content = _dev.Name
                        y.Foreground = New SolidColorBrush(Colors.Black)
                        y.Background = New SolidColorBrush(Colors.DarkGray)
                        y.BorderBrush = New SolidColorBrush(Colors.Black)
                        y.Margin = New Thickness(-15, 1, 0, 0)
                        y.IsEnabled = False
                        y.Uid = _dev.ID
                    End If

                    If x IsNot Nothing Then
                        For i As Integer = 0 To x.Count - 1

                            Dim a As Historisation = x.Item(i)

                            If _dev.ID = a.IdDevice And IsNode = False Then
                                y.Tag = a.Nom
                                y.Foreground = New SolidColorBrush(Colors.White)
                                y.IsEnabled = True

                                TreeViewHisto.Items.Add(y)
                                trv = True
                            ElseIf _dev.ID = a.IdDevice And IsNode = True Then
                                Dim y1 As New CheckBox
                                y1.Content = _dev.Name
                                y1.Foreground = New SolidColorBrush(Colors.White)
                                y1.Background = New SolidColorBrush(Colors.DarkGray)
                                y1.BorderBrush = New SolidColorBrush(Colors.Black)
                                y1.Margin = New Thickness(-15, 1, 0, 0)
                                y1.Uid = _dev.ID
                                y1.Content = a.Nom
                                y1.Tag = a.Nom

                                parent.Items.Add(y1)
                                trv = True
                            End If
                        Next
                        If IsNode Then
                            TreeViewHisto.Items.Add(parent)
                        End If
                    End If
                    If trv = False Then TreeViewHisto.Items.Add(y)
                Next
            End If

            TreeViewHisto.Items.SortDescriptions.Clear()
            TreeViewHisto.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
            Me.Cursor = Nothing
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub AffHisto: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Retourne True si le device a plusieurs histo sur des propriétés différents
    Private Function AsMultiHisto(ByVal Deviceid As String, ByVal x As List(Of HoMIDom.HoMIDom.Historisation)) As Boolean
        Dim retour As Boolean = False
        Dim tmp As Boolean = False
        Dim _name As String = ""

        For Each _histo As HoMIDom.HoMIDom.Historisation In x
            If _histo.IdDevice = Deviceid And _name = "" Then
                _name = _histo.Nom
            ElseIf _histo.IdDevice = Deviceid And _name <> _histo.Nom Then
                retour = True
            End If
        Next

        Return retour
    End Function

    Private Sub BtnGenereGraph_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnGenereGraph.Click
        Generation()
    End Sub

    Private Sub Generation()
        Try
            If IsConnect = False Then
                Exit Sub
            End If

            Me.Cursor = Cursors.Wait

            Dim Devices As New List(Of Dictionary(Of String, String))


            For i As Integer = 0 To TreeViewHisto.Items.Count - 1
                Dim chk As CheckBox

                If TreeViewHisto.Items(i).GetType.ToString.Contains("CheckBox") Then
                    chk = TreeViewHisto.Items(i)

                    If chk.IsChecked = True Then
                        Dim y As New Dictionary(Of String, String)
                        y.Add(chk.Uid, chk.Tag)
                        Devices.Add(y)
                    End If

                Else 'il a des enfants
                    Dim trv1 As TreeViewItem = TreeViewHisto.Items(i)
                    For j1 As Integer = 0 To trv1.Items.Count - 1
                        chk = trv1.Items(j1)

                        If chk.IsChecked = True Then
                            Dim y As New Dictionary(Of String, String)
                            y.Add(chk.Uid, chk.Tag)
                            Devices.Add(y)
                        End If

                    Next
                End If
            Next
            _Devices = Devices
            Update_Graphe()
            Me.Cursor = Nothing

            _FirstGener = True
        Catch ex As Exception
            Me.Cursor = Nothing
            MessageBox.Show("ERREUR Sub Generation: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub Chart2_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Try
            ' Call HitTest
            Dim result As HitTestResult = _CurrentChart.HitTest(e.X, e.Y)

            ' Reset Data Point Attributes
            For Each point As DataPoint In _CurrentChart.Series(0).Points
                point.BorderWidth = 3
            Next

            ' If the mouse if over a data point
            'If result.ChartElementType = ChartElementType.DataPoint Then
            '    If result.PointIndex >= 0 Then
            '        If _CurrentChart.Series(0).Points(result.PointIndex) IsNot Nothing Then
            '            Dim pointt As DataPoint = _CurrentChart.Series(0).Points(result.PointIndex)
            '            pointt.BorderWidth = 1
            '        End If
            '    End If
            'Else
            '    ' Set default cursor
            '    Me.Cursor = Cursors.Arrow
            'End If
        Catch ex As Exception
            MessageBox.Show("Erreur uHisto Chart2_MouseMove: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

End Class
