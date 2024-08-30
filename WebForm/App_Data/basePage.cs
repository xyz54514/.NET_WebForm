using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace WebForm
{
    /// <summary>
    /// 網頁基底類別
    /// </summary>
    public partial class basePage : System.Web.UI.Page  //共用功能
    {
        public enum Mode { Nomal, AddNew, Modify, View, Delete, Print, Default, Save, Send, Referral, Reply };

        protected string strUserID
        {
            get
            {
                return ConfigurationManager.AppSettings["UserID"].ToString();
            }
        }

        protected string FormID
        {
            get
            {
                return "";
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;

            base.OnLoad(e);
        }

        /// </summary>
        /// <param name="webPage">傳 this</param>
        /// <param name="key">Key</param>
        /// <param name="AlertMessage">Message</param>
        protected void DoAlertinAjax(System.Web.UI.Page webPage, string key, string AlertMessage)
        {
            ScriptManager.RegisterStartupScript(webPage, GetType(), key, "alert('" + AlertMessage.Replace("'", "\\'").Replace("\r\n", "") + "');", true);
        }

        #region 下載檔案
        public void DownloadFile(MemoryStream ms, string fileName)
        {
            Response.Clear();
            Response.ClearHeaders();
            Response.Buffer = false;
            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Response.ContentType = "application/octet-stream";
            Response.AppendHeader("Content-Length", ms.Length.ToString());
            Response.AppendHeader("Content-Disposition", "attachment;filename=\"" + HttpUtility.UrlEncode(Server.UrlDecode(fileName), System.Text.Encoding.UTF8) + "\"");


            try
            {
                long chunkSize = 102400;
                byte[] buffer = new byte[chunkSize];
                long dataToRead = ms.Length;

                while (dataToRead > 0)
                {
                    if (Response.IsClientConnected)
                    {
                        int length = ms.Read(buffer, 0, Convert.ToInt32(chunkSize));
                        Response.OutputStream.Write(buffer, 0, length);
                        Response.Flush();
                        Response.Clear();
                        dataToRead -= length;
                    }
                    else
                    {
                        //防止client失去连接
                        dataToRead = -1;
                    }
                }
                ms.Close();

            }
            catch (Exception ex)
            {
                Response.Write("Error:" + ex.Message);
            }
            finally
            {
                try
                {
                    Response.End();
                }
                catch (System.Threading.ThreadAbortException)
                {
                    // 捕捉並忽略 ThreadAbortException
                }
            }
        }
        #endregion
    }
}