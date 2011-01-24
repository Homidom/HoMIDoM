Imports System.Xml
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Soap
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels.Http
Imports System.Runtime.Remoting.Channels
Imports HoMIDom.HoMIDom
Imports HoMIDom.HoMIDom.Api

Public Class FRMMere

    '***********************************************************
    'Declaration des variables
    '***********************************************************
    Public Obj As HoMIDom.HoMIDom.IHoMIDom
    Public IsConnect As Boolean

#Region "Affichage"
    'Afficher la liste des zones
    Public Sub AffZone()
        'TreeViewZone.Nodes.Clear()
        'For i As Integer = 0 To Obj.Zones.Count - 1
        '    TreeViewZone.Nodes.Add(Obj.Zones.Item(i).ID, Obj.Zones.Item(0).Name)
        'Next
    End Sub

    'Afficher la liste des drivers
    Public Sub AffDriver()
        TreeViewDriver.Nodes.Clear()
        For i As Integer = 0 To Obj.Drivers.Count - 1
            TreeViewDriver.Nodes.Add(Obj.Drivers.Item(i).id, Obj.Drivers.Item(i).Nom)
        Next
    End Sub

    'Afficher la liste des devices
    Public Sub AffDevice()
        TreeViewDevice.Nodes.Clear()
        For i As Integer = 0 To Obj.Devices.Count - 1
            TreeViewDevice.Nodes.Add(Obj.Devices.Item(i).ID, Obj.Devices.Item(i).Name & " {" & Obj.Devices.Item(i).Type & "}")
        Next
    End Sub

    'Afficher la liste des scenes
    Public Sub AffScene()
        'TreeViewScene.Nodes.Clear()
        'For i As Integer = 0 To Obj.Scripts.Count - 1
        '    TreeViewScene.Nodes.Add(Obj.Scripts.Item(i).ID, Obj.Scripts.Item(i).Name)
        'Next
    End Sub

    'Afficher la liste des triggers
    Public Sub AffTrigger()
        'TreeViewTrigger.Nodes.Clear()
        'For i As Integer = 0 To Obj.Triggers.Count - 1
        '    TreeViewTrigger.Nodes.Add(Obj.Triggers.Item(i).ID, Obj.Triggers.Item(i).Name)
        'Next
    End Sub

    'Afficher la liste des schedule
    Public Sub AffSchedule()
        'TreeViewSchedule.Nodes.Clear()
        'For i As Integer = 0 To Obj.Schedules.Count - 1
        '    TreeViewSchedule.Nodes.Add(Obj.Schedules.Item(i).ID, Obj.Schedules.Item(i).Name)
        'Next
    End Sub

#End Region

    'Chargement de la page
    Private Sub FRMMere_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            'Connexion au service Web
            Dim channel As New HttpChannel()
            ChannelServices.RegisterChannel(channel, False)

            Obj = CType(Activator.GetObject( _
                GetType(HoMIDom.HoMIDom.IHoMIDom), _
                "http://localhost:8888/RemoteObjectServer.soap"),  _
                 HoMIDom.HoMIDom.IHoMIDom)

            If Obj IsNot Nothing Then
                IsConnect = True
            End If

            'Afficher les différentes listes
            AffZone()
            AffDevice()
            AffDriver()
            'AffScene()
            'AffTrigger()
            'AffSchedule()

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub StartServer()
        'For i As Integer = 0 To mDriver.Count - 1
        '    If mDriver.Item(i).StartAuto = True Then 'vérifie que le driver doit être automatiqement lancé

        '    End If
        'Next
    End Sub

#Region "Driver"
    'Sélection d'un driver dans la liste
    Private Sub TreeViewDriver_NodeMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles TreeViewDriver.NodeMouseClick
        For i As Integer = 0 To Obj.Drivers.Count - 1
            If Obj.Drivers.Item(i).id = e.Node.Name Then
                PropertyGrid1.SelectedObject = Obj.Drivers.Item(i)
                Exit Sub
            End If
        Next
    End Sub

    'Démarrer le driver
    Private Sub BtnStartDriver_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnStartDriver.Click
        'If TreeViewDriver.SelectedNode.Text <> "" Then
        '    For i As Integer = 0 To Obj.Drivers.Count - 1
        '        If TreeViewDriver.SelectedNode.Text = Obj.Drivers.Item(0).nom Then
        '            Obj.Drivers.Item(i).start()
        '        End If
        '    Next
        'End If
    End Sub

    'Arrêter le driver
    Private Sub BtnStopDriver_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnStopDriver.Click
        'If TreeViewDriver.SelectedNode.Text <> "" Then
        '    For i As Integer = 0 To Obj.Drivers.Count - 1
        '        If TreeViewDriver.SelectedNode.Text = Obj.Drivers.Item(0).nom Then
        '            Obj.Drivers.Item(i).stop()
        '        End If
        '    Next
        'End If
    End Sub
