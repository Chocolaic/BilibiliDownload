using System.Collections.Generic;

namespace BiliClient.Utils.Net.API
{
    public class SeasonInfo
    {
        public int code { get; set; }
        public string message { get; set; }
        public Result result { get; set; }
        public class Result
        {
            public string cover { get; set; }
            public string evaluate { get; set; }
            public List<Episode> episodes { get; set; }
            public class Episode
            {
                public long aid { get; set; }
                public long cid { get; set; }
                public string bvid { get; set; }
                public string cover { get; set; }
                public string title { get; set; }
                public string long_title { get; set; }
                public override string ToString()
                {
                    return title + " " + long_title;
                }
            }
        }
    }
}
