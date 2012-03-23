#Region "Imports"
Imports System
Imports System.IO
Imports System.Data
Imports System.Data.Linq
Imports System.Xml
Imports System.Xml.XPath
Imports System.Xml.Serialization
Imports System.Reflection
Imports System.ServiceModel
Imports System.ServiceModel.Description
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web.HttpUtility
Imports System.Threading
Imports System.Data.SQLite
Imports System.Net.Mail
Imports taglib
#End Region

Namespace HoMIDom

    ''' <summary>Classe Server</summary>
    ''' <remarks></remarks>
    <Serializable()> Public Class Server
        Implements HoMIDom.IHoMIDom 'implémente l'interface dans cette class

#Region "Declaration des variables"

        Private Shared WithEvents _ListDrivers As New ArrayList 'Liste des drivers
        Private Shared _ListImgDrivers As New List(Of Driver)
        Private Shared WithEvents _ListDevices As New ArrayList 'Liste des devices
        <NonSerialized()> Private Shared _ListZones As New List(Of Zone) 'Liste des zones
        <NonSerialized()> Private Shared _ListUsers As New List(Of Users.User) 'Liste des users
        <NonSerialized()> Private Shared _ListMacros As New List(Of Macro) 'Liste des macros
        <NonSerialized()> Private Shared _ListTriggers As New List(Of Trigger) 'Liste de tous les triggers
        <NonSerialized()> Private Shared _ListGroups As New List(Of Groupes) 'Liste de tous les groupes
        <NonSerialized()> Private sqlite_homidom As New Sqlite("homidom") 'BDD sqlite pour Homidom
        <NonSerialized()> Private sqlite_medias As New Sqlite("medias") 'BDD sqlite pour les medias
        <NonSerialized()> Shared Soleil As New Soleil 'Déclaration class Soleil
        <NonSerialized()> Shared _Longitude As Double = 0 'Longitude
        <NonSerialized()> Shared _Latitude As Double = 0 'latitude
        <NonSerialized()> Private Shared _HeureLeverSoleil As DateTime 'heure du levé du soleil
        <NonSerialized()> Private Shared _HeureCoucherSoleil As DateTime 'heure du couché du soleil
        <NonSerialized()> Shared _HeureLeverSoleilCorrection As Integer = 0 'correction à appliquer sur heure du levé du soleil
        <NonSerialized()> Shared _HeureCoucherSoleilCorrection As Integer = 0 'correction à appliquer sur heure du couché du soleil
        <NonSerialized()> Shared _SMTPServeur As String = "smtp.homidom.fr" 'adresse du serveur SMTP
        <NonSerialized()> Shared _SMTPLogin As String = "" 'login du serveur SMTP
        <NonSerialized()> Shared _SMTPassword As String = "" 'password du serveur SMTP
        <NonSerialized()> Shared _SMTPmailEmetteur As String = "homidom@mail.com" 'adresse mail de l'émetteur
        <NonSerialized()> Private Shared _PortSOAP As String = "" 'Port IP de connexion SOAP
        <NonSerialized()> Private Shared _IPSOAP As String = "localhost" 'IP de connexion SOAP
        <NonSerialized()> Dim TimerSecond As New Timers.Timer 'Timer à la seconde
        <NonSerialized()> Shared _DateTimeLastStart As Date = Now
        <NonSerialized()> Private Shared _ListExtensionAudio As New List(Of Audio.ExtensionAudio) 'Liste des extensions audio
        <NonSerialized()> Private Shared _ListRepertoireAudio As New List(Of Audio.RepertoireAudio) 'Liste des répertoires audio
        <NonSerialized()> Private _ListTagAudio As New List(Of Audio.FilePlayList) ' Liste des tags des fichiers audio 
        <NonSerialized()> Public Shared Etat_server As Boolean 'etat du serveur : true = démarré
        <NonSerialized()> Dim fsw As FileSystemWatcher
        <NonSerialized()> Dim _MaxMonthLog As Integer = 2
        <NonSerialized()> Private Shared _TypeLogEnable As New List(Of Boolean) 'True si on doit pas prendre en compte le type de log
        <NonSerialized()> Shared _4Log(3) As String  'Le serveur est prêt
        <NonSerialized()> Shared _CycleSave As Integer  'Enregistrer toute les X minutes
        <NonSerialized()> Shared _NextTimeSave As DateTime  'Enregistrer toute les X minutes
        <NonSerialized()> Shared _Finish As Boolean  'Le serveur est prêt
        Private Shared lock_logwrite As New Object

#End Region

#Region "Event"
        '********************************************************************
        'Gestion des Evènements
        '********************************************************************

        ''' <summary>Evenement provenant des drivers </summary>
        ''' <param name="DriveName"></param>
        ''' <param name="TypeEvent"></param>
        ''' <param name="Parametre"></param>
        ''' <remarks></remarks>
        Public Sub DriversEvent(ByVal DriveName As String, ByVal TypeEvent As String, ByVal Parametre As Object)
            Try
                If Etat_server Then

                End If
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DriversEvent", "Exception : " & ex.Message)
            End Try
        End Sub

        ''' <summary>Evenement provenant des devices</summary>
        ''' <param name="Device"></param>
        ''' <param name="Property"></param>
        ''' <param name="Parametres"></param>
        ''' <remarks></remarks>
        Public Sub DeviceChange(ByVal Device As Object, ByVal [Property] As String, ByVal Parametres As Object)
            Dim retour As String = ""

            Try
                If Etat_server Then
                    Dim valeur = Parametres
                    '--- on logue tout ce qui arrive en mode debug
                    Log(TypeLog.DEBUG, TypeSource.SERVEUR, "DeviceChange", "Le device " & Device.name & " a changé : " & [Property] & " = " & valeur.ToString)

                    If Mid(valeur.ToString, 1, 4) <> "ERR:" Then 'si y a pas erreur d'acquisition

                        '------------------------------------------------------------------------------------------------
                        '    MACRO/Triggers
                        '------------------------------------------------------------------------------------------------
                        Try
                            'Parcour des triggers pour vérifier si le device déclenche des macros
                            For i As Integer = 0 To _ListTriggers.Count - 1
                                If _ListTriggers.Item(i).Enable = True Then
                                    If _ListTriggers.Item(i).Type = Trigger.TypeTrigger.DEVICE And Device.id = _ListTriggers.Item(i).ConditionDeviceId And _ListTriggers.Item(i).ConditionDeviceProperty = [Property] Then 'c'est un trigger type device + enable + device concerné
                                        Log(TypeLog.DEBUG, TypeSource.SERVEUR, "DeviceChange", " -> " & Device.name & " est associé au trigger : " & _ListTriggers.Item(i).Nom)
                                        'on lance toutes les macros associés
                                        For j As Integer = 0 To _ListTriggers.Item(i).ListMacro.Count - 1
                                            Dim _m As Macro = ReturnMacroById(_IdSrv, _ListTriggers.Item(i).ListMacro.Item(j))
                                            Log(TypeLog.DEBUG, TypeSource.SERVEUR, "DeviceChange", " --> " & _ListTriggers.Item(i).Nom & " Lance la macro : " & _m.Nom)
                                            If _m IsNot Nothing Then _m.Execute(Me)
                                            _m = Nothing
                                        Next
                                    End If
                                End If
                            Next
                        Catch ex As Exception
                            Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeviceChange", "Macro/Triggers Exception : " & ex.Message)
                        End Try

                        '------------------------------------------------------------------------------------------------
                        '    HISTORIQUE
                        '------------------------------------------------------------------------------------------------
                        Try
                            'Ajout dans la BDD
                            retour = sqlite_homidom.nonquery("INSERT INTO historiques (device_id,source,dateheure,valeur) VALUES (@parameter0, @parameter1, @parameter2, @parameter3)", Device.ID, [Property], Now.ToString(), valeur)
                            If Mid(retour, 1, 4) = "ERR:" Then
                                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeviceChange", "Erreur Requete sqlite : " & retour)
                            End If
                        Catch ex As Exception
                            Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeviceChange", "Historique Exception : " & ex.Message)
                        End Try

                        'Ancienne gestion, maintenant directement dans les devices
                        '--- si on teste la value (et non les autres propriétés d'un device) et si lastetat=True, on vérifie que la valeur a changé par rapport a l'avant dernier etat (valuelast) 
                        'If [Property] = "Value" Then

                        '    If TypeOf Device.value Is Double Or TypeOf Device.value Is Integer Then
                        '        'le device est de type double/integer (nombre), on converti la valeur récupérée en string pour la traiter
                        '        Dim valeurstring As String = valeur.ToString
                        '        '--- Remplacement de , par .
                        '        valeurstring = valeurstring.Replace(",", ".") '?encore besoin ???

                        '        '--- si lastetat=True, on vérifie que la valeur a changé par rapport a l'avant dernier etat (valuelast) 
                        '        If Device.LastEtat And valeurstring = Device.ValueLast.ToString Then
                        '            'log de "inchangé lastetat"
                        '            Log(TypeLog.VALEUR_INCHANGE_LASTETAT, TypeSource.SERVEUR, "DeviceChange", Device.Name.ToString() & " : " & Device.Adresse1 & " : " & valeurstring & " (inchangé lastetat " & Device.ValueLast.ToString & ")")
                        '        Else
                        '            '--- on vérifie que la valeur a changé de plus de composants_precision sinon inchangé
                        '            If (CDbl(valeur) + CDbl(Device.Precision)) >= CDbl(Device.Value) And (CDbl(valeur) - CDbl(Device.Precision)) <= CDbl(Device.Value) Then
                        '                'log de "inchangé précision"
                        '                Log(TypeLog.VALEUR_INCHANGE_PRECISION, TypeSource.SERVEUR, "DeviceChange", Device.Name.ToString() & " : " & Device.Adresse1 & " : " & valeurstring & " (inchangé precision " & Device.ValueLast.ToString & ")")
                        '            Else
                        '                'log de la nouvelle valeur
                        '                Log(TypeLog.VALEUR_CHANGE, TypeSource.SERVEUR, "DeviceChange", Device.Name.ToString() & " : " & Device.Adresse1 & " : " & valeurstring)
                        '                'On historise la nouvellevaleur
                        '                retour = sqlite_homidom.nonquery("INSERT INTO historiques (device_id,source,dateheure,valeur) VALUES (@parameter0, @parameter1, @parameter2, @parameter3)", Device.ID, [Property], Now.ToString(), valeurstring)
                        '                If Mid(retour, 1, 4) = "ERR:" Then
                        '                    Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeviceChange", "Erreur lors Requete sqlite : " & retour)
                        '                End If
                        '            End If
                        '        End If

                        '    ElseIf TypeOf Device.value Is Boolean Or TypeOf Device.value Is String Then
                        '        '--- Valeur est autre chose qu'un nombre
                        '        '--- historise la valeur si ce n'est pas une simple info de config
                        '        If Mid(valeur.ToString, 1, 4) <> "CFG:" Then
                        '            '--- si lastetat=True, on vérifie que la valeur a changé par rapport a l'avant dernier etat (valuelast) 
                        '            If Device.LastEtat And valeur.ToString = Device.ValueLast.ToString Then
                        '                'log de "inchangé lastetat"
                        '                Log(TypeLog.VALEUR_INCHANGE_LASTETAT, TypeSource.SERVEUR, "DeviceChange", Device.Name.ToString() & " : " & Device.Adresse1 & " : " & valeur.ToString & " (inchangé lastetat " & Device.ValueLast.ToString & ")")
                        '            Else
                        '                'log de la nouvelle valeur
                        '                Log(TypeLog.VALEUR_CHANGE, TypeSource.SERVEUR, "DeviceChange", Device.Name.ToString() & " : " & Device.Adresse1 & " : " & valeur.ToString)
                        '                'Ajout dans la BDD
                        '                retour = sqlite_homidom.nonquery("INSERT INTO historiques (device_id,source,dateheure,valeur) VALUES (@parameter0, @parameter1, @parameter2, @parameter3)", Device.ID, [Property], Now.ToString(), valeur.ToString)
                        '                If Mid(retour, 1, 4) = "ERR:" Then
                        '                    Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeviceChange", "Erreur lors Requete sqlite : " & retour)
                        '                End If
                        '            End If
                        '        Else
                        '            'log de l'info de config
                        '            Log(TypeLog.VALEUR_CHANGE, TypeSource.SERVEUR, "DeviceChange", Device.Name.ToString() & " : " & Device.Adresse1 & " : " & valeur.ToString)
                        '        End If
                        '    Else
                        '        '-- Type non reconnu : ERREUR
                        '        Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeviceChange", "Type de device non reconnu " & Device.Name.ToString)
                        '    End If


                        'Else
                        '    'C'est une autre propriété, on logue directement et stocke la modif
                        '    Log(TypeLog.VALEUR_CHANGE, TypeSource.SERVEUR, "DeviceChange", Device.Name.ToString() & " : " & Device.Adresse1 & " : " & valeur & " (" & [Property] & ")")
                        '    'Ajout dans la BDD
                        '    retour = sqlite_homidom.nonquery("INSERT INTO historiques (device_id,source,dateheure,valeur) VALUES (@parameter0, @parameter1, @parameter2, @parameter3)", Device.ID, [Property], Now.ToString(), valeur)
                        '    If Mid(retour, 1, 4) = "ERR:" Then
                        '        Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeviceChange", "Erreur lors Requete sqlite : " & retour)
                        '    End If
                        'End If
                    Else
                        'erreur d'acquisition
                        Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeviceChange", "Erreur d'acquisition : " & Device.Name & " - " & valeur.ToString)
                    End If
                End If
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeviceChange", "Exception : " & ex.Message)
            End Try
        End Sub

        ''' <summary>Traitement à effectuer toutes les secondes/minutes/heures/minuit/midi</summary>
        ''' <remarks></remarks>
        Sub TimerSecTick()
            Dim ladate As DateTime = Now 'on récupére la date/heure

            '---- Action à effectuer toutes les secondes ----
            Dim thr As New Thread(AddressOf VerifTimeDevice)
            thr.IsBackground = True
            thr.Start()

            If _CycleSave > 0 And _Finish = True Then
                If ladate.Minute >= _NextTimeSave.Minute Then
                    _NextTimeSave = Now.AddMinutes(_CycleSave)
                    SaveConfig(_MonRepertoire & "\config\homidom.xml")
                End If
            End If

            Try
                '---- Actions à effectuer toutes les minutes ----
                If ladate.Second = 0 Then
                    VerifIsJour()
                End If

                '---- Actions à effectuer toutes les heures ----
                'If ladate.Minute = 59 And ladate.Second = 59 Then

                'End If

                '---- Actions à effectuer à minuit ----
                If ladate.Hour = 0 And ladate.Minute = 0 And ladate.Second = 0 Then
                    MAJ_HeuresSoleil()
                    CleanLog(_MaxMonthLog)
                End If

                '---- Actions à effectuer à midi ----
                If ladate.Hour = 12 And ladate.Minute = 0 And ladate.Second = 0 Then
                    MAJ_HeuresSoleil()
                End If

                ladate = Nothing
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "TimerSecTick", "Exception : " & ex.Message)
            End Try
        End Sub

        Private Sub VerifTimeDevice()
            'on checke si il y a cron à faire
            Try
                For i As Integer = 0 To _listTriggers.Count() - 1
                    If _listTriggers.Item(i).Type = Trigger.TypeTrigger.TIMER Then
                        If _listTriggers.Item(i).Enable = True Then
                            If _listTriggers.Item(i).Prochainedateheure <= DateAndTime.Now.ToString("yyyy-MM-dd HH:mm:ss") Then
                                _listTriggers.Item(i).maj_cron() 'reprogrammation du prochain shedule
                                'lancement des macros associées
                                For j As Integer = 0 To _listTriggers.Item(i).ListMacro.Count - 1
                                    'on cherche la macro et on la lance en testant ces conditions
                                    Dim _m As Macro = ReturnMacroById(_IdSrv, _listTriggers.Item(i).ListMacro.Item(j))
                                    If _m IsNot Nothing Then _m.Execute(Me)
                                    _m = Nothing
                                Next
                            End If
                        End If
                    End If
                Next
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "TimerSecTick TriggerTimer", "Exception : " & ex.Message)
            End Try
        End Sub
#End Region

#Region "Fonctions/Sub propres au serveur"

#Region "Serveur"
        ''' <summary>
        ''' Vérifie si l'IDsrv est correct, retourne True si ok
        ''' </summary>
        ''' <param name="Value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function VerifIdSrv(ByVal Value As String) As Boolean
            Try
                If Value = _IdSrv Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "VerifIdSrv", "Exception : " & ex.Message)
            End Try
        End Function
#End Region

#Region "Soleil"
        Private Sub VerifIsJour()
            If _HeureLeverSoleil <= Now And _HeureCoucherSoleil >= Now Then
                For i As Integer = 0 To _ListDevices.Count - 1
                    If _ListDevices.Item(i).id = "soleil01" Then
                        If _ListDevices.Item(i).value = False Then _ListDevices.Item(i).value = True
                        Exit For
                    End If
                Next
            Else
                For i As Integer = 0 To _ListDevices.Count - 1
                    If _ListDevices.Item(i).id = "soleil01" Then
                        If _ListDevices.Item(i).value = True Then _ListDevices.Item(i).value = False
                        Exit For
                    End If
                Next
            End If
        End Sub

        ''' <summary>Initialisation des heures du soleil</summary>
        ''' <remarks></remarks>
        Public Sub MAJ_HeuresSoleil()
            Dim dtSunrise As Date
            Dim dtSolarNoon As Date
            Dim dtSunset As Date
            Try
                Soleil.CalculateSolarTimes(_Latitude, _Longitude, Date.Now, dtSunrise, dtSolarNoon, dtSunset)
                Log(TypeLog.INFO, TypeSource.SERVEUR, "MAJ_HeuresSoleil", "Initialisation des heures du soleil")
                _HeureCoucherSoleil = DateAdd(DateInterval.Minute, _HeureCoucherSoleilCorrection, dtSunset)
                _HeureLeverSoleil = DateAdd(DateInterval.Minute, _HeureLeverSoleilCorrection, dtSunrise)

                Log(TypeLog.INFO, TypeSource.SERVEUR, "MAJ_HeuresSoleil", "Heure du lever : " & _HeureLeverSoleil)
                Log(TypeLog.INFO, TypeSource.SERVEUR, "MAJ_HeuresSoleil", "Heure du coucher : " & _HeureCoucherSoleil)

                VerifIsJour()
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "MAJ_HeuresSoleil", "Exception : " & ex.Message)
            End Try
        End Sub
#End Region

