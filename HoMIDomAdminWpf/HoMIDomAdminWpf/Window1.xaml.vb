Imports System.Windows.Threading
Imports System.Runtime.Serialization.Formatters.Soap
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels.Http
Imports System.Runtime.Remoting.Channels
Imports HoMIDom.HoMIDom

Class Window1

    Public Shared Obj As IHoMIDom
    Public Shared IsConnect As Boolean

    Public Sub New()

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        Dim dt As DispatcherTimer = New DispatcherTimer()
        AddHandler dt.Tick, AddressOf dispatcherTimer_Tick
        dt.Interval = New TimeSpan(0, 0, 1)
        dt.Start()

        'Connexion au serveur
        'Connexion au service Web
        Try
            Dim channel As New HttpChannel()
            ChannelServices.RegisterChannel(channel, False)

            Obj = CType(Activator.GetObject( _
                GetType(HoMIDom.HoMIDom.IHoMIDom), _
                "http://localhost:8888/RemoteObjectServer.soap"),  _
                 HoMIDom.HoMIDom.IHoMIDom)

            If Obj IsNot Nothing Then
                IsConnect = True
            End If

            AffDriver()
            AffDevice()
        Catch ex As Exception
            IsConnect = False
        End Try
    End Sub

    'Affiche la date et heure, heures levé et couché du soleil
    Public Sub dispatcherTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        If IsConnect = True Then
            LblStatus.Content = Now.ToLongDateString & " " & Now.ToShortTimeString & "      " & "Serveur connecté"
        Else
            LblStatus.Content = Now.ToLongDateString & " " & Now.ToShortTimeString & "      " & "Serveur non connecté"
        End If
    End Sub

    'Menu Quitter
    Private Sub Quitter(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuItem1.Click
        End
    End Sub

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
        TreeViewDriver.Items.Clear()
        For i As Integer = 0 To Obj.Drivers.Count - 1
            Dim newchild As New TreeViewItem
            newchild.Header = Obj.Drivers.Item(i).Nom
            newchild.Uid = Obj.Drivers.Item(i).id
            TreeViewDriver.Items.Add(newchild)
        Next
    End Sub

    'Afficher la liste des devices
    Public Sub AffDevice()
        TreeViewDevice.Items.Clear()
        For i As Integer = 0 To Obj.Devices.Count - 1
            Dim newchild As New TreeViewItem
            newchild.Header = Obj.Devices.Item(i).Name
            newchild.Uid = Obj.Devices.Item(i).id
            TreeViewDevice.Items.Add(newchild)
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

#Region "Drivers"
    Private Sub TreeViewDriver_SelectedItemChanged(ByVal sender As Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of Object)) Handles TreeViewDriver.SelectedItemChanged
        For i As Integer = 0 To Obj.Drivers.Count - 1
            If Obj.Drivers.Item(i).id = e.NewValue.uid Then
                PropertyGrid1.SelectedObject = Obj.Drivers.Item(i)
                Exit Sub
            End If
        Next
    End Sub

    Private Sub BtnStop_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnStop.Click
        If TreeViewDriver.SelectedItem IsNot Nothing Then
            For i As Integer = 0 To Obj.Drivers.Count - 1
                If TreeViewDriver.SelectedItem.Header = Obj.Drivers.Item(0).Nom Then
                    Obj.Drivers.Item(i).Stop()
                End If
            Next
        Else
            MessageBox.Show("Veuillez sélectionner un Driver!")
        End If
    End Sub

    Private Sub BtnStart_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnStart.Click
        If TreeViewDriver.SelectedItem IsNot Nothing Then
            For i As Integer = 0 To Obj.Drivers.Count - 1
                If TreeViewDriver.SelectedItem.Header = Obj.Drivers.Item(0).Nom Then
                    Obj.Drivers.Item(i).Start()
                End If
            Next
        Else
            MessageBox.Show("Veuillez sélectionner un Driver!")
        End If
    End Sub
#End Region

#Region "Devices"
    'Modifier un device
    Private Sub TreeViewDevice_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles TreeViewDevice.MouseDoubleClick
        If TreeViewDevice.SelectedItem IsNot Nothing Then
            For i As Integer = 0 To Obj.Devices.Count - 1
                If Obj.Devices.Item(i).id = TreeViewDevice.SelectedItem.uid Then
                    PropertyGrid1.SelectedObject = Obj.Devices.Item(i)

                    Dim x As New uDevice(uDevice.EAction.Modifier, TreeViewDevice.SelectedItem.uid)
                    x.Uid = System.Guid.NewGuid.ToString()
                    AddHandler x.CloseMe, AddressOf UnloadControl
                    CanvasRight.Children.Add(x)
                    CanvasRight.SetLeft(x, 50)
                    CanvasRight.SetTop(x, 5)

                    Exit Sub
                End If
            Next
        End If
    End Sub

    'Bouton nouveau Device
    Private Sub BtnNewDevice_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnNewDevice.Click
        Dim x As New uDevice(uDevice.EAction.Nouveau, "")
        x.Uid = System.Guid.NewGuid.ToString()
        AddHandler x.CloseMe, AddressOf UnloadControl
        CanvasRight.Children.Add(x)
        CanvasRight.SetLeft(x, 50)
        CanvasRight.SetTop(x, 5)
    End Sub

    'Bouton supprimer device
    Private Sub BtnDelDevice_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDelDevice.Click
        If TreeViewDevice.SelectedItem IsNot Nothing Then
            Window1.Obj.DeleteDevice(TreeViewDevice.SelectedItem.uid)
            AffDevice()
        Else
            MessageBox.Show("Veuillez sélectionner un Device à supprimer!")
        End If
    End Sub
#End Region

    'Menu Save Config
    Private Sub MnuSaveConfig(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuItem2.Click
        Obj.SaveConfig()
    End Sub

    'Décharger une fenêtre suivant son Id
    Public Sub UnloadControl(ByVal MyControl As Object)
        For i As Integer = 0 To CanvasRight.Children.Count - 1
            If CanvasRight.Children.Item(i).Uid = MyControl.uid Then
                CanvasRight.Children.RemoveAt(i)
                AffDevice()
                Exit Sub
            End If
        Next

    End Sub

    'Menu Sauvegarder la config
    Private Sub MnuConfigSrv(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuItem3.Click
        Dim x As New uConfigServer
        x.Uid = System.Guid.NewGuid.ToString()
        AddHandler x.CloseMe, AddressOf UnloadControl
        CanvasRight.Children.Add(x)
        CanvasRight.SetLeft(x, 50)
        CanvasRight.SetTop(x, 50)
    End Sub

    'Menu Consulter le log
    Private Sub MnuViewLog(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuItem4.Click
        Dim x As New uLog
        x.Uid = System.Guid.NewGuid.ToString()
        AddHandler x.CloseMe, AddressOf UnloadControl
        CanvasRight.Children.Add(x)
        CanvasRight.SetLeft(x, 50)
        CanvasRight.SetTop(x, 50)
    End Sub
End Class
