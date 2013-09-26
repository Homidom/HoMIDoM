Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Navigation
Imports System.Windows.Shapes
Imports System.Windows.Controls.Primitives

Public Class DynamicImageButton
    Inherits ButtonBase

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(DynamicImageButton), New FrameworkPropertyMetadata(GetType(DynamicImageButton)))
    End Sub

    Public Property IconImageUri() As String
        Get
            Return DirectCast(GetValue(IconImageUriProperty), String)
        End Get
        Set(ByVal value As String)
            SetValue(IconImageUriProperty, value)
        End Set
    End Property


    Public Shared ReadOnly IconImageUriProperty As DependencyProperty = DependencyProperty.Register("IconImageUri", GetType(String), GetType(DynamicImageButton), New UIPropertyMetadata(String.Empty, Function(o, e)
                                                                                                                                                                                                           Try
                                                                                                                                                                                                               Dim uriSource As New Uri(DirectCast(e.NewValue, String), UriKind.RelativeOrAbsolute)
                                                                                                                                                                                                               If uriSource IsNot Nothing Then
                                                                                                                                                                                                                   Dim button As DynamicImageButton = TryCast(o, DynamicImageButton)
                                                                                                                                                                                                                   Dim img As New BitmapImage(uriSource)
                                                                                                                                                                                                                   button.SetValue(IconImageProperty, img)
                                                                                                                                                                                                               End If
                                                                                                                                                                                                           Catch ex As Exception
                                                                                                                                                                                                               Throw ex
                                                                                                                                                                                                           End Try
                                                                                                                                                                                                           Return ""
                                                                                                                                                                                                       End Function))


    Public Property IconImage() As BitmapImage
        Get
            Return DirectCast(GetValue(IconImageProperty), BitmapImage)
        End Get
        Set(ByVal value As BitmapImage)
            SetValue(IconImageProperty, value)
        End Set
    End Property
    Public Shared ReadOnly IconImageProperty As DependencyProperty = DependencyProperty.Register("IconImage", GetType(BitmapImage), GetType(DynamicImageButton), New UIPropertyMetadata(Nothing))

End Class

