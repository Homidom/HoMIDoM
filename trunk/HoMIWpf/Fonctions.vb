Imports System.IO
Imports System.Windows.Media.Animation
Imports System.Net
Imports System.Xml
Imports System.Xml.XPath
Imports HoMIDom.HoMIDom
Imports System.Reflection
Imports System.Threading

Module Fonctions
    ''' <summary>
    ''' Affiche le message et connecté log dans le serveur
    ''' </summary>
    ''' <param name="Type"></param>
    ''' <param name="Message"></param>
    ''' <param name="Title"></param>
    ''' <param name="Fonction"></param>
    ''' <remarks></remarks>
    Public Sub AfficheMessageAndLog(ByVal Type As TypeLog, ByVal Message As String, Optional ByVal Title As String = "", Optional ByVal Fonction As String = "")
        Dim Icon As MessageBoxImage = MessageBoxImage.Error

        Select Case Type
            Case TypeLog.INFO
                Icon = MessageBoxImage.Information
            Case TypeLog.MESSAGE
                Icon = MessageBoxImage.Exclamation
            Case Else
                Icon = MessageBoxImage.Error
        End Select
        MessageBox.Show(Message, Title, MessageBoxButton.OK, Icon)

        Log(Type, TypeSource.CLIENT, Fonction, Message)
    End Sub

