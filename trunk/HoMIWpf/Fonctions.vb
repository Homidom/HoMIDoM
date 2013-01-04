Imports System.IO
Imports System.Windows.Media.Animation
Imports System.Net

Module Fonctions

    Public Function ConvertArrayToImage(ByVal value As Object) As BitmapImage
        Try
            Dim ImgSource As BitmapImage = Nothing
            Dim array As Byte() = TryCast(value, Byte())

            If array IsNot Nothing Then
                ImgSource = New BitmapImage()
                ImgSource.BeginInit()
                ImgSource.CacheOption = BitmapCacheOption.OnLoad
                ImgSource.CreateOptions = BitmapCreateOptions.DelayCreation
                ImgSource.StreamSource = New MemoryStream(array)
                array = Nothing
                ImgSource.EndInit()
                If ImgSource.CanFreeze Then ImgSource.Freeze()
            End If

            Return ImgSource
            ImgSource = Nothing
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub ConvertArrayToImage: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
            Return Nothing
        End Try
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

    Public Function UrlIsValid(ByVal url As String) As Boolean
        Dim is_valid As Boolean = False
        If url.ToLower().StartsWith("www.") Then url = _
            "http://" & url

        Dim web_response As HttpWebResponse = Nothing
        Try
            Dim web_request As HttpWebRequest = _
                HttpWebRequest.Create(url)
            web_response = _
                DirectCast(web_request.GetResponse(),  _
                HttpWebResponse)
            Return True
            web_request = Nothing
            web_response = Nothing
        Catch ex As Exception
            Return False
        Finally
            If Not (web_response Is Nothing) Then _
                web_response.Close()
        End Try
    End Function

    ''' <summary>
    ''' Vérifie si la valeur est un boolean
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsBoolean(ByVal value As Object) As Boolean
        Try
            Dim x As Boolean = value
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
End Module
