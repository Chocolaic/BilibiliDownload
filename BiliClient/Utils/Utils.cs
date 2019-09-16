using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliClient.Utils
{
    class Utils
    {
        internal static long getUnixStamp { get
            {
                TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                return Convert.ToInt64(ts.TotalSeconds);
            } }
        internal static string errorMsg(int code)
        {
            switch (code)
            {
                case -404:
                    return "404 NOT FOUND";
                case 62002:
                    return "视频找不到了";
                default:
                    return "";
            }
        }
    }
}
