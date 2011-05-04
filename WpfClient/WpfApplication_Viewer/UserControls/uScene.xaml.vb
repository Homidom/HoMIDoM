Partial Public Class uScene
    Dim _Label As String
    Dim _Script As String
    Dim _Fonction As String
    Dim _Command As String
    Dim _Icon As String

    Public Enum eTypeDevice
        OnOff = 0
        OnOffVariation = 1
        Capteur = 2
        Audio = 3
    End Enum

    Public Property Label() As String
        Get
            Return _Label
        End Get
        Set(ByVal value As String)
            _Label = value
            Lbl.Content = value
        End Set
    End Property
    Public Property Script() As String
        Get
            Return _Script
        End Get
        Set(ByVal value As String)
            _Script = value
        End Set
    End Property
    Public Property Fonction() As String
        Get
            Return _Fonction
        End Get
        Set(ByVal value As String)
            _Fonction = value
        End Set
    End Property
    Public Property Command() As String
        Get
            Return _Command
        End Get
        Set(ByVal value As String)
            _Command = value
        End Set
    End Property


    Public Property Icon() As String
        Get
            Return _Icon
        End Get
        Set(ByVal value As String)
            _Icon = value
            If _Icon <> "" Then
                Dim bmpImage As New BitmapImage()
                bmpImage.BeginInit()
                bmpImage.UriSource = New Uri(_Icon, UriKind.Absolute)
                bmpImage.EndInit()
                Ico.Source = bmpImage
            End If
        End Set
    End Property

    Public Sub New()

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()
        Dim ImgSrc As ImageSource = New BitmapImage(New Uri("C:\ehome\Images\11.png"))
        Dim imgBrush As ImageBrush = New ImageBrush(ImgSrc)
        BtnRun.Background = imgBrush

    End Sub


    Private Sub BtnRun_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnRun.Click
        If IsHSConnect = True Then
            Dim s
            's = hs.RunEx(_Script, _Fonction, _Command)
        Else
            MessageBox.Show("Impossible d'exécuter le script car vous n'êtes pas connecté à HomeSeer")
        End If
    End Sub
End Class
