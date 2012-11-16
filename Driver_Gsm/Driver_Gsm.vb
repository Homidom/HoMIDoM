

Imports HoMIDom
Imports HoMIDom.HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports System.IO
Imports System.Net
Imports System.Web
Imports System.Text
Imports STRGS = Microsoft.VisualBasic.Strings

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
'Imports System.Drawing
'Imports System.Windows.Forms
Imports GsmComm.GsmCommunication
'Imports GsmComm.Interfaces
Imports GsmComm.PduConverter
'Imports GsmComm.Server

Imports System.Data.OleDb
Imports System.Management

Imports System.IO.Ports

' Auteur : Fabien
' Date : 09/09/2012
'-------------------------------------------------------------------------------------
'                                                                      
'-------------------------------------------------------------------------------------

' Driver GSM
''' <summary>Class Driver GSM </summary>
''' <remarks>Les drivers de votre modem doivent etre installés
''' Nécessite les dll GSMCommServer.dll,  GSMCommunication.dll, GSMCommShared.dll,  PDUConverter.dll </remarks>


<Serializable()> Public Class Driver_Gsm
    Implements HoMIDom.HoMIDom.IDriver


#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    ' Dim _ID As String = "7396FA30-0050-11E2-9BC2-523B6288709B"
    Dim _ID As String = "596AD836-FA1D-11E1-BDE2-639C6188709B"
    Dim _Nom As String = "GSM"
    Dim _Enable As String = False
    Dim _Description As String = "Driver GSM"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "GSM"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "COM6"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "GSM"
    Dim _Version As String = My.Application.Info.Version.ToString
    Dim _OsPlatform As String = "3264"
    Dim _Picture As String = ""
    Dim _Server As HoMIDom.HoMIDom.Server
    Dim _DeviceSupport As New ArrayList
    Dim _Device As HoMIDom.HoMIDom.Device
    Dim _Parametres As New ArrayList
    Dim _LabelsDriver As New ArrayList
    Dim _LabelsDevice As New ArrayList
    Dim MyTimer As New Timers.Timer
    Dim _Idsrv As String
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande)

    'param avancé
    Dim _DEBUG As Boolean = False
    Dim _MODE As String = "PDU"
    '    Dim _STORAGE As PhoneStorageType

#End Region

#Region "Variables Internes"

    Private ackreceived As Boolean = False
    Private WithEvents comm As GsmCommMain

    Private ATport As New SerialPort(_Com, 9600, Parity.None, 8, StopBits.One)
    Dim MODES As Array

#End Region

#Region "Propriétés génériques"
    Public WriteOnly Property IdSrv As String Implements HoMIDom.HoMIDom.IDriver.IdSrv
        Set(ByVal value As String)
            _Idsrv = value
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
    Public Property Port_TCP() As Object Implements HoMIDom.HoMIDom.IDriver.Port_TCP
        Get
            Return _Port_TCP
        End Get
        Set(ByVal value As Object)
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
    ''' 
    Public Function ExecuteCommand(ByVal MyDevice As Object, ByVal Command As String, Optional ByVal Param() As Object = Nothing) As Boolean
        Dim retour As Boolean = False
        Try
            If MyDevice IsNot Nothing Then

                'Pas de commande demandée donc erreur
                If Command = "" Then
                    Return False
                Else
                    _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & "ExecuteCommand", Command & " - " & Param(0))
                    Write(MyDevice, Command, Param(0))
                    Return True
                End If
            Else
                Return False
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & "ExecuteCommand", "exception : " & ex.Message)
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
            Select Case UCase(Champ)
                Case "ADRESSE1" ' numero de telephone 
                    If Value IsNot Nothing Then
                        If Value = "" Or Value = " " Then
                            retour = "Veuillez saisir un numero de telephone valide."
                        End If
                    End If
                    ' Case "ADRESSE2" ' textmessage
                    '        If Value IsNot Nothing Then
                    'If Value = "" Or Value = " " Then
                    'retour = "Veuillez saisir un message"
                    '
                    'End If
                    'If (Len(Value) > 150) Then
                    'retour = "Veuillez saisir un message de taille inférieure à 150 characteres"

                    ' End If

                    'End If
            End Select


            Return retour
        Catch ex As Exception
            Return "Une erreur est apparue lors de la vérification du champ " & Champ & ": " & ex.ToString
        End Try
    End Function

    ''' <summary>Démarrer le du driver</summary>
    ''' <remarks></remarks>
    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start


        Try
            _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "Lecture des mode Supportés")



            '
            'lecture AT+CMGF=? pour connaitre les modes supportés par le modem gsm connecté PDU ou TEXT
            ' reponse 0 pour le mode PDU
            ' reponse 1 pour le mode TEXT
            '
            ATport.ReadTimeout = 500
            ATport.WriteTimeout = 500
            ATport.Open()
            Dim sIncomming As String = ""
            Do Until Left(sIncomming, 6) = "+CMGF:"
                ATport.WriteLine("AT+CMGF=?" & vbCrLf) ' vbcrlf
                sIncomming = ATport.ReadLine()
            Loop

            Dim startIndex As Integer = sIncomming.IndexOf("(") + "(".Length
            Dim endIndex As Integer = sIncomming.IndexOf(")")
            Dim result As String = sIncomming.Substring(startIndex, endIndex - startIndex)
            '_Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM PORT", "Read:" & result)
            MODES = result.Split(New Char() {","c})

            Dim SuportedMode As Boolean = False
            For m = 0 To (MODES.Length - 1)
                Select Case MODES(m).ToString
                    Case "0"
                        MODES(m) = "PDU"
                        _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM MODE", "Mode " & m & " (PDU) possible")
                    Case "1"
                        MODES(m) = "TEXTE"
                        _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM MODE", "Mode " & m & "(TEXTE) possible")
                    Case Else
                        MODES(m) = "UNKNOW"
                        _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM MODE", "Mode " & m & " inconnu ( ne sera pas utilisé)")
                End Select

                'If _MODE = MODES(m) Then
                'SuportedMode = True
                'Else
                'SuportedMode = SuportedMode Or False
                '_Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GSM Start", "Driver non démarré : Le mode '" & _MODE & "' n'est pas correct, choisi PDU ou TEXTE")
                'End If
            Next

            ATport.Close()
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GSM Start", "Erreur dans les paramétres avancés. utilisation des valeur par défaut" & ex.Message)

        End Try




        Dim retour As String = ""
        'récupération des paramétres avancés
        Try
            _DEBUG = _Parametres.Item(0).Valeur.ToString.ToUpper
            _MODE = _Parametres.Item(1).Valeur.ToString.ToUpper
            '_STORAGE = _Parametres.Item(2).Valeur
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GSM Start", "Erreur dans les paramétres avancés. utilisation des valeur par défaut" & ex.Message)
        End Try

        'Verification du MODE de connexion
        Try

            _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "Verification du mode selectionné")
            ' verif si le mode choisi par le user est bon
            Dim SuportedMode As Boolean = False
            Dim ModeToUse As String = ""
            For m As Integer = 0 To MODES.Length - 1
                If _MODE = MODES(m).ToString Then
                    SuportedMode = True
                Else
                    SuportedMode = (SuportedMode Or False)
                End If
            Next
          
            If SuportedMode = False Then
                For n As Integer = 0 To MODES.Length - 1
                    If n > 1 Then

                        _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "SuportedMode : '" & MODES(n).ToString & "'.")

                        ModeToUse = MODES(n)

                    Else
                        ModeToUse = ModeToUse & " ou " & MODES(n)
                    End If
                Next
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GSM Start", "Driver non démarré : Le mode choisi '" & _MODE & "' n'est pas correct, Veuillez choisir le mode " & ModeToUse & ".")
                _IsConnect = False
                Exit Sub
            Else
                _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "Démarrage du driver avec le mode '" & _MODE & "'.")

            End If

            ' If _MODE <> "PDU" And _MODE <> "TEXTE" Then
            '  _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GSM Start", "Driver non démarré : Le mode '" & _MODE & "' n'est pas correct, choisi PDU ou TEXTE")
            ' _IsConnect = False
            'Exit Sub
            'End If

            'interroge le tel/modem pour verifier si ce mode est supporté

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GSM Start", "Erreur Dans la verification du mode " & ex.Message)
        End Try

        'ouverture de la communication avec le GSM
        Try
            If _Com <> "" Then
                retour = ouvrir()
            Else
                retour = "ERR: Port Com non défini. Impossible d'ouvrir le port !"
            End If
            If STRGS.Left(retour, 4) = "ERR:" Then
                _IsConnect = False
                retour = STRGS.Right(retour, retour.Length - 5)
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GSM Start", "Driver non démarré : " & retour)
            Else
                _IsConnect = True
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "GSM Start", retour)
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GSM Start", " Driver en erreur lors du démarrage: " & ex.Message)

        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            'on ferme la connexion avec le téléphone
            fermer()

            '????
            For i As Integer = 0 To _Server.Devices.Count - 1
                If _Server.Devices.Item(i).Type = "Gsm" And _Server.Devices.Item(i).Adresse1 <> "" Then
                    Dim ProcId As Object = Shell(_Server.Devices.Item(i).Adresse1 & " /exit", AppWinStyle.Hide)
                End If
            Next

            _IsConnect = False
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Gsm", "Driver " & Me.Nom & " arrêté")
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Gsm Stop", ex.Message)
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
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Gsm Read", ex.Message)
        End Try
    End Sub

    ''' <summary>Commander un device</summary>
    ''' <param name="Objet">Objet représetant le device à interroger</param>
    ''' <param name="Command">La commande à passer</param>
    ''' <param name="Parametre1">Phone Number</param>
    Public Sub Write(ByVal Objet As Object, ByVal Command As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
        'Parametre1 = TxtMsg
        'Objet.adresse1.ToString = phonedestination
        Try
            If _Enable = False Then Exit Sub

            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "WRITE", "Commande: " & Command & ", Texte: " & Parametre1 & ", Composant: " & Objet.Name)
            Select Case UCase(Command)
                Case "SEND"

                    Select Case _MODE
                        Case "PDU"
                            Try
                                Dim pdu As SmsSubmitPdu
                                If _IsConnect Then
                                    'on verifie si un texte est passé en paramètre
                                    If Parametre1 = "" Then
                                        _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "SMS à envoyer vide, annulation")
                                    Else
                                        _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "SMS envoyé : " & Parametre1 & " à " & Objet.adresse1.ToString)
                                        pdu = New SmsSubmitPdu(Parametre1, Objet.adresse1.ToString, "")
                                        comm.SendMessage(pdu, True)
                                        'on modifie la valeur du composant pour stocker les sms envoyés
                                        Objet.Value = "SEND - " & Parametre1
                                    End If
                                Else
                                    _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "No GSM Phone / Modem Connected")
                                End If
                            Catch ex As Exception
                                _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "Error Sending SMS: " & ex.ToString)
                            End Try

                        Case "TEXTE"

                            Try
                                If _IsConnect Then
                                    'a titre indicatif ( passerelle sms )
                                    'Bouygues Télécom : +33660003000
                                    'Orange : +33689004000
                                    'SFR : +33609001390
                                    'sending AT commands
                                    'ATport.WriteLine("AT")
                                    If Parametre1 = "" Then
                                        _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "SMS à envoyer vide, annulation")
                                    Else
                                        _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "SMS envoi en cours : " & Parametre1 & " à " & Objet.adresse1.ToString)
                                        ATport.WriteLine("AT+CMGF=1" & vbCrLf) 'set command message format to text mode(1)
                                        ' SMSPort.WriteLine("AT+CSCA=""+33689004000""" & vbCrLf) 'set service center address (orange) 
                                        ATport.WriteLine("AT+CMGS=" & Objet.adresse1.ToString & vbCrLf) ' enter the mobile number whom you want to send the SMS
                                        '_ContSMS = False
                                        ATport.WriteLine(Parametre1 & vbCrLf & Chr(26)) 'SMS sending
                                        _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "SMS envoyé : " & Parametre1 & " à " & Objet.adresse1.ToString)
                                        Objet.Value = "SEND - " & Parametre1
                                    End If
                                Else
                                    _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "No GSM Phone / Modem Connected")
                                End If

                            Catch ex As Exception
                                _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "Error Sending SMS: " & ex.ToString)

                            End Try
                             

                        Case Else
                            Try
                                _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "No GSM Phone / Modem Connected")

                            Catch ex As Exception
                                _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "Error Sending SMS: " & ex.ToString)
                            End Try

                    End Select








                Case "RECEIVE"
        Try
            If comm.IsConnected() = True Then
                Try
                    Dim MsgLocation As Integer
                    Dim counter As Integer = 0
                    '       counter = 0

                    ' Dim messages As DecodedShortMessage() = comm.ReadMessages(PhoneMessageStatus.ReceivedUnread, PhoneStorageType.Sim)
                    ' Dim messages As DecodedShortMessage() = comm.ReadMessages(PhoneMessageStatus.All, PhoneStorageType.Phone)
                    Dim messages As DecodedShortMessage() = comm.ReadMessages(PhoneMessageStatus.ReceivedUnread, PhoneStorageType.Phone)
                    For Each Message In messages
                        MsgLocation = Message.Index
                        _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write SmsReceive", " message recu: " & Message.Data.UserDataText)
                        ' Message.Data.UserDataText) '& (Message.Status).ToString & (Message.Storage).ToString & (Message.Index).ToString
                        counter = counter + 1
                        'Thread.Sleep(1000)
                        Try
                            comm.DeleteMessage(MsgLocation, PhoneStorageType.Phone)
                        Catch ex As Exception
                            _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write SmsReceive", "error deleting from inbox")
                            Exit Sub
                        Finally
                            messages = Nothing
                        End Try
                    Next
                    If counter = 0 Then
                        _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write SmsReceive", "Aucun message n'a été recu")
                        'comm.Close()
                    End If
                    Exit Sub

                    '  If counter > 0 Then
                    ' NewMsg = True
                    ' End If
                Catch ex As GsmComm.GsmCommunication.CommException
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DRIVER, "GSM Write SmsReceive", "error:" & ex.InnerException.Message)
                End Try
            End If
        Catch ex As GsmComm.GsmCommunication.CommException
            _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DRIVER, "GSM Write SmsReceive", "error:" & ex.InnerException.Message)
        End Try

                Case "CALL"
        _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write CALL", "Not Yet Implemented")
                Case Else
        _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DRIVER, "GSM Write", "Commande " & Command & " non gérée")
            End Select

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GSM Write", ex.Message)
        End Try

    End Sub

    ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GSM DeleteDevice", ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice
        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GSM NewDevice", ex.Message)
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

    ''' <summary>Creation d'un objet de type</summary>
    ''' <remarks></remarks>
    Public Sub New()
        Try
            _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString

            'Liste des devices compatibles
            '_DeviceSupport.Add(ListeDevices.GENERIQUESTRING.ToString)
            _DeviceSupport.Add(ListeDevices.GENERIQUESTRING)

            'ajout des commandes avancées pour les devices
            'add_devicecommande("COMMANDE", "DESCRIPTION", nbparametre)
            Add_DeviceCommande("SEND", "envoyer un SMS", 1)
            Add_DeviceCommande("RECEIVE", "recevoir un ou des SMS", 0)
            Add_DeviceCommande("CALL", "Appeler", 0)

            Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", True)
            Add_ParamAvance("Mode", "Choisir le mode de connexion (PDU/TEXTE)", True)
            'Add_ParamAvance("storage", "sim card : true / gsm : false", True)
            'ajout des commandes avancées pour les devices

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Numero téléphone (ex: 0605040302)", "")
            Add_LibelleDevice("ADRESSE2", "@", "")

            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("MODELE", "Type de stockage", "Type de stockage : GSM/SIM", "GSM|SIM")
            Add_LibelleDevice("BAUDRATE", "Vitesse du port", " BAUD :300|600|1200|2400|9600|14400|19200|38400|57600|115200", "300|600|1200|2400|9600|14400|19200|38400|57600|115200")


            Add_LibelleDevice("REFRESH", "Refresh", "")
            Add_LibelleDevice("LASTCHANGEDUREE", "@", "")
            ' Add_LibelleDevice("ComPort", "COM7", "")
              ' Add_LibelleDevice("CommTimeout", "LastChange Durée", "")

        Catch ex As Exception
            ' WriteLog("ERR: New Exception : " & ex.Message)
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GSM New", ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick()

    End Sub

