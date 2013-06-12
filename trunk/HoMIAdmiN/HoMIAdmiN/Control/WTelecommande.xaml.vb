Public Class WTelecommande

#Region "Variables"
    Dim FlagNewCmd As Boolean
    Dim x As HoMIDom.HoMIDom.TemplateDevice = Nothing
    Dim _DeviceId As String 'Id du device à modifier
    Dim _Driver As HoMIDom.HoMIDom.TemplateDriver
    'Dim _SelectDriverIndex As Integer 'Index du driver sélectionné
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
        ImgCommande.Source = Nothing
        ImgCommande.Tag = ""

        BtnNewCmd.Visibility = Windows.Visibility.Hidden
        BtnSaveCmd.Visibility = Windows.Visibility.Visible
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
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "Numérique obligatoire pour repeat !!", "Erreur", "BtnSaveCmd.Click")
                Exit Sub
            End If
            If TxtCmdName.Text = "" Or TxtCmdName.Text = " " Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "Le nom de la commande est obligatoire !!", "Erreur", "BtnSaveCmd.Click")
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

            End If

            ListCmd.Items.Clear()
            For i2 As Integer = 0 To x.Commandes.Count - 1
                ListCmd.Items.Add(x.Commandes.Item(i2).Name)
            Next

            BtnNewCmd.Visibility = Windows.Visibility.Visible
            FlagNewCmd = False
        Catch Ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur BtnSaveCmd: " & Ex.Message, "Erreur", "")
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
                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur lors de l'enregistrement de la commande dans le template: " & retour, "Erreur", "")
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
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur BtnDelCmd: " & Ex.Message, "Erreur", "")
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
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, retour, "Erreur", "")
            End If
        Catch Ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur BtnTstCmd: " & Ex.Message, "Erreur", "")
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
                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Impossible d'apprendre un code car le driver n'est pas activé ou n'est pas connecté", "Erreur", "")
                End If
            Else
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Impossible d'apprendre un code car le driver n'a pas été trouvé", "Erreur", "")
            End If
        Catch Ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur BtnLearnCmd: " & Ex.Message, "Erreur", "")
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
                    BtnSaveCmd.Visibility = Windows.Visibility.Visible
                End If
                frm.Close()
            Else
                frm.Close()
            End If
            frm = Nothing
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub ImgCommande_MouseLeftButtonDown: " & ex.Message, "ERREUR", "")
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

            BtnSaveCmd.Visibility = Windows.Visibility.Collapsed
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
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur ListCmd_SelectionChanged: " & Ex.Message, "Erreur", "")
        End Try
    End Sub

    Private Sub TxtCmdData_TextChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtCmdData.TextChanged
        BtnSaveCmd.Visibility = Windows.Visibility.Visible
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
    Private Sub ImgCommande_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ImgCommande.MouseLeftButtonDown ', Canvas2.MouseLeftButtonDown
        For i As Integer = 0 To grid_Telecommande.Children.Count - 1
            Dim cvs As Canvas = grid_Telecommande.Children.Item(i)

            If cvs IsNot Nothing Then
                If cvs.Children.Count <> 0 Then
                    Dim y As ImageButton = cvs.Children.Item(0)
                    If ListCmd.SelectedValue = y.Command Then
                        AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.MESSAGE, "Vous ne pouvez pas inclure cette commande dans la grille car elle y est déjà utilisée!", "Information", "BtnImgCommande.Click")
                        Exit Sub
                    End If

                End If
            End If
        Next

        If ImgCommande.Source IsNot Nothing Then
            Dim effects As DragDropEffects
            Dim obj As New DataObject()
            obj.SetData(GetType(Image), sender)
            effects = DragDrop.DoDragDrop(sender, obj, DragDropEffects.Copy Or DragDropEffects.Move)
        Else
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Vous ne pouvez pas inclure cette commande dans la grille car elle ne comporte aucune image!", "Information", "")
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
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le nom du fabricant est obligatoire et ne doit pas comporter le caractère: - ", "Erreur", "")
                TxtTplFab.Focus()
                Exit Sub
            End If
            If TxtTplMod.Text = "" Or TxtTplMod.Text = " " Or InStr(TxtTplMod.Text, "-") > 0 Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le nom du modèle est obligatoire et ne doit pas comporter le caractère: - ", "Erreur", "")
                TxtTplMod.Focus()
                Exit Sub
            End If

            If _Driver IsNot Nothing Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "OK", "", "")
            Else
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "VIDE", "", "")
            End If

            Dim mytemplate As String = LCase(TxtTplFab.Text) & "-" & LCase(TxtTplMod.Text) & "-" & _Driver.Protocol

            Dim retour As String = myService.CreateNewTemplate(TxtTplFab.Text, TxtTplMod.Text, _Driver.Protocol, cbBase.SelectedIndex, slider_Row.Value, slider_Column.Value)
            If retour <> "0" Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur lors de la création du nouveau template: " & retour, "Erreur", "")
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
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur: " & ex.ToString)
        End Try
    End Sub


