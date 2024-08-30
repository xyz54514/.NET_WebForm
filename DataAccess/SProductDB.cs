using System;
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
    public class SProductDB : baseDB
    {
        public SProductDB()
        {

        }

        /// <summary>
        /// 依據PK載入Student資料
        /// </summary>
        /// <returns>true代表成功載入，false代表找不到任何資料</returns>
        public DataTable LoadProduct(Int32 iStoreID)
        {
            //用於儲存資料庫回傳的執行結果
            DataSet ds;

            //儲存上述ds的第一張資料表(ds.table[0]) 
            DataTable dt = new DataTable();

            //Database為宣告資料庫功能的物件，提供連線字串的取得
            Database db = base.GetDatabase();

            //用於儲存SQL字串，類似於string，但採用可變字元序列，能以較高性能構建字串。
            StringBuilder sbCmd = new StringBuilder();

            //將SQL語法存進StringBuilder
            sbCmd.Append(" SELECT * FROM [StoreProduct] WITH (Nolock) ");
            sbCmd.Append(" WHERE (1=1) ");
            sbCmd.Append(" AND [StoreID] = @iStoreID ");
            sbCmd.Append(" ORDER BY SN ASC ");

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            #region Add In Parameter

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@iStoreID", DbType.Int32, iStoreID);

            #endregion

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
        /// 查詢有關Student資料數量
        /// </summary>
        /// <param name="iCourse_ID"></param>
        /// <returns></returns>
        public int InqProductCount(Int32 iStoreID, string sProductID)
        {
            //Database為宣告資料庫功能的物件，提供連線字串的取得
            Database db = base.GetDatabase();

            //用於儲存回傳結果
            object rowCount;

            //用於儲存SQL字串，類似於string，但採用可變字元序列，能以較高性能構建字串。
            StringBuilder sbCmd = new StringBuilder();

            //將SQL語法存進StringBuilder
            sbCmd.Append(" SELECT COUNT(ProductID) FROM [StoreProduct] S ");
            sbCmd.Append(" WHERE (1=1) ");
            sbCmd.Append(" AND [StoreID] = @iStoreID ");
            sbCmd.Append(" AND [ProductID] = @sProductID ");

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@iStoreID", DbType.Int32, iStoreID);
            db.AddInParameter(dbCommand, "@sProductID", DbType.String, sProductID);

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
        /// 新增一筆資料到Student
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public void InsertProduct(SProductInfo info, SqlConnection iConn = null, SqlTransaction iTxn = null)
        {
            //Database為宣告資料庫功能的物件，提供連線字串的取得
            Database db = base.GetDatabase();

            //用於儲存SQL字串，類似於string，但採用可變字元序列，能以較高性能構建字串。
            StringBuilder sbCmd = new StringBuilder();

            //將SQL語法存進StringBuilder
            sbCmd.Append(" INSERT INTO [StoreProduct]        ");
            sbCmd.Append("     (                ");
            sbCmd.Append("     [StoreID]        ");
            sbCmd.Append("     ,[ProductID]        ");
            sbCmd.Append("     ,[ProductName]        ");
            sbCmd.Append("     ,[Price]        ");
            sbCmd.Append("     ,[Remark]        ");
            sbCmd.Append("     )                ");
            sbCmd.Append(" VALUES        ");
            sbCmd.Append("     (                ");
            sbCmd.Append("     @StoreID        ");
            sbCmd.Append("     ,@ProductID        ");
            sbCmd.Append("     ,@ProductName        ");
            sbCmd.Append("     ,@Price        ");
            sbCmd.Append("     ,@Remark        ");
            sbCmd.Append("     )                ");

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            #region Add In Parameter

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@StoreID", DbType.Int32, info.StoreID);
            db.AddInParameter(dbCommand, "@ProductID", DbType.String, info.ProductID);
            db.AddInParameter(dbCommand, "@ProductName", DbType.String, info.ProductName);
            db.AddInParameter(dbCommand, "@Price", DbType.String, info.Price);
            db.AddInParameter(dbCommand, "@Remark", DbType.String, info.Remark);

            #endregion

            try
            {
                //向資料庫插入資料
                db.ExecuteNonQuery(dbCommand, iConn, iTxn);

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
        /// 根據指定的Course_ID & sStudent_ID刪除特定Student資料
        /// </summary>
        /// <param name="Course_ID"></param>
        /// <param name="sStudent_ID"></param>
        /// <param name="iConn"></param>
        /// <param name="iTxn"></param>
        /// <returns></returns>
        public bool DeleteProduct(int StoreID, string sProductID, SqlConnection iConn = null, SqlTransaction iTxn = null)
        {
            //Database為宣告資料庫功能的物件，提供連線字串的取得
            Database db = base.GetDatabase();

            //用於儲存SQL字串，類似於string，但採用可變字元序列，能以較高性能構建字串。
            StringBuilder sbCmd = new StringBuilder();

            //將SQL語法存進StringBuilder
            sbCmd.Append(" DELETE [StoreProduct]");
            sbCmd.Append(" WHERE (1=1)");
            sbCmd.Append(" AND [StoreID] = @StoreID");
            sbCmd.Append(" AND [ProductID] = @ProductID");

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            #region Add In Parameter

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@StoreID", DbType.Int32, StoreID);
            db.AddInParameter(dbCommand, "@ProductID", DbType.String, sProductID);
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
        /// 根據指定的Course_ID刪除Student資料
        /// </summary>
        /// <param name="Course_ID"></param>
        /// <param name="iConn"></param>
        /// <param name="iTxn"></param>
        /// <returns></returns>
        public bool DeleteProduct(int StoreID, SqlConnection iConn = null, SqlTransaction iTxn = null)
        {
            //Database為宣告資料庫功能的物件，提供連線字串的取得
            Database db = base.GetDatabase();

            //用於儲存SQL字串，類似於string，但採用可變字元序列，能以較高性能構建字串。
            StringBuilder sbCmd = new StringBuilder();

            //將SQL語法存進StringBuilder
            sbCmd.Append(" DELETE [StoreProduct]");
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

        /// <summary>
        /// 更新Student資料
        /// </summary>
        /// <param name="info"></param>
        public bool UpdateProduct(SProductInfo info, SqlConnection iConn = null, SqlTransaction iTxn = null)
        {
            //Database為宣告資料庫功能的物件，提供連線字串的取得
            Database db = base.GetDatabase();

            //用於儲存SQL字串，類似於string，但採用可變字元序列，能以較高性能構建字串。
            StringBuilder sbCmd = new StringBuilder();

            //將SQL語法存進StringBuilder
            sbCmd.Append(" UPDATE [StoreProduct] SET         ");
            sbCmd.Append("     [ProductName] = @ProductName         ");
            sbCmd.Append("     ,[Price] = @Price         ");
            sbCmd.Append("     ,[Remark] = @Remark         ");
            sbCmd.Append(" WHERE (1=1) ");
            sbCmd.Append("     AND [StoreID] = @StoreID         ");
            sbCmd.Append("     AND [ProductID] = @ProductID         ");

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            #region Add In Parameter

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@StoreID", DbType.Int32, info.StoreID);
            db.AddInParameter(dbCommand, "@ProductID", DbType.String, info.ProductID);
            db.AddInParameter(dbCommand, "@ProductName", DbType.String, info.ProductName);
            db.AddInParameter(dbCommand, "@Price", DbType.String, info.Price);
            db.AddInParameter(dbCommand, "@Remark", DbType.String, info.Remark);
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
