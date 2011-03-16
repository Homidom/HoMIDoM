Imports HoMIDom.HoMIDom.Api

Partial Public Class uTestDevice
    Dim _DeviceId As String
    Dim _Device As HoMIDom.HoMIDom.TemplateDevice

    Public Event CloseMe(ByVal MyObject As Object)

    Private Sub BtnTest_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnTest.Click
        Try
            Dim x As New HoMIDom.HoMIDom.DeviceAction

            x.Nom = CbCmd.Text

            If TxtP1.Text <> "" Then
                Dim y As New HoMIDom.HoMIDom.DeviceAction.Parametre
                y.Value = TxtP1.Text
                x.Parametres.Add(y)
            End If
            If TxtP2.Text <> "" Then
                Dim y As New HoMIDom.HoMIDom.DeviceAction.Parametre
                y.Value = TxtP2.Text
                x.Parametres.Add(y)
            End If
            If TxtP3.Text <> "" Then
                Dim y As New HoMIDom.HoMIDom.DeviceAction.Parametre
                y.Value = TxtP3.Text
                x.Parametres.Add(y)
            End If
            If TxtP4.Text <> "" Then
                Dim y As New HoMIDom.HoMIDom.DeviceAction.Parametre
                y.Value = TxtP4.Text
                x.Parametres.Add(y)
            End If
            If TxtP5.Text <> "" Then
                Dim y As New HoMIDom.HoMIDom.DeviceAction.Parametre
                y.Value = TxtP5.Text
                x.Parametres.Add(y)
            End If

            Window1.myService.ExecuteDeviceCommand(_DeviceId, x)
        Catch ex As Exception
            MessageBox.Show("Erreur lors du test: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Public Sub New(ByVal DeviceId As String)

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        _DeviceId = DeviceId

        _Device = Window1.myService.ReturnDeviceByID(_DeviceId)

        If _Device IsNot Nothing Then

            For i As Integer = 0 To _Device.DeviceAction.Count - 1
                CbCmd.Items.Add(_Device.DeviceAction.Item(i).Nom)
                If _Device.DeviceAction.Item(i).Parametres.Count > 5 Then
                    MessageBox.Show("Seuls 5 paramètres sont acceptés veuillez contacter l'administrateur !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                    RaiseEvent CloseMe(Me)
                End If
            Next

        Else
            MessageBox.Show("Le Device est inconnu !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            RaiseEvent CloseMe(Me)
        End If

    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me)
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
        For j As Integer = 0 To _Device.DeviceAction.Item(Idx).Parametres.Count - 1

            Select Case j
                Case 0
                    LblP1.Content = _Device.DeviceAction.Item(Idx).Parametres.Item(j).Nom & " :"
                    LblP1.Visibility = Windows.Visibility.Visible
                    TxtP1.ToolTip = _Device.DeviceAction.Item(Idx).Parametres.Item(j).Type
                    TxtP1.Visibility = Windows.Visibility.Visible
                Case 1
                    LblP2.Content = _Device.DeviceAction.Item(Idx).Parametres.Item(j).Nom & " :"
                    LblP2.Visibility = Windows.Visibility.Visible
                    TxtP2.ToolTip = _Device.DeviceAction.Item(Idx).Parametres.Item(j).Type
                    TxtP2.Visibility = Windows.Visibility.Visible
                Case 2
                    LblP3.Content = _Device.DeviceAction.Item(Idx).Parametres.Item(j).Nom & " :"
                    LblP3.Visibility = Windows.Visibility.Visible
                    TxtP3.ToolTip = _Device.DeviceAction.Item(Idx).Parametres.Item(j).Type
                    TxtP3.Visibility = Windows.Visibility.Visible
                Case 3
                    LblP4.Content = _Device.DeviceAction.Item(Idx).Parametres.Item(j).Nom & " :"
                    LblP4.Visibility = Windows.Visibility.Visible
                    TxtP4.ToolTip = _Device.DeviceAction.Item(Idx).Parametres.Item(j).Type
                    TxtP4.Visibility = Windows.Visibility.Visible
                Case 4
                    LblP5.Content = _Device.DeviceAction.Item(Idx).Parametres.Item(j).Nom & " :"
                    LblP5.Visibility = Windows.Visibility.Visible
                    TxtP5.ToolTip = _Device.DeviceAction.Item(Idx).Parametres.Item(j).Type
                    TxtP5.Visibility = Windows.Visibility.Visible
            End Select
        Next
    End Sub

End Class
