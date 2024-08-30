<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="myExport.aspx.cs" Inherits="WebForm.Form.myExport" %>

<%@ Register Src="~/UserControl/ucCalendar.ascx" TagPrefix="uc1" TagName="ucCalendar" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Excel</title>
    <link href="~/Css/style.css" type="text/css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"  />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <table width="100%" align="center">
                    <tr>
                        <td class="FormTitle">myExport
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Panel ID="plQuery" runat="server" GroupingText="Search" Width="100%">
                                <table width="100%" align="center">
                                    <tr>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblStoreName" runat="server" Text="StoreName" />
                                        </td>
                                        <td class="ms-formbody" width="30%">
                                            <asp:TextBox ID="txtStoreName" runat="server" width="50%" />
                                        </td>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblAddress" runat="server" Text="Address" />
                                        </td>
                                        <td class="ms-formbody" width="30%">
                                            <asp:DropDownList ID="ddlAddress" runat="server" width="50%">
                                            </asp:DropDownList>
                                        </td>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblCreateDate" runat="server" Text="CreateDate" />
                                        </td>
                                        <td>
                                            <uc1:ucCalendar runat="server" ID="ucStartDate" />
                                            &nbsp;~&nbsp;
                                            <uc1:ucCalendar runat="server" ID="ucEndDate" />
                                        </td>
                                    </tr>
                                </table>
                                <table width="100%">
                                    <tr>
                                        <td align="center">
                                            <asp:Button ID="btnQuery" runat="server" Text="Query" OnClick="btnQuery_Click" />
                                            <asp:HiddenField ID="hdPageIndex" runat="server" />
                                            <asp:HiddenField ID="hdTotalPage" runat="server" />
                                            <asp:HiddenField ID="hdStoreName" runat="server" />
                                            <asp:HiddenField ID="hdAddress" runat="server" />
                                            <asp:HiddenField ID="hdSDate" runat="server" />
                                            <asp:HiddenField ID="hdEDate" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel ID="plFunction" runat="server" GroupingText="Function" Width="100%">
                                <table>
                                    <tr>
                                        <td align="left">
                                            <asp:Button ID="btnExpExcel" runat="server" UseSubmitBehavior="false" Text="Export_Excel" OnClick="btnExpExcel_Click" />
                                        </td>
                                        <td align="left">
                                            <asp:Button ID="btnExpCsv" runat="server" UseSubmitBehavior="false" Text="Export_Csv" OnClick="btnExpCsv_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel ID="plList" runat="server" GroupingText="List" Width="100%">
                               <asp:GridView id="gvList" runat="server" Width="100%" AutoGenerateColumns="false" AllowPaging="true" PageSize="10">
                                   <Columns>
                                       <asp:BoundField HeaderText="StoreID" DataField="StoreID" />
                                       <asp:BoundField HeaderText="StoreName" DataField="StoreName" />
                                       <asp:BoundField HeaderText="Address" DataField="Address" />
                                       <asp:BoundField HeaderText="Telephone" DataField="Telephone" />
                                       <asp:BoundField HeaderText="CreateDate" DataField="CreateDate" />
                                   </Columns>
                                   <EmptyDataRowStyle HorizontalAlign="Left" Width="100%" />
                                   <EmptyDataTemplate>
                                       查無資料
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
        <div>
        </div>
    </form>
</body>
</html>
