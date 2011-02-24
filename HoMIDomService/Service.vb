Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels
Imports System.Runtime.Remoting.Channels.Http
Imports System.Runtime.Remoting.Lifetime
Imports HoMIDom.HoMIDom
Imports System.IO

'***********************************************
'** SERVICE HOMIDom - Simple exe qui sera ensuite convertit en service Windows
'** Ce service créer le serveur SOAP Web via l'interface IHoMIDom
'** Par défaut le service est lancé sur le port 8888
'** version 1.0
'** Date de création: 14/01/2011
'** Historique (SebBergues): 14/01/2011: Création 
'** Historique (SebBergues) : 21/01/2011: Ajout de la gestion de la config
'***********************************************

Module Service

    Dim monserveur As Server = New Server()

    Sub Main()
        Try

            Console.WriteLine("******************************")
            Console.WriteLine("DEMARRAGE DU SERVEUR**********")
            Console.WriteLine("******************************")
            Console.WriteLine(" ")

            monserveur.start()

            'Démarrage du serviceWeb
            Console.WriteLine(Now & " ")
            Console.WriteLine(Now & " Start ServiceWeb")
            Dim chnl As New HttpChannel(8888) 'obj.PortTCP)
            ChannelServices.RegisterChannel(chnl, False)
            LifetimeServices.LeaseTime = Nothing
            RemotingServices.Marshal(monserveur, "RemoteObjectServer.soap")
            Console.WriteLine(Now & " ServiceWeb Démarré sur port:8888") ' & obj.PortTCP)

            Console.WriteLine("******************************")
            Console.WriteLine("SERVEUR DEMARRE **************")
            Console.WriteLine("******************************")
            Console.WriteLine(" ")

            'obj.SaveZone("", "Maison")
            'obj.SaveConfiguration()

            Console.ReadLine()
            
        Catch ex As Exception
            Console.WriteLine(Now & " ERREUR " & ex.Message)
        End Try
    End Sub

    Sub close()
        End
    End Sub

End Module
