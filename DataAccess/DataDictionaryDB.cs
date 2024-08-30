using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Information;

namespace DataAccess
{
    public class DataDictionaryDB : baseDB
    {
        public DataDictionaryDB()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public DataTable InqDataDictionary(string dataType)
        {
            //Database為宣告資料庫功能的物件，提供連線字串的取得
            Database db = base.GetDatabase();

            //用於儲存資料庫回傳的執行結果
            DataSet ds;

            //儲存上述ds的第一張資料表(ds.table[0]) 
            DataTable dt = new DataTable();

            //用於儲存SQL字串，類似於string，但由於採用可變字元序列，適用於構建字串以提高性能。
            StringBuilder sbCmd = new StringBuilder();

            //將SQL語法存進StringBuilder
            sbCmd.Append(" SELECT ValueField, TextField, SEQ ");
            sbCmd.Append(" FROM [DataDictionary]");
            sbCmd.Append(" WHERE 1 = 1 ");

            //判斷變數是否有值，有再填加該段SQL語法
            if (!string.IsNullOrEmpty(dataType))
            {
                sbCmd.AppendLine(@" AND DataType = @dataType ");
            }

            //創建 DbCommand 物件，存入SQL語法。
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            // 向 DbCommand 添加參數，傳入參數分別為"指定的DbCommand"、"SQL中參數名"、"Table中的參數類型"、"參數值"。
            db.AddInParameter(dbCommand, "@dataType", DbType.String, dataType);

            try
            {
                //向資料庫執行操作，取回結果
                ds = db.ExecuteDataSet(dbCommand);

                //判斷ds是否為空，如果有再取出要的table
                if (ds != null && ds.Tables.Count > 0)
                    dt = ds.Tables[0];

                //回傳資料
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

        public void InsertsAddress(string sAddress)
        {
            //Database為宣告資料庫功能的物件，提供連線字串的取得
            Database db = base.GetDatabase();

            //用於儲存SQL字串，類似於string，但採用可變字元序列，能以較高性能構建字串。
            StringBuilder sbCmd = new StringBuilder();

            //將SQL語法存進StringBuilder

            sbCmd.Append("     IF NOT EXISTS                ");
            sbCmd.Append("     (SELECT  [DataType], [ValueField]  FROM [DataDictionary]   ");
            sbCmd.Append("     WHERE [ValueField] = @StoreAddress AND  [DataType] = 'sAddress')  ");
            sbCmd.Append("     BEGIN                ");
            sbCmd.Append(" INSERT INTO [DataDictionary]        ");
            sbCmd.Append("     (                ");
            sbCmd.Append("     [DataType]        ");
            sbCmd.Append("     ,[ValueField]        ");
            sbCmd.Append("     ,[TextField]        ");
            sbCmd.Append("     ,[SEQ]        ");
            sbCmd.Append("     ,[Creator]        ");
            sbCmd.Append("     ,[CreatedDate]        ");
            sbCmd.Append("     ,[Modifier]        ");
            sbCmd.Append("     ,[ModifiedDate]        ");
            sbCmd.Append("     )                ");
            sbCmd.Append(" VALUES        ");
            sbCmd.Append("     (                ");
            sbCmd.Append("     'sAddress'        ");
            sbCmd.Append("     ,@StoreAddress        ");
            sbCmd.Append("     ,@StoreAddress        ");
            sbCmd.Append("     ,'1'        ");
            sbCmd.Append("     , 'webpage'       ");
            sbCmd.Append("     , GETDATE()       ");
            sbCmd.Append("     , 'webpage'       ");
            sbCmd.Append("     , GETDATE()       ");
            sbCmd.Append("     )                ");
            sbCmd.Append("     END                ");
            
            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            
            db.AddInParameter(dbCommand, "@StoreAddress", DbType.String, sAddress);
            
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

        public void DeletesAddress(string sAddress)
        {
            //Database為宣告資料庫功能的物件，提供連線字串的取得
            Database db = base.GetDatabase();

            //用於儲存SQL字串，類似於string，但採用可變字元序列，能以較高性能構建字串。
            StringBuilder sbCmd = new StringBuilder();

            //將SQL語法存進StringBuilder

            sbCmd.Append("     IF NOT EXISTS                ");
            sbCmd.Append("     (SELECT  [Address]  FROM [Store]  ");
            sbCmd.Append("     WHERE [Address] = @StoreAddress  )  ");
            sbCmd.Append("     BEGIN                ");
            sbCmd.Append(" DELETE FROM [DataDictionary]        ");
            sbCmd.Append(" WHERE [ValueField] = @StoreAddress AND [DataType] = 'sAddress'  ");
            sbCmd.Append("     END                ");

            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());


            db.AddInParameter(dbCommand, "@StoreAddress", DbType.String, sAddress);
            
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

        public void UpdatesAddress(StoreInfo info, string lastAddress)
        {
            Database db = base.GetDatabase();

            StringBuilder sbCmd = new StringBuilder();

            sbCmd.Append("     IF NOT EXISTS                ");
            sbCmd.Append("     (SELECT  [Address]  FROM [Store]  ");
            sbCmd.Append("     WHERE [Address] = @lastAddress  )  ");
            sbCmd.Append("     BEGIN                ");

            sbCmd.Append("     IF NOT EXISTS                ");
            sbCmd.Append("     ( SELECT  [DataType], [ValueField]  FROM [DataDictionary]   ");
            sbCmd.Append("     WHERE [ValueField] = @NewAddress AND  [DataType] = 'sAddress')  ");
            sbCmd.Append("     BEGIN                ");
            sbCmd.Append(" UPDATE [DataDictionary]        ");
            sbCmd.Append(" SET [ValueField] = @NewAddress, [TextField] = @NewAddress, [ModifiedDate] = GETDATE()  ");
            sbCmd.Append(" WHERE [ValueField] = @lastAddress AND [DataType] = 'sAddress'  ");
            sbCmd.Append("     END                ");
            sbCmd.Append("     ELSE                ");
            sbCmd.Append("     BEGIN                ");
            sbCmd.Append(" DELETE FROM [DataDictionary]        ");
            sbCmd.Append(" WHERE [ValueField] = @lastAddress AND [DataType] = 'sAddress'  ");
            sbCmd.Append("     END                ");
            sbCmd.Append("     END                ");
              
            //sbCmd.Append("     ELSE                ");
            //sbCmd.Append("     BEGIN               ");

            sbCmd.Append("     IF NOT EXISTS                ");
            sbCmd.Append("     ( SELECT  [DataType], [ValueField]  FROM [DataDictionary]   ");
            sbCmd.Append("     WHERE [ValueField] = @NewAddress AND  [DataType] = 'sAddress')  ");
            sbCmd.Append("     BEGIN                ");
            sbCmd.Append(" INSERT INTO [DataDictionary]        ");
            sbCmd.Append("     (                ");
            sbCmd.Append("     [DataType]        ");
            sbCmd.Append("     ,[ValueField]        ");
            sbCmd.Append("     ,[TextField]        ");
            sbCmd.Append("     ,[SEQ]        ");
            sbCmd.Append("     ,[Creator]        ");
            sbCmd.Append("     ,[CreatedDate]        ");
            sbCmd.Append("     ,[Modifier]        ");
            sbCmd.Append("     ,[ModifiedDate]        ");
            sbCmd.Append("     )                ");
            sbCmd.Append(" VALUES        ");
            sbCmd.Append("     (                ");
            sbCmd.Append("     'sAddress'        ");
            sbCmd.Append("     ,@NewAddress        ");
            sbCmd.Append("     ,@NewAddress        ");
            sbCmd.Append("     ,'1'        ");
            sbCmd.Append("     , 'webpage'       ");
            sbCmd.Append("     , GETDATE()       ");
            sbCmd.Append("     , 'webpage'       ");
            sbCmd.Append("     , GETDATE()       ");
            sbCmd.Append("     )                ");
            //sbCmd.Append("     END                ");
            sbCmd.Append("     END                ");

            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());


            db.AddInParameter(dbCommand, "@lastAddress", DbType.String, lastAddress);
            db.AddInParameter(dbCommand, "@NewAddress", DbType.String, info.Address);

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
    }
}
