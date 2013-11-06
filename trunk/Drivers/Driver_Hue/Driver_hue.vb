Imports HoMIDom
Imports HoMIDom.HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device

Imports Q42.HueApi

Imports AsyncCtpExtensions
Imports AsyncCtpThreadingExtensions
Imports Newtonsoft.Json

Imports System
Imports System.IO
Imports System.Net
Imports System.Data
Imports System.Linq
Imports System.ComponentModel
Imports System.Threading.Tasks
Imports System.Collections.Generic
Imports System.Runtime.Serialization

Imports STRGS = Microsoft.VisualBasic.Strings
' Auteur : Alamata
' Date : 01/11/2013

''' <summary>Class Driver_Hue</summary>
''' <remarks></remarks>
<Serializable()> Public Class Driver_hue
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "13A70564-4610-11E3-B526-5C6B6188709B" 'ne pas modifier car utilisé dans le code du serveur
    Dim _Nom As String = "Hue" 'Nom du driver à afficher
    Dim _Enable As Boolean = False 'Activer/Désactiver le driver
    Dim _Description As String = "Driver Hue Philips" 'Description du driver
    Dim _StartAuto As Boolean = False 'True si le driver doit démarrer automatiquement
    Dim _Protocol As String = "Ethernet" 'Protocole utilisé par le driver, exemple: RS232
    Dim _IsConnect As Boolean = False 'True si le driver est connecté et sans erreur
    Dim _IP_TCP As String = "" 'Adresse IP TCP à utiliser, "@" si non applicable pour le cacher côté client
    Dim _Port_TCP As String = "@" 'Port TCP à utiliser, "@" si non applicable pour le cacher côté client
    Dim _IP_UDP As String = "@" 'Adresse IP UDP à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Port_UDP As String = "@" 'Port UDP à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Com As String = "@" 'Port COM à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Refresh As Integer = 0 'Valeur à laquelle le driver doit rafraichir les valeurs des devices (ex: toutes les 200ms aller lire les devices)
    Dim _Modele As String = "Ethernet" 'Modèle du driver/interface
    Dim _Version As String = My.Application.Info.Version.ToString 'Version du driver
    Dim _OsPlatform As String = "3264" 'Plateforme compatible 32 64 ou 3264
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

    'param avancé
    'Dim _DEBUG As Boolean = False
#End Region

