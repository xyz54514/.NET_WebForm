using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security.AntiXss;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebUICommon;
using Information;
using Business;

namespace WebForm.Form
{
    public partial class Maker : basePage
    {
        #region variable
        private const int igvListPageCount = 10;

        #endregion

        #region Page Function
        protected void Page_Load(object sender, EventArgs e)
        {
            //將可能產生錯誤訊息的程式碼撰寫在try中，以便錯誤發生時做適當處理。
            try
            {
                //僅在剛進入時執行
                if (!IsPostBack)
                {
                    //初始化畫面
                    this.ClosePanel();
                    this.OpenPanel(Mode.Default);

                    //設定預設值
                    hdName.Value = string.Empty;

                    //綁訂清單
                    this.gvListBind();
                }
            }
            //在錯誤發生時，會跳進catch並傳入錯誤訊息，在此撰寫Log進資料庫以便日後查看，同時彈出訊息給使用者
            catch (Exception ex)
            {
                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();

                //傳入className、methodName、exception供記錄用
                objLogExpBiz.InsertLogExp("Maker", "Page_Load", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr",
                    "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;
            }
        }

        #endregion

        #region button function

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            try 
            {
                //檢查搜尋欄位
                string checkMsg = SearchFieldCheck();

                //有錯誤跳提示
                if (!String.IsNullOrWhiteSpace(checkMsg))
                {
                    base.DoAlertinAjax(this.Page, "msg", checkMsg);
                    return;
                }

                //儲存搜尋關鍵字
                hdName.Value = txtName.Text.Trim();

                //綁訂清單
                gvListBind();
            }
            catch (Exception ex)
            {
                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();
                objLogExpBiz.InsertLogExp("Maker", "btnQuery_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", 
                    "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                //重設畫面
                this.ClosePanel();
                this.OpenPanel(Mode.AddNew);
                btnDelete.Visible = false;

                //初始化新增頁面的欄位
                plSet.GroupingText = "Add";
                txtSetName.Text = String.Empty;
                txtSetPrice.Text = String.Empty;

                //紀錄執行操作
                hdMode.Value = "Add";
            }
            catch (Exception ex)
            {
                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();
                objLogExpBiz.InsertLogExp("Maker", "btnAdd_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr",
                    "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            //重設畫面
            this.ClosePanel();
            this.OpenPanel(Mode.Default);
        }
        /// <summary>
        /// 儲存按鈕設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                //判斷執行動作
                switch (hdMode.Value)
                {
                    case "Add":
                        //新增Product
                        AddProduct();
                        break;
                    case "Modify":
                        //修改Product
                        ModifyProduct();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                LogExpBiz objLogExpBiz = new LogExpBiz();
                objLogExpBiz.InsertLogExp("Maker", "btnSubmit_Click", ex);

                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr",
                    "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;
            }
        }
        /// <summary>
        /// 刪除指定Product資料
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                ProductBiz objProductBiz = new ProductBiz();

                //用key從資料庫讀取該筆資料
                ProductInfo info = objProductBiz.LoadProduct(int.Parse(ViewState["PID"].ToString()));

                //確認被退件才能刪除
                if (info.Status == "M")
                {
                    //刪除指定Product資料
                    int key = Convert.ToInt32(ViewState["PID"].ToString());

                    //利用key刪除該筆資料
                    objProductBiz.DeleteProduct(key);

                    //提示訊息
                    base.DoAlertinAjax(this.Page, "msg", "刪除成功！");

                    //重新查詢資料
                    gvListBind();

                    this.ClosePanel();
                    this.OpenPanel(Mode.Default);
                }
                else
                {
                    base.DoAlertinAjax(this.Page, "msg", "此資料無法刪除！");
                }
            }
            catch (Exception ex)
            {
                LogExpBiz objLogExpBiz = new LogExpBiz();
                objLogExpBiz.InsertLogExp("Maker", "btnDelete_Click", ex);

                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr",
                    "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;
            }
        }
        #endregion

        #region 自定義Function

        protected string SearchFieldCheck()
        {
            StringBuilder sb = new StringBuilder();

            if (txtName.Text.Trim().Length > 10)
            {
                sb.Append("ProductName的字數不能超過10個字！\\n");
            }

            return sb.ToString();
        }

        protected string SetFieldCheck()
        {
            StringBuilder sb = new StringBuilder();
            if (String.IsNullOrWhiteSpace(txtSetName.Text.Trim()))
                sb.Append("ProductName為必填！\\n");
            if (String.IsNullOrWhiteSpace(txtSetPrice.Text.Trim()))
                sb.Append("Price為必填！\\n");

            return sb.ToString();
        }

        protected void AddProduct()
        {
            string checkMsg = SetFieldCheck();
            if (!String.IsNullOrWhiteSpace(checkMsg))
            {
                base.DoAlertinAjax(this.Page, "msg", checkMsg);
                return;
            }

            //正確的話，創建info並將資料存入變數
            ProductInfo info = new ProductInfo();

            info.ProdName = txtSetName.Text.Trim();
            info.Price = Convert.ToInt32(txtSetPrice.Text.Trim());
            info.Status = "C";
            info.CreateDate = DateTime.Now;
            info.Creator = base.strUserID;

            //新增資料
            ProductBiz objProductBiz = new ProductBiz();
            objProductBiz.InsertProduct(info);

            base.DoAlertinAjax(this.Page, "msg", "新增成功！");

            gvListBind();

            this.ClosePanel();
            this.OpenPanel(Mode.Default);
        }

