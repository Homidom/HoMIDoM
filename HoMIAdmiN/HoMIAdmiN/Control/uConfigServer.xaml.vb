Partial Public Class uConfigServer
    Public Event CloseMe(ByVal MyObject As Object)
    Dim Flag As Boolean 'True si nouveau serveur

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        If Window1.IsConnect = True Then
            If IsNumeric(TxtSOAP.Text) = False Or TxtSOAP.Text = "" Then
                MessageBox.Show("Le port SOAP est erroné !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
                Exit Sub
            End If
            If TxtSOAP.Text <> Window1.myService.GetPortSOAP Then
                MessageBox.Show("Vous avez modifié le port SOAP, n'oubliez pas:" & vbCrLf & "- D'enregistrer la configuration pour qu'elle soit prise en compte au prochain démarrage" & vbCrLf & "- De redémarrer le service", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Window1.myService.SetPortSOAP(TxtSOAP.Text)
            End If
            Window1.myService.SetLongitude(CDbl(TxtLong.Text.Replace(".", ",")))
            Window1.myService.SetLatitude(CDbl(TxtLat.Text.Replace(".", ",")))
            Window1.myService.SetHeureCorrectionLever(CInt(HCL.Text))
            Window1.myService.SetHeureCorrectionCoucher(CInt(HCC.Text))
        End If
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
            TxtSOAP.Text = Window1.myService.GetPortSOAP
            TxtLat.Text = Window1.myService.GetLatitude
            TxtLong.Text = Window1.myService.GetLongitude
            HCL.Text = Window1.myService.GetHeureCorrectionLever
            HCC.Text = Window1.myService.GetHeureCorrectionCoucher
        End If

        RefreshList()
        If ListBox1.Items.Count = 0 Then Flag = True
    End Sub

    Private Sub BtnNewServer_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnNewServer.Click
        TxtAdr.Text = ""
        CheckBox1.IsChecked = False
        TxtPort.Text = ""
        Flag = True
    End Sub

    Private Sub BtnSaveServer_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnSaveServer.Click
        If TxtAdr.Text = "" Or TxtPort.Text = "" Or IsNumeric(TxtPort.Text) = False Then
            MessageBox.Show("Veuillez saisir une adresse et un numéro de port!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        Else
            If Flag = True Then
                If CheckBox1.IsChecked = True Then
                    If Window1.ListServer.Count = 0 Then
                        MessageBox.Show("Ceci est votre première adresse donc elle sera mis par défaut!", "Message", MessageBoxButton.OK, MessageBoxImage.Information)
                        Dim x As New ClServer
                        x.Adresse = TxtAdr.Text
                        x.Defaut = True
                        x.Port = TxtPort.Text
                        Window1.ListServer.Add(x)
                    Else
                        For i As Integer = 0 To Window1.ListServer.Count - 1
                            If Window1.ListServer.Item(i).Defaut = True Then
                                If MessageBox.Show("Etes vous sur de mettre cette adresse par défaut à la place de: " & Window1.ListServer.Item(i).Adresse, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.Yes Then
                                    Window1.ListServer.Item(i).Defaut = False
                                    Dim x As New ClServer
                                    x.Adresse = TxtAdr.Text
                                    x.Defaut = CheckBox1.IsChecked
                                    x.Port = TxtPort.Text
                                    Window1.ListServer.Add(x)
                                End If
                            End If
                        Next
                    End If
                Else
                    Dim x As New ClServer
                    x.Adresse = TxtAdr.Text
                    x.Defaut = CheckBox1.IsChecked
                    x.Port = TxtPort.Text
                    Window1.ListServer.Add(x)
                End If
            Else
                If ListBox1.SelectedIndex < 0 Then
                    MessageBox.Show("Veuillez sélectionner un élément dans la liste!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                    Exit Sub
                End If
                Window1.ListServer.Item(ListBox1.SelectedIndex).Adresse = TxtAdr.Text
                Window1.ListServer.Item(ListBox1.SelectedIndex).Defaut = CheckBox1.IsChecked
                Window1.ListServer.Item(ListBox1.SelectedIndex).Port = TxtPort.Text
                End If
        End If
        RefreshList()
        Flag = False
    End Sub

    Private Sub RefreshList()
        ListBox1.Items.Clear()
        For i As Integer = 0 To Window1.ListServer.Count - 1
            Dim a As String = Window1.ListServer.Item(i).Adresse & " : " & Window1.ListServer.Item(i).Port
            ListBox1.Items.Add(a)
        Next
    End Sub

    Private Sub BtnDelServer_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDelServer.Click
        If ListBox1.SelectedIndex < 0 Then
            MessageBox.Show("Veuillez un élément dans la liste!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        Else
            Window1.ListServer.RemoveAt(ListBox1.SelectedIndex)
            TxtAdr.Text = ""
            TxtPort.Text = ""
            CheckBox1.IsChecked = False
            RefreshList()
        End If
    End Sub

    Private Sub ListBox1_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles ListBox1.SelectionChanged
        If ListBox1.SelectedIndex >= 0 Then
            TxtAdr.Text = Window1.ListServer.Item(ListBox1.SelectedIndex).Adresse
            TxtPort.Text = Window1.ListServer.Item(ListBox1.SelectedIndex).Port
            CheckBox1.IsChecked = Window1.ListServer.Item(ListBox1.SelectedIndex).Defaut
        End If
    End Sub
End Class