#Region "Configuration"
        ''' <summary>Chargement de la config depuis le fichier XML</summary>
        ''' <param name="Fichier"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LoadConfig(ByVal Fichier As String) As String
            'Copy du fichier de config avant chargement
            Try
                Dim _file As String = Fichier & "homidom"
                If IO.File.Exists(_file & ".bak") = True Then IO.File.Delete(_file & ".bak")
                IO.File.Copy(_file & ".xml", Mid(_file & ".xml", 1, Len(_file & ".xml") - 4) & ".bak")
                Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Création du backup (.bak) du fichier de config avant chargement")
                _file = Nothing
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "LoadConfig", "Erreur impossible de créer une copie de backup du fichier de config: " & ex.Message)
            End Try

            Try
                Dim dirInfo As New System.IO.DirectoryInfo(Fichier)
                Dim file As System.IO.FileInfo
                Dim files() As System.IO.FileInfo = dirInfo.GetFiles("homidom.xml")
                Dim myxml As XML

                If (files IsNot Nothing) Then
                    For Each file In files
                        Dim myfile As String = file.FullName
                        Dim list As XmlNodeList

                        myxml = New XML(myfile)

                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Chargement du fichier config: " & myfile)

                        '******************************************
                        'on va chercher les paramètres du serveur
                        '******************************************
                        list = myxml.SelectNodes("/homidom/server")
                        If list.Count > 0 Then 'présence des paramètres du server
                            For j As Integer = 0 To list.Item(0).Attributes.Count - 1
                                Select Case list.Item(0).Attributes.Item(j).Name
                                    Case "longitude"
                                        _Longitude = list.Item(0).Attributes.Item(j).Value.Replace(".", ",")
                                    Case "latitude"
                                        _Latitude = list.Item(0).Attributes.Item(j).Value.Replace(".", ",")
                                    Case "heurecorrectionlever"
                                        _HeureLeverSoleilCorrection = list.Item(0).Attributes.Item(j).Value
                                    Case "heurecorrectioncoucher"
                                        _HeureCoucherSoleilCorrection = list.Item(0).Attributes.Item(j).Value
                                    Case "ipsoap"
                                        _IPSOAP = list.Item(0).Attributes.Item(j).Value
                                    Case "portsoap"
                                        _PortSOAP = list.Item(0).Attributes.Item(j).Value
                                    Case "idsrv"
                                        _IdSrv = list.Item(0).Attributes.Item(j).Value
                                    Case "smtpserver"
                                        _SMTPServeur = list.Item(0).Attributes.Item(j).Value
                                    Case "smtpmail"
                                        _SMTPmailEmetteur = list.Item(0).Attributes.Item(j).Value
                                    Case "smtplogin"
                                        _SMTPLogin = list.Item(0).Attributes.Item(j).Value
                                    Case "smtppassword"
                                        _SMTPassword = list.Item(0).Attributes.Item(j).Value
                                    Case "log0"
                                        _TypeLogEnable(0) = list.Item(0).Attributes.Item(j).Value
                                    Case "log1"
                                        _TypeLogEnable(1) = list.Item(0).Attributes.Item(j).Value
                                    Case "log2"
                                        _TypeLogEnable(2) = list.Item(0).Attributes.Item(j).Value
                                    Case "log3"
                                        _TypeLogEnable(3) = list.Item(0).Attributes.Item(j).Value
                                    Case "log4"
                                        _TypeLogEnable(4) = list.Item(0).Attributes.Item(j).Value
                                    Case "log5"
                                        _TypeLogEnable(5) = list.Item(0).Attributes.Item(j).Value
                                    Case "log6"
                                        _TypeLogEnable(6) = list.Item(0).Attributes.Item(j).Value
                                    Case "log7"
                                        _TypeLogEnable(7) = list.Item(0).Attributes.Item(j).Value
                                    Case "log8"
                                        _TypeLogEnable(8) = list.Item(0).Attributes.Item(j).Value
                                    Case "log9"
                                        _TypeLogEnable(9) = list.Item(0).Attributes.Item(j).Value
                                    Case "cyclesave"
                                        _CycleSave = list.Item(0).Attributes.Item(j).Value
                                    Case Else
                                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Un attribut correspondant au serveur est inconnu: nom:" & list.Item(0).Attributes.Item(j).Name & " Valeur: " & list.Item(0).Attributes.Item(j).Value)
                                End Select
                            Next
                        Else
                            MsgBox("Il manque les paramètres du serveur dans le fichier de config !!", MsgBoxStyle.Exclamation, "Erreur serveur")
                        End If
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Paramètres du serveur chargés")

                        '********************************
                        'on va chercher les drivers
                        '*********************************
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Chargement des drivers :")
                        list = Nothing
                        list = myxml.SelectNodes("/homidom/drivers/driver")

                        If list.Count > 0 Then 'présence d'un ou des driver(s)
                            For j As Integer = 0 To list.Count - 1
                                'on récupère l'id du driver
                                Dim _IdDriver = list.Item(j).Attributes.Item(0).Value
                                Dim _drv As IDriver = ReturnDrvById(_IdSrv, _IdDriver)

                                If _drv IsNot Nothing Then
                                    _drv.Enable = list.Item(j).Attributes.GetNamedItem("enable").Value
                                    _drv.StartAuto = list.Item(j).Attributes.GetNamedItem("startauto").Value
                                    _drv.IP_TCP = list.Item(j).Attributes.GetNamedItem("iptcp").Value
                                    _drv.Port_TCP = list.Item(j).Attributes.GetNamedItem("porttcp").Value
                                    _drv.IP_UDP = list.Item(j).Attributes.GetNamedItem("ipudp").Value
                                    _drv.Port_UDP = list.Item(j).Attributes.GetNamedItem("portudp").Value
                                    _drv.COM = list.Item(j).Attributes.GetNamedItem("com").Value
                                    _drv.Refresh = list.Item(j).Attributes.GetNamedItem("refresh").Value
                                    _drv.Modele = list.Item(j).Attributes.GetNamedItem("modele").Value
                                    _drv.Picture = list.Item(j).Attributes.GetNamedItem("picture").Value

                                    For i As Integer = 0 To list.Item(j).Attributes.Count - 1
                                        Dim a As String = UCase(list.Item(j).Attributes.Item(i).Name)
                                        If a.StartsWith("PARAMETRE") Then
                                            Dim idx As Integer = Mid(a, 10, Len(a) - 9)
                                            If idx < _drv.Parametres.Count Then
                                                _drv.Parametres.Item(idx).valeur = list.Item(j).Attributes.Item(i).Value
                                            End If
                                        End If
                                        a = Nothing
                                    Next

                                    Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", " - " & _drv.Nom & " chargé")
                                    _drv = Nothing
                                End If
                            Next
                        Else
                            Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Aucun driver n'est enregistré dans le fichier de config")
                        End If

                        '******************************************
                        'on va chercher les zones
                        '******************************************
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Chargement des zones :")
                        list = Nothing
                        list = myxml.SelectNodes("/homidom/zones/zone")
                        If list.Count > 0 Then 'présence des zones
                            For i As Integer = 0 To list.Count - 1
                                Dim x As New Zone
                                For j As Integer = 0 To list.Item(i).Attributes.Count - 1
                                    Select Case list.Item(i).Attributes.Item(j).Name
                                        Case "id"
                                            x.ID = list.Item(i).Attributes.Item(j).Value
                                        Case "name"
                                            x.Name = list.Item(i).Attributes.Item(j).Value
                                        Case "icon"
                                            If list.Item(i).Attributes.Item(j).Value <> Nothing Then
                                                If IO.File.Exists(list.Item(0).Attributes.Item(j).Value) Then
                                                    x.Icon = list.Item(0).Attributes.Item(j).Value
                                                Else
                                                    x.Icon = _MonRepertoire & "\images\Zones\icon\defaut.png"
                                                End If
                                            Else
                                                x.Icon = _MonRepertoire & "\images\Zones\icon\defaut.png"
                                            End If
                                        Case "image"
                                            If list.Item(i).Attributes.Item(j).Value <> Nothing Then
                                                If IO.File.Exists(list.Item(0).Attributes.Item(j).Value) Then
                                                    x.Image = list.Item(0).Attributes.Item(j).Value
                                                Else
                                                    x.Image = _MonRepertoire & "\images\Zones\image\defaut.jpg"
                                                End If
                                            Else
                                                x.Image = _MonRepertoire & "\images\Zones\image\defaut.jpg"
                                            End If
                                        Case Else
                                                Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", " -> Un attribut correspondant à la zone est inconnu: nom:" & list.Item(i).Attributes.Item(j).Name & " Valeur: " & list.Item(0).Attributes.Item(j).Value)
                                    End Select
                                Next
                                If list.Item(i).HasChildNodes = True Then
                                    For k As Integer = 0 To list.Item(i).ChildNodes.Count - 1
                                        If list.Item(i).ChildNodes.Item(k).Name = "element" Then
                                            Dim _dev As New Zone.Element_Zone(list.Item(i).ChildNodes.Item(k).Attributes(0).Value, list.Item(i).ChildNodes.Item(k).Attributes(1).Value)
                                            x.ListElement.Add(_dev)
                                        End If
                                    Next
                                End If
                                _ListZones.Add(x)
                            Next
                        Else
                            Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", " -> Aucune zone enregistrée dans le fichier de config")
                        End If
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", " -> " & _ListZones.Count & " Zone(s) chargée(s)")

                        '******************************************
                        'on va chercher les users
                        '******************************************
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Chargement des users :")
                        list = Nothing
                        list = myxml.SelectNodes("/homidom/users/user")
                        If list.Count > 0 Then 'présence des users
                            For i As Integer = 0 To list.Count - 1
                                Dim x As New Users.User
                                For j As Integer = 0 To list.Item(i).Attributes.Count - 1
                                    Select Case list.Item(i).Attributes.Item(j).Name
                                        Case "id"
                                            x.ID = list.Item(i).Attributes.Item(j).Value
                                        Case "username"
                                            x.UserName = list.Item(i).Attributes.Item(j).Value
                                        Case "nom"
                                            x.Nom = list.Item(i).Attributes.Item(j).Value
                                        Case "prenom"
                                            x.Prenom = list.Item(i).Attributes.Item(j).Value
                                        Case "profil"
                                            x.Profil = list.Item(i).Attributes.Item(j).Value
                                        Case "password"
                                            x.Password = list.Item(i).Attributes.Item(j).Value
                                        Case "numberidentification"
                                            x.NumberIdentification = list.Item(i).Attributes.Item(j).Value
                                        Case "image"
                                            If list.Item(i).Attributes.Item(j).Value <> Nothing Then
                                                If IO.File.Exists(list.Item(0).Attributes.Item(j).Value) Then
                                                    x.Image = list.Item(0).Attributes.Item(j).Value
                                                Else
                                                    x.Image = _MonRepertoire & "\images\icones\user_128.png"
                                                End If
                                            Else
                                                x.Image = _MonRepertoire & "\images\icones\user_128.png"
                                            End If
                                        Case "email"
                                                x.eMail = list.Item(i).Attributes.Item(j).Value
                                        Case "emailautre"
                                                x.eMailAutre = list.Item(i).Attributes.Item(j).Value
                                        Case "telfixe"
                                                x.TelFixe = list.Item(i).Attributes.Item(j).Value
                                        Case "telmobile"
                                                x.TelMobile = list.Item(i).Attributes.Item(j).Value
                                        Case "telautre"
                                                x.TelAutre = list.Item(i).Attributes.Item(j).Value
                                        Case "adresse"
                                                x.Adresse = list.Item(i).Attributes.Item(j).Value
                                        Case "ville"
                                                x.Ville = list.Item(i).Attributes.Item(j).Value
                                        Case "codepostal"
                                                x.CodePostal = list.Item(i).Attributes.Item(j).Value
                                        Case Else
                                                Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", " -> Un attribut correspondant à la zone est inconnu: nom:" & list.Item(i).Attributes.Item(j).Name & " Valeur: " & list.Item(0).Attributes.Item(j).Value)
                                    End Select
                                Next
                                _ListUsers.Add(x)
                            Next
                        Else
                            Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", " -> Création du user admin par défaut !!")
                            SaveUser(_IdSrv, "", "Admin", "password", Users.TypeProfil.admin, "Administrateur", "Admin")
                        End If
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", " -> " & _ListUsers.Count & " Users(s) chargé(s)")


                        '******************************************
                        'on va chercher les devices
                        '********************************************
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Chargement des devices :")
                        list = Nothing
                        list = myxml.SelectNodes("/homidom/devices/device")
                        Dim trvSoleil As Boolean = False

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
                                    Case "BAROMETRE"
                                        Dim o As New Device.BAROMETRE(Me)
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "BATTERIE"
                                        Dim o As New Device.BATTERIE(Me)
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "COMPTEUR"
                                        Dim o As New Device.COMPTEUR(Me)
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "CONTACT"
                                        Dim o As New Device.CONTACT(Me)
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "DETECTEUR"
                                        Dim o As New Device.DETECTEUR(Me)
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
                                    Case "GENERIQUEBOOLEEN"
                                        Dim o As New Device.GENERIQUEBOOLEEN(Me)
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "GENERIQUESTRING"
                                        Dim o As New Device.GENERIQUESTRING(Me)
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "GENERIQUEVALUE"
                                        Dim o As New Device.GENERIQUEVALUE(Me)
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
                                    If (Not list.Item(j).Attributes.GetNamedItem("id") Is Nothing) Then .ID = list.Item(j).Attributes.GetNamedItem("id").Value
                                    If (Not list.Item(j).Attributes.GetNamedItem("name") Is Nothing) Then .Name = list.Item(j).Attributes.GetNamedItem("name").Value
                                    If (Not list.Item(j).Attributes.GetNamedItem("enable") Is Nothing) Then .Enable = list.Item(j).Attributes.GetNamedItem("enable").Value
                                    If (Not list.Item(j).Attributes.GetNamedItem("driverid") Is Nothing) Then .DriverId = list.Item(j).Attributes.GetNamedItem("driverid").Value
                                    If (Not list.Item(j).Attributes.GetNamedItem("description") Is Nothing) Then .Description = list.Item(j).Attributes.GetNamedItem("description").Value
                                    If (Not list.Item(j).Attributes.GetNamedItem("adresse1") Is Nothing) Then .Adresse1 = list.Item(j).Attributes.GetNamedItem("adresse1").Value
                                    If (Not list.Item(j).Attributes.GetNamedItem("adresse2") Is Nothing) Then .Adresse2 = list.Item(j).Attributes.GetNamedItem("adresse2").Value
                                    If (Not list.Item(j).Attributes.GetNamedItem("datecreated") Is Nothing) Then .DateCreated = list.Item(j).Attributes.GetNamedItem("datecreated").Value
                                    If (Not list.Item(j).Attributes.GetNamedItem("lastchange") Is Nothing) Then .LastChange = list.Item(j).Attributes.GetNamedItem("lastchange").Value
                                    If (Not list.Item(j).Attributes.GetNamedItem("lastchangeduree") Is Nothing) Then .LastChangeDuree = list.Item(j).Attributes.GetNamedItem("lastchangeduree").Value
                                    If (Not list.Item(j).Attributes.GetNamedItem("refresh") Is Nothing) Then .Refresh = Replace(list.Item(j).Attributes.GetNamedItem("refresh").Value, ".", ",")
                                    If (Not list.Item(j).Attributes.GetNamedItem("modele") Is Nothing) Then .Modele = list.Item(j).Attributes.GetNamedItem("modele").Value
                                    If list.Item(j).Attributes.GetNamedItem("picture").Value IsNot Nothing Then
                                        If IO.File.Exists(.Picture = list.Item(j).Attributes.GetNamedItem("picture").Value) Then
                                            .Picture = list.Item(j).Attributes.GetNamedItem("picture").Value
                                        Else
                                            Dim fileimg As String = _MonRepertoire & "\images\Devices\" & LCase(_Dev.type) & "-defaut.png"
                                            If IO.File.Exists(fileimg) Then
                                                .Picture = fileimg
                                            Else
                                                .Picture = _MonRepertoire & "\images\icones\composant_128.png"
                                            End If
                                        End If
                                    Else
                                        .Picture = _MonRepertoire & "\images\icones\composant_128.png"
                                    End If
                                    If (Not list.Item(j).Attributes.GetNamedItem("solo") Is Nothing) Then .Solo = list.Item(j).Attributes.GetNamedItem("solo").Value
                                    If (Not list.Item(j).Attributes.GetNamedItem("value") Is Nothing) Then .Value = Replace(list.Item(j).Attributes.GetNamedItem("value").Value, ".", ",")
                                    If (Not list.Item(j).Attributes.GetNamedItem("valuelast") Is Nothing) Then .ValueLast = Replace(list.Item(j).Attributes.GetNamedItem("valuelast").Value, ".", ",")
                                    If (Not list.Item(j).Attributes.GetNamedItem("lastetat") Is Nothing) Then .LastEtat = list.Item(j).Attributes.GetNamedItem("lastetat").Value
                                    '-- propriétés generique value --
                                    If _Dev.Type = "BAROMETRE" _
                                    Or _Dev.Type = "COMPTEUR" _
                                    Or _Dev.Type = "ENERGIEINSTANTANEE" _
                                    Or _Dev.Type = "ENERGIETOTALE" _
                                    Or _Dev.Type = "GENERIQUEVALUE" _
                                    Or _Dev.Type = "HUMIDITE" _
                                    Or _Dev.Type = "LAMPE" _
                                    Or _Dev.Type = "PLUIECOURANT" _
                                    Or _Dev.Type = "PLUIETOTAL" _
                                    Or _Dev.Type = "TEMPERATURE" _
                                    Or _Dev.Type = "TEMPERATURECONSIGNE" _
                                    Or _Dev.Type = "VITESSEVENT" _
                                    Or _Dev.Type = "UV" _
                                    Or _Dev.Type = "VOLET" _
                                    Then
                                        If (Not list.Item(j).Attributes.GetNamedItem("valuemin") Is Nothing) Then .ValueMin = list.Item(j).Attributes.GetNamedItem("valuemin").Value
                                        If (Not list.Item(j).Attributes.GetNamedItem("valuemax") Is Nothing) Then .ValueMax = list.Item(j).Attributes.GetNamedItem("valuemax").Value
                                        If (Not list.Item(j).Attributes.GetNamedItem("valuedef") Is Nothing) Then .ValueDef = list.Item(j).Attributes.GetNamedItem("valuedef").Value
                                        If (Not list.Item(j).Attributes.GetNamedItem("precision") Is Nothing) Then .Precision = list.Item(j).Attributes.GetNamedItem("precision").Value
                                        If (Not list.Item(j).Attributes.GetNamedItem("correction") Is Nothing) Then .Correction = list.Item(j).Attributes.GetNamedItem("correction").Value
                                        If (Not list.Item(j).Attributes.GetNamedItem("formatage") Is Nothing) Then .Formatage = list.Item(j).Attributes.GetNamedItem("formatage").Value
                                    End If
                                    '-- cas spécifique du multimedia pour récupérer les commandes IR --
                                    'If _Dev.type = "MULTIMEDIA" Then
                                    '    For k As Integer = 0 To list.Item(j).ChildNodes.Count - 1
                                    '        If list.Item(j).ChildNodes.Item(k).Name = "commands" Then
                                    '            _Dev.ListCommandName.Clear()
                                    '            _Dev.ListCommandData.Clear()
                                    '            _Dev.ListCommandRepeat.Clear()
                                    '            For k1 As Integer = 0 To list.Item(j).ChildNodes.Item(k).ChildNodes.Count - 1
                                    '                _Dev.ListCommandName.Add(list.Item(j).ChildNodes.Item(k).ChildNodes.Item(k1).Attributes(0).Value)
                                    '                _Dev.ListCommandData.Add(list.Item(j).ChildNodes.Item(k).ChildNodes.Item(k1).Attributes(1).Value)
                                    '                _Dev.ListCommandRepeat.Add(list.Item(j).ChildNodes.Item(k).ChildNodes.Item(k1).Attributes(2).Value)
                                    '            Next
                                    '        End If
                                    '    Next
                                    'End If
                                    If .ID <> "" And .Name <> "" And .Adresse1 <> "" And .DriverId <> "" Then
                                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", " - " & .Name & " (" & .ID & " - " & .Adresse1 & " - " & .Type & ")")
                                        If .ID = "soleil01" Then
                                            trvSoleil = True
                                        End If
                                    Else
                                        _Dev.Enable = False
                                        Log(TypeLog.ERREUR, TypeSource.SERVEUR, "LoadConfig", " -> Erreur lors du chargement du device (information incomplete -> Disable) " & .Name & " (" & .ID & " - " & .Adresse1 & " - " & .Type & ")")
                                    End If
                                End With
                                _ListDevices.Add(_Dev)
                                _Dev = Nothing
                            Next
                            If trvSoleil = False Then
                                Dim _Devs As New Device.GENERIQUEBOOLEEN(Me)
                                _Devs.ID = "soleil01"
                                _Devs.Name = "HOMI_Jour"
                                _Devs.Enable = True
                                _Devs.Adresse1 = "N/A"
                                _Devs.Description = "Levé/Couché du soleil : True si il fait jour"
                                _Devs.DriverID = "DE96B466-2540-11E0-A321-65D7DFD72085"
                                _ListDevices.Add(_Devs)
                                Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", " - " & _Devs.Name & " (" & _Devs.ID & " - " & _Devs.Adresse1 & " - " & _Devs.Type & ")")
                                _Devs = Nothing
                            End If
                            Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", " -> " & _ListDevices.Count & " devices(s) trouvé(s)")
                        Else
                            Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", " -> Aucun device enregistré dans le fichier de config")
                        End If
                        list = Nothing

                        '******************************************
                        'on va chercher les triggers
                        '******************************************
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Chargement des triggers :")
                        list = Nothing
                        list = myxml.SelectNodes("/homidom/triggers/trigger")
                        If list.Count > 0 Then 'présence des triggers
                            For i As Integer = 0 To list.Count - 1
                                Dim x As New Trigger
                                x._Server = Me
                                For j1 As Integer = 0 To list.Item(i).Attributes.Count - 1
                                    Select Case list.Item(i).Attributes.Item(j1).Name
                                        Case "id"
                                            x.ID = list.Item(i).Attributes.Item(j1).Value
                                        Case "nom"
                                            x.Nom = list.Item(i).Attributes.Item(j1).Value
                                        Case "enable"
                                            x.Enable = list.Item(i).Attributes.Item(j1).Value
                                        Case "type"
                                            If list.Item(i).Attributes.Item(j1).Value = "0" Then
                                                x.Type = Trigger.TypeTrigger.TIMER
                                            Else
                                                x.Type = Trigger.TypeTrigger.DEVICE
                                            End If
                                        Case "description"
                                            If list.Item(i).Attributes.Item(j1).Value <> Nothing Then x.Description = list.Item(i).Attributes.Item(j1).Value
                                        Case "conditiontime"
                                            If list.Item(i).Attributes.Item(j1).Value <> Nothing Then x.ConditionTime = list.Item(i).Attributes.Item(j1).Value
                                        Case "conditiondeviceid"
                                            If list.Item(i).Attributes.Item(j1).Value <> Nothing Then x.ConditionDeviceId = list.Item(i).Attributes.Item(j1).Value
                                        Case "conditiondeviceproperty"
                                            If list.Item(i).Attributes.Item(j1).Value <> Nothing Then x.ConditionDeviceProperty = list.Item(i).Attributes.Item(j1).Value
                                        Case "prochainedateheure"
                                            If list.Item(i).Attributes.Item(j1).Value <> Nothing Then x.Prochainedateheure = list.Item(i).Attributes.Item(j1).Value
                                        Case Else
                                            Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", " -> Un attribut correspondant au trigger est inconnu: nom:" & list.Item(i).Attributes.Item(j1).Name & " Valeur: " & list.Item(0).Attributes.Item(j1).Value)
                                    End Select
                                Next
                                If list.Item(i).HasChildNodes = True Then
                                    If list.Item(i).ChildNodes.Item(0).Name = "macros" And list.Item(i).ChildNodes.Item(0).HasChildNodes Then
                                        For k9 As Integer = 0 To list.Item(i).ChildNodes.Item(0).ChildNodes.Count - 1
                                            If list.Item(i).ChildNodes.Item(0).ChildNodes.Item(k9).Name = "macro" Then
                                                If list.Item(i).ChildNodes.Item(0).ChildNodes.Item(k9).Attributes.Count > 0 And list.Item(i).ChildNodes.Item(0).ChildNodes.Item(k9).Attributes.Item(0).Name = "id" Then
                                                    x.ListMacro.Add(list.Item(i).ChildNodes.Item(0).ChildNodes.Item(k9).Attributes.Item(0).Value)
                                                End If
                                            End If
                                        Next
                                    End If
                                End If
                                _listTriggers.Add(x)
                            Next
                        Else
                            Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", " -> Aucun trigger enregistré dans le fichier de config")
                        End If
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", " -> " & _ListTriggers.Count & " Trigger(s) chargé(s)")
                        list = Nothing

                        '******************************************
                        'on va chercher les macros
                        '******************************************
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Chargement des macros :")
                        list = Nothing
                        list = myxml.SelectNodes("/homidom/macros/macro")
                        If list.Count > 0 Then 'présence des macros
                            For i As Integer = 0 To list.Count - 1
                                Dim x As New Macro
                                For j1 As Integer = 0 To list.Item(i).Attributes.Count - 1
                                    Select Case list.Item(i).Attributes.Item(j1).Name
                                        Case "id"
                                            x.ID = list.Item(i).Attributes.Item(j1).Value
                                        Case "nom"
                                            x.Nom = list.Item(i).Attributes.Item(j1).Value
                                        Case "enable"
                                            x.Enable = list.Item(i).Attributes.Item(j1).Value
                                        Case "description"
                                            If list.Item(i).Attributes.Item(j1).Value <> Nothing Then x.Description = list.Item(0).Attributes.Item(j1).Value
                                        Case Else
                                            Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", " -> Un attribut correspondant à la macro est inconnu: nom:" & list.Item(i).Attributes.Item(j1).Name & " Valeur: " & list.Item(0).Attributes.Item(j1).Value)
                                    End Select
                                Next
                                LoadAction(list.Item(i), x.ListActions)
                                _ListMacros.Add(x)
                            Next
                        Else
                            Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", " -> Aucune macro enregistrée dans le fichier de config")
                        End If
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", " -> " & _ListMacros.Count & " Macro(s) chargée(s)")
                        list = Nothing

                        '******************************************
                        'on va chercher des extensions audios
                        '******************************************
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Chargement des extensions audio :")
                        list = Nothing
                        list = myxml.SelectNodes("/homidom/audios/extension")
                        If list.Count > 0 Then 'présence des extension
                            For i As Integer = 0 To list.Count - 1
                                Dim x As New Audio.ExtensionAudio
                                For j As Integer = 0 To list.Item(i).Attributes.Count - 1
                                    Select Case list.Item(i).Attributes.Item(j).Name
                                        Case "extension"
                                            x.Extension = list.Item(i).Attributes.Item(j).Value
                                        Case "enable"
                                            x.Enable = list.Item(i).Attributes.Item(j).Value
                                        Case Else
                                            Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", " -> Un attribut correspondant à une extension audio est inconnu: nom:" & list.Item(i).Attributes.Item(j).Name & " Valeur: " & list.Item(0).Attributes.Item(j).Value)
                                    End Select
                                Next
                                _ListExtensionAudio.Add(x)
                            Next
                        End If
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", " -> " & _ListExtensionAudio.Count & " Extension(s) Audio chargée(s)")
                        list = Nothing

                        '******************************************
                        'on va chercher les répertoires audios
                        '******************************************
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Chargement des répertoires audio :")
                        list = Nothing
                        list = myxml.SelectNodes("/homidom/audios/repertoire")
                        If list.Count > 0 Then 'présence des répertoires
                            For i As Integer = 0 To list.Count - 1
                                Dim x As New Audio.RepertoireAudio
                                For j As Integer = 0 To list.Item(i).Attributes.Count - 1
                                    Select Case list.Item(i).Attributes.Item(j).Name
                                        Case "repertoire"
                                            x.Repertoire = list.Item(i).Attributes.Item(j).Value
                                        Case "enable"
                                            x.Enable = list.Item(i).Attributes.Item(j).Value
                                        Case Else
                                            Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", " -> Un attribut correspondant à un répertoire audio est inconnu: nom:" & list.Item(i).Attributes.Item(j).Name & " Valeur: " & list.Item(0).Attributes.Item(j).Value)
                                    End Select
                                Next
                                _ListRepertoireAudio.Add(x)
                            Next
                        End If
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", " -> " & _ListRepertoireAudio.Count & " Répertoire(s) Audio chargé(s)")
                        list = Nothing

                        '******************************************
                        'on va nettoyer les devices disparus
                        '********************************************
                        'For Each _zone In _ListZones
                        '    For Each _elmt In _zone.ListElement
                        '        If ReturnDeviceById(_IdSrv, _elmt.ElementID) Is Nothing And ReturnZoneById(_IdSrv, _elmt.ElementID) Is Nothing And ReturnMacroById(_IdSrv, _elmt.ElementID) Is Nothing Then
                        '            _zone.ListElement.Remove(_elmt)
                        '        End If
                        '    Next
                        'Next

                        Exit For
                    Next
                Else
                    Log(TypeLog.ERREUR, TypeSource.SERVEUR, "LoadConfig", "Fichier de configuration non trouvé")
                End If

                _Finish = True

                'Vide les variables
                dirInfo = Nothing
                file = Nothing
                files = Nothing
                myxml = Nothing

                Return " Chargement de la configuration terminée"

            Catch ex As Exception
                Return " Erreur de chargement de la config: " & ex.ToString
            End Try
        End Function

        Private Sub LoadAction(ByVal list As XmlNode, ByVal ListAction As ArrayList)
            Try
                If list.HasChildNodes Then
                    For j2 As Integer = 0 To list.ChildNodes.Count - 1
                        If list.ChildNodes.Item(j2).Name = "action" Then
                            Dim _Act As Object = Nothing
                            Select Case list.ChildNodes.Item(j2).Attributes.Item(0).Value
                                Case "ActionDevice"
                                    Dim o As New Action.ActionDevice
                                    _Act = o
                                    o = Nothing
                                Case "ActionMail"
                                    Dim o As New Action.ActionMail
                                    _Act = o
                                    o = Nothing
                                Case "ActionIf"
                                    Dim o As New Action.ActionIf
                                    _Act = o
                                    o = Nothing
                                Case "ActionMacro"
                                    Dim o As New Action.ActionMacro
                                    _Act = o
                                    o = Nothing
                            End Select
                            For j3 As Integer = 0 To list.ChildNodes.Item(j2).Attributes.Count - 1
                                Select Case list.ChildNodes.Item(j2).Attributes.Item(j3).Name
                                    Case "timing"
                                        _Act.timing = CDate(list.ChildNodes.Item(j2).Attributes.Item(j3).Value)
                                    Case "iddevice"
                                        _Act.iddevice = list.ChildNodes.Item(j2).Attributes.Item(j3).Value
                                    Case "idmacro"
                                        _Act.idmacro = list.ChildNodes.Item(j2).Attributes.Item(j3).Value
                                    Case "method"
                                        _Act.method = list.ChildNodes.Item(j2).Attributes.Item(j3).Value
                                    Case "userid"
                                        _Act.userid = list.ChildNodes.Item(j2).Attributes.Item(j3).Value
                                    Case "sujet"
                                        _Act.sujet = list.ChildNodes.Item(j2).Attributes.Item(j3).Value
                                    Case "message"
                                        _Act.message = list.ChildNodes.Item(j2).Attributes.Item(j3).Value
                                    Case "parametres"
                                        Dim b As String = list.ChildNodes.Item(j2).Attributes.Item(j3).Value
                                        Dim a() As String = b.Split("|")
                                        Dim c As New ArrayList
                                        For cnt1 As Integer = 0 To a.Count - 1
                                            c.Add(a(cnt1))
                                        Next
                                        _Act.parametres = c
                                        b = Nothing
                                        a = Nothing
                                        c = Nothing
                                End Select
                            Next
                            If list.ChildNodes.Item(j2).HasChildNodes Then
                                For j3 As Integer = 0 To list.ChildNodes.Item(j2).ChildNodes.Count - 1
                                    If list.ChildNodes.Item(j2).ChildNodes.Item(j3).Name = "conditions" Then
                                        For j4 As Integer = 0 To list.ChildNodes.Item(j2).ChildNodes.Item(j3).ChildNodes.Count - 1
                                            Dim Condi As New Action.Condition
                                            For j5 As Integer = 0 To list.ChildNodes.Item(j2).ChildNodes.Item(j3).ChildNodes.Item(j4).Attributes.Count - 1
                                                Select Case list.ChildNodes.Item(j2).ChildNodes.Item(j3).ChildNodes.Item(j4).Attributes.Item(j5).Name
                                                    Case "typecondition"
                                                        Select Case list.ChildNodes.Item(j2).ChildNodes.Item(j3).ChildNodes.Item(j4).Attributes.Item(j5).Value
                                                            Case Action.TypeCondition.DateTime.ToString
                                                                Condi.Type = Action.TypeCondition.DateTime
                                                            Case Action.TypeCondition.Device.ToString
                                                                Condi.Type = Action.TypeCondition.Device
                                                        End Select
                                                    Case "datetime"
                                                        Condi.DateTime = list.ChildNodes.Item(j2).ChildNodes.Item(j3).ChildNodes.Item(j4).Attributes.Item(j5).Value
                                                    Case "iddevice"
                                                        Condi.IdDevice = list.ChildNodes.Item(j2).ChildNodes.Item(j3).ChildNodes.Item(j4).Attributes.Item(j5).Value
                                                    Case "propertydevice"
                                                        Condi.PropertyDevice = list.ChildNodes.Item(j2).ChildNodes.Item(j3).ChildNodes.Item(j4).Attributes.Item(j5).Value
                                                    Case "value"
                                                        Condi.Value = list.ChildNodes.Item(j2).ChildNodes.Item(j3).ChildNodes.Item(j4).Attributes.Item(j5).Value
                                                    Case "condition"
                                                        Select Case list.ChildNodes.Item(j2).ChildNodes.Item(j3).ChildNodes.Item(j4).Attributes.Item(j5).Value
                                                            Case Action.TypeSigne.Different.ToString
                                                                Condi.Condition = Action.TypeSigne.Different
                                                            Case Action.TypeSigne.Egal.ToString
                                                                Condi.Condition = Action.TypeSigne.Egal
                                                            Case Action.TypeSigne.Inferieur.ToString
                                                                Condi.Condition = Action.TypeSigne.Inferieur
                                                            Case Action.TypeSigne.InferieurEgal.ToString
                                                                Condi.Condition = Action.TypeSigne.InferieurEgal
                                                            Case Action.TypeSigne.Superieur.ToString
                                                                Condi.Condition = Action.TypeSigne.Superieur
                                                            Case Action.TypeSigne.SuperieurEgal.ToString
                                                                Condi.Condition = Action.TypeSigne.SuperieurEgal
                                                        End Select
                                                    Case "operateur"
                                                        Select Case list.ChildNodes.Item(j2).ChildNodes.Item(j3).ChildNodes.Item(j4).Attributes.Item(j5).Value
                                                            Case Action.TypeOperateur.NONE.ToString
                                                                Condi.Operateur = Action.TypeOperateur.NONE
                                                            Case Action.TypeOperateur.AND.ToString
                                                                Condi.Operateur = Action.TypeOperateur.AND
                                                            Case Action.TypeOperateur.OR.ToString
                                                                Condi.Operateur = Action.TypeOperateur.OR
                                                        End Select
                                                End Select
                                            Next
                                            _Act.Conditions.add(Condi)
                                        Next
                                    End If
                                    If list.ChildNodes.Item(j2).ChildNodes.Item(j3).Name = "then" Then
                                        LoadAction(list.ChildNodes.Item(j2).ChildNodes.Item(j3), _Act.ListTrue)
                                    End If
                                    If list.ChildNodes.Item(j2).ChildNodes.Item(j3).Name = "else" Then
                                        LoadAction(list.ChildNodes.Item(j2).ChildNodes.Item(j3), _Act.ListFalse)
                                    End If
                                Next
                            End If
                            ListAction.Add(_Act)
                        End If
                    Next
                End If
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "LoadAction", ex.ToString)
            End Try
        End Sub

        ''' <summary>Sauvegarde de la config dans le fichier XML</summary>
        ''' <remarks></remarks>
        Private Function SaveConfig(ByVal Fichier As String) As Boolean
            Try
                Log(TypeLog.INFO, TypeSource.SERVEUR, "SaveConfig", "Sauvegarde de la config sous le fichier " & Fichier)

                ''Copy du fichier de config avant sauvegarde
                Try
                    Dim _file As String = Fichier.Replace(".xml", "")
                    If IO.File.Exists(_file & ".sav") = True Then IO.File.Delete(_file & ".sav")
                    IO.File.Copy(_file & ".xml", _file & ".sav")
                    Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Création de sauvegarde (.sav) du fichier de config avant sauvegarde")
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
                writer.WriteStartAttribute("ipsoap")
                writer.WriteValue(_IPSOAP)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("portsoap")
                writer.WriteValue(_PortSOAP)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("idsrv")
                writer.WriteValue(_IdSrv)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("longitude")
                writer.WriteValue(_Longitude)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("latitude")
                writer.WriteValue(_Latitude)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("heurecorrectionlever")
                writer.WriteValue(_HeureLeverSoleilCorrection)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("heurecorrectioncoucher")
                writer.WriteValue(_HeureCoucherSoleilCorrection)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("smtpserver")
                writer.WriteValue(_SMTPServeur)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("smtpmail")
                writer.WriteValue(_SMTPmailEmetteur)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("smtplogin")
                writer.WriteValue(_SMTPLogin)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("smtppassword")
                writer.WriteValue(_SMTPassword)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("log0")
                writer.WriteValue(_TypeLogEnable(0))
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("log1")
                writer.WriteValue(_TypeLogEnable(1))
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("log2")
                writer.WriteValue(_TypeLogEnable(2))
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("log3")
                writer.WriteValue(_TypeLogEnable(3))
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("log4")
                writer.WriteValue(_TypeLogEnable(4))
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("log5")
                writer.WriteValue(_TypeLogEnable(5))
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("log6")
                writer.WriteValue(_TypeLogEnable(6))
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("log7")
                writer.WriteValue(_TypeLogEnable(7))
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("log8")
                writer.WriteValue(_TypeLogEnable(8))
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("log9")
                writer.WriteValue(_TypeLogEnable(9))
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("cyclesave")
                writer.WriteValue(_CycleSave)
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
                    If _ListDrivers.Item(i).Picture IsNot Nothing Then
                        writer.WriteValue(_ListDrivers.Item(i).Picture)
                    Else
                        writer.WriteValue(" ")
                    End If
                    writer.WriteEndAttribute()
                    If _ListDrivers.Item(i).Parametres IsNot Nothing Then
                        For j As Integer = 0 To _ListDrivers.Item(i).Parametres.count - 1
                            writer.WriteStartAttribute("parametre" & j)
                            If _ListDrivers.Item(i).Parametres.Item(j).valeur IsNot Nothing Then
                                writer.WriteValue(_ListDrivers.Item(i).Parametres.Item(j).valeur)
                            Else
                                writer.WriteValue(" ")
                            End If
                            writer.WriteEndAttribute()
                        Next
                    End If
                    writer.WriteEndElement()
                Next
                writer.WriteEndElement()

                ''------------
                ''Sauvegarde des zones
                ''------------
                Log(TypeLog.INFO, TypeSource.SERVEUR, "SaveConfig", "Sauvegarde des zones")
                writer.WriteStartElement("zones")
                For i As Integer = 0 To _ListZones.Count - 1
                    writer.WriteStartElement("zone")
                    writer.WriteStartAttribute("id")
                    writer.WriteValue(_ListZones.Item(i).ID)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("name")
                    writer.WriteValue(_ListZones.Item(i).Name)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("icon")
                    writer.WriteValue(_ListZones.Item(i).Icon)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("image")
                    writer.WriteValue(_ListZones.Item(i).Image)
                    writer.WriteEndAttribute()
                    If _ListZones.Item(i).ListElement IsNot Nothing Then
                        For j As Integer = 0 To _ListZones.Item(i).ListElement.Count - 1
                            writer.WriteStartElement("element")
                            writer.WriteStartAttribute("elementid")
                            writer.WriteValue(_ListZones.Item(i).ListElement.Item(j).ElementID)
                            writer.WriteEndAttribute()
                            writer.WriteStartAttribute("visible")
                            writer.WriteValue(_ListZones.Item(i).ListElement.Item(j).Visible)
                            writer.WriteEndAttribute()
                            writer.WriteEndElement()
                        Next
                    End If
                    writer.WriteEndElement()
                Next
                writer.WriteEndElement()

                ''------------
                ''Sauvegarde des users
                ''------------
                Log(TypeLog.INFO, TypeSource.SERVEUR, "SaveConfig", "Sauvegarde des users")
                writer.WriteStartElement("users")
                For i As Integer = 0 To _ListUsers.Count - 1
                    writer.WriteStartElement("user")
                    writer.WriteStartAttribute("id")
                    writer.WriteValue(_ListUsers.Item(i).ID)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("username")
                    writer.WriteValue(_ListUsers.Item(i).UserName)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("nom")
                    writer.WriteValue(_ListUsers.Item(i).Nom)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("prenom")
                    writer.WriteValue(_ListUsers.Item(i).Prenom)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("profil")
                    Select Case _ListUsers.Item(i).Profil
                        Case Users.TypeProfil.invite
                            writer.WriteValue("0")
                        Case Users.TypeProfil.user
                            writer.WriteValue("1")
                        Case Users.TypeProfil.admin
                            writer.WriteValue("2")
                    End Select
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("password")
                    writer.WriteValue(_ListUsers.Item(i).Password)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("numberidentification")
                    writer.WriteValue(_ListUsers.Item(i).NumberIdentification)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("image")
                    writer.WriteValue(_ListUsers.Item(i).Image)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("email")
                    writer.WriteValue(_ListUsers.Item(i).eMail)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("emailautre")
                    writer.WriteValue(_ListUsers.Item(i).eMailAutre)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("telfixe")
                    writer.WriteValue(_ListUsers.Item(i).TelFixe)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("telmobile")
                    writer.WriteValue(_ListUsers.Item(i).TelMobile)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("telautre")
                    writer.WriteValue(_ListUsers.Item(i).TelAutre)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("adresse")
                    writer.WriteValue(_ListUsers.Item(i).Adresse)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("ville")
                    writer.WriteValue(_ListUsers.Item(i).Ville)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("codepostal")
                    writer.WriteValue(_ListUsers.Item(i).CodePostal)
                    writer.WriteEndAttribute()
                    writer.WriteEndElement()
                Next
                writer.WriteEndElement()

                ''------------
                ''Sauvegarde des extensions audios
                ''------------
                Log(TypeLog.INFO, TypeSource.SERVEUR, "SaveConfig", "Sauvegarde des extensions et répertoires audio")
                writer.WriteStartElement("audios")
                For i As Integer = 0 To _ListExtensionAudio.Count - 1
                    writer.WriteStartElement("extension")
                    writer.WriteStartAttribute("extension")
                    writer.WriteValue(_ListExtensionAudio.Item(i).Extension)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("enable")
                    writer.WriteValue(_ListExtensionAudio.Item(i).Enable)
                    writer.WriteEndAttribute()
                    writer.WriteEndElement()
                Next
                For i As Integer = 0 To _ListRepertoireAudio.Count - 1
                    writer.WriteStartElement("repertoire")
                    writer.WriteStartAttribute("repertoire")
                    writer.WriteValue(_ListRepertoireAudio.Item(i).Repertoire)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("enable")
                    writer.WriteValue(_ListRepertoireAudio.Item(i).Enable)
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
                    'Log(TypeLog.DEBUG, TypeSource.SERVEUR, "SaveConfig", " - " & _ListDevices.Item(i).name)
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
                    Dim _pict As String = _ListDevices.Item(i).picture
                    If _pict = "" Or _pict = Nothing Then _pict = " "
                    writer.WriteValue(_pict)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("solo")
                    writer.WriteValue(_ListDevices.Item(i).solo)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("value")
                    If _ListDevices.Item(i).value IsNot Nothing Then writer.WriteValue(_ListDevices.Item(i).value)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("lastetat")
                    writer.WriteValue(_ListDevices.Item(i).lastetat)
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
                        writer.WriteStartAttribute("valuelast")
                        writer.WriteValue(_ListDevices.Item(i).valuelast)
                        writer.WriteEndAttribute()
                    End If

                    '-- Cas Code IR a ajouter pour MULTIMEDIA
                    'If _ListDevices.Item(i).Type = "MULTIMEDIA" Then
                    '    writer.WriteStartElement("commands")
                    '    For k As Integer = 0 To _ListDevices.Item(i).ListCommandName.Count - 1
                    '        writer.WriteStartElement("command")
                    '        writer.WriteStartAttribute("key")
                    '        writer.WriteValue(_ListDevices.Item(i).ListCommandName(k))
                    '        writer.WriteEndAttribute()
                    '        writer.WriteStartAttribute("data")
                    '        writer.WriteValue(_ListDevices.Item(i).ListCommandData(k))
                    '        writer.WriteEndAttribute()
                    '        writer.WriteStartAttribute("repeat")
                    '        writer.WriteValue(_ListDevices.Item(i).ListCommandRepeat(k))
                    '        writer.WriteEndAttribute()
                    '        writer.WriteEndElement()
                    '    Next
                    '    writer.WriteEndElement()
                    'End If
                    writer.WriteEndElement()
                Next
                writer.WriteEndElement()

                ''------------
                ''Sauvegarde des triggers
                ''------------
                Log(TypeLog.INFO, TypeSource.SERVEUR, "SaveConfig", "Sauvegarde des triggers")
                writer.WriteStartElement("triggers")
                For i As Integer = 0 To _ListTriggers.Count - 1
                    writer.WriteStartElement("trigger")
                    writer.WriteStartAttribute("id")
                    writer.WriteValue(_ListTriggers.Item(i).ID)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("nom")
                    writer.WriteValue(_ListTriggers.Item(i).Nom)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("description")
                    writer.WriteValue(_ListTriggers.Item(i).Description)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("enable")
                    writer.WriteValue(_ListTriggers.Item(i).Enable)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("type")
                    If _ListTriggers.Item(i).Type = Trigger.TypeTrigger.TIMER Then
                        writer.WriteValue("0")
                    Else
                        writer.WriteValue("1")
                    End If
                    writer.WriteEndAttribute()
                    If _ListTriggers.Item(i).Type = Trigger.TypeTrigger.TIMER Then
                        writer.WriteStartAttribute("conditiontime")
                        writer.WriteValue(_ListTriggers.Item(i).ConditionTime)
                        writer.WriteEndAttribute()
                        writer.WriteStartAttribute("prochainedateheure")
                        writer.WriteValue(_ListTriggers.Item(i).Prochainedateheure)
                        writer.WriteEndAttribute()
                    End If
                    If _ListTriggers.Item(i).Type = Trigger.TypeTrigger.DEVICE Then
                        writer.WriteStartAttribute("conditiondeviceid")
                        writer.WriteValue(_ListTriggers.Item(i).ConditionDeviceId)
                        writer.WriteEndAttribute()
                        writer.WriteStartAttribute("conditiondeviceproperty")
                        writer.WriteValue(_ListTriggers.Item(i).ConditionDeviceProperty)
                        writer.WriteEndAttribute()
                    End If
                    writer.WriteStartElement("macros")
                    For k = 0 To _ListTriggers.Item(i).ListMacro.Count - 1
                        writer.WriteStartElement("macro")
                        writer.WriteStartAttribute("id")
                        writer.WriteValue(_ListTriggers.Item(i).ListMacro.Item(k))
                        writer.WriteEndAttribute()
                        writer.WriteEndElement()
                    Next
                    writer.WriteEndElement()
                    writer.WriteEndElement()
                Next
                writer.WriteEndElement()

                ''------------
                ''Sauvegarde des macros
                ''------------
                Log(TypeLog.INFO, TypeSource.SERVEUR, "SaveConfig", "Sauvegarde des macros")
                writer.WriteStartElement("macros")
                For i As Integer = 0 To _ListMacros.Count - 1
                    writer.WriteStartElement("macro")
                    writer.WriteStartAttribute("id")
                    writer.WriteValue(_ListMacros.Item(i).ID)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("nom")
                    writer.WriteValue(_ListMacros.Item(i).Nom)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("description")
                    writer.WriteValue(_ListMacros.Item(i).Description)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("enable")
                    writer.WriteValue(_ListMacros.Item(i).Enable)
                    writer.WriteEndAttribute()
                    WriteListAction(writer, _ListMacros.Item(i).ListActions)
                    writer.WriteEndElement()
                Next
                writer.WriteEndElement()
                ''FIN DES ELEMENTS------------

                writer.WriteEndDocument()
                writer.Close()
                Log(TypeLog.INFO, TypeSource.SERVEUR, "SaveConfig", "Sauvegarde terminée")
                Return True
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SaveConfig", " Erreur de sauvegarde de la configuration: " & ex.Message)
                Return False
            End Try

        End Function

        ''' <summary>
        ''' Ecris les actions dans le fichier de config
        ''' </summary>
        ''' <param name="writer"></param>
        ''' <param name="ListActions"></param>
        ''' <remarks></remarks>
        Private Sub WriteListAction(ByVal writer As XmlTextWriter, ByVal ListActions As ArrayList)
            Try
                For j As Integer = 0 To ListActions.Count - 1
                    writer.WriteStartElement("action")
                    writer.WriteStartAttribute("typeaction")
                    writer.WriteValue(ListActions.Item(j).TypeAction.ToString)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("timing")
                    writer.WriteValue(ListActions.Item(j).timing)
                    writer.WriteEndAttribute()
                    Select Case ListActions.Item(j).TypeAction
                        Case Action.TypeAction.ActionDevice
                            writer.WriteStartAttribute("iddevice")
                            writer.WriteValue(ListActions.Item(j).IdDevice)
                            writer.WriteEndAttribute()
                            writer.WriteStartAttribute("method")
                            writer.WriteValue(ListActions.Item(j).Method)
                            writer.WriteEndAttribute()
                            Dim a As String = ""
                            For k As Integer = 0 To ListActions.Item(j).parametres.count - 1
                                a = a & ListActions.Item(j).parametres.item(k) & "|"
                            Next
                            writer.WriteStartAttribute("parametres")
                            writer.WriteValue(a)
                            writer.WriteEndAttribute()
                        Case Action.TypeAction.ActionMacro
                            writer.WriteStartAttribute("idmacro")
                            writer.WriteValue(ListActions.Item(j).IdMacro)
                            writer.WriteEndAttribute()
                        Case Action.TypeAction.ActionMail
                            writer.WriteStartAttribute("userid")
                            writer.WriteValue(ListActions.Item(j).UserId)
                            writer.WriteEndAttribute()
                            writer.WriteStartAttribute("sujet")
                            writer.WriteValue(ListActions.Item(j).Sujet)
                            writer.WriteEndAttribute()
                            writer.WriteStartAttribute("message")
                            writer.WriteValue(ListActions.Item(j).Sujet)
                            writer.WriteEndAttribute()
                        Case Action.TypeAction.ActionIf
                            writer.WriteStartElement("conditions")
                            For i2 As Integer = 0 To ListActions.Item(j).Conditions.count - 1
                                writer.WriteStartElement("condition")
                                writer.WriteStartAttribute("typecondition")
                                writer.WriteValue(ListActions.Item(j).Conditions.item(i2).Type.ToString)
                                writer.WriteEndAttribute()
                                If ListActions.Item(j).conditions.item(i2).Type = Action.TypeCondition.DateTime Then
                                    writer.WriteStartAttribute("datetime")
                                    writer.WriteValue(ListActions.Item(j).Conditions.item(i2).DateTime)
                                    writer.WriteEndAttribute()
                                End If
                                If ListActions.Item(j).conditions.item(i2).Type = Action.TypeCondition.Device Then
                                    writer.WriteStartAttribute("iddevice")
                                    writer.WriteValue(ListActions.Item(j).Conditions.item(i2).IdDevice)
                                    writer.WriteEndAttribute()
                                    writer.WriteStartAttribute("propertydevice")
                                    writer.WriteValue(ListActions.Item(j).Conditions.item(i2).propertydevice)
                                    writer.WriteEndAttribute()
                                    writer.WriteStartAttribute("value")
                                    writer.WriteValue(ListActions.Item(j).Conditions.item(i2).Value.ToString)
                                    writer.WriteEndAttribute()
                                End If
                                writer.WriteStartAttribute("condition")
                                writer.WriteValue(ListActions.Item(j).Conditions.item(i2).Condition.ToString)
                                writer.WriteEndAttribute()
                                writer.WriteStartAttribute("operateur")
                                writer.WriteValue(ListActions.Item(j).Conditions.item(i2).Operateur.ToString)
                                writer.WriteEndAttribute()
                                writer.WriteEndElement()
                            Next
                            writer.WriteEndElement()
                            writer.WriteStartElement("then")
                            If ListActions.Item(j).ListTrue IsNot Nothing Then WriteListAction(writer, ListActions.Item(j).ListTrue)
                            writer.WriteEndElement()
                            writer.WriteStartElement("else")
                            If ListActions.Item(j).ListFalse IsNot Nothing Then WriteListAction(writer, ListActions.Item(j).ListFalse)
                            writer.WriteEndElement()
                    End Select
                    writer.WriteEndElement()
                Next
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "WriteListAction", "Exception : " & ex.Message)
            End Try
        End Sub
