Imports HoMIDom.HoMIDom

Public Class uAction
    'Variables
    Dim _ObjAction As Object
    Public Span As Integer
    Public _Zoom As Integer

    'Evenement 
    Public Event DeleteAction(ByVal ID As String)
    Public Event ChangeAction(ByVal ID As String)

    'Propriété zoom, placer l'action dans le timeline
    Public Property Zoom As Integer
        Get
            Return _Zoom
        End Get
        Set(ByVal value As Integer)
            _Zoom = value
            Dim j As Double = (_ObjAction.Timing.Hour * 3600) + (_ObjAction.Timing.Minute * 60) + _ObjAction.Timing.Second
            j = (Span * j) / _Zoom
            Me.Fond.SetLeft(Rectangle1, j)
            Me.Fond.SetLeft(ImgDelete, j + 160)
        End Set
    End Property

    'Paramétrage de l'action 
    Private Sub Rectangle1_MouseLeftButtonDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Rectangle1.MouseLeftButtonDown
        Dim frm As New WActionParametrage(ObjAction)
        frm.ShowDialog()
        If frm.DialogResult.HasValue And frm.DialogResult.Value Then
            ObjAction = frm.ObjAction
            frm.Close()
            RaiseEvent ChangeAction(Me.Uid)
        End If
        Refresh_Position()
    End Sub

    'Mise à jour de la position de l'action dans le timeline suite à sa valeur de timing
    Sub Refresh_Position()
        Dim j As Double = (_ObjAction.Timing.Hour * 3600) + (_ObjAction.Timing.Minute * 60) + _ObjAction.Timing.Second
        j = (Span * j) / _Zoom
        Fond.SetLeft(Rectangle1, j)
        Fond.SetLeft(ImgDelete, j + 160)
    End Sub

    'Objet action
    Public Property ObjAction As Object
        Get
            Return _ObjAction
        End Get
        Set(ByVal value As Object)
            _ObjAction = value
            If _ObjAction IsNot Nothing Then
                Dim i As HoMIDom.HoMIDom.Action.TypeAction = _ObjAction.TypeAction
                Select Case i
                    Case Action.TypeAction.ActionDevice
                        Dim x As HoMIDom.HoMIDom.Action.ActionDevice
                        x = _ObjAction
                        If x.IdDevice IsNot Nothing Then Label1.Content = Window1.myService.ReturnDeviceByID(x.IdDevice).Name
                        If x.Method IsNot Nothing Then Label2.Content = x.Method
                    Case Action.TypeAction.ActionMail
                        Dim x As HoMIDom.HoMIDom.Action.ActionMail
                        x = _ObjAction
                        Label1.Content = "Mail "
                        If x.UserId IsNot Nothing Then Label1.Content = Label1.Content & "{" & Window1.myService.ReturnUserById(x.UserId).Nom & "}"
                        If x.Sujet IsNot Nothing Then Label2.Content = x.Sujet
                    Case Action.TypeAction.ActionIf
                        Dim x As HoMIDom.HoMIDom.Action.ActionIf
                        Label1.Content = "If"
                        x = _ObjAction
                        Label2.Content = ""
                End Select
                Refresh_Position()
            End If
        End Set
    End Property

    'Supprimer l'action du timeline
    Private Sub ImgDelete_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ImgDelete.MouseLeftButtonDown
        RaiseEvent DeleteAction(Me.Uid)
    End Sub
End Class
