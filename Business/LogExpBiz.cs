using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Information;

namespace Business
{
    public class LogExpBiz
    {
        /// <summary>
        /// 寫入LogExp
        /// </summary>
        /// <param name="className">類別名稱</param>
        /// <param name="methodName">函式名稱</param>
        /// <param name="ex">Exception</param>
        public void InsertLogExp(string className, string methodName, Exception ex)
        {
            //建構Info供記錄錯誤訊息至資料庫
            LogExpInfo myLogExpInfo = new LogExpInfo();

            //儲存發生錯誤的class
            myLogExpInfo.ClassName = className;

            //儲存發生錯誤的Method
            myLogExpInfo.MethodName = methodName;

            //儲存錯誤訊息
            myLogExpInfo.ErrMsg = ex.ToString();

            //寫入至資料庫
            myLogExpInfo.Insert();
        }
    }
}
