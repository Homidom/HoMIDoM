'***********************************************
'** CLASS TEMPLATEDEVICE
'** version 1.0
'** Date de création: 09/02/2011
'** Historique (SebBergues): 09/02/2011: Création 
'***********************************************
Namespace HoMIDom

    ''' <summary>
    ''' Template de class de type Device pour le service web
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> Public Class TemplateDevice
        Dim _ID As String = ""
        Dim _Name As String = ""
        Dim _Enable As Boolean = False
        Dim _DriverId As String = ""
        Dim _Description As String = ""
        Dim _Type As Device.ListeDevices
        Dim _Adresse1 As String = ""
        Dim _Adresse2 As String = ""
        Dim _DateCreated As Date = Now
        Dim _LastChange As Date = Now
        Dim _LastChangeDuree As Integer = 0
        Dim _Refresh As Double = 0
        Dim _Modele As String = ""
        Dim _Picture As String = ""
        Dim _Solo As Boolean = True
        Dim _LastEtat As Boolean = False
        Dim _ValueLast As Double = 0
        Dim _ValueMin As Double = -9999
        Dim _ValueMax As Double = 9999
        Dim _ValueDef As Double = 0
        Dim _Precision As Double = 0
        Dim _Correction As String = "0"
        Dim _Formatage As String = ""
        Dim _Value As Object = Nothing
        Dim _DeviceAction As New List(Of DeviceAction)
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
        Dim _Unit As String = ""
        Dim _Puiss As Integer = 0
        Dim _AllValue As Boolean
        Dim _ListCommandPlus As New List(Of Device.DeviceCommande)

        ''' <summary>
        ''' Nom du device
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = value
            End Set
        End Property

        ''' <summary>
        ''' ID du device
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ID() As String
            Get
                Return _ID
            End Get
            Set(ByVal value As String)
                _ID = value
            End Set
        End Property

        ''' <summary>
        ''' Activation/désactivation du device
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Enable() As Boolean
            Get
                Return _Enable
            End Get
            Set(ByVal value As Boolean)
                _Enable = value
            End Set
        End Property

        ''' <summary>
        ''' ID du driver associé au device
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DriverID() As String
            Get
                Return _DriverId
            End Get
            Set(ByVal value As String)
                _DriverId = value
            End Set
        End Property

        ''' <summary>
        ''' Description qui peut être le modèle du device ou autre chose
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Description() As String
            Get
                Return _Description
            End Get
            Set(ByVal value As String)
                _Description = value
            End Set
        End Property

        ''' <summary>
        ''' Type de device TEMPERATURE|HUMIDITE|APPAREIL|LUMIERE|CONTACT|TV…
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Type() As Device.ListeDevices
            Get
                Return _Type
            End Get
            Set(ByVal value As Device.ListeDevices)
                _Type = value
            End Set
        End Property

        ''' <summary>
        ''' Unité du composant
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Unit As String
            Get
                Return _Unit
            End Get
            Set(ByVal value As String)
                _Unit = value
            End Set
        End Property

        ''' <summary>
        ''' Puissance du composant
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Puissance As Integer
            Get
                Return _Puiss
            End Get
            Set(ByVal value As Integer)
                _Puiss = value
            End Set
        End Property

        ''' <summary>
        ''' Prendre en compte toutes les valeurs même inchangées
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property AllValue As Boolean
            Get
                Return _AllValue
            End Get
            Set(ByVal value As Boolean)
                _AllValue = value
            End Set
        End Property

        ''' <summary>
        ''' Adresse par défaut (pour le X10 par exemple)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Adresse1() As String
            Get
                Return _Adresse1
            End Get
            Set(ByVal value As String)
                _Adresse1 = value
            End Set
        End Property

        ''' <summary>
        ''' Adresse supplémentaire si besoin (cas du RFXCOM)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Adresse2() As String
            Get
                Return _Adresse2
            End Get
            Set(ByVal value As String)
                _Adresse2 = value
            End Set
        End Property

        ''' <summary>
        ''' Date et heure de création du device
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DateCreated() As Date
            Get
                Return _DateCreated
            End Get
            Set(ByVal value As Date)
                _DateCreated = value
            End Set
        End Property

        ''' <summary>
        ''' Date et heure de la derniere modification de Value
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property LastChange() As Date
            Get
                Return _LastChange
            End Get
            Set(ByVal value As Date)
                _LastChange = value
            End Set
        End Property

        ''' <summary>
        ''' Si Lastchanged+LastchangeDuree inférieur à Now alors le composant a un problème car il n'a pas emis depuis au moins lastchangedduree.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property LastChangeDuree() As Integer
            Get
                Return _LastChangeDuree
            End Get
            Set(ByVal value As Integer)
                _LastChangeDuree = value
            End Set
        End Property

        ''' <summary>
        ''' Modèle du device
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Modele() As String
            Get
                Return _Modele
            End Get
            Set(ByVal value As String)
                _Modele = value
            End Set
        End Property

        ''' <summary>
        ''' Adresse de son image
        ''' </summary>
        ''' <value></value>
        ''' <returns>Chemin complet du fichier image situé sur le serveur </returns>
        ''' <remarks></remarks>
        Public Property Picture() As String
            Get
                Return _Picture
            End Get
            Set(ByVal value As String)
                _Picture = value
            End Set
        End Property

        ''' <summary>
        ''' Si le device est solo ou s'il contient plusieurs I/O
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Solo() As Boolean
            Get
                Return _Solo
            End Get
            Set(ByVal value As Boolean)
                _Solo = value
            End Set
        End Property

        ''' <summary>
        ''' si =true, on ne prend pas en comtpe les modifications style 19.1 19 19.1 19...
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property LastEtat() As Boolean
            Get
                Return _LastEtat
            End Get
            Set(ByVal value As Boolean)
                _LastEtat = value
            End Set
        End Property

        ''' <summary>
        ''' List des actions associées au Device
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DeviceAction() As List(Of DeviceAction)
            Get
                Return _DeviceAction
            End Get
            Set(ByVal value As List(Of DeviceAction))
                _DeviceAction = value
            End Set
        End Property

        ''' <summary>
        ''' Valeur de rafraichissement du device
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Refresh() As Double
            Get
                Return _Refresh
            End Get
            Set(ByVal value As Double)
                _Refresh = value
            End Set
        End Property

        ''' <summary>
        ''' Valeur du device
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Value() As Object
            Get
                Return _Value
            End Get
            Set(ByVal value As Object)
                _Value = value
            End Set
        End Property


        ''' <summary>
        ''' Contien l'avant derniere valeur
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ValueLast() As Double
            Get
                Return _ValueLast
            End Get
            Set(ByVal value As Double)
                _ValueLast = value
            End Set
        End Property

        ''' <summary>
        ''' Valeur minimale que value peut avoir
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ValueMin() As Double
            Get
                Return _ValueMin
            End Get
            Set(ByVal value As Double)
                _ValueMin = value
            End Set
        End Property

        ''' <summary>
        ''' Valeur maximale que value peut avoir 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ValueMax() As Double
            Get
                Return _ValueMax
            End Get
            Set(ByVal value As Double)
                _ValueMax = value
            End Set
        End Property

        ''' <summary>
        ''' Valeur par défaut de Value au démarrage du Device, si Vide = Value
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ValueDef() As Double
            Get
                Return _ValueDef
            End Get
            Set(ByVal value As Double)
                _ValueDef = value
            End Set
        End Property

        ''' <summary>
        ''' Precision de value
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Precision() As Double
            Get
                Return _Precision
            End Get
            Set(ByVal value As Double)
                _Precision = value
            End Set
        End Property

        ''' <summary>
        ''' Correction en +/-/*/div à effectuer sur la value
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Correction() As String
            Get
                Return _Correction
            End Get
            Set(ByVal value As String)
                _Correction = value
            End Set
        End Property

        ''' <summary>
        ''' Format de value 0.0 ou 0.00...
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Formatage() As String
            Get
                Return _Formatage
            End Get
            Set(ByVal value As String)
                _Formatage = value
            End Set
        End Property

        ''' <summary>
        ''' Condition actuelle (device meteo)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ConditionActuel() As String
            Get
                Return _ConditionActuel
            End Get
            Set(ByVal value As String)
                _ConditionActuel = value
            End Set
        End Property

        ''' <summary>
        ''' Temperature actuelle (device meteo)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property TemperatureActuel() As String
            Get
                Return _TempActuel
            End Get
            Set(ByVal value As String)
                _TempActuel = value
            End Set
        End Property

        ''' <summary>
        ''' Humidité actuelle (device meteo)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property HumiditeActuel() As String
            Get
                Return _HumActuel
            End Get
            Set(ByVal value As String)
                _HumActuel = value
            End Set
        End Property

        ''' <summary>
        ''' Icon de la meteo actuelle (device meteo)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IconActuel() As String
            Get
                Return _IconActuel
            End Get
            Set(ByVal value As String)
                _IconActuel = value
            End Set
        End Property

        ''' <summary>
        ''' Vitesse du vent actuelle (device meteo)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property VentActuel() As String
            Get
                Return _VentActuel
            End Get
            Set(ByVal value As String)
                _VentActuel = value
            End Set
        End Property

        ''' <summary>
        ''' Nom du jour actuel (device meteo)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property JourToday() As String
            Get
                Return _JourToday
            End Get
            Set(ByVal value As String)
                _JourToday = value
            End Set
        End Property

        ''' <summary>
        ''' Temperature minimale du jour actuel (device meteo)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property MinToday() As String
            Get
                Return _MinToday
            End Get
            Set(ByVal value As String)
                _MinToday = value
            End Set
        End Property

        ''' <summary>
        ''' Temperature maximale du jour actuel (device meteo)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property MaxToday() As String
            Get
                Return _MaxToday
            End Get
            Set(ByVal value As String)
                _MaxToday = value
            End Set
        End Property

        ''' <summary>
        ''' Icon de la meteo du jour actuel (device meteo)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IconToday() As String
            Get
                Return _IconToday
            End Get
            Set(ByVal value As String)
                _IconToday = value
            End Set
        End Property

        ''' <summary>
        ''' Condition météorologique du jour actuel (device meteo)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ConditionToday() As String
            Get
                Return _ConditionToday
            End Get
            Set(ByVal value As String)
                _ConditionToday = value
            End Set
        End Property

        ''' <summary>
        ''' Nom du jour + 1 (device meteo)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property JourJ1() As String
            Get
                Return _JourJ1
            End Get
            Set(ByVal value As String)
                _JourJ1 = value
            End Set
        End Property

        ''' <summary>
        ''' Température minimale à J+1 (device meteo)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property MinJ1() As String
            Get
                Return _MinJ1
            End Get
            Set(ByVal value As String)
                _MinJ1 = value
            End Set
        End Property

        ''' <summary>
        ''' Température maximale à J+1 (device meteo)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property MaxJ1() As String
            Get
                Return _MaxJ1
            End Get
            Set(ByVal value As String)
                _MaxJ1 = value
            End Set
        End Property

        ''' <summary>
        ''' Icon de la météo à J+1 (device meteo)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IconJ1() As String
            Get
                Return _IconJ1
            End Get
            Set(ByVal value As String)
                _IconJ1 = value
            End Set
        End Property

        ''' <summary>
        ''' Condition météorologique à J+1 (device meteo)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ConditionJ1() As String
            Get
                Return _ConditionJ1
            End Get
            Set(ByVal value As String)
                _ConditionJ1 = value
            End Set
        End Property

        ''' <summary>
        ''' Nom du jour + 2 (device meteo)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property JourJ2() As String
            Get
                Return _JourJ2
            End Get
            Set(ByVal value As String)
                _JourJ2 = value
            End Set
        End Property

        ''' <summary>
        ''' Température minimale à J+2 (device meteo)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property MinJ2() As String
            Get
                Return _MinJ2
            End Get
            Set(ByVal value As String)
                _MinJ2 = value
            End Set
        End Property

        ''' <summary>
        ''' Température maximale à J+2 (device meteo)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property MaxJ2() As String
            Get
                Return _MaxJ2
            End Get
            Set(ByVal value As String)
                _MaxJ2 = value
            End Set
        End Property

        ''' <summary>
        ''' Icon de la météo à J+2 (device meteo)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IconJ2() As String
            Get
                Return _IconJ2
            End Get
            Set(ByVal value As String)
                _IconJ2 = value
            End Set
        End Property

        ''' <summary>
        ''' Condition météorologique à J+2 (device meteo)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ConditionJ2() As String
            Get
                Return _ConditionJ2
            End Get
            Set(ByVal value As String)
                _ConditionJ2 = value
            End Set
        End Property

        ''' <summary>
        ''' Nom du jour à j+3 (device meteo)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property JourJ3() As String
            Get
                Return _JourJ3
            End Get
            Set(ByVal value As String)
                _JourJ3 = value
            End Set
        End Property

        ''' <summary>
        ''' Température minimale à j+3 (device meteo)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property MinJ3() As String
            Get
                Return _MinJ3
            End Get
            Set(ByVal value As String)
                _MinJ3 = value
            End Set
        End Property

        ''' <summary>
        ''' Température maximale à j+3 (device meteo)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property MaxJ3() As String
            Get
                Return _MaxJ3
            End Get
            Set(ByVal value As String)
                _MaxJ3 = value
            End Set
        End Property

        ''' <summary>
        ''' Icon de la meteo à j+3 (device meteo)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IconJ3() As String
            Get
                Return _IconJ3
            End Get
            Set(ByVal value As String)
                _IconJ3 = value
            End Set
        End Property

        ''' <summary>
        ''' Condition météorologique à j+3 (device meteo)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ConditionJ3() As String
            Get
                Return _ConditionJ3
            End Get
            Set(ByVal value As String)
                _ConditionJ3 = value
            End Set
        End Property

        ''' <summary>
        ''' Retourne la liste des commandes avancées
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property GetDeviceCommandePlus As List(Of Device.DeviceCommande)
            Get
                Return _ListCommandPlus
            End Get
            Set(ByVal value As List(Of Device.DeviceCommande))
                _ListCommandPlus = value
            End Set
        End Property

        ''' <summary>
        ''' Liste des noms des commandes IR (device IR)
        ''' </summary>
        ''' <remarks></remarks>
        Public Commandes As New List(Of Telecommande.Commandes)

        ''' <summary>
        ''' Liste des noms des variables IR (device IR)
        ''' </summary>
        ''' <remarks></remarks>
        Public Variables As New List(Of Telecommande.TemplateVar)
    End Class
End Namespace