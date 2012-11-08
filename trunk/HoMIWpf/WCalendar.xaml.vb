Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Windows
Imports System.Linq
Imports Jarloo.Calendar

Public Class WCalendar

    Public Sub New()

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        Dim months As New List(Of String)() From { _
 "Janvier", _
 "Fevrier", _
 "Mars", _
 "Avril", _
 "Mai", _
 "Juin", _
 "Juillet", _
 "Aout", _
 "Septembre", _
 "Octobre", _
 "Novembre", _
 "Decembre" _
}
        cboMonth.ItemsSource = months

        For i As Integer = -50 To 49
            cboYear.Items.Add(DateTime.Today.AddYears(i).Year)
        Next

        cboMonth.SelectedIndex = DateTime.Today.Month - 1
        cboYear.SelectedItem = DateTime.Today.Year

        Calendar.DayNames.Item(0) = "Dim"
        Calendar.DayNames.Item(1) = "Lun"
        Calendar.DayNames.Item(2) = "Mar"
        Calendar.DayNames.Item(3) = "Mer"
        Calendar.DayNames.Item(4) = "Jeu"
        Calendar.DayNames.Item(5) = "Ven"
        Calendar.DayNames.Item(6) = "Sam"
        'For i As Integer = 0 To Calendar.DayNames.Count - 1
        '    MsgBox(Calendar.DayNames.Item(i))
        'Next
    End Sub

    Private Sub RefreshCalendar()
        If cboYear.SelectedItem Is Nothing Then
            Return
        End If
        If cboMonth.SelectedItem Is Nothing Then
            Return
        End If

        Dim year As Integer = CInt(cboYear.SelectedItem)

        Dim month As Integer = cboMonth.SelectedIndex + 1

        Dim targetDate As New DateTime(year, month, 1)

        Calendar.BuildCalendar(targetDate)
    End Sub

    Private Sub Calendar_DayChanged(ByVal sender As Object, ByVal e As DayChangedEventArgs)
        'save the text edits to persistant storage
    End Sub

    Private Sub cboMonth_SelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cboMonth.SelectionChanged
        RefreshCalendar()
    End Sub

    Private Sub cboYear_SelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cboYear.SelectionChanged
        RefreshCalendar()
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub
End Class
