Imports HoMIDom.HoMIDom
Imports System.ServiceModel
Imports System.Xml
Imports System.Net
Imports System.Windows.Threading

Public NotInheritable Class LogsForm
    Dim myfile As String = My.Application.Info.DirectoryPath & "\Config\Homidom.xml"
    Dim myChannelFactory As ServiceModel.ChannelFactory(Of HoMIDom.HoMIDom.IHoMIDom) = Nothing
    Dim myService As HoMIDom.HoMIDom.IHoMIDom
    Dim IsConnect As Boolean = False 'True si connecté au serveur
    Dim _IPSOAP, _PortSOAP, _IdSrv, _PortSrvWeb, _EnableSrvWeb As String
    Dim dt As DispatcherTimer = New DispatcherTimer()

    Private Sub LogsForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            'connexion au serveur
            connexion_SOAP()

            'lancement timer logs
            AddHandler dt.Tick, AddressOf LastLogs_Tick
            dt.Interval = New TimeSpan(0, 0, 0, 1)
            dt.Start()
        Catch ex As Exception
            MsgBox("Erreur Load : " & ex.ToString, MsgBoxStyle.Critical, "HoMIGuI - Logs ERROR")
        End Try
    End Sub

    Private Sub LogsForm_Close(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.FormClosing
        Try
            RemoveHandler dt.Tick, AddressOf LastLogs_Tick
            If IsConnect Then myChannelFactory.Abort()
        Catch ex As Exception
            MsgBox("Erreur lors de la fermeture")
        End Try
    End Sub

    Private Sub connexion_SOAP()
        Try
            'Lecture des settings dans la config du serveur
            Try
                Me.Cursor = Cursors.WaitCursor
                If System.IO.File.Exists(myfile) Then
                    Dim doc As New XmlDocument
                    Dim List As XmlNodeList
                    doc.Load(myfile)
                    'execute et récupère la valeur de la requête
                    List = doc.SelectNodes("/homidom/server")
                    If List.Count > 0 Then 'présence des paramètres du server
                        For j As Integer = 0 To List.Item(0).Attributes.Count - 1
                            Select Case List.Item(0).Attributes.Item(j).Name
                                Case "ipsoap" : _IPSOAP = List.Item(0).Attributes.Item(j).Value
                                Case "portsoap" : _PortSOAP = List.Item(0).Attributes.Item(j).Value
                                Case "idsrv" : _IdSrv = List.Item(0).Attributes.Item(j).Value
                                Case "portweb" : _PortSrvWeb = List.Item(0).Attributes.Item(j).Value
                                Case "enablesrvweb" : _EnableSrvWeb = List.Item(0).Attributes.Item(j).Value
                            End Select
                        Next
                        List = Nothing
                        doc = Nothing
                    Else
                        List = Nothing
                        doc = Nothing
                        Me.Cursor = Cursors.Default
                        MsgBox("Il manque les paramètres du serveur dans le fichier de config du service !!", MsgBoxStyle.OkOnly)
                        Exit Sub
                    End If
                Else
                    Me.Cursor = Cursors.Default
                    MsgBox("impossible de trouver le fichier de config du service : " & myfile, MsgBoxStyle.OkOnly)
                    Exit Sub
                End If
            Catch ex As Exception
                Me.Cursor = Cursors.Default
                MsgBox("Erreur lors de la connexion au serveur : " & Chr(10) & _IPSOAP & ":" & _PortSOAP & vbCrLf & "Veuillez vérifier que celui-ci est démarré", MsgBoxStyle.OkOnly)
                Exit Sub
            End Try

            'Connexion SOAP au serveur
            Try
                Dim myadress As String = "http://" & _IPSOAP & ":" & _PortSOAP & "/service"

                Try
                    Dim url As New Uri(myadress)
                    Dim Request As HttpWebRequest = CType(HttpWebRequest.Create(url), System.Net.HttpWebRequest)
                    Dim response As Net.HttpWebResponse = CType(Request.GetResponse(), Net.HttpWebResponse)
                Catch ex As Exception
                    Me.Cursor = Cursors.Default
                    MsgBox("Erreur lors de la connexion au serveur : " & Chr(10) & _IPSOAP & ":" & _PortSOAP & vbCrLf & "Veuillez vérifier que celui-ci est démarré", MsgBoxStyle.OkOnly)
                    Exit Sub
                End Try

                Dim binding As New ServiceModel.BasicHttpBinding
                binding.MaxBufferPoolSize = 250000000
                binding.MaxReceivedMessageSize = 250000000
                binding.MaxBufferSize = 250000000
                binding.ReaderQuotas.MaxArrayLength = 250000000
                binding.ReaderQuotas.MaxNameTableCharCount = 250000000
                binding.ReaderQuotas.MaxBytesPerRead = 250000000
                binding.ReaderQuotas.MaxStringContentLength = 250000000
                binding.SendTimeout = TimeSpan.FromMinutes(60)
                binding.CloseTimeout = TimeSpan.FromMinutes(60)
                binding.OpenTimeout = TimeSpan.FromMinutes(60)
                binding.ReceiveTimeout = TimeSpan.FromMinutes(60)

                myChannelFactory = New ServiceModel.ChannelFactory(Of HoMIDom.HoMIDom.IHoMIDom)(binding, New System.ServiceModel.EndpointAddress(myadress))
                myService = myChannelFactory.CreateChannel()
                If myChannelFactory.State <> CommunicationState.Opened Then
                    Me.Cursor = Cursors.Default
                    MsgBox("Erreur lors de la connexion au serveur : " & Chr(10) & _IPSOAP & ":" & _PortSOAP & vbCrLf & "Veuillez vérifier que celui-ci est démarré", MsgBoxStyle.OkOnly)
                    Exit Sub
                Else
                    Try
                        myService.GetTime()
                    Catch ex As Exception
                        myChannelFactory.Abort()
                        IsConnect = False
                        Me.Cursor = Cursors.Default
                        MsgBox("Erreur lors de la connexion au serveur : " & Chr(10) & _IPSOAP & ":" & _PortSOAP & vbCrLf & ex.ToString, MsgBoxStyle.OkOnly)
                        Exit Sub
                    End Try

                    If myService.GetIdServer(_IdSrv) = "99" Then
                        Me.Cursor = Cursors.Default
                        MsgBox("L'ID du serveur est erroné, impossible de communiquer avec celui-ci", MsgBoxStyle.OkOnly)
                        Exit Sub
                    End If

                    IsConnect = True
                End If

                binding = Nothing
            Catch ex As Exception
                myChannelFactory.Abort()
                IsConnect = False
                Me.Cursor = Cursors.Default
                MsgBox("Erreur lors de la connexion au serveur : " & ex.Message, MsgBoxStyle.OkOnly)
                Exit Sub
            End Try
            Me.Cursor = Cursors.Default

        Catch ex As Exception
            Me.Cursor = Cursors.Default
            MsgBox("Erreur lors de la connexion au serveur : " & ex.Message, MsgBoxStyle.OkOnly)
            Exit Sub
        End Try
    End Sub

    Private Sub LastLogs_Tick(ByVal sender As Object, ByVal e As EventArgs)
        Try
            If IsConnect Then
                Me.Cursor = Cursors.WaitCursor
                Dim list As List(Of String) = myService.GetLastLogs
                Dim _string As String = ""
                If list.Count > 0 Then
                    For Each logtext As String In list
                        If String.IsNullOrEmpty(logtext) = False Then _string &= logtext & vbCrLf
                    Next
                    LogsTextBox.Text = _string
                    list.Clear()
                Else
                    LogsTextBox.Text = "Pas de logs"
                End If

                list = myService.GetLastLogsError
                _string = ""
                If list.Count > 0 Then
                    For Each logerror As String In list
                        If String.IsNullOrEmpty(logerror) = False Then _string &= logerror & vbCrLf
                    Next
                    ErrorsTextBox.Text = _string
                    _string = Nothing
                Else
                    ErrorsTextBox.Text = "Pas d'errreur"
                End If
                list.Clear()
                Me.Cursor = Cursors.Default
            Else
                LogsTextBox.Text = "Non connecté au serveur"
                ErrorsTextBox.Text = "Non connecté au serveur"
            End If

        Catch ex As Exception
            Me.Cursor = Cursors.Default
            LogsTextBox.Text = "ERREUR lors de la récupération des logs du serveur"
            ErrorsTextBox.Text = "ERREUR lors de la récupération des logs du serveur"
        End Try

    End Sub

End Class