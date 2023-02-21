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
    /// WBase.xaml 的交互逻辑
    /// </summary>
    public partial class WBase : Window
    {
        public WBase()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (FunctionsPack.IsInDesignMode(this))
            {
                return;
            }
            LightEditor.Column1.Width = new GridLength(0);
            LightEditor.Row1.Height = new GridLength(0);
            LightEditor.ThisTextEditor.ShowLineNumbers = false;
            LightEditor.DataContext = this.DataContext as Node;
            TbTitle.Text = (this.DataContext as Node).Title;
            TbSummary.Text = (this.DataContext as Node).Summary;
            if (string.IsNullOrEmpty((this.DataContext as Node).Text))
            {
                LightEditor.ThisTextEditor.Text = "　　";
            }
            else
            {
                LightEditor.ThisTextEditor.Text = (this.DataContext as Node).Text;
            }
            (this.DataContext as Node).HasChange = false;
            this.Focus();
            LightEditor.ThisTextEditor.TextArea.Focus();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //左键按下时，允许拖曳
            DragMove();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            (this.DataContext as Node).HasChange = true;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as Node).Title = TbTitle.Text;
            (this.DataContext as Node).Summary = TbSummary.Text;
            LightEditor.BtnSaveText_Click(null, null);
            (this.DataContext as Node).HasChange = false;
        }



        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (BtnSave.IsEnabled == true)
            {
                MessageBoxResult dr = MessageBox.Show("尚未保存\n要在退出前保存更改吗？", "Tip", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Yes);
                if (dr == MessageBoxResult.Yes)
                {
                    BtnSave_Click(null, null);
                }
                if (dr == MessageBoxResult.No)
                {

                }
                if (dr == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            this.Close();
        }


        private void BtnSeeLess_Click(object sender, RoutedEventArgs e)
        {
            GSummary.Visibility = Visibility.Collapsed;
            BtnSeeLess.Visibility = Visibility.Collapsed;
            BtnSeeMore.Visibility = Visibility.Visible;
        }

        private void BtnSeeMore_Click(object sender, RoutedEventArgs e)
        {
            GSummary.Visibility = Visibility.Visible;
            BtnSeeMore.Visibility = Visibility.Collapsed;
            BtnSeeLess.Visibility = Visibility.Visible;
        }
    }
}
