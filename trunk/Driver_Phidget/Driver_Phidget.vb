Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports Phidgets

Public Class Driver_Phidget
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variable Driver"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "10160480-24B7-11E1-ACF1-C9F34824019B"
    Dim _Nom As String = "Phidget InterfaceKit"
    Dim _Enable As String = False
    Dim _Description As String = "PhidgetInterfaceKit 0/16/16"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "USB"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "@"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "PhidgetInterfaceKit 0/16/16"
    Dim _Version As String = "1.0"
    Dim _Picture As String = ""
    Dim _Server As HoMIDom.HoMIDom.Server
    Dim _Device As HoMIDom.HoMIDom.Device
    Dim _DeviceSupport As New ArrayList
    Dim _Parametres As New ArrayList
    Dim MyTimer As New Timers.Timer
    Dim _idsrv As String
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande)

    'A ajouter dans les ppt du driver
    Dim _tempsentrereponse As Integer = 1500
    Dim _ignoreadresse As Boolean = False
    Dim _lastetat As Boolean = True
#End Region

#Region "Declaration"
    Dim WithEvents phidgetIFK As Phidgets.InterfaceKit
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

        ' Objet.Value = ReadBinaireChannel(Objet.Adresse1)
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

    ''' <summary>
    ''' Execute une commande avancée
    ''' </summary>
    ''' <param name="Command"></param>
    ''' <param name="Param"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ExecuteCommand(ByVal Command As String, Optional ByVal Param() As Object = Nothing) As Boolean
        Dim retour As Boolean = False

        If Command = "" Then
            Return False
            Exit Function
        End If

        Select Case UCase(Command)
            Case ""
            Case Else
        End Select

        Return retour
    End Function


    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        'cree l'objet
        Try
            phidgetIFK = New Phidgets.InterfaceKit
            phidgetIFK.open()
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Phidget InterfaceKit Start", "Carte connectée")
            _IsConnect = True
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Phidget InterfaceKit Start", "Erreur lors du démarrage du driver: " & ex.ToString)
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
            'RemoveHandler phidgetIFK.Attach, AddressOf phidgetIFK_Attach
            'RemoveHandler phidgetIFK.Detach, AddressOf phidgetIFK_Detach
            'RemoveHandler phidgetIFK.Error, AddressOf phidgetIFK_Error
            'RemoveHandler phidgetIFK.InputChange, AddressOf phidgetIFK_InputChange
            'RemoveHandler phidgetIFK.OutputChange, AddressOf phidgetIFK_OutputChange
            'RemoveHandler phidgetIFK.SensorChange, AddressOf phidgetIFK_SensorChange

            phidgetIFK.close()
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Phidget InterfaceKit Stop", "Driver arrêté")
            _IsConnect = False
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Phidget InterfaceKit Stop", "Erreur lors de l'arrêt du driver: " & ex.ToString)
            _IsConnect = False
        End Try
    End Sub

    Public ReadOnly Property Version() As String Implements HoMIDom.HoMIDom.IDriver.Version
        Get
            Return _Version
        End Get
    End Property

    Public Sub Write(ByVal Objet As Object, ByVal Commande As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
        If _Enable = False Then Exit Sub
        If _IsConnect = False Then Exit Sub
        Try
            Select Case Objet.Type
                Case 1 Or 12 'APPAREIL Or GENERIQUEBOOLEAN
                    If Commande = "ON" Then
                        phidgetIFK.outputs(Objet.Adresse1) = True
                    End If
                    If Command() = "OFF" Then
                        phidgetIFK.outputs(Objet.Adresse1) = False
                    End If
            End Select
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Phidget InterfaceKit Write", "Erreur: " & ex.ToString)
        End Try
    End Sub

    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice

    End Sub

    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice

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
    'attach event handler... here we'll display the interface kit details as well as determine how many output and input
    'fields to display as well as determine the range of values for the output simulator slider
    Private Sub phidgetIFK_Attach(ByVal sender As Object, ByVal e As Phidgets.Events.AttachEventArgs) Handles phidgetIFK.Attach
        Dim a As String = ""
        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Phidget InterfaceKit Details", "Carte connectée: " & phidgetIFK.Attached.ToString())
        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Phidget InterfaceKit Details", "Nom: " & phidgetIFK.Name)
        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Phidget InterfaceKit Details", "Serial Number: " & sender.SerialNumber.ToString())
        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Phidget InterfaceKit Details", "Version: " & sender.Version.ToString())
        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Phidget InterfaceKit Details", "Nombre d'entrées: " & sender.inputs.Count.ToString())
        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Phidget InterfaceKit Details", "Nombre de sorties: " & phidgetIFK.outputs.Count.ToString())
        '_Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Phidget 0/16/16 Details", "Nombre de capteurs analogique: " & phidgetIFK.sensors.Count.ToString())
    End Sub

    ''ifkit detach event handler... here we display the statu, which will be false as the device is not attached.  We
    ''will also clear the display fields and hide the inputs and outputs.
    Private Sub phidgetIFK_Detach(ByVal sender As Object, ByVal e As Phidgets.Events.DetachEventArgs) Handles phidgetIFK.Detach
        _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Phidget InterfaceKit Details", "Carte connectée: " & phidgetIFK.Attached.ToString())
    End Sub

    Private Sub phidgetIFK_Error(ByVal sender As Object, ByVal e As Phidgets.Events.ErrorEventArgs) Handles phidgetIFK.Error
        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Phidget InterfaceKit Error", "Erreur: " & e.Description)
    End Sub

    'digital input change event handler... here we check or uncheck the corresponding input checkbox based on the index of
    'the digital input that generated the event
    Private Sub phidgetIFK_InputChange(ByVal sender As Object, ByVal e As Phidgets.Events.InputChangeEventArgs) Handles phidgetIFK.InputChange
        traitement(e.Value, e.Index)
        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "Phidget InterfaceKit InputChange", "Entrée N°:" & e.Index & " Value:" & e.Value)
    End Sub

    ''digital output change event handler... here we check or uncheck the corresponding output checkbox based on the index of
    ''the output that generated the event
    Private Sub phidgetIFK_OutputChange(ByVal sender As Object, ByVal e As Phidgets.Events.OutputChangeEventArgs) Handles phidgetIFK.OutputChange
        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "Phidget InterfaceKit OutputChange", "Sortie N°:" & e.Index & " Value:" & e.Value)
    End Sub

    ''' <summary>Traite les paquets reçus</summary>
    ''' <remarks></remarks>
    Private Sub traitement(ByVal valeur As Boolean, ByVal adresse As String)
        Try
            'Recherche si un device affecté
            Dim listedevices As New ArrayList

            listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_idsrv, adresse, "CONTACT", Me._ID, True)
            'un device trouvé on maj la value
            If (listedevices.Count = 1) Then
                'correction valeur pour correspondre au type de value
                If TypeOf listedevices.Item(0).Value Is Integer Then
                    If valeur = True Then
                        valeur = 100
                    ElseIf valeur = False Then
                        valeur = 0
                    End If
                End If

                listedevices.Item(0).Value = valeur

            ElseIf (listedevices.Count > 1) Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Phidget InterfaceKit Process", "Plusieurs devices correspondent à : " & adresse & ":" & valeur)
            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Phidget InterfaceKit Process", "Device non trouvé : " & adresse & ":" & valeur)

            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Phidget InterfaceKit traitement", "Exception : " & ex.Message & " --> " & adresse & " : " & valeur)
        End Try
    End Sub

#End Region

End Class
