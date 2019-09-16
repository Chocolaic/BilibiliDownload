using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliClient.Utils
{
    class FormatConvert
    {
        string ffmpegCom = "ffmpeg.exe";
        public FormatConvert()
        {
            if (!File.Exists("ffmpeg.exe"))
                InteractHandler.UIhandle.Msg("组件丢失", "将无法完成编码转换,默认flv格式");
        }
        public void flvToMp4(string videoUrl)
        {
            string param = string.Format("-i {0} -b 1024k -acodec copy -f mp4 {1}", videoUrl, videoUrl + ".mp4");
            launchProc(param);
        }
        private void launchProc(string param)
        {
            Process p = new Process();
            p.StartInfo.FileName = ffmpegCom;
            p.StartInfo.Arguments = param;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.WaitForExit();
            p.Close();
        }
    }
}
