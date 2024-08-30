using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Common
{
    public class MailAttachment
    {
        public string name { get; set; }
        public byte[] data { get; set; }
        public MailAttachment(string sName, byte[] bData)
        {
            name = sName;
            data = bData;
        }
    }

    public class Mail
    {
        string _Server = "";
        int _Port;
        string _Account = "";
        string _Pwd = "";
        bool _enableSSL = false;

        public string smtpServer { set { _Server = value; } }
        public int smtpPort { set { _Port = value; } }
        public string mailAccount { set { _Account = value; } }
        public string mailPwd { set { _Pwd = value; } }
        public bool enableSSL { set { _enableSSL = value; } }


        /// <summary>
        /// 完整的寄信功能
        /// </summary>
        /// <param name="MailFrom">寄信人E-mail Address</param>
        /// <param name="MailTos">收信人E-mail Address</param>
        /// <param name="Ccs">副本E-mail Address</param>
        /// <param name="MailSub">主旨</param>
        /// <param name="MailBody">信件內容</param>
        /// <param name="isBodyHtml">是否採用HTML格式</param>
        /// <param name="filePaths">附檔在WebServer檔案總管路徑</param>
        public void Send(string MailFrom, string[] MailTos, string[] Ccs, string MailSub, string MailBody, bool isBodyHtml, MailAttachment[] files)
        {
            //建立MailMessage物件
            MailMessage mms = new MailMessage();
            //指定一位寄信人MailAddress
            mms.From = new MailAddress(MailFrom);
            //信件主旨
            mms.Subject = MailSub;
            //信件內容
            mms.BodyEncoding = System.Text.Encoding.UTF8;
            mms.Body = MailBody;
            //信件內容 是否採用Html格式
            mms.IsBodyHtml = isBodyHtml;

            #region 收件人
            if (MailTos != null)//防呆
            {
                for (int i = 0; i < MailTos.Length; i++)
                {
                    //加入信件的收信人(們)address
                    if (!string.IsNullOrEmpty(MailTos[i].Trim()))
                    {
                        mms.To.Add(new MailAddress(MailTos[i].Trim()));
                    }
                }
            }
            #endregion

            #region 寄件副本
            if (Ccs != null) //防呆
            {
                for (int i = 0; i < Ccs.Length; i++)
                {
                    if (!string.IsNullOrEmpty(Ccs[i].Trim()))
                    {
                        //加入信件的副本(們)address
                        mms.CC.Add(new MailAddress(Ccs[i].Trim()));
                    }
                }
            }
            #endregion

            #region 附加檔案
            if (files != null)//防呆
            {//有夾帶檔案
                for (int i = 0; i < files.Length; i++)
                {
                    if (!string.IsNullOrEmpty(files[i].name))
                    {
                        Attachment file = new Attachment(new MemoryStream(files[i].data), files[i].name);
                        //加入信件的夾帶檔案
                        mms.Attachments.Add(file);
                    }
                }
            }//End if (filePaths!=null)//防呆
            #endregion

            #region 發送mail
            SmtpClient smtpClient = new SmtpClient(_Server, _Port);//或公司、客戶的smtp_server

            if (!string.IsNullOrEmpty(_Account) && !string.IsNullOrEmpty(_Pwd))//有帳密的話
            {
                //※程式在客戶那邊執行的話，問客戶，程式在自家公司執行的話，問自家公司MIS，最準確XD
                smtpClient.Credentials = new NetworkCredential(_Account, _Pwd);//寄信帳密
            }
            smtpClient.EnableSsl = _enableSSL;

            smtpClient.Send(mms);//寄出一封信
            #endregion

            #region 釋放每個附件，才不會Lock住
            if (mms.Attachments != null && mms.Attachments.Count > 0)
            {
                for (int i = 0; i < mms.Attachments.Count; i++)
                {
                    mms.Attachments[i].Dispose();
                }
            }
            #endregion
        }
    }
}