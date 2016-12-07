Public Class BoardUI
    Private WithEvents Board As DotsBoard
    Private plyrs() As Player
    Private plr As Integer
    Private stp, endp As Point
    Private lgndTop As Integer


    Private _status As enmStatus = enmStatus.Invalid
    Public Enum enmStatus
        StartPoint = 0
        EndPoint = 1
        Invalid = -1
    End Enum
    Public Property Status() As enmStatus
        Get
            Return _status
        End Get
        Set(ByVal Value As enmStatus)
            _status = Value
        End Set
    End Property
    Public ReadOnly Property PictureSize() As Size
        Get
            Dim p As Point = ScaleBoardPoint(New Point(Board.BoardSize + 2, Board.BoardSize + 1))
            Return New Size(p)
        End Get
    End Property

    Public Property LegendTop() As Integer
        Get
            Return lgndTop
        End Get
        Set(ByVal Value As Integer)
            lgndTop = Value
        End Set
    End Property
    Private pScale As Integer = 40
    Private rectScale As Integer = 4

    Public Property BoxSize() As Integer
        Get
            Return pScale
        End Get
        Set(ByVal Value As Integer)
            pScale = Value
        End Set
    End Property
    Public Property DotSize() As Integer
        Get
            Return rectScale
        End Get
        Set(ByVal Value As Integer)
            rectScale = Value
        End Set
    End Property
    Public Function ScaleBoardPoint(ByVal BoardPoint As Point) As Point
        Return New Point((BoardPoint.X * pScale) + pScale, (BoardPoint.Y * pScale) + pScale)
    End Function
    Public Function BoardPointFromPoint(ByVal pnt As Point) As Point
        Dim x, y As Integer
        x = pnt.X
        y = pnt.Y
        x = x - pScale
        If x <> 0 Then x = x / pScale
        y = y - pScale
        If y <> 0 Then y = y / pScale
        Return New Point(x, y)

    End Function
    Public Sub PaintBoard(ByVal g As Graphics)
        For x As Integer = 0 To Board.BoardSize - 1
            For y As Integer = 0 To Board.BoardSize - 1
                Dim pnt As Point = ScaleBoardPoint(New Point(x, y))
                g.DrawRectangle(Pens.Black, pnt.X, pnt.Y, rectScale, rectScale)
                g.FillRectangle(Brushes.Black, pnt.X, pnt.Y, rectScale, rectScale)

            Next
        Next
        PaintLegend(g)
        For Each pl As Player In Board.Players
            PaintMoves(g, pl.Moves)
            PaintBoxes(g, pl.Boxes)
        Next
        ' sbpBoxes.Text = plyrs(plr).Boxes.Count.ToString

    End Sub
    Public Sub PaintLegend(ByVal g As Graphics)
        Dim font As New Font("Tahoma", 8S)
        For x As Integer = 0 To Board.BoardSize - 1

            Dim p As Point = ScaleBoardPoint(New Point(x, 0))
            p.Y = lgndTop
            p.Offset(-rectScale / 2, 0)
            Dim sz As SizeF = g.MeasureString(x.ToString, font)
            'g.DrawRectangle(Pens.Red, p.X, p.Y, sz.Width, sz.Height)
            g.DrawString(x.ToString, font, Brushes.Black, p.X, p.Y)

        Next
        For x As Integer = 1 To Board.BoardSize - 1

            Dim p As Point = ScaleBoardPoint(New Point(0, x))
            Dim sz As SizeF = g.MeasureString(x.ToString, font)
            p.Offset(-pScale / 2, -sz.Height / 2)

            'g.DrawRectangle(Pens.Red, p.X, p.Y, sz.Width, sz.Height)
            g.DrawString(x.ToString, font, Brushes.Black, p.X, p.Y)

        Next
    End Sub
    Private Sub PaintMoves(ByVal g As Graphics, ByVal moves As Move.MoveCollection)
        For Each mv As Move In moves

            Dim stp, endp As Point
            stp = ScaleBoardPoint(mv.StartPoint)
            endp = ScaleBoardPoint(mv.EndPoint)
            Select Case mv.MoveType
                Case DotsGame.Move.enmMoveType.Horizontal
                    stp.Offset(0, 1)
                    endp.Offset(0, 1)
                Case DotsGame.Move.enmMoveType.Vertical
                    stp.Offset(1, 0)
                    endp.Offset(1, 0)
            End Select
            Dim p As New Pen(mv.Player.Color)
            g.DrawLine(p, stp, endp)
            Select Case mv.MoveType
                Case DotsGame.Move.enmMoveType.Horizontal
                    stp.Offset(0, 1)
                    endp.Offset(0, 1)
                Case DotsGame.Move.enmMoveType.Vertical
                    stp.Offset(1, 0)
                    endp.Offset(1, 0)
            End Select

            g.DrawLine(p, stp, endp)
            p.Dispose()
        Next
    End Sub

    Private Sub PaintBoxes(ByVal g As Graphics, ByVal Boxes As Box.BoxCollection)
        Dim fnt As New Font("Tahoma", 8S)
        For Each bx As Box In Boxes
            Dim brsh As New SolidBrush(bx.Player.Color)
            Dim pnt As New Point(bx.Box.X, bx.Box.Y)
            pnt = ScaleBoardPoint(pnt)
            'pnt.Offset(rectScale + 1, rectScale + 1)

            Dim sz As SizeF = g.MeasureString(bx.Player.Name.Substring(bx.Player.Name.Length - 1), fnt)
            Dim rect As New RectangleF(pnt.X, pnt.Y, pScale, pScale)
            'g.DrawRectangle(Pens.Cyan, Rectangle.Ceiling(rect))
            Dim frmt As New StringFormat
            frmt.Alignment = StringAlignment.Center
            frmt.LineAlignment = StringAlignment.Center
            g.DrawString(bx.Player.Name, fnt, brsh, rect, frmt)
            brsh.Dispose()
        Next
        fnt.Dispose()

    End Sub
    Private bSize As Integer
    Public Sub New(ByVal Board As DotsBoard)
        Me.Board = Board
    End Sub
End Class
