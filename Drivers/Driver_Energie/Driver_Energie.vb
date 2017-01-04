Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports System.IO.Ports
Imports System.Xml
Imports System.Globalization

'************************************************
'INFOS 
'************************************************
'Le driver fonctionne de la manière suivante:
' 
'ETAPE1: Le driver est créé par Homidom --> lancement de la fonction Sub (pour récupérer/définir les paramètres avancés)
'ETAPE2: Le driver est lancé par Homidom --> lancement de la fonction Start (communication, ajout des évènements, config des pins...)
'ETAPE3:
'          - une pin (ana ou binaire) change sur la carte --> déclenchement des fonctions DigitalMessageReceieved ou AnalogMessageReceieved
'          - l'utilisateur active un device (ON/OFF) --> lancement de la fonction write
'          - l'utilisateur demande la lecture d'un device --> lancement de la fonction read
'ETAPE4: le driver est arrêté par Homidom --> lancement de la fonction stop
'************************************************

Public Class Driver_Energie
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variable Driver"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "E2D5B888-D057-11E6-8921-CB0A02593AC9"
    Dim _Nom As String = "Energie"
    Dim _Enable As Boolean = False
    Dim _Description As String = "Energie"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = ""
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
    Dim _idsrv As String
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande)
    Dim _AutoDiscover As Boolean = False

    'A ajouter dans les ppt du driver
    Dim _tempsentrereponse As Integer = 1500
    Dim _ignoreadresse As Boolean = False
    Dim _lastetat As Boolean = True
#End Region

#Region "Declaration"

#Region "Variables EAU"
    Dim _Cpt_EAU_Journalier As Integer = 0 'Valeur du totaliseur journalier = _cpt_Actuel - _Cpt_First_Day
    Dim _Cpt_EAU_Jm1 As Integer = 0 'J-1 'Valeur du totaliseur J-1
    Dim _Cpt_EAU_First_Day As Integer = 0 'Valeur du compteur totaliseur de la journée
    Dim _cpt_EAU_Actuel As Integer = 0 'Compteur d'eau

    Dim _ID_CptEauJournalier As String = String.Empty  'ID du device Compteur Eau journalier
    Dim _ID_CptEauActuel As String = String.Empty  'ID du device compteur Eau actuel
    Dim _ID_Cpt_EAU_Jm1 As String = String.Empty  'Id du device Valeur du totaliseur J-1

    Dim _LastDayEau As Integer = 0
#End Region

#Region "Variables EDF"
    Dim _Cpt_EDF_Journalier As Integer = 0 'Valeur du totaliseur journalier = _cpt_Actuel - _Cpt_First_Day
    Dim _Cpt_EDF_Jm1 As Integer = 0 'Valeur du totaliseur J-1
    Dim _Cpt_EDF_First_Day As Integer = 0 'Valeur du compteur totaliseur de la journée
    Dim _cpt_EDF_Actuel As Integer = 0 'Compteur EDF
    Dim _I_EDF_Actuel As Integer = 0 'Intensité Actuelle
    Dim _I_Max_EDF As Integer = 0 'Intensité max
    Dim _Pourcent_Puiss_EDF As Integer = 0 '% puissance consommée = puissance consommée actuelle *220 / (intensité max * 220)

    Dim _ID_CptEDFJournalier As String = String.Empty
    Dim _ID_Cpt_EDF_Actuel As String = String.Empty
    Dim _ID_Cpt_EDF_Jm1 As String = String.Empty  'Id du device Valeur du totaliseur J-1
    Dim _ID_I_EDF_Actuel As String = String.Empty
    Dim _ID_IMax_EDF As String = String.Empty
    Dim _ID_Pourcent_Puiss_EDF As String = String.Empty

    Dim _LastDayEDF As Integer = 0
#End Region

