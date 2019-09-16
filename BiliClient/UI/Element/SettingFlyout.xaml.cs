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
    /// SettingFlyout.xaml 的交互逻辑
    /// </summary>
    public partial class SettingFlyout
    {
        public SettingFlyout()
        {
            DataContext = new ViewModel();
            InitializeComponent();
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.IsOpen = false;
        }
    }
}
