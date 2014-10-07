#Region "License, Terms and Author(s)"
'
' NCrontab - Crontab for .NET
' Copyright (c) 2008 Atif Aziz. All rights reserved.
'
'  Author(s):
'
'      Atif Aziz, http://www.raboof.com
'
' Licensed under the Apache License, Version 2.0 (the "License");
' you may not use this file except in compliance with the License.
' You may obtain a copy of the License at
'
'     http://www.apache.org/licenses/LICENSE-2.0
'
' Unless required by applicable law or agreed to in writing, software
' distributed under the License is distributed on an "AS IS" BASIS,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the License for the specific language governing permissions and
' limitations under the License.
'
#End Region

#Region "Imports"

Imports HoMIDom.HoMIDom
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Runtime.Serialization
Imports Debug = System.Diagnostics.Debug

#End Region

Namespace HoMIDom

#Region "CrontabSchedule"

    ''' <summary>
    ''' Represents a schedule initialized from the crontab expression.
    ''' </summary>

    <Serializable()> _
    Public Class CrontabSchedule 'NotInheritable
        Private ReadOnly _seconds As CrontabField
        Private ReadOnly _minutes As CrontabField
        Private ReadOnly _hours As CrontabField
        Private ReadOnly _days As CrontabField
        Private ReadOnly _months As CrontabField
        Private ReadOnly _daysOfWeek As CrontabField

        <NonSerialized()> Public Shared _Server As Server


        Private Shared ReadOnly _separators As Char() = {" "c}

        '
        ' Crontab expression format:
        '
        ' * * * * * *
        ' - - - - - -
        ' | | | | | |
        ' | | | | | +----- day of week (0 - 6) (Sunday=0)
        ' | | | | +------- month (1 - 12)
        ' | | | +--------- day of month (1 - 31)
        ' | | +----------- hour (0 - 23)
        ' | +------------- min (0 - 59)
        ' +--------------- sec (0 - 59)
        ' Star (*) in the value field above means all legal values as in 
        ' braces for that column. The value column can have a * or a list 
        ' of elements separated by commas. An element is either a number in 
        ' the ranges shown above or two numbers in the range separated by a 
        ' hyphen (meaning an inclusive range). 
        '
        ' Source: http://www.adminschoice.com/docs/crontab.htm
        '

        Public Shared Function Parse(ByVal expression As String) As CrontabSchedule

            Return TryParse(expression, ErrorHandling.[Throw]).Value

        End Function

        Public Shared Function TryParse(ByVal expression As String) As ValueOrError(Of CrontabSchedule)
            Return TryParse(expression, Nothing)
        End Function

        Private Shared Function TryParse(ByVal expression As String, ByVal onError As ExceptionHandler) As ValueOrError(Of CrontabSchedule)
            Try
                If expression Is Nothing Then
                    Throw New ArgumentNullException("expression")
                End If

                Dim tokens = expression.Split(_separators, StringSplitOptions.RemoveEmptyEntries)

                If tokens.Length <> 6 Then
                    Return ErrorHandling.OnError(Function() New CrontabException(String.Format("'{0}' is not a valid crontab expression. It must contain at least 6 components of a schedule " & "(in the sequence of seconds,minutes, hours, days, months, days of week).", expression)), onError)
                End If

                Dim fields = New CrontabField(5) {}

                For i As Integer = 0 To fields.Length - 1
                    Dim field = CrontabField.TryParse(DirectCast(i, CrontabFieldKind), tokens(i), onError)
                    If field.IsError Then
                        Return field.ErrorProvider
                    End If

                    fields(i) = field.Value
                Next

                Return New CrontabSchedule(_Server, fields(0), fields(1), fields(2), fields(3), fields(4), fields(5))
            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, " TryParse Cron Execute", "Exception: " & ex.ToString)
                Return Nothing
            End Try
        End Function

        Private Sub New( ByVal server As Server,ByVal seconds As CrontabField, ByVal minutes As CrontabField, ByVal hours As CrontabField, ByVal days As CrontabField, ByVal months As CrontabField, ByVal daysOfWeek As CrontabField)
            _Server = server
            Debug.Assert(seconds IsNot Nothing)
            Debug.Assert(minutes IsNot Nothing)
            Debug.Assert(hours IsNot Nothing)
            Debug.Assert(days IsNot Nothing)
            Debug.Assert(months IsNot Nothing)
            Debug.Assert(daysOfWeek IsNot Nothing)

            _seconds = seconds
            _minutes = minutes
            _hours = hours
            _days = days
            _months = months

            _daysOfWeek = daysOfWeek
        End Sub

        ''' <summary>
        ''' Gets the next occurrence of this schedule starting with a base time.
        ''' </summary>

        Public Function GetNextOccurrence(ByVal baseTime As DateTime) As DateTime
            Return GetNextOccurrence(baseTime, DateTime.MaxValue)
        End Function

        ''' <summary>
        ''' Gets the next occurrence of this schedule starting with a base 
        ''' time and up to an end time limit.
        ''' </summary>
        ''' <remarks>
        ''' This method does not return the value of <paramref name="baseTime"/>
        ''' itself if it falls on the schedule. For example, if <paramref name="baseTime" />
        ''' is midnight and the schedule was created from the expression <c>* * * * *</c> 
        ''' (meaning every minute) then the next occurrence of the schedule 
        ''' will be at one minute past midnight and not midnight itself.
        ''' The method returns the <em>next</em> occurrence <em>after</em> 
        ''' <paramref name="baseTime"/>. Also, <param name="endTime" /> is
        ''' exclusive.
        ''' </remarks>

        Public Function GetNextOccurrence(ByVal baseTime As DateTime, ByVal endTime As DateTime) As DateTime

            Try

                Const nil As Integer = -1

                Dim baseYear = baseTime.Year
                Dim baseMonth = baseTime.Month
                Dim baseDay = baseTime.Day
                Dim baseHour = baseTime.Hour
                Dim baseMinute = baseTime.Minute
                Dim baseSecond = baseTime.Second

                Dim endYear = endTime.Year
                Dim endMonth = endTime.Month
                Dim endDay = endTime.Day

                Dim year = baseYear
                Dim month = baseMonth
                Dim day = baseDay
                Dim hour = baseHour
                Dim minute = baseMinute
                Dim second = baseSecond + 1

                '
                ' Second
                '

                Dim secondM = _seconds.[Next](second)

                If secondM = nil Then
                    minute += 1
                    second = _seconds.GetNext(second)
                Else
                    second = secondM
                End If
                If second = nil Then
                    second = _seconds.GetFirst()
                End If

                '
                ' Minute
                '

                Dim minuteM = _minutes.[Next](minute)

                If minuteM = nil Then
                    minute = _minutes.GetNext(minute)
                    hour += 1
                Else
                    minute = minuteM
                End If
                If minute = nil Then
                    'second = _seconds.GetFirst();
                    minute = _minutes.GetFirst()
                End If

                '
                ' Hour
                '

                Dim hourm = _hours.[Next](hour)

                If hourm = nil Then
                    hour = _hours.GetNext(hour)
                    day += 1
                Else
                    hour = hourm
                End If
                If hour = nil Then
                    'second = _seconds.GetFirst();
                    'minute = _minutes.GetFirst();
                    hour = _hours.GetFirst()
                    'second = _seconds.GetFirst();
                    'minute = _minutes.GetFirst();
                ElseIf hour > baseHour Then
                End If

                '
                ' Day
                '

                Dim dayM = _days.[Next](day)
