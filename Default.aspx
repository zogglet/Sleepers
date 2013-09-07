<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sleeper Smash: A Game by Maggy Maffia</title>
    
    <link href="style.css" rel="stylesheet" type="text/css" />
    <link href="favicon.ico" rel="icon" type="image/x-icon" />
    
    <script type="text/javascript" language="javascript">

        function startTimer() {
            var timer = $get('<%# level_timer.ClientID %>');
            timer._startTimer();
        }

        function stopTimer() {
            var timer = $get('<%# level_timer.ClientID %>');
            timer._stopTimer();
        }

        function startInnerTimer() {
            var timer = $get('<%# inner_timer.ClientID %>');
            timer._startTimer();
        }

        function stopInnerTimer() {
            var timer = $get('<%# inner_timer.ClientID %>');
            timer._stopTimer();
        }

        function startStopTimer() {
            var timer = $get('<%# stop_timer.ClientID %>');
            timer._startTimer();
        }

        function stopStopTimer() {
            var timer = $get('<%# stop_timer.ClientID %>');
            timer._stopTimer();
        }

        //Prevents Timer from snapping to the top of the page on each tick
        function scrollTo() {
            return;
        }
        
    
    </script>
    
</head>
<body>
    <form id="form1" runat="server">
    
        <%--Required for use of AJAX Control Toolkit --%>
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server" />
        
        <h1>Sleeper Smash</h1>
        <h2>A Game by Maggy Maffia</h2>
        
        <div id="outer_div">
        
            <table id="outer_table">
                
                <tr>
                    <td>
                        Welcome to <b>Sleeper Smash</b>, where you get to pummel waves of sleeping Grunts!
                        <span class="Divider"><br /></span>
                        
                        <br />
                        <asp:UpdatePanel ID="optionsStatus_updatePnl" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                            
                                <asp:Panel ID="options_pnl" runat="server">
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                <asp:DropDownList ID="difficulty_ddl" runat="server" CssClass="InputStyle" DataSourceID="difficulty_sds" DataTextField="DifficultyLevel" DataValueField="ButtonCount" />
                                                
                                                <asp:SqlDataSource ID="difficulty_sds" runat="server" ConnectionString="<%$ ConnectionStrings:HighScoresConnectionString %>" 
                                                    SelectCommand="SELECT ButtonCount, DifficultyLevel FROM Difficulties UNION SELECT -1 AS ButtonCount, '&laquo; Select Difficulty &raquo;' AS DifficultyLevel ORDER BY ButtonCount" />
                                                <asp:CompareValidator ID="difficulty_cVal" runat="server" Operator="NotEqual" ValueToCompare="-1" Display="None" 
                                                    ControlToValidate="difficulty_ddl" ErrorMessage="Please select a difficulty first." />
                                                <asp:ValidatorCalloutExtender ID="difficulty_vcExt" runat="server" TargetControlID="difficulty_cVal" WarningIconImageUrl="warningIcon.png"
                                                    CloseImageUrl="closeIcon.png" CssClass="ValidatorCalloutStyle" Width="200px" />
                                            </td>
                                            <td>
                                            <td align="right">
                                                <asp:Button ID="viewScores_btn" runat="server" CssClass="ButtonStyle" Text="High Scores" CausesValidation="false" />
                                                <asp:Button ID="instructions_btn" runat="server" CssClass="ButtonStyle" Text="Instructions" CausesValidation="false" />
                                                &nbsp;<asp:Button ID="generate_btn" runat="server" Text="Start Game &raquo;" CssClass="ButtonStyle" />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                
                                <asp:Panel ID="status_pnl" runat="server" Visible="false">
                                    <table width="100%">
                                        <tr>
                                            <td valign="top">
                                                <asp:Literal ID="countdown_lit" runat="server" />
                                            </td>
                                            <td valign="top">
                                                <asp:Literal ID="level_lit" runat="server" />
                                            </td>
                                            <td valign="top">
                                                <asp:Literal ID="status_lit" runat="server" />
                                            </td>
                                            <td align="right">
                                                <asp:Button ID="clear_btn" runat="server" Text="Start Over" CssClass="ButtonStyle" OnClick="resetBoard" CausesValidation="false" />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>

                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="generate_btn" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="level_timer" EventName="Tick" />
                                <asp:AsyncPostBackTrigger ControlID="inner_timer" EventName="Tick" />
                                <asp:AsyncPostBackTrigger ControlID="stop_timer" EventName="Tick" />
                                <asp:AsyncPostBackTrigger ControlID="clear_btn" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="restart_lBtn" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        
                        <asp:DropShadowExtender ID="instructions_dsExt" runat="server" TargetControlID="instructions_pnl" Opacity=".15" Width="4" TrackPosition="true" />
                        
                        <%--Instructions Modal Popup--%>
                        <asp:ModalPopupExtender ID="instructions_mpExt" runat="server" TargetControlID="dummy" PopupControlID="instructions_pnl" />
                        <input type="button" id="dummy" runat="server" style="display: none;" />
                        
                        <asp:Panel ID="instructions_pnl" runat="server" CssClass="ModalStyle" Width="350px">
                            
                            <asp:UpdatePanel ID="instructions_updatePnl" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Literal ID="instructions_lit" runat="server" />
                                </ContentTemplate>
                                
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="instructions_btn" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="close_lBtn" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                            
                            <span style="text-align: center; width:100%; display: block;"><asp:LinkButton ID="close_lBtn" runat="server" Text="[Close]" CausesValidation="false" /></span>
                        </asp:Panel>
                        
                        
                        <asp:DropShadowExtender ID="result_dsExt" runat="server" TargetControlID="result_pnl" Opacity=".15" Width="4" TrackPosition="true" />
                        
                        <%--Result Modal Popup--%>
                        <asp:ModalPopupExtender ID="result_mpExt" runat="server" TargetControlID="dummy2" PopupControlID="result_pnl" />
                        <input type="button" id="dummy2" runat="server" style="display: none;" />
                        
                        <asp:Panel ID="result_pnl" runat="server" CssClass="ResultStyle">
                            
                            <asp:UpdatePanel ID="result_updatePnl" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Literal ID="result_lit" runat="server" />
                                    
                                    <%--High score input panel--%>
                                    <asp:Panel ID="highScore_pnl" runat="server" Visible="false" CssClass="inner">
                                        <span class="ModalDivider">&nbsp;</span>
                                        <br />
                                            <span class="ScoreStyle">New High Score!</span>
                                            <p>Enter your name to submit your score: <asp:TextBox ID="name_txt" runat="server" /></p>
                                            <asp:Button ID="submitScore_btn" runat="server" Text="Submit &raquo;" CssClass="ButtonStyle" />
                                    </asp:Panel>
                 
                                </ContentTemplate>
                                
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="level_timer" EventName="Tick" />
                                    <asp:AsyncPostBackTrigger ControlID="restart_lBtn" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="close_lBtn2" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="submitScore_btn" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <br />
                            <span class="inner"><asp:LinkButton ID="restart_lBtn" runat="server" Text="Play Again" OnClick="resetBoard" CausesValidation="false" /> // <asp:LinkButton ID="close_lBtn2" runat="server" Text="[Close]" CausesValidation="false" /></span>
                            
                        </asp:Panel>
                        
                        
                        <asp:DropShadowExtender ID="highScores_dsExt" runat="server" TargetControlID="viewScores_pnl" Opacity=".15" Width="4" TrackPosition="true" />
                        
                        <%--High Scores Modal Popup--%>
                        <asp:ModalPopupExtender ID="highScores_mpExt" runat="server" TargetControlID="dummy4" PopupControlID="viewScores_pnl" />
                        <input type="button" id="dummy4" runat="server" style="display: none;" />
                        
                        
                        <%--High score display panel--%>
                        <asp:Panel ID="viewScores_pnl" runat="server" CssClass="ModalStyle">
                            
                            <asp:UpdatePanel ID="viewScores_UpdatePnl" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <table width="100%">
                                        <tr>
                                            <td align="center">
                                                <span class="ScoreStyle">High Scores</span>
                                                <br /><br />
                                                <asp:Gridview ID="scores_gv" runat="server" DataKeyNames="ID" AutoGenerateColumns="false" GridLines="Horizontal" AllowSorting="false" Width="375px">
                                                    <Columns>
                                                        <asp:BoundField DataField="Score" HeaderText="Score" ItemStyle-CssClass="GVScoreStyle" />
                                                        <asp:BoundField DataField="Name" HeaderText="Name" ItemStyle-CssClass="GVItemStyle" />
                                                        <asp:BoundField DataField="DifficultyLevel" HeaderText="Difficulty" ItemStyle-CssClass="GVItemStyle" />
                                                        <asp:TemplateField HeaderText="Date" ItemStyle-CssClass="GVItemStyle">
                                                            <ItemTemplate>
                                                                <asp:Literal ID="date_lit" runat="server" Text='<%#formatDateText(eval("TheDate")) %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        
                                                    </Columns>
                                                    <HeaderStyle CssClass="GVHeaderStyle" />
                                                </asp:Gridview>
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                                
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="viewScores_btn" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="close_lbtn4" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="submitScore_btn" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                            
                            <span style="text-align: center; width:100%; display: block;"><asp:LinkButton ID="close_lbtn4" runat="server" Text="[Close]" CausesValidation="false" /></span>
                            
                            </asp:Panel>
  
                    </td>

                </tr>
                
                <tr>
                    <td colspan="2">
                        <span class="Divider"></span>
                    </td>
                </tr>
                
                <tr>
                    <td colspan="2" align="center">
                        <asp:UpdatePanel ID="board_updatePnl" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Panel ID="board_pnl" runat="server"></asp:Panel>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="generate_btn" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="clear_btn" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="level_timer" EventName="Tick" />
                                <asp:AsyncPostBackTrigger ControlID="stop_timer" EventName="Tick" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <asp:Timer ID="level_timer" runat="server" Interval="15000" Enabled="false" />
                        <asp:Timer ID="inner_timer" runat="server" Interval="1000" Enabled="false" />
                        <asp:Timer ID="stop_timer" runat="server" Interval="1000" Enabled="false" />

                    </td>
                </tr>
                
            </table>

        </div>
        
        <br />
                    
        <div class="Footer">
        
            Copyright &copy; 2011, <a href="mailto:maggy@zogglet.com?subject=About your awesome Sleeper Smash game">Maggy Maffia</a>
            
        </div>
        
    </form>
</body>
</html>
