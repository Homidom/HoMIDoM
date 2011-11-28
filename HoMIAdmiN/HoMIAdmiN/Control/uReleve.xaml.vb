Imports HoMIDom.HoMIDom

Public Class uReleve
    Dim _listhisto As New List(Of Historisation)
    Public Event CloseMe(ByVal MyObject As Object)

    Public Sub New(ByVal ListReleve As Object, ByVal Label As String)
        Try
            ' Cet appel est requis par le concepteur.
            InitializeComponent()

            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            _listhisto = ListReleve
            DataGrid1.ItemsSource = _listhisto
            AddHandler DataGrid1.Loaded, AddressOf GridOk

            LblDev.Content = Label
        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'affichage du relevé: " & ex.ToString, "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub GridOk(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Try
            If DataGrid1.Columns.Count > 0 Then DataGrid1.Columns.RemoveAt(0)
            If DataGrid1.Columns.Count > 0 Then DataGrid1.Columns.RemoveAt(0)
            If DataGrid1.Columns.Count > 0 Then DataGrid1.Columns.RemoveAt(0)
        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'affichage du relevé GridOK: " & ex.ToString, "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me)
    End Sub
End Class
