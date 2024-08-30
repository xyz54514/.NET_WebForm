using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Xml;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Configuration;

namespace Information
{
    public class SProductInfo
    {

        /// <summary>
        /// Constructors
        /// </summary>
        public SProductInfo()
        {
            this.Init();
        }

        /// <summary>
        /// Constructors
        /// </summary>
        public SProductInfo(DataRow dr)
        {
            SN = Convert.ToInt32(dr["SN"]);

            StoreID = Convert.ToInt32(dr["StoreID"]);

            ProductID = Convert.ToString(dr["ProductID"]);

            ProductName = Convert.ToString(dr["ProductName"]);

            Price = Convert.ToString(dr["Price"]);

            //if (dr["Price"] == DBNull.Value)
            //    Price = null;
            //else
            //    Price = Convert.ToString(dr["Price"]);

            //允許空值??
            if (dr["Remark"] == DBNull.Value)
                Remark = null;
            else
                Remark = Convert.ToString(dr["Remark"]);

        }


        #region Init
        private void Init()
        {
            this._SN = 0;                                 //
            this._StoreID = 0;                          //
            this._ProductID = "";                        //
            this._ProductName = "";                      //
            this._Price = "";                           //
            this._Remark = null;                           //
        }
        #endregion


        #region Private Properties
        private Int32 _SN;
        private Int32 _StoreID;
        private String _ProductID;
        private String _ProductName;
        private String _Price;
        private String _Remark;
        #endregion


        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public Int32 SN
        {
            get { return _SN; }
            set { _SN = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 StoreID
        {
            get { return _StoreID; }
            set { _StoreID = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String ProductID
        {
            get { return _ProductID; }
            set { _ProductID = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String ProductName
        {
            get { return _ProductName; }
            set { _ProductName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Price
        {
            get { return _Price; }
            set { _Price = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Remark
        {
            get { return _Remark; }
            set { _Remark = value; }
        }
        #endregion

    }
}


