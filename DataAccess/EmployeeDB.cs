using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class EmployeeDB : baseDB
    {
        public EmployeeDB()
        {

        }

        /// <summary>
        /// 查詢有關Employee資料的數量
        /// </summary>
        /// <param name="sName"></param>
        /// <param name="sDept"></param>
        /// <param name="SDate"></param>
        /// <param name="EDate"></param>
        /// <returns></returns>
        public int InqEmployeeCount(string sName, string sDept, DateTime? SDate, DateTime? EDate)
        {
            //Database為宣告資料庫功能的物件，提供連線字串的取得
            Database db = base.GetDatabase();

            //用於儲存回傳結果
            object rowCount;

            //用於儲存SQL字串，類似於string，但採用可變字元序列，能以較高性能構建字串。
            StringBuilder sbCmd = new StringBuilder();

            //將SQL語法存進StringBuilder
            sbCmd.AppendLine(@"SELECT COUNT(Employee_ID) FROM Employee E");
            sbCmd.AppendLine(@"WHERE 1 = 1");

            //判斷變數是否有值，有再填加該段SQL語法
            if (!string.IsNullOrEmpty(sName))
            {
                sbCmd.AppendLine(@" AND E.Employee_Name LIKE @sName ");
            }

            //判斷變數是否有值，有再填加該段SQL語法
            if (!string.IsNullOrEmpty(sDept))
            {
                sbCmd.AppendLine(@" AND E.Dept = @sDept ");
            }

            //判斷變數是否有值，有再填加該段SQL語法
            if (SDate != null)
            {
                sbCmd.AppendLine(@" AND E.JoiningDate >= @SDate ");
            }

            //判斷變數是否有值，有再填加該段SQL語法
            if (EDate != null)
            {
                sbCmd.AppendLine(@" AND E.JoiningDate <= @EDate ");
            }

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@sName", DbType.String, $"%{sName}%");
            db.AddInParameter(dbCommand, "@sDept", DbType.String, sDept);
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
        /// 查詢有關Employee的資料
        /// </summary>
        /// <param name="sName"></param>
        /// <param name="sDept"></param>
        /// <param name="SDate"></param>
        /// <param name="EDate"></param>
        /// <param name="iStartRow"></param>
        /// <param name="iEndRow"></param>
        /// <returns></returns>
        public DataTable InqEmployee(string sName, string sDept, DateTime? SDate, DateTime? EDate, int iStartRow, int iEndRow)
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
            sbCmd.AppendLine("   CASE WHEN Sex = '1' Then '男' WHEN Sex = '2' Then '女' ELSE '' END AS SexNm, ");
            sbCmd.AppendLine("   ROW_NUMBER() OVER(ORDER BY E.Employee_ID ASC ) AS ROWID ");
            sbCmd.AppendLine("   FROM Employee E ");
            sbCmd.AppendLine("   WHERE 1 = 1 ");

            //判斷變數是否有值，有再填加該段SQL語法
            if (!string.IsNullOrEmpty(sName))
            {
                sbCmd.AppendLine(@" AND E.Employee_Name LIKE @sName ");
            }

            //判斷變數是否有值，有再填加該段SQL語法
            if (!string.IsNullOrEmpty(sDept))
            {
                sbCmd.AppendLine(@" AND E.Dept = @sDept ");
            }

            //判斷變數是否有值，有再填加該段SQL語法
            if (SDate != null)
            {
                sbCmd.AppendLine(@" AND E.JoiningDate >= @SDate ");
            }

            //判斷變數是否有值，有再填加該段SQL語法
            if (EDate != null)
            {
                sbCmd.AppendLine(@" AND E.JoiningDate <= @EDate ");
            }
            //SQL前面加@程式碼換行時自動換行
            sbCmd.AppendLine(" ) TMP ");
            sbCmd.AppendLine(" WHERE TMP.ROWID >= @sStartRow AND TMP.ROWID <= @sEndRow ");

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@sName", DbType.String, $"%{sName}%");
            db.AddInParameter(dbCommand, "@sDept", DbType.String, sDept);
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
        /// 查詢有關Employee的資料做匯出
        /// </summary>
        /// <param name="sName"></param>
        /// <param name="sDept"></param>
        /// <param name="SDate"></param>
        /// <param name="EDate"></param>
        /// <returns></returns>
        public DataTable ExpEmployee(string sName, string sDept, DateTime? SDate, DateTime? EDate)
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
            sbCmd.AppendLine("   CASE WHEN Sex = '1' Then '男' WHEN Sex = '2' Then '女' ELSE '' END AS SexNm ");
            sbCmd.AppendLine("   FROM Employee E ");
            sbCmd.AppendLine("   WHERE 1 = 1 ");

            //判斷變數是否有值，有再填加該段SQL語法
            if (!string.IsNullOrEmpty(sName))
            {
                sbCmd.AppendLine(@" AND E.Employee_Name LIKE @sName ");
            }

            //判斷變數是否有值，有再填加該段SQL語法
            if (!string.IsNullOrEmpty(sDept))
            {
                sbCmd.AppendLine(@" AND E.Dept = @sDept ");
            }

            //判斷變數是否有值，有再填加該段SQL語法
            if (SDate != null)
            {
                sbCmd.AppendLine(@" AND E.JoiningDate >= @SDate ");
            }

            //判斷變數是否有值，有再填加該段SQL語法
            if (EDate != null)
            {
                sbCmd.AppendLine(@" AND E.JoiningDate <= @EDate ");
            }

            sbCmd.AppendLine(" ) TMP ");

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@sName", DbType.String, $"%{sName}%");
            db.AddInParameter(dbCommand, "@sDept", DbType.String, sDept);
            db.AddInParameter(dbCommand, "@SDate", DbType.DateTime, SDate);
            db.AddInParameter(dbCommand, "@EDate", DbType.DateTime, EDate);

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

        public bool InsertBySqlBulkCopy(DataTable dtInput)
        {
            bool result = false;

            // 使用 SqlConnection 物件，利用 ConnectionPool 取得資料庫連線
            using (System.Data.SqlClient.SqlConnection Conn = new System.Data.SqlClient.SqlConnection(base.GetConnString()))
            {
                // 開啟資料庫連線
                Conn.Open();

                // 使用 StringBuilder 來建立 SQL 指令字串 理論用不到
                StringBuilder sbCmd = new StringBuilder();

                // 建立 SqlCommand 物件，並指定連線及逾時時間
                SqlCommand Cmd = new SqlCommand();
                Cmd.Connection = Conn;
                Cmd.CommandTimeout = 600;

                #region bulkCopy

                // 使用 SqlBulkCopy 進行大量資料寫入
                using (SqlBulkCopy sqlBC = new SqlBulkCopy(Conn))
                {

                    // 設定一個批次量寫入多少筆資料 
                    sqlBC.BatchSize = 1000;
                    // 設定逾時的秒數 
                    sqlBC.BulkCopyTimeout = 600;
                    // 設定 NotifyAfter 屬性，以便在每複製 10000 個資料列至資料表後，呼叫事件處理常式。 
                    sqlBC.NotifyAfter = 10000;
                    // 設定要寫入的資料庫 
                    sqlBC.DestinationTableName = "Employee";
                    // 對應資料行 
                    foreach (DataColumn dc in dtInput.Columns)
                    {
                        // 將 DataTable 的欄位名稱對應到目標資料表的欄位名稱
                        sqlBC.ColumnMappings.Add(dc.ColumnName, dc.ColumnName);
                    }
                    // 開始整批寫入
                    sqlBC.WriteToServer(dtInput);
                }

                #endregion

                result = true;

                // 關閉資料庫連線
                Conn.Close();
            }

            return result;
        }
    }
}
