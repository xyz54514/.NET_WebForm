using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace WebUICommon
{
    static partial class UI
    {
        public static void ClsValue(CheckBox iControl)
        {
            iControl.Checked = false;
        }

        #region SetValue
        public static void SetValue(CheckBox iControl, bool iValue)
        {
            iControl.Checked = iValue;
        }

        public static void SetValue(CheckBox iControl, string iValue)
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

        public static void SetValue(CheckBox iControl, string iValue, string strTrue)
        {
            iControl.Checked = (iValue.ToUpper().Trim() == strTrue.ToUpper().Trim());
        }
        #endregion

        #region GetValue
        public static string GetValue(CheckBox iControl, string strTrue, string strFalse)
        {
            return (iControl.Checked ? strTrue : strFalse);
        }

        public static string GetValue(CheckBox iControl)
        {
            return (iControl.Checked ? "1" : "0");
        }

        public static bool GetValue2bool(CheckBox iControl)
        {
            return iControl.Checked;
        }
        #endregion
    }
}
