Public Class uMainMenu
    Public Event menu_gerer(ByVal IndexMenu As String)
    Public Event menu_delete(ByVal IndexMenu As String)
    Public Event menu_create(ByVal IndexMenu As String)
    Public Event menu_edit(ByVal IndexMenu As String)
    Public Event menu_autre(ByVal IndexMenu As String)
    Public Event menu_contextmenu(ByVal IndexMenu As String)

    'action quand on appuie sur une icone principale
    Private Sub Gerer_ContextMenu(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles img_driver.MouseDown, img_composant.MouseDown, _
        img_zone.MouseDown, img_user.MouseDown, img_trigger.MouseDown, img_macro.MouseDown

        RaiseEvent menu_gerer(sender.tag)
    End Sub

    'action quand on appuie sur un sous menu Gérer
    Private Sub Gerer_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles _
        img_composant_gerer.MouseDown, img_zone_gerer.MouseDown, img_user_gerer.MouseDown, _
        img_trigger_gerer.MouseDown, img_macro_gerer.MouseDown, img_driver_gerer.MouseDown

        RaiseEvent menu_gerer(sender.tag)
    End Sub

    'action quand on appuie sur un sous menu Supprimer
    Private Sub Delete_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles img_composant_supprimer.MouseDown, _
        img_macro_supprimer.MouseDown, img_trigger_supprimer.MouseDown, img_user_supprimer.MouseDown, img_zone_supprimer.MouseDown

        'Attention 1002 correspond à enregistrer la config et non supprimer
        RaiseEvent menu_delete(sender.tag)
    End Sub

    'action quand on appuie sur un sous menu Créer
    Private Sub Create_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles img_composant_ajouter.MouseDown, img_user_ajouter.MouseDown, _
        img_zone_ajouter.MouseDown, img_macro_ajouter.MouseDown, img_trigger_ajouterdevice.MouseDown, img_trigger_ajoutertimer.MouseDown, img_module_ajouter.MouseDown, _
        img_module.MouseDown

        RaiseEvent menu_create(sender.tag)
    End Sub

    'action quand on appuie sur un sous menu Editer
    Private Sub Edit_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles img_composant_editer.MouseDown, img_zone_editer.MouseDown, _
        img_driver_editer.MouseDown, img_macro_editer.MouseDown, img_trigger_editer.MouseDown, img_user_editer.MouseDown

        RaiseEvent menu_edit(sender.tag)
    End Sub

    'action quand on appuie sur un sous menu autre
    Private Sub Autre_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles img_histo.MouseDown, img_config_log.MouseDown, _
        img_config_exporter.MouseDown, img_config_importer.MouseDown, img_config_sauvegarder.MouseDown, img_quitter.MouseDown, img_aide.MouseDown, img_multimedia.MouseDown, _
        img_config_gerer.MouseDown, img_config.MouseDown, img_quitter_start.MouseDown, img_quitter_stop.MouseDown

        RaiseEvent menu_autre(sender.tag)
    End Sub

    Sub New()
        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().

        Try
            'ContextMenu Drivers
            Dim ctxMenudrv As New ContextMenu
            ctxMenudrv.Foreground = System.Windows.Media.Brushes.White
            ctxMenudrv.Background = System.Windows.Media.Brushes.LightGray
            ctxMenudrv.BorderBrush = System.Windows.Media.Brushes.Black
            Dim mnu1 As New MenuItem
            mnu1.Header = "Gérer"
            mnu1.Tag = "drv"
            mnu1.Uid = "drv_gerer"
            AddHandler mnu1.Click, AddressOf menu_contextmenuclick
            ctxMenudrv.Items.Add(mnu1)
            Dim mnu2 As New MenuItem
            mnu2.Header = "Modifier"
            mnu2.Tag = "drv"
            mnu2.Uid = "drv_modifier"
            AddHandler mnu2.Click, AddressOf menu_contextmenuclick
            ctxMenudrv.Items.Add(mnu2)
            img_driver.ContextMenu = ctxMenudrv

            'ContextMenu Drivers
            Dim ctxMenuComposants As New ContextMenu
            ctxMenuComposants.Foreground = System.Windows.Media.Brushes.White
            ctxMenuComposants.Background = System.Windows.Media.Brushes.LightGray
            ctxMenuComposants.BorderBrush = System.Windows.Media.Brushes.Black
            Dim mnu11 As New MenuItem
            mnu11.Header = "Gérer"
            mnu11.Tag = "cpt"
            mnu11.Uid = "cpt_gerer"
            AddHandler mnu11.Click, AddressOf menu_contextmenuclick
            ctxMenuComposants.Items.Add(mnu11)
            Dim mnu12 As New MenuItem
            mnu12.Header = "Ajouter"
            mnu12.Tag = "cpt"
            mnu12.Uid = "cpt_ajouter"
            AddHandler mnu12.Click, AddressOf menu_contextmenuclick
            ctxMenuComposants.Items.Add(mnu12)
            Dim mnu13 As New MenuItem
            mnu13.Header = "Modifier"
            mnu13.Tag = "cpt"
            mnu13.Uid = "cpt_modifier"
            AddHandler mnu13.Click, AddressOf menu_contextmenuclick
            ctxMenuComposants.Items.Add(mnu13)
            Dim mnu14 As New MenuItem
            mnu14.Header = "Supprimer"
            mnu14.Tag = "cpt"
            mnu14.Uid = "cpt_supprimer"
            AddHandler mnu14.Click, AddressOf menu_contextmenuclick
            ctxMenuComposants.Items.Add(mnu14)
            img_composant.ContextMenu = ctxMenuComposants

            'ContextMenu Zone
            Dim ctxMenuZone As New ContextMenu
            ctxMenuZone.Foreground = System.Windows.Media.Brushes.White
            ctxMenuZone.Background = System.Windows.Media.Brushes.LightGray
            ctxMenuZone.BorderBrush = System.Windows.Media.Brushes.Black
            Dim mnu21 As New MenuItem
            mnu21.Header = "Gérer"
            mnu21.Tag = "zon"
            mnu21.Uid = "zon_gerer"
            AddHandler mnu21.Click, AddressOf menu_contextmenuclick
            ctxMenuZone.Items.Add(mnu21)
            Dim mnu22 As New MenuItem
            mnu22.Header = "Ajouter"
            mnu22.Tag = "zon"
            mnu22.Uid = "zon_ajouter"
            AddHandler mnu22.Click, AddressOf menu_contextmenuclick
            ctxMenuZone.Items.Add(mnu22)
            Dim mnu23 As New MenuItem
            mnu23.Header = "Modifier"
            mnu23.Tag = "zon"
            mnu23.Uid = "zon_modifier"
            AddHandler mnu23.Click, AddressOf menu_contextmenuclick
            ctxMenuZone.Items.Add(mnu23)
            Dim mnu24 As New MenuItem
            mnu24.Header = "Supprimer"
            mnu24.Tag = "zon"
            mnu24.Uid = "zon_supprimer"
            AddHandler mnu24.Click, AddressOf menu_contextmenuclick
            ctxMenuZone.Items.Add(mnu24)
            img_zone.ContextMenu = ctxMenuZone

            'ContextMenu user
            Dim ctxMenuUser As New ContextMenu
            ctxMenuUser.Foreground = System.Windows.Media.Brushes.White
            ctxMenuUser.Background = System.Windows.Media.Brushes.LightGray
            ctxMenuUser.BorderBrush = System.Windows.Media.Brushes.Black
            Dim mnu31 As New MenuItem
            mnu31.Header = "Gérer"
            mnu31.Tag = "usr"
            mnu31.Uid = "usr_gerer"
            AddHandler mnu31.Click, AddressOf menu_contextmenuclick
            ctxMenuUser.Items.Add(mnu31)
            Dim mnu32 As New MenuItem
            mnu32.Header = "Ajouter"
            mnu32.Tag = "usr"
            mnu32.Uid = "usr_ajouter"
            AddHandler mnu32.Click, AddressOf menu_contextmenuclick
            ctxMenuUser.Items.Add(mnu32)
            Dim mnu33 As New MenuItem
            mnu33.Header = "Modifier"
            mnu33.Tag = "usr"
            mnu33.Uid = "usr_modifier"
            AddHandler mnu33.Click, AddressOf menu_contextmenuclick
            ctxMenuUser.Items.Add(mnu33)
            Dim mnu34 As New MenuItem
            mnu34.Header = "Supprimer"
            mnu34.Tag = "usr"
            mnu34.Uid = "usr_supprimer"
            AddHandler mnu24.Click, AddressOf menu_contextmenuclick
            ctxMenuUser.Items.Add(mnu34)
            img_user.ContextMenu = ctxMenuUser

            'ContextMenu Trigger
            Dim ctxMenuTrigger As New ContextMenu
            ctxMenuTrigger.Foreground = System.Windows.Media.Brushes.White
            ctxMenuTrigger.Background = System.Windows.Media.Brushes.LightGray
            ctxMenuTrigger.BorderBrush = System.Windows.Media.Brushes.Black
            Dim mnu41 As New MenuItem
            mnu41.Header = "Gérer"
            mnu41.Tag = "trg"
            mnu41.Uid = "trg_gerer"
            AddHandler mnu41.Click, AddressOf menu_contextmenuclick
            ctxMenuTrigger.Items.Add(mnu41)
            Dim mnu42 As New MenuItem
            mnu42.Header = "Ajouter un Trigger Timer"
            mnu42.Tag = "trg"
            mnu42.Uid = "trg_ajouter_timer"
            AddHandler mnu42.Click, AddressOf menu_contextmenuclick
            ctxMenuTrigger.Items.Add(mnu42)
            Dim mnu43 As New MenuItem
            mnu43.Header = "Ajouter un Trigger Composant"
            mnu43.Tag = "trg"
            mnu43.Uid = "trg_ajouter_composant"
            AddHandler mnu43.Click, AddressOf menu_contextmenuclick
            ctxMenuTrigger.Items.Add(mnu43)
            Dim mnu44 As New MenuItem
            mnu44.Header = "Modifier"
            mnu44.Tag = "trg"
            mnu44.Uid = "trg_modifier"
            AddHandler mnu44.Click, AddressOf menu_contextmenuclick
            ctxMenuTrigger.Items.Add(mnu44)
            Dim mnu45 As New MenuItem
            mnu45.Header = "Supprimer"
            mnu45.Tag = "trg"
            mnu45.Uid = "trg_supprimer"
            AddHandler mnu45.Click, AddressOf menu_contextmenuclick
            ctxMenuTrigger.Items.Add(mnu45)
            img_trigger.ContextMenu = ctxMenuTrigger

            'ContextMenu macro
            Dim ctxMenuMacro As New ContextMenu
            ctxMenuMacro.Foreground = System.Windows.Media.Brushes.White
            ctxMenuMacro.Background = System.Windows.Media.Brushes.LightGray
            ctxMenuMacro.BorderBrush = System.Windows.Media.Brushes.Black
            Dim mnu51 As New MenuItem
            mnu51.Header = "Gérer"
            mnu51.Tag = "mac"
            mnu51.Uid = "mac_gerer"
            AddHandler mnu51.Click, AddressOf menu_contextmenuclick
            ctxMenuMacro.Items.Add(mnu51)
            Dim mnu52 As New MenuItem
            mnu52.Header = "Ajouter"
            mnu52.Tag = "mac"
            mnu52.Uid = "mac_ajouter"
            AddHandler mnu52.Click, AddressOf menu_contextmenuclick
            ctxMenuMacro.Items.Add(mnu52)
            Dim mnu53 As New MenuItem
            mnu53.Header = "Modifier"
            mnu53.Tag = "mac"
            mnu53.Uid = "mac_modifier"
            AddHandler mnu53.Click, AddressOf menu_contextmenuclick
            ctxMenuMacro.Items.Add(mnu53)
            Dim mnu54 As New MenuItem
            mnu54.Header = "Supprimer"
            mnu54.Tag = "mac"
            mnu54.Uid = "mac_supprimer"
            AddHandler mnu54.Click, AddressOf menu_contextmenuclick
            ctxMenuMacro.Items.Add(mnu54)
            img_macro.ContextMenu = ctxMenuMacro


            If My.Settings.ShowLogError = True Then
                groupBox1.Visibility = Windows.Visibility.Visible
            Else
                groupBox1.Visibility = Windows.Visibility.Collapsed
            End If
            If My.Settings.ShowDeviceNoMaJ = True Then
                groupBox2.Visibility = Windows.Visibility.Visible
            Else
                groupBox2.Visibility = Windows.Visibility.Collapsed
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uMainMenu New: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub menu_contextmenuclick(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        'Select Case sender.tag
        '    Case "drv"
        '        Select Case sender.uid
        '            Case "drv_gerer"
        '                MessageBox.Show("ERREUR Sub menu_contextmenuclick Driver Gerer", "INFO", MessageBoxButton.OK, MessageBoxImage.Information)
        '            Case "drv_modifier"
        '                MessageBox.Show("ERREUR Sub menu_contextmenuclick Driver Modifier", "INFO", MessageBoxButton.OK, MessageBoxImage.Information)
        '        End Select
        'End Select
        RaiseEvent menu_contextmenu(sender.uid)
    End Sub
End Class
