﻿#Region "Imports"
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Threading
Imports System.Xml
Imports System.Xml.XPath
Imports System.IO
Imports System.ServiceModel
Imports System.ServiceModel.Description
Imports System.Xml.Serialization
Imports HoMIDom.HoMIDom
Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.Web.HttpUtility
Imports System.Windows.Controls.Primitives
Imports WpfApplication1.Designer
Imports WpfApplication1.Designer.ResizeRotateAdorner
#End Region


Class Window1

#Region "Data"
    ' Used when manually scrolling.
    Private scrollTarget As Point
    Private scrollStartPoint As Point
    Private scrollStartOffset As Point
    Private imgStackPnl As New StackPanel()

    'Paramètres de connexion à HomeSeer
    Dim _Serveur As String = ""
    Dim _Login As String = ""
    Dim _Password As String = ""
    Dim _PortSOAP As String = "8000"
    Dim _IP As String = "localhost"

    'XML
    Dim myxml As clsXML
    Dim list As XmlNodeList

    'User Graphic
    Dim _ShowSoleil As Boolean
    Dim _ShowTemperature As Boolean
    Dim _ImageBackGroundDefault As String
    Dim _ListMnu As New List(Of uCtrlImgMnu)
    Dim _Design As Boolean = False
    Dim mybuttonstyle As Style
    Dim _CurrentIdZone As String
    'Service TV
    Dim _ServiceTV As New ServiceTV(Me)

#End Region

#Region "Property"

    Public Property ShowSoleil As Boolean
        Get
            Return _ShowSoleil
        End Get
        Set(ByVal value As Boolean)
            _ShowSoleil = value
            Affiche("Soleil", value)
        End Set
    End Property

    Public Property ShowTemperature As Boolean
        Get
            Return _ShowTemperature
        End Get
        Set(ByVal value As Boolean)
            _ShowTemperature = value
            Affiche("Temperature", value)
        End Set
    End Property

    Public Property ImageBackGround As String
        Get
            Return _ImageBackGroundDefault
        End Get
        Set(ByVal value As String)
            _ImageBackGroundDefault = value
            If _ImageBackGroundDefault <> "" Then
                If File.Exists(_ImageBackGroundDefault) Then
                    Dim bmpImage As New BitmapImage()
                    bmpImage.BeginInit()
                    bmpImage.UriSource = New Uri(_ImageBackGroundDefault, UriKind.Absolute)
                    bmpImage.EndInit()
                    ImgBackground.Source = bmpImage
                End If
            End If
        End Set
    End Property

    Public Property Friction As Double
        Get
            Return m_friction
        End Get
        Set(ByVal value As Double)
            m_friction = value
        End Set
    End Property

    Public Property SpeedTouch As Double
        Get
            Return m_SpeedTouch
        End Get
        Set(ByVal value As Double)
            If value < 100 Then value = 100
            If value > 1000 Then value = 100
            m_SpeedTouch = value
        End Set
    End Property

    Public Property PortSOAP As String
        Get
            Return _PortSOAP
        End Get
        Set(ByVal value As String)
            _PortSOAP = value
        End Set
    End Property

    Public Property IP As String
        Get
            Return _IP
        End Get
        Set(ByVal value As String)
            _IP = value
        End Set
    End Property

    Public Property HSAdresse As String
        Get
            Return _Serveur
        End Get
        Set(ByVal value As String)
            _Serveur = value
        End Set
    End Property

    Public Property HSLogin As String
        Get
            Return _Login
        End Get
        Set(ByVal value As String)
            _Login = value
        End Set
    End Property

    Public Property HSPassword As String
        Get
            Return _Password
        End Get
        Set(ByVal value As String)
            _Password = value
        End Set
    End Property

    Public Property ListMnu As List(Of uCtrlImgMnu)
        Get
            Return _ListMnu
        End Get
        Set(ByVal value As List(Of uCtrlImgMnu))
            _ListMnu = value
        End Set
    End Property
