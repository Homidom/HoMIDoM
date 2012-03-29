''' <summary>
''' This class is actually a stripped-down version of the actual Appointment class, which was generated using the 
''' Linq-to-SQL Designer (essentially a Linq ORM to the Appointments table in the db)
''' </summary>
''' <remarks>Obviously, you should use your own appointment object/classes, and change the code-behind in MonthView.xaml.vb to
''' support a List(Of T) where T is whatever the System.Type is for your appointment class.
''' </remarks>
''' <author>Kirk Davis, February 2009 (in like, 4 hours, and it shows!)</author>
<Serializable()> Class Appointment

    Private _AppointmentID As Integer
    Private _Subject As String
    Private _Location As String
    Private _Details As String
    Private _StartTime As System.Nullable(Of Date)
    Private _EndTime As System.Nullable(Of Date)
    Private _reccreatedDate As Date


    Public Sub New()
        MyBase.New()
    End Sub

    Public Property AppointmentID() As Integer
        Get
            Return Me._AppointmentID
        End Get
        Set(ByVal value As Integer)
            If ((Me._AppointmentID = value) = False) Then
                Me._AppointmentID = value
            End If
        End Set
    End Property

    Public Property Subject() As String
        Get
            Return Me._Subject
        End Get
        Set(ByVal value As String)
            If (String.Equals(Me._Subject, value) = False) Then
                Me._Subject = value
            End If
        End Set
    End Property

    Public Property Location() As String
        Get
            Return Me._Location
        End Get
        Set(ByVal value As String)
            If (String.Equals(Me._Location, value) = False) Then
                Me._Location = value
            End If
        End Set
    End Property

    Public Property Details() As String
        Get
            Return Me._Details
        End Get
        Set(ByVal value As String)
            If (String.Equals(Me._Details, value) = False) Then
                Me._Details = value
            End If
        End Set
    End Property

    Public Property StartTime() As System.Nullable(Of Date)
        Get
            Return Me._StartTime
        End Get
        Set(ByVal value As System.Nullable(Of Date))
            If (Me._StartTime.Equals(value) = False) Then
                Me._StartTime = value
            End If
        End Set
    End Property

    Public Property EndTime() As System.Nullable(Of Date)
        Get
            Return Me._EndTime
        End Get
        Set(ByVal value As System.Nullable(Of Date))
            If (Me._EndTime.Equals(value) = False) Then
                Me._EndTime = value
            End If
        End Set
    End Property

    Public Property reccreatedDate() As Date
        Get
            Return Me._reccreatedDate
        End Get
        Set(ByVal value As Date)
            If ((Me._reccreatedDate = value) = False) Then
                Me._reccreatedDate = value
            End If
        End Set
    End Property

End Class