RetryDayMonth:
                'Dim dayM = _days.[Next](day)
                If dayM = nil Then
                    day = _days.GetNext(day)
                    month += 1
                Else
                    day = dayM
                End If
                If day = nil Then
                    'second = _seconds.GetFirst();
                    'minute = _minutes.GetFirst();
                    'hour = _hours.GetFirst();
                    day = _days.GetFirst()
                    'second = _seconds.GetFirst();
                    'minute = _minutes.GetFirst();
                    'hour = _hours.GetFirst();
                ElseIf day > baseDay Then
                End If

                '
                ' Month
                '

                Dim monthM = _months.[Next](month)

                If monthM = nil Then
                    month = _months.GetNext(day)
                    year += 1
                Else
                    month = monthM
                End If
                If month = nil Then
                    'second = _seconds.GetFirst();
                    'minute = _minutes.GetFirst();
                    'hour = _hours.GetFirst();
                    'day = _days.GetFirst();
                    month = _months.GetFirst()
                    'second = _seconds.GetFirst();
                    'minute = _minutes.GetFirst();
                    'hour = _hours.GetFirst();
                    'day = _days.GetFirst();
                ElseIf month > baseMonth Then
                End If

                '
                ' The day field in a cron expression spans the entire range of days
                ' in a month, which is from 1 to 31. However, the number of days in
                ' a month tend to be variable depending on the month (and the year
                ' in case of February). So a check is needed here to see if the
                ' date is a border case. If the day happens to be beyond 28
                ' (meaning that we're dealing with the suspicious range of 29-31)
                ' and the date part has changed then we need to determine whether
                ' the day still makes sense for the given year and month. If the
                ' day is beyond the last possible value, then the day/month part
                ' for the schedule is re-evaluated. So an expression like "0 0
                ' 15,31 * *" will yield the following sequence starting on midnight
                ' of Jan 1, 2000:
                '
                '  Jan 15, Jan 31, Feb 15, Mar 15, Apr 15, Apr 31, ...
                '

