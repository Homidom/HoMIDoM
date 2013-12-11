Public Class UWidgetLog

    Dim _datetime As String
    Dim _typesource As String
    Dim _source As String
    Dim _fonction As String
    Dim _message As String

    Public Property DateTime As String
        Get
            Return _datetime
        End Get
        Set(value As String)
            _datetime = value
            LblTime.Content = value
        End Set
    End Property

    Public Property Source As String
        Get
            Return _source
        End Get
        Set(value As String)
            _source = value
            LblSource.Content = value
        End Set
    End Property

    Public Property TypeSource As String
        Get
            Return _typesource
        End Get
        Set(value As String)
            _typesource = value
            Select Case value.ToUpper
                Case "INFO"
                    ImageIcon.Source = New BitmapImage(New Uri("/HoMIWpF;component/Images/info.png", UriKind.RelativeOrAbsolute))
                Case "ACTION"
                    ImageIcon.Source = New BitmapImage(New Uri("/HoMIWpF;component/Images/info.png", UriKind.RelativeOrAbsolute))
                Case "MESSAGE"
                    ImageIcon.Source = New BitmapImage(New Uri("/HoMIWpF;component/Images/bulle.png", UriKind.RelativeOrAbsolute))
                Case "VALEUR_CHANGE"
                    ImageIcon.Source = New BitmapImage(New Uri("/HoMIWpF;component/Images/Value_Change.png", UriKind.RelativeOrAbsolute))
                Case "VALEUR_INCHANGE"
                    ImageIcon.Source = New BitmapImage(New Uri("/HoMIWpF;component/Images/Value_NotChange.png", UriKind.RelativeOrAbsolute))
                Case "VALEUR_INCHANGE_PRECISION"
                    ImageIcon.Source = New BitmapImage(New Uri("/HoMIWpF;component/Images/Value_NotChange.png", UriKind.RelativeOrAbsolute))
                Case "VALEUR_INCHANGE_LASTETAT"
                    ImageIcon.Source = New BitmapImage(New Uri("/HoMIWpF;component/Images/Value_NotChange.png", UriKind.RelativeOrAbsolute))
                Case "ERREUR"
                    ImageIcon.Source = New BitmapImage(New Uri("/HoMIWpF;component/Images/error.png", UriKind.RelativeOrAbsolute))
                Case "ERREUR_CRITIQUE"
                    ImageIcon.Source = New BitmapImage(New Uri("/HoMIWpF;component/Images/error.png", UriKind.RelativeOrAbsolute))
                Case "DEBUG"
                    ImageIcon.Source = New BitmapImage(New Uri("/HoMIWpF;component/Images/debug.png", UriKind.RelativeOrAbsolute))
                Case Else
                    ImageIcon.Source = New BitmapImage(New Uri("/HoMIWpF;component/Images/info.png", UriKind.RelativeOrAbsolute))
            End Select
        End Set
    End Property

    Public Property Fonction As String
        Get
            Return _fonction
        End Get
        Set(value As String)
            _fonction = value
            LblMessage.Text = value & ": " & _message
        End Set
    End Property

    Public Property Message As String
        Get
            Return _message
        End Get
        Set(value As String)
            _message = value
            LblMessage.Text = value & ": " & _message
        End Set
    End Property

    Public Sub New(ByVal TypLog As String, ByVal Source As String, ByVal Fonction As String, ByVal Message As String, DateTime As String)

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        Me.TypeSource = TypLog
        Me.Source = Source
        Me.Fonction = Fonction
        Me.Message = Message
        Me.DateTime = DateTime
    End Sub
End Class
