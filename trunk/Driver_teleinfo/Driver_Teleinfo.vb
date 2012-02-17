Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.IO.Ports
Imports VB = Microsoft.VisualBasic


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
        Dim _Com As String = "COM1"
        Dim _Refresh As Integer = 0
        Dim _Modele As String = ""
        Dim _Version As String = "1.0"
        Dim _Picture As String = "compteur-monophase.png"
        Dim _Server As HoMIDom.HoMIDom.Server
        Dim _Device As HoMIDom.HoMIDom.Device
        Dim _DeviceSupport As New ArrayList
        Dim _Parametres As New ArrayList
        Dim _LabelsDriver As New ArrayList
        Dim _LabelsDevice As New ArrayList
        Dim MyTimer As New Timers.Timer
        Dim _IdSrv As String
        Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande)

		'Ajoutés dans les ppt avancés dans New()

		#End Region

#Region "Variables Internes"
        ' Variables de gestion du port COM
        Private WithEvents port As New System.IO.Ports.SerialPort
        Private port_name As String = ""
        Private BufferIn(8192) As Byte
        Private DebutTrame As Boolean = False
        Private DebutInfo As Boolean = False

        Private bytecnt As Integer = 0
        Private messcnt As Integer = 0
        <NonSerialized()> Dim TimerSecond As New Timers.Timer 'Timer à la seconde
        Private recbuf(300), recbytes, recbits As Byte
        Private InfoTrame() As String

        Private mess As Boolean = False
        Private trame As Boolean = False

        Dim ADCO, OPTARIF, ISOUSC, HCHC, HCHP,
         BASE, PTEC, PEJP, IMAX, PAPP, HHPHC, IINST, MOTDETAT As String


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
                If value >= 1 Then
                    _Refresh = value

                End If
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

            'récupération des paramétres avancés
            'Try
            'TeleInfoRefresh = _Parametres.Item(0).Valeur

            'Catch ex As Exception
            '_Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "TeleInfo Start", "Erreur dans les paramétres avancés. utilisation des valeur par défaut" & ex.Message)
            'End Try

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
                If _IsConnect = False Then Exit Sub

                If Objet IsNot Nothing Then
                    Select Case Objet.Type
                        Case "ENERGIETOTALE", "ENERGIEINSTANTANEE"
                            Dim retour As Double = Val(Sauve_temp_teleinfo(LTrim(Objet.Adresse1)))
                            If retour <> 9999 Then Objet.Value = retour

                        Case "GENERIQUESTRING"
                            Dim retour As String = LTrim(Objet.Adresse1)
                            Objet.Value = retour

                        Case "GENERIQUEVALUE"
                            Dim retour As Double = Val(Sauve_temp_teleinfo(LTrim(Objet.Adresse1)))
                            Objet.Value = retour

                        Case Else
                            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Teleinfo Read", "Erreur du type du composant de " & Objet.Adresse1)

                    End Select

                End If


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
        Private Sub Add_LibelleDriver(ByVal Nom As String, ByVal Labelchamp As String, ByVal Tooltip As String, Optional ByVal Parametre As String = "")
            Try
                Dim y0 As New HoMIDom.HoMIDom.Driver.cLabels
                y0.LabelChamp = Labelchamp
                y0.NomChamp = UCase(Nom)
                y0.Tooltip = Tooltip
                y0.Parametre = Parametre
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
        Private Sub Add_LibelleDevice(ByVal Nom As String, ByVal Labelchamp As String, ByVal Tooltip As String, Optional ByVal Parametre As String = "")
            Try
                Dim ld0 As New HoMIDom.HoMIDom.Driver.cLabels
                ld0.LabelChamp = Labelchamp
                ld0.NomChamp = UCase(Nom)
                ld0.Tooltip = Tooltip
                ld0.Parametre = Parametre
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
                ''liste des devices compatibles
                _DeviceSupport.Add(ListeDevices.ENERGIEINSTANTANEE.ToString)
                _DeviceSupport.Add(ListeDevices.ENERGIETOTALE.ToString)


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
                    port.PortName = numero 'nom du port : COM1,COM2, COM3...
                    port.BaudRate = 1200 'vitesse du port 300, 600, 1200, 2400, 9600, 14400, 19200, 38400, 57600, 115200
                    port.Parity = IO.Ports.Parity.Even ' parité paire
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

        ''' <summary>Fermer le port TeleInfo</summary>
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
            Try
                Dim count As Integer = 0
                count = port.BytesToRead
                If _IsConnect And count > 0 Then
                    port.Read(BufferIn, 0, count)
                    For i As Integer = 0 To count - 1
                        ProcessReceivedChar(BufferIn(i))
                    Next
                End If

            Catch Ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "TeleInfo Datareceived", "Exception : " & Ex.Message)
            End Try
        End Sub

        ''' <summary>Rassemble un message complet pour ensuite l'envoyer à displaymess</summary>
        ''' <param name="temp">Byte recu</param>
        ''' <remarks></remarks>
        Private Sub ProcessReceivedChar(ByVal temp As Byte)
            Try
                
                If (temp = 2) Then ' Debut de trame recu 
                    DebutTrame = True
                    DebutInfo = False
                    bytecnt = 0
                    messcnt = 0
                    mess = False
                    trame = False

                ElseIf (DebutTrame And temp = 3) Then 'Fin de trame recue
                    trame = True

                ElseIf (DebutTrame And temp = 10) Then ' debut d'info recu
                    mess = False
                    bytecnt = 0

                ElseIf (DebutTrame And temp = 13) Then ' Fin d'info rec
                    mess = True
                Else 'Recuperation de l'info
                    recbuf(bytecnt) = temp
                    bytecnt += 1
                End If

            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Teleinfo  ProcessReceivedChar", ex.Message)
            End Try

            Try
                If trame Then
                    ' Ajout de valeurs pour debug
                    ' ReDim Preserve InfoTrame(messcnt)
                    ' InfoTrame(messcnt) = "HCHC 000059672845 S"
                    ' messcnt += 1
                    ' ReDim Preserve InfoTrame(messcnt)
                    ' InfoTrame(messcnt) = "HCHP 067159650 x"
                    ' messcnt += 1
                    Process(InfoTrame)

                ElseIf mess Then ' Un message est recu ==> on le stocke
                    Dim xxx As String = ""
                    For i As Integer = 0 To bytecnt - 1
                        xxx = xxx & (ChrW(recbuf(i)))
                    Next

                    ReDim Preserve InfoTrame(messcnt)
                    InfoTrame(messcnt) = xxx.ToString
                    messcnt += 1
                End If

            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Teleinfo ProcessReceivedChar - Traite Message", ex.Message)
            End Try
        End Sub

        ''' <summary>Recomponse les messages reçu</summary>
        ''' <remarks></remarks>
        Private Sub Process(ByRef combuffer() As String)

            Dim TeleInfo_adresse As String = ""
            Dim data1 As String = ""
            Dim InfoRec As String = ""

            Dim charSeparators() As Char = {" "c}
            Dim result() As String


            For Each InfoRec In combuffer
                Try
                    ' _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "TeleInfo Process", "ligne recue : " & InfoRec)
                    result = InfoRec.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries)

                    If result.Count > 1 Then
                        TeleInfo_adresse = result(0)
                        data1 = result(1)
                        ' _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "TeleInfo Process : Get data", "Result : " & TeleInfo_adresse & ": " & data1)
                    Else
                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "TeleInfo Process", " Get data Error trame incorrecte")
                    End If


                Catch ex As Exception
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "TeleInfo Process : Get data", TeleInfo_adresse & " avec la valeur : " & data1 & "Exception :  " & ex.Message)
                End Try


                Try

                    ' Console.WriteLine("Test:#" & LTrim(UCase(TeleInfo_adresse)) & "#")
                    Select Case (LTrim(UCase(TeleInfo_adresse)))

                        Case "ADCO"
                            ADCO = data1

                        Case "OPTARIF"
                            OPTARIF = data1

                        Case "ISOUSC"
                            ISOUSC = data1

                        Case "HCHC"
                            HCHC = data1

                        Case "HCHP"
                            HCHP = data1

                        Case "BASE"
                            BASE = data1

                        Case "PTEC"
                            PTEC = data1

                        Case "PEJP"
                            PEJP = data1

                        Case "IMAX"
                            IMAX = data1

                        Case "PAPP"
                            PAPP = data1

                        Case "HHPHC"
                            HHPHC = data1

                        Case "IINST"
                            IINST = data1

                        Case "MOTDETAT"
                            MOTDETAT = data1

                        Case Else
                            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "TeleInfo Process : Case Teleinfo_adresse ", "Parametre non reconnu adresse : " & TeleInfo_adresse)

                    End Select


                Catch ex As Exception
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "TeleInfo Process : Traitement Exception ", "Exception : " & ex.Message)
                End Try
            Next

        End Sub

        Private Function Sauve_temp_teleinfo(ByVal adresse As String) As String

            Dim retour As String = ""
            Try

                Select Case LTrim(UCase(adresse))

                    Case "ADCO"
                        retour = ADCO

                    Case "OPTARIF"
                        retour = OPTARIF

                    Case "ISOUSC"
                        retour = ISOUSC

                    Case "HCHC"
                        retour = Val(HCHC)

                    Case "HCHP"
                        retour = HCHP

                    Case "BASE"
                        retour = BASE

                    Case "PEJP"
                        retour = PEJP

                    Case "PTEC"
                        retour = PTEC

                    Case "IINST"
                        retour = IINST

                    Case "IMAX"
                        retour = IMAX

                    Case "PAPP"
                        retour = PAPP

                    Case "HHPHC"
                        retour = HHPHC

                    Case "IINST"
                        retour = IINST

                    Case "MOTDETAT"
                        retour = MOTDETAT

                    Case Else
                        retour = "-1"
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "TeleInfo Sauve_temp_teleinfo : Case Teleinfo_adresse ", "Parametre non reconnu adresse : " & adresse)
                End Select


            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "TeleInfo Process : Traitement Exception ", "Exception : " & ex.Message)
            End Try
            Return retour
        End Function




#End Region

    End Class
End Class
