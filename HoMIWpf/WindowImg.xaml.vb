
Public Class WindowImg
    Dim _ListImg As New List(Of HoMIDom.HoMIDom.ImageFile)
    Dim _oldstk As StackPanel = Nothing
    Public FileName As String


    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        Try
            DialogResult = True
        Catch ex As Exception
            MessageBox.Show("Erreur BtnOK_Click: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnCancel.Click
        DialogResult = False
    End Sub

    Public Sub New()

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        _ListImg = myservice.GetListOfImage
        Affiche()
    End Sub

    Private Sub Affiche(Optional ByVal Filtre As String = "")
        Try
            Wrp.Children.Clear()
            _ListImg = myService.GetListOfImage

            For i As Integer = 0 To _ListImg.Count - 1
                Dim stk As New StackPanel
                Dim img As New Image
                Dim lbl As New Label
                Dim x As HoMIDom.HoMIDom.ImageFile = _ListImg.Item(i)

                img.Height = 90
                img.Width = 90
                stk.Orientation = Orientation.Vertical
                stk.Margin = New Thickness(10)

                Try
                    img.Source = ConvertArrayToImage(myService.GetByteFromImage(x.Path))
                    img.ToolTip = x.FileName
                    lbl.Content = x.FileName
                    lbl.Foreground = New SolidColorBrush(Colors.White)
                    lbl.FontSize = 9
                    lbl.Width = 110
                    lbl.HorizontalContentAlignment = Windows.HorizontalAlignment.Center
                    stk.Tag = x.Path
                    AddHandler stk.MouseDown, AddressOf stk_MouseDown
                    stk.Children.Add(img)
                    stk.Children.Add(lbl)
                    Wrp.Children.Add(stk)
                Catch ex As Exception
                    MessageBox.Show("Erreur Affiche: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
                End Try
            Next
        Catch ex As Exception
            MessageBox.Show("Erreur: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub stk_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
        Try
            FileName = sender.tag

            If _oldstk IsNot Nothing Then
                _oldstk.Background = Brushes.DarkGray
            End If

            sender.Background = Brushes.LightGray
            _oldstk = sender
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub stk_MouseDown: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnUpload_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnUpload.Click
        Try
            ' Configure open file dialog box
            Dim dlg As New Microsoft.Win32.OpenFileDialog()
            dlg.FileName = "" ' Default file name

            ' Show open file dialog box
            Dim result As Boolean = dlg.ShowDialog()

            ' Process open file dialog box results
            If result = True Then
                ' Open document
                Dim array As Byte() = Nothing
                Dim filename As String = dlg.FileName
                Using fs As New IO.FileStream(dlg.FileName, IO.FileMode.Open, IO.FileAccess.Read)
                    Dim reader As New IO.BinaryReader(fs)
                    If reader IsNot Nothing Then
                        array = reader.ReadBytes(CInt(fs.Length))
                        reader.Close()
                        reader = Nothing
                    End If
                End Using
                Dim Namefile As String = dlg.SafeFileName
                dlg = Nothing
                Dim retour As String = myService.UploadFile(IdSrv, array, Namefile)
                If retour <> "0" Then
                    If retour = "99" Then
                        MessageBox.Show("Vous n'avez pas les droits pour accéder au serveur", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
                    Else
                        MessageBox.Show("Erreur: " & retour, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
                    End If
                Else
                    Affiche()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

End Class
