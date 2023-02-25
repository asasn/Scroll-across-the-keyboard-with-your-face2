using RootNS.Helper;
using RootNS.Models;
using RootNS.Views;
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

namespace RootNS.MyControls
{
    /// <summary>
    /// NodeDoc.xaml 的交互逻辑
    /// </summary>
    public partial class NodeItem : UserControl
    {
        public NodeItem()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void ThisControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Node node = this.DataContext as Node;
            if (node.IsChecked == true ||
                node.IsDir == true)
            {
                return;
            }
            if (node.TypeName == Book.TypeNameEnum.事件记录.ToString())
            {
                WBase wBase = new WBase() { DataContext = node };
                wBase.Show();
            }
            if (node.TypeName == Book.TypeNameEnum.文章片段.ToString() ||
                node.TypeName == Book.TypeNameEnum.全局文章片段.ToString())
            {
                WBase wBase = new WBase() { DataContext = node };
                wBase.Show();
            }
            if (node.TypeName == Book.TypeNameEnum.信息卡.ToString() ||
                node.TypeName == Book.TypeNameEnum.全局信息卡.ToString())
            {
                WCard wCard = new WCard();
                wCard.DataContext = node;
                wCard.Show();
            }
            if (node.TypeName == Book.TypeNameEnum.全局情节设计.ToString())
            {
                WBase wBase = new WBase() { DataContext = node };
                wBase.Show();
            }
            if (node.TypeName == Book.TypeNameEnum.全局题材管理.ToString())
            {
                WBase wBase = new WBase() { DataContext = node };
                wBase.Show();
            }
            if (node.TypeName == Book.TypeNameEnum.全局资料管理.ToString())
            {
                WBase wBase = new WBase() { DataContext = node };
                wBase.Show();
            }
            if (node.TypeName == Book.TypeNameEnum.全局灵感管理.ToString())
            {
                WBase wBase = new WBase() { DataContext = node };
                wBase.Show();
            }
            if (node.TypeName == Book.TypeNameEnum.草稿.ToString() ||
                node.TypeName == Book.TypeNameEnum.作品相关.ToString() ||
                node.TypeName == Book.TypeNameEnum.已发布.ToString())
            {
                //if (node.IsDir == true) { return; }
                Gval.PreviousText = String.Empty;
                if (Gval.OpeningDocList.Contains(node) == false)
                {
                    Gval.OpeningDocList.Add(node);
                }
                EditorHelper.SelectItem(Gval.Views.EditorTabControl, node);
            }

        }

        private void ThisControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            (this.DataContext as Node).IsSelected = true;
        }

        private void TbReName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                (this.DataContext as Node).FinishRename();
                e.Handled = true;//防止触发对应的快捷键
            }
        }

        private void TbReName_LostFocus(object sender, RoutedEventArgs e)
        {
            (this.DataContext as Node).FinishRename();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}
