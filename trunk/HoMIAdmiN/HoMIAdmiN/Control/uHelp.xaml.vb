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
            TVersion.Content = "Version:" & My.Application.Info.Version.ToString & " (" & My.Application.Info.Copyright & ")"
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
            listesversionsdrivers.Text &= " HoMIAdmiN : " & My.Application.Info.Version.ToString & vbCrLf
            listesversionsdrivers.Text &= " HoMIDomService : " & myService.GetServerVersion() & vbCrLf
            listesversionsdrivers.Text &= " Service démarré : " & myService.GetLastStartTime & vbCrLf
            listesversionsdrivers.Text &= " Heure du serveur : " & myService.GetTime & vbCrLf
            listesversionsdrivers.Text &= " Port SOAP utilisé : " & myService.GetPortSOAP & vbCrLf

            'affiche des infos sur la config
            listesdivers.Text = ""
            listesdivers.Text &= "Nb Composants : " & myService.GetAllDevices(IdSrv).Count()
            listesdivers.Text &= "Nb Zones : " & myService.GetAllZones(IdSrv).Count()
            listesdivers.Text &= "Nb Macros : " & myService.GetAllMacros(IdSrv).Count()
            listesdivers.Text &= "Nb Triggers : " & myService.GetAllTriggers(IdSrv).Count()

        Catch ex As Exception
            MessageBox.Show("Erreur lors sur la fonction New de uHelp: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
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


End Class
