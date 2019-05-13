using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliClient.Utils.Net.API
{
    public class AccountInfo
    {
        public int code { get; set; }
        public Data data { get; set; }
        public class Data
        {
            public long mid { get; set; }
            public string name { get; set; }
            public string face { get; set; }
            public int coin { get; set; }
            public int level { get; set; }
            public int vip_type { get; set; }
            public int following { get; set; }
            public int follower { get; set; }
        }
    }
}
