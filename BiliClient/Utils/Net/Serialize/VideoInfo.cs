namespace BiliClient.Utils.Net.API
{
    public class VideoInfo
    {
        public int code { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
        public class Data
        {
            public long aid { get; set; }
            public string title { get; set; }
            public string desc { get; set; }
            public string pic { get; set; }
            public Owner owner { get; set; }
            public long cid { get; set; }
            public Season season { get; set; }
            public class Owner
            {
                public long mid { get; set; }
                public string name { get; set; }
                public string face { get; set; }
            }
            public class Season
            {
                public string season_id { get; set; }
                public string newest_ep_id { get; set; }
                public string title { get; set; }
                public string cover { get; set; }
                public string is_finish { get; set; }
            }
        }
    }
}