#End Region

#Region "Device"
        ''' <summary>Arretes les devices (Handlers)</summary>
        ''' <remarks></remarks>
        Public Sub Devices_Stop()
            Try
                'Cherche tous les devices chargés
                Log(TypeLog.INFO, TypeSource.SERVEUR, "Devices_Stop", "Arrêt des devices :")
                For Each _dev As Device.DeviceGenerique In _ListDevices
                    Log(TypeLog.INFO, TypeSource.SERVEUR, "Devices_Stop", " - " & _dev.Name & " arrété")
                    'marche pas !!!!!
                    'Suivant chaque type de device
                    Select Case _dev.Type
                        Case "APPAREIL"
                            Dim o As Device.APPAREIL
                            o = _dev
                            RemoveHandler o.DeviceChanged, AddressOf DeviceChange
                            o = Nothing
                        Case "AUDIO"
                            Dim o As Device.AUDIO
                            o = _dev
                            RemoveHandler o.DeviceChanged, AddressOf DeviceChange
                            o = Nothing
                        Case "BAROMETRE"
                            Dim o As Device.BAROMETRE
                            o = _dev
                            RemoveHandler o.DeviceChanged, AddressOf DeviceChange
                            o = Nothing
                        Case "BATTERIE"
                            Dim o As Device.BATTERIE
                            o = _dev
                            RemoveHandler o.DeviceChanged, AddressOf DeviceChange
                            o = Nothing
                        Case "COMPTEUR"
                            Dim o As Device.COMPTEUR
                            o = _dev
                            RemoveHandler o.DeviceChanged, AddressOf DeviceChange
                            o = Nothing
                        Case "CONTACT"
                            Dim o As Device.CONTACT
                            o = _dev
                            RemoveHandler o.DeviceChanged, AddressOf DeviceChange
                            o = Nothing
                        Case "DETECTEUR"
                            Dim o As Device.DETECTEUR
                            o = _dev
                            RemoveHandler o.DeviceChanged, AddressOf DeviceChange
                            o = Nothing
                        Case "DIRECTIONVENT"
                            Dim o As Device.DIRECTIONVENT
                            o = _dev
                            RemoveHandler o.DeviceChanged, AddressOf DeviceChange
                            o = Nothing
                        Case "ENERGIEINSTANTANEE"
                            Dim o As Device.ENERGIEINSTANTANEE
                            o = _dev
                            RemoveHandler o.DeviceChanged, AddressOf DeviceChange
                            o = Nothing
                        Case "ENERGIETOTALE"
                            Dim o As Device.ENERGIETOTALE
                            o = _dev
                            RemoveHandler o.DeviceChanged, AddressOf DeviceChange
                            o = Nothing
                        Case "FREEBOX"
                            Dim o As Device.FREEBOX
                            o = _dev
                            RemoveHandler o.DeviceChanged, AddressOf DeviceChange
                            o = Nothing
                        Case "GENERIQUEBOOLEEN"
                            Dim o As Device.GENERIQUEBOOLEEN
                            o = _dev
                            RemoveHandler o.DeviceChanged, AddressOf DeviceChange
                            o = Nothing
                        Case "GENERIQUESTRING"
                            Dim o As Device.GENERIQUESTRING
                            o = _dev
                            RemoveHandler o.DeviceChanged, AddressOf DeviceChange
                            o = Nothing
                            o = _dev
                            RemoveHandler o.DeviceChanged, AddressOf DeviceChange
                            o = Nothing
                        Case "GENERIQUEVALUE"
                            Dim o As Device.GENERIQUEVALUE
                            o = _dev
                            RemoveHandler o.DeviceChanged, AddressOf DeviceChange
                            o = Nothing
                        Case "HUMIDITE"
                            Dim o As Device.HUMIDITE
                            o = _dev
                            RemoveHandler o.DeviceChanged, AddressOf DeviceChange
                            o = Nothing
                        Case "LAMPE"
                            Dim o As Device.LAMPE
                            o = _dev
                            RemoveHandler o.DeviceChanged, AddressOf DeviceChange
                            o = Nothing
                        Case "METEO"
                            Dim o As Device.METEO
                            o = _dev
                            RemoveHandler o.DeviceChanged, AddressOf DeviceChange
                            o = Nothing
                        Case "MULTIMEDIA"
                            Dim o As Device.MULTIMEDIA
                            o = _dev
                            RemoveHandler o.DeviceChanged, AddressOf DeviceChange
                            o = Nothing
                        Case "PLUIECOURANT"
                            Dim o As Device.PLUIECOURANT
                            o = _dev
                            RemoveHandler o.DeviceChanged, AddressOf DeviceChange
                            o = Nothing
                        Case "PLUIETOTAL"
                            Dim o As Device.PLUIETOTAL
                            o = _dev
                            RemoveHandler o.DeviceChanged, AddressOf DeviceChange
                            o = Nothing
                        Case "SWITCH"
                            Dim o As Device.SWITCH
                            o = _dev
                            RemoveHandler o.DeviceChanged, AddressOf DeviceChange
                            o = Nothing
                        Case "TELECOMMANDE"
                            Dim o As Device.TELECOMMANDE
                            o = _dev
                            RemoveHandler o.DeviceChanged, AddressOf DeviceChange
                            o = Nothing
                        Case "TEMPERATURE"
                            Dim o As Device.TEMPERATURE
                            o = _dev
                            RemoveHandler o.DeviceChanged, AddressOf DeviceChange
                            o = Nothing
                        Case "TEMPERATURECONSIGNE"
                            Dim o As Device.TEMPERATURECONSIGNE
                            o = _dev
                            RemoveHandler o.DeviceChanged, AddressOf DeviceChange
                            o = Nothing
                        Case "UV"
                            Dim o As Device.UV
                            o = _dev
                            RemoveHandler o.DeviceChanged, AddressOf DeviceChange
                            o = Nothing
                        Case "VITESSEVENT"
                            Dim o As Device.VITESSEVENT
                            o = _dev
                            RemoveHandler o.DeviceChanged, AddressOf DeviceChange
                            o = Nothing
                        Case "VOLET"
                            Dim o As Device.VOLET
                            o = _dev
                            RemoveHandler o.DeviceChanged, AddressOf DeviceChange
                            o = Nothing
                        Case Else
                            Dim o As Device.GENERIQUEVALUE
                            o = _dev
                            RemoveHandler o.DeviceChanged, AddressOf DeviceChange
                            o = Nothing
                    End Select

                Next
            Catch ex As Exception
                MsgBox("Erreur lors de l arret des drivers: " & ex.Message, MsgBoxStyle.Exclamation, "Erreur Serveur")
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Devices_Stop", " -> Erreur lors de l'arret des devices: " & ex.Message)
            End Try
        End Sub

        ''' <summary>Liste les type de devices par leur valeur d'Enum</summary>
        ''' <param name="Index"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ReturnSringFromEnumDevice(ByVal Index As Integer) As String
            Try
                For Each value As Device.ListeDevices In [Enum].GetValues(GetType(Device.ListeDevices))
                    If value.GetHashCode = Index Then
                        Return value.ToString
                        Exit Function
                    End If
                Next
                Return ""
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "ReturnSringFromEnumDevice", "Exception : " & ex.Message)
                Return ""
            End Try
        End Function
#End Region

#Region "Driver"
        ''' <summary>Retourne les propriétés d'un driver</summary>
        ''' <remarks></remarks>
        Public Function ReturnDriver(ByVal DriverId As String) As ArrayList
            Try
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
                        tabl.Add(_ListDrivers.Item(i).Parametres)
                        tabl.Add(_ListDrivers.Item(i).Labels)
                        Return tabl
                        Exit For
                    End If
                Next
                Return Nothing
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "ReturnDriver", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function

        ''' <summary>Ecrire ou lance propritété/Sub d'un driver</summary>
        ''' <remarks></remarks>
        Sub WriteDriver(ByVal DriverId As String, ByVal Command As String, ByVal Parametre As Object)
            Try
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
                            Case "MODELE"
                                _ListDrivers.Item(i).Refresh = Parametre
                            Case "STARTAUTO"
                                _ListDrivers.Item(i).StartAuto = Parametre
                            Case "START"
                                _ListDrivers.Item(i).Start()
                            Case "STOP"
                                _ListDrivers.Item(i).Stop()
                            Case "RESTART"
                                _ListDrivers.Item(i).Restart()
                            Case "PARAMETRES"
                                For idx As Integer = 0 To Parametre.count - 1
                                    _ListDrivers.Item(i).Parametres.item(idx).valeur = Parametre(idx)
                                Next
                            Case "LABELS"
                                For idx As Integer = 0 To Parametre.count - 1
                                    _ListDrivers.Item(i).Labels.item(idx).tooltip = Parametre(idx)
                                Next
                            Case "DELETEDEVICE"
                                _ListDrivers.Item(i).DeleteDevice(Parametre)
                            Case "NEWDEVICE"
                                _ListDrivers.Item(i).NewDevice(Parametre)
                        End Select
                        Exit For

                    End If
                Next
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "WriteDriver", "Exception : " & ex.Message)
            End Try
        End Sub

        ''' <summary>Charge les drivers, donc toutes les dll dans le sous répertoire "drivers"</summary>
        ''' <remarks></remarks>
        Public Sub Drivers_Load()
            Try
                Dim tx As String = ""
                Dim dll As Reflection.Assembly
                Dim tp As Type
                Dim Chm As String = _MonRepertoire & "\Drivers\" 'Emplacement par défaut des drivers

                Dim strFileSize As String = ""
                Dim di As New IO.DirectoryInfo(Chm)
                Dim aryFi As IO.FileInfo() = di.GetFiles("Driver_*.dll")
                Dim fi As IO.FileInfo

                'Cherche tous les fichiers dll dans le répertoie plugin
                Log(TypeLog.INFO, TypeSource.SERVEUR, "Drivers_Load", "Chargement des DLL des drivers :")
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
                                Dim i1 As IDriver
                                i1 = DirectCast(dll.CreateInstance(tp.FullName), IDriver)
                                i1 = CType(i1, IDriver)
                                i1.Server = Me
                                i1.IdSrv = _IdSrv
                                Dim pt As New Driver(Me, _IdSrv, i1.ID)
                                _ListDrivers.Add(i1)
                                _ListImgDrivers.Add(pt)
                                Log(TypeLog.INFO, TypeSource.SERVEUR, "Drivers_Load", " - " & i1.Nom & " chargé")
                                i1 = Nothing
                                pt = Nothing
                            End If
                        End If
                    Next
                Next

                dll = Nothing
                Chm = Nothing
                di = Nothing
                aryFi = Nothing
                fi = Nothing
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Drivers_Load", " Erreur lors du chargement des drivers: " & ex.ToString)
            End Try
        End Sub

        ''' <summary>Démarre tous les drivers dont la propriété StartAuto=True</summary>
        ''' <remarks></remarks>
        Public Sub Drivers_Start()
            Try
                'Cherche tous les drivers chargés
                Log(TypeLog.INFO, TypeSource.SERVEUR, "Drivers_Start", "Démarrage des drivers :")
                For Each driver In _ListDrivers
                    If driver.Enable And driver.StartAuto Then
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "Drivers_Start", " - " & driver.Nom & " démarré")
                        driver.start()
                    Else
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "Drivers_Start", " - " & driver.Nom & " non démarré car non Auto")
                    End If
                Next
            Catch ex As Exception
                MsgBox("Erreur lors du démarrage des drivers: " & ex.Message, MsgBoxStyle.Exclamation, "Erreur Serveur")
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Drivers_Start", " -> Erreur lors du démarrage des drivers: " & ex.Message)
            End Try
        End Sub

        ''' <summary>Arretes les drivers démarrés</summary>
        ''' <remarks></remarks>
        Public Sub Drivers_Stop()
            Try
                'Cherche tous les drivers chargés
                Log(TypeLog.INFO, TypeSource.SERVEUR, "Drivers_Stop", "Arrêt des drivers :")
                For Each driver In _ListDrivers
                    If driver.Enable And driver.IsConnect Then
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "Drivers_Stop", " - " & driver.Nom & " : ")
                        driver.stop()
                    End If
                Next
            Catch ex As Exception
                MsgBox("Erreur lors de l arret des drivers: " & ex.Message, MsgBoxStyle.Exclamation, "Erreur Serveur")
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Drivers_Stop", " -> Erreur lors de l'arret des drivers: " & ex.Message)
            End Try
        End Sub

#End Region

#Region "Cryptage"
        ''' <summary>
        ''' Crypter un string
        ''' </summary>
        ''' <param name="sIn"></param>
        ''' <param name="sKey"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function EncryptTripleDES(ByVal sIn As String, ByVal sKey As String) As String
            Dim DES As New System.Security.Cryptography.TripleDESCryptoServiceProvider
            Dim hashMD5 As New System.Security.Cryptography.MD5CryptoServiceProvider

            Try
                ' scramble the key
                sKey = ScrambleKey(sKey)
                ' Compute the MD5 hash.
                DES.Key = hashMD5.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(sKey))
                ' Set the cipher mode.
                DES.Mode = System.Security.Cryptography.CipherMode.ECB
                ' Create the encryptor.
                Dim DESEncrypt As System.Security.Cryptography.ICryptoTransform = DES.CreateEncryptor()
                ' Get a byte array of the string.
                Dim Buffer As Byte() = System.Text.ASCIIEncoding.ASCII.GetBytes(sIn)
                ' Transform and return the string.
                Return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length))
            Catch ex As Exception
                'Log(TypeLog.ERREUR, TypeSource.SERVEUR, "EncryptTripleDES", "Exception : " & ex.Message)
                Return ""
            End Try
        End Function

        ''' <summary>Décrypter un string</summary>
        ''' <param name="sOut"></param>
        ''' <param name="sKey"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DecryptTripleDES(ByVal sOut As String, ByVal sKey As String) As String
            Dim DES As New System.Security.Cryptography.TripleDESCryptoServiceProvider()
            Dim hashMD5 As New System.Security.Cryptography.MD5CryptoServiceProvider

            Try
                ' scramble the key
                sKey = ScrambleKey(sKey)
                ' Compute the MD5 hash.
                DES.Key = hashMD5.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(sKey))
                ' Set the cipher mode.
                DES.Mode = System.Security.Cryptography.CipherMode.ECB
                ' Create the decryptor.
                Dim DESDecrypt As System.Security.Cryptography.ICryptoTransform = DES.CreateDecryptor()
                Dim Buffer As Byte() = Convert.FromBase64String(sOut)
                ' Transform and return the string.
                Return System.Text.ASCIIEncoding.ASCII.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length))
            Catch ex As Exception
                'Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DecryptTripleDES", "Exception : " & ex.Message)
                Return ""
            End Try
        End Function

        Private Shared Function ScrambleKey(ByVal v_strKey As String) As String
            Dim sbKey As New System.Text.StringBuilder
            Dim intPtr As Integer
            Try
                For intPtr = 1 To v_strKey.Length
                    Dim intIn As Integer = v_strKey.Length - intPtr + 1
                    sbKey.Append(Mid(v_strKey, intIn, 1))
                Next
                Dim strKey As String = sbKey.ToString
                Return sbKey.ToString
            Catch ex As Exception
                'Log(TypeLog.ERREUR, TypeSource.SERVEUR, "ScrambleKey", "Exception : " & ex.Message)
                Return ""
            End Try
        End Function
#End Region

#Region "Log"
        'Dim _File As String = _MonRepertoire & "\logs\log.xml" 'Représente le fichier log: ex"C:\homidom\log\log.xml"
        Dim _FichierLog As String = _MonRepertoire & "\logs\log_" & DateAndTime.Now.ToString("yyyyMMdd") & ".txt"
        Dim _MaxFileSize As Long = 5120 'en Koctets

        ''' <summary>
        ''' Permet de connaître le chemin du fichier log
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property FichierLog() As String
            Get
                'Return _File
                Return _FichierLog
            End Get
        End Property

        ''' <summary>
        ''' Retourne/Fixe la Taille max du fichier log en Ko
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property MaxFileSize() As Long
            Get
                Return _MaxFileSize
            End Get
            Set(ByVal value As Long)
                _MaxFileSize = value
            End Set
        End Property

        ''' <summary>Indique le type du Log: si c'est une erreur, une info, un message...</summary>
        ''' <remarks></remarks>
        Public Enum TypeLog
            INFO = 1                    'divers
            ACTION = 2                  'action lancé par un driver/device/trigger
            MESSAGE = 3
            VALEUR_CHANGE = 4           'Valeur ayant changé
            VALEUR_INCHANGE = 5         'Valeur n'ayant pas changé
            VALEUR_INCHANGE_PRECISION = 6 'Valeur n'ayant pas changé pour cause de precision
            VALEUR_INCHANGE_LASTETAT = 7 'Valeur n'ayant pas changé pour cause de lastetat
            ERREUR = 8                   'erreur générale
            ERREUR_CRITIQUE = 9          'erreur critique demandant la fermeture du programme
            DEBUG = 10                   'visible uniquement si Homidom est en mode debug
        End Enum

        ''' <summary>Indique la source du log si c'est le serveur, un script, un device...</summary>
        ''' <remarks></remarks>
        Public Enum TypeSource
            SERVEUR = 1
            SCRIPT = 2
            TRIGGER = 3
            DEVICE = 4
            DRIVER = 5
            SOAP = 6
            CLIENT = 7
        End Enum

        Private Sub Write4Log(ByVal TypLog As TypeLog, ByVal Source As TypeSource, ByVal Fonction As String, ByVal Message As String)
            _4Log(3) = _4Log(2)
            _4Log(2) = _4Log(1)
            _4Log(1) = _4Log(0)
            _4Log(0) = Now & " - " & TypLog.ToString & " - " & Source.ToString & " - " & Fonction & " - " & Message
        End Sub

        ''' <summary>Ecrit un log dans le fichier log au format xml</summary>
        ''' <param name="TypLog"></param>
        ''' <param name="Source"></param>
        ''' <param name="Fonction"></param>
        ''' <param name="Message"></param>
        ''' <remarks></remarks>
        Public Sub Log(ByVal TypLog As TypeLog, ByVal Source As TypeSource, ByVal Fonction As String, ByVal Message As String) Implements IHoMIDom.Log
            Try

                If _TypeLogEnable(TypLog - 1) = True Then Exit Sub

                'on affiche dans la console
                Console.WriteLine(Now & " " & TypLog.ToString & " " & Source.ToString & " " & Fonction & " " & Message)
                Write4Log(TypLog, Source, Fonction, Message)

                'écriture dans un fichier texte
                _FichierLog = _MonRepertoire & "\logs\log_" & DateAndTime.Now.ToString("yyyyMMdd") & ".txt"
                Dim FreeF As Integer
                Dim texte As String = Now & vbTab & TypLog.ToString & vbTab & Source.ToString & vbTab & Fonction & vbTab & Message

                Try
                    FreeF = FreeFile()
                    texte = Replace(texte, vbLf, vbCrLf)
                    SyncLock lock_logwrite
                        FileOpen(FreeF, FichierLog, OpenMode.Append)
                        Print(FreeF, texte & vbCrLf)
                        'WriteLine(FreeF, texte & vbCrLf)
                        FileClose(FreeF)
                    End SyncLock
                Catch ex As IOException
                    'wait(500)
                    Console.WriteLine(Now & " " & TypLog & " SERVER LOG ERROR IOException : " & ex.ToString)
                Catch ex As Exception
                    'wait(500)
                    Console.WriteLine(Now & " " & TypLog & " SERVER LOG ERROR Exception : " & ex.ToString)
                End Try


                'Dim Fichier As FileInfo
                ''Vérifie si le fichier log existe sinon le crée
                'If IO.File.Exists(_File) Then
                '    Fichier = New FileInfo(_File)
                '    'Vérifie si le fichier est trop gros si oui, on l'archive
                '    If (Fichier.Length / 1000) > _MaxFileSize Then
                '        Dim filearchive As String = Mid(_File, 1, _File.Length - 4) & Now.ToString("_yyyyMMdd_HHmmss") & ".xml"
                '        IO.File.Move(_File, filearchive)
                '    End If
                'Else
                '    CreateNewFileLog(_File)
                '    Fichier = New FileInfo(_File)
                'End If

                ''Dim timeout As DateTime = Now.AddSeconds(3)
                ''Do While FileIsOpen(_File) = True And Now < timeout

                ''Loop

                ''If Now = timeout And FileIsOpen(_File) = True Then
                ''    Console.WriteLine(Now & " Impossible d'écrire dans le fichier log car il est toujours en ouvert, création d'un nouveau fichier log")
                ''    Dim filearchive As String
                ''    filearchive = Mid(_File, 1, _File.Length - 4) & Now.ToString("_yyyyMMdd_HHmmss") & ".xml"
                ''    IO.File.Move(_File, filearchive)
                ''    CreateNewFileLog(_File)
                ''    Fichier = New FileInfo(_File)
                ''End If

                'Dim xmldoc As New XmlDocument()

                'SyncLock lock_logwrite
                '    'Ecrire le log
                '    Try
                '        Dim elelog As XmlElement = xmldoc.CreateElement("log") 'création de l'élément log
                '        Dim atttime As XmlAttribute = xmldoc.CreateAttribute("time") 'création de l'attribut time
                '        Dim atttype As XmlAttribute = xmldoc.CreateAttribute("type") 'création de l'attribut type
                '        Dim attsrc As XmlAttribute = xmldoc.CreateAttribute("source") 'création de l'attribut source
                '        Dim attfct As XmlAttribute = xmldoc.CreateAttribute("fonction") 'création de l'attribut source
                '        Dim attmsg As XmlAttribute = xmldoc.CreateAttribute("message") 'création de l'attribut message

                '        'on affecte les attributs à l'élément
                '        elelog.SetAttributeNode(atttime)
                '        elelog.SetAttributeNode(atttype)
                '        elelog.SetAttributeNode(attsrc)
                '        elelog.SetAttributeNode(attfct)
                '        elelog.SetAttributeNode(attmsg)

                '        'on affecte les valeur
                '        elelog.SetAttribute("time", Now)
                '        elelog.SetAttribute("type", TypLog)
                '        elelog.SetAttribute("source", Source)
                '        elelog.SetAttribute("fonction", HtmlEncode(Fonction))
                '        elelog.SetAttribute("message", HtmlEncode(Message))

                '        'SyncLock lock_logwrite
                '        xmldoc.Load(_File) 'ouvre le fichier xml
                '        Dim root As XmlElement = xmldoc.Item("logs")
                '        root.AppendChild(elelog)
                '        'on enregistre le fichier xml
                '        xmldoc.Save(_File)
                '        xmldoc = Nothing
                '        'End SyncLock

                '    Catch ex As Exception 'Le fichier xml est corrompu ou comporte des caractères non supportés par xml
                '        Console.WriteLine(Now & " Impossible d'écrire dans le fichier log un nouveau fichier à été créé: " & ex.Message)
                '        Dim filearchive As String
                '        filearchive = Mid(_File, 1, _File.Length - 4) & Now.ToString("_yyyyMMdd_HHmmss") & ".xml"
                '        IO.File.Move(_File, filearchive)
                '        CreateNewFileLog(_File)
                '        Fichier = New FileInfo(_File)

                '        xmldoc.Load(_File) 'ouvre le fichier xml
                '        Dim elelog As XmlElement = xmldoc.CreateElement("log") 'création de l'élément log
                '        Dim atttime As XmlAttribute = xmldoc.CreateAttribute("time") 'création de l'attribut time
                '        Dim atttype As XmlAttribute = xmldoc.CreateAttribute("type") 'création de l'attribut type
                '        Dim attsrc As XmlAttribute = xmldoc.CreateAttribute("source") 'création de l'attribut source
                '        Dim attfct As XmlAttribute = xmldoc.CreateAttribute("fonction") 'création de l'attribut source
                '        Dim attmsg As XmlAttribute = xmldoc.CreateAttribute("message") 'création de l'attribut message

                '        'on affecte les attributs à l'élément
                '        elelog.SetAttributeNode(atttime)
                '        elelog.SetAttributeNode(atttype)
                '        elelog.SetAttributeNode(attsrc)
                '        elelog.SetAttributeNode(attfct)
                '        elelog.SetAttributeNode(attmsg)

                '        'on affecte les valeur
                '        elelog.SetAttribute("time", Now)
                '        elelog.SetAttribute("type", TypLog)
                '        elelog.SetAttribute("source", Source)
                '        elelog.SetAttribute("fonction", Fonction)
                '        elelog.SetAttribute("message", HtmlEncode(Message))

                '        Dim root As XmlElement = xmldoc.Item("logs")
                '        root.AppendChild(elelog)

                '        'on enregistre le fichier xml
                '        xmldoc.Save(_File)
                '        xmldoc = Nothing
                '    End Try

                '    Fichier = Nothing
                'End SyncLock
            Catch ex As Exception
                Console.WriteLine("Erreur lors de l'écriture d'un log: " & ex.ToString, MsgBoxStyle.Exclamation, "Erreur Serveur")
            End Try
        End Sub

        'Private Function FileIsOpen(ByVal File As String) As Boolean
        '    Try
        '        'on tente d'ouvrir un stream sur le fichier, s'il est déjà utilisé, cela déclenche une erreur.
        '        Dim fs As IO.FileStream = My.Computer.FileSystem.GetFileInfo(File).Open(IO.FileMode.Open, _
        '        IO.FileAccess.Read)
        '        fs.Close()
        '        Return False
        '    Catch ex As Exception
        '        Return True
        '    End Try
        'End Function


        ' ''' <summary>Créer nouveau Fichier (donner chemin complet et nom) log</summary>
        ' ''' <param name="NewFichier"></param>
        ' ''' <remarks></remarks>
        'Public Sub CreateNewFileLog(ByVal NewFichier As String)
        '    Try
        '        Dim rw As XmlTextWriter = New XmlTextWriter(NewFichier, Nothing)
        '        rw.WriteStartDocument()
        '        rw.WriteStartElement("logs")
        '        rw.WriteStartElement("log")
        '        rw.WriteAttributeString("time", Now)
        '        rw.WriteAttributeString("type", 1)
        '        rw.WriteAttributeString("source", 1)
        '        rw.WriteAttributeString("fonction", "Log")
        '        rw.WriteAttributeString("message", "Création du nouveau fichier log")
        '        rw.WriteEndElement()
        '        rw.WriteEndElement()
        '        rw.WriteEndDocument()
        '        rw.Close()
        '    Catch ex As Exception
        '        Log(TypeLog.ERREUR, TypeSource.SERVEUR, "CreateNewFileLog", "Erreur: " & ex.ToString)
        '    End Try
        'End Sub
#End Region

