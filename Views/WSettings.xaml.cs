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
        }
        private void ThisWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WebdavTabTbUrl.Text = Gval.Webdav.Url;
            WebdavTabTbName.Text = Gval.Webdav.UserName;
            WebdavTabTbPassWord.Text = Gval.Webdav.PassWord;
            BtnWebdavTabSave.IsEnabled = false;
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
    }
}
