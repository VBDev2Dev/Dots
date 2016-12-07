Imports DotsGame
Public Class frmBoard
    Inherits System.Windows.Forms.Form
    Private WithEvents Board As DotsBoard
    Private stp, endp As Point
    Private plr As Integer
    Private UI As BoardUI

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        Me.SetStyle(ControlStyles.DoubleBuffer, True)
        Me.SetStyle(ControlStyles.ResizeRedraw, True)
        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents sbpName As System.Windows.Forms.StatusBarPanel
    Friend WithEvents sbpMove As System.Windows.Forms.StatusBarPanel
    Friend WithEvents sbpBoxes As System.Windows.Forms.StatusBarPanel
    Friend WithEvents sbar As System.Windows.Forms.StatusBar
    Friend WithEvents lnkColors As System.Windows.Forms.LinkLabel
    Friend WithEvents lnkClear As System.Windows.Forms.LinkLabel
    Friend WithEvents lnkUndo As System.Windows.Forms.LinkLabel
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(frmBoard))
        Me.sbar = New System.Windows.Forms.StatusBar
        Me.sbpName = New System.Windows.Forms.StatusBarPanel
        Me.sbpMove = New System.Windows.Forms.StatusBarPanel
        Me.sbpBoxes = New System.Windows.Forms.StatusBarPanel
        Me.lnkColors = New System.Windows.Forms.LinkLabel
        Me.lnkClear = New System.Windows.Forms.LinkLabel
        Me.lnkUndo = New System.Windows.Forms.LinkLabel
        CType(Me.sbpName, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.sbpMove, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.sbpBoxes, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'sbar
        '
        Me.sbar.Location = New System.Drawing.Point(0, 244)
        Me.sbar.Name = "sbar"
        Me.sbar.Panels.AddRange(New System.Windows.Forms.StatusBarPanel() {Me.sbpName, Me.sbpMove, Me.sbpBoxes})
        Me.sbar.ShowPanels = True
        Me.sbar.Size = New System.Drawing.Size(292, 22)
        Me.sbar.TabIndex = 0
        Me.sbar.Text = "StatusBar1"
        '
        'sbpName
        '
        Me.sbpName.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents
        Me.sbpName.Width = 10
        '
        'sbpMove
        '
        Me.sbpMove.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents
        Me.sbpMove.Width = 10
        '
        'sbpBoxes
        '
        Me.sbpBoxes.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring
        Me.sbpBoxes.Width = 256
        '
        'lnkColors
        '
        Me.lnkColors.AutoSize = True
        Me.lnkColors.Location = New System.Drawing.Point(0, 8)
        Me.lnkColors.Name = "lnkColors"
        Me.lnkColors.Size = New System.Drawing.Size(97, 16)
        Me.lnkColors.TabIndex = 1
        Me.lnkColors.TabStop = True
        Me.lnkColors.Text = "Randomize Colors"
        '
        'lnkClear
        '
        Me.lnkClear.AutoSize = True
        Me.lnkClear.Location = New System.Drawing.Point(112, 8)
        Me.lnkClear.Name = "lnkClear"
        Me.lnkClear.Size = New System.Drawing.Size(65, 16)
        Me.lnkClear.TabIndex = 2
        Me.lnkClear.TabStop = True
        Me.lnkClear.Text = "Clear Board"
        '
        'lnkUndo
        '
        Me.lnkUndo.AutoSize = True
        Me.lnkUndo.Location = New System.Drawing.Point(192, 8)
        Me.lnkUndo.Name = "lnkUndo"
        Me.lnkUndo.Size = New System.Drawing.Size(31, 16)
        Me.lnkUndo.TabIndex = 2
        Me.lnkUndo.TabStop = True
        Me.lnkUndo.Text = "Undo"
        '
        'frmBoard
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(292, 266)
        Me.Controls.Add(Me.lnkClear)
        Me.Controls.Add(Me.lnkColors)
        Me.Controls.Add(Me.sbar)
        Me.Controls.Add(Me.lnkUndo)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmBoard"
        Me.Text = "Dots"
        CType(Me.sbpName, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.sbpMove, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.sbpBoxes, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

#End Region
    Private Const pScale As Integer = 40
    Private Const rectScale As Integer = 4
    
    


    Private Sub frmBoard_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles MyBase.Paint
        UI.PaintBoard(e.Graphics)
        If Control.MouseButtons = Windows.Forms.MouseButtons.Left And UI.Status = BoardUI.enmStatus.StartPoint Then
            Dim pn As Pen = New Pen(Board.Players(plr).Color)
            With e.Graphics
                .DrawLine(pn, UI.ScaleBoardPoint(stp), Me.PointToClient(Control.MousePosition))
                Dim en, st As Point
                en = Me.PointToClient(Control.MousePosition)
                en.Offset(rectScale, rectScale)
                st = New Point(UI.ScaleBoardPoint(stp).X, UI.ScaleBoardPoint(stp).Y)
                st.Offset(rectScale, rectScale)
                .DrawLine(pn, st, en)
                pn.Dispose()
            End With
        End If
    End Sub

    Private Sub frmBoard_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Location = New Point(0, 0)
        Dim sz As String = InputBox("Please enter the size of the board?  Must be 5 or greater.", "Size of Board", "6")
        Try
            Dim bsz As Integer = Integer.Parse(sz)
            Board = New DotsBoard(bsz)
            UI = New BoardUI(Board)
            UI.LegendTop = Me.lnkColors.Top + lnkClear.Height
            Me.Size = New Size(Me.UI.ScaleBoardPoint(New Point(bsz + 2, bsz + 1)))
            Dim prs As String = InputBox("How many players?  Must be 2 or more.", "Number of Players", "2")
            Dim plyrs(Integer.Parse(prs) - 1) As Player

            For x As Integer = 0 To plyrs.Length - 1
                plyrs(x) = New Player(InputBox("Please enter Player " & x + 1.ToString & "'s name?", "Player " & x + 1.ToString & "'s Name", "Player " & x + 1.ToString))
            Next
            Board.Players = plyrs
            SetColors()
            plr = 0
            UpdateStatus()

        Catch ex As FormatException
            MsgBox("You must enter a number in this box.")
            Me.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
            Me.Close()

        End Try
    End Sub
    Private Sub SetColors()
        Dim g As Graphics = Graphics.FromHwnd(Me.Handle)
        For x As Integer = 0 To Board.Players.Length - 1
            Randomize()
            Dim r, gr, b As Integer
            r = CInt(Int((255 + 1) * Rnd()))
            gr = CInt(Int((255 + 1) * Rnd()))
            b = CInt(Int((255 + 1) * Rnd()))
            Board.Players(x).Color = Color.FromArgb(r, gr, b)
            Board.Players(x).Color = g.GetNearestColor(Board.Players(x).Color)
            Debug.WriteLine(Board.Players(x).Color.Name)
        Next
        g.Dispose()
        Me.Invalidate()
    End Sub

    Private Sub frmBoard_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseDown
        Dim pnt As Point = UI.BoardPointFromPoint(New Point(e.X, e.Y))
        If Math.Sign(pnt.X) = -1 OrElse pnt.X >= Board.BoardSize OrElse Math.Sign(pnt.Y) = -1 OrElse pnt.Y >= Board.BoardSize Then
            stp = New Point(-1, -1)
            endp = New Point(-1, -1)
            UI.Status = BoardUI.enmStatus.Invalid
            sbpMove.Text = "Invalid Move"

        Else
            endp = New Point(-1, -1)
            stp = pnt
            sbpMove.Text = stp.ToString
            UI.Status = BoardUI.enmStatus.StartPoint
        End If
    End Sub
    Private Sub frmBoard_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseUp
        If UI.Status = BoardUI.enmStatus.StartPoint Then
            Dim pnt As Point = UI.BoardPointFromPoint(New Point(e.X, e.Y))

            If Math.Sign(pnt.X) = -1 OrElse pnt.X >= Board.BoardSize OrElse Math.Sign(pnt.Y) = -1 OrElse pnt.Y >= Board.BoardSize Then
                stp = New Point(-1, -1)
                endp = New Point(-1, -1)
                UI.Status = BoardUI.enmStatus.Invalid
                sbpMove.Text = "Invalid Move"

            Else
                endp = pnt
                UI.Status = BoardUI.enmStatus.EndPoint
                playAgain = False
                If Board.MakeMove(stp, endp, Board.Players(plr)) Then
                    plr += 1
                    If plr = Board.Players.Length Then plr = 0
                    UpdateStatus()
                End If
            End If
        End If
        Me.Invalidate()
    End Sub

    Dim playAgain As Boolean
    Private Sub Board_BoxMade(ByVal sender As Object, ByVal e As DotsGame.BoxMadeEventArgs) Handles Board.BoxMade
        ' MsgBox("Box made by " & e.Player.Name & ".")
        If playAgain = False Then
            plr -= 1

            playAgain = True
        End If

    End Sub

    Private Sub frmBoard_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseMove
        If e.Button = Windows.Forms.MouseButtons.Left And UI.Status = BoardUI.enmStatus.StartPoint Then
            Me.Invalidate()


        End If
    End Sub

    Private Sub UpdateStatus()
        If Not Board.LastPlayer Is Nothing Then
            sbpMove.Text = Board.LastPlayer.Moves(Board.LastPlayer.Moves.Count - 1).ToString
            If UI.Status = BoardUI.enmStatus.Invalid Then sbpMove.Text = "Invalid move."
        Else
            sbpMove.Text = ""
        End If
        If UI.Status = BoardUI.enmStatus.StartPoint Then sbpMove.Text = stp.ToString
        sbpBoxes.Text = Board.Players(plr).Boxes.Count.ToString
        sbpName.Text = Board.Players(plr).Name

    End Sub


    Private Sub Board_GameOver(ByVal sender As Object, ByVal e As GameOverEventArgs) Handles Board.GameOver
        MsgBox("Game over!  " & e.Winner.Name & " has " & e.Winner.Boxes.Count.ToString & " boxes.")

    End Sub

    Private Sub lnkColors_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lnkColors.LinkClicked
        SetColors()
    End Sub

    Private Sub lnkClear_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lnkClear.LinkClicked
        Board.Clear()
        plr = 0
        For Each p As Player In Board.Players
            p.Moves.Clear()
            p.Boxes.Clear()
        Next
        sbpMove.Text = ""
        UpdateStatus()
        Me.Invalidate()
    End Sub

    Private Sub lnkUndo_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lnkUndo.LinkClicked
        Dim pl As Player = Board.Undo
        Dim tmp As Integer = plr
        plr = -1
        For x As Integer = 0 To Board.Players.Length - 1
            If Board.Players(x) Is pl Then
                plr = x
                Exit For
            End If
        Next
        If plr = -1 Then plr = tmp
        UpdateStatus()
        Me.Invalidate()

    End Sub
End Class
