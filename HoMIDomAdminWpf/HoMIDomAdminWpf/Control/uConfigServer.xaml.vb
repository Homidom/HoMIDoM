Partial Public Class uConfigServer
    Public Event CloseMe(ByVal MyObject As Object)
    Dim Flag As Boolean 'True si nouveau serveur

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        Window1.myService.SetLongitude(CDbl(TxtLong.Text.Replace(".", ",")))
        Window1.myService.SetLatitude(CDbl(TxtLat.Text.Replace(".", ",")))
        Window1.myService.SetHeureCorrectionLever(CInt(HCL.Text))
        Window1.myService.SetHeureCorrectionCoucher(CInt(HCC.Text))
        RaiseEvent CloseMe(Me)
    End Sub

    Private Sub BtnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnCancel.Click
        RaiseEvent CloseMe(Me)
    End Sub

    Public Sub New()

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        If Window1.IsConnect = True Then
            TxtLat.Text = Window1.myService.GetLatitude
            TxtLong.Text = Window1.myService.GetLongitude
            HCL.Text = Window1.myService.GetHeureCorrectionLever
            HCC.Text = Window1.myService.GetHeureCorrectionCoucher
        End If

        For i As Integer = 0 To Window1.ListServer.Count - 1
            CbAdress.Items.Add(Window1.ListServer.Item(i).Adresse)
        Next

    End Sub

    Private Sub BtnNewServer_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnNewServer.Click
        CbAdress.Text = ""
        CheckBox1.IsChecked = False
    End Sub

    Private Sub BtnSaveServer_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnSaveServer.Click
        If CbAdress.Text = "" Then
            MessageBox.Show("Veuillez saisir une adresse!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        Else
            If CheckBox1.IsChecked = True Then
                If Window1.ListServer.Count = 0 Then
                    MessageBox.Show("Ceci est votre première adresse donc elle sera mis par défaut!", "Message", MessageBoxButton.OK, MessageBoxImage.Information)
                    Dim x As New ClServer
                    x.Adresse = CbAdress.Text
                    x.Defaut = True
                    Window1.ListServer.Add(x)
                Else
                    For i As Integer = 0 To Window1.ListServer.Count - 1
                        If Window1.ListServer.Item(i).Defaut = True Then
                            If MessageBox.Show("Etes vous sur de mettre cette adresse par défaut à la place de: " & Window1.ListServer.Item(i).Adresse, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.Yes Then
                                Window1.ListServer.Item(i).Defaut = False
                                Dim x As New ClServer
                                x.Adresse = CbAdress.Text
                                x.Defaut = CheckBox1.IsChecked
                                Window1.ListServer.Add(x)
                            End If
                        End If
                    Next
                End If
            Else
                Dim x As New ClServer
                x.Adresse = CbAdress.Text
                x.Defaut = CheckBox1.IsChecked
                Window1.ListServer.Add(x)
            End If

        End If
    End Sub

    Private Sub BtnDelServer_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDelServer.Click
        If CbAdress.Text = "" Then
            MessageBox.Show("Veuillez saisir une adresse!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        Else
            For i As Integer = 0 To Window1.ListServer.Count - 1
                If Window1.ListServer.Item(i).Adresse = CbAdress.Text Then
                    Window1.ListServer.RemoveAt(i)
                End If
            Next
        End If
    End Sub


    Private Sub CbAdress_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles CbAdress.SelectionChanged
        If CbAdress.SelectedIndex < 0 Then Exit Sub
        CheckBox1.IsChecked = Window1.ListServer.Item(CbAdress.SelectedIndex).Defaut
    End Sub
End Class
