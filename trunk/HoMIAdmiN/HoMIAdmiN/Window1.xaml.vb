﻿Imports System.Windows.Threading
Imports System.Runtime.Serialization.Formatters.Soap
Imports HoMIDom.HoMIDom
Imports System.ServiceModel
Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Threading
Imports System.Reflection.Assembly
Imports System.Windows.Media.Animation

Class Window1

    Public Event menu_gerer(ByVal IndexMenu As Integer)

    Public Shared CanvasUser As Canvas
    Public Shared ListServer As New List(Of ClServer)

    Dim Myfile As String
    Dim MyPort As String = ""
    Dim myChannelFactory As ServiceModel.ChannelFactory(Of HoMIDom.HoMIDom.IHoMIDom) = Nothing
    Dim myadress As String = ""
    Dim FlagStart As Boolean = False
    Dim MemCanvas As Canvas
    Dim MyRep As String = System.Environment.CurrentDirectory
    Dim WMainMenu As Double = 0
    Dim HMainMenu As Double = 0
    Dim _MainMenuAction As Integer = -1 'NEW,MODIF,SUPP -1 si aucun
    Dim myBrushVert As New RadialGradientBrush()
    Dim myBrushRouge As New RadialGradientBrush()
    Dim flagTreeV As Boolean = False

