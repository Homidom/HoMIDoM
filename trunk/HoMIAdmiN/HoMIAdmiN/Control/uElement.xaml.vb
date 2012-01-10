Public Class uElement
    Dim _Title As String = ""
    Dim _Image As String = ""
    Dim _ID As String = ""
    Dim _Select As Boolean

    Public Event Click(ByVal sender As Object)

    Public Property ID As String
        Get
            Return _ID
        End Get
        Set(ByVal value As String)
            _ID = value
        End Set
    End Property

    Public Property Title As String
        Get
            Return _Title
        End Get
        Set(ByVal value As String)
            _Title = value
            LabelElement.Content = _Title
        End Set
    End Property
    Public Property IsSelect As Boolean
        Get
            Return _Select
        End Get
        Set(ByVal value As Boolean)
            _Select = value
            If _Select = True Then
                Grid1.Background = Brushes.OrangeRed
            Else
                Grid1.Background = Brushes.DarkGray
            End If
        End Set
    End Property
    Public WriteOnly Property Image As String
        Set(ByVal value As String)
            If value.Length < 10 Then Exit Property
            MyImage.Source = ConvertArrayToImage(myservice.GetByteFromImage(value))
        End Set
    End Property

    Private Sub Grid1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Grid1.MouseDown
        RaiseEvent Click(Me)
        Grid1.Background = Brushes.OrangeRed
    End Sub

    Private Sub Grid1_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles Grid1.MouseMove
        Grid1.Background = Brushes.OrangeRed
    End Sub

    Private Sub Grid1_MouseLeave(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles Grid1.MouseLeave
        If _Select = False Then Grid1.Background = Brushes.DarkGray
    End Sub




End Class
