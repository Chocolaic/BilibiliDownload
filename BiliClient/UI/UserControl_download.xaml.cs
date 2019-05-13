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
                            finDownload(dl.dp, dl.lbDL, dl.progress);
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
                Interact.InteractHandler.UIhandle.Msg("等一下", "你还有没完成的下载，是否关闭下载?", ClearList);
            }
            else
                ClearList();
        }
        private void ClearList()
        {
            foreach (DownloadThread dl in dlList)
            {
                dl.dlThread.Abort();
            }
            dlList.Clear();
            panel.Children.Clear();
        }
        internal void createDownload(string url, string referer, string path, string name, string desc)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (!path.EndsWith("\\"))
                path += "\\";
            createGroupMember(name, desc).createThread(url, path + name, referer);
        }
        private void finDownload(DockPanel dp, Label lbDL, ProgressBar progress)
        {
            lbDL.Content = "下载完成";
            dp.Children.Remove(progress);
        }
        private DownloadThread createGroupMember(string name, string desc)
        {
            Label lbDL = new Label();
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
            progress.Width = sv.Width;
            progress.Height = 2;
            progress.Margin = new Thickness(-300, 0, 0, 0);
            DockPanel.SetDock(progress, Dock.Bottom);
            dp.Children.Add(lblName);
            dp.Children.Add(lblResume);
            dp.Children.Add(progress);
            dp.Children.Add(lbDL);
            panel.Children.Add(dp);

            DownloadThread dl = new DownloadThread(lbDL, progress, dp);
            dlList.Add(dl);
            return dl;
        }

        public class DownloadThread
        {
            public long totalBytes = 0;
            public long totalDownloadedByte = 0;
            public Thread dlThread;
            public Label lbDL;
            public ProgressBar progress;
            public DockPanel dp;
            DownloadHandler handle;
            public DownloadThread(Label lbDL, ProgressBar progress, DockPanel dp)
            {
                this.lbDL = lbDL;
                this.progress = progress;
                this.dp = dp;
                handle = handle = new DownloadHandler();
            }
            public void createThread(string url, string filename, string referer)
            {
                dlThread = new Thread(new ThreadStart(() =>
                {
                    handle.DownloadFile(url, referer, filename, ref totalBytes, ref totalDownloadedByte);
                }));
                dlThread.Start();
            }
        }
    }
}
