Imports HoMIDom.HoMIDom
Imports System.IO

Partial Public Class uZone
    '--- Variables ------------------
    Public Event CloseMe(ByVal MyObject As Object)
    Dim _Action As EAction 'Définit si modif ou création d'un device
    Dim _ZoneId As String 'Id de la zone à modifier
    Dim FlagNewCmd As Boolean
    Dim _ListIdDispo As New ArrayList
    Dim _ListIdSelect As New ArrayList

    Public Enum EAction
        Nouveau
        Modifier
    End Enum

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        If TxtName.Text = "" Then
            MessageBox.Show("Le nom de la zone est obligatoire!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        End If

        Window1.Obj.SaveZone(_ZoneId, TxtName.Text, _ListIdSelect, ImgIcon.Tag, ImgZone.Tag)
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me)
    End Sub

    Public Sub New(ByVal Action As EAction, ByVal ZoneId As String)

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ImgIcon.Tag = " "
        ImgZone.Tag = " "

        For i As Integer = 0 To Window1.Obj.Devices.Count - 1
            _ListIdDispo.Add(Window1.Obj.Devices.Item(i).id)
        Next

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        If Action = EAction.Nouveau Then 'Nouvelle Zone

        Else 'Modifier zone
            Dim x As Object = Window1.Obj.ReturnZoneByID(ZoneId)
            _ZoneId = ZoneId
            If x IsNot Nothing Then
                TxtName.Text = x.Name
                For j As Integer = 0 To x.listdevice.count - 1
                    _ListIdSelect.Add(x.listdevice.item(j))
                    For k As Integer = 0 To _ListIdDispo.Count - 1
                        If _ListIdDispo.Item(k) = x.listdevice.item(j).deviceid Then
                            _ListIdDispo.RemoveAt(k)
                        End If
                    Next
                Next


                If File.Exists(x.image) = True And x.image <> "" And x.image <> " " Then
                    Dim bmpImage As New BitmapImage()
                    bmpImage.BeginInit()
                    bmpImage.UriSource = New Uri(x.image, UriKind.Absolute)
                    bmpImage.EndInit()
                    ImgZone.Source = bmpImage
                    ImgZone.Tag = x.image
                End If

                If File.Exists(x.image) = True And x.icon <> "" And x.icon <> " " Then
                    Dim bmpImage As New BitmapImage()
                    bmpImage.BeginInit()
                    bmpImage.UriSource = New Uri(x.icon, UriKind.Absolute)
                    bmpImage.EndInit()
                    ImgIcon.Source = bmpImage
                    ImgIcon.Tag = x.icon
                End If
            End If
        End If

        RefreshLists()
    End Sub

    Private Sub RefreshLists()
        ListBxDispo.Items.Clear()
        ListBxDevice.Items.Clear()

        For i As Integer = 0 To _ListIdDispo.Count - 1
            Dim x As Object = Window1.Obj.ReturnDeviceByID(_ListIdDispo.Item(i))
            ListBxDispo.Items.Add(x.name)
            x = Nothing
        Next

        For i As Integer = 0 To _ListIdSelect.Count - 1
            ListBxDevice.Items.Add(Window1.Obj.ReturnDeviceByID(_ListIdSelect.Item(i).deviceid).name)
        Next
    End Sub

    Private Sub BtnAjout_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnAjout.Click
        If ListBxDispo.SelectedIndex < 0 Then
            MessageBox.Show("Veuillez sélectionner un device", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation)
        Else
            Dim x As New Zone.Device_Zone(_ListIdDispo.Item(ListBxDispo.SelectedIndex), False, 0, 0)
            _ListIdSelect.Add(x)
            _ListIdDispo.RemoveAt(ListBxDispo.SelectedIndex)
        End If
        RefreshLists()
    End Sub

    Private Sub BtnDel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDel.Click
        If ListBxDevice.SelectedIndex < 0 Then
            MessageBox.Show("Veuillez sélectionner un device", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation)
        Else
            _ListIdDispo.Add(_ListIdSelect.Item(ListBxDevice.SelectedIndex).deviceid)
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

    Private Sub ListBxDevice_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ListBxDevice.MouseDoubleClick
        If ListBxDevice.SelectedIndex < 0 Then Exit Sub

        ChkVisible.IsChecked = _ListIdSelect.Item(ListBxDevice.SelectedIndex).visible
        TxtX.Text = _ListIdSelect.Item(ListBxDevice.SelectedIndex).x
        TxtY.Text = _ListIdSelect.Item(ListBxDevice.SelectedIndex).y
    End Sub

    Private Sub BtnOkDev_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOkDev.Click
        If ListBxDevice.SelectedIndex < 0 Then
            MessageBox.Show("Veuillez sélectionner un device", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation)
        Else
            _ListIdSelect(ListBxDevice.SelectedIndex).visible = ChkVisible.IsChecked
            _ListIdSelect(ListBxDevice.SelectedIndex).x = TxtX.Text
            _ListIdSelect(ListBxDevice.SelectedIndex).y = TxtY.Text
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
