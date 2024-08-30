using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Xml;

namespace Common
{
    public class XmlFunc
    {
        /// <summary>
        /// 建構函數
        /// </summary>
        public XmlFunc()
        {

        }



        /// <summary>
        /// 將xml字串轉換為DataSet
        /// </summary>
        /// <param name="xmlData"></param>
        /// <returns></returns>
        public DataSet ConvertXMLToDataSet(string xmlData)
        {
            StringReader stream = null;
            XmlTextReader reader = null;
            try
            {
                DataSet xmlDS = new DataSet();
                stream = new StringReader(xmlData);
                //从stream装载到XmlTextReader
                reader = new XmlTextReader(stream);
                xmlDS.ReadXml(reader, XmlReadMode.Auto);
                return xmlDS;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

        /// <summary>
        /// 將xml文件轉換為DataSet
        /// </summary>
        /// <param name="xmlFile"></param>
        /// <returns></returns>
        public DataSet ConvertXMLFileToDataSet(string xmlFile)
        {
            StringReader stream = null;
            XmlTextReader reader = null;
            try
            {
                XmlDocument xmld = new XmlDocument();
                xmld.Load(xmlFile);

                DataSet xmlDS = new DataSet();
                stream = new StringReader(xmld.InnerXml);
                //从stream Load 到XmlTextReader
                reader = new XmlTextReader(stream);
                xmlDS.ReadXml(reader, XmlReadMode.ReadSchema);
                //xmlDS.ReadXml(xmlFile);
                return xmlDS;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

        /// <summary>
        /// 將DataSet轉換為xml字串
        /// </summary>
        /// <param name="xmlDS"></param>
        /// <returns></returns>
        public string ConvertDataSetToXML(DataSet xmlDS)
        {
            MemoryStream stream = null;
            XmlTextWriter writer = null;

            try
            {
                stream = new MemoryStream();
                //以stream載入到XmlTextReader
                writer = new XmlTextWriter(stream, Encoding.Unicode);

                //用WriteXml方法寫入文件並包含Schema.
                xmlDS.WriteXml(writer, XmlWriteMode.WriteSchema);
                int count = (int)stream.Length;
                byte[] arr = new byte[count];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(arr, 0, count);

                UnicodeEncoding utf = new UnicodeEncoding();
                return utf.GetString(arr).Trim();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (writer != null) writer.Close();
            }
        }

        /// <summary>
        /// 將DataSet轉換為xml文件
        /// </summary>
        /// <param name="xmlDS"></param>
        /// <param name="xmlFile"></param>
        public void ConvertDataSetToXMLFile(DataSet xmlDS, string xmlFile)
        {
            MemoryStream stream = null;
            XmlTextWriter writer = null;

            try
            {
                stream = new MemoryStream();
                //從stream Load XmlTextReader
                writer = new XmlTextWriter(stream, Encoding.Unicode);

                //用WriteXml方法寫入文件
                xmlDS.WriteXml(writer, XmlWriteMode.WriteSchema);
                int count = (int)stream.Length;
                byte[] arr = new byte[count];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(arr, 0, count);

                //傳回Unicode字串

                UnicodeEncoding utf = new UnicodeEncoding();
                StreamWriter sw = new StreamWriter(xmlFile);
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                sw.WriteLine(utf.GetString(arr).Trim());
                sw.Close();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (writer != null) writer.Close();
            }
        }

        /// <summary>
        /// 在指定節點插入CDATA
        /// </summary>
        /// <param name="ds">DataSet</param>
        /// <param name="cdataSections">含CDATA數據的節點</param>
        /// <returns></returns>
        private string InsertCDATASections(DataSet ds, string[] cdataSections)
        {
            //Convert to XML with expanded general entities and CDATA sections
            //as appropriate
            XmlValidatingReader reader = null;
            XmlTextWriter writer = null;
            StringWriter sw = null;

            Array.Sort(cdataSections);
            try
            {
                reader = new XmlValidatingReader(ds.GetXml(), XmlNodeType.Document, null);
                sw = new StringWriter();

                writer = new XmlTextWriter(sw);

                writer.Formatting = Formatting.Indented;
                reader.ValidationType = ValidationType.None;
                reader.EntityHandling = EntityHandling.ExpandCharEntities;
                string currentElement = String.Empty;
                writer.WriteStartDocument(true);
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            currentElement = reader.Name;
                            writer.WriteStartElement(currentElement);
                            while (reader.MoveToNextAttribute())
                            {
                                writer.WriteAttributeString(reader.Name, reader.Value);
                            }
                            break;
                        case XmlNodeType.Text:
                            if (Array.BinarySearch(cdataSections, currentElement) < 0)
                            {
                                writer.WriteString(reader.Value);
                            }
                            else
                            {
                                writer.WriteCData(reader.Value);
                            }
                            break;
                        case XmlNodeType.EndElement:
                            writer.WriteEndElement();
                            break;
                        default:
                            break;
                    }
                }
                writer.WriteEndDocument();
            }
            catch (Exception exp)
            {
                return exp.Message;
            }
            finally
            {
                reader.Close();
                writer.Close();
            }

            return sw.ToString();//sw.ToString();  

        }

        /// <summary>
        /// 將DataSet轉換為XML並將String欄位補入Cdata
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public string ConvertDataSetToXmlWithCDATA(DataSet ds)
        {

            System.Collections.Specialized.StringCollection sc = new System.Collections.Specialized.StringCollection();

            //取得所有字串欄位

            foreach (DataTable dt in ds.Tables)
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    if (dc.DataType == System.Type.GetType("System.String"))
                    {
                        sc.Add(dc.ColumnName);
                    }
                }
            }


            string[] strArray = new string[sc.Count];
            sc.CopyTo(strArray, 0);

            //進行轉換
            string xml = this.InsertCDATASections(ds, strArray);

            xml = xml.Replace("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>", "");
            return xml;
        }
    }
}

