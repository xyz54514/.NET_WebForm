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
    public class StudentInfo
    {

        /// <summary>
        /// Constructors
        /// </summary>
        public StudentInfo()
        {
            this.Init();
        }

        /// <summary>
        /// Constructors
        /// </summary>
        public StudentInfo(DataRow dr)
        {
            SN = Convert.ToInt32(dr["SN"]);

            Course_ID = Convert.ToInt32(dr["Course_ID"]);

            Student_ID = Convert.ToString(dr["Student_ID"]);

            Student_Name = Convert.ToString(dr["Student_Name"]);

            if (dr["Phone"] == DBNull.Value)
                Phone = null;
            else
                Phone = Convert.ToString(dr["Phone"]);

            if (dr["Email"] == DBNull.Value)
                Email = null;
            else
                Email = Convert.ToString(dr["Email"]);

        }


        #region Init
        private void Init()
        {
            this._SN = 0;                                 //
            this._Course_ID = 0;                          //
            this._Student_ID = "";                        //
            this._Student_Name = "";                      //
            this._Phone = null;                           //
            this._Email = null;                           //
        }
        #endregion


        #region Private Properties
        private Int32 _SN;
        private Int32 _Course_ID;
        private String _Student_ID;
        private String _Student_Name;
        private String _Phone;
        private String _Email;
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
        public Int32 Course_ID
        {
            get { return _Course_ID; }
            set { _Course_ID = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Student_ID
        {
            get { return _Student_ID; }
            set { _Student_ID = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Student_Name
        {
            get { return _Student_Name; }
            set { _Student_Name = value; }
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
        public String Email
        {
            get { return _Email; }
            set { _Email = value; }
        }
        #endregion

    }
}


