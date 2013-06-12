Public Class uMainMenu
    Public Event menu_gerer(ByVal IndexMenu As String)
    Public Event menu_delete(ByVal IndexMenu As String)
    Public Event menu_create(ByVal IndexMenu As String)
    Public Event menu_edit(ByVal IndexMenu As String)
    Public Event menu_autre(ByVal IndexMenu As String)
    Public Event menu_contextmenu(ByVal IndexMenu As String)

    'action quand on appuie sur une icone principale
    Private Sub Gerer_ContextMenu(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles img_driver.MouseLeftButtonDown, _
        img_composant.MouseLeftButtonDown, img_zone.MouseLeftButtonDown, img_user.MouseLeftButtonDown, img_trigger.MouseLeftButtonDown, img_macro.MouseLeftButtonDown
        Try
            If e.ClickCount = 1 Then RaiseEvent menu_gerer(sender.tag)
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uMainMenu Gerer_ContextMenu : " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    'action quand on appuie sur un sous menu Gérer
    Private Sub Gerer_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles _
        img_composant_gerer.MouseLeftButtonDown, img_zone_gerer.MouseLeftButtonDown, img_user_gerer.MouseLeftButtonDown, _
        img_trigger_gerer.MouseLeftButtonDown, img_macro_gerer.MouseLeftButtonDown, img_driver_gerer.MouseLeftButtonDown
        Try
            If e.ClickCount = 1 Then RaiseEvent menu_gerer(sender.tag)
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uMainMenu Gerer_MouseLeftButtonDown : " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    'action quand on appuie sur un sous menu Supprimer
    Private Sub Delete_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles img_composant_supprimer.MouseLeftButtonDown, _
        img_macro_supprimer.MouseLeftButtonDown, img_trigger_supprimer.MouseLeftButtonDown, img_user_supprimer.MouseLeftButtonDown, img_zone_supprimer.MouseLeftButtonDown
        'Attention 1002 correspond à enregistrer la config et non supprimer
        Try
            If e.ClickCount = 1 Then RaiseEvent menu_delete(sender.tag)
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uMainMenu Delete_MouseLeftButtonDown : " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    'action quand on appuie sur un sous menu Créer
    Private Sub Create_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles img_composant_ajouter.MouseLeftButtonDown, _
        img_user_ajouter.MouseLeftButtonDown, img_zone_ajouter.MouseLeftButtonDown, img_macro_ajouter.MouseLeftButtonDown, img_trigger_ajouterdevice.MouseLeftButtonDown, _
        img_trigger_ajoutertimer.MouseLeftButtonDown, img_module_ajouter.MouseLeftButtonDown, img_module.MouseLeftButtonDown
        Try
            If e.ClickCount = 1 Then RaiseEvent menu_create(sender.tag)
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uMainMenu Create_MouseLeftButtonDown : " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    'action quand on appuie sur un sous menu Editer
    Private Sub Edit_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles img_composant_editer.MouseLeftButtonDown, _
        img_zone_editer.MouseLeftButtonDown, img_driver_editer.MouseLeftButtonDown, img_macro_editer.MouseLeftButtonDown, img_trigger_editer.MouseLeftButtonDown, _
        img_user_editer.MouseLeftButtonDown
        Try
            If e.ClickCount = 1 Then RaiseEvent menu_edit(sender.tag)
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uMainMenu Edit_MouseLeftButtonDown : " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    'action quand on appuie sur un sous menu autre
    Private Sub Autre_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles img_histo.MouseLeftButtonDown, _
        img_config.MouseLeftButtonDown, img_config_log.MouseLeftButtonDown, _
        img_config_sauvegarder.MouseLeftButtonDown, img_aide.MouseLeftButtonDown, img_multimedia.MouseDown, _
        img_quitter.MouseLeftButtonDown, img_quitter_start.MouseLeftButtonDown, img_quitter_stop.MouseLeftButtonDown, img_composant_gerer_nvx.MouseLeftButtonDown, _
        img_histo_import.MouseLeftButtonDown
        Try
            If e.ClickCount = 1 Then RaiseEvent menu_autre(sender.tag)
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uMainMenu Autre_MouseLeftButtonDown : " & ex.ToString, "ERREUR", "")
        End Try
    End Sub



    Sub New()
        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().

        Try
            'ContextMenu Drivers
            Dim ctxMenudrv As New ContextMenu
            ctxMenudrv.Foreground = System.Windows.Media.Brushes.Black
            ctxMenudrv.Background = System.Windows.Media.Brushes.LightGray
            ctxMenudrv.BorderBrush = System.Windows.Media.Brushes.Black
            Dim mnu01 As New MenuItem
            mnu01.Header = "Gérer"
            mnu01.Tag = "drv"
            mnu01.Uid = "drv_gerer"
            AddHandler mnu01.Click, AddressOf menu_contextmenuclick
            ctxMenudrv.Items.Add(mnu01)
            mnu01 = New MenuItem
            'Dim mnu02 As New MenuItem
            mnu01.Header = "Modifier"
            mnu01.Tag = "drv"
            mnu01.Uid = "drv_modifier"
            AddHandler mnu01.Click, AddressOf menu_contextmenuclick
            ctxMenudrv.Items.Add(mnu01)
            menu_driver.ContextMenu = ctxMenudrv
            ctxMenudrv = Nothing

            'ContextMenu Composants
            Dim ctxMenuComposants As New ContextMenu
            ctxMenuComposants.Foreground = System.Windows.Media.Brushes.Black
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
            menu_composant.ContextMenu = ctxMenuComposants
            ctxMenuComposants = Nothing

            'ContextMenu Zone
            Dim ctxMenuZone As New ContextMenu
            ctxMenuZone.Foreground = System.Windows.Media.Brushes.Black
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
            menu_zone.ContextMenu = ctxMenuZone
            ctxMenuZone = Nothing

            'ContextMenu user
            Dim ctxMenuUser As New ContextMenu
            ctxMenuUser.Foreground = System.Windows.Media.Brushes.Black
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
            menu_user.ContextMenu = ctxMenuUser
            ctxMenuUser = Nothing

            'ContextMenu Trigger
            Dim ctxMenuTrigger As New ContextMenu
            ctxMenuTrigger.Foreground = System.Windows.Media.Brushes.Black
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
            menu_trigger.ContextMenu = ctxMenuTrigger
            ctxMenuTrigger = Nothing

            'ContextMenu macro
            Dim ctxMenuMacro As New ContextMenu
            ctxMenuMacro.Foreground = System.Windows.Media.Brushes.Black
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
            menu_macro.ContextMenu = ctxMenuMacro
            ctxMenuMacro = Nothing

            'ContextMenu Config
            Dim ctxMenuConfig As New ContextMenu
            ctxMenuConfig.Foreground = System.Windows.Media.Brushes.Black
            ctxMenuConfig.Background = System.Windows.Media.Brushes.LightGray
            ctxMenuConfig.BorderBrush = System.Windows.Media.Brushes.Black
            Dim mnu61 As New MenuItem
            mnu61.Header = "Consulter les logs"
            mnu61.Tag = "cfg"
            mnu61.Uid = "cfg_log"
            AddHandler mnu61.Click, AddressOf menu_contextmenuclick
            ctxMenuConfig.Items.Add(mnu61)
            Dim mnu62 As New MenuItem
            mnu62.Header = "Configurer le serveur"
            mnu62.Tag = "cfg"
            mnu62.Uid = "cfg_configurer"
            AddHandler mnu62.Click, AddressOf menu_contextmenuclick
            ctxMenuConfig.Items.Add(mnu62)

            Dim mnu65 As New MenuItem
            mnu65.Header = "Sauvegarder la configuration"
            mnu65.Tag = "cfg"
            mnu65.Uid = "cfg_sauvegarder"
            AddHandler mnu65.Click, AddressOf menu_contextmenuclick
            ctxMenuConfig.Items.Add(mnu65)
            menu_config.ContextMenu = ctxMenuConfig
            ctxMenuConfig = Nothing

            'ContextMenu Quitter
            Dim ctxMenuQuitter As New ContextMenu
            ctxMenuQuitter.Foreground = System.Windows.Media.Brushes.Black
            ctxMenuQuitter.Background = System.Windows.Media.Brushes.LightGray
            ctxMenuQuitter.BorderBrush = System.Windows.Media.Brushes.Black
            Dim mnu71 As New MenuItem
            mnu71.Header = "Quitter l'interface"
            mnu71.Tag = "qit"
            mnu71.Uid = "qit_quitter"
            AddHandler mnu71.Click, AddressOf menu_contextmenuclick
            ctxMenuQuitter.Items.Add(mnu71)
            Dim mnu72 As New MenuItem
            mnu72.Header = "Arrêter le serveur"
            mnu72.Tag = "qit"
            mnu72.Uid = "qit_stop"
            AddHandler mnu72.Click, AddressOf menu_contextmenuclick
            ctxMenuQuitter.Items.Add(mnu72)
            Dim mnu73 As New MenuItem
            mnu73.Header = "Démarrer le serveur"
            mnu73.Tag = "qit"
            mnu73.Uid = "qit_start"
            AddHandler mnu73.Click, AddressOf menu_contextmenuclick
            ctxMenuQuitter.Items.Add(mnu73)
            menu_quitter.ContextMenu = ctxMenuQuitter
            ctxMenuQuitter = Nothing

            'ContextMenu Histo
            Dim ctxMenuHisto As New ContextMenu
            ctxMenuHisto.Foreground = System.Windows.Media.Brushes.Black
            ctxMenuHisto.Background = System.Windows.Media.Brushes.LightGray
            ctxMenuHisto.BorderBrush = System.Windows.Media.Brushes.Black
            Dim mnu81 As New MenuItem
            mnu81.Header = "Visualiser"
            mnu81.Tag = "hst"
            mnu81.Uid = "hst_gérer"
            AddHandler mnu81.Click, AddressOf menu_contextmenuclick
            ctxMenuHisto.Items.Add(mnu81)
            Dim mnu85 As New MenuItem
            mnu85.Header = "Importer des données d'historiques"
            mnu85.Tag = "hst"
            mnu85.Uid = "hst_importer"
            AddHandler mnu85.Click, AddressOf menu_contextmenuclick
            ctxMenuHisto.Items.Add(mnu85)
            menu_histo.ContextMenu = ctxMenuHisto
            ctxMenuHisto = Nothing

            'ContextMenu Aide
            Dim ctxMenuAide As New ContextMenu
            ctxMenuAide.Foreground = System.Windows.Media.Brushes.Black
            ctxMenuAide.Background = System.Windows.Media.Brushes.LightGray
            ctxMenuAide.BorderBrush = System.Windows.Media.Brushes.Black
            Dim mnu91 As New MenuItem
            mnu91.Header = "Visualiser"
            mnu91.Tag = "aid"
            mnu91.Uid = "aid_gérer"
            AddHandler mnu91.Click, AddressOf menu_contextmenuclick
            ctxMenuAide.Items.Add(mnu91)
            menu_aide.ContextMenu = ctxMenuAide
            ctxMenuAide = Nothing

            'ContextMenu Module
            Dim ctxMenuModule As New ContextMenu
            ctxMenuModule.Foreground = System.Windows.Media.Brushes.Black
            ctxMenuModule.Background = System.Windows.Media.Brushes.LightGray
            ctxMenuModule.BorderBrush = System.Windows.Media.Brushes.Black
            Dim mnu101 As New MenuItem
            mnu101.Header = "Ajouter"
            mnu101.Tag = "mod"
            mnu101.Uid = "mod_ajouter"
            AddHandler mnu101.Click, AddressOf menu_contextmenuclick
            ctxMenuModule.Items.Add(mnu101)
            menu_module.ContextMenu = ctxMenuModule
            ctxMenuModule = Nothing

            Me.UpdateLayout()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub uMainMenu New: " & ex.Message, "ERREUR", "")
        End Try
    End Sub

    Private Sub menu_contextmenuclick(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        RaiseEvent menu_contextmenu(sender.uid)
    End Sub
End Class
