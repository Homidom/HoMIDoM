Imports HoMIDom.HoMIDom

Public Class uReleve
    Dim _listhisto As New List(Of Historisation)
    Public Event CloseMe(ByVal MyObject As Object)

    Public Sub New(ByVal ListReleve)

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        _listhisto = ListReleve
        DataGrid1.ItemsSource = _listhisto
        AddHandler DataGrid1.Loaded, AddressOf GridOK
    End Sub

    Private Sub GridOk(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        DataGrid1.Columns.RemoveAt(0)
        DataGrid1.Columns.RemoveAt(0)
        DataGrid1.Columns.RemoveAt(0)
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me)
    End Sub
End Class
