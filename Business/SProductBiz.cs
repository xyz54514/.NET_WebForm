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
    public class SProductBiz
    {
        public string GetConnString()
        {
            SProductDB objSProductDB = new SProductDB();
            return objSProductDB.GetConnString();
        }

        public DataTable LoadProduct(int iStoreID)
        {
            SProductDB objSProductDB = new SProductDB();
            return objSProductDB.LoadProduct(iStoreID);
        }

        public void InsertProduct(SProductInfo info, SqlConnection iConn = null, SqlTransaction iTxn = null)
        {
            SProductDB objSProductDB = new SProductDB();
            objSProductDB.InsertProduct(info, iConn, iTxn);
        }

        public bool DeleteProduct(int StoreID, SqlConnection iConn = null, SqlTransaction iTxn = null)
        {
            SProductDB objSProductDB = new SProductDB();
            return objSProductDB.DeleteProduct(StoreID, iConn, iTxn);
        }

        public bool DeleteProduct(int StoreID, string sProductID, SqlConnection iConn = null, SqlTransaction iTxn = null)
        {
            SProductDB objSProductDB = new SProductDB();
            return objSProductDB.DeleteProduct(StoreID, sProductID, iConn, iTxn);
        }

        public bool UpdateProduct(SProductInfo info, SqlConnection iConn = null, SqlTransaction iTxn = null)
        {
            SProductDB objSProductDB = new SProductDB();
            return objSProductDB.UpdateProduct(info, iConn, iTxn);
        }
    }
}
