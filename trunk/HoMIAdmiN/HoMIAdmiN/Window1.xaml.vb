Imports System.Windows.Threading
Imports System.Runtime.Serialization.Formatters.Soap
Imports HoMIDom.HoMIDom
Imports System.ServiceModel
Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Threading
Imports System.Reflection.Assembly
Imports System.Data

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
            Me.Title = "HoMIAdmiN v" & My.Application.Info.Version.ToString & " - HoMIDoM"

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
                    Dim tool As New ToolTip
                    tool.Content = "Serveur connecté adresse utilisée: " & myChannelFactory.Endpoint.Address.ToString()
                    LblConnect.Content = Mid("Serveur connecté adresse utilisée: " & myChannelFactory.Endpoint.Address.ToString(), 1, 64) & "..."
                    LblConnect.ToolTip = tool
                    tool = Nothing

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
            Dim retour As MessageBoxResult
            retour = MessageBox.Show("Voulez-vous enregistrer la configuration avant de quitter?", "HomIAdmin", MessageBoxButton.YesNo, MessageBoxImage.Question)

            If retour = MessageBoxResult.Yes Then
                Try
                    If IsConnect = False Then
                        MessageBox.Show("Impossible d'enregistrer la configuration car le serveur n'est pas connecté !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Asterisk)
                    Else
                        myService.SaveConfig()
                    End If
                Catch ex As Exception
                    MessageBox.Show("ERREUR Sub Quitter: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
                End Try
            End If

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
            CntZone.Content = myService.GetAllZones.Count & " Zone(s)"
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
            CntUser.Content = myService.GetAllUsers.Count & " User(s)"
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
            CntDriver.Content = myService.GetAllDrivers.Count & " Driver(s)"
            For i As Integer = 0 To myService.GetAllDrivers.Count - 1 'Obj.Drivers.Count - 1
                Dim newchild As New TreeViewItem
                Dim stack As New StackPanel
                stack.Orientation = Orientation.Horizontal
                stack.HorizontalAlignment = HorizontalAlignment.Left

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
                newchild.Header = stack
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
            CntDevice.Content = myService.GetAllDevices.Count & " Device(s)"
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
        Try
            TreeViewMacros.Items.Clear()
            If IsConnect = False Then Exit Sub
            CntMacro.Content = myService.GetAllMacros.Count & " Macro(s)"
            For i As Integer = 0 To myService.GetAllMacros.Count - 1
                Dim newchild As New TreeViewItem
                Dim stack As New StackPanel
                Dim img As New Image
                Dim uri As String = ""
                Dim bmpImage As New BitmapImage()

                stack.Orientation = Orientation.Horizontal

                img.Height = 20
                img.Width = 20

                Dim label As New Label
                label.Foreground = New SolidColorBrush(Colors.White)
                label.Content = myService.GetAllMacros.Item(i).Nom

                uri = MyRep & "\Images\Icones\script-128.png"
                bmpImage.BeginInit()
                bmpImage.UriSource = New Uri(uri, UriKind.Absolute)
                bmpImage.EndInit()
                img.Source = bmpImage

                stack.Children.Add(img)
                stack.Children.Add(label)

                newchild.Foreground = New SolidColorBrush(Colors.White)
                newchild.Header = stack
                newchild.Uid = myService.GetAllMacros.Item(i).ID
                TreeViewMacros.Items.Add(newchild)
            Next
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub AffScene: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Afficher la liste des triggers
    Public Sub AffTrigger()
        Try
            TreeViewTriggers.Items.Clear()
            If IsConnect = False Then Exit Sub
            CntTrigger.Content = myService.GetAllTriggers.Count & " Trigger(s)"
            For i As Integer = 0 To myService.GetAllTriggers.Count - 1
                Dim newchild As New TreeViewItem
                Dim stack As New StackPanel
                Dim img As New Image
                Dim uri As String = ""
                Dim bmpImage As New BitmapImage()

                stack.Orientation = Orientation.Horizontal

                img.Height = 20
                img.Width = 20

                Dim label As New Label
                label.Foreground = New SolidColorBrush(Colors.White)
                label.Content = myService.GetAllTriggers.Item(i).Nom

                uri = MyRep & "\Images\Icones\drapeau-vert-32.png"
                bmpImage.BeginInit()
                bmpImage.UriSource = New Uri(uri, UriKind.Absolute)
                bmpImage.EndInit()
                img.Source = bmpImage

                stack.Children.Add(img)
                stack.Children.Add(label)

                newchild.Foreground = New SolidColorBrush(Colors.White)
                newchild.Header = stack
                newchild.Uid = myService.GetAllTriggers.Item(i).ID
                TreeViewTriggers.Items.Add(newchild)
            Next
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub Afftrigger: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Afficher la liste des schedule
    Public Sub AffSchedule()
        'TreeViewSchedule.Nodes.Clear()
        'For i As Integer = 0 To Obj.Schedules.Count - 1
        '    TreeViewSchedule.Nodes.Add(Obj.Schedules.Item(i).ID, Obj.Schedules.Item(i).Name)
        'Next
    End Sub

    'Afficher la liste des historisations
    Public Sub AffHisto()
        Try
            ListHisto.Items.Clear()
            Dim x As New List(Of HoMIDom.HoMIDom.Historisation)
            x = myService.GetAllListHisto

            If x IsNot Nothing Then
                For i As Integer = 0 To x.Count - 1
                    Dim y As New CheckBox
                    Dim a As Historisation = x.Item(i)
                    Dim b As String = myService.ReturnDeviceByID(a.IdDevice).Name
                    If b = "" Then b = "?"
                    y.Content = a.Nom & " {" & b & "}"
                    y.Tag = a.Nom
                    y.Uid = a.IdDevice
                    y.Foreground = New SolidColorBrush(Colors.White)
                    ListHisto.Items.Add(y)
                Next
            End If
        Catch ex As Exception

        End Try
    End Sub
#End Region

#Region "Drivers"

    Private Sub TreeViewDriver_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles TreeViewDriver.MouseDoubleClick
        Try

            If TreeViewDriver.SelectedItem IsNot Nothing Then
                For i As Integer = 0 To myService.GetAllDrivers.Count - 1
                    If myService.GetAllDrivers.Item(i).ID = TreeViewDriver.SelectedItem.uid Then
                        Dim y As TemplateDriver = myService.GetAllDrivers.Item(i)

                        Dim x As New uDriver(TreeViewDriver.SelectedItem.uid)
                        x.Uid = System.Guid.NewGuid.ToString()
                        AddHandler x.CloseMe, AddressOf UnloadControl
                        CanvasRight.Children.Clear()
                        CanvasRight.Children.Add(x)
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
                AffDriver()
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
                If myService.ReturnDriverByID(TreeViewDriver.SelectedItem.uid).Enable = True Then
                    myService.StartDriver(TreeViewDriver.SelectedItem.uid)
                    AffDriver()
                Else
                    MessageBox.Show("Le driver ne peut être démarré car sa propriété Enable est à False!", "Avertissement", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                End If
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

                        Dim x As New uDevice(uDevice.EAction.Modifier, TreeViewDevice.SelectedItem.uid)
                        x.Uid = System.Guid.NewGuid.ToString()
                        AddHandler x.CloseMe, AddressOf UnloadControl
                        CanvasRight.Children.Clear()
                        CanvasRight.Children.Add(x)
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
            CanvasRight.Children.Clear()
            CanvasRight.Children.Add(x)
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
            CanvasRight.Children.Clear()
            CanvasRight.Children.Add(x)
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub BtnNewZone_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Modifier une zone
    Private Sub TreeViewZone_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles TreeViewZone.MouseDoubleClick
        Try
            If TreeViewZone.SelectedItem.uid Is Nothing Then Exit Sub

            If TreeViewZone.SelectedItem IsNot Nothing Then
                For i As Integer = 0 To myService.GetAllZones.Count - 1
                    If myService.GetAllZones.Item(i).ID = TreeViewZone.SelectedItem.uid Then

                        Dim x As New uZone(uDevice.EAction.Modifier, TreeViewZone.SelectedItem.uid)
                        x.Uid = System.Guid.NewGuid.ToString()
                        AddHandler x.CloseMe, AddressOf UnloadControl
                        CanvasRight.Children.Clear()
                        CanvasRight.Children.Add(x)
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
            CanvasRight.Children.Clear()
            CanvasRight.Children.Add(x)
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub BtnNewUser_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Bouton supprimer un user
    Private Sub BtnDelUser_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDelUser.Click
        Try
            If TreeViewUsers.SelectedItem IsNot Nothing And TreeViewUsers.SelectedItem.uid IsNot Nothing Then
                Dim cntAdmin As Integer = 0
                For i As Integer = 0 To Window1.myService.GetAllUsers.Count - 1
                    If Window1.myService.GetAllUsers.Item(i).Profil = Users.TypeProfil.admin Then
                        cntAdmin += 1
                    End If
                Next
                If cntAdmin <= 1 Then
                    MessageBox.Show("Impossible de supprimer cet utilisateur car il n'existe qu'un profil Administrateur !")
                    Exit Sub
                End If
                'If MessageBox.Show("Etes vous sur de supprimer ce user : " & TreeViewUsers.SelectedItem & " ?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.Yes Then
                If MessageBox.Show("Etes vous sur de supprimer ce user: ?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.Yes Then
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

                        Dim x As New uUser(uDevice.EAction.Modifier, TreeViewUsers.SelectedItem.uid)
                        x.Uid = System.Guid.NewGuid.ToString()
                        AddHandler x.CloseMe, AddressOf UnloadControl
                        CanvasRight.Children.Clear()
                        CanvasRight.Children.Add(x)
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
            CanvasRight.Children.Clear()
            CanvasRight.Children.Add(x)
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub BtnNewTriggerDevice_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnNewTriggerTime_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnNewTriggerTime.Click
        Try
            Dim x As New uTriggerTimer(0, "")
            x.Uid = System.Guid.NewGuid.ToString()
            AddHandler x.CloseMe, AddressOf UnloadControl
            CanvasRight.Children.Clear()
            CanvasRight.Children.Add(x)
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub BtnNewTriggerTime_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Modifier un trigger
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
                            CanvasRight.Children.Clear()
                            CanvasRight.Children.Add(x)
                        Else
                            Dim x As New uTriggerDevice(uTriggerDevice.EAction.Modifier, TreeViewTriggers.SelectedItem.uid)
                            x.Uid = System.Guid.NewGuid.ToString()
                            AddHandler x.CloseMe, AddressOf UnloadControl
                            CanvasRight.Children.Clear()
                            CanvasRight.Children.Add(x)
                        End If
                        Exit Sub
                    End If
                Next
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub TreeViewDevice_MouseDoubleClick: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnDelTrigger_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDelTrigger.Click
        Try
            If TreeViewTriggers.SelectedItem IsNot Nothing Then
                If TreeViewTriggers.SelectedItem.uid IsNot Nothing Then
                    If MessageBox.Show("Etes vous sur de supprimer ce trigger: " & myService.ReturnTriggerById(TreeViewTriggers.SelectedItem.uid).Nom & " ?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.Yes Then
                        Window1.myService.DeleteTrigger(TreeViewTriggers.SelectedItem.uid)
                        AffTrigger()
                    Else
                        MessageBox.Show("Veuillez sélectionner un trigger à supprimer!")
                    End If
                End If
            Else
                MessageBox.Show("Veuillez sélectionner un trigger à supprimer!")
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub BtnDelTrigger_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
#End Region

#Region "Macro"
    'Nouvelle macro
    Private Sub BtnNewMacro_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnNewMacro.Click
        Try
            If IsConnect = False Then
                MessageBox.Show("Impossible d'afficher la macro car le serveur n'est pas connecté !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Asterisk)
                Exit Sub
            End If

            Dim x As New uMacro(uMacro.EAction.Nouveau, "")
            x.Uid = System.Guid.NewGuid.ToString()
            AddHandler x.CloseMe, AddressOf UnloadControl
            CanvasRight.Children.Clear()
            CanvasRight.Children.Add(x)
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub BtnNewMacro_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Modifier macro
    Private Sub TreeViewMacros_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles TreeViewMacros.MouseDoubleClick
        Try
            Dim x As New uMacro(uMacro.EAction.Modifier, TreeViewMacros.SelectedItem.uid)
            x.Uid = System.Guid.NewGuid.ToString()
            AddHandler x.CloseMe, AddressOf UnloadControl
            CanvasRight.Children.Clear()
            CanvasRight.Children.Add(x)
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub TreeViewMacros_MouseDoubleClick: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'supprimer macro
    Private Sub BtnDelMacro_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDelMacro.Click
        Try
            If TreeViewMacros.SelectedItem IsNot Nothing Then
                If TreeViewMacros.SelectedItem.uid IsNot Nothing Then
                    If MessageBox.Show("Etes vous sur de supprimer cette macro: " & myService.ReturnMacroById(TreeViewMacros.SelectedItem.uid).Nom & " ?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.Yes Then
                        Window1.myService.DeleteMacro(TreeViewMacros.SelectedItem.uid)
                        AffScene()
                    End If
                Else
                    MessageBox.Show("Veuillez sélectionner une macro à supprimer!")
                End If
            Else
                MessageBox.Show("Veuillez sélectionner une macro à supprimer!")
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub BtnDelMacro_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
#End Region

    'Menu Save Config
    Private Sub MnuSaveConfig(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuSaveConfig.Click
        Try
            If IsConnect = False Then
                MessageBox.Show("Impossible d'enregistrer la config car le serveur n'est pas connecté !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Asterisk)
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

                    Select Case MyControl.Tag
                        Case "DRIVER"
                            AffDriver()
                        Case "DEVICE"
                            AffDevice()
                        Case "ZONE"
                            AffZone()
                        Case "USER"
                            AffUser()
                        Case "TRIGGER"
                            AffTrigger()
                        Case "MACRO"
                            AffScene()
                    End Select
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
            CanvasRight.Children.Clear()
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
            CanvasRight.Children.Clear()
            CanvasRight.Children.Add(x)
            CanvasRight.SetLeft(x, 50)
            CanvasRight.SetTop(x, 50)
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub MnuViewLog: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
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
            CanvasRight.Children.Clear()
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
    ''' <param name="Name"></param>
    ''' <param name="IP"></param>
    ''' <param name="Port"></param>
    ''' <remarks></remarks>
    Public Sub Connect_Srv(ByVal Name As String, ByVal IP As String, ByVal Port As String)
        Try
            myadress = "http://" & IP & ":" & Port & "/ServiceModelSamples/service"
            MyPort = Port

            Dim binding As New ServiceModel.BasicHttpBinding
            binding.MaxBufferPoolSize = 5000000
            binding.MaxReceivedMessageSize = 5000000
            binding.MaxBufferSize = 5000000
            binding.ReaderQuotas.MaxArrayLength = 5000000
            binding.ReaderQuotas.MaxStringContentLength = 5000000
            binding.SendTimeout = TimeSpan.FromMinutes(60)
            binding.CloseTimeout = TimeSpan.FromMinutes(60)
            binding.OpenTimeout = TimeSpan.FromMinutes(60)
            binding.ReceiveTimeout = TimeSpan.FromMinutes(60)

            myChannelFactory = New ServiceModel.ChannelFactory(Of HoMIDom.HoMIDom.IHoMIDom)(binding, New System.ServiceModel.EndpointAddress(myadress))
            myService = myChannelFactory.CreateChannel()
            IsConnect = True

            LblSrv.Content = "Serveur courant: " & Name
            For i As Integer = 0 To ListServer.Count - 1
                If ListServer.Item(i).Nom = Name Then
                    If File.Exists(ListServer.Item(i).Icon) Then
                        Dim bmpImage As New BitmapImage()
                        bmpImage.BeginInit()
                        bmpImage.UriSource = New Uri(ListServer.Item(i).Icon, UriKind.Absolute)
                        bmpImage.EndInit()
                        ImgSrv.Source = bmpImage
                    End If
                End If
            Next
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

            AffDriver()
            AffDevice()
            AffZone()
            AffUser()
            AffTrigger()
            AffScene()
            AffHisto()
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
                Connect_Srv(frm.TxtName.Text, frm.TxtIP.Text, frm.TxtPort.Text)
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
        'Dim x As New uHisto
        'x.Uid = System.Guid.NewGuid.ToString()
        'AddHandler x.CloseMe, AddressOf UnloadControl
        'CanvasRight.Children.Clear()
        'CanvasRight.Children.Add(x)
        'Try
        '    Image5.Source = ConvertArrayToImage(myService.GetByteFromImage("d:\cyber-SPA\PE - Perso\PERSO\HoMIDom\Source\DEBUG\Images\Graphes\test.png"))
        'Catch ex As Exception
        '    MessageBox.Show(ex.ToString)
        'End Try
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

    Private Sub MenuConfigAudio_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuConfigAudio.Click
        Try
            Dim x As New uConfigAudio
            x.Uid = System.Guid.NewGuid.ToString()
            AddHandler x.CloseMe, AddressOf UnloadControl
            CanvasRight.Children.Clear()
            CanvasRight.Children.Add(x)
            CanvasRight.SetLeft(x, 50)
            CanvasRight.SetTop(x, 8)
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub MenuConfigAudio: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub TreeViewMacros_SelectedItemChanged(ByVal sender As Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of Object)) Handles TreeViewMacros.SelectedItemChanged
        If Mouse.LeftButton = MouseButtonState.Pressed Then
            If TreeViewMacros.SelectedItem IsNot Nothing Then
                Dim effects As DragDropEffects
                Dim obj As New DataObject()
                obj.SetData(GetType(String), TreeViewMacros.SelectedItem.uid)
                effects = DragDrop.DoDragDrop(Me.TreeViewMacros, obj, DragDropEffects.Copy Or DragDropEffects.Move)
            End If
        End If
    End Sub

    Private Sub BtnGenereGraph_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnGenereGraph.Click
        Dim myPane As New ZedGraph.GraphPane
        Dim ListColor As New List(Of System.Drawing.Color)
        Dim idxcolor As Integer = -1


        ListColor.Add(System.Drawing.Color.Blue)
        ListColor.Add(System.Drawing.Color.Red)
        ListColor.Add(System.Drawing.Color.Green)
        ListColor.Add(System.Drawing.Color.Yellow)
        ListColor.Add(System.Drawing.Color.Orange)
        ListColor.Add(System.Drawing.Color.Violet)
        ListColor.Add(System.Drawing.Color.DarkBlue)
        ListColor.Add(System.Drawing.Color.DarkRed)
        ListColor.Add(System.Drawing.Color.DarkGreen)
        ListColor.Add(System.Drawing.Color.Turquoise)
        ListColor.Add(System.Drawing.Color.DarkOrange)
        ListColor.Add(System.Drawing.Color.DarkViolet)

        For i As Integer = 0 To ListHisto.Items.Count - 1
            Dim chk As CheckBox = ListHisto.Items(i)

            If chk.IsChecked = True Then
                Dim _listhisto As New List(Of Historisation)
                _listhisto = myService.GetHisto(chk.Tag, chk.Uid)

                Dim listpoint As New ZedGraph.PointPairList
                For j As Integer = 0 To _listhisto.Count - 1
                    If IsNumeric(_listhisto.Item(j).Value) = False Then
                        listpoint = Nothing
                        Exit For
                    End If
                    listpoint.Add(New ZedGraph.PointPair(_listhisto.Item(j).DateTime.ToOADate, _listhisto.Item(j).Value))
                Next

                Dim courbe As ZedGraph.LineItem
                idxcolor += 1
                If idxcolor = ListColor.Count - 1 Then idxcolor = 0

                courbe = myPane.AddCurve(chk.Content, listpoint, ListColor(idxcolor), ZedGraph.SymbolType.None)
                courbe.Line.Width = 1
                'courbe.Line.Fill = New ZedGraph.Fill(System.Drawing.Color.FromArgb(150, 250, 220, 220))

            End If


            'Mypane
            'myPane.Rect = New System.Drawing.RectangleF(0, 0, largeur, hauteur)
            'myPane.Title.FontSpec.FontColor = Color.DodgerBlue
            'myPane.Legend.IsVisible = True 'on affiche la légende ou non
            myPane.Chart.Fill = New ZedGraph.Fill(System.Drawing.Color.FromArgb(240, 245, 250), System.Drawing.Color.FromArgb(210, 230, 240), -90) 'fond dégradé

            'Axe X
            myPane.XAxis.Type = ZedGraph.AxisType.Date
            myPane.XAxis.Scale.Format = "dd-MM-yy HH:mm" '"dd-MMM-yy HH:mm:ss"
            myPane.XAxis.MajorGrid.Color = System.Drawing.Color.LightGray
            myPane.XAxis.MajorGrid.PenWidth = 1
            myPane.XAxis.MajorGrid.IsVisible = True

            'Axe Y
            myPane.YAxis.MajorGrid.Color = System.Drawing.Color.LightGray
            myPane.YAxis.MajorGrid.PenWidth = 1
            myPane.YAxis.MajorGrid.IsVisible = True

            Dim x As New uHisto(myPane)
            x.Uid = System.Guid.NewGuid.ToString()
            AddHandler x.CloseMe, AddressOf UnloadControl
            CanvasRight.Children.Clear()
            CanvasRight.Children.Add(x)
        Next
    End Sub
End Class
