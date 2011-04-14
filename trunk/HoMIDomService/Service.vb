Imports HoMIDom.HoMIDom
Imports System.IO
Imports System.Xml
Imports System.Xml.XPath
Imports System.ServiceModel
Imports System.ServiceModel.Description

'***********************************************
'** SERVICE HOMIDom - Simple exe qui sera ensuite convertit en service Windows
'** Ce service créer le serveur SOAP Web via l'interface IHoMIDom
'** Par défaut le service est lancé sur le port 8000
'** version 1.0
'** Date de création: 14/01/2011
'** Historique (SebBergues): 14/01/2011: Création 
'** Historique (SebBergues) : 21/01/2011: Ajout de la gestion de la config
'***********************************************

Module Service

    Dim myService As HoMIDom.HoMIDom.IHoMIDom
    Dim MyRep As String = System.Environment.CurrentDirectory

    Sub Main()
        Try

            Console.WriteLine("******************************")
            Console.WriteLine("DEMARRAGE DU SERVEUR**********")
            Console.WriteLine("******************************")
            Console.WriteLine(" ")

            'Démarrage du serviceWeb
            Console.WriteLine(Now & " Start ServiceWeb")
            Dim PortSOAP As String = LoadPort()
            If PortSOAP = "" Or IsNumeric(PortSOAP) = False Then
                PortSOAP = "8000"
                Console.WriteLine(Now & "ERREUR: Le fichier de config ou la balise portsoap n'ont pas été trouvé !")
            End If

            
            Dim baseAddress As Uri = New Uri("http://localhost:" & PortSOAP & "/ServiceModelSamples/service")
            Dim fileServerAddress As Uri = New Uri("http://localhost:" & PortSOAP & "/ServiceModelSamples/fileServer")

            Console.WriteLine(Now & " Adresss SOAP: " & baseAddress.ToString)

            Using host As New ServiceHost(GetType(Server), baseAddress)
                host.Open()

                Console.WriteLine(Now & " ServiceWeb Démarré") ' & obj.PortTCP)

                Console.WriteLine("******************************")
                Console.WriteLine("SERVEUR DEMARRE **************")
                Console.WriteLine("******************************")
                Console.WriteLine(" ")

                'Connexion au serveur
                Dim myChannelFactory As ServiceModel.ChannelFactory(Of HoMIDom.HoMIDom.IHoMIDom) = Nothing

                Try
                    'myChannelFactory = New ServiceModel.ChannelFactory(Of HoMIDom.HoMIDom.IHoMIDom)("ConfigurationHttpHomidom")
                    Dim myadress As String = "http://localhost:" & PortSOAP & "/ServiceModelSamples/service"
                    myChannelFactory = New ServiceModel.ChannelFactory(Of HoMIDom.HoMIDom.IHoMIDom)(New System.ServiceModel.BasicHttpBinding, New System.ServiceModel.EndpointAddress(myadress))
                    myService = myChannelFactory.CreateChannel()
                    'Démarrage du serveur pour charger la config
                    myService.Start()
                Catch ex As Exception
                    myChannelFactory.Abort()
                    Console.WriteLine(Now & "ERREUR: Erreur lors du lancement du service SOAP: " & ex.Message)
                End Try

                Using hostFileServer As New ServiceHost(GetType(HoMIDom.FileServer), fileServerAddress)
                    hostFileServer.Open()
                    Console.WriteLine("Démarrage du serveur de fichiers OK")

                    Console.ReadLine()
                    hostFileServer.Close()
                End Using
                host.Close()
            End Using
        Catch ex As Exception
            MsgBox("Erreur lors du service: " & ex.Message, MsgBoxStyle.Critical, "ERREUR SERVICE")
            Console.WriteLine(Now & " ERREUR " & ex.Message & " : " & ex.ToString)
            Console.ReadLine()
        End Try
    End Sub

    Function LoadPort() As String
        Dim _portip As String = ""

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
                            Exit For
                    End Select
                Next
            Else
                _portip = ""
            End If

        End If
        Return _portip
    End Function

    Sub close()
        End
    End Sub

End Module