#End Region

#Region "Fonctions internes"

    ''' <summary>Ouvrir le port du modem</summary>
    Private Function ouvrir() As String
          Try
            'ouverture du port
            Select Case _MODE
                Case "PDU"
                    If Not _IsConnect Then
                        'vitesse du port 300, 600, 1200, 2400, 9600, 14400, 19200, 38400, 57600, 115200
                        Dim portnumber As Integer
                        If (Integer.TryParse(_Com.Substring(3), portnumber)) Then
                            comm = New GsmCommMain(portnumber, 57600, 100)
                        End If

                        comm.Open()
                        'comm.EnableMessageNotifications()
                        'comm.EnableMessageRouting()
                        Return ("Port " & _Com & " ouvert")
                    Else
                        Return ("Port " & _Com & " dejà ouvert")
                    End If
                Case "TEXTE"
                    If Not _IsConnect Then

                        ATport.ReadTimeout = 500
                        ATport.WriteTimeout = 500
                        ATport.Open()
                        Return ("Port " & _Com & " ouvert")
                    Else
                        Return ("Port " & _Com & " dejà ouvert")
                    End If
                Case Else
                    Return ("Port " & _Com & "  non ouvert")
            End Select


        Catch ex As Exception
            Return ("ERR: " & ex.Message)
        End Try
    End Function



    ''' <summary>Fermer le port du modem</summary>
    ''' <remarks></remarks>
    Private Function fermer() As String


        Select Case _MODE

            Case "PDU"

                Try
                    If _IsConnect Then
                        If (Not (comm Is Nothing)) Then ' The COM port exists.
                            If comm.IsOpen Then
                                comm.Close()
                                _IsConnect = False
                                Return ("Port " & _Com & " fermé")
                            Else
                                Return ("Port " & _Com & "  est déjà fermé")
                            End If
                        Else
                            Return ("Port " & _Com & " n'existe pas")
                        End If
                    Else
                        Return ("Port " & _Com & "  est déjà fermé (Disconnected)")
                    End If
                Catch ex As UnauthorizedAccessException
                    Return ("ERR: Port " & _Com & " IGNORE")
                    ' The port may have been removed. Ignore.
                End Try
                Return True


            Case "TEXTE"

                Try
                    If _IsConnect Then

                        If (Not (ATport Is Nothing)) Then ' The COM port exists.
                            If ATport.IsOpen Then
                                ATport.Close()
                                _IsConnect = False
                                Return ("Port " & _Com & " fermé")
                            Else
                                Return ("Port " & _Com & "  est déjà fermé")
                            End If
                        Else
                            Return ("Port " & _Com & " n'existe pas")
                        End If
                    Else
                        Return ("Port " & _Com & "  est déjà fermé (Disconnected)")
                    End If
                Catch ex As UnauthorizedAccessException
                    Return ("ERR: Port " & _Com & " IGNORE")
                    ' The port may have been removed. Ignore.
                End Try
                Return True

            Case Else
                Try
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GSM close", "unknow")
                 Catch ex As UnauthorizedAccessException
                    Return ("ERR: Port " & _Com & " IGNORE")
                    ' The port may have been removed. Ignore.
                End Try
                Return True
        End Select

    End Function


    Private Sub MessageReceived(ByVal sender As Object, ByVal e As MessageReceivedEventArgs) Handles comm.MessageReceived
        Dim obj As IMessageIndicationObject = e.IndicationObject

        Try
            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "GSM MessageReceived", "un message a été reçu")

            'If it's just a notification, print out the memory location of the new message
            If TypeOf obj Is MemoryLocation Then
                Dim loc As MemoryLocation = CType(obj, MemoryLocation)
                _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM MessageReceived", String.Format("New message received in storage {0}, index {1}.", loc.Storage, loc.Index))
                Exit Sub
            End If

            'If it's a complete message, then it was routed directly
            If TypeOf obj Is ShortMessage Then
                Dim msg As ShortMessage = CType(obj, ShortMessage)
                Dim pdu As SmsPdu = comm.DecodeReceivedMessage(msg)
                If TypeOf pdu Is SmsSubmitPdu Then
                    'Stored (sent/unsent) message
                    Dim data As SmsSubmitPdu = CType(pdu, SmsSubmitPdu)
                    ReceptionSMS("STORED", data.DestinationAddress, data.UserDataText, "")
                    Exit Sub
                End If

                If TypeOf pdu Is SmsDeliverPdu Then
                    'Received message
                    Dim data As SmsDeliverPdu = CType(pdu, SmsDeliverPdu)
                    ReceptionSMS("RECEIVED", data.OriginatingAddress, data.UserDataText, data.SCTimestamp.ToString())
                    Exit Sub
                End If

                If TypeOf pdu Is SmsStatusReportPdu Then
                    'Status report
                    Dim data As SmsStatusReportPdu = CType(pdu, SmsStatusReportPdu)
                    _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM MessageReceived", "STATUS REPORT, Recipient:" & data.RecipientAddress & ", Status:" & data.Status.ToString() & ", Timestamp: " & data.DischargeTime.ToString() & ", Message ref: " & data.MessageReference.ToString())
                    Exit Sub
                End If
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DRIVER, "GSM MessageReceived", "Typede SMS inconnu.")
                Exit Sub
            End If

            _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DRIVER, "GSM MessageReceived", "Notification inconnue.")
        Catch ex As Exception
            _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DRIVER, "GSM MessageReceived", "Exception" & ex.Message)
        End Try
    End Sub

    ''' <summary>Reception de SMS</summary>
    Private Sub ReceptionSMS(ByVal type As String, ByVal numero As String, ByVal texte As String, ByVal dateenvoi As String)
        Try
            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "GSM ReceptionSMS", "Rception d un SMS de type " & type & ", numéro " & numero & ", texte " & texte & ",  Date " & dateenvoi)

            'Recherche si un device affecté
            Dim listedevices As New ArrayList
            listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_Idsrv, numero, "", Me._ID, True)
            'un device trouvé on maj la value
            If (listedevices.Count = 1) Then
                listedevices.Item(0).Value = "RECEIVE - " & texte & "(" & dateenvoi & ")"
            ElseIf (listedevices.Count > 1) Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GSM ReceptionSMS", "Plusieurs composants correspondent à : " & numero & ":" & texte)
            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GSM ReceptionSMS", "Composant non trouvé : " & numero & ":" & texte)


                'Ajouter la gestion des composants bannis (si dans la liste des composant bannis alors on log en debug sinon onlog device non trouve empty)


            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GSM ReceptionSMS", "Exception" & ex.Message)
        End Try
    End Sub

#End Region


End Class