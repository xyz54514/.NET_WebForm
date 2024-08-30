<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="myBatch_import.aspx.cs" Inherits="WebForm.Form.myBatch_import" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>myBatch Import</title>
    <link href="~/Css/style.css" type="text/css" rel="stylesheet" />
    <style>
        .button {
            background-color: #4CAF50; /* Green */
            border: none;
            color: white;
            padding: 16px 32px;
            text-align: center;
            text-decoration: none;
            display: inline-block;
            font-size: 16px;
            margin: 4px 2px;
            transition-duration: 0.4s;
            cursor: pointer;
        }

        .button1 {
            background-color: white;
            color: black;
            border: 2px solid #4CAF50;
        }

            .button1:hover {
                background-color: #4CAF50;
                color: white;
            }

        .button2 {
            background-color: white;
            color: black;
            border: 2px solid #008CBA;
        }

            .button2:hover {
                background-color: #008CBA;
                color: white;
            }

        .button3 {
            background-color: white;
            color: black;
            border: 2px solid #f44336;
        }

            .button3:hover {
                background-color: #f44336;
                color: white;
            }

        .button4 {
            background-color: white;
            color: black;
            border: 2px solid #e7e7e7;
        }

            .button4:hover {
                background-color: #e7e7e7;
            }
    </style>

    <script>

        //開啟檔案總管
        function showBrowseDialog(UniqueID) {
            var objGetFile = document.getElementById(UniqueID);

            if (objGetFile && document.createEvent) {
                var evt = new MouseEvent("click", { bubbles: true, cancelable: false, composed: true });
                objGetFile.dispatchEvent(evt);
            }
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <table width="100%" align="center">
                    <tr>
                        <td class="FormTitle">Batch Import(Employee)
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Panel ID="plFunction" runat="server" GroupingText="Function" Width="100%">
                                <asp:FileUpload ID="fuFile" runat="server" />
                                <asp:Button ID="btnExcel_Import" runat="server" Text="Excel Import" OnClick="btnExcel_Import_Click" />
                                <asp:Button ID="btnCSV_Import" runat="server" Text="CSV Import" OnClick="btnCSV_Import_Click" />
                                <asp:Button ID="btnTelegram_Import" runat="server" Text="Telegram Import" OnClick="btnTelegram_Import_Click" />
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="btnExcel_Import" />
                <asp:PostBackTrigger ControlID="btnCSV_Import" />
                <asp:PostBackTrigger ControlID="btnTelegram_Import" />
            </Triggers>
        </asp:UpdatePanel>
    </form>
</body>
</html>
