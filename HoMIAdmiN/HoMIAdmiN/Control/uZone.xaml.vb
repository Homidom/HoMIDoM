Imports HoMIDom.HoMIDom
Imports System.IO

Partial Public Class uZone
    '--- Variables ------------------
    Public Event CloseMe(ByVal MyObject As Object)
    Dim _Action As EAction 'Définit si modif ou création d'un device
    Dim _ZoneId As String 'Id de la zone à modifier
    Dim FlagNewCmd As Boolean
    Dim _ListIdSelect As New List(Of Zone.Element_Zone)

    Public Enum EAction
        Nouveau
        Modifier
    End Enum

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        Try
            If TxtName.Text = "" Then
                MessageBox.Show("Le nom de la zone est obligatoire!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Exit Sub
            End If

            If _ZoneId = "" Then
                For i As Integer = 0 To Window1.myService.GetAllZones(IdSrv).Count - 1
                    If Window1.myService.GetAllZones(IdSrv).Item(i).Name = TxtName.Text Then
                        MessageBox.Show("Le nom de cette zone est déjà utilisée, veuillez en choisir un autre!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                        Exit Sub
                    End If
                Next
            End If

            Window1.myService.SaveZone(IdSrv, _ZoneId, TxtName.Text, _ListIdSelect, ImgIcon.Tag.ToString, ImgZone.Tag.ToString)
            RaiseEvent CloseMe(Me)
        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'enregistrement de la zone, message: " & ex.ToString, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me)
    End Sub

    Public Sub New(ByVal Action As EAction, ByVal ZoneId As String)

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ImgIcon.Tag = ""
        ImgZone.Tag = ""

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        If Action = EAction.Nouveau Then 'Nouvelle Zone

        Else 'Modifier zone
            Dim x As Zone = Window1.myService.ReturnZoneByID(IdSrv, ZoneId)
            Dim _list As New List(Of Integer)

            _ZoneId = ZoneId
            If x IsNot Nothing Then
                TxtName.Text = x.Name
                For j As Integer = 0 To x.ListElement.Count - 1
                    _ListIdSelect.Add(x.ListElement.Item(j))
                Next

                If x.Image <> "" And x.Image <> " " Then
                    ImgZone.Source = ConvertArrayToImage(Window1.myService.GetByteFromImage(x.Image))
                    ImgZone.Tag = x.Image
                End If

                If x.Icon <> "" And x.Icon <> "" And x.Icon <> " " Then
                    ImgIcon.Source = ConvertArrayToImage(Window1.myService.GetByteFromImage(x.Icon))
                    ImgIcon.Tag = x.Icon
                End If
            End If
        End If

        RefreshLists()
    End Sub

    Private Sub RefreshLists()
        ListBxDevice.Items.Clear()

        For i As Integer = 0 To _ListIdSelect.Count - 1
            If Window1.myService.ReturnDeviceByID(IdSrv, _ListIdSelect.Item(i).ElementID) IsNot Nothing Then
                ListBxDevice.Items.Add(Window1.myService.ReturnDeviceByID(IdSrv, _ListIdSelect.Item(i).ElementID).Name)
            End If
            If Window1.myService.ReturnZoneByID(IdSrv, _ListIdSelect.Item(i).ElementID) IsNot Nothing Then
                ListBxDevice.Items.Add(Window1.myService.ReturnZoneByID(IdSrv, _ListIdSelect.Item(i).ElementID).Name)
            End If
            If Window1.myService.ReturnMacroById(IdSrv, _ListIdSelect.Item(i).ElementID) IsNot Nothing Then
                ListBxDevice.Items.Add(Window1.myService.ReturnMacroById(IdSrv, _ListIdSelect.Item(i).ElementID).Nom)
            End If
        Next
    End Sub

    Private Sub BtnAjout_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        'If ListBxDispo.SelectedIndex < 0 Then
        '    MessageBox.Show("Veuillez sélectionner un élément", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation)
        'Else
        '    _ListIdSelect.Add(_ListIdDispo.Item(ListBxDispo.SelectedIndex))
        '    _ListIdDispo.RemoveAt(ListBxDispo.SelectedIndex)
        'End If
        'RefreshLists()
    End Sub

    Private Sub BtnDel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDel.Click
        If ListBxDevice.SelectedIndex < 0 Then
            MessageBox.Show("Veuillez sélectionner un élément à supprimer", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation)
        Else
            _ListIdSelect.RemoveAt(ListBxDevice.SelectedIndex)
        End If

        ChkVisible.IsChecked = False
        RefreshLists()
    End Sub

    Private Sub ListBxDevice_DragOver(ByVal sender As Object, ByVal e As System.Windows.DragEventArgs) Handles ListBxDevice.DragOver
        If e.Data.GetDataPresent(GetType(String)) Then
            e.Effects = DragDropEffects.Copy
        Else
            e.Effects = DragDropEffects.None
        End If

    End Sub

    Private Sub ListBxDevice_Drop(ByVal sender As Object, ByVal e As System.Windows.DragEventArgs) Handles ListBxDevice.Drop
        If e.Data.GetDataPresent(GetType(String)) Then
            e.Effects = DragDropEffects.Copy

            Dim uri As String = e.Data.GetData(GetType(String)).ToString  'DirectCast(e.Data.GetData(GetType(TreeViewItem)), String)
            If uri = _ZoneId Then
                MessageBox.Show("Une zone ne peut être une sous zone à elle même !", "Zone", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Exit Sub
            End If
            Dim y As New Zone.Element_Zone(uri, False)
            If _ListIdSelect.Contains(y) Then
                MessageBox.Show("Cet élément existe déjà dans la liste !", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Exit Sub
            End If
            _ListIdSelect.Add(y)
            y = Nothing
            RefreshLists()
        Else
            e.Effects = DragDropEffects.None
        End If
    End Sub

    Private Sub ListBxDevice_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ListBxDevice.MouseDoubleClick
        If ListBxDevice.SelectedIndex < 0 Then Exit Sub

        ChkVisible.Visibility = Windows.Visibility.Visible
        BtnOkDev.Visibility = Windows.Visibility.Visible

        ChkVisible.IsChecked = _ListIdSelect.Item(ListBxDevice.SelectedIndex).Visible
    End Sub

    Private Sub BtnOkDev_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOkDev.Click
        If ListBxDevice.SelectedIndex < 0 Then
            MessageBox.Show("Veuillez sélectionner un device", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation)
        Else
            _ListIdSelect(ListBxDevice.SelectedIndex).Visible = ChkVisible.IsChecked
            ChkVisible.Visibility = Windows.Visibility.Hidden
            BtnOkDev.Visibility = Windows.Visibility.Hidden
        End If
    End Sub

    Private Sub ImgZone_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ImgZone.MouseLeftButtonDown
        Try
            Dim frm As New WindowImg
            frm.ShowDialog()
            If frm.DialogResult.HasValue And frm.DialogResult.Value Then
                Dim retour As String = frm.FileName
                If retour <> "" Then
                    ImgZone.Source = ConvertArrayToImage(Window1.myService.GetByteFromImage(retour))
                    ImgZone.Tag = retour
                End If
                frm.Close()
            Else
                frm.Close()
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub ImgZone_MouseLeftButtonDown: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub ImgIcon_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ImgIcon.MouseLeftButtonDown
        Try
            Dim frm As New WindowImg
            frm.ShowDialog()
            If frm.DialogResult.HasValue And frm.DialogResult.Value Then
                Dim retour As String = frm.FileName
                If retour <> "" Then
                    ImgIcon.Source = ConvertArrayToImage(Window1.myService.GetByteFromImage(retour))
                    ImgIcon.Tag = retour
                End If
                frm.Close()
            Else
                frm.Close()
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub ImgIcon_MouseLeftButtonDown: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub


End Class
