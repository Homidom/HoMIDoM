Public Class WActionDevice
    Public _ID As String = ""
    Public _Action As String = ""
    Public _Delai As DateTime
    Public _Parametres As New ArrayList

    Public Property ID As String
        Get
            Return _ID
        End Get
        Set(ByVal value As String)
            _ID = value
            Dim a As String = Window1.myService.ReturnDeviceByID(value).Name
            For i As Integer = 0 To CbDevice.Items.Count - 1
                If a = CbDevice.Items(i) Then
                    CbDevice.SelectedIndex = i
                    Exit For
                End If
            Next
        End Set
    End Property

    Public Property Action As String
        Get
            Return _Action
        End Get
        Set(ByVal value As String)
            _Action = value
            For i As Integer = 0 To CbAction.Items.Count - 1
                If CbAction.Items(i) = _Action Then
                    CbAction.SelectedIndex = i
                    Exit For
                End If
            Next
        End Set
    End Property

    Public Property Delai As DateTime
        Get
            Return _Delai
        End Get
        Set(ByVal value As DateTime)
            _Delai = value
            Dim i As Integer
            i = _Delai.Hour
            TxtHr.Text = Format(i, "00")
            i = _Delai.Minute
            TxtMn.Text = Format(i, "00")
            i = _Delai.Second
            TxtSc.Text = Format(i, "00")
        End Set
    End Property

    Public Property Parametres As ArrayList
        Get
            Return _Parametres
        End Get
        Set(ByVal value As ArrayList)
            _Parametres = value
            If _Parametres IsNot Nothing Then
                Select Case _Parametres.Count
                    Case 1
                        TxtP1.Text = _Parametres.Item(0)
                    Case 2
                        TxtP1.Text = _Parametres.Item(0)
                        TxtP2.Text = _Parametres.Item(1)
                    Case 3
                        TxtP1.Text = _Parametres.Item(0)
                        TxtP2.Text = _Parametres.Item(1)
                        TxtP3.Text = _Parametres.Item(2)
                    Case 4
                        TxtP1.Text = _Parametres.Item(0)
                        TxtP2.Text = _Parametres.Item(1)
                        TxtP3.Text = _Parametres.Item(2)
                        TxtP4.Text = _Parametres.Item(3)
                    Case 5
                        TxtP1.Text = _Parametres.Item(0)
                        TxtP2.Text = _Parametres.Item(1)
                        TxtP3.Text = _Parametres.Item(2)
                        TxtP4.Text = _Parametres.Item(3)
                        TxtP5.Text = _Parametres.Item(4)
                End Select
            End If
        End Set
    End Property

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button1.Click
        If CbDevice.Text <> "" Then
            ID = Window1.myService.GetAllDevices.Item(CbDevice.SelectedIndex).ID
        End If
        If CbAction.Text <> "" Then
            Action = CbAction.Text
        End If
        Delai = New System.DateTime(Now.Year, Now.Month, Now.Day, TxtHr.Text, TxtMn.Text, TxtSc.Text)

        _Parametres.Clear()
        If TxtP1.Visibility = Visibility.Visible Then
            _Parametres.Add(TxtP1.Text)
            If TxtP2.Visibility = Visibility.Visible Then
                _Parametres.Add(TxtP2.Text)
                If TxtP3.Visibility = Visibility.Visible Then
                    _Parametres.Add(TxtP3.Text)
                    If TxtP4.Visibility = Visibility.Visible Then
                        _Parametres.Add(TxtP4.Text)
                        If TxtP5.Visibility = Visibility.Visible Then
                            _Parametres.Add(TxtP5.Text)
                        End If
                    End If
                End If
            End If
        End If

        DialogResult = True
    End Sub

    Private Sub CbDevice_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles CbDevice.SelectionChanged
        If CbDevice.SelectedIndex >= 0 Then
            CbAction.Items.Clear()
            For i As Integer = 0 To Window1.myService.GetAllDevices.Item(CbDevice.SelectedIndex).DeviceAction.Count - 1
                CbAction.Items.Add(Window1.myService.GetAllDevices.Item(CbDevice.SelectedIndex).DeviceAction.Item(i).Nom)
            Next
        End If
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

    Public Sub New()

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        For i As Integer = 0 To Window1.myService.GetAllDevices.Count - 1
            CbDevice.Items.Add(Window1.myService.GetAllDevices.Item(i).Name)
        Next
    End Sub

    Private Sub CbAction_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles CbAction.SelectionChanged
        LblP1.Visibility = Windows.Visibility.Hidden
        LblP2.Visibility = Windows.Visibility.Hidden
        LblP3.Visibility = Windows.Visibility.Hidden
        LblP4.Visibility = Windows.Visibility.Hidden
        LblP5.Visibility = Windows.Visibility.Hidden
        'TxtP1.Text = ""
        'TxtP2.Text = ""
        'TxtP3.Text = ""
        'TxtP4.Text = ""
        'TxtP5.Text = ""
        TxtP1.Visibility = Windows.Visibility.Hidden
        TxtP2.Visibility = Windows.Visibility.Hidden
        TxtP3.Visibility = Windows.Visibility.Hidden
        TxtP4.Visibility = Windows.Visibility.Hidden
        TxtP5.Visibility = Windows.Visibility.Hidden

        If CbDevice.SelectedIndex < 0 Then Exit Sub
        If CbAction.SelectedIndex < 0 Then Exit Sub

        Dim Idx As Integer = CbAction.SelectedIndex
        For j As Integer = 0 To Window1.myService.GetAllDevices.Item(CbDevice.SelectedIndex).DeviceAction.Item(Idx).Parametres.Count - 1

            Select Case j
                Case 0
                    LblP1.Content = Window1.myService.GetAllDevices.Item(CbDevice.SelectedIndex).DeviceAction.Item(Idx).Parametres.Item(j).Nom & " :"
                    LblP1.Visibility = Windows.Visibility.Visible
                    TxtP1.ToolTip = Window1.myService.GetAllDevices.Item(CbDevice.SelectedIndex).DeviceAction.Item(Idx).Parametres.Item(j).Type
                    TxtP1.Visibility = Windows.Visibility.Visible
                Case 1
                    LblP2.Content = Window1.myService.GetAllDevices.Item(CbDevice.SelectedIndex).DeviceAction.Item(Idx).Parametres.Item(j).Nom & " :"
                    LblP2.Visibility = Windows.Visibility.Visible
                    TxtP2.ToolTip = Window1.myService.GetAllDevices.Item(CbDevice.SelectedIndex).DeviceAction.Item(Idx).Parametres.Item(j).Type
                    TxtP2.Visibility = Windows.Visibility.Visible
                Case 2
                    LblP3.Content = Window1.myService.GetAllDevices.Item(CbDevice.SelectedIndex).DeviceAction.Item(Idx).Parametres.Item(j).Nom & " :"
                    LblP3.Visibility = Windows.Visibility.Visible
                    TxtP3.ToolTip = Window1.myService.GetAllDevices.Item(CbDevice.SelectedIndex).DeviceAction.Item(Idx).Parametres.Item(j).Type
                    TxtP3.Visibility = Windows.Visibility.Visible
                Case 3
                    LblP4.Content = Window1.myService.GetAllDevices.Item(CbDevice.SelectedIndex).DeviceAction.Item(Idx).Parametres.Item(j).Nom & " :"
                    LblP4.Visibility = Windows.Visibility.Visible
                    TxtP4.ToolTip = Window1.myService.GetAllDevices.Item(CbDevice.SelectedIndex).DeviceAction.Item(Idx).Parametres.Item(j).Type
                    TxtP4.Visibility = Windows.Visibility.Visible
                Case 4
                    LblP5.Content = Window1.myService.GetAllDevices.Item(CbDevice.SelectedIndex).DeviceAction.Item(Idx).Parametres.Item(j).Nom & " :"
                    LblP5.Visibility = Windows.Visibility.Visible
                    TxtP5.ToolTip = Window1.myService.GetAllDevices.Item(CbDevice.SelectedIndex).DeviceAction.Item(Idx).Parametres.Item(j).Type
                    TxtP5.Visibility = Windows.Visibility.Visible
            End Select
        Next
    End Sub

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


End Class
