using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security.AntiXss;
using System.Web.UI;
using System.Web.UI.WebControls;
using Business;
using Common;
using Information;

namespace WebForm.Form
{
    public partial class myMaster_Detail : basePage
    {
        private const int igvListPageCount = 10;

        //記錄產品資訊
        private DataTable SProductDt
        {
            //ViewState可存各型別資料，存在期間僅限在該頁面中，頁面刷新時資料仍存在
            get { return ViewState["SProductDt"] as DataTable ?? new DataTable(); }
            set { ViewState["SProductDt"] = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //將可能產生錯誤訊息的程式碼撰寫在try中，以便錯誤發生時做適當處理。
            try
            {
                //僅在剛進入時執行
                if (!IsPostBack)
                {
                    //設定預設值
                    hdStoreName.Value = string.Empty;
                    hdAddress.Value = null;
                    hdCDate.Value = null;
                    hdSDate.Value = null;
                    hdEDate.Value = null;

                    //綁訂清單
                    this.gvListBind();
                    this.BindAddressLevel(ddlAddress);
                }
            }
            //在錯誤發生時，會跳進catch並傳入錯誤訊息，在此撰寫Log進資料庫以便日後查看，同時彈出訊息給使用者
            catch (Exception ex)
            {
                #region 紀錄Log

                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();

                //傳入className、methodName、exception供記錄用
                objLogExpBiz.InsertLogExp("myMaster_Detail", "Page_Load", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;

                #endregion
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                this.ClosePanel();
                this.OpenPanel(Mode.Default);
            }
            catch (Exception ex)
            {
                #region 紀錄Log

                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();

                //傳入className、methodName、exception供記錄用
                objLogExpBiz.InsertLogExp("btnBack_Click", "btnAdd_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;

                #endregion
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                switch (hdMode.Value.ToUpper())
                {
                    case "ADD":
                        AddStore();
                        break;
                    case "MODIFY":
                        ModifyStore();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                #region 紀錄Log

                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();

                //傳入className、methodName、exception供記錄用
                objLogExpBiz.InsertLogExp("myMaster_Detail", "btnSave_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;

                #endregion
            }
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                //檢查搜尋欄位
                string checkMsg = SearchFieldCheck();

                //有錯誤跳提示
                if (!String.IsNullOrEmpty(checkMsg))
                {
                    //利用basePage的DoAlertinAjax彈出提示訊息給使用者
                    base.DoAlertinAjax(this.Page, "msg", checkMsg);
                    return;
                }

                //儲存搜尋關鍵字
                hdStoreName.Value = txtStoreName.Text.Trim();
                hdAddress.Value = ddlAddress.SelectedValue;
                hdSDate.Value = ucStartDate.Text.ToString();
                hdEDate.Value = ucEndDate.Text.ToString();

                //回第一頁
                hdPageIndex.Value = "1";

                //綁定清單
                gvListBind();
            }
            catch (Exception ex)
            {
                #region 紀錄Log

                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();

                //傳入className、methodName、exception供記錄用
                objLogExpBiz.InsertLogExp("myMaster_Detail", "btnQuery_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;

                #endregion
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                #region 初始化

                //初始化畫面
                this.ClosePanel();
                this.OpenPanel(Mode.AddNew);

                //初始化新增頁面 Master區塊
                txtMStoreName.Text = string.Empty;
                txtMAddress.Text = string.Empty;
                txtMTelephone.Text = string.Empty;
                ucMCreateDate.Text = string.Empty;
                //ucMStartDate.Text = string.Empty;
                //ucMEndDate.Text = string.Empty;

                //初始化新增頁面 Detail區塊
                gvDList.DataSource = null;
                gvDList.DataBind();

                //初始化ViewState
                DataTable dt = new DataTable();
                dt.Columns.Add("SN", typeof(int)); //流水號，對應用
                dt.Columns.Add("StoreID", typeof(int)); //為儲存時空值
                dt.Columns.Add("ProductID", typeof(string));
                dt.Columns.Add("ProductName", typeof(string));
                dt.Columns.Add("Price", typeof(string));
                dt.Columns.Add("Remark", typeof(string));
                dt.Columns.Add("Status", typeof(string));
                SProductDt = dt;

                #endregion

                //隱藏刪除按鈕
                btnMDel.Visible = false;

                //紀錄執行操作
                hdMode.Value = "Add";
            }
            catch (Exception ex)
            {
                #region 紀錄Log

                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();

                //傳入className、methodName、exception供記錄用
                objLogExpBiz.InsertLogExp("myMaster_Detail", "btnAdd_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;

                #endregion
            }
        }

        protected void btnMDelte_Click(object sender, EventArgs e)
        {
            StoreBiz objStoreBiz;
            SProductBiz objSProductBiz;
            SqlConnection CONNSEC;
            SqlTransaction myTransaction;

            #region Transaction設置

            try
            {
                objStoreBiz = new StoreBiz();
                objSProductBiz = new SProductBiz();

                //獲取DB Connection
                CONNSEC = new SqlConnection(objStoreBiz.GetConnString());

                //開啟Connection
                CONNSEC.Open();

                //開啟交易
                myTransaction = CONNSEC.BeginTransaction();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            #endregion

            try
            {
                int StoreID = Convert.ToInt32(hdStoreID.Value);

                //刪除主表Course資料
                objStoreBiz.DeleteStore(StoreID, CONNSEC, myTransaction);

                //刪除副表Student資料
                objSProductBiz.DeleteProduct(StoreID, CONNSEC, myTransaction);

                //認可本次交易
                myTransaction.Commit();

                //彈出提示訊息給使用者
                base.DoAlertinAjax(this.Page, "msg", "刪除成功！");

                //重新查詢資料
                gvListBind();
                this.BindAddressLevel(ddlAddress);
                //返回清單畫面
                this.ClosePanel();
                this.OpenPanel(Mode.Default);
            }
            catch (Exception ex)
            {
                #region 失敗Rollback

                myTransaction.Rollback();

                #endregion

                #region 紀錄Log

                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();

                //傳入className、methodName、exception供記錄用
                objLogExpBiz.InsertLogExp("Master_Detail", "btnMDelte_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;

                #endregion
            }
            finally
            {
                #region 釋放資源

                myTransaction.Dispose();

                if (CONNSEC.State == ConnectionState.Open)
                    CONNSEC.Close();

                CONNSEC.Dispose();

                #endregion
            }
        }

        protected void btnDAdd_Click(object sender, EventArgs e)
        {
            try
            {
                #region DataTable資料處理

                //DataTable是call by referance
                DataTable dtTmp = SProductDt;

                //添加新Row
                DataRow dr = dtTmp.NewRow();
                dr["Status"] = "A";
                dtTmp.Rows.Add(dr);

                //將資料寫入ViewState
                DetailTableSet(gvDList, dtTmp);

                #endregion

                //Detail清單Bind
                gvDList.DataSource = dtTmp;
                gvDList.DataBind();
            }
            catch (Exception ex)
            {
                #region 紀錄Log

                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();

                //傳入className、methodName、exception供記錄用
                objLogExpBiz.InsertLogExp("myMaster_Detail", "btnDAdd_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;

                #endregion
            }
        }

        //只控制前端???
        protected void btnDModify_Click(object sender, EventArgs e)
        {
            #region 取得Button所在的GridViewRow

            Button btn = (Button)sender;
            GridViewRow gvr = (GridViewRow)btn.NamingContainer;

            #endregion

            #region 變更背景顏色

            gvr.BackColor = System.Drawing.ColorTranslator.FromHtml("#E0E0E0");

            #endregion

            #region DataTable處理

            //取得該RowIndex
            int rowIndex = gvr.RowIndex;

            //取得ViewState DataTable
            DataTable dt = SProductDt;

            //Status欄位紀錄為M (表示修改)
            dt.Rows[rowIndex]["Status"] = "M";

            #endregion

            #region 設置控制項Visible Enable

            btn.Visible = false;

            TextBox txtProductName = (TextBox)gvr.FindControl("txtDProductName");
            txtProductName.Enabled = txtProductName.Visible = true;
            Label lblProductName = (Label)gvr.FindControl("lblDProductName");
            lblProductName.Visible = false;

            TextBox txtPrice = (TextBox)gvr.FindControl("txtDPrice");
            txtPrice.Enabled = txtPrice.Visible = true;
            Label lblPrice = (Label)gvr.FindControl("lblDPrice");
            lblPrice.Visible = false;

            TextBox txtRemark = (TextBox)gvr.FindControl("txtDRemark");
            txtRemark.Enabled = txtRemark.Visible = true;
            Label lblRemark = (Label)gvr.FindControl("lblDRemark");
            lblRemark.Visible = false;

            #endregion
        }

        protected void btnDDel_Click(object sender, EventArgs e)
        {
            #region 取得Button所在的GridViewRow

            Button btn = (Button)sender;
            GridViewRow gvr = (GridViewRow)btn.NamingContainer;

            #endregion

            #region DataTable處理

            //取得該RowIndex
            int rowIndex = gvr.RowIndex;

            //取得ViewState DataTable
            DataTable dt = SProductDt;

            //將資料寫入ViewState
            DetailTableSet(gvDList, dt); //沒寫的話當頁面刷新時值會不見

            //SN為空代表為本次新增
            if (string.IsNullOrEmpty(dt.Rows[rowIndex]["SN"].ToString()))
            {
                //直接刪除此Row
                dt.Rows.Remove(dt.Rows[rowIndex]);
            }
            else
            {
                //Status欄位紀錄為D (表示刪除)
                dt.Rows[rowIndex]["Status"] = "D";
            }

            #endregion

            #region 重新綁定

            gvDList.DataSource = dt;
            gvDList.DataBind();

            #endregion
        }

        // 計算分頁數量，並儲存當前頁與總頁數至hiddenfield
        protected void Pagination(int rowCount, ref int tStartRow, ref int tEndRow)
        {
            //計算分頁數量
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
            }

            //總頁數存進隱藏欄位
            hdTotalPage.Value = tTotalPage.ToString();

            //計算資料範圍
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
        // 設定分頁按鈕
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

        //  檢查關鍵字欄位是否錯誤
        protected string SearchFieldCheck()
        {
            StringBuilder sb = new StringBuilder();
            DateTime SDate;
            DateTime EDate;
            bool SDateValid = DateTime.TryParse(ucStartDate.Text, out SDate);
            bool EDateValid = DateTime.TryParse(ucEndDate.Text, out EDate);

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

        // 儲存前欄位檢核
        protected string SaveFieldCheck()
        {
            #region 變數宣告

            StringBuilder sb = new StringBuilder();
            string sStoreName = txtMStoreName.Text.Trim();
            string Address = txtMAddress.Text.Trim();
            string Telephone = txtMTelephone.Text.Trim();
            DateTime CDate;
            //DateTime SDate;
            //DateTime EDate;
            bool CDateValid = DateTime.TryParse(ucMCreateDate.Text, out CDate);
            //bool SDateValid = DateTime.TryParse(ucMStartDate.Text, out SDate);
            //bool EDateValid = DateTime.TryParse(ucMEndDate.Text, out EDate);

            #endregion

            #region Master

            if (string.IsNullOrEmpty(sStoreName))
                sb.Append("StoreName不可為空！\\n");

            if (string.IsNullOrEmpty(Address))
                sb.Append("Address不可為空！\\n");

            if (string.IsNullOrEmpty(Telephone))
                sb.Append("Telephone不可為空！\\n");

            if (string.IsNullOrEmpty(ucMCreateDate.Text))
                sb.Append("日期不可為空！\\n");
            else if (!CDateValid)
                sb.Append("日期格式錯誤！\\n");
                
                //if (string.IsNullOrEmpty(ucMStartDate.Text))
                //    sb.Append("起始日期不可為空！\\n");
                //else if (!SDateValid)
                //    sb.Append("起始日期格式錯誤！\\n");

                //if (string.IsNullOrEmpty(ucMEndDate.Text))
                //    sb.Append("結束日期不可為空！\\n");
                //else if (!EDateValid)
                //    sb.Append("結束日期格式錯誤！\\n");

                //if ((SDateValid && EDateValid) && SDate > EDate)
                //    sb.Append("起始日期不能晚於結束日期！\\n");

            #endregion

            #region Detail

            DataTable dtDetail = SProductDt;
            string sProductID = string.Empty;
            string sProductName = string.Empty;
            string sPrice = string.Empty;
            //int sProductID = 0;
            string sStatus = string.Empty;

            //需重新執行此函式一次，將最新一筆的Row資料寫入ViewState，
            DetailTableSet(gvDList, dtDetail);

            //正規表達式條件
            string pattern = @"\d{4}$";

            //string PricePattern = @"^/+?[1-9][0-9]*$";
            string PricePattern = @"^(0|[1-9][0-9]*)$";
            //string pattern = @"^[A-Za-z]\d{6}$";

            int Del_Count = dtDetail.AsEnumerable().Count(row => row["Status"].ToString() == "D") - 1;
            for (int i = 0; i < dtDetail.Rows.Count; i++)
            {
                sProductID = dtDetail.Rows[i]["ProductID"].ToString();
                sProductName = dtDetail.Rows[i]["ProductName"].ToString();
                sPrice = dtDetail.Rows[i]["Price"].ToString();
                //sProductID = Convert.ToInt32(dtDetail.Rows[i]["ProductID"]);
                sStatus = dtDetail.Rows[i]["Status"].ToString();

                if (string.IsNullOrEmpty(sProductID))
                    sb.Append($"第{gvDList.Rows[i].RowIndex - Del_Count}項產品ProducID不可為空！\\n");
                else if (!Regex.IsMatch(sProductID, pattern))
                    sb.Append($"第{gvDList.Rows[i].RowIndex - Del_Count}項產品ProducID格式錯誤！\\n");

                if (string.IsNullOrEmpty(sProductName))
                    sb.Append($"第{gvDList.Rows[i].RowIndex - Del_Count}項產品ProducName不可為空！\\n");

                if (string.IsNullOrEmpty(sPrice))
                    sb.Append($"第{gvDList.Rows[i].RowIndex - Del_Count}項產品Price不可為空！\\n");
                else if (!Regex.IsMatch(sPrice, PricePattern))
                    sb.Append($"第{gvDList.Rows[i].RowIndex - Del_Count}項產品Price只能輸入正整數！\\n");
            }

            //使用LINQ檢查目前Table內Student_ID是否有重複值
            bool CheckID = dtDetail.AsEnumerable()
                          .Where(row => row["Status"].ToString() != "D")
                          .Where(row => row["ProductID"] != DBNull.Value && !string.IsNullOrWhiteSpace(row.Field<string>("ProductID")))
                          .Select(row => row["ProductID"])
                          .GroupBy(id => id)
                          .Any(g => g.Count() > 1);

            if (CheckID)
                sb.Append($"禁止輸入重複的ProductID！\\n");

            #endregion

            //回傳錯誤訊息
            return sb.ToString();
        }

        // 關閉全部的Panel
        private void ClosePanel()
        {
            plQuery.Visible = false;
            plFunction.Visible = false;
            plList.Visible = false;
            plSet.Visible = false;
        }

        // 開啟對應模式的Panel
        protected void OpenPanel(Mode mode)
        {
            //判斷模式，開啟對應Panel
            switch (mode)
            {
                //預設
                case Mode.Default:
                    plQuery.Visible = true;
                    plFunction.Visible = true;
                    plList.Visible = true;
                    break;
                //新增
                case Mode.AddNew:
                    plSet.Visible = true;
                    plSet.GroupingText = "Add";
                    break;
                //編輯
                case Mode.Modify:
                    plSet.Visible = true;
                    plSet.GroupingText = "Modify";
                    break;
                default:
                    break;
            }
        }

        // 設置編輯Panel
        protected void SetModifyPanel(int StoreID)
        {
            //顯示刪除按鈕
            btnMDel.Visible = true;

            #region Master

            StoreBiz objStoreBiz = new StoreBiz();

            //用key從資料庫讀取該筆資料
            StoreInfo info = objStoreBiz.LoadStore(StoreID);

            //將資料放入欄位中
            txtMStoreName.Text = info.StoreName;
            txtMAddress.Text = info.Address;
            txtMTelephone.Text = info.Telephone;
            ucMCreateDate.Text = info.CreateDate.ToString();
            //ucMEndDate.Text = info.EndDate.ToString();

            #endregion

            #region Detail

            SProductBiz objSProductBiz = new SProductBiz();

            DataTable dtDetail = objSProductBiz.LoadProduct(StoreID);

            //新增IsDel欄位，讓欄位跟student型態一樣
            dtDetail.Columns.Add("Status", typeof(string));

            //使用ViewState暫存原本的Student資料
            SProductDt = dtDetail;

            //Detail清單Bind
            gvDList.DataSource = dtDetail;
            gvDList.DataBind();

            #endregion

            //開啟Modify Panel
            this.ClosePanel();
            this.OpenPanel(Mode.Modify);

            //紀錄執行操作
            hdMode.Value = "Modify";

            //CourseID存入隱藏欄位
            hdStoreID.Value = StoreID.ToString();
        }

        protected void AddStore()
        {
            StoreBiz objStoreBiz;
            SProductBiz objSProductBiz;
            SqlConnection CONNSEC;
            SqlTransaction myTransaction;

            #region Transaction設置
            //啟用transaction
            try
            {
                objStoreBiz = new StoreBiz();
                objSProductBiz = new SProductBiz();

                //獲取DB Connection
                CONNSEC = new SqlConnection(objStoreBiz.GetConnString());

                //開啟Connection
                CONNSEC.Open();

                //開啟交易
                myTransaction = CONNSEC.BeginTransaction();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            #endregion

            try
            {
                #region 欄位檢核

                //判斷輸入欄位是否格式不符
                string checkMsg = SaveFieldCheck();

                //如有錯誤彈出錯誤訊息，並返回頁面
                if (!string.IsNullOrEmpty(checkMsg))
                {
                    //利用basePage的DoAlertinAjax彈出提示訊息給使用者
                    base.DoAlertinAjax(this.Page, "msg", checkMsg);
                    return;
                }

                #endregion

                #region Master

                //創建info並將資料存入變數
                StoreInfo Sinfo = new StoreInfo();
                Sinfo.StoreName = txtMStoreName.Text.Trim();
                Sinfo.Address = txtMAddress.Text.Trim();
                Sinfo.Telephone = txtMTelephone.Text.Trim();
                Sinfo.CreateDate = ucMCreateDate.Text.Trim();
                //Sinfo.CreateDate = Convert.ToDateTime(ucMCreateDate.Text);
                //Sinfo.EndDate = Convert.ToDateTime(ucMEndDate.Text);

                //寫入資料庫並回傳本次新增資料流水號
                int SN = objStoreBiz.InsertStore(Sinfo, CONNSEC, myTransaction);

                #endregion

                #region Detail

                SProductInfo Pinfo = new SProductInfo();
                DataTable dtDetail = SProductDt; //有新存的product資料

                for (int i = 0; i < dtDetail.Rows.Count; i++)
                {
                    Pinfo.StoreID = SN;
                    Pinfo.ProductID = dtDetail.Rows[i]["ProductID"].ToString();
                    //Pinfo.ProductID = Convert.ToInt32(dtDetail.Rows[i]["ProductID"]);
                    Pinfo.ProductName = dtDetail.Rows[i]["ProductName"].ToString();

                    //待確認寫法
                    Pinfo.Price = dtDetail.Rows[i]["Price"].ToString();
                    //Pinfo.Price = Convert.ToInt32(dtDetail.Rows[i]["Price"]);
                    Pinfo.Remark = dtDetail.Rows[i]["Remark"].ToString();
                    objSProductBiz.InsertProduct(Pinfo, CONNSEC, myTransaction);
                }

                #endregion

                #region Transaction Success

                //認可本次交易
                myTransaction.Commit();

                //彈出提示訊息給使用者
                base.DoAlertinAjax(this.Page, "msg", "新增成功！");

                //重新查詢資料
                gvListBind();
                this.BindAddressLevel(ddlAddress);
                //返回清單畫面
                this.ClosePanel();
                this.OpenPanel(Mode.Default);

                #endregion
            }
            catch (Exception ex)
            {
                #region  Transaction Failed

                myTransaction.Rollback();

                throw ex;

                #endregion
            }
            finally //不管try能過或是進catch，最後都一定會進finally
            {
                #region 釋放資源

                myTransaction.Dispose();

                if (CONNSEC.State == ConnectionState.Open)
                    CONNSEC.Close();

                CONNSEC.Dispose();

                #endregion
            }
        }

        // 編輯Course
        protected void ModifyStore()
        {
            StoreBiz objStoreBiz;
            SProductBiz objSProductBiz;
            SqlConnection CONNSEC;
            SqlTransaction myTransaction;

            #region Transaction設置

            try
            {
                objStoreBiz = new StoreBiz();
                objSProductBiz = new SProductBiz();

                //獲取DB Connection
                CONNSEC = new SqlConnection(objStoreBiz.GetConnString());

                //開啟Connection
                CONNSEC.Open();

                //開啟交易
                myTransaction = CONNSEC.BeginTransaction();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            #endregion

            try
            {
                #region 欄位檢核

                //判斷輸入欄位是否格式不符
                string checkMsg = SaveFieldCheck();

                //如有錯誤彈出錯誤訊息，並返回頁面
                if (!String.IsNullOrWhiteSpace(checkMsg))
                {
                    //利用basePage的DoAlertinAjax彈出提示訊息給使用者
                    base.DoAlertinAjax(this.Page, "msg", checkMsg);
                    return;
                }

                #endregion

                #region Master

                int StoreID = Convert.ToInt32(hdStoreID.Value);

                //利用key讀取該筆資料
                StoreInfo Sinfo = objStoreBiz.LoadStore(StoreID);

                //存入修改資料
                Sinfo.StoreID = StoreID;
                Sinfo.StoreName = txtMStoreName.Text.Trim();
                Sinfo.Address = txtMAddress.Text.Trim();
                Sinfo.CreateDate = ucMCreateDate.Text.Trim();
                //Sinfo.StartDate = Convert.ToDateTime(ucMStartDate.Text);
                //Cinfo.EndDate = Convert.ToDateTime(ucMEndDate.Text);

                //更新至資料庫
                objStoreBiz.UpdateStore(Sinfo, CONNSEC, myTransaction);

                #endregion

                #region Detail

                objSProductBiz = new SProductBiz();
                SProductInfo Pinfo = new SProductInfo();
                DataTable dtDetail = SProductDt;
                string sStatus = string.Empty;

                for (int i = 0; i < dtDetail.Rows.Count; i++)
                {
                    //判斷資料狀態
                    sStatus = dtDetail.Rows[i]["Status"].ToString();

                    switch (sStatus.ToUpper())
                    {
                        //編輯
                        case "M":
                            Pinfo.StoreID = StoreID;
                            Pinfo.ProductID = dtDetail.Rows[i]["ProductID"].ToString();
                            //Pinfo.ProductID = Convert.ToInt32(dtDetail.Rows[i]["ProductID"]);
                            Pinfo.ProductName = dtDetail.Rows[i]["ProductName"].ToString();
                            Pinfo.Price = dtDetail.Rows[i]["Price"].ToString();
                            //Pinfo.Price = Convert.ToInt32(dtDetail.Rows[i]["Price"]);
                            Pinfo.Remark = dtDetail.Rows[i]["Remark"].ToString();
                            objSProductBiz.UpdateProduct(Pinfo, CONNSEC, myTransaction);
                            break;
                        //刪除
                        case "D":
                            objSProductBiz.DeleteProduct(StoreID, dtDetail.Rows[i]["ProductID"].ToString(), CONNSEC, myTransaction);
                            break;
                        //新增
                        case "A":
                            Pinfo.StoreID = StoreID;
                            Pinfo.ProductID = dtDetail.Rows[i]["ProductID"].ToString();
                            //Pinfo.ProductID = Convert.ToInt32(dtDetail.Rows[i]["ProductID"]);
                            Pinfo.ProductName = dtDetail.Rows[i]["ProductName"].ToString();
                            //Pinfo.Price = Convert.ToInt32(dtDetail.Rows[i]["Price"]);
                            Pinfo.Price = dtDetail.Rows[i]["Price"].ToString();
                            Pinfo.Remark = dtDetail.Rows[i]["Remark"].ToString();
                            objSProductBiz.InsertProduct(Pinfo, CONNSEC, myTransaction);
                            break;
                        default:
                            break;
                    }
                }

                #endregion

                #region Transaction Success

                //認可本次交易
                myTransaction.Commit();

                //彈出提示訊息給使用者
                base.DoAlertinAjax(this.Page, "msg", "修改成功！");

                //重新查詢資料
                gvListBind();
                this.BindAddressLevel(ddlAddress);
                //返回清單畫面
                this.ClosePanel();
                this.OpenPanel(Mode.Default);

                #endregion
            }
            catch (Exception ex)
            {
                #region Transaction Failed

                myTransaction.Rollback();

                throw ex;

                #endregion
            }
            finally
            {
                #region 釋放資源

                myTransaction.Dispose();

                if (CONNSEC.State == ConnectionState.Open)
                    CONNSEC.Close();

                CONNSEC.Dispose();

                #endregion
            }
        }

        protected void btnFirst_Click(object sender, EventArgs e)
        {
            try
            {
                hdPageIndex.Value = "1";
                gvListBind();
            }
            catch (Exception ex)
            {
                #region 紀錄Log
                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();
                objLogExpBiz.InsertLogExp("myMaster_Detail", "btnFirst_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;
                #endregion
            }
        }

        protected void btnPre_Click(object sender, EventArgs e)
        {
            try
            {
                int tPageInedex = Convert.ToInt32(hdPageIndex.Value);
                tPageInedex--;
                hdPageIndex.Value = tPageInedex.ToString();
                gvListBind();
            }
            catch (Exception ex)
            {
                #region 紀錄Log
                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();
                objLogExpBiz.InsertLogExp("myMaster_Detail", "btnPre_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;
                #endregion
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                int tPageInedex = Convert.ToInt32(hdPageIndex.Value);
                tPageInedex++;
                hdPageIndex.Value = tPageInedex.ToString();
                gvListBind();
            }
            catch (Exception ex)
            {
                #region 紀錄Log
                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();
                objLogExpBiz.InsertLogExp("myMaster_Detail", "btnNext_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;
                #endregion
            }
        }

        protected void btnLast_Click(object sender, EventArgs e)
        {
            try
            {
                hdPageIndex.Value = AntiXssEncoder.HtmlEncode(hdTotalPage.Value, true);
                gvListBind();
            }
            catch (Exception ex)
            {
                #region 紀錄Log
                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();
                objLogExpBiz.InsertLogExp("myMaster_Detail", "btnLast_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;
                #endregion
            }
        }

        protected void gvListBind()
        {
            //取出搜尋關鍵字
            string SName = hdStoreName.Value;
            string Address = hdAddress.Value;
            DateTime? CDate = null;
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
            if (!string.IsNullOrEmpty(hdCDate.Value))
            {
                CDate = Convert.ToDateTime(hdCDate.Value);
            }

            //取得資料數量
            StoreBiz objStoreBiz = new StoreBiz();
            int rowCount = objStoreBiz.InqStoreCount2(SName, Address, SDate, EDate);

            //用於儲存開始與結束的行數
            int tStartRow = 0;
            int tEndRow = 0;

            //計算分頁數量，並儲存當前頁與總頁數至hiddenfield
            this.Pagination(rowCount, ref tStartRow, ref tEndRow);

            //向DB查詢資料
            DataTable dt = objStoreBiz.InqStore2(SName, Address, SDate, EDate, tStartRow, tEndRow);

            //綁定資料
            gvList.DataSource = dt;
            gvList.DataBind();

            //設定gvList換頁按鈕
            this.gvListButtonSet((dt == null || dt.Rows.Count == 0) ? false : true, rowCount);
        }

        protected void gvList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int key;

            try
            {
                //以CommandName判斷執行命令為何
                switch (e.CommandName.ToUpper())
                {
                    case "MODIFY":

                        //從CommandArgument取出key值
                        key = Convert.ToInt32(e.CommandArgument);

                        //設置編輯畫面與填上資料
                        this.SetModifyPanel(key);

                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                #region 紀錄Log
                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();
                objLogExpBiz.InsertLogExp("myMaster_Detail", "gvList_RowCommand", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;
                #endregion
            }
        }

        protected void DetailTableSet(GridView gv, DataTable dt)
        {
            for (int i = 0; i < gv.Rows.Count; i++)
            {
                if (hdMode.Value.ToUpper() == "MODIFY")
                    dt.Rows[i]["StoreID"] = hdStoreID.Value;

                TextBox txtProductID = (TextBox)gvDList.Rows[i].FindControl("txtDProductID");
                dt.Rows[i]["ProductID"] = txtProductID.Text.Trim();

                TextBox txtProductName = (TextBox)gvDList.Rows[i].FindControl("txtDProductName");
                dt.Rows[i]["ProductName"] = txtProductName.Text.Trim();

                TextBox txtPrice = (TextBox)gvDList.Rows[i].FindControl("txtDPrice");
                dt.Rows[i]["Price"] = txtPrice.Text.Trim();
                //dt.Rows[i]["Price"] = Convert.ToInt32(txtPrice.Text.Trim());

                TextBox txtRemark = (TextBox)gvDList.Rows[i].FindControl("txtDRemark");
                dt.Rows[i]["Remark"] = txtRemark.Text.Trim();
            }
        }

        protected void gvDList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //綁定控制項
                Button btnModify = e.Row.FindControl("btnDModify") as Button;
                TextBox txtProductID = e.Row.FindControl("txtDProductID") as TextBox;
                Label lblProductID = e.Row.FindControl("lblDProductID") as Label;
                TextBox txtProductName = e.Row.FindControl("txtDProductName") as TextBox;
                Label lblProductName = e.Row.FindControl("lblDProductName") as Label;
                TextBox txtPrice = e.Row.FindControl("txtDPrice") as TextBox;
                Label lblPrice = e.Row.FindControl("lblDPrice") as Label;
                TextBox txtRemark = e.Row.FindControl("txtDRemark") as TextBox;
                Label lblRemark = e.Row.FindControl("lblDRemark") as Label;

                //特定的Row
                GridViewRow gvr = e.Row;

                //特定Row資料
                DataRowView drv = e.Row.DataItem as DataRowView;

                if (drv != null)
                {
                    // ""未更動 A新增 M編輯 D刪除
                    string status = drv["Status"].ToString();

                    gvr.Visible = status != "D";
                    #region 同上
                    //if (status != "D")
                    //{
                    //    gvr.Visible = true;
                    //}
                    //else
                    //{
                    //    gvr.Visible = false;
                    //}
                    #endregion

                    gvr.BackColor = status == "M" ? System.Drawing.ColorTranslator.FromHtml("#E0E0E0") : gvr.BackColor;
                    #region 同上
                    //if (status == "M")
                    //{
                    //    gvr.BackColor = System.Drawing.ColorTranslator.FromHtml("#E0E0E0");
                    //}
                    #endregion

                    btnModify.Enabled = status != "A";
                    #region 同上
                    //if (status != "A")
                    //{
                    //    btnModify.Enabled = true;
                    //}
                    //else
                    //{
                    //    btnModify.Enabled = false;
                    //}
                    #endregion

                    btnModify.Visible = status == "A" || status == "M" ? false : true;
                    #region 同上
                    //if (status == "A" || status == "M")
                    //{
                    //    btnModify.Visible = false;
                    //}
                    //else
                    //{
                    //    btnModify.Visible = true;
                    //}
                    #endregion

                    txtProductID.Enabled = txtProductID.Visible = status == "A";
                    #region 同上
                    //if (status == "A")
                    //{
                    //    txtStudent_ID.Enabled = true;
                    //    txtStudent_ID.Visible = true;
                    //}
                    //else
                    //{
                    //    txtStudent_ID.Enabled = false;
                    //    txtStudent_ID.Visible = false;
                    //}
                    #endregion

                    lblProductID.Visible = !(status == "A");
                    #region 同上
                    //if (!(status == "A"))
                    //{
                    //    lblStudent_ID.Visible = true;
                    //}
                    //else
                    //{
                    //    lblStudent_ID.Visible = false;
                    //}
                    #endregion

                    txtProductName.Enabled = txtProductName.Visible = status == "M" || status == "A";
                    #region 同上
                    //if (status == "M" || status == "A")
                    //{
                    //    txtStudent_Name.Enabled = true;
                    //    txtStudent_Name.Visible = true;
                    //}
                    //else
                    //{
                    //    txtStudent_Name.Enabled = false;
                    //    txtStudent_Name.Visible = false;
                    //}
                    #endregion

                    lblProductName.Visible = status == "";
                    #region 同上
                    //if (status == "")
                    //{
                    //    lblStudent_Name.Visible = true;
                    //}
                    //else
                    //{
                    //    lblStudent_Name.Visible = false;
                    //}
                    #endregion

                    txtPrice.Enabled = txtPrice.Visible = status == "M" || status == "A";
                    lblPrice.Visible = status == "";

                    txtRemark.Enabled = txtRemark.Visible = status == "M" || status == "A";
                    lblRemark.Visible = status == "";
                }
            }
        }


    }
}