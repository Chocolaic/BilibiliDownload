using BiliClient.Utils.Net.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliClient.Utils.Session
{
    class SessionToken
    {
        internal static AccountInfo info { get; set; }
        internal static bool Logined { get; set; }
        internal static string access { get; set; }
        internal static string cookies { get; set; }
    }
}
