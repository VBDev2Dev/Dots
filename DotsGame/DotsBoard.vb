Imports System.Drawing
#Region "Board"

Public Class DotsBoard

    Private _moves As New Move.MoveCollection
    Private _BoardSize As Integer
    Private _Boxes As New Box.BoxCollection
    Private _PossibleBoxes As Rectangle()

    Public Sub New(ByVal gridSize As Integer)
        If gridSize <= 3 Then Throw New ArgumentOutOfRangeException("gridsize", "Must be greater than or equal to four.")
        _BoardSize = gridSize
        ReDim _PossibleBoxes(((gridSize - 1) ^ 2) - 1)

    End Sub
    Public Sub Clear()
        _moves.Clear()
        _Boxes.Clear()
    End Sub
    Public ReadOnly Property BoardSize() As Integer
        Get
            Return _BoardSize
        End Get
    End Property
    Public ReadOnly Property BottomRight() As Point
        Get
            Return New Point(_BoardSize - 1, BoardSize - 1)
        End Get
    End Property
    Private plyrs() As Player
    Public Property Players() As Player()
        Get
            Return plyrs
        End Get
        Set(ByVal Value() As Player)
            plyrs = Value
        End Set
    End Property
    Public Function MakeMove(ByVal StartPoint As Point, ByVal EndPoint As Point, ByVal Plyr As Player) As Boolean
        Dim mv As New Move(StartPoint, EndPoint, Plyr)
        If mv.IsValid AndAlso
            mv.StartPoint.Y <= Me.BottomRight.Y AndAlso
            mv.EndPoint.Y <= Me.BottomRight.Y AndAlso mv.StartPoint.X <= Me.BottomRight.X AndAlso
            mv.EndPoint.X <= Me.BottomRight.X AndAlso Not _moves.Contains(mv) Then
            _moves.Add(mv)
            Plyr.Moves.Add(mv)
            IsCompleteBox(mv)

        Else
            RaiseEvent InvalidMove(Me, New InvalidMoveEventArgs(mv))
            Return False
        End If
        Return True
    End Function

    Public Event BoxMade(ByVal sender As Object, ByVal e As BoxMadeEventArgs)
    Public Event InvalidMove(ByVal Sender As Object, ByVal e As InvalidMoveEventArgs)
    Public Event GameOver(ByVal sender As Object, ByVal e As GameOverEventArgs)

    Public Function IsCompleteBox(ByVal mv As Move) As Boolean
        Dim possible As New ArrayList
        For Each rect As Rectangle In GetPossibleBoxes(mv)
            If Not possible.Contains(rect) Then possible.Add(rect)
        Next
        Dim found As Boolean = False
        For Each rect As Rectangle In possible
            Dim mvs As Move.MoveCollection = Move.MoveCollection.FromRectangle(rect)
            Dim complete As Boolean = True
            For Each tmpmv As Move In mvs
                Debug.WriteLine(tmpmv.StartPoint.ToString & "-" & tmpmv.EndPoint.ToString)
                If Not _moves.Contains(tmpmv) Then
                    complete = False
                    Exit For
                End If

            Next
            If complete Then

                Dim moves As Move.MoveCollection = Move.MoveCollection.FromRectangle(rect)
                Dim tmpmoves As New Move.MoveCollection
                For Each tmpmv As Move In Me._moves
                    If moves.Contains(tmpmv) Then
                        tmpmoves.Add(tmpmv)
                    End If
                Next
                Dim bx As Box = New Box(rect, mv.Player, tmpmoves)
                If Not _Boxes.Contains(bx) Then
                    _Boxes.Add(bx)
                    mv.Player.Boxes.Add(bx)

                End If
                RaiseEvent BoxMade(Me, New BoxMadeEventArgs(bx))
                found = True
            End If

        Next
        If _Boxes.Count = Me.GetPossibleBoxes().Length Then
            Dim winner As Player
            Dim pls As New ArrayList
            For Each bx As Box In _Boxes
                If Not pls.Contains(bx.Player) Then pls.Add(bx.Player)
            Next
            winner = pls(0)
            For Each pl As Player In pls
                If pl.Boxes.Count > winner.Boxes.Count Then winner = pl
            Next
            RaiseEvent GameOver(Me, New GameOverEventArgs(winner))
        End If
        Return found
    End Function

    Public Function GetPossibleBoxes(ByVal mv As Move) As Rectangle()
        Dim tmp As New ArrayList

        '(2,3)-(3,3)(3,3)-(3,4)(3,4)-(2,4)
        '(2,3)-(3,3)(3,3)-(3,2)(3,2)-(2,2)
        If mv.MoveType = Move.enmMoveType.Horizontal Then
            If Me.CanMoveDown(mv.StartPoint) Then
                tmp.Add(New Rectangle(mv.StartPoint, New Size(1, 1)))
            End If
            If Me.CanMoveUp(mv.StartPoint) Then
                tmp.Add(New Rectangle(New Point(mv.StartPoint.X, mv.StartPoint.Y - 1), New Size(1, 1)))
            End If
        ElseIf mv.MoveType = Move.enmMoveType.Vertical Then
            If Me.CanMoveRight(mv.StartPoint) Then
                tmp.Add(New Rectangle(mv.StartPoint, New Size(1, 1)))
            End If
            If Me.CanMoveLeft(mv.StartPoint) Then
                tmp.Add(New Rectangle(New Point(mv.StartPoint.X - 1, mv.StartPoint.Y), New Size(1, 1)))
            End If
        End If
        Return tmp.ToArray(GetType(Rectangle))
    End Function
    Friend Function CanMoveUp(ByVal pt As Point) As Boolean
        Return pt.Y > 0
    End Function

    Friend Function CanMoveDown(ByVal pt As Point) As Boolean
        Return pt.Y <= Me.BoardSize - 2
    End Function

    Friend Function CanMoveLeft(ByVal pt As Point) As Boolean
        Return pt.X > 0
    End Function

    Friend Function CanMoveRight(ByVal pt As Point) As Boolean
        Return pt.X <= Me.BoardSize - 2
    End Function

    Public Function GetPossibleBoxes() As Rectangle()

        Dim board(((BoardSize - 1) ^ 2) - 1) As Rectangle
        Dim counter As Integer
        For x As Integer = 0 To BoardSize - 2
            For y As Integer = 0 To BoardSize - 2
                counter += 1
                Debug.WriteLine(String.Format("counter={0} array index={1}", counter, counter - 1))
                Debug.WriteLine(String.Format("x={0} y={1}", x, y))
                Dim tmp As New Rectangle(x, y, 1, 1)
                board(counter - 1) = tmp
                Debug.WriteLine(board(counter - 1))
            Next
        Next
        Return board
    End Function
    Public Function Undo() As Player
        If _moves.Count = 0 Then Return Nothing
        Dim pl As Player = _moves(_moves.Count - 1).Player
        Dim mv As Move = _moves(_moves.Count - 1)
        pl.Moves.Remove(mv)
        Dim removeboxes As New Box.BoxCollection
        For Each bx As Box In _Boxes
            If bx.Moves.Contains(mv) Then removeboxes.Add(bx)
        Next
        For Each bx As Box In removeboxes
            _Boxes.Remove(bx)
            pl.Boxes.Remove(bx)
        Next
        _moves.Remove(mv)

        Return pl
    End Function
    Public ReadOnly Property LastPlayer() As Player
        Get
            If _moves.Count = 0 Then Return Nothing
            Return _moves(_moves.Count - 1).Player
        End Get
    End Property
