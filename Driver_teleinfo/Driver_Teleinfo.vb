Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.IO.Ports


Public Class Driver_Teleinfo

    ' Auteur : Laurent
    ' Date : 11/01/2012

    ''' <summary>Class Driver_Teleinfo, permet de communiquer avec le module USB de A DAUGUET</summary>
    ''' <remarks>Nécessite l'installation des pilotes fournis sur le site </remarks>
    <Serializable()> Public Class Driver_Teleinfo
        Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
        '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
        'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
        Dim _ID As String = "3BB1F870-3A41-11E1-B86C-0800200C9A66"
        Dim _Nom As String = "Teleinfo"
        Dim _Enable As String = False
        Dim _Description As String = "Adaptateur USB DAUGUET"
        Dim _StartAuto As Boolean = False
        Dim _Protocol As String = "COM"
        Dim _IsConnect As Boolean = False
        Dim _IP_TCP As String = "@"
        Dim _Port_TCP As String = "@"
        Dim _IP_UDP As String = "@"
        Dim _Port_UDP As String = "@"
        Dim _Com As String = "@"
        Dim _Refresh As Integer = 0
        Dim _Modele As String = ""
        Dim _Version As String = "1.0"
        Dim _Picture As String = ""
        Dim _Server As HoMIDom.HoMIDom.Server
        Dim _Device As HoMIDom.HoMIDom.Device
        Dim _DeviceSupport As New ArrayList
        Dim _Parametres As New ArrayList
        Dim _LabelsDriver As New ArrayList
        Dim _LabelsDevice As New ArrayList
        Dim MyTimer As New Timers.Timer
        Dim _IdSrv As String
        Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande)
#End Region

#Region "Variables Internes"
        ' Variables de gestion du port COM
        Private WithEvents port As New System.IO.Ports.SerialPort
        Private port_name As String = ""
        Private BufferIn(8192) As Byte
        Private firstbyte As Boolean = True
        Private bytecnt As Integer = 0
        Private recbuf(30) As Byte

#End Region

