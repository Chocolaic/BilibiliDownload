using BiliClient.Utils.Net;
using BiliClient.Utils.Net.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Windows.Threading;

namespace BiliClient.UI
{
    public delegate void tabRemove(TabItem info=null);
    /// <summary>
    /// UserControl_av.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl_av : UserControl
    {
        public event tabRemove rmTabItem;
        private VideoInfo info;

        private bool Liked=false;
        private int Coins = 0;
        private bool Favorited=false;
        public UserControl_av(VideoInfo info)
        {
            InitializeComponent();
            this.info = info;
        }

        private void button_close_Click(object sender, RoutedEventArgs e)
        {
            if (rmTabItem != null)
            {
                if (Reader.HistoryRecord)
                    UserHandler.setHistory(info.aid, info.cid);
                rmTabItem();
            }
        }
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            label_title.Content = info.title;
            label_auth.Content += info.author;
            textBlock.Text = info.description;
            fav_folder.Visibility = Visibility.Hidden;
            Background = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(info.pic))
            };
            grid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A5E5E5E5"));

        }
        private void button_dl_Click(object sender, RoutedEventArgs e)
        {
            setDownload("flv");
        }
        private void button2_dl_Click(object sender, RoutedEventArgs e)
        {
            setDownload("mp4");
        }
        private void button_close_MouseEnter(object sender, MouseEventArgs e)
        {
            button_close.Background = Brushes.Salmon;
        }
        private void button_close_MouseLeave(object sender, MouseEventArgs e)
        {
            button_close.Background = Brushes.White;
        }
        int click_i = 0;
        private void grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            click_i += 1;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            timer.Tick += (s, e1) => { timer.IsEnabled = false; click_i = 0; };
            timer.IsEnabled = true;
            if (click_i % 2 == 0)
            {
                timer.IsEnabled = false;
                click_i = 0;
                hide_control();
            }
        }
        private void setDownload(string format)
        { 
            Window_dl dl = new Window_dl(info,format);
            dl.Show();
        }
        private void hide_control()
        {
            if (label_title.Visibility == Visibility.Visible)
            {
                label_title.Visibility = Visibility.Hidden;
                label_auth.Visibility = Visibility.Hidden;
                button_dl.Visibility = Visibility.Hidden;
                sv.Visibility = Visibility.Hidden;
                fav_folder.Visibility = Visibility.Hidden;
            }
            else
            {
                label_title.Visibility = Visibility.Visible;
                label_auth.Visibility = Visibility.Visible;
                button_dl.Visibility = Visibility.Visible;
                sv.Visibility = Visibility.Visible;
            }
        }

        private void act_like_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!Utils.Session.SessionToken.Logined)
            {
                InteractHandler.UIhandle.Msg("请登陆", "如果您已经登陆，请登出后重试");
                return;
            }

            if (!Liked && UserHandler.sendLike(info.aid))
            {
                act_like.Source = new BitmapImage(ResData.ico_like);
                Liked = true;
            }
        }

        private void act_coin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!Utils.Session.SessionToken.Logined)
            {
                InteractHandler.UIhandle.Msg("请登陆", "如果您已经登陆，请登出后重试");
                return;
            }

            if (Coins<2)
            {
                string msg = String.Empty;
                if (UserHandler.sendCoins(info.aid, info.mid, ref msg) != 0)
                    InteractHandler.UIhandle.Msg("不对哦", msg);
                else
                {
                    act_coin.Source = new BitmapImage(ResData.ico_coin);
                    Utils.Session.SessionToken.info = BlblApi.getUserInfo();
                    Coins += 1;
                }
            }
        }

        private void act_favo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!Utils.Session.SessionToken.Logined)
            {
                InteractHandler.UIhandle.Msg("请登陆", "如果您已经登陆，请登出后重试");
                return;
            }
            if (fav_folder.Visibility == Visibility.Hidden)
            {
                Newtonsoft.Json.Linq.JArray jsonArr = UserHandler.getFavFolder(info.aid);
                fav_panel.Children.Clear();
                for (int i = 0; i < jsonArr.Count; i++)
                {
                    DockPanel dp = new DockPanel();
                    dp.Name = "f"+jsonArr[i]["fid"].ToString();
                    dp.LastChildFill = false;
                    Label lbName = new Label();
                    lbName.Margin = new Thickness(10, 0, 0, 0);
                    lbName.Height = 25;
                    lbName.Content = jsonArr[i]["name"];
                    lbName.FontSize = 13;
                    lbName.FontWeight = FontWeights.Bold;
                    DockPanel.SetDock(lbName, Dock.Top);
                    Label lbID = new Label();
                    lbID.Margin = new Thickness(10, 0, 0, 0);
                    lbID.Content= jsonArr[i]["fid"];
                    DockPanel.SetDock(lbID, Dock.Left);
                    dp.Children.Add(lbName);
                    dp.Children.Add(lbID);
                    fav_panel.Children.Add(dp);
                    dp.MouseDown += fav_folder_MouseDown;
                }
                fav_folder.Visibility = Visibility.Visible;
            }
            else
            {
                fav_folder.Visibility = Visibility.Hidden;
            }
        }

        private void fav_folder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DockPanel dp = (DockPanel)sender;
            string msg = String.Empty;
            if (!Favorited)
            {
                if (UserHandler.setFavo(info.aid, long.Parse(dp.Name.Substring(1)), "add", ref msg) != 0)
                    InteractHandler.UIhandle.Msg("不对哦", msg);
                else
                {
                    act_favo.Source = new BitmapImage(ResData.ico_favo);
                    fav_folder.Visibility = Visibility.Hidden;
                    Favorited = true;
                }
            }
            else
            {
                if (UserHandler.setFavo(info.aid, long.Parse(dp.Name.Substring(1)), "del", ref msg) != 0)
                    InteractHandler.UIhandle.Msg("不对哦", msg);
                else
                {
                    act_favo.Source = new BitmapImage(ResData.ico_ufavo);
                    fav_folder.Visibility = Visibility.Hidden;
                    Favorited = false;
                }
            }
        }
    }
}
