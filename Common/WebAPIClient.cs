using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class WebAPIClient
    {
        string _apiUrl = string.Empty;
        string _userName = string.Empty;
        string _userPassword = string.Empty;

        public uint APITimeout { get; set; }

        public WebAPIClient(string apiUrl, string userName, string userPassword, uint apiTimeout = 60)
        {
            _apiUrl = apiUrl;
            _userName = userName;
            _userPassword = userPassword;
            APITimeout = apiTimeout;
        }

        public string CallWebAPI(string MethodName, string body)
        {
#if DEBUG
            // 無視伺服器憑證驗證(正式環境不加)
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            // 設定安全協定為 TLS 1.2 與 TLS 1.1
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
#endif

            string result = string.Empty;

            // 設定 WebService 要呼叫的 API 位置
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_apiUrl + MethodName);

            // 設定授權標頭
            string authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_userName}:{_userPassword}"));
            request.Headers.Add("Authorization", "Basic" + authHeader);

            // 設定 HTTP 請求方法為 POST
            request.Method = "POST";

            // 設定請求內容類型為 JSON 格式
            request.ContentType = @"application/json; charset=utf-8";

            // 將請求內容寫入請求流
            using (StreamWriter streamwriter = new StreamWriter(request.GetRequestStream()))
            {
                streamwriter.Write(body);
            }

            // 設定請求超時時間
            request.Timeout = Convert.ToInt32(APITimeout) * 1000;

            // 取得 WebService 回應
            WebResponse response = request.GetResponse();

            // 讀取回應流中的內容
            using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            return result;
        }
    }
}
