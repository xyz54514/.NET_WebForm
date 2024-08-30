using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Information.CustomerAPIInfo;
using System.Xml.Linq;
using Business;
using Common;

namespace WebForm.Form
{
    public partial class WebAPI : basePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnRequest_Click(object sender, EventArgs e)
        {
            // 從 config 取得 API 連接 URL
            string apiURL = ConfigurationManager.AppSettings["CustomerApiUrl"].ToString();

            // 建立 WebAPIClient 實例，用於呼叫 Web Service
            WebAPIClient api = new WebAPIClient(apiURL, "", "");

            // 建立客戶請求物件
            Customer_SendRequest cInputValue = new Customer_SendRequest();

            // 建立客戶回應物件
            Customer_SendResponse cOutValue = new Customer_SendResponse();

            // 從 UI 取得使用者輸入的 CID
            cInputValue.CID = txtCID.Text.Trim();

            // 建立字典物件，用於將輸入值轉換成 JSON 格式
            Dictionary<string, object> dInputValue = new Dictionary<string, object>();

            // 用於儲存 Web Service 回傳的 JSON 字串
            string sOutValue;

            try
            {
                // 將 CID 加入字典
                dInputValue.Add("CID", cInputValue.CID);

                // 呼叫 Web API，傳送 JSON 字串並取得回應
                sOutValue = api.CallWebAPI("Employee", JsonConvert.SerializeObject(dInputValue));

                // 將回應的 JSON 字串轉換成客戶回應物件
                cOutValue = JsonConvert.DeserializeObject<Customer_SendResponse>(sOutValue);
            }
            catch (System.Net.WebException ex)
            {
                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();

                //傳入className、methodName、exception供記錄用
                objLogExpBiz.InsertLogExp("WebAPI", "btnRequest_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;
            }
            catch (Exception ex)
            {
                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();

                //傳入className、methodName、exception供記錄用
                objLogExpBiz.InsertLogExp("WebAPI", "btnRequest_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;
            }

            // 若 Web Service 回應結果為失敗，顯示相應錯誤訊息
            if (!cOutValue.Result)
            {
                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "Msg", "alert('無相關資料');", true);
                return;
            }

            txtJson.Text = sOutValue;

            // 將 Web Service 回傳的客戶資料填入 UI 中
            txtName.Text = cOutValue.Name;
            txtCity.Text = cOutValue.City;
            txtPhone.Text = cOutValue.Phone;
            txtType.Text = cOutValue.Type;
        }
    }
}