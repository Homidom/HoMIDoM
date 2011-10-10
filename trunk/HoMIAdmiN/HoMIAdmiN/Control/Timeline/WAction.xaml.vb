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
                    obj.IdDevice = Window1.myService.GetAllDevices(IdSrv).Item(Cb1.SelectedIndex).ID
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
                    obj.IdMacro = Window1.myService.GetAllMacros(IdSrv).Item(Cb1.SelectedIndex).ID
                    _ObjAction = obj
                Case HoMIDom.HoMIDom.Action.TypeAction.ActionMail
                    If Cb1.SelectedIndex < 0 Or Txt2.Text = "" Or TxtValue.Text = "" Then
                        MessageBox.Show("Veuillez renseigner tous les champs !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                        Exit Sub
                    End If
                    Dim obj As Action.ActionMail = _ObjAction
                    obj.UserId = Window1.myService.GetAllUsers(IdSrv).Item(Cb1.SelectedIndex).ID
                    obj.Sujet = Txt2.Text
                    obj.Message = TxtValue.Text
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
    End Sub

    Private Sub Cb1_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles Cb1.SelectionChanged
        If _ObjAction.TypeAction = Action.TypeAction.ActionDevice Then
            If Cb1.SelectedIndex >= 0 Then
                Cb2.Items.Clear()
                For i As Integer = 0 To Window1.myService.GetAllDevices(IdSrv).Item(Cb1.SelectedIndex).DeviceAction.Count - 1
                    Cb2.Items.Add(Window1.myService.GetAllDevices(IdSrv).Item(Cb1.SelectedIndex).DeviceAction.Item(i).Nom)
                Next
            End If
        End If
    End Sub

    Public Sub New(ByVal ObjAction As Object)

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        _ObjAction = ObjAction
        Dim _typ As Action.TypeAction
        If _ObjAction IsNot Nothing Then
            _typ = _ObjAction.TypeAction
            Me.Height = 250
            Me.Width = 500
            TabControl1.Visibility = Windows.Visibility.Hidden
            Select Case _typ
                Case HoMIDom.HoMIDom.Action.TypeAction.ActionDevice
                    Dim obj As Action.ActionDevice = _ObjAction

                    'Mise en forme graphique
                    Lbl1.Content = "Device:"
                    Lbl2.Content = "Action:"
                    Lbl2.Visibility = Visibility.Visible
                    Cb2.Visibility = Windows.Visibility.Visible
                    Cb2.Height = 20
                    Txt2.Visibility = Windows.Visibility.Hidden
                    Txt2.Height = 0
                    TxtValue.Height = 21

                    For i As Integer = 0 To Window1.myService.GetAllDevices(IdSrv).Count - 1
                        Cb1.Items.Add(Window1.myService.GetAllDevices(IdSrv).Item(i).Name)
                    Next
                    Dim a As String = ""
                    If obj.IdDevice IsNot Nothing Then
                        a = Window1.myService.ReturnDeviceByID(IdSrv, obj.IdDevice).Name
                        For i As Integer = 0 To Cb1.Items.Count - 1
                            If a = Cb1.Items(i) Then
                                Cb1.SelectedIndex = i
                                Exit For
                            End If
                        Next
                        For i As Integer = 0 To Cb2.Items.Count - 1
                            If obj.Method = Cb2.Items(i) Then
                                Cb2.SelectedIndex = i
                                Exit For
                            End If
                        Next
                        TxtValue.Text = obj.Parametres.Item(0)
                    End If
                Case HoMIDom.HoMIDom.Action.TypeAction.ActionMacro
                    Dim obj As Action.ActionMacro = _ObjAction

                    'Mise en forme graphique
                    Lbl1.Content = "Macro:"
                    Lbl2.Visibility = Visibility.Hidden
                    LblValue.Visibility = Windows.Visibility.Hidden
                    Cb2.Visibility = Windows.Visibility.Hidden
                    Cb2.Height = 0
                    Txt2.Visibility = Windows.Visibility.Hidden
                    Txt2.Height = 20
                    TxtValue.Visibility = Windows.Visibility.Hidden

                    For i As Integer = 0 To Window1.myService.GetAllMacros(IdSrv).Count - 1
                        Cb1.Items.Add(Window1.myService.GetAllMacros(IdSrv).Item(i).Nom)
                    Next
                    Dim a As String = ""
                    If obj.IdMacro IsNot Nothing Then
                        a = Window1.myService.ReturnMacroById(IdSrv, obj.IdMacro).Nom
                        For i As Integer = 0 To Cb1.Items.Count - 1
                            If a = Cb1.Items(i) Then
                                Cb1.SelectedIndex = i
                                Exit For
                            End If
                        Next
                    End If
                Case HoMIDom.HoMIDom.Action.TypeAction.ActionMail
                    Dim obj As Action.ActionMail = _ObjAction

                    'Mise en forme graphique
                    Lbl1.Content = "User:"
                    Lbl2.Content = "Sujet:"
                    LblValue.Content = "Message:"
                    Txt2.Text = ""
                    Cb2.Visibility = Windows.Visibility.Hidden
                    Cb2.Height = 0
                    Txt2.Visibility = Windows.Visibility.Visible
                    Txt2.Height = 20
                    TxtValue.Text = ""
                    TxtValue.Height = 80

                    For i As Integer = 0 To Window1.myService.GetAllUsers(IdSrv).Count - 1
                        Cb1.Items.Add(Window1.myService.GetAllUsers(IdSrv).Item(i).Nom & " " & Window1.myService.GetAllUsers(IdSrv).Item(i).Prenom)
                    Next
                    If obj.UserId IsNot Nothing Then
                        Dim _user As Users.User = Window1.myService.ReturnUserById(IdSrv, obj.UserId)
                        Dim a As String = _user.Nom & " " & _user.Prenom
                        For i As Integer = 0 To Cb1.Items.Count - 1
                            If a = Cb1.Items(i) Then
                                Cb1.SelectedIndex = i
                                Exit For
                            End If
                        Next
                        Txt2.Text = obj.Sujet
                        TxtValue.Text = obj.Message
                    End If
                Case HoMIDom.HoMIDom.Action.TypeAction.ActionIf
                    Me.Height = 650
                    Me.Width = 667
                    StkProperty.Height = 0
                    StkProperty.Width = 0
                    TabControl1.Visibility = Windows.Visibility.Visible
                    Cb2.Visibility = Windows.Visibility.Hidden
                    Cb2.Height = 0
                    Cb1.Visibility = Windows.Visibility.Hidden
                    Txt2.Visibility = Windows.Visibility.Hidden
                    Txt2.Height = 0
                    TxtValue.Visibility = Windows.Visibility.Hidden
                    Lbl1.Visibility = Windows.Visibility.Hidden
                    Lbl2.Visibility = Windows.Visibility.Hidden
                    LblValue.Visibility = Windows.Visibility.Hidden

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

    End Sub

    Private Sub Cb2_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles Cb2.SelectionChanged
        LblValue.Visibility = Windows.Visibility.Hidden
        TxtValue.Visibility = Windows.Visibility.Hidden

        If Cb1.SelectedIndex < 0 Then Exit Sub
        If Cb1.SelectedIndex < 0 Then Exit Sub

        Dim Idx As Integer = Cb2.SelectedIndex
        For j As Integer = 0 To Window1.myService.GetAllDevices(IdSrv).Item(Cb1.SelectedIndex).DeviceAction.Item(Idx).Parametres.Count - 1
            Select Case j
                Case 0
                    LblValue.Content = Window1.myService.GetAllDevices(IdSrv).Item(Cb1.SelectedIndex).DeviceAction.Item(Idx).Parametres.Item(j).Nom & " :"
                    LblValue.Visibility = Windows.Visibility.Visible
                    TxtValue.ToolTip = Window1.myService.GetAllDevices(IdSrv).Item(Cb1.SelectedIndex).DeviceAction.Item(Idx).Parametres.Item(j).Type
                    TxtValue.Visibility = Windows.Visibility.Visible
            End Select
        Next
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
