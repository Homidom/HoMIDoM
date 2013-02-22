Imports HoMIDom.HoMIDom

Public Class uReleve

    Dim _CurrentId As String = "" 'ID du device courant
    Dim _CurrentSource As String = "" 'Source des relevés VALUE, METEOACTUEL...
    Dim _CurrentValue As String = "" 'Valeur du relevé selectionne
    Dim _CurrentDateTime As String = "" 'DateTime  du relevé selectionne


    Public Sub New(ByVal ListReleve As List(Of Historisation), ByVal Label As String, ByVal DeviceID As String, ByVal Source As String)
        Try
            ' Cet appel est requis par le concepteur.
            InitializeComponent()
            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            DataGrid1.ItemsSource = ListReleve
            AddHandler DataGrid1.Loaded, AddressOf GridOk

            LblDev.Content = Label & "(" & ListReleve.Count & " relevés)"

            _CurrentId = DeviceID
            _CurrentSource = Source
            txtTime2.Text = DateTime.Now
            txtValue2.Text = ""
            'If ListReleve.Count > 0 Then
            '    _CurrentId = ListReleve(0).IdDevice
            'End If
        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'affichage du relevé: " & ex.ToString, "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub GridOk(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Try
            Do While DataGrid1.Columns.Count > 2
                DataGrid1.Columns.RemoveAt(0)
            Loop

        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'affichage du relevé GridOK: " & ex.ToString, "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnUpdate_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnUpdate.Click
        Try
            Dim retour As Integer = myService.UpdateHisto(IdSrv, _CurrentId, txtTime.Text, txtValue.Text, _CurrentDateTime, _CurrentValue, _CurrentSource)

            If retour <> 0 Then
                MessageBox.Show("Une erreur s'est produite, veuillez consulter le log pour en connaître la raison", "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
            Else
                '    DataGrid1.SelectedItem.Value = txtValue.Text
                '    DataGrid1.SelectedItem.DateTime = txtTime.Text
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur BtnUpdate_Click: " & ex.ToString, "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnDelete_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDelete.Click
        Try
            Dim retour As Integer = myService.DeleteHisto(IdSrv, _CurrentId, _CurrentDateTime, _CurrentValue, _CurrentSource)

            If retour <> 0 Then MessageBox.Show("Une erreur s'est produite, veuillez consulter le log pour en connaître la raison", "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
        Catch ex As Exception
            MessageBox.Show("Erreur BtnDelete_Click: " & ex.ToString, "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnAdd_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnAdd.Click
        Try
            If String.IsNullOrEmpty(txtTime2.Text) = True Then
                MessageBox.Show("Erreur veuillez saisir le champ DateTime", "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
            End If
            If String.IsNullOrEmpty(txtValue2.Text) = True Then
                MessageBox.Show("Erreur veuillez saisir le champ Valeur", "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
            End If

            Dim retour As Integer = myService.AddHisto(IdSrv, _CurrentId, txtTime2.Text, txtValue2.Text, _CurrentSource)


            If retour <> 0 Then MessageBox.Show("Une erreur s'est produite, veuillez consulter le log pour en connaître la raison", "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
        Catch ex As Exception
            MessageBox.Show("Erreur BtnAdd_Click: " & ex.ToString, "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub DataGrid1_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles DataGrid1.SelectionChanged
        _CurrentValue = DataGrid1.SelectedItem.Value
        _CurrentDateTime = DataGrid1.SelectedItem.DateTime
        txtTime.Text = DataGrid1.SelectedItem.DateTime
        txtValue.Text = DataGrid1.SelectedItem.Value

    End Sub
End Class
