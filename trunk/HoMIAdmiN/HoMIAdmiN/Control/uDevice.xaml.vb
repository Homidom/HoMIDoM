Imports HoMIDom.HoMIDom.Device
Imports HoMIDom.HoMIDom.Api
Imports System.IO
Imports System.Threading

Partial Public Class uDevice

    '--- Variables ------------------
    Public Event CloseMe(ByVal MyObject As Object)
    Dim _Action As EAction 'Définit si modif ou création d'un device
    Dim _DeviceId As String 'Id du device à modifier
    Dim FlagNewCmd As Boolean
    Dim _Driver As HoMIDom.HoMIDom.TemplateDriver

    Public Sub New(ByVal Action As Classe.EAction, ByVal DeviceId As String)

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
            For i As Integer = 0 To myService.GetAllDrivers(IdSrv).Count - 1
                CbDriver.Items.Add(myService.GetAllDrivers(IdSrv).Item(i).Nom)
            Next

            If Action = EAction.Nouveau Then 'Nouveau Device
                ImgDevice.Tag = ""
                StkCde.Height = 0
            Else 'Modification d'un Device
                Dim x As HoMIDom.HoMIDom.TemplateDevice = myservice.ReturnDeviceByID(IdSrv, DeviceId)

                If x IsNot Nothing Then 'on a trouvé le device

                    TxtNom.Text = x.Name
                    TxtDescript.Text = x.Description
                    ChkEnable.IsChecked = x.Enable
                    ChKSolo.IsChecked = x.Solo
                    ChKLastEtat.IsChecked = x.LastEtat
                    CbType.SelectedValue = x.Type.ToString
                    CbType.IsEnabled = False
                    BtnRead.Visibility = Windows.Visibility.Visible

                    If CbType.SelectedValue = "FREEBOX" Then
                        Label6.Content = "Adresse Http Freebox:"
                        Label7.Content = "Code Telecommande:"
                    End If

                    'For j As Integer = 0 To myService.GetAllDrivers(IdSrv).Count - 1
                    '    If myService.GetAllDrivers(IdSrv).Item(j).ID = x.DriverID Then
                    '        _Driver = myService.GetAllDrivers(IdSrv).Item(j)
                    '        CbDriver.SelectedValue = _Driver.Nom
                    '        MaJDriver()
                    '        Exit For
                    '    End If
                    'Next

                    _Driver = myService.ReturnDriverByID(IdSrv, x.DriverID)
                    If _Driver IsNot Nothing Then
                        CbDriver.SelectedValue = _Driver.Nom
                    End If

                    TxtAdresse1.Text = x.Adresse1
                    TxtAdresse2.Text = x.Adresse2
                    TxtModele.Text = x.Modele
                    TxtModele2.Text = x.Modele
                    TxtRefresh.Text = x.Refresh
                    TxtLastChangeDuree.Text = x.LastChangeDuree

                    ImgDevice.Source = ConvertArrayToImage(myService.GetByteFromImage(x.Picture))
                    ImgDevice.Tag = x.Picture

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

                    'on verifie si le device est un device systeme pour ne pas le rendre modifiable
                    If Left(x.Name, 5) = "HOMI_" Then
                        Label1.Content = "Device SYSTEME"
                        TxtNom.IsReadOnly = True
                        TxtDescript.IsReadOnly = True
                        CbDriver.IsEditable = False
                        CbDriver.IsReadOnly = True
                        CbDriver.IsEnabled = False
                        ChkEnable.Visibility = Windows.Visibility.Hidden
                        ChKSolo.Visibility = Windows.Visibility.Hidden
                        TxtAdresse1.Visibility = Windows.Visibility.Hidden
                        TxtAdresse2.Visibility = Windows.Visibility.Hidden
                        TxtModele.Visibility = Windows.Visibility.Hidden
                        TxtRefresh.Visibility = Windows.Visibility.Hidden
                        TxtLastChangeDuree.Visibility = Windows.Visibility.Hidden
                        BtnRead.Visibility = Windows.Visibility.Hidden
                        Label6.Visibility = Windows.Visibility.Hidden
                        Label7.Visibility = Windows.Visibility.Hidden
                        Label8.Visibility = Windows.Visibility.Hidden
                        Label9.Visibility = Windows.Visibility.Hidden
                        Label19.Visibility = Windows.Visibility.Hidden
                    End If

                End If
                End If

                'Liste toutes les zones dans la liste
                For i As Integer = 0 To myService.GetAllZones(IdSrv).Count - 1
                    Dim ch1 As New CheckBox
                    Dim ch2 As New CheckBox

                    Dim stk As New StackPanel
                    stk.Orientation = Orientation.Horizontal

                    ch1.Width = 80
                    ch1.Content = myService.GetAllZones(IdSrv).Item(i).Name
                    ch1.ToolTip = ch1.Content
                    ch1.Uid = myService.GetAllZones(IdSrv).Item(i).ID
                    AddHandler ch1.Click, AddressOf ChkElement_Click

                    ch2.Content = "Visible"
                    ch2.ToolTip = "Visible dans la zone côté client"
                    ch2.Visibility = Windows.Visibility.Hidden

                    For j As Integer = 0 To myService.GetAllZones(IdSrv).Item(i).ListElement.Count - 1
                        If myService.GetAllZones(IdSrv).Item(i).ListElement.Item(j).ElementID = _DeviceId Then
                            ch1.IsChecked = True
                            ch2.Visibility = Windows.Visibility.Visible

                            If myService.GetAllZones(IdSrv).Item(i).ListElement.Item(j).Visible = True Then
                                ch2.IsChecked = True
                            End If
                            Exit For
                        End If
                    Next

                    stk.Children.Add(ch1)
                    stk.Children.Add(ch2)
                    ListZone.Items.Add(stk)
                Next
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

            TxtRefresh.Text = Replace(TxtRefresh.Text, ".", ",")

            Dim _driverid As String = ""
            For i As Integer = 0 To myservice.GetAllDrivers(IdSrv).Count - 1 'Window1.Obj.Drivers.Count - 1
                If myservice.GetAllDrivers(IdSrv).Item(i).Nom = CbDriver.Text Then
                    _driverid = myService.GetAllDrivers(IdSrv).Item(i).ID
                    Exit For
                End If
            Next

            Dim retour As String = myService.VerifChamp(IdSrv, _driverid, "ADRESSE1", TxtAdresse1.Text)
            If retour <> "0" Then
                MessageBox.Show("Champ " & Label6.Content & ": " & retour, "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Exit Sub
            End If
            retour = myService.VerifChamp(IdSrv, _driverid, "ADRESSE2", TxtAdresse2.Text)
            If retour <> "0" Then
                MessageBox.Show("Champ " & Label7.Content & ": " & retour, "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Exit Sub
            End If

            Dim _modele As String
            If TxtModele.Tag = 1 Then
                _modele = TxtModele.Text
            Else
                If TxtModele2.Tag = 1 Then
                    _modele = TxtModele2.Text
                Else
                    _modele = ""
                End If
            End If

            myService.SaveDevice(IdSrv, _DeviceId, TxtNom.Text, TxtAdresse1.Text, ChkEnable.IsChecked, ChKSolo.IsChecked, _driverid, CbType.Text, TxtRefresh.Text, TxtAdresse2.Text, ImgDevice.Tag, _modele, TxtDescript.Text, TxtLastChangeDuree.Text, ChKLastEtat.IsChecked, TxtCorrection.Text, TxtFormatage.Text, TxtPrecision.Text, TxtValueMax.Text, TxtValueMin.Text, TxtValDef.Text)
            SaveInZone()
            FlagChange = True
            RaiseEvent CloseMe(Me)
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uDevice BtnOK_Click: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub SaveInZone()
        Try
            For i As Integer = 0 To ListZone.Items.Count - 1
                Dim stk As StackPanel = ListZone.Items(i)
                Dim x1 As CheckBox = stk.Children.Item(0)
                Dim X2 As CheckBox = stk.Children.Item(1)
                Dim trv As Boolean = False

                For Each dev In myservice.GetDeviceInZone(IdSrv, x1.Uid)
                    If dev IsNot Nothing Then
                        If dev.ID = _DeviceId Then
                            trv = True
                            Exit For
                        End If
                    End If
                Next

                If trv = True And x1.IsChecked = False Then
                    myservice.DeleteDeviceToZone(IdSrv, x1.Uid, _DeviceId)
                Else
                    If trv = True And x1.IsChecked = True Then
                        myservice.AddDeviceToZone(IdSrv, x1.Uid, _DeviceId, X2.IsChecked)
                    Else
                        If trv = False And x1.IsChecked = True Then
                            myservice.AddDeviceToZone(IdSrv, x1.Uid, _DeviceId, X2.IsChecked)
                        End If
                    End If
                End If
            Next

        Catch ex As Exception
            MessageBox.Show("Erreur dans le programme SaveInZone: " & ex.ToString, "Admin", MessageBoxButton.OK, MessageBoxImage.Error)
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
        Try
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
                    StkCde.Height = Double.NaN
                Else
                    StkCde.Height = 0
                End If
            End If
        Catch Ex As Exception
            MessageBox.Show("Erreur lors du changement de type: " & Ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnNewCmd_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnNewCmd.Click
        TxtCmdName.Text = ""
        TxtCmdRepeat.Text = "0"
        TxtCmdData.Text = ""

        FlagNewCmd = True
    End Sub

    Private Sub BtnSaveCmd_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnSaveCmd.Click
        Try
            If IsNumeric(TxtCmdRepeat.Text) = False Then
                MsgBox("Numérique obligatoire pour repeat !!")
                Exit Sub
            End If
            If TxtCmdName.Text = "" Or TxtCmdName.Text = " " Then
                MsgBox("Le nom de la commande est obligatoire !!")
                Exit Sub
            End If

            If FlagNewCmd = True Then 'nouvelle commande
                myservice.SaveDeviceCommandIR(IdSrv, _DeviceId, TxtCmdName.Text, TxtCmdData.Text, TxtCmdRepeat.Text)
            Else 'modifier commande
                myservice.SaveDeviceCommandIR(IdSrv, _DeviceId, TxtCmdName.Text, TxtCmdData.Text, TxtCmdRepeat.Text)
            End If

            ListCmd.Items.Clear()
            Dim x As Object = myservice.ReturnDeviceByID(IdSrv, _DeviceId)
            For i As Integer = 0 To x.listcommandname.count - 1
                ListCmd.Items.Add(x.listcommandname(i))
            Next
            x = Nothing

            FlagNewCmd = False
        Catch Ex As Exception
            MessageBox.Show("Erreur BtnSaveCmd: " & Ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnDelCmd_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDelCmd.Click
        Try
            If ListCmd.SelectedIndex >= 0 Then
                myservice.DeleteDeviceCommandIR(IdSrv, _DeviceId, TxtCmdName.Text)

                TxtCmdName.Text = ""
                TxtCmdData.Text = ""
                TxtCmdRepeat.Text = "0"
                ListCmd.Items.Clear()

                Dim x As Object = myservice.ReturnDeviceByID(IdSrv, _DeviceId)
                For i As Integer = 0 To x.listcommandname.count - 1
                    ListCmd.Items.Add(x.listcommandname(i))
                Next
                x = Nothing

            End If
        Catch Ex As Exception
            MessageBox.Show("Erreur BtnDelCmd: " & Ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnTstCmd_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnTstCmd.Click
        Try
            Dim _Param As New ArrayList
            Dim x As New HoMIDom.HoMIDom.DeviceAction
            Dim y As New HoMIDom.HoMIDom.DeviceAction.Parametre

            y.Value = TxtCmdName.Text

            With x
                .Nom = "SendCommand"
                .Parametres.Add(y)
            End With

            myservice.ExecuteDeviceCommand(IdSrv, _DeviceId, x)
        Catch Ex As Exception
            MessageBox.Show("Erreur BtnTstCmd: " & Ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnLearn_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnLearn.Click
        Try
            TxtCmdData.Text = myservice.StartIrLearning(IdSrv)
        Catch Ex As Exception
            MessageBox.Show("Erreur BtnLearnCmd: " & Ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
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
                    ImgDevice.Source = ConvertArrayToImage(myservice.GetByteFromImage(retour))
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
        Try
            If ListCmd.SelectedIndex < 0 Then Exit Sub
            TxtCmdName.Text = myservice.ReturnDeviceByID(IdSrv, _DeviceId).ListCommandName(ListCmd.SelectedIndex)
            TxtCmdData.Text = myservice.ReturnDeviceByID(IdSrv, _DeviceId).ListCommandData(ListCmd.SelectedIndex)
            TxtCmdRepeat.Text = myservice.ReturnDeviceByID(IdSrv, _DeviceId).ListCommandRepeat(ListCmd.SelectedIndex)
            TxtCmdData.Text = myservice.StartIrLearning(IdSrv)
        Catch Ex As Exception
            MessageBox.Show("Erreur ListCmd_SelectionChanged: " & Ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnRead_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnRead.Click
        If myservice.ReturnDeviceByID(IdSrv, _DeviceId).Enable = False Then
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


    Private Sub UnloadControl(ByVal MyControl As Object)
        For i As Integer = 0 To Window1.CanvasUser.Children.Count - 1
            If Window1.CanvasUser.Children.Item(i).Uid = MyControl.uid Then
                Window1.CanvasUser.Children.RemoveAt(i)
                Exit Sub
            End If
        Next

    End Sub

    Private Sub BtnSave_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnSave.Click
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

            TxtRefresh.Text = Replace(TxtRefresh.Text, ".", ",")

            Dim _driverid As String = ""
            For i As Integer = 0 To myservice.GetAllDrivers(IdSrv).Count - 1 'Window1.Obj.Drivers.Count - 1
                If myservice.GetAllDrivers(IdSrv).Item(i).Nom = CbDriver.Text Then
                    _driverid = myservice.GetAllDrivers(IdSrv).Item(i).ID
                    Exit For
                End If
            Next
            myservice.SaveDevice(IdSrv, _DeviceId, TxtNom.Text, TxtAdresse1.Text, ChkEnable.IsChecked, ChKSolo.IsChecked, _driverid, CbType.Text, TxtRefresh.Text, TxtAdresse2.Text, ImgDevice.Tag, TxtModele.Text, TxtDescript.Text, TxtLastChangeDuree.Text)
            SaveInZone()

            BtnRead.Visibility = Windows.Visibility.Visible

        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uDevice BtnSave_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub ChkElement_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        For Each stk As StackPanel In ListZone.Items
            Dim x As CheckBox = stk.Children.Item(0)
            If x.IsChecked = True Then
                stk.Children.Item(1).Visibility = Windows.Visibility.Visible
            Else
                stk.Children.Item(1).Visibility = Windows.Visibility.Hidden
            End If
        Next
    End Sub

    Private Sub CbDriver_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles CbDriver.MouseLeave
        If CbDriver.Text <> tmp Then
            tmp = CbDriver.Text
            MaJDriver()
        End If
    End Sub

    Dim tmp As String = ""

    Private Sub CbDriver_SelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles CbDriver.SelectionChanged
        tmp = CbDriver.Text
        MaJDriver()
    End Sub

    Private Sub MaJDriver()
        'If CbDriver.SelectedIndex < 0 Then Exit Sub

        'For i As Integer = 0 To myService.GetAllDrivers(IdSrv).Count - 1
        _Driver = myService.GetAllDrivers(IdSrv).Item(CbDriver.SelectedIndex)
        If _Driver IsNot Nothing Then 'myService.GetAllDrivers(IdSrv).Item(i).Nom = CbDriver.Text Then
            '_Driver = myService.GetAllDrivers(IdSrv).Item(i)
            If _Driver.LabelsDevice.Count > 0 Then
                For k As Integer = 0 To _Driver.LabelsDevice.Count - 1
                    Select Case UCase(_Driver.LabelsDevice.Item(k).NomChamp)
                        Case "ADRESSE1"
                            If _Driver.LabelsDevice.Item(k).LabelChamp = "@" Then
                                TxtAdresse1.Visibility = Windows.Visibility.Hidden
                                Label6.Visibility = Windows.Visibility.Hidden
                            Else
                                Label6.Content = _Driver.LabelsDevice.Item(k).LabelChamp
                                TxtAdresse1.ToolTip = _Driver.LabelsDevice.Item(k).Tooltip
                                TxtAdresse1.Visibility = Windows.Visibility.Visible
                                Label6.Visibility = Windows.Visibility.Visible
                            End If
                        Case "ADRESSE2"
                            If _Driver.LabelsDevice.Item(k).LabelChamp = "@" Then
                                TxtAdresse2.Visibility = Windows.Visibility.Hidden
                                Label7.Visibility = Windows.Visibility.Hidden
                            Else
                                Label7.Content = _Driver.LabelsDevice.Item(k).LabelChamp
                                TxtAdresse2.ToolTip = _Driver.LabelsDevice.Item(k).Tooltip
                                TxtAdresse2.Visibility = Windows.Visibility.Visible
                                Label7.Visibility = Windows.Visibility.Visible
                            End If
                        Case "SOLO"
                            If _Driver.LabelsDevice.Item(k).LabelChamp = "@" Then
                                ChKSolo.Visibility = Windows.Visibility.Hidden
                            Else
                                ChKSolo.ToolTip = _Driver.LabelsDevice.Item(k).Tooltip
                                ChKSolo.Visibility = Windows.Visibility.Visible
                            End If
                        Case "REFRESH"
                            If _Driver.LabelsDevice.Item(k).LabelChamp = "@" Then
                                TxtRefresh.Visibility = Windows.Visibility.Hidden
                                Label9.Visibility = Windows.Visibility.Hidden
                            Else
                                Label9.Content = _Driver.LabelsDevice.Item(k).LabelChamp
                                TxtRefresh.ToolTip = _Driver.LabelsDevice.Item(k).Tooltip
                                TxtRefresh.Visibility = Windows.Visibility.Visible
                                Label9.Visibility = Windows.Visibility.Visible
                            End If
                        Case "LASTCHANGEDUREE"
                            If _Driver.LabelsDevice.Item(k).LabelChamp = "@" Then
                                TxtLastChangeDuree.Visibility = Windows.Visibility.Hidden
                                Label19.Visibility = Windows.Visibility.Hidden
                            Else
                                Label19.Content = _Driver.LabelsDevice.Item(k).LabelChamp
                                TxtLastChangeDuree.ToolTip = _Driver.LabelsDevice.Item(k).Tooltip
                                TxtLastChangeDuree.Visibility = Windows.Visibility.Visible
                                Label19.Visibility = Windows.Visibility.Visible
                            End If
                        Case "MODELE"
                            If _Driver.LabelsDevice.Item(k).LabelChamp = "@" Then
                                TxtModele.Visibility = Windows.Visibility.Hidden
                                TxtModele.Tag = 0
                                TxtModele.Width = 0
                                TxtModele2.Visibility = Windows.Visibility.Hidden
                                TxtModele2.Tag = 0
                                TxtModele2.Width = 215
                                Label8.Visibility = Windows.Visibility.Hidden
                            Else
                                Label8.Visibility = Windows.Visibility.Visible
                                If _Driver.LabelsDevice.Item(k).LabelChamp <> "" Then Label8.Content = _Driver.LabelsDevice.Item(k).LabelChamp

                                If _Driver.LabelsDevice.Item(k).Parametre <> "" Then
                                    TxtModele.Items.Clear()
                                    Dim a() As String = _Driver.LabelsDevice.Item(k).Parametre.Split("|")
                                    If a.Length > 0 Then
                                        For g As Integer = 0 To a.Length - 1
                                            TxtModele.Items.Add(a(g))
                                        Next
                                        TxtModele.IsEditable = False
                                        TxtModele.Visibility = Windows.Visibility.Visible
                                        TxtModele.Tag = 1
                                        TxtModele.Width = 215
                                        TxtModele2.Visibility = Windows.Visibility.Hidden
                                        TxtModele2.Tag = 0
                                        TxtModele2.Width = 0
                                    Else
                                        TxtModele.Visibility = Windows.Visibility.Hidden
                                        TxtModele.Tag = 0
                                        TxtModele.Width = 0
                                        TxtModele2.Visibility = Windows.Visibility.Visible
                                        TxtModele2.Tag = 1
                                        TxtModele2.Width = 215
                                        TxtModele2.Text = a(0)
                                    End If
                                Else
                                    TxtModele.Visibility = Windows.Visibility.Hidden
                                    TxtModele.Tag = 0
                                    TxtModele.Width = 0
                                    TxtModele2.Visibility = Windows.Visibility.Visible
                                    TxtModele2.Tag = 1
                                    TxtModele2.Width = 215
                                End If
                            End If
                    End Select
                Next
            End If
            'Exit For
        End If
        'Next
    End Sub
End Class
