Imports HoMIDom.HoMIDom.Device
Imports HoMIDom.HoMIDom.Api
Imports System.IO

Partial Public Class uDriver
    '--- Variables ------------------
    Public Event CloseMe(ByVal MyObject As Object)
    Dim _DriverId As String 'Id du device à modifier

    Public Sub New(ByVal DriverId As String)

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()
        Try
            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            _DriverId = DriverId

            Dim x As HoMIDom.HoMIDom.TemplateDriver = Window1.myService.ReturnDriverByID(DriverId) 'Window1.Obj.ReturnDeviceByID(DeviceId)

            If x IsNot Nothing Then 'on a trouvé le device
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
        Window1.myService.SaveDriver(_DriverId, TxtNom.Text, ChkEnable.IsChecked, CbStartAuto.IsChecked, TxtAdrTCP.Text, TxtPortTCP.Text, TxtAdrUDP.Text, TxtPortUDP.Text, TxtCom.Text, TxtRefresh.Text, ImgDevice.Tag)
        RaiseEvent CloseMe(Me)
    End Sub

    Private Sub TxtRefresh_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtRefresh.TextChanged
        If TxtRefresh.Text <> "" And IsNumeric(TxtRefresh.Text) = False Then
            MessageBox.Show("Veuillez saisir une valeur numérique")
            TxtRefresh.Text = 0
        End If
    End Sub


    Private Sub ImgDevice_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ImgDevice.MouseLeftButtonDown
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
    End Sub

    Private Sub UnloadControl(ByVal MyControl As Object)
        For i As Integer = 0 To Window1.CanvasUser.Children.Count - 1
            If Window1.CanvasUser.Children.Item(i).Uid = MyControl.uid Then
                Window1.CanvasUser.Children.RemoveAt(i)
                Exit Sub
            End If
        Next

    End Sub

End Class
