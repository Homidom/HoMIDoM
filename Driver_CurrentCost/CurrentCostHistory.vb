Imports System.Collections.Generic
Imports System.Text

'
' * A CurrentCost update (as described in CurrentCostUpdate) is a relative
' *  description of your electricity usage.
' *
' * E.g. you used this much electricity 2 hours ago
' *
' * This class converts this into an absolute description of your electricity
' *   usage.
' *
' * E.g. you used this much electricity at 1pm
' *
' *
' *  Dale Lane (http://dalelane.co.uk/blog)
' 


Namespace CurrentCost
    Public Class CurrentCostHistory
        Public yearDataCollection As New Dictionary(Of YearData, Integer)()
        Public monthDataCollection As New Dictionary(Of MonthData, Integer)()
        Public dayDataCollection As New Dictionary(Of DayData, Integer)()
        Public hourDataCollection As New Dictionary(Of HourData, Single)()


        Public Sub UpdateData(ByVal datasource As CurrentCostUpdate)
            ' --- YEARS ----------------------------------
            Dim y1 As YearData = YearData.GetOldDate(datasource.TimeStamp, 1)
            yearDataCollection(y1) = datasource.WattsYear1

            Dim y2 As YearData = YearData.GetOldDate(datasource.TimeStamp, 2)
            yearDataCollection(y2) = datasource.WattsYear2

            Dim y3 As YearData = YearData.GetOldDate(datasource.TimeStamp, 3)
            yearDataCollection(y3) = datasource.WattsYear3

            Dim y4 As YearData = YearData.GetOldDate(datasource.TimeStamp, 4)
            yearDataCollection(y4) = datasource.WattsYear4

            ' --- MONTHS ---------------------------------
            Dim m1 As MonthData = MonthData.GetOldDate(datasource.TimeStamp, 1)
            monthDataCollection(m1) = datasource.WattsMonth01

            Dim m2 As MonthData = MonthData.GetOldDate(datasource.TimeStamp, 2)
            monthDataCollection(m2) = datasource.WattsMonth02

            Dim m3 As MonthData = MonthData.GetOldDate(datasource.TimeStamp, 3)
            monthDataCollection(m3) = datasource.WattsMonth03

            Dim m4 As MonthData = MonthData.GetOldDate(datasource.TimeStamp, 4)
            monthDataCollection(m4) = datasource.WattsMonth04

            Dim m5 As MonthData = MonthData.GetOldDate(datasource.TimeStamp, 5)
            monthDataCollection(m5) = datasource.WattsMonth05

            Dim m6 As MonthData = MonthData.GetOldDate(datasource.TimeStamp, 6)
            monthDataCollection(m6) = datasource.WattsMonth06

            Dim m7 As MonthData = MonthData.GetOldDate(datasource.TimeStamp, 7)
            monthDataCollection(m7) = datasource.WattsMonth07

            Dim m8 As MonthData = MonthData.GetOldDate(datasource.TimeStamp, 8)
            monthDataCollection(m8) = datasource.WattsMonth08

            Dim m9 As MonthData = MonthData.GetOldDate(datasource.TimeStamp, 9)
            monthDataCollection(m9) = datasource.WattsMonth09

            Dim m10 As MonthData = MonthData.GetOldDate(datasource.TimeStamp, 10)
            monthDataCollection(m10) = datasource.WattsMonth10

            Dim m11 As MonthData = MonthData.GetOldDate(datasource.TimeStamp, 11)
            monthDataCollection(m11) = datasource.WattsMonth11

            Dim m12 As MonthData = MonthData.GetOldDate(datasource.TimeStamp, 12)
            monthDataCollection(m12) = datasource.WattsMonth12

            ' --- DAYS ------------------------------------
            Dim d1 As DayData = DayData.GetOldDate(datasource.TimeStamp, 1)
            dayDataCollection(d1) = datasource.WattsDay01

            Dim d2 As DayData = DayData.GetOldDate(datasource.TimeStamp, 2)
            dayDataCollection(d2) = datasource.WattsDay02

            Dim d3 As DayData = DayData.GetOldDate(datasource.TimeStamp, 3)
            dayDataCollection(d3) = datasource.WattsDay03

            Dim d4 As DayData = DayData.GetOldDate(datasource.TimeStamp, 4)
            dayDataCollection(d4) = datasource.WattsDay04

            Dim d5 As DayData = DayData.GetOldDate(datasource.TimeStamp, 5)
            dayDataCollection(d5) = datasource.WattsDay05

            Dim d6 As DayData = DayData.GetOldDate(datasource.TimeStamp, 6)
            dayDataCollection(d6) = datasource.WattsDay06

            Dim d7 As DayData = DayData.GetOldDate(datasource.TimeStamp, 7)
            dayDataCollection(d7) = datasource.WattsDay07

            Dim d8 As DayData = DayData.GetOldDate(datasource.TimeStamp, 8)
            dayDataCollection(d8) = datasource.WattsDay08

            Dim d9 As DayData = DayData.GetOldDate(datasource.TimeStamp, 9)
            dayDataCollection(d9) = datasource.WattsDay09

            Dim d10 As DayData = DayData.GetOldDate(datasource.TimeStamp, 10)
            dayDataCollection(d10) = datasource.WattsDay10

            Dim d11 As DayData = DayData.GetOldDate(datasource.TimeStamp, 11)
            dayDataCollection(d11) = datasource.WattsDay11

            Dim d12 As DayData = DayData.GetOldDate(datasource.TimeStamp, 12)
            dayDataCollection(d12) = datasource.WattsDay12

            Dim d13 As DayData = DayData.GetOldDate(datasource.TimeStamp, 13)
            dayDataCollection(d13) = datasource.WattsDay13

            Dim d14 As DayData = DayData.GetOldDate(datasource.TimeStamp, 14)
            dayDataCollection(d14) = datasource.WattsDay14

            Dim d15 As DayData = DayData.GetOldDate(datasource.TimeStamp, 15)
            dayDataCollection(d15) = datasource.WattsDay15

            Dim d16 As DayData = DayData.GetOldDate(datasource.TimeStamp, 16)
            dayDataCollection(d16) = datasource.WattsDay16

            Dim d17 As DayData = DayData.GetOldDate(datasource.TimeStamp, 17)
            dayDataCollection(d17) = datasource.WattsDay17

            Dim d18 As DayData = DayData.GetOldDate(datasource.TimeStamp, 18)
            dayDataCollection(d18) = datasource.WattsDay18

            Dim d19 As DayData = DayData.GetOldDate(datasource.TimeStamp, 19)
            dayDataCollection(d19) = datasource.WattsDay19

            Dim d20 As DayData = DayData.GetOldDate(datasource.TimeStamp, 20)
            dayDataCollection(d20) = datasource.WattsDay20

            Dim d21 As DayData = DayData.GetOldDate(datasource.TimeStamp, 21)
            dayDataCollection(d21) = datasource.WattsDay21

            Dim d22 As DayData = DayData.GetOldDate(datasource.TimeStamp, 22)
            dayDataCollection(d22) = datasource.WattsDay22

            Dim d23 As DayData = DayData.GetOldDate(datasource.TimeStamp, 23)
            dayDataCollection(d23) = datasource.WattsDay23

            Dim d24 As DayData = DayData.GetOldDate(datasource.TimeStamp, 24)
            dayDataCollection(d24) = datasource.WattsDay24

            Dim d25 As DayData = DayData.GetOldDate(datasource.TimeStamp, 25)
            dayDataCollection(d25) = datasource.WattsDay25

            Dim d26 As DayData = DayData.GetOldDate(datasource.TimeStamp, 26)
            dayDataCollection(d26) = datasource.WattsDay26

            Dim d27 As DayData = DayData.GetOldDate(datasource.TimeStamp, 27)
            dayDataCollection(d27) = datasource.WattsDay27

            Dim d28 As DayData = DayData.GetOldDate(datasource.TimeStamp, 28)
            dayDataCollection(d28) = datasource.WattsDay28

            Dim d29 As DayData = DayData.GetOldDate(datasource.TimeStamp, 29)
            dayDataCollection(d29) = datasource.WattsDay29

            Dim d30 As DayData = DayData.GetOldDate(datasource.TimeStamp, 30)
            dayDataCollection(d30) = datasource.WattsDay30

            Dim d31 As DayData = DayData.GetOldDate(datasource.TimeStamp, 31)
            dayDataCollection(d31) = datasource.WattsDay31

            ' --- HOURS ----------------------------------
            Dim h0 As HourData = HourData.GetOldDate(datasource.TimeStamp, 0)
            hourDataCollection(h0) = datasource.kWattsHour02

            Dim h2 As HourData = HourData.GetOldDate(datasource.TimeStamp, 2)
            hourDataCollection(h2) = datasource.kWattsHour04

            Dim h4 As HourData = HourData.GetOldDate(datasource.TimeStamp, 4)
            hourDataCollection(h4) = datasource.kWattsHour06

            Dim h6 As HourData = HourData.GetOldDate(datasource.TimeStamp, 6)
            hourDataCollection(h6) = datasource.kWattsHour08

            Dim h8 As HourData = HourData.GetOldDate(datasource.TimeStamp, 8)
            hourDataCollection(h8) = datasource.kWattsHour10

            Dim h10 As HourData = HourData.GetOldDate(datasource.TimeStamp, 10)
            hourDataCollection(h10) = datasource.kWattsHour12

            Dim h12 As HourData = HourData.GetOldDate(datasource.TimeStamp, 12)
            hourDataCollection(h12) = datasource.kWattsHour14

            Dim h14 As HourData = HourData.GetOldDate(datasource.TimeStamp, 14)
            hourDataCollection(h14) = datasource.kWattsHour16

            Dim h16 As HourData = HourData.GetOldDate(datasource.TimeStamp, 16)
            hourDataCollection(h16) = datasource.kWattsHour18

            Dim h18 As HourData = HourData.GetOldDate(datasource.TimeStamp, 18)
            hourDataCollection(h18) = datasource.kWattsHour20

            Dim h20 As HourData = HourData.GetOldDate(datasource.TimeStamp, 20)
            hourDataCollection(h20) = datasource.kWattsHour22

            Dim h22 As HourData = HourData.GetOldDate(datasource.TimeStamp, 22)
            hourDataCollection(h22) = datasource.kWattsHour24

            Dim h24 As HourData = HourData.GetOldDate(datasource.TimeStamp, 24)
            hourDataCollection(h24) = datasource.kWattsHour26
        End Sub


        '*************************************************

        '  debug                                          

        '*************************************************


        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()

            For Each kvp As KeyValuePair(Of CurrentCostHistory.HourData, Single) In hourDataCollection
                sb.AppendLine(Convert.ToString(kvp.Key) & "  =  " & kvp.Value)
            Next
            For Each kvp As KeyValuePair(Of CurrentCostHistory.DayData, Integer) In dayDataCollection
                sb.AppendLine(Convert.ToString(kvp.Key) & "  =  " & kvp.Value)
            Next
            For Each kvp As KeyValuePair(Of CurrentCostHistory.MonthData, Integer) In monthDataCollection
                sb.AppendLine(Convert.ToString(kvp.Key) & "  =  " & kvp.Value)
            Next
            For Each kvp As KeyValuePair(Of CurrentCostHistory.YearData, Integer) In yearDataCollection
                sb.AppendLine(Convert.ToString(kvp.Key) & "  =  " & kvp.Value)
            Next

            Return sb.ToString()
        End Function




        '*********************************************************

        '   inner classes used to define custom calendar types    

        '*********************************************************



        Public MustInherit Class CurrentCostTime
        End Class


        '***************************************************

        ' Represents the date for a CurrentCost record of a 

        '   year's electricity usage.                       

        '***************************************************

        Public Class YearData
            Inherits CurrentCostTime
            Implements IComparable
            Public Year As Integer

            Public Shared Function GetOldDate(ByVal referenceDate As DateTime, ByVal yearsago As Integer) As YearData
                Dim oldYear As New YearData()
                oldYear.Year = referenceDate.Year - yearsago

                Return oldYear
            End Function

            '************************************************

            '  we add the following methods so that we can   

            '   store YearData in Dictionary objects         

            '************************************************


            Public Function CompareTo(ByVal obj As Object) As Integer Implements IComparable.CompareTo
                If TypeOf obj Is YearData Then
                    Dim comp As YearData = DirectCast(obj, YearData)

                    Return Year.CompareTo(comp.Year)
                End If

                Throw New ArgumentException()
            End Function

            Public Overrides Function Equals(ByVal compare As Object) As Boolean
                If TypeOf compare Is YearData Then
                    If Year = DirectCast(compare, YearData).Year Then
                        Return True
                    End If
                End If
                Return False
            End Function

            Public Overrides Function GetHashCode() As Integer
                Return Year.GetHashCode()
            End Function

            '*************************************************

            '  debug                                          

            '*************************************************

            Public Overrides Function ToString() As String
                Return Year.ToString()
            End Function
        End Class


        '***************************************************

        ' Represents the date for a CurrentCost record of a 

        '   month's electricity usage.                      

        '***************************************************

        Public Class MonthData
            Inherits CurrentCostTime
            Implements IComparable
            Public Month As Integer
            Public Year As Integer


            Public Shared Function GetOldDate(ByVal referenceDate As DateTime, ByVal monthsago As Integer) As MonthData
                Dim oldMonth As New MonthData()
                Dim newmonth As Integer = referenceDate.Month - monthsago
                Dim newyear As Integer = referenceDate.Year

                If newmonth <= 0 Then
                    newmonth += 12
                    newyear -= 1
                End If

                oldMonth.Year = newyear
                oldMonth.Month = newmonth

                Return oldMonth
            End Function

            '************************************************

            '  we add the following methods so that we can   

            '   store MonthData in Dictionary objects        

            '************************************************


            Public Function CompareTo(ByVal obj As Object) As Integer Implements IComparable.CompareTo
                If TypeOf obj Is MonthData Then
                    Dim comp As MonthData = DirectCast(obj, MonthData)

                    Select Case Year.CompareTo(comp.Year)
                        Case -1
                            Return -1
                        Case 1
                            Return 1
                        Case 0
                            Return Month.CompareTo(comp.Month)
                    End Select
                End If

                Throw New ArgumentException()
            End Function

            Public Overrides Function Equals(ByVal compare As Object) As Boolean
                If TypeOf compare Is MonthData Then
                    If (Year = DirectCast(compare, MonthData).Year) AndAlso (Month = DirectCast(compare, MonthData).Month) Then
                        Return True
                    End If
                End If
                Return False
            End Function

            Public Overrides Function GetHashCode() As Integer
                Return Month.GetHashCode() Xor Year.GetHashCode()
            End Function

            '*************************************************

            '  debug                                          

            '*************************************************


            Public Overrides Function ToString() As String
                Select Case Month
                    Case 1
                        Return "Jan " & Year
                    Case 2
                        Return "Feb " & Year
                    Case 3
                        Return "Mar " & Year
                    Case 4
                        Return "Apr " & Year
                    Case 5
                        Return "May " & Year
                    Case 6
                        Return "Jun " & Year
                    Case 7
                        Return "Jul " & Year
                    Case 8
                        Return "Aug " & Year
                    Case 9
                        Return "Sep " & Year
                    Case 10
                        Return "Oct " & Year
                    Case 11
                        Return "Nov " & Year
                    Case 12
                        Return "Dec " & Year
                    Case Else
                        Return Month & " / " & Year
                End Select
            End Function
        End Class



        '***************************************************

        ' Represents the date for a CurrentCost record of a 

        '   days's electricity usage.                       

        '***************************************************

        Public Class DayData
            Inherits CurrentCostTime
            Implements IComparable
            Public [Date] As Integer
            Public Month As Integer
            Public Year As Integer


            Public Shared Function GetOldDate(ByVal referenceDate As DateTime, ByVal daysago As Integer) As DayData
                Dim oldDay As New DayData()
                Dim newday As Integer = referenceDate.Day - daysago
                Dim newmonth As Integer = referenceDate.Month
                Dim newyear As Integer = referenceDate.Year

                If newday <= 0 Then
                    Dim [sub] As DateTime = referenceDate.Subtract(New TimeSpan(daysago, 0, 0, 0))
                    newday = [sub].Day
                    newmonth = [sub].Month
                    newyear = [sub].Year
                End If

                oldDay.[Date] = newday
                oldDay.Month = newmonth
                oldDay.Year = newyear

                Return oldDay
            End Function

            '************************************************

            '  we add the following methods so that we can   

            '   store DayData in Dictionary objects          

            '************************************************


            Public Function CompareTo(ByVal obj As Object) As Integer Implements IComparable.CompareTo
                If TypeOf obj Is DayData Then
                    Dim comp As DayData = DirectCast(obj, DayData)

                    Select Case Year.CompareTo(comp.Year)
                        Case -1
                            Return -1
                        Case 1
                            Return 1
                        Case 0
                            Select Case Month.CompareTo(comp.Month)
                                Case -1
                                    Return -1
                                Case 1
                                    Return 1
                                Case 0
                                    Return [Date].CompareTo(comp.[Date])
                            End Select
                            Exit Select
                    End Select
                End If

                Throw New ArgumentException()
            End Function

            Public Overrides Function Equals(ByVal compare As Object) As Boolean
                If TypeOf compare Is DayData Then
                    If (Year = DirectCast(compare, DayData).Year) AndAlso (Month = DirectCast(compare, DayData).Month) AndAlso ([Date] = DirectCast(compare, DayData).[Date]) Then
                        Return True
                    End If
                End If
                Return False
            End Function

            Public Overrides Function GetHashCode() As Integer
                Return [Date].GetHashCode() Xor Month.GetHashCode() Xor Year.GetHashCode()
            End Function

            '*************************************************

            '  debug                                          

            '*************************************************


            Public Overrides Function ToString() As String
                Return [Date] & "/" & Month & "/" & Year
            End Function
        End Class


        '****************************************************

        ' Represents the date for a CurrentCost record of an 

        '   hour's electricity usage.                        

        '****************************************************


        Public Class HourData
            Inherits CurrentCostTime
            Implements IComparable
            Public TwoHourBlock As Integer
            Public [Date] As Integer
            Public Month As Integer
            Public Year As Integer


            Public Shared Function GetOldDate(ByVal referenceDate As DateTime, ByVal hoursago As Integer) As HourData
                Dim start As Integer = referenceDate.Hour
                If (start Mod 2) = 0 Then
                    hoursago += 1
                End If

                Dim newhour As Integer = start - hoursago
                Dim newday As Integer = referenceDate.Day
                Dim newmonth As Integer = referenceDate.Month
                Dim newyear As Integer = referenceDate.Year

                If newhour < -24 Then
                    newhour += 48

                    Dim [sub] As DateTime = referenceDate.Subtract(New TimeSpan(1, 0, 0, 0))
                    newday = [sub].Day
                    newmonth = [sub].Month
                    newyear = [sub].Year
                ElseIf newhour < 0 Then
                    newhour += 24

                    Dim [sub] As DateTime = referenceDate.Subtract(New TimeSpan(1, 0, 0, 0))
                    newday = [sub].Day
                    newmonth = [sub].Month
                    newyear = [sub].Year
                End If

                Dim oldHour As New HourData()
                oldHour.TwoHourBlock = newhour
                oldHour.[Date] = newday
                oldHour.Month = newmonth
                oldHour.Year = newyear

                Return oldHour
            End Function


            '************************************************

            '  we add the following methods so that we can   

            '   store DayData in Dictionary objects          

            '************************************************


            Public Function CompareTo(ByVal obj As Object) As Integer Implements IComparable.CompareTo
                If TypeOf obj Is HourData Then
                    Dim comp As HourData = DirectCast(obj, HourData)

                    Select Case Year.CompareTo(comp.Year)
                        Case -1
                            Return -1
                        Case 1
                            Return 1
                        Case 0
                            Select Case Month.CompareTo(comp.Month)
                                Case -1
                                    Return -1
                                Case 1
                                    Return 1
                                Case 0
                                    Select Case [Date].CompareTo(comp.[Date])
                                        Case -1
                                            Return -1
                                        Case 1
                                            Return 1
                                        Case 0
                                            Return TwoHourBlock.CompareTo(comp.TwoHourBlock)
                                    End Select
                                    Exit Select
                            End Select
                            Exit Select
                    End Select
                End If

                Throw New ArgumentException()
            End Function

            Public Overrides Function Equals(ByVal compare As Object) As Boolean
                If TypeOf compare Is HourData Then
                    If (TwoHourBlock = DirectCast(compare, HourData).TwoHourBlock) AndAlso (Year = DirectCast(compare, HourData).Year) AndAlso (Month = DirectCast(compare, HourData).Month) AndAlso ([Date] = DirectCast(compare, HourData).[Date]) Then
                        Return True
                    End If
                End If
                Return False
            End Function

            Public Overrides Function GetHashCode() As Integer
                Return TwoHourBlock.GetHashCode() Xor [Date].GetHashCode() Xor Month.GetHashCode() Xor Year.GetHashCode()
            End Function

            '*************************************************

            '  debug                                          

            '*************************************************


            Public Overrides Function ToString() As String
                Return TwoHourBlock & ":00  " & [Date] & "/" & Month & "/" & Year
            End Function
        End Class
    End Class
End Namespace
