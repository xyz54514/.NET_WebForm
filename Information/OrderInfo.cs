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
    public class OrderInfo
    {
        /// <summary>
        /// Constructors
        /// </summary>
        public OrderInfo()
        {
            this.Init();
        }

        /// <summary>
        /// Constructors
        /// </summary>
        public OrderInfo(DataRow dr)
        {
            OrderID = Convert.ToInt32(dr["OrderID"]);

            if (dr["ProductName"] == DBNull.Value)
                ProductName = null;
            else
                ProductName = Convert.ToString(dr["ProductName"]);

            if (dr["Quantity"] == DBNull.Value)
                Quantity = null;
            else
                Quantity = Convert.ToInt32(dr["Quantity"]);

            if (dr["UnitPrice"] == DBNull.Value)
                UnitPrice = null;
            else
                UnitPrice = Convert.ToInt32(dr["UnitPrice"]);

            if (dr["TotalPrice"] == DBNull.Value)
                TotalPrice = null;
            else
                TotalPrice = Convert.ToInt32(dr["TotalPrice"]);

            if (dr["Status"] == DBNull.Value)
                Status = null;
            else
                Status = Convert.ToString(dr["Status"]);

            if (dr["CreateDate"] == DBNull.Value)
                CreateDate = null;
            else
                CreateDate = Convert.ToDateTime(dr["CreateDate"]);

            if (dr["OrderDate"] == DBNull.Value)
                OrderDate = null;
            else
                OrderDate = Convert.ToDateTime(dr["OrderDate"]);

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
            this._OrderID = 0;                                //
            this._ProductName = null;                        //
            this._Quantity = null;                           //
            this._UnitPrice = null;                          //
            this._TotalPrice = null;
            this._OrderDate = null;
            this._Status = null;
            this._CreateDate = null;                      //
            this._Creator = null;                         //
            this._Maker = null;                           //
            this._Checker = null;                         //
        }
        #endregion


        #region Private Properties
        private Int32 _OrderID;
        private String _ProductName;
        private Int32? _Quantity;
        private Int32? _UnitPrice;
        private Int32? _TotalPrice;
        private DateTime? _OrderDate;
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
        public Int32 OrderID
        {
            get { return _OrderID; }
            set { _OrderID = value; }
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
        public Int32? Quantity
        {
            get { return _Quantity; }
            set { _Quantity = value; }
        }

        public Int32? UnitPrice
        {
            get { return _UnitPrice; }
            set { _UnitPrice = value; }
        }
        public Int32? TotalPrice
        {
            get { return _TotalPrice; }
            set { _TotalPrice = value; }
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
        public DateTime? OrderDate
        {
            get { return _OrderDate; }
            set { _OrderDate = value; }
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
