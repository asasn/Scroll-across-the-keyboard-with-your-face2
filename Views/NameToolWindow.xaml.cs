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
using System.Windows.Shapes;

namespace RootNS.Views
{
    /// <summary>
    /// NameToolWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NameToolWindow : Window
    {
        public NameToolWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CbNameQantity_SelectionChanged(null, null);
        }


        public Name Nameer { get; set; } = new Name();


        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            Nameer.Generate();
        }
        private void CbSurnameLength_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Nameer.SurnameLength = CbSurnameLength.SelectedIndex + 1;
        }
        private void CbNameQantity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NameSetBoxPanel == null)
            {
                return;
            }
            Nameer.NameQuantity = CbNameLength.SelectedIndex + 1;
            foreach (NameSetBox box in NameSetBoxPanel.Children)
            {
                if (box.Number > Nameer.NameQuantity)
                {
                    box.Visibility = Visibility.Collapsed;
                }
                else
                {
                    box.Visibility = Visibility.Visible;
                }
            }
        }
        private void CbSuffixLength_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Nameer.SuffixLength = CbSuffixLength.SelectedIndex + 1;
        }
        private void RbStyleNormal_Checked(object sender, RoutedEventArgs e)
        {
            Nameer.Style = Models.Name.StyleValue.常规.ToString();
            CbSuffixLength.SelectedIndex = 0;
            CbNameLength.SelectedIndex = 0;
        }
        private void RbStyleHonor_Checked(object sender, RoutedEventArgs e)
        {
            Nameer.Style = Models.Name.StyleValue.称号.ToString();
            CbNameLength.SelectedIndex = 1;
            CbSuffixLength.SelectedIndex = 1;
        }

        private void RbStylePlace_Checked(object sender, RoutedEventArgs e)
        {
            Nameer.Style = Models.Name.StyleValue.地名.ToString();
            CbNameLength.SelectedIndex = 1;
            CbSuffixLength.SelectedIndex = 0;
        }


        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText((sender as TextBox).Text);
            HandyControl.Controls.Growl.SuccessGlobal("已复制名字到剪贴板！");

            WpShowWord.Children.Clear();
            foreach (char c in (sender as TextBox).Text)
            {
                string pinyin = Nameer.ReadFromPinyinDict(c.ToString());

                TextBox tb = new TextBox() { Margin = new Thickness(2), Text = pinyin.Trim(), IsReadOnly = true };
                WpShowWord.Children.Add(tb);
            }
        }
    }
}