DOW:            Dim dateChanged = day <> baseDay OrElse month <> baseMonth OrElse year <> baseYear


                If day > 28 AndAlso dateChanged AndAlso day > Calendar.GetDaysInMonth(year, month) Then
                    If year >= endYear AndAlso month >= endMonth AndAlso day >= endDay Then
                        Return endTime
                    End If

                    day = nil
                    dayM = nil
                    GoTo RetryDayMonth
                End If

                If Not _daysOfWeek.Contains(Convert.ToInt32(New DateTime(year, month, day, hour, minute, second, _
                0, baseTime.Kind).DayOfWeek)) Then
                    day += 1
                    GoTo DOW
                End If

                Dim nextTime = New DateTime(year, month, day, hour, minute, second, _
                 0, baseTime.Kind)

                If nextTime >= endTime Then
                    Return endTime
                End If

                '
                ' Day of week
                '

                If _daysOfWeek.Contains(Convert.ToInt32(nextTime.DayOfWeek)) Then
                    Return nextTime
                End If

                Return GetNextOccurrence(New DateTime(year, month, day, 23, 59, 0, _
                 0, baseTime.Kind), endTime)

            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "GetNextOccurence Cron Execute", "Exception: " & ex.ToString)
            End Try
        End Function

        ''' <summary>
        ''' Returns a string in crontab expression (expanded) that represents 
        ''' this schedule.
        ''' </summary>

        Public Overrides Function ToString() As String
            Dim writer = New StringWriter(CultureInfo.InvariantCulture)

            _seconds.Format(writer, True)
            writer.Write(" "c)
            _minutes.Format(writer, True)
            writer.Write(" "c)
            _hours.Format(writer, True)
            writer.Write(" "c)
            _days.Format(writer, True)
            writer.Write(" "c)
            _months.Format(writer, True)
            writer.Write(" "c)
            _daysOfWeek.Format(writer, True)

            Return writer.ToString()
        End Function

        Private Shared ReadOnly Property Calendar() As Calendar
            Get
                Return CultureInfo.InvariantCulture.Calendar
            End Get
        End Property
    End Class
#End Region

