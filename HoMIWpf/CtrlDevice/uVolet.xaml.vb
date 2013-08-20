Imports System.Windows.Media.Animation

Public Class uVolet
    Dim _val As Integer = 0
    Dim _flagFirst As Boolean = True

    Public Property Value As Integer
        Get
            Return _val
        End Get
        Set(value As Integer)
            If value <= 0 Then value = 0
            If value >= 100 Then value = 100
            _val = value

            Dim re As RectangleGeometry = Image3.Clip
            Dim _deb As Integer = re.Rect.Height
            Dim _fin As Integer = ((100 - value) / 100) * Me.ActualHeight
            Dim _duree As Integer = 2

            Dim ra As RectAnimation = New RectAnimation()
            ra.From = New Rect(0, 0, Me.ActualWidth, _deb)
            ra.To = New Rect(0, 0, Me.ActualWidth, _fin)
            If _flagFirst Then
                ra.Duration = New Duration(TimeSpan.FromMilliseconds(5))
                _flagFirst = False
            Else
                ra.Duration = New Duration(TimeSpan.FromSeconds(_duree))
            End If

            Image3.Clip.BeginAnimation(RectangleGeometry.RectProperty, ra)
        End Set
    End Property
End Class
