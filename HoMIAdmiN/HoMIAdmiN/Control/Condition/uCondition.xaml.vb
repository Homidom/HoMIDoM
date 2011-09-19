Imports HoMIDom.HoMIDom

Public Class uCondition
    Dim _TypeCondition As Action.TypeCondition
    Dim _Operateur As Action.TypeOperateur
    Dim _Signe As Action.TypeSigne
    Dim _DateTime As String = "0#0#0#0#0#0000000" '#jj#MMM#JJJ
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
            CbSeconde.SelectedIndex = a(0)
            CbMinute.SelectedIndex = a(1)
            CbHeure.SelectedIndex = a(2)
            CbJour.SelectedIndex = a(3)
            CbMois.SelectedIndex = a(4)
            Chk1.IsChecked = Mid(a(5), 1, 1)
            Chk2.IsChecked = Mid(a(5), 2, 1)
            Chk3.IsChecked = Mid(a(5), 3, 1)
            Chk4.IsChecked = Mid(a(5), 4, 1)
            Chk5.IsChecked = Mid(a(5), 5, 1)
            Chk6.IsChecked = Mid(a(5), 6, 1)
            Chk7.IsChecked = Mid(a(5), 7, 1)
        End Set
    End Property

    Public Property IdDevice As String
        Get
            Return _IdDevice
        End Get
        Set(ByVal value As String)
            _IdDevice = value
            For i As Integer = 0 To Window1.myService.GetAllDevices.Count - 1
                If Window1.myService.GetAllDevices.Item(i).ID = _IdDevice Then
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
        Select Case Window1.myService.GetAllDevices.Item(CbDevice.SelectedIndex).Type
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
        For i As Integer = 0 To Window1.myService.GetAllDevices.Count - 1
            CbDevice.Items.Add(Window1.myService.GetAllDevices.Item(i).Name)
        Next
    End Sub

    Private Sub uCondition_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles Me.MouseLeave
        If CbOperateur.SelectedIndex < 0 Then Exit Sub
        _Operateur = CbOperateur.SelectedIndex
        If _TypeCondition = Action.TypeCondition.DateTime Then
            _DateTime = CbSeconde.Text & "#" & CbMinute.Text & "#" & CbHeure.Text & "#" & CbJour.Text & "#" & CbMois.Text & "#"
            If Chk1.IsChecked Then
                _DateTime &= "1"
            Else
                _DateTime &= "0"
            End If
            If Chk2.IsChecked Then
                _DateTime &= "1"
            Else
                _DateTime &= "0"
            End If
            If Chk3.IsChecked Then
                _DateTime &= "1"
            Else
                _DateTime &= "0"
            End If
            If Chk4.IsChecked Then
                _DateTime &= "1"
            Else
                _DateTime &= "0"
            End If
            If Chk5.IsChecked Then
                _DateTime &= "1"
            Else
                _DateTime &= "0"
            End If
            If Chk6.IsChecked Then
                _DateTime &= "1"
            Else
                _DateTime &= "0"
            End If
            If Chk7.IsChecked Then
                _DateTime &= "1"
            Else
                _DateTime &= "0"
            End If
            _Signe = CbSigne1.SelectedIndex
        Else
            If CbDevice.SelectedIndex >= 0 Then
                _IdDevice = Window1.myService.GetAllDevices.Item(CbDevice.SelectedIndex).ID
                _PropertyDevice = CbPropertyDevice.Text
                _Signe = CbSigne2.SelectedIndex
                _Value = TxtValue.Text
            End If
        End If


    End Sub

    Private Sub CbDevice_SelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles CbDevice.SelectionChanged
        CbPropertyDevice.Items.Clear()
        Select Case Window1.myService.GetAllDevices.Item(CbDevice.SelectedIndex).Type
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
End Class
