Imports System.Data.SQLite

Partial Public Class uContact
#Region "VARIABLE"
    'VARIABLES
    Dim SQLconnect As New SQLiteConnection()
    Dim MaxRecord As Integer
    Dim Index As Integer = 1
    Dim TxtID As String
    Dim stk As New StackPanel
    Dim FlagNew As Boolean
    Private scrollTarget As Point
    Private scrollStartPoint As Point
    Private scrollStartOffset As Point
#End Region

    Public Sub New()

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        Dim bmpImage As New BitmapImage()
        bmpImage.BeginInit()
        bmpImage.UriSource = New Uri("C:\ehome\images\GRAYDOCKED-base.png", UriKind.Absolute)
        bmpImage.EndInit()
        Image1.Source = bmpImage
        Image2.Source = bmpImage
        Image3.Source = bmpImage

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        SQLconnect.ConnectionString = "Data Source=C:\ehome\data\contact.db;"
        SQLconnect.Open()

        Dim SQLcommand As SQLiteCommand
        Dim SQLreader As SQLiteDataReader

        SQLcommand = SQLconnect.CreateCommand
        SQLcommand.CommandText = "SELECT * FROM contact order by nom"
        SQLreader = SQLcommand.ExecuteReader()

        Dim a As String = ""

        If SQLreader.HasRows = True Then
            While SQLreader.Read()
                Dim x As New Label
                Dim doc As New TextBlock 'FlowDocument
                Dim para As New Paragraph

                If Mid(SQLreader(1), 1, 1) <> a Then
                    a = Mid(SQLreader(1), 1, 1)
                    Dim y As New Label
                    y.Width = ScrollViewer1.Width - 10
                    y.Height = 25
                    y.Background = Brushes.Blue
                    y.Foreground = Brushes.White
                    y.Content = a
                    y.Tag = "Lbl"
                    stk.Children.Add(y)
                End If

                x.Width = ScrollViewer1.Width - 10
                x.Height = 25
                x.Tag = SQLreader(0)
                x.Background = Brushes.White
                x.Foreground = Brushes.Black

                doc.Inlines.Add(New Bold(New Run(SQLreader(1) & " ")))
                doc.Inlines.Add(SQLreader(2))
                x.Content = doc
                AddHandler x.MouseDown, AddressOf Contact_Click
                stk.Children.Add(x)

                MaxRecord += 1
            End While
        End If

        ScrollViewer1.Content = stk
    End Sub

    Private Sub Refresh()
        stk.Children.Clear()

        Dim SQLcommand As SQLiteCommand
        Dim SQLreader As SQLiteDataReader

        SQLcommand = SQLconnect.CreateCommand
        SQLcommand.CommandText = "SELECT * FROM contact order by nom"
        SQLreader = SQLcommand.ExecuteReader()

        Dim a As String = ""

        If SQLreader.HasRows = True Then
            While SQLreader.Read()
                Dim x As New Label
                Dim doc As New TextBlock 'FlowDocument
                Dim para As New Paragraph


                If Mid(SQLreader(1), 1, 1) <> a Then
                    a = Mid(SQLreader(1), 1, 1)
                    Dim y As New Label
                    y.Width = ScrollViewer1.Width - 10
                    y.Height = 25
                    y.Background = Brushes.Blue
                    y.Foreground = Brushes.White
                    y.Content = a
                    y.Tag = "Lbl"
                    stk.Children.Add(y)
                End If

                x.Width = ScrollViewer1.Width - 10
                x.Height = 25
                x.Tag = SQLreader(0)
                x.Background = Brushes.White
                x.Foreground = Brushes.Black

                doc.Inlines.Add(New Bold(New Run(SQLreader(1) & " ")))
                doc.Inlines.Add(SQLreader(2))
                x.Content = doc
                AddHandler x.MouseDown, AddressOf Contact_Click
                stk.Children.Add(x)

                MaxRecord += 1
            End While
        End If

        ScrollViewer1.Content = stk
    End Sub

    Private Sub Contact_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Try
            Dim SQLcommand As SQLiteCommand
            Dim SQLreader As SQLiteDataReader

            SQLcommand = SQLconnect.CreateCommand
            SQLcommand.CommandText = "SELECT * FROM contact where contact_id='" & sender.tag & "'"
            SQLreader = SQLcommand.ExecuteReader()

            If SQLreader.HasRows = True Then
                While SQLreader.Read()
                    TxtID = SQLreader(0)
                    TxtNOM.Text = SQLreader(1)
                    TxtPRENOM.Text = SQLreader(2)
                    TxtTFixe.Text = SQLreader(3)
                    TxtTMobile.Text = SQLreader(4)
                    TxtTAutre.Text = SQLreader(5)
                    TxtMail.Text = SQLreader(6)
                    TxtAdresse.Text = SQLreader(7)
                End While
            End If

            CvsContact.Visibility = Windows.Visibility.Visible
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        SQLconnect.Close()
        MyBase.Finalize()
    End Sub

    Private Sub AffContact(ByVal Index As Integer)
        Dim SQLcommand As SQLiteCommand
        SQLcommand = SQLconnect.CreateCommand
        Dim SQLreader As SQLiteDataReader
        Dim i As Integer = 1

        SQLcommand.CommandText = "SELECT * FROM contact"
        SQLreader = SQLcommand.ExecuteReader()
        If SQLreader.HasRows = True Then
            While SQLreader.Read()
                If i = Index Then
                    TxtID = SQLreader(0)
                    TxtNOM.Text = SQLreader(1)
                    TxtPRENOM.Text = SQLreader(2)
                    TxtTFixe.Text = SQLreader(3)
                    TxtTMobile.Text = SQLreader(4)
                    TxtTAutre.Text = SQLreader(5)
                    TxtMail.Text = SQLreader(6)
                    TxtAdresse.Text = SQLreader(7)
                    Exit Sub
                Else
                    i += 1
                End If
            End While
        End If
    End Sub

    Private Function NewContact(ByVal Nom As String, ByVal Prenom As String, ByVal TelFix As String, ByVal TelMobil As String, ByVal TelAutre As String, ByVal Mail As String, ByVal Adresse As String) As String
        Try
            Dim SQLcommand As SQLiteCommand
            Dim RandomClass As New Random
            Dim j As Double = RandomClass.Next(0, 30000)
            SQLcommand = SQLconnect.CreateCommand
            SQLcommand.CommandText = "INSERT INTO contact (contact_id,nom, prenom,telfixe,telmobile,telautre,mail,adresse) VALUES ('" & j & "','" & Nom & "', '" & Prenom & "','" & TelFix & "','" & TelMobil & "','" & TelAutre & "','" & Mail & "','" & Adresse & "')"
            SQLcommand.ExecuteNonQuery()
            SQLcommand.Dispose()
            MaxRecord += 1
            Return ""
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Private Function DelContact(ByVal ID As Integer) As String
        Try
            Dim SQLcommand As SQLiteCommand
            SQLcommand = SQLconnect.CreateCommand
            SQLcommand.CommandText = "DELETE FROM contact where Contact_id='" & ID & "'"
            SQLcommand.ExecuteNonQuery()
            SQLcommand.Dispose()
            MaxRecord -= 1
            Return ""
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Private Function ModifyContact(ByVal ID As String, ByVal Nom As String, ByVal Prenom As String, ByVal TelFix As String, ByVal TelMobil As String, ByVal TelAutre As String, ByVal Mail As String, ByVal Adresse As String) As String
        Try
            Dim SQLcommand As SQLiteCommand
            SQLcommand = SQLconnect.CreateCommand
            SQLcommand.CommandText = "UPDATE contact SET nom='" & Nom & "', prenom='" & Prenom & "',telfixe='" & TelFix & "',telmobile='" & TelMobil & "',telautre='" & TelAutre & "',mail='" & Mail & "',adresse='" & Adresse & "' WHERE Contact_ID='" & ID & "'"
            SQLcommand.ExecuteNonQuery()
            SQLcommand.Dispose()
            Return ""
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Private Sub BtnNew_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnNew.Click
        FlagNew = True
        TxtID = ""
        TxtNOM.Text = ""
        TxtPRENOM.Text = ""
        TxtTFixe.Text = ""
        TxtTMobile.Text = ""
        TxtTAutre.Text = ""
        TxtMail.Text = ""
        TxtAdresse.Text = ""
        CvsContact.Visibility = Windows.Visibility.Visible
    End Sub

    Private Sub BtnSave_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnSave.Click
        If TxtNOM.Text = "" Then
            MsgBox("Veuillez au moins saisir un nom")
            Exit Sub
        End If
        If FlagNew = False Then
            Dim r As String = ModifyContact(TxtID, TxtNOM.Text, TxtPRENOM.Text, TxtTFixe.Text, TxtTMobile.Text, TxtTAutre.Text, TxtMail.Text, TxtAdresse.Text)
            If r <> "" Then
                MsgBox("Erreur: " & r)
            End If
        End If
        If FlagNew = True Then
            If TxtNOM.Text = "" Then
                MsgBox("Veuillez au moins saisir un nom")
                Exit Sub
            End If
            Dim r As String = NewContact(TxtNOM.Text, TxtPRENOM.Text, TxtTFixe.Text, TxtTMobile.Text, TxtTAutre.Text, TxtMail.Text, TxtAdresse.Text)
            If r <> "" Then
                MsgBox(r)
            End If
            FlagNew = False
        End If
        CvsContact.Visibility = Windows.Visibility.Hidden
        Refresh()
    End Sub

    Private Sub BtnDelete_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnDelete.Click
        Dim r As String = DelContact(TxtID)
        CvsContact.Visibility = Windows.Visibility.Hidden
        If r <> "" Then
            MsgBox("Erreur: " & r)
        End If
        Refresh()
    End Sub

    Private Sub CvsList_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles CvsList.MouseDown
        CvsContact.Visibility = Windows.Visibility.Hidden
    End Sub

    Private Sub ScrollViewer1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
        CvsContact.Visibility = Windows.Visibility.Hidden
    End Sub

    Private Sub BtnAnnul_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnAnnul.Click
        CvsContact.Visibility = Windows.Visibility.Hidden
    End Sub

    Private Sub ScrollViewer1_PreviewMouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ScrollViewer1.PreviewMouseDown
        scrollStartPoint = e.GetPosition(Me)
        scrollStartOffset.Y = ScrollViewer1.VerticalOffset
    End Sub

    Private Sub ScrollViewer1_PreviewMouseMove(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles ScrollViewer1.PreviewMouseMove
        If e.LeftButton = MouseButtonState.Pressed Then
            Dim currentPoint As Point = e.GetPosition(Me)
            Dim delta As New Point(scrollStartPoint.X - currentPoint.X, scrollStartPoint.Y - currentPoint.Y)
            scrollTarget.Y = scrollStartOffset.Y + delta.Y
            ScrollToPosition(ScrollViewer1, scrollTarget.X, currentPoint.Y, m_SpeedTouch)
        End If
    End Sub
End Class
