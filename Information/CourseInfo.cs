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
    public class CourseInfo
    {

        /// <summary>
        /// Constructors
        /// </summary>
        public CourseInfo()
        {
            this.Init();
        }

        /// <summary>
        /// Constructors
        /// </summary>
        public CourseInfo(DataRow dr)
        {
            Course_ID = Convert.ToInt32(dr["Course_ID"]);

            Course_Name = Convert.ToString(dr["Course_Name"]);

            if (dr["Instructor"] == DBNull.Value)
                Instructor = null;
            else
                Instructor = Convert.ToString(dr["Instructor"]);

            StartDate = Convert.ToDateTime(dr["StartDate"]);

            EndDate = Convert.ToDateTime(dr["EndDate"]);

        }


        #region Init
        private void Init()
        {
            this._Course_ID = 0;                          //
            this._Course_Name = "";                       //
            this._Instructor = null;                      //
            this._StartDate = new DateTime();             //
            this._EndDate = new DateTime();               //
        }
        #endregion


        #region Private Properties
        private Int32 _Course_ID;
        private String _Course_Name;
        private String _Instructor;
        private DateTime _StartDate;
        private DateTime _EndDate;
        #endregion


        #region Public Properties

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
        public String Course_Name
        {
            get { return _Course_Name; }
            set { _Course_Name = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Instructor
        {
            get { return _Instructor; }
            set { _Instructor = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime StartDate
        {
            get { return _StartDate; }
            set { _StartDate = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime EndDate
        {
            get { return _EndDate; }
            set { _EndDate = value; }
        }
        #endregion

    }
}


