Imports System.IO
Imports System.Xml
Imports System.Xml.XPath
Imports System.Xml.Serialization
Imports System.Reflection

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
                MAJ_HeuresSoleil()
            End If

            'Actions à effectuer à midi
            If Now.Hour = 12 And Now.Minute = 0 And Now.Second = 0 Then
                MAJ_HeuresSoleil()
            End If
        End Sub

#End Region

#Region "Declaration des variables"
        'Déclaration des variables
        Private Shared WithEvents _ListDrivers As New ArrayList 'Liste des drivers
        Private Shared _ListImgDrivers As New ArrayList
        Public Shared WithEvents _ListDevices As New ArrayList 'Liste des devices

        'Application.StartupPath
        Public _MonRepertoire As String = System.Environment.CurrentDirectory 'représente le répertoire de l'application
        'Public _MonRepertoire As String = "C:\Homidom\" ' System.Reflection.Assembly.GetExecutingAssembly.Location.ToString()

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
            Log(TypeLog.INFO, TypeSource.SERVEUR, "MAJ_HeuresSoleil", "Initialisation des heures du soleil")
            _HeureCoucherSoleil = DateAdd(DateInterval.Minute, _HeureCoucherSoleilCorrection, dtSunset)
            _HeureLeverSoleil = DateAdd(DateInterval.Minute, _HeureLeverSoleilCorrection, dtSunrise)

            Log(TypeLog.INFO, TypeSource.SERVEUR, "MAJ_HeuresSoleil", "Heure du lever : " & _HeureLeverSoleil)
            Log(TypeLog.INFO, TypeSource.SERVEUR, "MAJ_HeuresSoleil", "Heure du coucher : " & _HeureCoucherSoleil)
        End Sub

        '--- Chargement de la config depuis le fichier XML
        Public Function LoadConfig(ByVal Fichier As String) As String
            'Copy du fichier de config avant chargement
            Try
                Dim _file As String = Fichier & "homidom"
                If File.Exists(_file & ".bak") = True Then File.Delete(_file & ".bak")
                File.Copy(_file & ".xml", Mid(_file & ".xml", 1, Len(_file & ".xml") - 4) & ".bak")
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "LoadConfig", "Erreur impossible de créer une copie de backup du fichier de config: " & ex.Message)
            End Try

            Try
                Dim dirInfo As New System.IO.DirectoryInfo(Fichier)
                Dim file As System.IO.FileInfo
                Dim files() As System.IO.FileInfo = dirInfo.GetFiles("homidom.xml")
                Dim myxml As Xml

                If (files IsNot Nothing) Then
                    For Each file In files
                        Dim myfile As String = file.FullName
                        Dim list As XmlNodeList

                        myxml = New Xml(myfile)

                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Chargement du fichier config: " & myfile)

                        '******************************************
                        'on va chercher les paramètres du serveur
                        '******************************************
                        list = myxml.SelectNodes("/homidom/server")
                        If list.Count > 0 Then 'présence des paramètres du server
                            For j As Integer = 0 To list.Item(0).Attributes.Count - 1
                                Select Case list.Item(0).Attributes.Item(j).Name
                                    Case "longitude"
                                        _Longitude = list.Item(0).Attributes.Item(j).Value
                                    Case "latitude"
                                        _Latitude = list.Item(0).Attributes.Item(j).Value
                                    Case "heurecorrectionlever"
                                        _HeureLeverSoleilCorrection = list.Item(0).Attributes.Item(j).Value
                                    Case "heurecorrectioncoucher"
                                        _HeureCoucherSoleilCorrection = list.Item(0).Attributes.Item(j).Value
                                    Case Else
                                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Un attribut correspondant au serveur est inconnu: nom:" & list.Item(0).Attributes.Item(j).Name & " Valeur: " & list.Item(0).Attributes.Item(j).Value)
                                End Select
                            Next
                        Else
                            MsgBox("Il manque les paramètres du serveur dans le fichier de config !!")
                        End If
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Paramètres du serveur chargés")

                        '********************************
                        'on va chercher les drivers
                        '*********************************
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Chargement des drivers")
                        list = Nothing
                        list = myxml.SelectNodes("/homidom/drivers/driver")

                        If list.Count > 0 Then 'présence d'un ou des driver(s)
                            For j As Integer = 0 To list.Count - 1
                                'on récupère l'id du driver
                                Dim _IdDriver = list.Item(j).Attributes.Item(0).Value
                                Dim _drv As IDriver = ReturnDriverById(_IdDriver)

                                If _drv IsNot Nothing Then
                                    _drv.Enable = list.Item(j).Attributes.GetNamedItem("enable").Value
                                    _drv.StartAuto = list.Item(j).Attributes.GetNamedItem("startauto").Value
                                    _drv.IP_TCP = list.Item(j).Attributes.GetNamedItem("iptcp").Value
                                    _drv.Port_TCP = list.Item(j).Attributes.GetNamedItem("porttcp").Value
                                    _drv.IP_UDP = list.Item(j).Attributes.GetNamedItem("ipudp").Value
                                    _drv.Port_UDP = list.Item(j).Attributes.GetNamedItem("portudp").Value
                                    _drv.COM = list.Item(j).Attributes.GetNamedItem("com").Value
                                    _drv.Refresh = list.Item(j).Attributes.GetNamedItem("refresh").Value
                                    _drv.Picture = list.Item(j).Attributes.GetNamedItem("picture").Value
                                    Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Driver " & _drv.Nom & " chargé")
                                    _drv = Nothing
                                End If
                            Next
                        Else
                            Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Aucun driver n'est enregistré dans le fichier de config")
                        End If

                        '******************************************
                        'on va chercher les devices
                        '********************************************
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Chargement des devices")
                        list = Nothing
                        list = myxml.SelectNodes("/homidom/devices/device")

                        If list.Count > 0 Then 'présence d'un device
                            For j As Integer = 0 To list.Count - 1
                                Dim _Dev As Object = Nothing

                                'Suivant chaque type de device
                                Select Case UCase(list.Item(j).Attributes.GetNamedItem("type").Value)
                                    Case "APPAREIL"
                                        Dim o As New Device.APPAREIL(Me)
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "AUDIO"
                                        Dim o As New Device.AUDIO(Me)
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "BATTERIE"
                                        Dim o As New Device.BATTERIE(Me)
                                        _Dev = o
                                        o = Nothing
                                    Case "CONTACT"
                                        Dim o As New Device.CONTACT(Me)
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "DIRECTIONVENT"
                                        Dim o As New Device.DIRECTIONVENT(Me)
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "ENERGIEINSTANTANEE"
                                        Dim o As New Device.ENERGIEINSTANTANEE(Me)
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "ENERGIETOTALE"
                                        Dim o As New Device.ENERGIETOTALE(Me)
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "FREEBOX"
                                        Dim o As New Device.FREEBOX(Me)
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "HUMIDITE"
                                        Dim o As New Device.HUMIDITE(Me)
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "LAMPE"
                                        Dim o As New Device.LAMPE(Me)
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "METEO"
                                        Dim o As New Device.METEO(Me)
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "MULTIMEDIA"
                                        Dim o As New Device.MULTIMEDIA(Me)
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "NIVRECEPTION"
                                        Dim o As New Device.NIVRECEPTION(Me)
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "OBSCURITE"
                                        Dim o As New Device.OBSCURITE(Me)
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "PLUIECOURANT"
                                        Dim o As New Device.PLUIECOURANT(Me)
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "PLUIETOTAL"
                                        Dim o As New Device.PLUIETOTAL(Me)
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "SWITCH"
                                        Dim o As New Device.SWITCH(Me)
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "TELECOMMANDE"
                                        Dim o As New Device.TELECOMMANDE(Me)
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "TEMPERATURE"
                                        Dim o As New Device.TEMPERATURE(Me)
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "TEMPERATURECONSIGNE"
                                        Dim o As New Device.TEMPERATURECONSIGNE(Me)
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "UV"
                                        Dim o As New Device.UV(Me)
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "VITESSEVENT"
                                        Dim o As New Device.VITESSEVENT(Me)
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "VOLET"
                                        Dim o As New Device.VOLET(Me)
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                End Select

                                With _Dev
                                    'Affectation des valeurs sur les propriétés génériques
                                    .ID = list.Item(j).Attributes.GetNamedItem("id").Value
                                    .Name = list.Item(j).Attributes.GetNamedItem("name").Value
                                    .Enable = list.Item(j).Attributes.GetNamedItem("enable").Value
                                    .DriverId = list.Item(j).Attributes.GetNamedItem("driverid").Value
                                    .Description = list.Item(j).Attributes.GetNamedItem("description").Value
                                    .Adresse1 = list.Item(j).Attributes.GetNamedItem("adresse1").Value
                                    .Adresse2 = list.Item(j).Attributes.GetNamedItem("adresse2").Value
                                    .DateCreated = list.Item(j).Attributes.GetNamedItem("datecreated").Value
                                    .LastChange = list.Item(j).Attributes.GetNamedItem("lastchange").Value
                                    .Refresh = list.Item(j).Attributes.GetNamedItem("refresh").Value
                                    .Modele = list.Item(j).Attributes.GetNamedItem("modele").Value
                                    .Picture = list.Item(j).Attributes.GetNamedItem("picture").Value
                                    .Solo = list.Item(j).Attributes.GetNamedItem("solo").Value
                                    '-- propriétés generique value --
                                    If _Dev.Type = "TEMPERATURE" _
                                    Or _Dev.Type = "HUMIDITE" _
                                    Or _Dev.Type = "TEMPERATURECONSIGNE" _
                                    Or _Dev.Type = "ENERGIETOTALE" _
                                    Or _Dev.Type = "ENERGIEINSTANTANEE" _
                                    Or _Dev.Type = "PLUIETOTAL" _
                                    Or _Dev.Type = "PLUIECOURANT" _
                                    Or _Dev.Type = "VITESSEVENT" _
                                    Or _Dev.Type = "UV" _
                                    Or _Dev.Type = "HUMIDITE" _
                                    Then
                                        .Value = list.Item(j).Attributes.GetNamedItem("value").Value
                                        .ValueMin = list.Item(j).Attributes.GetNamedItem("valuemin").Value
                                        .ValueMax = list.Item(j).Attributes.GetNamedItem("valuemax").Value
                                        .ValueDef = list.Item(j).Attributes.GetNamedItem("valuedef").Value
                                        .Precision = list.Item(j).Attributes.GetNamedItem("precision").Value
                                        .Correction = list.Item(j).Attributes.GetNamedItem("correction").Value
                                        .Formatage = list.Item(j).Attributes.GetNamedItem("formatage").Value
                                    End If
                                    '-- cas spécifique du multimedia pour récupérer les commandes IR --
                                    If _Dev.type = "MULTIMEDIA" Then
                                        For k As Integer = 0 To list.Item(j).ChildNodes.Count - 1
                                            If list.Item(j).ChildNodes.Item(k).Name = "commands" Then
                                                _Dev.ListCommandName.Clear()
                                                _Dev.ListCommandData.Clear()
                                                _Dev.ListCommandRepeat.Clear()
                                                For k1 As Integer = 0 To list.Item(j).ChildNodes.Item(k).ChildNodes.Count - 1
                                                    _Dev.ListCommandName.Add(list.Item(j).ChildNodes.Item(k).ChildNodes.Item(k1).Attributes(0).Value)
                                                    _Dev.ListCommandData.Add(list.Item(j).ChildNodes.Item(k).ChildNodes.Item(k1).Attributes(1).Value)
                                                    _Dev.ListCommandRepeat.Add(list.Item(j).ChildNodes.Item(k).ChildNodes.Item(k1).Attributes(2).Value)
                                                Next
                                            End If
                                        Next
                                    End If
                                End With
                                _ListDevices.Add(_Dev)
                                _Dev = Nothing
                            Next
                        End If
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", _ListDevices.Count & " devices(s) trouvé(s)")
                        list = Nothing
                    Next
                Else
                    Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Aucun device n'est enregistré dans le fichier de config")
                End If

                'Vide les variables
                dirInfo = Nothing
                file = Nothing
                files = Nothing
                myxml = Nothing

                Return " Chargement de la configuration terminée"

            Catch ex As Exception
                Return " Erreur de chargement de la config: " & ex.Message
            End Try
        End Function

        '--- Sauvegarde de la config dans le fichier XML
        Private Sub SaveConfig(ByVal Fichier As String)
            Try
                Log(TypeLog.INFO, TypeSource.SERVEUR, "SaveConfig", "Sauvegarde de la config sous le fichier " & Fichier)

                ''Copy du fichier de config avant sauvegarde
                Try
                    Dim _file As String = Fichier.Replace(".xml", "")
                    If File.Exists(_file & ".sav") = True Then File.Delete(_file & ".sav")
                    File.Copy(_file & ".xml", _file & ".sav")
                Catch ex As Exception
                    Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SaveConfig", "Erreur impossible de créer une copie de backup du fichier de config: " & ex.Message)
                End Try

                ''Creation du fichier XML
                Dim writer As New XmlTextWriter(Fichier, System.Text.Encoding.UTF8)
                writer.WriteStartDocument(True)
                writer.Formatting = Formatting.Indented
                writer.Indentation = 2

                writer.WriteStartElement("homidom")

                Log(TypeLog.INFO, TypeSource.SERVEUR, "SaveConfig", "Sauvegarde des paramètres serveur")
                ''------------ server
                writer.WriteStartElement("server")
                writer.WriteStartAttribute("longitude")
                writer.WriteValue(_Longitude)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("latitude")
                writer.WriteValue(_Latitude)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("heurecorrectionlever")
                writer.WriteValue(HeureCorrectionLever)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("heurecorrectioncoucher")
                writer.WriteValue(HeureCorrectionCoucher)
                writer.WriteEndAttribute()
                writer.WriteEndElement()

                ''-------------------
                ''------------drivers
                ''------------------
                Log(TypeLog.INFO, TypeSource.SERVEUR, "SaveConfig", "Sauvegarde des drivers")
                writer.WriteStartElement("drivers")
                For i As Integer = 0 To _ListDrivers.Count - 1
                    writer.WriteStartElement("driver")
                    writer.WriteStartAttribute("id")
                    writer.WriteValue(_ListDrivers.Item(i).ID)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("nom")
                    writer.WriteValue(_ListDrivers.Item(i).Nom)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("enable")
                    writer.WriteValue(_ListDrivers.Item(i).Enable)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("description")
                    writer.WriteValue(_ListDrivers.Item(i).Description)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("startauto")
                    writer.WriteValue(_ListDrivers.Item(i).StartAuto)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("protocol")
                    writer.WriteValue(_ListDrivers.Item(i).Protocol)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("iptcp")
                    writer.WriteValue(_ListDrivers.Item(i).IP_TCP)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("porttcp")
                    writer.WriteValue(_ListDrivers.Item(i).Port_TCP)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("ipudp")
                    writer.WriteValue(_ListDrivers.Item(i).IP_UDP)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("portudp")
                    writer.WriteValue(_ListDrivers.Item(i).Port_UDP)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("com")
                    writer.WriteValue(_ListDrivers.Item(i).Com)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("refresh")
                    writer.WriteValue(_ListDrivers.Item(i).Refresh)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("modele")
                    writer.WriteValue(_ListDrivers.Item(i).modele)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("picture")
                    writer.WriteValue(_ListDrivers.Item(i).Picture)
                    writer.WriteEndAttribute()
                    writer.WriteEndElement()
                Next
                writer.WriteEndElement()

                ''------------
                ''Sauvegarde des devices
                ''------------
                Log(TypeLog.INFO, TypeSource.SERVEUR, "SaveConfig", "Sauvegarde des devices")
                writer.WriteStartElement("devices")
                For i As Integer = 0 To _ListDevices.Count - 1
                    writer.WriteStartElement("device")
                    '-- propriétés génériques --
                    writer.WriteStartAttribute("id")
                    writer.WriteValue(_ListDevices.Item(i).id)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("name")
                    writer.WriteValue(_ListDevices.Item(i).Name)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("enable")
                    writer.WriteValue(_ListDevices.Item(i).enable)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("driverid")
                    writer.WriteValue(_ListDevices.Item(i).driverid)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("description")
                    writer.WriteValue(_ListDevices.Item(i).description)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("type")
                    writer.WriteValue(_ListDevices.Item(i).type)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("adresse1")
                    writer.WriteValue(_ListDevices.Item(i).adresse1)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("adresse2")
                    writer.WriteValue(_ListDevices.Item(i).adresse2)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("datecreated")
                    writer.WriteValue(_ListDevices.Item(i).datecreated)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("lastchange")
                    writer.WriteValue(_ListDevices.Item(i).lastchange)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("refresh")
                    writer.WriteValue(_ListDevices.Item(i).refresh)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("modele")
                    writer.WriteValue(_ListDevices.Item(i).modele)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("picture")
                    writer.WriteValue(_ListDevices.Item(i).picture)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("solo")
                    writer.WriteValue(_ListDevices.Item(i).solo)
                    writer.WriteEndAttribute()

                    '-- propriétés generique value --
                    If _ListDevices.Item(i).Type = "TEMPERATURE" _
                    Or _ListDevices.Item(i).Type = "HUMIDITE" _
                    Or _ListDevices.Item(i).Type = "TEMPERATURECONSIGNE" _
                    Or _ListDevices.Item(i).Type = "ENERGIETOTALE" _
                    Or _ListDevices.Item(i).Type = "ENERGIEINSTANTANEE" _
                    Or _ListDevices.Item(i).Type = "PLUIETOTAL" _
                    Or _ListDevices.Item(i).Type = "PLUIECOURANT" _
                    Or _ListDevices.Item(i).Type = "VITESSEVENT" _
                    Or _ListDevices.Item(i).Type = "UV" _
                    Or _ListDevices.Item(i).Type = "HUMIDITE" _
                    Then
                        writer.WriteStartAttribute("value")
                        writer.WriteValue(_ListDevices.Item(i).value)
                        writer.WriteEndAttribute()
                        writer.WriteStartAttribute("valuemin")
                        writer.WriteValue(_ListDevices.Item(i).valuemin)
                        writer.WriteEndAttribute()
                        writer.WriteStartAttribute("valuemax")
                        writer.WriteValue(_ListDevices.Item(i).valuemax)
                        writer.WriteEndAttribute()
                        writer.WriteStartAttribute("precision")
                        writer.WriteValue(_ListDevices.Item(i).precision)
                        writer.WriteEndAttribute()
                        writer.WriteStartAttribute("correction")
                        writer.WriteValue(_ListDevices.Item(i).correction)
                        writer.WriteEndAttribute()
                        writer.WriteStartAttribute("valuedef")
                        writer.WriteValue(_ListDevices.Item(i).valuedef)
                        writer.WriteEndAttribute()
                        writer.WriteStartAttribute("formatage")
                        writer.WriteValue(_ListDevices.Item(i).formatage)
                        writer.WriteEndAttribute()
                    End If

                    '-- Cas Code IR a ajouter pour MULTIMEDIA
                    If _ListDevices.Item(i).Type = "MULTIMEDIA" Then
                        writer.WriteStartElement("commands")
                        For k As Integer = 0 To _ListDevices.Item(i).ListCommandName.Count - 1
                            writer.WriteStartElement("command")
                            writer.WriteStartAttribute("key")
                            writer.WriteValue(_ListDevices.Item(i).ListCommandName(k))
                            writer.WriteEndAttribute()
                            writer.WriteStartAttribute("data")
                            writer.WriteValue(_ListDevices.Item(i).ListCommandData(k))
                            writer.WriteEndAttribute()
                            writer.WriteStartAttribute("repeat")
                            writer.WriteValue(_ListDevices.Item(i).ListCommandRepeat(k))
                            writer.WriteEndAttribute()
                            writer.WriteEndElement()
                        Next
                        writer.WriteEndElement()
                    End If
                    writer.WriteEndElement()
                Next
                writer.WriteEndElement()
                ''------------

                writer.WriteEndDocument()
                writer.Close()
                Log(TypeLog.INFO, TypeSource.SERVEUR, "SaveConfig", "Sauvegarde terminée")
            Catch ex As Exception
                MsgBox("ERREUR SAVECONFIG " & ex.ToString)
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SaveConfig", " Erreur de sauvegarde de la configuration: " & ex.Message)
            End Try

        End Sub

        '--- Charge les drivers
        Public Sub LoadDrivers()
            Try
                Dim tx As String
                Dim dll As Reflection.Assembly
                Dim tp As Type
                Dim Chm As String = _MonRepertoire & "\applications\Plugins\" 'Emplacement par défaut des plugins

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
                                Dim pt As New Driver(Me, i1.ID)
                                _ListDrivers.Add(i1)
                                _ListImgDrivers.Add(pt)
                                i1.Start()
                            End If
                        End If
                    Next
                Next
            Catch ex As Exception
                MsgBox("Erreur lors du chargement des drivers: " & ex.Message)
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "LoadDrivers", " Erreur lors du chargement des drivers: " & ex.Message)
            End Try
        End Sub

        '-- Retourne les propriétés d'un driver
        Public Function ReturnDriver(ByVal DriverId As String) As ArrayList
            For i As Integer = 0 To _ListDrivers.Count - 1
                Dim tabl As New ArrayList
                If _ListDrivers.Item(i).ID = DriverId Then
                    tabl.Add(_ListDrivers.Item(i).nom)
                    tabl.Add(_ListDrivers.Item(i).enable)
                    tabl.Add(_ListDrivers.Item(i).description)
                    tabl.Add(_ListDrivers.Item(i).startauto)
                    tabl.Add(_ListDrivers.Item(i).protocol)
                    tabl.Add(_ListDrivers.Item(i).isconnect)
                    tabl.Add(_ListDrivers.Item(i).IP_TCP)
                    tabl.Add(_ListDrivers.Item(i).Port_TCP)
                    tabl.Add(_ListDrivers.Item(i).IP_UDP)
                    tabl.Add(_ListDrivers.Item(i).Port_UDP)
                    tabl.Add(_ListDrivers.Item(i).COM)
                    tabl.Add(_ListDrivers.Item(i).Refresh)
                    tabl.Add(_ListDrivers.Item(i).Modele)
                    tabl.Add(_ListDrivers.Item(i).Version)
                    tabl.Add(_ListDrivers.Item(i).Picture)
                    tabl.Add(_ListDrivers.Item(i).DeviceSupport)
                    Return tabl
                    Exit For
                End If
            Next
        End Function

        '-- Ecrire ou lance propritété/Sub d'un driver
        Sub WriteDriver(ByVal DriverId As String, ByVal Command As String, ByVal Parametre As Object)
            For i As Integer = 0 To _ListDrivers.Count - 1
                If _ListDrivers.Item(i).ID = DriverId Then
                    Select Case UCase(Command)
                        Case "COM"
                            _ListDrivers.Item(i).Com = Parametre
                        Case "ENABLE"
                            _ListDrivers.Item(i).Enable = Parametre
                        Case "IP_TCP"
                            _ListDrivers.Item(i).IP_TCP = Parametre
                        Case "PORT_TCP"
                            _ListDrivers.Item(i).Port_TCP = Parametre
                        Case "IP_UDP"
                            _ListDrivers.Item(i).IP_UDP = Parametre
                        Case "PORT_UDP"
                            _ListDrivers.Item(i).Port_UDP = Parametre
                        Case "PICTURE"
                            _ListDrivers.Item(i).Picture = Parametre
                        Case "REFRESH"
                            _ListDrivers.Item(i).Refresh = Parametre
                        Case "STARTAUTO"
                            _ListDrivers.Item(i).StartAuto = Parametre
                        Case "START"
                            _ListDrivers.Item(i).Start()
                        Case "STOP"
                            _ListDrivers.Item(i).Stop()
                        Case "RESTART"
                            _ListDrivers.Item(i).Restart()
                    End Select
                    Exit For
                End If
            Next
        End Sub
