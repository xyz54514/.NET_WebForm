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
    public class ProductBiz
    {
        /// <summary>
        /// 查詢有關Product資料的數量
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int InqProductCountMaker(string name)
        {
            ProductDB objProductDB = new ProductDB();

            //取得資料數量
            return objProductDB.InqProductCountMaker(name);
        }
        /// <summary>
        /// 查詢有關Product資料的數量
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int InqProductCountChecker(string name)
        {
            ProductDB objProductDB = new ProductDB();

            //取得資料數量
            return objProductDB.InqProductCountChecker(name);
        }
        /// <summary>
        /// 查詢有關的Product資料 Maker
        /// </summary>
        /// <param name="name">名稱</param>
        /// <param name="tStartRow">起始行數</param>
        /// <param name="tEndRow">結束行數</param>
        /// <returns></returns>
        public DataTable InqProductMaker(string name, int tStartRow, int tEndRow)
        {
            ProductDB objProductDB = new ProductDB();

            return objProductDB.InqProductMaker(name, tStartRow, tEndRow);
        }

        /// <summary>
        /// 查詢有關的Product資料 Checker
        /// </summary>
        /// <param name="name">名稱</param>
        /// <param name="tStartRow">起始行數</param>
        /// <param name="tEndRow">結束行數</param>
        /// <returns></returns>
        public DataTable InqProductChecker(string name, int tStartRow, int tEndRow)
        {
            ProductDB objProductDB = new ProductDB();

            return objProductDB.InqProductChecker(name, tStartRow, tEndRow);
        }

        /// <summary>
        /// 新增一筆資料到Product
        /// </summary>
        /// <param name="info"></param>
        public void InsertProduct(ProductInfo info)
        {
            ProductDB objProductDB = new ProductDB();
            objProductDB.InsertProduct(info);
        }

        /// <summary>
        /// 透過key讀取指定Product資料
        /// </summary>
        /// <param name="PID"></param>
        /// <returns></returns>
        public ProductInfo LoadProduct(int PID)
        {
            ProductDB objProductDB = new ProductDB();
            return objProductDB.LoadProduct(PID);
        }

        /// <summary>
        /// 更新Product資料
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool UpdateProduct(ProductInfo info)
        {
            ProductDB objProductDB = new ProductDB();
            return objProductDB.UpdateProduct(info);
        }
        /// <summary>
        /// 刪除指定Product資料
        /// </summary>
        /// <param name="PID"></param>
        /// <returns></returns>
        public bool DeleteProduct(int PID)
        {
            ProductDB objProductDB = new ProductDB();
            return objProductDB.DeleteProduct(PID);
        }
    }
}
