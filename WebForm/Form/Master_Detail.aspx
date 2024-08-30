<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Master_Detail.aspx.cs" Inherits="WebForm.Form.Master_Detail" StylesheetTheme="WebForm" %>

<%@ Register Src="~/UserControl/ucCalendar.ascx" TagPrefix="uc2" TagName="ucCalendar" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Master/Detail</title>
    <link href="~/Css/style.css" type="text/css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <table width="100%" align="center">
                    <tr>
                        <td class="FormTitle">Master / Detail
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Panel ID="plQuery" runat="server" GroupingText="Search" Width="100%">
                                <table width="100%" align="center">
                                    <tr>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblCourse_Name" runat="server" Text="CourseName" />
                                        </td>
                                        <td class="ms-formbody" width="30%">
                                            <asp:TextBox ID="txtCourse_Name" runat="server" Width="50%" />
                                        </td>
                                        <td class="ms-formlabel">
                                            <asp:Label ID="lblCourse_Date" runat="server" Text="CourseDate" />
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
                                            <asp:HiddenField ID="hdCourseName" runat="server" />
                                            <asp:HiddenField ID="hdSDate" runat="server" />
                                            <asp:HiddenField ID="hdEDate" runat="server" />
                                            <asp:HiddenField ID="hdMode" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="plFunction" GroupingText="Function" Width="100%">
                                <table>
                                    <tr>
                                        <td align="left">
                                            <asp:Button runat="server" ID="btnAdd" UseSubmitBehavior="false" Text="Add" OnClick="btnAdd_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel ID="plList" runat="server" GroupingText="List" Width="100%">
                                <asp:HiddenField runat="server" ID="hdCourse_ID"/>
                                <asp:GridView ID="gvList" runat="server" Width="100%" AutoGenerateColumns="false" AllowPaging="true" PageSize="10"
                                    OnRowCommand="gvList_RowCommand">
                                    <Columns>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:Button ID="btnModify" runat="server" Text="Modify" CommandName="Modify" CommandArgument='<%# Eval("Course_ID") %>' />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" Width="5%" Wrap="false" />
                                        </asp:TemplateField>
                                        <asp:BoundField HeaderText="Course_Name" DataField="Course_Name" />
                                        <asp:BoundField HeaderText="Instructor" DataField="Instructor" />
                                        <asp:BoundField HeaderText="StartDate" DataField="SDate" />
                                        <asp:BoundField HeaderText="EndDate" DataField="EDate" />
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
                            <asp:Panel ID="plSet" runat="server" Visible="false">
                                <asp:Panel ID="plMaster" runat="server" GroupingText="Master" Width="100%">
                                    <table width="100%" align="center">
                                        <tr>
                                            <td class="ms-formlabel" width="10%">
                                                <asp:Label ID="lblMCourse_Name" runat="server" Text="CourseName" />
                                            </td>
                                            <td class="ms-formbody">
                                                <asp:TextBox ID="txtMCourse_Name" runat="server" Width="80%" />
                                            </td>
                                            <td class="ms-formlabel" width="10%">
                                                <asp:Label ID="lblMInstructor" runat="server" Text="Instructor" />
                                            </td>
                                            <td class="ms-formbody">
                                                <asp:TextBox ID="txtMInstructor" runat="server" Width="80%" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="ms-formlabel" width="10%">
                                                <asp:Label ID="lblMStartDate" runat="server" Text="StartDate" />
                                            </td>
                                            <td class="ms-formbody">
                                                <uc2:ucCalendar runat="server" ID="ucMStartDate" />
                                            </td>
                                            <td class="ms-formlabel" width="10%">
                                                <asp:Label ID="lblMEndDate" runat="server" Text="EndDate" />
                                            </td>
                                            <td class="ms-formbody">
                                                <uc2:ucCalendar runat="server" ID="ucMEndDate" />
                                            </td>
                                        </tr>
                                    </table>
                                    <table width="100%">
                                        <tr>
                                            <td align="center">
                                                <asp:Button ID="btnMDel" runat="server" Text="Delete" OnClick="btnMDelte_Click" OnClientClick="if(confirm('確定要刪除嗎?') != true) {return false;}" />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                <asp:Panel ID="plDetail" runat="server" GroupingText="Detail" Width="100%">
                                    <asp:Button runat="server" ID="btnDAdd" UseSubmitBehavior="false" Text="Add" OnClick="btnDAdd_Click" />
                                    <asp:Label runat="server" ID="lblDTotice" ForeColor="Red" Text="＊Student_ID格式為1英文＋6數字" />
                                    <asp:GridView ID="gvDList" runat="server" Width="100%" AutoGenerateColumns="false" AllowPaging="false" PageSize="10"
                                        OnRowDataBound="gvDList_RowDataBound">
                                        <Columns>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:Button ID="btnDModify" runat="server" Text="Modify" CommandName="Modify" OnClick="btnDModify_Click" />
                                                    <asp:Button ID="btnDDel" runat="server" Text="Delete" CommandName="Del" OnClick="btnDDel_Click" />
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" Width="10%" Wrap="false" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Student_ID">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtDStudent_ID" runat="server" Text='<%#Eval("Student_ID").ToString() %>' Width="90%" MaxLength="7" />
                                                    <asp:Label ID="lblDStudent_ID" runat="server" Text='<%#Eval("Student_ID").ToString() %>' MaxLength="7" />
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" Width="20%" Wrap="false" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Student_Name">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtDStudent_Name" runat="server" Text='<%#Eval("Student_Name").ToString() %>' Width="90%" />
                                                    <asp:Label ID="lblDStudent_Name" runat="server" Text='<%#Eval("Student_Name").ToString() %>' />
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" Width="20%" Wrap="false" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Phone">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtDPhone" runat="server" Text='<%#Eval("Phone").ToString() %>' Width="90%" MaxLength="10" />
                                                    <asp:Label ID="lblDPhone" runat="server" Text='<%#Eval("Phone").ToString() %>' MaxLength="10" />
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" Width="20%" Wrap="false" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Email">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtDEmail" runat="server" Text='<%#Eval("Email").ToString() %>' Width="90%" />
                                                    <asp:Label ID="lblDEmail" runat="server" Text='<%#Eval("Email").ToString() %>' />
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" Width="30%" Wrap="false" />
                                            </asp:TemplateField>
                                        </Columns>
                                        <EmptyDataTemplate>
                                            查無資料！
                                        </EmptyDataTemplate>
                                    </asp:GridView>
                                </asp:Panel>
                                <table width="100%">
                                    <tr>
                                        <td align="center">
                                            <asp:Button ID="btnBack" runat="server" Text="Back" OnClick="btnBack_Click" />
                                            <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
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
