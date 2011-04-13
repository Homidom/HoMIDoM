Imports HoMIDom.HoMIDom
Imports System.IO

Partial Public Class uZone
    '--- Variables ------------------
    Public Event CloseMe(ByVal MyObject As Object)
    Dim _Action As EAction 'Définit si modif ou création d'un device
    Dim _ZoneId As String 'Id de la zone à modifier
    Dim FlagNewCmd As Boolean
    Dim _ListIdDispo As New List(Of Zone.Device_Zone)
    Dim _ListIdSelect As New List(Of Zone.Device_Zone)

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
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me)
    End Sub

    Public Sub New(ByVal Action As EAction, ByVal ZoneId As String)

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ImgIcon.Tag = " "
        ImgZone.Tag = " "

        For i As Integer = 0 To Window1.myService.GetAllDevices.Count - 1
            Dim x As New Zone.Device_Zone(Window1.myService.GetAllDevices.Item(i).ID, False, 0, 0)
            _ListIdDispo.Add(x)
        Next

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        If Action = EAction.Nouveau Then 'Nouvelle Zone

        Else 'Modifier zone
            Dim x As Zone = Window1.myService.ReturnZoneByID(ZoneId)
            Dim _list As New List(Of Integer)

            _ZoneId = ZoneId
            If x IsNot Nothing Then
                TxtName.Text = x.Name
                For j As Integer = 0 To x.ListDevice.Count - 1
                    _ListIdSelect.Add(x.ListDevice.Item(j))

                    For k As Integer = 0 To _ListIdDispo.Count - 1
                        If _ListIdDispo.Item(k).DeviceID = x.ListDevice.Item(j).DeviceID Then
                            _list.Add(k)
                        End If
                    Next
                Next

                For j As Integer = 0 To _list.Count - 1
                    _ListIdDispo.RemoveAt(_list.Item(j))
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
            Dim x As TemplateDevice = Window1.myService.ReturnDeviceByID(_ListIdDispo.Item(i).DeviceID)
            ListBxDispo.Items.Add(x.name)
            x = Nothing
        Next

        For i As Integer = 0 To _ListIdSelect.Count - 1
            ListBxDevice.Items.Add(Window1.myService.ReturnDeviceByID(_ListIdSelect.Item(i).DeviceID).Name)
        Next
    End Sub

    Private Sub BtnAjout_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnAjout.Click
        If ListBxDispo.SelectedIndex < 0 Then
            MessageBox.Show("Veuillez sélectionner un device", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation)
        Else
            _ListIdSelect.Add(_ListIdDispo.Item(ListBxDispo.SelectedIndex))
            _ListIdDispo.RemoveAt(ListBxDispo.SelectedIndex)
        End If
        RefreshLists()
    End Sub

    Private Sub BtnDel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDel.Click
        If ListBxDevice.SelectedIndex < 0 Then
            MessageBox.Show("Veuillez sélectionner un device", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation)
        Else
            _ListIdDispo.Add(_ListIdSelect.Item(ListBxDevice.SelectedIndex))
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

    Private Sub ListBxDevice_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ListBxDevice.MouseDown

    End Sub
End Class