#Region "Variables Internes"
    'Insérer ici les variables internes propres au driver et non communes
    Dim _Registered As Boolean = False
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
    Public ReadOnly Property OsPlatform() As String Implements HoMIDom.HoMIDom.IDriver.OsPlatform
        Get
            Return _OsPlatform
        End Get
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
                    Write(MyDevice, Command, Param(0))
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
        Dim retour As String = ""

        'récupération des paramétres avancés
        Try
            '_DEBUG = _Parametres.Item(0).Valeur
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "Erreur dans les paramétres avancés. utilisation des valeur par défaut" & ex.Message)
        End Try
        Try
            If _IP_TCP <> "" Then


                    GetAllHueDevices()





            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "ERR: Impossible d'atteindre le Bridge Hue pas d'IP renseignée !")
            End If

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
            If _Enable = False Then Exit Sub
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", ex.Message)
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

            If UCase(Command) = "ONONE" Then
                ONone(Objet.adresse1.ToString())
            End If
            If UCase(Command) = "ONALL" Then
                ONall()
            End If
            If UCase(Command) = "OFFONE" Then
                OFFone(Objet.adresse1.ToString())
            End If
            If UCase(Command) = "OFFALL" Then
                OFFall()
            End If

            If UCase(Command) = "ONONESETCOLOR" Then
                If Parametre1 IsNot Nothing Then
                    Objet.Value = Parametre1
                    ONoneSetColor(Parametre1, Objet.adresse1.ToString())

                End If
            End If
            If UCase(Command) = "ONALLSETCOLOR" Then
                If Parametre1 IsNot Nothing Then
                    Objet.Value = Parametre1
                    ONallSetColor(Parametre1)

                End If
            End If
            If UCase(Command) = "SETCOLOR" Then
                If Parametre1 IsNot Nothing Then
                    Objet.Value = Parametre1
                    SetColor(Parametre1, Objet.adresse1.ToString())

                End If
            End If
            If UCase(Command) = "REGISTER" Then
                Register()


            End If
            If UCase(Command) = "OPERATION" Then      'ID:###########OP:-ID:############OP:+NB:2 --> composant1-composant2+2
                If Parametre1 IsNot Nothing Then
                    If InStr(Parametre1, "OP:") Then
                        Dim rValue As Double
                        Dim TabOp As String() = {"+", "-", "*", "/", "^"}
                        Dim op As String = Mid(Parametre1, InStr(Parametre1, "OP:") + 3, 1)
                        If TabOp.Contains(op) Then
                            Dim Param = Split(Parametre1, "OP:" & op)
                            For i As Integer = 0 To Param.Count - 1
                                If Param(i) IsNot Nothing Then
                                    If InStr(Param(i), ":") Then
                                        rValue = Eval(rValue & op & Decode(Param(i)))
                                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Write Opération", "Opération: " & op & Decode(Param(i)) & " Egal: " & rValue)
                                    Else
                                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write Opération", "Il manque le type d'opérande (NB: ou ID:) dans l'expression " & Param(i))
                                    End If
                                Else
                                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write Opération", "Le parametre " & i & " est incorrecte ")
                                End If
                            Next
                            Objet.setValue(rValue)
                        Else
                            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write Opération", "L'opérateur n'existe pas ")
                        End If
                    Else
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write Opération", "Il manque OP: ")
                    End If
                Else
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write Opération", "Il manque le Parametre1 ")
                End If
            End If

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write", ex.Message)
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
            x.DescriptionCommand = Description
            x.CountParam = NbParam
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
            _DeviceSupport.Add(ListeDevices.LAMPE)

            'Parametres avancés
            'add_paramavance("nom", "Description", valeupardefaut)
            'Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)

            'ajout des commandes avancées pour les devices
            'add_devicecommande("COMMANDE", "DESCRIPTION", nbparametre)
            Add_DeviceCommande("ONone", "Allumer un Device Hue", 0)
            Add_DeviceCommande("ONall", "Allumer tous les Devices Hue", 0)
            Add_DeviceCommande("OFFone", "Eteindre un DeviceHue", 0)
            Add_DeviceCommande("OFFall", "Eteindre tous les Devices Hue", 0)
            Add_DeviceCommande("ONoneSetColor", "Allumer et Indiquer une couleur a un DeviceHue", 1)
            Add_DeviceCommande("ONallSetColor", "Allumer et Indiquer une couleur a tous les Devices Hue", 1)
            Add_DeviceCommande("SetColor", "Indiquer une couleur au Device Hue", 1)
            Add_DeviceCommande("Register", "Enregistrement sur le Bridge Hue", 0)
            'Libellé Driver
            'Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Device ID", "ID du Device Hue associé")
            Add_LibelleDevice("ADRESSE2", "Hue", "Valeur de la Teinte (0-65535)")
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("MODELE", "ColorTmeperature", "Valeur de la Temperature des blancs (153-500)")
            'Add_LibelleDevice("MODELE", "Modele", "Nom du modele de composant : xxx/yyy/zzz", "xxx|yyy|zzz")
            Add_LibelleDevice("REFRESH", "@", "")
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

    Private Function Decode(ByVal msg As String) As Double

        Dim ParaAdr2 = Split(msg, ":")
        Dim ValueR As Double

        Select Case ParaAdr2(0).ToUpper
            Case "ID"
                Dim TempDevice As TemplateDevice = _Server.ReturnDeviceById(_IdSrv, ParaAdr2(1))
                If TempDevice IsNot Nothing Then
                    ValueR = TempDevice.Value
                Else
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Decodage", "le composant qu a l'ID " & ParaAdr2(1) & " n'existe pas")
                End If
            Case "NB"
                ValueR = ParaAdr2(1)
            Case Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Decodage", "la commande " & ParaAdr2(0) & " n'existe pas")
        End Select

        Return ValueR

    End Function

    Private Function Eval(ByVal command As String) As Object
        Dim MyProvider As New VBCodeProvider
        Dim cp As New CodeDom.Compiler.CompilerParameters

        cp.GenerateExecutable = False
        cp.GenerateInMemory = True

        Dim ClassName As String = "class" & Now.Ticks
        Dim TempModuleSource As String = "Imports System" & Environment.NewLine & _
                                         "Namespace ns " & Environment.NewLine & _
                                         "    Public Class " & ClassName & Environment.NewLine & _
                                         "        Public Shared Function Evaluate()" & Environment.NewLine & _
                                         "            Return (" & command & ")" & Environment.NewLine & _
                                         "        End Function" & Environment.NewLine & _
                                         "    End Class" & Environment.NewLine & _
                                         "End Namespace"
        Dim cr As CodeDom.Compiler.CompilerResults = MyProvider.CompileAssemblyFromSource(cp, TempModuleSource)

        If cr.Errors.Count > 0 Then
            Return Nothing
        Else

            Dim methInfo As Reflection.MethodInfo = cr.CompiledAssembly.GetType("ns." & ClassName).GetMethod("Evaluate")

            Return methInfo.Invoke(methInfo, New Object() {})
        End If
    End Function

    Public Async Function GetAllHueDevices() As Task


        Dim client As New HueClient(_IP_TCP)

        client.Initialize("8123486789")

        Dim resultgethuedevices = Await client.GetLightsAsync()

        For Each device As Object In resultgethuedevices
            _IsConnect = True
            Dim gen = New HoMIDom.HoMIDom.Server()
            Dim listedevices As New ArrayList
            listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, device.ID.ToString(), "GENERIQUESTRING", Me._ID, True)
            If IsNothing(listedevices) Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "ERR: Communication impossible avec le serveur, l'IDsrv est peut être erroné : ", _IdSrv)
                Exit Function
            End If
            If (listedevices.Count = 0) Then
                gen.SaveDevice(_IdSrv, "", "Hue-" + device.Name, device.ID.ToString(), True, False, _ID, "GENERIQUESTRING", 0, device.State.Hue.ToString(), "", device.State.ColorTemperature.ToString(), "Devices et Valeurs recuperées du Bridge Hue", 0, False, "0", "", 0, 9999, -9999, 0.0, Nothing, "", 0, True)

                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Start", "Driver " & Me.Nom & " démarré")

            End If


        Next



    End Function

    Public Function ONone(ByVal ParametreID As String)

        Dim command = New LightCommand()
        Dim client As New HueClient(_IP_TCP)
        client.Initialize("8123486789")

        client.SendCommandAsync(command.TurnOn(), New List(Of String)() From { _
         ParametreID _
        })

    End Function

    Public Function ONall()

        Dim command = New LightCommand()
        Dim client As New HueClient(_IP_TCP)
        client.Initialize("8123486789")


        client.SendCommandAsync(command.TurnOn())

    End Function

    Public Function OFFone(ByVal ParametreID As String)

        Dim command = New LightCommand()
        Dim client As New HueClient(_IP_TCP)
        client.Initialize("8123486789")


        client.SendCommandAsync(command.TurnOff(), New List(Of String)() From { _
         ParametreID _
        })

    End Function

    Public Function OFFall()

        Dim command = New LightCommand()
        Dim client As New HueClient(_IP_TCP)
        client.Initialize("8123486789")


        client.SendCommandAsync(command.TurnOff())

    End Function

    Public Function ONoneSetColor(ByVal ParametreRGB As String, ByVal ParametreID As String)

        Dim command = New LightCommand()
        Dim client As New HueClient(_IP_TCP)
        client.Initialize("8123486789")

        Dim arrRGB() As String = ParametreRGB.Split("-")
        Dim paramR As String = arrRGB(0)
        Dim paramG As String = arrRGB(1)
        Dim paramB As String = arrRGB(2)



        client.SendCommandAsync(command.TurnOn().SetColor(paramR, paramG, paramB), New List(Of String)() From { _
         ParametreID _
        })



    End Function

    Public Function ONallSetColor(ByVal ParametreRGB As String)

        Dim command = New LightCommand()
        Dim client As New HueClient(_IP_TCP)
        client.Initialize("8123486789")

        Dim arrRGB() As String = ParametreRGB.Split("-")
        Dim paramR As String = arrRGB(0)
        Dim paramG As String = arrRGB(1)
        Dim paramB As String = arrRGB(2)



        client.SendCommandAsync(command.TurnOn().SetColor(paramR, paramG, paramB))



    End Function

    Public Function SetColor(ByVal ParametreRGB As String, ByVal ParametreID As String)


        Dim command = New LightCommand()
        Dim client As New HueClient(_IP_TCP)
        client.Initialize("8123486789")

        Dim arrRGB() As String = ParametreRGB.Split("-")
        Dim paramR As String = arrRGB(0)
        Dim paramG As String = arrRGB(1)
        Dim paramB As String = arrRGB(2)



        client.SendCommandAsync(command.SetColor(paramR, paramG, paramB), New List(Of String)() From { _
         ParametreID _
        })



    End Function

    Public Async Function Register() As Task

        Dim client As New HueClient(_IP_TCP)


        Dim result = Await client.RegisterAsync("driverHue", "81234567898")

        If result = True Then



            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Enregistrement", "Driver " & Me.Nom & "Enregistrement Bridge Hue Reussi !!")
            GetAllHueDevices()


        Else
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Enregistrement", "Driver " & Me.Nom & "Adresse Ip Bridge Correct !!")
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Enregistrement", "Driver " & Me.Nom & "Enregistrement Bridge Hue Echoué !!")
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Enregistrement", "Driver " & Me.Nom & "Push on the LINK Button on the Bridge Hue BEFORE !!")

        End If
    End Function
#End Region



End Class
