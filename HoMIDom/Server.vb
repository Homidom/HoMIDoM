Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization

Namespace HoMIDom

    '***********************************************
    '** CLASS SERVER
    '** version 1.0
    '** Date de création: 12/01/2011
    '** Historique (SebBergues): 12/01/2011: Création 
    '***********************************************

    <Serializable()> Public Class Server
        Inherits MarshalByRefObject
        Implements IHoMIDom 'implémente l'interface dans cette class

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

        Dim _MonRepertoire As String 'représente le répertoire où est située la dll

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
            Log(TypeLog.INFO, TypeSource.SERVEUR, "Initialisation des heures du soleil")
            _HeureCoucherSoleil = DateAdd(DateInterval.Minute, _HeureCoucherSoleilCorrection, dtSunset)
            _HeureLeverSoleil = DateAdd(DateInterval.Minute, _HeureLeverSoleilCorrection, dtSunrise)

            Log(TypeLog.INFO, TypeSource.SERVEUR, "     -> Heure du lever : " & _HeureLeverSoleil)
            Log(TypeLog.INFO, "Serveur", "     -> Heure du coucher : " & _HeureCoucherSoleil)
        End Sub

        '--- Chargement de la config depuis le fichier XML
        Public Function LoadConfig(ByVal Fichier As String) As Server
            Try
                Dim xR As XmlReader = XmlReader.Create(Fichier)
                Dim xS As New XmlSerializer(GetType(Server))
                Return TryCast(xS.Deserialize(xR), Server)
            Catch ex As Exception
                MsgBox("Impossible de charger le fichier de config xml: " & ex.Message)
                Return Nothing
            End Try
        End Function

        '--- Sauvegarde de la config dans le fichier XML
        Public Sub SaveConfig(ByVal Fichier As String, ByVal Objet As Object)
            Try
                'Serialize object to a text file.
                Dim objStreamWriter As New StreamWriter(Fichier)
                Dim x As New XmlSerializer(Objet.GetType)
                x.Serialize(objStreamWriter, Objet)
                objStreamWriter.Close()

                'Log(TypeLog.INFO, TypeSource.SERVEUR, "Sauvegarde de la config sous le fichier " & Fichier)

                ''Copy du fichier de config avant sauvegarde
                'Try
                '    Dim _file As String = Fichier.Replace(".xml", "")
                '    If File.Exists(_file & ".sav") = True Then File.Delete(_file & ".sav")
                '    File.Copy(_file & ".xml", _file & ".sav")
                'Catch ex As Exception
                '    Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Erreur impossible de créer une copie de backup du fichier de config: " & ex.Message)
                'End Try

                ''Creation du fichier XML
                'Dim writer As New XmlTextWriter(Fichier, System.Text.Encoding.UTF8)
                'writer.WriteStartDocument(True)
                'writer.Formatting = Formatting.Indented
                'writer.Indentation = 2

                'writer.WriteStartElement("homidom")

                'Log(TypeLog.INFO, TypeSource.SERVEUR, "Sauvegarde des paramètres serveur")
                ''------------ server
                'writer.WriteStartElement("server")
                'writer.WriteStartElement("longitude")
                'writer.WriteString(_Longitude)
                'writer.WriteEndElement()
                'writer.WriteStartElement("latitude")
                'writer.WriteString(_Latitude)
                'writer.WriteEndElement()
                'writer.WriteStartElement("heurecorrectionlever")
                'writer.WriteString(HeureCorrectionLever)
                'writer.WriteEndElement()
                'writer.WriteStartElement("heurecorrectioncoucher")
                'writer.WriteString(HeureCorrectionCoucher)
                'writer.WriteEndElement()
                'writer.WriteEndElement()

                ''-------------------
                ''------------drivers
                ''------------------
                'Log(TypeLog.INFO, TypeSource.SERVEUR, "Sauvegarde des drivers")
                'For i As Integer = 0 To _ListDrivers.Count - 1
                '    writer.WriteStartElement("driver")
                '    writer.WriteStartAttribute("id")
                '    writer.WriteValue(_ListDrivers.Item(i).ID)
                '    writer.WriteEndAttribute()
                '    writer.WriteStartAttribute("nom")
                '    writer.WriteValue(_ListDrivers.Item(i).Nom)
                '    writer.WriteEndAttribute()
                '    writer.WriteStartAttribute("enable")
                '    writer.WriteValue(_ListDrivers.Item(i).Enable)
                '    writer.WriteEndAttribute()
                '    writer.WriteStartAttribute("description")
                '    writer.WriteValue(_ListDrivers.Item(i).Description)
                '    writer.WriteEndAttribute()
                '    writer.WriteStartAttribute("startauto")
                '    writer.WriteValue(_ListDrivers.Item(i).StartAuto)
                '    writer.WriteEndAttribute()
                '    writer.WriteStartAttribute("protocol")
                '    writer.WriteValue(_ListDrivers.Item(i).Protocol)
                '    writer.WriteEndAttribute()
                '    writer.WriteStartAttribute("iptcp")
                '    writer.WriteValue(_ListDrivers.Item(i).IP_TCP)
                '    writer.WriteEndAttribute()
                '    writer.WriteStartAttribute("porttcp")
                '    writer.WriteValue(_ListDrivers.Item(i).Port_TCP)
                '    writer.WriteEndAttribute()
                '    writer.WriteStartAttribute("ipudp")
                '    writer.WriteValue(_ListDrivers.Item(i).IP_UDP)
                '    writer.WriteEndAttribute()
                '    writer.WriteStartAttribute("portudp")
                '    writer.WriteValue(_ListDrivers.Item(i).Port_UDP)
                '    writer.WriteEndAttribute()
                '    writer.WriteStartAttribute("com")
                '    writer.WriteValue(_ListDrivers.Item(i).Com)
                '    writer.WriteEndAttribute()
                '    writer.WriteStartAttribute("refresh")
                '    writer.WriteValue(_ListDrivers.Item(i).Refresh)
                '    writer.WriteEndAttribute()
                '    writer.WriteStartAttribute("modele")
                '    writer.WriteValue(_ListDrivers.Item(i).modele)
                '    writer.WriteEndAttribute()
                '    writer.WriteStartAttribute("picture")
                '    writer.WriteValue(_ListDrivers.Item(i).Picture)
                '    writer.WriteEndAttribute()
                '    writer.WriteEndElement()
                'Next

                ''------------
                ''Sauvegarde des devices
                ''------------
                'Log(TypeLog.INFO, TypeSource.SERVEUR, "Sauvegarde des devices")
                'writer.WriteStartElement("devices")
                'For i As Integer = 0 To _ListDevices.Count - 1
                '    writer.WriteStartElement("device")
                '    writer.WriteStartElement("typeclass")
                '    writer.WriteString(_ListDevices.Item(i).TypeClass)
                '    writer.WriteEndElement()
                '    writer.WriteStartElement("id")
                '    writer.WriteString(_ListDevices.Item(i).ID)
                '    writer.WriteEndElement()
                '    writer.WriteStartElement("name")
                '    writer.WriteString(_ListDevices.Item(i).Name)
                '    writer.WriteEndElement()
                '    writer.WriteStartElement("adresse")
                '    writer.WriteString(_ListDevices.Item(i).Adresse)
                '    writer.WriteEndElement()
                '    writer.WriteStartElement("enable")
                '    writer.WriteString(_ListDevices.Item(i).Enable)
                '    writer.WriteEndElement()
                '    writer.WriteStartElement("driver")
                '    writer.WriteString(_ListDevices.Item(i).DriverID)
                '    writer.WriteEndElement()
                '    writer.WriteStartElement("image")
                '    writer.WriteString(_ListDevices.Item(i).Picture)
                '    writer.WriteEndElement()
                '    Select Case _ListDevices.Item(i).TypeClass
                '        Case "tv"
                '            writer.WriteStartElement("commands")
                '            For k As Integer = 0 To _ListDevices.Item(i).ListCommandName.Count - 1
                '                writer.WriteStartElement("command")
                '                writer.WriteStartAttribute("key")
                '                writer.WriteValue(_ListDevices.Item(i).ListCommandName(k))
                '                writer.WriteEndAttribute()
                '                writer.WriteStartAttribute("data")
                '                writer.WriteValue(_ListDevices.Item(i).ListCommandData(k))
                '                writer.WriteEndAttribute()
                '                writer.WriteStartAttribute("repeat")
                '                writer.WriteValue(_ListDevices.Item(i).ListCommandRepeat(k))
                '                writer.WriteEndAttribute()
                '                writer.WriteEndElement()
                '            Next
                '            writer.WriteEndElement()
                '    End Select
                '    writer.WriteEndElement()
                'Next
                'writer.WriteEndElement()
                ''------------

                'writer.WriteEndDocument()
                'writer.Close()
                Log(TypeLog.INFO, TypeSource.SERVEUR, "Sauvegarde terminée")
            Catch ex As Exception
                MsgBox("ERREUR SAVECONFIG " & ex.ToString)
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, " Erreur de sauvegarde de la configuration: " & ex.Message)
            End Try

        End Sub

        '--- Charge les drivers
        Public Sub LoadDrivers()
            Try
                Dim tx As String
                Dim dll As Reflection.Assembly
                Dim tp As Type
                Dim Chm As String = "C:\HoMIDom\Applications\Plugins\" 'Emplacement par défaut des plugins


                Dim strFileSize As String = ""
                Dim di As New IO.DirectoryInfo(Chm)
                Dim aryFi As IO.FileInfo() = di.GetFiles("*.dll")
                Dim fi As IO.FileInfo

                'Cherche tous les fichiers dll dans le répertoie plugin
                For Each fi In aryFi
                    'chargement du plugin
                    tx = fi.FullName   'emplacement de la dll
                    'chargement de la dll
                    dll = Reflection.Assembly.LoadFrom(tx)
                    'Vérification de la présence de l'interface recherchée
                    For Each tp In dll.GetTypes
                        If tp.IsClass Then
                            If tp.GetInterface("IDriver", True) IsNot Nothing Then
                                'création de la référence au plugin
                                'Dim obj As Object
                                'obj = AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(tp.FullName, "IDriver")

                                Dim i1 As IDriver
                                i1 = DirectCast(dll.CreateInstance(tp.FullName), IDriver)
                                i1 = CType(i1, IDriver)
                                'i1 = dll.CreateInstance(tp.ToString)
                                i1.Server = Me
                                _ListDrivers.Add(i1)
                                i1.Start()
                            End If
                        End If
                    Next
                Next
            Catch ex As Exception
                MsgBox("Erreur lors du chargement des drivers: " & ex.Message)
            End Try
        End Sub
