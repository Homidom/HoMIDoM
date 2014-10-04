Imports HoMIDom.HoMIDom

Public Class WActionParametrage
    Dim _ObjAction As Object = Nothing
    Public _Parametres As New ArrayList
    Dim _ListuConditions As New List(Of uCondition)
    Dim mycontextmnu As New ContextMenu
    Dim mycontextmnu2 As New ContextMenu
    Dim ListeDevices As List(Of TemplateDevice)

    Public Property ObjAction As Object
        Get
            Return _ObjAction
        End Get
        Set(ByVal value As Object)
            _ObjAction = value
        End Set
    End Property

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOk.Click
        Try
            Dim _typ As Action.TypeAction

            If _ObjAction IsNot Nothing Then
                _typ = _ObjAction.TypeAction

                Select Case _typ
                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionDevice
                        If Cb1.SelectedIndex < 0 Or Cb2.SelectedIndex < 0 Or (TxtValue.Visibility = Windows.Visibility.Visible And String.IsNullOrEmpty(TxtValue.Text) = True) Then
                            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "Veuillez renseigner tous les champs !", "Erreur", "BtnOk.Click")
                            Exit Sub
                        End If

                        Dim obj As Action.ActionDevice = _ObjAction
                        obj.IdDevice = Cb1.SelectedItem.Id
                        obj.Method = Cb2.Text
                        obj.Parametres.Clear()

                        If String.IsNullOrEmpty(TxtValue.Text) = False Then obj.Parametres.Add(TxtValue.Text)

                        _ObjAction = obj

                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionDriver
                        If Cb1.SelectedIndex < 0 Or Cb2.SelectedIndex < 0 Or (TxtValue.Visibility = Windows.Visibility.Visible And String.IsNullOrEmpty(TxtValue.Text) = True) Then
                            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "Veuillez renseigner tous les champs !", "Erreur", "BtnOk.Click")
                            Exit Sub
                        End If

                        Dim obj As Action.ActionDriver = _ObjAction
                        obj.IdDriver = Cb1.SelectedItem.Id
                        obj.Method = Cb2.Text
                        obj.Parametres.Clear()

                        If String.IsNullOrEmpty(TxtValue.Text) = False Then obj.Parametres.Add(TxtValue.Text)

                        _ObjAction = obj
                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionVar
                        If Cb1.SelectedIndex < 0 Then
                            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "Veuillez renseigner tous les champs !", "Erreur", "BtnOk.Click")
                            Exit Sub
                        End If

                        Dim obj As Action.ActionVar = _ObjAction
                        obj.Nom = Cb1.SelectedItem.nom
                        obj.Value = TxtValue.Text

                        _ObjAction = obj
                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionMacro
                        If Cb1.SelectedIndex < 0 Then
                            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "Veuillez sélectionner une macro !", "Erreur", "BtnOK.Click")
                            Exit Sub
                        End If

                        Dim obj As Action.ActionMacro = _ObjAction
                        obj.IdMacro = Cb1.SelectedItem.ID
                        _ObjAction = obj

                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionMail
                        If Cb1.SelectedIndex < 0 Or String.IsNullOrEmpty(Txt2.Text) = True Or String.IsNullOrEmpty(TxtValue.Text) = True Then
                            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "Veuillez renseigner tous les champs !", "Erreur", "BtnOK.Click")
                            Exit Sub
                        End If

                        Dim obj As Action.ActionMail = _ObjAction
                        obj.UserId = Cb1.SelectedItem.ID
                        obj.Sujet = Txt2.Text
                        obj.Message = TxtValue.Text
                        _ObjAction = obj

                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionSpeech
                        If String.IsNullOrEmpty(TxtValue.Text) = True Then
                            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "Veuillez renseigner tous les champs !", "Erreur", "BtnOK.Click")
                            Exit Sub
                        End If

                        Dim obj As Action.ActionSpeech = _ObjAction
                        obj.Message = TxtValue.Text
                        _ObjAction = obj

                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionVB
                        If String.IsNullOrEmpty(TxtValue.Text) = True Or String.IsNullOrEmpty(Txt2.Text) = True Then
                            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "Veuillez renseigner tous les champs !", "Erreur", "BtnOK.Click")
                            Exit Sub
                        End If

                        Dim obj As Action.ActionVB = _ObjAction
                        obj.Script = TxtValue.Text
                        obj.Label = Txt2.Text
                        _ObjAction = obj

                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionLogEvent
                        If String.IsNullOrEmpty(TxtValue.Text) = True Then
                            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "Veuillez renseigner tous les champs car il n'y a pas de message !", "Erreur", "BtnOK.Click")
                            Exit Sub
                        End If
                        If Cb1.SelectedIndex < 0 Then
                            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "Veuillez sélectionner un type de log !", "Erreur", "BtnOK.Click")
                            Exit Sub
                        End If
                        If IsNumeric(Txt2.Text) = False Then
                            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "L'event ID doit être numérique !", "Erreur", "BtnOK.Click")
                            Exit Sub
                        End If

                        Dim obj As Action.ActionLogEvent = _ObjAction
                        obj.Message = TxtValue.Text
                        obj.Eventid = Txt2.Text
                        obj.Type = Cb1.SelectedIndex + 1
                        _ObjAction = obj

                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionLogEventHomidom
                        If String.IsNullOrEmpty(TxtValue.Text) = True Then
                            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "Veuillez renseigner tous les champs car il n'y a pas de message !", "Erreur", "BtnOK.Click")
                            Exit Sub
                        End If
                        If Cb1.SelectedIndex < 0 Then
                            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "Veuillez sélectionner un type de log !", "Erreur", "BtnOK.Click")
                            Exit Sub
                        End If
                        If String.IsNullOrEmpty(Txt2.Text) = True Then
                            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "Veuillez saisir le nom de la fonction !", "Erreur", "BtnOK.Click")
                            Exit Sub
                        End If

                        Dim obj As Action.ActionLogEventHomidom = _ObjAction
                        obj.Message = TxtValue.Text
                        obj.Fonction = Txt2.Text
                        obj.Type = Cb1.SelectedIndex + 1
                        _ObjAction = obj

                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionDOS
                        If String.IsNullOrEmpty(Txt2.Text) = True Then
                            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "Veuillez saisir le chemin du fichier !", "Erreur", "BtnOK.Click")
                            Exit Sub
                        End If

                        Dim obj As Action.ActionDos = _ObjAction
                        obj.Arguments = TxtValue.Text
                        obj.Fichier = Txt2.Text
                        _ObjAction = obj

                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionHttp
                        If String.IsNullOrEmpty(TxtValue.Text) = True Then
                            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "Veuillez renseigner tous les champs !", "Erreur", "BtnOK.Click")
                            Exit Sub
                        End If

                        Dim obj As Action.ActionHttp = _ObjAction
                        obj.Commande = TxtValue.Text
                        _ObjAction = obj

                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionIf
                        Dim obj As Action.ActionIf = _ObjAction

                        obj.Conditions.Clear()
                        For j As Integer = 0 To _ListuConditions.Count - 1
                            Dim _condi As New Action.Condition
                            _condi.Type = _ListuConditions.Item(j).TypeCondition
                            _condi.Operateur = _ListuConditions.Item(j).Operateur
                            _condi.Condition = _ListuConditions.Item(j).Signe
                            If _condi.Type = Action.TypeCondition.DateTime Then
                                _condi.DateTime = _ListuConditions.Item(j).DateTime
                            End If
                            If _condi.Type = Action.TypeCondition.Device Then
                                _condi.IdDevice = _ListuConditions.Item(j).IdDevice
                                _condi.PropertyDevice = _ListuConditions.Item(j).PropertyDevice
                                _condi.Value = _ListuConditions.Item(j).Value
                                If _condi.Value.ToString.ToUpper = "ON" Then _condi.Value = True
                                If _condi.Value.ToString.ToUpper = "OFF" Then _condi.Value = False
                            End If
                            obj.Conditions.Add(_condi)
                        Next
                        obj.ListTrue = UScenario1.Items
                        obj.ListFalse = UScenario2.Items
                        _ObjAction = obj

                End Select
                _ObjAction.Timing = New System.DateTime(Now.Year, Now.Month, Now.Day, TxtHr.Text, TxtMn.Text, TxtSc.Text)
            End If

            DialogResult = True
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur Click_Ok: " & ex.ToString, "Erreur Admin", "")
        End Try
    End Sub

    Private Sub Cb1_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles Cb1.SelectionChanged
        Try
            Cb2.Items.Clear()
            If _ObjAction.TypeAction = Action.TypeAction.ActionDevice Then
            If Cb1.SelectedItem IsNot Nothing Then
                'Ajout des commandes standard
                For i As Integer = 0 To Cb1.SelectedItem.DeviceAction.Count - 1
                    Cb2.Items.Add(Cb1.SelectedItem.DeviceAction.Item(i).Nom)
                Next

                'Ajout des commandes avancées
                For i As Integer = 0 To Cb1.SelectedItem.GetDeviceCommandePlus.Count - 1
                    Cb2.Items.Add("{" & Cb1.SelectedItem.GetDeviceCommandePlus.Item(i).NameCommand & "}")
                Next

            End If
            ElseIf _ObjAction.TypeAction = Action.TypeAction.ActionDriver Then
            If Cb1.SelectedItem IsNot Nothing Then
                'Ajout des commandes standard
                Cb2.Items.Add("Start")
                Cb2.Items.Add("Stop")
                Cb2.Items.Add("Restart")
            End If
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur Cb1_selectionChanged: " & ex.ToString, "Erreur Admin", "")
        End Try
    End Sub

    Public Sub New(ByVal ObjAction As Object)

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        Try
            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            _ObjAction = ObjAction
            Dim _typ As Action.TypeAction

            If _ObjAction IsNot Nothing Then
                _typ = _ObjAction.TypeAction

                TabControl1.Visibility = Windows.Visibility.Collapsed

                'creation du menu Click droit
                ListeDevices = myService.GetAllDevices(IdSrv)
                Dim y98 As New MenuItem
                y98.Header = "Effectuer un calcul"
                y98.Uid = "CALCUL"
                AddHandler y98.Click, AddressOf MenuItemDev_Click
                mycontextmnu.Items.Add(y98)
                Dim y99 As New MenuItem
                y99.Header = "Effacer la valeur"
                y99.Uid = "delete99"
                AddHandler y99.Click, AddressOf MenuItemDev_Click
                mycontextmnu.Items.Add(y99)
                For Each _dev As TemplateDevice In ListeDevices
                    Dim x As New MenuItem
                    x.Header = _dev.Name
                    x.Uid = _dev.ID
                    Select Case _dev.Type
                        Case 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27
                            'Dim y As New MenuItem
                            'y.Header = "Value"
                            'y.Uid = _dev.ID
                            'AddHandler y.Click, AddressOf MenuItemDev_Click
                            'x.Items.Add(y)
                            AddHandler x.Click, AddressOf MenuItemDev_Click
                        Case 17
                            Dim y0 As New MenuItem
                            y0.Header = ("Value")
                            y0.Uid = "SubItem"
                            AddHandler y0.Click, AddressOf MenuItemDev_Click
                            x.Items.Add(y0)
                            Dim y1 As New MenuItem
                            y1.Header = ("ConditionActuel")
                            y1.Uid = "SubItem"
                            AddHandler y1.Click, AddressOf MenuItemDev_Click
                            x.Items.Add(y1)
                            Dim y2 As New MenuItem
                            y2.Header = ("TemperatureActuel")
                            y2.Uid = "SubItem"
                            AddHandler y2.Click, AddressOf MenuItemDev_Click
                            x.Items.Add(y2)
                            Dim y3 As New MenuItem
                            y3.Header = ("HumiditeActuel")
                            y3.Uid = "SubItem"
                            AddHandler y3.Click, AddressOf MenuItemDev_Click
                            x.Items.Add(y3)
                            Dim y4 As New MenuItem
                            y4.Header = ("VentActuel")
                            y4.Uid = "SubItem"
                            AddHandler y4.Click, AddressOf MenuItemDev_Click
                            x.Items.Add(y4)
                            Dim y5 As New MenuItem
                            y5.Header = ("JourToday")
                            y5.Uid = "SubItem"
                            AddHandler y5.Click, AddressOf MenuItemDev_Click
                            x.Items.Add(y5)
                            Dim y6 As New MenuItem
                            y6.Header = ("MinToday")
                            y6.Uid = "SubItem"
                            AddHandler y6.Click, AddressOf MenuItemDev_Click
                            x.Items.Add(y6)
                            Dim y7 As New MenuItem
                            y7.Header = ("MaxToday")
                            y7.Uid = "SubItem"
                            AddHandler y7.Click, AddressOf MenuItemDev_Click
                            x.Items.Add(y7)
                            Dim y8 As New MenuItem
                            y8.Header = ("ConditionToday")
                            y8.Uid = "SubItem"
                            AddHandler y8.Click, AddressOf MenuItemDev_Click
                            x.Items.Add(y8)
                            Dim y9 As New MenuItem
                            y9.Header = ("JourJ1")
                            y9.Uid = "SubItem"
                            AddHandler y9.Click, AddressOf MenuItemDev_Click
                            x.Items.Add(y9)
                            Dim y10 As New MenuItem
                            y10.Header = ("MinJ1")
                            y10.Uid = "SubItem"
                            AddHandler y10.Click, AddressOf MenuItemDev_Click
                            x.Items.Add(y10)
                            Dim y11 As New MenuItem
                            y11.Header = ("MaxJ1")
                            y11.Uid = "SubItem"
                            AddHandler y11.Click, AddressOf MenuItemDev_Click
                            x.Items.Add(y11)
                            Dim y12 As New MenuItem
                            y12.Header = ("ConditionJ1")
                            y12.Uid = "SubItem"
                            AddHandler y12.Click, AddressOf MenuItemDev_Click
                            x.Items.Add(y12)
                            Dim y13 As New MenuItem
                            y13.Header = ("JourJ2")
                            y13.Uid = "SubItem"
                            AddHandler y13.Click, AddressOf MenuItemDev_Click
                            x.Items.Add(y13)
                            Dim y14 As New MenuItem
                            y14.Header = ("MinJ2")
                            y14.Uid = "SubItem"
                            AddHandler y14.Click, AddressOf MenuItemDev_Click
                            x.Items.Add(y14)
                            Dim y15 As New MenuItem
                            y15.Header = ("MaxJ2")
                            y15.Uid = "SubItem"
                            AddHandler y15.Click, AddressOf MenuItemDev_Click
                            x.Items.Add(y15)
                            Dim y16 As New MenuItem
                            y16.Header = ("ConditionJ2")
                            y16.Uid = "SubItem"
                            AddHandler y16.Click, AddressOf MenuItemDev_Click
                            x.Items.Add(y16)
                            Dim y17 As New MenuItem
                            y17.Header = ("JourJ3")
                            y17.Uid = "SubItem"
                            AddHandler y17.Click, AddressOf MenuItemDev_Click
                            x.Items.Add(y17)
                            Dim y18 As New MenuItem
                            y18.Header = ("MinJ3")
                            y18.Uid = "SubItem"
                            AddHandler y18.Click, AddressOf MenuItemDev_Click
                            x.Items.Add(y18)
                            Dim y19 As New MenuItem
                            y19.Header = ("MaxJ3")
                            y19.Uid = "SubItem"
                            AddHandler y19.Click, AddressOf MenuItemDev_Click
                            x.Items.Add(y19)
                            Dim y20 As New MenuItem
                            y20.Header = ("ConditionJ3")
                            y20.Uid = "SubItem"
                            AddHandler y20.Click, AddressOf MenuItemDev_Click
                            x.Items.Add(y20)
                    End Select
                    mycontextmnu.Items.Add(x)
                Next
                Dim S1 As New MenuItem
                S1.Header = "SYSTEM_DATE"
                S1.Uid = "SYSTEM"
                AddHandler S1.Click, AddressOf MenuItemDev_Click
                mycontextmnu.Items.Add(S1)
                Dim S2 As New MenuItem
                S2.Header = "SYSTEM_LONG_DATE"
                S2.Uid = "SYSTEM"
                AddHandler S2.Click, AddressOf MenuItemDev_Click
                mycontextmnu.Items.Add(S2)
                Dim S3 As New MenuItem
                S3.Header = "SYSTEM_TIME"
                S3.Uid = "SYSTEM"
                AddHandler S3.Click, AddressOf MenuItemDev_Click
                mycontextmnu.Items.Add(S3)
                Dim S4 As New MenuItem
                S4.Header = "SYSTEM_LONG_TIME"
                S4.Uid = "SYSTEM"
                AddHandler S4.Click, AddressOf MenuItemDev_Click
                mycontextmnu.Items.Add(S4)
                Dim S5 As New MenuItem
                S5.Header = "SYSTEM_SOLEIL_COUCHE"
                S5.Uid = "SYSTEM"
                AddHandler S5.Click, AddressOf MenuItemDev_Click
                mycontextmnu.Items.Add(S5)
                Dim S6 As New MenuItem
                S6.Header = "SYSTEM_SOLEIL_LEVE"
                S6.Uid = "SYSTEM"
                AddHandler S6.Click, AddressOf MenuItemDev_Click
                mycontextmnu.Items.Add(S6)
                TxtValue.ContextMenu = mycontextmnu

                Dim y198 As New MenuItem
                y198.Header = "Effectuer un calcul"
                y198.Uid = "CALCUL"
                AddHandler y198.Click, AddressOf MenuItemDev2_Click
                mycontextmnu2.Items.Add(y198)
                Dim y199 As New MenuItem
                y199.Header = "Effacer la valeur"
                y199.Uid = "delete99"
                AddHandler y199.Click, AddressOf MenuItemDev2_Click
                mycontextmnu2.Items.Add(y199)
                For Each _dev As TemplateDevice In ListeDevices
                    Dim x As New MenuItem
                    x.Header = _dev.Name
                    x.Uid = _dev.ID
                    Select Case _dev.Type
                        Case 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27
                            'Dim y As New MenuItem
                            'y.Header = "Value"
                            'y.Uid = _dev.ID
                            'AddHandler y.Click, AddressOf MenuItemDev_Click
                            'x.Items.Add(y)
                            AddHandler x.Click, AddressOf MenuItemDev2_Click
                        Case 17
                            Dim y0 As New MenuItem
                            y0.Header = ("Value")
                            y0.Uid = "SubItem"
                            AddHandler y0.Click, AddressOf MenuItemDev2_Click
                            x.Items.Add(y0)
                            Dim y1 As New MenuItem
                            y1.Header = ("ConditionActuel")
                            y1.Uid = "SubItem"
                            AddHandler y1.Click, AddressOf MenuItemDev2_Click
                            x.Items.Add(y1)
                            Dim y2 As New MenuItem
                            y2.Header = ("TemperatureActuel")
                            y2.Uid = "SubItem"
                            AddHandler y2.Click, AddressOf MenuItemDev2_Click
                            x.Items.Add(y2)
                            Dim y3 As New MenuItem
                            y3.Header = ("HumiditeActuel")
                            y3.Uid = "SubItem"
                            AddHandler y3.Click, AddressOf MenuItemDev2_Click
                            x.Items.Add(y3)
                            Dim y4 As New MenuItem
                            y4.Header = ("VentActuel")
                            y4.Uid = "SubItem"
                            AddHandler y4.Click, AddressOf MenuItemDev2_Click
                            x.Items.Add(y4)
                            Dim y5 As New MenuItem
                            y5.Header = ("JourToday")
                            y5.Uid = "SubItem"
                            AddHandler y5.Click, AddressOf MenuItemDev2_Click
                            x.Items.Add(y5)
                            Dim y6 As New MenuItem
                            y6.Header = ("MinToday")
                            y6.Uid = "SubItem"
                            AddHandler y6.Click, AddressOf MenuItemDev2_Click
                            x.Items.Add(y6)
                            Dim y7 As New MenuItem
                            y7.Header = ("MaxToday")
                            y7.Uid = "SubItem"
                            AddHandler y7.Click, AddressOf MenuItemDev2_Click
                            x.Items.Add(y7)
                            Dim y8 As New MenuItem
                            y8.Header = ("ConditionToday")
                            y8.Uid = "SubItem"
                            AddHandler y8.Click, AddressOf MenuItemDev2_Click
                            x.Items.Add(y8)
                            Dim y9 As New MenuItem
                            y9.Header = ("JourJ1")
                            y9.Uid = "SubItem"
                            AddHandler y9.Click, AddressOf MenuItemDev2_Click
                            x.Items.Add(y9)
                            Dim y10 As New MenuItem
                            y10.Header = ("MinJ1")
                            y10.Uid = "SubItem"
                            AddHandler y10.Click, AddressOf MenuItemDev2_Click
                            x.Items.Add(y10)
                            Dim y11 As New MenuItem
                            y11.Header = ("MaxJ1")
                            y11.Uid = "SubItem"
                            AddHandler y11.Click, AddressOf MenuItemDev2_Click
                            x.Items.Add(y11)
                            Dim y12 As New MenuItem
                            y12.Header = ("ConditionJ1")
                            y12.Uid = "SubItem"
                            AddHandler y12.Click, AddressOf MenuItemDev2_Click
                            x.Items.Add(y12)
                            Dim y13 As New MenuItem
                            y13.Header = ("JourJ2")
                            y13.Uid = "SubItem"
                            AddHandler y13.Click, AddressOf MenuItemDev2_Click
                            x.Items.Add(y13)
                            Dim y14 As New MenuItem
                            y14.Header = ("MinJ2")
                            y14.Uid = "SubItem"
                            AddHandler y14.Click, AddressOf MenuItemDev2_Click
                            x.Items.Add(y14)
                            Dim y15 As New MenuItem
                            y15.Header = ("MaxJ2")
                            y15.Uid = "SubItem"
                            AddHandler y15.Click, AddressOf MenuItemDev2_Click
                            x.Items.Add(y15)
                            Dim y16 As New MenuItem
                            y16.Header = ("ConditionJ2")
                            y16.Uid = "SubItem"
                            AddHandler y16.Click, AddressOf MenuItemDev2_Click
                            x.Items.Add(y16)
                            Dim y17 As New MenuItem
                            y17.Header = ("JourJ3")
                            y17.Uid = "SubItem"
                            AddHandler y17.Click, AddressOf MenuItemDev2_Click
                            x.Items.Add(y17)
                            Dim y18 As New MenuItem
                            y18.Header = ("MinJ3")
                            y18.Uid = "SubItem"
                            AddHandler y18.Click, AddressOf MenuItemDev2_Click
                            x.Items.Add(y18)
                            Dim y19 As New MenuItem
                            y19.Header = ("MaxJ3")
                            y19.Uid = "SubItem"
                            AddHandler y19.Click, AddressOf MenuItemDev2_Click
                            x.Items.Add(y19)
                            Dim y20 As New MenuItem
                            y20.Header = ("ConditionJ3")
                            y20.Uid = "SubItem"
                            AddHandler y20.Click, AddressOf MenuItemDev2_Click
                            x.Items.Add(y20)
                    End Select
                    mycontextmnu2.Items.Add(x)
                Next
                Dim S11 As New MenuItem
                S11.Header = "SYSTEM_DATE"
                S11.Uid = "SYSTEM"
                AddHandler S11.Click, AddressOf MenuItemDev2_Click
                mycontextmnu2.Items.Add(S11)
                Dim S12 As New MenuItem
                S12.Header = "SYSTEM_LONG_DATE"
                S12.Uid = "SYSTEM"
                AddHandler S12.Click, AddressOf MenuItemDev2_Click
                mycontextmnu2.Items.Add(S12)
                Dim S13 As New MenuItem
                S13.Header = "SYSTEM_TIME"
                S13.Uid = "SYSTEM"
                AddHandler S13.Click, AddressOf MenuItemDev2_Click
                mycontextmnu2.Items.Add(S13)
                Dim S14 As New MenuItem
                S14.Header = "SYSTEM_LONG_TIME"
                S14.Uid = "SYSTEM"
                AddHandler S14.Click, AddressOf MenuItemDev2_Click
                mycontextmnu2.Items.Add(S14)
                Dim S15 As New MenuItem
                S15.Header = "SYSTEM_SOLEIL_COUCHE"
                S15.Uid = "SYSTEM"
                AddHandler S15.Click, AddressOf MenuItemDev2_Click
                mycontextmnu2.Items.Add(S15)
                Dim S16 As New MenuItem
                S16.Header = "SYSTEM_SOLEIL_LEVE"
                S16.Uid = "SYSTEM"
                AddHandler S16.Click, AddressOf MenuItemDev2_Click
                mycontextmnu2.Items.Add(S16)
                Txt2.ContextMenu = mycontextmnu2



                Select Case _typ
                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionDevice
                        Dim obj As Action.ActionDevice = _ObjAction

                        'Mise en forme graphique
                        Lbl1.Content = "Composant:"
                        Lbl2.Content = "Action:"
                        Lbl2.Visibility = Visibility.Visible
                        Cb2.Visibility = Windows.Visibility.Visible
                        Txt2.Visibility = Windows.Visibility.Collapsed
                        TxtValue.Height = 25

                        Cb1.ItemsSource = myService.GetAllDevices(IdSrv)
                        Cb1.DisplayMemberPath = "Name"

                        If obj.IdDevice IsNot Nothing Then
                            For i As Integer = 0 To Cb1.Items.Count - 1
                                If obj.IdDevice = Cb1.Items(i).Id Then
                                    Cb1.SelectedIndex = i
                                    Exit For
                                End If
                            Next
                            For i As Integer = 0 To Cb2.Items.Count - 1
                                If obj.Method = Cb2.Items(i).ToString Then
                                    Cb2.SelectedIndex = i
                                    Exit For
                                End If
                            Next
                            If obj.Parametres.Count > 0 Then TxtValue.Text = obj.Parametres.Item(0)
                        End If
                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionDriver
                        Dim obj As Action.ActionDriver = _ObjAction

                        'Mise en forme graphique
                        Lbl1.Content = "Driver:"
                        Lbl2.Content = "Action:"
                        Lbl2.Visibility = Visibility.Visible
                        Cb2.Visibility = Windows.Visibility.Visible
                        Txt2.Visibility = Windows.Visibility.Collapsed
                        TxtValue.Height = 25

                        Cb1.ItemsSource = myService.GetAllDrivers(IdSrv)
                        Cb1.DisplayMemberPath = "Nom"

                        If obj.IdDriver IsNot Nothing Then
                            For i As Integer = 0 To Cb1.Items.Count - 1
                                If obj.IdDriver = Cb1.Items(i).Id Then
                                    Cb1.SelectedIndex = i
                                    Exit For
                                End If
                            Next
                            For i As Integer = 0 To Cb2.Items.Count - 1
                                If obj.Method = Cb2.Items(i).ToString Then
                                    Cb2.SelectedIndex = i
                                    Exit For
                                End If
                            Next
                            If obj.Parametres.Count > 0 Then TxtValue.Text = obj.Parametres.Item(0)
                        End If
                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionMacro
                        Dim obj As Action.ActionMacro = _ObjAction

                        'Mise en forme graphique
                        Lbl1.Content = "Macro:"
                        Lbl2.Visibility = Visibility.Collapsed
                        LblValue.Visibility = Windows.Visibility.Collapsed
                        Cb2.Visibility = Windows.Visibility.Collapsed
                        Txt2.Visibility = Windows.Visibility.Collapsed
                        Txt2.Height = 25
                        TxtValue.Visibility = Windows.Visibility.Collapsed

                        Cb1.ItemsSource = myService.GetAllMacros(IdSrv)
                        Cb1.DisplayMemberPath = "Nom"

                        Dim a As String = ""
                        If obj.IdMacro IsNot Nothing Then
                            For i As Integer = 0 To Cb1.Items.Count - 1
                                If obj.IdMacro = Cb1.Items(i).ID Then
                                    Cb1.SelectedIndex = i
                                    Exit For
                                End If
                            Next
                        End If
                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionMail
                        Dim obj As Action.ActionMail = _ObjAction

                        'Mise en forme graphique
                        Lbl1.Content = "Destinataire:"
                        Lbl2.Content = "Sujet:"
                        LblValue.Content = "Message:"
                        Txt2.Text = ""
                        Cb2.Visibility = Windows.Visibility.Collapsed
                        Txt2.Visibility = Windows.Visibility.Visible
                        Txt2.Height = 25
                        TxtValue.Text = ""
                        TxtValue.Height = 80

                        Cb1.ItemsSource = myService.GetAllUsers(IdSrv)
                        Cb1.DisplayMemberPath = "UserName"

                        If obj.UserId IsNot Nothing Then
                            Dim _user As Users.User = myService.ReturnUserById(IdSrv, obj.UserId)

                            For i As Integer = 0 To Cb1.Items.Count - 1
                                If _user.UserName = Cb1.Items(i).Username Then
                                    Cb1.SelectedIndex = i
                                    Exit For
                                End If
                            Next
                            Txt2.Text = obj.Sujet
                            TxtValue.Text = obj.Message
                        End If
                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionSpeech
                        Dim obj As Action.ActionSpeech = _ObjAction

                        'Mise en forme graphique
                        Lbl1.Visibility = Windows.Visibility.Collapsed
                        Lbl2.Visibility = Windows.Visibility.Collapsed
                        LblValue.Content = "Message:"
                        Txt2.Text = ""
                        Cb2.Visibility = Windows.Visibility.Collapsed
                        Cb1.Visibility = Windows.Visibility.Collapsed
                        Txt2.Visibility = Windows.Visibility.Collapsed
                        TxtValue.Text = ""
                        TxtValue.Height = 80

                        TxtValue.Text = obj.Message
                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionVB
                        Dim obj As Action.ActionVB = _ObjAction

                        'Mise en forme graphique
                        Lbl1.Visibility = Windows.Visibility.Collapsed
                        Lbl2.Content = "Label:"
                        Lbl2.Visibility = Windows.Visibility.Visible
                        LblValue.Content = "Code:"
                        Cb2.Visibility = Windows.Visibility.Collapsed
                        Cb1.Visibility = Windows.Visibility.Collapsed
                        Txt2.Visibility = Windows.Visibility.Visible
                        Txt2.Height = 25
                        TxtValue.Text = ""
                        TxtValue.Height = 400
                        TxtValue.Width = 650
                        TxtValue.VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                        TxtValue.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto

                        Txt2.Text = obj.Label
                        TxtValue.Text = obj.Script
                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionHttp
                        Dim obj As Action.ActionHttp = _ObjAction

                        'Mise en forme graphique
                        Lbl1.Visibility = Windows.Visibility.Collapsed
                        Lbl2.Visibility = Windows.Visibility.Collapsed
                        LblValue.Content = "Commande:"
                        Txt2.Text = ""
                        Cb2.Visibility = Windows.Visibility.Collapsed
                        Cb1.Visibility = Windows.Visibility.Collapsed
                        Txt2.Visibility = Windows.Visibility.Visible
                        Txt2.Height = 0
                        TxtValue.Text = ""
                        TxtValue.Height = 80

                        TxtValue.Text = obj.Commande

                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionLogEvent
                        Dim obj As Action.ActionLogEvent = _ObjAction

                        'Mise en forme graphique
                        Lbl1.Content = "Type:"
                        Lbl2.Content = "EventID:"
                        LblValue.Content = "Message:"

                        Cb1.Items.Add("ERREUR")
                        Cb1.Items.Add("WARNING")
                        Cb1.Items.Add("INFORMATION")

                        If obj.Type > 0 Then
                            Cb1.SelectedIndex = (obj.Type) - 1
                        Else
                            Cb1.SelectedIndex = 0
                        End If

                        Cb2.Visibility = Windows.Visibility.Collapsed
                        Cb2.Height = 0
                        Txt2.Visibility = Windows.Visibility.Visible
                        Txt2.Height = 25
                        Txt2.Text = obj.Eventid
                        TxtValue.Text = ""
                        TxtValue.Height = 80

                        TxtValue.Text = obj.Message

                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionLogEventHomidom
                        Dim obj As Action.ActionLogEventHomidom = _ObjAction

                        'Mise en forme graphique
                        Lbl1.Content = "Type:"
                        Lbl2.Content = "Fonction:"
                        LblValue.Content = "Message:"

                        Cb1.Items.Add("INFO")
                        Cb1.Items.Add("ACTION")
                        Cb1.Items.Add("MESSAGE")
                        Cb1.Items.Add("VALEUR_CHANGE")
                        Cb1.Items.Add("VALEUR_INCHANGE")
                        Cb1.Items.Add("VALEUR_INCHANGE_PRECISION")
                        Cb1.Items.Add("VALEUR_INCHANGE_LASTETAT")
                        Cb1.Items.Add("ERREUR")
                        Cb1.Items.Add("ERREUR_CRITIQUE")
                        Cb1.Items.Add("DEBUG")

                        If obj.Type > 0 Then
                            Cb1.SelectedIndex = (obj.Type) - 1
                        Else
                            Cb1.SelectedIndex = 0
                        End If

                        Cb2.Visibility = Windows.Visibility.Collapsed
                        Txt2.Visibility = Windows.Visibility.Visible
                        Txt2.Height = 25
                        Txt2.Text = obj.Fonction
                        TxtValue.Text = ""
                        TxtValue.Height = 80
                        TxtValue.Text = obj.Message

                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionDOS
                        Dim obj As Action.ActionDos = _ObjAction

                        'Mise en forme graphique
                        Lbl2.Content = "Fichier:"
                        LblValue.Content = "Arguments:"

                        Cb1.Visibility = Windows.Visibility.Collapsed
                        Lbl1.Visibility = Windows.Visibility.Collapsed
                        Cb2.Visibility = Windows.Visibility.Collapsed
                        Txt2.Visibility = Windows.Visibility.Visible
                        Txt2.Height = 25
                        Txt2.ToolTip = "Veuillez saisir le chemin du fichier (exemple: C:\test\program.exe)"
                        Txt2.Text = obj.Fichier
                        TxtValue.Text = ""
                        TxtValue.ToolTip = "Veuillez saisir les arguments associés au fichier (exemple \a -b)"
                        TxtValue.Height = 25
                        TxtValue.Text = obj.Arguments

                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionIf

                        StkProperty.Visibility = Windows.Visibility.Collapsed
                        TabControl1.Visibility = Windows.Visibility.Visible

                        Dim obj As Action.ActionIf = _ObjAction

                        StkCondition.Children.Clear()

                        For i As Integer = 0 To obj.Conditions.Count - 1
                            Dim x As New uCondition
                            If i = 0 Then
                                x.IsFirst = True
                            Else
                                x.IsFirst = False
                            End If
                            x.Uid = HoMIDom.HoMIDom.Api.GenerateGUID
                            x.TypeCondition = obj.Conditions.Item(i).Type

                            x.Operateur = obj.Conditions.Item(i).Operateur
                            x.Signe = obj.Conditions.Item(i).Condition
                            If x.TypeCondition = Action.TypeCondition.DateTime Then
                                x.DateTime = obj.Conditions.Item(i).DateTime
                            End If
                            If x.TypeCondition = Action.TypeCondition.Device Then
                                x.IdDevice = obj.Conditions.Item(i).IdDevice
                                x.PropertyDevice = obj.Conditions.Item(i).PropertyDevice
                                x.Value = obj.Conditions.Item(i).Value
                            End If
                            AddHandler x.UpCondition, AddressOf UpCondition
                            AddHandler x.DeleteCondition, AddressOf DeleteCondition
                            _ListuConditions.Add(x)
                            StkCondition.Children.Add(x)
                        Next

                        UScenario1.Items = obj.ListTrue
                        UScenario2.Items = obj.ListFalse
                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionStop

                        StkProperty.Visibility = Windows.Visibility.Collapsed
                        TabControl1.Visibility = Windows.Visibility.Collapsed

                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionVar
                        Dim obj As Action.ActionVar = _ObjAction

                        'Mise en forme graphique
                        Lbl1.Content = "Variable:"
                        Lbl2.Visibility = Visibility.Collapsed
                        Cb2.Visibility = Windows.Visibility.Collapsed
                        Txt2.Visibility = Windows.Visibility.Collapsed
                        TxtValue.Height = 25

                        Cb1.ItemsSource = myService.GetAllVariables(IdSrv)
                        Cb1.DisplayMemberPath = "Nom"

                        If String.IsNullOrEmpty(obj.Nom) = False Then
                            For i As Integer = 0 To Cb1.Items.Count - 1
                                If obj.Nom = Cb1.Items(i).Nom Then
                                    Cb1.SelectedIndex = i
                                    Exit For
                                End If
                            Next
                            TxtValue.Text = obj.Value
                        End If
                End Select

                Dim t1 As Integer
                t1 = _ObjAction.timing.Hour
                TxtHr.Text = Format(t1, "00")
                t1 = _ObjAction.timing.Minute
                TxtMn.Text = Format(t1, "00")
                t1 = _ObjAction.timing.Second
                TxtSc.Text = Format(t1, "00")

            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur New: " & ex.ToString, "Erreur Admin", "")
        End Try
    End Sub

    Private Sub MenuItemDev_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Try
            If sender.uid = "delete99" Then
                TxtValue.Text = ""
            ElseIf sender.uid = "CALCUL" Then
                TxtValue.Text = TxtValue.Text & "{formule à calculer}"
            ElseIf sender.uid = "SYSTEM" Then
                TxtValue.Text = TxtValue.Text & "<" & sender.header & ">"
            ElseIf sender.uid = "SubItem" Then
                TxtValue.Text = TxtValue.Text & "<" & sender.parent.header & "." & sender.header & ">"
            Else
                TxtValue.Text = TxtValue.Text & "<" & sender.header & ">"
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur: Waction_MenuItemDev_Click" & ex.ToString)
        End Try
    End Sub
    Private Sub MenuItemDev2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Try
            If sender.uid = "delete99" Then
                Txt2.Text = ""
            ElseIf sender.uid = "CALCUL" Then
                Txt2.Text = Txt2.Text & "{formule à calculer}"
            ElseIf sender.uid = "SYSTEM" Then
                Txt2.Text = Txt2.Text & "<" & sender.header & ">"
            ElseIf sender.uid = "SubItem" Then
                Txt2.Text = Txt2.Text & "<" & sender.parent.header & "." & sender.header & ">"
            Else
                Txt2.Text = Txt2.Text & "<" & sender.header & ">"
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur: Waction_MenuItemDev_Click" & ex.ToString)
        End Try
    End Sub


    Private Sub Cb2_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles Cb2.SelectionChanged
        Try
            LblValue.Visibility = Windows.Visibility.Collapsed
            TxtValue.Visibility = Windows.Visibility.Collapsed

            If Cb1.SelectedIndex < 0 Then Exit Sub
            If Cb2.SelectedIndex < 0 Then Exit Sub

            Cb2.ToolTip = ""
            Dim Idx As Integer = Cb2.SelectedIndex

            If _ObjAction.TypeAction = HoMIDom.HoMIDom.Action.TypeAction.ActionDevice Then
                If Cb2.SelectedValue.ToString.StartsWith("{") And Cb2.SelectedValue.ToString.EndsWith("}") Then
                    'c'est une commande avancée
                    Dim _cmdav As String = Mid(Cb2.SelectedValue.ToString, 2, Cb2.SelectedValue.ToString.Length - 2)
                    For j As Integer = 0 To myService.GetAllDevices(IdSrv).Item(Cb1.SelectedIndex).GetDeviceCommandePlus.Count - 1
                        If myService.GetAllDevices(IdSrv).Item(Cb1.SelectedIndex).GetDeviceCommandePlus.Item(j).NameCommand = _cmdav Then
                            Cb2.ToolTip = myService.GetAllDevices(IdSrv).Item(Cb1.SelectedIndex).GetDeviceCommandePlus.Item(j).DescriptionCommand
                            If myService.GetAllDevices(IdSrv).Item(Cb1.SelectedIndex).GetDeviceCommandePlus.Item(j).CountParam > 0 Then
                                LblValue.Content = "Parametre:"
                                LblValue.Visibility = Windows.Visibility.Visible
                                TxtValue.Visibility = Windows.Visibility.Visible
                                Exit For
                            End If
                        End If
                    Next
                Else
                    'c'est une commande standard
                    For j As Integer = 0 To myService.GetAllDevices(IdSrv).Item(Cb1.SelectedIndex).DeviceAction.Item(Idx).Parametres.Count - 1
                        Select Case j
                            Case 0
                                LblValue.Content = Cb1.SelectedItem.DeviceAction.Item(Idx).Parametres.Item(j).Nom & " :"
                                LblValue.Visibility = Windows.Visibility.Visible
                                TxtValue.ToolTip = Cb1.SelectedItem.DeviceAction.Item(Idx).Parametres.Item(j).Type
                                TxtValue.Visibility = Windows.Visibility.Visible
                        End Select
                    Next
                End If
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur Cb2_selectionChanged: " & ex.ToString, "Erreur Admin", "")
        End Try
    End Sub

