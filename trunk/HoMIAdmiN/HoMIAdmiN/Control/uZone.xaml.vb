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

            SaveElement()
            Window1.myService.SaveZone(IdSrv, _ZoneId, TxtName.Text, _ListIdSelect, ImgIcon.Tag.ToString, ImgZone.Tag.ToString)
            RaiseEvent CloseMe(Me)
        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'enregistrement de la zone, message: " & ex.ToString, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub SaveElement()
        Try

            _ListIdSelect.Clear()
            Dim y As StackPanel
            Dim x1 As CheckBox
            Dim x2 As CheckBox

            For j As Integer = 0 To ListBxDevice.Items.Count - 1
                y = ListBxDevice.Items(j)
                x1 = y.Children.Item(0)
                x2 = y.Children.Item(1)
                If x1.IsChecked = True Then
                    Dim z As New Zone.Element_Zone(x1.Uid, x2.IsChecked)
                    _ListIdSelect.Add(z)
                End If
            Next
            For j As Integer = 0 To ListBxZone.Items.Count - 1
                y = ListBxZone.Items(j)
                x1 = y.Children.Item(0)
                x2 = y.Children.Item(1)
                If x1.IsChecked = True Then
                    Dim z As New Zone.Element_Zone(x1.Uid, x2.IsChecked)
                    _ListIdSelect.Add(z)
                End If
            Next
            For j As Integer = 0 To ListBxMacro.Items.Count - 1
                y = ListBxMacro.Items(j)
                x1 = y.Children.Item(0)
                x2 = y.Children.Item(1)
                If x1.IsChecked = True Then
                    Dim z As New Zone.Element_Zone(x1.Uid, x2.IsChecked)
                    _ListIdSelect.Add(z)
                End If
            Next
        Catch ex As Exception
            MessageBox.Show("Erreur lors du traitement SaveElement: " & ex.ToString, "Admin", MessageBoxButton.OK, MessageBoxImage.Error)
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

        For Each Device In Window1.myService.GetAllDevices(IdSrv)
            Dim stk As New StackPanel
            stk.Orientation = Orientation.Horizontal

            Dim x As New CheckBox
            x.Content = Device.Name
            x.ToolTip = Device.Name
            x.Uid = Device.ID
            x.Width = 110
            AddHandler x.Click, AddressOf ChkElement_Click
            stk.Children.Add(x)

            Dim y As New CheckBox
            y.Content = "Visible"
            y.ToolTip = "Visible côté client"
            y.Visibility = Windows.Visibility.Hidden
            stk.Children.Add(y)

            ListBxDevice.Items.Add(stk)
        Next

        For Each Zone In Window1.myService.GetAllZones(IdSrv)
            Dim stk As New StackPanel
            stk.Orientation = Orientation.Horizontal

            Dim x As New CheckBox
            x.Content = Zone.Name
            If Zone.Name = TxtName.Text Then x.IsEnabled = False
            x.ToolTip = Zone.Name
            x.Uid = Zone.ID
            x.Width = 110
            AddHandler x.Click, AddressOf ChkElement_Click
            stk.Children.Add(x)

            Dim y As New CheckBox
            y.Content = "Visible"
            y.ToolTip = "Visible côté client"
            y.Visibility = Windows.Visibility.Hidden
            stk.Children.Add(y)

            ListBxZone.Items.Add(stk)
        Next

        For Each Macro In Window1.myService.GetAllMacros(IdSrv)
            Dim stk As New StackPanel
            stk.Orientation = Orientation.Horizontal

            Dim x As New CheckBox
            x.Content = Macro.Nom
            x.ToolTip = Macro.Nom
            x.Uid = Macro.ID
            x.Width = 110
            AddHandler x.Click, AddressOf ChkElement_Click
            stk.Children.Add(x)

            Dim y As New CheckBox
            y.Content = "Visible"
            y.ToolTip = "Visible côté client"
            y.Visibility = Windows.Visibility.Hidden
            stk.Children.Add(y)

            ListBxMacro.Items.Add(stk)
        Next
        RefreshLists()
    End Sub

    Private Sub RefreshLists()

        For i As Integer = 0 To _ListIdSelect.Count - 1
            Dim x As CheckBox
            Dim y As CheckBox
            Dim stk As StackPanel

            For j As Integer = 0 To ListBxDevice.Items.Count - 1
                stk = ListBxDevice.Items(j)
                x = stk.Children.Item(0)
                y = stk.Children.Item(1)

                If x.Uid = _ListIdSelect.Item(i).ElementID Then
                    x.IsChecked = True
                    y.Visibility = Windows.Visibility.Visible
                    If _ListIdSelect.Item(i).Visible = True Then
                        y.IsChecked = True
                    Else
                        y.IsChecked = False
                    End If
                    Exit For
                End If
            Next

            For j As Integer = 0 To ListBxZone.Items.Count - 1
                stk = ListBxZone.Items(j)
                x = stk.Children.Item(0)
                y = stk.Children.Item(1)
                If x.Uid = _ListIdSelect.Item(i).ElementID Then
                    x.IsChecked = True
                    y.Visibility = Windows.Visibility.Visible
                    If _ListIdSelect.Item(i).Visible = True Then
                        y.IsChecked = True
                    Else
                        y.IsChecked = False
                    End If
                    Exit For
                Else
                    x.IsChecked = False
                    y.Visibility = Windows.Visibility.Hidden
                End If
            Next
            For j As Integer = 0 To ListBxMacro.Items.Count - 1
                stk = ListBxMacro.Items(j)
                x = stk.Children.Item(0)
                y = stk.Children.Item(1)
                If x.Uid = _ListIdSelect.Item(i).ElementID Then
                    x.IsChecked = True
                    y.Visibility = Windows.Visibility.Visible
                    If _ListIdSelect.Item(i).Visible = True Then
                        y.IsChecked = True
                    Else
                        y.IsChecked = False
                    End If
                    Exit For
                Else
                    x.IsChecked = False
                    y.Visibility = Windows.Visibility.Hidden
                End If
            Next
        Next
    End Sub

    Private Sub ListBxDevice_DragOver(ByVal sender As Object, ByVal e As System.Windows.DragEventArgs)
        If e.Data.GetDataPresent(GetType(String)) Then
            e.Effects = DragDropEffects.Copy
        Else
            e.Effects = DragDropEffects.None
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


    Private Sub ChkElement_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        If InStr(UCase(sender.parent.parent.name), "DEVICE") > 0 Then
            For Each stk As StackPanel In ListBxDevice.Items
                Dim x As CheckBox = stk.Children.Item(0)
                If x.IsChecked = True Then
                    stk.Children.Item(1).Visibility = Windows.Visibility.Visible
                Else
                    stk.Children.Item(1).Visibility = Windows.Visibility.Hidden
                End If
            Next
        End If
        If InStr(UCase(sender.parent.parent.name), "ZONE") > 0 Then
            For Each stk As StackPanel In ListBxZone.Items
                Dim x As CheckBox = stk.Children.Item(0)
                If x.IsChecked = True Then
                    stk.Children.Item(1).Visibility = Windows.Visibility.Visible
                Else
                    stk.Children.Item(1).Visibility = Windows.Visibility.Hidden
                End If
            Next
        End If
        If InStr(UCase(sender.parent.parent.name), "MACRO") > 0 Then
            For Each stk As StackPanel In ListBxMacro.Items
                Dim x As CheckBox = stk.Children.Item(0)
                If x.IsChecked = True Then
                    stk.Children.Item(1).Visibility = Windows.Visibility.Visible
                Else
                    stk.Children.Item(1).Visibility = Windows.Visibility.Hidden
                End If
            Next
        End If
    End Sub
End Class
