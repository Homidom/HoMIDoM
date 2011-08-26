Imports System.IO
Imports System.IO.Compression
Imports System.Xml
Imports System.Xml.XPath
Imports System.Data.SQLite
Imports HoMIDom.HoMIDom.Server

Module GuideTV
    Public MyChaine As New List(Of sChaine)
    Public MyProgramme As New List(Of sProgramme)
    Dim MyXML As clsXML
    Dim timestart As String
    Dim MyWindow As Window1

#Region "Strucure"
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
#End Region

#Region "Conversion"
    Public Function ConvertTextToHTML(ByVal Text As String) As String
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
    End Function

    Public Function ConvertHtmlToText(ByVal Text As String) As String
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
    End Function
#End Region

#Region "Gestion des chaines"
    'Permet de lister les chaines dans la base de données
    Public Sub ChaineFromXMLToDB()
        Try
            MyXML = New clsXML("C:\ehome\data\complet.xml")
            Dim liste As XmlNodeList = MyXML.SelectNodes("/tv/channel")
            Dim i As Integer
            Dim a As String
            Dim b As String
            Dim SQLconnect As New SQLiteConnection()
            Dim SQLcommand As SQLiteCommand
            SQLconnect.ConnectionString = "Data Source=C:\ehome\data\guidetv.db;"
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
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    'Charge les chaines depuis la base de données en mémoire
    Public Sub ChargeChaineFromDB()
        Try
            MyChaine.Clear()
            Dim SQLconnect As New SQLiteConnection()
            Dim SQLcommand As SQLiteCommand
            SQLconnect.ConnectionString = "Data Source=C:\ehome\data\guidetv.db;"
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
                MsgBox("aucune chaine à charger depuis la DB!")
            End If
            SQLcommand.Dispose()
            SQLconnect.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    'Active/Désactive une chaine
    Public Sub EnableChaine(ByVal ChaineID As String, ByVal Value As Boolean)
        Dim SQLconnect As New SQLiteConnection()
        Dim SQLcommand As SQLiteCommand
        SQLconnect.ConnectionString = "Data Source=C:\ehome\data\guidetv.db;"
        SQLconnect.Open()
        SQLcommand = SQLconnect.CreateCommand
        Dim SQLreader As SQLiteDataReader

        SQLcommand.CommandText = "SELECT * FROM chaineTV where ID='" & ChaineID & "'"
        SQLreader = SQLcommand.ExecuteReader()
        If SQLreader.HasRows = True Then
            SQLreader.Close()
            SQLcommand = SQLconnect.CreateCommand
            If Value = True Then SQLcommand.CommandText = "UPDATE chaineTV set enable='1' where ID='" & ChaineID & "'"
            If Value = False Then SQLcommand.CommandText = "UPDATE chaineTV set enable='0'where ID='" & ChaineID & "'"
            SQLcommand.ExecuteNonQuery()
        Else
            MsgBox("Chaine " & ChaineID & " non trouvée!")
        End If
        SQLcommand.Dispose()
        SQLconnect.Close()
    End Sub
#End Region

