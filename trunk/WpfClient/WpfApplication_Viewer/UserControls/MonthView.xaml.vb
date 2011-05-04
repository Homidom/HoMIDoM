Imports System.Globalization
Imports System.Math
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Input
Imports System.Collections.Generic

Partial Public Class MonthView

    Friend _DisplayStartDate As Date = Date.Now.AddDays(-1 * (Date.Now.Day - 1))
    Private _DisplayMonth As Integer = _DisplayStartDate.Month
    Private _DisplayYear As Integer = _DisplayStartDate.Year
    Private _cultureInfo As New CultureInfo(CultureInfo.CurrentUICulture.LCID)
    Private sysCal As System.Globalization.Calendar = _cultureInfo.Calendar()
    Private _monthAppointments As List(Of Appointment)

    Public Event DisplayMonthChanged(ByVal e As MonthChangedEventArgs)
    Public Event DayBoxDoubleClicked(ByVal e As NewAppointmentEventArgs)
    Public Event AppointmentDblClicked(ByVal Appointment_Id As Integer)

    Public Property DisplayStartDate() As Date
        Get
            Return _DisplayStartDate
        End Get
        Set(ByVal value As Date)
            _DisplayStartDate = value
            _DisplayMonth = _DisplayStartDate.Month
            _DisplayYear = _DisplayStartDate.Year
        End Set
    End Property

    Friend Property MonthAppointments() As List(Of Appointment)
        Get
            Return _monthAppointments
        End Get
        Set(ByVal value As List(Of Appointment))
            _monthAppointments = value
            Call BuildCalendarUI()
        End Set
    End Property

    Private Sub MonthView_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles MyBase.Loaded
        '-- Want to have the calendar show up, even if no appoints are assigned 
        '   Note - in my own app, appointments are loaded by a backgroundWorker thread to avoid a laggy UI
        If _monthAppointments Is Nothing Then Call BuildCalendarUI()
    End Sub

    Private Sub BuildCalendarUI()
        Dim iDaysInMonth As Integer = sysCal.GetDaysInMonth(_DisplayStartDate.Year, _DisplayStartDate.Month)
        Dim iOffsetDays As Integer = CInt(System.Enum.ToObject(GetType(System.DayOfWeek), _DisplayStartDate.DayOfWeek))
        Dim iWeekCount As Integer = 0
        Dim weekRowCtrl As New WeekOfDaysControls()

        MonthViewGrid.Children.Clear()
        Call AddRowsToMonthGrid(iDaysInMonth, iOffsetDays)
        MonthYearLabel.Content = Microsoft.VisualBasic.MonthName(_DisplayMonth) & " " & _DisplayYear

        For i As Integer = 1 To iDaysInMonth
            If (i <> 1) AndAlso System.Math.IEEERemainder((i + iOffsetDays - 1), 7) = 0 Then
                '-- add existing weekrowcontrol to the monthgrid
                Grid.SetRow(weekRowCtrl, iWeekCount)
                MonthViewGrid.Children.Add(weekRowCtrl)
                '-- make a new weekrowcontrol
                weekRowCtrl = New WeekOfDaysControls()
                iWeekCount += 1
            End If

            '-- load each weekrow with a DayBoxControl whose label is set to day number
            Dim dayBox As New DayBoxControl()
            dayBox.DayNumberLabel.Content = i.ToString
            dayBox.Tag = i
            AddHandler dayBox.MouseDoubleClick, AddressOf DayBox_DoubleClick

            '-- customize daybox for today:
            If (New Date(_DisplayYear, _DisplayMonth, i)) = Date.Today Then
                dayBox.DayLabelRowBorder.Background = CType(dayBox.TryFindResource("OrangeGradientBrush"), Brush)
                dayBox.DayAppointmentsStack.Background = Brushes.Wheat
            End If

            '-- for design mode, add appointments to random days for show...
            If System.ComponentModel.DesignerProperties.GetIsInDesignMode(Me) Then
                If Microsoft.VisualBasic.Rnd(1) < 0.25 Then
                    Dim apt As New DayBoxAppointmentControl()
                    apt.DisplayText.Text = "Apt on " & i & "th"
                    dayBox.DayAppointmentsStack.Children.Add(apt)
                End If

            ElseIf _monthAppointments IsNot Nothing Then
                '-- Compiler warning about unpredictable results if using i (the iterator) in lambda, the 
                '   "hint" suggests declaring another var and set equal to iterator var
                Dim iday As Integer = i
                Dim aptInDay As List(Of Appointment) = _
                    _monthAppointments.FindAll(New System.Predicate(Of Appointment)( _
                                               Function(apt As Appointment) CDate(apt.StartTime).Day = iday))
                For Each a As Appointment In aptInDay
                    Dim apt As New DayBoxAppointmentControl()
                    apt.DisplayText.Text = a.Subject
                    apt.Tag = a.AppointmentID
                    AddHandler apt.MouseDoubleClick, AddressOf Appointment_DoubleClick
                    dayBox.DayAppointmentsStack.Children.Add(apt)
                Next

            End If

            Grid.SetColumn(dayBox, (i - (iWeekCount * 7)) + iOffsetDays)
            weekRowCtrl.WeekRowGrid.Children.Add(dayBox)
        Next
        Grid.SetRow(weekRowCtrl, iWeekCount)
        MonthViewGrid.Children.Add(weekRowCtrl)
    End Sub

    Private Sub AddRowsToMonthGrid(ByVal DaysInMonth As Integer, ByVal OffSetDays As Integer)
        MonthViewGrid.RowDefinitions.Clear()
        Dim rowHeight As New System.Windows.GridLength(60, System.Windows.GridUnitType.Star)

        Dim EndOffSetDays As Integer = 7 - _
            (CInt(System.Enum.ToObject(GetType(System.DayOfWeek), _DisplayStartDate.AddDays(DaysInMonth - 1).DayOfWeek)) + 1)

        For i As Integer = 1 To CInt((DaysInMonth + OffSetDays + EndOffSetDays) / 7)
            Dim rowDef = New RowDefinition()
            rowDef.Height = rowHeight
            MonthViewGrid.RowDefinitions.Add(rowDef)
        Next
    End Sub

    Private Sub UpdateMonth(ByVal MonthsToAdd As Integer)
        Dim ev As New MonthChangedEventArgs()
        ev.OldDisplayStartDate = _DisplayStartDate
        Me.DisplayStartDate = _DisplayStartDate.AddMonths(MonthsToAdd)
        ev.NewDisplayStartDate = _DisplayStartDate
        RaiseEvent DisplayMonthChanged(ev)
    End Sub

