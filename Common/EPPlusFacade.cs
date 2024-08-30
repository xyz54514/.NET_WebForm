using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml.Style;

namespace Common
{
    public class EPPlusFacade
    {
        public static void ExportDataTable(DataTable dtData, out Byte[] excelFileContent)
        {
            // default 
            excelFileContent = null;

            // take sheet name
            string sheetName = dtData.TableName;
            if (String.IsNullOrWhiteSpace(sheetName))
                sheetName = "sheet1";

            //# 新建 Excel 
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet ws = ep.Workbook.Worksheets.Add(sheetName); // 1-base

            // header
            for (int i = 0; i < dtData.Columns.Count; i++)
            {
                DataColumn col = dtData.Columns[i];
                ws.Cells[1, i + 1].Value = col.ColumnName;
            }

            //# copy data row to excel
            int rowIdx = 2;
            foreach (DataRow dr in dtData.Rows)
            {
                for (int i = 0; i < dtData.Columns.Count; i++)
                {
                    ws.Cells[rowIdx, i + 1].Value = Convert.ToString(dr[i]);
                }

                // next row
                rowIdx++;
            }

            // return
            excelFileContent = ep.GetAsByteArray();
        }

        public static void ExportDataTable(DataTable dtData, FileInfo excelFile)
        {
            // take sheet name
            string sheetName = dtData.TableName;
            if (String.IsNullOrWhiteSpace(sheetName))
                sheetName = "sheet1";

            //# 新建 Excel 
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet ws = ep.Workbook.Worksheets.Add(sheetName); // 1-base

            //# header row
            for (int i = 0; i < dtData.Columns.Count; i++)
            {
                DataColumn col = dtData.Columns[i];
                ws.Cells[1, i + 1].Value = col.ColumnName;
            }

            //# copy data row to excel
            int rowIdx = 2;
            foreach (DataRow dr in dtData.Rows)
            {
                for (int i = 0; i < dtData.Columns.Count; i++)
                {
                    ws.Cells[rowIdx, i + 1].Value = Convert.ToString(dr[i]);
                }

                // next row
                rowIdx++;
            }

            //# done
            ep.SaveAs(excelFile);
        }

        public static void ExportDataSet(DataSet dtDatas, FileInfo excelFile)
        {
            //# 新建 Excel 
            ExcelPackage ep = new ExcelPackage();

            int sheetIdx = 1; // 1-base
            foreach (DataTable dtData in dtDatas.Tables)
            {
                // take sheet name
                string sheetName = dtData.TableName;
                if (String.IsNullOrWhiteSpace(sheetName))
                    sheetName = string.Format("sheet{0}", sheetIdx);

                // new create sheet
                ExcelWorksheet ws = ep.Workbook.Worksheets.Add(sheetName); // 1-base

                //# header row
                for (int i = 0; i < dtData.Columns.Count; i++)
                {
                    DataColumn col = dtData.Columns[i];
                    ws.Cells[1, i + 1].Value = col.ColumnName;
                }

                //# copy data row to excel
                int rowIdx = 2;
                foreach (DataRow dr in dtData.Rows)
                {
                    for (int i = 0; i < dtData.Columns.Count; i++)
                    {
                        ws.Cells[rowIdx, i + 1].Value = Convert.ToString(dr[i]);
                    }

                    // next row
                    rowIdx++;
                }

                // next sheet
                sheetIdx++;
            }

            //# done
            ep.SaveAs(excelFile);
        }

        public static void ExportDataTable2(DataTable dtData, FileInfo excelFile)
        {
            // take sheet name
            string sheetName = dtData.TableName;
            if (String.IsNullOrWhiteSpace(sheetName))
                sheetName = "sheet1";

            //# 新建 Excel 
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet ws = ep.Workbook.Worksheets.Add(sheetName); // 1-base

            //# header row
            for (int i = 0; i < dtData.Columns.Count; i++)
            {
                DataColumn col = dtData.Columns[i];
                ws.Cells[1, i + 1].Value = col.ColumnName;
            }

            //# copy data row to excel
            int rowIdx = 2;
            foreach (DataRow dr in dtData.Rows)
            {
                for (int i = 0; i < dtData.Columns.Count; i++)
                {
                    // 數字右排
                    if (dr[i] is decimal || dr[i] is int || dr[i] is long)
                    {
                        var cell = ws.Cells[rowIdx, i + 1];
                        cell.Value = Convert.ToDecimal(dr[i]);
                        cell.Style.Numberformat.Format = "#,##0";
                        cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    }
                    else if (dr[i] is DateTime)
                    {
                        var cell = ws.Cells[rowIdx, i + 1];
                        cell.Value = Convert.ToDateTime(dr[i]);
                        cell.Style.Numberformat.Format = "yyyy\\/MM\\/dd HH:mm:ss";
                    }
                    else
                    {
                        // default : as string
                        ws.Cells[rowIdx, i + 1].Value = Convert.ToString(dr[i]);
                    }
                }

                // next row
                rowIdx++;
            }

            // auto-resize all cells
            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            //# done
            ep.SaveAs(excelFile);
        }

