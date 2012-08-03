﻿Imports HoMIDom.HoMIDom.Device
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

            x = myservice.ReturnDriverByID(IdSrv, DriverId) 'Window1.Obj.ReturnDeviceByID(DeviceId)

            If x IsNot Nothing Then 'on a trouvé le device
                'on cache certains champs si leur valeur est @
                If x.IP_TCP = "@" Then
                    Label4.Visibility = Windows.Visibility.Collapsed
                    TxtAdrTCP.Visibility = Windows.Visibility.Collapsed
                Else
                    Label4.Visibility = Windows.Visibility.Visible
                    TxtAdrTCP.Visibility = Windows.Visibility.Visible
                End If
                If x.Port_TCP = "@" Then
                    Label5.Visibility = Windows.Visibility.Collapsed
                    TxtPortTCP.Visibility = Windows.Visibility.Collapsed
                Else
                    Label5.Visibility = Windows.Visibility.Visible
                    TxtPortTCP.Visibility = Windows.Visibility.Visible
                End If
                If x.IP_UDP = "@" Then
                    Label7.Visibility = Windows.Visibility.Collapsed
                    TxtAdrUDP.Visibility = Windows.Visibility.Collapsed
                Else
                    Label7.Visibility = Windows.Visibility.Visible
                    TxtAdrUDP.Visibility = Windows.Visibility.Visible
                End If
                If x.Port_UDP = "@" Then
                    Label16.Visibility = Windows.Visibility.Collapsed
                    TxtPortUDP.Visibility = Windows.Visibility.Collapsed
                Else
                    Label16.Visibility = Windows.Visibility.Visible
                    TxtPortUDP.Visibility = Windows.Visibility.Visible
                End If
                If x.COM = "@" Then
                    Label10.Visibility = Windows.Visibility.Collapsed
                    TxtCom.Visibility = Windows.Visibility.Collapsed
                    rectcom1.Visibility = Windows.Visibility.Collapsed
                    rectcom2.Visibility = Windows.Visibility.Collapsed
                Else
                    Label10.Visibility = Windows.Visibility.Visible
                    TxtCom.Visibility = Windows.Visibility.Visible
                End If
                If x.Modele = "@" Then
                    Label8.Visibility = Windows.Visibility.Collapsed
                    TxtModele.Visibility = Windows.Visibility.Collapsed
                Else
                    Label8.Visibility = Windows.Visibility.Visible
                    TxtModele.Visibility = Windows.Visibility.Visible
                End If
                If IsNumeric(x.Refresh) = False Then
                    Label9.Visibility = Windows.Visibility.Collapsed
                    TxtRefresh.Visibility = Windows.Visibility.Collapsed
                Else
                    Label9.Visibility = Windows.Visibility.Visible
                    TxtRefresh.Visibility = Windows.Visibility.Visible
                End If

                'on affiche les parametres avancées
                If x.Parametres IsNot Nothing And x.Parametres.Count > 0 Then
                    Label15.Visibility = Windows.Visibility.Visible
                    Label14.Visibility = Windows.Visibility.Visible
                    TxtParam.Visibility = Windows.Visibility.Visible
                    CbParam.Visibility = Windows.Visibility.Visible
                    BtnOkParam.Visibility = Windows.Visibility.Visible
                    CbParam.Items.Clear()
                    For k As Integer = 0 To x.Parametres.Count - 1
                        CbParam.Items.Add(x.Parametres.Item(k).Nom)
                        _ListParam.Add(x.Parametres.Item(k).Valeur)
                    Next
                Else
                    Label15.Visibility = Windows.Visibility.Collapsed
                    Label14.Visibility = Windows.Visibility.Collapsed
                    TxtParam.Visibility = Windows.Visibility.Collapsed
                    CbParam.Visibility = Windows.Visibility.Collapsed
                    BtnOkParam.Visibility = Windows.Visibility.Collapsed
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

                If x.LabelsDriver.Count > 0 Then
                    For k As Integer = 0 To x.LabelsDriver.Count - 1
                        Select Case x.LabelsDriver.Item(k).NomChamp
                            Case "HELP"
                                BtnHelp.Visibility = Windows.Visibility.Visible
                                BtnHelp.ToolTip = x.LabelsDriver.Item(k).Tooltip
                        End Select
                    Next
                End If

                'Fonctions avancées
                If x.DeviceAction.Count = 0 Then
                    Label12.Visibility = Windows.Visibility.Collapsed
                    BtnAv.Visibility = Windows.Visibility.Collapsed
                    GroupBox1.Visibility = Windows.Visibility.Collapsed
                End If

                If x.Picture <> "" And x.Picture <> " " Then
                    ImgDevice.Source = ConvertArrayToImage(myService.GetByteFromImage(x.Picture))
                    ImgDevice.Tag = x.Picture
                End If

                'si c'est le driver virtuel, on cache certains champs
                If x.Nom = "Virtuel" Then
                    Label1.Content = "Driver SYSTEME"
                    ChkEnable.Visibility = Windows.Visibility.Collapsed
                    CbStartAuto.Visibility = Windows.Visibility.Collapsed
                    Label8.Visibility = Windows.Visibility.Collapsed
                    TxtModele.Visibility = Windows.Visibility.Collapsed
                    Label9.Visibility = Windows.Visibility.Collapsed
                    TxtRefresh.Visibility = Windows.Visibility.Collapsed
                    Label11.Visibility = Windows.Visibility.Collapsed
                    TxtVersion.Visibility = Windows.Visibility.Collapsed
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
            'verification of value
            Dim verif As Boolean = True
            'verif of PORT_COM
            If TxtCom.Text <> "" And TxtCom.Text <> "@" Then
                If Not (TxtCom.Text.StartsWith("COM") Or TxtCom.Text.StartsWith("USB")) Then
                    verif = False
                    MessageBox.Show("Le Champ COM doit être configuré avec COMx ou USBx (ou x est un entier)", "Erreur port COM", MessageBoxButton.OK, MessageBoxImage.Error)
                End If
            End If

            'if all OK, save the driver and close window
            If verif = True Then
                myService.SaveDriver(IdSrv, _DriverId, TxtNom.Text, ChkEnable.IsChecked, CbStartAuto.IsChecked, TxtAdrTCP.Text, TxtPortTCP.Text, TxtAdrUDP.Text, TxtPortUDP.Text, TxtCom.Text, TxtRefresh.Text, ImgDevice.Tag, TxtModele.Text, _ListParam)
                FlagChange = True
                RaiseEvent CloseMe(Me)
            End If
        Catch ex As Exception
            MessageBox.Show(BtnHelp.ToolTip, "Aide", MessageBoxButton.OK, MessageBoxImage.Question)
            RaiseEvent CloseMe(Me)
        End Try
    End Sub

    Private Sub TxtRefresh_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtRefresh.TextChanged
        If TxtRefresh.Text <> "" And IsNumeric(TxtRefresh.Text) = False Then
            MessageBox.Show("Veuillez saisir une valeur numérique")
            TxtRefresh.Text = 0
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
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub ImgDevice_MouseLeftButtonDown: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
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

            myservice.ExecuteDriverCommand(IdSrv, _DriverId, a)
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

    Private Sub BtnHelp_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnHelp.Click
        MessageBox.Show(BtnHelp.ToolTip, "Aide", MessageBoxButton.OK, MessageBoxImage.Question)
    End Sub
End Class