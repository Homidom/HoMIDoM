Imports HoMIDom.HoMIDom
Imports HoMIDom.HoMIDom.Device
Imports HoMIDom.HoMIDom.Api
Imports System.IO

Class uModuleSimple
    Private listedevices

    Public Event CloseMe(ByVal MyObject As Object)

    Public Sub New()
        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()
        Try
            'Ajoute les choix des modules
            CbType.Items.Add("Associer un interrupteur/detecteur à un appareil/lampe/volet") '0
            CbType.Items.Add("Associer des interrupteurs/detecteurs à un appareil/lampe/volet") '1
            CbType.Items.Add("Associer un interrupteur/detecteur à des appareils/lampes/volets") '2
            CbType.Items.Add("Associer des interrupteurs/detecteurs à des appareils/lampes/volets") '3
            CbType.Items.Add("Programmer une action sur un composant") '4

            'Ajout des devices
            listedevices = myService.GetAllDevices(IdSrv)
            For Each Device In listedevices
                CbEmetteur.Items.Add(Device.Name)
                CbRecepteur.Items.Add(Device.Name)

                Dim stk As New StackPanel
                stk.Orientation = Orientation.Horizontal
                Dim x As New CheckBox
                x.Content = Device.Name
                x.ToolTip = Device.Description
                x.IsChecked = False
                stk.Children.Add(x)
                LbEmetteurMulti.Items.Add(stk)

                Dim stk2 As New StackPanel
                stk2.Orientation = Orientation.Horizontal
                Dim y As New CheckBox
                y.Content = Device.Name
                y.ToolTip = Device.Description
                y.IsChecked = False
                stk2.Children.Add(y)
                LbRecepteurMulti.Items.Add(stk2)
            Next
        Catch Ex As Exception
            MessageBox.Show("Erreur: UmoduleSimple New: " & Ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnAnnuler_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnAnnuler.Click
        Try
            RaiseEvent CloseMe(Me)
        Catch ex As Exception
            MessageBox.Show("Erreur: UmoduleSimple BtnAnnuler_Click: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnAjouter_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnAjouter.Click
        Try
            Dim _listeActions As New ArrayList
            Dim _listeMacros As New List(Of String)
            Dim _MacroId As String
            Select Case CbType.SelectedIndex
                Case 0
                    'Creation de la macro
                    Dim _actionif As New Action.ActionIf
                    _actionif.Timing = New System.DateTime(Now.Year, Now.Month, Now.Day, 0, 0, 0)

                    'Creation de la condition
                    Dim _condition As New Action.Condition
                    _condition.Type = Action.TypeCondition.Device
                    For Each Device In listedevices
                        If CbEmetteur.SelectedItem.ToString = Device.Name Then
                            _condition.IdDevice = Device.ID
                            Exit For
                        End If
                    Next
                    _condition.PropertyDevice = "Value"
                    _condition.Operateur = Action.TypeOperateur.NONE
                    _condition.Value = True
                    _actionif.Conditions.Add(_condition)

                    'creation de l'action si true/false
                    Dim _actiontrue As New Action.ActionDevice
                    Dim _actionfalse As New Action.ActionDevice
                    For Each Device In listedevices
                        If CbRecepteur.SelectedItem.ToString = Device.Name Then
                            _actiontrue.IdDevice = Device.ID
                            _actionfalse.IdDevice = Device.ID
                            MsgBox("type" & Device.Type.ToString)
                            If Device.Type.ToString = "VOLET" Then
                                _actiontrue.Method = "MONTER"
                                _actionfalse.Method = "DESCENDRE"
                            Else
                                _actiontrue.Method = "ON"
                                _actionfalse.Method = "OFF"
                            End If
                            Exit For
                        End If
                    Next
                    _actiontrue.Parametres.Clear()
                    _actionfalse.Parametres.Clear()
                    _actiontrue.Timing = New System.DateTime(Now.Year, Now.Month, Now.Day, 0, 0, 0)
                    _actionfalse.Timing = New System.DateTime(Now.Year, Now.Month, Now.Day, 0, 0, 0)
                    _actionif.ListTrue.Add(_actiontrue)
                    _actionif.ListFalse.Add(_actionfalse)

                    'ajout de la macro
                    _listeActions.Add(_actionif)
                    _MacroId = myService.SaveMacro(IdSrv, "", TxtNom.Text, True, "Macro créée depuis un Module", _listeActions)
                    _listeMacros.Add(_MacroId)

                    'creation du trigger
                    myService.SaveTrigger(IdSrv, "", TxtNom.Text, True, Trigger.TypeTrigger.DEVICE, "Trigger créé depuis un Module", "", _condition.IdDevice, "Value", _listeMacros)

                Case 1

                Case 2

                Case 3

                Case 4

            End Select
            RaiseEvent CloseMe(Me)
        Catch ex As Exception
            MessageBox.Show("Erreur: UmoduleSimple BtnAjouter_Click: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnHelp_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnHelp.Click
        Try
            MessageBox.Show("Les modules permettent de créer automatiquement des trigers et macros suivant des scénarios prédéfinis." & Chr(10) _
                            & "Veuillez sélectionner un émetteur (un ou des composants), un récepteur (un ou des ccomposants, un timer, une commande à donner... suivant le scénario choisi", _
                            "Aide", MessageBoxButton.OK, MessageBoxImage.Question)
        Catch ex As Exception
            MessageBox.Show("Erreur: UmoduleSimple BtnHelp_Click: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub CbType_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles CbType.SelectionChanged
        Try
            Select Case CbType.SelectedValue
                Case "Associer un interrupteur/detecteur à un appareil/lampe/volet"
                    StkCompEmet.Visibility = Windows.Visibility.Visible
                    StkCompRecep.Visibility = Windows.Visibility.Visible
                    StkMulti.Visibility = Windows.Visibility.Collapsed
                    StkCompMultiEmet.Visibility = Windows.Visibility.Collapsed
                    StkCompMultiRecep.Visibility = Windows.Visibility.Collapsed
                    StkOrdre.Visibility = Windows.Visibility.Collapsed
                Case "Associer des interrupteurs/detecteurs à un appareil/lampe/volet"
                    StkCompEmet.Visibility = Windows.Visibility.Collapsed
                    StkCompRecep.Visibility = Windows.Visibility.Visible
                    StkMulti.Visibility = Windows.Visibility.Visible
                    StkCompMultiEmet.Visibility = Windows.Visibility.Visible
                    StkCompMultiRecep.Visibility = Windows.Visibility.Collapsed
                    StkOrdre.Visibility = Windows.Visibility.Collapsed
                Case "Associer un interrupteur/detecteur à des appareils/lampes/volets"
                    StkCompEmet.Visibility = Windows.Visibility.Visible
                    StkCompRecep.Visibility = Windows.Visibility.Collapsed
                    StkMulti.Visibility = Windows.Visibility.Visible
                    StkCompMultiEmet.Visibility = Windows.Visibility.Collapsed
                    StkCompMultiRecep.Visibility = Windows.Visibility.Visible
                    StkOrdre.Visibility = Windows.Visibility.Collapsed
                Case "Associer des interrupteurs/detecteurs à des appareils/lampes/volets"
                    StkCompEmet.Visibility = Windows.Visibility.Collapsed
                    StkCompRecep.Visibility = Windows.Visibility.Collapsed
                    StkMulti.Visibility = Windows.Visibility.Visible
                    StkCompMultiEmet.Visibility = Windows.Visibility.Visible
                    StkCompMultiRecep.Visibility = Windows.Visibility.Visible
                    StkOrdre.Visibility = Windows.Visibility.Collapsed
                Case "Programmer une action sur un composant"
                    StkCompEmet.Visibility = Windows.Visibility.Collapsed
                    StkCompRecep.Visibility = Windows.Visibility.Visible
                    StkMulti.Visibility = Windows.Visibility.Collapsed
                    StkCompMultiEmet.Visibility = Windows.Visibility.Collapsed
                    StkCompMultiRecep.Visibility = Windows.Visibility.Collapsed
                    StkOrdre.Visibility = Windows.Visibility.Visible
            End Select
        Catch ex As Exception
            MessageBox.Show("Erreur: UmoduleSimple CbType_SelectionChanged: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class
