Imports HoMIDom.HoMIDom
'Imports HoMIDomWebAPI
Imports System.IO
Imports System.Xml
Imports System.Xml.XPath
Imports System.ServiceModel
Imports System.ServiceModel.Description
Imports System.Xml.Serialization
Imports System.ServiceModel.Channels
Imports System.Net

'***********************************************
'** SERVICE HOMIDom - Simple exe qui sera ensuite convertit en service Windows
'** Ce service créer le serveur SOAP Web via l'interface IHoMIDom
'** Par défaut le service est lancé sur le port 7999
'** version 1.0
'** Date de création: 14/01/2011
'***********************************************

Module Service

    Dim myService As IHoMIDom
    Dim MyRep As String = System.Environment.CurrentDirectory
    Dim _IdSrv As String
    Dim _Addrip As String = "localhost"

    Sub Main()
        Try
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
            Console.Title = "HoMIDomService"

            Console.WriteLine("******************************")
            Console.WriteLine("**** DEMARRAGE DU SERVEUR ****")
            Console.WriteLine(" ")

            'Démarrage du serviceWeb
            Console.WriteLine(Now & " Start ServiceWeb")
            Dim PortSOAP As String = LoadPort()
            If PortSOAP = "" Or IsNumeric(PortSOAP) = False Then
                PortSOAP = "7999"
                Console.WriteLine(Now & "ERREUR: Le fichier de config ou la balise portsoap n'ont pas été trouvé !")
            End If
            If _Addrip = "" Then
                _Addrip = "localhost"
                Console.WriteLine(Now & "ERREUR: Le fichier de config ou la balise ip n'ont pas été trouvé, l'adresse par défaut sera localhost !")
            End If

            Dim baseAddress As Uri = New Uri("http://" & _Addrip & ":" & PortSOAP & "/ServiceModelSamples/service")
            Dim fileServerAddress As Uri = New Uri("http://" & _Addrip & ":" & PortSOAP & "/ServiceModelSamples/fileServer")
            'Dim ServerCallBack As Uri = New Uri("http://" & _Addrip & ":" & "8000" & "/callback")

            'Dim CallBackAddress As Uri = New Uri("http://" & _Addrip & ":" & PortSOAP & "/ServiceModelSamples/callback")
            'Dim baseAddress As Uri = New Uri("http://" & Dns.GetHostName() & ":" & PortSOAP & "/ServiceModelSamples/service")
            'Dim fileServerAddress As Uri = New Uri("http://" & Dns.GetHostName() & ":" & PortSOAP & "/ServiceModelSamples/fileServer")

            Console.WriteLine(Now & " Adresss SOAP: " & _Addrip & ":" & PortSOAP)

            'Dim hostsrvcallback As New ServiceHost(GetType(HomiCallBack.HoMIDom.MyServiceCallBack), ServerCallBack)
            'hostsrvcallback.Open()
            'Console.WriteLine(Now & " Service CallBack démarré sur l'adresse: " & ServerCallBack.ToString())

            Using host As New ServiceHost(GetType(Server), baseAddress)

                host.CloseTimeout = TimeSpan.FromMinutes(60)
                host.OpenTimeout = TimeSpan.FromMinutes(60)
                AddHandler host.Faulted, AddressOf HostFaulted
                'AddHandler host.UnknownMessageReceived, AddressOf HostUnknown
                host.Open()
                Console.WriteLine(Now & " ServiceWeb Démarré") ' & obj.PortTCP)
                Console.WriteLine("")

                'Connexion au serveur
                Dim myChannelFactory As ServiceModel.ChannelFactory(Of IHoMIDom) = Nothing

                Try
                    'myChannelFactory = New ServiceModel.ChannelFactory(Of HoMIDom.HoMIDom.IHoMIDom)("ConfigurationHttpHomidom")
                    'Dim myadress As String = "http://" & _Addrip & ":" & PortSOAP & "/ServiceModelSamples/service"
                    Dim myadress As String = "http://" & Dns.GetHostName() & ":" & PortSOAP & "/ServiceModelSamples/service"
                    Dim binding As New ServiceModel.BasicHttpBinding
                    'Dim Context As OperationContext = OperationContext.Current 'non utilisé donc commenté : DMS
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

                Catch ex As Exception
                    myChannelFactory.Abort()
                    Console.WriteLine(Now & " ERREUR: Erreur lors du lancement du service SOAP: " & ex.Message)
                End Try

                Dim apiServerAddress As Uri = New Uri("http://" & _Addrip & ":" & PortSOAP)
                HoMIDomWebAPI.HoMIDomAPI.CurrentServer = Server.Instance
                HoMIDomWebAPI.HoMIDomAPI.Start(apiServerAddress.ToString(), _IdSrv)

                Using hostFileServer As New ServiceHost(GetType(FileServer), fileServerAddress)
                    hostFileServer.Open()
                    Console.WriteLine(Now & " Serveur de fichiers démarré sur l'adresse: " & fileServerAddress.ToString())

                    'démarrage OK
                    '                    Console.Beep()
                    Console.WriteLine(" ")
                    Console.WriteLine("******************************")
                    Console.WriteLine("****   SERVEUR DEMARRE    ****")
                    Console.WriteLine("******************************")
                    Console.WriteLine(" ")

                    Console.ReadLine()

                    'fin --> on arrete
                    myService.Stop(_IdSrv)

                    hostFileServer.Close()
                End Using
                host.Close()
            End Using

        Catch ex As Exception
            Dim message As String = ex.ToString
            message = DelRep(message)

            MsgBox("Erreur lors du démarrage service: " & ex.Message & vbCrLf & vbCrLf & message, MsgBoxStyle.Critical, "ERREUR SERVICE")

            If LCase(ex.ToString).Contains("badimageformat") And LCase(ex.ToString).Contains("sqlite") Then
                MsgBox("Veuillez vérifier que la dll sqlite installée dans le répertoire du service correspond bien à la version de votre OS (32 ou 64 bits)!!", MsgBoxStyle.Information, "RESOLUTION")
            End If

            Console.WriteLine(Now & " ERREUR " & ex.Message & " : " & message)
            Console.ReadLine()
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
        Console.WriteLine(Now & " Le serveur s'est mis en erreur")
        MsgBox(Now & " Le serveur s'est mis en erreur")
    End Sub

    Sub HostUnknown(ByVal sender As Object, ByVal e As System.ServiceModel.UnknownMessageReceivedEventArgs)
        Console.WriteLine(Now & " Le serveur a reçu un message inconnu:" & e.Message.ToString)
        MsgBox(Now & " Le serveur a reçu un message inconnu:" & e.Message.ToString)
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
            Dim reponse As MsgBoxResult = MsgBox("Erreur lors de la lecture des paramètres du serveur dans le fichier xml: " & ex.Message & vbCrLf & "Voulez-vous tenter de récupérer ces paramètres depuis le fichier de config sauvegardé?", MsgBoxStyle.YesNo, "ERREUR SERVICE LoadPort")
            Console.WriteLine(Now & " ERREUR LoadPort : " & ex.Message & " : " & ex.ToString)

            If reponse = MsgBoxResult.Yes Then
                Try 'Seconde chance
                    Dim _portip As String = ""
                    MyRep = MyRep & "\Config\homidom.sav"

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
                Catch ex2 As Exception
                    MsgBox("Erreur lors de la lecture des paramètres du serveur dans le fichier xml de sauvegarde: " & ex.Message, MsgBoxStyle.Critical, "ERREUR SERVICE LoadPort")
                    Console.WriteLine(Now & " ERREUR LoadPort seconde chance: " & ex.Message & " : " & ex.ToString)
                    Console.ReadLine()
                    Return ""
                End Try
            Else
                Return ""
            End If
        End Try
    End Function

    Sub close()
        Try
            myService.Stop(_IdSrv)
            End
        Catch ex As Exception
            MsgBox("Erreur lors de l'arret du service: " & ex.Message, MsgBoxStyle.Critical, "ERREUR SERVICE close")
            Console.WriteLine(Now & " ERREUR SERVICE close : " & ex.Message & " : " & ex.ToString)
            Console.ReadLine()
        End Try
    End Sub

End Module
