using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace WebUICommon
{
    public static partial class UI
    {
        static void FormatBindData(ref DataTable DT)
        {
            foreach (DataRow DR in DT.Rows)
            {
                DR[1] = DR[0].ToString() + " - " + DR[1].ToString();
            }

            DT.AcceptChanges();
        }

        #region GetValue
        public static string GetValue(string iControl)
        {
            return iControl.Trim();
        }

        public static string[] GetValue2Array(string iControl)
        {
            if (iControl.Trim() == "")
            {
                return new string[0];
            }
            else
            {
                string[] iValue = iControl.Split(',');
                for (int i = 0; i < iValue.Length; i++)
                {
                    iValue[i] = "," + iValue[i].Trim();
                }
                return iValue;
            }
        }

        public static bool GetValue2bool(string iControl)
        {
            bool iValue;
            Boolean.TryParse(iControl.Trim(), out iValue);
            return iValue;
        }

        public static int GetValue2int(string iControl)
        {
            decimal iValue;
            Decimal.TryParse(iControl.Trim(), out iValue);
            return Convert.ToInt32(Math.Round(iValue, MidpointRounding.AwayFromZero));
        }

        public static long GetValue2long(string iControl)
        {
            decimal iValue;
            Decimal.TryParse(iControl.Trim(), out iValue);
            return Convert.ToInt64(Math.Round(iValue, MidpointRounding.AwayFromZero));
        }

        public static double GetValue2double(string iControl)
        {
            double iValue;
            Double.TryParse(iControl.Trim(), out iValue);
            return iValue;
        }

        public static decimal GetValue2decimal(string iControl)
        {
            decimal iValue;
            Decimal.TryParse(iControl.Trim(), out iValue);
            return iValue;
        }

        public static DateTime GetValue2Date(string iControl)
        {
            DateTime iValue;
            switch (iControl.Length)
            {
                case 6:
                    DateTime.TryParseExact(iControl.Trim(), "yyMMdd", null, DateTimeStyles.None, out iValue);
                    break;
                case 8:
                    DateTime.TryParseExact(iControl.Trim(), "yyyyMMdd", null, DateTimeStyles.None, out iValue);
                    break;
                case 10:
                    DateTime.TryParseExact(iControl.Trim(), "yyyy/MM/dd", null, DateTimeStyles.None, out iValue);
                    break;
                default:
                    DateTime.TryParse(iControl.Trim(), out iValue);
                    break;
            }
            return iValue;
        }

        public static DateTime GetValue2Time(string iControl)
        {
            DateTime iValue;
            switch (iControl.Length)
            {
                case 6:
                    DateTime.TryParseExact(iControl.Trim(), "HHmmss", null, DateTimeStyles.None, out iValue);
                    break;
                case 8:
                    DateTime.TryParseExact(iControl.Trim(), "HH:mm:ss", null, DateTimeStyles.None, out iValue);
                    break;
                default:
                    DateTime.TryParse(iControl.Trim(), out iValue);
                    break;
            }
            return iValue;
        }

        public static DateTime GetValue2DateTime(string iControl)
        {
            DateTime iValue;
            switch (iControl.Length)
            {
                case 14:
                    DateTime.TryParseExact(iControl.Trim(), "yyyyMMddHHmmss", null, DateTimeStyles.None, out iValue);
                    break;
                case 19:
                    DateTime.TryParseExact(iControl.Trim(), "yyyy/MM/dd HH:mm:ss", null, DateTimeStyles.None, out iValue);
                    break;
                default:
                    DateTime.TryParse(iControl.Trim(), out iValue);
                    break;
            }
            return iValue;
        }

        public static DateTime GetValue2YM(string iControl)
        {
            DateTime iValue;
            switch (iControl.Length)
            {
                case 4:
                    DateTime.TryParseExact(iControl.Trim() + "01", "yyMMdd", null, DateTimeStyles.None, out iValue);
                    break;
                case 6:
                    DateTime.TryParseExact(iControl.Trim() + "01", "yyyyMMdd", null, DateTimeStyles.None, out iValue);
                    break;
                case 7:
                    DateTime.TryParseExact(iControl.Trim() + "/01", "yyyy/MM/dd", null, DateTimeStyles.None, out iValue);
                    break;
                default:
                    iValue = DateTime.MinValue;
                    break;
            }
            return iValue;
        }
        #endregion

        public static void ResetCanvas(ControlCollection controls)
        {
            foreach (Control item in controls)
            {
                if (item is Panel)
                    item.Visible = false;

                if (item.HasControls())
                    ResetCanvas(item.Controls);
            }
        }
    }
}