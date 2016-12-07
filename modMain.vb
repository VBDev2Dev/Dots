Module modMain
    Public Sub Main()
        Dim game As New frmBoard
        Try
            Application.Run(game)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    
End Module
