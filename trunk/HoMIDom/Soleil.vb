Imports System.Math

Namespace HoMIDom

    ''' <summary>
    ''' Classe permettant le calcul des heures de lever et coucher du soleil
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Soleil
        Public Sub CalculateSolarTimes(ByVal dLatitude As Double, ByVal dLongitude As Double, _
                                     ByVal dtDesiredDate As Date, ByRef dtSunrise As Date, _
                                     ByRef dtSolarNoon As Date, ByRef dtSunset As Date)
            Dim lDaySavings As Long
            Dim dGammaSolarNoon As Double
            Dim dTimeGMT As Double
            Dim dSolarNoonGMT As Double
            Dim dTimeLST As Double
            Dim dSolarNoonLST As Double
            Dim dSunsetTimeGMT As Double
            Dim dSunsetTimeLST As Double
            Dim tsTimeZone As TimeSpan
            Dim dZone As Double
            Dim dEquationOfTime As Double
            Dim dSolarDeclination As Double

            dLongitude = dLongitude * -1

            If dtDesiredDate.IsDaylightSavingTime Then
                lDaySavings = 60
            Else
                lDaySavings = 0
            End If
            tsTimeZone = dtDesiredDate.ToUniversalTime.Subtract(dtDesiredDate)
            dZone = tsTimeZone.TotalHours
            If dtDesiredDate.IsDaylightSavingTime Then dZone += 1

            If dLatitude >= -90 And dLatitude < -89.8 Then
                dLatitude = -89.8
            End If
            If dLatitude <= 90 And dLatitude > 89.8 Then
                dLatitude = 89.8
            End If

            ' Calculate the time of sunrise
            dGammaSolarNoon = CalculateGamma2(dtDesiredDate.DayOfYear, CLng(12 + (dLongitude / 15)))
            dEquationOfTime = CalculatedEquationOfTime(dGammaSolarNoon)
            dSolarDeclination = CalculateSolarDeclination(dGammaSolarNoon)

            dTimeGMT = CalculateSunriseGMT(dtDesiredDate.DayOfYear, dLatitude, dLongitude)

            dSolarNoonGMT = CalculateSolarNoonGMT(dtDesiredDate.DayOfYear, dLongitude)

            dTimeLST = dTimeGMT - (60 * dZone) + lDaySavings
            dtSunrise = dtDesiredDate.Date.AddMinutes(CInt(dTimeLST))

            'Calculate solar noon
            dSolarNoonLST = dSolarNoonGMT - (60 * dZone) + lDaySavings
            dtSolarNoon = dtDesiredDate.Date.AddMinutes(dSolarNoonLST)

            'Calculate  sunset
            dSunsetTimeGMT = CalculateSunsetGMT(dtDesiredDate.DayOfYear, dLatitude, dLongitude)
            dSunsetTimeLST = dSunsetTimeGMT - (60 * dZone) + lDaySavings
            dtSunset = dtDesiredDate.Date.AddSeconds(dSunsetTimeLST * 60)

        End Sub

        Private Function RadiansToDegrees(ByVal dAndgleInRadians As Double) As Double
            Return 180 * dAndgleInRadians / PI
        End Function

        Private Function DegreesToRadians(ByVal dAngleInDegrees As Double) As Double
            Return PI * dAngleInDegrees / 180
        End Function

        Private Function CalculateGamma(ByVal nJulianDay As Long) As Double
            Return (2 * PI / 365) * (nJulianDay - 1)
        End Function

        Private Function CalculateGamma2(ByVal nJulianDay As Long, ByVal lHour As Long) As Double
            Return (2 * PI / 365) * (nJulianDay - 1 + (lHour / 24))
        End Function

        Private Function CalculatedEquationOfTime(ByVal dGamma As Double) As Double
            Return (229.18 * (0.000075 + 0.001568 * Cos(dGamma) - 0.032077 * Sin(dGamma) - _
                    0.014615 * Cos(2 * dGamma) - 0.040849 * Sin(2 * dGamma)))
        End Function

        Private Function CalculateSolarDeclination(ByVal dGamma As Double) As Double
            Return (0.006918 - 0.399912 * Cos(dGamma) + 0.070257 * Sin(dGamma) - 0.006758 * _
                    Cos(2 * dGamma) + 0.000907 * Sin(2 * dGamma))
        End Function

        Private Function CalculateHourAngle(ByVal dLatitude As Double, ByVal dSolarDeclination As Double, _
                                            ByVal bIsTime As Boolean) As Double

            Dim dRadianLatitude As Double
            Dim dHourAngle As Double

            dRadianLatitude = DegreesToRadians(dLatitude)

            If bIsTime Then
                dHourAngle = (Acos(Cos(DegreesToRadians(90.833)) / (Cos(dRadianLatitude) * _
                   Cos(dSolarDeclination)) - Tan(dRadianLatitude) * Tan(dSolarDeclination)))

            Else
                dHourAngle = -(Acos(Cos(DegreesToRadians(90.833)) / (Cos(dRadianLatitude) * _
                   Cos(dSolarDeclination)) - Tan(dRadianLatitude) * Tan(dSolarDeclination)))

            End If

            Return dHourAngle

        End Function

        Private Function CalculateSunriseGMT(ByVal lJulianDay As Long, ByVal dLatitude As Double, _
                                             ByVal dLongitude As Double) As Double

            Dim dGamma As Double
            Dim dEquationOfTime As Double
            Dim dSolarDeclination As Double
            Dim dHourAngle As Double
            Dim dDelta As Double
            Dim dTimeDifference As Double
            Dim dTimeGMT As Double
            Dim dGammaSunrise As Double

            dGamma = CalculateGamma(lJulianDay)
            dEquationOfTime = CalculatedEquationOfTime(dGamma)
            dSolarDeclination = CalculateSolarDeclination(dGamma)
            dHourAngle = CalculateHourAngle(dLatitude, dSolarDeclination, True)
            dDelta = dLongitude - RadiansToDegrees(dHourAngle)
            dTimeDifference = 4 * dDelta
            dTimeGMT = 720 + dTimeDifference - dEquationOfTime

            dGammaSunrise = CalculateGamma2(lJulianDay, CLng(dTimeGMT / 60))
            dEquationOfTime = CalculatedEquationOfTime(dGammaSunrise)
            dSolarDeclination = CalculateSolarDeclination(dGammaSunrise)
            dHourAngle = CalculateHourAngle(dLatitude, dSolarDeclination, True)
            dDelta = dLongitude - RadiansToDegrees(dHourAngle)
            dTimeDifference = 4 * dDelta
            dTimeGMT = 720 + dTimeDifference - dEquationOfTime

            Return dTimeGMT

        End Function

        Private Function CalculateSolarNoonGMT(ByVal nJulianDay As Long, ByVal nLongitude As Double) As Double

            Dim dGammaSolarNoon As Double
            Dim dEquationOfTime As Double
            Dim dSolarNoonDeclination As Double
            Dim dSolarNoonGMT As Double

            dGammaSolarNoon = CalculateGamma2(nJulianDay, CLng(12 + (nLongitude / 15)))
            dEquationOfTime = CalculatedEquationOfTime(dGammaSolarNoon)
            dSolarNoonDeclination = CalculateSolarDeclination(dGammaSolarNoon)
            dSolarNoonGMT = 720 + (nLongitude * 4) - dEquationOfTime

            Return dSolarNoonGMT

        End Function

        Private Function CalculateSunsetGMT(ByVal nJulianDay As Long, ByVal nLatitude As Double, _
                                            ByVal nLongitude As Double) As Double


            Dim dGamma As Double
            Dim dEquationOfTime As Double
            Dim dSolarDeclination As Double
            Dim dHourAngle As Double
            Dim dDelta As Double
            Dim dTimeDiff As Double
            Dim dSetTimeGMT As Double
            Dim dGammaSunset As Double

            dGamma = CalculateGamma(nJulianDay + 1)
            dEquationOfTime = CalculatedEquationOfTime(dGamma)
            dSolarDeclination = CalculateSolarDeclination(dGamma)
            dHourAngle = CalculateHourAngle(nLatitude, dSolarDeclination, False)
            dDelta = nLongitude - RadiansToDegrees(dHourAngle)
            dTimeDiff = 4 * dDelta
            dSetTimeGMT = 720 + dTimeDiff - dEquationOfTime

            dGammaSunset = CalculateGamma2(nJulianDay, CLng(dSetTimeGMT / 60))
            dEquationOfTime = CalculatedEquationOfTime(dGammaSunset)

            dSolarDeclination = CalculateSolarDeclination(dGammaSunset)

            dHourAngle = CalculateHourAngle(nLatitude, dSolarDeclination, False)
            dDelta = nLongitude - RadiansToDegrees(dHourAngle)
            dTimeDiff = 4 * dDelta
            dSetTimeGMT = 720 + dTimeDiff - dEquationOfTime

            Return dSetTimeGMT

        End Function
    End Class

End Namespace