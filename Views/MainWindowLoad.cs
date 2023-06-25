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

        /// <summary>
        /// 在窗口的内容呈现完毕后发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WinMain_ContentRendered(object sender, EventArgs e)
        {
            Timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1)
            };
            Timer.Tick += TimeRuner;
            Timer.Start();
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
                _loading = true;
                Console.WriteLine("----------Load----------");
                //载入书籍数据
                Workflow.LoadBooksToBank();
                LoadBookProgressBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                CheckVersion();
                Timer.Stop();
                Console.WriteLine("----------Stop----------");
            }
        }

        private void CheckVersion()
        {
            DateTime now = DateTime.Now;
            string lastCheckHour  = now.ToString("yyyy-MM-dd HH");
            object record = Settings.Get(Gval.MaterialBook, Gval.SettingsKeys.LastCheckHour);
            if (record != null)
            {
                Gval.LastCheckHour = Convert.ToString(record);
            }
            if (Gval.LastCheckHour == lastCheckHour)
            {
                return;
            }
            else
            {
                Gval.LastCheckHour = lastCheckHour;
                Settings.Set(Gval.MaterialBook, Gval.SettingsKeys.LastCheckHour, lastCheckHour);
            }

            StreamReader reader = WebHelper.GetHtmlReaderObject(Gval.Url.Latest);
            if (reader == null)
            {
                Gval.LatestVersion = "网络错误！";
                return;
            }
            else
            {
                string text = reader.ReadToEnd();
                Dictionary<string, object> latestInfo = JsonHelper.Jto<Dictionary<string, object>>(text);
                string versionName = latestInfo["name"].ToString();
                Match match = Regex.Match(text, "\\d+\\.\\d+\\.\\d+\\.\\d+");
                if (match.Success)
                {
                    Gval.LatestVersion = HttpUtility.UrlDecode(match.Value);
                }
            }
            if (Gval.CurrentVersion == Gval.LatestVersion)
            {

            }
            else if (Gval.LatestVersion != "网络错误！" && Gval.LatestVersion != "未检查")
            {
                Gval.HasNewVersion = true;
            }
        }


    }
}