#End Region

#Region "Device"
    'Sélection d'un device dans la liste
    Private Sub TreeViewDevice_NodeMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles TreeViewDevice.NodeMouseClick
        Dim x As Object = Obj.ReturnDeviceByID(e.Node.Name)
        'Afficher ses propriétés
        If x IsNot Nothing Then PropertyGrid1.SelectedObject = x
    End Sub

    'Dble clic sur un device dans la liste
    Private Sub TreeViewDevice_NodeMouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles TreeViewDevice.NodeMouseDoubleClick
        For i As Integer = 0 To Obj.Devices.Count - 1
            If Obj.Devices.Item(i).id = e.Node.Name Then
                'Afficher ses propriétés
                PropertyGrid1.SelectedObject = Obj.Devices.Item(i)

                'Afficher la fenêtre device
                FrmDevice.DeviceID = Obj.Devices.Item(i).id
                FrmDevice.Action = FrmDevice.EAction.Modifier
                FrmDevice.MdiParent = Me
                Me.SplitContainer1.Panel1.Controls.Add(FrmDevice)
                FrmDevice.Show()

            End If
        Next
    End Sub

    Private Sub BtnDeleteDevice_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnDeleteDevice.Click
        Dim i As Integer
        Dim flag As Boolean
        For i = 0 To TreeViewDevice.Nodes.Count - 1
            If TreeViewDevice.Nodes(i).IsSelected Then
                Obj.DeleteDevice(TreeViewDevice.Nodes(i).Name)
                flag = True
            End If
        Next
        If flag = False Then
            MessageBox.Show("Veuillez sélectionner un driver à supprimer")
        End If
        AffDevice()
    End Sub

    'Créer un nouveau device
    Private Sub BtnNewDevice_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnNewDevice.Click
        FrmDevice.Action = FrmDevice.EAction.Nouveau
        FrmDevice.MdiParent = Me
        Me.SplitContainer1.Panel1.Controls.Add(FrmDevice)
        FrmDevice.Show()
    End Sub

#End Region

#Region "Zone"
    Private Sub BtnDeleteZone_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnDeleteZone.Click
        'Dim flag As Boolean
        'For i As Integer = 0 To TreeViewZone.Nodes.Count - 1
        '    If TreeViewZone.Nodes(i).IsSelected Then
        '        Obj.DeleteZone(TreeViewZone.Nodes(i).Name)
        '        flag = True
        '    End If
        'Next
        'If flag = False Then
        '    MessageBox.Show("Veuillez sélectionner une zone à supprimer")
        'End If
        'AffZone()
    End Sub
    Private Sub BtnNewZone_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnNewZone.Click
        'Dim retour As String
        'retour = InputBox("Nouvelle Zone", "Zone:")
        'If retour = "" Then
        '    MessageBox.Show("Le nom de la zone ne peut être vide", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '    Exit Sub
        'End If
        'Obj.SaveZone("", retour, "", Nothing)
        'AffZone()
    End Sub
    Private Sub TreeViewZone_NodeMouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles TreeViewZone.NodeMouseDoubleClick
        FrmZone.MdiParent = Me
        Me.SplitContainer1.Panel1.Controls.Add(FrmZone)
        FrmZone.MyID = e.Node.Name
        FrmZone.Show()
    End Sub

    'affiche les propriétés
    Private Sub TreeViewzone_NodeMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles TreeViewZone.NodeMouseClick
        'Dim x As Zone = Obj.ReturnZoneByID(e.Node.Name)
        ''Afficher ses propriétés
        'If x IsNot Nothing Then PropertyGrid1.SelectedObject = x
    End Sub
#End Region

#Region "Trigger"
    'Affiche les propriétés du trigger
    Private Sub TreeViewTrigger_NodeMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles TreeViewTrigger.NodeMouseClick
        'Dim x As Object = Obj.ReturnTriggerByID(e.Node.Name)
        ''Afficher ses propriétés
        'If x IsNot Nothing Then PropertyGrid1.SelectedObject = x
    End Sub
    Private Sub TreeViewTrigger_NodeMouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles TreeViewTrigger.NodeMouseDoubleClick
        FrmTrigger.Action = FrmTrigger.EAction.Modifier
        FrmTrigger.MdiParent = Me
        Me.SplitContainer1.Panel1.Controls.Add(FrmTrigger)
        FrmTrigger.MyID = e.Node.Name
        FrmTrigger.Show()
    End Sub
    Private Sub BtnDelTrigger_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnDelTrigger.Click
        'Dim flag As Boolean
        'For i As Integer = 0 To TreeViewTrigger.Nodes.Count - 1
        '    If TreeViewTrigger.Nodes(i).IsSelected Then
        '        Obj.DeleteTrigger(TreeViewTrigger.Nodes(i).Name)
        '        flag = True
        '    End If
        'Next
        'If flag = False Then
        '    MessageBox.Show("Veuillez sélectionner un Trigger à supprimer")
        'End If
        'AffTrigger()
    End Sub
    Private Sub BtnNewTrigger_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnNewTrigger.Click
        FrmTrigger.Action = FrmTrigger.EAction.Nouveau
        FrmTrigger.MdiParent = Me
        Me.SplitContainer1.Panel1.Controls.Add(FrmTrigger)
        FrmTrigger.Show()
    End Sub
