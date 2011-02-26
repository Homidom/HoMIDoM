Imports System.IO
Imports System.Xml
Imports System.Xml.XPath
Imports System.Xml.Serialization
Imports System.Reflection
Imports STRGS = Microsoft.VisualBasic.Strings

Namespace HoMIDom

    '***********************************************
    '** CLASS SERVER
    '** version 1.0
    '** Date de création: 12/01/2011
    '** Historique (SebBergues): 12/01/2011: Création 
    '***********************************************

    ''' <summary>
    ''' Classe Server
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> Public Class Server
        Inherits MarshalByRefObject
        Implements IHoMIDom 'implémente l'interface dans cette class

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

        End Sub

        ''' <summary>Evenement provenant des devices</summary>
        ''' <param name="Device"></param>
        ''' <param name="Property"></param>
        ''' <param name="Parametres"></param>
        ''' <remarks></remarks>
        Public Sub DeviceChange(ByVal Device As Object, ByVal [Property] As String, ByVal Parametres As Object)
            Dim retour As String
            Try
                Dim valeur = Parametres
                Log(TypeLog.INFO, TypeSource.SERVEUR, "DeviceChange", "Historiser " & Device.name & " (" & [Property] & ") : " & valeur)

                If STRGS.Left(valeur, 4) <> "ERR:" Then 'si y a pas erreur d'acquisition
                    '--- Remplacement de , par .
                    Parametres = STRGS.Replace(valeur, ",", ".")
                    '--- si c'est un nombre
                    If (IsNumeric(valeur) And IsNumeric(Device.Value) And IsNumeric(Device.ValueLast)) Then
                        'si lastetat=True, on vérifie que la valeur a changé par rapport a l'avant dernier etat (valuelast) 
                        If Device.LastEtat And valeur.ToString = Device.ValueLast Then
                            'log de "inchangé lastetat"
                            Log(TypeLog.VALEUR_INCHANGE_LASTETAT, TypeSource.SERVEUR, "DeviceChange", Device.Name.ToString() & " : " & Device.Adresse1 & " : " & valeur & " (inchangé lastetat " & Device.ValueLast & ")")
                        Else
                            'on vérifie que la valeur a changé de plus de composants_precision sinon inchangé
                            If (CDbl(valeur) + CDbl(Device.Precision)) >= CDbl(Device.Value) And (CDbl(valeur) - CDbl(Device.Precision)) <= CDbl(Device.Value) Then
                                'log de "inchangé précision"
                                Log(TypeLog.VALEUR_INCHANGE_PRECISION, TypeSource.SERVEUR, "DeviceChange", Device.Name.ToString() & " : " & Device.Adresse1 & " : " & valeur & " (inchangé precision " & Device.ValueLast & ")")
                            Else
                                'log de la nouvelle valeur
                                Log(TypeLog.VALEUR_CHANGE, TypeSource.SERVEUR, "DeviceChange", Device.Name.ToString() & " : " & Device.Adresse1 & " : " & valeur)
                                'On historise la nouvellevaleur
                                retour = sqlite_homidom.nonquery("INSERT INTO historiques (device_id,source,dateheure,valeur) VALUES (" & Device.ID & "," & [Property] & "," & Now.ToString() & "," & valeur & ")")
                                If STRGS.Left(retour, 4) = "ERR:" Then
                                    Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeviceChange", "Erreur lors Requete sqlite : " & retour)
                                End If
                            End If
                        End If
                    Else
                        'Valeur est autre chose qu'un nombre
                        '--- log de la nouvelle valeur
                        Log(TypeLog.VALEUR_CHANGE, TypeSource.SERVEUR, "DeviceChange", Device.Name.ToString() & " : " & Device.Adresse1 & " : " & valeur)
                        '--- historise la valeur si ce n'est pas une simple info de config
                        If STRGS.Left(valeur, 4) <> "CFG:" Then
                            retour = sqlite_homidom.nonquery("INSERT INTO historiques (device_id,source,dateheure,valeur) VALUES (" & Device.ID & "," & [Property] & "," & Now.ToString() & "," & valeur & ")")
                            If STRGS.Left(retour, 4) = "ERR:" Then
                                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeviceChange", "Erreur lors Requete sqlite : " & retour)
                            End If
                        End If
                    End If
                Else
                    'erreur d'acquisition
                    Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeviceChange", "Erreur d'acquisition : " & Device.Name & " - " & valeur)
                End If
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeviceChange", "Exception : " & ex.Message)
            End Try




            ''on verifie si un composant correspond à cette adresse
            'tabletmp = domos_svc.table_composants.Select("composants_adresse = '" & adresse.ToString & "' AND composants_modele_norme = 'RFX'")
            'If tabletmp.GetUpperBound(0) >= 0 Then
            '    '--- On attend au moins x seconde entre deux receptions ou si valeur<>valeurlastetat (donc pas le meme composant)
            '    'If (DateTime.Now - Date.Parse(tabletmp(0)("composants_etatdate"))).TotalMilliseconds > tempsentrereponse Or valeur <> valeurlast Then
            '    If (DateTime.Now - Date.Parse(tabletmp(0)("composants_etatdate"))).TotalMilliseconds > tempsentrereponse Then
            '        If VB.Left(valeur, 4) <> "ERR:" Then 'si y a pas erreur d'acquisition
            '            '--- Remplacement de , par .
            '            valeur = STRGS.Replace(valeur, ",", ".")
            '            '--- Correction si besoin ---
            '            If (tabletmp(0)("composants_correction") <> "" And tabletmp(0)("composants_correction") <> "0") Then
            '                valeur = valeur + CDbl(tabletmp(0)("composants_correction"))
            '            End If
            '            '--- comparaison du relevé avec le dernier etat ---
            '            '--- si la valeur a changé ou autre chose qu'un nombre (ON, OFF, ALERT...) --- 
            '            If valeur.ToString <> tabletmp(0)("composants_etat").ToString() Or Not IsNumeric(valeur) Then
            '                'si nombre alors 
            '                If (IsNumeric(valeur) And IsNumeric(tabletmp(0)("lastetat")) And IsNumeric(tabletmp(0)("composants_etat"))) Then
            '                    'on vérifie que la valeur a changé par rapport a l'avant dernier etat (lastetat) si lastetat (table config)
            '                    If lastetat And valeur.ToString = tabletmp(0)("lastetat").ToString() Then
            '                        _Server.Log(TypeLog.VALEUR_INCHANGE_LASTETAT, TypeSource.DRIVER, "RFXCOM_RECEIVER " & tabletmp(0)("composants_nom").ToString() & " : " & tabletmp(0)("composants_adresse").ToString() & " : " & valeur.ToString & " (inchangé lastetat " & tabletmp(0)("lastetat").ToString() & ")")
            '                        '--- Modification de la date dans la base SQL ---
            '                        dateheure = DateAndTime.Now.Year.ToString() & "-" & DateAndTime.Now.Month.ToString() & "-" & DateAndTime.Now.Day.ToString() & " " & STRGS.Left(DateAndTime.Now.TimeOfDay.ToString(), 8)
            '                        Err = domos_svc.mysql.mysql_nonquery("UPDATE composants SET composants_etatdate='" & dateheure & "' WHERE composants_id='" & tabletmp(0)("composants_id") & "'")
            '                        If Err <> "" Then WriteLog("ERR: inchange lastetat " & Err)
            '                    Else
            '                        'on vérifie que la valeur a changé de plus de composants_precision sinon inchangé
            '                        'If (valeur + CDbl(tabletmp(0)("composants_precision"))).ToString >= tabletmp(0)("composants_etat").ToString() And (valeur - CDbl(tabletmp(0)("composants_precision"))).ToString <= tabletmp(0)("composants_etat").ToString() Then
            '                        If (CDbl(valeur) + CDbl(tabletmp(0)("composants_precision"))) >= CDbl(tabletmp(0)("composants_etat")) And (CDbl(valeur) - CDbl(tabletmp(0)("composants_precision"))) <= CDbl(tabletmp(0)("composants_etat")) Then
            '                            'log de "inchangé précision"
            '                            _Server.Log(TypeLog.VALEUR_INCHANGE_PRECISION, TypeSource.DRIVER, "RFXCOM_RECEIVER " & tabletmp(0)("composants_nom").ToString() & " : " & tabletmp(0)("composants_adresse").ToString() & " : " & valeur.ToString & " (inchangé precision " & tabletmp(0)("composants_etat").ToString & "+-" & tabletmp(0)("composants_precision").ToString & ")")
            '                            '--- Modification de la date dans la base SQL ---
            '                            dateheure = DateAndTime.Now.Year.ToString() & "-" & DateAndTime.Now.Month.ToString() & "-" & DateAndTime.Now.Day.ToString() & " " & STRGS.Left(DateAndTime.Now.TimeOfDay.ToString(), 8)
            '                            Err = domos_svc.mysql.mysql_nonquery("UPDATE composants SET composants_etatdate='" & dateheure & "' WHERE composants_id='" & tabletmp(0)("composants_id") & "'")
            '                            If Err <> "" Then WriteLog("ERR: inchange precision " & Err)
            '                        Else
            '                            _Server.Log(TypeLog.VALEUR_CHANGE, TypeSource.DRIVER, "RFXCOM_RECEIVER " & tabletmp(0)("composants_nom").ToString() & " : " & tabletmp(0)("composants_adresse").ToString() & " : " & valeur.ToString)

            '                            '  --- modification de l'etat du composant dans la table en memoire ---
            '                            tabletmp(0)("lastetat") = tabletmp(0)("composants_etat") 'on garde l'ancien etat en memoire pour le test de lastetat
            '                            tabletmp(0)("composants_etat") = valeur.ToString
            '                            tabletmp(0)("composants_etatdate") = DateAndTime.Now.Year.ToString() & "-" & DateAndTime.Now.Month.ToString() & "-" & DateAndTime.Now.Day.ToString() & " " & STRGS.Left(DateAndTime.Now.TimeOfDay.ToString(), 8)
            '                        End If
            '                    End If
            '                Else
            '                    'si la valeur a changer et = ON ou OFF on logue sinon debug
            '                    If valeur.ToString = tabletmp(0)("composants_etat").ToString() And (valeur.ToString = "ON" Or valeur.ToString = "OFF") Then
            '                        WriteLog("DBG: inchange ON-OFF: " & tabletmp(0)("composants_nom").ToString() & " : " & tabletmp(0)("composants_adresse").ToString() & " : " & valeur.ToString)
            '                    Else
            '                        _Server.Log(TypeLog.VALEUR_CHANGE, TypeSource.DRIVER, "RFXCOM_RECEIVER " & tabletmp(0)("composants_nom").ToString() & " : " & tabletmp(0)("composants_adresse").ToString() & " : " & valeur.ToString)
            '                    End If
            '                    '  --- modification de l'etat du composant dans la table en memoire ---
            '                    If VB.Left(valeur, 4) <> "CFG:" Then
            '                        tabletmp(0)("lastetat") = tabletmp(0)("composants_etat") 'on garde l'ancien etat en memoire pour le test de lastetat
            '                        tabletmp(0)("composants_etat") = valeur.ToString
            '                        tabletmp(0)("composants_etatdate") = DateAndTime.Now.Year.ToString() & "-" & DateAndTime.Now.Month.ToString() & "-" & DateAndTime.Now.Day.ToString() & " " & STRGS.Left(DateAndTime.Now.TimeOfDay.ToString(), 8)
            '                    End If
            '                End If
            '            Else
            '                'la valeur n'a pas changé, on log en 7 et on maj la date dans la base sql
            '                _Server.Log(TypeLog.VALEUR_INCHANGE, TypeSource.DRIVER, "RFXCOM_RECEIVER " & tabletmp(0)("composants_nom").ToString() & " : " & tabletmp(0)("composants_adresse").ToString() & " : " & valeur.ToString & " (inchangé " & tabletmp(0)("composants_etat").ToString() & ")")
            '                '--- Modification de la date dans la base SQL ---
            '                dateheure = DateAndTime.Now.Year.ToString() & "-" & DateAndTime.Now.Month.ToString() & "-" & DateAndTime.Now.Day.ToString() & " " & STRGS.Left(DateAndTime.Now.TimeOfDay.ToString(), 8)
            '                Err = domos_svc.mysql.mysql_nonquery("UPDATE composants SET composants_etatdate='" & dateheure & "' WHERE composants_id='" & tabletmp(0)("composants_id") & "'")
            '                If Err <> "" Then WriteLog("ERR: inchange : " & Err)
            '            End If
            '        Else
            '            'erreur d'acquisition ou battery
            '            If InStr(LCase(valeur), "battery") > 0 Then
            '                WriteLog("ERR: " & tabletmp(0)("composants_nom").ToString() & " : " & valeur.ToString)
            '            Else
            '                WriteLog("ERR: " & tabletmp(0)("composants_nom").ToString() & " : " & valeur.ToString)
            '            End If
            '        End If
            '    Else
            '        WriteLog("DBG: IGNORE : Etat recu il y a moins de " & domos_svc.rfx_tpsentrereponse & " msec : " & adresse.ToString & " : " & valeur.ToString)
            '    End If
            'ElseIf Not domos_svc.RFX_ignoreadresse Then
            '    'erreur d'adresse composant
            '    If adresse <> adresselast Then
            '        tabletmp = domos_svc.table_composants_bannis.Select("composants_bannis_adresse = '" & adresse.ToString & "' AND composants_bannis_norme = 'RFX'")
            '        If tabletmp.GetUpperBound(0) >= 0 Then
            '            'on logue en debug car c'est une adresse bannie
            '            WriteLog("DBG: IGNORE : Adresse Bannie : " & adresse.ToString & " : " & valeur.ToString)
            '        Else
            '            WriteLog("ERR: Adresse composant : " & adresse.ToString & " : " & valeur.ToString)
            '        End If
            '    Else
            '        'on logue en debug car c'est la même adresse non trouvé depuis le dernier message
            '        WriteLog("DBG: IGNORE : Adresse composant : " & adresse.ToString & " : " & valeur.ToString)
            '    End If
            'Else
            '    WriteLog("DBG: IGNORE : Adresse composant : " & adresse.ToString & " : " & valeur.ToString)
            'End If
            'adresselast = adresse
            'valeurlast = valeur

        End Sub

        ''' <summary>Traitement à effectuer toutes les secondes/minutes/heures/minuit/midi</summary>
        ''' <remarks></remarks>
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
        Private Shared WithEvents _ListDrivers As New ArrayList 'Liste des drivers
        Private Shared _ListImgDrivers As New ArrayList
        Private Shared WithEvents _ListDevices As New ArrayList 'Liste des devices
        Private Shared _ListZones As New ArrayList 'Liste des zones
        Private sqlite_homidom As New Sqlite 'BDD sqlite pour Homidom
        Private sqlite_medias As New Sqlite 'BDD sqlite pour les medias
        Private _MonRepertoire As String = System.Environment.CurrentDirectory 'représente le répertoire de l'application 'Application.StartupPath
        Dim Soleil As New Soleil 'Déclaration class Soleil
        Dim _Longitude As Double 'Longitude
        Dim _Latitude As Double 'latitude
        Dim _HeureLeverSoleil As DateTime 'heure du levé du soleil
        Dim _HeureCoucherSoleil As DateTime 'heure du couché du soleil
        Dim _HeureLeverSoleilCorrection As Integer 'correction à appliquer sur heure du levé du soleil
        Dim _HeureCoucherSoleilCorrection As Integer 'correction à appliquer sur heure du couché du soleil

        Dim TimerSecond As New Timers.Timer 'Timer à la seconde

#End Region

#Region "Fonctions/Sub propres au serveur"

        ''' <summary>Liste les type de devices par leur valeur d'Enum</summary>
        ''' <param name="Index"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ReturnSringFromEnumDevice(ByVal Index As Integer) As String
            For Each value As Device.ListeDevices In [Enum].GetValues(GetType(Device.ListeDevices))
                If value.GetHashCode = Index Then
                    Return value.ToString
                    Exit Function
                End If
            Next
            Return ""
        End Function

        ''' <summary>Initialisation des heures du soleil</summary>
        ''' <remarks></remarks>
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

        ''' <summary>Chargement de la config depuis le fichier XML</summary>
        ''' <param name="Fichier"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
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
                        'on va chercher les zones
                        '******************************************
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Chargement des zones")
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
                                            If list.Item(i).Attributes.Item(j).Value <> Nothing Then x.Icon = list.Item(0).Attributes.Item(j).Value
                                        Case "image"
                                            If list.Item(i).Attributes.Item(j).Value <> Nothing Then x.Image = list.Item(0).Attributes.Item(j).Value
                                        Case Else
                                            Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Un attribut correspondant à la zone est inconnu: nom:" & list.Item(i).Attributes.Item(j).Name & " Valeur: " & list.Item(0).Attributes.Item(j).Value)
                                    End Select
                                Next
                                If list.Item(i).HasChildNodes = True Then
                                    For k As Integer = 0 To list.Item(i).ChildNodes.Count - 1
                                        If list.Item(i).ChildNodes.Item(k).Name = "device" Then
                                            For k1 As Integer = 0 To list.Item(i).ChildNodes.Item(k).ChildNodes.Count - 1
                                                Dim _dev As New Zone.Device_Zone(list.Item(i).ChildNodes.Item(k).ChildNodes.Item(k1).Attributes(0).Value, list.Item(i).ChildNodes.Item(k).ChildNodes.Item(k1).Attributes(1).Value, list.Item(i).ChildNodes.Item(k).ChildNodes.Item(k1).Attributes(2).Value, list.Item(i).ChildNodes.Item(k).ChildNodes.Item(k1).Attributes(3).Value)
                                                x.ListDevice.Add(_dev)
                                            Next
                                        End If
                                    Next
                                End If
                                _ListZones.Add(x)
                            Next
                        Else
                            Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Il manque les zones dans le fichier de config !!")
                        End If
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", _ListZones.Count & " Zone(s) chargée(s)")

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
                                    If (Not list.Item(j).Attributes.GetNamedItem("refresh") Is Nothing) Then .Refresh = list.Item(j).Attributes.GetNamedItem("refresh").Value
                                    If (Not list.Item(j).Attributes.GetNamedItem("modele") Is Nothing) Then .Modele = list.Item(j).Attributes.GetNamedItem("modele").Value
                                    If (Not list.Item(j).Attributes.GetNamedItem("picture") Is Nothing) Then .Picture = list.Item(j).Attributes.GetNamedItem("picture").Value
                                    If (Not list.Item(j).Attributes.GetNamedItem("solo") Is Nothing) Then .Solo = list.Item(j).Attributes.GetNamedItem("solo").Value
                                    If (Not list.Item(j).Attributes.GetNamedItem("value") Is Nothing) Then .Value = list.Item(j).Attributes.GetNamedItem("value").Value
                                    If (Not list.Item(j).Attributes.GetNamedItem("valuelast") Is Nothing) Then .ValueLast = list.Item(j).Attributes.GetNamedItem("valuelast").Value
                                    If (Not list.Item(j).Attributes.GetNamedItem("lastetat") Is Nothing) Then .LastEtat = list.Item(j).Attributes.GetNamedItem("lastetat").Value
                                    '-- propriétés generique value --
                                    If _Dev.Type = "BAROMETRE" _
                                    Or _Dev.Type = "COMPTEUR" _
                                    Or _Dev.Type = "ENERGIEINSTANTANEE" _
                                    Or _Dev.Type = "ENERGIETOTALE" _
                                    Or _Dev.Type = "GENERIQUEVALUE" _
                                    Or _Dev.Type = "HUMIDITE" _
                                    Or _Dev.Type = "PLUIECOURANT" _
                                    Or _Dev.Type = "PLUIETOTAL" _
                                    Or _Dev.Type = "TEMPERATURE" _
                                    Or _Dev.Type = "TEMPERATURECONSIGNE" _
                                    Or _Dev.Type = "VITESSEVENT" _
                                    Or _Dev.Type = "UV" _
                                    Or _Dev.Type = "VITESSEVENT" _
                                    Then
                                        If (Not list.Item(j).Attributes.GetNamedItem("valuemin") Is Nothing) Then .ValueMin = list.Item(j).Attributes.GetNamedItem("valuemin").Value
                                        If (Not list.Item(j).Attributes.GetNamedItem("valuemax") Is Nothing) Then .ValueMax = list.Item(j).Attributes.GetNamedItem("valuemax").Value
                                        If (Not list.Item(j).Attributes.GetNamedItem("valuedef") Is Nothing) Then .ValueDef = list.Item(j).Attributes.GetNamedItem("valuedef").Value
                                        If (Not list.Item(j).Attributes.GetNamedItem("precision") Is Nothing) Then .Precision = list.Item(j).Attributes.GetNamedItem("precision").Value
                                        If (Not list.Item(j).Attributes.GetNamedItem("correction") Is Nothing) Then .Correction = list.Item(j).Attributes.GetNamedItem("correction").Value
                                        If (Not list.Item(j).Attributes.GetNamedItem("formatage") Is Nothing) Then .Formatage = list.Item(j).Attributes.GetNamedItem("formatage").Value
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
                                    If .ID <> "" And .Name <> "" And .Adresse1 <> "" And .DriverId <> "" Then
                                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Chargement du device " & .Name & " (" & .ID & " - " & .Adresse1 & " - " & .Type & ")")
                                    Else
                                        _Dev.Enable = False
                                        Log(TypeLog.ERREUR, TypeSource.SERVEUR, "LoadConfig", "Erreur lors du chargement du device (information incomplete -> Disable) " & .Name & " (" & .ID & " - " & .Adresse1 & " - " & .Type & ")")
                                    End If
                                End With
                                _ListDevices.Add(_Dev)
                                _Dev = Nothing
                            Next
                            Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", _ListDevices.Count & " devices(s) trouvé(s)")
                        Else
                            Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Aucun device trouvé dans le fichier de configuration")
                        End If
                        list = Nothing
                    Next


                Else
                    Log(TypeLog.ERREUR, TypeSource.SERVEUR, "LoadConfig", "Fichier de configuration non trouvé")
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

        ''' <summary>Sauvegarde de la config dans le fichier XML</summary>
        ''' <remarks></remarks>
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
                ''Sauvegarde des zones
                ''------------
                Log(TypeLog.INFO, TypeSource.SERVEUR, "SaveConfig", "Sauvegarde des zones")
                writer.WriteStartElement("zones")
                For i As Integer = 0 To _ListZones.Count - 1
                    writer.WriteStartElement("zone")
                    writer.WriteStartAttribute("id")
                    writer.WriteValue(_ListZones.Item(i).id)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("name")
                    writer.WriteValue(_ListZones.Item(i).name)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("icon")
                    writer.WriteValue(_ListZones.Item(i).icon)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("image")
                    writer.WriteValue(_ListZones.Item(i).image)
                    writer.WriteEndAttribute()
                    If _ListZones.Item(i).ListDevice IsNot Nothing Then
                        For j As Integer = 0 To _ListZones.Item(i).listdevice.count - 1
                            writer.WriteStartElement("device")
                            writer.WriteStartAttribute("deviceid")
                            writer.WriteValue(_ListZones.Item(i).listdevice.item(j).deviceid)
                            writer.WriteEndAttribute()
                            writer.WriteStartAttribute("visible")
                            writer.WriteValue(_ListZones.Item(i).listdevice.item(j).visible)
                            writer.WriteEndAttribute()
                            writer.WriteStartAttribute("X")
                            writer.WriteValue(_ListZones.Item(i).listdevice.item(j).x)
                            writer.WriteEndAttribute()
                            writer.WriteStartAttribute("Y")
                            writer.WriteValue(_ListZones.Item(i).listdevice.item(j).y)
                            writer.WriteEndAttribute()
                            writer.WriteEndElement()
                        Next
                    End If
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
                    Dim _pict As String = _ListDevices.Item(i).picture
                    If _pict = "" Or _pict = Nothing Then _pict = " "
                    writer.WriteValue(_pict)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("solo")
                    writer.WriteValue(_ListDevices.Item(i).solo)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("value")
                    writer.WriteValue(_ListDevices.Item(i).value)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("valuelast")
                    writer.WriteValue(_ListDevices.Item(i).valuelast)
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

        ''' <summary>Charge les drivers, donc toutes les dll dans le sous répertoire "plugins"</summary>
        ''' <remarks></remarks>
        Public Sub Drivers_Load()
            Try
                Dim tx As String
                Dim dll As Reflection.Assembly
                Dim tp As Type
                Dim Chm As String = _MonRepertoire & "\Plugins\" 'Emplacement par défaut des plugins

                Dim strFileSize As String = ""
                Dim di As New IO.DirectoryInfo(Chm)
                Dim aryFi As IO.FileInfo() = di.GetFiles("*.dll")
                Dim fi As IO.FileInfo

                'Cherche tous les fichiers dll dans le répertoie plugin
                Log(TypeLog.INFO, TypeSource.SERVEUR, "Drivers_Load", "Chargement des DLL des drivers")
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
                                'i1.Start()
                                Log(TypeLog.INFO, TypeSource.SERVEUR, "Drivers_Load", " - " & i1.Nom & " chargé")
                            End If
                        End If
                    Next
                Next
            Catch ex As Exception
                MsgBox("Erreur lors du chargement des drivers: " & ex.Message)
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Drivers_Load", " Erreur lors du chargement des drivers: " & ex.Message)
            End Try
        End Sub

        ''' <summary>Démarre tous les drivers dont la propriété StartAuto=True</summary>
        ''' <remarks></remarks>
        Public Sub Drivers_Start()
            Try
                'Cherche tous les drivers chargés
                Log(TypeLog.INFO, TypeSource.SERVEUR, "Drivers_Start", "Démarrage des drivers")
                For Each driver In _ListDrivers
                    If driver.Enable And driver.StartAuto Then
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "Drivers_Start", " - " & driver.Nom & " démarré")
                        driver.start()
                    Else
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "Drivers_Start", " - " & driver.Nom & " non démarré car non Auto")
                    End If
                Next
            Catch ex As Exception
                MsgBox("Erreur lors du démarrage des drivers: " & ex.Message)
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Drivers_Start", " Erreur lors du démarrage des drivers: " & ex.Message)
            End Try
        End Sub

        ''' <summary>Arretes les drivers démarrés</summary>
        ''' <remarks></remarks>
        Public Sub Drivers_Stop()
            Try
                'Cherche tous les drivers chargés
                Log(TypeLog.INFO, TypeSource.SERVEUR, "Drivers_Stop", "Arrêt des drivers")
                For Each driver In _ListDrivers
                    If driver.Enable And driver.IsConnect Then
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "Drivers_Stop", " - " & driver.Nom & " démarré")
                        driver.stop()
                    End If
                Next
            Catch ex As Exception
                MsgBox("Erreur lors de l arret des drivers: " & ex.Message)
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Drivers_Stop", " Erreur lors de l'arret des drivers: " & ex.Message)
            End Try
        End Sub

        ''' <summary>Arretes les devices (Handlers)</summary>
        ''' <remarks></remarks>
        Public Sub Devices_Stop()
            Try
                'Cherche tous les drivers chargés
                Log(TypeLog.INFO, TypeSource.SERVEUR, "Devices_Stop", "Arrêt des devices")
                For Each _dev In _ListDevices
                    Log(TypeLog.INFO, TypeSource.SERVEUR, "Devices_Stop", " - " & _dev.Name & " démarré")
                    'marche pas !!!!!

                    'RemoveHandler _dev.DeviceChanged, AddressOf DeviceChange

                Next
            Catch ex As Exception
                MsgBox("Erreur lors de l arret des drivers: " & ex.Message)
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Devices_Stop", " Erreur lors de l'arret des devices: " & ex.Message)
            End Try
        End Sub

        ''' <summary>Retourne les propriétés d'un driver</summary>
        ''' <remarks></remarks>
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

        ''' <summary>Ecrire ou lance propritété/Sub d'un driver</summary>
        ''' <remarks></remarks>
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

        Public Property Zones() As ArrayList Implements IHoMIDom.Zones
            Get
                Return _ListZones
            End Get
            Set(ByVal value As ArrayList)
                _ListZones = value
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
                    _ListDevices.RemoveAt(i)
                    DeleteDevice = 0
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

        'Supprimer une zone de la config
        Public Function DeleteZone(ByVal zoneId As String) As Integer Implements IHoMIDom.DeleteZone
            For i As Integer = 0 To _ListZones.Count - 1
                If _ListZones.Item(i).Id = zoneId Then
                    _ListZones.Item(i).removeat(i)
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

        'ajouter un device à une zone
        Function AddDeviceToZone(ByVal ZoneId As String, ByVal DeviceId As String, ByVal Visible As Boolean, Optional ByVal X As Double = 0, Optional ByVal Y As Double = 0) As String Implements IHoMIDom.AddDeviceToZone
            Dim _zone As Zone = ReturnZoneById(ZoneId)
            Dim _retour As String = ""

            If _zone IsNot Nothing Then
                Dim _dev As New Zone.Device_Zone("", Visible, X, Y)
                _zone.ListDevice.Add(_dev)
                _retour = "0"
            End If

            Return _retour
        End Function

        'supprimer un device à une zone
        Function DeleteDeviceToZone(ByVal ZoneId As String, ByVal DeviceId As String) As String Implements IHoMIDom.DeleteDeviceToZone
            Dim _zone As Zone = ReturnZoneById(ZoneId)
            Dim _retour As String = ""

            If _zone IsNot Nothing Then
                For i As Integer = 0 To _zone.ListDevice.Count - 1
                    If _zone.ListDevice.Item(i).deviceid = DeviceId Then
                        _zone.ListDevice.RemoveAt(i)
                        Exit For
                    End If
                Next
                _retour = "0"
            End If

            Return _retour
        End Function

        'renvoi le fichier log suivant une requête xml si besoin
        Function ReturnLog(Optional ByVal Requete As String = "") As String Implements IHoMIDom.ReturnLog
            If Requete = "" Then
                Dim SR As New StreamReader(_MonRepertoire & "\logs\log.xml")
                ReturnLog = SR.ReadToEnd()
                SR.Close()
            Else
                'creation d'une nouvelle instance du membre xmldocument
                Dim XmlDoc As XmlDocument = New XmlDocument()
                XmlDoc.Load(_MonRepertoire & "\logs\log.xml")
            End If
        End Function

        'Sauvegarder la configuration
        Public Sub SaveConfiguration() Implements IHoMIDom.SaveConfig
            SaveConfig(_MonRepertoire & "\config\homidom.xml")
        End Sub

        'Sauvegarder ou créer un device
        Public Function SaveDevice(ByVal deviceId As String, ByVal name As String, ByVal address1 As String, ByVal enable As Boolean, ByVal solo As Boolean, ByVal driverid As String, ByVal type As String, ByVal refresh As Integer, Optional ByVal address2 As String = "", Optional ByVal image As String = "", Optional ByVal modele As String = "", Optional ByVal description As String = "", Optional ByVal lastchangeduree As Integer = 0) As String Implements IHoMIDom.SaveDevice
            Dim myID As String

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
                        _ListDevices.Item(i).driverid = driverid
                        _ListDevices.Item(i).description = description
                        _ListDevices.Item(i).modele = modele
                        _ListDevices.Item(i).refresh = refresh
                        _ListDevices.Item(i).solo = solo
                        _ListDevices.Item(i).LastChangeDuree = lastchangeduree
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

        'sauvegarde ou créer une zone dans la config
        Function SaveZone(ByVal zoneId As String, ByVal name As String, Optional ByVal ListDevice As ArrayList = Nothing, Optional ByVal icon As String = "", Optional ByVal image As String = "") As String Implements IHoMIDom.SaveZone
            Dim myID As String = ""

            If zoneId = "" Then
                Dim x As New Zone
                With x
                    x.ID = GenerateGUID()
                    x.Name = name
                    x.Icon = icon
                    x.Image = image
                    x.ListDevice = ListDevice
                End With
                myID = x.ID
                _ListZones.Add(x)
            Else
                'zone Existante
                myID = zoneId
                For i As Integer = 0 To _ListZones.Count - 1
                    If _ListZones.Item(i).id = zoneId Then
                        _ListZones.Item(i).name = name
                        _ListZones.Item(i).icon = icon
                        _ListZones.Item(i).image = image
                        _ListZones.Item(i).listdevice = ListDevice
                    End If
                Next
            End If

            'génration de l'event
            Return myID
        End Function

        ''' <summary>
        ''' Supprime une commande IR d'un device
        ''' </summary>
        ''' <param name="deviceId"></param>
        ''' <param name="CmdName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteDeviceCommandIR(ByVal deviceId As String, ByVal CmdName As String) As Integer Implements IHoMIDom.DeleteDeviceCommandIR
            For i As Integer = 0 To _ListDevices.Count - 1
                If _ListDevices.Item(i).Id = deviceId Then
                    For j As Integer = 0 To _ListDevices.Item(i).ListCommandname.count - 1
                        If _ListDevices.Item(i).ListCommandname(j) = CmdName Then
                            _ListDevices.Item(i).ListCommandname.removeat(j)
                            _ListDevices.Item(i).ListCommanddata.removeat(j)
                            _ListDevices.Item(i).ListCommandrepeat.removeat(j)
                            'génération de l'event
                            Exit Function
                        End If
                    Next
                End If
            Next
        End Function

        ''' <summary>
        ''' Ajoute ou modifie une commande IR à un device
        ''' </summary>
        ''' <param name="deviceId"></param>
        ''' <param name="CmdName"></param>
        ''' <param name="CmdData"></param>
        ''' <param name="CmdRepeat"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SaveDeviceCommandIR(ByVal deviceId As String, ByVal CmdName As String, ByVal CmdData As String, ByVal CmdRepeat As String) As String Implements IHoMIDom.SaveDeviceCommandIR
            Dim flag As Boolean

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
        End Function

        ''' <summary>
        ''' Commencer un apprentissage IR
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function StartIrLearning() As String Implements IHoMIDom.StartIrLearning
            Dim retour As String = ""
            For i As Integer = 0 To _ListDrivers.Count - 1
                If _ListDrivers.Item(i).protocol = "IR" Then
                    Dim x As Object = _ListDrivers.Item(i)
                    retour = x.LearnCodeIR()
                    Log(TypeLog.INFO, TypeSource.SERVEUR, "SERVEUR", "Apprentissage IR: " & retour)
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

        'retourne la zone par son ID
        Public Function ReturnZoneById(ByVal ZoneId As String) As Object Implements IHoMIDom.ReturnZoneByID
            Dim retour As Object = Nothing
            For i As Integer = 0 To _ListZones.Count - 1
                If _ListZones.Item(i).ID = ZoneId Then
                    retour = _ListZones.Item(i)
                    Exit For
                End If
            Next
            Return retour
        End Function

        'retourne le driver par son Nom
        Public Function ReturnDriverByNom(ByVal DriverNom As String) As Object Implements IHoMIDom.ReturnDriverByNom
            Dim retour As Object = Nothing
            For i As Integer = 0 To _ListDrivers.Count - 1
                If _ListDrivers.Item(i).Nom = DriverNom.ToUpper() Then
                    retour = _ListDrivers.Item(i)
                    Exit For
                End If
            Next
            Return retour
        End Function

        'retourne une liste de device par son Adresse1 et/ou type et/ou son driver, ex: "A1" "TEMPERATURE" "RFXCOM_RECEIVER"
        Public Function ReturnDeviceByAdresse1TypeDriver(ByVal DeviceAdresse As String, ByVal DeviceType As String, ByVal DriverID As String) As ArrayList Implements IHoMIDom.ReturnDeviceByAdresse1TypeDriver
            Dim retour As Object = Nothing
            Dim listresultat As New ArrayList
            For i As Integer = 0 To _ListDevices.Count - 1
                If (DeviceAdresse = "" Or _ListDevices.Item(i).Adresse1 = DeviceAdresse.ToUpper()) And (DeviceType = "" Or _ListDevices.Item(i).type = DeviceType.ToUpper()) And (DriverID = "" Or _ListDevices.Item(i).DriverID = DriverID.ToUpper()) Then
                    retour = _ListDevices.Item(i)
                    listresultat.Add(retour)
                    retour = Nothing
                End If
            Next
            Return listresultat
        End Function


        ''' <summary>
        ''' Permet d'exécuter une commande Sub d'un Device
        ''' </summary>
        ''' <param name="DeviceId"></param>
        ''' <param name="Command"></param>
        ''' <param name="Param"></param>
        ''' <remarks></remarks>
        Sub ExecuteDeviceCommand(ByVal DeviceId As String, ByVal Command As String, ByVal Param As ArrayList) Implements IHoMIDom.ExecuteDeviceCommand
            Dim _retour As Object
            Dim x As Object
            Try
                x = ReturnDeviceById(DeviceId)
                If x IsNot Nothing Then

                    If Param.Count > 0 Then
                        Select Case Param.Count
                            Case 1
                                _retour = CallByName(x, Command, CallType.Method, Param(0))
                            Case 2
                                _retour = CallByName(x, Command, CallType.Method, Param(0), Param(1))
                            Case 3
                                _retour = CallByName(x, Command, CallType.Method, Param(0), Param(1), Param(2))
                            Case 4
                                _retour = CallByName(x, Command, CallType.Method, Param(0), Param(1), Param(2), Param(3))
                            Case 5
                                _retour = CallByName(x, Command, CallType.Method, Param(0), Param(1), Param(2), Param(3), Param(4))
                        End Select

                    Else
                        CallByName(x, Command, CallType.Method)
                    End If
                End If
            Catch ex As Exception
                MsgBox("Erreur lors du test: " & ex.Message, "Erreur")
            End Try
        End Sub
