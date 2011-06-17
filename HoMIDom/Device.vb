Imports System.Net
Imports System.IO

Namespace HoMIDom
    '***********************************************
    '** CLASS DEVICE
    '** version 1.1
    '** Date de création: 12/01/2011
    '** Historique (SebBergues): 12/01/2011: Création 
    '** Historique (Davidinfo): 17/01/2011: Ajout de classes generiques + ajout _formatage
    '** Historique (SebBergues: 19/01/2011: Ecriture ou lecture via Read/Write + ajout proriété Solo
    '***********************************************

    ''' <summary>Class Device, définie tous différents types de devices</summary>
    ''' <remarks></remarks>
    Public Class Device

        Dim _Server As Server

        ''' <summary>Indique la liste des devices gérés</summary>
        ''' <remarks></remarks>
        Public Enum ListeDevices
            APPAREIL = 1 'modules pour diriger un appareil  ON/OFF
            AUDIO = 2
            BAROMETRE = 3 'pour stocker les valeur issu d'un barometre meteo ou web
            BATTERIE = 4
            COMPTEUR = 5 'compteur DS2423, RFXPower...
            CONTACT = 6 'detecteur de contact : switch 1-wire
            DETECTEUR = 7 'tous detecteurs : mouvement, obscurite...
            DIRECTIONVENT = 8
            ENERGIEINSTANTANEE = 9
            ENERGIETOTALE = 10
            FREEBOX = 11
            GENERIQUEBOOLEEN = 12
            GENERIQUESTRING = 13
            GENERIQUEVALUE = 14
            HUMIDITE = 15
            LAMPE = 16
            METEO = 17
            MULTIMEDIA = 18
            PLUIECOURANT = 19
            PLUIETOTAL = 20
            SWITCH = 21
            TELECOMMANDE = 22
            TEMPERATURE = 23
            TEMPERATURECONSIGNE = 24
            UV = 25
            VITESSEVENT = 26
            VOLET = 27
        End Enum

        ''' <summary>Class de déclaration du Device Générique</summary>
        ''' <remarks></remarks>
        <Serializable()> Public MustInherit Class DeviceGenerique
            Protected _Server As Server
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
            Protected _LastEtat As Boolean = False
            Protected MyTimer As New Timers.Timer

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
                    _Driver = _Server.ReturnDrvById(DriverID)
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
                    _Picture = value
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

            Protected Overrides Sub Finalize()
                MyBase.Finalize()
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
                    _Refresh = value
                    If _Refresh > 0 Then
                        MyTimer.Interval = _Refresh
                        MyTimer.Enabled = True
                        AddHandler MyTimer.Elapsed, AddressOf Read
                    End If
                End Set
            End Property

            Public Sub Read()
                If _Enable = False Then Exit Sub
                If Driver.IsConnect() Then Driver.Read(Me)
            End Sub

            'Valeur
            Public Property Value() As Double
                Get
                    Return _Value
                End Get
                Set(ByVal value As Double)
                    Dim tmp As Double = value
                    _LastChange = Now
                    If tmp < _ValueMin Then tmp = _ValueMin
                    If tmp > _ValueMax Then tmp = _ValueMax
                    If _Formatage <> "" Then tmp = Format(tmp, _Formatage)
                    tmp += _Correction
                    'Si la valeur a changé on la prend en compte et on créer l'event
                    'MsgBox("Device double a recu une value : " & _Name & " - " & tmp)
                    If tmp <> _Value Then
                        _ValueLast = _Value 'on garde l'ancienne value en memoire
                        _Value = tmp
                        RaiseEvent DeviceChanged(Me, "Value", _Value)
                    Else
                        _Server.Log(Server.TypeLog.VALEUR_INCHANGE, Server.TypeSource.SERVEUR, "DeviceChange", _Name & " : " & _Adresse1 & " : " & _Value)
                    End If
                End Set
            End Property
        End Class

        ''' <summary>Classe valeur True/False pour device ON/OFF</summary>
        ''' <remarks></remarks>
        <Serializable()> Public Class DeviceGenerique_ValueBool
            Inherits DeviceGenerique

            Protected _Value As Boolean = False
            Protected _ValueLast As Double = 0

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
                    _Refresh = value
                    If _Refresh > 0 Then
                        MyTimer.Interval = _Refresh
                        MyTimer.Enabled = True
                        AddHandler MyTimer.Elapsed, AddressOf Read
                    End If
                End Set
            End Property

            'Demande de Lecture au driver
            Public Sub Read()
                If _Enable = False Then Exit Sub
                If Driver.IsConnect() Then Driver.Read(Me)
            End Sub

            'Valeur : ON/OFF = True/False
            Public Property Value() As Boolean
                Get
                    Return _Value
                End Get
                Set(ByVal value As Boolean)
                    Dim tmp As Boolean = value
                    _LastChange = Now
                    _ValueLast = _Value 'on garde l'ancienne value en memoire
                    _Value = tmp 'on prend en compte la value à chaque fois car on peut donne le même ordre plusieurs fois
                    RaiseEvent DeviceChanged(Me, "Value", _Value)
                End Set
            End Property

        End Class

        ''' <summary>Classe valeur Integer pour device avce valeur de 0(OFF) à 100(ON)</summary>
        ''' <remarks></remarks>
        <Serializable()> Public MustInherit Class DeviceGenerique_ValueInt
            Inherits DeviceGenerique

            Protected _Value As Integer = 0
            Protected _ValueLast As Integer = 0
            Protected _ValueMin As Integer = -9999
            Protected _ValueMax As Integer = 9999
            Protected _ValueDef As Integer = 0
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

            'Si X= 0 le serveur attend un event du driver pour mettre à jour la value du device (Cas du RFXCOM)
            'Si X>0 (cas du 1wire par ex) un timer propre au device se lance et effectue un mondevicetemp.Driver.ReadTemp(Me), le driver récupère l’adresse sur l’objet Me sachant que c’est un ReadTemp (donc température) va lire une température à l’adresse spécifié. Cependant un event d’un driver peut modifier la value d’un device même si un refresh a été paramétré
            Public Property Refresh() As Integer
                Get
                    Return _Refresh
                End Get
                Set(ByVal value As Integer)
                    _Refresh = value
                    If _Refresh > 0 Then
                        MyTimer.Interval = _Refresh
                        MyTimer.Enabled = True
                        AddHandler MyTimer.Elapsed, AddressOf Read
                    End If
                End Set
            End Property

            'Demande de Lecture au driver
            Public Sub Read()
                If _Enable = False Then Exit Sub
                If Driver.IsConnect() Then Driver.Read(Me)
            End Sub

            'Valeur de 0 à 100
            Public Property Value() As Integer
                Get
                    Return _Value
                End Get
                Set(ByVal value As Integer)
                    Dim tmp As Integer = value
                    _LastChange = Now
                    If tmp < 0 Then tmp = 0
                    If tmp > 100 Then tmp = 100
                    _ValueLast = _Value 'on garde l'ancienne value en memoire
                    _Value = tmp 'on prend en compte la value à chaque fois car on peut donne le même ordre plusieurs fois
                    RaiseEvent DeviceChanged(Me, "Value", _Value)
                End Set
            End Property

        End Class

        ''' <summary>Classe valeur String pour device style Direction du Vent</summary>
        ''' <remarks></remarks>
        <Serializable()> Public Class DeviceGenerique_ValueString
            Inherits DeviceGenerique

            Protected _Value As String = ""
            Protected _ValueLast As Double = 0

            'Contien l'avant derniere valeur
            Public Property ValueLast() As Double
                Get
                    Return _ValueLast
                End Get
                Set(ByVal value As Double)
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
                    _Refresh = value
                    If _Refresh > 0 Then
                        MyTimer.Interval = _Refresh
                        MyTimer.Enabled = True
                        AddHandler MyTimer.Elapsed, AddressOf Read
                    End If
                End Set
            End Property

            Private Sub Read()
                If _Enable = False Then Exit Sub
                If Driver.IsConnect() Then Driver.Read(Me)
            End Sub

            Public Property Value() As String
                Get
                    Return _Value
                End Get
                Set(ByVal value As String)
                    Dim tmp As String = value
                    _LastChange = Now
                    'Si la valeur a changé on la prend en compte et on créer l'event
                    If tmp <> _Value Then
                        _ValueLast = _Value 'on garde l'ancienne value en memoire
                        _Value = tmp
                        RaiseEvent DeviceChanged(Me, "Value", _Value)
                    Else
                        _Server.Log(Server.TypeLog.VALEUR_INCHANGE, Server.TypeSource.SERVEUR, "DeviceChange", _Name & " : " & _Adresse1 & " : " & _Value)
                    End If
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
            End Sub

            'redéfinition car on ne veut rien faire
            Public Sub Read()
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

            Public Sub Play(ByVal Fichier As String)
                touche("PlayAudio")
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

        End Class

        <Serializable()> Class BAROMETRE
            Inherits DeviceGenerique_ValueDouble

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "BAROMETRE"
            End Sub

        End Class

        <Serializable()> Class BATTERIE
            Inherits DeviceGenerique_ValueString

            'Creation d'un device BATTERIE
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "BATTERIE"
            End Sub

        End Class

        <Serializable()> Class COMPTEUR
            Inherits DeviceGenerique_ValueDouble

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "COMPTEUR"
            End Sub

        End Class

        <Serializable()> Class CONTACT
            Inherits DeviceGenerique_ValueBool

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "CONTACT"
            End Sub

        End Class

        <Serializable()> Class DETECTEUR
            Inherits DeviceGenerique_ValueBool

            'Creation du device
            Public Sub New(ByVal server As Server)
                _Server = server
                _Type = "DETECTEUR"
            End Sub

        End Class

        <Serializable()> Class DIRECTIONVENT
            Inherits DeviceGenerique_ValueString

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "DIRECTIONVENT"
            End Sub

        End Class

        <Serializable()> Class ENERGIEINSTANTANEE
            Inherits DeviceGenerique_ValueDouble

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "ENERGIEINSTANTANEE"
            End Sub

        End Class

        <Serializable()> Class ENERGIETOTALE
            Inherits DeviceGenerique_ValueDouble

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "ENERGIETOTALE"
            End Sub

        End Class

        <Serializable()> Class GENERIQUEBOOLEEN
            Inherits DeviceGenerique_ValueBool

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "GENERIQUEBOOLEEN"
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
            End Sub

        End Class

        <Serializable()> Class GENERIQUEVALUE
            Inherits DeviceGenerique_ValueDouble

            'Creation d'un device Temperature
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "GENERIQUEVALUE"
            End Sub

        End Class

        <Serializable()> Class FREEBOX
            Inherits DeviceGenerique_ValueString

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "FREEBOX"
                _Adresse1 = " http://hd1.freebox.fr/pub/remote_control ?key="
            End Sub

            'redefinition de read pour ne rien faire :)
            Public Sub Read()
                If _Enable = False Then Exit Sub
            End Sub

            Private Function Sendhttp(ByVal cmd As String) As String
                Dim URL As String = Adresse1 & cmd
                Dim request As WebRequest = WebRequest.Create(URL)
                Dim response As WebResponse = request.GetResponse()
                Dim reader As StreamReader = New StreamReader(response.GetResponseStream())
                Dim str As String = reader.ReadToEnd
                'Do While str.Length > 0
                '    Console.WriteLine(str)
                '    str = reader.ReadLine()
                'Loop
                reader.Close()
                Return str
            End Function

            'function generique pour toutes les touches appelé par les fonctions touchexxx
            Private Sub Touche(ByVal commande As String)
                Try
                    Dim retour As String
                    retour = Sendhttp(commande)
                    Value = commande
                Catch ex As Exception
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DEVICE, Me.Name, " Touche" & commande & " : " & ex.Message)
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

        End Class

        <Serializable()> Class HUMIDITE
            Inherits DeviceGenerique_ValueDouble

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "HUMIDITE"
            End Sub

        End Class

        <Serializable()> Class LAMPE
            Inherits DeviceGenerique_ValueInt

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "LAMPE"
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
            Dim _ValueLast As Double
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
            End Sub

            Public Event DeviceChanged(ByVal device As Object, ByVal [Property] As String, ByVal Parametre As Object)

            'Si X= 0 le serveur attend un event du driver pour mettre à jour la value du device (Cas du RFXCOM)
            'Si X>0 (cas du 1wire par ex) un timer propre au device se lance et effectue un mondevicetemp.Driver.ReadTemp(Me), le driver récupère l’adresse sur l’objet Me sachant que c’est un ReadTemp (donc température) va lire une température à l’adresse spécifié. Cependant un event d’un driver peut modifier la value d’un device même si un refresh a été paramétré
            Public Property Refresh() As Double
                Get
                    Return _Refresh
                End Get
                Set(ByVal value As Double)
                    _Refresh = value
                    If _Refresh > 0 Then
                        Driver.Read(Me)
                        LastChange = Now
                        MyTimer.Interval = value * 1000
                        MyTimer.Enabled = True
                        AddHandler MyTimer.Elapsed, AddressOf Read
                    End If
                End Set
            End Property

            Public Sub Read()
                If _Enable = False Then Exit Sub
                Driver.Read(Me)
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
                Set(ByVal value As String)
                    Dim tmp As String = value
                    _LastChange = Now
                    'Si la valeur a changé on la prend en compte et on créer l'event
                    If tmp <> _Value Then
                        _ValueLast = _Value 'on garde l'ancienne valeur en memoire
                        _Value = tmp
                        RaiseEvent DeviceChanged(Me, "Value", _Value)
                    End If
                End Set
            End Property

            Public Property ConditionActuel() As String
                Get
                    Return _ConditionActuel
                End Get
                Set(ByVal value As String)
                    _ConditionActuel = value
                    _LastChange = Now
                    RaiseEvent DeviceChanged(Me, "ConditionActuel", value)
                End Set
            End Property

            Public Property TemperatureActuel() As String
                Get
                    Return _TempActuel
                End Get
                Set(ByVal value As String)
                    _TempActuel = value
                    _LastChange = Now
                    RaiseEvent DeviceChanged(Me, "TemperatureActuel", value)
                End Set
            End Property

            Public Property HumiditeActuel() As String
                Get
                    Return _HumActuel
                End Get
                Set(ByVal value As String)
                    _HumActuel = value
                    _LastChange = Now
                    RaiseEvent DeviceChanged(Me, "HumiditeActuel", value)
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
                Set(ByVal value As String)
                    _VentActuel = value
                    _LastChange = Now
                    RaiseEvent DeviceChanged(Me, "VentActuel", value)
                End Set
            End Property

            Public Property JourToday() As String
                Get
                    Return _JourToday
                End Get
                Set(ByVal value As String)
                    _JourToday = value
                    _LastChange = Now
                    RaiseEvent DeviceChanged(Me, "JourToday", value)
                End Set
            End Property

            Public Property MinToday() As String
                Get
                    Return _MinToday
                End Get
                Set(ByVal value As String)
                    _MinToday = value
                    _LastChange = Now
                    RaiseEvent DeviceChanged(Me, "MinToday", value)
                End Set
            End Property

            Public Property MaxToday() As String
                Get
                    Return _MaxToday
                End Get
                Set(ByVal value As String)
                    _MaxToday = value
                    _LastChange = Now
                    RaiseEvent DeviceChanged(Me, "MaxToday", value)
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
                Set(ByVal value As String)
                    _ConditionToday = value
                    _LastChange = Now
                    RaiseEvent DeviceChanged(Me, "ConditionToday", value)
                End Set
            End Property

            Public Property JourJ1() As String
                Get
                    Return _JourJ1
                End Get
                Set(ByVal value As String)
                    _JourJ1 = value
                    _LastChange = Now
                    RaiseEvent DeviceChanged(Me, "JourJ1", value)
                End Set
            End Property

            Public Property MinJ1() As String
                Get
                    Return _MinJ1
                End Get
                Set(ByVal value As String)
                    _MinJ1 = value
                    _LastChange = Now
                    RaiseEvent DeviceChanged(Me, "MinJ1", value)
                End Set
            End Property

            Public Property MaxJ1() As String
                Get
                    Return _MaxJ1
                End Get
                Set(ByVal value As String)
                    _MaxJ1 = value
                    _LastChange = Now
                    RaiseEvent DeviceChanged(Me, "MaxJ1", value)
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
                    _ConditionJ1 = value
                    _LastChange = Now
                    RaiseEvent DeviceChanged(Me, "ConditionJ1", value)
                End Set
            End Property

            Public Property JourJ2() As String
                Get
                    Return _JourJ2
                End Get
                Set(ByVal value As String)
                    _JourJ2 = value
                    _LastChange = Now
                    RaiseEvent DeviceChanged(Me, "JourJ2", value)
                End Set
            End Property

            Public Property MinJ2() As String
                Get
                    Return _MinJ2
                End Get
                Set(ByVal value As String)
                    _MinJ2 = value
                    _LastChange = Now
                    RaiseEvent DeviceChanged(Me, "MinJ2", value)
                End Set
            End Property

            Public Property MaxJ2() As String
                Get
                    Return _MaxJ2
                End Get
                Set(ByVal value As String)
                    _MaxJ2 = value
                    _LastChange = Now
                    RaiseEvent DeviceChanged(Me, "MaxJ2", value)
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
                    _ConditionJ2 = value
                    _LastChange = Now
                    RaiseEvent DeviceChanged(Me, "ConditionJ2", value)
                End Set
            End Property

            Public Property JourJ3() As String
                Get
                    Return _JourJ3
                End Get
                Set(ByVal value As String)
                    _JourJ3 = value
                    _LastChange = Now
                    RaiseEvent DeviceChanged(Me, "JourJ3", value)
                End Set
            End Property

            Public Property MinJ3() As String
                Get
                    Return _MinJ3
                End Get
                Set(ByVal value As String)
                    _MinJ3 = value
                    _LastChange = Now
                    RaiseEvent DeviceChanged(Me, "MinJ3", value)
                End Set
            End Property

            Public Property MaxJ3() As String
                Get
                    Return _MaxJ3
                End Get
                Set(ByVal value As String)
                    _MaxJ3 = value
                    _LastChange = Now
                    RaiseEvent DeviceChanged(Me, "MaxJ3", value)
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
                    _ConditionJ3 = value
                    _LastChange = Now
                    RaiseEvent DeviceChanged(Me, "ConditionJ3", value)
                End Set
            End Property

            Protected Overrides Sub Finalize()
                MyBase.Finalize()
                MyTimer.Enabled = True
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
            Private Sub Read()
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
            End Sub

        End Class

        <Serializable()> Class PLUIETOTAL
            Inherits DeviceGenerique_ValueDouble

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "PLUIETOTAL"
            End Sub

        End Class

        <Serializable()> Class SWITCH
            Inherits DeviceGenerique_ValueBool

            'Creation du device
            Public Sub New(ByVal server As Server)
                _Server = server
                _Type = "SWITCH"
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
            End Sub

            'redéfinition car on veut rien faire
            Private Sub Read()
                If _Enable = False Then Exit Sub
            End Sub

        End Class

        <Serializable()> Class TEMPERATURE
            Inherits DeviceGenerique_ValueDouble

            'Creation d'un device Temperature
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "TEMPERATURE"
            End Sub

        End Class

        <Serializable()> Class TEMPERATURECONSIGNE
            Inherits DeviceGenerique_ValueDouble

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "TEMPERATURECONSIGNE"
            End Sub

        End Class

        <Serializable()> Class UV
            Inherits DeviceGenerique_ValueDouble

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "UV"
            End Sub

        End Class

        <Serializable()> Class VITESSEVENT
            Inherits DeviceGenerique_ValueDouble

            'Creation du device
            Public Sub New(ByVal Server As Server)
                _Server = Server
                _Type = "VITESSEVENT"
            End Sub

        End Class

        <Serializable()> Class VOLET
            Inherits DeviceGenerique_ValueInt

            'Creation du device
            Public Sub New(ByVal server As Server)
                _Server = server
                _Type = "VOLET"
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

    End Class
End Namespace