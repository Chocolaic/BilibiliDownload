using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BiliClient.Utils.Net
{
    class UserHandler
    {
        internal static bool sendLike(long aid)
        {
            if (Session.SessionToken.Logined)
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("appkey", "buildkey");
                param.Add("aid", aid.ToString());
                param.Add("dislike", "0");
                param.Add("like", "0");
                param.Add("ts", Utils.getUnixStamp.ToString());

                string result = HTTPSRequest("https://app.biliapi.net/x/v2/view/like",BlblApi.getUrlParam(param),"POST");
                JObject jsonData = JObject.Parse(result);
                return (int.Parse(jsonData["code"].ToString()) == 0);
            }
            return false;
        }
        internal static int sendCoins(long aid,long mid,ref string msg)
        {
            if (Session.SessionToken.Logined)
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("appkey", "buildkey");
                param.Add("aid", aid.ToString());
                param.Add("mid", mid.ToString());
                param.Add("multiply", "1");
                param.Add("ts", Utils.getUnixStamp.ToString());

                string result = HTTPSRequest("https://app.biliapi.com/x/v2/view/coin/add", BlblApi.getUrlParam(param),"POST");
                JObject jsonData = JObject.Parse(result);
                msg = jsonData["message"].ToString();
                return int.Parse(jsonData["code"].ToString());
            }
            return -1;
        }
        internal static JArray getFavFolder(long aid)
        {
            if (Session.SessionToken.Logined)
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("appkey", "buildkey");
                param.Add("aid", aid.ToString());
                param.Add("vmid", Session.SessionToken.info.data.mid.ToString());
                param.Add("ts", Utils.getUnixStamp.ToString());

                string result = HTTPSRequest("https://api.bilibili.com/x/v2/fav/folder?" + BlblApi.getUrlParam(param),null , "GET");
                JArray jsonArr=JArray.Parse(JObject.Parse(result)["data"].ToString());
                return jsonArr;
            }
            return null;
        }
        internal static API.FavoriteInfo getFavo(long fid)
        {
            if (Session.SessionToken.Logined)
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("appkey", "buildkey");
                param.Add("vmid", Session.SessionToken.info.data.mid.ToString());
                param.Add("fid", fid.ToString());
                param.Add("ts", Utils.getUnixStamp.ToString());
                param.Add("order", "ftime");

                string result = HTTPSRequest("https://app.bilibili.com/x/v2/favorite/video?" + BlblApi.getUrlParam(param), null, "GET");
                API.FavoriteInfo info = Newtonsoft.Json.JsonConvert.DeserializeObject<API.FavoriteInfo>(result);
                return info;
            }
            return null;
        }
        internal static int setFavo(long aid,long fid,string act,ref string msg)
        {
            if (Session.SessionToken.Logined)
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("appkey", "buildkey");
                param.Add("aid", aid.ToString());
                param.Add("fid", fid.ToString());
                param.Add("ts", Utils.getUnixStamp.ToString());

                string result = HTTPSRequest("https://api.bilibili.com/x/v2/fav/video/"+act, BlblApi.getUrlParam(param), "POST");
                JObject jsonData = JObject.Parse(result);
                msg = jsonData["message"].ToString();
                return int.Parse(jsonData["code"].ToString());
            }
            return -1;
        }
        internal static int setHistory(long aid,long cid)
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

                string result = HTTPSRequest("https://api.bilibili.com/x/v2/history/report", BlblApi.getUrlParam(param), "POST");
                JObject jsonData = JObject.Parse(result);
                return int.Parse(jsonData["code"].ToString());
            }
            return -1;
        }
        private static string HTTPSRequest(string url,string data,string type)
        {
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
            req.Method = type;
            req.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
            req.UserAgent = "Mozilla/5.0 BiliDroid/5.32.0 (bbcallen@gmail.com)";
            if (type == "POST")
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
