Imports Oregon
Public Class Form1
    Dim WithEvents wmr100 As WMR100

    Dim oLastReception As New Collection

    Const FRAMEKEY_DATETIME As Integer = 96
    Const FRAMEKEY_RAIN As Integer = 65
    Const FRAMEKEY_TEMPERATURE As Integer = 66
    Const FRAMEKEY_WIND As Integer = 72
    Const FRAMEKEY_BAROMETER As Integer = 70

    Const FRAMELENGTH_DATETIME As Integer = 11
    Const FRAMELENGTH_RAIN As Integer = 16
    Const FRAMELENGTH_TEMPERATURE As Integer = 11
    Const FRAMELENGTH_WIND As Integer = 10
    Const FRAMELENGTH_BAROMETER As Integer = 7

    Private GlobalInputReportBuffer(1000) As Byte
    Private GlobalInputReportBufferLength As Integer = 0

    Private Delegate Sub MarshalToForm _
        (ByVal action As String, _
        ByVal textToAdd As String)

    Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        wmr100 = New WMR100

        wmr100._ProductID = "CA01" ' "0039" '"CA01"
        wmr100._VendorID = "0FDE" ' "045E" '"0FDE"

        wmr100.Start()

    End Sub

    Sub info(ByVal data As String) Handles wmr100.Info
        'TextBox1.Text = TextBox1.Text & data
        MyMarshalToForm("Log1", data)

    End Sub

    Sub translate(ByVal data As String) Handles wmr100.Translate
        Dim etape As string
        Dim count As Integer
        Dim byteValue As String
        Dim s As String = ""
        Dim tempdata As String = data
        Dim InputReportBuffer(9) As Byte
        Dim InputReportBufferlenght As Integer

        etape = "0"

        Try
            While tempdata <> ""
                Dim w As Integer = InStr(tempdata, ";")
                InputReportBuffer(InputReportBufferlenght) = CByte(Strings.Left(tempdata, w - 1))
                tempdata = Strings.Right(tempdata, tempdata.Length - w)
                InputReportBufferlenght = InputReportBufferlenght + 1
            End While

            etape = "1"

            For i As Integer = 2 To 1 + InputReportBuffer(1) 'UBound(InputReportBuffer)
                GlobalInputReportBuffer(GlobalInputReportBufferLength + i - 2) = InputReportBuffer(i)
            Next i

            etape = "1-1"

            GlobalInputReportBufferLength += 1 + InputReportBuffer(1) - 1 'UBound(InputReportBuffer) - 1

            etape = "1-2"

            For i As Integer = 0 To UBound(InputReportBuffer)
                InputReportBuffer(i) = 0
            Next i

            etape = "1-3"

            For count = GlobalInputReportBufferLength - 1 To 1 Step -1
                If GlobalInputReportBuffer(count) <> 0 Or s <> "" Then

                    etape = "1-4"

                    'Add a leading zero to values from 0 to F.
                    If Len(Hex(GlobalInputReportBuffer(count))) < 2 Then
                        byteValue = "0" & Hex(GlobalInputReportBuffer(count))
                    Else
                        byteValue = Hex(GlobalInputReportBuffer(count))
                    End If

                    etape = "1-5"

                    If s <> "" Then s = " " & s
                    s = byteValue & s
                End If
            Next count

            etape = "2"

            Dim frameKeys As Integer() = {FRAMEKEY_DATETIME, _
                                          FRAMEKEY_RAIN, _
                                          FRAMEKEY_TEMPERATURE, _
                                          FRAMEKEY_WIND, _
                                          FRAMEKEY_BAROMETER}
            Dim frameLengths As Integer() = {FRAMELENGTH_DATETIME, _
                                          FRAMELENGTH_RAIN, _
                                          FRAMELENGTH_TEMPERATURE, _
                                          FRAMELENGTH_WIND, _
                                          FRAMELENGTH_BAROMETER}

            Dim iMaxFrame As Integer = 0

            Dim f As Integer
            Dim j As Integer
            Dim checkSum As Long

            etape = "3"

            For i = 0 To GlobalInputReportBufferLength - 1
                For f = 0 To frameKeys.Length - 1
                    If GlobalInputReportBuffer(i + 1) = frameKeys(f) Then
                        checkSum = 0
                        If (i + frameLengths(f) < GlobalInputReportBufferLength) Then
                            For j = i To i + frameLengths(f) - 2
                                checkSum += GlobalInputReportBuffer(j)
                            Next j
                            checkSum = checkSum

                            etape = "4"

                            If (GlobalInputReportBuffer(i + frameLengths(f) - 1) + 256 * GlobalInputReportBuffer(i + frameLengths(f)) = checkSum) Then
                                'If HEXA_LOG_MODE = 2 Then
                                '    MyMarshalToForm("Log1", Format(Now, "HH:mm:ss") & " : Frame start in buffer : " & i & vbCrLf)
                                'End If
                                'Frame Found and CheckSum OK
                                Dim bytes(frameLengths(f)) As Byte
                                For j = i To i + frameLengths(f)
                                    bytes(j - i) = GlobalInputReportBuffer(j)
                                Next j

                                ''''

                                'For l = 0 To bytes.Length - 3
                                'checkSum += bytes(l)
                                'Next l
                                'checkSum = checkSum Mod 256

                                Dim bCS As Boolean = False

                                Console.Write(Format(Now, "HH:mm:ss") & " : " & "[ " & s & " ]")
                                'MyMarshalToForm("Log2", Format(Now, "HH:mm:ss") & " : [ " & s & " ]" & vbCrLf)
                                If (checkSum = bytes(bytes.Length - 2) + 256 * bytes(bytes.Length - 1)) Then
                                    Console.WriteLine(" CheckSum OK")
                                    bCS = True
                                Else
                                    Console.WriteLine()
                                End If

                                etape = "5"

                                If bCS Then
                                    s = "OK : "
                                Else
                                    s = "     : "
                                End If

                                Dim sTime As String = ""
                                Dim lr As Date = Date.FromOADate(0)
                                If bCS Then
                                    Dim sKey As String = Hex(bytes(1))
                                    If bytes(1) = FRAMEKEY_TEMPERATURE Then

                                        Dim iSensor As Integer = bytes(2) Mod 16
                                        sKey &= iSensor
                                    End If
                                    Try
                                        lr = CDate(oLastReception(sKey))
                                    Catch ex As Exception
                                    End Try

                                    If (lr.ToOADate = 0) Then
                                        sTime = " "
                                    Else
                                        sTime = Int((Now.ToOADate - lr.ToOADate) * 24 * 60 * 60) & "s"
                                    End If

                                    Try
                                        oLastReception.Remove(sKey)
                                    Catch ex As Exception
                                    End Try
                                    oLastReception.Add(Now, sKey)
                                End If

                                etape = "6"

                                s &= " " & sTime & " : "

                                If (bytes(1) = FRAMEKEY_WIND) Or (bytes(1) = FRAMEKEY_DATETIME) Or (bytes(1) = FRAMEKEY_RAIN) Then
                                    Dim iPower As Integer = bytes(0) \ 16
                                    Dim bPower As Boolean = (iPower Mod 8) < 4
                                    Dim bWithoutSector As Boolean = iPower >= 8

                                    If bWithoutSector Then
                                        s &= "Secteur débranché ; "
                                    End If
                                    If bPower Then
                                        s &= "Piles chargées ; "
                                    Else
                                        s &= "Piles vides ; "
                                    End If
                                End If

                                etape = "7"

                                'Anémomètre
                                If (bytes(1) = FRAMEKEY_WIND) Then
                                    Dim dWindGust As Double = CDbl(bytes(4) + 256 * (bytes(5) Mod 16)) * 0.1
                                    Dim dWindAverage As Double = CDbl(bytes(5) \ 16 + bytes(6) * 16) * 0.1
                                    Dim dWindDirection As Double = (CDbl(bytes(2)) Mod 16) * 360 / 16


                                    s &= "Wind : "
                                    s &= "(G) " & dWindGust & " m/s ; "
                                    's &= "G " & CDbl(Bytes(4)) * 0.1 & " m/s ; "
                                    s &= "(M) " & dWindAverage & " m/s ; "
                                    s &= dWindDirection & "°"


                                    'If Not oForm1 Is Nothing Then
                                    'oForm1.WeatherDataManager.Add(WeatherDataManager.C_WINDGUST, dWindGust)
                                    'oForm1.WeatherDataManager.Add(WeatherDataManager.C_WINDSPEED, dWindAverage)
                                    'oForm1.WeatherDataManager.Add(WeatherDataManager.C_WINDDIRECTION, dWindDirection)
                                    'End If
                                End If

                                etape = "8"

                                'Thermomètre/Hygromètre
                                If bytes(1) = FRAMEKEY_TEMPERATURE Then

                                    Dim iSensor As Integer = bytes(2) Mod 16
                                    Dim dTemperature As Double = CDbl(bytes(3) + bytes(4) * 16 * 16) / 10
                                    Dim dHumidity As Double = CDbl(bytes(5))
                                    Dim dDewPoint As Double = CDbl(bytes(6) + bytes(7) * 16 * 16) / 10
                                    Dim iConfort As Integer = bytes(2) \ 16
                                    Dim iTempVariation As Integer = bytes(0) \ 16

                                    s &= "Sensor " & iSensor & " : "
                                    s &= dTemperature & "°C "
                                    Select Case iTempVariation
                                        Case 0 : s &= "(=) ; "
                                        Case 1 : s &= "(+) ; "
                                        Case 2 : s &= "(-) ; "
                                        Case Else : s &= "(" & iTempVariation & ") ; "
                                    End Select
                                    s &= dHumidity & "% ; "
                                    s &= dDewPoint & "°C ; "
                                    s &= "Confort(" & iConfort & ") "
                                    Select Case iConfort
                                        Case 8 : s &= "Non convenable"
                                        Case 12 : s &= "Neutre"
                                        Case Else : s &= "?"
                                    End Select

                                    'If Not oForm1 Is Nothing Then
                                    'If iSensor = 0 Then
                                    'oForm1.WeatherDataManager.Add(WeatherDataManager.C_INSIDETEMPERATURE, dTemperature)
                                    'oForm1.WeatherDataManager.Add(WeatherDataManager.C_INSIDEHUMIDITY, dHumidity)
                                    'End If
                                    'If iSensor = 1 Then
                                    'oForm1.WeatherDataManager.Add(WeatherDataManager.C_OUTSIDETEMPERATURE, dTemperature)
                                    'oForm1.WeatherDataManager.Add(WeatherDataManager.C_OUTSIDEHUMIDITY, dHumidity)
                                    'End If
                                    'End If
                                End If

                                etape = "9"

                                'Baromètre
                                If (bytes(1) = FRAMEKEY_BAROMETER) Then
                                    Dim dPressionRelative As Double = CDbl(bytes(4) + (bytes(5) Mod 16) * 16 * 16)
                                    Dim dPressionAbsolue As Double = CDbl(bytes(2) + (bytes(3) Mod 16) * 16 * 16)

                                    s &= "Baro : "
                                    s &= "PR=" & dPressionRelative & "mb ; "
                                    s &= "PA=" & dPressionAbsolue & "mb ; "

                                    Dim p As Integer

                                    p = bytes(5) \ 16
                                    Select Case p
                                        Case 0 : s &= "Partly Cloudy"
                                        Case 1 : s &= "Rainy"
                                        Case 2 : s &= "Cloudy"
                                        Case 3 : s &= "Sunny"
                                        Case 4 : s &= "Snowy"
                                        Case Else : s &= "?"
                                    End Select

                                    s &= " => "

                                    p = bytes(3) \ 16
                                    Select Case p
                                        Case 0 : s &= "Partly Cloudy"
                                        Case 1 : s &= "Rainy"
                                        Case 2 : s &= "Cloudy"
                                        Case 3 : s &= "Sunny"
                                        Case 4 : s &= "Snowy"
                                        Case Else : s &= "?"
                                    End Select

                                    'If Not oForm1 Is Nothing Then
                                    'oForm1.WeatherDataManager.Add(WeatherDataManager.C_BAROMETER, dPressionRelative)
                                    'End If
                                End If

                                etape = "10"

                                'Pluviomètre
                                If (bytes(1) = FRAMEKEY_RAIN) Then
                                    Dim dRainRate As Double = CDbl(bytes(2) + bytes(3) * 16 * 16) / 100 * 25.4
                                    Dim dRain1h As Double = CDbl(bytes(4) + bytes(5) * 16 * 16) / 100 * 25.4
                                    Dim dRain24h As Double = CDbl(bytes(6) + bytes(7) * 16 * 16) / 100 * 25.4
                                    Dim dRainAccum As Double = CDbl(bytes(8) + bytes(9) * 16 * 16) / 100 * 25.4

                                    s &= "Rain : "

                                    s &= "Rate = " & dRainRate & " mm/h ; "
                                    s &= "1h = " & dRain1h & " mm ; "
                                    s &= "24h  = " & dRain24h & " mm ; "
                                    s &= "Accum = " & dRainAccum & " mm "

                                    s &= "since " & Format(bytes(12), "00") & "/" & Format(bytes(13), "00") & "/" & Format(bytes(14), "00") & " "
                                    s &= Format(bytes(11), "00") & ":" & Format(bytes(10), "00")

                                    'If Not oForm1 Is Nothing Then
                                    'oForm1.WeatherDataManager.Add(WeatherDataManager.C_RAIN, dRain1h)
                                    'End If
                                End If
                                etape = "11"

                                'Date/Heure
                                If (bytes(1) = FRAMEKEY_DATETIME) Then
                                    Dim iRadioFreq As Integer = (bytes(0) \ 16) Mod 4

                                    s &= "Date/Time : "

                                    s &= Format(bytes(6), "00") & "/" & Format(bytes(7), "00") & "/" & Format(bytes(8), "00") & " "
                                    s &= Format(bytes(5), "00") & ":" & Format(bytes(4), "00") & " ; "
                                    s &= "GMT "
                                    If bytes(9) < 128 Then
                                        s &= "+"
                                    Else
                                        s &= "-"
                                    End If
                                    s &= Format(bytes(9) Mod 128, "00") & " ; "
                                    Select Case iRadioFreq
                                        Case 0 : s &= " RadioFreq désactivée"
                                        Case 1 : s &= " RadioFreq en recherche ou signal faible"
                                        Case 2 : s &= " RadioFreq signal moyen"
                                        Case 3 : s &= " RadioFreq signal fort"
                                    End Select

                                End If
                                MyMarshalToForm("Log1", s & vbCrLf)
                                etape = "12"

                                iMaxFrame = i + frameLengths(f)
                            End If
                        End If
                    End If
                Next f
            Next i

            etape = "13"

            If iMaxFrame > 0 Then
                For i = iMaxFrame + 1 To GlobalInputReportBufferLength
                    GlobalInputReportBuffer(i - iMaxFrame - 1) = GlobalInputReportBuffer(i)
                Next i
                GlobalInputReportBufferLength -= iMaxFrame + 1
                'GlobalInputReportBufferLength = 0
                'For i = GlobalInputReportBufferLength To UBound(GlobalInputReportBuffer)
                'GlobalInputReportBuffer(i) = 0
                'Next i

                'MyHID.FlushQueue(ReadHandle)

                'SendToHID(MSG_CI2)
                'SendToHID(MSG_PCR)

            End If


        Catch ex As Exception
            MsgBox("translate error = " & etape)
        End Try
    End Sub

    Private Sub MyMarshalToForm _
        (ByVal action As String, _
        ByVal textToDisplay As String)

        'Purpose    : Enables accessing a form's controls from another thread 

        'Accepts    : action - a string that names the action to perform on the form
        '           : formText - text that the form displays or the code uses for 
        '           : another purpose. Actions that don't use text ignore this parameter.  

        Dim args() As Object = {action, textToDisplay}
        Dim MarshalToFormDelegate As MarshalToForm

        ' The AccessForm routine contains the code that accesses the form.

        MarshalToFormDelegate = New MarshalToForm(AddressOf AccessForm)

        ' Execute AccessForm, passing the parameters in args.

        MyBase.Invoke(MarshalToFormDelegate, args)

    End Sub

    Private Sub AccessForm(ByVal action As String, ByVal formText As String)

        'Purpose    : In asynchronous ReadFiles, the callback function GetInputReportData  
        '           : uses this routine to access the application's Form, which runs in 
        '           : a different thread.
        '           : The routine performs various application-specific functions that
        '           : involve accessing the application's form.

        'Accepts    : action - a string that names the action to perform on the form
        '           : formText - text that the form displays or the code uses for 
        '           : another purpose. Actions that don't use text ignore this parameter.  

        Try

            ' Select an action to perform on the form:

            Dim bDeletePreviousLine As Boolean = False '(nNotDetectedMessage >= 1)

            Dim oTextBox As TextBox = Nothing

            Select Case action
                Case "Log1"
                    oTextBox = TextBox1

                Case "Log2"
                    'oTextBox = TextBox2


                Case Else
            End Select

            If Not IsNothing(oTextBox) Then
                If bDeletePreviousLine Then
                    Dim iPos As Integer = oTextBox.Text.LastIndexOf(vbCrLf, oTextBox.Text.Length - 2)
                    oTextBox.Text = oTextBox.Text.Remove(iPos + 2)
                End If
                oTextBox.Text = oTextBox.Text & formText
                oTextBox.SelectionStart = oTextBox.Text.Length
                oTextBox.ScrollToCaret()
            End If

        Catch ex As Exception
        End Try

    End Sub

   
End Class
