using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Information
{
    public class CustomerAPIInfo
    {
        //向API請求資料時攜帶的參數
        public class Customer_SendRequest
        {
            public string CID { get; set; }
        }

        //用於儲存回傳資料的類型
        public class Customer_SendResponse
        {
            public string Name { get; set; }

            public string City { get; set; }

            public string Phone { get; set; }

            public string Type { get; set; }

            public bool Result { get; set; }

            public string ErrMsg { get; set; }
        }
    }
}
