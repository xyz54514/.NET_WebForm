<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebService.aspx.cs" Inherits="WebForm.Form.WebService" StylesheetTheme="WebForm"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>WebService</title>
    <link href="~/Css/style.css" type="text/css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <table width="100%" align="center">
                    <tr>
                        <td class="FormTitle">WebService
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Panel ID="plCal" runat="server" GroupingText="Calculate" Width="100%">
                                <span style="color:red">
                                    ＊注意事項：使用此功能前請先確認參考服務的WSDL檔路徑是否正確，若錯誤則依照手冊的步驟進行重新設定。
                                </span>
                                <table width="100%" align="center">
                                    <tr>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblHeight" runat="server" Text="Height" />
                                        </td>
                                        <td class="ms-formbody" width="30%">
                                            <asp:TextBox ID="txtHeight" runat="server" Width="50%" />
                                        </td>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblWeight" runat="server" Text="Weight" />
                                        </td>
                                        <td class="ms-formbody" width="30%">
                                             <asp:TextBox ID="txtWeight" runat="server" Width="50%" />
                                        </td>
                                    </tr>
                                </table>
                                <table width="100%">
                                    <tr>
                                        <td align="center">
                                            <asp:Button ID="btnCal" runat="server" Text="Calculate" OnClick="btnCal_Click" />
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
