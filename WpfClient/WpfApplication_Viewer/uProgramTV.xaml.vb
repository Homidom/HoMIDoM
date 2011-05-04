Partial Public Class uProgramTV

    Private Sub uProgramTV_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Dim x As New uProgrammeTV
        If Grid1.Children.Count > 0 Then Grid1.Children.Clear()
        Grid1.Width = Me.Width
        Grid1.Height = Me.Height
        Grid1.Children.Add(x)
    End Sub
End Class