#Region "CrontabField"

    ''' <summary>
    ''' Represents a single crontab field.
    ''' </summary>

    <Serializable()> _
    Public NotInheritable Class CrontabField
        Implements ICrontabField
        Private ReadOnly _bits As BitArray
        ' readonly 
        Private _minValueSet As Integer
        ' readonly 
        Private _maxValueSet As Integer
        Private ReadOnly _impl As CrontabFieldImpl

        ''' <summary>
        ''' Parses a crontab field expression given its kind.
        ''' </summary>

        Public Shared Function Parse(ByVal kind As CrontabFieldKind, ByVal expression As String) As CrontabField
            Return TryParse(kind, expression, ErrorHandling.[Throw]).Value
        End Function

        Public Shared Function TryParse(ByVal kind As CrontabFieldKind, ByVal expression As String) As ValueOrError(Of CrontabField)
            Return TryParse(kind, expression, Nothing)
        End Function

        Public Shared Function TryParse(ByVal kind As CrontabFieldKind, ByVal expression As String, ByVal onError As ExceptionHandler) As ValueOrError(Of CrontabField)

            Dim field = New CrontabField(CrontabFieldImpl.FromKind(kind))
            Dim [error] As ValueOrError(Of CrontabField) = field._impl.TryParse(expression, AddressOf field.Accumulate, onError)
            Return If([error].Error Is Nothing, field, [error].Value)

            'Return If([error] Is Nothing, field, DirectCast([error], ValueOrError(Of CrontabField)))
        End Function

        ''' <summary>
        ''' Parses a crontab field expression representing seconds.
        ''' </summary>

        Public Shared Function seconds(ByVal expression As String) As CrontabField
            Return Parse(CrontabFieldKind.Second, expression)
        End Function
        ''' <summary>
        ''' Parses a crontab field expression representing minutes.
        ''' </summary>

        Public Shared Function Minutes(ByVal expression As String) As CrontabField
            Return Parse(CrontabFieldKind.Minute, expression)
        End Function

        ''' <summary>
        ''' Parses a crontab field expression representing hours.
        ''' </summary>

        Public Shared Function Hours(ByVal expression As String) As CrontabField
            Return Parse(CrontabFieldKind.Hour, expression)
        End Function

        ''' <summary>
        ''' Parses a crontab field expression representing days in any given month.
        ''' </summary>

        Public Shared Function Days(ByVal expression As String) As CrontabField
            Return Parse(CrontabFieldKind.Day, expression)
        End Function

        ''' <summary>
        ''' Parses a crontab field expression representing months.
        ''' </summary>

        Public Shared Function Months(ByVal expression As String) As CrontabField
            Return Parse(CrontabFieldKind.Month, expression)
        End Function

        ''' <summary>
        ''' Parses a crontab field expression representing days of a week.
        ''' </summary>

        Public Shared Function DaysOfWeek(ByVal expression As String) As CrontabField
            Return Parse(CrontabFieldKind.DayOfWeek, expression)
        End Function

        Private Sub New(ByVal impl As CrontabFieldImpl)
            If impl Is Nothing Then
                Throw New ArgumentNullException("impl")
            End If

            _impl = impl
            _bits = New BitArray(impl.ValueCount)

            _bits.SetAll(False)
            _minValueSet = Integer.MaxValue
            _maxValueSet = -1
        End Sub
        ''' <summary>
        ''' Gets the first value of the field or -1.
        ''' </summary>

        Public Function GetFirst() As Integer Implements ICrontabField.GetFirst
            Return If(_minValueSet < Integer.MaxValue, _minValueSet, -1)
        End Function

        ''' <summary>
        ''' Gets the next value of the field that occurs after the given 
        ''' start value or -1 if there is no next value available.
        ''' </summary>

        Public Function [Next](ByVal start As Integer) As Integer Implements ICrontabField.Next
            If start < _minValueSet Then
                Return _minValueSet
            End If

            Dim startIndex = ValueToIndex(start)
            If _impl.Every > 1 Then
                Dim temp = 0
                If _impl.Kind = CrontabFieldKind.Second Then
                    temp = 1
                Else
                    temp = 0
                End If
                If startIndex + _impl.Every - temp <= _impl.MaxValue Then
                    Return startIndex + _impl.Every - temp + _impl.MinValue
                End If
            Else
                Dim lastIndex = ValueToIndex(_maxValueSet)

                For i As Integer = startIndex To lastIndex
                    If _bits(i) Then
                        Return IndexToValue(i)
                    End If

                Next
            End If
            Return -1
        End Function
        ''' <summary>
        ''' Gets the first value of the field or -1.
        ''' </summary>

        Public Function GetNext(ByVal start As Integer) As Integer Implements ICrontabField.GetNext

            If start < _minValueSet Then
                Return _minValueSet
            End If

            Dim startIndex = ValueToIndex(start)

            If _impl.Every > 1 And startIndex + _impl.Every > _impl.MaxValue Then
                Dim temp = 0
                If _impl.Kind = CrontabFieldKind.Second Then
                    temp = 1
                Else
                    temp = 0
                End If
                Return startIndex + _impl.Every - _impl.MaxValue - 1 + _impl.MinValue - temp
            End If

            Return -1
        End Function

        Private Function IndexToValue(ByVal index As Integer) As Integer
            Return index + _impl.MinValue
        End Function

        Private Function ValueToIndex(ByVal value As Integer) As Integer
            Return value - _impl.MinValue
        End Function

        ''' <summary>
        ''' Determines if the given value occurs in the field.
        ''' </summary>

        Public Function Contains(ByVal value As Integer) As Boolean Implements ICrontabField.Contains
            Return _bits(ValueToIndex(value))
        End Function

        ''' <summary>
        ''' Accumulates the given range (start to end) and interval of values
        ''' into the current set of the field.
        ''' </summary>
        ''' <remarks>
        ''' To set the entire range of values representable by the field,
        ''' set <param name="start" /> and <param name="end" /> to -1 and
        ''' <param name="interval" /> to 1.
        ''' </remarks>

        Private Function Accumulate(ByVal start As Integer, ByVal [end] As Integer, ByVal interval As Integer, ByVal onError As ExceptionHandler) As ExceptionProvider
            Dim minValue = _impl.MinValue
            Dim maxValue = _impl.MaxValue

            If start = [end] Then
                If start < 0 Then
                    '
                    ' We're setting the entire range of values.
                    '

                    If interval <= 1 Then
                        _minValueSet = minValue
                        _maxValueSet = maxValue
                        _bits.SetAll(True)
                        Return Nothing
                    End If

                    start = minValue
                    [end] = maxValue
                Else
                    '
                    ' We're only setting a single value - check that it is in range.
                    '

                    If start < minValue Then
                        Return OnValueBelowMinError(start, onError)
                    End If

                    If start > maxValue Then
                        Return OnValueAboveMaxError(start, onError)
                    End If
                End If
            Else
                '
                ' For ranges, if the start is bigger than the end value then
                ' swap them over.
                '

                If start > [end] Then
                    [end] = [end] Xor start
                    start = start Xor [end]
                    [end] = [end] Xor start
                End If

                If start < 0 Then
                    start = minValue
                ElseIf start < minValue Then
                    Return OnValueBelowMinError(start, onError)
                End If

                If [end] < 0 Then
                    [end] = maxValue
                ElseIf [end] > maxValue Then
                    Return OnValueAboveMaxError([end], onError)
                End If
            End If

            If interval < 1 Then
                interval = 1
            End If

            Dim i As Integer

            '
            ' Populate the _bits table by setting all the bits corresponding to
            ' the valid field values.
            '

            i = start - minValue
            While i <= ([end] - minValue)
                _bits(i) = True
                i += interval
            End While

            '
            ' Make sure we remember the minimum value set so far Keep track of
            ' the highest and lowest values that have been added to this field
            ' so far.
            '

            If _minValueSet > start Then
                _minValueSet = start
            End If

            i += (minValue - interval)

            If _maxValueSet < i Then
                _maxValueSet = i
            End If

            Return Nothing
        End Function

        Private Function OnValueAboveMaxError(ByVal value As Integer, ByVal onError As ExceptionHandler) As ExceptionProvider
            Return ErrorHandling.OnError(Function() New CrontabException(String.Format("{0} is higher than the maximum allowable value for the [{3}] field. Value must be between {1} and {2} (all inclusive).", value, _impl.MinValue, _impl.MaxValue, _impl.Kind)), onError)
        End Function

        Private Function OnValueBelowMinError(ByVal value As Integer, ByVal onError As ExceptionHandler) As ExceptionProvider
            Return ErrorHandling.OnError(Function() New CrontabException(String.Format("{0} is lower than the minimum allowable value for the [{3}] field. Value must be between {1} and {2} (all inclusive).", value, _impl.MinValue, _impl.MaxValue, _impl.Kind)), onError)
        End Function

        Public Overrides Function ToString() As String
            Return ToString(Nothing)
        End Function

        Public Overloads Function ToString(ByVal format__1 As String) As String
            Dim writer = New StringWriter(CultureInfo.InvariantCulture)

            Select Case format__1
                Case "G", Nothing
                    Format(writer, True)
                    Exit Select
                Case "N"
                    Format(writer)
                    Exit Select
                Case Else
                    Throw New FormatException()
            End Select

            Return writer.ToString()
        End Function

        Public Sub Format(ByVal writer As TextWriter)
            Format(writer, False)
        End Sub

        Public Sub Format(ByVal writer As TextWriter, ByVal noNames As Boolean)
            _impl.Format(Me, writer, noNames)
        End Sub
    End Class
