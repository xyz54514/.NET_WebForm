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
    public class StoreInfo
    {

        /// <summary>
        /// Constructors
        /// </summary>
        public StoreInfo()
        {
            this.Init();
        }

        /// <summary>
        /// Constructors
        /// </summary>
        public StoreInfo(DataRow dr)
        {
            StoreID = Convert.ToInt32(dr["StoreID"]);

            if (dr["StoreName"] == DBNull.Value)
                StoreName = null;
            else
                StoreName = Convert.ToString(dr["StoreName"]);

            if (dr["Address"] == DBNull.Value)
                Address = null;
            else
                Address = Convert.ToString(dr["Address"]);

            if (dr["Telephone"] == DBNull.Value)
                Telephone = null;
            else
                Telephone = Convert.ToString(dr["Telephone"]);

            if (dr["Mobile"] == DBNull.Value)
                Mobile = null;
            else
                Mobile = Convert.ToString(dr["Mobile"]);

            if (dr["Remark"] == DBNull.Value)
                Remark = null;
            else
                Remark = Convert.ToString(dr["Remark"]);

            if (dr["CreateDate"] == DBNull.Value)
                CreateDate = null;
            else
                CreateDate = Convert.ToString(dr["CreateDate"]);

        }


        #region Init
        private void Init()
        {
            this._StoreID = 0;                                //
            this._StoreName = null;                            //
            this._Address = null;                            //
            this._Mobile = null;                           //
            this._Remark = null;                            //
            this._CreateDate = null;
        }
        #endregion


        #region Private Properties
        private Int32 _StoreID;
        private String _StoreName;
        private String _Address;
        private String _Telephone;
        private String _Mobile;
        private String _Remark;
        private String _CreateDate;
        #endregion


        #region Public Properties

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
        public String StoreName
        {
            get { return _StoreName; }
            set { _StoreName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Address
        {
            get { return _Address; }
            set { _Address = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Telephone
        {
            get { return _Telephone; }
            set { _Telephone = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Mobile
        {
            get { return _Mobile; }
            set { _Mobile = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Remark
        {
            get { return _Remark; }
            set { _Remark = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String CreateDate
        {
            get { return _CreateDate; }
            set { _CreateDate = value; }
        }


        #endregion

    }
}
