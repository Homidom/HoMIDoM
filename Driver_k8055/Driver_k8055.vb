Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device


''' <summary>Driver Velleman k8055, le device doit dans son adresse 1 indiqué sa carte et son numéro de relais séparé par un x, exemple pour le relais 1 de la carte 1: 1x1</summary>
''' <remarks>Nécessite la dll k8055d.dll</remarks>
<Serializable()> Public Class Driver_k8055
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variable Driver"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "1BB97374-34F5-11E0-BF44-48D8DED72085"
    Dim _Nom As String = "K8055"
    Dim _Enable As String = False
    Dim _Description As String = "Carte Velleman k8055"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "USB"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = ""
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "k8055"
    Dim _Version As String = "1.0"
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

    'A ajouter dans les ppt du driver
    Dim _tempsentrereponse As Integer = 1500
    Dim _ignoreadresse As Boolean = False
    Dim _lastetat As Boolean = True
#End Region

#Region "Declaration"
    Private Declare Function OpenDevice Lib "k8055d.dll" (ByVal CardAddress As Long) As Long
    Private Declare Sub CloseDevice Lib "k8055d.dll" ()
    Private Declare Sub WriteAllDigital Lib "k8055d.dll" (ByVal Data As Long)
    Private Declare Sub ClearDigitalChannel Lib "k8055d.dll" (ByVal Channel As Long)
    Private Declare Sub ClearAllDigital Lib "k8055d.dll" ()
    Private Declare Sub SetDigitalChannel Lib "k8055d.dll" (ByVal Channel As Long)
    Private Declare Sub SetAllDigital Lib "k8055d.dll" ()
    Private Declare Function ReadDigitalChannel Lib "k8055d.dll" (ByVal Channel As Long) As Boolean
    Private Declare Function ReadAllDigital Lib "k8055d.dll" () As Long
    Private Declare Function ReadAnalogChannel Lib "k8055d.dll" (ByVal Channel As Long) As Long
    Private Declare Sub ReadAllAnalog Lib "k8055d.dll" (ByVal Data1 As Long, ByVal Data2 As Long)
    Private Declare Sub OutputAnalogChannel Lib "k8055d.dll" (ByVal Channel As Long, ByVal Data As Long)
    Private Declare Sub OutputAllAnalog Lib "k8055d.dll" (ByVal Data1 As Long, ByVal Data2 As Long)
    Private Declare Sub ClearAnalogChannel Lib "k8055d.dll" (ByVal Channel As Long)
    Private Declare Function ReadCounter Lib "k8055d.dll" (ByVal CounterNr As Long) As Long
    Private Declare Sub SetAllAnalog Lib "k8055d.dll" ()
    Private Declare Sub ClearAllAnalog Lib "k8055d.dll" ()
    Private Declare Sub ResetCounter Lib "k8055d.dll" (ByVal CounterNr As Long)
    Private Declare Sub SetAnalogChannel Lib "k8055d.dll" (ByVal Channel As Long)
    Private Declare Sub SetCounterDebounceTime Lib "k8055d.dll" (ByVal CounterNr As Long, ByVal DebounceTime As Long)
#End Region

