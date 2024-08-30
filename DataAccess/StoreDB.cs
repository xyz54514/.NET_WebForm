using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Information;
using System.Data.SqlClient;

namespace DataAccess
{
    public class StoreDB : baseDB
    {
        public StoreDB()
        {
        
        }

        /// <summary>
        /// 查詢有關Customer資料的數量
        /// </summary>
        /// <param name="sName">名稱</param>
        /// <param name="sType">會員等級</param>
        /// <returns></returns>
        public int InqStoreCount(string sStoreName, string sAddress)
        {
            //Database為宣告資料庫功能的物件，提供連線字串的取得
            Database db = base.GetDatabase();

            //用於儲存回傳結果
            object rowCount;

            //用於儲存SQL字串，類似於string，但採用可變字元序列，能以較高性能構建字串。
            StringBuilder sbCmd = new StringBuilder();

            //將SQL語法存進StringBuilder
            sbCmd.AppendLine(@"SELECT COUNT(StoreID) FROM Store S");
            sbCmd.AppendLine(@"WHERE 1 = 1");

            //判斷變數是否有值，有再填加該段SQL語法
            if (!string.IsNullOrWhiteSpace(sStoreName))
            {
                sbCmd.AppendLine(@" AND S.StoreName LIKE @sStoreName ");
            }

            //判斷變數是否有值，有再填加該段SQL語法
            if (!string.IsNullOrWhiteSpace(sAddress))
            {
                sbCmd.AppendLine(@" AND S.Address = @sAddress ");
            }

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@sStoreName", DbType.String, $"%{sStoreName}%");
            db.AddInParameter(dbCommand, "@sAddress", DbType.String, sAddress);

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
        /// 查詢有關的Store資料
        /// </summary>
        /// <param name="sStoreName">店名稱</param>
        /// <param name="sAddress">店址</param>
        /// <param name="iStartRow">起始行數</param>
        /// <param name="iEndRow">結束行數</param>
        /// <returns></returns>
        public DataTable InqStore(string sStoreName, string sAddress, int iStartRow, int iEndRow)
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
            sbCmd.AppendLine("   SELECT *,ROW_NUMBER() OVER(ORDER BY S.StoreID ASC ) AS ROWID ");
            sbCmd.AppendLine("   FROM Store S ");
            sbCmd.AppendLine("   WHERE 1 = 1 ");

            //判斷變數是否有值，有再填加該段SQL語法
            if (!string.IsNullOrEmpty(sStoreName))
            {
                sbCmd.AppendLine(@" AND S.StoreName LIKE @sStoreName ");
            }

            if (!string.IsNullOrEmpty(sAddress))
            {
                sbCmd.AppendLine(@" AND S.Address = @sAddress ");
            }

            sbCmd.AppendLine(" ) TMP ");
            sbCmd.AppendLine(" WHERE TMP.ROWID >= @sStartRow AND TMP.ROWID <= @sEndRow ");

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@sStoreName", DbType.String, $"%{sStoreName}%");
            db.AddInParameter(dbCommand, "@sAddress", DbType.String, sAddress);
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
        /// 新增一筆資料到Customer
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public void InsertStore(StoreInfo info)
        {
            //Database為宣告資料庫功能的物件，提供連線字串的取得
            Database db = base.GetDatabase();

            //用於儲存SQL字串，類似於string，但採用可變字元序列，能以較高性能構建字串。
            StringBuilder sbCmd = new StringBuilder();

            //將SQL語法存進StringBuilder
            sbCmd.Append(" INSERT INTO [Store]        ");
            sbCmd.Append("     (                ");
            sbCmd.Append("     [StoreName]        ");
            sbCmd.Append("     ,[Address]        ");
            sbCmd.Append("     ,[Telephone]        ");
            sbCmd.Append("     ,[Remark]        ");
            sbCmd.Append("     ,[CreateDate]        ");
            sbCmd.Append("     )                ");
            sbCmd.Append(" VALUES        ");
            sbCmd.Append("     (                ");
            sbCmd.Append("     @StoreName        ");
            sbCmd.Append("     ,@Address        ");
            sbCmd.Append("     ,@Telephone        ");
            sbCmd.Append("     ,@Remark        ");
            sbCmd.Append("     , GETDATE()       ");
            sbCmd.Append("     )                ");

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            #region Add In Parameter

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@StoreID", DbType.Int32, info.StoreID);
            db.AddInParameter(dbCommand, "@StoreName", DbType.String, info.StoreName);
            db.AddInParameter(dbCommand, "@Address", DbType.String, info.Address);
            db.AddInParameter(dbCommand, "@Telephone", DbType.String, info.Telephone);
            db.AddInParameter(dbCommand, "@Remark", DbType.String, info.Remark);

            string StoreAddress = info.Address;
            DataDictionaryDB ddb = new DataDictionaryDB();
            ddb.InsertsAddress(StoreAddress);

            #endregion

            try
            {
                //向資料庫插入資料
                db.ExecuteNonQuery(dbCommand);

                //記錄此次操作為成功
                base.ErrFlag = true;
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
        /// 依據PK載入一筆Store資料
        /// </summary>
        /// <returns>true代表成功載入，false代表找不到任何資料</returns>
        public StoreInfo LoadStore(Int32 iStoreID)
        {
            //Database為宣告資料庫功能的物件，提供連線字串的取得
            Database db = base.GetDatabase();

            //用於儲存SQL字串，類似於string，但採用可變字元序列，能以較高性能構建字串。
            StringBuilder sbCmd = new StringBuilder();

            //將SQL語法存進StringBuilder
            sbCmd.Append(" SELECT * FROM [Store] WITH (Nolock) ");
            sbCmd.Append(" WHERE (1=1) ");
            sbCmd.Append("     AND [StoreID] = @StoreID         ");

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            #region Add In Parameter

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@StoreID", DbType.Int32, iStoreID);

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
                    return new StoreInfo(dr);
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
        /// 更新Customer資料
        /// </summary>
        /// <param name="info"></param>
        public bool UpdateStore(StoreInfo info, string lastAddress)
        {
            //Database為宣告資料庫功能的物件，提供連線字串的取得
            Database db = base.GetDatabase();

            //用於儲存SQL字串，類似於string，但採用可變字元序列，能以較高性能構建字串。
            StringBuilder sbCmd = new StringBuilder();

            //將SQL語法存進StringBuilder
            sbCmd.Append(" UPDATE [Store] SET         ");
            sbCmd.Append("        [StoreName] = @StoreName         ");
            sbCmd.Append("     ,[Address] = @Address         ");
            sbCmd.Append("     ,[Telephone] = @Telephone         ");
            //sbCmd.Append("     ,[Type] = @Type         ");
            sbCmd.Append(" WHERE (1=1) ");
            sbCmd.Append("     AND [StoreID] = @StoreID         ");

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            #region Add In Parameter

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@StoreID", DbType.Int32, info.StoreID);
            db.AddInParameter(dbCommand, "@StoreName", DbType.String, info.StoreName);
            db.AddInParameter(dbCommand, "@Address", DbType.String, info.Address);
            db.AddInParameter(dbCommand, "@Telephone", DbType.String, info.Telephone);
            //db.AddInParameter(dbCommand, "@Type", DbType.String, info.Type);
            #endregion

            try
            {
                //更新資料庫中資料，取回受影響的筆數
                int i = db.ExecuteNonQuery(dbCommand);

                if (info.Address != lastAddress)
                {
                    DataDictionaryDB ddb = new DataDictionaryDB();
                    ddb.UpdatesAddress(info, lastAddress);
                }

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
        /// 刪除指定Customer資料
        /// </summary>
        /// <param name="CID"></param>
        /// <returns></returns>
        public bool DeleteStore(StoreInfo info)
        {
            //Database為宣告資料庫功能的物件，提供連線字串的取得
            Database db = base.GetDatabase();

            //用於儲存SQL字串，類似於string，但採用可變字元序列，能以較高性能構建字串。
            StringBuilder sbCmd = new StringBuilder();

            //將SQL語法存進StringBuilder
            sbCmd.Append(" DELETE [Store]        ");
            sbCmd.Append(" WHERE (1=1)         ");
            sbCmd.Append("     AND [StoreID] = @StoreID         ");

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            #region Add In Parameter

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@StoreID", DbType.Int32, info.StoreID);

            #endregion

            try
            {
                //刪除資料庫中指定資料，取回受影響的筆數
                int i = db.ExecuteNonQuery(dbCommand);
                string StoreAddress = info.Address;
                DataDictionaryDB ddb = new DataDictionaryDB();
                ddb.DeletesAddress(StoreAddress);
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
        /// 讀取資料庫範例
        /// </summary>
        /// <param name="variable">變數</param>
        /// <returns></returns>
        public DataSet InqSpecTable(string variable)
        {
            DataSet ds;
            Database db = base.GetDatabase();
            StringBuilder sbcmd = new StringBuilder();

            #region Collect SQL Script
            sbcmd.AppendLine(" SELECT * ");
            sbcmd.AppendLine(" FROM TableName ");
            sbcmd.AppendLine(" WHERE StoreID = @variable ");
            #endregion

            #region Collect Variable
            DbCommand dbCommand = db.GetSqlStringCommand(sbcmd.ToString());
            db.AddInParameter(dbCommand, "@variable", DbType.String, variable);
            #endregion

            //Execute Command
            ds = db.ExecuteDataSet(dbCommand);

            return ds;
        }


        /// 0708 myExport
        /// 
        public DataTable InqStore2(string sName, string Address, DateTime? SDate, DateTime? EDate, int tStartRow, int tEndRow)
        {
            DataSet ds;

            DataTable dt = new DataTable();

            StringBuilder sbCmd = new StringBuilder();
            Database db = base.GetDatabase();

            sbCmd.AppendLine(@" SELECT * ");
            sbCmd.AppendLine(@" FROM ( ");
            sbCmd.AppendLine(@" SELECT *, ROW_NUMBER() OVER(ORDER BY S.StoreID ASC) AS ROWID ");
            sbCmd.AppendLine(@" FROM Store S ");
            sbCmd.AppendLine(@" WHERE 1 = 1 ");

            if (!string.IsNullOrEmpty(sName))
            {
                sbCmd.AppendLine(@" AND S.StoreName LIKE @sName ");
            }
            
            if (!string.IsNullOrEmpty(Address))
            {
                sbCmd.AppendLine(@" AND S.Address = @sAddress ");
            }

            if (SDate != null)
            {
                sbCmd.AppendLine(@" AND S.CreateDate >= @SDate ");
            }

            if (EDate != null)
            {
                sbCmd.AppendLine(@" AND S.CreateDate <= @EDate ");
            }

            sbCmd.AppendLine(@" ) TMP ");
            sbCmd.AppendLine(@" WHERE TMP.ROWID >= @sStartRow AND TMP.ROWID <= @sEndRow ");

            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            db.AddInParameter(dbCommand, "@sName", DbType.String, $"%{sName}%");
            db.AddInParameter(dbCommand, "@sAddress", DbType.String, Address);
            db.AddInParameter(dbCommand, "@SDate", DbType.DateTime, SDate);
            db.AddInParameter(dbCommand, "@EDate", DbType.DateTime, EDate);
            db.AddInParameter(dbCommand, "@sStartRow", DbType.Int32, tStartRow);
            db.AddInParameter(dbCommand, "@sEndRow", DbType.Int32, tEndRow);

            try
            {
                ds = db.ExecuteDataSet(dbCommand);

                if (ds != null && ds.Tables.Count > 0)
                    dt = ds.Tables[0];

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

        public int InqStoreCount2(string sName, string Address, DateTime? SDate, DateTime? EDate)
        {
            Database db = base.GetDatabase();

            object rowCount;

            StringBuilder sbCmd = new StringBuilder();

            sbCmd.AppendLine(@" SELECT COUNT(StoreID) FROM Store S ");
            sbCmd.AppendLine(@" WHERE 1 = 1 ");

            if (!string.IsNullOrEmpty(sName))
            {
                sbCmd.AppendLine(@" AND S.StoreName LIKE @sName ");
            }

            if (!string.IsNullOrEmpty(Address))
            {
                sbCmd.AppendLine(@" AND S.Address = @sAddress ");
            }

            if (SDate != null)
            {
                sbCmd.AppendLine(@" AND S.CreateDate >= @SDate ");
            }

            if (EDate != null)
            {
                sbCmd.AppendLine(@" AND S.CreateDate <= @EDate ");
            }

            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            db.AddInParameter(dbCommand, "@sName", DbType.String, $"%{sName}%");
            db.AddInParameter(dbCommand, "@sAddress", DbType.String, Address);
            db.AddInParameter(dbCommand, "@SDate", DbType.DateTime, SDate);
            db.AddInParameter(dbCommand, "@EDate", DbType.DateTime, EDate);

            try
            {
                rowCount = db.ExecuteScalar(dbCommand);

                if (rowCount == null)   return 0;
                return Convert.ToInt32(rowCount);
            }
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
        /// 查詢有關Employee的資料做匯出
        /// </summary>
        /// <param name="sName"></param>
        /// <param name="sDept"></param>
        /// <param name="SDate"></param>
        /// <param name="EDate"></param>
        /// <returns></returns>
        public DataTable ExpStore(string sName, string Address, DateTime? SDate, DateTime? EDate)
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
            sbCmd.AppendLine(" FROM Store S ");
            sbCmd.AppendLine("   WHERE 1 = 1 ");

            //判斷變數是否有值，有再填加該段SQL語法
            if (!string.IsNullOrEmpty(sName))
            {
                sbCmd.AppendLine(@" AND S.StoreName LIKE @sName ");
            }

            //判斷變數是否有值，有再填加該段SQL語法
            if (!string.IsNullOrEmpty(Address))
            {
                sbCmd.AppendLine(@" AND S.Address = @sAddrss ");
            }

            //判斷變數是否有值，有再填加該段SQL語法
            if (SDate != null)
            {
                sbCmd.AppendLine(@" AND S.CreateDate >= @SDate ");
            }

            //判斷變數是否有值，有再填加該段SQL語法
            if (EDate != null)
            {
                sbCmd.AppendLine(@" AND S.CreateDate <= @EDate ");
            }

            

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@sName", DbType.String, $"%{sName}%");
            db.AddInParameter(dbCommand, "@sAddress", DbType.String, Address);
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
                    sqlBC.DestinationTableName = "Store";
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

        public bool DeleteStore(int StoreID, SqlConnection iConn = null, SqlTransaction iTxn = null)
        {
            //Database為宣告資料庫功能的物件，提供連線字串的取得
            Database db = base.GetDatabase();

            //用於儲存SQL字串，類似於string，但採用可變字元序列，能以較高性能構建字串。
            StringBuilder sbCmd = new StringBuilder();

            //將SQL語法存進StringBuilder
            sbCmd.Append(" DELETE [Store]");
            sbCmd.Append(" WHERE (1=1)");
            sbCmd.Append(" AND [StoreID] = @StoreID");

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            #region Add In Parameter

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@StoreID", DbType.Int32, StoreID);

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

        public bool UpdateStore(StoreInfo info, SqlConnection iConn = null, SqlTransaction iTxn = null)
        {
            //Database為宣告資料庫功能的物件，提供連線字串的取得
            Database db = base.GetDatabase();

            //用於儲存SQL字串，類似於string，但採用可變字元序列，能以較高性能構建字串。
            StringBuilder sbCmd = new StringBuilder();

            //將SQL語法存進StringBuilder
            sbCmd.Append(" UPDATE [Store] SET         ");
            sbCmd.Append("        [StoreName] = @StoreName         ");
            sbCmd.Append("     ,[Address] = @Address         ");
            sbCmd.Append("     ,[Telephone] = @Telephone         ");
            sbCmd.Append("     ,[CreateDate] = @CreateDate         ");
            sbCmd.Append(" WHERE (1=1) ");
            sbCmd.Append("     AND [StoreID] = @StoreID         ");

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            #region Add In Parameter

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@StoreID", DbType.Int32, info.StoreID);
            db.AddInParameter(dbCommand, "@StoreName", DbType.String, info.StoreName);
            db.AddInParameter(dbCommand, "@Address", DbType.String, info.Address);
            db.AddInParameter(dbCommand, "@Telephone", DbType.DateTime, info.Telephone);
            db.AddInParameter(dbCommand, "@CreateDate", DbType.DateTime, info.CreateDate);
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

        public int InsertStore(StoreInfo info, SqlConnection iConn = null, SqlTransaction iTxn = null)
        {
            //用於儲存回傳結果
            object rowCount;

            //Database為宣告資料庫功能的物件，提供連線字串的取得
            Database db = base.GetDatabase();

            //用於儲存SQL字串，類似於string，但採用可變字元序列，能以較高性能構建字串。
            StringBuilder sbCmd = new StringBuilder();

            //將SQL語法存進StringBuilder
            sbCmd.Append(" INSERT INTO [Store]        ");
            sbCmd.Append("     (                ");
            sbCmd.Append("     [StoreName]        ");
            sbCmd.Append("     ,[Address]        ");
            sbCmd.Append("     ,[Telephone]        ");
            sbCmd.Append("     ,[CreateDate]        ");
            sbCmd.Append("     )                ");
            sbCmd.Append(" VALUES        ");
            sbCmd.Append("     (                ");
            sbCmd.Append("     @StoreName        ");
            sbCmd.Append("     ,@Address        ");
            sbCmd.Append("     ,@Telephone        ");
            sbCmd.Append("     ,@CreateDate        ");
            sbCmd.Append("     )                ");

            sbCmd.Append(" SELECT @@IDENTITY AS 'SN' "); //執行完會return這次的流水號

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            #region Add In Parameter

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            //db.AddInParameter(dbCommand, "@StoreID", DbType.Int32, info.StoreID);
            db.AddInParameter(dbCommand, "@StoreName", DbType.String, info.StoreName);
            db.AddInParameter(dbCommand, "@Address", DbType.String, info.Address);
            db.AddInParameter(dbCommand, "@Telephone", DbType.DateTime, info.Telephone);
            db.AddInParameter(dbCommand, "@CreateDate", DbType.DateTime, info.CreateDate);

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
    }

}
