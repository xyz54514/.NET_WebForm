<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Export.aspx.cs" Inherits="WebForm.Form.Export_Excel" StylesheetTheme="WebForm" %>

<%@ Register Src="~/UserControl/ucCalendar.ascx" TagPrefix="uc2" TagName="ucCalendar" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Excel</title>
    <link href="~/Css/style.css" type="text/css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <table width="100%" align="center">
                    <tr>
                        <td class="FormTitle">Export
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Panel ID="plQuery" runat="server" GroupingText="Search" Width="100%">
                                <table width="100%" align="center">
                                    <tr>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblName" runat="server" Text="Name" />
                                        </td>
                                        <td class="ms-formbody" width="30%">
                                            <asp:TextBox ID="txtEmpName" runat="server" Width="50%" />
                                        </td>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblDept" runat="server" Text="Department" />
                                        </td>
                                        <td class="ms-formbody" width="30%">
                                            <asp:DropDownList ID="ddlDept" runat="server" Width="50%">
                                            </asp:DropDownList>
                                        </td>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblDate" runat="server" Text="JoiningDate" />
                                        </td>
                                        <td class="ms-formbody" width="40%">
                                            <uc2:ucCalendar runat="server" ID="ucStartDate" />
                                            &nbsp;~&nbsp;
                                            <uc2:ucCalendar runat="server" ID="ucEndDate" />
                                        </td>
                                    </tr>
                                </table>
                                <table width="100%">
                                    <tr>
                                        <td align="center">
                                            <asp:Button ID="btnQuery" runat="server" Text="Query" OnClick="btnQuery_Click" />
                                            <asp:HiddenField ID="hdPageIndex" runat="server" />
                                            <asp:HiddenField ID="hdTotalPage" runat="server" />
                                            <asp:HiddenField ID="hdName" runat="server" />
                                            <asp:HiddenField ID="hdDept" runat="server" />
                                            <asp:HiddenField ID="hdSDate" runat="server" />
                                            <asp:HiddenField ID="hdEDate" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="plFunction" GroupingText="Function" Width="100%">
                                <table>
                                    <tr>
                                        <td align="left">
                                            <asp:Button runat="server" ID="btnExpExcel" UseSubmitBehavior="false" Text="Export_Excel" OnClick="btnExpExcel_Click" />
                                        </td>
                                        <td align="left">
                                            <asp:Button runat="server" ID="btnExpCsv" UseSubmitBehavior="false" Text="Export_Csv" OnClick="btnExpCsv_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel ID="plList" runat="server" GroupingText="List" Width="100%">
                                <asp:GridView ID="gvList" runat="server" Width="100%" AutoGenerateColumns="false" AllowPaging="true" PageSize="10">
                                    <Columns>
                                        <asp:BoundField HeaderText="Employee_ID" DataField="Employee_ID" />
                                        <asp:BoundField HeaderText="Name" DataField="Employee_Name" />
                                        <asp:BoundField HeaderText="Sex" DataField="SexNm" />
                                        <asp:BoundField HeaderText="Dept" DataField="Dept" />
                                        <asp:BoundField HeaderText="Email" DataField="Email" />
                                        <asp:BoundField HeaderText="JoiningDate" DataField="JoiningDate" />
                                    </Columns>
                                    <EmptyDataRowStyle HorizontalAlign="Left" Width="100%" />
                                    <EmptyDataTemplate>
                                        查無資料！
                                    </EmptyDataTemplate>
                                </asp:GridView>
                                <table width="100%">
                                    <tr>
                                        <td align="center">
                                            <asp:Button ID="btnFirst" runat="server" Text="第一頁" OnClick="btnFirst_Click" />
                                            <asp:Button ID="btnPre" runat="server" Text="上一頁" OnClick="btnPre_Click" />
                                            <asp:Button ID="btnNext" runat="server" Text="下一頁" OnClick="btnNext_Click" />
                                            <asp:Button ID="btnLast" runat="server" Text="最後一頁" OnClick="btnLast_Click" />
                                            <asp:Label ID="lblPager" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="btnExpExcel" />
                <asp:PostBackTrigger ControlID="btnExpCsv" />
            </Triggers>
        </asp:UpdatePanel>
    </form>
</body>
</html>
