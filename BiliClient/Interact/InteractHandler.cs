using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BiliClient.Interact
{
    class InteractHandler
    {
        internal static IControllar UIhandle { get; set; }
        internal static void URLConvert(string url,ref long avNum)
        {
            string Str = url.Trim().ToLower();
            Regex regex;
            if (Str.StartsWith("http"))
            {
                regex = new Regex(@"https?://www.bilibili.com/video/av(\d*)", RegexOptions.IgnoreCase);
            }else if (Str.StartsWith("av"))
            {
                regex = new Regex(@"av(\d*)", RegexOptions.IgnoreCase);
            }
            else
            {
                regex = new Regex(@"(\d*)", RegexOptions.IgnoreCase);
            }
            avNum = long.Parse(regex.Match(url).Result("$1"));

        }
    }
}
