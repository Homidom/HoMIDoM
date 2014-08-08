Imports System.Windows.Media.Animation

Public Class uMoteur
    Dim _val As Integer = 0
    Dim _flagFirst As Boolean = True
    Dim _Min As Integer = 0
    Dim _Max As Integer = 1
    Dim _value As Double = 1
    Dim _Echelle As Double = _Max - _Min

    Public Property Min As Integer
        Get
            Return _Min
        End Get
        Set(value As Integer)
            _Min = value
            _Echelle = _Max - _Min
        End Set
    End Property

    Public Property Max As Integer
        Get
            Return _Max
        End Get
        Set(value As Integer)
            _Max = value
            _Echelle = _Max - _Min
        End Set
    End Property

    Public Property Value As Integer
        Get
            Return _val
        End Get
        Set(value As Integer)
            Try
                If IsNumeric(value) Then
                    If value = _val Then Exit Property
                    _val = value

                    'Calcul à effectuer sachant que la vitesse max=1
                    If _val < _Min Then _val = _Min
                    If _val > _Max Then _val = _Max

                    'If _val = 0 And _Echelle = 0 Then Exit Property

                    _value = 1.5 - CDbl(_val / _Echelle)

                    If _value = 1.5 Then _value = 0

                    'Animation
                    Dim da As DoubleAnimation = New DoubleAnimation(0, 360, New Duration(TimeSpan.FromSeconds(_value)))
                    Dim rt As RotateTransform = New RotateTransform()
                    Img.RenderTransform = rt
                    Img.RenderTransformOrigin = New Point(0.5, 0.5)
                    da.RepeatBehavior = RepeatBehavior.Forever
                    rt.BeginAnimation(RotateTransform.AngleProperty, da)
                End If
            Catch ex As Exception
                AfficheMessageAndLog(FctLog.TypeLog.ERREUR, "Erreur uMoteur.Value: " & ex.ToString & vbCrLf & "Value=" & _value, "Erreur", "uMoteur.Value")
            End Try
        End Set
    End Property


End Class
