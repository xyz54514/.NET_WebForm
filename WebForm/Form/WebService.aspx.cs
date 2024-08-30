using WebForm.BMI;
using System.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Business;

namespace WebForm.Form
{
    public partial class WebService : basePage
    {
        #region Page Function

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    //BMISoapClient BMI = new BMISoapClient();
                    //EndpointAddress address = new EndpointAddress(@"https://localhost:44343/WebService/BMI.asmx");
                    //BMI.Endpoint.Address = address;
                }
            }
            catch (Exception ex)
            {
                #region 紀錄Log

                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();

                //傳入className、methodName、exception供記錄用
                objLogExpBiz.InsertLogExp("WebService", "Page_Load", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;

                #endregion
            }
        }

        #endregion

        #region Button Function

        protected void btnCal_Click(object sender, EventArgs e)
        {
            try
            {
                #region 欄位檢核

                //判斷輸入欄位是否格式不符
                string checkMsg = FieldCheck();

                //如有錯誤彈出錯誤訊息，並返回頁面
                if (!string.IsNullOrEmpty(checkMsg))
                {
                    //利用basePage的DoAlertinAjax彈出提示訊息給使用者
                    base.DoAlertinAjax(this.Page, "msg", checkMsg);
                    return;
                }

                #endregion

                string Result = string.Empty;
                double Height = Convert.ToDouble(txtHeight.Text.Trim());
                double Weight = Convert.ToDouble(txtWeight.Text.Trim());
                BMISoapClient BMI = new BMISoapClient();

                //傳回的資料為BMI數值,體態狀況 Ex : 22,正常
                Result = BMI.BMICal(Height, Weight);

                string sBMI = Result.Split(',')[0];
                string sStatus = Result.Split(',')[1];

                //彈出提示訊息給使用者
                base.DoAlertinAjax(this.Page, "msg", $"計算結果如下\\nBMI：{sBMI}\\n體態：{sStatus}");
            }
            catch (Exception ex)
            {
                #region 紀錄Log

                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();
                objLogExpBiz.InsertLogExp("WebService", "btnCal_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;

                #endregion
            }
        }

        #endregion

        #region 自定義Function

        /// <summary>
        /// 檢核是否為數字
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected bool IsNumeric(string input)
        {
            // 使用正規表達式檢查是否只包含數字及小數點
            Regex regex = new Regex(@"^[0-9]*\.?[0-9]+$");
            return regex.IsMatch(input);
        }

        /// <summary>
        ///  檢查關鍵字欄位是否錯誤
        /// </summary>
        /// <returns></returns>
        protected string FieldCheck()
        {
            StringBuilder sb = new StringBuilder();

            if (string.IsNullOrEmpty(txtHeight.Text.Trim()))
            {
                sb.Append("Height不可為空！\\n");
            }
            else if (!IsNumeric(txtHeight.Text.Trim()))
            {
                sb.Append("Height不可為非數字！\\n");
            }

            if (string.IsNullOrEmpty(txtWeight.Text.Trim()))
            {
                sb.Append("Weight不可為空！\\n");
            }
            else if (!IsNumeric(txtWeight.Text.Trim()))
            {
                sb.Append("Weight不可為非數字！\\n");
            }

            //回傳錯誤訊息
            return sb.ToString();
        }

        #endregion
    }
}