Public Class uTriggerDevice
    Public Event CloseMe(ByVal MyObject As Object)

    Dim _Action As EAction
    Dim _TriggerId As String
    Dim _ListMacro As New List(Of String)

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me)
    End Sub

    Public Sub New(ByVal Action As Classe.EAction, ByVal TriggerId As String)
        Try


            ' Cet appel est requis par le concepteur.
            InitializeComponent()

            Mouse.OverrideCursor = Cursors.Wait

            _Action = Action
            _TriggerId = TriggerId

            CbDevice.ItemsSource = myService.GetAllDevices(IdSrv)
            CbDevice.DisplayMemberPath = "Name"

            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            If _Action = EAction.Nouveau Then 'Nouveau Trigger

            Else 'Modifier Trigger
                Dim x As HoMIDom.HoMIDom.Trigger = myService.ReturnTriggerById(IdSrv, _TriggerId)

                If x IsNot Nothing Then
                    TxtNom.Text = x.Nom
                    ChkEnable.IsChecked = x.Enable
                    TxtDescription.Text = x.Description
                    For i As Integer = 0 To CbDevice.Items.Count - 1
                        If CbDevice.Items(i).ID = x.ConditionDeviceId Then
                            CbDevice.SelectedIndex = i
                            CbProperty.Text = x.ConditionDeviceProperty
                            Exit For
                        End If
                    Next
                    _ListMacro = x.ListMacro
                End If

            End If
            RemplirMacro()
            Mouse.OverrideCursor = Nothing
        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'ouverture de la fenêtre triggerdevice, message: " & ex.ToString, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        Try
            If TxtNom.Text = "" Or CbDevice.SelectedIndex < 0 Or CbDevice.SelectedItem Is Nothing Then
                MessageBox.Show("Le nom du trigger ou le device sont obligatoires!", "Trigger", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Exit Sub
            End If

            _ListMacro.Clear()
            For i As Integer = 0 To ListBox1.Items.Count - 1
                Dim x As StackPanel = ListBox1.Items.Item(i)
                Dim w As CheckBox = x.Children.Item(0)
                If w.IsChecked = True Then _ListMacro.Add(ListBox1.Items.Item(i).uid)
            Next

            If _Action = EAction.Nouveau Then
                myservice.SaveTrigger(IdSrv, "", TxtNom.Text, ChkEnable.IsChecked, HoMIDom.HoMIDom.Trigger.TypeTrigger.DEVICE, TxtDescription.Text, "", CbDevice.SelectedItem.id, CbProperty.Text, _ListMacro)
            Else
                myService.SaveTrigger(IdSrv, _TriggerId, TxtNom.Text, ChkEnable.IsChecked, HoMIDom.HoMIDom.Trigger.TypeTrigger.DEVICE, TxtDescription.Text, "", CbDevice.SelectedItem.id, CbProperty.Text, _ListMacro)
            End If

            FlagChange = True
            RaiseEvent CloseMe(Me)
        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'enregistrement du trigger, message: " & ex.ToString, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub CbDevice_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles CbDevice.SelectionChanged
        Try
            If CbDevice.SelectedIndex >= 0 And CbDevice.SelectedItem IsNot Nothing Then
                CbProperty.Items.Clear()

                Select Case CbDevice.SelectedItem.Type
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

                CbProperty.SelectedIndex = 0
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur: " & ex.Message)
        End Try
    End Sub

    'Private Sub UpMacro_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles UpMacro.Click
    '    Try
    '        If ListBox1.SelectedIndex <= 0 Then
    '            MessageBox.Show("Aucune macro sélectionnée ou celle-ci est déjà en 1ère position, veuillez en choisir une à déplacer !", "Trigger", MessageBoxButton.OK, MessageBoxImage.Exclamation)
    '            Exit Sub
    '        Else
    '            Dim i As Integer = ListBox1.SelectedIndex

    '            Dim x As StackPanel = ListBox1.Items.Item(i)
    '            Dim w As CheckBox = x.Children.Item(0)
    '            If w.IsChecked = False Then
    '                MessageBox.Show("Cette macro n'est pas cochée, elle ne peut être déplacée !", "Trigger", MessageBoxButton.OK, MessageBoxImage.Exclamation)
    '                Exit Sub
    '            End If

    '            Dim a As String = _ListMacro(i - 1)
    '            Dim b As String = _ListMacro(i)

    '            _ListMacro(i - 1) = b
    '            _ListMacro(i) = a
    '            RemplirMacro()
    '        End If
    '    Catch ex As Exception
    '        MessageBox.Show("Erreur UpMacro: " & ex.ToString, "Erreur")
    '    End Try
    'End Sub

    'Private Sub DownMacro_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles DownMacro.Click
    '    Try

    '        If ListBox1.SelectedIndex < 0 Or ListBox1.SelectedIndex = ListBox1.Items.Count - 1 Then
    '            MessageBox.Show("Aucune macro sélectionnée ou celle-ci est déjà en dernière position, veuillez en choisir une déplacer!", "Trigger", MessageBoxButton.OK, MessageBoxImage.Exclamation)
    '            Exit Sub
    '        Else
    '            Dim i As Integer = ListBox1.SelectedIndex

    '            Dim x As StackPanel = ListBox1.Items.Item(i)
    '            Dim w As CheckBox = x.Children.Item(0)
    '            If w.IsChecked = False Then
    '                MessageBox.Show("Cette macro n'est pas cochée, elle ne peut être déplacée !", "Trigger", MessageBoxButton.OK, MessageBoxImage.Exclamation)
    '                Exit Sub
    '            End If
    '            x = ListBox1.Items.Item(i + 1)
    '            w = x.Children.Item(0)
    '            If w.IsChecked = False Then
    '                MessageBox.Show("Cette macro ne peut pas être descendu dans la liste car la macro suivante n'est pas cochée, elle ne peut être déplacée !", "Trigger", MessageBoxButton.OK, MessageBoxImage.Exclamation)
    '                Exit Sub
    '            End If

    '            Dim a As String = _ListMacro(i + 1)
    '            Dim b As String = _ListMacro(i)

    '            _ListMacro(i + 1) = b
    '            _ListMacro(i) = a
    '            RemplirMacro()
    '        End If
    '    Catch ex As Exception
    '        MessageBox.Show("Erreur DownMacro: " & ex.ToString, "Erreur")
    '    End Try
    'End Sub

    Private Sub RemplirMacro()
        Try
            Dim _ListIDMacroDelete As New List(Of String) 'Liste des macro à supprimer si elles n'existent plus

            ListBox1.Items.Clear()

            If _ListMacro IsNot Nothing Then
                For Each _IDMacro As String In _ListMacro
                    Dim x As New CheckBox
                    Dim stk As New StackPanel
                    Dim _macro As HoMIDom.HoMIDom.Macro = myService.ReturnMacroById(IdSrv, _IDMacro)

                    'Vérifie que la mcro existe toujours
                    If _macro IsNot Nothing Then
                        stk.Margin = New Thickness(2)
                        x.Content = _macro.Nom
                        stk.Uid = _macro.ID
                        x.Uid = stk.Uid
                        x.IsChecked = True
                        AddHandler x.Click, AddressOf CheckClick
                        stk.Children.Add(x)
                        ListBox1.Items.Add(stk)
                    Else
                        _ListIDMacroDelete.Add(_IDMacro)
                    End If
                Next
            End If

            'Supprime la macro rattachée si elle n'existe plus
            For Each _IDmacro As String In _ListIDMacroDelete
                _ListMacro.Remove(_IDmacro)
            Next

            For i As Integer = 0 To myService.GetAllMacros(IdSrv).Count - 1
                If IsInList(myService.GetAllMacros(IdSrv).Item(i).ID) = False Then
                    Dim x As New CheckBox
                    Dim stk As New StackPanel
                    x.Content = myService.GetAllMacros(IdSrv).Item(i).Nom
                    x.IsChecked = False
                    AddHandler x.Click, AddressOf CheckClick
                    stk.Uid = myService.GetAllMacros(IdSrv).Item(i).ID
                    x.Uid = stk.Uid
                    stk.Margin = New Thickness(2)
                    stk.Children.Add(x)
                    ListBox1.Items.Add(stk)
                End If
            Next
        Catch ex As Exception
            MessageBox.Show("Erreur remplirMacro: " & ex.ToString, "Erreur")
        End Try
    End Sub

    Private Sub CheckClick(ByVal sender As Object, ByVal e As Windows.RoutedEventArgs)
        If sender.IsChecked = True Then
            _ListMacro.Add(sender.uid)
        Else
            For j As Integer = 0 To _ListMacro.Count - 1
                If _ListMacro.Item(j) = sender.uid Then
                    _ListMacro.RemoveAt(j)
                    Exit For
                End If
            Next
        End If
        RemplirMacro()
    End Sub

    Private Function IsInList(ByVal uid As String) As Boolean
        Try
            Dim flag As Boolean = False

            For i As Integer = 0 To ListBox1.Items.Count - 1
                If ListBox1.Items.Item(i).uid = uid Then
                    flag = True
                    Exit For
                End If
            Next

            Return flag
        Catch ex As Exception
            MessageBox.Show("Erreur IsInList: " & ex.ToString, "Erreur")
        End Try
    End Function

End Class
