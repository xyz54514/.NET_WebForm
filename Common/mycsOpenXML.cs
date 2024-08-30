using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common
{
    public class mycsOpenXML
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pOutputFilePath">要取代的路徑</param>
        /// <param name="pReplaceDic"></param>
        /// <returns></returns>
        public bool WordReplace(string pOutputFilePath, Dictionary<string, string> pReplaceDic)
        {
            bool tResultbool = true;
            using (WordprocessingDocument tWordDocument = WordprocessingDocument.Open(pOutputFilePath, true))
            {
                Body tBody = tWordDocument.MainDocumentPart.Document.Body;

                foreach (KeyValuePair<string, string> tKeyVP in pReplaceDic)
                {
                    string tSource = tKeyVP.Value;
                    string tTarget = tKeyVP.Key;
                    char[] tTargetArray = tTarget.ToCharArray();

                    foreach (Paragraph tParagraph in tBody.Descendants<Paragraph>())
                    {
                        //若尋找目標存在於此段落
                        if (tParagraph.InnerText.Trim().Contains(tTarget) == true)
                        {
                            int tIndex = 0;  //Target Index
                            int tRunIndex = 0; // 目前 Run 的 Index
                            int tStartRun = 0; //啟始的 Run
                            string tStartString = string.Empty;
                            int tEndRun = 0; //最後的 Run
                            string tEndString = string.Empty;
                            bool tIsFindStart = false;
                            foreach (Run tRun in tParagraph.Descendants<Run>())
                            {
                                char[] tSourceArray = tRun.InnerText.Trim().ToCharArray();

                                bool tIsFind = FindWord(tTargetArray, tSourceArray, ref tIndex);

                                if (tIsFind == true)
                                {
                                    if (tIsFindStart == false)
                                    {
                                        tIsFindStart = true; //記錄目前有找到Run
                                        tStartRun = tRunIndex; //記錄第一個找到的Run
                                        tStartString = tTarget.Substring(0, tIndex + 1);
                                        tTargetArray = null;
                                        tTargetArray = tTarget.Substring(tIndex + 1, tTarget.Length - (tIndex + 1)).ToCharArray();
                                        tIndex = 0;

                                        if (tTargetArray.Length == 0)
                                        {
                                            tEndRun = tRunIndex;
                                            tEndString = tStartString;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        string tTempString = new string(tTargetArray);
                                        tTargetArray = null;
                                        tTargetArray = tTempString.Substring(tIndex + 1, tTempString.Length - (tIndex + 1)).ToCharArray();
                                        tIndex = 0;

                                        if (tTargetArray.Length == 0)
                                        {
                                            tEndRun = tRunIndex;
                                            tEndString = tTempString;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    if (tIsFindStart == true)
                                    {
                                        tStartRun = -1;
                                        tIsFindStart = false;
                                        tTargetArray = null;
                                        tTargetArray = tTarget.ToCharArray();
                                        tIndex = 0;
                                        tStartString = string.Empty;
                                        tEndString = string.Empty;
                                    }
                                }

                                tRunIndex++;
                            }

                            tRunIndex = 0;
                            foreach (Run tRun in tParagraph.Descendants<Run>())
                            {
                                if (tRunIndex >= tStartRun && tRunIndex <= tEndRun)
                                {
                                    foreach (Text tText in tRun.Descendants<Text>())
                                    {
                                        if (tRunIndex == tStartRun || tStartRun == tEndRun)
                                        {
                                            tText.Text = tText.Text.Replace(tStartString, tSource);
                                        }
                                        else if (tRunIndex == tEndRun)
                                        {
                                            tText.Text = tText.Text.Replace(tEndString, "");
                                        }
                                        else
                                        {
                                            tText.Text = "";
                                        }
                                    }
                                }
                                tRunIndex++;
                            }

                            //重新設定一次要尋找的目標
                            tTargetArray = tTarget.ToCharArray();
                        }
                    }
                }

                tWordDocument.MainDocumentPart.Document.Save();
                tWordDocument.Dispose();
            }

            return tResultbool;
        }

        public bool FindWord(char[] pTargetArray, char[] pSourceArray, ref int pTargetIndex)
        {
            bool tResultBool = true;

            if (pSourceArray.Length > 0)
            {
                for (int i = 0; i < pSourceArray.Length; i++)
                {
                    if (pSourceArray[i] == pTargetArray[pTargetIndex])
                    {
                        if ((pTargetIndex + 1) == pTargetArray.Length || (pTargetIndex + 1) == pSourceArray.Length)
                        {
                            break;
                        }
                        pTargetIndex++;
                    }
                    else
                    {
                        if ((pTargetIndex + 1) == pTargetArray.Length && (pSourceArray.Length - (pTargetIndex + 1)) == 0)
                        {
                            tResultBool = false;
                            pTargetIndex = 0;
                            break;
                        }
                        else
                        {
                            //重置 Source Array
                            string tTempString = new string(pSourceArray);
                            //一次右移一碼
                            char[] tSourceArray = tTempString.Substring(1, pSourceArray.Length - 1).ToCharArray();
                            //重置目標 index
                            pTargetIndex = 0;

                            tResultBool = FindWord(pTargetArray, tSourceArray, ref pTargetIndex);
                            break;
                        }
                    }
                }
            }
            else
            {
                tResultBool = false;
                pTargetIndex = 0;
            }

            return tResultBool;
        }

        public void InsertPicture(string pOutputFilePath, Dictionary<string, string> pReplacePic)
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(pOutputFilePath, true))
            {
                Body tBody = wordDoc.MainDocumentPart.Document.Body;

                foreach (KeyValuePair<string, string> tKeyVP in pReplacePic)
                {
                    string tSource = tKeyVP.Value;
                    string tTarget = tKeyVP.Key;
                    char[] tTargetArray = tTarget.ToCharArray();

                    foreach (Paragraph tParagraph in tBody.Descendants<Paragraph>())
                    {
                        //若尋找目標存在於此段落
                        if (tParagraph.InnerText.Trim().Contains(tTarget) == true)
                        {
                            int tIndex = 0;  //Target Index
                            int tRunIndex = 0; // 目前 Run 的 Index
                            int tStartRun = 0; //啟始的 Run
                            string tStartString = string.Empty;
                            int tEndRun = 0; //最後的 Run
                            string tEndString = string.Empty;
                            bool tIsFindStart = false;
                            foreach (Run tRun in tParagraph.Descendants<Run>())
                            {
                                char[] tSourceArray = tRun.InnerText.Trim().ToCharArray();

                                bool tIsFind = FindWord(tTargetArray, tSourceArray, ref tIndex);

                                if (tIsFind == true)
                                {
                                    if (tIsFindStart == false)
                                    {
                                        tIsFindStart = true; //記錄目前有找到Run
                                        tStartRun = tRunIndex; //記錄第一個找到的Run
                                        tStartString = tTarget.Substring(0, tIndex + 1);
                                        tTargetArray = null;
                                        tTargetArray = tTarget.Substring(tIndex + 1, tTarget.Length - (tIndex + 1)).ToCharArray();
                                        tIndex = 0;

                                        if (tTargetArray.Length == 0)
                                        {
                                            tEndRun = tRunIndex;
                                            tEndString = tStartString;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        string tTempString = new string(tTargetArray);
                                        tTargetArray = null;
                                        tTargetArray = tTempString.Substring(tIndex + 1, tTempString.Length - (tIndex + 1)).ToCharArray();
                                        tIndex = 0;

                                        if (tTargetArray.Length == 0)
                                        {
                                            tEndRun = tRunIndex;
                                            tEndString = tTempString;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    if (tIsFindStart == true)
                                    {
                                        tStartRun = -1;
                                        tIsFindStart = false;
                                        tTargetArray = null;
                                        tTargetArray = tTarget.ToCharArray();
                                        tIndex = 0;
                                        tStartString = string.Empty;
                                        tEndString = string.Empty;
                                    }
                                }

                                tRunIndex++;
                            }

                            tRunIndex = 0;
                            foreach (Run tRun in tParagraph.Descendants<Run>())
                            {
                                if (tRunIndex >= tStartRun && tRunIndex <= tEndRun)
                                {
                                    foreach (Text tText in tRun.Descendants<Text>())
                                    {
                                        if (tRunIndex == tStartRun || tStartRun == tEndRun)
                                        {
                                            tText.Text = tText.Text.Replace(tStartString, tSource);
                                        }
                                        else if (tRunIndex == tEndRun)
                                        {
                                            tText.Text = tText.Text.Replace(tEndString, "");
                                        }
                                        else
                                        {
                                            tText.Text = "";
                                        }
                                    }
                                }
                                tRunIndex++;
                            }

                            //重新設定一次要尋找的目標
                            tTargetArray = tTarget.ToCharArray();
                        }
                    }
                }

                wordDoc.MainDocumentPart.Document.Save();
                wordDoc.Dispose();
            }
        }
    }
}
