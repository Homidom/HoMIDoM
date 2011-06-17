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
            Dim j As Double = (_ObjAction.Timing.Minute * 60) + _ObjAction.Timing.Second
            j = (Span * j) / Zoom
            Fond.SetLeft(Rectangle1, j)
            Fond.SetLeft(ImgDelete, j + 160)
        End Set
    End Property

    'Paramétrage de l'action 
    Private Sub Rectangle1_MouseLeftButtonDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Rectangle1.MouseLeftButtonDown
        Dim frm As New WActionParametrage(_ObjAction)
        frm.ShowDialog()
        If frm.DialogResult.HasValue And frm.DialogResult.Value Then
            _ObjAction = frm.ObjAction
            frm.Close()
            RaiseEvent ChangeAction(Me.Uid)
        End If
        Refresh_Position()
    End Sub

    'Mise à jour de la position de l'action dans le timeline suite à sa valeur de timing
    Sub Refresh_Position()
        Dim j As Double = (ObjAction.Timing.Hour * 3600) + (ObjAction.Timing.Minute * 60) + ObjAction.Timing.Second
        j = (Span * j) / Zoom
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
                        Label1.Content = Window1.myService.ReturnDeviceByID(x.IdDevice).Name
                        Label2.Content = x.Method
                    Case Action.TypeAction.ActionMail
                        Dim x As HoMIDom.HoMIDom.Action.ActionMail
                        x = _ObjAction
                        Label1.Content = "Mail {" & x.To & "}"
                        Label2.Content = x.Sujet
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
