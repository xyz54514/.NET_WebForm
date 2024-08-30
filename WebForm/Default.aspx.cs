using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebForm
{
    public partial class Default : basePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 設定 TreeView Node 的 ValuePath 分隔符號，預設是 /，
                // 故意設成跟 SQL Server CTE 路經一樣的分隔符號 _
                TreeView1.PathSeparator = Convert.ToChar("_");
                // 顯示父 Node 和 子 Node 的關聯線
                TreeView1.ShowLines = true;

                // 初始設定，預設顯示的頁面
                iframeContent.Attributes["src"] = "HomePage.aspx";

                // 設定使用者名稱
                lblUserID.Text = base.strUserID;
            }
        }

        protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
        {
            // 獲取選擇的頁面 URL
            string selectedPage = TreeView1.SelectedNode.Value;
            // 修改 iframe 的 src 屬性來顯示對應的頁面
            iframeContent.Attributes["src"] = selectedPage;
        }
    }
}