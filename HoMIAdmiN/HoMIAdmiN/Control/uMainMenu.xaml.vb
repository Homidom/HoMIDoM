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
End Class
