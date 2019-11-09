using System.Collections.Generic;

namespace BiliClient.Utils.Net.API
{
    public class ResInfo
    {
        public string referer { get; set; }
        public long aid { get; set; }
        public Data data { get; set; }
        public class Data
        {
            public int quality { get; set; }
            public string format { get; set; }
            public List<int> accept_quality { get; set; }
            public List<string> accept_description { get; set; }
            public List<Durl> durl { get; set; }
        }
        public class Durl
        {
            public long length { get; set; }
            public long size { get; set; }
            public string url { get; set; }
            public List<string> backup_url { get; set; }
        }
    }

}