#Region " UI Event Handlers "

    Private Sub MonthGoPrev_MouseLeftButtonUp(ByVal sender As System.Object, ByVal e As MouseButtonEventArgs)
        UpdateMonth(-1)
    End Sub

    Private Sub MonthGoNext_MouseLeftButtonUp(ByVal sender As System.Object, ByVal e As MouseButtonEventArgs)
        UpdateMonth(1)
    End Sub

    Private Sub Appointment_DoubleClick(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
        If e.Source.GetType Is GetType(DayBoxAppointmentControl) Then
            If CType(e.Source, DayBoxAppointmentControl).Tag IsNot Nothing Then
                '-- You could put your own call to your appointment-displaying code or whatever here..
                RaiseEvent AppointmentDblClicked(CInt(CType(e.Source, DayBoxAppointmentControl).Tag))
            End If
            e.Handled = True
        End If
    End Sub

    Private Sub DayBox_DoubleClick(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
        '-- call to FindVisualAncestor to make sure they didn't click on existing appointment (in which case,
        '   that appointment window is already opened by handler Appointment_DoubleClick)
        If e.Source.GetType Is GetType(DayBoxControl) AndAlso _
        Utilities.FindVisualAncestor(GetType(DayBoxAppointmentControl), e.OriginalSource) Is Nothing Then

            Dim ev As New NewAppointmentEventArgs()
            If CType(e.Source, DayBoxControl).Tag IsNot Nothing Then
                ev.StartDate = New Date(_DisplayYear, _DisplayMonth, CInt(CType(e.Source, DayBoxControl).Tag), 10, 0, 0)
                ev.EndDate = CDate(ev.StartDate).AddHours(2)
            End If
            RaiseEvent DayBoxDoubleClicked(ev)
            e.Handled = True
        End If
    End Sub

#End Region

End Class

Public Structure MonthChangedEventArgs
    Public OldDisplayStartDate As Date
    Public NewDisplayStartDate As Date
End Structure

Public Structure NewAppointmentEventArgs
    Public StartDate As Date?
    Public EndDate As Date?
    Public CandidateId As Integer?
    Public RequirementId As Integer?
End Structure

Class Utilities
    '-- Many thanks to Bea Stollnitz, on whose blog I found the original C# version of below in a drag-drop helper class... 
    Public Shared Function FindVisualAncestor( _
                ByVal ancestorType As System.Type, _
                ByVal visual As Media.Visual) As FrameworkElement

        While (visual IsNot Nothing AndAlso Not ancestorType.IsInstanceOfType(visual))
            visual = DirectCast(Media.VisualTreeHelper.GetParent(visual), Media.Visual)
        End While
        Return CType(visual, FrameworkElement)
    End Function

End Class