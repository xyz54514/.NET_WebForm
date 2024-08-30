using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security.AntiXss;
using System.Web.UI;
using System.Web.UI.WebControls;
using Information;
using Business;

namespace WebForm.Form
{
    public partial class Checker : basePage
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
                objLogExpBiz.InsertLogExp("Checker", "Page_Load", ex);

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

        #endregion

        #region 自定義Function

        protected string SearchFieldCheck()
        {
            return String.Empty;
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
        }

        protected void OpenPanel(Mode mode)
        {
            switch (mode)
            {
                case Mode.Default:
                    plQuery.Visible = true;
                    plList.Visible = true;
                    break;
                default:
                    break;
            }
        }

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

            //計算資料範圍，取得當前分頁需要的行數是第幾到第幾
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
        protected void HandleProduct(string status, int key)
        {
            ProductBiz objProductBiz = new ProductBiz();

            //用key從資料庫讀取該筆資料
            ProductInfo info = objProductBiz.LoadProduct(key);

            if (info.Status == "C")  //在待審核的狀態下
            {
                //存入修改資料
                info.Status = status;
                info.Checker = base.strUserID;

                //更新至資料庫
                objProductBiz.UpdateProduct(info);

                //提示訊息
                if (status == "A") 
                {
                    base.DoAlertinAjax(this.Page, "msg", "核准成功！");
                }
                else
                {
                    base.DoAlertinAjax(this.Page, "msg", "退回成功！");
                }

                gvListBind();
            }
            else 
            {
                base.DoAlertinAjax(this.Page, "msg", "資料非待審核狀態！");
            }
            this.ClosePanel();
            this.OpenPanel(Mode.Default);
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

                // 假設你要檢查的值為 "Approve"
                if (sStatus_Ch != null && sStatus_Ch.Equals("已核准"))
                {
                    // 找到Button控制項，並隱藏它
                    Button btnApprove = e.Row.FindControl("btnApprove") as Button;
                    if (btnApprove != null)
                    {
                        btnApprove.Visible = false;
                    }
                }
            }

            
        }

        protected void gvList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try 
            {
                ProductInfo info = new ProductInfo();
                int key;

                //以CommandName判斷執行命令為何
                switch (e.CommandName)
                {
                    //退回
                    case "Reject":
                        key = Convert.ToInt32(e.CommandArgument);
                        HandleProduct("M", key);
                        break;
                    //核准
                    case "Approve":
                        key = Convert.ToInt32(e.CommandArgument);
                        HandleProduct("A", key);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                LogExpBiz objLogExpBiz = new LogExpBiz();
                objLogExpBiz.InsertLogExp("Checker", "gvList_RowCommand", ex);

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
            ProductBiz objCustomerBiz = new ProductBiz();
            int rowCount = objCustomerBiz.InqProductCountChecker(name);

            //用於儲存開始與結束的行數
            int tStartRow = 0;
            int tEndRow = rowCount;

            //計算分頁數量，並儲存當前頁與總頁數至hiddenfield
            this.Pagination(rowCount, ref tStartRow, ref tEndRow);

            //向DB查詢資料
            DataTable dt = objCustomerBiz.InqProductChecker(name, tStartRow, tEndRow);

            //綁定資料
            gvList.DataSource = dt;
            gvList.DataBind();

            //設定gvList換頁按鈕
            this.gvListButtonSet(dt, rowCount);
        }

        #endregion
    }
}