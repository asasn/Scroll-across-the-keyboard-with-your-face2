using RootNS.Helper;
using RootNS.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using VerifyLib;

namespace RootNS.Views
{
    /// <summary>
    /// WHelp.xaml 的交互逻辑
    /// </summary>
    public partial class WRegister : Window
    {
        public WRegister()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BtnGetVerifyCode_Click(null, null);
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //左键按下时，允许拖曳
            DragMove();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnGetMachineId_Click(object sender, RoutedEventArgs e)
        {
            TbMachineId.Text = VerifyHelper.GetMachineCode();
            BtnGetMachineId.IsEnabled = false;
        }
        private void BtnCopyMachineId_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(TbMachineId.Text.ToString());
            HandyControl.Controls.Growl.SuccessGlobal("已复制机器码到剪贴板！");
        }
        private void BtnGetVerifyCode_Click(object sender, RoutedEventArgs e)
        {
            TbVerify.Text = Gval.VerifyCode;
        }

        private void BtnVerify_Click(object sender, RoutedEventArgs e)
        {
            int n = VerifyHelper.VerifyCode(TbVerify.Text, true);
            if (n == 0)
            {
                Settings.Set(Gval.MaterialBook, Gval.SettingsKeys.VerifyCode, TbVerify.Text);
                Gval.Views.GboxTree.IsEnabled = true;
                Gval.Views.GboxWork.IsEnabled = true;
                Gval.Views.LbShowNoVerify.Visibility = Visibility.Collapsed;
                Gval.ShowNoVerify = false;
                Settings.Set(Gval.MaterialBook, Gval.SettingsKeys.ShowNoVerify, Gval.ShowNoVerify);
            }
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            //Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WSettings wSettings = new Views.WSettings();
            wSettings.TabMain.SelectedIndex = 3;
            wSettings.Show();
            this.Close();
        }


    }
}
