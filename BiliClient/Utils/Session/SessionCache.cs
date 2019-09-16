using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Web;

namespace BiliClient.Utils.Session
{
    class SessionCache
    {
        internal static void SaveAccount()
        {
            if (SessionToken.cookies != null)
            {
                XmlDocument xmldoc = (XmlDocument)JsonConvert.DeserializeXmlNode(SessionToken.cookies, "userdata");
                XmlElement token = xmldoc.CreateElement("token");
                token.InnerText = SessionToken.access;
                xmldoc.DocumentElement.AppendChild(token);
                xmldoc.Save("user.xml");
            }

        }

        internal static bool LoadUserInfo()
        { 
            if (File.Exists("user.xml"))
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load("user.xml");
                XmlNodeList cookies= xmldoc.DocumentElement.GetElementsByTagName("cookies");
                Dictionary<string, string> param = new Dictionary<string, string>();
                foreach (XmlNode node in cookies)
                {
                    string key=((XmlElement)node).GetElementsByTagName("name")[0].InnerText;
                    string value= ((XmlElement)node).GetElementsByTagName("value")[0].InnerText;
                    long expires= long.Parse(((XmlElement)node).GetElementsByTagName("expires")[0].InnerText);
                    if (expires < Utils.getUnixStamp)
                        return false;
                    //param.Add(key, value);

                }
                param.Add("access_token", xmldoc.DocumentElement.GetElementsByTagName("token")[0].InnerText);
                Net.LoginHandler handle = new Net.LoginHandler();
                long mid=0;
                string newtoken=handle.getAccess(param,ref mid);
                if (newtoken != null)
                {
                    SessionToken.access = newtoken;
                    SessionToken.Logined = true;
                    SessionToken.info = Net.BlblApi.getUserInfo();
                    InteractHandler.UIhandle.setLogin(SessionToken.info);
                    return true;
                }
            }
            return false;
        }
    }
}
