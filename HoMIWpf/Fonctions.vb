Imports System.IO
Imports System.Windows.Media.Animation

Module Fonctions

    Public Function ConvertArrayToImage(ByVal value As Object) As Object
        Dim ImgSource As BitmapImage = Nothing
        Dim array As Byte() = TryCast(value, Byte())

        If array IsNot Nothing Then
            ImgSource = New BitmapImage()
            ImgSource.BeginInit()
            ImgSource.StreamSource = New MemoryStream(array)
            ImgSource.EndInit()
        End If
        Return ImgSource
    End Function


    Public Sub ScrollToPosition(ByVal ScrollViewer As UAniScrollViewer.AniScrollViewer, ByVal x As Double, ByVal y As Double, ByVal Duree As Double)
        Dim vertAnim As New DoubleAnimation()
        vertAnim.From = ScrollViewer.VerticalOffset
        vertAnim.[To] = y
        vertAnim.DecelerationRatio = 0.2
        vertAnim.Duration = New Duration(TimeSpan.FromMilliseconds(250))

        Dim horzAnim As New DoubleAnimation()
        horzAnim.From = ScrollViewer.HorizontalOffset
        horzAnim.[To] = x
        horzAnim.DecelerationRatio = 0.99
        horzAnim.Duration = New Duration(TimeSpan.FromMilliseconds(Duree))

        Dim sb As New Storyboard()
        'sb.Children.Add(vertAnim)
        sb.Children.Add(horzAnim)

        Storyboard.SetTarget(vertAnim, ScrollViewer)
        'Storyboard.SetTargetProperty(vertAnim, New PropertyPath(UAniScrollViewer.AniScrollViewer.CurrentVerticalOffsetProperty))
        Storyboard.SetTarget(horzAnim, ScrollViewer)
        Storyboard.SetTargetProperty(horzAnim, New PropertyPath(UAniScrollViewer.AniScrollViewer.CurrentHorizontalOffsetProperty))

        sb.Begin()

    End Sub
End Module
