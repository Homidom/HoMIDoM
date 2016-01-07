Imports System.Data
Imports System.IO
Imports System.Collections.ObjectModel
Imports HoMIDom.HoMIDom
Imports HoMIDom.HoMIDom.Api

Public Class uNewDevice
    Public Event CloseMe(ByVal MyObject As Object)
    Public Event CreateNewDevice(ByVal MyObject As Object)
    Dim _list As New List(Of NewDevice)
    Dim _Listdevices As New List(Of TemplateDevice)

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

            'recupération de la liste des composants existants
            _Listdevices = myService.GetAllDevices(IdSrv)

        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur lors sur la fonction New de uNewDevice: " & ex.ToString, "ERREUR", "")
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
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uNewDevice GridOK: " & ex.ToString, "Erreur Admin", "")
        End Try
    End Sub


    Private Sub txtDriver_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles txtDriver.TextChanged
        Try
            If String.IsNullOrEmpty(txtDriver.Text) = False Then
                Dim x As HoMIDom.HoMIDom.TemplateDriver = myService.ReturnDriverByID(IdSrv, txtDriver.Text)

                If x IsNot Nothing Then LblDriver.Text = x.Nom
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur txtDriver_TextChanged: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Sub BtnDelete_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDelete.Click
        Try
            If String.IsNullOrEmpty(txtID.Text) = False Then
                Dim retour As Integer = myService.DeleteNewDevice(IdSrv, txtID.Text)

                If retour <> 0 Then
                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Une erreur s'est produite, veuillez consulter le log pour en connaître la raison", "Erreur Admin", "")
                Else
                    Refresh_Grid()

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
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur BtnDelete_Click: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Sub BtnUpdate_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnUpdate.Click
        Try
            If String.IsNullOrEmpty(txtName.Text) Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Veuillez saisir un nom pour ce nouveau composant", "Erreur Admin", "")
                txtName.Undo()
            Else
                If String.IsNullOrEmpty(txtID.Text) = False Then
                    Dim x As NewDevice = myService.ReturnNewDevice(txtID.Text)

                    If x IsNot Nothing Then
                        x.Name = txtName.Text
                        x.Type = CbType.SelectedValue
                        x.Ignore = ChkIgnore.IsChecked
                        myService.SaveNewDevice(x)
                    End If
                End If
                Refresh_Grid(CheckBox1.IsChecked)
                FlagChange = True
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur BtnUpdate_Click: " & ex.ToString, "ERREUR", "")
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
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur Refresh_Grid: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Sub CheckBox1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CheckBox1.Click
        Refresh_Grid(CheckBox1.IsChecked)
    End Sub

    Private Sub BtnCreate_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnCreate.Click
        Try
            If String.IsNullOrEmpty(txtID.Text) Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Veuillez sélectionner un composant dans la grille!", "ERREUR", "")
                Exit Sub
            End If

            Dim x As New NewDevice
            x.ID = txtID.Text
            x.IdDriver = txtDriver.Text
            'enleve les caratère spéciaux
            Dim Str As String = ""
            For i = 0 To Len(txtName.Text) - 1
                Select Case txtName.Text(i)
                    Case "a" To "z" : Str += txtName.Text(i)
                    Case "A" To "Z" : Str += txtName.Text(i)
                    Case "0" To "9" : Str += txtName.Text(i)
                    Case " " : Str += " "
                    Case Else
                        Str += " "
                End Select
            Next
            x.Name = Trim(Str)
            x.Adresse1 = txtAdresse1.Text
            x.Adresse2 = txtAdresse2.Text
            x.Type = CbType.Text
            NewDevice = x
            RaiseEvent CreateNewDevice(Me)
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur BtnCreate_Click: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    'Mettre à jour les champs adresse1 et adresse2 du composant selectionné avec le nouveau composant et le supprimer
    Private Sub BtnUpdateComposant_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles BtnUpdateComposant.Click
        Try
            Dim retour As String = ""

            'verif des champs
            If String.IsNullOrEmpty(txtID.Text) Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Veuillez sélectionner un composant dans la grille!", "ERREUR", "")
                Exit Sub
            End If
            If String.IsNullOrEmpty(txtAdresse1.Text) = True Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "L'adresse 1 du composant est obligatoire !!", "Erreur", "")
                Exit Sub
            End If

            'mise à jour du composant
            Dim x As HoMIDom.HoMIDom.TemplateDevice = Nothing
            x = myService.ReturnDeviceByID(IdSrv, CbComposants.SelectedItem.ID)
            If x IsNot Nothing Then 'on a trouvé le device
                retour = myService.SaveDevice(IdSrv, x.ID, x.Name, txtAdresse1.Text, x.Enable, x.Solo, x.DriverID, x.Type, x.Refresh, x.IsHisto, x.RefreshHisto, x.Purge, x.MoyJour, x.MoyHeure, txtAdresse2.Text, x.Picture, x.Modele, x.Description, x.LastChangeDuree, x.LastEtat, x.Correction, x.Formatage, x.Precision, x.ValueMax, x.ValueMin, x.ValueDef, x.Commandes, x.Unit, x.Puissance, x.AllValue)

                'suppression du nouveau composant
                Dim retour2 As Integer = myService.DeleteNewDevice(IdSrv, txtID.Text)
                If retour2 <> 0 Then
                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Une erreur s'est produite, veuillez consulter le log pour en connaître la raison", "Erreur Admin", "")
                Else
                    Refresh_Grid()
                    If DGW.Columns.Count > 2 Then
                        Dim y As DataGridColumn = DGW.Columns(0)
                        y.Width = 0
                        y.Visibility = Windows.Visibility.Collapsed
                        Dim z As DataGridColumn = DGW.Columns(1)
                        z.Width = 0
                        z.Visibility = Windows.Visibility.Collapsed
                    End If
                    FlagChange = True
                End If
            Else
                MessageBox.Show("Le composant selectionné n'a pas été trouvé sur le serveur")
            End If


        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur BtnUpdateComposant_Click: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()

        _list.Clear()
    End Sub

    'quand la selection dans la liste change, on met à jour la liste des composants existants pouvant correspondre (driver, type, adresse1 et adresse2)
    Private Sub DGW_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles DGW.SelectionChanged
        Try
            'CbComposants.ItemsSource = myService.GetAllDevices(IdSrv)
            'CbComposants.DisplayMemberPath = "Name"

            Dim _listdevicestemp As New List(Of TemplateDevice)
            For Each Device As TemplateDevice In _Listdevices
                If Device.DriverID = txtDriver.Text Then
                    _listdevicestemp.Add(Device)
                End If
            Next
            _listdevicestemp.Sort(AddressOf sortDevice)

            


            CbComposants.ItemsSource = _listdevicestemp
            CbComposants.DisplayMemberPath = "Name"
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur DGW_SelectionChanged: " & ex.ToString, "ERREUR", "")
        End Try

    End Sub
    Private Function sortDevice(ByVal x As TemplateDevice, ByVal y As TemplateDevice) As Integer
        Try
            Return x.Name.CompareTo(y.Name)
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur sortDevice: " & ex.ToString, "ERREUR", "")
            Return 0
        End Try
    End Function

End Class
