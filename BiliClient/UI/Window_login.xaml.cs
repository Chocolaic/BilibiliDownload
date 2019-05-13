using BiliClient.Utils.Net;
using BiliClient.Utils.Session;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
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
using System.Windows.Shapes;

namespace BiliClient.UI
{
    public delegate void LoginHander(Utils.Net.API.AccountInfo info);
    /// <summary>
    /// Window_login.xaml 的交互逻辑
    /// </summary>
    public partial class Window_login : MetroWindow
    {
        public event LoginHander setLogin;
        LoginHandler handle;
        public Window_login()
        {
            InitializeComponent();
            handle = new LoginHandler();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Background = new ImageBrush
            {
                ImageSource = new BitmapImage(ResData.banner_login)
            };
            ReloadCaptcha();        
        }

        private void btn_login_Click(object sender, RoutedEventArgs e)
        {
            label_username.Content = ""; label_passwd.Content = ""; label_captcha.Content = "";
            if (textBox_username.Text=="" || textBox_password.Text=="" || textBox_captcha.Text == "")
            {
                if (textBox_username.Text == "")
                    label_username.Content = "你还没输入账号！";
                if (textBox_password.Text == "")
                    label_passwd.Content = "密码不能少";
                if (textBox_captcha.Text == "")
                    label_captcha.Content = "我是验证码";
            }
            else
            {
                Utils.Net.API.LoginInfo info =handle.getLogin(textBox_username.Text, textBox_password.Text, textBox_captcha.Text);
                if (info.code != 0)
                {
                    switch (info.code)
                    {
                        case -105:
                            label_captcha.Content = "验证码不对";
                            return;
                        case -629:
                            label_passwd.Content = "账号或者密码错误";
                            return;
                    }
                }
                else
                {
                    SessionToken.Logined = true;
                    SessionToken.access = info.data.token_info.access_token;
                    if (setLogin != null)
                    {
                        SessionToken.info = BlblApi.getUserInfo();
                        setLogin(SessionToken.info);
                        this.Close();
                    }
                    
                }
            }

        }

        private async void ReloadCaptcha()
        {
            await Dispatcher.BeginInvoke(new Action(delegate
            {
                img_captcha.BeginInit();
                img_captcha.Source = handle.getCaptcha();
                img_captcha.EndInit();
            }));
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            
        }

        private void img_captcha_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ReloadCaptcha();
        }
    }
}
