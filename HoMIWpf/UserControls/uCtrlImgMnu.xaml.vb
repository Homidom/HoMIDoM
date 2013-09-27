Imports System.IO

Partial Public Class uCtrlImgMnu
    'Type de Menu
    Public Enum TypeOfMnu
        None = 0
        Internet = 1
        Meteo = 2
        Zone = 3
        LecteurMedia = 4
        Config = 99
    End Enum

    'Declaration des variables
    Dim vImage As String
    Dim vText As String
    Dim vTag As String
    Dim vDown As DateTime
    Dim vId As String
    Dim _submnu As New List(Of String)
    Dim _type As TypeOfMnu = 0
    Dim _Parametres As New List(Of String)
    Dim _Defaut As Boolean
    Dim _Idelement As String
    Dim _Visible As Boolean 'en désactivant cette propriété la page n’est plus accessible depuis le bandeau de navigation ou par navigation avec le doigt (glissement vers la gauche ou la droite). La page n’est alors accessible que par "jump" depuis le "What I Do" des widgets. Cela permet de créer des pages cachées ou annexes.
    Dim _IsSelect As Boolean
    Dim _IsDefaut As Boolean 'page par defaut
    Dim _Statique 'une page statique est visible en permanence à l’écran. Elle est donc retirée automatiquement de la navigation. Cela permet de placer widgets communs ou un fond commun à toutes les pages.
    Dim _ShowBackground As Boolean

    'cette propriété permet d’afficher ou non l’image de fond de la page. Sans image de fond vous verrez apparaitre le fond noir de l’application.
    Public Property ShowBackground As Boolean
        Get
            Return _ShowBackground
        End Get
        Set(value As Boolean)
            _ShowBackground = value
        End Set
    End Property

    'une page statique est visible en permanence à l’écran. Elle est donc retirée automatiquement de la navigation. Cela permet de placer widgets communs ou un fond commun à toutes les pages.
    Public Property Statique As Boolean
        Get
            Return _Statique
        End Get
        Set(value As Boolean)
            _Statique = value
        End Set
    End Property

    'Page par défaut
    Public Property IsDefaut As Boolean
        Get
            Return _IsDefaut
        End Get
        Set(value As Boolean)
            _IsDefaut = value
        End Set
    End Property

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
            If String.IsNullOrEmpty(value) = False Then
                vImage = value
                If File.Exists(value) And _type <> TypeOfMnu.Zone Then
                    Image.Source = LoadBitmapImage(vImage)
                ElseIf File.Exists(value) = False And _type <> TypeOfMnu.Zone Then
                    vImage = ""
                End If
            End If
        End Set
    End Property

    Public Property Visible As Boolean
        Get
            Return _Visible
        End Get
        Set(ByVal value As Boolean)
            _Visible = value
        End Set
    End Property

    Public Property Label As String
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


    Private Sub Image_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Border.MouseLeftButtonUp
        Dim vDiff As TimeSpan = Now - vDown
        If vDiff.Seconds < 1 Then
            RaiseEvent click(Me, e)
        End If
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

    Public Property IDElement As String
        Get
            Return _Idelement
        End Get
        Set(ByVal value As String)
            _Idelement = value
            If _type = TypeOfMnu.Zone And IsConnect = True And String.IsNullOrEmpty(_Idelement) = False Then
                Dim file As String = myService.ReturnZoneByID(IdSrv, _Idelement).Icon
                Dim tab As Byte()
                If String.IsNullOrEmpty(file) = False Then
                    tab = myService.GetByteFromImage(file)
                    If tab IsNot Nothing Then Image.Source = ConvertArrayToImage(tab)
                End If

            End If
        End Set
    End Property

    Public Property Parametres As List(Of String)
        Get
            Return _Parametres
        End Get
        Set(ByVal value As List(Of String))
            If value IsNot Nothing Then _Parametres = value
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

    Public Property IsSelect As Boolean
        Get
            Return _IsSelect
        End Get
        Set(value As Boolean)
            _IsSelect = value
            If value Then
                Border.BorderBrush = Brushes.Red
            Else
                Border.BorderBrush = Brushes.DarkGray
            End If
        End Set
    End Property

    Public Event click(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)

    Private Sub Image_PreviewMouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Border.PreviewMouseDown
        vDown = Now
    End Sub
End Class
