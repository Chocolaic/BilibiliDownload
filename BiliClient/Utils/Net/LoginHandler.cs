using BiliClient.Utils.Net.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BiliClient.Utils.Net
{
    class LoginHandler
    {
        private CookieContainer cookie;

        public LoginHandler()
        {
            cookie = new CookieContainer();
        }

        internal void getLogin(string username, string pwd, string captcha, HttpCallBack call)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("appkey", "buildkey");
            HTTPSRequest<JObject>("https://passport.bilibili.com/api/oauth2/getKey", "POST", param, null,
                (bool isSuccess, object data) =>
                {
                    if (isSuccess)
                    {
                        JObject jsonData = (JObject)data;
                        string hash = jsonData["data"]["hash"].ToString(), key = jsonData["data"]["key"].ToString();
                        RSACryptoServiceProvider RSAService = Crypto.CryptoHandler.CreateRsaProviderFromPublicKey(key);
                        string enc_pw = Convert.ToBase64String(RSAService.Encrypt(Encoding.UTF8.GetBytes(hash + pwd), false));
                        param.Clear();
                        param.Add("appkey", "buildkey");
                        param.Add("captcha", captcha);
                        param.Add("username", username);
                        param.Add("password", enc_pw);
                        HTTPSRequest<LoginInfo>("https://passport.bilibili.com/api/v2/oauth2/login", "POST", param, null,
                            (bool _isSuccess, object _data) =>
                            {
                                call(_isSuccess, _data);//LoginInfo
                            });
                    }
                    else
                        call(isSuccess, data);
                });
        }
        internal void getAccess(Dictionary<string, string> param, HttpCallBack call)
        {
            param.Add("appkey", "buildkey");
            param.Add("ts", Utils.getUnixStamp.ToString());
            HTTPSRequest<JObject>("https://passport.bilibili.com/api/v2/oauth2/info", "GET", param, null, call);
        }
        internal void getCaptcha(HttpCallBack call)
        {
            HTTPSRequest<string>("https://passport.bilibili.com/captcha?_=" + Utils.getUnixStamp, "GET", null, "https://passport.bilibili.com/login",
                (bool isSuccess, object data) =>
                {
                    if (isSuccess)
                    {
                        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) =>
                        {
                            return true;
                        });
                        HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://passport.bilibili.com/captcha?_=" + Utils.getUnixStamp);
                        req.ProtocolVersion = HttpVersion.Version11;

                        req.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
                        req.UserAgent = "Mozilla/5.0 BiliDroid/5.32.0 (bbcallen@gmail.com)";
                        req.Headers.Add("Cookie", "JSESSIONID=" + HttpUtils.GetCookie("JSESSIONID", cookie) + "; " + "sid=" + HttpUtils.GetCookie("sid", cookie));
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
                        call(true, captcha);
                    }
                    else
                        call(isSuccess, data);
                });
        }
        internal async void HTTPSRequest<T>(string url, string type, Dictionary<string, string> param, string referer = null, HttpCallBack call = null)
        {
            string paramStr = String.Empty;
            if (param != null)
            {
                paramStr = BlblApi.getUrlParam(param);
                url = url.EndsWith("?") ? url + BlblApi.getUrlParam(param) : url + "?" + paramStr;
            }
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
            req.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
            req.UserAgent = "Mozilla/5.0 BiliDroid/5.32.0 (bbcallen@gmail.com)";
            req.CookieContainer = cookie;
            if (referer != null)
                req.Referer = referer;

            await Task.Run(() =>
            {
                try
                {
                    if (type == "POST")
                    {
                        byte[] postData = Encoding.UTF8.GetBytes(paramStr);
                        req.ContentLength = postData.Length;
                        Stream writer = req.GetRequestStream();
                        writer.Write(postData, 0, postData.Length);
                        writer.Close();
                    }
                    HttpWebResponse wr = (HttpWebResponse)req.GetResponse();
                    using (StreamReader stream = new StreamReader(wr.GetResponseStream()))
                    {
                        string result = BlblApi.UnicodeConvent(stream.ReadToEnd());
                        wr.Close();

                        Type t = typeof(T);
                        if (t.Name == "String")
                            call(true, result);
                        else if (t.Name == "JObject")
                            call(true, JObject.Parse(result));
                        else
                            call(true, JsonConvert.DeserializeObject<T>(result));
                    }
                }
                catch (Exception e)
                {
                    call(false, e.Message);
                }
            });
        }
    }
}
