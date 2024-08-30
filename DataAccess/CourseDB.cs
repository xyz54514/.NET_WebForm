﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Information;

namespace DataAccess
{
    public class CourseDB : baseDB
    {
        public CourseDB()
        {

        }

        /// <summary>
        /// 查詢有關Course資料的數量
        /// </summary>
        /// <param name="CourseName"></param>
        /// <param name="SDate"></param>
        /// <param name="EDate"></param>
        /// <returns></returns>
        public int InqCourseCount(string CourseName, DateTime? SDate, DateTime? EDate)
        {
            //Database為宣告資料庫功能的物件，提供連線字串的取得
            Database db = base.GetDatabase();

            //用於儲存回傳結果
            object rowCount;

            //用於儲存SQL字串，類似於string，但採用可變字元序列，能以較高性能構建字串。
            StringBuilder sbCmd = new StringBuilder();

            //將SQL語法存進StringBuilder
            sbCmd.AppendLine(@"SELECT COUNT(Course_ID) FROM Course C");
            sbCmd.AppendLine(@"WHERE 1 = 1");

            //判斷變數是否有值，有再填加該段SQL語法
            if (!string.IsNullOrEmpty(CourseName))
            {
                sbCmd.AppendLine(@" AND C.Course_Name LIKE @sCourseName ");
            }

            //判斷變數是否有值，有再填加該段SQL語法
            if (SDate != null)
            {
                sbCmd.AppendLine(@" AND C.StartDate >= @SDate ");
            }

            //判斷變數是否有值，有再填加該段SQL語法
            if (EDate != null)
            {
                sbCmd.AppendLine(@" AND C.EndDate <= @EDate ");
            }

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@sCourseName", DbType.String, $"%{CourseName}%");
            db.AddInParameter(dbCommand, "@SDate", DbType.DateTime, SDate);
            db.AddInParameter(dbCommand, "@EDate", DbType.DateTime, EDate);

            //將可能產生錯誤訊息的程式碼撰寫在try中，以便錯誤發生時做適當處理。
            try
            {
                //向資料庫執行操作，取回結果
                rowCount = db.ExecuteScalar(dbCommand);

                //判斷是否有值，如果沒有回傳0
                if (rowCount == null) return 0;

                //有值，將結果轉成int回傳
                return Convert.ToInt32(rowCount);
            }
            //在錯誤發生時，會跳進catch並傳入錯誤訊息，在此撰寫Log進資料庫以便日後Debug
            catch (Exception ex)
            {
                #region 呼叫Base.LogExpInf 進行Exception Log 記錄 (固定寫法)

                //取得發生錯誤的函式名稱
                System.Diagnostics.StackFrame stackFrame = new System.Diagnostics.StackFrame();
                System.Reflection.MethodBase myMethodBase = stackFrame.GetMethod();

                //記錄是否發生錯誤
                base.ErrFlag = false;

                //儲存錯誤訊息
                base.ErrMsg = ex.ToString();

                //記錄發生錯誤的函式名稱
                base.ErrMethodName = myMethodBase.Name.ToString();

                //記錄錯誤資訊至資料庫
                base.LogExpInf();
                #endregion

                //向呼叫者丟回錯誤訊息
                throw;
            }
        }

