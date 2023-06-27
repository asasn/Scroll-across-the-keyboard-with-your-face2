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
    /// WDelBook.xaml 的交互逻辑
    /// </summary>
    public partial class WDelBook : Window
    {
        public WDelBook()
        {
            InitializeComponent();
        }
        private void TbVerify_Loaded(object sender, RoutedEventArgs e)
        {
            TbVerify.Focus();
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

        private void BtnVerify_Click(object sender, RoutedEventArgs e)
        {
            if (TbVerify.Text.Equals(TbkVerify.Text) == false)
            {
                return;
            }
            MessageBoxResult dr = MessageBox.Show("最后确认：真的删除吗？\n（将会从书库删除并移至系统回收站）", "Tip", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes);
            if (dr == MessageBoxResult.Yes)
            {
                if (Gval.Views.EditorTabControl.Items.Count > 0)
                {
                    FunctionsPack.ShowMessageBox("请先关闭所有正在编辑的文档！");
                    this.Close();
                    return;
                }
                Gval.CurrentBook.IsDel = true;
                DataOut.UpdateBookInfo(Gval.CurrentBook);
                int n = Gval.BooksBank.IndexOf(Gval.CurrentBook);
                DataOut.DeleteBook(Gval.CurrentBook);
                SqliteHelper.PoolOperate.Remove(Gval.CurrentBook.Guid.ToString());
                FileIO.DeleteFile(Gval.Path.DataDirectory + "/" + Gval.CurrentBook.Guid.ToString() + ".db");
                Gval.BooksBank.Remove(Gval.CurrentBook);
                if (n >= Gval.BooksBank.Count)
                {
                    n = Gval.BooksBank.Count - 1;
                }
                if (Gval.BooksBank.Count > 0)
                {
                    Gval.CurrentBook = Gval.BooksBank[n];
                }
                this.Close();
            }
            if (dr == MessageBoxResult.No)
            {
                this.Close();
                return;
            }
        }

        private void TbVerify_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TbVerify.Text.Equals(TbkVerify.Text))
            {
                BtnVerify.IsEnabled = true;
            }
            else
            {
                BtnVerify.IsEnabled = false;
            }
        }


    }
}
