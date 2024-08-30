using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Globalization;

namespace WebUICommon
{
    static partial class UI
    {
        public static void ClsValue(TextBox iControl)
        {
            iControl.Text = "";
        }

        #region SetValue
        public static void SetValue(TextBox iControl, string iValue)
        {
            if (iValue == null)
                iControl.Text = "";
            else
                iControl.Text = iValue.Trim();
        }

        public static void SetValue(TextBox iControl, string[] iValue)
        {
            iControl.Text = "";
            foreach (string Temp in iValue)
            {
                iControl.Text += "," + Temp.Trim();
            }
            if (iControl.Text.Length > 0) iControl.Text = iControl.Text.Substring(1);
        }

        public static void SetValue(TextBox iControl, bool iValue)
        {
            iControl.Text = (iValue ? "True" : "False");
        }

        public static void SetValue(TextBox iControl, int iValue)
        {
            iControl.Text = iValue.ToString();
        }

        public static void SetValue(TextBox iControl, long iValue)
        {
            iControl.Text = iValue.ToString();
        }

        public static void SetValue(TextBox iControl, double iValue)
        {
            iControl.Text = iValue.ToString();
        }

        public static void SetValue(TextBox iControl, decimal iValue)
        {
            iControl.Text = iValue.ToString();
        }

        public static void SetValue(TextBox iControl, DateTime iValue)
        {
            switch (iControl.MaxLength)
            {
                case 8:
                    SetTimeValue(iControl, iValue);
                    break;
                case 10:
                    SetDateValue(iControl, iValue);
                    break;
                case 14:
                case 19:
                    SetDateTimeValue(iControl, iValue);
                    break;
                default:
                    SetDateTimeValue(iControl, iValue);
                    break;
            }
        }

        public static void SetDateValue(TextBox iControl, DateTime iValue)
        {
            switch (iControl.MaxLength)
            {
                case 6:
                    iControl.Text = iValue.ToString("yyMMdd");
                    break;
                case 8:
                    iControl.Text = iValue.ToString("yyyyMMdd");
                    break;
                case 10:
                    iControl.Text = iValue.ToString("yyyy/MM/dd");
                    break;
                default:
                    iControl.Text = iValue.ToString("yyyy/MM/dd");
                    break;
            }
        }

        public static void SetTimeValue(TextBox iControl, DateTime iValue)
        {
            switch (iControl.MaxLength)
            {
                case 6:
                    iControl.Text = iValue.ToString("HHmmss");
                    break;
                case 8:
                    iControl.Text = iValue.ToString("HH:mm:ss");
                    break;
                default:
                    iControl.Text = iValue.ToString("HH:mm:ss");
                    break;
            }
        }

        public static void SetDateTimeValue(TextBox iControl, DateTime iValue)
        {
            switch (iControl.MaxLength)
            {
                case 14:
                    iControl.Text = iValue.ToString("yyyyMMddHHmmss");
                    break;
                case 19:
                    iControl.Text = iValue.ToString("yyyy/MM/dd HH:mm:ss");
                    break;
                default:
                    iControl.Text = iValue.ToString("yyyy/MM/dd HH:mm:ss");
                    break;
            }
        }
        #endregion

        #region GetValue
        public static string GetValue(TextBox iControl)
        {
            return iControl.Text.Trim();
        }

        public static string[] GetValue2Array(TextBox iControl)
        {
            if (iControl.Text.Trim() == "")
            {
                return new string[0];
            }
            else
            {
                string[] iValue = iControl.Text.Split(',');
                for (int i = 0; i < iValue.Length; i++)
                {
                    iValue[i] = "," + iValue[i].Trim();
                }
                return iValue;
            }
        }

        public static bool GetValue2bool(TextBox iControl)
        {
            bool iValue;
            Boolean.TryParse(iControl.Text.Trim(), out iValue);
            return iValue;
        }

        public static int GetValue2int(TextBox iControl)
        {
            decimal iValue;
            Decimal.TryParse(iControl.Text.Trim(), out iValue);
            return Convert.ToInt32(Math.Round(iValue, MidpointRounding.AwayFromZero));
        }

        public static long GetValue2long(TextBox iControl)
        {
            decimal iValue;
            Decimal.TryParse(iControl.Text.Trim(), out iValue);
            return Convert.ToInt64(Math.Round(iValue, MidpointRounding.AwayFromZero));
        }

        public static double GetValue2double(TextBox iControl)
        {
            double iValue;
            Double.TryParse(iControl.Text.Trim(), out iValue);
            return iValue;
        }

        public static decimal GetValue2decimal(TextBox iControl)
        {
            decimal iValue;
            Decimal.TryParse(iControl.Text.Trim(), out iValue);
            return iValue;
        }

        public static DateTime GetValue2Date(TextBox iControl)
        {
            DateTime iValue;
            switch (iControl.MaxLength)
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

        public static DateTime GetValue2Time(TextBox iControl)
        {
            DateTime iValue;
            switch (iControl.MaxLength)
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

        public static DateTime GetValue2DateTime(TextBox iControl)
        {
            DateTime iValue;
            switch (iControl.MaxLength)
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

        public static DateTime GetValue2YM(TextBox iControl)
        {
            DateTime iValue;
            switch (iControl.MaxLength)
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