        /// <summary>
        /// 查詢有關Course的資料
        /// </summary>
        /// <param name="CourseName"></param>
        /// <param name="SDate"></param>
        /// <param name="EDate"></param>
        /// <param name="iStartRow"></param>
        /// <param name="iEndRow"></param>
        /// <returns></returns>
        public DataTable InqCourse(string CourseName, DateTime? SDate, DateTime? EDate, int iStartRow, int iEndRow)
        {
            //用於儲存資料庫回傳的執行結果
            DataSet ds;

            //儲存上述ds的第一張資料表(ds.table[0]) 
            DataTable dt = new DataTable();

            //用於儲存SQL字串，類似於string，但由於採用可變字元序列，適用於構建字串以提高性能。
            StringBuilder sbCmd = new StringBuilder();
            Database db = base.GetDatabase();

            //將SQL語法存進StringBuilder
            sbCmd.AppendLine(" SELECT * ");
            sbCmd.AppendLine(" FROM ( ");
            sbCmd.AppendLine("   SELECT *, ");
            sbCmd.AppendLine("   ROW_NUMBER() OVER(ORDER BY C.Course_ID ASC ) AS ROWID, ");
            sbCmd.AppendLine("   CONVERT(VARCHAR(10),C.StartDate,111) AS SDate, ");
            sbCmd.AppendLine("   CONVERT(VARCHAR(10),C.EndDate,111) AS EDate ");
            sbCmd.AppendLine("   FROM Course C ");
            sbCmd.AppendLine("   WHERE 1 = 1 ");

            //判斷變數是否有值，有再填加該段SQL語法
            if (!string.IsNullOrEmpty(CourseName))
            {
                sbCmd.AppendLine(@" AND C.Course_Name LIKE @sCourseName ");
            }

            //判斷變數是否有值，有再填加該段SQL語法
            if (SDate != null)
            {
                sbCmd.AppendLine(@" AND C.StartDate >= @SDate ");
            }

            //判斷變數是否有值，有再填加該段SQL語法
            if (EDate != null)
            {
                sbCmd.AppendLine(@" AND C.EndDate <= @EDate ");
            }

            sbCmd.AppendLine(" ) TMP ");
            sbCmd.AppendLine(" WHERE TMP.ROWID >= @sStartRow AND TMP.ROWID <= @sEndRow ");

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@sCourseName", DbType.String, $"%{CourseName}%");
            db.AddInParameter(dbCommand, "@SDate", DbType.DateTime, SDate);
            db.AddInParameter(dbCommand, "@EDate", DbType.DateTime, EDate);
            db.AddInParameter(dbCommand, "@sStartRow", DbType.Int32, iStartRow);
            db.AddInParameter(dbCommand, "@sEndRow", DbType.Int32, iEndRow);

            try
            {
                //向資料庫執行操作，取回結果
                ds = db.ExecuteDataSet(dbCommand);

                //判斷ds是否為空，如果有再取出要的table
                if (ds != null && ds.Tables.Count > 0)
                    dt = ds.Tables[0];

                //回傳table
                return dt;
            }
            catch (Exception ex)
            {
                #region 呼叫Base.LogExpInf 進行Exception Log 記錄 (固定寫法)
                System.Diagnostics.StackFrame stackFrame = new System.Diagnostics.StackFrame();
                System.Reflection.MethodBase myMethodBase = stackFrame.GetMethod();

                base.ErrFlag = false;
                base.ErrMsg = ex.ToString();
                base.ErrMethodName = myMethodBase.Name.ToString();
                base.LogExpInf();
                #endregion

                throw;
            }
        }