#Region "Gestion Timing"

    Private Sub BtnPHr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnPHr.Click
        Dim i As Integer = TxtHr.Text
        i += 1
        If i > 23 Then i = 0
        TxtHr.Text = Format(i, "00")
    End Sub

    Private Sub BtnMHr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnMHr.Click
        Dim i As Integer = TxtHr.Text
        i -= 1
        If i < 0 Then i = 23
        TxtHr.Text = Format(i, "00")
    End Sub

    Private Sub BtnPMn_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnPMn.Click
        Dim i As Integer = CInt(TxtMn.Text)
        i += 1
        If i > 59 Then i = 0
        TxtMn.Text = Format(i, "00")
    End Sub

    Private Sub BtnMMn_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnMMn.Click
        Dim i As Integer = TxtMn.Text
        i -= 1
        If i < 0 Then i = 59
        TxtMn.Text = Format(i, "00")
    End Sub

    Private Sub BtnPSc_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnPSc.Click
        Dim i As Integer = TxtSc.Text
        i += 1
        If i > 59 Then i = 0
        TxtSc.Text = Format(i, "00")
    End Sub

    Private Sub BtnMSec_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnMSec.Click
        Dim i As Integer = TxtSc.Text
        i -= 1
        If i < 0 Then i = 59
        TxtSc.Text = Format(i, "00")
    End Sub

    Private Sub TxtMn_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtMn.TextChanged
        If IsNumeric(TxtMn.Text) = False Then
            TxtMn.Text = "00"
        End If
    End Sub

    Private Sub TxtSc_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtSc.TextChanged
        If IsNumeric(TxtSc.Text) = False Then
            TxtMn.Text = "00"
        End If
    End Sub

    Private Sub TxtHr_TextChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtHr.TextChanged
        If IsNumeric(TxtHr.Text) = False Then
            TxtHr.Text = "00"
        End If
    End Sub

