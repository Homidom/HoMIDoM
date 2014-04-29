Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports System.Net
Imports System.IO
Imports System.Xml

<Serializable()> Public Class Driver_http
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "D04010DA-5E22-11E1-A742-4E4A4824019B" 'ne pas modifier car utilisé dans le code du serveur
    Dim _Nom As String = "HTTP" 'Nom du driver à afficher
    Dim _Enable As Boolean = False 'Activer/Désactiver le driver
    Dim _Description As String = "Driver Http" 'Description du driver
    Dim _StartAuto As Boolean = False 'True si le driver doit démarrer automatiquement
    Dim _Protocol As String = "Http" 'Protocole utilisé par le driver, exemple: RS232
    Dim _IsConnect As Boolean = False 'True si le driver est connecté et sans erreur
    Dim _IP_TCP As String = "@" 'Adresse IP TCP à utiliser, "@" si non applicable pour le cacher côté client
    Dim _Port_TCP As String = "@" 'Port TCP à utiliser, "@" si non applicable pour le cacher côté client
    Dim _IP_UDP As String = "@" 'Adresse IP UDP à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Port_UDP As String = "@" 'Port UDP à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Com As String = "@" 'Port COM à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Refresh As Integer = 0 'Valeur à laquelle le driver doit rafraichir les valeurs des devices (ex: toutes les 200ms aller lire les devices)
    Dim _Modele As String = "" 'Modèle du driver/interface
    Dim _Version As String = My.Application.Info.Version.ToString 'Version du driver
    Dim _OsPlatform As String = "3264" 'plateforme compatible 32 64 ou 3264 bits
    Dim _Picture As String = "" 'Image du driver (non utilisé actuellement)
    Dim _Server As HoMIDom.HoMIDom.Server 'Objet Reflètant le serveur
    Dim _DeviceSupport As New ArrayList 'Type de Device supporté par le driver
    Dim _Device As HoMIDom.HoMIDom.Device 'Image reflétant un device
    Dim _Parametres As New ArrayList 'Paramètres supplémentaires associés au driver
    Dim _LabelsDriver As New ArrayList 'Libellés, tooltip associés au driver
    Dim _LabelsDevice As New ArrayList 'Libellés, tooltip des devices associés au driver
    Dim MyTimer As New Timers.Timer 'Timer du driver
    Dim _IdSrv As String 'Id du Serveur (pour autoriser à utiliser des commandes)
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande) 'Liste des commandes avancées du driver
    Dim _AutoDiscover As Boolean = False

#End Region

#Region "Variables Internes"
    'Insérer ici les variables internes propres au driver et non communes

#End Region

