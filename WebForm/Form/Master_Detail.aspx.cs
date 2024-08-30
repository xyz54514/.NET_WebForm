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
    public partial class Master_Detail : basePage
    {
        #region Variable

        private const int igvListPageCount = 10;

        //記錄學生資訊
        private DataTable StudentDt
        {
            //ViewState可存各型別資料，存在期間僅限在該頁面中，頁面刷新仍存在
            get { return ViewState["StudentDt"] as DataTable ?? new DataTable(); }
            set { ViewState["StudentDt"] = value; }
        }

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
                    //設定預設值
                    hdCourseName.Value = string.Empty;
                    hdSDate.Value = null;
                    hdEDate.Value = null;

                    //綁訂清單
                    this.gvListBind();
                }
            }
            //在錯誤發生時，會跳進catch並傳入錯誤訊息，在此撰寫Log進資料庫以便日後查看，同時彈出訊息給使用者
            catch (Exception ex)
            {
                #region 紀錄Log

                //呼叫LogExpBiz 進行Exception Log 記錄
                LogExpBiz objLogExpBiz = new LogExpBiz();

                //傳入className、methodName、exception供記錄用
                objLogExpBiz.InsertLogExp("Master_Detail", "Page_Load", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;

                #endregion
            }
        }

        #endregion

        #region button Function

        #region 共用

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
                        AddCourse();
                        break;
                    case "MODIFY":
                        ModifyCourse();
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
                objLogExpBiz.InsertLogExp("Master_Detail", "btnSave_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;

                #endregion
            }
        }

        #endregion

        #region Search Panel

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
                hdCourseName.Value = txtCourse_Name.Text.Trim();
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
                objLogExpBiz.InsertLogExp("Master_Detail", "btnQuery_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;

                #endregion
            }
        }

        #endregion

        #region Function Panel

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                #region 初始化

                //初始化畫面
                this.ClosePanel();
                this.OpenPanel(Mode.AddNew);

                //初始化新增頁面 Master區塊
                txtMCourse_Name.Text = string.Empty;
                txtMInstructor.Text = string.Empty;
                ucMStartDate.Text = string.Empty;
                ucMEndDate.Text = string.Empty;

                //初始化新增頁面 Detail區塊
                gvDList.DataSource = null;
                gvDList.DataBind();

                //初始化ViewState
                DataTable dt = new DataTable();
                dt.Columns.Add("SN", typeof(int)); //流水號，對應用
                dt.Columns.Add("Coursd_ID", typeof(int)); //為儲存時空值
                dt.Columns.Add("Student_ID", typeof(string));
                dt.Columns.Add("Student_Name", typeof(string));
                dt.Columns.Add("Phone", typeof(string));
                dt.Columns.Add("Email", typeof(string));
                dt.Columns.Add("Status", typeof(string));
                StudentDt = dt;

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
                objLogExpBiz.InsertLogExp("Master_Detail", "btnAdd_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;

                #endregion
            }
        }

        #endregion

        #region Master Panel

        protected void btnMDelte_Click(object sender, EventArgs e)
        {
            CourseBiz objCourseBiz;
            StudentBiz objStudentBiz;
            SqlConnection CONNSEC;
            SqlTransaction myTransaction;

            #region Transaction設置

            try
            {
                objCourseBiz = new CourseBiz();
                objStudentBiz = new StudentBiz();

                //獲取DB Connection
                CONNSEC = new SqlConnection(objCourseBiz.GetConnString());

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
                int Course_ID = Convert.ToInt32(hdCourse_ID.Value);

                //刪除主表Course資料
                objCourseBiz.DeleteCourse(Course_ID, CONNSEC, myTransaction);

                //刪除副表Student資料
                objStudentBiz.DeleteStudent(Course_ID, CONNSEC, myTransaction);

                //認可本次交易
                myTransaction.Commit();

                //彈出提示訊息給使用者
                base.DoAlertinAjax(this.Page, "msg", "刪除成功！");

                //重新查詢資料
                gvListBind();

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

        #endregion

        #region Detail Panel

        protected void btnDAdd_Click(object sender, EventArgs e)
        {
            try
            {
                #region DataTable資料處理

                //DataTable是call by referance
                DataTable dtTmp = StudentDt;

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
                objLogExpBiz.InsertLogExp("Master_Detail", "btnDAdd_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;

                #endregion
            }
        }

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
            DataTable dt = StudentDt;

            //Status欄位紀錄為M (表示修改)
            dt.Rows[rowIndex]["Status"] = "M";

            #endregion

            #region 設置控制項Visible Enable

            btn.Visible = false;

            TextBox txtStudent_Name = (TextBox)gvr.FindControl("txtDStudent_Name");
            txtStudent_Name.Enabled = txtStudent_Name.Visible = true;
            Label lblStudent_Name = (Label)gvr.FindControl("lblDStudent_Name");
            lblStudent_Name.Visible = false;

            TextBox txtPhone = (TextBox)gvr.FindControl("txtDPhone");
            txtPhone.Enabled = txtPhone.Visible = true;
            Label lblPhone = (Label)gvr.FindControl("lblDPhone");
            lblPhone.Visible = false;

            TextBox txtEmail = (TextBox)gvr.FindControl("txtDEmail");
            txtEmail.Enabled = txtEmail.Visible = true;
            Label lblEmail = (Label)gvr.FindControl("lblDEmail");
            lblEmail.Visible = false;

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
            DataTable dt = StudentDt;

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

        #endregion

        #endregion

        #region 自定義 Function

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

        /// <summary>
        ///  檢查關鍵字欄位是否錯誤
        /// </summary>
        /// <returns></returns>
        protected string SearchFieldCheck()
        {
            StringBuilder sb = new StringBuilder();
            DateTime SDate;
            DateTime EDate;
            bool SDateValid = DateTime.TryParse(ucStartDate.Text, out SDate);
            bool EDateValid = DateTime.TryParse(ucEndDate.Text, out EDate);

            if (txtCourse_Name.Text.Trim().Length > 50)
            {
                sb.Append("CourseName的字數不能超過50個字！\\n");
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

        /// <summary>
        /// 儲存前欄位檢核
        /// </summary>
        /// <returns></returns>
        protected string SaveFieldCheck()
        {
            #region 變數宣告

            StringBuilder sb = new StringBuilder();
            string sCourse_Name = txtMCourse_Name.Text.Trim();
            DateTime SDate;
            DateTime EDate;
            bool SDateValid = DateTime.TryParse(ucMStartDate.Text, out SDate);
            bool EDateValid = DateTime.TryParse(ucMEndDate.Text, out EDate);

            #endregion

            #region Master

            if (string.IsNullOrEmpty(sCourse_Name))
                sb.Append("CourseName不可為空！\\n");

            if (string.IsNullOrEmpty(ucMStartDate.Text))
                sb.Append("起始日期不可為空！\\n");
            else if (!SDateValid)
                sb.Append("起始日期格式錯誤！\\n");

            if (string.IsNullOrEmpty(ucMEndDate.Text))
                sb.Append("結束日期不可為空！\\n");
            else if (!EDateValid)
                sb.Append("結束日期格式錯誤！\\n");

            if ((SDateValid && EDateValid) && SDate > EDate)
                sb.Append("起始日期不能晚於結束日期！\\n");

            #endregion

            #region Detail

            DataTable dtDetail = StudentDt;
            string sStudent_ID = string.Empty;
            string sStatus = string.Empty;

            //需重新執行此函式一次，將最新一筆的Row資料寫入ViewState，
            DetailTableSet(gvDList, dtDetail);

            //正規表達式條件
            string pattern = @"^[A-Za-z]\d{6}$";

            int Del_Count = dtDetail.AsEnumerable().Count(row => row["Status"].ToString() == "D") - 1;
            for (int i = 0; i < dtDetail.Rows.Count; i++)
            {
                sStudent_ID = dtDetail.Rows[i]["Student_ID"].ToString();
                sStatus = dtDetail.Rows[i]["Status"].ToString();

                if (string.IsNullOrEmpty(sStudent_ID))
                    sb.Append($"第{gvDList.Rows[i].RowIndex - Del_Count}位學生Student_ID不可為空！\\n");
                else if (!Regex.IsMatch(sStudent_ID, pattern))
                    sb.Append($"第{gvDList.Rows[i].RowIndex - Del_Count}位學生Student_ID格式錯誤！\\n");
            }

            //使用LINQ檢查目前Table內Student_ID是否有重複值
            bool CheckID = dtDetail.AsEnumerable()
                          .Where(row => row["Status"].ToString() != "D")
                          .Where(row => row["Student_ID"] != DBNull.Value && !string.IsNullOrWhiteSpace(row.Field<string>("Student_ID")))
                          .Select(row => row["Student_ID"])
                          .GroupBy(id => id)
                          .Any(g => g.Count() > 1);

            if (CheckID)
                sb.Append($"禁止輸入重複的Sudent_ID！\\n");

            #endregion

            //回傳錯誤訊息
            return sb.ToString();
        }

        /// <summary>
        /// 關閉全部的Panel
        /// </summary>
        private void ClosePanel()
        {
            plQuery.Visible = false;
            plFunction.Visible = false;
            plList.Visible = false;
            plSet.Visible = false;
        }

        /// <summary>
        /// 開啟對應模式的Panel
        /// </summary>
        /// <param name="mode">操作模式</param>
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

        /// <summary>
        /// 設置編輯Panel
        /// </summary>
        /// <param name="key"></param>
        protected void SetModifyPanel(int Course_ID)
        {
            //顯示刪除按鈕
            btnMDel.Visible = true;

            #region Master

            CourseBiz objCourseBiz = new CourseBiz();

            //用key從資料庫讀取該筆資料
            CourseInfo info = objCourseBiz.LoadCourse(Course_ID);

            //將資料放入欄位中
            txtMCourse_Name.Text = info.Course_Name;
            txtMInstructor.Text = info.Instructor;
            ucMStartDate.Text = info.StartDate.ToString();
            ucMEndDate.Text = info.EndDate.ToString();

            #endregion

            #region Detail

            StudentBiz objStudentBiz = new StudentBiz();

            DataTable dtDetail = objStudentBiz.LoadStudent(Course_ID);

            //新增IsDel欄位，讓欄位跟student型態一樣
            dtDetail.Columns.Add("Status", typeof(string));

            //使用ViewState暫存原本的Student資料
            StudentDt = dtDetail;

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
            hdCourse_ID.Value = Course_ID.ToString();
        }

        /// <summary>
        /// 新增Course
        /// </summary>
        protected void AddCourse()
        {
            CourseBiz objCourseBiz;
            StudentBiz objStudentBiz;
            SqlConnection CONNSEC;
            SqlTransaction myTransaction;

            #region Transaction設置
            //啟用transaction
            try
            {
                objCourseBiz = new CourseBiz();
                objStudentBiz = new StudentBiz();

                //獲取DB Connection
                CONNSEC = new SqlConnection(objCourseBiz.GetConnString());

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
                CourseInfo Cinfo = new CourseInfo();
                Cinfo.Course_Name = txtMCourse_Name.Text.Trim();
                Cinfo.Instructor = txtMInstructor.Text.Trim();
                Cinfo.StartDate = Convert.ToDateTime(ucMStartDate.Text);
                Cinfo.EndDate = Convert.ToDateTime(ucMEndDate.Text);

                //寫入資料庫並回傳本次新增資料流水號
                int SN = objCourseBiz.InsertCourse(Cinfo, CONNSEC, myTransaction);

                #endregion

                #region Detail

                StudentInfo Sinfo = new StudentInfo();
                DataTable dtDetail = StudentDt; //有新存的student資料

                for (int i = 0; i < dtDetail.Rows.Count; i++)
                {
                    Sinfo.Course_ID = SN;
                    Sinfo.Student_ID = dtDetail.Rows[i]["Student_ID"].ToString();
                    Sinfo.Student_Name = dtDetail.Rows[i]["Student_Name"].ToString();
                    Sinfo.Phone = dtDetail.Rows[i]["Phone"].ToString();
                    Sinfo.Email = dtDetail.Rows[i]["Email"].ToString();
                    objStudentBiz.InsertStudent(Sinfo, CONNSEC, myTransaction);
                }

                #endregion

                #region Transaction Success

                //認可本次交易
                myTransaction.Commit();

                //彈出提示訊息給使用者
                base.DoAlertinAjax(this.Page, "msg", "新增成功！");

                //重新查詢資料
                gvListBind();

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

        /// <summary>
        /// 編輯Course
        /// </summary>
        protected void ModifyCourse()
        {
            CourseBiz objCourseBiz;
            StudentBiz objStudentBiz;
            SqlConnection CONNSEC;
            SqlTransaction myTransaction;

            #region Transaction設置

            try
            {
                objCourseBiz = new CourseBiz();
                objStudentBiz = new StudentBiz();

                //獲取DB Connection
                CONNSEC = new SqlConnection(objCourseBiz.GetConnString());

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

                int Course_ID = Convert.ToInt32(hdCourse_ID.Value);

                //利用key讀取該筆資料
                CourseInfo Cinfo = objCourseBiz.LoadCourse(Course_ID);

                //存入修改資料
                Cinfo.Course_ID = Course_ID;
                Cinfo.Course_Name = txtMCourse_Name.Text.Trim();
                Cinfo.Instructor = txtMInstructor.Text.Trim();
                Cinfo.StartDate = Convert.ToDateTime(ucMStartDate.Text);
                Cinfo.EndDate = Convert.ToDateTime(ucMEndDate.Text);

                //更新至資料庫
                objCourseBiz.UpdateCourse(Cinfo, CONNSEC, myTransaction);

                #endregion

                #region Detail

                objStudentBiz = new StudentBiz();
                StudentInfo Sinfo = new StudentInfo();
                DataTable dtDetail = StudentDt;
                string sStatus = string.Empty;

                for (int i = 0; i < dtDetail.Rows.Count; i++)
                {
                    //判斷資料狀態
                    sStatus = dtDetail.Rows[i]["Status"].ToString();

                    switch (sStatus.ToUpper())
                    {
                        //編輯
                        case "M":
                            Sinfo.Course_ID = Course_ID;
                            Sinfo.Student_ID = dtDetail.Rows[i]["Student_ID"].ToString();
                            Sinfo.Student_Name = dtDetail.Rows[i]["Student_Name"].ToString();
                            Sinfo.Phone = dtDetail.Rows[i]["Phone"].ToString();
                            Sinfo.Email = dtDetail.Rows[i]["Email"].ToString();
                            objStudentBiz.UpdateStudent(Sinfo, CONNSEC, myTransaction);
                            break;
                        //刪除
                        case "D":
                            objStudentBiz.DeleteStudent(Course_ID, dtDetail.Rows[i]["Student_ID"].ToString(), CONNSEC, myTransaction);
                            break;
                        //新增
                        case "A":
                            Sinfo.Course_ID = Course_ID;
                            Sinfo.Student_ID = dtDetail.Rows[i]["Student_ID"].ToString();
                            Sinfo.Student_Name = dtDetail.Rows[i]["Student_Name"].ToString();
                            Sinfo.Phone = dtDetail.Rows[i]["Phone"].ToString();
                            Sinfo.Email = dtDetail.Rows[i]["Email"].ToString();
                            objStudentBiz.InsertStudent(Sinfo, CONNSEC, myTransaction);
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

        #endregion

        #region gvList Function

        #region 分頁

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
                objLogExpBiz.InsertLogExp("Master_Detail", "btnFirst_Click", ex);

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
                objLogExpBiz.InsertLogExp("Master_Detail", "btnPre_Click", ex);

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
                objLogExpBiz.InsertLogExp("Master_Detail", "btnNext_Click", ex);

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
                objLogExpBiz.InsertLogExp("Master_Detail", "btnLast_Click", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;
                #endregion
            }
        }

        #endregion

        protected void gvListBind()
        {
            //取出搜尋關鍵字
            string CName = hdCourseName.Value;
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

            //取得資料數量
            CourseBiz objCourseBiz = new CourseBiz();
            int rowCount = objCourseBiz.InqCourseCount(CName, SDate, EDate);

            //用於儲存開始與結束的行數
            int tStartRow = 0;
            int tEndRow = 0;

            //計算分頁數量，並儲存當前頁與總頁數至hiddenfield
            this.Pagination(rowCount, ref tStartRow, ref tEndRow);

            //向DB查詢資料
            DataTable dt = objCourseBiz.InqCourse(CName, SDate, EDate, tStartRow, tEndRow);

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
                objLogExpBiz.InsertLogExp("Master_Detail", "gvList_RowCommand", ex);

                //彈出錯誤訊息給使用者
                ScriptManager.RegisterStartupScript(Page, GetType(), "ServerErr", "alert('服務異常, 請稍後再試(" + DateTime.Now.ToString("yyyy\\/MM\\/dd\\/HH:mm:ss:fff") + ")');", true);
                return;
                #endregion
            }
        }

        #endregion

        #region gvDList Function

        /// <summary>
        /// gvDlist資料處理
        /// </summary>
        /// <param name="gv"></param>
        /// <param name="dt"></param>
        protected void DetailTableSet(GridView gv, DataTable dt)
        {
            for (int i = 0; i < gv.Rows.Count; i++)
            {
                if (hdMode.Value.ToUpper() == "MODIFY")
                    dt.Rows[i]["Course_ID"] = hdCourse_ID.Value;

                TextBox txtStudent_ID = (TextBox)gvDList.Rows[i].FindControl("txtDStudent_ID");
                dt.Rows[i]["Student_ID"] = txtStudent_ID.Text.Trim();

                TextBox txtStudent_Name = (TextBox)gvDList.Rows[i].FindControl("txtDStudent_Name");
                dt.Rows[i]["Student_Name"] = txtStudent_Name.Text.Trim();

                TextBox txtPhone = (TextBox)gvDList.Rows[i].FindControl("txtDPhone");
                dt.Rows[i]["Phone"] = txtPhone.Text.Trim();

                TextBox txtEmail = (TextBox)gvDList.Rows[i].FindControl("txtDEmail");
                dt.Rows[i]["Email"] = txtEmail.Text.Trim();
            }
        }

        protected void gvDList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //綁定控制項
                Button btnModify = e.Row.FindControl("btnDModify") as Button;
                TextBox txtStudent_ID = e.Row.FindControl("txtDStudent_ID") as TextBox;
                Label lblStudent_ID = e.Row.FindControl("lblDStudent_ID") as Label;
                TextBox txtStudent_Name = e.Row.FindControl("txtDStudent_Name") as TextBox;
                Label lblStudent_Name = e.Row.FindControl("lblDStudent_Name") as Label;
                TextBox txtPhone = e.Row.FindControl("txtDPhone") as TextBox;
                Label lblPhone = e.Row.FindControl("lblDPhone") as Label;
                TextBox txtEmail = e.Row.FindControl("txtDEmail") as TextBox;
                Label lblEmail = e.Row.FindControl("lblDEmail") as Label;

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

                    txtStudent_ID.Enabled = txtStudent_ID.Visible = status == "A";
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

                    lblStudent_ID.Visible = !(status == "A");
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

                    txtStudent_Name.Enabled = txtStudent_Name.Visible = status == "M" || status == "A";
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

                    lblStudent_Name.Visible = status == "";
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

                    txtPhone.Enabled = txtPhone.Visible = status == "M" || status == "A";
                    lblPhone.Visible = status == "";

                    txtEmail.Enabled = txtEmail.Visible = status == "M" || status == "A";
                    lblEmail.Visible = status == "";
                }
            }
        }

        #endregion
    }
}