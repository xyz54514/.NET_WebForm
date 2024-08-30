<%@ Control Language="C#" AutoEventWireup="true" Inherits="WebForm.UserControl.ucCalendar" Codebehind="ucCalendar.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:TextBox runat="server" ID="txtCalendar" Width="100px"></asp:TextBox>
<asp:ImageButton runat="server" ID="ibtnCalendar" ImageUrl="~/Images/Calendar.gif"
    AlternateText="開啟小日曆" OnClientClick="return false;" />
<asp:ImageButton ID="ibtnClear" runat="server" ImageUrl="~/Images/cross.png" AlternateText="清空"/>
<cc1:CalendarExtender runat="server" TargetControlID="txtCalendar" Format="yyyy/MM/dd"
    PopupButtonID="ibtnCalendar" ID="ceCalendar" />
<cc1:MaskedEditExtender ID="MaskedEditExtender" runat="server" TargetControlID="txtCalendar"
    Mask="9999/99/99" MessageValidatorTip="true" MaskType="Date" CultureName="zh-TW" />
<asp:CompareValidator ID="CompareValidator" runat="server" ControlToValidate="txtCalendar"
    Operator="DataTypeCheck" Type="Date" Display="Dynamic">日期錯誤</asp:CompareValidator>