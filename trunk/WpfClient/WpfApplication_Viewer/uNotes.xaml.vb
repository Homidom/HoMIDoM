Imports System.Windows.Shapes
Imports System.IO
Imports System.Windows.Media
Imports System.Windows.Threading

Partial Public Class uNotes
    Private _PreviousPoint As Point ' Dernier point de la souris
    Private Flag As Boolean
    Private MyBrush As System.Windows.Media.Brush
    Private MyStroke As Integer
    Private _imageList As New List(Of Image)()
    Private imgStackPnl As New StackPanel()
    Private CurrentImg As String

    Private scrollTarget As Point
    Private scrollStartPoint As Point
    Private scrollStartOffset As Point
    Private previousPoint As Point

#Region "Dessiner"

    Private Sub canvasImg_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles CanvasImg.MouseDown
        Dim NewPoint As Point = e.GetPosition(CanvasImg)
        _PreviousPoint = NewPoint
        Flag = True
        NewPoint = Nothing
    End Sub

    Private Sub CanvasImg_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles CanvasImg.MouseLeave
        Flag = False
    End Sub

    Private Sub canvasImg_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles CanvasImg.MouseMove
        If Flag = True Then
            Dim NewPoint As Point = e.GetPosition(CanvasImg)
            Dim x As New Line
            x.X1 = _PreviousPoint.X
            x.Y1 = _PreviousPoint.Y
            x.X2 = NewPoint.X
            x.Y2 = NewPoint.Y
            x.Stroke = MyBrush
            x.StrokeThickness = MyStroke
            CanvasImg.Children.Add(x)
            _PreviousPoint = NewPoint
            NewPoint = Nothing
        End If
    End Sub

    Private Sub canvasImg_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles CanvasImg.MouseUp
        Flag = False
    End Sub
    'Choix d'une couleur
    Private Sub Color_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ColorBlack.MouseDown, ColorBlue.MouseDown, ColorGreen.MouseDown, ColorOrange.MouseDown, ColorRed.MouseDown, ColorYellow.MouseDown
        ColorBlack.StrokeThickness = 2
        ColorBlue.StrokeThickness = 2
        ColorRed.StrokeThickness = 2
        ColorGreen.StrokeThickness = 2
        ColorOrange.StrokeThickness = 2
        ColorYellow.StrokeThickness = 2

        Select Case sender.name
            Case "ColorBlue"
                MyBrush = Brushes.Blue
                ColorBlue.StrokeThickness = 5
            Case "ColorBlack"
                MyBrush = Brushes.Black
                ColorBlack.StrokeThickness = 5
            Case "ColorGreen"
                MyBrush = Brushes.Green
                ColorGreen.StrokeThickness = 5
            Case "ColorOrange"
                MyBrush = Brushes.Orange
                ColorOrange.StrokeThickness = 5
            Case "ColorRed"
                MyBrush = Brushes.Red
                ColorRed.StrokeThickness = 5
            Case "ColorYellow"
                MyBrush = Brushes.Yellow
                ColorYellow.StrokeThickness = 5
        End Select
    End Sub
    'choix de l'epaisseur du trait
    Private Sub Stroke_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Stroke10.MouseDown, Stroke2.MouseDown, Stroke4.MouseDown, Stroke6.MouseDown, Stroke8.MouseDown
        Stroke2.StrokeThickness = 2
        Stroke4.StrokeThickness = 2
        Stroke6.StrokeThickness = 2
        Stroke8.StrokeThickness = 2
        Stroke10.StrokeThickness = 2

        Select Case sender.name
            Case "Stroke2"
                MyStroke = 2
                Stroke2.StrokeThickness = 5
            Case "Stroke4"
                MyStroke = 4
                Stroke4.StrokeThickness = 5
            Case "Stroke6"
                MyStroke = 6
                Stroke6.StrokeThickness = 5
            Case "Stroke8"
                MyStroke = 8
                Stroke8.StrokeThickness = 5
            Case "Stroke10"
                MyStroke = 10
                Stroke10.StrokeThickness = 5
        End Select
    End Sub
