Imports HoMIDom.HoMIDom.Telecommande
Imports HoMIDom.HoMIDom.Api

Public Class WTelecommandeNew

#Region "Variables"
    Dim FlagNewCmd As Boolean
    'Dim x As HoMIDom.HoMIDom.TemplateDevice = Nothing
    'Dim _SelectDriverIndex As Integer 'Index du driver sélectionné
    'Dim ListButton As New List(Of ImageButton)
    Dim _Row As Integer
    Dim _Col As Integer
    Dim _MyTemplate As HoMIDom.HoMIDom.Telecommande.Template = Nothing
    Dim _CurrentTemplate As HoMIDom.HoMIDom.Telecommande.Template = Nothing
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
        'ImgCommande2.Source = Nothing
        ' ImgCommande2.Tag = ""

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
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Numérique obligatoire pour repeat !!", "Erreur", "")
                Exit Sub
            End If
            If String.IsNullOrEmpty(TxtCmdName.Text) Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le nom de la commande est obligatoire !!", "Erreur", "")
                Exit Sub
            End If

            If _CurrentTemplate IsNot Nothing Then

                If FlagNewCmd = True Then 'nouvelle commande
                    Dim _cmd As New HoMIDom.HoMIDom.Telecommande.Commandes
                    With _cmd
                        .Name = TxtCmdName.Text
                        .Code = TxtCmdData.Text
                        .Repeat = TxtCmdRepeat.Text
                        '.Picture = ImgCommande2.Tag
                    End With
                    _CurrentTemplate.Commandes.Add(_cmd)
                Else 'modifier commande
                    Dim idx As Integer = ListCmd.SelectedIndex
                    If idx < 0 Then Exit Sub

                    With _CurrentTemplate.Commandes.Item(idx)
                        .Name = TxtCmdName.Text
                        .Code = TxtCmdData.Text
                        .Repeat = TxtCmdRepeat.Text
                        '.Picture = ImgCommande2.Tag
                    End With
                End If
                TxtCmdName.Text = ""
                TxtCmdData.Text = ""
                TxtCmdRepeat.Text = ""

                ListCmd.Items.Clear()
                For i2 As Integer = 0 To _CurrentTemplate.Commandes.Count - 1
                    ListCmd.Items.Add(_CurrentTemplate.Commandes.Item(i2).Name)
                Next
            End If

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
                _CurrentTemplate.Commandes.RemoveAt(ListCmd.SelectedIndex)
                Dim retour As String = myService.SaveTemplate(IdSrv, _CurrentTemplate)
                If retour <> "0" Then
                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur lors de l'enregistrement de la commande dans le template: " & retour, "Erreur", "")
                    Exit Sub
                Else
                    ListCmd.Items.Clear()
                    For i2 As Integer = 0 To _CurrentTemplate.Commandes.Count - 1
                        ListCmd.Items.Add(_CurrentTemplate.Commandes.Item(i2).Name)
                    Next

                End If
                BtnDelCmd.Visibility = Windows.Visibility.Hidden
                TxtCmdData.Text = ""
                TxtCmdName.Text = ""
                TxtCmdRepeat.Text = ""
                'ImgCommande2.Source = Nothing
                'ImgCommande2.Tag = Nothing
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
            If String.IsNullOrEmpty(TxtCmdName.Text) = False Then
                _CurrentTemplate.ExecuteCommand(IdSrv, TxtCmdName.Text, myService)
            End If
            'Dim retour As String = myService.TelecommandeSendCommand(IdSrv, _DeviceId, TxtCmdName.Text)
            'If retour <> 0 Then
            '    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, retour, "Erreur", "")
            'End If
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
            'If _Driver IsNot Nothing Then
            '    If _Driver.Enable = True And _Driver.IsConnect Then
            '        TxtCmdData.Text = myService.StartLearning(IdSrv, _Driver.ID)
            '    Else
            '        AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Impossible d'apprendre un code car le driver n'est pas activé ou n'est pas connecté", "Erreur", "")
            '    End If
            'Else
            '    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Impossible d'apprendre un code car le driver n'a pas été trouvé", "Erreur", "")
            'End If
        Catch Ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur BtnLearnCmd: " & Ex.Message, "Erreur", "")
        End Try
    End Sub

    'Private Sub BtnImg_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnImg.Click
    '    Try
    '        Dim frm As New WindowImg
    '        frm.ShowDialog()
    '        If frm.DialogResult.HasValue And frm.DialogResult.Value Then
    '            Dim retour As String = frm.FileName
    '            If String.IsNullOrEmpty(retour) = False Then
    '                ImgCommande2.Source = ConvertArrayToImage(myService.GetByteFromImage(retour))
    '                ImgCommande2.Tag = retour
    '                BtnSaveCmd.Visibility = Windows.Visibility.Visible
    '            End If
    '            frm.Close()
    '        Else
    '            frm.Close()
    '        End If
    '        frm = Nothing
    '    Catch ex As Exception
    '        AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub ImgCommande_MouseLeftButtonDown: " & ex.Message, "ERREUR", "")
    '    End Try
    'End Sub

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

            If _CurrentTemplate IsNot Nothing Then

                BtnSaveCmd.Visibility = Windows.Visibility.Collapsed
                BtnDelCmd.Visibility = Windows.Visibility.Visible
                BtnTstCmd.Visibility = Windows.Visibility.Visible
                TxtCmdName.Text = _CurrentTemplate.Commandes.Item(i).Name
                TxtCmdData.Text = _CurrentTemplate.Commandes.Item(i).Code
                TxtCmdRepeat.Text = _CurrentTemplate.Commandes.Item(i).Repeat

                'If String.IsNullOrEmpty(_CurrentTemplate.Commandes.Item(i).Picture) = False Then
                '    ImgCommande2.Source = ConvertArrayToImage(myService.GetByteFromImage(_CurrentTemplate.Commandes.Item(i).Picture))
                '    ImgCommande2.Tag = _CurrentTemplate.Commandes.Item(i).Picture
                '    ImgCommande2.Command = TxtCmdName.Text
                'End If

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
    'Private Sub ImgCommande_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ImgCommande2.MouseLeftButtonDown ', Canvas2.MouseLeftButtonDown
    '    For i As Integer = 0 To grid_Telecommande.Children.Count - 1
    '        Dim cvs As Canvas = grid_Telecommande.Children.Item(i)

    '        If cvs IsNot Nothing Then
    '            If cvs.Children.Count <> 0 Then
    '                Dim y As ImageButton = cvs.Children.Item(0)
    '                If ListCmd.SelectedValue = y.Command Then
    '                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Vous ne pouvez pas inclure cette commande dans la grille car elle y est déjà utilisée!", "Information", "")
    '                    Exit Sub
    '                End If

    '            End If
    '        End If
    '    Next

    '    If ImgCommande2.Source IsNot Nothing Then
    '        Dim effects As DragDropEffects
    '        Dim obj As New DataObject()
    '        obj.SetData(GetType(ImageButton), sender)
    '        effects = DragDrop.DoDragDrop(sender, obj, DragDropEffects.Copy Or DragDropEffects.Move)
    '    Else
    '        AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Vous ne pouvez pas inclure cette commande dans la grille car elle ne comporte aucune image!", "Information", "")
    '    End If
    'End Sub

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
        TxtTplName.Focus()
        BtnSaveTemplate.Visibility = Windows.Visibility.Visible
        StkTplBase.Visibility = Windows.Visibility.Visible
        TxtTplFab.IsReadOnly = False
        TxtTplMod.IsReadOnly = False
        TxtTplName.IsReadOnly = False
        TxtTplFab.Text = ""
        TxtTplName.Text = ""
        TxtTplMod.Text = ""
        cbTemplate.Text = ""
        RdHttp.IsEnabled = True
        RdIR.IsEnabled = True
        RdRS232.IsEnabled = True
    End Sub

    ''' <summary>
    ''' Sauvegarder Template
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BtnSaveTemplate_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnSaveTemplate.Click
        Try

            If String.IsNullOrEmpty(TxtTplFab.Text) Or HaveCaractSpecial(TxtTplFab.Text) Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le nom du fabricant est obligatoire et ne doit pas comporter de caractère spécial (%,-,!...) ", "Erreur", "")
                TxtTplFab.Focus()
                Exit Sub
            End If
            If String.IsNullOrEmpty(TxtTplMod.Text) Or HaveCaractSpecial(TxtTplMod.Text) Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le nom du modèle est obligatoire et ne doit pas comporter de caractère spécial (%,-,!...)", "Erreur", "")
                TxtTplMod.Focus()
                Exit Sub
            End If
            If String.IsNullOrEmpty(TxtTplName.Text) Or HaveCaractSpecial(TxtTplName.Text) Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Le nom du template est obligatoire et ne doit pas comporter de caractère spécial (%,-,!...)", "Erreur", "")
                TxtTplName.Focus()
                Exit Sub
            End If

            _CurrentTemplate = New HoMIDom.HoMIDom.Telecommande.Template

            With _CurrentTemplate
                .Name = TxtTplName.Text
                .Modele = TxtTplMod.Text
                .Fabricant = TxtTplFab.Text
            End With

            'If _Driver IsNot Nothing Then
            '    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "OK", "", "")
            'Else
            '    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "VIDE", "", "")
            'End If

            Dim _type As String = ""
            If RdHttp.IsChecked Then
                _CurrentTemplate.Type = 0
            ElseIf RdIR.IsChecked Then
                _CurrentTemplate.Type = 1
            ElseIf RdRS232.IsChecked Then
                _CurrentTemplate.Type = 2
            End If

            Dim retour As String = myService.CreateNewTemplate(_CurrentTemplate)
            If retour <> "0" Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur lors de la création du nouveau template: " & retour, "Erreur", "")
                Exit Sub
            Else

                StkNewTemplate.Visibility = Windows.Visibility.Collapsed
                BtnSaveTemplate.Visibility = Windows.Visibility.Hidden
                StkTplBase.Visibility = Windows.Visibility.Collapsed

                cbTemplate.Items.Clear()
                Dim _list As New List(Of HoMIDom.HoMIDom.Telecommande.Template)
                _list = myService.GetListOfTemplate
                For i As Integer = 0 To _list.Count - 1
                    Dim tpl As String = Replace(_list(i).File, ".xml", "")
                    cbTemplate.Items.Add(tpl)
                Next

                cbTemplate.SelectedValue = _CurrentTemplate.Name

                TxtTplFab.IsReadOnly = True
                TxtTplMod.IsReadOnly = True
                TxtTplName.IsReadOnly = True
                RdHttp.IsEnabled = False
                RdIR.IsEnabled = False
                RdRS232.IsEnabled = False
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur: " & ex.ToString)
        End Try
    End Sub


