using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class FormCommon
    {
        /// <summary>
        /// 計算起始與結束的資料行數，取得當前分頁需要的行數是第幾到第幾行
        /// </summary>
        /// <param name="pPageIndex">當前頁碼</param>
        /// <param name="pPageCount">一頁的最大顯示數量</param>
        /// <param name="pStartRow">起始行數</param>
        /// <param name="pEndRow">結束行數</param>
        public static void CalDataSAndE(int pPageIndex, int pPageCount, ref int pStartRow, ref int pEndRow)
        {
            //計算起始與結束的資料行數，取得當前分頁需要的行數是第幾到第幾行
            if (pPageIndex < 2)
            {
                pStartRow = 1;
                pEndRow = pPageCount;
            }
            else
            {
                pStartRow = ((pPageIndex - 1) * pPageCount) + 1;
                pEndRow = (pPageIndex * pPageCount);
            }
        }
    }
}
