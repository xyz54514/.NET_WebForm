using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using SEC;

namespace DataAccess
{
    /// <summary>
    /// 資料庫連線
    /// </summary>
    public class baseDB
    {
        #region 基本處理
        /// <summary>
        /// 連線字串
        /// </summary>
        private string ConnectionString = ConfigurationManager.AppSettings["ConnectionString"].ToString();
        private Coder Coder = new Coder();

        /// <summary>
        /// 使用InstanceName取得資料連線設定
        /// </summary>
        /// <param name="InstanceName"></param>
        /// <returns></returns>
        protected Database GetDatabase()
        {
            if (!string.IsNullOrEmpty(ConnectionString))
            {
                return new Database(Coder.Decrypt(ConnectionString));
            }
            else
            {
                return null;
            }
        }

        public string GetConnString()
        {
            if (!string.IsNullOrEmpty(ConnectionString))
            {
                return Coder.Decrypt(ConnectionString);
            }
            else
            {
                return null;
            }
        }


        public DbConnection CreateConnection()
        {
            Database db = GetDatabase();
            return db.CreateConnection();
        }

        public DbTransaction GetDbTransaction(DbConnection conn)
        {
            if (conn.State == ConnectionState.Closed) conn.Open();
            return conn.BeginTransaction();
        }


        #endregion

        #region Log Functions

        #region ErrorMessage
        /// <summary>
        /// 錯誤狀態Info
        /// </summary>
        public Information.ErrInfo ErrInfo = new Information.ErrInfo();

        /// <summary>
        /// 錯誤檢查 Trus為執行成功 / False為發生錯誤
        /// </summary>
        public bool ErrFlag
        {
            get { return ErrInfo.ErrFlag; }
            set
            {
                ErrInfo.ErrFlag = value;

                //狀態清除時重設相關欄位
                if (value)
                {
                    this.ErrInfo.ErrMethodName = "";
                    this.ErrInfo.ErrMsg = "";
                }
            }
        }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrMsg
        {
            get { return ErrInfo.ErrMsg; }
            set { ErrInfo.ErrMsg = value; }
        }

        /// <summary>
        /// 錯誤Method
        /// </summary>
        public string ErrMethodName
        {
            get { return ErrInfo.ErrMethodName; }
            set { ErrInfo.ErrMethodName = value; }
        }
        #endregion

        #region TrackMessage
        /// <summary>
        /// Track Mode ( Add / Mod / Del )
        /// </summary>
        [NonSerialized()]
        public string TrackMode;

        /// <summary>
        /// Track Table Name
        /// </summary>
        [NonSerialized()]
        public string TrackTable;

        /// <summary>
        /// Track MSG / Key
        /// </summary>
        [NonSerialized()]
        public string TrackMsg;


        /// <summary>
        /// Track Before
        /// </summary>
        [NonSerialized()]
        public string TrackBefore;


        /// <summary>
        /// Track After
        /// </summary>
        [NonSerialized()]
        public string TrackAfter;
        #endregion

        #region Execute Log
        /// <summary>
        /// 記錄Exp資訊
        /// </summary>
        public void LogExpInf()
        {
            //記錄狀態為Exception
            this.ErrFlag = false;

            //寫入Log   
            Information.LogExpInfo myLogExpInfo = new Information.LogExpInfo();
            myLogExpInfo.ClassName = this.GetType().FullName.ToString();
            myLogExpInfo.MethodName = this.ErrMethodName;
            myLogExpInfo.ErrMsg = this.ErrMsg;
            myLogExpInfo.Insert();
        }
        #endregion

        ////記錄Track Log Sample
        //base.TrackMode = "Select" / "ADD" / "MOD" / "DEL" 
        //base.TrackTable = "Cust";
        //base.TrackMsg = "OK" / "other description"
        //base.LogTrackInf();

        ///記錄Exception Log Sample        ///
        #region 呼叫Base.LogExpInf 進行Exception Log 記錄 (固定寫法)
        ////取得目前MethodName
        //System.Diagnostics.StackFrame stackFrame = new System.Diagnostics.StackFrame();
        //System.Reflection.MethodBase myMethodBase = stackFrame.GetMethod();

        //base.ErrFlag = false;
        //base.ErrMsg = ex.ToString();
        //base.ErrMethodName = myMethodBase.Name.ToString();
        //base.LogExpInf(); 
        #endregion

        #endregion
    }
}
