Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Media

Namespace Designer

    Public Class ResizeRotateAdorner
        Inherits Adorner

        Private visuals As VisualCollection
        Private chrome As ResizeRotateChrome

        Protected Overrides ReadOnly Property VisualChildrenCount() As Integer
            Get
                Return visuals.Count
            End Get
        End Property

        Public Sub New(ByVal designerItem As ContentControl)
            MyBase.New(designerItem)
            SnapsToDevicePixels = True
            chrome = New ResizeRotateChrome()
            chrome.DataContext = designerItem
            visuals = New VisualCollection(Me)
            visuals.Add(chrome)
        End Sub

        Protected Overrides Function ArrangeOverride(ByVal arrangeBounds As Size) As Size
            Me.chrome.Arrange(New Rect(arrangeBounds))
            Return arrangeBounds
        End Function

        Protected Overrides Function GetVisualChild(ByVal index As Integer) As Visual
            Return visuals(index)
        End Function
    End Class
End Namespace