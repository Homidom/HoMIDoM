Imports HoMIDom.HoMIDom.Device
Imports HoMIDom.HoMIDom.Api
Imports System.IO

Partial Public Class uDevice
    '--- Variables ------------------
    Public Event CloseMe(ByVal MyObject As Object)
    Dim _Action As EAction 'Définit si modif ou création d'un device
    Dim _DeviceId As String 'Id du device à modifier
    Dim FlagNewCmd As Boolean

    Public Enum EAction
        Nouveau
        Modifier
    End Enum

    Public Sub New(ByVal Action As EAction, ByVal DeviceId As String)

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()
        Try
            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            _DeviceId = DeviceId
            _Action = Action

            'Liste les type de devices dans le combo
            For Each value As ListeDevices In [Enum].GetValues(GetType(HoMIDom.HoMIDom.Device.ListeDevices))
                CbType.Items.Add(value.ToString)
            Next

            'Liste les drivers dans le combo
            For i As Integer = 0 To Window1.myService.GetAllDrivers(IdSrv).Count - 1 'Window1.Obj.Drivers.Count - 1
                CbDriver.Items.Add(Window1.myService.GetAllDrivers(IdSrv).Item(i).Nom)  '(Window1.Obj.Drivers.Item(i).nom)
            Next

            If Action = EAction.Nouveau Then 'Nouveau Device
                ImgDevice.Tag = ""
            Else 'Modification d'un Device
                Dim x As HoMIDom.HoMIDom.TemplateDevice = Window1.myService.ReturnDeviceByID(IdSrv, DeviceId)

                If x IsNot Nothing Then 'on a trouvé le device
                    TxtNom.Text = x.Name
                    TxtDescript.Text = x.Description
                    ChkEnable.IsChecked = x.Enable
                    ChKSolo.IsChecked = x.Solo
                    CbType.SelectedValue = x.Type.ToString
                    CbType.IsEnabled = False

                    If CbType.SelectedValue = "FREEBOX" Then
                        Label6.Content = "Adresse Http Freebox:"
                        Label7.Content = "Code Telecommande:"
                    End If

                    For j As Integer = 0 To Window1.myService.GetAllDrivers(IdSrv).Count - 1 'Window1.Obj.Drivers.Count - 1
                        If Window1.myService.GetAllDrivers(IdSrv).Item(j).ID = x.DriverID Then
                            CbDriver.SelectedValue = Window1.myService.GetAllDrivers(IdSrv).Item(j).Nom
                            Exit For
                        End If
                    Next

                    TxtAdresse1.Text = x.Adresse1
                    TxtAdresse2.Text = x.Adresse2
                    TxtModele.Text = x.Modele
                    TxtRefresh.Text = x.Refresh
                    TxtLastChangeDuree.Text = x.LastChangeDuree

                    If x.Picture <> "" And x.Picture <> " " Then
                        ImgDevice.Source = ConvertArrayToImage(Window1.myService.GetByteFromImage(x.Picture))
                        ImgDevice.Tag = x.Picture
                    End If

                    'Gestion si Device avec Value
                    If x.Type = ListeDevices.TEMPERATURE _
                                       Or x.Type = ListeDevices.HUMIDITE _
                                       Or x.Type = ListeDevices.TEMPERATURECONSIGNE _
                                       Or x.Type = ListeDevices.ENERGIETOTALE _
                                       Or x.Type = ListeDevices.ENERGIEINSTANTANEE _
                                       Or x.Type = ListeDevices.PLUIETOTAL _
                                       Or x.Type = ListeDevices.PLUIECOURANT _
                                       Or x.Type = ListeDevices.VITESSEVENT _
                                       Or x.Type = ListeDevices.UV _
                                       Or x.Type = ListeDevices.HUMIDITE _
                                       Then
                        TxtCorrection.Visibility = Windows.Visibility.Visible
                        TxtCorrection.Text = x.Correction
                        TxtFormatage.Visibility = Windows.Visibility.Visible
                        TxtFormatage.Text = x.Formatage
                        TxtPrecision.Visibility = Windows.Visibility.Visible
                        TxtPrecision.Text = x.Precision
                        TxtValueMax.Visibility = Windows.Visibility.Visible
                        TxtValueMax.Text = x.ValueMax
                        TxtValueMin.Visibility = Windows.Visibility.Visible
                        TxtValueMin.Text = x.ValueMin
                        TxtValDef.Visibility = Windows.Visibility.Visible
                        TxtValDef.Text = x.ValueDef
                        Label10.Visibility = Windows.Visibility.Visible
                        Label11.Visibility = Windows.Visibility.Visible
                        Label12.Visibility = Windows.Visibility.Visible
                        Label13.Visibility = Windows.Visibility.Visible
                        Label14.Visibility = Windows.Visibility.Visible
                        Label15.Visibility = Windows.Visibility.Visible
                    Else
                        TxtCorrection.Height = 0
                        TxtFormatage.Height = 0
                        TxtPrecision.Height = 0
                        TxtValueMax.Height = 0
                        TxtValueMin.Height = 0
                        TxtValDef.Height = 0
                    End If

                    If x.Type = ListeDevices.MULTIMEDIA Then
                        ListCmd.Items.Clear()
                        For i As Integer = 0 To x.ListCommandName.Count - 1
                            ListCmd.Items.Add(x.ListCommandName(i))
                        Next
                        x = Nothing
                    Else
                        StkCde.Height = 0
                    End If
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
        Try
            If TxtNom.Text = "" Then
                MessageBox.Show("Le nom du device est obligatoire !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Exit Sub
            End If
            If CbType.Text = "" Then
                MessageBox.Show("Le type du device est obligatoire !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Exit Sub
            End If
            If CbDriver.Text = "" Then
                MessageBox.Show("Le driver du device est obligatoire !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Exit Sub
            End If
            If TxtAdresse1.Text = "" Then
                MessageBox.Show("L'adresse de base du device est obligatoire !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Exit Sub
            End If

            Dim _driverid As String = ""
            For i As Integer = 0 To Window1.myService.GetAllDrivers(IdSrv).Count - 1 'Window1.Obj.Drivers.Count - 1
                If Window1.myService.GetAllDrivers(IdSrv).Item(i).Nom = CbDriver.Text Then
                    _driverid = Window1.myService.GetAllDrivers(IdSrv).Item(i).ID
                    Exit For
                End If
            Next
            Window1.myService.SaveDevice(IdSrv, _DeviceId, TxtNom.Text, TxtAdresse1.Text, ChkEnable.IsChecked, ChKSolo.IsChecked, _driverid, CbType.Text, TxtRefresh.Text, TxtAdresse2.Text, ImgDevice.Tag, TxtModele.Text, TxtDescript.Text, TxtLastChangeDuree.Text)
            RaiseEvent CloseMe(Me)
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uDevice BtnOK_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub TxtRefresh_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtRefresh.TextChanged
        Try
            If TxtRefresh.Text <> "" And IsNumeric(TxtRefresh.Text) = False Then
                MessageBox.Show("Veuillez saisir une valeur numérique")
                TxtRefresh.Text = 0
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uDevice TxtRefresh_TextChanged: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub CbType_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles CbType.MouseLeave
        TxtCorrection.Visibility = Windows.Visibility.Hidden
        TxtFormatage.Visibility = Windows.Visibility.Hidden
        TxtPrecision.Visibility = Windows.Visibility.Hidden
        TxtValueMax.Visibility = Windows.Visibility.Hidden
        TxtValueMin.Visibility = Windows.Visibility.Hidden
        TxtValDef.Visibility = Windows.Visibility.Hidden
        Label10.Visibility = Windows.Visibility.Hidden
        Label11.Visibility = Windows.Visibility.Hidden
        Label12.Visibility = Windows.Visibility.Hidden
        Label13.Visibility = Windows.Visibility.Hidden
        Label14.Visibility = Windows.Visibility.Hidden
        Label15.Visibility = Windows.Visibility.Hidden

        If _Action = EAction.Nouveau Then
            'Gestion si Device avec Value
            If CbType.SelectedValue Is Nothing Then Exit Sub
            If CbType.SelectedValue = "TEMPERATURE" _
                               Or CbType.Text = "HUMIDITE" _
                               Or CbType.Text = "TEMPERATURECONSIGNE" _
                               Or CbType.Text = "ENERGIETOTALE" _
                               Or CbType.Text = "ENERGIEINSTANTANEE" _
                               Or CbType.Text = "PLUIETOTAL" _
                               Or CbType.Text = "PLUIECOURANT" _
                               Or CbType.Text = "VITESSEVENT" _
                               Or CbType.Text = "UV" _
                               Then
                TxtCorrection.Visibility = Windows.Visibility.Visible
                TxtFormatage.Visibility = Windows.Visibility.Visible
                TxtPrecision.Visibility = Windows.Visibility.Visible
                TxtValueMax.Visibility = Windows.Visibility.Visible
                TxtValueMin.Visibility = Windows.Visibility.Visible
                TxtValDef.Visibility = Windows.Visibility.Visible
                Label10.Visibility = Windows.Visibility.Visible
                Label11.Visibility = Windows.Visibility.Visible
                Label12.Visibility = Windows.Visibility.Visible
                Label13.Visibility = Windows.Visibility.Visible
                Label14.Visibility = Windows.Visibility.Visible
                Label15.Visibility = Windows.Visibility.Visible
            End If

            If CbType.SelectedValue = "FREEBOX" Then
                TxtAdresse1.Text = "http://hd1.freebox.fr"
                CbDriver.SelectedValue = "Virtuel"
                Label6.Content = "Adresse Http Freebox:"
                Label7.Content = "Code Telecommande:"
            End If

            If CbType.SelectedValue = "MULTIMEDIA" Then

            Else
                StkCde.Height = 0
            End If
        End If
    End Sub

    Private Sub BtnNewCmd_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnNewCmd.Click
        TxtCmdName.Text = ""
        TxtCmdRepeat.Text = "0"
        TxtCmdData.Text = ""

        FlagNewCmd = True
    End Sub

    Private Sub BtnSaveCmd_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnSaveCmd.Click
        If IsNumeric(TxtCmdRepeat.Text) = False Then
            MsgBox("Numérique obligatoire pour repeat !!")
            Exit Sub
        End If
        If TxtCmdName.Text = "" Or TxtCmdName.Text = " " Then
            MsgBox("Le nom de la commande est obligatoire !!")
            Exit Sub
        End If

        If FlagNewCmd = True Then 'nouvelle commande
            Window1.myService.SaveDeviceCommandIR(IdSrv, _DeviceId, TxtCmdName.Text, TxtCmdData.Text, TxtCmdRepeat.Text)
        Else 'modifier commande
            Window1.myService.SaveDeviceCommandIR(IdSrv, _DeviceId, TxtCmdName.Text, TxtCmdData.Text, TxtCmdRepeat.Text)
        End If

        ListCmd.Items.Clear()
        Dim x As Object = Window1.myService.ReturnDeviceByID(IdSrv, _DeviceId)
        For i As Integer = 0 To x.listcommandname.count - 1
            ListCmd.Items.Add(x.listcommandname(i))
        Next
        x = Nothing

        FlagNewCmd = False
    End Sub

    Private Sub BtnDelCmd_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDelCmd.Click
        If ListCmd.SelectedIndex >= 0 Then
            Window1.myService.DeleteDeviceCommandIR(IdSrv, _DeviceId, TxtCmdName.Text)

            TxtCmdName.Text = ""
            TxtCmdData.Text = ""
            TxtCmdRepeat.Text = "0"
            ListCmd.Items.Clear()

            Dim x As Object = Window1.myService.ReturnDeviceByID(IdSrv, _DeviceId)
            For i As Integer = 0 To x.listcommandname.count - 1
                ListCmd.Items.Add(x.listcommandname(i))
            Next
            x = Nothing

        End If
    End Sub

    Private Sub BtnTstCmd_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnTstCmd.Click
        Dim _Param As New ArrayList
        Dim x As New HoMIDom.HoMIDom.DeviceAction
        Dim y As New HoMIDom.HoMIDom.DeviceAction.Parametre

        y.Value = TxtCmdName.Text

        With x
            .Nom = "SendCommand"
            .Parametres.Add(y)
        End With

        Window1.myService.ExecuteDeviceCommand(IdSrv, _DeviceId, x)
    End Sub

    Private Sub BtnLearn_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnLearn.Click
        TxtCmdData.Text = Window1.myService.StartIrLearning(IdSrv)
    End Sub

    Private Sub TxtLastChangeDuree_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtLastChangeDuree.TextChanged
        If TxtLastChangeDuree.Text <> "" And IsNumeric(TxtLastChangeDuree.Text) = False Then
            MessageBox.Show("Veuillez saisir une valeur numérique")
            TxtLastChangeDuree.Text = 0
        End If
    End Sub

    Private Sub ImgDevice_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ImgDevice.MouseLeftButtonDown
        Try
            Dim frm As New WindowImg
            frm.ShowDialog()
            If frm.DialogResult.HasValue And frm.DialogResult.Value Then
                Dim retour As String = frm.FileName
                If retour <> "" Then
                    ImgDevice.Source = ConvertArrayToImage(Window1.myService.GetByteFromImage(retour))
                    ImgDevice.Tag = retour
                End If
                frm.Close()
            Else
                frm.Close()
            End If
            frm = Nothing
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub ImgDevice_MouseLeftButtonDown: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub ListCmd_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles ListCmd.SelectionChanged
        If ListCmd.SelectedIndex < 0 Then Exit Sub
        TxtCmdName.Text = Window1.myService.ReturnDeviceByID(IdSrv, _DeviceId).ListCommandName(ListCmd.SelectedIndex)
        TxtCmdData.Text = Window1.myService.ReturnDeviceByID(IdSrv, _DeviceId).ListCommandData(ListCmd.SelectedIndex)
        TxtCmdRepeat.Text = Window1.myService.ReturnDeviceByID(IdSrv, _DeviceId).ListCommandRepeat(ListCmd.SelectedIndex)
    End Sub

    Private Sub BtnRead_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnRead.Click
        If Window1.myService.ReturnDeviceByID(IdSrv, _DeviceId).Enable = False Then
            MessageBox.Show("Vous ne pouvez pas exécuter de commandes car le device n'est pas activé (propriété Enable)!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        End If

        Try
            Dim y As New uTestDevice(_DeviceId)
            y.Uid = System.Guid.NewGuid.ToString()
            AddHandler y.CloseMe, AddressOf UnloadControl
            Window1.CanvasUser.Children.Add(y)
        Catch ex As Exception
            MessageBox.Show("Erreur Tester: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Private Sub BtnWrite_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    Try
    '        Window1.Obj.TestWrite(_DeviceId, TxtCmd.Text, TxtP1.Text, TxtP2.Text)
    '    Catch ex As Exception
    '        MessageBox.Show("Error Write device" & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
    '    End Try
    'End Sub

    Private Sub UnloadControl(ByVal MyControl As Object)
        For i As Integer = 0 To Window1.CanvasUser.Children.Count - 1
            If Window1.CanvasUser.Children.Item(i).Uid = MyControl.uid Then
                Window1.CanvasUser.Children.RemoveAt(i)
                Exit Sub
            End If
        Next

    End Sub

End Class