End Class
#End Region

#Region "Board Helpers"

Public Class Player
    Private _Name As String
    Private _Moves As New Move.MoveCollection
    Private _Boxes As New Box.BoxCollection
    Private _clr As Color

    Public Sub New(ByVal Name As String)
        _Name = Name
    End Sub
    Public ReadOnly Property Moves() As Move.MoveCollection
        Get
            Return _Moves
        End Get
    End Property
    Public ReadOnly Property Boxes() As Box.BoxCollection
        Get
            Return _Boxes
        End Get
    End Property

    Public Property Name() As String
        Get
            Return _Name
        End Get
        Set(ByVal Value As String)
            _Name = Value
        End Set
    End Property
    Shared ReadOnly Property EmptyPlayer() As Player
        Get
            Return New Player("")
        End Get
    End Property

    Public Property Color() As Color
        Get
            Return _clr
        End Get
        Set(ByVal Value As Color)
            _clr = Value
        End Set
    End Property
End Class

Public Class Move
    Public Overrides Function ToString() As String
        If Me.Player.Name <> "" Then Return "Player:" & Me.Player.Name & " From " & Me.StartPoint.ToString & " To " & Me.EndPoint.ToString
        Return "From " & Me.StartPoint.ToString & " To " & Me.EndPoint.ToString
    End Function

    Public Sub New(ByVal StartPoint As Point, ByVal Endpoint As Point, Optional ByVal plyr As Player = Nothing)
        _Start = StartPoint
        _End = Endpoint

        If IsValid() Then
            If _Start.X = _End.X Then
                _MoveType = enmMoveType.Vertical
            Else
                _MoveType = enmMoveType.Horizontal
            End If
        End If
        If plyr Is Nothing Then plyr = DotsGame.Player.EmptyPlayer
        Player = plyr
    End Sub

    Public Function IsValid() As Boolean
        If _Start.X <> _End.X AndAlso _Start.Y <> _End.Y Then
            _MoveType = enmMoveType.Invalid
            Return False
        End If
        If Me.MoveType = enmMoveType.Horizontal Then
            If Me.StartPoint.X > Me.EndPoint.X Then Swap()
        Else
            If Me.StartPoint.Y > Me.EndPoint.Y Then Swap()
        End If
        If _Start.Equals(_End) Then Return False
        Select Case Me.MoveType
            Case enmMoveType.Horizontal
                If _End.X > _Start.X + 1 Then Return False
            Case enmMoveType.Vertical
                If _End.Y > _Start.Y + 1 Then Return False
        End Select
        Return True
    End Function
    Private Sub Swap()
        Dim tmp As Point = New Point(Me.StartPoint.X, Me.StartPoint.Y)
        Me._Start = New Point(Me.EndPoint.X, Me.EndPoint.Y)
        Me._End = New Point(tmp.X, tmp.Y)

    End Sub

    Public ReadOnly Property MoveType() As enmMoveType
        Get
            Return _MoveType
        End Get
    End Property
    Public ReadOnly Property StartPoint() As Point
        Get
            Return _Start
        End Get
    End Property
    Public ReadOnly Property EndPoint() As Point
        Get
            Return _End
        End Get
    End Property
    Public Property Player() As Player
        Get
            Return _Player
        End Get
        Set(ByVal Value As Player)
            _Player = Value
        End Set
    End Property
    Public Enum enmMoveType
        Vertical
        Horizontal
        Invalid
    End Enum
    Private _Start, _End As Point
    Private _MoveType As enmMoveType
    Private _Player As Player

    Public Class MoveCollection
        Inherits System.Collections.CollectionBase

        Public Function Add(ByVal Mv As Move) As Integer
            If Not Mv.IsValid Then Return -1
            Return MyBase.List.Add(Mv)
        End Function
        Public Function Add(ByVal Moves As MoveCollection) As Integer
            Return MyBase.List.Add(Moves)
        End Function
        Public Function Add(ByVal Moves As Move()) As Integer
            Return MyBase.List.Add(Moves)
        End Function
        Public Sub Remove(ByVal mv As Move)
            If MyBase.List.Contains(mv) Then MyBase.List.Remove(mv)
        End Sub

        Default Public ReadOnly Property Item(ByVal Index As Integer) As Move
            Get
                Return MyBase.List.Item(Index)
            End Get

        End Property
        Shared Function FromRectangle(ByVal Rect As Rectangle) As MoveCollection
            If Rect.Height <> 1 OrElse Rect.Width <> 1 Then
                Return Nothing
            End If

            Dim moves As New MoveCollection

            Dim tl As New Point(Rect.Left, Rect.Top)
            Dim first As New Point(tl.X, tl.Y)
            Dim last As New Point(tl.X + 1, tl.Y)

            moves.Add(New Move(New Point(first.X, first.Y), New Point(last.X, last.Y)))

            first = New Point(last.X, last.Y)
            last.Offset(0, 1)

            moves.Add(New Move(New Point(first.X, first.Y), New Point(last.X, last.Y)))

            first = New Point(last.X, last.Y)
            last.Offset(-1, 0)

            moves.Add(New Move(New Point(first.X, first.Y), New Point(last.X, last.Y)))

            first = New Point(last.X, last.Y)
            last.Offset(0, -1)

            moves.Add(New Move(New Point(first.X, first.Y), New Point(last.X, last.Y)))
            Return moves

        End Function
        Public Function Contains(ByVal mv As Move) As Boolean
            For Each tmpMv As Move In Me
                If Point.op_Equality(mv.StartPoint, tmpMv.StartPoint) AndAlso
                    Point.op_Equality(mv.EndPoint, tmpMv.EndPoint) Then Return True
            Next
            Return False
        End Function

    End Class