#Region "Declaration de la classe Server"

        ''' <summary>Déclaration de la class Server</summary>
        ''' <remarks></remarks>
        Public Sub New()

        End Sub

        ''' <summary>
        ''' Redémarre le service et charge la config
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Restart(ByVal IdSrv As String) Implements IHoMIDom.ReStart
            If VerifIdSrv(IdSrv) = False Then
                Exit Sub
            End If

            [stop](_IdSrv)
            start()
        End Sub

        ''' <summary>Démarrage du serveur</summary>
        ''' <remarks></remarks>
        Public Sub start() Implements IHoMIDom.Start
            Try
                Dim retour As String

                'Cree les sous répertoires s'ils nexistent pas
                If System.IO.Directory.Exists(_MonRepertoire & "\Logs") = False Then
                    System.IO.Directory.CreateDirectory(_MonRepertoire & "\Logs")
                    Log(TypeLog.INFO, TypeSource.SERVEUR, "Start", "Création du dossier logs")
                End If
                If System.IO.Directory.Exists(_MonRepertoire & "\Captures") = False Then
                    System.IO.Directory.CreateDirectory(_MonRepertoire & "\captures")
                    Log(TypeLog.INFO, TypeSource.SERVEUR, "Start", "Création du dossier captures")
                End If
                If System.IO.Directory.Exists(_MonRepertoire & "\Config") = False Then
                    System.IO.Directory.CreateDirectory(_MonRepertoire & "\config")
                    Log(TypeLog.INFO, TypeSource.SERVEUR, "Start", "Création du dossier config")
                End If
                If System.IO.Directory.Exists(_MonRepertoire & "\Images") = False Then
                    System.IO.Directory.CreateDirectory(_MonRepertoire & "\Images")
                    Log(TypeLog.INFO, TypeSource.SERVEUR, "Start", "Création du dossier images")
                End If
                If System.IO.Directory.Exists(_MonRepertoire & "\Drivers") = False Then
                    System.IO.Directory.CreateDirectory(_MonRepertoire & "\Drivers")
                    Log(TypeLog.INFO, TypeSource.SERVEUR, "Start", "Création du dossier Drivers")
                End If
                If System.IO.Directory.Exists(_MonRepertoire & "\Templates") = False Then
                    System.IO.Directory.CreateDirectory(_MonRepertoire & "\Templates")
                    Log(TypeLog.INFO, TypeSource.SERVEUR, "Start", "Création du dossier templates")
                End If

                'Charge les types de log
                For i As Integer = 0 To 9
                    _TypeLogEnable.Add(False)
                Next

                'Si sauvegarde automatique
                If _CycleSave > 0 Then _NextTimeSave = Now.AddMinutes(_CycleSave)

                '----- Démarre les connexions Sqlite ----- 
                'retour = sqlite_homidom.connect()
                'If retour.StartsWith("ERR:") Then
                '    Log(TypeLog.ERREUR_CRITIQUE, TypeSource.SERVEUR, "Start", "Erreur lors de la connexion à la BDD Homidom : " & retour)
                '    'on arrête tout


                'Else
                '    Log(TypeLog.INFO, TypeSource.SERVEUR, "Start", "Connexion à la BDD Homidom : " & retour)
                'End If
                'retour = sqlite_medias.connect()
                'If retour.StartsWith("ERR:") Then
                '    Log(TypeLog.ERREUR_CRITIQUE, TypeSource.SERVEUR, "Start", "Erreur lors de la connexion à la BDD Medias : " & retour)
                '    'on arrête tout


                'Else
                '    Log(TypeLog.INFO, TypeSource.SERVEUR, "Start", "Connexion à la BDD Medias : " & retour)
                'End If

                '----- Charge les drivers ----- 
                Drivers_Load()

                '----- Chargement de la config ----- 
                retour = LoadConfig(_MonRepertoire & "\Config\")
                Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", retour)

                '----- Démarre les drivers ----- 
                Drivers_Start()

                '----- Calcul les heures de lever et coucher du soleil ----- 
                MAJ_HeuresSoleil()
                VerifIsJour()

                '----- Maj des triggers type CRON ----- 
                For i = 0 To _ListTriggers.Count - 1
                    'on vérifie si la condition est un cron
                    If _ListTriggers.Item(i).Type = Trigger.TypeTrigger.TIMER Then
                        _ListTriggers.Item(i).maj_cron() 'on calcule la date de prochain execution
                    End If
                Next

                '----- Démarre le Timer -----
                TimerSecond.Interval = 1000
                AddHandler TimerSecond.Elapsed, AddressOf TimerSecTick
                TimerSecond.Enabled = True

                'test avec graphe
                '.grapher_courbe("test", "Temperature extérieure", 800, 400)

                'Change l'etat du server
                Etat_server = True

                'test biblio
                '
                ' Create a FileSystemWatcher object passing it the folder to watch.
                '
                'fsw = New FileSystemWatcher("C:\homidom")
                '
                ' Assign event procedures to the events to watch.
                '
                'AddHandler fsw.Created, AddressOf OnChanged
                'AddHandler fsw.Changed, AddressOf OnChanged
                'AddHandler fsw.Deleted, AddressOf OnChanged
                'AddHandler fsw.Renamed, AddressOf OnRenamed

                'With fsw
                '.EnableRaisingEvents = True
                '.IncludeSubdirectories = True
                '
                ' Specif the event to watch for.
                '
                '.WaitForChanged(WatcherChangeTypes.Created Or _
                '                WatcherChangeTypes.Changed Or _
                '                WatcherChangeTypes.Deleted Or _
                '                WatcherChangeTypes.Renamed)
                '
                ' Watch certain file types.
                '
                '.Filter = "*.txt"
                '
                ' Specify file change notifications.
                '
                '.NotifyFilter = (NotifyFilters.LastAccess Or _
                '                 NotifyFilters.LastWrite Or _
                '                 NotifyFilters.FileName Or _
                '                 NotifyFilters.DirectoryName)
                'End With

                'test log
                CleanLog(_MaxMonthLog)

                Log(TypeLog.INFO, TypeSource.SERVEUR, "Start", "Serveur démarré")
            Catch ex As Exception
                Log(TypeLog.ERREUR_CRITIQUE, TypeSource.SERVEUR, "Start", "Exception : " & ex.Message)
            End Try
        End Sub

        ''' <summary>Arrêt du serveur</summary>
        ''' <remarks></remarks>
        Public Sub [stop](ByVal IdSrv As String) Implements IHoMIDom.Stop
            If VerifIdSrv(IdSrv) = False Then
                Exit Sub
            End If

            Try
                'Dim retour As String

                'on change l'etat du server pour ne plus lancer de traitement
                Etat_server = False

                TimerSecond.Enabled = False
                RemoveHandler TimerSecond.Elapsed, AddressOf TimerSecTick
                TimerSecond.Dispose()

                '----- Arrete les devices ----- 
                Devices_Stop()
                _ListDevices.Clear()

                '----- Arrete les drivers ----- 
                Drivers_Stop()
                _ListDrivers.Clear()

                '----- Vide les variables -----
                _ListExtensionAudio.Clear()
                _ListGroups.Clear()
                _ListImgDrivers.Clear()
                _ListMacros.Clear()
                _ListRepertoireAudio.Clear()
                _ListTagAudio.Clear()
                _ListTriggers.Clear()
                _ListUsers.Clear()
                _ListZones.Clear()

                '----- Arrete les connexions Sqlite -----
                'retour = sqlite_homidom.disconnect("homidom")
                'If Mid(retour, 1, 4) = "ERR:" Then
                '    Log(TypeLog.ERREUR_CRITIQUE, TypeSource.SERVEUR, "Stop", "Erreur lors de la deconnexion de la BDD Homidom : " & retour)
                'End If
                'retour = sqlite_medias.disconnect("medias")
                'If Mid(retour, 1, 4) = "ERR:" Then
                '    Log(TypeLog.ERREUR_CRITIQUE, TypeSource.SERVEUR, "Stop", "Erreur lors de la deconnexion de la BDD Medias : " & retour)
                'End If

                If _CycleSave > 0 Then SaveConfig(_MonRepertoire & "\config\homidom.xml")

                Log(TypeLog.INFO, TypeSource.SERVEUR, "Stop", "Serveur Arrêté")
            Catch ex As Exception
                Log(TypeLog.ERREUR_CRITIQUE, TypeSource.SERVEUR, "Stop", "Exception : " & ex.Message)
            End Try
        End Sub

        Protected Overrides Sub Finalize()
            Try
                'Mettre le Code pour l'arret
                ' [stop]()
                MyBase.Finalize()
            Catch ex As Exception
                Log(TypeLog.ERREUR_CRITIQUE, TypeSource.SERVEUR, "Finalize", "Exception : " & ex.Message)
            End Try
        End Sub
#End Region

#Region "Macro"
        Private Sub Execute(ByVal Id As String)
            Try
                Dim mymacro As New Macro
                mymacro = ReturnMacroById(_IdSrv, Id)
                If mymacro IsNot Nothing Then
                    Log(TypeLog.DEBUG, TypeSource.SERVEUR, "Macro:Action", "Lancement de la macro " & mymacro.Nom)
                    For i = 0 To mymacro.ListActions.Count - 1
                        Dim _Action As New ThreadAction(Me, mymacro.ListActions.Item(i))
                        Dim x As New Thread(AddressOf _Action.Execute)
                        x.Start()
                    Next
                End If
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Macro:Action", ex.ToString)
            End Try
        End Sub
#End Region

#Region "GuideTV"
        Public MyChaine As New List(Of sChaine)
        Public MyProgramme As New List(Of sProgramme)
        Dim MyXML As HoMIDom.XML
        Dim timestart As String

        Public Structure sProgramme
            Dim DateStart As String
            Dim DateEnd As String
            Dim TimeStart As String
            Dim TimeEnd As String
            Dim IDChannel As String
            Dim Titre As String
            Dim SousTitre As String
            Dim Description As String
            Dim Duree As Integer
            Dim Categorie1 As String
            Dim Categorie2 As String
            Dim Annee As Integer
            Dim Credits As String
        End Structure

        Public Structure sChaine
            Dim Nom As String
            Dim ID As String
            Dim Ico As String
            Dim Enable As Integer
            Dim Categorie As String
            Dim Numero As Integer
        End Structure

        Public Function ConvertTextToHTML(ByVal Text As String) As String
            Try
                Text = Replace(Text, "'", "&#191;")
                Text = Replace(Text, "À", "&#192;")
                Text = Replace(Text, "Á", "&#193;")
                Text = Replace(Text, "Â", "&#194;")
                Text = Replace(Text, "Ã", "&#195;")
                Text = Replace(Text, "Ä", "&#196;")
                Text = Replace(Text, "Å", "&#197;")
                Text = Replace(Text, "Æ", "&#198;")
                Text = Replace(Text, "à", "&#224;")
                Text = Replace(Text, "á", "&#225;")
                Text = Replace(Text, "â", "&#226;")
                Text = Replace(Text, "ã", "&#227;")
                Text = Replace(Text, "ä", "&#228;")
                Text = Replace(Text, "å", "&#229;")
                Text = Replace(Text, "æ", "&#230;")
                Text = Replace(Text, "Ç", "&#199;")
                Text = Replace(Text, "ç", "&#231;")
                Text = Replace(Text, "Ð", "&#208;")
                Text = Replace(Text, "ð", "&#240;")
                Text = Replace(Text, "È", "&#200;")
                Text = Replace(Text, "É", "&#201;")
                Text = Replace(Text, "Ê", "&#202;")
                Text = Replace(Text, "Ë", "&#203;")
                Text = Replace(Text, "è", "&#232;")
                Text = Replace(Text, "é", "&#233;")
                Text = Replace(Text, "ê", "&#234;")
                Text = Replace(Text, "ë", "&#235;")
                Text = Replace(Text, "Ì", "&#204;")
                Text = Replace(Text, "Í", "&#205;")
                Text = Replace(Text, "Î", "&#206;")
                Text = Replace(Text, "Ï", "&#207;")
                Text = Replace(Text, "ì", "&#236;")
                Text = Replace(Text, "í", "&#237;")
                Text = Replace(Text, "î", "&#238;")
                Text = Replace(Text, "ï", "&#239;")
                Text = Replace(Text, "Ñ", "&#209;")
                Text = Replace(Text, "ñ", "&#241;")
                Text = Replace(Text, "Ò", "&#210;")
                Text = Replace(Text, "Ó", "&#211;")
                Text = Replace(Text, "Ô", "&#212;")
                Text = Replace(Text, "Õ", "&#213;")
                Text = Replace(Text, "Ö", "&#214;")
                Text = Replace(Text, "Ø", "&#216;")
                Text = Replace(Text, "Œ", "&#140;")
                Text = Replace(Text, "ò", "&#242;")
                Text = Replace(Text, "ó", "&#243;")
                Text = Replace(Text, "ô", "&#244;")
                Text = Replace(Text, "õ", "&#245;")
                Text = Replace(Text, "ö", "&#246;")
                Text = Replace(Text, "ø", "&#248;")
                Text = Replace(Text, "œ", "&#156;")
                Text = Replace(Text, "Š", "&#138;")
                Text = Replace(Text, "š", "&#154;")
                Text = Replace(Text, "Ù", "&#217;")
                Text = Replace(Text, "Ú", "&#218;")
                Text = Replace(Text, "Û", "&#219;")
                Text = Replace(Text, "Ü", "&#220;")
                Text = Replace(Text, "ù", "&#249;")
                Text = Replace(Text, "ú", "&#250;")
                Text = Replace(Text, "û", "&#251;")
                Text = Replace(Text, "ü", "&#252;")
                Text = Replace(Text, "Ý", "&#221;")
                Text = Replace(Text, "Ÿ", "&#159;")
                Text = Replace(Text, "ý", "&#253;")
                Text = Replace(Text, "ÿ", "&#255;")
                Text = Replace(Text, "Ž", "&#142;")
                Text = Replace(Text, "ž", "&#158;")
                Text = Replace(Text, "¢", "&#162;")
                Text = Replace(Text, "£", "&#163;")
                Text = Replace(Text, "¥", "&#165;")
                Text = Replace(Text, "™", "&#153;")
                Text = Replace(Text, "©", "&#169;")
                Text = Replace(Text, "®", "&#174;")
                Text = Replace(Text, "‰", "&#137;")
                Text = Replace(Text, "ª", "&#170;")
                Text = Replace(Text, "º", "&#186;")
                Text = Replace(Text, "¹", "&#185;")
                Text = Replace(Text, "²", "&#178;")
                Text = Replace(Text, "³", "&#179;")
                Text = Replace(Text, "¼", "&#188;")
                Text = Replace(Text, "½", "&#189;")
                Text = Replace(Text, "¾", "&#190;")
                Text = Replace(Text, "÷", "&#247;")
                Text = Replace(Text, "×", "&#215;")
                Text = Replace(Text, ">", "&#155;")
                Text = Replace(Text, "<", "&#139;")
                Text = Replace(Text, "±", "&#177;")
                Text = Replace(Text, "&", "")
                Text = Replace(Text, "‚", "&#130;")
                Text = Replace(Text, "ƒ", "&#131;")
                Text = Replace(Text, "„", "&#132;")
                Text = Replace(Text, "…", "&#133;")
                Text = Replace(Text, "†", "&#134;")
                Text = Replace(Text, "‡", "&#135;")
                Text = Replace(Text, "ˆ", "&#136;")
                Text = Replace(Text, "‘", "&#145;")
                Text = Replace(Text, "’", "&#146;")
                'Text=Replace(text,"“","&#147;")
                'Text=Replace(text,"”","&#148;")
                Text = Replace(Text, "•", "&#149;")
                Text = Replace(Text, "–", "&#150;")
                Text = Replace(Text, "—", "&#151;")
                Text = Replace(Text, "˜", "&#152;")
                Text = Replace(Text, "¿", "&#191;")
                Text = Replace(Text, "¡", "&#161;")
                Text = Replace(Text, "¤", "&#164;")
                Text = Replace(Text, "¦", "&#166;")
                Text = Replace(Text, "§", "&#167;")
                Text = Replace(Text, "¨", "&#168;")
                Text = Replace(Text, "«", "&#171;")
                Text = Replace(Text, "»", "&#187;")
                Text = Replace(Text, "¬", "&#172;")
                Text = Replace(Text, "¯", "&#175;")
                Text = Replace(Text, "´", "&#180;")
                Text = Replace(Text, "µ", "&#181;")
                Text = Replace(Text, "¶", "&#182;")
                Text = Replace(Text, "·", "&#183;")
                Text = Replace(Text, "¸", "&#184;")
                Text = Replace(Text, "þ", "&#222;")
                Text = Replace(Text, "ß", "&#223;")
                ConvertTextToHTML = Text
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "ConvertTextToHTML", "Exception : " & ex.Message)
                Return ""
            End Try
        End Function

        Public Function ConvertHtmlToText(ByVal Text As String) As String
            Try
                Text = Replace(Text, "#191;", "'")
                Text = Replace(Text, "#192;", "À")
                Text = Replace(Text, "#193;", "Á")
                Text = Replace(Text, "#194;", "Â")
                Text = Replace(Text, "#195;", "Ã")
                Text = Replace(Text, "#196;", "Ä")
                Text = Replace(Text, "#197;", "Å")
                Text = Replace(Text, "#198;", "Æ")
                Text = Replace(Text, "#224;", "à")
                Text = Replace(Text, "#225;", "á")
                Text = Replace(Text, "#226;", "â")
                Text = Replace(Text, "#227;", "ã")
                Text = Replace(Text, "#228;", "ä")
                Text = Replace(Text, "#229;", "å")
                Text = Replace(Text, "#230;", "æ")
                Text = Replace(Text, "#199;", "Ç")
                Text = Replace(Text, "#231;", "ç")
                Text = Replace(Text, "#208;", "Ð")
                Text = Replace(Text, "#240;", "ð")
                Text = Replace(Text, "#200;", "È")
                Text = Replace(Text, "#201;", "É")
                Text = Replace(Text, "#202;", "Ê")
                Text = Replace(Text, "#203;", "Ë")
                Text = Replace(Text, "#232;", "è")
                Text = Replace(Text, "#233;", "é")
                Text = Replace(Text, "#234;", "ê")
                Text = Replace(Text, "#235;", "ë")
                Text = Replace(Text, "#204;", "Ì")
                Text = Replace(Text, "#205;", "Í")
                Text = Replace(Text, "#206;", "Î")
                Text = Replace(Text, "#207;", "Ï")
                Text = Replace(Text, "#236;", "ì")
                Text = Replace(Text, "#237;", "í")
                Text = Replace(Text, "#238;", "î")
                Text = Replace(Text, "#239;", "ï")
                Text = Replace(Text, "#209;", "Ñ")
                Text = Replace(Text, "#241;", "ñ")
                Text = Replace(Text, "#210;", "Ò")
                Text = Replace(Text, "#211;", "Ó")
                Text = Replace(Text, "#212;", "Ô")
                Text = Replace(Text, "#213;", "Õ")
                Text = Replace(Text, "#214;", "Ö")
                Text = Replace(Text, "#216;", "Ø")
                Text = Replace(Text, "#140;", "Œ")
                Text = Replace(Text, "#242;", "ò")
                Text = Replace(Text, "#243;", "ó")
                Text = Replace(Text, "#244;", "ô")
                Text = Replace(Text, "#245;", "õ")
                Text = Replace(Text, "#246;", "ö")
                Text = Replace(Text, "#248;", "ø")
                Text = Replace(Text, "#156;", "œ")
                Text = Replace(Text, "#138;", "Š")
                Text = Replace(Text, "#154;", "š")
                Text = Replace(Text, "#217;", "Ù")
                Text = Replace(Text, "#218;", "Ú")
                Text = Replace(Text, "#219;", "Û")
                Text = Replace(Text, "#220;", "Ü")
                Text = Replace(Text, "#249;", "ù")
                Text = Replace(Text, "#250;", "ú")
                Text = Replace(Text, "#251;", "û")
                Text = Replace(Text, "#252;", "ü")
                Text = Replace(Text, "#221;", "Ý")
                Text = Replace(Text, "#159;", "Ÿ")
                Text = Replace(Text, "#253;", "ý")
                Text = Replace(Text, "#255;", "ÿ")
                Text = Replace(Text, "#142;", "Ž")
                Text = Replace(Text, "#158;", "ž")
                Text = Replace(Text, "#162;", "¢")
                Text = Replace(Text, "#163;", "£")
                Text = Replace(Text, "#165;", "¥")
                Text = Replace(Text, "#153;", "™")
                Text = Replace(Text, "#169;", "©")
                Text = Replace(Text, "#174;", "®")
                Text = Replace(Text, "#137;", "‰")
                Text = Replace(Text, "#170;", "ª")
                Text = Replace(Text, "#186;", "º")
                Text = Replace(Text, "#185;", "¹")
                Text = Replace(Text, "#178;", "²")
                Text = Replace(Text, "#179;", "³")
                Text = Replace(Text, "#188;", "¼")
                Text = Replace(Text, "#189;", "½")
                Text = Replace(Text, "#190;", "¾")
                Text = Replace(Text, "#247;", "÷")
                Text = Replace(Text, "#215;", "×")
                Text = Replace(Text, "#155;", ">")
                Text = Replace(Text, "#139;", "<")
                Text = Replace(Text, "#177;", "±")
                'Text = Replace(Text, "", "&")
                Text = Replace(Text, "#130;", "‚")
                Text = Replace(Text, "#131;", "ƒ")
                Text = Replace(Text, "#132;", "„")
                Text = Replace(Text, "#133;", "…")
                Text = Replace(Text, "#134;", "†")
                Text = Replace(Text, "#135;", "‡")
                Text = Replace(Text, "#136;", "ˆ")
                Text = Replace(Text, "#145;", "‘")
                Text = Replace(Text, "#146;", "’")
                Text = Replace(Text, "#149;", "•")
                Text = Replace(Text, "#150;", "–")
                Text = Replace(Text, "#151;", "—")
                Text = Replace(Text, "#152;", "˜")
                Text = Replace(Text, "#191;", "¿")
                Text = Replace(Text, "#161;", "¡")
                Text = Replace(Text, "#164;", "¤")
                Text = Replace(Text, "#166;", "¦")
                Text = Replace(Text, "#167;", "§")
                Text = Replace(Text, "#168;", "¨")
                Text = Replace(Text, "#171;", "«")
                Text = Replace(Text, "#187;", "»")
                Text = Replace(Text, "#172;", "¬")
                Text = Replace(Text, "#175;", "¯")
                Text = Replace(Text, "#180;", "´")
                Text = Replace(Text, "#181;", "µ")
                Text = Replace(Text, "#182;", "¶")
                Text = Replace(Text, "#183;", "·")
                Text = Replace(Text, "#184;", "¸")
                Text = Replace(Text, "#222;", "þ")
                Text = Replace(Text, "#223;", "ß")
                ConvertHtmlToText = Text
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "ConvertHtmlToText", "Exception : " & ex.Message)
                Return ""
            End Try
        End Function


#Region "Gestion des chaines"
        'Permet de lister les chaines dans la base de données
        Public Sub ChaineFromXMLToDB()
            MyXML = New HoMIDom.XML(_MonRepertoire & "\data\complet.xml")
            Dim liste As XmlNodeList = MyXML.SelectNodes("/tv/channel")
            Dim i As Integer
            Dim a As String
            Dim b As String
            Dim SQLconnect As New SQLiteConnection()
            Dim SQLcommand As SQLiteCommand
            SQLconnect.ConnectionString = "Data Source= " & _MonRepertoire & "\bdd\guidetv.db;"
            SQLconnect.Open()
            SQLcommand = SQLconnect.CreateCommand
            SQLcommand.CommandText = "DELETE FROM chaineTV where id<>''"
            SQLcommand.ExecuteNonQuery()
            SQLcommand = SQLconnect.CreateCommand
            Dim SQLreader As SQLiteDataReader

            'liste toute les chaines
            For i = 0 To liste.Count - 1
                a = liste(i).Attributes.Item(0).Value
                'Affiche l'ID et le nom
                SQLcommand.CommandText = "SELECT * FROM chaineTV where ID='" & a & "'"
                SQLreader = SQLcommand.ExecuteReader()
                If SQLreader.HasRows = False Then
                    b = liste(i).ChildNodes.Item(0).ChildNodes.Item(0).Value
                    b = ConvertTextToHTML(b)
                    SQLreader.Close()
                    SQLcommand = SQLconnect.CreateCommand
                    SQLcommand.CommandText = "INSERT INTO chaineTV (id, nom,ico,enable,numero,categorie) VALUES ('" & a & "', '" & b & "','?','0','0','99')"
                    SQLcommand.ExecuteNonQuery()
                End If
            Next
            SQLcommand.Dispose()
            SQLconnect.Close()
            ChargeChaineFromDB()
            MyXML = Nothing
        End Sub

        'Charge les chaines depuis la base de données en mémoire
        Public Sub ChargeChaineFromDB()
            Dim SQLconnect As New SQLiteConnection()
            Dim SQLcommand As SQLiteCommand
            SQLconnect.ConnectionString = "Data Source= " & _MonRepertoire & "\bdd\guidetv.db;"
            SQLconnect.Open()
            SQLcommand = SQLconnect.CreateCommand
            Dim SQLreader As SQLiteDataReader

            SQLcommand.CommandText = "SELECT * FROM chaineTV"
            SQLreader = SQLcommand.ExecuteReader()
            If SQLreader.HasRows = True Then
                While SQLreader.Read()
                    Dim vChaine As sChaine = New sChaine
                    vChaine.Nom = SQLreader(1)
                    vChaine.ID = SQLreader(2)
                    vChaine.Ico = SQLreader(3)
                    vChaine.Enable = SQLreader(4)
                    vChaine.Numero = SQLreader(5)
                    vChaine.Categorie = SQLreader(6)
                    MyChaine.Add(vChaine)
                End While
            Else
                Console.WriteLine(Now & ": aucune chaine à charger depuis la DB!")
            End If
            SQLcommand.Dispose()
            SQLconnect.Close()
        End Sub

#End Region


#Region "Compression"
        Public Function decompression(ByVal cheminSource As String, ByVal cheminDestination As String) As Boolean
            Dim process As Process = New Process()
            process.StartInfo.FileName = "C:\Program Files\7-zip\7z.exe"
            process.StartInfo.Arguments = " e " + cheminSource & " -aoa -o" & cheminDestination
            process.Start()
        End Function
#End Region
#End Region

#Region "Bibliotheques"
        Public Sub SearchTag()
            Try
                For i As Integer = 0 To _ListRepertoireAudio.Count - 1
                    '  Dim x As New Thread(AddressOf FileTagRepload(_ListRepertoireAudio.Item(i).Repertoire))
                Next
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SearchTag", "Exception : " & ex.Message)
            End Try
        End Sub

        Public Class ThreadSearchTag
            Dim _Repertoire As String
            Dim _mylist As List(Of Audio.FilePlayList)

            Sub New(ByVal Repertoire As String)
                _Repertoire = Repertoire
            End Sub


            '''' <summary>Fonction de chargement des tags des fichiers audio des repertoires contenus dans la liste active </summary>
            '''' <remarks>Recupere les fichiers Audios selon les extensions actives</remarks>
            Sub FileTagRepload()

                ' Créér une reference du dossier
                Dim di As New DirectoryInfo(_Repertoire)

                ' Pour chacune des extensions
                For cpt2 = 0 To _ListExtensionAudio.Count - 1

                    Dim _extension As String = _ListExtensionAudio.Item(cpt2).Extension
                    Dim _extensionenable As Boolean = _ListExtensionAudio.Item(cpt2).Enable

                    ' Recupere la liste des fichiers du repertoire si l'extension est active
                    If _extensionenable Then ' Extension active 
                        ' Recuperation des fichiers du repertoire
                        Dim fiArr As FileInfo() = di.GetFiles("*" & _extension, SearchOption.TopDirectoryOnly)

                        ' Boucle sur tous les fichiers du repertoire
                        For i = 0 To fiArr.Length - 1
                            Dim ii = i
                            Dim Resultat = (From FileAudio In _mylist Where FileAudio.SourceWpath = fiArr(ii).FullName Select FileAudio).Count
                            If Resultat = 0 Then
                                Dim X As TagLib.File
                                ' Recupere les tags du fichier Audio 
                                X = TagLib.File.Create(fiArr(i).FullName)
                                Dim a As New Audio.FilePlayList(X.Tag.Title, X.Tag.FirstPerformer, X.Tag.Album, X.Tag.Year, X.Tag.Comment, X.Tag.FirstGenre,
                                                          System.Convert.ToString(X.Properties.Duration.Minutes) & ":" & System.Convert.ToString(Format(X.Properties.Duration.Seconds, "00")),
                                                          fiArr(i).Name, fiArr(i).FullName, X.Tag.Track)

                                _mylist.Add(a)

                                a = Nothing
                                X = Nothing
                            End If
                        Next
                    End If
                Next
            End Sub

        End Class


        Public Sub OnChanged(ByVal source As Object, ByVal e As FileSystemEventArgs)

        End Sub

        Public Sub OnRenamed(ByVal source As Object, ByVal e As RenamedEventArgs)

        End Sub

#End Region

#End Region

#Region "Interface Client via IHOMIDOM"
        '********************************************************************
        'Fonctions/Sub/Propriétés partagées en service soap pour les clients
        '********************************************************************

        '**** PROPRIETES ***************************

        Public Property Devices() As ArrayList
            Get
                Return _ListDevices
            End Get
            Set(ByVal value As ArrayList)
                _ListDevices = value
            End Set
        End Property

        '*** FONCTIONS ******************************************
