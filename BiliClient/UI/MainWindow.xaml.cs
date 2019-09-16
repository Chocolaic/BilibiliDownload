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
using MahApps.Metro.Controls;

using MahApps.Metro.Controls.Dialogs;
using BiliClient.Utils.Session;
using BiliClient.UI.Element;

namespace BiliClient.UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow, Interact.IControllar
    {
        UserControl_search ui_search;
        UserControl_download ui_dl;

        SettingFlyout tab_setting;
        AccountFlyout tab_account;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InteractHandler.UIhandle = this;
            ui_search = new UserControl_search();
            ui_dl = new UserControl_download();
            tabItem_main.Content=ui_search;
            tabItem_download.Content = ui_dl;
            InitFlyouts();         
            this.Dispatcher.BeginInvoke(new Action(delegate
            {
                Reader.Init();
                SessionCache.LoadUserInfo();
            }));

        }
        private void InitFlyouts()
        {
            tab_setting = new SettingFlyout();
            flyoutsControl.Items.Add(tab_setting);
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ui_dl.dlList.Count > 0)
            {
                e.Cancel = true;
                Msg("等一下", "你还有未完成的下载，是否退出?", close);
            }
        }
        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public void createTab(Utils.Net.API.VideoInfo info)
        {
            TabItem item = new TabItem();
            string title = info.title.Replace(" ", "");
            item.Header = (title.Length > 10) ? title.Substring(0, 10) : title;
            UserControl_av ui_av = new UserControl_av(info);
            ui_av.rmTabItem += removeTab;
            item.Content = ui_av;
            tabControl.Items.Add(item);
            tabControl.SelectedItem = item;
        }
        public void setLogin(Utils.Net.API.AccountInfo info)
        {
            tab_account = new AccountFlyout { Header = info.data.name };
            btn_login.Content = info.data.name;
            flyoutsControl.Items.Add(tab_account);
            tab_account.image.Source = new BitmapImage(new Uri(info.data.face));
        }
        public FlyoutsControl getFlyoutsControl()
        {
            return flyoutsControl;
        }

        private void removeTab(TabItem item=null)
        {
            if (item == null)
                tabControl.Items.Remove(tabControl.SelectedItem);
            else
                tabControl.Items.Remove(item);
        }

        private void Btn_Login_Click(object sender, RoutedEventArgs e)
        {
            if (Utils.Session.SessionToken.Logined != true)
            {
                Window_login log = new Window_login();
                log.Show();
            }
            else
            {
                if (tab_account.IsOpen == false)
                {
                    tab_account.account_level.Content = string.Format("等级：{0}\r\n硬币：{1}\r\n关注：{2}", SessionToken.info.data.level, SessionToken.info.data.coin, SessionToken.info.data.following);
                }
                tab_account.IsOpen = !tab_account.IsOpen;
            }
        }

        public void setLogout()
        {
            Utils.Session.SessionToken.Logined = false;
            if (System.IO.File.Exists("user.xml"))
                System.IO.File.Delete("user.xml");
            btn_login.Content = "请登陆";

        }
        public void setDownload(Utils.Net.API.ResInfo info, string path, string name, string desc, string format)
        {
            ui_dl.createDownload(info.data.durl[0].url, info.referer, format, path, name, desc);
            tabControl.SelectedItem = tabItem_download;
        }
        
        private void close()
        {
            ui_dl.ClearList();
            Application.Current.Shutdown();
        }
        public async void Msg(string title, string msg,Action e=null)
        {
            if (e != null)
            {
                MessageDialogResult result = await this.ShowMessageAsync(title, msg, MessageDialogStyle.AffirmativeAndNegative);
                if (result == MessageDialogResult.Affirmative)
                    e();
            }
            else
            {
                await this.ShowMessageAsync(title, msg);
            }
        }

        private void btn_setting_Click(object sender, RoutedEventArgs e)
        {
            tab_setting.IsOpen=!tab_setting.IsOpen;
        }
    }
}
