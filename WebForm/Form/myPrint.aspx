<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="myPrint.aspx.cs" Inherits="WebForm.Form.myPrint" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>myPrint</title>
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
                        <td class="FormTitle">
                            myPrint
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Panel ID="Panel1" runat="server" GroupingText="myPrint">
                                <table style="text-align:left;" width="100%">
                                    <tr>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblDate" runat="server" Text="Date" />
                                        </td>
                                        <td class="ms-formbody">
                                            <asp:TextBox ID="txtDate" runat="server" Width="50%" />
                                        </td>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblCustName" runat="server" Text="CustName" />
                                        </td>
                                        <td class="ms-formbody">
                                            <asp:TextBox ID="txtCustName" runat="server" Width="50%" />
                                        </td>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblAddress" runat="server" Text="Address" />
                                        </td>
                                        <td class="ms-formbody">
                                            <asp:TextBox ID="txtAddress" runat="server" Width="50%" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblNo" runat="server" Text="A/C NO." />
                                        </td>
                                        <td class="ms-formbody">
                                            <asp:TextBox ID="txtNo" runat="server" Width="50%" />
                                        </td>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblRemark" runat="server" Text="Remark" />
                                        </td>
                                        <td class="ms-formbody">
                                            <asp:TextBox ID="txtRemark" runat="server" Width="50%" />
                                        </td>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblComment" runat="server" Text="Comment" />
                                        </td>
                                        <td class="ms-formbody">
                                            <asp:TextBox ID="txtComment" runat="server" Width="50%" />
                                        </td>
                                    </tr>
                                   <%-- <tr>
                                       <div class="form"> 
                                        
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblPicture" runat="server" Text="Picture" />
                                        </td>
                                        <td class="ms-formbody">
                                            
                                            <asp:FileUpload ID="picUpload" runat="server" onchange="fileUpload_Changed"/>
                                            <asp:Button ID="btnUpload" runat="server" Text="Upload" OnClick="btnUpload_Click" />
                                            <br />
                                            <asp:Image ID="imgPreview" runat="server" style="max-width: 200px; max-height: 200px;"/>
                                        </td>
                                    </tr>--%>
                                </table></ br>
                                <div class="divFunction" style="text-align: center;">
                                    <%--<asp:Button ID="btnPDF" runat="server" Text="Print_PDF" OnClick="btnPDF_Click"/>--%>
                                    <asp:Button ID="btnWord" runat="server" Text="Print_WORD" OnClick="btnWord_Click"/>
                                </div>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <%--<asp:PostBackTrigger ControlID="btnUpload" />--%>
                <%--
                <asp:PostBackTrigger ControlID="btnPDF" />
                --%>
                <asp:PostBackTrigger ControlID="btnWord" />
            </Triggers>
        </asp:UpdatePanel>
    </form>
</body>
</html>
