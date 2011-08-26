Imports System.Windows
Imports System.Windows.Controls

Namespace Designer



    Public Class ResizeRotateChrome
        Inherits Control

        Shared Sub New()
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(GetType(ResizeRotateChrome), New FrameworkPropertyMetadata(GetType(ResizeRotateChrome)))
        End Sub
    End Class
End Namespace