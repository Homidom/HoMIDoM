Imports HoMIDom.HoMIDom

Public Class uCondition
    Dim _TypeCondition As Action.TypeCondition
    Dim _Operateur As Action.TypeOperateur
    Dim _Signe As Action.TypeSigne
    Dim _DateTime As String = "0#0#0#0#0#0000000#0#0" '#jj#MMM#JJJ
    Dim _IdDevice As String = ""
    Dim _PropertyDevice As String = ""
    Dim _Value As Object = Nothing
    Dim _IsFirst As Boolean
    Dim MyMenuItem As New MenuItem
    Dim mycontextmnu As New ContextMenu

    Dim ListeDevices As List(Of TemplateDevice)

    Public Event DeleteCondition(ByVal uid As String)
    Public Event UpCondition(ByVal uid As String)

    Public Property TypeCondition As Action.TypeCondition
        Get
            Return _TypeCondition
        End Get
        Set(ByVal value As Action.TypeCondition)
            Try
                _TypeCondition = value
                Select Case _TypeCondition
                    Case Action.TypeCondition.DateTime
                        LblTitre.Content &= " DateTime"
                        StkDevice.Visibility = Visibility.Hidden
                        StkDevice.Children.Clear()
                    Case Action.TypeCondition.Device
                        LblTitre.Content &= " Composant"
                        StkTime.Visibility = Visibility.Hidden
                        StkTime.Children.Clear()
                End Select
            Catch ex As Exception
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uCondition TypeCondition: " & ex.ToString, "ERREUR", "")
            End Try
        End Set
    End Property

    Public Property Operateur As Action.TypeOperateur
        Get
            Return _Operateur
        End Get
        Set(ByVal value As Action.TypeOperateur)
            Try
                _Operateur = value
                If value = 1 Then CbOperateur.SelectedIndex = 0
                If value = 2 Then CbOperateur.SelectedIndex = 1
            Catch ex As Exception
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uCondition Operateur: " & ex.ToString, "ERREUR", "")
            End Try
        End Set
    End Property

    Public Property IsFirst As Boolean
        Get
            Return _IsFirst
        End Get
        Set(ByVal value As Boolean)
            _IsFirst = value
            If _IsFirst Then
                StkOp.Visibility = Windows.Visibility.Hidden
            Else
                StkOp.Visibility = Windows.Visibility.Visible
                If _Operateur = 0 Then Operateur = 1
            End If
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
            Try
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
                If String.IsNullOrEmpty(a(5)) = False Then
                    If stk3.Visibility = Windows.Visibility.Hidden Then
                        ChkJour.IsChecked = True
                        stk3.Visibility = Windows.Visibility.Visible
                    End If
                End If
                If a.Count > 7 Then
                    If a(6) = "1" Then ChkLeveS.IsChecked = True
                    If a(7) = "1" Then ChkCoucheS.IsChecked = True
                End If
            Catch ex As Exception
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uCondition DateTime: " & ex.ToString, "ERREUR", "")
            End Try
        End Set
    End Property

    Public Property IdDevice As String
        Get
            Return _IdDevice
        End Get
        Set(ByVal value As String)
            Try
                _IdDevice = value
                CbDevice.Text = myService.ReturnDeviceByID(IdSrv, _IdDevice).Name

            Catch ex As Exception
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uCondition IdDevice: " & ex.ToString, "ERREUR", "")
            End Try
        End Set
    End Property

    Public Property PropertyDevice As String
        Get
            Return _PropertyDevice
        End Get
        Set(ByVal value As String)
            Try
                _PropertyDevice = value
                For i As Integer = 0 To CbPropertyDevice.Items.Count - 1
                    If CbPropertyDevice.Items(i) = value Then
                        CbPropertyDevice.SelectedIndex = i
                        Exit For
                    End If
                Next
            Catch ex As Exception
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uCondition PropertyDevice: " & ex.ToString, "ERREUR", "")
            End Try
        End Set
    End Property

    Public Property Value As Object
        Get
            Return _Value
        End Get
        Set(ByVal value As Object)
            Try
                _Value = value
                TxtValue.Text = value.ToString
            Catch ex As Exception
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uCondition Value: " & ex.ToString, "ERREUR", "")
            End Try
        End Set
    End Property

    Private Sub BtnUp_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnUp.MouseDown
        RaiseEvent UpCondition(Me.Uid)
    End Sub

    Private Sub BtnDelete_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnDelete.MouseDown
        RaiseEvent DeleteCondition(Me.Uid)
    End Sub

    Private Sub CbDevice_DropDownClosed(ByVal sender As Object, ByVal e As System.EventArgs) Handles CbDevice.DropDownClosed
        Try
            If CbDevice.SelectedIndex < 0 Then Exit Sub

            CbPropertyDevice.Items.Clear()

            'Select Case myService.GetAllDevices(IdSrv).Item(CbDevice.SelectedIndex).Type
            Select Case ListeDevices.Item(CbDevice.SelectedIndex).Type
                Case 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27
                    CbPropertyDevice.Items.Add("Value")
                    CbPropertyDevice.SelectedIndex = 0
                Case 17
                    CbPropertyDevice.Items.Add("Value")
                    CbPropertyDevice.SelectedIndex = 0
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
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uCondition CbDevice_DropDownClosed: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Public Sub New()

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        Try
            ListeDevices = myService.GetAllDevices(IdSrv)
            CbDevice.ItemsSource = ListeDevices
            CbDevice.DisplayMemberPath = "Name"

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
            AddHandler S4.Click, AddressOf MenuItemDev_Click
            mycontextmnu.Items.Add(S5)
            Dim S6 As New MenuItem
            S6.Header = "SYSTEM_SOLEIL_LEVE"
            S6.Uid = "SYSTEM"
            AddHandler S6.Click, AddressOf MenuItemDev_Click
            mycontextmnu.Items.Add(S6)

            TxtValue.ContextMenu = mycontextmnu

        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur New uCondition: " & ex.ToString)
        End Try
    End Sub

    Private Sub MenuItemDev_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Try
            'TxtValue.ToolTip = "Veuillez passer par le menu via le clic droit pour changer le composant ou sa propriété"
            If sender.uid = "delete99" Then
                TxtValue.Text = ""
                'TxtValue.ToolTip = ""
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
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur: uCondition_MenuItemDev_Click" & ex.ToString)
        End Try
    End Sub

    Private Sub uCondition_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles Me.MouseLeave
        Try
            If CbOperateur.SelectedIndex < 0 Then Exit Sub
            _Operateur = CbOperateur.SelectedIndex + 1
            If IsFirst Then _Operateur = 0

            If _TypeCondition = Action.TypeCondition.DateTime Then

                Dim _myconditiontime As String = ""
                If String.IsNullOrEmpty(TxtSc.Text) = True Then
                    _myconditiontime &= "*#"
                Else
                    _myconditiontime &= TxtSc.Text & "#"
                End If
                If String.IsNullOrEmpty(TxtMn.Text) = True Then
                    _myconditiontime &= "*#"
                Else
                    _myconditiontime &= TxtMn.Text & "#"
                End If
                If String.IsNullOrEmpty(TxtHr.Text) = True Then
                    _myconditiontime &= "*#"
                Else
                    _myconditiontime &= TxtHr.Text & "#"
                End If
                If String.IsNullOrEmpty(TxtJr.Text) = True Then
                    _myconditiontime &= "*#"
                Else
                    If TxtJr.Text = 0 Then
                        _myconditiontime &= "*#"
                    Else
                        _myconditiontime &= TxtJr.Text & "#"
                    End If
                End If
                If String.IsNullOrEmpty(TxtMs.Text) = True Then
                    _myconditiontime &= "*#"
                Else
                    If TxtMs.Text = 0 Then
                        _myconditiontime &= "*#"
                    Else
                        _myconditiontime &= TxtMs.Text & "#"
                    End If
                End If

                Dim _prepajr As String = ""
                If Chk1.IsChecked = True Then _prepajr = "1"
                If Chk2.IsChecked = True Then
                    If String.IsNullOrEmpty(_prepajr) = False Then
                        _prepajr &= ",2"
                    Else
                        _prepajr = "2"
                    End If
                End If
                If Chk3.IsChecked = True Then
                    If String.IsNullOrEmpty(_prepajr) = False Then
                        _prepajr &= ",3"
                    Else
                        _prepajr = "3"
                    End If
                End If
                If Chk4.IsChecked = True Then
                    If String.IsNullOrEmpty(_prepajr) = False Then
                        _prepajr &= ",4"
                    Else
                        _prepajr = "4"
                    End If
                End If
                If Chk5.IsChecked = True Then
                    If String.IsNullOrEmpty(_prepajr) = False Then
                        _prepajr &= ",5"
                    Else
                        _prepajr = "5"
                    End If
                End If
                If Chk6.IsChecked = True Then
                    If String.IsNullOrEmpty(_prepajr) = False Then
                        _prepajr &= ",6"
                    Else
                        _prepajr = "6"
                    End If
                End If
                If Chk7.IsChecked = True Then
                    If String.IsNullOrEmpty(_prepajr) = False Then
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
                    '_IdDevice = myService.GetAllDevices(IdSrv).Item(CbDevice.SelectedIndex).ID
                    _IdDevice = ListeDevices.Item(CbDevice.SelectedIndex).ID
                    _PropertyDevice = CbPropertyDevice.Text
                    _Signe = CbSigne2.SelectedIndex
                    _Value = TxtValue.Text

                End If
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uCondition uCondition_MouseLeave: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Sub CbDevice_SelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles CbDevice.SelectionChanged
        Try
            CbPropertyDevice.Items.Clear()
            'Select Case myService.GetAllDevices(IdSrv).Item(CbDevice.SelectedIndex).Type
            Select Case ListeDevices.Item(CbDevice.SelectedIndex).Type
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
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uCondition CbDevice_SelectionChanged: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

