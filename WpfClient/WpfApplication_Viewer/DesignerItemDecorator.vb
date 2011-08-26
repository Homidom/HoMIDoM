Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows
Imports System.Windows.Media

Namespace Designer



    Public Class DesignerItemDecorator
        Inherits Control

        Private adorner As Adorner

        Public Property ShowDecorator() As Boolean
            Get
                Return CBool(GetValue(ShowDecoratorProperty))
            End Get
            Set(ByVal value As Boolean)
                SetValue(ShowDecoratorProperty, value)
            End Set
        End Property

        Public Shared ReadOnly ShowDecoratorProperty As DependencyProperty = DependencyProperty.Register("ShowDecorator", GetType(Boolean), GetType(DesignerItemDecorator), New FrameworkPropertyMetadata(False, New PropertyChangedCallback(AddressOf ShowDecoratorProperty_Changed)))

        Public Sub New()
            Dim Unloaded As RoutedEventHandler
            AddHandler Unloaded, (AddressOf Me.DesignerItemDecorator_Unloaded)
        End Sub

        Private Sub HideAdorner()
            If Me.adorner IsNot Nothing Then
                Me.adorner.Visibility = Visibility.Hidden
            End If
        End Sub

        Private Sub ShowAdorner()
            If Me.adorner Is Nothing Then
                Dim adornerLayer__1 As AdornerLayer = AdornerLayer.GetAdornerLayer(Me)

                If adornerLayer__1 IsNot Nothing Then
                    Dim designerItem As ContentControl = TryCast(Me.DataContext, ContentControl)
                    Dim canvas As Canvas = TryCast(VisualTreeHelper.GetParent(designerItem), Canvas)
                    Me.adorner = New ResizeRotateAdorner(designerItem)
                    adornerLayer__1.Add(Me.adorner)

                    If Me.ShowDecorator Then
                        Me.adorner.Visibility = Visibility.Visible
                    Else
                        Me.adorner.Visibility = Visibility.Hidden
                    End If
                End If
            Else
                Me.adorner.Visibility = Visibility.Visible
            End If
        End Sub

        Private Sub DesignerItemDecorator_Unloaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
            If Me.adorner IsNot Nothing Then
                Dim adornerLayer__1 As AdornerLayer = AdornerLayer.GetAdornerLayer(Me)
                If adornerLayer__1 IsNot Nothing Then
                    adornerLayer__1.Remove(Me.adorner)
                End If

                Me.adorner = Nothing
            End If
        End Sub

        Private Shared Sub ShowDecoratorProperty_Changed(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
            Dim decorator As DesignerItemDecorator = DirectCast(d, DesignerItemDecorator)
            Dim showDecorator As Boolean = CBool(e.NewValue)

            If showDecorator Then
                decorator.ShowAdorner()
            Else
                decorator.HideAdorner()
            End If
        End Sub
    End Class

End Namespace