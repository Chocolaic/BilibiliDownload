using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BiliClient.Utils.Net
{
    class UserHandler
    {
        internal static void sendLike(long aid, HttpCallBack call)
        {
            if (Session.SessionToken.Logined)
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("appkey", "buildkey");
                param.Add("aid", aid.ToString());
                param.Add("dislike", "0");
                param.Add("like", "0");
                param.Add("ts", Utils.getUnixStamp.ToString());

                HTTPSRequest<JObject>("https://app.biliapi.net/x/v2/view/like", "POST", param, null, call);
                return;
            }
            call(false, "unlogin");
        }
        internal static void sendCoins(long aid, long mid, HttpCallBack call)
        {
            if (Session.SessionToken.Logined)
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("appkey", "buildkey");
                param.Add("aid", aid.ToString());
                param.Add("mid", mid.ToString());
                param.Add("multiply", "1");
                param.Add("ts", Utils.getUnixStamp.ToString());

                HTTPSRequest<JObject>("https://app.biliapi.com/x/v2/view/coin/add", "POST", param, null, call);
                return;
            }
            call(false, "unlogin");
        }
        internal static void getFavFolder(long aid, HttpCallBack call)
        {
            if (Session.SessionToken.Logined)
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("appkey", "buildkey");
                param.Add("aid", aid.ToString());
                param.Add("vmid", Session.SessionToken.info.data.mid.ToString());
                param.Add("ts", Utils.getUnixStamp.ToString());

                HTTPSRequest<JObject>("https://api.bilibili.com/x/v2/fav/folder", "GET", param, null,
                    (bool isSuccess, object data) =>
                    {
                        if (isSuccess)
                        {
                            JArray jsonArr = JArray.Parse(((JObject)data)["data"].ToString());
                            call(true, jsonArr);
                        }
                    });
                return;
            }
            call(false, "unlogin");
        }
        internal static void getFavo(long fid, HttpCallBack call)
        {
            if (Session.SessionToken.Logined)
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("appkey", "buildkey");
                param.Add("vmid", Session.SessionToken.info.data.mid.ToString());
                param.Add("fid", fid.ToString());
                param.Add("ts", Utils.getUnixStamp.ToString());
                param.Add("order", "ftime");

                HTTPSRequest<API.FavoriteInfo>("https://app.bilibili.com/x/v2/favorite/video", "GET", param, null, call);
                return;
            }
            call(false, "unlogin");
        }
        internal static void setFavo(long aid, long fid, string act, HttpCallBack call)
        {
            if (Session.SessionToken.Logined)
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("appkey", "buildkey");
                param.Add("aid", aid.ToString());
                param.Add("fid", fid.ToString());
                param.Add("ts", Utils.getUnixStamp.ToString());

                HTTPSRequest<JObject>("https://api.bilibili.com/x/v2/fav/video/" + act, "POST", param, null, call);
                return;
            }
            call(false, "unlogin");
        }
        internal static void setHistory(long aid, long cid, HttpCallBack call)
        {
            if (Session.SessionToken.Logined)
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("appkey", "buildkey");
                param.Add("aid", aid.ToString());
                param.Add("cid", cid.ToString());
                param.Add("epid", "0");
                param.Add("realtime", "4");
                param.Add("type", "3");

                HTTPSRequest<JObject>("https://api.bilibili.com/x/v2/history/report", "POST", param, null, call);
                return;
            }
            call(false, "unlogin");
        }
        private static async void HTTPSRequest<T>(string url, string type, Dictionary<string, string> param, string referer = null, HttpCallBack call = null)
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
