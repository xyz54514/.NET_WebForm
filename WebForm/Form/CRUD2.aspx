<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CRUD2.aspx.cs" Inherits="WebForm.Form.CRUD2" StylesheetTheme="WebForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>CRUD</title>
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
                        <td class="FormTitle">CRUD2
                        </td>
                    </tr>
                    <tr>
                        <td class="flex-container">
                            <asp:Panel ID="plQuery" runat="server" GroupingText="Search" Width="60%">
                                <table style="text-align: left;" width="100%">
                                    <tr>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblName" runat="server" Text="Name" />
                                        </td>
                                        <td class="ms-formbody">
                                            <asp:TextBox ID="txtName" runat="server" Width="75%" />
                                        </td>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblType" runat="server" Text="會員等級" />
                                        </td>
                                        <td class="ms-formbody">
                                            <asp:DropDownList ID="ddlType" runat="server" Width="50%">
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
                                    <asp:HiddenField ID="hdName" runat="server" />
                                    <asp:HiddenField ID="hdType" runat="server" />
                                </div>
                            </asp:Panel>
                            <asp:Panel ID="plList" runat="server" GroupingText="List" Width="60%">
                                <asp:Repeater ID="rptList" runat="server" OnItemDataBound="rptList_ItemDataBound" OnItemCommand="rptList_ItemCommand">
                                    <HeaderTemplate>
                                        <table border="1" width="100%">
                                            <tr>
                                                <td></td>
                                                <td>Name</td>
                                                <td>會員等級</td>
                                                <td>City</td>
                                                <td>Phone</td>
                                            </tr>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr style="background-color: #ffffff;">
                                            <td>
                                                <asp:Button ID="btnModify" runat="server" Text="Modify" CommandName="Modify" CommandArgument='<%# Eval("CID") %>' />
                                                <asp:Button ID="btnDelete" runat="server" Text="Delete" CommandName="Del" CommandArgument='<%# Eval("CID") %>' OnClientClick="return confirm('確定要刪除?');" />
                                            </td>
                                            <td>
                                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("Type") %>'></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="Label4" runat="server" Text='<%# Eval("City") %>'></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("Phone") %>'></asp:Label>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <AlternatingItemTemplate>
                                        <tr style="background-color: #f0f0f0;">
                                            <td>
                                                <asp:Button ID="btnModify" runat="server" Text="Modify" CommandName="Modify" CommandArgument='<%# Eval("CID") %>' />
                                                <asp:Button ID="btnDelete" runat="server" Text="Delete" CommandName="Del" CommandArgument='<%# Eval("CID") %>' OnClientClick="return confirm('確定要刪除?');" />
                                            </td>
                                            <td>
                                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("Type") %>'></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="Label4" runat="server" Text='<%# Eval("City") %>'></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("Phone") %>'></asp:Label>
                                            </td>
                                        </tr>
                                    </AlternatingItemTemplate>
                                    <FooterTemplate>
                                        </table>
                                    </FooterTemplate>
                                </asp:Repeater>
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
                                            <asp:Label ID="lblCID" runat="server" Text="CID" />
                                        </td>
                                        <td class="ms-formbody">
                                            <asp:TextBox ID="txtSetCID" runat="server" Width="75%" Enabled="false" />
                                        </td>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblSetName" runat="server" Text="Name" />
                                        </td>
                                        <td class="ms-formbody">
                                            <asp:TextBox ID="txtSetName" runat="server" Width="75%" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblSetCity" runat="server" Text="City" />
                                        </td>
                                        <td class="ms-formbody">
                                            <asp:TextBox ID="txtSetCity" runat="server" Width="75%" />
                                        </td>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblSetPhone" runat="server" Text="Phone" />
                                        </td>
                                        <td class="ms-formbody">
                                            <asp:TextBox ID="txtSetPhone" runat="server" Width="75%" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblSetType" runat="server" Text="會員等級" />
                                        </td>
                                        <td class="ms-formbody">
                                            <asp:DropDownList ID="ddlSetType" runat="server" Width="50%">
                                            </asp:DropDownList>
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
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
