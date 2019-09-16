using BiliClient.Utils.Net;
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
        private int quality { get; set; } = 32;
        private string Format { get; set; }
        private Dictionary<string, int> accept_qn = new Dictionary<string, int>();
        public Window_dl(VideoInfo info,string format="flv")
        {
            InitializeComponent();
            this.info = info;
            this.Format = format;
        }
        private void button_dl_Click(object sender, RoutedEventArgs e)
        {
            InteractHandler.UIhandle.setDownload(BlblApi.getResInfo(info.aid,info.cid,accept_qn[select_quality.SelectedItem.ToString()]), textBox_fn.Text, info.title,info.description, select_format.SelectedItem.ToString());
            BlblApi.DoHTTPDownload(textBox_fn.Text + "\\pic.jpg", info.pic);
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
            this.Dispatcher.BeginInvoke(new Action(delegate
            {
                var _init=Utils.Net.BlblApi.getResInfo(info.aid, info.cid);
                for(int i = 0; i < _init.data.accept_quality.Count; i++)
                {
                    select_quality.Items.Add(_init.data.accept_description[i]);
                    accept_qn.Add(_init.data.accept_description[i], _init.data.accept_quality[i]);
                }
                button_dl.IsEnabled = true;
            }));
            select_format.Items.Add("flv");
            select_format.Items.Add("mp4");
            textBox_fn.Text = Environment.CurrentDirectory + "\\Download\\" + info.aid;
            System.Timers.Timer timer = new System.Timers.Timer(); timer.Interval = 500;
            timer.Elapsed += delegate
            {
                Dispatcher.BeginInvoke(new Action(delegate
                {
                    string filepath = (textBox_fn.Text.EndsWith("\\")) ? textBox_fn.Text + info.aid : textBox_fn.Text + "\\" + info.title;
                    filepath += "."+Format;
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