#End Region

    Private Sub UDessiner_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        MyBrush = Brushes.Black
        MyStroke = 2
        ColorBlack.StrokeThickness = 5
        Stroke2.StrokeThickness = 5

        ColorYellow.Stroke = Brushes.DarkGray
        ColorRed.Stroke = Brushes.DarkGray
        ColorBlack.Stroke = Brushes.DarkGray
        ColorBlue.Stroke = Brushes.DarkGray
        ColorGreen.Stroke = Brushes.DarkGray
        ColorOrange.Stroke = Brushes.DarkGray

        Stroke2.Stroke = Brushes.DarkGray
        Stroke4.Stroke = Brushes.DarkGray
        Stroke6.Stroke = Brushes.DarkGray
        Stroke8.Stroke = Brushes.DarkGray
        Stroke10.Stroke = Brushes.DarkGray

        Canvas1.Width = Me.ActualWidth
        Canvas4.Width = Me.ActualWidth
        ScrollViewer1.Width = Canvas4.ActualWidth
        CanvasImg.Width = Me.ActualWidth - 150
        Canvas1.SetLeft(Canvas3, CanvasImg.Width + 35)
        Canvas1.SetLeft(Canvas5, CanvasImg.Width + 35)
        Canvas1.SetLeft(BtnClear, CanvasImg.Width + 35)
        Canvas1.SetLeft(BtnSave, CanvasImg.Width + 35)
        Canvas1.SetLeft(BtnDelete, CanvasImg.Width + 35)
        CanvasImg.Height = Me.ActualHeight - Canvas4.ActualHeight - 15
    End Sub

    Public Sub New()
        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        GetNotes()

        Dim myBrush As New RadialGradientBrush
        myBrush.GradientOrigin = New Point(0.5, 0.5)
        myBrush.GradientStops.Add(New GradientStop(Colors.Black, 0.0))
        myBrush.GradientStops.Add(New GradientStop(Colors.DarkGray, 0.5))
        myBrush.GradientStops.Add(New GradientStop(Colors.Black, 1.0))
        ScrollViewer1.Background = myBrush

    End Sub

