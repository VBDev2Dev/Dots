Imports System.Drawing
Module DotsTester
    Dim WithEvents GBoard As DotsGame.DotsBoard
    Sub Main()
        Dim boardsize As Integer
        Dim inpt As String
        Console.Write("Please enter the size of the board:")
        inpt = Console.ReadLine
        Try

            boardsize = Integer.Parse(inpt)
            Dim board(((boardsize - 1) ^ 2) - 1) As Rectangle
            Dim counter As Integer
            Console.WriteLine("Possible Boxes for a board of " & boardsize.ToString & "X" & boardsize.ToString & " board.")
            For x As Integer = 0 To boardsize - 2
                For y As Integer = 0 To boardsize - 2
                    counter += 1
                    Console.WriteLine(String.Format("counter={0} array index={1}", counter, counter - 1))
                    Console.WriteLine(String.Format("x={0} y={1}", x, y))
                    Dim tmp As New Rectangle(x, y, 1, 1)
                    board(counter - 1) = tmp
                    Console.WriteLine(board(counter - 1))
                Next
            Next

            For x As Integer = 0 To board.Length - 1
                Dim ptTopLeft, ptBottomRight, tmppt As Point
                tmppt = New Point(board(x).x, board(x).Y)
                ptTopLeft = New Point(tmppt.X, tmppt.Y)
                tmppt.Offset(board(x).Width, board(x).Height)
                ptBottomRight = New Point(tmppt.X, tmppt.Y)
                Console.WriteLine(x)
                Console.WriteLine(String.Format("TopLeft:{0} BottomRight:{1}", ptTopLeft, ptBottomRight))
            Next

            GBoard = New DotsGame.DotsBoard(boardsize)

            inpt = "n"



            While (inpt & "").Trim.ToLower <> "q"
                Try
                    MakeMove(GBoard)
                Catch ex As Exception
                    Console.Error.WriteLine(ex.ToString)
                Finally
                    Console.WriteLine("Enter ""q"" to quit.")
                    inpt = Console.ReadLine
                End Try



            End While

        Catch ex As Exception
            Console.Error.WriteLine("Error in running program.")
            Console.Error.WriteLine(ex.ToString)
            Console.Read()

        End Try












    End Sub
    Private Sub MakeMove(ByVal board As DotsGame.DotsBoard)
        Static Player As Integer = 0

        Dim inpt As String

        Console.WriteLine("Please enter the start point of a move?  EX:2,4")
        inpt = Console.ReadLine
        Dim delim As String = ","
        Dim values() As String = inpt.Split(delim.ToCharArray, 2)
        Dim pt, pt2 As Point
        pt = New Point(Integer.Parse(values(0)), Integer.Parse(values(1)))

        Console.WriteLine("Please enter the end point of a move?  EX:2,4")
        inpt = Console.ReadLine
        values = inpt.Split(delim.ToCharArray, 2)
        pt2 = New Point(Integer.Parse(values(0)), Integer.Parse(values(1)))
        Dim p1, p2 As DotsGame.Player
        p1 = New DotsGame.Player("Player 1")
        p2 = New DotsGame.Player("Player 2")


        If Player = 0 Then
            If board.MakeMove(pt, pt2, p1) Then Player = 1
        Else
            If board.MakeMove(pt, pt2, p2) Then Player = 0

        End If
        Dim mv As DotsGame.Move = New DotsGame.Move(pt, pt2)
        PrintPossibleFromMove(mv)
        


    End Sub

    Private Sub GBoard_BoxMade(ByVal sender As Object, ByVal e As DotsGame.BoxMadeEventargs) Handles GBoard.BoxMade
        Console.WriteLine("Box made by " & e.Player.Name & "!!!!!!")
        For Each mv As DotsGame.Move In e.Moves
            Console.WriteLine(mv.ToString)
        Next
        Console.WriteLine(e.BoxArea)
        Console.WriteLine()

    End Sub

    Private Sub GBoard_InvalidMove(ByVal Sender As Object, ByVal e As DotsGame.InvalidMoveEventArgs) Handles GBoard.InvalidMove
        Console.WriteLine("Invalid Move.  You must move horizontaly or verticaly 1 space.")

    End Sub
    Private Sub PrintPossibleFromMove(ByVal mv As DotsGame.Move)
        Console.WriteLine("Possible boxes for the " & mv.ToString & " move.")
        For Each rect As Rectangle In GBoard.GetPossibleBoxes(mv)
            Console.WriteLine(rect)
            For Each tmp As DotsGame.Move In DotsGame.Move.MoveCollection.FromRectangle(rect)
                Console.WriteLine(tmp)
            Next
        Next
        Console.WriteLine()
    End Sub
End Module