#Region "Propriétés génériques"
        Public WriteOnly Property IdSrv As String Implements HoMIDom.HoMIDom.IDriver.IdSrv
            Set(ByVal value As String)
                _IdSrv = value
            End Set
        End Property

        Public Property COM() As String Implements HoMIDom.HoMIDom.IDriver.COM
            Get
                Return _Com
            End Get
            Set(ByVal value As String)
                _Com = value
            End Set
        End Property
        Public ReadOnly Property Description() As String Implements HoMIDom.HoMIDom.IDriver.Description
            Get
                Return _Description
            End Get
        End Property
        Public ReadOnly Property DeviceSupport() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.DeviceSupport
            Get
                Return _DeviceSupport
            End Get
        End Property

        Public Property Parametres() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.Parametres
            Get
                Return _Parametres
            End Get
            Set(ByVal value As System.Collections.ArrayList)
                _Parametres = value
            End Set
        End Property

        Public Property LabelsDriver() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.LabelsDriver
            Get
                Return _LabelsDriver
            End Get
            Set(ByVal value As System.Collections.ArrayList)
                _LabelsDriver = value
            End Set
        End Property
        Public Property LabelsDevice() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.LabelsDevice
            Get
                Return _LabelsDevice
            End Get
            Set(ByVal value As System.Collections.ArrayList)
                _LabelsDevice = value
            End Set
        End Property
        Public Event DriverEvent(ByVal DriveName As String, ByVal TypeEvent As String, ByVal Parametre As Object) Implements HoMIDom.HoMIDom.IDriver.DriverEvent
        Public Property Enable() As Boolean Implements HoMIDom.HoMIDom.IDriver.Enable
            Get
                Return _Enable
            End Get
            Set(ByVal value As Boolean)
                _Enable = value
            End Set
        End Property
        Public ReadOnly Property ID() As String Implements HoMIDom.HoMIDom.IDriver.ID
            Get
                Return _ID
            End Get
        End Property
        Public Property IP_TCP() As String Implements HoMIDom.HoMIDom.IDriver.IP_TCP
            Get
                Return _IP_TCP
            End Get
            Set(ByVal value As String)
                _IP_TCP = value
            End Set
        End Property
        Public Property IP_UDP() As String Implements HoMIDom.HoMIDom.IDriver.IP_UDP
            Get
                Return _IP_UDP
            End Get
            Set(ByVal value As String)
                _IP_UDP = value
            End Set
        End Property
        Public ReadOnly Property IsConnect() As Boolean Implements HoMIDom.HoMIDom.IDriver.IsConnect
            Get
                Return _IsConnect
            End Get
        End Property
        Public Property Modele() As String Implements HoMIDom.HoMIDom.IDriver.Modele
            Get
                Return _Modele
            End Get
            Set(ByVal value As String)
                _Modele = value
            End Set
        End Property
        Public ReadOnly Property Nom() As String Implements HoMIDom.HoMIDom.IDriver.Nom
            Get
                Return _Nom
            End Get
        End Property
        Public Property Picture() As String Implements HoMIDom.HoMIDom.IDriver.Picture
            Get
                Return _Picture
            End Get
            Set(ByVal value As String)
                _Picture = value
            End Set
        End Property
        Public Property Port_TCP() As Object Implements HoMIDom.HoMIDom.IDriver.Port_TCP
            Get
                Return _Port_TCP
            End Get
            Set(ByVal value As Object)
                _Port_TCP = value
            End Set
        End Property
        Public Property Port_UDP() As String Implements HoMIDom.HoMIDom.IDriver.Port_UDP
            Get
                Return _Port_UDP
            End Get
            Set(ByVal value As String)
                _Port_UDP = value
            End Set
        End Property
        Public ReadOnly Property Protocol() As String Implements HoMIDom.HoMIDom.IDriver.Protocol
            Get
                Return _Protocol
            End Get
        End Property
        Public Property Refresh() As Integer Implements HoMIDom.HoMIDom.IDriver.Refresh
            Get
                Return _Refresh
            End Get
            Set(ByVal value As Integer)
                _Refresh = value
            End Set
        End Property
        Public Property Server() As HoMIDom.HoMIDom.Server Implements HoMIDom.HoMIDom.IDriver.Server
            Get
                Return _Server
            End Get
            Set(ByVal value As HoMIDom.HoMIDom.Server)
                _Server = value
            End Set
        End Property
        Public ReadOnly Property Version() As String Implements HoMIDom.HoMIDom.IDriver.Version
            Get
                Return _Version
            End Get
        End Property
        Public Property StartAuto() As Boolean Implements HoMIDom.HoMIDom.IDriver.StartAuto
            Get
                Return _StartAuto
            End Get
            Set(ByVal value As Boolean)
                _StartAuto = value
            End Set
        End Property
#End Region

