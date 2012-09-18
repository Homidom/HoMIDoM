Imports System.Windows.Controls.DataVisualization.Charting
Imports System.Data
Imports HoMIDom.HoMIDom

Public Class uHisto
    Public Event CloseMe(ByVal MyObject As Object)
    Dim result As New List(Of Historisation)
    Dim _Devices As New List(Of Dictionary(Of String, String))

    Public Sub New(ByVal Devices As List(Of Dictionary(Of String, String)))

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        Try
            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().


            DateStartSelect.DisplayDate = "01/01/1970"
            DateFinSelect.DisplayDate = Now.Date.ToString

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
            Update_Graphe()
        Catch ex As Exception
            MessageBox.Show("Erreur uHisto Refresh_Click: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Sub Update_Graphe()
        Try
            Chart1.Series.Clear()

            Do While TabControl1.Items.Count > 1
                TabControl1.Items.RemoveAt(1)
            Loop

            For Each _item In _Devices
                For Each kvp As KeyValuePair(Of String, String) In _item
                    Dim _Series As New Object
                    Dim LineSeries As New LineSeries
                    Dim HistoSeries As New BarSeries
                    Dim PieSeries As New PieSeries

                    Dim MyData As New List(Of KeyValuePair(Of DateTime, Double))

                    Dim _namedevice As String = myService.ReturnDeviceByID(IdSrv, kvp.Key).Name
                    Dim datestart As String = DateStartSelect.DisplayDate
                    Dim dateend As String = DateFinSelect.DisplayDate

                    If ChkLine.IsChecked Then
                        _Series = LineSeries
                    ElseIf ChkHisto.IsChecked Then
                        _Series = HistoSeries
                    Else
                        _Series = PieSeries
                    End If

                    Dim a() As String = datestart.Split("/")
                    If a.Length = 3 Then
                        datestart = a(2) & "-" & a(1) & "-" & a(0)
                    Else
                        datestart = ""
                    End If
                    a = dateend.Split("/")
                    If a.Length = 3 Then
                        dateend = a(2) & "-" & a(1) & "-" & a(0)
                    Else
                        dateend = ""
                    End If

                    result = myService.GetHistoDeviceSource(IdSrv, kvp.Key, kvp.Value, datestart, dateend)

                    For Each data As Historisation In result
                        MyData.Add(New KeyValuePair(Of DateTime, Double)(data.DateTime.ToString, data.Value))
                    Next

                    _Series.ItemsSource = MyData
                    _Series.Title = _namedevice & ": " & kvp.Value

                    _Series.IndependentValueBinding = New Binding("Key")
                    _Series.DependentValueBinding = New Binding("Value")
                  

                    Chart1.Series.Add(_Series)

                    Dim _Tabitem As New TabItem
                    _Tabitem.Header = _namedevice & ":" & kvp.Value

                    Dim _Releve As New uReleve(result, _namedevice & ": " & kvp.Value)
                    _Tabitem.Content = _Releve

                    TabControl1.Items.Add(_Tabitem)

                Next kvp
            Next
            
            Me.Cursor = Nothing

        Catch ex As Exception
            MessageBox.Show("Erreur uHisto Update_Graphe: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class
