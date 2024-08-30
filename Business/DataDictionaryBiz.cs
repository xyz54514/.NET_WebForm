using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;

namespace Business
{
    public class DataDictionaryBiz
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="DataType"></param>
        /// <returns></returns>
        public DataTable InqDataDictionary(string DataType)
        {
            DataDictionaryDB objDataDictionaryDB = new DataDictionaryDB();
            return objDataDictionaryDB.InqDataDictionary(DataType);
        }
    }
}