#End Region

#Region "Declaration de la classe Server"

        ''' <summary>Déclaration de la class Server</summary>
        ''' <remarks></remarks>
        Public Sub New()

        End Sub

        ''' <summary>Démarrage du serveur</summary>
        ''' <remarks></remarks>
        Public Sub start()
            Try
                Dim retour As String

                '----- Démarre les connexions Sqlite ----- 
                retour = sqlite_homidom.connect("homidom")
                If STRGS.Left(retour, 4) = "ERR:" Then
                    Log(TypeLog.ERREUR_CRITIQUE, TypeSource.SERVEUR, "Start", "Erreur lors de la connexion à la BDD Homidom : " & retour)
                    'on arrête tout
                Else
                    Log(TypeLog.INFO, TypeSource.SERVEUR, "Start", "Connexion à la BDD Homidom : " & retour)
                End If

                retour = sqlite_medias.connect("medias")
                If STRGS.Left(retour, 4) = "ERR:" Then
                    Log(TypeLog.ERREUR_CRITIQUE, TypeSource.SERVEUR, "Start", "Erreur lors de la connexion à la BDD Medias : " & retour)
                    'on arrête tout
                Else
                    Log(TypeLog.INFO, TypeSource.SERVEUR, "Start", "Connexion à la BDD Medias : " & retour)
                End If

                '----- Charge les drivers ----- 
                Drivers_Load()

                '----- Chargement de la config ----- 
                retour = LoadConfig(_MonRepertoire & "\Config\")
                Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", retour)

                '----- Démarre les drivers ----- 
                Drivers_Start()
                TimerSecond.Interval = 1000
                AddHandler TimerSecond.Elapsed, AddressOf TimerSecTick
                TimerSecond.Enabled = True

                'Calcul les heures de lever et coucher du soleil
                MAJ_HeuresSoleil()
            Catch ex As Exception
                Log(TypeLog.ERREUR_CRITIQUE, TypeSource.SERVEUR, "Start", "Exception : " & ex.Message)
            End Try
        End Sub

        ''' <summary>Arrêt du serveur</summary>
        ''' <remarks></remarks>
        Public Sub [stop]()
            Try
                Dim retour As String
                TimerSecond.Enabled = False
                TimerSecond.Dispose()

                '----- Arrete les devices ----- 
                Devices_Stop()
                _ListDevices = Nothing

                '----- Arrete les drivers ----- 
                Drivers_Stop()
                _ListDrivers = Nothing

                '----- Arrete les connexions Sqlite -----
                retour = sqlite_homidom.disconnect("homidom")
                If STRGS.Left(retour, 4) = "ERR:" Then
                    Log(TypeLog.ERREUR_CRITIQUE, TypeSource.SERVEUR, "Stop", "Erreur lors de la deconnexion de la BDD Homidom : " & retour)
                End If
                retour = sqlite_medias.disconnect("medias")
                If STRGS.Left(retour, 4) = "ERR:" Then
                    Log(TypeLog.ERREUR_CRITIQUE, TypeSource.SERVEUR, "Stop", "Erreur lors de la deconnexion de la BDD Medias : " & retour)
                End If
            Catch ex As Exception
                Log(TypeLog.ERREUR_CRITIQUE, TypeSource.SERVEUR, "Stop", "Exception : " & ex.Message)
            End Try
        End Sub

        Protected Overrides Sub Finalize()
            'Mettre le Code pour l'arret
            [stop]()
            MyBase.Finalize()
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
        End Enum

        ''' <summary>Ecrit un log dans le fichier log au format xml</summary>
        ''' <param name="TypLog"></param>
        ''' <param name="Source"></param>
        ''' <param name="Fonction"></param>
        ''' <param name="Message"></param>
        ''' <remarks></remarks>
        Public Sub Log(ByVal TypLog As TypeLog, ByVal Source As TypeSource, ByVal Fonction As String, ByVal Message As String)
            Try
                Dim Fichier As FileInfo

                'Vérifie si le fichier log existe sinon le crée
                If File.Exists(_File) Then
                    Fichier = New FileInfo(_File)
                    'Vérifie si le fichier est trop gros si oui, on l'archive
                    If Fichier.Length > _MaxFileSize Then
                        Dim filearchive As String
                        filearchive = STRGS.Left(_File, _File.Length - 4) & Now.ToString("_yyyyMMdd_HHmmss") & ".xml"
                        File.Move(_File, _File)
                    End If
                Else
                    CreateNewFileLog(_File)
                    Fichier = New FileInfo(_File)
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

        ''' <summary>Créer nouveau Fichier (donner chemin complet et nom) log</summary>
        ''' <param name="NewFichier"></param>
        ''' <remarks></remarks>
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