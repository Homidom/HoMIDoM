Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Media

Namespace Designer

    Public Class SizeAdorner
        Inherits Adorner

        Private chrome As SizeChrome
        Private visuals As VisualCollection
        Private designerItem As ContentControl

        Protected Overrides ReadOnly Property VisualChildrenCount() As Integer
            Get
                Return Me.visuals.Count
            End Get
        End Property

        Public Sub New(ByVal designerItem As ContentControl)
            MyBase.New(designerItem)
            Me.SnapsToDevicePixels = True
            Me.designerItem = designerItem
            Me.chrome = New SizeChrome()
            Me.chrome.DataContext = designerItem
            Me.visuals = New VisualCollection(Me)
            Me.visuals.Add(Me.chrome)
        End Sub

        Protected Overrides Function GetVisualChild(ByVal index As Integer) As Visual
            Return Me.visuals(index)
        End Function

        Protected Overrides Function ArrangeOverride(ByVal arrangeBounds As Size) As Size
            Me.chrome.Arrange(New Rect(New Point(0.0, 0.0), arrangeBounds))
            Return arrangeBounds
        End Function
    End Class
End Namespace