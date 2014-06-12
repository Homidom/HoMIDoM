Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports Microsoft.VisualBasic.Strings
Imports System.Net.Sockets
Imports System.Threading
Imports System.Globalization
Imports com

Public Class Driver_onewire
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "6DAA2DE2-0545-11E1-B580-3CCD4824019B"
    Dim _Nom As String = "1-Wire"
    Dim _Enable As Boolean = False
    Dim _Description As String = "Adaptateur 1-wire USB / COM"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "COM"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "USB1"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "{DS9490}"
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
    Dim _IdSrv As String
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande)
    Dim _AutoDiscover As Boolean = False

#End Region

#Region "Variables Internes"
    Dim wir_adapter As com.dalsemi.onewire.adapter.DSPortAdapter
    Public adapter_present = 0 '=1 si adapteur présent sinon =0
    Private Shared lock_portwrite As New Object
#End Region

#Region "Déclaration"

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
        Try
            If Not _IsConnect Then
                'Initialisation de la cle USB 1-WIRE
                '_Modele = "{DS9490B}"
                ' _Com = "USB2"
                Try
                    If (_Modele = "" Or _Modele = " ") And (_Com = "" Or _Com = " ") Then
                        wir_adapter = dalsemi.onewire.OneWireAccessProvider.getDefaultAdapter
                    Else
                        wir_adapter = dalsemi.onewire.OneWireAccessProvider.getAdapter(_Modele, _Com)
                    End If
                    adapter_present = 1
                    _IsConnect = True
                    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "1-Wire Start", "Adapter " & wir_adapter.getAdapterName & " " & wir_adapter.getPortName)
                Catch ex As Exception
                    adapter_present = 0
                    _IsConnect = False
                    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "1-Wire Start", "ERR: Initialisation : " & ex.ToString & " - " & _Modele & " - " & _Com)

                End Try
            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire Start", "Driver déjà connecté")
            End If
        Catch ex As Exception
            _IsConnect = False
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire Start", ex.Message)
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            If _IsConnect Then
                _IsConnect = False
                wir_adapter.freePort()
                adapter_present = 0
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "1-Wire Stop", "Port " & _Com & " fermé")
            Else
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "1-Wire Stop", "Port " & _Com & " est déjà fermé")
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire Stop", ex.Message)
        End Try
    End Sub

    ''' <summary>Re-Démarrer le du driver</summary>
    ''' <remarks></remarks>
    Public Sub Restart() Implements HoMIDom.HoMIDom.IDriver.Restart
        [Stop]()
        Thread.Sleep(2000)
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
                    Case "TEMPERATURE"
                        Dim retour As Double = temp_get_save(Objet.Adresse1)
                        If retour <> 9999 Then Objet.Value = retour
                    Case "HUMIDITE"
                        Dim retour As Double = humidity_get(Objet.Adresse1)
                        If retour <> 9999 Then Objet.Value = retour
                    Case "SWITCH"
                        If Objet.Adresse2 = "" Then
                            'c'est un simple switch
                            Dim retour As Integer = switch_get(Objet.Adresse1)
                            If retour <> 9999 Then
                                If retour = 1 Then
                                    Objet.Value = True
                                Else
                                    Objet.Value = False
                                End If
                            End If
                        Else
                            If IsNumeric(Objet.Adresse2) Then
                                'l'adresse2 est renseigné et représente le numero du switch à interroger
                                Dim retour As New ArrayList
                                retour = switchs_get(Objet.Adresse1)
                                If Not (retour Is Nothing) Then
                                    'on vérifie si ppt SOLO à true pour multiswitch
                                    If Objet.Solo Then
                                        'SOLO = true : on lit uniquement la valeur du composant qui a lancé le read
                                        'on a bien un resultat, on verifie si notre numero de switch est renvoyé
                                        If CInt(Objet.Adresse2) <= retour.Count Then
                                            If retour(CInt(Objet.Adresse2) - 1) = 1 Then
                                                Objet.Value = True
                                            Else
                                                Objet.Value = False
                                            End If
                                        Else
                                            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire Read", "Le multi-switch " & Objet.Name & " n a pas de switch numero " & Objet.Adresse2)
                                        End If
                                    Else
                                        'SOLO = false : Pour chaque valeur renvoyé, on cherche le composant et on met à jour sa valeur
                                        'on recupere la liste des composant ayant la même adresse1 en 1-wire (même multiswitch)
                                        Dim listedevices As New ArrayList
                                        listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, Objet.Adresse1, "", Me._ID, True)
                                        For i As Integer = 0 To retour.Count
                                            'si le numero est celui de notre composant qui a lancé le read, on met à jour direct sinon on cherche le composant
                                            If i = (Objet.Adresse2 - 1) Then
                                                If retour(CInt(Objet.Adresse2) - 1) = 1 Then
                                                    Objet.Value = True
                                                Else
                                                    Objet.Value = False
                                                End If
                                            Else
                                                'on cherche le composant
                                                For j = 0 To listedevices.Count
                                                    If i = (listedevices.Item(j).Adresse2 - 1) Then
                                                        If retour(listedevices.Item(j).Adresse2 - 1) = 1 Then
                                                            Objet.Value = True
                                                        Else
                                                            Objet.Value = False
                                                        End If
                                                    End If
                                                Next
                                            End If
                                        Next
                                    End If
                                End If
                            Else
                                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire Read", "Le multi-switch " & Objet.Name & " a une adresse2 incorrecte : " & Objet.Adresse2)
                            End If
                        End If

                    Case "CONTACT"
                        Dim retour As Integer = switch_get(Objet.Adresse1)
                        If retour <> 9999 Then
                            If retour = 1 Then
                                Objet.Value = True
                            Else
                                Objet.Value = False
                            End If
                        End If
                    Case "DETECTEUR"
                        Dim retour As Integer = switch_get(Objet.Adresse1)
                        If retour <> 9999 Then
                            If retour = 1 Then
                                Objet.Value = True
                            Else
                                Objet.Value = False
                            End If
                        End If
                    Case "GENERIQUEVALUE"
                        Dim retour As Integer = switch_get(Objet.Adresse1)
                        If retour <> 9999 Then
                            Objet.Value = retour
                        End If
                    Case "COMPTEUR"
                        Dim flag As Boolean = True
                        If Objet.Adresse2 = "B" Then flag = False 'on interroge le deuxieme compteur
                        Dim retour As String = counter(Objet.Adresse1, flag)
                        If retour <> 9999 Then
                            Objet.Value = retour
                        End If
                End Select
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire Read", ex.Message)
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
            If _IsConnect = False Then Exit Sub
            Select Case UCase(Command)
                Case "ON"
                    If Objet.Type = "SWITCH" Then
                        Dim retour As Integer = switch_setstate(Objet.Adresse1, True)
                        If retour = 1 Then Objet.value = True
                    End If
                Case "OFF"
                    If Objet.Type = "SWITCH" Then
                        Dim retour As Integer = switch_setstate(Objet.Adresse1, False)
                        If retour = 0 Then Objet.value = False
                    End If
                Case "DIM"
                Case "OUVERTURE"
            End Select
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire Write", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire DeleteDevice", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire NewDevice", ex.Message)
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

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)

    End Sub

    Public Sub New()
        Try
            _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString

            'liste des devices compatibles
            _DeviceSupport.Add(ListeDevices.APPAREIL.ToString)
            _DeviceSupport.Add(ListeDevices.CONTACT.ToString)
            _DeviceSupport.Add(ListeDevices.DETECTEUR.ToString)
            _DeviceSupport.Add(ListeDevices.SWITCH.ToString)
            _DeviceSupport.Add(ListeDevices.TEMPERATURE.ToString)
            _DeviceSupport.Add(ListeDevices.HUMIDITE.ToString)
            _DeviceSupport.Add(ListeDevices.COMPTEUR.ToString)

            'Parametres avancés
            'add_paramavance("nom", "Description", valeupardefaut)

            'ajout des commandes avancées pour les devices
            'add_devicecommande("COMMANDE", "DESCRIPTION", nbparametre)
            'add_devicecommande("PRESETDIM", "permet de paramétrer le DIM : param1=niveau, param2=timer", 2)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Adresse", "Adresse du composant de type 44000002F4CBFD28")
            Add_LibelleDevice("ADRESSE2", "Port", "Nom/numéro du port pour un compteur (A/B) ou un switch (1/8)	pour un multiswitch/compteur uniquement")
            Add_LibelleDevice("SOLO", "SOLO", "Permet de mettre à jour d une seule lecture tous les swicths/compteurs du même composant dans le cas d un multiswitch/compteur")
            Add_LibelleDevice("MODELE", "@", "")
            'Add_LibelleDevice("REFRESH", "Refresh (sec)", "Valeur de rafraîchissement de la mesure en secondes")
            'Add_LibelleDevice("LASTCHANGEDUREE", "LastChange Durée", "")

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire New", ex.Message)
        End Try
    End Sub
