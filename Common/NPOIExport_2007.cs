using NPOI.HSSF.UserModel;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class NPOIExport_2007
    {
        public enum FileType { xls, xlsx };

        #region static method
        /// <summary>
        /// 將Excel行值轉換成Index A=>0,B=>1,AA=>26
        /// </summary>
        /// <param name="Col Value"> A </param>
        public static int Get_EXCEL_COLUMNINDEX(string Col)
        {
            //先全部轉成大寫
            Char[] sCode = Col.ToUpper().ToCharArray();

            int iResult = 0;

            for (int i = 0; i < Col.Length; i++)
            {
                int iPow = (Col.Length - 1) - i;
                //ASCII : A:65  @:64            
                iResult += sCode[i].CompareTo('@') * Convert.ToInt32(Math.Pow(26, iPow));

            }
            //ColumnIndex從0開始，故值須減1
            iResult = iResult - 1;
            return iResult;
        }
        #endregion

        IWorkbook _Workbook;
        ISheet _ActiveSheet;

        public IWorkbook Workbook
        {
            get { return _Workbook; }
        }

        public ISheet ActiveSheet
        {
            get { return _ActiveSheet; }
        }

        public int ActiveSheetIndex
        {
            get { return _Workbook.GetSheetIndex(_ActiveSheet); }
        }

        private const int _AutoSizeColumn_MinBytes = 5; //5個英文字
        private const int _AutoSizeColumn_MaxBytes = 50; //50個英文字
        string _DefaultFontName = "新細明體"; //"Arial";
        short _DefaultFontSize = 11;
        private int _CharWidth = 370; //1個字元在NPOI中的大概寬度
        private int _ColumnKeepSpace = 500; //欄位寬度2邊多出留白,較容易觀看內容

        FileType _xlFileType = FileType.xls;
        IDataFormat _IDataFormat;
        IFont _Font_Header;
        IFont _Font_Detail;
        ICellStyle _HeadStyle;
        ICellStyle _DetailStyle;
        ICellStyle CellStyle_OnlyDate;
        ICellStyle CellStyle_DateTime;
        ICellStyle CellStyle_Text;
        ICellStyle CellStyle_Integer;
        ICellStyle CellStyle_Decimal; //0.0#######

        /// <summary>
        /// Create a empty workbook contains a sheet.
        /// </summary>
        /// <param name="fileType"></param>
        public NPOIExport_2007(FileType fileType)
        {
            _NPOIExport_Create(fileType, "");
        }

        /// <summary>
        /// Create a workbook from template file. Template file must exist.
        /// Template可以使用xls/xlsx.
        /// </summary>
        /// <param name="templatePath"></param>
        public NPOIExport_2007(string templatePath)
        {
            if (string.IsNullOrEmpty(templatePath) || templatePath.Trim() == null)
                throw new Exception("Template path is null or empty.");

            if (!File.Exists(templatePath))
                throw new Exception("Template file is not existent: " + templatePath);

            if (Path.GetExtension(templatePath).ToLower().Equals(".xlsx"))
                _NPOIExport_Create(FileType.xlsx, templatePath);
            else if (Path.GetExtension(templatePath).ToLower().Equals(".xls"))
                _NPOIExport_Create(FileType.xls, templatePath);
            else
                throw new Exception("Wrong template file existent name.");

        }

        /// <summary>
        /// 請不要再使用這個 Method. Create a workbook from template file.
        /// </summary>
        /// <param name="fileType"></param>
        /// <param name="templatePath"></param>
        public NPOIExport_2007(FileType fileType, string templatePath)
        {
            if (fileType == FileType.xls)
            {
                if (!Path.GetExtension(templatePath).ToLower().Equals(".xls"))
                    throw new Exception("FileType and template file existent name is different.");
            }
            else if (fileType == FileType.xlsx)
            {
                if (!Path.GetExtension(templatePath).ToLower().Equals(".xlsx"))
                    throw new Exception("FileType and template file existent name is different.");
            }
            else
                throw new Exception("Wrong file type.");

            _NPOIExport_Create(fileType, templatePath);
        }

        private void _NPOIExport_Create(FileType fileType, string templatePath)
        {
            _xlFileType = fileType;

            if (_xlFileType == FileType.xls)
            {
                if (string.IsNullOrEmpty(templatePath))
                {
                    _Workbook = new HSSFWorkbook();
                    _Workbook.CreateSheet("Sheet1");
                }
                else
                {
                    FileStream file = new FileStream(templatePath, FileMode.Open, FileAccess.Read);
                    _Workbook = new HSSFWorkbook(file);
                }
            }
            else //FileType.xlsx
            {
                if (string.IsNullOrEmpty(templatePath))
                {
                    _Workbook = new XSSFWorkbook();
                    _Workbook.CreateSheet("Sheet1");
                }
                else
                {
                    FileStream file = new FileStream(templatePath, FileMode.Open, FileAccess.Read);
                    _Workbook = new XSSFWorkbook(file);
                }
            }

            _Workbook.SetActiveSheet(0);
            _ActiveSheet = _Workbook.GetSheetAt(0);

            _IDataFormat = _Workbook.CreateDataFormat();

            string key_val = _GetAppConfig("ExcelDefaultFontName");
            if (!string.IsNullOrEmpty(key_val) && key_val.Trim() != "")
                _DefaultFontName = key_val.Trim();

            key_val = _GetAppConfig("ExcelDefaultFontSize");
            if (!string.IsNullOrEmpty(key_val) && key_val.Trim() != "")
            {
                short.TryParse(key_val, out _DefaultFontSize);

                if (_DefaultFontSize > 20)
                    _DefaultFontSize = 20;
                else if (_DefaultFontSize < 6)
                    _DefaultFontSize = 6;
            }

            key_val = _GetAppConfig("ExcelCharWidth");
            if (!string.IsNullOrEmpty(key_val) && key_val.Trim() != "")
            {
                int.TryParse(key_val, out _CharWidth);

                if (_CharWidth > 1000)
                    _CharWidth = 1000;
                else if (_CharWidth < 100)
                    _CharWidth = 100;
            }

            #region 設定使用字型/格式
            _Font_Header = _Workbook.CreateFont();
            _Font_Header.IsBold = true; //Boldweight = 700;
            _Font_Header.FontHeightInPoints = _DefaultFontSize;
            _Font_Header.FontName = _DefaultFontName;

            _Font_Detail = _Workbook.CreateFont();
            _Font_Detail.FontHeightInPoints = _DefaultFontSize;
            _Font_Detail.FontName = _DefaultFontName;

            //設定Cell使用格式
            //DefaultCellStyle = _Workbook.CreateCellStyle();

            _HeadStyle = _Workbook.CreateCellStyle();
            _HeadStyle.Alignment = HorizontalAlignment.Center;
            _HeadStyle.SetFont(_Font_Header);

            _DetailStyle = _Workbook.CreateCellStyle();
            _DetailStyle.SetFont(_Font_Detail);

            CellStyle_OnlyDate = _Workbook.CreateCellStyle();
            CellStyle_OnlyDate.DataFormat = _IDataFormat.GetFormat("yyyy/mm/dd");
            CellStyle_OnlyDate.SetFont(_Font_Detail);

            CellStyle_DateTime = _Workbook.CreateCellStyle();
            CellStyle_DateTime.DataFormat = _IDataFormat.GetFormat("yyyy/mm/dd hh:mm");
            CellStyle_DateTime.SetFont(_Font_Detail);

            CellStyle_Text = _Workbook.CreateCellStyle();
            CellStyle_Text.DataFormat = _IDataFormat.GetFormat("text"); //(short)BuiltinFormats.GetBuiltinFormat("text"); // 
            CellStyle_Text.SetFont(_Font_Detail);
            CellStyle_Integer = _Workbook.CreateCellStyle();
            CellStyle_Integer.DataFormat = _IDataFormat.GetFormat("0");
            CellStyle_Integer.SetFont(_Font_Detail);

            CellStyle_Decimal = _Workbook.CreateCellStyle();
            CellStyle_Decimal.DataFormat = _IDataFormat.GetFormat("0.0#######");
            CellStyle_Decimal.SetFont(_Font_Detail);
            #endregion 設定使用字型/格式            
        }

        public IFont DefaultHeaderFont
        {
            get { return _Font_Header; }
        }

        public IFont DefaultDetailFont
        {
            get { return _Font_Detail; }
        }

        public ICellStyle DefaultHeaderStyle
        {
            get { return _HeadStyle; }
        }

        /// <summary>
        /// 設定Active Sheet,並將資料填入Sheet中
        /// </summary>
        /// <param name="dtInput"></param>
        /// <param name="sheetIndex">設定Active Sheet(從0開始)</param>
        public void DataTableToSheet(DataTable dtInput, int sheetIndex)
        {
            if (_Workbook.NumberOfSheets < sheetIndex + 1)
                throw new Exception("sheetIndex is not existed.");

            _Workbook.SetActiveSheet(sheetIndex);
            _ActiveSheet = _Workbook.GetSheetAt(sheetIndex);

            DataTableToActiveSheet(dtInput);
        }

        public void DataTableToActiveSheet(DataTable dtInput)
        {
            string SheetMainName = _ActiveSheet.SheetName; ;
            int SheetCount = 0;
            IRow headerRow;

            #region 列頭及樣式
            headerRow = _ActiveSheet.CreateRow(0);
            foreach (DataColumn column in dtInput.Columns)
            {
                headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                headerRow.GetCell(column.Ordinal).CellStyle = _HeadStyle;
            }
            #endregion

            int rowIndex = 1;
            foreach (DataRow row in dtInput.Rows)
            {
                #region 新建表，填充表頭，填充列頭，樣式
                if (_xlFileType == FileType.xls && rowIndex == 65536)
                {
                    //目前的Sheet自動調整欄寬,較容易觀看內容.
                    AutoSizeColumn2(dtInput.Columns.Count);

                    SheetCount++;
                    //建立新Sheet
                    _ActiveSheet = _Workbook.CreateSheet(SheetMainName + SheetCount.ToString());

                    #region 列頭及樣式
                    {
                        headerRow = _ActiveSheet.CreateRow(0);
                        foreach (DataColumn column in dtInput.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            headerRow.GetCell(column.Ordinal).CellStyle = _HeadStyle;
                        }
                    }
                    #endregion

                    rowIndex = 1;
                }
                #endregion

                #region 填充內容
                IRow dataRow = _ActiveSheet.CreateRow(rowIndex);

                foreach (DataColumn column in dtInput.Columns)
                {
                    //一律用 CreateCell
                    ICell newCell = dataRow.CreateCell(column.Ordinal);
                    //Cell 使用 Default Style 即可
                    newCell.CellStyle = _DetailStyle;

                    if (row[column] == null)
                    {
                        newCell.SetCellValue("");
                        continue;
                    }

                    string drValue = row[column].ToString();

                    //修改本段程式時,請同時參考 SetValue.
                    //Cell.CellStyle 要指定到新的 Style, 不可直接更改 Cell.CellStyle 中的屬性, 例如 DataFormat
                    //每個新 Cell 的 CellStyle, 會指定到共用的預設 CellStyle.
                    switch (row[column].GetType().ToString())
                    {
                        case "System.String"://字串類型
                            newCell.SetCellValue(drValue);
                            newCell.CellStyle = CellStyle_Text;
                            break;
                        case "System.DateTime"://日期類型
                            DateTime dt_tmp = Convert.ToDateTime(row[column]);
                            newCell.SetCellValue(dt_tmp);
                            if (dt_tmp.Date == dt_tmp)
                            {
                                //只有日期,沒有時間
                                newCell.CellStyle = CellStyle_OnlyDate;
                            }
                            else
                            {
                                newCell.CellStyle = CellStyle_DateTime;
                            }
                            break;
                        case "System.Boolean"://布林型
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;
                        case "System.Int16"://整數型
                        case "System.Int32":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            newCell.CellStyle = CellStyle_Integer;
                            break;
                        case "System.Int64":
                            Int64 int64V = 0;
                            Int64.TryParse(drValue, out int64V);
                            newCell.SetCellValue(int64V);
                            newCell.CellStyle = CellStyle_Integer;
                            break;
                        case "System.Decimal"://浮點型
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);

                            double doubV2 = Math.Ceiling(doubV);
                            if (doubV2 == doubV)
                                newCell.CellStyle = CellStyle_Integer;
                            else
                                newCell.CellStyle = CellStyle_Decimal;
                            break;
                        case "System.DBNull"://空值處理
                            newCell.SetCellValue("");
                            break;
                        default:
                            newCell.SetCellValue(drValue);
                            break;
                    }

                }
                #endregion

                rowIndex++;
            }

            //目前的Sheet自動調整欄寬,較容易觀看內容.
            AutoSizeColumn2(dtInput.Columns.Count);
        }

        /// <summary>
        /// 填值
        /// </summary>
        /// <param name="intRowIndex">Row</param>
        /// <param name="cellIndex">Cell</param>
        /// <param name="value">Value</param>
        public void SetValue(int rowIndex, int cellIndex, object value)
        {
            //修改本段程式時,請同時參考 DataTableToActiveSheet
            //可能使用 Template, 所以新 Cell 才會設定 Style.
            //Cell.CellStyle 要指定到新的 Style, 不可直接更改 Cell.CellStyle 中的屬性, 例如 DataFormat
            //每個新 Cell 的 CellStyle, 會指定到共用的預設 CellStyle.

            IRow iRow = _ActiveSheet.GetRow(rowIndex);
            if (iRow == null)
                iRow = (IRow)_ActiveSheet.CreateRow(rowIndex);

            bool create_new_cell = false;
            ICell newCell = iRow.GetCell(cellIndex);
            if (newCell == null)
            {
                create_new_cell = true;
                newCell = iRow.CreateCell(cellIndex);
                newCell.CellStyle = _DetailStyle; //Bon add
            }

            if (value == null)
            {
                newCell.SetCellValue("");
                return;
            }

            string drValue = Convert.ToString(value);

            switch (value.GetType().ToString())
            {
                case "System.String"://字串類型
                    newCell.SetCellValue(drValue);
                    if (create_new_cell)
                    {
                        //新的Cell, 可設定 CellStyle = Text
                        newCell.CellStyle = CellStyle_Text; //Bon add
                    }
                    break;
                case "System.DateTime"://日期類型
                    //newCell.SetCellValue(drValue.ToString()); //會有中文字
                    //Bon add
                    DateTime dt_tmp = Convert.ToDateTime(value);
                    newCell.SetCellValue(dt_tmp);
                    if (create_new_cell || newCell.CellStyle.DataFormat == 0)
                    {
                        //DataFormat = 0 : General, 日期無法正確顯示, 要設定格式
                        //新的Cell, 需設定 CellStyle
                        if (dt_tmp.Date == dt_tmp)
                        {
                            //只有日期,沒有時間
                            newCell.CellStyle = CellStyle_OnlyDate;
                        }
                        else
                        {
                            newCell.CellStyle = CellStyle_DateTime;
                        }
                    }
                    break;
                case "System.Boolean"://布林型
                    bool boolV = false;
                    bool.TryParse(drValue, out boolV);
                    newCell.SetCellValue(boolV);
                    break;
                case "System.Int16"://整數型
                case "System.Int32":
                case "System.Byte":
                    int intV = 0;
                    int.TryParse(drValue, out intV);
                    newCell.SetCellValue(intV);
                    if (create_new_cell)
                    {
                        //新的Cell
                        newCell.CellStyle = CellStyle_Integer; //Bon add
                    }
                    break;
                case "System.Int64":
                    Int64 int64V = 0;
                    Int64.TryParse(drValue, out int64V);
                    newCell.SetCellValue(int64V);
                    if (create_new_cell)
                    {
                        //新的Cell
                        newCell.CellStyle = CellStyle_Integer; //Bon add
                    }
                    break;
                case "System.Decimal"://浮點型
                case "System.Double":
                    double doubV = 0;
                    double.TryParse(drValue, out doubV);
                    newCell.SetCellValue(doubV);
                    if (create_new_cell)
                    {
                        //新的Cell
                        double doubV2 = Math.Ceiling(doubV);
                        if (doubV2 == doubV)
                            newCell.CellStyle = CellStyle_Integer;
                        else
                            newCell.CellStyle = CellStyle_Decimal;
                    }
                    break;
                case "System.DBNull"://空值處理
                    newCell.SetCellValue("");
                    break;
                default:
                    newCell.SetCellValue(drValue);
                    break;
            }
        }

        /// <summary>
        /// Return Cell
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="cellIndex"></param>
        /// <returns></returns>
        public ICell GetCell(int rowIndex, int cellIndex)
        {
            IRow iRow = _ActiveSheet.GetRow(rowIndex);
            if (iRow == null)
                return null;
            else
                return iRow.GetCell(cellIndex);
        }

        public string GetCellValue(int rowIndex, int cellIndex)
        {
            IRow iRow = _ActiveSheet.GetRow(rowIndex);
            if (iRow == null)
                return null;
            else
            {
                ICell cell = iRow.GetCell(cellIndex);
                if (cell == null)
                    return null;
                else
                    return cell.ToString();
            }
        }

        public int GetRow_NumberOfCells(int rowIndex)
        {
            IRow iRow = _ActiveSheet.GetRow(rowIndex);
            if (iRow == null)
                return 0;
            else
                return iRow.PhysicalNumberOfCells;
        }

        /// <summary>
        /// 新增新的Sheet,並設為Active Sheet.
        /// </summary>
        /// <param name="sheetName"></param>
        /// <returns>新的 Sheet 的 Index(從0開始計算)</returns>
        public int NewSheet(string sheetName)
        {
            //檢查傳入的名稱是否重複
            int chk_no = 0;
            if (string.IsNullOrEmpty(sheetName) || sheetName.Trim() == "")
            {
                sheetName = "Sheet" + (_Workbook.NumberOfSheets + 1).ToString();
                while (IndexOfSheet(sheetName) >= 0)
                {
                    chk_no++;
                    sheetName += chk_no.ToString();
                }
            }
            else
            {
                sheetName = sheetName.Trim();
                while (IndexOfSheet(sheetName) >= 0)
                {
                    chk_no++;
                    sheetName += chk_no.ToString();
                }
            }

            _Workbook.CreateSheet(sheetName);

            int sheet_idx = _Workbook.NumberOfSheets - 1;
            _Workbook.SetActiveSheet(sheet_idx);
            _ActiveSheet = _Workbook.GetSheetAt(sheet_idx);

            return sheet_idx;
        }

        /// <summary>
        /// 指定目前要操作的sheet,sheet必需要存在
        /// </summary>
        /// <param name="sheetIndex">從0開始計算</param>
        public void SetActiveSheet(int sheetIndex)
        {
            if (sheetIndex >= _Workbook.NumberOfSheets)
                throw new Exception("Wrong sheetIndex(" + sheetIndex + ")");

            _Workbook.SetActiveSheet(sheetIndex);
            _ActiveSheet = _Workbook.GetSheetAt(sheetIndex);
        }

        /// <summary>
        /// 指定目前要操作的sheet,sheet必需要存在
        /// </summary>
        /// <param name="sheetName"></param>
        public void SetActiveSheet(string sheetName)
        {
            for (int i = 0; i < _Workbook.NumberOfSheets; i++)
            {
                if (_Workbook.GetSheetName(i).ToLower().Trim() == sheetName.Trim().ToLower())
                {
                    _Workbook.SetActiveSheet(i);
                    _ActiveSheet = _Workbook.GetSheetAt(i);
                    return;
                }
            }

            throw new Exception("Wrong sheetName(" + sheetName + ")");
        }

        /// <summary>
        /// 回傳 Sheet Name 的 Index(從0開鈶),不在在時回傳 -1.
        /// </summary>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public int IndexOfSheet(string sheetName)
        {
            for (int i = 0; i < _Workbook.NumberOfSheets; i++)
            {
                if (_Workbook.GetSheetName(i).ToLower().Trim() == sheetName.Trim().ToLower())
                {
                    return i;
                }
            }

            return -1;
        }

        //調整整個Sheet的column寬度
        public void SetALLColumnWidth(int columnWidth)
        {
            //先取得最後一個有值的CEll
            int CellCount = 1;
            while (_ActiveSheet.GetRow(1).GetCell(CellCount) != null)
            {
                CellCount = CellCount + 1;
            }

            for (int i = 0; i <= CellCount - 1; i++)
            {
                _ActiveSheet.SetColumnWidth(i, columnWidth * 256);
            }
        }

        //調整整個Sheet的單一column寬度
        public void SetColumnWidth(int columnIndex, int columnWidth)
        {
            _ActiveSheet.SetColumnWidth(columnIndex, columnWidth * 256);
        }

        /// <summary>
        /// 儲存至Excel
        /// </summary>
        /// <param name="filePath"></param>
        public void SaveExcel(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create, System.IO.FileAccess.Write))
            {
                _Workbook.Write(fs);
                fs.Close();
            }
        }

        /// <summary>
        /// 複製一整列
        /// </summary>
        /// <param name="worksheet">worksheet obj</param>
        /// <param name="sourceRowNum">來源Row Number</param>
        /// <param name="destinationRowNum">貼上Row Number</param>
        /// <param name="IsCoverRow">覆蓋模式True  插入模式Flase</param>
        /// <param name="IsRemoveSrcRow">清除原Row</param>
        /// <param name="copyRowHeight">複製Row高度</param>
        /// <param name="resetOriginalRowHeight">清除OriginalRow高度</param>
        public void CopyRow(ref ISheet worksheet, int sourceRowNum, int destinationRowNum, bool IsCoverRow, bool IsRemoveSrcRow, bool copyRowHeight, bool resetOriginalRowHeight)
        {
            #region 變數宣告
            IRow destRow = worksheet.GetRow(destinationRowNum);
            IRow sourceRow = worksheet.GetRow(sourceRowNum);
            IRow newRow;
            ICell oldCell, newCell;
            int i;

            Dictionary<string, int> dtsc = new Dictionary<string, int>();
            #endregion

            #region 判斷Row
            if (destRow == null)
                newRow = worksheet.CreateRow(destinationRowNum);
            else
            {
                if (!IsCoverRow)  //Row往下移
                {
                    worksheet.ShiftRows(destinationRowNum, worksheet.LastRowNum, 1);
                    newRow = worksheet.CreateRow(destinationRowNum);
                }
                else
                {
                    newRow = destRow;
                }
            }
            #endregion

            #region 逐一Cell Copy
            for (i = 0; i < sourceRow.LastCellNum; i++)
            {
                // Grab a copy of the old/new cell
                oldCell = sourceRow.GetCell(i);
                newCell = newRow.GetCell(i);

                if (newCell == null)
                    newCell = newRow.CreateCell(i);

                // If the old cell is null jump to next cell
                if (oldCell == null)
                {
                    newCell = null;
                    continue;
                }

                #region Copy Style and Vaue
                // Copy style from old cell and apply to new cell
                newCell.CellStyle = oldCell.CellStyle;

                // If there is a cell comment, copy
                if (newCell.CellComment != null) newCell.CellComment = oldCell.CellComment;

                // If there is a cell hyperlink, copy
                if (oldCell.Hyperlink != null) newCell.Hyperlink = oldCell.Hyperlink;

                // Set the cell data value
                switch (oldCell.CellType)
                {
                    #region SetValue
                    case CellType.Blank:
                        newCell.SetCellValue(oldCell.StringCellValue);
                        break;
                    case CellType.Boolean:
                        newCell.SetCellValue(oldCell.BooleanCellValue);
                        break;
                    case CellType.Error:
                        newCell.SetCellErrorValue(oldCell.ErrorCellValue);
                        break;
                    case CellType.Formula:
                        // newCell.CellFormula = oldCell.CellFormula;
                        break;
                    case CellType.Numeric:
                        newCell.SetCellValue(oldCell.NumericCellValue);
                        break;
                    case CellType.String:
                        newCell.SetCellValue(oldCell.RichStringCellValue);
                        break;
                    default:
                        newCell.SetCellValue(oldCell.StringCellValue);
                        break;
                        #endregion
                }
                #endregion
            }
            #endregion

            #region Merge Copy
            // If there are are any merged regions in the source row, copy to new row
            NPOI.SS.Util.CellRangeAddress cellRangeAddress = null;
            NPOI.SS.Util.CellRangeAddress newCellRangeAddress = null;
            for (i = 0; i < worksheet.NumMergedRegions; i++)
            {
                cellRangeAddress = worksheet.GetMergedRegion(i);
                if (cellRangeAddress.FirstRow == sourceRow.RowNum)
                {
                    newCellRangeAddress = new NPOI.SS.Util.CellRangeAddress(newRow.RowNum,
                                                                                (newRow.RowNum +
                                                                                 (cellRangeAddress.LastRow -
                                                                                  cellRangeAddress.FirstRow)),
                                                                                cellRangeAddress.FirstColumn,
                                                                                cellRangeAddress.LastColumn);
                    worksheet.AddMergedRegion(newCellRangeAddress);

                    //記錄Merge Source for Remove
                    dtsc.Add(i.ToString(), i);
                }
            }
            #endregion

            #region 後續處理
            //複製行高到新列
            if (copyRowHeight)
                newRow.Height = sourceRow.Height;
            //重製原始列行高
            if (resetOriginalRowHeight)
                sourceRow.Height = worksheet.DefaultRowHeight;
            //清掉原列
            //if (IsRemoveSrcRow == true)
            //{
            //    worksheet.RemoveRow(sourceRow);

            //    //清除Merge , 降冪排序才能正確清除
            //    //var result3 = from pair in dtsc orderby pair.Value descending select pair;
            //    //foreach (KeyValuePair<string, int> pair in result3)
            //    foreach (KeyValuePair<string, int> pair in dtsc)
            //    {
            //        worksheet.RemoveMergedRegion(pair.Value);
            //    }
            //}
            #endregion
        }

        /// <summary>
        /// 複製Rows
        /// </summary>
        /// <param name="sheet">worksheet Obj</param>
        /// <param name="startrow">Copy Start Row Number</param>
        /// <param name="endrow">Copy End Row Number</param>
        /// <param name="startPastIndex">貼上Row Index</param>
        /// <param name="copyRowHeight">複製Row高度</param>
        /// <param name="resetOriginalRowHeight">清除OriginalRow高度</param>
        public void ShiftRows(ref ISheet sheet, int intStartRow, int intEndRow, int intPastStart, bool IsCoverRow, bool IsRemoveSrcRow, bool copyRowHeight, bool resetOriginalRowHeight)
        {
            if (intPastStart < 0) return;

            int i;
            int j = 0;

            for (i = intStartRow; i <= intEndRow; i++)
            {
                if (sheet.GetRow(i) != null)
                {
                    CopyRow(ref sheet, i, intPastStart + j, IsCoverRow, IsRemoveSrcRow, copyRowHeight, resetOriginalRowHeight);
                }
                j++;
            }
        }

        /// <summary>
        /// 複製並插入列s
        /// </summary>
        /// <param name="intStartRow">開始Row</param>
        /// <param name="intEndRow">結束Row</param>
        /// <param name="intPastStart">貼上起始Row</param>
        public void CopyInsert(int startRow, int endRow, int pastStart)
        {
            ShiftRows(ref _ActiveSheet, startRow, endRow, pastStart, false, false, true, false);
        }

        /// <summary>
        /// 刪除列
        /// </summary>
        /// <param name="rowIndex"></param>
        public void DeleteRow(int rowIndex)
        {

            IRow newRow = _ActiveSheet.GetRow(rowIndex);
            _ActiveSheet.RemoveRow(newRow);
            ShiftRows(ref _ActiveSheet, rowIndex + 1, _ActiveSheet.LastRowNum, rowIndex, true, true, true, true);
        }

        /// <summary>
        /// 刪除欄
        /// </summary>
        /// <param name="intRowIndex"></param>
        public void DeleteColumn(int columnIndex)
        {
            for (int h = 0; h <= _ActiveSheet.LastRowNum; h++)
            {
                IRow iRow = _ActiveSheet.GetRow(h);
                iRow.RemoveCell(iRow.GetCell(columnIndex));
            }

        }

        /// <summary>
        /// 填公式
        /// </summary>
        /// <param name="rowIndex">Row</param>
        /// <param name="cellIndex">Cell</param>
        /// <param name="objCellValue">Value</param>
        public void SetFormula(int rowIndex, int cellIndex, string forumla)
        {
            IRow iRow = _ActiveSheet.GetRow(rowIndex);
            if (iRow == null)
                iRow = _ActiveSheet.CreateRow(rowIndex);

            ICell newCell = iRow.GetCell(cellIndex);
            if (newCell == null)
                newCell = iRow.CreateCell(cellIndex);

            newCell.SetCellFormula(forumla);

        }

        /// <summary>
        /// 重新計算Workbook中全部Cell中的公式
        /// </summary>
        public void EvaluateAllFormulaCells()
        {
            if (_xlFileType == FileType.xlsx)
                XSSFFormulaEvaluator.EvaluateAllFormulaCells(_Workbook);
            else
                HSSFFormulaEvaluator.EvaluateAllFormulaCells(_Workbook);
        }

        /// <summary>
        /// Sheet 重新命名
        /// </summary>
        /// <param name="intSheetIndex">第幾個Sheet</param>
        /// <param name="sheetNewName">要更改的Sheet名稱</param>
        public void SetSheetName(int sheetIndex, string sheetNewName)
        {
            if (!string.IsNullOrEmpty(sheetNewName))
                _Workbook.SetSheetName(sheetIndex, sheetNewName);
        }

        /// <summary>
        /// Active Sheet 重新命名
        /// </summary>
        /// <param name="sheetNewName">要更改的Sheet名稱</param>
        public void SetSheetName(string sheetNewName)
        {
            if (!string.IsNullOrEmpty(sheetNewName))
                _Workbook.SetSheetName(ActiveSheetIndex, sheetNewName);
        }

        /// <summary>
        /// 立即重算Excel檔全部sheet的Formula
        /// </summary>
        public void EvaluateFormulaCell_AllSheet()
        {
            for (int i = 0; i < _Workbook.NumberOfSheets; i++)
            {
                _EvaluateFormulaCell(_Workbook.GetSheetAt(i));
            }
        }

        /// <summary>
        /// 立即重算整個Active Sheet的Formula
        /// </summary>
        public void EvaluateFormulaCell_ActiveSheet()
        {
            _EvaluateFormulaCell(_ActiveSheet);
        }

        /// <summary>
        /// 重算整個isheet的Formula
        /// </summary>
        /// <param name="isheet"></param>
        private void _EvaluateFormulaCell(ISheet isheet)
        {
            IFormulaEvaluator eva = null;
            if (_xlFileType == FileType.xls)
                eva = new HSSFFormulaEvaluator(_Workbook);
            else
                eva = new XSSFFormulaEvaluator(_Workbook);


            IRow row;
            for (int i = 0; i < isheet.LastRowNum; i++)
            {
                row = isheet.GetRow(i);
                if (row == null) continue;

                for (int j = 0; j < row.LastCellNum; j++)
                {
                    ICell cell = row.GetCell(j);
                    if (cell != null && cell.CellType == CellType.Formula)
                    {
                        try
                        {
                            eva.EvaluateFormulaCell(cell);
                        }
                        catch { }
                    }
                }
            }
        }

        /// <summary>
        /// Active Sheet 自動調整欄寬,使用 NPOI 的 AutoSizeColumn 方法.
        /// 調整速度較慢,請考慮是否符合需求. 測試結果:10萬筆,30欄位,總共約 90 秒.
        /// 調整後欄寬會有預設的最小及最大欄寬限制,較容易觀看內容.
        /// </summary>
        /// <param name="columnCount">要設定寬度的前幾個欄位數(從1開始)</param>
        public void AutoSizeColumn(int columnCount)
        {
            AutoSizeColumn(_ActiveSheet, columnCount, _AutoSizeColumn_MinBytes, _AutoSizeColumn_MaxBytes);
        }

        /// <summary>
        /// Sheet 自動調整欄寬,使用 NPOI 的 AutoSizeColumn 方法.
        /// 調整速度較慢,請考慮是否符合需求. 測試結果:10萬筆,30欄位,總共約 90 秒.
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="columnCount">要設定寬度的前幾個欄位數(從1開始)</param>
        /// <param name="minColWidth">調整後欄位預設最小欄寬(英文字數,0:不設定)</param>
        /// <param name="maxColWidth">調整後欄位預設最大欄寬(英文字數,0:不設定)</param>
        public void AutoSizeColumn(ISheet sheet, int columnCount, int minColWidth, int maxColWidth)
        {
            if (minColWidth > 0) minColWidth = minColWidth * _CharWidth + _ColumnKeepSpace;
            if (maxColWidth > 0) maxColWidth = maxColWidth * _CharWidth + _ColumnKeepSpace;

            for (int col_idx = 0; col_idx < columnCount; col_idx++)
            {
                sheet.AutoSizeColumn(col_idx); //調整速度挺慢. 測試結果:10萬筆,30欄位,總共大約 90 秒

                if (minColWidth > 0 && minColWidth > sheet.GetColumnWidth(col_idx))
                    sheet.SetColumnWidth(col_idx, minColWidth);
                else if (maxColWidth > 0 && maxColWidth < sheet.GetColumnWidth(col_idx))
                    sheet.SetColumnWidth(col_idx, maxColWidth);
                else
                    sheet.SetColumnWidth(col_idx, sheet.GetColumnWidth(col_idx) + _ColumnKeepSpace);
            }
        }

        /// <summary>
        /// Active Sheet 自動調整欄寬,使用自訂方法,調整速度快.
        /// 調整後欄寬會有預設的最小及最大欄寬限制,較容易觀看內容.
        /// </summary>
        /// <param name="columnCount">要設定寬度的前幾個欄位數(從1開始)</param>
        public void AutoSizeColumn2(int columnCount)
        {
            AutoSizeColumn2(_ActiveSheet, columnCount, _AutoSizeColumn_MinBytes, _AutoSizeColumn_MaxBytes);
        }

        /// <summary>
        /// Sheet 自動調整欄寬,使用自訂方法,調整速度快.
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="columnCount">要設定寬度的前幾個欄位數(從1開始)</param>
        /// <param name="minColWidth">調整後欄位預設最小欄寬(英文字數,0:不設定)</param>
        /// <param name="maxColWidth">調整後欄位預設最大欄寬(英文字數,0:不設定)</param>
        public void AutoSizeColumn2(ISheet sheet, int columnCount, int minColWidth, int maxColWidth)
        {
            Encoding bog5_encoding = Encoding.GetEncoding("big5");
            int[] ar_col_max_bytes = new int[columnCount];
            int byte_count = 0;

            //自定欄寬調整. 測試結果:10萬筆,30欄位,約 1.5 秒
            for (int row_idx = 0; row_idx < sheet.PhysicalNumberOfRows; row_idx++)
            {
                IRow iRow = sheet.GetRow(row_idx);
                if (iRow == null) continue;

                for (int col_idx = 0; col_idx < columnCount; col_idx++)
                {
                    ICell cell = iRow.GetCell(col_idx);
                    if (cell == null) continue;

                    byte_count = bog5_encoding.GetByteCount(cell.ToString());
                    if (byte_count > ar_col_max_bytes[col_idx])
                        ar_col_max_bytes[col_idx] = byte_count;
                }
            }

            for (int col_idx = 0; col_idx < columnCount; col_idx++)
            {
                if (minColWidth > 0 && minColWidth > ar_col_max_bytes[col_idx])
                    sheet.SetColumnWidth(col_idx, minColWidth * _CharWidth + _ColumnKeepSpace);
                else if (maxColWidth > 0 && maxColWidth < ar_col_max_bytes[col_idx])
                    sheet.SetColumnWidth(col_idx, maxColWidth * _CharWidth + _ColumnKeepSpace);
                else
                    sheet.SetColumnWidth(col_idx, ar_col_max_bytes[col_idx] * _CharWidth + _ColumnKeepSpace);
            }
        }

        /// <summary>
        /// 指定列印範圍
        /// </summary>
        /// <param name="startCol">startCol</param>
        /// <param name="EndCol">EndCol</param>
        /// <param name="startRow">startRow</param>
        /// <param name="endRow">endRow</param>
        public void SetPrintArea(int startCol, int EndCol, int startRow, int endRow)
        {
            _Workbook.SetPrintArea(0, startCol, EndCol, startRow, endRow);
        }

        /// <summary>
        /// 指定目前要操作的sheet,並讀取全部資料. sheet必需要存在.
        /// 預設將忽略檔案中的空白列,不會新到到Table.
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="startRow">從第幾列開始讀(列數從1開始).</param>
        /// <param name="readCells">讀取的欄數,0代表全部.</param>
        /// <returns></returns>
        public DataTable SheetToDataTable(string sheetName, int startRow, int readCells)
        {
            _ActiveSheet = _Workbook.GetSheet(sheetName);
            if (_ActiveSheet == null) throw new Exception("Wrong sheet name.");
            return _SheetToDataTable(startRow, readCells, true);
        }

        /// <summary>
        /// 指定目前要操作的sheet,並讀取全部資料. sheet必需要存在.
        /// 預設將忽略檔案中的空白列,不會新到到Table.
        /// </summary>
        /// <param name="sheetIndex"></param>
        /// <param name="startRow">從第幾列開始讀(列數從1開始).</param>
        /// <param name="readCells">讀取的欄數,0代表全部.</param>
        /// <returns></returns>
        public DataTable SheetToDataTable(int sheetIndex, int startRow, int readCells)
        {
            _ActiveSheet = _Workbook.GetSheetAt(sheetIndex);
            if (_ActiveSheet == null) throw new Exception("Wrong sheet index.");
            return _SheetToDataTable(startRow, readCells, true);
        }

        /// <summary>
        /// 指定目前要操作的sheet,並讀取全部資料. sheet必需要存在.
        /// 預設將忽略檔案中的空白列,不會新到到Table.
        /// </summary>
        /// <param name="sheetIndex"></param>
        /// <param name="startRow">從第幾列開始讀(列數從1開始).</param>
        /// <param name="readCells">讀取的欄數,0代表全部.</param>
        /// <param name="skipBlankRow">略過空白列.</param>
        /// <returns></returns>
        public DataTable SheetToDataTable(int sheetIndex, int startRow, int readCells, bool skipBlankRow)
        {
            _ActiveSheet = _Workbook.GetSheetAt(sheetIndex);
            if (_ActiveSheet == null) throw new Exception("Wrong sheet index.");
            return _SheetToDataTable(startRow, readCells, skipBlankRow);
        }

        /// <summary>
        /// 讀取 Active Sheet 全部資料.
        /// </summary>
        /// <param name="startRow">從第幾列開始讀(列數從1開始).</param>
        /// <param name="readCells">讀取的欄數,0代表全部.</param>
        /// <param name="skipBlankRow">略過空白列.</param>
        /// <returns></returns>
        private DataTable _SheetToDataTable(int startRow, int readCells, bool skipBlankRow)
        {
            try
            {
                DataTable DT = new DataTable();
                DT.TableName = _ActiveSheet.SheetName;

                if (readCells > 0)
                {
                    for (int i = 0; i < readCells; i++)
                    {
                        DT.Columns.Add("F" + i);
                    }
                }

                int start_row = startRow > 1 ? startRow - 1 : 0;
                for (int RowNumb = start_row; RowNumb <= _ActiveSheet.LastRowNum; RowNumb++)
                {
                    try
                    {
                        IRow IR = _ActiveSheet.GetRow(RowNumb);

                        //if (IR.GetCell(0) == null || IR.GetCell(0).ToString().Trim() == "") break;   //停止讀檔

                        if (IR == null)
                        {
                            if (!skipBlankRow) DT.Rows.Add(DT.NewRow());

                            continue;
                        }

                        while (readCells <= 0 && IR.Cells.Count > DT.Columns.Count)
                        {
                            DT.Columns.Add("F" + DT.Columns.Count);
                        }

                        DataRow NewDR = DT.NewRow();

                        for (int ICNumb = 0; ICNumb < DT.Columns.Count; ICNumb++)
                        {
                            //ICell IC = IR.Cells[ICNumb];
                            ICell IC = IR.GetCell(ICNumb);

                            if (IC == null)
                            {
                                NewDR[ICNumb] = "";
                                continue;
                            }

                            switch (IC.CellType)
                            {
                                case CellType.Numeric:
                                    if (DateUtil.IsCellDateFormatted(IC))
                                        NewDR[ICNumb] = IC.DateCellValue.ToString("yyyy/MM/dd hh:mm:ss");
                                    else
                                    {
                                        if (IC.NumericCellValue.ToString().Contains("E"))
                                        {
                                            try
                                            {
                                                NewDR[ICNumb] = Convert.ToDecimal(IC.NumericCellValue).ToString();
                                            }
                                            catch
                                            {
                                                NewDR[ICNumb] = IC.NumericCellValue;
                                            }
                                        }
                                        else
                                            NewDR[ICNumb] = IC.NumericCellValue;
                                    }

                                    break;
                                case CellType.Error:
                                    NewDR[ICNumb] = "#VALUE!"; //IC.ErrorCellValue;
                                    break;
                                case CellType.Boolean:
                                    NewDR[ICNumb] = IC.BooleanCellValue;
                                    break;
                                case CellType.Blank:
                                    NewDR[ICNumb] = String.Empty;
                                    break;
                                case CellType.Formula:
                                    switch (IC.CachedFormulaResultType)
                                    {
                                        case CellType.Numeric:
                                            if (DateUtil.IsCellDateFormatted(IC))
                                                NewDR[ICNumb] = IC.DateCellValue.ToString("yyyy/MM/dd hh:mm:ss");
                                            else
                                            {
                                                if (IC.NumericCellValue.ToString().Contains("E"))
                                                {
                                                    try
                                                    {
                                                        NewDR[ICNumb] = Convert.ToDecimal(IC.NumericCellValue).ToString();
                                                    }
                                                    catch
                                                    {
                                                        NewDR[ICNumb] = IC.NumericCellValue;
                                                    }
                                                }
                                                else
                                                    NewDR[ICNumb] = IC.NumericCellValue;
                                            }
                                            break;
                                        case CellType.Error:
                                            NewDR[ICNumb] = "#VALUE!"; //IC.ErrorCellValue;
                                            break;
                                        case CellType.Boolean:
                                            NewDR[ICNumb] = IC.BooleanCellValue;
                                            break;
                                        case CellType.Blank:
                                            NewDR[ICNumb] = String.Empty;
                                            break;
                                        default:
                                            NewDR[ICNumb] = IC.StringCellValue;
                                            break;
                                    }

                                    break;
                                default:
                                    NewDR[ICNumb] = IC.StringCellValue;
                                    break;
                            }
                        } //end for (int ICNumb = 0

                        if (skipBlankRow)
                        {
                            foreach (object obj in NewDR.ItemArray)
                            {
                                if (Convert.ToString(obj).Trim() != "")
                                {
                                    DT.Rows.Add(NewDR);
                                    break;
                                }
                            }
                        }
                        else
                            DT.Rows.Add(NewDR);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Row No=" + RowNumb + ", error:" + ex.Message);
                    }

                } //end for (int RowNumb

                return DT;
            }
            catch (Exception ex)
            {
                //有錯誤時檢查原因
                throw;
            }
        }

        private string _GetAppConfig(string strKey)
        {
            foreach (string key in ConfigurationManager.AppSettings)
            {
                if (key == strKey)
                {
                    return ConfigurationManager.AppSettings[strKey];
                }
            }
            return null;
        }

        public MemoryStream OutputExcel()
        {
            MemoryStream ms = new MemoryStream();
            _Workbook.Write(ms);

            MemoryStream copyms = new MemoryStream(ms.ToArray());

            copyms.Flush();
            copyms.Position = 0;

            return copyms;
        }
    }
}