#Region "Serveur"
        ''' <summary>
        ''' Vérifie si un élément existe dans une zone, une macro, un trigger... avant de le supprimer
        ''' </summary>
        ''' <param name="IdSrv"></param>
        ''' <param name="Id"></param>
        ''' <returns>Retourne une erreur commencant par ERREUR ou la liste des noms des macros, zones...</returns>
        ''' <remarks></remarks>
        Public Function CanDelete(ByVal IdSrv As String, ByVal Id As String) As List(Of String) Implements IHoMIDom.CanDelete
            Dim retour As New List(Of String)
            Try
                Dim thr As New ThreadDelete(Me, IdSrv, Id, retour)
                Dim x As New Thread(AddressOf thr.Traite)
                x.Start()

                Do While retour.Count = 0

                Loop
                Do While retour(retour.Count - 1) <> "0"

                Loop
                Return retour
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "CanDelete", "Erreur : " & ex.Message)
                retour.Add("ERREUR lors de l'exécution de la fonction: " & ex.ToString)
                Return retour
            End Try
        End Function

        Private Class ThreadDelete
            Dim _retour As List(Of String)
            Dim _server As Server
            Dim _id As String
            Dim _Idsrv As String

            Public Sub New(ByVal Server As Server, ByVal IdSrv As String, ByVal Id As String, ByVal Retour As List(Of String))
                _server = Server
                _retour = Retour
                _id = Id
                _Idsrv = IdSrv
            End Sub

            Public Sub Traite()
                _server._CanDelete(_Idsrv, _id, _retour)
            End Sub
        End Class

        Private Sub _CanDelete(ByVal IdSrv As String, ByVal Id As String, ByVal retour As List(Of String))
            Try
                If VerifIdSrv(IdSrv) = False Then
                    retour.Add("ERREUR: L'Id du serveur est erronée")
                    retour.Add("0")
                    Exit Sub
                End If
                If Id = "" Then
                    retour.Add("ERREUR: L'Id est vide")
                    retour.Add("0")
                    Exit Sub
                End If

                'va vérifier toutes les zones
                For i As Integer = 0 To _ListZones.Count - 1
                    For j As Integer = 0 To _ListZones.Item(i).ListElement.Count - 1
                        If _ListZones.Item(i).ListElement.Item(j).ElementID = Id Then
                            AddLabel(retour, "Zone: " & _ListZones.Item(i).Name)
                            Exit For
                        End If
                    Next
                Next
                'va vérifier tous les triggers
                For i As Integer = 0 To _ListTriggers.Count - 1
                    If _ListTriggers.Item(i).ConditionDeviceId = Id Then AddLabel(retour, "Trigger: " & _ListTriggers.Item(i).Nom)
                Next
                'va vérifier toutes les macros
                For i As Integer = 0 To _ListMacros.Count - 1
                    VerifIdInAction(_ListMacros.Item(i).ListActions, Id, _ListMacros.Item(i).Nom, retour)
                Next

                retour.Add("0")
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "_CanDelete", "Erreur : " & ex.Message)
                retour.Add("ERREUR lors de l'exécution de la fonction: " & ex.ToString)
                retour.Add("0")
            End Try
        End Sub

        Private Sub AddLabel(ByVal List As List(Of String), ByVal Message As String)
            For i As Integer = 0 To List.Count - 1
                If List(i) = Message Then Exit Sub
            Next
            List.Add(Message)
        End Sub

        Private Sub VerifIdInAction(ByVal Actions As ArrayList, ByVal Id As String, ByVal NameMacro As String, ByVal Retour As List(Of String))
            Try
                For j As Integer = 0 To Actions.Count - 1
                    Select Case Actions.Item(j).TypeAction
                        Case Action.TypeAction.ActionDevice
                            If Actions.Item(j).IdDevice = Id Then AddLabel(Retour, "Macro: " & NameMacro)
                        Case Action.TypeAction.ActionIf
                            Dim x As Action.ActionIf = Actions.Item(j)
                            For k As Integer = 0 To x.Conditions.Count - 1
                                If x.Conditions.Item(k).IdDevice = Id Then AddLabel(Retour, "Macro: " & NameMacro)
                            Next
                            VerifIdInAction(x.ListTrue, Id, NameMacro, Retour)
                            VerifIdInAction(x.ListFalse, Id, NameMacro, Retour)
                        Case Action.TypeAction.ActionMail
                            If Actions.Item(j).UserId = Id Then AddLabel(Retour, "Macro: " & NameMacro)
                        Case Action.TypeAction.ActionMacro
                            If Actions.Item(j).IdMacro = Id Then AddLabel(Retour, "Macro: " & NameMacro)
                    End Select
                Next
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "VerifIdInAction", "Erreur : " & ex.ToString)
                AddLabel(Retour, "ERREUR lors de l'exécution de la fonction: " & ex.ToString)
            End Try
        End Sub

        ''' <summary>
        ''' Retourne le paramètre de sauvegarde
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetTimeSave(ByVal IdSrv As String) As Integer Implements IHoMIDom.GetTimeSave
            If VerifIdSrv(IdSrv) = False Then
                Return "-1"
                Exit Function
            End If
            Return _CycleSave
        End Function

        ''' <summary>
        ''' Fixe le paramètre de sauvegarde
        ''' </summary>
        ''' <param name="Value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SetTimeSave(ByVal IdSrv As String, ByVal Value As Integer) As String Implements IHoMIDom.SetTimeSave
            If VerifIdSrv(IdSrv) = False Then
                Return 99
                Exit Function
            End If

            If IsNumeric(Value) = False Or Value < 0 Then
                Return "ERR: la valeur doit être numérique, positive et non nulle"
            Else
                _CycleSave = Value
                _NextTimeSave = Now.AddMinutes(Value)
                Return 0
            End If
        End Function

        ''' <summary>
        ''' Retourne l'Id du serveur pour SOAP
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetIdServer(ByVal IdSrv As String) As String Implements IHoMIDom.GetIdServer
            If VerifIdSrv(IdSrv) = False Then
                Return "99"
                Exit Function
            End If
            Return _IdSrv
        End Function

        ''' <summary>
        ''' Fixe l'Id du serveur pour SOAP
        ''' </summary>
        ''' <param name="Value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SetIdServer(ByVal IdSrv As String, ByVal Value As String) As String Implements IHoMIDom.SetIdServer
            If VerifIdSrv(IdSrv) = False Then
                Return 99
                Exit Function
            End If

            If Value = "" Or Value = " " Then
                Return "ERR: l'Id ne peut être null"
            Else
                _IdSrv = Value
                Return 0
            End If
        End Function

        ''' <summary>Retourne la date et heure du dernier démarrage du serveur</summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetLastStartTime() As Date Implements IHoMIDom.GetLastStartTime
            Return _DateTimeLastStart
        End Function

        ''' <summary>Retourne la version du serveur</summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetServerVersion() As String Implements IHoMIDom.GetServerVersion
            Return My.Application.Info.Version.ToString
        End Function

        ''' <summary>Retourne l'heure du serveur</summary>
        ''' <returns>String : heure du serveur</returns>
        Public Function GetTime() As String Implements IHoMIDom.GetTime
            Try
                Return Now.ToLongTimeString
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetTime", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function

        ''' <summary>Permet d'envoyer un message d'un client vers le server</summary>
        ''' <param name="Message"></param>
        ''' <remarks></remarks>
        Public Sub MessageToServer(ByVal Message As String) Implements IHoMIDom.MessageToServer
            Try
                'traiter le message
                Log(TypeLog.MESSAGE, TypeSource.SERVEUR, "MessageToServer", "Message From client : " & Message)
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "MessageToServer", "Exception : " & ex.Message)
            End Try
        End Sub

        ''' <summary>Convert a file on a byte array.</summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetByteFromImage(ByVal file As String) As Byte() Implements IHoMIDom.GetByteFromImage
            Dim array As Byte() = Nothing
            Try
                If file = "" Then
                    Return Nothing
                End If

                If IO.File.Exists(file) = False Then
                    Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetByteFromImage", "le fichier n'existe pas: " & file)
                    Return Nothing
                End If

                Using fs As New FileStream(file, FileMode.Open, FileAccess.Read)
                    Dim reader As New BinaryReader(fs)
                    If reader IsNot Nothing Then
                        array = reader.ReadBytes(CInt(fs.Length))
                        reader.Close()
                        reader = Nothing
                    End If
                End Using
                Return array
                array = Nothing
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetByteFromImage", ex.Message)
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Retourne la liste de tous les fichiers image (png ou jpg) présents sur le serveur
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetListOfImage() As List(Of ImageFile) Implements IHoMIDom.GetListOfImage
            Dim _list As New List(Of ImageFile)

            Try
                Dim dirInfo As New System.IO.DirectoryInfo(_MonRepertoire & "\images\")
                Dim file As System.IO.FileInfo
                Dim files() As System.IO.FileInfo = dirInfo.GetFiles("*.*g", System.IO.SearchOption.AllDirectories)

                If (files IsNot Nothing) Then
                    For Each file In files
                        Dim x As New ImageFile
                        x.Path = file.FullName
                        x.FileName = file.Name
                        _list.Add(x)
                    Next
                End If

                Return _list
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetListOfImage", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function

#End Region

#Region "Historisation"
        Public Function GetAllListHisto(ByVal idsrv As String) As List(Of Historisation) Implements IHoMIDom.GetAllListHisto
            Try
                If VerifIdSrv(idsrv) = False Then
                    Return Nothing
                    Exit Function
                End If

                'If Not sqlite_homidom.getconnecte Then
                '    Dim retour2 As String = sqlite_homidom.connect()
                '    If retour2.StartsWith("ERR:") Then
                '        Log(TypeLog.ERREUR_CRITIQUE, TypeSource.SERVEUR, "GetAllListHisto", "Erreur lors de la connexion à la BDD Homidom : " & retour2)
                '    Else
                '        Log(TypeLog.INFO, TypeSource.SERVEUR, "GetAllListHisto", "Connexion à la BDD Homidom : " & retour2)
                '    End If
                'End If

                Dim result As New DataTable
                result.TableName = "ListHisto"
                Dim retour As String
                Dim commande As String = "select distinct source, device_id from historiques;"
                retour = sqlite_homidom.query(commande, result, "")
                If UCase(Mid(retour, 1, 3)) <> "ERR" Then
                    If result IsNot Nothing Then
                        Dim _list As New List(Of Historisation)
                        For i As Integer = 0 To result.Rows.Count - 1
                            Dim a As New Historisation
                            a.Nom = result.Rows.Item(i).Item(0).ToString
                            a.IdDevice = result.Rows.Item(i).Item(1).ToString
                            _list.Add(a)
                        Next
                        Return _list
                    Else
                        Return Nothing
                    End If
                Else
                    Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetAllListHisto", retour)
                    Return Nothing
                End If
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetAllListHisto", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function

        Public Function GetHisto(ByVal IdSrv As String, ByVal Source As String, ByVal idDevice As String) As List(Of Historisation) Implements IHoMIDom.GetHisto
            Try
                If VerifIdSrv(IdSrv) = False Then
                    Return Nothing
                    Exit Function
                End If

                'If Not sqlite_homidom.connecte Then
                '    Dim retour2 As String = sqlite_homidom.connect()
                '    If retour2.StartsWith("ERR:") Then
                '        Log(TypeLog.ERREUR_CRITIQUE, TypeSource.SERVEUR, "GetHisto", "Erreur lors de la connexion à la BDD Homidom : " & retour2)
                '    Else
                '        Log(TypeLog.INFO, TypeSource.SERVEUR, "GetHisto", "Connexion à la BDD Homidom : " & retour2)
                '    End If
                '    retour2 = Nothing
                'End If

                Dim result As New DataTable("HistoDB")
                Dim retour As String = ""
                Dim commande As String = "select * from historiques where source='" & Source & "' and device_id='" & idDevice & "' ;"
                Dim _list As New List(Of Historisation)

                retour = sqlite_homidom.query(commande, result, "")
                If UCase(Mid(retour, 1, 3)) <> "ERR" Then
                    If result IsNot Nothing Then
                        For i As Integer = 0 To result.Rows.Count - 1
                            Dim a As New Historisation
                            a.Nom = result.Rows.Item(i).Item(2).ToString
                            a.IdDevice = result.Rows.Item(i).Item(1).ToString
                            a.DateTime = CDate(result.Rows.Item(i).Item(3).ToString)
                            a.Value = result.Rows.Item(i).Item(4).ToString
                            _list.Add(a)
                        Next
                        result = Nothing
                        Return _list
                        _list = Nothing
                    Else
                        Return Nothing
                    End If
                Else
                    Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetAllListHisto", retour)
                    result = Nothing
                    _list = Nothing
                    Return Nothing
                End If
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetHisto", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function
#End Region

#Region "Audio"
        ''' <summary>Supprimer une extension Audio</summary>
        ''' <param name="NomExtension"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteExtensionAudio(ByVal IdSrv As String, ByVal NomExtension As String) As Integer Implements IHoMIDom.DeleteExtensionAudio
            Try
                If VerifIdSrv(IdSrv) = False Then
                    Return 99
                    Exit Function
                End If

                Dim retour As Integer = -1
                For i As Integer = 0 To _ListExtensionAudio.Count - 1
                    If NomExtension = _ListExtensionAudio.Item(i).Extension Then
                        _ListExtensionAudio.RemoveAt(i)
                        retour = 0
                        Exit For
                    End If
                Next
                Return retour
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeleteExtensionAudio", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>
        ''' Ajouter une nouvelle extension audio
        ''' </summary>
        ''' <param name="NomExtension"></param>
        ''' <param name="Enable"></param>
        ''' <returns>-1 si déjà existant</returns>
        ''' <remarks></remarks>
        Public Function NewExtensionAudio(ByVal IdSrv As String, ByVal NomExtension As String, Optional ByVal Enable As Boolean = False) As Integer Implements IHoMIDom.NewExtensionAudio
            Try
                If VerifIdSrv(IdSrv) = False Then
                    Return 99
                    Exit Function
                End If

                For i As Integer = 0 To _ListExtensionAudio.Count - 1
                    If _ListExtensionAudio.Item(i).Extension = NomExtension Then
                        Return -1
                        Exit Function
                    End If
                Next
                Dim x As New Audio.ExtensionAudio
                x.Extension = NomExtension
                x.Enable = Enable
                _ListExtensionAudio.Add(x)
                Return 0
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "NewExtensionAudio", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>
        ''' Active ou désactive une extension Audio
        ''' </summary>
        ''' <param name="NomExtension"></param>
        ''' <param name="Enable"></param>
        ''' <returns>-1 si Extension non trouvée</returns>
        ''' <remarks></remarks>
        Public Function EnableExtensionAudio(ByVal IdSrv As String, ByVal NomExtension As String, ByVal Enable As Boolean) As Integer Implements IHoMIDom.EnableExtensionAudio
            Try
                If VerifIdSrv(IdSrv) = False Then
                    Return 99
                    Exit Function
                End If

                Dim retour As Integer = -1
                For i As Integer = 0 To _ListExtensionAudio.Count - 1
                    If _ListExtensionAudio.Item(i).Extension = NomExtension Then
                        _ListExtensionAudio.Item(i).Enable = Enable
                        retour = 0
                    End If
                Next
                Return retour
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "EnableExtensionAudio", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>
        ''' Supprimer un répertoire Audio
        ''' </summary>
        ''' <param name="NomRepertoire"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteRepertoireAudio(ByVal IdSrv As String, ByVal NomRepertoire As String) As Integer Implements IHoMIDom.DeleteRepertoireAudio
            Try
                If VerifIdSrv(IdSrv) = False Then
                    Return 99
                    Exit Function
                End If

                Dim retour As Integer = -1
                For i As Integer = 0 To _ListRepertoireAudio.Count - 1
                    If NomRepertoire = _ListRepertoireAudio.Item(i).Repertoire Then
                        _ListRepertoireAudio.RemoveAt(i)
                        retour = 0
                        Exit For
                    End If
                Next
                Return retour
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeleteRepertoireAudio", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>
        ''' Ajouter un nouveau répertoire audio
        ''' </summary>
        ''' <param name="NomRepertoire"></param>
        ''' <param name="Enable"></param>
        ''' <returns>-1 si déjà existant</returns>
        ''' <remarks></remarks>
        Public Function NewRepertoireAudio(ByVal IdSrv As String, ByVal NomRepertoire As String, Optional ByVal Enable As Boolean = False) As Integer Implements IHoMIDom.NewRepertoireAudio
            Try
                If VerifIdSrv(IdSrv) = False Then
                    Return 99
                    Exit Function
                End If

                For i As Integer = 0 To _ListRepertoireAudio.Count - 1
                    If _ListRepertoireAudio.Item(i).Repertoire = NomRepertoire Then
                        Return -1
                        Exit Function
                    End If
                Next
                Dim x As New Audio.RepertoireAudio
                x.Repertoire = NomRepertoire
                x.Enable = Enable
                _ListRepertoireAudio.Add(x)
                Return 0
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "NewRepertoireAudio", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>
        ''' Active ou désactive un répertoire Audio
        ''' </summary>
        ''' <param name="NomRepertoire"></param>
        ''' <param name="Enable"></param>
        ''' <returns>-1 si répertoire non trouvé</returns>
        ''' <remarks></remarks>
        Public Function EnableRepertoireAudio(ByVal IdSrv As String, ByVal NomRepertoire As String, ByVal Enable As Boolean) As Integer Implements IHoMIDom.EnableRepertoireAudio
            Try
                If VerifIdSrv(IdSrv) = False Then
                    Return 99
                    Exit Function
                End If

                Dim retour As Integer = -1
                For i As Integer = 0 To _ListRepertoireAudio.Count - 1
                    If _ListRepertoireAudio.Item(i).Repertoire = NomRepertoire Then
                        _ListRepertoireAudio.Item(i).Enable = Enable
                        retour = 0
                    End If
                Next
                Return retour
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "EnableRepertoireAudio", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>Retourne la liste de tous les répertoires audio</summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetAllRepertoiresAudio(ByVal IdSrv As String) As List(Of Audio.RepertoireAudio) Implements IHoMIDom.GetAllRepertoiresAudio
            Try
                If VerifIdSrv(IdSrv) = False Then
                    Return Nothing
                    Exit Function
                End If

                Dim _list As New List(Of Audio.RepertoireAudio)
                For i As Integer = 0 To _ListRepertoireAudio.Count - 1
                    Dim x As New Audio.RepertoireAudio
                    With x
                        .Repertoire = _ListRepertoireAudio.Item(i).Repertoire
                        .Enable = _ListRepertoireAudio.Item(i).Enable
                    End With
                    _list.Add(x)
                Next
                Return _list
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetAllRepertoiresAudio", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function

        ''' <summary>Retourne la liste de toutes les extensions audio</summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetAllExtensionsAudio(ByVal IdSrv As String) As List(Of Audio.ExtensionAudio) Implements IHoMIDom.GetAllExtensionsAudio
            Try
                If VerifIdSrv(IdSrv) = False Then
                    Return Nothing
                    Exit Function
                End If

                Dim _list As New List(Of Audio.ExtensionAudio)
                For i As Integer = 0 To _ListExtensionAudio.Count - 1
                    Dim x As New Audio.ExtensionAudio
                    With x
                        .Extension = _ListExtensionAudio.Item(i).Extension
                        .Enable = _ListExtensionAudio.Item(i).Enable
                    End With
                    _list.Add(x)
                Next
                Return _list
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetAllExtensionsAudio", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function
#End Region

#Region "SMTP"
        ''' <summary>Retourne l'adresse du serveur SMTP</summary>
        Public Function GetSMTPServeur(ByVal IdSrv As String) As String Implements IHoMIDom.GetSMTPServeur
            Try
                If VerifIdSrv(IdSrv) = False Then
                    Return 99
                    Exit Function
                End If

                Return _SMTPServeur
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetSMTPServeur", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>Fixe l'adresse du serveur SMTP</summary>
        Public Sub SetSMTPServeur(ByVal IdSrv As String, ByVal Value As String) Implements IHoMIDom.SetSMTPServeur
            Try
                If VerifIdSrv(IdSrv) = False Then
                    Exit Sub
                End If

                _SMTPServeur = Value
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SetSMTPServeur", "Exception : " & ex.Message)
            End Try
        End Sub

        ''' <summary>Retourne le login du serveur SMTP</summary>
        Public Function GetSMTPLogin(ByVal IdSrv As String) As String Implements IHoMIDom.GetSMTPLogin
            Try
                If VerifIdSrv(IdSrv) = False Then
                    Return 99
                    Exit Function
                End If

                Return _SMTPLogin
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetSMTPLogin", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>Fixe le login du serveur SMTP</summary>
        Public Sub SetSMTPLogin(ByVal IDSrv As String, ByVal Value As String) Implements IHoMIDom.SetSMTPLogin
            Try
                If VerifIdSrv(IDSrv) = False Then
                    Exit Sub
                End If

                _SMTPLogin = Value
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SetSMTPLogin", "Exception : " & ex.Message)
            End Try
        End Sub

        ''' <summary>Retourne le password du serveur SMTP</summary>
        Public Function GetSMTPPassword(ByVal IdSrv As String) As String Implements IHoMIDom.GetSMTPPassword
            Try
                If VerifIdSrv(IdSrv) = False Then
                    Return 99
                    Exit Function
                End If

                Return _SMTPassword
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetSMTPPassword", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>Fixe le password du serveur SMTP</summary>
        Public Sub SetSMTPPassword(ByVal IdSrv As String, ByVal Value As String) Implements IHoMIDom.SetSMTPPassword
            Try
                If VerifIdSrv(IdSrv) = False Then
                    Exit Sub
                End If

                _SMTPassword = Value
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SetSMTPPassword", "Exception : " & ex.Message)
            End Try
        End Sub

        ''' <summary>Retourne l'adresse mail du serveur</summary>
        Public Function GetSMTPMailServeur(ByVal IdSrv As String) As String Implements IHoMIDom.GetSMTPMailServeur
            Try
                If VerifIdSrv(IdSrv) = False Then
                    Return 99
                    Exit Function
                End If

                Return _SMTPmailEmetteur
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetSMTPMailServeur", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>Fixe le password du serveur SMTP</summary>
        Public Sub SetSMTPMailServeur(ByVal IdSrv As String, ByVal Value As String) Implements IHoMIDom.SetSMTPMailServeur
            Try
                If VerifIdSrv(IdSrv) = False Then
                    Exit Sub
                End If

                _SMTPmailEmetteur = Value
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SetSMTPPassword", "Exception : " & ex.Message)
            End Try
        End Sub
#End Region

#Region "Gestion Soleil"
        ''' <summary>Retourne l'heure du couché du soleil</summary>
        Function GetHeureCoucherSoleil() As String Implements IHoMIDom.GetHeureCoucherSoleil
            Try
                Return _HeureCoucherSoleil
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetHeureCoucherSoleil", "Exception : " & ex.Message)
                Return ""
            End Try
        End Function

        ''' <summary>Retour l'heure de lever du soleil</summary>
        Function GetHeureLeverSoleil() As String Implements IHoMIDom.GetHeureLeverSoleil
            Try
                Return _HeureLeverSoleil
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetHeureLeverSoleil", "Exception : " & ex.Message)
                Return ""
            End Try
        End Function

        ''' <summary>Retourne la longitude du serveur</summary>
        Function GetLongitude() As Double Implements IHoMIDom.GetLongitude
            Try
                Return _Longitude
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetLongitude", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>Applique une valeur de longitude au serveur</summary>
        ''' <param name="value"></param>
        Sub SetLongitude(ByVal IdSrv As String, ByVal value As Double) Implements IHoMIDom.SetLongitude
            Try
                If VerifIdSrv(IdSrv) = False Then
                    Exit Sub
                End If

                If _Longitude <> value Then
                    _Longitude = value
                    MAJ_HeuresSoleil()
                End If
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SetLongitude", "Exception : " & ex.Message)
            End Try
        End Sub

        ''' <summary>Retourne la latitude du serveur</summary>
        Function GetLatitude() As Double Implements IHoMIDom.GetLatitude
            Try
                Return _Latitude
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetLatitude", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>Applique une valeur de latitude du serveur</summary>
        ''' <param name="value"></param>
        Sub SetLatitude(ByVal IdSrv As String, ByVal value As Double) Implements IHoMIDom.SetLatitude
            Try
                If VerifIdSrv(IdSrv) = False Then
                    Exit Sub
                End If

                If _Latitude <> value Then
                    _Latitude = value
                    MAJ_HeuresSoleil()
                End If
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SetLatitude", "Exception : " & ex.Message)
            End Try
        End Sub

        ''' <summary>Retourne la valeur de correction de l'heure de coucher du soleil</summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetHeureCorrectionCoucher() As Integer Implements IHoMIDom.GetHeureCorrectionCoucher
            Try

                Return _HeureCoucherSoleilCorrection
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetHeureCorrectionCoucher", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>Applique la valeur de correction de l'heure de coucher du soleil</summary>
        ''' <param name="value"></param>
        ''' <remarks></remarks>
        Sub SetHeureCorrectionCoucher(ByVal IdSrv As String, ByVal value As Integer) Implements IHoMIDom.SetHeureCorrectionCoucher
            Try
                If VerifIdSrv(IdSrv) = False Then
                    Exit Sub
                End If

                If _HeureCoucherSoleilCorrection <> value Then
                    _HeureCoucherSoleilCorrection = value
                    MAJ_HeuresSoleil()
                End If
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SetHeureCorrectionCoucher", "Exception : " & ex.Message)
            End Try
        End Sub

        ''' <summary>Retourne la valeur de correction de l'heure de lever du soleil</summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetHeureCorrectionLever() As Integer Implements IHoMIDom.GetHeureCorrectionLever
            Try
                Return _HeureLeverSoleilCorrection
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetHeureCorrectionLever", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>Applique la valeur de correction de l'heure de coucher du soleil</summary>
        ''' <param name="value"></param>
        ''' <remarks></remarks>
        Sub SetHeureCorrectionLever(ByVal IdSrv As String, ByVal value As Integer) Implements IHoMIDom.SetHeureCorrectionLever
            Try
                If VerifIdSrv(IdSrv) = False Then
                    Exit Sub
                End If

                If _HeureLeverSoleilCorrection <> value Then
                    _HeureLeverSoleilCorrection = value
                    MAJ_HeuresSoleil()
                End If
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SetHeureCorrectionLever", "Exception : " & ex.Message)
            End Try
        End Sub
#End Region

#Region "Driver"
        ''' <summary>Supprimer un driver de la config</summary>
        ''' <param name="driverId"></param>
        Public Function DeleteDriver(ByVal IdSrv As String, ByVal driverId As String) As Integer Implements IHoMIDom.DeleteDriver
            Try
                If VerifIdSrv(IdSrv) = False Then
                    Return 99
                    Exit Function
                End If

                If driverId = "DE96B466-2540-11E0-A321-65D7DFD72085" Then
                    Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeleteDriver", "La suppression du driver Virtuel est impossible")
                    Return -1
                End If
                For i As Integer = 0 To _ListDrivers.Count - 1
                    If _ListDrivers.Item(i).Id = driverId Then
                        _ListDrivers.RemoveAt(i)
                        Return 0
                        Exit For
                    End If
                Next
                Return -1
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeleteDriver", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>Arrête un driver par son Id</summary>
        ''' <param name="DriverId"></param>
        ''' <remarks></remarks>
        Public Sub StopDriver(ByVal IdSrv As String, ByVal DriverId As String) Implements IHoMIDom.StopDriver
            Try
                If VerifIdSrv(IdSrv) = False Then
                    Exit Sub
                End If

                For i As Integer = 0 To _ListDrivers.Count - 1
                    If _ListDrivers.Item(i).id = DriverId Then
                        _ListDrivers.Item(i).stop()
                    End If
                Next
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "StopDriver", "Exception : " & ex.Message)
            End Try
        End Sub

        ''' <summary>Démarre un driver par son id</summary>
        ''' <param name="DriverId"></param>
        ''' <remarks></remarks>
        Public Sub StartDriver(ByVal IdSrv As String, ByVal DriverId As String) Implements IHoMIDom.StartDriver
            Try
                If VerifIdSrv(IdSrv) = False Then
                    Exit Sub
                End If

                For i As Integer = 0 To _ListDrivers.Count - 1
                    If _ListDrivers.Item(i).id = DriverId Then
                        _ListDrivers.Item(i).start()
                        Exit For
                    End If
                Next
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "StartDriver", "Exception : " & ex.Message)
            End Try
        End Sub

        Public Function VerifChamp(ByVal Idsrv As String, ByVal DriverId As String, ByVal Champ As String, ByVal Value As Object) As String Implements IHoMIDom.VerifChamp
            Try
                If VerifIdSrv(Idsrv) = False Then
                    Return "L'ID du serveur est erroné pour pouvoir exécuter cette fonction VERIFCHAMP"
                    Exit Function
                End If

                Dim retour As String = "0"

                For i As Integer = 0 To _ListDrivers.Count - 1
                    If _ListDrivers.Item(i).id = DriverId Then
                        retour = _ListDrivers.Item(i).VerifChamp(Champ, Value)
                        Exit For
                    End If
                Next
                Return retour
            Catch ex As Exception
                Return "Une erreur est apparue lors de la vérification du champ " & Champ & ": " & ex.ToString
            End Try
        End Function

        ''' <summary>Retourne la liste de tous les drivers</summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetAllDrivers(ByVal IdSrv As String) As List(Of TemplateDriver) Implements IHoMIDom.GetAllDrivers
            If VerifIdSrv(IdSrv) = False Then
                Return Nothing
                Exit Function
            End If

            Dim _list As New List(Of TemplateDriver)
            Try
                For i As Integer = 0 To _ListDrivers.Count - 1
                    Dim x As New TemplateDriver
                    With x
                        .Nom = _ListDrivers.Item(i).nom
                        .ID = _ListDrivers.Item(i).id
                        .COM = _ListDrivers.Item(i).com
                        .Description = _ListDrivers.Item(i).description
                        .Enable = _ListDrivers.Item(i).enable
                        .IP_TCP = _ListDrivers.Item(i).ip_tcp
                        .IP_UDP = _ListDrivers.Item(i).ip_udp
                        .IsConnect = _ListDrivers.Item(i).isconnect
                        .Modele = _ListDrivers.Item(i).modele
                        .Picture = _ListDrivers.Item(i).picture
                        .Port_TCP = _ListDrivers.Item(i).port_tcp
                        .Port_UDP = _ListDrivers.Item(i).port_udp
                        .Protocol = _ListDrivers.Item(i).protocol
                        .Refresh = _ListDrivers.Item(i).refresh
                        .StartAuto = _ListDrivers.Item(i).startauto
                        .Version = _ListDrivers.Item(i).version
                        For j As Integer = 0 To _ListDrivers.Item(i).DeviceSupport.count - 1
                            .DeviceSupport.Add(_ListDrivers.Item(i).devicesupport.item(j).ToString)
                        Next
                        For j As Integer = 0 To _ListDrivers.Item(i).Parametres.count - 1
                            Dim y As New Driver.Parametre
                            y.Nom = _ListDrivers.Item(i).Parametres.item(j).nom
                            y.Description = _ListDrivers.Item(i).Parametres.item(j).description
                            y.Valeur = _ListDrivers.Item(i).Parametres.item(j).valeur
                            .Parametres.Add(y)
                        Next
                        For j As Integer = 0 To _ListDrivers.Item(i).LabelsDriver.count - 1
                            Dim y As New Driver.cLabels
                            y.NomChamp = _ListDrivers.Item(i).LabelsDriver.item(j).NomChamp
                            y.LabelChamp = _ListDrivers.Item(i).LabelsDriver.item(j).LabelChamp
                            y.Tooltip = _ListDrivers.Item(i).LabelsDriver.item(j).Tooltip
                            y.Parametre = _ListDrivers.Item(i).LabelsDriver.item(j).Parametre
                            .LabelsDriver.Add(y)
                        Next
                        For j As Integer = 0 To _ListDrivers.Item(i).LabelsDevice.count - 1
                            Dim y As New Driver.cLabels
                            y.NomChamp = _ListDrivers.Item(i).LabelsDevice.item(j).NomChamp
                            y.LabelChamp = _ListDrivers.Item(i).LabelsDevice.item(j).LabelChamp
                            y.Tooltip = _ListDrivers.Item(i).LabelsDevice.item(j).Tooltip
                            y.Parametre = _ListDrivers.Item(i).LabelsDevice.item(j).Parametre
                            .LabelsDevice.Add(y)
                        Next
                        Dim _listactdrv As New ArrayList
                        Dim _listactd As New List(Of String)
                        For j As Integer = 0 To Api.ListMethod(_ListDrivers.Item(i)).Count - 1
                            _listactd.Add(Api.ListMethod(_ListDrivers.Item(i)).Item(j).ToString)
                        Next
                        If _listactd.Count > 0 Then
                            For n As Integer = 0 To _listactd.Count - 1
                                Dim a() As String = _listactd.Item(n).Split("|")
                                Dim p As New DeviceAction
                                With p
                                    .Nom = a(0)
                                    If a.Length > 1 Then
                                        For t As Integer = 1 To a.Length - 1
                                            Dim pr As New DeviceAction.Parametre
                                            Dim b() As String = a(t).Split(":")
                                            With pr
                                                .Nom = b(0)
                                                .Type = b(1)
                                            End With
                                            p.Parametres.Add(pr)
                                        Next
                                    End If
                                End With
                                .DeviceAction.Add(p)
                            Next
                        End If

                        _listactd = Nothing
                        _listactdrv = Nothing
                    End With
                    _list.Add(x)
                Next
                _list.Sort(AddressOf sortDriver)
                Return _list
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetAllDrivers", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function

        Private Function sortDriver(ByVal x As TemplateDriver, ByVal y As TemplateDriver) As Integer
            Return x.Nom.CompareTo(y.Nom)
        End Function

        ''' <summary>Sauvegarde ou créer un driver dans la config</summary>
        ''' <param name="driverId"></param>
        ''' <param name="name"></param>
        ''' <param name="enable"></param>
        ''' <param name="startauto"></param>
        ''' <param name="iptcp"></param>
        ''' <param name="porttcp"></param>
        ''' <param name="ipudp"></param>
        ''' <param name="portudp"></param>
        ''' <param name="com"></param>
        ''' <param name="refresh"></param>
        ''' <param name="picture"></param>
        ''' <param name="modele"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SaveDriver(ByVal IdSrv As String, ByVal driverId As String, ByVal name As String, ByVal enable As Boolean, ByVal startauto As Boolean, ByVal iptcp As String, ByVal porttcp As String, ByVal ipudp As String, ByVal portudp As String, ByVal com As String, ByVal refresh As Integer, ByVal picture As String, ByVal modele As String, Optional ByVal Parametres As ArrayList = Nothing) As String Implements IHoMIDom.SaveDriver
            If VerifIdSrv(IdSrv) = False Then
                Return 99
                Exit Function
            End If

            Dim myID As String
            Try
                'Driver Existant
                myID = driverId

                'verification pour ne pas modifier le driver virtuel
                If driverId = "DE96B466-2540-11E0-A321-65D7DFD72085" Then
                    Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SaveDriver", "La modification du driver Virtuel est impossible")
                    Return -1
                End If

                For i As Integer = 0 To _ListDrivers.Count - 1
                    If _ListDrivers.Item(i).id = driverId Then
                        _ListDrivers.Item(i).Enable = enable
                        _ListDrivers.Item(i).StartAuto = startauto
                        If _ListDrivers.Item(i).IP_TCP <> "@" Then _ListDrivers.Item(i).IP_TCP = iptcp
                        If _ListDrivers.Item(i).Port_TCP <> "@" Then _ListDrivers.Item(i).Port_TCP = porttcp
                        If _ListDrivers.Item(i).IP_UDP <> "@" Then _ListDrivers.Item(i).IP_UDP = ipudp
                        If _ListDrivers.Item(i).Port_UDP <> "@" Then _ListDrivers.Item(i).Port_UDP = portudp
                        If _ListDrivers.Item(i).Com <> "@" Then _ListDrivers.Item(i).Com = com
                        _ListDrivers.Item(i).Refresh = refresh
                        _ListDrivers.Item(i).Picture = picture
                        _ListDrivers.Item(i).Modele = modele
                        If Parametres IsNot Nothing Then
                            For j As Integer = 0 To Parametres.Count - 1
                                _ListDrivers.Item(i).parametres.item(j).valeur = Parametres.Item(j)
                            Next
                        End If
                    End If
                Next
                'génération de l'event
                Return myID
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SaveDriver", "Exception : " & ex.Message)
                Return "-1"
            End Try
        End Function

        ''' <summary>Retourne un driver par son ID</summary>
        ''' <param name="DriverId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ReturnDriverById(ByVal IdSrv As String, ByVal DriverId As String) As TemplateDriver Implements IHoMIDom.ReturnDriverByID
            If VerifIdSrv(IdSrv) = False Then
                Return Nothing
                Exit Function
            End If

            Dim retour As New TemplateDriver
            Try
                For i As Integer = 0 To _ListDrivers.Count - 1
                    If _ListDrivers.Item(i).ID = DriverId Then
                        retour.Nom = _ListDrivers.Item(i).nom
                        retour.ID = _ListDrivers.Item(i).id
                        retour.COM = _ListDrivers.Item(i).com
                        retour.Description = _ListDrivers.Item(i).description
                        retour.Enable = _ListDrivers.Item(i).enable
                        retour.IP_TCP = _ListDrivers.Item(i).ip_tcp
                        retour.IP_UDP = _ListDrivers.Item(i).ip_udp
                        retour.IsConnect = _ListDrivers.Item(i).isconnect
                        retour.Modele = _ListDrivers.Item(i).modele
                        retour.Picture = _ListDrivers.Item(i).picture
                        retour.Port_TCP = _ListDrivers.Item(i).port_tcp
                        retour.Port_UDP = _ListDrivers.Item(i).port_udp
                        retour.Protocol = _ListDrivers.Item(i).protocol
                        retour.Refresh = _ListDrivers.Item(i).refresh
                        retour.StartAuto = _ListDrivers.Item(i).startauto
                        retour.Version = _ListDrivers.Item(i).version

                        For j As Integer = 0 To _ListDrivers.Item(i).DeviceSupport.count - 1
                            retour.DeviceSupport.Add(_ListDrivers.Item(i).devicesupport.item(j).ToString)
                        Next
                        For j As Integer = 0 To _ListDrivers.Item(i).Parametres.count - 1
                            Dim y As New Driver.Parametre
                            y.Nom = _ListDrivers.Item(i).Parametres.item(j).nom
                            y.Description = _ListDrivers.Item(i).Parametres.item(j).description
                            y.Valeur = _ListDrivers.Item(i).Parametres.item(j).valeur
                            retour.Parametres.Add(y)
                        Next
                        For j As Integer = 0 To _ListDrivers.Item(i).LabelsDriver.count - 1
                            Dim y As New Driver.cLabels
                            y.NomChamp = _ListDrivers.Item(i).LabelsDriver.item(j).NomChamp
                            y.LabelChamp = _ListDrivers.Item(i).LabelsDriver.item(j).LabelChamp
                            y.Tooltip = _ListDrivers.Item(i).LabelsDriver.item(j).Tooltip
                            y.Parametre = _ListDrivers.Item(i).LabelsDriver.item(j).Parametre
                            retour.LabelsDriver.Add(y)
                        Next
                        For j As Integer = 0 To _ListDrivers.Item(i).LabelsDevice.count - 1
                            Dim y As New Driver.cLabels
                            y.NomChamp = _ListDrivers.Item(i).LabelsDevice.item(j).NomChamp
                            y.LabelChamp = _ListDrivers.Item(i).LabelsDevice.item(j).LabelChamp
                            y.Tooltip = _ListDrivers.Item(i).LabelsDevice.item(j).Tooltip
                            y.Parametre = _ListDrivers.Item(i).LabelsDevice.item(j).Parametre
                            retour.LabelsDevice.Add(y)
                        Next
                        Dim _listactdrv As New ArrayList
                        Dim _listactd As New List(Of String)
                        For j As Integer = 0 To Api.ListMethod(_ListDrivers.Item(i)).Count - 1
                            _listactd.Add(Api.ListMethod(_ListDrivers.Item(i)).Item(j).ToString)
                        Next
                        If _listactd.Count > 0 Then
                            For n As Integer = 0 To _listactd.Count - 1
                                Dim a() As String = _listactd.Item(n).Split("|")
                                Dim p As New DeviceAction
                                With p
                                    .Nom = a(0)
                                    If a.Length > 1 Then
                                        For t As Integer = 1 To a.Length - 1
                                            Dim pr As New DeviceAction.Parametre
                                            Dim b() As String = a(t).Split(":")
                                            With pr
                                                .Nom = b(0)
                                                .Type = b(1)
                                            End With
                                            p.Parametres.Add(pr)
                                        Next
                                    End If
                                End With
                                retour.DeviceAction.Add(p)
                            Next
                        End If

                        _listactd = Nothing
                        _listactdrv = Nothing
                        Exit For
                    End If
                Next
                Return retour
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "ReturnDriverById", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function

        ''' <summary>Retourne un driver par son ID</summary>
        ''' <param name="DriverId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ReturnDrvById(ByVal IdSrv As String, ByVal DriverId As String) As Object
            If VerifIdSrv(IdSrv) = False Then
                Return Nothing
                Exit Function
            End If

            Dim retour As Object = Nothing
            Try
                For i As Integer = 0 To _ListDrivers.Count - 1
                    If _ListDrivers.Item(i).ID = DriverId Then
                        retour = _ListDrivers.Item(i)
                        Exit For
                    End If
                Next
                Return retour
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "ReturnDrvById", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function

        ''' <summary>Retourne le driver par son Nom</summary>
        ''' <param name="DriverNom"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ReturnDriverByNom(ByVal IdSrv As String, ByVal DriverNom As String) As Object Implements IHoMIDom.ReturnDriverByNom
            If VerifIdSrv(IdSrv) = False Then
                Return Nothing
                Exit Function
            End If

            Dim retour As New TemplateDriver
            Try
                For i As Integer = 0 To _ListDrivers.Count - 1
                    If _ListDrivers.Item(i).Nom = DriverNom.ToUpper() Then
                        retour.Nom = _ListDrivers.Item(i).nom
                        retour.ID = _ListDrivers.Item(i).id
                        retour.COM = _ListDrivers.Item(i).com
                        retour.Description = _ListDrivers.Item(i).description
                        retour.Enable = _ListDrivers.Item(i).enable
                        retour.IP_TCP = _ListDrivers.Item(i).ip_tcp
                        retour.IP_UDP = _ListDrivers.Item(i).ip_udp
                        retour.IsConnect = _ListDrivers.Item(i).isconnect
                        retour.Modele = _ListDrivers.Item(i).modele
                        retour.Picture = _ListDrivers.Item(i).picture
                        retour.Port_TCP = _ListDrivers.Item(i).port_tcp
                        retour.Port_UDP = _ListDrivers.Item(i).port_udp
                        retour.Protocol = _ListDrivers.Item(i).protocol
                        retour.Refresh = _ListDrivers.Item(i).refresh
                        retour.StartAuto = _ListDrivers.Item(i).startauto
                        retour.Version = _ListDrivers.Item(i).version

                        For j As Integer = 0 To _ListDrivers.Item(i).DeviceSupport.count - 1
                            retour.DeviceSupport.Add(_ListDrivers.Item(i).devicesupport.item(j).ToString)
                        Next
                        For j As Integer = 0 To _ListDrivers.Item(i).Parametres.count - 1
                            Dim y As New Driver.Parametre
                            y.Nom = _ListDrivers.Item(i).Parametres.item(j).nom
                            y.Description = _ListDrivers.Item(i).Parametres.item(j).description
                            y.Valeur = _ListDrivers.Item(i).Parametres.item(j).valeur
                            retour.Parametres.Add(y)
                        Next
                        For j As Integer = 0 To _ListDrivers.Item(i).LabelsDriver.count - 1
                            Dim y As New Driver.cLabels
                            y.NomChamp = _ListDrivers.Item(i).LabelsDriver.item(j).NomChamp
                            y.LabelChamp = _ListDrivers.Item(i).LabelsDriver.item(j).LabelChamp
                            y.Tooltip = _ListDrivers.Item(i).LabelsDriver.item(j).Tooltip
                            y.Parametre = _ListDrivers.Item(i).LabelsDriver.item(j).Parametre
                            retour.LabelsDriver.Add(y)
                        Next
                        For j As Integer = 0 To _ListDrivers.Item(i).LabelsDevice.count - 1
                            Dim y As New Driver.cLabels
                            y.NomChamp = _ListDrivers.Item(i).LabelsDevice.item(j).NomChamp
                            y.LabelChamp = _ListDrivers.Item(i).LabelsDevice.item(j).LabelChamp
                            y.Tooltip = _ListDrivers.Item(i).LabelsDevice.item(j).Tooltip
                            y.Parametre = _ListDrivers.Item(i).LabelsDevice.item(j).Parametre
                            retour.LabelsDevice.Add(y)
                        Next
                        Dim _listactdrv As New ArrayList
                        Dim _listactd As New List(Of String)
                        For j As Integer = 0 To Api.ListMethod(_ListDrivers.Item(i)).Count - 1
                            _listactd.Add(Api.ListMethod(_ListDrivers.Item(i)).Item(j).ToString)
                        Next
                        If _listactd.Count > 0 Then
                            For n As Integer = 0 To _listactd.Count - 1
                                Dim a() As String = _listactd.Item(n).Split("|")
                                Dim p As New DeviceAction
                                With p
                                    .Nom = a(0)
                                    If a.Length > 1 Then
                                        For t As Integer = 1 To a.Length - 1
                                            Dim pr As New DeviceAction.Parametre
                                            Dim b() As String = a(t).Split(":")
                                            With pr
                                                .Nom = b(0)
                                                .Type = b(1)
                                            End With
                                            p.Parametres.Add(pr)
                                        Next
                                    End If
                                End With
                                retour.DeviceAction.Add(p)
                            Next
                        End If

                        _listactd = Nothing
                        _listactdrv = Nothing
                        Return retour
                        'Return _ListDrivers.Item(i)
                        Exit For
                    End If
                Next
                Return Nothing
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "ReturnDriverByNom", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function

        ''' <summary>Permet d'exécuter une commande Sub d'un Driver</summary>
        ''' <param name="DriverId"></param>
        ''' <param name="Action"></param>
        ''' <remarks></remarks>
        Sub ExecuteDriverCommand(ByVal IdSrv As String, ByVal DriverId As String, ByVal Action As DeviceAction) Implements IHoMIDom.ExecuteDriverCommand
            If VerifIdSrv(IdSrv) = False Then
                Exit Sub
            End If

            Dim _retour As Object
            Dim x As Object = Nothing

            Try
                For i As Integer = 0 To _ListDrivers.Count - 1
                    If _ListDrivers.Item(i).id = DriverId Then
                        x = _ListDrivers.Item(i)
                        Exit For
                    End If
                Next

                If x IsNot Nothing Then

                    If Action.Parametres.Count > 0 Then
                        Select Case Action.Parametres.Count
                            Case 1
                                _retour = CallByName(x, Action.Nom, CallType.Method, Action.Parametres.Item(0).Value)
                            Case 2
                                _retour = CallByName(x, Action.Nom, CallType.Method, Action.Parametres.Item(0).Value, Action.Parametres.Item(1).Value)
                            Case 3
                                _retour = CallByName(x, Action.Nom, CallType.Method, Action.Parametres.Item(0).Value, Action.Parametres.Item(1).Value, Action.Parametres.Item(2).Value)
                            Case 4
                                _retour = CallByName(x, Action.Nom, CallType.Method, Action.Parametres.Item(0).Value, Action.Parametres.Item(1).Value, Action.Parametres.Item(2).Value, Action.Parametres.Item(3).Value)
                            Case 5
                                _retour = CallByName(x, Action.Nom, CallType.Method, Action.Parametres.Item(0).Value, Action.Parametres.Item(1).Value, Action.Parametres.Item(2).Value, Action.Parametres.Item(3).Value, Action.Parametres.Item(4).Value)
                        End Select
                    Else
                        CallByName(x, Action.Nom, CallType.Method)
                    End If
                End If
            Catch ex As Exception
                MsgBox("Erreur lors du traitement de la commande ExecuteDriverCommand: " & ex.Message, MsgBoxStyle.Exclamation, "Erreur Serveur")
            End Try
        End Sub

