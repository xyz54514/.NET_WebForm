<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm2.aspx.cs" Inherits="WebForm.Form.WebForm2" StylesheetTheme="WebForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>WebForm2</title>
    <link href="~/Css/style.css" type="text/css" rel="stylesheet" />
    <style>
        .divFunction {
            text-align: center;
            position: relative;
        }

        .flex-container {
            display: flex;
            justify-content: center;
            width: 100%;
            flex-wrap: wrap;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <table width="100%" align="center">
                        <tr>
                            <td class="FormTitle">WebForm2
                            </td>
                        </tr>
                        <tr>
                            <td class="flex-container">
                                <asp:Panel ID="plQuery" runat="server" GroupingText="Search" Width="60%">
                                    <table style="text-align: left;" width="100%">
                                        <tr>
                                            <td class="ms-formlabel">
                                                <asp:Label ID="lblName" runat="server" Text="StoreName" />
                                            </td>
                                            <td class="ms-formbody">
                                                <asp:TextBox ID="TextBox1" runat="server" Width="75%" />
                                            </td>
                                            <td class="ms-formlabel">
                                                <asp:Label ID="lblType" runat="server" Text="Address" />
                                            </td>
                                            <td class="ms-formbody">
                                                <asp:DropDownList ID="DropDownList1" runat="server" Width="50%">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                    </table>
                                    <br />
                                    <div class="divFunction">
                                        <asp:Button ID="btnQuery" runat="server" Text="Query" OnClick="btnQuery_Click" />
                                        <asp:Button ID="btnAdd" runat="server" Text="Add" OnClick="btnAdd_Click" />
                                        <asp:HiddenField ID="hdPageIndex" runat="server" />
                                        <asp:HiddenField ID="hdTotalPage" runat="server" />
                                        <asp:HiddenField ID="hdStoreName" runat="server" />
                                        <asp:HiddenField ID="hdAddress" runat="server" />
                                    </div>
                                </asp:Panel>
                                <asp:Panel ID="plList" runat="server" GroupingText="List" Width="60%">
                                <asp:GridView ID="gvList" runat="server" Width="100%" AutoGenerateColumns="false" AllowPaging="true" PageSize="10" OnRowCommand="gvList_RowCommand">
                                    <%--OnRowDataBound="gvList_RowDataBound" OnRowCommand="gvList_RowCommand">--%>
                                    <Columns>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:Button ID="btnModify" runat="server" Text="Modify" CommandName="Modify" CommandArgument='<%# Eval("StoreID") %>' />
                                                <asp:Button ID="btnDelete" runat="server" Text="Delete" CommandName="Del" CommandArgument='<%# Eval("StoreID") %>' OnClientClick="return confirm('確定要刪除?');" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField HeaderText="StoreName" DataField="StoreName" />
                                        <asp:BoundField HeaderText="Address" DataField="Address" />
                                        <asp:BoundField HeaderText="CreateDate" DataField="CreateDate" />
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
                            <asp:Panel ID="plSet" runat="server" Width="60%">
                                <table style="text-align: left;" width="100%">
                                    <tr>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblSID" runat="server" Text="StoreID" />
                                        </td>
                                        <td class="ms-formbody">
                                            <asp:TextBox ID="txtSetSID" runat="server" Width="75%" Enabled="false" />
                                        </td>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblSetSName" runat="server" Text="StoreName" />
                                        </td>
                                        <td class="ms-formbody">
                                            <asp:TextBox ID="txtSetSName" runat="server" Width="75%" />
                                        </td>
                                            <tr>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblSetAddress" runat="server" Text="Address" />
                                        </td>
                                        <td class="ms-formbody">
                                            <asp:TextBox ID="txtSetAddress" runat="server" Width="75%" />
                                        </td>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblSettPhone" runat="server" Text="Telephone" />
                                        </td>
                                        <td class="ms-formbody">
                                            <asp:TextBox ID="txtSettPhone" runat="server" Width="75%" />
                                        </td>
                                    </tr>
                                    
                                </table>
                                <br />
                                <div class="divFunction">
                                    <asp:Button ID="btnBack" runat="server" Text="Back" OnClick="btnBack_Click" />
                                    <asp:Button ID="btnSet" runat="server" OnClick="btnSave_Click" />
                                    <asp:HiddenField ID="hdMode" runat="server" />
                                </div>
                            </asp:Panel>
            <div>
            </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
