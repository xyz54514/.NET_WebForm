using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Information
{
    public class ErrInfo
    {
        public ErrInfo()
        {
            this.ErrFlag = true;
            this.ErrMethodName = "";
            this.ErrMsg = "";
        }

        //錯誤檢查
        public bool ErrFlag;

        //錯誤Method
        public string ErrMethodName;

        //錯誤訊息
        public string ErrMsg;
    }
}
