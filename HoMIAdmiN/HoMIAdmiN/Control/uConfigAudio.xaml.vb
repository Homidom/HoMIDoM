Partial Public Class uConfigAudio
    Public Event CloseMe(ByVal MyObject As Object)

    Public Sub New()

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        Try
            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            If Window1.IsConnect = True Then
                RefreshList()
            End If

            BtnDelRepertoireAudio.IsEnabled = False
            BtnDeleteExtension.IsEnabled = False

        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uconfigserver New: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub RefreshList()
        Try
            ListBox1.Items.Clear()
            For i As Integer = 0 To Window1.myService.GetAllRepertoiresAudio(IdSrv).Count - 1
                Dim a As New CheckBox
                a.Content = Window1.myService.GetAllRepertoiresAudio(IdSrv).Item(i).Repertoire
                a.IsChecked = Window1.myService.GetAllRepertoiresAudio(IdSrv).Item(i).Enable
                ListBox1.Items.Add(a)
            Next

            ListBox2.Items.Clear()
            For i As Integer = 0 To Window1.myService.GetAllExtensionsAudio(IdSrv).Count - 1
                Dim a As New CheckBox
                a.Content = Window1.myService.GetAllExtensionsAudio(IdSrv).Item(i).Extension
                a.IsChecked = Window1.myService.GetAllExtensionsAudio(IdSrv).Item(i).Enable
                ListBox2.Items.Add(a)
            Next

            BtnDelRepertoireAudio.IsEnabled = False
            BtnDeleteExtension.IsEnabled = False
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uconfigaudio RefreshList: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnDelRepertoireAudio_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDelRepertoireAudio.Click
        Try
            If ListBox1.SelectedIndex < 0 Then
                MessageBox.Show("Veuillez un répertoire dans la liste!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Exit Sub
            Else
                Window1.myService.DeleteRepertoireAudio(IdSrv, ListBox1.SelectedItem.content)
                RefreshList()
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uconfigaudio BtnDelRepertoireAudio_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnNewRepertoire_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnNewRepertoire.Click
        Dim retour As String
        retour = InputBox("Veuillez saisir le chemin du répertoire Audio à scanner, et l'activer ensuite si besoin", "Répertoire Audio", "")
        If retour = "" Then
            MessageBox.Show("Le chemin du répertoire audio ne peut être vide!", "Repertoire Audio", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        Else
            If Window1.myService.NewRepertoireAudio(retour, False) = -1 Then
                MessageBox.Show("Le chemin du répertoire audio existe déjà il ne sera pas pris en compte", "Repertoire Audio", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Exit Sub
            Else
                RefreshList()
            End If
        End If
    End Sub

    Private Sub BtnNewExension_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnNewExension.Click
        Dim retour As String
        retour = InputBox("Veuillez saisir la nouvelle extension Audio sans le point, puis l'activer si besoin par la suite", "Extension Audio", "")
        If retour = "" Then
            MessageBox.Show("L'extension audio ne peut être vide!", "Extension Audio", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        End If
        If InStr(retour, ".") > 0 Then
            MessageBox.Show("Veuillez ne pas saisir le . !", "Extension Audio", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        End If
        If retour.Length <> 3 Then
            MessageBox.Show("L'extension doit comporter 3 caractères", "Extension Audio", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        End If
        retour = "." & LCase(retour)
        If Window1.myService.NewExtensionAudio(retour, False) = -1 Then
            MessageBox.Show("L'extension audio existe déjà elle ne sera pas pris en compte", "Extension Audio", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        Else
            RefreshList()
        End If

    End Sub

    Private Sub BtnDeleteExtension_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDeleteExtension.Click
        Try
            If ListBox2.SelectedIndex < 0 Then
                MessageBox.Show("Veuillez une extension dans la liste!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Exit Sub
            Else
                Window1.myService.DeleteExtensionAudio(IdSrv, ListBox2.SelectedItem.content)
                RefreshList()
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub uconfigaudio BtnDeleteExtension_Click: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnCancel.Click
        RaiseEvent CloseMe(Me)
    End Sub


    Private Sub ListBox1_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles ListBox1.SelectionChanged
        If ListBox1.SelectedItem IsNot Nothing Then
            BtnDelRepertoireAudio.IsEnabled = True
        Else
            BtnDelRepertoireAudio.IsEnabled = False
        End If
    End Sub

    Private Sub ListBox2_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles ListBox2.SelectionChanged
        If ListBox2.SelectedItem IsNot Nothing Then
            BtnDeleteExtension.IsEnabled = True
        Else
            BtnDeleteExtension.IsEnabled = False
        End If
    End Sub

    Private Sub BtnOk_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOk.Click
        For i As Integer = 0 To ListBox1.Items.Count - 1
            Dim x As CheckBox = ListBox1.Items(i)
            Window1.myService.EnableRepertoireAudio(IdSrv, x.Content, x.IsChecked)
        Next

        For i As Integer = 0 To ListBox2.Items.Count - 1
            Dim x As CheckBox = ListBox2.Items(i)
            Window1.myService.EnableExtensionAudio(IdSrv, x.Content, x.IsChecked)
        Next
    End Sub
End Class
