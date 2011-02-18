Imports HoMIDom.HoMIDom.Device
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
            For i As Integer = 0 To Window1.Obj.Drivers.Count - 1
                CbDriver.Items.Add(Window1.Obj.Drivers.Item(i).nom)
            Next

            If Action = EAction.Nouveau Then 'Nouveau Device
                ImgDevice.Tag = " "
            Else 'Modification d'un Device
                Dim x As Object = Window1.Obj.ReturnDeviceByID(DeviceId)

                If x IsNot Nothing Then 'on a trouvé le device
                    TxtNom.Text = x.name
                    TxtDescript.Text = x.description
                    ChkEnable.IsChecked = x.Enable
                    ChKSolo.IsChecked = x.Solo
                    CbType.SelectedValue = x.type
                    CbType.IsEnabled = False

                    For j As Integer = 0 To Window1.Obj.Drivers.Count - 1
                        If Window1.Obj.Drivers.Item(j).id = x.driverid Then
                            CbDriver.SelectedValue = Window1.Obj.Drivers.Item(j).nom
                            Exit For
                        End If
                    Next

                    TxtAdresse1.Text = x.adresse1
                    TxtAdresse2.Text = x.adresse2
                    TxtModele.Text = x.modele
                    TxtRefresh.Text = x.refresh
                    TxtLastChangeDuree.Text = x.LastChangeDuree

                    If x.Picture <> "" And File.Exists(x.picture) = True Then
                        Dim bmpImage As New BitmapImage()
                        bmpImage.BeginInit()
                        bmpImage.UriSource = New Uri(x.picture, UriKind.Absolute)
                        bmpImage.EndInit()
                        ImgDevice.Source = bmpImage
                        ImgDevice.Tag = x.picture
                    End If

                    'Gestion si Device avec Value
                    If x.Type = "TEMPERATURE" _
                                       Or x.Type = "HUMIDITE" _
                                       Or x.Type = "TEMPERATURECONSIGNE" _
                                       Or x.Type = "ENERGIETOTALE" _
                                       Or x.Type = "ENERGIEINSTANTANEE" _
                                       Or x.Type = "PLUIETOTAL" _
                                       Or x.Type = "PLUIECOURANT" _
                                       Or x.Type = "VITESSEVENT" _
                                       Or x.Type = "UV" _
                                       Or x.Type = "HUMIDITE" _
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
                        TxtValueMin.Text = x.valueMin
                        TxtValDef.Visibility = Windows.Visibility.Visible
                        TxtValDef.Text = x.valueDef
                        Label10.Visibility = Windows.Visibility.Visible
                        Label11.Visibility = Windows.Visibility.Visible
                        Label12.Visibility = Windows.Visibility.Visible
                        Label13.Visibility = Windows.Visibility.Visible
                        Label14.Visibility = Windows.Visibility.Visible
                        Label15.Visibility = Windows.Visibility.Visible
                    End If

                    If x.Type = "MULTIMEDIA" Then
                        GroupBox1.Visibility = Windows.Visibility.Visible
                        ListCmd.Items.Clear()
                        For i As Integer = 0 To x.listcommandname.count - 1
                            ListCmd.Items.Add(x.listcommandname(i))
                        Next
                        x = Nothing
                    Else
                        GroupBox1.Visibility = Windows.Visibility.Hidden
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
        For i As Integer = 0 To Window1.Obj.Drivers.Count - 1
            If Window1.Obj.Drivers.Item(i).nom = CbDriver.Text Then
                _driverid = Window1.Obj.Drivers.Item(i).id
                Exit For
            End If
        Next
        Window1.Obj.SaveDevice(_DeviceId, TxtNom.Text, TxtAdresse1.Text, ChkEnable.IsChecked, ChKSolo.IsChecked, _driverid, CbType.Text, TxtRefresh.Text, TxtAdresse2.Text, ImgDevice.Tag, TxtModele.Text, TxtDescript.Text, TxtLastChangeDuree.Text)
        RaiseEvent CloseMe(Me)
    End Sub

    Private Sub TxtRefresh_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtRefresh.TextChanged
        If TxtRefresh.Text <> "" And IsNumeric(TxtRefresh.Text) = False Then
            MessageBox.Show("Veuillez saisir une valeur numérique")
            TxtRefresh.Text = 0
        End If
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

            If CbType.SelectedValue = "MULTIMEDIA" Then
                GroupBox1.Visibility = Windows.Visibility.Visible
            Else
                GroupBox1.Visibility = Windows.Visibility.Hidden
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
            Window1.Obj.SaveDeviceCommandIR(_DeviceId, TxtCmdName.Text, TxtCmdData.Text, TxtCmdRepeat.Text)
        Else 'modifier commande
            Window1.Obj.SaveDeviceCommandIR(_DeviceId, TxtCmdName.Text, TxtCmdData.Text, TxtCmdRepeat.Text)
        End If

        ListCmd.Items.Clear()
        Dim x As Object = Window1.Obj.ReturnDeviceByID(_DeviceId)
        For i As Integer = 0 To x.listcommandname.count - 1
            ListCmd.Items.Add(x.listcommandname(i))
        Next
        x = Nothing

        FlagNewCmd = False
    End Sub

    Private Sub BtnDelCmd_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDelCmd.Click
        If ListCmd.SelectedIndex >= 0 Then
            Window1.Obj.DeleteDeviceCommandIR(_DeviceId, TxtCmdName.Text)

            TxtCmdName.Text = ""
            TxtCmdData.Text = ""
            TxtCmdRepeat.Text = "0"
            ListCmd.Items.Clear()

            Dim x As Object = Window1.Obj.ReturnDeviceByID(_DeviceId)
            For i As Integer = 0 To x.listcommandname.count - 1
                ListCmd.Items.Add(x.listcommandname(i))
            Next
            x = Nothing

        End If
    End Sub

    Private Sub BtnTstCmd_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnTstCmd.Click
        Dim x As Object = Window1.Obj.ReturnDeviceByID(_DeviceId)

        If x IsNot Nothing Then
            x.sendcommand(TxtCmdName.Text)
        End If
    End Sub

    Private Sub BtnLearn_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnLearn.Click
        TxtCmdData.Text = Window1.Obj.StartIrLearning
    End Sub

    Private Sub TxtLastChangeDuree_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtLastChangeDuree.TextChanged
        If TxtLastChangeDuree.Text <> "" And IsNumeric(TxtLastChangeDuree.Text) = False Then
            MessageBox.Show("Veuillez saisir une valeur numérique")
            TxtLastChangeDuree.Text = 0
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

    Private Sub ListCmd_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles ListCmd.SelectionChanged
        If ListCmd.SelectedIndex < 0 Then Exit Sub
        TxtCmdName.Text = Window1.Obj.ReturnDeviceByID(_DeviceId).listcommandname(ListCmd.SelectedIndex)
        TxtCmdData.Text = Window1.Obj.ReturnDeviceByID(_DeviceId).listcommanddata(ListCmd.SelectedIndex)
        TxtCmdRepeat.Text = Window1.Obj.ReturnDeviceByID(_DeviceId).listcommandrepeat(ListCmd.SelectedIndex)
    End Sub

    Private Sub BtnRead_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnRead.Click
        Dim x As Object = Nothing
        Try
            x = Window1.Obj.ReturnDeviceByID(_DeviceId)
        Catch ex As Exception
            MessageBox.Show("Error Read GET device" & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
        Try
            MessageBox.Show("test " & x.Adresse1, "tes", MessageBoxButton.OK, MessageBoxImage.Error)
            If x IsNot Nothing Then
                x.Read()
            Else
                MessageBox.Show("Impossible d'exécuter le read car le device n'a pas été trouvé!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
            End If
        Catch ex As Exception
            MessageBox.Show("Error Read action" & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnWrite_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnWrite.Click
        Try
            Dim x As Object = Window1.Obj.ReturnDeviceByID(_DeviceId)
            If x IsNot Nothing Then
                If TxtCmd.Text = "" Then
                    MessageBox.Show("Le nom de la commande est obligatoire!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
                    Exit Sub
                End If
                x.TestWrite(TxtCmd.Text, TxtP1.Text, TxtP2.Text)
            Else
                MessageBox.Show("Impossible d'exécuter le write car le device n'a pas été trouvé!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
            End If
        Catch ex As Exception
            MessageBox.Show("Error Write device" & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class
