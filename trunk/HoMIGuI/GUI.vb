Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.ServiceProcess
Imports Microsoft.Win32
Imports System.Threading
Imports System.Globalization
Imports System.IO

Imports HoMIDom.HoMIDom
Imports System.ServiceModel
Imports System.Xml
Imports System.Net
Imports System.Windows.Threading

Public Class HoMIGuI
    Private controller As New ServiceController

    Dim myfile As String = My.Application.Info.DirectoryPath & "\Config\Homidom.xml"
    Dim myChannelFactory As ServiceModel.ChannelFactory(Of HoMIDom.HoMIDom.IHoMIDom) = Nothing
    Dim myService As HoMIDom.HoMIDom.IHoMIDom
    Dim IsConnect As Boolean = False 'True si connecté au serveur en soap
    Dim _IPSOAP, _PortSOAP, _IdSrv, _PortSrvWeb, _EnableSrvWeb As String
    Dim errorloop As Integer = 0 'utilisé pour vérifier si on entre pas dans une boucle dans getdrivers/timertick
    Dim Serviceinstalled As Boolean = False
    Dim ServiceRunning As Boolean = False 'true si le process homiservice tourne
    Dim TimerConnexionSOAP As DispatcherTimer = New DispatcherTimer()


    'Chargement du GUI et ContextMenu
    Private Sub HoMIGuI_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Me.Cursor = Cursors.WaitCursor

        'creation de l'objet service
        Try
            ServiceEtatToolStripMenuItem.Enabled = False
            ServiceStartToolStripMenuItem.Enabled = False
            ServiceStopToolStripMenuItem.Enabled = False
            ServiceRestartToolStripMenuItem.Enabled = False
            ServiceEtatToolStripMenuItem.ToolTipText = "Etat du service Windows"
            ServiceStartToolStripMenuItem.ToolTipText = "Démarrer le service Windows"
            ServiceStopToolStripMenuItem.ToolTipText = "Arrêter le service Windows"
            ServiceRestartToolStripMenuItem.ToolTipText = "Redémarrer le service Windows"
            controller.ServiceName = "HoMIServicE"
            controller.MachineName = "."
        Catch ex As Exception
            ServiceEtatToolStripMenuItem.ToolTipText = "Non disponible : Homiservice n'est pas installé en tant que Service Windows"
            ServiceStartToolStripMenuItem.ToolTipText = "Non disponible : Homiservice n'est pas installé en tant que Service Windows"
            ServiceStopToolStripMenuItem.ToolTipText = "Non disponible : Homiservice n'est pas installé en tant que Service Windows"
            ServiceRestartToolStripMenuItem.ToolTipText = "Non disponible : Homiservice n'est pas installé en tant que Service Windows"
        End Try

        'Lecture des settings dans la config du serveur
        Try
            Me.Cursor = Cursors.WaitCursor
            If System.IO.File.Exists(myfile) Then
                Dim doc As New XmlDocument
                Dim List As XmlNodeList
                doc.Load(myfile)
                'execute et récupère la valeur de la requête
                List = doc.SelectNodes("/homidom/server")
                If List.Count > 0 Then 'présence des paramètres du server
                    For j As Integer = 0 To List.Item(0).Attributes.Count - 1
                        Select Case List.Item(0).Attributes.Item(j).Name
                            Case "ipsoap" : _IPSOAP = List.Item(0).Attributes.Item(j).Value
                            Case "portsoap" : _PortSOAP = List.Item(0).Attributes.Item(j).Value
                            Case "idsrv" : _IdSrv = List.Item(0).Attributes.Item(j).Value
                            Case "portweb" : _PortSrvWeb = List.Item(0).Attributes.Item(j).Value
                            Case "enablesrvweb" : _EnableSrvWeb = List.Item(0).Attributes.Item(j).Value
                        End Select
                    Next
                    List = Nothing
                    doc = Nothing
                Else
                    List = Nothing
                    doc = Nothing
                    Me.Cursor = Cursors.Default
                    MessageBox.Show("Il manque les paramètres du serveur dans le fichier Homidom.xml --> EXIT", "HoMIGuI - Load", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    Application.Exit()
                End If
            Else
                Me.Cursor = Cursors.Default
                MessageBox.Show("Impossible de trouver le fichier de configuration du service : " & myfile & " --> EXIT", "HoMIGuI - Load", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Application.Exit()
            End If
        Catch ex As Exception
            Me.Cursor = Cursors.Default
            MessageBox.Show("Erreur lors de la lecture de configuration dans homidom.xml --> EXIT", "HoMIGuI - Load", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Application.Exit()
        End Try

        ''Liste des drivers Installés
        'Dim Chemin As String = ""
        'Try
        '    Chemin = My.Application.Info.DirectoryPath & "\Drivers"
        '    If Directory.Exists(Chemin) Then
        '        Dim fichiersTrouvé = Directory.GetFiles(Chemin, "Driver_*.dll", SearchOption.TopDirectoryOnly)
        '        For Each DriverFichier In fichiersTrouvé
        '            DriversToolStripMenuItem.DropDownItems.Add(Mid(System.IO.Path.GetFileName(DriverFichier), 8, Len(System.IO.Path.GetFileName(DriverFichier)) - 11))
        '        Next
        '    Else
        '        MsgBox("Chemin non trouvé : " & Chemin, MsgBoxStyle.Information, "MLoad: Liste des Drivers Installés")
        '    End If
        'Catch ex As Exception
        '    MsgBox("Error While opening : " & Chemin, MsgBoxStyle.Critical, "ERROR")
        'End Try

        'Connexion au serveur SOAP
        Try
            AddHandler TimerConnexionSOAP.Tick, AddressOf TimerConnexionSOAP_Tick
            TimerConnexionSOAP.Interval = New TimeSpan(0, 0, 5)
            TimerConnexionSOAP.Start()
        Catch ex As Exception
            MessageBox.Show("Erreur lors du lancement du Timer ConnexionSOAP", "HoMIGuI - Load", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try

    End Sub
    Private Sub ContextMenuStrip_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles homiguiContextMenuStrip.Opening
        'Récupere l'etat du service
        Try
            controller.Refresh()
            ServiceEtatToolStripMenuItem.Text = "Service : " & controller.Status.ToString
            Serviceinstalled = True
        Catch ex As Exception
            Serviceinstalled = False
        End Try

        'Check si le service Windows ou Console Répond
        Try
            ServiceRunning = False
            For Each clsProcess As Process In Process.GetProcesses()
                If clsProcess.ProcessName.ToUpper = "HOMISERVICE" Then
                    ServiceRunning = True
                    Exit For
                End If
            Next
            If ServiceRunning Then
                ServiceConsoleToolStripMenuItem.Enabled = False
                ServiceConsoleToolStripMenuItem.ToolTipText = "Non disponible : HoMIService est déja démarré"
            Else
                ServiceConsoleToolStripMenuItem.Enabled = True
                ServiceConsoleToolStripMenuItem.ToolTipText = "Démarrer le service en mode Console"
            End If

        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'affichage du menu CONSOLE :" & vbCrLf & ex.ToString, "HoMIGuI - ConTextMenu Opening", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try

        'Affiche les bons menus suivant l'etat du service
        Try
            If Serviceinstalled Then
                If (ServiceRunning And (controller.Status.Equals(ServiceControllerStatus.Stopped) Or controller.Status.Equals(ServiceControllerStatus.Paused))) Then
                    'le service tourne en mode console : servicewindows STOP but process tourne
                    ServiceStartToolStripMenuItem.Visible = False
                    ServiceStartToolStripMenuItem.Enabled = False
                    ServiceStopToolStripMenuItem.Visible = False
                    ServiceStopToolStripMenuItem.Enabled = False
                    ServiceRestartToolStripMenuItem.Visible = False
                    ServiceRestartToolStripMenuItem.Enabled = False
                    ServiceEtatToolStripMenuItem.ToolTipText = "Etat du service Windows"
                    ServiceEtatToolStripMenuItem.Text = "Service: Lancé en mode console"
                Else
                    If controller.Status.Equals(ServiceControllerStatus.Running) Then
                        ServiceStartToolStripMenuItem.Visible = False
                        ServiceStartToolStripMenuItem.Enabled = False
                        ServiceStopToolStripMenuItem.Visible = True
                        ServiceStopToolStripMenuItem.Enabled = True
                        ServiceRestartToolStripMenuItem.Visible = True
                        ServiceRestartToolStripMenuItem.Enabled = True
                    ElseIf controller.Status.Equals(ServiceControllerStatus.Stopped) Then
                        ServiceStartToolStripMenuItem.Visible = True
                        ServiceStartToolStripMenuItem.Enabled = True
                        ServiceStopToolStripMenuItem.Visible = False
                        ServiceStopToolStripMenuItem.Enabled = False
                        ServiceRestartToolStripMenuItem.Visible = False
                        ServiceRestartToolStripMenuItem.Enabled = False
                    ElseIf controller.Status.Equals(ServiceControllerStatus.Paused) Then
                        ServiceStartToolStripMenuItem.Visible = True
                        ServiceStartToolStripMenuItem.Enabled = True
                        ServiceStopToolStripMenuItem.Visible = True
                        ServiceStopToolStripMenuItem.Enabled = True
                        ServiceRestartToolStripMenuItem.Visible = True
                        ServiceRestartToolStripMenuItem.Enabled = True
                    ElseIf controller.Status.Equals(ServiceControllerStatus.StopPending) Or controller.Status.Equals(ServiceControllerStatus.PausePending) Or controller.Status.Equals(ServiceControllerStatus.StartPending) Or controller.Status.Equals(ServiceControllerStatus.ContinuePending) Then
                        ServiceStartToolStripMenuItem.Visible = False
                        ServiceStartToolStripMenuItem.Enabled = False
                        ServiceStopToolStripMenuItem.Visible = False
                        ServiceStopToolStripMenuItem.Enabled = False
                        ServiceRestartToolStripMenuItem.Visible = False
                        ServiceRestartToolStripMenuItem.Enabled = False
                    Else
                        ServiceStartToolStripMenuItem.Visible = False
                        ServiceStartToolStripMenuItem.Enabled = False
                        ServiceStopToolStripMenuItem.Visible = False
                        ServiceStopToolStripMenuItem.Enabled = False
                        ServiceRestartToolStripMenuItem.Visible = False
                        ServiceRestartToolStripMenuItem.Enabled = False
                    End If
                    ServiceEtatToolStripMenuItem.ToolTipText = "Etat du service Windows"
                    ServiceEtatToolStripMenuItem.Text = "Service: " & controller.Status.ToString
                End If
                
            Else
                ServiceEtatToolStripMenuItem.Text = "Service : non installé"
                ServiceEtatToolStripMenuItem.ToolTipText = "HoMIServicE n'est pas installé en tant que service Windows"
                ServiceStartToolStripMenuItem.Visible = False
                ServiceStartToolStripMenuItem.Enabled = False
                ServiceStopToolStripMenuItem.Visible = False
                ServiceStopToolStripMenuItem.Enabled = False
                ServiceRestartToolStripMenuItem.Visible = False
                ServiceRestartToolStripMenuItem.Enabled = False
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'affichage des menus Service :" & vbCrLf & ex.ToString, "HoMIGuI - ConTextMenu Opening", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try

        'Check si connecté en SOAP pour afficher les logsLive et Drivers
        If IsConnect Then
            LogsToolStripMenuItem.Enabled = True
            LogsToolStripMenuItem.ToolTipText = "Visualiser les logs en temps réel"
            DriversToolStripMenuItem.Enabled = True
            LogsToolStripMenuItem.ToolTipText = "Afficher la liste des drivers en temps réel"
        Else
            LogsToolStripMenuItem.Enabled = False
            LogsToolStripMenuItem.ToolTipText = "Non disponible : Non connecté en SOAP au serveur"
            DriversToolStripMenuItem.Enabled = False
            DriversToolStripMenuItem.ToolTipText = "Non disponible : Non connecté en SOAP au serveur"
        End If
        getdrivers()

    End Sub

    'Gestion de la connexion SOAP
    Private Sub TimerConnexionSOAP_Tick(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Try
                If IsConnect Then myService.GetTime()
            Catch ex As Exception
                IsConnect = False
                myChannelFactory.Abort()
            End Try

            If Not IsConnect Then connexion_SOAP()

        Catch ex As Exception
            MessageBox.Show("Erreur lors du Timer ConnexionSOAP", "HoMIGuI - TimerConnexionSOAP_Tick", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            errorloop += 1
            If errorloop > 10 Then
                MessageBox.Show("Erreur lors du Timer ConnexionSOAP : BOUCLE --> EXIT", "HoMIGuI - TimerConnexionSOAP_Tick", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Application.Exit()
            End If
        End Try
    End Sub
    Private Sub connexion_SOAP()
        Try
            IsConnect = False

            Dim myadress As String = "http://" & _IPSOAP & ":" & _PortSOAP & "/service"

            'Test si le service SOAP répond sur cette URL
            Try
                Dim url As New Uri(myadress)
                Dim Request As HttpWebRequest = CType(HttpWebRequest.Create(url), System.Net.HttpWebRequest)
                Dim response As Net.HttpWebResponse = CType(Request.GetResponse(), Net.HttpWebResponse)
            Catch ex As Exception
                Me.Cursor = Cursors.Default
                Exit Sub
            End Try

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
                Exit Sub
            Else
                Try
                    myService.GetTime()
                Catch ex As Exception
                    myChannelFactory.Abort()
                    Exit Sub
                End Try

                If myService.GetIdServer(_IdSrv) = "99" Then
                    myChannelFactory.Abort()
                    MsgBox("L'ID du serveur est erroné, impossible de communiquer avec celui-ci", MsgBoxStyle.OkOnly)
                    errorloop += 1
                    If errorloop > 10 Then
                        MessageBox.Show("Erreur lors de la Connexion SOAP : BOUCLE --> EXIT", "HoMIGuI - connexion_SOAP", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        Application.Exit()
                    End If
                    Exit Sub
                End If

                ServiceConsoleToolStripMenuItem.Enabled = False
                ServiceConsoleToolStripMenuItem.ToolTipText = "Non disponible : HoMIService est déja démarré"
                IsConnect = True
            End If

            binding = Nothing
        Catch ex As Exception
            IsConnect = False
            myChannelFactory.Abort()
            MessageBox.Show("Erreur lors de la connexion au serveur SOAP" & vbCrLf & ex.ToString, "HoMIGuI - connexion_SOAP", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Exit Sub
        End Try
    End Sub

    'Gestion des menus Services CLICK
    Private Sub ServiceStartToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ServiceStartToolStripMenuItem.Click
        Try
            controller.Refresh()
            If controller.Status.Equals(ServiceControllerStatus.StopPending) Then
                MessageBox.Show("Le service est en train de s'arrêter, veuillez patienter", "HoMIGuI - ServiceStartToolStripMenuItem", MessageBoxButtons.OK, MessageBoxIcon.Information)
            ElseIf controller.Status.Equals(ServiceControllerStatus.PausePending) Then
                MessageBox.Show("Le service est en train de se mettre en pause, veuillez patienter", "HoMIGuI - ServiceStartToolStripMenuItem", MessageBoxButtons.OK, MessageBoxIcon.Information)
            ElseIf controller.Status.Equals(ServiceControllerStatus.StartPending) Then
                MessageBox.Show("Le service est en cours de démarrage, veuillez patienter", "HoMIGuI - ServiceStartToolStripMenuItem", MessageBoxButtons.OK, MessageBoxIcon.Information)
            ElseIf controller.Status.Equals(ServiceControllerStatus.Running) Then
                MessageBox.Show("Le service est déjà démarré", "HoMIGuI - ServiceStartToolStripMenuItem", MessageBoxButtons.OK, MessageBoxIcon.Information)
            ElseIf controller.Status.Equals(ServiceControllerStatus.Paused) Then
                controller.Continue()
            ElseIf controller.Status.Equals(ServiceControllerStatus.Stopped) Then
                controller.Start()
            End If
            'controller.Refresh()
        Catch ex As Exception
            MessageBox.Show("Erreur lors du démarrage du service" & vbCrLf & ex.ToString, "HoMIGuI - ServiceStartToolStripMenuItem", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try
    End Sub
    Private Sub ServiceStopToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ServiceStopToolStripMenuItem.Click
        Try
            controller.Refresh()
            If controller.Status.Equals(ServiceControllerStatus.StartPending) Then
                MessageBox.Show("Le service est en train de démarrer, veuillez patienter", "HoMIGuI - ServiceStopToolStripMenuItem", MessageBoxButtons.OK, MessageBoxIcon.Information)
            ElseIf controller.Status.Equals(ServiceControllerStatus.PausePending) Then
                MessageBox.Show("Le service est en train de se mettre en pause, veuillez patienter", "HoMIGuI - ServiceStopToolStripMenuItem", MessageBoxButtons.OK, MessageBoxIcon.Information)
            ElseIf controller.Status.Equals(ServiceControllerStatus.StopPending) Then
                MessageBox.Show("Le service est en cours d'arrêt, veuillez patienter", "HoMIGuI - ServiceStopToolStripMenuItem", MessageBoxButtons.OK, MessageBoxIcon.Information)
            ElseIf controller.Status.Equals(ServiceControllerStatus.Stopped) Then
                MessageBox.Show("Le service est déja arrêté", "HoMIGuI - ServiceStopToolStripMenuItem", MessageBoxButtons.OK, MessageBoxIcon.Information)
            ElseIf controller.Status.Equals(ServiceControllerStatus.Running) Or controller.Status.Equals(ServiceControllerStatus.Paused) Then
                controller.Stop()
            End If
            'controller.Refresh()
        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'arrêt du service" & vbCrLf & ex.ToString, "HoMIGuI - ServiceStopToolStripMenuItem", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try
    End Sub
    Private Sub ServiceRestartToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ServiceRestartToolStripMenuItem.Click
        Try
            controller.Refresh()
            If controller.Status.Equals(ServiceControllerStatus.StartPending) Then
                MessageBox.Show("Le service est en train de démarrer, veuillez patienter", "HoMIGuI - ServiceRestartToolStripMenuItem", MessageBoxButtons.OK, MessageBoxIcon.Information)
            ElseIf controller.Status.Equals(ServiceControllerStatus.PausePending) Then
                MessageBox.Show("Le service est en train de se mettre en pause, veuillez patienter", "HoMIGuI - ServiceRestartToolStripMenuItem", MessageBoxButtons.OK, MessageBoxIcon.Information)
            ElseIf controller.Status.Equals(ServiceControllerStatus.StopPending) Then
                MessageBox.Show("Le service est en train de s'arrêter, veuillez patienter", "HoMIGuI - ServiceRestartToolStripMenuItem", MessageBoxButtons.OK, MessageBoxIcon.Information)
            ElseIf controller.Status.Equals(ServiceControllerStatus.Running) Or controller.Status.Equals(ServiceControllerStatus.Paused) Then
                controller.Stop()
                controller.Refresh()
                controller.WaitForStatus(ServiceControllerStatus.Stopped)
                controller.Refresh()
                controller.Start()
            ElseIf controller.Status.Equals(ServiceControllerStatus.Stopped) Then
                controller.Start()
            End If
            'controller.Refresh()
        Catch ex As Exception
            MessageBox.Show("Erreur lors du redémarrage du service" & vbCrLf & ex.ToString, "HoMIGuI - ServiceRestartToolStripMenuItem", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try
    End Sub
    Private Sub ServiceConsoleToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ServiceConsoleToolStripMenuItem.Click
        Dim Chemin As String = ""
        Try
            Chemin = My.Application.Info.DirectoryPath & "\Homiservice.exe"
            If File.Exists(Chemin) Then
                System.Diagnostics.Process.Start(Chemin)
            Else
                MessageBox.Show("Erreur lors du démarrage du service en mode console : chemin non trouve : " & Chemin, "HoMIGuI - ServiceConsoleToolStripMenuItem", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur lors du démarrage du service en mode console" & vbCrLf & ex.ToString, "HoMIGuI - ServiceConsoleToolStripMenuItem", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try
    End Sub

    'Gestion des Dossiers CLICK
    Private Sub DossierHomidomStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DossierHomidomStripMenuItem.Click
        Dim Chemin As String = ""
        Try
            Chemin = My.Application.Info.DirectoryPath
            If Directory.Exists(Chemin) Then
                System.Diagnostics.Process.Start(Chemin)
            Else
                MessageBox.Show("Erreur lors de l'ouverture du dossier Homidom : chemin non trouve : " & Chemin, "HoMIGuI - DossierHomidomStripMenuItem", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'ouverture du dossier Homidom" & vbCrLf & ex.ToString, "HoMIGuI - DossierHomidomStripMenuItem", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try
    End Sub
    Private Sub DossierLogsStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DossierLogsStripMenuItem.Click
        Dim Chemin As String = ""
        Try
            Chemin = My.Application.Info.DirectoryPath & "\logs"
            If Directory.Exists(Chemin) Then
                System.Diagnostics.Process.Start(Chemin)
            Else
                MessageBox.Show("Erreur lors de l'ouverture du dossier des logs : chemin non trouve : " & Chemin, "HoMIGuI - DossierLogsStripMenuItem", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'ouverture du dossier des logs" & vbCrLf & ex.ToString, "HoMIGuI - DossierLogsStripMenuItem", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try
    End Sub
    Private Sub DossierConfigUtilisateurStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DossierConfigUtilisateurStripMenuItem.Click
        Dim Chemin As String = ""
        Try
            Chemin = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData)
            If Directory.Exists(Chemin) Then
                System.Diagnostics.Process.Start(Chemin)
            Else
                MessageBox.Show("Erreur lors de l'ouverture du dossier de configuration Utilisateur : chemin non trouve : " & Chemin, "HoMIGuI - DossierConfigUtilisateurStripMenuItem", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'ouverture du dossier de configuration Utilisateur" & vbCrLf & ex.ToString, "HoMIGuI - DossierConfigUtilisateurStripMenuItem", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try
    End Sub

    'Gestion des Drivers
    Private Sub getdrivers()
        Try
            If IsConnect Then
                'Recuperation des Drivers
                Dim ListeDrivers As List(Of TemplateDriver) = Nothing
                Try
                    ListeDrivers = myService.GetAllDrivers(_IdSrv)
                Catch ex As Exception
                    IsConnect = False
                    DriversToolStripMenuItem.DropDownItems.Clear()
                    DriversToolStripMenuItem.DropDownItems.Add("Non disponible : Non connecté au serveur SOAP")
                    Exit Sub
                End Try

                'Affichage de la liste des drivers
                DriversToolStripMenuItem.DropDownItems.Clear()
                For Each drv In ListeDrivers
                    Dim newdriveritem As New ToolStripMenuItem
                    newdriveritem.Text = drv.Nom
                    If (drv.Enable) Then
                        newdriveritem.ForeColor = Color.Black
                        If (drv.IsConnect) Then
                            newdriveritem.ToolTipText = "Arrêter le driver (V" & drv.Version & ")"
                            newdriveritem.Image = My.Resources.Resources.play
                            newdriveritem.Tag = drv.ID
                            AddHandler newdriveritem.Click, AddressOf Driver_Stop_Click
                        Else
                            newdriveritem.ToolTipText = "Démarrer le driver (V" & drv.Version & ")"
                            newdriveritem.Image = My.Resources.Resources.stopped
                            newdriveritem.Tag = drv.ID
                            AddHandler newdriveritem.Click, AddressOf Driver_Start_Click
                        End If
                    Else
                        newdriveritem.ForeColor = Color.Gray
                        newdriveritem.ToolTipText = "Driver désactivé: Utilisez l'administration pour le configurer (V" & drv.Version & ")"
                    End If
                    If drv.ID = "DE96B466-2540-11E0-A321-65D7DFD72085" Then
                        newdriveritem.Enabled = False
                        newdriveritem.ToolTipText = "Driver Système non modifiable (V" & drv.Version & ")"
                        RemoveHandler newdriveritem.Click, AddressOf Driver_Stop_Click
                    End If
                    DriversToolStripMenuItem.DropDownItems.Add(newdriveritem)
                Next
            Else
                DriversToolStripMenuItem.DropDownItems.Clear()
                DriversToolStripMenuItem.DropDownItems.Add("Non disponible : Non connecté au serveur SOAP")
            End If
        Catch ex As Exception
            IsConnect = False
            DriversToolStripMenuItem.DropDownItems.Add("Non disponible : Non connecté au serveur SOAP")
            MessageBox.Show("Erreur lors de la récupération des drivers en SOAP" & vbCrLf & ex.ToString, "HoMIGuI - GetDrivers", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try
    End Sub
    Private Sub DriversToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DriversToolStripMenuItem.DropDownOpening, DriversToolStripMenuItem.Click, DriversToolStripMenuItem.DoubleClick
        'getdrivers()
    End Sub
    Public Sub Driver_Stop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            If IsConnect Then
                If sender.tag = "DE96B466-2540-11E0-A321-65D7DFD72085" Then
                    MessageBox.Show("Vous ne pouvez pas manipuler ce driver systeme", "HoMIGuI - DriverStop", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If

                myService.StopDriver(_IdSrv, sender.tag)

                Dim RetourDriverStarted As Boolean = True
                Dim t As DateTime = DateTime.Now
                Do While DateTime.Now < t.AddSeconds(10) And RetourDriverStarted = True
                    RetourDriverStarted = myService.ReturnDriverByID(_IdSrv, sender.tag).IsConnect
                    Thread.Sleep(1000)
                Loop
                If RetourDriverStarted = True Then MessageBox.Show("Le driver n'a pas pu être arrêté, veuillez consulter les logs", "HoMIGuI - DriverStop", MessageBoxButtons.OK, MessageBoxIcon.Warning)

            End If
        Catch ex As Exception
            IsConnect = False
            MessageBox.Show("Erreur lors de l'arret du driver en SOAP" & vbCrLf & ex.ToString, "HoMIGuI - DriverStop", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try
    End Sub
    Public Sub Driver_Start_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            If IsConnect Then
                If sender.tag = "DE96B466-2540-11E0-A321-65D7DFD72085" Then
                    MessageBox.Show("Vous ne pouvez pas manipuler ce driver systeme", "HoMIGuI - DriverStart", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If

                myService.StartDriver(_IdSrv, sender.tag)

                Dim RetourDriverStarted As Boolean = True
                Dim t As DateTime = DateTime.Now
                Do While DateTime.Now < t.AddSeconds(10) And RetourDriverStarted = True
                    RetourDriverStarted = myService.ReturnDriverByID(_IdSrv, sender.tag).IsConnect
                    Thread.Sleep(1000)
                Loop
                If RetourDriverStarted = False Then MessageBox.Show("Le driver n'a pas pu être démarré, veuillez consulter les logs", "HoMIGuI - DriverStart", MessageBoxButtons.OK, MessageBoxIcon.Warning)

            End If
        Catch ex As Exception
            IsConnect = False
            MessageBox.Show("Erreur lors du démarrage du driver en SOAP" & vbCrLf & ex.ToString, "HoMIGuI - DriverStart", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try
    End Sub

    'Gestion des Updates
    Private Sub CheckUpdateToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckUpdateToolStripMenuItem.Click
        MessageBox.Show("Non disponible pour le moment", "HoMIGuI - CheckUpdateToolStripMenuItem", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub
    Public Sub CheckUpdate()

    End Sub

    'Gestion des autres menus
    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        AboutForm.Show()
    End Sub
    Private Sub LogsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LogsToolStripMenuItem.Click
        LogsForm.Show()
    End Sub
    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        Try
            If IsConnect Then myChannelFactory.Abort()
            Application.Exit()
        Catch ex As Exception
            MessageBox.Show("Erreur lors de la fermeture" & vbCrLf & ex.ToString, "HoMIGuI - ExitToolStripMenuItem", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try
    End Sub

End Class
