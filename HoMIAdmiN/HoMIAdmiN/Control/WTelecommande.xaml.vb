Public Class WTelecommande

#Region "Variables"
    Dim FlagNewCmd As Boolean
    Dim x As HoMIDom.HoMIDom.TemplateDevice = Nothing
    Dim _DeviceId As String 'Id du device à modifier
    Dim _Driver As HoMIDom.HoMIDom.TemplateDriver
    Dim _SelectDriverIndex As Integer 'Index du driver sélectionné
    Dim ListButton As New List(Of ImageButton)
    Dim _Row As Integer
    Dim _Col As Integer
#End Region

#Region "Gestion des commandes"
    ''' <summary>
    ''' Nouvelle commande
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BtnNewCmd_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnNewCmd.Click
        TxtCmdName.Text = ""
        TxtCmdRepeat.Text = "0"
        TxtCmdData.Text = ""

        BtnNewCmd.Visibility = Windows.Visibility.Hidden
        FlagNewCmd = True
    End Sub

    ''' <summary>
    ''' Sauvegarder commande
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BtnSaveCmd_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnSaveCmd.Click
        Try
            If IsNumeric(TxtCmdRepeat.Text) = False Then
                MsgBox("Numérique obligatoire pour repeat !!")
                Exit Sub
            End If
            If TxtCmdName.Text = "" Or TxtCmdName.Text = " " Then
                MsgBox("Le nom de la commande est obligatoire !!")
                Exit Sub
            End If

            If x IsNot Nothing Then

                If FlagNewCmd = True Then 'nouvelle commande
                    Dim _cmd As New HoMIDom.HoMIDom.Telecommande.Commandes
                    With _cmd
                        .Name = TxtCmdName.Text
                        .Code = TxtCmdData.Text
                        .Repeat = TxtCmdRepeat.Text
                        .Picture = ImgCommande.Tag
                    End With
                    x.Commandes.Add(_cmd)
                Else 'modifier commande
                    Dim idx As Integer = ListCmd.SelectedIndex
                    If idx < 0 Then Exit Sub

                    With x.Commandes.Item(idx)
                        .Name = TxtCmdName.Text
                        .Code = TxtCmdData.Text
                        .Repeat = TxtCmdRepeat.Text
                        .Picture = ImgCommande.Tag

                    End With
                End If

                Dim retour As String = myService.SaveTemplate(IdSrv, cbTemplate.Text, x.Commandes, slider_Row.Value, slider_Column.Value)
                If retour <> "0" Then
                    MessageBox.Show("Erreur lors de l'enregistrement de la commande dans le template: " & retour, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
                    Exit Sub
                Else
                    ListCmd.Items.Clear()
                    For i2 As Integer = 0 To x.Commandes.Count - 1
                        ListCmd.Items.Add(x.Commandes.Item(i2).Name)
                    Next
                End If

            End If

            BtnNewCmd.Visibility = Windows.Visibility.Visible
            FlagNewCmd = False
        Catch Ex As Exception
            MessageBox.Show("Erreur BtnSaveCmd: " & Ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Supprimer une commande
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BtnDelCmd_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDelCmd.Click
        Try
            If ListCmd.SelectedIndex >= 0 Then
                x.Commandes.RemoveAt(ListCmd.SelectedIndex)
                Dim retour As String = myService.SaveTemplate(IdSrv, cbTemplate.Text, x.Commandes, slider_Row.Value, slider_Column.Value)
                If retour <> "0" Then
                    MessageBox.Show("Erreur lors de l'enregistrement de la commande dans le template: " & retour, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
                    Exit Sub
                Else
                    ListCmd.Items.Clear()
                    For i2 As Integer = 0 To x.Commandes.Count - 1
                        ListCmd.Items.Add(x.Commandes.Item(i2).Name)
                    Next

                End If
                BtnDelCmd.Visibility = Windows.Visibility.Hidden
                TxtCmdData.Text = ""
                TxtCmdName.Text = ""
                TxtCmdRepeat.Text = ""
            End If
        Catch Ex As Exception
            MessageBox.Show("Erreur BtnDelCmd: " & Ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Tester une commande
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BtnTstCmd_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnTstCmd.Click
        Try
            If TxtCmdName.Text = "" Or TxtCmdName.Text = " " Then

            End If
            Dim retour As String = myService.TelecommandeSendCommand(IdSrv, _DeviceId, TxtCmdName.Text)
            If retour <> 0 Then
                MessageBox.Show(retour, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
            End If
        Catch Ex As Exception
            MessageBox.Show("Erreur BtnTstCmd: " & Ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Apprendre une commande IR
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BtnLearn_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnLearn.Click
        Try
            If _Driver IsNot Nothing Then
                If _Driver.Enable = True And _Driver.IsConnect Then
                    TxtCmdData.Text = myService.StartLearning(IdSrv, _Driver.ID)
                Else
                    MessageBox.Show("Impossible d'apprendre un code car le driver n'est pas activé ou n'est pas connecté", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
                End If
            Else
                MessageBox.Show("Impossible d'apprendre un code car le driver n'a pas été trouvé", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
            End If
        Catch Ex As Exception
            MessageBox.Show("Erreur BtnLearnCmd: " & Ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnImg_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnImg.Click
        Try
            Dim frm As New WindowImg
            frm.ShowDialog()
            If frm.DialogResult.HasValue And frm.DialogResult.Value Then
                Dim retour As String = frm.FileName
                If retour <> "" Then
                    ImgCommande.Source = ConvertArrayToImage(myService.GetByteFromImage(retour))
                    ImgCommande.Tag = retour
                End If
                frm.Close()
            Else
                frm.Close()
            End If
            frm = Nothing
        Catch ex As Exception
            MessageBox.Show("ERREUR Sub ImgCommande_MouseLeftButtonDown: " & ex.Message, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Changement de sélection d'une commande dans la liste
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ListCmd_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles ListCmd.SelectionChanged
        Try
            Dim i As Integer = ListCmd.SelectedIndex
            If i < 0 Then Exit Sub

            BtnDelCmd.Visibility = Windows.Visibility.Visible
            BtnTstCmd.Visibility = Windows.Visibility.Visible
            TxtCmdName.Text = x.Commandes.Item(i).Name
            TxtCmdData.Text = x.Commandes.Item(i).Code
            TxtCmdRepeat.Text = x.Commandes.Item(i).Repeat

            If x.Commandes.Item(i).Picture IsNot Nothing Then
                If x.Commandes.Item(i).Picture <> " " Then
                    ImgCommande.Source = ConvertArrayToImage(myService.GetByteFromImage(x.Commandes.Item(i).Picture))
                    ImgCommande.Tag = x.Commandes.Item(i).Picture
                End If

            End If
        Catch Ex As Exception
            MessageBox.Show("Erreur ListCmd_SelectionChanged: " & Ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Valeur de data de la commande a changée
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub TxtCmdData_TextInput(ByVal sender As Object, ByVal e As System.Windows.Input.TextCompositionEventArgs) Handles TxtCmdData.TextInput
        BtnTstCmd.Visibility = Windows.Visibility.Visible
    End Sub

    ''' <summary>
    ''' Clic sur image de la commande
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ImgCommande_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ImgCommande.MouseLeftButtonDown, Canvas2.MouseLeftButtonDown
        If ImgCommande.Source IsNot Nothing Then
            Dim effects As DragDropEffects
            Dim obj As New DataObject()
            obj.SetData(GetType(Image), sender)
            effects = DragDrop.DoDragDrop(sender, obj, DragDropEffects.Copy Or DragDropEffects.Move)
        Else
            MessageBox.Show("Vous ne pouvez pas inclure cette commande dans la grille car elle ne comporte aucune image!", "Information", MessageBoxButton.OK, MessageBoxImage.Information)
        End If
    End Sub

#End Region


#Region "Gestion des Templates"
    ''' <summary>
    ''' Nouveau template
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BtnNewTemplate_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnNewTemplate.Click
        StkNewTemplate.Visibility = Windows.Visibility.Visible
        TxtTplFab.Focus()
        BtnSaveTemplate.Visibility = Windows.Visibility.Visible
        lbltplbase.Visibility = Windows.Visibility.Visible
        cbBase.Visibility = Windows.Visibility.Visible
        TxtTplFab.IsReadOnly = False
        TxtTplMod.IsReadOnly = False
    End Sub

    ''' <summary>
    ''' Sauvegarder Template
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BtnSaveTemplate_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnSaveTemplate.Click
        Try

            If TxtTplFab.Text = "" Or TxtTplFab.Text = " " Or InStr(TxtTplFab.Text, "-") > 0 Then
                MessageBox.Show("Le nom du fabricant est obligatoire et ne doit pas comporter le caractère: - ", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                TxtTplFab.Focus()
                Exit Sub
            End If
            If TxtTplMod.Text = "" Or TxtTplMod.Text = " " Or InStr(TxtTplMod.Text, "-") > 0 Then
                MessageBox.Show("Le nom du modèle est obligatoire et ne doit pas comporter le caractère: - ", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                TxtTplMod.Focus()
                Exit Sub
            End If

            Dim mytemplate As String = LCase(TxtTplFab.Text) & "-" & LCase(TxtTplMod.Text) & "-" & LCase(myService.GetAllDrivers(IdSrv).Item(_SelectDriverIndex).Protocol)

            Dim retour As String = myService.CreateNewTemplate(TxtTplFab.Text, TxtTplMod.Text, myService.GetAllDrivers(IdSrv).Item(_SelectDriverIndex).Protocol, cbBase.SelectedIndex, slider_Row.Value, slider_Column.Value)
            If retour <> "0" Then
                MessageBox.Show("Erreur lors de la création du nouveau template: " & retour, "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Exit Sub
            Else

                StkNewTemplate.Visibility = Windows.Visibility.Collapsed
                BtnSaveTemplate.Visibility = Windows.Visibility.Hidden
                lbltplbase.Visibility = Windows.Visibility.Hidden
                cbBase.Visibility = Windows.Visibility.Hidden

                cbTemplate.Items.Clear()
                Dim _list As New List(Of HoMIDom.HoMIDom.Telecommande.Template)
                _list = myService.GetListOfTemplate
                For i As Integer = 0 To _list.Count - 1
                    Dim tpl As String = Replace(_list(i).File, ".xml", "")
                    cbTemplate.Items.Add(tpl)
                Next

                cbTemplate.Text = mytemplate

                TxtTplFab.IsReadOnly = True
                TxtTplMod.IsReadOnly = True
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur: " & ex.ToString)
        End Try
    End Sub


#End Region


    Public Sub New(Optional ByVal DeviceId As String = "", Optional ByVal SelectDriverIndex As Integer = -1, Optional ByVal Driver As HoMIDom.HoMIDom.TemplateDriver = Nothing, Optional ByVal Template As HoMIDom.HoMIDom.TemplateDevice = Nothing)

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        Try
            _DeviceId = DeviceId
            _SelectDriverIndex = SelectDriverIndex
            _Driver = Driver
            x = Template

            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            ListCmd.Items.Clear()

            'Affiche le bouton apprendre que c'est un protocole de type IR
            If _Driver.Protocol = "IR" Then
                BtnLearn.Visibility = Windows.Visibility.Visible
            Else
                BtnLearn.Visibility = Windows.Visibility.Collapsed
            End If

            'Récupère la liste des template
            Dim _list As New List(Of HoMIDom.HoMIDom.Telecommande.Template)
            _list = myService.GetListOfTemplate

            Dim idx As Integer = -1
            For i As Integer = 0 To _list.Count - 1
                Dim tpl As String = Replace(_list(i).File, ".xml", "") 'récupère le nom du template
                cbTemplate.Items.Add(tpl) 'ajoute le nom du template dans la liste

                'si le device compote un template (qui est stocké dans son modele)
                If x.Modele IsNot Nothing Then
                    If tpl = x.Modele.ToString Then

                        cbTemplate.IsEnabled = False 'impossible de modifier le template
                        BtnNewTemplate.Visibility = Windows.Visibility.Collapsed
                        TxtTplFab.Text = _list.Item(i).Fabricant
                        TxtTplMod.Text = _list.Item(i).Modele
                        idx = i
                        _Col = _list.Item(i).Ligne
                        _Row = _list.Item(i).Colonne

                        If x.Commandes IsNot Nothing Then
                            GrpCmd.Visibility = Windows.Visibility.Visible
                            For i2 As Integer = 0 To x.Commandes.Count - 1
                                ListCmd.Items.Add(x.Commandes.Item(i2).Name)
                            Next
                        End If
                    End If
                End If
            Next
            cbTemplate.SelectedIndex = idx

        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'ouverture de la fenêtre d'édition:" & ex.ToString, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

#Region "Designer"


    Private Sub slider_Row_ValueChanged(ByVal sender As Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of Double)) Handles slider_Row.ValueChanged
        Try
            '==== Initalisation ====
            If Me.grid_Telecommande Is Nothing Then Exit Sub

            'Initialisation des lignes de la grille si celle-ci est vide
            If Me.grid_Telecommande.RowDefinitions IsNot Nothing Then
                If grid_Telecommande.RowDefinitions.Count = 0 Then
                    grid_Telecommande.RowDefinitions.Clear()
                    'grid_Telecommande.Height = 50
                    'Initialisation de la hauteur du background
                    'rectangle.Height = 70
                End If
            End If

            '==== Modification de la grille et du background pour chaque division du slider ====
            If slider_Row.Value > 1 Then
                'Si la grille contient moins de ligne que définit
                If Me.grid_Telecommande.RowDefinitions.Count < slider_Row.Value Then
                    Dim diff As Integer = slider_Row.Value - Me.grid_Telecommande.RowDefinitions.Count
                    For i As Integer = 1 To diff
                        'Augmente la hauteur de la grille
                        grid_Telecommande.Height = slider_Row.Value * 50
                        'Augmente la hauteur du background
                        rectangle.Height = slider_Row.Value * 50 + 20
                        'Ajoute une ligne à la grille
                        Dim rowDef As New RowDefinition
                        grid_Telecommande.RowDefinitions.Add(rowDef)

                        For j As Integer = 0 To grid_Telecommande.ColumnDefinitions.Count - 1
                            Dim x As New Canvas
                            x.Width = 45
                            x.Height = 45
                            x.Background = Brushes.Black
                            x.AllowDrop = True
                            x.Tag = grid_Telecommande.RowDefinitions.Count - 1 & "|" & j
                            AddHandler x.DragOver, AddressOf CVS_DragOver
                            AddHandler x.Drop, AddressOf CVS_Drop
                            Grid.SetColumn(x, j)
                            Grid.SetRow(x, grid_Telecommande.RowDefinitions.Count - 1)
                            grid_Telecommande.Children.Add(x)
                        Next

                    Next
                End If
                'Si la grille contient plus de ligne que définit
                If Me.grid_Telecommande.RowDefinitions.Count > slider_Row.Value Then
                    Dim diff As Integer = Me.grid_Telecommande.RowDefinitions.Count - slider_Row.Value
                    For i As Integer = 1 To diff
                        'Augmente la hauteur de la grille
                        grid_Telecommande.Height = slider_Row.Value * 50
                        'Augmente la hauteur du background
                        rectangle.Height = slider_Row.Value * 50 + 20
                        'Retire une ligne à la grille
                        grid_Telecommande.RowDefinitions.RemoveAt(Me.grid_Telecommande.RowDefinitions.Count - 1)
                    Next
Retour:
                    For i As Integer = 0 To grid_Telecommande.Children.Count - 1
                        Dim x As Canvas = grid_Telecommande.Children.Item(i)
                        Dim a() As String = Split(x.Tag, "|")
                        If a(0) > grid_Telecommande.RowDefinitions.Count - 1 Then
                            grid_Telecommande.Children.RemoveAt(i)
                            GoTo Retour
                        End If
                    Next
                End If
                'For i As Integer = 1 To slider_Row.Value
                '    'Augmente la hauteur de la grille
                '    grid_Telecommande.Height = slider_Row.Value * 50
                '    'Augmente la hauteur du background
                '    rectangle.Height = slider_Row.Value * 50 + 20
                '    'Ajoute une ligne à la grille
                '    Dim rowDef As New RowDefinition
                '    grid_Telecommande.RowDefinitions.Add(rowDef)
                'Next
            End If

            'Remplir()
        Catch ex As Exception
            MessageBox.Show("Erreur slider_Row_ValueChanged: " & ex.ToString)
        End Try
    End Sub

    Private Sub slider_Column_ValueChanged(ByVal sender As Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of Double)) Handles slider_Column.ValueChanged
        Try
            '==== Initalisation ====
            If Me.grid_Telecommande Is Nothing Then Exit Sub
            'Initialisation des colonnes de la grille si celle-ci est vide

            If grid_Telecommande.ColumnDefinitions IsNot Nothing Then
                If grid_Telecommande.ColumnDefinitions.Count = 0 Then
                    grid_Telecommande.ColumnDefinitions.Clear()
                    grid_Telecommande.Width = 50
                    'Initialisation de la largeur du background
                    'Me.rectangle.Width = 70
                End If
            End If

            '==== Modification de la grille et du background pour chaque division du slider ====
            If slider_Column.Value > 1 Then
                'Si la grille contient moins de ligne que définit
                If Me.grid_Telecommande.ColumnDefinitions.Count < slider_Column.Value Then
                    Dim diff As Integer = slider_Column.Value - Me.grid_Telecommande.ColumnDefinitions.Count
                    For i As Integer = 1 To diff
                        'Augmente la largeur de la grille
                        grid_Telecommande.Width = slider_Column.Value * 50
                        'Augmente la largeur du background
                        rectangle.Width = slider_Column.Value * 50 + 20
                        'Ajoute une colonne à la grille
                        Dim colDef As New ColumnDefinition
                        grid_Telecommande.ColumnDefinitions.Add(colDef)

                        For j As Integer = 0 To grid_Telecommande.RowDefinitions.Count - 1
                            Dim x As New Canvas
                            x.Width = 45
                            x.Height = 45
                            x.Background = Brushes.Black
                            x.AllowDrop = True
                            x.Tag = j & "|" & grid_Telecommande.ColumnDefinitions.Count - 1
                            AddHandler x.DragOver, AddressOf CVS_DragOver
                            AddHandler x.Drop, AddressOf CVS_Drop
                            Grid.SetColumn(x, grid_Telecommande.ColumnDefinitions.Count - 1)
                            Grid.SetRow(x, j)
                            grid_Telecommande.Children.Add(x)
                        Next
                    Next
                End If
                'Si la grille contient plus de colonne que définit
                If Me.grid_Telecommande.ColumnDefinitions.Count > slider_Column.Value Then
                    Dim diff As Integer = Me.grid_Telecommande.ColumnDefinitions.Count - slider_Column.Value
                    For i As Integer = 1 To diff
                        'Augmente la hauteur de la grille
                        grid_Telecommande.Height = slider_Row.Value * 50
                        'Augmente la hauteur du background
                        rectangle.Height = slider_Row.Value * 50 + 20
                        'Retire une ligne à la grille
                        grid_Telecommande.ColumnDefinitions.RemoveAt(Me.grid_Telecommande.ColumnDefinitions.Count - 1)
                    Next
Retour:
                    For i As Integer = 0 To grid_Telecommande.Children.Count - 1
                        Dim x As Canvas = grid_Telecommande.Children.Item(i)
                        Dim a() As String = Split(x.Tag, "|")
                        If a(1) > grid_Telecommande.ColumnDefinitions.Count - 1 Then
                            grid_Telecommande.Children.RemoveAt(i)
                            GoTo Retour
                        End If
                    Next
                End If
                'For j As Integer = 1 To slider_Column.Value
                '    'Augmente la largeur de la grille
                '    grid_Telecommande.Width = slider_Column.Value * 50
                '    'Augmente la largeur du background
                '    rectangle.Width = slider_Column.Value * 50 + 20
                '    'Ajoute une colonne à la grille
                '    Dim colDef As New ColumnDefinition
                '    grid_Telecommande.ColumnDefinitions.Add(colDef)
                'Next
            End If

            'Remplir()
        Catch ex As Exception
            MessageBox.Show("Erreur slider_Column_ValueChanged: " & ex.ToString)
        End Try
    End Sub

    Private Sub Save()
        ListButton.Clear()

        For i As Integer = 0 To grid_Telecommande.Children.Count - 1
            Dim x As Canvas = grid_Telecommande.Children.Item(i)
            If x IsNot Nothing Then
                If x.Children.Count <> 0 Then
                    ListButton.Add(x.Children.Item(0))
                End If
            End If
        Next

        'MessageBox.Show(ListButton.Count & " button(s) enregistré(s)")
    End Sub

    Private Sub Recharge()
        Remplir()

        For i As Integer = 0 To ListButton.Count - 1
            Dim x As New Canvas
            x.Width = 45
            x.Height = 45
            x.Background = Brushes.Black
            x.AllowDrop = True
            x.Tag = ListButton(i).Row & "|" & ListButton(i).Column
            AddHandler x.DragOver, AddressOf CVS_DragOver
            AddHandler x.Drop, AddressOf CVS_Drop
            Grid.SetColumn(x, ListButton(i).Column)
            Grid.SetRow(x, ListButton(i).Row)

            Dim img1 As New ImageButton
            img1.Source = ListButton(i).Source
            img1.Command = ListButton(i).Command
            img1.AllowDrop = True
            Dim a() As String = x.Tag.split("|")
            img1.Row = a(0)
            img1.Column = a(1)
            AddHandler img1.MouseLeftButtonDown, AddressOf Img_MouseLeftButtonDown
            AddHandler img1.Delete, AddressOf DeleteButton
            x.Children.Add(img1)

            grid_Telecommande.Children.Add(x)
        Next
    End Sub

    Private Sub CVS_DragOver(ByVal sender As Object, ByVal e As System.Windows.DragEventArgs)
        If e.Data.GetDataPresent(GetType(Image)) Then
            e.Effects = DragDropEffects.Copy
        Else
            e.Effects = DragDropEffects.None
        End If

    End Sub

    Private Sub CVS_Drop(ByVal sender As System.Object, ByVal e As System.Windows.DragEventArgs)
        Try
            If e.Data.GetDataPresent(GetType(Image)) Then
                If sender.children.count > 0 Then
                    Exit Sub
                End If

                e.Effects = DragDropEffects.Copy
                ' Utiliser uri comme vous le souhaitez
                Dim img1 As New ImageButton
                If InStr(e.Data.GetData(GetType(Image)).parent.GetType.ToString, "Canvas") Then
                    e.Data.GetData(GetType(Image)).parent.children.clear()
                End If
                img1.Source = e.Data.GetData(GetType(Image)).source
                img1.AllowDrop = True
                img1.Command = e.Data.GetData(GetType(Image)).tooltip
                Dim a() As String = sender.tag.split("|")
                img1.Row = a(0)
                img1.Column = a(1)
                AddHandler img1.MouseLeftButtonDown, AddressOf Img_MouseLeftButtonDown
                AddHandler img1.Delete, AddressOf DeleteButton
                sender.Children.Add(img1)
            Else
                e.Effects = DragDropEffects.None
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur: " & ex.ToString)
        End Try
    End Sub

    Private Sub Remplir()
        For i As Integer = 0 To grid_Telecommande.RowDefinitions.Count - 1
            For j As Integer = 0 To grid_Telecommande.ColumnDefinitions.Count - 1
                Dim x As New Canvas
                x.Width = 45
                x.Height = 45
                x.Background = Brushes.Black
                x.AllowDrop = True
                x.Tag = i & "|" & j
                AddHandler x.DragOver, AddressOf CVS_DragOver
                AddHandler x.Drop, AddressOf CVS_Drop
                Grid.SetColumn(x, j)
                Grid.SetRow(x, i)
                grid_Telecommande.Children.Add(x)
            Next
        Next
    End Sub

    'Supprimer un bouton via menu contextuel
    Private Sub DeleteButton(ByVal sender As Object)
        sender.parent.children.clear()
    End Sub

    Private Sub Img_MouseLeftButtonDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
        Dim effects As DragDropEffects
        Dim obj As New DataObject()
        obj.SetData(GetType(Image), sender)
        effects = DragDrop.DoDragDrop(sender, obj, DragDropEffects.Copy Or DragDropEffects.Move)
    End Sub

    Private Sub button_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles button.Click
        x.Modele = cbTemplate.Text
        DialogResult = True
    End Sub

    'Sauvegarder la grille
    Private Sub BtnSaveGrid_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnSaveGrid.Click
        Save()
    End Sub

    'Vider la grille
    Private Sub BtnVide_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnVide.Click
        grid_Telecommande.Children.Clear()
    End Sub

    'Recharger la grille
    Private Sub BtnRechargeGrid_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnRechargeGrid.Click
        Recharge()
    End Sub
#End Region

    Private Sub WTelecommande_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Try
            slider_Column.Value = _Col
            slider_Row.Value = _Row
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub cbTemplate_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cbTemplate.SelectionChanged
        Try
            If cbTemplate.SelectedIndex < 0 Then
            Else
                'Récupère la liste des template
                Dim _list As New List(Of HoMIDom.HoMIDom.Telecommande.Template)
                _list = myService.GetListOfTemplate

                TxtTplFab.Text = _list.Item(cbTemplate.SelectedIndex).Fabricant
                TxtTplMod.Text = _list.Item(cbTemplate.SelectedIndex).Modele
                slider_Column.Value = _list.Item(cbTemplate.SelectedIndex).Ligne
                slider_Row.Value = _list.Item(cbTemplate.SelectedIndex).Colonne
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur: " & ex.ToString)
        End Try
    End Sub
End Class
