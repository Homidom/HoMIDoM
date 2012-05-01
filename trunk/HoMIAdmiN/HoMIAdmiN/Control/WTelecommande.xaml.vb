Public Class WTelecommande

#Region "Variables"
    Dim FlagNewCmd As Boolean
    Dim x As HoMIDom.HoMIDom.TemplateDevice = Nothing
    Dim _DeviceId As String 'Id du device à modifier
    Dim _Driver As HoMIDom.HoMIDom.TemplateDriver
    Dim _SelectDriverIndex As Integer 'Index du driver sélectionné
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

                Dim retour As String = myService.SaveTemplate(IdSrv, cbTemplate.Text, x.Commandes)
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
                Dim retour As String = myService.SaveTemplate(IdSrv, cbTemplate.Text, x.Commandes)
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

#End Region


#Region "Gestion des Templates"
    ''' <summary>
    ''' Nouveau template
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BtnNewTemplate_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnNewTemplate.Click
        StkNewTemplate.Height = Double.NaN
        TxtTplFab.Focus()
        BtnSaveTemplate.Visibility = Windows.Visibility.Visible
        lbltplbase.Visibility = Windows.Visibility.Visible
        cbBase.Visibility = Windows.Visibility.Visible
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

            Dim retour As String = myService.CreateNewTemplate(TxtTplFab.Text, TxtTplMod.Text, myService.GetAllDrivers(IdSrv).Item(_SelectDriverIndex).Protocol, cbBase.SelectedIndex)
            If retour <> "0" Then
                MessageBox.Show("Erreur lors de la création du nouveau template: " & retour, "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Exit Sub
            Else

                StkNewTemplate.Height = 0
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
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur: " & ex.ToString)
        End Try
    End Sub

#End Region


    Public Sub New(Optional ByVal DeviceId As String = "", Optional ByVal SelectDriverIndex As Integer = -1, Optional ByVal Driver As HoMIDom.HoMIDom.TemplateDriver = Nothing, Optional ByVal Template As HoMIDom.HoMIDom.TemplateDevice = Nothing)

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        _DeviceId = DeviceId
        _SelectDriverIndex = SelectDriverIndex
        _Driver = Driver
        x = Template

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        ListCmd.Items.Clear()
        StkCmd.Height = 0

        If _Driver.Protocol = "IR" Then
            BtnLearn.Visibility = Windows.Visibility.Visible
        Else
            BtnLearn.Visibility = Windows.Visibility.Hidden
        End If

        Dim _list As New List(Of HoMIDom.HoMIDom.Telecommande.Template)
        _list = myService.GetListOfTemplate
        Dim idx As Integer = -1
        For i As Integer = 0 To _list.Count - 1
            Dim tpl As String = Replace(_list(i).File, ".xml", "")
            cbTemplate.Items.Add(tpl)
            If x.Modele IsNot Nothing Then
                If tpl = x.Modele.ToString Then
                    StkCmd.Height = Double.NaN
                    cbTemplate.IsEnabled = False
                    BtnNewTemplate.Visibility = Windows.Visibility.Hidden
                    idx = i
                    If x.Commandes IsNot Nothing Then
                        For i2 As Integer = 0 To x.Commandes.Count - 1
                            ListCmd.Items.Add(x.Commandes.Item(i2).Name)
                        Next
                    End If
                End If
            End If
        Next
        cbTemplate.SelectedIndex = idx


    End Sub
End Class