#End Region

#Region "Device"

        ''' <summary>
        ''' Supprimer un device de la config
        ''' </summary>
        ''' <param name="IdSrv"></param>
        ''' <param name="deviceId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteDevice(ByVal IdSrv As String, ByVal deviceId As String) As Integer Implements IHoMIDom.DeleteDevice
            If VerifIdSrv(IdSrv) = False Then
                Return 99
                Exit Function
            End If

            Try
                For i As Integer = 0 To _ListDevices.Count - 1
                    If _ListDevices.Item(i).Id = deviceId Then
                        'on teste si c'est un device systeme pour ne pas le supprimer
                        If Left(_ListDevices.Item(i).Name, 5) = "HOMI_" Then
                            Return -2
                            Exit Function
                        End If
                        'on arrete le timer en forçant le refresh à 0
                        _ListDevices.Item(i).refresh = 0
                        _ListDevices.Item(i).driver.deletedevice(deviceId)
                        _ListDevices.RemoveAt(i)
                        DeleteDevice = 0
                        Exit Function
                    End If
                Next
                Return -1
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeleteDevice", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>Retourne la liste de tous les devices</summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetAllDevices(ByVal IdSrv As String) As List(Of TemplateDevice) Implements IHoMIDom.GetAllDevices
            If VerifIdSrv(IdSrv) = False Then
                Return Nothing
                Exit Function
            End If

            Dim _list As New List(Of TemplateDevice)
            Try
                For i As Integer = 0 To _ListDevices.Count - 1
                    Dim x As New TemplateDevice
                    Dim _listact As New List(Of String)

                    With x
                        .Name = _ListDevices.Item(i).name
                        .ID = _ListDevices.Item(i).id
                        .Enable = _ListDevices.Item(i).enable
                        .LastEtat = _ListDevices.Item(i).LastEtat
                        Select Case UCase(_ListDevices.Item(i).type)
                            Case "APPAREIL" : .Type = Device.ListeDevices.APPAREIL  'modules pour diriger un appareil  ON/OFF
                            Case "AUDIO" : .Type = Device.ListeDevices.AUDIO
                            Case "BAROMETRE" : .Type = Device.ListeDevices.BAROMETRE  'pour stocker les valeur issu d'un barometre meteo ou web
                            Case "BATTERIE" : .Type = Device.ListeDevices.BAROMETRE
                            Case "COMPTEUR" : .Type = Device.ListeDevices.COMPTEUR  'compteur DS2423, RFXPower...
                            Case "CONTACT" : .Type = Device.ListeDevices.CONTACT  'detecteur de contact : switch 1-wire
                            Case "DETECTEUR" : .Type = Device.ListeDevices.DETECTEUR  'tous detecteurs : mouvement, obscurite...
                            Case "DIRECTIONVENT" : .Type = Device.ListeDevices.DIRECTIONVENT
                            Case "ENERGIEINSTANTANEE" : .Type = Device.ListeDevices.ENERGIEINSTANTANEE
                            Case "ENERGIETOTALE" : .Type = Device.ListeDevices.ENERGIETOTALE
                            Case "FREEBOX" : .Type = Device.ListeDevices.FREEBOX
                            Case "GENERIQUEBOOLEEN" : .Type = Device.ListeDevices.GENERIQUEBOOLEEN
                            Case "GENERIQUESTRING" : .Type = Device.ListeDevices.GENERIQUESTRING
                            Case "GENERIQUEVALUE" : .Type = Device.ListeDevices.GENERIQUEVALUE
                            Case "HUMIDITE" : .Type = Device.ListeDevices.HUMIDITE
                            Case "LAMPE" : .Type = Device.ListeDevices.LAMPE
                            Case "METEO" : .Type = Device.ListeDevices.METEO
                            Case "MULTIMEDIA" : .Type = Device.ListeDevices.MULTIMEDIA
                            Case "PLUIECOURANT" : .Type = Device.ListeDevices.PLUIECOURANT
                            Case "PLUIETOTAL" : .Type = Device.ListeDevices.PLUIETOTAL
                            Case "SWITCH" : .Type = Device.ListeDevices.SWITCH
                            Case "TELECOMMANDE" : .Type = Device.ListeDevices.TELECOMMANDE
                            Case "TEMPERATURE" : .Type = Device.ListeDevices.TEMPERATURE
                            Case "TEMPERATURECONSIGNE" : .Type = Device.ListeDevices.TEMPERATURECONSIGNE
                            Case "UV" : .Type = Device.ListeDevices.UV
                            Case "VITESSEVENT" : .Type = Device.ListeDevices.VITESSEVENT
                            Case "VOLET" : .Type = Device.ListeDevices.VOLET
                        End Select
                        .Description = _ListDevices.Item(i).description
                        .Adresse1 = _ListDevices.Item(i).adresse1
                        .Adresse2 = _ListDevices.Item(i).adresse2
                        .DriverID = _ListDevices.Item(i).driverid
                        .Picture = _ListDevices.Item(i).picture
                        .Solo = _ListDevices.Item(i).solo
                        .Refresh = _ListDevices.Item(i).refresh
                        .Modele = _ListDevices.Item(i).modele
                        .GetDeviceCommandePlus = _ListDevices.Item(i).GetCommandPlus
                        .Value = _ListDevices.Item(i).value
                        If IsNumeric(_ListDevices.Item(i).valuelast) Then .ValueLast = _ListDevices.Item(i).valuelast

                        _listact = ListMethod(_ListDevices.Item(i).id)
                        If _listact.Count > 0 Then
                            For n As Integer = 0 To _listact.Count - 1
                                Dim a() As String = _listact.Item(n).Split("|")
                                Dim p As New DeviceAction
                                With p
                                    .Nom = a(0)
                                    If a.Length > 1 Then
                                        For t As Integer = 1 To a.Length - 1
                                            Dim pr As New DeviceAction.Parametre
                                            Dim b() As String = a(t).Split(":")
                                            With pr
                                                .Nom = b(0)
                                                .Type = b(1)
                                            End With
                                            p.Parametres.Add(pr)
                                        Next
                                    End If
                                End With
                                .DeviceAction.Add(p)
                                a = Nothing
                                p = Nothing
                            Next
                        End If
                        _listact = Nothing

                        If .Type = (Device.ListeDevices.BAROMETRE) _
                                        Or .Type = Device.ListeDevices.COMPTEUR _
                                        Or .Type = Device.ListeDevices.ENERGIEINSTANTANEE _
                                        Or .Type = Device.ListeDevices.ENERGIETOTALE _
                                        Or .Type = Device.ListeDevices.GENERIQUEVALUE _
                                        Or .Type = Device.ListeDevices.HUMIDITE _
                                        Or .Type = Device.ListeDevices.PLUIECOURANT _
                                        Or .Type = Device.ListeDevices.PLUIETOTAL _
                                        Or .Type = Device.ListeDevices.TEMPERATURE _
                                        Or .Type = Device.ListeDevices.TEMPERATURECONSIGNE _
                                        Or .Type = Device.ListeDevices.VITESSEVENT _
                                        Or .Type = Device.ListeDevices.UV _
                                        Or .Type = Device.ListeDevices.VITESSEVENT _
                                        Then
                            .Correction = _ListDevices.Item(i).correction
                            .Precision = _ListDevices.Item(i).precision
                            .Formatage = _ListDevices.Item(i).formatage
                            .ValueDef = _ListDevices.Item(i).valuedef
                            .ValueMax = _ListDevices.Item(i).valuemax
                            .ValueMin = _ListDevices.Item(i).valuemin
                        End If

                        If .Type = Device.ListeDevices.METEO Then
                            .ConditionActuel = _ListDevices.Item(i).ConditionActuel
                            .ConditionJ1 = _ListDevices.Item(i).ConditionJ1
                            .ConditionJ2 = _ListDevices.Item(i).ConditionActuel
                            .ConditionJ3 = _ListDevices.Item(i).ConditionJ3
                            .ConditionToday = _ListDevices.Item(i).ConditionToday
                            .HumiditeActuel = _ListDevices.Item(i).HumiditeActuel
                            .IconActuel = _ListDevices.Item(i).IconActuel
                            .IconJ1 = _ListDevices.Item(i).IconJ1
                            .IconJ2 = _ListDevices.Item(i).IconJ2
                            .IconJ3 = _ListDevices.Item(i).IconJ3
                            .IconToday = _ListDevices.Item(i).IconToday
                            .JourJ1 = _ListDevices.Item(i).JourJ1
                            .JourJ2 = _ListDevices.Item(i).JourJ2
                            .JourJ3 = _ListDevices.Item(i).JourJ3
                            .JourToday = _ListDevices.Item(i).JourToday
                            .MaxJ1 = _ListDevices.Item(i).MaxJ1
                            .MaxJ2 = _ListDevices.Item(i).MaxJ2
                            .MaxJ3 = _ListDevices.Item(i).MaxJ3
                            .MaxToday = _ListDevices.Item(i).MaxToday
                            .MinJ1 = _ListDevices.Item(i).MinJ1
                            .MinJ2 = _ListDevices.Item(i).MinJ2
                            .MinJ3 = _ListDevices.Item(i).MinJ3
                            .MinToday = _ListDevices.Item(i).MinToday
                            .TemperatureActuel = _ListDevices.Item(i).TemperatureActuel
                            .VentActuel = _ListDevices.Item(i).VentActuel
                        End If

                        If .Type = Device.ListeDevices.MULTIMEDIA Then
                            .Commandes = _ListDevices.Item(i).Commandes
                            'For j As Integer = 0 To _ListDevices.Item(i).listcommandname.count - 1
                            '    .ListCommandName.Add(_ListDevices.Item(i).listcommandname.item(j))
                            '    .ListCommandData.Add(_ListDevices.Item(i).ListCommandData.item(j))
                            '    .ListCommandRepeat.Add(_ListDevices.Item(i).ListCommandRepeat.item(j))
                            'Next
                        End If
                    End With
                    _list.Add(x)
                Next
                _list.Sort(AddressOf sortDevice)
                Return _list
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetAllDevices", "Exception : " & ex.ToString)
                Return Nothing
            End Try
        End Function

        Private Function sortDevice(ByVal x As TemplateDevice, ByVal y As TemplateDevice) As Integer
            Return x.Name.CompareTo(y.Name)
        End Function

        ''' <summary>
        ''' Sauvegarder ou créer un device
        ''' </summary>
        ''' <param name="IdSrv"></param>
        ''' <param name="deviceId"></param>
        ''' <param name="name"></param>
        ''' <param name="address1"></param>
        ''' <param name="enable"></param>
        ''' <param name="solo"></param>
        ''' <param name="driverid"></param>
        ''' <param name="type"></param>
        ''' <param name="refresh"></param>
        ''' <param name="address2"></param>
        ''' <param name="image"></param>
        ''' <param name="modele"></param>
        ''' <param name="description"></param>
        ''' <param name="lastchangeduree"></param>
        ''' <param name="lastEtat"></param>
        ''' <param name="correction"></param>
        ''' <param name="formatage"></param>
        ''' <param name="precision"></param>
        ''' <param name="valuemax"></param>
        ''' <param name="valuemin"></param>
        ''' <param name="valuedef"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SaveDevice(ByVal IdSrv As String, ByVal deviceId As String, ByVal name As String, ByVal address1 As String, ByVal enable As Boolean, ByVal solo As Boolean, ByVal driverid As String, ByVal type As String, ByVal refresh As Integer, Optional ByVal address2 As String = "", Optional ByVal image As String = "", Optional ByVal modele As String = "", Optional ByVal description As String = "", Optional ByVal lastchangeduree As Integer = 0, Optional ByVal lastEtat As Boolean = True, Optional ByVal correction As Double = 0, Optional ByVal formatage As String = "", Optional ByVal precision As Double = 0, Optional ByVal valuemax As Double = 9999, Optional ByVal valuemin As Double = -9999, Optional ByVal valuedef As Double = 0, Optional ByVal Commandes As List(Of Telecommande.Commandes) = Nothing) As String Implements IHoMIDom.SaveDevice
            If VerifIdSrv(IdSrv) = False Then
                Return 99
                Exit Function
            End If

            Dim myID As String
            Try
                If deviceId = "" Then 'C'est un nouveau device
                    myID = Api.GenerateGUID
                    'Suivant chaque type de device
                    Select Case UCase(type)
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
                                .DriverID = driverid
                                .Modele = modele
                                .Refresh = refresh
                                .Solo = solo
                                .Description = description
                                .LastChangeDuree = lastchangeduree
                                .LastEtat = lastEtat
                                .Correction = correction
                                .Formatage = formatage
                                .Precision = precision
                                .ValueMax = valuemax
                                .ValueMin = valuemin
                                .ValueDef = valuedef
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
                                .DriverID = driverid
                                .Modele = modele
                                .Refresh = refresh
                                .Solo = solo
                                .Description = description
                                .LastChangeDuree = lastchangeduree
                                .LastEtat = lastEtat
                                .Correction = correction
                                .Formatage = formatage
                                .Precision = precision
                                .ValueMax = valuemax
                                .ValueMin = valuemin
                                .ValueDef = valuedef
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
                                .DriverID = driverid
                                .Modele = modele
                                .Refresh = refresh
                                .Solo = solo
                                .Description = description
                                .LastChangeDuree = lastchangeduree
                                .LastEtat = lastEtat
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
                                .DriverID = driverid
                                .Modele = modele
                                .Refresh = refresh
                                .Solo = solo
                                .Description = description
                                .LastChangeDuree = lastchangeduree
                                .LastEtat = lastEtat
                                .Correction = correction
                                .Formatage = formatage
                                .Precision = precision
                                .ValueMax = valuemax
                                .ValueMin = valuemin
                                .ValueDef = valuedef
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
                                .DriverID = driverid
                                .Modele = modele
                                .Refresh = refresh
                                .Solo = solo
                                .Description = description
                                .LastChangeDuree = lastchangeduree
                                .LastEtat = lastEtat
                                .Correction = correction
                                .Formatage = formatage
                                .Precision = precision
                                .ValueMax = valuemax
                                .ValueMin = valuemin
                                .ValueDef = valuedef
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
                                .DriverID = driverid
                                .Modele = modele
                                .Refresh = refresh
                                .Solo = solo
                                .Description = description
                                .LastChangeDuree = lastchangeduree
                                .LastEtat = lastEtat
                                .Correction = correction
                                .Formatage = formatage
                                .Precision = precision
                                .ValueMax = valuemax
                                .ValueMin = valuemin
                                .ValueDef = valuedef
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
                                .DriverID = driverid
                                .Modele = modele
                                .Refresh = refresh
                                .Solo = solo
                                .Description = description
                                .LastChangeDuree = lastchangeduree
                                .LastEtat = lastEtat
                                .Correction = correction
                                .Formatage = formatage
                                .Precision = precision
                                .ValueMax = valuemax
                                .ValueMin = valuemin
                                .ValueDef = valuedef
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
                                .DriverID = driverid
                                .Modele = modele
                                .Refresh = refresh
                                .Solo = solo
                                .Description = description
                                .LastChangeDuree = lastchangeduree
                                .LastEtat = lastEtat
                                .Correction = correction
                                .Formatage = formatage
                                .Precision = precision
                                .ValueMax = valuemax
                                .ValueMin = valuemin
                                .ValueDef = valuedef
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
                                .DriverID = driverid
                                .Modele = modele
                                .Refresh = refresh
                                .Solo = solo
                                .Description = description
                                .LastChangeDuree = lastchangeduree
                                .LastEtat = lastEtat
                                .Correction = correction
                                .Formatage = formatage
                                .Precision = precision
                                .ValueMax = valuemax
                                .ValueMin = valuemin
                                .ValueDef = valuedef
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
                                .DriverID = driverid
                                .Modele = modele
                                .Refresh = refresh
                                .Solo = solo
                                .Description = description
                                .LastChangeDuree = lastchangeduree
                                .LastEtat = lastEtat
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
                                .DriverID = driverid
                                .Modele = modele
                                .Refresh = refresh
                                .Solo = solo
                                .Description = description
                                .LastChangeDuree = lastchangeduree
                                .LastEtat = lastEtat
                                .Correction = correction
                                .Formatage = formatage
                                .Precision = precision
                                .ValueMax = valuemax
                                .ValueMin = valuemin
                                .ValueDef = valuedef
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
                                .DriverID = driverid
                                .Modele = modele
                                .Refresh = refresh
                                .Solo = solo
                                .Description = description
                                .LastChangeDuree = lastchangeduree
                                .LastEtat = lastEtat
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
                                .DriverID = driverid
                                .Modele = modele
                                .Refresh = refresh
                                .Solo = solo
                                .Description = description
                                .LastChangeDuree = lastchangeduree
                                .LastEtat = lastEtat
                                .Correction = correction
                                .Formatage = formatage
                                .Precision = precision
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
                                .DriverID = driverid
                                .Modele = modele
                                .Refresh = refresh
                                .Solo = solo
                                .Description = description
                                .LastChangeDuree = lastchangeduree
                                .LastEtat = lastEtat
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
                                .DriverID = driverid
                                .Modele = modele
                                .Refresh = refresh
                                .Solo = solo
                                .Description = description
                                .LastChangeDuree = lastchangeduree
                                .LastEtat = lastEtat
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
                                .DriverID = driverid
                                .Modele = modele
                                .Refresh = refresh
                                .Solo = solo
                                .Description = description
                                .LastChangeDuree = lastchangeduree
                                .LastEtat = lastEtat
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
                                .DriverID = driverid
                                .Modele = modele
                                .Refresh = refresh
                                .Solo = solo
                                .Description = description
                                .LastChangeDuree = lastchangeduree
                                .LastEtat = lastEtat
                                '   .Commandes = Commandes
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
                                .DriverID = driverid
                                .Modele = modele
                                .Refresh = refresh
                                .Solo = solo
                                .Description = description
                                .LastChangeDuree = lastchangeduree
                                .LastEtat = lastEtat
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
                                .DriverID = driverid
                                .Modele = modele
                                .Refresh = refresh
                                .Solo = solo
                                .Description = description
                                .LastChangeDuree = lastchangeduree
                                .LastEtat = lastEtat
                                .Correction = correction
                                .Formatage = formatage
                                .Precision = precision
                                AddHandler o.DeviceChanged, AddressOf DeviceChange
                            End With
                            _ListDevices.Add(o)
                        Case "BAROMETRE"
                            Dim o As New Device.BAROMETRE(Me)
                            With o
                                .ID = myID
                                .Name = name
                                .DateCreated = Now
                                .Picture = image
                                .Adresse1 = address1
                                .Adresse2 = address2
                                .Enable = enable
                                .DriverID = driverid
                                .Modele = modele
                                .Refresh = refresh
                                .Solo = solo
                                .Description = description
                                .LastChangeDuree = lastchangeduree
                                .LastEtat = lastEtat
                                .Correction = correction
                                .Formatage = formatage
                                .Precision = precision
                                .ValueMax = valuemax
                                .ValueMin = valuemin
                                .ValueDef = valuedef
                                AddHandler o.DeviceChanged, AddressOf DeviceChange
                            End With
                            _ListDevices.Add(o)
                        Case "COMPTEUR"
                            Dim o As New Device.COMPTEUR(Me)
                            With o
                                .ID = myID
                                .Name = name
                                .DateCreated = Now
                                .Picture = image
                                .Adresse1 = address1
                                .Adresse2 = address2
                                .Enable = enable
                                .DriverID = driverid
                                .Modele = modele
                                .Refresh = refresh
                                .Solo = solo
                                .Description = description
                                .LastChangeDuree = lastchangeduree
                                .LastEtat = lastEtat
                                .Correction = correction
                                .Formatage = formatage
                                .Precision = precision
                                .ValueMax = valuemax
                                .ValueMin = valuemin
                                .ValueDef = valuedef
                                AddHandler o.DeviceChanged, AddressOf DeviceChange
                            End With
                            _ListDevices.Add(o)
                        Case "DETECTEUR"
                            Dim o As New Device.DETECTEUR(Me)
                            With o
                                .ID = myID
                                .Name = name
                                .DateCreated = Now
                                .Picture = image
                                .Adresse1 = address1
                                .Adresse2 = address2
                                .Enable = enable
                                .DriverID = driverid
                                .Modele = modele
                                .Refresh = refresh
                                .Solo = solo
                                .Description = description
                                .LastChangeDuree = lastchangeduree
                                .LastEtat = lastEtat
                                AddHandler o.DeviceChanged, AddressOf DeviceChange
                            End With
                            _ListDevices.Add(o)
                        Case "GENERIQUEBOOLEEN"
                            Dim o As New Device.GENERIQUEBOOLEEN(Me)
                            With o
                                .ID = myID
                                .Name = name
                                .DateCreated = Now
                                .Picture = image
                                .Adresse1 = address1
                                .Adresse2 = address2
                                .Enable = enable
                                .DriverID = driverid
                                .Modele = modele
                                .Refresh = refresh
                                .Solo = solo
                                .Description = description
                                .LastChangeDuree = lastchangeduree
                                .LastEtat = lastEtat
                                AddHandler o.DeviceChanged, AddressOf DeviceChange
                            End With
                            _ListDevices.Add(o)
                        Case "GENERIQUESTRING"
                            Dim o As New Device.GENERIQUESTRING(Me)
                            With o
                                .ID = myID
                                .Name = name
                                .DateCreated = Now
                                .Picture = image
                                .Adresse1 = address1
                                .Adresse2 = address2
                                .Enable = enable
                                .DriverID = driverid
                                .Modele = modele
                                .Refresh = refresh
                                .Solo = solo
                                .Description = description
                                .LastChangeDuree = lastchangeduree
                                .LastEtat = lastEtat
                                AddHandler o.DeviceChanged, AddressOf DeviceChange
                            End With
                            _ListDevices.Add(o)
                        Case "GENERIQUEVALUE"
                            Dim o As New Device.GENERIQUEVALUE(Me)
                            With o
                                .ID = myID
                                .Name = name
                                .DateCreated = Now
                                .Picture = image
                                .Adresse1 = address1
                                .Adresse2 = address2
                                .Enable = enable
                                .DriverID = driverid
                                .Modele = modele
                                .Refresh = refresh
                                .Solo = solo
                                .Description = description
                                .LastChangeDuree = lastchangeduree
                                .LastEtat = lastEtat
                                .Correction = correction
                                .Formatage = formatage
                                .Precision = precision
                                .ValueMax = valuemax
                                .ValueMin = valuemin
                                .ValueDef = valuedef
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
                                .DriverID = driverid
                                .Modele = modele
                                .Refresh = refresh
                                .Solo = solo
                                .Description = description
                                .LastChangeDuree = lastchangeduree
                                .LastEtat = lastEtat
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
                                .DriverID = driverid
                                .Modele = modele
                                .Refresh = refresh
                                .Solo = solo
                                .Description = description
                                .LastChangeDuree = lastchangeduree
                                .LastEtat = lastEtat
                                AddHandler o.DeviceChanged, AddressOf DeviceChange
                            End With
                            _ListDevices.Add(o)
                            o.Driver.newdevice(deviceId)
                    End Select
                Else 'Device Existant
                    myID = deviceId
                    For i As Integer = 0 To _ListDevices.Count - 1
                        If _ListDevices.Item(i).ID = deviceId Then

                            'on teste si c'est un device systeme pour ne pas le modifier
                            If Left(_ListDevices.Item(i).Name, 5) = "HOMI_" Then
                                Return -2
                                Exit Function
                            End If

                            _ListDevices.Item(i).name = name
                            _ListDevices.Item(i).adresse1 = address1
                            _ListDevices.Item(i).adresse2 = address2
                            _ListDevices.Item(i).picture = image
                            _ListDevices.Item(i).enable = enable
                            _ListDevices.Item(i).driverid = driverid
                            _ListDevices.Item(i).description = description
                            _ListDevices.Item(i).modele = modele
                            _ListDevices.Item(i).refresh = refresh
                            _ListDevices.Item(i).solo = solo
                            _ListDevices.Item(i).LastChangeDuree = lastchangeduree
                            _ListDevices.Item(i).LastEtat = lastEtat
                            '_ListDevices.Item(i).Commandes = Commandes
                            _ListDevices.Item(i).Driver.newdevice(deviceId)
                            'si c'est un device de type double ou integer
                            If _ListDevices.Item(i).type = "VITESSEVENT" Or _ListDevices.Item(i).type = "UV" Or _ListDevices.Item(i).type = "TEMPERATURECONSIGNE" Or _ListDevices.Item(i).type = "TEMPERATURE" Or _ListDevices.Item(i).type = "PLUIETOTAL" Or _ListDevices.Item(i).type = "PLUIECOURANT" Or _ListDevices.Item(i).type = "LAMPE" Or _ListDevices.Item(i).type = "HUMIDITE" Or _ListDevices.Item(i).type = "GENERIQUEVALUE" Or _ListDevices.Item(i).type = "ENERGIETOTALE" Or _ListDevices.Item(i).type = "ENERGIEINSTANTANEE" Or _ListDevices.Item(i).type = "COMPTEUR" Or _ListDevices.Item(i).type = "BAROMETRE" Then
                                _ListDevices.Item(i).Correction = correction
                                _ListDevices.Item(i).Formatage = formatage
                                _ListDevices.Item(i).Precision = precision
                                _ListDevices.Item(i).ValueMax = valuemax
                                _ListDevices.Item(i).ValueMin = valuemin
                                _ListDevices.Item(i).ValueDef = valuedef
                            End If
                            Exit For 'on a trouvé le device, on arrete donc de le chercher.
                        End If
                    Next
                End If
                'génération de l'event
                Return myID
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SaveDevice", "Exception : " & ex.Message)
                Return "-1"
            End Try
        End Function

        ''' <summary>Supprime une commande IR d'un device</summary>
        ''' <param name="deviceId"></param>
        ''' <param name="CmdName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteDeviceCommandIR(ByVal IdSrv As String, ByVal deviceId As String, ByVal CmdName As String) As Integer Implements IHoMIDom.DeleteDeviceCommandIR
            If VerifIdSrv(IdSrv) = False Then
                Return 99
                Exit Function
            End If

            Try

                For i As Integer = 0 To _ListDevices.Count - 1
                    If _ListDevices.Item(i).Id = deviceId Then
                        For j As Integer = 0 To _ListDevices.Item(i).ListCommandname.count - 1
                            If _ListDevices.Item(i).ListCommandname(j) = CmdName Then
                                _ListDevices.Item(i).ListCommandname.removeat(j)
                                _ListDevices.Item(i).ListCommanddata.removeat(j)
                                _ListDevices.Item(i).ListCommandrepeat.removeat(j)
                                Return 0
                                'génération de l'event
                                Exit Function
                            End If
                        Next
                    End If
                Next
                Return -1
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeleteDeviceCommandIR", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>Ajoute ou modifie une commande IR à un device</summary>
        ''' <param name="deviceId"></param>
        ''' <param name="CmdName"></param>
        ''' <param name="CmdData"></param>
        ''' <param name="CmdRepeat"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SaveDeviceCommandIR(ByVal IdSrv As String, ByVal deviceId As String, ByVal CmdName As String, ByVal CmdData As String, ByVal CmdRepeat As String) As String Implements IHoMIDom.SaveDeviceCommandIR
            If VerifIdSrv(IdSrv) = False Then
                Return 99
                Exit Function
            End If

            Dim flag As Boolean
            Try
                'On vérifie avant que si la commande existe on la modifie
                For i As Integer = 0 To _ListDevices.Count - 1
                    If _ListDevices.Item(i).id = deviceId Then
                        For j As Integer = 0 To _ListDevices.Item(i).listcommandName.count - 1
                            If _ListDevices.Item(i).listcommandname(j) = CmdName Then
                                _ListDevices.Item(i).listcommanddata(j) = CmdData
                                _ListDevices.Item(i).listcommandrepeat(j) = CmdRepeat
                                flag = True
                            End If
                        Next
                        'sinon on la crée
                        If flag = False Then
                            _ListDevices.Item(i).listcommandname.add(CmdName)
                            _ListDevices.Item(i).listcommanddata.add(CmdData)
                            _ListDevices.Item(i).listcommandrepeat.add(CmdRepeat)
                        End If
                    End If
                Next
                Return 0
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SaveDeviceCommandIR", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>Commencer un apprentissage IR</summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function StartIrLearning(ByVal IdSrv As String) As String Implements IHoMIDom.StartIrLearning
            If VerifIdSrv(IdSrv) = False Then
                Return 99
                Exit Function
            End If

            Dim retour As String = ""
            Try
                For i As Integer = 0 To _ListDrivers.Count - 1
                    If _ListDrivers.Item(i).protocol = "IR" Then
                        Dim x As Object = _ListDrivers.Item(i)
                        retour = x.LearnCodeIR()
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "SERVEUR", "Apprentissage IR: " & retour)
                    End If
                Next
                Return retour
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "StartIrLearning", "Exception : " & ex.Message)
                Return ""
            End Try
        End Function

        ''' <summary>Retourne un device par son ID</summary>
        ''' <param name="DeviceId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ReturnDeviceById(ByVal IdSrv As String, ByVal DeviceId As String) As TemplateDevice Implements IHoMIDom.ReturnDeviceByID
            If VerifIdSrv(IdSrv) = False Then
                Return Nothing
                Exit Function
            End If

            Dim retour As New TemplateDevice
            Dim _listact As New List(Of String)

            Try
                For i As Integer = 0 To _ListDevices.Count - 1
                    If _ListDevices.Item(i).ID = DeviceId Then
                        retour.ID = _ListDevices.Item(i).id
                        retour.Name = _ListDevices.Item(i).name
                        retour.Enable = _ListDevices.Item(i).enable
                        retour.GetDeviceCommandePlus = _ListDevices.Item(i).GetCommandPlus
                        Select Case UCase(_ListDevices.Item(i).type)
                            Case "APPAREIL" : retour.Type = Device.ListeDevices.APPAREIL  'modules pour diriger un appareil  ON/OFF
                            Case "AUDIO" : retour.Type = Device.ListeDevices.AUDIO
                            Case "BAROMETRE" : retour.Type = Device.ListeDevices.BAROMETRE  'pour stocker les valeur issu d'un barometre meteo ou web
                            Case "BATTERIE" : retour.Type = Device.ListeDevices.BAROMETRE
                            Case "COMPTEUR" : retour.Type = Device.ListeDevices.COMPTEUR  'compteur DS2423, RFXPower...
                            Case "CONTACT" : retour.Type = Device.ListeDevices.CONTACT  'detecteur de contact : switch 1-wire
                            Case "DETECTEUR" : retour.Type = Device.ListeDevices.DETECTEUR  'tous detecteurs : mouvement, obscurite...
                            Case "DIRECTIONVENT" : retour.Type = Device.ListeDevices.DIRECTIONVENT
                            Case "ENERGIEINSTANTANEE" : retour.Type = Device.ListeDevices.ENERGIEINSTANTANEE
                            Case "ENERGIETOTALE" : retour.Type = Device.ListeDevices.ENERGIETOTALE
                            Case "FREEBOX" : retour.Type = Device.ListeDevices.FREEBOX
                            Case "GENERIQUEBOOLEEN" : retour.Type = Device.ListeDevices.GENERIQUEBOOLEEN
                            Case "GENERIQUESTRING" : retour.Type = Device.ListeDevices.GENERIQUESTRING
                            Case "GENERIQUEVALUE" : retour.Type = Device.ListeDevices.GENERIQUEVALUE
                            Case "HUMIDITE" : retour.Type = Device.ListeDevices.HUMIDITE
                            Case "LAMPE" : retour.Type = Device.ListeDevices.LAMPE
                            Case "METEO" : retour.Type = Device.ListeDevices.METEO
                            Case "MULTIMEDIA" : retour.Type = Device.ListeDevices.MULTIMEDIA
                            Case "PLUIECOURANT" : retour.Type = Device.ListeDevices.PLUIECOURANT
                            Case "PLUIETOTAL" : retour.Type = Device.ListeDevices.PLUIETOTAL
                            Case "SWITCH" : retour.Type = Device.ListeDevices.SWITCH
                            Case "TELECOMMANDE" : retour.Type = Device.ListeDevices.TELECOMMANDE
                            Case "TEMPERATURE" : retour.Type = Device.ListeDevices.TEMPERATURE
                            Case "TEMPERATURECONSIGNE" : retour.Type = Device.ListeDevices.TEMPERATURECONSIGNE
                            Case "UV" : retour.Type = Device.ListeDevices.UV
                            Case "VITESSEVENT" : retour.Type = Device.ListeDevices.VITESSEVENT
                            Case "VOLET" : retour.Type = Device.ListeDevices.VOLET
                        End Select
                        retour.Description = _ListDevices.Item(i).description
                        retour.Adresse1 = _ListDevices.Item(i).adresse1
                        retour.Adresse2 = _ListDevices.Item(i).adresse2
                        retour.DriverID = _ListDevices.Item(i).driverid
                        retour.Picture = _ListDevices.Item(i).picture
                        retour.Solo = _ListDevices.Item(i).solo
                        retour.Refresh = _ListDevices.Item(i).refresh
                        retour.Modele = _ListDevices.Item(i).modele
                        retour.LastEtat = _ListDevices.Item(i).LastEtat

                        _listact = ListMethod(_ListDevices.Item(i).id)
                        _listact = _listact
                        If _listact.Count > 0 Then
                            For n As Integer = 0 To _listact.Count - 1
                                Dim a() As String = _listact.Item(n).Split("|")
                                Dim p As New DeviceAction
                                With p
                                    .Nom = a(0)
                                    If a.Length > 1 Then
                                        For t As Integer = 1 To a.Length - 1
                                            Dim pr As New DeviceAction.Parametre
                                            Dim b() As String = a(t).Split(":")
                                            With pr
                                                .Nom = b(0)
                                                .Type = b(1)
                                            End With
                                            p.Parametres.Add(pr)
                                        Next
                                    End If
                                End With
                                retour.DeviceAction.Add(p)
                            Next
                        End If

                        If retour.Type = Device.ListeDevices.BAROMETRE _
                                        Or retour.Type = Device.ListeDevices.COMPTEUR _
                                        Or retour.Type = Device.ListeDevices.ENERGIEINSTANTANEE _
                                        Or retour.Type = Device.ListeDevices.ENERGIETOTALE _
                                        Or retour.Type = Device.ListeDevices.GENERIQUEVALUE _
                                        Or retour.Type = Device.ListeDevices.HUMIDITE _
                                        Or retour.Type = Device.ListeDevices.PLUIECOURANT _
                                        Or retour.Type = Device.ListeDevices.PLUIETOTAL _
                                        Or retour.Type = Device.ListeDevices.TEMPERATURE _
                                        Or retour.Type = Device.ListeDevices.TEMPERATURECONSIGNE _
                                        Or retour.Type = Device.ListeDevices.VITESSEVENT _
                                        Or retour.Type = Device.ListeDevices.UV _
                                        Or retour.Type = Device.ListeDevices.VITESSEVENT _
                                        Then
                            retour.Correction = _ListDevices.Item(i).correction
                            retour.Precision = _ListDevices.Item(i).precision
                            retour.Formatage = _ListDevices.Item(i).formatage
                            retour.Value = _ListDevices.Item(i).value
                            retour.ValueDef = _ListDevices.Item(i).valuedef
                            retour.ValueLast = _ListDevices.Item(i).valuelast
                            retour.ValueMax = _ListDevices.Item(i).valuemax
                            retour.ValueMin = _ListDevices.Item(i).valuemin
                        End If

                        If retour.Type <> Device.ListeDevices.METEO And retour.Type <> Device.ListeDevices.MULTIMEDIA And retour.Type <> Device.ListeDevices.FREEBOX Then
                            retour.Value = _ListDevices.Item(i).value
                        End If

                        If retour.Type = Device.ListeDevices.METEO Then
                            retour.ConditionActuel = _ListDevices.Item(i).ConditionActuel
                            retour.ConditionJ1 = _ListDevices.Item(i).ConditionJ1
                            retour.ConditionJ2 = _ListDevices.Item(i).ConditionActuel
                            retour.ConditionJ3 = _ListDevices.Item(i).ConditionJ3
                            retour.ConditionToday = _ListDevices.Item(i).ConditionToday
                            retour.HumiditeActuel = _ListDevices.Item(i).HumiditeActuel
                            retour.IconActuel = _ListDevices.Item(i).IconActuel
                            retour.IconJ1 = _ListDevices.Item(i).IconJ1
                            retour.IconJ2 = _ListDevices.Item(i).IconJ2
                            retour.IconJ3 = _ListDevices.Item(i).IconJ3
                            retour.IconToday = _ListDevices.Item(i).IconToday
                            retour.JourJ1 = _ListDevices.Item(i).JourJ1
                            retour.JourJ2 = _ListDevices.Item(i).JourJ2
                            retour.JourJ3 = _ListDevices.Item(i).JourJ3
                            retour.JourToday = _ListDevices.Item(i).JourToday
                            retour.MaxJ1 = _ListDevices.Item(i).MaxJ1
                            retour.MaxJ2 = _ListDevices.Item(i).MaxJ2
                            retour.MaxJ3 = _ListDevices.Item(i).MaxJ3
                            retour.MaxToday = _ListDevices.Item(i).MaxToday
                            retour.MinJ1 = _ListDevices.Item(i).MinJ1
                            retour.MinJ2 = _ListDevices.Item(i).MinJ2
                            retour.MinJ3 = _ListDevices.Item(i).MinJ3
                            retour.MinToday = _ListDevices.Item(i).MinToday
                            retour.TemperatureActuel = _ListDevices.Item(i).TemperatureActuel
                            retour.VentActuel = _ListDevices.Item(i).VentActuel
                        End If

                        If retour.Type = Device.ListeDevices.MULTIMEDIA Then
                            retour.Commandes = _ListDevices.Item(i).Commandes
                            'For j As Integer = 0 To _ListDevices.Item(i).listcommandname.count - 1
                            '    retour.ListCommandName.Add(_ListDevices.Item(i).listcommandname.item(j))
                            '    retour.ListCommandData.Add(_ListDevices.Item(i).ListCommandData.item(j))
                            '    retour.ListCommandRepeat.Add(_ListDevices.Item(i).ListCommandRepeat.item(j))
                            'Next
                        End If
                        Exit For
                    End If
                Next

                If retour.ID <> "" Then
                    Return retour
                Else
                    Return Nothing
                End If

            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "ReturnDeviceById", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function

        ''' <summary>Retourne un device par son ID</summary>
        ''' <param name="DeviceId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ReturnRealDeviceById(ByVal DeviceId As String) As Object
            Try
                For i As Integer = 0 To _ListDevices.Count - 1
                    If _ListDevices.Item(i).ID = DeviceId Then
                        Return _ListDevices.Item(i)
                        Exit For
                    End If
                Next
                Return Nothing
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "ReturnRealDeviceById", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function

        ''' <summary>liste les méthodes d'un device depuis son ID</summary>
        ''' <param name="DeviceId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function ListMethod(ByVal DeviceId As String) As List(Of String) Implements IHoMIDom.ListMethod
            Dim _list As New List(Of String)
            Try
                For i As Integer = 0 To _ListDevices.Count - 1
                    If _ListDevices.Item(i).ID = DeviceId Then
                        For j As Integer = 0 To Api.ListMethod(_ListDevices.Item(i)).Count - 1
                            _list.Add(Api.ListMethod(_ListDevices.Item(i)).Item(j).ToString)
                        Next
                    End If
                Next
                Return _list
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "ListMethod", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function

        ''' <summary>Retourne une liste de device par son Adresse1 et/ou type et/ou son driver, ex: "A1" "TEMPERATURE" "RFXCOM_RECEIVER"</summary>
        ''' <param name="DeviceAdresse"></param>
        ''' <param name="DeviceType"></param>
        ''' <param name="DriverID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ReturnDeviceByAdresse1TypeDriver(ByVal IdSrv As String, ByVal DeviceAdresse As String, ByVal DeviceType As String, ByVal DriverID As String, ByVal Enable As Boolean) As ArrayList Implements IHoMIDom.ReturnDeviceByAdresse1TypeDriver
            If VerifIdSrv(IdSrv) = False Then
                Return Nothing
                Exit Function
            End If

            Try
                Dim retour As Object = Nothing
                Dim listresultat As New ArrayList
                For i As Integer = 0 To _ListDevices.Count - 1
                    If (DeviceAdresse = "" Or _ListDevices.Item(i).Adresse1 = DeviceAdresse.ToUpper()) And (DeviceType = "" Or _ListDevices.Item(i).type = DeviceType.ToUpper()) And (DriverID = "" Or _ListDevices.Item(i).DriverID = DriverID.ToUpper()) And _ListDevices.Item(i).Enable = Enable Then
                        retour = _ListDevices.Item(i)
                        listresultat.Add(retour)
                        retour = Nothing
                    End If
                Next
                Return listresultat
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "ReturnDeviceByAdresse1TypeDriver", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function

        ''' <summary>Permet d'exécuter une commande Sub d'un Device</summary>
        ''' <param name="DeviceId"></param>
        ''' <param name="Action"></param>
        ''' <remarks></remarks>
        Sub ExecuteDeviceCommand(ByVal IdSrv As String, ByVal DeviceId As String, ByVal Action As DeviceAction) Implements IHoMIDom.ExecuteDeviceCommand
            If VerifIdSrv(IdSrv) = False Then
                Exit Sub
            End If

            Dim _retour As Object
            Dim x As Object = Nothing

            Try
                For i As Integer = 0 To _ListDevices.Count - 1
                    If _ListDevices.Item(i).ID = DeviceId Then
                        x = _ListDevices.Item(i)
                        Exit For
                    End If
                Next

                If x IsNot Nothing Then

                    If Action.Parametres IsNot Nothing Then
                        If Action.Parametres.Count > 0 Then
                            Select Case Action.Parametres.Count
                                Case 1
                                    _retour = CallByName(x, Action.Nom, CallType.Method, Action.Parametres.Item(0).Value)
                                    Log(Server.TypeLog.INFO, Server.TypeSource.SERVEUR, "ExecuteDevicecommand", "ExecuteDeviceCommand effectué: " & x.Name & " Command: " & Action.Nom & " Parametre: " & Action.Parametres.Item(0).Value)
                                Case 2
                                    _retour = CallByName(x, Action.Nom, CallType.Method, Action.Parametres.Item(0).Value, Action.Parametres.Item(1).Value)
                                Case 3
                                    _retour = CallByName(x, Action.Nom, CallType.Method, Action.Parametres.Item(0).Value, Action.Parametres.Item(1).Value, Action.Parametres.Item(2).Value)
                                Case 4
                                    _retour = CallByName(x, Action.Nom, CallType.Method, Action.Parametres.Item(0).Value, Action.Parametres.Item(1).Value, Action.Parametres.Item(2).Value, Action.Parametres.Item(3).Value)
                                Case 5
                                    _retour = CallByName(x, Action.Nom, CallType.Method, Action.Parametres.Item(0).Value, Action.Parametres.Item(1).Value, Action.Parametres.Item(2).Value, Action.Parametres.Item(3).Value, Action.Parametres.Item(4).Value)
                            End Select
                        Else
                            CallByName(x, Action.Nom, CallType.Method)
                            Log(Server.TypeLog.INFO, Server.TypeSource.SERVEUR, "ExecuteDevicecommand", "ExecuteDeviceCommand effectué: " & x.Name & " Command: " & Action.Nom)
                        End If
                    Else
                        CallByName(x, Action.Nom, CallType.Method)
                        Log(Server.TypeLog.INFO, Server.TypeSource.SERVEUR, "ExecuteDevicecommand", "ExecuteDeviceCommand effectué: " & x.Name & " Command: " & Action.Nom & " Aucun paramètre")
                    End If
                Else
                    Log(Server.TypeLog.INFO, Server.TypeSource.SERVEUR, "ExecuteDevicecommand", "ExecuteDeviceCommand non effectué car le device est null: " & x.Name & " Command: " & Action.Nom)
                End If
            Catch ex As Exception
                Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "ExecuteDevicecommand", "Erreur lors du traitemant du Sub ExecuteDeviceCommand: " & ex.ToString)
            End Try
        End Sub

