using BiliClient.Utils.Net.API;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BiliClient.UI
{
    /// <summary>
    /// Window_dl.xaml 的交互逻辑
    /// </summary>
    public partial class Window_dl : MetroWindow
    {
        VideoInfo info;
        public Window_dl(VideoInfo info)
        {
            InitializeComponent();
            this.info = info;
        }
        private void button_dl_Click(object sender, RoutedEventArgs e)
        {
            Interact.InteractHandler.UIhandle.setDownload(Utils.Net.BlblApi.getResInfo(info.aid,info.cid), textBox_fn.Text, info.title,info.description);
            this.Close();
        }
        private void button_view_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Saved As...";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    textBox_fn.Text = dialog.SelectedPath;
                }
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            textBox_fn.Text = Environment.CurrentDirectory + "\\Download\\" + info.aid;
            System.Timers.Timer timer = new System.Timers.Timer(); timer.Interval = 500;
            timer.Elapsed += delegate
            {
                Dispatcher.BeginInvoke(new Action(delegate
                {
                    string filepath = (textBox_fn.Text.EndsWith("\\")) ? textBox_fn.Text + info.aid : textBox_fn.Text + "\\" + info.title;
                    if (File.Exists(filepath))
                        label_file.Content = "文件已存在，将会替换原有文件.";
                    else
                        label_file.Content = "";
                }));
            };
            timer.Start();
        }
    }
}
