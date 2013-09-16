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

Public Class HoMIGuI
    Private controller As New ServiceController
    'Private controller As New ServiceController("HoMIServicE", ".")

    Dim myfile As String = My.Application.Info.DirectoryPath & "\Config\Homidom.xml"
    Dim myChannelFactory As ServiceModel.ChannelFactory(Of HoMIDom.HoMIDom.IHoMIDom) = Nothing
    Dim myService As HoMIDom.HoMIDom.IHoMIDom
    Dim IsConnect As Boolean = False 'True si connecté au serveur
    Dim _IPSOAP, _PortSOAP, _IdSrv, _PortSrvWeb, _EnableSrvWeb As String
    Dim errorloop As Integer = 0 'utilisé pour vérifier si on entre pas dans une boucle dans getdrivers



    Private Sub HoMIGuI_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'initialisation graphique
        ServiceEtatToolStripMenuItem.Enabled = False
        ServiceStartToolStripMenuItem.Enabled = False
        ServiceStopToolStripMenuItem.Enabled = False
        ServiceRestartToolStripMenuItem.Enabled = False

        'creation de l'objet service
        Try
            controller.ServiceName = "HoMIServicE"
            controller.MachineName = "."
            'controller.Status
            'Dim x = controller.ServiceName
        Catch ex As Exception
            MsgBox("Service HoMIServicE don't exist !", MsgBoxStyle.Critical, "ERROR")
            'Application.Exit()
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

        'Connexion au serveur SOAP et recup des listes/infos
        Try
            connexion_SOAP()
            If IsConnect Then
                ServiceConsoleToolStripMenuItem.Enabled = False
            End If
        Catch ex As Exception
            MsgBox("Error While connecting to HoMIServicE !", MsgBoxStyle.Critical, "ERROR")
        End Try

    End Sub

    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        AboutForm.Show()
    End Sub

    Private Sub ConfigurationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ConfigurationToolStripMenuItem.Click
        MsgBox("Non implémenté", MsgBoxStyle.Information, "Information")
    End Sub

    Private Sub ServiceStartToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ServiceStartToolStripMenuItem.Click
        Try
            controller.Refresh()
            If controller.Status.Equals(ServiceControllerStatus.StopPending) Or controller.Status.Equals(ServiceControllerStatus.PausePending) Then
                MsgBox("Wait that HoMIServicE to be completely stopped/paused before starting i !t")
            ElseIf controller.Status.Equals(ServiceControllerStatus.Running) Then
                MsgBox("HoMIServicE is already started !")
            ElseIf controller.Status.Equals(ServiceControllerStatus.Paused) Then
                controller.Continue()
            ElseIf controller.Status.Equals(ServiceControllerStatus.Stopped) Then
                controller.Start()
            End If
            'controller.Refresh()
        Catch ex As Exception
            MsgBox("Error while starting HoMIServicE" & Chr(10) & Chr(10) & ex.ToString, MsgBoxStyle.Critical, "Start HoMIServicE")
        End Try
    End Sub

    Private Sub ServiceStopToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ServiceStopToolStripMenuItem.Click
        Try
            controller.Refresh()
            If controller.Status.Equals(ServiceControllerStatus.StartPending) Or controller.Status.Equals(ServiceControllerStatus.PausePending) Then
                MsgBox("Wait that HoMIServicE to be completely started/paused before stoping it !")
            ElseIf controller.Status.Equals(ServiceControllerStatus.Stopped) Or controller.Status.Equals(ServiceControllerStatus.StopPending) Then
                MsgBox("HoMIServicE is already stopped/stoping !")
            ElseIf controller.Status.Equals(ServiceControllerStatus.Running) Or controller.Status.Equals(ServiceControllerStatus.Paused) Then
                controller.Stop()
            End If
            'controller.Refresh()
        Catch ex As Exception
            MsgBox("Error while stopping HoMIServicE" & Chr(10) & Chr(10) & ex.ToString, MsgBoxStyle.Critical, "Stop HoMIServicE")
        End Try
    End Sub

    Private Sub ServiceRestartToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ServiceRestartToolStripMenuItem.Click
        Try
            controller.Refresh()
            If controller.Status.Equals(ServiceControllerStatus.StartPending) Or controller.Status.Equals(ServiceControllerStatus.PausePending) Then
                MsgBox("Wait that HoMIServicE to be completely started/paused before restarting it")
            ElseIf controller.Status.Equals(ServiceControllerStatus.StopPending) Then
                MsgBox("Wait that HoMIServicE to be completely stoped before restarting it")
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
            MsgBox("Error while restarting HoMIServicE" & Chr(10) & Chr(10) & ex.ToString, MsgBoxStyle.Critical, "Restart HoMIServicE")
        End Try
    End Sub

    Private Sub LogsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LogsToolStripMenuItem.Click
        LogsForm.Show()
    End Sub

    Private Sub DossierHomidomStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DossierHomidomStripMenuItem.Click
        Dim Chemin As String = ""
        Try
            Chemin = My.Application.Info.DirectoryPath
            If Directory.Exists(Chemin) Then
                System.Diagnostics.Process.Start(Chemin)
            Else
                MsgBox("Chemin non trouvé : " & Chemin, MsgBoxStyle.Information, "Ouvrir le dossier HoMIDoM")
            End If
        Catch ex As Exception
            MsgBox("Error While opening : " & Chemin, MsgBoxStyle.Critical, "ERROR")
        End Try
    End Sub

    Private Sub DossierLogsStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DossierLogsStripMenuItem.Click
        Dim Chemin As String = ""
        Try
            Chemin = My.Application.Info.DirectoryPath & "\logs"
            If Directory.Exists(Chemin) Then
                System.Diagnostics.Process.Start(Chemin)
            Else
                MsgBox("Chemin non trouvé : " & Chemin, MsgBoxStyle.Information, "Ouvrir le dossier Logs")
            End If
        Catch ex As Exception
            MsgBox("Error While opening : " & Chemin, MsgBoxStyle.Critical, "ERROR")
        End Try
    End Sub

    Private Sub DossierConfigUtilisateurStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DossierConfigUtilisateurStripMenuItem.Click
        Dim Chemin As String = ""
        Try
            Chemin = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData)
            If Directory.Exists(Chemin) Then
                System.Diagnostics.Process.Start(Chemin)
            Else
                MsgBox("Chemin non trouvé : " & Chemin, MsgBoxStyle.Information, "Ouvrir le dossier Logs")
            End If
        Catch ex As Exception
            MsgBox("Error While opening : " & Chemin, MsgBoxStyle.Critical, "ERROR")
        End Try
    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        Try
            If IsConnect Then myChannelFactory.Abort()
            Application.Exit()
        Catch ex As Exception
            MsgBox("Erreur lors de la fermeture")
        End Try
    End Sub

    Private Sub ContextMenuStrip_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles homiguiContextMenuStrip.Opening
        Try
            controller.Refresh()
            ServiceEtatToolStripMenuItem.Text = "Service : " & controller.Status.ToString
        Catch ex As Exception
            ServiceEtatToolStripMenuItem.Text = "Service : non installé"
            ServiceStartToolStripMenuItem.Visible = False
            ServiceStartToolStripMenuItem.Enabled = False
            ServiceStopToolStripMenuItem.Visible = False
            ServiceStopToolStripMenuItem.Enabled = False
            ServiceRestartToolStripMenuItem.Visible = False
            ServiceRestartToolStripMenuItem.Enabled = False
            'MsgBox("Error : " & ex.Message, MsgBoxStyle.Critical, "ERROR")
            Exit Sub
        End Try
        Try
            'controller.Refresh()
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
        Catch ex As Exception
            MsgBox("Error : " & ex.Message, MsgBoxStyle.Critical, "ERROR")
        End Try
    End Sub

    Private Sub DriversToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DriversToolStripMenuItem.DropDownOpening, DriversToolStripMenuItem.Click, DriversToolStripMenuItem.DoubleClick
        getdrivers()
    End Sub

    Private Sub ServiceConsoleToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ServiceConsoleToolStripMenuItem.Click
        Dim Chemin As String = ""
        Try
            Chemin = My.Application.Info.DirectoryPath & "\Homiservice.exe"
            If Directory.Exists(Chemin) Then
                System.Diagnostics.Process.Start(Chemin)
            Else
                MsgBox("Chemin non trouvé : " & Chemin, MsgBoxStyle.Information, "Lancer le Service en mode console")
            End If
        Catch ex As Exception
            MsgBox("Error While opening : " & Chemin, MsgBoxStyle.Critical, "ERROR")
        End Try
    End Sub

    Private Sub connexion_SOAP()
        Try

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
                        MsgBox("Il manque les paramètres du serveur dans le fichier de config du service !!", MsgBoxStyle.OkOnly)
                        Exit Sub
                    End If
                Else
                    Me.Cursor = Cursors.Default
                    MsgBox("impossible de trouver le fichier de config du service : " & myfile, MsgBoxStyle.OkOnly)
                    Exit Sub
                End If
            Catch ex As Exception
                Me.Cursor = Cursors.Default
                MsgBox("Erreur lors de la connexion au serveur : " & Chr(10) & _IPSOAP & ":" & _PortSOAP & vbCrLf & "Veuillez vérifier que celui-ci est démarré", MsgBoxStyle.OkOnly)
                Exit Sub
            End Try

            'Connexion SOAP au serveur
            Try
                Dim myadress As String = "http://" & _IPSOAP & ":" & _PortSOAP & "/service"

                Try
                    Dim url As New Uri(myadress)
                    Dim Request As HttpWebRequest = CType(HttpWebRequest.Create(url), System.Net.HttpWebRequest)
                    Dim response As Net.HttpWebResponse = CType(Request.GetResponse(), Net.HttpWebResponse)
                Catch ex As Exception
                    Me.Cursor = Cursors.Default
                    'MsgBox("Erreur lors de la connexion au serveur : " & Chr(10) & _IPSOAP & ":" & _PortSOAP & vbCrLf & "Veuillez vérifier que celui-ci est démarré", MsgBoxStyle.OkOnly)
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
                    Me.Cursor = Cursors.Default
                    'MsgBox("Erreur lors de la connexion au serveur : " & Chr(10) & _IPSOAP & ":" & _PortSOAP & vbCrLf & "Veuillez vérifier que celui-ci est démarré", MsgBoxStyle.OkOnly)
                    Exit Sub
                Else
                    Try
                        myService.GetTime()
                    Catch ex As Exception
                        myChannelFactory.Abort()
                        IsConnect = False
                        Me.Cursor = Cursors.Default
                        'MsgBox("Erreur lors de la connexion au serveur : " & Chr(10) & _IPSOAP & ":" & _PortSOAP & vbCrLf & ex.ToString, MsgBoxStyle.OkOnly)
                        Exit Sub
                    End Try

                    If myService.GetIdServer(_IdSrv) = "99" Then
                        Me.Cursor = Cursors.Default
                        MsgBox("L'ID du serveur est erroné, impossible de communiquer avec celui-ci", MsgBoxStyle.OkOnly)
                        Exit Sub
                    End If

                    IsConnect = True
                    ServiceConsoleToolStripMenuItem.Visible = False
                End If

                binding = Nothing
            Catch ex As Exception
                myChannelFactory.Abort()
                IsConnect = False
                Me.Cursor = Cursors.Default
                MsgBox("Erreur lors de la connexion au serveur : " & ex.Message, MsgBoxStyle.OkOnly)
                Exit Sub
            End Try
        Catch ex As Exception
            Me.Cursor = Cursors.Default
            MsgBox("Erreur lors de la connexion au serveur : " & ex.Message, MsgBoxStyle.OkOnly)
            Exit Sub
        End Try
    End Sub

    'Drivers
    Private Sub getdrivers()
        Try
            Me.Cursor = Cursors.WaitCursor
            DriversToolStripMenuItem.DropDownItems.Clear()
            If Not IsConnect Then connexion_SOAP()
            If IsConnect Then
                'Recuperation des Drivers
                Dim ListeDrivers As List(Of TemplateDriver) = Nothing
                Try
                    ListeDrivers = myService.GetAllDrivers(_IdSrv)
                Catch ex As Exception
                    IsConnect = False
                    DriversToolStripMenuItem.DropDownItems.Add("Non connecté au serveur SOAP")
                    errorloop += 1
                    If errorloop > 10 Then
                        Me.Cursor = Cursors.Default
                        MsgBox("Erreur Fatale de connexion en SOAP , fermeture du Gui", MsgBoxStyle.OkOnly)
                        Application.Exit()
                    End If
                    getdrivers()
                End Try
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
                    DriversToolStripMenuItem.DropDownItems.Add(newdriveritem)
                Next
            Else
                DriversToolStripMenuItem.DropDownItems.Add("Non connecté au serveur SOAP")
            End If
            Me.Cursor = Cursors.Default
        Catch ex As Exception
            Me.Cursor = Cursors.Default
            IsConnect = False
            MsgBox("Erreur lors de la récupération des drivers en SOAP / Non connecté au Serveur : " & ex.Message, MsgBoxStyle.OkOnly)
            DriversToolStripMenuItem.DropDownItems.Add("Non connecté au serveur SOAP")
        End Try
    End Sub
    Public Sub Driver_Stop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            If IsConnect Then
                If sender.tag = "DE96B466-2540-11E0-A321-65D7DFD72085" Then
                    MsgBox("Vous ne pouvez pas manipuler ce driver systeme", MsgBoxStyle.OkOnly, "HoMIGuI - Driver Stop")
                    Exit Sub
                End If

                Me.Cursor = Cursors.WaitCursor
                myService.StopDriver(_IdSrv, sender.tag)

                Dim OK As Boolean = True
                Dim t As DateTime = DateTime.Now
                Do While DateTime.Now < t.AddSeconds(10) And OK = True
                    OK = myService.ReturnDriverByID(_IdSrv, sender.tag).IsConnect
                    Thread.Sleep(1000)
                Loop

                Me.Cursor = Nothing
                If OK = True Then
                    MsgBox("Le driver n'a pas pu être arrêté, veuillez consulter le log", MsgBoxStyle.OkOnly, "HoMIGuI - Driver Stop")
                End If
            End If
        Catch ex As Exception
            Me.Cursor = Nothing
            MsgBox("Erreur lors de l'arret du driver : " & ex.ToString, MsgBoxStyle.OkOnly, "HoMIGuI - Driver Stop")
        End Try
    End Sub
    Public Sub Driver_Start_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            If IsConnect Then
                If sender.tag = "DE96B466-2540-11E0-A321-65D7DFD72085" Then
                    MsgBox("Vous ne pouvez pas manipuler ce driver systeme", MsgBoxStyle.OkOnly, "HoMIGuI - Driver Stop")
                    Exit Sub
                End If

                Me.Cursor = Cursors.WaitCursor
                myService.StartDriver(_IdSrv, sender.tag)

                Dim OK As Boolean = True
                Dim t As DateTime = DateTime.Now
                Do While DateTime.Now < t.AddSeconds(10) And OK = True
                    OK = myService.ReturnDriverByID(_IdSrv, sender.tag).IsConnect
                    Thread.Sleep(1000)
                Loop

                Me.Cursor = Nothing
                If OK = False Then
                    MsgBox("Le driver n'a pas pu être démarré, veuillez consulter le log", MsgBoxStyle.OkOnly, "HoMIGuI - Driver Start")
                End If
            End If
        Catch ex As Exception
            Me.Cursor = Nothing
            MsgBox("Erreur lors du démarrage du driver : " & ex.ToString, MsgBoxStyle.OkOnly, "HoMIGuI - Driver Start")
        End Try
    End Sub
End Class