#End Region

#Region "Fonctions propres au driver"

    Private Function temp_get_save(ByVal adresse As String) As Double
        ' Renvoi la temperature du capteur X
        Dim resolution As Double = 0.1 'resolution de la temperature : 0.1 ou 0.5
        Dim retour As Double = 9999
        Dim state As Object
        SyncLock lock_portwrite
            Try
                Dim tc As com.dalsemi.onewire.container.TemperatureContainer
                Dim owd As com.dalsemi.onewire.container.OneWireContainer
                If _IsConnect = True Then
                    'demande l'acces exclusif au reseau
                    wir_adapter.beginExclusive(False)
                    owd = wir_adapter.getDeviceContainer(adresse) 'recupere le composant
                    If owd.isPresent() Then
                        Try
                            tc = DirectCast(owd, com.dalsemi.onewire.container.TemperatureContainer) 'creer la connexion
                            state = tc.readDevice 'lit le capteur
                            If tc.hasSelectableTemperatureResolution() Then
                                tc.setTemperatureResolution(resolution, state) 'modifie la resolution à 0.1 degré (0.5 par défaut)
                                tc.writeDevice(state)
                            End If
                            tc.doTemperatureConvert(state) 'converti la valeur obtenu en temperature
                            state = tc.readDevice 'lit la conversion
                            retour = Math.Round(tc.getTemperature(state), 1)
                        Catch ex As Exception
                            retour = 9999
                            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire GetTemp", ex.ToString)
                        End Try
                    Else
                        retour = 9999
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire GetTemp", "Capteur à l'adresse " & adresse & " Non présent")
                    End If
                    wir_adapter.endExclusive()
                Else
                    retour = 9999
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire GetTemp", "Erreur Adaptateur non présent")
                End If
            Catch ex As Exception
                retour = 9999
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire GetTemp", ex.ToString)
            End Try
        End SyncLock
        Return retour
    End Function

    Private Function temp_get(ByVal adresse As String, ByVal resolution As Double) As String
        ' Renvoi la temperature du capteur X
        'resolution = Domos.WIR_res --> resolution de la temperature : 0.1 ou 0.5
        Dim retour As String = ""
        Dim state As Object
        Dim result As Boolean = False
        SyncLock lock_portwrite
            Try
                Dim tc As com.dalsemi.onewire.container.TemperatureContainer
                Dim owd As com.dalsemi.onewire.container.OneWireContainer
                If adapter_present Then
                    'demande l'acces exclusif au reseau
                    result = wir_adapter.beginExclusive(False)
                    If result Then
                        'wir_adapter.reset()
                        owd = wir_adapter.getDeviceContainer(adresse) 'recupere le composant
                        tc = DirectCast(owd, com.dalsemi.onewire.container.TemperatureContainer) 'creer la connexion
                        If owd.isPresent() Then
                            state = tc.readDevice() 'lit le capteur
                            tc.setTemperatureResolution(resolution, state) 'modifie la resolution à 0.1 degré (0.5 par défaut)
                            tc.doTemperatureConvert(state)
                            state = tc.readDevice 'lit la conversion
                            retour = Math.Round(tc.getTemperature(state), 1)
                        Else
                            retour = "ERR: temp_get : Capteur non présent"
                        End If
                    Else
                        retour = "ERR: temp_get : Acces Exclusif refusé"
                    End If
                    wir_adapter.endExclusive()
                Else
                    retour = "ERR: temp_get : Adaptateur non présent"
                End If
            Catch ex As Exception
                retour = "ERR: temp_get : " & ex.ToString
            End Try
        End SyncLock
        Return retour
    End Function

    Private Function humidity_get(ByVal adresse As String) As Double
        ' Renvoi la valeur de l'humidité du capteur X
        Dim resolution As Double = 5 'resolution de l"humidité en %
        Dim retour As Double = 9999
        Dim state As Object
        SyncLock lock_portwrite
            Try
                Dim hc As com.dalsemi.onewire.container.HumidityContainer
                Dim owd As com.dalsemi.onewire.container.OneWireContainer
                If _IsConnect = True Then
                    'demande l'acces exclusif au reseau
                    wir_adapter.beginExclusive(False)
                    owd = wir_adapter.getDeviceContainer(adresse) 'recupere le composant
                    If owd.isPresent() Then
                        Try
                            hc = DirectCast(owd, com.dalsemi.onewire.container.HumidityContainer) 'creer la connexion
                            state = hc.readDevice 'lit le capteur
                            If hc.hasSelectableHumidityResolution() Then
                                hc.setHumidityResolution(resolution, state)
                                hc.writeDevice(state)
                            End If
                            hc.doHumidityConvert(state) 'converti la valeur obtenu en humidité
                            state = hc.readDevice 'lit la conversion
                            'retour = Math.Round(tc.getHumidity(state), 1)
                            retour = hc.getHumidity(state)
                            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "1-Wire humidity_get", "DEBUG: valeur humidity read : " & retour)
                        Catch ex As Exception
                            retour = 9999
                            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire humidity_get", ex.ToString)
                        End Try
                    Else
                        retour = 9999
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire humidity_get", "Capteur à l'adresse " & adresse & " Non présent")
                    End If
                    wir_adapter.endExclusive()
                Else
                    retour = 9999
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire humidity_get", "Erreur Adaptateur non présent")
                End If
            Catch ex As Exception
                retour = 9999
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire humidity_get", ex.ToString)
            End Try
        End SyncLock
        Return retour
    End Function

    Private Function switchs_get(ByVal adresse As String) As ArrayList
        ' Renvoie l'etat d'un multiswitch sous forme d'un tableau : 0=fermé, 1=ouvert, 2=fermé mais ouvert entre temps
        Dim retour As New ArrayList
        Dim state As Object
        SyncLock lock_portwrite
            Try
                Dim owd As com.dalsemi.onewire.container.OneWireContainer
                Dim tc As com.dalsemi.onewire.container.SwitchContainer
                Dim switch_activity, switch_state
                If adapter_present Then
                    If wir_adapter.isPresent(adresse) Then
                        wir_adapter.beginExclusive(True)
                        owd = wir_adapter.getDeviceContainer(adresse)
                        tc = DirectCast(owd, com.dalsemi.onewire.container.SwitchContainer)
                        state = tc.readDevice()
                        Dim number_of_switches = tc.getNumberChannels(state)
                        For i = 0 To (number_of_switches - 1)
                            switch_state = tc.getLatchState(i, state) 'recup l'etat du switch
                            switch_activity = tc.getSensedActivity(i, state) 'recup l'activité du switch
                            If switch_state Then
                                retour.Add("1")
                            Else
                                If switch_activity Then retour.Add("2") Else retour.Add("0")
                            End If
                        Next
                        tc.clearActivity()
                        tc.readDevice()
                        wir_adapter.endExclusive()
                    Else
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire switchs_get", "Capteur à l'adresse " & adresse & " Non présent")
                        Return Nothing
                    End If
                Else
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire switchs_get", "Adaptateur non présent")
                    Return Nothing
                End If
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire switchs_get", "Erreur: " & ex.ToString)
                Return Nothing
            End Try
        End SyncLock
        Return retour
    End Function

    Private Function switch_switchstate(ByVal adresse As String) As String
        ' Change l'etat d'un switch et renvoi le nouveau etat (ex :0 ==> Off)
        Dim retour As String = ""
        Dim state As Object
        SyncLock lock_portwrite
            Try
                Dim owd As com.dalsemi.onewire.container.OneWireContainer
                Dim tc As com.dalsemi.onewire.container.SwitchContainer
                Dim switch_state
                If _IsConnect Then
                    If wir_adapter.isPresent(adresse) Then
                        wir_adapter.beginExclusive(True)
                        owd = wir_adapter.getDeviceContainer(adresse)
                        tc = DirectCast(owd, com.dalsemi.onewire.container.SwitchContainer)
                        state = tc.readDevice()
                        switch_state = tc.getLatchState(0, state)
                        If switch_state Then retour = "0" Else retour = "1"
                        tc.setLatchState(0, Not switch_state, False, state)
                        tc.writeDevice(state)
                        wir_adapter.endExclusive()
                    Else
                        retour = "ERR: switch_switchstate : Capteur non présent"
                    End If
                Else
                    retour = "ERR: switch_switchstate : Adaptateur non présent"
                End If
            Catch ex As Exception
                retour = "ERR: switch_switchstate : " & ex.ToString
            End Try
        End SyncLock
        Return retour
    End Function

    Private Function switchs_switchstate(ByVal adresse As String, ByVal channel As Integer) As String
        ' Change l'etat d'un switch
        Dim retour As String = ""
        Dim state As Object
        SyncLock lock_portwrite
            Try
                Dim owd As com.dalsemi.onewire.container.OneWireContainer
                Dim tc As com.dalsemi.onewire.container.SwitchContainer
                Dim switch_state
                If adapter_present Then
                    If wir_adapter.isPresent(adresse) Then
                        wir_adapter.beginExclusive(True)
                        owd = wir_adapter.getDeviceContainer(adresse)
                        tc = DirectCast(owd, com.dalsemi.onewire.container.SwitchContainer)
                        state = tc.readDevice()
                        Dim number_of_switches = tc.getNumberChannels(state)
                        For i = 0 To (number_of_switches - 1)
                            If i = channel Then
                                switch_state = tc.getLatchState(i, state)
                                If i <> 0 Then retour = retour & "-"
                                If switch_state Then retour = retour & "0" Else retour = retour & "1"
                                tc.setLatchState(i, Not switch_state, False, state)
                            End If
                        Next
                        tc.writeDevice(state)
                        wir_adapter.endExclusive()
                    Else
                        retour = "ERR: switchs_switchstate : Capteur non présent"
                    End If
                Else
                    retour = "ERR: switchs_switchstate : Adaptateur non présent"
                End If
            Catch ex As Exception
                retour = "ERR: switchs_switchstate : " & ex.ToString
            End Try
        End SyncLock
        Return retour
    End Function

    Private Function switch_get(ByVal adresse As String) As Integer
        ' Renvoie l'etat du switch 0=fermé, 1=ouvert, 2=fermé mais ouvert entre temps
        Dim retour As Integer = 9999
        Dim state As Object
        SyncLock lock_portwrite
            Try
                'Dim owd As com.dalsemi.onewire.container.OneWireContainer12 'Modif du container suite à remarque Christ. sur TLD
                Dim owd As com.dalsemi.onewire.container.OneWireContainer
                Dim tc As com.dalsemi.onewire.container.SwitchContainer
                Dim switch_state, switch_activity
                If _IsConnect Then
                    If wir_adapter.isPresent(adresse) Then
                        wir_adapter.beginExclusive(True) 'demande l'acces exclusif au reseau
                        owd = wir_adapter.getDeviceContainer(adresse) 'recupere le composant
                        tc = DirectCast(owd, com.dalsemi.onewire.container.SwitchContainer) 'creer la connexion
                        state = tc.readDevice()  'lit les infos du composant
                        switch_state = tc.getLatchState(0, state) 'recup l'etat du switch
                        switch_activity = tc.getSensedActivity(0, state) 'recup l'activité du switch
                        If switch_state Then
                            retour = "1"
                        Else
                            If switch_activity Then retour = "2" Else retour = "0"
                        End If
                        tc.clearActivity()
                        tc.readDevice()
                        wir_adapter.endExclusive() 'rend l'accés au reseau
                    Else
                        retour = 9999
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire switch_get", "Capteur à l'adresse " & adresse & " Non présent")
                    End If
                Else
                    retour = 9999
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire switch_get", "Adaptateur non présent")
                End If
            Catch ex As Exception
                retour = 9999
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire switch_get", "Erreur: " & ex.ToString)
            End Try
        End SyncLock
        Return retour
    End Function

    Private Function switch_setstate(ByVal adresse As String, ByVal etat As Boolean) As String
        ' Change l'etat d'un switch et renvoi le nouvel etat
        Dim retour As String = ""
        Dim state As Object
        SyncLock lock_portwrite
            Try
                Dim owd As com.dalsemi.onewire.container.OneWireContainer
                Dim tc As com.dalsemi.onewire.container.SwitchContainer
                Dim switch_state
                If adapter_present Then
                    If wir_adapter.isPresent(adresse) Then
                        wir_adapter.beginExclusive(True)
                        owd = wir_adapter.getDeviceContainer(adresse)
                        tc = DirectCast(owd, com.dalsemi.onewire.container.SwitchContainer)
                        state = tc.readDevice()
                        switch_state = tc.getLatchState(0, state)
                        If etat Then retour = "1" Else retour = "0"
                        tc.setLatchState(0, etat, False, state)
                        tc.writeDevice(state)
                        wir_adapter.endExclusive()
                    Else
                        retour = 9999
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire switch_set", "Capteur à l'adresse " & adresse & " Non présent")
                    End If
                Else
                    retour = 9999
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire switch_set", "Adaptateur non présent")
                End If
            Catch ex As Exception
                retour = 9999
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire switch_set", "Erreur: " & ex.ToString)
            End Try
        End SyncLock
        Return retour
    End Function

    Private Function switchs_setstate(ByVal adresse As String, ByVal channel As Integer, ByVal etat As Boolean) As String
        ' Change l'etat du channel x du switch Y et renvoi le nouvel etat
        Dim retour As String = ""
        Dim state As Object
        SyncLock lock_portwrite
            Try
                Dim owd As com.dalsemi.onewire.container.OneWireContainer
                Dim tc As com.dalsemi.onewire.container.SwitchContainer
                Dim switch_state
                If adapter_present Then
                    If wir_adapter.isPresent(adresse) Then
                        wir_adapter.beginExclusive(True)
                        owd = wir_adapter.getDeviceContainer(adresse)
                        tc = DirectCast(owd, com.dalsemi.onewire.container.SwitchContainer)
                        state = tc.readDevice()
                        Dim number_of_switches = tc.getNumberChannels(state)
                        For i = 0 To (number_of_switches - 1)
                            If i = channel Then
                                switch_state = tc.getLatchState(i, state)
                                If etat Then retour = "1" Else retour = "0"
                                tc.setLatchState(i, etat, False, state)
                            End If
                        Next
                        tc.writeDevice(state)
                        wir_adapter.endExclusive()
                    Else
                        retour = "ERR: switchs_setstate : Capteur non présent"
                    End If
                Else
                    retour = "ERR: switchs_setstate : Adaptateur non présent"
                End If
            Catch ex As Exception
                retour = "ERR: switchs_setstate : " & ex.ToString
            End Try
        End SyncLock
        Return retour
    End Function

    Private Function switch_clearactivity(ByVal adresse As String) As String
        ' Récupere l'etat et activité d'un switch
        Dim retour As String = ""
        Dim state As Object
        SyncLock lock_portwrite
            Try
                Dim owd As com.dalsemi.onewire.container.OneWireContainer
                Dim tc As com.dalsemi.onewire.container.SwitchContainer
                Dim switch_activity, switch_state
                If adapter_present Then
                    If wir_adapter.isPresent(adresse) Then
                        wir_adapter.beginExclusive(True)
                        owd = wir_adapter.getDeviceContainer(adresse)
                        tc = DirectCast(owd, com.dalsemi.onewire.container.SwitchContainer)
                        state = tc.readDevice()
                        Dim number_of_switches = tc.getNumberChannels(state)
                        For i = 0 To (number_of_switches - 1)
                            switch_state = tc.getLatchState(i, state)
                            switch_activity = tc.getSensedActivity(i, state)
                            If Not switch_state Then
                                retour = "Switch " & i & " => Activité " & switch_activity & " à False"
                                'retour = "0"
                                tc.clearActivity()
                                tc.readDevice()
                            Else
                                retour = "1"
                            End If
                        Next
                        wir_adapter.endExclusive()
                    Else
                        retour = "ERR: switch_clearactivity : Capteur non présent"
                    End If
                Else
                    retour = "ERR: switch_clearactivity : Adaptateur non présent"
                End If
            Catch ex As Exception
                retour = "ERR: switch_clearactivity : " & ex.ToString
            End Try
        End SyncLock
        Return retour
    End Function

    Private Function counter(ByVal adresse As String, ByVal countera As Boolean) As String
        'recupere la valeur du compteur A (true) ou B (false)
        Dim retour As String = ""
        SyncLock lock_portwrite
            Try
                Dim CounterContainer As com.dalsemi.onewire.container.OneWireContainer1D
                Dim owd As com.dalsemi.onewire.container.OneWireContainer
                Dim counterstate As Long
                If _IsConnect Then
                    wir_adapter.beginExclusive(True)
                    'owd = wir_adapter.getDeviceContainer(adresse)
                    'CounterContainer = New com.dalsemi.onewire.container.OneWireContainer1D(wir_adapter, adresse)
                    'If countera Then
                    '    counterstate = CounterContainer.readCounter(14)
                    'Else
                    '    counterstate = CounterContainer.readCounter(15)
                    'End If
                    owd = wir_adapter.getDeviceContainer(adresse)
                    CounterContainer = DirectCast(owd, com.dalsemi.onewire.container.OneWireContainer1D)
                    If countera Then
                        counterstate = CounterContainer.readCounter(14)
                    Else
                        counterstate = CounterContainer.readCounter(15)
                    End If
                    wir_adapter.endExclusive()
                    retour = counterstate.ToString
                Else
                    retour = 9999
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire counter", "ERR: counter : Adaptateur non présent")
                End If
            Catch ex As Exception
                retour = 9999
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "1-Wire counter", "ERR: counter : " & ex.ToString)
            End Try
        End SyncLock
        Return retour
    End Function

#End Region

End Class

