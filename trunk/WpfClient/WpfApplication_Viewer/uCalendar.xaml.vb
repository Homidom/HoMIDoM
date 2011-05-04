Imports System.Xml
Imports System.Xml.XPath
Imports System.IO

Partial Public Class uCalendar
    '***********************************
    'Variables *************************
    '**********************************
    Dim WithEvents aptCalendar As MonthView = New MonthView
    Private _myAppointmentsList As New List(Of Appointment)
    Dim SelectDay As String
    Dim oTxtSujet As New TextBox
    Dim oTxtDesc As New TextBox
    Dim oID As Integer
    '**********************************************************

    Public Sub New()

        ' Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        Try
            If File.Exists("C:\ehome\data\calendrier.xml") = True Then
                Dim myxml As clsXML = New clsXML("C:\ehome\data\calendrier.xml")
                Dim list As XmlNodeList = myxml.SelectNodes("/calendrier/element")
                Dim i As Integer
                For i = 0 To list.Count - 1
                    Dim x As New Appointment
                    x.AppointmentID = list(i).Attributes.Item(0).Value
                    x.StartTime = list(i).Attributes.Item(2).Value
                    x.EndTime = list(i).Attributes.Item(3).Value
                    x.Subject = list(i).Attributes.Item(1).Value
                    x.Details = list(i).Attributes.Item(4).Value
                    _myAppointmentsList.Add(x)
                Next
            End If
        Catch ex As Exception
            MsgBox("Erreur lors du chargement du fichier calendrier.xml:" & ex.Message)
        End Try

        aptCalendar.VerticalAlignment = Windows.VerticalAlignment.Stretch
        aptCalendar.VerticalContentAlignment = Windows.VerticalAlignment.Center
        Grid1.Children.Add(aptCalendar)
        SetAppointments()
    End Sub

    'Double click sur un jour du calendrier
    Private Sub DayBoxDoubleClicked_event(ByVal e As NewAppointmentEventArgs) Handles aptCalendar.DayBoxDoubleClicked
        Dim mycanva As New Canvas
        Dim lbl As New Label
        Dim TxtSujet As New TextBox
        Dim TxtDesc As New TextBox
        Dim BtnSave As New Button
        Dim BtnClose As New Button
        Dim myBrush As New LinearGradientBrush

        myBrush.GradientStops.Add(New GradientStop(Colors.DarkGray, 0.0))
        myBrush.GradientStops.Add(New GradientStop(Colors.Black, 1.0))

        lbl.Content = CDate(e.StartDate).ToShortDateString()
        SelectDay = lbl.Content
        lbl.Foreground = New SolidColorBrush(Colors.White)
        lbl.FontSize = 14
        mycanva.Children.Add(lbl)
        mycanva.SetLeft(lbl, 10)
        mycanva.SetTop(lbl, 10)

        TxtSujet.Text = "Sujet:"
        TxtSujet.Width = 350
        TxtSujet.Height = 30
        TxtSujet.FontSize = 12
        oTxtSujet = TxtSujet
        mycanva.Children.Add(TxtSujet)
        mycanva.SetLeft(TxtSujet, 10)
        mycanva.SetTop(TxtSujet, 40)

        TxtDesc.Text = "Description:"
        TxtDesc.Width = 350
        TxtDesc.Height = 200
        TxtDesc.TextWrapping = TextWrapping.Wrap
        TxtDesc.FontSize = 12
        oTxtDesc = TxtDesc
        mycanva.Children.Add(TxtDesc)
        mycanva.SetLeft(TxtDesc, 10)
        mycanva.SetTop(TxtDesc, 80)

        BtnSave.Content = "Enregistrer"
        BtnSave.Width = 80
        AddHandler BtnSave.Click, AddressOf Save
        mycanva.Children.Add(BtnSave)
        mycanva.SetLeft(BtnSave, 150)
        mycanva.SetTop(BtnSave, 10)

        BtnClose.Content = "Fermer"
        BtnClose.Width = 80
        AddHandler BtnClose.Click, AddressOf Close
        mycanva.Children.Add(BtnClose)
        mycanva.SetLeft(BtnClose, 250)
        mycanva.SetTop(BtnClose, 10)

        mycanva.Height = 300
        mycanva.Width = 400
        mycanva.Background = myBrush
        Grid1.Children.Add(mycanva)
    End Sub

#Region "Gestion appointment"
    'Enregistre un nouveau appointment 
    Private Sub Save()
        Dim x As New Appointment
        x.StartTime = SelectDay
        x.EndTime = SelectDay
        x.Details = oTxtDesc.Text
        x.Subject = oTxtSujet.Text
        x.AppointmentID = GenerateID()
        _myAppointmentsList.Add(x)
        SetAppointments()
        Close()
    End Sub

    'Modifier un appointment
    Private Sub Modifier()
        For i As Integer = 0 To _myAppointmentsList.Count - 1
            If _myAppointmentsList.Item(i).AppointmentID = oID Then
                _myAppointmentsList.Item(i).Subject = oTxtSujet.Text
                _myAppointmentsList.Item(i).Details = oTxtDesc.Text

                Exit For
            End If
        Next
        SetAppointments()
        Close()
    End Sub

    'Efface un appointment 
    Private Sub Efface()
        For i As Integer = 0 To _myAppointmentsList.Count - 1
            If _myAppointmentsList.Item(i).AppointmentID = oID Then
                _myAppointmentsList.RemoveAt(i)
                Exit For
            End If
        Next
        SetAppointments()
        Close()
    End Sub

    Private Function GenerateID() As Integer
        Dim _ID As Integer = 0
        For i As Integer = 0 To _myAppointmentsList.Count - 1
            If _myAppointmentsList.Item(i).AppointmentID > _ID Then
                _ID += _myAppointmentsList.Item(i).AppointmentID + 1
            End If
        Next
        Return _ID
    End Function
