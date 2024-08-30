<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebAPI.aspx.cs" Inherits="WebForm.Form.WebAPI" StylesheetTheme="WebForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>WebAPI</title>
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
                        <td class="FormTitle">WebAPI(Employee)
                        </td>
                    </tr>
                    <tr>
                        <td class="flex-container">
                            <asp:Panel ID="plQuery" runat="server" GroupingText="Search" Width="60%">
                                <table style="text-align: left;" width="100%">
                                    <tr>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblCIDr" runat="server" Text="客戶編號" />
                                        </td>
                                        <td class="ms-formbody">
                                            <asp:TextBox ID="txtCID" runat="server" Width="75%" />
                                        </td>
                                    </tr>
                                </table>
                                <div class="divFunction">
                                    <asp:Button ID="btnRequest" runat="server" Text="查詢" OnClick="btnRequest_Click" />
                                </div>
                            </asp:Panel>
                            <asp:Panel ID="Panel1" runat="server" GroupingText="客戶資料" Width="60%">
                                <table style="text-align: left;" width="100%">
                                    <tr>
                                        <td class="ms-formlabel" colspan="4" style="text-align:center;">
                                            <asp:Label ID="lblJson" runat="server" Text="Json資料" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="ms-formbody" colspan="4">
                                            <asp:TextBox ID="txtJson" runat="server" Width="100%" Rows="5" TextMode="MultiLine" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="ms-formlabel" colspan="4" style="text-align:center;">
                                            <asp:Label ID="Label1" runat="server" Text="經過Json轉class後" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblName" runat="server" Text="客戶名稱" />
                                        </td>
                                        <td class="ms-formbody">
                                            <asp:TextBox ID="txtName" runat="server" Width="75%" />
                                        </td>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblCity" runat="server" Text="居住城市" />
                                        </td>
                                        <td class="ms-formbody">
                                            <asp:TextBox ID="txtCity" runat="server" Width="75%" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblPhone" runat="server" Text="手機號碼" />
                                        </td>
                                        <td class="ms-formbody">
                                            <asp:TextBox ID="txtPhone" runat="server" Width="75%" />
                                        </td>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblType" runat="server" Text="會員等級" />
                                        </td>
                                        <td class="ms-formbody">
                                            <asp:TextBox ID="txtType" runat="server" Width="75%" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
