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
    public class StudentBiz
    {
        /// <summary>
        /// 取得ConnString
        /// </summary>
        /// <returns></returns>
        public string GetConnString()
        {
            StudentDB objStudentDB = new StudentDB();
            return objStudentDB.GetConnString();
        }

        /// <summary>
        /// 透過key讀取Student資料
        /// </summary>
        /// <param name="Course_ID"></param>
        /// <returns></returns>
        public DataTable LoadStudent(int iCourse_ID)
        {
            StudentDB objStudentDB = new StudentDB();
            return objStudentDB.LoadStudent(iCourse_ID);
        }

        /// <summary>
        /// 新增一筆資料到Student
        /// </summary>
        /// <param name="info"></param>
        /// <param name="iConn"></param>
        /// <param name="iTxn"></param>
        public void InsertStudent(StudentInfo info, SqlConnection iConn = null, SqlTransaction iTxn = null)
        {
            StudentDB objStudentDB = new StudentDB();
            objStudentDB.InsertStudent(info, iConn, iTxn);
        }

        /// <summary>
        /// 根據指定的Course_ID刪除Student資料
        /// </summary>
        /// <param name="Course_ID"></param>
        /// <param name="iConn"></param>
        /// <param name="iTxn"></param>
        /// <returns></returns>
        public bool DeleteStudent(int Course_ID, SqlConnection iConn = null, SqlTransaction iTxn = null)
        {
            StudentDB objStudentDB = new StudentDB();
            return objStudentDB.DeleteStudent(Course_ID, iConn, iTxn);
        }

        /// <summary>
        /// 根據指定的Course_ID & sStudent_ID刪除特定Student資料
        /// </summary>
        /// <param name="Course_ID"></param>
        /// <param name="sStudent_ID"></param>
        /// <param name="iConn"></param>
        /// <param name="iTxn"></param>
        /// <returns></returns>
        public bool DeleteStudent(int Course_ID, string sStudent_ID, SqlConnection iConn = null, SqlTransaction iTxn = null)
        {
            StudentDB objStudentDB = new StudentDB();
            return objStudentDB.DeleteStudent(Course_ID, sStudent_ID, iConn, iTxn);
        }

        /// <summary>
        /// 更新Student資料
        /// </summary>
        /// <param name="info"></param>
        /// <param name="iConn"></param>
        /// <param name="iTxn"></param>
        /// <returns></returns>
        public bool UpdateStudent(StudentInfo info, SqlConnection iConn = null, SqlTransaction iTxn = null)
        {
            StudentDB objStudentDB = new StudentDB();
            return objStudentDB.UpdateStudent(info, iConn, iTxn);
        }
    }
}
