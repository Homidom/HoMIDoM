Imports System.Globalization
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data

Namespace Designer

    Public Class SizeChrome
        Inherits Control

        Shared Sub New()
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(GetType(SizeChrome), New FrameworkPropertyMetadata(GetType(SizeChrome)))
        End Sub
    End Class

    Public Class DoubleFormatConverter
        Implements IValueConverter


        Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
            Dim d As Double = CDbl(value)
            Return Math.Round(d)
        End Function

        Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
            Return Nothing
        End Function

    End Class

End Namespace