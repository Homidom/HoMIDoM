Imports System.Threading
Imports System.IO
Imports System.Xml

Namespace HoMIDom

    Module Log
        'Dim _File As String 'Représente le fichier log: ex"C:\log.xml"
        'Dim _MaxFileSize As Long = 5120000 'en bytes

        'Public Property FichierLog() As String
        '    Get
        '        Return _File
        '    End Get
        '    Set(ByVal value As String)
        '        _File = value
        '    End Set
        'End Property

        'Public Property MaxFileSize() As Long
        '    Get
        '        Return _MaxFileSize
        '    End Get
        '    Set(ByVal value As Long)
        '        _MaxFileSize = value
        '    End Set
        'End Property

        ''Indique le type du Log: si c'est une erreur, une info, un message...
        'Public Enum TypeLog
        '    ERREUR
        '    INFO
        '    MESSAGE
        '    DEBUG
        'End Enum

        ''Indique la source du log si c'est le serveur, un scriopt, un device...
        'Public Enum TypeSource
        '    SERVEUR
        '    SCRIPT
        '    TRIGGER
        '    DEVICE
        '    DRIVER
        'End Enum

        'Public Sub Log(ByVal TypLog As TypeLog, ByVal Source As TypeSource, ByVal Message As String)
        '    Try
        '        Dim Fichier As FileInfo

        '        'Vérifie si le fichier log existe sinon le crée
        '        If File.Exists(_File) = False Then
        '            CreateNewFileLog(_File)
        '        End If

        '        Fichier = New FileInfo(_File)

        '        'Vérifie si le fichier est trop gros si oui le supprime
        '        If Fichier.Length > _MaxFileSize Then
        '            File.Delete(_File)
        '        End If

        '        Dim xmldoc As New XmlDocument()

        '        'Ecrire le log
        '        Try
        '            xmldoc.Load(_File) 'ouvre le fichier xml
        '            Dim elelog As XmlElement = xmldoc.CreateElement("log") 'création de l'élément log
        '            Dim atttime As XmlAttribute = xmldoc.CreateAttribute("time") 'création de l'attribut time
        '            Dim atttype As XmlAttribute = xmldoc.CreateAttribute("type") 'création de l'attribut type
        '            Dim attsrc As XmlAttribute = xmldoc.CreateAttribute("source") 'création de l'attribut source
        '            Dim attmsg As XmlAttribute = xmldoc.CreateAttribute("message") 'création de l'attribut message

        '            'on affecte les attributs à l'élément
        '            elelog.SetAttributeNode(atttime)
        '            elelog.SetAttributeNode(atttype)
        '            elelog.SetAttributeNode(attsrc)
        '            elelog.SetAttributeNode(attmsg)

        '            'on affecte les valeur
        '            elelog.SetAttribute("time", Now)
        '            elelog.SetAttribute("type", TypLog)
        '            elelog.SetAttribute("source", Source)
        '            elelog.SetAttribute("message", Message)

        '            Dim root As XmlElement = xmldoc.Item("logs")
        '            root.AppendChild(elelog)

        '            'on enregistre le fichier xml
        '            xmldoc.Save(_File)

        '        Catch ex As Exception

        '        End Try

        '        Fichier = Nothing
        '    Catch ex As Exception

        '    End Try
        'End Sub

        ''Créer nouveau Fichier (donner chemin complet et nom) log
        'Public Sub CreateNewFileLog(ByVal NewFichier As String)
        '    Dim rw As XmlTextWriter = New XmlTextWriter(NewFichier, Nothing)
        '    rw.WriteStartDocument()
        '    rw.WriteStartElement("logs")
        '    rw.WriteStartElement("log")
        '    rw.WriteAttributeString("time", Now)
        '    rw.WriteAttributeString("type", 0)
        '    rw.WriteAttributeString("source", 0)
        '    rw.WriteAttributeString("message", "Création du nouveau fichier log")
        '    rw.WriteEndElement()
        '    rw.WriteEndElement()
        '    rw.WriteEndDocument()
        '    rw.Close()
        'End Sub
    End Module

End Namespace
