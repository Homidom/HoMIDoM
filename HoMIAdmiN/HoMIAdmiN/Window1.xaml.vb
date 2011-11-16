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
    Dim _CurrentMnu As Integer

    Public Sub New()
        Try
            Me.Title = "HoMIAdmiN v" & My.Application.Info.Version.ToString & " - HoMIDoM"

            Dim spl As Window2 = New Window2
            spl.Show()
            Thread.Sleep(1000)

            ' Cet appel est requis par le Concepteur Windows Form.
            InitializeComponent()

            spl.Close()
            spl = Nothing

            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            Dim dt As DispatcherTimer = New DispatcherTimer()
            AddHandler dt.Tick, AddressOf dispatcherTimer_Tick
            dt.Interval = New TimeSpan(0, 0, 1)
            dt.Start()

            Myfile = MyRep & "\Config\HoMIAdmiN.xml"

            ErazeMnu()
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
                    mytime = ""

                    If LblConnect.ToolTip Is Nothing Then
                        Dim tool As New ToolTip
                        tool.Content = "Serveur connecté adresse utilisée: " & myChannelFactory.Endpoint.Address.ToString()
                        LblConnect.ToolTip = tool
                        tool = Nothing
                    End If
                    
                    LblConnect.Content = Mid("Serveur connecté adresse utilisée: " & myChannelFactory.Endpoint.Address.ToString(), 1, 64) & "..."

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
                    myBrush = Nothing

                Catch ex As Exception
                    IsConnect = False
                    LblStatus.Content = Now.ToLongDateString & " " & Now.ToLongTimeString & " "
                    LblConnect.Content = "Serveur non connecté"
                    LblConnect.ToolTip = Nothing

                    TreeViewG.Items.Clear()

                    Dim myBrush As New RadialGradientBrush()
                    myBrush.GradientOrigin = New Point(0.75, 0.25)
                    myBrush.GradientStops.Add(New GradientStop(Colors.Yellow, 0.0))
                    myBrush.GradientStops.Add(New GradientStop(Colors.Red, 0.5))
                    myBrush.GradientStops.Add(New GradientStop(Colors.DarkRed, 1.0))
                    Ellipse1.Fill = myBrush
                    myBrush = Nothing
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
                myBrush = Nothing
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR dispatcherTimer_Tick: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Menu Quitter
    Private Sub Quitter(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuQuitter.Click
        Try
            If IsConnect = True Then
                Dim retour As MessageBoxResult
                retour = MessageBox.Show("Voulez-vous enregistrer la configuration avant de quitter?", "HomIAdmin", MessageBoxButton.YesNo, MessageBoxImage.Question)

                If retour = MessageBoxResult.Yes Then
                    Try
                        If IsConnect = False Then
                            MessageBox.Show("Impossible d'enregistrer la configuration car le serveur n'est pas connecté !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Asterisk)
                        Else
                            myService.SaveConfig(IdSrv)
                        End If
                    Catch ex As Exception
                        MessageBox.Show("ERREUR Sub Quitter: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
                    End Try
                End If
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
            TreeViewG.Items.Clear()
            If IsConnect = False Then Exit Sub
            CntZone.Content = myService.GetAllZones(IdSrv).Count & " Zone(s)"

            For Each zon In myService.GetAllZones(IdSrv)
                Dim newchild As New TreeViewItem
                Dim stack As New StackPanel
                Dim img As New Image
                Dim uri As String = ""
                Dim bmpImage As New BitmapImage()
                stack.Orientation = Orientation.Horizontal

                img.Height = 20
                img.Width = 20

                If zon.Icon <> "" Then
                    img.Source = ConvertArrayToImage(Window1.myService.GetByteFromImage(zon.Icon))
                End If

                Dim label As New Label
                label.Foreground = New SolidColorBrush(Colors.White)
                label.Content = zon.Name

                stack.Children.Add(img)
                stack.Children.Add(label)

                newchild.Foreground = New SolidColorBrush(Colors.White)
                newchild.Header = stack
                newchild.Uid = zon.ID
                TreeViewG.Items.Add(newchild)
            Next

        Catch ex As Exception
            MessageBox.Show("ERREUR Sub AffZone: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Afficher la liste des users
    Public Sub AffUser()
        Try
            TreeViewG.Items.Clear()
            If IsConnect = False Then Exit Sub
            CntUser.Content = myService.GetAllUsers(IdSrv).Count & " User(s)"

            For Each Usr In myService.GetAllUsers(IdSrv)
                Dim newchild As New TreeViewItem
                Dim stack As New StackPanel
                Dim img As New Image
                Dim uri As String = ""
                Dim bmpImage As New BitmapImage()

                stack.Orientation = Orientation.Horizontal

                img.Height = 20
                img.Width = 20

                If Usr.Image <> "" And File.Exists(Usr.Image) = True Then
                    uri = Usr.Image
                Else
                    uri = MyRep & "\Images\icones\user.png"
                End If
                bmpImage.BeginInit()
                bmpImage.UriSource = New Uri(uri, UriKind.Absolute)
                bmpImage.EndInit()
                img.Source = bmpImage

                Dim label As New Label
                label.Foreground = New SolidColorBrush(Colors.White)
                label.Content = Usr.UserName

                stack.Children.Add(img)
                stack.Children.Add(label)

                newchild.Foreground = New SolidColorBrush(Colors.White)
                newchild.Header = stack
                newchild.Uid = Usr.ID
                TreeViewG.Items.Add(newchild)
            Next

        Catch ex As Exception
            MessageBox.Show("ERREUR Sub AffUser: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Afficher la liste des drivers
    Public Sub AffDriver()
        Try
            TreeViewG.Items.Clear()
            If IsConnect = False Then Exit Sub
            CntDriver.Content = myService.GetAllDrivers(IdSrv).Count & " Driver(s)"

            For Each Drv In myService.GetAllDrivers(IdSrv)
                Dim newchild As New TreeViewItem
                Dim stack As New StackPanel
                stack.Orientation = Orientation.Horizontal
                stack.HorizontalAlignment = HorizontalAlignment.Left

                Dim Elipse As New Ellipse
                Elipse.Width = 9
                Elipse.Height = 9

                Dim myBrush As New RadialGradientBrush()
                myBrush.GradientOrigin = New Point(0.75, 0.25)

                If Drv.IsConnect = True Then
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
                If Drv.Enable = True Then
                    label.Foreground = New SolidColorBrush(Colors.White)
                Else
                    label.Foreground = New SolidColorBrush(Colors.Black)
                End If
                label.Content = Drv.Nom

                stack.Children.Add(Elipse)
                stack.Children.Add(label)

                newchild.Foreground = New SolidColorBrush(Colors.White)
                newchild.Header = stack
                newchild.Uid = Drv.ID
                TreeViewG.Items.Add(newchild)
            Next

            For i As Integer = 0 To myService.GetAllDrivers(IdSrv).Count - 1 'Obj.Drivers.Count - 1
                

            Next
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub AffDriver: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Afficher la liste des devices
    Public Sub AffDevice()
        Try
            TreeViewG.Items.Clear()
            If IsConnect = False Then Exit Sub

            CntDevice.Content = myService.GetAllDevices(IdSrv).Count & " Device(s)"

            For Each Dev In myService.GetAllDevices(IdSrv)

                Dim newchild As New TreeViewItem
                Dim stack As New StackPanel
                Dim img As New Image
                Dim uri As String = ""
                Dim bmpImage As New BitmapImage()
                stack.Orientation = Orientation.Horizontal

                img.Height = 20
                img.Width = 20

                If Dev.Picture <> "" And File.Exists(Dev.Picture) = True Then
                    uri = Dev.Picture
                Else
                    uri = MyRep & "\Images\Devices\Defaut-128.png"
                End If

                bmpImage.BeginInit()
                bmpImage.UriSource = New Uri(uri, UriKind.Absolute)
                bmpImage.EndInit()
                img.Source = bmpImage

                Dim drv As String = Dev.Name
                drv &= " (" & myService.ReturnDriverByID(IdSrv, Dev.DriverID).Nom & ")"

                Dim tl As New ToolTip
                tl.Content = drv
                Dim label As New Label
                If Dev.Enable = True Then
                    label.Foreground = New SolidColorBrush(Colors.White)
                Else
                    label.Foreground = New SolidColorBrush(Colors.Black)
                End If
                label.Content = drv
                label.ToolTip = tl

                stack.Children.Add(img)
                stack.Children.Add(label)

                newchild.Foreground = New SolidColorBrush(Colors.White)
                newchild.Header = stack
                newchild.Uid = Dev.ID
                TreeViewG.Items.Add(newchild)
            Next
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub AffDevice: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Afficher la liste des scenes
    Public Sub AffScene()
        Try
            TreeViewG.Items.Clear()
            If IsConnect = False Then Exit Sub
            CntMacro.Content = myService.GetAllMacros(IdSrv).Count & " Macro(s)"

            For Each Mac In myService.GetAllMacros(IdSrv)
                Dim newchild As New TreeViewItem
                Dim stack As New StackPanel
                Dim img As New Image
                Dim uri As String = ""
                Dim bmpImage As New BitmapImage()

                stack.Orientation = Orientation.Horizontal

                img.Height = 20
                img.Width = 20

                Dim label As New Label
                If Mac.Enable = True Then
                    label.Foreground = New SolidColorBrush(Colors.White)
                Else
                    label.Foreground = New SolidColorBrush(Colors.Black)
                End If
                label.Content = Mac.Nom

                uri = MyRep & "\Images\Icones\script-128.png"
                bmpImage.BeginInit()
                bmpImage.UriSource = New Uri(uri, UriKind.Absolute)
                bmpImage.EndInit()
                img.Source = bmpImage

                stack.Children.Add(img)
                stack.Children.Add(label)

                newchild.Foreground = New SolidColorBrush(Colors.White)
                newchild.Header = stack
                newchild.Uid = Mac.ID
                TreeViewG.Items.Add(newchild)
            Next

        Catch ex As Exception
            MessageBox.Show("ERREUR Sub AffScene: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Afficher la liste des triggers
    Public Sub AffTrigger()
        Try
            TreeViewG.Items.Clear()
            If IsConnect = False Then Exit Sub
            CntTrigger.Content = myService.GetAllTriggers(IdSrv).Count & " Trigger(s)"

            For Each Trig In myService.GetAllTriggers(IdSrv)
                Dim newchild As New TreeViewItem
                Dim stack As New StackPanel
                Dim img As New Image
                Dim uri As String = ""
                Dim bmpImage As New BitmapImage()

                stack.Orientation = Orientation.Horizontal

                img.Height = 20
                img.Width = 20

                Dim label As New Label
                If Trig.Enable = True Then
                    label.Foreground = New SolidColorBrush(Colors.White)
                Else
                    label.Foreground = New SolidColorBrush(Colors.Black)
                End If
                label.Content = Trig.Nom

                uri = MyRep & "\Images\Icones\drapeau-vert-32.png"
                bmpImage.BeginInit()
                bmpImage.UriSource = New Uri(uri, UriKind.Absolute)
                bmpImage.EndInit()
                img.Source = bmpImage

                stack.Children.Add(img)
                stack.Children.Add(label)

                newchild.Foreground = New SolidColorBrush(Colors.White)
                newchild.Header = stack
                newchild.Uid = Trig.ID
                TreeViewG.Items.Add(newchild)
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
            TreeViewG.Items.Clear()
            Dim x As New List(Of HoMIDom.HoMIDom.Historisation)
            x = myService.GetAllListHisto(IdSrv)

            If x IsNot Nothing Then
                For i As Integer = 0 To x.Count - 1
                    Dim y As New CheckBox
                    Dim a As Historisation = x.Item(i)
                    Dim b As String = myService.ReturnDeviceByID(IdSrv, a.IdDevice).Name
                    If b = "" Then b = "?"
                    y.Content = a.Nom & " {" & b & "}"
                    y.Tag = a.Nom
                    y.Uid = a.IdDevice
                    y.Foreground = New SolidColorBrush(Colors.White)
                    TreeViewG.Items.Add(y)
                Next
            End If
        Catch ex As Exception

        End Try
    End Sub
#End Region

#Region "Drivers"
    Private Sub BtnStop_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnStop.Click
        Try
            If IsConnect = False Then
                MessageBox.Show("Impossible le serveur n'est pas connecté !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Asterisk)
                Exit Sub
            End If

            If TreeViewG.SelectedItem IsNot Nothing Then
                myService.StopDriver(IdSrv, TreeViewG.SelectedItem.uid)
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
            If IsConnect = False Then
                MessageBox.Show("Impossible le serveur n'est pas connecté !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Asterisk)
                Exit Sub
            End If

            If TreeViewG.SelectedItem IsNot Nothing Then
                If myService.ReturnDriverByID(IdSrv, TreeViewG.SelectedItem.uid).Enable = True Then
                    myService.StartDriver(IdSrv, TreeViewG.SelectedItem.uid)
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
    'Bouton nouveau Device
    Private Sub BtnNewDevice_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnNewDevice.Click
        Try
            If IsConnect = False Then
                MessageBox.Show("Impossible le serveur n'est pas connecté !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Asterisk)
                Exit Sub
            End If

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
            If IsConnect = False Then
                MessageBox.Show("Impossible le serveur n'est pas connecté !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Asterisk)
                Exit Sub
            End If

            If TreeViewG.SelectedItem IsNot Nothing Then
                Dim retour As Integer = Window1.myService.DeleteDevice(IdSrv, TreeViewG.SelectedItem.uid)
                If retour = -2 Then
                    MessageBox.Show("Vous ne pouvez pas supprimer ce device !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                    Exit Sub
                End If
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
            If IsConnect = False Then
                MessageBox.Show("Impossible le serveur n'est pas connecté !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Asterisk)
                Exit Sub
            End If

            Dim x As New uZone(uDevice.EAction.Nouveau, "")
            x.Uid = System.Guid.NewGuid.ToString()
            AddHandler x.CloseMe, AddressOf UnloadControl
            CanvasRight.Children.Clear()
            CanvasRight.Children.Add(x)
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub BtnNewZone_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'bouton supprimer une zone
    Private Sub BtnDelZone_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDelZone.Click
        Try
            If IsConnect = False Then
                MessageBox.Show("Impossible le serveur n'est pas connecté !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Asterisk)
                Exit Sub
            End If

            If TreeViewG.SelectedItem IsNot Nothing Then
                Window1.myService.DeleteZone(IdSrv, TreeViewG.SelectedItem.uid)
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
            If IsConnect = False Then
                MessageBox.Show("Impossible le serveur n'est pas connecté !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Asterisk)
                Exit Sub
            End If

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
            If IsConnect = False Then
                MessageBox.Show("Impossible le serveur n'est pas connecté !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Asterisk)
                Exit Sub
            End If

            If TreeViewG.SelectedItem IsNot Nothing And TreeViewG.SelectedItem.uid IsNot Nothing Then
                Dim cntAdmin As Integer = 0
                For i As Integer = 0 To Window1.myService.GetAllUsers(IdSrv).Count - 1
                    If Window1.myService.GetAllUsers(IdSrv).Item(i).Profil = Users.TypeProfil.admin Then
                        cntAdmin += 1
                    End If
                Next
                If cntAdmin <= 1 Then
                    MessageBox.Show("Impossible de supprimer cet utilisateur car il n'existe qu'un profil Administrateur !")
                    Exit Sub
                End If
                'If MessageBox.Show("Etes vous sur de supprimer ce user : " & TreeViewUsers.SelectedItem & " ?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.Yes Then
                If MessageBox.Show("Etes vous sur de supprimer ce user: ?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.Yes Then
                    Window1.myService.DeleteUser(IdSrv, TreeViewG.SelectedItem.uid)
                    AffUser()
                End If
            Else
                MessageBox.Show("Veuillez sélectionner un utilisateur à supprimer!")
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub BtnDelUser_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

#End Region

#Region "Trigger"
    Private Sub BtnNewTriggerDevice_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnNewTriggerDevice.Click
        Try
            If IsConnect = False Then
                MessageBox.Show("Impossible le serveur n'est pas connecté !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Asterisk)
                Exit Sub
            End If

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
            If IsConnect = False Then
                MessageBox.Show("Impossible le serveur n'est pas connecté !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Asterisk)
                Exit Sub
            End If

            Dim x As New uTriggerTimer(0, "")
            x.Uid = System.Guid.NewGuid.ToString()
            AddHandler x.CloseMe, AddressOf UnloadControl
            CanvasRight.Children.Clear()
            CanvasRight.Children.Add(x)
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub BtnNewTriggerTime_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub


    Private Sub BtnDelTrigger_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDelTrigger.Click
        Try
            If IsConnect = False Then
                MessageBox.Show("Impossible le serveur n'est pas connecté !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Asterisk)
                Exit Sub
            End If

            If TreeViewG.SelectedItem IsNot Nothing Then
                If TreeViewG.SelectedItem.uid IsNot Nothing Then
                    If MessageBox.Show("Etes vous sur de supprimer ce trigger: " & myService.ReturnTriggerById(IdSrv, TreeViewG.SelectedItem.uid).Nom & " ?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.Yes Then
                        Window1.myService.DeleteTrigger(IdSrv, TreeViewG.SelectedItem.uid)
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


    'supprimer macro
    Private Sub BtnDelMacro_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDelMacro.Click
        Try
            If IsConnect = False Then
                MessageBox.Show("Impossible le serveur n'est pas connecté !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Asterisk)
                Exit Sub
            End If

            If TreeViewG.SelectedItem IsNot Nothing Then
                If TreeViewG.SelectedItem.uid IsNot Nothing Then
                    If MessageBox.Show("Etes vous sur de supprimer cette macro: " & myService.ReturnMacroById(IdSrv, TreeViewG.SelectedItem.uid).Nom & " ?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.Yes Then
                        Window1.myService.DeleteMacro(IdSrv, TreeViewG.SelectedItem.uid)
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

            myService.SaveConfig(IdSrv)
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub MnuSaveConfig: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Décharger une fenêtre suivant son Id
    Public Sub UnloadControl(ByVal MyControl As Object)
        Try
            CanvasRight.Children.Clear()
            Select Case _CurrentMnu
                Case 0
                    AffDriver()
                Case 1
                    AffDevice()
                Case 2
                    AffZone()
                Case 3
                    AffUser()
                Case 4
                    AffTrigger()
                Case 5
                    AffScene()
                Case 6
                    AffHisto()
            End Select
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
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub MnuViewLog: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Menu à propos
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
    Public Function Connect_Srv(ByVal Name As String, ByVal IP As String, ByVal Port As String) As Integer
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
            If myChannelFactory.State <> CommunicationState.Opened Then
                MessageBox.Show("Erreur lors de la connexion au serveur", "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
                IsConnect = False
                Return -1
                Exit Function
            Else
                IsConnect = True
            End If


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

            Return 0
        Catch ex As Exception
            myChannelFactory.Abort()
            IsConnect = False
            MessageBox.Show("Erreur lors de la connexion au serveur sélectionné: " & ex.Message, "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
            Return -1
        End Try
    End Function

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
                If Connect_Srv(frm.TxtName.Text, frm.TxtIP.Text, frm.TxtPort.Text) <> 0 Then
                    Exit Sub
                End If
                If myService.GetIdServer(IdSrv) = "99" Then
                    MessageBox.Show("L'ID du serveur est erroné, impossible de communiquer avec celui-ci", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                    Exit Sub
                End If
                'If myService.VerifLogin(frm.TxtUsername.Text, frm.TxtPassword.Password) = False Then
                '    MessageBox.Show("Le username ou le password sont erroné, impossible veuillez réessayer", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                'Else
                frm.Close()
                FlagStart = True
                frm = Nothing
                'End If
            Else
            End
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Connexion: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
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

        For i As Integer = 0 To TreeViewG.Items.Count - 1
            Dim chk As CheckBox = TreeViewG.Items(i)

            If chk.IsChecked = True Then
                Dim _listhisto As New List(Of Historisation)
                _listhisto = myService.GetHisto(IdSrv, chk.Tag, chk.Uid)

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
            End If


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

#Region "BtnMnu"

    Private Sub MnuDriver_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MnuDriver.Click
        _CurrentMnu = 0
        ErazeMnu()
        StkMnu0.Height = Double.NaN
        AffDriver()
        BtnStart.Visibility = Windows.Visibility.Hidden
        BtnStop.Visibility = Windows.Visibility.Hidden
    End Sub

    Private Sub MnuDevice_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MnuDevice.Click
        _CurrentMnu = 1
        ErazeMnu()
        StkMnu1.Height = Double.NaN
        AffDevice()
    End Sub

    Private Sub MnuZone_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MnuZone.Click
        _CurrentMnu = 2
        ErazeMnu()
        StkMnu2.Height = Double.NaN
        AffZone()
    End Sub

    Private Sub MnuUser_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MnuUser.Click
        _CurrentMnu = 3
        ErazeMnu()
        StkMnu3.Height = Double.NaN
        AffUser()
    End Sub

    Private Sub MnuTrigger_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MnuTrigger.Click
        _CurrentMnu = 4
        ErazeMnu()
        StkMnu4.Height = Double.NaN
        AffTrigger()
    End Sub

    Private Sub MnuMacro_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MnuMacro.Click
        _CurrentMnu = 5
        ErazeMnu()
        StkMnu5.Height = Double.NaN
        AffScene()
    End Sub

    Private Sub MnuHisto_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MnuHisto.Click
        _CurrentMnu = 6
        ErazeMnu()
        StkMnu6.Height = Double.NaN
        AffHisto()
    End Sub

    Private Sub TreeViewG_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles TreeViewG.MouseDoubleClick
        Try
            If IsConnect = False Then
                MessageBox.Show("Impossible le serveur n'est pas connecté !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Asterisk)
                Exit Sub
            End If

            If TreeViewG.SelectedItem IsNot Nothing Then
                If TreeViewG.SelectedItem.uid Is Nothing Then Exit Sub

                Select Case _CurrentMnu
                    Case 0 'driver
                        Dim x As New uDriver(TreeViewG.SelectedItem.uid)
                        x.Uid = System.Guid.NewGuid.ToString()
                        AddHandler x.CloseMe, AddressOf UnloadControl
                        CanvasRight.Children.Clear()
                        CanvasRight.Children.Add(x)
                    Case 1 'device
                        Dim x As New uDevice(uDevice.EAction.Modifier, TreeViewG.SelectedItem.uid)
                        x.Uid = System.Guid.NewGuid.ToString()
                        AddHandler x.CloseMe, AddressOf UnloadControl
                        CanvasRight.Children.Clear()
                        CanvasRight.Children.Add(x)
                    Case 2 'zone
                        Dim x As New uZone(uDevice.EAction.Modifier, TreeViewG.SelectedItem.uid)
                        x.Uid = System.Guid.NewGuid.ToString()
                        AddHandler x.CloseMe, AddressOf UnloadControl
                        CanvasRight.Children.Clear()
                        CanvasRight.Children.Add(x)
                    Case 3 'user
                        Dim x As New uUser(uDevice.EAction.Modifier, TreeViewG.SelectedItem.uid)
                        x.Uid = System.Guid.NewGuid.ToString()
                        AddHandler x.CloseMe, AddressOf UnloadControl
                        CanvasRight.Children.Clear()
                        CanvasRight.Children.Add(x)
                    Case 4 'trigger
                        Dim _Trig As Trigger = myService.ReturnTriggerById(IdSrv, TreeViewG.SelectedItem.uid)

                            If _Trig IsNot Nothing Then
                                If _Trig.Type = Trigger.TypeTrigger.TIMER Then
                                    Dim x As New uTriggerTimer(uTriggerTimer.EAction.Modifier, TreeViewG.SelectedItem.uid)
                                    x.Uid = System.Guid.NewGuid.ToString()
                                    AddHandler x.CloseMe, AddressOf UnloadControl
                                    CanvasRight.Children.Clear()
                                    CanvasRight.Children.Add(x)
                                Else
                                    Dim x As New uTriggerDevice(uTriggerDevice.EAction.Modifier, TreeViewG.SelectedItem.uid)
                                    x.Uid = System.Guid.NewGuid.ToString()
                                    AddHandler x.CloseMe, AddressOf UnloadControl
                                    CanvasRight.Children.Clear()
                                    CanvasRight.Children.Add(x)
                                End If
                                _Trig = Nothing
                            End If
                    Case 5 'macros
                        Dim x As New uMacro(uMacro.EAction.Modifier, TreeViewG.SelectedItem.uid)
                        x.Uid = System.Guid.NewGuid.ToString()
                        AddHandler x.CloseMe, AddressOf UnloadControl
                        CanvasRight.Children.Clear()
                        CanvasRight.Children.Add(x)
                    Case 6 'histo

                End Select
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub TreeView_MouseDoubleClick: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub TreeViewG_SelectedItemChanged(ByVal sender As System.Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Object)) Handles TreeViewG.SelectedItemChanged
        Try
            If IsConnect = False Then
                MessageBox.Show("Impossible le serveur n'est pas connecté !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Asterisk)
                Exit Sub
            End If

            If Mouse.LeftButton = MouseButtonState.Pressed Then
                If TreeViewG.SelectedItem IsNot Nothing Then
                    Dim effects As DragDropEffects
                    Dim obj As New DataObject()
                    obj.SetData(GetType(String), TreeViewG.SelectedItem.uid)
                    effects = DragDrop.DoDragDrop(Me.TreeViewG, obj, DragDropEffects.Copy Or DragDropEffects.Move)
                End If
            End If

            If TreeViewG.SelectedItem IsNot Nothing Then
                If TreeViewG.SelectedItem.uid Is Nothing Then Exit Sub

                Select Case _CurrentMnu
                    Case 0 'driver
                        If myService.ReturnDriverByID(IdSrv, TreeViewG.SelectedItem.uid).ID = "DE96B466-2540-11E0-A321-65D7DFD72085" Then
                            BtnStart.Visibility = Windows.Visibility.Hidden
                            BtnStop.Visibility = Windows.Visibility.Hidden
                        Else
                            If myService.ReturnDriverByID(IdSrv, TreeViewG.SelectedItem.uid).IsConnect Then
                                BtnStart.Visibility = Windows.Visibility.Hidden
                                BtnStop.Visibility = Windows.Visibility.Visible
                            Else
                                BtnStart.Visibility = Windows.Visibility.Visible
                                BtnStop.Visibility = Windows.Visibility.Hidden
                            End If
                        End If
                    Case 1 'device
                        If Mid(myService.ReturnDeviceByID(IdSrv, TreeViewG.SelectedItem.uid).Name, 1, 4) = "HOMI" Then
                            BtnDelDevice.Visibility = Windows.Visibility.Hidden
                        Else
                            BtnDelDevice.Visibility = Windows.Visibility.Visible
                        End If
                    Case 2 'zone

                    Case 3 'user

                    Case 4 'trigger

                    Case 5 'macros

                    Case 6 'histo

                End Select
            End If

        Catch ex As Exception
            MessageBox.Show("ERREUR Sub TreeViewG_SelectedItemChanged: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
#End Region

    Private Sub ErazeMnu()
        StkMnu0.Height = 0
        StkMnu1.Height = 0
        StkMnu2.Height = 0
        StkMnu3.Height = 0
        StkMnu4.Height = 0
        StkMnu5.Height = 0
        StkMnu6.Height = 0
    End Sub

    Private Sub BtnGenereReleve_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnGenereReleve.Click
        Try

            Dim _listhisto As New List(Of Historisation)
            Dim _Two As Boolean = False
            Dim lbl As String = ""

            For i As Integer = 0 To TreeViewG.Items.Count - 1
                Dim chk As CheckBox = TreeViewG.Items(i)

                If chk.IsChecked = True And _Two = False Then
                    _listhisto = myService.GetHisto(IdSrv, chk.Tag, chk.Uid)
                    lbl = chk.Content
                    _Two = True
                Else
                    If chk.IsChecked = True And _Two = True Then
                        MessageBox.Show("Seul un élément peut être affiché dans les relevés!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                        Exit Sub
                    End If
                End If
            Next

            If _listhisto IsNot Nothing And _Two Then
                Dim x As New uReleve(_listhisto, lbl)
                x.Uid = System.Guid.NewGuid.ToString()
                AddHandler x.CloseMe, AddressOf UnloadControl
                CanvasRight.Children.Clear()
                CanvasRight.Children.Add(x)
            End If

        Catch ex As Exception
            MessageBox.Show("Erreur lors de la génération du relevé: " & ex.ToString, "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub Ellipse1_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Ellipse1.MouseDown
        If IsConnect = False Then PageConnexion()
    End Sub
End Class
