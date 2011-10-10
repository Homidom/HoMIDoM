Imports System
Imports System.Text
Imports System.Drawing
Imports System.Drawing.Imaging
Imports ZedGraph
Imports HoMIDom
Imports HoMIDom.HoMIDom.Server

Namespace HoMIDom

    Public Class graphes
        Dim zg As New ZedGraphControl
        Dim _repertoire As String = ""
        Dim _Server As HoMIDom.Server

        ''' <summary>Créer un graphe de type Courbe simple</summary>
        ''' <param name="nomfichier">Nom du fichier : "test"</param>
        ''' <param name="largeur">largeur de l'image</param>
        ''' <param name="hauteur">hauteur de l'image</param>
        ''' <param name="titre">Titre du graphe</param>
        ''' <remarks></remarks>
        Public Sub grapher_courbe(ByVal nomfichier As String, ByVal titre As String, ByVal largeur As Integer, ByVal hauteur As Integer)
            Try
                Dim legende As Boolean = False 'afficher la légende

                Dim myPane As New GraphPane
                Dim List, List2 As New PointPairList
                Dim datex As Double
                Dim courbe As LineItem

                'Data 1 et 2
                Dim random As New Random
                Dim j As Double = 0
                Dim k As Double = 0
                For i As Integer = 0 To 1000
                    datex = DateAdd(DateInterval.Hour, i, Now).ToOADate()
                    j = j + (random.Next(-9, 10) / 10)
                    k = k + (random.Next(-9, 10) / 10)
                    List.Add(New PointPair(datex, j))
                    List2.Add(New PointPair(datex, k))
                    If (k - j > 10) Then k = k - 2
                    If (k - j < -10) Then k = k + 2
                Next

                'Courbe 1
                courbe = myPane.AddCurve("temperature", List, Color.Blue, SymbolType.None)
                courbe.Line.Width = 1
                courbe.Line.Fill = New Fill(Color.FromArgb(150, 250, 220, 220))

                'Courbe 2
                courbe = myPane.AddCurve("temperature_last", List2, Color.LightBlue, SymbolType.None)
                courbe.Line.Width = 0.5
                courbe.Line.Fill = New Fill(Color.FromArgb(50, 250, 220, 220))

                'Mypane
                myPane.Rect = New RectangleF(0, 0, largeur, hauteur)
                myPane.Title.Text = titre
                myPane.Title.FontSpec.FontColor = Color.DodgerBlue
                myPane.Legend.IsVisible = legende 'on affiche la légende ou non
                myPane.Chart.Fill = New Fill(Color.FromArgb(240, 245, 250), Color.FromArgb(210, 230, 240), -90) 'fond dégradé

                'Axe X
                myPane.XAxis.Type = AxisType.Date
                myPane.XAxis.Scale.Format = "dd-MM-yy HH:mm" '"dd-MMM-yy HH:mm:ss"
                myPane.XAxis.MajorGrid.Color = Color.LightGray
                myPane.XAxis.MajorGrid.PenWidth = 1
                myPane.XAxis.MajorGrid.IsVisible = True
                myPane.XAxis.Scale.MinGrace = 0
                myPane.XAxis.Scale.MaxGrace = 0

                'Axe Y
                myPane.YAxis.MajorGrid.Color = Color.LightGray
                myPane.YAxis.MajorGrid.PenWidth = 1
                myPane.YAxis.MajorGrid.IsVisible = True

                'generation de l'image
                Dim bm As New Bitmap(10, 10)
                Dim g = Graphics.FromImage(bm)
                myPane.AxisChange(g)
                Dim im = myPane.GetImage
                im.Save(_repertoire + nomfichier + ".png", ImageFormat.Png)
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Graphes:graphe_courbe", ex.ToString)
            End Try
        End Sub

        ''' <summary>initialisation de l'objet Graphes</summary>
        ''' <param name="chemin">Chemin ou enregistrer les graphes</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal chemin As String)
            Try
                _repertoire = chemin
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.SERVEUR, "Graphes:New", ex.ToString)
            End Try

        End Sub
    End Class

End Namespace