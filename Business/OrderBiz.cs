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
    public class OrderBiz
    {
        /// <summary>
        /// 查詢有關Product資料的數量
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int InqOrderCountMaker(string name, string time)
        {
            OrderDB objOrderDB = new OrderDB();

            //取得資料數量
            return objOrderDB.InqOrderCountMaker(name, time);
        }
        /// <summary>
        /// 查詢有關Product資料的數量
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int InqOrderCountChecker(string name, string time)
        {
            OrderDB objOrderDB = new OrderDB();

            //取得資料數量
            return objOrderDB.InqOrderCountChecker(name, time);
        }
        /// <summary>
        /// 查詢有關的Product資料 Maker
        /// </summary>
        /// <param name="name">名稱</param>
        /// <param name="tStartRow">起始行數</param>
        /// <param name="tEndRow">結束行數</param>
        /// <returns></returns>
        public DataTable InqOrderMaker(string name, string time, int tStartRow, int tEndRow)
        {
            OrderDB objOrderDB = new OrderDB();

            return objOrderDB.InqOrderMaker(name, time, tStartRow, tEndRow);
        }

        /// <summary>
        /// 查詢有關的Product資料 Checker
        /// </summary>
        /// <param name="name">名稱</param>
        /// <param name="tStartRow">起始行數</param>
        /// <param name="tEndRow">結束行數</param>
        /// <returns></returns>
        public DataTable InqOrderChecker(string name, string time, int tStartRow, int tEndRow)
        {
            OrderDB objOrderDB = new OrderDB();

            return objOrderDB.InqOrderChecker(name, time, tStartRow, tEndRow);
        }

        /// <summary>
        /// 新增一筆資料到Product
        /// </summary>
        /// <param name="info"></param>
        public void InsertOrder(OrderInfo info)
        {
            OrderDB objOrderDB = new OrderDB();
            objOrderDB.InsertOrder(info);
        }

        /// <summary>
        /// 透過key讀取指定Product資料
        /// </summary>
        /// <param name="PID"></param>
        /// <returns></returns>
        public OrderInfo LoadOrder(int OrderID)
        {
            OrderDB objOrderDB = new OrderDB();
            return objOrderDB.LoadOrder(OrderID);
        }

        /// <summary>
        /// 更新Product資料
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool UpdateOrder(OrderInfo info)
        {
            OrderDB objOrderDB = new OrderDB();
            return objOrderDB.UpdateOrder(info);
        }
        /// <summary>
        /// 刪除指定Order資料
        /// </summary>
        /// <param name="OID"></param>
        /// <returns></returns>
        public bool DeleteOrder(int OrderID)
        {
            OrderDB objOrderDB = new OrderDB();
            return objOrderDB.DeleteOrder(OrderID);
        }
    }
}