#End Region


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="TemplateName">Nom du template à afficher</param>
    ''' <param name="HaveDevice">Si le template est associé à un device (si oui on peut tester ou apprendre des commandes sinon non)</param>
    ''' <remarks></remarks>
    Public Sub New(Optional TemplateName As String = "", Optional HaveDevice As Boolean = False)

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        Try

            'Récupère la liste des template
            Dim _list As New List(Of HoMIDom.HoMIDom.Telecommande.Template)
            _list = myService.GetListOfTemplate

            Dim idx As Integer = -1
            For i As Integer = 0 To _list.Count - 1
                cbTemplate.Items.Add(_list(i).Name) 'ajoute le nom du template dans la liste
                If String.IsNullOrEmpty(TemplateName) = False Then
                    If TemplateName = _list(i).Name Then idx = i
                End If
            Next
            cbTemplate.SelectedIndex = idx

            BtnSaveCmd.Visibility = Windows.Visibility.Collapsed
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur lors de l'ouverture de la fenêtre d'édition:" & ex.ToString, "Erreur", "")
        End Try
    End Sub

#Region "Designer"


    '    Private Sub slider_Row_ValueChanged(ByVal sender As Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of Double)) Handles slider_Row.ValueChanged
    '        Try
    '            '==== Initalisation ====
    '            If Me.grid_Telecommande Is Nothing Then Exit Sub

    '            'Initialisation des lignes de la grille si celle-ci est vide
    '            If Me.grid_Telecommande.RowDefinitions IsNot Nothing Then
    '                If grid_Telecommande.RowDefinitions.Count = 0 Then
    '                    grid_Telecommande.RowDefinitions.Clear()
    '                    'grid_Telecommande.Height = 50
    '                    'Initialisation de la hauteur du background
    '                    'rectangle.Height = 70
    '                End If
    '            End If

    '            'Augmente la hauteur de la grille
    '            grid_Telecommande.Height = slider_Row.Value * 50
    '            'Augmente la hauteur du background
    '            rectangle.Height = slider_Row.Value * 50 + 20
    '            Caneva_grid.Height = slider_Row.Value * 50 + 60

    '            '==== Modification de la grille et du background pour chaque division du slider ====
    '            If slider_Row.Value > 1 Then
    '                'Si la grille contient moins de ligne que définit
    '                If Me.grid_Telecommande.RowDefinitions.Count < slider_Row.Value Then
    '                    Dim diff As Integer = slider_Row.Value - Me.grid_Telecommande.RowDefinitions.Count
    '                    For i As Integer = 1 To diff

    '                        'Ajoute une ligne à la grille
    '                        Dim rowDef As New RowDefinition
    '                        grid_Telecommande.RowDefinitions.Add(rowDef)

    '                        For j As Integer = 0 To grid_Telecommande.ColumnDefinitions.Count - 1
    '                            Dim x As New Canvas
    '                            x.Width = 45
    '                            x.Height = 45
    '                            x.Background = Brushes.Black
    '                            x.AllowDrop = True
    '                            x.Tag = grid_Telecommande.RowDefinitions.Count - 1 & "|" & j
    '                            AddHandler x.DragOver, AddressOf CVS_DragOver
    '                            AddHandler x.Drop, AddressOf CVS_Drop
    '                            Grid.SetColumn(x, j)
    '                            Grid.SetRow(x, grid_Telecommande.RowDefinitions.Count - 1)
    '                            grid_Telecommande.Children.Add(x)
    '                        Next

    '                    Next
    '                End If
    '                'Si la grille contient plus de ligne que définit
    '                If Me.grid_Telecommande.RowDefinitions.Count > slider_Row.Value Then
    '                    Dim diff As Integer = Me.grid_Telecommande.RowDefinitions.Count - slider_Row.Value
    '                    For i As Integer = 1 To diff
    '                        'Retire une ligne à la grille
    '                        grid_Telecommande.RowDefinitions.RemoveAt(Me.grid_Telecommande.RowDefinitions.Count - 1)
    '                    Next
    'Retour:
    '                    For i As Integer = 0 To grid_Telecommande.Children.Count - 1
    '                        Dim x As Canvas = grid_Telecommande.Children.Item(i)
    '                        Dim a() As String = Split(x.Tag, "|")
    '                        If a(0) > grid_Telecommande.RowDefinitions.Count - 1 Then
    '                            grid_Telecommande.Children.RemoveAt(i)
    '                            GoTo Retour
    '                        End If
    '                    Next
    '                End If
    '            End If

    '            'Remplir()
    '        Catch ex As Exception
    '            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur slider_Row_ValueChanged: " & ex.ToString)
    '        End Try
    '    End Sub

    '    Private Sub slider_Column_ValueChanged(ByVal sender As Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of Double)) Handles slider_Column.ValueChanged
    '        Try
    '            '==== Initalisation ====
    '            If Me.grid_Telecommande Is Nothing Then Exit Sub
    '            'Initialisation des colonnes de la grille si celle-ci est vide

    '            If grid_Telecommande.ColumnDefinitions IsNot Nothing Then
    '                If grid_Telecommande.ColumnDefinitions.Count = 0 Then
    '                    grid_Telecommande.ColumnDefinitions.Clear()
    '                    grid_Telecommande.Width = 50
    '                    'Initialisation de la largeur du background
    '                    'Me.rectangle.Width = 70
    '                End If
    '            End If

    '            'Augmente la largeur de la grille
    '            grid_Telecommande.Width = slider_Column.Value * 50
    '            'Augmente la largeur du background
    '            rectangle.Width = slider_Column.Value * 50 + 20
    '            Caneva_grid.Width = slider_Column.Value * 50 + 60

    '            '==== Modification de la grille et du background pour chaque division du slider ====
    '            If slider_Column.Value > 1 Then
    '                'Si la grille contient moins de ligne que définit
    '                If Me.grid_Telecommande.ColumnDefinitions.Count < slider_Column.Value Then
    '                    Dim diff As Integer = slider_Column.Value - Me.grid_Telecommande.ColumnDefinitions.Count
    '                    For i As Integer = 1 To diff

    '                        'Ajoute une colonne à la grille
    '                        Dim colDef As New ColumnDefinition
    '                        grid_Telecommande.ColumnDefinitions.Add(colDef)

    '                        For j As Integer = 0 To grid_Telecommande.RowDefinitions.Count - 1
    '                            Dim x As New Canvas
    '                            x.Width = 45
    '                            x.Height = 45
    '                            x.Background = Brushes.Black
    '                            x.AllowDrop = True
    '                            x.Tag = j & "|" & grid_Telecommande.ColumnDefinitions.Count - 1
    '                            AddHandler x.DragOver, AddressOf CVS_DragOver
    '                            AddHandler x.Drop, AddressOf CVS_Drop
    '                            Grid.SetColumn(x, grid_Telecommande.ColumnDefinitions.Count - 1)
    '                            Grid.SetRow(x, j)
    '                            grid_Telecommande.Children.Add(x)
    '                        Next
    '                    Next

    '                    Me.Width = grid_Telecommande.Width + 300
    '                End If

    '                'Si la grille contient plus de colonne que définit
    '                If Me.grid_Telecommande.ColumnDefinitions.Count > slider_Column.Value Then
    '                    Dim diff As Integer = Me.grid_Telecommande.ColumnDefinitions.Count - slider_Column.Value
    '                    For i As Integer = 1 To diff
    '                        'Retire une ligne à la grille
    '                        grid_Telecommande.ColumnDefinitions.RemoveAt(Me.grid_Telecommande.ColumnDefinitions.Count - 1)
    '                    Next
    'Retour:
    '                    For i As Integer = 0 To grid_Telecommande.Children.Count - 1
    '                        Dim x As Canvas = grid_Telecommande.Children.Item(i)
    '                        Dim a() As String = Split(x.Tag, "|")
    '                        If a(1) > grid_Telecommande.ColumnDefinitions.Count - 1 Then
    '                            grid_Telecommande.Children.RemoveAt(i)
    '                            GoTo Retour
    '                        End If
    '                    Next
    '                End If
    '            End If


    '            'Remplir()
    '        Catch ex As Exception
    '            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur slider_Column_ValueChanged: " & ex.ToString)
    '        End Try
    '    End Sub


    Private Sub Recharge()
        Try
            'Remplir()

            For i As Integer = 0 To _CurrentTemplate.Commandes.Count - 1
                Dim x As New Canvas
                x.Width = 45
                x.Height = 45
                x.Margin = New Thickness(15)
                x.HorizontalAlignment = Windows.HorizontalAlignment.Center
                x.VerticalAlignment = Windows.VerticalAlignment.Center
                x.Background = Brushes.Black
                x.AllowDrop = True
                x.Tag = _CurrentTemplate.Commandes(i).Row & "|" & _CurrentTemplate.Commandes(i).Column
                'AddHandler x.DragOver, AddressOf CVS_DragOver
                'AddHandler x.Drop, AddressOf CVS_Drop
                Grid.SetColumn(x, _CurrentTemplate.Commandes(i).Column)
                Grid.SetRow(x, _CurrentTemplate.Commandes(i).Row)

                Dim img1 As New ImageButton
                img1.Source = ConvertArrayToImage(myService.GetByteFromImage(_CurrentTemplate.Commandes(i).Picture))
                img1.Command = _CurrentTemplate.Commandes(i).Name
                img1.AllowDrop = True
                Dim a() As String = x.Tag.split("|")
                img1.Row = a(0)
                img1.Column = a(1)
                AddHandler img1.MouseLeftButtonDown, AddressOf Img_MouseLeftButtonDown
                AddHandler img1.Delete, AddressOf DeleteButton
                x.Children.Add(img1)

                'grid_Telecommande.Children.Add(x)
            Next
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub Telecommande Recharge: " & ex.Message, "ERREUR", "")
        End Try
    End Sub

    'Private Sub CVS_DragOver(ByVal sender As Object, ByVal e As System.Windows.DragEventArgs)
    '    Try
    '        If e.Data.GetDataPresent(GetType(ImageButton)) Then
    '            e.Effects = DragDropEffects.Copy
    '        Else
    '            e.Effects = DragDropEffects.None
    '        End If
    '    Catch ex As Exception
    '        AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub Telecommande CVS_DragOver: " & ex.Message, "ERREUR", "")
    '    End Try
    'End Sub

    'Private Sub CVS_Drop(ByVal sender As System.Object, ByVal e As System.Windows.DragEventArgs)
    '    Try
    '        If e.Data.GetDataPresent(GetType(ImageButton)) Then
    '            If sender.children.count > 0 Then
    '                Exit Sub
    '            End If

    '            e.Effects = DragDropEffects.Copy
    '            ' Utiliser uri comme vous le souhaitez
    '            Dim img1 As New ImageButton
    '            If InStr(e.Data.GetData(GetType(ImageButton)).parent.GetType.ToString, "Canvas") Then
    '                If e.Data.GetData(GetType(ImageButton)).name <> "ImgCommande" Then
    '                    e.Data.GetData(GetType(ImageButton)).parent.children.clear()
    '                End If
    '            End If
    '            img1.Source = e.Data.GetData(GetType(ImageButton)).source
    '            img1.Tag = e.Data.GetData(GetType(ImageButton)).Tag
    '            img1.AllowDrop = True
    '            If String.IsNullOrEmpty(ListCmd.SelectedValue) = False Then
    '                img1.Command = ListCmd.SelectedValue
    '                img1.ToolTip = ListCmd.SelectedValue
    '            Else
    '                img1.Command = e.Data.GetData(GetType(ImageButton)).Command
    '                img1.ToolTip = e.Data.GetData(GetType(ImageButton)).ToolTip
    '            End If

    '            Dim a() As String = sender.tag.split("|")
    '            img1.Row = a(0)
    '            img1.Column = a(1)
    '            img1.HorizontalAlignment = Windows.HorizontalAlignment.Center
    '            img1.VerticalAlignment = Windows.VerticalAlignment.Center

    '            AddHandler img1.MouseLeftButtonDown, AddressOf Img_MouseLeftButtonDown
    '            AddHandler img1.Delete, AddressOf DeleteButton
    '            sender.Children.Add(img1)

    '        Else
    '            e.Effects = DragDropEffects.None
    '        End If
    '    Catch ex As Exception
    '        AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub Telecommande CVS_Drop: " & ex.ToString, "ERREUR", "")
    '    End Try
    'End Sub

    'Private Sub Remplir()
    '    Try
    '        For i As Integer = 0 To grid_Telecommande.RowDefinitions.Count - 1
    '            For j As Integer = 0 To grid_Telecommande.ColumnDefinitions.Count - 1
    '                Dim x As New Canvas
    '                x.Width = 45
    '                x.Height = 45
    '                x.Background = Brushes.Black
    '                x.Margin = New Thickness(15)
    '                x.VerticalAlignment = Windows.VerticalAlignment.Center
    '                x.HorizontalAlignment = Windows.HorizontalAlignment.Center
    '                x.AllowDrop = True
    '                x.Tag = i & "|" & j
    '                AddHandler x.DragOver, AddressOf CVS_DragOver
    '                AddHandler x.Drop, AddressOf CVS_Drop
    '                Grid.SetColumn(x, j)
    '                Grid.SetRow(x, i)
    '                grid_Telecommande.Children.Add(x)
    '            Next
    '        Next
    '    Catch ex As Exception
    '        AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub Telecommande Remplir: " & ex.Message, "ERREUR", "")
    '    End Try
    'End Sub

    'Supprimer un bouton via menu contextuel
    Private Sub DeleteButton(ByVal sender As Object)
        Try
            sender.parent.children.clear()

            For i As Integer = 0 To _CurrentTemplate.Commandes.Count - 1
                If _CurrentTemplate.Commandes(i).Name = sender.command Then
                    _CurrentTemplate.Commandes(i).Row = -1
                    _CurrentTemplate.Commandes(i).Column = -1
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
            obj.SetData(GetType(ImageButton), sender)
            effects = DragDrop.DoDragDrop(sender, obj, DragDropEffects.Copy Or DragDropEffects.Move)
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub Telecommande Img_MouseLeftButtonDown: " & ex.Message, "ERREUR", "")
        End Try
    End Sub

    'Fermer
    Private Sub button_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles button.Click
        Try
            DialogResult = True
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "ERREUR Sub Telecommande button_Click: " & ex.Message, "ERREUR", "")
        End Try
    End Sub



