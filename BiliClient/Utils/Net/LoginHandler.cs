using BiliClient.Utils.Net.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Media.Imaging;

namespace BiliClient.Utils.Net
{
    class LoginHandler
    {
        private CookieContainer cookie;
        private string captcha;

        internal LoginInfo getLogin(string username,string pass,string captcha,ref string jsonData)
        {
            this.captcha = captcha;
            string hash = String.Empty, key = String.Empty;
            getKey(ref hash, ref key);
            RSACryptoServiceProvider RSAService = Crypto.CryptoHandler.CreateRsaProviderFromPublicKey(key);
            string enc_pw=Convert.ToBase64String(RSAService.Encrypt(Encoding.UTF8.GetBytes(hash + pass), false));
            WebHeaderCollection header = new WebHeaderCollection();
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("appkey", "buildkey");
            param.Add("captcha", captcha);
            param.Add("username", username);
            param.Add("password", enc_pw);
            string result = HTTPSRequest("https://passport.bilibili.com/api/v2/oauth2/login", "POST", null, header,ref cookie ,BlblApi.getUrlParam(param), "https://passport.bilibili.com/login");
            jsonData = result;
            LoginInfo info = JsonConvert.DeserializeObject<LoginInfo>(result);
            return info;
        }

        private void getCookie()
        {
            cookie = new CookieContainer();
            HTTPSRequest("https://passport.bilibili.com/captcha?_=" + Utils.getUnixStamp, "GET", null, new WebHeaderCollection(), ref cookie, null, "https://passport.bilibili.com/login");

        }

        internal BitmapImage getCaptcha()
        {
            getCookie();
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) =>
            {
                return true;
            });
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://passport.bilibili.com/captcha?_="+Utils.getUnixStamp);
            req.ProtocolVersion = HttpVersion.Version11;
            
            req.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
            req.UserAgent = "Mozilla/5.0 BiliDroid/5.32.0 (bbcallen@gmail.com)";
            req.Headers.Add("Cookie", "JSESSIONID="+HttpUtils.GetCookie("JSESSIONID", cookie)+"; "+ "sid=" + HttpUtils.GetCookie("sid", cookie));
            req.Referer = "https://passport.bilibili.com/login";
            HttpWebResponse wr = (HttpWebResponse)req.GetResponse();
            Stream stream = wr.GetResponseStream();
            MemoryStream ms = new MemoryStream();
            int bufferLen = 4096;
            byte[] buffer = new byte[bufferLen];
            int bytesCount = 0;
            while ((bytesCount = stream.Read(buffer, 0, bufferLen)) > 0)
            {
                ms.Write(buffer, 0, bytesCount);
            }
            stream.Close();
            string filePath = Environment.CurrentDirectory + "\\Captcha.jpg";
            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            byte[] fileBytes = ms.ToArray();
            fs.Write(fileBytes, 0, fileBytes.Length);
            fs.Close();
            ms.Close();
            BitmapImage captcha = new BitmapImage();
            captcha.BeginInit(); captcha.CacheOption = BitmapCacheOption.OnLoad;
            captcha.UriSource = new Uri(filePath);
            captcha.EndInit();
            captcha.Freeze();
            return captcha;
        }
        internal string getAccess(Dictionary<string,string> param,ref long mid)
        {
            WebHeaderCollection header = new WebHeaderCollection();
            CookieContainer cookie_tmp = new CookieContainer();

            param.Add("appkey", "buildkey");
            param.Add("ts", Utils.getUnixStamp.ToString());
            string result = HTTPSRequest("https://passport.bilibili.com/api/v2/oauth2/info?" + BlblApi.getUrlParam(param), "GET", null, header, ref cookie_tmp);
            JObject jsonData = JObject.Parse(result);
            if (int.Parse(jsonData["code"].ToString()) == 0)
            {
                mid = long.Parse(jsonData["data"]["mid"].ToString());
                return jsonData["data"]["access_token"].ToString();
            }
            return null;
        }

        private void getKey(ref string hash,ref string key)
        {
            WebHeaderCollection header = new WebHeaderCollection();

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("appkey", "buildkey");
            param.Add("captcha", captcha);
            string result = HTTPSRequest("https://passport.bilibili.com/api/oauth2/getKey", "POST", null,header , ref cookie,BlblApi.getUrlParam(param));
            JObject jsonData = JObject.Parse(result);
            hash = jsonData["data"]["hash"].ToString();
            key= jsonData["data"]["key"].ToString();
        }

        private string HTTPSRequest(string url, string type ,Dictionary<string, string> param, WebHeaderCollection header,ref CookieContainer cookie,string data=null,string referer=null)
        {
            if(param!=null)
                url = url.EndsWith("?") ? url + BlblApi.getUrlParam(param) : url + "?" + BlblApi.getUrlParam(param);
            HttpWebRequest req;

            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) =>
                {
                    return true;
                });
                req = (HttpWebRequest)WebRequest.Create(url);
                req.ProtocolVersion = HttpVersion.Version11;
            }
            else
            {
                req = (HttpWebRequest)WebRequest.Create(url);
            }
            req.ServicePoint.Expect100Continue = false;
            req.Method = type;
            req.Headers = header;
            req.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
            req.UserAgent= "Mozilla/5.0 BiliDroid/5.32.0 (bbcallen@gmail.com)";
            req.CookieContainer = cookie;
            if (referer != null)
                req.Referer = referer;
            if (data != null)
            {
                byte[] postData = Encoding.UTF8.GetBytes(data);
                req.ContentLength = data.Length;
                Stream writer = req.GetRequestStream();
                writer.Write(postData, 0, postData.Length);
                writer.Close();
            }
            HttpWebResponse wr = (HttpWebResponse)req.GetResponse();
            using (StreamReader stream = new StreamReader(wr.GetResponseStream()))
            {
                string result = stream.ReadToEnd();
                wr.Close();
                return BlblApi.UnicodeConvent(result);
            }
        }
    }
}
