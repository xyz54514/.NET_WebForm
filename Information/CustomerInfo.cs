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
    public class CustomerInfo
    {

        /// <summary>
        /// Constructors
        /// </summary>
        public CustomerInfo()
        {
            this.Init();
        }

        /// <summary>
        /// Constructors
        /// </summary>
        public CustomerInfo(DataRow dr)
        {
            CID = Convert.ToInt32(dr["CID"]);

            if (dr["Name"] == DBNull.Value)
                Name = null;
            else
                Name = Convert.ToString(dr["Name"]);

            if (dr["City"] == DBNull.Value)
                City = null;
            else
                City = Convert.ToString(dr["City"]);

            if (dr["Phone"] == DBNull.Value)
                Phone = null;
            else
                Phone = Convert.ToString(dr["Phone"]);

            if (dr["Type"] == DBNull.Value)
                Type = null;
            else
                Type = Convert.ToString(dr["Type"]);

        }


        #region Init
        private void Init()
        {
            this._CID = 0;                                //
            this._Name = null;                            //
            this._City = null;                            //
            this._Phone = null;                           //
            this._Type = null;                            //
        }
        #endregion


        #region Private Properties
        private Int32 _CID;
        private String _Name;
        private String _City;
        private String _Phone;
        private String _Type;
        #endregion


        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public Int32 CID
        {
            get { return _CID; }
            set { _CID = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String City
        {
            get { return _City; }
            set { _City = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Phone
        {
            get { return _Phone; }
            set { _Phone = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Type
        {
            get { return _Type; }
            set { _Type = value; }
        }
        #endregion

    }
}


