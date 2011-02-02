Imports HoMIDom.HoMIDom.Device

Partial Public Class uDevice
    '--- Variables ------------------
    Public Event CloseMe(ByVal MyObject As Object)
    Dim _Action As EAction 'Définit si modif ou création d'un device
    Dim _DeviceId As String 'Id du device à modifier
    Public Enum EAction
        Nouveau
        Modifier
    End Enum

    Public Sub New(ByVal Action As EAction, ByVal DeviceId As String)

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        _DeviceId = DeviceId
        _Action = Action

        'Liste les type de devices dans le combo
        For Each value As ListeDevices In [Enum].GetValues(GetType(HoMIDom.HoMIDom.Device.ListeDevices))
            CbType.Items.Add(value.ToString)
        Next

        'Liste les drivers dans le combo
        For i As Integer = 0 To Window1.Obj.Drivers.Count - 1
            CbDriver.Items.Add(Window1.Obj.Drivers.Item(i).nom)
        Next

        If Action = EAction.Nouveau Then 'Nouveau Device

        Else 'Modification d'un Device
            Dim x As Object = Window1.Obj.ReturnDeviceByID(DeviceId)

            If x IsNot Nothing Then 'on a trouvé le device
                TxtNom.Text = x.name
                TxtDescript.Text = x.description
                ChkEnable.IsChecked = x.Enable
                ChKSolo.IsChecked = x.Solo
                CbType.SelectedValue = x.type

                For j As Integer = 0 To Window1.Obj.Drivers.Count - 1
                    If Window1.Obj.Drivers.Item(j).id = x.driverid Then
                        CbDriver.SelectedValue = Window1.Obj.Drivers.Item(j).nom
                        Exit For
                    End If
                Next

                TxtAdresse1.Text = x.adresse1
                TxtAdresse2.Text = x.adresse2
                TxtModele.Text = x.modele
                TxtRefresh.Text = x.refresh

                'Gestion si Device avec Value
                If x.Type = "TEMPERATURE" _
                                   Or x.Type = "HUMIDITE" _
                                   Or x.Type = "TEMPERATURECONSIGNE" _
                                   Or x.Type = "ENERGIETOTALE" _
                                   Or x.Type = "ENERGIEINSTANTANEE" _
                                   Or x.Type = "PLUIETOTAL" _
                                   Or x.Type = "PLUIECOURANT" _
                                   Or x.Type = "VITESSEVENT" _
                                   Or x.Type = "UV" _
                                   Or x.Type = "HUMIDITE" _
                                   Then
                    TxtCorrection.Visibility = Windows.Visibility.Visible
                    TxtCorrection.Text = x.Correction
                    TxtFormatage.Visibility = Windows.Visibility.Visible
                    TxtFormatage.Text = x.Formatage
                    TxtPrecision.Visibility = Windows.Visibility.Visible
                    TxtPrecision.Text = x.Precision
                    TxtValueMax.Visibility = Windows.Visibility.Visible
                    TxtValueMax.Text = x.ValueMax
                    TxtValueMin.Visibility = Windows.Visibility.Visible
                    TxtValueMin.Text = x.valueMin
                    TxtValDef.Visibility = Windows.Visibility.Visible
                    TxtValDef.Text = x.valueDef
                    Label10.Visibility = Windows.Visibility.Visible
                    Label11.Visibility = Windows.Visibility.Visible
                    Label12.Visibility = Windows.Visibility.Visible
                    Label13.Visibility = Windows.Visibility.Visible
                    Label14.Visibility = Windows.Visibility.Visible
                    Label15.Visibility = Windows.Visibility.Visible
                End If
            End If
        End If
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me)
    End Sub

    Private Sub CbType_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles CbType.SelectionChanged

    End Sub

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        Dim x As Object = Window1.Obj.ReturnDeviceByID(_DeviceId)
        x.description = TxtCorrection.Text
    End Sub
End Class
