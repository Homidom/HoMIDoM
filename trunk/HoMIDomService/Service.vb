Imports HoMIDom.HoMIDom
Imports System.IO
Imports System.Xml
Imports System.Xml.XPath
Imports System.ServiceModel
Imports System.ServiceModel.Description
Imports System.Xml.Serialization
Imports System.ServiceModel.Channels

'***********************************************
'** SERVICE HOMIDom - Simple exe qui sera ensuite convertit en service Windows
'** Ce service créer le serveur SOAP Web via l'interface IHoMIDom
'** Par défaut le service est lancé sur le port 7999
'** version 1.0
'** Date de création: 14/01/2011
'***********************************************

Module Service

    Dim myService As HoMIDom.HoMIDom.IHoMIDom
    Dim MyRep As String = System.Environment.CurrentDirectory
    Dim _IdSrv As String

    Sub Main()
        Try
            Console.SetWindowSize(132, 50)
            Console.SetBufferSize(200, 1000)
            Console.BackgroundColor = ConsoleColor.White 'Couleur du fond
            Console.Clear()  'Applique la couleur du fond
            Console.ForegroundColor = ConsoleColor.Black 'Couleur du texte

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

            Dim baseAddress As Uri = New Uri("http://localhost:" & PortSOAP & "/ServiceModelSamples/service")
            Dim fileServerAddress As Uri = New Uri("http://localhost:" & PortSOAP & "/ServiceModelSamples/fileServer")

            Console.WriteLine(Now & " Adresss SOAP: " & baseAddress.ToString)

            Using host As New ServiceHost(GetType(Server), baseAddress)
                host.CloseTimeout = TimeSpan.FromMinutes(60)
                host.OpenTimeout = TimeSpan.FromMinutes(60)
                AddHandler host.Faulted, AddressOf HostFaulted
                AddHandler host.UnknownMessageReceived, AddressOf HostUnknown
                host.Open()
                Console.WriteLine(Now & " ServiceWeb Démarré") ' & obj.PortTCP)
                Console.WriteLine("")

                'Connexion au serveur
                Dim myChannelFactory As ServiceModel.ChannelFactory(Of HoMIDom.HoMIDom.IHoMIDom) = Nothing

                Try
                    'myChannelFactory = New ServiceModel.ChannelFactory(Of HoMIDom.HoMIDom.IHoMIDom)("ConfigurationHttpHomidom")
                    Dim myadress As String = "http://localhost:" & PortSOAP & "/ServiceModelSamples/service"
                    Dim binding As New ServiceModel.BasicHttpBinding
                    Dim Context As OperationContext = OperationContext.Current

                    binding.MaxBufferPoolSize = 5000000
                    binding.MaxReceivedMessageSize = 5000000
                    binding.ReaderQuotas.MaxArrayLength = 5000000
                    binding.ReaderQuotas.MaxStringContentLength = 5000000
                    binding.SendTimeout = TimeSpan.FromMinutes(60)
                    binding.CloseTimeout = TimeSpan.FromMinutes(60)
                    binding.OpenTimeout = TimeSpan.FromMinutes(60)
                    binding.ReceiveTimeout = TimeSpan.FromMinutes(60)
                    myChannelFactory = New ServiceModel.ChannelFactory(Of HoMIDom.HoMIDom.IHoMIDom)(binding, New System.ServiceModel.EndpointAddress(myadress))

                    myService = myChannelFactory.CreateChannel()

                    'Démarrage du serveur pour charger la config
                    myService.Start()

                Catch ex As Exception
                    myChannelFactory.Abort()
                    Console.WriteLine(Now & " ERREUR: Erreur lors du lancement du service SOAP: " & ex.Message)
                End Try

                Using hostFileServer As New ServiceHost(GetType(HoMIDom.HoMIDom.FileServer), fileServerAddress)
                    hostFileServer.Open()
                    Console.WriteLine(Now & " Démarrage du serveur de fichiers OK")

                    'démarrage OK
                    Console.Beep()
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
            MsgBox("Erreur lors du service: " & ex.Message & vbCrLf & vbCrLf & ex.ToString, MsgBoxStyle.Critical, "ERREUR SERVICE")
            Console.WriteLine(Now & " ERREUR " & ex.Message & " : " & ex.ToString)
            Console.ReadLine()
        End Try
    End Sub

    Sub HostFaulted(ByVal sender As Object, ByVal e As System.EventArgs)
        Console.WriteLine(Now & " Le serveur s'est mis en erreur")
        MsgBox(Now & " Le serveur s'est mis en erreur")
    End Sub

    Sub HostUnknown(ByVal sender As Object, ByVal e As System.ServiceModel.UnknownMessageReceivedEventArgs)
        Console.WriteLine(Now & " Le serveur a reçu un message inconnu:" & e.Message.ToString)
        MsgBox(Now & " Le serveur a reçu un message inconnu:" & e.Message.ToString)
    End Sub

    Function LoadPort() As String
        Dim _portip As String = ""
        Try
            MyRep = MyRep & "\Config\homidom.xml"

            If File.Exists(MyRep) = True Then
                Dim myxml As XML
                Dim list As XmlNodeList

                myxml = New XML(MyRep)

                list = myxml.SelectNodes("/homidom/server")
                If list.Count > 0 Then 'présence des paramètres du server
                    For j As Integer = 0 To list.Item(0).Attributes.Count - 1
                        Select Case list.Item(0).Attributes.Item(j).Name
                            Case "portsoap"
                                _portip = list.Item(0).Attributes.Item(j).Value
                            Case "idsrv"
                                _IdSrv = list.Item(0).Attributes.Item(j).Value
                        End Select
                    Next
                Else
                    _portip = ""
                End If

            End If
            Return _portip
        Catch ex As Exception
            MsgBox("Erreur lors du service: " & ex.Message, MsgBoxStyle.Critical, "ERREUR SERVICE LoadPort")
            Console.WriteLine(Now & " ERREUR LoadPort : " & ex.Message & " : " & ex.ToString)
            Console.ReadLine()
            Return ""
        End Try
    End Function

    Sub close()
        myService.Stop(_IdSrv)
        End
    End Sub

End Module
