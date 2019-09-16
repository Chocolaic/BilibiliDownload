using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliClient.Utils.Net.API
{
    public class VideoInfo
    {
        public int code { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public long mid { get; set; }
        public string pic { get; set; }
        public string face { get; set; }
        public long cid { get; set; }
        public string author { get; set; }

        public long aid { get; set; }
    }
}
