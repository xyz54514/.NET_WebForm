using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security.AntiXss;
using System.Web.UI;
using System.Web.UI.WebControls;
using Business;
using Common;
using static Common.NPOIExport_2007;

namespace WebForm.Form
{
    public partial class myExport : basePage
    {
        private const int igvListPageCount = 10;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    hdStoreName.Value = string.Empty;
                    hdAddress.Value = string.Empty;
                    hdSDate.Value = null;
                    hdEDate.Value = null;

                    this.gvListBind();
                    this.BindAddressLevel(ddlAddress);
                }
            }
            catch (Exception ex)
            {
                LogExpBiz objLogExpBiz = new LogExpBiz();

                objLogExpBiz.InsertLogExp("myExport_Excel", "Page_Load", ex);
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常，請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;
            }
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            try 
            {
                string checkMsg = FieldCheck();

                if (!String.IsNullOrEmpty(checkMsg))
                {
                    base.DoAlertinAjax(this.Page, "msg", checkMsg);
                    return;
                }

                hdStoreName.Value = txtStoreName.Text.Trim();
                hdAddress.Value = ddlAddress.SelectedValue;
                hdSDate.Value = ucStartDate.Text.ToString();
                hdEDate.Value = ucEndDate.Text.ToString();

                gvListBind();
            }
            catch (Exception ex)
            {
                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();
                objLogExpBiz.InsertLogExp("Export_Excel", "btnQuery_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;
            }
        }

        protected void BindAddressLevel(DropDownList ddl)
        {
            DataDictionaryBiz objDataDictionaryBiz = new DataDictionaryBiz();

            DataTable typeList = objDataDictionaryBiz.InqDataDictionary("sAddress");

            ddl.DataSource = typeList;
            ddl.DataTextField = "TextField";
            ddl.DataValueField = "ValueField";
            ddl.DataBind();

            ddl.Items.Insert(0, new ListItem("請選擇", string.Empty));
            ddl.SelectedIndex = 0;
        }

        protected void Pagination(int rowCount, ref int tStartRow, ref int tEndRow)
        {
            int tTotalPage = 1;
            if (rowCount != 0)
            {
                if ((Convert.ToInt32(rowCount) % igvListPageCount) == 0)
                {
                    tTotalPage = (rowCount / igvListPageCount);
                }
                else
                {
                    tTotalPage = (rowCount / igvListPageCount) + 1;
                }

                hdTotalPage.Value = tTotalPage.ToString();

                int tPageIndex;
                if (int.TryParse(hdPageIndex.Value, out tPageIndex) == false)
                {
                    tPageIndex = 1;
                    hdPageIndex.Value = tPageIndex.ToString();
                }

                //比較所在頁數是否大於總頁數
                if (tPageIndex > tTotalPage)
                {
                    tPageIndex = tTotalPage;
                    hdPageIndex.Value = tPageIndex.ToString();
                }

                //計算資料範圍，取得當前分頁需要的行數是第幾到第幾行
                FormCommon.CalDataSAndE(tPageIndex, igvListPageCount, ref tStartRow, ref tEndRow);
            }
        }

        protected void gvListButtonSet(bool visible, int rowCount)
        {
            //若有資料則顯示分頁畫面，反之則不顯示
            if (!visible)
            {
                btnFirst.Visible = false;
                btnPre.Visible = false;
                btnNext.Visible = false;
                btnLast.Visible = false;
                lblPager.Visible = false;
            }
            else
            {
                btnFirst.Visible = true;
                btnPre.Visible = true;
                btnNext.Visible = true;
                btnLast.Visible = true;
                lblPager.Visible = true;
            }

            //設定Button Enabled
            if (hdTotalPage.Value == "1")
            {
                btnFirst.Enabled = false;
                btnPre.Enabled = false;
                btnNext.Enabled = false;
                btnLast.Enabled = false;
            }
            else if (hdPageIndex.Value.Trim() == "1")
            {
                btnFirst.Enabled = false;
                btnPre.Enabled = false;
                btnNext.Enabled = true;
                btnLast.Enabled = true;
            }
            else if (hdTotalPage.Value == hdPageIndex.Value.Trim())
            {
                btnFirst.Enabled = true;
                btnPre.Enabled = true;
                btnNext.Enabled = false;
                btnLast.Enabled = false;
            }
            else
            {
                btnFirst.Enabled = true;
                btnPre.Enabled = true;
                btnNext.Enabled = true;
                btnLast.Enabled = true;
            }
            lblPager.Text = string.Format($"第 {hdPageIndex.Value}/{hdTotalPage.Value} 頁.共{rowCount}筆");
        }

        protected string FieldCheck()
        {
            StringBuilder sb = new StringBuilder();
            DateTime SDate;
            DateTime EDate;
            bool SDateValid = DateTime.TryParse(ucStartDate.Text, out SDate);
            bool EDateValid = DateTime.TryParse(ucEndDate.Text, out EDate);

            //判斷Name的關鍵字是否超過50個字元
            if (txtStoreName.Text.Trim().Length > 50)
            {
                sb.Append("StoreName的字數不能超過50個字！\\n");
            }

            if (!string.IsNullOrEmpty(ucStartDate.Text) && SDateValid == false)
            {
                sb.Append("起始日期格式錯誤\\n");
            }

            if (!string.IsNullOrEmpty(ucEndDate.Text) && EDateValid == false)
            {
                sb.Append("結束日期格式錯誤\\n");
            }

            if ((SDateValid && EDateValid) && SDate > EDate)
            {
                sb.Append("起始日期不能晚於結束日期\\n");
            }

            //回傳錯誤訊息
            return sb.ToString();
        }

        #region Export Function

        /// <summary>
        /// 整理需匯出的欄位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected Dictionary<string, string> getColMapping()
        {
            //StringComparer.OrdinalIgnoreCase用於尋找ContainsKey時忽略大小寫差異
            Dictionary<string, string> dColMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            //左邊為ContainsKey 右邊為Excel輸出Header名稱
            dColMapping.Add("StoreID", "商店ID");
            dColMapping.Add("StoreName", "商店名稱");
            dColMapping.Add("Address", "地址");
            dColMapping.Add("CreateDate", "創建時間");

            return dColMapping;
        }

        /// <summary>
        /// DataTable欄位整理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ExportTableSet(DataTable dt)
        {
            Dictionary<string, string> dColMapping = getColMapping();

            //要排除的Columns
            List<string> lRemoveColumns = new List<string>();

            foreach (DataColumn dc in dt.Columns)
            {
                //檢查Table Column有沒有對應到Dictionary中所設定的
                if (dColMapping.ContainsKey(dc.ColumnName))
                {
                    //將Table的ColumnName做為Key去Dictionary抓對應的Value
                    dc.ColumnName = dColMapping[dc.ColumnName];
                }
                else
                {
                    //將Table多出的Column存入List做紀錄
                    lRemoveColumns.Add(dc.ColumnName);
                }
            }

            //移除不需顯示的Table欄位
            foreach (string sRemoveColumns in lRemoveColumns.ToArray())
            {
                //檢查Table Column有沒有對應List中所存放之Column
                if (dt.Columns.Contains(sRemoveColumns))
                {
                    //排除Table的該行Column
                    dt.Columns.Remove(sRemoveColumns);
                }
            }
        }

        protected void btnExpExcel_Click(object sender, EventArgs e)
        {
            try
            {
                #region 變數處理

                string StoreName = hdStoreName.Value;
                string Address = hdAddress.Value;
                DateTime? SDate = null;
                DateTime? EDate = null;

                if (!string.IsNullOrEmpty(hdSDate.Value))
                {
                    SDate = Convert.ToDateTime(hdSDate.Value);
                }
                if (!string.IsNullOrEmpty(hdEDate.Value))
                {
                    EDate = Convert.ToDateTime(hdEDate.Value).AddDays(1).AddMilliseconds(-1);
                }

                #endregion

                #region 取得匯出資料

                StoreBiz objStoreBiz = new StoreBiz();
                DataTable dtExcelData = objStoreBiz.ExpStore(StoreName, Address, SDate, EDate);

                #endregion

                #region 下載

                //判斷Table是否有資料
                if (dtExcelData != null && dtExcelData.Rows.Count > 0)
                {
                    //DataTable欄位整理(篩選)
                    ExportTableSet(dtExcelData);

                    MemoryStream ms = EPPlus.DateTableExport(dtExcelData, Page.Title);
                    DownloadFile(ms, "Excel_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");
                }

                #endregion
            }
            catch (Exception ex)
            {
                #region 紀錄Log

                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();
                objLogExpBiz.InsertLogExp("Export", "btnExpExcel_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;

                #endregion
            }
        }

        #endregion

        #region CSV Export Function

        /// <summary>
        /// 檢查資料是否包含特定符號，包含則進行資料處理
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected string CsvDataSet(string data)
        {
            //是否包含逗號、雙引號、換行符、前後是否為空白
            bool Check = data.Contains(",") || data.Contains("\"") ||
                         data.Contains("\n") || data.Trim() != data;

            if (Check)
            {
                //將單個雙引號Replace為兩個雙引號
                data = data.Replace("\"", "\"\"");

                //字串前後添加雙引號
                data = "\"" + data + "\"";
            }

            return data;
        }

        /// <summary>
        /// 輸出檔案至
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        protected byte[] ExportCsv(DataTable dt)
        {
            byte[] tFile = new byte[] { };

            //DataTable欄位整理
            ExportTableSet(dt);

            if (dt != null && dt.Rows.Count > 0)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (StreamWriter sw = new StreamWriter(ms, Encoding.Default)) //Encoding.Default避免亂碼
                    {
                        #region 寫入Column資料

                        foreach (DataColumn column in dt.Columns)
                        {
                            sw.Write($"{CsvDataSet(column.ColumnName)}");
                            if (column.Ordinal < dt.Columns.Count - 1) //最後一筆不加逗號
                            {
                                sw.Write(",");
                            }
                        }
                        sw.WriteLine();

                        #endregion

                        #region 寫入Row資料

                        foreach (DataRow row in dt.Rows)
                        {
                            for (int i = 0; i < dt.Columns.Count; i++)
                            {
                                sw.Write($"{CsvDataSet(row[i].ToString())}");
                                if (i < dt.Columns.Count - 1)
                                {
                                    sw.Write(",");
                                }
                            }
                            sw.WriteLine();
                        }

                        #endregion

                        // 確保所有資料都寫入MemoryStream中
                        //sw.Flush();
                    }
                    tFile = ms.ToArray();
                }
            }

            return tFile;
        }

        protected void btnExpCsv_Click(object sender, EventArgs e)
        {
            try
            {
                #region 變數處理

                string StoreName = hdStoreName.Value;
                string Address = hdAddress.Value;
                DateTime? SDate = null;
                DateTime? EDate = null;

                if (!string.IsNullOrEmpty(hdSDate.Value))
                {
                    SDate = Convert.ToDateTime(hdSDate.Value);
                }
                if (!string.IsNullOrEmpty(hdEDate.Value))
                {
                    EDate = Convert.ToDateTime(hdEDate.Value).AddDays(1).AddMilliseconds(-1);
                }

                #endregion

                #region 取得匯出資料

                StoreBiz objStoreBiz = new StoreBiz();
                DataTable dtCsvData = objStoreBiz.ExpStore(StoreName, Address, SDate, EDate);

                #endregion

                #region 下載

                //判斷Table是否有資料
                if (dtCsvData != null && dtCsvData.Rows.Count > 0)
                {
                    byte[] bFile = ExportCsv(dtCsvData);
                    DownloadFile(new MemoryStream(bFile), "Csv_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv");
                }

                #endregion
            }
            catch (Exception ex)
            {
                #region 紀錄Log

                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();
                objLogExpBiz.InsertLogExp("Export", "btnExpCSV_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;

                #endregion
            }
        }

        #endregion



        protected void btnFirst_Click(object sender, EventArgs e)
        {
            hdPageIndex.Value = "1";
            gvListBind();
        }

        protected void btnPre_Click(object sender, EventArgs e)
        {
            int tPageInedex = Convert.ToInt32(hdPageIndex.Value);
            tPageInedex--;
            hdPageIndex.Value = tPageInedex.ToString();
            gvListBind();
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            int tPageInedex = Convert.ToInt32(hdPageIndex.Value);
            tPageInedex++;
            hdPageIndex.Value = tPageInedex.ToString();
            gvListBind();
        }

        protected void btnLast_Click(object sender, EventArgs e)
        {
            hdPageIndex.Value = AntiXssEncoder.HtmlEncode(hdTotalPage.Value, true);
            gvListBind();
        }

        protected void gvListBind()
        {
            string StoreName = hdStoreName.Value;
            string Address = hdAddress.Value;
            DateTime? SDate = null;
            DateTime? EDate = null;

            if (!string.IsNullOrEmpty(hdSDate.Value))
            {
                SDate = Convert.ToDateTime(hdSDate.Value);
            }

            if (!string.IsNullOrEmpty(hdEDate.Value))
            {
                EDate = Convert.ToDateTime(hdEDate.Value);
            }

            StoreBiz objStoreBiz = new StoreBiz();
            int rowCount = objStoreBiz.InqStoreCount2(StoreName, Address, SDate, EDate);

            int tStartRow = 0;
            int tEndRow = 0;

            this.Pagination(rowCount, ref tStartRow, ref tEndRow);

            DataTable dt = objStoreBiz.InqStore2(StoreName, Address, SDate, EDate, tStartRow, tEndRow);

            gvList.DataSource = dt;
            gvList.DataBind();

            this.gvListButtonSet((dt == null || dt.Rows.Count == 0) ? false : true, rowCount);
        }
    }
}