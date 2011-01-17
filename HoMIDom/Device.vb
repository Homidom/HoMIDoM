Imports System.Net
Imports System.IO

Namespace HoMIDom
    '***********************************************
    '** CLASS DEVICE
    '** version 1.1
    '** Date de création: 12/01/2011
    '** Historique (SebBergues): 12/01/2011: Création 
    '** Historique (Davidinfo): 17/01/2011: Ajout de classes generiques + ajout _formatage
    '***********************************************

    Public Class Device

        Public Class DeviceGenerique
            Protected _ID As String
            Protected _Name As String
            Protected _Enable As Boolean
            Protected _Driver As Object
            Protected _Description As String
            Protected _Type As String
            Protected _Adresse1 As String
            Protected _Adresse2 As String
            Protected _DateCreated As Date
            Protected _LastChanged As Date
            Protected _Refresh As Integer
            Protected _Modele As String
            Protected _Picture As String
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

            'Driver affecté (représentant l’objet déclaré du driver)
            Public Property Driver() As Object
                Get
                    Return _Driver
                End Get
                Set(ByVal value As Object)
                    _Driver = value
                End Set
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

            'Date et heure du dernier changement de propriétés (Value, Status…) correspondant à l’event généré
            Public Property LastChange() As Date
                Get
                    Return _LastChanged
                End Get
                Set(ByVal value As Date)
                    _LastChanged = value
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

        End Class

        Class DeviceGeneriqueValue
            Inherits DeviceGenerique

            Protected _Value As Double
            Protected _ValueMin As Double = -9999
            Protected _ValueMax As Double = 9999
            Protected _ValueDef As Double
            Protected _Precision As Double
            Protected _Correction As Double
            Protected _Formatage As String

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
            Public Property Precision() As String
                Get
                    Return _Precision
                End Get
                Set(ByVal value As String)
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

        End Class

        <Serializable()> Class TEMPERATURE
            Inherits DeviceGeneriqueValue

            'Creation d'un device Temperature
            Public Sub New()
                _Type = "TEMPERATURE"
            End Sub

            Public Event DeviceChanged(ByVal Id As String, ByVal [Property] As String, ByVal Parametre As Object)

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
                        AddHandler MyTimer.Elapsed, AddressOf TimerTick
                    End If
                End Set
            End Property

            Private Sub TimerTick()
                Value = Driver.ReadTemp(Me)
            End Sub

            'Valeur de température
            Public Property Value() As Double
                Get
                    Return _Value
                End Get
                Set(ByVal value As Double)
                    Dim tmp As Double = value
                    If tmp < _ValueMin Then tmp = _ValueMin
                    If tmp > _ValueMax Then tmp = _ValueMax
                    If _Formatage <> "" Then tmp = Format(tmp, _Formatage)
                    tmp += _Correction

                    'Si la valeur a changé on la prend en compte et on créer l'event
                    If tmp <> _Value Then
                        _Value = tmp
                        _LastChanged = Now
                        RaiseEvent DeviceChanged(_ID, "Value", _Value)
                    End If
                End Set
            End Property

        End Class

        <Serializable()> Class HUMIDITE
            Inherits DeviceGeneriqueValue

            'Creation du device
            Public Sub New()
                _Type = "HUMIDITE"
            End Sub

            Public Event DeviceChanged(ByVal Id As String, ByVal [Property] As String, ByVal Parametre As Object)

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
                        AddHandler MyTimer.Elapsed, AddressOf TimerTick
                    End If
                End Set
            End Property

            Private Sub TimerTick()
                Value = Driver.ReadHum(Me)
            End Sub

            'Valeur d'Humidité
            Public Property Value() As Double
                Get
                    Return _Value
                End Get
                Set(ByVal value As Double)
                    Dim tmp As Double = value
                    If tmp < _ValueMin Then tmp = _ValueMin
                    If tmp > _ValueMax Then tmp = _ValueMax
                    If _Formatage <> "" Then tmp = Format(tmp, _Formatage)
                    tmp += _Correction

                    'Si la valeur a changé on la prend en compte et on créer l'event
                    If tmp <> _Value Then
                        _Value = tmp
                        _LastChanged = Now
                        RaiseEvent DeviceChanged(_ID, "Value", _Value)
                    End If
                End Set
            End Property

        End Class

        <Serializable()> Class BATTERIE
            Inherits DeviceGenerique

            Dim _Value As String

            'Creation d'un device BATTERIE
            Public Sub New()
                _Type = "BATTERIE"
            End Sub

            Public Event DeviceChanged(ByVal Id As String, ByVal [Property] As String, ByVal Parametre As Object)

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
                        AddHandler MyTimer.Elapsed, AddressOf TimerTick
                    End If
                End Set
            End Property

            Private Sub TimerTick()
                Value = Driver.ReadBatterie(Me)
            End Sub

            'OK/Low
            Public Property Value() As String
                Get
                    Return _Value
                End Get
                Set(ByVal value As String)
                    Dim tmp As String = value

                    'Si la valeur a changé on la prend en compte et on créer l'event
                    If tmp <> _Value Then
                        _Value = tmp
                        _LastChanged = Now
                        RaiseEvent DeviceChanged(_ID, "Value", _Value)
                    End If
                End Set
            End Property

        End Class

        <Serializable()> Class NIVRECEPTION
            Inherits DeviceGenerique

            Dim _Value As Integer

            'Creation du device
            Public Sub New()
                _Type = "NIVRECEPTION"
            End Sub

            Public Event DeviceChanged(ByVal Id As String, ByVal [Property] As String, ByVal Parametre As Object)

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
                        AddHandler MyTimer.Elapsed, AddressOf TimerTick
                    End If
                End Set
            End Property

            Private Sub TimerTick()
                Value = Driver.ReadNivRecept(Me)
            End Sub

            'Niv de Réception
            Public Property Value() As Integer
                Get
                    Return _Value
                End Get
                Set(ByVal value As Integer)
                    Dim tmp As String = value

                    'Si la valeur a changé on la prend en compte et on créer l'event
                    If tmp <> _Value Then
                        _Value = tmp
                        _LastChanged = Now
                        RaiseEvent DeviceChanged(_ID, "Value", _Value)
                    End If
                End Set
            End Property

        End Class

        <Serializable()> Class TEMPERATURECONSIGNE
            Inherits DeviceGeneriqueValue

            Public Event DeviceChanged(ByVal Id As String, ByVal [Property] As String, ByVal Parametre As Object)

            'Creation du device
            Public Sub New()
                _Type = "TEMPERATURECONSIGNE"
            End Sub

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
                        AddHandler MyTimer.Elapsed, AddressOf TimerTick
                    End If
                End Set
            End Property

            Private Sub TimerTick()
                Value = Driver.ReadTempCsg(Me)
            End Sub

            'Valeur de Température de consigne
            Public Property Value() As Double
                Get
                    Return _Value
                End Get
                Set(ByVal value As Double)
                    Dim tmp As Double = value
                    If tmp < _ValueMin Then tmp = _ValueMin
                    If tmp > _ValueMax Then tmp = _ValueMax
                    If _Formatage <> "" Then tmp = Format(tmp, _Formatage)
                    tmp += _Correction

                    'Si la valeur a changé on la prend en compte et on créer l'event
                    If tmp <> _Value Then
                        _Value = tmp
                        _LastChanged = Now
                        RaiseEvent DeviceChanged(_ID, "Value", _Value)
                    End If
                End Set
            End Property

        End Class

        <Serializable()> Class ENERGIETOTALE
            Inherits DeviceGeneriqueValue

            'Creation du device
            Public Sub New()
                _Type = "ENERGIETOTALE"
            End Sub

            Public Event DeviceChanged(ByVal Id As String, ByVal [Property] As String, ByVal Parametre As Object)

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
                        AddHandler MyTimer.Elapsed, AddressOf TimerTick
                    End If
                End Set
            End Property

            Private Sub TimerTick()
                Value = Driver.ReadEnergieTot(Me)
            End Sub

            'Valeur Energie Totale
            Public Property Value() As Double
                Get
                    Return _Value
                End Get
                Set(ByVal value As Double)
                    Dim tmp As Double = value
                    If tmp < _ValueMin Then tmp = _ValueMin
                    If tmp > _ValueMax Then tmp = _ValueMax
                    If _Formatage <> "" Then tmp = Format(tmp, _Formatage)
                    tmp += _Correction

                    'Si la valeur a changé on la prend en compte et on créer l'event
                    If tmp <> _Value Then
                        _Value = tmp
                        _LastChanged = Now
                        RaiseEvent DeviceChanged(_ID, "Value", _Value)
                    End If
                End Set
            End Property

        End Class

        <Serializable()> Class ENERGIEINSTANTANEE
            Inherits DeviceGeneriqueValue

            'Creation du device
            Public Sub New()
                _Type = "ENERGIEINSTANTANEE"
            End Sub

            Public Event DeviceChanged(ByVal Id As String, ByVal [Property] As String, ByVal Parametre As Object)

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
                        AddHandler MyTimer.Elapsed, AddressOf TimerTick
                    End If
                End Set
            End Property

            Private Sub TimerTick()
                Value = Driver.ReadEnergieInst(Me)
            End Sub

            'Valeur Energie Instantanée
            Public Property Value() As Double
                Get
                    Return _Value
                End Get
                Set(ByVal value As Double)
                    Dim tmp As Double = value
                    If tmp < _ValueMin Then tmp = _ValueMin
                    If tmp > _ValueMax Then tmp = _ValueMax
                    If _Formatage <> "" Then tmp = Format(tmp, _Formatage)
                    tmp += _Correction

                    'Si la valeur a changé on la prend en compte et on créer l'event
                    If tmp <> _Value Then
                        _Value = tmp
                        _LastChanged = Now
                        RaiseEvent DeviceChanged(_ID, "Value", _Value)
                    End If
                End Set
            End Property

        End Class

        <Serializable()> Class PLUIETOTAL
            Inherits DeviceGeneriqueValue

            'Creation du device
            Public Sub New()
                _Type = "PLUIETOTAL"
            End Sub


            Public Event DeviceChanged(ByVal Id As String, ByVal [Property] As String, ByVal Parametre As Object)

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
                        AddHandler MyTimer.Elapsed, AddressOf TimerTick
                    End If
                End Set
            End Property

            Private Sub TimerTick()
                Value = Driver.ReadPluieTot(Me)
            End Sub

            'Valeur Pluie Totale
            Public Property Value() As Double
                Get
                    Return _Value
                End Get
                Set(ByVal value As Double)
                    Dim tmp As Double = value
                    If tmp < _ValueMin Then tmp = _ValueMin
                    If tmp > _ValueMax Then tmp = _ValueMax
                    If _Formatage <> "" Then tmp = Format(tmp, _Formatage)
                    tmp += _Correction

                    'Si la valeur a changé on la prend en compte et on créer l'event
                    If tmp <> _Value Then
                        _Value = tmp
                        _LastChanged = Now
                        RaiseEvent DeviceChanged(_ID, "Value", _Value)
                    End If
                End Set
            End Property

        End Class

        <Serializable()> Class PLUIECOURANT
            Inherits DeviceGeneriqueValue

            'Creation du device
            Public Sub New()
                _Type = "PLUIECOURANT"
            End Sub

            Public Event DeviceChanged(ByVal Id As String, ByVal [Property] As String, ByVal Parametre As Object)

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
                        AddHandler MyTimer.Elapsed, AddressOf TimerTick
                    End If
                End Set
            End Property

            Private Sub TimerTick()
                Value = Driver.ReadPluieCrt(Me)
            End Sub

            'Valeur Niveau de pluie courant (Currant Rain)
            Public Property Value() As Double
                Get
                    Return _Value
                End Get
                Set(ByVal value As Double)
                    Dim tmp As Double = value
                    If tmp < _ValueMin Then tmp = _ValueMin
                    If tmp > _ValueMax Then tmp = _ValueMax
                    If _Formatage <> "" Then tmp = Format(tmp, _Formatage)
                    tmp += _Correction

                    'Si la valeur a changé on la prend en compte et on créer l'event
                    If tmp <> _Value Then
                        _Value = tmp
                        _LastChanged = Now
                        RaiseEvent DeviceChanged(_ID, "Value", _Value)
                    End If
                End Set
            End Property

        End Class

        <Serializable()> Class VITESSEVENT
            Inherits DeviceGeneriqueValue

            'Creation du device
            Public Sub New()
                _Type = "VITESSEVENT"
            End Sub

            Public Event DeviceChanged(ByVal Id As String, ByVal [Property] As String, ByVal Parametre As Object)

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
                        AddHandler MyTimer.Elapsed, AddressOf TimerTick
                    End If
                End Set
            End Property

            Private Sub TimerTick()
                Value = Driver.ReadVitessVent(Me)
            End Sub

            'Valeur Mesure de la vitesse du vent
            Public Property Value() As Double
                Get
                    Return _Value
                End Get
                Set(ByVal value As Double)
                    Dim tmp As Double = value
                    If tmp < _ValueMin Then tmp = _ValueMin
                    If tmp > _ValueMax Then tmp = _ValueMax
                    If _Formatage <> "" Then tmp = Format(tmp, _Formatage)
                    tmp += _Correction

                    'Si la valeur a changé on la prend en compte et on créer l'event
                    If tmp <> _Value Then
                        _Value = tmp
                        _LastChanged = Now
                        RaiseEvent DeviceChanged(_ID, "Value", _Value)
                    End If
                End Set
            End Property

        End Class

        <Serializable()> Class DIRECTIONVENT
            Inherits DeviceGenerique
            Dim _Value As String

            'Creation du device
            Public Sub New()
                _Type = "DIRECTIONVENT"
            End Sub

            Public Event DeviceChanged(ByVal Id As String, ByVal [Property] As String, ByVal Parametre As Object)

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
                        AddHandler MyTimer.Elapsed, AddressOf TimerTick
                    End If
                End Set
            End Property

            Private Sub TimerTick()
                Value = Driver.ReadDirectVent(Me)
            End Sub

            'Valeur Direction du vent
            Public Property Value() As String
                Get
                    Return _Value
                End Get
                Set(ByVal value As String)
                    Dim tmp As String = value
                    'Si la valeur a changé on la prend en compte et on créer l'event
                    If tmp <> _Value Then
                        _Value = tmp
                        _LastChanged = Now
                        RaiseEvent DeviceChanged(_ID, "Value", _Value)
                    End If
                End Set
            End Property

        End Class

        <Serializable()> Class UV
            Inherits DeviceGeneriqueValue

            'Creation du device
            Public Sub New()
                _Type = "UV"
            End Sub

            Public Event DeviceChanged(ByVal Id As String, ByVal [Property] As String, ByVal Parametre As Object)

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
                        AddHandler MyTimer.Elapsed, AddressOf TimerTick
                    End If
                End Set
            End Property

            Private Sub TimerTick()
                Value = Driver.ReadUV(Me)
            End Sub

            'Valeur Niveau d’UV
            Public Property Value() As Double
                Get
                    Return _Value
                End Get
                Set(ByVal value As Double)
                    Dim tmp As Double = value
                    If tmp < _ValueMin Then tmp = _ValueMin
                    If tmp > _ValueMax Then tmp = _ValueMax
                    If _Formatage <> "" Then tmp = Format(tmp, _Formatage)
                    tmp += _Correction

                    'Si la valeur a changé on la prend en compte et on créer l'event
                    If tmp <> _Value Then
                        _Value = tmp
                        _LastChanged = Now
                        RaiseEvent DeviceChanged(_ID, "Value", _Value)
                    End If
                End Set
            End Property

        End Class

        <Serializable()> Class APPAREIL
            Inherits DeviceGenerique
            Dim _Value As Boolean

            'Creation du device
            Public Sub New()
                _Type = "APPAREIL"
            End Sub

            Public Event DeviceChanged(ByVal Id As String, ByVal [Property] As String, ByVal Parametre As Object)

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
                        AddHandler MyTimer.Elapsed, AddressOf TimerTick
                    End If
                End Set
            End Property

            Private Sub TimerTick()
                Value = Driver.ReadAppareil(Me)
            End Sub

            'Valeur Direction du vent
            Public Property Value() As Boolean
                Get
                    Return _Value
                End Get
                Set(ByVal value As Boolean)
                    Dim tmp As Boolean = value
                    'Si la valeur a changé on la prend en compte et on créer l'event
                    If tmp <> _Value Then
                        _Value = tmp
                        _LastChanged = Now
                        RaiseEvent DeviceChanged(_ID, "Value", _Value)
                    End If
                End Set
            End Property

            'ON
            Public Sub [ON]()
                Driver.ON(Me)
            End Sub

            'OFF
            Public Sub OFF()
                Driver.OFF(Me)
            End Sub
        End Class

        <Serializable()> Class LAMPE
            Inherits DeviceGenerique
            Dim _Value As Integer

            'Creation du device
            Public Sub New()
                _Type = "LAMPE"
            End Sub

            Public Event DeviceChanged(ByVal Id As String, ByVal [Property] As String, ByVal Parametre As Object)

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
                        AddHandler MyTimer.Elapsed, AddressOf TimerTick
                    End If
                End Set
            End Property

            Private Sub TimerTick()
                Value = Driver.ReadLampe(Me)
            End Sub

            'Valeur Variation
            Public Property Value() As Integer
                Get
                    Return _Value
                End Get
                Set(ByVal value As Integer)
                    Dim tmp As Integer = value
                    If tmp < 0 Then tmp = 0
                    If tmp > 100 Then tmp = 100

                    'Si la valeur a changé on la prend en compte et on créer l'event
                    If tmp <> _Value Then
                        _Value = tmp
                        _LastChanged = Now
                        Driver.DIM(Me, _Value)
                        RaiseEvent DeviceChanged(_ID, "Value", _Value)
                    End If
                End Set
            End Property

            'ON
            Public Sub [ON]()
                Value = 100
            End Sub

            'OFF
            Public Sub OFF()
                Value = 0
            End Sub

            'DIM
            Public Sub [DIM](ByVal Variation As Integer)
                If Variation < 0 Then
                    Value = 0
                ElseIf Variation > 100 Then
                    Value = 100
                Else
                    Value = Variation
                End If
            End Sub

        End Class

        <Serializable()> Class CONTACT
            Inherits DeviceGenerique
            Dim _Value As Boolean

            'Creation du device
            Public Sub New()
                _Type = "CONTACT"
            End Sub

            Public Event DeviceChanged(ByVal Id As String, ByVal [Property] As String, ByVal Parametre As Object)

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
                        AddHandler MyTimer.Elapsed, AddressOf TimerTick
                    End If
                End Set
            End Property

            Private Sub TimerTick()
                Value = Driver.ReadContact(Me)
            End Sub

            'Valeur Contact
            Public Property Value() As Boolean
                Get
                    Return _Value
                End Get
                Set(ByVal value As Boolean)
                    Dim tmp As Boolean = value
                    'Si la valeur a changé on la prend en compte et on créer l'event
                    If tmp <> _Value Then
                        _Value = tmp
                        _LastChanged = Now
                        RaiseEvent DeviceChanged(_ID, "Value", _Value)
                    End If
                End Set
            End Property

        End Class

        <Serializable()> Class METEO
            Inherits DeviceGenerique
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
            Public Sub New()
                _Type = "METEO"
            End Sub

            Public Event DeviceChanged(ByVal Id As String, ByVal [Property] As String, ByVal Parametre As Object)

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
                        AddHandler MyTimer.Elapsed, AddressOf TimerTick
                    End If
                End Set
            End Property

            Private Sub TimerTick()
                Driver.ReadMeteo(Me)
            End Sub

            Public Property ConditionActuel() As String
                Get
                    Return _ConditionActuel
                End Get
                Set(ByVal value As String)
                    _ConditionActuel = value
                    _LastChanged = Now
                    RaiseEvent DeviceChanged(_ID, "ConditionActuel", value)
                End Set
            End Property

            Public Property TemperatureActuel() As String
                Get
                    Return _TempActuel
                End Get
                Set(ByVal value As String)
                    _TempActuel = value
                    _LastChanged = Now
                    RaiseEvent DeviceChanged(_ID, "TemperatureActuel", value)
                End Set
            End Property

            Public Property HumiditeActuel() As String
                Get
                    Return _HumActuel
                End Get
                Set(ByVal value As String)
                    _HumActuel = value
                    _LastChanged = Now
                    RaiseEvent DeviceChanged(_ID, "HumiditeActuel", value)
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
                    _LastChanged = Now
                    RaiseEvent DeviceChanged(_ID, "VentActuel", value)
                End Set
            End Property

            Public Property JourToday() As String
                Get
                    Return _JourToday
                End Get
                Set(ByVal value As String)
                    _JourToday = value
                    _LastChanged = Now
                    RaiseEvent DeviceChanged(_ID, "JourToday", value)
                End Set
            End Property

            Public Property MinToday() As String
                Get
                    Return _MinToday
                End Get
                Set(ByVal value As String)
                    _MinToday = value
                    _LastChanged = Now
                    RaiseEvent DeviceChanged(_ID, "MinToday", value)
                End Set
            End Property

            Public Property MaxToday() As String
                Get
                    Return _MaxToday
                End Get
                Set(ByVal value As String)
                    _MaxToday = value
                    _LastChanged = Now
                    RaiseEvent DeviceChanged(_ID, "MaxToday", value)
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
                    _LastChanged = Now
                    RaiseEvent DeviceChanged(_ID, "ConditionToday", value)
                End Set
            End Property

            Public Property JourJ1() As String
                Get
                    Return _JourJ1
                End Get
                Set(ByVal value As String)
                    _JourJ1 = value
                    _LastChanged = Now
                    RaiseEvent DeviceChanged(_ID, "JourJ1", value)
                End Set
            End Property

            Public Property MinJ1() As String
                Get
                    Return _MinJ1
                End Get
                Set(ByVal value As String)
                    _MinJ1 = value
                    _LastChanged = Now
                    RaiseEvent DeviceChanged(_ID, "MinJ1", value)
                End Set
            End Property

            Public Property MaxJ1() As String
                Get
                    Return _MaxJ1
                End Get
                Set(ByVal value As String)
                    _MaxJ1 = value
                    _LastChanged = Now
                    RaiseEvent DeviceChanged(_ID, "MaxJ1", value)
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
                    _LastChanged = Now
                    RaiseEvent DeviceChanged(_ID, "ConditionJ1", value)
                End Set
            End Property

            Public Property JourJ2() As String
                Get
                    Return _JourJ2
                End Get
                Set(ByVal value As String)
                    _JourJ2 = value
                    _LastChanged = Now
                    RaiseEvent DeviceChanged(_ID, "JourJ2", value)
                End Set
            End Property

            Public Property MinJ2() As String
                Get
                    Return _MinJ2
                End Get
                Set(ByVal value As String)
                    _MinJ2 = value
                    _LastChanged = Now
                    RaiseEvent DeviceChanged(_ID, "MinJ2", value)
                End Set
            End Property

            Public Property MaxJ2() As String
                Get
                    Return _MaxJ2
                End Get
                Set(ByVal value As String)
                    _MaxJ2 = value
                    _LastChanged = Now
                    RaiseEvent DeviceChanged(_ID, "MaxJ2", value)
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
                    _LastChanged = Now
                    RaiseEvent DeviceChanged(_ID, "ConditionJ2", value)
                End Set
            End Property

            Public Property JourJ3() As String
                Get
                    Return _JourJ3
                End Get
                Set(ByVal value As String)
                    _JourJ3 = value
                    _LastChanged = Now
                    RaiseEvent DeviceChanged(_ID, "JourJ3", value)
                End Set
            End Property

            Public Property MinJ3() As String
                Get
                    Return _MinJ3
                End Get
                Set(ByVal value As String)
                    _MinJ3 = value
                    _LastChanged = Now
                    RaiseEvent DeviceChanged(_ID, "MinJ3", value)
                End Set
            End Property

            Public Property MaxJ3() As String
                Get
                    Return _MaxJ3
                End Get
                Set(ByVal value As String)
                    _MaxJ3 = value
                    _LastChanged = Now
                    RaiseEvent DeviceChanged(_ID, "MaxJ3", value)
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
                    _LastChanged = Now
                    RaiseEvent DeviceChanged(_ID, "ConditionJ3", value)
                End Set
            End Property

        End Class

        <Serializable()> Class AUDIO
            Inherits DeviceGenerique
            Dim _Value As String
            Dim _Fichier As String

            'Creation du device
            Public Sub New()
                _Type = "AUDIO"
            End Sub

            Public Event DeviceChanged(ByVal Id As String, ByVal [Property] As String, ByVal Parametre As Object)

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
                        AddHandler MyTimer.Elapsed, AddressOf TimerTick
                    End If
                End Set
            End Property

            Private Sub TimerTick()

            End Sub

            'Représente le status du lecteur (Play, Pause, Stop)
            Public Property Value() As String
                Get
                    Return _Value
                End Get
                Set(ByVal value As String)
                    Dim tmp As String = value
                    'Si la valeur a changé on la prend en compte et on créer l'event
                    If tmp <> _Value Then
                        _Value = tmp
                        _LastChanged = Now
                        RaiseEvent DeviceChanged(_ID, "Value", _Value)
                    End If
                End Set
            End Property

            Public Property Fichier() As String
                Get
                    Return _Fichier
                End Get
                Set(ByVal value As String)
                    _Fichier = value
                End Set
            End Property

            Public Sub Play()
                Try
                    Driver.PlayAudio(Me)
                    Value = "PLAY"
                Catch ex As Exception
                    '_Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " PLAY: " & ex.Message)
                End Try
            End Sub

            Public Sub Pause()
                Try
                    Driver.PauseAudio(Me)
                    Value = "PAUSE"
                Catch ex As Exception
                    '_Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " PAUSE: " & ex.Message)
                End Try
            End Sub

            Public Sub [Stop]()
                Try
                    Driver.StopAudio(Me)
                    Value = "STOP"
                Catch ex As Exception
                    '_Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " STOP: " & ex.Message)
                End Try
            End Sub

            Public Sub Random()
                Try
                    Driver.RandomAudio(Me)
                Catch ex As Exception
                    '_Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " RANDOM: " & ex.Message)
                End Try
            End Sub

            Public Sub [Next]()
                Try
                    Driver.NextAudio(Me)
                Catch ex As Exception
                    '_Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " NEXT: " & ex.Message)
                End Try
            End Sub

            Public Sub Previous()
                Try
                    Driver.PreviousAudio(Me)
                Catch ex As Exception
                    '_Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " PREVIOUS: " & ex.Message)
                End Try
            End Sub

            Public Sub VolumeDown()
                Try
                    Driver.VolumeDownAudio(Me)
                Catch ex As Exception
                    '_Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " VOLUME DOWN: " & ex.Message)
                End Try
            End Sub

            Public Sub VolumeUp()
                Try
                    Driver.VolumeUpAudio(Me)
                Catch ex As Exception
                    '_Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " VOLUME UP: " & ex.Message)
                End Try
            End Sub

            Public Sub VolumeMute()
                Try
                    Driver.VolumeMuteAudio(Me)
                Catch ex As Exception
                    '_Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " VOLUME MUTE: " & ex.Message)
                End Try
            End Sub

        End Class

        <Serializable()> Class MULTIMEDIA
            Inherits DeviceGenerique
            Dim _Value As String

            Public ListCommandName As New ArrayList
            Public ListCommandData As New ArrayList
            Public ListCommandRepeat As New ArrayList

            'Creation du device
            Public Sub New()
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

            Public Event DeviceChanged(ByVal Id As String, ByVal [Property] As String, ByVal Parametre As Object)

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
                        AddHandler MyTimer.Elapsed, AddressOf TimerTick
                    End If
                End Set
            End Property

            Private Sub TimerTick()

            End Sub

            'Représente le status du lecteur (Play, Pause, Stop)
            Public Property Value() As String
                Get
                    Return _Value
                End Get
                Set(ByVal value As String)
                    Dim tmp As String = value
                    'Si la valeur a changé on la prend en compte et on créer l'event
                    If tmp <> _Value Then
                        _Value = tmp
                        _LastChanged = Now
                        RaiseEvent DeviceChanged(_ID, "Value", _Value)
                    End If
                End Set
            End Property

            Public Sub SendCommand(ByVal NameCommand As String)
                For i As Integer = 0 To ListCommandName.Count - 1
                    If ListCommandName(i) = NameCommand Then
                        Driver.SendCodeIR(ListCommandData(i), ListCommandRepeat(i))
                    End If
                Next
            End Sub

        End Class

        <Serializable()> Class FREEBOX
            Inherits DeviceGenerique
            Dim _Value As String
            Public ListCommandName As New ArrayList
            Public ListCommandData As New ArrayList
            Public ListCommandRepeat As New ArrayList

            'Creation du device
            Public Sub New()
                _Type = "FREEBOX"
                _Adresse1 = " http://hd1.freebox.fr/pub/remote_control ?key="
            End Sub

            Public Event DeviceChanged(ByVal Id As String, ByVal [Property] As String, ByVal Parametre As Object)

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
                        AddHandler MyTimer.Elapsed, AddressOf TimerTick
                    End If
                End Set
            End Property

            Private Sub TimerTick()

            End Sub

            'Représente la dernière commande envoyée
            Public Property Value() As String
                Get
                    Return _Value
                End Get
                Set(ByVal value As String)
                    Dim tmp As String = value
                    'Si la valeur a changé on la prend en compte et on créer l'event
                    If tmp <> _Value Then
                        _Value = tmp
                        _LastChanged = Now
                        RaiseEvent DeviceChanged(_ID, "Value", _Value)
                    End If
                End Set
            End Property

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

            Public Sub Touche0()
                Try
                    Dim retour As String
                    retour = Sendhttp("0")
                    Value = "0"
                Catch ex As Exception
                    Log.Log(Log.TypeLog.ERREUR, Log.TypeSource.DEVICE, "Erreur " & Me.Name & " Touche0: " & ex.Message)
                End Try
            End Sub

            Public Sub Touche1()
                Try
                    Dim retour As String
                    retour = Sendhttp("1")
                    Value = "1"
                Catch ex As Exception
                    Log.Log(Log.TypeLog.ERREUR, Log.TypeSource.DEVICE, "Erreur " & Me.Name & " Touche1: " & ex.Message)
                End Try
            End Sub

            Public Sub Touche2()
                Try
                    Dim retour As String
                    retour = Sendhttp("2")
                    Value = "2"
                Catch ex As Exception
                    Log.Log(Log.TypeLog.ERREUR, Log.TypeSource.DEVICE, "Erreur " & Me.Name & " Touche2: " & ex.Message)
                End Try
            End Sub

            Public Sub Touche3()
                Try
                    Dim retour As String
                    retour = Sendhttp("3")
                    Value = "3"
                Catch ex As Exception
                    Log.Log(Log.TypeLog.ERREUR, Log.TypeSource.DEVICE, "Erreur " & Me.Name & " Touche3: " & ex.Message)
                End Try
            End Sub

            Public Sub Touche4()
                Try
                    Dim retour As String
                    retour = Sendhttp("4")
                    Value = "4"
                Catch ex As Exception
                    Log.Log(Log.TypeLog.ERREUR, Log.TypeSource.DEVICE, "Erreur " & Me.Name & " Touche4: " & ex.Message)
                End Try
            End Sub

            Public Sub Touche5()
                Try
                    Dim retour As String
                    retour = Sendhttp("5")
                    Value = "5"
                Catch ex As Exception
                    Log.Log(Log.TypeLog.ERREUR, Log.TypeSource.DEVICE, "Erreur " & Me.Name & " Touche5: " & ex.Message)
                End Try
            End Sub

            Public Sub Touche6()
                Try
                    Dim retour As String
                    retour = Sendhttp("6")
                    Value = "6"
                Catch ex As Exception
                    Log.Log(Log.TypeLog.ERREUR, Log.TypeSource.DEVICE, "Erreur " & Me.Name & " Touche6: " & ex.Message)
                End Try
            End Sub

            Public Sub Touche7()
                Try
                    Dim retour As String
                    retour = Sendhttp("7")
                    Value = "7"
                Catch ex As Exception
                    Log.Log(Log.TypeLog.ERREUR, Log.TypeSource.DEVICE, "Erreur " & Me.Name & " Touche7: " & ex.Message)
                End Try
            End Sub

            Public Sub Touche8()
                Try
                    Dim retour As String
                    retour = Sendhttp("8")
                    Value = "8"
                Catch ex As Exception
                    Log.Log(Log.TypeLog.ERREUR, Log.TypeSource.DEVICE, "Erreur " & Me.Name & " Touche8: " & ex.Message)
                End Try
            End Sub

            Public Sub Touche9()
                Try
                    Dim retour As String
                    retour = Sendhttp("9")
                    Value = "9"
                Catch ex As Exception
                    Log.Log(Log.TypeLog.ERREUR, Log.TypeSource.DEVICE, "Erreur " & Me.Name & " Touche9: " & ex.Message)
                End Try
            End Sub

            Public Sub VolumeUp()
                Try
                    Dim retour As String
                    retour = Sendhttp("vol_inc")
                    Value = "vol_inc"
                Catch ex As Exception
                    Log.Log(Log.TypeLog.ERREUR, Log.TypeSource.DEVICE, "Erreur " & Me.Name & " VolumeUp: " & ex.Message)
                End Try
            End Sub

            Public Sub VolumeDown()
                Try
                    Dim retour As String
                    retour = Sendhttp("vol_dec")
                    Value = "vol_dec"
                Catch ex As Exception
                    Log.Log(Log.TypeLog.ERREUR, Log.TypeSource.DEVICE, "Erreur " & Me.Name & " VolumeDown: " & ex.Message)
                End Try
            End Sub

            Public Sub OK()
                Try
                    Dim retour As String
                    retour = Sendhttp("ok")
                    Value = "ok"
                Catch ex As Exception
                    Log.Log(Log.TypeLog.ERREUR, Log.TypeSource.DEVICE, "Erreur " & Me.Name & " OK: " & ex.Message)
                End Try
            End Sub

            Public Sub HAUT()
                Try
                    Dim retour As String
                    retour = Sendhttp("up")
                    Value = "up"
                Catch ex As Exception
                    Log.Log(Log.TypeLog.ERREUR, Log.TypeSource.DEVICE, "Erreur " & Me.Name & " HAUT: " & ex.Message)
                End Try
            End Sub

            Public Sub BAS()
                Try
                    Dim retour As String
                    retour = Sendhttp("down")
                    Value = "down"
                Catch ex As Exception
                    Log.Log(Log.TypeLog.ERREUR, Log.TypeSource.DEVICE, "Erreur " & Me.Name & " BAS: " & ex.Message)
                End Try
            End Sub

            Public Sub GAUCHE()
                Try
                    Dim retour As String
                    retour = Sendhttp("left")
                    Value = "left"
                Catch ex As Exception
                    Log.Log(Log.TypeLog.ERREUR, Log.TypeSource.DEVICE, "Erreur " & Me.Name & " GAUCHE: " & ex.Message)
                End Try
            End Sub

            Public Sub DROITE()
                Try
                    Dim retour As String
                    retour = Sendhttp("right")
                    Value = "right"
                Catch ex As Exception
                    Log.Log(Log.TypeLog.ERREUR, Log.TypeSource.DEVICE, "Erreur " & Me.Name & " DROITE: " & ex.Message)
                End Try
            End Sub

            Public Sub MUTE()
                Try
                    Dim retour As String
                    retour = Sendhttp("mute")
                    Value = "mute"
                Catch ex As Exception
                    Log.Log(Log.TypeLog.ERREUR, Log.TypeSource.DEVICE, "Erreur " & Me.Name & " MUTE: " & ex.Message)
                End Try
            End Sub

            Public Sub HOME()
                Try
                    Dim retour As String
                    retour = Sendhttp("home")
                    Value = "home"
                Catch ex As Exception
                    Log.Log(Log.TypeLog.ERREUR, Log.TypeSource.DEVICE, "Erreur " & Me.Name & " HOME: " & ex.Message)
                End Try
            End Sub

            Public Sub ENREGISTRER()
                Try
                    Dim retour As String
                    retour = Sendhttp("rec")
                    Value = "rec"
                Catch ex As Exception
                    Log.Log(Log.TypeLog.ERREUR, Log.TypeSource.DEVICE, "Erreur " & Me.Name & " ENREGISTRER: " & ex.Message)
                End Try
            End Sub

            Public Sub RETOUR()
                Try
                    Dim retour As String
                    retour = Sendhttp("bwd")
                    Value = "bwd"
                Catch ex As Exception
                    Log.Log(Log.TypeLog.ERREUR, Log.TypeSource.DEVICE, "Erreur " & Me.Name & " RETOUR: " & ex.Message)
                End Try
            End Sub

            Public Sub PRECEDENT()
                Try
                    Dim retour As String
                    retour = Sendhttp("prev")
                    Value = "prev"
                Catch ex As Exception
                    Log.Log(Log.TypeLog.ERREUR, Log.TypeSource.DEVICE, "Erreur " & Me.Name & " PRECEDENT: " & ex.Message)
                End Try
            End Sub

            Public Sub PLAY()
                Try
                    Dim retour As String
                    retour = Sendhttp("play")
                    Value = "play"
                Catch ex As Exception
                    Log.Log(Log.TypeLog.ERREUR, Log.TypeSource.DEVICE, "Erreur " & Me.Name & " PLAY: " & ex.Message)
                End Try
            End Sub

            Public Sub AVANCE()
                Try
                    Dim retour As String
                    retour = Sendhttp("fwd")
                    Value = "fwd"
                Catch ex As Exception
                    Log.Log(Log.TypeLog.ERREUR, Log.TypeSource.DEVICE, "Erreur " & Me.Name & " AVANCE: " & ex.Message)
                End Try
            End Sub

            Public Sub SUIVANT()
                Try
                    Dim retour As String
                    retour = Sendhttp("next")
                    Value = "next"
                Catch ex As Exception
                    Log.Log(Log.TypeLog.ERREUR, Log.TypeSource.DEVICE, "Erreur " & Me.Name & " SUIVANT: " & ex.Message)
                End Try
            End Sub

            Public Sub BoutonROUGE()
                Try
                    Dim retour As String
                    retour = Sendhttp("red")
                    Value = "red"
                Catch ex As Exception
                    Log.Log(Log.TypeLog.ERREUR, Log.TypeSource.DEVICE, "Erreur " & Me.Name & " BoutonROUGE: " & ex.Message)
                End Try
            End Sub

            Public Sub BoutonVERT()
                Try
                    Dim retour As String
                    retour = Sendhttp("green")
                    Value = "green"
                Catch ex As Exception
                    Log.Log(Log.TypeLog.ERREUR, Log.TypeSource.DEVICE, "Erreur " & Me.Name & " BoutonVERT: " & ex.Message)
                End Try
            End Sub

            Public Sub BoutonJAUNE()
                Try
                    Dim retour As String
                    retour = Sendhttp("yellow")
                    Value = "yellow"
                Catch ex As Exception
                    Log.Log(Log.TypeLog.ERREUR, Log.TypeSource.DEVICE, "Erreur " & Me.Name & " BoutonJAUNE: " & ex.Message)
                End Try
            End Sub

            Public Sub BoutonBLEU()
                Try
                    Dim retour As String
                    retour = Sendhttp("blue")
                    Value = "blue"
                Catch ex As Exception
                    Log.Log(Log.TypeLog.ERREUR, Log.TypeSource.DEVICE, "Erreur " & Me.Name & " BoutonBLEU: " & ex.Message)
                End Try
            End Sub

        End Class

        <Serializable()> Class VOLET
            Inherits DeviceGenerique
            Dim _Value As Integer

            'Creation du device
            Public Sub New()
                _Type = "VOLET"
            End Sub

            Public Event DeviceChanged(ByVal Id As String, ByVal [Property] As String, ByVal Parametre As Object)

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
                        AddHandler MyTimer.Elapsed, AddressOf TimerTick
                    End If
                End Set
            End Property

            Private Sub TimerTick()
                Value = Driver.ReadVolet(Me)
            End Sub

            'Valeur Variation ouverture volet
            Public Property Value() As Integer
                Get
                    Return _Value
                End Get
                Set(ByVal value As Integer)
                    Dim tmp As Integer = value
                    If tmp < 0 Then tmp = 0
                    If tmp > 100 Then tmp = 100

                    'Si la valeur a changé on la prend en compte et on créer l'event
                    If tmp <> _Value Then
                        _Value = tmp
                        _LastChanged = Now
                        Driver.Volet(Me, _Value)
                        RaiseEvent DeviceChanged(_ID, "Value", _Value)
                    End If
                End Set
            End Property

            'Ouvrir volet
            Public Sub OPEN()
                Value = 100
            End Sub

            'Fermer Volet
            Public Sub CLOSE()
                Value = 0
            End Sub

            'Ouvrir/Fermer % Volet
            Public Sub VARIATION(ByVal Variation As Integer)
                Value = Variation
            End Sub

        End Class

        <Serializable()> Class OBSCURITE
            Inherits DeviceGenerique
            Dim _Value As Boolean

            'Creation du device
            Public Sub New()
                _Type = "OBSCURITE"
            End Sub

            Public Event DeviceChanged(ByVal Id As String, ByVal [Property] As String, ByVal Parametre As Object)

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
                        AddHandler MyTimer.Elapsed, AddressOf TimerTick
                    End If
                End Set
            End Property

            Private Sub TimerTick()
                Value = Driver.ReadObscurite(Me)
            End Sub

            'Valeur Contact
            Public Property Value() As Boolean
                Get
                    Return _Value
                End Get
                Set(ByVal value As Boolean)
                    Dim tmp As Boolean = value
                    'Si la valeur a changé on la prend en compte et on créer l'event
                    If tmp <> _Value Then
                        _Value = tmp
                        _LastChanged = Now
                        RaiseEvent DeviceChanged(_ID, "Value", _Value)
                    End If
                End Set
            End Property

        End Class

        <Serializable()> Class SWITCH
            Inherits DeviceGenerique
            Dim _Value As Boolean

            'Creation du device
            Public Sub New()
                _Type = "SWITCH"
            End Sub

            Public Event DeviceChanged(ByVal Id As String, ByVal [Property] As String, ByVal Parametre As Object)

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
                        AddHandler MyTimer.Elapsed, AddressOf TimerTick
                    End If
                End Set
            End Property

            Private Sub TimerTick()
                Value = Driver.ReadSwitch(Me)
            End Sub

            'Valeur Du Switch
            Public Property Value() As Boolean
                Get
                    Return _Value
                End Get
                Set(ByVal value As Boolean)
                    Dim tmp As Boolean = value
                    'Si la valeur a changé on la prend en compte et on créer l'event
                    If tmp <> _Value Then
                        _Value = tmp
                        _LastChanged = Now
                        RaiseEvent DeviceChanged(_ID, "Value", _Value)
                    End If
                End Set
            End Property

        End Class

        <Serializable()> Class TELECOMMANDE
            Inherits DeviceGenerique
            Dim _Value As String

            'Creation du device
            Public Sub New()
                _Type = "TELECOMMANDE"
            End Sub

            Public Event DeviceChanged(ByVal Id As String, ByVal [Property] As String, ByVal Parametre As Object)

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
                        AddHandler MyTimer.Elapsed, AddressOf TimerTick
                    End If
                End Set
            End Property

            Private Sub TimerTick()
                Value = Driver.ReadTelecommande(Me)
            End Sub

            'Valeur Du Switch
            Public Property Value() As String
                Get
                    Return _Value
                End Get
                Set(ByVal value As String)
                    Dim tmp As String = value
                    'Si la valeur a changé on la prend en compte et on créer l'event
                    If tmp <> _Value Then
                        _Value = tmp
                        _LastChanged = Now
                        RaiseEvent DeviceChanged(_ID, "Value", _Value)
                    End If
                End Set
            End Property

        End Class

    End Class
End Namespace