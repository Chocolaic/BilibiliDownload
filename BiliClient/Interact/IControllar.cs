using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliClient.Interact
{
    interface IControllar
    {
        void setDownload(Utils.Net.API.ResInfo info, string path,string name ,string desc, string format);
        void Msg(string title, string msg, Action e = null);
        void setLogin(Utils.Net.API.AccountInfo info);
        void setLogout();
        MahApps.Metro.Controls.FlyoutsControl getFlyoutsControl();
        void createTab(Utils.Net.API.VideoInfo info);
    }
}
