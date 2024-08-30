<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="測試用.aspx.cs" Inherits="WebForm.Form.測試用" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Upload Image</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Upload Image</h2>
            <asp:FileUpload ID="fileUpload" runat="server" Text="選擇上傳圖片"/>
            
            
            <asp:Button ID="btnUpload" runat="server" Text="Upload" OnClick="btnUpload_Click" />
            <br /><br />
            <asp:Image id="imgPreview" runat="server" style="max-width: 300px; max-height: 300px;" />
            <%--
            <asp:Panel ID="plPreview" runat="server" Width="60%">
                <asp:Image id="imgPreview" runat="server" style="max-width: 300px; max-height: 300px;" />
            </asp:Panel>
            --%>
        </div>
    </form>
</body>
</html>
