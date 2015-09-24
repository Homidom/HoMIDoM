Imports HoMIDom
Imports HoMIDom.HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device

Imports System.IO
Imports System.Data
Imports System.Data.SQLite
Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.Text.RegularExpressions
Imports System.Xml

' Auteur : jphomi 
' Date : 01/08/2015

''' <summary>Driver BDD gestion BDD</summary>
''' <remarks></remarks>
<Serializable()> Public Class Driver_BDD
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "E5431E50-299E-11E5-9D2E-42CC1D5D46B0"
    Dim _Nom As String = "BDD"
    Dim _Enable As Boolean = False
    Dim _Description As String = "Interrogation base de données"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = ""
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "@"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = ""
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
    Dim _IdSrv As String
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande)
    Dim _AutoDiscover As Boolean = False

    'param avancé
    Dim _DEBUG As Boolean = False
    Dim _PathQu As String = "C:\Program Files\HoMIDoM\Bdd"

    'A ajouter dans les ppt du driver


#End Region

#Region "Variables internes"
    Dim _Obj As Object = Nothing
#End Region

#Region "Propriétés génériques"
    Public WriteOnly Property IdSrv As String Implements HoMIDom.HoMIDom.IDriver.IdSrv
        Set(ByVal value As String)
            _IdSrv = value
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
    Public Property Refresh() As Integer Implements HoMIDom.HoMIDom.IDriver.Refresh
        Get
            Return _Refresh
        End Get
        Set(ByVal value As Integer)
            _Refresh = value
        End Set
    End Property
    Public Property Server() As HoMIDom.HoMIDom.Server Implements HoMIDom.HoMIDom.IDriver.Server
        Get
            Return _Server
        End Get
        Set(ByVal value As HoMIDom.HoMIDom.Server)
            _Server = value
        End Set
    End Property
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
    Public Property StartAuto() As Boolean Implements HoMIDom.HoMIDom.IDriver.StartAuto
        Get
            Return _StartAuto
        End Get
        Set(ByVal value As Boolean)
            _StartAuto = value
        End Set
    End Property
    Public Property AutoDiscover() As Boolean Implements HoMIDom.HoMIDom.IDriver.AutoDiscover
        Get
            Return _AutoDiscover
        End Get
        Set(ByVal value As Boolean)
            _AutoDiscover = value
        End Set
    End Property
#End Region

