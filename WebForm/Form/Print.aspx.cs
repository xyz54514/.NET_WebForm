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
    public partial class Print : basePage
    {
        #region Variable
        private string PDF_tmplPath = ConfigurationManager.AppSettings["PDF_tmplPath"].ToString();
        private string WORD_tmplPath = ConfigurationManager.AppSettings["WORD_tmplPath"].ToString();
        private string TempFolderPath = ConfigurationManager.AppSettings["TempFolderPath"].ToString();
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {

        }

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

                //使用套件
                byte[] bFile = objPrintPDFBiz.DoPrint(PDF_tmplPath, dicValue);
                DownloadFile(new MemoryStream(bFile), "PDF_output_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf");
            }
            //C:\Users\Andrew\Downloads\WebForm_20240625\WebForm\Template
            catch (Exception ex)
            {
                #region 紀錄Log

                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();
                objLogExpBiz.InsertLogExp("Export", "btnPDF_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;

                #endregion
            }

        }

        protected void btnWord_Click(object sender, EventArgs e)
        {
            try 
            {
                //填入被取代的值和取代的值
                csOpenXML objOpenXML = new csOpenXML();
                Dictionary<string, string> dicValue = new Dictionary<string, string>() { };
                dicValue.Add("[DATE]", txtDate.Text);
                dicValue.Add("[CUSTNAME]", txtCustName.Text);
                dicValue.Add("[ADDRESS]", txtAddress.Text);
                dicValue.Add("[ACCOUNT_NO]", txtNo.Text);
                dicValue.Add("[REMARK]", txtRemark.Text);

                //暫存路徑加檔名
                string WORD_outputPath = TempFolderPath + "\\WORD_output" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".docx";
                
                //該路徑若不存在，建立路徑
                if (!Directory.Exists(TempFolderPath))
                    Directory.CreateDirectory(TempFolderPath);

                //複製檔案
                if (System.IO.File.Exists(WORD_tmplPath))
                {
                    System.IO.File.Copy(WORD_tmplPath, WORD_outputPath, true);
                }
                
                //使用套件
                objOpenXML.WordReplace(WORD_outputPath, dicValue);
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
    }
}