#End Region

#Region "CrontabFieldImpl"

    Public Delegate Function CrontabFieldAccumulator(ByVal start As Integer, ByVal [end] As Integer, ByVal interval As Integer, ByVal onError As ExceptionHandler) As ExceptionProvider

    <Serializable()> _
    Public NotInheritable Class CrontabFieldImpl
        Implements IObjectReference
        Public Shared ReadOnly Second As New CrontabFieldImpl(CrontabFieldKind.Second, 0, 59, Nothing)
        Public Shared ReadOnly Minute As New CrontabFieldImpl(CrontabFieldKind.Minute, 0, 59, Nothing)
        Public Shared ReadOnly Hour As New CrontabFieldImpl(CrontabFieldKind.Hour, 0, 23, Nothing)
        Public Shared ReadOnly Day As New CrontabFieldImpl(CrontabFieldKind.Day, 1, 31, Nothing)
        Public Shared ReadOnly Month As New CrontabFieldImpl(CrontabFieldKind.Month, 1, 12, {"January", "February", "March", "April", "May", "June", _
   "July", "August", "September", "October", "November", "December"})
        Public Shared ReadOnly DayOfWeek As New CrontabFieldImpl(CrontabFieldKind.DayOfWeek, 0, 6, {"Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", _
   "Saturday"})

        Private Shared ReadOnly _fieldByKind As CrontabFieldImpl() = {Second, Minute, Hour, Day, Month, DayOfWeek}

        Private Shared ReadOnly _comparer As CompareInfo = CultureInfo.InvariantCulture.CompareInfo
        Private Shared ReadOnly _comma As Char() = {","c}

        Private ReadOnly _kind As CrontabFieldKind
        Private ReadOnly _minValue As Integer
        Private ReadOnly _maxValue As Integer
        Private _every As Integer
        Private ReadOnly _names As String()

        Public Shared Function FromKind(ByVal kind As CrontabFieldKind) As CrontabFieldImpl
            If Not [Enum].IsDefined(GetType(CrontabFieldKind), kind) Then
                Throw New ArgumentException(String.Format("Invalid crontab field kind. Valid values are {0}.", String.Join(", ", [Enum].GetNames(GetType(CrontabFieldKind)))), "kind")
            End If

            Return _fieldByKind(CInt(kind))
        End Function

        Private Sub New(ByVal kind As CrontabFieldKind, ByVal minValue As Integer, ByVal maxValue As Integer, ByVal names As String())
            Debug.Assert([Enum].IsDefined(GetType(CrontabFieldKind), kind))
            Debug.Assert(minValue >= 0)
            Debug.Assert(maxValue >= minValue)
            Debug.Assert(names Is Nothing OrElse names.Length = (maxValue - minValue + 1))

            _kind = kind
            _minValue = minValue
            _maxValue = maxValue
            _names = names
        End Sub

        Public ReadOnly Property Kind() As CrontabFieldKind
            Get
                Return _kind
            End Get
        End Property

        Public ReadOnly Property MinValue() As Integer
            Get
                Return _minValue
            End Get
        End Property

        Public ReadOnly Property MaxValue() As Integer
            Get
                Return _maxValue
            End Get
        End Property

        Public ReadOnly Property Every() As Integer
            Get
                Return _every
            End Get
        End Property

        Public ReadOnly Property ValueCount() As Integer
            Get
                Return _maxValue - _minValue + 1
            End Get
        End Property

        Public Sub Format(ByVal field As ICrontabField, ByVal writer As TextWriter)
            Format(field, writer, False)
        End Sub

        Public Sub Format(ByVal field As ICrontabField, ByVal writer As TextWriter, ByVal noNames As Boolean)
            If field Is Nothing Then
                Throw New ArgumentNullException("field")
            End If

            If writer Is Nothing Then
                Throw New ArgumentNullException("writer")
            End If

            Dim [next] = field.GetFirst()
            Dim count = 0

            While [next] <> -1
                Dim first = [next]
                Dim last As Integer

                Do
                    last = [next]
                    [next] = field.[Next](last + 1)
                Loop 'While [next] - last = 1 'Is 1

                If count = 0 AndAlso first = _minValue AndAlso last = _maxValue Then
                    writer.Write("*"c)
                    Return
                End If

                If count > 0 Then
                    writer.Write(","c)
                End If

                If first = last Then
                    FormatValue(first, writer, noNames)
                Else
                    FormatValue(first, writer, noNames)
                    writer.Write("-"c)
                    FormatValue(last, writer, noNames)
                End If

                count += 1
            End While
        End Sub

        Private Sub FormatValue(ByVal value As Integer, ByVal writer As TextWriter, ByVal noNames As Boolean)
            Debug.Assert(writer IsNot Nothing)

            If noNames OrElse _names Is Nothing Then
                If value >= 0 AndAlso value < 100 Then
                    FastFormatNumericValue(value, writer)
                Else
                    writer.Write(value.ToString(CultureInfo.InvariantCulture))
                End If
            Else
                Dim index = value - _minValue
                writer.Write(_names(index))
            End If
        End Sub

        Private Shared Sub FastFormatNumericValue(ByVal value As Integer, ByVal writer As TextWriter)
            Debug.Assert(value >= 0 AndAlso value < 100)
            Debug.Assert(writer IsNot Nothing)

            If value >= 10 Then
                writer.Write(ChrW("0"c & (value \ 10)))
                writer.Write(ChrW("0"c & (value Mod 10)))
            Else
                writer.Write(ChrW("0"c & value))
            End If
        End Sub

        Public Sub Parse(ByVal str As String, ByVal acc As CrontabFieldAccumulator)
            TryParse(str, acc, ErrorHandling.[Throw])
        End Sub

        Public Function TryParse(ByVal str As String, ByVal acc As CrontabFieldAccumulator, ByVal onError As ExceptionHandler) As ExceptionProvider
            If acc Is Nothing Then
                Throw New ArgumentNullException("acc")
            End If

            If String.IsNullOrEmpty(str) Then
                Return Nothing
            End If

            Try
                Return InternalParse(str, acc, onError)
            Catch e As FormatException
                Return OnParseException(e, str, onError)
            Catch e As CrontabException
                Return OnParseException(e, str, onError)
            End Try
        End Function

        Private Function OnParseException(ByVal innerException As Exception, ByVal str As String, ByVal onError As ExceptionHandler) As ExceptionProvider
            Debug.Assert(str IsNot Nothing)
            Debug.Assert(innerException IsNot Nothing)

            Return ErrorHandling.OnError(Function() New CrontabException(String.Format("'{0}' is not a valid [{1}] crontab field expression.", str, Kind), innerException), onError)
        End Function

        Private Function InternalParse(ByVal str As String, ByVal acc As CrontabFieldAccumulator, ByVal onError As ExceptionHandler) As ExceptionProvider
            Debug.Assert(str IsNot Nothing)
            Debug.Assert(acc IsNot Nothing)

            If str.Length = 0 Then
                Return ErrorHandling.OnError(Function() New CrontabException("A crontab field value cannot be empty."), onError)
            End If

            '
            ' Next, look for a list of values (e.g. 1,2,3).
            '

            Dim commaIndex = str.IndexOf(",")

            If commaIndex > 0 Then
                Dim e As ExceptionProvider = Nothing
                Dim token = DirectCast(str.Split(_comma), IEnumerable(Of String)).GetEnumerator()
                While token.MoveNext() AndAlso e Is Nothing
                    e = InternalParse(token.Current, acc, onError)
                End While
                Return e
            End If

            _every = 1

            'crontab field expression
            ' Look for stepping first (e.g. */2 = every 2nd).
            ' 

            Dim slashIndex = str.IndexOf("/")

            If slashIndex > 0 Then
                _every = Integer.Parse(str.Substring(slashIndex + 1), CultureInfo.InvariantCulture)
                str = str.Substring(0, slashIndex)
            End If

            '
            ' Next, look for wildcard (*).
            '

            If str.Length = 1 AndAlso str(0) = "*"c Then
                ' if (str.Length == 1 && str[0] == '*')
                Return acc(-1, -1, _every, onError)
            End If

            '
            ' Next, look for a range of values (e.g. 2-10).
            '

            Dim dashIndex = str.IndexOf("-")

            If dashIndex > 0 Then
                Dim first = ParseValue(str.Substring(0, dashIndex))
                Dim last = ParseValue(str.Substring(dashIndex + 1))

                Return acc(first, last, _every, onError)
            End If

            '
            ' Finally, handle the case where there is only one number.
            '

            Dim value = ParseValue(str)

            If _every = 1 Then
                Return acc(value, value, 1, onError)
            End If

            Debug.Assert(_every <> 0)
            Return acc(value, _maxValue, _every, onError)
        End Function

        Private Function ParseValue(ByVal str As String) As Integer
            Debug.Assert(str IsNot Nothing)

            If str.Length = 0 Then
                Throw New CrontabException("A crontab field value cannot be empty.")
            End If

            Dim firstChar = str(0)

            If firstChar >= "0"c AndAlso firstChar <= "9"c Then
                Return Integer.Parse(str, CultureInfo.InvariantCulture)
            End If

            If _names Is Nothing Then
                Throw New CrontabException(String.Format("'{0}' is not a valid [{3}] crontab field value. It must be a numeric value between {1} and {2} (all inclusive).", str, _minValue.ToString(), _maxValue.ToString(), _kind.ToString()))
            End If

            For i As Integer = 0 To _names.Length - 1
                If _comparer.IsPrefix(_names(i), str, CompareOptions.IgnoreCase) Then
                    Return i + _minValue
                End If
            Next

            Throw New CrontabException(String.Format("'{0}' is not a known value name. Use one of the following: {1}.", str, String.Join(", ", _names)))
        End Function

        Private Function IObjectReference_GetRealObject(ByVal context As StreamingContext) As Object Implements IObjectReference.GetRealObject
            Return FromKind(Kind)
        End Function
    End Class