#End Region

    'Ferme la fenêtre de l'appointment
    Private Sub Close()
        If File.Exists("C:\ehome\data\calendrier.xml") Then File.Delete("C:\ehome\data\calendrier.xml")
        Try
            Dim SW As New StreamWriter("C:\ehome\data\calendrier.xml")
            SW.WriteLine("<?xml version=" & Chr(34) & "1.0" & Chr(34) & " encoding=" & Chr(34) & "utf-8" & Chr(34) & " ?>")
            SW.WriteLine("<calendrier>")
            If _myAppointmentsList.Count > 0 Then
                For i As Integer = 0 To _myAppointmentsList.Count - 1
                    SW.WriteLine("<element id='" & _myAppointmentsList.Item(i).AppointmentID & "' sujet='" & ConvertTextToHTML(_myAppointmentsList.Item(i).Subject) & "' startdate='" & _myAppointmentsList.Item(i).StartTime & "' enddate='" & _myAppointmentsList.Item(i).EndTime & "' details='" & ConvertTextToHTML(_myAppointmentsList.Item(i).Details) & "' />")
                Next
            End If
            SW.WriteLine("</calendrier>")
            SW.Close()
        Catch e As Exception
            MsgBox("Erreur dans la création du fichier : " & e.Message)
        End Try
        Grid1.Children.RemoveAt(Grid1.Children.Count - 1)
    End Sub

    Private Sub AppointmentDblClicked(ByVal Appointment_Id As Integer) Handles aptCalendar.AppointmentDblClicked
        Dim mycanva As New Canvas
        Dim lbl As New Label
        Dim TxtSujet As New TextBox
        Dim TxtDesc As New TextBox
        Dim BtnSave As New Button
        Dim BtnClose As New Button
        Dim BtnDelete As New Button
        Dim myBrush As New LinearGradientBrush
        Dim x As New Appointment

        For i As Integer = 0 To _myAppointmentsList.Count - 1
            If _myAppointmentsList.Item(i).AppointmentID = Appointment_Id Then
                oID = Appointment_Id
                x = _myAppointmentsList.Item(i)
            End If
        Next

        myBrush.GradientStops.Add(New GradientStop(Colors.DarkGray, 0.0))
        myBrush.GradientStops.Add(New GradientStop(Colors.Black, 1.0))

        lbl.Content = x.StartTime
        SelectDay = lbl.Content
        lbl.Foreground = New SolidColorBrush(Colors.White)
        lbl.FontSize = 14
        mycanva.Children.Add(lbl)
        mycanva.SetLeft(lbl, 10)
        mycanva.SetTop(lbl, 10)

        TxtSujet.Text = x.Subject
        TxtSujet.Width = 350
        TxtSujet.Height = 30
        TxtSujet.FontSize = 12
        oTxtSujet = TxtSujet
        mycanva.Children.Add(TxtSujet)
        mycanva.SetLeft(TxtSujet, 10)
        mycanva.SetTop(TxtSujet, 40)

        TxtDesc.Text = x.Details
        TxtDesc.Width = 350
        TxtDesc.Height = 200
        TxtDesc.TextWrapping = TextWrapping.Wrap
        TxtDesc.FontSize = 12
        oTxtDesc = TxtDesc
        mycanva.Children.Add(TxtDesc)
        mycanva.SetLeft(TxtDesc, 10)
        mycanva.SetTop(TxtDesc, 80)

        BtnSave.Content = "Enregistrer"
        BtnSave.Width = 80
        AddHandler BtnSave.Click, AddressOf Modifier
        mycanva.Children.Add(BtnSave)
        mycanva.SetLeft(BtnSave, 120)
        mycanva.SetTop(BtnSave, 10)

        BtnClose.Content = "Fermer"
        BtnClose.Width = 80
        AddHandler BtnClose.Click, AddressOf Close
        mycanva.Children.Add(BtnClose)
        mycanva.SetLeft(BtnClose, 210)
        mycanva.SetTop(BtnClose, 10)

        BtnDelete.Content = "Effacer"
        BtnDelete.Width = 80
        AddHandler BtnDelete.Click, AddressOf Efface
        mycanva.Children.Add(BtnDelete)
        mycanva.SetLeft(BtnDelete, 300)
        mycanva.SetTop(BtnDelete, 10)

        mycanva.Height = 300
        mycanva.Width = 400
        mycanva.Background = myBrush
        Grid1.Children.Add(mycanva)
    End Sub

    Private Sub DisplayMonthChanged(ByVal e As MonthChangedEventArgs) Handles aptCalendar.DisplayMonthChanged
        Call SetAppointments()
    End Sub

    Private Sub SetAppointments()
        '-- Use whatever function you want to load the MonthAppointments list, I happen to have a list filled by linq that has
        '   many (possibly the past several years) of them loaded, so i filter to only pass the ones showing up in the displayed
        '   month.  Note that the "setter" for MonthAppointments also triggers a redraw of the display.
        Me.aptCalendar.MonthAppointments = _myAppointmentsList.FindAll( _
                        New System.Predicate(Of Appointment)( _
                        Function(apt As Appointment) _
                            apt.StartTime IsNot Nothing AndAlso _
                            CDate(apt.StartTime).Month = Me.aptCalendar.DisplayStartDate.Month AndAlso _
                            CDate(apt.StartTime).Year = Me.aptCalendar.DisplayStartDate.Year))
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
