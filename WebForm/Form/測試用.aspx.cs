using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace WebForm.Form
{
    public partial class 測試用 : basePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //this.ClosePanel();
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (fileUpload.HasFile)
            {
                try
                {
                    if (fileUpload.PostedFile.ContentType.ToLower().StartsWith("image/"))
                    {
                        // 設置圖片預覽
                        imgPreview.ImageUrl = "~/UploadedImages/" + fileUpload.FileName;
                        // 保存文件到指定文件夾
                        string filePath = Server.MapPath("~/UploadedImages/") + fileUpload.FileName;
                        fileUpload.SaveAs(filePath);
                    }
                    else
                    {
                        // 非圖片文件的處理
                        // 可以顯示錯誤信息給用戶
                        base.DoAlertinAjax(this.Page, "msg", "請上傳圖片格式!");
                        return;
                    }
                    // 取得上傳檔案的名稱和路徑
                    
                    /*
                    string filename = Path.GetFileName(fileUpload.FileName);
                    string path = Server.MapPath("~/UploadedImages/") + filename;

                    // 將檔案存儲在伺服器上指定的路徑
                    fileUpload.SaveAs(path);

                    // 提示上傳成功
                    Response.Write("Upload status: File uploaded successfully!");
                    */
                }
                catch (Exception ex)
                {
                    // 提示上傳失敗
                    base.DoAlertinAjax(this.Page, "msg", "上傳失敗!");
                    return;
                }
            }
            else
            {
                // 如果沒有選擇檔案，提示用戶選擇檔案
                //Response.Write("Please select a file to upload.");
                //StringBuilder sb = new StringBuilder();
                base.DoAlertinAjax(this.Page, "msg", "未輸入圖片!");
                return;
            }
        }

        public void ClosePanel()
        {
            //plPreview.Visible = false;
        }
        public void OpenPanel()
        {
            //plPreview.Visible = true;
        }
    }
}