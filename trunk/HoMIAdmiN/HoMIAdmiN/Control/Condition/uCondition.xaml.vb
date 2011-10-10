Imports HoMIDom.HoMIDom

Public Class uCondition
    Dim _TypeCondition As Action.TypeCondition
    Dim _Operateur As Action.TypeOperateur
    Dim _Signe As Action.TypeSigne
    Dim _DateTime As String = "0#0#0#0#0#0000000#0#0" '#jj#MMM#JJJ
    Dim _IdDevice As String
    Dim _PropertyDevice As String
    Dim _Value As Object

    Public Event DeleteCondition(ByVal uid As String)
    Public Event UpCondition(ByVal uid As String)

    Public Property TypeCondition As Action.TypeCondition
        Get
            Return _TypeCondition
        End Get
        Set(ByVal value As Action.TypeCondition)
            _TypeCondition = value
            Select Case _TypeCondition
                Case Action.TypeCondition.DateTime
                    LblTitre.Content &= " DateTime"
                    StkDevice.Visibility = Visibility.Hidden
                    StkDevice.Children.Clear()
                Case Action.TypeCondition.Device
                    LblTitre.Content &= " Device"
                    StkTime.Visibility = Visibility.Hidden
                    StkTime.Children.Clear()
            End Select
        End Set
    End Property

    Public Property Operateur As Action.TypeOperateur
        Get
            Return _Operateur
        End Get
        Set(ByVal value As Action.TypeOperateur)
            _Operateur = value
            CbOperateur.SelectedIndex = _Operateur
        End Set
    End Property

    Public Property Signe As Action.TypeSigne
        Get
            Return _Signe
        End Get
        Set(ByVal value As Action.TypeSigne)
            _Signe = value
            CbSigne1.SelectedIndex = _Signe
            CbSigne2.SelectedIndex = _Signe
        End Set
    End Property

    Public Property DateTime As String
        Get
            Return _DateTime
        End Get
        Set(ByVal value As String)
            _DateTime = value
            Dim a() As String = _DateTime.Split("#")

            If a(0) = "*" Then
                TxtSc.Text = ""
            Else
                TxtSc.Text = a(0)
                If Stk1.Visibility = Windows.Visibility.Hidden Then
                    ChkHeure.IsChecked = True
                    Stk1.Visibility = Windows.Visibility.Visible
                End If
            End If
            If a(1) = "*" Then
                TxtMn.Text = ""
            Else
                TxtMn.Text = a(1)
                If Stk1.Visibility = Windows.Visibility.Hidden Then
                    ChkHeure.IsChecked = True
                    Stk1.Visibility = Windows.Visibility.Visible
                End If
            End If
            If a(2) = "*" Then
                TxtHr.Text = ""
            Else
                TxtHr.Text = a(2)
                If Stk1.Visibility = Windows.Visibility.Hidden Then
                    ChkHeure.IsChecked = True
                    Stk1.Visibility = Windows.Visibility.Visible
                End If
            End If
            If a(3) = "*" Then
                TxtJr.Text = ""
            Else
                TxtJr.Text = a(3)
                If stk2.Visibility = Windows.Visibility.Hidden Then
                    ChkDate.IsChecked = True
                    stk2.Visibility = Windows.Visibility.Visible
                End If
            End If
            If a(4) = "*" Then
                TxtMs.Text = ""
            Else
                TxtMs.Text = a(4)
                If stk2.Visibility = Windows.Visibility.Hidden Then
                    ChkDate.IsChecked = True
                    stk2.Visibility = Windows.Visibility.Visible
                End If
            End If

            If InStr(a(5), "1") Then Chk1.IsChecked = True
            If InStr(a(5), "2") Then Chk2.IsChecked = True
            If InStr(a(5), "3") Then Chk3.IsChecked = True
            If InStr(a(5), "4") Then Chk4.IsChecked = True
            If InStr(a(5), "5") Then Chk5.IsChecked = True
            If InStr(a(5), "6") Then Chk6.IsChecked = True
            If InStr(a(5), "0") Then Chk7.IsChecked = True
            If a(5) <> "" Then
                If stk3.Visibility = Windows.Visibility.Hidden Then
                    ChkJour.IsChecked = True
                    stk3.Visibility = Windows.Visibility.Visible
                End If
            End If
            If a.Count > 7 Then
                If a(6) = "1" Then ChkLeveS.IsChecked = True
                If a(7) = "1" Then ChkCoucheS.IsChecked = True
            End If
        End Set
    End Property

    Public Property IdDevice As String
        Get
            Return _IdDevice
        End Get
        Set(ByVal value As String)
            _IdDevice = value
            For i As Integer = 0 To Window1.myService.GetAllDevices(IdSrv).Count - 1
                If Window1.myService.GetAllDevices(IdSrv).Item(i).ID = _IdDevice Then
                    CbDevice.SelectedIndex = i
                    Exit For
                End If
            Next
        End Set
    End Property

    Public Property PropertyDevice As String
        Get
            Return _PropertyDevice
        End Get
        Set(ByVal value As String)
            _PropertyDevice = value
            For i As Integer = 0 To CbPropertyDevice.Items.Count - 1
                If CbPropertyDevice.Items(i) = value Then
                    CbPropertyDevice.SelectedIndex = i
                    Exit For
                End If
            Next
        End Set
    End Property

    Public Property Value As Object
        Get
            Return _Value
        End Get
        Set(ByVal value As Object)
            _Value = value
            TxtValue.Text = value
        End Set
    End Property

    Private Sub BtnUp_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnUp.MouseDown
        RaiseEvent UpCondition(Me.Uid)
    End Sub

    Private Sub BtnDelete_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnDelete.MouseDown
        RaiseEvent DeleteCondition(Me.Uid)
    End Sub

    Private Sub CbDevice_DropDownClosed(ByVal sender As Object, ByVal e As System.EventArgs) Handles CbDevice.DropDownClosed
        If CbDevice.SelectedIndex < 0 Then Exit Sub

        CbPropertyDevice.Items.Clear()
        Select Case Window1.myService.GetAllDevices(IdSrv).Item(CbDevice.SelectedIndex).Type
            Case 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27
                CbPropertyDevice.Items.Add("Value")
            Case 17
                CbPropertyDevice.Items.Add("Value")
                CbPropertyDevice.Items.Add("ConditionActuel")
                CbPropertyDevice.Items.Add("TemperatureActuel")
                CbPropertyDevice.Items.Add("HumiditeActuel")
                CbPropertyDevice.Items.Add("VentActuel")
                CbPropertyDevice.Items.Add("JourToday")
                CbPropertyDevice.Items.Add("MinToday")
                CbPropertyDevice.Items.Add("MaxToday")
                CbPropertyDevice.Items.Add("ConditionToday")
                CbPropertyDevice.Items.Add("JourJ1")
                CbPropertyDevice.Items.Add("MinJ1")
                CbPropertyDevice.Items.Add("MaxJ1")
                CbPropertyDevice.Items.Add("ConditionJ1")
                CbPropertyDevice.Items.Add("JourJ2")
                CbPropertyDevice.Items.Add("MinJ2")
                CbPropertyDevice.Items.Add("MaxJ2")
                CbPropertyDevice.Items.Add("ConditionJ2")
                CbPropertyDevice.Items.Add("JourJ3")
                CbPropertyDevice.Items.Add("MinJ3")
                CbPropertyDevice.Items.Add("MaxJ3")
                CbPropertyDevice.Items.Add("ConditionJ3")
        End Select
        For i As Integer = 0 To CbPropertyDevice.Items.Count - 1
            If CbPropertyDevice.Items(i) = _PropertyDevice Then
                CbPropertyDevice.SelectedIndex = i
                Exit For
            End If
        Next
    End Sub

    Public Sub New()

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        For i As Integer = 0 To Window1.myService.GetAllDevices(IdSrv).Count - 1
            CbDevice.Items.Add(Window1.myService.GetAllDevices(IdSrv).Item(i).Name)
        Next

    End Sub

    Private Sub uCondition_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles Me.MouseLeave
        If CbOperateur.SelectedIndex < 0 Then Exit Sub
        _Operateur = CbOperateur.SelectedIndex

        If _TypeCondition = Action.TypeCondition.DateTime Then

            Dim _myconditiontime As String = ""
            If TxtSc.Text = "" Then
                _myconditiontime &= "*#"
            Else
                _myconditiontime &= TxtSc.Text & "#"
            End If
            If TxtMn.Text = "" Then
                _myconditiontime &= "*#"
            Else
                _myconditiontime &= TxtMn.Text & "#"
            End If
            If TxtHr.Text = "" Then
                _myconditiontime &= "*#"
            Else
                _myconditiontime &= TxtHr.Text & "#"
            End If
            If TxtJr.Text = "" Then
                _myconditiontime &= "*#"
            Else
                _myconditiontime &= TxtJr.Text & "#"
            End If
            If TxtMs.Text = "" Then
                _myconditiontime &= "*#"
            Else
                _myconditiontime &= TxtMs.Text & "#"
            End If

            Dim _prepajr As String = ""
            If Chk1.IsChecked = True Then _prepajr = "1"
            If Chk2.IsChecked = True Then
                If _prepajr <> "" Then
                    _prepajr &= ",2"
                Else
                    _prepajr = "2"
                End If
            End If
            If Chk3.IsChecked = True Then
                If _prepajr <> "" Then
                    _prepajr &= ",3"
                Else
                    _prepajr = "3"
                End If
            End If
            If Chk4.IsChecked = True Then
                If _prepajr <> "" Then
                    _prepajr &= ",4"
                Else
                    _prepajr = "4"
                End If
            End If
            If Chk5.IsChecked = True Then
                If _prepajr <> "" Then
                    _prepajr &= ",5"
                Else
                    _prepajr = "5"
                End If
            End If
            If Chk6.IsChecked = True Then
                If _prepajr <> "" Then
                    _prepajr &= ",6"
                Else
                    _prepajr = "6"
                End If
            End If
            If Chk7.IsChecked = True Then
                If _prepajr <> "" Then
                    _prepajr &= ",0"
                Else
                    _prepajr = "0"
                End If
            End If
            _myconditiontime &= _prepajr & "#"

            If ChkLeveS.IsChecked = True Then
                _myconditiontime &= "1#"
            Else
                _myconditiontime &= "0#"
            End If
            If ChkCoucheS.IsChecked = True Then
                _myconditiontime &= "1"
            Else
                _myconditiontime &= "0"
            End If

            _DateTime = _myconditiontime
            _Signe = CbSigne1.SelectedIndex
        Else
            If CbDevice.SelectedIndex >= 0 Then
                _IdDevice = Window1.myService.GetAllDevices(IdSrv).Item(CbDevice.SelectedIndex).ID
                _PropertyDevice = CbPropertyDevice.Text
                _Signe = CbSigne2.SelectedIndex
                _Value = TxtValue.Text
            End If
        End If


    End Sub

    Private Sub CbDevice_SelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles CbDevice.SelectionChanged
        CbPropertyDevice.Items.Clear()
        Select Case Window1.myService.GetAllDevices(IdSrv).Item(CbDevice.SelectedIndex).Type
            Case 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27
                CbPropertyDevice.Items.Add("Value")
            Case 17
                CbPropertyDevice.Items.Add("Value")
                CbPropertyDevice.Items.Add("ConditionActuel")
                CbPropertyDevice.Items.Add("TemperatureActuel")
                CbPropertyDevice.Items.Add("HumiditeActuel")
                CbPropertyDevice.Items.Add("VentActuel")
                CbPropertyDevice.Items.Add("JourToday")
                CbPropertyDevice.Items.Add("MinToday")
                CbPropertyDevice.Items.Add("MaxToday")
                CbPropertyDevice.Items.Add("ConditionToday")
                CbPropertyDevice.Items.Add("JourJ1")
                CbPropertyDevice.Items.Add("MinJ1")
                CbPropertyDevice.Items.Add("MaxJ1")
                CbPropertyDevice.Items.Add("ConditionJ1")
                CbPropertyDevice.Items.Add("JourJ2")
                CbPropertyDevice.Items.Add("MinJ2")
                CbPropertyDevice.Items.Add("MaxJ2")
                CbPropertyDevice.Items.Add("ConditionJ2")
                CbPropertyDevice.Items.Add("JourJ3")
                CbPropertyDevice.Items.Add("MinJ3")
                CbPropertyDevice.Items.Add("MaxJ3")
                CbPropertyDevice.Items.Add("ConditionJ3")
        End Select
        For i As Integer = 0 To CbPropertyDevice.Items.Count - 1
            If CbPropertyDevice.Items(i) = _PropertyDevice Then
                CbPropertyDevice.SelectedIndex = i
                Exit For
            End If
        Next
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
                TxtHr.Text = "00"
            End If
        End If
    End Sub

    Private Sub TxtMn_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtMn.TextChanged
        If TxtMn.Text <> "" Then
            If IsNumeric(TxtMn.Text) = False Then
                TxtMn.Text = "00"
            End If
        End If
    End Sub

    Private Sub TxtSc_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtSc.TextChanged
        If TxtSc.Text <> "" Then
            If IsNumeric(TxtSc.Text) = False Then
                TxtSc.Text = "00"
            End If
        End If
    End Sub

    Private Sub TxtJr_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtJr.TextChanged
        If TxtJr.Text <> "" And TxtJr.Text <> "*" Then
            If IsNumeric(TxtJr.Text) = False Then
                TxtJr.Text = "01"
            End If
        End If
    End Sub

    Private Sub TxtMs_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtMs.TextChanged
        If TxtMs.Text <> "" And TxtJr.Text <> "*" Then
            If IsNumeric(TxtMs.Text) = False Then
                TxtMs.Text = "01"
            End If
        End If
    End Sub
