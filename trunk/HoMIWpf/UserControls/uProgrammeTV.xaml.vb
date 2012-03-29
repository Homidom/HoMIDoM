Imports System.IO
Imports System.Windows.Media.Animation

Partial Public Class uProgrammeTV
    Public EchelleMinute As Integer
    Public MargeV As Integer
    Public HeightP As Integer
    Dim TableTop As New List(Of Double) 'Mémorise l'emplacement top des chaines
    Dim TableTop2 As New List(Of Double) 'Mémorise l'emplacement top des programmes
    Dim CurDay As Integer = 0
    Dim IndexUp As Integer = 0
    Private HscrollStartPoint As Point 'memorise l'emplacement horizontal pour les mouvements
    Private VscrollStartPoint As Point 'memorise l'emplacement vertical pour les mouvements
    Dim Index As Integer = 0 'index pour déplacement horizontal
    Dim FlagDetail As Boolean 'si on affiche une fenêtre détail ou chaine
    Dim MaxChn As Integer 'nombre max de chaine pour défilement
    Dim IndexH As Integer 'index pour déplacement vertical

#Region "Data"
    ' Used when manually scrolling.
    Private scrollTarget As Point
    Private scrollStartPoint As Point
    Private scrollStartOffset As Point
    Private previousPoint As Point