#End Region

#Region "Interface Client"
        '********************************************************************
        'Fonctions/Sub/Propriétés partagées en service web pour les clients
        '********************************************************************

        '**** PROPRIETES ***************************

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

        Public Property Longitude() As Double Implements IHoMIDom.Longitude
            Get
                Return _Longitude
            End Get
            Set(ByVal value As Double)
                _Longitude = value
            End Set
        End Property

        Public Property Latitude() As Double Implements IHoMIDom.Latitude
            Get
                Return _Latitude
            End Get
            Set(ByVal value As Double)
                _Latitude = value
            End Set
        End Property

        Public Property HeureCorrectionCoucher() As Integer Implements IHoMIDom.HeureCorrectionCoucher
            Get
                Return _HeureCoucherSoleilCorrection
            End Get
            Set(ByVal value As Integer)
                _HeureCoucherSoleilCorrection = value
            End Set
        End Property

        Public Property HeureCorrectionLever() As Integer Implements IHoMIDom.HeureCorrectionLever
            Get
                Return _HeureLeverSoleilCorrection
            End Get
            Set(ByVal value As Integer)
                _HeureLeverSoleilCorrection = value
            End Set
        End Property

        '*** FONCTIONS ******************************************

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
        Public Function SaveDevice(ByVal deviceId As String, ByVal name As String, ByVal address1 As String, ByVal address2 As String, ByVal image As String, ByVal enable As Boolean, ByVal driverId As String, ByVal typeclass As String) As String Implements IHoMIDom.SaveDevice
            Dim myID As String

            If deviceId = "" Then 'C'est un nouveau device
                myID = Api.GenerateGUID
                'Suivant chaque type de device
                Select Case UCase(typeclass)
                    Case "TEMPERATURE"
                        Dim o As New Device.TEMPERATURE(Me)
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .DriverID = driverId
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "HUMIDITE"
                        Dim o As New Device.HUMIDITE(Me)
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .DriverID = driverId
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "BATTERIE"
                        Dim o As New Device.BATTERIE(Me)
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .DriverID = DriverId
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "NIVRECEPTION"
                        Dim o As New Device.NIVRECEPTION(Me)
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .DriverID = driverId
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "TEMPERATURECONSIGNE"
                        Dim o As New Device.TEMPERATURECONSIGNE(Me)
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .DriverID = driverId
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "ENERGIETOTALE"
                        Dim o As New Device.ENERGIETOTALE(Me)
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .DriverID = driverId
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "ENERGIEINSTANTANEE"
                        Dim o As New Device.ENERGIEINSTANTANEE(Me)
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .DriverID = driverId
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "PLUIETOTAL"
                        Dim o As New Device.PLUIETOTAL(Me)
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .DriverID = driverId
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "PLUIECOURANT"
                        Dim o As New Device.PLUIECOURANT(Me)
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .DriverID = driverId
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "VITESSEVENT"
                        Dim o As New Device.VITESSEVENT(Me)
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .DriverID = driverId
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "DIRECTIONVENT"
                        Dim o As New Device.DIRECTIONVENT(Me)
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .DriverID = driverId
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "UV"
                        Dim o As New Device.UV(Me)
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .DriverID = driverId
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "APPAREIL"
                        Dim o As New Device.APPAREIL(Me)
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .DriverID = driverId
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "LAMPE"
                        Dim o As New Device.LAMPE(Me)
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .DriverID = driverId
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "CONTACT"
                        Dim o As New Device.CONTACT(Me)
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .DriverID = driverId
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "METEO"
                        Dim o As New Device.METEO(Me)
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .DriverID = driverId
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "AUDIO"
                        Dim o As New Device.AUDIO(Me)
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .DriverID = driverId
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "MULTIMEDIA"
                        Dim o As New Device.MULTIMEDIA(Me)
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .DriverID = driverId
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "FREEBOX"
                        Dim o As New Device.FREEBOX(Me)
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .DriverID = driverId
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "VOLET"
                        Dim o As New Device.VOLET(Me)
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .DriverID = driverId
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "OBSCURITE"
                        Dim o As New Device.OBSCURITE(Me)
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .DriverID = driverId
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "SWITCH"
                        Dim o As New Device.SWITCH(Me)
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .DriverID = driverId
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        _ListDevices.Add(o)
                    Case "TELECOMMANDE"
                        Dim o As New Device.TELECOMMANDE(Me)
                        With o
                            .ID = myID
                            .Name = name
                            .DateCreated = Now
                            .Picture = image
                            .Adresse1 = address1
                            .Adresse2 = address2
                            .Enable = enable
                            .DriverID = driverId
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
                        _ListDevices.Item(i).driverid = driverId
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
                    'Log.Log(Log.TypeLog.INFO, TypeSource.SERVEUR, "Apprentissage IR: " & retour)
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
            LoadDrivers()
            TimerSecond.Interval = 1000
            AddHandler TimerSecond.Elapsed, AddressOf TimerSecTick
            TimerSecond.Enabled = True
        End Sub
