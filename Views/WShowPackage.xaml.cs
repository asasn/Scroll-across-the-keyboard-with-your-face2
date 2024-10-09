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

namespace RootNS.Views
{
    /// <summary>
    /// WShowPackage.xaml 的交互逻辑
    /// </summary>
    public partial class WShowPackage : Window
    {
        public WShowPackage()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //获取鼠标位置以设置窗口
            Point point = Mouse.GetPosition(Models.Gval.Views.MainWindow);
            this.Left = point.X - this.ActualWidth * 0.5;
        }
    }
}
