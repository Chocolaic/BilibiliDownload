using BiliClient.Utils.Net.API;
using Newtonsoft.Json;
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
    class BlblApi
    {
        internal static VideoInfo getVideoInfo(long id)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("appkey", "appkey");
            param.Add("id", id.ToString());
            param.Add("type", "json");
            string result= HTTPRequest("https://api.bilibili.com/view", param);
            VideoInfo info= JsonConvert.DeserializeObject<VideoInfo>(result);
            return info;
        }

        internal static ResInfo getResInfo(long aid,long cid,int qn=32)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("appkey", "appkey");
            param.Add("aid", aid.ToString());
            param.Add("cid", cid.ToString());
            param.Add("qn", qn.ToString());
            param.Add("ts", Utils.getUnixStamp.ToString());
            param.Add("otype", "json");
            string url = "https://app.bilibili.com/x/playurl?"+getUrlParam(param);
            string result = HTTPRequest(url, null);
            ResInfo info= JsonConvert.DeserializeObject<ResInfo>(result);
            info.aid = aid;
            info.referer = url;
            return info;
        }
        internal static AccountInfo getUserInfo()
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("appkey", "buildkey");
            param.Add("ts", Utils.getUnixStamp.ToString());
            param.Add("platform", "android");
            param.Add("mobi_app", "android");
            param.Add("build", "5320000");
            string result= HTTPRequest("https://app.biliapi.com/x/v2/account/mine", param);
            AccountInfo info= JsonConvert.DeserializeObject<AccountInfo>(result);
            return info;
        }
        internal static HistoryInfo getHistory(int max = 0)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("appkey", "buildkey");
            param.Add("business", "archive");
            param.Add("max", max.ToString());
            param.Add("max_tp", max.ToString());
            param.Add("ps", "20");
            param.Add("ts", Utils.getUnixStamp.ToString());
            string result = HTTPRequest("https://app.bilibili.com/x/v2/history/cursor", param);
            HistoryInfo info= JsonConvert.DeserializeObject<HistoryInfo>(result);
            return info;
        }

        internal static string getUrlParam(Dictionary<string,string> param)
        {
            if (Session.SessionToken.Logined)
                param.Add("access_key", Session.SessionToken.access);
            var dicSort = from objDic in param orderby objDic.Key select objDic;
            string data = String.Empty;
            string[] deKey = AppData.getValue(Resource.ResourceManager.GetString(param["appkey"])).Split('&');param["appkey"] = deKey[0];
            foreach (KeyValuePair<string, string> kvp in dicSort)
            {
                data += kvp.Key + "=" + urlEncode(kvp.Value)+"&";
            }
            MD5 md5=new MD5CryptoServiceProvider();
            byte[] md5Data=md5.ComputeHash(Encoding.UTF8.GetBytes(data.TrimEnd('&') + deKey[1]));
            return data+"sign="+BitConverter.ToString(md5Data).Replace("-","").ToLower();
        }

        private static string HTTPRequest(string url, Dictionary<string, string> param=null)
        {
            if(param!=null)
                url = url.EndsWith("?") ? url + getUrlParam(param) : url + "?" + getUrlParam(param);
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
            HttpWebResponse wr= (HttpWebResponse)req.GetResponse();
            using (StreamReader stream = new StreamReader(wr.GetResponseStream()))
            {
                string result=stream.ReadToEnd();
                wr.Close();
                return UnicodeConvent(result);
            }
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