#Region "Gestion Date/time"
    Private Sub BtnPHr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnPHr.Click
        Dim i As Integer
        If String.IsNullOrEmpty(TxtHr.Text) = True Then
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
        If String.IsNullOrEmpty(TxtMn.Text) = True Then
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
        If String.IsNullOrEmpty(TxtSc.Text) = True Then
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
        If String.IsNullOrEmpty(TxtHr.Text) = True Then
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
        If String.IsNullOrEmpty(TxtMn.Text) = True Then
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
        If String.IsNullOrEmpty(TxtSc.Text) = True Then
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
        If String.IsNullOrEmpty(TxtJr.Text) = True Then
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
        If String.IsNullOrEmpty(TxtMs.Text) = True Then
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
        If String.IsNullOrEmpty(TxtJr.Text) = True Then
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
        If String.IsNullOrEmpty(TxtMs.Text) = True Then
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
        If String.IsNullOrEmpty(TxtHr.Text) = False Then
            If IsNumeric(TxtHr.Text) = False Then
                TxtHr.Text = "00"
            End If
        End If
    End Sub

    Private Sub TxtMn_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtMn.TextChanged
        If String.IsNullOrEmpty(TxtMn.Text) = False Then
            If IsNumeric(TxtMn.Text) = False Then
                TxtMn.Text = "00"
            End If
        End If
    End Sub

    Private Sub TxtSc_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtSc.TextChanged
        If String.IsNullOrEmpty(TxtSc.Text) = False Then
            If IsNumeric(TxtSc.Text) = False Then
                TxtSc.Text = "00"
            End If
        End If
    End Sub

    Private Sub TxtJr_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtJr.TextChanged
        If String.IsNullOrEmpty(TxtJr.Text) = False And TxtJr.Text <> "*" Then
            If IsNumeric(TxtJr.Text) = False Then
                TxtJr.Text = "01"
            End If
        End If
    End Sub

    Private Sub TxtMs_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtMs.TextChanged
        If String.IsNullOrEmpty(TxtMs.Text) = False And TxtJr.Text <> "*" Then
            If IsNumeric(TxtMs.Text) = False Then
                TxtMs.Text = "01"
            End If
        End If
    End Sub