#Region "Fonctions génériques"
        ''' <summary>
        ''' Retourne la liste des Commandes avancées
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCommandPlus() As List(Of DeviceCommande)
            Return _DeviceCommandPlus
        End Function

        ''' <summary>Execute une commande avancée</summary>
        ''' <param name="MyDevice">Objet représentant le Device </param>
        ''' <param name="Command">Nom de la commande avancée à éxécuter</param>
        ''' <param name="Param">tableau de paramétres</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExecuteCommand(ByVal MyDevice As Object, ByVal Command As String, Optional ByVal Param() As Object = Nothing) As Boolean
            Dim retour As Boolean = False
            Try
                If MyDevice IsNot Nothing Then
                    'Pas de commande demandée donc erreur
                    If Command = "" Then
                        Return False
                    Else
                        'Write(deviceobject, Command, Param(0), Param(1))
                        Select Case UCase(Command)
                            Case ""
                            Case Else
                        End Select
                        Return True
                    End If
                Else
                    Return False
                End If
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ExecuteCommand", "exception : " & ex.Message)
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Permet de vérifier si un champ est valide
        ''' </summary>
        ''' <param name="Champ"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function VerifChamp(ByVal Champ As String, ByVal Value As Object) As String Implements HoMIDom.HoMIDom.IDriver.VerifChamp
            Try
                Dim retour As String = "0"
                Select Case UCase(Champ)


                End Select
                Return retour
            Catch ex As Exception
                Return "Une erreur est apparue lors de la vérification du champ " & Champ & ": " & ex.ToString
            End Try
        End Function

        ''' <summary>Démarrer le du driver</summary>
        ''' <remarks></remarks>
        Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
            Dim retour As String = ""
            'ouverture du port suivant le Port Com
            Try
                If _Com <> "" Then
                    retour = ouvrir(_Com)
                Else
                    retour = "ERR: Port Com non défini. Impossible d'ouvrir le port !"
                End If
                'traitement du message de retour
                If STRGS.Left(retour, 4) = "ERR:" Then
                    _IsConnect = False
                    retour = STRGS.Right(retour, retour.Length - 5)
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "TeleInfo", "Driver non démarré : " & retour)
                Else
                    _IsConnect = True
                    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "TeleInfo", retour)
                End If
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Teleinfo Start", ex.Message)
                _IsConnect = False
            End Try



        End Sub

        ''' <summary>Arrêter le du driver</summary>
        ''' <remarks></remarks>
        Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
            Try

            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Teleinfo Stop", ex.Message)
            End Try
        End Sub

        ''' <summary>Re-Démarrer le du driver</summary>
        ''' <remarks></remarks>
        Public Sub Restart() Implements HoMIDom.HoMIDom.IDriver.Restart
            [Stop]()
            Start()
        End Sub

        ''' <summary>Intérroger un device</summary>
        ''' <param name="Objet">Objet représetant le device à interroger</param>
        ''' <remarks>pas utilisé</remarks>
        Public Sub Read(ByVal Objet As Object) Implements HoMIDom.HoMIDom.IDriver.Read
            Try
                If _Enable = False Then Exit Sub
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Teleinfo Read", ex.Message)
            End Try
        End Sub

        ''' <summary>Commander un device</summary>
        ''' <param name="Objet">Objet représetant le device à interroger</param>
        ''' <param name="Command">La commande à passer</param>
        ''' <param name="Parametre1"></param>
        ''' <param name="Parametre2"></param>
        ''' <remarks></remarks>
        Public Sub Write(ByVal Objet As Object, ByVal Command As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
            Try
                If _Enable = False Then Exit Sub
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Teleinfo Write", ex.Message)
            End Try
        End Sub

        ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
        ''' <param name="DeviceId">Objet représetant le device à interroger</param>
        ''' <remarks></remarks>
        Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice
            Try

            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Teleinfo DeleteDevice", ex.Message)
            End Try
        End Sub

        ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
        ''' <param name="DeviceId">Objet représetant le device à interroger</param>
        ''' <remarks></remarks>
        Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice
            Try

            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Teleinfo NewDevice", ex.Message)
            End Try
        End Sub

        ''' <summary>ajout des commandes avancées pour les devices</summary>
        ''' <param name="nom">Nom de la commande avancée</param>
        ''' <param name="description">Description qui sera affichée dans l'admin</param>
        ''' <param name="nbparam">Nombre de parametres attendus</param>
        ''' <remarks></remarks>
        Private Sub Add_DeviceCommande(ByVal Nom As String, ByVal Description As String, ByVal NbParam As Integer)
            Try
                Dim x As New DeviceCommande
                x.NameCommand = Nom
                x.DescriptionCommand = description
                x.CountParam = nbparam
                _DeviceCommandPlus.Add(x)
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " add_devicecommande", "Exception : " & ex.Message)
            End Try
        End Sub

        ''' <summary>ajout Libellé pour le Driver</summary>
        ''' <param name="nom">Nom du champ : HELP</param>
        ''' <param name="labelchamp">Nom à afficher : Aide</param>
        ''' <param name="tooltip">Tooltip à afficher au dessus du champs dans l'admin</param>
        ''' <remarks></remarks>
        Private Sub Add_LibelleDriver(ByVal Nom As String, ByVal Labelchamp As String, ByVal Tooltip As String)
            Try
                Dim y0 As New HoMIDom.HoMIDom.Driver.cLabels
                y0.LabelChamp = Labelchamp
                y0.NomChamp = UCase(Nom)
                y0.Tooltip = Tooltip
                _LabelsDriver.Add(y0)
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " add_devicecommande", "Exception : " & ex.Message)
            End Try
        End Sub

        ''' <summary>Ajout Libellé pour les Devices</summary>
        ''' <param name="nom">Nom du champ : HELP</param>
        ''' <param name="labelchamp">Nom à afficher : Aide, si = "@" alors le champ ne sera pas affiché</param>
        ''' <param name="tooltip">Tooltip à afficher au dessus du champs dans l'admin</param>
        ''' <remarks></remarks>
        Private Sub Add_LibelleDevice(ByVal Nom As String, ByVal Labelchamp As String, ByVal Tooltip As String)
            Try
                Dim ld0 As New HoMIDom.HoMIDom.Driver.cLabels
                ld0.LabelChamp = Labelchamp
                ld0.NomChamp = UCase(Nom)
                ld0.Tooltip = Tooltip
                _LabelsDevice.Add(ld0)
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " add_devicecommande", "Exception : " & ex.Message)
            End Try
        End Sub

        ''' <summary>ajout de parametre avancés</summary>
        ''' <param name="nom">Nom du parametre (sans espace)</param>
        ''' <param name="description">Description du parametre</param>
        ''' <param name="valeur">Sa valeur</param>
        ''' <remarks></remarks>
        Private Sub Add_ParamAvance(ByVal nom As String, ByVal description As String, ByVal valeur As Object)
            Try
                Dim x As New HoMIDom.HoMIDom.Driver.Parametre
                x.Nom = nom
                x.Description = description
                x.Valeur = valeur
                _Parametres.Add(x)
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " add_devicecommande", "Exception : " & ex.Message)
            End Try
        End Sub

        ''' <summary>Creation d'un objet de type</summary>
        ''' <remarks></remarks>
        Public Sub New()
            Try
                'liste des devices compatibles
                _DeviceSupport.Add(ListeDevices.ENERGIEINSTANTANEE.ToString)
                _DeviceSupport.Add(ListeDevices.ENERGIETOTALE.ToString)


                'ajout des commandes avancées pour les devices
                'Ci-dessous un exemple
                'Dim x As New DeviceCommande
                'x.NameCommand = "Test"
                'x.DescriptionCommand = "Ceci est une commande avancée de test"
                'x.CountParam = 1
                '_DeviceCommandPlus.Add(x)
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Teleinfo New", ex.Message)
            End Try
        End Sub

        ''' <summary>Si refresh >0 gestion du timer</summary>
        ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
        Private Sub TimerTick()

        End Sub

