using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Configuration;

namespace Information
{
    /// <summary>
    /// 工作清單資訊
    /// </summary>
    public class LogExpInfo : baseDB
    {
        /// <summary>
        /// 建構式 
        /// </summary>
        public LogExpInfo()
        {
            //設定初始值
            SystemName = "WebForm";
            ClassName = "";
            MethodName = "";
            ErrMsg = "";
        }

        #region 公用變數
        /// <summary>
        /// Log Error System Name
        /// </summary>
        public string SystemName;

        /// <summary>
        /// Log Error Class
        /// </summary>
        public string ClassName;

        /// <summary>
        /// Log Err Method Name
        /// </summary>
        public string MethodName;

        /// <summary>
        /// Log Error Msg
        /// </summary>
        public string ErrMsg;
        #endregion

        /// <summary>
        /// 寫入Log
        /// </summary>
        public void Insert()
        {
            //不使用Transaction , 以正確的記錄發生那些Error
            using (System.Transactions.TransactionScope Ts = new System.Transactions.TransactionScope(TransactionScopeOption.Suppress))
            {
                //設定連結字串
                Database db = base.GetDatabase();


                StringBuilder sbCommand = new StringBuilder();

                sbCommand.Append("INSERT INTO ExpLog (ClassName, MethodName, ErrMsg) ");
                sbCommand.Append("VALUES (@ClassName,@MethodName,@ErrMsg) ");


                DbCommand dbCommand = db.GetSqlStringCommand(sbCommand.ToString());

                #region Add In Parameter
                db.AddInParameter(dbCommand, "@ClassName", DbType.String, this.ClassName);
                db.AddInParameter(dbCommand, "@MethodName", DbType.String, this.MethodName);
                db.AddInParameter(dbCommand, "@ErrMsg", DbType.String, this.ErrMsg);
                #endregion

                try
                {
                    db.ExecuteNonQuery(dbCommand);
                }
                catch
                {

                }
            }
        }
    }
}
