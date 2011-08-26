Partial Public Class uCtrlImgMnu

    Dim vImage As String
    Dim vText As String
    Dim vTag As String
    Dim vDown As DateTime

    Public Property Icon() As String
        Get
            Return vImage
        End Get
        Set(ByVal value As String)
            vImage = value
            Dim bmpImage As New BitmapImage()
            bmpImage.BeginInit()
            bmpImage.UriSource = New Uri(vImage, UriKind.Absolute)
            bmpImage.EndInit()
            Image.Source = bmpImage
        End Set
    End Property

    Public Property Text() As String
        Get
            Return vText
        End Get
        Set(ByVal value As String)
            vText = value
            Lbl.Content = value
        End Set
    End Property

    Public Property Index() As String
        Get
            Return vTag
        End Get
        Set(ByVal value As String)
            vTag = value
        End Set
    End Property

    Public Sub New()

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
    End Sub

    Private Sub Image_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Image.MouseLeftButtonUp
        Dim vDiff As TimeSpan = Now - vDown
        If vDiff.Seconds < 1 Then RaiseEvent click(Me, e)
    End Sub

    Public Event click(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)

    Private Sub Image_PreviewMouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Image.PreviewMouseDown
        vDown = Now
    End Sub
End Class
