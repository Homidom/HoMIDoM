Imports System.Windows.Threading
Imports System.Runtime.Serialization.Formatters.Soap
Imports HoMIDom.HoMIDom
Imports System.ServiceModel
Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Threading
Imports System.Reflection.Assembly

Class Window1

    Public Shared IsConnect As Boolean = False
    Public Shared CanvasUser As Canvas
    Public Shared myService As HoMIDom.HoMIDom.IHoMIDom
    Public Shared ListServer As New List(Of ClServer)
    Dim Myfile As String
    Dim MyPort As String = ""
    Dim myChannelFactory As ServiceModel.ChannelFactory(Of HoMIDom.HoMIDom.IHoMIDom) = Nothing
    Dim myadress As String = ""
    Dim FlagStart As Boolean = False
    Dim MemCanvas As Canvas
    Dim MyRep As String = System.Environment.CurrentDirectory

    Public Sub New()
        Try
            Dim spl As Window2 = New Window2
            spl.Show()
            Thread.Sleep(1000)
            ' Cet appel est requis par le Concepteur Windows Form.
            InitializeComponent()
            spl.Close()

            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            Dim dt As DispatcherTimer = New DispatcherTimer()
            AddHandler dt.Tick, AddressOf dispatcherTimer_Tick
            dt.Interval = New TimeSpan(0, 0, 1)
            dt.Start()

            Myfile = MyRep & "\HoMIAdmiN_config.xml"

        Catch ex As Exception
            MessageBox.Show("ERREUR Sub New: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try

    End Sub

    'Affiche la date et heure, heures levé et couché du soleil
    Public Sub dispatcherTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        Try
            If IsConnect = True Then
                Try
                    Dim mytime As String = myService.GetTime
                    LblStatus.Content = Now.ToLongDateString & " " & mytime & " "
                    LblConnect.Content = "Serveur connecté adresse utilisée: " & myChannelFactory.Endpoint.Address.ToString()
                    Dim mydate As Date
                    mydate = myService.GetHeureLeverSoleil
                    LHS.Content = mydate.ToShortTimeString
                    mydate = myService.GetHeureCoucherSoleil
                    LCS.Content = mydate.ToShortTimeString

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
        Catch ex As Exception
            MessageBox.Show("ERREUR dispatcherTimer_Tick: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Menu Quitter
    Private Sub Quitter(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuQuitter.Click
        Try
            Me.Close()
            End
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub Quitter: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

#Region "Affichage"
    'Afficher la liste des zones
    Public Sub AffZone()
        Try
            TreeViewZone.Items.Clear()
            If IsConnect = False Then Exit Sub
            For i As Integer = 0 To myService.GetAllZones.Count - 1
                Dim newchild As New TreeViewItem
                Dim stack As New StackPanel
                Dim img As New Image
                Dim uri As String = ""
                Dim bmpImage As New BitmapImage()

                stack.Orientation = Orientation.Horizontal

                img.Height = 20
                img.Width = 20

                If myService.GetAllZones.Item(i).Image <> "" And File.Exists(myService.GetAllDevices.Item(i).Picture) = True Then
                    uri = myService.GetAllZones.Item(i).Image
                Else
                    uri = MyRep & "\Images\icones\Defaut-128.png"
                End If
                bmpImage.BeginInit()
                bmpImage.UriSource = New Uri(uri, UriKind.Absolute)
                bmpImage.EndInit()
                img.Source = bmpImage

                Dim label As New Label
                label.Foreground = New SolidColorBrush(Colors.White)
                label.Content = myService.GetAllZones.Item(i).Name

                stack.Children.Add(img)
                stack.Children.Add(label)

                newchild.Foreground = New SolidColorBrush(Colors.White)
                newchild.Header = stack 'myService.GetAllZones.Item(i).Name
                newchild.Uid = myService.GetAllZones.Item(i).ID
                TreeViewZone.Items.Add(newchild)
            Next
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub AffZone: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Afficher la liste des users
    Public Sub AffUser()
        Try
            TreeViewUsers.Items.Clear()
            If IsConnect = False Then Exit Sub
            For i As Integer = 0 To myService.GetAllUsers.Count - 1
                Dim newchild As New TreeViewItem
                Dim stack As New StackPanel
                Dim img As New Image
                Dim uri As String = ""
                Dim bmpImage As New BitmapImage()

                stack.Orientation = Orientation.Horizontal

                img.Height = 20
                img.Width = 20

                If myService.GetAllUsers.Item(i).Image <> "" And File.Exists(myService.GetAllDevices.Item(i).Picture) = True Then
                    uri = myService.GetAllUsers.Item(i).Image
                Else
                    uri = MyRep & "\Images\icones\user.png"
                End If
                bmpImage.BeginInit()
                bmpImage.UriSource = New Uri(uri, UriKind.Absolute)
                bmpImage.EndInit()
                img.Source = bmpImage

                Dim label As New Label
                label.Foreground = New SolidColorBrush(Colors.White)
                label.Content = myService.GetAllUsers.Item(i).UserName

                stack.Children.Add(img)
                stack.Children.Add(label)

                newchild.Foreground = New SolidColorBrush(Colors.White)
                newchild.Header = stack 'myService.GetAllUsers.Item(i).UserName
                newchild.Uid = myService.GetAllUsers.Item(i).ID
                TreeViewUsers.Items.Add(newchild)
            Next
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub AffUser: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Afficher la liste des drivers
    Public Sub AffDriver()
        Try
            TreeViewDriver.Items.Clear()
            If IsConnect = False Then Exit Sub
            For i As Integer = 0 To myService.GetAllDrivers.Count - 1 'Obj.Drivers.Count - 1
                Dim newchild As New TreeViewItem
                Dim stack As New StackPanel
                stack.Orientation = Orientation.Horizontal

                Dim Elipse As New Ellipse
                Elipse.Width = 9
                Elipse.Height = 9
                Dim myBrush As New RadialGradientBrush()
                myBrush.GradientOrigin = New Point(0.75, 0.25)

                If myService.GetAllDrivers.Item(i).IsConnect = True Then
                    myBrush.GradientStops.Add(New GradientStop(Colors.LightGreen, 0.0))
                    myBrush.GradientStops.Add(New GradientStop(Colors.Green, 0.5))
                    myBrush.GradientStops.Add(New GradientStop(Colors.DarkGreen, 1.0))
                Else
                    myBrush.GradientStops.Add(New GradientStop(Colors.Yellow, 0.0))
                    myBrush.GradientStops.Add(New GradientStop(Colors.Red, 0.5))
                    myBrush.GradientStops.Add(New GradientStop(Colors.DarkRed, 1.0))
                End If

                Elipse.Fill = myBrush

                Dim label As New Label
                label.Foreground = New SolidColorBrush(Colors.White)
                label.Content = myService.GetAllDrivers.Item(i).Nom

                stack.Children.Add(Elipse)
                stack.Children.Add(label)

                newchild.Foreground = New SolidColorBrush(Colors.White)
                newchild.Header = stack 'myService.GetAllDrivers.Item(i).Nom
                newchild.Uid = myService.GetAllDrivers.Item(i).ID
                TreeViewDriver.Items.Add(newchild)

            Next
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub AffDriver: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Afficher la liste des devices
    Public Sub AffDevice()
        Try
            TreeViewDevice.Items.Clear()
            If IsConnect = False Then Exit Sub
            For i As Integer = 0 To myService.GetAllDevices.Count - 1 'Obj.Devices.Count - 1
                Dim newchild As New TreeViewItem
                Dim stack As New StackPanel
                Dim img As New Image
                Dim uri As String = ""
                Dim bmpImage As New BitmapImage()

                stack.Orientation = Orientation.Horizontal

                img.Height = 20
                img.Width = 20

                If myService.GetAllDevices.Item(i).Picture <> "" And File.Exists(myService.GetAllDevices.Item(i).Picture) = True Then
                    uri = myService.GetAllDevices.Item(i).Picture
                Else
                    uri = MyRep & "\Images\Devices\Defaut-128.png"
                End If
                bmpImage.BeginInit()
                bmpImage.UriSource = New Uri(uri, UriKind.Absolute)
                bmpImage.EndInit()
                img.Source = bmpImage

                Dim label As New Label
                label.Foreground = New SolidColorBrush(Colors.White)
                label.Content = myService.GetAllDevices.Item(i).Name

                stack.Children.Add(img)
                stack.Children.Add(label)

                newchild.Foreground = New SolidColorBrush(Colors.White)
                newchild.Header = stack
                newchild.Uid = myService.GetAllDevices.Item(i).ID
                TreeViewDevice.Items.Add(newchild)
            Next
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub AffDevice: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
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
        Try
            TreeViewTriggers.Items.Clear()
            If IsConnect = False Then Exit Sub
            For i As Integer = 0 To myService.GetAllTriggers.Count - 1
                Dim newchild As New TreeViewItem
                Dim stack As New StackPanel
                Dim img As New Image
                Dim uri As String = ""
                Dim bmpImage As New BitmapImage()

                stack.Orientation = Orientation.Horizontal

                img.Height = 20
                img.Width = 20

                'If myService.GetAllTriggers.Item(i).Picture <> "" And File.Exists(myService.GetAllDevices.Item(i).Picture) = True Then
                '    uri = myService.GetAllDevices.Item(i).Picture
                'Else
                '    uri = MyRep & "\Images\Devices\Defaut-128.png"
                'End If
                'bmpImage.BeginInit()
                'bmpImage.UriSource = New Uri(uri, UriKind.Absolute)
                'bmpImage.EndInit()
                'img.Source = bmpImage

                Dim label As New Label
                label.Foreground = New SolidColorBrush(Colors.White)
                label.Content = myService.GetAllTriggers.Item(i).Nom

                stack.Children.Add(img)
                stack.Children.Add(label)

                newchild.Foreground = New SolidColorBrush(Colors.White)
                newchild.Header = stack
                newchild.Uid = myService.GetAllTriggers.Item(i).ID
                TreeViewTriggers.Items.Add(newchild)
            Next
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub Afftrigger: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
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
        Try

            If TreeViewDriver.SelectedItem IsNot Nothing Then
                For i As Integer = 0 To myService.GetAllDrivers.Count - 1
                    If myService.GetAllDrivers.Item(i).ID = TreeViewDriver.SelectedItem.uid Then
                        Dim y As TemplateDriver = myService.GetAllDrivers.Item(i)
                        'PropertyGrid1.SelectedObject = y

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

        Catch ex As Exception
            MessageBox.Show("ERREUR Sub TreeViewDriver_MouseDoubleClick: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
    Private Sub TreeViewDriver_SelectedItemChanged(ByVal sender As Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of Object)) Handles TreeViewDriver.SelectedItemChanged
        Try
            If IsConnect = False Then
                MessageBox.Show("Impossible d'afficher le driver car le serveur n'est pas connecté !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Asterisk)
                Exit Sub
            End If

            If e.NewValue Is Nothing Then Exit Sub

            For i As Integer = 0 To myService.GetAllDrivers.Count - 1
                If myService.GetAllDrivers.Item(i).ID = e.NewValue.uid Then
                    Dim x As TemplateDriver = myService.GetAllDrivers.Item(i)
                    'PropertyGrid1.SelectedObject = x
                    Exit Sub
                End If
            Next
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub TreeViewDriver_SelectedItemChanged: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnStop_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnStop.Click
        Try
            If TreeViewDriver.SelectedItem IsNot Nothing Then
                myService.StopDriver(TreeViewDriver.SelectedItem.uid)
            Else
                MessageBox.Show("Veuillez sélectionner un Driver!")
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub BtnStop_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnStart_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnStart.Click
        Try
            If TreeViewDriver.SelectedItem IsNot Nothing Then
                myService.StartDriver(TreeViewDriver.SelectedItem.uid)
            Else
                MessageBox.Show("Veuillez sélectionner un Driver!")
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub BtnStart_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
#End Region

#Region "Devices"
    'Modifier un device
    Private Sub TreeViewDevice_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles TreeViewDevice.MouseDoubleClick
        Try
            If IsConnect = False Then
                MessageBox.Show("Impossible d'afficher le device car le serveur n'est pas connecté !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Asterisk)
                Exit Sub
            End If

            If TreeViewDevice.SelectedItem.uid Is Nothing Then Exit Sub

            If TreeViewDevice.SelectedItem IsNot Nothing Then

                For i As Integer = 0 To myService.GetAllDevices.Count - 1
                    If myService.GetAllDevices.Item(i).ID = TreeViewDevice.SelectedItem.uid Then
                        Dim y As TemplateDevice = myService.GetAllDevices.Item(i)
                        'PropertyGrid1.SelectedObject = y

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
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub TreeViewDevice_MouseDoubleClick: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Bouton nouveau Device
    Private Sub BtnNewDevice_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnNewDevice.Click
        Try
            Dim x As New uDevice(uDevice.EAction.Nouveau, "")
            x.Uid = System.Guid.NewGuid.ToString()
            AddHandler x.CloseMe, AddressOf UnloadControl
            CanvasRight.Children.Add(x)
            CanvasRight.SetLeft(x, 50)
            CanvasRight.SetTop(x, 5)
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub BtnNewDevice_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Bouton supprimer device
    Private Sub BtnDelDevice_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDelDevice.Click
        Try
            If TreeViewDevice.SelectedItem IsNot Nothing Then
                Window1.myService.DeleteDevice(TreeViewDevice.SelectedItem.uid)
                AffDevice()
            Else
                MessageBox.Show("Veuillez sélectionner un Device à supprimer!")
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub BtnDelDevice_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
#End Region

#Region "Zones"
    'Bouton nouvelle zone
    Private Sub BtnNewZone_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnNewZone.Click
        Try
            Dim x As New uZone(uDevice.EAction.Nouveau, "")
            x.Uid = System.Guid.NewGuid.ToString()
            AddHandler x.CloseMe, AddressOf UnloadControl
            CanvasRight.Children.Add(x)
            CanvasRight.SetLeft(x, 50)
            CanvasRight.SetTop(x, 5)
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub BtnNewZone_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Modiifer une zone
    Private Sub TreeViewZone_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles TreeViewZone.MouseDoubleClick
        Try
            If TreeViewZone.SelectedItem.uid Is Nothing Then Exit Sub

            If TreeViewZone.SelectedItem IsNot Nothing Then
                For i As Integer = 0 To myService.GetAllZones.Count - 1
                    If myService.GetAllZones.Item(i).ID = TreeViewZone.SelectedItem.uid Then
                        'PropertyGrid1.SelectedObject = myService.GetAllZones.Item(i)

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
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub TreeViewZone_MouseDoubleClick: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'bouton supprimer une zone
    Private Sub BtnDelZone_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDelZone.Click
        Try
            If TreeViewZone.SelectedItem IsNot Nothing Then
                Window1.myService.DeleteZone(TreeViewZone.SelectedItem.uid)
                AffZone()
            Else
                MessageBox.Show("Veuillez sélectionner une Zone à supprimer!")
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub BtnDelZone_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
#End Region

#Region "User"
    'Bouton Nouveau user
    Private Sub BtnNewUser_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnNewUser.Click
        Try
            Dim x As New uUser(uDevice.EAction.Nouveau, "")
            x.Uid = System.Guid.NewGuid.ToString()
            AddHandler x.CloseMe, AddressOf UnloadControl
            CanvasRight.Children.Add(x)
            CanvasRight.SetLeft(x, 50)
            CanvasRight.SetTop(x, 5)
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub BtnNewUser_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Bouton supprimer un user
    Private Sub BtnDelUser_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDelUser.Click
        Try
            If TreeViewUsers.SelectedItem IsNot Nothing And TreeViewUsers.SelectedItem.uid IsNot Nothing Then
                If MessageBox.Show("Etes vous sur de supprimer ce user: " & TreeViewUsers.SelectedItem.header & " ?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.Yes Then
                    Window1.myService.DeleteUser(TreeViewUsers.SelectedItem.uid)
                    AffUser()
                End If
            Else
                MessageBox.Show("Veuillez sélectionner un utilisateur à supprimer!")
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub BtnDelUser_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Sélection d'un user
    Private Sub TreeViewUsers_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles TreeViewUsers.MouseDoubleClick
        Try
            If TreeViewUsers.SelectedItem IsNot Nothing Then
                For i As Integer = 0 To myService.GetAllUsers.Count - 1
                    If myService.GetAllUsers.Item(i).ID = TreeViewUsers.SelectedItem.uid Then
                        'PropertyGrid1.SelectedObject = myService.GetAllUsers.Item(i)

                        Dim x As New uUser(uDevice.EAction.Modifier, TreeViewUsers.SelectedItem.uid)
                        x.Uid = System.Guid.NewGuid.ToString()
                        AddHandler x.CloseMe, AddressOf UnloadControl
                        CanvasRight.Children.Add(x)
                        CanvasRight.SetLeft(x, 50)
                        CanvasRight.SetTop(x, 5)

                        Exit Sub
                    End If
                Next
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub TreeViewUsers_MouseDoubleClick: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
#End Region

#Region "Trigger"
    Private Sub BtnNewTriggerDevice_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnNewTriggerDevice.Click
        Try
            Dim x As New uTriggerDevice(0, "")
            x.Uid = System.Guid.NewGuid.ToString()
            AddHandler x.CloseMe, AddressOf UnloadControl
            CanvasRight.Children.Add(x)
            CanvasRight.SetLeft(x, 50)
            CanvasRight.SetTop(x, 8)
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub BtnNewTriggerDevice_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnNewTriggerTime_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnNewTriggerTime.Click
        Try
            Dim x As New uTriggerTimer(0, "")
            x.Uid = System.Guid.NewGuid.ToString()
            AddHandler x.CloseMe, AddressOf UnloadControl
            CanvasRight.Children.Add(x)
            CanvasRight.SetLeft(x, 50)
            CanvasRight.SetTop(x, 8)
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub BtnNewTriggerTime_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Modifier un device
    Private Sub TreeViewTriggers_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles TreeViewTriggers.MouseDoubleClick
        Try
            If IsConnect = False Then
                MessageBox.Show("Impossible d'afficher le trigger car le serveur n'est pas connecté !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Asterisk)
                Exit Sub
            End If

            If TreeViewTriggers.SelectedItem.uid Is Nothing Then Exit Sub

            If TreeViewTriggers.SelectedItem IsNot Nothing Then

                For i As Integer = 0 To myService.GetAllTriggers.Count - 1
                    If myService.GetAllTriggers.Item(i).ID = TreeViewTriggers.SelectedItem.uid Then
                        Dim y As Trigger = myService.GetAllTriggers.Item(i)

                        If myService.GetAllTriggers.Item(i).Type = Trigger.TypeTrigger.TIMER Then
                            Dim x As New uTriggerTimer(uTriggerTimer.EAction.Modifier, TreeViewTriggers.SelectedItem.uid)
                            x.Uid = System.Guid.NewGuid.ToString()
                            AddHandler x.CloseMe, AddressOf UnloadControl
                            CanvasRight.Children.Add(x)
                            CanvasRight.SetLeft(x, 50)
                            CanvasRight.SetTop(x, 5)
                        Else
                            Dim x As New uTriggerDevice(uTriggerDevice.EAction.Modifier, TreeViewTriggers.SelectedItem.uid)
                            x.Uid = System.Guid.NewGuid.ToString()
                            AddHandler x.CloseMe, AddressOf UnloadControl
                            CanvasRight.Children.Add(x)
                            CanvasRight.SetLeft(x, 50)
                            CanvasRight.SetTop(x, 5)
                        End If
                        Exit Sub
                    End If
                Next
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub TreeViewDevice_MouseDoubleClick: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
#End Region

    'Menu Save Config
    Private Sub MnuSaveConfig(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuSaveConfig.Click
        Try
            If IsConnect = False Then
                MessageBox.Show("Impossible d'afficher le log car le serveur n'est pas connecté !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Asterisk)
                Exit Sub
            End If

            myService.SaveConfig()
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub MnuSaveConfig: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Décharger une fenêtre suivant son Id
    Public Sub UnloadControl(ByVal MyControl As Object)
        Try
            For i As Integer = 0 To CanvasRight.Children.Count - 1
                If CanvasRight.Children.Item(i).Uid = MyControl.uid Then
                    CanvasRight.Children.RemoveAt(i)
                    AffDriver()
                    AffDevice()
                    AffZone()
                    AffUser()
                    AffTrigger()
                    Exit Sub
                End If
            Next
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub UnloadControl: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Menu paramétrer le serveur
    Private Sub MnuConfigSrv(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuConfigSrv.Click
        Try
            Dim x As New uConfigServer
            x.Uid = System.Guid.NewGuid.ToString()
            AddHandler x.CloseMe, AddressOf UnloadControl
            CanvasRight.Children.Add(x)
            CanvasRight.SetLeft(x, 50)
            CanvasRight.SetTop(x, 8)
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub MnuConfigSrv: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Menu Consulter le log
    Private Sub MnuViewLog(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuConsultLog.Click
        Try
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
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub MnuViewLog: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Permet de se connecter à un serveur dans le sous menu connexion
    Private Sub MenuConnexion(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Try
            Call Connect_Srv(sender.uid)
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub MenuConnexion: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Menu à propose
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub MnuPropos(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuPropos.Click
        Try
            Process.Start("http://www.homidom.com/#")
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub MnuPropos: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Afficher la playlist
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub MnuPlayList(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuPlayList.Click
        Try
            Dim x As New uPlaylist
            x.Uid = System.Guid.NewGuid.ToString()
            AddHandler x.CloseMe, AddressOf UnloadControl
            CanvasRight.Children.Add(x)
            CanvasRight.SetLeft(x, 10)
            CanvasRight.SetTop(x, 5)
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub MnuPlayList: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        Try
            If ListServer.Count > 0 Then
                'Serialize object to a text file.
                Dim objStreamWriter As New StreamWriter(Myfile)
                Dim x As New XmlSerializer(ListServer.GetType)
                x.Serialize(objStreamWriter, ListServer)
                objStreamWriter.Close()
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'enregistrement du fichier de config xml de l'application Admin : " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
        MyBase.Finalize()
    End Sub

    ''' <summary>
    ''' Sélection d'un serveur pour connexion dans le sous menu connexion ou au démarrage
    ''' </summary>
    ''' <param name="Index"></param>
    ''' <remarks></remarks>
    Public Sub Connect_Srv(ByVal Index As Integer, Optional ByVal IP As String = "", Optional ByVal Port As String = "")
        Try
            If Index = 0 Then
                myadress = "http://" & IP & ":" & Port & "/ServiceModelSamples/service"
                MyPort = Port
            Else
                myadress = "http://" & ListServer.Item(Index - 1).Adresse & ":" & ListServer.Item(Index - 1).Port & "/ServiceModelSamples/service"
                MyPort = ListServer.Item(Index - 1).Port
            End If

            Dim binding As New ServiceModel.BasicHttpBinding
            binding.MaxBufferPoolSize = 5000000
            binding.MaxReceivedMessageSize = 5000000
            binding.MaxBufferSize = 5000000
            binding.ReaderQuotas.MaxArrayLength = 5000000
            binding.ReaderQuotas.MaxStringContentLength = 5000000

            myChannelFactory = New ServiceModel.ChannelFactory(Of HoMIDom.HoMIDom.IHoMIDom)(binding, New System.ServiceModel.EndpointAddress(myadress))
            myService = myChannelFactory.CreateChannel()
            IsConnect = True

            If Index = 0 Then
                Dim x As New ClServer
                x.Adresse = IP
                x.Port = Port
                x.Nom = "Serveur" & ListServer.Count
                If ListServer.Count = 0 Then
                    x.Defaut = True
                End If
                ListServer.Add(x)
            End If

            CanvasUser = CanvasRight
        Catch ex As Exception
            myChannelFactory.Abort()
            IsConnect = False
            MessageBox.Show("Erreur lors de la connexion au serveur sélectionné: " & ex.Message, "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub Window1_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        Try
            'Connexion au serveur web
            If File.Exists(Myfile) Then
                Try
                    'Deserialize text file to a new object.
                    Dim objStreamReader As New StreamReader(Myfile)
                    Dim x As New XmlSerializer(ListServer.GetType)
                    ListServer = x.Deserialize(objStreamReader)
                    objStreamReader.Close()
                Catch ex As Exception
                    MessageBox.Show("Erreur lors de l'ouverture du fichier de config xml (" & Myfile & "), vérifiez que toutes les balises requisent soient présentes: " & ex.Message, "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
                End Try

                If ListServer.Count = 0 Then 'Aucun serveur trouvé dans le fichier de config
                    MessageBox.Show("Aucun serveur n'a été trouvé dans le fichier de config (" & Myfile & "), veuillez saisir manuellement les paramètres du serveur", "Info Admin", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                End If
                'on ajoute la liste des serveurs dans le menu Connexion
            Else
                MessageBox.Show("Le fichier de config xml de l'admin (" & Myfile & ") est absent, veuillez utiliser la connexion manuelle", "Admin", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            End If

            'Page de connexion
            Do While FlagStart = False
                PageConnexion()
            Loop

            'If My.Settings.ViewProperty = False Then
            '    MemCanvas = Canvas2
            '    StackPanel3.Children.RemoveAt(1)
            '    Canvas1.MaxHeight = StackPanel3.Height
            '    TabControl1.MaxHeight = 500
            'End If

            AffDriver()
            AffDevice()
            AffZone()
            AffUser()
            AffTrigger()
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub Window1_Loaded: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Affiche la fenêtre de connexion
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PageConnexion()
        Try
            Dim frm As New Window3
            frm.Owner = Me
            frm.ShowDialog()
            If frm.DialogResult.HasValue And frm.DialogResult.Value Then
                If frm.CbServer.SelectedIndex > 0 Then
                    Connect_Srv(frm.CbServer.SelectedIndex)
                Else
                    Connect_Srv(frm.CbServer.SelectedIndex, frm.TxtIP.Text, frm.TxtPort.Text)
                End If
                If myService.VerifLogin(frm.TxtUsername.Text, frm.TxtPassword.Password) = False Then
                    MessageBox.Show("Le username ou le password sont erroné, impossible veuillez réessayer", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Else
                    frm.Close()
                    FlagStart = True
                End If
            Else
                End
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub PageConnexion: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Private Sub MenuPropriete_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuPropriete.Click
    '    Try
    '        If StackPanel3.Children.Count = 2 Then Exit Sub
    '        StackPanel3.Children.Add(MemCanvas)
    '        Canvas1.MaxHeight = StackPanel3.Height - 200
    '        TabControl1.MaxHeight = 300
    '        My.Settings.ViewProperty = True
    '        My.Settings.Save()
    '    Catch ex As Exception
    '        MessageBox.Show("ERREUR Sub MenuPropriete_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
    '    End Try
    'End Sub

    'Private Sub BtnCloseProperties_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnCloseProperties.Click
    '    Try
    '        MemCanvas = Canvas2
    '        StackPanel3.Children.RemoveAt(1)
    '        Canvas1.MaxHeight = StackPanel3.Height
    '        TabControl1.MaxHeight = 500
    '        My.Settings.ViewProperty = False
    '        My.Settings.Save()
    '    Catch ex As Exception
    '        MessageBox.Show("ERREUR Sub BtnCloseProperties_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
    '    End Try
    'End Sub

    Private Sub MenuTest_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuTest.Click
        Try
            Image5.Source = ConvertArrayToImage(myService.GetByteFromImage("d:\cyber-SPA\PE - Perso\PERSO\HoMIDom\Source\DEBUG\Images\Graphes\test.png"))
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TreeViewDevice_SelectedItemChanged(ByVal sender As Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of Object)) Handles TreeViewDevice.SelectedItemChanged
        If Mouse.LeftButton = MouseButtonState.Pressed Then
            If TreeViewDevice.SelectedItem IsNot Nothing Then
                Dim effects As DragDropEffects
                Dim obj As New DataObject()
                obj.SetData(GetType(String), TreeViewDevice.SelectedItem.uid)
                effects = DragDrop.DoDragDrop(Me.TreeViewDevice, obj, DragDropEffects.Copy Or DragDropEffects.Move)
            End If
        End If
    End Sub

    Private Sub TreeViewZone_SelectedItemChanged(ByVal sender As Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of Object)) Handles TreeViewZone.SelectedItemChanged
        If Mouse.LeftButton = MouseButtonState.Pressed Then
            If TreeViewZone.SelectedItem IsNot Nothing Then
                Dim effects As DragDropEffects
                Dim obj As New DataObject()
                obj.SetData(GetType(String), TreeViewZone.SelectedItem.uid)
                effects = DragDrop.DoDragDrop(Me.TreeViewZone, obj, DragDropEffects.Copy Or DragDropEffects.Move)
            End If
        End If
    End Sub

    Public Function ConvertArrayToImage(ByVal value As Object) As Object
        Dim ImgSource As BitmapImage = Nothing
        Dim array As Byte() = TryCast(value, Byte())

        If array IsNot Nothing Then
            ImgSource = New BitmapImage()
            ImgSource.BeginInit()
            ImgSource.StreamSource = New MemoryStream(array)
            ImgSource.EndInit()
        End If
        Return ImgSource
    End Function

    Private Sub MenuConfigAudio_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuConfigAudio.Click
        Try
            Dim x As New uConfigAudio
            x.Uid = System.Guid.NewGuid.ToString()
            AddHandler x.CloseMe, AddressOf UnloadControl
            CanvasRight.Children.Add(x)
            CanvasRight.SetLeft(x, 50)
            CanvasRight.SetTop(x, 8)
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub MenuConfigAudio: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub



End Class