        protected void ModifyProduct()
        {
            string checkMsg = SetFieldCheck();
            if (!String.IsNullOrWhiteSpace(checkMsg))
            {
                base.DoAlertinAjax(this.Page, "msg", checkMsg);
                return;
            }
            ProductBiz objProductBiz = new ProductBiz();

            //用PID從資料庫讀取該筆資料
            ProductInfo info = objProductBiz.LoadProduct(int.Parse(ViewState["PID"].ToString()));

            info.ProdName = txtSetName.Text.Trim();
            info.Price = Convert.ToInt32(txtSetPrice.Text.Trim());
            info.Status = "C"; //狀態為待審核(C)
            info.Maker = base.strUserID;

            objProductBiz.UpdateProduct(info);

            base.DoAlertinAjax(this.Page, "msg", "送審成功！");

            gvListBind();

            this.ClosePanel();
            this.OpenPanel(Mode.Default);
        }

        private void CalDataSAndE(int pPageIndex, int pPageCount, ref int pStartRow, ref int pEndRow)
        {
            //計算資料範圍，取得當前分頁需要的行數是第幾到第幾
            if (pPageIndex < 2)
            {
                pStartRow = 1;
                pEndRow = pPageCount;
            }
            else
            {
                pStartRow = ((pPageIndex - 1) * pPageCount) + 1;
                pEndRow = (pPageIndex * pPageCount);
            }
        }

        private void ClosePanel()
        {
            plQuery.Visible = false;
            plList.Visible = false;
            plSet.Visible = false;
        }

        protected void OpenPanel(Mode mode)
        {
            switch (mode)
            {
                case Mode.Default:
                    plQuery.Visible = true;
                    plList.Visible = true;
                    break;
                case Mode.AddNew:
                    plSet.Visible = true;
                    break;
                case Mode.Modify:
                    plSet.Visible = true;
                    break;
                default:
                    break;
            }
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
            }

            hdTotalPage.Value = tTotalPage.ToString();

            int tPageIndex;
            if (int.TryParse(hdPageIndex.Value, out tPageIndex) == false)
            {
                tPageIndex = 1;
                hdPageIndex.Value = tPageIndex.ToString();
            }

            if (tPageIndex > tTotalPage)
            {
                tPageIndex = tTotalPage;
                hdPageIndex.Value = tPageIndex.ToString();
            }

            this.CalDataSAndE(tPageIndex, igvListPageCount, ref tStartRow, ref tEndRow);
        }

        protected void gvListButtonSet(DataTable dt, int rowCount)
        {
            //若有資料則顯示分頁畫面，反之則不顯示
            if (dt == null || dt.Rows.Count == 0)
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
        /// <summary>
        /// 設置編輯畫面與填入資料
        /// </summary>
        /// <param name="key"></param>
        protected void SetModifyPanel(int key)
        {
            ViewState["PID"] = key;

            ProductBiz objProductBiz = new ProductBiz();

            //用key從資料庫讀取該筆資料
            ProductInfo info = objProductBiz.LoadProduct(key);

            //將資料放入欄位中
            txtSetName.Text = info.ProdName;
            txtSetPrice.Text = info.Price.ToString();

            //開啟或關閉Panel
            this.ClosePanel();
            this.OpenPanel(Mode.Modify);
            btnDelete.Visible = true;

            //設定編輯頁面顯示文字
            plSet.GroupingText = "Modify";

            //紀錄執行操作
            hdMode.Value = "Modify";
        }

        #endregion

        #region gvList Function

        #region 分頁
         
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

        #endregion

        protected void gvList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // 根據你的欄位名稱，取得某一列的值
                string sStatus_Ch = DataBinder.Eval(e.Row.DataItem, "Status_Ch") as string;

                // 假設你要檢查的值為 "待審核"
                if (sStatus_Ch != null && sStatus_Ch.Equals("待審核"))
                {
                    // 找到Button控制項，並隱藏它
                    Button btnModify = e.Row.FindControl("btnModify") as Button;
                    if (btnModify != null)
                    {
                        btnModify.Visible = false;
                    }
                }
            }
        }

        protected void gvList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int key;

            try
            {
                //以CommandName判斷執行命令為何
                switch (e.CommandName)
                {
                    case "Modify":
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
                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();
                objLogExpBiz.InsertLogExp("Maker", "gvList_RowCommand", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr",
                    "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;
            }
        }

        protected void gvListBind()
        {
            //取出搜尋關鍵字
            string name = hdName.Value;

            //取得資料數量
            ProductBiz objProductBiz = new ProductBiz();
            int rowCount = objProductBiz.InqProductCountMaker(name);

            int tStartRow = 0;
            int tEndRow = rowCount;

            this.Pagination(rowCount, ref tStartRow, ref tEndRow);

            //向DB查詢資料
            DataTable dt = objProductBiz.InqProductMaker(name, tStartRow, tEndRow);

            gvList.DataSource = dt;
            gvList.DataBind();

            this.gvListButtonSet(dt, rowCount);
        }

        #endregion

    }
}