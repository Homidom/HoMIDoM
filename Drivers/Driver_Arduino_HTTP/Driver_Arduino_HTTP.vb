Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.Net
Imports System.IO

'************************************************
'INFOS 
'************************************************
'Le driver communique en HTTP avec l'arduino qui doit implémenter un sketch spécifique
'************************************************

Public Class Driver_Arduino_HTTP
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "673A4D3C-A549-11E4-BE93-B82B1E5D46B0"
    Dim _Nom As String = "Arduino HTTP"
    Dim _Enable As Boolean = False
    Dim _Description As String = "Arduino HTTP"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "HTTP"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "@"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "@"
    Dim _Version As String = My.Application.Info.Version.ToString
    Dim _OsPlatform As String = "3264"
    Dim _Picture As String = ""
    Dim _Server As HoMIDom.HoMIDom.Server
    Dim _Device As HoMIDom.HoMIDom.Device
    Dim _DeviceSupport As New ArrayList
    Dim _Parametres As New ArrayList
    Dim _LabelsDriver As New ArrayList
    Dim _LabelsDevice As New ArrayList
    Dim MyTimer As New Timers.Timer
    Dim _idsrv As String
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande)
    Dim _AutoDiscover As Boolean = False

    'param avancé
    Dim _DEBUG As Boolean = False

#End Region

#Region "Variables Internes"

#End Region

