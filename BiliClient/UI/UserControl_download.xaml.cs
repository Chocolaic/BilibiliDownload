using BiliClient.Utils;
using BiliClient.Utils.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BiliClient.UI
{
    /// <summary>
    /// UserControl_download.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl_download : UserControl
    {
        public List<DownloadThread> dlList = new List<DownloadThread>();
        public UserControl_download()
        {
            InitializeComponent();
        }
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 500;
            timer.Elapsed += delegate
            {
                Dispatcher.BeginInvoke(new Action(delegate
                {
                    foreach (DownloadThread dl in dlList.ToArray())
                    {
                        if (dl.dlThread.IsAlive)
                        {
                            dl.lbDL.Content = string.Format("{0}Kb/{1}Kb", dl.totalDownloadedByte / 1024, dl.totalBytes / 1024);
                            if (dl.progress.Maximum != dl.totalBytes)
                                dl.progress.Maximum = dl.totalBytes;
                            dl.progress.Maximum = dl.totalBytes;
                            dl.progress.Value = dl.totalDownloadedByte;
                        }
                        else
                        {
                            dl.lbDL.Content = "正在转换编码，可能需要一定时间";
                            convertFormat(dl.filename, dl.format);
                            finDownload(dl.sp, dl.lbDL, dl.progress);
                            dlList.Remove(dl);
                        }
                    }
                }));
            };
            timer.Start();
        }
        private void button_clear_Click(object sender, RoutedEventArgs e)
        {
            if (dlList.Count > 0)
            {
                InteractHandler.UIhandle.Msg("等一下", "你还有没完成的下载，是否关闭下载?", ClearList);
            }
            else
                ClearList();
        }
        internal void ClearList()
        {
            foreach (DownloadThread dl in dlList)
            {
                dl.Disponse();
            }
            dlList.Clear();
            panel.Children.Clear();
        }
        internal void createDownload(string url, string referer, string format, string path, string name, string desc)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (!path.EndsWith("\\"))
                path += "\\";
            createGroupMember(name, desc).createThread(url, path + name, referer, format);
        }
        private void convertFormat(string filename, string format)
        {
            FormatConvert fc = new FormatConvert();
            switch (format)
            {
                case "flv":
                    File.Move(filename, filename + ".flv");
                    break;
                case "mp4":
                    fc.flvToMp4(filename);
                    break;
            }
        }
        private void finDownload(StackPanel sp, Label lbDL, ProgressBar progress)
        {
            lbDL.Content = "下载完成";
            sp.Children.Remove(progress);
        }
        private DownloadThread createGroupMember(string name, string desc)
        {
            Label lbDL = new Label();
            StackPanel sp = new StackPanel();
            ProgressBar progress = new ProgressBar();
            DockPanel dp = new DockPanel();
            dp.LastChildFill = false;
            //Title
            Label lblName = new Label();
            lblName.Margin = new Thickness(15, 0, 0, 0);
            lblName.Height = 25;
            lblName.Content = name;
            lblName.FontSize = 13;
            lblName.FontWeight = FontWeights.Bold;
            DockPanel.SetDock(lblName, Dock.Top);
            //Describe
            Label lblResume = new Label();
            lblResume.Margin = new Thickness(15, 0, 0, 0);
            desc = desc.Replace("\n", "");
            if (desc.Length > 30)
                lblResume.Content = desc.Substring(0, 30) + "...";
            else
                lblResume.Content = desc;
            DockPanel.SetDock(lblResume, Dock.Left);
            //DownloadInfo
            lbDL.Margin = new Thickness(80, 0, 0, 0);
            lbDL.Content = "0Kb/0Kb";
            //DockPanel.SetDock(lbDL, Dock.Bottom);
            
            progress.Height = 2;
            progress.VerticalAlignment = VerticalAlignment.Bottom;

            dp.Children.Add(lblName);
            dp.Children.Add(lblResume);
            dp.Children.Add(lbDL);

            sp.Children.Add(dp);
            sp.Children.Add(progress);
            panel.Children.Add(sp);

            DownloadThread dl = new DownloadThread(lbDL, progress, sp);
            dlList.Add(dl);
            return dl;
        }

        public class DownloadThread
        {
            public long totalBytes = 0;
            public long totalDownloadedByte = 0;
            public string format="flv";
            public string filename;
            public Thread dlThread;
            public Label lbDL;
            public StackPanel sp;
            public ProgressBar progress;
            internal DownloadHandler handle;
            public DownloadThread(Label lbDL, ProgressBar progress, StackPanel sp)
            {
                this.lbDL = lbDL;
                this.progress = progress;
                this.sp = sp;
                handle = new DownloadHandler();
            }
            public void createThread(string url, string filename, string referer,string format)
            {
                this.format = format;
                this.filename = filename;
                dlThread = new Thread(new ThreadStart(() =>
                {
                    handle.DownloadFile(url, referer, filename, ref totalBytes, ref totalDownloadedByte);
                }));
                dlThread.Start();
            }
            public void Disponse()
            {
                handle.isWorking = false;
            }
        }
    }
}
