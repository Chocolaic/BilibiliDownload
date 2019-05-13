using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliClient.Utils.Net.API
{
    public class LoginInfo
    {
        public int code { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
        public class Data
        {
            public Token_info token_info { get; set; }
            public Cookie_info cookie_info { get; set; }
            public class Cookie_info
            {
                List<Cookies> cookies { get; set; }
                public class Cookies
                {
                    public string name { get; set; }
                    public string value { get; set; }
                    public int http_only { get; set; }
                    public long expires { get; set; }
                }
            }
            public class Token_info
            {
                public long mid { get; set; }
                public string access_token { get; set; }
                public string refresh_token { get; set; }

            }
        }

    }

}