#Region "Variables GAZ"
    Dim _Cpt_GAZ_Journalier As Integer = 0 'Valeur du totaliseur journalier = _cpt_Actuel - _Cpt_First_Day
    Dim _Cpt_GAZ_Jm1 As Integer = 0 'Valeur du totaliseur J-1
    Dim _Cpt_GAZ_First_Day As Integer = 0 'Valeur du compteur totaliseur de la journée
    Dim _cpt_GAZ_Actuel As Integer = 0 'Compteur GAZ

    Dim _ID_CptGAZJournalier As String = String.Empty
    Dim _ID_Cpt_GAZ_Actuel As String = String.Empty
    Dim _ID_Cpt_GAZ_Jm1 As String = String.Empty  'Id du device Valeur du totaliseur J-1

    Dim _LastDayGAZ As Integer = 0
#End Region

    Dim _flag_First As Boolean = False 'flag=true quand on fait start du driver et =false au stop

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

    Public Property AutoDiscover() As Boolean Implements HoMIDom.HoMIDom.IDriver.AutoDiscover
        Get
            Return _AutoDiscover
        End Get
        Set(ByVal value As Boolean)
            _AutoDiscover = value
        End Set
    End Property

    ''' <summary>
    ''' Aller lire une entrée
    ''' </summary>
    ''' <param name="Objet">Device</param>
    ''' <remarks></remarks>
    Public Sub Read(ByVal Objet As Object) Implements HoMIDom.HoMIDom.IDriver.Read
        Try
            If _Enable = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Impossible d'effectuer un Read car le driver n'est pas Activé")
                Exit Sub
            End If
            If _IsConnect = False Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Impossible d'effectuer un Read car le driver n'est pas connecté")
                Exit Sub
            End If

            Try
                _ID_CptEauActuel = _Parametres.Item(0).Valeur
                _ID_CptEauJournalier = _Parametres.Item(1).Valeur
                _ID_Cpt_EAU_Jm1 = _Parametres.Item(2).Valeur

                _ID_Cpt_EDF_Actuel = _Parametres.Item(3).Valeur 'Compteur eau
                _ID_CptEDFJournalier = _Parametres.Item(4).Valeur 'Compteur eau journalier
                _ID_Cpt_EDF_Jm1 = _Parametres.Item(5).Valeur
                _ID_I_EDF_Actuel = _Parametres.Item(6).Valeur
                _ID_IMax_EDF = _Parametres.Item(7).Valeur
                _ID_Pourcent_Puiss_EDF = _Parametres.Item(8).Valeur

                _ID_Cpt_GAZ_Actuel = _Parametres.Item(9).Valeur
                _ID_CptGAZJournalier = _Parametres.Item(10).Valeur
                _ID_Cpt_GAZ_Jm1 = _Parametres.Item(11).Valeur
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur lors du chargement des paramètres: " & ex.Message)
            End Try

            If Objet.Type = "GENERIQUEVALUE" Or Objet.Type = "GENERIQUESTRING" Then
                '########################################################################################
                'CALCULS EAU
                '########################################################################################
                Try
                    If (Objet.ID = _ID_CptEauJournalier) And String.IsNullOrEmpty(_ID_CptEauActuel) = False Then
                        'Recupe la valeur actuelle du compteur
                        _cpt_EAU_Actuel = CInt(Server.ReturnDeviceById(_idsrv, _ID_CptEauActuel).Value)

                        'Fixe la valeur du totaliseur journalier
                        If _LastDayEau <> Now.Day Or (_Cpt_EAU_First_Day = 0 And _cpt_EAU_Actuel > 0) Then
                            _Cpt_EAU_First_Day = _cpt_EAU_Actuel
                            If _LastDayEau <> Now.Day Then
                                _Cpt_EAU_Jm1 = _Cpt_EAU_First_Day
                                _Cpt_EAU_First_Day = 0
                                _LastDayEau = Now.Day
                            End If
                        End If

                        'Pas de calcul si la valeur du compteur actuel = 0
                        If _cpt_EAU_Actuel > 0 Then
                            'Calcul de la valeur du compteur journalier
                            _Cpt_EAU_Journalier = _cpt_EAU_Actuel - _Cpt_EAU_First_Day
                            If _Cpt_EAU_Journalier < 0 Then _Cpt_EAU_Journalier = 0

                            'Mise a jour de la valeur du device
                            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Device:" & Objet.Name & " Adresse:" & Objet.Adresse1 & " Valeur:" & _Cpt_EAU_Journalier)
                            Objet.Value = _Cpt_EAU_Journalier

                            Dim dev As Object = Nothing
                            If String.IsNullOrEmpty(_ID_Cpt_EAU_Jm1) = False Then
                                dev = Server.ReturnRealDeviceById(_ID_Cpt_EAU_Jm1)
                                If IsNothing(dev) = False Then
                                    dev.Value = _Cpt_EAU_Jm1
                                    dev = Nothing
                                Else
                                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur l'ID du device Compteur EAU journalier - 1 est erroné (" & _ID_Cpt_EDF_Jm1 & ")")
                                End If
                            End If
                        End If

                        Exit Sub
                    End If
                Catch ex As Exception
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur lors des calculs liés à l'EAU: " & ex.Message)
                End Try

                '########################################################################################
                'CALCULS EDF
                '########################################################################################
                Try
                    If (Objet.ID = _ID_CptEDFJournalier) And String.IsNullOrEmpty(_ID_Cpt_EDF_Actuel) = False Then
                        'Recupe la valeur actuelle du compteur
                        _cpt_EDF_Actuel = CInt(Server.ReturnDeviceById(_idsrv, _ID_Cpt_EDF_Actuel).Value)

                        'Fixe la valeur du totaliseur journalier
                        If _LastDayEDF <> Now.Day Or (_Cpt_EDF_First_Day = 0 And _cpt_EDF_Actuel > 0) Then
                            _Cpt_EDF_First_Day = _cpt_EDF_Actuel
                            If _LastDayEDF <> Now.Day Then
                                _Cpt_EDF_Jm1 = _Cpt_EDF_First_Day
                                _Cpt_EDF_First_Day = 0
                                _LastDayEDF = Now.Day
                            End If
                        End If

                        'Pas de calcul si la valeur du compteur actuel = 0
                        If _cpt_EDF_Actuel > 0 Then
                            'Calcul de la valeur du compteur journalier
                            _Cpt_EDF_Journalier = _cpt_EDF_Actuel - _Cpt_EDF_First_Day
                            If _Cpt_EDF_Journalier < 0 Then _Cpt_EDF_Journalier = 0

                            'Mise a jour de la valeur du device
                            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Device:" & Objet.Name & " Adresse:" & Objet.Adresse1 & " Valeur:" & _Cpt_EDF_Journalier)
                            Objet.Value = _Cpt_EDF_Journalier

                            Dim dev As Object = Nothing
                            If String.IsNullOrEmpty(_ID_Cpt_EDF_Jm1) = False Then
                                dev = Server.ReturnRealDeviceById(_ID_Cpt_EDF_Jm1)
                                If IsNothing(dev) = False Then
                                    dev.Value = _Cpt_EDF_Jm1
                                    dev = Nothing
                                Else
                                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur l'ID du device Compteur EDF journalier - 1 est erroné (" & _ID_Cpt_EDF_Jm1 & ")")
                                End If
                            End If
                        End If

                        If String.IsNullOrEmpty(_ID_I_EDF_Actuel) = False And String.IsNullOrEmpty(_ID_IMax_EDF) = False And String.IsNullOrEmpty(_ID_Pourcent_Puiss_EDF) = False Then
                            Dim dev As Object = Nothing

                            'Recupe I actuelle
                            dev = Server.ReturnDeviceById(_idsrv, _ID_I_EDF_Actuel)
                            If IsNothing(dev) = False Then
                                _I_EDF_Actuel = dev.Value
                                dev = Nothing
                            Else
                                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur l'ID du device Intensité Actuelle est erroné (" & _ID_I_EDF_Actuel & ")")
                            End If
                            'Recupe I Max
                            dev = Server.ReturnDeviceById(_idsrv, _ID_IMax_EDF)
                            If IsNothing(dev) = False Then
                                _I_Max_EDF = dev.Value
                                dev = Nothing
                            Else
                                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur l'ID du device Intensité souscrite est erroné (" & _ID_IMax_EDF & ")")
                            End If

                            'Calcul % Puissance
                            If _I_EDF_Actuel > 0 And _I_Max_EDF > 0 Then
                                Try
                                    _Pourcent_Puiss_EDF = (((_I_EDF_Actuel * 220) / (_I_Max_EDF * 220)) * 100).ToString("00,0")

                                    dev = Server.ReturnRealDeviceById(_ID_Pourcent_Puiss_EDF)
                                    If IsNothing(dev) = False Then
                                        dev.Value = _Pourcent_Puiss_EDF
                                        dev = Nothing
                                    Else
                                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur l'ID du device % de puissance est erroné (" & _ID_Pourcent_Puiss_EDF & ")")
                                    End If
                                Catch ex As Exception
                                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur lors du calcul du % de puissance: " & ex.Message)
                                End Try
                            End If
                        End If

                        Exit Sub
                    End If
                Catch ex As Exception
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur lors des calculs liés à l'EDF: " & ex.Message)
                End Try

                '########################################################################################
                'CALCULS GAZ
                '########################################################################################
                Try
                    If (Objet.ID = _ID_CptGAZJournalier) And String.IsNullOrEmpty(_ID_Cpt_GAZ_Actuel) = False Then
                        'Recupe la valeur actuelle du compteur
                        _cpt_GAZ_Actuel = CInt(Server.ReturnDeviceById(_idsrv, _ID_Cpt_GAZ_Actuel).Value)

                        'Fixe la valeur du totaliseur journalier
                        If _LastDayGAZ <> Now.Day Or (_Cpt_GAZ_First_Day = 0 And _cpt_GAZ_Actuel > 0) Then
                            _Cpt_GAZ_First_Day = _cpt_GAZ_Actuel
                            If _LastDayGAZ <> Now.Day Then
                                _Cpt_GAZ_Jm1 = _Cpt_GAZ_First_Day
                                _Cpt_GAZ_First_Day = 0
                                _LastDayGAZ = Now.Day
                            End If
                        End If

                        'Pas de calcul si la valeur du compteur actuel = 0
                        If _cpt_GAZ_Actuel > 0 Then
                            'Calcul de la valeur du compteur journalier
                            _Cpt_GAZ_Journalier = _cpt_GAZ_Actuel - _Cpt_GAZ_First_Day
                            If _Cpt_GAZ_Journalier < 0 Then _Cpt_GAZ_Journalier = 0

                            'Mise a jour de la valeur du device
                            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Read", "Device:" & Objet.Name & " Adresse:" & Objet.Adresse1 & " Valeur:" & _Cpt_GAZ_Journalier)
                            Objet.Value = _Cpt_GAZ_Journalier

                            Dim dev As Object = Nothing
                            If String.IsNullOrEmpty(_ID_Cpt_GAZ_Jm1) = False Then
                                dev = Server.ReturnRealDeviceById(_ID_Cpt_GAZ_Jm1)
                                If IsNothing(dev) = False Then
                                    dev.Value = _Cpt_GAZ_Jm1
                                    dev = Nothing
                                End If
                            End If
                        End If
                    End If
                Catch ex As Exception
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur lors des calculs liés au GAZ: " & ex.Message)
                End Try
            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Le type de device " & Objet.Type & " n'est pas supporté pas ce driver")
            End If

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur : " & ex.ToString)
        End Try
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
            Return retour
        Catch ex As Exception
            Return "Une erreur est apparue lors de la vérification du champ " & Champ & ": " & ex.ToString
        End Try
    End Function

    ''' <summary>
    ''' Démarrer le driver
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        Try
            'récupération des paramétres avancés
            Try
                _ID_CptEauActuel = _Parametres.Item(0).Valeur
                _ID_CptEauJournalier = _Parametres.Item(1).Valeur
                _ID_Cpt_EAU_Jm1 = _Parametres.Item(2).Valeur

                _ID_Cpt_EDF_Actuel = _Parametres.Item(3).Valeur 'Compteur eau
                _ID_CptEDFJournalier = _Parametres.Item(4).Valeur 'Compteur eau journalier
                _ID_Cpt_EDF_Jm1 = _Parametres.Item(5).Valeur
                _ID_I_EDF_Actuel = _Parametres.Item(6).Valeur
                _ID_IMax_EDF = _Parametres.Item(7).Valeur
                _ID_Pourcent_Puiss_EDF = _Parametres.Item(8).Valeur

                _ID_Cpt_GAZ_Actuel = _Parametres.Item(9).Valeur
                _ID_CptGAZJournalier = _Parametres.Item(10).Valeur
                _ID_Cpt_GAZ_Jm1 = _Parametres.Item(11).Valeur

                _LastDayEau = 0
                _LastDayEDF = 0
                _LastDayGAZ = 0
                _IsConnect = True
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Start", "Driver démarré")
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "Erreur dans les paramétres avancés. utilisation des valeur par défaut" & ex.Message)
            End Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "Erreur lors du démarrage du driver: " & ex.ToString)
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

    ''' <summary>
    ''' Arrêter le driver
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Stop", "Driver arrêté")
            _IsConnect = False
            _LastDayEau = 0
            _LastDayEDF = 0
            _LastDayGAZ = 0
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Stop", "Erreur lors de l'arrêt du driver: " & ex.ToString)
            _IsConnect = False
        End Try
    End Sub

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

    ''' <summary>
    ''' Activer une sortie
    ''' </summary>
    ''' <param name="Objet"></param>
    ''' <param name="Commande"></param>
    ''' <param name="Parametre1"></param>
    ''' <param name="Parametre2"></param>
    ''' <remarks></remarks>
    Public Sub Write(ByVal Objet As Object, ByVal Commande As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
        If _Enable = False Then
            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Write", "Impossible de traiter cette commande car le driver n'est pas activé")
            Exit Sub
        End If
        If _IsConnect = False Then
            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Write", "Impossible de traiter cette commande car le driver n'est connecté")
            Exit Sub
        End If
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write", "Erreur: " & ex.ToString)
        End Try
    End Sub

    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice

    End Sub

    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice

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
            x.DescriptionCommand = Description
            x.CountParam = NbParam
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

    ''' <summary>
    ''' Déclaration du driver
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString

        'Devices supportés par le driver
        _DeviceSupport.Add(ListeDevices.GENERIQUEVALUE)
        _DeviceSupport.Add(ListeDevices.GENERIQUESTRING)

        'Paramétres avancés pouvant être définit côté Admin sur le driver
        Add_ParamAvance("ID Compteur Eau", "ID du device Compteur Eau", "")
        Add_ParamAvance("ID Compteur Eau Journalier", "ID Compteur Eau Journalier", "")
        Add_ParamAvance("ID Compteur Eau Journalier-1", "ID Compteur Eau Journalier-1", "")
        Add_ParamAvance("ID Compteur EDF", "ID du device Compteur EDF", "")
        Add_ParamAvance("ID Compteur EDF Journalier", "ID Compteur EDF Journalier", "")
        Add_ParamAvance("ID Compteur EDF Journalier-1", "ID Compteur EDF Journalier-1", "")
        Add_ParamAvance("ID Intensité Actuelle EDF", "ID Intensité Actuelle EDF", "")
        Add_ParamAvance("ID Intensité Max EDF", "ID Intensité Max EDF", "")
        Add_ParamAvance("ID % Puissance EDF", "ID % Puissance EDF", "")
        Add_ParamAvance("ID Compteur Gaz", "ID du device Compteur Gaz", "")
        Add_ParamAvance("ID Compteur Gaz Journalier", "ID Compteur Gaz Journalier", "")
        Add_ParamAvance("ID Compteur Gaz Journalier-1", "ID Compteur Gaz Journalier-1", "")

        Add_LibelleDevice("ADRESSE1", "Tag", "")
        Add_LibelleDevice("ADRESSE2", "@", "")
        Add_LibelleDevice("SOLO", "@", "")
        Add_LibelleDevice("MODELE", "@", "")
    End Sub
#End Region

#Region "Fonctions propres au driver"



#End Region


End Class
