using iTextSharp.text.pdf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Business;
using Common;
using static iTextSharp.text.pdf.PdfSigGenericPKCS;
using static System.Net.WebRequestMethods;

namespace WebForm.Form
{
    public partial class myPrint : basePage
    {
        //private string myPDF_tmplPath = ConfigurationManager.AppSettings["myPDF_tmplPath"].ToString();
        private string myWORD_tmplPath = ConfigurationManager.AppSettings["myWORD_tmplPath"].ToString();
        private string TempFolderPath = ConfigurationManager.AppSettings["TempFolderPath"].ToString();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /*
        protected void btnPDF_Click(object sender, EventArgs e)
        {
            try
            {
                //填入被取代的值和取代的值
                PrintPDFBiz objPrintPDFBiz = new PrintPDFBiz();
                Dictionary<string, string> dicValue = new Dictionary<string, string>() { };
                dicValue.Add("[DATE]", txtDate.Text);
                dicValue.Add("[CUSTNAME]", txtCustName.Text);
                dicValue.Add("[ADDRESS]", txtAddress.Text);
                dicValue.Add("[ACCOUNT_NO]", txtNo.Text);
                dicValue.Add("[REMARK]", txtRemark.Text);
                dicValue.Add("[Comment]", txtComment.Text);

                //使用套件
                byte[] bFile = objPrintPDFBiz.DoPrint(myPDF_tmplPath, dicValue);
                DownloadFile(new MemoryStream(bFile), "PDF_output_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf");
            }
            catch (Exception ex)
            {
                LogExpBiz objLogExpBiz = new LogExpBiz();
                objLogExpBiz.InsertLogExp("Export", "btnPDF_Click", ex);
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
            }
        }
        */

        protected void btnWord_Click(object sender, EventArgs e)
        {
            try
            {
                //填入被取代的值和取代的值
                mycsOpenXML objOpenXML = new mycsOpenXML();
                Dictionary<string, string> dicValue = new Dictionary<string, string>() { };
                dicValue.Add("[DATE]", txtDate.Text);
                dicValue.Add("[CUSTNAME]", txtCustName.Text);
                dicValue.Add("[ADDRESS]", txtAddress.Text);
                dicValue.Add("[ACCOUNT_NO]", txtNo.Text);
                dicValue.Add("[REMARK]", txtRemark.Text);
                dicValue.Add("[comment]", txtComment.Text);

                Dictionary<string, string> dicPicture = new Dictionary<string, string>() { };
                //string picPath = Server.MapPath("~/UploadedImages/") + picUpload.FileName;

                //dicValue.Add("[picture]", picPath);
                //dicPicture.Add("[picture]", picPath);

                //暫存路徑加檔名
                string WORD_outputPath = TempFolderPath + "\\WORD_output" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".docx";

                //該路徑若不存在，建立路徑
                if (!Directory.Exists(TempFolderPath))
                    Directory.CreateDirectory(TempFolderPath);

                //複製檔案
                if (System.IO.File.Exists(myWORD_tmplPath))
                {
                    System.IO.File.Copy(myWORD_tmplPath, WORD_outputPath, true);
                }

                //使用套件
                objOpenXML.WordReplace(WORD_outputPath, dicValue);
                //objOpenXML.InsertPicture(WORD_outputPath, dicPicture);
                DownloadFile(new MemoryStream(System.IO.File.ReadAllBytes(WORD_outputPath)), "WORD_output_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".docx");
            }
            catch (Exception ex)
            {
                #region 紀錄Log

                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();
                objLogExpBiz.InsertLogExp("Export", "btnWord_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;

                #endregion
            }
        }

        //protected void btnUpload_Click(object sender, EventArgs e)
        //{
        //    if (picUpload.HasFile)
        //    {
        //        try
        //        {
        //            if (picUpload.PostedFile.ContentType.ToLower().StartsWith("image/"))
        //            {
        //                // 設置圖片預覽
        //                imgPreview.ImageUrl = "~/UploadedImages/" + picUpload.FileName;
        //                // 保存文件到指定文件夾
        //                string picPath = Server.MapPath("~/UploadedImages/") + picUpload.FileName;
        //                picUpload.SaveAs(picPath);
        //            }
        //            else
        //            {
        //                // 非圖片文件的處理
        //                // 可以顯示錯誤信息給用戶
        //                base.DoAlertinAjax(this.Page, "msg", "請上傳圖片格式!");
        //                return;
        //            }
        //            // 取得上傳檔案的名稱和路徑

        //            /*
        //            string filename = Path.GetFileName(fileUpload.FileName);
        //            string path = Server.MapPath("~/UploadedImages/") + filename;

        //            // 將檔案存儲在伺服器上指定的路徑
        //            fileUpload.SaveAs(path);

        //            // 提示上傳成功
        //            Response.Write("Upload status: File uploaded successfully!");
        //            */
        //        }
        //        catch (Exception ex)
        //        {
        //            // 提示上傳失敗
        //            base.DoAlertinAjax(this.Page, "msg", "上傳失敗!");
        //            return;
        //        }
        //    }
        //    else
        //    {
        //        // 如果沒有選擇檔案，提示用戶選擇檔案
        //        //Response.Write("Please select a file to upload.");
        //        //StringBuilder sb = new StringBuilder();
        //        base.DoAlertinAjax(this.Page, "msg", "未輸入圖片!");
        //        return;
        //    }
        //}
    }
}