Imports System.Drawing
Module Module1

    Sub Main()

        Dim boardsize As Integer
        Dim inpt As String
        Console.Write("Please enter the size of the board:")
        inpt = Console.ReadLine
        Try
            boardsize = Integer.Parse(inpt)
            Dim board(((boardsize - 1) ^ 2) - 1) As Rectangle
            Dim counter As Integer
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

        Catch ex As Exception
            Console.WriteLine("You did not enter a valid whole number.")

        End Try
        Console.Read()


    End Sub

End Module
