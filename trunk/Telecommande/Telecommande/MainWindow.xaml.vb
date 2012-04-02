Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents

Partial Public Class MainWindow
    Inherits Window

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
        Dim i As Integer
        i = 1
        'Initialisation des lignes de la grille
        Me.grid_Telecommande.RowDefinitions.Clear()
        Me.grid_Telecommande.Height = 50
        'Initialisation de la hauteur du background
        Me.rectangle.Height = 70

        '==== Modification de la grille et du background pour chaque division du slider ====
        If slider_Row.Value > 1 Then
            For i = 1 To slider_Row.Value
                'Augmente la hauteur de la grille
                grid_Telecommande.Height = slider_Row.Value * 50
                'Augmente la hauteur du background
                rectangle.Height = slider_Row.Value * 50 + 20
                'Ajoute une ligne à la grille
                Dim rowDef As New RowDefinition
                grid_Telecommande.RowDefinitions.Add(rowDef)
            Next
        End If

    End Sub

    Private Sub slider_Column_PreviewMouseMove(sender As Object, e As System.Windows.Input.MouseEventArgs) Handles slider_Column.PreviewMouseMove

        '==== Initalisation ====
        Dim j As Integer
        j = 1
        'Initialisation des colonnes de la grille
        Me.grid_Telecommande.ColumnDefinitions.Clear()
        Me.grid_Telecommande.Width = 50
        'Initialisation de la largeur du background
        Me.rectangle.Width = 70

        '==== Modification de la grille et du background pour chaque division du slider ====
        If slider_Column.Value > 1 Then
            For j = 1 To slider_Column.Value
                'Augmente la largeur de la grille
                grid_Telecommande.Width = slider_Column.Value * 50
                'Augmente la largeur du background
                rectangle.Width = slider_Column.Value * 50 + 20
                'Ajoute une colonne à la grille
                Dim colDef As New ColumnDefinition
                grid_Telecommande.ColumnDefinitions.Add(colDef)
            Next
        End If

    End Sub

    Private Sub img_Source_MouseMove(sender As Object, e As System.Windows.Input.MouseEventArgs) Handles img_Source.MouseMove
        If e.LeftButton = MouseButtonState.Pressed Then
            Dim effects As DragDropEffects
            Dim obj As New DataObject()
            obj.SetData(GetType(Image), img_Source.Children)
            effects = DragDrop.DoDragDrop(img_Source, obj, DragDropEffects.Copy Or DragDropEffects.Move) ' 
        End If
    End Sub

    Private Sub grid_Telecommande_DragOver(sender As Object, e As System.Windows.DragEventArgs) Handles grid_Telecommande.DragOver
        If e.Data.GetDataPresent(GetType(Image)) Then
            e.Effects = DragDropEffects.Copy
            ' Utiliser uri comme vous le souhaitez

            Dim img1 As New Image
            Dim bi1 As New BitmapImage
            bi1.BeginInit()
            bi1.UriSource = New Uri((DirectCast(e.Data.GetData(GetType(Image)), String)), UriKind.Relative)
            bi1.EndInit()
            img1.Source = bi1
            grid_Telecommande.Children.Add(img1)

        Else
            e.Effects = DragDropEffects.None
        End If

    End Sub
End Class