#Region "Button"
    'Bouton effacer le dessin en cours
    Private Sub BtnClear_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnClear.Click
        CanvasImg.Children.Clear()
    End Sub
    'Enregistrer la note
    Private Sub BtnSave_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnSave.Click
        If CurrentImg <> "" Then
            ExportToPng(New Uri(CurrentImg), CanvasImg)
            CurrentImg = ""
        Else
            ExportToPng(New Uri("C:\ehome\data\notes\" & Now.Year & Now.Month & Now.Day & Now.Hour & Now.Minute & Now.Second & ".png"), CanvasImg)
        End If
        GetNotes()
    End Sub
    'Supprimer la note
    Private Sub BtnDelete_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDelete.Click
        Try
            If CurrentImg <> "" Then
                File.Delete(CurrentImg)
                CurrentImg = ""
                Canvas1.Children.Clear()
                GetNotes()
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur lors de la suppression du fichier: " & ex.Message)
        End Try
    End Sub

#End Region

#Region "Gestion images"
    Private Sub ExportToPng(ByVal Path As Uri, ByVal Surface As Canvas)
        Try
            Dim Y1 As Long

            Y1 = Surface.ActualHeight
            Dim _Size As Size = New Size(Surface.ActualWidth, Surface.Height)

            Surface.Measure(_Size)
            Surface.Arrange(New Rect(_Size))

            Dim renderbitamp As RenderTargetBitmap = New RenderTargetBitmap(_Size.Width, _Size.Height, 100D, 100D, PixelFormats.Pbgra32)
            renderbitamp.Render(Surface)

            Dim outstream As FileStream
            outstream = New FileStream(Path.LocalPath, FileMode.Create)


            Dim encoder As PngBitmapEncoder = New PngBitmapEncoder
            encoder.Frames.Add(BitmapFrame.Create(renderbitamp))
            encoder.Save(outstream)
            outstream.Close()
            outstream = Nothing
            Surface.Height = Y1
        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'enregistrement du fichier: " & ex.Message)
        End Try
    End Sub

    Private Function GetImageList() As List(Of System.Windows.Controls.Image)
        Dim strpath As String = "C:\ehome\data\notes"
        Dim imageList As New List(Of System.Windows.Controls.Image)()
        Dim strFilePath As String = ""

        If Directory.Exists(strpath) = False Then
            MessageBox.Show(String.Format("{0} path could not be found.", strpath))
            Return imageList
        End If

        Try
            ' supported image files
            Dim formatList As New List(Of String)()
            formatList.Add(".png")

            Dim dirInfo As New DirectoryInfo(strpath)
            Dim files As FileInfo() = dirInfo.GetFiles()

            For Each curFile As FileInfo In files
                ' only look for image files
                If formatList.Contains(curFile.Extension.ToLower()) = False Then
                    Continue For
                End If

                strFilePath = curFile.FullName

                Dim curImage As New System.Windows.Controls.Image()

                curImage.Height = 65
                curImage.Width = 65
                curImage.Stretch = Stretch.Fill

                curImage.Source = GetImageSource(curFile.FullName) 'bmpImage
                curImage.Margin = New Thickness(10)
                curImage.Tag = curFile.FullName
                AddHandler curImage.MouseDown, AddressOf Click
                imageList.Add(curImage)
            Next

            If imageList.Count = 0 Then
                MessageBox.Show(String.Format("No image files could be found in {0}", strpath))
            End If
        Catch ex As Exception
            MessageBox.Show(String.Format("{0}-{1}", ex.Message, strFilePath))
        End Try

        Return imageList
    End Function

    Private Sub Click(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
        Dim URL As String = sender.tag
        CurrentImg = URL
        Dim curimg As New Image
        curimg.Height = CanvasImg.ActualHeight
        curimg.Width = CanvasImg.ActualWidth
        curimg.Source = GetImageSource(URL) 'bmpImage
        CanvasImg.Children.Add(curimg)
    End Sub

    Private Sub GetNotes()
        If imgStackPnl.Children.Count > 0 Then imgStackPnl.Children.Clear()
        ScrollViewer1.Content = Nothing

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        imgStackPnl.HorizontalAlignment = HorizontalAlignment.Left
        imgStackPnl.VerticalAlignment = VerticalAlignment.Top
        imgStackPnl.Orientation = Orientation.Horizontal

        _imageList = GetImageList()
        For i As Integer = 0 To _imageList.Count - 1
            imgStackPnl.Children.Add(_imageList.Item(i))
        Next

        ScrollViewer1.Content = imgStackPnl
    End Sub

    Private Function GetImageSource(ByVal path As String) As ImageSource

        ' Ouverture d'une stream vers le fichier original
        Dim reader As New StreamReader(path)

        ' Préparation d'un tableau de Byte pour lire la stream
        Dim length As Int32 = Convert.ToInt32(reader.BaseStream.Length)
        Dim data(length) As Byte

        ' Lecture de la stream
        reader.BaseStream.Read(data, 0, length)

        ' Création d'une nouvelle stream mémoire 
        ' afin de copier le contenu de la stream originale
        Dim stream As New MemoryStream(data)

        ' Création de l'image à parir de la stream en mémoire
        Dim image As New BitmapImage()
        image.BeginInit()
        image.StreamSource = stream
        image.EndInit()
        reader.Close()

        ' Libération des ressources
        reader.Dispose()
        reader = Nothing

        data = Nothing
        stream = Nothing

        Return image
    End Function
#End Region

    Private Sub ScrollViewer1_PreviewMouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ScrollViewer1.PreviewMouseDown
        scrollStartPoint = e.GetPosition(Me)
        scrollStartOffset.X = ScrollViewer1.HorizontalOffset
    End Sub

    Private Sub ScrollViewer1_PreviewMouseMove(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles ScrollViewer1.PreviewMouseMove
        If e.LeftButton = MouseButtonState.Pressed Then
            Dim currentPoint As Point = e.GetPosition(Me)
            Dim delta As New Point(scrollStartPoint.X - currentPoint.X, scrollStartPoint.Y - currentPoint.Y)
            scrollTarget.X = scrollStartOffset.X + delta.X
            ScrollToPosition(ScrollViewer1, scrollTarget.X, currentPoint.Y, 600)
        End If
    End Sub

    Private Sub Canvas1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles Canvas1.MouseMove
        'Flag = False
    End Sub
End Class
