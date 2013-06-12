
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
                borderelemnt.Background = Brushes.OrangeRed
            Else
                borderelemnt.Background = Brushes.Gray
            End If
        End Set
    End Property

    Public WriteOnly Property Image As String
        Set(ByVal value As String)
            If value.Length < 10 Then Exit Property
            MyImage.Source = ConvertArrayToImage(myService.GetByteFromImage(value))
        End Set
    End Property

    Private Sub Grid1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Grid1.MouseDown
        RaiseEvent Click(Me)
        'borderelemnt.Background = Brushes.DarkGray
    End Sub

    Private Sub Grid1_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles Grid1.MouseMove
        If _Select = False Then borderelemnt.Background = Brushes.DarkGray
        'Try
        '    Dim brush As New SolidColorBrush(Colors.DarkGray)
        '    Grid1.Background = brush

        '    Dim myDoubleAnimation As ColorAnimation = New ColorAnimation()
        '    myDoubleAnimation.From = Colors.DarkGray
        '    myDoubleAnimation.To = Colors.OrangeRed
        '    myDoubleAnimation.Duration = New Duration(TimeSpan.FromSeconds(1))

        '    Dim myStoryboard As Storyboard
        '    myStoryboard = New Storyboard()
        '    myStoryboard.Children.Add(myDoubleAnimation)
        '    '        AddHandler myStoryboard.Completed, AddressOf StoryBoardFinish

        '    Storyboard.SetTarget(myDoubleAnimation, brush)
        '    Storyboard.SetTargetProperty(myDoubleAnimation, New PropertyPath(SolidColorBrush.ColorProperty))
        '    myStoryboard.Begin()
        'Catch ex As Exception
        '    AfficheMessageAndLog (HoMIDom.HoMIDom.Server.TypeLog.ERREUR,"Erreur: " & ex.ToString, "Erreur Admin", MessageBoxButton.OK)
        'End Try
    End Sub

    Private Sub Grid1_MouseLeave(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles Grid1.MouseLeave
        If _Select = False Then borderelemnt.Background = Brushes.Gray
    End Sub




End Class
