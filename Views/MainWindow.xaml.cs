using RootNS.Helper;
using RootNS.Models;
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
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //------------------程序入口------------------
            if (FunctionsPack.IsInDesignMode(this))
            {
                return;
            }
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

        /// <summary>
        /// 点击：恢复默认的主面板布局
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReLayout_Click(object sender, RoutedEventArgs e)
        {
            ColTree.Width = new GridLength(240, GridUnitType.Pixel);
            ColEditor.Width = new GridLength(700, GridUnitType.Pixel);
        }

        private void BtnThemeColor_Click(object sender, RoutedEventArgs e)
        {
            Random ran = new Random();
            byte a = (byte)(ran.Next(180, 256));
            byte r = (byte)(ran.Next(0, 256));
            byte g = (byte)(ran.Next(0, 256));
            byte b = (byte)(ran.Next(0, 256));
            Brush brush1 = new SolidColorBrush(Color.FromArgb(a, r, g, b));
            Gval.CurrentThemeColor = (SolidColorBrush)brush1;
            Brush brush2 = new SolidColorBrush(Color.FromArgb(a,
                (byte)(255 - r), (byte)(255 - g), (byte)(255 - b)));
            Resources["PrimaryBrush"] = (SolidColorBrush)brush2;
            Settings.Set(Gval.MaterialBook, Gval.SettingsKeys.CurrentThemeColor, brush1.ToString());
        }

        private void BtnThemeColor_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Brush brush2 = new SolidColorBrush(Color.FromArgb(Gval.DefaultThemeColor.Color.A,
                (byte)(255 - Gval.DefaultThemeColor.Color.R),
                (byte)(255 - Gval.DefaultThemeColor.Color.G),
                (byte)(255 - Gval.DefaultThemeColor.Color.B)));
            Resources["PrimaryBrush"] = (SolidColorBrush)brush2;
            Gval.CurrentThemeColor = Gval.DefaultThemeColor;
            Settings.Set(Gval.MaterialBook, Gval.SettingsKeys.CurrentThemeColor, Gval.DefaultThemeColor.ToString());
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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Settings.Set(Gval.MaterialBook, Gval.SettingsKeys.CurrentBookGuid, Gval.CurrentBook.Guid);
            Gval.CurrentBook.Load();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void BtnBankManage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAddABook_Click(object sender, RoutedEventArgs e)
        {
            Workflow.CreadANewBook();
        }

        private void BtnDelCurrentBook_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnEditCurrentBookInfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RbHistory_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void RbStory_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void RbCardsInBook_Checked(object sender, RoutedEventArgs e)
        {

        }


        private void RbCards_Checked(object sender, RoutedEventArgs e)
        {

        }


        private void RbSnippetsInBook_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void RbSnippets_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void RbMaterial_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void RbInspiration_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void RbTopic_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void RbPlotDesign_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void RbPlotDesignInBook_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Gval.Views.EditorTabControl != null)
            {
                while (Gval.Views.EditorTabControl.Items.Count > 0)//集合可能改变，故而不需要i++之类的条件
                {
                    HandyControl.Controls.TabItem tabItem = Gval.Views.EditorTabControl.Items[Gval.Views.EditorTabControl.Items.Count - 1] as HandyControl.Controls.TabItem;
                    tabItem.Focus();
                    Workflow.FindByName(tabItem.CommandBindings, "Close").Execute(tabItem);
                }
            }
            foreach (SqliteHelper cSqlite in SqliteHelper.PoolDict.Values)
            {
                cSqlite.Close();
            }
            if (MyControls.FindReplaceDialog.theDialog != null)
            {
                MyControls.FindReplaceDialog.theDialog.Close();
            }
            Application.Current.Shutdown(0);
        }


    }
}