#End Region

#Region "Scene"
    Private Sub TreeViewScene_NodeMouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles TreeViewScene.NodeMouseDoubleClick
        FrmScene.Action = FrmScene.EAction.Modifier
        FrmScene.MdiParent = Me
        FrmScene.MyID = e.Node.Name
        Me.SplitContainer1.Panel1.Controls.Add(FrmScene)
        FrmScene.Show()
    End Sub
    Private Sub BtnDelScene_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnDelScene.Click
        'Dim flag As Boolean
        'For i As Integer = 0 To TreeViewScene.Nodes.Count - 1
        '    If TreeViewScene.Nodes(i).IsSelected Then
        '        Obj.DeleteMacro(TreeViewScene.Nodes(i).Name)
        '        flag = True
        '    End If
        'Next
        'If flag = False Then
        '    MessageBox.Show("Veuillez sélectionner une scene à supprimer")
        'End If
        'AffScene()
    End Sub
    Private Sub BtnNewScene_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnNewScene.Click
        FrmScene.Action = FrmScene.EAction.Nouveau
        FrmScene.MdiParent = Me
        Me.SplitContainer1.Panel1.Controls.Add(FrmScene)
        FrmScene.Show()
    End Sub
#End Region

#Region "Schedule"
    Private Sub BtnNewSchedule_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnNewSchedule.Click
        FrmSchedule.Action = FrmSchedule.EAction.Nouveau
        FrmSchedule.MdiParent = Me
        Me.SplitContainer1.Panel1.Controls.Add(FrmSchedule)
        FrmSchedule.Show()
    End Sub

    'affiche les propriétés
    Private Sub TreeViewSchedule_NodeMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles TreeViewSchedule.NodeMouseClick
        '    Dim x As Object = Obj.ReturnScheduleByID(e.Node.Name)
        'Afficher ses propriétés
        '   If x IsNot Nothing Then PropertyGrid1.SelectedObject = x
    End Sub

    Private Sub TreeViewSchedule_NodeMouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles TreeViewSchedule.NodeMouseDoubleClick
        FrmSchedule.Action = FrmSchedule.EAction.Modifier
        FrmSchedule.MdiParent = Me
        Me.SplitContainer1.Panel1.Controls.Add(FrmSchedule)
        FrmSchedule.MyID = e.Node.Name
        FrmSchedule.Show()
    End Sub


    Private Sub BtnDelSchedule_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnDelSchedule.Click
        'Dim flag As Boolean
        'For i As Integer = 0 To TreeViewSchedule.Nodes.Count - 1
        '    If TreeViewSchedule.Nodes(i).IsSelected Then
        '        Obj.DeleteSchedule(TreeViewSchedule.Nodes(i).Name)
        '        flag = True
        '    End If
        'Next
        'If flag = False Then
        '    MessageBox.Show("Veuillez sélectionner un schedule à supprimer")
        'End If
        'AffSchedule()
    End Sub
#End Region

    'Quitter l'application
    Private Sub QuitterToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles QuitterToolStripMenuItem.Click
        End
    End Sub

    Private Sub TimerTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimerTimer.Tick
        ToolStripStatusLabel1.Text = Now
    End Sub

    Private Sub ParametresServeurToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ParametresServeurToolStripMenuItem.Click
        FrmConfigProperty.MdiParent = Me
        Me.SplitContainer1.Panel1.Controls.Add(FrmConfigProperty)
        FrmConfigProperty.Show()
    End Sub

    Private Sub LogToolStripMenuItem_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LogToolStripMenuItem.Click
        FrmLog.MdiParent = Me
        Me.SplitContainer1.Panel1.Controls.Add(FrmLog)
        FrmLog.Show()
    End Sub

    'Sauvegarder la configuration dans fichier XML
    Private Sub SaveConfigToolStripMenuItem_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveConfigToolStripMenuItem.Click
        Obj.SaveConfig()
    End Sub

    Private Sub HistorisationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HistorisationToolStripMenuItem.Click
        FrmHisto.MdiParent = Me
        Me.SplitContainer1.Panel1.Controls.Add(FrmHisto)
        FrmHisto.Show()
    End Sub

    Private Sub SplitContainer1_Panel1_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles SplitContainer1.Panel1.Paint

    End Sub
End Class

