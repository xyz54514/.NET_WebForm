using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;

namespace Business
{
    public class EmployeeBiz
    {
        
        /// <summary>
        /// 查詢有關Employee資料的數量
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Dept"></param>
        /// <param name="SDate"></param>
        /// <param name="EDate"></param>
        /// <returns></returns>
        public int InqEmployeeCount(string Name, string Dept, DateTime? SDate, DateTime? EDate)
        {
            EmployeeDB objEmployeeDB = new EmployeeDB();

            //取得資料數量
            return objEmployeeDB.InqEmployeeCount(Name, Dept, SDate, EDate);
        }

        /// <summary>
        /// 查詢有關Employee的資料
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Dept"></param>
        /// <param name="SDate"></param>
        /// <param name="EDate"></param>
        /// <param name="tStartRow"></param>
        /// <param name="tEndRow"></param>
        /// <returns></returns>
        public DataTable InqEmployee(string Name, string Dept, DateTime? SDate, DateTime? EDate, int tStartRow, int tEndRow)
        {
            EmployeeDB objEmployeeDB = new EmployeeDB();

            //查詢有關的Employee資料
            return objEmployeeDB.InqEmployee(Name, Dept, SDate, EDate, tStartRow, tEndRow);
        }

        /// <summary>
        /// 獲取匯出的Employee資料
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Dept"></param>
        /// <param name="SDate"></param>
        /// <param name="EDate"></param>
        /// <returns></returns>
        public DataTable ExpEmployee(string Name, string Dept, DateTime? SDate, DateTime? EDate)
        {
            EmployeeDB objEmployeeDB = new EmployeeDB();

            //查詢有關的Employee資料
            return objEmployeeDB.ExpEmployee(Name, Dept, SDate, EDate);
        }

        public bool InsertBySqlBulkCopy(DataTable dt)
        {
            EmployeeDB objEmployeeDB = new EmployeeDB();

            return objEmployeeDB.InsertBySqlBulkCopy(dt);
        }
    }
}