#End Region

#Region "Zone"
        ''' <summary>Supprimer une zone de la config</summary>
        ''' <param name="zoneId"></param>
        Public Function DeleteZone(ByVal IdSrv As String, ByVal zoneId As String) As Integer Implements IHoMIDom.DeleteZone
            If VerifIdSrv(IdSrv) = False Then
                Return 99
                Exit Function
            End If

            Try
                For i As Integer = 0 To _ListZones.Count - 1
                    If _ListZones.Item(i).ID = zoneId Then
                        _ListZones.RemoveAt(i)
                        DeleteZone = 0
                        Exit Function
                    End If
                Next
                Return -1
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeleteZone", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>Retourne la liste de toutes les zones</summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetAllZones(ByVal IdSrv As String) As List(Of Zone) Implements IHoMIDom.GetAllZones
            If VerifIdSrv(IdSrv) = False Then
                Return Nothing
                Exit Function
            End If

            Try
                Dim _list As New List(Of Zone)
                For i As Integer = 0 To _ListZones.Count - 1
                    Dim x As New Zone
                    With x
                        .Name = _ListZones.Item(i).Name
                        .ID = _ListZones.Item(i).ID
                        .Icon = _ListZones.Item(i).Icon
                        .Image = _ListZones.Item(i).Image
                        For j As Integer = 0 To _ListZones.Item(i).ListElement.Count - 1
                            .ListElement.Add(_ListZones.Item(i).ListElement.Item(j))
                        Next
                    End With
                    _list.Add(x)
                    x = Nothing
                Next
                _list.Sort(AddressOf sortZone)
                Return _list
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetAllZones", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function

        Private Function sortZone(ByVal x As Zone, ByVal y As Zone) As Integer
            Return x.Name.CompareTo(y.Name)
        End Function

        ''' <summary>ajouter un device à une zone</summary>
        ''' <param name="ZoneId"></param>
        ''' <param name="DeviceId"></param>
        ''' <param name="Visible"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function AddDeviceToZone(ByVal IdSrv As String, ByVal ZoneId As String, ByVal DeviceId As String, ByVal Visible As Boolean) As String Implements IHoMIDom.AddDeviceToZone
            If VerifIdSrv(IdSrv) = False Then
                Return 99
                Exit Function
            End If

            Dim _zone As Zone = ReturnZoneById(_IdSrv, ZoneId)
            Dim _retour As String = -1
            Try
                If _zone IsNot Nothing Then
                    For i As Integer = 0 To _zone.ListElement.Count - 1
                        If _zone.ListElement.Item(i).ElementID = DeviceId Then
                            _zone.ListElement.Item(i).Visible = Visible
                            Return 0
                            Exit Function
                        End If
                    Next
                    Dim _dev As New Zone.Element_Zone(DeviceId, Visible)
                    _zone.ListElement.Add(_dev)
                    _retour = 0
                End If
                Return _retour
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "AddDeviceToZone", "Exception : " & ex.Message)
                Return "-1"
            End Try
        End Function

        ''' <summary>supprimer un device à une zone</summary>
        ''' <param name="ZoneId"></param>
        ''' <param name="DeviceId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function DeleteDeviceToZone(ByVal IdSrv As String, ByVal ZoneId As String, ByVal DeviceId As String) As String Implements IHoMIDom.DeleteDeviceToZone
            If VerifIdSrv(IdSrv) = False Then
                Return 99
                Exit Function
            End If

            Dim _zone As Zone = ReturnZoneById(_IdSrv, ZoneId)
            Dim _retour As String = -1
            Try
                If _zone IsNot Nothing Then
                    For i As Integer = 0 To _zone.ListElement.Count - 1
                        If _zone.ListElement.Item(i).ElementID = DeviceId Then
                            _zone.ListElement.RemoveAt(i)
                            Exit For
                        End If
                    Next
                    _retour = 0
                End If
                Return _retour
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeleteDeviceToZone", "Exception : " & ex.Message)
                Return "-1"
            End Try
        End Function

        ''' <summary>sauvegarde ou créer une zone dans la config</summary>
        ''' <param name="zoneId"></param>
        ''' <param name="name"></param>
        ''' <param name="ListElement"></param>
        ''' <param name="icon"></param>
        ''' <param name="image"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function SaveZone(ByVal IdSrv As String, ByVal zoneId As String, ByVal name As String, Optional ByVal ListElement As List(Of Zone.Element_Zone) = Nothing, Optional ByVal icon As String = "", Optional ByVal image As String = "") As String Implements IHoMIDom.SaveZone
            If VerifIdSrv(IdSrv) = False Then
                Return 99
                Exit Function
            End If

            Dim myID As String = ""

            If icon = "" Then icon = _MonRepertoire & "\images\icones\Zone_128.png"
            If image = "" Then image = _MonRepertoire & "\images\icones\Zone_Image.png"

            Try
                If zoneId = "" Then
                    Dim x As New Zone
                    With x
                        x.ID = GenerateGUID()
                        x.Name = name
                        x.Icon = icon
                        x.Image = image
                        x.ListElement = ListElement
                    End With
                    myID = x.ID
                    _ListZones.Add(x)
                Else
                    'zone Existante
                    myID = zoneId
                    For i As Integer = 0 To _ListZones.Count - 1
                        If _ListZones.Item(i).ID = zoneId Then
                            _ListZones.Item(i).Name = name
                            _ListZones.Item(i).Icon = icon
                            _ListZones.Item(i).Image = image
                            _ListZones.Item(i).ListElement = ListElement
                        End If
                    Next
                End If
                'génération de l'event
                Return myID
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SaveZone", "Exception : " & ex.Message)
                Return ""
            End Try
        End Function

        ''' <summary>Retourne la liste des devices d'une zone depuis son ID</summary>
        ''' <param name="ZoneId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetDeviceInZone(ByVal IdSrv As String, ByVal zoneId As String) As List(Of TemplateDevice) Implements IHoMIDom.GetDeviceInZone
            If VerifIdSrv(IdSrv) = False Then
                Return Nothing
                Exit Function
            End If

            Try
                Dim x As Zone = ReturnZoneById(_IdSrv, zoneId)
                Dim y As New List(Of TemplateDevice)
                If x IsNot Nothing Then
                    If x.ListElement.Count > 0 Then
                        For i As Integer = 0 To x.ListElement.Count - 1
                            y.Add(ReturnDeviceById(_IdSrv, x.ListElement.Item(i).ElementID))
                        Next
                    End If
                End If
                Return y
                y = Nothing
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetDeviceInZone", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Indique si la zone ne contient aucun device
        ''' </summary>
        ''' <param name="zoneId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ZoneIsEmpty(ByVal IdSrv As String, ByVal zoneId As String) As Boolean Implements IHoMIDom.ZoneIsEmpty
            If VerifIdSrv(IdSrv) = False Then
                Return False
                Exit Function
            End If

            Dim retour As Boolean = True
            Dim x As Zone = ReturnZoneById(_IdSrv, zoneId)
            If x IsNot Nothing Then
                If x.ListElement.Count > 0 Then
                    retour = False
                End If
            End If
            Return retour
        End Function

        ''' <summary>Retourne la zone par son ID</summary>
        ''' <param name="ZoneId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ReturnZoneById(ByVal IdSrv As String, ByVal ZoneId As String) As Zone Implements IHoMIDom.ReturnZoneByID
            If VerifIdSrv(IdSrv) = False Then
                Return Nothing
                Exit Function
            End If

            Try
                If (From Zone In _ListZones Where Zone.ID = ZoneId Select Zone).Count > 0 Then
                    Dim Resultat = (From Zone In _ListZones Where Zone.ID = ZoneId Select Zone).First
                    Return Resultat
                Else
                    Return Nothing
                End If
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "ReturnZoneById", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function


#End Region

#Region "Macro"
        ''' <summary>Supprimer une macro de la config</summary>
        ''' <param name="macroId"></param>
        Public Function DeleteMacro(ByVal IdSrv As String, ByVal macroId As String) As Integer Implements IHoMIDom.DeleteMacro
            If VerifIdSrv(IdSrv) = False Then
                Return 99
                Exit Function
            End If

            Try
                For i As Integer = 0 To _ListMacros.Count - 1
                    If _ListMacros.Item(i).ID = macroId Then
                        _ListMacros.RemoveAt(i)
                        DeleteMacro = 0
                        Exit Function
                    End If
                Next
                Return -1
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeleteMacro", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>Retourne la liste de toutes les macros</summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetAllMacros(ByVal IdSrv As String) As List(Of Macro) Implements IHoMIDom.GetAllMacros
            If VerifIdSrv(IdSrv) = False Then
                Return Nothing
                Exit Function
            End If

            Try
                Dim _list As New List(Of Macro)
                For i As Integer = 0 To _ListMacros.Count - 1
                    Dim x As New Macro
                    With x
                        .Nom = _ListMacros.Item(i).Nom
                        .ID = _ListMacros.Item(i).ID
                        .Description = _ListMacros.Item(i).Description
                        .Enable = _ListMacros.Item(i).Enable
                        .ListActions = _ListMacros.Item(i).ListActions
                    End With
                    _list.Add(x)
                Next
                _list.Sort(AddressOf sortMacro)
                Return _list
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetAllMacros", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function

        Private Function sortMacro(ByVal x As Macro, ByVal y As Macro) As Integer
            Return x.Nom.CompareTo(y.Nom)
        End Function

        ''' <summary>
        ''' Permet de créer ou modifier une macro
        ''' </summary>
        ''' <param name="macroId"></param>
        ''' <param name="nom"></param>
        ''' <param name="enable"></param>
        ''' <param name="description"></param>
        ''' <param name="listactions"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SaveMacro(ByVal IdSrv As String, ByVal macroId As String, ByVal nom As String, ByVal enable As Boolean, Optional ByVal description As String = "", Optional ByVal listactions As ArrayList = Nothing) As String Implements IHoMIDom.SaveMacro
            If VerifIdSrv(IdSrv) = False Then
                Return 99
                Exit Function
            End If

            Dim myID As String = ""
            Try
                If macroId = "" Then
                    Dim x As New Macro
                    With x
                        x._Server = Me
                        x.ID = GenerateGUID()
                        x.Nom = nom
                        x.Enable = enable
                        x.Description = description
                        x.ListActions = listactions
                    End With
                    myID = x.ID
                    _ListMacros.Add(x)
                Else
                    'macro Existante
                    myID = macroId
                    For i As Integer = 0 To _ListMacros.Count - 1
                        If _ListMacros.Item(i).ID = macroId Then
                            _ListMacros.Item(i).Nom = nom
                            _ListMacros.Item(i).Enable = enable
                            _ListMacros.Item(i).Description = description
                            _ListMacros.Item(i).ListActions = listactions
                        End If
                    Next
                End If
                'génération de l'event
                Return myID
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SaveMacro", "Exception : " & ex.Message)
                Return "-1"
            End Try
        End Function

        ''' <summary>Retourne la macro par son ID</summary>
        ''' <param name="MacroId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ReturnMacroById(ByVal IdSrv As String, ByVal MacroId As String) As Macro Implements IHoMIDom.ReturnMacroById
            If VerifIdSrv(IdSrv) = False Then
                Return Nothing
                Exit Function
            End If

            Try
                If (From Macro In _ListMacros Where Macro.ID = MacroId Select Macro).Count > 0 Then
                    Dim Resultat = (From Macro In _ListMacros Where Macro.ID = MacroId Select Macro).First
                    Return Resultat
                Else
                    Return Nothing
                End If
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "ReturnMacroById", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function

        Public Sub RunMacro(ByVal IDSrv As String, ByVal Id As String) Implements IHoMIDom.RunMacro
            If VerifIdSrv(IDSrv) = False Then
                Exit Sub
            End If

            Execute(Id)
        End Sub
#End Region

#Region "Trigger"
        ''' <summary>Supprimer un trigger de la config</summary>
        ''' <param name="triggerId"></param>
        Public Function DeleteTrigger(ByVal IdSrv As String, ByVal triggerId As String) As Integer Implements IHoMIDom.DeleteTrigger
            If VerifIdSrv(IdSrv) = False Then
                Return 99
                Exit Function
            End If

            Try
                For i As Integer = 0 To _ListTriggers.Count - 1
                    If _ListTriggers.Item(i).ID = triggerId Then
                        _ListTriggers.RemoveAt(i)
                        DeleteTrigger = 0
                        Exit Function
                    End If
                Next
                Return -1
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeleteTrigger", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>Retourne la liste de toutes les macros</summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetAllTriggers(ByVal IdSrv As String) As List(Of Trigger) Implements IHoMIDom.GetAllTriggers
            If VerifIdSrv(IdSrv) = False Then
                Return Nothing
                Exit Function
            End If

            Try
                Dim _list As New List(Of Trigger)
                For i As Integer = 0 To _ListTriggers.Count - 1
                    Dim x As New Trigger
                    With x
                        .Nom = _ListTriggers.Item(i).Nom
                        .ID = _ListTriggers.Item(i).ID
                        .Description = _ListTriggers.Item(i).Description
                        .Enable = _ListTriggers.Item(i).Enable
                        .Prochainedateheure = _ListTriggers.Item(i).Prochainedateheure
                        .Type = _ListTriggers.Item(i).Type
                        .ConditionTime = _ListTriggers.Item(i).ConditionTime
                        .ConditionDeviceId = _ListTriggers.Item(i).ConditionDeviceId
                        .ConditionDeviceProperty = _ListTriggers.Item(i).ConditionDeviceProperty
                        .ListMacro = _ListTriggers.Item(i).ListMacro
                    End With
                    _list.Add(x)
                Next
                _list.Sort(AddressOf sortTrigger)
                Return _list
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetAllTriggers", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function

        Private Function sortTrigger(ByVal x As Trigger, ByVal y As Trigger) As Integer
            Return x.Nom.CompareTo(y.Nom)
        End Function

        ''' <summary>
        ''' Permet de créer ou modifier un trigger
        ''' </summary>
        ''' <param name="triggerId"></param>
        ''' <param name="nom"></param>
        ''' <param name="enable"></param>
        ''' <param name="description"></param>
        ''' <param name="conditiontimer"></param>
        ''' <param name="deviceid"></param>
        ''' <param name="deviceproperty"></param>
        ''' <param name="TypeTrigger"></param>
        ''' <param name="macro"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SaveTrigger(ByVal IdSrv As String, ByVal triggerId As String, ByVal nom As String, ByVal enable As Boolean, ByVal TypeTrigger As Trigger.TypeTrigger, Optional ByVal description As String = "", Optional ByVal conditiontimer As String = "", Optional ByVal deviceid As String = "", Optional ByVal deviceproperty As String = "", Optional ByVal macro As List(Of String) = Nothing) As String Implements IHoMIDom.SaveTrigger
            If VerifIdSrv(IdSrv) = False Then
                Return 99
                Exit Function
            End If

            Dim myID As String = ""
            Try
                If triggerId = "" Then
                    Dim x As New Trigger
                    With x
                        x._Server = Me
                        x.ID = GenerateGUID()
                        x.Nom = nom
                        x.Enable = enable
                        Select Case TypeTrigger
                            Case Trigger.TypeTrigger.TIMER
                                x.Type = Trigger.TypeTrigger.TIMER
                                x.ConditionTime = conditiontimer
                            Case Trigger.TypeTrigger.DEVICE
                                x.Type = Trigger.TypeTrigger.DEVICE
                                x.ConditionDeviceId = deviceid
                                x.ConditionDeviceProperty = deviceproperty
                        End Select
                        If description <> "" Then x.Description = description
                        If macro IsNot Nothing Then
                            If macro.Count > 0 Then
                                x.ListMacro = macro
                            End If
                        End If

                    End With
                    myID = x.ID
                    _ListTriggers.Add(x)
                    x = Nothing
                Else
                    'trigger Existante
                    myID = triggerId
                    For i As Integer = 0 To _ListTriggers.Count - 1
                        If _ListTriggers.Item(i).ID = triggerId Then
                            _ListTriggers.Item(i).Nom = nom
                            _ListTriggers.Item(i).Enable = enable
                            _ListTriggers.Item(i).Description = description
                            Select Case TypeTrigger
                                Case Trigger.TypeTrigger.TIMER
                                    _ListTriggers.Item(i).Type = HoMIDom.Trigger.TypeTrigger.TIMER
                                    _ListTriggers.Item(i).ConditionTime = conditiontimer
                                Case Trigger.TypeTrigger.DEVICE
                                    _ListTriggers.Item(i).Type = HoMIDom.Trigger.TypeTrigger.DEVICE
                                    _ListTriggers.Item(i).ConditionDeviceId = deviceid
                                    _ListTriggers.Item(i).ConditionDeviceProperty = deviceproperty
                            End Select
                            If macro IsNot Nothing Then _ListTriggers.Item(i).ListMacro = macro
                        End If
                    Next
                End If
                'génération de l'event
                Return myID
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SaveTrigger", "Exception : " & ex.Message & vbCrLf & ex.ToString)
                Return "-1"
            End Try
        End Function

        ''' <summary>Retourne le trigger par son ID</summary>
        ''' <param name="TriggerId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ReturnTriggerById(ByVal IdSrv As String, ByVal TriggerId As String) As Trigger Implements IHoMIDom.ReturnTriggerById
            If VerifIdSrv(IdSrv) = False Then
                Return Nothing
                Exit Function
            End If

            Try
                Dim Resultat = (From Trigger In _ListTriggers Where Trigger.ID = TriggerId Select Trigger).First
                Return Resultat

            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "ReturnTriggerById", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function

#End Region

#Region "User"
        ''' <summary>Supprime un user</summary>
        ''' <param name="userId"></param>
        Public Function DeleteUser(ByVal IdSrv As String, ByVal userId As String) As Integer Implements IHoMIDom.DeleteUser
            If VerifIdSrv(IdSrv) = False Then
                Return 99
                Exit Function
            End If

            Try
                For i As Integer = 0 To _ListUsers.Count - 1
                    If _ListUsers.Item(i).ID = userId Then
                        _ListUsers.RemoveAt(i)
                        DeleteUser = 0
                        Exit Function
                    End If
                Next
                Return -1
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeleteUser", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>Retourne la liste de tous les users</summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetAllUsers(ByVal IdSrv As String) As List(Of Users.User) Implements IHoMIDom.GetAllUsers
            If VerifIdSrv(IdSrv) = False Then
                Return Nothing
                Exit Function
            End If

            Dim _list As New List(Of Users.User)
            Try
                For i As Integer = 0 To _ListUsers.Count - 1
                    Dim x As New Users.User
                    With x
                        .Adresse = _ListUsers.Item(i).Adresse
                        .CodePostal = _ListUsers.Item(i).CodePostal
                        .eMail = _ListUsers.Item(i).eMail
                        .eMailAutre = _ListUsers.Item(i).eMailAutre
                        .ID = _ListUsers.Item(i).ID
                        .Image = _ListUsers.Item(i).Image
                        .Nom = _ListUsers.Item(i).Nom
                        .NumberIdentification = _ListUsers.Item(i).NumberIdentification
                        .Password = _ListUsers.Item(i).Password
                        .Prenom = _ListUsers.Item(i).Prenom
                        .Profil = _ListUsers.Item(i).Profil
                        .TelAutre = _ListUsers.Item(i).TelAutre
                        .TelFixe = _ListUsers.Item(i).TelFixe
                        .TelMobile = _ListUsers.Item(i).TelMobile
                        .UserName = _ListUsers.Item(i).UserName
                        .Ville = _ListUsers.Item(i).Ville
                    End With
                    _list.Add(x)
                Next
                _list.Sort(AddressOf sortUser)
                Return _list
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetAllUsers", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function

        Private Function sortUser(ByVal x As Users.User, ByVal y As Users.User) As Integer
            Return x.UserName.CompareTo(y.UserName)
        End Function

        ''' <summary>
        ''' Retourne un user par son username
        ''' </summary>
        ''' <param name="Username"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ReturnUserByUsername(ByVal IdSrv As String, ByVal Username As String) As Users.User Implements IHoMIDom.ReturnUserByUsername
            If VerifIdSrv(IdSrv) = False Then
                Return Nothing
                Exit Function
            End If

            Dim Resultat = (From User In _ListUsers Where User.UserName = Username Select User).First
            Return Resultat
        End Function

        ''' <summary>
        ''' Créer ou modifie un user par son ID
        ''' </summary>
        ''' <param name="userId"></param>
        ''' <param name="UserName"></param>
        ''' <param name="Password"></param>
        ''' <param name="Profil"></param>
        ''' <param name="Nom"></param>
        ''' <param name="Prenom"></param>
        ''' <param name="NumberIdentification"></param>
        ''' <param name="Image"></param>
        ''' <param name="eMail"></param>
        ''' <param name="eMailAutre"></param>
        ''' <param name="TelFixe"></param>
        ''' <param name="TelMobile"></param>
        ''' <param name="TelAutre"></param>
        ''' <param name="Adresse"></param>
        ''' <param name="Ville"></param>
        ''' <param name="CodePostal"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function SaveUser(ByVal IdSrv As String, ByVal userId As String, ByVal UserName As String, ByVal Password As String, ByVal Profil As Users.TypeProfil, ByVal Nom As String, ByVal Prenom As String, Optional ByVal NumberIdentification As String = "", Optional ByVal Image As String = "", Optional ByVal eMail As String = "", Optional ByVal eMailAutre As String = "", Optional ByVal TelFixe As String = "", Optional ByVal TelMobile As String = "", Optional ByVal TelAutre As String = "", Optional ByVal Adresse As String = "", Optional ByVal Ville As String = "", Optional ByVal CodePostal As String = "") As String Implements IHoMIDom.SaveUser
            If VerifIdSrv(IdSrv) = False Then
                Return 99
                Exit Function
            End If

            Dim myID As String = ""
            Try
                If userId = "" Then
                    For i As Integer = 0 To _ListUsers.Count - 1
                        If _ListUsers.Item(i).UserName = UserName Then
                            myID = "ERROR Username déjà utlisé"
                            Return myID
                        End If
                    Next
                    Dim x As New Users.User
                    With x
                        x.ID = GenerateGUID()
                        x.Adresse = Adresse
                        x.CodePostal = CodePostal
                        x.eMail = eMail
                        x.eMailAutre = eMailAutre
                        x.Image = Image
                        x.Nom = Nom
                        x.NumberIdentification = NumberIdentification
                        x.Password = EncryptTripleDES(Password, "homidom")
                        x.Prenom = Prenom
                        x.Profil = Profil
                        x.TelAutre = TelAutre
                        x.TelFixe = TelFixe
                        x.TelMobile = TelMobile
                        x.UserName = UserName
                        x.Ville = Ville
                    End With
                    myID = x.ID
                    _ListUsers.Add(x)
                Else
                    'user Existant
                    myID = userId
                    For i As Integer = 0 To _ListUsers.Count - 1
                        If _ListUsers.Item(i).ID = userId Then
                            _ListUsers.Item(i).Adresse = Adresse
                            _ListUsers.Item(i).CodePostal = CodePostal
                            _ListUsers.Item(i).eMail = eMail
                            _ListUsers.Item(i).eMailAutre = eMailAutre
                            _ListUsers.Item(i).Image = Image
                            _ListUsers.Item(i).Nom = Nom
                            _ListUsers.Item(i).NumberIdentification = NumberIdentification
                            _ListUsers.Item(i).Password = EncryptTripleDES(Password, "homidom")
                            _ListUsers.Item(i).Prenom = Prenom
                            _ListUsers.Item(i).Profil = Profil
                            _ListUsers.Item(i).TelAutre = TelAutre
                            _ListUsers.Item(i).TelFixe = TelFixe
                            _ListUsers.Item(i).TelMobile = TelMobile
                            _ListUsers.Item(i).UserName = UserName
                            _ListUsers.Item(i).Ville = Ville
                        End If
                    Next
                End If

                'génération de l'event
                Return myID
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SaveUser", "Exception : " & ex.Message)
                Return "-1"
            End Try
        End Function

        ''' <summary>Vérifie le couple username password</summary>
        ''' <param name="Username"></param>
        ''' <param name="Password"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function VerifLogin(ByVal Username As String, ByVal Password As String) As Boolean Implements IHoMIDom.VerifLogin
            Dim retour As Boolean = False
            Try
                For i As Integer = 0 To _ListUsers.Count - 1
                    If _ListUsers.Item(i).UserName = Username Then
                        Dim a As String = EncryptTripleDES(Password, "homidom")
                        If a = _ListUsers.Item(i).Password Then
                            Return True
                            Exit For
                        End If
                    End If
                Next
                Return retour
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "VerifLogin", "Exception : " & ex.Message)
                Return False
            End Try
        End Function

        ''' <summary>Permet de changer de Password sur un user</summary>
        ''' <param name="Username"></param>
        ''' <param name="OldPassword"></param>
        ''' <param name="Password"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function ChangePassword(ByVal IdSrv As String, ByVal Username As String, ByVal OldPassword As String, ByVal ConfirmNewPassword As String, ByVal Password As String) As Boolean Implements IHoMIDom.ChangePassword
            If VerifIdSrv(IdSrv) = False Then
                Return False
                Exit Function
            End If

            Dim retour As Boolean = False
            Try
                For i As Integer = 0 To _ListUsers.Count - 1
                    If _ListUsers.Item(i).UserName = Username Then
                        If _ListUsers.Item(i).Password = OldPassword Then
                            If ConfirmNewPassword = Password Then
                                _ListUsers.Item(i).Password = EncryptTripleDES(Password, "homidom")
                                retour = True
                                Exit For
                            End If
                        End If
                    End If
                Next
                Return retour
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "ChangePassword", "Exception : " & ex.Message)
                Return False
            End Try
        End Function

        ''' <summary>Retourne un user par son ID</summary>
        ''' <param name="UserId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ReturnUserById(ByVal IdSrv As String, ByVal UserId As String) As Users.User Implements IHoMIDom.ReturnUserById
            If VerifIdSrv(IdSrv) = False Then
                Return Nothing
                Exit Function
            End If

            Try
                Dim Resultat = (From User In _ListUsers Where User.ID = UserId Select User).First
                Return Resultat

            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "ReturnUserById", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function

#End Region

#Region "Telecommande"
        ''' <summary>
        ''' Retourne la liste des templates télécommande (fichier xml), présents dans le répertoire templates
        ''' </summary>
        ''' <returns>List of Templates</returns>
        ''' <remarks></remarks>
        Public Function GetListOfTemplate() As List(Of Telecommande.Template) Implements IHoMIDom.GetListOfTemplate
            Try
                Dim Tabl As New List(Of Telecommande.Template)
                Dim MyPath As String = _MonRepertoire & "\templates\"
                Dim xml As XML = Nothing

                Dim dirInfo As New System.IO.DirectoryInfo(MyPath)
                Dim file As System.IO.FileInfo
                Dim files() As System.IO.FileInfo = dirInfo.GetFiles("*.xml", System.IO.SearchOption.AllDirectories)

                If (files IsNot Nothing) Then
                    For Each file In files
                        MyXML = New XML(file.FullName)
                        Dim list As XmlNodeList

                        list = MyXML.SelectNodes("/template")
                        If list.Count > 0 Then 'présence des paramètres du template
                            Dim x As New Telecommande.Template
                            For j As Integer = 0 To list.Item(0).Attributes.Count - 1
                                Select Case list.Item(0).Attributes.Item(j).Name
                                    Case "fabricant"
                                        x.Fabricant = list.Item(0).Attributes.Item(j).Value
                                    Case "modele"
                                        x.Modele = list.Item(0).Attributes.Item(j).Value
                                    Case "driver"
                                        x.Driver = list.Item(0).Attributes.Item(j).Value
                                End Select
                                x.File = file.Name
                            Next
                            Tabl.Add(x)
                        End If
                        xml = Nothing
                        list = Nothing
                    Next
                End If

                Return Tabl
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetListOfTemplate", "Erreur : " & ex.Message)
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Crée un nouveau template dans le répertoire templates
        ''' </summary>
        ''' <param name="Fabricant">nom du fabricant</param>
        ''' <param name="Modele">modele</param>
        ''' <param name="Driver">driver</param>
        ''' <param name="Type">Type de base, si différent de VIDE va mettre les commandes de bases par défaut</param>
        ''' <returns>0 si ok, sinon message d'erreur</returns>
        ''' <remarks></remarks>
        Public Function CreateNewTemplate(ByVal Fabricant As String, ByVal Modele As String, ByVal Driver As String, ByVal Type As Telecommande.TypeEquipement) As String Implements IHoMIDom.CreateNewTemplate
            Try
                Dim MyPath As String = _MonRepertoire & "\templates\"
                Dim _Fichier As String = MyPath & LCase(Fabricant) & "-" & LCase(Modele) & "-" & Trim(LCase(Driver)) & ".xml"

                If IO.File.Exists(_Fichier) Then
                    Log(TypeLog.DEBUG, TypeSource.SERVEUR, "CreateNewTemplate", "Le template existe déjà pour ce même couple fabricant, modèle et driver!")
                    Return "Le template existe déjà pour ce même couple fabricant, modèle et driver!"
                    Exit Function
                End If

                ''Creation du fichier XML
                Dim writer As New XmlTextWriter(_Fichier, System.Text.Encoding.UTF8)
                writer.WriteStartDocument(True)
                writer.Formatting = Formatting.Indented
                writer.Indentation = 2

                writer.WriteStartElement("template")
                writer.WriteStartAttribute("fabricant")
                writer.WriteValue(LCase(Fabricant))
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("modele")
                writer.WriteValue(LCase(Modele))
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("driver")
                writer.WriteValue(LCase(Driver))
                writer.WriteEndAttribute()
                writer.WriteStartElement("commandes")

                Dim _listcmd() As String = Nothing
                Select Case Type
                    Case Telecommande.TypeEquipement.VIDE
                    Case Telecommande.TypeEquipement.TV
                        _listcmd = Telecommande.TemplateTV
                    Case Telecommande.TypeEquipement.DVD
                        _listcmd = Telecommande.TemplateDVD
                    Case Telecommande.TypeEquipement.BOX
                        _listcmd = Telecommande.TemplateBOX
                    Case Telecommande.TypeEquipement.AUDIO
                        _listcmd = Telecommande.TemplateAUDIO
                End Select

                If _listcmd IsNot Nothing Then
                    For i As Integer = 0 To _listcmd.Length - 1
                        writer.WriteStartElement("cmd")
                        writer.WriteStartAttribute("name")
                        writer.WriteValue(_listcmd(i))
                        writer.WriteEndAttribute()
                        writer.WriteStartAttribute("code")
                        writer.WriteValue("")
                        writer.WriteEndAttribute()
                        writer.WriteStartAttribute("repeat")
                        writer.WriteValue("0")
                        writer.WriteEndAttribute()
                        writer.WriteStartAttribute("picture")
                        writer.WriteValue(_MonRepertoire & "\images\telecommande\" & _listcmd(i) & ".png")
                        writer.WriteEndAttribute()
                        writer.WriteEndElement()
                    Next
                End If

                writer.WriteEndElement()
                writer.WriteEndElement()

                writer.WriteEndDocument()
                writer.Close()
                Log(TypeLog.INFO, TypeSource.SERVEUR, "CreateNewTemplate", "Nouveau template créé: " & Fabricant & "-" & Modele & "-" & Driver)

                Return "0"
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "CreateNewTemplate", "Erreur : " & ex.Message)
                Return ex.Message
            End Try
        End Function

        ''' <summary>
        ''' Recupère la liste des commandes d'un template donné
        ''' </summary>
        ''' <param name="Fabricant"></param>
        ''' <param name="Modele"></param>
        ''' <param name="Driver"></param>
        ''' <returns>Liste de commandes</returns>
        ''' <remarks></remarks>
        Public Function ReadTemplate(ByVal Fabricant As String, ByVal Modele As String, ByVal Driver As String) As List(Of Telecommande.Commandes)
            Try
                Dim _list As New List(Of Telecommande.Commandes)

                Dim MyPath As String = _MonRepertoire & "\templates\" & LCase(Fabricant) & "-" & LCase(Modele) & "-" & LCase(Driver) & ".xml"

                If IO.File.Exists(MyPath) = False Then
                    Log(TypeLog.ERREUR, TypeSource.SERVEUR, "ReadTemplate", "Erreur le fichier n'existe pas: " & MyPath)
                End If

                Dim xml As XML = Nothing

                MyXML = New XML(MyPath)
                Dim list As XmlNodeList

                list = MyXML.SelectNodes("/template/commandes/cmd")
                If list.Count > 0 Then 'présence des paramètres du template
                    For i As Integer = 0 To list.Count - 1
                        Dim x As New Telecommande.Commandes
                        For j As Integer = 0 To list.Item(i).Attributes.Count - 1
                            Select Case list.Item(i).Attributes.Item(j).Name
                                Case "name"
                                    If list.Item(i).Attributes.Item(j).Value IsNot Nothing Then x.Name = list.Item(i).Attributes.Item(j).Value
                                Case "code"
                                    If list.Item(i).Attributes.Item(j).Value IsNot Nothing Then x.Code = HtmlDecode(Replace(list.Item(i).Attributes.Item(j).Value, "&amp;", "&"))
                                Case "repeat"
                                    If list.Item(i).Attributes.Item(j).Value IsNot Nothing Then
                                        If IsNumeric(list.Item(i).Attributes.Item(j).Value) Then x.Repeat = list.Item(i).Attributes.Item(j).Value
                                    End If
                                Case "picture"
                                    If list.Item(i).Attributes.Item(j).Value IsNot Nothing Then
                                        If IO.File.Exists(list.Item(i).Attributes.Item(j).Value) Then
                                            x.Picture = list.Item(i).Attributes.Item(j).Value
                                        Else
                                            x.Picture = _MonRepertoire & "\images\telecommande\" & x.Name & ".png"
                                        End If
                                    End If
                            End Select
                        Next
                        _list.Add(x)
                    Next

                End If
                xml = Nothing
                list = Nothing

                Return _list
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "ReadTemplate", "Erreur : " & ex.ToString)
                Return Nothing
            End Try
        End Function

        ''' <summary>Demander un apprentissage à un driver</summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function StartLearning(ByVal IdSrv As String, ByVal DriverId As String) As String Implements IHoMIDom.StartLearning
            If VerifIdSrv(IdSrv) = False Then
                Return "ERREUR: l'Id du serveur est erroné"
                Exit Function
            End If

            Dim retour As String = ""
            Try
                For i As Integer = 0 To _ListDrivers.Count - 1
                    If _ListDrivers.Item(i).ID = DriverId Then
                        Dim x As Object = _ListDrivers.Item(i)
                        retour = x.LearnCode()
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "SERVEUR", "StartLearning: " & retour)
                    End If
                Next
                Return retour
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "StartLearning", "Erreur : " & ex.Message)
                Return ("ERREUR: " & ex.Message)
            End Try
        End Function

        ''' <summary>
        ''' Sauvegarde les commandes dans un template donné
        ''' </summary>
        ''' <param name="IdSrv">Id du Serveur</param>
        ''' <param name="Template">Nom du template</param>
        ''' <param name="Commandes">Liste des commandes</param>
        ''' <returns>O si ok sinon message d'erreur</returns>
        ''' <remarks></remarks>
        Public Function SaveTemplate(ByVal IdSrv As String, ByVal Template As String, ByVal Commandes As List(Of Telecommande.Commandes)) As String Implements IHoMIDom.SaveTemplate
            Try
                If VerifIdSrv(IdSrv) = False Then
                    Return 99
                    Exit Function
                End If

                Dim MyPath As String = _MonRepertoire & "\templates\"
                Dim _Fichier As String = MyPath & LCase(Template) & ".xml"

                If IO.File.Exists(_Fichier) = False Then
                    Return "Le template " & Template & ".xml n'existe pas!"
                    Exit Function
                End If

                Dim a() As String = Template.Split("-")
                If a.Length <> 3 Then
                    Return "Le nom du template " & Template & " est erroné!"
                    Exit Function
                End If

                ''Creation du fichier XML
                Dim writer As New XmlTextWriter(_Fichier, System.Text.Encoding.UTF8)
                writer.WriteStartDocument(True)
                writer.Formatting = Formatting.Indented
                writer.Indentation = 2

                writer.WriteStartElement("template")
                writer.WriteStartAttribute("fabricant")
                writer.WriteValue(LCase(a(0)))
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("modele")
                writer.WriteValue(LCase(a(1)))
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("driver")
                writer.WriteValue(LCase(a(2)))
                writer.WriteEndAttribute()
                writer.WriteStartElement("commandes")

                If Commandes IsNot Nothing Then
                    For i As Integer = 0 To Commandes.Count - 1
                        writer.WriteStartElement("cmd")
                        writer.WriteStartAttribute("name")
                        writer.WriteValue(Commandes(i).Name)
                        writer.WriteEndAttribute()
                        writer.WriteStartAttribute("code")
                        writer.WriteValue(HtmlEncode(Commandes(i).Code))
                        writer.WriteEndAttribute()
                        writer.WriteStartAttribute("repeat")
                        writer.WriteValue(Commandes(i).Repeat)
                        writer.WriteEndAttribute()
                        writer.WriteStartAttribute("picture")
                        If Commandes(i).Picture IsNot Nothing Then writer.WriteValue(Commandes(i).Picture)
                        writer.WriteEndAttribute()
                        writer.WriteEndElement()
                    Next
                End If

                writer.WriteEndElement()
                writer.WriteEndElement()

                writer.WriteEndDocument()
                writer.Close()

                Return "0"
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SaveTemplate", "Erreur : " & ex.Message)
                Return ex.Message
            End Try
        End Function

        ''' <summary>
        ''' Demande au device d'envoyer une commande (telecommande) à son driver
        ''' </summary>
        ''' <param name="IdSrv">Id du serveur, retourne 99 si non OK</param>
        ''' <param name="IdDevice">Id du device concerné</param>
        ''' <param name="Commande">Nom de la Commande à envoyée</param>
        ''' <returns>0 si Ok sinon erreur</returns>
        ''' <remarks></remarks>
        Public Function TelecommandeSendCommand(ByVal IdSrv As String, ByVal IdDevice As String, ByVal Commande As String) As String Implements IHoMIDom.TelecommandeSendCommand
            Try
                If VerifIdSrv(IdSrv) = False Then
                    Return 99
                    Exit Function
                End If

                Dim x As Object = Nothing

                For i As Integer = 0 To _ListDevices.Count - 1
                    If _ListDevices.Item(i).ID = IdDevice Then
                        _ListDevices.Item(i).EnvoyerCommande(Commande)
                        Return "0"
                        Exit Function
                    End If
                Next

                Return "Erreur: le device n'a pas été trouvé"
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "TelecommandeSendCommand", "Erreur : " & ex.ToString)
                Return "Erreur lors du traitement de la fonction TelecommandeSendCommand: " & ex.ToString
            End Try
        End Function
