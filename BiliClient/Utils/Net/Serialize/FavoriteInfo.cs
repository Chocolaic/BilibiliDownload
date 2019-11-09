using System.Collections.Generic;

namespace BiliClient.Utils.Net.API
{
    public class FavoriteInfo
    {
        public int code { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
        public class Data
        {
            public int count { get; set; }
            public List<Item> items { get; set; }
        }
        public class Item
        {
            public long aid { get; set; }
            public string title { get; set; }
            public string pic { get; set; }
            public string name { get; set; }
        }
    }
}
