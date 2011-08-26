Imports System.IO
Imports System.Windows.Media.Animation

Module Fonctions
    Public Sub WriteLog(ByVal Message As String, Optional ByVal Group As String = "", Optional ByVal User As String = "")
        If _Debug = False Then Exit Sub
        Dim Fichier As FileInfo
        'Vérifie si le fichier log existe sinon le crée
        If File.Exists("c:\ehome\log\ehome.log") = False Then
            File.Create("c:\ehome\log\ehome.log")
        End If
        Fichier = New FileInfo("c:\ehome\log\ehome.log")
        'Vérifie si le fichier fait 10Mo si oui le supprime
        If Fichier.Length > 10000000 Then
            File.Delete("c:\ehome\log\ehome.log")
        End If
        'Ecrire le log
        Dim SW As New StreamWriter("c:\ehome\log\ehome.log", True)
        SW.WriteLine(Now.ToString & " - " & Message & " - " & Group & " - " & User)
        SW.Close()
    End Sub


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
