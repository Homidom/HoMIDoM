Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents

Partial Public Class MainWindow
    Inherits Window

    Private ListButton As New List(Of ImageButton)

    Private Sub button_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles button.Click
        Dim img1 As New Image
        Dim bi1 As New BitmapImage
        Static Dim state As Boolean

        bi1.BeginInit()
        bi1.UriSource = New Uri("\Images\1.png", UriKind.Relative)
        bi1.EndInit()
        img1.Source = bi1

        If state = False Then
            Grid.SetColumn(img1, 0)
            Grid.SetRow(img1, 0)
            grid_Telecommande.Children.Add(img1)
            state = True
        Else
            Me.grid_Telecommande.Children.Remove(img1)
            state = False
        End If
    End Sub

    Private Sub slider_Row_ValueChanged(sender As Object, e As System.Windows.RoutedPropertyChangedEventArgs(Of Double)) Handles slider_Row.ValueChanged
        '==== Initalisation ====
        'Initialisation des lignes de la grille
        Me.grid_Telecommande.RowDefinitions.Clear()
        Me.grid_Telecommande.Height = 50
        'Initialisation de la hauteur du background
        Me.rectangle.Height = 70

        '==== Modification de la grille et du background pour chaque division du slider ====
        If slider_Row.Value > 1 Then
            For i As Integer = 1 To slider_Row.Value
                'Augmente la hauteur de la grille
                grid_Telecommande.Height = slider_Row.Value * 50
                'Augmente la hauteur du background
                rectangle.Height = slider_Row.Value * 50 + 20
                'Ajoute une ligne à la grille
                Dim rowDef As New RowDefinition
                grid_Telecommande.RowDefinitions.Add(rowDef)
            Next
        End If

        Remplir()
    End Sub

    Private Sub slider_Column_PreviewMouseMove(sender As Object, e As System.Windows.Input.MouseEventArgs) Handles slider_Column.PreviewMouseMove

        '==== Initalisation ====
        'Initialisation des colonnes de la grille
        Me.grid_Telecommande.ColumnDefinitions.Clear()
        Me.grid_Telecommande.Width = 50
        'Initialisation de la largeur du background
        Me.rectangle.Width = 70

        '==== Modification de la grille et du background pour chaque division du slider ====
        If slider_Column.Value > 1 Then
            For j As Integer = 1 To slider_Column.Value
                'Augmente la largeur de la grille
                grid_Telecommande.Width = slider_Column.Value * 50
                'Augmente la largeur du background
                rectangle.Width = slider_Column.Value * 50 + 20
                'Ajoute une colonne à la grille
                Dim colDef As New ColumnDefinition
                grid_Telecommande.ColumnDefinitions.Add(colDef)
            Next
        End If

        Remplir()
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

        MessageBox.Show(ListButton.Count & " button(s) enregistré(s)")
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
            img1.AllowDrop = True
            img1.ToolTip = x.Tag
            Dim a() As String = x.Tag.split("|")
            img1.Row = a(0)
            img1.Column = a(1)
            AddHandler img1.MouseLeftButtonDown, AddressOf Img_MouseLeftButtonDown
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
            img1.ToolTip = sender.tag
            Dim a() As String = sender.tag.split("|")
            img1.Row = a(0)
            img1.Column = a(1)
            AddHandler img1.MouseLeftButtonDown, AddressOf Img_MouseLeftButtonDown
            sender.Children.Add(img1)
        Else
            e.Effects = DragDropEffects.None
        End If
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

    Private Sub MainWindow_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Try
            For i As Integer = 0 To img_Source.Children.Count - 1
                Dim x As Image = img_Source.Children.Item(i)
                x.AllowDrop = True
                AddHandler x.MouseLeftButtonDown, AddressOf Img_MouseLeftButtonDown
            Next
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub Img_MouseLeftButtonDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
        Dim effects As DragDropEffects
        Dim obj As New DataObject()
        obj.SetData(GetType(Image), sender)
        effects = DragDrop.DoDragDrop(sender, obj, DragDropEffects.Copy Or DragDropEffects.Move)
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button1.Click
        Save()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button2.Click
        Recharge()
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button3.Click
        grid_Telecommande.Children.Clear()
    End Sub
End Class