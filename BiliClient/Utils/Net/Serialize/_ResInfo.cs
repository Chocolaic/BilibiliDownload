using System.Collections.Generic;

namespace BiliClient.Utils.Net.API
{
    public class _ResInfo
    {
        public string referer { get; set; }
        public int quality { get; set; }
        public string format { get; set; }
        public long timelength { get; set; }
        public string accept_format { get; set; }
        public List<int> accept_quality { get; set; }
        public List<Durl> durl { get; set; }
        public class Durl
        {
            public int order { get; set; }
            public long length { get; set; }
            public long size { get; set; }
            public string url { get; set; }
            public List<string> backup_url { get; set; }
        }
    }
}
