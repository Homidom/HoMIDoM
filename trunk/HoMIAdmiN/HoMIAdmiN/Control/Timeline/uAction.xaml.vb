Public Class uAction
    Dim _TypeAction As Integer
    Dim _Libelle As String
    Dim _Action As String
    Dim _IDdevice As String
    Dim _Timing As DateTime
    Dim _Parametres As New ArrayList
    Public Span As Integer
    Public _Zoom As Integer

    Public Event DeleteAction(ByVal ID As String)
    Public Event ChangeAction(ByVal ID As String)

    Public Property Zoom As Integer
        Get
            Return _Zoom
        End Get
        Set(ByVal value As Integer)
            _Zoom = value
            Dim j As Double = (_Timing.Minute * 60) + _Timing.Second
            j = (Span * j) / Zoom
            Fond.SetLeft(Rectangle1, j)
            Fond.SetLeft(ImgDelete, j + 160)
        End Set
    End Property

    Private Sub Rectangle1_MouseLeftButtonDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Rectangle1.MouseLeftButtonDown
        Select Case _TypeAction
            Case 0 'Action Device
                Dim frm As New WActionDevice
                If IDDevice <> "" Then
                    frm.ID = IDDevice
                    frm.Action = Action
                    frm.Delai = Timing
                    frm.Parametres = Parametres
                End If
                frm.ShowDialog()
                If frm.DialogResult.HasValue And frm.DialogResult.Value Then
                    IDDevice = frm.ID
                    Action = frm.Action
                    Timing = frm.Delai
                    Parametres = frm.Parametres
                    frm.Close()
                    RaiseEvent ChangeAction(Me.Uid)
                End If
        End Select
    End Sub

    Public Property Timing As DateTime
        Get
            Return _Timing
        End Get
        Set(ByVal value As DateTime)
            _Timing = value
            Dim j As Double = (_Timing.Hour * 60 * 60) + (_Timing.Minute * 60) + _Timing.Second
            j = (Span * j) / zoom
            Fond.SetLeft(Rectangle1, j)
            Fond.SetLeft(ImgDelete, j + 160)
        End Set
    End Property

    Public Property IDDevice As String
        Get
            Return _IDdevice
        End Get
        Set(ByVal value As String)
            _IDdevice = value
            Libelle = Window1.myService.ReturnDeviceByID(_IDdevice).Name
        End Set
    End Property

    Public Property TypeAction As Integer
        Get
            Return _TypeAction
        End Get
        Set(ByVal value As Integer)
            _TypeAction = value
        End Set
    End Property

    Public Property Libelle As String
        Get
            Return _Libelle
        End Get
        Set(ByVal value As String)
            _Libelle = value
            Label1.Content = value
        End Set
    End Property

    Public Property Action As String
        Get
            Return _Action
        End Get
        Set(ByVal value As String)
            _Action = value
            Label2.Content = value
        End Set
    End Property

    Public Property Parametres As ArrayList
        Get
            Return _Parametres
        End Get
        Set(ByVal value As ArrayList)
            _Parametres = value
        End Set
    End Property

    Private Sub ImgDelete_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ImgDelete.MouseLeftButtonDown
        RaiseEvent DeleteAction(Me.Uid)
    End Sub
End Class