        //開啟Excel
        public ExcelPackage OpenExcel()
        {
            ExcelPackage ep = new ExcelPackage();

            return ep;
        }

        public ExcelPackage OpenExcel(FileInfo ExcelInfo)
        {
            ExcelPackage ep = new ExcelPackage(ExcelInfo);

            return ep;
        }

        //新增工作業
        public ExcelWorksheet AddWorkSheet(ExcelPackage ep, string SheetName)
        {
            ExcelWorksheet ws;

            if (!string.IsNullOrEmpty(SheetName))
            {
                ws = ep.Workbook.Worksheets.Add(SheetName);
            }
            else
            {
                ws = ep.Workbook.Worksheets.Add("Sheet1");
            }

            return ws;
        }

        //跳至工作業
        public ExcelWorksheet SetWorkSheet(ExcelPackage ep, int SheetIndex)
        {
            ExcelWorksheet ws = ep.Workbook.Worksheets[SheetIndex];

            return ws;
        }

        //複製Sheet
        public ExcelPackage CopySheet(ExcelPackage ep, string SampleSheetName, string NewSheetName)
        {
            ep.Workbook.Worksheets.Copy(SampleSheetName, NewSheetName);

            return ep;
        }

        //刪除Sheet
        public ExcelPackage DeleteSheet(ExcelPackage ep, int SheetIndex)
        {
            ep.Workbook.Worksheets.Delete(SheetIndex);

            return ep;
        }

        //新增Row
        public ExcelWorksheet InsertRow(ExcelWorksheet ws, int DestRow, int CopyQty)
        {
            ws.InsertRow(DestRow, CopyQty);

            return ws;
        }

        //複製Row
        public ExcelWorksheet CopyRow(ExcelWorksheet ws, int SourceRow, int DestRow, int CopyQty, bool CopyStartRowStyle = true)
        {
            if (CopyStartRowStyle)
                ws.InsertRow(DestRow, CopyQty, SourceRow);
            else
                ws.InsertRow(DestRow, CopyQty);

            return ws;
        }

        //複製Row(範圍)
        public ExcelWorksheet CopyRow_Range(ExcelWorksheet ws, int StartRow, int StartCol, int EndRow, int EndCol, int DestStartRow, int DestStartCol, int DestEndRow, int DestEndCol)
        {
            ws.Cells[StartRow, StartCol, EndRow, EndCol].Copy(ws.Cells[DestStartRow, DestStartCol, DestEndRow, DestEndCol]);

            return ws;
        }

        //刪除Row
        public ExcelWorksheet DeleteRow(ExcelWorksheet ws, int DestRow)
        {
            ws.DeleteRow(DestRow);

            return ws;
        }

        //複製Cell
        public ExcelWorksheet CopyCell(ExcelWorksheet ws, int CopyRowIndex, int CopyCellStart, int CopyCellEnd, int DestRowIndex, int DestCellStart, int DestCellEnd)
        {
            ws.Cells[CopyRowIndex, CopyCellStart, CopyRowIndex, CopyCellEnd].Copy(ws.Cells[DestRowIndex, DestCellStart, DestRowIndex, DestCellEnd]);

            return ws;
        }

        //填入欄位值
        public ExcelWorksheet SetValue(ExcelWorksheet ws, int Cell_Row, int Cell_Column, object CellValue)
        {
            string drValue = CellValue.ToString();

            //Cell值型態判斷
            switch (CellValue.GetType().ToString())
            {
                case "System.String"://字串類型
                    ws.Cells[Cell_Row, Cell_Column].Value = drValue;
                    break;

                case "System.DateTime"://日期類型
                    ws.Cells[Cell_Row, Cell_Column].Value = drValue;
                    break;

                case "System.Boolean"://布林型
                    bool boolValue = false;
                    bool.TryParse(drValue, out boolValue);
                    ws.Cells[Cell_Row, Cell_Column].Value = boolValue;
                    break;

                case "System.Int16"://整數型
                case "System.Int32":
                case "System.Byte":
                    int intValue = 0;
                    int.TryParse(drValue, out intValue);
                    ws.Cells[Cell_Row, Cell_Column].Value = intValue;
                    break;

                case "System.Int64":
                    Int64 int64Value = 0;
                    Int64.TryParse(drValue, out int64Value);
                    ws.Cells[Cell_Row, Cell_Column].Value = int64Value;
                    break;

                case "System.Decimal"://浮點型
                case "System.Double":
                    double doubValue = 0;
                    double.TryParse(drValue, out doubValue);
                    ws.Cells[Cell_Row, Cell_Column].Value = doubValue;
                    break;

                case "System.DBNull"://空值處理
                    ws.Cells[Cell_Row, Cell_Column].Value = "Null";
                    break;

                default:
                    ws.Cells[Cell_Row, Cell_Column].Value = "";
                    break;
            }

            return ws;
        }