#Region "Propriétés génériques"
    Public WriteOnly Property IdSrv As String Implements HoMIDom.HoMIDom.IDriver.IdSrv
        Set(ByVal value As String)
            _idsrv = value
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
    Public ReadOnly Property DeviceSupport() As ArrayList Implements HoMIDom.HoMIDom.IDriver.DeviceSupport
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
    Public Property Port_TCP() As String Implements HoMIDom.HoMIDom.IDriver.Port_TCP
        Get
            Return _Port_TCP
        End Get
        Set(ByVal value As String)
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
    Public ReadOnly Property Version() As String Implements HoMIDom.HoMIDom.IDriver.Version
        Get
            Return _Version
        End Get
    End Property
    Public ReadOnly Property OsPlatform() As String Implements HoMIDom.HoMIDom.IDriver.OsPlatform
        Get
            Return _OsPlatform
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
    Public Property AutoDiscover() As Boolean Implements HoMIDom.HoMIDom.IDriver.AutoDiscover
        Get
            Return _AutoDiscover
        End Get
        Set(ByVal value As Boolean)
            _AutoDiscover = value
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
    ''' <param name="Command"></param>
    ''' <param name="Param"></param>
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
                    Write(MyDevice, Command, Param(0), Param(1))
                    Return True
                End If
            Else
                Return False
            End If
        Catch ex As Exception
            WriteLog("ERR: ExecuteCommand exception : " & ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>Permet de vérifier si un champ est valide</summary>
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
        'récupération des paramétres avancés
        Try
            _DEBUG = _Parametres.Item(0).Valeur
        Catch ex As Exception
            WriteLog("ERR: Erreur dans les paramétres avancés. utilisation des valeur par défaut : " & ex.Message)
        End Try

        'ouverture du port
        Try
            _IsConnect = True
            WriteLog("Driver Démarré")
        Catch ex As Exception
            _IsConnect = False
            WriteLog("ERR: Start Exception " & ex.Message)
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            WriteLog("Stop")
            _IsConnect = False
        Catch ex As Exception
            WriteLog("ERR: Stop Exception " & ex.Message)
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

            If Objet.Modele = "ENTREE" Then
                Dim urlcommande As String = ""
                urlcommande = "http://" & Objet.Adresse1 & "/?homidom_ENR_" & Objet.Adresse2


            Else
                WriteLog("La fonction Read n'est implementée que le type de PIN ENTREE (ANALOGIQUE)")
            End If

        Catch ex As Exception
            WriteLog("ERR: READ " & ex.ToString)
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
            If _IsConnect = False Then
                WriteLog("Le driver n'est pas démarré, impossible de communiquer avec l'arduino")
                Exit Sub
            End If
            If _DEBUG Then WriteLog("DBG: WRITE Device " & Objet.Name & " <-- " & Command)

            'verification si adresse1 n'est pas vide (doit etre egale à l'IP de l'arduino)
            If String.IsNullOrEmpty(Objet.Adresse1) Or Objet.Adresse1 = "" Then
                WriteLog("ERR: WRITE l'adresse IP de l'arduino doit etre renseigné (ex: 192.168.1.13) : " & Objet.Name)
                Exit Sub
            End If


            'suivant le type du PIN on lance la bonne commande : ENTREE|SORTIE|PWM|1WIRE

            'liste des commmandes coté arduino
            'http://ip/?homidom_ON_X
            'http://ip/?homidom_OFF_X
            'http://ip/?homidom_DIM_X_level (level = 0 to 255)
            'http://ip/?homidom_ENR_X
            'http://ip/?homidom_CFG_X_TYPE (Type = 0 pour Input, 1 pour Output, 2 pour pwm et 3 pour One wire)


            Dim urlcommande As String = ""
            If Command = "CONFIG_TYPE_PIN" Then
                Select Case UCase(Objet.Modele)
                    Case "ENTREE" : urlcommande = "http://" & Objet.Adresse1 & "/?homidom_CFG_" & Objet.Adresse2 & "_0"
                    Case "SORTIE" : urlcommande = "http://" & Objet.Adresse1 & "/?homidom_CFG_" & Objet.Adresse2 & "_1"
                    Case "PWM" : urlcommande = "http://" & Objet.Adresse1 & "/?homidom_CFG_" & Objet.Adresse2 & "_2"
                    Case "1WIRE" : urlcommande = "http://" & Objet.Adresse1 & "/?homidom_CFG_" & Objet.Adresse2 & "_3"
                    Case Else : WriteLog("ERR: WRITE CONFIG_TYPE_PIN : Type de PIN non géré : " & Objet.Modele.ToString.ToUpper & " (" & Objet.Name & ")")
                End Select
            Else
                Select Case UCase(Objet.Modele)
                    Case "ENTREE" : urlcommande = "http://" & Objet.Adresse1 & "/?homidom_ENR_" & Objet.Adresse2
                    Case "SORTIE"
                        Select Case Command
                            Case "OFF" : urlcommande = "http://" & Objet.Adresse1 & "/?homidom_ON_" & Objet.Adresse2
                            Case "ON" : urlcommande = "http://" & Objet.Adresse1 & "/?homidom_OFF_" & Objet.Adresse2
                            Case "DIM"
                                If Not IsNothing(Parametre1) Then
                                    If IsNumeric(Parametre1) Then
                                        urlcommande = "http://" & Objet.Adresse1 & "/?homidom_DIM_" & Objet.Adresse2 & "_" & CInt(Parametre1)
                                    Else
                                        WriteLog("ERR: WRITE DIM Le parametre " & CStr(Parametre1) & " n'est pas un entier (" & Objet.Name & ")")
                                    End If
                                Else
                                    WriteLog("ERR: WRITE DIM Il manque un parametre (" & Objet.Name & ")")
                                End If
                            Case Else
                                WriteLog("ERR: Send AC : Commande invalide : " & Command)
                                Exit Sub
                        End Select
                    Case "PWM"
                        If Not IsNothing(Parametre1) Then
                            If IsNumeric(Parametre1) Then
                                urlcommande = "http://" & Objet.Adresse1 & "/?homidom_DIM_" & Objet.Adresse2 & "_" & CInt(Parametre1)
                            Else
                                WriteLog("ERR: WRITE DIM Le parametre " & CStr(Parametre1) & " n'est pas un entier (" & Objet.Name & ")")
                            End If
                        Else
                            WriteLog("ERR: WRITE DIM Il manque un parametre (" & Objet.Name & ")")
                        End If
                    Case "1WIRE"
                        WriteLog("le 1-wire n'est pas encore géré :" & Objet.Name)
                    Case "" : WriteLog("ERR: WRITE Pas de protocole d'emission pour " & Objet.Name)
                    Case Else : WriteLog("ERR: WRITE Protocole non géré : " & Objet.Modele.ToString.ToUpper)
                End Select
            End If

            If urlcommande <> "" Then
                If _DEBUG Then WriteLog("DBG: WRITE Composant " & Objet.Name & " URL : " & urlcommande)

                Dim request As HttpWebRequest = WebRequest.Create(urlcommande)
                request.Timeout = 3000
                CType(request, HttpWebRequest).UserAgent = "Other"


                'Dim response As WebResponse = request.GetResponse()
                'If CType(response, HttpWebResponse).StatusCode = HttpStatusCode.OK Then
                '    Dim dataStream As Stream = response.GetResponseStream()
                '    Dim reader As New StreamReader(dataStream)
                '    Dim responseFromServer As String = reader.ReadToEnd()
                '    WriteLog("DBG: Commande passée à l arduino : " & urlcommande & " --> " & responseFromServer & " (" & CType(response, HttpWebResponse).StatusDescription & ")")
                'Else
                '    WriteLog("DBG: Commande passée à l arduino : " & urlcommande & " --> Réponse incorrecte reçu : " & CType(response, HttpWebResponse).StatusCode & " (" & CType(response, HttpWebResponse).StatusDescription & ")")
                'End If


                'Get a web response  
                Dim response As WebResponse
                Try
                    response = request.GetResponse()
                    If CType(response, HttpWebResponse).StatusCode = HttpStatusCode.OK Then
                        Dim dataStream As Stream = response.GetResponseStream()
                        Dim reader As New StreamReader(dataStream)
                        Dim responseFromServer As String = reader.ReadToEnd()
                        WriteLog("DBG: Commande passée à l arduino : " & urlcommande & " --> " & responseFromServer & " (" & CType(response, HttpWebResponse).StatusDescription & ")")
                    Else
                        WriteLog("DBG: Commande passée à l arduino : " & urlcommande & " --> Réponse incorrecte reçu : " & CType(response, HttpWebResponse).StatusCode & " (" & CType(response, HttpWebResponse).StatusDescription & ")")
                    End If
                    response.Close()
                Catch ex As System.Net.WebException
                    WriteLog("ERR: Commande passée à l arduino : " & urlcommande & " --> Erreur de communication : " & ex.Message.ToString)

                    If ex.Status = WebExceptionStatus.ProtocolError Then

                    End If
                End Try

            End If

        Catch ex As Exception
            WriteLog("ERR: WRITE " & ex.ToString)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice

    End Sub

    ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice

    End Sub

    ''' <summary>ajout des commandes avancées pour les devices</summary>
    ''' <remarks></remarks>
    Private Sub add_devicecommande(ByVal nom As String, ByVal description As String, ByVal nbparam As Integer)
        Try
            Dim x As New DeviceCommande
            x.NameCommand = nom
            x.DescriptionCommand = description
            x.CountParam = nbparam
            _DeviceCommandPlus.Add(x)
        Catch ex As Exception
            WriteLog("ERR: add_devicecommande Exception : " & ex.Message)
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
            WriteLog("ERR: Add_LibelleDriver Exception : " & ex.Message)
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
            WriteLog("ERR: Add_LibelleDevice Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>ajout de parametre avancés</summary>
    ''' <param name="nom">Nom du parametre (sans espace)</param>
    ''' <param name="description">Description du parametre</param>
    ''' <param name="valeur">Sa valeur</param>
    ''' <remarks></remarks>
    Private Sub add_paramavance(ByVal nom As String, ByVal description As String, ByVal valeur As Object)
        Try
            Dim x As New HoMIDom.HoMIDom.Driver.Parametre
            x.Nom = nom
            x.Description = description
            x.Valeur = valeur
            _Parametres.Add(x)
        Catch ex As Exception
            WriteLog("ERR: add_devicecommande Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Creation d'un objet de type</summary>
    ''' <remarks></remarks>
    Public Sub New()
        Try
            _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString

            'Parametres avancés
            add_paramavance("Debug", "Activer le Debug complet (True/False)", False)
            'add_paramavance("AutoDiscover", "Permet de créer automatiquement des composants si ceux-ci n'existent pas encore (True/False)", False)
            'add_paramavance("Protocole Undec", "Protocole UNDEC 0=disable 1=enable", 1)

            'liste des devices compatibles
            _DeviceSupport.Add(ListeDevices.APPAREIL.ToString)
            _DeviceSupport.Add(ListeDevices.BAROMETRE.ToString)
            _DeviceSupport.Add(ListeDevices.BATTERIE.ToString)
            _DeviceSupport.Add(ListeDevices.COMPTEUR.ToString)
            _DeviceSupport.Add(ListeDevices.CONTACT.ToString)
            _DeviceSupport.Add(ListeDevices.DETECTEUR.ToString)
            _DeviceSupport.Add(ListeDevices.DIRECTIONVENT.ToString)
            _DeviceSupport.Add(ListeDevices.ENERGIEINSTANTANEE.ToString)
            _DeviceSupport.Add(ListeDevices.ENERGIETOTALE.ToString)
            _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN.ToString)
            _DeviceSupport.Add(ListeDevices.GENERIQUESTRING.ToString)
            _DeviceSupport.Add(ListeDevices.GENERIQUEVALUE.ToString)
            _DeviceSupport.Add(ListeDevices.HUMIDITE.ToString)
            _DeviceSupport.Add(ListeDevices.LAMPE.ToString)
            _DeviceSupport.Add(ListeDevices.PLUIECOURANT.ToString)
            _DeviceSupport.Add(ListeDevices.PLUIETOTAL.ToString)
            _DeviceSupport.Add(ListeDevices.SWITCH.ToString)
            _DeviceSupport.Add(ListeDevices.TELECOMMANDE.ToString)
            _DeviceSupport.Add(ListeDevices.TEMPERATURE.ToString)
            _DeviceSupport.Add(ListeDevices.TEMPERATURECONSIGNE.ToString)
            _DeviceSupport.Add(ListeDevices.UV.ToString)
            _DeviceSupport.Add(ListeDevices.VITESSEVENT.ToString)
            _DeviceSupport.Add(ListeDevices.VOLET.ToString)

            'ajout des commandes avancées pour les devices
            'add_devicecommande("COMMANDE", "DESCRIPTION", nbparametre)
            add_devicecommande("CONFIG_TYPE_PIN", "configurer le type de PIN sur l arduino suivant les propriétés du composant", 0)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Adresse IP Arduino", "Adresse IP de l arduino gérant ce composant (ex:192.168.1.13)")
            Add_LibelleDevice("ADRESSE2", "Numéro du PIN", "Numéro du PIN sur l arduino (ex: 1)")
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("MODELE", "TYPE PIN", "Type du PIN : ENTREE(Analogique)/SORTIE(ON/OFF)/PWM(0-255)/1WIRE", "ENTREE|SORTIE|PWM|1WIRE")
            Add_LibelleDevice("REFRESH", "@", "")
            'Add_LibelleDevice("LASTCHANGEDUREE", "LastChange Durée", "")

        Catch ex As Exception
            WriteLog("ERR: New Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)
        If _Enable = False Then Exit Sub
        WriteLog("La fonction Refresh n'est pas implementée pour ce driver")
    End Sub

#End Region

#Region "Fonctions Internes"

    ''' <summary>Va écrire les valeurs dans Homidom</summary>
    ''' <remarks></remarks>
    Private Sub traitement(ByVal valeur As Integer, ByVal adresse As String, ByVal type As Integer)
        Try
            'Recherche si un device affecté
            Dim listedevices As New ArrayList
            Dim _Type As String = ""

            Select Case type
                Case 0 'CONTACT
                    _Type = "CONTACT" 'ENTREE
                Case 1 'APPAREIL
                    _Type = "APPAREIL" 'SORTIE
                Case 2 'GENERIQUE VALUE
                    _Type = "GENERIQUEVALUE" 'ENTREE ANA
                Case Else
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Process", "Le type de device n'appartient pas à ce driver: " & type)
                    Exit Sub
            End Select

            listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_idsrv, adresse, type, Me._ID, True)

            'un device trouvé on maj la value
            If (listedevices.Count = 1) Then
                'correction valeur pour correspondre au type de value
                'If TypeOf listedevices.Item(0).Value Is Integer Then
                '    If valeur = 1 Then
                '        valeur = 100
                '    ElseIf valeur = 0 Then
                '        valeur = 0
                '    End If
                'Else
                listedevices.Item(0).Value = valeur
                'End If

            ElseIf (listedevices.Count > 1) Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " traitement", "Plusieurs devices correspondent à : " & adresse & ":" & valeur)
            Else
                'si autodiscover = true ou modedecouverte du serveur actif alors on crée le composant sinon on logue
                If _AutoDiscover Or _Server.GetModeDecouverte Then
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " traitement", "Device non trouvé, AutoCreation du composant : " & type & " " & adresse & ":" & valeur)
                    _Server.AddDetectNewDevice(adresse, _ID, type, "")
                Else
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " traitement", "Device non trouvé : " & type & " " & adresse & ":" & valeur)
                End If
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " traitement", "Exception : " & ex.Message & " --> " & adresse & " : " & valeur)
        End Try
    End Sub

    Private Sub WriteLog(ByVal message As String)
        Try
            'utilise la fonction de base pour loguer un event
            If STRGS.InStr(message, "DBG:") > 0 Then
                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "ArduinoHTTP", STRGS.Right(message, message.Length - 5))
            ElseIf STRGS.InStr(message, "ERR:") > 0 Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ArduinoHTTP", STRGS.Right(message, message.Length - 5))
            Else
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "ArduinoHTTP", message)
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ArduinoHTTP WriteLog", ex.Message)
        End Try
    End Sub

#End Region



End Class
