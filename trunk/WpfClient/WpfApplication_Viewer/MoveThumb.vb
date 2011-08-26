Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Controls.Primitives
Imports System.Windows.Media

Namespace Designer

    Public Class MoveThumb
        Inherits Thumb

        Private rotateTransform As RotateTransform
        Private designerItem As ContentControl

        Public Sub New()
            Dim DragStarted As DragStartedEventHandler
            AddHandler DragStarted, AddressOf Me.MoveThumb_DragStarted
            Dim DragDelta As DragDeltaEventHandler
            AddHandler DragDelta, AddressOf Me.MoveThumb_DragDelta
        End Sub

        Private Sub MoveThumb_DragStarted(ByVal sender As Object, ByVal e As DragStartedEventArgs)
            Me.designerItem = TryCast(DataContext, ContentControl)

            If Me.designerItem IsNot Nothing Then
                Me.rotateTransform = TryCast(Me.designerItem.RenderTransform, RotateTransform)
            End If
        End Sub

        Private Sub MoveThumb_DragDelta(ByVal sender As Object, ByVal e As DragDeltaEventArgs)
            If Me.designerItem IsNot Nothing Then
                Dim dragDelta As New Point(e.HorizontalChange, e.VerticalChange)

                If Me.rotateTransform IsNot Nothing Then
                    dragDelta = Me.rotateTransform.Transform(dragDelta)
                End If

                Canvas.SetLeft(Me.designerItem, Canvas.GetLeft(Me.designerItem) + dragDelta.X)
                Canvas.SetTop(Me.designerItem, Canvas.GetTop(Me.designerItem) + dragDelta.Y)
            End If
        End Sub
    End Class

End Namespace