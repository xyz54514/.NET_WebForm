<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebForm.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    
    <style>
        html, body {
            height: 100%;
            margin: 0;
        }
        .top-panel {
            height: 30px; /* 可以根據需要調整高度 */
            background-color: #d0d0d0; /* 這只是示例，方便看效果 */
            display: flex;
            align-items: center;
            justify-content: flex-end; /* 文字靠右對齊 */
            padding: 0 20px;
            box-sizing: border-box;
        }
        .container {
            display: flex;
            height: calc(100vh - 30px); /* 減去 TOP 區塊的高度 */
        }
        .left-panel {
            width: 200px;
            background-color: #f0f0f0; /* 這只是示例，方便看效果 */
        }
        .right-panel {
            flex-grow: 1;
            padding-left: 20px;
        }
        iframe {
            width: 100%;
            height: 100%; /* 可以根據需要調整 */
            border: none;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <asp:UpdatePanel runat="server" ID="UpdatePanel">
            <ContentTemplate>
                <div class="top-panel">
                    <!-- 這裡可以放置你想在 TOP 區塊顯示的內容 -->
                    HI,
                    <asp:Label ID="lblUserID" runat="Server" Text="UserName"></asp:Label>
                    您好!
                </div>
                <div class="container">
                    <div class="left-panel">
                        <asp:TreeView ID="TreeView1" runat="server" AutoPostBack="True" OnSelectedNodeChanged="TreeView1_SelectedNodeChanged">
                            <Nodes>
                                <asp:TreeNode Text="WebForm" Value="HomePage.aspx">
                                    <asp:TreeNode Text="CRUD" Value="Form/CRUD.aspx"></asp:TreeNode>
                                    <asp:TreeNode Text="CRUD2" Value="Form/CRUD2.aspx"></asp:TreeNode>
                                    <asp:TreeNode Text="Maker" Value="Form/Maker.aspx"></asp:TreeNode>
                                    <asp:TreeNode Text="Checker" Value="Form/Checker.aspx"></asp:TreeNode>
                                    <asp:TreeNode Text="Print" Value="Form/Print.aspx"></asp:TreeNode>
                                    <asp:TreeNode Text="Export" Value="Form/Export.aspx"></asp:TreeNode>
                                    <asp:TreeNode Text="Batch Import" Value="Form/Batch_Import.aspx"></asp:TreeNode>
                                    <asp:TreeNode Text="Master / Detail" Value="Form/Master_Detail.aspx"></asp:TreeNode>
                                    <asp:TreeNode Text="WebAPI" Value="Form/WebAPI.aspx"></asp:TreeNode>
                                    <asp:TreeNode Text="WebService" Value="Form/WebService.aspx"></asp:TreeNode>
                                    <asp:TreeNode Text="WebForm1" Value="Form/WebForm1.aspx"></asp:TreeNode>
                                    <asp:TreeNode Text="WebForm2" Value="Form/WebForm2.aspx"></asp:TreeNode>
                                    <asp:TreeNode Text="myMaker" Value="Form/myMaker.aspx"></asp:TreeNode>
                                    <asp:TreeNode Text="myChecker" Value="Form/myChecker.aspx"></asp:TreeNode>
                                    <asp:TreeNode Text="myPrint" Value="Form/myPrint.aspx"></asp:TreeNode>
                                    <asp:TreeNode Text="myExport" Value="Form/myExport.aspx"></asp:TreeNode>
                                    <asp:TreeNode Text="myBatch Import" Value="Form/myBatch_Import.aspx"></asp:TreeNode>
                                    <asp:TreeNode Text="測試用" Value="Form/測試用.aspx"></asp:TreeNode>
                                    <asp:TreeNode Text="myMaster / Detail" Value="Form/myMaster_Detail.aspx"></asp:TreeNode>
                                </asp:TreeNode>
                            </Nodes>
                        </asp:TreeView>
                    </div>
                    <div class="right-panel">
                        <iframe id="iframeContent" runat="server" src="HomePage.aspx"></iframe>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
