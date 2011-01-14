Namespace HoMIDom

    '***********************************************
    '** CLASS SERVER
    '** version 1.0
    '** Date de création: 12/01/2011
    '** Historique (SebBergues): 12/01/2011: Création 
    '***********************************************

    <Serializable()> Public Class Server
        Inherits MarshalByRefObject
        Implements IHoMIDom

#Region "Event"
        '********************************************************************
        'Gestion des Evènements
        '********************************************************************

        'Evenement provenant des drivers
        Public Sub DriversEvent(ByVal DriveName As String, ByVal TypeEvent As String, ByVal Parametre As Object)

        End Sub

        'Evenement provenant des devices
        Public Sub DeviceChange(ByVal Id As String, ByVal [Property] As String, ByVal Parametres As Object)

        End Sub

        'Traitement à effectuer toutes les secondes/minutes/heures/minuit/midi
        Sub TimerSecTick()
            'Action à effectuer toutes les secondes

            'Actions à effectuer toutes les minutes
            If Now.Second = 1 Then

            End If

            'Actions à effectuer toutes les heures
            If Now.Minute = 59 And Now.Second = 59 Then

            End If

            'Actions à effectuer à minuit
            If Now.Hour = 0 And Now.Minute = 0 And Now.Second = 0 Then

            End If

            'Actions à effectuer à midi
            If Now.Hour = 12 And Now.Minute = 0 And Now.Second = 0 Then

            End If
        End Sub

#End Region

#Region "Declaration des variables"
        'Déclaration des variables
        Public Shared WithEvents _ListDrivers As New ArrayList 'Liste des drivers
        Public Shared WithEvents _ListDevices As New ArrayList 'Liste des devices

        Dim Soleil As New Soleil 'Déclaration class Soleil
        Dim _Longitude As Double 'Longitude
        Dim _Latitude As Double 'latitude
        Dim _HeureLeverSoleil As DateTime
        Dim _HeureCoucherSoleil As DateTime
        Dim _HeureLeverSoleilCorrection As Integer
        Dim _HeureCoucherSoleilCorrection As Integer

        Dim TimerSecond As New Timers.Timer 'Timer à la seconde
#End Region

#Region "Fonctions/Sub propres au serveur"

        '---------- Initialisation des heures du soleil -------              
        Public Sub MAJ_HeuresSoleil()

            Dim dtSunrise As Date
            Dim dtSolarNoon As Date
            Dim dtSunset As Date

            Soleil.CalculateSolarTimes(_Latitude, _Longitude, Date.Now, dtSunrise, dtSolarNoon, dtSunset)
            Log.Log(TypeLog.INFO, TypeSource.SERVEUR, "Initialisation des heures du soleil")
            _HeureCoucherSoleil = DateAdd(DateInterval.Minute, _HeureCoucherSoleilCorrection, dtSunset)
            _HeureLeverSoleil = DateAdd(DateInterval.Minute, _HeureLeverSoleilCorrection, dtSunrise)

            Log.Log(Log.TypeLog.INFO, TypeSource.SERVEUR, "     -> Heure du lever : " & _HeureLeverSoleil)
            Log.Log(Log.TypeLog.INFO, "Serveur", "     -> Heure du coucher : " & _HeureCoucherSoleil)
        End Sub
#End Region

