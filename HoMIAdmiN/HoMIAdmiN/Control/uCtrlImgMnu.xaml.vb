Imports System.IO

Partial Public Class uCtrlImgMnu

    Dim vText As String
    Dim vimage As String
    Dim _Defaut As Boolean
    Dim _Adresse As String
    Dim _Port As String
    Dim _IsSelect As Boolean = False
    Dim _Id As String

    Public Property ID As String
        Get
            Return _Id
        End Get
        Set(ByVal value As String)
            _Id = value
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

    Public Property Adresse As String
        Get
            Return _Adresse
        End Get
        Set(ByVal value As String)
            _Adresse = value
        End Set
    End Property

    Public Property Port As String
        Get
            Return _Port
        End Get
        Set(ByVal value As String)
            _Port = value
        End Set
    End Property

    Public Property Icon() As String
        Get
            Return vimage
        End Get
        Set(ByVal value As String)
            Try
                vimage = value
                If File.Exists(value) Then
                    Dim bmpImage As New BitmapImage()
                    bmpImage.BeginInit()
                    bmpImage.UriSource = New Uri(vimage, UriKind.Absolute)
                    bmpImage.EndInit()
                    Image.Source = bmpImage
                End If
            Catch ex As Exception
                MessageBox.Show("Erreur uCtrlImgMnu SetIcon: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
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

    Public Property IsSelect As Boolean
        Get
            Return _IsSelect
        End Get
        Set(ByVal value As Boolean)
            _IsSelect = value
            If value = True Then
                Border1.BorderBrush = New SolidColorBrush(Colors.Blue)
            Else
                Border1.BorderBrush = New SolidColorBrush(Colors.DarkGray)
            End If
        End Set
    End Property

    Public Sub New()

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
    End Sub

    Private Sub Image_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Image.MouseLeftButtonUp
        RaiseEvent click(Me, e)
        IsSelect = True
    End Sub

    Public Function ConvertArrayToImage(ByVal value As Object) As Object
        Try
            Dim ImgSource As BitmapImage = Nothing
            Dim array As Byte() = TryCast(value, Byte())

            If array IsNot Nothing Then
                ImgSource = New BitmapImage()
                ImgSource.BeginInit()
                ImgSource.StreamSource = New MemoryStream(array)
                ImgSource.EndInit()
            End If
            Return ImgSource
        Catch ex As Exception
            MessageBox.Show("Erreur uCtrlImgMnu ConvertArrayToImage: " & ex.ToString, "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error)
            Return Nothing
        End Try
    End Function

    Public Event click(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
    Public Event Rightclick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)

    Private Sub Image_MouseRightButtonDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Image.MouseRightButtonDown
        RaiseEvent Rightclick(Me, e)
    End Sub
End Class