#End Region


    Public Sub New(Optional ByVal DeviceId As String = "", Optional ByVal DriverUid As String = "", Optional ByVal Template As HoMIDom.HoMIDom.TemplateDevice = Nothing)

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        Try
            _DeviceId = DeviceId
            _Driver = myService.ReturnDriverByID(IdSrv, DriverUid)
            x = Template

            ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            ListCmd.Items.Clear()

            'Affiche le bouton apprendre que c'est un protocole de type IR
            If _Driver IsNot Nothing Then
                If _Driver.Protocol = "IR" Then
                    BtnLearn.Visibility = Windows.Visibility.Visible
                Else
                    BtnLearn.Visibility = Windows.Visibility.Collapsed
                End If
            End If

            'Récupère la liste des template
            Dim _list As New List(Of HoMIDom.HoMIDom.Telecommande.Template)
            _list = myService.GetListOfTemplate

            Dim idx As Integer = -1
            For i As Integer = 0 To _list.Count - 1
                Dim tpl As String = Replace(_list(i).File, ".xml", "") 'récupère le nom du template
                cbTemplate.Items.Add(tpl) 'ajoute le nom du template dans la liste

                'si le device comporte un template (qui est stocké dans son modele)
                If x IsNot Nothing Then
                    If x.Modele <> "" Then
                        If tpl = x.Modele.ToString Then

                            cbTemplate.IsEnabled = False 'impossible de modifier le template
                            BtnNewTemplate.Visibility = Windows.Visibility.Collapsed
                            TxtTplFab.Text = _list.Item(i).Fabricant
                            TxtTplMod.Text = _list.Item(i).Modele
                            idx = i
                            _Col = _list.Item(i).Colonne
                            _Row = _list.Item(i).Ligne

                            If x.Commandes IsNot Nothing Then
                                GrpCmd.Visibility = Windows.Visibility.Visible

                                For i2 As Integer = 0 To x.Commandes.Count - 1
                                    ListCmd.Items.Add(x.Commandes.Item(i2).Name)
                                Next
                            End If
                        End If
                    End If
                End If
            Next
            cbTemplate.SelectedIndex = idx

            BtnSaveCmd.Visibility = Windows.Visibility.Collapsed
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur lors de l'ouverture de la fenêtre d'édition:" & ex.ToString, "Erreur", "")
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

            'Augmente la hauteur de la grille
            grid_Telecommande.Height = slider_Row.Value * 50
            'Augmente la hauteur du background
            rectangle.Height = slider_Row.Value * 50 + 20
            Caneva_grid.Height = slider_Row.Value * 50 + 60

            '==== Modification de la grille et du background pour chaque division du slider ====
            If slider_Row.Value > 1 Then
                'Si la grille contient moins de ligne que définit
                If Me.grid_Telecommande.RowDefinitions.Count < slider_Row.Value Then
                    Dim diff As Integer = slider_Row.Value - Me.grid_Telecommande.RowDefinitions.Count
                    For i As Integer = 1 To diff

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
            End If

            'Remplir()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur slider_Row_ValueChanged: " & ex.ToString)
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

            'Augmente la largeur de la grille
            grid_Telecommande.Width = slider_Column.Value * 50
            'Augmente la largeur du background
            rectangle.Width = slider_Column.Value * 50 + 20
            Caneva_grid.Width = slider_Column.Value * 50 + 60

            '==== Modification de la grille et du background pour chaque division du slider ====
            If slider_Column.Value > 1 Then
                'Si la grille contient moins de ligne que définit
                If Me.grid_Telecommande.ColumnDefinitions.Count < slider_Column.Value Then
                    Dim diff As Integer = slider_Column.Value - Me.grid_Telecommande.ColumnDefinitions.Count
                    For i As Integer = 1 To diff

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

                    Me.Width = grid_Telecommande.Width + 300
                End If

                'Si la grille contient plus de colonne que définit
                If Me.grid_Telecommande.ColumnDefinitions.Count > slider_Column.Value Then
                    Dim diff As Integer = Me.grid_Telecommande.ColumnDefinitions.Count - slider_Column.Value
                    For i As Integer = 1 To diff
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
            End If


            'Remplir()
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur slider_Column_ValueChanged: " & ex.ToString)
        End Try
    End Sub

    Private Sub Save()
        Try
            ListButton.Clear()

            For i As Integer = 0 To grid_Telecommande.Children.Count - 1
                Dim cvs As Canvas = grid_Telecommande.Children.Item(i)
                If cvs IsNot Nothing Then
                    If cvs.Children.Count <> 0 Then
                        ListButton.Add(cvs.Children.Item(0))
                    End If
                End If
            Next
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub Telecommande Save: " & ex.Message, "ERREUR", "")
        End Try
    End Sub

    Private Sub Recharge()
        Try
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
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub Telecommande Recharge: " & ex.Message, "ERREUR", "")
        End Try
    End Sub

    Private Sub CVS_DragOver(ByVal sender As Object, ByVal e As System.Windows.DragEventArgs)
        Try
            If e.Data.GetDataPresent(GetType(Image)) Then
                e.Effects = DragDropEffects.Copy
            Else
                e.Effects = DragDropEffects.None
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub Telecommande CVS_DragOver: " & ex.Message, "ERREUR", "")
        End Try
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
                    If e.Data.GetData(GetType(Image)).name <> "ImgCommande" Then
                        e.Data.GetData(GetType(Image)).parent.children.clear()
                    End If
                End If
                img1.Source = e.Data.GetData(GetType(Image)).source
                img1.Tag = e.Data.GetData(GetType(Image)).Tag
                img1.AllowDrop = True
                img1.Command = ListCmd.SelectedValue
                img1.ToolTip = ListCmd.SelectedValue
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
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub Telecommande CVS_Drop: " & ex.Message, "ERREUR", "")
        End Try
    End Sub

    Private Sub Remplir()
        Try
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
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub Telecommande Remplir: " & ex.Message, "ERREUR", "")
        End Try
    End Sub

    'Supprimer un bouton via menu contextuel
    Private Sub DeleteButton(ByVal sender As Object)
        Try
            sender.parent.children.clear()

            For i As Integer = 0 To x.Commandes.Count - 1
                If x.Commandes(i).Name = sender.command Then
                    x.Commandes(i).Row = -1
                    x.Commandes(i).Column = -1
                End If
            Next
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub Telecommande DeleteButton: " & ex.Message, "ERREUR", "")
        End Try
    End Sub

    Private Sub Img_MouseLeftButtonDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
        Try
            Dim effects As DragDropEffects
            Dim obj As New DataObject()
            obj.SetData(GetType(Image), sender)
            effects = DragDrop.DoDragDrop(sender, obj, DragDropEffects.Copy Or DragDropEffects.Move)
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub Telecommande Img_MouseLeftButtonDown: " & ex.Message, "ERREUR", "")
        End Try
    End Sub

    'Fermer
    Private Sub button_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles button.Click
        Try
            x.Modele = cbTemplate.Text
            For i As Integer = 0 To grid_Telecommande.Children.Count - 1
                Dim cvs As Canvas = grid_Telecommande.Children.Item(i)

                If cvs IsNot Nothing Then
                    If cvs.Children.Count <> 0 Then
                        For j As Integer = 0 To x.Commandes.Count - 1

                            Dim y As ImageButton = cvs.Children.Item(0)
                            If x.Commandes(j).Name = y.Command Then
                                x.Commandes(j).Row = y.Row
                                x.Commandes(j).Column = y.Column
                                Exit For
                            End If
                        Next
                    End If
                End If
            Next

            If cbTemplate.Text <> "" Then
                Dim retour As String = myService.SaveTemplate(IdSrv, cbTemplate.Text, x.Commandes, slider_Row.Value, slider_Column.Value)
                If retour <> "0" Then
                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur lors de l'enregistrement de la commande dans le template: " & retour, "Erreur", "")
                End If
            End If

            DialogResult = True
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub Telecommande button_Click: " & ex.Message, "ERREUR", "")
        End Try
    End Sub



