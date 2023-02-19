using RootNS.Helper;
using RootNS.Models;
using RootNS.MyControls;
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

namespace RootNS
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataEntrance();
        }

        /// <summary>
        /// 自定义的程序数据入口
        /// </summary>
        private void DataEntrance()
        {
            //------------------程序入口------------------

            //读取和应用一些设置
            //-------------------------------------------
            Workflow.Start();

            //改变动态资源当中的颜色主题
            //-------------------------------------------
            try
            {
                var brushConverter = new BrushConverter();
                var color = Settings.Get(Gval.MaterialBook, Gval.SettingsKeys.CurrentThemeColor);
                SolidColorBrush solidColorBrush = (SolidColorBrush)brushConverter.ConvertFrom(color);
                Gval.CurrentThemeColor = solidColorBrush;
                Brush brush2 = new SolidColorBrush(Color.FromArgb(solidColorBrush.Color.A,
                    (byte)(255 - solidColorBrush.Color.R),
                    (byte)(255 - solidColorBrush.Color.G),
                    (byte)(255 - solidColorBrush.Color.B)));
                Resources["PrimaryBrush"] = (SolidColorBrush)brush2;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                BtnThemeColor_MouseRightButtonDown(BtnThemeColor, null);
            }
            //-------------------------------------------
        }


        private void WinMain_Loaded(object sender, RoutedEventArgs e)
        {
            Gval.Views.MainWindow = this;
        }

        private void BorderR_Loaded(object sender, RoutedEventArgs e)
        {
            Gval.Views.BorderR = sender as Border;
        }

        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            Gval.Views.EditorTabControl = sender as HandyControl.Controls.TabControl;
        }

        private void UcSearcher_Loaded(object sender, RoutedEventArgs e)
        {
            Gval.Views.UcSearcher = sender as UcSearcher;
        }
        private void MyTree_Loaded(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 在窗口的内容呈现完毕后发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WinMain_ContentRendered(object sender, EventArgs e)
        {
            //载入书籍数据
            Workflow.LoadBooksToBank();
        }

    }
}
