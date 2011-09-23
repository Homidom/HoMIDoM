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
        If TxtName.Text = "" Then
            MessageBox.Show("Le nom de la zone est obligatoire!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        End If
        Window1.myService.SaveZone(_ZoneId, TxtName.Text, _ListIdSelect, ImgIcon.Tag, ImgZone.Tag)
        RaiseEvent CloseMe(Me)
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me)
    End Sub

    Public Sub New(ByVal Action As EAction, ByVal ZoneId As String)

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ImgIcon.Tag = " "
        ImgZone.Tag = " "

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        If Action = EAction.Nouveau Then 'Nouvelle Zone

        Else 'Modifier zone
            Dim x As Zone = Window1.myService.ReturnZoneByID(ZoneId)
            Dim _list As New List(Of Integer)

            _ZoneId = ZoneId
            If x IsNot Nothing Then
                TxtName.Text = x.Name
                For j As Integer = 0 To x.ListElement.Count - 1
                    _ListIdSelect.Add(x.ListElement.Item(j))
                Next

                If File.Exists(x.image) = True And x.image <> "" And x.image <> " " Then
                    Dim bmpImage As New BitmapImage()
                    bmpImage.BeginInit()
                    bmpImage.UriSource = New Uri(x.image, UriKind.Absolute)
                    bmpImage.EndInit()
                    ImgZone.Source = bmpImage
                    ImgZone.Tag = x.image
                End If

                If File.Exists(x.Icon) = True And x.Icon <> "" And x.Icon <> " " Then
                    Dim bmpImage As New BitmapImage()
                    bmpImage.BeginInit()
                    bmpImage.UriSource = New Uri(x.Icon, UriKind.Absolute)
                    bmpImage.EndInit()
                    ImgIcon.Source = bmpImage
                    ImgIcon.Tag = x.Icon
                End If
            End If
        End If

        RefreshLists()
    End Sub

    Private Sub RefreshLists()
        ListBxDevice.Items.Clear()

        For i As Integer = 0 To _ListIdSelect.Count - 1
            If Window1.myService.ReturnDeviceByID(_ListIdSelect.Item(i).ElementID) IsNot Nothing Then
                ListBxDevice.Items.Add(Window1.myService.ReturnDeviceByID(_ListIdSelect.Item(i).ElementID).Name)
            End If
            If Window1.myService.ReturnZoneByID(_ListIdSelect.Item(i).ElementID) IsNot Nothing Then
                ListBxDevice.Items.Add(Window1.myService.ReturnZoneByID(_ListIdSelect.Item(i).ElementID).Name)
            End If
            If Window1.myService.ReturnMacroById(_ListIdSelect.Item(i).ElementID) IsNot Nothing Then
                ListBxDevice.Items.Add(Window1.myService.ReturnMacroById(_ListIdSelect.Item(i).ElementID).Nom)
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

        TxtX.Text = "0"
        TxtY.Text = "0"
        ChkVisible.IsChecked = False

        RefreshLists()
    End Sub

    Private Sub TxtX_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtX.TextChanged
        If TxtX.Text <> "" And IsNumeric(TxtX.Text) = False Then
            MessageBox.Show("Veuillez saisir une valeur numérique")
            TxtX.Text = 0
            Exit Sub
        End If
    End Sub

    Private Sub TxtY_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtY.TextChanged
        If TxtY.Text <> "" And IsNumeric(TxtY.Text) = False Then
            MessageBox.Show("Veuillez saisir une valeur numérique")
            TxtY.Text = 0
            Exit Sub
        End If
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
            Dim y As New Zone.Element_Zone(uri, False, 0, 0)
            _ListIdSelect.Add(y)
            RefreshLists()
        Else
            e.Effects = DragDropEffects.None
        End If
    End Sub

    Private Sub ListBxDevice_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ListBxDevice.MouseDoubleClick
        If ListBxDevice.SelectedIndex < 0 Then Exit Sub

        ChkVisible.Visibility = Windows.Visibility.Visible
        TxtX.Visibility = Windows.Visibility.Visible
        TxtY.Visibility = Windows.Visibility.Visible
        BtnOkDev.Visibility = Windows.Visibility.Visible
        Label7.Visibility = Windows.Visibility.Visible
        Label8.Visibility = Windows.Visibility.Visible

        ChkVisible.IsChecked = _ListIdSelect.Item(ListBxDevice.SelectedIndex).Visible
        TxtX.Text = _ListIdSelect.Item(ListBxDevice.SelectedIndex).X
        TxtY.Text = _ListIdSelect.Item(ListBxDevice.SelectedIndex).Y
    End Sub

    Private Sub BtnOkDev_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOkDev.Click
        If ListBxDevice.SelectedIndex < 0 Then
            MessageBox.Show("Veuillez sélectionner un device", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation)
        Else
            _ListIdSelect(ListBxDevice.SelectedIndex).Visible = ChkVisible.IsChecked
            _ListIdSelect(ListBxDevice.SelectedIndex).X = TxtX.Text
            _ListIdSelect(ListBxDevice.SelectedIndex).Y = TxtY.Text

            ChkVisible.Visibility = Windows.Visibility.Hidden
            TxtX.Visibility = Windows.Visibility.Hidden
            TxtY.Visibility = Windows.Visibility.Hidden
            BtnOkDev.Visibility = Windows.Visibility.Hidden
            Label7.Visibility = Windows.Visibility.Hidden
            Label8.Visibility = Windows.Visibility.Hidden
        End If
    End Sub

    Private Sub ImgZone_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ImgZone.MouseLeftButtonDown
        Dim dlg As New Microsoft.Win32.OpenFileDialog()
        dlg.Filter = "jpeg (*.jpg) |*.jpg;*.jpeg|(*.png) |*.png|(*.*) |*.*"

        If dlg.ShowDialog() = True Then
            Dim bmpImage As New BitmapImage()
            bmpImage.BeginInit()
            bmpImage.UriSource = New Uri(dlg.FileName, UriKind.Absolute)
            bmpImage.EndInit()
            ImgZone.Source = bmpImage
            ImgZone.Tag = dlg.FileName
        End If
    End Sub

    Private Sub ImgIcon_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ImgIcon.MouseLeftButtonDown
        Dim dlg As New Microsoft.Win32.OpenFileDialog()
        dlg.Filter = "jpeg (*.jpg) |*.jpg;*.jpeg|(*.png) |*.png|(*.*) |*.*"

        If dlg.ShowDialog() = True Then
            Dim bmpImage As New BitmapImage()
            bmpImage.BeginInit()
            bmpImage.UriSource = New Uri(dlg.FileName, UriKind.Absolute)
            bmpImage.EndInit()
            ImgIcon.Source = bmpImage
            ImgIcon.Tag = dlg.FileName
        End If
    End Sub


End Class
