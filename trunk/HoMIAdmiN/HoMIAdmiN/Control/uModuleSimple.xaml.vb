Imports HoMIDom.HoMIDom
Imports HoMIDom.HoMIDom.Device
Imports HoMIDom.HoMIDom.Api
Imports System.IO

Class uModuleSimple
    Private listedevices, listemacros, listetriggers

    Public Event CloseMe(ByVal MyObject As Object)

    Public Sub New()
        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()
        Try
            'On cache tous les champs
            StkCompEmet.Visibility = Windows.Visibility.Collapsed
            StkCompRecep.Visibility = Windows.Visibility.Collapsed
            StkMulti.Visibility = Windows.Visibility.Collapsed
            StkCompMultiEmet.Visibility = Windows.Visibility.Collapsed
            StkCompMultiRecep.Visibility = Windows.Visibility.Collapsed
            StkOrdre.Visibility = Windows.Visibility.Collapsed
            StkCron.Visibility = Windows.Visibility.Collapsed

            'récuperation de la liste des macros/trigger
            listemacros = myService.GetAllMacros(IdSrv)
            listetriggers = myService.GetAllTriggers(IdSrv)

            'Ajoute les choix des modules
            CbType.Items.Add("Associer un interrupteur/detecteur à un appareil/lampe/volet") '0
            CbType.Items.Add("Associer des interrupteurs/detecteurs à un appareil/lampe/volet") '1
            CbType.Items.Add("Associer un interrupteur/detecteur à des appareils/lampes/volets") '2
            CbType.Items.Add("Associer des interrupteurs/detecteurs à des appareils/lampes/volets") '3
            CbType.Items.Add("Programmer une action sur un composant") '4
            CbType.SelectedIndex = 0

            'Ajout des devices aux listes/Combobox
            listedevices = myService.GetAllDevices(IdSrv)

            CbComposant.ItemsSource = listedevices
            CbComposant.DisplayMemberPath = "Name"
            CbEmetteur.ItemsSource = listedevices
            CbEmetteur.DisplayMemberPath = "Name"
            CbRecepteur.ItemsSource = listedevices
            CbRecepteur.DisplayMemberPath = "Name"
            For Each Device In listedevices
                'CbEmetteur.Items.Add(Device.Name)
                'CbRecepteur.Items.Add(Device.Name)

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
            'selection par défaut du premier composant dans les listes
            CbEmetteur.SelectedIndex = 0
            CbRecepteur.SelectedIndex = 0
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
            Dim resultatmessage As String = ""

            'vérifications
            If TxtNom.Text = "" Then
                MsgBox("Il faut renseigner un nom de module !", MsgBoxStyle.Critical)
                Exit Sub
            End If
            For Each Mac In listemacros
                If Mac.Nom.ToString.ToUpper = TxtNom.Text.ToUpper Then
                    MsgBox("Attention, ce nom existe déjà !", MsgBoxStyle.Critical)
                    TxtNom.Text = ""
                    Exit Sub
                End If
            Next
            For Each Trig In listetriggers
                If TxtNom.Text.ToUpper = Split(Trig.Nom.ToString.ToUpper, " :: ")(0) Then
                    MsgBox("Attention, ce nom existe déjà !", MsgBoxStyle.Critical)
                    TxtNom.Text = ""
                    Exit Sub
                End If
                If TxtNom.Text.ToUpper = Trig.Nom.ToString.ToUpper Then
                    MsgBox("Attention, ce nom existe déjà !", MsgBoxStyle.Critical)
                    TxtNom.Text = ""
                    Exit Sub
                End If
            Next

            resultatmessage = resultatmessage & "Création d'un module simple de type : " & Chr(10)
            Select Case CbType.SelectedIndex
                Case 0 : resultatmessage = resultatmessage & " -> Associer un interrupteur/detecteur à un appareil/lampe/volet :" & Chr(10)
                Case 1 : resultatmessage = resultatmessage & " -> Associer des interrupteurs/detecteurs à un appareil/lampe/volet :" & Chr(10)
                Case 2 : resultatmessage = resultatmessage & " -> Associer un interrupteur/detecteur à des appareils/lampes/volets :" & Chr(10)
                Case 3 : resultatmessage = resultatmessage & " -> Associer des interrupteurs/detecteurs à des appareils/lampes/volets :" & Chr(10)
                Case 4 : resultatmessage = resultatmessage & " -> Programmer une action sur un composant :" & Chr(10)
            End Select

            'Si on on associe un/des composants à un/des composants
            If CbType.SelectedIndex = 0 Or CbType.SelectedIndex = 1 Or CbType.SelectedIndex = 2 Or CbType.SelectedIndex = 3 Then

                'Creation de la macro
                Dim _actionif As New Action.ActionIf
                _actionif.Timing = New System.DateTime(Now.Year, Now.Month, Now.Day, 0, 0, 0)

                'Creation de la condition
                If CbType.SelectedIndex = 0 Or CbType.SelectedIndex = 2 Then
                    'pour un emetteur
                    Dim _condition As New Action.Condition
                    _condition.Type = Action.TypeCondition.Device
                    'For Each Device In listedevices
                    '    If CbEmetteur.SelectedItem.ToString = Device.Name Then
                    '        _condition.IdDevice = Device.ID
                    '        Exit For
                    '    End If
                    'Next
                    _condition.IdDevice = CbEmetteur.SelectedItem.Id
                    _condition.PropertyDevice = "Value"
                    _condition.Operateur = Action.TypeOperateur.NONE
                    _condition.Value = True
                    _actionif.Conditions.Add(_condition)
                ElseIf CbType.SelectedIndex = 1 Or CbType.SelectedIndex = 3 Then
                    'pour chaque emetteur
                    Dim nbemetteur As Integer = 0
                    For Each emetteurstk As StackPanel In LbEmetteurMulti.Items
                        Dim emetteur As CheckBox = emetteurstk.Children.Item(0)
                        If emetteur.IsChecked Then
                            nbemetteur += 1
                            Dim _condition As New Action.Condition
                            _condition = New Action.Condition
                            _condition.Type = Action.TypeCondition.Device
                            For Each Device In listedevices
                                If emetteur.Content = Device.Name Then
                                    _condition.IdDevice = Device.ID
                                    Exit For
                                End If
                            Next
                            _condition.PropertyDevice = "Value"
                            _condition.Operateur = Action.TypeOperateur.AND
                            _condition.Value = True
                            _actionif.Conditions.Add(_condition)
                        End If
                    Next
                    If nbemetteur = 0 Then
                        MsgBox("Il faut Sélectionner au moins un Emetteur !", MsgBoxStyle.Critical)
                        Exit Sub
                    End If
                End If

                'Creation de l'action si true/false
                If CbType.SelectedIndex = 0 Or CbType.SelectedIndex = 1 Then
                    Dim _actiontrue As New Action.ActionDevice
                    Dim _actionfalse As New Action.ActionDevice
                    'For Each Device In listedevices
                    '    If CbRecepteur.SelectedItem.ToString = Device.Name Then
                    '        _actiontrue.IdDevice = Device.ID
                    '        _actionfalse.IdDevice = Device.ID
                    '        If Device.Type.ToString = "VOLET" Then
                    '            _actiontrue.Method = "OPEN"
                    '            _actionfalse.Method = "CLOSE"
                    '        Else
                    '            _actiontrue.Method = "ON"
                    '            _actionfalse.Method = "OFF"
                    '        End If
                    '        Exit For
                    '    End If
                    'Next
                    _actiontrue.IdDevice = CbRecepteur.SelectedItem.Id
                    _actionfalse.IdDevice = CbRecepteur.SelectedItem.Id
                    If CbRecepteur.SelectedItem.Type.ToString = "VOLET" Then
                        _actiontrue.Method = "OPEN"
                        _actionfalse.Method = "CLOSE"
                    Else
                        _actiontrue.Method = "ON"
                        _actionfalse.Method = "OFF"
                    End If
                    _actiontrue.Parametres.Clear()
                    _actionfalse.Parametres.Clear()
                    _actiontrue.Timing = New System.DateTime(Now.Year, Now.Month, Now.Day, 0, 0, 0)
                    _actionfalse.Timing = New System.DateTime(Now.Year, Now.Month, Now.Day, 0, 0, 0)
                    _actionif.ListTrue.Add(_actiontrue)
                    _actionif.ListFalse.Add(_actionfalse)
                ElseIf CbType.SelectedIndex = 2 Or CbType.SelectedIndex = 3 Then
                    'pour chaque recepteur
                    Dim nbrecepteur As Integer = 0
                    For Each recepteurstk As StackPanel In LbRecepteurMulti.Items
                        Dim recepteur As CheckBox = recepteurstk.Children.Item(0)
                        If recepteur.IsChecked Then
                            nbrecepteur += 1
                            Dim _actiontrue As New Action.ActionDevice
                            Dim _actionfalse As New Action.ActionDevice
                            For Each Device In listedevices
                                If recepteur.Content = Device.Name Then
                                    _actiontrue.IdDevice = Device.ID
                                    _actionfalse.IdDevice = Device.ID
                                    If Device.Type.ToString = "VOLET" Then
                                        _actiontrue.Method = "OPEN"
                                        _actionfalse.Method = "CLOSE"
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
                        End If
                    Next
                    If nbrecepteur = 0 Then
                        MsgBox("Il faut Sélectionner au moins un Récepteur !", MsgBoxStyle.Critical)
                        Exit Sub
                    End If
                End If

                'ajout de la macro
                _listeActions.Add(_actionif)
                _MacroId = myService.SaveMacro(IdSrv, "", TxtNom.Text, True, "Macro créée depuis un Module", _listeActions)
                _listeMacros.Add(_MacroId)
                resultatmessage = resultatmessage & "     - Une Macro : " & TxtNom.Text & Chr(10)

                'creation du trigger
                If CbType.SelectedIndex = 0 Or CbType.SelectedIndex = 2 Then
                    myService.SaveTrigger(IdSrv, "", TxtNom.Text, True, Trigger.TypeTrigger.DEVICE, "Trigger créé depuis un Module", "", _actionif.Conditions.Item(0).IdDevice, "Value", _listeMacros)
                    resultatmessage = resultatmessage & "     - Un Trigger composant : " & TxtNom.Text & Chr(10)
                ElseIf CbType.SelectedIndex = 1 Or CbType.SelectedIndex = 3 Then
                    'creation des triggers
                    Dim nom As String = ""
                    resultatmessage = resultatmessage & "     - Des triggers : " & Chr(10)
                    For Each condition In _actionif.Conditions
                        For Each Device In listedevices
                            If condition.IdDevice = Device.ID Then
                                nom = Device.Name
                                Exit For
                            End If
                        Next
                        myService.SaveTrigger(IdSrv, "", TxtNom.Text & " :: " & nom, True, Trigger.TypeTrigger.DEVICE, "Trigger créé depuis un Module", "", condition.IdDevice, "Value", _listeMacros)
                        resultatmessage = resultatmessage & "        * " & TxtNom.Text & " :: " & nom & Chr(10)
                    Next
                End If
            ElseIf CbType.SelectedIndex = 4 Then 'on programme une action

                'on créé la macro action device
                Dim _actiondevice As New Action.ActionDevice
                _actiondevice.Timing = New System.DateTime(Now.Year, Now.Month, Now.Day, 0, 0, 0)
                _actiondevice.IdDevice = CbComposant.SelectedItem.Id
                _actiondevice.Method = CbAction.Text
                _actiondevice.Parametres.Clear()
                If TxtParametre.Text <> "" Then _actiondevice.Parametres.Add(TxtParametre.Text)
                _listeActions.Add(_actiondevice)
                _MacroId = myService.SaveMacro(IdSrv, "", TxtNom.Text, True, "Macro créée depuis un Module", _listeActions)
                _listeMacros.Add(_MacroId)
                resultatmessage = resultatmessage & "     - Une Macro : " & TxtNom.Text & Chr(10)


                'on créé le trigger Timer
                Dim _myconditiontime As String = "_cron"
                If TxtSc.Text = "" Then _myconditiontime &= "*#" Else _myconditiontime &= CInt(TxtSc.Text) & "#"
                If TxtMn.Text = "" Then _myconditiontime &= "*#" Else _myconditiontime &= CInt(TxtMn.Text) & "#"
                If TxtHr.Text = "" Then _myconditiontime &= "*#" Else _myconditiontime &= CInt(TxtHr.Text) & "#"
                If TxtJr.Text = "" Then _myconditiontime &= "*#" Else _myconditiontime &= CInt(TxtJr.Text) & "#"
                If TxtMs.Text = "" Then _myconditiontime &= "*#" Else _myconditiontime &= CInt(TxtMs.Text) & "#"
                Dim _prepajr As String = ""
                If CheckBox7.IsChecked = True Then _prepajr = "0"
                If CheckBox1.IsChecked = True Then If _prepajr <> "" Then _prepajr &= ",1" Else _prepajr = "1"
                If CheckBox2.IsChecked = True Then If _prepajr <> "" Then _prepajr &= ",2" Else _prepajr = "2"
                If CheckBox3.IsChecked = True Then If _prepajr <> "" Then _prepajr &= ",3" Else _prepajr = "3"
                If CheckBox4.IsChecked = True Then If _prepajr <> "" Then _prepajr &= ",4" Else _prepajr = "4"
                If CheckBox5.IsChecked = True Then If _prepajr <> "" Then _prepajr &= ",5" Else  : _prepajr = "5"
                If CheckBox6.IsChecked = True Then If _prepajr <> "" Then _prepajr &= ",6" Else _prepajr = "6"
                If _prepajr = "" Then _prepajr = "*"
                _myconditiontime &= _prepajr
                myService.SaveTrigger(IdSrv, "", TxtNom.Text, True, Trigger.TypeTrigger.TIMER, "Trigger créé depuis un Module", _myconditiontime, "", "", _listeMacros)
                resultatmessage = resultatmessage & "     - Un Trigger Timer : " & TxtNom.Text & Chr(10)

            End If



            'End Select
            MsgBox(resultatmessage, MsgBoxStyle.Information, "Création d'un module simple")
            RaiseEvent CloseMe(Me)
        Catch ex As Exception
            MessageBox.Show("Erreur: ModuleSimple BtnAjouter_Click: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnHelp_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnHelp.Click
        Try
            MessageBox.Show("Les modules permettent de créer automatiquement des trigers et macros suivant des scénarios prédéfinis." & Chr(10) _
                            & "Veuillez sélectionner un émetteur (un ou des composants), un récepteur (un ou des composants), un timer, une commande à envoyer... suivant le scénario choisi." & Chr(10) & Chr(10) _
                            & "INFO: Un module permet uniquement de créer des triggers/macros, il faut ensuite utiliser le panneau des macros/triggers pour les modifier/supprimer",
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
                    StkCron.Visibility = Windows.Visibility.Collapsed
                Case "Associer des interrupteurs/detecteurs à un appareil/lampe/volet"
                    StkCompEmet.Visibility = Windows.Visibility.Collapsed
                    StkCompRecep.Visibility = Windows.Visibility.Visible
                    StkMulti.Visibility = Windows.Visibility.Visible
                    StkCompMultiEmet.Visibility = Windows.Visibility.Visible
                    StkCompMultiRecep.Visibility = Windows.Visibility.Collapsed
                    StkOrdre.Visibility = Windows.Visibility.Collapsed
                    StkCron.Visibility = Windows.Visibility.Collapsed
                Case "Associer un interrupteur/detecteur à des appareils/lampes/volets"
                    StkCompEmet.Visibility = Windows.Visibility.Visible
                    StkCompRecep.Visibility = Windows.Visibility.Collapsed
                    StkMulti.Visibility = Windows.Visibility.Visible
                    StkCompMultiEmet.Visibility = Windows.Visibility.Collapsed
                    StkCompMultiRecep.Visibility = Windows.Visibility.Visible
                    StkOrdre.Visibility = Windows.Visibility.Collapsed
                    StkCron.Visibility = Windows.Visibility.Collapsed
                Case "Associer des interrupteurs/detecteurs à des appareils/lampes/volets"
                    StkCompEmet.Visibility = Windows.Visibility.Collapsed
                    StkCompRecep.Visibility = Windows.Visibility.Collapsed
                    StkMulti.Visibility = Windows.Visibility.Visible
                    StkCompMultiEmet.Visibility = Windows.Visibility.Visible
                    StkCompMultiRecep.Visibility = Windows.Visibility.Visible
                    StkOrdre.Visibility = Windows.Visibility.Collapsed
                    StkCron.Visibility = Windows.Visibility.Collapsed
                Case "Programmer une action sur un composant"
                    StkCompEmet.Visibility = Windows.Visibility.Collapsed
                    StkCompRecep.Visibility = Windows.Visibility.Collapsed
                    StkMulti.Visibility = Windows.Visibility.Collapsed
                    StkCompMultiEmet.Visibility = Windows.Visibility.Collapsed
                    StkCompMultiRecep.Visibility = Windows.Visibility.Collapsed
                    StkOrdre.Visibility = Windows.Visibility.Visible
                    StkCron.Visibility = Windows.Visibility.Visible
            End Select
        Catch ex As Exception
            MessageBox.Show("Erreur: UmoduleSimple CbType_SelectionChanged: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub TxtNom_LostFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles TxtNom.LostFocus
        Try
            If TxtNom.Text <> "" Then
                For Each Mac In listemacros
                    If Mac.Nom.ToString.ToUpper = TxtNom.Text.ToUpper Then
                        MsgBox("Attention, ce nom existe déjà !", MsgBoxStyle.Critical)
                        TxtNom.Text = ""
                        Exit Sub
                    End If
                Next
                For Each Trig In listetriggers
                    'If TxtNom.Text.ToUpper = Left(Trig.Nom.ToString.ToUpper, TxtNom.Text.Length) Then
                    If TxtNom.Text.ToUpper = Split(Trig.Nom.ToString.ToUpper, " :: ")(0) Then
                        MsgBox("Attention, ce nom existe déjà !", MsgBoxStyle.Critical)
                        TxtNom.Text = ""
                        Exit Sub
                    End If
                    If TxtNom.Text.ToUpper = Trig.Nom.ToString.ToUpper Then
                        MsgBox("Attention, ce nom existe déjà !", MsgBoxStyle.Critical)
                        TxtNom.Text = ""
                        Exit Sub
                    End If
                Next
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur: UmoduleSimple TxtNom_LostFocus: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub CbComposant_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles CbComposant.SelectionChanged
        Try
            If CbComposant.SelectedItem IsNot Nothing Then
                CbAction.ItemsSource = Nothing
                CbAction.Items.Clear()
                CbAction.ItemsSource = CbComposant.SelectedItem.DeviceAction
                CbAction.DisplayMemberPath = "Nom"
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur CbComposant_SelectionChanged: " & ex.ToString, "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub CbAction_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles CbAction.SelectionChanged
        Try
            StkParametre.Visibility = Windows.Visibility.Hidden

            If CbComposant.SelectedIndex < 0 Then Exit Sub
            If CbAction.SelectedIndex < 0 Then Exit Sub

            Dim Idx As Integer = CbAction.SelectedIndex
            For j As Integer = 0 To listedevices.Item(CbComposant.SelectedIndex).DeviceAction.Item(Idx).Parametres.Count - 1
                Select Case j
                    Case 0
                        StkParametre.Visibility = Windows.Visibility.Visible
                        Labelparametre.Content = CbComposant.SelectedItem.DeviceAction.Item(Idx).Parametres.Item(j).Nom & " :"
                        TxtParametre.ToolTip = CbComposant.SelectedItem.DeviceAction.Item(Idx).Parametres.Item(j).Type
                End Select
            Next
        Catch ex As Exception
            MessageBox.Show("Erreur CbAction_SelectionChanged: " & ex.ToString, "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

#Region "Gestion Date/time"
    Private Sub BtnPHr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnPHr.Click
        Dim i As Integer
        If TxtHr.Text = "" Then
            i = 0
            TxtHr.Text = Format(i, "00")
        Else
            i = TxtHr.Text
            i += 1
            If i > 23 Then
                TxtHr.Text = ""
            Else
                TxtHr.Text = i
            End If

        End If
    End Sub

    Private Sub BtnPMn_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnPMn.Click
        Dim i As Integer
        If TxtMn.Text = "" Then
            i = 0
            TxtMn.Text = Format(i, "00")
        Else
            i = TxtMn.Text
            i += 1
            If i > 59 Then
                TxtMn.Text = ""
            Else
                TxtMn.Text = i
            End If

        End If
    End Sub

    Private Sub BtnPSc_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnPSc.Click
        Dim i As Integer
        If TxtSc.Text = "" Then
            i = 0
            TxtSc.Text = Format(i, "00")
        Else
            i = TxtSc.Text
            i += 1
            If i > 59 Then
                TxtSc.Text = ""
            Else
                TxtSc.Text = i
            End If

        End If
    End Sub

    Private Sub BtnMHr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnMHr.Click
        Dim i As Integer
        If TxtHr.Text = "" Then
            i = 23
            TxtHr.Text = Format(i, "00")
        Else
            i = TxtHr.Text
            i -= 1
            If i < 0 Then
                TxtHr.Text = ""
            Else
                TxtHr.Text = i
            End If

        End If
    End Sub

    Private Sub BtnMMn_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnMMn.Click
        Dim i As Integer
        If TxtMn.Text = "" Then
            i = 59
            TxtMn.Text = Format(i, "00")
        Else
            i = TxtMn.Text
            i -= 1
            If i < 0 Then
                TxtMn.Text = ""
            Else
                TxtMn.Text = i
            End If

        End If

    End Sub

    Private Sub BtnMSec_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnMSec.Click
        Dim i As Integer
        If TxtSc.Text = "" Then
            i = 59
            TxtSc.Text = Format(i, "00")
        Else
            i = TxtSc.Text
            i -= 1
            If i < 0 Then
                TxtSc.Text = ""
            Else
                TxtSc.Text = i
            End If

        End If
    End Sub

    Private Sub BtnPJr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnPJr.Click
        Dim i As Integer
        If TxtJr.Text = "" Then
            i = 1
            TxtJr.Text = Format(i, "00")
        Else
            i = TxtJr.Text
            i += 1
            If i > 31 Then
                TxtJr.Text = ""
            Else
                TxtJr.Text = i
            End If
        End If
    End Sub

    Private Sub BtnPMs_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnPMs.Click
        Dim i As Integer
        If TxtMs.Text = "" Then
            i = 1
            TxtMs.Text = Format(i, "00")
        Else
            i = TxtMs.Text
            i += 1
            If i > 12 Then
                TxtMs.Text = ""
            Else
                TxtMs.Text = i
            End If

        End If
    End Sub

    Private Sub BtnMJr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnMJr.Click
        Dim i As Integer
        If TxtJr.Text = "" Then
            i = 31
            TxtJr.Text = Format(i, "00")
        Else
            i = TxtJr.Text
            i -= 1
            If i < 1 Then
                TxtJr.Text = ""
            Else
                TxtJr.Text = i
            End If

        End If
    End Sub

    Private Sub BtnMMs_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnMMs.Click
        Dim i As Integer
        If TxtMs.Text = "" Then
            i = 12
            TxtMs.Text = Format(i, "00")
        Else
            i = TxtMs.Text
            i -= 1
            If i < 1 Then
                TxtMs.Text = ""
            Else
                TxtMs.Text = i
            End If

        End If
    End Sub

    Private Sub TxtHr_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtHr.TextChanged
        If TxtHr.Text <> "" Then
            If IsNumeric(TxtHr.Text) = False Then
                TxtHr.Text = "0"
            ElseIf TxtHr.Text <> "" Then
                If TxtHr.Text < 0 Then TxtHr.Text = ""
                If TxtHr.Text > 23 Then TxtHr.Text = ""
            End If
        End If
    End Sub

    Private Sub TxtMn_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtMn.TextChanged
        If TxtMn.Text <> "" Then
            If IsNumeric(TxtMn.Text) = False Then
                TxtMn.Text = "0"
            ElseIf TxtMn.Text <> "" Then
                If TxtMn.Text < 0 Then TxtMn.Text = ""
                If TxtMn.Text > 59 Then TxtMn.Text = ""
            End If
        End If
    End Sub

    Private Sub TxtSc_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtSc.TextChanged
        If TxtSc.Text <> "" Then
            If IsNumeric(TxtSc.Text) = False Then
                TxtSc.Text = "0"
            ElseIf TxtSc.Text <> "" Then
                If TxtSc.Text < 0 Then TxtSc.Text = ""
                If TxtSc.Text > 59 Then TxtSc.Text = ""
            End If
        End If
    End Sub

    Private Sub TxtJr_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtJr.TextChanged
        If TxtJr.Text <> "" Then
            If IsNumeric(TxtJr.Text) = False Then
                TxtJr.Text = "1"
            ElseIf TxtJr.Text <> "" Then
                If TxtJr.Text < 0 Then TxtJr.Text = ""
                If TxtJr.Text > 31 Then TxtJr.Text = ""
            End If
        End If
    End Sub

    Private Sub TxtMs_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtMs.TextChanged
        If TxtMs.Text <> "" Then
            If IsNumeric(TxtMs.Text) = False Then
                TxtMs.Text = "1"
            ElseIf TxtMs.Text <> "" Then
                If TxtMs.Text < 0 Then TxtMs.Text = ""
                If TxtMs.Text > 12 Then TxtMs.Text = ""
            End If
        End If
    End Sub
#End Region

End Class
