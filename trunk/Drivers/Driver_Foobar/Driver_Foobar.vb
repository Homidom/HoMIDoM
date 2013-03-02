Imports HoMIDom
Imports HoMIDom.HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports System.IO
Imports System.Net
Imports System.Web
Imports System.Text
Imports System.Web.HttpUtility

' Driver Foobar multiroom

''' <summary>Class Foobar le device doit donner par le biais de son adresse l'emplacement de l'executable de Foobar</summary>
''' <remarks>Adresse1: Adresse de l'executable foobar sinon la propriété ip_tcp doit être renseigné pour commander via http</remarks>
<Serializable()> Public Class Driver_Foobar
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "9C3E7696-34F7-11E0-AEB0-53DBDED72085"
    Dim _Nom As String = "Foobar"
    Dim _Enable As Boolean = False
    Dim _Description As String = "Multiroom audio Foobar"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "AUDIO"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "@"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "Foobar"
    Dim _Version As String = My.Application.Info.Version.ToString
    Dim _OsPlatform As String = "3264"
    Dim _Picture As String = "audio.png"
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
#End Region

#Region "Variables Internes"

#End Region

#Region "Propriétés génériques"
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
            For i As Integer = 0 To _Server.Devices.Count - 1
                If _Server.Devices.Item(i).Type = "AUDIO" And _Server.Devices.Item(i).Adresse1 <> "" And _Server.Devices.Item(i).Enable = True Then
                    If File.Exists(_Server.Devices.Item(i).Adresse1) Then
                        Dim ProcId As Object = Shell(_Server.Devices.Item(i).Adresse1 & " /hide", AppWinStyle.Hide)
                    End If
                End If
            Next
            _IsConnect = True
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "FOOBAR", "Driver " & Me.Nom & " démarré")
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "FOOBAR Start", ex.Message)
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            For i As Integer = 0 To _Server.Devices.Count - 1
                If _Server.Devices.Item(i).Type = "AUDIO" And _Server.Devices.Item(i).Adresse1 <> "" Then
                    Dim ProcId As Object = Shell(_Server.Devices.Item(i).Adresse1 & " /exit", AppWinStyle.Hide)
                End If
            Next
            _IsConnect = False
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "FOOBAR", "Driver " & Me.Nom & " arrêté")
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "FOOBAR Stop", ex.Message)
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
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "FOOBAR Read", ex.Message)
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
            If _Enable = False Then Exit Sub
            If Objet.type = "AUDIO" Then
                If File.Exists(Objet.adresse1) = False And Objet.adresse2 = "" Then
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "FOOBAR", "Le fichier executable foobar n'existe pas ou l'adresse IP n'est pas définie")
                    Exit Sub
                End If
                Select Case UCase(Commande)
                    Case "EMPTYPLAYLIST"
                        If Objet.adresse2 <> "" Then
                            SendCommandhttp(Objet.Adresse2, "EmptyPlaylist")
                        End If
                    Case "BROWSE"
                        If Objet.adresse2 <> "" Then
                            SendCommandhttp(Objet.Adresse2, "Browse&param1=" & UrlPathEncode(Parametre1))
                        End If
                    Case "START"
                        'Start playback in active playlist.
                        '* param1=playlist item starting from 0. 
                        If Objet.adresse2 <> "" Then
                            SendCommandhttp(Objet.Adresse2, "start&param1=" & Parametre1)
                        End If
                    Case "PLAYAUDIO"
                        If Objet.adresse2 = "" Then
                            If Objet.Fichier = "" Then
                                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "FOOBAR", "Aucun fichier Audio n'a été passé en paramètre (exemple C:\test.mp3)")
                                Exit Sub
                            End If

                            Dim ProcId As Object
                            Dim myfile As String = Objet.fichier
                            If myfile.StartsWith(Chr(34)) = False Then
                                myfile = Chr(34) & myfile
                            End If
                            If myfile.EndsWith(Chr(34)) = False Then
                                myfile = myfile & Chr(34)
                            End If
                            ProcId = Shell(Objet.Adresse1 & " /add " & Objet.Fichier, AppWinStyle.Hide)
                            System.Threading.Thread.Sleep(3000)
                            ProcId = Shell(Objet.Adresse1 & " /play", AppWinStyle.Hide)
                        Else
                            SendCommandhttp(Objet.Adresse2, "PlayOrPause")
                        End If
                        Objet.Value = "PLAY"
                    Case "PAUSEAUDIO"
                        If Objet.adresse2 = "" Then
                            Dim ProcId As Object
                            ProcId = Shell(Objet.Adresse1 & " /pause", AppWinStyle.Hide)
                        Else
                            SendCommandhttp(Objet.Adresse2, "PlayOrPause")
                        End If
                        Objet.Value = "PAUSE"
                    Case "STOPAUDIO"
                        'Stop playback.
                        If Objet.adresse2 = "" Then
                            Dim ProcId As Object
                            ProcId = Shell(Objet.Adresse1 & " /command:Clear", AppWinStyle.Hide)
                            System.Threading.Thread.Sleep(500)
                            ProcId = Shell(Objet.Adresse1 & " /stop", AppWinStyle.Hide)
                        Else
                            SendCommandhttp(Objet.Adresse2, "Stop")
                        End If
                        Objet.Value = "STOP"
                    Case "RANDOMAUDIO"
                        'Start playback of random item in active playlist.
                        If Objet.adresse2 = "" Then
                            Dim ProcId As Object
                            ProcId = Shell(Objet.Adresse1 & " /random", AppWinStyle.Hide)
                        Else
                            SendCommandhttp(Objet.Adresse2, "StartRandom")
                        End If
                        Objet.Value = "RANDOM"
                    Case "NEXTAUDIO"
                        'Start playback of next item in active playlist.
                        If Objet.adresse2 = "" Then
                            Dim ProcId As Object
                            ProcId = Shell(Objet.Adresse1 & " /next", AppWinStyle.Hide)
                        Else
                            SendCommandhttp(Objet.Adresse2, "StartNext")
                        End If
                        Objet.Value = "NEXT"
                    Case "PREVIOUSAUDIO"
                        'Start playback of previous item in active playlist.
                        If Objet.adresse2 = "" Then
                            Dim ProcId As Object
                            ProcId = Shell(Objet.Adresse1 & " /previous", AppWinStyle.Hide)
                        Else
                            SendCommandhttp(Objet.Adresse2, "StartPrevious")
                        End If
                        Objet.Value = "PREVIOUS"
                    Case "VOLUME"
                        'Set volume level, in percent.
                        'param1=volume level, 0...100
                        If Objet.adresse2 <> "" Then
                            Dim param As Integer = Parametre1
                            If param < 0 Then param = 0
                            If param > 100 Then param = 100
                            SendCommandhttp(Objet.Adresse2, "Volume&param1=" & param)
                        End If
                    Case "VOLUMEDB"
                        'Set volume level, in dB.
                        'param1=volume level, 0...665 (0...-66.5 db), or 1000 to mute
                        If Objet.adresse2 <> "" Then
                            Dim param As Integer = Parametre1
                            If param < 0 Then param = 0
                            If param > 1000 Then param = 1000
                            SendCommandhttp(Objet.Adresse2, "VolumeDB&param1=" & param)
                        End If
                    Case "VOLUMEDBDELTA"
                        'Change volume level by given delta, in percent.
                        'param1=signed delta,-N...N
                        If Objet.adresse2 <> "" Then
                            Dim param As Integer = Parametre1
                            If param < -10 Then param = -10
                            If param > 10 Then param = 10
                            SendCommandhttp(Objet.Adresse2, "VolumeDBDelta&param1=" & param)
                        End If
                    Case "SEEK"
                        'Seek playing item, by percent.
                        'param1=position, from 0 to 100
                        If Objet.adresse2 <> "" Then
                            Dim param As Integer = Parametre1
                            If param < 0 Then param = 0
                            If param > 100 Then param = 100
                            SendCommandhttp(Objet.Adresse2, "Seek&param1=" & param)
                        End If
                    Case "SEEKDELTA"
                        'Seek playing item by given delta, in seconds
                        If Objet.adresse2 <> "" Then
                            Dim param As Integer = Parametre1
                            If param < 0 Then param = 0
                            SendCommandhttp(Objet.Adresse2, "SeekDelta&param1=" & param)
                        End If
                    Case "PLAYBACKORDER"
                        'Change playback order (Default, Repeat (Playlist), Repeat (Track), Random, Shuffle (tracks), Shuffle (Albums), Shuffle (Folders)).
                        If Objet.adresse2 <> "" Then
                            Dim param As Integer = Parametre1
                            If param < 0 Then param = 0
                            If param > 7 Then param = 7
                            SendCommandhttp(Objet.Adresse2, "PlaybackOrder&param1=" & param)
                        End If
                    Case "SAC"
                        'Change stop after current flag (if stop after current feautre is enabled in component preferences).
                        '* param1=0 or 1 
                        If Objet.adresse2 <> "" Then
                            Dim param As Integer = Parametre1
                            If param < 0 Then param = 0
                            If param > 1 Then param = 1
                            SendCommandhttp(Objet.Adresse2, "Sac&param1=" & param)
                        End If
                    Case "VOLUMEDOWNAUDIO"
                        If Objet.Adresse1 <> "" Then
                            Dim ProcId As Object
                            ProcId = Shell(Objet.Adresse1 & " /Volume Down", AppWinStyle.Hide)
                            Objet.Value = "VOLUME DOWN"
                        End If
                    Case "VOLUMEUPAUDIO"
                        If Objet.Adresse1 <> "" Then
                            Dim ProcId As Object
                            ProcId = Shell(Objet.Adresse1 & " /Volume Up", AppWinStyle.Hide)
                            Objet.Value = "VOLUME UP"
                        End If
                    Case "VOLUMEMUTEAUDIO"
                        If Objet.adresse2 = "" Then
                            Dim ProcId As Object
                            ProcId = Shell(Objet.Adresse1 & " /Volume mute", AppWinStyle.Hide)
                        Else
                            SendCommandhttp(Objet.Adresse2, "VolumeDB&param1=" & 1000)
                        End If
                        Objet.Value = "VOLUME MUTE"
                    Case "QUEUEITEMS"
                        'Put active playlist items to playback queue.
                        '* param1=item indexes separated by any delitemeter 
                        If Objet.adresse2 <> "" Then
                            SendCommandhttp(Objet.Adresse2, "QueueItems&param1=" & Parametre1)
                        End If
                    Case "QUEUEALBUM"
                        'Queue album.
                        '* param1=item index album search starts from (focused item is used if param1 is omitted)  
                        If Objet.adresse2 <> "" Then
                            SendCommandhttp(Objet.Adresse2, "QueueAlbum&param1=" & Parametre1)
                        End If
                    Case "QUEUERANDOMITEMS"
                        'Queue random items in active playlist.
                        '    * param1=item count
                        If Objet.adresse2 <> "" Then
                            SendCommandhttp(Objet.Adresse2, "QueueRandomItems&param1=" & Parametre1)
                        End If
                    Case "DEQUEUEITEMS"
                        'Remove specified active playlist items from playback queue.
                        '* param1=item indexes separated by any delitemeter 
                        If Objet.adresse2 <> "" Then
                            SendCommandhttp(Objet.Adresse2, "DequeueItems&param1=" & Parametre1)
                        End If
                    Case "FLUSHQUEUE"
                        'Flush queue completely
                        If Objet.adresse2 <> "" Then
                            SendCommandhttp(Objet.Adresse2, "FlushQueue")
                        End If
                    Case "DEL"
                        'Delete one or more playlist items.
                        '* param1=item numbers separated by any delitemeter
                        '* param2=optionally specifies playlist index  
                        If Objet.adresse2 <> "" Then
                            SendCommandhttp(Objet.Adresse2, "Del&param1=" & Parametre1 & "&param2=" & Parametre2)
                        End If
                    Case "UNDO"
                        'Undo changes made to playlist backed up by restore point.
                        '* param1=optionally specifies playlist index 
                        If Objet.adresse2 <> "" Then
                            SendCommandhttp(Objet.Adresse2, "Undo&param1=" & Parametre1)
                        End If
                    Case "MOVE"
                        'Move one or more active playlist items.
                        '* param1=item numbers separated by any delitemeter
                        '* param2=signed move delta  
                        If Objet.adresse2 <> "" Then
                            SendCommandhttp(Objet.Adresse2, "Move&param1=" & Parametre1 & "&param2=" & Parametre2)
                        End If
                    Case "SETSELECTION"
                        'Set playlist selection.
                        '* param1=item numbers separated by any delitemeter, empty to remove selection, ~ to select all
                        '* param2=optionally specifies playlist index 
                        If Objet.adresse2 <> "" Then
                            SendCommandhttp(Objet.Adresse2, "SetSelection&param1=" & Parametre1 & "&param2=" & Parametre2)
                        End If
                    Case "SWITCHPLAYLIST"
                        'Switch playlist.
                        '* param1=playlist index 
                        If Objet.adresse2 <> "" Then
                            SendCommandhttp(Objet.Adresse2, "SwitchPlaylist&param1=" & Parametre1)
                        End If
                    Case "SETFOCUS"
                        'Set focus to specific item of active playlist.
                        '* param1=item index 
                        If Objet.adresse2 <> "" Then
                            SendCommandhttp(Objet.Adresse2, "SetFocus&param1=" & Parametre1)
                        End If
                    Case "REMOVEPLAYLIST"
                        'Removes specified playlist.
                        '* param1=playlist index 
                        If Objet.adresse2 <> "" Then
                            SendCommandhttp(Objet.Adresse2, "RemovePlaylist&param1=" & Parametre1)
                        End If
                    Case Else
                        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "FOOBAR", "Commande inconnue:" & Commande)
                End Select
            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "FOOBAR", "Impossible d'envoyer un code IR pour un type de device autre que AUDIO")
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "FOOBAR Write", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "FOOBAR DeleteDevice", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "FOOBAR NewDevice", ex.Message)
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
            _DeviceSupport.Add(ListeDevices.AUDIO)

            'ajout des commandes avancées pour les devices
            'add_devicecommande("COMMANDE", "DESCRIPTION", nbparametre)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Instance Foobar", "Nom de l'executable à lancer")
            Add_LibelleDevice("ADRESSE2", "Paramètres http", "Adresse http du serveur Foobar si utilisation en mode serveur")
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("MODELE", "@", "")
            Add_LibelleDevice("REFRESH", "@", "")
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "FOOBAR New", ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)

    End Sub

#End Region

#Region "Fonctions internes"
    Private Sub SendCommandhttp(ByVal AdresseIP As String, ByVal Command As String)
        Dim send As String = AdresseIP
        send &= "/default/?cmd=" & Command

        Try
            Dim URL As String = send
            Dim request As WebRequest = WebRequest.Create(URL)
            Dim response As WebResponse = request.GetResponse()
            Dim reader As StreamReader = New StreamReader(response.GetResponseStream())
            Dim str As String = reader.ReadToEnd
            If Len(str) > 255 Then str = Mid(str, 1, 255)
            _Server.Log(Server.TypeLog.DEBUG, Server.TypeSource.DRIVER, "Foobar SendCommandhttp", "Command: " & send & " Return:" & str)
            reader.Close()
        Catch ex As Exception
            _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DRIVER, "Foobar SendCommandhttp", "Erreur lors de l'envoi de la commande: " & send & " - " & ex.ToString)
        End Try
    End Sub
#End Region

End Class
