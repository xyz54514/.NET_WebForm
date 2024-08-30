using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.DynamicData;
using System.Web.UI;
using System.Web.UI.WebControls;
using Business;
using Common;

namespace WebForm.Form
{
    public partial class myBatch_import : basePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnFile_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, GetType(), "OpenFileManager", $"showBrowseDialog('{fuFile.UniqueID}');", true);
        }

        protected void btnExcel_Import_Click(object sender, EventArgs e)
        {
            try
            {
                // 獲取上傳的路徑與儲存檔案
                string filePath = CheckAndSaveFile("Excel");

                // 如果沒有選擇檔案，則中斷操作
                if (filePath == string.Empty)
                    return;

                //取檔案
                FileInfo file = new FileInfo(filePath);

                //Excel轉DataSet
                DataSet ds = EPPlus.Excel2DataSet2(file);

                //寫入資料庫
                StoreBiz objStoreBiz = new StoreBiz();

                //有幾頁做幾次
                foreach (DataTable dt in ds.Tables)
                    objStoreBiz.InsertBySqlBulkCopy(dt);

                //刪除暫存檔案
                file.Delete();

                //提示訊息
                base.DoAlertinAjax(this.Page, "Message", "上傳成功！");
            }
            catch (Exception ex)
            {
                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();

                //傳入className、methodName、exception供記錄用
                objLogExpBiz.InsertLogExp("Batch_Import", "btnExcel_Import_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;
            }
        }

        protected void btnCSV_Import_Click(object sender, EventArgs e)
        {
            try
            {
                // 獲取上傳的路徑與儲存檔案
                string filePath = CheckAndSaveFile("CSV");

                // 如果沒有選擇檔案，則中斷操作
                if (filePath == string.Empty)
                    return;

                // 轉成DataTable
                DataTable dt = CSV2DataTable(filePath);

                // 寫入至資料庫
                StoreBiz objStoreBiz = new StoreBiz();
                objStoreBiz.InsertBySqlBulkCopy(dt);

                // 取檔案
                FileInfo file = new FileInfo(filePath);

                // 刪除檔案
                file.Delete();

                // 提示訊息
                base.DoAlertinAjax(this.Page, "Message", "上傳成功！");
            }
            catch (Exception ex)
            {
                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();

                //傳入className、methodName、exception供記錄用
                objLogExpBiz.InsertLogExp("Batch_Import", "btnCSV_Import_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;
            }
        }

        protected void btnTelegram_Import_Click(object sender, EventArgs e)
        {
            try
            {
                // 獲取上傳的路徑與儲存檔案
                string filePath = CheckAndSaveFile("TELEGRAM");

                // 如果沒有選擇檔案，則中斷操作
                if (filePath == string.Empty)
                    return;

                // 轉成DataTable
                DataTable dt = Telegram2DataTable(filePath);

                // 寫入資料至資料庫
                StoreBiz objStoreBiz = new StoreBiz();
                objStoreBiz.InsertBySqlBulkCopy(dt);

                // 取檔案
                FileInfo file = new FileInfo(filePath);

                // 刪除檔案
                file.Delete();

                // 提示訊息
                base.DoAlertinAjax(this.Page, "Message", "上傳成功！");
            }
            catch (Exception ex)
            {
                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();

                //傳入className、methodName、exception供記錄用
                objLogExpBiz.InsertLogExp("Batch_Import", "btnTelegram_Import_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;
            }
        }

        // 檢查與寫入檔案
        protected string CheckAndSaveFile(string type)
        {
            //確認有無選擇檔案
            if (!fuFile.HasFile)
            {
                base.DoAlertinAjax(this.Page, "Message", "請先選擇檔案！");
                return string.Empty;
            }

            // 獲取上傳的文件名
            string fileName = Path.GetFileName(fuFile.FileName);

            // 獲取上傳文件的擴展名(檔名)
            string fileExtension = Path.GetExtension(fileName);

            // 判斷檔案類型是否錯誤
            if (type == "Excel" && !(fileExtension == ".xlsx" || fileExtension == ".xls"))
            {
                base.DoAlertinAjax(this.Page, "Message", "請選擇excel檔！");
                return string.Empty;
            }

            // 如果沒有選擇檔案，則中斷操作
            if (type == "CSV" && fileExtension != ".csv")
            {
                base.DoAlertinAjax(this.Page, "Message", "請選擇CSV檔！");
                return string.Empty;
            }

            // 如果沒有選擇檔案，則中斷操作
            // 在此以.txt取字串的方法製作
            // 主要是對字串做讀取
            if (type == "TELEGRAM" && fileExtension != ".txt")
            {
                base.DoAlertinAjax(this.Page, "Message", "請選擇txt檔！");
                return string.Empty;
            }

            // 獲取上傳文件的保存路徑
            string folderPath = ConfigurationManager.AppSettings["TempFolderPath"].ToString();

            // 如果資料夾不存在創建一個
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            //檔案位置(包含檔名+附檔名)
            string filePath;

            //生成唯一的文件名，以防止重複
            //重複產生直到無重複檔名
            do
            {
                //利用Guid作為檔名 guid可產生亂數
                string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;

                //組合路徑
                filePath = folderPath + uniqueFileName;
            } while (File.Exists(filePath)); //檢查是否重複檔名

            //保存文件到伺服器
            fuFile.SaveAs(filePath);

            //確認是否保存成功
            if (!File.Exists(filePath))
            {
                base.DoAlertinAjax(this.Page, "Message", "檔案儲存失敗！");
                return string.Empty;
            }

            return filePath;
        }

        //轉換檔案內容成DataTable
        protected DataTable CSV2DataTable(string filePath)
        {
            // 創建一個DataTable
            DataTable dt = new DataTable();

            try
            {
                //利用TextFieldParser開啟CSV文件 
                using (TextFieldParser csvReader = new TextFieldParser(filePath))
                {
                    // 設定分隔符號為","
                    csvReader.SetDelimiters(new string[] { "," });

                    // 設定是否允許引號括起來的資料內包含分隔符號
                    csvReader.HasFieldsEnclosedInQuotes = true;

                    // 讀取CSV文件的第一行作為表頭
                    string[] headers = csvReader.ReadFields();

                    // 添加表頭到DataTable
                    foreach (string header in headers)
                    {
                        dt.Columns.Add(header);
                    }

                    // 讀取CSV文件的其餘行並添加到DataTable
                    while (!csvReader.EndOfData)
                    {
                        //取出一行
                        string[] fields = csvReader.ReadFields();

                        //複製DataTable一行的架構
                        DataRow dataRow = dt.NewRow();

                        //取出各欄資料
                        for (int i = 0; i < fields.Length; i++)
                        {
                            dataRow[i] = fields[i];
                        }

                        //新增至DataTable
                        dt.Rows.Add(dataRow);
                    }
                }
            }
            catch (Exception ex)
            {
                base.DoAlertinAjax(this.Page, "Message", "讀取CSV文件時出錯!");
                throw ex;
            }

            #region 修改datatable 資料

            dt.Columns.Add(new DataColumn("CreateDate2", typeof(DateTime)));

            // 将 OriginalColumn 转换为 DateTime，并将结果复制给 TargetColumn
            foreach (DataRow row in dt.Rows)
            {
                // 获取原始列的值
                string originalValue = row["CreateDate"].ToString();

                // 尝试将字符串转换为 DateTime
                if (DateTime.TryParse(originalValue, out DateTime result))
                {
                    // 将转换后的 DateTime 存储到目标列
                    row["CreateDate2"] = result;
                }
                else
                {
                    // 处理无法转换的情况
                    Console.WriteLine($"Unable to convert value: {originalValue}");
                }
            }

            dt.Columns.Remove("CreateDate");

            dt.Columns[dt.Columns.Count - 1].ColumnName = "CreateDate";

            #endregion

            return dt;
        }

        protected DataTable Telegram2DataTable(string filePath)
        {
            // 創建一個DataTable
            DataTable dt = new DataTable();

            // 添加 DataTable 的各列（Columns）
            dt.Columns.Add("StoreName", typeof(string));
            dt.Columns.Add("Address", typeof(string));
            dt.Columns.Add("Telephone", typeof(string));
            dt.Columns.Add("Mobile", typeof(string));
            dt.Columns.Add("Remark", typeof(string));
            dt.Columns.Add("CreateDate", typeof(DateTime));

            //利用StreamReader讀取指定檔案
            using (StreamReader reader = new StreamReader(filePath))
            {
                // 當還有資料未讀取時繼續讀取
                while (!reader.EndOfStream)
                {
                    // 讀取下一行
                    string rows = reader.ReadLine();

                    // 轉換成byte[]
                    byte[] b = Encoding.GetEncoding("Big5").GetBytes(rows);

                    // 複製DataTable的架構
                    DataRow dataRow = dt.NewRow();

                    // 取出各欄資料
                    dataRow["StoreName"] = Encoding.GetEncoding("Big5").GetString(b, 0, 10).Trim();
                    dataRow["Address"] = Encoding.GetEncoding("Big5").GetString(b, 10, 10).Trim();
                    dataRow["Telephone"] = Encoding.GetEncoding("Big5").GetString(b, 20, 10).Trim();
                    dataRow["Mobile"] = Encoding.GetEncoding("Big5").GetString(b, 30, 10).Trim();
                    dataRow["Remark"] = Encoding.GetEncoding("Big5").GetString(b, 40, 10).Trim();
                    dataRow["CreateDate"] = Encoding.GetEncoding("Big5").GetString(b, 50, 23).Trim();

                    // 添加至DataTable
                    dt.Rows.Add(dataRow);
                }
            }

            return dt;
        }
    }
}