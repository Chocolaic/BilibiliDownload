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

namespace BiliClient.UI.Element
{
    /// <summary>
    /// AccountFlyout.xaml 的交互逻辑
    /// </summary>
    public partial class AccountFlyout
    {
        public AccountFlyout()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InteractHandler.UIhandle.setLogout();
            this.IsOpen = false;
        }

        private void btn_history_Click(object sender, RoutedEventArgs e)
        {
            MahApps.Metro.Controls.FlyoutsControl control = InteractHandler.UIhandle.getFlyoutsControl();
            MahApps.Metro.Controls.Flyout tab_history = new MahApps.Metro.Controls.Flyout() {
                Header = "历史记录",
                Width = 300,
                Position = MahApps.Metro.Controls.Position.Right,
                Theme = MahApps.Metro.Controls.FlyoutTheme.Adapt
            };
            tab_history.Loaded += (object s, RoutedEventArgs args) =>
            {
                Grid grid = new Grid();
                ScrollViewer sv = new ScrollViewer();
                ListBox lb = new ListBox();
                HistoryInfo info = BlblApi.getHistory();
                foreach (var v in info.data.list)
                {
                    System.Windows.Controls.Primitives.UniformGrid ug = new System.Windows.Controls.Primitives.UniformGrid();
                    ug.Columns = 2;
                    DockPanel dp = new DockPanel();
                    Label desc = new Label() { Margin = new Thickness(0, 0, 0, 0), FontSize = 15, FontWeight = FontWeights.Bold, Content = v.name };
                    Label title = new Label() { Margin = new Thickness(0, 0, 0, 0), Content = (v.title.Length >= 10) ? v.title.Substring(0, 10) : v.title };
                    DockPanel.SetDock(title, Dock.Top);

                    Image img = new Image() { Width = 80, Height = 50, Margin = new Thickness(0, 0, 0, 0) };
                    BitmapImage imgSrc = new BitmapImage();
                    img.Source = new BitmapImage(new Uri(string.Format("{0}@{1}w_{2}h", v.cover, img.Width, img.Height)));

                    dp.Children.Add(title);
                    dp.Children.Add(desc);

                    ug.Children.Add(img);
                    ug.Children.Add(dp);
                    lb.Items.Add(ug);
                    ug.MouseDown += (object ms, MouseButtonEventArgs me) =>
                    {
                        VideoInfo vi = BlblApi.getVideoInfo(v.history.oid);
                        vi.aid = v.history.oid;
                        InteractHandler.UIhandle.createTab(vi);
                        tab_history.IsOpen = false;

                    };
                }
                sv.Content = lb;
                grid.Children.Add(sv);
                tab_history.Content = grid;
            };
            tab_history.IsOpenChanged += async (object os, RoutedEventArgs oe) =>
            {
                if (!tab_history.IsOpen)
                {
                    await Task.Delay(100);
                    control.Items.Remove(tab_history);
                }
            };
            control.Items.Add(tab_history);
            this.IsOpen = false;
            tab_history.IsOpen = true;
        }

        private void btn_favorite_Click(object sender, RoutedEventArgs e)
        {
            MahApps.Metro.Controls.FlyoutsControl control = InteractHandler.UIhandle.getFlyoutsControl();
            MahApps.Metro.Controls.Flyout tab_favorite = new MahApps.Metro.Controls.Flyout()
            {
                Header="我的收藏",
                Width=300,
                Position = MahApps.Metro.Controls.Position.Right,
                Theme = MahApps.Metro.Controls.FlyoutTheme.Adapt
            };
            tab_favorite.Loaded += (object ls, RoutedEventArgs le) =>
            {
                Grid grid = new Grid();
                ScrollViewer sv = new ScrollViewer();
                TreeView tv = new TreeView();

                var json = UserHandler.getFavFolder(0);
                for(int i = 0; i < json.Count; i++)
                {
                    TreeViewItem item = new TreeViewItem() { Header = json[i]["name"] };
                    var info = UserHandler.getFavo(long.Parse(json[i]["fid"].ToString()));
                    foreach(var v in info.data.items)
                    {
                        System.Windows.Controls.Primitives.UniformGrid ug = new System.Windows.Controls.Primitives.UniformGrid() { Columns = 2 };
                        DockPanel dp = new DockPanel();
                        Label desc = new Label() { Margin = new Thickness(0, 0, 0, 0), FontSize = 15, FontWeight = FontWeights.Bold, Content = v.name };
                        Label title = new Label() { Margin = new Thickness(0, 0, 0, 0), Content = (v.title.Length >= 10) ? v.title.Substring(0, 10) : v.title };
                        DockPanel.SetDock(title, Dock.Top);

                        Image img = new Image() { Width = 80, Height = 50, Margin = new Thickness(0, 0, 0, 0) };
                        img.Source = new BitmapImage(new Uri(string.Format("{0}@{1}w_{2}h", v.pic, img.Width, img.Height)));
                        dp.Children.Add(title);
                        dp.Children.Add(desc);

                        ug.Children.Add(img);
                        ug.Children.Add(dp);
                        item.Items.Add(ug);
                        ug.MouseDown += (object ms, MouseButtonEventArgs me) =>
                        {
                            VideoInfo vi = BlblApi.getVideoInfo(v.aid);
                            if (vi.code == 0)
                            {
                                vi.aid = v.aid;
                                InteractHandler.UIhandle.createTab(vi);
                            }
                            else
                                InteractHandler.UIhandle.Msg("出..出错了", Utils.Utils.errorMsg(vi.code));
                            tab_favorite.IsOpen = false;

                        };
                    }
                    tv.Items.Add(item);
                }
                sv.Content = tv;
                grid.Children.Add(sv);
                tab_favorite.Content = grid;
            };
            tab_favorite.IsOpenChanged += async (object os, RoutedEventArgs oe) =>
            {
                if (!tab_favorite.IsOpen)
                {
                    await Task.Delay(100);
                    control.Items.Remove(tab_favorite);
                }
            };
            control.Items.Add(tab_favorite);
            this.IsOpen = false;
            tab_favorite.IsOpen = true;
        }
    }
}