#End Region

#Region "Interface Client"
        '********************************************************************
        'Fonctions/Sub/Propriétés partagées en service web pour les clients
        '********************************************************************

        '**** PROPRIETES ***************************

        Public Property Drivers() As ArrayList Implements IHoMIDom.Drivers
            Get
                Return _ListImgDrivers
            End Get
            Set(ByVal value As ArrayList)

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

        'Sauvegarder la configuration
        Public Sub SaveConfiguration() Implements IHoMIDom.SaveConfig
            SaveConfig(_MonRepertoire & "\config\homidom.xml")
        End Sub

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

            'Driver Existant
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
            'Chargement de la config
            LoadConfig(_MonRepertoire & "\Config\")
            LoadDrivers()
            TimerSecond.Interval = 1000
            AddHandler TimerSecond.Elapsed, AddressOf TimerSecTick
            TimerSecond.Enabled = True
            MAJ_HeuresSoleil()
        End Sub
#End Region

#Region "Log"
        Dim _File As String = _MonRepertoire & "\logs\log.xml" 'Représente le fichier log: ex"C:\homidom\log\log.xml"
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
            DEBUG               'visible uniquement si Homidom est en mode debug
            ERREUR              'erreur générale
            ERREUR_CRITIQUE     'erreur critique demandant la fermeture du programme
            INFO                'divers
            MESSAGE
            ACTION              'action lancé par un driver/device/trigger
            VALEUR_CHANGE       'Valeur ayant changé
            VALEUR_INCHANGE     'Valeur n'ayant pas changé
            VALEUR_INCHANGE_PRECISION   'Valeur n'ayant pas changé pour cause de precision
            VALEUR_INCHANGE_LASTETAT    'Valeur n'ayant pas changé pour cause de lastetat
        End Enum

        'Indique la source du log si c'est le serveur, un script, un device...
        Public Enum TypeSource
            SERVEUR
            SCRIPT
            TRIGGER
            DEVICE
            DRIVER
            SOAP
        End Enum

        Public Sub Log(ByVal TypLog As TypeLog, ByVal Source As TypeSource, ByVal Fonction As String, ByVal Message As String)
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

                'on affiche dans la console
                Console.WriteLine(Now & " " & TypLog & " " & Source & " " & Fonction & " " & Message)

                Dim xmldoc As New XmlDocument()

                'Ecrire le log
                Try
                    xmldoc.Load(_File) 'ouvre le fichier xml
                    Dim elelog As XmlElement = xmldoc.CreateElement("log") 'création de l'élément log
                    Dim atttime As XmlAttribute = xmldoc.CreateAttribute("time") 'création de l'attribut time
                    Dim atttype As XmlAttribute = xmldoc.CreateAttribute("type") 'création de l'attribut type
                    Dim attsrc As XmlAttribute = xmldoc.CreateAttribute("source") 'création de l'attribut source
                    Dim attfct As XmlAttribute = xmldoc.CreateAttribute("fonction") 'création de l'attribut source
                    Dim attmsg As XmlAttribute = xmldoc.CreateAttribute("message") 'création de l'attribut message

                    'on affecte les attributs à l'élément
                    elelog.SetAttributeNode(atttime)
                    elelog.SetAttributeNode(atttype)
                    elelog.SetAttributeNode(attsrc)
                    elelog.SetAttributeNode(attfct)
                    elelog.SetAttributeNode(attmsg)

                    'on affecte les valeur
                    elelog.SetAttribute("time", Now)
                    elelog.SetAttribute("type", TypLog)
                    elelog.SetAttribute("source", Source)
                    elelog.SetAttribute("fonction", Fonction)
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