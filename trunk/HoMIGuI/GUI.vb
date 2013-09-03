Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.ServiceProcess
Imports Microsoft.Win32
Imports System.Threading
Imports System.Globalization
Imports System.IO

Public Class HoMIGuI
    Private controller As New ServiceController
    'Private controller As New ServiceController("HoMIServicE", ".")

    Private Sub HoMIGuI_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

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

        'Liste des drivers Installés
        Dim Chemin As String = ""
        Try
            Chemin = My.Application.Info.DirectoryPath & "\Drivers"
            If Directory.Exists(Chemin) Then
                Dim fichiersTrouvé = Directory.GetFiles(Chemin, "Driver_*.dll", SearchOption.TopDirectoryOnly)
                For Each DriverFichier In fichiersTrouvé
                    DriversToolStripMenuItem.DropDownItems.Add(Mid(System.IO.Path.GetFileName(DriverFichier), 8, Len(System.IO.Path.GetFileName(DriverFichier)) - 11))
                Next
            Else
                MsgBox("Chemin non trouvé : " & Chemin, MsgBoxStyle.Information, "MLoad: Liste des Drivers Installés")
            End If
        Catch ex As Exception
            MsgBox("Error While opening : " & Chemin, MsgBoxStyle.Critical, "ERROR")
        End Try

    End Sub

    Private Sub AboutToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        About.Show()
    End Sub

    Private Sub ConfigurationToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ConfigurationToolStripMenuItem.Click
        MsgBox("Non implémenté", MsgBoxStyle.Information, "Information")
    End Sub

    Private Sub ServiceStartToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ServiceStartToolStripMenuItem.Click
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

    Private Sub ServiceStopToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ServiceStopToolStripMenuItem.Click
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

    Private Sub ServiceRestartToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ServiceRestartToolStripMenuItem.Click
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

    Private Sub LogsToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles LogsToolStripMenuItem.Click
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

    Private Sub DossierHomidomStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles DossierHomidomStripMenuItem.Click
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

    Private Sub DossierLogsStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles DossierLogsStripMenuItem.Click
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

    Private Sub DossierConfigUtilisateurStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles DossierConfigUtilisateurStripMenuItem.Click
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

    Private Sub ExitToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        Application.Exit()
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

    Private Sub DriversToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles DriversToolStripMenuItem.Click
        'MsgBox("Non implémenté", MsgBoxStyle.Information, "Information")
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
End Class