#Region "Log"

    Dim _File As String = _MonRepertoireAppData & "\logs\logClientWPF.xml" 'Représente le fichier log: ex"C:\users\xxx\homiwpf\logs\log.xml"
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

            'écriture dans un fichier texte
            _File = _MonRepertoire & "\logs\log_" & DateAndTime.Now.ToString("yyyyMMdd") & ".txt"
            Dim FreeF As Integer
            Dim texte As String = Now & vbTab & TypLog.ToString & vbTab & Source.ToString & vbTab & Fonction & vbTab & Message

            Try
                FreeF = FreeFile()
                texte = Replace(texte, vbLf, vbCrLf)
                SyncLock lock_logwrite
                    FileOpen(FreeF, _File, OpenMode.Append)
                    Print(FreeF, texte & vbCrLf)
                    FileClose(FreeF)
                End SyncLock
            Catch ex As IOException
                'wait(500)
                'Console.WriteLine(Now & " " & TypLog & " CLIENT WPF LOG ERROR IOException : " & ex.ToString)
            Catch ex As Exception
                'wait(500)
                'Console.WriteLine(Now & " " & TypLog & " CLIENT WPF LOG ERROR Exception : " & ex.ToString)
            End Try
            texte = Nothing
            FreeF = Nothing

            If IsConnect Then
                If myService IsNot Nothing Then
                    myService.Log(TypLog, HoMIDom.HoMIDom.Server.TypeSource.CLIENT, "CLIENT WPF " & Fonction, Message)
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'écriture d'un log: " & ex.Message, MsgBoxStyle.Exclamation, "Erreur Client WPF")
        End Try
    End Sub

    ''' <summary>Créer nouveau Fichier (donner chemin complet et nom) log</summary>
    ''' <param name="NewFichier"></param>
    ''' <remarks></remarks>
    Public Sub CreateNewFileLog(ByVal NewFichier As String)
        Try
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
        Catch ex As Exception
            MessageBox.Show("Erreur CreateNewFileLog: " & ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
#End Region


    ''' <summary>
    ''' Permet de vérifier si 2 objets sont identiques au niveau type et valeur(s)
    ''' </summary>
    ''' <param name="objet1"></param>
    ''' <param name="objet2"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsDiff(ByVal objet1 As Object, ByVal objet2 As Object) As Boolean
        Try
            If objet2 Is Nothing Or objet1 Is Nothing Then
                Return False
            ElseIf objet1.GetType <> objet2.GetType Then
                Return False
            End If

            For Each pi As PropertyInfo In objet1.GetType.GetProperties()
                Try
                    If pi.GetValue(objet1, Nothing) <> pi.GetValue(objet2, Nothing) Then
                        Return True
                    End If
                Catch ex As Exception
                    Return True
                End Try
            Next
            Return False

        Catch ex As Exception
            MessageBox.Show("Erreur IsDiff: " & ex.Message)
        End Try
    End Function

    ' ''' <summary>
    ' ''' Permet de recopier les valeurs propriétés d'un objet vers un autre
    ' ''' </summary>
    ' ''' <param name="objet1"></param>
    ' ''' <param name="objet2"></param>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Function Duplicate(ByVal source As Object, ByVal destination As Object) As Boolean
    '    Try
    '        If source Is Nothing Then
    '            Return False
    '        ElseIf source.GetType <> destination.GetType Then
    '            Return False
    '        End If

    '        For Each pi As PropertyInfo In source.GetType.GetProperties()
    '            Try
    '                pi.SetValue (source,pi.GetValue (source,
    '                If pi.GetValue(source, Nothing) <> pi.GetValue(objet2, Nothing) Then
    '                    Return True
    '                End If
    '            Catch ex As Exception
    '                Return True
    '            End Try
    '        Next
    '        Return False

    '    Catch ex As Exception
    '        MessageBox.Show("Erreur IsDiff: " & ex.Message)
    '    End Try
    'End Function


    Public Function ConvertArrayToImage(ByVal value As Byte()) As BitmapImage
        Try
            Dim ImgSource As BitmapImage = Nothing
            Dim array As Byte() = TryCast(value, Byte())

            If array IsNot Nothing Then
                ImgSource = New BitmapImage()
                ImgSource.BeginInit()
                ImgSource.CacheOption = BitmapCacheOption.OnLoad
                ImgSource.CreateOptions = BitmapCreateOptions.DelayCreation
                ImgSource.StreamSource = New MemoryStream(array)
                array = Nothing
                ImgSource.EndInit()
                If ImgSource.CanFreeze Then ImgSource.Freeze()
            End If

            Return ImgSource
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "ERREUR Sub ConvertArrayToImage: " & ex.Message, "Erreur", "ConvertArrayToImage")
            Return Nothing
        End Try
    End Function

    Public Sub ScrollToPosition(ByVal ScrollViewer As UAniScrollViewer.AniScrollViewer, ByVal x As Double, ByVal y As Double, ByVal Duree As Double)
        Dim vertAnim As New DoubleAnimation()
        vertAnim.From = ScrollViewer.VerticalOffset
        vertAnim.[To] = y
        vertAnim.DecelerationRatio = 0.2
        vertAnim.Duration = New Duration(TimeSpan.FromMilliseconds(250))

        Dim horzAnim As New DoubleAnimation()
        horzAnim.From = ScrollViewer.HorizontalOffset
        horzAnim.[To] = x
        horzAnim.DecelerationRatio = 0.99
        horzAnim.Duration = New Duration(TimeSpan.FromMilliseconds(Duree))

        Dim sb As New Storyboard()
        'sb.Children.Add(vertAnim)
        sb.Children.Add(horzAnim)

        Storyboard.SetTarget(vertAnim, ScrollViewer)
        'Storyboard.SetTargetProperty(vertAnim, New PropertyPath(UAniScrollViewer.AniScrollViewer.CurrentVerticalOffsetProperty))
        Storyboard.SetTarget(horzAnim, ScrollViewer)
        Storyboard.SetTargetProperty(horzAnim, New PropertyPath(UAniScrollViewer.AniScrollViewer.CurrentHorizontalOffsetProperty))

        sb.Begin()

    End Sub

    Public Function UrlIsValid(ByVal url As String) As Boolean
        Dim is_valid As Boolean = False
        If url.ToLower().StartsWith("www.") Then url = _
            "http://" & url

        Dim web_response As HttpWebResponse = Nothing
        Try
            Dim web_request As HttpWebRequest = _
                HttpWebRequest.Create(url)
            web_response = _
                DirectCast(web_request.GetResponse(),  _
                HttpWebResponse)
            Return True
            web_request = Nothing
            web_response = Nothing
        Catch ex As Exception
            Return False
        Finally
            If Not (web_response Is Nothing) Then _
                web_response.Close()
        End Try
    End Function

    ''' <summary>
    ''' Vérifie si la valeur est un boolean
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsBoolean(ByVal value As Object) As Boolean
        Try
            Dim x As Boolean = value
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Fonction permettant de charger une image 
    ''' </summary>
    ''' <param name="FileChm">chemin du fichier</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LoadBitmapImage(ByVal FileChm As String) As BitmapImage
        Dim bmpImage As New BitmapImage

        Try
            If File.Exists(FileChm) Then
                bmpImage.BeginInit()
                bmpImage.CacheOption = BitmapCacheOption.OnLoad
                bmpImage.CreateOptions = BitmapCreateOptions.DelayCreation
                bmpImage.UriSource = New Uri(FileChm, UriKind.Absolute)
                bmpImage.EndInit()
                If bmpImage.CanFreeze Then bmpImage.Freeze()
            End If
            Return bmpImage
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "ERREUR Sub LoadBitmapImage (FileChm= " & FileChm & "): " & ex.Message, "Erreur", "LoadBitmapImage")
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Retourne la valeur d'une variable entourée par des balises "<" ">"
    ''' </summary>
    ''' <param name="ValueTxt"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function TraiteBalise(ByVal ValueTxt As String) As String
        Try
            If String.IsNullOrEmpty(ValueTxt) Then Return ""
            Dim x As String = ValueTxt.ToUpper.Trim(" ")

            If x.StartsWith("<") And x.EndsWith(">") Then
                Dim _val As String = Mid(x, 2, Len(x) - 2)
                Select Case _val
                    Case "SYSTEM_DATE"
                        Return Now.Date.ToShortDateString
                    Case "SYSTEM_LONG_DATE"
                        Return Now.Date.ToLongDateString
                    Case "SYSTEM_TIME"
                        Return Now.ToShortTimeString
                    Case "SYSTEM_LONG_TIME"
                        Return Now.ToLongTimeString
                    Case "SYSTEM_SOLEIL_COUCHE"
                        If IsConnect Then
                            Dim _date As Date = myService.GetHeureCoucherSoleil
                            Return _date.ToShortTimeString
                        Else
                            Return ""
                        End If
                    Case "SYSTEM_SOLEIL_LEVE"
                        If IsConnect Then
                            Dim _date As Date = myService.GetHeureLeverSoleil
                            Return _date.ToShortTimeString
                        Else
                            Return ""
                        End If
                    Case "SYSTEM_CONDITION"
                        If AllDevices IsNot Nothing And String.IsNullOrEmpty(frmMere.Ville) = False Then
                            For Each ObjMeteo As HoMIDom.HoMIDom.TemplateDevice In AllDevices
                                If ObjMeteo.Type = HoMIDom.HoMIDom.Device.ListeDevices.METEO And ObjMeteo.Enable = True And ObjMeteo.Name.ToUpper = frmMere.Ville.ToUpper Then
                                    Return ObjMeteo.ConditionActuel
                                End If
                            Next
                        Else
                            Return ""
                        End If
                    Case "SYSTEM_TEMP_ACTUELLE"
                        If AllDevices IsNot Nothing And String.IsNullOrEmpty(frmMere.Ville) = False Then
                            For Each ObjMeteo As HoMIDom.HoMIDom.TemplateDevice In AllDevices
                                If ObjMeteo.Type = HoMIDom.HoMIDom.Device.ListeDevices.METEO And ObjMeteo.Enable = True And ObjMeteo.Name.ToUpper = frmMere.Ville.ToUpper Then
                                    Return ObjMeteo.TemperatureActuel & " °C"
                                End If
                            Next
                        Else
                            Return "# °C"
                        End If
                    Case "SYSTEM_ICO_METEO"
                        If AllDevices IsNot Nothing And String.IsNullOrEmpty(frmMere.Ville) = False Then
                            For Each ObjMeteo As HoMIDom.HoMIDom.TemplateDevice In AllDevices
                                If ObjMeteo.Type = HoMIDom.HoMIDom.Device.ListeDevices.METEO And ObjMeteo.Enable = True And ObjMeteo.Name.ToUpper = frmMere.Ville.ToUpper Then
                                    Return ObjMeteo.IconActuel
                                End If
                            Next
                        Else
                            Return ""
                        End If
                    Case Else
                        Return " "
                End Select
            Else
                Return ValueTxt
            End If
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur Fonctions.TraiteBalise: " & ex.ToString, "Erreur", " Fonctions.TraiteBalise")
            Return "Erreur"
        End Try
    End Function

    Public Sub Refresh()
        Try
            If IsConnect Then
                Do While lock_dev
                    Thread.Sleep(100)
                Loop

                lock_dev = True
                AllDevices = myService.GetAllDevices(IdSrv)
                lock_dev = False
            End If
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur Fonctions.Thread_MAJ.Refresh: " & ex.ToString, "Erreur", " Fonctions.Thread_MAJ.Refresh")
        End Try
    End Sub

    Public Function ReturnDeviceById(DeviceId As String) As TemplateDevice
        Try
            If AllDevices IsNot Nothing Then
                Dim retour As TemplateDevice = Nothing

                Do While lock_dev
                    Thread.Sleep(100)
                Loop

                lock_dev = True
                For Each _dev In AllDevices
                    If _dev.ID = DeviceId Then
                        retour = _dev
                        Exit For
                    End If
                Next

                lock_dev = False
                Return retour
            End If
        Catch ex As Exception
            AfficheMessageAndLog(Fonctions.TypeLog.ERREUR, "Erreur Fonctions.ReturnDeviceById: " & ex.ToString, "Erreur", " Fonctions.ReturnDeviceById")
            Return Nothing
        End Try
    End Function

End Module
