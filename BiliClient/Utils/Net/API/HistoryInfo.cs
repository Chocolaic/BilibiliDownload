using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliClient.Utils.Net.API
{
    public class HistoryInfo
    {
        public int code { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
        public class Data
        {
            public List<List> list { get; set; }
        }
        public class List
        {
            public string title { get; set; }
            public string cover { get; set; }
            public string uri { get; set; }
            public long mid { get; set; }
            public string name { get; set; }
            public History history { get; set; }
        }
        public class History
        {
            public long oid { get; set; }
            public long cid { get; set; }
        }
    }
}