#End Region

    Private Sub WTelecommande_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Try
            slider_Column.Value = _Col
            slider_Row.Value = _Row

            If x IsNot Nothing Then
                If x.Commandes IsNot Nothing Then
                    GrpCmd.Visibility = Windows.Visibility.Visible

                    Remplir()

                    For i2 As Integer = 0 To x.Commandes.Count - 1

                        If x.Commandes.Item(i2).Row > 0 And x.Commandes.Item(i2).Column > 0 Then
                            Dim cvs As New Canvas
                            cvs.Width = 45
                            cvs.Height = 45
                            cvs.Background = Brushes.Black
                            cvs.AllowDrop = True
                            cvs.Tag = x.Commandes.Item(i2).Row & "|" & x.Commandes.Item(i2).Column 'ListButton(i).Row & "|" & ListButton(i).Column
                            AddHandler cvs.DragOver, AddressOf CVS_DragOver
                            AddHandler cvs.Drop, AddressOf CVS_Drop
                            Grid.SetColumn(cvs, x.Commandes.Item(i2).Column) ' ListButton(i).Column)
                            Grid.SetRow(cvs, x.Commandes.Item(i2).Row) 'ListButton(i).Row)

                            Dim img1 As New ImageButton
                            img1.Source = ConvertArrayToImage(myService.GetByteFromImage(x.Commandes.Item(i2).Picture))
                            img1.Tag = x.Commandes.Item(i2).Picture
                            img1.Command = x.Commandes.Item(i2).Name

                            img1.AllowDrop = True
                            Dim a() As String = cvs.Tag.split("|")
                            img1.Row = a(0)
                            img1.Column = a(1)
                            img1.HorizontalAlignment = Windows.HorizontalAlignment.Stretch
                            img1.VerticalAlignment = Windows.VerticalAlignment.Stretch
                            AddHandler img1.MouseLeftButtonDown, AddressOf Img_MouseLeftButtonDown
                            AddHandler img1.Delete, AddressOf DeleteButton
                            cvs.Children.Add(img1)

                            grid_Telecommande.Children.Add(cvs)
                        End If
                    Next
                End If
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, ex.ToString)
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
                slider_Column.Value = _list.Item(cbTemplate.SelectedIndex).Colonne
                slider_Row.Value = _list.Item(cbTemplate.SelectedIndex).Ligne
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur: " & ex.ToString)
        End Try
    End Sub

    Private Sub TxtCmdName_TextChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtCmdName.TextChanged
        BtnSaveCmd.Visibility = Windows.Visibility.Visible
    End Sub

    Private Sub TxtCmdRepeat_TextChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TxtCmdRepeat.TextChanged
        BtnSaveCmd.Visibility = Windows.Visibility.Visible
    End Sub
End Class
