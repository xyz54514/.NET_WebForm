using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Information;

namespace Business
{
    public class StoreBiz
    {
        public string GetConnString()
        {
            StoreDB objStoreDB = new StoreDB();
            return objStoreDB.GetConnString();
        }

        public bool DeleteStore(int StoreID, SqlConnection iConn = null, SqlTransaction iTxn = null)
        {
            StoreDB objStoreDB = new StoreDB();
            return objStoreDB.DeleteStore(StoreID, iConn, iTxn);
        }

        public bool UpdateStore(StoreInfo info, SqlConnection iConn = null, SqlTransaction iTxn = null)
        {
            StoreDB objStoreDB = new StoreDB();
            return objStoreDB.UpdateStore(info, iConn, iTxn);
        }

        public int InsertStore(StoreInfo info, SqlConnection iConn = null, SqlTransaction iTxn = null)
        {
            StoreDB objStoreDB = new StoreDB();
            return objStoreDB.InsertStore(info, iConn, iTxn);
        }

        /// <summary>
        /// 查詢有關Store資料的數量
        /// </summary>
        /// <param name="Storename"></param>
        /// <param name="Address"></param>
        /// <returns></returns>
        public int InqStoreCount(string StoreName, string Address)
        {
            StoreDB objStoreDB = new StoreDB();

            //取得資料數量
            return objStoreDB.InqStoreCount(StoreName, Address);
        }

        /// <summary>
        /// 查詢有關的Store資料
        /// </summary>
        /// <param name="Storename">店名稱</param>
        /// <param name="Address">店址</param>
        /// <param name="tStartRow">起始行數</param>
        /// <param name="tEndRow">結束行數</param>
        /// <returns></returns>
        public DataTable InqStore(string StoreName, string Address, int tStartRow, int tEndRow)
        {
            StoreDB objStoreDB = new StoreDB();

            //查詢有關的Store資料
            return objStoreDB.InqStore(StoreName, Address, tStartRow, tEndRow);
        }
        
        
        /// <summary>
        /// 新增一筆資料到Customer
        /// </summary>
        /// <param name="info"></param>
        public void InsertStore(StoreInfo info)
        {
            StoreDB objCustomerDB = new StoreDB();
            objCustomerDB.InsertStore(info);
        }
        
        /// <summary>
        /// 透過key讀取指定Customer資料
        /// </summary>
        /// <param name="CID"></param>
        /// <returns></returns>
        public StoreInfo LoadStore(int SID)
        {
            StoreDB objStoreDB = new StoreDB();
            return objStoreDB.LoadStore(SID);
        }
        
        /// <summary>
        /// 更新Customer資料
        /// </summary>
        /// <param name="info"></param>
        public bool UpdateStore(StoreInfo info, string lastAddress)
        {
            StoreDB objStoreDB = new StoreDB();
            return objStoreDB.UpdateStore(info, lastAddress);
        }
        
        /// <summary>
        /// 刪除指定Customer資料
        /// </summary>
        /// <param name="CID"></param>
        /// <returns></returns>
        public bool DeleteStore(StoreInfo info)
        {
            StoreDB objStoreDB = new StoreDB();
            return objStoreDB.DeleteStore(info);
        }


        /// 0708 myExport
        /// 

        public int InqStoreCount2(string sName, string Address, DateTime? SDate, DateTime? EDate)
        {
            StoreDB objStoreDB = new StoreDB();
            return objStoreDB.InqStoreCount2(sName, Address, SDate, EDate);
        }

        public DataTable InqStore2(string sName, string Address, DateTime? SDate, DateTime? EDate, int tStartRow, int tEndRow)
        {
            StoreDB objStoreDB = new StoreDB();
            return objStoreDB.InqStore2(sName, Address, SDate, EDate, tStartRow, tEndRow);
        }

        /// 獲取匯出的Employee資料
        public DataTable ExpStore(string sName, string Address, DateTime? SDate, DateTime? EDate)
        {
            StoreDB objStoreDB = new StoreDB();

            //查詢有關的Employee資料
            return objStoreDB.ExpStore(sName, Address, SDate, EDate);
        }

        public bool InsertBySqlBulkCopy(DataTable dt)
        {
            StoreDB objStoreDB = new StoreDB();
            return objStoreDB.InsertBySqlBulkCopy(dt);
        }
    }
}
