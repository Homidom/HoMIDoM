
Public Class WindowImg
    Dim _ListImg As New List(Of HoMIDom.HoMIDom.ImageFile)
    Public FileName As String

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        If ListBoxImg.SelectedIndex > -1 Then
            Dim y As StackPanel = ListBoxImg.Items(ListBoxImg.SelectedIndex)
            FileName = y.Tag
        End If
        DialogResult = True
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
        If _ListImg IsNot Nothing Then
            For i As Integer = 0 To _ListImg.Count - 1
                Dim stk As New StackPanel
                Dim img As New Image
                Dim lbl As New Label
                Dim x As HoMIDom.HoMIDom.ImageFile = _ListImg.Item(i)

                img.Height = 45
                img.Width = 45
                stk.Orientation = Orientation.Horizontal

                Try
                    img.Source = ConvertArrayToImage(myService.GetByteFromImage(x.Path))
                    lbl.Content = x.Path
                    lbl.Foreground = New SolidColorBrush(Colors.White)
                    stk.Tag = x.Path
                    stk.Children.Add(img)
                    stk.Children.Add(lbl)
                    ListBoxImg.Items.Add(stk)

                Catch ex As Exception
                    AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur WindowImg.Affiche: " & ex.Message, "Erreur", "WindowImg.Affiche")
                End Try
            Next
        End If
    End Sub


End Class
