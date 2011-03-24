Imports System.Windows.Threading
Imports System.Runtime.Serialization.Formatters.Soap
Imports HoMIDom.HoMIDom
Imports System.ServiceModel

Class Window1

    Public Shared IsConnect As Boolean = False
    Public Shared CanvasUser As Canvas
    Public Shared myService As HoMIDom.HoMIDom.IHoMIDom

    Public Sub New()

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        Dim dt As DispatcherTimer = New DispatcherTimer()
        AddHandler dt.Tick, AddressOf dispatcherTimer_Tick
        dt.Interval = New TimeSpan(0, 0, 1)
        dt.Start()

        'Connexion au serveur web
        Dim myChannelFactory As ServiceModel.ChannelFactory(Of HoMIDom.HoMIDom.IHoMIDom) = Nothing

        Try
            myChannelFactory = New ServiceModel.ChannelFactory(Of HoMIDom.HoMIDom.IHoMIDom)("ConfigurationHttpHomidom")
            myService = myChannelFactory.CreateChannel()
            IsConnect = True
        Catch ex As Exception
            myChannelFactory.Abort()
            IsConnect = False
        End Try

        'Affichage des éléments 
        Try
            AffDriver()
            AffDevice()
            AffZone()

            CanvasUser = CanvasRight
        Catch ex As Exception
            IsConnect = False
            MessageBox.Show("Erreur: " & ex.ToString)
        End Try
    End Sub

    'Affiche la date et heure, heures levé et couché du soleil
    Public Sub dispatcherTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        If IsConnect = True Then
            Try
                Dim mytime As String = myService.GetTime
                LblStatus.Content = Now.ToLongDateString & " " & mytime & " "
                LblConnect.Content = "Serveur connecté"

                Dim myBrush As New RadialGradientBrush()
                myBrush.GradientOrigin = New Point(0.75, 0.25)
                myBrush.GradientStops.Add(New GradientStop(Colors.LightGreen, 0.0))
                myBrush.GradientStops.Add(New GradientStop(Colors.Green, 0.5))
                myBrush.GradientStops.Add(New GradientStop(Colors.DarkGreen, 1.0))
                Ellipse1.Fill = myBrush
            Catch ex As Exception
                IsConnect = False
                LblStatus.Content = Now.ToLongDateString & " " & Now.ToLongTimeString & " "
                LblConnect.Content = "Serveur non connecté"

                Dim myBrush As New RadialGradientBrush()
                myBrush.GradientOrigin = New Point(0.75, 0.25)
                myBrush.GradientStops.Add(New GradientStop(Colors.Yellow, 0.0))
                myBrush.GradientStops.Add(New GradientStop(Colors.Red, 0.5))
                myBrush.GradientStops.Add(New GradientStop(Colors.DarkRed, 1.0))

                Ellipse1.Fill = myBrush
            End Try
        Else
            LblStatus.Content = Now.ToLongDateString & " " & Now.ToLongTimeString & "      "
            LblConnect.Content = "Serveur non connecté"

            Dim myBrush As New RadialGradientBrush()
            myBrush.GradientOrigin = New Point(0.75, 0.25)
            myBrush.GradientStops.Add(New GradientStop(Colors.Yellow, 0.0))
            myBrush.GradientStops.Add(New GradientStop(Colors.Red, 0.5))
            myBrush.GradientStops.Add(New GradientStop(Colors.DarkRed, 1.0))

            Ellipse1.Fill = myBrush
        End If
    End Sub

    'Menu Quitter
    Private Sub Quitter(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuQuitter.Click
        End
    End Sub

#Region "Affichage"
    'Afficher la liste des zones
    Public Sub AffZone()
        TreeViewZone.Items.Clear()
        For i As Integer = 0 To myService.GetAllZones.Count - 1
            Dim newchild As New TreeViewItem
            newchild.Foreground = New SolidColorBrush(Colors.White)
            newchild.Header = myService.GetAllZones.Item(i).Name
            newchild.Uid = myService.GetAllZones.Item(i).ID
            TreeViewZone.Items.Add(newchild)
        Next
    End Sub

    'Afficher la liste des drivers
    Public Sub AffDriver()
        TreeViewDriver.Items.Clear()
        For i As Integer = 0 To myService.GetAllDrivers.Count - 1 'Obj.Drivers.Count - 1
            Dim newchild As New TreeViewItem
            newchild.Foreground = New SolidColorBrush(Colors.White)
            newchild.Header = myService.GetAllDrivers.Item(i).Nom 'Obj.Drivers.Item(i).Nom
            newchild.Uid = myService.GetAllDrivers.Item(i).ID 'Obj.Drivers.Item(i).id
            TreeViewDriver.Items.Add(newchild)
        Next
    End Sub

    'Afficher la liste des devices
    Public Sub AffDevice()
        TreeViewDevice.Items.Clear()
        For i As Integer = 0 To myService.GetAllDevices.Count - 1 'Obj.Devices.Count - 1
            Dim newchild As New TreeViewItem
            newchild.Foreground = New SolidColorBrush(Colors.White)
            newchild.Header = myService.GetAllDevices.Item(i).name 'Obj.Devices.Item(i).Name
            newchild.Uid = myService.GetAllDevices.Item(i).id 'Obj.Devices.Item(i).id
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

    Private Sub TreeViewDriver_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles TreeViewDriver.MouseDoubleClick
        If TreeViewDriver.SelectedItem IsNot Nothing Then
            For i As Integer = 0 To myService.GetAllDrivers.Count - 1
                If myService.GetAllDrivers.Item(i).ID = TreeViewDriver.SelectedItem.uid Then
                    Dim y As TemplateDriver = myService.GetAllDrivers.Item(i)
                    PropertyGrid1.SelectedObject = y

                    Dim x As New uDriver(TreeViewDriver.SelectedItem.uid)
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
    Private Sub TreeViewDriver_SelectedItemChanged(ByVal sender As Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of Object)) Handles TreeViewDriver.SelectedItemChanged
        For i As Integer = 0 To myService.GetAllDrivers.Count - 1
            If myService.GetAllDrivers.Item(i).ID = e.NewValue.uid Then
                Dim x As TemplateDriver = myService.GetAllDrivers.Item(i)
                PropertyGrid1.SelectedObject = x
                Exit Sub
            End If
        Next
    End Sub

    Private Sub BtnStop_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnStop.Click
        If TreeViewDriver.SelectedItem IsNot Nothing Then
            myService.StopDriver(TreeViewDriver.SelectedItem.uid)
        Else
            MessageBox.Show("Veuillez sélectionner un Driver!")
        End If
    End Sub

    Private Sub BtnStart_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnStart.Click
        If TreeViewDriver.SelectedItem IsNot Nothing Then
            myService.StartDriver(TreeViewDriver.SelectedItem.uid)
        Else
            MessageBox.Show("Veuillez sélectionner un Driver!")
        End If
    End Sub
