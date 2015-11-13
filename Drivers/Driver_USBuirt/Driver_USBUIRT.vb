Imports HoMIDom
Imports HoMIDom.HoMIDom.Device
Imports HoMIDom.HoMIDom.Server
Imports UsbUirt

' Auteur : Seb
' Date : 10/02/2011

''' <summary>Class Driver_USBUIRT, permet d'apprendre des codes IR et de les restituer par la suite </summary>
''' <remarks>Nécessite la dll usbuirtmanagedwrapper</remarks>
<Serializable()> Public Class Driver_USBUIRT
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "74FD4E7C-34ED-11E0-8AC4-70CEDED72085"
    Dim _Nom As String = "USBuirt"
    Dim _Enable As Boolean = False
    Dim _Description As String = "Emetteur/Récepteur Infrarouge sur port USB"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "IR"
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
    Dim _IdSrv As String
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande)
    Dim _AutoDiscover As Boolean = False

    'A ajouter dans les ppt du driver
    Dim _tempsentrereponse As Integer = 1500
    Dim _ignoreadresse As Boolean = False
    Dim _lastetat As Boolean = True
#End Region

#Region "Variables internes"
    'variables propres à ce driver
    <NonSerialized()> Dim mc As Controller 'var pour l'usb uirt
    <NonSerialized()> Private learn_code_modifier As LearnCodeModifier = LearnCodeModifier.ForceStruct
    <NonSerialized()> Private code_format As CodeFormat = CodeFormat.Uuirt
    <NonSerialized()> Dim args As LearnCompletedEventArgs = Nothing 'arguments récup lors de l'apprentissage
    <NonSerialized()> Dim Count As Integer = 1
    <NonSerialized()> Dim _CodeFormat As CodeFormat = CodeFormat.Uuirt
    Private last_received_code As String        'dernier code recu

    Public Structure ircodeinfo
        Public code_to_send As String
        Public code_to_receive As String
    End Structure

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
    Public Property StartAuto() As Boolean Implements HoMIDom.HoMIDom.IDriver.StartAuto
        Get
            Return _StartAuto
        End Get
        Set(ByVal value As Boolean)
            _StartAuto = value
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
            Count = _Parametres.Item(0).Valeur
            If IsNumeric(_Parametres.Item(0).Valeur) Then
                If CInt(_Parametres.Item(0).Valeur) = 0 Then
                    _CodeFormat = UsbUirt.CodeFormat.Uuirt
                End If
                If CInt(_Parametres.Item(0).Valeur) = 1 Then
                    _CodeFormat = UsbUirt.CodeFormat.Pronto
                End If
            End If
            Me.mc = New Controller
            'capte les events
            AddHandler mc.Received, AddressOf handler_mc_received

            _IsConnect = True
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "USBUIRT", "Driver démarré")
        Catch ex As Exception
            _IsConnect = False
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "USBUirt Start", "Driver erreur lors du démarrage: " & ex.Message)
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            Me.mc = Nothing
            _IsConnect = False
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "USBUIRT", "Driver arrêté")
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "USBUirt Stop", ex.Message)
        End Try
    End Sub

    ''' <summary>Re-Démarrer le driver</summary>
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
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "USBUirt Read", ex.Message)
        End Try
    End Sub

    ''' <summary>Commander un device</summary>
    ''' <param name="Objet">Objet représetant le device à interroger</param>
    ''' <param name="Commande">La commande à passer</param>
    ''' <param name="Parametre1"></param>
    ''' <param name="Parametre2"></param>
    ''' <remarks></remarks>
    Public Sub Write(ByVal Objet As Object, ByVal Commande As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
        Try
            If _Enable = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "USBUIRT", "Le Driver n'est pas activé, impossible de traiter cette action")
                Exit Sub
            End If

            If _IsConnect = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "USBUIRT", "Le Driver n'est pas connecté, impossible de traiter cette action")
                Exit Sub
            End If


            If Objet.type = "MULTIMEDIA" Then
                If Commande = "SendCodeIR" Then
                    SendCodeIR(Parametre1, Parametre2)
                Else
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "USBUIRT", "La commande " & Commande & " est inconnue pour ce driver")
                End If
            Else
                If Commande = "ON" And String.IsNullOrEmpty(Objet.Adresse1) = False Then
                    If String.IsNullOrEmpty(Objet.Adresse1) Then
                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "USBUIRT", "La trame correspondant à ON est vide pour le composant " & Objet.Name)
                    Else
                        Count = _Parametres.Item(0).Valeur
                        If IsNumeric(_Parametres.Item(1).Valeur) Then
                            If CInt(_Parametres.Item(1).Valeur) = 0 Then
                                _CodeFormat = UsbUirt.CodeFormat.Uuirt
                            End If
                            If CInt(_Parametres.Item(1).Valeur) = 1 Then
                                _CodeFormat = UsbUirt.CodeFormat.Pronto
                            End If
                        End If
                        SendCodeIR(Objet.Adresse1, Count)
                        Objet.Value = 100
                        Exit Sub
                    End If
                End If
                If Commande = "OFF" Then
                    If String.IsNullOrEmpty(Objet.Adresse2) Then
                        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "USBUIRT", "La trame correspondant à OFF est vide pour le composant " & Objet.Name)
                    Else
                        Count = _Parametres.Item(0).Valeur
                        If IsNumeric(_Parametres.Item(1).Valeur) Then
                            If CInt(_Parametres.Item(1).Valeur) = 0 Then
                                _CodeFormat = UsbUirt.CodeFormat.Uuirt
                            End If
                            If CInt(_Parametres.Item(1).Valeur) = 1 Then
                                _CodeFormat = UsbUirt.CodeFormat.Pronto
                            End If
                        End If
                        SendCodeIR(Objet.Adresse2, Count)
                        Objet.Value = 0
                        Exit Sub
                    End If
                End If
                If Commande = "OUVERTURE" Then
                    If Parametre1 = 0 Then
                        If String.IsNullOrEmpty(Objet.Adresse2) Then
                            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "USBUIRT", "La trame correspondant à OFF est vide pour le composant " & Objet.Name)
                        Else
                            Count = _Parametres.Item(0).Valeur
                            If IsNumeric(_Parametres.Item(1).Valeur) Then
                                If CInt(_Parametres.Item(1).Valeur) = 0 Then
                                    _CodeFormat = UsbUirt.CodeFormat.Uuirt
                                End If
                                If CInt(_Parametres.Item(1).Valeur) = 1 Then
                                    _CodeFormat = UsbUirt.CodeFormat.Pronto
                                End If
                            End If
                            SendCodeIR(Objet.Adresse2, Count)
                            Objet.Value = 0
                            Exit Sub
                        End If
                    Else
                        If String.IsNullOrEmpty(Objet.Adresse1) Then
                            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "USBUIRT", "La trame correspondant à ON est vide pour le composant " & Objet.Name)
                        Else
                            Count = _Parametres.Item(0).Valeur
                            If IsNumeric(_Parametres.Item(1).Valeur) Then
                                If CInt(_Parametres.Item(1).Valeur) = 0 Then
                                    _CodeFormat = UsbUirt.CodeFormat.Uuirt
                                End If
                                If CInt(_Parametres.Item(1).Valeur) = 1 Then
                                    _CodeFormat = UsbUirt.CodeFormat.Pronto
                                End If
                            End If
                            SendCodeIR(Objet.Adresse1, Count)
                            Objet.Value = 100
                            Exit Sub
                        End If
                    End If
                End If
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "USBUIRT", "Impossible d'envoyer un code IR pour un type de device autre que MULTIMEDIA")
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "USBUirt Write", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "USBUirt DeleteDevice", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "USBUirt NewDevice", ex.Message)
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
            _DeviceSupport.Add(ListeDevices.MULTIMEDIA)
            _DeviceSupport.Add(ListeDevices.SWITCH)
            _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN)
            _DeviceSupport.Add(ListeDevices.CONTACT)
            _DeviceSupport.Add(ListeDevices.APPAREIL)
            _DeviceSupport.Add(ListeDevices.LAMPE)
            _DeviceSupport.Add(ListeDevices.VOLET)

            'Parametres avancés
            Add_ParamAvance("Nombre d'envoi de la trame", "Nombre de fois à envoyer la trame", 1)
            Add_ParamAvance("Format (Uuirt/Pronto)", "Format trame (Uuirt=0/Pronto=1)", 0)

            Add_LibelleDevice("ADRESSE1", "Trame ON", "Trame reçue ou envoyée si ON ou simulation appuie sur bouton télécommande")
            Add_LibelleDevice("ADRESSE2", "Trame OFF", "Trame reçue ou envoyée si OFF")
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("MODELE", "@", "")
            'Add_LibelleDevice("REFRESH", "Refresh (sec)", "Valeur de rafraîchissement de la mesure en secondes")
            'Add_LibelleDevice("LASTCHANGEDUREE", "LastChange Durée", "")
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "USBUirt New", ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)

    End Sub

#End Region

#Region "Fonctions internes"
    ''' <summary>Apprendre un code IR</summary>
    ''' <returns>Retourne le code IR</returns>
    ''' <remarks></remarks>
    Public Function LearnCode() As String
        If _IsConnect = False Then
            Return "ERREUR: Impossible d'apprendre le code IR le driver n'est pas connecté"
        Else
            _FlagFinish = False
            Dim x As ircodeinfo = Nothing
            x = wait_for_code()
            If String.IsNullOrEmpty(x.code_to_send) Then
                Return String.Empty
            Else
                Dim f As Integer = 0
                If _CodeFormat = CodeFormat.Pronto Then f = 1
                If _CodeFormat = CodeFormat.Uuirt Then f = 0
                Return "c=" & x.code_to_send & ";r=" & Count & ";f=" & f
            End If
        End If
    End Function

    Dim _FlagFinish As Boolean = False

    'boucle qui attend kon recoive
    <System.STAThread()> _
    Private Function wait_for_code() As ircodeinfo

        'handler
        AddHandler mc.Learning, AddressOf handler_mc_learning
        AddHandler mc.LearnCompleted, AddressOf handler_mc_learning_completed
        Me.args = Nothing

        'lance l'apprentissage
        Try
            Try
                Me.mc.LearnAsync(Me._CodeFormat, Me.learn_code_modifier, Me.args)
                'Me.mc.Learn(Me.code_format, Me.learn_code_modifier, TimeSpan.Zero)
            Catch ex As Exception
                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "USBUIRT", "Erreur lors de l'apprentissage:" & ex.Message)
                Return Nothing
            End Try

            'attend que ce soit appris
            Dim timeout As Date = Now.AddSeconds(15)

            Do While IsNothing(Me.args) And timeout > Now And _FlagFinish = False
                Threading.Thread.Sleep(500)
            Loop

            'retourne le code
            wait_for_code.code_to_send = Me.args.Code
            wait_for_code.code_to_receive = last_received_code

            'c appris ou non !!!
            RemoveHandler mc.Learning, AddressOf handler_mc_learning
            RemoveHandler mc.LearnCompleted, AddressOf handler_mc_learning_completed

            Return wait_for_code
        Catch ex As Exception
            'c appris ou non !!!
            RemoveHandler mc.Learning, AddressOf handler_mc_learning
            RemoveHandler mc.LearnCompleted, AddressOf handler_mc_learning_completed

            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "USBUIRT", "Erreur lors de l'apprentissage:" & ex.Message)
            Return Nothing
        End Try

        ''retourne le code 17/03/2015
        'wait_for_code.code_to_send = Me.args.Code
        'wait_for_code.code_to_receive = last_received_code
        'Return wait_for_code
    End Function

    ''' <summary>Emet un code infrarouge</summary>
    ''' <param name="ir_code"></param>
    ''' <param name="RepeatCount"></param>
    ''' <remarks></remarks>
    Public Sub SendCodeIR(ByVal ir_code As String, ByVal RepeatCount As Integer)
        Try
            Dim _format As String = ""
            Dim _tabl() As String = ir_code.Split(";") 'c=<Code>;r=<REPEAT>;f=<Format>
            Dim _ir_code As String = ir_code

            If _tabl IsNot Nothing Then
                If _tabl.Length > 0 Then
                    For i = 0 To _tabl.Length - 1
                        'variable code
                        If _tabl(i).ToLower.StartsWith("c=") Then
                            Dim _var As String = Mid(_tabl(i), 3, Len(_tabl(i)) - 2)
                            If String.IsNullOrEmpty(_var) = False Then
                                _ir_code = _var
                            End If
                        End If
                        'variable repeat
                        If _tabl(i).ToLower.StartsWith("r=") Then
                            Dim _var As String = Mid(_tabl(i), 3, Len(_tabl(i)) - 2)
                            If IsNumeric(_var) Then
                                RepeatCount = CInt(_var)
                                If RepeatCount < 0 Then RepeatCount = 0
                            End If
                        End If
                        'variable format
                        If _tabl(i).ToLower.StartsWith("f=") Then
                            Dim _var As String = Mid(_tabl(i), 3, Len(_tabl(i)) - 2)
                            If IsNumeric(_var) Then
                                If CInt(_var) = 0 Then
                                    _CodeFormat = UsbUirt.CodeFormat.Uuirt
                                End If
                                If CInt(_var) = 1 Then
                                    _CodeFormat = UsbUirt.CodeFormat.Pronto
                                End If
                            End If
                        End If
                    Next
                End If
            End If

            If _CodeFormat = CodeFormat.Uuirt Then _format = "Uuirt"
            If _CodeFormat = CodeFormat.Pronto Then _format = "Pronto"

            mc.Transmit(_ir_code, _CodeFormat, RepeatCount, TimeSpan.Zero)
            _Server.Log(TypeLog.MESSAGE, TypeSource.DRIVER, "USBUIRT", "Code IR envoyé: " & _ir_code & " Repeat: " & RepeatCount & " Format:" & _format & " FromTrame:" & ir_code)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "USBUIRT", "Problème de transmission: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Envoyer une commande IR
    ''' </summary>
    ''' <param name="Code"></param>
    ''' <param name="Repeat"></param>
    ''' <remarks></remarks>
    Public Sub EnvoyerCode(ByVal Code As String, ByVal Repeat As Integer, Optional Format As Integer = 99)
        Try
            If _Enable = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " EnvoyerCommande", "Erreur: Impossible de traiter la commande car le driver n'est pas activé (Enable)")
                Exit Sub
            End If

            If _IsConnect = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " EnvoyerCommande", "Erreur: Impossible de traiter la commande car le driver n'est pas connecté")
                Exit Sub
            End If

            Dim _myformat As CodeFormat = _CodeFormat

            If Format = 0 Then
                _myformat = CodeFormat.Uuirt
            End If
            If Format = 1 Then
                _myformat = CodeFormat.Pronto
            End If

            If Repeat <= 0 Then Repeat = 1

            mc.Transmit(Code, _myformat, Repeat, TimeSpan.Zero)
            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " EnvoyerCommande", "Code IR envoyé: " & Code & " repeat: " & Repeat & " Format:" & _myformat.ToString)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " EnvoyerCommande", "Erreur: " & ex.ToString & vbCrLf & "Code=" & Code & "  Repeat=" & Repeat)
        End Try
    End Sub

    'handler code recu
    Private Sub handler_mc_received(ByVal sender As Object, ByVal e As ReceivedEventArgs)
        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "USBUIRT", "Code IR reçu: " & e.IRCode)

        If String.IsNullOrEmpty(e.IRCode) = False Then
            Try
                'Recherche si un device affecté pour Adresse 1
                Dim listedevices As New ArrayList
                listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_IdSrv, e.IRCode, "", Me._ID, True)
                'un device trouvé on maj la value
                If listedevices IsNot Nothing Then
                    If (listedevices.Count = 1) Then
                        'correction valeur pour correspondre au type de value
                        If TypeOf listedevices.Item(0).Value Is Integer Then
                            listedevices.Item(0).Value = 100
                            Exit Sub
                        End If
                        If TypeOf listedevices.Item(0).Value Is Boolean Then
                            listedevices.Item(0).Value = True
                            Exit Sub
                        End If
                    End If
                End If

                listedevices = New ArrayList
                listedevices = _Server.ReturnDeviceByAdresse2TypeDriver(_IdSrv, e.IRCode, "", Me._ID, True)
                'un device trouvé on maj la value
                If listedevices IsNot Nothing Then
                    If (listedevices.Count = 1) Then
                        'correction valeur pour correspondre au type de value
                        If TypeOf listedevices.Item(0).Value Is Integer Then
                            listedevices.Item(0).Value = 0
                            Exit Sub
                        End If
                        If TypeOf listedevices.Item(0).Value Is Boolean Then
                            listedevices.Item(0).Value = False
                            Exit Sub
                        End If
                    End If
                End If

                listedevices = New ArrayList
                listedevices = _Server.ReturnDeviceByDriver(_IdSrv, Me._ID, True)

                If listedevices IsNot Nothing Then
                    For Each _dev In listedevices
                        If _dev.Adresse1.ToString.Contains(e.IRCode) Then 'un device trouvé via adresse1 on maj la value
                            'correction valeur pour correspondre au type de value
                            If TypeOf _dev.Value Is Integer Then
                                _dev.Value = 100
                            End If
                            If TypeOf _dev.Value Is Boolean Then
                                _dev.Value = True
                            End If
                        End If
                        If _dev.Adresse2.ToString.Contains(e.IRCode) Then 'un device trouvé via adresse 2 on maj la value
                            'correction valeur pour correspondre au type de value
                            If TypeOf _dev.Value Is Integer Then
                                _dev.Value = 0
                            End If
                            If TypeOf _dev.Value Is Boolean Then
                                _dev.Value = False
                            End If
                        End If
                    Next
                End If

            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " handler_mc_received", "Erreur : " & ex.ToString)
        End Try
        End If


        last_received_code = e.IRCode
        RaiseEvent DriverEvent(_Nom, "CODE_RECU", e.IRCode)
    End Sub

    'handler en apprentissage
    Private Sub handler_mc_learning(ByVal sender As Object, ByVal e As LearningEventArgs)
        Try
            If e.Progress >= 100 Then
                _FlagFinish = True
            End If
            '_Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "USBUIRT", "Learning: " & e.Progress & " freq=" & e.CarrierFrequency & " quality=" & e.SignalQuality)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " handler_mc_learning", "Erreur : " & ex.ToString)
        End Try
    End Sub

    'handler a appris
    Private Sub handler_mc_learning_completed(ByVal sender As Object, ByVal e As LearnCompletedEventArgs)
        Try
            args = e
            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "USBUIRT", "Learning completed: " & e.Code)
            _FlagFinish = True
            'RaiseEvent DriverEvent(_Nom, "LEARN_TERMINE", e.Code)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " handler_mc_learning_completed", "Erreur : " & ex.ToString)
        End Try

    End Sub
#End Region

End Class
