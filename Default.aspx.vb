Imports GruntButton
Imports System.Media
Imports System.IO
Imports System.Data
Imports System.Data.SqlClient

Partial Class _Default
    Inherits System.Web.UI.Page

    Dim btns As ArrayList
    Dim grunts As ArrayList

    Dim soundsDir As String = ""

    Dim oConn As New SqlConnection(ConfigurationManager.ConnectionStrings.Item("HighScoresConnectionString").ConnectionString)
    Dim oCmd As New SqlCommand
    Dim oDA As New SqlDataAdapter
    Dim odTbl As New DataTable
    Dim strSQL As String = ""
    Dim oParam As New SqlParameter

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If IsPostBack Then

            If Session("FromBoard") = True Then

                Dim btns_tbl As Table = CType(Session("BoardTable"), Table)

                btns_tbl.Rows.Clear()

                board_pnl.Controls.Add(btns_tbl)

                Dim btns As ArrayList = CType(Session("Buttons"), ArrayList)
                Dim numRows As Integer = Math.Sqrt(btns.Count)

                Dim row As TableRow = Nothing
                Dim cell As TableCell = Nothing

                Dim i As Integer = 0

                While i < btns.Count

                    For j As Integer = 0 To numRows - 1

                        row = New TableRow

                        For k As Integer = 0 To numRows - 1
                            cell = New TableCell()
                            cell.Width = 600 / numRows

                            cell.Controls.Add(CType(btns(i), GruntButton))

                            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(btns(i))
                            AddHandler CType(btns(i), GruntButton).Click, AddressOf btnClick

                            row.Cells.Add(cell)
                            i += 1

                        Next

                        btns_tbl.Rows.Add(row)
                    Next

                End While

            End If
        Else
            Session("FromBoard") = Nothing
        End If

    End Sub


    Protected Sub generate_btn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles generate_btn.Click

        Session("FromBoard") = Nothing

        options_pnl.visible = False
        status_pnl.Visible = True

        generateBoard()

        status_lit.Text = "<i>Pummel the grunts!</i>"
        level_lit.Text = "<b>Level " & Session("LevelCounter") & "</b>"
        countdown_lit.Text = "<span class='CountdownStyle'>:" & Session("SecondCounter") & "</span>"

        level_timer.Enabled = True
        Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "level_timer", "startTimer();", True)

        inner_timer.Enabled = True
        Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "inner_timer", "startInnerTimer();", True)

    End Sub

    ' Create the entire game board
    Protected Sub generateBoard()
        Dim btns_tbl As Table = New Table
        btns_tbl.ID = "btns_tbl"
        btns_tbl.Width = 600

        board_pnl.Enabled = True
        board_pnl.Controls.Add(btns_tbl)
        Session("BoardTable") = btns_tbl

        Dim numButtons As Integer = CInt(difficulty_ddl.SelectedValue)

        Dim numRows As Integer = Math.Sqrt(numButtons)

        Dim row As TableRow = Nothing
        Dim cell As TableCell = Nothing

        btns = New ArrayList(numButtons)
        grunts = New ArrayList()

        Dim btn As GruntButton = Nothing
        Dim i As Integer = 0

        While i < numButtons

            For j As Integer = 0 To numRows - 1

                row = New TableRow

                For k As Integer = 0 To numRows - 1
                    cell = New TableCell()
                    cell.Width = 600 / numRows

                    btn = New GruntButton()

                    With btn
                        .ID = "btn_" & i + 1
                        .Width = (600 / numRows) - 3
                        .Height = (600 / numRows) - 3
                    End With

                    cell.Controls.Add(btn)

                    btns.Add(btn)

                    ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(btn)

                    AddHandler btn.Click, AddressOf btnClick

                    row.Cells.Add(cell)
                    i += 1
                Next

                btns_tbl.Rows.Add(row)
            Next

        End While

        soundsDir = Page.MapPath(".") & "\sounds\"
        Session("SoundsDir") = soundsDir

        'Create arrays of sounds from which to pull data
        Session("HitSounds") = configSounds("hits\")
        Session("SleepDefeatSounds") = configSounds("sleepDefeat\")

        firstConfigGrunts()

        Session("Score") = 0
        Session("TimeRemaining") = 0
        Session("LevelGrunts") = 0
        Session("TotalGrunts") = 0
        Session("LevelCounter") = 1

        Session("Buttons") = btns
        Session("Grunts") = grunts
        Session("TotalMissedGrunts") = grunts.Count
        Session("SecondCounter") = 15

        Session("FromBoard") = True

    End Sub

    Protected Function configSounds(ByVal path As String) As ArrayList

        Dim soundFiles() As String = Directory.GetFiles(soundsDir & path, "*")
        Dim sounds As ArrayList = New ArrayList

        For i As Integer = 0 To soundFiles.Length - 1
            sounds.Add(soundFiles(i))
        Next

        Return sounds

    End Function


    Protected Sub btnClick(ByVal sender As Object, ByVal e As EventArgs)

        Dim clickedBtn As GruntButton = CType(sender, GruntButton)
        Dim gruntName As String = ""
        Dim innerStr As String = ""

        Dim soundPlayer As SoundPlayer = New SoundPlayer()
        Dim randSound As String = ""



        For i As Integer = 0 To CType(Session("Grunts"), ArrayList).Count - 1
            If clickedBtn.ID = CType(Session("Grunts"), ArrayList)(i).ID Then
                gruntName = "Grunt " & i + 1
                Exit For
            End If
        Next

        If clickedBtn.IsGrunt Then

            If clickedBtn.IsLeftOverGrunt Then

                clickedBtn.HitCounter += 1

                If clickedBtn.HitCounter = 3 Then
                    CType(Session("Grunts"), ArrayList).Remove(clickedBtn)
                    clickedBtn.IsLeftOverGrunt = False
                    clickedBtn.IsGrunt = False

                    innerStr = "<span class='SuccessStyle'>" & gruntName & " pummelled!</span> "

                    soundPlayer.SoundLocation = Session("SoundsDir") & "\final.wav"
                    soundPlayer.Play()

                    Session("Score") += 10
                    Session("LevelGrunts") += 1
                    Session("TotalGrunts") += 1
                    Session("TotalMissedGrunts") -= 1
                Else

                    innerStr = "<span class='PummellingStyle'>Pummelling in progress (" & (3 - clickedBtn.HitCounter).ToString & ")</span>"

                    randSound = CType(Session("HitSounds"), ArrayList)(New Random().Next(0, CType(Session("HitSounds"), ArrayList).Count))
                    soundPlayer.SoundLocation = randSound
                    soundPlayer.Play()

                End If

            Else

                innerStr = "<span class='SuccessStyle'>" & gruntName & " pummelled!</span> "

                CType(Session("Grunts"), ArrayList).Remove(clickedBtn)
                clickedBtn.IsGrunt = False

                randSound = CType(Session("SleepDefeatSounds"), ArrayList)(New Random().Next(0, CType(Session("SleepDefeatSounds"), ArrayList).Count))
                soundPlayer.SoundLocation = randSound
                soundPlayer.Play()

                Session("Score") += 10
                Session("LevelGrunts") += 1
                Session("TotalGrunts") += 1
                Session("TotalMissedGrunts") -= 1

            End If

            ' Handle level advance
            If CType(Session("Grunts"), ArrayList).Count = 0 Then

                Session("TimeRemaining") += Session("SecondCounter") * 10

                Session("LevelCounter") += 1
                Session("LevelGrunts") = 0

                inner_timer.Enabled = False
                Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "inner_timer", "stopInnerTimer();", True)

                level_timer.Enabled = False
                Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "level_timer", "stopLevelTimer();", True)

                'If all grunts are pummelled by the last level
                If Session("LevelCounter") = 6 Then

                    Dim scores As DataTable = currentHighScores()

                    Session("FinalScore") = Session("Score") + Session("TimeRemaining")

                    board_pnl.Enabled = False
                    level_lit.Text = "<b><i>GAME OVER</i></b>"
                    status_lit.Text = "Total pummelled: <span class='SuccessStyle'>" & Session("TotalGrunts") & "</span><br /><span class='SuccessStyle'>No grunts missed! You win!</span>"

                    result_lit.Text = "<span class='ScoreStyle'>Congratulations! You beat the game!</span><br /><br />Total grunts pummelled: <span class='ResultSuccessStyle'>" & Session("TotalGrunts") & "</span><br /><br />Score: <span class='ResultSuccessStyle'>" & Session("Score") & "</span><br />+ <span class='ResultSuccessStyle'>" & Session("TimeRemaining") & "</span> (total time bonuses)<br /><span class='ScoreStyle'>= <span style='color:#8ee221;'>" & Session("FinalScore") & "</span></span>"


                    If Session("FinalScore") >= CInt(scores.Rows(scores.Rows.Count - 1)("Score")) Then
                        highScore_pnl.Visible = True
                    End If

                    result_updatePnl.Update()
                    result_mpExt.Show()
                Else

                    stop_timer.Enabled = True
                    Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "stop_timer", "startStopTimer();", True)

                End If

            End If


        Else
            Session("Score") -= 10
            innerStr = "<span class='FailStyle'>Uh oh. That's not a grunt.</span> "
        End If

        level_lit.Text = IIf(CType(Session("Grunts"), ArrayList).Count = 0, "<span class='SuccessStyle'>Level " & IIf(CType(Session("Grunts"), ArrayList).Count = 0, Session("LevelCounter") - 1, Session("LevelCounter")) & " beaten!</span><br />Bonus: <span class='SuccessStyle'>+" & Session("SecondCounter") * 10, "</span><b>Level " & Session("LevelCounter") & "</b><br />Grunts Pummelled: <b>" & Session("LevelGrunts") & "</b>")
        status_lit.Text = innerStr & "<br />Total pummelled: <span class='SuccessStyle'>" & Session("TotalGrunts") & "</span><br />"

        board_updatePnl.Update()
        optionsStatus_updatePnl.Update()

        Session("FromBoard") = True
    End Sub

    Protected Sub resetBoard(ByVal sender As Object, ByVal e As System.EventArgs)

        Session.Clear()

        inner_timer.Enabled = False
        Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "inner_timer", "stopInnerTimer();", True)

        level_timer.Enabled = False
        Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "level_timer", "stopLevelTimer();", True)

        result_mpExt.Hide()

        status_pnl.Visible = False
        options_pnl.Visible = True

        board_pnl.Controls.Clear()

        board_updatePnl.Update()
        optionsStatus_updatePnl.Update()

    End Sub


    Protected Sub firstConfigGrunts()

        Dim rand As New Random

        For Each gb As GruntButton In btns

            If rand.Next(0, 4) = 0 Then
                gb.IsGrunt = True
                gb.HitCounter = 0
                grunts.Add(gb)
            End If
        Next

        board_updatePnl.Update()
    End Sub


    Protected Sub level_timer_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles level_timer.Tick
        Session("LevelCounter") += 1
        Session("LevelGrunts") = 0

        If Session("LevelCounter") = 6 Then

            Dim scores = currentHighScores()

            level_timer.Enabled = False
            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "level_timer", "stopTimer();", True)

            inner_timer.Enabled = False
            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "inner_timer", "stopInnerTimer();", True)

            board_pnl.Enabled = False

            Session("FinalScore") = Session("Score")

            level_lit.Text = "<b><i>GAME OVER</i></b>"
            status_lit.Text = "Total pummelled: <span class='SuccessStyle'>" & Session("TotalGrunts") & "</span><br />" & IIf(Session("TotalMissedGrunts") = 0, "<span class='SuccessStyle'>No grunts missed! You win!</span>", "Total missed: <span class='FailStyle'>" & Session("TotalMissedGrunts") & "</span>")

            result_lit.Text = "<span class='ScoreStyle'>GAME OVER</span><br /><br />" & IIf(Session("TotalMissedGrunts") = 0, "<i>Congratulations! You're a grunt master!</i>", "Grunts missed: <span class='ResultMissedStyle'>" & Session("TotalMissedGrunts") & "</span>") & "<br />Total Grunts pummelled: <span class='ResultSuccessStyle'>" & Session("TotalGrunts") & "</span><br /><br /><span class='ScoreStyle'>Score: <span style='color:#8ee221;'>" & Session("FinalScore") & "</span></span>"

            If Session("FinalScore") >= CInt(scores.Rows(scores.Rows.Count - 1)("Score")) Then
                highScore_pnl.Visible = True
            End If

            result_mpExt.Show()


        Else
            level_lit.Text = "<b>Level " & Session("LevelCounter") & "</b><br />Grunts Pummelled: <b>" & Session("LevelGrunts") & "</b>"
            status_lit.Text = "Total pummelled: <span class='SuccessStyle'>" & Session("TotalGrunts") & "</span>"

            Session("SecondCounter") = 15
            inner_timer.Enabled = True
            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "inner_timer", "startInnerTimer();", True)

            countdown_lit.Text = "<span class='CountdownStyle'>:" & Session("SecondCounter") & "</span>"


            'Set all missed grunts to "Left over" status (aka - "awake"). From the remaining non-grunt buttons, add another set 
            'of grunts
            Dim rand As New Random()
            For Each gb As GruntButton In CType(Session("Buttons"), ArrayList)

                If gb.IsGrunt And Not gb.IsLeftOverGrunt Then
                    gb.IsLeftOverGrunt = True
                ElseIf gb.IsGrunt = False And gb.IsLeftOverGrunt = False Then
                    If rand.Next(0, 4) = 0 Then
                        gb.IsGrunt = True
                        CType(Session("Grunts"), ArrayList).Add(gb)
                        Session("TotalMissedGrunts") += 1
                    End If
                End If
            Next

            board_updatePnl.Update()
        End If

        optionsStatus_updatePnl.Update()
    End Sub

    Protected Sub instructions_btn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles instructions_btn.Click
        Dim instructionsStr As String = "<h4>Welcome to Sleeper Smash</h4>" & _
                                        "<p><img src='sleepingGrunt.jpg' class='InlineImage' />This game consists of five levels or ""waves"", during each of which you must pummel as many sleeping grunts as possible before the next wave starts, spawning an additional set of grunts.</p> " & _
                                        "<p>If you pummel all of the grunts on the board within a wave, you beat the level. </p>" & _
                                        "<p>You must be as fast as possible, as any remaining undefeated grunts at the start of a new wave will wake up, and become tougher to defeat. " & _
                                        "(A woken grunt will require three hits to defeat, versus one hit for a sleeping grunt.) </p>" & _
                                        "<p><i>I created this game as a study in dynamic control generation and retaining data over postbacks.</i></p>"

        instructions_lit.Text = instructionsStr

        instructions_mpExt.Show()

    End Sub


    Protected Sub inner_timer_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles inner_timer.Tick

        Session("SecondCounter") -= 1

        If Session("SecondCounter") = 0 Then

            inner_timer.Enabled = False
            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "inner_timer", "stopInnerTimer();", True)

        Else
            countdown_lit.Text = "<span class='CountdownStyle'>:" & IIf(Session("SecondCounter") <= 9, "0", "") & Session("SecondCounter") & "</span>"
        End If

    End Sub


    Protected Sub close_lBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles close_lBtn.Click
        instructions_mpExt.Hide()
    End Sub

    Protected Sub close_lBtn2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles close_lBtn2.Click
        result_mpExt.Hide()

    End Sub


    Protected Sub stop_timer_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles stop_timer.Tick

        Session("SecondCounter") = 15

        Dim rand As New Random()
        For Each gb As GruntButton In CType(Session("Buttons"), ArrayList)
            If rand.Next(0, 4) = 0 Then
                gb.IsGrunt = True
                CType(Session("Grunts"), ArrayList).Add(gb)
                Session("TotalMissedGrunts") += 1
            End If
        Next

        'Session("TotalMissedGrunts") = Session("TotalMissedGrunts") + CType(Session("Grunts"), ArrayList).Count

        countdown_lit.Text = "<span class='CountdownStyle'>:" & Session("SecondCounter") & "</span>"

        board_updatePnl.Update()
        optionsStatus_updatePnl.Update()

        inner_timer.Enabled = True
        Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "inner_timer", "startInnerTimer();", True)

        level_timer.Enabled = True
        Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "level_timer", "startLevelTimer();", True)

        stop_timer.Enabled = False
        Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "stop_timer", "stopStopTimer();", True)
    End Sub

    Protected Function currentHighScores() As DataTable

        Try
            oCmd.Connection = oConn
            oCmd.CommandType = CommandType.Text

            odTbl.Clear()

            strSQL = "SELECT *, DifficultyLevel FROM HighScores INNER JOIN Difficulties ON HighScores.DifficultyID = Difficulties.ID ORDER BY Score DESC"

            oCmd.CommandText = strSQL

            oDA.SelectCommand = oCmd
            oDA.Fill(odTbl)

            Return odTbl

        Catch ex As Exception
            Throw ex
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If

            oCmd.Dispose()
            oDA.Dispose()
        End Try


    End Function

    Protected Sub addNewHighScore(ByVal replace As Boolean)

        Try

            oCmd.Connection = oConn
            oCmd.CommandType = CommandType.Text

            oCmd.Parameters.Clear()
            odTbl.Clear()

            If replace Then

                strSQL = "DELETE FROM HighScores WHERE Score = (SELECT MIN(Score) FROM HighScores)"

                oCmd.CommandText = strSQL

                oCmd.Connection.Open()
                oCmd.ExecuteScalar()
                oCmd.Connection.Close()

            End If

            strSQL = "INSERT INTO HighScores (Name, Score, DifficultyID, TheDate) VALUES (@Name, @Score, @Difficulty, @TheDate)"

            oCmd.CommandText = strSQL

            oParam = New SqlParameter
            oParam.ParameterName = "Name"
            oParam.SqlDbType = SqlDbType.VarChar
            oParam.Value = IIf(name_txt.Text.Trim.Length > 0, name_txt.Text.Trim, "Anonymous")
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter
            oParam.ParameterName = "Score"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = Session("FinalScore")
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter
            oParam.ParameterName = "Difficulty"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = difficulty_ddl.SelectedIndex
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter
            oParam.ParameterName = "TheDate"
            oParam.SqlDbType = SqlDbType.DateTime
            oParam.Value = DateTime.Now
            oCmd.Parameters.Add(oParam)

            oCmd.Connection.Open()
            oCmd.ExecuteScalar()

        Catch ex As Exception
            Throw ex
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If

            oCmd.Dispose()
        End Try
        

    End Sub

    Protected Sub submitScore_btn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles submitScore_btn.Click
        result_mpExt.Hide()

        Dim scores As DataTable = currentHighScores()

        If scores.Rows.Count < 10 Then
            addNewHighScore(False)
        ElseIf Session("FinalScore") >= CInt(scores.Rows(scores.Rows.Count - 1)("Score")) Then
            addNewHighScore(True)
        End If

        scores_gv.DataSource = currentHighScores()
        scores_gv.DataBind()

        viewScores_UpdatePnl.Update()

        highScores_mpExt.Show()


    End Sub


    Protected Sub scores_gv_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles scores_gv.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            If e.Row.RowIndex = 0 Then
                For i As Integer = 0 To e.Row.Cells.Count - 1
                    e.Row.Cells(i).Font.Bold = True
                Next
                e.Row.Cells(0).ForeColor = Drawing.ColorTranslator.FromHtml("#8ee221")
            End If
        End If
    End Sub

    Public Function formatDateText(ByVal theDate As DateTime) As String
        Return (theDate.ToShortDateString & ", " & theDate.ToShortTimeString)
    End Function

    Protected Sub viewScores_btn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles viewScores_btn.Click

        scores_gv.DataSource = currentHighScores()
        scores_gv.DataBind()

        highScores_mpExt.Show()

    End Sub

    Protected Sub close_lbtn4_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles close_lbtn4.Click
        highScores_mpExt.Hide()
    End Sub

End Class
