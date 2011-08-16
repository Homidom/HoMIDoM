Public Class uTriggerDevice
    Public Event CloseMe(ByVal MyObject As Object)

    Public Enum EAction
        Nouveau
        Modifier
    End Enum

    Dim _Action As EAction
    Dim _TriggerId As String
    Dim _ListDeviceId As New ArrayList
    Dim _ListMacro As New ArrayList

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me)
    End Sub

    Public Sub New(ByVal Action As EAction, ByVal TriggerId As String)

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        _Action = Action
        _TriggerId = TriggerId


        For i As Integer = 0 To Window1.myService.GetAllDevices.Count - 1
            _ListDeviceId.Add(Window1.myService.GetAllDevices.Item(i))
            CbDevice.Items.Add(_ListDeviceId.Item(i).Name)
        Next

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        If _Action = EAction.Nouveau Then 'Nouveau Trigger

        Else 'Modifier Trigger
            Dim x As HoMIDom.HoMIDom.Trigger = Window1.myService.ReturnTriggerById(_TriggerId)

            If x IsNot Nothing Then
                TxtNom.Text = x.Nom
                ChkEnable.IsChecked = x.Enable
                TxtDescription.Text = x.Description
                For i As Integer = 0 To _ListDeviceId.Count - 1
                    If _ListDeviceId.Item(i).ID = x.ConditionDeviceId Then
                        CbDevice.SelectedIndex = i
                        CbProperty.Text = x.ConditionDeviceProperty
                        Exit For
                    End If
                Next
                _ListMacro = x.ListMacro

                If _ListMacro IsNot Nothing Then
                    For i As Integer = 0 To _ListMacro.Count - 1
                        ListBox1.Items.Add(Window1.myService.ReturnMacroById(_ListMacro.Item(i)).Nom)
                    Next
                End If
            End If

        End If
    End Sub

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        If TxtNom.Text = "" Or CbDevice.SelectedIndex < 0 Then
            MessageBox.Show("Le nom du trigger ou le device sont obligatoires!", "Trigger", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        End If

        If _Action = EAction.Nouveau Then
            Window1.myService.SaveTrigger("", TxtNom.Text, ChkEnable.IsChecked, HoMIDom.HoMIDom.Trigger.TypeTrigger.DEVICE, TxtDescription.Text, "", _ListDeviceId(CbDevice.SelectedIndex).id, CbProperty.Text, _ListMacro)
        Else
            Window1.myService.SaveTrigger(_TriggerId, TxtNom.Text, ChkEnable.IsChecked, HoMIDom.HoMIDom.Trigger.TypeTrigger.DEVICE, TxtDescription.Text, "", _ListDeviceId(CbDevice.SelectedIndex).id, CbProperty.Text, _ListMacro)
        End If
    End Sub

    Private Sub CbDevice_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles CbDevice.SelectionChanged
        Try
            If CbDevice.SelectedIndex >= 0 Then
                CbProperty.Items.Clear()

                Select Case _ListDeviceId.Item(CbDevice.SelectedIndex).Type
                    Case 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27
                        CbProperty.Items.Add("Value")
                    Case 17
                        CbProperty.Items.Add("Value")
                        CbProperty.Items.Add("ConditionActuel")
                        CbProperty.Items.Add("TemperatureActuel")
                        CbProperty.Items.Add("HumiditeActuel")
                        CbProperty.Items.Add("VentActuel")
                        CbProperty.Items.Add("JourToday")
                        CbProperty.Items.Add("MinToday")
                        CbProperty.Items.Add("MaxToday")
                        CbProperty.Items.Add("ConditionToday")
                        CbProperty.Items.Add("JourJ1")
                        CbProperty.Items.Add("MinJ1")
                        CbProperty.Items.Add("MaxJ1")
                        CbProperty.Items.Add("ConditionJ1")
                        CbProperty.Items.Add("JourJ2")
                        CbProperty.Items.Add("MinJ2")
                        CbProperty.Items.Add("MaxJ2")
                        CbProperty.Items.Add("ConditionJ2")
                        CbProperty.Items.Add("JourJ3")
                        CbProperty.Items.Add("MinJ3")
                        CbProperty.Items.Add("MaxJ3")
                        CbProperty.Items.Add("ConditionJ3")
                End Select
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur: " & ex.Message)
        End Try
    End Sub

    Private Sub ListBox1_DragOver(ByVal sender As Object, ByVal e As System.Windows.DragEventArgs) Handles ListBox1.DragOver
        If e.Data.GetDataPresent(GetType(String)) Then
            e.Effects = DragDropEffects.Copy
        Else
            e.Effects = DragDropEffects.None
        End If
    End Sub

    Private Sub ListBox1_Drop(ByVal sender As Object, ByVal e As System.Windows.DragEventArgs) Handles ListBox1.Drop
        If e.Data.GetDataPresent(GetType(String)) Then
            e.Effects = DragDropEffects.Copy

            Dim uri As String = e.Data.GetData(GetType(String)).ToString
            _ListMacro.Add(uri)
            ListBox1.Items.Add(Window1.myService.ReturnMacroById(Uid).Nom)
        Else
            e.Effects = DragDropEffects.None
        End If
    End Sub
End Class
