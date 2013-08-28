<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class HoMIGuI
    Inherits System.Windows.Forms.Form

    'Form remplace la méthode Dispose pour nettoyer la liste des composants.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requise par le Concepteur Windows Form
    Private components As System.ComponentModel.IContainer

    'REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
    'Elle peut être modifiée à l'aide du Concepteur Windows Form.  
    'Ne la modifiez pas à l'aide de l'éditeur de code.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(HoMIGuI))
        Me.homiguiContextMenuStrip = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ConfigurationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ServiceStartToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ServiceStopToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ServiceRestartToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ServiceEtatToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LogsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DossiersToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DossierHomidomStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DossierLogsStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DossierConfigUtilisateurStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DriversToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.NotifyIcon1 = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.homiguiContextMenuStrip.SuspendLayout()
        Me.SuspendLayout()
        '
        'homiguiContextMenuStrip
        '
        Me.homiguiContextMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AboutToolStripMenuItem, Me.ConfigurationToolStripMenuItem, Me.ServiceStartToolStripMenuItem, Me.ServiceStopToolStripMenuItem, Me.ServiceRestartToolStripMenuItem, Me.ServiceEtatToolStripMenuItem, Me.LogsToolStripMenuItem, Me.DossiersToolStripMenuItem, Me.DriversToolStripMenuItem, Me.ExitToolStripMenuItem})
        Me.homiguiContextMenuStrip.Name = "ContextMenuStrip1"
        Me.homiguiContextMenuStrip.Size = New System.Drawing.Size(159, 246)
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.Image = Global.HoMIGuI.My.Resources.Resources.help
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(158, 22)
        Me.AboutToolStripMenuItem.Text = "About"
        '
        'ConfigurationToolStripMenuItem
        '
        Me.ConfigurationToolStripMenuItem.Enabled = False
        Me.ConfigurationToolStripMenuItem.Image = Global.HoMIGuI.My.Resources.Resources.settings
        Me.ConfigurationToolStripMenuItem.Name = "ConfigurationToolStripMenuItem"
        Me.ConfigurationToolStripMenuItem.Size = New System.Drawing.Size(158, 22)
        Me.ConfigurationToolStripMenuItem.Text = "Configuration"
        '
        'ServiceStartToolStripMenuItem
        '
        Me.ServiceStartToolStripMenuItem.Enabled = False
        Me.ServiceStartToolStripMenuItem.Image = Global.HoMIGuI.My.Resources.Resources.play
        Me.ServiceStartToolStripMenuItem.Name = "ServiceStartToolStripMenuItem"
        Me.ServiceStartToolStripMenuItem.Size = New System.Drawing.Size(158, 22)
        Me.ServiceStartToolStripMenuItem.Text = "Service - Start"
        Me.ServiceStartToolStripMenuItem.ToolTipText = "Démarrer le service"
        '
        'ServiceStopToolStripMenuItem
        '
        Me.ServiceStopToolStripMenuItem.Enabled = False
        Me.ServiceStopToolStripMenuItem.Image = Global.HoMIGuI.My.Resources.Resources.stopped
        Me.ServiceStopToolStripMenuItem.Name = "ServiceStopToolStripMenuItem"
        Me.ServiceStopToolStripMenuItem.Size = New System.Drawing.Size(158, 22)
        Me.ServiceStopToolStripMenuItem.Text = "Service - Stop"
        Me.ServiceStopToolStripMenuItem.ToolTipText = "Arrêter le service"
        '
        'ServiceRestartToolStripMenuItem
        '
        Me.ServiceRestartToolStripMenuItem.Enabled = False
        Me.ServiceRestartToolStripMenuItem.Image = Global.HoMIGuI.My.Resources.Resources.restart
        Me.ServiceRestartToolStripMenuItem.Name = "ServiceRestartToolStripMenuItem"
        Me.ServiceRestartToolStripMenuItem.Size = New System.Drawing.Size(158, 22)
        Me.ServiceRestartToolStripMenuItem.Text = "Service - Restart"
        Me.ServiceRestartToolStripMenuItem.ToolTipText = "Redémarrer le service"
        '
        'ServiceEtatToolStripMenuItem
        '
        Me.ServiceEtatToolStripMenuItem.Enabled = False
        Me.ServiceEtatToolStripMenuItem.Name = "ServiceEtatToolStripMenuItem"
        Me.ServiceEtatToolStripMenuItem.Size = New System.Drawing.Size(158, 22)
        Me.ServiceEtatToolStripMenuItem.Text = "Service - Etat"
        Me.ServiceEtatToolStripMenuItem.ToolTipText = "Etat actuel du service"
        '
        'LogsToolStripMenuItem
        '
        Me.LogsToolStripMenuItem.Image = Global.HoMIGuI.My.Resources.Resources.edit
        Me.LogsToolStripMenuItem.Name = "LogsToolStripMenuItem"
        Me.LogsToolStripMenuItem.Size = New System.Drawing.Size(158, 22)
        Me.LogsToolStripMenuItem.Text = "Logs"
        Me.LogsToolStripMenuItem.ToolTipText = "Visualiser les logs en temps réel"
        '
        'DossiersToolStripMenuItem
        '
        Me.DossiersToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DossierHomidomStripMenuItem, Me.DossierLogsStripMenuItem, Me.DossierConfigUtilisateurStripMenuItem})
        Me.DossiersToolStripMenuItem.Image = Global.HoMIGuI.My.Resources.Resources.dossier
        Me.DossiersToolStripMenuItem.Name = "DossiersToolStripMenuItem"
        Me.DossiersToolStripMenuItem.Size = New System.Drawing.Size(158, 22)
        Me.DossiersToolStripMenuItem.Text = "Dossiers"
        Me.DossiersToolStripMenuItem.ToolTipText = "Ouvrir les dossiers locaux"
        '
        'DossierHomidomStripMenuItem
        '
        Me.DossierHomidomStripMenuItem.Name = "DossierHomidomStripMenuItem"
        Me.DossierHomidomStripMenuItem.Size = New System.Drawing.Size(204, 22)
        Me.DossierHomidomStripMenuItem.Text = "HoMIDoM"
        Me.DossierHomidomStripMenuItem.ToolTipText = "Dossier ou est installé HoMIDoM"
        '
        'DossierLogsStripMenuItem
        '
        Me.DossierLogsStripMenuItem.Name = "DossierLogsStripMenuItem"
        Me.DossierLogsStripMenuItem.Size = New System.Drawing.Size(204, 22)
        Me.DossierLogsStripMenuItem.Text = "Logs"
        Me.DossierLogsStripMenuItem.ToolTipText = "Dossier contenant les logs du service"
        '
        'DossierConfigUtilisateurStripMenuItem
        '
        Me.DossierConfigUtilisateurStripMenuItem.Name = "DossierConfigUtilisateurStripMenuItem"
        Me.DossierConfigUtilisateurStripMenuItem.Size = New System.Drawing.Size(204, 22)
        Me.DossierConfigUtilisateurStripMenuItem.Text = "Configuration Utilisateur"
        Me.DossierConfigUtilisateurStripMenuItem.ToolTipText = "Dossier contenant la configuration coté Utilisateur"
        '
        'DriversToolStripMenuItem
        '
        Me.DriversToolStripMenuItem.Image = Global.HoMIGuI.My.Resources.Resources.Driver_32
        Me.DriversToolStripMenuItem.Name = "DriversToolStripMenuItem"
        Me.DriversToolStripMenuItem.Size = New System.Drawing.Size(158, 22)
        Me.DriversToolStripMenuItem.Text = "Drivers"
        Me.DriversToolStripMenuItem.ToolTipText = "Configurer la connexion SOAP du GUI"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Image = Global.HoMIGuI.My.Resources.Resources.delete
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(158, 22)
        Me.ExitToolStripMenuItem.Text = "Exit"
        Me.ExitToolStripMenuItem.ToolTipText = "Quitter HoMIGuI (le serveur reste actif)"
        '
        'NotifyIcon1
        '
        Me.NotifyIcon1.ContextMenuStrip = Me.homiguiContextMenuStrip
        Me.NotifyIcon1.Icon = CType(resources.GetObject("NotifyIcon1.Icon"), System.Drawing.Icon)
        Me.NotifyIcon1.Text = "HoMIGuI"
        Me.NotifyIcon1.Visible = True
        '
        'HoMIGuI
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(284, 262)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "HoMIGuI"
        Me.ShowInTaskbar = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "HoMIGuI"
        Me.WindowState = System.Windows.Forms.FormWindowState.Minimized
        Me.homiguiContextMenuStrip.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents homiguiContextMenuStrip As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents AboutToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ConfigurationToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ServiceStartToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ServiceStopToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ServiceRestartToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LogsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DossiersToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DossierHomidomStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DossierLogsStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DossierConfigUtilisateurStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ServiceEtatToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NotifyIcon1 As System.Windows.Forms.NotifyIcon
    Friend WithEvents DriversToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
