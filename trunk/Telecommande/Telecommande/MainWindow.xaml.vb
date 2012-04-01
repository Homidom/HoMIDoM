Class MainWindow

    Private Sub button_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles button.Click
        Dim img1 As New Image
        Dim bi1 As New BitmapImage
        Static Dim state As Boolean

        bi1.BeginInit()
        bi1.UriSource = New Uri("\Images\1.png", UriKind.Relative)
        bi1.EndInit()
        img1.Source = bi1

        If state = False Then
            grid.SetColumn(img1, 2)
            grid.SetRow(img1, 1)
            grid_Telecommande.Children.Add(img1)
            state = True
        Else
            Me.grid_Telecommande.Children.Remove(img1)
            state = False
        End If
        Me.Content = grid_Telecommande
    End Sub
End Class