        /// <summary>
        /// 新增一筆資料到Course
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public int InsertCourse(CourseInfo info, SqlConnection iConn = null, SqlTransaction iTxn = null)
        {
            //用於儲存回傳結果
            object rowCount;

            //Database為宣告資料庫功能的物件，提供連線字串的取得
            Database db = base.GetDatabase();

            //用於儲存SQL字串，類似於string，但採用可變字元序列，能以較高性能構建字串。
            StringBuilder sbCmd = new StringBuilder();

            //將SQL語法存進StringBuilder
            sbCmd.Append(" INSERT INTO [Course]        ");
            sbCmd.Append("     (                ");
            sbCmd.Append("     [Course_Name]        ");
            sbCmd.Append("     ,[Instructor]        ");
            sbCmd.Append("     ,[StartDate]        ");
            sbCmd.Append("     ,[EndDate]        ");
            sbCmd.Append("     )                ");
            sbCmd.Append(" VALUES        ");
            sbCmd.Append("     (                ");
            sbCmd.Append("     @Course_Name        ");
            sbCmd.Append("     ,@Instructor        ");
            sbCmd.Append("     ,@StartDate        ");
            sbCmd.Append("     ,@EndDate        ");
            sbCmd.Append("     )                ");

            sbCmd.Append(" SELECT @@IDENTITY AS 'SN' "); //執行完會return這次的流水號

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            #region Add In Parameter

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@Course_ID", DbType.Int32, info.Course_ID);
            db.AddInParameter(dbCommand, "@Course_Name", DbType.String, info.Course_Name);
            db.AddInParameter(dbCommand, "@Instructor", DbType.String, info.Instructor);
            db.AddInParameter(dbCommand, "@StartDate", DbType.DateTime, info.StartDate);
            db.AddInParameter(dbCommand, "@EndDate", DbType.DateTime, info.EndDate);

            #endregion

            try
            {
                //向資料庫執行操作，取回結果
                rowCount = db.ExecuteScalar(dbCommand, iConn, iTxn);

                //記錄此次操作為成功
                base.ErrFlag = true;

                //判斷是否有值，如果沒有回傳0
                if (rowCount == null) return 0;

                //有值，將結果轉成int回傳
                return Convert.ToInt32(rowCount);   
            }
            catch (Exception ex)
            {
                #region 呼叫Base.LogExpInf 進行Exception Log 記錄 (固定寫法)
                //取得目前MethodName
                System.Diagnostics.StackFrame stackFrame = new System.Diagnostics.StackFrame();
                System.Reflection.MethodBase myMethodBase = stackFrame.GetMethod();

                base.ErrFlag = false;
                base.ErrMsg = ex.ToString();
                base.ErrMethodName = myMethodBase.Name.ToString();
                base.LogExpInf();
                #endregion

                throw; //將原來的 exception 再次的抛出去
            }
        }

        /// <summary>
        /// 刪除指定Course資料
        /// </summary>
        /// <param name="Course_ID"></param>
        /// <returns></returns>
        public bool DeleteCourse(int Course_ID, SqlConnection iConn = null, SqlTransaction iTxn = null)
        {
            //Database為宣告資料庫功能的物件，提供連線字串的取得
            Database db = base.GetDatabase();

            //用於儲存SQL字串，類似於string，但採用可變字元序列，能以較高性能構建字串。
            StringBuilder sbCmd = new StringBuilder();

            //將SQL語法存進StringBuilder
            sbCmd.Append(" DELETE [Course]");
            sbCmd.Append(" WHERE (1=1)");
            sbCmd.Append(" AND [Course_ID] = @Course_ID");

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            #region Add In Parameter

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@Course_ID", DbType.Int32, Course_ID);

            #endregion

            try
            {
                //刪除資料庫中指定資料，取回受影響的筆數
                int i = db.ExecuteNonQuery(dbCommand, iConn, iTxn);

                //依受影響的筆數，判斷是否更新成功
                base.ErrFlag = (i == 0 ? false : true);

                //回傳結果
                return base.ErrFlag;
            }
            catch (Exception ex)
            {
                #region 呼叫Base.LogExpInf 進行Exception Log 記錄 (固定寫法)
                //取得目前MethodName
                System.Diagnostics.StackFrame stackFrame = new System.Diagnostics.StackFrame();
                System.Reflection.MethodBase myMethodBase = stackFrame.GetMethod();

                base.ErrFlag = false;
                base.ErrMsg = ex.ToString();
                base.ErrMethodName = myMethodBase.Name.ToString();
                base.LogExpInf();
                #endregion

                throw; //將原來的 exception 再次的抛出去
            }
        }

