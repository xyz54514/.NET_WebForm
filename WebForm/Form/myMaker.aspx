﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="myMaker.aspx.cs" Inherits="WebForm.Form.myMaker" %>
<%@ Register Src="~/UserControl/ucCalendar.ascx" TagPrefix="uc1" TagName="ucCalendar" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>myMaker</title>
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
<script type="text/javascript">
    function isNumberKey(evt) {
        var charCode = (evt.which) ? evt.which : event.keyCode;
        if (charCode > 31 && (charCode < 48 || charCode > 57))
            return false;
        return true;
    }
</script>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <table width="100%" align="center">
                    <tr>
                        <td class="FormTitle">
                            myMaker
                        </td>
                    </tr>
                    <tr>
                        <td class="flex-container">
                            <asp:Panel ID="plQuery" runat="server" GroupingText="Search" Width="99%">
                                <table style="text-align:left;" width="100%">
                                    <tr>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblName" runat="server" Text="ProductName" />
                                        </td>
                                        <td class="ms-formbody">
                                            <asp:TextBox ID="txtName" runat="server" Width="30%" />
                                            <uc1:ucCalendar runat="server" ID="ucCalendar" />                                      
                                        </td>
                                    </tr>
                                </table><br />
                                <div class="divFunction" style="text-align: center;">
                                    <asp:Button ID="btnQuery" runat="server" Text="Query" OnClick="btnQuery_Click" />
                                    <asp:Button ID="btnAdd" runat="server" Text="Add" OnClick="btnAdd_Click" />
                                    <asp:HiddenField ID="hdPageIndex" runat="server" />
                                    <asp:HiddenField ID="hdTotalPage" runat="server" />
                                    <asp:HiddenField ID="hdName" runat="server" />
                                    <asp:HiddenField ID="hdTime" runat="server" />
                                </div>
                            </asp:Panel>
                            <asp:Panel ID="plList" runat="server" GroupingText="List" Width="99%">
                                <asp:GridView ID="gvList" runat="server" Width="100%" AutoGenerateColumns="false" AllowPaging="true" PageSize="10"
                                    OnRowDataBound="gvList_RowDataBound" OnRowCommand="gvList_RowCommand">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Action">
                                            <ItemTemplate>
                                                <asp:Button ID="btnModify" runat="server" Text="Modify" CommandName="Modify" CommandArgument='<%# Eval("OrderID") %>' />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" Width="15%" />
                                        </asp:TemplateField>
                                        <asp:BoundField HeaderText="ProductName" DataField="ProductName" />
                                        <asp:BoundField HeaderText="Quantity" DataField="Quantity" />
                                        <asp:BoundField HeaderText="UnitPrice" DataField="UnitPrice" />
                                        <asp:BoundField HeaderText="TotalPrice" DataField="TotalPrice" />
                                        <asp:BoundField HeaderText="Orderdate" DataField="Orderdate" />
                                        <asp:BoundField HeaderText="Createdate" DataField="Createdate" />
                                        <asp:BoundField HeaderText="Status" DataField="Status_Ch" />
                                    </Columns>
                                    <EmptyDataRowStyle HorizontalAlign="Left" Width="100%" />
                                    <EmptyDataTemplate>
                                        查無資料！
                                    </EmptyDataTemplate>
                                </asp:GridView>
                                <table width="100%">
                                    <tr>
                                        <td align="center">
                                            <asp:Button ID="btnFirst" runat="server" Text="第一頁" OnClick="btnFirst_Click"/>
                                            <asp:Button ID="btnPre" runat="server" Text="上一頁" OnClick="btnPre_Click"/>
                                            <asp:Button ID="btnNext" runat="server" Text="下一頁" OnClick="btnNext_Click"/>
                                            <asp:Button ID="btnLast" runat="server" Text="最後一頁" OnClick="btnLast_Click"/>
                                            <asp:Label ID="lblPager" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel ID="plSet" runat="server" GroupingText="Add" Width="99%">
                                <table style="text-align:left;" width="100%">
                                    <tr>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblSetName" runat="server" Text="ProductName" />
                                        </td>
                                        <td class="ms-formbody">
                                            <asp:TextBox ID="txtSetName" runat="server" Width="30%" />
                                        </td>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblSetQuantity" runat="server" Text="Quantity" />
                                        </td>
                                        <td class="ms-formbody">
                                            <asp:TextBox ID="txtSetQuantity" runat="server" Width="30%" onkeypress="return isNumberKey(event)" />
                                        </td>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblSetUnitPrice" runat="server" Text="UnitPrice" />
                                        </td>
                                        <td class="ms-formbody">
                                            <asp:TextBox ID="txtSetUnitPrice" runat="server" Width="30%" onkeypress="return isNumberKey(event)" />
                                        </td>
                                    </tr>
                                </table><br />
                                <div class="divFunction">
                                    <asp:Button ID="btnBack" runat="server" Text="Back" OnClick="btnBack_Click"/>
                                    <asp:Button ID="btnDelete" runat="server" Text="Delete" OnClick="btnDelete_Click" OnClientClick="return confirm('確定要刪除?');"/>
                                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" />
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
