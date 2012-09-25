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

        RaiseEvent menu_contextmenu(sender.tag)
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

        'ContextMenu Drivers
        Dim ctxMenudrv As New ContextMenu
        ctxMenudrv.Foreground = System.Windows.Media.Brushes.White
        ctxMenudrv.Background = System.Windows.Media.Brushes.LightGray
        ctxMenudrv.BorderBrush = System.Windows.Media.Brushes.Black
        Dim mnu1 As New MenuItem
        mnu1.Header = "Gérer"
        mnu1.Tag = "driver"
        mnu1.Uid = "driver_gerer"
        AddHandler mnu1.Click, AddressOf menu_contextmenuclick
        ctxMenudrv.Items.Add(mnu1)
        Dim mnu2 As New MenuItem
        mnu2.Header = "Modifier"
        mnu2.Tag = "driver"
        mnu2.Uid = "driver_modifier"
        AddHandler mnu2.Click, AddressOf menu_contextmenuclick
        ctxMenudrv.Items.Add(mnu2)
        img_driver.ContextMenu = ctxMenudrv

    End Sub

    Private Sub menu_contextmenuclick(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Select Case sender.tag
            Case "driver"
                Select Case sender.uid
                    Case "driver_gerer"

                    Case "driver_modifier"

                End Select
            Case "composants"
                Select Case sender.uid
                    Case "composants_gerer"

                    Case "composants_modifier"

                    Case "composants_supprimer"

                End Select
        End Select
    End Sub
End Class
