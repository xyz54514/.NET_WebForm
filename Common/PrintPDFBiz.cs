using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;


namespace Common
{
    public class PrintPDFBiz
    {
        /// <summary>
        /// 開啟PDF Template並將值填入PDF
        /// </summary>
        /// <param name="TemplatePath"></param>
        /// <param name="dicValue"></param>
        /// <returns></returns>
        public byte[] DoPrint(string TemplatePath, Dictionary<string, string> dicValue)
        {
            byte[] tFile = new byte[] { };

            using (MemoryStream ms = new MemoryStream())
            {
                PdfReader reader = new PdfReader(TemplatePath);
                PdfStamper stamper = new PdfStamper(reader, ms);
                //BaseFont bfChinese = BaseFont.CreateFont(@"c:\Windows\Fonts\SIMSUN.TTC,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                //BaseFont bfChinese = BaseFont.CreateFont(@"c:\Windows\Fonts\KAIU.TTF", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                BaseFont bfChinese = BaseFont.CreateFont(@"c:\Windows\Fonts\KAIU.TTF", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                stamper.AcroFields.AddSubstitutionFont(bfChinese);

                BaseFont.HasSetSubstitutionFont = true;
                AcroFields fields = stamper.AcroFields;

                BaseFont.AddToResourceSearch(Assembly.Load("iTextAsian"));
                //iTextSharp.text.io.StreamUtil.AddToResourceSearch(Assembly.Load("iTextAsian"));

                foreach (string key in fields.Fields.Keys)
                {
                    string s;

                    if (dicValue.TryGetValue(key, out s))
                    {
                        fields.SetField(key, s, s);
                    }
                    else
                    {
                        fields.SetField(key, "");
                    }
                }

                stamper.FormFlattening = true;

                stamper.Close();
                reader.Close();

                tFile = ms.ToArray();
            }

            return tFile;
        }
    }
}