#Region "Fonctions génériques"
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
                    WriteLog("DBG: ExecuteCommandAdvance : " & MyDevice.ToString & " : " & Command & "/" & Param(0) & "-" & Param(1))
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
            WriteLog("ERR: ExecuteCommand exception : " & ex.Message)
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
            Dim result As String = "0"

            Select Case UCase(Champ)
                Case "ADRESSE1"
                    If String.IsNullOrEmpty(Value) Then
                        result = "Veuillez saisir la valeur StatutAC (mode secteur/batterie) ou StatutEC (economiseur energie) ou TpsRestant (Temps restant en min.) ou TpsCharge (Temps de charge en min.)"
                    End If
            End Select
            Return result
        Catch ex As Exception
            Return "Une erreur est apparue lors de la vérification du champ " & Champ & ": " & ex.ToString
        End Try
    End Function

    ''' <summary>Démarrer le driver</summary>
    ''' <remarks></remarks>
    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        Try
            _DEBUG = _Parametres.Item(0).Valeur
            _PathQu = _Parametres.Item(1).Valeur
            If Right(_PathQu, 1) <> "\" Then _PathQu = _PathQu & "\"

            If Directory.Exists(_PathQu) Then
                WriteLog("DBG: Repertoire " & _PathQu & " existant")
                _IsConnect = True
            Else
                WriteLog("DBG: Repertoire " & _PathQu & " inexistant")
                _IsConnect = False
            End If

        Catch ex As Exception
            _IsConnect = False
            WriteLog("ERR: Driver " & Me.Nom & " non démarré")
        End Try
    End Sub


    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            _IsConnect = False
            WriteLog("Driver " & Me.Nom & " arrêté")
        Catch ex As Exception
            WriteLog("ERR: Driver " & Me.Nom & " Erreur arrêt " & ex.Message)
        End Try
    End Sub

    ''' <summary>Re-Démarrer le du driver</summary>
    ''' <remarks></remarks>
    Public Sub Restart() Implements HoMIDom.HoMIDom.IDriver.Restart
        [Stop]()
        Start()
    End Sub

    ''' <summary>Intérroger un device</summary>
    ''' <param name="Objet">Objet représetant le device à interroger</param>
    ''' <remarks>pas utilisé</remarks>
    Public Sub Read(ByVal Objet As Object) Implements HoMIDom.HoMIDom.IDriver.Read

        Try
            If _Enable = False Then Exit Sub

            If _IsConnect = False Then
                WriteLog("ERR: READ, Le driver n'est pas démarré, impossible d'executer la requete")
                Exit Sub
            End If

            Try ' lecture de la variable debug, permet de rafraichir la variable debug sans redemarrer le service
                _DEBUG = _Parametres.Item(0).Valeur
            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Erreur de lecture de debug : " & ex.Message)
            End Try

            Dim NomQfile As String = _PathQu + Objet.adresse1
            Dim TypeBdd As String = ""
            Dim Provider As String = ""
            Dim NomServer As String = ""
            Dim PathBdd As String = ""
            Dim NomBdd As String = ""
            Dim User As String = ""
            Dim Password As String = ""
            Dim Separ As String = ""
            Dim str As String = ""

            WriteLog("DBG: Nom fichier requete => " & NomQfile)

            If File.Exists(NomQfile) Then

                Dim Qsql As String = ""
                Dim Qvalue As String = ""
                Dim reader As XmlTextReader = New XmlTextReader(NomQfile)
                reader.WhitespaceHandling = WhitespaceHandling.Significant
                WriteLog("DBG: Fichier xml -> " & NomQfile & " acquis")
                While reader.Read()
                    Str = reader.ReadString
                    ' WriteLog("DBG: " & reader.Name & " -> " & str)
                    Select Case True
                        Case reader.Name = "TypeBdd"
                            TypeBdd = str
                            WriteLog("DBG: " & "TypeBdd -> " & TypeBdd)
                        Case reader.Name = "Provider"
                            provider = str
                            WriteLog("DBG: " & "Provider -> " & Provider)
                        Case reader.Name = "NomServeur"
                            NomServer = str
                            WriteLog("DBG: " & "NomServer -> " & NomServer)
                        Case reader.Name = "PathBdd"
                            PathBdd = str
                            If Right(PathBdd, 1) <> "\" Then PathBdd = PathBdd & "\"
                            WriteLog("DBG: " & "PathBdd -> " & PathBdd)
                        Case reader.Name = "NomBdd"
                            NomBdd = str
                            WriteLog("DBG: " & "NomBdd -> " & NomBdd)
                        Case reader.Name = "User"
                            User = str
                            WriteLog("DBG: " & "User -> " & User)
                        Case reader.Name = "Password"
                            Password = str
                            WriteLog("DBG: " & "Password -> " & Password)
                        Case reader.Name = "Separate"
                            Separ = str
                            WriteLog("DBG: " & "Separ -> " & Separ)
                        Case InStr(1, reader.Name, "lig") > 0
                            If InStr(Objet.adresse2, reader.Name) > 0 Then 'remplace la ligne de la requete par celle de l'adresse2
                                str = Mid(Objet.adresse2, InStr(Objet.adresse2, ":") + 1)
                                WriteLog("DBG: Sql adr2 : " & Objet.adresse2)
                            End If

                            '                            If (InStr(str, "<<") <> 0) And (InStr(str, ">>") <> 0) Then 'cas du passage de paramétrage
                            'Dim startcmd As Integer = InStr(str, "<<")
                            '                            Dim endcmd As Integer = InStr(str, ">>")
                            If (InStr(str, "{") <> 0) And (InStr(str, "}") <> 0) Then 'cas du passage de paramétrage
                                Dim startcmd As Integer = InStr(str, "{")
                                Dim endcmd As Integer = InStr(str, "}")
                                Dim cmd As String = Trim(Mid(str, startcmd + 1, endcmd - startcmd - 1))
                                WriteLog("DBG: DecodeCommand cmd: " & cmd)

                                Dim valdevice As String = ""
                                Dim ListeDevices As List(Of TemplateDevice)

                                Select Case UCase(cmd)
                                    Case "SYSTEM_DATE_DD-MM-YYYY"
                                        valdevice = Format(Date.Now(), "dd-MM-yyyy")
                                    Case "SYSTEM_DATE_DD/MM/YYYY"
                                        valdevice = Format(Date.Now(), "dd/MM/yyyy")
                                    Case "SYSTEM_DATE_MM-DD-YYYY"
                                        valdevice = Format(Date.Now(), "MM-dd-yyyy")
                                    Case "SYSTEM_DATE_MM/DD/YYYY"
                                        valdevice = Format(Date.Now(), "MM/dd/yyyy")
                                    Case "SYSTEM_DATE_YYYY-MM-DD"
                                        valdevice = Format(Date.Now(), "yyyy-MM-dd")
                                    Case "SYSTEM_DATE_YYYY/MM/DD"
                                        valdevice = Format(Date.Now(), "yyyy/MM/dd")
                                    Case "SYSTEM_LONG_DATE"
                                        valdevice = Date.Now.Date.ToLongDateString
                                    Case "SYSTEM_TIME"
                                        valdevice = Date.Now.ToShortTimeString
                                    Case "SYSTEM_LONG_TIME"
                                        valdevice = Date.Now.ToLongTimeString
                                    Case Else
                                        ListeDevices = _Server.GetAllDevices(NomServer)
                                        For Each _dev As TemplateDevice In ListeDevices
                                            If _dev.Name = cmd Then
                                                valdevice = _dev.ID
                                            End If
                                        Next
                                End Select
                                WriteLog("DBG: DecodeCommad valeur decodee : " & valdevice)
                                WriteLog("DBG: Sql parametre " & str & " => " & cmd)
                                '                                str = Replace(str, "<<" & cmd & ">>", valdevice)
                                str = Replace(str, "{" & cmd & "}", valdevice)
                                '                                WriteLog("DBG: Sql parametre Replace " & "<<" & cmd & ">>" & " => " & valdevice)
                                WriteLog("DBG: Sql parametre Replace " & "{" & cmd & "}" & " => " & valdevice)
                            End If
                            Qsql = Qsql & " " & str
                    End Select
                    str = ""
                End While

                Qsql = Trim(Qsql)
                If Len(Qsql) = 0 Then
                    WriteLog("ERR: Requete SQL vide !")
                    Exit Sub
                End If

                WriteLog("DBG: Sql => " & Qsql)

                Select Case UCase(TypeBdd)
                    Case "ACCESS", "EXCEL"
                        Qvalue = QOffice(TypeBdd, Provider, PathBdd & NomBdd, Qsql, User, Password)
                    Case "MYSQL"
                        Qvalue = QMySql(NomServer, Provider, NomBdd, Qsql, User, Password)
                    Case "SQL"
                        Qvalue = QSsql(NomServer, Provider, NomBdd, Qsql, User, Password)
                    Case "SQLITE"
                        Qvalue = QSqLite(TypeBdd, Provider, PathBdd & NomBdd, Qsql, User, Password)
                    Case "CSV"
                        Qvalue = QCsv(TypeBdd, Provider, PathBdd & NomBdd, Qsql, User, Password)
                    Case "TXT"
                End Select
                WriteLog("DBG: Valeur retournée par la requete => " & Qvalue)

                If Trim(Qvalue) <> "" Then
                    Select Case Objet.Type
                        Case "GENERIQUESTRING", "GENERIQUEBOOLEAN"
                            Objet.Value = Qvalue
                        Case "GENERIQUEVALUE"
                            If UCase(Qvalue) <> "FALSE" Then
                                Objet.Value = Regex.Replace(CStr(Qvalue), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                            Else
                                Objet.Value = -1
                            End If
                    End Select
                End If

            Else
                WriteLog("ERR: fichier " & NomQfile & " inexistant")
                Exit Sub
            End If

        Catch ex As Exception
            WriteLog("ERR: Read, Exception : " & ex.Message)
            WriteLog("ERR: Read, adresse1 : " & Objet.adresse1)
        End Try
    End Sub

    ''' <summary>Commander un device</summary>
    ''' <param name="Objet">Objet représentant le device à interroger</param>
    ''' <param name="Command">La commande à passer</param>
    ''' <param name="Parametre1"></param>
    ''' <param name="Parametre2"></param>
    ''' <remarks></remarks>
    Public Sub Write(ByVal Objet As Object, ByVal Command As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
        Try
            If _Enable = False Then Exit Sub

            If _IsConnect = False Then
                WriteLog("ERR: WRITE, Le driver n'est pas démarré, impossible d'écrire")
                Exit Sub
            End If

            WriteLog("DBG: WRITE Device " & Objet.Name & " --> " & Command)


        Catch ex As Exception
            WriteLog("ERR: WRITE, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice
        Try

        Catch ex As Exception
            WriteLog("ERR: DeleteDevice, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice
        Try

        Catch ex As Exception
            WriteLog("ERR: NewDevice, Exception : " & ex.Message)
        End Try
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
            WriteLog("ERR: Add_DeviceCommande, Exception :" & ex.Message)
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
            WriteLog("ERR: Add_LibelleDriver, Exception : " & ex.Message)
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
            WriteLog("ERR: Add_LibelleDevice, Exception : " & ex.Message)
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
            WriteLog("ERR: Add_ParamAvance, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Creation d'un objet de type</summary>
    ''' <remarks></remarks>
    Public Sub New()
        Try
            _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString

            'liste des devices compatibles
            _DeviceSupport.Add(ListeDevices.GENERIQUEVALUE.ToString)
            _DeviceSupport.Add(ListeDevices.GENERIQUESTRING.ToString)
            _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN.ToString)

            'Parametres avancés
            Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)
            Add_ParamAvance("Répertoire Query", "Répertoire des fichiers Query.xml", "")

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Fichier SQL", "Nom du fichier SQL", "")
            Add_LibelleDevice("ADRESSE2", "Paramètre", "", "")
            Add_LibelleDevice("REFRESH", "Refresh en sec", "Minimum 600, valeur rafraicissement station", "600")
            ' Libellés Device inutiles
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("MODELE", "@", "")
            Add_LibelleDevice("LASTCHANGEDUREE", "@", "")
        Catch ex As Exception
            WriteLog("ERR: New, Exception : " & ex.Message)
        End Try
    End Sub

