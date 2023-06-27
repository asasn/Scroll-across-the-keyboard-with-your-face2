using RootNS.Helper;
using RootNS.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace RootNS.Views
{
    /// <summary>
    /// WSettings.xaml 的交互逻辑
    /// </summary>
    public partial class WSettings : Window
    {
        public WSettings()
        {
            InitializeComponent();


            if (Gval.CurrentVersion == Gval.LatestVersion)
            {
                labelTip.Content = "已是最新版本";
                BtnCheckVersion.Visibility = Visibility.Hidden;
            }
            else if (Gval.LatestVersion != "网络错误！" && Gval.LatestVersion != "未检查")
            {
                labelTip.Foreground = new SolidColorBrush(Colors.DodgerBlue);
                labelTip.Content = "有新版本，请打开GitHub仓库以更新";
                BtnCheckVersion.Visibility = Visibility.Hidden;
            }
        }
        private void ThisWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WebdavTabTbUrl.Text = Gval.Webdav.Url;
            WebdavTabTbName.Text = Gval.Webdav.UserName;
            WebdavTabTbPassWord.Text = Gval.Webdav.PassWord;
            BtnWebdavTabSave.IsEnabled = false;
            Gval.Views.WSettings = sender as Window;
        }
        private void WebdavTabTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            BtnWebdavTabSave.IsEnabled = true;
        }

        private void BtnWebdavTabSave_Click(object sender, RoutedEventArgs e)
        {
            Gval.Webdav.Url = WebdavTabTbUrl.Text;
            Gval.Webdav.UserName = WebdavTabTbName.Text;
            Gval.Webdav.PassWord = WebdavTabTbPassWord.Text;
            Settings.Set(Gval.MaterialBook, Gval.SettingsKeys.WebdavUrl, Gval.Webdav.Url);
            Settings.Set(Gval.MaterialBook, Gval.SettingsKeys.WebdavUserName, Gval.Webdav.UserName);
            Settings.Set(Gval.MaterialBook, Gval.SettingsKeys.WebdavPassWord, Gval.Webdav.PassWord);
            BtnWebdavTabSave.IsEnabled = false;
        }

        private void Rb1_Checked(object sender, RoutedEventArgs e)
        {
            Gval.EditorSettings.CursorToEnd = false;
            Settings.Set(Gval.MaterialBook, Gval.SettingsKeys.CursorToEnd, Gval.EditorSettings.CursorToEnd);
        }

        private void Rb2_Checked(object sender, RoutedEventArgs e)
        {
            Gval.EditorSettings.CursorToEnd = true; 
            Settings.Set(Gval.MaterialBook, Gval.SettingsKeys.CursorToEnd, Gval.EditorSettings.CursorToEnd);
        }

        private void CursorToEndSetting_Loaded(object sender, RoutedEventArgs e)
        {
            rb1.IsChecked = !Gval.EditorSettings.CursorToEnd;
            rb2.IsChecked = Gval.EditorSettings.CursorToEnd;
        }

        private void BtnCheckVersion_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            BtnCheckVersion.Visibility = Visibility.Hidden;            
            StreamReader reader = WebHelper.GetHtmlReaderObject(Gval.Url.Latest);
            if (reader == null)
            {
                Gval.LatestVersion = "网络错误！";
                labelTip.Content = "网络错误，请稍等片刻之后再次尝试！";
                (sender as Button).IsEnabled = true;
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
                labelTip.Content = "已是最新版本";
            }
            else if (Gval.LatestVersion != "网络错误！" && Gval.LatestVersion != "未检查")
            {
                labelTip.Foreground = new SolidColorBrush(Colors.DodgerBlue);
                labelTip.Content = "有新版本，请打开GitHub仓库以更新";
                Gval.HasNewVersion = true;
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Gval.Url.HomePage);
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
        }
    }
}
