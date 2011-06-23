#Region "Imports"
Imports System.Windows.Forms
Imports System.IO
Imports TagLib
'Imports System.Collections.ObjectModel

#End Region


Public Class uPlaylist

#Region "Declaration des variables"

    Private _MonRepertoire As String = System.Environment.CurrentDirectory 'représente le répertoire de l'application 'Application.StartupPath
    ' Liste des extensions audio
    Private _ListeExt As List(Of String)

#End Region

    Public Sub New()

        Dim cpt1 As Integer

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        Try
            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            If Window1.IsConnect = True Then
                ListTagAudio.Clear()
            End If


        Catch ex As Exception
            'MessageBox.Show("ERREUR Sub uconfigserver New: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try

        ' Creation de la liste des repertoires audios
        For cpt1 = 0 To Window1.myService.GetAllRepertoiresAudio.Count - 1
            ' Repertoire actif
            If Window1.myService.GetAllRepertoiresAudio.Item(cpt1).Enable = True Then NomRep.Items.Add(Window1.myService.GetAllRepertoiresAudio.Item(cpt1).Repertoire)
        Next

    End Sub

    Public Event CloseMe(ByVal MyObject As Object)

    Public Enum EAction
        Nouveau
        Modifier
    End Enum


    Public Class FilePlayList
        Dim _Titre As String
        Dim _Artiste As String
        Dim _Album As String
        Dim _Année As String
        Dim _Comment As String
        Dim _Genre As String
        Dim _Durée As String
        Dim _Source As String
        Dim _SourceWpath As String
        Dim _Track As String

        Public Sub New(ByVal titre As String, ByVal artiste As String, ByVal album As String, ByVal année As String, ByVal comment As String,
                       ByVal genre As String, ByVal durée As String, ByVal source As String, ByVal sourcepath As String, ByVal track As String)
            Me._Titre = titre
            Me._Durée = durée
            Me._Genre = genre
            Me._Année = année
            Me._SourceWpath = sourcepath
            Me._Track = track
        End Sub

        Public Property Titre() As String
            Get
                Return Me._Titre
            End Get
            Set(ByVal value As String)
                Me._Titre = value
            End Set
        End Property

        Public Property Artiste() As String
            Get
                Return Me._Artiste
            End Get
            Set(ByVal value As String)
                Me._Artiste = value
            End Set
        End Property

        Public Property Album() As String
            Get
                Return Me._Album
            End Get
            Set(ByVal value As String)
                Me._Album = value
            End Set
        End Property

        Public Property Année() As String
            Get
                Return Me._Année
            End Get
            Set(ByVal value As String)
                Me._Année = value
            End Set
        End Property

        Public Property Comment() As String
            Get
                Return Me._Comment
            End Get
            Set(ByVal value As String)
                Me._Comment = value
            End Set
        End Property

        Public Property Genre() As String
            Get
                Return Me._Genre
            End Get
            Set(ByVal value As String)
                Me._Genre = value
            End Set
        End Property

        Public Property Durée() As String
            Get
                Return Me._Durée
            End Get
            Set(ByVal value As String)
                Me._Durée = value
            End Set
        End Property

        Public Property Source() As String
            Get
                Return Me._Source
            End Get
            Set(ByVal value As String)
                Me._Source = value
            End Set
        End Property


        Public Property SourceWpath() As String
            Get
                Return Me._SourceWpath
            End Get
            Set(ByVal value As String)
                Me._SourceWpath = value
            End Set
        End Property

        Public Property Track() As String
            Get
                Return Me._Track
            End Get
            Set(ByVal value As String)
                Me._Track = value
            End Set
        End Property
    End Class

    '--- Variables ------------------
    Private ListTagAudio As New List(Of FilePlayList) ' Liste des tags des fichiers audio 
    Private ListPlist As New List(Of FilePlayList)    ' Liste des fichiers audio de la playlist


    ''' <summary>Fonction de chargement des tags des fichiers audio des repertoires contenus dans la liste active </summary>
    ''' <remarks>Recupere les fichiers Audios selon les extensions actives</remarks>
    Sub FileTagRepload(ByRef PathRep As String)

        'Efface le tableau actuel
        ListTagAudio.Clear()

        ' Créér une reference du dossier
        Dim di As New DirectoryInfo(PathRep)

        ' Pour chacune des extensions
        For cpt2 = 0 To Window1.myService.GetAllExtensionsAudio.Count - 1

            Dim _extension As String
            Dim _extensionenable As String = True
            _extension = Window1.myService.GetAllExtensionsAudio.Item(cpt2).Extension
            _extensionenable = Window1.myService.GetAllExtensionsAudio.Item(cpt2).Enable

            ' Recupere la liste des fichiers du repertoire si l'extension est active
            If _extensionenable Then ' Extension active 
                ' Recuperation des fichiers du repertoire
                Dim fiArr As FileInfo() = di.GetFiles("*" & _extension, SearchOption.TopDirectoryOnly)

                ' Boucle sur tous les fichiers du repertoire
                For i = 0 To fiArr.Length - 1
                    Dim X As TagLib.File
                    ' Recupere les tags du fichier Audio 
                    X = TagLib.File.Create(fiArr(i).FullName)
                    Dim a As New FilePlayList(X.Tag.Title, X.Tag.FirstPerformer, X.Tag.Album, X.Tag.Year, X.Tag.Comment, X.Tag.FirstGenre,
                                              System.Convert.ToString(X.Properties.Duration.Minutes) & ":" & System.Convert.ToString(Format(X.Properties.Duration.Seconds, "00")),
                                              fiArr(i).Name, PathRep + "\" + fiArr(i).Name, X.Tag.Track)

                    ListTagAudio.Add(a)

                    a = Nothing
                    X = Nothing
                Next
            End If
        Next



    End Sub

    ''' <summary>Creation du fichier au format m3u Extend </summary>
    ''' <param name="objWriterFile"></param>
    ''' <remarks></remarks>
    Sub CreatePlayListFile(ByRef objWriterFile As StreamWriter)
        objWriterFile.WriteLine("#EXTM3U")
    End Sub

    ''' <summary>Enregistrement d un titre au format m3u extend </summary>
    ''' <param name="FileAudioPlist"></param>
    ''' <param name="objWriterFile"></param>
    ''' <remarks></remarks> 
    Sub WriteTitle(ByRef FileAudioPlist As FilePlayList, ByRef objWriterFile As StreamWriter)
        objWriterFile.WriteLine("#EXTINF:" & FileAudioPlist.Durée & "," & FileAudioPlist.Titre)
        objWriterFile.WriteLine(FileAudioPlist.SourceWpath)
    End Sub

    ''' <summary> Recherche la pochette contenue dans le repertoire et l'affiche</summary>
    ''' <param name="PathRep"></param>
    ''' <remarks></remarks>
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
            bmpImage.UriSource = New Uri("..\Images\homidom_icone_moyen.png", UriKind.Relative)
            bmpImage.EndInit()

        End If
        ' Affichage de la pochette 
        ImgPochette.Source = bmpImage
    End Sub


    ' Procedure de mise à des listes de la fenetre PlayList dans l'interface
    Private Sub RefreshLists()

        ListFilePList.Items.Clear()

        Dim ListToto As New List(Of FilePlayList)

        For i As Integer = 0 To ListTagAudio.Count - 1
            ListToto.Add(New FilePlayList(ListTagAudio.Item(i).Titre, ListTagAudio.Item(i).Artiste, ListTagAudio.Item(i).Album, ListTagAudio.Item(i).Année, ListTagAudio.Item(i).Comment,
                                          ListTagAudio.Item(i).Genre, ListTagAudio.Item(i).Durée, ListTagAudio.Item(i).Source, ListTagAudio.Item(i).SourceWpath, ListTagAudio.Item(i).Track))
        Next

        ListFileAudio.DataContext = ListToto

        For i As Integer = 0 To ListPlist.Count - 1
            ListFilePList.Items.Add(ListPlist.Item(i).Titre)
        Next

    End Sub

    ' Bouton Ok
    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        If TxtName.Text = "" Then
            MessageBox.Show("Le nom de la zone est obligatoire!", "Erreur")
            Exit Sub
        End If
        ' Creation du fichier m3u
        Dim objWriter As New StreamWriter(_MonRepertoire + "\multimedia\" + TxtName.Text + ".m3u")
        CreatePlayListFile(objWriter)
        For i = 0 To ListPlist.Count - 1
            WriteTitle(ListPlist.Item(i), objWriter)
        Next
        objWriter.Close()
    End Sub

    ' Bouton close
    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClose.Click
        RaiseEvent CloseMe(Me)
    End Sub


    ' Fonction d'ajout d'un titre à la PlayList
    Private Sub ButtonPlus_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ButtonPlus.Click

        ' Controle q'un titre est bien selectionné dans la listee
        If ListFileAudio.SelectedIndex >= 0 Then
            ' Est ce le premier element de la playList
            If ListFilePList.SelectedIndex > 0 Then
                ListPlist.Insert(ListFilePList.SelectedIndex, ListTagAudio(ListFileAudio.SelectedIndex))
            Else
                ListPlist.Add(ListTagAudio(ListFileAudio.SelectedIndex))
            End If
        End If
        RefreshLists()

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
        Dim a As FilePlayList

        If ListFilePList.SelectedIndex > 0 Then
            a = ListPlist.Item(ListFilePList.SelectedIndex)
            ListPlist.RemoveAt(ListFilePList.SelectedIndex)
            ListPlist.Insert(ListFilePList.SelectedIndex - 1, (a))
            RefreshLists()
        End If

    End Sub

    ' Fonction de deplacement d'un titre de la  PlayList
    Private Sub ButtonDown_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ButtonDown.Click
        Dim a As FilePlayList

        If ((ListFilePList.SelectedIndex >= 0) And (ListFilePList.SelectedIndex < ListFilePList.Items.Count - 1)) Then
            a = ListPlist.Item(ListFilePList.SelectedIndex)
            ListPlist.RemoveAt(ListFilePList.SelectedIndex)
            ListPlist.Insert(ListFilePList.SelectedIndex + 1, (a))
            RefreshLists()
        End If

    End Sub

    Private Sub NomRep_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles NomRep.SelectionChanged

        ' Affichage de la pochette du disque
        ImagePochetteLoad(NomRep.SelectedItem)

        ' Recupere la liste des fichiers audio du repertoire
        FileTagRepload(NomRep.SelectedItem)
        RefreshLists()

    End Sub


End Class
