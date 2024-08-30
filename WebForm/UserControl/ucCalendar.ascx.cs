using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace WebForm.UserControl
{
    public partial class ucCalendar : System.Web.UI.UserControl
    {

        /// <summary>
        /// 取得或設定TextBox的文字
        /// </summary>
        public string Text
        {
            set
            {
                if (value.Length == 8)
                {
                    txtCalendar.Text = value.Substring(0, 4) + "/" + value.Substring(4, 2) + "/" + value.Substring(6, 2);
                }
                else
                {
                    txtCalendar.Text = value;
                }
            }
            get
            {
                return txtCalendar.Text.Trim().Replace("-", "/");
            }
        }

        public string CssClass
        {
            set
            {
                txtCalendar.CssClass = value;
            }
            get
            {
                return txtCalendar.CssClass;
            }
        }

        public bool ReadOnly
        {
            set
            {
                txtCalendar.ReadOnly = value;
                ibtnCalendar.Enabled = !value;
                ceCalendar.Enabled = !value;
            }
            get
            {
                return txtCalendar.ReadOnly;
            }
        }

        public bool IsEnabled
        {
            set
            {
                txtCalendar.Enabled = value;
                ibtnCalendar.Enabled = value;
                ceCalendar.Enabled = value;
            }
            get
            {
                return txtCalendar.Enabled;
            }
        }

        public bool IsAutoPostBack
        {
            set
            {
                txtCalendar.AutoPostBack = value;
            }
            get
            {
                return txtCalendar.AutoPostBack;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("function Clear_" + txtCalendar.ClientID + "() {");
            sb.AppendLine("     var MyCalendar = document.getElementById('" + txtCalendar.ClientID + "');");
            sb.AppendLine("     MyCalendar.value='';");
            sb.AppendLine("}");
            ScriptManager.RegisterStartupScript(Page, GetType(), "Clear_" + txtCalendar.ClientID, sb.ToString(), true);

            ibtnClear.OnClientClick = "Clear_" + txtCalendar.ClientID + "(); return false;";

            if (!IsPostBack)
            {
                //HttpBrowserCapabilities hbc = Request.Browser;
                //if (hbc.Browser.ToUpper() == "CHROME")
                //{
                //    txtCalendar.TextMode = TextBoxMode.Date;
                //    txtCalendar.Width = 120;
                //    ibtnCalendar.Visible = false;
                //    ceCalendar.Enabled = false;
                //    MaskedEditExtender.Enabled = false;
                //}

                //由Server端指定JS的Method，避免在aspx無法動態組Method Name造成Function名字衝突的情況
                //ceCalendar.OnClientDateSelectionChanged = this.ClientID + "_txtCalendar_OnClientDateSelectionChanged";

                //ScriptManager.RegisterStartupScript(this, GetType(), this.ClientID, "alert('" + btnDateSelectedEvent. +"');", true);
            }
        }

        public DateTime GetDate()
        {
            DateTime theSelectedDate;
            if (!DateTime.TryParse(txtCalendar.Text, out theSelectedDate))
            {
                throw new Exception("輸入之日期格式不正確！");
            }

            return theSelectedDate;
        }

        public void SetDate(DateTime theDate)
        {
            txtCalendar.Text = theDate.ToString("yyyy\\/MM\\/dd");
        }

        /// <summary>
        /// (來文登打頁面)收文日期加速別的計算由JS端完成，此Method用於設定Fuction
        /// </summary>
        /// <param name="strJSFunctionName">JavaScript端處理的Function Name</param>
        public void SetDateSelectionJSHandler(string strJSFunctionName)
        {
            ceCalendar.OnClientDateSelectionChanged = strJSFunctionName;
        }
    }
}