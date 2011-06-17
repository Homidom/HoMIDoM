Imports HoMIDom.HoMIDom

Public Class WActionParametrage
    Dim _ObjAction As Object
    Public _Action As String = ""
    Public _Parametres As New ArrayList

    Public Property ObjAction As Object
        Get
            Return _ObjAction
        End Get
        Set(ByVal value As Object)
            _ObjAction = value
        End Set
    End Property

    Public Property Action As String
        Get
            Return _Action
        End Get
        Set(ByVal value As String)
            _Action = value
            For i As Integer = 0 To Cb2.Items.Count - 1
                If Cb2.Items(i) = _Action Then
                    Cb2.SelectedIndex = i
                    Exit For
                End If
            Next
        End Set
    End Property

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button1.Click
        If Cb1.Text <> "" Then
            'ID = Window1.myService.GetAllDevices.Item(Cb1.SelectedIndex).ID
        End If
        If Cb1.Text <> "" Then
            'Action = Cb2.Text
        End If
        _ObjAction.timing = New System.DateTime(Now.Year, Now.Month, Now.Day, TxtHr.Text, TxtMn.Text, TxtSc.Text)

        _Parametres.Clear()
        If TxtValue.Visibility = Visibility.Visible Then
            _Parametres.Add(TxtValue.Text)
        End If

        DialogResult = True
    End Sub

    Private Sub Cb1_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles Cb1.SelectionChanged
        If Cb1.SelectedIndex >= 0 Then
            Cb2.Items.Clear()
            For i As Integer = 0 To Window1.myService.GetAllDevices.Item(Cb1.SelectedIndex).DeviceAction.Count - 1
                Cb2.Items.Add(Window1.myService.GetAllDevices.Item(Cb1.SelectedIndex).DeviceAction.Item(i).Nom)
            Next
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
            Select Case _typ
                Case HoMIDom.HoMIDom.Action.TypeAction.ActionDevice
                    Dim obj As Action.ActionDevice = _ObjAction

                    'Mise en forme graphique
                    Lbl1.Content = "Device:"
                    Lbl2.Content = "Action:"
                    Lbl2.Visibility = Visibility.Visible
                    Cb2.Visibility = Windows.Visibility.Visible

                    For i As Integer = 0 To Window1.myService.GetAllDevices.Count - 1
                        Cb1.Items.Add(Window1.myService.GetAllDevices.Item(i).Name)
                    Next
                    Dim a As String = Window1.myService.ReturnDeviceByID(obj.IdDevice).Name
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

                    'Select Case obj.Parametres.Count
                    'Case 1
                    TxtValue.Text = obj.Parametres.Item(0)
                    'End Select

                Case HoMIDom.HoMIDom.Action.TypeAction.ActionMail
                    Dim obj As Action.ActionMail = _ObjAction

                    'Mise en forme graphique
                    Lbl1.Content = "User:"
                    Lbl2.Visibility = Visibility.Hidden
                    Cb2.Visibility = Windows.Visibility.Hidden

                    For i As Integer = 0 To Window1.myService.GetAllUsers.Count - 1
                        Cb1.Items.Add(Window1.myService.GetAllUsers.Item(i).Nom & " " & Window1.myService.GetAllUsers.Item(i).Prenom)
                    Next
                    Dim _user As Users.User = Window1.myService.ReturnUserById(obj.UserId)
                    Dim a As String = _user.Nom & " " & _user.Prenom
                    For i As Integer = 0 To Cb1.Items.Count - 1
                        If a = Cb1.Items(i) Then
                            Cb1.SelectedIndex = i
                            Exit For
                        End If
                    Next


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
        For j As Integer = 0 To Window1.myService.GetAllDevices.Item(Cb1.SelectedIndex).DeviceAction.Item(Idx).Parametres.Count - 1
            Select Case j
                Case 0
                    LblValue.Content = Window1.myService.GetAllDevices.Item(Cb1.SelectedIndex).DeviceAction.Item(Idx).Parametres.Item(j).Nom & " :"
                    LblValue.Visibility = Windows.Visibility.Visible
                    TxtValue.ToolTip = Window1.myService.GetAllDevices.Item(Cb1.SelectedIndex).DeviceAction.Item(Idx).Parametres.Item(j).Type
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
End Class
