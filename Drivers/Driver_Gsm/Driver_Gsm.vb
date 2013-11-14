

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
Imports System.Threading



' Auteur : Fabien
' Date : 09/09/2012
'-------------------------------------------------------------------------------------
' Updated : 04/03/2013 ajout d'envoide sms en mode Texte
' a lire pour optimiser : http://grafikm.developpez.com/portcomm/ ( )
'-------------------------------------------------------------------------------------

' Driver GSM
''' <summary>Class Driver GSM </summary>
''' <remarks>Les drivers de votre modem doivent etre installés
''' Nécessite les dll GSMCommServer.dll,  GSMCommunication.dll, GSMCommShared.dll,PDUConverter.dll </remarks>


<Serializable()> Public Class Driver_Gsm


    Implements HoMIDom.HoMIDom.IDriver



#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    ' Dim _ID As String = "7396FA30-0050-11E2-9BC2-523B6288709B"
    Dim _ID As String = "596AD836-FA1D-11E1-BDE2-639C6188709B"
    Dim _Nom As String = "GSM"
    Dim _Enable As Boolean = False
    Dim _Description As String = "Driver GSM"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "GSM"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "COM4"
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
    Dim _AutoDiscover As Boolean = False

    'param avancé
    Dim _DEBUG As Boolean = False
    Dim _MODE As String = "PDU"
    Dim _BAUD As Integer = "9600"
    Dim _PinCode As String = ""
    Dim _Carnet As Boolean = False

#End Region

#Region "Variables Internes"

    Private ackreceived As Boolean = False
    Private WithEvents comm As GsmCommMain
    Private _SMSROUTING As Boolean = False

    Dim WithEvents ATport As New SerialPort
    Private port_name As String = ""

    Dim MODES As Array
    Dim STOCKAGE As Array
    ' declaration
    Private BufferIn(8196) As Byte
    Private DebutTrame As Boolean = False
    'Private DebutInfo As Boolean = False
    Private bytecnt As Integer = 0
    Private messcnt As Integer = 0

    '<NonSerialized()> Dim TimerSecond As New Timers.Timer 'Timer à la seconde
    Private recbuf(300), recbytes, recbits As Byte
    Private InfoTrame() As String
    Private mess As Boolean = False
    Private trame As Boolean = False
    'Private CommandeAT As String = ""
    'Dim startdecodesms As Boolean = False



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
            If value >= 1 Then
                _Refresh = value

            End If
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
                    'retour = "Veuillez saisir un message de taille inférieure à 150 characteres("")

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
        Dim retour As String = ""

        'récupération des paramétres avancés
        Try
            _DEBUG = _Parametres.Item(0).Valeur.ToString.ToUpper
            _MODE = _Parametres.Item(1).Valeur.ToString.ToUpper
            _BAUD = _Parametres.Item(2).Valeur.ToString.ToUpper
            _PinCode = _Parametres.Item(3).Valeur.ToString.ToUpper
            _Carnet = _Parametres.Item(4).Valeur.ToString.ToUpper

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GSM Start", "Erreur dans les paramétres avancés. utilisation des valeur par défaut" & ex.Message)
        End Try

        'ouverture de la communication avec le GSM
        Try
            If _Com <> "" Then
                retour = ouvrir(_Com)


               
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
            If _IsConnect = False Then
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "GSM Write", "Le driver n'est pas démarré, impossible d'écrire sur le port")
                Exit Sub
            End If
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
                                        Objet.Value = "SEND: " & Parametre1
                                    End If
                                Else
                                    _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "No GSM Phone / Modem Connected")
                                End If
                            Catch ex As Exception
                                _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "Error Sending SMS: " & ex.ToString)
                            End Try

                        Case "TEXTE"
                            Try
                                ' MsgBox("0")
                                If ATport.IsOpen Then
                                    RemoveHandler ATport.DataReceived, AddressOf DataReceived
                                   
                                    'a titre indicatif ( passerelle sms )
                                    'Bouygues Télécom : +33660003000
                                    'Orange : +33689004000
                                    'SFR : +33609001390
                                    'sending AT commands
                                    'ATport.WriteLine("AT")
                                    Dim sIncomming As String = ""
                                    If Parametre1 = "" Then
                                        _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "SMS à envoyer vide, annulation")
                                    Else

                                        '   If IsSimpinOk() = True Then
                                        _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "SMS envoi en cours : " & Parametre1 & " à " & Objet.adresse1.ToString)


                                        ' SMSPort.WriteLine("AT+CSCA=""+33689004000""" & vbCrLf) 'set service center address (orange) 
                                        ATport.DiscardOutBuffer()
                                        ATport.WriteLine("AT+CMGS=" & Objet.adresse1.ToString & vbCrLf) ' enter the mobile number whom you want to send the SMS.
                                        Thread.Sleep(100)
                                       
                                        _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "tentative d'envoi : " & sIncomming.Replace(vbCr, "").Replace(vbLf, ""))

                                        ATport.WriteLine(Parametre1 & vbCrLf & Chr(26)) 'SMS sending
                                        Thread.Sleep(400)

                                        '+CMGS:xx  Le modem retourne le numéro d'identification du SMS
                                        ' sIncomming = ""
                                        sIncomming = ""
                                        Do Until Left(sIncomming, 6) = "+CMGS:"
                                            Thread.Sleep(600)
                                            sIncomming = ATport.ReadLine()
                                            sIncomming = (sIncomming.Replace(vbCr, "").Replace(vbLf, ""))
                                        Loop

                                        _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "tentative d'envoi : " & sIncomming.Replace(vbCr, "").Replace(vbLf, ""))

                                        If Left(sIncomming, 6) = "+CMGS:" Then
                                            ' _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "SMS envoyé : " & sIncomming)
                                            _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "SMS envoyé : (" & Parametre1.Length & ")" & Parametre1 & " à " & Objet.adresse1.ToString)
                                            Objet.Value = "SEND: " & Parametre1
                                        End If

                                        ATport.DiscardInBuffer()
                                        ATport.DiscardOutBuffer()
                                        AddHandler ATport.DataReceived, New SerialDataReceivedEventHandler(AddressOf DataReceived)

                                    End If
                                    ' End If
                                Else
                                    _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "No GSM Phone / Modem Connected")
                                End If
                            Catch ex As Exception
                                _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "Error Sending SMS: " & ex.ToString)
                            End Try

                        Case Else : _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "No MODE configured")
                    End Select

                Case "RECEIVE" : Lecture_sms()

                Case "CALL"
                    Select Case _MODE
                        Case "TEXTE"
                            Try
                                _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write CALL", "CALL mode TEXTE en cours")
                                ATport.WriteLine("ATD" & Parametre1 & vbCrLf) 'Telephone au numero du composant


                                'Verbose result code with ATV0 set  Description OK 0 if the call succeeds, for voice call only
                                'CONNECT <speed> 10,11,12, if the call succeeds, for data calls only, 13,14,15 <speed> takes the value negotiated by the product.
                                'BUSY 7 If the called party is already in communication
                                'NO ANSWER 8 If no hang up is detected after a fixed network(time - out)
                                'NO CARRIER 3 Call setup failed or remote user release. Use the AT+CEER command to know the failure cause

                            Catch ex As Exception
                                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DRIVER, "GSM Write CALL", "error:" & ex.Message)
                            End Try
                        Case "PDU"
                            Try
                                _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write CALL", "CALL PDU Not Yet Implemented")
                            Catch ex As Exception
                                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DRIVER, "GSM Write CALL", "error:" & ex.Message)
                            End Try
                        Case Else : _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write CALL", "Appel Impossible : aucun mode selectionné")
                    End Select
                Case Else : _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DRIVER, "GSM Write", "Commande " & Command & " non gérée")
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
            _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString()

            'Liste des devices compatibles
            '_DeviceSupport.Add(ListeDevices.GENERIQUESTRING.ToString)
            _DeviceSupport.Add(ListeDevices.GENERIQUESTRING)

            'ajout des commandes avancées pour les devices
            'add_devicecommande("COMMANDE", "DESCRIPTION", nbparametre)
            Add_DeviceCommande("SEND", "envoyer un SMS", 1)
            Add_DeviceCommande("RECEIVE", "recevoir un ou des SMS", 0)
            Add_DeviceCommande("CALL", "Appeler", 0)


            Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", True)
            Add_ParamAvance("Mode", "Choisir le mode de connexion (PDU/TEXTE)", "PDU")
            Add_ParamAvance("Baud", "Vitesse du port : 300|600|1200|2400|9600|14400|19200|38400|57600|115200", "9600")
            Add_ParamAvance("PinCode", "En test, inscrire le code pin de la carte SIM", "")
            Add_ParamAvance("Carnet", "Recuperer le carnet d'adresses (True/False)", False)
            'Add_ParamAvance("storage", "sim card : true / gsm : false", True)
            'ajout des commandes avancées pour les devices

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Numero Tel (ex: +33612345678)", "")
            Add_LibelleDevice("ADRESSE2", "@", "")

            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("MODELE", "Type de stockage", "Type de stockage : GSM/SIM", "GSM|SIM")

        Catch ex As Exception
            ' WriteLog("ERR: New Exception : " & ex.Message)
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GSM New", ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)

    End Sub

#End Region
#Region "Fonctions internes"

    ''' <summary>Ouvrir le port du modem</summary>
    ''' <param name="numero">Nom/Numero du port COM</param>
    ''' <remarks></remarks>
    Private Function ouvrir(ByVal numero As String) As String
        port_name = numero 'pour se rapeller du nom du port 
        Dim sIncomming As String
        Try

            If Not _IsConnect Then

                Try

                    'lecture AT+CMGF=? pour connaitre les modes supportés par le modem gsm connecté PDU ou TEXT
                    ' reponse 0 pour le mode PDU
                    ' reponse 1 pour le mode TEXT
                    _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "   * ouverture du port : " & port_name & " à " & _BAUD & " bauds")
                    ATport.PortName = port_name 'nom du port : COM1
                    ATport.BaudRate = _BAUD 'vitesse du port 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 57600, 115200
                    ATport.Parity = Parity.None 'pas de parité
                    ATport.StopBits = StopBits.One '1 bit d'arrêt par octet
                    ATport.DataBits = 8 'nombre de bit par octet

                    'ATport.Encoding = System.Text.Encoding.GetEncoding(1252)  

                    'Extended ASCII (8-bits)

                    ATport.Handshake = Handshake.RequestToSend
                    ATport.ReadBufferSize = CInt(4096)
                    'RS232Port.ReceivedBytesThreshold = 1
                    ATport.ReadTimeout = 300
                    ATport.WriteTimeout = 300



                    ATport.Open()
                    '  AddHandler ATport.DataReceived, New SerialDataReceivedEventHandler(AddressOf DataReceived)

                Catch ex As Exception
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GSM Start", "   * Erreur dans l'ouverture du port : " & ex.Message)
                    Return ("ERR: Port COM non ouvert : " & ex.Message)
                End Try
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "GSM Ouvrir", "Testing du model/téléphone :")

                Try

                    'recupere le manufacturer

                    sIncomming = ""
                    Dim MyIncomingIndex As Integer = 0
                    Dim mystring(MyIncomingIndex) As String
                    ATport.WriteLine("AT+CGMI" & vbCrLf) ' vbcrlf

                    Thread.Sleep(50)
                    Do Until (Left(sIncomming, 2) = "OK")
                        ReDim Preserve mystring(MyIncomingIndex)
                        sIncomming = ATport.ReadLine()
                        mystring(MyIncomingIndex) = sIncomming.Replace(vbCr, "").Replace(vbLf, "")
                        MyIncomingIndex = MyIncomingIndex + 1
                    Loop
                    Dim phonemanufacturer = mystring(mystring.Length - 3)
                    _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "   * AT+CGMI ( manufacturer )")
                    _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "    -> " & phonemanufacturer)


                    Thread.Sleep(200)
                    sIncomming = ""
                    '         
                    MyIncomingIndex = 0
                    Dim mystring2(MyIncomingIndex) As String
                    ATport.WriteLine("AT+CGMM" & vbCrLf) ' vbcrlf
                    Do Until (Left(sIncomming, 2) = "OK")
                        ReDim Preserve mystring2(MyIncomingIndex)
                        sIncomming = ATport.ReadLine()
                        mystring2(MyIncomingIndex) = sIncomming.Replace(vbCr, "").Replace(vbLf, "")
                        MyIncomingIndex = MyIncomingIndex + 1
                    Loop
                    Dim phonemodel = mystring2(mystring2.Length - 3)
                    _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "   * AT+CGMM ( model )")
                    _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "    -> " & phonemodel)

                    ''''''''''''
                    ' suppresion de la verification des commandes supporté ( trop peu de model supporte cette commande )
                    ''''''''''''

                    '                   If Not ((phonemanufacturer = "NOKIA") Or (phonemanufacturer = " WAVECOM MODEM") Or (phonemanufacturer = "huawei")) Then
                    '' Si ce n'est pas un Nokia
                    '                   ' regardons ce que votre modem a dans le ventre ! 
                    '                  ' 
                    '                  _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "   * AT+CLAC Start listes des commandes du modem :")
                    '                   ATport.WriteLine("AT+CLAC" & vbCrLf) ' vbcrlf
                    '                  Thread.Sleep(100)
                    '                 sIncomming = ATport.ReadLine()
                    '                _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "    -> " & sIncomming.Replace(vbCr, "").Replace(vbLf, ""))
                    '               End If



                    '               '+CPIN: (READY,SIM PIN,SIM PUK,SIM PIN2,SIM PUK2,PH-SIM PIN,PH-NET PIN,PH-NETSUB PIN,PH-SP PIN,PH-CORP PIN,PH-ESL PIN,PH-SIMLOCK PIN,BLOCKED)
                    '               'AT+CPIN? doit on rentrer un code pin ?
                    '               sIncomming = ""
                    '               ' Do Until (Left(sIncomming, 12) = "+CPIN: READY" Or Left(sIncomming, 14) = "+CPIN: SIM PIN" Or Left(sIncomming, 14) = "+CPIN: SIM PUK" Or Left(sIncomming, 15) = "+CPIN: SIM PIN2" Or Left(sIncomming, 15) = "+CPIN: SIM PUK2" Or Left(sIncomming, 17) = "+CPIN: PH-SIM PIN" Or Left(sIncomming, 14) = "+CPIN: BLOCKED(")")
                    '               ATport.WriteLine("AT+CPIN?" & vbCrLf) ' vbcrlf
                    '               Thread.Sleep(100)
                    '               sIncomming = ATport.ReadLine()
                    '               If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "GSM Mode", "    -> " & sIncomming.Replace(vbCr, "").Replace(vbLf, ""))
                    '
                    '                    ' Loop
                    '                   _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "   * AT+CPIN")
                    '                    _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "    -> " & sIncomming.Replace(vbCr, "").Replace(vbLf, ""))
                    '
                    '                    ' on insère le code pin
                    '                    If sIncomming.Replace(vbCr, "").Replace(vbLf, "") = "+CPIN: SIM PIN" Then
                    ' If _PinCode <> "" Then
                    ' sIncomming = ""
                    ' Do Until (Left(sIncomming, 2) = "OK" Or Left(sIncomming, 11) = "+CME ERROR:" Or Left(sIncomming, 5) = "ERROR")
                    ' ATport.WriteLine("AT+CPIN=" & _PinCode & vbCrLf) ' vbcrlf
                    ' If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "GSM Mode", "    -> AT+CPIN=" & _PinCode)
                    ' Thread.Sleep(100)
                    ' sIncomming = ATport.ReadLine()
                    ' If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "GSM Mode", "    -> " & sIncomming.Replace(vbCr, "").Replace(vbLf, ""))
                    ' Loop
                    ' _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "    -> AT+CPIN=" & _PinCode & " - " & sIncomming.Replace(vbCr, "").Replace(vbLf, ""))
                    '
                    '            Else
                    '                    _Server.Log(Server.TypeLog.INFO,                     Server.TypeSource.DRIVER, "GSM Mode", "    -> Votre modem indique qu'il est necessaire d 'entrée le code PIN de votre carte SIM. Merci de remplir le parametre Code PIN du Driver(")")
                    '
                    '            End If

                    '                    End If


                    Thread.Sleep(200)
                    sIncomming = ""
                    '    'quel heure est  il ?

                    MyIncomingIndex = 0
                    Dim mystring3(MyIncomingIndex) As String
                    ATport.WriteLine("AT+CCLK?" & vbCrLf) ' vbcrlf
                    Do Until (Left(sIncomming, 2) = "OK")
                        ReDim Preserve mystring3(MyIncomingIndex)
                        sIncomming = ATport.ReadLine()
                        mystring3(MyIncomingIndex) = sIncomming.Replace(vbCr, "").Replace(vbLf, "")
                        MyIncomingIndex = MyIncomingIndex + 1
                    Loop
                    Dim Modemhoraire = mystring3(mystring3.Length - 3)
                    _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "   * AT+CCLK? ( horaire )")
                    _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "    -> " & Modemhoraire)

                    Thread.Sleep(200)
                    sIncomming = ""
                    'AT+CMFG=?  lister les mode supporté
                    _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "   * Lecture des mode Supportés")
                    MyIncomingIndex = 0
                    Dim mystring4(MyIncomingIndex) As String
                    ATport.WriteLine("AT+CMGF=?" & vbCrLf) ' vbcrlf
                    Do Until (Left(sIncomming, 2) = "OK")
                        ReDim Preserve mystring4(MyIncomingIndex)
                        sIncomming = ATport.ReadLine()
                        mystring4(MyIncomingIndex) = sIncomming.Replace(vbCr, "").Replace(vbLf, "")
                        MyIncomingIndex = MyIncomingIndex + 1
                    Loop

                    Dim Modemmodem = mystring4(mystring4.Length - 3)
                    _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "   * AT+CMGF=? ( mode )")
                    _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "    -> " & Modemmodem)
                    sIncomming = Modemmodem

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
                                _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM MODE", "    -> Mode " & m & " (PDU) possible")
                            Case "1"
                                MODES(m) = "TEXTE"
                                _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM MODE", "    -> Mode " & m & " (TEXTE) possible")
                            Case Else
                                MODES(m) = "UNKNOW"
                                _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM MODE", "    -> Mode " & m & " inconnu (ne sera pas utilisé)")
                        End Select
                    Next


                    '  Do Until sIncomming = Nothing
                    ' Thread.Sleep(100)
                    '   sIncomming = ATport.ReadLine()
                    ' If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "GSM Mode", "   * flush: " & sIncomming.Replace(vbCr, "").Replace(vbLf, ""))
                    ' sIncomming = ATport.ReadLine()

                    '  Loop

                    'AT+CPMS=?  stokage supporté
                    Thread.Sleep(200)
                    sIncomming = ""
                    _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "   * Lecture des mode Supportés")
                    MyIncomingIndex = 0
                    Dim mystring5(MyIncomingIndex) As String
                    ATport.WriteLine("AT+CPMS=?" & vbCrLf) ' vbcrlf
                    Do Until (Left(sIncomming, 2) = "OK")
                        ReDim Preserve mystring5(MyIncomingIndex)
                        sIncomming = ATport.ReadLine()
                        mystring5(MyIncomingIndex) = sIncomming.Replace(vbCr, "").Replace(vbLf, "")
                        MyIncomingIndex = MyIncomingIndex + 1
                    Loop

                    Dim TypeStockage = mystring5(mystring5.Length - 3)
                    _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM TypeStockage", "   * AT+CPMS=? ( stockage )")
                    '_Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM TypeStockage", "    -> " & TypeStockage)
                    sIncomming = TypeStockage.Replace("+CPMS: ", "")
                    Dim indexts As Integer
                    Dim response(indexts)


                    Dim stringSeparators() As String = {"),("}
                    response = TypeStockage.Replace("+CPMS: ", "").Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries)
                    Dim i As Integer
                    For i = 0 To response.Length - 1
                        response(i) = response(i).Replace("""", "")
                        response(i) = response(i).Replace("(", "")
                        response(i) = response(i).Replace(")", "")
                        response(i) = response(i).Replace(",", "")
                    Next
                    'MsgBox(response.Length)
                    'For i = 0 To response.Length - 1
                    'MsgBox(response(i))
                    'Next

                    Dim tmpindex
                    Dim offsset = 0
                    ' If response(0).length > 0 Then

                    For i = 0 To response.Length - 1


                        offsset = 0

                        For tmpindex = 0 To ((response(i).ToString.Length / 2) - 1)
                            offsset = offsset + 1
                            _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM TypeStockage", "    -> stockage " & Mid(response(i), (tmpindex + offsset), 2) & " possible")

                            ' MsgBox(Mid(response(0), (tmpindex + offsset), 2))
                        Next

                    Next

                    'Description
                    'SM
                    'Read SMS messages from the SIM card. This storage is supported on every GSM phone,
                    ' because a SIM card should always be present. Usually a SIM card can store up to 15 messages.
                    '
                    'Me
                    'Read SMS messages from the modem or mobile phone memory.
                    ' The number of messages that can be stored here depends on the size of the phones memory.
                    '
                    'MT
                    'Read SMS messages from all storages on the mobile phone.
                    ' For instance when the phone supports "ME" and "SM", the "MT"
                    ' memory combines the "ME" and "SM" memories as if it was a single storage.
                    '
                    'BM
                    'This storage is only used to read stored incoming cell broadcast
                    ' messages. It is normally not used to store SMS messages.
                    'SR
                    'When you enable status reports when sending SMS messages,
                    ' the status reports that are received are stored in this memory.
                    ' These reports can read the same way as SMS messages

                    '_Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM TypeStockage", "a titre indicatif ( non traité ):" & sIncomming)

                    ' Dim startIndex2 As Integer = sIncomming.IndexOf("(") + "(".Length
                    ' Dim endIndex2 As Integer = sIncomming.IndexOf(")")
                    ' Dim result2 As String = sIncomming.Substring(startIndex2, endIndex2 - startIndex2)
                    ' _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM TypeStockage", "Read:" & result2)
                    'STOCKAGE = result2.Split(New Char() {","c})
                    'Dim SuportedStockage As String = ""
                    ' For s = 0 To (STOCKAGE.Length - 1)
                    ''
                    '' (("SM","BM","SR"),("SM")) ou ME)
                    'Select Case STOCKAGE(s).ToString
                    '    Case "SM"
                    '' MODES(m) = "PDU"
                    '_Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM TypeStockage", "    -> Mode " & s & " (SM) possible")
                    '    Case "ME"
                    '' MODES(m) = "TEXTE"
                    '_Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM TypeStockage", "    -> Mode " & s & " (ME) possible")
                    '    Case "MT"
                    '' MODES(m) = "PDU"
                    '' _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM TypeStockage", "    -> Mode " & s & " (MT) possible")
                    '   Case "BM"
                    '' MODES(m) = "TEXTE"
                    '_Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM TypeStockage", "    -> Mode " & s & " (BM) possible")
                    '    Case "SR"
                    '' MODES(m) = "PDU"
                    '_Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM TypeStockage", "    -> Mode " & s & " (SR) possible")
                    '    Case Else
                    'STOCKAGE(s) = "UNKNOW"
                    '_Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM TypeStockage", "    -> Mode " & s & " inconnu (ne sera pas utilisé)")
                    'End Select
                    'Next
                    '

                    ATport.DiscardInBuffer()
                    ATport.DiscardOutBuffer()

                    sIncomming = Nothing
                    _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "   * Fermeture du port COM pour le testing")
                    ATport.Close()
                Catch ex As Exception
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GSM Start", "   * Erreur dans les tests de lecture des infos: " & ex.Message)
                End Try



                'Verification du MODE de connexion
                _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "   * Verification du mode selectionné")
                Try
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
                                _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "    -> SuportedMode : '" & MODES(n).ToString & "'.")
                                ModeToUse = MODES(n)
                            Else
                                ModeToUse = ModeToUse & " ou " & MODES(n)
                            End If
                        Next
                        _IsConnect = False
                        Return ("ERR: " & "Le mode choisi '" & _MODE & "' n'est pas correct, Veuillez choisir le mode " & ModeToUse & ".")
                        Exit Function
                    Else
                        _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "    -> Mode '" & _MODE & "' valide.")
                    End If
                Catch ex As Exception
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GSM Start", "    -> Erreur Dans la verification du mode " & ex.Message)
                End Try

               


            End If
        Catch ex As Exception
            Return ("ERR: " & ex.Message)
        End Try

        'ouverture du port
        Try
            Select Case _MODE
                Case "PDU"
                    If Not _IsConnect Then
                        _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "   * ouverture du port : " & port_name & " à " & _BAUD & " bauds")
                        Dim portnumber As Integer
                        If (Integer.TryParse(port_name.Substring(3), portnumber)) Then
                            comm = New GsmCommMain(portnumber, _BAUD, 100) 'vitesse du port(300, 600, 1200, 2400, 9600, 14400, 19200, 38400, 57600, 115200)
                        End If
                        comm.Open()
                        _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Start", "   * Activation des notifications")
                        comm.EnableMessageNotifications()
                        Try
                            comm.EnableMessageRouting()
                            _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Start", "   * Activation du routage des SMS")
                            _SMSROUTING = True



                            PhoneDir()

                        Catch ex As Exception
                            _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Start", "   * Désactivation du routage des SMS car non supporté par le modem")
                            _SMSROUTING = False
                        End Try

                        Return ("Port " & _Com & " ouvert à " & _BAUD & " bauds.")
                    Else
                        Return ("Port " & _Com & " dejà ouvert")
                    End If

                Case "TEXTE"
                    If Not _IsConnect Then
                        ATport.PortName = port_name 'nom du port : COM1
                        ATport.BaudRate = _BAUD 'vitesse du port 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 57600, 115200
                        ATport.Parity = Parity.None 'pas de parité
                        ATport.StopBits = StopBits.One '1 bit d'arrêt par octet
                        ATport.DataBits = 8 'nombre de bit par octet
                        ' ATport.Encoding = System.Text.Encoding.GetEncoding(1252)  

                        'Extended ASCII (8-bits)
                        '  ATport.Encoding = Encoding.GetEncoding("iso-8859-1")
                        ATport.Handshake = Handshake.RequestToSend
                        ATport.ReadBufferSize = CInt(4096)
                        'ATport.Encoding = Encoding.GetEncoding("iso-8859-1")
                        'RS232Port.ReceivedBytesThreshold = 1
                        ATport.ReadTimeout = 300
                        ATport.WriteTimeout = 300
                        ATport.Open()

                        Thread.Sleep(200)
                        If IsSimpinOk() = True Then

                            sIncomming = ""
                            Do Until Left(sIncomming, 2) = "OK"
                                'SMS auformat Texte
                                ATport.WriteLine("AT+CMGF=1" & vbCrLf) ' vbcrlf
                                Thread.Sleep(100)
                                sIncomming = ATport.ReadLine()
                                sIncomming = sIncomming.Replace(vbCr, "").Replace(vbLf, "")
                                ' MsgBox(sIncomming)
                                _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "AT+CMGF=1 : " & sIncomming)
                            Loop

                          
                            ' More Message to Send
                            '+CMMS=2 for multiple messages
                            '     sIncomming = ""
                            '     Do Until Left(sIncomming, 2) = "OK"
                            'ATport.WriteLine("AT+CMMS=2" & vbCrLf) ' 
                            '    Thread.Sleep(100)
                            '     sIncomming = ATport.ReadLine()
                            '     sIncomming = sIncomming.Replace(vbCr, "").Replace(vbLf, "")
                            '     _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "AT+CMMS=2 : " & sIncomming)
                            '     Loop

                            Thread.Sleep(200)
                            sIncomming = ""
                            ' SMSPort.WriteLine("AT+CSCA=""+33689004000""" & vbCrLf) 

                            'set service center address (orange) 
                            ATport.WriteLine("AT+CMEE=1" & vbCrLf) '
                            Thread.Sleep(100)
                            sIncomming = ATport.ReadLine()
                            sIncomming = (sIncomming.Replace(vbCr, "").Replace(vbLf, ""))
                            'MsgBox(sIncomming)
                            _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "AT+CMEE=1 : " & sIncomming)

                            Thread.Sleep(200)
                            sIncomming = ""
                            ' A GSM/GPRS modem or mobile phone uses +CMTI ( AT+CNMI=X,2,X,X,X ) 
                            'to forward a newly received SMS message to the computer / PC.
                            'A GSM/GPRS modem or mobile phone uses +CMTI ( AT+CNMI=X,1,X,X,X )to notify the computer / PC
                            ' that a new SMS message has been received and the memory location where it is stored


                            'reponse a trapper lors du handle lors de la reception  : +CMTI: "SM",<index>

                            ATport.WriteLine("AT+CNMI=1,1,0,0,0" & vbCrLf) '
                            Thread.Sleep(100)
                            sIncomming = ATport.ReadLine()
                            sIncomming = (sIncomming.Replace(vbCr, "").Replace(vbLf, ""))
                            _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "AT+CNMI=1,1,0,0,0 : " & sIncomming)

                            _IsConnect = True


                            PhoneDir()

                            sIncomming = Nothing

                            'suppression des 50 msg 
                            ' For i = 1 To 50
                            ' ATport.WriteLine("AT+CMGD=" & i & vbCrLf) '
                            ' Thread.Sleep(500)
                            ' _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "delete msg : " & i)
                            ' Next

                            ATport.DiscardInBuffer()
                            ATport.DiscardOutBuffer()

                            AddHandler ATport.DataReceived, New SerialDataReceivedEventHandler(AddressOf DataReceived)
                            Return ("Port " & port_name & " ouvert à " & _BAUD & " bauds.")




                        End If
                    Else
                        Return ("Port " & port_name & " dejà ouvert")
                    End If
                    Return ("Port " & port_name & "  non ouvert")
                Case Else
                    Return ("Port " & port_name & "  non ouvert")
            End Select
        Catch ex As Exception
            Return ("ERR: Port COM non ouvert" & ex.Message)
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
                                _IsConnect = False
                                Return ("Port " & _Com & "  est déjà fermé")
                            End If
                        Else
                            _IsConnect = False
                            Return ("Port " & _Com & " n'existe pas")
                        End If
                    Else
                        Return ("Port " & _Com & "  est déjà fermé (Disconnected)")
                    End If
                Catch ex As UnauthorizedAccessException
                    Return ("ERR: Port " & _Com & " IGNORE") ' The port may have been removed. Ignore.
                End Try

            Case "TEXTE"
                Try
                    If _IsConnect Then
                        If (Not (ATport Is Nothing)) Then ' The COM port exists.
                            If ATport.IsOpen Then
                                RemoveHandler ATport.DataReceived, AddressOf DataReceived

                                ATport.Close()

                                _IsConnect = False
                                Return ("Port " & _Com & " fermé")
                            Else
                                _IsConnect = False
                                Return ("Port " & _Com & "  est déjà fermé")
                            End If
                        Else
                            ATport.Close()
                            _IsConnect = False
                            Return ("Port " & _Com & " n'existe pas")
                        End If
                    Else
                        Return ("Port " & _Com & "  est déjà fermé (Disconnected)")
                    End If
                Catch ex As UnauthorizedAccessException
                    Return ("ERR: Port " & _Com & " IGNORE") ' The port may have been removed. Ignore.
                End Try

            Case Else
                Try
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GSM close", "unknow")
                    Return ""
                Catch ex As UnauthorizedAccessException
                    Return ("ERR: Port " & _Com & " IGNORE") ' The port may have been removed. Ignore.
                End Try
        End Select
    End Function

    ''' <summary>Lit les SMS non lus sur le modem/tel</summary>
    ''' <remarks></remarks>
    Private Sub Lecture_sms()
        Try
            If _Enable = False Then Exit Sub
            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "GSM Lecture_sms", "Lecture des nouveaux SMS")

            Select Case _MODE
                Case "PDU"
                    If comm.IsConnected() = True Then
                        Try
                            Dim MsgLocation As Integer
                            Dim counter As Integer = 0
                            ' Dim messages As DecodedShortMessage() = 

                            comm.ReadMessages(PhoneMessageStatus.ReceivedUnread, PhoneStorageType.Sim)
                            ' Dim messages As DecodedShortMessage() = 

                            comm.ReadMessages(PhoneMessageStatus.All, PhoneStorageType.Phone)
                            Dim messages As DecodedShortMessage() = comm.ReadMessages(PhoneMessageStatus.ReceivedUnread, PhoneStorageType.Phone)
                            For Each Message In messages
                                If TypeOf Message.Data Is SmsDeliverPdu Then
                                    Dim data As SmsDeliverPdu = CType(Message.Data, SmsDeliverPdu)
                                    ReceptionSMS("RECEIVED", data.OriginatingAddress, data.UserDataText, data.SCTimestamp.ToString())
                                ElseIf TypeOf Message.Data Is SmsSubmitPdu Then
                                    Dim data As SmsSubmitPdu = CType(Message.Data, SmsSubmitPdu)
                                    ReceptionSMS("STORED", data.DestinationAddress, data.UserDataText, "")
                                    '_Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Lecture_sms", " -> sms envoyé à " & data.DestinationAddress & " : " & data.UserDataText)
                                ElseIf TypeOf Message.Data Is SmsStatusReportPdu Then
                                    Dim data As SmsStatusReportPdu = CType(Message.Data, SmsStatusReportPdu)
                                    _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Lecture_sms", " -> sms status report de " & data.RecipientAddress & ", Status: " & data.Status.ToString() & ", Date: " & data.DischargeTime.ToString())
                                Else
                                    _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Lecture_sms", " -> Format de SMS non reconnu: " & Message.Data.UserDataText)
                                End If

                                ' Message.Data.UserDataText) '& (Message.Status).ToString & (Message.Storage).ToString & (Message.Index).ToString
                                counter = counter + 1
                                'Thread.Sleep(1000)
                                MsgLocation = Message.Index
                                Try
                                    comm.DeleteMessage(MsgLocation, PhoneStorageType.Phone)
                                    If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "GSM Lecture_sms", " -> Message effacé.")
                                Catch ex As Exception
                                    _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Lecture_sms", " -> Error deleting from inbox")
                                    Exit Sub
                                Finally
                                    messages = Nothing
                                End Try
                            Next
                            If counter = 0 Then
                                _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Lecture_sms", "Aucun message n'a été recu")
                                'comm.Close()
                            End If
                            Exit Sub
                            '  If counter > 0 Then NewMsg = True
                        Catch ex As GsmComm.GsmCommunication.CommException
                            _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DRIVER, "GSM Lecture_sms", "error:" & ex.InnerException.Message)
                        End Try

                    Else
                        _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DRIVER, "GSM Lecture_sms", "Téléphone/Modem non connecté")
                    End If
                Case "TEXTE"


                    Try

                        If ATport.IsOpen Then

                            Dim sIncomming As String = ""

                            ATport.WriteLine("AT+CPMS=" & Chr(34) & "SM" & Chr(34) & vbCrLf) ' 
                            _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "reception sms : " & "AT+CPMS=" & Chr(34) & "SM" & Chr(34))

                            '+CMGS:xx  Le modem retourne le numéro d’identification du SMS
                            ' sIncomming = ""
                            Do Until Left(sIncomming, 6) = "+CPMS:"
                                Thread.Sleep(50)
                                sIncomming = ATport.ReadLine()
                                sIncomming = (sIncomming.Replace(vbCr, "").Replace(vbLf, ""))
                                _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "reception sms : " & sIncomming)

                            Loop


                            If Left(sIncomming, 6) = "+CPMS:" Then
                                _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "reception sms : " & (sIncomming.Replace(vbCr, "").Replace(vbLf, "")))

                                Dim msgs
                                msgs = sIncomming.Replace("+CPMS: ", "").Replace(vbCr, "").Replace(vbLf, "").Split(New Char() {","c})


                                ' MsgBox(msgs(0))
                                For i = Convert.ToInt32(msgs(0)) To 1 Step -1
                                    _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "reception sms : " & "AT+CMGR=" & i)
                                    ATport.WriteLine("AT+CMGR=" & i & vbCrLf)
                                    '+CMGR: <status>,<from_address>,<mr>,<scts><CRLF><data> 
                                    '  at(+cmgr = 6)
                                    '+CMGR: "REC READ","<phoneNumber>",,"13/03/09,22:19:14+04"
                                    ' msgtexte
                                    '
                                    'OK

                                    Thread.Sleep(400)
                                    'ATport.WriteLine("AT+CMGD=" & i & vbCrLf)
                                    ' sIncomming = ATport.ReadLine() ' texte
                                    ' MsgBox(sIncomming)
                                    'sIncomming.Replace(vbCrLf, "|").Split(New Char() {","c})
                                    ' MsgBox(sIncomming.Replace(vbCrLf, "|"))
                                    _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "reception sms : " & sIncomming)
                                    Thread.Sleep(400)

                                Next




                            End If


                        End If

                    Catch ex As Exception
                        _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "Error receiving SMS: " & ex.ToString)
                    End Try





                    '                 Try
                    'Dim MsgLocation As Integer
                    'Dim counter As Integer = 0
                    '' Dim messages As DecodedShortMessage() = comm.ReadMessages(PhoneMessageStatus.ReceivedUnread, PhoneStorageType.Sim)
                    '' Dim messages As DecodedShortMessage() = comm.ReadMessages(PhoneMessageStatus.All, PhoneStorageType.Phone)
                    'Dim messages As DecodedShortMessage() = comm.ReadMessages(PhoneMessageStatus.ReceivedUnread, PhoneStorageType.Phone)
                    'For Each Message In messages
                    ' If TypeOf Message.Data Is SmsDeliverPdu Then
                    ' Dim data As SmsDeliverPdu = CType(Message.Data, SmsDeliverPdu)
                    ' ReceptionSMS("RECEIVED", data.OriginatingAddress, data.UserDataText, data.SCTimestamp.ToString())
                    ' ElseIf TypeOf Message.Data Is SmsSubmitPdu Then
                    ' Dim data As SmsSubmitPdu = CType(Message.Data, SmsSubmitPdu)
                    ' ReceptionSMS("STORED", data.DestinationAddress, data.UserDataText, "")
                    ' '_Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Lecture_sms", " -> sms envoyé à " & data.DestinationAddress & " : " & data.UserDataText)
                    ' ElseIf TypeOf Message.Data Is SmsStatusReportPdu Then
                    ' Dim data As SmsStatusReportPdu = CType(Message.Data, SmsStatusReportPdu)
                    ' _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Lecture_sms", " -> sms status report de " & data.RecipientAddress & ", Status: " & data.Status.ToString() & ", Date: " & data.DischargeTime.ToString())
                    ' Else
                    ' _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Lecture_sms", " -> Format de SMS non reconnu: " & Message.Data.UserDataText)
                    ' End If
                    '
                    '                        ' Message.Data.UserDataText) '& (Message.Status).ToString & (Message.Storage).ToString & (Message.Index).ToString
                    '                        counter = counter + 1
                    '                        'Thread.Sleep(1000)
                    '                        MsgLocation = Message.Index
                    '                        Try
                    '                            comm.DeleteMessage(MsgLocation, PhoneStorageType.Phone)
                    ' If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "GSM Lecture_sms", " -> Message effacé.")
                    ' Catch ex As Exception
                    '_Server.Log() '(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Lecture_sms", " -> Error deleting from inbox")
                    'Exit Sub
                    'Finally
                    ' messages = Nothing
                    ' End Try
                    '                 Next
                    '                If counter = 0 Then
                    '_Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Lecture_sms", "Aucun message n'a été recu")
                    ''comm.Close()
                    'End If
                    'Exit Sub
                    '  If counter > 0 Then NewMsg = True
                    'Catch ex As GsmComm.GsmCommunication.CommException
                    ' _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DRIVER, "GSM Lecture_sms", "error:" & ex.InnerException.Message)
                    'End Try



                Case Else : _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Write", "No MODE configured")
            End Select

        Catch ex As Exception
            _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DRIVER, "GSM Lecture_sms", "error:" & ex.Message)
        End Try
    End Sub

    ''' <summary>Gestion des SMS/Notifications automatiquement reçus pour le mode PDU</summary>
    ''' <remarks></remarks>
    Private Sub MessageReceived(ByVal sender As Object, ByVal e As MessageReceivedEventArgs) Handles comm.MessageReceived
        Dim obj As IMessageIndicationObject = e.IndicationObject

        Try
            If _Enable = False Then Exit Sub
            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "GSM MessageReceived", "un message a été reçu")

            'If it's just a notification, print out the memory location of the new message
            If TypeOf obj Is MemoryLocation Then
                Dim loc As MemoryLocation = CType(obj, MemoryLocation)
                If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "GSM MessageReceived", String.Format("Nouveau message reçu : {0}, index {1}.", loc.Storage, loc.Index))
                'si le routage n'est pas actif, on lance une lecture manuelle du sms reçu.
                If _SMSROUTING = False Then Lecture_sms()
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
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DRIVER, "GSM MessageReceived", "Type de SMS inconnu.")
                Exit Sub
            End If

            _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DRIVER, "GSM MessageReceived", "Notification inconnue.")
        Catch ex As Exception
            _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DRIVER, "GSM MessageReceived", "Exception" & ex.Message)
        End Try
    End Sub


    ''' <summary>Gestion des SMS/Notifications automatiquement reçus pour le mode TEXTE</summary>
    ''' <remarks></remarks>
    Private Sub DataReceived(ByVal sender As Object, ByVal e As SerialDataReceivedEventArgs)
        Dim sIncomming As String = ""

        sIncomming = ATport.ReadLine
        'MsgBox("##" & sIncomming)
        _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM MessageReceived", sIncomming)

        Try

            ''           'une notification de sms arrive
            If Left(sIncomming, 7) = "+CMTI: " Then
                Dim storemsg As String
                Dim indexmsg As String

                sIncomming = sIncomming.Replace("+CMTI: ", "").Replace(vbCr, "").Replace(vbLf, "")
                _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM MessageReceived", sIncomming)
                'décodage de l'index du message arrivé
                Dim decodecmti() As String = sIncomming.Split(New Char() {","c})
                storemsg = decodecmti(0)
                indexmsg = decodecmti(1)


                ATport.WriteLine("AT+CMGR=" & indexmsg & vbCrLf)
                Thread.Sleep(100)

                'lecture du message
                If _DEBUG Then _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Datareceived", " : le message " & indexmsg & " et stocké dans " & storemsg & " à été recu, l'information n'est pas encore traitée par HMIDom")

                ATport.WriteLine("AT+CMGD=" & indexmsg & vbCrLf)
                Thread.Sleep(100)
                If _DEBUG Then _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Datareceived", " : le message " & indexmsg & " et stocké dans " & storemsg & " à été effacé")
                If _DEBUG Then _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Datareceived", " : le message " & indexmsg & " à été traité.")

                'flush()
                'ATport.DiscardInBuffer()
                'ATport.DiscardOutBuffer()


            End If
          
            'lecture du message
            If Left(sIncomming, 6) = "+CMGR:" Then
                '+CMGR: <status>,<from_address>,<mr>,<scts><CRLF><data> 
                '  at(+cmgr = 6)
                '+CMGR: "REC READ","<phoneNumber>",,"13/03/09,22:19:14+04"
                ' msgtexte
                '
                'OK

                '//On sort le reste des infos de la 1e ligne
                Dim i As Integer = 0

                Dim statut = ""
                Dim numero = ""
                Dim mr = ""
                Dim datetimereception = ""
                Dim ladate = ""

                Dim tmp_expl2 As String() = sIncomming.Split(New Char() {","c})
                statut = tmp_expl2(0).Replace("+CMGR: ", "").Replace(Chr(34), "")
                numero = tmp_expl2(1).Replace(Chr(34), "")
                mr = tmp_expl2(2).Replace(Chr(34), "")


                datetimereception = tmp_expl2(3).Replace(Chr(34), "") & " " & Left((tmp_expl2(4).Replace(Chr(34), "")), Len(tmp_expl2(4).Replace(Chr(34), "")) - 4)
                'On sort le reste des infos de le message
                'MsgBox(statut)
                'MsgBox(numero)
                'MsgBox(mr) ' en general vide 
                ' MsgBox(datetimereception)
                'arrive en format yy/MM/dd H:mm:ss+04
                'MsgBox("#" & datetimereception)
                Dim dateValue As DateTime
                DateTime.TryParseExact(datetimereception, "yy/MM/dd H:mm:ss", Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, dateValue)

                ' transformation au format d/MM/yyyy H:mm:ss
                datetimereception = dateValue.ToString("d/MM/yyyy h:mm:ss")
                'MsgBox("#" & datetimereception)
                ' _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Datareceived", "msg : " & datetimereception)
                Thread.Sleep(400)

                sIncomming = ATport.ReadExisting
                ' MsgBox(sIncomming)
                Dim msg As String = sIncomming.Replace(vbCr, "").Replace(vbLf, "").Replace("OK", "")
                _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Datareceived", "msg : " & msg)



                ReceptionSMS("RECEIVED", numero, msg, datetimereception)

                ' If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "GSM ReceptionSMS", "Reception d'un SMS de type " & Type & ", numéro " & numero & ", texte " & texte & ",  Date " & dateenvoi)
                ' If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "GSM ReceptionSMS", "Reception d'un SMS de type " & Type & ", numéro " & numero & ", texte " & texte & ",  Date " & dateenvoi)
                   End If
          

            If Left(sIncomming, 4) = "RING" Then
                ' ATA
                ATport.WriteLine("ATA" & vbCrLf)
                Thread.Sleep(100)
                '  ATH <Enter>
            End If

        Catch Ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GSM Datareceived", "Exception : " & Ex.Message)
        End Try
    End Sub



    ''' <summary>Reception de SMS</summary>
    Private Sub ReceptionSMS(ByVal type As String, ByVal numero As String, ByVal texte As String, ByVal dateenvoi As String)
        Try
            If _Enable = False Then Exit Sub
            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "GSM ReceptionSMS", "Reception d'un SMS de type " & type & ", numéro " & numero & ", texte " & texte & ",  Date " & dateenvoi)

            'Recherche si un device affecté
            Dim listedevices As New ArrayList
            listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_Idsrv, numero, "", Me._ID, True)
            'un device trouvé on maj la value
            If (listedevices.Count = 1) Then
                If type = "RECEIVED" Then
                    'listedevices.Item(0).Value = "RECEIVED: " & texte & "(" & dateenvoi & ")"
                    listedevices.Item(0).Value = "RECEIVED: " & texte
                ElseIf type = "STORED" Then
                    'listedevices.Item(0).Value = "STORED: " & texte & "(" & dateenvoi & ")"
                    listedevices.Item(0).Value = "STORED: " & texte
                End If

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


    ''' <summary>vérification codepin</summary>
    ''' <remarks></remarks>
    Function IsSimpinOk() As Boolean

        '+CPIN: (READY,SIM PIN,SIM PUK,SIM PIN2,SIM PUK2,PH-SIM PIN,PH-NET PIN,PH-NETSUB PIN,PH-SP PIN,PH-CORP PIN,PH-ESL PIN,PH-SIMLOCK PIN,BLOCKED)
        'AT+CPIN? doit on rentrer un code pin ?
        Dim sIncomming As String = ""
        Do Until (Left(sIncomming, 12) = "+CPIN: READY" Or Left(sIncomming, 14) = "+CPIN: SIM PIN" Or Left(sIncomming, 14) = "+CPIN: SIM PUK" Or Left(sIncomming, 15) = "+CPIN: SIM PIN2" Or Left(sIncomming, 15) = "+CPIN: SIM PUK2" Or Left(sIncomming, 17) = "+CPIN: PH-SIM PIN" Or Left(sIncomming, 14) = "+CPIN: BLOCKED")
            ATport.WriteLine("AT+CPIN?" & vbCrLf) ' vbcrlf
            If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "GSM Mode", "    -> " & sIncomming.Replace(vbCr, "").Replace(vbLf, ""))
            sIncomming = ATport.ReadLine()
        Loop
        _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "   * AT+CPIN")
        _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "    -> " & sIncomming.Replace(vbCr, "").Replace(vbLf, ""))

        ' on insère le code pin
        Select Case sIncomming.Replace(vbCr, "").Replace(vbLf, "")
            Case "+CPIN: SIM PUK"
                _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Pin", "    -> Veuillez consulter la notice de votre materiel et inserer le code PUK")
                Return False
            Case "+CPIN: SIM PIN2"
                _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "    -> Veuillez consulter la notice de votre materiel : SIM PIN2")
                Return False
            Case "+CPIN: SIM PUK2"
                _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "    -> Veuillez consulter la notice de votre materiel : SIM PUK2")
                Return False
            Case "+CPIN: PH-SIM PIN"
                _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "    -> Veuillez consulter la notice de votre materiel : PH-SIM PIN")
                Return False
            Case "+CPIN: BLOCKED"
                _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "    -> Veuillez consulter la notice de votre materiel : BLOCKED")
                Return False

            Case "+CPIN: SIM PIN"

                If _PinCode <> "" Then
                    sIncomming = ""
                    Do Until (Left(sIncomming, 2) = "OK" Or Left(sIncomming, 11) = "+CME ERROR:" Or Left(sIncomming, 5) = "ERROR")
                        ATport.WriteLine("AT+CPIN=" & _PinCode & vbCrLf) ' vbcrlf
                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "GSM Mode", "    -> AT+CPIN=" & _PinCode)
                        Thread.Sleep(50)
                        sIncomming = ATport.ReadLine()
                        If _DEBUG Then _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "GSM Mode", "    -> " & sIncomming.Replace(vbCr, "").Replace(vbLf, ""))
                    Loop
                    _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "    -> AT+CPIN=" & _PinCode & " - " & sIncomming.Replace(vbCr, "").Replace(vbLf, ""))
                    IsSimpinOk()
                Else
                    _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "    -> Votre modem indique qu'il est necessaire d'entrée le code PIN de votre carte SIM. Merci de remplir le parametre Code PIN du Driver")

                End If

            Case "+CPIN: READY"

                _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM Mode", "    -> CPIN READY")
                Return True
            Case Else


        End Select
        Return 0
    End Function



    ''' <summary>Recuperation carnet d'adresse</summary>
    Private Sub PhoneDir()
        Try
            Select Case _MODE
                Case "PDU"
                    _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "CARNET", _Carnet)
                    If _Carnet = True Then
                        _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "CARNET", "Récuperation du carnet d'adresses")

                        Dim storage As String = PhoneStorageType.Sim
                        Dim entries As PhonebookEntry() = comm.GetPhonebook(storage)

                        If entries.Length > 0 Then
                            ' Display the entries read

                            For Each entry As PhonebookEntry In entries
                                If entry.Text.IndexOf("0") <> -1 Then



                                    _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, entry.Number, HexToString(entry.Text))
                                    Dim strok As String = HexToString(entry.Text)

                                    Dim sb As New System.Text.StringBuilder

                                    For Each ch As Char In strok
                                        If Char.IsLetterOrDigit(ch) OrElse ch = " "c Then
                                            sb.Append(ch)
                                        End If
                                    Next




                                    Dim gen = New HoMIDom.HoMIDom.Server()
                                    Dim listedevices As New ArrayList
                                    listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_Idsrv, entry.Number, "GENERIQUESTRING", Me._ID, True)
                                    If IsNothing(listedevices) Then

                                        Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GSM Start", "ERR: Communication impossible avec le serveur, l'IDsrv est peut être erroné : " & _Idsrv)
                                        Exit Sub
                                    End If
                                    If (listedevices.Count = 0) Then
                                        gen.SaveDevice(_Idsrv, "", "GSM_" + sb.ToString(), entry.Number, True, False, _ID, "GENERIQUESTRING", 0, "", "", "", "Créé depuis le carnet d adresse du téléphone", 0, False, "0", "", 0, 9999, -9999, 0.0, Nothing, "", 0, True)
                                    End If


                                End If
                                If entry.Text.Length <> 0 Then
                                    Dim stra = ""
                                    Dim strb = ""
                                    If entry.Text.IndexOf("0") = -1 Then

                                        stra = entry.Number.Replace(",", "").Replace("""", "").Replace("1", "").Replace("2", "").Replace("3", "").Replace("4", "").Replace("5", "").Replace("6", "").Replace("7", "").Replace("8", "").Replace("9", "").Replace("0", "").Replace("+", "")
                                        strb = entry.Number.Remove(entry.Number.IndexOf(""""))



                                        Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, entry.Number, stra)
                                        Dim gen = New HoMIDom.HoMIDom.Server()
                                        Dim listedevices As New ArrayList
                                        listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_Idsrv, entry.Number, "GENERIQUESTRING", Me._ID, True)
                                        If IsNothing(listedevices) Then

                                            Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "GSM Start", "ERR: Communication impossible avec le serveur, l'IDsrv est peut être erroné : " & _Idsrv)
                                            Exit Sub
                                        End If
                                        If (listedevices.Count = 0) Then

                                            gen.SaveDevice(_Idsrv, "", "GSM_" + stra, strb, True, False, _ID, "GENERIQUESTRING", 0, "", "", "", "Créé depuis le carnet d adresse du téléphone", 0, False, "0", "", 0, 9999, -9999, 0.0, Nothing, "", 0, True)
                                        End If
                                    End If

                                End If
                            Next

                        End If
                    End If

                Case "TEXTE"
                    If _Carnet = True Then
                        _Server.Log(Server.TypeLog.INFO, Server.TypeSource.DRIVER, "GSM", "Carnet d adresse actuellement non supporté dans ce mode")
                    Else
                    End If

                Case Else
                    _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DRIVER, "GSM", "mode non supporté")

            End Select
        Catch ex As Exception
            _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.DRIVER, "GSM", "ERR: Phonedir  :" & ex.Message)


        End Try
    End Sub





    ''' <summary>hexa to string</summary>
    ''' <remarks></remarks>
    Function HexToString(ByVal hex As String) As String
        Dim text As New System.Text.StringBuilder(hex.Length \ 2)
        For i As Integer = 0 To hex.Length - 2 Step 2
            text.Append(Chr(Convert.ToByte(hex.Substring(i, 2), 16)))
        Next
        Return text.ToString
    End Function


#End Region


End Class