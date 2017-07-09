Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports LiveCharts
Imports LiveCharts.Wpf

Public Class uChart
    Dim _ChartTitleVisibility As Boolean = False
    Dim _ChartTitle As String = String.Empty
    Dim _ChartSubTitle As String = String.Empty
    'Dim chartserie As ChartSeries = New ChartSeries
    'Public Series As New ObservableCollection(Of ChartSeries)
    Public SeriesLine As New SeriesCollection()
    Public SeriesColumn As New SeriesCollection()
    Public SeriesBar As New SeriesCollection()
    Public AxisX As Axis = New Axis
    Dim _Value As Integer = 0
    Dim blocks As New ObservableCollection(Of Dataclass)
    'Dim WithEvents cl As New Testclass("", 0)
    Dim ChartColonHoriz As LiveCharts.Wpf.CartesianChart 'De.TorstenMandelkow.MetroChart.ClusteredBarChart = Nothing
    Dim ChartColonVert As LiveCharts.Wpf.CartesianChart 'De.TorstenMandelkow.MetroChart.ClusteredColumnChart = Nothing
    Dim ChartLine As LiveCharts.Wpf.CartesianChart = Nothing
    Dim _TypeChart As TypeCharts = TypeCharts.Column
    Dim _CurrentChart As Object = Nothing
    Dim _ID As String = String.Empty
    Dim _Periode As Periodes = Periodes.Jour

    ''' <summary>
    ''' Periode à afficher, à la journée, au mois à l'année
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum Periodes
        Jour = 0
        Mois = 1
        Annee = 2
    End Enum

    ''' <summary>
    ''' Types de chart (line, column, bar..)
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum TypeCharts
        Column = 0
        Bar = 1
        Line = 2
    End Enum

    Public Property Periode As Periodes
        Get
            Return _Periode
        End Get
        Set(value As Periodes)
            If _Periode <> value Then
                _Periode = value
                GetData()
                Redraw()
                RaiseEvent PeriodeChange(_Periode)
            End If
        End Set
    End Property

    Public Event PeriodeChange(e As Periodes)

    Public Property IDDevice As String
        Get
            Return _ID
        End Get
        Set(value As String)
            If _ID <> value Then
                GetData()
                Redraw()
            End If
            _ID = value
        End Set
    End Property

    Public Property TypeChart As TypeCharts
        Get
            Return _TypeChart
        End Get
        Set(value As TypeCharts)
            If _TypeChart <> value Then
                _TypeChart = value
                GetData()
                Redraw()
                RaiseEvent TypeChartChange(_TypeChart)
            End If
        End Set
    End Property

    Public Event TypeChartChange(e As TypeCharts)

    Public Property ChartTitleVisibility As Boolean
        Get
            Return _ChartTitleVisibility
        End Get
        Set(value3 As Boolean)
            _ChartTitleVisibility = value3
        End Set
    End Property

    Public Property ChartTitle As String
        Get
            Return _ChartTitle
        End Get
        Set(value2 As String)
            _ChartTitle = value2
        End Set
    End Property

    Public Property ChartSubTitle As String
        Get
            Return _ChartSubTitle
        End Get
        Set(value2 As String)
            _ChartSubTitle = value2
        End Set
    End Property

    Public Property Value As Integer
        Get
            Return _Value
        End Get
        Set(value2 As Integer)
            _Value = value2
            blocks.Item(0).Number = value2
        End Set
    End Property

    Public Sub New(ID As String)

        ' This call is required by the designer.
        InitializeComponent()


        ' Add any initialization after the InitializeComponent() call.
        IDDevice = ID

        'chartserie.SeriesTitle = " "
        'chartserie.DisplayMember = "Category"
        'chartserie.ValueMember = "Number"

    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    ''' <summary>
    ''' Recharge le chart
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Redraw()
        Try
            StkChart.Children.Clear()
            Me.UpdateLayout()


            Select Case _TypeChart
                Case TypeCharts.Bar
                    ChartColonHoriz = New LiveCharts.Wpf.CartesianChart
                    AxisX.Title = "Mois"
                    With ChartColonHoriz
                        .AxisY.Add(AxisX)
                        .Series = SeriesBar
                    End With
                    StkChart.Children.Add(ChartColonHoriz)
                Case TypeCharts.Column
                    ChartColonVert = New LiveCharts.Wpf.CartesianChart
                    AxisX.Title = "Mois"
                    With ChartColonVert
                        .AxisX.Add(AxisX)
                        .Series = SeriesColumn
                    End With
                    StkChart.Children.Add(ChartColonVert)
                Case TypeCharts.Line
                    ChartLine = New LiveCharts.Wpf.CartesianChart
                    AxisX.Title = "Mois"
                    With ChartLine
                        .AxisX.Add(AxisX)
                        .Series = SeriesLine
                    End With
                    Select Case Periode
                        Case Periodes.Annee
                            AxisX.Separator.Step = 24
                        Case Periodes.Mois
                            'AxisX.Separator.Step = 12
                    End Select

                    StkChart.Children.Add(ChartLine)
                    DataContext = Me
            End Select


        Catch ex As Exception
            MessageBox.Show("Erreur Redraw:" & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Recupère les données
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub GetData()

        Dim datestart As String = String.Empty
        Dim dateend As String = String.Empty
        Dim myData As New List(Of HoMIDom.HoMIDom.Historisation)
 
        Try

            SeriesLine = New SeriesCollection
            SeriesColumn = New SeriesCollection
            SeriesBar = New SeriesCollection
            AxisX = New Axis

           Select Case Periode
                Case Periodes.Jour
                    datestart = Format(Now.Date, "yyyy-MM-dd") ' & " 0:0:0"
                    dateend = Format(Now.Date.AddDays(1), "yyyy-MM-dd") ' & " 23:59:59"
                    myData = myService.GetHistoDeviceSource(IdSrv, _ID, "Value", datestart, dateend, ComboBoxMoyenne.Text) '"HEURE")
                Case Periodes.Mois
                    datestart = Format(Now.Date.AddDays(-31), "yyyy-MM-dd") ' & " 0:0:0"
                    dateend = Format(Now.Date, "yyyy-MM-dd") ' & " 23:59:59"
                    myData = myService.GetHistoDeviceSource(IdSrv, _ID, "Value", datestart, dateend, ComboBoxMoyenne.Text) '"JOUR")
                Case Periodes.Annee
                    datestart = (Now.Year - 1).ToString & "-" & Now.Month & "-" & Now.Day ' & " 0:0:0" 'Format(Now.Date.AddYears(-1), "yyyy/MM/dd") & " 0:0:0"
                    dateend = Format(Now.Date, "yyyy-MM-dd") ' & " 23:59:59"
                    myData = myService.GetHistoDeviceSource(IdSrv, _ID, "Value", datestart, dateend, "JOUR")
            End Select
            ''Dim datas As System.Data.DataTable = myService.RequeteSqLHisto(IdSrv, "Select * from historiques where device_id='" & _ID & "'")
            ''If datas IsNot Nothing Then
            ''    MessageBox.Show(datas.Rows.Count)
            ''End If 

            Dim Data3 As New LineSeries
            Data3.Values = New ChartValues(Of Double)
            Data3.Title = ""
            Dim Data4 As New StackedColumnSeries
            Data4.Values = New ChartValues(Of Double)
            Data4.Title = ""
            Dim Data5 As New RowSeries
            Data5.Values = New ChartValues(Of Double)
            Data5.Title = ""

            Dim Labels As New List(Of String)

            For Each Data As HoMIDom.HoMIDom.Historisation In myData
                Data3.Values.Add(CDbl(Data.Value))
                Data4.Values.Add(CDbl(Data.Value))
                Data5.Values.Add(CDbl(Data.Value))
                Labels.Add(Data.DateTime)
            Next

            SeriesLine.Add(Data3)
            SeriesColumn.Add(Data4)
            SeriesBar.Add(Data5)
            AxisX.Labels = Labels
            'chartserie.ItemsSource = blocks
            'Series.Add(chartserie)

        Catch ex As Exception
            MessageBox.Show("Erreur GetData:" & ex.ToString, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try

        myData = Nothing
    End Sub

    Private Sub uChart_LayoutUpdated(sender As Object, e As EventArgs) Handles Me.LayoutUpdated
        If ChartLine IsNot Nothing Then ChartLine.Height = Me.Height - 70
        If ChartColonHoriz IsNot Nothing Then ChartColonHoriz.Height = Me.Height - 70
        If ChartColonVert IsNot Nothing Then ChartColonVert.Height = Me.Height - 70
    End Sub

    Private Sub uChart_SizeChanged(sender As Object, e As SizeChangedEventArgs) Handles Me.SizeChanged
        If ChartLine IsNot Nothing Then ChartLine.Height = Me.Height - 70
        If ChartColonHoriz IsNot Nothing Then ChartColonHoriz.Height = Me.Height - 70
        If ChartColonVert IsNot Nothing Then ChartColonVert.Height = Me.Height - 70
    End Sub

#Region "Boutons"

    ''' <summary>
    ''' Bouton Jour
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BtnJour_Click(sender As Object, e As RoutedEventArgs) Handles BtnJour.Click
        Try
            Periode = Periodes.Jour
            GetData()
            Redraw()
        Catch ex As Exception
            MessageBox.Show("Erreur:" & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Bouton Mois
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BtnMois_Click(sender As Object, e As RoutedEventArgs) Handles BtnMois.Click
        Try
            Periode = Periodes.Mois
            GetData()
            Redraw()

        Catch ex As Exception
            MessageBox.Show("Erreur:" & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Bouton Annee
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BtnAnnee_Click(sender As Object, e As RoutedEventArgs) Handles BtnAnnee.Click
        Try
            Periode = Periodes.Annee
            GetData()
            Redraw()
        Catch ex As Exception
            MessageBox.Show("Erreur:" & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Bouton Graphe Bar
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BtnBar_Click(sender As Object, e As RoutedEventArgs) Handles BtnBar.Click
        Try
            TypeChart = TypeCharts.Bar
        Catch ex As Exception
            MessageBox.Show("Erreur:" & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Bouton Graphe colonne
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BtnColumn_Click(sender As Object, e As RoutedEventArgs) Handles BtnColumn.Click
        Try
            TypeChart = TypeCharts.Column
        Catch ex As Exception
            MessageBox.Show("Erreur:" & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Bouton Graphe Line
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BtnLine_Click(sender As Object, e As RoutedEventArgs) Handles BtnLine.Click
        Try
            TypeChart = TypeCharts.Line
        Catch ex As Exception
            MessageBox.Show("Erreur:" & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
    Private Sub ComboBoxMoyenne_SelectionChanged(sender As Object, e As RoutedEventArgs) Handles ComboBoxMoyenne.SelectionChanged
        Try
            If ComboBoxMoyenne.Text = "Jour" Then
                ComboBoxMoyenne.Text = "Heure"
            Else
                ComboBoxMoyenne.Text = "Jour"
            End If
            GetData()
            Redraw()
        Catch ex As Exception
            MessageBox.Show("Erreur:" & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

#End Region

End Class
Public Class Dataclass
    Implements INotifyPropertyChanged

    Dim _Category As String
    Dim _Number As Integer

    Sub New(category As String, number As Integer)
        Me.Category = category
        Me.Number = number
    End Sub

    Public Property Category As String
        Get
            Return _Category
        End Get
        Set(value As String)
            _Category = value
        End Set
    End Property


    Public Property Number As Integer
        Set(value As Integer)
            _Number = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Number"))
        End Set
        Get
            Return _Number
        End Get
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
End Class