#End Region

    Private Sub ScrollViewer1_PreviewMouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ScrollViewer1.PreviewMouseDown
        scrollStartPoint = e.GetPosition(Me)
        scrollStartOffset.X = ScrollViewer1.HorizontalOffset
        scrollStartOffset.Y = ScrollViewer1.VerticalOffset
    End Sub

    Private Sub ScrollViewer1_PreviewMouseMove(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles ScrollViewer1.PreviewMouseMove
        If e.LeftButton = MouseButtonState.Pressed Then
            Dim currentPoint As Point = e.GetPosition(Me)
            Dim delta As New Point(scrollStartPoint.X - currentPoint.X, scrollStartPoint.Y - currentPoint.Y)
            scrollTarget.Y = scrollStartOffset.Y + delta.Y
            scrollTarget.X = scrollStartOffset.X + delta.X
            ScrollToPosition(ScrollViewer1, scrollTarget.X, scrollTarget.Y, m_SpeedTouch)
        End If
    End Sub

    Private Sub uPrgTV_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        'Charge les chaines et les programmes de la base de données
        ChargeChaineFromDB()
        ChargeProgrammesFromDB()

        EchelleMinute = 6
        MargeV = 10
        HeightP = 30

        'Affiche les étiquettes des heures par tranche de 30mn
        For i As Integer = 0 To 47
            Dim myBrush As New LinearGradientBrush
            Dim lbl As New Label

            myBrush.GradientStops.Add(New GradientStop(Colors.DarkGray, 0.0))
            myBrush.GradientStops.Add(New GradientStop(Colors.Black, 1.0))

            lbl.Height = 20
            lbl.FontSize = 8
            lbl.Width = 30 * EchelleMinute

            Dim b As String
            b = i / 2
            If InStr(b, ",") > 1 Then
                b = Mid(b, 1, InStr(b, ",") - 1) & "h30"
            Else
                b = b & "h"
            End If

            lbl.Content = b
            lbl.Background = myBrush
            lbl.Foreground = New SolidColorBrush(Colors.White)
            lbl.BorderThickness = New Thickness(1)
            lbl.BorderBrush = New SolidColorBrush(Colors.Black)

            StkLabel.Children.Add(lbl)
        Next

        AffProgramme(CurDay)
        ScrollViewer3.Width = Me.Width - 85
    End Sub

    '**************************************************
    'Affiche les programmes
    '**************************************************
    Private Sub AffProgramme(ByVal Jour As Integer)
        Dim Chainencour As String = ""
        Dim Chn As Integer = 0
        Dim stk As StackPanel = Nothing
        Dim first As Boolean = False 'Changement de chaine

        StkMid.Children.Clear()
        StkLeft.Children.Clear()
        TableTop2.Clear()

        'Affiche le jour sélectionné
        LblDate.Content = Now.AddDays(Jour).ToLongDateString

        'Affiche les chaine
        For i As Integer = 0 To MyProgramme.Count - 1
            If Chainencour <> MyProgramme.Item(i).IDChannel Then
                If stk IsNot Nothing Then
                    StkMid.Children.Add(stk)
                End If
                first = True
                stk = New StackPanel
                stk.Orientation = Orientation.Horizontal
                stk.HorizontalAlignment = Windows.HorizontalAlignment.Left
                stk.MinHeight = 40

                Chainencour = MyProgramme.Item(i).IDChannel
                Dim myimage As New Rectangle
                myimage.Width = 40
                myimage.Height = 40

                For ch As Integer = 0 To MyChaine.Count - 1
                    If MyChaine.Item(ch).ID = Chainencour Then
                        Dim name As String = MyChaine.Item(ch).Nom
                        name = ConvertHtmlToText(name).Replace(" ", "")
                        name = name.Replace("è", "e")
                        name = name.Replace("é", "e")
                        name = name.Replace("ô", "o")
                        name = UCase(name)
                        If File.Exists("C:\ehome\images\tv\" & name & ".gif") = True Then 'vérifie si le fichier image de la chaine existe
                            Dim mybrush As New ImageBrush
                            mybrush.ImageSource = New BitmapImage(New Uri("C:\ehome\images\tv\" & name & ".gif", UriKind.Relative))
                            myimage.Fill = mybrush
                        End If
                        Exit For
                    End If
                Next
                myimage.Tag = Chn
                TableTop.Add(0)
                StkLeft.Children.Add(myimage)
            End If
            

            'Ajoute les programmes
            If MyProgramme.Item(i).DateStart = Now.AddDays(Jour).ToShortDateString Then
                Dim lbl As New Label
                Dim vTime As DateTime = CDate(MyProgramme.Item(i).TimeStart)
                Dim myBrush As New LinearGradientBrush

                If first = True Then
                    Dim x As New Rectangle
                    x.Width = (vTime.Minute + (vTime.Hour * 60)) * EchelleMinute
                    stk.Children.Add(x)
                    first = False
                End If

                lbl.Width = CDbl(MyProgramme.Item(i).Duree) * EchelleMinute
                lbl.Height = 40
                lbl.Content = ConvertHtmlToText(MyProgramme.Item(i).Titre) & vbCrLf & MyProgramme.Item(i).TimeStart & " - " & MyProgramme.Item(i).TimeEnd
                lbl.BorderThickness = New Thickness(1)
                lbl.BorderBrush = New SolidColorBrush(Colors.Black)
                lbl.Foreground = New SolidColorBrush(Colors.White)
                lbl.Background = myBrush
                lbl.Tag = i
                AddHandler lbl.MouseDoubleClick, AddressOf ProgramMouseDown

                'Applique une couleur suivant le type de catégorie de programme
                Select Case ConvertHtmlToText(MyProgramme.Item(i).Categorie1)
                    Case "série"
                        myBrush.GradientStops.Add(New GradientStop(Colors.DarkTurquoise, 0.0))
                        myBrush.GradientStops.Add(New GradientStop(Colors.Turquoise, 1.0))
                    Case "magazine"
                        myBrush.GradientStops.Add(New GradientStop(Colors.DarkMagenta, 0.0))
                        myBrush.GradientStops.Add(New GradientStop(Colors.Magenta, 1.0))
                    Case "documentaire"
                        myBrush.GradientStops.Add(New GradientStop(Colors.DarkSeaGreen, 0.0))
                        myBrush.GradientStops.Add(New GradientStop(Colors.SeaGreen, 1.0))
                    Case "divertissement"
                        myBrush.GradientStops.Add(New GradientStop(Colors.DarkKhaki, 0.0))
                        myBrush.GradientStops.Add(New GradientStop(Colors.Khaki, 1.0))
                    Case "jeunesse"
                        myBrush.GradientStops.Add(New GradientStop(Colors.DarkOrchid, 0.0))
                        myBrush.GradientStops.Add(New GradientStop(Colors.Orchid, 1.0))
                    Case "divers"
                        myBrush.GradientStops.Add(New GradientStop(Colors.DarkCyan, 0.0))
                        myBrush.GradientStops.Add(New GradientStop(Colors.Cyan, 1.0))
                    Case "téléfilm"
                        myBrush.GradientStops.Add(New GradientStop(Colors.DarkOrange, 0.0))
                        myBrush.GradientStops.Add(New GradientStop(Colors.Orange, 1.0))
                    Case "sport"
                        myBrush.GradientStops.Add(New GradientStop(Colors.DarkGreen, 0.0))
                        myBrush.GradientStops.Add(New GradientStop(Colors.Green, 1.0))
                    Case "film"
                        myBrush.GradientStops.Add(New GradientStop(Colors.DarkViolet, 0.0))
                        myBrush.GradientStops.Add(New GradientStop(Colors.Violet, 1.0))
                    Case Else
                        myBrush.GradientStops.Add(New GradientStop(Colors.DarkBlue, 0.0))
                        myBrush.GradientStops.Add(New GradientStop(Colors.Blue, 1.0))
                End Select

                TableTop2.Add(0) 'mémorise l'emplacement d'origine pour les déplacements
                stk.Children.Add(lbl)
            End If
        Next

        MaxChn = Chn / 2
    End Sub

    Private Sub ProgramMouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
        Dim a As String
        a = UCase(ConvertHtmlToText(MyProgramme.Item(sender.tag).Titre))
        If MyProgramme.Item(sender.tag).SousTitre <> "" Then
            a += " - " & ConvertHtmlToText(MyProgramme.Item(sender.tag).SousTitre) & vbCrLf
        Else
            a += vbCrLf
        End If
        a += MyProgramme.Item(sender.tag).TimeStart & " - " & MyProgramme.Item(sender.tag).TimeEnd & vbCrLf
        If MyProgramme.Item(sender.tag).Categorie1 <> "" Then a += "CATEGORIE: " & ConvertHtmlToText(MyProgramme.Item(sender.tag).Categorie1) & vbCrLf
        If MyProgramme.Item(sender.tag).Description <> "" Then a += "DESCRIPTION: " & ConvertHtmlToText(MyProgramme.Item(sender.tag).Description) & vbCrLf
        If MyProgramme.Item(sender.tag).Annee <> 0 Then a += "ANNEE: " & MyProgramme.Item(sender.tag).Annee & vbCrLf
        If MyProgramme.Item(sender.tag).Credits <> "" Then a += "PRODUCTION: " & vbCrLf & MyProgramme.Item(sender.tag).Credits & vbCrLf

        Dim frm As New FrmDetailProgram(a)
        frm.Owner = frmMere
        frm.ShowDialog()
        If frm.DialogResult.HasValue And frm.DialogResult.Value Then
            frm.Close()
        End If
    End Sub

    Private Sub CloseDetail()
        FlagDetail = False
        'CanvasMiddle.Children.RemoveAt(CanvasMiddle.Children.Count - 1)
    End Sub

    Private Sub BtnD_MouseDown1(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnD.MouseDown
        CurDay += 1
        If CurDay > 4 Then CurDay = 4
        AffProgramme(CurDay)
    End Sub

    Private Sub BtnG_MouseDown1(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles BtnG.MouseDown
        CurDay -= 1
        If CurDay < 0 Then CurDay = 0
        AffProgramme(CurDay)
    End Sub

#Region "Animation"

    Private Sub StkLeft_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles StkLeft.MouseDown
        Dim frm As New FrmConfigChaine
        frm.Owner = frmMere
        frm.ShowDialog()
        If frm.DialogResult.HasValue And frm.DialogResult.Value Then
            frm.Close()
        End If
    End Sub

#End Region

    Private Sub ScrollViewer1_ScrollChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.ScrollChangedEventArgs) Handles ScrollViewer1.ScrollChanged
        ScrollViewer2.ScrollToVerticalOffset(e.VerticalOffset)
        ScrollViewer3.ScrollToHorizontalOffset(e.HorizontalOffset)
    End Sub
End Class
