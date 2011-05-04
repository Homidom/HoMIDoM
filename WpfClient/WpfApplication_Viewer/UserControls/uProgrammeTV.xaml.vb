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
    Dim TableTmp As New List(Of Double) 'Mémorise l'emplacement left des chaines
    Dim TableTmp2 As New List(Of Double) 'Mémorise l'emplacement left des programmes
    Dim Index As Integer = 0 'index pour déplacement horizontal
    Dim FlagDetail As Boolean 'si on affiche une fenêtre détail ou chaine
    Dim MaxChn As Integer 'nombre max de chaine pour défilement
    Dim IndexH As Integer 'index pour déplacement vertical

    Private Sub uPrgTV_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        If CanvasMiddle.Children.Count > 0 Then CanvasMiddle.Children.Clear()

        'Charge les chaines et les programmes de la base de données
        ChargeChaineFromDB()
        ChargeProgrammesFromDB()

        EchelleMinute = 6
        MargeV = 10
        HeightP = 30

        CanvasD.Width = Canvas2.ActualWidth
        CanvasMiddle.Width = Canvas2.ActualWidth

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

            TableTmp.Add(0)
            CanvasD.SetLeft(lbl, i * (30 * EchelleMinute))
            CanvasD.Children.Add(lbl)
        Next

        AffProgramme(CurDay)

        For i As Integer = 0 To Now.Hour - 1
            Avance()
        Next

        Canvas1.Height = DockPanel1.Height
        DockPanel1.SetZIndex(Canvas4, 1)
        DockPanel1.SetZIndex(Canvas2, 1)
    End Sub

    '**************************************************
    'Affiche les programmes
    '**************************************************
    Private Sub AffProgramme(ByVal Jour As Integer)
        Dim Chainencour As String
        Dim Chn As Integer = 0

        CanvasMiddle.Children.Clear()
        Canvas1.Children.Clear()
        TableTmp2.Clear()
        TableTop2.Clear()

        'Affiche le jour sélectionné
        LblDate.Content = Now.AddDays(Jour).ToLongDateString

        'Affiche les chaine
        Chainencour = MyProgramme.Item(0).IDChannel
        For i As Integer = 0 To MyProgramme.Count - 1
            If i = 0 Then 'c'est la première chaine
                Dim myimage As New Rectangle
                AddHandler myimage.MouseDown, AddressOf ConfigChaine
                myimage.Width = 40
                myimage.Height = 40
                For ch As Integer = 0 To MyChaine.Count - 1
                    If MyChaine.Item(ch).ID = Chainencour Then
                        Dim name As String = UCase(MyChaine.Item(ch).Nom.Replace(" ", ""))
                        If File.Exists("C:\ehome\images\tv\" & name & ".gif") = True Then 'vérifie si le fichier image de la chaine existe
                            Dim mybrush As New ImageBrush
                            mybrush.ImageSource = New BitmapImage(New Uri("C:\ehome\images\tv\" & name & ".gif", UriKind.Relative))
                            myimage.Fill = mybrush
                        End If
                        Exit For
                    End If
                Next
                myimage.Tag = Chn
                Canvas1.SetTop(myimage, (Chn * 40) + MargeV)
                TableTop.Add(0)
                Canvas1.SetLeft(myimage, 40)
                Canvas1.Children.Add(myimage)
            End If
            If Chainencour <> MyProgramme.Item(i).IDChannel Then 'on traite les autres chaines
                Chainencour = MyProgramme.Item(i).IDChannel
                Chn += 1
                Dim myimage As New Rectangle
                AddHandler myimage.MouseDown, AddressOf ConfigChaine
                myimage.Tag = Chn
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
                        If File.Exists("C:\ehome\images\tv\" & name & ".gif") = True Then
                            Dim mybrush As New ImageBrush
                            mybrush.ImageSource = New BitmapImage(New Uri("C:\ehome\images\tv\" & name & ".gif", UriKind.Relative))
                            myimage.Fill = mybrush
                        End If
                        Exit For
                    End If
                Next
                TableTop.Add(0)
                Canvas1.SetTop(myimage, (Chn * 40) + MargeV)
                Canvas1.SetLeft(myimage, 40)
                Canvas1.Children.Add(myimage)
            End If

            'Ajoute les programmes
            If MyProgramme.Item(i).DateStart = Now.AddDays(Jour).ToShortDateString Then
                Dim lbl As New Label
                Dim vTime As DateTime = CDate(MyProgramme.Item(i).TimeStart)
                Dim myBrush As New LinearGradientBrush
                Dim w As Long

                lbl.Width = CDbl(MyProgramme.Item(i).Duree) * EchelleMinute
                lbl.Height = 40
                lbl.Content = ConvertHtmlToText(MyProgramme.Item(i).Titre) & vbCrLf & MyProgramme.Item(i).TimeStart & " - " & MyProgramme.Item(i).TimeEnd
                lbl.BorderThickness = New Thickness(1)
                lbl.BorderBrush = New SolidColorBrush(Colors.Black)
                lbl.Foreground = New SolidColorBrush(Colors.White)
                lbl.Background = myBrush
                lbl.Tag = i
                AddHandler lbl.PreviewMouseDown, AddressOf ProgramMouseDown

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

                'Calcul le LEFT de positionnement du programme
                w = (((vTime.Hour * 60) + vTime.Minute) * EchelleMinute) + Canvas1.Width
                CanvasMiddle.SetLeft(lbl, w)
                'Place le TOP de positionnement du programme
                CanvasMiddle.SetTop(lbl, (Chn * 40) + MargeV)
                TableTop2.Add(0) 'mémorise l'emplacement d'origine pour les déplacements
                CanvasMiddle.Children.Add(lbl)
                TableTmp2.Add(0)
            End If
        Next

        MaxChn = Chn / 2
    End Sub

    Private Sub ProgramMouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
        FlagDetail = True
        Dim Detail As New Canvas
        Dim myBrush As New LinearGradientBrush
        Detail.Width = 600
        Detail.Height = 300
        myBrush.GradientStops.Add(New GradientStop(Colors.Black, 0.0))
        myBrush.GradientStops.Add(New GradientStop(Colors.DarkGray, 1.0))
        Detail.Background = myBrush

        Dim lbl = New TextBox()
        lbl.TextWrapping = TextWrapping.Wrap
        lbl.Background = Brushes.Transparent
        lbl.Foreground = Brushes.White
        lbl.TextAlignment = TextAlignment.Left
        lbl.MaxLines = 90
        lbl.Width = 580
        lbl.Height = 250
        lbl.Text = UCase(ConvertHtmlToText(MyProgramme.Item(sender.tag).Titre))
        If MyProgramme.Item(sender.tag).SousTitre <> "" Then
            lbl.Text += " - " & ConvertHtmlToText(MyProgramme.Item(sender.tag).SousTitre) & vbCrLf
        Else
            lbl.Text += vbCrLf
        End If
        lbl.Text += MyProgramme.Item(sender.tag).TimeStart & " - " & MyProgramme.Item(sender.tag).TimeEnd & vbCrLf
        If MyProgramme.Item(sender.tag).Categorie1 <> "" Then lbl.Text += "CATEGORIE: " & ConvertHtmlToText(MyProgramme.Item(sender.tag).Categorie1) & vbCrLf
        If MyProgramme.Item(sender.tag).Description <> "" Then lbl.Text += "DESCRIPTION: " & ConvertHtmlToText(MyProgramme.Item(sender.tag).Description) & vbCrLf
        If MyProgramme.Item(sender.tag).Annee <> 0 Then lbl.Text += "ANNEE: " & MyProgramme.Item(sender.tag).Annee & vbCrLf
        If MyProgramme.Item(sender.tag).Credits <> "" Then lbl.Text += "PRODUCTION: " & vbCrLf & MyProgramme.Item(sender.tag).Credits & vbCrLf
        lbl.VerticalScrollBarVisibility = ScrollBarVisibility.Auto

        Detail.Children.Add(lbl)
        Detail.SetTop(lbl, 10)
        Detail.SetLeft(lbl, 10)
        AddHandler Detail.MouseDown, AddressOf CloseDetail

        CanvasMiddle.Children.Add(Detail)
        CanvasMiddle.SetTop(Detail, 60)
        CanvasMiddle.SetLeft(Detail, 160)
    End Sub

    Private Sub CloseDetail()
        FlagDetail = False
        CanvasMiddle.Children.RemoveAt(CanvasMiddle.Children.Count - 1)
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

    Private Sub ConfigChaine()
        FlagDetail = True
        Dim mycanva As New Canvas
        Dim Mylistbox As New ListBox
        Dim MyBtnSave As New Button
        Dim MyBtnClose As New Button
        Dim MyBtnSearchChaine As New Button

        Mylistbox.Width = 190
        Mylistbox.Height = 120
        Mylistbox.FontSize = 14
        For i As Integer = 0 To MyChaine.Count - 1
            Dim chk As New CheckBox
            chk.IsChecked = MyChaine.Item(i).Enable
            chk.Content = ConvertHtmlToText(MyChaine.Item(i).Nom)
            Mylistbox.Items.Add(chk)
        Next
        listboxtp = Mylistbox
        mycanva.Children.Add(Mylistbox)
        mycanva.SetLeft(Mylistbox, 10)
        mycanva.SetTop(Mylistbox, 10)

        MyBtnSave.Width = 70
        MyBtnSave.Height = 25
        MyBtnSave.Content = "Sauvegarder"
        AddHandler MyBtnSave.Click, AddressOf BtnSaveChaine_Click
        mycanva.Children.Add(MyBtnSave)
        mycanva.SetLeft(MyBtnSave, 210)
        mycanva.SetTop(MyBtnSave, 10)

        MyBtnSearchChaine.Width = 70
        MyBtnSearchChaine.Height = 25
        MyBtnSearchChaine.Content = "Chercher"
        AddHandler MyBtnSearchChaine.Click, AddressOf BtnSearchChaine_Click
        mycanva.Children.Add(MyBtnSearchChaine)
        mycanva.SetLeft(MyBtnSearchChaine, 210)
        mycanva.SetTop(MyBtnSearchChaine, 45)

        MyBtnClose.Width = 70
        MyBtnClose.Height = 25
        MyBtnClose.Content = "Fermer"
        AddHandler MyBtnClose.Click, AddressOf CloseConfigChaine
        mycanva.Children.Add(MyBtnClose)
        mycanva.SetLeft(MyBtnClose, 210)
        mycanva.SetTop(MyBtnClose, 80)

        Dim myBrush As New LinearGradientBrush
        myBrush.GradientStops.Add(New GradientStop(Colors.Black, 0.0))
        myBrush.GradientStops.Add(New GradientStop(Colors.DarkGray, 1.0))
        mycanva.Height = 150
        mycanva.Width = 300
        mycanva.Background = myBrush
        CanvasMiddle.Children.Add(mycanva)
        CanvasMiddle.SetLeft(mycanva, 190)
        CanvasMiddle.SetTop(mycanva, 18)
        CanvasMiddle.SetZIndex(mycanva, 1)
    End Sub

    Dim listboxtp As New ListBox

    Private Sub CloseConfigChaine(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        CanvasMiddle.Children.RemoveAt(CanvasMiddle.Children.Count - 1)
        FlagDetail = False
    End Sub

    Private Sub BtnSaveChaine_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        For i As Integer = 0 To MyChaine.Count - 1
            Dim chk As New CheckBox
            chk = listboxtp.Items.Item(i)
            If chk.IsChecked = True Then
                EnableChaine(MyChaine.Item(i).ID, True)
            Else
                EnableChaine(MyChaine.Item(i).ID, False)
            End If
        Next
        ChargeChaineFromDB()
    End Sub

    Private Sub BtnSearchChaine_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim Thread1 As New System.Threading.Thread(AddressOf ChaineFromXMLToDB)
        Thread1.Start()
        CanvasMiddle.Children.RemoveAt(CanvasMiddle.Children.Count - 1)
    End Sub

#Region "Animation"
    Public Sub Avance()
        If Index > 23 Then Exit Sub
        If TableTmp.Count = 0 Then Exit Sub
        Dim sb As New Storyboard()

        For i As Integer = 0 To CanvasMiddle.Children.Count - 1
            Dim horzAnim As New DoubleAnimation()
            horzAnim.From = TableTmp2(i)
            horzAnim.[To] = TableTmp2(i) - (30 * EchelleMinute * 2)
            horzAnim.DecelerationRatio = 0.99
            horzAnim.Duration = New Duration(TimeSpan.FromMilliseconds(600))
            Dim trans As TranslateTransform = New TranslateTransform
            trans.BeginAnimation(TranslateTransform.XProperty, horzAnim)
            CanvasMiddle.Children.Item(i).RenderTransform = trans
            TableTmp2(i) -= (30 * EchelleMinute * 2)
        Next

        For i As Integer = 0 To 47
            Dim horzAnim As New DoubleAnimation()
            horzAnim.From = TableTmp(i)
            horzAnim.[To] = TableTmp(i) - (30 * EchelleMinute * 2)
            horzAnim.DecelerationRatio = 0.99
            horzAnim.Duration = New Duration(TimeSpan.FromMilliseconds(600))
            Dim trans As TranslateTransform = New TranslateTransform
            CanvasD.Children.Item(i).RenderTransform = trans
            trans.BeginAnimation(TranslateTransform.XProperty, horzAnim)
            TableTmp(i) -= (30 * EchelleMinute * 2)
        Next

        Index += 1
    End Sub

    Public Sub Recule()
        If Index = 0 Then Exit Sub
        Dim sb As New Storyboard()

        For i As Integer = 0 To CanvasMiddle.Children.Count - 1
            Dim horzAnim As New DoubleAnimation()
            horzAnim.From = TableTmp2(i)
            horzAnim.[To] = TableTmp2(i) + (30 * EchelleMinute * 2)
            horzAnim.DecelerationRatio = 0.99
            horzAnim.Duration = New Duration(TimeSpan.FromMilliseconds(600))
            Dim trans As TranslateTransform = New TranslateTransform
            trans.BeginAnimation(TranslateTransform.XProperty, horzAnim)
            CanvasMiddle.Children.Item(i).RenderTransform = trans
            TableTmp2(i) += (30 * EchelleMinute * 2)
        Next

        For i As Integer = 0 To 47
            Dim horzAnim As New DoubleAnimation()
            horzAnim.From = TableTmp(i)
            horzAnim.[To] = TableTmp(i) + (30 * EchelleMinute * 2)
            Dim coef As Double = System.Windows.SystemParameters.PrimaryScreenWidth / Me.ActualWidth
            horzAnim.DecelerationRatio = 0.99
            horzAnim.Duration = New Duration(TimeSpan.FromMilliseconds(600))
            Dim trans As TranslateTransform = New TranslateTransform
            CanvasD.Children.Item(i).RenderTransform = trans
            trans.BeginAnimation(TranslateTransform.XProperty, horzAnim)
            TableTmp(i) += (30 * EchelleMinute * 2)
        Next

        Index -= 1
    End Sub

    Public Sub Monte()
        If IndexH > MaxChn Then Exit Sub
        Dim sb As New Storyboard()

        For i As Integer = 0 To CanvasMiddle.Children.Count - 1
            Dim vertAnim As New DoubleAnimation()
            vertAnim.From = TableTop2(i)
            vertAnim.[To] = TableTop2(i) - 80
            vertAnim.DecelerationRatio = 0.99
            vertAnim.Duration = New Duration(TimeSpan.FromMilliseconds(600))
            Dim trans As TranslateTransform = New TranslateTransform
            trans.X = TableTmp2(i)
            trans.BeginAnimation(TranslateTransform.YProperty, vertAnim)
            CanvasMiddle.Children.Item(i).RenderTransform = trans
            TableTop2(i) -= 80
        Next

        For i As Integer = 0 To Canvas1.Children.Count - 1
            Dim vertAnim As New DoubleAnimation()
            vertAnim.From = TableTop(i)
            vertAnim.[To] = TableTop(i) - 80
            vertAnim.DecelerationRatio = 0.99
            vertAnim.Duration = New Duration(TimeSpan.FromMilliseconds(600))
            Dim trans As TranslateTransform = New TranslateTransform
            Canvas1.Children.Item(i).RenderTransform = trans
            trans.BeginAnimation(TranslateTransform.YProperty, vertAnim)
            TableTop(i) -= 80
        Next

        IndexH += 1
    End Sub

    Public Sub Descend()
        If IndexH = 0 Then Exit Sub
        Dim sb As New Storyboard()

        For i As Integer = 0 To CanvasMiddle.Children.Count - 1
            Dim vertAnim As New DoubleAnimation()
            vertAnim.From = TableTop2(i)
            vertAnim.[To] = TableTop2(i) + 80
            vertAnim.DecelerationRatio = 0.99
            vertAnim.Duration = New Duration(TimeSpan.FromMilliseconds(600))
            Dim trans As TranslateTransform = New TranslateTransform
            trans.X = TableTmp2(i)
            trans.BeginAnimation(TranslateTransform.YProperty, vertAnim)
            CanvasMiddle.Children.Item(i).RenderTransform = trans
            TableTop2(i) += 80
        Next

        For i As Integer = 0 To Canvas1.Children.Count - 1
            Dim vertAnim As New DoubleAnimation()
            vertAnim.From = TableTop(i)
            vertAnim.[To] = TableTop(i) + 80
            vertAnim.DecelerationRatio = 0.99
            vertAnim.Duration = New Duration(TimeSpan.FromMilliseconds(600))
            Dim trans As TranslateTransform = New TranslateTransform
            Canvas1.Children.Item(i).RenderTransform = trans
            trans.BeginAnimation(TranslateTransform.YProperty, vertAnim)
            TableTop(i) += 80
        Next

        IndexH -= 1
    End Sub

    Private Sub Canvas4_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Canvas4.MouseDown
        If FlagDetail = True Then Exit Sub 'ne rien faire si une fenêtre détail prog est en cours
        HscrollStartPoint = e.GetPosition(Me)
    End Sub

    Private Sub Canvas4_PreviewMouseUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Canvas4.PreviewMouseUp
        If FlagDetail = True Then Exit Sub 'ne rien faire si une fenêtre détail prog est en cours
        Dim currentPoint As Point = e.GetPosition(Me)
        Dim i As Integer
        Dim delta As New Point(HscrollStartPoint.X - currentPoint.X, HscrollStartPoint.Y - currentPoint.Y)
        If delta.X < 0 Then
            i = delta.X * -1
            If 30 < i < 100 Then
                Avance()
            End If
            If 99 < i < 200 Then
                Avance()
                Avance()
                Exit Sub
            End If
            If i > 199 Then
                Avance()
                Avance()
                Avance()
                Avance()
                Exit Sub
            End If
        Else

            i = delta.X
            If 30 < i < 100 Then
                Recule()
                Exit Sub
            End If
            If 99 < i < 200 Then
                Recule()
                Recule()
                Exit Sub
            End If
            If i > 199 Then
                Recule()
                Recule()
                Recule()
                Recule()
                Exit Sub
            End If
        End If

    End Sub

    Private Sub Canvas1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Canvas1.MouseDown
        VscrollStartPoint = e.GetPosition(Me)
    End Sub

    Private Sub Canvas1_PreviewMouseUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Canvas1.PreviewMouseUp
        If FlagDetail = True Then Exit Sub 'ne rien faire si une fenêtre détail prog est en cours
        Dim currentPoint As Point = e.GetPosition(Me)
        Dim i As Integer
        Dim delta As New Point(VscrollStartPoint.X - currentPoint.X, VscrollStartPoint.Y - currentPoint.Y)

        If delta.Y < 0 Then
            i = delta.Y * -1
            If i > 35 Then Descend()
        Else
            i = delta.Y
            If i > 35 Then Monte()
        End If
    End Sub
#End Region
End Class
