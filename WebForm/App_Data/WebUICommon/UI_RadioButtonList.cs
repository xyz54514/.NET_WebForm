using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Data;
using System.Globalization;

namespace WebUICommon
{
    static partial class UI
    {
        public static void ClsValue(RadioButtonList iControl)
        {
            iControl.SelectedIndex = -1;
        }

        public static void DataBind(RadioButtonList iControl, DataTable DT)
        {
            FormatBindData(ref DT);
            iControl.DataSource = DT;
            iControl.DataValueField = DT.Columns[0].ColumnName;
            iControl.DataTextField = DT.Columns[1].ColumnName;
            iControl.DataBind();
        }

        public static void DataBind(RadioButtonList iControl, DataTable DT, string DataValueField, string DataTextField)
        {
            iControl.DataSource = DT;
            iControl.DataValueField = DataValueField;
            iControl.DataTextField = DataTextField;
            iControl.DataBind();
        }

        public static void DataBind(RadioButtonList iControl, string iValue)
        {
            DataBind(iControl, iValue.Split(','));
        }

        public static void DataBind(RadioButtonList iControl, string[] iValue)
        {
            iControl.Items.Clear();
            if (iValue == null) return;
            if (iValue.Length == 0) return;

            ListItem _temp = null;
            foreach (string _s in iValue)
            {
                if (_s != "")
                {
                    _temp = iControl.Items.FindByValue(_s);
                    if (_temp == null)
                        iControl.Items.Add(_s);
                }
            }
        }

        public static void SetValue(RadioButtonList iControl, string iValue)
        {
            if (iControl.Items.FindByValue(iValue) != null)
            {
                iControl.SelectedValue = iValue;
            }
        }

        #region GetValue
        public static string GetValue(RadioButtonList iControl)
        {
            return iControl.SelectedValue.Trim();
        }

        public static bool GetValue2bool(RadioButtonList iControl)
        {
            bool iValue;
            Boolean.TryParse(iControl.SelectedValue.Trim(), out iValue);
            return iValue;
        }

        public static int GetValue2int(RadioButtonList iControl)
        {
            decimal iValue;
            Decimal.TryParse(iControl.SelectedValue.Trim(), out iValue);
            return Convert.ToInt32(Math.Round(iValue, MidpointRounding.AwayFromZero));
        }

        public static long GetValue2long(RadioButtonList iControl)
        {
            decimal iValue;
            Decimal.TryParse(iControl.SelectedValue.Trim(), out iValue);
            return Convert.ToInt64(Math.Round(iValue, MidpointRounding.AwayFromZero));
        }

        public static double GetValue2double(RadioButtonList iControl)
        {
            double iValue;
            Double.TryParse(iControl.SelectedValue.Trim(), out iValue);
            return iValue;
        }

        public static decimal GetValue2decimal(RadioButtonList iControl)
        {
            decimal iValue;
            Decimal.TryParse(iControl.SelectedValue.Trim(), out iValue);
            return iValue;
        }

        public static DateTime GetValue2Date(RadioButtonList iControl)
        {
            DateTime iValue;
            switch (iControl.SelectedValue.Length)
            {
                case 6:
                    DateTime.TryParseExact(iControl.SelectedValue.Trim(), "yyMMdd", null, DateTimeStyles.None, out iValue);
                    break;
                case 8:
                    DateTime.TryParseExact(iControl.SelectedValue.Trim(), "yyyyMMdd", null, DateTimeStyles.None, out iValue);
                    break;
                case 10:
                    DateTime.TryParseExact(iControl.SelectedValue.Trim(), "yyyy/MM/dd", null, DateTimeStyles.None, out iValue);
                    break;
                default:
                    DateTime.TryParse(iControl.SelectedValue.Trim(), out iValue);
                    break;
            }
            return iValue;
        }

        public static DateTime GetValue2Time(RadioButtonList iControl)
        {
            DateTime iValue;
            switch (iControl.SelectedValue.Length)
            {
                case 6:
                    DateTime.TryParseExact(iControl.SelectedValue.Trim(), "HHmmss", null, DateTimeStyles.None, out iValue);
                    break;
                case 8:
                    DateTime.TryParseExact(iControl.SelectedValue.Trim(), "HH:mm:ss", null, DateTimeStyles.None, out iValue);
                    break;
                default:
                    DateTime.TryParse(iControl.SelectedValue.Trim(), out iValue);
                    break;
            }
            return iValue;
        }

        public static DateTime GetValue2DateTime(RadioButtonList iControl)
        {
            DateTime iValue;
            switch (iControl.SelectedValue.Length)
            {
                case 14:
                    DateTime.TryParseExact(iControl.SelectedValue.Trim(), "yyyyMMddHHmmss", null, DateTimeStyles.None, out iValue);
                    break;
                case 19:
                    DateTime.TryParseExact(iControl.SelectedValue.Trim(), "yyyy/MM/dd HH:mm:ss", null, DateTimeStyles.None, out iValue);
                    break;
                default:
                    DateTime.TryParse(iControl.SelectedValue.Trim(), out iValue);
                    break;
            }
            return iValue;
        }
        #endregion
    }
}
