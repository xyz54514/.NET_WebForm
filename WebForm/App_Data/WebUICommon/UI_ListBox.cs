using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Data;

namespace WebUICommon
{
    static partial class UI
    {
        public static void ClsValue(ListBox iControl)
        {
            foreach (ListItem _item in iControl.Items)
            {
                _item.Selected = false;
            }
        }

        public static void DataBind(ListBox iControl, DataTable DT)
        {
            FormatBindData(ref DT);
            iControl.DataSource = DT;
            iControl.DataValueField = DT.Columns[0].ColumnName;
            iControl.DataTextField = DT.Columns[1].ColumnName;
            iControl.DataBind();
            iControl.Items.Insert(0, "");
        }

        public static void DataBind(ListBox iControl, DataTable DT, string DataValueField, string DataTextField)
        {
            iControl.DataSource = DT;
            iControl.DataValueField = DataValueField;
            iControl.DataTextField = DataTextField;
            iControl.DataBind();
            iControl.Items.Insert(0, "");
        }

        public static void DataBind(ListBox iControl, string iValue)
        {
            if (iValue == null) iValue = "";
            DataBind(iControl, iValue.Split(','));
        }

        public static void DataBind(ListBox iControl, string[] iValue)
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

        public static void AddListItem(ListBox iControl, string iValue)
        {
            if (iValue == null) return;
            if (iValue == "") return;

            if (iControl.Items.FindByValue(iValue) == null)
                iControl.Items.Add(iValue);
        }

        public static void AddListItem(ListBox iControl, string[] iValues)
        {
            foreach (string iValue in iValues)
            {
                AddListItem(iControl, iValue);
            }
        }

        public static void AddListItem(ListBox iControl, ListItem iValue)
        {
            if (iValue == null) return;
            if (iValue.Value == "") return;

            if (iControl.Items.FindByValue(iValue.Value) == null)
                iControl.Items.Add(iValue);
        }

        public static void RemoveListItem(ListBox iControl, string iValue)
        {
            if (iValue == null) return;
            if (iValue == "") return;

            ListItem _temp = iControl.Items.FindByValue(iValue);
            if (_temp != null)
                iControl.Items.Remove(_temp);
        }

        public static void RemoveListItem(ListBox iControl)
        {
            for (int i = iControl.Items.Count - 1; i >= 0; i--)
            {
                if (iControl.Items[i].Selected)
                    iControl.Items.RemoveAt(i);
            }
        }

        public static string GetListItem(ListBox iControl)
        {
            string _Out = "";
            foreach (ListItem _item in iControl.Items)
            {
                _Out += "," + _item.Value;
            }
            if (_Out != "") _Out = _Out.Substring(1);
            return _Out;
        }

        public static void SetValue(ListBox iControl, string iValue)
        {
            SetValue(iControl, iValue.Split(','));
        }

        public static void SetValue(ListBox iControl, string[] iValue)
        {
            ClsValue(iControl);
            if (iValue == null) return;
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

        public static string GetValue(ListBox iControl)
        {
            string _Out = "";
            foreach (ListItem _item in iControl.Items)
            {
                if (_item.Value == "") continue;
                if (_item.Selected)
                    _Out += "," + _item.Value;
            }
            if (_Out != "") _Out = _Out.Substring(1);
            return _Out;
        }

        public static string[] GetValue2Array(ListBox iControl)
        {
            List<string> _Out = new List<string>();
            foreach (ListItem _item in iControl.Items)
            {
                if (_item.Value == "") continue;
                if (_item.Selected)
                    _Out.Add(_item.Value);
            }
            return _Out.ToArray();
        }
    }
}
