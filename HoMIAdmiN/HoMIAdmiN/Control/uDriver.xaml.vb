Imports HoMIDom.HoMIDom.Device
Imports HoMIDom.HoMIDom.Api
Imports System.IO
Imports System.IO.Ports
Imports System.Text.RegularExpressions
Imports System.Net

Partial Public Class uDriver
    '--- Variables ------------------
    Public Event CloseMe(ByVal MyObject As Object)
    Dim _DriverId As String = "" 'Id du device à modifier
    Dim x As HoMIDom.HoMIDom.TemplateDriver = Nothing
    Dim _ListParam As New ArrayList

    Public Sub New(ByVal DriverId As String)

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()
        Try
            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            _DriverId = DriverId

            x = myService.ReturnDriverByID(IdSrv, DriverId) 'Window1.Obj.ReturnDeviceByID(DeviceId)

            If x IsNot Nothing Then 'on a trouvé le driver
                'on cache certains champs si leur valeur est @
                If x.IP_TCP = "@" Then
                    StkAdrIP.Visibility = Windows.Visibility.Collapsed
                Else
                    StkAdrIP.Visibility = Windows.Visibility.Visible
                End If
                If x.Port_TCP = "@" Then
                    StkPortIP.Visibility = Windows.Visibility.Collapsed
                Else
                    StkPortIP.Visibility = Windows.Visibility.Visible
                End If
                If x.IP_UDP = "@" Then
                    StkAdrUDP.Visibility = Windows.Visibility.Collapsed
                Else
                    StkAdrUDP.Visibility = Windows.Visibility.Visible
                End If
                If x.Port_UDP = "@" Then
                    StkPortUDP.Visibility = Windows.Visibility.Collapsed
                Else
                    StkPortUDP.Visibility = Windows.Visibility.Visible
                End If
                If x.COM = "@" Then
                    StkCom.Visibility = Windows.Visibility.Collapsed
                Else
                    StkCom.Visibility = Windows.Visibility.Visible

                    For Each serialPortName As String In myService.GetPortComDispo
                        TxtCom.Items.Add(serialPortName)
                    Next
                End If
                If x.Modele = "@" Then
                    StkModele.Visibility = Windows.Visibility.Collapsed
                Else
                    StkModele.Visibility = Windows.Visibility.Visible
                End If
                If IsNumeric(x.Refresh) = False Then
                    StkRefresh.Visibility = Windows.Visibility.Collapsed
                Else
                    StkRefresh.Visibility = Windows.Visibility.Visible
                End If

                'on affiche les parametres avancées
                If x.Parametres IsNot Nothing And x.Parametres.Count > 0 Then
                    StkParam.Visibility = Windows.Visibility.Visible
                    CbParam.Items.Clear()
                    For k As Integer = 0 To x.Parametres.Count - 1
                        CbParam.Items.Add(x.Parametres.Item(k).Nom)
                        _ListParam.Add(x.Parametres.Item(k).Valeur)
                    Next
                Else
                    StkParam.Visibility = Windows.Visibility.Collapsed
                End If

                TxtNom.Text = x.Nom
                TxtDescript.Text = x.Description
                ChkEnable.IsChecked = x.Enable
                CbStartAuto.IsChecked = x.StartAuto
                CbAutoDiscover.IsChecked = x.AutoDiscover
                TxtProtocol.Text = x.Protocol
                TxtAdrTCP.Text = x.IP_TCP
                TxtPortTCP.Text = x.Port_TCP
                TxtAdrUDP.Text = x.IP_UDP
                TxtPortUDP.Text = x.Port_UDP
                TxtCom.Text = x.COM
                TxtRefresh.Text = x.Refresh
                TxtVersion.Text = x.Version
                TxtModele.Text = x.Modele

                BtnHelp.Visibility = Windows.Visibility.Visible
                BtnHelp.ToolTip = "Afficher l'aide en ligne au format PDF"
                'If x.LabelsDriver.Count > 0 Then
                '    For k As Integer = 0 To x.LabelsDriver.Count - 1
                '        Select Case x.LabelsDriver.Item(k).NomChamp
                '            Case "HELP"
                '                BtnHelp.Visibility = Windows.Visibility.Visible
                '                BtnHelp.ToolTip = x.LabelsDriver.Item(k).Tooltip
                '        End Select
                '    Next
                'End If

                'Fonctions avancées
                If x.DeviceAction.Count = 0 Then
                    Label12.Visibility = Windows.Visibility.Collapsed
                    BtnAv.Visibility = Windows.Visibility.Collapsed
                    GroupBox1.Visibility = Windows.Visibility.Collapsed
                End If

                If String.IsNullOrEmpty(x.Picture) = False Then
                    ImgDevice.Source = ConvertArrayToImage(myService.GetByteFromImage(x.Picture))
                    ImgDevice.Tag = x.Picture
                End If

                'si c'est le driver virtuel, on cache certains champs
                If x.Nom = "Virtuel" Then
                    Label1.Content = "Driver SYSTEME"
                    'ChkEnable.Visibility = Windows.Visibility.Collapsed
                    'CbStartAuto.Visibility = Windows.Visibility.Collapsed
                    CbAutoDiscover.Visibility = Windows.Visibility.Collapsed
                    StkModele.Visibility = Windows.Visibility.Collapsed
                    StkRefresh.Visibility = Windows.Visibility.Collapsed
                    StkVersion.Visibility = Windows.Visibility.Collapsed
                End If
            End If

        Catch Ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur: " & Ex.Message, "Erreur", "")
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
            If String.IsNullOrEmpty(TxtCom.Text) = False And TxtCom.Text <> "@" Then
                If Not (TxtCom.Text.StartsWith("COM") Or TxtCom.Text.StartsWith("USB")) Then
                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le Champ COM doit être configuré avec COMx ou USBx (ou x est un entier)", "Erreur port COM", "")
                    Exit Sub
                End If
            End If
            If String.IsNullOrEmpty(TxtPortTCP.Text) = False And TxtPortTCP.Text <> "@" Then
                If IsValidPortIP(TxtPortTCP.Text) = False Then
                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le Port TCP doit être compris entre 0 et 65535", "Erreur port COM", "")
                    Exit Sub
                End If
            End If
            If String.IsNullOrEmpty(TxtPortUDP.Text) = False And TxtPortUDP.Text <> "@" Then
                If IsValidPortIP(TxtPortUDP.Text) = False Then
                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le Port UDP doit être compris entre 0 et 65535", "Erreur port COM", "")
                    Exit Sub
                End If
            End If
            If String.IsNullOrEmpty(TxtAdrTCP.Text) = False And TxtAdrTCP.Text <> "@" Then
                If IsValidIP(TxtAdrTCP.Text) = False Then
                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "L'adresse IP doit être au format xx.xx.xx.xx", "Erreur port COM", "")
                    Exit Sub
                End If
            End If
            If String.IsNullOrEmpty(TxtAdrUDP.Text) = False And TxtAdrUDP.Text <> "@" Then
                If IsValidIP(TxtAdrUDP.Text) = False Then
                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "L'adresse UDP doit être au format xx.xx.xx.xx", "Erreur port COM", "")
                    Exit Sub
                End If
            End If

            'if all OK, save the driver and close window
            If verif = True Then
                myService.SaveDriver(IdSrv, _DriverId, TxtNom.Text, ChkEnable.IsChecked, CbStartAuto.IsChecked, TxtAdrTCP.Text, TxtPortTCP.Text, TxtAdrUDP.Text, TxtPortUDP.Text, TxtCom.Text, TxtRefresh.Text, ImgDevice.Tag, TxtModele.Text, CbAutoDiscover.IsChecked, _ListParam)
                FlagChange = True
                ManagerDrivers.LoadDrivers()
                RaiseEvent CloseMe(Me)
            End If
        Catch ex As Exception
            MessageBox.Show(BtnHelp.ToolTip, "Aide", MessageBoxButton.OK, MessageBoxImage.Question)
            RaiseEvent CloseMe(Me)
        End Try
    End Sub

    Private Sub TxtRefresh_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtRefresh.TextChanged
        Try
            If String.IsNullOrEmpty(TxtRefresh.Text) = False And IsNumeric(TxtRefresh.Text) = False Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Veuillez saisir une valeur numérique")
                TxtRefresh.Text = 0
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uDriver TxtRefresh_TextChanged: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Sub UnloadControl(ByVal MyControl As Object)
        Try
            For i As Integer = 0 To Window1.CanvasUser.Children.Count - 1
                If Window1.CanvasUser.Children.Item(i).Uid = MyControl.uid Then
                    Window1.CanvasUser.Children.RemoveAt(i)
                    Exit Sub
                End If
            Next
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uDriver UnloadControl: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Sub BtnAv_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnAv.Click
        Try
            If GroupBox1.Visibility = Windows.Visibility.Collapsed Then
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
                GroupBox1.Visibility = Windows.Visibility.Collapsed
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uDriver BtnAv_Click: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Sub CbCmd_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles CbCmd.MouseLeave
        Try
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

            If String.IsNullOrEmpty(CbCmd.Text) = True Then Exit Sub
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
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uDriver CbCmd_MouseLeave: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Sub BtnTest_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnTest.Click
        Try
            Dim a As New HoMIDom.HoMIDom.DeviceAction

            a.Nom = CbCmd.Text

            If String.IsNullOrEmpty(TxtP1.Text) = False Then
                Dim y As New HoMIDom.HoMIDom.DeviceAction.Parametre
                y.Value = TxtP1.Text
                a.Parametres.Add(y)
            End If
            If String.IsNullOrEmpty(TxtP2.Text) = False Then
                Dim y As New HoMIDom.HoMIDom.DeviceAction.Parametre
                y.Value = TxtP2.Text
                a.Parametres.Add(y)
            End If
            If String.IsNullOrEmpty(TxtP3.Text) = False Then
                Dim y As New HoMIDom.HoMIDom.DeviceAction.Parametre
                y.Value = TxtP3.Text
                a.Parametres.Add(y)
            End If
            If String.IsNullOrEmpty(TxtP4.Text) = False Then
                Dim y As New HoMIDom.HoMIDom.DeviceAction.Parametre
                y.Value = TxtP4.Text
                a.Parametres.Add(y)
            End If
            If String.IsNullOrEmpty(TxtP5.Text) = False Then
                Dim y As New HoMIDom.HoMIDom.DeviceAction.Parametre
                y.Value = TxtP5.Text
                a.Parametres.Add(y)
            End If

            myService.ExecuteDriverCommand(IdSrv, _DriverId, a)
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur lors du test: " & ex.Message, "Erreur", "")
        End Try
    End Sub

    Private Sub CbParam_SelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles CbParam.SelectionChanged
        Try
            If CbParam.SelectedIndex >= 0 Then
                CbParam.ToolTip = x.Parametres.Item(CbParam.SelectedIndex).Description
                TxtParam.Text = _ListParam.Item(CbParam.SelectedIndex)
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uDriver CbParam_SelectionChanged: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Sub BtnOkParam_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOkParam.Click
        Try
            If CbParam.SelectedIndex >= 0 And String.IsNullOrEmpty(TxtParam.Text) = False Then
                _ListParam.Item(CbParam.SelectedIndex) = TxtParam.Text
            Else
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Veuillez sélectionner un paramètre ou saisir sa valeur", "Erreur", "")
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uDriver BtnOkParam_Click: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Sub BtnHelp_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnHelp.Click
        'MessageBox.Show(BtnHelp.ToolTip, "Aide", MessageBoxButton.OK, MessageBoxImage.Question)
        Try
            Dim startInfo As ProcessStartInfo = New ProcessStartInfo()
            startInfo.UseShellExecute = True
            'startInfo.FileName = "http://www.homidom.com/upload/documentation/HoMIDoM-Driver-" & TxtNom.Text & ".pdf"
            startInfo.FileName = "http://www.homidom.com/drivers-c44.html"
            Process.Start(startInfo)
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uDriver BtnHelp_Click: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Function IsValidIP(ByVal addr As String) As Boolean
        Try
            If String.IsNullOrEmpty(addr) = False Then
                Return True
                Exit Function
            End If

            'create our match pattern
            Dim pattern As String = "^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\." & _
            "([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$"
            'create our Regular Expression object
            Dim check As New Text.RegularExpressions.Regex(pattern)
            'boolean variable to hold the status
            Dim valid As Boolean = False
            'check to make sure an ip address was provided
            If String.IsNullOrEmpty(addr) = True Then
                'no address provided so return false
                valid = False
            Else
                'address provided so use the IsMatch Method
                'of the Regular Expression object
                valid = check.IsMatch(addr, 0)
            End If
            'return the results
            Return (valid)
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uDriver IsValidIP: " & ex.ToString, "ERREUR", "")
        End Try
    End Function

    Private Function IsValidPortIP(ByVal addr As String) As Boolean
        Try
            Dim valid As Boolean = False
            Dim _addr As Integer

            If String.IsNullOrEmpty(addr) = True Then
                Return True
                Exit Function
            End If

            If IsNumeric(addr) = False Then
                valid = False
            Else
                _addr = CInt(addr)
                If _addr < 0 Then
                    valid = False
                Else
                    If _addr > 65535 Then
                        valid = False
                    Else
                        valid = True
                    End If
                End If
            End If

            Return valid
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uDriver IsValidPortIP: " & ex.ToString, "ERREUR", "")
        End Try
    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
