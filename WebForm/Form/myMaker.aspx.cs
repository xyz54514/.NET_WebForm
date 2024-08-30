﻿using System;
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
    public partial class myMaker : basePage
    {

        private const int igvListPageCount = 10;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    this.ClosePanel();
                    this.OpenPanel(Mode.Default);

                    hdName.Value = string.Empty;

                    this.gvListBind();
                }
            }
            catch (Exception ex)
            {
                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();

                //傳入className、methodName、exception供記錄用
                objLogExpBiz.InsertLogExp("myMaker", "Page_Load", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr",
                    "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;
            }
        }

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
                hdTime.Value = ucCalendar.Text.Trim();
                
                //綁訂清單
                gvListBind();
            }
            catch (Exception ex)
            {
                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();
                objLogExpBiz.InsertLogExp("myMaker", "btnQuery_Click", ex);

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
                txtSetQuantity.Text = String.Empty;
                txtSetUnitPrice.Text = String.Empty;

                //紀錄執行操作
                hdMode.Value = "Add";
            }
            catch (Exception ex)
            {
                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();
                objLogExpBiz.InsertLogExp("myMaker", "btnAdd_Click", ex);

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
                        AddOrder();
                        break;
                    case "Modify":
                        //修改Product
                        ModifyOrder();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                LogExpBiz objLogExpBiz = new LogExpBiz();
                objLogExpBiz.InsertLogExp("myMaker", "btnSubmit_Click", ex);

                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr",
                    "alert('有點問題服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
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
                OrderBiz objOrderBiz = new OrderBiz();

                //用key從資料庫讀取該筆資料
                OrderInfo info = objOrderBiz.LoadOrder(int.Parse(ViewState["OrderID"].ToString()));

                //確認被退件才能刪除
                if (info.Status == "M")
                {
                    //刪除指定Product資料
                    int key = Convert.ToInt32(ViewState["OrderID"].ToString());

                    //利用key刪除該筆資料
                    objOrderBiz.DeleteOrder(key);

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
                objLogExpBiz.InsertLogExp("myMaker", "btnDelete_Click", ex);

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
            if (String.IsNullOrWhiteSpace(txtSetQuantity.Text.Trim()))
                sb.Append("Quantity為必填！\\n");
            if (String.IsNullOrWhiteSpace(txtSetUnitPrice.Text.Trim()))
                sb.Append("UnitPrice為必填！\\n");
            

            return sb.ToString();
        }

        protected void AddOrder()
        {
            string checkMsg = SetFieldCheck();
            if (!String.IsNullOrWhiteSpace(checkMsg))
            {
                base.DoAlertinAjax(this.Page, "msg", checkMsg);
                return;
            }

            //正確的話，創建info並將資料存入變數
            OrderInfo info = new OrderInfo();

            info.ProductName = txtSetName.Text.Trim();
            info.Quantity = Convert.ToInt32(txtSetQuantity.Text.Trim());
            info.UnitPrice = Convert.ToInt32(txtSetUnitPrice.Text.Trim());
            info.TotalPrice = info.Quantity * info.UnitPrice;
            info.Status = "C";
            info.CreateDate = DateTime.Now;
            info.OrderDate = DateTime.Now;
            info.Creator = base.strUserID;

            //新增資料
            OrderBiz objOrderBiz = new OrderBiz();
            objOrderBiz.InsertOrder(info);

            base.DoAlertinAjax(this.Page, "msg", "新增成功！");

            gvListBind();

            this.ClosePanel();
            this.OpenPanel(Mode.Default);
        }

        protected void ModifyOrder()
        {
            string checkMsg = SetFieldCheck();
            if (!String.IsNullOrWhiteSpace(checkMsg))
            {
                base.DoAlertinAjax(this.Page, "msg", checkMsg);
                return;
            }
            OrderBiz objOrderBiz = new OrderBiz();

            //用PID從資料庫讀取該筆資料
            OrderInfo info = objOrderBiz.LoadOrder(int.Parse(ViewState["OrderID"].ToString()));

            info.ProductName = txtSetName.Text.Trim();
            info.Quantity = Convert.ToInt32(txtSetQuantity.Text.Trim());
            info.UnitPrice = Convert.ToInt32(txtSetUnitPrice.Text.Trim());
            info.TotalPrice = info.Quantity * info.UnitPrice;
            info.Status = "C"; //狀態為待審核(C)
            info.Maker = base.strUserID;

            objOrderBiz.UpdateOrder(info);

            base.DoAlertinAjax(this.Page, "msg", "送審成功！");

            gvListBind();

            this.ClosePanel();
            this.OpenPanel(Mode.Default);
        }
        protected void ClosePanel()
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
                objLogExpBiz.InsertLogExp("myMaker", "gvList_RowCommand", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr",
                    "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;
            }
        }

        protected void SetModifyPanel(int key)
        {
            ViewState["OrderID"] = key;

            OrderBiz objOrderBiz = new OrderBiz();

            //用key從資料庫讀取該筆資料
            OrderInfo info = objOrderBiz.LoadOrder(key);

            //將資料放入欄位中
            txtSetName.Text = info.ProductName;
            txtSetQuantity.Text = info.Quantity.ToString();
            txtSetUnitPrice.Text = info.UnitPrice.ToString();
            //txtSetTotalPrice.Text = info.TotalPrice.ToString();

            //開啟或關閉Panel
            this.ClosePanel();
            this.OpenPanel(Mode.Modify);
            btnDelete.Visible = true;

            //設定編輯頁面顯示文字
            plSet.GroupingText = "Modify";

            //紀錄執行操作
            hdMode.Value = "Modify";
        }

        protected void gvListBind()
        {
            //取出搜尋關鍵字
            string name = hdName.Value;
            string time = hdTime.Value;
            
            //取得資料數量
            
            OrderBiz objOrderBiz = new OrderBiz();
            int rowCount = objOrderBiz.InqOrderCountMaker(name, time);

            int tStartRow = 0;
            int tEndRow = rowCount;

            this.Pagination(rowCount, ref tStartRow, ref tEndRow);

            //向DB查詢資料
            DataTable dt = objOrderBiz.InqOrderMaker(name, time, tStartRow, tEndRow);

            gvList.DataSource = dt;
            gvList.DataBind();

            this.gvListButtonSet(dt, rowCount);
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
    }
}