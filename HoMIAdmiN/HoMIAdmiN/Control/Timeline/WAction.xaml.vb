Imports HoMIDom.HoMIDom

Public Class WActionParametrage
    Dim _ObjAction As Object
    Public _Parametres As New ArrayList
    Dim _ListuConditions As New List(Of uCondition)

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
                        If Cb1.SelectedIndex < 0 Or Cb2.SelectedIndex < 0 Or (TxtValue.Visibility = Windows.Visibility.Visible And TxtValue.Text = "") Then
                            MessageBox.Show("Veuillez renseigner tous les champs !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                            Exit Sub
                        End If

                        Dim obj As Action.ActionDevice = _ObjAction
                        obj.IdDevice = Cb1.SelectedItem.Id
                        obj.Method = Cb2.Text
                        obj.Parametres.Clear()

                        If TxtValue.Text <> "" Then obj.Parametres.Add(TxtValue.Text)
                        _ObjAction = obj

                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionMacro
                        If Cb1.SelectedIndex < 0 Then
                            MessageBox.Show("Veuillez sélectionner une macro !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                            Exit Sub
                        End If

                        Dim obj As Action.ActionMacro = _ObjAction
                        obj.IdMacro = Cb1.SelectedItem.ID
                        _ObjAction = obj

                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionMail
                        If Cb1.SelectedIndex < 0 Or Txt2.Text = "" Or TxtValue.Text = "" Then
                            MessageBox.Show("Veuillez renseigner tous les champs !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                            Exit Sub
                        End If

                        Dim obj As Action.ActionMail = _ObjAction
                        obj.UserId = Cb1.SelectedItem.ID
                        obj.Sujet = Txt2.Text
                        obj.Message = TxtValue.Text
                        _ObjAction = obj

                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionSpeech
                        If TxtValue.Text = "" Then
                            MessageBox.Show("Veuillez renseigner tous les champs !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                            Exit Sub
                        End If

                        Dim obj As Action.ActionSpeech = _ObjAction
                        obj.Message = TxtValue.Text
                        _ObjAction = obj

                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionVB
                        If TxtValue.Text = "" Or Txt2.Text = "" Then
                            MessageBox.Show("Veuillez renseigner tous les champs !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                            Exit Sub
                        End If

                        Dim obj As Action.ActionVB = _ObjAction
                        obj.Script = TxtValue.Text
                        obj.Label = Txt2.Text
                        _ObjAction = obj

                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionLogEvent
                        If TxtValue.Text = "" Then
                            MessageBox.Show("Veuillez renseigner tous les champs car il n'y a pas de message !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                            Exit Sub
                        End If
                        If Cb1.SelectedIndex < 0 Then
                            MessageBox.Show("Veuillez sélectionner un type de log !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                            Exit Sub
                        End If
                        If IsNumeric(Txt2.Text) = False Then
                            MessageBox.Show("L'event ID doit être numérique !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                            Exit Sub
                        End If

                        Dim obj As Action.ActionLogEvent = _ObjAction
                        obj.Message = TxtValue.Text
                        obj.Eventid = Txt2.Text
                        obj.Type = Cb1.SelectedIndex + 1
                        _ObjAction = obj

                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionLogEventHomidom
                        If TxtValue.Text = "" Then
                            MessageBox.Show("Veuillez renseigner tous les champs car il n'y a pas de message !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                            Exit Sub
                        End If
                        If Cb1.SelectedIndex < 0 Then
                            MessageBox.Show("Veuillez sélectionner un type de log !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                            Exit Sub
                        End If
                        If Txt2.Text = "" Then
                            MessageBox.Show("Veuillez saisir le nom de la fonction !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                            Exit Sub
                        End If

                        Dim obj As Action.ActionLogEventHomidom = _ObjAction
                        obj.Message = TxtValue.Text
                        obj.Fonction = Txt2.Text
                        obj.Type = Cb1.SelectedIndex + 1
                        _ObjAction = obj

                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionDOS
                        If Txt2.Text = "" Then
                            MessageBox.Show("Veuillez saisir le chemin du fichier !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                            Exit Sub
                        End If

                        Dim obj As Action.ActionDos = _ObjAction
                        obj.Arguments = TxtValue.Text
                        obj.Fichier = Txt2.Text
                        _ObjAction = obj

                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionHttp
                        If TxtValue.Text = "" Then
                            MessageBox.Show("Veuillez renseigner tous les champs !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
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
            MessageBox.Show("Erreur Click_Ok: " & ex.ToString, "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub Cb1_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles Cb1.SelectionChanged
        Try
            If _ObjAction.TypeAction = Action.TypeAction.ActionDevice Then
                If Cb1.SelectedItem IsNot Nothing Then
                    Cb2.ItemsSource = Nothing
                    Cb2.Items.Clear()
                    Cb2.ItemsSource = Cb1.SelectedItem.DeviceAction
                    Cb2.DisplayMemberPath = "Nom"
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur Cb1_selectionChanged: " & ex.ToString, "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
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

                Select Case _typ
                    Case HoMIDom.HoMIDom.Action.TypeAction.ActionDevice
                        Dim obj As Action.ActionDevice = _ObjAction

                        'Mise en forme graphique
                        Lbl1.Content = "Device:"
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
                                If obj.Method = Cb2.Items(i).Nom Then
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
                        Lbl1.Content = "Username:"
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
                        'Cb2.Visibility = Windows.Visibility.Collapsed
                        'Cb1.Visibility = Windows.Visibility.Collapsed
                        'Txt2.Visibility = Windows.Visibility.Collapsed
                        'TxtValue.Visibility = Windows.Visibility.Hidden
                        'Lbl1.Visibility = Windows.Visibility.Hidden
                        'Lbl2.Visibility = Windows.Visibility.Hidden
                        'LblValue.Visibility = Windows.Visibility.Hidden

                        Dim obj As Action.ActionIf = _ObjAction

                        StkCondition.Children.Clear()
                        For i As Integer = 0 To obj.Conditions.Count - 1
                            Dim x As New uCondition
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
            MessageBox.Show("Erreur New: " & ex.ToString, "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub Cb2_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles Cb2.SelectionChanged
        Try
            LblValue.Visibility = Windows.Visibility.Hidden
            TxtValue.Visibility = Windows.Visibility.Hidden

            If Cb1.SelectedIndex < 0 Then Exit Sub
            If Cb2.SelectedIndex < 0 Then Exit Sub

            Dim Idx As Integer = Cb2.SelectedIndex
            For j As Integer = 0 To myservice.GetAllDevices(IdSrv).Item(Cb1.SelectedIndex).DeviceAction.Item(Idx).Parametres.Count - 1
                Select Case j
                    Case 0
                        LblValue.Content = Cb1.SelectedItem.DeviceAction.Item(Idx).Parametres.Item(j).Nom & " :"
                        LblValue.Visibility = Windows.Visibility.Visible
                        TxtValue.ToolTip = Cb1.SelectedItem.DeviceAction.Item(Idx).Parametres.Item(j).Type
                        TxtValue.Visibility = Windows.Visibility.Visible
                End Select
            Next
        Catch ex As Exception
            MessageBox.Show("Erreur Cb2_selectionChanged: " & ex.ToString, "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
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
        Dim x As New uCondition
        x.TypeCondition = Action.TypeCondition.DateTime
        x.Uid = HoMIDom.HoMIDom.Api.GenerateGUID
        AddHandler x.DeleteCondition, AddressOf DeleteCondition
        AddHandler x.UpCondition, AddressOf UpCondition
        StkCondition.Children.Add(x)
        _ListuConditions.Add(x)
    End Sub

    Private Sub BtnCondiDevice_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnCondiDevice.MouseLeftButtonDown
        Dim x As New uCondition
        x.TypeCondition = Action.TypeCondition.Device
        x.Uid = HoMIDom.HoMIDom.Api.GenerateGUID
        AddHandler x.DeleteCondition, AddressOf DeleteCondition
        AddHandler x.UpCondition, AddressOf UpCondition
        StkCondition.Children.Add(x)
        _ListuConditions.Add(x)
    End Sub

    Private Sub DeleteCondition(ByVal uid As String)
        For i As Integer = 0 To StkCondition.Children.Count - 1
            If StkCondition.Children.Item(i).Uid = uid Then
                StkCondition.Children.RemoveAt(i)
                _ListuConditions.RemoveAt(i)
                Exit For
            End If
        Next
    End Sub

    Private Sub UpCondition(ByVal uid As String)
        For i As Integer = 0 To _StkCondition.Children.Count - 1
            If _StkCondition.Children.Item(i).Uid = uid Then
                If i = 0 Then Exit Sub
                'on verifi si c'est le 1er car on peu plus monter
                Dim x As Object = _ListuConditions.Item(i - 1)
                _ListuConditions.Item(i - 1) = _ListuConditions.Item(i)
                _ListuConditions.Item(i) = x

                StkCondition.Children.Clear()
                For j As Integer = 0 To _ListuConditions.Count - 1
                    StkCondition.Children.Add(_ListuConditions.Item(j))
                Next
                Exit For
            End If
        Next
    End Sub
#End Region


End Class
