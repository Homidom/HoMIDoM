Imports System.IO

Partial Public Class uCtrlImgMnu
    Public Enum TypeOfMnu
        None = 0
        Internet = 1
        Meteo = 2
        Zone = 3
        Config = 99
    End Enum

    Dim vImage As String
    Dim vText As String
    Dim vTag As String
    Dim vDown As DateTime
    Dim vId As String
    Dim _submnu As New List(Of String)
    Dim _type As TypeOfMnu = 0
    Dim _Parametres As New List(Of String)
    Dim _Defaut As Boolean

    Public Property Id As String
        Get
            Return vId
        End Get
        Set(ByVal value As String)
            vId = value
        End Set
    End Property

    Public Property Icon() As String
        Get
            Return vImage
        End Get
        Set(ByVal value As String)
            vImage = value
            If File.Exists(value) Then
                Dim bmpImage As New BitmapImage()
                bmpImage.BeginInit()
                bmpImage.UriSource = New Uri(vImage, UriKind.Absolute)
                bmpImage.EndInit()
                Image.Source = bmpImage
            End If
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

    Public Property Type As TypeOfMnu
        Get
            Return _type
        End Get
        Set(ByVal value As TypeOfMnu)
            _type = value
        End Set
    End Property

    Public Property SubMenu As List(Of String)
        Get
            Return _submnu
        End Get
        Set(ByVal value As List(Of String))
            _submnu = value
        End Set
    End Property

    Public Function ConvertArrayToImage(ByVal value As Object) As Object
        Dim ImgSource As BitmapImage = Nothing
        Dim array As Byte() = TryCast(value, Byte())

        If array IsNot Nothing Then
            ImgSource = New BitmapImage()
            ImgSource.BeginInit()
            ImgSource.StreamSource = New MemoryStream(array)
            ImgSource.EndInit()
        End If
        Return ImgSource
    End Function

    Public Property Parametres As List(Of String)
        Get
            Return _Parametres
        End Get
        Set(ByVal value As List(Of String))
            _Parametres = value
            If _type = TypeOfMnu.Zone Then
                If _Parametres(0) <> "" Then
                    Image.Source = ConvertArrayToImage(myService.GetByteFromImage(myService.ReturnZoneByID(IdSrv, Parametres(0)).Icon))
                End If
            End If
        End Set
    End Property

    Public Property Defaut As Boolean
        Get
            Return _Defaut
        End Get
        Set(ByVal value As Boolean)
            _Defaut = value
        End Set
    End Property

    Public Event click(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)

    Private Sub Image_PreviewMouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Image.PreviewMouseDown
        vDown = Now
    End Sub
End Class
