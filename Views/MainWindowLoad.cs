using RootNS.Helper;
using RootNS.Models;
using RootNS.MyControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
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
using VerifyLib;

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
        private void GboxTree_Loaded(object sender, RoutedEventArgs e)
        {
            Gval.Views.GboxTree = sender as GroupBox;
        }

        private void GboxWork_Loaded(object sender, RoutedEventArgs e)
        {
            Gval.Views.GboxWork = sender as GroupBox;
        }
        private void LbShowNoVerify_Loaded(object sender, RoutedEventArgs e)
        {
            Gval.Views.LbShowNoVerify = sender as Label;
        }
        private void BorderR_Loaded(object sender, RoutedEventArgs e)
        {
            Gval.Views.BorderR = sender as Border;
        }

        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            Gval.Views.EditorTabControl = sender as HandyControl.Controls.TabControl;
        }
        private void UcShower_Loaded(object sender, RoutedEventArgs e)
        {
            Gval.Views.UcShower = sender as UcShower;
        }
        private void UcSearcher_Loaded(object sender, RoutedEventArgs e)
        {
            Gval.Views.UcSearcher = sender as UcSearcher;
        }
        private void ChapterTree_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void NotesTree_Loaded(object sender, RoutedEventArgs e)
        {
            Gval.Views.UcNotesTree = sender as MyTree;
        }

        private void MaterialsTree_Loaded(object sender, RoutedEventArgs e)
        {
            Gval.Views.UcMaterials = sender as MyTree;
        }
        private void BtnSettings_Loaded(object sender, RoutedEventArgs e)
        {
            Gval.Views.BtnSettings = sender as Button;
        }

        /// <summary>
        /// 在窗口的内容呈现完毕后发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WinMain_ContentRendered(object sender, EventArgs e)
        {
            //Timer = new DispatcherTimer
            //{
            //    Interval = TimeSpan.FromMilliseconds(1)
            //};
            //Timer.Tick += TimeRuner;
            //Timer.Start();

            LoadVerify();
            Workflow.LoadBooksToBank();
            LoadBookProgressBar.Visibility = Visibility.Collapsed;
        }

        private DispatcherTimer Timer = new DispatcherTimer();

        static bool _loading = false;
        /// <summary>
        /// 方法：每次间隔运行的内容
        /// </summary>
        private void TimeRuner(object sender, EventArgs e)
        {
            if (_loading == false)
            {
                Console.WriteLine("----------Load----------");
                //载入书籍数据
                Workflow.LoadBooksToBank();
                _loading = true;
                LoadBookProgressBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                Console.WriteLine("----------Stop----------");
                Console.WriteLine("----------版本检查----------");
                FunctionsPack.CheckVersion();
                Timer.Stop();
            }
        }

        /// <summary>
        /// 防君子不防小人的随缘验证
        /// </summary>
        private static void LoadVerify()
        {
            if (Gval.ShowNoVerify == true)
            {
                Gval.Views.GboxTree.IsEnabled = false;
                Gval.Views.GboxWork.IsEnabled = false;
                Gval.Views.LbShowNoVerify.Visibility = Visibility.Visible;
            }
            else
            {
                Gval.Views.GboxTree.IsEnabled = true;
                Gval.Views.GboxWork.IsEnabled = true;
                Gval.Views.LbShowNoVerify.Visibility = Visibility.Collapsed;
            }
        }

        


    }
}