#End Region

#Region "Fonctions internes"
        ''' <summary>Ouvrir le port Teleinfo</summary>
        ''' <param name="numero">Nom/Numero du port COM: COM2</param>
        ''' <remarks></remarks>
        Private Function ouvrir(ByVal numero As String) As String
            Try
                'ouverture du port
                If Not _IsConnect Then
                    port_name = numero
                    port.PortName = numero 'nom du port : COM1
                    port.BaudRate = 1200 'vitesse du port 300, 600, 1200, 2400, 9600, 14400, 19200, 38400, 57600, 115200
                    port.Parity = IO.Ports.Parity.Even 'pas de parité
                    port.StopBits = IO.Ports.StopBits.One 'un bit d'arrêt par octet
                    port.DataBits = 7 'nombre de bit par octet
                    'port.Handshake = Handshake.None
                    port.ReadTimeout = 3000
                    port.WriteTimeout = 5000
                    'port.RtsEnable = False 'ligne Rts désactivé
                    'port.DtrEnable = False 'ligne Dtr désactivé
                    port.Open()
                    AddHandler port.DataReceived, New SerialDataReceivedEventHandler(AddressOf DataReceived)
                    Return ("Port " & port_name & " ouvert")
                Else
                    Return ("Port " & port_name & " dejà ouvert")
                End If
            Catch ex As Exception
                Return ("ERR: " & ex.Message)
            End Try
        End Function

        ''' <summary>Fermer le port PLCBUS</summary>
        ''' <remarks></remarks>
        Private Function fermer() As String
            Try
                If _IsConnect Then
                    If (Not (port Is Nothing)) Then ' The COM port exists.
                        If port.IsOpen Then
                            port.DiscardOutBuffer()
                            port.Close()
                            port.Dispose()
                            _IsConnect = False
                            Return ("Port " & port_name & " fermé")
                        Else
                            Return ("Port " & port_name & "  est déjà fermé")
                        End If
                    Else
                        Return ("Port " & port_name & " n'existe pas")
                    End If
                Else
                    Return ("Port " & port_name & "  est déjà fermé (port_ouvert=false)")
                End If
            Catch ex As UnauthorizedAccessException
                Return ("ERR: Port " & port_name & " IGNORE")
                ' The port may have been removed. Ignore.
            End Try
            Return True
        End Function

        ''' <summary>Fonction lancée sur reception de données sur le port COM</summary>
        ''' <remarks></remarks>
        Private Sub DataReceived(ByVal sender As Object, ByVal e As SerialDataReceivedEventArgs)
            'fonction qui lit les données sur le port serie
            Dim LigneRecu As String

            Try
                Dim count As Integer = 0
                count = port.BytesToRead
                '_Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "TeleInfo Datareceived", "Nombre de caracteres : " & count)
                If count > 0 Then
                    LigneRecu = port.ReadLine().ToString
                    '_Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "TeleInfo Datareceived", "ligne recu : " & LigneRecu)
                    Process(LigneRecu)
                End If

            Catch Ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "TeleInfo Datareceived", "Exception : " & Ex.Message)
            End Try
        End Sub
#End Region

  
        ''' <summary>Recomponse les messages reçu</summary>
        ''' <remarks></remarks>
        Private Sub Process(ByVal comBuffer As String)

            Dim TeleInfo_adresse As String = ""
            Dim data1 As String = ""

            Dim charSeparators() As Char = {" "c}
            Dim result() As String

            ' Recupere les informations de la chaine recue
            Try
                result = comBuffer.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries)
                TeleInfo_adresse = result(0)
                data1 = result(1)

            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "TeleInfo Process : Get data", "Exception : " & ex.Message)
            End Try

 
            Try

                Select Case TeleInfo_adresse
                    Case "ADCO", "OPTARIF", "ISOUSC", "BASE", "PTEC"
                        Try
                            'Recherche si un device affecté
                            Dim listedevices As New ArrayList
                            listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, TeleInfo_adresse, "", Me._ID, True)
                            'un device trouvé on maj la value
                            If (listedevices.Count = 1) Then
                                If TypeOf listedevices.Item(0).Value Is Integer Then
                                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "TeleInfo Process", " Type Conversion ")

                                    listedevices.Item(0).Value = Convert.ToInt64(data1)
                                    'correction valeur pour correspondre au type de value
                                    '_Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "TeleInfo Process", " Value " & listedevices.Item(0) & " recue avec la valeur : " & data1)
                                Else
                                    listedevices.Item(0).Value = data1
                                End If


                                ' Plusieurs composants correspondent à l'adresse
                            ElseIf (listedevices.Count > 1) Then
                                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "TeleInfo Process", "Plusieurs devices correspondent à : " & TeleInfo_adresse & ":" & data1)

                            End If
                        Catch ex As Exception
                            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "TeleInfo traitement", "Exception : " & ex.Message & " --> " & TeleInfo_adresse & " : " & data1)
                        End Try
                End Select

            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "TeleInfo Process : Traitement commande reçue", "Exception : " & ex.Message)
            End Try
   
        End Sub

    End Class
End Class
