Imports System.Data
Imports System.IO
Imports System.Collections.ObjectModel
Imports HoMIDom.HoMIDom

Public Class uNewDevice
    Public Event CloseMe(ByVal MyObject As Object)
    Public Event CreateNewDevice(ByVal MyObject As Object)
    Dim _list As New List(Of NewDevice)

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me)
    End Sub

    Public Sub New()
        Try

            ' Cet appel est requis par le Concepteur Windows Form.
            InitializeComponent()

            'Liste les type de devices dans le combo
            For Each value As HoMIDom.HoMIDom.Device.ListeDevices In [Enum].GetValues(GetType(HoMIDom.HoMIDom.Device.ListeDevices))
                CbType.Items.Add(value.ToString)
            Next

            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            Refresh_Grid(CheckBox1.IsChecked)
            AddHandler DGW.Loaded, AddressOf GridOk


        Catch ex As Exception
            MessageBox.Show("Erreur lors sur la fonction New de uNewDevice: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try

    End Sub

    Private Sub GridOk(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Try
            If DGW.Columns.Count > 2 Then
                Dim x As DataGridColumn = DGW.Columns(0)
                x.Width = 0
                x.Visibility = Windows.Visibility.Collapsed
                Dim y As DataGridColumn = DGW.Columns(1)
                y.Width = 0
                y.Visibility = Windows.Visibility.Collapsed
            End If

        Catch ex As Exception
            MessageBox.Show("Erreur uNewDevice GridOK: " & ex.ToString, "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub


    Private Sub txtDriver_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles txtDriver.TextChanged
        Try
            If String.IsNullOrEmpty(txtDriver.Text) = False Then
                Dim x As HoMIDom.HoMIDom.TemplateDriver = myService.ReturnDriverByID(IdSrv, txtDriver.Text)

                If x IsNot Nothing Then LblDriver.Text = x.Nom
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur txtDriver_TextChanged: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnDelete_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDelete.Click
        Try
            If String.IsNullOrEmpty(txtID.Text) = False Then
                Dim retour As Integer = myService.DeleteNewDevice(IdSrv, txtID.Text)

                If retour <> 0 Then
                    MessageBox.Show("Une erreur s'est produite, veuillez consulter le log pour en connaître la raison", "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
                Else
                    Refresh_Grid

                    If DGW.Columns.Count > 2 Then
                        Dim x As DataGridColumn = DGW.Columns(0)
                        x.Width = 0
                        x.Visibility = Windows.Visibility.Collapsed
                        Dim y As DataGridColumn = DGW.Columns(1)
                        y.Width = 0
                        y.Visibility = Windows.Visibility.Collapsed
                    End If
                    FlagChange = True
                End If

            End If
        Catch ex As Exception

        End Try
    End Sub


    Private Sub BtnUpdate_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnUpdate.Click
        Try
            If String.IsNullOrEmpty(txtName.Text) Then
                MessageBox.Show("Veuillez saisir un nom pour ce nouveau composant", "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
                txtName.Undo()
            Else
                If String.IsNullOrEmpty(txtID.Text) = False Then
                    Dim x As NewDevice = myService.ReturnNewDevice(txtID.Text)

                    If x IsNot Nothing Then
                        x.Name = txtName.Text
                        x.Type = CbType.SelectedIndex
                        x.Ignore = ChkIgnore.IsChecked
                        myService.SaveNewDevice(x)
                    End If
                End If
                Refresh_Grid(CheckBox1.IsChecked)
                FlagChange = True
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur BtnUpdate_Click: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub Refresh_Grid(Optional ByVal Tous As Boolean = True)
        Try
            _list.Clear()

            If Tous = False Then
                For Each _newdev As NewDevice In myService.GetAllNewDevice
                    If _newdev.Ignore = False Then
                        _list.Add(_newdev)
                    End If
                Next
            Else
                _list = myService.GetAllNewDevice
            End If

            DGW.ItemsSource = _list
            DGW.Items.Refresh()

            GridOk(Me, Nothing)
        Catch ex As Exception
            MessageBox.Show("Erreur Refresh_Grid: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub CheckBox1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CheckBox1.Click
        Refresh_Grid(CheckBox1.IsChecked)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()

        _list.Clear()
    End Sub

    Private Sub BtnCreate_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnCreate.Click
        Try
            If String.IsNullOrEmpty(txtID.Text) Then
                MessageBox.Show("Veuillez sélectionner un composant dans la grille!", "ERREUR", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Exit Sub
            End If

            Dim x As New NewDevice
            x.ID = txtID.Text
            x.IdDriver = txtDriver.Text
            x.Name = txtName.Text
            x.Adresse1 = txtAdresse1.Text
            x.Adresse2 = txtAdresse2.Text
            x.Type = CbType.SelectedIndex
            NewDevice = x
            RaiseEvent CreateNewDevice(Me)
        Catch ex As Exception
            MessageBox.Show("Erreur BtnCreate_Click: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class
