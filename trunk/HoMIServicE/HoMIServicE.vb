Imports HoMIDom.HoMIDom
Imports System.IO
Imports System.Xml
Imports System.Xml.XPath
Imports System.ServiceModel
Imports System.ServiceModel.Description
Imports System.Xml.Serialization
Imports System.ServiceModel.Channels
Imports System.Net
Imports STRGS = Microsoft.VisualBasic.Strings

Public Class HoMIServicE

    Dim myService As IHoMIDom
    'Dim MyRep As String = System.Environment.CurrentDirectory
    Dim MyRep As String = My.Application.Info.DirectoryPath
    Dim _IdSrv As String
    Dim _Addrip As String = "localhost"
    Dim host As ServiceHost
    Dim hostFileServer As ServiceHost

    Protected Overrides Sub OnStart(ByVal args() As String)
        Try
            If (Environment.UserInteractive) Then
                Dim largeur As Integer = Console.WindowWidth
                Dim hauteur As Integer = Console.WindowHeight
                Try
                    Console.SetWindowSize(115, 48)
                Catch ex As Exception
                    Console.SetWindowSize(largeur, hauteur)
                End Try
                Console.SetBufferSize(200, 1000)
                Console.BackgroundColor = ConsoleColor.White 'Couleur du fond
                Console.Clear()  'Applique la couleur du fond
                Console.ForegroundColor = ConsoleColor.Black 'Couleur du texte
                Console.Title = "HoMIService"
                Console.WriteLine(Now & " INFO    ******************************")
                Console.WriteLine(Now & " INFO    **** DEMARRAGE DU SERVEUR ****")
                Console.WriteLine(" ")
            End If

            'Démarrage du serviceWeb
            log("Service Web -> Démarrage")

            'MyRep = "c:\homidom"
            'MyRep = My.Application.Info.DirectoryPath

            Dim PortSOAP As String = LoadPort()
            If PortSOAP = "" Or IsNumeric(PortSOAP) = False Then
                PortSOAP = "7999"
                logerror("Le fichier de config ou la balise portsoap n'ont pas été trouvé !")
            End If
            If _Addrip = "" Then
                _Addrip = "localhost"
                logerror("Le fichier de config ou la balise ip n'ont pas été trouvé, l'adresse par défaut sera localhost !")
            End If
            Dim baseAddress As Uri = New Uri("http://" & _Addrip & ":" & PortSOAP & "/service")
            Dim fileServerAddress As Uri = New Uri("http://" & _Addrip & ":" & PortSOAP & "/fileServer")
            log("Adresss SOAP: " & _Addrip & ":" & PortSOAP)
            host = New ServiceHost(GetType(Server), baseAddress)
            host.CloseTimeout = TimeSpan.FromMinutes(60)
            host.OpenTimeout = TimeSpan.FromMinutes(60)
            AddHandler host.Faulted, AddressOf HostFaulted
            'AddHandler host.UnknownMessageReceived, AddressOf HostUnknown
            host.Open()
            log("Service Web -> Démarré : http://" & _Addrip & ":" & PortSOAP & "/service")
            If (Environment.UserInteractive) Then Console.WriteLine("")

            'Connexion au serveur
            Dim myChannelFactory As ServiceModel.ChannelFactory(Of IHoMIDom) = Nothing
            Try
                log("Serveur Homidom -> Démarrage")
                'Dim myadress As String = "http://" & Dns.GetHostName() & ":" & PortSOAP & "/service"
                Dim myadress As String = "http://" & _Addrip & ":" & PortSOAP & "/service"
                Dim binding As New ServiceModel.BasicHttpBinding
                binding.MaxBufferPoolSize = 250000000
                binding.MaxReceivedMessageSize = Integer.MaxValue
                binding.MaxBufferSize = Integer.MaxValue
                binding.ReaderQuotas.MaxArrayLength = 250000000
                binding.ReaderQuotas.MaxNameTableCharCount = 250000000
                binding.ReaderQuotas.MaxBytesPerRead = 250000000
                binding.ReaderQuotas.MaxStringContentLength = 250000000
                binding.SendTimeout = TimeSpan.FromMinutes(60)
                binding.CloseTimeout = TimeSpan.FromMinutes(60)
                binding.OpenTimeout = TimeSpan.FromMinutes(60)
                binding.ReceiveTimeout = TimeSpan.FromMinutes(60)
                myChannelFactory = New ServiceModel.ChannelFactory(Of IHoMIDom)(binding, New System.ServiceModel.EndpointAddress(myadress))
                myService = myChannelFactory.CreateChannel()
                'Démarrage du serveur pour charger la config
                myService.Start()
                log("Serveur HoMIDoM -> démarré")
            Catch ex As Exception
                myChannelFactory.Abort()
                logerror("Erreur lors du démmarage du serveur HoMIDoM : " & ex.Message)
            End Try

            'démmarage de l'API WEB
            Dim apiServerAddress As Uri = New Uri("http://" & _Addrip & ":" & PortSOAP)
            HoMIDomWebAPI.HoMIDomAPI.CurrentServer = Server.Instance
            HoMIDomWebAPI.HoMIDomAPI.Start(apiServerAddress.ToString(), _IdSrv)
            'log("API Web démarré : http://" & _Addrip & ":" & PortSOAP & "/api")

            'démarrage du serveur de fichier
            hostFileServer = New ServiceHost(GetType(FileServer), fileServerAddress)
            hostFileServer.Open()
            log("Serveur de fichiers démarré : http://" & _Addrip & ":" & PortSOAP & "/fileServer")

            'démarrage OK
            If (Environment.UserInteractive) Then
                Console.WriteLine(" ")
                Console.WriteLine(Now & " INFO    ****   SERVEUR DEMARRE    ****")
                Console.WriteLine(Now & " INFO    ******************************")
                Console.WriteLine(" ")
            End If

        Catch ex As Exception
            Dim message As String = ex.ToString
            message = DelRep(message)

            If LCase(ex.ToString).Contains("badimageformat") And LCase(ex.ToString).Contains("sqlite") Then
                logerror("Veuillez vérifier que la dll sqlite installée dans le répertoire du service correspond bien à la version de votre OS (32 ou 64 bits)!!")
                logerror("ERREUR " & ex.Message & " : " & message)
            Else
                logerror("Erreur lors du démarrage service: " & ex.Message & vbCrLf & vbCrLf & message)
            End If
            'If (Environment.UserInteractive) Then Console.ReadLine()
        End Try

        'en mode interactif (console) on attend une frappe du clavier pour fermer l'appli
        Try
            If (Environment.UserInteractive) Then
                Console.WriteLine(" ")
                Console.WriteLine("Press any key to stop program")
                Console.WriteLine(" ")
                Console.Read()
                OnStop()
            End If
        Catch ex As Exception
            Dim message As String = ex.ToString
            message = DelRep(message)
            logerror("Erreur lors du démarrage service: " & ex.Message & vbCrLf & vbCrLf & message)
            If (Environment.UserInteractive) Then Console.ReadLine()
        End Try
    End Sub

    Sub log(ByVal texte As String)
        If (Environment.UserInteractive) Then
            'log dans la console
            Console.WriteLine(Now & " INFO    " & texte)
        Else
            'log dans les events
            Dim myEventLog = New EventLog()
            myEventLog.Source = "HoMIServicE"
            myEventLog.WriteEntry(texte, EventLogEntryType.Information, 0)

        End If
    End Sub
    Sub logerror(ByVal texte As String)
        If (Environment.UserInteractive) Then
            'log dans la console
            Console.WriteLine(Now & " ERROR   " & texte)
        Else
            'log dans les events
            Dim myEventLog = New EventLog()
            myEventLog.Source = "HoMIServicE"
            myEventLog.WriteEntry(texte, EventLogEntryType.Error, 90)
        End If
    End Sub

    Protected Overrides Sub OnStop()
        Try
            log("****   ARRET DU SERVEUR    ****")
            myService.Stop(_IdSrv)

            log("****   ARRET DU FILESERVER ET HOST    ****")
            hostFileServer.Close()
            host.Close()

            log("****   SERVEUR ARRETE    ****")

            Me.close()
        Catch ex As Exception
            Dim message As String = ex.ToString
            message = DelRep(message)
            logerror("ERREUR " & ex.Message & " : " & Message)
            If (Environment.UserInteractive) Then Console.ReadLine()
        End Try

    End Sub

    ''' <summary>
    ''' Supprime le répertoire du développeur
    ''' </summary>
    ''' <param name="Message"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function DelRep(ByVal Message As String) As String
        Try
            Dim i As Integer = 1
            Dim newmess As String = Message
            Dim start As Integer

            start = InStr(i, UCase(newmess), "C:\")
            If start = 0 Then
                start = InStr(i, UCase(newmess), "D:\")
            End If

            Do While start > 0
                Dim newstart As Integer = InStr(start, LCase(newmess), "\homidom\")
                newmess = Mid(newmess, 1, start + 2) & "..." & Mid(newmess, newstart, newmess.Length - newstart + 1)
                i = start + 5
                start = InStr(i, UCase(newmess), "C:\")
                If start = 0 Then
                    start = InStr(i, UCase(newmess), "D:\")
                End If
            Loop

            Return newmess
        Catch ex As Exception
            Return Message
        End Try
    End Function

    Sub HostFaulted(ByVal sender As Object, ByVal e As System.EventArgs)
        logerror("Le serveur s'est mis en erreur")
    End Sub

    Sub HostUnknown(ByVal sender As Object, ByVal e As System.ServiceModel.UnknownMessageReceivedEventArgs)
        logerror("Le serveur a reçu un message inconnu: " & e.Message.ToString)
    End Sub

    Function LoadPort() As String
        Try
            Dim _portip As String = ""
            MyRep = MyRep & "\Config\homidom.xml"

            If File.Exists(MyRep) = True Then
                Dim myxml As XML
                Dim list As XmlNodeList

                myxml = New XML(MyRep)

                list = myxml.SelectNodes("/homidom/server")
                If list IsNot Nothing Then
                    If list.Count > 0 Then 'présence des paramètres du server
                        For j As Integer = 0 To list.Item(0).Attributes.Count - 1
                            Select Case list.Item(0).Attributes.Item(j).Name
                                Case "ipsoap"
                                    _Addrip = list.Item(0).Attributes.Item(j).Value
                                Case "portsoap"
                                    _portip = list.Item(0).Attributes.Item(j).Value
                                Case "idsrv"
                                    _IdSrv = list.Item(0).Attributes.Item(j).Value
                            End Select
                        Next
                    Else
                        _portip = ""
                    End If
                    list = Nothing
                End If
                myxml = Nothing
            End If

            Return _portip
        Catch ex As Exception
            logerror("Erreur lors de la lecture des paramètres du serveur dans le fichier xml: " & ex.Message)
            Return ""

            'Dim reponse As MsgBoxResult = MsgBox("Erreur lors de la lecture des paramètres du serveur dans le fichier xml: " & ex.Message & vbCrLf & "Voulez-vous tenter de récupérer ces paramètres depuis le fichier de config sauvegardé?", MsgBoxStyle.YesNo, "ERREUR SERVICE LoadPort")
            'Console.WriteLine(Now & " ERREUR LoadPort : " & ex.Message & " : " & ex.ToString)

            'If reponse = MsgBoxResult.Yes Then
            '    Try 'Seconde chance
            '        Dim _portip As String = ""
            '        MyRep = MyRep & "\Config\homidom.sav"

            '        If File.Exists(MyRep) = True Then
            '            Dim myxml As XML
            '            Dim list As XmlNodeList

            '            myxml = New XML(MyRep)

            '            list = myxml.SelectNodes("/homidom/server")
            '            If list IsNot Nothing Then
            '                If list.Count > 0 Then 'présence des paramètres du server
            '                    For j As Integer = 0 To list.Item(0).Attributes.Count - 1
            '                        Select Case list.Item(0).Attributes.Item(j).Name
            '                            Case "ipsoap"
            '                                _Addrip = list.Item(0).Attributes.Item(j).Value
            '                            Case "portsoap"
            '                                _portip = list.Item(0).Attributes.Item(j).Value
            '                            Case "idsrv"
            '                                _IdSrv = list.Item(0).Attributes.Item(j).Value
            '                        End Select
            '                    Next
            '                Else
            '                    _portip = ""
            '                End If
            '                list = Nothing
            '            End If
            '            myxml = Nothing
            '        End If
            '        Return _portip
            '    Catch ex2 As Exception
            '        MsgBox("Erreur lors de la lecture des paramètres du serveur dans le fichier xml de sauvegarde: " & ex.Message, MsgBoxStyle.Critical, "ERREUR SERVICE LoadPort")
            '        Console.WriteLine(Now & " ERREUR LoadPort seconde chance: " & ex.Message & " : " & ex.ToString)
            '        Console.ReadLine()
            '        Return ""
            '    End Try
            'Else
            '    Return ""
            'End If
        End Try
    End Function

    Sub close()
        Try
            myService.Stop(_IdSrv)
            End
        Catch ex As Exception
            logerror("Erreur lors de l'arret du service: " & ex.Message)
            If (Environment.UserInteractive) Then Console.ReadLine()
        End Try
    End Sub


End Class
