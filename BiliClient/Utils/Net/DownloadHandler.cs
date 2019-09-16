using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BiliClient.Utils.Net
{
    class DownloadHandler
    {
        private Stream so;
        internal bool isWorking { get; set; }
        /// <summary>
        /// Import https://www.cnblogs.com/whboxl/p/7102731.html
        /// </summary>
        /// <param name="url"></param>
        /// <param name="referer"></param>
        /// <param name="filename"></param>
        /// <param name="totalBytes"></param>
        /// <param name="totalDownloadedByte"></param>
        internal void DownloadFile(string url,string referer,string filename, ref long totalBytes, ref long totalDownloadedByte)
        {
            isWorking = true;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.UserAgent = "Mozilla/5.0 BiliClient/1.0";
            req.Accept = "*/*";
            req.Referer = referer;
            HttpWebResponse response = (HttpWebResponse)req.GetResponse();
            totalBytes = response.ContentLength;
            Stream st = response.GetResponseStream();
            so = new FileStream(filename, FileMode.Create);
            byte[] byt = new byte[1024];
            int osize = st.Read(byt, 0, byt.Length);
            while (osize > 0 && isWorking)
            {
                totalDownloadedByte = osize + totalDownloadedByte;
                so.Write(byt, 0, osize);
                osize = st.Read(byt, 0, byt.Length);
            }
            so.Close();
            st.Close();
            isWorking = false;
        }
    }
}
