using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Web.Security.AntiXss;

namespace WebUICommon
{
    static partial class UI
    {
        public static void ClsValue(HiddenField iControl)
        {
            iControl.Value = "";
        }

        #region SetValue
        public static void SetValue(HiddenField iControl, string iValue)
        {
            iControl.Value = iValue.Trim();
        }

        public static void SetValue(HiddenField iControl, string[] iValue)
        {
            iControl.Value = "";
            foreach (string Temp in iValue)
            {
                iControl.Value += AntiXssEncoder.HtmlEncode("," + Temp.Trim(), true);
            }
            if (iControl.Value.Length > 0) iControl.Value = AntiXssEncoder.HtmlEncode(iControl.Value.Substring(1), true);
        }

        public static void SetValue(HiddenField iControl, bool iValue)
        {
            iControl.Value = (iValue ? "True" : "False");
        }

        public static void SetValue(HiddenField iControl, int iValue)
        {
            iControl.Value = iValue.ToString();
        }

        public static void SetValue(HiddenField iControl, long iValue)
        {
            iControl.Value = iValue.ToString();
        }

        public static void SetValue(HiddenField iControl, double iValue)
        {
            iControl.Value = iValue.ToString();
        }

        public static void SetValue(HiddenField iControl, decimal iValue)
        {
            iControl.Value = iValue.ToString();
        }

        public static void SetValue(HiddenField iControl, DateTime iValue)
        {
            SetDateTimeValue(iControl, iValue);
        }

        public static void SetDateValue(HiddenField iControl, DateTime iValue)
        {
            iControl.Value = iValue.ToString("yyyy/MM/dd");
        }

        public static void SetTimeValue(HiddenField iControl, DateTime iValue)
        {
            iControl.Value = iValue.ToString("HH:mm:ss");
        }

        public static void SetDateTimeValue(HiddenField iControl, DateTime iValue)
        {
            iControl.Value = iValue.ToString("yyyy/MM/dd HH:mm:ss");
        }
        #endregion

        #region GetValue
        public static string GetValue(HiddenField iControl)
        {
            return iControl.Value.Trim();
        }

        public static string[] GetValue2Array(HiddenField iControl)
        {
            if (iControl.Value.Trim() == "")
            {
                return new string[0];
            }
            else
            {
                string[] iValue = iControl.Value.Split(',');
                for (int i = 0; i < iValue.Length; i++)
                {
                    iValue[i] = "," + iValue[i].Trim();
                }
                return iValue;
            }
        }

        public static bool GetValue2bool(HiddenField iControl)
        {
            bool iValue;
            Boolean.TryParse(iControl.Value.Trim(), out iValue);
            return iValue;
        }

        public static int GetValue2int(HiddenField iControl)
        {
            decimal iValue;
            Decimal.TryParse(iControl.Value.Trim(), out iValue);
            return Convert.ToInt32(Math.Round(iValue, MidpointRounding.AwayFromZero));
        }

        public static long GetValue2long(HiddenField iControl)
        {
            decimal iValue;
            Decimal.TryParse(iControl.Value.Trim(), out iValue);
            return Convert.ToInt64(Math.Round(iValue, MidpointRounding.AwayFromZero));
        }

        public static double GetValue2double(HiddenField iControl)
        {
            double iValue;
            Double.TryParse(iControl.Value.Trim(), out iValue);
            return iValue;
        }

        public static decimal GetValue2decimal(HiddenField iControl)
        {
            decimal iValue;
            Decimal.TryParse(iControl.Value.Trim(), out iValue);
            return iValue;
        }

        public static DateTime GetValue2Date(HiddenField iControl)
        {
            DateTime iValue;
            switch (iControl.Value.Length)
            {
                case 6:
                    DateTime.TryParseExact(iControl.Value.Trim(), "yyMMdd", null, DateTimeStyles.None, out iValue);
                    break;
                case 8:
                    DateTime.TryParseExact(iControl.Value.Trim(), "yyyyMMdd", null, DateTimeStyles.None, out iValue);
                    break;
                case 10:
                    DateTime.TryParseExact(iControl.Value.Trim(), "yyyy/MM/dd", null, DateTimeStyles.None, out iValue);
                    break;
                default:
                    DateTime.TryParse(iControl.Value.Trim(), out iValue);
                    break;
            }
            return iValue;
        }

        public static DateTime GetValue2Time(HiddenField iControl)
        {
            DateTime iValue;
            switch (iControl.Value.Length)
            {
                case 6:
                    DateTime.TryParseExact(iControl.Value.Trim(), "HHmmss", null, DateTimeStyles.None, out iValue);
                    break;
                case 8:
                    DateTime.TryParseExact(iControl.Value.Trim(), "HH:mm:ss", null, DateTimeStyles.None, out iValue);
                    break;
                default:
                    DateTime.TryParse(iControl.Value.Trim(), out iValue);
                    break;
            }
            return iValue;
        }

        public static DateTime GetValue2DateTime(HiddenField iControl)
        {
            DateTime iValue;
            switch (iControl.Value.Length)
            {
                case 14:
                    DateTime.TryParseExact(iControl.Value.Trim(), "yyyyMMddHHmmss", null, DateTimeStyles.None, out iValue);
                    break;
                case 19:
                    DateTime.TryParseExact(iControl.Value.Trim(), "yyyy/MM/dd HH:mm:ss", null, DateTimeStyles.None, out iValue);
                    break;
                default:
                    DateTime.TryParse(iControl.Value.Trim(), out iValue);
                    break;
            }
            return iValue;
        }

        public static DateTime GetValue2YM(HiddenField iControl)
        {
            DateTime iValue;
            switch (iControl.Value.Length)
            {
                case 4:
                    DateTime.TryParseExact(iControl.Value.Trim() + "01", "yyMMdd", null, DateTimeStyles.None, out iValue);
                    break;
                case 6:
                    DateTime.TryParseExact(iControl.Value.Trim() + "01", "yyyyMMdd", null, DateTimeStyles.None, out iValue);
                    break;
                case 7:
                    DateTime.TryParseExact(iControl.Value.Trim() + "/01", "yyyy/MM/dd", null, DateTimeStyles.None, out iValue);
                    break;
                default:
                    iValue = DateTime.MinValue;
                    break;
            }
            return iValue;
        }
        #endregion
    }
}
