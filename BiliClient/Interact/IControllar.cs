using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliClient.Interact
{
    interface IControllar
    {
        void setDownload(Utils.Net.API.ResInfo info, string path,string name,string desc);
        void Msg(string title, string msg, Action e = null);
    }
}
