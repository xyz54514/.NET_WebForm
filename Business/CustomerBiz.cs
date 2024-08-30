using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Information;

namespace Business
{
    public class CustomerBiz
    {
        /// <summary>
        /// 查詢有關Customer資料的數量
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public int InqCustomerCount(string name, string type)
        {
            CustomerDB objCustomerDB = new CustomerDB();

            //取得資料數量
            return objCustomerDB.InqCustomerCount(name, type);
        }

        /// <summary>
        /// 查詢有關的Customer資料
        /// </summary>
        /// <param name="name">名稱</param>
        /// <param name="type">會員等級</param>
        /// <param name="tStartRow">起始行數</param>
        /// <param name="tEndRow">結束行數</param>
        /// <returns></returns>
        public DataTable InqCustomer(string name, string type, int tStartRow, int tEndRow)
        {
            CustomerDB objCustomerDB = new CustomerDB();

            //查詢有關的Customer資料
            return objCustomerDB.InqCustomer(name, type, tStartRow, tEndRow);
        }

        /// <summary>
        /// 新增一筆資料到Customer
        /// </summary>
        /// <param name="info"></param>
        public void InsertCustomer(CustomerInfo info)
        {
            CustomerDB objCustomerDB = new CustomerDB();
            objCustomerDB.InsertCustomer(info);
        }

        /// <summary>
        /// 透過key讀取指定Customer資料
        /// </summary>
        /// <param name="CID"></param>
        /// <returns></returns>
        public CustomerInfo LoadCustomer(int CID)
        {
            CustomerDB objCustomerDB = new CustomerDB();
            return objCustomerDB.LoadCustomer(CID);
        }

        /// <summary>
        /// 更新Customer資料
        /// </summary>
        /// <param name="info"></param>
        public bool UpdateCustomer(CustomerInfo info)
        {
            CustomerDB objCustomerDB = new CustomerDB();
            return objCustomerDB.UpdateCustomer(info);
        }

        /// <summary>
        /// 刪除指定Customer資料
        /// </summary>
        /// <param name="CID"></param>
        /// <returns></returns>
        public bool DeleteCustomer(int CID)
        {
            CustomerDB objCustomerDB = new CustomerDB();
            return objCustomerDB.DeleteCustomer(CID);
        }
    }
}
