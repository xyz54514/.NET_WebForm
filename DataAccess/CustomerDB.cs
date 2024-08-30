using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Information;

namespace DataAccess
{
    public class CustomerDB : baseDB
    {
        public CustomerDB()
        {

        }

        /// <summary>
        /// 查詢有關Customer資料的數量
        /// </summary>
        /// <param name="sName">名稱</param>
        /// <param name="sType">會員等級</param>
        /// <returns></returns>
        public int InqCustomerCount(string sName, string sType)
        {
            //Database為宣告資料庫功能的物件，提供連線字串的取得
            Database db = base.GetDatabase();

            //用於儲存回傳結果
            object rowCount;

            //用於儲存SQL字串，類似於string，但採用可變字元序列，能以較高性能構建字串。
            StringBuilder sbCmd = new StringBuilder();

            //將SQL語法存進StringBuilder
            sbCmd.AppendLine(@"SELECT COUNT(CID) FROM Customer C");
            sbCmd.AppendLine(@"WHERE 1 = 1");

            //判斷變數是否有值，有再填加該段SQL語法
            if (!string.IsNullOrWhiteSpace(sName))
            {
                sbCmd.AppendLine(@" AND C.Name LIKE @sName ");
            }

            //判斷變數是否有值，有再填加該段SQL語法
            if (!string.IsNullOrWhiteSpace(sType))
            {
                sbCmd.AppendLine(@" AND C.Type = @sType ");
            }

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@sName", DbType.String, $"%{sName}%");
            db.AddInParameter(dbCommand, "@sType", DbType.String, sType);

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
        /// 查詢有關的Customer資料
        /// </summary>
        /// <param name="sName">名稱</param>
        /// <param name="sType">會員等級</param>
        /// <param name="iStartRow">起始行數</param>
        /// <param name="iEndRow">結束行數</param>
        /// <returns></returns>
        public DataTable InqCustomer(string sName, string sType, int iStartRow, int iEndRow)
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
            sbCmd.AppendLine("   SELECT *,ROW_NUMBER() OVER(ORDER BY C.CID ASC ) AS ROWID ");
            sbCmd.AppendLine("   FROM Customer C ");
            sbCmd.AppendLine("   WHERE 1 = 1 ");

            //判斷變數是否有值，有再填加該段SQL語法
            if (!string.IsNullOrEmpty(sName))
            {
                sbCmd.AppendLine(@" AND C.Name LIKE @sName ");
            }

            if (!string.IsNullOrEmpty(sType))
            {
                sbCmd.AppendLine(@" AND C.Type = @sType ");
            }

            sbCmd.AppendLine(" ) TMP ");
            sbCmd.AppendLine(" WHERE TMP.ROWID >= @sStartRow AND TMP.ROWID <= @sEndRow ");

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@sName", DbType.String, $"%{sName}%"); //$是為了接模糊查詢使用
            db.AddInParameter(dbCommand, "@sType", DbType.String, sType);
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
        public void InsertCustomer(CustomerInfo info)
        {
            //Database為宣告資料庫功能的物件，提供連線字串的取得
            Database db = base.GetDatabase();

            //用於儲存SQL字串，類似於string，但採用可變字元序列，能以較高性能構建字串。
            StringBuilder sbCmd = new StringBuilder();

            //將SQL語法存進StringBuilder
            sbCmd.Append(" INSERT INTO [Customer]        ");
            sbCmd.Append("     (                ");
            sbCmd.Append("     [Name]        ");
            sbCmd.Append("     ,[City]        ");
            sbCmd.Append("     ,[Phone]        ");
            sbCmd.Append("     ,[Type]        ");
            sbCmd.Append("     )                ");
            sbCmd.Append(" VALUES        ");
            sbCmd.Append("     (                ");
            sbCmd.Append("     @Name        ");
            sbCmd.Append("     ,@City        ");
            sbCmd.Append("     ,@Phone        ");
            sbCmd.Append("     ,@Type        ");
            sbCmd.Append("     )                ");

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            #region Add In Parameter

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@CID", DbType.Int32, info.CID);
            db.AddInParameter(dbCommand, "@Name", DbType.String, info.Name);
            db.AddInParameter(dbCommand, "@City", DbType.String, info.City);
            db.AddInParameter(dbCommand, "@Phone", DbType.String, info.Phone);
            db.AddInParameter(dbCommand, "@Type", DbType.String, info.Type);

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
        /// 依據PK載入一筆Customer資料
        /// </summary>
        /// <returns>true代表成功載入，false代表找不到任何資料</returns>
        public CustomerInfo LoadCustomer(Int32 iCID)
        {
            //Database為宣告資料庫功能的物件，提供連線字串的取得
            Database db = base.GetDatabase();

            //用於儲存SQL字串，類似於string，但採用可變字元序列，能以較高性能構建字串。
            StringBuilder sbCmd = new StringBuilder();

            //將SQL語法存進StringBuilder
            sbCmd.Append(" SELECT * FROM [Customer] WITH (Nolock) ");
            sbCmd.Append(" WHERE (1=1) ");
            sbCmd.Append("     AND [CID] = @CID         ");

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            #region Add In Parameter

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@CID", DbType.Int32, iCID);

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
                    return new CustomerInfo(dr);
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
        public bool UpdateCustomer(CustomerInfo info)
        {
            //Database為宣告資料庫功能的物件，提供連線字串的取得
            Database db = base.GetDatabase();

            //用於儲存SQL字串，類似於string，但採用可變字元序列，能以較高性能構建字串。
            StringBuilder sbCmd = new StringBuilder();

            //將SQL語法存進StringBuilder
            sbCmd.Append(" UPDATE [Customer] SET         ");
            sbCmd.Append("        [Name] = @Name         ");
            sbCmd.Append("     ,[City] = @City         ");
            sbCmd.Append("     ,[Phone] = @Phone         ");
            sbCmd.Append("     ,[Type] = @Type         ");
            sbCmd.Append(" WHERE (1=1) ");
            sbCmd.Append("     AND [CID] = @CID         ");

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            #region Add In Parameter

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@CID", DbType.Int32, info.CID);
            db.AddInParameter(dbCommand, "@Name", DbType.String, info.Name);
            db.AddInParameter(dbCommand, "@City", DbType.String, info.City);
            db.AddInParameter(dbCommand, "@Phone", DbType.String, info.Phone);
            db.AddInParameter(dbCommand, "@Type", DbType.String, info.Type);
            #endregion

            try
            {
                //更新資料庫中資料，取回受影響的筆數
                int i = db.ExecuteNonQuery(dbCommand);

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
        public bool DeleteCustomer(int CID)
        {
            //Database為宣告資料庫功能的物件，提供連線字串的取得
            Database db = base.GetDatabase();

            //用於儲存SQL字串，類似於string，但採用可變字元序列，能以較高性能構建字串。
            StringBuilder sbCmd = new StringBuilder();

            //將SQL語法存進StringBuilder
            sbCmd.Append(" DELETE [Customer]        ");
            sbCmd.Append(" WHERE (1=1)         ");
            sbCmd.Append("     AND [CID] = @CID         ");

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            #region Add In Parameter

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@CID", DbType.Int32, CID);

            #endregion

            try
            {
                //刪除資料庫中指定資料，取回受影響的筆數
                int i = db.ExecuteNonQuery(dbCommand);

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
            sbcmd.AppendLine(" WHERE CID = @variable ");
            #endregion

            #region Collect Variable
            DbCommand dbCommand = db.GetSqlStringCommand(sbcmd.ToString());
            db.AddInParameter(dbCommand, "@variable", DbType.String, variable);
            #endregion

            //Execute Command
            ds = db.ExecuteDataSet(dbCommand);

            return ds;
        }
    }
}