#End Region

    Public Sub [Affiche](ByVal Tag As String, ByVal Visible As Boolean)
        For i As Integer = 0 To StkTop.Children.Count - 1
            Dim x As Object = StkTop.Children.Item(i)
            If UCase(x.Tag) = UCase(Tag) Then
                If Visible = True Then
                    x.Visibility = Visibility.Visible
                Else
                    x.Visibility = Visibility.Hidden
                End If
            End If
        Next
    End Sub

    Public Sub UnloadControl(ByVal uid As String)
        For i As Integer = 0 To Canvas1.Children.Count - 1
            If Canvas1.Children.Item(i).Uid = uid Then
                Canvas1.Children.RemoveAt(i)
                Exit For
            End If
        Next

    End Sub

    Public Sub New()
        Try
            ' Cet appel est requis par le Concepteur Windows Form.
            InitializeComponent()
            ' _MonRepertoire = System.Environment.CurrentDirectory 'représente le répertoire de l'application 

            Dim mystyles As New ResourceDictionary()
            mystyles.Source = New Uri("/HoMIWpF;component/Resources/DesignerItem.xaml",
                    UriKind.RelativeOrAbsolute)
            mybuttonstyle = mystyles("DesignerItemStyle")

            'Chargement des paramètres
            Log(TypeLog.INFO, TypeSource.CLIENT, "LOADCONFIG", "Message: " & LoadConfig(_MonRepertoire & "\Config\"))

            ' Create StackPanel and set child elements to horizontal orientation
            imgStackPnl.HorizontalAlignment = HorizontalAlignment.Left
            imgStackPnl.VerticalAlignment = VerticalAlignment.Center
            imgStackPnl.Orientation = Orientation.Horizontal

            ConnectToHomidom()

            'Creation  du menu
            If ListMnu.Count = 0 Then
                NewBtnMnu("Paramètres", uCtrlImgMnu.TypeOfMnu.Config, , True, , )
            End If

            If IsConnect = True Then
                Dim cntNewZone As Integer = 0
                For i As Integer = 0 To myService.GetAllZones(IdSrv).Count - 1
                    Dim x As Zone = myService.GetAllZones(IdSrv).Item(i)
                    If ListMnu.Count = 0 Then
                        NewBtnMnu(x.Name, uCtrlImgMnu.TypeOfMnu.Zone, , False, , , x.ID)
                        cntNewZone += 1
                    Else
                        Dim flagexist As Boolean = False
                        For j As Integer = 0 To ListMnu.Count - 1
                            If ListMnu.Item(j).IDElement = x.ID Then
                                flagexist = True
                            End If
                        Next
                        If flagexist = False Then
                            NewBtnMnu(x.Name, uCtrlImgMnu.TypeOfMnu.Zone, , False, , , x.ID, True)
                            cntNewZone += 1
                        End If
                    End If
                Next
                If cntNewZone > 0 Then
                    MessageBox.Show(cntNewZone & " nouvelle(s) zone(s) ajoutée(s)")
                End If
            End If
            'NewBtnMnu("Journal", uCtrlImgMnu.TypeOfMnu.Internet, , , , "/parametres-icone-3667-128.png")
            'NewBtnMnu("Programme TV", "1", "C:\ehome\images\125_tv.png")
            'NewBtnMnu("Contacts", "2", "C:\ehome\images\contact2.png")
            'NewBtnMnu("Module", "3", "C:\ehome\images\125_light.png")
            'NewBtnMnu("Meteo", "13", "C:\ehome\images\125_goodmorning.png")
            'NewBtnMnu("Paramètres", "5", "C:\ehome\images\125_settings.png")
            'NewBtnMnu("Recette", "6", "C:\ehome\images\Recette.png")
            'NewBtnMnu("Pages Jaunes", "7", "C:\ehome\images\pages-jaunes.png")
            'NewBtnMnu("Internet", "8", "C:\ehome\images\Internet.png")
            'NewBtnMnu("Itinéraire", "9", "C:\ehome\images\map.png")
            'NewBtnMnu("Note", "15", "C:\ehome\images\calendar3.png")
            'NewBtnMnu("Traffic", "16", "C:\ehome\images\traffic.png")

            'Mise en forme du scrollviewer
            ScrollViewer1.Content = imgStackPnl

            'Timer pour afficher la date & heure et levé/couché soleil
            Dim dt As DispatcherTimer = New DispatcherTimer()
            AddHandler dt.Tick, AddressOf dispatcherTimer_Tick
            dt.Interval = New TimeSpan(0, 0, 1)
            dt.Start()

            ''Connexion à HomeSeer
            'ConnectToHS()


            'If IsHSConnect = True Then
            '    LblLeve.Content = Mid(hs.Sunrise, 1, 5)
            '    LblCouche.Content = Mid(hs.Sunset, 1, 5)
            'End If
            If IsConnect = True And ShowSoleil = True Then
                Dim mydate As Date
                mydate = myService.GetHeureLeverSoleil
                LblLeve.Content = mydate.ToShortTimeString
                mydate = myService.GetHeureCoucherSoleil
                LblCouche.Content = mydate.ToShortTimeString
            End If

            myxml = Nothing
            frmMere = Me
        Catch ex As Exception
            MessageBox.Show("Erreur: " & ex.ToString)
        End Try
    End Sub


#Region "Configuration"
    ''' <summary>Chargement de la config depuis le fichier XML</summary>
    ''' <param name="Fichier"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LoadConfig(ByVal Fichier As String) As String
        'Copy du fichier de config avant chargement
        Try
            Dim _file As String = Fichier & "HoMIWpF"
            If File.Exists(_file & ".bak") = True Then File.Delete(_file & ".bak")
            File.Copy(_file & ".xml", Mid(_file & ".xml", 1, Len(_file & ".xml") - 4) & ".bak")
            Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Création du backup (.bak) du fichier de config avant chargement")
        Catch ex As Exception
            Log(TypeLog.ERREUR, TypeSource.SERVEUR, "LoadConfig", "Erreur impossible de créer une copie de backup du fichier de config: " & ex.Message)
        End Try

        Try
            Dim dirInfo As New System.IO.DirectoryInfo(Fichier)
            Dim file As System.IO.FileInfo
            Dim files() As System.IO.FileInfo = dirInfo.GetFiles("HoMIWpF.xml")
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
                                Case "portsoap"
                                    _PortSOAP = list.Item(0).Attributes.Item(j).Value
                                Case "ip"
                                    _IP = list.Item(0).Attributes.Item(j).Value
                                Case Else
                                    Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Un attribut correspondant au serveur est inconnu: nom:" & list.Item(0).Attributes.Item(j).Name & " Valeur: " & list.Item(0).Attributes.Item(j).Value)
                            End Select
                        Next
                    Else
                        MsgBox("Il manque les paramètres du client WPF dans le fichier de config !!", MsgBoxStyle.Exclamation, "Erreur serveur")
                    End If
                    Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Paramètres du client WPF chargés")

                    'ON peut se connecter
                    'Connexion à Homidom
                    ConnectToHomidom()

                    '******************************************
                    'on va chercher les paramètres HomeSeer
                    '******************************************
                    list = myxml.SelectNodes("/homidom/HS")
                    If list.Count > 0 Then 'présence des paramètres du server
                        For j As Integer = 0 To list.Item(0).Attributes.Count - 1
                            Select Case list.Item(0).Attributes.Item(j).Name
                                Case "adresse"
                                    _Serveur = list.Item(0).Attributes.Item(j).Value
                                Case "login"
                                    _Login = list.Item(0).Attributes.Item(j).Value
                                Case "password"
                                    _Password = list.Item(0).Attributes.Item(j).Value
                                Case Else
                                    Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Un attribut correspondant au serveur est inconnu: nom:" & list.Item(0).Attributes.Item(j).Name & " Valeur: " & list.Item(0).Attributes.Item(j).Value)
                            End Select
                        Next
                    Else
                        MsgBox("Il manque les paramètres du client WPF dans le fichier de config !!", MsgBoxStyle.Exclamation, "Erreur Client WPF")
                    End If
                    Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Paramètres du Serveur HomeSeer chargés")

                    '******************************************
                    'on va chercher les paramètres tactile
                    '******************************************
                    list = myxml.SelectNodes("/homidom/tactile")
                    If list.Count > 0 Then 'présence des paramètres du server
                        For j As Integer = 0 To list.Item(0).Attributes.Count - 1
                            Select Case list.Item(0).Attributes.Item(j).Name
                                Case "friction"
                                    m_friction = CDbl(list.Item(0).Attributes.Item(j).Value.Replace(".", ","))
                                Case "speedtouch"
                                    m_SpeedTouch = CDbl(list.Item(0).Attributes.Item(j).Value.Replace(".", ","))
                                Case Else
                                    Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Un attribut correspondant au serveur est inconnu: nom:" & list.Item(0).Attributes.Item(j).Name & " Valeur: " & list.Item(0).Attributes.Item(j).Value)
                            End Select
                        Next
                    Else
                        MsgBox("Il manque les paramètres du client WPF dans le fichier de config !!", MsgBoxStyle.Exclamation, "Erreur Client WPF")
                    End If
                    Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Paramètres tactiles chargés")

                    '******************************************
                    'on va chercher les paramètres interface
                    '******************************************
                    list = myxml.SelectNodes("/homidom/interface")
                    If list.Count > 0 Then 'présence des paramètres du server
                        For j As Integer = 0 To list.Item(0).Attributes.Count - 1
                            Select Case list.Item(0).Attributes.Item(j).Name
                                Case "showsoleil"
                                    ShowSoleil = list.Item(0).Attributes.Item(j).Value
                                Case "showtemperature"
                                    ShowTemperature = list.Item(0).Attributes.Item(j).Value
                                Case "imgbackground"
                                    ImageBackGround = list.Item(0).Attributes.Item(j).Value
                                Case Else
                                    Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Un attribut correspondant au serveur est inconnu: nom:" & list.Item(0).Attributes.Item(j).Name & " Valeur: " & list.Item(0).Attributes.Item(j).Value)
                            End Select
                        Next
                    Else
                        MsgBox("Il manque les paramètres du client WPF dans le fichier de config !!", MsgBoxStyle.Exclamation, "Erreur Client WPF")
                    End If
                    Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Paramètres de l'interface chargés")

                    '******************************************
                    'on va chercher les menus
                    '******************************************
                    list = myxml.SelectNodes("/homidom/menus/menu")
                    If list.Count > 0 Then 'présence des paramètres du server
                        For j As Integer = 0 To list.Count - 1
                            Dim _MnuNom As String = ""
                            Dim _MnuType As uCtrlImgMnu.TypeOfMnu
                            Dim _MnuIcon As String = ""
                            Dim _MnuParam As New List(Of String)
                            Dim _Mnudefaut As Boolean
                            Dim _MnuIDElement As String = ""
                            Dim _MnuVisible As Boolean = False

                            For k As Integer = 0 To list.Item(j).Attributes.Count - 1
                                Select Case list.Item(j).Attributes.Item(k).Name
                                    Case "nom"
                                        _MnuNom = list.Item(j).Attributes.Item(k).Value
                                    Case "defaut"
                                        _Mnudefaut = list.Item(j).Attributes.Item(k).Value
                                    Case "type"
                                        Select Case list.Item(j).Attributes.Item(k).Value
                                            Case uCtrlImgMnu.TypeOfMnu.Internet.ToString
                                                _MnuType = uCtrlImgMnu.TypeOfMnu.Internet
                                            Case uCtrlImgMnu.TypeOfMnu.Meteo.ToString
                                                _MnuType = uCtrlImgMnu.TypeOfMnu.Meteo
                                            Case uCtrlImgMnu.TypeOfMnu.Zone.ToString
                                                _MnuType = uCtrlImgMnu.TypeOfMnu.Zone
                                            Case uCtrlImgMnu.TypeOfMnu.Config.ToString
                                                _MnuType = uCtrlImgMnu.TypeOfMnu.Config
                                        End Select
                                    Case "icon"
                                        _MnuIcon = list.Item(j).Attributes.Item(k).Value
                                    Case "idelement"
                                        _MnuIDElement = list.Item(j).Attributes.Item(k).Value
                                    Case "visible"
                                        _MnuVisible = list.Item(j).Attributes.Item(k).Value
                                    Case Else
                                        Dim a As String = list.Item(j).Attributes.Item(k).Name
                                        If a.Contains("parametre") Then
                                            _MnuParam.Add(list.Item(j).Attributes.Item(k).Value)
                                        End If
                                End Select
                            Next
                            NewBtnMnu(_MnuNom, _MnuType, _MnuParam, _Mnudefaut, , _MnuIcon, _MnuIDElement, _MnuVisible)
                        Next
                    Else
                        'MsgBox("Il manque les paramètres du client WPF dans le fichier de config !!", MsgBoxStyle.Exclamation, "Erreur Client WPF")
                    End If
                    Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Menus chargés")

                    '******************************************
                    'on va chercher les éléments
                    '******************************************
                    list = myxml.SelectNodes("/homidom/elements/element")
                    If list.Count > 0 Then 'présence des éléments
                        For j As Integer = 0 To list.Count - 1
                            Dim x As New cElement

                            For k As Integer = 0 To list.Item(j).Attributes.Count - 1
                                Select Case list.Item(j).Attributes.Item(k).Name
                                    Case "id"
                                        x.ID = list.Item(j).Attributes.Item(k).Value
                                    Case "zoneid"
                                        x.ZoneId = list.Item(j).Attributes.Item(k).Value
                                    Case "x"
                                        x.X = list.Item(j).Attributes.Item(k).Value
                                    Case "y"
                                        x.Y = list.Item(j).Attributes.Item(k).Value
                                    Case "width"
                                        x.Width = list.Item(j).Attributes.Item(k).Value
                                    Case "height"
                                        x.Height = list.Item(j).Attributes.Item(k).Value
                                    Case "angle"
                                        x.Angle = list.Item(j).Attributes.Item(k).Value
                                End Select
                            Next
                            _ListElement.Add(x)
                        Next
                    Else
                        'MsgBox("Il manque les paramètres du client WPF dans le fichier de config !!", MsgBoxStyle.Exclamation, "Erreur Client WPF")
                    End If
                    Log(TypeLog.INFO, TypeSource.SERVEUR, "LoadConfig", "Eléments chargés")
                    '**************
                Next
            End If

            'Vide les variables
            dirInfo = Nothing
            file = Nothing
            files = Nothing
            myxml = Nothing

            Return " Chargement de la configuration terminée"

        Catch ex As Exception
            MsgBox("ERREUR LOADCONFIG " & ex.ToString, MsgBoxStyle.Exclamation, "Erreur Client WPF")
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

            Log(TypeLog.INFO, TypeSource.SERVEUR, "SaveConfig", "Sauvegarde des paramètres du client WPF")
            ''------------ server
            writer.WriteStartElement("server")
            writer.WriteStartAttribute("portsoap")
            writer.WriteValue(_PortSOAP)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("ip")
            writer.WriteValue(_IP)
            writer.WriteEndAttribute()
            writer.WriteEndElement()

            ''------------ HomeSeer
            writer.WriteStartElement("HS")
            writer.WriteStartAttribute("adresse")
            writer.WriteValue(_Serveur)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("login")
            writer.WriteValue(_Login)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("password")
            writer.WriteValue(_Login)
            writer.WriteEndAttribute()
            writer.WriteEndElement()

            ''-------------------
            ''------------tactile
            ''------------------
            writer.WriteStartElement("tactile")
            writer.WriteStartAttribute("speedtouch")
            writer.WriteValue(SpeedTouch)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("friction")
            writer.WriteValue(Friction)
            writer.WriteEndAttribute()
            writer.WriteEndElement()

            ''------------
            ''Sauvegarde des parametres d'interface
            ''------------
            writer.WriteStartElement("interface")
            writer.WriteStartAttribute("showsoleil")
            writer.WriteValue(ShowSoleil)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("showtemperature")
            writer.WriteValue(ShowTemperature)
            writer.WriteEndAttribute()
            writer.WriteStartAttribute("imgbackground")
            writer.WriteValue(ImageBackGround)
            writer.WriteEndAttribute()
            writer.WriteEndElement()

            ''------------
            ''Sauvegarde des menus
            ''------------
            writer.WriteStartElement("menus")
            For i As Integer = 0 To ListMnu.Count - 1
                writer.WriteStartElement("menu")
                writer.WriteStartAttribute("nom")
                writer.WriteValue(ListMnu.Item(i).Text)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("defaut")
                writer.WriteValue(ListMnu.Item(i).Defaut)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("type")
                writer.WriteValue(ListMnu.Item(i).Type.ToString)
                writer.WriteEndAttribute()
                If ListMnu.Item(i).Type <> uCtrlImgMnu.TypeOfMnu.Zone Then
                    writer.WriteStartAttribute("icon")
                    writer.WriteValue(ListMnu.Item(i).Icon)
                    writer.WriteEndAttribute()
                End If
                If ListMnu.Item(i).Type = uCtrlImgMnu.TypeOfMnu.Zone Then
                    writer.WriteStartAttribute("idelement")
                    writer.WriteValue(ListMnu.Item(i).IDElement)
                    writer.WriteEndAttribute()
                    writer.WriteStartAttribute("visible")
                    writer.WriteValue(ListMnu.Item(i).Visible)
                    writer.WriteEndAttribute()
                End If
                If ListMnu.Item(i).Parametres IsNot Nothing Then
                    For j As Integer = 0 To ListMnu.Item(i).Parametres.Count - 1
                        writer.WriteStartAttribute("parametre" & j)
                        writer.WriteValue(ListMnu.Item(i).Parametres(j))
                        writer.WriteEndAttribute()
                    Next
                End If
                writer.WriteEndElement()
            Next
            writer.WriteEndElement()

            ''------------
            ''Sauvegarde des elements
            ''------------
            writer.WriteStartElement("elements")
            For i As Integer = 0 To _ListElement.Count - 1
                writer.WriteStartElement("element")
                writer.WriteStartAttribute("id")
                writer.WriteValue(_ListElement.Item(i).ID)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("zoneid")
                writer.WriteValue(_ListElement.Item(i).ZoneId)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("x")
                writer.WriteValue(Replace(_ListElement.Item(i).X, ".", ","))
                writer.WriteEndAttribute()
                writer.WriteStartAttribute("y")
                writer.WriteValue(Replace(_ListElement.Item(i).Y, ".", ","))
                writer.WriteEndAttribute()
                writer.WriteStartAttribute(Replace("width", ".", ","))
                writer.WriteValue(_ListElement.Item(i).Width)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute(Replace("height", ".", ","))
                writer.WriteValue(_ListElement.Item(i).Height)
                writer.WriteEndAttribute()
                writer.WriteStartAttribute(Replace("angle", ".", ","))
                writer.WriteValue(_ListElement.Item(i).Angle)
                writer.WriteEndAttribute()
                writer.WriteEndElement()
            Next
            writer.WriteEndElement()

            ''FIN DES ELEMENTS------------

            writer.WriteEndDocument()
            writer.Close()
            Log(TypeLog.INFO, TypeSource.SERVEUR, "SaveConfig", "Sauvegarde terminée")
        Catch ex As Exception
            MsgBox("ERREUR SAVECONFIG " & ex.ToString, MsgBoxStyle.Exclamation, "Erreur Client WPF")
            Log(TypeLog.ERREUR, TypeSource.SERVEUR, "SaveConfig", " Erreur de sauvegarde de la configuration: " & ex.Message)
        End Try

    End Sub

#End Region

#Region "Log"
    Dim _File As String = _MonRepertoire & "\logs\logClientWPF.xml" 'Représente le fichier log: ex"C:\homidom\log\log.xml"
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
    Public Sub Log(ByVal TypLog As TypeLog, ByVal Source As TypeSource, ByVal Fonction As String, ByVal Message As String)
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

#Region "Connexion"
    'Connexion au serveur HomeSeer
    Private Sub ConnectToHS()
        'If My.Computer.Network.IsAvailable = True Then
        '    Try
        '        myxml = New clsXML("C:\ehome\config\wehome_config.xml")
        '        list = myxml.SelectNodes("/wehome/connect/element")
        '        For i As Integer = 0 To list.Count - 1
        '            _Serveur = list(i).Attributes.Item(0).Value
        '            _Login = list(i).Attributes.Item(1).Value
        '            _Password = list(i).Attributes.Item(2).Value
        '        Next
        '    Catch ex As Exception
        '        Log(TypeLog.INFO, TypeSource.CLIENT, "ConnectToHS", "Erreur lors du chargement des paramètres de connexion: " & ex.Message)
        '    End Try

        '    Dim hsapp As HomeSeer2.application = New HomeSeer2.application
        '    hsapp.SetHost(_Serveur) '("seb-serveur-002:81")
        '    Dim rval As String = hsapp.Connect(_Login, _Password) '("sebastien", "clarisse1705")
        '    If rval <> "" Then
        '        Log(TypeLog.INFO, TypeSource.CLIENT, "ConnectToHS", "State: Unable to connect to HomeSeer, is it running? You need to be on the same subnet as HomeSeer")
        '        rval = hsapp.Connect(_Login, _Password) '("sebastien", "clarisse1705")
        '        If rval <> "" Then
        '            IsHSConnect = False
        '            rval = hsapp.Connect(_Login, _Password) '("sebastien", "clarisse1705")
        '            If rval <> "" Then
        '                IsHSConnect = False
        '            Else
        '                IsHSConnect = True
        '                hs = hsapp.GetHSRef
        '            End If
        '        Else
        '            IsHSConnect = True
        '            hs = hsapp.GetHSRef
        '        End If
        '    Else
        '        Log(TypeLog.INFO, TypeSource.CLIENT, "ConnectToHS", "State: Connect to HomeSeer")
        '        IsHSConnect = True
        '        hs = hsapp.GetHSRef
        '    End If
        'Else
        '    IsHSConnect = False
        'End If
    End Sub

    'Connexion au serveur Homdidom
    Private Sub ConnectToHomidom()
        Dim myChannelFactory As ServiceModel.ChannelFactory(Of HoMIDom.HoMIDom.IHoMIDom) = Nothing

        Try
            Dim myadress As String = "http://" & IP & ":" & PortSOAP & "/ServiceModelSamples/service"
            Dim binding As New ServiceModel.BasicHttpBinding
            binding.MaxBufferPoolSize = 5000000
            binding.MaxReceivedMessageSize = 5000000
            binding.MaxBufferSize = 5000000
            binding.ReaderQuotas.MaxArrayLength = 5000000
            binding.ReaderQuotas.MaxStringContentLength = 5000000
            binding.SendTimeout = TimeSpan.FromMinutes(60)
            binding.CloseTimeout = TimeSpan.FromMinutes(60)
            binding.OpenTimeout = TimeSpan.FromMinutes(60)
            binding.ReceiveTimeout = TimeSpan.FromMinutes(60)

            myChannelFactory = New ServiceModel.ChannelFactory(Of HoMIDom.HoMIDom.IHoMIDom)(binding, New System.ServiceModel.EndpointAddress(myadress))
            myService = myChannelFactory.CreateChannel()
            IsConnect = True
        Catch ex As Exception
            myChannelFactory.Abort()
            IsConnect = False
        End Try
    End Sub
#End Region

    'Creation  du menu
    Private Sub NewBtnMnu(ByVal Label As String, ByVal type As uCtrlImgMnu.TypeOfMnu, Optional ByVal Parametres As List(Of String) = Nothing, Optional ByVal Defaut As Boolean = False, Optional ByVal Tag As String = "", Optional ByVal Icon As String = "", Optional ByVal IdElement As String = "", Optional ByVal Visible As Boolean = False)
        Try
            Dim ctrl As New uCtrlImgMnu
            ctrl.Type = type
            ctrl.Defaut = Defaut
            ctrl.Id = HoMIDom.HoMIDom.Api.GenerateGUID
            ctrl.Text = Label
            ctrl.Tag = Tag
            ctrl.Icon = Icon
            ctrl.Parametres = Parametres
            ctrl.IDElement = IdElement
            ctrl.Visible = Visible
            AddHandler ctrl.click, AddressOf IconMnuDoubleClick
            _ListMnu.Add(ctrl)
            If type = uCtrlImgMnu.TypeOfMnu.Zone Then
                If Visible = True Then imgStackPnl.Children.Add(ctrl)
            Else
                imgStackPnl.Children.Add(ctrl)
            End If

        Catch ex As Exception
            Log(TypeLog.INFO, TypeSource.CLIENT, "NewBtnMnu", "Erreur NewBtnMnu: " & ex.Message)
            MessageBox.Show("Erreur lors de la création du bouton menu: " & ex.ToString)
        End Try
    End Sub

    'Affiche la date et heure, heures levé et couché du soleil
    Public Sub dispatcherTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        Try
            'ConnectToHomeSeer()
            LblTime.Content = Now.ToLongDateString & " " & Now.ToShortTimeString
            'If IsHSConnect = True Then
            '    LblLeve.Content = Mid(hs.Sunrise, 1, 5)
            '    LblCouche.Content = Mid(hs.Sunset, 1, 5)
            '    LblTemp.Content = "INT: " & hs.DeviceString("T3") & "°C / EXT: " & hs.DeviceString("T2") & "°C"
            'Else
            If IsConnect = True And ShowSoleil = True Then
                Dim mydate As Date
                mydate = myService.GetHeureLeverSoleil
                LblLeve.Content = mydate.ToShortTimeString
                mydate = myService.GetHeureCoucherSoleil
                LblCouche.Content = mydate.ToShortTimeString
            Else
                LblTemp.Content = "?"
            End If
            ' End If
        Catch ex As Exception
            Log(TypeLog.INFO, TypeSource.CLIENT, "DispatcherTimer", "DispatcherTimer: " & ex.Message)
        End Try
    End Sub

    'Clic sur un menu de la barre du bas
    Private Sub IconMnuDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Try
            Me.Cursor = Cursors.Wait
            If Canvas1.Children.Count > 0 Then
                Canvas1.Children.Clear()
            End If

            ImageBackGround = _ImageBackGroundDefault

            Dim y As uCtrlImgMnu = sender

            If y IsNot Nothing Then
                Select Case y.Type
                    Case uCtrlImgMnu.TypeOfMnu.Internet
                        Dim x As New uInternet(y.Parametres(0))
                        Canvas1.Children.Add(x)
                        x.Width = Canvas1.ActualWidth
                        x.Height = Canvas1.ActualHeight
                    Case uCtrlImgMnu.TypeOfMnu.Config
                        Dim x As New WConfig(Me)
                        x.Owner = Me
                        x.ShowDialog()
                        If x.DialogResult.HasValue And x.DialogResult.Value Then
                            imgStackPnl.Children.Clear()
                            For i As Integer = 0 To ListMnu.Count - 1
                                AddHandler ListMnu.Item(i).click, AddressOf IconMnuDoubleClick
                                If ListMnu.Item(i).Type = uCtrlImgMnu.TypeOfMnu.Zone Then
                                    If ListMnu.Item(i).Visible = True Then imgStackPnl.Children.Add(ListMnu.Item(i))
                                Else
                                    imgStackPnl.Children.Add(ListMnu.Item(i))
                                End If
                            Next
                            x.Close()
                        Else
                            x.Close()
                        End If
                    Case uCtrlImgMnu.TypeOfMnu.Meteo
                        Dim x As New uMeteos
                        Canvas1.Children.Add(x)
                        x.Width = Canvas1.ActualWidth
                        x.Height = Canvas1.ActualHeight
                    Case uCtrlImgMnu.TypeOfMnu.Zone
                        'Dim x As New uZone(y.IDElement, Canvas1.ActualHeight, Canvas1.ActualWidth)
                        ShowZone(y.IDElement)
                        'Case 0 'Actualités
                        '    Dim x As New uInternet("http://fr.news.yahoo.com/")
                        '    Canvas1.Children.Add(x)
                        '    x.Width = Canvas1.ActualWidth
                        '    x.Height = Canvas1.ActualHeight
                        'Case 1 'Prgr TV
                        '    'Dim x As New uInternet("http://www.programme-tv.net/programme/toutes-les-chaines/")
                        '    Dim x As New uProgrammeTV
                        '    Canvas1.Children.Add(x)
                        '    x.Width = Canvas1.ActualWidth
                        '    x.Height = Canvas1.ActualHeight
                        'Case 2 'Contacts
                        '    Dim x As New uContact
                        '    Canvas1.Children.Add(x)
                        '    x.Width = Canvas1.ActualWidth
                        '    x.Height = Canvas1.ActualHeight
                        'Case 3 'Module
                        '    Dim x As New uModules
                        '    Canvas1.Children.Add(x)
                        '    x.Width = Canvas1.ActualWidth
                        '    x.Height = Canvas1.ActualHeight
                        'Case 4 'Scene
                        '    'Dim x As New uScenes
                        '    'Canvas1.Children.Add(x)
                        '    'x.Width = Canvas1.ActualWidth
                        '    'x.Height = Canvas1.ActualHeight
                        'Case 6 'Recette
                        '    Dim x As New uInternet("http://www.marmiton.org/")
                        '    Canvas1.Children.Add(x)
                        '    x.Width = Canvas1.ActualWidth
                        '    x.Height = Canvas1.ActualHeight
                        'Case 7 'Pages jaunes
                        '    Dim x As New uInternet("http://www.pagesjaunes.fr/")
                        '    Canvas1.Children.Add(x)
                        '    x.Width = Canvas1.ActualWidth
                        '    x.Height = Canvas1.ActualHeight
                        'Case 8 'Internet
                        '    Dim x As New uInternet("http://www.google.fr/")
                        '    Canvas1.Children.Add(x)
                        '    x.Width = Canvas1.ActualWidth
                        '    x.Height = Canvas1.ActualHeight
                        'Case 9 'Itinéraire
                        '    Dim x As New uInternet("http://maps.google.fr/maps?hl=fr&tab=wl")
                        '    Canvas1.Children.Add(x)
                        '    x.Width = Canvas1.ActualWidth
                        '    x.Height = Canvas1.ActualHeight
                        'Case 10 'Calculatrice
                        'Case 11 'Facebook
                        '    Dim x As New uInternet("http://fr-fr.facebook.com/")
                        '    Canvas1.Children.Add(x)
                        '    x.Width = Canvas1.ActualWidth
                        '    x.Height = Canvas1.ActualHeight
                        'Case 12 'Horloge
                        '    'Dim x As New uHorloge
                        '    'Canvas1.Children.Add(x)
                        '    'x.Width = Canvas1.ActualWidth
                        '    'x.Height = Canvas1.ActualHeight
                        'Case 13 'Meteo
                        '    Dim x As New uMeteos
                        '    Canvas1.Children.Add(x)
                        '    x.Width = Canvas1.ActualWidth
                        '    x.Height = Canvas1.ActualHeight
                        'Case 14 'Calendrier
                        '    'Dim x As New uCalendar
                        '    'Canvas1.Children.Add(x)
                        '    'x.Width = Canvas1.ActualWidth
                        '    'x.Height = Canvas1.ActualHeight
                        'Case 15 'Notes
                        '    Dim x As New uNotes
                        '    Canvas1.Children.Add(x)
                        '    x.Width = Canvas1.ActualWidth
                        '    x.Height = Canvas1.ActualHeight
                        'Case 16
                        '    Dim x As New uInternet("http://www.bison-fute.equipement.gouv.fr/diri/Accueil.do")
                        '    Canvas1.Children.Add(x)
                        '    x.Width = Canvas1.ActualWidth
                        '    x.Height = Canvas1.ActualHeight
                End Select
            End If
            Me.Cursor = Cursors.Arrow
        Catch ex As Exception
            MessageBox.Show("Erreur: " & ex.ToString, "Erreur")
            Log(TypeLog.INFO, TypeSource.CLIENT, "IconMnuDoubleClick", "Erreur: " & ex.Message)
        End Try
    End Sub

    'Element demande afficher zone
    Private Sub ElementShowZone(ByVal Zoneid As String)
        ShowZone(Zoneid)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Private Sub Window1_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        Try
            Log(TypeLog.INFO, TypeSource.CLIENT, "Client", "Fermeture de l'application")
        Catch ex As Exception
            Log(TypeLog.INFO, TypeSource.CLIENT, "Client", "Erreur Lors de la fermeture: " & ex.Message)
        End Try
    End Sub

    Private Sub ScrollViewer1_PreviewMouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ScrollViewer1.PreviewMouseDown
        scrollStartPoint = e.GetPosition(Me)
        scrollStartOffset.X = ScrollViewer1.HorizontalOffset
    End Sub

    Private Sub ScrollViewer1_PreviewMouseMove(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles ScrollViewer1.PreviewMouseMove
        If e.LeftButton = MouseButtonState.Pressed Then
            Dim currentPoint As Point = e.GetPosition(Me)
            Dim delta As New Point(scrollStartPoint.X - currentPoint.X, scrollStartPoint.Y - currentPoint.Y)
            scrollTarget.X = scrollStartOffset.X + delta.X
            ScrollToPosition(ScrollViewer1, scrollTarget.X, currentPoint.Y, m_SpeedTouch)
        End If
    End Sub

    'Bouton Quitter
    Private Sub BtnQuit_Click_1(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnQuit.Click
        If Chk1.IsChecked = True Then Chk1.IsChecked = False

        SaveConfig(_MonRepertoire & "\config\HoMIWpF.xml")
        Log(TypeLog.INFO, TypeSource.CLIENT, "Client", "Fermture de l'application")
        End
    End Sub

    'Private Sub Canvas1_SizeChanged(ByVal sender As Object, ByVal e As System.Windows.SizeChangedEventArgs) Handles Canvas1.SizeChanged
    '    For i As Integer = 0 To Canvas1.Children.Count - 1
    '        Dim x As Object = Canvas1.Children.Item(i)
    '        x.Width = Canvas1.ActualWidth
    '        x.Height = e.NewSize.Height
    '    Next
    'End Sub

    Private Sub Window1_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded

    End Sub

    Private Sub ShowZone(ByVal IdZone As String)
        Try
            Dim _zone As Zone
            Dim _flagnew As Boolean
            Dim _flagTrouv As Boolean = False
            Dim _Left As Double = 0
            Dim _Top As Double = 20
            Dim _idx As Integer = 0

            If IsConnect = True Then
                _CurrentIdZone = IdZone
                If Canvas1.Children.Count > 0 Then Canvas1.Children.Clear()

                _zone = myService.ReturnZoneByID(IdSrv, IdZone)
                ImgBackground.Source = ConvertArrayToImage(myService.GetByteFromImage(_zone.Image))

                If _ListElement.Count = 0 Then 'C'est la première fois que les élements sont affichés
                    _flagnew = True
                End If


                For i As Integer = 0 To _zone.ListElement.Count - 1
                    Dim z As Zone.Element_Zone = myService.ReturnZoneByID(IdSrv, IdZone).ListElement.Item(i)
                    If z.Visible = True Then
                        For j As Integer = 0 To _ListElement.Count - 1
                            If _ListElement.Item(j).ID = z.ElementID And _ListElement.Item(j).ZoneId = IdZone Then


                                'Ajouter un nouveau Control
                                Dim x As New ContentControl
                                Dim Trg As New TransformGroup
                                Dim Rot As New RotateTransform(_ListElement.Item(j).Angle)

                                Trg.Children.Add(Rot)
                                x.Width = _ListElement.Item(j).Width
                                x.Height = _ListElement.Item(j).Height
                                x.RenderTransform = Trg
                                x.Style = mybuttonstyle
                                x.Tag = True
                                x.Uid = z.ElementID

                                Dim y As New uWidgetEmpty
                                y.Id = z.ElementID
                                y.IsHitTestVisible = True 'True:bouge pas False:Bouge
                                AddHandler y.PreviewMouseDown, AddressOf Dbleclk
                                AddHandler y.ShowZone, AddressOf ElementShowZone
                                x.Content = y
                                Canvas1.Children.Add(x)
                                Canvas.SetLeft(x, _ListElement.Item(j).X)
                                Canvas.SetTop(x, _ListElement.Item(j).Y)

                                _flagTrouv = True
                            End If
                        Next

                        If _flagnew = True Or _flagTrouv = False Then

                            'Ajouter un nouveau Control
                            Dim x As New ContentControl
                            x.Width = 140
                            x.Height = 60
                            x.Style = mybuttonstyle
                            x.Tag = True
                            x.Uid = z.ElementID

                            If _idx = 5 Then
                                _idx = 0
                                _Top += 70
                            End If
                            _Left = (150 * _idx) + 40

                            'Ajoute l'élément dans la liste
                            Dim elmt As New cElement
                            elmt.ID = z.ElementID
                            elmt.ZoneId = IdZone
                            elmt.Width = x.Width
                            elmt.Height = x.Height
                            elmt.Angle = 0
                            elmt.X = _Left
                            elmt.Y = _Top
                            _ListElement.Add(elmt)

                            Dim y As New uWidgetEmpty
                            y.Id = z.ElementID
                            y.IsHitTestVisible = True 'True:bouge pas False:Bouge
                            AddHandler y.PreviewMouseDown, AddressOf Dbleclk
                            x.Content = y
                            Canvas1.Children.Add(x)
                            Canvas.SetLeft(x, _Left)
                            Canvas.SetTop(x, _Top)

                            _idx += 1
                        End If

                    End If
                    _flagTrouv = False
                Next
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub Dbleclk(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs)
        'MessageBox.Show("OK")
    End Sub


    Private Sub RdB1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Chk1.Click
        Try
            If Chk1.IsChecked = True Then
                Design = True
                For Each child As ContentControl In Canvas1.Children
                    Selector.SetIsSelected(child, True)

                    Dim lbl As uWidgetEmpty = child.Content
                    lbl.IsHitTestVisible = False
                Next
            Else
                Design = False
                Dim a As String = ""
                For Each child As ContentControl In Canvas1.Children
                    Selector.SetIsSelected(child, False)

                    'a &= child.Name & " : Left=" & Canvas1.GetLeft(child) & " Top=" & Canvas1.GetTop(child) & " Width=" & child.Width & " Height=" & child.Height & " Angle=" & child.RenderTransform.GetValue(RotateTransform.AngleProperty) & vbCrLf
                    For j As Integer = 0 To _ListElement.Count - 1
                        If _ListElement.Item(j).ID = child.Uid And _ListElement.Item(j).ZoneId = _CurrentIdZone Then
                            _ListElement.Item(j).X = CType(Canvas.GetLeft(child), Double)
                            _ListElement.Item(j).Y = CType(Canvas.GetTop(child), Double)
                            _ListElement.Item(j).Width = child.Width
                            _ListElement.Item(j).Height = child.Height

                            If InStr(child.RenderTransform.GetType.ToString, "TransformGroup") > 0 Then
                                Dim gt As TransformGroup = child.RenderTransform '.GetValue(RotateTransform.AngleProperty)
                                For k = 0 To gt.Children.Count - 1
                                    If InStr(LCase(gt.Children.Item(k).GetType.ToString), "rotatetransform") > 0 Then
                                        Dim rt As RotateTransform = gt.Children.Item(k)
                                        If rt IsNot Nothing Then
                                            _ListElement.Item(j).Angle = rt.Angle 'child.RenderTransform.GetValue(RotateTransform.AngleProperty)
                                        End If
                                        Exit For
                                    End If
                                Next
                            End If
                            If InStr(child.RenderTransform.GetType.ToString, "RotateTransform") > 0 Then
                                _ListElement.Item(j).Angle = child.RenderTransform.GetValue(RotateTransform.AngleProperty)
                            End If
                        End If
                    Next

                    Dim lbl As uWidgetEmpty = child.Content
                    lbl.IsHitTestVisible = True

                Next
                'MsgBox(a)
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur: " & ex.ToString)
        End Try
    End Sub

End Class