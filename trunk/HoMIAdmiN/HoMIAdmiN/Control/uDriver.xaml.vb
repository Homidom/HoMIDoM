Imports HoMIDom.HoMIDom.Device
Imports HoMIDom.HoMIDom.Api
Imports System.IO

Partial Public Class uDriver
    '--- Variables ------------------
    Public Event CloseMe(ByVal MyObject As Object)
    Dim _DriverId As String 'Id du device à modifier
    Dim x As HoMIDom.HoMIDom.TemplateDriver
    Dim _ListParam As New ArrayList

    Public Sub New(ByVal DriverId As String)

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()
        Try
            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            _DriverId = DriverId

            x = Window1.myService.ReturnDriverByID(DriverId) 'Window1.Obj.ReturnDeviceByID(DeviceId)

            If x IsNot Nothing Then 'on a trouvé le device
                If x.IP_TCP = "@" Then
                    Label4.Visibility = Windows.Visibility.Hidden
                    TxtAdrTCP.Visibility = Windows.Visibility.Hidden
                Else
                    Label4.Visibility = Windows.Visibility.Visible
                    TxtAdrTCP.Visibility = Windows.Visibility.Visible
                End If
                If x.Port_TCP = "@" Then
                    Label5.Visibility = Windows.Visibility.Hidden
                    TxtPortTCP.Visibility = Windows.Visibility.Hidden
                Else
                    Label5.Visibility = Windows.Visibility.Visible
                    TxtPortTCP.Visibility = Windows.Visibility.Visible
                End If
                If x.IP_UDP = "@" Then
                    Label7.Visibility = Windows.Visibility.Hidden
                    TxtAdrUDP.Visibility = Windows.Visibility.Hidden
                Else
                    Label7.Visibility = Windows.Visibility.Visible
                    TxtAdrUDP.Visibility = Windows.Visibility.Visible
                End If
                If x.Port_UDP = "@" Then
                    Label16.Visibility = Windows.Visibility.Hidden
                    TxtPortUDP.Visibility = Windows.Visibility.Hidden
                Else
                    Label16.Visibility = Windows.Visibility.Visible
                    TxtPortUDP.Visibility = Windows.Visibility.Visible
                End If
                If x.COM = "@" Then
                    Label10.Visibility = Windows.Visibility.Hidden
                    TxtCom.Visibility = Windows.Visibility.Hidden
                Else
                    Label10.Visibility = Windows.Visibility.Visible
                    TxtCom.Visibility = Windows.Visibility.Visible
                End If
                If x.Parametres IsNot Nothing And x.Parametres.Count > 0 Then
                    Label15.Visibility = Windows.Visibility.Visible
                    Label14.Visibility = Windows.Visibility.Visible
                    TxtParam.Visibility = Windows.Visibility.Visible
                    CbParam.Visibility = Windows.Visibility.Visible
                    CbParam.Items.Clear()
                    For k As Integer = 0 To x.Parametres.Count - 1
                        CbParam.Items.Add(x.Parametres.Item(k).Nom)
                        _ListParam.Add(x.Parametres.Item(k).Valeur)
                    Next
                Else
                    Label15.Visibility = Windows.Visibility.Hidden
                    Label14.Visibility = Windows.Visibility.Hidden
                    TxtParam.Visibility = Windows.Visibility.Hidden
                    CbParam.Visibility = Windows.Visibility.Hidden
                End If
                TxtNom.Text = x.Nom
                TxtDescript.Text = x.Description
                ChkEnable.IsChecked = x.Enable
                CbStartAuto.IsChecked = x.StartAuto
                TxtProtocol.Text = x.Protocol
                TxtAdrTCP.Text = x.IP_TCP
                TxtPortTCP.Text = x.Port_TCP
                TxtAdrUDP.Text = x.IP_UDP
                TxtPortUDP.Text = x.Port_UDP
                TxtCom.Text = x.COM
                TxtRefresh.Text = x.Refresh
                TxtVersion.Text = x.Version
                TxtModele.Text = x.Modele

                If x.Picture <> "" And File.Exists(x.Picture) = True Then
                    Dim bmpImage As New BitmapImage()
                    bmpImage.BeginInit()
                    bmpImage.UriSource = New Uri(x.Picture, UriKind.Absolute)
                    bmpImage.EndInit()
                    ImgDevice.Source = bmpImage
                    ImgDevice.Tag = x.Picture
                End If

            End If

        Catch Ex As Exception
            MessageBox.Show("Erreur: " & Ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Bouton Fermer
    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me)
    End Sub

    'Bouton Ok
    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        Window1.myService.SaveDriver(_DriverId, TxtNom.Text, ChkEnable.IsChecked, CbStartAuto.IsChecked, TxtAdrTCP.Text, TxtPortTCP.Text, TxtAdrUDP.Text, TxtPortUDP.Text, TxtCom.Text, TxtRefresh.Text, ImgDevice.Tag, _ListParam)
        RaiseEvent CloseMe(Me)
    End Sub

    Private Sub TxtRefresh_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtRefresh.TextChanged
        If TxtRefresh.Text <> "" And IsNumeric(TxtRefresh.Text) = False Then
            MessageBox.Show("Veuillez saisir une valeur numérique")
            TxtRefresh.Text = 0
        End If
    End Sub


    Private Sub ImgDevice_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ImgDevice.MouseLeftButtonDown
        Try
            Dim dlg As New Microsoft.Win32.OpenFileDialog()
            dlg.Filter = "jpeg (*.jpg) |*.jpg;*.jpeg|(*.png) |*.png|(*.*) |*.*"

            If dlg.ShowDialog() = True Then
                Dim bmpImage As New BitmapImage()
                bmpImage.BeginInit()
                bmpImage.UriSource = New Uri(dlg.FileName, UriKind.Absolute)
                bmpImage.EndInit()
                ImgDevice.Source = bmpImage
                ImgDevice.Tag = dlg.FileName
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur uDriver ImgDevice_MouseLeftButtonDown: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub UnloadControl(ByVal MyControl As Object)
        For i As Integer = 0 To Window1.CanvasUser.Children.Count - 1
            If Window1.CanvasUser.Children.Item(i).Uid = MyControl.uid Then
                Window1.CanvasUser.Children.RemoveAt(i)
                Exit Sub
            End If
        Next

    End Sub

    Private Sub BtnAv_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnAv.Click
        If GroupBox1.Visibility = Windows.Visibility.Hidden Then
            BtnAv.Content = "<<"
            GroupBox1.Visibility = Windows.Visibility.Visible
            CbCmd.Items.Clear()
            If x IsNot Nothing Then
                For i As Integer = 0 To x.DeviceAction.Count - 1
                    CbCmd.Items.Add(x.DeviceAction.Item(i).Nom)
                Next
            End If
        Else
            CbCmd.Items.Clear()
            BtnAv.Content = ">>"
            GroupBox1.Visibility = Windows.Visibility.Hidden
        End If
    End Sub

    Private Sub CbCmd_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles CbCmd.MouseLeave
        LblP1.Visibility = Windows.Visibility.Hidden
        LblP2.Visibility = Windows.Visibility.Hidden
        LblP3.Visibility = Windows.Visibility.Hidden
        LblP4.Visibility = Windows.Visibility.Hidden
        LblP5.Visibility = Windows.Visibility.Hidden
        TxtP1.Text = ""
        TxtP2.Text = ""
        TxtP3.Text = ""
        TxtP4.Text = ""
        TxtP5.Text = ""
        TxtP1.Visibility = Windows.Visibility.Hidden
        TxtP2.Visibility = Windows.Visibility.Hidden
        TxtP3.Visibility = Windows.Visibility.Hidden
        TxtP4.Visibility = Windows.Visibility.Hidden
        TxtP5.Visibility = Windows.Visibility.Hidden

        If CbCmd.Text = "" Then Exit Sub
        If CbCmd.SelectedIndex < 0 Then Exit Sub

        Dim Idx As Integer = CbCmd.SelectedIndex
        For j As Integer = 0 To x.DeviceAction.Item(Idx).Parametres.Count - 1

            Select Case j
                Case 0
                    LblP1.Content = x.DeviceAction.Item(Idx).Parametres.Item(j).Nom & " :"
                    LblP1.Visibility = Windows.Visibility.Visible
                    TxtP1.ToolTip = x.DeviceAction.Item(Idx).Parametres.Item(j).Type
                    TxtP1.Visibility = Windows.Visibility.Visible
                Case 1
                    LblP2.Content = x.DeviceAction.Item(Idx).Parametres.Item(j).Nom & " :"
                    LblP2.Visibility = Windows.Visibility.Visible
                    TxtP2.ToolTip = x.DeviceAction.Item(Idx).Parametres.Item(j).Type
                    TxtP2.Visibility = Windows.Visibility.Visible
                Case 2
                    LblP3.Content = x.DeviceAction.Item(Idx).Parametres.Item(j).Nom & " :"
                    LblP3.Visibility = Windows.Visibility.Visible
                    TxtP3.ToolTip = x.DeviceAction.Item(Idx).Parametres.Item(j).Type
                    TxtP3.Visibility = Windows.Visibility.Visible
                Case 3
                    LblP4.Content = x.DeviceAction.Item(Idx).Parametres.Item(j).Nom & " :"
                    LblP4.Visibility = Windows.Visibility.Visible
                    TxtP4.ToolTip = x.DeviceAction.Item(Idx).Parametres.Item(j).Type
                    TxtP4.Visibility = Windows.Visibility.Visible
                Case 4
                    LblP5.Content = x.DeviceAction.Item(Idx).Parametres.Item(j).Nom & " :"
                    LblP5.Visibility = Windows.Visibility.Visible
                    TxtP5.ToolTip = x.DeviceAction.Item(Idx).Parametres.Item(j).Type
                    TxtP5.Visibility = Windows.Visibility.Visible
            End Select
        Next
    End Sub

    Private Sub BtnTest_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnTest.Click
        Try
            Dim a As New HoMIDom.HoMIDom.DeviceAction

            a.Nom = CbCmd.Text

            If TxtP1.Text <> "" Then
                Dim y As New HoMIDom.HoMIDom.DeviceAction.Parametre
                y.Value = TxtP1.Text
                a.Parametres.Add(y)
            End If
            If TxtP2.Text <> "" Then
                Dim y As New HoMIDom.HoMIDom.DeviceAction.Parametre
                y.Value = TxtP2.Text
                a.Parametres.Add(y)
            End If
            If TxtP3.Text <> "" Then
                Dim y As New HoMIDom.HoMIDom.DeviceAction.Parametre
                y.Value = TxtP3.Text
                a.Parametres.Add(y)
            End If
            If TxtP4.Text <> "" Then
                Dim y As New HoMIDom.HoMIDom.DeviceAction.Parametre
                y.Value = TxtP4.Text
                a.Parametres.Add(y)
            End If
            If TxtP5.Text <> "" Then
                Dim y As New HoMIDom.HoMIDom.DeviceAction.Parametre
                y.Value = TxtP5.Text
                a.Parametres.Add(y)
            End If

            Window1.myService.ExecuteDriverCommand(_DriverId, a)
        Catch ex As Exception
            MessageBox.Show("Erreur lors du test: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
    Private Sub CbParam_SelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles CbParam.SelectionChanged
        If CbParam.SelectedIndex >= 0 Then
            CbParam.ToolTip = x.Parametres.Item(CbParam.SelectedIndex).Description
            TxtParam.Text = _ListParam.Item(CbParam.SelectedIndex)
        End If
    End Sub

    Private Sub BtnOkParam_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOkParam.Click
        If CbParam.SelectedIndex >= 0 And TxtParam.Text <> "" Then
            _ListParam.Item(CbParam.SelectedIndex) = TxtParam.Text
        Else
            MessageBox.Show("Veuillez sélectionner un paramètre ou saisir sa valeur", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
        End If
    End Sub
End Class
