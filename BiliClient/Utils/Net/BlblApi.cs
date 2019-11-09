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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace BiliClient.Utils.Net
{
    internal delegate void HttpCallBack(bool isSuccess, object data);
    class BlblApi
    {
        internal static void getVideoInfo(long id, HttpCallBack call)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();

            param.Add("appkey", "buildkey");
            param.Add("aid", id.ToString());


            param.Add("build", "0");
            param.Add("mobi_app", "android");
            param.Add("autoplay", "1");
            param.Add("ts", Utils.getUnixStamp.ToString());
            HTTPRequest<VideoInfo>("https://app.bilibili.com/x/v2/view", param, call);
        }

        internal static void getResInfo(long aid, long cid, int qn, HttpCallBack call)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("appkey", "appkey");
            param.Add("aid", aid.ToString());
            param.Add("cid", cid.ToString());
            param.Add("qn", qn.ToString());
            param.Add("ts", Utils.getUnixStamp.ToString());
            param.Add("otype", "json");
            string url = "https://app.bilibili.com/playurl?" + getUrlParam(param);
            HTTPRequest<_ResInfo>(url, null,
                (bool isSuccess, object data) =>
                {
                    if (isSuccess)
                    {
                        _ResInfo info = (_ResInfo)data;
                        info.referer = url;
                        call(true, info);
                    }
                    else
                        call(isSuccess, data);
                });
        }
        internal static void getBangumiInfo(long aid, long cid, int qn, HttpCallBack call)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("appkey", "appkey");
            param.Add("aid", aid.ToString());
            param.Add("cid", cid.ToString());
            param.Add("qn", qn.ToString());
            param.Add("ts", Utils.getUnixStamp.ToString());
            param.Add("otype", "json");
            string url = "https://bangumi.bilibili.com/player/api/v2/playurl?" + getUrlParam(param);
            HTTPRequest<BangumiInfo>(url, null,
                (bool isSuccess, object data) =>
                {
                    if (isSuccess)
                    {
                        BangumiInfo info = (BangumiInfo)data;
                        info.referer = url;
                        call(true, info);
                    }
                    else
                        call(isSuccess, data);
                });
        }
        internal static void getAccountInfo(HttpCallBack call)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("appkey", "buildkey");
            param.Add("ts", Utils.getUnixStamp.ToString());
            param.Add("platform", "android");
            param.Add("mobi_app", "android");
            param.Add("build", "5320000");
            HTTPRequest<AccountInfo>("https://app.biliapi.com/x/v2/account/mine", param, call);
        }
        internal static void getSeasonInfo(string epid, HttpCallBack call)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("appkey", "buildkey");
            param.Add("build", "5320000");
            param.Add("ep_id", epid);
            param.Add("mobi_app", "android");
            param.Add("platform", "android");
            param.Add("track_path", "0");
            HTTPRequest<SeasonInfo>("https://api.bilibili.com/pgc/view/app/season", param, call);
        }
        internal static void getHistory(int max, HttpCallBack call)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("appkey", "buildkey");
            param.Add("business", "archive");
            param.Add("max", max.ToString());
            param.Add("max_tp", max.ToString());
            param.Add("ps", "20");
            param.Add("ts", Utils.getUnixStamp.ToString());
            HTTPRequest<HistoryInfo>("https://app.bilibili.com/x/v2/history/cursor", param, call);
        }

        internal static string getUrlParam(Dictionary<string, string> param)
        {
            if (Session.SessionToken.Logined)
                param.Add("access_key", Session.SessionToken.access);
            var dicSort = from objDic in param orderby objDic.Key select objDic;
            string data = String.Empty;
            string[] deKey = AppData.getValue(Resource.ResourceManager.GetString(param["appkey"])).Split('&'); param["appkey"] = deKey[0];
            foreach (KeyValuePair<string, string> kvp in dicSort)
            {
                data += kvp.Key + "=" + urlEncode(kvp.Value) + "&";
            }
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] md5Data = md5.ComputeHash(Encoding.UTF8.GetBytes(data.TrimEnd('&') + deKey[1]));
            return data + "sign=" + BitConverter.ToString(md5Data).Replace("-", "").ToLower();
        }

        private async static void HTTPRequest<T>(string url, Dictionary<string, string> param, HttpCallBack call)
        {
            if (param != null)
                url = url.EndsWith("?") ? url + getUrlParam(param) : url + "?" + getUrlParam(param);
            HttpWebRequest req;
            Console.WriteLine($"Request from {url}");
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) =>
                {
                    return true;
                });
                req = WebRequest.Create(url) as HttpWebRequest;
                req.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                req = WebRequest.Create(url) as HttpWebRequest;
            }
            req.Method = "GET";
            await Task.Run(() =>
            {
                try
                {
                    HttpWebResponse wr = (HttpWebResponse)req.GetResponse();
                    using (StreamReader stream = new StreamReader(wr.GetResponseStream()))
                    {
                        string result = UnicodeConvent(stream.ReadToEnd());
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
        internal static void DoHTTPDownload(string filePath, string url)
        {
            HttpWebRequest req;

            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) =>
                {
                    return true;
                });
                req = WebRequest.Create(url) as HttpWebRequest;
                req.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                req = WebRequest.Create(url) as HttpWebRequest;
            }
            req.Method = "GET";
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
            FileStream fs = new FileStream(filePath, FileMode.Create);
            byte[] fileBytes = ms.ToArray();
            fs.Write(fileBytes, 0, fileBytes.Length);
            fs.Close();
            ms.Close();
        }

        internal static string UnicodeConvent(string Str)
        {
            return new Regex(@"\\u([0-9A-F]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled).Replace(
                Str, x => string.Empty + Convert.ToChar(Convert.ToUInt16(x.Result("$1"), 16)));
        }

        internal static string urlEncode(string str)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char c in str)
            {
                if (HttpUtility.UrlEncode(c.ToString()).Length > 1)
                {
                    builder.Append(HttpUtility.UrlEncode(c.ToString()).ToUpper());
                }
                else
                    builder.Append(c);
            }
            return builder.ToString();
        }
    }
}
