Imports HoMIDom.HoMIDom

Public Class uReleve

    Public Sub New(ByVal ListReleve As List(Of Historisation), ByVal Label As String)
        Try
            ' Cet appel est requis par le concepteur.
            InitializeComponent()
            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            DataGrid1.ItemsSource = ListReleve
            AddHandler DataGrid1.Loaded, AddressOf GridOk

            LblDev.Content = Label
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
End Class
