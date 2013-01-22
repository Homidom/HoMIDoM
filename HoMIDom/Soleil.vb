Imports System.Math
Imports HoMIDom

Namespace HoMIDom

    ''' <summary>
    ''' Classe permettant le calcul des heures de lever et coucher du soleil
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Soleil
        Dim _Server As Server

        Public Sub CalculateSolarTimes(ByVal dLatitude As Double, ByVal dLongitude As Double, ByVal dtDesiredDate As Date, ByRef dtSunrise As Date, ByRef dtSolarNoon As Date, ByRef dtSunset As Date)
            Try

                Dim lDaySavings As Long = 0
                Dim dGammaSolarNoon As Double = 0
                Dim dTimeGMT As Double = 0
                Dim dSolarNoonGMT As Double = 0
                Dim dTimeLST As Double = 0
                Dim dSolarNoonLST As Double = 0
                Dim dSunsetTimeGMT As Double = 0
                Dim dSunsetTimeLST As Double = 0
                Dim tsTimeZone As TimeSpan
                Dim dZone As Double
                Dim dEquationOfTime As Double = 0
                Dim dSolarDeclination As Double = 0

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

            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "Soleil:CalculateSolarTimes", "Exception : " & ex.ToString)
            End Try
        End Sub

        Private Function RadiansToDegrees(ByVal dAndgleInRadians As Double) As Double
            Try
                Return 180 * dAndgleInRadians / PI
            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "Soleil:RadiansToDegrees", "Exception : " & ex.ToString)
                Return 0
            End Try
        End Function

        Private Function DegreesToRadians(ByVal dAngleInDegrees As Double) As Double
            Try
                Return PI * dAngleInDegrees / 180
            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "Soleil:DegreesToRadians", "Exception : " & ex.ToString)
                Return 0
            End Try
        End Function

        Private Function CalculateGamma(ByVal nJulianDay As Long) As Double
            Try
                Return (2 * PI / 365) * (nJulianDay - 1)
            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "Soleil:CalculateGamma", "Exception : " & ex.ToString)
                Return 0
            End Try
        End Function

        Private Function CalculateGamma2(ByVal nJulianDay As Long, ByVal lHour As Long) As Double
            Try
                Return (2 * PI / 365) * (nJulianDay - 1 + (lHour / 24))
            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "Soleil:CalculateGamma2", "Exception : " & ex.ToString)
                Return 0
            End Try
        End Function

        Private Function CalculatedEquationOfTime(ByVal dGamma As Double) As Double
            Try
                Return (229.18 * (0.000075 + 0.001568 * Cos(dGamma) - 0.032077 * Sin(dGamma) - 0.014615 * Cos(2 * dGamma) - 0.040849 * Sin(2 * dGamma)))
            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "Soleil:CalculatedEquationOfTime", "Exception : " & ex.ToString)
                Return 0
            End Try
        End Function

        Private Function CalculateSolarDeclination(ByVal dGamma As Double) As Double
            Try
                Return (0.006918 - 0.399912 * Cos(dGamma) + 0.070257 * Sin(dGamma) - 0.006758 * Cos(2 * dGamma) + 0.000907 * Sin(2 * dGamma))
            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "Soleil:CalculateSolarDeclination", "Exception : " & ex.ToString)
                Return 0
            End Try
        End Function

        Private Function CalculateHourAngle(ByVal dLatitude As Double, ByVal dSolarDeclination As Double, ByVal bIsTime As Boolean) As Double
            Try
                Dim dRadianLatitude As Double = 0
                Dim dHourAngle As Double = 0

                dRadianLatitude = DegreesToRadians(dLatitude)
                If bIsTime Then
                    dHourAngle = (Acos(Cos(DegreesToRadians(90.833)) / (Cos(dRadianLatitude) * Cos(dSolarDeclination)) - Tan(dRadianLatitude) * Tan(dSolarDeclination)))
                Else
                    dHourAngle = -(Acos(Cos(DegreesToRadians(90.833)) / (Cos(dRadianLatitude) * Cos(dSolarDeclination)) - Tan(dRadianLatitude) * Tan(dSolarDeclination)))
                End If
                Return dHourAngle
            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "Soleil:CalculateHourAngle", "Exception : " & ex.ToString)
                Return 0
            End Try
        End Function

        Private Function CalculateSunriseGMT(ByVal lJulianDay As Long, ByVal dLatitude As Double, ByVal dLongitude As Double) As Double
            Try
                Dim dGamma As Double = 0
                Dim dEquationOfTime As Double = 0
                Dim dSolarDeclination As Double = 0
                Dim dHourAngle As Double = 0
                Dim dDelta As Double = 0
                Dim dTimeDifference As Double = 0
                Dim dTimeGMT As Double = 0
                Dim dGammaSunrise As Double = 0

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
            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "Soleil:CalculateSunriseGMT", "Exception : " & ex.ToString)
                Return 0
            End Try
        End Function

        Private Function CalculateSolarNoonGMT(ByVal nJulianDay As Long, ByVal nLongitude As Double) As Double
            Try
                Dim dGammaSolarNoon As Double = 0
                Dim dEquationOfTime As Double = 0
                Dim dSolarNoonDeclination As Double = 0
                Dim dSolarNoonGMT As Double = 0

                dGammaSolarNoon = CalculateGamma2(nJulianDay, CLng(12 + (nLongitude / 15)))
                dEquationOfTime = CalculatedEquationOfTime(dGammaSolarNoon)
                dSolarNoonDeclination = CalculateSolarDeclination(dGammaSolarNoon)
                dSolarNoonGMT = 720 + (nLongitude * 4) - dEquationOfTime

                Return dSolarNoonGMT
            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "Soleil:CalculateSolarNoonGMT", "Exception : " & ex.ToString)
                Return 0
            End Try
        End Function

        Private Function CalculateSunsetGMT(ByVal nJulianDay As Long, ByVal nLatitude As Double, ByVal nLongitude As Double) As Double
            Try
                Dim dGamma As Double = 0
                Dim dEquationOfTime As Double = 0
                Dim dSolarDeclination As Double = 0
                Dim dHourAngle As Double = 0
                Dim dDelta As Double = 0
                Dim dTimeDiff As Double = 0
                Dim dSetTimeGMT As Double = 0
                Dim dGammaSunset As Double = 0

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
            Catch ex As Exception
                _Server.Log(Server.TypeLog.ERREUR, Server.TypeSource.SERVEUR, "Soleil:CalculateSunsetGMT", "Exception : " & ex.ToString)
                Return 0
            End Try
        End Function
    End Class

End Namespace