#End Region

    Private Sub ChkDate_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ChkDate.Click
        Try
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
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uCondition ChkDate_Click: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Sub ChkJour_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ChkJour.Click
        Try
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
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uCondition ChkJour_Click: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Sub ChkHeure_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ChkHeure.Click
        Try
            If ChkHeure.IsChecked = True Then
                Stk1.Visibility = Windows.Visibility.Visible
                If CbSigne1.Visibility = Windows.Visibility.Hidden Then CbSigne1.Visibility = Windows.Visibility.Visible
            Else
                Stk1.Visibility = Windows.Visibility.Hidden
                TxtHr.Text = ""
                TxtMn.Text = ""
                TxtSc.Text = ""
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uCondition ChkHeure_Click: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Sub ChkLeveS_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ChkLeveS.Checked
        Try
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
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uCondition ChkLeveS_Checked: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Sub ChkCoucheS_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ChkCoucheS.Checked
        Try
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
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uCondition ChkCoucheS_Checked: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Sub TxtValue_MouseLeave(sender As Object, e As System.Windows.Input.MouseEventArgs) Handles TxtValue.MouseLeave
        Dim _flag As Boolean = False

        Try
            If String.IsNullOrEmpty(CbDevice.Text) = False And String.IsNullOrEmpty(CbPropertyDevice.Text) = False And String.IsNullOrEmpty(TxtValue.Text) = False Then
                'Dim _ID As String = myService.GetAllDevices(IdSrv).Item(CbDevice.SelectedIndex).ID
                Dim _ID As String = ListeDevices.Item(CbDevice.SelectedIndex).ID
                Dim _type As String = myService.TypeOfPropertyOfDevice(_ID, CbPropertyDevice.Text)
                Dim _obj As Object = Nothing

                'If TxtValue.Tag IsNot Nothing Then
                '    If String.IsNullOrEmpty(TxtValue.Tag.ToString) = False Then
                '        If TxtValue.Tag.ToString.StartsWith("<") And TxtValue.Tag.ToString.EndsWith(">") And (TxtValue.Tag.ToString.Replace("|", ".").Contains(TxtValue.Text) Or TxtValue.Text.Contains(".")) Then
                '            Exit Sub
                '        End If
                '    End If
                'End If
                If (TxtValue.Text.IndexOf("<") < 0 And TxtValue.Text.IndexOf("{") < 0) Then 'si ce n'est pas un champ calculé

                    If String.IsNullOrEmpty(_type) = False Then
                        Select Case _type
                            Case "string"
                                Try
                                    _obj = CStr(TxtValue.Text)
                                Catch ex As Exception
                                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "Veuillez saisir un type String !!", "Erreur", "TxtValue.Changed")
                                    TxtValue.Text = ""
                                    _flag = True
                                End Try
                            Case "boolean"
                                Try
                                    If IsNumeric(TxtValue.Text) Then
                                        If CInt(TxtValue.Text) <= 0 Then TxtValue.Text = 0
                                        If CInt(TxtValue.Text) > 0 Then TxtValue.Text = 1
                                    End If
                                    _obj = CBool(TxtValue.Text)
                                Catch ex As Exception
                                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "Veuillez saisir un type Boolean (0 pour False et 1 pour True)!!", "Erreur", "TxtValue.TextChanged")
                                    TxtValue.Text = 0
                                    _flag = True
                                End Try
                            Case "byte"
                                Try
                                    _obj = CByte(TxtValue.Text)
                                Catch ex As Exception
                                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "Veuillez saisir un type Byte !!", "Erreur", "TxtValue.TextChanged")
                                    TxtValue.Text = 0
                                    _flag = True
                                End Try
                            Case "char"
                                Try
                                    _obj = CChar(TxtValue.Text)
                                Catch ex As Exception
                                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "Veuillez saisir un type Char !!", "Erreur", "TxtValue.TextChanged")
                                    TxtValue.Text = ""
                                    _flag = True
                                End Try
                            Case "datetime"
                                Try
                                    _obj = CDate(TxtValue.Text)
                                Catch ex As Exception
                                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "Veuillez saisir un type DateTime !!", "Erreur", "TxtValue.TextChanged")
                                    TxtValue.Text = ""
                                    _flag = True
                                End Try
                            Case "decimal"
                                Try
                                    _obj = CDec(TxtValue.Text)
                                Catch ex As Exception
                                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "Veuillez saisir un type Decimal !!", "Erreur", "TxtValue.TextChanged")
                                    TxtValue.Text = 0
                                    _flag = True
                                End Try
                            Case "double"
                                Try
                                    'pas de check pour permettre de saisir <componom.value> et des formulres
                                    '_obj = CDbl(TxtValue.Text)
                                Catch ex As Exception
                                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "Veuillez saisir un type Double !!", "Erreur", "TxtValue.TextChanged")
                                    TxtValue.Text = "0"
                                    _flag = True
                                End Try
                            Case "integer"
                                Try
                                    _obj = CInt(TxtValue.Text)
                                Catch ex As Exception
                                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "Veuillez saisir un type Integer !!", "Erreur", "TxtValue.TextChanged")
                                    TxtValue.Text = "0"
                                    _flag = True
                                End Try
                            Case "int16"
                                Try
                                    _obj = CInt(TxtValue.Text)
                                Catch ex As Exception
                                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "Veuillez saisir un type Integer !!", "Erreur", "TxtValue.TextChanged")
                                    TxtValue.Text = "0"
                                    _flag = True
                                End Try
                            Case "int32"
                                Try
                                    _obj = CInt(TxtValue.Text)
                                Catch ex As Exception
                                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "Veuillez saisir un type Integer !!", "Erreur", "TxtValue.TextChanged")
                                    TxtValue.Text = "0"
                                    _flag = True
                                End Try
                            Case "int64"
                                Try
                                    _obj = CInt(TxtValue.Text)
                                Catch ex As Exception
                                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "Veuillez saisir un type Integer !!", "Erreur", "TxtValue.TextChanged")
                                    TxtValue.Text = "0"
                                    _flag = True
                                End Try
                            Case "single"
                                Try
                                    _obj = CSng(TxtValue.Text)
                                Catch ex As Exception
                                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "Veuillez saisir un type Single !!", "Erreur", "TxtValue.TextChanged")
                                    TxtValue.Text = "0"
                                    _flag = True
                                End Try
                        End Select
                        'TxtValue.Tag = ""
                    End If
                End If
            End If
        Catch ex As Exception
            If _flag = False Then AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uCondition TxtValue_TextChanged: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

End Class