#End Region

#Region "Devices"
    'Modifier un device
    Private Sub TreeViewDevice_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles TreeViewDevice.MouseDoubleClick
        If TreeViewDevice.SelectedItem IsNot Nothing Then
            For i As Integer = 0 To myService.GetAllDevices.Count - 1
                If myService.GetAllDevices.Item(i).ID = TreeViewDevice.SelectedItem.uid Then
                    Dim y As TemplateDevice = myService.GetAllDevices.Item(i)
                    PropertyGrid1.SelectedObject = y

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
            Window1.myService.DeleteDevice(TreeViewDevice.SelectedItem.uid)
            AffDevice()
        Else
            MessageBox.Show("Veuillez sélectionner un Device à supprimer!")
        End If
    End Sub
#End Region

#Region "Zones"
    'Bouton nouvelle zone
    Private Sub BtnNewZone_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnNewZone.Click
        Dim x As New uZone(uDevice.EAction.Nouveau, "")
        x.Uid = System.Guid.NewGuid.ToString()
        AddHandler x.CloseMe, AddressOf UnloadControl
        CanvasRight.Children.Add(x)
        CanvasRight.SetLeft(x, 50)
        CanvasRight.SetTop(x, 5)
    End Sub

    'Modiifer une zone
    Private Sub TreeViewZone_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles TreeViewZone.MouseDoubleClick
        If TreeViewZone.SelectedItem IsNot Nothing Then
            For i As Integer = 0 To myService.GetAllZones.Count - 1
                If myService.GetAllZones.Item(i).ID = TreeViewZone.SelectedItem.uid Then
                    PropertyGrid1.SelectedObject = myService.GetAllZones.Item(i)

                    Dim x As New uZone(uDevice.EAction.Modifier, TreeViewZone.SelectedItem.uid)
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

    'bouton supprimer une zone
    Private Sub BtnDelZone_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDelZone.Click
        If TreeViewZone.SelectedItem IsNot Nothing Then
            Window1.myService.DeleteZone(TreeViewZone.SelectedItem.uid)
            AffZone()
        Else
            MessageBox.Show("Veuillez sélectionner une Zone à supprimer!")
        End If
    End Sub
#End Region

    'Menu Save Config
    Private Sub MnuSaveConfig(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuConfigSrv.Click
        If IsConnect = False Then
            MessageBox.Show("Impossible d'afficher le log car le serveur n'est pas connecté !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Asterisk)
            Exit Sub
        End If

        myService.SaveConfig()
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
    Private Sub MnuConfigSrv(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuConfigSrv.Click
        If IsConnect = False Then
            MessageBox.Show("Impossible d'afficher le log car le serveur n'est pas connecté !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Asterisk)
            Exit Sub
        End If

        Dim x As New uConfigServer
        x.Uid = System.Guid.NewGuid.ToString()
        AddHandler x.CloseMe, AddressOf UnloadControl
        CanvasRight.Children.Add(x)
        CanvasRight.SetLeft(x, 50)
        CanvasRight.SetTop(x, 50)
    End Sub

    'Menu Consulter le log
    Private Sub MnuViewLog(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuConsultLog.Click
        If IsConnect = False Then
            MessageBox.Show("Impossible d'afficher le log car le serveur n'est pas connecté !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Asterisk)
            Exit Sub
        End If

        Dim x As New uLog
        x.Uid = System.Guid.NewGuid.ToString()
        AddHandler x.CloseMe, AddressOf UnloadControl
        CanvasRight.Children.Add(x)
        CanvasRight.SetLeft(x, 50)
        CanvasRight.SetTop(x, 50)
    End Sub

    ''' <summary>
    ''' Menu à propose
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub MnuPropos(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuPropos.Click
        Process.Start("http://www.homidom.com/#")
    End Sub

    ''' <summary>
    ''' Afficher la playlist
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub MnuPlayList(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuPlayList.Click
        Dim x As New uPlaylist
        x.Uid = System.Guid.NewGuid.ToString()
        AddHandler x.CloseMe, AddressOf UnloadControl
        CanvasRight.Children.Add(x)
        CanvasRight.SetLeft(x, 10)
        CanvasRight.SetTop(x, 5)
    End Sub
End Class
