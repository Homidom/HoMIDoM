Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Controls.Primitives
Imports System.Windows.Documents
Imports System.Windows.Media
Imports System.Collections.Generic

Namespace Designer

    Public Class ResizeThumb
        Inherits Thumb

        Private rotateTransform As RotateTransform
        Private angle As Double
        Private adorner As Adorner
        Private transformOrigin As Point
        Private designerItem As ContentControl
        Private canvas As Canvas

        Public Sub New()
            'Dim DragStarted As DragStartedEventHandler
            AddHandler DragStarted, AddressOf ResizeThumb_DragStarted
            'Dim DragDelta As DragDeltaEventHandler
            AddHandler DragDelta, AddressOf ResizeThumb_DragDelta
            'Dim DragCompleted As DragCompletedEventHandler
            AddHandler DragCompleted, AddressOf ResizeThumb_DragCompleted
        End Sub

        Private Sub ResizeThumb_DragStarted(ByVal sender As Object, ByVal e As DragStartedEventArgs)
            Me.designerItem = TryCast(Me.DataContext, ContentControl)

            If Me.designerItem IsNot Nothing Then
                Me.canvas = TryCast(VisualTreeHelper.GetParent(Me.designerItem), Canvas)

                If Me.canvas IsNot Nothing Then
                    Me.transformOrigin = Me.designerItem.RenderTransformOrigin

                    Me.rotateTransform = TryCast(Me.designerItem.RenderTransform, RotateTransform)
                    If Me.rotateTransform IsNot Nothing Then
                        Me.angle = Me.rotateTransform.Angle * Math.PI / 180.0
                    Else
                        Me.angle = 0.0
                    End If

                    Dim adornerLayer__1 As AdornerLayer = AdornerLayer.GetAdornerLayer(Me.canvas)
                    If adornerLayer__1 IsNot Nothing Then
                        Me.adorner = New SizeAdorner(Me.designerItem)
                        adornerLayer__1.Add(Me.adorner)
                    End If
                End If
            End If
        End Sub

        Private Sub ResizeThumb_DragDelta(ByVal sender As Object, ByVal e As DragDeltaEventArgs)
            If Me.designerItem IsNot Nothing Then
                Dim deltaVertical As Double, deltaHorizontal As Double

                Select Case VerticalAlignment
                    Case System.Windows.VerticalAlignment.Bottom
                        deltaVertical = Math.Min(-e.VerticalChange, Me.designerItem.ActualHeight - Me.designerItem.MinHeight)
                        canvas.SetTop(Me.designerItem, canvas.GetTop(Me.designerItem) + (Me.transformOrigin.Y * deltaVertical * (1 - Math.Cos(-Me.angle))))
                        canvas.SetLeft(Me.designerItem, canvas.GetLeft(Me.designerItem) - deltaVertical * Me.transformOrigin.Y * Math.Sin(-Me.angle))
                        Me.designerItem.Height -= deltaVertical
                        Exit Select
                    Case System.Windows.VerticalAlignment.Top
                        deltaVertical = Math.Min(e.VerticalChange, Me.designerItem.ActualHeight - Me.designerItem.MinHeight)
                        canvas.SetTop(Me.designerItem, canvas.GetTop(Me.designerItem) + deltaVertical * Math.Cos(-Me.angle) + (Me.transformOrigin.Y * deltaVertical * (1 - Math.Cos(-Me.angle))))
                        canvas.SetLeft(Me.designerItem, canvas.GetLeft(Me.designerItem) + deltaVertical * Math.Sin(-Me.angle) - (Me.transformOrigin.Y * deltaVertical * Math.Sin(-Me.angle)))
                        Me.designerItem.Height -= deltaVertical
                        Exit Select
                    Case Else
                        Exit Select
                End Select

                Select Case HorizontalAlignment
                    Case System.Windows.HorizontalAlignment.Left
                        deltaHorizontal = Math.Min(e.HorizontalChange, Me.designerItem.ActualWidth - Me.designerItem.MinWidth)
                        canvas.SetTop(Me.designerItem, canvas.GetTop(Me.designerItem) + deltaHorizontal * Math.Sin(Me.angle) - Me.transformOrigin.X * deltaHorizontal * Math.Sin(Me.angle))
                        canvas.SetLeft(Me.designerItem, canvas.GetLeft(Me.designerItem) + deltaHorizontal * Math.Cos(Me.angle) + (Me.transformOrigin.X * deltaHorizontal * (1 - Math.Cos(Me.angle))))
                        Me.designerItem.Width -= deltaHorizontal
                        Exit Select
                    Case System.Windows.HorizontalAlignment.Right
                        deltaHorizontal = Math.Min(-e.HorizontalChange, Me.designerItem.ActualWidth - Me.designerItem.MinWidth)
                        canvas.SetTop(Me.designerItem, canvas.GetTop(Me.designerItem) - Me.transformOrigin.X * deltaHorizontal * Math.Sin(Me.angle))
                        canvas.SetLeft(Me.designerItem, canvas.GetLeft(Me.designerItem) + (deltaHorizontal * Me.transformOrigin.X * (1 - Math.Cos(Me.angle))))
                        Me.designerItem.Width -= deltaHorizontal
                        Exit Select
                    Case Else
                        Exit Select
                End Select
            End If

            e.Handled = True
        End Sub

        Private Sub ResizeThumb_DragCompleted(ByVal sender As Object, ByVal e As DragCompletedEventArgs)
            If Me.adorner IsNot Nothing Then
                Dim adornerLayer__1 As AdornerLayer = AdornerLayer.GetAdornerLayer(Me.canvas)
                If adornerLayer__1 IsNot Nothing Then
                    adornerLayer__1.Remove(Me.adorner)
                End If

                Me.adorner = Nothing
            End If
        End Sub
    End Class

End Namespace