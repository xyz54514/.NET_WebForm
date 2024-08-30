using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Common
{
    public class EPPlus
    {
        public ExcelWorkbook EppWorkBook;
        //public ExcelWorksheet EppWorkSheet;

        public EPPlus(string TemplatePath)
        {
            var existingFile = new FileInfo(TemplatePath);
            ExcelPackage package = new ExcelPackage(existingFile);
            EppWorkBook = package.Workbook;
        }

        public static MemoryStream DateSetExport(DataSet ds)
        {
            ExcelPackage package = new ExcelPackage();

            foreach (DataTable dt in ds.Tables)
            {
                DateTableExport(package, dt, dt.TableName);
            }

            MemoryStream ms = new MemoryStream();
            package.SaveAs(ms);
            ms.Flush();
            ms.Position = 0;

            package = null;
            return ms;


        }

        public static MemoryStream DateTableExport(DataTable dt, string title)
        {
            ExcelPackage package = new ExcelPackage();

            DateTableExport(package, dt, title);

            MemoryStream ms = new MemoryStream();
            package.SaveAs(ms);
            ms.Flush();
            ms.Position = 0;

            package = null;
            return ms;
        }

        public static void DateTableExport(ExcelPackage package, DataTable dt, string title)
        {
            System.Drawing.Color col02507C = System.Drawing.ColorTranslator.FromHtml("#02507C");

            string sheetName = dt.TableName;
            if (sheetName == "") sheetName = "Sheet" + Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);
            var ws = package.Workbook.Worksheets.Add(sheetName);
            ws.Cells[1, 1].Value = title;  //定義起始
            ws.Cells[1, 1, 1, dt.Columns.Count].Merge = true;

            ws.Cells[1, 1, 1, dt.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            for (int iCol = 0; iCol < dt.Columns.Count; iCol++)
            {
                ws.Cells[2, iCol + 1].Value = dt.Columns[iCol].ColumnName;

                ws.Cells[2, iCol + 1].Style.Font.Color.SetColor(System.Drawing.Color.White);

                ws.Cells[2, iCol + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                ws.Cells[2, iCol + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[2, iCol + 1].Style.Fill.BackgroundColor.SetColor(col02507C);

                switch (dt.Columns[iCol].DataType.Name.ToUpper())
                {
                    case "DATETIME":
                        ws.Cells[2, iCol + 1, dt.Rows.Count + 2, iCol + 1].Style.Numberformat.Format = "yyyy/MM/dd hh:mm:ss";
                        break;

                    default:
                        break;
                }
            }

            ws.Cells[3, 1].LoadFromDataTable(dt, false); //要不要印header

            ws.Cells[ws.Dimension.Address].AutoFitColumns();  //自動調整column size
            //畫線
            ws.Cells[2, 1, dt.Rows.Count + 2, dt.Columns.Count].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[2, 1, dt.Rows.Count + 2, dt.Columns.Count].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ws.Cells[2, 1, dt.Rows.Count + 2, dt.Columns.Count].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            ws.Cells[2, 1, dt.Rows.Count + 2, dt.Columns.Count].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            //讓第二欄有換行符號的自動換行(服務據點更新縣市匯出的第二欄需換行) by Leon
            ws.Column(2).Style.WrapText = true;
        }
        
        public static string ExportExcel(string FileType,string FilePath, DataTable dt, string title)
        {
            ExcelPackage package = new ExcelPackage();

            //調整Excel樣式
            DateTableExport(package, dt, title);

            string FileName = FileType + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            string destFile = FilePath + FileName;
            FileStream fs = new FileStream(destFile, FileMode.Create, FileAccess.ReadWrite);
            package.SaveAs(fs);
            fs.Close();
            package = null;
            return destFile;
        }

        public static DataSet Excel2DataSet(FileInfo file)
        {
            // 設置LicenseContext為NonCommercial以跳過認證(個人用)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // 使用EPPlus打開Excel檔案
            ExcelPackage package = new ExcelPackage(file);

            DataSet ds = new DataSet();

            // 逐個將工作表寫入資料集 workbook是excel檔，worksheet是分頁
            foreach (ExcelWorksheet worksheet in package.Workbook.Worksheets)
            {
                DataTable dt = new DataTable(worksheet.Name);

                // 添加DataTable的列，列名可以根據需要更改
                foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                {
                    dt.Columns.Add(firstRowCell.Text);
                }

                // 逐行將 Excel 數據添加到 DataTable
                for (int rowNumber = 2; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
                {
                    //取出一行資料
                    var worksheetRow = worksheet.Cells[rowNumber, 1, rowNumber, worksheet.Dimension.End.Column];

                    var row = dt.Rows.Add();

                    //將欄位逐個放入暫時的DataRow
                    foreach (var cell in worksheetRow)
                    {
                        row[cell.Start.Column - 1] = cell.Value;
                    }
                }

                #region 修改datatable 資料

                dt.Columns.Add(new DataColumn("JoiningDate2", typeof(DateTime)));

                // 將 OriginalColumn 轉換為 DateTime，並將结果複製给 TargetColumn
                foreach (DataRow row in dt.Rows)
                {
                    // 获取原始列的值
                    string originalValue = row["JoiningDate"].ToString();

                    // 尝试将字符串转换为 DateTime
                    if (DateTime.TryParse(originalValue, out DateTime result))
                    {
                        // 将转换后的 DateTime 存储到目标列
                        row["JoiningDate2"] = result;
                    }
                    else
                    {
                        // 处理无法转换的情况
                        Console.WriteLine($"Unable to convert value: {originalValue}");
                    }
                }

                dt.Columns.RemoveAt(dt.Columns.Count-2);

                dt.Columns["JoiningDate2"].ColumnName = "JoiningDate";

                #endregion

                //新增至DataTable
                ds.Tables.Add(dt);
            }

            //回傳DataSet
            return ds;
        }

        public static DataSet Excel2DataSet2(FileInfo file)
        {
            // 設置LicenseContext為NonCommercial以跳過認證(個人用)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // 使用EPPlus打開Excel檔案
            ExcelPackage package = new ExcelPackage(file);

            DataSet ds = new DataSet();

            // 逐個將工作表寫入資料集 workbook是excel檔，worksheet是分頁
            foreach (ExcelWorksheet worksheet in package.Workbook.Worksheets)
            {
                DataTable dt = new DataTable(worksheet.Name);

                // 添加DataTable的列，列名可以根據需要更改
                foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                {
                    dt.Columns.Add(firstRowCell.Text);
                }

                // 逐行將 Excel 數據添加到 DataTable
                for (int rowNumber = 2; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
                {
                    //取出一行資料
                    var worksheetRow = worksheet.Cells[rowNumber, 1, rowNumber, worksheet.Dimension.End.Column];

                    var row = dt.Rows.Add();

                    //將欄位逐個放入暫時的DataRow
                    foreach (var cell in worksheetRow)
                    {
                        row[cell.Start.Column - 1] = cell.Value;
                    }
                }

                #region 修改datatable 資料

                dt.Columns.Add(new DataColumn("CreateDate2", typeof(DateTime)));

                // 將 OriginalColumn 轉換為 DateTime，並將结果複製给 TargetColumn
                foreach (DataRow row in dt.Rows)
                {
                    // 获取原始列的值
                    string originalValue = row["CreateDate"].ToString();

                    // 尝试将字符串转换为 DateTime
                    if (DateTime.TryParse(originalValue, out DateTime result))
                    {
                        // 将转换后的 DateTime 存储到目标列
                        row["CreateDate2"] = result;
                    }
                    else
                    {
                        // 处理无法转换的情况
                        Console.WriteLine($"Unable to convert value: {originalValue}");
                    }
                }

                dt.Columns.RemoveAt(dt.Columns.Count - 2);

                dt.Columns["CreateDate2"].ColumnName = "CreateDate";

                #endregion

                //新增至DataTable
                ds.Tables.Add(dt);
            }

            //回傳DataSet
            return ds;
        }
    }
}
