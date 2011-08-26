Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Controls.Primitives
Imports System.Windows.Input
Imports System.Windows.Media

Namespace Designer

    Public Class RotateThumb
        Inherits Thumb
        Private initialAngle As Double
        Private rotateTransform As RotateTransform
        Private startVector As Vector
        Private centerPoint As Point
        Private designerItem As ContentControl
        Private canvas As Canvas

        Public Sub New()
            Dim DragDelta As DragDeltaEventHandler
            AddHandler DragDelta, (AddressOf Me.RotateThumb_DragDelta)
            Dim DragStarted As DragStartedEventHandler
            AddHandler DragStarted, (AddressOf Me.RotateThumb_DragStarted)
        End Sub

        Private Sub RotateThumb_DragStarted(ByVal sender As Object, ByVal e As DragStartedEventArgs)
            Me.designerItem = TryCast(DataContext, ContentControl)

            If Me.designerItem IsNot Nothing Then
                Me.canvas = TryCast(VisualTreeHelper.GetParent(Me.designerItem), Canvas)

                If Me.canvas IsNot Nothing Then
                    Me.centerPoint = Me.designerItem.TranslatePoint(New Point(Me.designerItem.Width * Me.designerItem.RenderTransformOrigin.X, Me.designerItem.Height * Me.designerItem.RenderTransformOrigin.Y), Me.canvas)

                    Dim startPoint As Point = Mouse.GetPosition(Me.canvas)
                    Me.startVector = Point.Subtract(startPoint, Me.centerPoint)

                    Me.rotateTransform = TryCast(Me.designerItem.RenderTransform, RotateTransform)
                    If Me.rotateTransform Is Nothing Then
                        Me.designerItem.RenderTransform = New RotateTransform(0)
                        Me.initialAngle = 0
                    Else
                        Me.initialAngle = Me.rotateTransform.Angle
                    End If
                End If
            End If
        End Sub

        Private Sub RotateThumb_DragDelta(ByVal sender As Object, ByVal e As DragDeltaEventArgs)
            If Me.designerItem IsNot Nothing AndAlso Me.canvas IsNot Nothing Then
                Dim currentPoint As Point = Mouse.GetPosition(Me.canvas)
                Dim deltaVector As Vector = Point.Subtract(currentPoint, Me.centerPoint)

                Dim angle As Double = Vector.AngleBetween(Me.startVector, deltaVector)

                Dim rotateTransform As RotateTransform = TryCast(Me.designerItem.RenderTransform, RotateTransform)
                rotateTransform.Angle = Me.initialAngle + Math.Round(angle, 0)
                Me.designerItem.InvalidateMeasure()
            End If
        End Sub
    End Class

End Namespace
