using BiliClient.Utils.Net.API;

namespace BiliClient.Utils.Session
{
    class SessionToken
    {
        internal static AccountInfo info { get; set; }
        internal static bool Logined { get; set; }
        internal static string access { get; set; }
        internal static LoginInfo.Data.Cookie_info cookies { get; set; }
    }
}
