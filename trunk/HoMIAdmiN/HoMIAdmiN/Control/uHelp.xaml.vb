Imports System.Data
Imports System.IO
Imports System.Collections.ObjectModel
Imports HoMIDom.HoMIDom

Partial Public Class uHelp
    Public Event CloseMe(ByVal MyObject As Object)

    Public Sub New()
        Try

            ' Cet appel est requis par le Concepteur Windows Form.
            InitializeComponent()

            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            Ttitre.Content = My.Application.Info.Title
            TVersion.Text = "Version:" & My.Application.Info.Version.ToString & " (" & My.Application.Info.Copyright & ")"
            Texte.Text = "HoMIDoM est le logiciel complet entiérement gratuit de gestion de système domotique multi-technologies pour la maison sous Microsoft Windows."
            Texte.Text &= "C'est un projet Open-Source (libre) proposé gratuitement à toute la communauté sous licence GNU GPL v2 !"

            'affiche la liste des drivers
            listesversionsdrivers.Text = ""
            Dim ListeDrivers = myService.GetAllDrivers(IdSrv)
            For Each Drv As TemplateDriver In ListeDrivers
                listesversionsdrivers.Text = listesversionsdrivers.Text & Drv.Nom & " : " & Drv.Version & vbCrLf
            Next

            'affiche les programmes
            listesversionsprogrammes.Text = ""
            listesversionsprogrammes.Text &= " HoMIAdmiN : " & My.Application.Info.Version.ToString & vbCrLf
            listesversionsprogrammes.Text &= " HoMIDomService : " & myService.GetServerVersion() & vbCrLf
            listesversionsprogrammes.Text &= " Service démarré : " & myService.GetLastStartTime & vbCrLf
            listesversionsprogrammes.Text &= " Heure du serveur : " & myService.GetTime & vbCrLf
            listesversionsprogrammes.Text &= " Port SOAP utilisé : " & myService.GetPortSOAP & vbCrLf
            listesversionsprogrammes.Text &= " Version Moteur SQLlite: " & myService.GetSqliteVersion & vbCrLf
            listesversionsprogrammes.Text &= " Version de la BDD: " & myService.GetSqliteBddVersion & vbCrLf
            listesversionsprogrammes.Text &= " Version du frameWork: " & System.Runtime.InteropServices.RuntimeEnvironment.GetSystemVersion() & vbCrLf
            If System.Environment.Is64BitOperatingSystem = True Then
                listesversionsprogrammes.Text &= " Version de l'OS: " & My.Computer.Info.OSFullName.ToString & " 64 Bits" & vbCrLf
            Else
                listesversionsprogrammes.Text &= " Version de l'OS: " & My.Computer.Info.OSFullName.ToString & " 32 Bits" & vbCrLf
            End If
            listesversionsprogrammes.Text &= " Répertoire utilisé par l'Admin: " & My.Application.Info.DirectoryPath.ToString & vbCrLf
            listesversionsprogrammes.Text &= " Répertoire utilisé par le serveur: " & myService.GetRepertoireOfServer & vbCrLf

            'affiche des infos sur la config
            listesdivers.Text = ""
            listesdivers.Text &= "Nb Composants : " & myService.GetAllDevices(IdSrv).Count() & vbCrLf
            listesdivers.Text &= "Nb Zones : " & myService.GetAllZones(IdSrv).Count() & vbCrLf
            listesdivers.Text &= "Nb Macros : " & myService.GetAllMacros(IdSrv).Count() & vbCrLf
            listesdivers.Text &= "Nb Triggers : " & myService.GetAllTriggers(IdSrv).Count() & vbCrLf

            TexteSoutien.Text = "Pourquoi faire un don ?"
            TexteSoutien.Text &= "Le projet HoMIDoM est un projet géré par plusieurs personnes volontaires et bénévoles." & vbCrLf
            TexteSoutien.Text &= "Le but étant de proposer au plus grand nombre une véritable solution de domotique gratuite." & vbCrLf
            TexteSoutien.Text &= "Pour pouvoir fonctionner et promouvoir notre solution, nous devons investir dans différentes ressources : matériels (achats personnels aux frais de chacun) mais aussi par exemple un hébergement et un nom de domaine." & vbCrLf
            TexteSoutien.Text &= "Cette dernière partie a un coût de quelques dizaines d'euros par an." & vbCrLf
            TexteSoutien.Text &= "Si vous souhaitez soutenir notre projet pour nous aider à le développer encore plus et à le promouvoir, ou tout simplement nous remercier du travail accompli qui, nous l'espérons, vous rendra la domotique plus facile et accessible, pensez à faire un don, aussi petit soit-il." & vbCrLf
            TexteSoutien.Text &= "Combien donner ?" & vbCrLf
            TexteSoutien.Text &= "Ce que vous souhaitez, le but des dons n'étant pas de devenir millionnaire :) mais de pouvoir uniquement réduire voir rembourser les frais inhérents au fonctionnement du projet." & vbCrLf
            TexteSoutien.Text &= "Comment ?" & vbCrLf
            TexteSoutien.Text &= "Vous pouvez faire un don simplement et librement via Paypal depuis notre site web ci-dessous : " & vbCrLf

        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur lors sur la fonction New de uHelp: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me)
    End Sub
    Private Sub BtnAideEnLigne_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnAideEnLigne.Click
        Process.Start("http://www.homidom.com/documentation-c16.html")
    End Sub
    Private Sub BtnAideLocale_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnAideLocale.Click
        Process.Start("explorer.exe", "Help")
    End Sub
    Private Sub BtnAideForum_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnAideForum.Click
        Process.Start("http://www.homidom.com/le-forum-c24.html")
    End Sub
    Private Sub BtnSoutien_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnSoutien.Click
        Process.Start("http://www.homidom.com/Dons-c29.html")
    End Sub

End Class