        //欄位框線
        public ExcelWorksheet setBorder(ExcelWorksheet ws, int Cell_Row, int Cell_Column, bool Top, bool Bottom, bool Left, bool Right)
        {
            if (Top)
            {
                ws.Cells[Cell_Row, Cell_Column].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            }

            if (Bottom)
            {
                ws.Cells[Cell_Row, Cell_Column].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }

            if (Left)
            {
                ws.Cells[Cell_Row, Cell_Column].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            }

            if (Right)
            {
                ws.Cells[Cell_Row, Cell_Column].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            }

            return ws;
        }

        //欄位框線(自訂)
        public ExcelWorksheet setBorder(ExcelWorksheet ws, int Cell_Row, int Cell_Column, ExcelBorderStyle TopStyle, ExcelBorderStyle BottomStyle, ExcelBorderStyle LeftStyle, ExcelBorderStyle RightStyle)
        {
            ws.Cells[Cell_Row, Cell_Column].Style.Border.Top.Style = TopStyle;
            ws.Cells[Cell_Row, Cell_Column].Style.Border.Bottom.Style = BottomStyle;
            ws.Cells[Cell_Row, Cell_Column].Style.Border.Left.Style = LeftStyle;
            ws.Cells[Cell_Row, Cell_Column].Style.Border.Right.Style = RightStyle;

            return ws;
        }

        //欄位框線(範圍)
        public ExcelWorksheet setRange_Border(ExcelWorksheet ws, int StartRow, int StartCol, int EndRow, int EndCol, string BorderSide, ExcelBorderStyle Style)
        {
            var CellArray = ws.Cells[StartRow, StartCol, EndRow, EndCol];

            switch (BorderSide.ToUpper())
            {
                case "TOP":
                    CellArray.Style.Border.Top.Style = Style;
                    break;

                case "BOTTOM":
                    CellArray.Style.Border.Bottom.Style = Style;
                    break;

                case "LEFT":
                    CellArray.Style.Border.Left.Style = Style;
                    break;

                case "RIGHT":
                    CellArray.Style.Border.Right.Style = Style;
                    break;
            }

            return ws;
        }

        //欄位框線(上)
        public ExcelWorksheet setBorder_Top(ExcelWorksheet ws, int Cell_Row, int Cell_Column, ExcelBorderStyle Style)
        {
            ws.Cells[Cell_Row, Cell_Column].Style.Border.Top.Style = Style;

            return ws;
        }

        //欄位框線(下)
        public ExcelWorksheet setBorder_Bottom(ExcelWorksheet ws, int Cell_Row, int Cell_Column, ExcelBorderStyle Style)
        {
            ws.Cells[Cell_Row, Cell_Column].Style.Border.Bottom.Style = Style;

            return ws;
        }

        //欄位框線(左)
        public ExcelWorksheet setBorder_Left(ExcelWorksheet ws, int Cell_Row, int Cell_Column, ExcelBorderStyle Style)
        {
            ws.Cells[Cell_Row, Cell_Column].Style.Border.Left.Style = Style;

            return ws;
        }

        //欄位框線(右)
        public ExcelWorksheet setBorder_Right(ExcelWorksheet ws, int Cell_Row, int Cell_Column, ExcelBorderStyle Style)
        {
            ws.Cells[Cell_Row, Cell_Column].Style.Border.Right.Style = Style;

            return ws;
        }

        //設定欄位字型大小
        public ExcelWorksheet setCellFont(ExcelWorksheet ws, int Cell_Row, int Cell_Column, string FontName, int FontSize = 12)
        {
            ws.Cells[Cell_Row, Cell_Column].Style.Font.Name = FontName;
            ws.Cells[Cell_Row, Cell_Column].Style.Font.Size = FontSize;

            return ws;
        }

        public ExcelWorksheet setRowHeight(ExcelWorksheet ws, int RowIndex, double RowHeight)
        {
            ws.Row(RowIndex).Height = RowHeight;

            return ws;
        }

        //儲存Excel
        public void SaveExcel(ExcelPackage ep, FileInfo SaveInfo)
        {
            ep.SaveAs(SaveInfo);
        }
    }
}
