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
Imports System.ComponentModel

Namespace Customcontrols
    ''' <summary>
    ''' Interaction logic for Colorpicker.xaml
    ''' </summary>
    Partial Public Class Colorpicker
        Inherits UserControl

        Public Event SelectColorChange(ByVal sender As Brush)

        Public Sub New()
            InitializeComponent()
        End Sub

        Public Property SelectedColor() As Brush
            Get
                Return DirectCast(GetValue(SelectedColorProperty), Brush)
            End Get
            Set(ByVal value As Brush)
                SetValue(SelectedColorProperty, value)
            End Set
        End Property

        ' Using a DependencyProperty as the backing store for SelectedColor.  
        ' This enables animation, styling, binding, etc...
        Public Shared ReadOnly SelectedColorProperty As DependencyProperty = DependencyProperty.Register("SelectedColor", GetType(Brush), GetType(Colorpicker), New UIPropertyMetadata(Nothing))

        Private Sub superCombo_SelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles superCombo.SelectionChanged
            'RaiseEvent SelectColorChange(superCombo.SelectedItem)
            Try
                Dim a As String = superCombo.SelectedItem.ToString
                a = a.Replace("System.Windows.Media.Color", "")
                Dim newColor As Color = DirectCast(ColorConverter.ConvertFromString(a), Color)
                RaiseEvent SelectColorChange(New SolidColorBrush(newColor))
            Catch ex As Exception
                MessageBox.Show("Erreur superCombo_SelectionChanged: " & ex.ToString, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End Sub
    End Class
End Namespace