#End Region

#Region "Log"
        Dim _File As String 'Représente le fichier log: ex"C:\log.xml"
        Dim _MaxFileSize As Long = 5120000 'en bytes

        Public Property FichierLog() As String
            Get
                Return _File
            End Get
            Set(ByVal value As String)
                _File = value
            End Set
        End Property

        Public Property MaxFileSize() As Long
            Get
                Return _MaxFileSize
            End Get
            Set(ByVal value As Long)
                _MaxFileSize = value
            End Set
        End Property

        'Indique le type du Log: si c'est une erreur, une info, un message...
        Public Enum TypeLog
            ERREUR
            INFO
            MESSAGE
            DEBUG
        End Enum

        'Indique la source du log si c'est le serveur, un scriopt, un device...
        Public Enum TypeSource
            SERVEUR
            SCRIPT
            TRIGGER
            DEVICE
            DRIVER
        End Enum

        Public Sub Log(ByVal TypLog As TypeLog, ByVal Source As TypeSource, ByVal Message As String)
            Try
                Dim Fichier As FileInfo

                'Vérifie si le fichier log existe sinon le crée
                If File.Exists(_File) = False Then
                    CreateNewFileLog(_File)
                End If

                Fichier = New FileInfo(_File)

                'Vérifie si le fichier est trop gros si oui le supprime
                If Fichier.Length > _MaxFileSize Then
                    File.Delete(_File)
                End If

                Dim xmldoc As New XmlDocument()

                'Ecrire le log
                Try
                    xmldoc.Load(_File) 'ouvre le fichier xml
                    Dim elelog As XmlElement = xmldoc.CreateElement("log") 'création de l'élément log
                    Dim atttime As XmlAttribute = xmldoc.CreateAttribute("time") 'création de l'attribut time
                    Dim atttype As XmlAttribute = xmldoc.CreateAttribute("type") 'création de l'attribut type
                    Dim attsrc As XmlAttribute = xmldoc.CreateAttribute("source") 'création de l'attribut source
                    Dim attmsg As XmlAttribute = xmldoc.CreateAttribute("message") 'création de l'attribut message

                    'on affecte les attributs à l'élément
                    elelog.SetAttributeNode(atttime)
                    elelog.SetAttributeNode(atttype)
                    elelog.SetAttributeNode(attsrc)
                    elelog.SetAttributeNode(attmsg)

                    'on affecte les valeur
                    elelog.SetAttribute("time", Now)
                    elelog.SetAttribute("type", TypLog)
                    elelog.SetAttribute("source", Source)
                    elelog.SetAttribute("message", Message)

                    Dim root As XmlElement = xmldoc.Item("logs")
                    root.AppendChild(elelog)

                    'on enregistre le fichier xml
                    xmldoc.Save(_File)

                Catch ex As Exception

                End Try

                Fichier = Nothing
            Catch ex As Exception

            End Try
        End Sub

        'Créer nouveau Fichier (donner chemin complet et nom) log
        Public Sub CreateNewFileLog(ByVal NewFichier As String)
            Dim rw As XmlTextWriter = New XmlTextWriter(NewFichier, Nothing)
            rw.WriteStartDocument()
            rw.WriteStartElement("logs")
            rw.WriteStartElement("log")
            rw.WriteAttributeString("time", Now)
            rw.WriteAttributeString("type", 0)
            rw.WriteAttributeString("source", 0)
            rw.WriteAttributeString("message", "Création du nouveau fichier log")
            rw.WriteEndElement()
            rw.WriteEndElement()
            rw.WriteEndDocument()
            rw.Close()
        End Sub
#End Region
    End Class

End Namespace