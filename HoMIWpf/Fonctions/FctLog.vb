Imports System.Xml
Imports System.Xml.XPath
Imports System.IO
Imports HoMIDom.HoMIDom

Module FctLog
#Region "Log"

    Dim _File As String = _MonRepertoireAppData & "\logs\logClientWPF.xml" 'Représente le fichier log: ex"C:\users\xxx\homiwpf\logs\log.xml"
    Dim _MaxFileSize As Long = 5120 'en Koctets

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

End Module
