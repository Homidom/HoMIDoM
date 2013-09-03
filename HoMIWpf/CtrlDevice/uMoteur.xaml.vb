Imports System.Windows.Media.Animation

Public Class uMoteur
    Dim _val As Integer = 0
    Dim _flagFirst As Boolean = True

    Public Property Value As Integer
        Get
            Return _val
        End Get
        Set(value As Integer)
            _val = value

        End Set
    End Property

    'var da = new DoubleAnimation(360, 0, new Duration(TimeSpan.FromSeconds(1)));
    '    var rt = new RotateTransform();
    '    rect1.RenderTransform = rt;
    '    rect1.RenderTransformOrigin = new Point(0.5, 0.5);
    '    da.RepeatBehavior = RepeatBehavior.Forever;
    '    rt.BeginAnimation(RotateTransform.AngleProperty, da);

End Class