#Region "Interface Client"
        '********************************************************************
        'Fonctions/Sub/Propriétés partagées en service web pour les clients
        '********************************************************************

        'PROPRIETES
        Public Property Drivers() As ArrayList Implements IHoMIDom.Drivers
            Get
                Return _ListDrivers
            End Get
            Set(ByVal value As ArrayList)
                _ListDrivers = value
            End Set
        End Property

        Public Property Devices() As ArrayList Implements IHoMIDom.Devices
            Get
                Return _ListDevices
            End Get
            Set(ByVal value As ArrayList)
                _ListDevices = value
            End Set
        End Property

        'FONCTIONS

        'Supprimer un device
        Public Function DeleteDevice(ByVal deviceId As String) As Integer Implements IHoMIDom.DeleteDevice
            For i As Integer = 0 To _ListDevices.Count - 1
                If _ListDevices.Item(i).Id = deviceId Then
                    _ListDevices.Item(i).removeat(i)
                    Exit Function
                End If
            Next
        End Function

        'Supprimer un driver de la config
        Public Function DeleteDriver(ByVal driverId As String) As Integer Implements IHoMIDom.DeleteDriver
            For i As Integer = 0 To _ListDrivers.Count - 1
                If _ListDrivers.Item(i).Id = driverId Then
                    _ListDrivers.Item(i).removeat(i)
                    Exit Function
                End If
            Next
        End Function

        'Retourne l'heure du couché du soleil
        Function HeureCoucherSoleil() As String Implements IHoMIDom.HeureCoucherSoleil
            Return _HeureCoucherSoleil
        End Function

        'Retour l'heure de lever du soleil
        Function HeureLeverSoleil() As String Implements IHoMIDom.HeureLeverSoleil
            Return _HeureLeverSoleil
        End Function

        'Sauvegarder ou créer un device
        Public Function SaveDevice(ByVal deviceId As String, ByVal name As String, ByVal address1 As String, ByVal address2 As String, ByVal image As String, ByVal enable As Boolean, ByVal adapter As String, ByVal typeclass As String) As String Implements IHoMIDom.SaveDevice
            Dim myID As String

            If deviceId = "" Then 'C'est un nouveau device
                myID = Api.GenerateGUID
                'Suivant chaque type de device
                Select Case UCase(typeclass)
                    Case "TEMPERATURE"
                        Dim o As New Device.TEMPERATURE
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "HUMIDITE"
                        Dim o As New Device.HUMIDITE
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "BATTERIE"
                        Dim o As New Device.BATTERIE
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "NIVRECEPTION"
                        Dim o As New Device.NIVRECEPTION
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "TEMPERATURECONSIGNE"
                        Dim o As New Device.TEMPERATURECONSIGNE
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "ENERGIETOTALE"
                        Dim o As New Device.ENERGIETOTALE
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "ENERGIEINSTANTANEE"
                        Dim o As New Device.ENERGIEINSTANTANEE
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "PLUIETOTAL"
                        Dim o As New Device.PLUIETOTAL
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "PLUIECOURANT"
                        Dim o As New Device.PLUIECOURANT
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "VITESSEVENT"
                        Dim o As New Device.VITESSEVENT
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "DIRECTIONVENT"
                        Dim o As New Device.DIRECTIONVENT
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "UV"
                        Dim o As New Device.UV
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "APPAREIL"
                        Dim o As New Device.APPAREIL
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "LAMPE"
                        Dim o As New Device.LAMPE
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "CONTACT"
                        Dim o As New Device.CONTACT
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "METEO"
                        Dim o As New Device.METEO
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "AUDIO"
                        Dim o As New Device.AUDIO
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "MULTIMEDIA"
                        Dim o As New Device.MULTIMEDIA
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "FREEBOX"
                        Dim o As New Device.FREEBOX
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "VOLET"
                        Dim o As New Device.VOLET
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "OBSCURITE"
                        Dim o As New Device.OBSCURITE
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "SWITCH"
                        Dim o As New Device.SWITCH
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "TELECOMMANDE"
                        Dim o As New Device.TELECOMMANDE
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                End Select

            Else 'Device Existant
                myID = deviceId
                For i As Integer = 0 To _ListDevices.Count - 1
                    If _ListDevices.Item(i).ID = deviceId Then
                        _ListDevices.Item(i).name = name
                        _ListDevices.Item(i).adresse1 = address1
                        _ListDevices.Item(i).adresse2 = address2
                        _ListDevices.Item(i).picture = image
                        _ListDevices.Item(i).enable = enable
                        _ListDevices.Item(i).driverid = adapter
                        _ListDevices.Item(i).Driver = ReturnDriverById(adapter)
                    End If
                Next
            End If

            'génration de l'event

            Return myID
        End Function

        'Sauvegarde ou créer un driver dans la config
        Public Function SaveDriver(ByVal driverId As String, ByVal name As String, ByVal enable As Boolean, ByVal startauto As Boolean, ByVal iptcp As String, ByVal porttcp As String, ByVal ipudp As String, ByVal portudp As String, ByVal com As String, ByVal refresh As Integer, ByVal picture As String) As String Implements IHoMIDom.SaveDriver
            Dim myID As String

            If driverId = "" Then 'C'est un nouveau driver
                myID = Api.GenerateGUID
                'Suivant chaque type de driver
                Select Case UCase(name)
                    Case "VIRTUEL"
                        Dim o As New Driver_Virtuel
                        With o
                            .ID = myID
                            .Enable = enable
                            .StartAuto = startauto
                            .IP_TCP = iptcp
                            .Port_TCP = porttcp
                            .IP_UDP = ipudp
                            .Port_UDP = portudp
                            .COM = com
                            .Refresh = refresh
                            .Picture = picture
                            AddHandler o.DriverEvent, AddressOf DriversEvent
                        End With
                        _ListDrivers.Add(o)
                End Select

            Else 'Driver Existant
                myID = driverId
                For i As Integer = 0 To _ListDrivers.Count - 1
                    If _ListDrivers.Item(i).id = driverId Then
                        _ListDrivers.Item(i).Enable = enable
                        _ListDrivers.Item(i).StartAuto = startauto
                        _ListDrivers.Item(i).IP_TCP = iptcp
                        _ListDrivers.Item(i).Port_TCP = porttcp
                        _ListDrivers.Item(i).IP_UDP = ipudp
                        _ListDrivers.Item(i).Port_UDP = portudp
                        _ListDrivers.Item(i).Com = com
                        _ListDrivers.Item(i).Refresh = refresh
                        _ListDrivers.Item(i).Picture = picture
                    End If
                Next
            End If

            'génration de l'event

            Return myID
        End Function

        'Commencer un apprentissage IR
        Public Function StartIrLearning() As String Implements IHoMIDom.StartIrLearning
            Dim retour As String = ""
            For i As Integer = 0 To _ListDrivers.Count - 1
                If _ListDrivers.Item(i).protocol = "IR" Then
                    Dim x As Driver_Usbuirt = _ListDrivers.Item(i)
                    retour = x.LearnCodeIR()
                    Log.Log(Log.TypeLog.INFO, TypeSource.SERVEUR, "Apprentissage IR: " & retour)
                End If
            Next
            Return retour
        End Function

        'retourne le device par son ID
        Public Function ReturnDeviceById(ByVal DeviceId As String) As Object Implements IHoMIDom.ReturnDeviceByID
            Dim retour As Object = Nothing
            For i As Integer = 0 To _ListDevices.Count - 1
                If _ListDevices.Item(i).ID = DeviceId Then
                    retour = _ListDevices.Item(i)
                    Exit For
                End If
            Next
            Return retour
        End Function

        'retourne le driver par son ID
        Public Function ReturnDriverById(ByVal DriverId As String) As Object Implements IHoMIDom.ReturnDriverByID
            Dim retour As Object = Nothing
            For i As Integer = 0 To _ListDrivers.Count - 1
                If _ListDrivers.Item(i).ID = DriverId Then
                    retour = _ListDrivers.Item(i)
                    Exit For
                End If
            Next
            Return retour
        End Function

#End Region

#Region "Declaration de la classe Server"
        Public Sub New()
            TimerSecond.Interval = 1000
            AddHandler TimerSecond.Elapsed, AddressOf TimerSecTick
            TimerSecond.Enabled = True
        End Sub
#End Region

    End Class

End Namespace