#End Region

#Region "Gestion Condition"

    Private Sub BtnCondiTime_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnCondiTime.MouseLeftButtonDown
        Try
            Dim x As New uCondition
            x.TypeCondition = Action.TypeCondition.DateTime
            x.Uid = HoMIDom.HoMIDom.Api.GenerateGUID
            AddHandler x.DeleteCondition, AddressOf DeleteCondition
            AddHandler x.UpCondition, AddressOf UpCondition
            If StkCondition.Children.Count = 0 Then
                x.IsFirst = True
            Else
                x.IsFirst = False
            End If
            StkCondition.Children.Add(x)
            _ListuConditions.Add(x)
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur WAction BtnCondiTime_MouseLeftButtonDown: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Sub BtnCondiDevice_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnCondiDevice.MouseLeftButtonDown
        Try
            Dim x As New uCondition
            x.TypeCondition = Action.TypeCondition.Device
            x.Uid = HoMIDom.HoMIDom.Api.GenerateGUID
            AddHandler x.DeleteCondition, AddressOf DeleteCondition
            AddHandler x.UpCondition, AddressOf UpCondition
            If StkCondition.Children.Count = 0 Then
                x.IsFirst = True
            Else
                x.IsFirst = False
            End If
            StkCondition.Children.Add(x)
            _ListuConditions.Add(x)
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur WAction BtnCondiDevice_MouseLeftButtonDown: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Sub DeleteCondition(ByVal uid As String)
        Try
            For i As Integer = 0 To StkCondition.Children.Count - 1
                If StkCondition.Children.Item(i).Uid = uid Then
                    StkCondition.Children.RemoveAt(i)
                    _ListuConditions.RemoveAt(i)
                    Exit For
                End If
            Next

            RefreshCondition()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur WAction DeleteCondition: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Sub UpCondition(ByVal uid As String)
        Try
            For i As Integer = 0 To _StkCondition.Children.Count - 1
                If _StkCondition.Children.Item(i).Uid = uid Then
                    If i = 0 Then Exit Sub
                    'on verifi si c'est le 1er car on peu plus monter
                    Dim x As uCondition = _ListuConditions.Item(i - 1)
                    _ListuConditions.Item(i - 1) = _ListuConditions.Item(i)
                    _ListuConditions.Item(i) = x

                    StkCondition.Children.Clear()
                    For j As Integer = 0 To _ListuConditions.Count - 1
                        StkCondition.Children.Add(_ListuConditions.Item(j))
                    Next
                    Exit For
                End If
            Next

            RefreshCondition()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur WAction UpCondition: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Sub RefreshCondition()
        Try
            If StkCondition.Children.Count > 0 Then
                For i As Integer = 0 To StkCondition.Children.Count - 1
                    Dim x As uCondition = StkCondition.Children.Item(i)
                    If i = 0 Then
                        x.IsFirst = True
                    Else
                        x.IsFirst = False
                    End If
                Next
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur RefreshCondition: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub
#End Region


End Class
