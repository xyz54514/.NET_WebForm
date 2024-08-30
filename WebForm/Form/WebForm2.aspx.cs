using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security.AntiXss;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security.AntiXss;
using Business;
using Common;
using Information;
using WebUICommon;

namespace WebForm.Form
{
    public partial class WebForm2 : basePage
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

                    //設定預設值
                    hdStoreName.Value = string.Empty;
                    hdAddress.Value = string.Empty;

                    //綁訂清單
                    this.gvListBind();

                    //綁訂會員等級
                    this.BindMemberLevel(DropDownList1);
                }
            }
            catch (Exception ex)
            {
                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();

                //傳入className、methodName、exception供記錄用
                objLogExpBiz.InsertLogExp("WebForm2", "Page_Load", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;
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

        /// <summary>
        /// 綁定會員等級至DropDownList
        /// </summary>
        /// <param name="ddl">指定dropdownList</param>
        protected void BindMemberLevel(DropDownList ddl)
        {
            //宣告對應Biz使用查詢函式
            DataDictionaryBiz objDataDictionaryBiz = new DataDictionaryBiz();

            //查詢會員等級的清單
            DataTable typeList = objDataDictionaryBiz.InqDataDictionary("sAddress");

            //綁訂資料至dropdownlist
            ddl.DataSource = typeList;
            ddl.DataTextField = "TextField";
            ddl.DataValueField = "ValueField";
            ddl.DataBind();

            //插入指定資料到第一列
            ddl.Items.Insert(0, new ListItem("請選擇", string.Empty));

            //選擇dropdownlist第一個item
            ddl.SelectedIndex = 0;
        }

        /// <summary>
        /// 查詢按鈕事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                //檢查搜尋欄位
                string checkMsg = SearchFieldCheck();

                //有錯誤跳提示
                if (!String.IsNullOrWhiteSpace(checkMsg))
                {
                    //利用basePage的DoAlertinAjax彈出提示訊息給使用者
                    base.DoAlertinAjax(this.Page, "msg", checkMsg);
                    return;
                }

                //儲存搜尋關鍵字
                hdStoreName.Value = TextBox1.Text.Trim();
                hdAddress.Value = DropDownList1.SelectedValue;

                //綁定清單
                gvListBind();
            }
            catch (Exception ex)
            {
                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();
                objLogExpBiz.InsertLogExp("CRUD", "btnQuery_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                this.ClosePanel();
                this.OpenPanel(Mode.AddNew);

                plSet.GroupingText = "Add";
                btnSet.Text = "Add";
                txtSetSName.Text = String.Empty;
                txtSetAddress.Text = String.Empty;
                txtSettPhone.Text = String.Empty;
                txtSetSID.Text = String.Empty;

                hdMode.Value = "Add";
            }
            catch (Exception ex)
            {
                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();
                objLogExpBiz.InsertLogExp("WebForm2", "btnAdd_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            this.ClosePanel();
            this.OpenPanel(Mode.Default);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                switch (hdMode.Value)
                {
                    case "Add":
                        AddStore();
                        break;
                    case "Modify":
                        
                        ModifyStore();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();
                objLogExpBiz.InsertLogExp("Webform2", "btnSave_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;
            }
        }

        /// <summary>
        ///  檢查關鍵字欄位是否錯誤
        /// </summary>
        /// <returns></returns>
        protected string SearchFieldCheck()
        {
            StringBuilder sb = new StringBuilder();

            //判斷Name的關鍵字是否超過50個字元
            // .Trim()是用於移除開頭與結尾的空白字元
            // Length是取得字元數量
            if (TextBox1.Text.Trim().Length > 50)
            {
                //新增提示字串至StringBuilder
                sb.Append("Name的字數不能超過50個字！\\n");
            }

            //回傳錯誤訊息
            return sb.ToString();
        }

        protected string FieldCheck()
        {
            StringBuilder sb = new StringBuilder();
            if (String.IsNullOrWhiteSpace(txtSetSName.Text.Trim()))
                sb.Append("StoreName為必填!\\n");
            if (String.IsNullOrWhiteSpace(txtSetAddress.Text.Trim()))
                sb.Append("Address為必填!\\n");
            if (String.IsNullOrWhiteSpace(txtSettPhone.Text.Trim()))
                sb.Append("Telephone為必填!\\n");
            return sb.ToString();
        }

        protected void AddStore()
        {
            string checkMsg = FieldCheck();

            if (!String.IsNullOrWhiteSpace(checkMsg))
            {
                base.DoAlertinAjax(this.Page, "msg", checkMsg);
                return;
            }

            StoreInfo info = new StoreInfo();
            info.StoreName = txtSetSName.Text.Trim();
            info.Address = txtSetAddress.Text.Trim();
            info.Telephone = txtSettPhone.Text.Trim();
            info.Remark = "網頁中輸入";

            StoreBiz objstoreBiz = new StoreBiz();
            objstoreBiz.InsertStore(info);

            //彈出提示訊息給使用者
            base.DoAlertinAjax(this.Page, "msg", "新增成功！");

            //重新查詢資料
            gvListBind();
            this.BindMemberLevel(DropDownList1);
            //返回清單畫面
            this.ClosePanel();
            this.OpenPanel(Mode.Default);
        }

        protected void ModifyStore()
        {
            //判斷輸入欄位是否格式不符
            string checkMsg = FieldCheck();

            //如有錯誤彈出錯誤訊息，並返回頁面
            if (!String.IsNullOrWhiteSpace(checkMsg))
            {
                //利用basePage的DoAlertinAjax彈出提示訊息給使用者
                base.DoAlertinAjax(this.Page, "msg", checkMsg);
                return;
            }

            StoreBiz objStoreBiz = new StoreBiz();

            //利用key讀取該筆資料
            StoreInfo info = objStoreBiz.LoadStore(int.Parse(txtSetSID.Text));

            string lastAddress = info.Address;
            //存入修改資料
            info.StoreName = txtSetSName.Text.Trim();
            info.Address = txtSetAddress.Text.Trim();
            info.Telephone = txtSettPhone.Text.Trim();

            //更新至資料庫
            objStoreBiz.UpdateStore(info, lastAddress);

            //提示訊息
            base.DoAlertinAjax(this.Page, "msg", "編輯成功！");

            //重新查詢資料
            gvListBind();
            this.BindMemberLevel(DropDownList1);
            //返回清單畫面
            this.ClosePanel();
            this.OpenPanel(Mode.Default);
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



        #region gvList Function

        #region 分頁

        //protected void btnFirst_Click(object sender, EventArgs e)
        //{
        //    hdPageIndex.Value = "1";
        //    gvListBind();
        //}

        //protected void btnPre_Click(object sender, EventArgs e)
        //{
        //    int tPageInedex = Convert.ToInt32(hdPageIndex.Value);
        //    tPageInedex--;
        //    hdPageIndex.Value = tPageInedex.ToString();
        //    gvListBind();
        //}

        //protected void btnNext_Click(object sender, EventArgs e)
        //{
        //    int tPageInedex = Convert.ToInt32(hdPageIndex.Value);
        //    tPageInedex++;
        //    hdPageIndex.Value = tPageInedex.ToString();
        //    gvListBind();
        //}

        //protected void btnLast_Click(object sender, EventArgs e)
        //{
        //    hdPageIndex.Value = AntiXssEncoder.HtmlEncode(hdTotalPage.Value, true);
        //    gvListBind();
        //}

        //#endregion

        /// <summary>
        /// gvList_RowDataBound 從SQL拿到的資料做後製處理
        /// </summary>
        /// <param name="sender"></param>
        /// <ram name="e"></param>
        //protected void gvList_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    // 確認目前處理的 GridView 行的類型是否為資料行
        //    if (e.Row.RowType == DataControlRowType.DataRow) //第一行是欄位名稱
        //    {
        //        // 根據第三個儲存格的數值進行不同的轉換
        //        switch (Convert.ToInt32(e.Row.Cells[2].Text))
        //        {
        //            // 如果數值為1，將該儲存格文字設為「普通會員」
        //            case 1:
        //                e.Row.Cells[2].Text = "普通會員";
        //                e.Row.Cells[2].ForeColor = System.Drawing.Color.Red;
        //                break;

        //            // 如果數值為2，將該儲存格文字設為「VIP會員」
        //            case 2:
        //                e.Row.Cells[2].Text = "VIP會員";
        //                break;

        //            // 如果數值為3，將該儲存格文字設為「VVIP會員」
        //            case 3:
        //                e.Row.Cells[2].Text = "VVIP會員";
        //                break;

        //            // 若數值不在預期範圍內，不進行任何轉換
        //            default:
        //                break;
        //        }
        //    }
        //}

        /// <summary>
        /// gvList RowCommand
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    case "Del":

                        //從CommandArgument取出key值
                        key = Convert.ToInt32(e.CommandArgument);

                        //刪除指定Customer資料
                        this.DelStore(key);

                        break;
                    default:

                        break;
                }
            }
            catch (Exception ex)
            {
                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();
                objLogExpBiz.InsertLogExp("WebForm2", "gvList_RowCommand", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;
            }
        }

        /// <summary>
        /// 計算分頁數量，並儲存當前頁與總頁數至hiddenfield
        /// </summary>
        /// <param name="rowCount">總資料數量</param>
        /// <param name="tStartRow">起始行數</param>
        /// <param name="tEndRow">結束行數</param>
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

        protected void gvListBind()
        {
            //取出搜尋關鍵字
            string name = hdStoreName.Value;
            string type = hdAddress.Value;

            //取得資料數量
            StoreBiz objStoreBiz = new StoreBiz();
            int rowCount = objStoreBiz.InqStoreCount(name, type);

            //用於儲存開始與結束的行數
            int tStartRow = 0;
            int tEndRow = 0;

            //計算分頁數量，並儲存當前頁與總頁數至hiddenfield
            this.Pagination(rowCount, ref tStartRow, ref tEndRow);

            //向DB查詢資料
            DataTable dt = objStoreBiz.InqStore(name, type, tStartRow, tEndRow);

            //綁定資料
            gvList.DataSource = dt;
            gvList.DataBind();

            //設定gvList換頁按鈕
            this.gvListButtonSet((dt == null || dt.Rows.Count == 0) ? false : true, rowCount);
        }

        /// <summary>
        /// 設定分頁按鈕
        /// </summary>
        /// <param name="visible">是否顯示分頁按鈕</param>
        /// <param name="rowCount"></param>
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
        #endregion

        protected void SetModifyPanel(int key)
        {
            StoreBiz objStoreBiz = new StoreBiz();

            //綁定會員等級
            //this.BindMemberLevel(ddlSetType);

            //用key從資料庫讀取該筆資料
            StoreInfo info = objStoreBiz.LoadStore(key);

            //將資料放入欄位中
            txtSetSID.Text = info.StoreID.ToString();
            txtSetSName.Text = info.StoreName;
            //ddlSetType.SelectedValue = info.Type;
            txtSetAddress.Text = info.Address;
            txtSettPhone.Text = info.Telephone;

            //開啟或關閉Panel
            this.ClosePanel();
            this.OpenPanel(Mode.Modify);

            //設定編輯頁面顯示文字
            plSet.GroupingText = "Modify";
            btnSet.Text = "Modify";

            //紀錄執行操作
            hdMode.Value = "Modify";
        }

        protected void DelStore(int key)
        {
            StoreBiz objStoreBiz = new StoreBiz();

            //利用key讀取該筆資料
            StoreInfo info = objStoreBiz.LoadStore(key);
            objStoreBiz.DeleteStore(info);
           
            
            base.DoAlertinAjax(this.Page, "msg", "刪除成功!");
            
            gvListBind();
            this.BindMemberLevel(DropDownList1);
        }
        #endregion
    }
}