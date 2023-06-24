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
    /// WBookInfo.xaml 的交互逻辑
    /// </summary>
    public partial class WBookInfo : Window
    {
        public WBookInfo()
        {
            InitializeComponent();
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //左键按下时，允许拖曳
            DragMove();
        }

        private bool IsBookNameTrue(TextBox tb)
        {
            bool result = false;
            if (hasInvalidChar == false && String.IsNullOrWhiteSpace(tb.Text) == false)
            {
                tb.Background = Brushes.White;
                result = true;
            }
            else
            {
                tb.Background = Brushes.Violet;
                result = false;
            }
            return result;
        }

        public bool hasInvalidChar { get; set; } = false;
        private void BookName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (BtnReName == null)
            {
                return;
            }
            string text = (sender as TextBox).Text;
            hasInvalidChar = false;
            foreach (char c in text)
            {
                if (FileIO.invalidCharsInFileName.Contains(c) || (sender as TextBox).Text.Contains('.') == true)
                {
                    hasInvalidChar = true;
                    break;
                }
            }
            if (IsBookNameTrue(sender as TextBox))
            {
                BtnReName.IsEnabled = true;
            }
            else
            {
                BtnReName.IsEnabled = false;
            }
        }

        private void TbPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            double.TryParse(tb.Text, out double str);
            tb.Text = str.ToString();
        }


        private void TbCurrentYear_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            long.TryParse(tb.Text, out long str);
            tb.Text = str.ToString();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateCurrentBookInfo();
        }

        /// <summary>
        /// 更新当前书籍信息
        /// </summary>
        private void UpdateCurrentBookInfo()
        {
            Gval.CurrentBook.Summary = TbSummary.Text;
            Gval.CurrentBook.Price = Convert.ToDouble(TbPrice.Text);
            Gval.CurrentBook.CurrentYear = Convert.ToInt64(TbCurrentYear.Text);
            DataOut.UpdateBookInfo(Gval.CurrentBook);
        }

        private void BtnReName_Click(object sender, RoutedEventArgs e)
        {
            Gval.CurrentBook.Title = TbName.Text;
            DataOut.UpdateBookName(Gval.CurrentBook);
            BtnReName.IsEnabled = false;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}