#End Region

    Private Sub WTelecommande_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Try
            'slider_Column.Value = _Col
            'slider_Row.Value = _Row
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, ex.ToString)
        End Try
    End Sub

    ''' <summary>
    ''' Affiche les boutons (commandes) du template dans la grille
    ''' </summary>
    ''' <remarks></remarks>
    'Private Sub AfficheGrilleOfCurrentTemplate()
    '    Try
    '        If _CurrentTemplate IsNot Nothing Then
    '            If _CurrentTemplate.Commandes IsNot Nothing Then

    '                'Remplir()

    '                For i2 As Integer = 0 To _CurrentTemplate.Commandes.Count - 1

    '                    If _CurrentTemplate.Commandes.Item(i2).Row >= 0 And _CurrentTemplate.Commandes.Item(i2).Column >= 0 Then
    '                        Dim cvs As New Canvas
    '                        cvs.Width = 45
    '                        cvs.Height = 45
    '                        cvs.Background = Brushes.Black
    '                        cvs.AllowDrop = True
    '                        cvs.Tag = _CurrentTemplate.Commandes.Item(i2).Row & "|" & _CurrentTemplate.Commandes.Item(i2).Column
    '                        'AddHandler cvs.DragOver, AddressOf CVS_DragOver
    '                        'AddHandler cvs.Drop, AddressOf CVS_Drop
    '                        Grid.SetColumn(cvs, _CurrentTemplate.Commandes.Item(i2).Column)
    '                        Grid.SetRow(cvs, _CurrentTemplate.Commandes.Item(i2).Row)

    '                        Dim img1 As New ImageButton
    '                        img1.Source = ConvertArrayToImage(myService.GetByteFromImage(_CurrentTemplate.Commandes.Item(i2).Picture))
    '                        img1.Tag = _CurrentTemplate.Commandes.Item(i2).Picture
    '                        img1.Command = _CurrentTemplate.Commandes.Item(i2).Name

    '                        img1.AllowDrop = True
    '                        Dim a() As String = cvs.Tag.split("|")
    '                        img1.Row = a(0)
    '                        img1.Column = a(1)
    '                        img1.Width = 45
    '                        img1.Height = 45
    '                        img1.HorizontalAlignment = Windows.HorizontalAlignment.Center
    '                        img1.VerticalAlignment = Windows.VerticalAlignment.Center
    '                        AddHandler img1.MouseLeftButtonDown, AddressOf Img_MouseLeftButtonDown
    '                        AddHandler img1.Delete, AddressOf DeleteButton
    '                        cvs.Children.Add(img1)

    '                        grid_Telecommande.Children.Add(cvs)
    '                    End If
    '                Next
    '            End If
    '        End If
    '    Catch ex As Exception
    '        AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, ex.ToString)
    '    End Try
    'End Sub

    ''' <summary>
    ''' Sélection d'un template
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cbTemplate_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cbTemplate.SelectionChanged
        Try
            If cbTemplate.SelectedIndex < 0 Then
            Else
                'Récupère la liste des templates
                Dim _list As New List(Of HoMIDom.HoMIDom.Telecommande.Template)
                _list = myService.GetListOfTemplate

                _CurrentTemplate = _list.Item(cbTemplate.SelectedIndex)
                ChargeCmd()
                ChargeVar()

                TxtTplName.Text = _list.Item(cbTemplate.SelectedIndex).Name
                TxtTplFab.Text = _list.Item(cbTemplate.SelectedIndex).Fabricant
                TxtTplMod.Text = _list.Item(cbTemplate.SelectedIndex).Modele
                'slider_Column.Value = _list.Item(cbTemplate.SelectedIndex).Colonne
                ' slider_Row.Value = _list.Item(cbTemplate.SelectedIndex).Ligne

                Select Case _list.Item(cbTemplate.SelectedIndex).Type
                    Case 0 'http
                        RdHttp.IsChecked = True
                        BtnLearn.Visibility = Windows.Visibility.Collapsed
                    Case 1 'IR
                        RdIR.IsChecked = True
                        BtnLearn.Visibility = Windows.Visibility.Visible
                    Case 2 'RS232
                        RdRS232.IsChecked = True
                        BtnLearn.Visibility = Windows.Visibility.Visible
                End Select

                RdHttp.IsEnabled = False
                RdIR.IsEnabled = False
                RdRS232.IsEnabled = False

                StkCmd.Visibility = Windows.Visibility.Visible
                StkVar.Visibility = Windows.Visibility.Visible

                'AfficheGrilleOfCurrentTemplate()
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur: " & ex.ToString)
        End Try
    End Sub

    Private Sub ChargeCmd()
        Try
            ListCmd.Items.Clear()
            'ImgCommande2.Source = Nothing
            'ImgCommande2.Tag = Nothing

            If _CurrentTemplate IsNot Nothing Then
                For Each cmd In _CurrentTemplate.Commandes
                    ListCmd.Items.Add(cmd.Name)
                Next

            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur ChargeCmd: " & ex.Message)
        End Try
    End Sub

    Private Sub ChargeVar()
        Try
            ListVar.Items.Clear()

            If _CurrentTemplate IsNot Nothing Then
                For Each var In _CurrentTemplate.Variables
                    ListVar.Items.Add(var.Name)
                Next
            Else

            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur ChargeVar: " & ex.Message)
        End Try
    End Sub

    Private Sub TxtCmdName_TextChanged(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles TxtCmdName.MouseDown
        BtnSaveCmd.Visibility = Windows.Visibility.Visible
    End Sub

    Private Sub TxtCmdRepeat_TextChanged(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles TxtCmdRepeat.MouseDown
        BtnSaveCmd.Visibility = Windows.Visibility.Visible
    End Sub

#Region "Variable"
    Dim FlagNewVar As Boolean

    Private Sub BtnNewVar_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnNewVar.Click
        StkParamVar.Visibility = Windows.Visibility.Visible
        TxtVarName.IsEnabled = True
        TxtVarName.Text = ""
        TxtVarVal.Text = ""
        CbVarType.SelectedIndex = 0
        FlagNewVar = True
    End Sub

    Private Sub ListVar_SelectionChanged(ByVal sender As System.Object, ByVal e As Object) Handles ListVar.SelectionChanged, ListVar.MouseLeftButtonDown
        Try
            FlagNewVar = False
            If _CurrentTemplate IsNot Nothing Then
                For Each _var In _CurrentTemplate.Variables
                    If _var.Name = ListVar.SelectedValue Then
                        TxtVarName.Text = _var.Name
                        TxtVarName.IsEnabled = False
                        TxtVarVal.Text = _var.Value
                        CbVarType.SelectedIndex = _var.Type
                        StkParamVar.Visibility = Windows.Visibility.Visible
                        Exit For
                    End If
                Next
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur ListVar_SelectionChanged: " & ex.Message, "Erreur", "")
        End Try
    End Sub

    Private Sub BtnAnnulVar_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnAnnulVar.Click
        Try
            FlagNewVar = False
            StkParamVar.Visibility = Windows.Visibility.Collapsed
            TxtVarName.Text = ""
            TxtVarVal.Text = ""
            CbVarType.SelectedIndex = 0
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur BtnAnnulVar_Click: " & ex.Message, "Erreur", "")
        End Try
    End Sub

    Private Sub BtnDeleteVar_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDeleteVar.Click
        Try
            FlagNewVar = False

            If ListVar.SelectedIndex < 0 Then
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Veuillez sélectionner une variable à supprimer!", "Erreur", "")
                Exit Sub
            End If

            If _CurrentTemplate IsNot Nothing Then
                Dim idx As Integer = 0
                For Each var In _CurrentTemplate.Variables
                    If var.Name = ListVar.SelectedValue Then
                        _CurrentTemplate.Variables.RemoveAt(idx)
                        Exit For
                    End If
                    idx += 1
                Next

                ChargeVar()
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur BtnDeleteVar_Click: " & ex.Message, "Erreur", "")
        End Try
    End Sub

    Private Sub BtnModifVar_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnModifVar.Click
        Try
            If FlagNewVar Then
                TxtVarName.IsEnabled = True
            Else
                TxtVarName.IsEnabled = False

                If ListVar.SelectedIndex < 0 Then
                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Veuillez sélectionner une variable à modifier!", "Erreur", "")
                    Exit Sub
                End If
            End If

            If _CurrentTemplate IsNot Nothing Then
                If FlagNewVar Then
                    For Each var In _CurrentTemplate.Variables
                        If var.Name.ToUpper = TxtVarName.Text.ToUpper Then
                            MessageBox.Show("Vous ne pouvez pas utiliser ce nom de variable car il existe déjà !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
                            Exit For
                        End If
                    Next

                    Dim v As New TemplateVar
                    With v
                        .Name = TxtVarName.Text
                        .Type = CbVarType.SelectedIndex
                        .Value = TxtVarVal.Text
                    End With
                    _CurrentTemplate.Variables.Add(v)

                Else
                    Dim idx As Integer = 0
                    For Each var In _CurrentTemplate.Variables
                        If var.Name = TxtVarName.Text Then
                            var.Type = CbVarType.SelectedIndex
                            var.Value = TxtVarVal.Text
                            Exit For
                        End If
                        idx += 1
                    Next
                End If


                ChargeVar()
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur BtnModifVar_Click: " & ex.Message, "Erreur", "")
        End Try
    End Sub
#End Region

    Private Sub buttonOk_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles buttonOk.Click
        Try
            If _CurrentTemplate IsNot Nothing Then
                'For i As Integer = 0 To grid_Telecommande.Children.Count - 1
                '    Dim cvs As Canvas = grid_Telecommande.Children.Item(i)

                '    If cvs IsNot Nothing Then
                '        If cvs.Children.Count <> 0 Then
                '            For j As Integer = 0 To _CurrentTemplate.Commandes.Count - 1

                '                Dim y As ImageButton = cvs.Children.Item(0)
                '                If _CurrentTemplate.Commandes(j).Name = y.Command Then
                '                    _CurrentTemplate.Commandes(j).Row = y.Row
                '                    _CurrentTemplate.Commandes(j).Column = y.Column
                '                    Exit For
                '                End If
                '            Next
                '        End If
                '    End If
                'Next

                Dim retour As String = myService.SaveTemplate(IdSrv, _CurrentTemplate)

                If retour <> "0" Then
                    AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur lors de l'enregistrement de la commande dans le template: " & retour, "Erreur", "")
                End If
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur buttonOk_Click: " & ex.Message, "Erreur", "buttonOk_Click")
        End Try
    End Sub
End Class
