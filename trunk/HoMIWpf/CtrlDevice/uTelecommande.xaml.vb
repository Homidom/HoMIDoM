Imports HoMIDom.HoMIDom

Public Class uTelecommande

    Dim _obj As TemplateDevice = Nothing
    Dim _ActualTemplate As Telecommande.Template = Nothing

    Public Event SendCommand(ByVal Command As String)

    Public Sub New(ByVal IdDevice As String)

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        _obj = myService.ReturnDeviceByID(IdSrv, IdDevice)

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        If _obj IsNot Nothing Then
            Dim _listTemplate As New List(Of Telecommande.Template)
            _listTemplate = myService.GetListOfTemplate

            For i As Integer = 0 To _listTemplate.Count - 1
                Dim tpl As String = Replace(_listTemplate(i).File, ".xml", "")
                If _obj.Modele = tpl Then
                    _ActualTemplate = _listTemplate(i)
                    Exit For
                End If
            Next
        End If
    End Sub

    Private Sub uTelecommande_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Try
            If _ActualTemplate IsNot Nothing Then
                SetGrid(_ActualTemplate.Ligne, _ActualTemplate.Colonne)
                Remplir()

                If _obj.Commandes IsNot Nothing Then

                    For i2 As Integer = 0 To _obj.Commandes.Count - 1

                        If _obj.Commandes.Item(i2).Row > 0 And _obj.Commandes.Item(i2).Column > 0 Then
                            Dim cvs As New Canvas
                            cvs.Width = 45
                            cvs.Height = 45

                            cvs.Background = Brushes.Black
                            cvs.AllowDrop = True
                            cvs.Tag = _obj.Commandes.Item(i2).Row & "|" & _obj.Commandes.Item(i2).Column
                            Grid.SetColumn(cvs, _obj.Commandes.Item(i2).Column)
                            Grid.SetRow(cvs, _obj.Commandes.Item(i2).Row)

                            Dim img1 As New ImageButton
                            img1.Source = ConvertArrayToImage(myService.GetByteFromImage(_obj.Commandes.Item(i2).Picture))
                            img1.Tag = _obj.Commandes.Item(i2).Picture
                            img1.Command = _obj.Commandes.Item(i2).Name

                            img1.AllowDrop = True
                            Dim a() As String = cvs.Tag.split("|")
                            img1.Row = a(0)
                            img1.Column = a(1)
                            img1.HorizontalAlignment = Windows.HorizontalAlignment.Stretch
                            img1.VerticalAlignment = Windows.VerticalAlignment.Stretch
                            AddHandler img1.MouseLeftButtonDown, AddressOf Img_MouseLeftButtonDown
                            cvs.Children.Add(img1)

                            grid_Telecommande.Children.Add(cvs)
                        End If
                    Next
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub Img_MouseLeftButtonDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
        Dim img1 As ImageButton = sender
        RaiseEvent SendCommand(img1.Command)
    End Sub


    Private Sub SetGrid(ByVal Row As Integer, ByVal Column As Integer)
        Try
            '==== Initalisation ====
            If Me.grid_Telecommande Is Nothing Then Exit Sub

            'Initialisation des lignes de la grille si celle-ci est vide
            If Me.grid_Telecommande.RowDefinitions IsNot Nothing Then
                If grid_Telecommande.RowDefinitions.Count = 0 Then
                    grid_Telecommande.RowDefinitions.Clear()
                    grid_Telecommande.Height = 50
                End If
            End If

            If grid_Telecommande.ColumnDefinitions IsNot Nothing Then
                If grid_Telecommande.ColumnDefinitions.Count = 0 Then
                    grid_Telecommande.ColumnDefinitions.Clear()
                    grid_Telecommande.Width = 50
                End If
            End If

            'Augmente la hauteur de la grille
            grid_Telecommande.Height = Row * 50
            grid_Telecommande.Width = Column * 50
            'Augmente la hauteur du background
            Rectangle.Width = Column * 50 + 20
            Rectangle.Height = Row * 50 + 20
            Caneva_grid.Height = Row * 50 + 60
            Caneva_grid.Width = Column * 50 + 60

            '==== Modification de la grille et du background pour chaque division du slider ====
            If Row > 1 Then
                'Si la grille contient moins de ligne que définit
                If Me.grid_Telecommande.RowDefinitions.Count < Row Then
                    Dim diff As Integer = Row - Me.grid_Telecommande.RowDefinitions.Count
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
                            Grid.SetColumn(x, j)
                            Grid.SetRow(x, grid_Telecommande.RowDefinitions.Count - 1)
                            grid_Telecommande.Children.Add(x)
                        Next

                    Next
                End If
                'Si la grille contient plus de ligne que définit
                If Me.grid_Telecommande.RowDefinitions.Count > Row Then
                    Dim diff As Integer = Me.grid_Telecommande.RowDefinitions.Count - Row
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

            '==== Modification de la grille et du background pour chaque division du slider ====
            If Column > 1 Then
                'Si la grille contient moins de ligne que définit
                If Me.grid_Telecommande.ColumnDefinitions.Count < Column Then
                    Dim diff As Integer = Column - Me.grid_Telecommande.ColumnDefinitions.Count
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
                            Grid.SetColumn(x, grid_Telecommande.ColumnDefinitions.Count - 1)
                            Grid.SetRow(x, j)
                            grid_Telecommande.Children.Add(x)
                        Next
                    Next

                    Me.Width = grid_Telecommande.Width + 300
                End If

                'Si la grille contient plus de colonne que définit
                If Me.grid_Telecommande.ColumnDefinitions.Count > Column Then
                    Dim diff As Integer = Me.grid_Telecommande.ColumnDefinitions.Count - Column
                    For i As Integer = 1 To diff
                        'Retire une ligne à la grille
                        grid_Telecommande.ColumnDefinitions.RemoveAt(Me.grid_Telecommande.ColumnDefinitions.Count - 1)
                    Next
Retour2:
                    For i As Integer = 0 To grid_Telecommande.Children.Count - 1
                        Dim x As Canvas = grid_Telecommande.Children.Item(i)
                        Dim a() As String = Split(x.Tag, "|")
                        If a(1) > grid_Telecommande.ColumnDefinitions.Count - 1 Then
                            grid_Telecommande.Children.RemoveAt(i)
                            GoTo Retour2
                        End If
                    Next
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur SetGrid: " & ex.Message)
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
                Grid.SetColumn(x, j)
                Grid.SetRow(x, i)
                grid_Telecommande.Children.Add(x)
            Next
        Next
    End Sub

End Class
