Imports System.Net
Imports System.IO
Imports HoMIDom.HoMIDom.Server

Namespace HoMIDom
    '***********************************************
    '** CLASS DEVICE
    '** version 1.1
    '** Date de création: 12/01/2011
    '***********************************************

    ''' <summary>Class Device, définie tous différents types de devices</summary>
    ''' <remarks></remarks>
    <Serializable()> Public Class Device

        <NonSerialized()> Dim _Server As Server

        ''' <summary>Indique la liste des devices gérés</summary>
        ''' <remarks></remarks>
        Public Enum ListeDevices
            ''' <summary>
            ''' modules pour diriger un appareil  ON/OFF
            ''' </summary>
            ''' <remarks></remarks>
            APPAREIL = 1
            ''' <summary>
            ''' module de type audio
            ''' </summary>
            ''' <remarks></remarks>
            AUDIO = 2
            ''' <summary>
            ''' pour stocker les valeur issu d'un barometre meteo ou web
            ''' </summary>
            ''' <remarks></remarks>
            BAROMETRE = 3
            ''' <summary>
            ''' Pour stocker les valeurs issu d'une batterie
            ''' </summary>
            ''' <remarks></remarks>
            BATTERIE = 4
            ''' <summary>
            ''' compteur DS2423, RFXPower...
            ''' </summary>
            ''' <remarks></remarks>
            COMPTEUR = 5
            ''' <summary>
            ''' detecteur de contact : switch 1-wire
            ''' </summary>
            ''' <remarks></remarks>
            CONTACT = 6
            ''' <summary>
            ''' tous detecteurs : mouvement, obscurite...
            ''' </summary>
            ''' <remarks></remarks>
            DETECTEUR = 7
            ''' <summary>
            ''' module donnant la direction du vent
            ''' </summary>
            ''' <remarks></remarks>
            DIRECTIONVENT = 8
            ''' <summary>
            ''' Valeur de consommation d'energie instantanée
            ''' </summary>
            ''' <remarks></remarks>
            ENERGIEINSTANTANEE = 9
            ''' <summary>
            ''' Valeur de consommation d'energie totale
            ''' </summary>
            ''' <remarks></remarks>
            ENERGIETOTALE = 10
            ''' <summary>
            ''' FreeBox
            ''' </summary>
            ''' <remarks></remarks>
            FREEBOX = 11
            ''' <summary>
            ''' Générique Boolean
            ''' </summary>
            ''' <remarks></remarks>
            GENERIQUEBOOLEEN = 12
            ''' <summary>
            ''' Générique de type String
            ''' </summary>
            ''' <remarks></remarks>
            GENERIQUESTRING = 13
            ''' <summary>
            ''' Générique de type numérique
            ''' </summary>
            ''' <remarks></remarks>
            GENERIQUEVALUE = 14
            ''' <summary>
            ''' Capteur d'humidité
            ''' </summary>
            ''' <remarks></remarks>
            HUMIDITE = 15
            ''' <summary>
            ''' Module de type lampe avec variation
            ''' </summary>
            ''' <remarks></remarks>
            LAMPE = 16
            ''' <summary>
            ''' Station meteo (physique ou web)
            ''' </summary>
            ''' <remarks></remarks>
            METEO = 17
            ''' <summary>
            ''' Multimedia
            ''' </summary>
            ''' <remarks></remarks>
            MULTIMEDIA = 18
            ''' <summary>
            ''' Valeur de pluie courante
            ''' </summary>
            ''' <remarks></remarks>
            PLUIECOURANT = 19
            ''' <summary>
            ''' Valeur de pluie totale
            ''' </summary>
            ''' <remarks></remarks>
            PLUIETOTAL = 20
            ''' <summary>
            ''' Interrupteur
            ''' </summary>
            ''' <remarks></remarks>
            SWITCH = 21
            ''' <summary>
            ''' Telecommande
            ''' </summary>
            ''' <remarks></remarks>
            TELECOMMANDE = 22
            ''' <summary>
            ''' Capteur de température
            ''' </summary>
            ''' <remarks></remarks>
            TEMPERATURE = 23
            ''' <summary>
            ''' Température de consigne (ex: thermostat)
            ''' </summary>
            ''' <remarks></remarks>
            TEMPERATURECONSIGNE = 24
            ''' <summary>
            ''' Indice UV soleil
            ''' </summary>
            ''' <remarks></remarks>
            UV = 25
            ''' <summary>
            ''' Vitesse du vent
            ''' </summary>
            ''' <remarks></remarks>
            VITESSEVENT = 26
            ''' <summary>
            ''' Volet roulant
            ''' </summary>
            ''' <remarks></remarks>
            VOLET = 27
        End Enum

        <Serializable()> Public Class DeviceCommande
            Dim _NameCommand As String
            Dim _DescriptionCommand As String
            Dim _CountParam As Integer = 0
            Public Property NameCommand As String
                Get
                    Return _NameCommand
                End Get
                Set(ByVal value As String)
                    _NameCommand = value
                End Set
            End Property

            Public Property DescriptionCommand As String
                Get
                    Return _DescriptionCommand
                End Get
                Set(ByVal value As String)
                    _DescriptionCommand = value
                End Set
            End Property

            Public Property CountParam As Integer
                Get
                    Return _CountParam
                End Get
                Set(ByVal value As Integer)
                    _CountParam = value
                End Set
            End Property
        End Class

        ''' <summary>Class de déclaration du Device Générique</summary>
        ''' <remarks></remarks>
        <Serializable()> Public MustInherit Class DeviceGenerique
            <NonSerialized()> Protected _Server As Server
            Protected _ID As String = ""
            Protected _Name As String = ""
            Protected _Enable As Boolean = False
            Protected _DriverId As String = ""
            <NonSerialized()> Protected _Driver As Object
            Protected _Description As String = ""
            Protected _Type As String = ""
            Protected _Adresse1 As String = ""
            Protected _Adresse2 As String = ""
            Protected _DateCreated As Date = Now
            Protected _LastChange As Date = Now
            Protected _LastChangeDuree As Integer = 0
            Protected _Refresh As Double = 0
            Protected _Modele As String = ""
            Protected _Picture As String = ""
            Protected _Solo As Boolean = True
            Protected _LastEtat As Boolean = True
            Protected MyTimer As New Timers.Timer
            '<NonSerialized()> Protected _FirstTime As Boolean = True


            ''' <summary>
            ''' Retourne la liste de tous les fichiers image (png ou jpg) présents sur le serveur
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function GetDefautDeviceImage(ByVal TypeDevice As String) As String
                Try
                    Dim dirInfo As New System.IO.DirectoryInfo(_MonRepertoire & "\images\devices\")
                    Dim file As System.IO.FileInfo
                    Dim files() As System.IO.FileInfo = dirInfo.GetFiles("*.*g", System.IO.SearchOption.AllDirectories)

                    If (files IsNot Nothing) Then
                        For Each file In files
                            If LCase(Mid(file.Name, 1, Len(file.Name) - 4)) = LCase(TypeDevice & "-defaut") Then
                                Return file.FullName
                                Exit Function
                            End If
                        Next
                    End If

                    Return _MonRepertoire & "\images\devices\defaut.png"
                Catch ex As Exception
                    _Server.Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetListOfImage", "Exception : " & ex.Message)
                    Return Nothing
                End Try
            End Function

            'Identification unique du device
            Public Property ID() As String
                Get
                    Return _ID
                End Get
                Set(ByVal value As String)
                    _ID = value
                End Set
            End Property

            'Libellé de device (qui sert aussi à l'affichage)
            Public Property Name() As String
                Get
                    Return _Name
                End Get
                Set(ByVal value As String)
                    _Name = value
                End Set
            End Property

            'Activation du Device
            Public Property Enable() As Boolean
                Get
                    Return _Enable
                End Get
                Set(ByVal value As Boolean)
                    _Enable = value
                End Set
            End Property

            'Id du driver affect
            Public Property DriverID() As String
                Get
                    Return _DriverId
                End Get
                Set(ByVal value As String)
                    _DriverId = value
                    _Driver = _Server.ReturnDrvById(_IdSrv, DriverID)
                End Set
            End Property

            'Driver affecté (représentant l’objet déclaré du driver)
            Public ReadOnly Property Driver() As Object
                Get
                    Return _Driver
                End Get
            End Property

            'Description qui peut être le modèle du device ou autre chose
            Public Property Description() As String
                Get
                    Return _Description
                End Get
                Set(ByVal value As String)
                    _Description = value
                End Set
            End Property

            'TEMPERATURE|HUMIDITE|APPAREIL|LUMIERE|CONTACT|TV…
            Public ReadOnly Property Type() As String
                Get
                    Return _Type
                End Get
            End Property

            'Adresse par défaut (pour le X10 par exemple)
            Public Property Adresse1() As String
                Get
                    Return _Adresse1
                End Get
                Set(ByVal value As String)
                    _Adresse1 = value
                End Set
            End Property

            'Adresse supplémentaire si besoin (cas du RFXCOM)
            Public Property Adresse2() As String
                Get
                    Return _Adresse2
                End Get
                Set(ByVal value As String)
                    _Adresse2 = value
                End Set
            End Property

            'Date et heure de création du device
            Public Property DateCreated() As Date
                Get
                    Return _DateCreated
                End Get
                Set(ByVal value As Date)
                    _DateCreated = value
                End Set
            End Property

            'Date et heure de la derniere modification de Value
            Public Property LastChange() As Date
                Get
                    Return _LastChange
                End Get
                Set(ByVal value As Date)
                    _LastChange = value
                End Set
            End Property

            'Si Lastchanged+LastchangeDuree< Now alors le composant a un problème car il n'a pas emis depuis au moins lastchangedduree.
            Public Property LastChangeDuree() As Integer
                Get
                    Return _LastChangeDuree
                End Get
                Set(ByVal value As Integer)
                    _LastChangeDuree = value
                End Set
            End Property

            'Modèle du composant
            Public Property Modele() As String
                Get
                    Return _Modele
                End Get
                Set(ByVal value As String)
                    _Modele = value
                End Set
            End Property

            'Adresse de son image
            Public Property Picture() As String
                Get
                    Return _Picture
                End Get
                Set(ByVal value As String)
                    If value = " " Or value = "" Then
                        _Picture = GetDefautDeviceImage(_Type.ToString)
                    Else
                        _Picture = value
                    End If
                End Set
            End Property

            'Si le device est solo ou s'il contient plusieurs I/O
            Public Property Solo() As Boolean
                Get
                    Return _Solo
                End Get
                Set(ByVal value As Boolean)
                    _Solo = value
                End Set
            End Property

            'si =true, on ne prend pas en comtpe les modifications style 19.1 19 19.1 19...
            Public Property LastEtat() As Boolean
                Get
                    Return _LastEtat
                End Get
                Set(ByVal value As Boolean)
                    _LastEtat = value
                End Set
            End Property

            Public ReadOnly Property GetCommandPlus As List(Of DeviceCommande)
                Get
                    If _Driver IsNot Nothing Then
                        Return _Driver.GetCommandPlus
                    Else
                        Return Nothing
                    End If
                End Get
            End Property

            Protected Overrides Sub Finalize()
                MyBase.Finalize()
                'RemoveHandler MyTimer.Elapsed, AddressOf read
            End Sub

            Public Function ExecuteCommand(ByVal Command As String, Optional ByVal Param() As Object = Nothing) As Boolean
                Try
                    If _Enable = False Then Exit Function
                    Dim CMD As String = UCase(Command)
                    Return _Driver.ExecuteCommand(Me, CMD, Param)
                Catch ex As Exception
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Device ExecuteCommand", ex.Message)
                    Return False
                End Try
            End Function

            Protected Sub setrefresh(ByVal value As Double)
                Try
                    If value > 0 Then
                        _Refresh = value
                        'si timer déjà lancé, on l'arrete
                        If MyTimer.Enabled Then MyTimer.Stop()
                        'si le serveur n'a pas fini de démarrer, on décale le lancement du timer pour eviter les conflits
                        If Not _Server.Etat_server Then System.Threading.Thread.Sleep(2000)
                        MyTimer.Interval = _Refresh
                        MyTimer.Start()
                    Else
                        _Refresh = 0
                        'si timer déjà lancé, on l'arrete
                        If MyTimer.Enabled Then MyTimer.Stop()
                    End If
                Catch ex As Exception
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Device Refresh", ex.Message)
                End Try
            End Sub
        End Class

        ''' <summary>Classe valeur Double avec min/max/def/correction...</summary>
        ''' <remarks></remarks>
        <Serializable()> Public Class DeviceGenerique_ValueDouble
            Inherits DeviceGenerique

            Protected _Value As Double = 0
            Protected _ValueLast As Double = 0
            Protected _ValueMin As Double = -9999
            Protected _ValueMax As Double = 9999
            Protected _ValueDef As Double = 0
            Protected _Precision As Double = 0
            Protected _Correction As Double = 0
            Protected _Formatage As String = ""

            'Contien l'avant derniere valeur
            Public Property ValueLast() As Double
                Get
                    Return _ValueLast
                End Get
                Set(ByVal value As Double)
                    _ValueLast = value
                End Set
            End Property

            'Valeur minimale que value peut avoir 
            Public Property ValueMin() As Double
                Get
                    Return _ValueMin
                End Get
                Set(ByVal value As Double)
                    _ValueMin = value
                End Set
            End Property

            'Valeur maximale que value peut avoir 
            Public Property ValueMax() As Double
                Get
                    Return _ValueMax
                End Get
                Set(ByVal value As Double)
                    _ValueMax = value
                End Set
            End Property

            'Valeur par défaut de Value au démarrage du Device, si Vide = Value
            Public Property ValueDef() As Double
                Get
                    Return _ValueDef
                End Get
                Set(ByVal value As Double)
                    _ValueDef = value
                    _Value = _ValueDef
                End Set
            End Property

            'Precision de value
            Public Property Precision() As Double
                Get
                    Return _Precision
                End Get
                Set(ByVal value As Double)
                    _Precision = value
                End Set
            End Property

            'Correction en +/-/*/div à effectuer sur la value
            Public Property Correction() As Double
                Get
                    Return _Correction
                End Get
                Set(ByVal value As Double)
                    _Correction = value
                End Set
            End Property

            'Format de value 0.0 ou 0.00...
            Public Property Formatage() As String
                Get
                    Return _Formatage
                End Get
                Set(ByVal value As String)
                    _Formatage = value
                End Set
            End Property

            'Event lancé sur changement de Value
            Public Event DeviceChanged(ByVal Device As Object, ByVal [Property] As String, ByVal Parametre As Object)

            Public Property Refresh() As Integer
                Get
                    Return _Refresh
                End Get
                Set(ByVal value As Integer)
                    setrefresh(value)
                End Set
            End Property

            Public Sub Read()
                Try
                    If _Enable = False Then Exit Sub
                    If Driver.IsConnect() And _Server.Etat_server Then Driver.Read(Me)
                Catch ex As Exception
                    _Server.Log(TypeLog.ERREUR, TypeSource.DEVICE, "DeviceBOOL Read", "Exception : " & ex.Message)
                End Try
            End Sub

            'Valeur
            Public Property Value() As Double
                Get
                    Return _Value
                End Get
                Set(ByVal tmp As Double)
                    Try
                        'si le serveur n'a pas fini de démarrer, on affecte juste la valeur
                        If Not _Server.Etat_server Then
                            _Value = tmp
                            _ValueLast = tmp
                        Else
                            _LastChange = Now
                            If tmp < _ValueMin Then tmp = _ValueMin
                            If tmp > _ValueMax Then tmp = _ValueMax
                            If _Formatage <> "" Then tmp = Format(tmp, _Formatage)
                            tmp += _Correction
                            'Si la valeur a changé on la prend en compte et on créer l'event
                            'If tmp <> _Value Then
                            '    _ValueLast = _Value 'on garde l'ancienne value en memoire
                            '    _Value = tmp
                            '    If _Server.Etat_server Then RaiseEvent DeviceChanged(Me, "Value", _Value)
                            'Else
                            '    If _Server.Etat_server Then _Server.Log(Server.TypeLog.VALEUR_INCHANGE, Server.TypeSource.SERVEUR, "DeviceValue Inchangé", _Name & " : " & _Adresse1 & " : " & _Value)
                            'End If

                            If tmp = _Value Then
                                _Server.Log(TypeLog.VALEUR_INCHANGE, TypeSource.DEVICE, "DeviceDBL Value", _Name & " : " & _Adresse1 & " : " & _Value.ToString & " (Inchangé)")
                            Else
                                '--- si lastetat=True, on vérifie que la valeur a changé par rapport a l'avant dernier etat (valuelast) 
                                If _LastEtat And tmp = _ValueLast Then
                                    'log de "inchangé lastetat"
                                    _Server.Log(TypeLog.VALEUR_INCHANGE_LASTETAT, TypeSource.DEVICE, "DeviceDBL Value", _Name & " : " & _Adresse1 & " : " & tmp.ToString & " (inchangé lastetat " & _ValueLast.ToString & ")")
                                Else
                                    'on vérifie que la valeur a changé de plus de precision sinon inchangé
                                    If ((_Precision <> 0) And ((tmp + _Precision) >= _Value) And ((tmp - _Precision) <= _Value)) Then
                                        'log de "inchangé précision"
                                        _Server.Log(TypeLog.VALEUR_INCHANGE_PRECISION, TypeSource.DEVICE, "DeviceDBL Value", _Name & " : " & _Adresse1 & " : " & tmp.ToString & " (inchangé precision " & _Value.ToString & "+-" & _Precision.ToString & ")")
                                    Else
                                        'enregistre la nouvelle valeur
                                        _Server.Log(TypeLog.VALEUR_CHANGE, TypeSource.DEVICE, "DeviceDBL Value", _Name & " : " & _Adresse1 & " : " & tmp.ToString)
                                        _ValueLast = _Value 'on garde l'ancienne value en memoire
                                        _Value = tmp
                                        RaiseEvent DeviceChanged(Me, "Value", _Value)
                                    End If
                                End If
                            End If
                        End If
                    Catch ex As Exception
                        _Server.Log(TypeLog.ERREUR, TypeSource.DEVICE, "DeviceDBL Value", "Exception : " & ex.Message)
                    End Try
                End Set
            End Property
        End Class

        ''' <summary>Classe valeur True/False pour device ON/OFF</summary>
        ''' <remarks></remarks>
        <Serializable()> Public Class DeviceGenerique_ValueBool
            Inherits DeviceGenerique

            Protected _Value As Boolean = False
            Protected _ValueLast As Boolean = False
            Protected _valuemustchange As Boolean = False 'si true alors value n'est modifié que si la valeur change

            'Contien l'avant derniere valeur
            Public Property ValueLast() As Double
                Get
                    Return _ValueLast
                End Get
                Set(ByVal value As Double)
                    _ValueLast = value
                End Set
            End Property

            'Event lancé sur changement de Value
            Public Event DeviceChanged(ByVal Device As Object, ByVal [Property] As String, ByVal Parametre As Object)

            'Si X= 0 le serveur attend un event du driver pour mettre à jour la value du device (Cas du RFXCOM)
            'Si X>0 (cas du 1wire par ex) un timer propre au device se lance et effectue un mondevicetemp.Driver.ReadTemp(Me), le driver récupère l’adresse sur l’objet Me sachant que c’est un ReadTemp (donc température) va lire une température à l’adresse spécifié. Cependant un event d’un driver peut modifier la value d’un device même si un refresh a été paramétré
            Public Property Refresh() As Integer
                Get
                    Return _Refresh
                End Get
                Set(ByVal value As Integer)
                    setrefresh(value)
                End Set
            End Property

            'Demande de Lecture au driver
            Public Overloads Sub Read()
                Try
                    If _Enable = False Then Exit Sub
                    If Driver.IsConnect() And _Server.Etat_server Then Driver.Read(Me)
                Catch ex As Exception
                    _Server.Log(TypeLog.ERREUR, TypeSource.DEVICE, "DeviceBOOL Read", "Exception : " & ex.Message)
                End Try
            End Sub

            'Valeur : ON/OFF = True/False
            Public Property Value As Boolean
                Get
                    Return _Value
                End Get
                Set(ByVal tmp As Boolean)
                    Try
                        'si le serveur n'a pas fini de démarrer, on affecte juste la valeur
                        If Not _Server.Etat_server Then
                            _Value = tmp
                            _ValueLast = tmp
                        Else
                            'on prend en compte la value à chaque fois car on peut donne le même ordre plusieurs fois (sauf si _valuemustchange = TRUE)
                            _LastChange = Now
                            If _valuemustchange And tmp = _Value Then
                                _Server.Log(TypeLog.VALEUR_INCHANGE, TypeSource.DEVICE, "DeviceDBL Value", _Name & " : " & _Adresse1 & " : " & _Value.ToString & " (Inchangé)")
                            Else
                                _Server.Log(TypeLog.VALEUR_CHANGE, TypeSource.DEVICE, "DeviceBool Value", _Name & " : " & _Adresse1 & " : " & tmp.ToString)
                                _ValueLast = _Value 'on garde l'ancienne value en memoire
                                _Value = tmp
                                RaiseEvent DeviceChanged(Me, "Value", _Value)
                            End If
                        End If
                    Catch ex As Exception
                        _Server.Log(TypeLog.ERREUR, TypeSource.DEVICE, "DeviceBOOL Value", "Exception : " & ex.Message)
                    End Try
                End Set
            End Property

        End Class

        ''' <summary>Classe valeur Integer pour device avec valeur de 0(OFF) à 100(ON)</summary>
        ''' <remarks></remarks>
        <Serializable()> Public MustInherit Class DeviceGenerique_ValueInt
            Inherits DeviceGenerique

            Protected _Value As Integer = 0
            Protected _ValueLast As Integer = 0
            Protected _ValueMin As Integer = 0
            Protected _ValueMax As Integer = 100
            Protected _ValueDef As Integer = 0
            Protected _Precision As Integer = 0
            Protected _Correction As Integer = 0
            Protected _Formatage As String = ""

            'Contien l'avant derniere valeur
            Public Property ValueLast() As Integer
                Get
                    Return _ValueLast
                End Get
                Set(ByVal value As Integer)
                    _ValueLast = value
                End Set
            End Property

            'Valeur minimale que value peut avoir 
            Public Property ValueMin() As Double
                Get
                    Return _ValueMin
                End Get
                Set(ByVal value As Double)
                    _ValueMin = value
                End Set
            End Property

            'Valeur maximale que value peut avoir 
            Public Property ValueMax() As Double
                Get
                    Return _ValueMax
                End Get
                Set(ByVal value As Double)
                    _ValueMax = value
                End Set
            End Property

            'Valeur par défaut de Value au démarrage du Device, si Vide = Value
            Public Property ValueDef() As Double
                Get
                    Return _ValueDef
                End Get
                Set(ByVal value As Double)
                    _ValueDef = value
                    _Value = _ValueDef
                End Set
            End Property
            'Precision de value
            Public Property Precision() As Integer
                Get
                    Return _Precision
                End Get
                Set(ByVal value As Integer)
                    _Precision = value
                End Set
            End Property

            'Correction en +/-/*/div à effectuer sur la value
            Public Property Correction() As Integer
                Get
                    Return _Correction
                End Get
                Set(ByVal value As Integer)
                    _Correction = value
                End Set
            End Property

            'Format de value 0.0 ou 0.00...
            Public Property Formatage() As String
                Get
                    Return _Formatage
                End Get
                Set(ByVal value As String)
                    _Formatage = value
                End Set
            End Property

            'Event lancé sur changement de Value
            Public Event DeviceChanged(ByVal Device As Object, ByVal [Property] As String, ByVal Parametre As Object)

            'Si X= 0 le serveur attend un event du driver pour mettre à jour la value du device (Cas du RFXCOM)
            'Si X>0 (cas du 1wire par ex) un timer propre au device se lance et effectue un mondevicetemp.Driver.ReadTemp(Me), le driver récupère l’adresse sur l’objet Me sachant que c’est un ReadTemp (donc température) va lire une température à l’adresse spécifié. Cependant un event d’un driver peut modifier la value d’un device même si un refresh a été paramétré
            Public Property Refresh() As Integer
                Get
                    Return _Refresh
                End Get
                Set(ByVal value As Integer)
                    setrefresh(value)
                End Set
            End Property

            'Demande de Lecture au driver
            Public Sub Read()
                Try
                    If _Enable = False Then Exit Sub
                    If Driver.IsConnect() And _Server.Etat_server Then Driver.Read(Me)
                Catch ex As Exception
                    _Server.Log(TypeLog.ERREUR, TypeSource.DEVICE, "DeviceINT Read", "Exception : " & ex.Message)
                End Try
            End Sub

            'Valeur de 0 à 100
            Public Property Value() As Integer
                Get
                    Return _Value
                End Get
                Set(ByVal tmp As Integer)
                    '    _LastChange = Now
                    '    If tmp < 0 Then tmp = 0
                    '    If tmp > 100 Then tmp = 100
                    '    _ValueLast = _Value 'on garde l'ancienne value en memoire
                    '    _Value = tmp 'on prend en compte la value à chaque fois car on peut donne le même ordre plusieurs fois
                    'If _Server.Etat_server Then RaiseEvent DeviceChanged(Me, "Value", _Value)

                    Try
                        'si le serveur n'a pas fini de démarrer, on affecte juste la valeur
                        If Not _Server.Etat_server Then
                            _Value = tmp
                            _ValueLast = tmp
                        Else
                            _LastChange = Now
                            If tmp < _ValueMin Then tmp = _ValueMin 'If tmp < 0 Then tmp = 0
                            If tmp > _ValueMax Then tmp = _ValueMax 'If tmp > 100 Then tmp = 100
                            If _Formatage <> "" Then tmp = Format(tmp, _Formatage)
                            tmp += _Correction

                            If tmp = _Value Then
                                _Server.Log(TypeLog.VALEUR_INCHANGE, TypeSource.DEVICE, "DeviceINT Value", _Name & " : " & _Adresse1 & " : " & _Value & " (Inchangé)")
                            Else
                                '--- si lastetat=True, on vérifie que la valeur a changé par rapport a l'avant dernier etat (valuelast) 
                                If _LastEtat And tmp = _ValueLast Then
                                    'log de "inchangé lastetat"
                                    _Server.Log(TypeLog.VALEUR_INCHANGE_LASTETAT, TypeSource.DEVICE, "DeviceINT", _Name & " : " & _Adresse1 & " : " & tmp.ToString & " (inchangé lastetat " & _ValueLast.ToString & ")")
                                Else
                                    'on vérifie que la valeur a changé de plus de precision sinon inchangé
                                    If ((tmp + _Precision) >= _Value) And ((tmp - _Precision) <= _Value) Then
                                        'log de "inchangé précision"
                                        _Server.Log(TypeLog.VALEUR_INCHANGE_PRECISION, TypeSource.DEVICE, "DeviceINT", _Name & " : " & _Adresse1 & " : " & tmp.ToString & " (inchangé precision " & _Value.ToString & "+-" & _Precision.ToString & ")")
                                    Else
                                        'enregistre la nouvelle valeur
                                        _Server.Log(TypeLog.VALEUR_CHANGE, TypeSource.DEVICE, "DeviceINT Value", _Name & " : " & _Adresse1 & " : " & tmp.ToString)
                                        _ValueLast = _Value 'on garde l'ancienne value en memoire
                                        _Value = tmp
                                        RaiseEvent DeviceChanged(Me, "Value", _Value)
                                    End If
                                End If
                            End If
                        End If
                    Catch ex As Exception
                        _Server.Log(TypeLog.ERREUR, TypeSource.DEVICE, "DeviceINT Value", "Exception : " & ex.Message)
                    End Try
                End Set
            End Property

        End Class

        ''' <summary>Classe valeur String </summary>
        ''' <remarks></remarks>
        <Serializable()> Public Class DeviceGenerique_ValueString
            Inherits DeviceGenerique

            Protected _Value As String = ""
            Protected _ValueLast As String = ""

            'Contient l'avant derniere valeur
            Public Property ValueLast() As String
                Get
                    Return _ValueLast
                End Get
                Set(ByVal value As String)
                    _ValueLast = value
                End Set
            End Property

            Public Event DeviceChanged(ByVal Device As Object, ByVal [Property] As String, ByVal Parametre As Object)

            'Si X= 0 le serveur attend un event du driver pour mettre à jour la value du device (Cas du RFXCOM)
            'Si X>0 (cas du 1wire par ex) un timer propre au device se lance et effectue un mondevicetemp.Driver.ReadTemp(Me), le driver récupère l’adresse sur l’objet Me sachant que c’est un ReadTemp (donc température) va lire une température à l’adresse spécifié. Cependant un event d’un driver peut modifier la value d’un device même si un refresh a été paramétré
            Public Property Refresh() As Integer
                Get
                    Return _Refresh
                End Get
                Set(ByVal value As Integer)
                    setrefresh(value)
                End Set
            End Property

            Public Overridable Sub Read()
                If _Enable = False Then Exit Sub
                If Driver.IsConnect() And _Server.Etat_server Then Driver.Read(Me)
            End Sub

            Public Property Value() As String
                Get
                    Return _Value
                End Get
                Set(ByVal tmp As String)
                    '_LastChange = Now
                    ''Si la valeur a changé on la prend en compte et on créer l'event
                    'If tmp <> _Value Then
                    '    _ValueLast = _Value 'on garde l'ancienne value en memoire
                    '    _Value = tmp
                    '    If _Server.Etat_server Then RaiseEvent DeviceChanged(Me, "Value", _Value)
                    'Else
                    '    If _Server.Etat_server Then _Server.Log(Server.TypeLog.VALEUR_INCHANGE, Server.TypeSource.SERVEUR, "DeviceValue Inchangé", _Name & " : " & _Adresse1 & " : " & _Value)
                    'End If

                    Try
                        'si le serveur n'a pas fini de démarrer, on affecte juste la valeur
                        If Not _Server.Etat_server Then
                            _ValueLast = tmp
                            _Value = tmp
                        Else
                            _LastChange = Now
                            'If Mid(tmp, 1, 4) <> "CFG:" Then
                            If tmp = _Value Then
                                _Server.Log(TypeLog.VALEUR_INCHANGE, TypeSource.DEVICE, "DeviceSTR Value", _Name & " : " & _Adresse1 & " : " & _Value & " (Inchangé)")
                            Else
                                '--- si lastetat=True, on vérifie que la valeur a changé par rapport a l'avant dernier etat (valuelast) 
                                If LastEtat And tmp = _ValueLast Then
                                    'log de "inchangé lastetat"
                                    _Server.Log(TypeLog.VALEUR_INCHANGE_LASTETAT, TypeSource.DEVICE, "DeviceSTR Value", _Name & " : " & _Adresse1 & " : " & tmp & " (inchangé lastetat " & _ValueLast & ")")
                                Else
                                    'enregistre la nouvelle valeur
                                    _Server.Log(TypeLog.VALEUR_CHANGE, TypeSource.DEVICE, "DeviceSTR Value", _Name & " : " & _Adresse1 & " : " & tmp)
                                    _ValueLast = _Value 'on garde l'ancienne value en memoire
                                    _Value = tmp
                                    If _Server.Etat_server Then RaiseEvent DeviceChanged(Me, "Value", _Value)
                                End If
                            End If
                            'Else
                            ''log de l'info de config
                            '_Server.Log(TypeLog.VALEUR_CHANGE, TypeSource.DEVICE, "DeviceSTR Value", _Name & " : " & _Adresse1 & " : " & tmp)
                            'End If
                        End If
                    Catch ex As Exception
                        _Server.Log(TypeLog.ERREUR, TypeSource.DEVICE, "DeviceSTR Value", "Exception : " & ex.Message)
                    End Try
                End Set
            End Property

        End Class

        <Serializable()> Class APPAREIL
            Inherits DeviceGenerique_ValueBool

            ''' <summary>Permet de tester une commande write</summary>
            ''' <param name="Commande"></param>
            ''' <param name="Parametre1"></param>
            ''' <param name="Parametre2"></param>
            ''' <remarks></remarks>
            Public Sub TestWrite(ByVal Commande As String, Optional ByVal Parametre1 As String = "", Optional ByVal Parametre2 As String = "")
                If _Enable = False Then Exit Sub
                Driver.Write(Me, UCase(Commande), Parametre1, Parametre2)
            End Sub

            ''' <summary>Permet de tester une commande read</summary>
            ''' <remarks></remarks>
            Public Sub TestRead()
                If _Enable = False Then Exit Sub
                Driver.read(Me)
            End Sub

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "APPAREIL"
                AddHandler MyTimer.Elapsed, AddressOf Read
            End Sub

            'ON
            Public Sub [ON]()
                If _Enable = False Then Exit Sub
                Driver.Write(Me, "ON")
            End Sub

            'OFF
            Public Sub OFF()
                If _Enable = False Then Exit Sub
                Driver.Write(Me, "OFF")
            End Sub
        End Class

        <Serializable()> Class AUDIO
            Inherits DeviceGenerique_ValueString
            Dim _Fichier As String

            ''' <summary>Permet de tester une commande write</summary>
            ''' <param name="Commande"></param>
            ''' <param name="Parametre1"></param>
            ''' <param name="Parametre2"></param>
            ''' <remarks></remarks>
            Public Sub TestWrite(ByVal Commande As String, Optional ByVal Parametre1 As String = "", Optional ByVal Parametre2 As String = "")
                If _Enable = False Then Exit Sub
                Driver.Write(Me, UCase(Commande), Parametre1, Parametre2)
            End Sub

            ''' <summary>Permet de tester une commande read</summary>
            ''' <remarks></remarks>
            Public Sub TestRead()
                If _Enable = False Then Exit Sub
                Driver.read(Me)
            End Sub

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "AUDIO"
                AddHandler MyTimer.Elapsed, AddressOf Read
            End Sub

            'redéfinition car on ne veut rien faire
            Public Overloads Sub Read()
                If _Enable = False Then Exit Sub
            End Sub

            Public Property Fichier() As String
                Get
                    Return _Fichier
                End Get
                Set(ByVal value As String)
                    _Fichier = value
                End Set
            End Property

            Public Sub SetFichierAudio(ByVal File As String)
                Try
                    If _Enable = False Then Exit Sub
                    _Fichier = File
                Catch ex As Exception
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DEVICE, Me.Name, " SetFichierAudio " & File & " : " & ex.Message)
                End Try
            End Sub

            Private Sub touche(ByVal commande As String)
                Try
                    If _Enable = False Then Exit Sub
                    Driver.Write(Me, commande)
                    Value = commande
                Catch ex As Exception
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DEVICE, Me.Name, " Touche" & commande & " : " & ex.Message)
                End Try
            End Sub

            Public Sub EmptyPlayList()
                touche("EmptyPlayList")
            End Sub

            Public Sub Browse(ByVal Param1 As String)
                Try
                    _Fichier = Param1
                    If _Enable = False Then Exit Sub
                    Driver.Write(Me, "Browse", _Fichier)
                    Value = "Browse"
                Catch ex As Exception
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DEVICE, Me.Name, "PlayAudio: " & ex.Message)
                End Try
            End Sub

            Public Sub Start(ByVal Item As Integer)
                Try
                    If _Enable = False Then Exit Sub
                    Driver.Write(Me, "start", Item)
                    Value = "Start"
                Catch ex As Exception
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DEVICE, Me.Name, "Erreur: " & ex.Message)
                End Try
            End Sub

            Public Sub Play(Optional ByVal Param1 As String = "")
                Try
                    _Fichier = Param1
                    If _Enable = False Then Exit Sub
                    Driver.Write(Me, "PlayAudio", _Fichier)
                    Value = "PlayAudio"
                Catch ex As Exception
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DEVICE, Me.Name, "PlayAudio: " & ex.Message)
                End Try
            End Sub

            Public Sub Pause()
                touche("PauseAudio")
            End Sub

            Public Sub [Stop]()
                touche("StopAudio")
            End Sub

            Public Sub Random()
                touche("RandomAudio")
            End Sub

            Public Sub [Next]()
                touche("NextAudio")
            End Sub

            Public Sub Previous()
                touche("PreviousAudio")
            End Sub

            Public Sub VolumeDown()
                touche("VolumeDownAudio")
            End Sub

            Public Sub VolumeUp()
                touche("VolumeUpAudio")
            End Sub

            Public Sub VolumeMute()
                touche("VolumeMuteAudio")
            End Sub

            Public Sub Volume(ByVal Level As Integer)
                Try
                    If _Enable = False Then Exit Sub
                    Driver.Write(Me, "volume", Level)
                    Value = "Volume"
                Catch ex As Exception
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DEVICE, Me.Name, "Erreur: " & ex.Message)
                End Try
            End Sub

            Public Sub VolumeDB(ByVal Level As Integer)
                Try
                    If _Enable = False Then Exit Sub
                    Driver.Write(Me, "volumedb", Level)
                    Value = "VolumeDB"
                Catch ex As Exception
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DEVICE, Me.Name, "Erreur: " & ex.Message)
                End Try
            End Sub

            Public Sub VolumeDBDelta(ByVal Level As Integer)
                Try
                    If _Enable = False Then Exit Sub
                    Driver.Write(Me, "volumedbdelta", Level)
                    Value = "VolumeDBDelta"
                Catch ex As Exception
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DEVICE, Me.Name, "Erreur: " & ex.Message)
                End Try
            End Sub

            Public Sub Seek(ByVal Position As Integer)
                Try
                    If _Enable = False Then Exit Sub
                    Driver.Write(Me, "Seek", Position)
                    Value = "Seek"
                Catch ex As Exception
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DEVICE, Me.Name, "Erreur: " & ex.Message)
                End Try
            End Sub

            Public Sub SeekDelta(ByVal Delta As Integer)
                Try
                    If _Enable = False Then Exit Sub
                    Driver.Write(Me, "SeekDelta", Delta)
                    Value = "SeekDelta"
                Catch ex As Exception
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DEVICE, Me.Name, "Erreur: " & ex.Message)
                End Try
            End Sub

            Public Sub PlayBackOrder(ByVal Order As Integer)
                Try
                    If _Enable = False Then Exit Sub
                    Driver.Write(Me, "PlayBackOrder", Order)
                    Value = "PlayBackOrder"
                Catch ex As Exception
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DEVICE, Me.Name, "Erreur: " & ex.Message)
                End Try
            End Sub

            Public Sub Sac(ByVal Flag As Integer)
                Try
                    If _Enable = False Then Exit Sub
                    Driver.Write(Me, "Sac", Flag)
                    Value = "Sac"
                Catch ex As Exception
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DEVICE, Me.Name, "Erreur: " & ex.Message)
                End Try
            End Sub

            Public Sub QueueItems(ByVal Index As Integer)
                Try
                    If _Enable = False Then Exit Sub
                    Driver.Write(Me, "QueueItems", Index)
                    Value = "QueueItems"
                Catch ex As Exception
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DEVICE, Me.Name, "Erreur: " & ex.Message)
                End Try
            End Sub

            Public Sub QueueAlbum(ByVal Index As Integer)
                Try
                    If _Enable = False Then Exit Sub
                    Driver.Write(Me, "QueueAlbum", Index)
                    Value = "QueueAlbum"
                Catch ex As Exception
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DEVICE, Me.Name, "Erreur: " & ex.Message)
                End Try
            End Sub

            Public Sub QueueRandomItems(ByVal Index As Integer)
                Try
                    If _Enable = False Then Exit Sub
                    Driver.Write(Me, "QueueRandomItems", Index)
                    Value = "QueueRandomItems"
                Catch ex As Exception
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DEVICE, Me.Name, "Erreur: " & ex.Message)
                End Try
            End Sub

            Public Sub DequeueItems(ByVal Index As Integer)
                Try
                    If _Enable = False Then Exit Sub
                    Driver.Write(Me, "DequeueItems", Index)
                    Value = "DequeueItems"
                Catch ex As Exception
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DEVICE, Me.Name, "Erreur: " & ex.Message)
                End Try
            End Sub

            Public Sub FlushQueue()
                Try
                    If _Enable = False Then Exit Sub
                    Driver.Write(Me, "FlushQueue")
                    Value = "FlushQueue"
                Catch ex As Exception
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DEVICE, Me.Name, "Erreur: " & ex.Message)
                End Try
            End Sub

            Public Sub Del(ByVal Item As String, ByVal Index As Integer)
                Try
                    If _Enable = False Then Exit Sub
                    Driver.Write(Me, "Del", Item, Index)
                    Value = "Del"
                Catch ex As Exception
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DEVICE, Me.Name, "Erreur: " & ex.Message)
                End Try
            End Sub

            Public Sub Undo(ByVal Index As Integer)
                Try
                    If _Enable = False Then Exit Sub
                    Driver.Write(Me, "Undo", Index)
                    Value = "Undo"
                Catch ex As Exception
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DEVICE, Me.Name, "Erreur: " & ex.Message)
                End Try
            End Sub

            Public Sub Move(ByVal Item As String, ByVal Index As Integer)
                Try
                    If _Enable = False Then Exit Sub
                    Driver.Write(Me, "Move", Item, Index)
                    Value = "Move"
                Catch ex As Exception
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DEVICE, Me.Name, "Erreur: " & ex.Message)
                End Try
            End Sub

            Public Sub SetSelection(ByVal Item As String, ByVal Index As Integer)
                Try
                    If _Enable = False Then Exit Sub
                    Driver.Write(Me, "SetSelection", Item, Index)
                    Value = "SetSelection"
                Catch ex As Exception
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DEVICE, Me.Name, "Erreur: " & ex.Message)
                End Try
            End Sub

            Public Sub SwitchPlayList(ByVal Index As Integer)
                Try
                    If _Enable = False Then Exit Sub
                    Driver.Write(Me, "SwitchPlayList", Index)
                    Value = "SwitchPlayList"
                Catch ex As Exception
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DEVICE, Me.Name, "Erreur: " & ex.Message)
                End Try
            End Sub

            Public Sub SetFocus(ByVal Index As Integer)
                Try
                    If _Enable = False Then Exit Sub
                    Driver.Write(Me, "SetFocus", Index)
                    Value = "SetFocus"
                Catch ex As Exception
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DEVICE, Me.Name, "Erreur: " & ex.Message)
                End Try
            End Sub

            Public Sub RemovePlaylist(ByVal Index As Integer)
                Try
                    If _Enable = False Then Exit Sub
                    Driver.Write(Me, "RemovePlaylist", Index)
                    Value = "RemovePlaylist"
                Catch ex As Exception
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DEVICE, Me.Name, "Erreur: " & ex.Message)
                End Try
            End Sub
        End Class

        <Serializable()> Class BAROMETRE
            Inherits DeviceGenerique_ValueDouble

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "BAROMETRE"
                AddHandler MyTimer.Elapsed, AddressOf Read
            End Sub

        End Class

        <Serializable()> Class BATTERIE
            Inherits DeviceGenerique_ValueString

            'Creation d'un device BATTERIE
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "BATTERIE"
                AddHandler MyTimer.Elapsed, AddressOf Read
            End Sub

        End Class

        <Serializable()> Class COMPTEUR
            Inherits DeviceGenerique_ValueDouble

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "COMPTEUR"
                AddHandler MyTimer.Elapsed, AddressOf Read
            End Sub

        End Class

        <Serializable()> Class CONTACT
            Inherits DeviceGenerique_ValueBool

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "CONTACT"
                _valuemustchange = True 'on ne prend en compte que si la value change
                AddHandler MyTimer.Elapsed, AddressOf Read
            End Sub

        End Class

        <Serializable()> Class DETECTEUR
            Inherits DeviceGenerique_ValueBool

            'Creation du device
            Public Sub New(ByVal server As Server)
                _Server = server
                _Type = "DETECTEUR"
                AddHandler MyTimer.Elapsed, AddressOf Read
            End Sub

        End Class

        <Serializable()> Class DIRECTIONVENT
            Inherits DeviceGenerique_ValueString

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "DIRECTIONVENT"
                AddHandler MyTimer.Elapsed, AddressOf Read
            End Sub

        End Class

        <Serializable()> Class ENERGIEINSTANTANEE
            Inherits DeviceGenerique_ValueDouble

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "ENERGIEINSTANTANEE"
                AddHandler MyTimer.Elapsed, AddressOf Read
            End Sub

        End Class

        <Serializable()> Class ENERGIETOTALE
            Inherits DeviceGenerique_ValueDouble

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "ENERGIETOTALE"
                AddHandler MyTimer.Elapsed, AddressOf Read
            End Sub

        End Class

        <Serializable()> Class GENERIQUEBOOLEEN
            Inherits DeviceGenerique_ValueBool

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "GENERIQUEBOOLEEN"
                AddHandler MyTimer.Elapsed, AddressOf Read
            End Sub

            'ON
            Public Sub [ON]()
                If _Enable = False Then Exit Sub
                Driver.Write(Me, "ON")
            End Sub

            'OFF
            Public Sub OFF()
                If _Enable = False Then Exit Sub
                Driver.Write(Me, "OFF")
            End Sub

        End Class

        <Serializable()> Class GENERIQUESTRING
            Inherits DeviceGenerique_ValueString

            'Creation du device
            Public Sub New(ByVal server As Server)
                _Server = server
                _Type = "GENERIQUESTRING"
                AddHandler MyTimer.Elapsed, AddressOf Read
            End Sub

        End Class

        <Serializable()> Class GENERIQUEVALUE
            Inherits DeviceGenerique_ValueDouble

            'Creation d'un device Temperature
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "GENERIQUEVALUE"
                AddHandler MyTimer.Elapsed, AddressOf Read
            End Sub

        End Class

        <Serializable()> Class FREEBOX
            Inherits DeviceGenerique_ValueString

            Dim _AdresseBox As String = "http://hd1.freebox.fr"

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "FREEBOX"
                Adresse1 = _AdresseBox
                AddHandler MyTimer.Elapsed, AddressOf Read
            End Sub

            'redefinition de read pour ne rien faire :)
            Public Overrides Sub Read()
                If _Enable = False Then Exit Sub
            End Sub

            Private Function Sendhttp(ByVal cmd As String) As String
                Dim URL As String = ""
                Dim Box As String

                Try
                    If Adresse1 = "" Then
                        Box = _AdresseBox
                    Else
                        Box = Adresse1
                    End If
                    Box &= "/pub/remote_control?key="
                    URL = Box & cmd & "&code=" & Adresse2
                    Dim request As WebRequest = WebRequest.Create(URL)
                    Dim response As WebResponse = request.GetResponse()
                    Dim reader As StreamReader = New StreamReader(response.GetResponseStream())
                    Dim str As String = reader.ReadToEnd
                    reader.Close()
                    Return str
                Catch ex As Exception
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DEVICE, "FREEBOX SendHttp", "Commande: " & URL & " Erreur: " & ex.ToString)
                    Return ""
                End Try
            End Function

            'function generique pour toutes les touches appelé par les fonctions touchexxx
            Private Sub Touche(ByVal commande As String)
                Try
                    Dim retour As String = Sendhttp(commande)
                    If retour <> "" Then Value = commande
                Catch ex As Exception
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DEVICE, Me.Name, " Touche" & commande & " : " & ex.ToString)
                End Try
            End Sub

            Public Sub Touche0()
                Touche("0")
            End Sub

            Public Sub Touche1()
                Touche("1")
            End Sub

            Public Sub Touche2()
                Touche("2")
            End Sub

            Public Sub Touche3()
                Touche("3")
            End Sub

            Public Sub Touche4()
                Touche("4")
            End Sub

            Public Sub Touche5()
                Touche("5")
            End Sub

            Public Sub Touche6()
                Touche("6")
            End Sub

            Public Sub Touche7()
                Touche("7")
            End Sub

            Public Sub Touche8()
                Touche("8")
            End Sub

            Public Sub Touche9()
                Touche("9")
            End Sub

            Public Sub VolumeUp()
                Touche("vol_inc")
            End Sub

            Public Sub VolumeDown()
                Touche("vol_dec")
            End Sub

            Public Sub OK()
                Touche("ok")
            End Sub

            Public Sub HAUT()
                Touche("up")
            End Sub

            Public Sub BAS()
                Touche("down")
            End Sub

            Public Sub GAUCHE()
                Touche("left")
            End Sub

            Public Sub DROITE()
                Touche("right")
            End Sub

            Public Sub MUTE()
                Touche("mute")
            End Sub

            Public Sub HOME()
                Touche("home")
            End Sub

            Public Sub ENREGISTRER()
                Touche("rec")
            End Sub

            Public Sub RETOUR()
                Touche("bwd")
            End Sub

            Public Sub PRECEDENT()
                Touche("prev")
            End Sub

            Public Sub PLAY()
                Touche("play")
            End Sub

            Public Sub AVANCE()
                Touche("fwd")
            End Sub

            Public Sub SUIVANT()
                Touche("next")
            End Sub

            Public Sub BoutonROUGE()
                Touche("red")
            End Sub

            Public Sub BoutonVERT()
                Touche("green")
            End Sub

            Public Sub BoutonJAUNE()
                Touche("yellow")
            End Sub

            Public Sub BoutonBLEU()
                Touche("blue")
            End Sub

            Public Sub ProgPlus()
                Touche("prgm_inc")
            End Sub

            Public Sub ProgMoins()
                Touche("prgm_dec")
            End Sub
        End Class

        <Serializable()> Class HUMIDITE
            Inherits DeviceGenerique_ValueDouble

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "HUMIDITE"
                AddHandler MyTimer.Elapsed, AddressOf Read
            End Sub

        End Class

        <Serializable()> Class LAMPE
            Inherits DeviceGenerique_ValueInt

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "LAMPE"
                AddHandler MyTimer.Elapsed, AddressOf Read
            End Sub

            'ON
            Public Sub [ON]()
                If _Enable = False Then Exit Sub
                _Driver.Write(Me, "ON")
            End Sub

            'OFF
            Public Sub OFF()
                If _Enable = False Then Exit Sub
                Driver.Write(Me, "OFF")
            End Sub

            'DIM
            Public Sub [DIM](ByVal Variation As Integer)
                If Variation < 0 Then
                    Variation = 0
                ElseIf Variation > 100 Then
                    Variation = 100
                End If
                If _Enable = False Then Exit Sub
                Driver.Write(Me, "DIM", Variation)
            End Sub

        End Class

        <Serializable()> Class METEO
            Inherits DeviceGenerique
            Dim _Value As String = ""
            Dim _ValueLast As String = ""
            Dim _ConditionActuel As String = ""
            Dim _TempActuel As String = ""
            Dim _HumActuel As String = ""
            Dim _IconActuel As String = ""
            Dim _VentActuel As String = ""
            Dim _JourToday As String = ""
            Dim _MinToday As String = ""
            Dim _MaxToday As String = ""
            Dim _IconToday As String = ""
            Dim _ConditionToday As String = ""
            Dim _JourJ1 As String = ""
            Dim _MinJ1 As String = ""
            Dim _MaxJ1 As String = ""
            Dim _IconJ1 As String = ""
            Dim _ConditionJ1 As String = ""
            Dim _JourJ2 As String = ""
            Dim _MinJ2 As String = ""
            Dim _MaxJ2 As String = ""
            Dim _IconJ2 As String = ""
            Dim _ConditionJ2 As String = ""
            Dim _JourJ3 As String = ""
            Dim _MinJ3 As String = ""
            Dim _MaxJ3 As String = ""
            Dim _IconJ3 As String = ""
            Dim _ConditionJ3 As String = ""

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "METEO"
                AddHandler MyTimer.Elapsed, AddressOf Read
            End Sub

            Public Event DeviceChanged(ByVal device As Object, ByVal [Property] As String, ByVal Parametre As Object)

            'Si X= 0 le serveur attend un event du driver pour mettre à jour la value du device (Cas du RFXCOM)
            'Si X>0 (cas du 1wire par ex) un timer propre au device se lance et effectue un mondevicetemp.Driver.ReadTemp(Me), le driver récupère l’adresse sur l’objet Me sachant que c’est un ReadTemp (donc température) va lire une température à l’adresse spécifié. Cependant un event d’un driver peut modifier la value d’un device même si un refresh a été paramétré
            Public Property Refresh() As Double
                Get
                    Return _Refresh
                End Get
                Set(ByVal value As Double)
                    setrefresh(value)
                End Set
            End Property

            Public Sub Read()
                If _Enable = False Then Exit Sub
                If Driver.IsConnect() And _Server.Etat_server Then Driver.Read(Me)
            End Sub

            'Contien l'avant derniere valeur
            Public Property ValueLast() As String
                Get
                    Return _ValueLast
                End Get
                Set(ByVal value As String)
                    _ValueLast = value
                End Set
            End Property

            Public Property Value() As String
                Get
                    Return _Value
                End Get
                Set(ByVal tmp As String)
                    Try
                        'si le serveur n'a pas fini de démarrer, on affecte juste la valeur
                        If Not _Server.Etat_server Then
                            _Value = tmp
                            _ValueLast = tmp
                        Else
                            _LastChange = Now
                            'Si la valeur a changé on la prend en compte et on créer l'event
                            If tmp <> _Value Then
                                _Server.Log(TypeLog.VALEUR_CHANGE, TypeSource.DEVICE, "DeviceMETEO Value", Name & " : " & Adresse1 & " : " & tmp)
                                _ValueLast = _Value 'on garde l'ancienne valeur en memoire
                                _Value = tmp
                                RaiseEvent DeviceChanged(Me, "Value", _Value)
                            Else
                                _Server.Log(TypeLog.VALEUR_INCHANGE, TypeSource.DEVICE, "DeviceMETEO Value", Name & " : " & Adresse1 & " : " & tmp & " (inchangé)")
                            End If
                        End If
                    Catch ex As Exception
                        _Server.Log(TypeLog.ERREUR, TypeSource.DEVICE, "DeviceMETEO Value", "Exception : " & ex.Message)
                    End Try
                End Set
            End Property

            Public Property ConditionActuel() As String
                Get
                    Return _ConditionActuel
                End Get
                Set(ByVal tmp As String)
                    Try
                        'si le serveur n'a pas fini de démarrer, on affecte juste la valeur
                        If Not _Server.Etat_server Then
                            _ConditionActuel = tmp
                        Else
                            If _ConditionActuel <> tmp Then
                                _Server.Log(TypeLog.VALEUR_CHANGE, TypeSource.DEVICE, "DeviceMETEO ConditionActuel", Name & " : " & Adresse1 & " : " & tmp)
                                _ConditionActuel = tmp
                                RaiseEvent DeviceChanged(Me, "ConditionActuel", tmp)
                            Else
                                _Server.Log(TypeLog.VALEUR_INCHANGE, TypeSource.DEVICE, "DeviceMETEO ConditionActuel", Name & " : " & Adresse1 & " : " & tmp & " (inchangé)")
                            End If
                        End If
                    Catch ex As Exception
                        _Server.Log(TypeLog.ERREUR, TypeSource.DEVICE, "DeviceMETEO ConditionActuel", "Exception : " & ex.Message)
                    End Try
                End Set
            End Property

            Public Property TemperatureActuel() As String
                Get
                    Return _TempActuel
                End Get
                Set(ByVal tmp As String)
                    Try
                        'si le serveur n'a pas fini de démarrer, on affecte juste la valeur
                        If Not _Server.Etat_server Then
                            _TempActuel = tmp
                        Else
                            If _TempActuel <> tmp Then
                                _Server.Log(TypeLog.VALEUR_CHANGE, TypeSource.DEVICE, "DeviceMETEO ConditionActuel", Name & " : " & Adresse1 & " : " & tmp)
                                _TempActuel = tmp
                                RaiseEvent DeviceChanged(Me, "TemperatureActuel", tmp)
                            Else
                                _Server.Log(TypeLog.VALEUR_INCHANGE, TypeSource.DEVICE, "DeviceMETEO TemperatureActuel", Name & " : " & Adresse1 & " : " & tmp & " (inchangé)")
                            End If
                        End If
                    Catch ex As Exception
                        _Server.Log(TypeLog.ERREUR, TypeSource.DEVICE, "DeviceMETEO TemperatureActuel", "Exception : " & ex.Message)
                    End Try
                End Set
            End Property

            Public Property HumiditeActuel() As String
                Get
                    Return _HumActuel
                End Get
                Set(ByVal tmp As String)
                    Try
                        'si le serveur n'a pas fini de démarrer, on affecte juste la valeur
                        If Not _Server.Etat_server Then
                            _HumActuel = tmp
                        Else
                            If _HumActuel <> tmp Then
                                _Server.Log(TypeLog.VALEUR_CHANGE, TypeSource.DEVICE, "DeviceMETEO HumiditeActuel", Name & " : " & Adresse1 & " : " & tmp)
                                _HumActuel = tmp
                                RaiseEvent DeviceChanged(Me, "HumiditeActuel", tmp)
                            Else
                                _Server.Log(TypeLog.VALEUR_INCHANGE, TypeSource.DEVICE, "DeviceMETEO HumiditeActuel", Name & " : " & Adresse1 & " : " & tmp & " (inchangé)")
                            End If
                        End If
                    Catch ex As Exception
                        _Server.Log(TypeLog.ERREUR, TypeSource.DEVICE, "DeviceMETEO HumiditeActuel", "Exception : " & ex.Message)
                    End Try
                End Set
            End Property

            Public Property IconActuel() As String
                Get
                    Return _IconActuel
                End Get
                Set(ByVal value As String)
                    _IconActuel = value
                End Set
            End Property

            Public Property VentActuel() As String
                Get
                    Return _VentActuel
                End Get
                Set(ByVal tmp As String)
                    Try
                        'si le serveur n'a pas fini de démarrer, on affecte juste la valeur
                        If Not _Server.Etat_server Then
                            _VentActuel = tmp
                        Else
                            If _VentActuel <> tmp Then
                                _Server.Log(TypeLog.VALEUR_CHANGE, TypeSource.DEVICE, "DeviceMETEO VentActuel", Name & " : " & Adresse1 & " : " & tmp)
                                _VentActuel = tmp
                                RaiseEvent DeviceChanged(Me, "VentActuel", tmp)
                            Else
                                _Server.Log(TypeLog.VALEUR_INCHANGE, TypeSource.DEVICE, "DeviceMETEO VentActuel", Name & " : " & Adresse1 & " : " & tmp & " (inchangé)")
                            End If
                        End If
                    Catch ex As Exception
                        _Server.Log(TypeLog.ERREUR, TypeSource.DEVICE, "DeviceMETEO VentActuel", "Exception : " & ex.Message)
                    End Try
                End Set
            End Property

            Public Property JourToday() As String
                Get
                    Return _JourToday
                End Get
                Set(ByVal tmp As String)
                    Try
                        'si le serveur n'a pas fini de démarrer, on affecte juste la valeur
                        If Not _Server.Etat_server Then
                            _JourToday = tmp
                        Else
                            If _JourToday <> tmp Then
                                _Server.Log(TypeLog.VALEUR_CHANGE, TypeSource.DEVICE, "DeviceMETEO JourToday", Name & " : " & Adresse1 & " : " & tmp)
                                _JourToday = tmp
                                'RaiseEvent DeviceChanged(Me, "JourToday", tmp)
                            Else
                                _Server.Log(TypeLog.VALEUR_INCHANGE, TypeSource.DEVICE, "DeviceMETEO JourToday", Name & " : " & Adresse1 & " : " & tmp & " (inchangé)")
                            End If
                        End If
                    Catch ex As Exception
                        _Server.Log(TypeLog.ERREUR, TypeSource.DEVICE, "DeviceMETEO JourToday", "Exception : " & ex.Message)
                    End Try
                End Set
            End Property

            Public Property MinToday() As String
                Get
                    Return _MinToday
                End Get
                Set(ByVal tmp As String)
                    Try
                        'si le serveur n'a pas fini de démarrer, on affecte juste la valeur
                        If Not _Server.Etat_server Then
                            _MinToday = tmp
                        Else
                            If _MinToday <> tmp Then
                                _Server.Log(TypeLog.VALEUR_CHANGE, TypeSource.DEVICE, "DeviceMETEO MinToday", Name & " : " & Adresse1 & " : " & tmp)
                                _MinToday = tmp
                                RaiseEvent DeviceChanged(Me, "MinToday", tmp)
                            Else
                                _Server.Log(TypeLog.VALEUR_INCHANGE, TypeSource.DEVICE, "DeviceMETEO MinToday", Name & " : " & Adresse1 & " : " & tmp & " (inchangé)")
                            End If
                        End If
                    Catch ex As Exception
                        _Server.Log(TypeLog.ERREUR, TypeSource.DEVICE, "DeviceMETEO MinToday", "Exception : " & ex.Message)
                    End Try
                End Set
            End Property

            Public Property MaxToday() As String
                Get
                    Return _MaxToday
                End Get
                Set(ByVal tmp As String)
                    Try
                        'si le serveur n'a pas fini de démarrer, on affecte juste la valeur
                        If Not _Server.Etat_server Then
                            _MaxToday = tmp
                        Else
                            If _MaxToday <> tmp Then
                                _Server.Log(TypeLog.VALEUR_CHANGE, TypeSource.DEVICE, "DeviceMETEO MaxToday", Name & " : " & Adresse1 & " : " & tmp)
                                _MaxToday = tmp
                                RaiseEvent DeviceChanged(Me, "MaxToday", tmp)
                            Else
                                _Server.Log(TypeLog.VALEUR_INCHANGE, TypeSource.DEVICE, "DeviceMETEO MaxToday", Name & " : " & Adresse1 & " : " & tmp & " (inchangé)")
                            End If
                        End If
                    Catch ex As Exception
                        _Server.Log(TypeLog.ERREUR, TypeSource.DEVICE, "DeviceMETEO MaxToday", "Exception : " & ex.Message)
                    End Try
                End Set
            End Property

            Public Property IconToday() As String
                Get
                    Return _IconToday
                End Get
                Set(ByVal value As String)
                    _IconToday = value
                End Set
            End Property

            Public Property ConditionToday() As String
                Get
                    Return _ConditionToday
                End Get
                Set(ByVal tmp As String)
                    Try
                        'si le serveur n'a pas fini de démarrer, on affecte juste la valeur
                        If Not _Server.Etat_server Then
                            _ConditionToday = tmp
                        Else
                            If _ConditionToday <> tmp Then
                                _Server.Log(TypeLog.VALEUR_CHANGE, TypeSource.DEVICE, "DeviceMETEO ConditionToday", Name & " : " & Adresse1 & " : " & tmp)
                                _ConditionToday = tmp
                                RaiseEvent DeviceChanged(Me, "ConditionToday", tmp)
                            Else
                                _Server.Log(TypeLog.VALEUR_INCHANGE, TypeSource.DEVICE, "DeviceMETEO ConditionToday", Name & " : " & Adresse1 & " : " & tmp & " (inchangé)")
                            End If
                        End If
                    Catch ex As Exception
                        _Server.Log(TypeLog.ERREUR, TypeSource.DEVICE, "DeviceMETEO ConditionToday", "Exception : " & ex.Message)
                    End Try
                End Set
            End Property

            Public Property JourJ1() As String
                Get
                    Return _JourJ1
                End Get
                Set(ByVal value As String)
                    If _JourJ1 <> value Then
                        _JourJ1 = value
                        'RaiseEvent DeviceChanged(Me, "JourJ1", value)
                    End If
                End Set
            End Property

            Public Property MinJ1() As String
                Get
                    Return _MinJ1
                End Get
                Set(ByVal value As String)
                    If MinJ1 <> value Then
                        _MinJ1 = value
                        'RaiseEvent DeviceChanged(Me, "MinJ1", value)
                    End If
                End Set
            End Property

            Public Property MaxJ1() As String
                Get
                    Return _MaxJ1
                End Get
                Set(ByVal value As String)
                    If _MaxJ1 <> value Then
                        _MaxJ1 = value
                         'RaiseEvent DeviceChanged(Me, "MaxJ1", value)
                    End If
                End Set
            End Property

            Public Property IconJ1() As String
                Get
                    Return _IconJ1
                End Get
                Set(ByVal value As String)
                    _IconJ1 = value
                End Set
            End Property

            Public Property ConditionJ1() As String
                Get
                    Return _ConditionJ1
                End Get
                Set(ByVal value As String)
                    If _ConditionJ1 <> value Then
                        _ConditionJ1 = value
                        ' RaiseEvent DeviceChanged(Me, "ConditionJ1", value)
                    End If
                End Set
            End Property

            Public Property JourJ2() As String
                Get
                    Return _JourJ2
                End Get
                Set(ByVal value As String)
                    If _JourJ2 <> value Then
                        _JourJ2 = value
                        'RaiseEvent DeviceChanged(Me, "JourJ2", value)
                    End If
                End Set
            End Property

            Public Property MinJ2() As String
                Get
                    Return _MinJ2
                End Get
                Set(ByVal value As String)
                    If _MinJ2 <> value Then
                        _MinJ2 = value
                        'RaiseEvent DeviceChanged(Me, "MinJ2", value)
                    End If
                End Set
            End Property

            Public Property MaxJ2() As String
                Get
                    Return _MaxJ2
                End Get
                Set(ByVal value As String)
                    If _MaxJ2 <> value Then
                        _MaxJ2 = value
                        'RaiseEvent DeviceChanged(Me, "MaxJ2", value)
                    End If
                End Set
            End Property

            Public Property IconJ2() As String
                Get
                    Return _IconJ2
                End Get
                Set(ByVal value As String)
                    _IconJ2 = value
                End Set
            End Property

            Public Property ConditionJ2() As String
                Get
                    Return _ConditionJ2
                End Get
                Set(ByVal value As String)
                    If _ConditionJ2 <> value Then
                        _ConditionJ2 = value
                        'RaiseEvent DeviceChanged(Me, "ConditionJ2", value)
                    End If
                End Set
            End Property

            Public Property JourJ3() As String
                Get
                    Return _JourJ3
                End Get
                Set(ByVal value As String)
                    If _JourJ3 <> value Then
                        _JourJ3 = value
                        'RaiseEvent DeviceChanged(Me, "JourJ3", value)
                    End If
                End Set
            End Property

            Public Property MinJ3() As String
                Get
                    Return _MinJ3
                End Get
                Set(ByVal value As String)
                    If _MinJ3 <> value Then
                        _MinJ3 = value
                        'RaiseEvent DeviceChanged(Me, "MinJ3", value)
                    End If
                End Set
            End Property

            Public Property MaxJ3() As String
                Get
                    Return _MaxJ3
                End Get
                Set(ByVal value As String)
                    If _MaxJ3 <> value Then
                        _MaxJ3 = value
                        'RaiseEvent DeviceChanged(Me, "MaxJ3", value)
                    End If
                End Set
            End Property

            Public Property IconJ3() As String
                Get
                    Return _IconJ3
                End Get
                Set(ByVal value As String)
                    _IconJ3 = value
                End Set
            End Property

            Public Property ConditionJ3() As String
                Get
                    Return _ConditionJ3
                End Get
                Set(ByVal value As String)
                    If _ConditionJ3 <> value Then
                        _ConditionJ3 = value
                        'RaiseEvent DeviceChanged(Me, "ConditionJ3", value)
                    End If
                End Set
            End Property

            Protected Overrides Sub Finalize()
                MyBase.Finalize()
                MyTimer.Enabled = false
            End Sub
        End Class

        <Serializable()> Class MULTIMEDIA
            Inherits DeviceGenerique_ValueString

            Public ListCommandName As New ArrayList
            Public ListCommandData As New ArrayList
            Public ListCommandRepeat As New ArrayList

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "MULTIMEDIA"
                AddHandler MyTimer.Elapsed, AddressOf Read

                ListCommandName.Add("Power")
                ListCommandData.Add("0")
                ListCommandRepeat.Add("0")
                ListCommandName.Add("ChannelUp")
                ListCommandData.Add("0")
                ListCommandRepeat.Add("0")
                ListCommandName.Add("ChannelDown")
                ListCommandData.Add("0")
                ListCommandRepeat.Add("0")
                ListCommandName.Add("VolumeUp")
                ListCommandData.Add("0")
                ListCommandRepeat.Add("0")
                ListCommandName.Add("VolumeDown")
                ListCommandData.Add("0")
                ListCommandRepeat.Add("0")
                ListCommandName.Add("Mute")
                ListCommandData.Add("0")
                ListCommandRepeat.Add("0")
                ListCommandName.Add("Source")
                ListCommandData.Add("0")
                ListCommandRepeat.Add("0")
                ListCommandName.Add("0")
                ListCommandData.Add("0")
                ListCommandRepeat.Add("0")
                ListCommandName.Add("1")
                ListCommandData.Add("0")
                ListCommandRepeat.Add("0")
                ListCommandName.Add("2")
                ListCommandData.Add("0")
                ListCommandRepeat.Add("0")
                ListCommandName.Add("3")
                ListCommandData.Add("0")
                ListCommandRepeat.Add("0")
                ListCommandName.Add("4")
                ListCommandData.Add("0")
                ListCommandRepeat.Add("0")
                ListCommandName.Add("5")
                ListCommandData.Add("0")
                ListCommandRepeat.Add("0")
                ListCommandName.Add("6")
                ListCommandData.Add("0")
                ListCommandRepeat.Add("0")
                ListCommandName.Add("7")
                ListCommandData.Add("0")
                ListCommandRepeat.Add("0")
                ListCommandName.Add("8")
                ListCommandData.Add("0")
                ListCommandRepeat.Add("0")
                ListCommandName.Add("9")
                ListCommandData.Add("0")
                ListCommandRepeat.Add("0")
            End Sub

            'redéfinition car on veut rien faire
            Public Overrides Sub Read()
                If _Enable = False Then Exit Sub
            End Sub

            Public Sub SendCommand(ByVal NameCommand As String)
                For i As Integer = 0 To ListCommandName.Count - 1
                    If ListCommandName(i) = NameCommand Then
                        If _Enable = False Then Exit Sub
                        Driver.Write(Me, "SendCodeIR", ListCommandData(i), ListCommandRepeat(i))
                        Exit For
                    End If
                Next
            End Sub

            Protected Overrides Sub Finalize()
                MyBase.Finalize()
                MyTimer.Enabled = False
            End Sub
        End Class

        <Serializable()> Class PLUIECOURANT
            Inherits DeviceGenerique_ValueDouble

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "PLUIECOURANT"
                AddHandler MyTimer.Elapsed, AddressOf Read
            End Sub

        End Class

        <Serializable()> Class PLUIETOTAL
            Inherits DeviceGenerique_ValueDouble

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "PLUIETOTAL"
                AddHandler MyTimer.Elapsed, AddressOf Read
            End Sub

        End Class

        <Serializable()> Class SWITCH
            Inherits DeviceGenerique_ValueBool

            'Creation du device
            Public Sub New(ByVal server As Server)
                _Server = server
                _Type = "SWITCH"
                AddHandler MyTimer.Elapsed, AddressOf Read
            End Sub

            'ON
            Public Sub [ON]()
                If _Enable = False Then Exit Sub
                Driver.Write(Me, "ON")
            End Sub

            'OFF
            Public Sub OFF()
                If _Enable = False Then Exit Sub
                Driver.Write(Me, "OFF")
            End Sub
        End Class

        <Serializable()> Class TELECOMMANDE
            Inherits DeviceGenerique_ValueString

            'Creation du device
            Public Sub New(ByVal server As Server)
                _Server = server
                _Type = "TELECOMMANDE"
                AddHandler MyTimer.Elapsed, AddressOf Read
            End Sub

            'redéfinition car on veut rien faire
            Public Overrides Sub Read()
                If _Enable = False Then Exit Sub
            End Sub

        End Class

        <Serializable()> Class TEMPERATURE
            Inherits DeviceGenerique_ValueDouble

            'Creation d'un device Temperature
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "TEMPERATURE"
                AddHandler MyTimer.Elapsed, AddressOf Read
            End Sub

        End Class

        <Serializable()> Class TEMPERATURECONSIGNE
            Inherits DeviceGenerique_ValueDouble

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "TEMPERATURECONSIGNE"
                AddHandler MyTimer.Elapsed, AddressOf Read
            End Sub

        End Class

        <Serializable()> Class UV
            Inherits DeviceGenerique_ValueDouble

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "UV"
                AddHandler MyTimer.Elapsed, AddressOf Read
            End Sub

        End Class

        <Serializable()> Class VITESSEVENT
            Inherits DeviceGenerique_ValueDouble

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "VITESSEVENT"
                AddHandler MyTimer.Elapsed, AddressOf Read
            End Sub

        End Class

        <Serializable()> Class VOLET
            Inherits DeviceGenerique_ValueInt

            'Creation du device
            Public Sub New(ByVal server As Server)
                _Server = server
                _Type = "VOLET"
                AddHandler MyTimer.Elapsed, AddressOf Read
            End Sub

            'Ouvrir volet
            Public Sub OPEN()
                If _Enable = False Then Exit Sub
                Driver.Write(Me, "ON")
            End Sub

            'Fermer Volet
            Public Sub CLOSE()
                If _Enable = False Then Exit Sub
                Driver.Write(Me, "OFF")
            End Sub

            'Ouvrir/Fermer % Volet
            Public Sub [OUVERTURE](ByVal Variation As Integer)
                If Variation < 0 Then
                    Variation = 0
                ElseIf Variation > 100 Then
                    Variation = 100
                End If
                If _Enable = False Then Exit Sub
                Driver.Write(Me, "OUVERTURE", Variation)
            End Sub

        End Class

    End Class
End Namespace