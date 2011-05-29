#Region "Imports"
Imports System
Imports System.IO
Imports System.Xml
Imports System.Xml.XPath
Imports System.Xml.Serialization
Imports System.Reflection
Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.ServiceModel
Imports System.ServiceModel.Description
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web.HttpUtility
#End Region

Namespace HoMIDom

    ''' <summary>Classe Server</summary>
    ''' <remarks></remarks>
    <Serializable()> Public Class Server
        Implements HoMIDom.IHoMIDom 'implémente l'interface dans cette class

#Region "Declaration des variables"

        Private Shared WithEvents _ListDrivers As New ArrayList 'Liste des drivers
        Private Shared _ListImgDrivers As New ArrayList
        Private Shared WithEvents _ListDevices As New ArrayList 'Liste des devices
        Private Shared _ListZones As New ArrayList 'Liste des zones
        Private Shared _ListUsers As New ArrayList 'Liste des users
        Private Shared _ListMacros As New ArrayList 'Liste des macros
        Private Shared _listTriggers As New ArrayList 'Liste de tous les triggers
        Private sqlite_homidom As New Sqlite 'BDD sqlite pour Homidom
        Private sqlite_medias As New Sqlite 'BDD sqlite pour les medias
        Private _MonRepertoire As String = System.Environment.CurrentDirectory 'représente le répertoire de l'application 'Application.StartupPath
        Shared Soleil As New Soleil 'Déclaration class Soleil
        Shared _Longitude As Double 'Longitude
        Shared _Latitude As Double 'latitude
        Private Shared _HeureLeverSoleil As DateTime 'heure du levé du soleil
        Private Shared _HeureCoucherSoleil As DateTime 'heure du couché du soleil
        Shared _HeureLeverSoleilCorrection As Integer 'correction à appliquer sur heure du levé du soleil
        Shared _HeureCoucherSoleilCorrection As Integer 'correction à appliquer sur heure du couché du soleil
        Shared _SMTPServeur As String = "" 'adresse du serveur SMTP
        Shared _SMTPLogin As String = "" 'login du serveur SMTP
        Shared _SMTPassword As String = "" 'password du serveur SMTP
        Shared _SMTPmailEmetteur As String = "" 'adresse mail de l'émetteur
        Private Shared _PortSOAP As String 'Port IP de connexion SOAP
        Dim TimerSecond As New Timers.Timer 'Timer à la seconde
        Private graphe As New graphes(_MonRepertoire + "\Images\Graphes\")
        Shared _DateTimeLastStart As Date = Now
        Private Shared _ListExtensionAudio As New ArrayList 'Liste des extensions audio
        Private Shared _ListRepertoireAudio As New ArrayList 'Liste des répertoires audio
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

        End Sub

        ''' <summary>Evenement provenant des devices</summary>
        ''' <param name="Device"></param>
        ''' <param name="Property"></param>
        ''' <param name="Parametres"></param>
        ''' <remarks></remarks>
        Public Sub DeviceChange(ByVal Device As Object, ByVal [Property] As String, ByVal Parametres As Object)
            Dim retour As String
            Dim listmacro As ArrayList
            Try
                Dim valeur As Object = Parametres
                '--- on logue tout ce qui arrive en mode debug
                Log(TypeLog.DEBUG, TypeSource.SERVEUR, "DeviceChange", "Le device " & Device.name & " a changé : " & [Property] & " = " & valeur)

                If STRGS.Left(valeur, 4) <> "ERR:" Then 'si y a pas erreur d'acquisition
                    '--- Remplacement de , par .
                    valeur = STRGS.Replace(valeur, ",", ".")

                    '------------------------------------------------------------------------------------------------
                    '    MACRO/Triggers
                    '------------------------------------------------------------------------------------------------

                    'Parcour des triggers pour vérifier si le device déclenche des macros
                    For i = 0 To _listTriggers.Count - 1
                        If _listTriggers.Item(i).Conditiondeviceid = Device.ID Then
                            'Device trouvé dans un trigger, on parcour la liste des macros associé à ce trigger et on les executent
                            listmacro = _listTriggers.Item(i).Macro
                            For j = 0 To listmacro.Count - 1
                                'on cherche la macro et on la lance
                                For k = 0 To _ListMacros.Count - 1
                                    If _ListMacros.Item(k).ID = listmacro.Item(j).ToString Then _ListMacros.Item(k).Execute_avec_conditions()
                                Next
                            Next
                        End If
                    Next

                    '------------------------------------------------------------------------------------------------
                    '    HISTORIQUE
                    '------------------------------------------------------------------------------------------------

                    '--- si on teste la value (et non les autres propriétés d'un device) et si lastetat=True, on vérifie que la valeur a changé par rapport a l'avant dernier etat (valuelast) 
                    If [Property] = "Value" Then
                        '--- si c'est un nombre
                        If (IsNumeric(valeur) And IsNumeric(Device.Value) And IsNumeric(Device.ValueLast)) Then
                            '--- si lastetat=True, on vérifie que la valeur a changé par rapport a l'avant dernier etat (valuelast) 
                            If Device.LastEtat And valeur.ToString = Device.ValueLast Then
                                'log de "inchangé lastetat"
                                Log(TypeLog.VALEUR_INCHANGE_LASTETAT, TypeSource.SERVEUR, "DeviceChange", Device.Name.ToString() & " : " & Device.Adresse1 & " : " & valeur & " (inchangé lastetat " & Device.ValueLast & ")")
                            Else
                                '--- on vérifie que la valeur a changé de plus de composants_precision sinon inchangé
                                If (CDbl(valeur) + CDbl(Device.Precision)) >= CDbl(Device.Value) And (CDbl(valeur) - CDbl(Device.Precision)) <= CDbl(Device.Value) Then
                                    'log de "inchangé précision"
                                    Log(TypeLog.VALEUR_INCHANGE_PRECISION, TypeSource.SERVEUR, "DeviceChange", Device.Name.ToString() & " : " & Device.Adresse1 & " : " & valeur & " (inchangé precision " & Device.ValueLast & ")")
                                Else
                                    'log de la nouvelle valeur
                                    Log(TypeLog.VALEUR_CHANGE, TypeSource.SERVEUR, "DeviceChange", Device.Name.ToString() & " : " & Device.Adresse1 & " : " & valeur)
                                    'On historise la nouvellevaleur
                                    'retour = sqlite_homidom.nonquery("INSERT INTO historiques (device_id,source,dateheure,valeur) VALUES ('" & Device.ID & "','" & [Property] & "','" & Now.ToString() & "','" & valeur & "')")
                                    retour = sqlite_homidom.nonquery("INSERT INTO historiques (device_id,source,dateheure,valeur) VALUES ('@parameter0','@parameter1','@parameter2','@parameter3')", Device.ID, [Property], Now.ToString(), valeur)
                                    If STRGS.Left(retour, 4) = "ERR:" Then
                                        Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeviceChange", "Erreur lors Requete sqlite : " & retour)
                                    End If
                                End If
                            End If
                        Else
                            '--- Valeur est autre chose qu'un nombre
                            '--- historise la valeur si ce n'est pas une simple info de config
                            If STRGS.Left(valeur, 4) <> "CFG:" Then
                                '--- si lastetat=True, on vérifie que la valeur a changé par rapport a l'avant dernier etat (valuelast) 
                                If Device.LastEtat And valeur.ToString = Device.ValueLast Then
                                    'log de "inchangé lastetat"
                                    Log(TypeLog.VALEUR_INCHANGE_LASTETAT, TypeSource.SERVEUR, "DeviceChange", Device.Name.ToString() & " : " & Device.Adresse1 & " : " & valeur & " (inchangé lastetat " & Device.ValueLast & ")")
                                Else
                                    'log de la nouvelle valeur
                                    Log(TypeLog.VALEUR_CHANGE, TypeSource.SERVEUR, "DeviceChange", Device.Name.ToString() & " : " & Device.Adresse1 & " : " & valeur)
                                    'Ajout dans la BDD
                                    'retour = sqlite_homidom.nonquery("INSERT INTO historiques (device_id,source,dateheure,valeur) VALUES ('" & Device.ID & "','" & [Property] & "','" & Now.ToString() & "','" & valeur & "')")
                                    retour = sqlite_homidom.nonquery("INSERT INTO historiques (device_id,source,dateheure,valeur) VALUES ('@parameter0','@parameter1','@parameter2','@parameter3')", Device.ID, [Property], Now.ToString(), valeur)
                                    If STRGS.Left(retour, 4) = "ERR:" Then
                                        Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeviceChange", "Erreur lors Requete sqlite : " & retour)
                                    End If
                                End If
                            Else
                                'log de la nouvelle valeur
                                Log(TypeLog.VALEUR_CHANGE, TypeSource.SERVEUR, "DeviceChange", Device.Name.ToString() & " : " & Device.Adresse1 & " : " & valeur)
                            End If
                        End If
                    Else
                        'C'est une autre propriété, on logue directement et stocke la modif
                        Log(TypeLog.VALEUR_CHANGE, TypeSource.SERVEUR, "DeviceChange", Device.Name.ToString() & " : " & Device.Adresse1 & " : " & valeur & " (" & [Property] & ")")
                        'Ajout dans la BDD
                        'retour = sqlite_homidom.nonquery("INSERT INTO historiques (device_id,source,dateheure,valeur) VALUES ('" & Device.ID & "','" & [Property] & "','" & Now.ToString() & "','" & valeur & "')")
                        retour = sqlite_homidom.nonquery("INSERT INTO historiques (device_id,source,dateheure,valeur) VALUES ('@parameter0','@parameter1','@parameter2','@parameter3')", Device.ID, [Property], Now.ToString(), valeur)
                        If STRGS.Left(retour, 4) = "ERR:" Then
                            Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeviceChange", "Erreur lors Requete sqlite : " & retour)
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
            Dim ladate As DateTime = Now 'on récupére la date/heure

            '---- Action à effectuer toutes les secondes ----
            'on checke si il y a cron à faire
            For i = 0 To _listTriggers.Count() - 1
                If (_listTriggers.Item(i).prochainedateheure IsNot Nothing And _listTriggers.Item(i).prochainedateheure <= DateAndTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) Then
                    _listTriggers.Item(i).maj_cron() 'reprogrammation du prochain shedule
                    'lancement des macros associées
                    For j = 0 To _listTriggers.Item(i).ListMacro.Count - 1
                        'on cherche la macro et on la lance en testant ces conditions
                        For k = 0 To _ListMacros.Count - 1
                            If _ListMacros.Item(k).ID = _listTriggers.Item(i).Macro.Item(j).ToString Then _ListMacros.Item(k).Execute_avec_conditions()
                        Next
                    Next
                End If
            Next

            '---- Actions à effectuer toutes les minutes ----
            If ladate.Second = 1 Then

            End If

            '---- Actions à effectuer toutes les heures ----
            If ladate.Minute = 59 And ladate.Second = 59 Then

            End If

            '---- Actions à effectuer à minuit ----
            If ladate.Hour = 0 And ladate.Minute = 0 And ladate.Second = 0 Then
                MAJ_HeuresSoleil()
            End If

            '---- Actions à effectuer à midi ----
            If ladate.Hour = 12 And ladate.Minute = 0 And ladate.Second = 0 Then
                MAJ_HeuresSoleil()
            End If
        End Sub

#End Region

#Region "Fonctions/Sub propres au serveur"

#Region "Soleil"
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
                If File.Exists(_file & ".bak") = True Then File.Delete(_file & ".bak")
                File.Copy(_file & ".xml", Mid(_file & ".xml", 1, Len(_file & ".xml") - 4) & ".bak")
                Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Création du backup (.bak) du fichier de config avant chargement")
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
                                    Case "portsoap"
                                        _PortSOAP = list.Item(0).Attributes.Item(j).Value
                                    Case "smtpserver"
                                        _SMTPServeur = list.Item(0).Attributes.Item(j).Value
                                    Case "smtpmail"
                                        _SMTPmailEmetteur = list.Item(0).Attributes.Item(j).Value
                                    Case "smtplogin"
                                        _SMTPLogin = list.Item(0).Attributes.Item(j).Value
                                    Case "smtppassword"
                                        _SMTPassword = list.Item(0).Attributes.Item(j).Value
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
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Chargement des drivers")
                        list = Nothing
                        list = myxml.SelectNodes("/homidom/drivers/driver")

                        If list.Count > 0 Then 'présence d'un ou des driver(s)
                            For j As Integer = 0 To list.Count - 1
                                'on récupère l'id du driver
                                Dim _IdDriver = list.Item(j).Attributes.Item(0).Value
                                Dim _drv As IDriver = ReturnDrvById(_IdDriver)

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

                                    For i As Integer = 0 To list.Item(j).Attributes.Count - 1
                                        Dim a As String = UCase(list.Item(j).Attributes.Item(i).Name)
                                        If a.StartsWith("PARAMETRE") Then
                                            Dim idx As Integer = Mid(a, 10, Len(a) - 9)
                                            _drv.Parametres.Item(idx).valeur = list.Item(j).Attributes.Item(i).Value
                                        End If
                                        a = Nothing
                                    Next

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
                                                Dim _dev As New Zone.Element_Zone(list.Item(i).ChildNodes.Item(k).ChildNodes.Item(k1).Attributes(0).Value, list.Item(i).ChildNodes.Item(k).ChildNodes.Item(k1).Attributes(1).Value, list.Item(i).ChildNodes.Item(k).ChildNodes.Item(k1).Attributes(2).Value, list.Item(i).ChildNodes.Item(k).ChildNodes.Item(k1).Attributes(3).Value)
                                                x.ListElement.Add(_dev)
                                            Next
                                        End If
                                    Next
                                End If
                                _ListZones.Add(x)
                            Next
                        Else
                            Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Aucune zone enregistrée dans le fichier de config")
                        End If
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", _ListZones.Count & " Zone(s) chargée(s)")

                        '******************************************
                        'on va chercher les users
                        '******************************************
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Chargement des users")
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
                                            x.Image = list.Item(i).Attributes.Item(j).Value
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
                                            Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Un attribut correspondant à la zone est inconnu: nom:" & list.Item(i).Attributes.Item(j).Name & " Valeur: " & list.Item(0).Attributes.Item(j).Value)
                                    End Select
                                Next
                                _ListUsers.Add(x)
                            Next
                        Else
                            Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Création du user admin par défaut !!")
                            SaveUser("", "Admin", "password", Users.TypeProfil.admin, "Administrateur", "Admin")
                        End If
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", _ListUsers.Count & " Users(s) chargé(s)")


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
                            Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Aucun device enregistré dans le fichier de config")
                        End If
                        list = Nothing

                        '******************************************
                        'on va chercher les triggers
                        '******************************************
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Chargement des triggers")
                        list = Nothing
                        list = myxml.SelectNodes("/homidom/triggers/trigger")
                        If list.Count > 0 Then 'présence des triggers
                            For i As Integer = 0 To list.Count - 1
                                Dim x As New Trigger
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
                                            Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Un attribut correspondant au trigger est inconnu: nom:" & list.Item(i).Attributes.Item(j1).Name & " Valeur: " & list.Item(0).Attributes.Item(j1).Value)
                                    End Select
                                    If list.Item(i).HasChildNodes = True Then
                                        If list.Item(i).ChildNodes.Item(0).Name = "macros" And list.Item(i).ChildNodes.Item(0).HasChildNodes Then
                                            For k = 0 To list.Item(i).ChildNodes.Item(0).ChildNodes.Count - 1
                                                If list.Item(i).ChildNodes.Item(0).ChildNodes.Item(k).Name = "macro" Then
                                                    If list.Item(i).ChildNodes.Item(0).ChildNodes.Item(k).Attributes.Count = 0 And list.Item(i).ChildNodes.Item(0).ChildNodes.Item(k).Attributes.Item(0).Name = "id" Then
                                                        x.ListMacro.Add(list.Item(i).ChildNodes.Item(0).ChildNodes.Item(k).Attributes.Item(0).Value)
                                                    End If
                                                End If
                                            Next
                                        End If
                                    End If
                                Next
                                _listTriggers.Add(x)
                            Next
                        Else
                            Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Aucun trigger enregistré dans le fichier de config")
                        End If
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", _listTriggers.Count & " Trigger(s) chargé(s)")
                        list = Nothing

                        '******************************************
                        'on va chercher les macros
                        '******************************************
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Chargement des macros")
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
                                            Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Un attribut correspondant à la macro est inconnu: nom:" & list.Item(i).Attributes.Item(j1).Name & " Valeur: " & list.Item(0).Attributes.Item(j1).Value)
                                    End Select
                                Next
                                If list.Item(i).HasChildNodes Then
                                    For j2 As Integer = 0 To list.Item(i).ChildNodes.Count - 1
                                        If list.Item(i).ChildNodes.Item(j2).Name = "action" Then
                                            Dim _Act As Object = Nothing
                                            Select Case list.Item(i).ChildNodes.Item(j2).Attributes.Item(0).Value
                                                Case "ActionDevice"
                                                    Dim o As New Action.ActionDevice
                                                    _Act = o
                                                    o = Nothing
                                            End Select
                                            For j3 As Integer = 0 To list.Item(i).ChildNodes.Item(j2).Attributes.Count - 1
                                                Select Case list.Item(i).ChildNodes.Item(j2).Attributes.Item(j3).Name
                                                    Case "timing"
                                                        _Act.timing = CDate(list.Item(i).ChildNodes.Item(j2).Attributes.Item(j3).Value)
                                                    Case "iddevice"
                                                        _Act.iddevice = list.Item(i).ChildNodes.Item(j2).Attributes.Item(j3).Value
                                                    Case "method"
                                                        _Act.method = list.Item(i).ChildNodes.Item(j2).Attributes.Item(j3).Value
                                                    Case "parametres"
                                                        Dim b As String = list.Item(i).ChildNodes.Item(j2).Attributes.Item(j3).Value
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
                                            x.ListActions.Add(_Act)
                                        End If
                                    Next
                                End If
                                _ListMacros.Add(x)
                            Next
                        Else
                            Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Aucune macro enregistrée dans le fichier de config")
                        End If
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", _ListMacros.Count & " Macro(s) chargée(s)")
                        list = Nothing

                        '******************************************
                        'on va chercher des extensions audios
                        '******************************************
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Chargement des extensions audio")
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
                                            Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Un attribut correspondant à une extension audio est inconnu: nom:" & list.Item(i).Attributes.Item(j).Name & " Valeur: " & list.Item(0).Attributes.Item(j).Value)
                                    End Select
                                Next
                                _ListExtensionAudio.Add(x)
                            Next
                        End If
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", _ListExtensionAudio.Count & " Extension(s) Audio chargée(s)")
                        list = Nothing

                        '******************************************
                        'on va chercher les répertoires audios
                        '******************************************
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Chargement des répertoires audio")
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
                                            Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Un attribut correspondant à un répertoire audio est inconnu: nom:" & list.Item(i).Attributes.Item(j).Name & " Valeur: " & list.Item(0).Attributes.Item(j).Value)
                                    End Select
                                Next
                                _ListRepertoireAudio.Add(x)
                            Next
                        End If
                        Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", _ListRepertoireAudio.Count & " Répertoire(s) Audio chargé(s)")
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
                writer.WriteStartAttribute("portsoap")
                writer.WriteValue(_PortSOAP)
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
                    If _ListZones.Item(i).ListElement IsNot Nothing Then
                        For j As Integer = 0 To _ListZones.Item(i).ListElement.count - 1
                            writer.WriteStartElement("element")
                            writer.WriteStartAttribute("elementid")
                            writer.WriteValue(_ListZones.Item(i).ListElement.item(j).elementid)
                            writer.WriteEndAttribute()
                            writer.WriteStartAttribute("visible")
                            writer.WriteValue(_ListZones.Item(i).ListElement.item(j).visible)
                            writer.WriteEndAttribute()
                            writer.WriteStartAttribute("X")
                            writer.WriteValue(_ListZones.Item(i).ListElement.item(j).x)
                            writer.WriteEndAttribute()
                            writer.WriteStartAttribute("Y")
                            writer.WriteValue(_ListZones.Item(i).ListElement.item(j).y)
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
                    writer.WriteValue(_ListUsers.Item(i).id)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("username")
                    writer.WriteValue(_ListUsers.Item(i).username)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("nom")
                    writer.WriteValue(_ListUsers.Item(i).nom)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("prenom")
                    writer.WriteValue(_ListUsers.Item(i).prenom)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("profil")
                    Select Case _ListUsers.Item(i).profil
                        Case Users.TypeProfil.invite
                            writer.WriteValue("0")
                        Case Users.TypeProfil.user
                            writer.WriteValue("1")
                        Case Users.TypeProfil.admin
                            writer.WriteValue("2")
                    End Select
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("password")
                    writer.WriteValue(_ListUsers.Item(i).password)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("numberidentification")
                    writer.WriteValue(_ListUsers.Item(i).numberidentification)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("image")
                    writer.WriteValue(_ListUsers.Item(i).image)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("email")
                    writer.WriteValue(_ListUsers.Item(i).email)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("emailautre")
                    writer.WriteValue(_ListUsers.Item(i).emailautre)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("telfixe")
                    writer.WriteValue(_ListUsers.Item(i).telfixe)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("telmobile")
                    writer.WriteValue(_ListUsers.Item(i).telmobile)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("telautre")
                    writer.WriteValue(_ListUsers.Item(i).telautre)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("adresse")
                    writer.WriteValue(_ListUsers.Item(i).adresse)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("ville")
                    writer.WriteValue(_ListUsers.Item(i).ville)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("codepostal")
                    writer.WriteValue(_ListUsers.Item(i).codepostal)
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
                    writer.WriteValue(_ListExtensionAudio.Item(i).extension)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("enable")
                    writer.WriteValue(_ListExtensionAudio.Item(i).enable)
                    writer.WriteEndAttribute()
                    writer.WriteEndElement()
                Next
                For i As Integer = 0 To _ListRepertoireAudio.Count - 1
                    writer.WriteStartElement("repertoire")
                    writer.WriteStartAttribute("repertoire")
                    writer.WriteValue(_ListRepertoireAudio.Item(i).repertoire)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("enable")
                    writer.WriteValue(_ListRepertoireAudio.Item(i).enable)
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
                ''Sauvegarde des triggers
                ''------------
                Log(TypeLog.INFO, TypeSource.SERVEUR, "SaveConfig", "Sauvegarde des triggers")
                writer.WriteStartElement("triggers")
                For i As Integer = 0 To _listTriggers.Count - 1
                    writer.WriteStartElement("trigger")
                    writer.WriteStartAttribute("id")
                    writer.WriteValue(_listTriggers.Item(i).id)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("nom")
                    writer.WriteValue(_listTriggers.Item(i).nom)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("description")
                    writer.WriteValue(_listTriggers.Item(i).description)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("enable")
                    writer.WriteValue(_listTriggers.Item(i).enable)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("type")
                    If _listTriggers.Item(i).type = Trigger.TypeTrigger.TIMER Then
                        writer.WriteValue("0")
                    Else
                        writer.WriteValue("1")
                    End If
                    writer.WriteEndAttribute()
                    If _listTriggers.Item(i).type = Trigger.TypeTrigger.TIMER Then
                        writer.WriteStartAttribute("conditiontime")
                        writer.WriteValue(_listTriggers.Item(i).conditiontime)
                        writer.WriteEndAttribute()
                        writer.WriteStartAttribute("prochainedateheure")
                        writer.WriteValue(_listTriggers.Item(i).prochainedateheure)
                        writer.WriteEndAttribute()
                    End If
                    If _listTriggers.Item(i).type = Trigger.TypeTrigger.DEVICE Then
                        writer.WriteStartAttribute("conditiondeviceid")
                        writer.WriteValue(_listTriggers.Item(i).conditiondeviceid)
                        writer.WriteEndAttribute()
                        writer.WriteStartAttribute("conditiondeviceproperty")
                        writer.WriteValue(_listTriggers.Item(i).conditiondeviceproperty)
                        writer.WriteEndAttribute()
                    End If
                    writer.WriteStartElement("macros")
                    For k = 0 To _listTriggers.Item(i).listmacro.count - 1
                        writer.WriteStartElement("macro")
                        writer.WriteStartAttribute("id")
                        writer.WriteValue(_listTriggers.Item(i).listmacro.item(k).id)
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
                    writer.WriteValue(_ListMacros.Item(i).id)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("nom")
                    writer.WriteValue(_ListMacros.Item(i).nom)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("description")
                    writer.WriteValue(_ListMacros.Item(i).description)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("enable")
                    writer.WriteValue(_ListMacros.Item(i).enable)
                    writer.WriteEndAttribute()
                    For j As Integer = 0 To _ListMacros.Item(i).ListActions.count - 1
                        writer.WriteStartElement("action")
                        writer.WriteStartAttribute("typeaction")
                        writer.WriteValue(_ListMacros.Item(i).listactions.item(j).TypeAction.ToString)
                        writer.WriteEndAttribute()
                        writer.WriteStartAttribute("timing")
                        writer.WriteValue(_ListMacros.Item(i).listactions.item(j).timing)
                        writer.WriteEndAttribute()
                        Select Case _ListMacros.Item(i).listactions.item(j).TypeAction
                            Case Action.TypeAction.ActionDevice
                                writer.WriteStartAttribute("iddevice")
                                writer.WriteValue(_ListMacros.Item(i).listactions.item(j).IdDevice)
                                writer.WriteEndAttribute()
                                writer.WriteStartAttribute("method")
                                writer.WriteValue(_ListMacros.Item(i).listactions.item(j).Method)
                                writer.WriteEndAttribute()
                                Dim a As String = ""
                                For k As Integer = 0 To _ListMacros.Item(i).listactions.item(j).parametres.count - 1
                                    a = a & _ListMacros.Item(i).listactions.item(j).parametres.item(k) & "|"
                                Next
                                writer.WriteStartAttribute("parametres")
                                writer.WriteValue(a)
                                writer.WriteEndAttribute()
                        End Select
                            writer.WriteEndElement()
                    Next
                    writer.WriteEndElement()
                Next
                writer.WriteEndElement()
                ''FIN DES ELEMENTS------------

                writer.WriteEndDocument()
                writer.Close()
                Log(TypeLog.INFO, TypeSource.SERVEUR, "SaveConfig", "Sauvegarde terminée")
            Catch ex As Exception
                MsgBox("ERREUR SAVECONFIG " & ex.ToString, MsgBoxStyle.Exclamation, "Erreur serveur")
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SaveConfig", " Erreur de sauvegarde de la configuration: " & ex.Message)
            End Try

        End Sub
#End Region

#Region "Device"
        ''' <summary>Arretes les devices (Handlers)</summary>
        ''' <remarks></remarks>
        Public Sub Devices_Stop()
            Try
                'Cherche tous les devices chargés
                Log(TypeLog.INFO, TypeSource.SERVEUR, "Devices_Stop", "Arrêt des devices")
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
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Devices_Stop", " Erreur lors de l'arret des devices: " & ex.Message)
            End Try
        End Sub

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
#End Region

#Region "Driver"
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
                    tabl.Add(_ListDrivers.Item(i).Parametres)
                    Return tabl
                    Exit For
                End If
            Next
            Return Nothing
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
                        Case "PARAMETRES"
                            For idx As Integer = 0 To Parametre.count - 1
                                _ListDrivers.Item(i).Parametres.item(idx).valeur = Parametre(idx)
                            Next
                        Case "DELETEDEVICE"
                            _ListDrivers.Item(i).DeleteDevice(Parametre)
                        Case "NEWDEVICE"
                            _ListDrivers.Item(i).NewDevice(Parametre)
                    End Select
                    Exit For

                End If
            Next
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
                MsgBox("Erreur lors du chargement des drivers: " & ex.Message, MsgBoxStyle.Exclamation, "Erreur Serveur")
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
                MsgBox("Erreur lors du démarrage des drivers: " & ex.Message, MsgBoxStyle.Exclamation, "Erreur Serveur")
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
                MsgBox("Erreur lors de l arret des drivers: " & ex.Message, MsgBoxStyle.Exclamation, "Erreur Serveur")
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Drivers_Stop", " Erreur lors de l'arret des drivers: " & ex.Message)
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
        End Function

        ''' <summary>Décrypter un string</summary>
        ''' <param name="sOut"></param>
        ''' <param name="sKey"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DecryptTripleDES(ByVal sOut As String, ByVal sKey As String) As String
            Dim DES As New System.Security.Cryptography.TripleDESCryptoServiceProvider()
            Dim hashMD5 As New System.Security.Cryptography.MD5CryptoServiceProvider

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
        End Function

        Private Shared Function ScrambleKey(ByVal v_strKey As String) As String
            Dim sbKey As New System.Text.StringBuilder
            Dim intPtr As Integer
            For intPtr = 1 To v_strKey.Length
                Dim intIn As Integer = v_strKey.Length - intPtr + 1
                sbKey.Append(Mid(v_strKey, intIn, 1))
            Next
            Dim strKey As String = sbKey.ToString
            Return sbKey.ToString
        End Function
#End Region

#Region "Log"
        Dim _File As String = _MonRepertoire & "\logs\log.xml" 'Représente le fichier log: ex"C:\homidom\log\log.xml"
        Dim _MaxFileSize As Long = 5120 'en Koctets

        ''' <summary>
        ''' Permet de connaître le chemin du fichier log
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property FichierLog() As String
            Get
                Return _File
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

        ''' <summary>Ecrit un log dans le fichier log au format xml</summary>
        ''' <param name="TypLog"></param>
        ''' <param name="Source"></param>
        ''' <param name="Fonction"></param>
        ''' <param name="Message"></param>
        ''' <remarks></remarks>
        Public Sub Log(ByVal TypLog As TypeLog, ByVal Source As TypeSource, ByVal Fonction As String, ByVal Message As String) Implements IHoMIDom.Log
            Try
                Dim Fichier As FileInfo

                'Vérifie si le fichier log existe sinon le crée
                If File.Exists(_File) Then
                    Fichier = New FileInfo(_File)
                    'Vérifie si le fichier est trop gros si oui, on l'archive
                    If (Fichier.Length / 1000) > _MaxFileSize Then
                        Dim filearchive As String
                        filearchive = STRGS.Left(_File, _File.Length - 4) & Now.ToString("_yyyyMMdd_HHmmss") & ".xml"
                        File.Move(_File, filearchive)
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
                    elelog.SetAttribute("fonction", HtmlEncode(Fonction))
                    elelog.SetAttribute("message", HtmlEncode(Message))

                    Dim root As XmlElement = xmldoc.Item("logs")
                    root.AppendChild(elelog)

                    'on enregistre le fichier xml
                    xmldoc.Save(_File)

                Catch ex As Exception 'Le fichier xml est corrompu ou comporte des caractères non supportés par xml
                    Console.WriteLine(Now & " Impossible d'écrire dans le fichier log un nouveau fichier à été créé: " & ex.Message)
                    Dim filearchive As String
                    filearchive = STRGS.Left(_File, _File.Length - 4) & Now.ToString("_yyyyMMdd_HHmmss") & ".xml"
                    File.Move(_File, filearchive)
                    CreateNewFileLog(_File)
                    Fichier = New FileInfo(_File)
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
                    elelog.SetAttribute("message", HtmlEncode(Message))

                    Dim root As XmlElement = xmldoc.Item("logs")
                    root.AppendChild(elelog)

                    'on enregistre le fichier xml
                    xmldoc.Save(_File)
                End Try

                Fichier = Nothing
            Catch ex As Exception
                MsgBox("Erreur lors de l'écriture d'un log: " & ex.Message, MsgBoxStyle.Exclamation, "Erreur Serveur")
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

#Region "Declaration de la classe Server"

        ''' <summary>Déclaration de la class Server</summary>
        ''' <remarks></remarks>
        Public Sub New()

        End Sub

        ''' <summary>Démarrage du serveur</summary>
        ''' <remarks></remarks>
        Public Sub start() Implements IHoMIDom.Start
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

                '----- Calcul les heures de lever et coucher du soleil ----- 
                MAJ_HeuresSoleil()

                '----- Maj des triggers type CRON ----- 
                For i = 0 To _listTriggers.Count - 1
                    'on vérifie si la condition est un cron
                    If _listTriggers.Item(i).type = Trigger.TypeTrigger.TIMER Then
                        _listTriggers.Item(i).maj_cron() 'on calcule la date de prochain execution
                    End If
                Next

                '----- Démarre le Timer -----
                TimerSecond.Interval = 1000
                AddHandler TimerSecond.Elapsed, AddressOf TimerSecTick
                TimerSecond.Enabled = True

                'test avec graphe
                graphe.grapher_courbe("test", "Temperature extérieure", 800, 400)

            Catch ex As Exception
                Log(TypeLog.ERREUR_CRITIQUE, TypeSource.SERVEUR, "Start", "Exception : " & ex.Message)
            End Try
        End Sub

        ''' <summary>Arrêt du serveur</summary>
        ''' <remarks></remarks>
        Public Sub [stop]() Implements IHoMIDom.Stop
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
            '[stop]()
            MyBase.Finalize()
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
        ''' Retourne la date et heure du dernier démarrage du serveur
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetLastStartTime() As Date Implements IHoMIDom.GetLastStartTime
            Return _DateTimeLastStart
        End Function

        ''' <summary>
        ''' Retourne la version du serveur
        ''' </summary>
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
                Return ""
            End Try
        End Function

        ''' <summary>
        ''' Permet d'envoyer un message d'un client vers le server
        ''' </summary>
        ''' <param name="Message"></param>
        ''' <remarks></remarks>
        Public Sub MessageToServer(ByVal Message As String) Implements IHoMIDom.MessageToServer
            'traiter le message
        End Sub

        ''' <summary>
        ''' Convert a file on a byte array.
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetByteFromImage(ByVal file As String) As Byte() Implements IHoMIDom.GetByteFromImage
            Dim array As Byte()
            Try
                Using fs As New FileStream(file, FileMode.Open, FileAccess.Read)
                    Dim reader As New BinaryReader(fs)
                    array = reader.ReadBytes(CInt(fs.Length))
                    reader.Close()
                End Using
                Return array
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetByteFromImage", ex.Message)
                Return Nothing
            End Try
        End Function

#End Region

#Region "Audio"
        ''' <summary>
        ''' Supprimer une extension Audio
        ''' </summary>
        ''' <param name="NomExtension"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteExtensionAudio(ByVal NomExtension As String) As Integer Implements IHoMIDom.DeleteExtensionAudio
            Dim retour As Integer = -1
            For i As Integer = 0 To _ListExtensionAudio.Count - 1
                If NomExtension = _ListExtensionAudio.Item(i).extension Then
                    _ListExtensionAudio.RemoveAt(i)
                    retour = 0
                    Exit For
                End If
            Next
            Return retour
        End Function

        ''' <summary>
        ''' Ajouter une nouvelle extension audio
        ''' </summary>
        ''' <param name="NomExtension"></param>
        ''' <param name="Enable"></param>
        ''' <returns>-1 si déjà existant</returns>
        ''' <remarks></remarks>
        Public Function NewExtensionAudio(ByVal NomExtension As String, Optional ByVal Enable As Boolean = False) As Integer Implements IHoMIDom.NewExtensionAudio
            For i As Integer = 0 To _ListExtensionAudio.Count - 1
                If _ListExtensionAudio.Item(i).extension = NomExtension Then
                    Return -1
                    Exit Function
                End If
            Next
            Dim x As New Audio.ExtensionAudio
            x.Extension = NomExtension
            x.Enable = Enable
            _ListExtensionAudio.Add(x)
            Return 0
        End Function

        ''' <summary>
        ''' Active ou désactive une extension Audio
        ''' </summary>
        ''' <param name="NomExtension"></param>
        ''' <param name="Enable"></param>
        ''' <returns>-1 si Extension non trouvée</returns>
        ''' <remarks></remarks>
        Public Function EnableExtensionAudio(ByVal NomExtension As String, ByVal Enable As Boolean) As Integer Implements IHoMIDom.EnableExtensionAudio
            Dim retour As Integer = -1
            For i As Integer = 0 To _ListExtensionAudio.Count - 1
                If _ListExtensionAudio.Item(i).extension = NomExtension Then
                    _ListExtensionAudio.Item(i).enable = Enable
                    retour = 0
                End If
            Next
            Return retour
        End Function

        ''' <summary>
        ''' Supprimer un répertoire Audio
        ''' </summary>
        ''' <param name="NomRepertoire"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteRepertoireAudio(ByVal NomRepertoire As String) As Integer Implements IHoMIDom.DeleteRepertoireAudio
            Dim retour As Integer = -1
            For i As Integer = 0 To _ListRepertoireAudio.Count - 1
                If NomRepertoire = _ListRepertoireAudio.Item(i).repertoire Then
                    _ListRepertoireAudio.RemoveAt(i)
                    retour = 0
                    Exit For
                End If
            Next
            Return retour
        End Function

        ''' <summary>
        ''' Ajouter un nouveau répertoire audio
        ''' </summary>
        ''' <param name="NomRepertoire"></param>
        ''' <param name="Enable"></param>
        ''' <returns>-1 si déjà existant</returns>
        ''' <remarks></remarks>
        Public Function NewRepertoireAudio(ByVal NomRepertoire As String, Optional ByVal Enable As Boolean = False) As Integer Implements IHoMIDom.NewRepertoireAudio
            For i As Integer = 0 To _ListRepertoireAudio.Count - 1
                If _ListRepertoireAudio.Item(i).repertoire = NomRepertoire Then
                    Return -1
                    Exit Function
                End If
            Next
            Dim x As New Audio.RepertoireAudio
            x.Repertoire = NomRepertoire
            x.Enable = Enable
            _ListRepertoireAudio.Add(x)
            Return 0
        End Function

        ''' <summary>
        ''' Active ou désactive un répertoire Audio
        ''' </summary>
        ''' <param name="NomRepertoire"></param>
        ''' <param name="Enable"></param>
        ''' <returns>-1 si répertoire non trouvé</returns>
        ''' <remarks></remarks>
        Public Function EnableRepertoireAudio(ByVal NomRepertoire As String, ByVal Enable As Boolean) As Integer Implements IHoMIDom.EnableRepertoireAudio
            Dim retour As Integer = -1
            For i As Integer = 0 To _ListRepertoireAudio.Count - 1
                If _ListRepertoireAudio.Item(i).repertoire = NomRepertoire Then
                    _ListRepertoireAudio.Item(i).enable = Enable
                    retour = 0
                End If
            Next
            Return retour
        End Function

        ''' <summary>Retourne la liste de tous les répertoires audio</summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetAllRepertoiresAudio() As List(Of Audio.RepertoireAudio) Implements IHoMIDom.GetAllRepertoiresAudio
            Try
                Dim _list As New List(Of Audio.RepertoireAudio)
                For i As Integer = 0 To _ListRepertoireAudio.Count - 1
                    Dim x As New Audio.RepertoireAudio
                    With x
                        .Repertoire = _ListRepertoireAudio.Item(i).repertoire
                        .Enable = _ListRepertoireAudio.Item(i).enable
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
        Function GetAllExtensionsAudio() As List(Of Audio.ExtensionAudio) Implements IHoMIDom.GetAllExtensionsAudio
            Try
                Dim _list As New List(Of Audio.ExtensionAudio)
                For i As Integer = 0 To _ListExtensionAudio.Count - 1
                    Dim x As New Audio.ExtensionAudio
                    With x
                        .Extension = _ListExtensionAudio.Item(i).extension
                        .Enable = _ListExtensionAudio.Item(i).enable
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
        Public Function GetSMTPServeur() As String Implements IHoMIDom.GetSMTPServeur
            Try
                Return _SMTPServeur
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetSMTPServeur", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>Fixe l'adresse du serveur SMTP</summary>
        Public Sub SetSMTPServeur(ByVal Value As String) Implements IHoMIDom.SetSMTPServeur
            Try
                _SMTPServeur = Value
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SetSMTPServeur", "Exception : " & ex.Message)
            End Try
        End Sub

        ''' <summary>Retourne le login du serveur SMTP</summary>
        Public Function GetSMTPLogin() As String Implements IHoMIDom.GetSMTPLogin
            Try
                Return _SMTPLogin
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetSMTPLogin", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>Fixe le login du serveur SMTP</summary>
        Public Sub SetSMTPLogin(ByVal Value As String) Implements IHoMIDom.SetSMTPLogin
            Try
                _SMTPLogin = Value
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SetSMTPLogin", "Exception : " & ex.Message)
            End Try
        End Sub

        ''' <summary>Retourne le password du serveur SMTP</summary>
        Public Function GetSMTPPassword() As String Implements IHoMIDom.GetSMTPPassword
            Try
                Return _SMTPassword
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetSMTPPassword", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>Fixe le password du serveur SMTP</summary>
        Public Sub SetSMTPPassword(ByVal Value As String) Implements IHoMIDom.SetSMTPPassword
            Try
                _SMTPassword = Value
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SetSMTPPassword", "Exception : " & ex.Message)
            End Try
        End Sub

        ''' <summary>Retourne l'adresse mail du serveur</summary>
        Public Function GetSMTPMailServeur() As String Implements IHoMIDom.GetSMTPMailServeur
            Try
                Return _SMTPmailEmetteur
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetSMTPMailServeur", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>Fixe le password du serveur SMTP</summary>
        Public Sub SetSMTPMailServeur(ByVal Value As String) Implements IHoMIDom.SetSMTPMailServeur
            Try
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
        Sub SetLongitude(ByVal value As Double) Implements IHoMIDom.SetLongitude
            Try
                _Longitude = value
                MAJ_HeuresSoleil()
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
        Sub SetLatitude(ByVal value As Double) Implements IHoMIDom.SetLatitude
            Try
                _Latitude = value
                MAJ_HeuresSoleil()
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
        Sub SetHeureCorrectionCoucher(ByVal value As Integer) Implements IHoMIDom.SetHeureCorrectionCoucher
            Try
                _HeureCoucherSoleilCorrection = value
                MAJ_HeuresSoleil()
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
        Sub SetHeureCorrectionLever(ByVal value As Integer) Implements IHoMIDom.SetHeureCorrectionLever
            Try
                _HeureLeverSoleilCorrection = value
                MAJ_HeuresSoleil()
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SetHeureCorrectionLever", "Exception : " & ex.Message)
            End Try
        End Sub
#End Region

#Region "Driver"
        ''' <summary>Supprimer un driver de la config</summary>
        ''' <param name="driverId"></param>
        Public Function DeleteDriver(ByVal driverId As String) As Integer Implements IHoMIDom.DeleteDriver
            Try
                For i As Integer = 0 To _ListDrivers.Count - 1
                    If _ListDrivers.Item(i).Id = driverId Then
                        _ListDrivers.Item(i).removeat(i)
                        DeleteDriver = 0
                        Exit Function
                    End If
                Next
                Return 1
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeleteDriver", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>Arrête un driver par son Id</summary>
        ''' <param name="DriverId"></param>
        ''' <remarks></remarks>
        Sub StopDriver(ByVal DriverId As String) Implements IHoMIDom.StopDriver
            Try
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
        Sub StartDriver(ByVal DriverId As String) Implements IHoMIDom.StartDriver
            Try
                For i As Integer = 0 To _ListDrivers.Count - 1
                    If _ListDrivers.Item(i).id = DriverId Then
                        _ListDrivers.Item(i).start()
                    End If
                Next
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "StartDriver", "Exception : " & ex.Message)
            End Try
        End Sub

        ''' <summary>Retourne la liste de tous les drivers</summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetAllDrivers() As List(Of TemplateDriver) Implements IHoMIDom.GetAllDrivers
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
                Return _list
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetAllDrivers", "Exception : " & ex.Message)
                Return Nothing
            End Try
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
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SaveDriver(ByVal driverId As String, ByVal name As String, ByVal enable As Boolean, ByVal startauto As Boolean, ByVal iptcp As String, ByVal porttcp As String, ByVal ipudp As String, ByVal portudp As String, ByVal com As String, ByVal refresh As Integer, ByVal picture As String, Optional ByVal Parametres As ArrayList = Nothing) As String Implements IHoMIDom.SaveDriver
            Dim myID As String
            Try
                'Driver Existant
                myID = driverId
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
        Public Function ReturnDriverById(ByVal DriverId As String) As TemplateDriver Implements IHoMIDom.ReturnDriverByID
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
        Public Function ReturnDrvById(ByVal DriverId As String) As Object
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
        Public Function ReturnDriverByNom(ByVal DriverNom As String) As Object Implements IHoMIDom.ReturnDriverByNom
            Dim retour As Object = Nothing
            Try
                For i As Integer = 0 To _ListDrivers.Count - 1
                    If _ListDrivers.Item(i).Nom = DriverNom.ToUpper() Then
                        retour = _ListDrivers.Item(i)
                        Exit For
                    End If
                Next
                Return retour
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "ReturnDriverByNom", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function

        ''' <summary>Permet d'exécuter une commande Sub d'un Driver</summary>
        ''' <param name="DriverId"></param>
        ''' <param name="Action"></param>
        ''' <remarks></remarks>
        Sub ExecuteDriverCommand(ByVal DriverId As String, ByVal Action As DeviceAction) Implements IHoMIDom.ExecuteDriverCommand
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
        ''' <summary>Supprimer un device de la config</summary>
        ''' <param name="deviceId"></param>
        Public Function DeleteDevice(ByVal deviceId As String) As Integer Implements IHoMIDom.DeleteDevice
            Try
                For i As Integer = 0 To _ListDevices.Count - 1
                    If _ListDevices.Item(i).Id = deviceId Then
                        _ListDevices.Item(i).driver.deletedevice(deviceId)
                        _ListDevices.RemoveAt(i)
                        DeleteDevice = 0
                        Exit Function
                    End If
                Next
                Return 0
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeleteDevice", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>Retourne la liste de tous les devices</summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetAllDevices() As List(Of TemplateDevice) Implements IHoMIDom.GetAllDevices
            Dim _list As New List(Of TemplateDevice)
            Try
                For i As Integer = 0 To _ListDevices.Count - 1
                    Dim x As New TemplateDevice
                    Dim _listact As New List(Of String)

                    With x
                        .Name = _ListDevices.Item(i).name
                        .ID = _ListDevices.Item(i).id
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
                            .Value = _ListDevices.Item(i).value
                            .ValueDef = _ListDevices.Item(i).valuedef
                            .ValueLast = _ListDevices.Item(i).valuelast
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
                            For j As Integer = 0 To _ListDevices.Item(i).listcommandname.count - 1
                                .ListCommandName.Add(_ListDevices.Item(i).listcommandname.item(j))
                                .ListCommandData.Add(_ListDevices.Item(i).ListCommandData.item(j))
                                .ListCommandRepeat.Add(_ListDevices.Item(i).ListCommandRepeat.item(j))
                            Next
                        End If
                    End With
                    _list.Add(x)
                Next
                Return _list
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetAllDevices", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function

        ''' <summary>Sauvegarder ou créer un device</summary>
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
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SaveDevice(ByVal deviceId As String, ByVal name As String, ByVal address1 As String, ByVal enable As Boolean, ByVal solo As Boolean, ByVal driverid As String, ByVal type As String, ByVal refresh As Integer, Optional ByVal address2 As String = "", Optional ByVal image As String = "", Optional ByVal modele As String = "", Optional ByVal description As String = "", Optional ByVal lastchangeduree As Integer = 0) As String Implements IHoMIDom.SaveDevice
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
                            o.Driver.newdevice(deviceId)
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
                            _ListDevices.Item(i).Driver.newdevice(deviceId)
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
        Public Function DeleteDeviceCommandIR(ByVal deviceId As String, ByVal CmdName As String) As Integer Implements IHoMIDom.DeleteDeviceCommandIR
            Try
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
                Return 0
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
        Public Function SaveDeviceCommandIR(ByVal deviceId As String, ByVal CmdName As String, ByVal CmdData As String, ByVal CmdRepeat As String) As String Implements IHoMIDom.SaveDeviceCommandIR
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
        Public Function StartIrLearning() As String Implements IHoMIDom.StartIrLearning
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
        Public Function ReturnDeviceById(ByVal DeviceId As String) As TemplateDevice Implements IHoMIDom.ReturnDeviceByID
            Dim retour As New TemplateDevice
            Dim _listact As New List(Of String)

            Try
                For i As Integer = 0 To _ListDevices.Count - 1
                    If _ListDevices.Item(i).ID = DeviceId Then
                        retour.ID = _ListDevices.Item(i).id
                        retour.Name = _ListDevices.Item(i).name
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
                            For j As Integer = 0 To _ListDevices.Item(i).listcommandname.count - 1
                                retour.ListCommandName.Add(_ListDevices.Item(i).listcommandname.item(j))
                                retour.ListCommandData.Add(_ListDevices.Item(i).ListCommandData.item(j))
                                retour.ListCommandRepeat.Add(_ListDevices.Item(i).ListCommandRepeat.item(j))
                            Next
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
        Public Function ReturnDeviceByAdresse1TypeDriver(ByVal DeviceAdresse As String, ByVal DeviceType As String, ByVal DriverID As String) As ArrayList Implements IHoMIDom.ReturnDeviceByAdresse1TypeDriver
            Try
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
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "ReturnDeviceByAdresse1TypeDriver", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function

        ''' <summary>Permet d'exécuter une commande Sub d'un Device</summary>
        ''' <param name="DeviceId"></param>
        ''' <param name="Action"></param>
        ''' <remarks></remarks>
        Sub ExecuteDeviceCommand(ByVal DeviceId As String, ByVal Action As DeviceAction) Implements IHoMIDom.ExecuteDeviceCommand
            Dim _retour As Object
            Dim x As Object = Nothing

            Try
                For i As Integer = 0 To _ListDevices.Count - 1
                    If _ListDevices.Item(i).id = DeviceId Then
                        x = _ListDevices.Item(i)
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
                MsgBox("Erreur lors du traitemant du Sub ExecuteDeviceCommand: " & ex.Message, MsgBoxStyle.Exclamation, "Erreur Serveur")
            End Try
        End Sub
#End Region

#Region "Zone"
        ''' <summary>Supprimer une zone de la config</summary>
        ''' <param name="zoneId"></param>
        Public Function DeleteZone(ByVal zoneId As String) As Integer Implements IHoMIDom.DeleteZone
            Try
                For i As Integer = 0 To _ListZones.Count - 1
                    If _ListZones.Item(i).Id = zoneId Then
                        _ListZones.RemoveAt(i)
                        DeleteZone = 0
                        Exit Function
                    End If
                Next
                Return 1
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeleteZone", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>Retourne la liste de toutes les zones</summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetAllZones() As List(Of Zone) Implements IHoMIDom.GetAllZones
            Try
                Dim _list As New List(Of Zone)
                For i As Integer = 0 To _ListZones.Count - 1
                    Dim x As New Zone
                    With x
                        .Name = _ListZones.Item(i).name
                        .ID = _ListZones.Item(i).id
                        .Icon = _ListZones.Item(i).icon
                        .Image = _ListZones.Item(i).Image
                        For j As Integer = 0 To _ListZones.Item(i).ListElement.count - 1
                            .ListElement.Add(_ListZones.Item(i).ListElement.item(j))
                        Next
                    End With
                    _list.Add(x)
                Next
                Return _list
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetAllZones", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function

        ''' <summary>ajouter un device à une zone</summary>
        ''' <param name="ZoneId"></param>
        ''' <param name="DeviceId"></param>
        ''' <param name="Visible"></param>
        ''' <param name="X"></param>
        ''' <param name="Y"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function AddDeviceToZone(ByVal ZoneId As String, ByVal DeviceId As String, ByVal Visible As Boolean, Optional ByVal X As Double = 0, Optional ByVal Y As Double = 0) As String Implements IHoMIDom.AddDeviceToZone
            Dim _zone As Zone = ReturnZoneById(ZoneId)
            Dim _retour As String = ""
            Try
                If _zone IsNot Nothing Then
                    Dim _dev As New Zone.Element_Zone("", Visible, X, Y)
                    _zone.ListElement.Add(_dev)
                    _retour = "0"
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
        Function DeleteDeviceToZone(ByVal ZoneId As String, ByVal DeviceId As String) As String Implements IHoMIDom.DeleteDeviceToZone
            Dim _zone As Zone = ReturnZoneById(ZoneId)
            Dim _retour As String = ""
            Try
                If _zone IsNot Nothing Then
                    For i As Integer = 0 To _zone.ListElement.Count - 1
                        If _zone.ListElement.Item(i).ElementID = DeviceId Then
                            _zone.ListElement.RemoveAt(i)
                            Exit For
                        End If
                    Next
                    _retour = "0"
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
        Function SaveZone(ByVal zoneId As String, ByVal name As String, Optional ByVal ListElement As List(Of Zone.Element_Zone) = Nothing, Optional ByVal icon As String = "", Optional ByVal image As String = "") As String Implements IHoMIDom.SaveZone
            Dim myID As String = ""
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
                        If _ListZones.Item(i).id = zoneId Then
                            _ListZones.Item(i).name = name
                            _ListZones.Item(i).icon = icon
                            _ListZones.Item(i).image = image
                            _ListZones.Item(i).listelement = ListElement
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
        Function GetDeviceInZone(ByVal zoneId) As List(Of TemplateDevice) Implements IHoMIDom.GetDeviceInZone
            Try
                Return Nothing
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
        Public Function ZoneIsEmpty(ByVal zoneId) As Boolean Implements IHoMIDom.ZoneIsEmpty
            Dim retour As Boolean = True
            Dim x As Zone = ReturnZoneById(zoneId)
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
        Public Function ReturnZoneById(ByVal ZoneId As String) As Zone Implements IHoMIDom.ReturnZoneByID
            Dim retour As Object = Nothing
            Try
                For i As Integer = 0 To _ListZones.Count - 1
                    If _ListZones.Item(i).ID = ZoneId Then
                        retour = _ListZones.Item(i)
                        Exit For
                    End If
                Next
                Return retour
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "ReturnZoneById", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function
#End Region

#Region "Macro"
        ''' <summary>Supprimer une macro de la config</summary>
        ''' <param name="macroId"></param>
        Public Function DeleteMacro(ByVal macroId As String) As Integer Implements IHoMIDom.DeleteMacro
            Try
                For i As Integer = 0 To _ListMacros.Count - 1
                    If _ListMacros.Item(i).Id = macroId Then
                        _ListMacros.RemoveAt(i)
                        DeleteMacro = 0
                        Exit Function
                    End If
                Next
                Return 1
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeleteMacro", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>Retourne la liste de toutes les macros</summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetAllMacros() As List(Of Macro) Implements IHoMIDom.GetAllMacros
            Try
                Dim _list As New List(Of Macro)
                For i As Integer = 0 To _ListMacros.Count - 1
                    Dim x As New Macro
                    With x
                        .Nom = _ListMacros.Item(i).nom
                        .ID = _ListMacros.Item(i).id
                        .Description = _ListMacros.Item(i).description
                        .Enable = _ListMacros.Item(i).enable
                    End With
                    _list.Add(x)
                Next
                Return _list
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetAllMacros", "Exception : " & ex.Message)
                Return Nothing
            End Try
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
        Public Function SaveMacro(ByVal macroId As String, ByVal nom As String, ByVal enable As Boolean, Optional ByVal description As String = "", Optional ByVal listactions As List(Of TemplateAction) = Nothing) As String Implements IHoMIDom.SaveMacro
            Dim myID As String = ""
            Try
                If macroId = "" Then
                    Dim x As New Macro
                    With x
                        x.ID = GenerateGUID()
                        x.Nom = nom
                        x.Enable = enable
                        x.Description = description
                        Dim tabl As New ArrayList
                        For i As Integer = 0 To listactions.Count - 1
                            Select Case listactions.Item(i).TypeAction
                                Case Action.TypeAction.ActionDevice
                                    Dim o As New Action.ActionDevice
                                    o.Timing = listactions.Item(i).Timing
                                    o.IdDevice = listactions.Item(i).IdDevice
                                    o.Method = listactions.Item(i).Action
                                    o.Parametres = listactions.Item(i).Parametres
                                    tabl.Add(o)
                            End Select
                        Next
                        x.ListActions = tabl
                    End With
                    myID = x.ID
                    _ListMacros.Add(x)
                Else
                    'macro Existante
                    myID = macroId
                    For i As Integer = 0 To _ListMacros.Count - 1
                        If _ListMacros.Item(i).id = macroId Then
                            _ListMacros.Item(i).nom = nom
                            _ListMacros.Item(i).enable = enable
                            _ListMacros.Item(i).description = description
                            Dim tabl As New ArrayList
                            For j As Integer = 0 To listactions.Count - 1
                                Select Case listactions.Item(j).TypeAction
                                    Case Action.TypeAction.ActionDevice
                                        Dim o As New Action.ActionDevice
                                        o.Timing = listactions.Item(j).Timing
                                        o.IdDevice = listactions.Item(j).IdDevice
                                        o.Method = listactions.Item(j).Action
                                        o.Parametres = listactions.Item(j).Parametres
                                        tabl.Add(o)
                                End Select
                            Next
                            _ListMacros.Item(i).ListActions = tabl
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
        Public Function ReturnMacroById(ByVal MacroId As String) As Macro Implements IHoMIDom.ReturnMacroById
            Dim retour As Macro = Nothing
            Try
                For i As Integer = 0 To _ListMacros.Count - 1
                    If _ListMacros.Item(i).ID = MacroId Then
                        retour = _ListMacros.Item(i)
                        Exit For
                    End If
                Next
                Return retour
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "ReturnMacroById", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function
#End Region

#Region "Trigger"
        ''' <summary>Supprimer un trigger de la config</summary>
        ''' <param name="triggerId"></param>
        Public Function DeleteTrigger(ByVal triggerId As String) As Integer Implements IHoMIDom.DeleteTrigger
            Try
                For i As Integer = 0 To _listTriggers.Count - 1
                    If _listTriggers.Item(i).Id = triggerId Then
                        _listTriggers.RemoveAt(i)
                        DeleteTrigger = 0
                        Exit Function
                    End If
                Next
                Return 1
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeleteTrigger", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>Retourne la liste de toutes les macros</summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetAllTriggers() As List(Of Trigger) Implements IHoMIDom.GetAllTriggers
            Try
                Dim _list As New List(Of Trigger)
                For i As Integer = 0 To _listTriggers.Count - 1
                    Dim x As New Trigger
                    With x
                        .Nom = _listTriggers.Item(i).nom
                        .ID = _listTriggers.Item(i).id
                        .Description = _listTriggers.Item(i).description
                        .Enable = _listTriggers.Item(i).enable
                        .Prochainedateheure = _listTriggers.Item(i).Prochainedateheure
                        .Type = _listTriggers.Item(i).type
                        .ConditionTime = _listTriggers.Item(i).conditiontime
                        .ConditionDeviceId = _listTriggers.Item(i).ConditionDeviceId
                        .ConditionDeviceProperty = _listTriggers.Item(i).ConditionDeviceProperty
                    End With
                    _list.Add(x)
                Next
                Return _list
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetAllTriggers", "Exception : " & ex.Message)
                Return Nothing
            End Try
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
        Public Function SaveTrigger(ByVal triggerId As String, ByVal nom As String, ByVal enable As Boolean, ByVal TypeTrigger As Trigger.TypeTrigger, Optional ByVal description As String = "", Optional ByVal conditiontimer As String = "", Optional ByVal deviceid As String = "", Optional ByVal deviceproperty As String = "", Optional ByVal macro As ArrayList = Nothing) As String Implements IHoMIDom.SaveTrigger
            Dim myID As String = ""
            Try
                If triggerId = "" Then
                    Dim x As New Trigger
                    With x
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
                        x.Description = description
                        x.ListMacro = macro
                    End With
                    myID = x.ID
                    _listTriggers.Add(x)
                Else
                    'trigger Existante
                    myID = triggerId
                    For i As Integer = 0 To _listTriggers.Count - 1
                        If _listTriggers.Item(i).id = triggerId Then
                            _listTriggers.Item(i).nom = nom
                            _listTriggers.Item(i).enable = enable
                            _listTriggers.Item(i).description = description
                            Select Case TypeTrigger
                                Case Trigger.TypeTrigger.TIMER
                                    _listTriggers.Item(i).type = HoMIDom.Trigger.TypeTrigger.TIMER
                                    _listTriggers.Item(i).ConditionTime = conditiontimer
                                Case Trigger.TypeTrigger.DEVICE
                                    _listTriggers.Item(i).type = HoMIDom.Trigger.TypeTrigger.DEVICE
                                    _listTriggers.Item(i).ConditionDeviceId = deviceid
                                    _listTriggers.Item(i).ConditionDeviceProperty = deviceproperty
                            End Select
                            _listTriggers.Item(i).macro = macro
                        End If
                    Next
                End If
                'génération de l'event
                Return myID
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SaveTrigger", "Exception : " & ex.Message)
                Return "-1"
            End Try
        End Function

        ''' <summary>Retourne le trigger par son ID</summary>
        ''' <param name="TriggerId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ReturnTriggerById(ByVal TriggerId As String) As Trigger Implements IHoMIDom.ReturnTriggerById
            Dim retour As Object = Nothing
            Try
                For i As Integer = 0 To _listTriggers.Count - 1
                    If _listTriggers.Item(i).ID = TriggerId Then
                        retour = _listTriggers.Item(i)
                        Exit For
                    End If
                Next
                Return retour
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "ReturnTriggerById", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function

#End Region

#Region "User"
        ''' <summary>Supprime un user</summary>
        ''' <param name="userId"></param>
        Public Function DeleteUser(ByVal userId As String) As Integer Implements IHoMIDom.DeleteUser
            Try
                For i As Integer = 0 To _ListUsers.Count - 1
                    If _ListUsers.Item(i).Id = userId Then
                        _ListUsers.RemoveAt(i)
                        DeleteUser = 0
                        Exit Function
                    End If
                Next
                Return 1
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "DeleteUser", "Exception : " & ex.Message)
                Return -1
            End Try
        End Function

        ''' <summary>Retourne la liste de tous les users</summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetAllUsers() As List(Of Users.User) Implements IHoMIDom.GetAllUsers
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
                Return _list
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "GetAllUsers", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Retourne un user par son username
        ''' </summary>
        ''' <param name="Username"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ReturnUserByUsername(ByVal Username As String) As Users.User Implements IHoMIDom.ReturnUserByUsername
            Dim retour As Users.User = Nothing
            For i As Integer = 0 To _ListUsers.Count - 1
                If _ListUsers.Item(i).username = Username Then
                    retour = _ListUsers.Item(i)
                    Exit For
                End If
            Next
            Return retour
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
        Function SaveUser(ByVal userId As String, ByVal UserName As String, ByVal Password As String, ByVal Profil As Users.TypeProfil, ByVal Nom As String, ByVal Prenom As String, Optional ByVal NumberIdentification As String = "", Optional ByVal Image As String = "", Optional ByVal eMail As String = "", Optional ByVal eMailAutre As String = "", Optional ByVal TelFixe As String = "", Optional ByVal TelMobile As String = "", Optional ByVal TelAutre As String = "", Optional ByVal Adresse As String = "", Optional ByVal Ville As String = "", Optional ByVal CodePostal As String = "") As String Implements IHoMIDom.SaveUser
            Dim myID As String = ""
            Try
                If userId = "" Then
                    For i As Integer = 0 To _ListUsers.Count - 1
                        If _ListUsers.Item(i).username = UserName Then
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
                        If _ListUsers.Item(i).id = userId Then
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
                    If _ListUsers.Item(i).username = Username Then
                        Dim a As String = EncryptTripleDES(Password, "homidom")
                        Dim b As String = DecryptTripleDES(_ListUsers.Item(i).password, "homidom")
                        If a = b Then
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
        Function ChangePassword(ByVal Username As String, ByVal OldPassword As String, ByVal ConfirmNewPassword As String, ByVal Password As String) As Boolean Implements IHoMIDom.ChangePassword
            Dim retour As Boolean = False
            Try
                For i As Integer = 0 To _ListUsers.Count - 1
                    If _ListUsers.Item(i).username = Username Then
                        If _ListUsers.Item(i).password = OldPassword Then
                            If ConfirmNewPassword = Password Then
                                _ListUsers.Item(i).password = EncryptTripleDES(Password, "homidom")
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
        Public Function ReturnUserById(ByVal UserId As String) As Users.User Implements IHoMIDom.ReturnUserById
            Dim retour As Object = Nothing
            Try
                For i As Integer = 0 To _ListUsers.Count - 1
                    If _ListUsers.Item(i).ID = UserId Then
                        retour = _ListUsers.Item(i)
                        Exit For
                    End If
                Next
                Return retour
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "ReturnUserById", "Exception : " & ex.Message)
                Return Nothing
            End Try
        End Function

#End Region

#Region "Log"
        ''' <summary>renvoi le fichier log suivant une requête xml si besoin</summary>
        ''' <param name="Requete"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function ReturnLog(Optional ByVal Requete As String = "") As String Implements IHoMIDom.ReturnLog
            Try
                Dim retour As String = ""
                If Requete = "" Then
                    Dim SR As New StreamReader(_MonRepertoire & "\logs\log.xml")
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
        ''' <summary>Sauvegarder la configuration</summary>
        ''' <remarks></remarks>
        Public Sub SaveConfiguration() Implements IHoMIDom.SaveConfig
            Try
                SaveConfig(_MonRepertoire & "\config\homidom.xml")
            Catch ex As Exception
                Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SaveConfiguration", "Exception : " & ex.Message)
            End Try
        End Sub
#End Region

#Region "SOAP"
        ''' <summary>Fixer la valeur du port SOAP</summary>
        ''' <param name="Value"></param>
        ''' <remarks></remarks>
        Public Sub SetPortSOAP(ByVal Value As Double) Implements IHoMIDom.SetPortSOAP
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
#End Region

#End Region

    End Class

End Namespace