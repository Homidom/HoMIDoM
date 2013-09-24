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
            Try
                _Zoom = value
                Dim j As Double = (_ObjAction.Timing.Hour * 3600) + (_ObjAction.Timing.Minute * 60) + _ObjAction.Timing.Second
                j = (Span * j) / _Zoom
                Canvas.SetLeft(Rectangle1, j)
                Canvas.SetLeft(ImgDelete, j + 160)
                Fond.Width = j
            Catch ex As Exception
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uAction Zoom: " & ex.ToString, "ERREUR", "")
            End Try
        End Set
    End Property

    'Paramétrage de l'action 
    Private Sub Rectangle1_MouseLeftButtonDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Rectangle1.MouseLeftButtonDown
        Try
            Dim frm As New WActionParametrage(ObjAction)
            frm.ShowDialog()
            If frm.DialogResult.HasValue And frm.DialogResult.Value Then
                ObjAction = frm.ObjAction
                frm.Close()
                RaiseEvent ChangeAction(Me.Uid)
                Refresh_Position()
            End If
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uAction Rectangle1_MouseLeftButtonDown: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    'Mise à jour de la position de l'action dans le timeline suite à sa valeur de timing
    Sub Refresh_Position()
        Try
            Dim j As Double = (_ObjAction.Timing.Hour * 3600) + (_ObjAction.Timing.Minute * 60) + _ObjAction.Timing.Second
            j = (Span * j) / _Zoom
            Canvas.SetLeft(Rectangle1, j)
            Canvas.SetLeft(ImgDelete, j + 160)
        Catch ex As Exception
            AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur uAction Refresh_Position: " & ex.ToString, "ERREUR", "")
        End Try
    End Sub

    'Objet action
    Public Property ObjAction As Object
        Get
            Return _ObjAction
        End Get
        Set(ByVal value As Object)
            Try
                _ObjAction = value

                If _ObjAction IsNot Nothing Then
                    Dim i As Action.TypeAction = _ObjAction.TypeAction
                    Select Case i
                        Case Action.TypeAction.ActionDevice
                            Dim x As Action.ActionDevice = _ObjAction
                            Label1.Content = "Composant"
                            If x.IdDevice IsNot Nothing Then
                                If myService.ReturnDeviceByID(IdSrv, x.IdDevice) IsNot Nothing Then
                                    Dim _dev As TemplateDevice = myService.ReturnDeviceByID(IdSrv, x.IdDevice)
                                    Label1.Content = _dev.Name
                                    If String.IsNullOrEmpty(_dev.Picture) = False Then
                                        Image1.Source = ConvertArrayToImage(myService.GetByteFromImage(_dev.Picture), Image1.Height)
                                        Image1.Visibility = Windows.Visibility.Visible
                                    Else
                                        Image1.Source = New BitmapImage(New Uri("/HoMIAdmiN;component/Images/composant_32.png", UriKind.RelativeOrAbsolute))
                                        Image1.Visibility = Windows.Visibility.Visible
                                    End If
                                End If
                            End If
                            If x.Method IsNot Nothing Then Label2.Content = x.Method
                        Case Action.TypeAction.ActionDriver
                            Dim x As Action.ActionDriver = _ObjAction
                            Label1.Content = "Driver"
                            If x.IdDriver IsNot Nothing Then
                                If myService.ReturnDriverByID(IdSrv, x.IdDriver) IsNot Nothing Then
                                    Dim _drv As TemplateDriver = myService.ReturnDriverByID(IdSrv, x.IdDriver)
                                    Label1.Content = _drv.Nom
                                    If String.IsNullOrEmpty(_drv.Picture) = False Then
                                        Image1.Source = ConvertArrayToImage(myService.GetByteFromImage(_drv.Picture), Image1.Height)
                                        Image1.Visibility = Windows.Visibility.Visible
                                    Else
                                        Image1.Source = New BitmapImage(New Uri("/HoMIAdmiN;component/Images/Driver_32.png", UriKind.RelativeOrAbsolute))
                                        Image1.Visibility = Windows.Visibility.Visible
                                    End If
                                End If
                            End If
                            If x.Method IsNot Nothing Then Label2.Content = x.Method
                        Case Action.TypeAction.ActionMail
                            Dim x As Action.ActionMail = _ObjAction
                            Label1.Content = "Mail "
                            If x.UserId IsNot Nothing Then Label1.Content = Label1.Content & "{" & myService.ReturnUserById(IdSrv, x.UserId).UserName & "}"
                            If x.Sujet IsNot Nothing Then Label2.Content = x.Sujet

                            Image1.Source = New BitmapImage(New Uri("/HoMIAdmiN;component/Images/email_32.png", UriKind.RelativeOrAbsolute))
                            Image1.Visibility = Windows.Visibility.Visible
                        Case Action.TypeAction.ActionSpeech
                            Dim x As Action.ActionSpeech = _ObjAction
                            Label1.Content = "Parler "
                            If x.Message IsNot Nothing Then Label2.Content = x.Message

                            Image1.Source = New BitmapImage(New Uri("/HoMIAdmiN;component/Images/Speech_128.png", UriKind.RelativeOrAbsolute))
                            Image1.Visibility = Windows.Visibility.Visible
                        Case Action.TypeAction.ActionHttp
                            Dim x As Action.ActionHttp = _ObjAction
                            Label1.Content = "Commande Http"
                            If x.Commande IsNot Nothing Then Label2.Content = x.Commande

                            Image1.Source = New BitmapImage(New Uri("/HoMIAdmiN;component/Images/Cmd_Http_128.png", UriKind.RelativeOrAbsolute))
                            Image1.Visibility = Windows.Visibility.Visible
                        Case Action.TypeAction.ActionIf
                            Dim x As Action.ActionIf = _ObjAction
                            Label1.Content = "If"
                            Label2.Content = ""

                            Image1.Source = New BitmapImage(New Uri("/HoMIAdmiN;component/Images/workflow_32.png", UriKind.RelativeOrAbsolute))
                            Image1.Visibility = Windows.Visibility.Visible
                        Case Action.TypeAction.ActionMacro
                            Dim x As Action.ActionMacro = _ObjAction
                            Label1.Content = "Macro"
                            If x.IdMacro IsNot Nothing Then
                                Dim _macro As HoMIDom.HoMIDom.Macro = myService.ReturnMacroById(IdSrv, x.IdMacro)
                                If _macro IsNot Nothing Then Label2.Content = _macro.Nom
                            End If

                            Image1.Source = New BitmapImage(New Uri("/HoMIAdmiN;component/Images/script_32.png", UriKind.RelativeOrAbsolute))
                            Image1.Visibility = Windows.Visibility.Visible
                        Case Action.TypeAction.ActionLogEvent
                            Dim x As Action.ActionLogEvent = _ObjAction
                            Label1.Content = "Log Windows"

                            If x.Message IsNot Nothing Then Label2.Content = x.Message

                            Image1.Source = New BitmapImage(New Uri("/HoMIAdmiN;component/Images/Log_Windows.png", UriKind.RelativeOrAbsolute))
                            Image1.Visibility = Windows.Visibility.Visible
                        Case Action.TypeAction.ActionLogEventHomidom
                            Dim x As Action.ActionLogEventHomidom = _ObjAction
                            Label1.Content = "Log Homidom"
                            If x.Message IsNot Nothing Then Label2.Content = x.Message

                            Image1.Source = New BitmapImage(New Uri("/HoMIAdmiN;component/Images/Log_Homidom.png", UriKind.RelativeOrAbsolute))
                            Image1.Visibility = Windows.Visibility.Visible
                        Case Action.TypeAction.ActionDOS
                            Dim x As Action.ActionDos = _ObjAction
                            Label1.Content = "Commande Dos"
                            If x.Fichier IsNot Nothing Then Label2.Content = x.Fichier
                            If x.Arguments IsNot Nothing Then Label2.Content &= " " & x.Arguments

                            Image1.Source = New BitmapImage(New Uri("/HoMIAdmiN;component/Images/command_prompt_64.png", UriKind.RelativeOrAbsolute))
                            Image1.Visibility = Windows.Visibility.Visible
                        Case Action.TypeAction.ActionVB
                            Dim x As Action.ActionVB = _ObjAction
                            Label1.Content = "Script VB"
                            If x.Label IsNot Nothing Then Label2.Content = x.Label

                            Image1.Source = New BitmapImage(New Uri("/HoMIAdmiN;component/Images/VBS.png", UriKind.RelativeOrAbsolute))
                            Image1.Visibility = Windows.Visibility.Visible
                        Case Action.TypeAction.ActionStop
                            Dim x As Action.ActionSTOP = _ObjAction
                            Label1.Content = "STOP"
                            Label2.Content = ""

                            Image1.Source = New BitmapImage(New Uri("/HoMIAdmiN;component/Images/Signal_stop.png", UriKind.RelativeOrAbsolute))
                            Image1.Visibility = Windows.Visibility.Visible
                    End Select
                    Refresh_Position()
                End If
            Catch ex As Exception
                AfficheMessageAndLog(HoMIDom.HoMIDom.Server.TypeLog.ERREUR, "Erreur Property ObjAction: " & ex.ToString, "Erreur", "")
            End Try
        End Set
    End Property

    'Supprimer l'action du timeline
    Private Sub ImgDelete_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ImgDelete.MouseLeftButtonDown
        RaiseEvent DeleteAction(Me.Uid)
    End Sub

End Class
