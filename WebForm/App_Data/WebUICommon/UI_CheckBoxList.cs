using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Data;

namespace WebUICommon
{
    static partial class UI
    {
        public static void ClsValue(CheckBoxList iControl)
        {
            foreach (ListItem _item in iControl.Items)
            {
                _item.Selected = false;
            }
        }

        public static void DataBind(CheckBoxList iControl, DataTable DT)
        {
            FormatBindData(ref DT);
            iControl.DataSource = DT;
            iControl.DataValueField = DT.Columns[0].ColumnName;
            iControl.DataTextField = DT.Columns[1].ColumnName;
            iControl.DataBind();
        }

        public static void DataBind(CheckBoxList iControl, DataTable DT, string DataValueField, string DataTextField)
        {
            iControl.DataSource = DT;
            iControl.DataValueField = DataValueField;
            iControl.DataTextField = DataTextField;
            iControl.DataBind();
        }

        public static void DataBind(CheckBoxList iControl, string iValue)
        {
            DataBind(iControl, iValue.Split(','));
        }

        public static void DataBind(CheckBoxList iControl, string[] iValue)
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

        public static void SetValue(CheckBoxList iControl, string iValue)
        {
            SetValue(iControl, iValue.Split(','));
        }

        public static void SetValue(CheckBoxList iControl, string[] iValue)
        {
            ClsValue(iControl);
            if (iValue.Length == 0) return;
            if (iValue.Length == 1 && iValue[0] == "") return;

            ListItem _temp = null;
            foreach (string _s in iValue)
            {
                if (_s != "")
                {
                    _temp = iControl.Items.FindByValue(_s);
                    if (_temp != null)
                    {
                        _temp.Selected = true;
                    }
                }
            }
        }

        public static string GetValue(CheckBoxList iControl)
        {
            string _Out = "";
            foreach (ListItem _item in iControl.Items)
            {
                if (_item.Selected)
                {
                    _Out += "," + _item.Value;
                }
            }
            if (_Out != "") _Out = _Out.Substring(1);
            return _Out;
        }

        public static string[] GetValue2Array(CheckBoxList iControl)
        {
            List<string> _Out = new List<string>();
            foreach (ListItem _item in iControl.Items)
            {
                if (_item.Selected)
                {
                    _Out.Add(_item.Value);
                }
            }
            return _Out.ToArray();
        }
    }
}
