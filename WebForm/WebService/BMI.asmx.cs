using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace WebForm.WebService
{
    /// <summary>
    ///BMI 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    // [System.Web.Script.Services.ScriptService]
    public class BMI : System.Web.Services.WebService
    {

        [WebMethod]
        public string BMICal(double Height, double Weigth)
        {
            //身高
            Height = Height / 100.0;

            //BMI
            double bmi = Weigth / (Height * Height);

            //根據BMI值返回不同的結果
            string result;
            if (bmi < 18.5)
            {
                result = "過輕";
            }
            else if (bmi >= 18.5 && bmi < 24)
            {
                result = "健康";
            }
            else if (24 <= bmi && bmi < 27)
            {
                result = "過重";
            }
            else if (27 <= bmi && bmi < 30)
            {
                result = "輕度肥胖";
            }
            else if (30 <= bmi && bmi < 35)
            {
                result = "中度肥胖";
            }
            else
            {
                result = "重度肥胖";
            }

            // 返回结果
            return $"{bmi:F2},{result}";
        }
    }
}