#End Region

#Region "CrontabFieldKind"

    <Serializable()> _
    Public Enum CrontabFieldKind
        Second = 0
        Minute
        ' Keep in order of appearance in expression
        Hour
        Day
        Month
        DayOfWeek
    End Enum

#End Region

#Region "CrontabException"

    <Serializable()> _
    Public Class CrontabException
        Inherits Exception
        Public Sub New()
            MyBase.New("Crontab error.")
        End Sub
        ' TODO: Fix message and add it to resource.
        Public Sub New(ByVal message As String)
            MyBase.New(message)
        End Sub

        Public Sub New(ByVal message As String, ByVal innerException As Exception)
            MyBase.New(message, innerException)
        End Sub

        Protected Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)
            MyBase.New(info, context)
        End Sub
    End Class

#End Region

#Region "ErrorHandling"

    ''' <summary>
    ''' Represents the method that will handle an <see cref="Exception"/> object.
    ''' </summary>

    Public Delegate Sub ExceptionHandler(ByVal e As Exception)

    ''' <summary>
    ''' Represents the method that will generate an <see cref="Exception"/> object.
    ''' </summary>

    Public Delegate Function ExceptionProvider() As Exception

    ''' <summary>
    ''' Defines error handling strategies.
    ''' </summary>

    Friend NotInheritable Class ErrorHandling
        Private Sub New()
        End Sub
        ''' <summary>
        ''' A stock <see cref="ExceptionHandler"/> that throws.
        ''' </summary>

        Public Shared ReadOnly [Throw] As ExceptionHandler = Function(e)
                                                                 Throw e

                                                             End Function

        Friend Shared Function OnError(ByVal provider As ExceptionProvider, ByVal handler As ExceptionHandler) As ExceptionProvider
            Debug.Assert(provider IsNot Nothing)

            handler(provider())

            Return (provider)
        End Function
    End Class

