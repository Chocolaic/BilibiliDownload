using BiliClient.Utils.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace BiliClient.Utils.Session
{
    class SessionCache
    {
        internal static void SaveAccount()
        {
            if (SessionToken.cookies != null)
            {
                XmlDocument xmldoc = (XmlDocument)JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(SessionToken.cookies), "userdata");
                XmlElement token = xmldoc.CreateElement("token");
                token.InnerText = SessionToken.access;
                xmldoc.DocumentElement.AppendChild(token);
                xmldoc.Save("user.xml");
            }

        }

        internal static void LoadUserInfo(HttpCallBack call)
        {
            if (File.Exists("user.xml"))
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load("user.xml");
                XmlNodeList cookies = xmldoc.DocumentElement.GetElementsByTagName("cookies");
                Dictionary<string, string> param = new Dictionary<string, string>();
                foreach (XmlNode node in cookies)
                {
                    string key = ((XmlElement)node).GetElementsByTagName("name")[0].InnerText;
                    string value = ((XmlElement)node).GetElementsByTagName("value")[0].InnerText;
                    long expires = long.Parse(((XmlElement)node).GetElementsByTagName("expires")[0].InnerText);
                    if (expires < Utils.getUnixStamp)
                        return;
                    //param.Add(key, value);

                }
                param.Add("access_token", xmldoc.DocumentElement.GetElementsByTagName("token")[0].InnerText);
                Net.LoginHandler handle = new Net.LoginHandler();
                handle.getAccess(param, (bool isSuccess, object data) =>
                {
                    call(isSuccess, data);
                });
            }
        }
    }
}