#Region "Gestion des programmes"
    'Permet de lister les programmes pour les chaines sélectionnées dans la base de données
    Public Sub ProgrammeFromXMLToDB()
        MyXML = New clsXML("C:\ehome\data\complet.xml")
        timestart = Now.ToShortTimeString

        Dim liste As XmlNodeList
        Dim i As Integer
        Dim i2 As Integer
        Dim b As String = ""
        Dim d As String = ""
        Dim SQLconnect As New SQLiteConnection()
        Dim SQLcommand As SQLiteCommand

        SQLconnect.ConnectionString = "Data Source=C:\ehome\data\guidetv.db;"
        SQLconnect.Open()
        SQLcommand = SQLconnect.CreateCommand
        SQLcommand.CommandText = "DELETE FROM programmeTV where idchannel<>''"
        SQLcommand.ExecuteNonQuery()
        SQLcommand = SQLconnect.CreateCommand

        'liste toute les chaines
        For i = 0 To MyChaine.Count - 1
            If MyChaine.Item(i).Enable = 1 Then 'cherche que les prog dont la chaine est sélectionné
                liste = MyXML.SelectNodes("/tv/programme[@channel='" & MyChaine.Item(i).ID & "']")
                For i2 = 0 To liste.Count - 1
                    Dim vProg As sProgramme = New sProgramme
                    b = ""
                    vProg.DateStart = Mid(liste(i2).Attributes.Item(0).Value, 7, 2) & "/" & Mid(liste(i2).Attributes.Item(0).Value, 5, 2) & "/" & Mid(liste(i2).Attributes.Item(0).Value, 1, 4)
                    vProg.DateEnd = Mid(liste(i2).Attributes.Item(1).Value, 7, 2) & "/" & Mid(liste(i2).Attributes.Item(1).Value, 5, 2) & "/" & Mid(liste(i2).Attributes.Item(1).Value, 1, 4)
                    vProg.TimeStart = Mid(liste(i2).Attributes.Item(0).Value, 9, 2) & ":" & Mid(liste(i2).Attributes.Item(0).Value, 11, 2)
                    vProg.TimeEnd = Mid(liste(i2).Attributes.Item(1).Value, 9, 2) & ":" & Mid(liste(i2).Attributes.Item(1).Value, 11, 2)
                    vProg.IDChannel = liste(i2).Attributes.Item(3).Value
                    For j = 0 To liste(i2).ChildNodes.Count - 1
                        If liste(i2).ChildNodes.Item(j).ChildNodes.Count > 1 Then
                            Dim c As Integer
                            For c = 0 To liste(i2).ChildNodes.Item(j).ChildNodes.Count - 1
                                If liste(i2).ChildNodes.Item(j).Name = "credits" Then
                                    Select Case liste(i2).ChildNodes.Item(j).ChildNodes.Item(c).Name
                                        Case "director" : d = "directeur"
                                        Case "actor" : d = "acteur"
                                    End Select
                                    b += d & ":" & ConvertTextToHTML(liste(i2).ChildNodes.Item(j).ChildNodes.Item(c).ChildNodes.Item(0).Value & vbCrLf)
                                End If
                            Next
                            If b <> "" Then vProg.Credits = b
                        Else
                            Select Case liste(i2).ChildNodes.Item(j).Name
                                Case "title" : vProg.Titre = ConvertTextToHTML(liste(i2).ChildNodes.Item(j).ChildNodes.Item(0).Value)
                                Case "sub-title" : vProg.SousTitre = ConvertTextToHTML(liste(i2).ChildNodes.Item(j).ChildNodes.Item(0).Value)
                                Case "desc" : vProg.Description = ConvertTextToHTML(liste(i2).ChildNodes.Item(j).ChildNodes.Item(0).Value)
                                Case "length"
                                    Dim lgth As Integer
                                    If liste(i2).ChildNodes.Item(j).Attributes.Item(0).Value = "hours" Then
                                        lgth = 60
                                    Else
                                        lgth = 1
                                    End If
                                    vProg.Duree = (liste(i2).ChildNodes.Item(j).ChildNodes.Item(0).Value * lgth)
                                Case "category"
                                    If vProg.Categorie1 = "" Then
                                        vProg.Categorie1 = ConvertTextToHTML(liste(i2).ChildNodes.Item(j).ChildNodes.Item(0).Value)
                                    Else
                                        vProg.Categorie2 = ConvertTextToHTML(liste(i2).ChildNodes.Item(j).ChildNodes.Item(0).Value)
                                    End If
                                Case "date" : vProg.Annee = liste(i2).ChildNodes.Item(j).ChildNodes.Item(0).Value

                            End Select
                        End If

                    Next
                    SQLcommand = SQLconnect.CreateCommand
                    SQLcommand.CommandText = "INSERT INTO programmeTV (timestart, timeend,datestart,dateend,idchannel,titre,soustitre,description,duree,categorie1,categorie2,annee,credits) VALUES ('" & vProg.TimeStart & "', '" & vProg.TimeEnd & "','" & vProg.DateStart & "','" & vProg.DateEnd & "','" & vProg.IDChannel & "','" & vProg.Titre & "','" & vProg.SousTitre & "','" & vProg.Description & "','" & vProg.Duree & "','" & vProg.Categorie1 & "','" & vProg.Categorie2 & "','" & vProg.Annee & "','" & vProg.Credits & "')"
                    SQLcommand.ExecuteNonQuery()
                Next
            End If
        Next
        SQLcommand.Dispose()
        SQLconnect.Close()
        MyXML = Nothing
        MyWindow.Log(TypeLog.INFO, TypeSource.CLIENT, "GuideTV", "Fin du chargement des programmes - start:" & timestart & " fin:" & Now.ToShortTimeString)
    End Sub

    'Charge les programmes depuis la base de données en mémoire
    Public Sub ChargeProgrammesFromDB()
        MyProgramme.Clear()
        Dim SQLconnect As New SQLiteConnection()
        Dim SQLcommand As SQLiteCommand
        SQLconnect.ConnectionString = "Data Source=C:\ehome\data\guidetv.db;"
        SQLconnect.Open()
        SQLcommand = SQLconnect.CreateCommand
        Dim SQLreader As SQLiteDataReader

        SQLcommand.CommandText = "SELECT * FROM programmeTV"
        SQLreader = SQLcommand.ExecuteReader()
        If SQLreader.HasRows = True Then
            While SQLreader.Read()
                Dim vProgramme As sProgramme = New sProgramme
                vProgramme.TimeStart = SQLreader(1)
                vProgramme.TimeEnd = SQLreader(2)
                vProgramme.DateStart = SQLreader(3)
                vProgramme.DateEnd = SQLreader(4)
                vProgramme.IDChannel = SQLreader(5)
                vProgramme.Titre = SQLreader(6)
                vProgramme.SousTitre = SQLreader(7)
                vProgramme.Description = SQLreader(8)
                vProgramme.Duree = SQLreader(9)
                vProgramme.Categorie1 = SQLreader(10)
                vProgramme.Categorie2 = SQLreader(11)
                vProgramme.Annee = SQLreader(12)
                vProgramme.Credits = SQLreader(13)
                MyProgramme.Add(vProgramme)
            End While
        Else
            MsgBox("aucun programme à charger depuis la DB!")
        End If
        SQLcommand.Dispose()
        SQLconnect.Close()
    End Sub

#End Region

End Module