#End Region

#Region "ICrontabField"

    Public Interface ICrontabField
        Function GetFirst() As Integer
        Function GetNext(ByVal start As Integer) As Integer
        Function [Next](ByVal start As Integer) As Integer
        Function Contains(ByVal value As Integer) As Boolean
    End Interface

#End Region

#Region "ValueOrError"

    ''' <summary>
    ''' A generic type that either represents a value or an error condition.
    ''' </summary>

    <Serializable()> _
    Public Structure ValueOrError(Of T)
        Private ReadOnly _hasValue As Boolean
        Private ReadOnly _value As T
        Private ReadOnly _ep As ExceptionProvider

        Private Shared ReadOnly _dep As ExceptionProvider = Function() New Exception("Value is undefined.")

        ''' <summary>
        ''' Initializes the object with a defined value.
        ''' </summary>

        Public Sub New(ByVal value As T)
            'Me.New(_value)
            _hasValue = True
            _value = value
        End Sub

        ''' <summary>
        ''' Initializes the object with an error.
        ''' </summary>

        Public Sub New(ByVal [error] As Exception)
            Me.new(CheckError([error]))
        End Sub

        Private Shared Function CheckError(ByVal [error] As Exception) As ExceptionProvider
            If [error] Is Nothing Then
                Throw New ArgumentNullException("error")
            End If
            Return Function() [error]
        End Function

        ''' <summary>
        ''' Initializes the object with a handler that will provide
        ''' the error result when needed.
        ''' </summary>

        Public Sub New(ByVal provider As ExceptionProvider)
            'me.new
            Try
                'If provider IsNot Nothing Then
                'Throw New ArgumentNullException("provider", "")
                'End If
                _ep = provider
            Catch e As Exception
                'MsgBox(e.Message)
            End Try


        End Sub

        ''' <summary>
        ''' Determines if object holds a defined value or not.
        ''' </summary>

        Public ReadOnly Property HasValue() As Boolean
            Get
                Return _hasValue
            End Get
        End Property

        ''' <summary>
        ''' Gets the value otherwise throws an error if undefined.
        ''' </summary>

        Public ReadOnly Property Value() As T

            Get
                Try
                    If Not HasValue Then
                        ' Throw ErrorProvider.Invoke
                    End If
                    Return _value
                Catch e As Exception
                    'MsgBox(e.Message)
                    Return Nothing
                End Try
            End Get
        End Property

        ''' <summary>
        ''' Determines if object identifies an error condition or not.
        ''' </summary>

        Public ReadOnly Property IsError() As Boolean
            Get
                'Return ErrorProvider IsNot Nothing
            End Get
        End Property

        ''' <summary>
        ''' Gets the <see cref="Exception"/> object if this object
        ''' represents an error condition otherwise it returns <c>null</c>.
        ''' </summary>

        Public ReadOnly Property [Error]() As Exception
            Get

                'Return If(IsError, ErrorProvider.Invoke, Nothing)
                Return Nothing
            End Get
        End Property

        ''' <summary>
        ''' Gets the <see cref="ExceptionProvider"/> object if this 
        ''' object represents an error condition otherwise it returns <c>null</c>.
        ''' </summary>

        Public ReadOnly Property ErrorProvider() As ExceptionProvider
            Get
                Return If(HasValue, Nothing, If(_ep, _dep))
            End Get
        End Property

        ''' <summary>
        ''' Attempts to get the defined value or another in case
        ''' of an error.
        ''' </summary>

        Public Function TryGetValue(ByVal errorValue As T) As T
            Return If(IsError, errorValue, Value)
        End Function

        ''' <summary>
        ''' Implicitly converts a <typeparamref name="T"/> value to
        ''' an object of this type.
        ''' </summary>

        Public Shared Widening Operator CType(ByVal value As T) As ValueOrError(Of T)
            Return New ValueOrError(Of T)(value)
        End Operator

        ''' <summary>
        ''' Implicitly converts an <see cref="Exception"/> object to
        ''' an object of this type that represents the error condition.
        ''' </summary>

        Public Shared Widening Operator CType(ByVal [error] As Exception) As ValueOrError(Of T)
            Return New ValueOrError(Of T)([error])
        End Operator

        ''' <summary>
        ''' Implicitly converts an <see cref="ExceptionProvider"/> object to
        ''' an object of this type that represents the error condition.
        ''' </summary>

        Public Shared Widening Operator CType(ByVal provider As ExceptionProvider) As ValueOrError(Of T)
            Return New ValueOrError(Of T)(provider)
        End Operator

        ''' <summary>
        ''' Explicits converts this object to a <typeparamref name="T"/> value.
        ''' </summary>

        Public Shared Narrowing Operator CType(ByVal ve As ValueOrError(Of T)) As T
            Return ve.Value
        End Operator

        ''' <summary>
        ''' Explicits converts this object to an <see cref="Exception"/> object
        ''' if it represents an error condition. The conversion yields <c>null</c>
        ''' if this object does not represent an error condition.
        ''' </summary>

        Public Shared Narrowing Operator CType(ByVal ve As ValueOrError(Of T)) As Exception
            Return ve.[Error]
        End Operator

        ''' <summary>
        ''' Explicits converts this object to an <see cref="ExceptionProvider"/> object
        ''' if it represents an error condition. The conversion yields <c>null</c>
        ''' if this object does not represent an error condition.
        ''' </summary>

        Public Shared Narrowing Operator CType(ByVal ve As ValueOrError(Of T)) As ExceptionProvider
            Return ve.ErrorProvider
        End Operator

        Public Overrides Function ToString() As String
            Dim error__1 = [Error]
            Return If(IsError, error__1.[GetType]().FullName & ": " & error__1.Message, If(_value IsNot Nothing, _value.ToString(), String.Empty))
        End Function
    End Structure

#End Region

End Namespace
