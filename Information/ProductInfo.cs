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
    public class ProductInfo
    {

        /// <summary>
        /// Constructors
        /// </summary>
        public ProductInfo()
        {
            this.Init();
        }

        /// <summary>
        /// Constructors
        /// </summary>
        public ProductInfo(DataRow dr)
        {
            PID = Convert.ToInt32(dr["PID"]);

            if (dr["ProdName"] == DBNull.Value)
                ProdName = null;
            else
                ProdName = Convert.ToString(dr["ProdName"]);

            if (dr["Price"] == DBNull.Value)
                Price = null;
            else
                Price = Convert.ToInt32(dr["Price"]);

            if (dr["Status"] == DBNull.Value)
                Status = null;
            else
                Status = Convert.ToString(dr["Status"]);

            if (dr["CreateDate"] == DBNull.Value)
                CreateDate = null;
            else
                CreateDate = Convert.ToDateTime(dr["CreateDate"]);

            if (dr["Creator"] == DBNull.Value)
                Creator = null;
            else
                Creator = Convert.ToString(dr["Creator"]);

            if (dr["Maker"] == DBNull.Value)
                Maker = null;
            else
                Maker = Convert.ToString(dr["Maker"]);

            if (dr["Checker"] == DBNull.Value)
                Checker = null;
            else
                Checker = Convert.ToString(dr["Checker"]);

        }


        #region Init
        private void Init()
        {
            this._PID = 0;                                //
            this._ProdName = null;                        //
            this._Price = null;                           //
            this._Status = null;                          //
            this._CreateDate = null;                      //
            this._Creator = null;                         //
            this._Maker = null;                           //
            this._Checker = null;                         //
        }
        #endregion


        #region Private Properties
        private Int32 _PID;
        private String _ProdName;
        private Int32? _Price;
        private String _Status;
        private DateTime? _CreateDate;
        private String _Creator;
        private String _Maker;
        private String _Checker;
        #endregion


        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public Int32 PID
        {
            get { return _PID; }
            set { _PID = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String ProdName
        {
            get { return _ProdName; }
            set { _ProdName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32? Price
        {
            get { return _Price; }
            set { _Price = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateDate
        {
            get { return _CreateDate; }
            set { _CreateDate = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Creator
        {
            get { return _Creator; }
            set { _Creator = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Maker
        {
            get { return _Maker; }
            set { _Maker = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Checker
        {
            get { return _Checker; }
            set { _Checker = value; }
        }
        #endregion

    }
}