#Region "Fonctions génériques"

    Public WriteOnly Property IdSrv As String Implements HoMIDom.HoMIDom.IDriver.IdSrv
        Set(ByVal value As String)
            _idsrv = value
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

    Public Sub Read(ByVal Objet As Object) Implements HoMIDom.HoMIDom.IDriver.Read
        If _Enable = False Then Exit Sub

        Objet.Value = ReadBinaireChannel(Objet.Adresse1)
    End Sub

    Public Property Refresh() As Integer Implements HoMIDom.HoMIDom.IDriver.Refresh
        Get
            Return _Refresh
        End Get
        Set(ByVal value As Integer)
            _Refresh = value
        End Set
    End Property

    Public Sub Restart() Implements HoMIDom.HoMIDom.IDriver.Restart
        [Stop]()
        Start()
    End Sub

    Public Property Server() As HoMIDom.HoMIDom.Server Implements HoMIDom.HoMIDom.IDriver.Server
        Get
            Return _Server
        End Get
        Set(ByVal value As HoMIDom.HoMIDom.Server)
            _Server = value
        End Set
    End Property

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


    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        'cree l'objet
        Try
            Dim h As Long
            Dim carte As Long = 0
            h = OpenDevice(carte)
            Select Case h
                Case 0, 1, 2, 3
                    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "K8055 Start", "Carte " & Str(h) & " connectée")
                    _IsConnect = True
                Case -1
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Start", "Carte " & Str(carte) & " non trouvée")
                    _IsConnect = False
            End Select
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Start", "Erreur lors du démarrage du driver: " & ex.ToString)
            _IsConnect = False
        End Try
    End Sub

    Public Property StartAuto() As Boolean Implements HoMIDom.HoMIDom.IDriver.StartAuto
        Get
            Return _StartAuto
        End Get
        Set(ByVal value As Boolean)
            _StartAuto = value
        End Set
    End Property

    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        'cree l'objet
        Try
            CloseDevice()
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "K8055 Stop", "Driver arrêté")
            _IsConnect = False
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Stop", "Erreur lors de l'arrêt du driver: " & ex.ToString)
            _IsConnect = False
        End Try
    End Sub

    Public ReadOnly Property Version() As String Implements HoMIDom.HoMIDom.IDriver.Version
        Get
            Return _Version
        End Get
    End Property

    Public Sub Write(ByVal Objet As Object, ByVal Commande As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
        Try
            If _Enable = False Then Exit Sub

            If IsNumeric(Objet.Adresse1) = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Write", "Erreur: l'adresse du device " & Objet.Adresse1 & " n'est pas une valeur numérique")
            End If

            Dim adr As Long = Objet.Adresse1

            If Objet.Type = "SWITCH" Or Objet.Type = "APPAREIL" Then
                If Commande = "ON" Then
                    SetBinaireChannel(adr)
                End If
                If Command() = "OFF" Then
                    ClearBinaireChannel(adr)
                End If
            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Write", "Erreur: le type du device " & Objet.Type & " n'est pas reconnu pour ce driver")
            End If

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8055 Write", "Erreur: " & ex.ToString)
        End Try
    End Sub

    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice

    End Sub

    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice

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

    Public Sub New()
        _DeviceSupport.Add(ListeDevices.SWITCH)
        _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN)
        _DeviceSupport.Add(ListeDevices.CONTACT)
        _DeviceSupport.Add(ListeDevices.APPAREIL)

        'ajout des commandes avancées pour les devices
        'Ci-dessous un exemple
        'Dim x As New DeviceCommande
        'x.NameCommand = "Test"
        'x.DescriptionCommand = "Ceci est une commande avancée de test"
        'x.CountParam = 1
        '_DeviceCommandPlus.Add(x)
    End Sub
#End Region

#Region "Fonctions propres au driver"
    Public Sub ClearAllAnalogique()
        'ClearAllAnalog()
    End Sub
    Public Sub ClearAllBinaire()
        'ClearAllDigital()
    End Sub

    Public Sub ClearAnalogiqueChannel(ByVal Channel As Long)
        'ClearAnalogChannel(Channel)
    End Sub

    Public Sub ClearBinaireChannel(ByVal Channel As Long)
        ClearDigitalChannel(Channel)
    End Sub

    Public Sub OutputAllAnalogique(ByVal Data1 As Long, ByVal Data2 As Long)
        'OutputAllAnalog(Data1, Data2)
    End Sub

    Public Sub OutputAnalogiqueChannel(ByVal Channel As Long, ByVal Data As Long)
        'OutputAnalogChannel(Channel, Data)
    End Sub

    Public Sub ReadAllAnalogique(ByVal Data1 As Long, ByVal Data2 As Long)
        'ReadAllAnalog(Data1, Data2)
    End Sub

    Public Function ReadAllBinaire() As Long
        'Return ReadAllDigital
    End Function

    Public Function ReadAnalogiqueChannel(ByVal Channel As Long) As Long
        'Return ReadAnalogChannel(Channel)
    End Function

    Public Function ReadCompter(ByVal CounterNr As Long) As Long
        'Return ReadCounter(CounterNr)
    End Function

    Public Function ReadBinaireChannel(ByVal Channel As Long) As Boolean
        Return ReadDigitalChannel(Channel)
    End Function

    Public Sub ResetCompteur(ByVal CounterNr As Long)
        'ResetCounter(CounterNr)
    End Sub

    Public Sub SetAllAnalogique()
        'SetAllAnalog()
    End Sub

    Public Sub SetAllBinaire()
        'SetAllDigital()
    End Sub

    Public Sub SetAnalogiqueChannel(ByVal Channel As Long)
        'SetAnalogChannel(Channel)
    End Sub

    Public Sub SetCompteurDebounceTime(ByVal CounterNr As Long, ByVal DebounceTime As Long)
        'SetCounterDebounceTime(CounterNr, DebounceTime)
    End Sub

    Public Sub SetBinaireChannel(ByVal Channel As Long)
        SetDigitalChannel(Channel)
    End Sub

    Public Sub WriteAllBinaire(ByVal Channel As Long)
        ' WriteAllDigital(Channel)
    End Sub

    Public Sub [On](ByVal Objet As Object)
        SetBinaireChannel(Objet.adresse)
    End Sub

    Public Sub Off(ByVal Objet As Object)
        ClearBinaireChannel(Objet.adresse)
    End Sub
#End Region

End Class
