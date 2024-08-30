using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace WebUICommon
{
    static partial class UI
    {
        public static void ClsValue(RadioButton iControl)
        {
            iControl.Checked = false;
        }

        public static void SetValue(RadioButton iControl, bool iValue)
        {
            iControl.Checked = iValue;
        }

        public static void SetValue(RadioButton iControl, string iValue)
        {
            switch (iValue.ToUpper().Trim())
            {
                case "1":
                case "Y":
                case "YES":
                case "T":
                case "TRUE":
                    iControl.Checked = true;
                    break;

                default:
                    iControl.Checked = false;
                    break;
            }
        }

        public static void SetValue(RadioButton iControl, string iValue, string strTrue)
        {
            iControl.Checked = (iValue.ToUpper().Trim() == strTrue.ToUpper().Trim());
        }

        public static string GetValue(RadioButton iControl, string strTrue, string strFalse)
        {
            return (iControl.Checked ? strTrue : strFalse);
        }

        public static string GetValue(RadioButton iControl)
        {
            return (iControl.Checked ? "1" : "0");
        }

        public static bool GetValue2bool(RadioButton iControl)
        {
            return iControl.Checked;
        }
    }
}
