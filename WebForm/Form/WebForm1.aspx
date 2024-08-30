<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="WebForm.Form.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            
            <asp:DropDownList runat="server" ID="ddlSex" >
            <asp:ListItem Text="男"></asp:ListItem>
            <asp:ListItem>女</asp:ListItem>
            <asp:ListItem>未知</asp:ListItem>
            </asp:DropDownList>

            <asp:RadioButton runat="server" ID="rdbtnM" Text="男" GroupName="Sex" />
            <asp:RadioButton runat="server" ID="rdbtnF" Text="女" GroupName="Sex"/>
            <br>
            <asp:Button runat="server" ID="fff" Text="commit123" />
            <br>

            CheckBox444:
            <asp:CheckBox runat="server" ID="cbpra" Text="checkbox1" />
             <asp:CheckBox runat="server" ID="Chec1" Text="checkbox2" />
            <br>

            CheckBoxList:
            <asp:CheckBoxList runat="server" ID="ssss"  RepeatDirection="Vertical">
                <asp:ListItem>1</asp:ListItem>
                <asp:ListItem>2</asp:ListItem>
                <asp:ListItem>3</asp:ListItem>
            </asp:CheckBoxList>
            <br>

            <asp:Label ID="tt" runat="server" Text="這是一個label" CssClass="Test"></asp:Label>
            <br>

            <asp:TextBox ID="hhh" runat="server" width="600px" height="150px" TextMode="MultiLine"  MaxLength="150"
                PlaceHolder="textbox的預設值"></asp:TextBox>
            <br>

            <%--<asp:GridView runat="server" ID="gvList" AllowPaging="true" AllowCustomPaging="true" AutoGenerateColumns="false" PageSize="2"
                OnPageIndexChanging="gvList_PageIndexChanging" OnPreRender="gvList_PreRender"
                OnRowDataBound="gvList_RowDataBound" OnRowCommand="gvList_RowCommand">
                <Columns>
                    <asp:BoundField HeaderText="ID" DataField="ID" />
                    <asp:BoundField HeaderText="Name" DataField="Name" />
                    <asp:BoundField HeaderText="Age" DataField="Age" />
                </Columns>--%>
                <%--
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:Button ID="btnSelect" runat="server" Text="Select" CommandName="Select" />
                    </ItemTemplate>
                </asp:TemplateField>
                --%>
           <%-- </asp:GridView>--%>


        </div>
    </form>
</body>
</html>
