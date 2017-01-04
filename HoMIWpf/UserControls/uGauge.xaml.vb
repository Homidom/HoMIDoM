Imports System.Collections.ObjectModel
Imports De.TorstenMandelkow.MetroChart
Imports System.ComponentModel

Public Class uGauge
    Dim _ChartTitleVisibility As Boolean = False
    Dim _ChartTitle As String = String.Empty
    Dim _ChartSubTitle As String = String.Empty
    Dim chartserie As ChartSeries = New ChartSeries
    Public Errors As New ObservableCollection(Of ChartSeries)
    Dim _Value As Integer = 0
    Dim blocks As New ObservableCollection(Of Testclass)
    Dim WithEvents cl As New Testclass("", 0)

    Public Property ChartTitleVisibility As Boolean
        Get
            Return _ChartTitleVisibility
        End Get
        Set(value3 As Boolean)
            If value3 = True Then
                barChart3.ChartTitleVisibility = Windows.Visibility.Visible
            Else
                barChart3.ChartTitleVisibility = Windows.Visibility.Collapsed
            End If
        End Set
    End Property

    Public Property ChartTitle As String
        Get
            Return _ChartTitle
        End Get
        Set(value2 As String)
            _ChartTitle = value2
            barChart3.ChartTitle = _ChartTitle
        End Set
    End Property

    Public Property ChartSubTitle As String
        Get
            Return _ChartSubTitle
        End Get
        Set(value2 As String)
            _ChartSubTitle = value2
            barChart3.ChartSubTitle = _ChartSubTitle
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

    Public Sub New(Title As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        blocks.Add(cl)
        chartserie.SeriesTitle = "Device"
        chartserie.DisplayMember = "Category"
        chartserie.ValueMember = "Number"
        chartserie.ItemsSource = blocks
        Errors.Add(chartserie)
        barChart3.Series = Errors
    End Sub
End Class

Public Class Testclass
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
