using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace BiliClient.Utils
{
    public delegate void ConvertCallBack(bool isCompleted);
    class FormatConvert
    {
        string ffmpegCom = "ffmpeg.exe";
        public FormatConvert()
        {
            if (!File.Exists("ffmpeg.exe"))
                InteractHandler.UIhandle.Msg("组件丢失", "将无法完成编码转换,默认flv格式");
        }
        public async void flvToMp4(string videoUrl, ConvertCallBack call)
        {
            string param = string.Format("-i {0} -b 1024k -acodec copy -f mp4 {1}", videoUrl, videoUrl + ".mp4");
            Process p = new Process();
            p.StartInfo.FileName = ffmpegCom;
            p.StartInfo.Arguments = param;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = false;
            await Task.Run(() =>
            {
                p.Start();
                p.WaitForExit();
                p.Close();
                call(true);
            });
        }
    }
}
