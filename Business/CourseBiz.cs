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
    public class CourseBiz
    {
        /// <summary>
        /// 取得ConnString
        /// </summary>
        /// <returns></returns>
        public string GetConnString()
        {
            CourseDB objCourseDB = new CourseDB();
            return objCourseDB.GetConnString();
        }

        /// <summary>
        /// 有關Course資料的數量
        /// </summary>
        /// <param name="CourseName"></param>
        /// <param name="SDate"></param>
        /// <param name="EDate"></param>
        /// <returns></returns>
        public int InqCourseCount(string CourseName, DateTime? SDate, DateTime? EDate)
        {
            CourseDB objCourseDB = new CourseDB();

            //取得資料數量
            return objCourseDB.InqCourseCount(CourseName, SDate, EDate);
        }

        /// <summary>
        /// 查詢有關Course的資料
        /// </summary>
        /// <param name="CourseName"></param>
        /// <param name="SDate"></param>
        /// <param name="EDate"></param>
        /// <param name="tStartRow"></param>
        /// <param name="tEndRow"></param>
        /// <returns></returns>
        public DataTable InqCourse(string CourseName, DateTime? SDate, DateTime? EDate, int tStartRow, int tEndRow)
        {
            CourseDB objCourseDB = new CourseDB();

            //查詢有關的Employee資料
            return objCourseDB.InqCourse(CourseName, SDate, EDate, tStartRow, tEndRow);
        }

        /// <summary>
        /// 新增一筆資料到Course
        /// </summary>
        /// <param name="info"></param>
        public int InsertCourse(CourseInfo info, SqlConnection iConn = null, SqlTransaction iTxn = null)
        {
            CourseDB objCourseDB = new CourseDB();
            return objCourseDB.InsertCourse(info, iConn, iTxn);
        }

        /// <summary>
        /// 透過key讀取指定Customer資料
        /// </summary>
        /// <param name="CID"></param>
        /// <returns></returns>
        public CourseInfo LoadCourse(int Course_ID)
        {
            CourseDB objCourseDB = new CourseDB();
            return objCourseDB.LoadCourse(Course_ID);
        }

        /// <summary>
        /// 刪除指定Course資料
        /// </summary>
        /// <param name="Course_ID"></param>
        /// <returns></returns>
        public bool DeleteCourse(int Course_ID, SqlConnection iConn = null, SqlTransaction iTxn = null)
        {
            CourseDB objCourseDB = new CourseDB();
            return objCourseDB.DeleteCourse(Course_ID, iConn, iTxn);
        }

        /// <summary>
        /// 更新Course資料
        /// </summary>
        /// <param name="info"></param>
        public bool UpdateCourse(CourseInfo info, SqlConnection iConn = null, SqlTransaction iTxn = null)
        {
            CourseDB objCourseDB = new CourseDB();
            return objCourseDB.UpdateCourse(info, iConn, iTxn);
        }

        
    }
}
