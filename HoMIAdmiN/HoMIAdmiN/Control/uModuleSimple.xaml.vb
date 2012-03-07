Imports HoMIDom.HoMIDom.Device
Imports HoMIDom.HoMIDom.Api
Imports System.IO

Class uModuleSimple
    Public Event CloseMe(ByVal MyObject As Object)

    Public Sub New()
        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()
        Try
            'Ajoute les choix des modules
            CbType.Items.Add("Associer un interrupteur/detecteur à un appareil/lampe/volet")
            CbType.Items.Add("Associer des interrupteurs/detecteurs à un appareil/lampe/volet")
            CbType.Items.Add("Associer un interrupteur/detecteur à des appareils/lampes/volets")
            CbType.Items.Add("Associer des interrupteurs/detecteurs à des appareils/lampes/volets")

            'Ajout des devices
            For Each Device In myService.GetAllDevices(IdSrv)
                CbEmetteur.Items.Add(Device.Name)
                CbRecepteur.Items.Add(Device.Name)
            Next
        Catch Ex As Exception
            MessageBox.Show("Erreur: " & Ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub


    Private Sub BtnAnnuler_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnAnnuler.Click
        RaiseEvent CloseMe(Me)
    End Sub

    Private Sub BtnAjouter_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnAjouter.Click

        RaiseEvent CloseMe(Me)
    End Sub

    Private Sub BtnHelp_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnHelp.Click
        MessageBox.Show(BtnHelp.ToolTip, "Aide", MessageBoxButton.OK, MessageBoxImage.Question)
    End Sub

End Class