#End Region

    Private Sub ChkDate_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ChkDate.Click
        If ChkDate.IsChecked = True Then
            If CbSigne1.Visibility = Windows.Visibility.Hidden Then CbSigne1.Visibility = Windows.Visibility.Visible
            stk2.Visibility = Windows.Visibility.Visible
            stk3.Visibility = Windows.Visibility.Hidden
            ChkJour.IsChecked = False
            Chk1.IsChecked = False
            Chk2.IsChecked = False
            Chk3.IsChecked = False
            Chk4.IsChecked = False
            Chk5.IsChecked = False
            Chk6.IsChecked = False
            Chk7.IsChecked = False
        Else
            stk2.Visibility = Windows.Visibility.Hidden
            TxtMs.Text = ""
            TxtJr.Text = ""
        End If
    End Sub

    Private Sub ChkJour_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ChkJour.Click
        If ChkJour.IsChecked = True Then
            If ChkHeure.IsChecked = False Then
                CbSigne1.SelectedIndex = 0
                CbSigne1.Visibility = Windows.Visibility.Hidden
            End If

            stk2.Visibility = Windows.Visibility.Hidden
            stk3.Visibility = Windows.Visibility.Visible
            ChkDate.IsChecked = False
            TxtMs.Text = ""
            TxtJr.Text = ""
        Else
            CbSigne1.Visibility = Windows.Visibility.Visible
            stk3.Visibility = Windows.Visibility.Hidden
            Chk1.IsChecked = False
            Chk2.IsChecked = False
            Chk3.IsChecked = False
            Chk4.IsChecked = False
            Chk5.IsChecked = False
            Chk6.IsChecked = False
            Chk7.IsChecked = False
        End If
    End Sub

    Private Sub ChkHeure_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ChkHeure.Click
        If ChkHeure.IsChecked = True Then
            Stk1.Visibility = Windows.Visibility.Visible
            If CbSigne1.Visibility = Windows.Visibility.Hidden Then CbSigne1.Visibility = Windows.Visibility.Visible
        Else
            Stk1.Visibility = Windows.Visibility.Hidden
            TxtHr.Text = ""
            TxtMn.Text = ""
            TxtSc.Text = ""
        End If
    End Sub

    Private Sub ChkLeveS_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ChkLeveS.Checked
        If ChkLeveS.IsChecked = True Then
            If CbSigne1.Visibility = Windows.Visibility.Hidden Then CbSigne1.Visibility = Windows.Visibility.Visible
            Stk1.Visibility = Windows.Visibility.Hidden
            stk2.Visibility = Windows.Visibility.Hidden
            stk3.Visibility = Windows.Visibility.Hidden
            ChkHeure.IsChecked = False
            ChkDate.IsChecked = False
            ChkJour.IsChecked = False
            TxtHr.Text = ""
            TxtMn.Text = ""
            TxtSc.Text = ""
            TxtMs.Text = ""
            TxtJr.Text = ""
            Chk1.IsChecked = False
            Chk2.IsChecked = False
            Chk3.IsChecked = False
            Chk4.IsChecked = False
            Chk5.IsChecked = False
            Chk6.IsChecked = False
            Chk7.IsChecked = False
            ChkCoucheS.IsChecked = False
        End If
    End Sub

    Private Sub ChkCoucheS_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ChkCoucheS.Checked
        If ChkCoucheS.IsChecked = True Then
            If CbSigne1.Visibility = Windows.Visibility.Hidden Then CbSigne1.Visibility = Windows.Visibility.Visible
            Stk1.Visibility = Windows.Visibility.Hidden
            stk2.Visibility = Windows.Visibility.Hidden
            stk3.Visibility = Windows.Visibility.Hidden
            ChkHeure.IsChecked = False
            ChkDate.IsChecked = False
            ChkJour.IsChecked = False
            TxtHr.Text = ""
            TxtMn.Text = ""
            TxtSc.Text = ""
            TxtMs.Text = ""
            TxtJr.Text = ""
            Chk1.IsChecked = False
            Chk2.IsChecked = False
            Chk3.IsChecked = False
            Chk4.IsChecked = False
            Chk5.IsChecked = False
            Chk6.IsChecked = False
            Chk7.IsChecked = False
            ChkLeveS.IsChecked = False
        End If
    End Sub
End Class
