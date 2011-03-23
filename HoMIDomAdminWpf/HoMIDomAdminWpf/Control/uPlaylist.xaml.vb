' Imports HoMIDom.HoMIDom
Imports System.Windows.Forms
Imports System.IO
Imports TagLib


Partial Public Class uPlaylist
    '--- Variables ------------------
    Public Event CloseMe(ByVal MyObject As Object)
    Dim _Action As EAction 'Défini si modif ou création d'une PlayList
    Dim _PlayListId As String 'Id de la playList à modifier
    Dim FlagNewCmd As Boolean

    Dim FILE_NAME As String = "C:\homidom\test2.txt"
    Private ListTagAudio As New List(Of FileAudioTag)
    Private ListPlist As New List(Of FilePlayList)



    Public Enum EAction
        Nouveau
        Modifier
    End Enum
	
    Public Structure FileAudioTag
        Dim Titre As String
        Dim Artiste As String
        Dim Album As String
        Dim Annee As String
        Dim Comment As String
        Dim Genre As String
        Dim DureeMin As String
        Dim DureeSec As String
        Dim source As String
        Dim sourceWPath As String
        Dim Track As String
    End Structure

    Public Structure FilePlayList
        Dim Titre As String
        Dim DureeMin As String
        Dim DureeSec As String
        Dim sourceWpath As String
    End Structure


    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        If TxtName.Text = "" Then
            MessageBox.Show("Le nom de la zone est obligatoire!", "Erreur")
            '.Show("Le nom de la zone est obligatoire!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        End If
        Dim objWriter As New StreamWriter("C:\homidom\" + TxtName.Text + ".m3u")
        CreatePlayListFile(objWriter)
        For i = 0 To ListPlist.Count - 1
            WriteTitle(ListPlist.Item(i), objWriter)
        Next
        objWriter.Close()
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me)
    End Sub

    ' Creation du fichier au format m3u Extend
    Sub CreatePlayListFile(ByRef objWriterFile As StreamWriter)
        objWriterFile.WriteLine("#EXTM3U")
    End Sub

    ' Enregistrement d un titre au format m3u extend
    Sub WriteTitle(ByRef FileAudioPlist As FilePlayList, ByRef objWriterFile As StreamWriter)
        objWriterFile.WriteLine("#EXTINF:" & (FileAudioPlist.DureeMin * 60 + FileAudioPlist.DureeSec) & "," & FileAudioPlist.Titre)
        objWriterFile.WriteLine(FileAudioPlist.sourceWpath)
    End Sub

    ' Recherche la pochette contenue dans le repertoire et l'affiche 
    Sub ImagePochetteLoad(ByRef PathRep As String)
        Dim bmpImage As New BitmapImage()
        Dim FilePochette As New FileInfo(PathRep & "\folder.jpg")
        ' Recherche du fichier de la pochette
        If (FilePochette.Exists) Then
            bmpImage.BeginInit()
            bmpImage.UriSource = New Uri(FilePochette.FullName, UriKind.Absolute)
            bmpImage.EndInit()
        Else
            bmpImage.BeginInit()
            bmpImage.UriSource = New Uri("..\images\nofound.png", UriKind.Relative)
            bmpImage.EndInit()

        End If
        ' Affichage de la pochette 
        ImgPochette.Source = bmpImage
    End Sub

	' Fonction de chargement des tags des fichiers auidio d'un repertoire
    Sub FileTagRepload(ByRef PathRep As String)

        'Efface le tableau actuel
        ListTagAudio.Clear()

        ' Créér une refenrence du dossier
        Dim di As New DirectoryInfo(PathRep)
        ' Recupere la liste des fichier MP3 du repertoire
        Dim fiArr As FileInfo() = di.GetFiles("*.mp3", SearchOption.TopDirectoryOnly)

        ' Boucle sur tous les fichiers du repertoire
        For i = 0 To fiArr.Length - 1
            Dim a As New FileAudioTag
            Dim X As TagLib.File
            ' Recupere les tags du fichier Audo 
            X = TagLib.File.Create(fiArr(i).FullName)
            a.Titre = X.Tag.Title       ' Sauve le nom du titre du morceau
            a.source = fiArr(i).Name    ' Sauve le nom du fichier audio 
            a.sourceWPath = PathRep + "\" + fiArr(i).Name    ' Sauve le nom complet du fichier audio (Nom + chemin) 
            a.DureeMin = System.Convert.ToString(X.Properties.Duration.Minutes)
            a.DureeSec = System.Convert.ToString(Format(X.Properties.Duration.Seconds, "00"))
            a.Genre = X.Tag.FirstGenre  ' Sauve le genre du morceau
            a.Annee = X.Tag.Year        ' Sauve l' année du morceau
            ListTagAudio.Add(a)

            a = Nothing
            X = Nothing
        Next
        ' ListTagAudio.Sort()

    End Sub

	' Procedure de mise à des listes du la fenetre PlayList
    Private Sub RefreshLists()
        ListFileAudio.Items.Clear()
        ListFilePList.Items.Clear()

        For i As Integer = 0 To ListTagAudio.Count - 1
            ListFileAudio.Items.Add(ListTagAudio.Item(i).source)
        Next

        For i As Integer = 0 To ListPlist.Count - 1
            ListFilePList.Items.Add(ListPlist.Item(i).Titre)
        Next

    End Sub

	' Procedure de chargement du repertoire 
    Private Sub NomRep_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles NomRep.MouseDoubleClick
        Dim dlg As New FolderBrowserDialog()

        ' Ouverture de la fenetre de selection d un repertoire
        Dim result As DialogResult = dlg.ShowDialog()

        If (result = DialogResult.OK) Then
            ' Affichage de la pochette du disque
            ImagePochetteLoad(dlg.SelectedPath)

            ' Affichage du repertoire selectionné
            TitleLength.Text = ""
            TitleGenre.Text = ""
            TitleYear.Text = ""
            NomRep.Text = dlg.SelectedPath
            FileTagRepload(dlg.SelectedPath)
            RefreshLists()

        End If
    End Sub

	' Fonction d'ajout d'un titre à la PlayList
    Private Sub ButtonPlus_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ButtonPlus.Click
        Dim a As New FilePlayList
        If ListFileAudio.SelectedIndex >= 0 Then
            a.Titre = ListTagAudio(ListFileAudio.SelectedIndex).Titre
            a.DureeMin = ListTagAudio(ListFileAudio.SelectedIndex).DureeMin
            a.DureeSec = ListTagAudio(ListFileAudio.SelectedIndex).DureeSec
            a.sourceWpath = ListTagAudio(ListFileAudio.SelectedIndex).sourceWPath
            If ListFilePList.SelectedIndex >= 0 Then
                ListPlist.Insert(ListFilePList.SelectedIndex, (a))
            Else
                ListPlist.Add(a)
            End If
            a = Nothing
            RefreshLists()
        End If
    End Sub

	' Fonction de suppression d'un titre à la PlayList
    Private Sub ButtonMoins_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ButtonMoins.Click
        If ListFilePList.SelectedIndex >= 0 Then
            ListPlist.RemoveAt(ListFilePList.SelectedIndex)
            RefreshLists()
        End If

    End Sub

	' Fonction de deplacement d'un titre de la  PlayList
    Private Sub ButtonUp_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ButtonUp.Click
        Dim a As New FilePlayList

        If ListFilePList.SelectedIndex > 0 Then
            a.Titre = ListPlist.Item(ListFilePList.SelectedIndex).Titre
            a.DureeMin = ListPlist.Item(ListFilePList.SelectedIndex).DureeMin
            a.DureeSec = ListPlist.Item(ListFilePList.SelectedIndex).DureeSec
            a.sourceWpath = ListPlist.Item(ListFilePList.SelectedIndex).sourceWpath
            ListPlist.RemoveAt(ListFilePList.SelectedIndex)
            ListPlist.Insert(ListFilePList.SelectedIndex - 1, (a))
            RefreshLists()
        End If

    End Sub
    Private Sub ButtonDown_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ButtonDown.Click
        Dim a As New FilePlayList

        If ((ListFilePList.SelectedIndex >= 0) And (ListFilePList.SelectedIndex < ListFilePList.Items.Count - 1)) Then
            a.Titre = ListPlist.Item(ListFilePList.SelectedIndex).Titre
            a.DureeMin = ListPlist.Item(ListFilePList.SelectedIndex).DureeMin
            a.DureeSec = ListPlist.Item(ListFilePList.SelectedIndex).DureeSec
            a.sourceWpath = ListPlist.Item(ListFilePList.SelectedIndex).sourceWpath
            ListPlist.RemoveAt(ListFilePList.SelectedIndex)
            ListPlist.Insert(ListFilePList.SelectedIndex + 1, (a))
            RefreshLists()
        End If

    End Sub
	
	' Fonction d'affichage des tags d'un titre 
    Private Sub ListFileAudio_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles ListFileAudio.SelectionChanged

        If ListFileAudio.Items.Count >= 1 Then
            TitleLength.Text = ListTagAudio(ListFileAudio.SelectedIndex).DureeMin & ":" & ListTagAudio(ListFileAudio.SelectedIndex).DureeSec
            TitleGenre.Text = ListTagAudio(ListFileAudio.SelectedIndex).Genre
            TitleYear.Text = ListTagAudio(ListFileAudio.SelectedIndex).Annee
        End If

    End Sub


End Class
