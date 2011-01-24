<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FRMMere
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
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FRMMere))
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.FichierToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ParametresServeurToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.SaveConfigToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.HistorisationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.LogToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.QuitterToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
        Me.PropertyGrid1 = New System.Windows.Forms.PropertyGrid
        Me.TabControl1 = New System.Windows.Forms.TabControl
        Me.TabPage1 = New System.Windows.Forms.TabPage
        Me.BtnStopDriver = New System.Windows.Forms.Button
        Me.BtnStartDriver = New System.Windows.Forms.Button
        Me.TreeViewDriver = New System.Windows.Forms.TreeView
        Me.TabPage2 = New System.Windows.Forms.TabPage
        Me.Panel6 = New System.Windows.Forms.Panel
        Me.BtnDeleteZone = New System.Windows.Forms.PictureBox
        Me.BtnNewZone = New System.Windows.Forms.PictureBox
        Me.TreeViewZone = New System.Windows.Forms.TreeView
        Me.TabPage3 = New System.Windows.Forms.TabPage
        Me.Panel5 = New System.Windows.Forms.Panel
        Me.BtnDeleteDevice = New System.Windows.Forms.PictureBox
        Me.BtnNewDevice = New System.Windows.Forms.PictureBox
        Me.TreeViewDevice = New System.Windows.Forms.TreeView
        Me.TabPage5 = New System.Windows.Forms.TabPage
        Me.Panel4 = New System.Windows.Forms.Panel
        Me.BtnNewMenu = New System.Windows.Forms.Button
        Me.TreeViewMenus = New System.Windows.Forms.TreeView
        Me.TabPage6 = New System.Windows.Forms.TabPage
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.BtnDelTrigger = New System.Windows.Forms.PictureBox
        Me.BtnNewTrigger = New System.Windows.Forms.PictureBox
        Me.TreeViewTrigger = New System.Windows.Forms.TreeView
        Me.TabPage7 = New System.Windows.Forms.TabPage
        Me.Panel3 = New System.Windows.Forms.Panel
        Me.BtnDelScene = New System.Windows.Forms.PictureBox
        Me.BtnNewScene = New System.Windows.Forms.PictureBox
        Me.TreeViewScene = New System.Windows.Forms.TreeView
        Me.TabPage8 = New System.Windows.Forms.TabPage
        Me.Panel2 = New System.Windows.Forms.Panel
        Me.BtnDelSchedule = New System.Windows.Forms.PictureBox
        Me.BtnNewSchedule = New System.Windows.Forms.PictureBox
        Me.TreeViewSchedule = New System.Windows.Forms.TreeView
        Me.TimerTimer = New System.Windows.Forms.Timer(Me.components)
        Me.StatusStrip1.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.Panel6.SuspendLayout()
        CType(Me.BtnDeleteZone, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BtnNewZone, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage3.SuspendLayout()
        Me.Panel5.SuspendLayout()
        CType(Me.BtnDeleteDevice, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BtnNewDevice, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage5.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.TabPage6.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.BtnDelTrigger, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BtnNewTrigger, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage7.SuspendLayout()
        Me.Panel3.SuspendLayout()
        CType(Me.BtnDelScene, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BtnNewScene, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage8.SuspendLayout()
        Me.Panel2.SuspendLayout()
        CType(Me.BtnDelSchedule, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BtnNewSchedule, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 712)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(1028, 22)
        Me.StatusStrip1.TabIndex = 0
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.BackColor = System.Drawing.Color.Transparent
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(111, 17)
        Me.ToolStripStatusLabel1.Text = "ToolStripStatusLabel1"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FichierToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(1028, 24)
        Me.MenuStrip1.TabIndex = 1
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FichierToolStripMenuItem
        '
        Me.FichierToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ParametresServeurToolStripMenuItem, Me.SaveConfigToolStripMenuItem, Me.HistorisationToolStripMenuItem, Me.LogToolStripMenuItem, Me.QuitterToolStripMenuItem})
        Me.FichierToolStripMenuItem.Name = "FichierToolStripMenuItem"
        Me.FichierToolStripMenuItem.Size = New System.Drawing.Size(50, 20)
        Me.FichierToolStripMenuItem.Text = "Fichier"
        '
        'ParametresServeurToolStripMenuItem
        '
        Me.ParametresServeurToolStripMenuItem.Name = "ParametresServeurToolStripMenuItem"
        Me.ParametresServeurToolStripMenuItem.Size = New System.Drawing.Size(181, 22)
        Me.ParametresServeurToolStripMenuItem.Text = "Parametres Serveur"
        '
        'SaveConfigToolStripMenuItem
        '
        Me.SaveConfigToolStripMenuItem.Name = "SaveConfigToolStripMenuItem"
        Me.SaveConfigToolStripMenuItem.Size = New System.Drawing.Size(181, 22)
        Me.SaveConfigToolStripMenuItem.Text = "Save Config"
        '
        'HistorisationToolStripMenuItem
        '
        Me.HistorisationToolStripMenuItem.Name = "HistorisationToolStripMenuItem"
        Me.HistorisationToolStripMenuItem.Size = New System.Drawing.Size(181, 22)
        Me.HistorisationToolStripMenuItem.Text = "Historisation"
        '
        'LogToolStripMenuItem
        '
        Me.LogToolStripMenuItem.Name = "LogToolStripMenuItem"
        Me.LogToolStripMenuItem.Size = New System.Drawing.Size(181, 22)
        Me.LogToolStripMenuItem.Text = "Log"
        '
        'QuitterToolStripMenuItem
        '
        Me.QuitterToolStripMenuItem.Name = "QuitterToolStripMenuItem"
        Me.QuitterToolStripMenuItem.Size = New System.Drawing.Size(181, 22)
        Me.QuitterToolStripMenuItem.Text = "Quitter"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 24)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.BackgroundImage = CType(resources.GetObject("SplitContainer1.Panel1.BackgroundImage"), System.Drawing.Image)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.PropertyGrid1)
        Me.SplitContainer1.Panel2.Controls.Add(Me.TabControl1)
        Me.SplitContainer1.Size = New System.Drawing.Size(1028, 688)
        Me.SplitContainer1.SplitterDistance = 797
        Me.SplitContainer1.TabIndex = 2
        '
        'PropertyGrid1
        '
        Me.PropertyGrid1.CommandsForeColor = System.Drawing.Color.White
        Me.PropertyGrid1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PropertyGrid1.Location = New System.Drawing.Point(0, 417)
        Me.PropertyGrid1.Name = "PropertyGrid1"
        Me.PropertyGrid1.Size = New System.Drawing.Size(227, 271)
        Me.PropertyGrid1.TabIndex = 0
        Me.PropertyGrid1.ViewBackColor = System.Drawing.Color.DimGray
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Controls.Add(Me.TabPage3)
        Me.TabControl1.Controls.Add(Me.TabPage5)
        Me.TabControl1.Controls.Add(Me.TabPage6)
        Me.TabControl1.Controls.Add(Me.TabPage7)
        Me.TabControl1.Controls.Add(Me.TabPage8)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Top
        Me.TabControl1.Location = New System.Drawing.Point(0, 0)
        Me.TabControl1.Multiline = True
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(227, 422)
        Me.TabControl1.TabIndex = 1
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.BtnStopDriver)
        Me.TabPage1.Controls.Add(Me.BtnStartDriver)
        Me.TabPage1.Controls.Add(Me.TreeViewDriver)
        Me.TabPage1.Location = New System.Drawing.Point(4, 40)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(219, 378)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Driver"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'BtnStopDriver
        '
        Me.BtnStopDriver.Location = New System.Drawing.Point(54, 305)
        Me.BtnStopDriver.Name = "BtnStopDriver"
        Me.BtnStopDriver.Size = New System.Drawing.Size(41, 20)
        Me.BtnStopDriver.TabIndex = 2
        Me.BtnStopDriver.Text = "Stop"
        Me.BtnStopDriver.UseVisualStyleBackColor = True
        '
        'BtnStartDriver
        '
        Me.BtnStartDriver.Location = New System.Drawing.Point(7, 305)
        Me.BtnStartDriver.Name = "BtnStartDriver"
        Me.BtnStartDriver.Size = New System.Drawing.Size(41, 20)
        Me.BtnStartDriver.TabIndex = 1
        Me.BtnStartDriver.Text = "Start"
        Me.BtnStartDriver.UseVisualStyleBackColor = True
        '
        'TreeViewDriver
        '
        Me.TreeViewDriver.BackColor = System.Drawing.Color.DimGray
        Me.TreeViewDriver.Dock = System.Windows.Forms.DockStyle.Top
        Me.TreeViewDriver.ForeColor = System.Drawing.Color.White
        Me.TreeViewDriver.Location = New System.Drawing.Point(3, 3)
        Me.TreeViewDriver.Name = "TreeViewDriver"
        Me.TreeViewDriver.Size = New System.Drawing.Size(213, 293)
        Me.TreeViewDriver.TabIndex = 0
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.Panel6)
        Me.TabPage2.Controls.Add(Me.TreeViewZone)
        Me.TabPage2.Location = New System.Drawing.Point(4, 40)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(219, 378)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Zone"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'Panel6
        '
        Me.Panel6.BackColor = System.Drawing.Color.Black
        Me.Panel6.Controls.Add(Me.BtnDeleteZone)
        Me.Panel6.Controls.Add(Me.BtnNewZone)
        Me.Panel6.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel6.Location = New System.Drawing.Point(3, 315)
        Me.Panel6.Name = "Panel6"
        Me.Panel6.Size = New System.Drawing.Size(213, 60)
        Me.Panel6.TabIndex = 4
        '
        'BtnDeleteZone
        '
        Me.BtnDeleteZone.Image = CType(resources.GetObject("BtnDeleteZone.Image"), System.Drawing.Image)
        Me.BtnDeleteZone.Location = New System.Drawing.Point(43, 3)
        Me.BtnDeleteZone.Name = "BtnDeleteZone"
        Me.BtnDeleteZone.Size = New System.Drawing.Size(33, 33)
        Me.BtnDeleteZone.TabIndex = 8
        Me.BtnDeleteZone.TabStop = False
        '
        'BtnNewZone
        '
        Me.BtnNewZone.Image = CType(resources.GetObject("BtnNewZone.Image"), System.Drawing.Image)
        Me.BtnNewZone.Location = New System.Drawing.Point(4, 3)
        Me.BtnNewZone.Name = "BtnNewZone"
        Me.BtnNewZone.Size = New System.Drawing.Size(33, 33)
        Me.BtnNewZone.TabIndex = 7
        Me.BtnNewZone.TabStop = False
        '
        'TreeViewZone
        '
        Me.TreeViewZone.BackColor = System.Drawing.Color.DimGray
        Me.TreeViewZone.Dock = System.Windows.Forms.DockStyle.Top
        Me.TreeViewZone.ForeColor = System.Drawing.Color.White
        Me.TreeViewZone.Location = New System.Drawing.Point(3, 3)
        Me.TreeViewZone.Name = "TreeViewZone"
        Me.TreeViewZone.Size = New System.Drawing.Size(213, 315)
        Me.TreeViewZone.TabIndex = 3
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.Panel5)
        Me.TabPage3.Controls.Add(Me.TreeViewDevice)
        Me.TabPage3.Location = New System.Drawing.Point(4, 40)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Size = New System.Drawing.Size(219, 378)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "Devices"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'Panel5
        '
        Me.Panel5.BackColor = System.Drawing.Color.Black
        Me.Panel5.Controls.Add(Me.BtnDeleteDevice)
        Me.Panel5.Controls.Add(Me.BtnNewDevice)
        Me.Panel5.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel5.Location = New System.Drawing.Point(0, 318)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(219, 60)
        Me.Panel5.TabIndex = 11
        '
        'BtnDeleteDevice
        '
        Me.BtnDeleteDevice.Image = CType(resources.GetObject("BtnDeleteDevice.Image"), System.Drawing.Image)
        Me.BtnDeleteDevice.Location = New System.Drawing.Point(49, 7)
        Me.BtnDeleteDevice.Name = "BtnDeleteDevice"
        Me.BtnDeleteDevice.Size = New System.Drawing.Size(33, 33)
        Me.BtnDeleteDevice.TabIndex = 6
        Me.BtnDeleteDevice.TabStop = False
        '
        'BtnNewDevice
        '
        Me.BtnNewDevice.Image = CType(resources.GetObject("BtnNewDevice.Image"), System.Drawing.Image)
        Me.BtnNewDevice.Location = New System.Drawing.Point(10, 7)
        Me.BtnNewDevice.Name = "BtnNewDevice"
        Me.BtnNewDevice.Size = New System.Drawing.Size(33, 33)
        Me.BtnNewDevice.TabIndex = 5
        Me.BtnNewDevice.TabStop = False
        '
        'TreeViewDevice
        '
        Me.TreeViewDevice.BackColor = System.Drawing.Color.DimGray
        Me.TreeViewDevice.Dock = System.Windows.Forms.DockStyle.Top
        Me.TreeViewDevice.ForeColor = System.Drawing.Color.White
        Me.TreeViewDevice.Location = New System.Drawing.Point(0, 0)
        Me.TreeViewDevice.Name = "TreeViewDevice"
        Me.TreeViewDevice.Size = New System.Drawing.Size(219, 321)
        Me.TreeViewDevice.TabIndex = 0
        '
        'TabPage5
        '
        Me.TabPage5.Controls.Add(Me.Panel4)
        Me.TabPage5.Controls.Add(Me.TreeViewMenus)
        Me.TabPage5.Location = New System.Drawing.Point(4, 40)
        Me.TabPage5.Name = "TabPage5"
        Me.TabPage5.Size = New System.Drawing.Size(219, 378)
        Me.TabPage5.TabIndex = 4
        Me.TabPage5.Text = "Menus"
        Me.TabPage5.UseVisualStyleBackColor = True
        '
        'Panel4
        '
        Me.Panel4.Controls.Add(Me.BtnNewMenu)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel4.Location = New System.Drawing.Point(0, 338)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(219, 40)
        Me.Panel4.TabIndex = 1
        '
        'BtnNewMenu
        '
        Me.BtnNewMenu.Location = New System.Drawing.Point(8, 8)
        Me.BtnNewMenu.Name = "BtnNewMenu"
        Me.BtnNewMenu.Size = New System.Drawing.Size(25, 25)
        Me.BtnNewMenu.TabIndex = 0
        Me.BtnNewMenu.Text = "+"
        Me.BtnNewMenu.UseVisualStyleBackColor = True
        '
        'TreeViewMenus
        '
        Me.TreeViewMenus.BackColor = System.Drawing.Color.DimGray
        Me.TreeViewMenus.Dock = System.Windows.Forms.DockStyle.Top
        Me.TreeViewMenus.ForeColor = System.Drawing.Color.White
        Me.TreeViewMenus.Location = New System.Drawing.Point(0, 0)
        Me.TreeViewMenus.Name = "TreeViewMenus"
        Me.TreeViewMenus.Size = New System.Drawing.Size(219, 340)
        Me.TreeViewMenus.TabIndex = 0
        '
        'TabPage6
        '
        Me.TabPage6.Controls.Add(Me.Panel1)
        Me.TabPage6.Controls.Add(Me.TreeViewTrigger)
        Me.TabPage6.Location = New System.Drawing.Point(4, 40)
        Me.TabPage6.Name = "TabPage6"
        Me.TabPage6.Size = New System.Drawing.Size(219, 378)
        Me.TabPage6.TabIndex = 5
        Me.TabPage6.Text = "Triggers"
        Me.TabPage6.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.Black
        Me.Panel1.Controls.Add(Me.BtnDelTrigger)
        Me.Panel1.Controls.Add(Me.BtnNewTrigger)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 319)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(219, 59)
        Me.Panel1.TabIndex = 3
        '
        'BtnDelTrigger
        '
        Me.BtnDelTrigger.Image = CType(resources.GetObject("BtnDelTrigger.Image"), System.Drawing.Image)
        Me.BtnDelTrigger.Location = New System.Drawing.Point(42, 6)
        Me.BtnDelTrigger.Name = "BtnDelTrigger"
        Me.BtnDelTrigger.Size = New System.Drawing.Size(33, 33)
        Me.BtnDelTrigger.TabIndex = 4
        Me.BtnDelTrigger.TabStop = False
        '
        'BtnNewTrigger
        '
        Me.BtnNewTrigger.Image = CType(resources.GetObject("BtnNewTrigger.Image"), System.Drawing.Image)
        Me.BtnNewTrigger.Location = New System.Drawing.Point(3, 6)
        Me.BtnNewTrigger.Name = "BtnNewTrigger"
        Me.BtnNewTrigger.Size = New System.Drawing.Size(33, 33)
        Me.BtnNewTrigger.TabIndex = 3
        Me.BtnNewTrigger.TabStop = False
        '
        'TreeViewTrigger
        '
        Me.TreeViewTrigger.AllowDrop = True
        Me.TreeViewTrigger.BackColor = System.Drawing.Color.DimGray
        Me.TreeViewTrigger.Dock = System.Windows.Forms.DockStyle.Top
        Me.TreeViewTrigger.ForeColor = System.Drawing.Color.White
        Me.TreeViewTrigger.Location = New System.Drawing.Point(0, 0)
        Me.TreeViewTrigger.Name = "TreeViewTrigger"
        Me.TreeViewTrigger.Size = New System.Drawing.Size(219, 319)
        Me.TreeViewTrigger.TabIndex = 0
        '
        'TabPage7
        '
        Me.TabPage7.BackColor = System.Drawing.Color.DimGray
        Me.TabPage7.Controls.Add(Me.Panel3)
        Me.TabPage7.Controls.Add(Me.TreeViewScene)
        Me.TabPage7.Location = New System.Drawing.Point(4, 40)
        Me.TabPage7.Name = "TabPage7"
        Me.TabPage7.Size = New System.Drawing.Size(219, 378)
        Me.TabPage7.TabIndex = 6
        Me.TabPage7.Text = "Script"
        Me.TabPage7.UseVisualStyleBackColor = True
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.Color.Black
        Me.Panel3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.Panel3.Controls.Add(Me.BtnDelScene)
        Me.Panel3.Controls.Add(Me.BtnNewScene)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel3.Location = New System.Drawing.Point(0, 318)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(219, 60)
        Me.Panel3.TabIndex = 3
        '
        'BtnDelScene
        '
        Me.BtnDelScene.Image = CType(resources.GetObject("BtnDelScene.Image"), System.Drawing.Image)
        Me.BtnDelScene.Location = New System.Drawing.Point(42, 3)
        Me.BtnDelScene.Name = "BtnDelScene"
        Me.BtnDelScene.Size = New System.Drawing.Size(33, 33)
        Me.BtnDelScene.TabIndex = 8
        Me.BtnDelScene.TabStop = False
        '
        'BtnNewScene
        '
        Me.BtnNewScene.Image = CType(resources.GetObject("BtnNewScene.Image"), System.Drawing.Image)
        Me.BtnNewScene.Location = New System.Drawing.Point(3, 3)
        Me.BtnNewScene.Name = "BtnNewScene"
        Me.BtnNewScene.Size = New System.Drawing.Size(33, 33)
        Me.BtnNewScene.TabIndex = 7
        Me.BtnNewScene.TabStop = False
        '
        'TreeViewScene
        '
        Me.TreeViewScene.BackColor = System.Drawing.Color.DimGray
        Me.TreeViewScene.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TreeViewScene.ForeColor = System.Drawing.Color.White
        Me.TreeViewScene.Location = New System.Drawing.Point(0, 0)
        Me.TreeViewScene.Name = "TreeViewScene"
        Me.TreeViewScene.Size = New System.Drawing.Size(219, 378)
        Me.TreeViewScene.TabIndex = 0
        '
        'TabPage8
        '
        Me.TabPage8.Controls.Add(Me.Panel2)
        Me.TabPage8.Controls.Add(Me.TreeViewSchedule)
        Me.TabPage8.Location = New System.Drawing.Point(4, 40)
        Me.TabPage8.Name = "TabPage8"
        Me.TabPage8.Size = New System.Drawing.Size(219, 378)
        Me.TabPage8.TabIndex = 7
        Me.TabPage8.Text = "Schedule"
        Me.TabPage8.UseVisualStyleBackColor = True
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.Black
        Me.Panel2.Controls.Add(Me.BtnDelSchedule)
        Me.Panel2.Controls.Add(Me.BtnNewSchedule)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel2.Location = New System.Drawing.Point(0, 318)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(219, 60)
        Me.Panel2.TabIndex = 5
        '
        'BtnDelSchedule
        '
        Me.BtnDelSchedule.Image = CType(resources.GetObject("BtnDelSchedule.Image"), System.Drawing.Image)
        Me.BtnDelSchedule.Location = New System.Drawing.Point(42, 6)
        Me.BtnDelSchedule.Name = "BtnDelSchedule"
        Me.BtnDelSchedule.Size = New System.Drawing.Size(33, 33)
        Me.BtnDelSchedule.TabIndex = 6
        Me.BtnDelSchedule.TabStop = False
        '
        'BtnNewSchedule
        '
        Me.BtnNewSchedule.Image = CType(resources.GetObject("BtnNewSchedule.Image"), System.Drawing.Image)
        Me.BtnNewSchedule.Location = New System.Drawing.Point(3, 6)
        Me.BtnNewSchedule.Name = "BtnNewSchedule"
        Me.BtnNewSchedule.Size = New System.Drawing.Size(33, 33)
        Me.BtnNewSchedule.TabIndex = 5
        Me.BtnNewSchedule.TabStop = False
        '
        'TreeViewSchedule
        '
        Me.TreeViewSchedule.BackColor = System.Drawing.Color.DimGray
        Me.TreeViewSchedule.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TreeViewSchedule.ForeColor = System.Drawing.Color.White
        Me.TreeViewSchedule.Location = New System.Drawing.Point(0, 0)
        Me.TreeViewSchedule.Name = "TreeViewSchedule"
        Me.TreeViewSchedule.Size = New System.Drawing.Size(219, 378)
        Me.TreeViewSchedule.TabIndex = 0
        '
        'TimerTimer
        '
        Me.TimerTimer.Enabled = True
        Me.TimerTimer.Interval = 900
        '
        'FRMMere
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Black
        Me.ClientSize = New System.Drawing.Size(1028, 734)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.IsMdiContainer = True
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "FRMMere"
        Me.Text = "HoMIDom Studio"
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage2.ResumeLayout(False)
        Me.Panel6.ResumeLayout(False)
        CType(Me.BtnDeleteZone, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BtnNewZone, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage3.ResumeLayout(False)
        Me.Panel5.ResumeLayout(False)
        CType(Me.BtnDeleteDevice, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BtnNewDevice, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage5.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.TabPage6.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        CType(Me.BtnDelTrigger, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BtnNewTrigger, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage7.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        CType(Me.BtnDelScene, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BtnNewScene, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage8.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        CType(Me.BtnDelSchedule, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BtnNewSchedule, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents PropertyGrid1 As System.Windows.Forms.PropertyGrid
    Friend WithEvents TreeViewDriver As System.Windows.Forms.TreeView
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage5 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage6 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage7 As System.Windows.Forms.TabPage
    Friend WithEvents TreeViewDevice As System.Windows.Forms.TreeView
    Friend WithEvents TreeViewZone As System.Windows.Forms.TreeView
    Friend WithEvents TreeViewTrigger As System.Windows.Forms.TreeView
    Friend WithEvents TabPage8 As System.Windows.Forms.TabPage
    Friend WithEvents TreeViewScene As System.Windows.Forms.TreeView
    Friend WithEvents ToolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents TimerTimer As System.Windows.Forms.Timer
    Friend WithEvents TreeViewSchedule As System.Windows.Forms.TreeView
    Friend WithEvents BtnStopDriver As System.Windows.Forms.Button
    Friend WithEvents BtnStartDriver As System.Windows.Forms.Button
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents BtnNewMenu As System.Windows.Forms.Button
    Friend WithEvents TreeViewMenus As System.Windows.Forms.TreeView
    Friend WithEvents FichierToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ParametresServeurToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LogToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents QuitterToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveConfigToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents BtnDelTrigger As System.Windows.Forms.PictureBox
    Friend WithEvents BtnNewTrigger As System.Windows.Forms.PictureBox
    Friend WithEvents Panel5 As System.Windows.Forms.Panel
    Friend WithEvents BtnDeleteDevice As System.Windows.Forms.PictureBox
    Friend WithEvents BtnNewDevice As System.Windows.Forms.PictureBox
    Friend WithEvents Panel6 As System.Windows.Forms.Panel
    Friend WithEvents BtnDeleteZone As System.Windows.Forms.PictureBox
    Friend WithEvents BtnNewZone As System.Windows.Forms.PictureBox
    Friend WithEvents BtnDelSchedule As System.Windows.Forms.PictureBox
    Friend WithEvents BtnNewSchedule As System.Windows.Forms.PictureBox
    Friend WithEvents BtnDelScene As System.Windows.Forms.PictureBox
    Friend WithEvents BtnNewScene As System.Windows.Forms.PictureBox
    Friend WithEvents HistorisationToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