        /// <summary>
        /// 依據PK載入一筆Course資料
        /// </summary>
        /// <returns>true代表成功載入，false代表找不到任何資料</returns>
        public CourseInfo LoadCourse(Int32 iCourse_ID)
        {
            //Database為宣告資料庫功能的物件，提供連線字串的取得
            Database db = base.GetDatabase();

            //用於儲存SQL字串，類似於string，但採用可變字元序列，能以較高性能構建字串。
            StringBuilder sbCmd = new StringBuilder();

            //將SQL語法存進StringBuilder
            sbCmd.Append(" SELECT * FROM [Course] WITH (Nolock) ");
            sbCmd.Append(" WHERE (1=1) ");
            sbCmd.Append("     AND [Course_ID] = @Course_ID         ");

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            #region Add In Parameter

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@Course_ID", DbType.Int32, iCourse_ID);

            #endregion

            try
            {
                //向資料庫做操作，取回相關資料
                DataTable dtTemp = db.ExecuteDataSet(dbCommand).Tables[0];

                //判斷是否有資料
                if (dtTemp != null && dtTemp.Rows.Count > 0)
                {
                    //只取出第一張Table
                    DataRow dr = dtTemp.Rows[0];

                    //建構CustomerInfo並填入資料
                    return new CourseInfo(dr);
                }
                else
                {
                    //無相關資料，回傳null
                    return null;
                }
            }
            catch (Exception ex)
            {
                #region 呼叫Base.LogExpInf 進行Exception Log 記錄 (固定寫法)
                //取得目前MethodName
                System.Diagnostics.StackFrame stackFrame = new System.Diagnostics.StackFrame();
                System.Reflection.MethodBase myMethodBase = stackFrame.GetMethod();

                base.ErrFlag = false;
                base.ErrMsg = ex.ToString();
                base.ErrMethodName = myMethodBase.Name.ToString();
                base.LogExpInf();
                #endregion

                throw; //將原來的 exception 再次的抛出去
            }
        }

        /// <summary>
        /// 更新Course資料
        /// </summary>
        /// <param name="info"></param>
        public bool UpdateCourse(CourseInfo info, SqlConnection iConn = null, SqlTransaction iTxn = null)
        {
            //Database為宣告資料庫功能的物件，提供連線字串的取得
            Database db = base.GetDatabase();

            //用於儲存SQL字串，類似於string，但採用可變字元序列，能以較高性能構建字串。
            StringBuilder sbCmd = new StringBuilder();

            //將SQL語法存進StringBuilder
            sbCmd.Append(" UPDATE [Course] SET         ");
            sbCmd.Append("        [Course_Name] = @Course_Name         ");
            sbCmd.Append("     ,[Instructor] = @Instructor         ");
            sbCmd.Append("     ,[StartDate] = @StartDate         ");
            sbCmd.Append("     ,[EndDate] = @EndDate         ");
            sbCmd.Append(" WHERE (1=1) ");
            sbCmd.Append("     AND [Course_ID] = @Course_ID         ");

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            #region Add In Parameter

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@Course_ID", DbType.Int32, info.Course_ID);
            db.AddInParameter(dbCommand, "@Course_Name", DbType.String, info.Course_Name);
            db.AddInParameter(dbCommand, "@Instructor", DbType.String, info.Instructor);
            db.AddInParameter(dbCommand, "@StartDate", DbType.DateTime, info.StartDate);
            db.AddInParameter(dbCommand, "@EndDate", DbType.DateTime, info.EndDate);
            #endregion

            try
            {
                //更新資料庫中資料，取回受影響的筆數
                int i = db.ExecuteNonQuery(dbCommand, iConn, iTxn);

                //依受影響的筆數，判斷是否更新成功
                base.ErrFlag = (i == 0 ? false : true);

                //回傳結果
                return base.ErrFlag;
            }
            catch (Exception ex)
            {
                #region 呼叫Base.LogExpInf 進行Exception Log 記錄 (固定寫法)
                //取得目前MethodName
                System.Diagnostics.StackFrame stackFrame = new System.Diagnostics.StackFrame();
                System.Reflection.MethodBase myMethodBase = stackFrame.GetMethod();

                base.ErrFlag = false;
                base.ErrMsg = ex.ToString();
                base.ErrMethodName = myMethodBase.Name.ToString();
                base.LogExpInf();
                #endregion

                throw; //將原來的 exception 再次的抛出去
            }
        }
    }
}
