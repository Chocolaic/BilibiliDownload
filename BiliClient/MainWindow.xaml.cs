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

using BiliClient.UI;
using MahApps.Metro.Controls.Dialogs;
using BiliClient.Utils.Session;

namespace BiliClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow, Interact.IControllar
    {
        UserControl_search ui_search;
        UserControl_download ui_dl;
        UserControl_setting ui_setting;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Interact.InteractHandler.UIhandle = this;
            ui_search = new UserControl_search();
            ui_dl = new UserControl_download();
            ui_setting = new UserControl_setting();
            ui_search.addTabItem += createTab;
            tabItem_main.Content=ui_search;
            tabItem_download.Content = ui_dl;
            tabItem_setting.Content = ui_setting;
            accountBoard.Visibility = Visibility.Hidden;
            
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            closeCheck();
        }
        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void createTab(Utils.Net.API.VideoInfo info)
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
        private void setLogin(Utils.Net.API.AccountInfo info)
        {
            btn_login.Content = info.data.name;
            account_img.Source = new BitmapImage(new Uri(info.data.face));
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
                log.setLogin += setLogin;
                log.Show();
            }
            else
            {
                if (accountBoard.Visibility == Visibility.Hidden)
                {
                    account_level.Content = string.Format("等级：{0}\r\n硬币：{1}", SessionToken.info.data.level, SessionToken.info.data.coin);
                    accountBoard.Visibility = Visibility.Visible;
                }
                else
                    accountBoard.Visibility = Visibility.Hidden;
            }
        }

        private void account_logout_Click(object sender, RoutedEventArgs e)
        {
            accountBoard.Visibility = Visibility.Hidden;
            Utils.Session.SessionToken.Logined = false;
            btn_login.Content = "请登陆";
        }
        public void setDownload(Utils.Net.API.ResInfo info, string path, string name, string desc)
        {
            ui_dl.createDownload(info.data.durl[0].url, info.referer, path, name, desc);
            tabControl.SelectedItem = tabItem_download;
        }
        private void closeCheck()
        {
            if (ui_dl.dlList.Count > 0)
            {
                Msg("等一下", "你还有未完成的下载，是否退出?", close);
            }
            else
                close();
        }
        private void close()
        {
            Environment.Exit(0);
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
    }
}
