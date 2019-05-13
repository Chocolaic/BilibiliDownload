using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliClient
{
    class ResData
    {
        internal static Uri banner_login { get; private set; } = new Uri("pack://application:,,,/Resources/banner.jpg");
        internal static Uri ico_unlike { get; private set; } = new Uri("pack://application:,,,/Resources/like_gray.png");
        internal static Uri ico_like { get; private set; } = new Uri("pack://application:,,,/Resources/like_pink.png");
        internal static Uri ico_coin { get; private set; } = new Uri("pack://application:,,,/Resources/coins_pink.png");
        internal static Uri ico_ucoin { get; private set; } = new Uri("pack://application:,,,/Resources/coins_gray.png");
        internal static Uri ico_favo { get; private set; } = new Uri("pack://application:,,,/Resources/favo_pink.png");
        internal static Uri ico_ufavo { get; private set; } = new Uri("pack://application:,,,/Resources/favo_gray.png");
    }
}
