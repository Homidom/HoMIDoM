Imports HoMIDom.HoMIDom

Public Class uScenario
    Dim Span As Integer 'Espacement correspondant à 1 seconde
    Dim _Duree As Integer = 15
    Dim _Zoom As Integer = 1
    Dim _ListAction As New ArrayList 'liste des actions

    'Duree max du timeline en minutes
    Public Property Duree As Integer
        Get
            Return _Duree
        End Get
        Set(ByVal value As Integer)
            _Duree = value
        End Set
    End Property

    'Zoom du timeline
    Public Property Zoom As Integer
        Get
            Return _Zoom
        End Get
        Set(ByVal value As Integer)
            _Zoom = value
        End Set
    End Property

    'Items (actions du timeline)
    Public Property Items As ArrayList
        Get
            Return _ListAction
        End Get
        Set(ByVal value As ArrayList)
            For i As Integer = 0 To value.Count - 1
                Dim x As New uAction
                x.ObjAction = value.Item(i)
                x.Uid = HoMIDom.HoMIDom.Api.GenerateGUID
                x.Span = Span
                x.Zoom = _Zoom
                AddHandler x.DeleteAction, AddressOf DeleteAction
                AddHandler x.ChangeAction, AddressOf ChangeAction
                x.Width = (_Duree * 3600) + 100
                _ListAction.Add(value.Item(i))
                StackPanel1.Children.Add(x)
            Next
        End Set
    End Property

    Public Sub New()

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        Mouse.OverrideCursor = Cursors.Wait
        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        Afficher()

        Mouse.OverrideCursor = Nothing
    End Sub

    'Ajouter action device
    Private Sub Image1_MouseLeftButtonDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ImgActDevice.MouseLeftButtonDown
        Dim effects As DragDropEffects
        Dim obj As New DataObject()
        obj.SetData(GetType(String), "ACTIONDEVICE")
        effects = DragDrop.DoDragDrop(Me.ImgActDevice, obj, DragDropEffects.Copy Or DragDropEffects.Move)
    End Sub

    Private Sub ScrollViewer1_DragOver(ByVal sender As System.Object, ByVal e As System.Windows.DragEventArgs) Handles ScrollViewer1.DragOver
        If e.Data.GetDataPresent(GetType(String)) Then
            e.Effects = DragDropEffects.Copy
        Else
            e.Effects = DragDropEffects.None
        End If
    End Sub

    Private Sub ScrollViewer1_Drop(ByVal sender As System.Object, ByVal e As System.Windows.DragEventArgs) Handles ScrollViewer1.Drop
        If e.Data.GetDataPresent(GetType(String)) Then
            e.Effects = DragDropEffects.Copy

            Dim uri As String = e.Data.GetData(GetType(String)).ToString

            Dim x As New uAction
            x.Uid = HoMIDom.HoMIDom.Api.GenerateGUID
            AddHandler x.DeleteAction, AddressOf DeleteAction
            AddHandler x.ChangeAction, AddressOf ChangeAction
            x.Width = (_Duree * 3600) + 100 'StckPnlLib.ActualWidth
            Select Case UCase(uri)
                Case "ACTIONDEVICE"
                    Dim y As New Action.ActionDevice
                    x.ObjAction = y
                Case "ACTIONMAIL"
                    Dim y As New Action.ActionMail
                    x.ObjAction = y
                Case "ACTIONIF"
                    Dim y As New Action.ActionIf
                    x.ObjAction = y
                Case "ACTIONMACRO"
                    Dim y As New Action.ActionMacro
                    x.ObjAction = y
            End Select
            x.Span = Span
            x.Zoom = _Zoom
            _ListAction.Add(x.ObjAction)
            StackPanel1.Children.Add(x)
        Else
            e.Effects = DragDropEffects.None
        End If
    End Sub

    'Supprimer une action
    Private Sub DeleteAction(ByVal Id As String)
        For i As Integer = 0 To StackPanel1.Children.Count - 1
            If StackPanel1.Children.Item(i).Uid = Id Then
                StackPanel1.Children.RemoveAt(i)
                _ListAction.RemoveAt(i)
                Exit For
            End If
        Next
    End Sub

    'Mise à jour d'une action
    Private Sub ChangeAction(ByVal Id As String)
        For i As Integer = 0 To StackPanel1.Children.Count - 1
            Dim x As uAction = StackPanel1.Children.Item(i)
            Dim j As Double = x.ObjAction.Timing.Minute + (x.ObjAction.Timing.Hour * 60)
            If j > Duree Then Duree = j + 5
        Next
        Afficher()
    End Sub

    Private Sub uScenario_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles uScenario.Loaded
        ScrollViewer1.MinHeight = uScenario.Height - 34
        ScrollViewer1.MaxHeight = uScenario.Height - 34
    End Sub

    Private Sub uScenario_SizeChanged(ByVal sender As Object, ByVal e As System.Windows.SizeChangedEventArgs) Handles uScenario.SizeChanged
        ScrollViewer1.MinHeight = uScenario.ActualHeight - 34
        ScrollViewer1.MaxHeight = uScenario.ActualHeight - 34
    End Sub

    'Afficher les éléments du timeline
    Private Sub Afficher()
        If StckPnlLib IsNot Nothing Then StckPnlLib.Children.Clear()
        If StckPnlLibTr IsNot Nothing Then StckPnlLibTr.Children.Clear()

        Dim _time As DateTime
        Dim t As Double = _Duree * 60 / _Zoom
        For j As Integer = 0 To t 'ajout des labels temps
            Dim x As New Label
            x.FontSize = 10
            x.HorizontalContentAlignment = HorizontalAlignment.Center
            x.Width = 60
            x.Foreground = New SolidColorBrush(Colors.White)
            x.Content = _time.ToLongTimeString
            StckPnlLib.Children.Add(x)
            _time = _time.AddSeconds(5 * _Zoom)
        Next
        Dim x2 As New Label 'ajout d'un label vide à la fin du timeline pour avoir un espace 
        x2.Width = 60
        StckPnlLib.Children.Add(x2)

        Span = 60 / 5

        Dim flag As Boolean
        For j As Integer = 0 To t
            Dim y1 As New Canvas
            y1.Width = 60
            If flag = False Then
                y1.Background = New SolidColorBrush(Colors.Transparent)
                flag = True
            Else
                y1.Background = New SolidColorBrush(Colors.Transparent)
                flag = False
            End If
            Dim R1 As New Rectangle
            R1.Fill = New SolidColorBrush(Colors.White)
            R1.Width = 3
            R1.Height = 16
            y1.Children.Add(R1)
            For k = 1 To 4
                Dim R2 As New Rectangle
                R2.Fill = New SolidColorBrush(Colors.White)
                R2.Width = 1
                R2.Height = 8
                y1.Children.Add(R2)
                y1.SetLeft(R2, k * Span)
            Next
            StckPnlLibTr.Children.Add(y1)
        Next

        'On affecte la valeur du Zoom à chaque action
        If StackPanel1 IsNot Nothing Then
            For i As Integer = 0 To StackPanel1.Children.Count - 1
                Dim x As uAction = StackPanel1.Children.Item(i)
                x.Width = (_Duree * 3600) + 100
                x.Zoom = _Zoom
            Next
        End If
    End Sub

    'Zoom avant
    Private Sub ZoomPlus_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ZoomPlus.MouseDown
        _Zoom -= 5
        If _Zoom <= 0 Then _Zoom = 1
        Afficher()
    End Sub

    'Zoom arrière
    Private Sub ZoomMoins_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ZoomMoins.MouseDown
        _Zoom += 5
        If _Zoom > 100 Then _Zoom = 100
        Afficher()
    End Sub

    'Ajouter action mail
    Private Sub ImgActMail_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ImgActMail.MouseLeftButtonDown
        Dim effects As DragDropEffects
        Dim obj As New DataObject()
        obj.SetData(GetType(String), "ACTIONMAIL")
        effects = DragDrop.DoDragDrop(Me.ImgActMail, obj, DragDropEffects.Copy Or DragDropEffects.Move)
    End Sub

    'Ajouter action if
    Private Sub ImgActIf_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ImgActIf.MouseLeftButtonDown
        Dim effects As DragDropEffects
        Dim obj As New DataObject()
        obj.SetData(GetType(String), "ACTIONIF")
        effects = DragDrop.DoDragDrop(Me.ImgActIf, obj, DragDropEffects.Copy Or DragDropEffects.Move)
    End Sub

    'Ajouter action macro
    Private Sub ImgActMacro_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ImgActMacro.MouseLeftButtonDown
        Dim effects As DragDropEffects
        Dim obj As New DataObject()
        obj.SetData(GetType(String), "ACTIONMACRO")
        effects = DragDrop.DoDragDrop(Me.ImgActMacro, obj, DragDropEffects.Copy Or DragDropEffects.Move)
    End Sub
End Class