#Region "Propriétés génériques"
    ''' <summary>
    ''' Evènement déclenché par le driver au serveur
    ''' </summary>
    ''' <param name="DriveName"></param>
    ''' <param name="TypeEvent"></param>
    ''' <param name="Parametre"></param>
    ''' <remarks></remarks>
    Public Event DriverEvent(ByVal DriveName As String, ByVal TypeEvent As String, ByVal Parametre As Object) Implements HoMIDom.HoMIDom.IDriver.DriverEvent

    ''' <summary>
    ''' ID du serveur
    ''' </summary>
    ''' <value>ID du serveur</value>
    ''' <remarks>Permet d'accéder aux commandes du serveur pour lesquels il faut passer l'ID du serveur</remarks>
    Public WriteOnly Property IdSrv As String Implements HoMIDom.HoMIDom.IDriver.IdSrv
        Set(ByVal value As String)
            _IdSrv = value
        End Set
    End Property

    ''' <summary>
    ''' Port COM du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
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

    ''' <summary>
    ''' Retourne la liste des devices supportés par le driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Voir Sub New</remarks>
    Public ReadOnly Property DeviceSupport() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.DeviceSupport
        Get
            Return _DeviceSupport
        End Get
    End Property

    ''' <summary>
    ''' Liste des paramètres avancés du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Voir Sub New</remarks>
    Public Property Parametres() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.Parametres
        Get
            Return _Parametres
        End Get
        Set(ByVal value As System.Collections.ArrayList)
            _Parametres = value
        End Set
    End Property

    ''' <summary>
    ''' Liste les libellés et tooltip des champs associés au driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LabelsDriver() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.LabelsDriver
        Get
            Return _LabelsDriver
        End Get
        Set(ByVal value As System.Collections.ArrayList)
            _LabelsDriver = value
        End Set
    End Property

    ''' <summary>
    ''' Liste les libellés et tooltip des champs associés au device associé au driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LabelsDevice() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.LabelsDevice
        Get
            Return _LabelsDevice
        End Get
        Set(ByVal value As System.Collections.ArrayList)
            _LabelsDevice = value
        End Set
    End Property

    ''' <summary>
    ''' Active/Désactive le driver
    ''' </summary>
    ''' <value>True si actif</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Enable() As Boolean Implements HoMIDom.HoMIDom.IDriver.Enable
        Get
            Return _Enable
        End Get
        Set(ByVal value As Boolean)
            _Enable = value
        End Set
    End Property

    ''' <summary>
    ''' ID du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ID() As String Implements HoMIDom.HoMIDom.IDriver.ID
        Get
            Return _ID
        End Get
    End Property

    ''' <summary>
    ''' Adresse IP TCP du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IP_TCP() As String Implements HoMIDom.HoMIDom.IDriver.IP_TCP
        Get
            Return _IP_TCP
        End Get
        Set(ByVal value As String)
            _IP_TCP = value
        End Set
    End Property

    ''' <summary>
    ''' Adresse IP UDP du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IP_UDP() As String Implements HoMIDom.HoMIDom.IDriver.IP_UDP
        Get
            Return _IP_UDP
        End Get
        Set(ByVal value As String)
            _IP_UDP = value
        End Set
    End Property

    ''' <summary>
    ''' Permet de savoir si le driver est actif
    ''' </summary>
    ''' <value>Retourne True si le driver est démarré</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsConnect() As Boolean Implements HoMIDom.HoMIDom.IDriver.IsConnect
        Get
            Return _IsConnect
        End Get
    End Property

    ''' <summary>
    ''' Modèle du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Modele() As String Implements HoMIDom.HoMIDom.IDriver.Modele
        Get
            Return _Modele
        End Get
        Set(ByVal value As String)
            _Modele = value
        End Set
    End Property

    ''' <summary>
    ''' Nom du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Nom() As String Implements HoMIDom.HoMIDom.IDriver.Nom
        Get
            Return _Nom
        End Get
    End Property

    ''' <summary>
    ''' Image du driver (non utilisé)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Picture() As String Implements HoMIDom.HoMIDom.IDriver.Picture
        Get
            Return _Picture
        End Get
        Set(ByVal value As String)
            _Picture = value
        End Set
    End Property

    ''' <summary>
    ''' Port TCP du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Port_TCP() As String Implements HoMIDom.HoMIDom.IDriver.Port_TCP
        Get
            Return _Port_TCP
        End Get
        Set(ByVal value As String)
            _Port_TCP = value
        End Set
    End Property

    ''' <summary>
    ''' Port UDP du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Port_UDP() As String Implements HoMIDom.HoMIDom.IDriver.Port_UDP
        Get
            Return _Port_UDP
        End Get
        Set(ByVal value As String)
            _Port_UDP = value
        End Set
    End Property

    ''' <summary>
    ''' Type de protocole utilisé par le driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Protocol() As String Implements HoMIDom.HoMIDom.IDriver.Protocol
        Get
            Return _Protocol
        End Get
    End Property

    ''' <summary>
    ''' Valeur de rafraichissement des devices
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Refresh() As Integer Implements HoMIDom.HoMIDom.IDriver.Refresh
        Get
            Return _Refresh
        End Get
        Set(ByVal value As Integer)
            _Refresh = value
        End Set
    End Property

    ''' <summary>
    ''' Objet représentant le serveur
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Server() As HoMIDom.HoMIDom.Server Implements HoMIDom.HoMIDom.IDriver.Server
        Get
            Return _Server
        End Get
        Set(ByVal value As HoMIDom.HoMIDom.Server)
            _Server = value
        End Set
    End Property

    ''' <summary>
    ''' Version du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
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

    ''' <summary>
    ''' True si le driver doit démarrer automatiquement
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
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

    ''' <summary>Retourne la liste des Commandes avancées de type DeviceCommande</summary>
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

    ''' <summary>Permet de vérifier si un champ est valide</summary>
    ''' <param name="Champ">Nom du champ à vérifier, ex ADRESSE1</param>
    ''' <param name="Value">Valeur à vérifier</param>
    ''' <returns>Retourne 0 si OK, sinon un message d'erreur</returns>
    ''' <remarks></remarks>
    Public Function VerifChamp(ByVal Champ As String, ByVal Value As Object) As String Implements HoMIDom.HoMIDom.IDriver.VerifChamp
        Try
            Dim retour As String = "0"
            Select Case UCase(Champ)
                Case "ADRESSE1"

                Case "ADRESSE2"

            End Select
            Return retour
        Catch ex As Exception
            Return "Une erreur est apparue lors de la vérification du champ " & Champ & ": " & ex.ToString
        End Try
    End Function

    ''' <summary>Démarrer le driver</summary>
    ''' <remarks></remarks>
    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        Try
            'récupération des paramétres avancés
            Try

            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "Erreur dans les paramétres avancés. utilisation des valeur par défaut" & ex.Message)
            End Try

            _IsConnect = True
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Start", "Driver " & Me.Nom & " démarré")
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", ex.Message)
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            _IsConnect = False
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Stop", "Driver " & Me.Nom & " arrêté")
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Stop", ex.Message)
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
    ''' <remarks>Le device demande au driver d'aller le lire suivant son adresse</remarks>
    Public Sub Read(ByVal Objet As Object) Implements HoMIDom.HoMIDom.IDriver.Read
        Try
            If _Enable = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur: Impossible de traiter la commande car le driver n'est pas activé (Enable)")
                Exit Sub
            End If

            Select Case Objet.Type
                Case "CONTACT"
                    If String.IsNullOrEmpty(Trim(UCase(Objet.Modele))) Then
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur: Le modèle du composant de type CONTACT n'a pas été renseigné (ex: IPX800)!")
                    Else
                        If Trim(UCase(Objet.Modele)) = "IPX800" Then
                            Dim idx As Integer = CInt(Objet.Adresse1)
                            If idx < 0 Or idx > 7 Then
                                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur: l'adresse du device (Adresse1) doit être comprise entre 0 et 7 pour une entrée")
                                Exit Sub
                            End If
                            Dim url As String = "http://" & Objet.Adresse2
                            Dim elmt As String = "btn" & idx
                            Objet.Value = GET_IPX800(url, elmt)
                        Else
                            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur: Le type CONTACT n'est pas géré par " & Objet.Modele)
                        End If
                    End If
                Case "APPAREIL"
                    If String.IsNullOrEmpty(Trim(UCase(Objet.Modele))) Then
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur: Le modèle du composant de type APPAREIL n'a pas été renseigné (ex: IPX800)!")
                    Else
                        If Trim(UCase(Objet.Modele)) = "IPX800" Then
                            Dim idx As Integer = CInt(Objet.Adresse1)
                            idx = idx - 1
                            If idx < 0 Or idx > 7 Then
                                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur: l'adresse du device (Adresse1) doit être comprise entre 1 et 8 pour une sortie")
                                Exit Sub
                            End If
                            Dim url As String = "http://" & Objet.Adresse2
                            Dim elmt As String = "led" & idx
                            Objet.Value = GET_IPX800(url, elmt)
                        Else
                            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur: Le type APPAREIL n'est pas géré par " & Objet.Modele)
                        End If
                    End If
                Case "GENERIQUEVALUE"
                    If Trim(UCase(Objet.Modele)) = "IPX800" Then
                        Dim idx As Integer = CInt(Objet.Adresse1)
                        If idx < 0 Or idx > 3 Then
                            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur: l'adresse du device (Adresse1) doit être comprise entre 0 et 3 pour une entrée analogique")
                            Exit Sub
                        End If
                        Dim url As String = "http://" & Objet.Adresse2
                        Dim elmt As String = "analog" & idx
                        Objet.Value = GET_IPX800(url, elmt)
                    ElseIf Trim(UCase(Objet.Modele)) = "ECODEVICE" Then
                        'Dim idx As String = Objet.Adresse1
                        'Dim url As String = "http://" & Objet.Adresse2 & "/teleinfo.xml"
                        'Dim objectretour As Object = GET_ECODEVICE(url, idx)
                        'If IsNumeric(objectretour) Then
                        '    Objet.Value = CDbl(objectretour)
                        'Else
                        '    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur: " & objectretour & " n'est pas au bon format pour " & Objet.Name & "(GENERIQUEVALUE)")
                        'End If
                        GET_ECODEVICE2(Objet)
                    Else
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur: Le type GENERIQUEVALUE n'est pas géré par " & Objet.Modele)
                    End If
                Case "GENERIQUESTRING"
                    If Trim(UCase(Objet.Modele)) = "RSS" Then
                        If Objet.Adresse1 = "" Or Objet.Adresse2 = "" Then
                            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur: les adresses du device Adresse1 (adresse http du flux rss) et Adresse2 (nom de l'item à lire) doivent être renseignés")
                            Exit Sub
                        End If

                        Dim xmlDoc As New XmlDocument()
                        xmlDoc.Load(Objet.Adresse1)
                        Dim itemNodes As XmlNodeList = xmlDoc.SelectNodes("//rss/channel/item")
                        Dim result As String = ""
                        For Each itemNode As XmlNode In itemNodes
                            Dim titleNode As XmlNode = itemNode.SelectSingleNode(Objet.Adresse2)
                            result &= titleNode.InnerText & vbCrLf
                        Next
                        Objet.Value = result
                    ElseIf Trim(UCase(Objet.Modele)) = "ECODEVICE" Then
                        'Dim idx As String = Objet.Adresse1
                        'Dim url As String = "http://" & Objet.Adresse2 & "/teleinfo.xml"
                        'Dim objectretour As Object = GET_ECODEVICE(url, idx)
                        'Objet.Value = CDbl(objectretour)
                        GET_ECODEVICE2(Objet)
                    Else
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur: Le type GENERIQUESTRING n'est pas géré par " & Objet.Modele)
                    End If
                Case "ENERGIEINSTANTANEE"
                    If Trim(UCase(Objet.Modele)) = "ECODEVICE" Then
                        'Dim idx As String = Objet.Adresse1
                        'Dim url As String = "http://" & Objet.Adresse2 & "/teleinfo.xml"
                        'Dim objectretour As Object = GET_ECODEVICE(url, idx)
                        'If IsNumeric(objectretour) Then
                        '    Objet.Value = CDbl(objectretour)
                        'Else
                        '    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur: " & objectretour & " n'est pas au bon format pour " & Objet.Name & "(ENERGIEINSTANTANEE)")
                        'End If
                        GET_ECODEVICE2(Objet)
                    Else
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur: Le type GENERIQUEVALUE n'est pas géré par " & Objet.Modele)
                    End If
                Case "ENERGIETOTALE"
                    If Trim(UCase(Objet.Modele)) = "ECODEVICE" Then
                        'Dim idx As String = Objet.Adresse1
                        'Dim url As String = "http://" & Objet.Adresse2 & "/teleinfo.xml"
                        'Dim objectretour As Object = GET_ECODEVICE(url, idx)
                        'If IsNumeric(objectretour) Then
                        '    Objet.Value = CDbl(objectretour)
                        'Else
                        '    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur: " & objectretour & " n'est pas au bon format pour " & Objet.Name & "(ENERGIEINSTANTANEE)")
                        'End If
                        GET_ECODEVICE2(Objet)
                    Else
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur: Le type GENERIQUEVALUE n'est pas géré par " & Objet.Modele)
                    End If
                Case Else
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur: Le Device n'est pas reconnu pour ce type " & Objet.Type)
                    Exit Sub
            End Select

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur: " & ex.ToString)
        End Try
    End Sub

    ''' <summary>Commander un device</summary>
    ''' <param name="Objet">Objet représetant le device à commander</param>
    ''' <param name="Command">La commande à passer</param>
    ''' <param name="Parametre1">parametre 1 de la commande, optionnel</param>
    ''' <param name="Parametre2">parametre 2 de la commande, optionnel</param>
    ''' <remarks></remarks>
    Public Sub Write(ByVal Objet As Object, ByVal Command As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
        Try
            If _Enable = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write", "Erreur: Impossible de traiter la commande car le driver n'est pas activé (Enable)")
                Exit Sub
            End If

            If _IsConnect = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write", "Erreur: Impossible de traiter la commande car le driver n'est pas connecté à la carte")
                Exit Sub
            End If

            If Objet.Type = "APPAREIL" Then
                Select Case UCase(Command)
                    Case "ON"
                        Select Case Trim(UCase(Objet.modele))
                            Case "IPX800"
                                Dim idx As Integer = CInt(Objet.Adresse1)
                                If idx < 1 Or idx > 8 Then
                                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write", "Erreur: l'adresse du device (Adresse1) doit être comprise entre 1 et 8 pour une sortie")
                                    Exit Sub
                                End If
                                Dim relais As Integer = idx
                                Dim url As String = "http://" & Objet.Adresse2 & "/preset.htm?led" & relais & "=1"
                                SEND_IPX800(url)
                            Case "ARDUINO"
                                Dim idx As Integer = CInt(Objet.Adresse1)
                                If idx < 1 Or idx > 4 Then
                                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write", "Erreur: l'adresse du device (Adresse1) doit être comprise entre 1 et pour une sortie")
                                    Exit Sub
                                End If
                                Dim relais As Integer = idx
                                Dim url As String = "http://" & Objet.Adresse2 & "/?L=" & relais
                                SEND_ARDUINO(url)
                        End Select
                        Objet.value = True
                    Case "OFF"
                        Select Case Trim(UCase(Objet.modele))
                            Case "IPX800"
                                Dim idx As Integer = CInt(Objet.Adresse1)
                                If idx < 1 Or idx > 8 Then
                                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write", "Erreur: l'adresse du device (Adresse1) doit être comprise entre 1 et 8 pour une sortie")
                                    Exit Sub
                                End If
                                Dim relais As Integer = idx
                                Dim url As String = "http://" & Objet.Adresse2 & "/preset.htm?led" & relais & "=0"
                                SEND_IPX800(url)
                            Case "ARDUINO"
                                Dim idx As Integer = CInt(Objet.Adresse1)
                                If idx < 1 Or idx > 4 Then
                                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write", "Erreur: l'adresse du device (Adresse1) doit être comprise entre 1 et pour une sortie")
                                    Exit Sub
                                End If
                                Dim relais As Integer = idx
                                Dim url As String = "http://" & Objet.Adresse2 & "/?L=" & relais
                                SEND_ARDUINO(url)
                        End Select
                        Objet.value = False
                End Select
            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write", "Erreur: le type du device " & Objet.Type & " n'est pas reconnu pour ce driver")
                Exit Sub
            End If

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write", "Erreur: " & ex.ToString)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " DeleteDevice", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " NewDevice", ex.Message)
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
            _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString

            'liste des devices compatibles
            _DeviceSupport.Add(ListeDevices.FREEBOX)
            _DeviceSupport.Add(ListeDevices.MULTIMEDIA)
            _DeviceSupport.Add(ListeDevices.CONTACT)
            _DeviceSupport.Add(ListeDevices.APPAREIL)
            _DeviceSupport.Add(ListeDevices.GENERIQUEVALUE)
            _DeviceSupport.Add(ListeDevices.GENERIQUESTRING)
            _DeviceSupport.Add(ListeDevices.ENERGIEINSTANTANEE)
            _DeviceSupport.Add(ListeDevices.ENERGIETOTALE)

            'Parametres avancés
            'add_paramavance("nom", "Description", valeupardefaut)

            'ajout des commandes avancées pour les devices
            'add_devicecommande("COMMANDE", "DESCRIPTION", nbparametre)
            'add_devicecommande("PRESETDIM", "permet de paramétrer le DIM : param1=niveau, param2=timer", 2)

            'Libellé Driver
            'add_libelledriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Début URL", "")
            Add_LibelleDevice("ADRESSE2", "Fin URL (Optionnel)", "")
            Add_LibelleDevice("SOLO", "si décoché, les autres composants eco-device seront mis à jour en même temps que celui-ci automatiquement.", "")
            Add_LibelleDevice("MODELE", "Modele", "modèle du device: IPX800,RSS,Arduino,Eco Device""Arduino|EcoDevice|IPX800|RSS")
            'Add_LibelleDevice("REFRESH", "Refresh (sec)", "Valeur de rafraîchissement de la mesure en secondes")
            'Add_LibelleDevice("LASTCHANGEDUREE", "LastChange Durée", "")

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " New", ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)

    End Sub

#End Region

#Region "Fonctions internes"
    'Insérer ci-dessous les fonctions propres au driver et nom communes (ex: start)
    Public Function EnvoyerCode(ByVal Code As String, Optional ByVal Repeat As Integer = 0) As String
        If _Enable = False Then
            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " EnvoyerCode", "Erreur: impossible d'envoyer le code, le driver http n'est pas activé")
            Return "Erreur: impossible d'envoyer le code, le driver http n'est pas activé"
            Exit Function
        End If

        If _IsConnect = False Then
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " EnvoyerCode", "Erreur: Impossible de traiter la commande car le driver n'est pas démarré")
            Return "Erreur: impossible d'envoyer le code, le driver http n'est pas démarré"
            Exit Function
        End If

        Dim URL As String = Code
        If URL = "" Then
            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " EnvoyerCode", "Erreur: impossible d'envoyer le code celui-ci est vide")
            Return "Erreur: impossible d'envoyer le code celui-ci est vide"
            Exit Function
        End If

        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " EnvoyerCode", "Code=" & URL)

        Try
            Dim reader As StreamReader = Nothing
            Dim str As String = ""
            Dim _repeat As Integer = Repeat

            If _repeat < 0 Then _repeat = 0

            For i As Integer = 0 To Repeat
                Dim request As WebRequest = WebRequest.Create(URL)
                Dim response As WebResponse = request.GetResponse()
                reader = New StreamReader(response.GetResponseStream())
                str = reader.ReadToEnd
                reader.Close()
            Next

            Return str
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " EnvoyerCode", ex.Message)
            Return "Erreur: " & ex.ToString
        End Try
    End Function

    Public Sub SEND_IPX800(ByVal Command As String)
        Try
            Dim reader As StreamReader = Nothing
            Dim str As String = ""
            Dim request As WebRequest = WebRequest.Create(Command)
            Dim response As WebResponse = request.GetResponse()
            reader = New StreamReader(response.GetResponseStream())
            str = reader.ReadToEnd
            reader.Close()
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " SEND_IPX800", ex.Message)
        End Try
    End Sub

    Public Function GET_IPX800(ByVal Adresse As String, ByVal Element As String) As Object
        Try
            'Dim doc As New XmlDocument
            'Dim nodes As XmlNodeList
            Dim retour As Object = Nothing
            ' Create a new XmlDocument   
            'doc = New XmlDocument()

            Dim url As New Uri(Adresse & "/status.xml")
            Dim Request As HttpWebRequest = CType(HttpWebRequest.Create(url), System.Net.HttpWebRequest)
            Dim response As Net.HttpWebResponse = CType(Request.GetResponse(), Net.HttpWebResponse)
            Dim flag As Boolean = False

            'doc.Load(response.GetResponseStream)
            'doc.Save("IPX800.xml")
            Dim SR As New StreamReader(response.GetResponseStream)

            Dim Line As String = ""
            Dim a As String = "<" & Element & ">"

            Do Until SR.Peek = -1
                Line = SR.ReadLine()

                Dim b As String = Trim(Line)
                If b.StartsWith(a) Then
                    Dim idx1 As Integer = InStr(2, b, ">")
                    Dim idx2 As Integer = InStr(idx1, b, "<")
                    If idx1 > 0 And idx2 > 0 And idx2 > idx1 Then
                        Dim idx3 As Integer = idx1 + 1
                        Dim c As String = ""
                        Do While Mid(b, idx3, 1) <> "<"
                            c &= Mid(b, idx3, 1)
                            idx3 += 1
                        Loop
                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " GET_IPX800", "Trouvé " & c)
                        retour = c
                        If IsNumeric(retour) = False Then
                            If LCase(retour.ToString) = "up" Then
                                retour = 1
                            Else
                                retour = 0
                            End If
                        End If
                    End If
                Else
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " GET_IPX800", "ligne: " & b)
                End If
            Loop

            SR.Close()
            'FileOpen(1, "IPX800.xml", OpenMode.Input) ' Ouvre en lecture.

            'While Not EOF(1) ' Boucler jusqu'à la fin du fichier
            '    Line = LineInput(1) ' Lire chaque ligne
            '    Dim a As String = "<" & Element & ">"
            '    Dim b As String = Trim(Line)
            '    If b.StartsWith(a) Then
            '        Dim idx1 As Integer = InStr(2, b, ">")
            '        Dim idx2 As Integer = InStr(idx1, b, "<")
            '        If idx1 > 0 And idx2 > 0 And idx2 > idx1 Then
            '            Dim idx3 As Integer = idx1 + 1
            '            Dim c As String = ""
            '            Do While Mid(b, idx3, 1) <> "<"
            '                c &= Mid(b, idx3, 1)
            '                idx3 += 1
            '            Loop
            '            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " GET_IPX800", "Trouvé " & c)
            '            retour = c
            '            If IsNumeric(retour) = False Then
            '                If LCase(retour.ToString) = "up" Then
            '                    retour = 1
            '                Else
            '                    retour = 0
            '                End If
            '            End If
            '        End If
            '    Else
            '        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " GET_IPX800", "ligne: " & b)
            '    End If
            'End While
            'FileClose(1) ' Fermer.
            'File.Delete("IPX800.xml")

            'nodes = doc.SelectNodes("response/")

            'If nodes.Count > 0 Then
            '    For Each node As XmlNode In nodes
            '        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " GET_IPX800", "Node " & node.Name & " Value=" & node.Value)
            '        If Trim(LCase(node.Name)) = Trim(LCase(Element)) Then
            '            flag = True
            '            retour = node.Value
            '            If IsNumeric(retour) = False Then
            '                If LCase(retour.ToString) = "up" Then
            '                    retour = 1
            '                Else
            '                    retour = 0
            '                End If
            '            End If
            '            Exit For
            '        End If
            '    Next
            '    If flag = False Then
            '        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " GET_IPX800", "l'élément " & Element & " n'a pas été trouvé en l'envoyant à l'adresse " & Adresse & "/status.xml")
            '    End If
            'Else
            '    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " GET_IPX800", "Aucun élément n'a été trouvé")
            'End If

            'doc = Nothing
            'nodes = Nothing

            Return retour
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " GET_IPX800", ex.Message)
            Return Nothing
        End Try
    End Function

    'Public Function GET_ECODEVICE(ByVal Adresse As String, ByVal Element As String) As Object
    '    Try
    '        Dim retour As Object = Nothing

    '        Dim url As New Uri(Adresse) 'http://adresseip/teleinfo.xml
    '        Dim Request As HttpWebRequest = CType(HttpWebRequest.Create(url), System.Net.HttpWebRequest)
    '        Dim response As Net.HttpWebResponse = CType(Request.GetResponse(), Net.HttpWebResponse)
    '        Dim flag As Boolean = False

    '        Dim SR As New StreamReader(response.GetResponseStream)
    '        Dim Line As String = ""
    '        Dim a As String = "<" & Element & ">"

    '        Do Until SR.Peek = -1
    '            Line = SR.ReadLine()
    '            Dim b As String = Trim(Line)
    '            If b.StartsWith(a) Then
    '                Dim idx1 As Integer = InStr(2, b, ">")
    '                Dim idx2 As Integer = InStr(idx1, b, "<")
    '                If idx1 > 0 And idx2 > 0 And idx2 > idx1 Then
    '                    Dim idx3 As Integer = idx1 + 1
    '                    Dim c As String = ""
    '                    Do While Mid(b, idx3, 1) <> "<"
    '                        c &= Mid(b, idx3, 1)
    '                        idx3 += 1
    '                    Loop
    '                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " GET_ECODEVICE", "Trouvé " & c)
    '                    retour = c
    '                End If
    '            Else
    '                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " GET_ECODEVICE", "ligne: " & b)
    '            End If
    '        Loop

    '        SR.Close()
    '        Return retour
    '    Catch ex As Exception
    '        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " GET_ECODEVICE", ex.Message)
    '        Return Nothing
    '    End Try
    'End Function
    Public Sub GET_ECODEVICE2(ByVal composant As Object)
        Try
            Dim valeur As String = ""

            Dim url As New Uri("http://" & composant.Adresse2 & "/teleinfo.xml")
            Dim Request As HttpWebRequest = CType(HttpWebRequest.Create(url), System.Net.HttpWebRequest)
            Dim response As Net.HttpWebResponse = CType(Request.GetResponse(), Net.HttpWebResponse)
            Dim flag As Boolean = False

            Dim SR As New StreamReader(response.GetResponseStream)
            Dim Line As String = ""
            'si on met à jour uniquement ce composant
            If composant.solo Then
                Do Until SR.Peek = -1
                    Line = Trim(SR.ReadLine())
                    If Line.StartsWith("<" & composant.adresse1 & ">") Then
                        valeur = Mid(Line, InStr(2, Line, ">") + 1, InStr(2, Line, "<") - InStr(2, Line, ">") - 1)
                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " GET_ECODEVICE", composant.Name & " Trouvé : " & valeur)
                        'mise à jour du composant
                        If UCase(composant.Type) = "ENERGIETOTALE" Or UCase(composant.Type) = "ENERGIEINSTANTANEE" Or UCase(composant.Type) = "GENERIQUEVALUE" Then
                            If IsNumeric(valeur) Then
                                composant.Value = CDbl(valeur)
                            Else
                                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " GET_ECODEVICE", "Erreur: " & valeur & " n'est pas au bon format pour " & composant.Name & "(" & UCase(composant.Type) & ")")
                            End If
                        ElseIf UCase(composant.Type) = "GENERIQUESTRING" Then
                            composant.Value = valeur
                        End If
                    End If
                Loop
            Else
                Do Until SR.Peek = -1
                    Line = Trim(SR.ReadLine())
                    If Line.StartsWith("<T1_") Or Line.StartsWith("<T2_") Then
                        Dim nombalise As String = Mid(Line, 2, InStr(2, Line, ">") - 2)
                        valeur = Mid(Line, InStr(2, Line, ">") + 1, InStr(2, Line, "<") - InStr(2, Line, ">") - 1)
                        'si c'est notre composant, on le met à jour directement
                        If nombalise = composant.adresse1 Then
                            If UCase(composant.Type) = "ENERGIETOTALE" Or UCase(composant.Type) = "ENERGIEINSTANTANEE" Or UCase(composant.Type) = "GENERIQUEVALUE" Then
                                If IsNumeric(valeur) Then
                                    composant.Value = CDbl(valeur)
                                Else
                                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " GET_ECODEVICE", "Erreur: " & valeur & " n'est pas au bon format pour " & composant.Name & "(" & UCase(composant.Type) & ")")
                                End If
                            ElseIf UCase(composant.Type) = "GENERIQUESTRING" Then
                                composant.Value = valeur
                            End If
                        Else
                            'sinon on cherche si un autre composant pour adresse la balise avec le même driver
                            Dim listedevices As New ArrayList
                            listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, nombalise, "", Me._ID, True)
                            If IsNothing(listedevices) Then
                                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " GET_ECODEVICE", "Communication impossible avec le serveur, l'IDsrv est peut être erroné : " & _IdSrv)
                                Exit Sub
                            ElseIf (listedevices.Count = 1) Then
                                'un device trouvé, on le met à jour
                                If UCase(listedevices.Item(0).Type) = "ENERGIETOTALE" Or UCase(listedevices.Item(0).Type) = "ENERGIEINSTANTANEE" Or UCase(listedevices.Item(0).Type) = "GENERIQUEVALUE" Then
                                    If IsNumeric(valeur) Then
                                        listedevices.Item(0).Value = CDbl(valeur)
                                    Else
                                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " GET_ECODEVICE", "Erreur: " & valeur & " n'est pas au bon format pour " & listedevices.Item(0).Name & "(" & UCase(listedevices.Item(0).Type) & ")")
                                    End If
                                ElseIf UCase(listedevices.Item(0).Type) = "GENERIQUESTRING" Then
                                    listedevices.Item(0).Value = valeur
                                End If
                            ElseIf (listedevices.Count > 1) Then
                                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " GET_ECODEVICE", "Plusieurs composants correspondent à : " & nombalise & ":" & valeur)
                            End If
                        End If
                    End If
                Loop
            End If
            SR.Close()
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " GET_ECODEVICE", ex.Message)
        End Try
    End Sub

    Public Sub SEND_ARDUINO(ByVal Command As String)
        Try
            Dim reader As StreamReader = Nothing
            Dim str As String = ""
            Dim request As WebRequest = WebRequest.Create(Command)
            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " SEND_ARDUINO", "Commande envoyée: " & Command)
            Dim response As WebResponse = request.GetResponse()
            reader = New StreamReader(response.GetResponseStream())
            str = reader.ReadToEnd
            reader.Close()
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " SEND_ARDUINO", ex.Message)
        End Try
    End Sub
#End Region
End Class
