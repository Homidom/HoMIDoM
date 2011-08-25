Namespace UAniScrollViewer



    Public Class AniScrollViewer
        Inherits ScrollViewer

        Public Shared CurrentVerticalOffsetProperty As DependencyProperty = DependencyProperty.Register("CurrentVerticalOffset", GetType(Double), GetType(AniScrollViewer), New PropertyMetadata(New PropertyChangedCallback(AddressOf OnVerticalChanged)))
        Public Shared CurrentHorizontalOffsetProperty As DependencyProperty = DependencyProperty.Register("CurrentHorizontalOffsetOffset", GetType(Double), GetType(AniScrollViewer), New PropertyMetadata(New PropertyChangedCallback(AddressOf OnHorizontalChanged)))

        Private Shared Sub OnVerticalChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
            Dim viewer As AniScrollViewer = TryCast(d, AniScrollViewer)
            viewer.ScrollToVerticalOffset(CDbl(e.NewValue))
        End Sub

        Private Shared Sub OnHorizontalChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
            Dim viewer As AniScrollViewer = TryCast(d, AniScrollViewer)
            viewer.ScrollToHorizontalOffset(CDbl(e.NewValue))
        End Sub

        Public Property CurrentHorizontalOffset() As Double
            Get
                Return CDbl(Me.GetValue(CurrentHorizontalOffsetProperty))
            End Get
            Set(ByVal value As Double)
                Me.SetValue(CurrentHorizontalOffsetProperty, value)
            End Set
        End Property

        Public Property CurrentVerticalOffset() As Double
            Get
                Return CDbl(Me.GetValue(CurrentVerticalOffsetProperty))
            End Get
            Set(ByVal value As Double)
                Me.SetValue(CurrentVerticalOffsetProperty, value)
            End Set
        End Property
    End Class
End Namespace