#Region "Fonctions de base"

    Public Sub New()
        Try
            Me.Title = "HoMIAdmiN v" & My.Application.Info.Version.ToString & " - HoMIDoM"

            Dim spl As Window2 = New Window2
            spl.Show()
            Thread.Sleep(1000)

            ' Cet appel est requis par le Concepteur Windows Form.
            InitializeComponent()

            myBrushVert.GradientOrigin = New Point(0.75, 0.25)
            myBrushVert.GradientStops.Add(New GradientStop(Colors.LightGreen, 0.0))
            myBrushVert.GradientStops.Add(New GradientStop(Colors.Green, 0.5))
            myBrushVert.GradientStops.Add(New GradientStop(Colors.DarkGreen, 1.0))

            myBrushRouge.GradientOrigin = New Point(0.75, 0.25)
            myBrushRouge.GradientStops.Add(New GradientStop(Colors.Yellow, 0.0))
            myBrushRouge.GradientStops.Add(New GradientStop(Colors.Red, 0.5))
            myBrushRouge.GradientStops.Add(New GradientStop(Colors.DarkRed, 1.0))


            'CloseTreeView()
            flagTreeV = True

            spl.Close()
            spl = Nothing

            'AddHandler Window1.menu_gerer, AddressOf MainMenuGerer
            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            Dim dt As DispatcherTimer = New DispatcherTimer()
            AddHandler dt.Tick, AddressOf dispatcherTimer_Tick
            dt.Interval = New TimeSpan(0, 0, 1)
            dt.Start()

            Myfile = MyRep & "\Config\HoMIAdmiN.xml"

        Catch ex As Exception
            MessageBox.Show("ERREUR Sub New: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try

    End Sub

    'Affiche la date et heure, heures levé et couché du soleil
    Public Sub dispatcherTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        Try
            If IsConnect = True Then
                Try
                    'Modifie la date et Heure
                    Dim mytime As String = myService.GetTime
                    LblStatus.Content = Now.ToLongDateString & " " & mytime & " "
                    mytime = ""

                    'Modifie l'adresse/port du serveur connecté
                    Dim serveurcomplet As String = myChannelFactory.Endpoint.Address.ToString()
                    serveurcomplet = Mid(serveurcomplet, 8, serveurcomplet.Length - 8)
                    serveurcomplet = Split(serveurcomplet, "/", 2)(0)
                    Dim serveuradresse As String = Split(serveurcomplet, ":", 2)(0)
                    Dim serveurport As String = Split(serveurcomplet, ":", 2)(1)
                    LblConnect.ToolTip = "Serveur connecté adresse utilisée: " & serveuradresse & ":" & serveurport
                    'LblConnect.Content = Mid(myChannelFactory.Endpoint.Address.ToString(), 1, 32) & "..."
                    LblConnect.Content = serveuradresse & ":" & serveurport

                    'Modifie les heures du soleil
                    Dim mydate As Date
                    mydate = myService.GetHeureLeverSoleil
                    LHS.Content = mydate.ToShortTimeString
                    mydate = myService.GetHeureCoucherSoleil
                    LCS.Content = mydate.ToShortTimeString

                    'Modifie les LOG
                    Dim list As List(Of String) = myService.Get4Log
                    Dim a As String = ""
                    For i As Integer = 0 To list.Count - 1
                        a &= list(i)
                        If (i <> list.Count - 1) Then a &= vbCrLf
                    Next
                    LOG.ToolTip = a
                    ImgLog.ToolTip = a
                    LOG.Content = Mid(list(0), 1, 100) & "..."

                    Ellipse1.Fill = myBrushVert
                Catch ex As Exception
                    IsConnect = False
                    LblStatus.Content = Now.ToLongDateString & " " & Now.ToLongTimeString & " "
                    LblConnect.Content = "Serveur non connecté"
                    LblConnect.ToolTip = Nothing

                    Serveur_notconnected_action()

                    Ellipse1.Fill = myBrushVert
                End Try
            Else
                LblStatus.Content = Now.ToLongDateString & " " & Now.ToLongTimeString & "      "
                LblConnect.Content = "Serveur non connecté"

                Ellipse1.Fill = myBrushRouge
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR dispatcherTimer_Tick: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub StoryBoardFinish(ByVal sender As Object, ByVal e As System.EventArgs)
        CanvasRight.Children.Clear()
        ShowMainMenu()
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
            binding.MaxBufferPoolSize = 250000000
            binding.MaxReceivedMessageSize = 250000000
            binding.MaxBufferSize = 250000000
            binding.ReaderQuotas.MaxArrayLength = 250000000
            binding.ReaderQuotas.MaxNameTableCharCount = 250000000
            binding.ReaderQuotas.MaxBytesPerRead = 250000000
            binding.ReaderQuotas.MaxStringContentLength = 250000000
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
                Try
                    myService.GetTime()
                Catch ex As Exception
                    myChannelFactory.Abort()
                    IsConnect = False
                    MessageBox.Show("Erreur lors de la connexion au serveur sélectionné: " & Chr(10) & Name & " - " & IP & ":" & Port, "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
                    Return -1
                End Try
                IsConnect = True
                Tabcontrol1.SelectedIndex = 0
                AffDriver()
                ShowMainMenu()
            End If

            LblSrv.Content = Name
            LblSrv.ToolTip = "Serveur courant : " & Name
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
                    frm.Close()
                    PageConnexion()
                    Exit Sub
                Else
                    If myService.GetIdServer(IdSrv) = "99" Then
                        MessageBox.Show("L'ID du serveur est erroné, impossible de communiquer avec celui-ci", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                        frm.Close()
                        Exit Sub
                    End If
                    'If myService.VerifLogin(frm.TxtUsername.Text, frm.TxtPassword.Password) = False Then
                    '    MessageBox.Show("Le username ou le password sont erroné, impossible veuillez réessayer", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                    'Else
                    frm.Close()
                    FlagStart = True
                    frm = Nothing
                    'End If
                End If
            Else
                End
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Connexion: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'action à faire quand le serveur n'est pas connecté
    Private Sub Serveur_notconnected_action()
        MessageBox.Show("Action Impossible, le serveur n'est pas connecté !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Asterisk)
        ClearAllTreeview() 'vide le treeview
        'CloseTreeView() 'ferme le treeview
        CanvasRight.Children.Clear()  'ferme le menu principal
        PageConnexion() 'affiche la fenetre de connexion
    End Sub

#End Region

#Region "Treeview"

    Private Sub CloseTreeView()
        DKpanel.Width = 0
        flagTreeV = False
    End Sub

    Private Sub ShowTreeView()
        If flagTreeV = True Then Exit Sub
        DKpanel.Width = Double.NaN
        Dim da3 As DoubleAnimation = New DoubleAnimation
        da3.From = 0
        da3.To = 1
        da3.Duration = New Duration(TimeSpan.FromMilliseconds(600))
        Dim sc As ScaleTransform = New ScaleTransform()
        DKpanel.RenderTransform = sc
        sc.BeginAnimation(ScaleTransform.ScaleXProperty, da3)
        flagTreeV = True

    End Sub

    Private Sub ClearAllTreeview()
        TreeViewDriver.Items.Clear()
        TreeViewDevice.Items.Clear()
        TreeViewZone.Items.Clear()
        TreeViewUser.Items.Clear()
        TreeViewTrigger.Items.Clear()
        TreeViewMacro.Items.Clear()
        TreeViewHisto.Items.Clear()
    End Sub

    Private Sub RefreshTreeView()
        If IsConnect = False Then
            'Serveur_notconnected_action()
            Exit Sub
        End If

        Me.Cursor = Cursors.Wait
        Select Case Tabcontrol1.SelectedIndex
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
        Me.Cursor = Nothing
    End Sub

    'Afficher la liste des zones
    Public Sub AffZone()
        Try
            TreeViewZone.Items.Clear()
            If IsConnect = False Then Exit Sub

            Dim ListeZones = myService.GetAllZones(IdSrv)
            CntZone.Content = ListeZones.Count & " Zone(s)"

            For Each zon In ListeZones
                Dim newchild As New TreeViewItem
                Dim stack As New StackPanel
                Dim img As New Image
                Dim uri As String = ""
                Dim bmpImage As New BitmapImage()
                stack.Orientation = Orientation.Horizontal

                img.Height = 20
                img.Width = 20

                If Trim(zon.Icon) <> "" Then
                    img.Source = ConvertArrayToImage(myService.GetByteFromImage(zon.Icon))
                Else
                    uri = MyRep & "\Images\icones\Zone_32.png"
                    bmpImage.BeginInit()
                    bmpImage.UriSource = New Uri(uri, UriKind.Absolute)
                    bmpImage.EndInit()
                    img.Source = bmpImage
                End If

                Dim label As New Label
                label.Foreground = New SolidColorBrush(Colors.White)
                label.Content = zon.Name & " {" & zon.ListElement.Count & " éléments}"

                stack.Children.Add(img)
                stack.Children.Add(label)

                newchild.Foreground = New SolidColorBrush(Colors.White)
                newchild.Header = stack
                newchild.Uid = zon.ID
                Dim marg As New Thickness(-12, 0, 0, 0)
                newchild.Margin = marg
                TreeViewZone.Items.Add(newchild)
            Next

        Catch ex As Exception
            MessageBox.Show("ERREUR Sub AffZone: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Afficher la liste des users
    Public Sub AffUser()
        Try
            TreeViewUser.Items.Clear()
            If IsConnect = False Then Exit Sub

            Dim ListeUsers = myService.GetAllUsers(IdSrv)
            CntUser.Content = ListeUsers.Count & " User(s)"

            For Each Usr In ListeUsers
                Dim newchild As New TreeViewItem
                Dim stack As New StackPanel
                Dim img As New Image
                Dim uri As String = ""
                Dim bmpImage As New BitmapImage()

                stack.Orientation = Orientation.Horizontal

                img.Height = 20
                img.Width = 20

                If Trim(Usr.Image) <> "" Then
                    img.Source = ConvertArrayToImage(myService.GetByteFromImage(Usr.Image))
                Else
                    uri = MyRep & "\Images\icones\User_32.png"
                    bmpImage.BeginInit()
                    bmpImage.UriSource = New Uri(uri, UriKind.Absolute)
                    bmpImage.EndInit()
                    img.Source = bmpImage
                End If


                Dim label As New Label
                label.Foreground = New SolidColorBrush(Colors.White)
                label.Content = Usr.UserName

                stack.Children.Add(img)
                stack.Children.Add(label)

                newchild.Foreground = New SolidColorBrush(Colors.White)
                newchild.Header = stack
                newchild.Uid = Usr.ID
                Dim marg As New Thickness(-12, 0, 0, 0)
                newchild.Margin = marg
                TreeViewUser.Items.Add(newchild)
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

            Dim ListeDrivers = myService.GetAllDrivers(IdSrv)
            CntDriver.Content = ListeDrivers.Count & " Driver(s)"

            For Each Drv In ListeDrivers
                Dim newchild As New TreeViewItem
                Dim stack As New StackPanel
                stack.Orientation = Orientation.Horizontal
                stack.HorizontalAlignment = HorizontalAlignment.Left
                Dim Graph As Object

                If Drv.IsConnect = True Then
                    Dim Rect As New Polygon
                    Dim myPointCollection As PointCollection = New PointCollection
                    myPointCollection.Add(New Point(0, 0))
                    myPointCollection.Add(New Point(10, 5))
                    myPointCollection.Add(New Point(0, 10))
                    Rect.Points = myPointCollection

                    Rect.Width = 9
                    Rect.Height = 9

                    Dim myBrush As New RadialGradientBrush()
                    myBrush.GradientStops.Add(New GradientStop(Colors.LightGreen, 0.0))
                    myBrush.GradientStops.Add(New GradientStop(Colors.Green, 0.5))
                    myBrush.GradientStops.Add(New GradientStop(Colors.DarkGreen, 1.0))

                    Rect.Fill = Brushes.DarkGreen 'myBrush
                    Rect.Tag = Drv.ID
                    Rect.ToolTip = "Arrêter le driver"
                    AddHandler Rect.MouseDown, AddressOf StopDriver
                    Graph = Rect
                Else
                    Dim Rect As New Rectangle
                    Rect.Width = 9
                    Rect.Height = 9

                    Dim myBrush As New RadialGradientBrush()
                    myBrush.GradientOrigin = New Point(0.75, 0.25)
                    myBrush.GradientStops.Add(New GradientStop(Colors.Yellow, 0.0))
                    myBrush.GradientStops.Add(New GradientStop(Colors.Red, 0.5))
                    myBrush.GradientStops.Add(New GradientStop(Colors.DarkRed, 1.0))

                    Rect.Fill = Brushes.Red 'myBrush
                    Rect.ToolTip = "Démarrer le driver"
                    Rect.Tag = Drv.ID
                    AddHandler Rect.MouseDown, AddressOf StartDriver
                    Graph = Rect
                End If

                Dim label As New Label
                If Drv.Enable = True Then
                    label.Foreground = New SolidColorBrush(Colors.White)
                Else
                    label.Foreground = New SolidColorBrush(Colors.Black)
                End If
                label.Content = Drv.Nom

                stack.Children.Add(Graph)
                stack.Children.Add(label)

                newchild.Foreground = New SolidColorBrush(Colors.White)
                newchild.Header = stack
                newchild.Uid = Drv.ID

                Dim marg As New Thickness(-12, 0, 0, 0)
                newchild.Margin = marg
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

            Dim ListeDevices = myService.GetAllDevices(IdSrv)
            Dim ListeZones = myService.GetAllZones(IdSrv)

            CntDevice.Content = ListeDevices.Count & " Device(s)"
            For Each Dev In ListeDevices
                Dim tool As String = ""
                Dim newchild As New TreeViewItem
                Dim stack As New StackPanel
                Dim img As New Image
                Dim img2 As New Image
                Dim uri As String = ""
                Dim uri2 As String = MyRep & "\Images\Icones\ZoneNo_32.png"
                Dim bmpImage As New BitmapImage()
                Dim bmpImage2 As New BitmapImage()
                Dim FlagZone As Boolean = False
                Dim nomdriver As String = myService.ReturnDriverByID(IdSrv, Dev.DriverID).Nom
                stack.Orientation = Orientation.Horizontal

                For Each Zon In ListeZones
                    If FlagZone = False Then
                        For Each elemnt In Zon.ListElement
                            If elemnt.ElementID = Dev.ID Then
                                FlagZone = True
                                Exit For
                            End If
                        Next
                    End If
                Next

                img.Height = 20
                img.Width = 20
                img2.Height = 20
                img2.Width = 20

                If Trim(Dev.Picture) <> "" Then
                    img.Source = ConvertArrayToImage(myService.GetByteFromImage(Dev.Picture))
                Else
                    uri = MyRep & "\Images\Icones\Composant_32.png"
                    bmpImage.BeginInit()
                    bmpImage.UriSource = New Uri(uri, UriKind.Absolute)
                    bmpImage.EndInit()
                    img.Source = bmpImage
                End If

                Dim drv As String = Dev.Name
                drv &= " (" & nomdriver & ")"

                tool = "Nom: " & Dev.Name & vbCrLf
                tool &= "Enable " & Dev.Enable & vbCrLf
                tool &= "Description: " & Dev.Description & vbCrLf
                tool &= "Type: " & Dev.Type.ToString & vbCrLf
                tool &= "Driver: " & nomdriver & vbCrLf
                tool &= "Value: " & Dev.Value

                Dim tl As New ToolTip
                tl.Content = tool
                Dim label As New Label
                If Dev.Enable = True Then
                    label.Foreground = New SolidColorBrush(Colors.White)
                Else
                    label.Foreground = New SolidColorBrush(Colors.Black)
                End If
                label.Content = drv
                label.ToolTip = tl

                stack.Children.Add(img)

                If FlagZone = False Then
                    bmpImage2.BeginInit()
                    bmpImage2.UriSource = New Uri(uri2, UriKind.Absolute)
                    bmpImage2.EndInit()
                    img2.Source = bmpImage2
                    img2.ToolTip = "Ce composant ne fait pas partie d'une zone"
                    stack.Children.Add(img2)
                End If

                stack.Children.Add(label)

                newchild.Foreground = New SolidColorBrush(Colors.White)
                newchild.Header = stack
                newchild.Uid = Dev.ID
                Dim marg As New Thickness(-12, 0, 0, 0)
                newchild.Margin = marg
                TreeViewDevice.Items.Add(newchild)
            Next
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub AffDevice: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Afficher la liste des scenes
    Public Sub AffScene()
        Try
            TreeViewMacro.Items.Clear()
            If IsConnect = False Then Exit Sub

            Dim ListeMacros = myService.GetAllMacros(IdSrv)
            CntMacro.Content = ListeMacros.Count & " Macro(s)"

            For Each Mac In ListeMacros
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

                uri = MyRep & "\Images\Icones\Macro_32.png"
                bmpImage.BeginInit()
                bmpImage.UriSource = New Uri(uri, UriKind.Absolute)
                bmpImage.EndInit()
                img.Source = bmpImage

                stack.Children.Add(img)
                stack.Children.Add(label)

                newchild.Foreground = New SolidColorBrush(Colors.White)
                newchild.Header = stack
                newchild.Uid = Mac.ID
                Dim marg As New Thickness(-12, 0, 0, 0)
                newchild.Margin = marg
                TreeViewMacro.Items.Add(newchild)
            Next

        Catch ex As Exception
            MessageBox.Show("ERREUR Sub AffScene: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Afficher la liste des triggers
    Public Sub AffTrigger()
        Try
            TreeViewTrigger.Items.Clear()
            If IsConnect = False Then Exit Sub

            Dim ListeTriggers = myService.GetAllTriggers(IdSrv)
            CntTrigger.Content = ListeTriggers.Count & " Trigger(s)"

            For Each Trig In ListeTriggers
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

                If Trig.Type = 0 Then
                    uri = MyRep & "\Images\Icones\Trigger_clock_32.png"
                Else
                    uri = MyRep & "\Images\Icones\Trigger_device_32.png"
                End If

                bmpImage.BeginInit()
                bmpImage.UriSource = New Uri(uri, UriKind.Absolute)
                bmpImage.EndInit()
                img.Source = bmpImage

                stack.Children.Add(img)
                stack.Children.Add(label)

                newchild.Foreground = New SolidColorBrush(Colors.White)
                newchild.Header = stack
                newchild.Uid = Trig.ID
                Dim marg As New Thickness(-12, 0, 0, 0)
                newchild.Margin = marg
                TreeViewTrigger.Items.Add(newchild)
            Next

        Catch ex As Exception
            MessageBox.Show("ERREUR Sub Afftrigger: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    'Afficher la liste des historisations
    Public Sub AffHisto()
        Try
            TreeViewHisto.Items.Clear()
            Dim x As New List(Of HoMIDom.HoMIDom.Historisation)
            x = myService.GetAllListHisto(IdSrv)

            If x IsNot Nothing Then
                For i As Integer = 0 To x.Count - 1
                    Dim y As New CheckBox
                    Dim a As Historisation = x.Item(i)
                    Dim b As String = myService.ReturnDeviceByID(IdSrv, a.IdDevice).Name
                    If b = "" Then b = "?"
                    If a.Nom = "Value" Then
                        y.Content = b
                    Else
                        y.Content = b & ": " & a.Nom
                    End If
                    y.Tag = a.Nom
                    y.Uid = a.IdDevice
                    y.Foreground = New SolidColorBrush(Colors.White)
                    y.Background = New SolidColorBrush(Colors.DarkGray)
                    y.BorderBrush = New SolidColorBrush(Colors.Black)
                    y.Margin = New Thickness(-15, 1, 0, 0)
                    TreeViewHisto.Items.Add(y)
                Next
            End If
        Catch ex As Exception

        End Try
    End Sub

    'Double Clic sur le treeview
    Private Sub TreeView_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles TreeViewDriver.MouseDoubleClick, TreeViewDevice.MouseDoubleClick, TreeViewZone.MouseDoubleClick, TreeViewUser.MouseDoubleClick, TreeViewTrigger.MouseDoubleClick, TreeViewMacro.MouseDoubleClick, TreeViewHisto.MouseDoubleClick
        Try
            If IsConnect = False Then
                Serveur_notconnected_action()
                Exit Sub
            End If

            If sender.SelectedItem IsNot Nothing Then
                If sender.SelectedItem.uid Is Nothing Then Exit Sub

                Me.Cursor = Cursors.Wait
                Select Case sender.Name
                    Case "TreeViewDriver"  'driver
                        Dim x As New uDriver(sender.SelectedItem.uid)
                        AddHandler x.CloseMe, AddressOf UnloadControl
                        AddHandler x.Loaded, AddressOf ControlLoaded
                        AffControlPage(x)
                    Case "TreeViewDevice"  'device
                        Dim x As New uDevice(Classe.EAction.Modifier, sender.SelectedItem.uid)
                        AddHandler x.CloseMe, AddressOf UnloadControl
                        AddHandler x.Loaded, AddressOf ControlLoaded
                        AffControlPage(x)
                    Case "TreeViewZone"  'zone
                        Dim x As New uZone(Classe.EAction.Modifier, sender.SelectedItem.uid)
                        AddHandler x.CloseMe, AddressOf UnloadControl
                        AddHandler x.Loaded, AddressOf ControlLoaded
                        AffControlPage(x)
                    Case "TreeViewUser"  'user
                        Dim x As New uUser(Classe.EAction.Modifier, sender.SelectedItem.uid)
                        AddHandler x.CloseMe, AddressOf UnloadControl
                        AddHandler x.Loaded, AddressOf ControlLoaded
                        AffControlPage(x)
                    Case "TreeViewTrigger"  'trigger
                        Dim _Trig As Trigger = myService.ReturnTriggerById(IdSrv, sender.SelectedItem.uid)

                        If _Trig IsNot Nothing Then
                            If _Trig.Type = Trigger.TypeTrigger.TIMER Then
                                Dim x As New uTriggerTimer(Classe.EAction.Modifier, sender.SelectedItem.uid)
                                AddHandler x.CloseMe, AddressOf UnloadControl
                                AddHandler x.Loaded, AddressOf ControlLoaded
                                AffControlPage(x)
                            Else
                                Dim x As New uTriggerDevice(Classe.EAction.Modifier, sender.SelectedItem.uid)
                                AddHandler x.CloseMe, AddressOf UnloadControl
                                AddHandler x.Loaded, AddressOf ControlLoaded
                                AffControlPage(x)
                            End If
                            _Trig = Nothing
                        End If
                    Case "TreeViewMacro"  'macros
                        Dim x As New uMacro(Classe.EAction.Modifier, sender.SelectedItem.uid)
                        AddHandler x.CloseMe, AddressOf UnloadControl
                        AddHandler x.Loaded, AddressOf ControlLoaded
                        AffControlPage(x)
                    Case 6 'histo

                End Select

            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub TreeView_MouseDoubleClick: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub TreeView_SelectedItemChanged(ByVal sender As System.Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Object)) Handles TreeViewDriver.SelectedItemChanged, TreeViewDevice.SelectedItemChanged, TreeViewZone.SelectedItemChanged, TreeViewUser.SelectedItemChanged, TreeViewTrigger.SelectedItemChanged, TreeViewMacro.SelectedItemChanged, TreeViewHisto.SelectedItemChanged
        Try
            If IsConnect = False Then
                Serveur_notconnected_action()
                Exit Sub
            End If

            If Mouse.LeftButton = MouseButtonState.Pressed Then
                If sender.SelectedItem IsNot Nothing Then
                    Dim effects As DragDropEffects
                    Dim obj As New DataObject()
                    obj.SetData(GetType(String), sender.SelectedItem.uid)
                    effects = DragDrop.DoDragDrop(sender, obj, DragDropEffects.Copy Or DragDropEffects.Move)
                End If
            End If

            If sender.SelectedItem IsNot Nothing Then
                If sender.SelectedItem.uid Is Nothing Then Exit Sub

                'Me.Cursor = Cursors.Wait
                'Select Case sender.Name
                '    Case "TreeViewDriver"  'driver
                '    Case "TreeViewDevice"  'device
                '    Case "TreeViewZone"  'zone
                '    Case "TreeViewUser"  'user
                '    Case "TreeViewTrigger"  'trigger
                '    Case "TreeViewMacro"  'macros
                '    Case "TreeViewHisto"  'histo
                'End Select
                'Me.Cursor = Nothing
            End If

        Catch ex As Exception
            MessageBox.Show("ERREUR Sub TreeViewG_SelectedItemChanged: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

#End Region

#Region "Drivers"

    Private Sub StopDriver(ByVal sender As Object, ByVal e As Input.MouseEventArgs)
        Try
            If IsConnect = False Then
                Serveur_notconnected_action()
                Exit Sub
            End If

            If sender.tag = "DE96B466-2540-11E0-A321-65D7DFD72085" Then
                MessageBox.Show("Vous ne pouvez pas arrêter ce driver", "Information", MessageBoxButton.OK, MessageBoxImage.Asterisk)
                Exit Sub
            End If

            myService.StopDriver(IdSrv, sender.tag)
            AffDriver()
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub StopDriver: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub StartDriver(ByVal sender As Object, ByVal e As Input.MouseEventArgs)
        Try
            If IsConnect = False Then
                Serveur_notconnected_action()
                Exit Sub
            End If

            If myService.ReturnDriverByID(IdSrv, sender.tag).Enable = True Then
                myService.StartDriver(IdSrv, sender.tag)
                AffDriver()
            Else
                MessageBox.Show("Le driver ne peut être démarré car sa propriété Enable est à False!", "Avertissement", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub BtnStart_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

#End Region

#Region "MainMenu"

    Private Sub ShowMainMenu()
        Dim MainMenu As New uMainMenu
        MainMenu.Uid = "MAINMENU"
        AddHandler MainMenu.menu_gerer, AddressOf MainMenuGerer
        AddHandler MainMenu.menu_delete, AddressOf MainMenuDelete
        AddHandler MainMenu.menu_edit, AddressOf MainMenuEdit
        AddHandler MainMenu.menu_create, AddressOf MainMenuNew
        AddHandler MainMenu.menu_autre, AddressOf MainMenuAutre
        CanvasRight.Children.Add(MainMenu)
        WMainMenu = CanvasRight.ActualWidth / 2 - (MainMenu.Width / 2)
        HMainMenu = CanvasRight.ActualHeight / 2 - (MainMenu.Height / 2)
        Canvas.SetLeft(MainMenu, WMainMenu)
        Canvas.SetTop(MainMenu, HMainMenu)
    End Sub

    Private Sub MainMenuGerer(ByVal index As String)
        Try
            If IsConnect = False Then
                Serveur_notconnected_action()
                Exit Sub
            End If

            Me.Cursor = Cursors.Wait
            Select Case index
                Case "tag_driver" : Tabcontrol1.SelectedIndex = 0
                Case "tag_composant" : Tabcontrol1.SelectedIndex = 1
                Case "tag_zone" : Tabcontrol1.SelectedIndex = 2
                Case "tag_user" : Tabcontrol1.SelectedIndex = 3
                Case "tag_trigger" : Tabcontrol1.SelectedIndex = 4
                Case "tag_macro" : Tabcontrol1.SelectedIndex = 5
            End Select
            ShowTreeView()
            Me.Cursor = Nothing
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub MainMenuGerer: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub MainMenuNew(ByVal index As String)
        Try
            _MainMenuAction = 0

            If IsConnect = False Then
                Serveur_notconnected_action()
                Exit Sub
            End If

            Select Case index
                Case "tag_composant"
                    Try
                        Tabcontrol1.SelectedIndex = 1
                        Dim x As New uDevice(Classe.EAction.Nouveau, "")
                        AddHandler x.CloseMe, AddressOf UnloadControl
                        AffControlPage(x)
                    Catch ex As Exception
                        MessageBox.Show("ERREUR Sub NewDevice: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
                    End Try
                Case "tag_zone"
                    Try
                        Tabcontrol1.SelectedIndex = 2
                        Dim x As New uZone(Classe.EAction.Nouveau, "")
                        AddHandler x.CloseMe, AddressOf UnloadControl
                        AffControlPage(x)
                    Catch ex As Exception
                        MessageBox.Show("ERREUR Sub NewZone: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
                    End Try
                Case "tag_user"
                    Try
                        Tabcontrol1.SelectedIndex = 3
                        Dim x As New uUser(Classe.EAction.Nouveau, "")
                        AddHandler x.CloseMe, AddressOf UnloadControl
                        AffControlPage(x)
                    Catch ex As Exception
                        MessageBox.Show("ERREUR Sub NewUser: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
                    End Try
                Case "tag_triggertimer"
                    Try
                        Tabcontrol1.SelectedIndex = 4
                        Dim x As New uTriggerTimer(0, "")
                        AddHandler x.CloseMe, AddressOf UnloadControl
                        AffControlPage(x)
                    Catch ex As Exception
                        MessageBox.Show("ERREUR Sub NewTriggerTime: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
                    End Try
                Case "tag_triggerdevice"
                    Try
                        Tabcontrol1.SelectedIndex = 4
                        Dim x As New uTriggerDevice(0, "")
                        AddHandler x.CloseMe, AddressOf UnloadControl
                        AffControlPage(x)
                    Catch ex As Exception
                        MessageBox.Show("ERREUR Sub NewTriggerDevice: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
                    End Try
                Case "tag_macro"
                    Try
                        Tabcontrol1.SelectedIndex = 5
                        Dim x As New uMacro(Classe.EAction.Nouveau, "")
                        AddHandler x.CloseMe, AddressOf UnloadControl
                        AffControlPage(x)
                    Catch ex As Exception
                        MessageBox.Show("ERREUR Sub NewMacro: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
                    End Try
                Case "tag_module"
                    Try
                        Dim x As New uModuleSimple()
                        AddHandler x.CloseMe, AddressOf UnloadControl
                        AffControlPage(x)
                    Catch ex As Exception
                        MessageBox.Show("ERREUR Sub NewModule: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
                    End Try
            End Select
            ShowTreeView()
            _MainMenuAction = -1
        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'exécution de MainMenuNew: " & ex.ToString, "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub MainMenuDelete(ByVal index As String)
        Try
            If IsConnect = False Then
                Serveur_notconnected_action()
                Exit Sub
            End If

            _MainMenuAction = 2
            CanvasRight.Children.Clear()

            Dim x As New uSelectElmt("Choisir {TITLE} à supprimer", index)
            Canvas.SetLeft(x, CanvasRight.ActualWidth / 2 - (x.ActualWidth / 2))
            AddHandler x.CloseMe, AddressOf UnloadSelectElmt
            CanvasRight.Children.Add(x)
        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'exécution de MainMenuDelete: " & ex.ToString, "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub MainMenuEdit(ByVal index As String)
        Try
            If IsConnect = False Then
                Serveur_notconnected_action()
                Exit Sub
            End If

            _MainMenuAction = 1
            CanvasRight.Children.Clear()

            Dim x As New uSelectElmt("Choisir {TITLE} à éditer", index)
            Canvas.SetLeft(x, CanvasRight.ActualWidth / 2 - (x.ActualWidth / 2) - 200)
            AddHandler x.CloseMe, AddressOf UnloadSelectElmt
            CanvasRight.Children.Add(x)
        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'exécution de MainMenuEdit: " & ex.ToString, "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub MainMenuAutre(ByVal index As String)
        Try
            If IsConnect = False And index <> "tag_quitter" Then
                Serveur_notconnected_action()
                Exit Sub
            End If

            Me.Cursor = Cursors.Wait
            Select Case index
                Case "tag_histo" : Tabcontrol1.SelectedIndex = 6
                Case "tag_config_log"
                    Try
                        If IsConnect = False Then
                            MessageBox.Show("Impossible d'afficher le log car le serveur n'est pas connecté !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Asterisk)
                            Exit Sub
                        End If
                        Dim x As New uLog
                        x.Uid = System.Guid.NewGuid.ToString()
                        AddHandler x.CloseMe, AddressOf UnloadControl
                        x.Width = CanvasRight.ActualWidth - 100
                        x.Height = CanvasRight.ActualHeight - 50
                        CanvasRight.Children.Clear()
                        CanvasRight.Children.Add(x)
                    Catch ex As Exception
                        MessageBox.Show("ERREUR Sub MainMenuAutre log: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
                    End Try

                Case "tag_multimedia" 'Multimedia Playlist
                    Try
                        Dim x As New uPlaylist
                        x.Uid = System.Guid.NewGuid.ToString()
                        AddHandler x.CloseMe, AddressOf UnloadControl
                        CanvasRight.Children.Clear()
                        CanvasRight.Children.Add(x)
                    Catch ex As Exception
                        MessageBox.Show("ERREUR Sub MainMenuAutre Playlist: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
                    End Try

                Case "tag_config" 'Configurer le serveur
                    Try
                        Dim x As New uConfigServer
                        x.Uid = System.Guid.NewGuid.ToString()
                        AddHandler x.CloseMe, AddressOf UnloadControl
                        CanvasRight.Children.Clear()
                        CanvasRight.Children.Add(x)
                    Catch ex As Exception
                        MessageBox.Show("ERREUR Sub MainMenuAutre config: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
                    End Try

                Case "tag_aide" 'Aide
                    Try
                        Process.Start("http://www.homidom.com/documentation-c16.html")
                    Catch ex As Exception
                        MessageBox.Show("ERREUR Sub MainMenuAutre aide: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
                    End Try

                Case "tag_quitter"
                    Try
                        If IsConnect = True And FlagChange Then

                            If MessageBox.Show("Voulez-vous enregistrer la configuration avant de quitter?", "HomIAdmin", MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.Yes Then
                                Try
                                    If IsConnect = False Then
                                        MessageBox.Show("Impossible d'enregistrer la configuration car le serveur n'est pas connecté !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Asterisk)
                                    Else
                                        Dim retour As String = myService.SaveConfig(IdSrv)
                                        If retour <> "0" Then
                                            MessageBox.Show("Erreur lors de l'enregistrement veuillez consulter le log", "HomIAdmin", MessageBoxButton.OK, MessageBoxImage.Error)
                                        Else
                                            MessageBox.Show("Enregistrement effectué", "HomIAdmin", MessageBoxButton.OK, MessageBoxImage.Information)
                                        End If
                                    End If
                                Catch ex As Exception
                                    MessageBox.Show("ERREUR Sub Quitter: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
                                End Try
                            End If
                        End If
                        Me.Close()
                        End
                    Catch ex As Exception
                        MessageBox.Show("ERREUR Sub MainMenuAutre Quitter: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
                    End Try

                Case "tag_config_exporter" 'Exporter le fichier de config
                    Try
                        ' Configure open file dialog box
                        Dim dlg As New Microsoft.Win32.SaveFileDialog()
                        dlg.FileName = "" ' Default file name
                        dlg.DefaultExt = ".xml" ' Default file extension
                        dlg.Filter = "Fichier de configuration (.xml)|*.xml" ' Filter files by extension

                        ' Show open file dialog box
                        Dim result As Boolean = dlg.ShowDialog()

                        ' Process open file dialog box results
                        If result = True Then
                            ' Open document
                            Dim filename As String = dlg.FileName
                            Dim retour As String = myService.ExportConfig(IdSrv)
                            If retour.StartsWith("ERREUR") Then
                                MessageBox.Show(retour, "Erreur export config", MessageBoxButton.OK, MessageBoxImage.Error)
                            Else
                                Dim TargetFile As StreamWriter
                                TargetFile = New StreamWriter(filename, True)
                                TargetFile.Write(retour)
                                TargetFile.Close()
                                MessageBox.Show("L'export du fichier de configuration a été effectué", "Export config", MessageBoxButton.OK, MessageBoxImage.Information)
                            End If
                        End If
                    Catch ex As Exception
                        MessageBox.Show("ERREUR Sub MainMenuAutre config_exporter: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
                    End Try

                Case "tag_config_importer" 'Importer le fichier de config
                    Try
                        ' Configure open file dialog box
                        Dim dlg As New Microsoft.Win32.OpenFileDialog()
                        dlg.FileName = "Homidom" ' Default file name
                        dlg.DefaultExt = ".xml" ' Default file extension
                        dlg.Filter = "Fichier de configuration (.xml)|*.xml" ' Filter files by extension

                        ' Show open file dialog box
                        Dim result As Boolean = dlg.ShowDialog()

                        ' Process open file dialog box results
                        If result = True Then
                            ' Open document
                            Dim filename As String = dlg.FileName

                            If MessageBox.Show("Etes vous sur que le serveur puisse accéder au fichier " & filename & " que ce soit en local ou via le réseau, sinon il ne pourra pas l'importer!", "Import Config", MessageBoxButton.OKCancel, MessageBoxImage.Question) = MessageBoxResult.Cancel Then
                                Exit Sub
                            End If

                            Dim retour As String = myService.ImportConfig(IdSrv, filename)
                            If retour <> "0" Then
                                MessageBox.Show(retour, "Erreur import config", MessageBoxButton.OK, MessageBoxImage.Error)
                            Else
                                MessageBox.Show("L'import du fichier de configuration a été effectué, l'ancien fichier a été renommé en .old, veuillez redémarrer le serveur pour prendre en compte cette nouvelle configuration", "Import config", MessageBoxButton.OK, MessageBoxImage.Information)
                            End If
                        End If
                    Catch ex As Exception
                        MessageBox.Show("ERREUR Sub MainMenuAutre config_importer: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
                    End Try

                Case "tag_config_sauvegarder"
                    Try
                        If IsConnect = False Then
                            MessageBox.Show("Impossible d'enregistrer la config car le serveur n'est pas connecté !!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Asterisk)
                            Exit Sub
                        End If

                        Dim retour As String = myService.SaveConfig(IdSrv)
                        If retour <> "0" Then
                            MessageBox.Show("Erreur lors de l'enregistrement veuillez consulter le log", "HomIAdmin", MessageBoxButton.OK, MessageBoxImage.Error)
                        Else
                            MessageBox.Show("Enregistrement effectué", "HomIAdmin", MessageBoxButton.OK, MessageBoxImage.Information)
                        End If
                    Catch ex As Exception
                        MessageBox.Show("ERREUR Sub MainMenuAutre config_sauvegarder: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
                    End Try

            End Select
            ShowTreeView()
            Me.Cursor = Nothing
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub MainMenuAutre: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

#End Region

#Region "Fenêtres et actions autres"

    Private Sub UnloadSelectElmt(ByVal Objet As Object)
        If Objet.retour = "CANCEL" Then
            CanvasRight.Children.Clear()
            ShowMainMenu()
        Else
            If IsConnect = False Then
                Serveur_notconnected_action()
                Exit Sub
            End If

            Select Case _MainMenuAction
                Case 1 'Edit
                    Try
                        If Objet.retour IsNot Nothing Then

                            Me.Cursor = Cursors.Wait
                            Select Case Objet.Type
                                Case 0 'driver
                                    Dim x As New uDriver(Objet.retour)
                                    x.Uid = System.Guid.NewGuid.ToString()
                                    AddHandler x.CloseMe, AddressOf UnloadControl
                                    CanvasRight.Children.Clear()
                                    CanvasRight.Children.Add(x)

                                    AnimationApparition(x)
                                Case 1 'device
                                    Dim x As New uDevice(Classe.EAction.Modifier, Objet.retour)
                                    x.Uid = System.Guid.NewGuid.ToString()
                                    AddHandler x.CloseMe, AddressOf UnloadControl
                                    CanvasRight.Children.Clear()
                                    CanvasRight.Children.Add(x)

                                    AnimationApparition(x)
                                Case 2 'zone
                                    Dim x As New uZone(Classe.EAction.Modifier, Objet.retour)
                                    x.Uid = System.Guid.NewGuid.ToString()
                                    AddHandler x.CloseMe, AddressOf UnloadControl
                                    CanvasRight.Children.Clear()
                                    CanvasRight.Children.Add(x)

                                    AnimationApparition(x)
                                Case 3 'user
                                    Dim x As New uUser(Classe.EAction.Modifier, Objet.retour)
                                    x.Uid = System.Guid.NewGuid.ToString()
                                    AddHandler x.CloseMe, AddressOf UnloadControl
                                    CanvasRight.Children.Clear()
                                    CanvasRight.Children.Add(x)

                                    AnimationApparition(x)

                                Case 4 'trigger
                                    Dim _Trig As Trigger = myService.ReturnTriggerById(IdSrv, Objet.retour)

                                    If _Trig IsNot Nothing Then
                                        If _Trig.Type = Trigger.TypeTrigger.TIMER Then
                                            Dim x As New uTriggerTimer(Classe.EAction.Modifier, Objet.retour)
                                            x.Uid = System.Guid.NewGuid.ToString()
                                            AddHandler x.CloseMe, AddressOf UnloadControl
                                            CanvasRight.Children.Clear()
                                            CanvasRight.Children.Add(x)

                                            AnimationApparition(x)

                                        Else
                                            Dim x As New uTriggerDevice(Classe.EAction.Modifier, Objet.retour)
                                            x.Uid = System.Guid.NewGuid.ToString()
                                            AddHandler x.CloseMe, AddressOf UnloadControl
                                            CanvasRight.Children.Clear()
                                            CanvasRight.Children.Add(x)

                                            AnimationApparition(x)
                                        End If
                                        _Trig = Nothing
                                    End If
                                Case 5 'macros

                                    Dim x As New uMacro(Classe.EAction.Modifier, Objet.retour)
                                    x.Uid = System.Guid.NewGuid.ToString()
                                    AddHandler x.CloseMe, AddressOf UnloadControl
                                    CanvasRight.Children.Clear()
                                    CanvasRight.Children.Add(x)

                                    AnimationApparition(x)
                                Case 6 'histo

                            End Select

                            ShowTreeView()

                            Me.Cursor = Nothing
                        End If
                    Catch ex As Exception
                        MessageBox.Show("ERREUR Sub TreeView_MouseDoubleClick: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
                    End Try
                Case 2 'Supprimer
                    Try
                        If Objet.retour IsNot Nothing Then
                            Me.Cursor = Cursors.Wait
                            Dim retour As Integer
                            Dim _retour As New List(Of String)
                            _retour = myService.CanDelete(IdSrv, Objet.retour)
                            'While _retour.Count = 0
                            '    Thread.Sleep(1500)
                            '    _retour = myService.CanDelete(IdSrv, Objet.retour)
                            'End While
                            'While _retour(_retour.Count - 1) <> "0"
                            '    Thread.Sleep(500)
                            '    _retour = myService.CanDelete(IdSrv, Objet.retour)
                            'End While
                            If _retour(0).StartsWith("ERREUR") Then
                                MessageBox.Show(_retour(0), "Erreur CanDelete", MessageBoxButton.OK, MessageBoxImage.Error)
                                Exit Sub
                            Else
                                If _retour(0) <> "0" Then
                                    Dim a As String = "Attention confirmez vous de supprimer cet élément car il est utilisé dans: " & vbCrLf
                                    For i As Integer = 0 To _retour.Count - 2
                                        a = a & _retour(i) & vbCrLf
                                    Next
                                    If MessageBox.Show(a, "Suppression", MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.No Then
                                        Exit Sub
                                    End If
                                End If
                            End If


                            Select Case Objet.Type
                                Case 0
                                Case 1
                                    retour = myService.DeleteDevice(IdSrv, Objet.retour)
                                    AffDevice()
                                Case 2
                                    retour = myService.DeleteZone(IdSrv, Objet.retour)
                                    AffZone()
                                Case 3
                                    retour = myService.DeleteUser(IdSrv, Objet.retour)
                                    AffUser()
                                Case 4
                                    retour = myService.DeleteTrigger(IdSrv, Objet.retour)
                                    AffTrigger()
                                Case 5
                                    retour = myService.DeleteMacro(IdSrv, Objet.retour)
                                    AffScene()
                            End Select

                            Me.Cursor = Nothing

                            If retour = -2 Then
                                MessageBox.Show("Vous ne pouvez pas supprimer cet élément!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                                Exit Sub
                            Else
                                FlagChange = True
                            End If

                        Else
                            MessageBox.Show("Veuillez sélectionner un élément à supprimer!")
                        End If
                    Catch ex As Exception
                        MessageBox.Show("ERREUR de la suppression: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
                    End Try
                    CanvasRight.Children.Clear()
                    ShowMainMenu()
            End Select

        End If
    End Sub

    Private Sub ControlLoaded()
        Me.Cursor = Nothing
    End Sub

    ''' <summary>
    ''' Affiche le usercontrol et l'anime
    ''' </summary>
    ''' <param name="Objet"></param>
    ''' <remarks></remarks>
    Private Sub AffControlPage(ByVal Objet As Object)
        Objet.Uid = System.Guid.NewGuid.ToString()
        CanvasRight.Children.Clear()
        CanvasRight.Children.Add(Objet)
        AnimationApparition(Objet)
    End Sub

    'Décharger une fenêtre suivant son Id
    Public Sub UnloadControl(ByVal MyControl As Object, Optional ByVal Cancel As Boolean = False)
        Try
            Me.Cursor = Cursors.Wait

            If Cancel = False Then RefreshTreeView()

            Dim myDoubleAnimation As DoubleAnimation = New DoubleAnimation()
            myDoubleAnimation.From = 1.0
            myDoubleAnimation.To = 0.0
            myDoubleAnimation.Duration = New Duration(TimeSpan.FromSeconds(1))
            Dim myStoryboard As Storyboard
            myStoryboard = New Storyboard()
            myStoryboard.Children.Add(myDoubleAnimation)
            AddHandler myStoryboard.Completed, AddressOf StoryBoardFinish

            Storyboard.SetTarget(myDoubleAnimation, MyControl)
            Storyboard.SetTargetProperty(myDoubleAnimation, New PropertyPath(UserControl.OpacityProperty))
            myStoryboard.Begin()

            Me.Cursor = Nothing
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub UnloadControl: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub AnimationApparition(ByVal Objet As Object)
        If Objet IsNot Nothing Then
            Dim da3 As DoubleAnimation = New DoubleAnimation
            da3.From = 0
            da3.To = 1
            da3.Duration = New Duration(TimeSpan.FromMilliseconds(800))
            Dim sc As ScaleTransform = New ScaleTransform()
            Objet.RenderTransform = sc
            sc.BeginAnimation(ScaleTransform.ScaleYProperty, da3)
        End If
    End Sub

    Private Sub BtnGenereGraph_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnGenereGraph.Click
        If IsConnect = False Then
            Serveur_notconnected_action()
            Exit Sub
        End If

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

        For i As Integer = 0 To TreeViewHisto.Items.Count - 1
            Dim chk As CheckBox = TreeViewHisto.Items(i)

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
            x.Width = CanvasRight.ActualWidth - 100
            x.Height = CanvasRight.ActualHeight - 50
            AddHandler x.CloseMe, AddressOf UnloadControl
            CanvasRight.Children.Clear()
            CanvasRight.Children.Add(x)
        Next
    End Sub

    Private Sub BtnGenereReleve_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnGenereReleve.Click
        Try
            If IsConnect = False Then
                Serveur_notconnected_action()
                Exit Sub
            End If

            Me.Cursor = Cursors.Wait

            Dim _listhisto As New List(Of Historisation)
            Dim _Two As Boolean = False
            Dim lbl As String = ""

            For i As Integer = 0 To TreeViewHisto.Items.Count - 1
                Dim chk As CheckBox = TreeViewHisto.Items(i)

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
            Me.Cursor = Nothing
        Catch ex As Exception
            MessageBox.Show("Erreur lors de la génération du relevé: " & ex.ToString, "Erreur Admin", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub Ellipse1_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Ellipse1.MouseDown
        If IsConnect = False Then PageConnexion()
    End Sub

    Private Sub Tabcontrol1_SelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles Tabcontrol1.SelectionChanged
        RefreshTreeView()
    End Sub

    Private Sub LOG_PreviewMouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles LOG.PreviewMouseMove, ImgLog.PreviewMouseMove
        If IsConnect = True Then
            Dim list As List(Of String) = myService.Get4Log
            Dim a As String = ""
            For i As Integer = 0 To list.Count - 1
                a &= list(i)
                If (i <> list.Count - 1) Then a &= vbCrLf
            Next
            LOG.ToolTip = a
        End If
    End Sub

    Private Sub Log_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ImgLog.MouseDown, LOG.MouseDown, LOG.PreviewMouseDown, ImgLog.PreviewMouseDown
        'RaiseEvent ChangeMenu(sender.tag)
    End Sub

#End Region

End Class