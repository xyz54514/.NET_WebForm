using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Globalization;

namespace WebUICommon
{
    static partial class UI
    {
        public static void ClsValue(Label iControl)
        {
            iControl.Text = "";
        }

        #region SetValue
        public static void SetValue(Label iControl, string iValue)
        {
            if (iValue == null)
                iControl.Text = "";
            else
                iControl.Text = System.Web.HttpUtility.HtmlEncode(iValue.Trim());
        }

        public static void SetValue(Label iControl, string[] iValue)
        {
            iControl.Text = "";
            foreach (string Temp in iValue)
            {
                iControl.Text += "," + Temp.Trim();
            }
            if (iControl.Text.Length > 0) iControl.Text = iControl.Text.Substring(1);
            iControl.Text = System.Web.HttpUtility.HtmlEncode(iControl.Text);
        }

        public static void SetValue(Label iControl, bool iValue)
        {
            iControl.Text = (iValue ? "True" : "False");
        }

        public static void SetValue(Label iControl, int iValue)
        {
            iControl.Text = iValue.ToString();
        }

        public static void SetValue(Label iControl, long iValue)
        {
            iControl.Text = iValue.ToString();
        }

        public static void SetValue(Label iControl, double iValue)
        {
            iControl.Text = iValue.ToString();
        }

        public static void SetValue(Label iControl, decimal iValue)
        {
            iControl.Text = iValue.ToString();
        }

        public static void SetDateValue(Label iControl, DateTime iValue)
        {
            iControl.Text = iValue.ToString("yyyy/MM/dd");
        }

        public static void SetTimeValue(Label iControl, DateTime iValue)
        {
            iControl.Text = iValue.ToString("HH:mm:ss");
        }

        public static void SetDateTimeValue(Label iControl, DateTime iValue)
        {
            iControl.Text = iValue.ToString("yyyy/MM/dd HH:mm:ss");
        }
        #endregion

        #region GetValue
        public static string GetValue(Label iControl)
        {
            return System.Web.HttpUtility.HtmlDecode(iControl.Text.Trim());
        }

        public static string[] GetValue2Array(Label iControl)
        {
            if (iControl.Text.Trim() == "")
            {
                return new string[0];
            }
            else
            {
                string[] iValue = System.Web.HttpUtility.HtmlDecode(iControl.Text).Split(',');
                for (int i = 0; i < iValue.Length; i++)
                {
                    iValue[i] = "," + iValue[i].Trim();
                }
                return iValue;
            }
        }

        public static bool GetValue2bool(Label iControl)
        {
            bool iValue;
            Boolean.TryParse(iControl.Text.Trim(), out iValue);
            return iValue;
        }

        public static int GetValue2int(Label iControl)
        {
            decimal iValue;
            Decimal.TryParse(iControl.Text.Trim(), out iValue);
            return Convert.ToInt32(Math.Round(iValue, MidpointRounding.AwayFromZero));
        }

        public static long GetValue2long(Label iControl)
        {
            decimal iValue;
            Decimal.TryParse(iControl.Text.Trim(), out iValue);
            return Convert.ToInt64(Math.Round(iValue, MidpointRounding.AwayFromZero));
        }

        public static double GetValue2double(Label iControl)
        {
            double iValue;
            Double.TryParse(iControl.Text.Trim(), out iValue);
            return iValue;
        }

        public static decimal GetValue2decimal(Label iControl)
        {
            decimal iValue;
            Decimal.TryParse(iControl.Text.Trim(), out iValue);
            return iValue;
        }

        public static DateTime GetValue2Date(Label iControl)
        {
            DateTime iValue;
            switch (iControl.Text.Length)
            {
                case 6:
                    DateTime.TryParseExact(iControl.Text.Trim(), "yyMMdd", null, DateTimeStyles.None, out iValue);
                    break;
                case 8:
                    DateTime.TryParseExact(iControl.Text.Trim(), "yyyyMMdd", null, DateTimeStyles.None, out iValue);
                    break;
                case 10:
                    DateTime.TryParseExact(iControl.Text.Trim(), "yyyy/MM/dd", null, DateTimeStyles.None, out iValue);
                    break;
                default:
                    DateTime.TryParse(iControl.Text.Trim(), out iValue);
                    break;
            }
            return iValue;
        }

        public static DateTime GetValue2Time(Label iControl)
        {
            DateTime iValue;
            switch (iControl.Text.Length)
            {
                case 6:
                    DateTime.TryParseExact(iControl.Text.Trim(), "HHmmss", null, DateTimeStyles.None, out iValue);
                    break;
                case 8:
                    DateTime.TryParseExact(iControl.Text.Trim(), "HH:mm:ss", null, DateTimeStyles.None, out iValue);
                    break;
                default:
                    DateTime.TryParse(iControl.Text.Trim(), out iValue);
                    break;
            }
            return iValue;
        }

        public static DateTime GetValue2DateTime(Label iControl)
        {
            DateTime iValue;
            switch (iControl.Text.Length)
            {
                case 14:
                    DateTime.TryParseExact(iControl.Text.Trim(), "yyyyMMddHHmmss", null, DateTimeStyles.None, out iValue);
                    break;
                case 19:
                    DateTime.TryParseExact(iControl.Text.Trim(), "yyyy/MM/dd HH:mm:ss", null, DateTimeStyles.None, out iValue);
                    break;
                default:
                    DateTime.TryParse(iControl.Text.Trim(), out iValue);
                    break;
            }
            return iValue;
        }

        public static DateTime GetValue2YM(Label iControl)
        {
            DateTime iValue;
            switch (iControl.Text.Length)
            {
                case 4:
                    DateTime.TryParseExact(iControl.Text.Trim() + "01", "yyMMdd", null, DateTimeStyles.None, out iValue);
                    break;
                case 6:
                    DateTime.TryParseExact(iControl.Text.Trim() + "01", "yyyyMMdd", null, DateTimeStyles.None, out iValue);
                    break;
                case 7:
                    DateTime.TryParseExact(iControl.Text.Trim() + "/01", "yyyy/MM/dd", null, DateTimeStyles.None, out iValue);
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