#End Region

#Region "Fonctions internes"

    Public Function QOffice(TypeBase As String, Provider As String, NomBase As String, QuSql As String, Optional User As String = "", Optional Password As String = "") As String

        Try
            Dim cnn As New ADODB.Connection, rst As New ADODB.Recordset
            Dim cconnect As String = ""
            Select Case UCase(TypeBase)
                Case "ACCESS"
                    cconnect = "Provider=Microsoft.ACE.OLEDB.12.0;" & "Data Source=" & NomBase & ";Mode=Share Deny None;"
                Case "EXCEL"
                    cconnect = "Provider=Microsoft.ACE.OLEDB.12.0;" & "Data Source=" & NomBase & ";Extended Properties=""Excel 12.0;HDR=YES;"""
            End Select

            If User <> "" Then
                cconnect = cconnect & "User ID=" & User & ";"
            End If
            If Password <> "" Then
                cconnect = cconnect & "Password=" & Password & ";"
            End If

            ' ne prends pas la chaine de connexion par defaut
            If Provider <> "" Then cconnect = Provider

            WriteLog("DBG: QOffice, c.connect : " & cconnect)
            cnn.ConnectionString = cconnect
            ' Ouverture de la connexion
            cnn.Open()
            ' Ouverture du Recordset en défilement en avant, et en lecture seule
            rst.Open(QuSql, cnn, ADODB.CursorTypeEnum.adOpenForwardOnly, ADODB.LockTypeEnum.adLockReadOnly)
            rst.MoveFirst()
            QOffice = rst.Fields(0).Value
            ' Fermeture du Recordset
            rst.Close()
            cnn.Close()

        Catch ex As Exception
            WriteLog("ERR: QOffice, Exception : " & ex.Message)
            QOffice = "False"
        End Try

    End Function

    Public Function QSqLite(TypeBase As String, Provider As String, NomBase As String, QuSql As String, Optional User As String = "", Optional Password As String = "") As String

        Try
            'WriteLog("DBG: QSqLite, Base " & TypeBase & " / " & NomBase & " à ouvrir")
            Dim cnn As New SQLiteConnection("Data Source=" & NomBase)

            ' Ouverture de la connexion
            cnn.Open()
            WriteLog("DBG: QSqLite, Base " & NomBase & " ouverte")
            QSqLite = "True"

            Dim cmd = New SQLiteCommand(QuSql, cnn)
            Dim rst As SQLiteDataReader = cmd.ExecuteReader
            While rst.Read
                QSqLite = rst(0).ToString
                WriteLog("DBG: QSqLite, Valeur lue " & rst(0).ToString)
            End While
            ' Fermeture du Recordset
            rst.Close()
            cnn.Close()

        Catch ex As Exception
            WriteLog("ERR: QSqlLite, Exception : " & ex.Message)
            QSqLite = "False"
        End Try

    End Function

    Public Function QSsql(Server As String, Provider As String, NomBase As String, QuSql As String, Optional User As String = "", Optional Password As String = "") As String

        Try
            Dim cnn As New ADODB.Connection, rst As New ADODB.Recordset
            Dim cconnect As String = ""
            cconnect = "provider=SQLOLEDB;Data Source=" & Server & ";Initial Catalog=" & NomBase & ";"
            If User <> "" Then
                cconnect = cconnect & "User ID=" & User & ";"
                cconnect = cconnect & "Password=" & Password & ";"
            Else
                cconnect = cconnect & "Integrated Security=SSPI"
            End If

            ' ne prends pas la chaine de connexion par defaut
            If Provider <> "" Then cconnect = Provider

            WriteLog("DBG: QSsql, c.connect : " & cconnect)
            cnn.ConnectionString = cconnect
            ' Ouverture de la connexion
            cnn.Open()
            ' Ouverture du Recordset en défilement en avant, et en lecture seule
            rst.Open(QuSql, cnn, ADODB.CursorTypeEnum.adOpenForwardOnly, ADODB.LockTypeEnum.adLockReadOnly)
            rst.MoveFirst()
            QSsql = rst.Fields(0).Value
            ' Fermeture du Recordset
            rst.Close()
            cnn.Close()

        Catch ex As Exception
            WriteLog("ERR: QSsql, Exception : " & ex.Message)
            QSsql = "False"
        End Try

    End Function
    Public Function QMySql(Server As String, Provider As String, NomBase As String, QuSql As String, Optional User As String = "", Optional Password As String = "") As String

        Try
            Dim cnn As New ADODB.Connection, rst As New ADODB.Recordset
            Dim cconnect As String = ""
            cconnect = "Provider=MySQLProv;Server=" & Server & "Data Source=" & NomBase & ";"
            If User <> "" Then
                cconnect = cconnect & "User Id=" & User & ";"
                cconnect = cconnect & "Password=" & Password & ";"
            End If

            ' ne prends pas la chaine de connexion par defaut
            If Provider <> "" Then cconnect = Provider

            WriteLog("DBG: QMySql, c.connect : " & cconnect)
            cnn.ConnectionString = cconnect
            ' Ouverture de la connexion
            cnn.Open()
            ' Ouverture du Recordset en défilement en avant, et en lecture seule
            rst.Open(QuSql, cnn, ADODB.CursorTypeEnum.adOpenForwardOnly, ADODB.LockTypeEnum.adLockReadOnly)
            rst.MoveFirst()
            QMySql = rst.Fields(0).Value
            ' Fermeture du Recordset
            rst.Close()
            cnn.Close()

        Catch ex As Exception
            WriteLog("ERR: QMySql, Exception : " & ex.Message)
            QMySql = "False"
        End Try

    End Function

    Public Function QCsv(Server As String, Provider As String, NomBase As String, QuSql As String, Optional User As String = "", Optional Password As String = "") As String

        Try
            Return ""
        Catch ex As Exception
            WriteLog("ERR: QCsv, Exception : " & ex.Message)
            QCsv = "False"
        End Try

    End Function
    Private Sub WriteLog(ByVal message As String)
        Try
            'utilise la fonction de base pour loguer un event
            If STRGS.InStr(message, "DBG:") > 0 Then
                If _DEBUG Then
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom, STRGS.Right(message, message.Length - 5))
                End If
            ElseIf STRGS.InStr(message, "ERR:") > 0 Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom, STRGS.Right(message, message.Length - 5))
            Else
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom, message)
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " WriteLog", ex.Message)
        End Try
    End Sub

#End Region

End Class

