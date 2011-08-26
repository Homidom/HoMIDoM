Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Threading
Imports System.Xml
Imports System.Xml.XPath
Imports System.IO

Class Window1

#Region "Data"
    ' Used when manually scrolling.
    Private scrollTarget As Point
    Private scrollStartPoint As Point
    Private scrollStartOffset As Point
    Private imgStackPnl As New StackPanel()

    'Paramètres de connexion à HomeSeer
    Dim _Serveur As String = ""
    Dim _Login As String = ""
    Dim _Password As String = ""

    'XML
    Dim myxml As clsXML
    Dim list As XmlNodeList
#End Region

    Public Sub New()
        Try
            ' Cet appel est requis par le Concepteur Windows Form.
            InitializeComponent()

            myxml = New clsXML("C:\ehome\config\wehome_config.xml")
            list = myxml.SelectNodes("/wehome/config/element")
            For i As Integer = 0 To list.Count - 1
                If list(i).Attributes.Item(0).Value = "debug" Then
                    _Debug = CBool(list(i).Attributes.Item(2).Value)
                End If
            Next

            WriteLog("Lancement de l'application")

            ' Create StackPanel and set child elements to horizontal orientation
            imgStackPnl.HorizontalAlignment = HorizontalAlignment.Left
            imgStackPnl.VerticalAlignment = VerticalAlignment.Center
            imgStackPnl.Orientation = Orientation.Horizontal

            'Creation  du menu
            NewBtnMnu("Journal", "0", "C:\ehome\images\yahoo.png")
            NewBtnMnu("Programme TV", "1", "C:\ehome\images\125_tv.png")
            NewBtnMnu("Contacts", "2", "C:\ehome\images\contact2.png")
            NewBtnMnu("Module", "3", "C:\ehome\images\125_light.png")
            'NewBtnMnu("Scene", "4", "C:\ehome\images\125_scenes.png")
            NewBtnMnu("Meteo", "13", "C:\ehome\images\125_goodmorning.png")
            'NewBtnMnu("Paramètres", "5", "C:\ehome\images\125_settings.png")
            NewBtnMnu("Recette", "6", "C:\ehome\images\Recette.png")
            NewBtnMnu("Pages Jaunes", "7", "C:\ehome\images\pages-jaunes.png")
            NewBtnMnu("Internet", "8", "C:\ehome\images\Internet.png")
            NewBtnMnu("Itinéraire", "9", "C:\ehome\images\map.png")
            NewBtnMnu("Calculatrice", "10", "C:\ehome\images\calc-.png")
            'NewBtnMnu("FaceBook", "11", "C:\ehome\images\facebook_icon.png")
            'NewBtnMnu("Horloge", "12", "C:\ehome\images\clock.png")
            'NewBtnMnu("Calendrier", "14", "C:\ehome\images\calendar02.png")
            NewBtnMnu("Note", "15", "C:\ehome\images\calendar.png")
            NewBtnMnu("Traffic", "16", "C:\ehome\images\traffic.png")

            'Mise en forme du scrollviewer
            ScrollViewer1.Content = imgStackPnl

            'Timer pour afficher la date & heure et levé/couché soleil
            Dim dt As DispatcherTimer = New DispatcherTimer()
            AddHandler dt.Tick, AddressOf dispatcherTimer_Tick
            dt.Interval = New TimeSpan(0, 0, 1)
            dt.Start()

            ''Connexion à HomeSeer
            If My.Computer.Network.IsAvailable = True Then
                Try
                    myxml = New clsXML("C:\ehome\config\wehome_config.xml")
                    list = myxml.SelectNodes("/wehome/connect/element")
                    For i As Integer = 0 To list.Count - 1
                        _Serveur = list(i).Attributes.Item(0).Value
                        _Login = list(i).Attributes.Item(1).Value
                        _Password = list(i).Attributes.Item(2).Value
                    Next
                Catch ex As Exception
                    WriteLog("Erreur lors du chargement des paramètres de connexion: " & ex.Message)
                End Try

                Dim hsapp As HomeSeer2.application = New HomeSeer2.application
                hsapp.SetHost(_Serveur) '("seb-serveur-002:81")
                Dim rval As String = hsapp.Connect(_Login, _Password) '("sebastien", "clarisse1705")
                If rval <> "" Then
                    WriteLog("State: Unable to connect to HomeSeer, is it running? You need to be on the same subnet as HomeSeer")
                    rval = hsapp.Connect(_Login, _Password) '("sebastien", "clarisse1705")
                    If rval <> "" Then
                        IsHSConnect = False
                        rval = hsapp.Connect(_Login, _Password) '("sebastien", "clarisse1705")
                        If rval <> "" Then
                            IsHSConnect = False
                        Else
                            IsHSConnect = True
                            hs = hsapp.GetHSRef
                        End If
                    Else
                        IsHSConnect = True
                        hs = hsapp.GetHSRef
                    End If
                Else
                    WriteLog("State: Connect to HomeSeer")
                    IsHSConnect = True
                    hs = hsapp.GetHSRef
                End If
            Else
                IsHSConnect = False
            End If

            If IsHSConnect = True Then
                LblLeve.Content = Mid(hs.Sunrise, 1, 5)
                LblCouche.Content = Mid(hs.Sunset, 1, 5)
            End If

            myxml = Nothing

        Catch ex As Exception
            MessageBox.Show("Erreur: " & ex.Message)
            WriteLog("Erreur: " & ex.Message)
        End Try
    End Sub

    'Creation  du menu
    Private Sub NewBtnMnu(ByVal Label As String, ByVal Name As String, ByVal Icon As String)
        Try
            Dim ctrl As New uCtrlImgMnu
            ctrl.Text = Label
            ctrl.Tag = Name
            ctrl.Icon = Icon
            AddHandler ctrl.click, AddressOf IconMnuDoubleClick
            imgStackPnl.Children.Add(ctrl)
        Catch ex As Exception
            WriteLog("Erreur NewBtnMnu: " & ex.Message)
        End Try
    End Sub

    'Affiche la date et heure, heures levé et couché du soleil
    Public Sub dispatcherTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        Try
            'ConnectToHomeSeer()
            LblTime.Content = Now.ToLongDateString & " " & Now.ToShortTimeString
            If IsHSConnect = True Then
                LblLeve.Content = Mid(hs.Sunrise, 1, 5)
                LblCouche.Content = Mid(hs.Sunset, 1, 5)
                LblTemp.Content = "INT: " & hs.DeviceString("T3") & "°C / EXT: " & hs.DeviceString("T2") & "°C"
            Else
                LblTemp.Content = "?"
            End If
        Catch ex As Exception
            WriteLog("DispatcherTimer: " & ex.Message)
        End Try
    End Sub

    'Clic sur un menu de la barre du bas
    Private Sub IconMnuDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Try
            Me.Cursor = Cursors.Wait
            If Canvas1.Children.Count > 0 And sender.tag <> 10 Then
                Canvas1.Children.Clear()
            End If

            Select Case sender.tag

                Case 0 'Actualités
                    Dim x As New uInternet("http://fr.news.yahoo.com/")
                    Canvas1.Children.Add(x)
                    x.Width = Canvas1.ActualWidth
                    x.Height = Canvas1.ActualHeight
                Case 1 'Prgr TV
                    Dim x As New uInternet("http://www.programme-tv.net/programme/toutes-les-chaines/")
                    Canvas1.Children.Add(x)
                    x.Width = Canvas1.ActualWidth
                    x.Height = Canvas1.ActualHeight
                Case 2 'Contacts
                    Dim x As New uContact
                    Canvas1.Children.Add(x)
                    x.Width = Canvas1.ActualWidth
                    x.Height = Canvas1.ActualHeight
                Case 3 'Module
                    Dim x As New uModules
                    Canvas1.Children.Add(x)
                    x.Width = Canvas1.ActualWidth
                    x.Height = Canvas1.ActualHeight
                Case 4 'Scene
                    'Dim x As New uScenes
                    'Canvas1.Children.Add(x)
                    'x.Width = Canvas1.ActualWidth
                    'x.Height = Canvas1.ActualHeight
                Case 5 'Paramètre
                Case 6 'Recette
                    Dim x As New uInternet("http://www.marmiton.org/")
                    Canvas1.Children.Add(x)
                    x.Width = Canvas1.ActualWidth
                    x.Height = Canvas1.ActualHeight
                Case 7 'Pages jaunes
                    Dim x As New uInternet("http://www.pagesjaunes.fr/")
                    Canvas1.Children.Add(x)
                    x.Width = Canvas1.ActualWidth
                    x.Height = Canvas1.ActualHeight
                Case 8 'Internet
                    Dim x As New uInternet("http://www.google.fr/")
                    Canvas1.Children.Add(x)
                    x.Width = Canvas1.ActualWidth
                    x.Height = Canvas1.ActualHeight
                Case 9 'Itinéraire
                    Dim x As New uInternet("http://maps.google.fr/maps?hl=fr&tab=wl")
                    Canvas1.Children.Add(x)
                    x.Width = Canvas1.ActualWidth
                    x.Height = Canvas1.ActualHeight
                Case 10 'Calculatrice
                    'Process.Start("calc.exe")
                    Dim x As New uCalc
                    Canvas1.Children.Add(x)
                    Canvas1.SetLeft(x, Canvas1.ActualWidth / 2 - x.Width / 2)
                Case 11 'Facebook
                    Dim x As New uInternet("http://fr-fr.facebook.com/")
                    Canvas1.Children.Add(x)
                    x.Width = Canvas1.ActualWidth
                    x.Height = Canvas1.ActualHeight
                Case 12 'Horloge
                    'Dim x As New uHorloge
                    'Canvas1.Children.Add(x)
                    'x.Width = Canvas1.ActualWidth
                    'x.Height = Canvas1.ActualHeight
                Case 13 'Meteo
                    Dim x As New uMeteos
                    Canvas1.Children.Add(x)
                    x.Width = Canvas1.ActualWidth
                    x.Height = Canvas1.ActualHeight
                Case 14 'Calendrier
                    'Dim x As New uCalendar
                    'Canvas1.Children.Add(x)
                    'x.Width = Canvas1.ActualWidth
                    'x.Height = Canvas1.ActualHeight
                Case 15 'Notes
                    Dim x As New uNotes
                    Canvas1.Children.Add(x)
                    x.Width = Canvas1.ActualWidth
                    x.Height = Canvas1.ActualHeight
                Case 16
                    Dim x As New uInternet("http://www.bison-fute.equipement.gouv.fr/diri/Accueil.do")
                    Canvas1.Children.Add(x)
                    x.Width = Canvas1.ActualWidth
                    x.Height = Canvas1.ActualHeight
            End Select
            Me.Cursor = Cursors.Arrow
        Catch ex As Exception
            WriteLog("Erreur: " & ex.Message)
        End Try
    End Sub

    'Afficher le clavier virtuel
    Private Sub Image1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
        Try
            Process.Start("C:\Program Files\Chessware\TouchIt\TouchItf.exe")
        Catch ex As Exception
            WriteLog("Erreur Lancement clavier virtuel: " & ex.Message)
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Private Sub Window1_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        Try
            If IsHSConnect = True Then hsapp.Disconnect()
            Canvas1.Children.Clear()
            End
            WriteLog("Fermeture de l'application")
        Catch ex As Exception
            WriteLog("Erreur Lors de la fermeture: " & ex.Message)
        End Try
    End Sub

    Private Sub ScrollViewer1_PreviewMouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ScrollViewer1.PreviewMouseDown
        scrollStartPoint = e.GetPosition(Me)
        scrollStartOffset.X = ScrollViewer1.HorizontalOffset
    End Sub

    Private Sub ScrollViewer1_PreviewMouseMove(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles ScrollViewer1.PreviewMouseMove
        If e.LeftButton = MouseButtonState.Pressed Then
            Dim currentPoint As Point = e.GetPosition(Me)
            Dim delta As New Point(scrollStartPoint.X - currentPoint.X, scrollStartPoint.Y - currentPoint.Y)
            scrollTarget.X = scrollStartOffset.X + delta.X
            ScrollToPosition(ScrollViewer1, scrollTarget.X, currentPoint.Y, 600)
        End If
    End Sub

    'Bouton Quitter
    Private Sub BtnQuit_Click_1(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnQuit.Click
        WriteLog("Fermeture de l'application")
        End
    End Sub

End Class
