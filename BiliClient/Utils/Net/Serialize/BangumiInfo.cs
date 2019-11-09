using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliClient.Utils.Net.API
{
    public class BangumiInfo
    {
        public int code { get; set; }
        public List<string> accept_description { get; set; }
        public string accept_format { get; set; }
        public List<int> accept_quality { get; set; }
        public List<Durl> durl { get; set; }
        public class Durl
        {
            public long length { get; set; }
            public long size { get; set; }
            public string url { get; set; }
        }
        public string referer { get; set; }
    }
}