End Class

Public Class Box
    Private _Rect As Rectangle
    Private _Player As Player
    Private _Moves As Move.MoveCollection
    Friend Sub New(ByVal rect As Rectangle, ByVal player As Player, ByVal moves As Move.MoveCollection)
        _Rect = rect
        _Player = player
        _Moves = moves
    End Sub
    Public ReadOnly Property Player() As Player
        Get
            Return _Player
        End Get
    End Property
    Public ReadOnly Property Box() As Rectangle
        Get
            Return _Rect
        End Get
    End Property
    Public ReadOnly Property Moves() As Move.MoveCollection
        Get
            Return _Moves
        End Get
    End Property

    Public Class BoxCollection
        Inherits System.Collections.CollectionBase

        Public Function Add(ByVal bx As Box) As Integer
            Return MyBase.List.Add(bx)
        End Function
        Public Function Add(ByVal Boxes As BoxCollection) As Integer
            Return MyBase.List.Add(Boxes)
        End Function
        Public Function Add(ByVal Boxes As Box()) As Integer
            Return MyBase.List.Add(Boxes)
        End Function
        Public Sub Remove(ByVal bx As Box)
            If MyBase.List.Contains(bx) Then MyBase.List.Remove(bx)
        End Sub

        'Shared Function FromRectangle(ByVal Rect As Rectangle) As MoveCollection
        '    If Rect.Height <> 1 OrElse Rect.Width <> 1 Then
        '        Return Nothing
        '    End If

        '    Dim moves As New MoveCollection

        '    Dim tl As New Point(Rect.Left, Rect.Top)
        '    Dim first As New Point(tl.X, tl.Y)
        '    Dim last As New Point(tl.X + 1, tl.Y)

        '    moves.Add(New Move(New Point(first.X, first.Y), New Point(last.X, last.Y)))

        '    first = New Point(last.X, last.Y)
        '    last.Offset(0, 1)

        '    moves.Add(New Move(New Point(first.X, first.Y), New Point(last.X, last.Y)))

        '    first = New Point(last.X, last.Y)
        '    last.Offset(-1, 0)

        '    moves.Add(New Move(New Point(first.X, first.Y), New Point(last.X, last.Y)))

        '    first = New Point(last.X, last.Y)
        '    last.Offset(0, -1)

        '    moves.Add(New Move(New Point(first.X, first.Y), New Point(last.X, last.Y)))
        '    Return moves

        'End Function
        Public Function Contains(ByVal bx As Box) As Boolean
            For Each tmpbx As Box In Me
                If tmpbx.Box.Equals(bx.Box) Then Return True
            Next
            Return False
        End Function

    End Class
End Class
#End Region

#Region "EventArgs"
Public Class GameOverEventArgs
    Inherits EventArgs
    Private _Winner As Player
    Public Sub New(ByVal Winner As Player)
        _Winner = Winner
    End Sub
    Public ReadOnly Property Winner() As Player
        Get
            Return _Winner
        End Get
    End Property
End Class

Public Class InvalidMoveEventArgs
    Inherits EventArgs
    Private _Move As Move
    Friend Sub New(ByVal mv As Move)
        _Move = mv
    End Sub
    Public ReadOnly Property Move() As Move
        Get
            Return _Move
        End Get
    End Property
End Class
Public Class BoxMadeEventArgs
    Inherits EventArgs
    Private _box As Box
    Friend Sub New(ByVal box As Box)
        _box = box

    End Sub

    Public ReadOnly Property Moves() As Move.MoveCollection
        Get
            Return _box.Moves
        End Get
    End Property
    Public ReadOnly Property Player() As Player
        Get
            Return _box.Player
        End Get
    End Property
    Public ReadOnly Property BoxArea() As Rectangle
        Get
            Return _box.Box
        End Get
    End Property
End Class
#End Region