#End Region

#Region "Log"

        ''' <summary>
        ''' Retourne pour chaque type de log s'il doit être pris en compte ou non
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetTypeLogEnable() As List(Of Boolean) Implements IHoMIDom.GetTypeLogEnable
            Dim _list As New List(Of Boolean)

            For i As Integer = 0 To _TypeLogEnable.Count - 1
                If _TypeLogEnable.Item(i) = True Then
                    _list.Add(False)
                Else
                    _list.Add(True)
                End If
            Next

            Return _list
        End Function

        ''' <summary>
        ''' Fixe si chaque type de log doit être pris en compte ou non
        ''' </summary>
        ''' <param name="ListTypeLogEnable"></param>
        ''' <remarks></remarks>
        Public Sub SetTypeLogEnable(ByVal ListTypeLogEnable As List(Of Boolean)) Implements IHoMIDom.SetTypeLogEnable
            Try
                For i As Integer = 0 To ListTypeLogEnable.Count - 1
                    If ListTypeLogEnable(i) = True Then
                        _TypeLogEnable.Item(i) = False
                    Else
                        _TypeLogEnable.Item(i) = True
                    End If
                Next
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SetTypeLogEnable", "Erreur : " & ex.Message)
            End Try
        End Sub

        ''' <summary>
        ''' Retourne les 4 logs les plus récents (du plus récent au plus ancien)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Get4Log() As List(Of String) Implements IHoMIDom.Get4Log
            Dim list As New List(Of String)

            list.Add(_4Log(0))
            list.Add(_4Log(1))
            list.Add(_4Log(2))
            list.Add(_4Log(3))

            Return list
        End Function

        ''' <summary>
        ''' Retourne le nombre de mois à conserver une archive de log avant de le supprimer
        ''' </summary>
        ''' <param name="Month"></param>
        ''' <remarks></remarks>
        Public Sub SetMaxMonthLog(ByVal Month As Integer) Implements IHoMIDom.SetMaxMonthLog
            If IsNumeric(Month) Then
                _MaxMonthLog = Month
            End If
        End Sub

        ''' <summary>
        ''' Définit le nombre de mois à conserver une archive de log avant de le supprimer
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetMaxMonthLog() As Integer Implements IHoMIDom.GetMaxMonthLog
            Return _MaxMonthLog
        End Function

        ''' <summary>renvoi le fichier log suivant une requête xml si besoin</summary>
        ''' <param name="Requete"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function ReturnLog(Optional ByVal Requete As String = "") As String Implements IHoMIDom.ReturnLog
            Try
                Dim retour As String = ""
                If Requete = "" Then
                    Dim SR As New StreamReader(_MonRepertoire & "\logs\log_" & DateAndTime.Now.ToString("yyyyMMdd") & ".txt")
                    retour = SR.ReadToEnd()
                    retour = HtmlDecode(retour)
                    SR.Close()
                Else
                    'creation d'une nouvelle instance du membre xmldocument
                    Dim XmlDoc As XmlDocument = New XmlDocument()
                    XmlDoc.Load(_MonRepertoire & "\logs\log.xml")
                End If
                If retour.Length > 5000000 Then
                    Dim retour2 As String = Mid(retour, retour.Length - 8001, 8000)
                    retour = "Erreur, trop de ligne à traiter depuis le log seules les dernières lignes seront affichées, merci de consulter le fichier sur le serveur par en avoir la totalité!!" & vbCrLf & vbCrLf & retour2
                    Return retour
                End If
                Return retour
            Catch ex As Exception
                ReturnLog = "Erreur lors de la récupération du log: " & ex.ToString
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "ReturnLog", "Exception : " & ex.Message)
            End Try
        End Function

        ''' <summary>
        ''' Fixe la taille max du fichier log en Ko avant d'en créer un nouveau
        ''' </summary>
        ''' <param name="Value"></param>
        ''' <remarks></remarks>
        Public Sub SetMaxFileSizeLog(ByVal Value As Long) Implements IHoMIDom.SetMaxFileSizeLog
            MaxFileSize = Value
        End Sub

        ''' <summary>
        ''' Retourne la taille max du fichier log en Ko 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetMaxFileSizeLog() As Long Implements IHoMIDom.GetMaxFileSizeLog
            Return MaxFileSize
        End Function
#End Region

#Region "Configuration"
        ''' <summary>
        ''' Exporte le fichier de config vers une destination
        ''' </summary>
        ''' <param name="IdSrv">Id du serveur</param>
        ''' <returns>le fichier sous format text si ok sinon message d'erreur commençant par ERREUR</returns>
        ''' <remarks></remarks>
        Public Function ExportConfig(ByVal IdSrv As String) As String Implements IHoMIDom.ExportConfig
            Try
                If VerifIdSrv(IdSrv) = False Then
                    Return "ERREUR: L'Id du serveur est incorrect"
                    Exit Function
                End If

                Dim retour As String
                Dim SR As New StreamReader(_MonRepertoire & "\logs\log_" & DateAndTime.Now.ToString("yyyyMMdd") & ".txt")
                retour = SR.ReadToEnd()
                SR.Close()
                Return retour
            Catch ex As Exception
                Return "ERREUR lors de l'exportation du fichier de config: " & ex.ToString
            End Try
        End Function

        ''' <summary>
        ''' Importe un fichier de config depuis une source
        ''' </summary>
        ''' <param name="Source">chemin + fichier (homidom.xml)</param>
        ''' <returns>"0" si ok sinon message d'erreur</returns>
        ''' <remarks></remarks>
        Public Function ImportConfig(ByVal IdSrv As String, ByVal Source As String) As String Implements IHoMIDom.ImportConfig
            Try
                If VerifIdSrv(IdSrv) = False Then
                    Return "L'Id du serveur est incorrect!"
                    Exit Function
                End If

                If IO.File.Exists(Source) = False Then
                    Return "Le serveur n'a pas trouvé le fichier de configuration !"
                    Exit Function
                End If

                'sauvegarde de l'ancien fichier sous .old
                IO.File.Copy(_MonRepertoire & "\config\homidom.xml", _MonRepertoire & "\config\homidom.old", True)
                IO.File.Copy(Source, _MonRepertoire & "\config\homidom.xml", True)

                Return "0"
            Catch ex As Exception
                Return "Erreur lors de l'importation du fichier de config: " & ex.ToString
            End Try
        End Function

        ''' <summary>Sauvegarder la configuration</summary>
        ''' <remarks></remarks>
        Public Function SaveConfiguration(ByVal IdSrv As String) As String Implements IHoMIDom.SaveConfig
            If VerifIdSrv(IdSrv) = False Then
                Return "L'Id du serveur est incorrect"
            End If

            Try
                If SaveConfig(_MonRepertoire & "\config\homidom.xml") = True Then
                    Return "0"
                Else
                    Return "Erreur lors de l'enregistrement veuillez consulter le log"
                End If

            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SaveConfiguration", "Exception : " & ex.Message)
                Return "Erreur lors de l'enregistrement veuillez consulter le log"
            End Try
        End Function
#End Region

#Region "SOAP"
        ''' <summary>Fixer la valeur du port SOAP</summary>
        ''' <param name="Value"></param>
        ''' <remarks></remarks>
        Public Sub SetPortSOAP(ByVal IdSrv As String, ByVal Value As Double) Implements IHoMIDom.SetPortSOAP
            If VerifIdSrv(IdSrv) = False Then
                Exit Sub
            End If

            Try
                _PortSOAP = Value
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SetPortSOAP", "Exception : " & ex.Message)
            End Try
        End Sub

        ''' <summary>Retourne la valeur du port SOAP</summary>
        ''' <returns>Numero du port ou -1 si erreur</returns>
        ''' <remarks></remarks>
        Public Function GetPortSOAP() As Double Implements IHoMIDom.GetPortSOAP
            Try
                Return _PortSOAP
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetPortSOAP", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>Fixer la valeur IP SOAP</summary>
        ''' <param name="Value"></param>
        ''' <remarks></remarks>
        Public Sub SetIPSOAP(ByVal IdSrv As String, ByVal Value As String) Implements IHoMIDom.SetIPSOAP
            If VerifIdSrv(IdSrv) = False Then
                Exit Sub
            End If

            Try
                _IPSOAP = Value
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SetIPSOAP", "Erreur: " & ex.Message)
            End Try
        End Sub

        ''' <summary>Retourne la valeur IP SOAP</summary>
        ''' <returns>Numero du port ou -1 si erreur</returns>
        ''' <remarks></remarks>
        Public Function GetIPSOAP() As String Implements IHoMIDom.GetIPSOAP
            Try
                Return _IPSOAP
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetIPSOAP", "Erreur: " & ex.Message)
                Return -1
            End Try
        End Function

#End Region

#Region "Maintenance"
        Public Function CleanLog(ByVal Mois As Integer) As String
            Try

                Dim dirInfo As New System.IO.DirectoryInfo(_MonRepertoire & "\logs\")
                Dim file As System.IO.FileInfo
                Dim files() As System.IO.FileInfo = dirInfo.GetFiles("*.xml", System.IO.SearchOption.AllDirectories)
                Dim DateRef As DateTime = Now.AddMonths(-1 * Mois)
                Dim cnt As Integer = 0

                If (files IsNot Nothing) Then
                    For Each file In files
                        Dim x As New ImageFile

                        'C'est un log archivé
                        If InStr(file.Name, "_") > 0 Then
                            Dim a() As String = file.Name.Split("_")
                            If a IsNot Nothing Then
                                If a.Count > 2 Then
                                    If a(0) = "log" Then
                                        Dim filedate As String = a(1)
                                        Dim fileyear As String = Mid(filedate, 1, 4)
                                        Dim filemonth As String = Mid(filedate, 5, 2)
                                        If DateRef.Year > fileyear Then
                                            file.Delete()
                                            cnt += 1
                                        Else
                                            If DateRef.Month >= filemonth Then
                                                file.Delete()
                                                cnt += 1
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    Next
                End If
                Log(TypeLog.INFO, TypeSource.SERVEUR, "CleanLog", cnt & " Fichier(s) log supprimé(s)")
                Return 0
            Catch ex As Exception
                Return "ERR:" & ex.ToString
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "CleanLog", "Erreur: " & ex.Message)
            End Try
        End Function
#End Region

#End Region

    End Class

End Namespace