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
    public class OrderDB :baseDB
    {
        public OrderDB()
        {

        }
        
        /// <summary>
        /// 查詢有關Product資料的數量
        /// </summary>
        /// <param name="sName">名稱</param>
        /// <returns></returns>
        public int InqOrderCountChecker(string sProductName, string sTime)
        {
            Database db = base.GetDatabase();
            object rowCount;
            StringBuilder sbCmd = new StringBuilder();

            sbCmd.AppendLine(@"SELECT COUNT(OrderID) FROM Orders O");
            sbCmd.AppendLine(@"WHERE 1 = 1");

            if (!string.IsNullOrWhiteSpace(sProductName))
            {
                sbCmd.AppendLine(@" AND O.ProductName LIKE @sProductName ");
            }

            if (!string.IsNullOrWhiteSpace(sTime))// && sTime != "____/__/__")
            {
                sbCmd.AppendLine(@" AND CONVERT(date, O.OrderDate) = @sTime ");
            }
            //Checker顯示待審核資料的數量
            sbCmd.AppendLine(@" AND O.Status = 'C'");

            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());
            db.AddInParameter(dbCommand, "@sProductName", DbType.String, $"%{sProductName}%");
            db.AddInParameter(dbCommand, "@sTime", DbType.String, sTime);
            try
            {
                rowCount = db.ExecuteScalar(dbCommand);

                if (rowCount == null) return 0;

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
        /// 查詢有關Product資料的數量
        /// </summary>
        /// <param name="sName">名稱</param>
        /// <returns></returns>
        public int InqOrderCountMaker(string sProdName, string sTime)
        {
            Database db = base.GetDatabase();
            object rowCount;
            StringBuilder sbCmd = new StringBuilder();

            sbCmd.AppendLine(@"SELECT COUNT(OrderID) FROM Orders O");
            sbCmd.AppendLine(@"WHERE 1 = 1");

            if (!string.IsNullOrWhiteSpace(sProdName))
            {
                sbCmd.AppendLine(@" AND O.ProductName LIKE @sProdName ");
            }

            if (!string.IsNullOrWhiteSpace(sTime))// && sTime != "____/__/__")
            {
                sbCmd.AppendLine(@" AND CONVERT(date, O.OrderDate) = @sTime ");
            }
            //Maker顯示待審核和退回資料的數量
            sbCmd.AppendLine(@" AND O.Status = 'M' OR O.Status = 'C'");

            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());
            db.AddInParameter(dbCommand, "@sProdName", DbType.String, $"%{sProdName}%");
            db.AddInParameter(dbCommand, "@sTime", DbType.String, sTime);// Convert.ToDateTime(sTime));//$"%{sTime}%");
            try
            {
                rowCount = db.ExecuteScalar(dbCommand);

                if (rowCount == null) return 0;

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
        /// 查詢有關的Product資料
        /// </summary>
        /// <param name="sName">名稱</param>
        /// <param name="iStartRow">起始行數</param>
        /// <param name="iEndRow">結束行數</param>
        /// <returns></returns>
        public DataTable InqOrderMaker(string sName, string sTime, int iStartRow, int iEndRow)
        {
            DataSet ds;
            DataTable dt = new DataTable();
            StringBuilder sbCmd = new StringBuilder();
            Database db = base.GetDatabase();

            sbCmd.AppendLine(" SELECT *, ");
            sbCmd.AppendLine("   CASE ");
            sbCmd.AppendLine("     WHEN Status = 'C' THEN '待審核' ");
            sbCmd.AppendLine("     WHEN Status = 'M' THEN '退回' ");
            sbCmd.AppendLine("     ELSE Status ");
            sbCmd.AppendLine("   END AS Status_Ch ");
            sbCmd.AppendLine(" FROM ( ");
            sbCmd.AppendLine("   SELECT *,ROW_NUMBER() OVER(ORDER BY O.OrderID ASC ) AS ROWID ");
            sbCmd.AppendLine("   FROM Orders O ");
            sbCmd.AppendLine("   WHERE Status IN ('C', 'M')");

            if (!string.IsNullOrEmpty(sName))
            {
                sbCmd.AppendLine(@" AND O.ProductName LIKE @sProdName ");
            }

            //判斷變數是否有值，有再填加該段SQL語法
            if (!string.IsNullOrWhiteSpace(sTime))// && sTime != "____ / __ / __")
            {
                sbCmd.AppendLine(@" AND CONVERT(date, O.OrderDate) = @sTime");
            }

            sbCmd.AppendLine(" ) TMP ");
            sbCmd.AppendLine(" WHERE TMP.ROWID >= @sStartRow AND TMP.ROWID <= @sEndRow ");

            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            db.AddInParameter(dbCommand, "@sProdName", DbType.String, $"%{sName}%");
            db.AddInParameter(dbCommand, "@sTime", DbType.String, sTime);//Convert.ToDateTime(sTime));//$"%{sTime}%");
            db.AddInParameter(dbCommand, "@sStartRow", DbType.Int32, iStartRow);
            db.AddInParameter(dbCommand, "@sEndRow", DbType.Int32, iEndRow);

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
        public DataTable InqOrderChecker(string sProductName, string sTime, int iStartRow, int iEndRow)
        {
            DataSet ds;
            DataTable dt = new DataTable();
            StringBuilder sbCmd = new StringBuilder();
            Database db = base.GetDatabase();

            sbCmd.AppendLine(" SELECT *, ");
            sbCmd.AppendLine("   CASE ");
            sbCmd.AppendLine("     WHEN Status = 'C' THEN '待審核' ");
            sbCmd.AppendLine("     WHEN Status = 'M' THEN '退回' ");
            sbCmd.AppendLine("     ELSE Status ");
            sbCmd.AppendLine("   END AS Status_Ch ");
            sbCmd.AppendLine(" FROM ( ");
            sbCmd.AppendLine("   SELECT *,ROW_NUMBER() OVER(ORDER BY O.OrderID ASC ) AS ROWID ");
            sbCmd.AppendLine("   FROM Orders O ");
            sbCmd.AppendLine("   WHERE Status = 'C'");

            if (!string.IsNullOrEmpty(sProductName))
            {
                sbCmd.AppendLine(@" AND O.ProductName LIKE @sProductName ");
            }

            if (!string.IsNullOrWhiteSpace(sTime))// && sTime != "____ / __ / __")
            {
                sbCmd.AppendLine(@" AND CONVERT(date, O.OrderDate) = @sTime");
            }

            sbCmd.AppendLine(" ) TMP ");
            sbCmd.AppendLine(" WHERE TMP.ROWID >= @sStartRow AND TMP.ROWID <= @sEndRow ");

            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            db.AddInParameter(dbCommand, "@sProductName", DbType.String, $"%{sProductName}%");
            db.AddInParameter(dbCommand, "@sTime", DbType.String, sTime);
            db.AddInParameter(dbCommand, "@sStartRow", DbType.Int32, iStartRow);
            db.AddInParameter(dbCommand, "@sEndRow", DbType.Int32, iEndRow);

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
        /// <summary>
        /// 新增Product資料
        /// </summary>
        /// <param name="info"></param>
        public void InsertOrder(OrderInfo info)
        {
            Database db = base.GetDatabase();
            StringBuilder sbCmd = new StringBuilder();

            sbCmd.Append(" INSERT INTO [Orders]        ");
            sbCmd.Append("     (                ");
            sbCmd.Append("     [ProductName]        ");
            sbCmd.Append("     ,[Quantity]        ");
            sbCmd.Append("     ,[UnitPrice]        ");
            sbCmd.Append("     ,[TotalPrice]        ");
            sbCmd.Append("     ,[Status]        ");
            sbCmd.Append("     ,[CreateDate]        ");
            sbCmd.Append("     ,[OrderDate]        ");
            sbCmd.Append("     ,[Creator]        ");
            sbCmd.Append("     )                ");
            sbCmd.Append(" VALUES        ");
            sbCmd.Append("     (                ");
            sbCmd.Append("     @ProductName        ");
            sbCmd.Append("     ,@Quantity        ");
            sbCmd.Append("     ,@UnitPrice        ");
            sbCmd.Append("     ,@TotalPrice        ");
            sbCmd.Append("     ,@Status        ");
            sbCmd.Append("     ,@CreateDate        ");
            sbCmd.Append("     ,@OrderDate        ");
            sbCmd.Append("     ,@Creator        ");
            sbCmd.Append("     )                ");

            sbCmd.Append(" select @@IDENTITY        ");

            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            #region Add In Parameter
            db.AddInParameter(dbCommand, "@ProductName", DbType.String, info.ProductName);
            db.AddInParameter(dbCommand, "@Quantity", DbType.Int32, info.Quantity);
            db.AddInParameter(dbCommand, "@UnitPrice", DbType.Int32, info.UnitPrice);
            db.AddInParameter(dbCommand, "@TotalPrice", DbType.Int32, info.TotalPrice);
            db.AddInParameter(dbCommand, "@Status", DbType.String, info.Status);
            db.AddInParameter(dbCommand, "@CreateDate", DbType.String, info.CreateDate);
            db.AddInParameter(dbCommand, "@OrderDate", DbType.String, info.OrderDate);
            db.AddInParameter(dbCommand, "@Creator", DbType.String, info.Creator);
            #endregion

            try
            {
                db.ExecuteNonQuery(dbCommand);

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
        /// 依據PK載入一筆Product資料
        /// </summary>
        /// <returns>true代表成功載入，false代表找不到任何資料</returns>
        public OrderInfo LoadOrder(Int32 iOrderID)
        {
            Database db = base.GetDatabase();
            StringBuilder sbCmd = new StringBuilder();

            sbCmd.Append(" SELECT * FROM [Orders] WITH (Nolock) ");
            sbCmd.Append(" WHERE (1=1) ");
            sbCmd.Append("     AND [OrderID] = @OrderID         ");

            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            #region Add In Parameter
            db.AddInParameter(dbCommand, "@OrderID", DbType.Int32, iOrderID);
            #endregion

            try
            {
                DataTable dtTemp = db.ExecuteDataSet(dbCommand).Tables[0];

                if (dtTemp != null && dtTemp.Rows.Count > 0)
                {
                    DataRow dr = dtTemp.Rows[0];
                    return new OrderInfo(dr);
                }
                else
                {
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
        /// 更新Product資料
        /// </summary>
        /// <param name="info"></param>
        public bool UpdateOrder(OrderInfo info)
        {
            Database db = base.GetDatabase();
            StringBuilder sbCmd = new StringBuilder();

            sbCmd.Append(" UPDATE [Orders] SET         ");
            sbCmd.Append("        [ProductName] = @ProductName         ");
            sbCmd.Append("     ,[Quantity] = @Quantity         ");
            sbCmd.Append("     ,[UnitPrice] = @UnitPrice         ");
            sbCmd.Append("     ,[TotalPrice] = @TotalPrice         ");
            //sbCmd.Append("     ,[OrderDate] = @OrderDate         ");
            sbCmd.Append("     ,[Status] = @Status         ");
            sbCmd.Append("     ,[Maker] = @Maker         ");
            sbCmd.Append("     ,[Checker] = @Checker         ");
            sbCmd.Append(" WHERE (1=1) ");
            sbCmd.Append("     AND [OrderID] = @OrderID         ");

            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            #region Add In Parameter
            db.AddInParameter(dbCommand, "@ProductName", DbType.String, info.ProductName);
            db.AddInParameter(dbCommand, "@Quantity", DbType.Int32, info.Quantity);
            db.AddInParameter(dbCommand, "@UnitPrice", DbType.Int32, info.UnitPrice);
            db.AddInParameter(dbCommand, "@TotalPrice", DbType.Int32, info.TotalPrice);
            db.AddInParameter(dbCommand, "@Status", DbType.String, info.Status);
            db.AddInParameter(dbCommand, "@OrderID", DbType.String, info.OrderID);
            db.AddInParameter(dbCommand, "@Maker", DbType.String, info.Maker);
            db.AddInParameter(dbCommand, "@Checker", DbType.String, info.Checker);
            #endregion

            try
            {
                int i = db.ExecuteNonQuery(dbCommand);
                base.ErrFlag = (i == 0 ? false : true);
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
        /// 刪除指定Product資料
        /// </summary>
        /// <param name="PID"></param>
        /// <returns></returns>
        public bool DeleteOrder(int OrderID)
        {
            Database db = base.GetDatabase();
            StringBuilder sbCmd = new StringBuilder();

            //將SQL語法存進StringBuilder
            sbCmd.Append(" DELETE [Orders]        ");
            sbCmd.Append(" WHERE (1=1)         ");
            sbCmd.Append("     AND [OrderID] = @OrderID         ");

            DbCommand dbCommand = db.GetSqlStringCommand(sbCmd.ToString());

            #region Add In Parameter
            db.AddInParameter(dbCommand, "@OrderID", DbType.Int32, OrderID);
            #endregion

            try
            {
                int i = db.ExecuteNonQuery(dbCommand);

                base.ErrFlag = (i == 0 ? false : true);

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
